Public Class TestArena

    Private Sub TestArena_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Arena1.ArenaBackground = My.Resources.ArenaBackgrounds.BlueBay

    End Sub

    Private Sub AddToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddToolStripMenuItem.Click

        Dim cre As New PurpleAfrit

        cre.SetPosition(New PointF(200, 200))
        Arena1.AddEntity(cre)

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Arena1.CallTick()
    End Sub


    Private Sub TestThrustToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestThrustToolStripMenuItem.Click

        If Me.Arena1.Entities.Count > 0 Then
            DirectCast(Me.Arena1.Entities.Item(0), ComplexCreature).Thrust(New Point(600, 600), 10)

        End If

    End Sub
End Class