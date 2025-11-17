Imports System.IO
Imports System.Text

Public Class PNGChunkReader
    Implements IDisposable

    Private g_strmPNG As Stream
    Private g_objReader As BinaryReader

    Private g_bDisposeOfStream As Boolean

    Public Sub New(ByVal file As String)
        Me.New(New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))

        g_bDisposeOfStream = True
    End Sub

    Public Sub New(ByVal pngData As Stream)
        g_bDisposeOfStream = False

        If pngData.CanSeek <> True Then
            Throw New Exception("Unable to seek within stream")
        End If

        If pngData.Position <> 0 Then
            Throw New Exception("Stream must be positioned at start")
        End If

        g_strmPNG = pngData

        g_objReader = New BinaryReader(g_strmPNG, Encoding.ASCII)

        ValidatePNG()
    End Sub

    Public Function ReadChunk() As PNGChunk

        'Check for the end of the PNG
        If g_strmPNG.Position = g_strmPNG.Length Then
            Return Nothing
        End If

        If g_strmPNG.Position = 0 Then
            g_strmPNG.Position = 8
        End If

        'PNG numeric data is in always big endian
        Dim chunkLen As Integer = BitConverter.ToInt32(g_objReader.ReadBytes(4).Reverse.ToArray, 0)
        Dim chunkName As String = New String(g_objReader.ReadChars(4))
        Dim data As Byte() = g_objReader.ReadBytes(chunkLen)

        'PNG numeric data is in always big endian
        Dim CRC As Integer = BitConverter.ToInt32(g_objReader.ReadBytes(4).Reverse.ToArray, 0)

        Return New PNGChunk(chunkName, data, CRC)
    End Function

    Public Sub Close()
        Me.Dispose()
    End Sub

    Private Sub ValidatePNG()
        Dim prevPos As Long = g_strmPNG.Position

        g_strmPNG.Position = 1

        Dim s As String = New String(g_objReader.ReadChars(3))

        If s <> "PNG" Then
            Throw New Exception("Invalid PNG signature. 'PNG' not found in signature")
        End If

        g_strmPNG.Position = prevPos
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then

            End If

            If g_bDisposeOfStream Then
                g_objReader.Close()
            End If

        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Private Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

Public Class PNGChunk

    Private _name As String
    Private _data As Byte()
    Private _crc As Integer

    Public Sub New(ByVal name As String, ByVal data As Byte(), ByVal crc As Integer)
        _name = name
        _data = data
        _crc = crc
    End Sub

    Public ReadOnly Property Name As String
        Get
            Return _name
        End Get
    End Property
    Public ReadOnly Property Length As Integer
        Get
            Return _data.Length
        End Get
    End Property

    Public ReadOnly Property Data As Byte()
        Get
            Return _data
        End Get
    End Property
    Public ReadOnly Property CRC As Integer
        Get
            Return _crc
        End Get
    End Property


End Class