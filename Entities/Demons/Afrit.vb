
Public Class GoldenAfrit
    Inherits ComplexCreature

    Public Sub New()
        MyBase.New(DemonSprites.GoldAfrit)
    End Sub

    Public Overrides ReadOnly Property Species As String
        Get
            Return "Afrit"
        End Get
    End Property

    Protected Overrides ReadOnly Property AgilityPercent As Integer
        Get
            Return 40
        End Get
    End Property

    Protected Overrides ReadOnly Property FlinchChance As Integer
        Get
            Return 60
        End Get
    End Property

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(StandardStateLabels.Start)
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Frames("fr07", "ABCD", 10, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("PrepAttack", 30)))


            lst.Add(New Expr_Label("PrepAttack"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(Me.TopSpeed * 2)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, True)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, True)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetDeceleration(0.3)))
            lst.Add(New Expr_Goto("SlowB4Atk"))

            lst.Add(New Expr_Label("SlowB4Atk"))
            lst.Add(New Expr_Frames("fr07", "E", 1, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving("BeginAttack")))

            lst.Add(New Expr_Label("BeginAttack"))
            lst.Add(New Expr_Frames("fr07", "F", 10, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("fr07", "G", 10, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(AfritProjectileExplosive40))))
            lst.Add(New Expr_Exec(Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget("AttackDone")))
            lst.Add(New Expr_Exec(Sub() ECMD_Jump("PrepAttack", 60)))
            lst.Add(New Expr_Goto("AttackDone"))

            lst.Add(New Expr_Label("AttackDone"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, False)))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))

            lst.Add(StandardStateLabels.Flinch)
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(0.1)))
            lst.Add(New Expr_Frames("fr07", "H", 10, Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Frames("fr07", "H", 1, Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving(StandardStateLabels.Start.LabelName)))


            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("fr07", "IJKLMNOPQR", 15))
            lst.Add(New Expr_Exec(Sub() ECMD_RemoveMe()))

            Return lst
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 200
        End Get
    End Property

    'Public Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(70, 70)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 3
        End Get
    End Property
End Class

Public Class BlueAfrit
    Inherits GoldenAfrit

    Public Sub New()
        MyBase.Sprites = DemonSprites.BlueAfrit
    End Sub

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(StandardStateLabels.Start)
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Frames("fr06", "ABCD", 10, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("ChooseAtk", 40)))

            lst.Add(New Expr_Label("ChooseAtk"))
            lst.Add(New Expr_Exec(Sub() ECMD_Jump("PrepAttack1", 20)))
            lst.Add(New Expr_Goto("Attack2"))
            lst.Add(New Expr_Return)

            lst.Add(New Expr_Label("Attack2"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(0.5)))
            lst.Add(New Expr_Frames("fr06", "S", 15, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("fr06", "T", 15, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("fr06", "U", 15, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(AfritProjectileWeak), 0)))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(AfritProjectileWeak), 5)))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(AfritProjectileWeak), -5)))
            lst.Add(New Expr_Exec(Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Return)

            lst.Add(New Expr_Label("PrepAttack1"))
            lst.Add(New Expr_Goto("PrepDash"))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(Me.TopSpeed * 2)))
            lst.Add(New Expr_Frames("fr06", "ABCD", 2))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfCloseToTarget(200, "Attack1")))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget("UndoDash")))

            lst.Add(New Expr_Label("Attack1"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(Me.TopSpeed \ 2)))
            lst.Add(New Expr_Frames("fr06", "E", 10, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("fr06", "F", 10, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("fr06", "G", 10, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(AfritProjectileExplosive100), 0)))
            lst.Add(New Expr_Exec(Sub() ECMD_SelectNewTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_CreateTimer(600, "Stop Attack1")))
            lst.Add(New Expr_Goto("PrepAttack1"))

            lst.Add(New Expr_Label("Stop Attack1"))
            lst.Add(New Expr_Goto("UndoDash"))

            lst.Add(New Expr_Label("UndoDash"))
            lst.Add(New Expr_Exec(Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_Goto(StandardStateLabels.Start.LabelName)))

            lst.Add(New Expr_Label("PrepDash"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, True)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, True)))
            lst.Add(New Expr_Return)

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, False)))
            lst.Add(New Expr_Frames("fr06", "H", 1, Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving(StandardStateLabels.Start.LabelName)))


            lst.Add(StandardStateLabels.Flinch)
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(0.1)))
            lst.Add(New Expr_Frames("fr06", "H", 10, Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))


            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("fr06", "IJKLMNOPQR", 15))
            lst.Add(New Expr_Exec(Sub() ECMD_RemoveMe()))

            Return lst
        End Get
    End Property


End Class

Public Class PurpleAfrit
    Inherits GoldenAfrit

    Public Sub New()
        MyBase.Sprites = DemonSprites.PurpleAfrit
    End Sub

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(StandardStateLabels.Start)
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Frames("fr02", "ABCD", 10, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("ChooseAtk", 40)))

            lst.Add(New Expr_Label("ChooseAtk"))
            lst.Add(New Expr_Exec(Sub() ECMD_Jump("PrepAttack1", 20)))
            lst.Add(New Expr_Goto("Attack2"))
            lst.Add(New Expr_Return)


            lst.Add(New Expr_Label("Attack2"))
            lst.Add(New Expr_Frames("fr02", "S", 10, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("fr02", "T", 10, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("fr02", "U", 5, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("fr02", "U", 5, Sub() ECMD_FireProjectileFoward(GetType(AfritProjectileWeak))))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(AfritProjectileWeak), 5)))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(AfritProjectileWeak), -5)))

            lst.Add(New Expr_Return)

            lst.Add(New Expr_Label("PrepAttack1"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, True)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, True)))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceAndApproachTarget(Me.TopSpeed * 2)))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfCloseToTarget(150, "Attack1")))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget("UndoDash")))
            lst.Add(New Expr_Frames("fr02", "A", 1))

            lst.Add(New Expr_Label("Attack1"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(Me.TopSpeed)))
            lst.Add(New Expr_Frames("fr02", "STU", 5, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(AfritProjectileWeak))))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_MoveFoward(90)))
            lst.Add(New Expr_Exec(Sub() ECMD_Jump("-90", 50)))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfFarFromTarget(150, "PrepAttack1")))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget("ChooseNewTargetForAtk1")))
            lst.Add(New Expr_Exec(Sub() ECMD_CreateTimer(800, "Stop Attack1")))

            lst.Add(New Expr_Label("Stop Attack1"))
            lst.Add(New Expr_Exec(Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Goto("UndoDash"))

            lst.Add(New Expr_Label("ChooseNewTargetForAtk1"))
            lst.Add(New Expr_Exec(Sub() ECMD_SelectNewTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget("UndoDash")))
            lst.Add(New Expr_Goto("PrepAttack1"))

            lst.Add(New Expr_Label("-90"))
            lst.Add(New Expr_Exec(Sub() ECMD_MoveFoward(-90)))
            lst.Add(New Expr_Return)

            lst.Add(New Expr_Label("UndoDash"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_Goto(StandardStateLabels.Start.LabelName)))

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, False)))
            lst.Add(New Expr_Frames("fr02", "H", 1, Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving(StandardStateLabels.Start.LabelName)))

            lst.Add(StandardStateLabels.Flinch)
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(0.1)))
            lst.Add(New Expr_Frames("fr02", "H", 10, Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))


            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("fr02", "IJKLMNOPQR", 15))
            lst.Add(New Expr_Exec(Sub() ECMD_RemoveMe()))


            Return lst
        End Get
    End Property


End Class

Public Class AfritProjectileExplosive100
    Inherits Projectile

    Public Sub New(ByVal owner As IArenaEntity, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.CometProjectile, owner, source, target)
    End Sub

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.CometProjectile, owner, position)
    End Sub

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 100
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "E:5,F:5,G:5,H:5,I:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Goldenrod
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:5,B:5,C:5,D:5"
        End Get
    End Property
    Protected Overrides ReadOnly Property IsExplosive As Boolean
        Get
            Return True
        End Get
    End Property

    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(20, 20)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 5
        End Get
    End Property
End Class

Public Class AfritProjectileExplosive40
    Inherits AfritProjectileExplosive100

    Public Sub New(ByVal owner As IArenaEntity, ByVal source As Point, ByVal target As Point)
        MyBase.New(owner, source, target)
    End Sub

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(owner, position)
    End Sub


    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 40
        End Get
    End Property
End Class

Public Class AfritProjectileWeak
    Inherits Projectile

    Public Sub New(ByVal owner As IArenaEntity, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.AfritProjectileWeak, owner, source, target)
    End Sub

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.AfritProjectileWeak, owner, position)
    End Sub


    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            'TODO: There are more death frames
            Return "C:12,D:12,E:12,F:12,G:12,H:12,I:12,J:12,K:12,L:12"
        End Get
    End Property

    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(20, 15)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 7
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:10,B:10"
        End Get
    End Property

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 5
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Yellow
        End Get
    End Property
End Class

'Public Class GoldenAfritSimple
'    Inherits AfritBase

'    Private _attacks As CreatureAttack()

'    Public Sub New()
'        MyBase.New(DemonSprites.GoldAfrit)

'        Dim lst As New List(Of CreatureAttack)
'        Dim atk1 As New CreatureAttack("E:25,F:25,G:5", GetType(AfritProjectileExplosive40))

'        atk1.SlowSpeedToAttack = True
'        lst.Add(atk1)

'        _attacks = lst.ToArray
'    End Sub

'    Protected Overrides ReadOnly Property Passivity As Integer
'        Get
'            Return 150
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
'        Get
'            Return _attacks
'        End Get
'    End Property
'End Class

'Public Class PurpleAfritSimple
'    Inherits AfritBase

'    Private _attacks As CreatureAttack()

'    Public Sub New()
'        MyBase.New(DemonSprites.PurpleAfrit)

'        Dim lst As New List(Of CreatureAttack)
'        Dim atk2 As New CreatureAttack("S:7,T:7,U:7", GetType(AfritProjectileWeak))

'        atk2.SlowSpeedToAttack = False
'        atk2.CanRoamAndAttack = True

'        atk2.Vollies = 10

'        lst.Add(atk2)

'        _attacks = lst.ToArray
'    End Sub

'    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
'        Get
'            Return _attacks
'        End Get
'    End Property

'End Class

'Public MustInherit Class AfritBase
'    Inherits Creature

'    Protected Sub New(ByVal sprites As SpriteCollection)
'        MyBase.New(sprites)
'    End Sub

'    Protected Overrides ReadOnly Property DeathAnimationSequence As String
'        Get
'            Return "I:10,J:10,K:10,L:10,M:10,N:10,O:10,P:10,Q:10,R:10"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MaxHitPoints As Integer
'        Get
'            Return 200
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MovementAnimationSequence As String
'        Get
'            Return "A:10,B:10,C:10,D:10"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Passivity As Integer
'        Get
'            Return 100
'        End Get
'    End Property

'    Public Overrides ReadOnly Property Size As System.Drawing.Size
'        Get
'            Return New Size(70, 70)
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Defense As CreatureDefense
'        Get
'            Return New DashDefense(New Integer() {500, 50, 100, 150}, New Integer() {50, 25, 25, 1000, 600, 600, 600}, Me.TopSpeed * 2)
'        End Get
'    End Property

'    'Protected Overrides ReadOnly Property Alertness As Integer
'    '    Get
'    '        Return 1
'    '    End Get
'    'End Property

'    Protected Overrides ReadOnly Property TopSpeed As Double
'        Get
'            Return 3
'        End Get
'    End Property
'End Class

'Public Class BlueAfritSimple
'    Inherits AfritBase

'    Private _attacks As CreatureAttack()

'    Public Sub New()
'        MyBase.New(DemonSprites.BlueAfrit)

'        Dim lst As New List(Of CreatureAttack)
'        Dim atk1 As New CreatureAttack("E:25,F:25,G:5", GetType(AfritProjectileExplosive100))
'        Dim atk2 As New CreatureAttack("S:15,T:15,U:15", GetType(AfritProjectileWeak))

'        atk1.SlowSpeedToAttack = True
'        atk2.SlowSpeedToAttack = True


'        atk2.Vollies = 3

'        atk1.ChanceRatio = 1
'        atk2.ChanceRatio = 3

'        lst.Add(atk1)
'        lst.Add(atk2)
'        _attacks = lst.ToArray
'    End Sub

'    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
'        Get
'            Return _attacks
'        End Get
'    End Property

'End Class