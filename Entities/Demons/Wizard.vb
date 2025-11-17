'Public Class Wizard
'    Inherits Creature

'    Private _attacks As CreatureAttack()

'    Public Sub New()
'        MyBase.New(DemonSprites.Wizard)

'        Dim lst As New List(Of CreatureAttack)

'        Dim atk1 As New CreatureAttack("C:10,D:10,E:10,F:10", GetType(WizardProjectile))

'        atk1.Vollies = 3
'        atk1.PauseOnLastFrame = True
'        atk1.VolleyDelay = 5

'        _attacks = New CreatureAttack() {atk1}
'    End Sub

'    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
'        Get
'            Return _attacks
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property DeathAnimationSequence As String
'        Get
'            Return "H:10,I:10,J:10,K:10,L:10,M:10,N:10,O:10,P:10,Q:10,R:10,S:10,T:10,U:10,V:10,W:10,X:10"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MaxHitPoints As Integer
'        Get
'            Return 40
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MovementAnimationSequence As String
'        Get
'            Return "A:10,B:10"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Passivity As Integer
'        Get
'            Return 60
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Defense As CreatureDefense
'        Get
'            Return New DashDefense(New Integer() {20, 20, 20, 50}, New Integer() {5, 10, 20}, 10)
'        End Get
'    End Property

'    'Public Overrides ReadOnly Property Size As System.Drawing.Size
'    '    Get
'    '        Return New Size(30, 60)
'    '    End Get
'    'End Property

'    Protected Overrides ReadOnly Property TopSpeed As Double
'        Get
'            Return 1
'        End Get
'    End Property
'End Class

Public Class Wizard
    Inherits ComplexCreature

    Public Sub New()
        MyBase.new(DemonSprites.Wizard)
    End Sub

    Public Overrides ReadOnly Property Species As String
        Get
            Return "Wizard"
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
            lst.Add(New Expr_Frames("BSH7", "AB", 10, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("PrepAttack", 60)))


            lst.Add(New Expr_Label("PrepAttack"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, True)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, True)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(10)))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("BSH7", "CDE", 6))
            lst.Add(New Expr_Goto("CloseDistance"))

            lst.Add(New Expr_Label("CloseDistance"))
            lst.Add(New Expr_Frames("BSH7", "E", 1))
            lst.Add(New Expr_Exec(Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfCloseToTarget(400, "Attack")))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget("DisableGhost")))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNoTarget(StandardStateLabels.Start.LabelName)))

            lst.Add(New Expr_Label("Attack"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(0)))
            lst.Add(New Expr_Frames("BSH7", "E", 3, Sub() ECMD_FireProjAtTarget(GetType(WizardProjectile))))
            lst.Add(New Expr_Frames("BSH7", "E", 3, Sub() ECMD_FireProjAtTarget(GetType(WizardProjectile))))
            lst.Add(New Expr_Frames("BSH7", "E", 3, Sub() ECMD_FireProjAtTarget(GetType(WizardProjectile))))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(1)))
            lst.Add(New Expr_Goto("DisableGhost"))
            lst.Add(New Expr_Goto(StandardStateLabels.Start.LabelName))

            lst.Add(New Expr_Label("DisableGhost"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(1)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.MoveBlur, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, False)))
            lst.Add(New Expr_Return)


            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("BSH7", "HIJKLMNOPQRSTUV", 10, Sub() ECMD_RemoveMe()))

            Return lst
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 65
        End Get
    End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 1
        End Get
    End Property
End Class


Public Class WizardProjectile
    Inherits Projectile

    Public Sub New(ByVal owner As Creature, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.WizardProjectile, owner, source, target)
    End Sub

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.WizardProjectile, owner, position)
    End Sub


    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 10
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "C:5,D:5,E:5,F:5,G:5,H:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Green
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