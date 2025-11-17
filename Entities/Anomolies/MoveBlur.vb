Public Class MoveBlur
    Inherits NonAnimatedEntity

    Public Property BlurSize As Size

    Protected Overrides ReadOnly Property Size As System.Drawing.Size
        Get
            Return Me.BlurSize
        End Get
    End Property

    Protected Overrides ReadOnly Property FadeOut As Boolean
        Get
            Return True
        End Get
    End Property
End Class
