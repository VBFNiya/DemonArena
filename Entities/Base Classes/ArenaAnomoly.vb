Public MustInherit Class Anomaly
    Inherits AnimatedEntityBase
    Implements IArenaEntity

    Private g_objArena As IArena
    Private g_ptCurPosition As PointF
    Private g_szSize As Size
    Private g_dblCurAlpha As Double

    Private g_iID As Long

    Public MustOverride ReadOnly Property Size As Size Implements IArenaEntity.Size
    Public MustOverride ReadOnly Property AnimationString As String

    Protected Sub New(ByVal sprites As SpriteCollection)
        g_iID = IDGenerator.GetID

        Me.Alpha = 1

        MyBase.SetSprites(sprites)

        MyBase.SetAnimationSequence(Me.AnimationString, SpriteDirection.None)
    End Sub

    Public ReadOnly Property EntityID As Long Implements IID.EntityID
        Get
            Return g_iID
        End Get
    End Property


    'Public ReadOnly Property ScaledSize As Size Implements IArenaEntity.Size
    '    Get
    '        If Me.Arena IsNot Nothing Then
    '            Return GeneralHelpers.ScaleSize(Me.Size, Me.Arena.ScaleFactor)
    '        End If

    '        Return Me.Size
    '    End Get
    'End Property


    Public Property Alpha As Double
        Get
            Return g_dblCurAlpha
        End Get
        Set(ByVal value As Double)
            g_dblCurAlpha = value
        End Set
    End Property

    Public Property Arena As IArena Implements IArenaEntity.Arena
        Get
            Return g_objArena
        End Get
        Set(ByVal value As IArena)
            g_objArena = value
        End Set
    End Property

    Public ReadOnly Property Bounds As System.Drawing.RectangleF Implements IArenaEntity.Bounds
        Get
            Return New RectangleF(g_ptCurPosition, New SizeF(Me.Size.Width, Me.Size.Height))
        End Get
    End Property

    Public ReadOnly Property Image As RenderInfo Implements IArenaEntity.RenderInfo
        Get
            Dim sp = MyBase.CurrentSprite

            sp.Alpha = g_dblCurAlpha

            Return sp
        End Get
    End Property

    Public ReadOnly Property Position As System.Drawing.PointF Implements IArenaEntity.Position
        Get
            Return g_ptCurPosition
        End Get
    End Property

    Public ReadOnly Property PositionFromCenter As System.Drawing.PointF Implements IArenaEntity.PositionFromCenter
        Get
            Return Me.Bounds.GetCenter
        End Get
    End Property

    Public Sub SetPosition(ByVal pt As PointF)
        g_ptCurPosition = pt
    End Sub

    Protected Overridable ReadOnly Property FadeOut As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overridable ReadOnly Property LoopAnimation As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides Sub Tick()
        If Me.FadeOut Then
            g_dblCurAlpha -= 0.02
        End If

        If g_dblCurAlpha <= 0 Then
            If Me.Arena IsNot Nothing Then
                Me.Arena.RemoveEntity(Me)
            End If
        End If

        MyBase.Tick()
    End Sub

    Protected Overrides Sub OnLastFrameAnimated()
        If Me.Arena IsNot Nothing AndAlso Not Me.LoopAnimation Then
            Me.Arena.RemoveEntity(Me)
        End If

        MyBase.OnLastFrameAnimated()
    End Sub

End Class
