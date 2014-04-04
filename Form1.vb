Imports System.Text
Imports System.Threading
Imports System
Imports System.IO
Imports System.Collections
Imports System.Collections.Generic

Public Class UsbCommunication
    Const BYTESTOREAD = 64
    Const BYTESTOWRITE = 64
    '    Const PATH = "C:\Users\ste\Documents\Visual Studio 2013\Projects\UsbExpresscomm\"
    Const CSVFILENAME = "\commandslist.csv"
    Const LOGFILENAME = "\logfile.txt"
    'lists of errorcodes of dll
    Private Enum ErrorCodes As Byte
        SI_SUCCESS = &H0
        SI_DEVICE_NOT_FOUND = &HFF
        SI_INVALID_HANDLE = &H1
        SI_READ_ERROR = &H2
        SI_RX_QUEUE_NOT_READY = &H3
        SI_WRITE_ERROR = &H4
        SI_RESET_ERROR = &H5
        SI_INVALID_PARAMETER = &H6
        SI_INVALID_REQUEST_LENGTH = &H7
        SI_DEVICE_IO_FAILED = &H8
        SI_INVALID_BAUDRATE = &H9
        SI_FUNCTION_NOT_SUPPORTED = &HA
        SI_GLOBAL_DATA_ERROR = &HB
        SI_SYSTEM_ERROR_CODE = &HC
        SI_READ_TIMED_OUT = &HD
        SI_WRITE_TIMED_OUT = &HE
        SI_IO_PENDING = &HF
    End Enum

    Private Enum RxQueueCodes As Byte
        SI_RX_NO_OVERRUN = &H0
        SI_RX_EMPTY = &H0
        SI_RX_READY = &H2
    End Enum

    Dim ErrorCode As Byte               'error code used for every call to dll
    Dim ConnectionPtr As IntPtr         'handle of USB connection
    Dim Connected As Boolean = False    'USB connected flag
    Private USBTimer As Threading.Timer 'Timer for cycling USB reading
    '   Private CRCTimer As Threading.Timer 'Timer for cycling CRC reading
    Dim IOoverlapped As New Overlapped  'overlapped object for USBXpress (not used)
    Dim IOPtr As IntPtr                 'pointer to overlapped object
    Dim LastChangedBySoftware As Boolean = False 'used to detect whether "custom command" has been changed by user
    Dim ChangedBySoftware As Boolean = False
    Dim file As System.IO.StreamWriter
    Dim path As String = System.Windows.Forms.Application.StartupPath
    Dim SearchForString As Boolean = False
    Dim CRCTimeMultiplier As UInteger = 0

    'Initialization, on Form LOAD
    Private Sub UsbCommunication_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim hiVer, loVer As UInteger
        ListBox1.Items.Clear()                  'Empty listbox
        DisplayConnected(0)                     'Set not connected, no devices listed
        Console.Beep()                          'Old style beep from console
        Console.WriteLine("DLL version:")
        ErrorCode = SIUSBXP.SI_GetDLLVersion(hiVer, loVer) 'get dll and driver version
        PrintError()
        If ErrorCode = 0 Then
            Console.WriteLine(((hiVer >> 16) And &HFFFF) & "." & (hiVer And &HFFFF) & "." & ((loVer >> 16 And &HFFFF)) & "." & (loVer And &HFFFF))
        End If
        PrintError()
        IOPtr = IOoverlapped.EventHandleIntPtr                               'initialize Overlapped object pointer
        USBTimer = New Threading.Timer(AddressOf UsbTick, Nothing, 100, 100) 'Start timer for USB reading
        '        CRCTimer = New Threading.Timer(AddressOf CRCTick, Nothing, 7250, 7250) 'Start timer for USB reading
        PopulateListView()
        CreateLogFile()
    End Sub

    Private Sub CreateLogFile()
        file = My.Computer.FileSystem.OpenTextFileWriter(path & LOGFILENAME, True)
        file.WriteLine(System.DateTime.Now)
    End Sub


    Private Sub PopulateListView()      'reads list of commands from .csv file
        Dim currentRow As String()
        Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(path & CSVFILENAME)
            MyReader.Delimiters = New String() {","}
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.HasFieldsEnclosedInQuotes = False
            MyReader.TrimWhiteSpace = True
            While Not MyReader.EndOfData
                currentRow = MyReader.ReadFields()
                Dim item As ListViewItem
                item = ListViewCmd.Items.Add(currentRow(0))
                Dim ix%
                For ix = 1 To UBound(currentRow)
                    item.SubItems.Add(currentRow(ix))
                Next
            End While
        End Using
    End Sub

    'Subroutine that runs every clock tick (on a different thread)
    Private Sub UsbTick(ByVal state As Object)
        Me.UsbTickOnMainThread()
    End Sub

    Private Sub UsbTickOnMainThread() 'on main thread
        If InvokeRequired Then                                  'code neede for threading timer
            BeginInvoke(New MethodInvoker(AddressOf UsbTickOnMainThread))
        Else
            If Connected Then
                ReadUsb()
                If SearchForString Then
                    CRCTimeMultiplier = CRCTimeMultiplier + 1
                    If CRCTimeMultiplier = 15 Then
                        CRCTimeMultiplier = 0
                        CrcChange()
                    End If
                End If
            End If
        End If
    End Sub

    'Decode error code from SiUSBXp.dll
    Private Sub PrintError()
        If ErrorCode <> &H0 Then
            Console.WriteLine([Enum].GetName(GetType(ErrorCodes), ErrorCode))         'output in console error code
        End If
    End Sub

    'Click on refresh button
    Private Sub RefreshButton_Click(sender As Object, e As EventArgs) Handles RefreshButton.Click
        Refresh_list()
    End Sub

    'Populate list of USB devices
    Private Sub Refresh_list()
        'Variables needed for connecting via usb
        Dim NumDev As UInteger
        Dim i As UInteger
        Dim SerialNum As New StringBuilder
        SerialNum.Clear()
        SerialNum.Capacity() = 500
        Console.WriteLine("Devices list:")
        ErrorCode = SIUSBXP.SI_GetNumDevices(NumDev)                                         'enumerate connected devices
        ListBox1.Items.Clear()
        PrintError()
        If NumDev <> 0 Then
            For i = 0 To NumDev - 1
                ErrorCode = SIUSBXP.SI_GetProductString(i, SerialNum, CType(0, Byte))        'build list of connected devices
                PrintError()
                ListBox1.Items.Add(SerialNum.ToString)
                Console.WriteLine(i & " " & ListBox1.Items.Item(i))                          'ouptut in console serial number of device
            Next i
            DisplayConnected(1)
        Else
            DisplayConnected(0)
        End If
    End Sub

    Private Sub ConnectButton_Click(sender As Object, e As EventArgs) Handles ConnectButton.Click
        Console.WriteLine("Connecting to " & ListBox1.SelectedItem & "...")                           'ouptut in console serial number of device
        ErrorCode = SIUSBXP.SI_Open(CType(ListBox1.SelectedIndex, UInteger), ConnectionPtr)           'open serial device
        PrintError()
        If ErrorCode = &H0 Then
            DisplayConnected(2)                                                                       'set UI elements to connected
            Console.WriteLine("Connected, setting timeouts")
            ErrorCode = SIUSBXP.SI_SetTimeouts(10, 10)                                                'set USB connection timeout
            PrintError()
            Console.WriteLine("Flushing buffers")
            SIUSBXP.SI_FlushBuffers(ConnectionPtr, 1, 1)
            PrintError()
        Else
            DisplayConnected(0)                                                                       'refresh must be done
        End If
    End Sub

    Private Sub DisconnectButton_Click(sender As Object, e As EventArgs) Handles DisconnectButton.Click
        Disconnect_device()
    End Sub

    Private Sub Disconnect_device()
        DisplayConnected(0)                             'refresh must be done
        ErrorCode = SIUSBXP.SI_Close(ConnectionPtr)
        PrintError()
        Console.WriteLine("Communication closed")
        ConnectionPtr = Nothing
    End Sub

    'Check if there is a selected element in the list of devices
    Private Sub DevicesList_click(sender As Object, e As EventArgs) Handles ListBox1.Click
        If ListBox1.SelectedItems.Count = 1 Then
            DisplayConnected(1)
        Else
            DisplayConnected(0)
        End If
    End Sub

    Private Sub ReadUsb()                                       'function for reading received usb bytes
        Dim readBuf(BYTESTOREAD) As Byte
        Dim readBytes As UInteger
        Dim readString As String
   
            'read serial
            ErrorCode = SIUSBXP.SI_Read(ConnectionPtr, readBuf, BYTESTOREAD, readBytes, IOPtr)
            If ErrorCode = 0 Then
                readString = Nothing
                For i = 0 To readBytes - 1                        'read received payload and reverse (MSB to LSB)
                    '                                               readString = (Format(readBuf(i), "x")).PadLeft(2, "0") & " " & readString
                    'bytes not reversed:
                    readString = readString & " " & (Format(readBuf(i), "x")).PadLeft(2, "0")
                Next i
                Console.WriteLine(readBytes & " bytes read: " & readString)
                file.WriteLine(readString)

                ElseIf ErrorCode = &HD Then         'omit "timeout" errors (nothing to read)
                    'nothing
                Else
                    PrintError()
                    Disconnect_device()           'error, disconnect device
                End If

            If SearchForString Then
            If (Format(readBuf(0), "x")).PadLeft(2, "0") & (Format(readBuf(1), "x")).PadLeft(2, "0") = "5b25" Then
                Console.WriteLine("Preamble FOUND!!!")
                file.WriteLine("Preamble FOUND!!!")
            End If
        End If
    End Sub

    Private Sub DisplayConnected(ConStatus As Byte) 'function to align button status
        'ConStatus = 0 --> not connected, no USB devices listed
        'ConStatus = 1 --> not connected, USB devices available
        'ConStatus = 2 --> connected
        If ConStatus = 0 Then
            ConnectButton.Enabled() = False
            DisconnectButton.Enabled() = False
            Connected = False
            CheckConnected.Checked = False
            ListBox1.Items.Clear()
            Button1.Enabled = False
        ElseIf ConStatus = 1 Then
            ConnectButton.Enabled() = True
            DisconnectButton.Enabled() = False
            Connected = False
            CheckConnected.Checked = False
            Button1.Enabled = False
        ElseIf ConStatus = 2 Then
            ConnectButton.Enabled() = False
            DisconnectButton.Enabled() = True
            Connected = True
            CheckConnected.Checked = True
            Button1.Enabled = True
        End If
    End Sub

    'Write  HEX command through USB port
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim StringToWrite As String
        Dim WrittenBytes, i As Integer
        If TextBox1.Text.Length <> 0 Then
            If TextBox1.Text.Length > 4 Then   'if a string of bytes must be written, flip LSB to MSB
                StringToWrite = TextBox1.Text.Remove(4, TextBox1.Text.Length - 4)
                For i = TextBox1.Text.Length - 1 To 4 Step -2
                    StringToWrite = StringToWrite & Mid(TextBox1.Text, i, 2)
                Next i
            Else
                'convert textbox in an array of bytes
                StringToWrite = TextBox1.Text
            End If
            WrittenBytes = WriteHexByteFromStr(StringToWrite)
            If ErrorCode = 0 Then
                If ChangedBySoftware Then                                                                                       'output in console issued command
                    Console.WriteLine("Command: " & ListViewCmd.SelectedItems.Item(0).SubItems(0).Text)
                End If
                Console.WriteLine(WrittenBytes & " bytes sent: " & TextBox1.Text.PadLeft(TextBox1.Text.Length, "0"))             'output in console byte written
            Else
                Disconnect_device()
            End If
        End If
    End Sub

    Private Function WriteHexByteFromStr(text As String) As UInteger
        Dim i As Integer
        Dim HEXstr As String
        Dim WriteBuf(BYTESTOWRITE) As Byte
        Dim WrittenBytes As UInteger
        'convert textbox in an array of bytes
        For i = 0 To (text.Length - 2) Step 2
            HEXstr = "&H" & Mid(text, i + 1, 2)
            WriteBuf(i / 2) = CInt(HEXstr)
        Next
        ErrorCode = SIUSBXP.SI_Write(ConnectionPtr, WriteBuf, i / 2, WrittenBytes, IOPtr)                                   'send HEX command
        PrintError()
        Return WrittenBytes
    End Function

    'Query USB buffer status
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim inQueque, qStatus As UInteger
        ErrorCode = SIUSBXP.SI_CheckRXQueue(ConnectionPtr, inQueque, qStatus)
        PrintError()
        Console.WriteLine(inQueque & " , " & qStatus)
    End Sub

    Private Sub ListViewCmd_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewCmd.SelectedIndexChanged
        RefreshAutoCmd()
    End Sub

    Private Sub Argument_TextChanged(sender As Object, e As EventArgs) Handles Argument.TextChanged
        RefreshAutoCmd()
    End Sub

    Private Sub RefreshAutoCmd()
        Dim dumStr As String
        If ListViewCmd.SelectedIndices.Count > 0 Then
            With ListViewCmd.SelectedItems.Item(0)
                If Argument.TextLength >= CInt(.SubItems(2).Text * 2) Then
                    dumStr = Argument.Text.Substring(0, CInt(.SubItems(2).Text * 2))
                Else
                    dumStr = Argument.Text
                End If
                LastChangedBySoftware = True
                TextBox1.Text = .SubItems(1).Text & dumStr
            End With
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If LastChangedBySoftware Then
            ChangedBySoftware = True
        Else
            ChangedBySoftware = False
        End If
        LastChangedBySoftware = False
    End Sub

    Private Sub UsbCommunication_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        file.Close()
    End Sub

    'CRC elaboration
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim payload As Byte() = {0, 0} ', &H55, &HF0, &H55, &H5B, &H25, &H7, &H0, &H3, &H1, &HA, &H64, &H7B, &H34, &H81, &H32, &H12, &H45, &H38}
        Dim initvalue As UInt16 = &HFFFF
        Dim k0, k1, k2 As Integer
        For k0 = 0 To 255
            ' For k1 = 0 To 255
            '    For k2 = 0 To 255
            payload(0) = k0
            payload(1) = k1
            '  payload(2) = k2
            If crcCalculate(payload, initvalue) = &H82CF Then
                Console.WriteLine(Hex(k0) & " " & Hex(k1) & " " & Hex(k2))
                file.WriteLine("""" & Hex(k0).PadLeft(2, "0") & Hex(k1).PadLeft(2, "0") & Hex(k2).PadLeft(2, "0") & """, ")
            End If
            'Next k2
            ' Next k1
        Next k0
        'Console.WriteLine(Hex(crcCalculate(payload, initvalue) And &HFFFF))
    End Sub


    '    /*****************************************************/
    '    /*  CRC table for polynomial Poly=0x1021 CCITT-FALSE */
    '    /*****************************************************/


    Dim crctab As UInt16() = {
   &H0, &H1021, &H2042, &H3063, &H4084, &H50A5, &H60C6, &H70E7,
 &H8108, &H9129, &HA14A, &HB16B, &HC18C, &HD1AD, &HE1CE, &HF1EF,
 &H1231, &H210, &H3273, &H2252, &H52B5, &H4294, &H72F7, &H62D6,
 &H9339, &H8318, &HB37B, &HA35A, &HD3BD, &HC39C, &HF3FF, &HE3DE,
 &H2462, &H3443, &H420, &H1401, &H64E6, &H74C7, &H44A4, &H5485,
 &HA56A, &HB54B, &H8528, &H9509, &HE5EE, &HF5CF, &HC5AC, &HD58D,
 &H3653, &H2672, &H1611, &H630, &H76D7, &H66F6, &H5695, &H46B4,
 &HB75B, &HA77A, &H9719, &H8738, &HF7DF, &HE7FE, &HD79D, &HC7BC,
 &H48C4, &H58E5, &H6886, &H78A7, &H840, &H1861, &H2802, &H3823,
 &HC9CC, &HD9ED, &HE98E, &HF9AF, &H8948, &H9969, &HA90A, &HB92B,
 &H5AF5, &H4AD4, &H7AB7, &H6A96, &H1A71, &HA50, &H3A33, &H2A12,
 &HDBFD, &HCBDC, &HFBBF, &HEB9E, &H9B79, &H8B58, &HBB3B, &HAB1A,
 &H6CA6, &H7C87, &H4CE4, &H5CC5, &H2C22, &H3C03, &HC60, &H1C41,
 &HEDAE, &HFD8F, &HCDEC, &HDDCD, &HAD2A, &HBD0B, &H8D68, &H9D49,
 &H7E97, &H6EB6, &H5ED5, &H4EF4, &H3E13, &H2E32, &H1E51, &HE70,
 &HFF9F, &HEFBE, &HDFDD, &HCFFC, &HBF1B, &HAF3A, &H9F59, &H8F78,
 &H9188, &H81A9, &HB1CA, &HA1EB, &HD10C, &HC12D, &HF14E, &HE16F,
 &H1080, &HA1, &H30C2, &H20E3, &H5004, &H4025, &H7046, &H6067,
 &H83B9, &H9398, &HA3FB, &HB3DA, &HC33D, &HD31C, &HE37F, &HF35E,
 &H2B1, &H1290, &H22F3, &H32D2, &H4235, &H5214, &H6277, &H7256,
 &HB5EA, &HA5CB, &H95A8, &H8589, &HF56E, &HE54F, &HD52C, &HC50D,
 &H34E2, &H24C3, &H14A0, &H481, &H7466, &H6447, &H5424, &H4405,
 &HA7DB, &HB7FA, &H8799, &H97B8, &HE75F, &HF77E, &HC71D, &HD73C,
 &H26D3, &H36F2, &H691, &H16B0, &H6657, &H7676, &H4615, &H5634,
 &HD94C, &HC96D, &HF90E, &HE92F, &H99C8, &H89E9, &HB98A, &HA9AB,
 &H5844, &H4865, &H7806, &H6827, &H18C0, &H8E1, &H3882, &H28A3,
 &HCB7D, &HDB5C, &HEB3F, &HFB1E, &H8BF9, &H9BD8, &HABBB, &HBB9A,
 &H4A75, &H5A54, &H6A37, &H7A16, &HAF1, &H1AD0, &H2AB3, &H3A92,
 &HFD2E, &HED0F, &HDD6C, &HCD4D, &HBDAA, &HAD8B, &H9DE8, &H8DC9,
 &H7C26, &H6C07, &H5C64, &H4C45, &H3CA2, &H2C83, &H1CE0, &HCC1,
 &HEF1F, &HFF3E, &HCF5D, &HDF7C, &HAF9B, &HBFBA, &H8FD9, &H9FF8,
 &H6E17, &H7E36, &H4E55, &H5E74, &H2E93, &H3EB2, &HED1, &H1EF0
}

    '//=========================================================================
    '//  Description:   Calculate a CRC on a buffer
    '//  Parameters:    buffer - address of buffer
    '//                 length - length of buffer
    '//  Returns:       CRC
    '//=========================================================================

    Private Function crcCalculate(Payload() As Byte, initvalue As UInt16) As UInt16
        Dim crc As UInt16 = initvalue
        Dim i As Integer
        For i = 0 To UBound(Payload)
            crc = crctab((crc >> 8) Xor (Payload(i)) And &HFF) Xor (crc << 8)
        Next i
        Return (crc)
    End Function

    'list of leading string that generates correct crc
    Dim LeadingBytes As String() = {
"00CCD2",
"01DCF3",
"02EC90",
"03FCB1",
"048C56",
"059C77",
"06AC14",
"07BC35",
"084DDA",
"095DFB",
"0A6D98",
"0B7DB9",
"0C0D5E",
"0D1D7F",
"0E2D1C",
"0F3D3D",
"10DEE3",
"11CEC2",
"12FEA1",
"13EE80",
"149E67",
"158E46",
"16BE25",
"17AE04",
"185FEB",
"194FCA",
"1A7FA9",
"1B6F88",
"1C1F6F",
"1D0F4E",
"1E3F2D",
"1F2F0C",
"20E8B0",
"21F891",
"22C8F2",
"23D8D3",
"24A834",
"25B815",
"268876",
"279857",
"2869B8",
"297999",
"2A49FA",
"2B59DB",
"2C293C",
"2D391D",
"2E097E",
"2F195F",
"30FA81",
"31EAA0",
"32DAC3",
"33CAE2",
"34BA05",
"35AA24",
"369A47",
"378A66",
"387B89",
"396BA8",
"3A5BCB",
"3B4BEA",
"3C3B0D",
"3D2B2C",
"3E1B4F",
"3F0B6E",
"408416",
"419437",
"42A454",
"43B475",
"44C492",
"45D4B3",
"46E4D0",
"47F4F1",
"48051E",
"49153F",
"4A255C",
"4B357D",
"4C459A",
"4D55BB",
"4E65D8",
"4F75F9",
"509627",
"518606",
"52B665",
"53A644",
"54D6A3",
"55C682",
"56F6E1",
"57E6C0",
"58172F",
"59070E",
"5A376D",
"5B274C",
"5C57AB",
"5D478A",
"5E77E9",
"5F67C8",
"60A074",
"61B055",
"628036",
"639017",
"64E0F0",
"65F0D1",
"66C0B2",
"67D093",
"68217C",
"69315D",
"6A013E",
"6B111F",
"6C61F8",
"6D71D9",
"6E41BA",
"6F519B",
"70B245",
"71A264",
"729207",
"738226",
"74F2C1",
"75E2E0",
"76D283",
"77C2A2",
"78334D",
"79236C",
"7A130F",
"7B032E",
"7C73C9",
"7D63E8",
"7E538B",
"7F43AA",
"805D5A",
"814D7B",
"827D18",
"836D39",
"841DDE",
"850DFF",
"863D9C",
"872DBD",
"88DC52",
"89CC73",
"8AFC10",
"8BEC31",
"8C9CD6",
"8D8CF7",
"8EBC94",
"8FACB5",
"904F6B",
"915F4A",
"926F29",
"937F08",
"940FEF",
"951FCE",
"962FAD",
"973F8C",
"98CE63",
"99DE42",
"9AEE21",
"9BFE00",
"9C8EE7",
"9D9EC6",
"9EAEA5",
"9FBE84",
"A07938",
"A16919",
"A2597A",
"A3495B",
"A439BC",
"A5299D",
"A619FE",
"A709DF",
"A8F830",
"A9E811",
"AAD872",
"ABC853",
"ACB8B4",
"ADA895",
"AE98F6",
"AF88D7",
"B06B09",
"B17B28",
"B24B4B",
"B35B6A",
"B42B8D",
"B53BAC",
"B60BCF",
"B71BEE",
"B8EA01",
"B9FA20",
"BACA43",
"BBDA62",
"BCAA85",
"BDBAA4",
"BE8AC7",
"BF9AE6",
"C0159E",
"C105BF",
"C235DC",
"C325FD",
"C4551A",
"C5453B",
"C67558",
"C76579",
"C89496",
"C984B7",
"CAB4D4",
"CBA4F5",
"CCD412",
"CDC433",
"CEF450",
"CFE471",
"D007AF",
"D1178E",
"D227ED",
"D337CC",
"D4472B",
"D5570A",
"D66769",
"D77748",
"D886A7",
"D99686",
"DAA6E5",
"DBB6C4",
"DCC623",
"DDD602",
"DEE661",
"DFF640",
"E031FC",
"E121DD",
"E211BE",
"E3019F",
"E47178",
"E56159",
"E6513A",
"E7411B",
"E8B0F4",
"E9A0D5",
"EA90B6",
"EB8097",
"ECF070",
"EDE051",
"EED032",
"EFC013",
"F023CD",
"F133EC",
"F2038F",
"F313AE",
"F46349",
"F57368",
"F6430B",
"F7532A",
"F8A2C5",
"F9B2E4",
"FA8287",
"FB92A6",
"FCE241",
"FDF260",
"FEC203",
"FFD222"}


    Dim TestedCrcCount As UInteger = 70
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            Console.WriteLine("CRC scan mode ON")
            file.WriteLine("CRC scan mode ON")
            SearchForString = True
        Else
            SearchForString = False
            Console.WriteLine("CRC scan mode OFF")
            file.WriteLine("CRC scan mode OFF")
        End If
    End Sub

    Private Sub CrcChange()
        Dim strCommand As String
        Console.WriteLine("Setting address to " & LeadingBytes(TestedCrcCount) & " | " & TestedCrcCount & " of " & UBound(LeadingBytes))
        file.WriteLine("Setting address to " & LeadingBytes(TestedCrcCount))
        'xx xx xx f0 55 5b 25 07 ....
        strCommand = ("2c05" & LeadingBytes(TestedCrcCount).Remove(2, 4) & "f0555b25")
        TextBox1.Text = strCommand
        Button1.PerformClick()
        'writtenbytes = WriteHexByteFromStr(strCommand)
        'Console.WriteLine(writtenbytes & " Bytes written")
        ' flush rx
        'writtenbytes = WriteHexByteFromStr("35")
        'find 5b25
        If TestedCrcCount = UBound(LeadingBytes) Then
            TestedCrcCount = 0
        Else
            TestedCrcCount = TestedCrcCount + 1
        End If
        '   End If
    End Sub
End Class