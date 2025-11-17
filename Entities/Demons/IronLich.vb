

Public Class IronLich
    Inherits ComplexCreature

    Public Sub New()
        MyBase.New(DemonSprites.IronLich)
    End Sub

    Protected Overrides ReadOnly Property Scale As Double
        Get

            Return 1
        End Get
    End Property

    Protected Overrides ReadOnly Property AgilityPercent As Integer
        Get
            Return 5
        End Get
    End Property

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(StandardStateLabels.Start)
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Frames("IRL3", "A", 3, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("ChooseAtk", 3)))


            lst.Add(New Expr_Label("ChooseAtk"))
            lst.Add(New Expr_Exec(Sub() ECMD_Jump("Atk1", 90)))
            lst.Add(New Expr_Goto("PrepAtk2"))

            lst.Add(New Expr_Label("Atk1"))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("IRL3", "B", 25))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(IronLichProjectile))))
            lst.Add(New Expr_Goto(StandardStateLabels.Start))


            lst.Add(New Expr_Label("PrepAtk2"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetDeceleration(0.01)))
            lst.Add(New Expr_Frames("IRL3", "A", 1))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfSpeedLessThan(0.2, "Atk2")))

            lst.Add(New Expr_Label("Atk2"))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("IRL3", "AB", 15, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("IRL3", "AB", 15, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("IRL3", "AB", 15, Sub() ECMD_FaceTarget()))

            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(IronLichProjectile))))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(IronLichProjectile), 3)))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(IronLichProjectile), -3)))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(IronLichProjectile), 5)))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(IronLichProjectile), -5)))

            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(1)))
            lst.Add(New Expr_Goto(StandardStateLabels.Start))

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Frames("IRL3", "A", 1, Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving(StandardStateLabels.Start.LabelName)))

            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("IRL3", "CDEFGHI", 15))
            lst.Add(New Expr_Exec(Sub() ECMD_RemoveMe()))

            Return lst
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 400
        End Get
    End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 1
        End Get
    End Property
End Class


'Public Class IronLich
'    Inherits Creature

'    Private _attacks As CreatureAttack()

'    Public Sub New()
'        MyBase.new(DemonSprites.IronLich)

'        Dim lst As New List(Of CreatureAttack)
'        Dim atk1 As New CreatureAttack("B:25", GetType(IronLichProjectile))

'        lst.Add(atk1)

'        _attacks = lst.ToArray
'    End Sub

'    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
'        Get
'            Return _attacks
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property DeathAnimationSequence As String
'        Get
'            Return "C:15,D:15,E:15,F:15,G:15,H:15,I:15"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MaxHitPoints As Integer
'        Get
'            Return 400
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MovementAnimationSequence As String
'        Get
'            Return "A:3"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Passivity As Integer
'        Get
'            Return 200
'        End Get
'    End Property

'    'Public Overrides ReadOnly Property Size As System.Drawing.Size
'    '    Get
'    '        Return New Size(100, 100)
'    '    End Get
'    'End Property
'    Protected Overrides ReadOnly Property Scale As Double
'        Get
'            Return 1
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property TopSpeed As Double
'        Get
'            Return 1
'        End Get
'    End Property
'End Class

Public Class IronLichProjectile
    Inherits Projectile

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.IronLichProjectile, owner, position)
    End Sub

    Public Sub New(ByVal owner As Creature, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.IronLichProjectile, owner, source, target)
    End Sub

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "D:5,E:5,F:5,G:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:5,B:5,C:5"
        End Get
    End Property

    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(25, 25)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.White
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowSize As System.Drawing.Size
        Get
            Return New Size(100, 100)
        End Get
    End Property

    Protected Overrides ReadOnly Property Scale As Double
        Get
            Return 1.5
        End Get
    End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 2
        End Get
    End Property

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 120
        End Get
    End Property

    Protected Overrides ReadOnly Property IsExplosive As Boolean
        Get
            Return True
        End Get
    End Property
End Class
