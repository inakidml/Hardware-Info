Public Class nombreFicheroForm
    Private mainForm As Form1
    Private nombreFichero As String
    Private archivo As Archivo
    Private texto As String
    'constructor
    Public Sub New(ByRef mainForm As Form, ByRef texto As String)
        ' This call is required by the designer.
        InitializeComponent()
        Me.mainForm = mainForm
        Me.texto = texto
        ' Add any initialization after the InitializeComponent() call.
    End Sub
    'Guardar
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not TextBox1.Text.Contains(".") Then
            nombreFichero = mainForm.getRutaEscritorio
            nombreFichero += TextBox1.Text & ".txt"
            archivo = New Archivo(nombreFichero, texto)
            Me.Dispose()
        Else
            MessageBox.Show("El nombre sin extensión")
        End If

    End Sub

    ' si se pulsa enter en el textbox se focus el botón
    Private Sub TextBox1_TextChanged(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.ChrW(13) Then
            Me.Button1.Focus()
        End If
    End Sub

End Class