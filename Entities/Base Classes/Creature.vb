Public Enum CreatureState
    Uninitialized
    Roaming
    'Dashing
    Attacking
    Dying
    Thrusted
    ThrustedDying
End Enum

Public Enum RoamState
    Normal
    Dashing
End Enum

'Public Enum RoamingMode
'    Normal
'    Dodging
'End Enum

Public MustInherit Class Creature
    Inherits AnimatedEntityBase
    Implements IDemon, IThrustable
    Implements IAlertableEntity, IFastListItem


    Private Enum GeneralDirection
        Down
        Up
        Left
        Right
    End Enum

    Private Structure ThrustInfo

        'Simply the ratio of the starting distance to
        'the distance at which deceleration begins.
        '0.5 means deceleration would start when half
        'the distance was travelled
        Public Const DECELERATIONRATIO As Double = 0.5

        Public Property ThrustTargetPoint As PointF
        Public Property DecelerationDistance As Double
        Public Property DistanceTravelled As Double
        Public Property OriginalDistance As Double
        Public Property InitialSpeed As Double
    End Structure

    Private g_lID As Long
    Private g_objRnd As New Random
    Private g_vecCurrentMovement As MovementTrajectory
    Private g_ptCurPos As PointF
    Private g_thrustInfo As ThrustInfo
    Private g_iCurTick As Integer
    Private g_objTarget As IArenaEntity
    Private g_enFacing As SpriteDirection
    Private g_iAttackVolliesFired As Integer
    Private g_iAttackTickCount As Integer
    Private g_enMoveState As RoamState
    Private g_szCurrentScaledSize As Size

    Private g_objArena As IArena

    'The attack that was chosen
    Private g_atkChosen As CreatureAttack

    Private g_enCreatureState As CreatureState = CreatureState.Uninitialized

    'Ticks remaining before this defense can be used
    'again
    Private g_iCoolDownTicksRemaining As Integer = 0

    'Ticks remaining before defense goes down again
    Private g_iDefenseTicksRemaining As Integer = 0


    Private g_dblDashSpeed As Double

    Protected MustOverride ReadOnly Property MovementAnimationSequence As String
    Protected MustOverride ReadOnly Property DeathAnimationSequence As String
    Protected MustOverride ReadOnly Property Attacks As IList(Of CreatureAttack)
    Protected MustOverride ReadOnly Property TopSpeed As Double
    Protected MustOverride ReadOnly Property Passivity As Integer
    Protected MustOverride ReadOnly Property MaxHitPoints As Integer
    'Public MustOverride ReadOnly Property Size As Size Implements IArenaEntity.Size

    Protected Sub New(ByVal sprites As SpriteCollection)
        MyBase.SetSprites(sprites)

        g_lID = IDGenerator.GetID

        Me.CurrentHitPoints = Me.MaxHitPoints

        ChangeMovementRandomNormal()

        SetState(CreatureState.Roaming)

        g_ptCurPos = New PointF(0, 0)
    End Sub

    Public ReadOnly Property EntityID As Long Implements IID.EntityID
        Get
            Return g_lID
        End Get
    End Property


    Public Overridable ReadOnly Property Species As String Implements IDemon.Species
        Get
            Return ""
        End Get
    End Property

    Public Property CurrentHitPoints As Integer Implements IProjectileTarget.CurrentHitPoints

    'Public ReadOnly Property ScaledSize As Size Implements IArenaEntity.Size
    '    Get
    '        If Me.Arena IsNot Nothing Then
    '            Return GeneralHelpers.ScaleSize(Me.Size, Me.Arena.ScaleFactor)
    '        End If
    '        Return Me.Size
    '    End Get
    'End Property

    Public ReadOnly Property IsDying As Boolean
        Get
            Return g_enCreatureState = CreatureState.Dying OrElse g_enCreatureState = CreatureState.ThrustedDying
        End Get
    End Property

    Public Property Arena As IArena Implements IArenaEntity.Arena
        Get
            Return g_objArena
        End Get
        Set(ByVal value As IArena)
            g_objArena = value
        End Set
    End Property

    Public ReadOnly Property Image As RenderInfo Implements IArenaEntity.RenderInfo
        Get
            Return Me.CurrentSprite
        End Get
    End Property

    Public ReadOnly Property CurrentPosition As PointF Implements IArenaEntity.Position
        Get
            Return g_ptCurPos
        End Get
    End Property

    Public ReadOnly Property Size As Size Implements IArenaEntity.Size
        Get
            Return g_szCurrentScaledSize
        End Get
    End Property

    Public ReadOnly Property Bounds As RectangleF Implements IArenaEntity.Bounds
        Get
            Return New RectangleF(g_ptCurPos.X, g_ptCurPos.Y, Me.Size.Width, Me.Size.Height)
        End Get
    End Property

    Public ReadOnly Property CurrentPositionCenter As PointF Implements IArenaEntity.PositionFromCenter
        Get
            Return Me.Bounds.GetCenter
        End Get
    End Property

    Public Sub DeductHitPoints(ByVal damage As Integer) Implements IProjectileTarget.DeductHitPoints
        Me.CurrentHitPoints -= damage
    End Sub

    Public Sub Thrust(ByVal toPosition As PointF, ByVal speed As Double) Implements IThrustable.Thrust
        Dim mv As MovementTrajectory = MovementTrajectory.CreateTrajectory(Me.CurrentPositionCenter, toPosition, speed)

        mv.Speed = speed

        g_thrustInfo.ThrustTargetPoint = toPosition
        g_thrustInfo.DecelerationDistance = Geometry.CalculateDistance(toPosition, Me.CurrentPositionCenter) * ThrustInfo.DECELERATIONRATIO
        g_thrustInfo.OriginalDistance = Geometry.CalculateDistance(toPosition, Me.CurrentPositionCenter)
        g_thrustInfo.DistanceTravelled = 0
        g_thrustInfo.InitialSpeed = speed

        If g_thrustInfo.OriginalDistance <> 0 Then
            UpdateMovement(mv, False)
            SetState(CreatureState.Thrusted)
        End If

    End Sub

    Public ReadOnly Property AllowProjectileHit As Boolean Implements IProjectileTarget.AllowProjectileHit, IThrustable.CanThrustNow
        Get
            Return Me.CurrentHitPoints > 0 AndAlso Not g_enMoveState = RoamState.Dashing
        End Get
    End Property

    Public Overrides Sub Tick()


        If MyBase.AllSprites.Count = 0 Then
            Throw New Exception("This creature's sprite collection is empty")
        End If

        If Me.CurrentHitPoints <= 0 Then
            If g_enCreatureState <> CreatureState.Thrusted AndAlso g_enCreatureState <> CreatureState.ThrustedDying Then
                SetState(CreatureState.Dying)
            Else
                SetState(CreatureState.ThrustedDying)
            End If
        End If

        If g_enMoveState = RoamState.Dashing Then
            DepositBlur()

            If g_iDefenseTicksRemaining = 0 Then
                SetMoveState(RoamState.Normal)
            End If

        End If

        Select Case g_enCreatureState
            Case CreatureState.Roaming
                If GeneralHelpers.Chance(Me.Calmness) Then

                    g_iCurTick = 0
                    ChangeMovementRandom()

                End If

                If GeneralHelpers.Chance(Me.Passivity) Then
                    Attack(GetNewTarget)
                End If

            Case CreatureState.Attacking
                g_iAttackTickCount += 1

                If g_atkChosen IsNot Nothing Then
                    If g_atkChosen.CanRoamAndAttack Then
                        If GeneralHelpers.Chance(Me.Calmness) Then
                            ChangeMovementRandom()
                        End If
                    End If
                End If

                FaceTowards(g_objTarget.Bounds.GetCenter)

            Case CreatureState.Thrusted, CreatureState.ThrustedDying
                Dim dist As Double = Geometry.CalculateDistance(Me.CurrentPositionCenter, g_thrustInfo.ThrustTargetPoint)

                If dist < g_thrustInfo.DecelerationDistance Then
                    g_vecCurrentMovement.Speed = (dist / g_thrustInfo.DecelerationDistance) * g_thrustInfo.InitialSpeed
                End If

                If g_thrustInfo.DistanceTravelled > g_thrustInfo.OriginalDistance OrElse g_vecCurrentMovement.Speed <= 0.2 Then
                    If g_enCreatureState <> CreatureState.ThrustedDying Then
                        SetState(CreatureState.Roaming)
                    End If
                End If
        End Select

        g_ptCurPos.X += g_vecCurrentMovement.X
        g_ptCurPos.Y += g_vecCurrentMovement.Y

        If g_enCreatureState = CreatureState.Thrusted OrElse g_enCreatureState = CreatureState.ThrustedDying Then
            g_thrustInfo.DistanceTravelled += g_vecCurrentMovement.Speed
        End If

        '********* Keeps creature within the bounds of the Arena ***********

        With Me.Bounds
            Dim changed As Boolean = False

            If .Left < 0 AndAlso (g_vecCurrentMovement.HorizontalDirection = HorizontalDirection.Left OrElse g_vecCurrentMovement.HorizontalDirection = HorizontalDirection.None) Then
                'g_vecCurrentMovement.SetHorizontalDirection(HorizontalDirection.Right)

                g_vecCurrentMovement.SetTrajectoryMaintainSpeed(GetTrajectoryRandomAngle(GeneralDirection.Right))
                changed = True
            End If

            If .Top < 0 AndAlso (g_vecCurrentMovement.VerticalDirection = VerticalDirection.Up OrElse g_vecCurrentMovement.VerticalDirection = VerticalDirection.None) Then
                'g_vecCurrentMovement.SetVerticalDirection(VerticalDirection.Down)

                g_vecCurrentMovement.SetTrajectoryMaintainSpeed(GetTrajectoryRandomAngle(GeneralDirection.Down))
                changed = True
            End If


            If Me.Arena IsNot Nothing Then
                If .Right > Me.Arena.ArenaSize.Width AndAlso (g_vecCurrentMovement.HorizontalDirection = HorizontalDirection.Right OrElse g_vecCurrentMovement.HorizontalDirection = HorizontalDirection.None) Then
                    'g_vecCurrentMovement.SetHorizontalDirection(HorizontalDirection.Left)

                    g_vecCurrentMovement.SetTrajectoryMaintainSpeed(GetTrajectoryRandomAngle(GeneralDirection.Left))
                    changed = True
                End If

                If .Bottom > Me.Arena.ArenaSize.Height AndAlso (g_vecCurrentMovement.VerticalDirection = VerticalDirection.Down OrElse g_vecCurrentMovement.VerticalDirection = VerticalDirection.None) Then
                    'g_vecCurrentMovement.SetVerticalDirection(VerticalDirection.Up)
                    g_vecCurrentMovement.SetTrajectoryMaintainSpeed(GetTrajectoryRandomAngle(GeneralDirection.Up))
                    changed = True
                End If

            End If

            If changed Then
                g_enFacing = g_vecCurrentMovement.Direction
                MyBase.SetDirection(g_vecCurrentMovement.Direction)

                If g_enCreatureState = CreatureState.Thrusted OrElse g_enCreatureState = CreatureState.ThrustedDying Then
                    Dim trajectory As New PointF(g_vecCurrentMovement.X, g_vecCurrentMovement.Y)
                    Dim target As PointF = Geometry.MovePositionAlongTrajectory(Me.CurrentPositionCenter, trajectory, g_thrustInfo.OriginalDistance - g_thrustInfo.DistanceTravelled)

                    g_thrustInfo.ThrustTargetPoint = target

                    g_vecCurrentMovement = MovementTrajectory.CreateTrajectory(Me.CurrentPositionCenter, target, g_vecCurrentMovement.Speed)
                End If
            End If
        End With


        If g_iDefenseTicksRemaining > 0 Then
            g_iDefenseTicksRemaining -= 1
        Else
            If g_iCoolDownTicksRemaining > 0 Then
                g_iCoolDownTicksRemaining -= 1
            End If
        End If

        MyBase.Tick()
    End Sub

    Public Sub SetPosition(ByVal pos As PointF) Implements IDemon.SetPosition
        g_ptCurPos = pos
    End Sub

    Protected Overridable ReadOnly Property Scale As Double
        Get
            Return 0.5

        End Get
    End Property

    'The higher the value, the less frequently the creature
    'would switch directions when moving

    Protected Overridable ReadOnly Property Calmness As Integer
        Get
            Return 100
        End Get
    End Property


    Protected Overridable ReadOnly Property Defense As CreatureDefense
        Get
            Return Nothing
        End Get
    End Property

    Protected Overrides Sub OnFrameChanged()
        g_szCurrentScaledSize = GeneralHelpers.ScaleSize(Me.CurrentSprite.Sprite.Image.Size, Me.Scale)
    End Sub

    Protected Overrides Sub OnLastFrameAnimated()
        'Note: Remember this event happens after the delay time
        'on the last frame has expired

        Select Case g_enCreatureState
            Case CreatureState.Attacking

                If g_atkChosen.PauseOnLastFrame Then
                    MyBase.PauseAnimation()

                    If g_iAttackVolliesFired > 0 Then

                        'Prevents vollies of projectiles from
                        'firing too fast
                        If g_iAttackTickCount <= g_atkChosen.VolleyDelay Then
                            Return
                        End If
                    End If
                End If

                Dim p As Projectile = SpawnProjectile(g_atkChosen.ProjectileType, Me.Bounds.GetCenter, g_objTarget.Bounds.GetCenter)

                p.HittableEntities = GetViableCreatureTargets()

                If Me.Arena IsNot Nothing Then
                    Me.Arena.AddEntity(p)
                End If
                g_iAttackTickCount = 0

                g_iAttackVolliesFired += 1

                If g_iAttackVolliesFired >= g_atkChosen.Vollies Then
                    g_iAttackVolliesFired = 0

                    MyBase.UnpauseAnimation()

                    SetState(CreatureState.Roaming)
                End If

            Case CreatureState.Dying, CreatureState.ThrustedDying
                Me.Arena.RemoveEntity(Me)
        End Select

    End Sub


    Private ReadOnly Property DashSpeed As Double
        Get
            If g_dblDashSpeed = 0 Then Return g_dblDashSpeed = Me.TopSpeed

            Return g_dblDashSpeed
        End Get
    End Property

    Private Function SpawnProjectile(ByVal projType As Type, ByVal position As PointF, ByVal target As PointF) As Projectile
        'All projectile types must implement a constructor to match
        'the arguments being passed to CreateInstance
        Return Activator.CreateInstance(projType, Me, position.ToPoint, target.ToPoint)
    End Function

    Private Sub SetMoveState(ByVal state As RoamState)

        g_enMoveState = state

        Select Case state
            Case RoamState.Dashing
                g_vecCurrentMovement.Speed = Me.DashSpeed
            Case RoamState.Normal
                g_vecCurrentMovement.Speed = Me.TopSpeed
        End Select

    End Sub

    Private Sub SetState(ByVal state As CreatureState)

        If g_enCreatureState = state Then Return

        MyBase.UnpauseAnimation()

        g_enCreatureState = state

        Select Case state
            Case CreatureState.Roaming

                MyBase.SetAnimationSequence(Me.MovementAnimationSequence,
                                           g_enFacing)


                ChangeMovementRandom()

            Case CreatureState.Attacking
                Dim lstAtk As New List(Of CreatureAttack)

                For Each a As CreatureAttack In Me.Attacks
                    For i = 1 To a.ChanceRatio
                        lstAtk.Add(a)
                    Next
                Next

                g_atkChosen = lstAtk.Item(g_objRnd.Next(0, lstAtk.Count))

                If g_atkChosen.SlowSpeedToAttack Then
                    g_vecCurrentMovement.Speed = 0.2
                End If

                MyBase.SetAnimationSequence(g_atkChosen.AttackAnimationSequence, g_enFacing)

                'Case CreatureState.Dashing

                '    MyBase.SetAnimationSequence(Me.MovementAnimationSequence,
                '               g_enFacing)

            Case CreatureState.Dying, CreatureState.ThrustedDying
                MyBase.SetAnimationSequence(Me.DeathAnimationSequence, g_enFacing, True)

            Case CreatureState.Thrusted
                MyBase.PauseAnimation()

        End Select

    End Sub

    Private Sub Attack(ByVal target As IArenaEntity)
        If target IsNot Nothing Then
            g_objTarget = target

            SetState(CreatureState.Attacking)

            FaceTowards(target.Bounds.GetCenter)
        Else
            g_objTarget = Nothing
        End If
    End Sub

    Private Sub ChangeMovementRandom()
        Select Case g_enMoveState
            Case RoamState.Dashing
                ChangeMovementRandomDash()
            Case RoamState.Normal
                ChangeMovementRandomNormal()
        End Select
    End Sub

    Private Sub ChangeMovementRandomDash()
        UpdateMovement(CreateRandomMoveVector(Me.DashSpeed), True)
    End Sub

    Private Sub ChangeMovementRandomNormal()
        UpdateMovement(CreateRandomMoveVector, True)
    End Sub

    Private Function CreateRandomMoveVector() As MovementTrajectory
        Return CreateRandomMoveVector(-1)
    End Function

    Private Function CreateRandomMoveVector(ByVal speed As Double) As MovementTrajectory
        Dim v As MovementTrajectory

        v = New MovementTrajectory(g_objRnd.Next(-100, 101), g_objRnd.Next(-100, 101))

        If speed <> -1 Then
            v.Speed = speed
        Else
            v.Speed = g_objRnd.Next(1, Me.TopSpeed + 1) '- g_objRnd.NextDouble
        End If

        Return v
    End Function

    Private Sub UpdateMovement(ByVal mv As MovementTrajectory, ByVal changeFacingDirection As Boolean)
        g_vecCurrentMovement = mv

        If changeFacingDirection Then
            SetFacingDirection(mv.AsPointF)
        End If
    End Sub

    Private Sub SetFacingDirection(ByVal direction As SpriteDirection)

        g_enFacing = direction

        'Sets the proper sprites
        MyBase.SetDirection(direction)
    End Sub

    Private Sub SetFacingDirection(ByVal trajectory As PointF)

        g_enFacing = SpriteHelpers.GetDirectionFromTrajectory(trajectory)

        'Sets the proper sprites
        MyBase.SetDirection(trajectory)
    End Sub

    Private Sub FaceTowards(ByVal pt As PointF)
        Dim mv As MovementTrajectory = MovementTrajectory.CreateTrajectory(Me.CurrentPosition.ToPoint, pt.ToPoint, 10)

        'SetFacingDirection(mv.Direction)
        SetFacingDirection(mv.AsPointF)
        'UpdateMovement(MovementVector.CreateVector(Me.CurrentPosition.ToPoint, pt.ToPoint, g_vecCurrentMovement.Speed))
    End Sub

    Private Function GetNewTarget() As IArenaEntity
        If g_objArena IsNot Nothing Then
            Dim viableTargets = GetViableCreatureTargets()

            If viableTargets.Count > 0 Then
                Return viableTargets(g_objRnd.Next(0, viableTargets.Count))
            End If
        End If
        Return Nothing
        'g_objTarget = Nothing
    End Function

    'All creature not of this same type would
    'be viable targets.
    Private Function GetViableCreatureTargets() As IList(Of IArenaEntity)

        Dim lst As New List(Of IArenaEntity)

        If g_objArena Is Nothing Then
            Return New List(Of IArenaEntity)
        Else
            'Return g_objArena.Entities.Where(Function(e) TypeOf e Is IDemon AndAlso e.GetType IsNot Me.GetType).ToArray
            Return g_objArena.Entities.Where(
                        Function(ent) TypeOf ent Is IDemon AndAlso (Me.GetType <> ent.GetType AndAlso (Me.Species = "" OrElse Not Me.Species.Equals(DirectCast(ent, IDemon).Species, StringComparison.CurrentCultureIgnoreCase)))).ToArray()
        End If

    End Function

    Private Sub AlertProjectile(ByVal proj As Projectile) Implements IAlertableEntity.AlertProjectile

        If Me.Defense IsNot Nothing Then
            If g_iCoolDownTicksRemaining = 0 Then

                g_iCoolDownTicksRemaining = Me.Defense.CoolDownInTicks.Item(g_objRnd.Next(0, Me.Defense.CoolDownInTicks.Count))
                g_iDefenseTicksRemaining = Me.Defense.DurationInTicks.Item(g_objRnd.Next(0, Me.Defense.DurationInTicks.Count))

                If TypeOf Me.Defense Is DashDefense Then
                    g_dblDashSpeed = DirectCast(Me.Defense, DashDefense).DashSpeed

                    SetMoveState(RoamState.Dashing)
                End If
            End If
        End If


    End Sub

    Private Function RectOutsideArena(ByVal rect As RectangleF) As Boolean

        If rect.Right > Me.Arena.ArenaSize.Width Then Return True
        If rect.Bottom > Me.Arena.ArenaSize.Height Then Return True
        If rect.X < 0 Then Return True
        If rect.Y < 0 Then Return True
        Return False
    End Function

    Private Sub DepositBlur()
        If Me.Arena IsNot Nothing Then
            Static tickCount As Integer = 0

            If tickCount >= 5 Then
                Dim blur As New MoveBlur

                blur.Image = Me.CurrentSprite.Sprite
                blur.BlurSize = Me.Size
                blur.SetPosition(Me.CurrentPosition)

                Me.Arena.AddEntity(blur)

                tickCount = 0
            End If
            tickCount += 1

        End If
    End Sub

    Private ReadOnly Property DetectionBounds As System.Drawing.RectangleF Implements IAlertableEntity.DetectionBounds
        Get
            Dim bounds As New RectangleF(0, 0, Me.Size.Width * 4, Me.Size.Height * 4)
            bounds.CenterRectInRect(Me.Bounds)

            Return bounds
        End Get
    End Property

    'Private Function ScaleSpeed(ByVal speed As Double) As Double
    '    If Me.Arena Is Nothing Then
    '        Return speed
    '    Else
    '        Return speed * Me.Arena.ScaleFactor
    '    End If
    'End Function

    Private Function GetTrajectoryRandomAngle(ByVal direction As GeneralDirection) As PointF

        Dim halfAngle As Double = -80
        Dim change As Double = g_objRnd.Next(halfAngle, Math.Abs(halfAngle) + 1)
        Dim angle As Double

        Select Case direction
            Case GeneralDirection.Down
                angle = 90 + change
            Case GeneralDirection.Right
                angle = 0 + change
            Case GeneralDirection.Left
                angle = 180 + change
            Case GeneralDirection.Up
                angle = 270 + change
        End Select

        Return Geometry.GetTrajectoryFromAngle(angle)
    End Function

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

Public MustInherit Class CreatureDefense

    'How long should this defense stay up.
    'Different durations chosen at random
    Public Property DurationInTicks As IList(Of Integer)

    'How long before this creature can use this
    'defense after its last use. Different cooldowns are
    'chosen at random
    Public Property CoolDownInTicks As IList(Of Integer)
End Class

Public Class DashDefense
    Inherits CreatureDefense

    Public Sub New()
    End Sub

    Public Sub New(ByVal duration As IList(Of Integer), ByVal coolDown As IList(Of Integer), ByVal dashSpeed As Integer)
        Me.CoolDownInTicks = coolDown
        Me.DurationInTicks = duration
        Me.DashSpeed = dashSpeed
    End Sub

    Public Property DashSpeed As Double
End Class

Public Class CreatureAttack
    Public Sub New(ByVal attackAni As String, ByVal projectile As Type)
        Me.AttackAnimationSequence = attackAni
        Me.ProjectileType = projectile
        Me.Vollies = 1
        Me.ChanceRatio = 1
        Me.CanRoamAndAttack = False
    End Sub

    Public Sub New(ByVal attackAni As String, ByVal projectile As Type, ByVal vollies As Integer)
        'Me.AttackAnimationSequence = attackAni
        'Me.ProjectileType = projectile
        Me.New(attackAni, projectile)
        Me.Vollies = vollies
    End Sub

    Public Property AttackAnimationSequence As String
    Public Property ProjectileType As Type
    Public Property Vollies As Integer
    Public Property SlowSpeedToAttack As Boolean
    Public Property CanRoamAndAttack As Boolean


    'Fires all vollies on the last frame
    Public Property PauseOnLastFrame As Boolean

    'Only effective when PauseOnLastFrame is True
    Public Property VolleyDelay As Integer

    'A ratio value that determines how
    'often this attack would be chosen
    Public Property ChanceRatio As Integer
End Class

#Region "Dead Code"

'Private Function GetEscapeTrajectory(ByVal proj As Projectile) As MovementTrajectory

'    Dim dodgeAng = Geometry.GetAngleOfTarget(Me.CurrentPositionCenter, proj.PositionFromCenter)

'    dodgeAng += 90

'    If dodgeAng >= 360 Then dodgeAng -= 360

'    Dim dodgeTraj As PointF = Geometry.GetRiseRunFromAngle(dodgeAng)

'    Dim mv As New MovementTrajectory(dodgeTraj.X, dodgeTraj.Y)
'    mv.Speed = Me.TopSpeed

'    'UpdateMovement(mv, True)

'    Return mv
'End Function

'Private Function FindSafeTrajectory(ByVal ticks As Integer) As PointF
'    Dim safe As Boolean
'    Dim hostileProjectiles = Me.Arena.Entities.OfType(Of Projectile).Where(Function(p) p.HittableEntities.Contains(Me))

'    For i = 0 To 359
'        Dim traj As PointF = Geometry.GetTrajectoryFromAngle(i)

'        Dim m As New MovementTrajectory(traj.X, traj.Y)
'        m.Speed = TopSpeed

'        safe = True
'        For t = 1 To ticks
'            Dim creatureProjectedPosition As PointF = Me.CurrentPosition

'            creatureProjectedPosition.X += t * m.X
'            creatureProjectedPosition.Y += t * m.Y

'            For Each proj As Projectile In hostileProjectiles
'                Dim projectileProjectedPosition As PointF = proj.Position

'                projectileProjectedPosition.X += t * proj.MovementVector.X
'                projectileProjectedPosition.Y += t * proj.MovementVector.Y

'                Dim projectedRect As New RectangleF(projectileProjectedPosition, proj.Bounds.Size)
'                Dim creatureProjectedRect As New RectangleF(creatureProjectedPosition, Me.Size)

'                If projectedRect.IntersectsWith(creatureProjectedRect) OrElse RectOutsideArena(creatureProjectedRect) Then
'                    safe = False
'                    Exit For
'                End If
'            Next
'        Next

'        If safe Then
'            Return m.AsPointF
'        End If

'    Next

'    If safe = False Then Return New PointF(0, 0)
'End Function


#End Region