Public Enum TargetSelectionType
    AnyCreature
    DifferentSpecies
    DerivedClassDefined
End Enum

Public Enum CreatureFlags
    MoveBlur
    GhostMode
    ArenaEdgeAlertOn
End Enum

Public MustInherit Class ComplexCreature
    Inherits ComplexEntityBase
    Implements IDemon

    Private Enum GeneralDirection
        Down
        Up
        Left
        Right
    End Enum
    Private Const TOO_SLOW As Double = 0.1

    Private g_bStarted As Boolean
    Private _rnd As New Random
    Private WithEvents _arena As IArena
    Private g_ptCurPos As PointF
    Private g_vecCurrentMovement As MovementTrajectory
    Private g_objTarget As IArenaEntity
    Private g_lstViableTargets As New List(Of IArenaEntity)
    Private g_objLocalVariables As New VariableManager
    Private g_szCurrentScaledSize As Size

    Private g_iCurHitpoints As Integer
    Private g_lstCreatureDef As New List(Of DefinitionExpression)
    Private g_dblDeceleration As Double

    Private g_bMoveBlur As Boolean

    'Will jump to special state when creature hits arena edge
    Private g_bCheckArenaEdge As Boolean

    'We don't use this one for movement,
    'only to represent the direction we are facing
    Private g_vecFacingDirection As MovementTrajectory

    'Projectiles will pass through is true
    Private g_bProjPassThrough As Boolean

    'Public MustOverride ReadOnly Property Size As System.Drawing.Size Implements IArenaEntity.Size
    Protected MustOverride ReadOnly Property AgilityPercent As Integer
    Protected MustOverride ReadOnly Property CreatureDefinition As IList(Of DefinitionExpression)
    Protected MustOverride ReadOnly Property TopSpeed As Double
    Protected MustOverride ReadOnly Property MaxHitPoints As Integer

    Protected Sub New(ByVal sprites As SpriteCollection)
        SetMovement(New MovementTrajectory(0, 0))

        g_lID = IDGenerator.GetID

        Me.Sprites = sprites
        Me.ActorDefinition = Me.CreatureDefinition
        Me.CurrentHitPoints = Me.MaxHitPoints

        g_bMoveBlur = False
        g_bProjPassThrough = False
        g_bCheckArenaEdge = False

        g_dblDeceleration = 0

        g_bStarted = False
    End Sub

#Region "IProjectileTarget"
    Public ReadOnly Property AllowProjectileHit As Boolean Implements IProjectileTarget.AllowProjectileHit
        Get
            Return Me.CurrentHitPoints > 0 AndAlso Not g_bProjPassThrough
        End Get
    End Property

    Public Property CurrentHitPoints As Integer Implements IProjectileTarget.CurrentHitPoints
        Get
            Return g_iCurHitpoints
        End Get
        Set(ByVal value As Integer)
            g_iCurHitpoints = value
        End Set
    End Property

    Public Sub DeductHitPoints(ByVal damage As Integer) Implements IProjectileTarget.DeductHitPoints
        Me.CurrentHitPoints -= damage

        'If we are still alive
        If Me.CurrentHitPoints > 0 Then
            If MyBase.HasState(StandardStateLabels.Flinch.LabelName) AndAlso Not MyBase.CurrentStateIs(StandardStateLabels.Thrusted.LabelName) Then
                If GeneralHelpers.RollDice(Me.FlinchChance) Then
                    ECMD_Goto(StandardStateLabels.Flinch.LabelName)
                End If
            End If
        End If

    End Sub

#End Region

#Region "IThrustable"
    Public ReadOnly Property CanThrustNow As Boolean Implements IThrustable.CanThrustNow
        Get
            If MyBase.HasState(StandardStateLabels.Thrusted.LabelName) Then
                If Me.CurrentHitPoints > 0 Then
                    Return True
                End If
            End If
            Return False
        End Get
    End Property

    Public Sub Thrust(ByVal thrustToPos As System.Drawing.PointF, ByVal speed As Double) Implements IThrustable.Thrust
        ECMD_Goto(StandardStateLabels.Thrusted.LabelName)

        g_vecCurrentMovement.SetTrajectory(Me.PositionFromCenter, thrustToPos, speed)
        Dim dist As Double = Geometry.CalculateDistance(Me.PositionFromCenter, thrustToPos)

        'This calculation would ensure that the creature would have 
        'travelled the correct
        'distance when the speed reaches 0
        g_dblDeceleration = (speed ^ 2) / (dist * 2)

    End Sub
#End Region
#Region "IID"
    Private g_lID As Long
    Public ReadOnly Property EntityID As Long Implements IID.EntityID
        Get
            Return g_lID
        End Get
    End Property

#End Region

    Public Property Arena As IArena Implements IArenaEntity.Arena
        Get
            Return _arena
        End Get
        Set(ByVal value As IArena)
            _arena = value
        End Set
    End Property

    Public Overridable ReadOnly Property Species As String Implements IDemon.Species
        Get
            Return ""
        End Get
    End Property


    Public ReadOnly Property Size As Size Implements IArenaEntity.Size
        Get
            'If Me.CurrentRI IsNot Nothing Then
            '    Return GeneralHelpers.ScaleSize(Me.CurrentRI.Sprite.Image.Size, Me.Scale)
            'Else
            '    Return New Size(0, 0)
            'End If

            Return g_szCurrentScaledSize

        End Get
    End Property

    Public ReadOnly Property Bounds As System.Drawing.RectangleF Implements IArenaEntity.Bounds
        Get
            Return New RectangleF(Me.Position, Me.Size)
        End Get
    End Property

    Public ReadOnly Property Position As System.Drawing.PointF Implements IArenaEntity.Position
        Get
            Return g_ptCurPos
        End Get
    End Property

    Public ReadOnly Property PositionFromCenter As System.Drawing.PointF Implements IArenaEntity.PositionFromCenter
        Get
            Return Me.Bounds.GetCenter
        End Get
    End Property

    Public ReadOnly Property RenderInfo As RenderInfo Implements IArenaEntity.RenderInfo
        Get
            Return Me.CurrentRI
        End Get
    End Property

    Public Sub SetPosition(ByVal pos As PointF) Implements IDemon.SetPosition
        g_ptCurPos = pos
    End Sub

    Protected Overridable ReadOnly Property Scale As Double
        Get
            Return 0.5
        End Get
    End Property

    Protected Overridable ReadOnly Property TargetSelection As TargetSelectionType
        Get
            Return TargetSelectionType.DifferentSpecies
        End Get
    End Property

    Protected Overridable ReadOnly Property FlinchChance As Integer
        Get
            Return 0
        End Get
    End Property

    'This is called by the base class whenever the rendering information has changed
    Protected Overrides Sub OnRenderInfoChanged()
        If Me.CurrentRI IsNot Nothing Then
            g_szCurrentScaledSize = GeneralHelpers.ScaleSize(Me.CurrentRI.Sprite.Image.Size, Me.Scale)
        Else
            g_szCurrentScaledSize = New Size(0, 0)
        End If
    End Sub

    Protected Function EFUNC_GetID() As Long
        Return g_lID
    End Function

    Protected Function EFUNC_GetTargetID() As Long
        If Me.Arena IsNot Nothing Then
            If g_objTarget IsNot Nothing Then
                If Me.Arena.Entities.Contains(g_objTarget) Then
                    Return DirectCast(g_objTarget, IID).EntityID
                End If
            End If
        End If
        Return 0
    End Function
    Protected Sub ECMD_SetAlpha(ByVal alpha As Double)
        MyBase.Alpha = alpha
    End Sub

    Protected Sub ECMD_SubtractAlpha(ByVal value As Double)
        MyBase.Alpha -= value
    End Sub

    Protected Sub ECMD_AddAlpha(ByVal value As Double)
        MyBase.Alpha += value
    End Sub

    Protected Function EFUNC_GetLocalVarLong(ByVal varName As String) As Long
        Return g_objLocalVariables.GetLongValue(varName)
    End Function

    Protected Function EFUNC_GetGlobalVarLong(ByVal varName As String) As Long
        Return VariableManager.GlobalManager.GetLongValue(varName)
    End Function

    Protected Sub ECMD_SetLocalVarLong(ByVal varName As String, ByVal value As Long)
        g_objLocalVariables.SetLongValue(varName, value)
    End Sub

    Protected Sub ECMD_SetGlobalVarLong(ByVal varName As String, ByVal value As Long)
        VariableManager.GlobalManager.SetLongValue(varName, value)
    End Sub

    Protected Sub ECMD_SetDeceleration(ByVal d As Double)
        g_dblDeceleration = d
    End Sub

    Protected Sub EMCD_JumpIfTrue(ByVal booleanResult As Boolean, ByVal stateLabel As String)
        If booleanResult Then
            ECMD_Goto(stateLabel)
        End If
    End Sub

    Protected Sub EMCD_JumpIfFalse(ByVal booleanResult As Boolean, ByVal stateLabel As String)
        If Not booleanResult Then
            ECMD_Goto(stateLabel)
        End If
    End Sub

    Protected Sub ECMD_JumpIfTargetValid(ByVal stateLabel As String)
        If Me.Arena IsNot Nothing Then
            If g_objTarget IsNot Nothing Then
                If Me.Arena.Entities.Contains(g_objTarget) Then
                    If DirectCast(g_objTarget, IDamagable).CurrentHitPoints > 0 Then
                        ECMD_Goto(stateLabel)
                    End If
                End If
            End If
        End If

    End Sub

    Protected Sub ECMD_JumpIfNotMoving(ByVal stateLabel As String)
        'ECMD_JumpIfSpeedLessThan(TOO_SLOW, stateLabel)
        If g_vecCurrentMovement.Speed = 0 Then
            ECMD_Goto(stateLabel)
        End If

    End Sub

    Protected Sub ECMD_JumpIfSpeedLessThan(ByVal speed As Double, ByVal stateLabel As String)
        If g_vecCurrentMovement.Speed < speed Then
            ECMD_Goto(stateLabel)
        End If
    End Sub

    Protected Sub ECMD_Goto(ByVal stateLabel As String)
        MyBase.GotoState(stateLabel)
    End Sub

    Protected Sub ECMD_Start()
        If g_bStarted Then
            Return
        Else
            g_bStarted = True
        End If

        Dim v = CreateRandomTrajectory()
        v.Speed = TopSpeed

        SetMovement(v)
    End Sub

    Protected Sub ECMD_AllowStart()
        g_bStarted = False
    End Sub

    Protected Sub EMCD_GotoParallel(ByVal state As String)
        MyBase.ExecuteStateParallel(state)
    End Sub

    Protected Sub ECMD_Yield()
        MyBase.Yield(1)
    End Sub

    Protected Sub ECMD_Yield(ByVal cycles As Integer)
        MyBase.Yield(cycles)
    End Sub

    Protected Sub ECMD_SetSpeed(ByVal speed As Double)
        g_vecCurrentMovement.Speed = speed
    End Sub

    Protected Sub ECMD_CreateTimerRandomInterval(ByVal min As Integer, ByVal max As Integer, ByVal handlerState As String)
        Dim ticks As Integer = _rnd.Next(min, max + 1)

        ECMD_CreateTimer(ticks, handlerState)
    End Sub

    Protected Sub ECMD_CreateTimer(ByVal ticks As Integer, ByVal handlerState As String)
        If Not MyBase.HasTimer(handlerState) Then
            MyBase.CreateTimer(ticks, handlerState, True)
        End If
    End Sub

    Protected Sub ECMD_DestroyTimer(ByVal handlerState As String)
        MyBase.RemoveTimer(handlerState)
    End Sub

    Protected Sub ECMD_CreatePermanentTimer(ByVal ticks As Integer, ByVal handlerState As String)
        If Not MyBase.HasTimer(handlerState) Then
            MyBase.CreateTimer(ticks, handlerState, False)
        End If
    End Sub

    Protected Sub ECMD_RemoveMe()
        If Me.Arena IsNot Nothing Then
            Me.Arena.RemoveEntity(Me)
        End If
    End Sub

    Protected Sub ECMD_ClearTarget()
        g_objTarget = Nothing
    End Sub

    Protected Sub ECMD_TryAcquireMeleeTargetAndJump(ByVal state As String)
        ECMD_SelectMeleeTarget()

        If Me.Target IsNot Nothing Then
            ECMD_Goto(state)
        End If
    End Sub

    Protected Sub ECMD_JumpIfTargetAcquired(ByVal state As String)
        If Me.Target IsNot Nothing Then
            ECMD_Goto(state)
        End If
    End Sub

    Protected Sub ECMD_JumpIfNoTarget(ByVal state As String)
        If Me.Target Is Nothing Then
            ECMD_Goto(state)
        End If
    End Sub

    Protected Sub ECMD_SelectTargetByID(ByVal targetID As Long)
        If Me.Arena IsNot Nothing Then
            Dim ent As IArenaEntity = Me.Arena.Entities.OfType(Of IID).FirstOrDefault(Function(e) e.EntityID = targetID)

            g_objTarget = ent
        End If
    End Sub

    Protected Sub ECMD_SelectNewTarget()
        Dim targets As IList(Of IArenaEntity) = g_lstViableTargets

        If targets.Count <> 0 Then
            Dim selectedTarget As IArenaEntity = targets.Item(_rnd.Next(0, targets.Count))

            'Make sure not to select the same target unless there is only
            'one target
            If targets.Count > 1 AndAlso g_objTarget IsNot Nothing AndAlso g_objTarget Is selectedTarget Then
                Do Until selectedTarget IsNot g_objTarget
                    selectedTarget = targets.Item(_rnd.Next(0, targets.Count))
                Loop
            End If

            g_objTarget = selectedTarget
        Else
            g_objTarget = Nothing
        End If

    End Sub

    Protected Sub ECMD_ThrustTarget(ByVal distance As Double, ByVal speed As Double)
        If Me.Target IsNot Nothing Then
            Dim cast As IThrustable

            cast = TryCast(Me.Target, IThrustable)

            If cast IsNot Nothing AndAlso cast.CanThrustNow Then
                Dim mv As MovementTrajectory = MovementTrajectory.CreateTrajectory(Me.PositionFromCenter, Me.Target.PositionFromCenter, 1)
                Dim thrustTo As PointF = Geometry.MovePositionAlongTrajectory(Me.Target.PositionFromCenter, mv.AsPointF, distance)

                cast.Thrust(thrustTo, speed)
            End If
        End If
    End Sub


    Protected Sub ECMD_SetFlag(ByVal flag As CreatureFlags, ByVal state As Boolean)
        Select Case flag
            Case CreatureFlags.MoveBlur
                g_bMoveBlur = state

            Case CreatureFlags.GhostMode
                g_bProjPassThrough = state

            Case CreatureFlags.ArenaEdgeAlertOn
                g_bCheckArenaEdge = state
        End Select
    End Sub

    'Protected Function EREQ_HasValidTarget() As Boolean
    '    If g_objTarget Is Nothing Then
    '        Return False
    '    Else
    '        If Me.Arena IsNot Nothing Then
    '            If Me.Arena.Entities.Contains(g_objTarget) Then
    '                Return True
    '            Else
    '                Return False
    '            End If
    '        Else
    '            Return False
    '        End If
    '    End If

    'End Function

    Protected Sub ECMD_AttackRandomTarget(ByVal attackState As String, ByVal chance As Integer)

        If GeneralHelpers.RollDice(chance) Then
            ECMD_SelectNewTarget()

            If Me.Target IsNot Nothing Then
                ECMD_Goto(attackState)
            End If
        End If

    End Sub

    Protected Sub ECMD_FireProjAtTarget(ByVal projType As Type)
        If Me.Arena IsNot Nothing AndAlso Me.Target IsNot Nothing Then
            Dim proj As Projectile = Activator.CreateInstance(projType, Me, Me.PositionFromCenter)

            Dim mv = MovementTrajectory.CreateTrajectory(Me.Position, g_objTarget.Position, 1)

            ECMD_FireProjectile(projType, Geometry.GetAngleInDegrees(mv.AsPointF))
        End If
    End Sub

    Protected Sub ECMD_FireProjectile(ByVal projType As Type, ByVal angle As Double)

        If Me.Arena IsNot Nothing Then
            Dim proj As Projectile = Activator.CreateInstance(projType, Me, Me.PositionFromCenter)

            Dim traj As PointF = Geometry.GetTrajectoryFromAngle(angle)

            proj.SetTrajectory(traj)
            proj.HittableEntities = g_lstViableTargets 'GetViableTargets()

            Me.Arena.AddEntity(proj)
        End If

    End Sub
    Protected Sub ECMD_JumpIfFarFromTarget(ByVal distance As Double, ByVal state As String)
        If Me.Target IsNot Nothing Then
            Dim dist = Geometry.CalculateDistance(Me.PositionFromCenter, Me.Target.PositionFromCenter)

            If dist > distance Then
                ECMD_Goto(state)
            End If
        End If
    End Sub

    Protected Sub ECMD_JumpIfCloseToTarget(ByVal distance As Double, ByVal state As String)
        If Me.Target IsNot Nothing Then
            Dim dist = Geometry.CalculateDistance(Me.PositionFromCenter, Me.Target.PositionFromCenter)

            If dist <= distance Then
                ECMD_Goto(state)
            End If
        End If
    End Sub

    Protected Sub ECMD_FireProjectileFoward(ByVal projType As Type, ByVal offsetAngleBy As Double)
        Dim traj As PointF = g_vecFacingDirection.AsPointF
        Dim currentAngle As Double = Geometry.GetAngleInDegrees(traj)

        currentAngle += offsetAngleBy

        ECMD_FireProjectile(projType, currentAngle)
    End Sub

    Protected Sub ECMD_FireProjectileFoward(ByVal projType As Type)
        ECMD_FireProjectileFoward(projType, 0)
    End Sub

    Protected Sub ECMD_FaceTarget()
        If Me.Target IsNot Nothing Then
            'We need this only to get the correct direction
            'we aren't actually going to use it for movement
            Dim mv As MovementTrajectory = MovementTrajectory.CreateTrajectory(Me.PositionFromCenter, g_objTarget.PositionFromCenter, 1)

            'MyBase.SetDirection(mv.Direction)
            SetFacingDirection(mv)
        End If
    End Sub

    Protected Sub ECMD_MoveFoward(ByVal strafeAngle As Double)
        ECMD_MoveFoward(g_vecCurrentMovement.Speed, strafeAngle)
    End Sub

    Protected Sub ECMD_MoveFoward(ByVal speed As Double, ByVal strafeAngle As Double)
        'We use the facing angle not the movement angle
        Dim currentFacingTraj As PointF = g_vecFacingDirection.AsPointF
        Dim currentAngle As Double = Geometry.GetAngleInDegrees(currentFacingTraj)

        currentAngle -= strafeAngle

        Dim newTraj As PointF = Geometry.GetTrajectoryFromAngle(currentAngle)

        Dim mv As New MovementTrajectory(newTraj.X, newTraj.Y)
        mv.Speed = speed

        g_vecCurrentMovement = mv
    End Sub

    Protected Sub ECMD_DamageTarget(ByVal damage As Integer)
        If Me.Target IsNot Nothing Then
            Dim cast = TryCast(Me.Target, IDamagable)

            If cast IsNot Nothing Then
                cast.DeductHitPoints(damage)
            End If
        End If
    End Sub

    'Will only select a target that it is actually touching
    'the creature
    Protected Sub ECMD_SelectMeleeTarget()
        Dim target = g_lstViableTargets.FirstOrDefault(Function(ent) ent.Bounds.IntersectsWith(Me.Bounds))

        If target IsNot Nothing AndAlso TypeOf target Is IProjectileTarget Then

            'Don't aquire melee targets that projectiles can't hit
            If Not DirectCast(target, IProjectileTarget).AllowProjectileHit Then
                target = Nothing
            End If
        End If

        g_objTarget = target
    End Sub

    Protected Sub ECMD_JumpIfTouchingPotentialTarget(ByVal state As String)

        If MyBase.HasState(state) Then
            Dim target = g_lstViableTargets.FirstOrDefault(Function(ent) ent.Bounds.IntersectsWith(Me.Bounds))

            If target IsNot Nothing Then
                ECMD_Goto(state)
            End If
        End If
    End Sub

    Protected Sub ECMD_FaceAndApproachTarget(ByVal speed As Double)
        If Me.Target IsNot Nothing Then
            g_vecCurrentMovement.SetTrajectory(Me.Position, Me.Target.Position, speed)


            SetFacingDirection(g_vecCurrentMovement.AsPointF)
        End If
    End Sub

    Protected Sub ECMD_FaceAndApproachTarget()
        ECMD_FaceAndApproachTarget(g_vecCurrentMovement.Speed)
    End Sub

    Protected Sub ECMD_ChangeTrajectoryRandom(ByVal speed As Double)

        Dim v As MovementTrajectory = CreateRandomTrajectory()

        v.Speed = speed
        SetMovement(v)
    End Sub

    Protected Sub ECMD_ChangeTrajectoryRandom()
        ECMD_ChangeTrajectoryRandom(_rnd.Next(1, Me.TopSpeed + 1))
    End Sub

    Protected Sub ECMD_Roam()
        ECMD_Roam(Me.AgilityPercent)
    End Sub

    Protected Sub ECMD_Roam(ByVal speed As Double)
        If GeneralHelpers.RollDice(Me.AgilityPercent) Then
            ECMD_ChangeTrajectoryRandom(speed)
        End If
    End Sub

    Protected Sub ECMD_Roam(ByVal changeDirChance As Integer)
        If GeneralHelpers.RollDice(changeDirChance) Then
            ECMD_ChangeTrajectoryRandom()
        End If
    End Sub

    Protected Sub ECMD_Jump(ByVal label As String, ByVal chance As Integer)
        If GeneralHelpers.RollDice(chance) Then
            ECMD_Goto(label)
        End If
    End Sub

    Protected Overrides Sub OnTick()
        If g_bMoveBlur Then
            DepositBlur()
        End If

        g_ptCurPos.X += g_vecCurrentMovement.X
        g_ptCurPos.Y += g_vecCurrentMovement.Y

        g_vecCurrentMovement.Speed -= g_dblDeceleration

        If g_vecCurrentMovement.Speed = 0 AndAlso g_dblDeceleration <> 0 Then
            'No more deceleration
            g_dblDeceleration = 0
        End If


        With Me.Bounds
            Dim changed As Boolean = False

            If .Left < 0 AndAlso (g_vecCurrentMovement.HorizontalDirection = HorizontalDirection.Left OrElse g_vecCurrentMovement.HorizontalDirection = HorizontalDirection.None) Then

                g_vecCurrentMovement.SetTrajectoryMaintainSpeed(GetTrajectoryRandomAngle(GeneralDirection.Right))
                changed = True
            End If

            If .Top < 0 AndAlso (g_vecCurrentMovement.VerticalDirection = VerticalDirection.Up OrElse g_vecCurrentMovement.VerticalDirection = VerticalDirection.None) Then

                g_vecCurrentMovement.SetTrajectoryMaintainSpeed(GetTrajectoryRandomAngle(GeneralDirection.Down))
                changed = True
            End If


            If Me.Arena IsNot Nothing Then
                If .Right > Me.Arena.ArenaSize.Width AndAlso (g_vecCurrentMovement.HorizontalDirection = HorizontalDirection.Right OrElse g_vecCurrentMovement.HorizontalDirection = HorizontalDirection.None) Then

                    g_vecCurrentMovement.SetTrajectoryMaintainSpeed(GetTrajectoryRandomAngle(GeneralDirection.Left))
                    changed = True
                End If

                If .Bottom > Me.Arena.ArenaSize.Height AndAlso (g_vecCurrentMovement.VerticalDirection = VerticalDirection.Down OrElse g_vecCurrentMovement.VerticalDirection = VerticalDirection.None) Then

                    g_vecCurrentMovement.SetTrajectoryMaintainSpeed(GetTrajectoryRandomAngle(GeneralDirection.Up))
                    changed = True
                End If

            End If

            If changed Then
                SetFacingDirection(g_vecCurrentMovement.AsPointF)

                If g_bCheckArenaEdge AndAlso MyBase.HasState(StandardStateLabels.HitArenaEdge.LabelName) Then
                    If Me.CurrentHitPoints > 0 Then
                        ECMD_Goto(StandardStateLabels.HitArenaEdge.LabelName)
                    End If
                End If
            End If
        End With


        If Me.CurrentHitPoints < 1 Then
            If Not MyBase.CurrentStateIs(StandardStateLabels.Dying.LabelName) Then
                ECMD_Goto(StandardStateLabels.Dying.LabelName)
            End If
        End If

    End Sub

    Private ReadOnly Property Offsets As Point
        Get
            If Me.CurrentRI IsNot Nothing Then
                Return Me.CurrentRI.Sprite.Offsets
            Else
                Return New Point(0, 0)
            End If
        End Get
    End Property

    Private ReadOnly Property Target() As IArenaEntity
        Get
            If Me.Arena IsNot Nothing AndAlso g_objTarget IsNot Nothing Then
                If Me.Arena.Entities.Contains(g_objTarget) Then
                    Return g_objTarget
                End If
            End If
            Return Nothing
        End Get
    End Property

    Private Sub SetFacingDirection(ByVal mv As MovementTrajectory)
        MyBase.SetDirection(mv.Direction)

        g_vecFacingDirection = mv
    End Sub

    Private Sub SetFacingDirection(ByVal trajectory As PointF)
        Dim mv As New MovementTrajectory(trajectory.X, trajectory.Y)

        SetFacingDirection(mv)
    End Sub

    Private Function CreateRandomTrajectory() As MovementTrajectory
        Return New MovementTrajectory(_rnd.Next(-100, 101), _rnd.Next(-100, 101))
    End Function

    Private Sub SetMovement(ByVal mv As MovementTrajectory)
        g_vecCurrentMovement = mv

        SetFacingDirection(mv)
        'MyBase.SetDirection(mv.Direction)
    End Sub

    Private Function GetTrajectoryRandomAngle(ByVal direction As GeneralDirection) As PointF

        Dim halfAngle As Double = -80
        Dim change As Double = _rnd.Next(halfAngle, Math.Abs(halfAngle) + 1)
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

    'Private Sub UpdateViableTargets()
    '    Dim targets As List(Of IArenaEntity) = Nothing

    '    If Me.Arena IsNot Nothing Then

    '        Select Case Me.TargetSelection
    '            Case TargetSelectionType.AnyCreature
    '                targets = Me.Arena.Entities.Where(
    '                    Function(ent) Not ent Is Me AndAlso TypeOf ent Is IDemon).ToList

    '            Case TargetSelectionType.DifferentSpecies
    '                targets = Me.Arena.Entities.Where(
    '                    Function(ent) TypeOf ent Is IDemon AndAlso (Me.GetType <> ent.GetType AndAlso (Me.Species = "" OrElse Not Me.Species.Equals(DirectCast(ent, IDemon).Species, StringComparison.CurrentCultureIgnoreCase)))).ToList

    '        End Select

    '        'If creatures IsNot Nothing AndAlso creatures.Count > 0 Then
    '        ' g_objTarget = creatures.ElementAt(_rnd.Next(0, creatures.Count))
    '        'End If
    '    End If

    '    g_lstViableTargets = targets
    'End Sub

    Private Sub RemoveViableTarget(ByVal potentialTarget As IArenaEntity)
        g_lstViableTargets.Remove(potentialTarget)
        'Dim i As Integer = g_lstViableTargets.IndexOf(potentialTarget)

        'If i <> -1 Then
        '    g_lstViableTargets(i) = Nothing
        'End If
    End Sub

    Private Sub UpdateViableTargets(ByVal potentialTarget As IArenaEntity)
        Select Case Me.TargetSelection
            Case TargetSelectionType.AnyCreature
                If Not potentialTarget Is Me AndAlso TypeOf potentialTarget Is IDemon Then
                    g_lstViableTargets.Add(potentialTarget)
                End If

            Case TargetSelectionType.DifferentSpecies
                If TypeOf potentialTarget Is IDemon AndAlso (Me.GetType <> potentialTarget.GetType AndAlso (Me.Species = "" OrElse Not Me.Species.Equals(DirectCast(potentialTarget, IDemon).Species, StringComparison.CurrentCultureIgnoreCase))) Then
                    g_lstViableTargets.Add(potentialTarget)
                End If

        End Select
    End Sub

    Private Sub DepositBlur()
        If Me.Arena IsNot Nothing Then
            Static tickCount As Integer = 0

            If tickCount >= 5 Then
                Dim blur As New MoveBlur

                blur.Image = Me.CurrentRI.Sprite
                blur.BlurSize = Me.Size
                blur.SetPosition(Me.Position)

                Me.Arena.AddEntity(blur)

                tickCount = 0
            End If
            tickCount += 1

        End If
    End Sub

    Private Sub _arena_EntityListChanged(sender As Object, e As EntityListChangedEventArgs) Handles _arena.EntityListChanged
        If e.ChangeType = EntityListChangeType.EntityAdded Then
            UpdateViableTargets(e.Entity)
        End If

        If e.ChangeType = EntityListChangeType.EntityRemoved Then
            RemoveViableTarget(e.Entity)
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
