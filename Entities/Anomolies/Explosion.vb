Public MustInherit Class Explosion
    Inherits Anomaly

    Public Sub New(ByVal explosionSprites As SpriteCollection)
        MyBase.New(explosionSprites)
    End Sub

    Public Overrides ReadOnly Property AnimationString As String
        Get
            Return "A:3,B:3,C:3,D:3,E:3,F:3,G:3,H:3,I:3,J:3,K:3,L:3,M:3,N:3,O:3,P:3,Q:3,R:3,S:3,T:3,U:3"
        End Get
    End Property

    Public Overrides ReadOnly Property Size As System.Drawing.Size
        Get
            Return New Size(60, 60)
        End Get
    End Property
End Class

Public Class BlueExplosion
    Inherits Explosion

    Public Sub New()
        MyBase.New(AnomolySprites.BlueExplosion)
    End Sub
End Class
