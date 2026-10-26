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
        tabFiche = New AltheaOnglets()
        tabPageIdentite = New TabPage()
        grpAlerte = New AltheaCarte()
        rteAlerte = New UC_RichTextEditor()
        grpAdministratif = New AltheaCarte()
        btnAjouterSituationFamiliale = New AltheaBouton()
        cboSituationFamiliale = New AltheaListe()
        lblSituationFamiliale = New Label()
        cboPrixSeanceDefaut = New AltheaListe()
        lblPrixSeanceDefaut = New Label()
        cboMutualite = New AltheaListe()
        btnFicheMutualite = New AltheaBouton()
        lblAffiliation = New Label()
        txtNumeroAffiliation = New TextBox()
        lblCodesTitulaire = New Label()
        txtCodeTitulaire1 = New TextBox()
        txtCodeTitulaire2 = New TextBox()
        lblMutualite = New Label()
        grpCoordonnees = New AltheaCarte()
        txtEmail = New TextBox()
        lblEmail = New Label()
        txtTelephone = New TextBox()
        lblTelephone = New Label()
        cboPays = New AltheaListe()
        lblPays = New Label()
        txtLocalite = New TextBox()
        lblLocalite = New Label()
        txtCodePostal = New TextBox()
        lblCodePostal = New Label()
        txtAdresseLigne2 = New TextBox()
        lblAdresseLigne2 = New Label()
        txtAdresseLigne1 = New TextBox()
        lblAdresseLigne1 = New Label()
        grpIdentite = New AltheaCarte()
        cboLateralite = New AltheaListe()
        cboCivilite = New AltheaListe()
        lblCivilite = New Label()
        lblLateralite = New Label()
        txtNiss = New TextBox()
        cboTypeNiss = New AltheaListe()
        lblNiss = New Label()
        dtpDateNaissance = New AltheaDate()
        lblDateNaissance = New Label()
        lblAge = New Label()
        txtPrenom = New TextBox()
        lblPrenom = New Label()
        txtNom = New TextBox()
        lblNom = New Label()
        tabPageAnamnese = New TabPage()
        pnlAnamnese = New Panel()
        rteAnamnese = New UC_RichTextEditor()
        carteAnamnese = New AltheaCarte()
        pnlAnamneseActions = New AltheaBarreOutils()
        btnAnnulerAnamnese = New AltheaBouton()
        btnEnregistrerAnamnese = New AltheaBouton()
        btnModifierAnamnese = New AltheaBouton()
        tabPageFamille = New TabPage()
        dgvContacts = New DataGridView()
        colContactLien = New DataGridViewTextBoxColumn()
        colContactNom = New DataGridViewTextBoxColumn()
        colContactPrenom = New DataGridViewTextBoxColumn()
        colContactTelephone = New DataGridViewTextBoxColumn()
        colContactRole = New DataGridViewTextBoxColumn()
        colContactCode = New DataGridViewTextBoxColumn()
        pagContacts = New UC_Pagination()
        pnlFamilleRecherche = New Panel()
        carteRechercheContacts = New AltheaCarte()
        txtRechercheContact = New AltheaRecherche()
        btnRechercherContact = New AltheaBouton()
        btnReinitialiserContacts = New AltheaBouton()
        carteContacts = New AltheaCarte()
        pnlFamilleActions = New AltheaBarreOutils()
        btnSupprimerContact = New AltheaBouton()
        btnModifierContact = New AltheaBouton()
        btnAjouterContact = New AltheaBouton()
        tabPageIntervenants = New TabPage()
        dgvIntervenants = New DataGridView()
        colIntervenantRole = New DataGridViewTextBoxColumn()
        colIntervenantNom = New DataGridViewTextBoxColumn()
        colIntervenantSpecialite = New DataGridViewTextBoxColumn()
        colIntervenantLieu = New DataGridViewTextBoxColumn()
        colIntervenantDateDebut = New DataGridViewTextBoxColumn()
        colIntervenantDateFin = New DataGridViewTextBoxColumn()
        colIntervenantCode = New DataGridViewTextBoxColumn()
        pagIntervenants = New UC_Pagination()
        pnlIntervenantsRecherche = New Panel()
        carteRechercheIntervenants = New AltheaCarte()
        txtRechercheIntervenant = New AltheaRecherche()
        btnRechercherIntervenant = New AltheaBouton()
        btnReinitialiserIntervenants = New AltheaBouton()
        carteIntervenants = New AltheaCarte()
        pnlIntervenantsActions = New AltheaBarreOutils()
        btnSupprimerIntervenant = New AltheaBouton()
        btnModifierIntervenant = New AltheaBouton()
        btnAjouterIntervenant = New AltheaBouton()
        tabPageDossiers = New TabPage()
        dgvDossiers = New DataGridView()
        colDossierCode = New DataGridViewTextBoxColumn()
        colDossierDomaine = New DataGridViewTextBoxColumn()
        colDossierStatut = New DataGridViewTextBoxColumn()
        colDossierDateOuverture = New DataGridViewTextBoxColumn()
        colDossierDateCloture = New DataGridViewTextBoxColumn()
        carteDossiers = New AltheaCarte()
        pnlDossiersActions = New AltheaBarreOutils()
        btnOuvrirDossier = New AltheaBouton()
        btnAjouterDossier = New AltheaBouton()
        pnlActions = New AltheaBarreActions()
        btnFermer = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        btnEnregistrer = New AltheaBouton()
        btnModifier = New AltheaBouton()
        btnNouveau = New AltheaBouton()
        pnlBandeau = New Panel()
        carteIdentite = New AltheaCarte()
        pnlAlerteBandeau = New Panel()
        lblAlerteBandeauTitre = New Label()
        pnlAlerteFond = New Panel()
        lblNaissanceBandeau = New Label()
        rtbAlerte = New RichTextBox()
        btnUploadPhoto = New AltheaBouton()
        lblCodePatient = New AltheaPastille()
        lblNomComplet = New Label()
        lblStatutPatient = New AltheaPastille()
        btnBasculeActif = New AltheaBouton()
        picPhoto = New PictureBox()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        tabFiche.SuspendLayout()
        tabPageIdentite.SuspendLayout()
        grpAlerte.SuspendLayout()
        grpAdministratif.SuspendLayout()
        grpCoordonnees.SuspendLayout()
        grpIdentite.SuspendLayout()
        tabPageAnamnese.SuspendLayout()
        pnlAnamnese.SuspendLayout()
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
        CType(dgvDossiers, ComponentModel.ISupportInitialize).BeginInit()
        pnlDossiersActions.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlBandeau.SuspendLayout()
        carteRechercheIntervenants.SuspendLayout()
        carteRechercheContacts.SuspendLayout()
        carteIdentite.SuspendLayout()
        pnlAlerteBandeau.SuspendLayout()
        pnlAlerteFond.SuspendLayout()
        CType(picPhoto, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(pnlBandeau)
        pnlForm.Controls.Add(enTete)
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
        tabFiche.Controls.Add(tabPageIdentite)
        tabFiche.Controls.Add(tabPageAnamnese)
        tabFiche.Controls.Add(tabPageFamille)
        tabFiche.Controls.Add(tabPageIntervenants)
        tabFiche.Controls.Add(tabPageDossiers)
        tabFiche.CouleurFond = Color.Transparent
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
        grpAlerte.Font = New Font("Calibri", 11F, FontStyle.Bold)
        grpAlerte.Name = "grpAlerte"
        grpAlerte.TabIndex = 3
        grpAlerte.TabStop = False
        grpAlerte.Location = New Point(486, 280)
        grpAlerte.Padding = New Padding(12, 8, 12, 10)
        grpAlerte.Size = New Size(420, 150)
        grpAlerte.Titre = "Alerte / Notes"
        ' 
        ' rteAlerte
        ' 
        rteAlerte.BackColor = Color.Transparent
        rteAlerte.Dock = DockStyle.Fill
        rteAlerte.Location = New Point(8, 22)
        rteAlerte.Margin = New Padding(4, 3, 4, 3)
        rteAlerte.Name = "rteAlerte"
        rteAlerte.Profil = ProfilEditeur.Compact
        rteAlerte.TitreDocument = "Alerte / Notes"
        rteAlerte.Size = New Size(874, 52)
        rteAlerte.TabIndex = 0
        ' 
        ' grpAdministratif
        ' 
        grpAdministratif.Controls.Add(txtCodeTitulaire2)
        grpAdministratif.Controls.Add(txtCodeTitulaire1)
        grpAdministratif.Controls.Add(lblCodesTitulaire)
        grpAdministratif.Controls.Add(txtNumeroAffiliation)
        grpAdministratif.Controls.Add(lblAffiliation)
        grpAdministratif.Controls.Add(btnFicheMutualite)
        grpAdministratif.Controls.Add(btnAjouterSituationFamiliale)
        grpAdministratif.Controls.Add(cboSituationFamiliale)
        grpAdministratif.Controls.Add(lblSituationFamiliale)
        grpAdministratif.Controls.Add(cboPrixSeanceDefaut)
        grpAdministratif.Controls.Add(lblPrixSeanceDefaut)
        grpAdministratif.Controls.Add(cboMutualite)
        grpAdministratif.Controls.Add(lblMutualite)
        grpAdministratif.Name = "grpAdministratif"
        grpAdministratif.TabIndex = 2
        grpAdministratif.TabStop = False
        grpAdministratif.Location = New Point(16, 251)
        grpAdministratif.Padding = New Padding(12, 8, 12, 10)
        grpAdministratif.Size = New Size(455, 173)
        grpAdministratif.Titre = "Administratif"
        ' 
        ' txtCodeTitulaire2
        ' 
        txtCodeTitulaire2.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCodeTitulaire2.Font = New Font("Calibri", 11F)
        txtCodeTitulaire2.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCodeTitulaire2.Location = New Point(393, 73)
        txtCodeTitulaire2.MaxLength = 3
        txtCodeTitulaire2.Name = "txtCodeTitulaire2"
        txtCodeTitulaire2.Size = New Size(38, 25)
        txtCodeTitulaire2.TabIndex = 7
        ' 
        ' txtCodeTitulaire1
        ' 
        txtCodeTitulaire1.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCodeTitulaire1.Font = New Font("Calibri", 11F)
        txtCodeTitulaire1.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCodeTitulaire1.Location = New Point(350, 73)
        txtCodeTitulaire1.MaxLength = 3
        txtCodeTitulaire1.Name = "txtCodeTitulaire1"
        txtCodeTitulaire1.Size = New Size(38, 25)
        txtCodeTitulaire1.TabIndex = 6
        ' 
        ' lblCodesTitulaire
        ' 
        lblCodesTitulaire.AutoSize = True
        lblCodesTitulaire.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCodesTitulaire.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCodesTitulaire.Location = New Point(284, 76)
        lblCodesTitulaire.Name = "lblCodesTitulaire"
        lblCodesTitulaire.Size = New Size(62, 17)
        lblCodesTitulaire.TabIndex = 5
        lblCodesTitulaire.Text = "CT1 / CT2"
        ' 
        ' txtNumeroAffiliation
        ' 
        txtNumeroAffiliation.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNumeroAffiliation.Font = New Font("Calibri", 11F)
        txtNumeroAffiliation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNumeroAffiliation.Location = New Point(140, 73)
        txtNumeroAffiliation.MaxLength = 30
        txtNumeroAffiliation.Name = "txtNumeroAffiliation"
        txtNumeroAffiliation.Size = New Size(130, 25)
        txtNumeroAffiliation.TabIndex = 4
        ' 
        ' lblAffiliation
        ' 
        lblAffiliation.AutoSize = True
        lblAffiliation.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblAffiliation.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAffiliation.Location = New Point(16, 76)
        lblAffiliation.Name = "lblAffiliation"
        lblAffiliation.Size = New Size(70, 17)
        lblAffiliation.TabIndex = 3
        lblAffiliation.Text = "Affiliation"
        ' 
        ' btnFicheMutualite
        ' 
        btnFicheMutualite.Image = CType(resources.GetObject("btnFicheMutualite.Image"), Image)
        btnFicheMutualite.Location = New Point(404, 40)
        btnFicheMutualite.Name = "btnFicheMutualite"
        btnFicheMutualite.Size = New Size(30, 30)
        btnFicheMutualite.TabIndex = 2
        btnFicheMutualite.Tag = "recherche_24_normal"
        btnFicheMutualite.Variante = VarianteBouton.Outil
        ' 
        ' btnAjouterSituationFamiliale
        ' 
        btnAjouterSituationFamiliale.Image = CType(resources.GetObject("btnAjouterSituationFamiliale.Image"), Image)
        btnAjouterSituationFamiliale.Location = New Point(404, 102)
        btnAjouterSituationFamiliale.Name = "btnAjouterSituationFamiliale"
        btnAjouterSituationFamiliale.Size = New Size(30, 30)
        btnAjouterSituationFamiliale.TabIndex = 10
        btnAjouterSituationFamiliale.Tag = "plus_24_normal"
        btnAjouterSituationFamiliale.Variante = VarianteBouton.Outil
        ' 
        ' cboSituationFamiliale
        ' 
        cboSituationFamiliale.Font = New Font("Calibri", 11F)
        cboSituationFamiliale.ItemHeight = 20
        cboSituationFamiliale.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboSituationFamiliale.Location = New Point(140, 104)
        cboSituationFamiliale.Name = "cboSituationFamiliale"
        cboSituationFamiliale.Size = New Size(260, 26)
        cboSituationFamiliale.TabIndex = 9
        ' 
        ' lblSituationFamiliale
        ' 
        lblSituationFamiliale.AutoSize = True
        lblSituationFamiliale.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblSituationFamiliale.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblSituationFamiliale.Location = New Point(16, 107)
        lblSituationFamiliale.Name = "lblSituationFamiliale"
        lblSituationFamiliale.Size = New Size(113, 17)
        lblSituationFamiliale.TabIndex = 8
        lblSituationFamiliale.Text = "Situation familiale"
        ' 
        ' cboPrixSeanceDefaut
        ' 
        cboPrixSeanceDefaut.Font = New Font("Calibri", 11F)
        cboPrixSeanceDefaut.ItemHeight = 20
        cboPrixSeanceDefaut.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboPrixSeanceDefaut.Location = New Point(140, 135)
        cboPrixSeanceDefaut.Name = "cboPrixSeanceDefaut"
        cboPrixSeanceDefaut.Size = New Size(260, 26)
        cboPrixSeanceDefaut.TabIndex = 12
        ' 
        ' lblPrixSeanceDefaut
        ' 
        lblPrixSeanceDefaut.AutoSize = True
        lblPrixSeanceDefaut.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPrixSeanceDefaut.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPrixSeanceDefaut.Location = New Point(16, 138)
        lblPrixSeanceDefaut.Name = "lblPrixSeanceDefaut"
        lblPrixSeanceDefaut.Size = New Size(98, 17)
        lblPrixSeanceDefaut.TabIndex = 11
        lblPrixSeanceDefaut.Text = "Tarif par défaut"
        ' 
        ' cboMutualite
        ' 
        cboMutualite.Font = New Font("Calibri", 11F)
        cboMutualite.ItemHeight = 20
        cboMutualite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboMutualite.Location = New Point(140, 42)
        cboMutualite.Name = "cboMutualite"
        cboMutualite.Size = New Size(260, 26)
        cboMutualite.TabIndex = 1
        ' 
        ' lblMutualite
        ' 
        lblMutualite.AutoSize = True
        lblMutualite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblMutualite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblMutualite.Location = New Point(16, 45)
        lblMutualite.Name = "lblMutualite"
        lblMutualite.Size = New Size(66, 17)
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
        grpCoordonnees.Name = "grpCoordonnees"
        grpCoordonnees.TabIndex = 1
        grpCoordonnees.TabStop = False
        grpCoordonnees.Location = New Point(486, 8)
        grpCoordonnees.Padding = New Padding(12, 8, 12, 10)
        grpCoordonnees.Size = New Size(420, 264)
        grpCoordonnees.Titre = "Coordonnées"
        ' 
        ' txtEmail
        ' 
        txtEmail.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtEmail.Font = New Font("Calibri", 11F)
        txtEmail.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtEmail.Location = New Point(120, 228)
        txtEmail.MaxLength = 150
        txtEmail.Name = "txtEmail"
        txtEmail.Size = New Size(280, 25)
        txtEmail.TabIndex = 13
        ' 
        ' lblEmail
        ' 
        lblEmail.AutoSize = True
        lblEmail.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblEmail.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblEmail.Location = New Point(16, 231)
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
        txtTelephone.Location = New Point(120, 197)
        txtTelephone.MaxLength = 50
        txtTelephone.Name = "txtTelephone"
        txtTelephone.Size = New Size(200, 25)
        txtTelephone.TabIndex = 11
        ' 
        ' lblTelephone
        ' 
        lblTelephone.AutoSize = True
        lblTelephone.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTelephone.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTelephone.Location = New Point(16, 200)
        lblTelephone.Name = "lblTelephone"
        lblTelephone.Size = New Size(70, 17)
        lblTelephone.TabIndex = 10
        lblTelephone.Text = "Téléphone"
        ' 
        ' cboPays
        ' 
        cboPays.Font = New Font("Calibri", 11F)
        cboPays.ItemHeight = 20
        cboPays.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboPays.Location = New Point(120, 166)
        cboPays.Name = "cboPays"
        cboPays.Size = New Size(200, 26)
        cboPays.TabIndex = 9
        ' 
        ' lblPays
        ' 
        lblPays.AutoSize = True
        lblPays.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPays.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPays.Location = New Point(16, 169)
        lblPays.Name = "lblPays"
        lblPays.Size = New Size(35, 17)
        lblPays.TabIndex = 8
        lblPays.Text = "Pays"
        ' 
        ' txtLocalite
        ' 
        txtLocalite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtLocalite.Font = New Font("Calibri", 11F)
        txtLocalite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLocalite.Location = New Point(120, 135)
        txtLocalite.MaxLength = 100
        txtLocalite.Name = "txtLocalite"
        txtLocalite.Size = New Size(280, 25)
        txtLocalite.TabIndex = 7
        ' 
        ' lblLocalite
        ' 
        lblLocalite.AutoSize = True
        lblLocalite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblLocalite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLocalite.Location = New Point(16, 138)
        lblLocalite.Name = "lblLocalite"
        lblLocalite.Size = New Size(53, 17)
        lblLocalite.TabIndex = 6
        lblLocalite.Text = "Localité"
        ' 
        ' txtCodePostal
        ' 
        txtCodePostal.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCodePostal.Font = New Font("Calibri", 11F)
        txtCodePostal.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCodePostal.Location = New Point(120, 104)
        txtCodePostal.MaxLength = 20
        txtCodePostal.Name = "txtCodePostal"
        txtCodePostal.Size = New Size(120, 25)
        txtCodePostal.TabIndex = 5
        ' 
        ' lblCodePostal
        ' 
        lblCodePostal.AutoSize = True
        lblCodePostal.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCodePostal.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCodePostal.Location = New Point(16, 107)
        lblCodePostal.Name = "lblCodePostal"
        lblCodePostal.Size = New Size(78, 17)
        lblCodePostal.TabIndex = 4
        lblCodePostal.Text = "Code postal"
        ' 
        ' txtAdresseLigne2
        ' 
        txtAdresseLigne2.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdresseLigne2.Font = New Font("Calibri", 11F)
        txtAdresseLigne2.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdresseLigne2.Location = New Point(120, 73)
        txtAdresseLigne2.MaxLength = 150
        txtAdresseLigne2.Name = "txtAdresseLigne2"
        txtAdresseLigne2.Size = New Size(280, 25)
        txtAdresseLigne2.TabIndex = 3
        ' 
        ' lblAdresseLigne2
        ' 
        lblAdresseLigne2.AutoSize = True
        lblAdresseLigne2.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblAdresseLigne2.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAdresseLigne2.Location = New Point(16, 76)
        lblAdresseLigne2.Name = "lblAdresseLigne2"
        lblAdresseLigne2.Size = New Size(73, 17)
        lblAdresseLigne2.TabIndex = 2
        lblAdresseLigne2.Text = "Adresse (2)"
        ' 
        ' txtAdresseLigne1
        ' 
        txtAdresseLigne1.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdresseLigne1.Font = New Font("Calibri", 11F)
        txtAdresseLigne1.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdresseLigne1.Location = New Point(120, 42)
        txtAdresseLigne1.MaxLength = 150
        txtAdresseLigne1.Name = "txtAdresseLigne1"
        txtAdresseLigne1.Size = New Size(280, 25)
        txtAdresseLigne1.TabIndex = 1
        ' 
        ' lblAdresseLigne1
        ' 
        lblAdresseLigne1.AutoSize = True
        lblAdresseLigne1.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblAdresseLigne1.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAdresseLigne1.Location = New Point(16, 45)
        lblAdresseLigne1.Name = "lblAdresseLigne1"
        lblAdresseLigne1.Size = New Size(73, 17)
        lblAdresseLigne1.TabIndex = 0
        lblAdresseLigne1.Text = "Adresse (1)"
        ' 
        ' grpIdentite
        ' 
        grpIdentite.Controls.Add(lblCivilite)
        grpIdentite.Controls.Add(cboCivilite)
        grpIdentite.Controls.Add(cboLateralite)
        grpIdentite.Controls.Add(lblLateralite)
        grpIdentite.Controls.Add(txtNiss)
        grpIdentite.Controls.Add(cboTypeNiss)
        grpIdentite.Controls.Add(lblNiss)
        grpIdentite.Controls.Add(dtpDateNaissance)
        grpIdentite.Controls.Add(lblDateNaissance)
        grpIdentite.Controls.Add(lblAge)
        grpIdentite.Controls.Add(txtPrenom)
        grpIdentite.Controls.Add(lblPrenom)
        grpIdentite.Controls.Add(txtNom)
        grpIdentite.Controls.Add(lblNom)
        grpIdentite.Name = "grpIdentite"
        grpIdentite.TabIndex = 0
        grpIdentite.TabStop = False
        grpIdentite.Location = New Point(16, 8)
        grpIdentite.Padding = New Padding(12, 8, 12, 10)
        grpIdentite.Size = New Size(455, 235)
        grpIdentite.Titre = "Identité civile"
        ' 
        ' lblCivilite
        ' 
        lblCivilite.AutoSize = True
        lblCivilite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCivilite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCivilite.Location = New Point(16, 45)
        lblCivilite.Name = "lblCivilite"
        lblCivilite.Size = New Size(50, 17)
        lblCivilite.TabIndex = 0
        lblCivilite.Text = "Civilité"
        ' 
        ' cboCivilite
        ' 
        cboCivilite.Font = New Font("Calibri", 11F)
        cboCivilite.ItemHeight = 20
        cboCivilite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboCivilite.Location = New Point(140, 42)
        cboCivilite.Name = "cboCivilite"
        cboCivilite.Size = New Size(160, 26)
        cboCivilite.TabIndex = 1
        ' 
        ' cboLateralite
        ' 
        cboLateralite.Font = New Font("Calibri", 11F)
        cboLateralite.ItemHeight = 20
        cboLateralite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboLateralite.Location = New Point(140, 197)
        cboLateralite.Name = "cboLateralite"
        cboLateralite.Size = New Size(160, 26)
        cboLateralite.TabIndex = 11
        ' 
        ' lblLateralite
        ' 
        lblLateralite.AutoSize = True
        lblLateralite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblLateralite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLateralite.Location = New Point(16, 200)
        lblLateralite.Name = "lblLateralite"
        lblLateralite.Size = New Size(63, 17)
        lblLateralite.TabIndex = 10
        lblLateralite.Text = "Latéralité"
        ' 
        ' txtNiss
        ' 
        txtNiss.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNiss.Font = New Font("Calibri", 11F)
        txtNiss.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNiss.Location = New Point(140, 166)
        txtNiss.MaxLength = 20
        txtNiss.Name = "txtNiss"
        txtNiss.Size = New Size(170, 25)
        txtNiss.TabIndex = 9
        ' 
        ' cboTypeNiss
        ' 
        cboTypeNiss.Font = New Font("Calibri", 11F)
        cboTypeNiss.ItemHeight = 20
        cboTypeNiss.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboTypeNiss.Location = New Point(315, 166)
        cboTypeNiss.Name = "cboTypeNiss"
        cboTypeNiss.Size = New Size(125, 26)
        cboTypeNiss.TabIndex = 10
        ' 
        ' lblNiss
        ' 
        lblNiss.AutoSize = True
        lblNiss.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNiss.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNiss.Location = New Point(16, 169)
        lblNiss.Name = "lblNiss"
        lblNiss.Size = New Size(35, 17)
        lblNiss.TabIndex = 8
        lblNiss.Text = "NISS"
        ' 
        ' dtpDateNaissance
        ' 
        dtpDateNaissance.Facultative = True
        dtpDateNaissance.Font = New Font("Calibri", 11F)
        dtpDateNaissance.Location = New Point(140, 135)
        dtpDateNaissance.Name = "dtpDateNaissance"
        dtpDateNaissance.Size = New Size(160, 26)
        dtpDateNaissance.TabIndex = 7
        ' 
        ' lblDateNaissance
        ' 
        lblDateNaissance.AutoSize = True
        lblDateNaissance.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDateNaissance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateNaissance.Location = New Point(16, 138)
        lblDateNaissance.Name = "lblDateNaissance"
        lblDateNaissance.Size = New Size(97, 17)
        lblDateNaissance.TabIndex = 6
        lblDateNaissance.Text = "Date naissance"
        ' 
        ' lblAge
        ' 
        lblAge.AutoSize = True
        lblAge.Font = New Font("Calibri", 10.5F, FontStyle.Italic)
        lblAge.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAge.Location = New Point(310, 138)
        lblAge.Name = "lblAge"
        lblAge.Size = New Size(0, 17)
        lblAge.TabIndex = 12
        ' 
        ' txtPrenom
        ' 
        txtPrenom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtPrenom.Font = New Font("Calibri", 11F)
        txtPrenom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPrenom.Location = New Point(140, 104)
        txtPrenom.MaxLength = 100
        txtPrenom.Name = "txtPrenom"
        txtPrenom.Size = New Size(290, 25)
        txtPrenom.TabIndex = 5
        ' 
        ' lblPrenom
        ' 
        lblPrenom.AutoSize = True
        lblPrenom.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPrenom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPrenom.Location = New Point(16, 107)
        lblPrenom.Name = "lblPrenom"
        lblPrenom.Size = New Size(54, 17)
        lblPrenom.TabIndex = 4
        lblPrenom.Text = "Prénom"
        ' 
        ' txtNom
        ' 
        txtNom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNom.Font = New Font("Calibri", 11F)
        txtNom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNom.Location = New Point(140, 73)
        txtNom.MaxLength = 100
        txtNom.Name = "txtNom"
        txtNom.Size = New Size(290, 25)
        txtNom.TabIndex = 3
        ' 
        ' lblNom
        ' 
        lblNom.AutoSize = True
        lblNom.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNom.Location = New Point(16, 76)
        lblNom.Name = "lblNom"
        lblNom.Size = New Size(36, 17)
        lblNom.TabIndex = 2
        lblNom.Text = "Nom"
        ' 
        ' tabPageAnamnese
        ' 
        tabPageAnamnese.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageAnamnese.Controls.Add(carteAnamnese)
        tabPageAnamnese.Location = New Point(4, 36)
        tabPageAnamnese.Name = "tabPageAnamnese"
        tabPageAnamnese.Padding = New Padding(8)
        tabPageAnamnese.Size = New Size(925, 438)
        tabPageAnamnese.TabIndex = 4
        tabPageAnamnese.Text = "Anamnèse"
        ' 
        ' pnlAnamnese
        ' 
        pnlAnamnese.Controls.Add(rteAnamnese)
        pnlAnamnese.Dock = DockStyle.Fill
        pnlAnamnese.Font = New Font("Calibri", 11F, FontStyle.Bold)
        pnlAnamnese.Location = New Point(8, 8)
        pnlAnamnese.Name = "pnlAnamnese"
        pnlAnamnese.Padding = New Padding(8, 4, 8, 8)
        pnlAnamnese.Size = New Size(909, 376)
        pnlAnamnese.TabIndex = 0
        pnlAnamnese.BackColor = Color.Transparent
        ' 
        ' rteAnamnese
        ' 
        rteAnamnese.BackColor = Color.Transparent
        rteAnamnese.Dock = DockStyle.Fill
        rteAnamnese.Location = New Point(8, 22)
        rteAnamnese.Margin = New Padding(4, 3, 4, 3)
        rteAnamnese.Name = "rteAnamnese"
        rteAnamnese.TitreDocument = "Anamnèse"
        rteAnamnese.Size = New Size(893, 346)
        rteAnamnese.TabIndex = 0
        ' 
        ' carteAnamnese
        ' 
        carteAnamnese.Controls.Add(pnlAnamnese)
        carteAnamnese.Controls.Add(pnlAnamneseActions)
        carteAnamnese.Dock = DockStyle.Fill
        carteAnamnese.Name = "carteAnamnese"
        carteAnamnese.Padding = New Padding(6, 2, 6, 6)
        carteAnamnese.TabIndex = 1
        ' 
        ' pnlAnamneseActions
        ' 
        pnlAnamneseActions.Controls.Add(btnAnnulerAnamnese)
        pnlAnamneseActions.Controls.Add(btnEnregistrerAnamnese)
        pnlAnamneseActions.Controls.Add(btnModifierAnamnese)
        pnlAnamneseActions.Dock = DockStyle.Top
        pnlAnamneseActions.Name = "pnlAnamneseActions"
        pnlAnamneseActions.Size = New Size(600, 42)
        pnlAnamneseActions.TabIndex = 1
        ' 
        ' btnAnnulerAnamnese
        ' 
        btnAnnulerAnamnese.Image = CType(resources.GetObject("btnAnnulerAnamnese.Image"), Image)
        btnAnnulerAnamnese.Location = New Point(140, 5)
        btnAnnulerAnamnese.Name = "btnAnnulerAnamnese"
        btnAnnulerAnamnese.Size = New Size(108, 30)
        btnAnnulerAnamnese.TabIndex = 2
        btnAnnulerAnamnese.Tag = "annuler_normal"
        btnAnnulerAnamnese.Text = "Annuler"
        btnAnnulerAnamnese.Variante = VarianteBouton.Outil
        ' 
        ' btnEnregistrerAnamnese
        ' 
        btnEnregistrerAnamnese.Image = CType(resources.GetObject("btnEnregistrerAnamnese.Image"), Image)
        btnEnregistrerAnamnese.Location = New Point(10, 5)
        btnEnregistrerAnamnese.Name = "btnEnregistrerAnamnese"
        btnEnregistrerAnamnese.Size = New Size(126, 30)
        btnEnregistrerAnamnese.TabIndex = 1
        btnEnregistrerAnamnese.Tag = "enregistrer_normal"
        btnEnregistrerAnamnese.Text = "Enregistrer"
        btnEnregistrerAnamnese.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' btnModifierAnamnese
        ' 
        btnModifierAnamnese.Location = New Point(10, 5)
        btnModifierAnamnese.Name = "btnModifierAnamnese"
        btnModifierAnamnese.Size = New Size(126, 30)
        btnModifierAnamnese.TabIndex = 0
        btnModifierAnamnese.Tag = "modifier_normal"
        btnModifierAnamnese.Text = "Modifier"
        btnModifierAnamnese.Variante = VarianteBouton.Outil
        ' 
        ' tabPageFamille
        ' 
        tabPageFamille.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageFamille.Controls.Add(carteContacts)
        tabPageFamille.Controls.Add(pagContacts)
        tabPageFamille.Controls.Add(pnlFamilleRecherche)
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
        dgvContacts.Columns.AddRange(New DataGridViewColumn() {colContactLien, colContactNom, colContactPrenom, colContactTelephone, colContactRole, colContactCode})
        dgvContacts.Dock = DockStyle.Fill
        dgvContacts.EditMode = DataGridViewEditMode.EditProgrammatically
        dgvContacts.Font = New Font("Calibri", 10F)
        dgvContacts.Location = New Point(8, 52)
        dgvContacts.MultiSelect = False
        dgvContacts.Name = "dgvContacts"
        dgvContacts.ReadOnly = True
        dgvContacts.RowHeadersVisible = False
        dgvContacts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvContacts.Size = New Size(909, 302)
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
        ' colContactCode
        ' 
        colContactCode.DataPropertyName = "CodeFamilleContact"
        colContactCode.FillWeight = 70F
        colContactCode.HeaderText = "Code"
        colContactCode.Name = "colContactCode"
        colContactCode.ReadOnly = True
        ' 
        ' pagContacts
        ' 
        pagContacts.BackColor = Color.Transparent
        pagContacts.Dock = DockStyle.Bottom
        pagContacts.Location = New Point(8, 354)
        pagContacts.Name = "pagContacts"
        pagContacts.Size = New Size(909, 32)
        pagContacts.TabIndex = 3
        ' 
        ' pnlFamilleRecherche
        ' 
        pnlFamilleRecherche.Controls.Add(carteRechercheContacts)
        pnlFamilleRecherche.BackColor = Color.Transparent
        pnlFamilleRecherche.Dock = DockStyle.Top
        pnlFamilleRecherche.Location = New Point(8, 8)
        pnlFamilleRecherche.Name = "pnlFamilleRecherche"
        pnlFamilleRecherche.Padding = New Padding(0, 0, 0, 8)
        pnlFamilleRecherche.Size = New Size(909, 66)
        pnlFamilleRecherche.TabIndex = 0
        ' 
        ' carteRechercheContacts
        ' 
        carteRechercheContacts.Controls.Add(txtRechercheContact)
        carteRechercheContacts.Controls.Add(btnReinitialiserContacts)
        carteRechercheContacts.Controls.Add(btnRechercherContact)
        carteRechercheContacts.Dock = DockStyle.Fill
        carteRechercheContacts.Font = New Font("Calibri", 11F)
        carteRechercheContacts.Location = New Point(0, 0)
        carteRechercheContacts.Name = "carteRechercheContacts"
        carteRechercheContacts.Padding = New Padding(14, 10, 14, 10)
        carteRechercheContacts.Size = New Size(909, 58)
        carteRechercheContacts.TabIndex = 0
        ' 
        ' txtRechercheContact
        ' 
        txtRechercheContact.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtRechercheContact.Location = New Point(14, 13)
        txtRechercheContact.Name = "txtRechercheContact"
        txtRechercheContact.Size = New Size(590, 32)
        txtRechercheContact.TabIndex = 0
        txtRechercheContact.TexteAide = "Nom, prénom, lien…"
        ' 
        ' btnRechercherContact
        ' 
        btnRechercherContact.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnRechercherContact.Image = CType(resources.GetObject("btnRechercherContact.Image"), Image)
        btnRechercherContact.Location = New Point(760, 13)
        btnRechercherContact.Name = "btnRechercherContact"
        btnRechercherContact.Size = New Size(135, 32)
        btnRechercherContact.TabIndex = 2
        btnRechercherContact.Tag = "recherche_24_normal"
        btnRechercherContact.Text = "Rechercher"
        ' 
        ' btnReinitialiserContacts
        ' 
        btnReinitialiserContacts.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnReinitialiserContacts.Image = CType(resources.GetObject("btnReinitialiserContacts.Image"), Image)
        btnReinitialiserContacts.Location = New Point(620, 13)
        btnReinitialiserContacts.Name = "btnReinitialiserContacts"
        btnReinitialiserContacts.Size = New Size(130, 32)
        btnReinitialiserContacts.TabIndex = 1
        btnReinitialiserContacts.Tag = "reinitialiser_24_normal"
        btnReinitialiserContacts.Text = "Réinitialiser"
        btnReinitialiserContacts.Variante = VarianteBouton.Clair
        ' 
        ' carteContacts
        ' 
        carteContacts.Controls.Add(dgvContacts)
        carteContacts.Controls.Add(pnlFamilleActions)
        carteContacts.Dock = DockStyle.Fill
        carteContacts.Name = "carteContacts"
        carteContacts.Padding = New Padding(6, 2, 6, 6)
        carteContacts.TabIndex = 1
        ' 
        ' pnlFamilleActions
        ' 
        pnlFamilleActions.Controls.Add(btnSupprimerContact)
        pnlFamilleActions.Controls.Add(btnModifierContact)
        pnlFamilleActions.Controls.Add(btnAjouterContact)
        pnlFamilleActions.Dock = DockStyle.Top
        pnlFamilleActions.Name = "pnlFamilleActions"
        pnlFamilleActions.Size = New Size(600, 42)
        pnlFamilleActions.TabIndex = 1
        ' 
        ' btnSupprimerContact
        ' 
        btnSupprimerContact.Image = CType(resources.GetObject("btnSupprimerContact.Image"), Image)
        btnSupprimerContact.Location = New Point(250, 5)
        btnSupprimerContact.Name = "btnSupprimerContact"
        btnSupprimerContact.Size = New Size(124, 30)
        btnSupprimerContact.TabIndex = 2
        btnSupprimerContact.Tag = "supprimer_normal"
        btnSupprimerContact.Text = "Supprimer"
        btnSupprimerContact.Variante = VarianteBouton.OutilDanger
        ' 
        ' btnModifierContact
        ' 
        btnModifierContact.Image = CType(resources.GetObject("btnModifierContact.Image"), Image)
        btnModifierContact.Location = New Point(119, 5)
        btnModifierContact.Name = "btnModifierContact"
        btnModifierContact.Size = New Size(113, 30)
        btnModifierContact.TabIndex = 1
        btnModifierContact.Tag = "modifier_normal"
        btnModifierContact.Text = "Modifier"
        btnModifierContact.Variante = VarianteBouton.Outil
        ' 
        ' btnAjouterContact
        ' 
        btnAjouterContact.Image = CType(resources.GetObject("btnAjouterContact.Image"), Image)
        btnAjouterContact.Location = New Point(10, 5)
        btnAjouterContact.Name = "btnAjouterContact"
        btnAjouterContact.Size = New Size(105, 30)
        btnAjouterContact.TabIndex = 0
        btnAjouterContact.Tag = "ajouter_normal"
        btnAjouterContact.Text = "Ajouter"
        btnAjouterContact.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' tabPageIntervenants
        ' 
        tabPageIntervenants.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageIntervenants.Controls.Add(carteIntervenants)
        tabPageIntervenants.Controls.Add(pagIntervenants)
        tabPageIntervenants.Controls.Add(pnlIntervenantsRecherche)
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
        dgvIntervenants.Columns.AddRange(New DataGridViewColumn() {colIntervenantRole, colIntervenantNom, colIntervenantSpecialite, colIntervenantLieu, colIntervenantDateDebut, colIntervenantDateFin, colIntervenantCode})
        dgvIntervenants.Dock = DockStyle.Fill
        dgvIntervenants.EditMode = DataGridViewEditMode.EditProgrammatically
        dgvIntervenants.Font = New Font("Calibri", 10F)
        dgvIntervenants.Location = New Point(8, 52)
        dgvIntervenants.MultiSelect = False
        dgvIntervenants.Name = "dgvIntervenants"
        dgvIntervenants.ReadOnly = True
        dgvIntervenants.RowHeadersVisible = False
        dgvIntervenants.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvIntervenants.Size = New Size(909, 302)
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
        ' colIntervenantCode
        ' 
        colIntervenantCode.DataPropertyName = "CodeAutreSuiviPatient"
        colIntervenantCode.FillWeight = 70F
        colIntervenantCode.HeaderText = "Code"
        colIntervenantCode.Name = "colIntervenantCode"
        colIntervenantCode.ReadOnly = True
        ' 
        ' pagIntervenants
        ' 
        pagIntervenants.BackColor = Color.Transparent
        pagIntervenants.Dock = DockStyle.Bottom
        pagIntervenants.Location = New Point(8, 354)
        pagIntervenants.Name = "pagIntervenants"
        pagIntervenants.Size = New Size(909, 32)
        pagIntervenants.TabIndex = 3
        ' 
        ' pnlIntervenantsRecherche
        ' 
        pnlIntervenantsRecherche.Controls.Add(carteRechercheIntervenants)
        pnlIntervenantsRecherche.BackColor = Color.Transparent
        pnlIntervenantsRecherche.Dock = DockStyle.Top
        pnlIntervenantsRecherche.Location = New Point(8, 8)
        pnlIntervenantsRecherche.Name = "pnlIntervenantsRecherche"
        pnlIntervenantsRecherche.Padding = New Padding(0, 0, 0, 8)
        pnlIntervenantsRecherche.Size = New Size(909, 66)
        pnlIntervenantsRecherche.TabIndex = 0
        ' 
        ' carteRechercheIntervenants
        ' 
        carteRechercheIntervenants.Controls.Add(txtRechercheIntervenant)
        carteRechercheIntervenants.Controls.Add(btnReinitialiserIntervenants)
        carteRechercheIntervenants.Controls.Add(btnRechercherIntervenant)
        carteRechercheIntervenants.Dock = DockStyle.Fill
        carteRechercheIntervenants.Font = New Font("Calibri", 11F)
        carteRechercheIntervenants.Location = New Point(0, 0)
        carteRechercheIntervenants.Name = "carteRechercheIntervenants"
        carteRechercheIntervenants.Padding = New Padding(14, 10, 14, 10)
        carteRechercheIntervenants.Size = New Size(909, 58)
        carteRechercheIntervenants.TabIndex = 0
        ' 
        ' txtRechercheIntervenant
        ' 
        txtRechercheIntervenant.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtRechercheIntervenant.Location = New Point(14, 13)
        txtRechercheIntervenant.Name = "txtRechercheIntervenant"
        txtRechercheIntervenant.Size = New Size(590, 32)
        txtRechercheIntervenant.TabIndex = 0
        txtRechercheIntervenant.TexteAide = "Rôle, nom, spécialité…"
        ' 
        ' btnRechercherIntervenant
        ' 
        btnRechercherIntervenant.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnRechercherIntervenant.Image = CType(resources.GetObject("btnRechercherIntervenant.Image"), Image)
        btnRechercherIntervenant.Location = New Point(760, 13)
        btnRechercherIntervenant.Name = "btnRechercherIntervenant"
        btnRechercherIntervenant.Size = New Size(135, 32)
        btnRechercherIntervenant.TabIndex = 2
        btnRechercherIntervenant.Text = "Rechercher"
        ' 
        ' btnReinitialiserIntervenants
        ' 
        btnReinitialiserIntervenants.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnReinitialiserIntervenants.Image = CType(resources.GetObject("btnReinitialiserIntervenants.Image"), Image)
        btnReinitialiserIntervenants.Location = New Point(620, 13)
        btnReinitialiserIntervenants.Name = "btnReinitialiserIntervenants"
        btnReinitialiserIntervenants.Size = New Size(130, 32)
        btnReinitialiserIntervenants.TabIndex = 1
        btnReinitialiserIntervenants.Tag = "reinitialiser_24_normal"
        btnReinitialiserIntervenants.Text = "Réinitialiser"
        btnReinitialiserIntervenants.Variante = VarianteBouton.Clair
        ' 
        ' carteIntervenants
        ' 
        carteIntervenants.Controls.Add(dgvIntervenants)
        carteIntervenants.Controls.Add(pnlIntervenantsActions)
        carteIntervenants.Dock = DockStyle.Fill
        carteIntervenants.Name = "carteIntervenants"
        carteIntervenants.Padding = New Padding(6, 2, 6, 6)
        carteIntervenants.TabIndex = 1
        ' 
        ' pnlIntervenantsActions
        ' 
        pnlIntervenantsActions.Controls.Add(btnSupprimerIntervenant)
        pnlIntervenantsActions.Controls.Add(btnModifierIntervenant)
        pnlIntervenantsActions.Controls.Add(btnAjouterIntervenant)
        pnlIntervenantsActions.Dock = DockStyle.Top
        pnlIntervenantsActions.Name = "pnlIntervenantsActions"
        pnlIntervenantsActions.Size = New Size(600, 42)
        pnlIntervenantsActions.TabIndex = 1
        ' 
        ' btnSupprimerIntervenant
        ' 
        btnSupprimerIntervenant.Image = CType(resources.GetObject("btnSupprimerIntervenant.Image"), Image)
        btnSupprimerIntervenant.Location = New Point(250, 5)
        btnSupprimerIntervenant.Name = "btnSupprimerIntervenant"
        btnSupprimerIntervenant.Size = New Size(124, 30)
        btnSupprimerIntervenant.TabIndex = 2
        btnSupprimerIntervenant.Text = "Supprimer"
        btnSupprimerIntervenant.Variante = VarianteBouton.OutilDanger
        ' 
        ' btnModifierIntervenant
        ' 
        btnModifierIntervenant.Image = CType(resources.GetObject("btnModifierIntervenant.Image"), Image)
        btnModifierIntervenant.Location = New Point(119, 5)
        btnModifierIntervenant.Name = "btnModifierIntervenant"
        btnModifierIntervenant.Size = New Size(113, 30)
        btnModifierIntervenant.TabIndex = 1
        btnModifierIntervenant.Text = "Modifier"
        btnModifierIntervenant.Variante = VarianteBouton.Outil
        ' 
        ' btnAjouterIntervenant
        ' 
        btnAjouterIntervenant.Image = CType(resources.GetObject("btnAjouterIntervenant.Image"), Image)
        btnAjouterIntervenant.Location = New Point(10, 5)
        btnAjouterIntervenant.Name = "btnAjouterIntervenant"
        btnAjouterIntervenant.Size = New Size(105, 30)
        btnAjouterIntervenant.TabIndex = 0
        btnAjouterIntervenant.Text = "Ajouter"
        btnAjouterIntervenant.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' tabPageDossiers
        ' 
        tabPageDossiers.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageDossiers.Controls.Add(carteDossiers)
        tabPageDossiers.Location = New Point(4, 36)
        tabPageDossiers.Name = "tabPageDossiers"
        tabPageDossiers.Padding = New Padding(8)
        tabPageDossiers.Size = New Size(925, 438)
        tabPageDossiers.TabIndex = 3
        tabPageDossiers.Text = "Dossiers"
        ' 
        ' dgvDossiers
        ' 
        dgvDossiers.AllowUserToAddRows = False
        dgvDossiers.AllowUserToDeleteRows = False
        dgvDossiers.AllowUserToResizeRows = False
        dgvDossiers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvDossiers.BackgroundColor = Color.White
        dgvDossiers.BorderStyle = BorderStyle.None
        dgvDossiers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvDossiers.Columns.AddRange(New DataGridViewColumn() {colDossierCode, colDossierDomaine, colDossierStatut, colDossierDateOuverture, colDossierDateCloture})
        dgvDossiers.Dock = DockStyle.Fill
        dgvDossiers.EditMode = DataGridViewEditMode.EditProgrammatically
        dgvDossiers.Font = New Font("Calibri", 10F)
        dgvDossiers.Location = New Point(8, 58)
        dgvDossiers.MultiSelect = False
        dgvDossiers.Name = "dgvDossiers"
        dgvDossiers.ReadOnly = True
        dgvDossiers.RowHeadersVisible = False
        dgvDossiers.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvDossiers.Size = New Size(909, 372)
        dgvDossiers.TabIndex = 1
        ' 
        ' colDossierCode
        ' 
        colDossierCode.DataPropertyName = "CodeDossier"
        colDossierCode.FillWeight = 90F
        colDossierCode.HeaderText = "Code"
        colDossierCode.Name = "colDossierCode"
        colDossierCode.ReadOnly = True
        ' 
        ' colDossierDomaine
        ' 
        colDossierDomaine.DataPropertyName = "LibelleDomaine"
        colDossierDomaine.FillWeight = 150F
        colDossierDomaine.HeaderText = "Domaine"
        colDossierDomaine.Name = "colDossierDomaine"
        colDossierDomaine.ReadOnly = True
        ' 
        ' colDossierStatut
        ' 
        colDossierStatut.DataPropertyName = "LibelleStatutDossier"
        colDossierStatut.FillWeight = 110F
        colDossierStatut.HeaderText = "Statut"
        colDossierStatut.Name = "colDossierStatut"
        colDossierStatut.ReadOnly = True
        ' 
        ' colDossierDateOuverture
        ' 
        colDossierDateOuverture.DataPropertyName = "DateOuverture"
        colDossierDateOuverture.FillWeight = 90F
        colDossierDateOuverture.HeaderText = "Ouverture"
        colDossierDateOuverture.Name = "colDossierDateOuverture"
        colDossierDateOuverture.ReadOnly = True
        ' 
        ' colDossierDateCloture
        ' 
        colDossierDateCloture.DataPropertyName = "DateCloture"
        colDossierDateCloture.FillWeight = 90F
        colDossierDateCloture.HeaderText = "Clôture"
        colDossierDateCloture.Name = "colDossierDateCloture"
        colDossierDateCloture.ReadOnly = True
        ' 
        ' carteDossiers
        ' 
        carteDossiers.Controls.Add(dgvDossiers)
        carteDossiers.Controls.Add(pnlDossiersActions)
        carteDossiers.Dock = DockStyle.Fill
        carteDossiers.Name = "carteDossiers"
        carteDossiers.Padding = New Padding(6, 2, 6, 6)
        carteDossiers.TabIndex = 0
        ' 
        ' pnlDossiersActions
        ' 
        pnlDossiersActions.Controls.Add(btnOuvrirDossier)
        pnlDossiersActions.Controls.Add(btnAjouterDossier)
        pnlDossiersActions.Dock = DockStyle.Top
        pnlDossiersActions.Name = "pnlDossiersActions"
        pnlDossiersActions.Size = New Size(600, 42)
        pnlDossiersActions.TabIndex = 0
        ' 
        ' btnOuvrirDossier
        ' 
        btnOuvrirDossier.Image = CType(resources.GetObject("btnOuvrirDossier.Image"), Image)
        btnOuvrirDossier.Location = New Point(176, 5)
        btnOuvrirDossier.Name = "btnOuvrirDossier"
        btnOuvrirDossier.Size = New Size(146, 30)
        btnOuvrirDossier.TabIndex = 1
        btnOuvrirDossier.Text = "Ouvrir dossier"
        btnOuvrirDossier.Variante = VarianteBouton.Outil
        ' 
        ' btnAjouterDossier
        ' 
        btnAjouterDossier.Image = CType(resources.GetObject("btnAjouterDossier.Image"), Image)
        btnAjouterDossier.Location = New Point(10, 5)
        btnAjouterDossier.Name = "btnAjouterDossier"
        btnAjouterDossier.Size = New Size(162, 30)
        btnAjouterDossier.TabIndex = 0
        btnAjouterDossier.Text = "Nouveau dossier"
        btnAjouterDossier.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(965, 57)
        pnlActions.TabIndex = 20
        ' 
        ' btnFermer
        ' 
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.Location = New Point(771, 8)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(161, 40)
        btnFermer.TabIndex = 4
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Retour patients"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(410, 8)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 40)
        btnAnnuler.TabIndex = 2
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(268, 8)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 40)
        btnEnregistrer.TabIndex = 1
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnModifier
        ' 
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(139, 8)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(119, 40)
        btnModifier.TabIndex = 0
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        ' 
        ' btnNouveau
        ' 
        btnNouveau.Image = CType(resources.GetObject("btnNouveau.Image"), Image)
        btnNouveau.Location = New Point(9, 8)
        btnNouveau.Name = "btnNouveau"
        btnNouveau.Size = New Size(120, 40)
        btnNouveau.TabIndex = 3
        btnNouveau.Tag = "nouveau_normal"
        btnNouveau.Text = "Nouveau"
        ' 
        ' pnlBandeau
        ' 
        pnlBandeau.BackColor = Color.Transparent
        pnlBandeau.Controls.Add(carteIdentite)
        pnlBandeau.Dock = DockStyle.Top
        pnlBandeau.Location = New Point(16, 74)
        pnlBandeau.Name = "pnlBandeau"
        pnlBandeau.Padding = New Padding(0, 6, 0, 8)
        pnlBandeau.Size = New Size(965, 136)
        pnlBandeau.TabIndex = 21
        ' 
        ' carteIdentite
        ' 
        carteIdentite.Controls.Add(pnlAlerteBandeau)
        carteIdentite.Controls.Add(picPhoto)
        carteIdentite.Controls.Add(lblNomComplet)
        carteIdentite.Controls.Add(lblCodePatient)
        carteIdentite.Controls.Add(lblStatutPatient)
        carteIdentite.Controls.Add(lblNaissanceBandeau)
        carteIdentite.Controls.Add(btnUploadPhoto)
        carteIdentite.Controls.Add(btnBasculeActif)
        carteIdentite.Dock = DockStyle.Fill
        carteIdentite.Location = New Point(0, 6)
        carteIdentite.Name = "carteIdentite"
        carteIdentite.Padding = New Padding(12)
        carteIdentite.Size = New Size(965, 122)
        carteIdentite.TabIndex = 0
        ' 
        ' pnlAlerteBandeau
        ' 
        pnlAlerteBandeau.BackColor = Color.Transparent
        pnlAlerteBandeau.Controls.Add(pnlAlerteFond)
        pnlAlerteBandeau.Controls.Add(lblAlerteBandeauTitre)
        pnlAlerteBandeau.Dock = DockStyle.Right
        pnlAlerteBandeau.Location = New Point(533, 12)
        pnlAlerteBandeau.Name = "pnlAlerteBandeau"
        pnlAlerteBandeau.Padding = New Padding(17, 0, 0, 0)
        pnlAlerteBandeau.Size = New Size(420, 98)
        pnlAlerteBandeau.TabIndex = 7
        ' 
        ' lblAlerteBandeauTitre
        ' 
        lblAlerteBandeauTitre.BackColor = Color.Transparent
        lblAlerteBandeauTitre.Dock = DockStyle.Top
        lblAlerteBandeauTitre.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblAlerteBandeauTitre.ForeColor = Color.FromArgb(CByte(122), CByte(74), CByte(64))
        lblAlerteBandeauTitre.Location = New Point(17, 0)
        lblAlerteBandeauTitre.Name = "lblAlerteBandeauTitre"
        lblAlerteBandeauTitre.Size = New Size(403, 22)
        lblAlerteBandeauTitre.TabIndex = 0
        lblAlerteBandeauTitre.Text = "ALERTE / NOTES"
        ' 
        ' pnlAlerteFond
        ' 
        pnlAlerteFond.BackColor = Color.FromArgb(CByte(247), CByte(237), CByte(234))
        pnlAlerteFond.Controls.Add(rtbAlerte)
        pnlAlerteFond.Dock = DockStyle.Fill
        pnlAlerteFond.Location = New Point(17, 22)
        pnlAlerteFond.Name = "pnlAlerteFond"
        pnlAlerteFond.Padding = New Padding(8, 5, 6, 5)
        pnlAlerteFond.Size = New Size(403, 76)
        pnlAlerteFond.TabIndex = 1
        ' 
        ' rtbAlerte
        ' 
        rtbAlerte.BackColor = Color.FromArgb(CByte(247), CByte(237), CByte(234))
        rtbAlerte.BorderStyle = BorderStyle.None
        rtbAlerte.Dock = DockStyle.Fill
        rtbAlerte.Font = New Font("Calibri", 10.5F)
        rtbAlerte.ForeColor = Color.FromArgb(CByte(122), CByte(74), CByte(64))
        rtbAlerte.Location = New Point(8, 5)
        rtbAlerte.Name = "rtbAlerte"
        rtbAlerte.ReadOnly = True
        rtbAlerte.ScrollBars = RichTextBoxScrollBars.Vertical
        rtbAlerte.Size = New Size(389, 66)
        rtbAlerte.TabIndex = 0
        rtbAlerte.TabStop = False
        rtbAlerte.Text = "Aucune note."
        ' 
        ' picPhoto
        ' 
        picPhoto.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        picPhoto.Location = New Point(14, 17)
        picPhoto.Name = "picPhoto"
        picPhoto.Size = New Size(88, 88)
        picPhoto.SizeMode = PictureBoxSizeMode.Zoom
        picPhoto.TabIndex = 0
        picPhoto.TabStop = False
        ' 
        ' lblNomComplet
        ' 
        lblNomComplet.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblNomComplet.AutoEllipsis = True
        lblNomComplet.BackColor = Color.Transparent
        lblNomComplet.Font = New Font("Calibri", 16F, FontStyle.Bold)
        lblNomComplet.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        lblNomComplet.Location = New Point(116, 10)
        lblNomComplet.Name = "lblNomComplet"
        lblNomComplet.Size = New Size(401, 30)
        lblNomComplet.TabIndex = 1
        lblNomComplet.Text = "Nouveau patient"
        ' 
        ' lblCodePatient
        ' 
        lblCodePatient.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblCodePatient.Location = New Point(118, 46)
        lblCodePatient.Name = "lblCodePatient"
        lblCodePatient.Size = New Size(30, 20)
        lblCodePatient.TabIndex = 2
        lblCodePatient.Text = "-"
        ' 
        ' lblStatutPatient
        ' 
        lblStatutPatient.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblStatutPatient.Location = New Point(206, 46)
        lblStatutPatient.Name = "lblStatutPatient"
        lblStatutPatient.Size = New Size(16, 20)
        lblStatutPatient.TabIndex = 3
        ' 
        ' lblNaissanceBandeau
        ' 
        lblNaissanceBandeau.AutoSize = True
        lblNaissanceBandeau.BackColor = Color.Transparent
        lblNaissanceBandeau.Font = New Font("Calibri", 10.5F)
        lblNaissanceBandeau.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblNaissanceBandeau.Location = New Point(320, 48)
        lblNaissanceBandeau.Name = "lblNaissanceBandeau"
        lblNaissanceBandeau.Size = New Size(0, 17)
        lblNaissanceBandeau.TabIndex = 4
        ' 
        ' btnUploadPhoto
        ' 
        btnUploadPhoto.Image = CType(resources.GetObject("btnUploadPhoto.Image"), Image)
        btnUploadPhoto.Location = New Point(116, 76)
        btnUploadPhoto.Name = "btnUploadPhoto"
        btnUploadPhoto.Size = New Size(108, 28)
        btnUploadPhoto.TabIndex = 5
        btnUploadPhoto.Text = "Photo..."
        btnUploadPhoto.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' btnBasculeActif
        ' 
        btnBasculeActif.Image = CType(resources.GetObject("btnBasculeActif.Image"), Image)
        btnBasculeActif.Location = New Point(230, 76)
        btnBasculeActif.Name = "btnBasculeActif"
        btnBasculeActif.Size = New Size(124, 28)
        btnBasculeActif.TabIndex = 6
        btnBasculeActif.Text = "Désactiver"
        btnBasculeActif.Variante = VarianteBouton.OutilDanger
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 5
        enTete.SousTitre = "Consultez, créez ou modifiez la fiche d'un patient."
        enTete.Text = "Fiche patient"
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
        pnlAnamnese.ResumeLayout(False)
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
        CType(dgvDossiers, ComponentModel.ISupportInitialize).EndInit()
        pnlDossiersActions.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        pnlAlerteFond.ResumeLayout(False)
        pnlAlerteBandeau.ResumeLayout(False)
        carteIdentite.ResumeLayout(False)
        carteIdentite.PerformLayout()
        carteRechercheContacts.ResumeLayout(False)
        carteRechercheIntervenants.ResumeLayout(False)
        pnlBandeau.ResumeLayout(False)
        CType(picPhoto, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents txtCodeTitulaire2 As TextBox
    Friend WithEvents txtCodeTitulaire1 As TextBox
    Friend WithEvents lblCodesTitulaire As Label
    Friend WithEvents txtNumeroAffiliation As TextBox
    Friend WithEvents lblAffiliation As Label
    Friend WithEvents btnFicheMutualite As AltheaBouton
    Friend WithEvents lblCivilite As Label
    Friend WithEvents cboCivilite As AltheaListe
    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents pnlBandeau As Panel
    Friend WithEvents picPhoto As PictureBox
    Friend WithEvents btnUploadPhoto As AltheaBouton
    Friend WithEvents lblNomComplet As Label
    Friend WithEvents lblCodePatient As AltheaPastille
    Friend WithEvents lblStatutPatient As AltheaPastille
    Friend WithEvents btnBasculeActif As AltheaBouton
    Friend WithEvents rtbAlerte As RichTextBox
    Friend WithEvents carteIdentite As AltheaCarte
    Friend WithEvents pnlAlerteBandeau As Panel
    Friend WithEvents lblAlerteBandeauTitre As Label
    Friend WithEvents pnlAlerteFond As Panel
    Friend WithEvents lblNaissanceBandeau As Label
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents tabFiche As AltheaOnglets
    Friend WithEvents tabPageIdentite As TabPage
    Friend WithEvents grpIdentite As AltheaCarte
    Friend WithEvents lblNom As Label
    Friend WithEvents txtNom As TextBox
    Friend WithEvents lblPrenom As Label
    Friend WithEvents txtPrenom As TextBox
    Friend WithEvents lblDateNaissance As Label
    Friend WithEvents dtpDateNaissance As AltheaDate
    Friend WithEvents lblAge As Label
    Friend WithEvents lblNiss As Label
    Friend WithEvents txtNiss As TextBox
    Friend WithEvents cboTypeNiss As AltheaListe
    Friend WithEvents lblLateralite As Label
    Friend WithEvents cboLateralite As AltheaListe
    Friend WithEvents grpCoordonnees As AltheaCarte
    Friend WithEvents lblAdresseLigne1 As Label
    Friend WithEvents txtAdresseLigne1 As TextBox
    Friend WithEvents lblAdresseLigne2 As Label
    Friend WithEvents txtAdresseLigne2 As TextBox
    Friend WithEvents lblCodePostal As Label
    Friend WithEvents txtCodePostal As TextBox
    Friend WithEvents lblLocalite As Label
    Friend WithEvents txtLocalite As TextBox
    Friend WithEvents lblPays As Label
    Friend WithEvents cboPays As AltheaListe
    Friend WithEvents lblTelephone As Label
    Friend WithEvents txtTelephone As TextBox
    Friend WithEvents lblEmail As Label
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents grpAdministratif As AltheaCarte
    Friend WithEvents lblMutualite As Label
    Friend WithEvents cboMutualite As AltheaListe
    Friend WithEvents lblSituationFamiliale As Label
    Friend WithEvents cboSituationFamiliale As AltheaListe
    Friend WithEvents btnAjouterSituationFamiliale As AltheaBouton
    Friend WithEvents lblPrixSeanceDefaut As Label
    Friend WithEvents cboPrixSeanceDefaut As AltheaListe
    Friend WithEvents grpAlerte As AltheaCarte
    Friend WithEvents rteAlerte As UC_RichTextEditor
    Friend WithEvents tabPageAnamnese As TabPage
    Friend WithEvents pnlAnamnese As Panel
    Friend WithEvents rteAnamnese As UC_RichTextEditor
    Friend WithEvents carteAnamnese As AltheaCarte
    Friend WithEvents pnlAnamneseActions As AltheaBarreOutils
    Friend WithEvents btnModifierAnamnese As AltheaBouton
    Friend WithEvents btnEnregistrerAnamnese As AltheaBouton
    Friend WithEvents btnAnnulerAnamnese As AltheaBouton
    Friend WithEvents tabPageFamille As TabPage
    Friend WithEvents dgvContacts As DataGridView
    Friend WithEvents pagContacts As UC_Pagination
    Friend WithEvents colContactLien As DataGridViewTextBoxColumn
    Friend WithEvents colContactNom As DataGridViewTextBoxColumn
    Friend WithEvents colContactPrenom As DataGridViewTextBoxColumn
    Friend WithEvents colContactTelephone As DataGridViewTextBoxColumn
    Friend WithEvents colContactRole As DataGridViewTextBoxColumn
    Friend WithEvents colContactCode As DataGridViewTextBoxColumn
    Friend WithEvents carteContacts As AltheaCarte
    Friend WithEvents pnlFamilleActions As AltheaBarreOutils
    Friend WithEvents btnAjouterContact As AltheaBouton
    Friend WithEvents btnModifierContact As AltheaBouton
    Friend WithEvents btnSupprimerContact As AltheaBouton
    Friend WithEvents pnlFamilleRecherche As Panel
    Friend WithEvents carteRechercheContacts As AltheaCarte
    Friend WithEvents txtRechercheContact As AltheaRecherche
    Friend WithEvents btnRechercherContact As AltheaBouton
    Friend WithEvents btnReinitialiserContacts As AltheaBouton
    Friend WithEvents tabPageIntervenants As TabPage
    Friend WithEvents dgvIntervenants As DataGridView
    Friend WithEvents pagIntervenants As UC_Pagination
    Friend WithEvents colIntervenantRole As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantNom As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantSpecialite As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantLieu As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantDateDebut As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantDateFin As DataGridViewTextBoxColumn
    Friend WithEvents colIntervenantCode As DataGridViewTextBoxColumn
    Friend WithEvents carteIntervenants As AltheaCarte
    Friend WithEvents pnlIntervenantsActions As AltheaBarreOutils
    Friend WithEvents btnAjouterIntervenant As AltheaBouton
    Friend WithEvents btnModifierIntervenant As AltheaBouton
    Friend WithEvents btnSupprimerIntervenant As AltheaBouton
    Friend WithEvents pnlIntervenantsRecherche As Panel
    Friend WithEvents carteRechercheIntervenants As AltheaCarte
    Friend WithEvents txtRechercheIntervenant As AltheaRecherche
    Friend WithEvents btnRechercherIntervenant As AltheaBouton
    Friend WithEvents btnReinitialiserIntervenants As AltheaBouton
    Friend WithEvents tabPageDossiers As TabPage
    Friend WithEvents dgvDossiers As DataGridView
    Friend WithEvents colDossierCode As DataGridViewTextBoxColumn
    Friend WithEvents colDossierDomaine As DataGridViewTextBoxColumn
    Friend WithEvents colDossierStatut As DataGridViewTextBoxColumn
    Friend WithEvents colDossierDateOuverture As DataGridViewTextBoxColumn
    Friend WithEvents colDossierDateCloture As DataGridViewTextBoxColumn
    Friend WithEvents carteDossiers As AltheaCarte
    Friend WithEvents pnlDossiersActions As AltheaBarreOutils
    Friend WithEvents btnOuvrirDossier As AltheaBouton
    Friend WithEvents btnAjouterDossier As AltheaBouton
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnNouveau As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents btnFermer As AltheaBouton

End Class
