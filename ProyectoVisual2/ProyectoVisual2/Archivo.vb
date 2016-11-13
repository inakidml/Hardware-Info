Imports System.IO

Public Class Archivo
    Private fs As FileStream

    Public Sub New(ByVal ruta As String, ByVal texto As String)
        Try
            fs = New FileStream(ruta, FileMode.OpenOrCreate, FileAccess.Write)
            Dim sw As StreamWriter
            sw = New StreamWriter(fs)
            sw.WriteLine(texto)
            sw.Close() 'super importante cerrar el flujo
            fs.Close()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.ToString)
        End Try

    End Sub

End Class
