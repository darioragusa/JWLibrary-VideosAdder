Imports Devart.Data
Imports Devart.Data.SQLite
Imports System.IO

Module AddVideos
    Sub AddVideo(ByVal dbDirWT As String, ByVal docID As Integer, ByVal dbDir As String, ByVal vidList As List(Of String))
        If Not System.IO.File.Exists(dbDir) Then Return
        Using SQLCon As New SQLiteConnection(String.Format("Data Source = {0}", dbDir))
            Dim trackList As List(Of Integer) = New List(Of Integer)
            Dim insertMediaKeyQuery As String = "INSERT INTO MediaKey(MediaKeyId, KeySymbol, MediaType, DocumentId, MepsLanguageIndex, IssueTagNumber, Track, BookNumber) VALUES(@MediaKeyId, @KeySymbol, @MediaType, @DocumentId, @MepsLanguageIndex, @IssueTagNumber, @Track, @BookNumber)"
            Dim insertVideoQuery As String = "INSERT INTO Video(MediaKeyId, Title, Version, MimeType, BitRate, FrameRate, Duration, Checksum, FileSize, FrameHeight, FrameWidth, Label, Subtitled, DownloadUrl, FilePath, Source) VALUES(@MediaKeyId, @Title, @Version, @MimeType, @BitRate, @FrameRate, @Duration, @Checksum, @FileSize, @FrameHeight, @FrameWidth, @Label, @Subtitled, @DownloadUrl, @FilePath, @Source)"
            SQLCon.Open()
            For Each Video In vidList
                Dim NewID = GetLastMediaKeyID(dbDir) + 1
                Dim mediaKeyCMD As New SQLiteCommand(insertMediaKeyQuery, SQLCon)
                Dim videoCMD As New SQLiteCommand(insertVideoQuery, SQLCon)
                mediaKeyCMD.Parameters.AddWithValue("@MediaKeyId", NewID)
                mediaKeyCMD.Parameters.AddWithValue("@KeySymbol", "sjjm") ' Fake song
                mediaKeyCMD.Parameters.AddWithValue("@MediaType", "1")
                mediaKeyCMD.Parameters.AddWithValue("@DocumentId", "0")
                mediaKeyCMD.Parameters.AddWithValue("@MepsLanguageIndex", "4")
                mediaKeyCMD.Parameters.AddWithValue("@IssueTagNumber", 0)
                mediaKeyCMD.Parameters.AddWithValue("@Track", NewID + 1000)
                mediaKeyCMD.Parameters.AddWithValue("@BookNumber", "0")
                mediaKeyCMD.ExecuteNonQuery()
                trackList.Add(NewID + 1000)
                videoCMD.Parameters.AddWithValue("@MediaKeyId", NewID)
                videoCMD.Parameters.AddWithValue("@Title", Video.Split("\").Last.Replace(".mp4", "").Replace(".MP4", ""))
                videoCMD.Parameters.AddWithValue("@Version", "1")
                videoCMD.Parameters.AddWithValue("@MimeType", "video/mp4")
                videoCMD.Parameters.AddWithValue("@BitRate", "1000") 'No matter
                videoCMD.Parameters.AddWithValue("@FrameRate", "30") 'No matter
                videoCMD.Parameters.AddWithValue("@Duration", CStr(GetDuration(Video)))
                videoCMD.Parameters.AddWithValue("@Checksum", "") 'No matter
                videoCMD.Parameters.AddWithValue("@FileSize", CStr(FileLen(Video)))
                videoCMD.Parameters.AddWithValue("@FrameHeight", "720") 'No matter
                videoCMD.Parameters.AddWithValue("@FrameWidth", "1280") 'No matter
                videoCMD.Parameters.AddWithValue("@Label", "720p") 'No matter
                videoCMD.Parameters.AddWithValue("@Subtitled", "0")
                videoCMD.Parameters.AddWithValue("@DownloadUrl", "https://github.com/darioragusa") 'No matter
                videoCMD.Parameters.AddWithValue("@FilePath", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) & "\JWLibrary\" & Video.ToString.Split("\").Last)
                videoCMD.Parameters.AddWithValue("@Source", "0")
                videoCMD.ExecuteNonQuery()
            Next
            SQLCon.Dispose() : SQLCon.Close() : GC.Collect() : GC.WaitForPendingFinalizers()
            AddVideoToWT(dbDirWT, docID, trackList)
        End Using
    End Sub

    Function GetLastMediaKeyID(ByVal dbDir As String) As Integer
        If Not System.IO.File.Exists(dbDir) Then Return -1
        Dim lastID As Integer = 0
        Using sqlCon As New SQLiteConnection(String.Format("Data Source = {0}", dbDir))
            Dim readIdQuery As String = "SELECT MAX(MediaKeyId) FROM MediaKey"
            sqlCon.Open()
            Using command = sqlCon.CreateCommand()
                command.CommandText = readIdQuery
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        lastID = reader.GetInt32(0)
                    End While
                End Using
            End Using
            sqlCon.Dispose() : sqlCon.Close() : GC.Collect() : GC.WaitForPendingFinalizers()
        End Using : Return lastID
    End Function

    Function GetDuration(ByVal MovieFullPath As String) As String
        If File.Exists(MovieFullPath) Then
            Dim objShell As Object = CreateObject("Shell.Application")
            Dim objFolder As Object = objShell.Namespace(Path.GetDirectoryName(MovieFullPath))
            For Each strFileName In objFolder.Items
                If strFileName.Name = Path.GetFileName(MovieFullPath) Then
                    Dim duration = objFolder.GetDetailsOf(strFileName, 27).ToString
                    Dim h = duration.Split(":")(0)
                    Dim m = duration.Split(":")(1)
                    Dim s = duration.Split(":")(2)
                    Return h & m & s
                    'Return objFolder.GetDetailsOf(strFileName, 27).ToString
                    Exit For
                    Exit Function
                End If
            Next
            Return ""
        Else
            Return ""
        End If
    End Function

    Sub AddVideoToWT(ByVal dbDir As String, ByVal docID As Integer, ByVal trackList As List(Of Integer))
        If Not System.IO.File.Exists(dbDir) Then Return
        Using SQLCon As New SQLiteConnection(String.Format("Data Source = {0}", dbDir))
            Dim insertMultimediaQuery As String = "INSERT INTO Multimedia(MultimediaId, DataType, MajorType, MinorType, MimeType, KeySymbol, Track, CategoryType, MepsLanguageIndex) VALUES(@MultimediaId, @Data, @Major, @Minor, @Mime, @KeySymbol, @Track, @Category, @Meps)"
            Dim insertDocumentMultimediaQuery As String = "INSERT INTO DocumentMultimedia(DocumentId, MultimediaId) VALUES(@Document, @Multimedia)"
            Dim newID As Integer = GetLastMultimediaID(dbDir) + 1
            If newID <> -1 Then
                SQLCon.Open()
                For Each track In trackList
                    Dim multimediaCMD As New SQLiteCommand(insertMultimediaQuery, SQLCon)
                    Dim documentMultimediaCMD As New SQLiteCommand(insertDocumentMultimediaQuery, SQLCon)
                    multimediaCMD.Parameters.AddWithValue("@MultimediaId", newID)
                    multimediaCMD.Parameters.AddWithValue("@Data", 2)
                    multimediaCMD.Parameters.AddWithValue("@Major", 2)
                    multimediaCMD.Parameters.AddWithValue("@Minor", 3)
                    multimediaCMD.Parameters.AddWithValue("@Mime", "video/mp4")
                    multimediaCMD.Parameters.AddWithValue("@KeySymbol", "sjjm")
                    multimediaCMD.Parameters.AddWithValue("@Track", track)
                    multimediaCMD.Parameters.AddWithValue("@Category", -1)
                    multimediaCMD.Parameters.AddWithValue("@Meps", 4)
                    multimediaCMD.ExecuteNonQuery()
                    documentMultimediaCMD.Parameters.AddWithValue("@Document", CStr(docID))
                    documentMultimediaCMD.Parameters.AddWithValue("@Multimedia", newID)
                    documentMultimediaCMD.ExecuteNonQuery()
                    newID += 1
                Next
            End If
            SQLCon.Dispose() : SQLCon.Close() : GC.Collect() : GC.WaitForPendingFinalizers()
        End Using
    End Sub

    Function GetLastMultimediaID(ByVal dbDir As String) As Integer
        If Not System.IO.File.Exists(dbDir) Then Return -1
        Dim lastID As Integer = -1
        Using sqlCon As New SQLiteConnection(String.Format("Data Source = {0}", dbDir))
            Dim readWeekQuery As String = "SELECT Max(MultimediaId) FROM Multimedia"
            sqlCon.Open()
            Using command = sqlCon.CreateCommand()
                command.CommandText = readWeekQuery
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        lastID = reader.GetInt32(0)
                    End While
                End Using
            End Using
            sqlCon.Dispose() : sqlCon.Close() : GC.Collect() : GC.WaitForPendingFinalizers()
        End Using : Return lastID
    End Function
End Module
