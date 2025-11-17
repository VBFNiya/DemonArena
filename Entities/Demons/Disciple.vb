Public Class Disciple
    Inherits ComplexCreature
    Public Sub New()
        MyBase.New(DemonSprites.Disciple)
    End Sub

    Public Overrides ReadOnly Property Species As String
        Get
            Return "Wizard"
        End Get
    End Property

    Protected Overrides ReadOnly Property Scale As Double
        Get
            Return 0.6
        End Get
    End Property

    Protected Overrides ReadOnly Property AgilityPercent As Integer
        Get
            Return 20
        End Get
    End Property

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(StandardStateLabels.Start)
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Frames("DISC", "AB", 10, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("ChooseAtk", 60)))

            lst.Add(New Expr_Label("ChooseAtk"))
            lst.Add(New Expr_Exec(Sub() ECMD_Jump("Atk1", 80)))
            lst.Add(New Expr_Exec(Sub() ECMD_Goto("PrepAtk2")))

            lst.Add(New Expr_Label("Atk1"))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("DISC", "E", 15, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("DISC", "F", 15, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjAtTarget(GetType(DiscipleProjectile1))))
            lst.Add(New Expr_Frames("DISC", "F", 3, Sub() ECMD_FireProjAtTarget(GetType(DiscipleProjectile1))))
            lst.Add(New Expr_Frames("DISC", "F", 3, Sub() ECMD_FireProjAtTarget(GetType(DiscipleProjectile1))))
            lst.Add(New Expr_Goto(StandardStateLabels.Start))

            lst.Add(New Expr_Label("PrepAtk2"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, True)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, True)))
            lst.Add(New Expr_Frames("DISC", "FE", 15))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(10)))
            lst.Add(New Expr_Goto("Attack2"))

            lst.Add(New Expr_Label("Attack2"))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("DISC", "E", 20, Sub() ECMD_Roam(10D)))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjAtTarget(GetType(DiscipleProjectile1))))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget("LeaveAtk2")))
            lst.Add(New Expr_LoopRandom(3, 20))
            lst.Add(New Expr_Goto("LeaveAtk2"))

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Goto("DisableGhost"))
            lst.Add(New Expr_Frames("DISC", "G", 1, Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving(StandardStateLabels.Start.LabelName)))

            lst.Add(New Expr_Label("LeaveAtk2"))
            lst.Add(New Expr_Goto("DisableGhost"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(1)))
            lst.Add(New Expr_Goto(StandardStateLabels.Start))

            lst.Add(New Expr_Label("DisableGhost"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, False)))
            lst.Add(New Expr_Return)


            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("DISC", "HIJKLMNO", 4, Sub() ECMD_RemoveMe()))


            Return lst
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 150
        End Get
    End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 1
        End Get
    End Property
End Class

Public Class DiscipleProjectile1
    Inherits Projectile

    Public Sub New(ByVal owner As Creature, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.DiscipleProjectile1, owner, source, target)
    End Sub

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.DiscipleProjectile1, owner, position)
    End Sub

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 15
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "C:5,D:5,E:5,F:5,G:5,H:5,I:5,J:5,K:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Red
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:10,B:10"
        End Get
    End Property

    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(15, 15)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 8
        End Get
    End Property
End Class