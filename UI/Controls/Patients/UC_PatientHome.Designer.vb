<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_PatientHome
    Inherits System.Windows.Forms.UserControl

    'UserControl remplace la méthode Dispose pour nettoyer la liste des composants.
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
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_PatientHome))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        dgvPatients = New DataGridView()
        colStatutSuivi = New DataGridViewImageColumn()
        colCodePatient = New DataGridViewTextBoxColumn()
        colNom = New DataGridViewTextBoxColumn()
        colPrenom = New DataGridViewTextBoxColumn()
        colDateNaissance = New DataGridViewTextBoxColumn()
        colNiss = New DataGridViewTextBoxColumn()
        colTelephone = New DataGridViewTextBoxColumn()
        colEmail = New DataGridViewTextBoxColumn()
        colAlerte = New DataGridViewCheckBoxColumn()
        colPhoto = New DataGridViewCheckBoxColumn()
        colDateModification = New DataGridViewTextBoxColumn()
        colIdPatient = New DataGridViewTextBoxColumn()
        pnlTop = New Panel()
        btnRechercher = New Button()
        btnReinitialiserFiltres = New Button()
        cboFiltreSuivi = New ComboBox()
        lblRecherche = New Label()
        txtRecherchePatient = New TextBox()
        pnlActions = New Panel()
        btnActualiser = New Button()
        btnOuvrir = New Button()
        btnNouveau = New Button()
        btnModifier = New Button()
        pnlTitre = New Panel()
        picTitre = New PictureBox()
        lblTop = New Label()
        lblTitreForm = New Label()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        CType(dgvPatients, ComponentModel.ISupportInitialize).BeginInit()
        pnlTop.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTitre.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
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
        pnlCenter.Controls.Add(dgvPatients)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(965, 624)
        pnlCenter.TabIndex = 21
        ' 
        ' dgvPatients
        ' 
        dgvPatients.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvPatients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvPatients.Columns.AddRange(New DataGridViewColumn() {colStatutSuivi, colCodePatient, colNom, colPrenom, colDateNaissance, colNiss, colTelephone, colEmail, colAlerte, colPhoto, colDateModification, colIdPatient})
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = SystemColors.Window
        DataGridViewCellStyle1.Font = New Font("Calibri", 9F)
        DataGridViewCellStyle1.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        DataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.False
        dgvPatients.DefaultCellStyle = DataGridViewCellStyle1
        dgvPatients.Dock = DockStyle.Fill
        dgvPatients.Location = New Point(16, 86)
        dgvPatients.Name = "dgvPatients"
        dgvPatients.ReadOnly = True
        dgvPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPatients.Size = New Size(933, 530)
        dgvPatients.TabIndex = 8
        ' 
        ' colStatutSuivi
        ' 
        colStatutSuivi.HeaderText = "Suivi"
        colStatutSuivi.Name = "colStatutSuivi"
        colStatutSuivi.ReadOnly = True
        colStatutSuivi.Resizable = DataGridViewTriState.False
        colStatutSuivi.Width = 50
        ' 
        ' colCodePatient
        ' 
        colCodePatient.HeaderText = "Code"
        colCodePatient.Name = "colCodePatient"
        colCodePatient.ReadOnly = True
        colCodePatient.Width = 90
        ' 
        ' colNom
        ' 
        colNom.HeaderText = "Nom"
        colNom.Name = "colNom"
        colNom.ReadOnly = True
        colNom.Width = 160
        ' 
        ' colPrenom
        ' 
        colPrenom.HeaderText = "Prénom"
        colPrenom.Name = "colPrenom"
        colPrenom.ReadOnly = True
        colPrenom.Width = 140
        ' 
        ' colDateNaissance
        ' 
        colDateNaissance.HeaderText = "Naissance"
        colDateNaissance.Name = "colDateNaissance"
        colDateNaissance.ReadOnly = True
        ' 
        ' colNiss
        ' 
        colNiss.HeaderText = "NISS"
        colNiss.Name = "colNiss"
        colNiss.ReadOnly = True
        colNiss.Width = 130
        ' 
        ' colTelephone
        ' 
        colTelephone.HeaderText = "Téléphone"
        colTelephone.Name = "colTelephone"
        colTelephone.ReadOnly = True
        colTelephone.Width = 120
        ' 
        ' colEmail
        ' 
        colEmail.HeaderText = "Email"
        colEmail.Name = "colEmail"
        colEmail.ReadOnly = True
        colEmail.Width = 190
        ' 
        ' colAlerte
        ' 
        colAlerte.HeaderText = "Alerte"
        colAlerte.Name = "colAlerte"
        colAlerte.ReadOnly = True
        colAlerte.Width = 55
        ' 
        ' colPhoto
        ' 
        colPhoto.HeaderText = "Photo"
        colPhoto.Name = "colPhoto"
        colPhoto.ReadOnly = True
        colPhoto.Width = 55
        ' 
        ' colDateModification
        ' 
        colDateModification.HeaderText = "Modifié le"
        colDateModification.Name = "colDateModification"
        colDateModification.ReadOnly = True
        colDateModification.Width = 130
        ' 
        ' colIdPatient
        ' 
        colIdPatient.HeaderText = "Id patient"
        colIdPatient.Name = "colIdPatient"
        colIdPatient.ReadOnly = True
        colIdPatient.Visible = False
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.Transparent
        pnlTop.Controls.Add(btnRechercher)
        pnlTop.Controls.Add(btnReinitialiserFiltres)
        pnlTop.Controls.Add(cboFiltreSuivi)
        pnlTop.Controls.Add(lblRecherche)
        pnlTop.Controls.Add(txtRecherchePatient)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(8)
        pnlTop.Size = New Size(933, 70)
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
        btnRechercher.Tag = ""
        btnRechercher.Text = "Rechercher"
        btnRechercher.TextAlign = ContentAlignment.MiddleRight
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
        btnReinitialiserFiltres.Location = New Point(596, 20)
        btnReinitialiserFiltres.Name = "btnReinitialiserFiltres"
        btnReinitialiserFiltres.Size = New Size(120, 32)
        btnReinitialiserFiltres.TabIndex = 4
        btnReinitialiserFiltres.Tag = "reinitialiser_24_normal"
        btnReinitialiserFiltres.Text = "Réinitialiser"
        btnReinitialiserFiltres.TextAlign = ContentAlignment.MiddleRight
        btnReinitialiserFiltres.TextImageRelation = TextImageRelation.ImageBeforeText
        btnReinitialiserFiltres.UseVisualStyleBackColor = False
        ' 
        ' cboFiltreSuivi
        ' 
        cboFiltreSuivi.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboFiltreSuivi.DropDownStyle = ComboBoxStyle.DropDownList
        cboFiltreSuivi.FlatStyle = FlatStyle.Flat
        cboFiltreSuivi.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        cboFiltreSuivi.Location = New Point(731, 26)
        cboFiltreSuivi.Name = "cboFiltreSuivi"
        cboFiltreSuivi.Size = New Size(180, 26)
        cboFiltreSuivi.TabIndex = 5
        ' 
        ' lblRecherche
        ' 
        lblRecherche.AutoSize = True
        lblRecherche.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRecherche.Location = New Point(25, 8)
        lblRecherche.Name = "lblRecherche"
        lblRecherche.Size = New Size(294, 18)
        lblRecherche.TabIndex = 1
        lblRecherche.Text = "Recherche (nom, prénom, NISS, code, contact)"
        ' 
        ' txtRecherchePatient
        ' 
        txtRecherchePatient.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtRecherchePatient.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtRecherchePatient.Location = New Point(25, 30)
        txtRecherchePatient.Name = "txtRecherchePatient"
        txtRecherchePatient.Size = New Size(430, 25)
        txtRecherchePatient.TabIndex = 2
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.FromArgb(CByte(231), CByte(223), CByte(214))
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.Fixed3D
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.Controls.Add(btnOuvrir)
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
        ' btnOuvrir
        ' 
        btnOuvrir.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnOuvrir.BackgroundImageLayout = ImageLayout.Center
        btnOuvrir.FlatAppearance.BorderSize = 0
        btnOuvrir.FlatStyle = FlatStyle.Flat
        btnOuvrir.ForeColor = Color.White
        btnOuvrir.Image = CType(resources.GetObject("btnOuvrir.Image"), Image)
        btnOuvrir.ImageAlign = ContentAlignment.MiddleLeft
        btnOuvrir.Location = New Point(484, 6)
        btnOuvrir.Name = "btnOuvrir"
        btnOuvrir.Size = New Size(112, 40)
        btnOuvrir.TabIndex = 10
        btnOuvrir.Tag = "open_normal"
        btnOuvrir.Text = "Ouvrir"
        btnOuvrir.TextAlign = ContentAlignment.MiddleLeft
        btnOuvrir.TextImageRelation = TextImageRelation.ImageBeforeText
        btnOuvrir.UseVisualStyleBackColor = False
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
        btnModifier.TabIndex = 9
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        btnModifier.TextAlign = ContentAlignment.MiddleLeft
        btnModifier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifier.UseVisualStyleBackColor = False
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(picTitre)
        pnlTitre.Controls.Add(lblTop)
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(16, 16)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(965, 58)
        pnlTitre.TabIndex = 5
        ' 
        ' picTitre
        ' 
        picTitre.BackgroundImage = CType(resources.GetObject("picTitre.BackgroundImage"), Image)
        picTitre.BackgroundImageLayout = ImageLayout.Stretch
        picTitre.ErrorImage = My.Resources.Resources.Fond_icone_Transp
        picTitre.Location = New Point(16, 0)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(60, 52)
        picTitre.TabIndex = 4
        picTitre.TabStop = False
        ' 
        ' lblTop
        ' 
        lblTop.Font = New Font("Calibri", 12F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(221, 19)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(476, 22)
        lblTop.TabIndex = 3
        lblTop.Text = "Recherchez un patient, consultez la liste et ouvrez une fiche."
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 18F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(70, 4)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(0, 4, 8, 4)
        lblTitreForm.Size = New Size(103, 37)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Patients"
        ' 
        ' UC_PatientHome
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_PatientHome"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        CType(dgvPatients, ComponentModel.ISupportInitialize).EndInit()
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        pnlActions.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnNouveau As Button
    Friend WithEvents btnModifier As Button
    Friend WithEvents btnOuvrir As Button
    Friend WithEvents btnActualiser As Button
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents pnlTop As Panel
    Friend WithEvents dgvPatients As DataGridView
    Friend WithEvents txtRecherchePatient As TextBox
    Friend WithEvents lblTop As Label
    Friend WithEvents lblRecherche As Label
    Friend WithEvents cboFiltreSuivi As ComboBox
    Friend WithEvents btnRechercher As Button
    Friend WithEvents btnReinitialiserFiltres As Button
    Friend WithEvents colStatutSuivi As DataGridViewImageColumn
    Friend WithEvents colCodePatient As DataGridViewTextBoxColumn
    Friend WithEvents colNom As DataGridViewTextBoxColumn
    Friend WithEvents colPrenom As DataGridViewTextBoxColumn
    Friend WithEvents colDateNaissance As DataGridViewTextBoxColumn
    Friend WithEvents colNiss As DataGridViewTextBoxColumn
    Friend WithEvents colTelephone As DataGridViewTextBoxColumn
    Friend WithEvents colEmail As DataGridViewTextBoxColumn
    Friend WithEvents colAlerte As DataGridViewCheckBoxColumn
    Friend WithEvents colPhoto As DataGridViewCheckBoxColumn
    Friend WithEvents colDateModification As DataGridViewTextBoxColumn
    Friend WithEvents colIdPatient As DataGridViewTextBoxColumn
    Friend WithEvents picTitre As PictureBox

End Class
