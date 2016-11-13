Imports System.IO
Imports System.Management
Imports System.Net
Imports System.Net.NetworkInformation

Public Class Form1
    Private seleccionPuertos As String
    Private usbDevices As New ArrayList
    Private usbDevice As UsbDevice
    Private rutaEscritorio As String
    Private consultadaMemoria As Boolean = False
    'getter rutaescritorio
    Public ReadOnly Property getRutaEscritorio() As String
        Get
            Return rutaEscritorio
        End Get
    End Property
    Sub GetSerialPortNames()
        ' Show all available COM ports.
        For Each sp As String In My.Computer.Ports.SerialPortNames
            ListBox1.Items.Add(sp)
        Next
    End Sub
    Sub GetUsbDevices()
        Dim moReturn As Management.ManagementObjectCollection
        Dim moSearch As Management.ManagementObjectSearcher
        Dim mo As Management.ManagementObject
        moSearch = New Management.ManagementObjectSearcher("Select * from Win32_USBControllerDevice")
        moReturn = moSearch.Get
        For Each mo In moReturn
            Dim moReturnDevice As Management.ManagementObjectCollection
            Dim moSearchDevice As Management.ManagementObjectSearcher
            Dim moDevice As Management.ManagementObject
            Dim strDeviceName As String = mo("Dependent").ToString.Replace(""""c, "")
            Dim strDevice As String = strDeviceName.Substring(strDeviceName.IndexOf("=") + 1)
            moSearchDevice = New Management.ManagementObjectSearcher("Select * From Win32_PnPEntity Where DeviceID = '" & strDevice & "'")
            moReturnDevice = moSearchDevice.Get
            For Each moDevice In moReturnDevice
                Dim strDeviceID As String
                strDeviceID = moDevice("DeviceID")
                strDeviceID.ToUpper()
                ListBox1.Items.Add(moDevice("Description"))
                Dim usbDevice As New UsbDevice(strDeviceID, moDevice("Description"), moDevice("Status"))
                usbDevices.Add(usbDevice)
                'MessageBox.Show("DeviceID is " + strDeviceID)
                'MessageBox.Show(String.Format("{0} - {1} ", moDevice("Description").ToString, moDevice("Status").ToString))
            Next
        Next
    End Sub
    'radio button usb
    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        GroupBoxUsb.Enabled = Enabled
        seleccionPuertos = "USB"
    End Sub
    'radio button COM
    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        seleccionPuertos = "COM"
    End Sub
    'listbox usb com
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If seleccionPuertos = "USB" Then
            For Each usbDeviceSelected As UsbDevice In usbDevices
                'como busco por descripción, algunos son iguales, 
                'así que solo se guarda el primero que encuentre,
                'mejor sería hacerlo por ID pero para una demo vale
                If usbDeviceSelected.getDescripcion = ListBox1.SelectedItem Then
                    usbDevice = usbDeviceSelected
                End If
            Next
            TextBoxUsb.AppendText(usbDevice.getDescripcion & vbCrLf)
        End If
    End Sub
    'radio button usb ID
    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        If RadioButton3.Checked Then
            If ListBox1.SelectedItem <> Nothing Then
                Dim usbDescription As String = ListBox1.SelectedItem
                TextBoxUsb.AppendText(usbDevice.getDeviceID & vbCrLf)
            End If
        End If
    End Sub
    'radio button usb status
    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged
        If RadioButton4.Checked Then
            If ListBox1.SelectedItem <> Nothing Then
                Dim usbDescription As String = ListBox1.SelectedItem
                TextBoxUsb.AppendText("- " & usbDevice.getStatus & vbCrLf)
            End If
        End If
    End Sub
    'Botón get puertos
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.UseWaitCursor = True
        Me.Cursor = Cursors.WaitCursor
        ListBox1.Items.Clear()
        Select Case seleccionPuertos
            Case Nothing
                MessageBox.Show("Seleccione Puertos COM o Dispositivos USB")
            Case "COM"
                GetSerialPortNames()
            Case "USB"
                GetUsbDevices()
        End Select
        Me.UseWaitCursor = False
        Me.Cursor = Cursors.Arrow

    End Sub
    'Botón get Gráfica
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim graphics_query As String = "SELECT * FROM Win32_VideoController"
            Dim graphics_searcher As New ManagementObjectSearcher(graphics_query)
            For Each info As ManagementObject In graphics_searcher.Get()
                TextBox1.Text = info.Properties("Name").Value.ToString() & " (" & info.Properties("CurrentHorizontalResolution").Value.ToString() & " x " & info.Properties("CurrentVerticalResolution").Value.ToString() & ")"
            Next info
            TextBox1.Enabled = False
        Catch ex As Exception
            MessageBox.Show("Esta versión es un poco antigua, solo veras la resolución" & vbCrLf & ex.ToString)
            Dim screenWidth As Integer = Screen.PrimaryScreen.Bounds.Width
            Dim screenHeight As Integer = Screen.PrimaryScreen.Bounds.Height
            TextBox1.Text = screenWidth.ToString & " X " & screenHeight.ToString
            TextBox1.Enabled = False
        End Try
    End Sub
    'Botón get IP
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Ip address
        Dim hostName As String = System.Net.Dns.GetHostName()
        TextBox2.Text = hostName
        TextBox2.Enabled = False
        TextBox3.Text = System.Net.Dns.GetHostByName(hostName).AddressList(0).ToString()
        TextBox3.Enabled = False
        'gateway
        Dim Interfaces As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim adapter As NetworkInterface
        Dim myAdapterProps As IPInterfaceProperties = Nothing
        Dim myGateways As GatewayIPAddressInformationCollection = Nothing
        For Each adapter In Interfaces
            If adapter.NetworkInterfaceType = NetworkInterfaceType.Loopback Then
                Continue For
            End If
            ' TextBox1.AppendText(adapter.Name & Environment.NewLine)
            'TextBox1.AppendText(adapter.Description & Environment.NewLine)
            myAdapterProps = adapter.GetIPProperties
            myGateways = myAdapterProps.GatewayAddresses
            ' Dim IPInfo As UnicastIPAddressInformationCollection = adapter.GetIPProperties().UnicastAddresses
            ' Dim properties As IPInterfaceProperties = adapter.GetIPProperties()
            'For Each IPAddressInfo As UnicastIPAddressInformation In IPInfo
            'TextBox1.AppendText("IP Address : " & IPAddressInfo.Address.ToString & Environment.NewLine)
            'TextBox1.AppendText("Subnet Mask :" & IPAddressInfo.IPv4Mask.ToString & Environment.NewLine)
            'Next

            For Each Gateway As GatewayIPAddressInformation In myGateways
                TextBox8.AppendText(Gateway.Address.ToString & Environment.NewLine)
            Next
            TextBox8.Enabled = False

            'TextBox1.AppendText("DNS Address :" & properties.DnsAddresses.ToString & Environment.NewLine)
        Next
    End Sub
    'Botón CPU
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.UseWaitCursor = True
        Me.Cursor = Cursors.WaitCursor
        'https://msdn.microsoft.com/en-us/library/aa394373(v=vs.85).aspx

        Dim objMOS As ManagementObjectSearcher
        Dim objMOC As Management.ManagementObjectCollection
        Dim objMO As Management.ManagementObject
        objMOS = New ManagementObjectSearcher("Select * From Win32_Processor")
        objMOC = objMOS.Get
        For Each objMO In objMOC
            TextBox4.Text = (CDbl(objMO("MaxClockSpeed")) / 1000)
            Label5.Text = ((objMO("Name")))
            Label6.Text = ((objMO("NumberOfCores")) & " Núcleos ")
            Label5.ForeColor = Color.Red
            Label6.ForeColor = Color.Red

        Next
        TextBox4.Enabled = False
        objMOS.Dispose()
        objMOS = Nothing
        objMO.Dispose()
        objMO = Nothing

        Me.UseWaitCursor = False
        Me.Cursor = Cursors.Arrow
    End Sub
    'Menu Guardar
    Private Sub GuardartxtToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GuardartxtToolStripMenuItem.Click
        If TextBox6.Text <> "" Then
            Dim texto As String
            texto = "USUARIO" & vbCrLf
            texto += "-------" & vbCrLf
            texto += "Dominio\Usuario" & vbCrLf
            texto += TextBox5.Text & vbCrLf
            texto += "Ruta Escritorio" & vbCrLf
            texto += TextBox6.Text & vbCrLf
            texto += vbCrLf
            texto += "Fecha y Hora" & vbCrLf
            texto += "----- - ----" & vbCrLf
            texto += Label10.Text & vbCrLf
            texto += vbCrLf
            texto += ("USB" & vbCrLf)
            texto += "---" & vbCrLf
            texto += TextBoxUsb.Text & vbCrLf
            texto += vbCrLf
            texto += ("CPU" & vbCrLf)
            texto += "---" & vbCrLf
            texto += Label5.Text & vbCrLf
            texto += Label6.Text & vbCrLf
            texto += "MaxFrec: " & TextBox4.Text & vbCrLf
            texto += vbCrLf
            texto += ("Memoria" & vbCrLf)
            texto += "-------" & vbCrLf
            texto += Label11.Text & vbCrLf
            texto += Label12.Text & vbCrLf
            texto += Label13.Text & vbCrLf
            texto += Label14.Text & vbCrLf
            texto += vbCrLf
            texto += "Gráfica" & vbCrLf
            texto += "-------" & vbCrLf
            texto += TextBox1.Text & vbCrLf
            texto += vbCrLf
            texto += ("RED" & vbCrLf)
            texto += "---" & vbCrLf
            texto += TextBox2.Text & vbCrLf
            texto += "IP:      " & TextBox3.Text & vbCrLf
            texto += "Gateway: " & TextBox8.Text & vbCrLf
            texto += TextBox7.Text & vbCrLf

            Dim guardarForm As New nombreFicheroForm(Me, texto)
            guardarForm.Show()
        Else
            MessageBox.Show("Haz get de Usuario para saber la ruta al escritorio")
        End If
    End Sub
    'Menu Salir
    Private Sub SalirToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SalirToolStripMenuItem.Click
        Me.Dispose()
    End Sub
    'Menu Acerca de
    Private Sub AcercaDeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AcercaDeToolStripMenuItem.Click
        Dim formAyuda As New FormAyuda
        formAyuda.Show()
    End Sub
    'Botón User
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        TextBox5.Text = My.User.Name
        TextBox6.Text = My.Computer.FileSystem.SpecialDirectories.Desktop
        rutaEscritorio = (My.Computer.FileSystem.SpecialDirectories.Desktop & "\")
    End Sub
    'Botón Ping
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim i As Integer
        Dim eco As New System.Net.NetworkInformation.Ping
        Dim res As System.Net.NetworkInformation.PingReply
        Dim ip As IPAddress
        ip = IPAddress.Parse("8.8.8.8")
        ProgressBar1.ForeColor = Color.Green
        ProgressBar1.Maximum = ComboBox1.SelectedItem
        TextBox7.AppendText("Ping" & vbCrLf)
        For i = 1 To ComboBox1.SelectedItem
            ProgressBar1.Value = i
            Try
                res = eco.Send(ip)
                If res.Status = NetworkInformation.IPStatus.Success Then
                    TextBox7.AppendText("Respuesta desde " & res.Address.ToString & " en " & res.RoundtripTime.ToString & " ms " & vbCrLf)
                Else
                    TextBox7.AppendText("Sin Respuesta" & vbCrLf)
                    ProgressBar1.ForeColor = Color.Red
                    'you need to disable the setting "Enable xp Visual Styles" under project properties
                End If
            Catch ex As Exception
                MessageBox.Show("Problema con el interfaz de Red" & vbCrLf & vbCrLf & ex.ToString)
                ProgressBar1.ForeColor = Color.Red
                Exit For

            End Try

        Next
    End Sub
    'Botón nsLookup
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        '//define new command
        Dim startInfo = New ProcessStartInfo("cmd.exe")
        Dim command = "nslookup www.google.es"

        With startInfo
            .UseShellExecute = False
            '//need to redirect input and output
            .RedirectStandardInput = True
            .RedirectStandardOutput = True
            '//don't want to see a window
            .CreateNoWindow = True
        End With
        Using proc = Process.Start(startInfo)
            With proc
                Using .StandardInput
                    '//process command
                    .StandardInput.WriteLine(command)
                End Using
                Using .StandardOutput
                    '//get output of command
                    TextBox7.AppendText("NSLookup" & vbCrLf)
                    TextBox7.AppendText(.StandardOutput.ReadToEnd())
                    TextBox7.AppendText(vbCrLf)
                End Using
            End With
        End Using
    End Sub
    'Constructor
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GroupBoxUsb.Enabled = False
        Timer1.Enabled = True
        ComboBox1.SelectedIndex = 1
    End Sub
    'timer Hora
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label10.Text = Date.Now.ToString("yyyy/MM/dd hh:mm:ss")
        If consultadaMemoria Then
            getMemory()
        End If
    End Sub
    'Botón Memory
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        getMemory()
        consultadaMemoria = True
    End Sub
    'Procedimiento Memoria
    Private Sub getMemory()
        Label11.Text = String.Format("TotalPhysicalMemory: {0} MBytes", System.Math.Round(My.Computer.Info.TotalPhysicalMemory / (1024 * 1024)), 2).ToString
        ProgressBar2.Maximum = System.Math.Round(My.Computer.Info.TotalPhysicalMemory / (1024 * 1024))
        ProgressBar2.Value = System.Math.Round(My.Computer.Info.TotalPhysicalMemory / (1024 * 1024)) - System.Math.Round(My.Computer.Info.AvailablePhysicalMemory / (1024 * 1024))
        Label12.Text = String.Format("AvailablePhysMemory: {0} MBytes", System.Math.Round(My.Computer.Info.AvailablePhysicalMemory / (1024 * 1024)), 2).ToString
        Label13.Text = String.Format("TotalVirtualMemory:  {0} MBytes", System.Math.Round(My.Computer.Info.TotalVirtualMemory / (1024 * 1024)), 2).ToString
        ProgressBar3.Maximum = System.Math.Round(My.Computer.Info.TotalVirtualMemory / (1024 * 1024))
        ProgressBar3.Value = System.Math.Round(My.Computer.Info.TotalVirtualMemory / (1024 * 1024)) - System.Math.Round(My.Computer.Info.AvailableVirtualMemory / (1024 * 1024))
        Label14.Text = String.Format("AvailableVirtMemory: {0} MBytes", System.Math.Round(My.Computer.Info.AvailableVirtualMemory / (1024 * 1024)), 2).ToString
    End Sub

End Class
