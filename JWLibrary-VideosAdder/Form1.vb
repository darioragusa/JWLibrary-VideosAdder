Public Class Form1
    Dim imgList As New List(Of String)

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub ButtonAdd_Menu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonAdd_Menu.Click
        Panel1.Hide()
    End Sub

    Private Sub ButtonAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonAdd.Click

    End Sub

    Private Sub ButtonSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonSelect.Click

    End Sub

    Private Sub ButtonDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonDelete.Click

    End Sub

    Sub ChangeLanguage()
        Dim UserCulture = System.Globalization.CultureInfo.CurrentCulture
        If LCase(UserCulture.ToString).Contains("it") Then
            Strings.Error1Text = "Cartella di JW Library non trovata"
            Strings.Error2Text = "Chiudi JW Library per continuare"
            Strings.Text1 = "Video"
            Strings.Text2 = "Il mio lavoro qui è finito"
            ButtonSelect.Text = "Seleziona i video da aggiungere"
            ButtonDelete.Text = "Elimina"
            ButtonAdd_Menu.Text = "Aggiungi video"
        End If
    End Sub

    Structure Strings
        Shared Error1Text As String = "JW Library folder not found"
        Shared Error2Text As String = "Close JW Library to continue"
        Shared Text1 As String = "Videos"
        Shared Text2 As String = "My job here is done"
    End Structure
End Class
