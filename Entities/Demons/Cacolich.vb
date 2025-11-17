Public Class Cacolich
    Inherits Creature

    Private _attacks As CreatureAttack()

    Public Sub New()
        MyBase.New(DemonSprites.Cacolich)

        Dim lst As New List(Of CreatureAttack)
        Dim atk1 As New CreatureAttack("C:10,D:10,E:10,F:10", GetType(CacoProjectile), 15)
        Dim atk2 As New CreatureAttack("C:10,D:10,E:10,F:10", GetType(CacoProjectile), 30)
        Dim atk3 As New CreatureAttack("C:10,D:10,E:10,F:10", GetType(CacoProjectile), 100)
        Dim atk4 As New CreatureAttack("C:10,D:10,E:10,F:10", GetType(CacoProjectile), 300)
        Dim volleyDelay As Integer = 1


        atk1.VolleyDelay = volleyDelay
        atk1.SlowSpeedToAttack = True
        atk1.PauseOnLastFrame = True

        atk2.VolleyDelay = volleyDelay
        atk2.SlowSpeedToAttack = True
        atk2.PauseOnLastFrame = True

        atk3.VolleyDelay = volleyDelay
        atk3.SlowSpeedToAttack = True
        atk3.PauseOnLastFrame = True

        atk4.VolleyDelay = volleyDelay
        atk4.SlowSpeedToAttack = True
        atk4.PauseOnLastFrame = True

        atk1.ChanceRatio = 10
        atk2.ChanceRatio = 7
        atk3.ChanceRatio = 3
        atk4.ChanceRatio = 1


        With lst
            .Add(atk1)
            .Add(atk2)
            .Add(atk3)
            .Add(atk4)
        End With


        _attacks = lst.ToArray
    End Sub

    Protected Overrides ReadOnly Property Attacks As System.Collections.Generic.IList(Of CreatureAttack)
        Get
            Return _attacks
        End Get
    End Property

    Protected Overrides ReadOnly Property DeathAnimationSequence As String
        Get
            Return "J:5,K:5,L:5,M:5,N:10,O:10,P:10,Q:10,R:10,S:20"
        End Get
    End Property

    Protected Overrides ReadOnly Property MaxHitPoints As Integer
        Get
            Return 100
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationSequence As String
        Get
            Return "A:5,B:5"
        End Get
    End Property

    Protected Overrides ReadOnly Property Passivity As Integer
        Get
            Return 50
        End Get
    End Property

    'Public Overrides ReadOnly Property Size As System.Drawing.Size
    '    Get
    '        Return New Size(50, 50)
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property TopSpeed As Double
        Get
            Return 4
        End Get
    End Property
End Class

Public Class CacoProjectile
    Inherits Projectile

    Public Sub New(ByVal owner As Creature, ByVal source As Point, ByVal target As Point)
        MyBase.New(ProjectileSprites.CacolichProjectile, owner, source, target)
    End Sub

    Protected Overrides ReadOnly Property Damage As Integer
        Get
            Return 1
        End Get
    End Property

    Protected Overrides ReadOnly Property ExplodeAnimationString As String
        Get
            Return "C:2,D:2,E:2,F:2,G:2,H:2,I:2,J:2"
        End Get
    End Property

    Protected Overrides ReadOnly Property MovementAnimationString As String
        Get
            Return "A:5,B:5"
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
            Return Color.Red
        End Get
    End Property
End Class

