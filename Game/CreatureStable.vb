Imports System.Collections.ObjectModel

Public Class CreatureStable
    Inherits Control

    Private Const TILEMARGIN As Integer = 5
    Private g_colCreatures As CreatureCollection

    Public Sub New()
        g_colCreatures = New CreatureCollection
    End Sub

    Public ReadOnly Property Creatures As CreatureCollection
        Get
            Return g_colCreatures
        End Get
    End Property

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        ControlPaint.DrawBorder(e.Graphics, Me.ClientRectangle, Color.Black, ButtonBorderStyle.Solid)

        MyBase.OnPaint(e)
    End Sub

End Class


Public Class CreatureCollection
    Inherits Collection(Of Creature)


End Class