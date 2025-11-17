Imports System.IO
Imports System.Drawing.Imaging

Public Class ImageOffsetting

    Public Shared Function ApplyOffsetsToPosition(ByVal position As PointF, ByVal offsets As Point) As PointF
        Return ApplyOffsetsToPosition(position, offsets, 1)
    End Function

    Public Shared Function ApplyOffsetsToPosition(ByVal position As PointF, ByVal offsets As Point, ByVal scale As Double) As PointF
        offsets.X *= scale
        offsets.Y *= scale

        Return New PointF(position.X + offsets.X, position.Y + offsets.Y)
    End Function

    Public Shared Function GetOffsetsFromPNG(ByVal image As Bitmap) As Point

        Dim ms As New MemoryStream

        'If the PNG used to create that Bitmap had
        'a grAb chunk, GDI+ will preserve it when
        'saving it to a stream
        image.Save(ms, ImageFormat.Png)

        ms.Position = 0

        Dim offsets As Point = GetOffsetsFromPNG(ms)

        ms.Dispose()
        Return offsets
    End Function

    Public Shared Function GetOffsetsFromPNG(ByVal pngFile As String) As Point

        Using fs As New FileStream(pngFile, FileMode.Open, FileAccess.Read, FileShare.Read)
            Return GetOffsetsFromPNG(fs)
        End Using

    End Function

    Public Shared Function GetOffsetsFromPNG(ByVal pngStream As Stream) As Point
        Dim reader As New PNGChunkReader(pngStream)

        Dim ch As PNGChunk = reader.ReadChunk()

        Do Until ch Is Nothing
            If ch.Name = "grAb" Then
                Return GetOffsets(ch)
            End If
            ch = reader.ReadChunk
        Loop
    End Function

    Private Shared Function GetOffsets(ByVal offsetChunk As PNGChunk) As Point

        If offsetChunk.Name = "grAb" Then
            Dim arr_X As Byte() = New Byte(3) {}
            Dim arr_y As Byte() = New Byte(3) {}

            Array.ConstrainedCopy(offsetChunk.Data, 0, arr_X, 0, 4)
            Array.ConstrainedCopy(offsetChunk.Data, 4, arr_y, 0, 4)

            'These values are in big endian order.
            'We must reverse to little endian order
            Array.Reverse(arr_X)
            Array.Reverse(arr_y)

            Return New Point(BitConverter.ToInt32(arr_X, 0), BitConverter.ToInt32(arr_y, 0))
        Else
            Throw New Exception("Invalid chunk. Must pass a grAb chunk")
        End If


    End Function



End Class