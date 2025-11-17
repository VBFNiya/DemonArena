Imports System.Reflection
Imports System.Threading


Public Class FrmArena

    Private g_objRnd As New Random

    Private g_iSpawnCountDown As Integer

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Arena1.CallTick()

        If g_iSpawnCountDown = 0 Then

            If SpawnAtRandomToolStripMenuItem.Checked Then
                If My.Settings.SpawnRatios IsNot Nothing Then
                    'Spawns one at a time but some
                    'more often than others depending on
                    'ratio
                    '*************************
                    'Dim lst As New List(Of Type)

                    'For Each spR As CreatureSpawnRatio In My.Settings.SpawnRatios
                    '    For i = 1 To spR.Ratio
                    '        lst.Add(GetCreatureTypeByName(spR.Creature))
                    '    Next
                    'Next

                    'Dim cre As Creature = Activator.CreateInstance(lst.Item(g_objRnd.Next(0, lst.Count)))

                    'cre.SetPosition(New Point(g_objRnd.Next(0, Me.Arena1.Width), g_objRnd.Next(0, Me.Arena1.Height)))

                    'Me.Arena1.AddEntity(cre)
                    '**************************

                    Dim selectedRatio = My.Settings.SpawnRatios.Item(g_objRnd.Next(0, My.Settings.SpawnRatios.Count))

                    For i = 1 To selectedRatio.Ratio
                        Dim cre As IDemon = Activator.CreateInstance(GetCreatureTypeByName(selectedRatio.Creature))
                        cre.SetPosition(New Point(g_objRnd.Next(0, Me.Arena1.ArenaSize.Width), g_objRnd.Next(0, Me.Arena1.ArenaSize.Height)))

                        Me.Arena1.AddEntity(cre)
                    Next

                Else
                    SpawnAtRandomToolStripMenuItem.Checked = False
                    MessageBox.Show("Please setup spawn ratios for random spawning", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            End If
            g_iSpawnCountDown = My.Settings.SpawnDuration
        Else
            g_iSpawnCountDown -= 1
        End If

    End Sub

    Private Function GetCreatureTypeByName(ByVal name As String) As Type
        For Each t As Type In Assembly.GetExecutingAssembly.GetTypes
            If t.Name = name Then Return t
        Next

        Return Nothing
    End Function

    Private Sub FrmArena_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
        Dim size As Size = Me.Arena1.ArenaSize
        Dim scaleFactor As Double = 0.2

        If e.KeyChar = "+" Then
            size.Width *= (scaleFactor + 1)
            size.Height *= (scaleFactor + 1)

            Me.Arena1.ArenaSize = size
        End If

        If e.KeyChar = "-" Then
            size.Width /= (scaleFactor + 1)
            size.Height /= (scaleFactor + 1)

            Me.Arena1.ArenaSize = size
        End If


    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        g_iSpawnCountDown = My.Settings.SpawnDuration

        Arena1.ArenaBackground = My.Resources.ArenaBackgrounds.DeadWorld

        Dim asm As Assembly = Assembly.GetExecutingAssembly()

        For Each t As Type In asm.GetTypes
            If t.GetInterface("IDemon") IsNot Nothing AndAlso Not t.IsAbstract Then
                Dim mi As New ToolStripMenuItem

                mi.Text = t.Name
                mi.Tag = t

                AddHandler mi.Click, AddressOf EH_MenuItemClick
                MenuStrip1.Items.Add(mi)
            End If
        Next

    End Sub
    Private Sub EH_MenuItemClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim mi As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

        Dim t As Type = mi.Tag

        'For i = 1 To 5000

        Dim cre As IDemon = Activator.CreateInstance(t)


        cre.SetPosition(New PointF(g_objRnd.Next(0, Arena1.ArenaSize.Width),
                                   g_objRnd.Next(0, Arena1.ArenaSize.Height)))

        Arena1.AddEntity(cre)
        'Next
    End Sub

    Private ReadOnly Property BackGroundOriginal As Bitmap
        Get
            Return My.Resources.ArenaBackgrounds.Beach
        End Get
    End Property

    Private Sub AddCreatureToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Dim lst As New List(Of Creature)
        'Dim rnd As New Random

        'lst.Add(New Wraith)
        'lst.Add(New Spirit)
        'lst.Add(New BlueSpider)

        'Arena1.AddEntity(lst.Item(rnd.Next(0, lst.Count)))
        'Threading.Thread.Sleep(5)
    End Sub

    Private Sub AddTestProjectileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim proj As New WraithProjectile(Nothing, New Point(0, 0), New Point(100, 500))

        Arena1.AddEntity(proj)
    End Sub

    Private Sub TestAnomolyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim x As New TeleportFog

        x.SetPosition(New PointF(200, 200))

        Arena1.AddEntity(x)
    End Sub

    Private Sub GlowEffectsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GlowEffectsToolStripMenuItem.Click

        Arena1.GlowEffectsOn = GlowEffectsToolStripMenuItem.Checked

    End Sub

    Private Sub KillAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KillAllToolStripMenuItem.Click

        For Each ent As IDemon In Arena1.Entities.OfType(Of IDemon)()
            ent.DeductHitPoints(1000)
        Next

    End Sub

    Private Sub TestThrustToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim cre As Creature = Arena1.Entities.FirstOrDefault(Function(t) TypeOf t Is Creature)

        If cre IsNot Nothing Then
            cre.Thrust(New PointF(-100, -100), 10)
        End If

    End Sub

    Private Sub Arena1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Arena1.Click

    End Sub

    Private Sub UseXNAToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UseXNAToolStripMenuItem.Click
        Arena1.UseXNA = UseXNAToolStripMenuItem.Checked
    End Sub

    Private Sub SpawnSettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SpawnSettingsToolStripMenuItem.Click
        Dim f As New FrmRandomSpawn

        f.Show()
    End Sub

    'Private Sub PerfTestToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PerfTestToolStripMenuItem.Click

    '    For i = 1 To 5000
    '        Dim cre As New Wraith
    '        Me.Arena1.AddEntity(cre)
    '    Next

    'End Sub
End Class

