Public Class Fallen
    Inherits Creature

    Private _attacks As CreatureAttack()

    Public Sub New()
        MyBase.New(DemonSprites.Fallen)


        Dim atk1 As New CreatureAttack("A:10", GetType(FallenPrjectile))
        Dim atk2 As New CreatureAttack("A:10", GetType(FallenPrjectile))
        Dim atk3 As New CreatureAttack("A:10", GetType(FallenPrjectile))

        atk2.Vollies = 2
        atk3.Vollies = 3


        _attacks = New CreatureAttack() {atk1, atk2, atk3}
    End Sub

    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
        Get
            Return _attacks
        End Get
    End Property

    Protected Overrides ReadOnly Property DeathAnimationSequence As String
        Get
            Return "H:10,I:10,J:10,K:10,L:10,M:10"
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 80
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationSequence As String
        Get
            Return "A:5,B:5,C:5,D:5,C:5,B:5,A:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property Calmness As Integer
        Get
            Return 25
        End Get
    End Property

    Protected Overrides ReadOnly Property Passivity As Integer
        Get
            Return 100
        End Get
    End Property

    'Public Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(70, 50)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 9
        End Get
    End Property
End Class

Public Class FallenPrjectile
    Inherits Projectile

    Public Sub New(ByVal owner As IArenaEntity, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.FallenProjectile, owner, source, target)
    End Sub

    Public Sub New(ByVal owner As IArenaEntity, ByVal position As PointF)
        MyBase.New(ProjectileSprites.FallenProjectile, owner, position)
    End Sub

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 5
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "C:10,D:10,E:10"
        End Get
    End Property

    Protected Overrides ReadOnly Property GlowColor As System.Drawing.Color
        Get
            Return Color.Red
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:5,B:5"
        End Get
    End Property

    'Protected Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(15, 15)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property Speed As Double
        Get
            Return 7
        End Get
    End Property
End Class
