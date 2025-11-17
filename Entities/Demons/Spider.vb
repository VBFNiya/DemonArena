Public Class BlueSpider
    Inherits Creature

    Private _attacks As CreatureAttack()

    Public Sub New()
        MyBase.New(DemonSprites.BlueSpider)

        Dim lst As New List(Of CreatureAttack)
        Dim atk1 As New CreatureAttack("C:3,D:3", GetType(SpiderProjectileWeak), 12)
        Dim atk2 As New CreatureAttack("C:9,D:9", GetType(SpiderProjectileStrong), 5)

        atk1.SlowSpeedToAttack = True
        atk1.ChanceRatio = 5

        atk2.SlowSpeedToAttack = True

        lst.Add(atk1)
        lst.Add(atk2)
        _attacks = lst.ToArray
    End Sub

    Public Overrides ReadOnly Property Species As String
        Get
            Return "Spider"
        End Get
    End Property

    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
        Get
            Return _attacks
        End Get
    End Property

    Protected Overrides ReadOnly Property DeathAnimationSequence As String
        Get
            Return "E:20,F:20,G:20,H:20,I:20,J:20"
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 300
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationSequence As String
        Get
            Return "A:5,B:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property Passivity As Integer
        Get
            Return 100
        End Get
    End Property

    'Public Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(70, 70)
    '    End Get
    'End Property

    'Protected Overrides ReadOnly Property Alertness As Integer
    '    Get
    '        Return 100
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 4
        End Get
    End Property
End Class

Public Class YellowSpider
    Inherits ComplexCreature
    Public Sub New()
        MyBase.new(DemonSprites.YellowSpider)
    End Sub

    Public Overrides ReadOnly Property Species As String
        Get
            Return "Spider"
        End Get
    End Property

    Protected Overrides ReadOnly Property AgilityPercent As Integer
        Get
            Return 20
        End Get
    End Property

    Protected Overrides ReadOnly Property FlinchChance As Integer
        Get
            Return 10
        End Get
    End Property

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(StandardStateLabels.Start)
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Frames("AR04", "AB", 5, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("Attack", 10)))

            lst.Add(New Expr_Label("Attack"))
            lst.Add(New Expr_Exec(Sub() ECMD_Roam(1D)))
            lst.Add(New Expr_Exec(Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Frames("AR04", "C", 3, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("AR04", "D", 3, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(GreenPlasmaBall))))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget(StandardStateLabels.Start.LabelName)))

            lst.Add(StandardStateLabels.Flinch)
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(0.1)))
            lst.Add(New Expr_Frames("AR04", "E", 3))
            lst.Add(New Expr_Exec(Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Frames("AR04", "e", 1))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving("thrustend")))

            lst.Add(New Expr_Label("ThrustEnd"))
            lst.Add(New Expr_Exec(Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))

            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("AR04", "EFGHIJ", 20, Sub() ECMD_RemoveMe()))

            Return lst
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 300
        End Get
    End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 4
        End Get
    End Property
End Class

Public Class GreenPlasmaBall
    Inherits Projectile

    Public Sub New(ByVal owner As IArenaEntity, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.GreenPlasmaBall, owner, source, target)
    End Sub

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.GreenPlasmaBall, owner, position)
    End Sub

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 2
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "C:5,D:5,E:5,F:5,G:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Lime
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:5,B:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 12
        End Get
    End Property
End Class

Public Class SpiderProjectileWeak
    Inherits Projectile

    Public Sub New(ByVal owner As Creature, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.SpiderProjectileWeak, owner, source, target)
    End Sub

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 1
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "C:10,D:10,E:10"
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:10,B:10"
        End Get
    End Property

    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(10, 10)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 6
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Green
        End Get
    End Property
End Class

Public Class SpiderProjectileStrong
    Inherits Projectile

    Public Sub New(ByVal owner As Creature, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.SpiderProjectileStrong, owner, source, target)
    End Sub

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 20
        End Get
    End Property

    Protected Overrides ReadOnly Property IsExplosive As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "C:10,D:10,E:10"
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:10,B:10"
        End Get
    End Property

    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(13, 13)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 3
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Blue
        End Get
    End Property
End Class