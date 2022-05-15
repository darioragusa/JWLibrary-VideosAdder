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
        Dim wtDir = findCurrentWatchtowers()
        If wtDir.Count = 0 Then
            MsgBox(Strings.Error4Text, MsgBoxStyle.Critical, Me.Text)
            Return
        End If
        For Each directory In wtDir
            makeBackup(directory)
            Dim dbDir As String = directory & "\" & directory.Split("\").Last & ".db"
            Dim weekN As Integer = GetWeekN(dbDir)
            If weekN = -1 Then
                MsgBox(Strings.Error5Text, MsgBoxStyle.Exclamation, Me.Text) : Exit Sub
            End If
            Dim docID As Integer = GetDocumentID(dbDir, weekN)


            Dim collectionDir As String = findCollectionDirectory()
            If collectionDir = "" Then
                MsgBox(Strings.Error3Text, MsgBoxStyle.Exclamation, Me.Text)
            End If
            Dim VideoDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) & "\JWLibrary"
            For Each vidDir In vidList
                System.IO.File.Copy(vidDir, VideoDir & "\" & vidDir.ToString.Split("\").Last, True)
            Next
            AddVideo(dbDir, docID, collectionDir, vidList)
        Next

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
            Strings.Error4Text = "Torre di Guardia attuale non trovata"
            Strings.Error5Text = "Settimana non trovata"
            Strings.Text1 = "Video"
            Strings.Text2 = "Il mio lavoro qui è finito"
            Strings.Text3 = "Pubblicazioni ripristinate"
            ButtonSelect.Text = "Seleziona i video da aggiungere"
            ButtonDelete.Text = "Elimina"
            ButtonAdd_Menu.Text = "Aggiungi video"
            ButtonRestoreAll.Text = "Ripristina pubblicazioni originali"
        End If
    End Sub

    Structure Strings
        Shared Error1Text As String = "JW Library folder not found"
        Shared Error2Text As String = "Close JW Library to continue"
        Shared Error3Text As String = "Collection not found"
        Shared Error4Text As String = "Current Watchtower not found"
        Shared Error5Text As String = "Week not found"
        Shared Text1 As String = "Videos"
        Shared Text2 As String = "My job here is done"
        Shared Text3 As String = "Publications restored"
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

    Private Sub ButtonRestoreAll_Click(sender As Object, e As EventArgs) Handles ButtonRestoreAll.Click
        If Not ChecKJW() Then Return
        For Each directory In findCurrentWatchtowers()
            If Not directory.ToString.EndsWith("_back") Then
                restoreBackup(directory)
            End If
        Next
        MsgBox(Strings.Text3, MsgBoxStyle.OkOnly, Me.Text)
    End Sub

    Function ChecKJW() As Boolean
_check:
        Dim proc() As Process
        proc = Process.GetProcessesByName("JWLibrary")
        If proc.Count > 0 Then
            Dim result = MsgBox(Strings.Error2Text, MsgBoxStyle.RetryCancel, Me.Text)
            If result = MsgBoxResult.Cancel Then
                Return False
            Else
                GoTo _check
            End If
        End If
        Return True
    End Function
End Class
