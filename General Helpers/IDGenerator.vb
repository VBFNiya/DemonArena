Public Class IDGenerator
    Private Shared gshr_lID As Long

    Public Shared Function GetID() As Long
        gshr_lID += 1
        Return gshr_lID
    End Function
End Class
