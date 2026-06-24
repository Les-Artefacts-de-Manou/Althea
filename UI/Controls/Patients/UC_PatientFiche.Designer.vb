<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_PatientFiche
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_PatientFiche))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        tabFiche = New TabControl()
        tabPageIdentite = New TabPage()
        grpAlerte = New GroupBox()
        rteAlerte = New UC_RichTextEditorSimple()
        grpAdministratif = New GroupBox()
        btnAjouterSituationFamiliale = New Button()
        cboSituationFamiliale = New ComboBox()
        lblSituationFamiliale = New Label()
        txtMutualite = New TextBox()
        lblMutualite = New Label()
        grpCoordonnees = New GroupBox()
        txtEmail = New TextBox()
        lblEmail = New Label()
        txtTelephone = New TextBox()
        lblTelephone = New Label()
        cboPays = New ComboBox()
        lblPays = New Label()
        txtLocalite = New TextBox()
        lblLocalite = New Label()
        txtCodePostal = New TextBox()
        lblCodePostal = New Label()
        txtAdresseLigne2 = New TextBox()
        lblAdresseLigne2 = New Label()
        txtAdresseLigne1 = New TextBox()
        lblAdresseLigne1 = New Label()
        grpIdentite = New GroupBox()
        cboLateralite = New ComboBox()
        lblLateralite = New Label()
        txtNiss = New TextBox()
        lblNiss = New Label()
        dtpDateNaissance = New DateTimePicker()
        lblDateNaissance = New Label()
        lblAge = New Label()
        txtPrenom = New TextBox()
        lblPrenom = New Label()
        txtNom = New TextBox()
        lblNom = New Label()
        tabPageAnamnese = New TabPage()
        grpAnamnese = New GroupBox()
        rteAnamnese = New UC_RichTextEditor()
        pnlAnamneseActions = New Panel()
        btnAnnulerAnamnese = New Button()
        btnEnregistrerAnamnese = New Button()
        btnModifierAnamnese = New Button()
        tabPageFamille = New TabPage()
        dgvContacts = New DataGridView()
        colContactLien = New DataGridViewTextBoxColumn()
        colContactNom = New DataGridViewTextBoxColumn()
        colContactPrenom = New DataGridViewTextBoxColumn()
        colContactTelephone = New DataGridViewTextBoxColumn()
        colContactRole = New DataGridViewTextBoxColumn()
        pnlFamilleRecherche = New Panel()
        lblRechercheContact = New Label()
        txtRechercheContact = New TextBox()
        btnRechercherContact = New Button()
        btnReinitialiserContacts = New Button()
        pnlFamilleActions = New Panel()
        btnSupprimerContact = New Button()
        btnModifierContact = New Button()
        btnAjouterContact = New Button()
        tabPageIntervenants = New TabPage()
        dgvIntervenants = New DataGridView()
        colIntervenantRole = New DataGridViewTextBoxColumn()
        colIntervenantNom = New DataGridViewTextBoxColumn()
        colIntervenantSpecialite = New DataGridViewTextBoxColumn()
        colIntervenantLieu = New DataGridViewTextBoxColumn()
        colIntervenantDateDebut = New DataGridViewTextBoxColumn()
        colIntervenantDateFin = New DataGridViewTextBoxColumn()
        pnlIntervenantsRecherche = New Panel()
        lblRechercheIntervenant = New Label()
        txtRechercheIntervenant = New TextBox()
        btnRechercherIntervenant = New Button()
        btnReinitialiserIntervenants = New Button()
        pnlIntervenantsActions = New Panel()
        btnSupprimerIntervenant = New Button()
        btnModifierIntervenant = New Button()
        btnAjouterIntervenant = New Button()
        tabPageDossiers = New TabPage()
        lblDossiersAVenir = New Label()
        pnlActions = New Panel()
        btnFermer = New Button()
        btnAnnuler = New Button()
        btnEnregistrer = New Button()
        btnModifier = New Button()
        btnNouveau = New Button()
        pnlBandeau = New Panel()
        rtbAlerte = New RichTextBox()
        btnUploadPhoto = New Button()
        lblCodePatient = New Label()
        lblNomComplet = New Label()
        picPhoto = New PictureBox()
        pnlTitre = New Panel()
        picTitre = New PictureBox()
        lblTop = New Label()
        lblTitreForm = New Label()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        tabFiche.SuspendLayout()
        tabPageIdentite.SuspendLayout()
        grpAlerte.SuspendLayout()
        grpAdministratif.SuspendLayout()
        grpCoordonnees.SuspendLayout()
        grpIdentite.SuspendLayout()
        tabPageAnamnese.SuspendLayout()
        grpAnamnese.SuspendLayout()
        pnlAnamneseActions.SuspendLayout()
        tabPageFamille.SuspendLayout()
        CType(dgvContacts, ComponentModel.ISupportInitialize).BeginInit()
        pnlFamilleRecherche.SuspendLayout()
        pnlFamilleActions.SuspendLayout()
        tabPageIntervenants.SuspendLayout()
        CType(dgvIntervenants, ComponentModel.ISupportInitialize).BeginInit()
        pnlIntervenantsRecherche.SuspendLayout()
        pnlIntervenantsActions.SuspendLayout()
        tabPageDossiers.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlBandeau.SuspendLayout()
        CType(picPhoto, ComponentModel.ISupportInitialize).BeginInit()
        pnlTitre.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(pnlBandeau)
        pnlForm.Controls.Add(pnlTitre)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Font = New Font("Calibri", 11F, FontStyle.Bold)
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 1
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(tabFiche)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 204)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 8, 16, 8)
        pnlCenter.Size = New Size(965, 494)
        pnlCenter.TabIndex = 22
        ' 
        ' tabFiche
        ' 
        tabFiche.Appearance = TabAppearance.FlatButtons
        tabFiche.Controls.Add(tabPageIdentite)
        tabFiche.Controls.Add(tabPageAnamnese)
        tabFiche.Controls.Add(tabPageFamille)
        tabFiche.Controls.Add(tabPageIntervenants)
        tabFiche.Controls.Add(tabPageDossiers)
        tabFiche.Dock = DockStyle.Fill
        tabFiche.Font = New Font("Calibri", 11F)
        tabFiche.Location = New Point(16, 8)
        tabFiche.Multiline = True
        tabFiche.Name = "tabFiche"
        tabFiche.Padding = New Point(12, 6)
        tabFiche.SelectedIndex = 0
        tabFiche.Size = New Size(933, 478)
        tabFiche.TabIndex = 0
        ' 
        ' tabPageIdentite
        ' 
        tabPageIdentite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageIdentite.Controls.Add(grpAlerte)
        tabPageIdentite.Controls.Add(grpAdministratif)
        tabPageIdentite.Controls.Add(grpCoordonnees)
        tabPageIdentite.Controls.Add(grpIdentite)
        tabPageIdentite.Location = New Point(4, 36)
        tabPageIdentite.Name = "tabPageIdentite"
        tabPageIdentite.Padding = New Padding(8)
        tabPageIdentite.Size = New Size(925, 438)
        tabPageIdentite.TabIndex = 0
        tabPageIdentite.Text = "Identité"
        ' 
        ' grpAlerte
        ' 
        grpAlerte.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        grpAlerte.Controls.Add(rteAlerte)
        grpAlerte.FlatStyle = FlatStyle.Flat
        grpAlerte.Font = New Font("Calibri", 11F, FontStyle.Bold)
        grpAlerte.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpAlerte.Location = New Point(16, 348)
        grpAlerte.Name = "grpAlerte"
        grpAlerte.Padding = New Padding(8, 4, 8, 8)
        grpAlerte.Size = New Size(890, 82)
        grpAlerte.TabIndex = 3
        grpAlerte.TabStop = False
        grpAlerte.Text = "Alerte / Notes"
        ' 
        ' rteAlerte
        ' 
        rteAlerte.BackColor = Color.Transparent
        rteAlerte.Dock = DockStyle.Fill
        rteAlerte.Location = New Point(8, 22)
        rteAlerte.Margin = New Padding(4, 3, 4, 3)
        rteAlerte.Name = "rteAlerte"
        rteAlerte.ReadOnlyMode = False
        rteAlerte.RtfContent = ""
        rteAlerte.ShowToolbar = False
        rteAlerte.Size = New Size(874, 52)
        rteAlerte.TabIndex = 0
        ' 
        ' grpAdministratif
        ' 
        grpAdministratif.Controls.Add(btnAjouterSituationFamiliale)
        grpAdministratif.Controls.Add(cboSituationFamiliale)
        grpAdministratif.Controls.Add(lblSituationFamiliale)
        grpAdministratif.Controls.Add(txtMutualite)
        grpAdministratif.Controls.Add(lblMutualite)
        grpAdministratif.FlatStyle = FlatStyle.Flat
        grpAdministratif.Font = New Font("Calibri", 11F, FontStyle.Bold)
        grpAdministratif.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpAdministratif.Location = New Point(16, 230)
        grpAdministratif.Name = "grpAdministratif"
        grpAdministratif.Size = New Size(455, 110)
        grpAdministratif.TabIndex = 2
        grpAdministratif.TabStop = False
        grpAdministratif.Text = "Administratif"
        ' 
        ' btnAjouterSituationFamiliale
        ' 
        btnAjouterSituationFamiliale.FlatAppearance.BorderColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        btnAjouterSituationFamiliale.FlatAppearance.BorderSize = 0
        btnAjouterSituationFamiliale.FlatStyle = FlatStyle.Flat
        btnAjouterSituationFamiliale.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnAjouterSituationFamiliale.Image = CType(resources.GetObject("btnAjouterSituationFamiliale.Image"), Image)
        btnAjouterSituationFamiliale.Location = New Point(404, 66)
        btnAjouterSituationFamiliale.Name = "btnAjouterSituationFamiliale"
        btnAjouterSituationFamiliale.Size = New Size(27, 27)
        btnAjouterSituationFamiliale.TabIndex = 4
        btnAjouterSituationFamiliale.Tag = "plus_24_normal"
        btnAjouterSituationFamiliale.Text = "+"
        btnAjouterSituationFamiliale.UseVisualStyleBackColor = True
        ' 
        ' cboSituationFamiliale
        ' 
        cboSituationFamiliale.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboSituationFamiliale.DropDownStyle = ComboBoxStyle.DropDownList
        cboSituationFamiliale.FlatStyle = FlatStyle.Flat
        cboSituationFamiliale.Font = New Font("Calibri", 11F)
        cboSituationFamiliale.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboSituationFamiliale.Location = New Point(140, 67)
        cboSituationFamiliale.Name = "cboSituationFamiliale"
        cboSituationFamiliale.Size = New Size(260, 26)
        cboSituationFamiliale.TabIndex = 3
        ' 
        ' lblSituationFamiliale
        ' 
        lblSituationFamiliale.AutoSize = True
        lblSituationFamiliale.Font = New Font("Calibri", 10.5F)
        lblSituationFamiliale.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblSituationFamiliale.Location = New Point(16, 70)
        lblSituationFamiliale.Name = "lblSituationFamiliale"
        lblSituationFamiliale.Size = New Size(109, 17)
        lblSituationFamiliale.TabIndex = 2
        lblSituationFamiliale.Text = "Situation familiale"
        ' 
        ' txtMutualite
        ' 
        txtMutualite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtMutualite.Font = New Font("Calibri", 11F)
        txtMutualite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtMutualite.Location = New Point(140, 32)
        txtMutualite.MaxLength = 100
        txtMutualite.Name = "txtMutualite"
        txtMutualite.Size = New Size(290, 25)
        txtMutualite.TabIndex = 1
        ' 
        ' lblMutualite
        ' 
        lblMutualite.AutoSize = True
        lblMutualite.Font = New Font("Calibri", 10.5F)
        lblMutualite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblMutualite.Location = New Point(16, 35)
        lblMutualite.Name = "lblMutualite"
        lblMutualite.Size = New Size(64, 17)
        lblMutualite.TabIndex = 0
        lblMutualite.Text = "Mutualité"
        ' 
        ' grpCoordonnees
        ' 
        grpCoordonnees.Controls.Add(txtEmail)
        grpCoordonnees.Controls.Add(lblEmail)
        grpCoordonnees.Controls.Add(txtTelephone)
        grpCoordonnees.Controls.Add(lblTelephone)
        grpCoordonnees.Controls.Add(cboPays)
        grpCoordonnees.Controls.Add(lblPays)
        grpCoordonnees.Controls.Add(txtLocalite)
        grpCoordonnees.Controls.Add(lblLocalite)
        grpCoordonnees.Controls.Add(txtCodePostal)
        grpCoordonnees.Controls.Add(lblCodePostal)
        grpCoordonnees.Controls.Add(txtAdresseLigne2)
        grpCoordonnees.Controls.Add(lblAdresseLigne2)
        grpCoordonnees.Controls.Add(txtAdresseLigne1)
        grpCoordonnees.Controls.Add(lblAdresseLigne1)
        grpCoordonnees.FlatStyle = FlatStyle.Flat
        grpCoordonnees.Font = New Font("Calibri", 11F, FontStyle.Bold)
        grpCoordonnees.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpCoordonnees.Location = New Point(486, 12)
        grpCoordonnees.Name = "grpCoordonnees"
        grpCoordonnees.Size = New Size(420, 328)
        grpCoordonnees.TabIndex = 1
        grpCoordonnees.TabStop = False
        grpCoordonnees.Text = "Coordonnées"
        ' 
        ' txtEmail
        ' 
        txtEmail.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtEmail.Font = New Font("Calibri", 11F)
        txtEmail.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtEmail.Location = New Point(120, 277)
        txtEmail.MaxLength = 150
        txtEmail.Name = "txtEmail"
        txtEmail.Size = New Size(280, 25)
        txtEmail.TabIndex = 13
        ' 
        ' lblEmail
        ' 
        lblEmail.AutoSize = True
        lblEmail.Font = New Font("Calibri", 10.5F)
        lblEmail.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblEmail.Location = New Point(16, 280)
        lblEmail.Name = "lblEmail"
        lblEmail.Size = New Size(39, 17)
        lblEmail.TabIndex = 12
        lblEmail.Text = "Email"
        ' 
        ' txtTelephone
        ' 
        txtTelephone.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtTelephone.Font = New Font("Calibri", 11F)
        txtTelephone.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtTelephone.Location = New Point(120, 240)
        txtTelephone.MaxLength = 50
        txtTelephone.Name = "txtTelephone"
        txtTelephone.Size = New Size(200, 25)
        txtTelephone.TabIndex = 11
        ' 
        ' lblTelephone
        ' 
        lblTelephone.AutoSize = True
        lblTelephone.Font = New Font("Calibri", 10.5F)
        lblTelephone.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblTelephone.Location = New Point(16, 243)
        lblTelephone.Name = "lblTelephone"
        lblTelephone.Size = New Size(66, 17)
        lblTelephone.TabIndex = 10
        lblTelephone.Text = "Téléphone"
        ' 
        ' cboPays
        ' 
        cboPays.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboPays.DropDownStyle = ComboBoxStyle.DropDownList
        cboPays.FlatStyle = FlatStyle.Flat
        cboPays.Font = New Font("Calibri", 11F)
        cboPays.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboPays.Location = New Point(120, 203)
        cboPays.Name = "cboPays"
        cboPays.Size = New Size(200, 26)
        cboPays.TabIndex = 9
        ' 
        ' lblPays
        ' 
        lblPays.AutoSize = True
        lblPays.Font = New Font("Calibri", 10.5F)
        lblPays.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblPays.Location = New Point(16, 206)
        lblPays.Name = "lblPays"
        lblPays.Size = New Size(33, 17)
        lblPays.TabIndex = 8
        lblPays.Text = "Pays"
        ' 
        ' txtLocalite
        ' 
        txtLocalite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtLocalite.Font = New Font("Calibri", 11F)
        txtLocalite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLocalite.Location = New Point(120, 166)
        txtLocalite.MaxLength = 100
        txtLocalite.Name = "txtLocalite"
        txtLocalite.Size = New Size(280, 25)
        txtLocalite.TabIndex = 7
        ' 
        ' lblLocalite
        ' 
        lblLocalite.AutoSize = True
        lblLocalite.Font = New Font("Calibri", 10.5F)
        lblLocalite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblLocalite.Location = New Point(16, 169)
        lblLocalite.Name = "lblLocalite"
        lblLocalite.Size = New Size(52, 17)
        lblLocalite.TabIndex = 6
        lblLocalite.Text = "Localité"
        ' 
        ' txtCodePostal
        ' 
        txtCodePostal.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCodePostal.Font = New Font("Calibri", 11F)
        txtCodePostal.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCodePostal.Location = New Point(120, 129)
        txtCodePostal.MaxLength = 20
        txtCodePostal.Name = "txtCodePostal"
        txtCodePostal.Size = New Size(120, 25)
        txtCodePostal.TabIndex = 5
        ' 
        ' lblCodePostal
        ' 
        lblCodePostal.AutoSize = True
        lblCodePostal.Font = New Font("Calibri", 10.5F)
        lblCodePostal.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblCodePostal.Location = New Point(16, 132)
        lblCodePostal.Name = "lblCodePostal"
        lblCodePostal.Size = New Size(73, 17)
        lblCodePostal.TabIndex = 4
        lblCodePostal.Text = "Code postal"
        ' 
        ' txtAdresseLigne2
        ' 
        txtAdresseLigne2.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdresseLigne2.Font = New Font("Calibri", 11F)
        txtAdresseLigne2.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdresseLigne2.Location = New Point(120, 92)
        txtAdresseLigne2.MaxLength = 150
        txtAdresseLigne2.Name = "txtAdresseLigne2"
        txtAdresseLigne2.Size = New Size(280, 25)
        txtAdresseLigne2.TabIndex = 3
        ' 
        ' lblAdresseLigne2
        ' 
        lblAdresseLigne2.AutoSize = True
        lblAdresseLigne2.Font = New Font("Calibri", 10.5F)
        lblAdresseLigne2.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblAdresseLigne2.Location = New Point(16, 95)
        lblAdresseLigne2.Name = "lblAdresseLigne2"
        lblAdresseLigne2.Size = New Size(70, 17)
        lblAdresseLigne2.TabIndex = 2
        lblAdresseLigne2.Text = "Adresse (2)"
        ' 
        ' txtAdresseLigne1
        ' 
        txtAdresseLigne1.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdresseLigne1.Font = New Font("Calibri", 11F)
        txtAdresseLigne1.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdresseLigne1.Location = New Point(120, 55)
        txtAdresseLigne1.MaxLength = 150
        txtAdresseLigne1.Name = "txtAdresseLigne1"
        txtAdresseLigne1.Size = New Size(280, 25)
        txtAdresseLigne1.TabIndex = 1
        ' 
        ' lblAdresseLigne1
        ' 
        lblAdresseLigne1.AutoSize = True
        lblAdresseLigne1.Font = New Font("Calibri", 10.5F)
        lblAdresseLigne1.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblAdresseLigne1.Location = New Point(16, 58)
        lblAdresseLigne1.Name = "lblAdresseLigne1"
        lblAdresseLigne1.Size = New Size(70, 17)
        lblAdresseLigne1.TabIndex = 0
        lblAdresseLigne1.Text = "Adresse (1)"
        ' 
        ' grpIdentite
        ' 
        grpIdentite.Controls.Add(cboLateralite)
        grpIdentite.Controls.Add(lblLateralite)
        grpIdentite.Controls.Add(txtNiss)
        grpIdentite.Controls.Add(lblNiss)
        grpIdentite.Controls.Add(dtpDateNaissance)
        grpIdentite.Controls.Add(lblDateNaissance)
        grpIdentite.Controls.Add(lblAge)
        grpIdentite.Controls.Add(txtPrenom)
        grpIdentite.Controls.Add(lblPrenom)
        grpIdentite.Controls.Add(txtNom)
        grpIdentite.Controls.Add(lblNom)
        grpIdentite.FlatStyle = FlatStyle.Flat
        grpIdentite.Font = New Font("Calibri", 11F, FontStyle.Bold)
        grpIdentite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpIdentite.Location = New Point(16, 12)
        grpIdentite.Name = "grpIdentite"
        grpIdentite.Size = New Size(455, 210)
        grpIdentite.TabIndex = 0
        grpIdentite.TabStop = False
        grpIdentite.Text = "Identité civile"
        ' 
        ' cboLateralite
        ' 
        cboLateralite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboLateralite.DropDownStyle = ComboBoxStyle.DropDownList
        cboLateralite.FlatStyle = FlatStyle.Flat
        cboLateralite.Font = New Font("Calibri", 11F)
        cboLateralite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboLateralite.Location = New Point(140, 166)
        cboLateralite.Name = "cboLateralite"
        cboLateralite.Size = New Size(160, 26)
        cboLateralite.TabIndex = 9
        ' 
        ' lblLateralite
        ' 
        lblLateralite.AutoSize = True
        lblLateralite.Font = New Font("Calibri", 10.5F)
        lblLateralite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblLateralite.Location = New Point(16, 169)
        lblLateralite.Name = "lblLateralite"
        lblLateralite.Size = New Size(63, 17)
        lblLateralite.TabIndex = 8
        lblLateralite.Text = "Latéralité"
        ' 
        ' txtNiss
        ' 
        txtNiss.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNiss.Font = New Font("Calibri", 11F)
        txtNiss.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNiss.Location = New Point(140, 129)
        txtNiss.MaxLength = 20
        txtNiss.Name = "txtNiss"
        txtNiss.Size = New Size(200, 25)
        txtNiss.TabIndex = 7
        ' 
        ' lblNiss
        ' 
        lblNiss.AutoSize = True
        lblNiss.Font = New Font("Calibri", 10.5F)
        lblNiss.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblNiss.Location = New Point(16, 132)
        lblNiss.Name = "lblNiss"
        lblNiss.Size = New Size(33, 17)
        lblNiss.TabIndex = 6
        lblNiss.Text = "NISS"
        ' 
        ' dtpDateNaissance
        ' 
        dtpDateNaissance.CalendarForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        dtpDateNaissance.CalendarMonthBackground = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dtpDateNaissance.CalendarTitleForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        dtpDateNaissance.Font = New Font("Calibri", 11F)
        dtpDateNaissance.Format = DateTimePickerFormat.Short
        dtpDateNaissance.Location = New Point(140, 92)
        dtpDateNaissance.Name = "dtpDateNaissance"
        dtpDateNaissance.ShowCheckBox = True
        dtpDateNaissance.Size = New Size(160, 25)
        dtpDateNaissance.TabIndex = 5
        ' 
        ' lblDateNaissance
        ' 
        lblDateNaissance.AutoSize = True
        lblDateNaissance.Font = New Font("Calibri", 10.5F)
        lblDateNaissance.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDateNaissance.Location = New Point(16, 95)
        lblDateNaissance.Name = "lblDateNaissance"
        lblDateNaissance.Size = New Size(93, 17)
        lblDateNaissance.TabIndex = 4
        lblDateNaissance.Text = "Date naissance"
        ' 
        ' lblAge
        ' 
        lblAge.AutoSize = True
        lblAge.Font = New Font("Calibri", 10.5F, FontStyle.Italic)
        lblAge.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAge.Location = New Point(310, 95)
        lblAge.Name = "lblAge"
        lblAge.Size = New Size(0, 17)
        lblAge.TabIndex = 10
        ' 
        ' txtPrenom
        ' 
        txtPrenom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtPrenom.Font = New Font("Calibri", 11F)
        txtPrenom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPrenom.Location = New Point(140, 61)
        txtPrenom.MaxLength = 100
        txtPrenom.Name = "txtPrenom"
        txtPrenom.Size = New Size(290, 25)
        txtPrenom.TabIndex = 3
        ' 
        ' lblPrenom
        ' 
        lblPrenom.AutoSize = True
        lblPrenom.Font = New Font("Calibri", 10.5F)
        lblPrenom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblPrenom.Location = New Point(16, 64)
        lblPrenom.Name = "lblPrenom"
        lblPrenom.Size = New Size(52, 17)
        lblPrenom.TabIndex = 2
        lblPrenom.Text = "Prénom"
        ' 
        ' txtNom
        ' 
        txtNom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNom.Font = New Font("Calibri", 11F)
        txtNom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNom.Location = New Point(140, 32)
        txtNom.MaxLength = 100
        txtNom.Name = "txtNom"
        txtNom.Size = New Size(290, 25)
        txtNom.TabIndex = 1
        ' 
        ' lblNom
        ' 
        lblNom.AutoSize = True
        lblNom.Font = New Font("Calibri", 10.5F)
        lblNom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblNom.Location = New Point(16, 35)
        lblNom.Name = "lblNom"
        lblNom.Size = New Size(35, 17)
        lblNom.TabIndex = 0
        lblNom.Text = "Nom"
        ' 
        ' tabPageAnamnese
        ' 
        tabPageAnamnese.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageAnamnese.Controls.Add(grpAnamnese)
        tabPageAnamnese.Controls.Add(pnlAnamneseActions)
        tabPageAnamnese.Location = New Point(4, 36)
        tabPageAnamnese.Name = "tabPageAnamnese"
        tabPageAnamnese.Padding = New Padding(8)
        tabPageAnamnese.Size = New Size(925, 438)
        tabPageAnamnese.TabIndex = 4
        tabPageAnamnese.Text = "Anamnèse"
        ' 
        ' grpAnamnese
        ' 
        grpAnamnese.Controls.Add(rteAnamnese)
        grpAnamnese.Dock = DockStyle.Fill
        grpAnamnese.FlatStyle = FlatStyle.Flat
        grpAnamnese.Font = New Font("Calibri", 11F, FontStyle.Bold)
        grpAnamnese.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpAnamnese.Location = New Point(8, 8)
        grpAnamnese.Name = "grpAnamnese"
        grpAnamnese.Padding = New Padding(8, 4, 8, 8)
        grpAnamnese.Size = New Size(909, 378)
        grpAnamnese.TabIndex = 0
        grpAnamnese.TabStop = False
        grpAnamnese.Text = "Anamnèse"
        ' 
        ' rteAnamnese
        ' 
        rteAnamnese.BackColor = Color.Transparent
        rteAnamnese.Dock = DockStyle.Fill
        rteAnamnese.Location = New Point(8, 22)
        rteAnamnese.Margin = New Padding(4, 3, 4, 3)
        rteAnamnese.Name = "rteAnamnese"
        rteAnamnese.ReadOnlyMode = False
        rteAnamnese.RtfContent = ""
        rteAnamnese.ShowToolbar = False
        rteAnamnese.Size = New Size(893, 348)
        rteAnamnese.TabIndex = 0
        ' 
        ' pnlAnamneseActions
        ' 
        pnlAnamneseActions.Controls.Add(btnAnnulerAnamnese)
        pnlAnamneseActions.Controls.Add(btnEnregistrerAnamnese)
        pnlAnamneseActions.Controls.Add(btnModifierAnamnese)
        pnlAnamneseActions.Dock = DockStyle.Bottom
        pnlAnamneseActions.Location = New Point(8, 386)
        pnlAnamneseActions.Name = "pnlAnamneseActions"
        pnlAnamneseActions.Padding = New Padding(0, 6, 0, 0)
        pnlAnamneseActions.Size = New Size(909, 44)
        pnlAnamneseActions.TabIndex = 1
        ' 
        ' btnAnnulerAnamnese
        ' 
        btnAnnulerAnamnese.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAnnulerAnamnese.FlatAppearance.BorderSize = 0
        btnAnnulerAnamnese.FlatStyle = FlatStyle.Flat
        btnAnnulerAnamnese.Font = New Font("Calibri", 10F)
        btnAnnulerAnamnese.ForeColor = Color.White
        btnAnnulerAnamnese.ImageAlign = ContentAlignment.MiddleLeft
        btnAnnulerAnamnese.Location = New Point(120, 6)
        btnAnnulerAnamnese.Name = "btnAnnulerAnamnese"
        btnAnnulerAnamnese.Size = New Size(112, 38)
        btnAnnulerAnamnese.TabIndex = 2
        btnAnnulerAnamnese.Tag = "annuler_normal"
        btnAnnulerAnamnese.Text = "Annuler"
        btnAnnulerAnamnese.TextAlign = ContentAlignment.MiddleLeft
        btnAnnulerAnamnese.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAnnulerAnamnese.UseVisualStyleBackColor = False
        ' 
        ' btnEnregistrerAnamnese
        ' 
        btnEnregistrerAnamnese.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnEnregistrerAnamnese.FlatAppearance.BorderSize = 0
        btnEnregistrerAnamnese.FlatStyle = FlatStyle.Flat
        btnEnregistrerAnamnese.Font = New Font("Calibri", 10F)
        btnEnregistrerAnamnese.ForeColor = Color.White
        btnEnregistrerAnamnese.ImageAlign = ContentAlignment.MiddleLeft
        btnEnregistrerAnamnese.Location = New Point(0, 6)
        btnEnregistrerAnamnese.Name = "btnEnregistrerAnamnese"
        btnEnregistrerAnamnese.Size = New Size(112, 38)
        btnEnregistrerAnamnese.TabIndex = 1
        btnEnregistrerAnamnese.Tag = "enregistrer_normal"
        btnEnregistrerAnamnese.Text = "Enregistrer"
        btnEnregistrerAnamnese.TextAlign = ContentAlignment.MiddleLeft
        btnEnregistrerAnamnese.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEnregistrerAnamnese.UseVisualStyleBackColor = False
        ' 
        ' btnModifierAnamnese
        ' 
        btnModifierAnamnese.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnModifierAnamnese.FlatAppearance.BorderSize = 0
        btnModifierAnamnese.FlatStyle = FlatStyle.Flat
        btnModifierAnamnese.Font = New Font("Calibri", 10F)
        btnModifierAnamnese.ForeColor = Color.White
        btnModifierAnamnese.ImageAlign = ContentAlignment.MiddleLeft
        btnModifierAnamnese.Location = New Point(0, 6)
        btnModifierAnamnese.Name = "btnModifierAnamnese"
        btnModifierAnamnese.Size = New Size(112, 38)
        btnModifierAnamnese.TabIndex = 0
        btnModifierAnamnese.Tag = "modifier_normal"
        btnModifierAnamnese.Text = "Modifier"
        btnModifierAnamnese.TextAlign = ContentAlignment.MiddleLeft
        btnModifierAnamnese.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifierAnamnese.UseVisualStyleBackColor = False
        ' 
        ' tabPageFamille
        ' 
        tabPageFamille.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageFamille.Controls.Add(dgvContacts)
        tabPageFamille.Controls.Add(pnlFamilleRecherche)
        tabPageFamille.Controls.Add(pnlFamilleActions)
        tabPageFamille.Location = New Point(4, 36)
        tabPageFamille.Name = "tabPageFamille"
        tabPageFamille.Padding = New Padding(8)
        tabPageFamille.Size = New Size(925, 438)
        tabPageFamille.TabIndex = 1
        tabPageFamille.Text = "Famille / Contacts"
        ' 
        ' dgvContacts
        ' 
        dgvContacts.AllowUserToAddRows = False
        dgvContacts.AllowUserToDeleteRows = False
        dgvContacts.AllowUserToResizeRows = False
        dgvContacts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvContacts.BackgroundColor = Color.White
        dgvContacts.BorderStyle = BorderStyle.None
        dgvContacts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvContacts.Columns.AddRange(New DataGridViewColumn() {colContactLien, colContactNom, colContactPrenom, colContactTelephone, colContactRole})
        dgvContacts.Dock = DockStyle.Fill
        dgvContacts.EditMode = DataGridViewEditMode.EditProgrammatically
        dgvContacts.Font = New Font("Calibri", 10F)
        dgvContacts.Location = New Point(8, 52)
        dgvContacts.MultiSelect = False
        dgvContacts.Name = "dgvContacts"
        dgvContacts.ReadOnly = True
        dgvContacts.RowHeadersVisible = False
        dgvContacts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvContacts.Size = New Size(909, 334)
        dgvContacts.TabIndex = 0
        ' 
        ' colContactLien
        ' 
        colContactLien.DataPropertyName = "LibelleLienPatient"
        colContactLien.FillWeight = 130F
        colContactLien.HeaderText = "Lien"
        colContactLien.Name = "colContactLien"
        colContactLien.ReadOnly = True
        ' 
        ' colContactNom
        ' 
        colContactNom.DataPropertyName = "Nom"
        colContactNom.FillWeight = 130F
        colContactNom.HeaderText = "Nom"
        colContactNom.Name = "colContactNom"
        colContactNom.ReadOnly = True
        ' 
        ' colContactPrenom
        ' 
        colContactPrenom.DataPropertyName = "Prenom"
        colContactPrenom.FillWeight = 130F
        colContactPrenom.HeaderText = "Prénom"
        colContactPrenom.Name = "colContactPrenom"
        colContactPrenom.ReadOnly = True
        ' 
        ' colContactTelephone
        ' 
        colContactTelephone.DataPropertyName = "Telephone"
        colContactTelephone.FillWeight = 130F
        colContactTelephone.HeaderText = "Téléphone"
        colContactTelephone.Name = "colContactTelephone"
        colContactTelephone.ReadOnly = True
        ' 
        ' colContactRole
        ' 
        colContactRole.DataPropertyName = "LibelleRoleLegal"
        colContactRole.FillWeight = 150F
        colContactRole.HeaderText = "Rôle légal"
        colContactRole.Name = "colContactRole"
        colContactRole.ReadOnly = True
        colContactRole.Resizable = DataGridViewTriState.True
        ' 
        ' pnlFamilleRecherche
        ' 
        pnlFamilleRecherche.Controls.Add(lblRechercheContact)
        pnlFamilleRecherche.Controls.Add(txtRechercheContact)
        pnlFamilleRecherche.Controls.Add(btnRechercherContact)
        pnlFamilleRecherche.Controls.Add(btnReinitialiserContacts)
        pnlFamilleRecherche.Dock = DockStyle.Top
        pnlFamilleRecherche.Location = New Point(8, 8)
        pnlFamilleRecherche.Name = "pnlFamilleRecherche"
        pnlFamilleRecherche.Padding = New Padding(0, 0, 0, 6)
        pnlFamilleRecherche.Size = New Size(909, 44)
        pnlFamilleRecherche.TabIndex = 0
        ' 
        ' lblRechercheContact
        ' 
        lblRechercheContact.AutoSize = True
        lblRechercheContact.Font = New Font("Calibri", 10F)
        lblRechercheContact.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRechercheContact.Location = New Point(0, 11)
        lblRechercheContact.Name = "lblRechercheContact"
        lblRechercheContact.Size = New Size(182, 17)
        lblRechercheContact.TabIndex = 0
        lblRechercheContact.Text = "Recherche (nom, prénom, lien)"
        ' 
        ' txtRechercheContact
        ' 
        txtRechercheContact.Font = New Font("Calibri", 10F)
        txtRechercheContact.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtRechercheContact.Location = New Point(206, 7)
        txtRechercheContact.Name = "txtRechercheContact"
        txtRechercheContact.Size = New Size(300, 24)
        txtRechercheContact.TabIndex = 1
        ' 
        ' btnRechercherContact
        ' 
        btnRechercherContact.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnRechercherContact.FlatAppearance.BorderSize = 0
        btnRechercherContact.FlatStyle = FlatStyle.Flat
        btnRechercherContact.Font = New Font("Calibri", 10F)
        btnRechercherContact.ForeColor = Color.White
        btnRechercherContact.Image = CType(resources.GetObject("btnRechercherContact.Image"), Image)
        btnRechercherContact.ImageAlign = ContentAlignment.MiddleLeft
        btnRechercherContact.Location = New Point(516, 5)
        btnRechercherContact.Name = "btnRechercherContact"
        btnRechercherContact.Size = New Size(112, 32)
        btnRechercherContact.TabIndex = 2
        btnRechercherContact.Tag = "recherche_24_normal"
        btnRechercherContact.Text = "Rechercher"
        btnRechercherContact.TextAlign = ContentAlignment.MiddleLeft
        btnRechercherContact.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRechercherContact.UseVisualStyleBackColor = False
        ' 
        ' btnReinitialiserContacts
        ' 
        btnReinitialiserContacts.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnReinitialiserContacts.FlatAppearance.BorderSize = 0
        btnReinitialiserContacts.FlatStyle = FlatStyle.Flat
        btnReinitialiserContacts.Font = New Font("Calibri", 10F)
        btnReinitialiserContacts.ForeColor = Color.White
        btnReinitialiserContacts.Image = CType(resources.GetObject("btnReinitialiserContacts.Image"), Image)
        btnReinitialiserContacts.ImageAlign = ContentAlignment.MiddleLeft
        btnReinitialiserContacts.Location = New Point(636, 5)
        btnReinitialiserContacts.Name = "btnReinitialiserContacts"
        btnReinitialiserContacts.Size = New Size(112, 32)
        btnReinitialiserContacts.TabIndex = 3
        btnReinitialiserContacts.Tag = "reinitialiser_24_normal"
        btnReinitialiserContacts.Text = "Réinitialiser"
        btnReinitialiserContacts.TextAlign = ContentAlignment.MiddleLeft
        btnReinitialiserContacts.TextImageRelation = TextImageRelation.ImageBeforeText
        btnReinitialiserContacts.UseVisualStyleBackColor = False
        ' 
        ' pnlFamilleActions
        ' 
        pnlFamilleActions.Controls.Add(btnSupprimerContact)
        pnlFamilleActions.Controls.Add(btnModifierContact)
        pnlFamilleActions.Controls.Add(btnAjouterContact)
        pnlFamilleActions.Dock = DockStyle.Bottom
        pnlFamilleActions.Location = New Point(8, 386)
        pnlFamilleActions.Name = "pnlFamilleActions"
        pnlFamilleActions.Padding = New Padding(0, 6, 0, 0)
        pnlFamilleActions.Size = New Size(909, 44)
        pnlFamilleActions.TabIndex = 1
        ' 
        ' btnSupprimerContact
        ' 
        btnSupprimerContact.BackColor = Color.FromArgb(CByte(176), CByte(122), CByte(122))
        btnSupprimerContact.FlatAppearance.BorderSize = 0
        btnSupprimerContact.FlatStyle = FlatStyle.Flat
        btnSupprimerContact.Font = New Font("Calibri", 10F)
        btnSupprimerContact.ForeColor = Color.White
        btnSupprimerContact.Image = CType(resources.GetObject("btnSupprimerContact.Image"), Image)
        btnSupprimerContact.ImageAlign = ContentAlignment.MiddleLeft
        btnSupprimerContact.Location = New Point(240, 6)
        btnSupprimerContact.Name = "btnSupprimerContact"
        btnSupprimerContact.Size = New Size(112, 38)
        btnSupprimerContact.TabIndex = 2
        btnSupprimerContact.Tag = "supprimer_normal"
        btnSupprimerContact.Text = "Supprimer"
        btnSupprimerContact.TextAlign = ContentAlignment.MiddleLeft
        btnSupprimerContact.TextImageRelation = TextImageRelation.ImageBeforeText
        btnSupprimerContact.UseVisualStyleBackColor = False
        ' 
        ' btnModifierContact
        ' 
        btnModifierContact.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnModifierContact.FlatAppearance.BorderSize = 0
        btnModifierContact.FlatStyle = FlatStyle.Flat
        btnModifierContact.Font = New Font("Calibri", 10F)
        btnModifierContact.ForeColor = Color.White
        btnModifierContact.Image = CType(resources.GetObject("btnModifierContact.Image"), Image)
        btnModifierContact.ImageAlign = ContentAlignment.MiddleLeft
        btnModifierContact.Location = New Point(120, 6)
        btnModifierContact.Name = "btnModifierContact"
        btnModifierContact.Size = New Size(112, 38)
        btnModifierContact.TabIndex = 1
        btnModifierContact.Tag = "modifier_normal"
        btnModifierContact.Text = "Modifier"
        btnModifierContact.TextAlign = ContentAlignment.MiddleLeft
        btnModifierContact.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifierContact.UseVisualStyleBackColor = False
        ' 
        ' btnAjouterContact
        ' 
        btnAjouterContact.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAjouterContact.FlatAppearance.BorderSize = 0
        btnAjouterContact.FlatStyle = FlatStyle.Flat
        btnAjouterContact.Font = New Font("Calibri", 10F)
        btnAjouterContact.ForeColor = Color.White
        btnAjouterContact.Image = CType(resources.GetObject("btnAjouterContact.Image"), Image)
        btnAjouterContact.ImageAlign = ContentAlignment.MiddleLeft
        btnAjouterContact.Location = New Point(0, 6)
        btnAjouterContact.Name = "btnAjouterContact"
        btnAjouterContact.Size = New Size(112, 38)
        btnAjouterContact.TabIndex = 0
        btnAjouterContact.Tag = "ajouter_normal"
        btnAjouterContact.Text = "Ajouter"
        btnAjouterContact.TextAlign = ContentAlignment.MiddleLeft
        btnAjouterContact.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAjouterContact.UseVisualStyleBackColor = False
        ' 
        ' tabPageIntervenants
        ' 
        tabPageIntervenants.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageIntervenants.Controls.Add(dgvIntervenants)
        tabPageIntervenants.Controls.Add(pnlIntervenantsRecherche)
        tabPageIntervenants.Controls.Add(pnlIntervenantsActions)
        tabPageIntervenants.Location = New Point(4, 36)
        tabPageIntervenants.Name = "tabPageIntervenants"
        tabPageIntervenants.Padding = New Padding(8)
        tabPageIntervenants.Size = New Size(925, 438)
        tabPageIntervenants.TabIndex = 2
        tabPageIntervenants.Text = "Intervenants"
        ' 
        ' dgvIntervenants
        ' 
        dgvIntervenants.AllowUserToAddRows = False
        dgvIntervenants.AllowUserToDeleteRows = False
        dgvIntervenants.AllowUserToResizeRows = False
        dgvIntervenants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvIntervenants.BackgroundColor = Color.White
        dgvIntervenants.BorderStyle = BorderStyle.None
        dgvIntervenants.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvIntervenants.Columns.AddRange(New DataGridViewColumn() {colIntervenantRole, colIntervenantNom, colIntervenantSpecialite, colIntervenantLieu, colIntervenantDateDebut, colIntervenantDateFin})
        dgvIntervenants.Dock = DockStyle.Fill
        dgvIntervenants.EditMode = DataGridViewEditMode.EditProgrammatically
        dgvIntervenants.Font = New Font("Calibri", 10F)
        dgvIntervenants.Location = New Point(8, 52)
        dgvIntervenants.MultiSelect = False
        dgvIntervenants.Name = "dgvIntervenants"
        dgvIntervenants.ReadOnly = True
        dgvIntervenants.RowHeadersVisible = False
        dgvIntervenants.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvIntervenants.Size = New Size(909, 334)
        dgvIntervenants.TabIndex = 0
        ' 
        ' colIntervenantRole
        ' 
        colIntervenantRole.DataPropertyName = "LibelleRoleIntervenant"
        colIntervenantRole.FillWeight = 130F
        colIntervenantRole.HeaderText = "Rôle"
        colIntervenantRole.Name = "colIntervenantRole"
        colIntervenantRole.ReadOnly = True
        ' 
        ' colIntervenantNom
        ' 
        colIntervenantNom.DataPropertyName = "NomProfessionnel"
        colIntervenantNom.FillWeight = 160F
        colIntervenantNom.HeaderText = "Nom / praticien"
        colIntervenantNom.Name = "colIntervenantNom"
        colIntervenantNom.ReadOnly = True
        ' 
        ' colIntervenantSpecialite
        ' 
        colIntervenantSpecialite.DataPropertyName = "Specialite"
        colIntervenantSpecialite.FillWeight = 130F
        colIntervenantSpecialite.HeaderText = "Spécialité"
        colIntervenantSpecialite.Name = "colIntervenantSpecialite"
        colIntervenantSpecialite.ReadOnly = True
        ' 
        ' colIntervenantLieu
        ' 
        colIntervenantLieu.DataPropertyName = "Lieu"
        colIntervenantLieu.FillWeight = 130F
        colIntervenantLieu.HeaderText = "Lieu"
        colIntervenantLieu.Name = "colIntervenantLieu"
        colIntervenantLieu.ReadOnly = True
        ' 
        ' colIntervenantDateDebut
        ' 
        colIntervenantDateDebut.DataPropertyName = "DateDebut"
        colIntervenantDateDebut.FillWeight = 80F
        colIntervenantDateDebut.HeaderText = "Depuis"
        colIntervenantDateDebut.Name = "colIntervenantDateDebut"
        colIntervenantDateDebut.ReadOnly = True
        ' 
        ' colIntervenantDateFin
        ' 
        colIntervenantDateFin.DataPropertyName = "DateFin"
        colIntervenantDateFin.FillWeight = 80F
        colIntervenantDateFin.HeaderText = "Jusqu'au"
        colIntervenantDateFin.Name = "colIntervenantDateFin"
        colIntervenantDateFin.ReadOnly = True
        ' 
        ' pnlIntervenantsRecherche
        ' 
        pnlIntervenantsRecherche.Controls.Add(lblRechercheIntervenant)
        pnlIntervenantsRecherche.Controls.Add(txtRechercheIntervenant)
        pnlIntervenantsRecherche.Controls.Add(btnRechercherIntervenant)
        pnlIntervenantsRecherche.Controls.Add(btnReinitialiserIntervenants)
        pnlIntervenantsRecherche.Dock = DockStyle.Top
        pnlIntervenantsRecherche.Location = New Point(8, 8)
        pnlIntervenantsRecherche.Name = "pnlIntervenantsRecherche"
        pnlIntervenantsRecherche.Padding = New Padding(0, 0, 0, 6)
        pnlIntervenantsRecherche.Size = New Size(909, 44)
        pnlIntervenantsRecherche.TabIndex = 0
        ' 
        ' lblRechercheIntervenant
        ' 
        lblRechercheIntervenant.AutoSize = True
        lblRechercheIntervenant.Font = New Font("Calibri", 10F)
        lblRechercheIntervenant.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRechercheIntervenant.Location = New Point(0, 11)
        lblRechercheIntervenant.Name = "lblRechercheIntervenant"
        lblRechercheIntervenant.Size = New Size(193, 17)
        lblRechercheIntervenant.TabIndex = 0
        lblRechercheIntervenant.Text = "Recherche (rôle, nom, spécialité)"
        ' 
        ' txtRechercheIntervenant
        ' 
        txtRechercheIntervenant.Font = New Font("Calibri", 10F)
        txtRechercheIntervenant.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtRechercheIntervenant.Location = New Point(206, 7)
        txtRechercheIntervenant.Name = "txtRechercheIntervenant"
        txtRechercheIntervenant.Size = New Size(300, 24)
        txtRechercheIntervenant.TabIndex = 1
        ' 
        ' btnRechercherIntervenant
        ' 
        btnRechercherIntervenant.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnRechercherIntervenant.FlatAppearance.BorderSize = 0
        btnRechercherIntervenant.FlatStyle = FlatStyle.Flat
        btnRechercherIntervenant.Font = New Font("Calibri", 10F)
        btnRechercherIntervenant.ForeColor = Color.White
        btnRechercherIntervenant.Image = CType(resources.GetObject("btnRechercherIntervenant.Image"), Image)
        btnRechercherIntervenant.ImageAlign = ContentAlignment.MiddleLeft
        btnRechercherIntervenant.Location = New Point(516, 5)
        btnRechercherIntervenant.Name = "btnRechercherIntervenant"
        btnRechercherIntervenant.Size = New Size(112, 32)
        btnRechercherIntervenant.TabIndex = 2
        btnRechercherIntervenant.Text = "Rechercher"
        btnRechercherIntervenant.TextAlign = ContentAlignment.MiddleLeft
        btnRechercherIntervenant.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRechercherIntervenant.UseVisualStyleBackColor = False
        ' 
        ' btnReinitialiserIntervenants
        ' 
        btnReinitialiserIntervenants.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnReinitialiserIntervenants.FlatAppearance.BorderSize = 0
        btnReinitialiserIntervenants.FlatStyle = FlatStyle.Flat
        btnReinitialiserIntervenants.Font = New Font("Calibri", 10F)
        btnReinitialiserIntervenants.ForeColor = Color.White
        btnReinitialiserIntervenants.Image = CType(resources.GetObject("btnReinitialiserIntervenants.Image"), Image)
        btnReinitialiserIntervenants.ImageAlign = ContentAlignment.MiddleLeft
        btnReinitialiserIntervenants.Location = New Point(636, 5)
        btnReinitialiserIntervenants.Name = "btnReinitialiserIntervenants"
        btnReinitialiserIntervenants.Size = New Size(112, 32)
        btnReinitialiserIntervenants.TabIndex = 3
        btnReinitialiserIntervenants.Tag = "reinitialiser_24_normal"
        btnReinitialiserIntervenants.Text = "Réinitialiser"
        btnReinitialiserIntervenants.TextAlign = ContentAlignment.MiddleLeft
        btnReinitialiserIntervenants.TextImageRelation = TextImageRelation.ImageBeforeText
        btnReinitialiserIntervenants.UseVisualStyleBackColor = False
        ' 
        ' pnlIntervenantsActions
        ' 
        pnlIntervenantsActions.Controls.Add(btnSupprimerIntervenant)
        pnlIntervenantsActions.Controls.Add(btnModifierIntervenant)
        pnlIntervenantsActions.Controls.Add(btnAjouterIntervenant)
        pnlIntervenantsActions.Dock = DockStyle.Bottom
        pnlIntervenantsActions.Location = New Point(8, 386)
        pnlIntervenantsActions.Name = "pnlIntervenantsActions"
        pnlIntervenantsActions.Padding = New Padding(0, 6, 0, 0)
        pnlIntervenantsActions.Size = New Size(909, 44)
        pnlIntervenantsActions.TabIndex = 1
        ' 
        ' btnSupprimerIntervenant
        ' 
        btnSupprimerIntervenant.BackColor = Color.FromArgb(CByte(176), CByte(122), CByte(122))
        btnSupprimerIntervenant.FlatAppearance.BorderSize = 0
        btnSupprimerIntervenant.FlatStyle = FlatStyle.Flat
        btnSupprimerIntervenant.Font = New Font("Calibri", 10F)
        btnSupprimerIntervenant.ForeColor = Color.White
        btnSupprimerIntervenant.Image = CType(resources.GetObject("btnSupprimerIntervenant.Image"), Image)
        btnSupprimerIntervenant.ImageAlign = ContentAlignment.MiddleLeft
        btnSupprimerIntervenant.Location = New Point(240, 6)
        btnSupprimerIntervenant.Name = "btnSupprimerIntervenant"
        btnSupprimerIntervenant.Size = New Size(112, 38)
        btnSupprimerIntervenant.TabIndex = 2
        btnSupprimerIntervenant.Text = "Supprimer"
        btnSupprimerIntervenant.TextAlign = ContentAlignment.MiddleLeft
        btnSupprimerIntervenant.TextImageRelation = TextImageRelation.ImageBeforeText
        btnSupprimerIntervenant.UseVisualStyleBackColor = False
        ' 
        ' btnModifierIntervenant
        ' 
        btnModifierIntervenant.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnModifierIntervenant.FlatAppearance.BorderSize = 0
        btnModifierIntervenant.FlatStyle = FlatStyle.Flat
        btnModifierIntervenant.Font = New Font("Calibri", 10F)
        btnModifierIntervenant.ForeColor = Color.White
        btnModifierIntervenant.Image = CType(resources.GetObject("btnModifierIntervenant.Image"), Image)
        btnModifierIntervenant.ImageAlign = ContentAlignment.MiddleLeft
        btnModifierIntervenant.Location = New Point(120, 6)
        btnModifierIntervenant.Name = "btnModifierIntervenant"
        btnModifierIntervenant.Size = New Size(112, 38)
        btnModifierIntervenant.TabIndex = 1
        btnModifierIntervenant.Text = "Modifier"
        btnModifierIntervenant.TextAlign = ContentAlignment.MiddleLeft
        btnModifierIntervenant.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifierIntervenant.UseVisualStyleBackColor = False
        ' 
        ' btnAjouterIntervenant
        ' 
        btnAjouterIntervenant.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAjouterIntervenant.FlatAppearance.BorderSize = 0
        btnAjouterIntervenant.FlatStyle = FlatStyle.Flat
        btnAjouterIntervenant.Font = New Font("Calibri", 10F)
        btnAjouterIntervenant.ForeColor = Color.White
        btnAjouterIntervenant.Image = CType(resources.GetObject("btnAjouterIntervenant.Image"), Image)
        btnAjouterIntervenant.ImageAlign = ContentAlignment.MiddleLeft
        btnAjouterIntervenant.Location = New Point(0, 6)
        btnAjouterIntervenant.Name = "btnAjouterIntervenant"
        btnAjouterIntervenant.Size = New Size(112, 38)
        btnAjouterIntervenant.TabIndex = 0
        btnAjouterIntervenant.Text = "Ajouter"
        btnAjouterIntervenant.TextAlign = ContentAlignment.MiddleLeft
        btnAjouterIntervenant.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAjouterIntervenant.UseVisualStyleBackColor = False
        ' 
        ' tabPageDossiers
        ' 
        tabPageDossiers.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageDossiers.Controls.Add(lblDossiersAVenir)
        tabPageDossiers.Location = New Point(4, 36)
        tabPageDossiers.Name = "tabPageDossiers"
        tabPageDossiers.Padding = New Padding(8)
        tabPageDossiers.Size = New Size(925, 438)
        tabPageDossiers.TabIndex = 3
        tabPageDossiers.Text = "Dossiers"
        ' 
        ' lblDossiersAVenir
        ' 
        lblDossiersAVenir.Dock = DockStyle.Fill
        lblDossiersAVenir.Font = New Font("Calibri", 12F, FontStyle.Italic)
        lblDossiersAVenir.ForeColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        lblDossiersAVenir.Location = New Point(8, 8)
        lblDossiersAVenir.Name = "lblDossiersAVenir"
        lblDossiersAVenir.Size = New Size(909, 422)
        lblDossiersAVenir.TabIndex = 0
        lblDossiersAVenir.Text = "Dossiers du patient (lot C2) : à venir."
        lblDossiersAVenir.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.FromArgb(CByte(231), CByte(223), CByte(214))
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.Fixed3D
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Padding = New Padding(8)
        pnlActions.Size = New Size(965, 57)
        pnlActions.TabIndex = 20
        ' 
        ' btnFermer
        ' 
        btnFermer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnFermer.BackgroundImageLayout = ImageLayout.Center
        btnFermer.FlatAppearance.BorderSize = 0
        btnFermer.FlatStyle = FlatStyle.Flat
        btnFermer.ForeColor = Color.White
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.ImageAlign = ContentAlignment.MiddleLeft
        btnFermer.Location = New Point(823, 6)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(130, 40)
        btnFermer.TabIndex = 4
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Retour liste"
        btnFermer.TextAlign = ContentAlignment.MiddleLeft
        btnFermer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnFermer.UseVisualStyleBackColor = False
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
        btnAnnuler.Location = New Point(366, 6)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(112, 40)
        btnAnnuler.TabIndex = 2
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
        btnEnregistrer.Location = New Point(247, 6)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(112, 40)
        btnEnregistrer.TabIndex = 1
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.TextAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEnregistrer.UseVisualStyleBackColor = False
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
        btnModifier.Location = New Point(128, 6)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(112, 40)
        btnModifier.TabIndex = 0
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        btnModifier.TextAlign = ContentAlignment.MiddleLeft
        btnModifier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifier.UseVisualStyleBackColor = False
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
        btnNouveau.Location = New Point(9, 6)
        btnNouveau.Name = "btnNouveau"
        btnNouveau.Size = New Size(112, 40)
        btnNouveau.TabIndex = 3
        btnNouveau.Tag = "nouveau_normal"
        btnNouveau.Text = "Nouveau"
        btnNouveau.TextAlign = ContentAlignment.MiddleLeft
        btnNouveau.TextImageRelation = TextImageRelation.ImageBeforeText
        btnNouveau.UseVisualStyleBackColor = False
        ' 
        ' pnlBandeau
        ' 
        pnlBandeau.BackColor = Color.FromArgb(CByte(231), CByte(223), CByte(214))
        pnlBandeau.Controls.Add(rtbAlerte)
        pnlBandeau.Controls.Add(btnUploadPhoto)
        pnlBandeau.Controls.Add(lblCodePatient)
        pnlBandeau.Controls.Add(lblNomComplet)
        pnlBandeau.Controls.Add(picPhoto)
        pnlBandeau.Dock = DockStyle.Top
        pnlBandeau.Location = New Point(16, 74)
        pnlBandeau.Name = "pnlBandeau"
        pnlBandeau.Padding = New Padding(12)
        pnlBandeau.Size = New Size(965, 130)
        pnlBandeau.TabIndex = 21
        ' 
        ' rtbAlerte
        ' 
        rtbAlerte.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        rtbAlerte.BackColor = Color.FromArgb(CByte(255), CByte(252), CByte(245))
        rtbAlerte.BorderStyle = BorderStyle.FixedSingle
        rtbAlerte.Font = New Font("Calibri", 10F)
        rtbAlerte.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        rtbAlerte.Location = New Point(470, 16)
        rtbAlerte.Name = "rtbAlerte"
        rtbAlerte.ReadOnly = True
        rtbAlerte.ScrollBars = RichTextBoxScrollBars.Vertical
        rtbAlerte.Size = New Size(475, 98)
        rtbAlerte.TabIndex = 3
        rtbAlerte.TabStop = False
        rtbAlerte.Text = "Aucune note."
        ' 
        ' btnUploadPhoto
        ' 
        btnUploadPhoto.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnUploadPhoto.FlatAppearance.BorderSize = 0
        btnUploadPhoto.FlatStyle = FlatStyle.Flat
        btnUploadPhoto.Font = New Font("Calibri", 9F)
        btnUploadPhoto.ForeColor = Color.White
        btnUploadPhoto.Image = CType(resources.GetObject("btnUploadPhoto.Image"), Image)
        btnUploadPhoto.ImageAlign = ContentAlignment.MiddleLeft
        btnUploadPhoto.Location = New Point(126, 84)
        btnUploadPhoto.Name = "btnUploadPhoto"
        btnUploadPhoto.Size = New Size(77, 28)
        btnUploadPhoto.TabIndex = 4
        btnUploadPhoto.Text = "Photo..."
        btnUploadPhoto.TextAlign = ContentAlignment.MiddleLeft
        btnUploadPhoto.TextImageRelation = TextImageRelation.ImageBeforeText
        btnUploadPhoto.UseVisualStyleBackColor = False
        ' 
        ' lblCodePatient
        ' 
        lblCodePatient.AutoSize = True
        lblCodePatient.Font = New Font("Calibri", 10.5F)
        lblCodePatient.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCodePatient.Location = New Point(126, 54)
        lblCodePatient.Name = "lblCodePatient"
        lblCodePatient.Size = New Size(12, 17)
        lblCodePatient.TabIndex = 2
        lblCodePatient.Text = "-"
        ' 
        ' lblNomComplet
        ' 
        lblNomComplet.AutoSize = True
        lblNomComplet.Font = New Font("Calibri", 18F, FontStyle.Bold)
        lblNomComplet.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNomComplet.Location = New Point(126, 16)
        lblNomComplet.Name = "lblNomComplet"
        lblNomComplet.Size = New Size(180, 29)
        lblNomComplet.TabIndex = 1
        lblNomComplet.Text = "Nouveau patient"
        ' 
        ' picPhoto
        ' 
        picPhoto.BackColor = Color.White
        picPhoto.BorderStyle = BorderStyle.FixedSingle
        picPhoto.Location = New Point(16, 16)
        picPhoto.Name = "picPhoto"
        picPhoto.Size = New Size(96, 96)
        picPhoto.SizeMode = PictureBoxSizeMode.Zoom
        picPhoto.TabIndex = 0
        picPhoto.TabStop = False
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
        picTitre.Location = New Point(16, 3)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(60, 52)
        picTitre.TabIndex = 4
        picTitre.TabStop = False
        ' 
        ' lblTop
        ' 
        lblTop.Font = New Font("Calibri", 12F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(249, 19)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(555, 22)
        lblTop.TabIndex = 3
        lblTop.Text = "Consultez, créez ou modifiez la fiche d'un patient."
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 18F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(71, 3)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(0, 4, 8, 4)
        lblTitreForm.Size = New Size(150, 37)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Fiche patient"
        ' 
        ' UC_PatientFiche
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_PatientFiche"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        tabFiche.ResumeLayout(False)
        tabPageIdentite.ResumeLayout(False)
        grpAlerte.ResumeLayout(False)
        grpAdministratif.ResumeLayout(False)
        grpAdministratif.PerformLayout()
        grpCoordonnees.ResumeLayout(False)
        grpCoordonnees.PerformLayout()
        grpIdentite.ResumeLayout(False)
        grpIdentite.PerformLayout()
        tabPageAnamnese.ResumeLayout(False)
        grpAnamnese.ResumeLayout(False)
        pnlAnamneseActions.ResumeLayout(False)
        tabPageFamille.ResumeLayout(False)
        CType(dgvContacts, ComponentModel.ISupportInitialize).EndInit()
        pnlFamilleRecherche.ResumeLayout(False)
        pnlFamilleRecherche.PerformLayout()
        pnlFamilleActions.ResumeLayout(False)
        tabPageIntervenants.ResumeLayout(False)
        CType(dgvIntervenants, ComponentModel.ISupportInitialize).EndInit()
        pnlIntervenantsRecherche.ResumeLayout(False)
        pnlIntervenantsRecherche.PerformLayout()
        pnlIntervenantsActions.ResumeLayout(False)
        tabPageDossiers.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        pnlBandeau.ResumeLayout(False)
        pnlBandeau.PerformLayout()
        CType(picPhoto, ComponentModel.ISupportInitialize).EndInit()
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTop As Label
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents pnlBandeau As Panel
    Friend WithEvents picPhoto As PictureBox
    Friend WithEvents btnUploadPhoto As Button
    Friend WithEvents lblNomComplet As Label
    Friend WithEvents lblCodePatient As Label
    Friend WithEvents rtbAlerte As RichTextBox
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents tabFiche As TabControl
    Friend WithEvents tabPageIdentite As TabPage
    Friend WithEvents grpIdentite As GroupBox
    Friend WithEvents lblNom As Label
    Friend WithEvents txtNom As TextBox
    Friend WithEvents lblPrenom As Label
    Friend WithEvents txtPrenom As TextBox
    Friend WithEvents lblDateNaissance As Label
    Friend WithEvents dtpDateNaissance As DateTimePicker
    Friend WithEvents lblAge As Label
    Friend WithEvents lblNiss As Label
    Friend WithEvents txtNiss As TextBox
    Friend WithEvents lblLateralite As Label
    Friend WithEvents cboLateralite As ComboBox
    Friend WithEvents grpCoordonnees As GroupBox
    Friend WithEvents lblAdresseLigne1 As Label
    Friend WithEvents txtAdresseLigne1 As TextBox
    Friend WithEvents lblAdresseLigne2 As Label
    Friend WithEvents txtAdresseLigne2 As TextBox
    Friend WithEvents lblCodePostal As Label
    Friend WithEvents txtCodePostal As TextBox
    Friend WithEvents lblLocalite As Label
    Friend WithEvents txtLocalite As TextBox
    Friend WithEvents lblPays As Label
    Friend WithEvents cboPays As ComboBox
    Friend WithEvents lblTelephone As Label
    Friend WithEvents txtTelephone As TextBox
    Friend WithEvents lblEmail As Label
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents grpAdministratif As GroupBox
    Friend WithEvents lblMutualite As Label
    Friend WithEvents txtMutualite As TextBox
    Friend WithEvents lblSituationFamiliale As Label
    Friend WithEvents cboSituationFamiliale As ComboBox
    Friend WithEvents btnAjouterSituationFamiliale As Button
    Friend WithEvents grpAlerte As GroupBox
    Friend WithEvents rteAlerte As UC_RichTextEditorSimple
    Friend WithEvents tabPageAnamnese As TabPage
    Friend WithEvents grpAnamnese As GroupBox
    Friend WithEvents rteAnamnese As UC_RichTextEditor
    Friend WithEvents pnlAnamneseActions As Panel
    Friend WithEvents btnModifierAnamnese As Button
    Friend WithEvents btnEnregistrerAnamnese As Button
    Friend WithEvents btnAnnulerAnamnese As Button
    Friend WithEvents tabPageFamille As TabPage
    Friend WithEvents dgvContacts As DataGridView
    Friend WithEvents colContactLien As DataGridViewTextBoxColumn
    Friend WithEvents colContactNom As DataGridViewTextBoxColumn
    Friend WithEvents colContactPrenom As DataGridViewTextBoxColumn
    Friend WithEvents colContactTelephone As DataGridViewTextBoxColumn
    Friend WithEvents colContactRole As DataGridViewTextBoxColumn
    Friend WithEvents pnlFamilleActions As Panel
    Friend WithEvents btnAjouterContact As Button
    Friend WithEvents btnModifierContact As Button
    Friend WithEvents btnSupprimerContact As Button
    Friend WithEvents pnlFamilleRecherche As Panel
    Friend WithEvents lblRechercheContact As Label
    Friend WithEvents txtRechercheContact As TextBox
    Friend WithEvents btnRechercherContact As Button
    Friend WithEvents btnReinitialiserContacts As Button
    Friend WithEvents tabPageIntervenants As TabPage
    Friend WithEvents dgvIntervenants As DataGridView
    Friend WithEvents colIntervenantRole As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantNom As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantSpecialite As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantLieu As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantDateDebut As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantDateFin As DataGridViewTextBoxColumn
    Friend WithEvents pnlIntervenantsActions As Panel
    Friend WithEvents btnAjouterIntervenant As Button
    Friend WithEvents btnModifierIntervenant As Button
    Friend WithEvents btnSupprimerIntervenant As Button
    Friend WithEvents pnlIntervenantsRecherche As Panel
    Friend WithEvents lblRechercheIntervenant As Label
    Friend WithEvents txtRechercheIntervenant As TextBox
    Friend WithEvents btnRechercherIntervenant As Button
    Friend WithEvents btnReinitialiserIntervenants As Button
    Friend WithEvents tabPageDossiers As TabPage
    Friend WithEvents lblDossiersAVenir As Label
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnNouveau As Button
    Friend WithEvents btnModifier As Button
    Friend WithEvents btnEnregistrer As Button
    Friend WithEvents btnAnnuler As Button
    Friend WithEvents btnFermer As Button
    Friend WithEvents picTitre As PictureBox

End Class
