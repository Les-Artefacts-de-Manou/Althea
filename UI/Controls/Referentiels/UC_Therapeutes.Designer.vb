<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_Therapeutes
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

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Therapeutes))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        dgvTherapeutes = New DataGridView()
        colNom = New DataGridViewTextBoxColumn()
        colPrenom = New DataGridViewTextBoxColumn()
        colSpecialite = New DataGridViewTextBoxColumn()
        colTelephone = New DataGridViewTextBoxColumn()
        colEmail = New DataGridViewTextBoxColumn()
        colLocalite = New DataGridViewTextBoxColumn()
        colActif = New DataGridViewCheckBoxColumn()
        colIdTherapeute = New DataGridViewTextBoxColumn()
        colCodeTherapeute = New DataGridViewTextBoxColumn()
        pnlTop = New Panel()
        btnRechercher = New Button()
        btnReinitialiserFiltres = New Button()
        chkAfficherInactifs = New CheckBox()
        lblRecherche = New Label()
        txtRecherche = New TextBox()
        pnlActions = New Panel()
        btnNouveau = New Button()
        btnModifier = New Button()
        btnActiverDesactiver = New Button()
        btnSupprimer = New Button()
        btnActualiser = New Button()
        pnlTitre = New Panel()
        lblTop = New Label()
        picTitre = New PictureBox()
        lblTitreForm = New Label()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        CType(dgvTherapeutes, ComponentModel.ISupportInitialize).BeginInit()
        pnlTop.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTitre.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        pnlForm.BackgroundImage = My.Resources.Resources.Fond_1000x770_FeuilleCoupee1
        pnlForm.BackgroundImageLayout = ImageLayout.Stretch
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlActions)
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
        pnlCenter.Controls.Add(dgvTherapeutes)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(965, 624)
        pnlCenter.TabIndex = 21
        ' 
        ' dgvTherapeutes
        ' 
        dgvTherapeutes.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvTherapeutes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvTherapeutes.Columns.AddRange(New DataGridViewColumn() {colNom, colPrenom, colSpecialite, colTelephone, colEmail, colLocalite, colActif, colIdTherapeute, colCodeTherapeute})
        dgvTherapeutes.Dock = DockStyle.Fill
        dgvTherapeutes.Location = New Point(16, 80)
        dgvTherapeutes.Name = "dgvTherapeutes"
        dgvTherapeutes.ReadOnly = True
        dgvTherapeutes.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvTherapeutes.Size = New Size(933, 536)
        dgvTherapeutes.TabIndex = 8
        ' 
        ' colNom
        ' 
        colNom.HeaderText = "Nom"
        colNom.Name = "colNom"
        colNom.ReadOnly = True
        colNom.Width = 180
        ' 
        ' colPrenom
        ' 
        colPrenom.HeaderText = "Prénom"
        colPrenom.Name = "colPrenom"
        colPrenom.ReadOnly = True
        colPrenom.Width = 150
        ' 
        ' colSpecialite
        ' 
        colSpecialite.HeaderText = "Spécialité"
        colSpecialite.Name = "colSpecialite"
        colSpecialite.ReadOnly = True
        colSpecialite.Width = 180
        ' 
        ' colTelephone
        ' 
        colTelephone.HeaderText = "Téléphone"
        colTelephone.Name = "colTelephone"
        colTelephone.ReadOnly = True
        colTelephone.Width = 130
        ' 
        ' colEmail
        ' 
        colEmail.HeaderText = "E-mail"
        colEmail.Name = "colEmail"
        colEmail.ReadOnly = True
        colEmail.Width = 180
        ' 
        ' colLocalite
        ' 
        colLocalite.HeaderText = "Localité"
        colLocalite.Name = "colLocalite"
        colLocalite.ReadOnly = True
        colLocalite.Width = 130
        ' 
        ' colActif
        ' 
        colActif.HeaderText = "Actif"
        colActif.Name = "colActif"
        colActif.ReadOnly = True
        colActif.Width = 50
        ' 
        ' colIdTherapeute
        ' 
        colIdTherapeute.HeaderText = "Id thérapeute"
        colIdTherapeute.Name = "colIdTherapeute"
        colIdTherapeute.ReadOnly = True
        colIdTherapeute.Visible = False
        ' 
        ' colCodeTherapeute
        ' 
        colCodeTherapeute.HeaderText = "Code thérapeute"
        colCodeTherapeute.Name = "colCodeTherapeute"
        colCodeTherapeute.ReadOnly = True
        colCodeTherapeute.Visible = False
        ' 
        ' pnlTop
        ' 
        pnlTop.Controls.Add(btnRechercher)
        pnlTop.Controls.Add(btnReinitialiserFiltres)
        pnlTop.Controls.Add(chkAfficherInactifs)
        pnlTop.Controls.Add(lblRecherche)
        pnlTop.Controls.Add(txtRecherche)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(8)
        pnlTop.Size = New Size(933, 64)
        pnlTop.TabIndex = 7
        ' 
        ' btnRechercher
        ' 
        btnRechercher.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnRechercher.FlatAppearance.BorderSize = 0
        btnRechercher.FlatStyle = FlatStyle.Flat
        btnRechercher.Font = New Font("Calibri", 10F)
        btnRechercher.ForeColor = Color.White
        btnRechercher.Image = CType(resources.GetObject("btnRechercher.Image"), Image)
        btnRechercher.ImageAlign = ContentAlignment.MiddleLeft
        btnRechercher.Location = New Point(470, 20)
        btnRechercher.Name = "btnRechercher"
        btnRechercher.Size = New Size(120, 32)
        btnRechercher.TabIndex = 3
        btnRechercher.Text = "Rechercher"
        btnRechercher.TextAlign = ContentAlignment.MiddleLeft
        btnRechercher.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRechercher.UseVisualStyleBackColor = False
        ' 
        ' btnReinitialiserFiltres
        ' 
        btnReinitialiserFiltres.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnReinitialiserFiltres.FlatAppearance.BorderSize = 0
        btnReinitialiserFiltres.FlatStyle = FlatStyle.Flat
        btnReinitialiserFiltres.Font = New Font("Calibri", 10F)
        btnReinitialiserFiltres.ForeColor = Color.White
        btnReinitialiserFiltres.Image = CType(resources.GetObject("btnReinitialiserFiltres.Image"), Image)
        btnReinitialiserFiltres.ImageAlign = ContentAlignment.MiddleLeft
        btnReinitialiserFiltres.Location = New Point(600, 20)
        btnReinitialiserFiltres.Name = "btnReinitialiserFiltres"
        btnReinitialiserFiltres.Size = New Size(120, 32)
        btnReinitialiserFiltres.TabIndex = 4
        btnReinitialiserFiltres.Tag = "reinitialiser_24_normal"
        btnReinitialiserFiltres.Text = "Réinitialiser"
        btnReinitialiserFiltres.TextAlign = ContentAlignment.MiddleLeft
        btnReinitialiserFiltres.TextImageRelation = TextImageRelation.ImageBeforeText
        btnReinitialiserFiltres.UseVisualStyleBackColor = False
        ' 
        ' chkAfficherInactifs
        ' 
        chkAfficherInactifs.AutoSize = True
        chkAfficherInactifs.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkAfficherInactifs.Location = New Point(745, 25)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(122, 22)
        chkAfficherInactifs.TabIndex = 5
        chkAfficherInactifs.Text = "Afficher inactifs"
        chkAfficherInactifs.TextAlign = ContentAlignment.MiddleCenter
        chkAfficherInactifs.UseVisualStyleBackColor = True
        ' 
        ' lblRecherche
        ' 
        lblRecherche.AutoSize = True
        lblRecherche.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRecherche.Location = New Point(25, 8)
        lblRecherche.Name = "lblRecherche"
        lblRecherche.Size = New Size(261, 18)
        lblRecherche.TabIndex = 0
        lblRecherche.Text = "Recherche sur nom + prénom + spécialité"
        ' 
        ' txtRecherche
        ' 
        txtRecherche.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtRecherche.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtRecherche.Location = New Point(25, 28)
        txtRecherche.Name = "txtRecherche"
        txtRecherche.Size = New Size(425, 25)
        txtRecherche.TabIndex = 2
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.FromArgb(CByte(231), CByte(223), CByte(214))
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.Fixed3D
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Controls.Add(btnActiverDesactiver)
        pnlActions.Controls.Add(btnSupprimer)
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Padding = New Padding(8)
        pnlActions.Size = New Size(965, 57)
        pnlActions.TabIndex = 20
        ' 
        ' btnNouveau
        ' 
        btnNouveau.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnNouveau.FlatAppearance.BorderSize = 0
        btnNouveau.FlatStyle = FlatStyle.Flat
        btnNouveau.Font = New Font("Calibri", 10F)
        btnNouveau.ForeColor = Color.White
        btnNouveau.Image = CType(resources.GetObject("btnNouveau.Image"), Image)
        btnNouveau.ImageAlign = ContentAlignment.MiddleLeft
        btnNouveau.Location = New Point(122, 8)
        btnNouveau.Name = "btnNouveau"
        btnNouveau.Size = New Size(112, 40)
        btnNouveau.TabIndex = 10
        btnNouveau.Tag = "nouveau_normal"
        btnNouveau.Text = "Nouveau"
        btnNouveau.TextAlign = ContentAlignment.MiddleLeft
        btnNouveau.TextImageRelation = TextImageRelation.ImageBeforeText
        btnNouveau.UseVisualStyleBackColor = False
        ' 
        ' btnModifier
        ' 
        btnModifier.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnModifier.FlatAppearance.BorderSize = 0
        btnModifier.FlatStyle = FlatStyle.Flat
        btnModifier.Font = New Font("Calibri", 10F)
        btnModifier.ForeColor = Color.White
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.ImageAlign = ContentAlignment.MiddleLeft
        btnModifier.Location = New Point(240, 8)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(112, 40)
        btnModifier.TabIndex = 11
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        btnModifier.TextAlign = ContentAlignment.MiddleLeft
        btnModifier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifier.UseVisualStyleBackColor = False
        ' 
        ' btnActiverDesactiver
        ' 
        btnActiverDesactiver.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnActiverDesactiver.FlatAppearance.BorderSize = 0
        btnActiverDesactiver.FlatStyle = FlatStyle.Flat
        btnActiverDesactiver.Font = New Font("Calibri", 10F)
        btnActiverDesactiver.ForeColor = Color.White
        btnActiverDesactiver.Image = CType(resources.GetObject("btnActiverDesactiver.Image"), Image)
        btnActiverDesactiver.ImageAlign = ContentAlignment.MiddleLeft
        btnActiverDesactiver.Location = New Point(358, 8)
        btnActiverDesactiver.Name = "btnActiverDesactiver"
        btnActiverDesactiver.Size = New Size(156, 40)
        btnActiverDesactiver.TabIndex = 12
        btnActiverDesactiver.Tag = "activer_normal"
        btnActiverDesactiver.Text = "Activer/Désactiver"
        btnActiverDesactiver.TextAlign = ContentAlignment.MiddleLeft
        btnActiverDesactiver.TextImageRelation = TextImageRelation.ImageBeforeText
        btnActiverDesactiver.UseVisualStyleBackColor = False
        ' 
        ' btnSupprimer
        ' 
        btnSupprimer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnSupprimer.FlatAppearance.BorderSize = 0
        btnSupprimer.FlatStyle = FlatStyle.Flat
        btnSupprimer.Font = New Font("Calibri", 10F)
        btnSupprimer.ForeColor = Color.White
        btnSupprimer.Image = CType(resources.GetObject("btnSupprimer.Image"), Image)
        btnSupprimer.ImageAlign = ContentAlignment.MiddleLeft
        btnSupprimer.Location = New Point(520, 8)
        btnSupprimer.Name = "btnSupprimer"
        btnSupprimer.Size = New Size(112, 40)
        btnSupprimer.TabIndex = 13
        btnSupprimer.Tag = "supprimer_normal"
        btnSupprimer.Text = "Supprimer"
        btnSupprimer.TextAlign = ContentAlignment.MiddleLeft
        btnSupprimer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnSupprimer.UseVisualStyleBackColor = False
        ' 
        ' btnActualiser
        ' 
        btnActualiser.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnActualiser.FlatAppearance.BorderSize = 0
        btnActualiser.FlatStyle = FlatStyle.Flat
        btnActualiser.Font = New Font("Calibri", 10F)
        btnActualiser.ForeColor = Color.White
        btnActualiser.Image = CType(resources.GetObject("btnActualiser.Image"), Image)
        btnActualiser.ImageAlign = ContentAlignment.MiddleLeft
        btnActualiser.Location = New Point(638, 8)
        btnActualiser.Name = "btnActualiser"
        btnActualiser.Size = New Size(112, 40)
        btnActualiser.TabIndex = 14
        btnActualiser.Tag = "actualiser_normal"
        btnActualiser.Text = "Actualiser"
        btnActualiser.TextAlign = ContentAlignment.MiddleLeft
        btnActualiser.TextImageRelation = TextImageRelation.ImageBeforeText
        btnActualiser.UseVisualStyleBackColor = False
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTop)
        pnlTitre.Controls.Add(picTitre)
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(16, 16)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(965, 58)
        pnlTitre.TabIndex = 22
        ' 
        ' lblTop
        ' 
        lblTop.Font = New Font("Calibri", 11F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(402, 14)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(533, 26)
        lblTop.TabIndex = 4
        lblTop.Text = "Consultez et gérez les intervenants et thérapeutes"
        ' 
        ' picTitre
        ' 
        picTitre.BackgroundImage = CType(resources.GetObject("picTitre.BackgroundImage"), Image)
        picTitre.BackgroundImageLayout = ImageLayout.Stretch
        picTitre.ErrorImage = My.Resources.Resources.Fond_icone_Transp
        picTitre.Location = New Point(16, 0)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(60, 52)
        picTitre.TabIndex = 3
        picTitre.TabStop = False
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.AutoSize = True
        lblTitreForm.Font = New Font("Calibri", 18F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(73, 11)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Size = New Size(291, 29)
        lblTitreForm.TabIndex = 0
        lblTitreForm.Text = "Référentiel des thérapeutes"
        ' 
        ' UC_Therapeutes
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Name = "UC_Therapeutes"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        CType(dgvTherapeutes, ComponentModel.ISupportInitialize).EndInit()
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        pnlActions.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents dgvTherapeutes As DataGridView
    Friend WithEvents colNom As DataGridViewTextBoxColumn
    Friend WithEvents colPrenom As DataGridViewTextBoxColumn
    Friend WithEvents colSpecialite As DataGridViewTextBoxColumn
    Friend WithEvents colTelephone As DataGridViewTextBoxColumn
    Friend WithEvents colEmail As DataGridViewTextBoxColumn
    Friend WithEvents colLocalite As DataGridViewTextBoxColumn
    Friend WithEvents colActif As DataGridViewCheckBoxColumn
    Friend WithEvents colIdTherapeute As DataGridViewTextBoxColumn
    Friend WithEvents colCodeTherapeute As DataGridViewTextBoxColumn
    Friend WithEvents pnlTop As Panel
    Friend WithEvents btnRechercher As Button
    Friend WithEvents btnReinitialiserFiltres As Button
    Friend WithEvents chkAfficherInactifs As CheckBox
    Friend WithEvents lblRecherche As Label
    Friend WithEvents txtRecherche As TextBox
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnNouveau As Button
    Friend WithEvents btnModifier As Button
    Friend WithEvents btnActiverDesactiver As Button
    Friend WithEvents btnSupprimer As Button
    Friend WithEvents btnActualiser As Button
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents picTitre As PictureBox
    Friend WithEvents lblTop As Label

End Class
