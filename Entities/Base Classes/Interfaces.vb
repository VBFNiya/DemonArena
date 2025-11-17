Imports System.Collections.ObjectModel

Public Interface ITickable
    Sub Tick()
End Interface

Public Interface IID
    ReadOnly Property EntityID As Long
End Interface

Public Interface IDamagable
    Sub DeductHitPoints(ByVal damage As Integer)
    Property CurrentHitPoints As Integer
End Interface

Public Interface IProjectileTarget
    Inherits IDamagable

    'Can be used to prevent a projectile from
    'hitting a creature under certain conditions
    ReadOnly Property AllowProjectileHit As Boolean
End Interface

Public Interface IDemon
    Inherits IArenaEntity, IProjectileTarget, IThrustable

    Sub SetPosition(ByVal position As PointF)
    ReadOnly Property Species As String
End Interface


'Entities that implement this
'can react to projectiles that pass
'too close
Public Interface IAlertableEntity
    'The bounds within which
    'a projectile should cause an entity to react
    ReadOnly Property DetectionBounds As RectangleF

    'The projectile that caused the alert
    Sub AlertProjectile(ByVal proj As Projectile)
End Interface

Public Interface IThrustable
    'Use this to prevent thrusting under certain conditions
    'like if dead
    ReadOnly Property CanThrustNow As Boolean
    Sub Thrust(ByVal thrustToPos As PointF, ByVal speed As Double)
End Interface

'Meant for the renderer
Public Interface IGlowingEntity
    'Position and size of the glow
    ReadOnly Property GlowImageBounds As RectangleF
    ReadOnly Property GlowColor As Color
End Interface

Public Interface IArena

    Event EntityListChanged As EventHandler(Of EntityListChangedEventArgs)

    Property ArenaSize As Size
    Property ArenaBackground As Bitmap

    ReadOnly Property Entities As EntityCollection

    Sub AddEntity(ByVal entity As IArenaEntity)
    Sub RemoveEntity(ByVal entity As IArenaEntity)
    Sub CallTick()

End Interface

Public Enum EntityListChangeType
    EntityAdded
    EntityRemoved
End Enum

Public Class EntityListChangedEventArgs
    Inherits EventArgs

    Private _ent As IArenaEntity
    Private _type As EntityListChangeType

    Public Sub New(ByVal entity As IArenaEntity, ByVal changeType As EntityListChangeType)
        _ent = entity
        _type = changeType
    End Sub

    Public ReadOnly Property Entity As IArenaEntity
        Get
            Return _ent
        End Get
    End Property

    Public ReadOnly Property ChangeType As EntityListChangeType
        Get
            Return _type
        End Get
    End Property

End Class

Public Interface IArenaEntity
    Inherits ITickable, IID, IFastListItem

    Property Arena As IArena
    ReadOnly Property Size As Size
    ReadOnly Property Position As PointF
    ReadOnly Property PositionFromCenter As PointF
    ReadOnly Property RenderInfo As RenderInfo
    ReadOnly Property Bounds As RectangleF

End Interface

Public Class EntityCollection
    Inherits ReadOnlyCollection(Of IArenaEntity)

    Public Sub New(ByVal entities As IList(Of IArenaEntity))
        MyBase.New(entities)
    End Sub
End Class