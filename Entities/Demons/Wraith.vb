

Public Class Wraith
    Inherits ComplexCreature

    Public Sub New()
        MyBase.New(DemonSprites.Wraith)
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
            lst.Add(New Expr_Frames("WR03", "ABCD", 10, Sub() ECMD_Roam()))
            lst.Add(New Expr_Exec(Sub() ECMD_AttackRandomTarget("Atk", 60)))
            lst.Add(New Expr_Exec(Sub() ECMD_Jump("GhostModeCheck", 20)))

            lst.Add(New Expr_Label("Atk"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetSpeed(0.5)))
            lst.Add(New Expr_Frames("WR03", "E", 10, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("WR03", "F", 10, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Frames("WR03", "G", 30, Sub() ECMD_FaceAndApproachTarget()))
            lst.Add(New Expr_Exec(Sub() ECMD_FireProjAtTarget(GetType(WraithProjectile))))
            lst.Add(New Expr_Exec(Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Return)

            lst.Add(New Expr_Label("GhostModeCheck"))
            lst.Add(New Expr_Exec(Sub() EMCD_JumpIfTrue(EFUNC_GetLocalVarLong("Fading"), StandardStateLabels.Start.LabelName)))
            lst.Add(New Expr_Exec(Sub() EMCD_JumpIfFalse(EFUNC_GetLocalVarLong("GModeOn"), "GhostModeOn")))
            lst.Add(New Expr_Exec(Sub() ECMD_Goto("GhostModeOff")))
            lst.Add(New Expr_Return)

            lst.Add(New Expr_Label("GhostModeOff"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetLocalVarLong("GModeOn", 0)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, False)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetLocalVarLong("Fading", 1)))
            lst.Add(New Expr_Exec(Sub() EMCD_GotoParallel("FadeIn")))
            lst.Add(New Expr_Goto(StandardStateLabels.Start))


            lst.Add(New Expr_Label("GhostModeOn"))
            lst.Add(New Expr_Exec(Sub() ECMD_SetLocalVarLong("GModeOn", 1)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetFlag(CreatureFlags.GhostMode, True)))
            lst.Add(New Expr_Exec(Sub() ECMD_SetLocalVarLong("Fading", 1)))
            lst.Add(New Expr_Exec(Sub() EMCD_GotoParallel("FadeOut")))
            lst.Add(New Expr_Goto(StandardStateLabels.Start))

            lst.Add(New Expr_Label("FadeIn"))
            lst.Add(New Expr_Exec(Sub() ECMD_AddAlpha(0.07)))
            lst.Add(New Expr_Exec(Sub() ECMD_Yield()))
            lst.Add(New Expr_LoopFixed(10))
            lst.Add(New Expr_Exec(Sub() ECMD_SetLocalVarLong("Fading", 0)))
            lst.Add(New Expr_KillThread)


            lst.Add(New Expr_Label("FadeOut"))
            lst.Add(New Expr_Exec(Sub() ECMD_SubtractAlpha(0.07)))
            lst.Add(New Expr_Exec(Sub() ECMD_Yield()))
            lst.Add(New Expr_LoopFixed(10))
            lst.Add(New Expr_Exec(Sub() ECMD_SetLocalVarLong("Fading", 0)))
            lst.Add(New Expr_KillThread)

            lst.Add(StandardStateLabels.Thrusted)
            lst.Add(New Expr_Frames("WR03", "H", 1, Sub() ECMD_AllowStart()))
            lst.Add(New Expr_Exec(Sub() ECMD_JumpIfNotMoving(StandardStateLabels.Start.LabelName)))

            lst.Add(StandardStateLabels.Dying)
            lst.Add(New Expr_Frames("WR03", "IJ", 50))
            lst.Add(New Expr_Frames("WR03", "KLMNOPQR", 5, Sub() ECMD_RemoveMe()))


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
            Return 2
        End Get
    End Property
End Class


'Public Class WraithOLD
'    Inherits Creature

'    Private _attacks As CreatureAttack() =
'    New CreatureAttack() {New CreatureAttack("E:10,F:10,G:30", GetType(WraithProjectile))}

'    Public Sub New()
'        MyBase.New(DemonSprites.Wraith)
'    End Sub

'    'Public Overrides ReadOnly Property Size As System.Drawing.Size
'    '    Get
'    '        Return New Size(50, 50)
'    '    End Get
'    'End Property

'    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
'        Get
'            Return _attacks
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property DeathAnimationSequence As String
'        Get
'            Return "I:50,J:50,K:5,L:5,M:5,N:5,O:5,P:5,Q:5,R:5"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MovementAnimationSequence As String
'        Get
'            Return "A:10,B:10,C:10,D:10"
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property TopSpeed As Double
'        Get
'            Return 2
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property Passivity As Integer
'        Get
'            Return 200
'        End Get
'    End Property

'    Protected Overrides ReadOnly Property MaxHitPoints As Integer
'        Get
'            Return 150
'        End Get
'    End Property
'End Class

Public Class WraithProjectile
    Inherits Projectile

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.WraithProjectile, owner, position)
    End Sub

    Public Sub New(ByVal owner As Creature, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.WraithProjectile, owner, source, target)
    End Sub

    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(20, 20)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 12
        End Get
    End Property

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

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 50
        End Get
    End Property

    Protected Overrides ReadOnly Property Scale As Double
        Get
            Return 0.7
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Red
        End Get
    End Property
End Class
