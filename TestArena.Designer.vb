<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestArena
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
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.AddToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Arena1 = New DemonArena.Arena()
        Me.TestThrustToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddToolStripMenuItem, Me.TestThrustToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(492, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'AddToolStripMenuItem
        '
        Me.AddToolStripMenuItem.Name = "AddToolStripMenuItem"
        Me.AddToolStripMenuItem.Size = New System.Drawing.Size(41, 20)
        Me.AddToolStripMenuItem.Text = "Add"
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1
        '
        'Arena1
        '
        Me.Arena1.ArenaBackground = Nothing
        Me.Arena1.ArenaSize = New System.Drawing.Size(1024, 768)
        Me.Arena1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Arena1.GlowEffectsOn = True
        Me.Arena1.Location = New System.Drawing.Point(0, 24)
        Me.Arena1.Name = "Arena1"
        Me.Arena1.Size = New System.Drawing.Size(492, 336)
        Me.Arena1.TabIndex = 1
        Me.Arena1.Text = "Arena1"
        Me.Arena1.UseXNA = True
        '
        'TestThrustToolStripMenuItem
        '
        Me.TestThrustToolStripMenuItem.Name = "TestThrustToolStripMenuItem"
        Me.TestThrustToolStripMenuItem.Size = New System.Drawing.Size(78, 20)
        Me.TestThrustToolStripMenuItem.Text = "Test Thrust"
        '
        'TestArena
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(492, 360)
        Me.Controls.Add(Me.Arena1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "TestArena"
        Me.Text = "TestArena"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents Arena1 As DemonArena.Arena
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents AddToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TestThrustToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
