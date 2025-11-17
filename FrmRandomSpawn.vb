Imports System.Reflection
Imports System.Collections.ObjectModel

Public Class FrmRandomSpawn

    Public Property Arena As IArena

    Private Const SPAWNDURATIONDEFAULT As Integer = 100

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim asm As Assembly = Assembly.GetExecutingAssembly()
        Dim i As Integer = 0

        Dim leftMargin As Integer = 9
        Dim curTop As Integer = 90

        For Each t As Type In asm.GetTypes
            If t.GetInterface("IDemon") IsNot Nothing AndAlso Not t.IsAbstract Then

                Dim l As New Label
                Dim n As New NumericUpDown

                Me.Controls.Add(l)
                Me.Controls.Add(n)

                l.Text = t.Name
                l.Left = leftMargin
                l.Top = curTop

                n.Tag = t
                n.Minimum = 1
                n.Maximum = 100
                n.Left = leftMargin + 150
                n.Top = curTop

                curTop += l.Height + 5

            End If
        Next

        Me.Height = curTop + 100

    End Sub

    Private Sub SaveRatios()

        Dim col As New SpawnRatioCollection

        For Each nud In Me.Controls.OfType(Of NumericUpDown)().Where(Function(n) n.Tag IsNot Nothing)
            col.Add(New CreatureSpawnRatio(DirectCast(nud.Tag, Type).Name, nud.Value))
        Next

        My.Settings.SpawnDuration = nudSpawnDuration.Value
        My.Settings.SpawnRatios = col
        My.Settings.Save()
    End Sub

    Private Sub LoadRatios()
        nudSpawnDuration.Value = If(My.Settings.SpawnDuration = 0, SPAWNDURATIONDEFAULT, My.Settings.SpawnDuration)

        If My.Settings.SpawnRatios IsNot Nothing Then
            For Each ratio In My.Settings.SpawnRatios
                Dim ratio1 As CreatureSpawnRatio = ratio

                Dim nud = Me.Controls.OfType(Of NumericUpDown).FirstOrDefault(Function(n) n.Tag IsNot Nothing AndAlso DirectCast(n.Tag, Type).Name = ratio1.Creature)

                If nud IsNot Nothing Then nud.Value = ratio.Ratio
            Next
        End If

    End Sub

    Private Sub FrmRandomSpawn_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        SaveRatios()
    End Sub

    Private Sub FrmRandomSpawn_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadRatios()
    End Sub

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Me.Close()
    End Sub

    Private Sub btnDefaults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDefaults.Click
        nudSpawnDuration.Value = SPAWNDURATIONDEFAULT

        For Each nud In Me.Controls.OfType(Of NumericUpDown).Where(Function(n) n.Tag IsNot Nothing)
            nud.Value = 1
        Next
    End Sub
End Class

'<Serializable()> _
Public Class SpawnRatioCollection
    Inherits Collection(Of CreatureSpawnRatio)
End Class

'<Serializable()> _
Public Class CreatureSpawnRatio
    Public Sub New(ByVal creatureType As String, ByVal ratio As Integer)
        Me.Creature = creatureType
        Me.Ratio = ratio
    End Sub

    Public Sub New()

    End Sub

    Public Property Creature As String
    Public Property Ratio As Integer
End Class