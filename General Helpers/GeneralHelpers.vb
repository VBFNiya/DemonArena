Public Class GeneralHelpers
    Private Shared _rnd As New Random

    Public Shared Function Chance(ByVal max As Integer) As Boolean
        If _rnd.Next(0, max) = 0 Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Shared Function RollDiceLose(ByVal losePercentage As Integer) As Boolean
        Return RollDice(100 - losePercentage)
    End Function

    Public Shared Function RollDice(ByVal winPercentage As Integer) As Boolean
        If _rnd.Next(0, 100) < winPercentage Then
            'For i = 1 To 100
            '    If Not _rnd.Next(0, 100) < winPercentage Then Return False
            'Next

            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function ScaleSize(ByVal sz As Size, ByVal scale As Double) As Size
        Return New Size(sz.Width * scale, sz.Height * scale)
    End Function

End Class
