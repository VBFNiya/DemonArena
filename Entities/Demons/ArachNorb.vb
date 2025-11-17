Public Class ArachNorb
    Inherits ComplexCreature

    Public Sub New()
        MyBase.New(DemonSprites.ArachNorb)
    End Sub

    Protected Overrides ReadOnly Property AgilityPercent As Integer
        Get
            Return 10
        End Get
    End Property

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(StandardStateLabels.Start)
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Exec(Sub() ECMD_Roam()))
            lst.Add(New Expr_Frames("ACNB", "AB", 5))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("PrepAtk", 10)))


            lst.Add(New Expr_Label("PrepAtk"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(1)))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("ACNB", "CB", 7, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("ACNB", "CB", 5, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("ACNB", "CB", 2, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("ACNB", "CB", 2, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("ACNB", "C", 3, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("ACNB", "C", 3, Sub() ECMD_FaceAndApproachTarget()))
            'lst.Add(New Expr_Exec(Sub() ECMD_CreateTimerRandomInterval(3, 50, "StopAtk")))
            lst.Add(New Expr_Goto("Atk"))

            lst.Add(New Expr_Label("Atk"))
            lst.Add(New Expr_Frames("ACNB", "C", 3, Sub() ECMD_FireProjectileFoward(GetType(ArachNorbProj))))
            lst.Add(New Expr_LoopRandom(1, 5))
            lst.Add(New Expr_Goto(StandardStateLabels.Start))

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Frames("ACNF", "I", 1))
            lst.Add(New Expr_Exec(Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving(StandardStateLabels.Start.LabelName)))

            lst.Add(New Expr_Label("StopAtk"))
            lst.Add(New Expr_Goto(StandardStateLabels.Start))

            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("ACNB", "DEFGH", 10, Sub() ECMD_RemoveMe()))

            Return lst
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 250
        End Get
    End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 5
        End Get
    End Property
End Class

Public Class ArachNorbProj
    Inherits Projectile

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.ArachNorbProjectile, owner, position)
    End Sub

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 13
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "C:5,D:5,E:5,F:5,G:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Yellow
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:3,B:3"
        End Get
    End Property
    Protected Overrides ReadOnly Property IsExplosive As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 8
        End Get
    End Property
End Class
