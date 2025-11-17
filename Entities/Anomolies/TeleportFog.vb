Public Class TeleportFog
    Inherits Anomaly

    Public Sub New()
        MyBase.New(AnomolySprites.TeleportFog)
    End Sub

    Public Overrides ReadOnly Property AnimationString As String
        Get
            'Return "A:5,B:5,C:5,D:5,E:5,F:2,G:2,H:2,I:2,J:2"

            Return "A:5,B:5,C:5,D:5"

        End Get
    End Property

    Protected Overrides ReadOnly Property FadeOut As Boolean
        Get
            Return True
        End Get
    End Property
    Protected Overrides ReadOnly Property LoopAnimation As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property Size As System.Drawing.Size
        Get
            Return New Size(60, 60)
        End Get
    End Property
End Class
