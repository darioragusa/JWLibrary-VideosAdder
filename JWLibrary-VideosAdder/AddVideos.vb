Imports Devart.Data
Imports Devart.Data.SQLite
Imports System.IO

Module AddVideos
    Sub AddImg(ByVal dbDir As String, ByVal vidList As List(Of String))
        If Not System.IO.File.Exists(dbDir) Then Return
        Using SQLCon As New SQLiteConnection(String.Format("Data Source = {0}", dbDir))
            Dim insertMediaKeyQuery As String = "INSERT INTO MediaKey(KeySymbol, MediaType, DocumentId, MepsLanguageIndex, IssueTagNumber, Track, BookNumber) VALUES(@KeySymbol, @MediaType, @DocumentId, @MepsLanguageIndex, @IssueTagNumber, @Track, @BookNumber)"
            Dim insertVideoQuery As String = "INSERT INTO Video(MediaKeyId, Title, Version, MimeType, BitRate, FrameRate, Duration, Checksum, FileSize, FrameHeight, FrameWidth, Label, Subtitled, DownloadUrl, FilePath, Source) VALUES(@MediaKeyId, @Title, @Version, @MimeType, @BitRate, @FrameRate, @Duration, @Checksum, @FileSize, @FrameHeight, @FrameWidth, @Label, @Subtitled, @DownloadUrl, @FilePath, @Source)"
            SQLCon.Open()
            For Each Video In vidList
                Dim mediaKeyCMD As New SQLiteCommand(insertMediaKeyQuery, SQLCon)
                Dim videoCMD As New SQLiteCommand(insertVideoQuery, SQLCon)
                mediaKeyCMD.Parameters.AddWithValue("@KeySymbol", "custom")
                mediaKeyCMD.Parameters.AddWithValue("@MediaType", "1")
                mediaKeyCMD.Parameters.AddWithValue("@DocumentId", "0")
                mediaKeyCMD.Parameters.AddWithValue("@MepsLanguageIndex", "4")
                mediaKeyCMD.Parameters.AddWithValue("@IssueTagNumber", Format(Now, "yyyyMMdd"))
                mediaKeyCMD.Parameters.AddWithValue("@Track", Format(Now, "hhmmss"))
                mediaKeyCMD.Parameters.AddWithValue("@BookNumber", "0")
                mediaKeyCMD.ExecuteNonQuery()
                videoCMD.Parameters.AddWithValue("@MediaKeyId", CStr(GetLastMediaKeyID(dbDir)))
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
                videoCMD.Parameters.AddWithValue("@DownloadUrl", "https://github.com/Miaosi001") 'No matter
                videoCMD.Parameters.AddWithValue("@FilePath", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) & "\JWLibrary\" & Video.ToString.Split("\").Last)
                videoCMD.Parameters.AddWithValue("@Source", "0")
                videoCMD.ExecuteNonQuery()
            Next
_End:
            SQLCon.Dispose() : SQLCon.Close() : GC.Collect() : GC.WaitForPendingFinalizers()
        End Using
    End Sub

    Function GetLastMediaKeyID(ByVal dbDir As String) As Integer
        If Not System.IO.File.Exists(dbDir) Then Return -1
        Dim lastID As Integer = 0
        Using sqlCon As New SQLiteConnection(String.Format("Data Source = {0}", dbDir))
            Dim readIdQuery As String = "SELECT MediaKeyId FROM MediaKey"
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
            Dim objFolder As Object = _
               objShell.Namespace(Path.GetDirectoryName(MovieFullPath))
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
End Module
