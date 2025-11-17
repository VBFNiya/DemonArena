Imports System.Reflection
Imports System.Collections.ObjectModel

Public Class AnimatedEntity
    Inherits AnimatedEntityBase

    Public Property Sprites As SpriteCollection
        Get
            Return Me.AllSprites
        End Get
        Set(ByVal value As SpriteCollection)
            MyBase.SetSprites(value)
        End Set
    End Property

    Public Sub SetSequence(ByVal frames As String, ByVal direction As SpriteDirection)
        MyBase.SetAnimationSequence(frames, direction)
    End Sub
End Class

Public MustInherit Class AnimatedEntityBase
    Implements ITickable, IFastListItem

    Private g_lstAnimation As New List(Of AnimationFrameInfo)
    Private g_iCurTick As Integer = 0
    Private g_iCurrentFrameIndex As Integer = 0
    Private g_colSprites As SpriteCollection

    Private g_bPaused As Boolean

    'We save this incase we just want to change the
    'direction
    Private g_sAnimationSequence As String

    Public ReadOnly Property CurrentSprite As RenderInfo
        Get
            Return New RenderInfo(Me.CurrentFrame.Sprite)
        End Get
    End Property

    Public Overridable Sub Tick() Implements ITickable.Tick
        g_iCurTick += 1

        If g_iCurTick >= Me.CurrentFrame.Delay OrElse g_bPaused Then
            g_iCurTick = 0

            If g_iCurrentFrameIndex = g_lstAnimation.Count - 1 Then
                OnLastFrameAnimated()
            End If

            If Not g_bPaused Then
                g_iCurrentFrameIndex += 1
            End If

            If g_iCurrentFrameIndex = g_lstAnimation.Count Then g_iCurrentFrameIndex = 0

            If Not g_bPaused Then
                OnFrameChanged()
            End If
        End If

    End Sub

    'Derived classes can override this if they need to know
    'when the current frame has changed
    Protected Overridable Sub OnFrameChanged()

    End Sub

    Protected Overridable Sub OnLastFrameAnimated()

    End Sub

    Protected ReadOnly Property AllSprites As SpriteCollection
        Get
            Return g_colSprites
        End Get
    End Property

    Protected ReadOnly Property IsLastFrame As Boolean
        Get
            Return g_iCurrentFrameIndex = g_lstAnimation.Count - 1
        End Get
    End Property

    Protected Sub PauseAnimation()
        g_bPaused = True
    End Sub

    Protected Sub UnpauseAnimation()
        g_bPaused = False
    End Sub


    Protected Sub SetSprites(ByVal sprites As SpriteCollection)
        g_colSprites = sprites
    End Sub

    Protected Sub SetDirection(ByVal trajectory As PointF)
        If Not String.IsNullOrEmpty(g_sAnimationSequence) Then
            SetAnimationSequence(g_sAnimationSequence, trajectory)
        End If
    End Sub

    Protected Sub SetDirection(ByVal direction As SpriteDirection)
        If Not String.IsNullOrEmpty(g_sAnimationSequence) Then
            SetAnimationSequence(g_sAnimationSequence, direction)
        End If
    End Sub

    Protected Overridable Sub SetAnimationSequence(ByVal animationString As String, ByVal trajectory As PointF)

        Dim angle As Double = Geometry.GetAngleInDegrees(trajectory)
        Dim d As SpriteDirection = SpriteHelpers.GetDirectionFromAngle(angle)

        SetAnimationSequence(animationString, d)
    End Sub

    Protected Overridable Sub SetAnimationSequence(ByVal animationString As String, ByVal direction As SpriteDirection)
        SetAnimationSequence(animationString, direction, False)
    End Sub

    Protected Overridable Sub SetAnimationSequence(ByVal animationString As String, ByVal direction As SpriteDirection, ByVal startFromFirstFrame As Boolean)
        Dim frames As String = SpriteHelpers.GetFrameLettersFromAnimationString(animationString)

        Dim smallList As SpriteCollection = Me.AllSprites.GetFrames(frames, direction)

        'TODO: Replace this with an implementation in
        'GetFrames itself. See the TODO comment there
        '--------------------------------------------
        'We assume that these particular frames are
        'direction 0 frames. For example, death frames
        'typically have no directions which means the
        'frames are valid in all directions
        If smallList.Count = 0 Then
            smallList = Me.AllSprites.GetFrames(frames, SpriteDirection.None)
        End If

        g_lstAnimation.Clear()

        For Each frame As String In animationString.Split(","c)

            Dim letter As Char = frame.Split(":"c)(0)
            Dim delay As Integer = frame.Split(":"c)(1)
            Dim sp As Sprite = smallList.GetSprite(direction, letter)

            Dim ani As New AnimationFrameInfo(sp, delay)

            g_lstAnimation.Add(ani)
        Next

        If g_iCurrentFrameIndex >= g_lstAnimation.Count Then g_iCurrentFrameIndex = 0

        If startFromFirstFrame Then g_iCurrentFrameIndex = 0

        g_sAnimationSequence = animationString
    End Sub

    Private ReadOnly Property CurrentFrame As AnimationFrameInfo
        Get
            Return g_lstAnimation.Item(g_iCurrentFrameIndex)
        End Get
    End Property

    Private _ref As Object
    Private Property LinkReference As Object Implements IFastListItem.LinkReference
        Get
            Return _ref
        End Get
        Set(value As Object)
            _ref = value
        End Set
    End Property

End Class

Public Class AnimationFrameInfo
    Public Sub New(ByVal sprite As Sprite, ByVal delay As Integer)
        Me.Sprite = sprite
        Me.Delay = delay
    End Sub

    Public Sub New()

    End Sub
    Public Property Sprite As Sprite
    Public Property Delay As Integer
End Class

Public Class RenderInfo
    'Inherits Sprite

    'RenderInfo objects are accessed a lot and
    'fields are faster than properties
    Public Sprite As Sprite
    Public Alpha As Single

    Public Sub New(ByVal sprite As Sprite, ByVal alpha As Single)
        Me.Alpha = alpha
        Me.Sprite = sprite
    End Sub

    Public Sub New(ByVal sprite As Sprite)
        Me.New(sprite, 1)
    End Sub

End Class

