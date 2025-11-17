
Public Class DarkCardinal
    Inherits ComplexCreature

    Public Sub New()
        MyBase.New(DemonSprites.DarkCardinal)
    End Sub

    Protected Overrides ReadOnly Property AgilityPercent As Integer
        Get
            Return 20
        End Get
    End Property

    Protected Overrides ReadOnly Property FlinchChance As Integer
        Get
            Return 30
        End Get
    End Property

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(New Expr_Label("start"))
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Frames("card", "ab", 12, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("atk", 10)))

            lst.Add(New Expr_Label("atk"))
            lst.Add(New Expr_Frames("card", "c", 20, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("card", "d", 20, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(CardinalRocket))))
            lst.Add(New Expr_LoopFixed(3))
            lst.Add(New Expr_Return)
            'lst.Add(New Expr_Goto("start"))

            lst.Add(StandardStateLabels.Flinch)
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(0.1)))
            lst.Add(New Expr_Frames("card", "e", 10))
            lst.Add(New Expr_Exec(Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))

            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("card", "efghijkl", 10))
            lst.Add(New Expr_Exec(Sub() ECMD_RemoveMe()))

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Frames("card", "e", 2))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving("thrustend")))

            lst.Add(New Expr_Label("ThrustEnd"))
            lst.Add(New Expr_Exec(Sub() ECMD_ChangeTrajectoryRandom()))
            lst.Add(New Expr_Goto("start"))

            Return lst.ToArray
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 200
        End Get
    End Property

    'Public Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(60, 60)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 2
        End Get
    End Property
End Class

Public Class CardinalRocket
    Inherits Projectile

    Public Sub New(ByVal owner As IArenaEntity, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.Rocket, owner, source, target)
    End Sub
    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.Rocket, owner, position)
    End Sub

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 250
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "C:10,D:10,E:10"
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

    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(20, 20)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 7
        End Get
    End Property

    Protected Overrides ReadOnly Property IsExplosive As Boolean
        Get
            Return True
        End Get
    End Property

End Class

'Public Class DarkCardinalSimple
'    Inherits Creature

'    Private _attacks As CreatureAttack()

'    Public Sub New()
'        MyBase.New(DemonSprites.DarkCardinal)

'        Dim atk1 As New CreatureAttack("C:20,D:20", GetType(CardinalRocket))

'        atk1.SlowSpeedToAttack = True
'        atk1.Vollies = 3

'        _attacks = New CreatureAttack() {atk1}

'    End Sub

'    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
'        Get
'            Return _attacks
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property DeathAnimationSequence As String
'        Get
'            Return "E:10,F:10,G:10,H:10,I:10,J:10,K:10,L:10"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MaxHitPoints As Integer
'        Get
'            Return 200
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MovementAnimationSequence As String
'        Get
'            Return "A:12,B:12"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Passivity As Integer
'        Get
'            Return 100
'        End Get
'    End Property

'    Public Overrides ReadOnly Property Size As System.Drawing.Size
'        Get
'            Return New Size(60, 60)
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property TopSpeed As Double
'        Get
'            Return 2
'        End Get
'    End Property
'End Class