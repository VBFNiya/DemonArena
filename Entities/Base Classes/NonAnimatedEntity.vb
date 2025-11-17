Public MustInherit Class NonAnimatedEntity
    Implements IArenaEntity


    Private g_objArena As IArena
    Private g_ptPosition As PointF
    Private g_szSize As Size
    Private g_dblCurAlpha As Double
    Private g_lID As Long

    Protected MustOverride ReadOnly Property Size As System.Drawing.Size Implements IArenaEntity.Size

    Public Sub New()
        g_dblCurAlpha = 1
        g_lID = IDGenerator.GetID
    End Sub

    'Public ReadOnly Property ScaledSize As Size Implements IArenaEntity.Size
    '    Get
    '        If Me.Arena IsNot Nothing Then
    '            Return GeneralHelpers.ScaleSize(Me.Size, Me.Arena.ScaleFactor)
    '        End If

    '        Return Me.Size
    '    End Get
    'End Property


    Public Property Arena As IArena Implements IArenaEntity.Arena
        Get
            Return g_objArena
        End Get
        Set(ByVal value As IArena)
            g_objArena = value
        End Set
    End Property

    Public ReadOnly Property EntityID As Long Implements IID.EntityID
        Get
            Return g_lID
        End Get
    End Property


    Public ReadOnly Property Bounds As System.Drawing.RectangleF Implements IArenaEntity.Bounds
        Get
            Return New RectangleF(Me.Position, Me.Size)
        End Get
    End Property

    Public ReadOnly Property Position As System.Drawing.PointF Implements IArenaEntity.Position
        Get
            Return g_ptPosition
        End Get
    End Property

    Public ReadOnly Property PositionFromCenter As System.Drawing.PointF Implements IArenaEntity.PositionFromCenter
        Get
            Return Me.Bounds.GetCenter
        End Get
    End Property

    Public ReadOnly Property RenderInfo As RenderInfo Implements IArenaEntity.RenderInfo
        Get
            Return New RenderInfo(Me.Image) With {.Alpha = g_dblCurAlpha}
        End Get
    End Property

    Public Property Image As Sprite

    Public Sub SetPosition(ByVal pos As PointF)
        g_ptPosition = pos
    End Sub

    Public Sub Tick() Implements ITickable.Tick
        If Me.FadeOut Then
            g_dblCurAlpha -= Me.FadeSpeed
        End If

        If g_dblCurAlpha <= 0 Then
            If Me.Arena IsNot Nothing Then
                Me.Arena.RemoveEntity(Me)
            End If
        End If
    End Sub

    Protected Overridable ReadOnly Property FadeSpeed As Double
        Get
            Return 0.05
        End Get
    End Property

    Protected Overridable ReadOnly Property FadeOut As Boolean
        Get
            Return False
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
