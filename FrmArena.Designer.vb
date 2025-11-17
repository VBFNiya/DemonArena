<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmArena
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpawnSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpawnAtRandomToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.KillAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.GlowEffectsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UseXNAToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Arena1 = New DemonArena.Arena()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OptionsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(489, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SpawnSettingsToolStripMenuItem, Me.SpawnAtRandomToolStripMenuItem, Me.ToolStripMenuItem2, Me.KillAllToolStripMenuItem, Me.ToolStripMenuItem1, Me.GlowEffectsToolStripMenuItem, Me.UseXNAToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(61, 20)
        Me.OptionsToolStripMenuItem.Text = "Options"
        '
        'SpawnSettingsToolStripMenuItem
        '
        Me.SpawnSettingsToolStripMenuItem.Name = "SpawnSettingsToolStripMenuItem"
        Me.SpawnSettingsToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.SpawnSettingsToolStripMenuItem.Text = "Spawn Settings"
        '
        'SpawnAtRandomToolStripMenuItem
        '
        Me.SpawnAtRandomToolStripMenuItem.CheckOnClick = True
        Me.SpawnAtRandomToolStripMenuItem.Name = "SpawnAtRandomToolStripMenuItem"
        Me.SpawnAtRandomToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.SpawnAtRandomToolStripMenuItem.Text = "Spawn at random"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(164, 6)
        '
        'KillAllToolStripMenuItem
        '
        Me.KillAllToolStripMenuItem.Name = "KillAllToolStripMenuItem"
        Me.KillAllToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.KillAllToolStripMenuItem.Text = "Kill all"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(164, 6)
        '
        'GlowEffectsToolStripMenuItem
        '
        Me.GlowEffectsToolStripMenuItem.Checked = True
        Me.GlowEffectsToolStripMenuItem.CheckOnClick = True
        Me.GlowEffectsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.GlowEffectsToolStripMenuItem.Name = "GlowEffectsToolStripMenuItem"
        Me.GlowEffectsToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.GlowEffectsToolStripMenuItem.Text = "Glow Effects"
        '
        'UseXNAToolStripMenuItem
        '
        Me.UseXNAToolStripMenuItem.Checked = True
        Me.UseXNAToolStripMenuItem.CheckOnClick = True
        Me.UseXNAToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.UseXNAToolStripMenuItem.Name = "UseXNAToolStripMenuItem"
        Me.UseXNAToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.UseXNAToolStripMenuItem.Text = "Use XNA"
        '
        'Arena1
        '
        Me.Arena1.ArenaBackground = Nothing
        Me.Arena1.ArenaSize = New System.Drawing.Size(1024, 768)
        Me.Arena1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Arena1.GlowEffectsOn = True
        Me.Arena1.Location = New System.Drawing.Point(0, 24)
        Me.Arena1.Name = "Arena1"
        Me.Arena1.Size = New System.Drawing.Size(489, 325)
        Me.Arena1.TabIndex = 1
        Me.Arena1.Text = "Arena1"
        Me.Arena1.UseXNA = True
        '
        'FrmArena
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(489, 349)
        Me.Controls.Add(Me.Arena1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FrmArena"
        Me.Text = "Form1"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents Arena1 As DemonArena.Arena
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GlowEffectsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents KillAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UseXNAToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SpawnAtRandomToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SpawnSettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator

End Class
