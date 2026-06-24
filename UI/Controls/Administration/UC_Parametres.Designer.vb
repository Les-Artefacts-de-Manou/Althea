<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_Parametres
    Inherits System.Windows.Forms.UserControl

    'UserControl remplace la méthode Dispose pour nettoyer la liste des composants.
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

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Parametres))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        tabParametres = New TabControl()
        pnlActions = New Panel()
        btnAnnuler = New Button()
        btnEnregistrer = New Button()
        btnNouveau = New Button()
        btnModifier = New Button()
        pnlTop = New Panel()
        chkAfficherInactifs = New CheckBox()
        pnlTitre = New Panel()
        lblTitreForm = New Label()
        picTitre = New PictureBox()
        lblTop = New Label()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTop.SuspendLayout()
        pnlTitre.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackgroundImage = CType(resources.GetObject("pnlForm.BackgroundImage"), Image)
        pnlForm.BackgroundImageLayout = ImageLayout.Stretch
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlTitre)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 1
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(tabParametres)
        pnlCenter.Controls.Add(pnlActions)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16)
        pnlCenter.Size = New Size(965, 681)
        pnlCenter.TabIndex = 5
        ' 
        ' tabParametres
        ' 
        tabParametres.Appearance = TabAppearance.FlatButtons
        tabParametres.Dock = DockStyle.Fill
        tabParametres.Location = New Point(16, 58)
        tabParametres.Name = "tabParametres"
        tabParametres.SelectedIndex = 0
        tabParametres.Size = New Size(933, 553)
        tabParametres.TabIndex = 20
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.FromArgb(CByte(231), CByte(223), CByte(214))
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.Fixed3D
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 611)
        pnlActions.Name = "pnlActions"
        pnlActions.Padding = New Padding(8)
        pnlActions.Size = New Size(933, 54)
        pnlActions.TabIndex = 19
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAnnuler.BackgroundImageLayout = ImageLayout.Center
        btnAnnuler.FlatAppearance.BorderSize = 0
        btnAnnuler.FlatStyle = FlatStyle.Flat
        btnAnnuler.ForeColor = Color.White
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.ImageAlign = ContentAlignment.MiddleLeft
        btnAnnuler.Location = New Point(587, 7)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(112, 40)
        btnAnnuler.TabIndex = 10
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.TextAlign = ContentAlignment.MiddleLeft
        btnAnnuler.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAnnuler.UseVisualStyleBackColor = False
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnEnregistrer.BackgroundImageLayout = ImageLayout.Center
        btnEnregistrer.FlatAppearance.BorderSize = 0
        btnEnregistrer.FlatStyle = FlatStyle.Flat
        btnEnregistrer.ForeColor = Color.White
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.ImageAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.Location = New Point(469, 7)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(112, 40)
        btnEnregistrer.TabIndex = 9
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.TextAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEnregistrer.UseVisualStyleBackColor = False
        ' 
        ' btnNouveau
        ' 
        btnNouveau.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnNouveau.BackgroundImageLayout = ImageLayout.Center
        btnNouveau.FlatAppearance.BorderSize = 0
        btnNouveau.FlatStyle = FlatStyle.Flat
        btnNouveau.ForeColor = Color.White
        btnNouveau.Image = CType(resources.GetObject("btnNouveau.Image"), Image)
        btnNouveau.ImageAlign = ContentAlignment.MiddleLeft
        btnNouveau.Location = New Point(233, 7)
        btnNouveau.Name = "btnNouveau"
        btnNouveau.Size = New Size(112, 40)
        btnNouveau.TabIndex = 8
        btnNouveau.Tag = "nouveau_normal"
        btnNouveau.Text = "Nouveau"
        btnNouveau.TextAlign = ContentAlignment.MiddleLeft
        btnNouveau.TextImageRelation = TextImageRelation.ImageBeforeText
        btnNouveau.UseVisualStyleBackColor = False
        ' 
        ' btnModifier
        ' 
        btnModifier.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnModifier.BackgroundImageLayout = ImageLayout.Center
        btnModifier.FlatAppearance.BorderSize = 0
        btnModifier.FlatStyle = FlatStyle.Flat
        btnModifier.ForeColor = Color.White
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.ImageAlign = ContentAlignment.MiddleLeft
        btnModifier.Location = New Point(351, 7)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(112, 40)
        btnModifier.TabIndex = 7
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        btnModifier.TextAlign = ContentAlignment.MiddleLeft
        btnModifier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifier.UseVisualStyleBackColor = False
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.Transparent
        pnlTop.Controls.Add(chkAfficherInactifs)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(8)
        pnlTop.Size = New Size(933, 42)
        pnlTop.TabIndex = 6
        ' 
        ' chkAfficherInactifs
        ' 
        chkAfficherInactifs.AutoSize = True
        chkAfficherInactifs.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkAfficherInactifs.Location = New Point(636, 11)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(237, 22)
        chkAfficherInactifs.TabIndex = 1
        chkAfficherInactifs.Text = "Afficher les paramètres désactivés"
        chkAfficherInactifs.UseVisualStyleBackColor = True
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTop)
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Controls.Add(picTitre)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(16, 16)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(965, 58)
        pnlTitre.TabIndex = 4
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.Anchor = AnchorStyles.Left
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 18F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(71, 3)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(0, 4, 8, 4)
        lblTitreForm.Size = New Size(137, 37)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Paramètres"
        ' 
        ' picTitre
        ' 
        picTitre.BackgroundImage = CType(resources.GetObject("picTitre.BackgroundImage"), Image)
        picTitre.BackgroundImageLayout = ImageLayout.Stretch
        picTitre.Location = New Point(14, 3)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(60, 52)
        picTitre.TabIndex = 2
        picTitre.TabStop = False
        ' 
        ' lblTop
        ' 
        lblTop.Font = New Font("Calibri", 12F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(232, 16)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(500, 26)
        lblTop.TabIndex = 3
        lblTop.Text = "Gestion des paramètres applicatifs : chemins, stockage, options"
        ' 
        ' UC_Parametres
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        BackgroundImageLayout = ImageLayout.Center
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_Parametres"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents picTitre As PictureBox
    Friend WithEvents pnlTop As Panel
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnModifier As Button
    Friend WithEvents tabParametres As TabControl
    Friend WithEvents btnAnnuler As Button
    Friend WithEvents btnEnregistrer As Button
    Friend WithEvents btnNouveau As Button
    Friend WithEvents chkAfficherInactifs As CheckBox
    Friend WithEvents lblTop As Label

End Class
