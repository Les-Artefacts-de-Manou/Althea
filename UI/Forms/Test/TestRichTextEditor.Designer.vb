<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TestRichTextEditor
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        pnlTop = New Panel()
        btnEffacer = New Button()
        chkShowToolbar = New CheckBox()
        chkReadOnly = New CheckBox()
        btnCharger = New Button()
        btnSauvegarder = New Button()
        lblTitre = New Label()
        pnlBottom = New Panel()
        lblStatus = New Label()
        ucEditor = New UC_RichTextEditor()
        pnlTop.SuspendLayout()
        pnlBottom.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.FromArgb(CByte(178), CByte(197), CByte(186))
        pnlTop.Controls.Add(btnEffacer)
        pnlTop.Controls.Add(chkShowToolbar)
        pnlTop.Controls.Add(chkReadOnly)
        pnlTop.Controls.Add(btnCharger)
        pnlTop.Controls.Add(btnSauvegarder)
        pnlTop.Controls.Add(lblTitre)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Location = New Point(0, 0)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(8)
        pnlTop.Size = New Size(1000, 60)
        pnlTop.TabIndex = 0
        ' 
        ' btnEffacer
        ' 
        btnEffacer.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnEffacer.BackColor = Color.FromArgb(CByte(194), CByte(106), CByte(118))
        btnEffacer.FlatStyle = FlatStyle.Flat
        btnEffacer.ForeColor = Color.White
        btnEffacer.Location = New Point(888, 16)
        btnEffacer.Name = "btnEffacer"
        btnEffacer.Size = New Size(100, 30)
        btnEffacer.TabIndex = 5
        btnEffacer.Text = "Effacer tout"
        btnEffacer.UseVisualStyleBackColor = False
        ' 
        ' chkShowToolbar
        ' 
        chkShowToolbar.AutoSize = True
        chkShowToolbar.Checked = True
        chkShowToolbar.CheckState = CheckState.Checked
        chkShowToolbar.Location = New Point(620, 22)
        chkShowToolbar.Name = "chkShowToolbar"
        chkShowToolbar.Size = New Size(113, 17)
        chkShowToolbar.TabIndex = 4
        chkShowToolbar.Text = "Afficher la toolbar"
        chkShowToolbar.UseVisualStyleBackColor = True
        ' 
        ' chkReadOnly
        ' 
        chkReadOnly.AutoSize = True
        chkReadOnly.Location = New Point(500, 22)
        chkReadOnly.Name = "chkReadOnly"
        chkReadOnly.Size = New Size(114, 17)
        chkReadOnly.TabIndex = 3
        chkReadOnly.Text = "Mode lecture seule"
        chkReadOnly.UseVisualStyleBackColor = True
        ' 
        ' btnCharger
        ' 
        btnCharger.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnCharger.Enabled = False
        btnCharger.FlatStyle = FlatStyle.Flat
        btnCharger.ForeColor = Color.White
        btnCharger.Location = New Point(280, 16)
        btnCharger.Name = "btnCharger"
        btnCharger.Size = New Size(100, 30)
        btnCharger.TabIndex = 2
        btnCharger.Text = "Charger (BDD)"
        btnCharger.UseVisualStyleBackColor = False
        ' 
        ' btnSauvegarder
        ' 
        btnSauvegarder.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnSauvegarder.Enabled = False
        btnSauvegarder.FlatStyle = FlatStyle.Flat
        btnSauvegarder.ForeColor = Color.White
        btnSauvegarder.Location = New Point(390, 16)
        btnSauvegarder.Name = "btnSauvegarder"
        btnSauvegarder.Size = New Size(100, 30)
        btnSauvegarder.TabIndex = 1
        btnSauvegarder.Text = "Sauvegarder"
        btnSauvegarder.UseVisualStyleBackColor = False
        ' 
        ' lblTitre
        ' 
        lblTitre.AutoSize = True
        lblTitre.Font = New Font("Calibri", 14F, FontStyle.Bold)
        lblTitre.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitre.Location = New Point(12, 18)
        lblTitre.Name = "lblTitre"
        lblTitre.Size = New Size(250, 23)
        lblTitre.TabIndex = 0
        lblTitre.Text = "Test UC_RichTextEditor - Althéa"
        ' 
        ' pnlBottom
        ' 
        pnlBottom.BackColor = Color.FromArgb(CByte(218), CByte(201), CByte(184))
        pnlBottom.Controls.Add(lblStatus)
        pnlBottom.Dock = DockStyle.Bottom
        pnlBottom.Location = New Point(0, 660)
        pnlBottom.Name = "pnlBottom"
        pnlBottom.Size = New Size(1000, 40)
        pnlBottom.TabIndex = 1
        ' 
        ' lblStatus
        ' 
        lblStatus.AutoSize = True
        lblStatus.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblStatus.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblStatus.Location = New Point(12, 12)
        lblStatus.Name = "lblStatus"
        lblStatus.Size = New Size(157, 17)
        lblStatus.TabIndex = 0
        lblStatus.Text = "Prêt - Éditeur initialisé"
        ' 
        ' ucEditor
        ' 
        ucEditor.BackColor = Color.Transparent
        ucEditor.Dock = DockStyle.Fill
        ucEditor.Location = New Point(0, 60)
        ucEditor.Name = "ucEditor"
        ucEditor.ReadOnlyMode = False
        ucEditor.ShowToolbar = True
        ucEditor.Size = New Size(1000, 600)
        ucEditor.TabIndex = 2
        ' 
        ' TestRichTextEditor
        ' 
        AutoScaleDimensions = New SizeF(6F, 13F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1000, 700)
        Controls.Add(ucEditor)
        Controls.Add(pnlBottom)
        Controls.Add(pnlTop)
        MinimumSize = New Size(800, 600)
        Name = "TestRichTextEditor"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Test RichTextEditor - Althéa"
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        pnlBottom.ResumeLayout(False)
        pnlBottom.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlTop As Panel
    Friend WithEvents lblTitre As Label
    Friend WithEvents btnSauvegarder As Button
    Friend WithEvents btnCharger As Button
    Friend WithEvents chkReadOnly As CheckBox
    Friend WithEvents chkShowToolbar As CheckBox
    Friend WithEvents pnlBottom As Panel
    Friend WithEvents lblStatus As Label
    Friend WithEvents ucEditor As UC_RichTextEditor
    Friend WithEvents btnEffacer As Button

End Class
