Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim mv As New MovementTrajectory(2, 10)

        Do
            Dim sp As Double = mv.Speed
            If sp - 0.01 < 0 Then
                Stop
                Exit Do
            End If

            mv.Speed -= 0.01

        Loop



    End Sub
End Class