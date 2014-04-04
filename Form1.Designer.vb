<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UsbCommunication
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla nell'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.RefreshButton = New System.Windows.Forms.Button()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ConnectButton = New System.Windows.Forms.Button()
        Me.CheckConnected = New System.Windows.Forms.CheckBox()
        Me.DisconnectButton = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.ListViewCmd = New System.Windows.Forms.ListView()
        Me.CmdName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.CmdHex = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ArgLen = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Descr = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Argument = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.TimerCRC = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'RefreshButton
        '
        Me.RefreshButton.Location = New System.Drawing.Point(12, 12)
        Me.RefreshButton.Name = "RefreshButton"
        Me.RefreshButton.Size = New System.Drawing.Size(75, 23)
        Me.RefreshButton.TabIndex = 0
        Me.RefreshButton.Text = "Refresh"
        Me.RefreshButton.UseVisualStyleBackColor = True
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(93, 29)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(139, 43)
        Me.ListBox1.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(90, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Devices list"
        '
        'ConnectButton
        '
        Me.ConnectButton.Location = New System.Drawing.Point(12, 41)
        Me.ConnectButton.Name = "ConnectButton"
        Me.ConnectButton.Size = New System.Drawing.Size(75, 23)
        Me.ConnectButton.TabIndex = 3
        Me.ConnectButton.Text = "Connect"
        Me.ConnectButton.UseVisualStyleBackColor = True
        '
        'CheckConnected
        '
        Me.CheckConnected.AutoSize = True
        Me.CheckConnected.Enabled = False
        Me.CheckConnected.Location = New System.Drawing.Point(93, 78)
        Me.CheckConnected.Name = "CheckConnected"
        Me.CheckConnected.Size = New System.Drawing.Size(113, 17)
        Me.CheckConnected.TabIndex = 5
        Me.CheckConnected.Text = "Connection Active"
        Me.CheckConnected.UseVisualStyleBackColor = True
        '
        'DisconnectButton
        '
        Me.DisconnectButton.Location = New System.Drawing.Point(12, 70)
        Me.DisconnectButton.Name = "DisconnectButton"
        Me.DisconnectButton.Size = New System.Drawing.Size(75, 23)
        Me.DisconnectButton.TabIndex = 6
        Me.DisconnectButton.Text = "Disconnect"
        Me.DisconnectButton.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(270, 13)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 9
        Me.Button1.Text = "Send"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(360, 33)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(88, 20)
        Me.TextBox1.TabIndex = 10
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(270, 42)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 11
        Me.Button2.Text = "RXSTATUS"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'ListViewCmd
        '
        Me.ListViewCmd.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.CmdName, Me.CmdHex, Me.ArgLen, Me.Descr})
        Me.ListViewCmd.FullRowSelect = True
        Me.ListViewCmd.GridLines = True
        Me.ListViewCmd.HideSelection = False
        Me.ListViewCmd.Location = New System.Drawing.Point(7, 124)
        Me.ListViewCmd.MultiSelect = False
        Me.ListViewCmd.Name = "ListViewCmd"
        Me.ListViewCmd.Size = New System.Drawing.Size(617, 163)
        Me.ListViewCmd.TabIndex = 14
        Me.ListViewCmd.UseCompatibleStateImageBehavior = False
        Me.ListViewCmd.View = System.Windows.Forms.View.Details
        '
        'CmdName
        '
        Me.CmdName.Text = "CmdName"
        Me.CmdName.Width = 132
        '
        'CmdHex
        '
        Me.CmdHex.Text = "CmdHex"
        Me.CmdHex.Width = 59
        '
        'ArgLen
        '
        Me.ArgLen.Text = "ArgLen"
        Me.ArgLen.Width = 48
        '
        'Descr
        '
        Me.Descr.Text = "Descr"
        Me.Descr.Width = 344
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 108)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 13)
        Me.Label3.TabIndex = 15
        Me.Label3.Text = "Commands"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(357, 13)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(91, 13)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "Custom command"
        '
        'Argument
        '
        Me.Argument.Location = New System.Drawing.Point(468, 33)
        Me.Argument.Name = "Argument"
        Me.Argument.Size = New System.Drawing.Size(156, 20)
        Me.Argument.TabIndex = 17
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(465, 13)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(102, 13)
        Me.Label5.TabIndex = 18
        Me.Label5.Text = "Command Argument"
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(468, 59)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(99, 23)
        Me.Button3.TabIndex = 19
        Me.Button3.Text = "Generate CRC"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(467, 88)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(100, 17)
        Me.CheckBox1.TabIndex = 20
        Me.CheckBox1.Text = "BruteForceCRC"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'TimerCRC
        '
        Me.TimerCRC.Interval = 10000
        '
        'UsbCommunication
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(634, 296)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Argument)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ListViewCmd)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.DisconnectButton)
        Me.Controls.Add(Me.CheckConnected)
        Me.Controls.Add(Me.ConnectButton)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ListBox1)
        Me.Controls.Add(Me.RefreshButton)
        Me.Name = "UsbCommunication"
        Me.Text = "UsbXpress communication"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RefreshButton As System.Windows.Forms.Button
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ConnectButton As System.Windows.Forms.Button
    Friend WithEvents CheckConnected As System.Windows.Forms.CheckBox
    Friend WithEvents DisconnectButton As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents ListViewCmd As System.Windows.Forms.ListView
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Argument As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Protected WithEvents CmdName As System.Windows.Forms.ColumnHeader
    Protected WithEvents CmdHex As System.Windows.Forms.ColumnHeader
    Protected WithEvents ArgLen As System.Windows.Forms.ColumnHeader
    Protected WithEvents Descr As System.Windows.Forms.ColumnHeader
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents TimerCRC As System.Windows.Forms.Timer

End Class
