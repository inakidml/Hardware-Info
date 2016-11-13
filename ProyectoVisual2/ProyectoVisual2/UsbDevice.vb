Public Class UsbDevice
    Private deviceID As String
    Private description As String
    Private status As String
    Public Sub New(ByVal deviceID As String, ByVal description As String, ByVal status As String)
        Me.deviceID = deviceID
        Me.description = description
        Me.status = status
    End Sub
    Public ReadOnly Property getDeviceID() As String
        Get
            Return Me.deviceID
        End Get
    End Property
    Public ReadOnly Property getDescripcion() As String
        Get
            Return Me.description
        End Get
    End Property
    Public ReadOnly Property getStatus() As String
        Get
            Return Me.status
        End Get
    End Property
End Class
