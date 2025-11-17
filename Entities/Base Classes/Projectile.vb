Public Enum ProjectileState
    Uninitialized
    Moving
    Dying
    Dead
End Enum

Public MustInherit Class Projectile
    Inherits AnimatedEntityBase
    Implements IArenaEntity
    Implements IGlowingEntity
    Implements IFastListItem

    Private g_objArena As IArena
    Private g_ptCurPosition As PointF
    Private g_vecMovement As MovementTrajectory
    Private g_entOwner As IArenaEntity
    Private g_lID As Long
    Private g_szCurrentScaledSize As Size

    Private g_enState As ProjectileState = ProjectileState.Uninitialized

    'Protected MustOverride ReadOnly Property Size As System.Drawing.Size Implements IArenaEntity.Size
    Protected MustOverride ReadOnly Property Speed As Double
    Protected MustOverride ReadOnly Property ExplodeAnimationString As String
    Protected MustOverride ReadOnly Property MovementAnimationString As String
    Protected MustOverride ReadOnly Property Damage As Integer
    Protected MustOverride ReadOnly Property GlowColor As Color Implements IGlowingEntity.GlowColor

    Protected Sub New(ByVal sprites As SpriteCollection, ByVal owner As IArenaEntity, ByVal source As PointF, ByVal target As PointF)
        MyBase.SetSprites(sprites)

        g_lID = IDGenerator.GetID

        g_ptCurPosition = source

        g_vecMovement = MovementTrajectory.CreateTrajectory(source.ToPoint, target.ToPoint, Me.Speed)

        g_entOwner = owner

        SetState(ProjectileState.Moving)
    End Sub

    Protected Sub New(ByVal sprites As SpriteCollection, ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.SetSprites(sprites)

        g_lID = IDGenerator.GetID
        g_ptCurPosition = position
        g_entOwner = owner

    End Sub


    Public Property HittableEntities As IList(Of IArenaEntity)

    Public ReadOnly Property EntityID As Long Implements IID.EntityID
        Get
            Return g_lID
        End Get
    End Property

    Public ReadOnly Property Owner As IArenaEntity
        Get
            Return g_entOwner
        End Get
    End Property

    Public ReadOnly Property GlowImageBounds As System.Drawing.RectangleF Implements IGlowingEntity.GlowImageBounds
        Get
            Dim b As New RectangleF(New PointF(0, 0), Me.GlowSize)

            b.CenterRectInRect(Me.Bounds)

            Return b
        End Get
    End Property

    Public Property Arena As IArena Implements IArenaEntity.Arena
        Get
            Return g_objArena
        End Get
        Set(ByVal value As IArena)
            g_objArena = value

            If g_objArena IsNot Nothing Then
                g_vecMovement.Speed = g_vecMovement.Speed
            End If

        End Set
    End Property


    Public ReadOnly Property Size As Size Implements IArenaEntity.Size
        Get
            'Return GeneralHelpers.ScaleSize(Me.CurrentSprite.Sprite.Image.Size, Me.Scale)
            Return g_szCurrentScaledSize
        End Get
    End Property

    Public ReadOnly Property Bounds As System.Drawing.RectangleF Implements IArenaEntity.Bounds
        Get
            Return New RectangleF(g_ptCurPosition.X,
                                  g_ptCurPosition.Y,
                                  Me.Size.Width,
                                  Me.Size.Height)

        End Get
    End Property

    Public ReadOnly Property PositionFromCenter As System.Drawing.PointF Implements IArenaEntity.PositionFromCenter
        Get
            Return Me.Bounds.GetCenter
        End Get
    End Property

    Public ReadOnly Property Image As RenderInfo Implements IArenaEntity.RenderInfo
        Get
            Return MyBase.CurrentSprite
        End Get
    End Property

    Public ReadOnly Property Position As System.Drawing.PointF Implements IArenaEntity.Position
        Get
            Return g_ptCurPosition
        End Get
    End Property

    Public ReadOnly Property MovementVector As PointF
        Get
            Return g_vecMovement.AsPointF
        End Get
    End Property

    Public Sub SetTrajectory(ByVal traj As PointF)
        SetState(ProjectileState.Uninitialized)

        g_vecMovement = New MovementTrajectory(traj.X, traj.Y)
        g_vecMovement.Speed = Me.Speed

        SetState(ProjectileState.Moving)

    End Sub

    Protected Overridable ReadOnly Property Scale As Double
        Get
            Return 0.5
        End Get
    End Property


    Public Overrides Sub Tick()
        If g_enState = ProjectileState.Moving Then
            With g_ptCurPosition
                .X += g_vecMovement.X
                .Y += g_vecMovement.Y
            End With

            If Me.Bounds.Right >= Me.Arena.ArenaSize.Width OrElse
               Me.Bounds.Left <= 0 OrElse
               Me.Bounds.Top < 0 OrElse
               Me.Bounds.Bottom > Me.Arena.ArenaSize.Height Then

                SetState(ProjectileState.Dying)
                If Me.IsExplosive Then
                    Explode()
                End If

            End If

            If Me.Arena IsNot Nothing Then

                For Each entity In Me.HittableEntities.OfType(Of IProjectileTarget)()
                    Dim castAsArenaEntity = DirectCast(entity, IArenaEntity)

                    If entity.AllowProjectileHit AndAlso Me.Bounds.IntersectsWith(castAsArenaEntity.Bounds) AndAlso Me.Arena.Entities.Contains(entity) Then
                        SetState(ProjectileState.Dying)

                        'If its explosive then we
                        'let the explosive logic handle
                        'damage
                        If Not Me.IsExplosive Then
                            'If TypeOf entity Is IProjectileTarget Then
                            'Dim e2 As IProjectileTarget = DirectCast(entity, IProjectileTarget)

                            'e2.DeductHitPoints(Me.Damage)
                            entity.DeductHitPoints(Me.Damage)
                            'End If
                        Else
                            Explode()
                        End If
                    End If


                    If TypeOf entity Is IAlertableEntity Then
                        Dim ae As IAlertableEntity = DirectCast(entity, IAlertableEntity)

                        If Me.Bounds.IntersectsWith(ae.DetectionBounds) Then
                            ae.AlertProjectile(Me)
                        End If
                    End If

                Next

            End If
        End If

        If g_enState = ProjectileState.Dying Then
            If MyBase.IsLastFrame() Then
                SetState(ProjectileState.Dead)
                If Me.Arena IsNot Nothing Then
                    Me.Arena.RemoveEntity(Me)
                End If
            End If
        End If

        MyBase.Tick()
    End Sub

    Protected Overridable ReadOnly Property IsExplosive As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overridable ReadOnly Property GlowSize As Size
        Get
            Return Me.Size + New Size(10, 10)
        End Get
    End Property

    Protected Overrides Sub OnFrameChanged()
        g_szCurrentScaledSize = GeneralHelpers.ScaleSize(Me.CurrentSprite.Sprite.Image.Size, Me.Scale)
    End Sub

    Private Sub SetState(ByVal state As ProjectileState)

        If g_enState = state Then Return

        g_enState = state

        Select Case state
            Case ProjectileState.Moving

                MyBase.SetAnimationSequence(Me.MovementAnimationString,
                                            g_vecMovement.Direction)

            Case ProjectileState.Dying
                MyBase.SetAnimationSequence(Me.ExplodeAnimationString, g_vecMovement.Direction)
        End Select
    End Sub

    'Private Function ScaleValue(ByVal speed As Double) As Double
    '    If Me.Arena Is Nothing Then
    '        Return speed
    '    Else
    '        Return speed * Me.Arena.ScaleFactor
    '    End If
    'End Function

    Private Sub Explode()
        Dim exp As New BlueExplosion
        Dim expBounds = exp.Bounds

        expBounds.CenterRectInRect(Me.Bounds)

        exp.SetPosition(expBounds.Location)

        Me.Arena.AddEntity(exp)

        ApplyExplosionEffect()
    End Sub

    Private Sub ApplyExplosionEffect()
        If Me.Arena IsNot Nothing Then
            Dim affected = Me.HittableEntities.Where(Function(et) DirectCast(et, IProjectileTarget).AllowProjectileHit)

            Dim RADIUS As Integer = 250

            Dim MAXDAMAGE = 120

            Dim MAXTHRUSTDISTANCE As Integer = 300

            For Each entity As IArenaEntity In affected
                Dim entityCenter As PointF = entity.PositionFromCenter
                Dim meCenter As PointF = Me.PositionFromCenter

                Dim distance As Double = Geometry.CalculateDistance(entityCenter, meCenter)

                If distance <= RADIUS Then
                    Dim damage As Integer = ((RADIUS - distance) / RADIUS) * Me.Damage
                    If damage < 0 Then damage = 0

                    Dim thrustDistance As Double = (damage / MAXDAMAGE) * MAXTHRUSTDISTANCE

                    If TypeOf entity Is IThrustable AndAlso DirectCast(entity, IThrustable).CanThrustNow Then
                        'This calculation doesn't represen a position,
                        'it represents the movement direction of an entity
                        'based on where the explosion occured
                        Dim trajectory As PointF = New PointF(entity.PositionFromCenter.X - Me.PositionFromCenter.X,
                                                          entity.PositionFromCenter.Y - Me.PositionFromCenter.Y)
                        Dim thrustDestination As PointF =
                        Geometry.MovePositionAlongTrajectory(entityCenter, trajectory, thrustDistance)

                        DirectCast(entity, IThrustable).Thrust(thrustDestination, 10)
                    End If

                    If TypeOf entity Is IProjectileTarget Then
                        DirectCast(entity, IProjectileTarget).DeductHitPoints(damage)
                    End If
                End If
            Next
        End If

    End Sub


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
