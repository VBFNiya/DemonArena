Public Class Spirit
    Inherits ComplexCreature

    Public Sub New()
        MyBase.New(DemonSprites.Spirit)
    End Sub

    Protected Overrides ReadOnly Property AgilityPercent As Integer
        Get
            Return 95
        End Get
    End Property

    Protected Overrides ReadOnly Property CreatureDefinition As System.Collections.Generic.IList(Of DefinitionExpression)
        Get
            Dim lst As New List(Of DefinitionExpression)

            lst.Add(StandardStateLabels.Start)
            lst.Add(New Expr_Exec(Sub() ECMD_Start()))
            lst.Add(New Expr_Frames("ETH6", "ABCD", 10, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_Jump("SelectTarget", 80)))

            lst.Add(New Expr_Label("SelectTarget"))
            lst.Add(New Expr_Exec(Sub() ECMD_SelectTargetByID(EFUNC_GetGlobalVarLong("SpiritTarget"))))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfTargetValid("PrepAtk")))
            lst.Add(New Expr_Exec(Sub() ECMD_SelectNewTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_SetGlobalVarLong("SpiritTarget", EFUNC_GetTargetID())))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfTargetValid("PrepAtk")))
            lst.Add(New Expr_Return)

            lst.Add(New Expr_Label("PrepAtk"))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceAndApproachTarget(Me.TopSpeed)))
            lst.Add(New Expr_Frames("ETH6", "A", 5, Sub() ECMD_JumpIfCloseToTarget(300, "Atk")))
            lst.Add(New Expr_Frames("ETH6", "A", 5, Sub() ECMD_JumpIfCloseToTarget(300, "Atk")))
            lst.Add(New Expr_Frames("ETH6", "B", 5, Sub() ECMD_JumpIfCloseToTarget(300, "Atk")))
            lst.Add(New Expr_Frames("ETH6", "B", 5, Sub() ECMD_JumpIfCloseToTarget(300, "Atk")))
            lst.Add(New Expr_Frames("ETH6", "C", 5, Sub() ECMD_JumpIfCloseToTarget(300, "Atk")))
            lst.Add(New Expr_Frames("ETH6", "C", 5, Sub() ECMD_JumpIfCloseToTarget(300, "Atk")))
            lst.Add(New Expr_Frames("ETH6", "D", 5, Sub() ECMD_JumpIfCloseToTarget(300, "Atk")))
            lst.Add(New Expr_Frames("ETH6", "D", 5, Sub() ECMD_JumpIfCloseToTarget(300, "Atk")))
            'lst.Add(New Expr_Exec(Sub() ECMD_JumpIfCloseToTarget(50, "Atk")))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget(StandardStateLabels.Start.LabelName)))

            lst.Add(New Expr_Label("Atk"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(1)))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("ETH6", "E", 2, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("ETH6", "F", 2, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Frames("ETH6", "G", 2, Sub() ECMD_FaceTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjectileFoward(GetType(SpiritProjectile))))
            lst.Add(New Expr_Goto(StandardStateLabels.Start))

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Frames("ETH6", "G", 1, Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving(StandardStateLabels.Start.LabelName)))


            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("ETH6", "HIJKLMNO", 5, Sub() ECMD_RemoveMe()))

            Return lst
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 80
        End Get
    End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 4
        End Get
    End Property
End Class




'Public Class Spirit
'    Inherits Creature

'    Private _attacks As CreatureAttack() =
'        New CreatureAttack() {New CreatureAttack("E:10,F:10,G:10", GetType(SpiritProjectile))}

'    Public Sub New()
'        MyBase.New(DemonSprites.Spirit)
'    End Sub


'    'Public Overrides ReadOnly Property Size As System.Drawing.Size
'    '    Get
'    '        Return New Size(25, 25)
'    '    End Get
'    'End Property


'    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
'        Get
'            Return _attacks
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property DeathAnimationSequence As String
'        Get
'            Return "H:5,I:5,J:5,K:5,L:5,M:5,N:5,O:5"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MovementAnimationSequence As String
'        Get
'            Return "A:10,B:10,C:10,D:10"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Calmness As Integer
'        Get
'            Return 25
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property TopSpeed As Double
'        Get
'            Return 4
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Passivity As Integer
'        Get
'            Return 50
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MaxHitPoints As Integer
'        Get
'            Return 40
'        End Get
'    End Property
'End Class

Public Class SpiritProjectile
    Inherits Projectile


    Public Sub New(ByVal owner As Creature, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.SpiritProjectile, owner, source, target)
    End Sub

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.SpiritProjectile, owner, position)
    End Sub


    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            'TODO: There are more death frames
            Return "C:12,D:12,E:12,F:12,G:12,H:12,I:12,J:12,K:12,L:12"
        End Get
    End Property


    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(10, 10)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 5
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:10,B:10"
        End Get
    End Property

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 10
        End Get
    End Property


    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Violet
        End Get
    End Property
End Class
