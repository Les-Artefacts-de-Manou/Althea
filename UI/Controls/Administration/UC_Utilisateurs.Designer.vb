<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_Utilisateurs
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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Utilisateurs))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        dgvUtilisateurs = New DataGridView()
        colEtat = New DataGridViewImageColumn()
        colLoginUtilisateur = New DataGridViewTextBoxColumn()
        colNomAffichage = New DataGridViewTextBoxColumn()
        colRoleUtilisateur = New DataGridViewTextBoxColumn()
        colDernierLogin = New DataGridViewTextBoxColumn()
        colIdUtilisateur = New DataGridViewTextBoxColumn()
        colCodeUtilisateur = New DataGridViewTextBoxColumn()
        colRoleMaxElevation = New DataGridViewTextBoxColumn()
        colActif = New DataGridViewCheckBoxColumn()
        colMustChangePassword = New DataGridViewCheckBoxColumn()
        colNbEchecsLogin = New DataGridViewTextBoxColumn()
        colCompteVerrouille = New DataGridViewCheckBoxColumn()
        colDateVerrouillage = New DataGridViewTextBoxColumn()
        colDateCreation = New DataGridViewTextBoxColumn()
        colDateModification = New DataGridViewTextBoxColumn()
        pnlTop = New Panel()
        btnRechercher = New Button()
        btnReinitialiserFiltres = New Button()
        chkFiltrerDate = New CheckBox()
        chkAfficherInactifs = New CheckBox()
        chkCompteVerrouille = New CheckBox()
        lblFiltreRole = New Label()
        dtpDernierLoginDepuis = New DateTimePicker()
        cboFiltreRole = New ComboBox()
        lblRecherche = New Label()
        txtRechercheUtilisateur = New TextBox()
        pnlActions = New Panel()
        btnActiverDesactiver = New Button()
        btnActualiser = New Button()
        btnNouveau = New Button()
        btnModifier = New Button()
        pnlTitre = New Panel()
        lblTop = New Label()
        lblTitreForm = New Label()
        picTitre = New PictureBox()
        errProvider = New ErrorProvider(components)
        ttMain = New ToolTip(components)
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        CType(dgvUtilisateurs, ComponentModel.ISupportInitialize).BeginInit()
        pnlTop.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTitre.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackgroundImage = CType(resources.GetObject("pnlForm.BackgroundImage"), Image)
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
        pnlCenter.Controls.Add(dgvUtilisateurs)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(965, 624)
        pnlCenter.TabIndex = 21
        ' 
        ' dgvUtilisateurs
        ' 
        dgvUtilisateurs.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvUtilisateurs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvUtilisateurs.Columns.AddRange(New DataGridViewColumn() {colEtat, colLoginUtilisateur, colNomAffichage, colRoleUtilisateur, colDernierLogin, colIdUtilisateur, colCodeUtilisateur, colRoleMaxElevation, colActif, colMustChangePassword, colNbEchecsLogin, colCompteVerrouille, colDateVerrouillage, colDateCreation, colDateModification})
        dgvUtilisateurs.Dock = DockStyle.Fill
        dgvUtilisateurs.Location = New Point(16, 128)
        dgvUtilisateurs.Name = "dgvUtilisateurs"
        dgvUtilisateurs.ReadOnly = True
        dgvUtilisateurs.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvUtilisateurs.Size = New Size(933, 488)
        dgvUtilisateurs.TabIndex = 8
        ' 
        ' colEtat
        ' 
        colEtat.HeaderText = "Etat"
        colEtat.Name = "colEtat"
        colEtat.ReadOnly = True
        colEtat.Width = 50
        ' 
        ' colLoginUtilisateur
        ' 
        colLoginUtilisateur.HeaderText = "Login"
        colLoginUtilisateur.Name = "colLoginUtilisateur"
        colLoginUtilisateur.ReadOnly = True
        colLoginUtilisateur.Width = 230
        ' 
        ' colNomAffichage
        ' 
        colNomAffichage.HeaderText = "Nom affiché"
        colNomAffichage.Name = "colNomAffichage"
        colNomAffichage.ReadOnly = True
        colNomAffichage.Width = 280
        ' 
        ' colRoleUtilisateur
        ' 
        colRoleUtilisateur.HeaderText = "Rôle"
        colRoleUtilisateur.Name = "colRoleUtilisateur"
        colRoleUtilisateur.ReadOnly = True
        colRoleUtilisateur.Width = 170
        ' 
        ' colDernierLogin
        ' 
        colDernierLogin.HeaderText = "Dernier Login"
        colDernierLogin.Name = "colDernierLogin"
        colDernierLogin.ReadOnly = True
        colDernierLogin.Width = 130
        ' 
        ' colIdUtilisateur
        ' 
        colIdUtilisateur.HeaderText = "Id utilisateur"
        colIdUtilisateur.Name = "colIdUtilisateur"
        colIdUtilisateur.ReadOnly = True
        colIdUtilisateur.Visible = False
        ' 
        ' colCodeUtilisateur
        ' 
        colCodeUtilisateur.HeaderText = "Code utilisateur"
        colCodeUtilisateur.Name = "colCodeUtilisateur"
        colCodeUtilisateur.ReadOnly = True
        colCodeUtilisateur.Visible = False
        ' 
        ' colRoleMaxElevation
        ' 
        colRoleMaxElevation.HeaderText = "Rôle max élévation"
        colRoleMaxElevation.Name = "colRoleMaxElevation"
        colRoleMaxElevation.ReadOnly = True
        colRoleMaxElevation.Visible = False
        ' 
        ' colActif
        ' 
        colActif.HeaderText = "Actif"
        colActif.Name = "colActif"
        colActif.ReadOnly = True
        colActif.Visible = False
        ' 
        ' colMustChangePassword
        ' 
        colMustChangePassword.HeaderText = "Must change PW"
        colMustChangePassword.Name = "colMustChangePassword"
        colMustChangePassword.ReadOnly = True
        colMustChangePassword.Visible = False
        ' 
        ' colNbEchecsLogin
        ' 
        colNbEchecsLogin.HeaderText = "Nb échecs"
        colNbEchecsLogin.Name = "colNbEchecsLogin"
        colNbEchecsLogin.ReadOnly = True
        colNbEchecsLogin.Visible = False
        ' 
        ' colCompteVerrouille
        ' 
        colCompteVerrouille.HeaderText = "Compte verrouillé"
        colCompteVerrouille.Name = "colCompteVerrouille"
        colCompteVerrouille.ReadOnly = True
        colCompteVerrouille.Visible = False
        ' 
        ' colDateVerrouillage
        ' 
        colDateVerrouillage.HeaderText = "Date verrouillage"
        colDateVerrouillage.Name = "colDateVerrouillage"
        colDateVerrouillage.ReadOnly = True
        colDateVerrouillage.Visible = False
        ' 
        ' colDateCreation
        ' 
        colDateCreation.HeaderText = "Date création"
        colDateCreation.Name = "colDateCreation"
        colDateCreation.ReadOnly = True
        colDateCreation.Visible = False
        ' 
        ' colDateModification
        ' 
        colDateModification.HeaderText = "Date modification"
        colDateModification.Name = "colDateModification"
        colDateModification.ReadOnly = True
        colDateModification.Visible = False
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.Transparent
        pnlTop.Controls.Add(btnRechercher)
        pnlTop.Controls.Add(btnReinitialiserFiltres)
        pnlTop.Controls.Add(chkFiltrerDate)
        pnlTop.Controls.Add(chkAfficherInactifs)
        pnlTop.Controls.Add(chkCompteVerrouille)
        pnlTop.Controls.Add(lblFiltreRole)
        pnlTop.Controls.Add(dtpDernierLoginDepuis)
        pnlTop.Controls.Add(cboFiltreRole)
        pnlTop.Controls.Add(lblRecherche)
        pnlTop.Controls.Add(txtRechercheUtilisateur)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(8)
        pnlTop.Size = New Size(933, 112)
        pnlTop.TabIndex = 7
        ' 
        ' btnRechercher
        ' 
        btnRechercher.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnRechercher.FlatAppearance.BorderSize = 0
        btnRechercher.FlatStyle = FlatStyle.Flat
        btnRechercher.ForeColor = Color.White
        btnRechercher.Image = CType(resources.GetObject("btnRechercher.Image"), Image)
        btnRechercher.ImageAlign = ContentAlignment.MiddleLeft
        btnRechercher.Location = New Point(25, 68)
        btnRechercher.Name = "btnRechercher"
        btnRechercher.Size = New Size(120, 38)
        btnRechercher.TabIndex = 19
        btnRechercher.Tag = "rechercherUser_normal"
        btnRechercher.Text = "Rechercher"
        btnRechercher.TextAlign = ContentAlignment.MiddleRight
        btnRechercher.UseVisualStyleBackColor = False
        ' 
        ' btnReinitialiserFiltres
        ' 
        btnReinitialiserFiltres.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnReinitialiserFiltres.FlatAppearance.BorderSize = 0
        btnReinitialiserFiltres.FlatStyle = FlatStyle.Flat
        btnReinitialiserFiltres.ForeColor = Color.White
        btnReinitialiserFiltres.Image = CType(resources.GetObject("btnReinitialiserFiltres.Image"), Image)
        btnReinitialiserFiltres.ImageAlign = ContentAlignment.MiddleLeft
        btnReinitialiserFiltres.Location = New Point(155, 68)
        btnReinitialiserFiltres.Name = "btnReinitialiserFiltres"
        btnReinitialiserFiltres.Size = New Size(120, 38)
        btnReinitialiserFiltres.TabIndex = 21
        btnReinitialiserFiltres.Tag = "resetText_normal"
        btnReinitialiserFiltres.Text = "Réinitialiser"
        btnReinitialiserFiltres.TextAlign = ContentAlignment.MiddleRight
        btnReinitialiserFiltres.UseVisualStyleBackColor = False
        ' 
        ' chkFiltrerDate
        ' 
        chkFiltrerDate.AutoSize = True
        chkFiltrerDate.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkFiltrerDate.Location = New Point(545, 11)
        chkFiltrerDate.Name = "chkFiltrerDate"
        chkFiltrerDate.Size = New Size(172, 22)
        chkFiltrerDate.TabIndex = 20
        chkFiltrerDate.Text = "Filtrer par date de login"
        chkFiltrerDate.TextAlign = ContentAlignment.MiddleCenter
        chkFiltrerDate.UseVisualStyleBackColor = True
        ' 
        ' chkAfficherInactifs
        ' 
        chkAfficherInactifs.AutoSize = True
        chkAfficherInactifs.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkAfficherInactifs.Location = New Point(762, 41)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(122, 22)
        chkAfficherInactifs.TabIndex = 17
        chkAfficherInactifs.Text = "Afficher inactifs"
        chkAfficherInactifs.TextAlign = ContentAlignment.MiddleCenter
        chkAfficherInactifs.UseVisualStyleBackColor = True
        ' 
        ' chkCompteVerrouille
        ' 
        chkCompteVerrouille.AutoSize = True
        chkCompteVerrouille.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkCompteVerrouille.Location = New Point(762, 13)
        chkCompteVerrouille.Name = "chkCompteVerrouille"
        chkCompteVerrouille.Size = New Size(152, 22)
        chkCompteVerrouille.TabIndex = 18
        chkCompteVerrouille.Text = "Comptes verrouillés"
        chkCompteVerrouille.TextAlign = ContentAlignment.MiddleCenter
        chkCompteVerrouille.UseVisualStyleBackColor = True
        ' 
        ' lblFiltreRole
        ' 
        lblFiltreRole.AutoSize = True
        lblFiltreRole.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblFiltreRole.Location = New Point(369, 13)
        lblFiltreRole.Name = "lblFiltreRole"
        lblFiltreRole.Size = New Size(69, 18)
        lblFiltreRole.TabIndex = 15
        lblFiltreRole.Text = "Filtre rôle"
        ' 
        ' dtpDernierLoginDepuis
        ' 
        dtpDernierLoginDepuis.CalendarForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        dtpDernierLoginDepuis.Location = New Point(545, 38)
        dtpDernierLoginDepuis.Name = "dtpDernierLoginDepuis"
        dtpDernierLoginDepuis.Size = New Size(200, 25)
        dtpDernierLoginDepuis.TabIndex = 14
        ' 
        ' cboFiltreRole
        ' 
        cboFiltreRole.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboFiltreRole.DropDownStyle = ComboBoxStyle.DropDownList
        cboFiltreRole.FlatStyle = FlatStyle.Flat
        cboFiltreRole.Font = New Font("Calibri", 9F)
        cboFiltreRole.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboFiltreRole.FormattingEnabled = True
        cboFiltreRole.Location = New Point(375, 41)
        cboFiltreRole.Name = "cboFiltreRole"
        cboFiltreRole.Size = New Size(153, 22)
        cboFiltreRole.TabIndex = 13
        ' 
        ' lblRecherche
        ' 
        lblRecherche.AutoSize = True
        lblRecherche.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRecherche.Location = New Point(25, 13)
        lblRecherche.Name = "lblRecherche"
        lblRecherche.Size = New Size(214, 18)
        lblRecherche.TabIndex = 12
        lblRecherche.Text = "Recherche sur login + nom affiché"
        ' 
        ' txtRechercheUtilisateur
        ' 
        txtRechercheUtilisateur.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtRechercheUtilisateur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtRechercheUtilisateur.Location = New Point(23, 38)
        txtRechercheUtilisateur.Name = "txtRechercheUtilisateur"
        txtRechercheUtilisateur.Size = New Size(335, 25)
        txtRechercheUtilisateur.TabIndex = 2
        ' 
        ' pnlActions
        ' 
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.Fixed3D
        pnlActions.Controls.Add(btnActiverDesactiver)
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Padding = New Padding(8)
        pnlActions.Size = New Size(965, 57)
        pnlActions.TabIndex = 20
        ' 
        ' btnActiverDesactiver
        ' 
        btnActiverDesactiver.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnActiverDesactiver.BackgroundImageLayout = ImageLayout.Center
        btnActiverDesactiver.FlatAppearance.BorderSize = 0
        btnActiverDesactiver.FlatStyle = FlatStyle.Flat
        btnActiverDesactiver.ForeColor = Color.White
        btnActiverDesactiver.Image = CType(resources.GetObject("btnActiverDesactiver.Image"), Image)
        btnActiverDesactiver.ImageAlign = ContentAlignment.MiddleLeft
        btnActiverDesactiver.Location = New Point(484, 6)
        btnActiverDesactiver.Name = "btnActiverDesactiver"
        btnActiverDesactiver.Size = New Size(112, 40)
        btnActiverDesactiver.TabIndex = 13
        btnActiverDesactiver.Tag = "activer_normal"
        btnActiverDesactiver.Text = "Désactiver"
        btnActiverDesactiver.TextAlign = ContentAlignment.MiddleLeft
        btnActiverDesactiver.TextImageRelation = TextImageRelation.ImageBeforeText
        btnActiverDesactiver.UseVisualStyleBackColor = False
        ' 
        ' btnActualiser
        ' 
        btnActualiser.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnActualiser.BackgroundImageLayout = ImageLayout.Center
        btnActualiser.FlatAppearance.BorderSize = 0
        btnActualiser.FlatStyle = FlatStyle.Flat
        btnActualiser.ForeColor = Color.White
        btnActualiser.Image = CType(resources.GetObject("btnActualiser.Image"), Image)
        btnActualiser.ImageAlign = ContentAlignment.MiddleLeft
        btnActualiser.Location = New Point(602, 6)
        btnActualiser.Name = "btnActualiser"
        btnActualiser.Size = New Size(112, 40)
        btnActualiser.TabIndex = 11
        btnActualiser.Tag = "actualiser_normal"
        btnActualiser.Text = "Actualiser"
        btnActualiser.TextAlign = ContentAlignment.MiddleLeft
        btnActualiser.TextImageRelation = TextImageRelation.ImageBeforeText
        btnActualiser.UseVisualStyleBackColor = False
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
        btnNouveau.Location = New Point(247, 6)
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
        btnModifier.Location = New Point(366, 6)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(112, 40)
        btnModifier.TabIndex = 7
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        btnModifier.TextAlign = ContentAlignment.MiddleLeft
        btnModifier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifier.UseVisualStyleBackColor = False
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
        pnlTitre.TabIndex = 5
        ' 
        ' lblTop
        ' 
        lblTop.Font = New Font("Calibri", 11F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(228, 16)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(533, 26)
        lblTop.TabIndex = 3
        lblTop.Text = "Consultez et gérez les comptes utilisateurs, leurs rôles et leurs états de sécurité."
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.Anchor = AnchorStyles.Left
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 18F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(73, 7)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(0, 4, 8, 4)
        lblTitreForm.Size = New Size(137, 37)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Utilisateurs"
        ' 
        ' picTitre
        ' 
        picTitre.BackgroundImage = CType(resources.GetObject("picTitre.BackgroundImage"), Image)
        picTitre.BackgroundImageLayout = ImageLayout.Stretch
        picTitre.ErrorImage = My.Resources.Resources.Fond_icone_Transp
        picTitre.Location = New Point(14, 3)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(60, 52)
        picTitre.TabIndex = 2
        picTitre.TabStop = False
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' UC_Utilisateurs
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_Utilisateurs"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        CType(dgvUtilisateurs, ComponentModel.ISupportInitialize).EndInit()
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        pnlActions.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).EndInit()
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents picTitre As PictureBox
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnNouveau As Button
    Friend WithEvents btnModifier As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents btnActiverDesactiver As Button
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents pnlTop As Panel
    Friend WithEvents dgvUtilisateurs As DataGridView
    Friend WithEvents txtRechercheUtilisateur As TextBox
    Friend WithEvents lblTop As Label
    Friend WithEvents lblRecherche As Label
    Friend WithEvents cboFiltreRole As ComboBox
    Friend WithEvents lblFiltreRole As Label
    Friend WithEvents dtpDernierLoginDepuis As DateTimePicker
    Friend WithEvents chkAfficherInactifs As CheckBox
    Friend WithEvents chkCompteVerrouille As CheckBox
    Friend WithEvents chkFiltrerDate As CheckBox
    Friend WithEvents btnRechercher As Button
    Friend WithEvents btnReinitialiserFiltres As Button
    Friend WithEvents btnActualiser As Button
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents colEtat As DataGridViewImageColumn
    Friend WithEvents colLoginUtilisateur As DataGridViewTextBoxColumn
    Friend WithEvents colNomAffichage As DataGridViewTextBoxColumn
    Friend WithEvents colRoleUtilisateur As DataGridViewTextBoxColumn
    Friend WithEvents colDernierLogin As DataGridViewTextBoxColumn
    Friend WithEvents colIdUtilisateur As DataGridViewTextBoxColumn
    Friend WithEvents colCodeUtilisateur As DataGridViewTextBoxColumn
    Friend WithEvents colRoleMaxElevation As DataGridViewTextBoxColumn
    Friend WithEvents colActif As DataGridViewCheckBoxColumn
    Friend WithEvents colMustChangePassword As DataGridViewCheckBoxColumn
    Friend WithEvents colNbEchecsLogin As DataGridViewTextBoxColumn
    Friend WithEvents colCompteVerrouille As DataGridViewCheckBoxColumn
    Friend WithEvents colDateVerrouillage As DataGridViewTextBoxColumn
    Friend WithEvents colDateCreation As DataGridViewTextBoxColumn
    Friend WithEvents colDateModification As DataGridViewTextBoxColumn

End Class
