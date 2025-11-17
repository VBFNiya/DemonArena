Public Class LostSoul
    Inherits ComplexCreature

    Public Sub New()
        MyBase.New(DemonSprites.LostSoul)
    End Sub

    Protected Overrides ReadOnly Property AgilityPercent As Integer
        Get
            Return 5
        End Get
    End Property

    Protected Overrides ReadOnly Property Scale As Double
        Get
            Return 0.6
        End Get
    End Property

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(StandardStateLabels.Start)
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Frames("SKUL", "AB", 15, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("PrepCharge", 40)))

            lst.Add(New Expr_Label("PrepCharge"))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(8)))
            lst.Add(New Expr_Exec(Sub() ECMD_SelectMeleeTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.ArenaEdgeAlertOn, True)))
            lst.Add(New Expr_Goto("ChargeAttack"))

            lst.Add(New Expr_Label("ChargeAttack"))
            lst.Add(New Expr_Frames("SKUL", "C", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))
            lst.Add(New Expr_Frames("SKUL", "C", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))
            lst.Add(New Expr_Frames("SKUL", "C", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))
            lst.Add(New Expr_Frames("SKUL", "C", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))
            lst.Add(New Expr_Frames("SKUL", "C", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))
            lst.Add(New Expr_Frames("SKUL", "D", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))
            lst.Add(New Expr_Frames("SKUL", "D", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))
            lst.Add(New Expr_Frames("SKUL", "D", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))
            lst.Add(New Expr_Frames("SKUL", "D", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))
            lst.Add(New Expr_Frames("SKUL", "D", 1, Sub() ECMD_TryAcquireMeleeTargetAndJump("DealDamage")))

            lst.Add(New Expr_Label("DealDamage"))
            lst.Add(New Expr_Exec(Sub() ECMD_ThrustTarget(150, 12)))
            lst.Add(New Expr_Exec(Sub() ECMD_DamageTarget(15)))
            lst.Add(New Expr_Exec(Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Exec(Sub() ECMD_ClearTarget()))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))

            lst.Add(StandardStateLabels.HitArenaEdge)
            lst.Add(New Expr_Exec(Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.ArenaEdgeAlertOn, False)))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Frames("SKUL", "E", 1))
            lst.Add(New Expr_Exec(Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving(StandardStateLabels.Start.LabelName)))

            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("SKUL", "FGHIJK", 10, Sub() ECMD_RemoveMe()))

            Return lst
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 80
        End Get
    End Property

    'Public Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(25, 40)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 1
        End Get
    End Property
End Class
