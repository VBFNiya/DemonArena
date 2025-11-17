Imports System.Collections.ObjectModel
Imports Microsoft.Xna.Framework.Graphics

Public Enum SpriteDirection
    [None]
    Up = 5
    UpperRight = 6
    Right = 7
    LowerRight = 8
    Down = 1
    LowerLeft = 2
    Left = 3
    UpperLeft = 4
End Enum


Partial Public Class Sprite

    Private Shared gshr_lstSprites As New List(Of WeakReference)

    Public Shared Sub DisposeXNAOnAllSprites()
        For Each ref As WeakReference In gshr_lstSprites
            Dim sp As Sprite = TryCast(ref.Target, Sprite)

            If sp IsNot Nothing Then
                sp.DisposeXNA()
            End If
        Next
    End Sub

End Class

Public Class Sprite
    Private _mirrored As Sprite
    Private _xnaImg As Texture2D
    Private _xnaGlowImg As Texture2D
    Private _image As Bitmap
    Private _offsets As Point

    Public Sub New(ByVal name As String, ByVal image As Bitmap)
        If String.IsNullOrEmpty(name) Then
            Throw New ArgumentNullException("name")
        End If

        If image Is Nothing Then
            Throw New ArgumentNullException("image")
        End If

        Me.SpriteName = name
        Me.Image = image

        _offsets = ImageOffsetting.GetOffsetsFromPNG(Me.Image)

        gshr_lstSprites.Add(New WeakReference(Me))
    End Sub

    Public ReadOnly Property Offsets As Point
        Get
            Return _offsets
        End Get
    End Property

    Public Property SpriteName As String

    Public ReadOnly Property SpriteGroupName As String
        Get
            Return Me.SpriteName.Substring(0, 4)
        End Get
    End Property

    Public ReadOnly Property XNAGlowImage As Texture2D
        Get
            Return _xnaGlowImg
        End Get
    End Property

    Public ReadOnly Property XNAImage As Texture2D
        Get
            Return _xnaImg
        End Get
    End Property

    Public Property Image As Bitmap
        Get
            Return _image
        End Get
        Set(ByVal value As Bitmap)
            _image = value
        End Set
    End Property

    Public ReadOnly Property MainDirection As SpriteDirection
        Get
            Return CType(Me.SpriteName.Substring(5, 1), SpriteDirection)
        End Get
    End Property

    Public ReadOnly Property HasMirroredDirection As Boolean
        Get
            Return Me.SpriteName.Length > 6
        End Get
    End Property
    Public ReadOnly Property MirroredDirection As SpriteDirection
        Get
            If Me.HasMirroredDirection Then
                Return CType(Me.SpriteName.Substring(7, 1), SpriteDirection)

            Else
                Return SpriteDirection.None
            End If
        End Get
    End Property

    Public ReadOnly Property FrameLetter As Char
        Get
            Return CChar(Me.SpriteName.Substring(4, 1).ToUpper)
        End Get
    End Property

    Public ReadOnly Property FrameNumber As Integer
        Get
            Dim frameLetter As Char = Me.FrameLetter
            Dim letters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

            Return letters.IndexOf(frameLetter) + 1
        End Get
    End Property

    Public Sub DisposeXNA()
        If _xnaGlowImg IsNot Nothing Then _xnaGlowImg.Dispose()
        If _xnaImg IsNot Nothing Then _xnaImg.Dispose()

        _xnaGlowImg = Nothing
        _xnaImg = Nothing
    End Sub

    Public Sub SetXNAGlowImg(ByVal tex As Texture2D)
        _xnaGlowImg = tex
    End Sub

    Public Sub SetXNAImage(ByVal tex As Texture2D)
        _xnaImg = tex
    End Sub

    Public Function CreateMirroredDirection() As Sprite
        If Me.HasMirroredDirection Then
            If _mirrored Is Nothing Then
                Dim img As New Bitmap(Me.Image)
                Dim spriteName As String = Me.SpriteName.Substring(0, 4)
                spriteName += Me.FrameLetter
                spriteName += CInt(Me.MirroredDirection).ToString
                img.RotateFlip(RotateFlipType.Rotate180FlipY)
                _mirrored = New Sprite(spriteName, img)


                Return _mirrored
            Else
                Return _mirrored
            End If
        End If
        Return Nothing
    End Function


    Public Shared Narrowing Operator CType(ByVal op1 As Sprite) As Bitmap
        Return op1.Image
    End Operator
End Class

Public Class SpriteCollection
    Inherits Collection(Of Sprite)

    Private g_dictSprites As New Dictionary(Of String, Sprite)

    Public Sub New()

    End Sub

    Public Sub New(ByVal sprites As IList(Of Sprite))
        For Each sp As Sprite In sprites
            Me.Add(sp)
        Next
    End Sub

    Public Sub MergeCollection(ByVal sprites As SpriteCollection)
        For Each sp In sprites
            Me.Add(sp)
        Next
    End Sub

    Private Function CreateKey(ByVal spriteGroupName As String, ByVal direction As SpriteDirection, ByVal frameLetter As Char) As String
        Return spriteGroupName + "_" + CInt(direction).ToString() + "_" + frameLetter.ToString
    End Function

    'Specifically for collections with multiple sprite names
    Public Function GetSprite(ByVal spriteGroupName As String, ByVal direction As SpriteDirection, ByVal frameLetter As Char) As Sprite

        Dim fnd As Sprite = Nothing
        Dim key As String = CreateKey(spriteGroupName, direction, frameLetter)

        If g_dictSprites.TryGetValue(key, fnd) Then
            Return fnd
        Else
            fnd = Me.FirstOrDefault(Function(sp)
                                        Return sp.SpriteGroupName.Equals(spriteGroupName, StringComparison.CurrentCultureIgnoreCase) AndAlso (sp.MainDirection = direction OrElse sp.MirroredDirection = direction OrElse sp.MainDirection = SpriteDirection.None) AndAlso Char.ToLower(frameLetter) = Char.ToLower(sp.FrameLetter)
                                    End Function)

            If fnd Is Nothing Then Throw New Exception("Frame not found")


            If fnd.MainDirection = direction OrElse fnd.MainDirection = SpriteDirection.None Then
                g_dictSprites.Add(key, fnd)

                Return fnd
            End If

            If fnd.MirroredDirection = direction Then
                Dim mirr As Sprite = fnd.CreateMirroredDirection

                g_dictSprites.Add(key, mirr)

                Return mirr
            End If
            Return Nothing

        End If


    End Function


    Public Function GetSprite(ByVal direction As SpriteDirection, ByVal frameLetter As Char) As Sprite

        Dim fnd As Sprite = Me.FirstOrDefault(Function(sp)
                                                  Return (sp.MainDirection = direction OrElse sp.MirroredDirection = direction OrElse sp.MainDirection = SpriteDirection.None) AndAlso Char.ToLower(frameLetter) = Char.ToLower(sp.FrameLetter)
                                              End Function)

        If fnd Is Nothing Then Throw New Exception("Frame not found")


        If fnd.MainDirection = direction OrElse fnd.MainDirection = SpriteDirection.None Then Return fnd
        If fnd.MirroredDirection = direction Then Return fnd.CreateMirroredDirection()

        Return Nothing
    End Function

    Public Function GetFrames(ByVal frameList As String, ByVal direction As SpriteDirection) As SpriteCollection

        'TODO: An implementation to return 0 direction frames
        'for every frame where there is one but none for
        'any of the 1-8 directions. This may have to be implemented
        'in the Sprite class. I'm not sure yet.
        Dim colSprites As New SpriteCollection

        For Each sp As Sprite In Me

            If sp.MainDirection = direction OrElse (sp.HasMirroredDirection AndAlso sp.MirroredDirection = direction) Then

                For Each frame As Char In frameList
                    If Char.ToLower(frame) = Char.ToLower(sp.FrameLetter) Then
                        If sp.MainDirection = direction Then
                            colSprites.Add(sp)
                        ElseIf sp.HasMirroredDirection AndAlso (sp.MirroredDirection = direction) Then
                            colSprites.Add(sp.CreateMirroredDirection())
                        End If
                    End If
                Next


            End If

        Next

        Return colSprites
    End Function
End Class


