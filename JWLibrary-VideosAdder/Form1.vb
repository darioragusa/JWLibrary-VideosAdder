Imports System.IO

Public Class Form1
    Dim vidList As New List(Of String)

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ChangeLanguage()
        Me.Icon = My.Resources.Ico
    End Sub

    Private Sub ButtonAdd_Menu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonAdd_Menu.Click
        Panel1.Hide()
    End Sub

    Private Sub ButtonAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonAdd.Click
        Dim collectionDir As String = findCollectionDirectory()
        If collectionDir = "" Then
            MsgBox(Strings.Error3Text, MsgBoxStyle.Exclamation, Me.Text)
        End If
        Dim VideoDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) & "\JWLibrary"
        For Each vidDir In vidList
            System.IO.File.Copy(vidDir, VideoDir & "\" & vidDir.ToString.Split("\").Last, True)
        Next
        AddImg(collectionDir, vidList)
        MsgBox(Strings.Text2, MsgBoxStyle.OkOnly, Me.Text)
        Application.Restart()
        'I have to close otherwise it gives error
        'for some reason the database is not closed and trying to reopen it does not work
    End Sub

    Private Sub ButtonSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonSelect.Click
        Dim OpenFileDialog1 As New OpenFileDialog
        OpenFileDialog1.Filter = Strings.Text1 + "| *.mp4"

        OpenFileDialog1.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.Desktop
        OpenFileDialog1.Multiselect = True
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            For Each vidFile In OpenFileDialog1.FileNames
                vidList.Add(vidFile)
            Next
        End If
        ListBoxVid.DataSource = Nothing
        ListBoxVid.Items.Clear()
        ListBoxVid.DataSource = vidList
        If vidList.Count = 0 Then ButtonAdd.Enabled = False Else ButtonAdd.Enabled = True
        If vidList.Count = 0 Then ButtonDelete.Enabled = False Else ButtonDelete.Enabled = True
    End Sub

    Private Sub ButtonDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonDelete.Click
        If ListBoxVid.SelectedIndex = -1 Then vidList.Clear() Else vidList.RemoveAt(ListBoxVid.SelectedIndex)
        ListBoxVid.DataSource = Nothing
        ListBoxVid.Items.Clear()
        ListBoxVid.DataSource = vidList
        If vidList.Count = 0 Then ButtonAdd.Enabled = False Else ButtonAdd.Enabled = True
        If vidList.Count = 0 Then ButtonDelete.Enabled = False Else ButtonDelete.Enabled = True
    End Sub

    Sub ChangeLanguage()
        Dim UserCulture = System.Globalization.CultureInfo.CurrentCulture
        If LCase(UserCulture.ToString).Contains("it") Then
            Strings.Error1Text = "Cartella di JW Library non trovata"
            Strings.Error2Text = "Chiudi JW Library per continuare"
            Strings.Error3Text = "Collezione non trovata"
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
        Shared Error3Text As String = "Collection not found"
        Shared Text1 As String = "Videos"
        Shared Text2 As String = "My job here is done"
    End Structure

    Function findCollectionDirectory() As String
        Dim path As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Packages"
        If IO.Directory.Exists(path) Then
            For Each Dir As String In Directory.GetDirectories(path)
                If LCase(Dir).Contains("watchtower") Then
                    path = Dir & "\LocalState\Data\mediaCollection.db"
                    Exit For
                End If
            Next
        End If
        If IO.File.Exists(path) Then Return path Else Return ""
    End Function
End Class
