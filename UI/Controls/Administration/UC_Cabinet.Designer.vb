<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_Cabinet
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Cabinet))
        pnlForm = New Panel()
        tabCabinet = New AltheaOnglets()
        tpCabinet = New TabPage()
        carteCabIdentite = New AltheaCarte()
        carteCabAdresse = New AltheaCarte()
        carteCabCoordonnees = New AltheaCarte()
        carteCabFacturation = New AltheaCarte()
        lblNomCabinet = New Label()
        txtNomCabinet = New TextBox()
        lblCabSituation = New Label()
        cboCabSituation = New AltheaListe()
        lblCabRue = New Label()
        txtCabRue = New TextBox()
        lblCabNumero = New Label()
        txtCabNumero = New TextBox()
        lblCabCp = New Label()
        txtCabCp = New TextBox()
        lblCabLocalite = New Label()
        txtCabLocalite = New TextBox()
        lblCabPays = New Label()
        cboCabPays = New AltheaListe()
        lblCabTelephone = New Label()
        txtCabTelephone = New TextBox()
        lblCabEmail = New Label()
        txtCabEmail = New TextBox()
        lblCabSite = New Label()
        txtCabSite = New TextBox()
        lblCabBce = New Label()
        txtCabBce = New TextBox()
        lblCabTva = New Label()
        txtCabTva = New TextBox()
        lblCabIban = New Label()
        txtCabIban = New TextBox()
        lblCabBic = New Label()
        txtCabBic = New TextBox()
        grpLogo = New AltheaCarte()
        picLogo = New PictureBox()
        btnLogoImporter = New AltheaBouton()
        tpPraticien = New TabPage()
        carteIdentite = New AltheaCarte()
        carteCoordonnees = New AltheaCarte()
        carteAdresse = New AltheaCarte()
        lblPraCivilite = New Label()
        cboPraCivilite = New AltheaListe()
        lblPraNom = New Label()
        txtPraNom = New TextBox()
        lblPraPrenom = New Label()
        txtPraPrenom = New TextBox()
        lblPraTitre = New Label()
        txtPraTitre = New TextBox()
        lblPraTelephone = New Label()
        txtPraTelephone = New TextBox()
        lblPraEmail = New Label()
        txtPraEmail = New TextBox()
        lblPraSite = New Label()
        txtPraSite = New TextBox()
        lblPraRue = New Label()
        txtPraRue = New TextBox()
        lblPraNumero = New Label()
        txtPraNumero = New TextBox()
        lblPraCp = New Label()
        txtPraCp = New TextBox()
        lblPraLocalite = New Label()
        txtPraLocalite = New TextBox()
        lblPraPays = New Label()
        cboPraPays = New AltheaListe()
        pnlImages = New FlowLayoutPanel()
        grpPhoto = New AltheaCarte()
        picPhoto = New PictureBox()
        btnPhotoImporter = New AltheaBouton()
        grpSignature = New AltheaCarte()
        picSignature = New PictureBox()
        btnSignatureImporter = New AltheaBouton()
        grpCompte = New AltheaCarte()
        lblCompteEtat = New Label()
        btnCompteCreer = New AltheaBouton()
        btnCompteLier = New AltheaBouton()
        btnCompteDelier = New AltheaBouton()
        tpIdentifiants = New TabPage()
        carteIdentifiants = New AltheaCarte()
        dgvIdentifiants = New DataGridView()
        lblAideIdentifiants = New Label()
        carteQualifications = New AltheaCarte()
        dgvQualifications = New DataGridView()
        lblAideQualifications = New Label()
        tpCachet = New TabPage()
        carteCachet = New AltheaCarte()
        picCachet = New PictureBox()
        pnlCachetActions = New AltheaBarreOutils()
        btnCachetGenerer = New AltheaBouton()
        btnCachetImporter = New AltheaBouton()
        lblCachetInfo = New Label()
        barreActions = New AltheaBarreActions()
        pnlBas = New FlowLayoutPanel()
        btnEnregistrer = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        btnModifier = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        tabCabinet.SuspendLayout()
        tpCabinet.SuspendLayout()
        carteCabIdentite.SuspendLayout()
        carteCabAdresse.SuspendLayout()
        carteCabCoordonnees.SuspendLayout()
        carteCabFacturation.SuspendLayout()
        grpLogo.SuspendLayout()
        CType(picLogo, ComponentModel.ISupportInitialize).BeginInit()
        tpPraticien.SuspendLayout()
        carteIdentite.SuspendLayout()
        carteCoordonnees.SuspendLayout()
        carteAdresse.SuspendLayout()
        pnlImages.SuspendLayout()
        grpPhoto.SuspendLayout()
        CType(picPhoto, ComponentModel.ISupportInitialize).BeginInit()
        grpSignature.SuspendLayout()
        CType(picSignature, ComponentModel.ISupportInitialize).BeginInit()
        grpCompte.SuspendLayout()
        tpIdentifiants.SuspendLayout()
        carteIdentifiants.SuspendLayout()
        carteQualifications.SuspendLayout()
        CType(dgvIdentifiants, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgvQualifications, ComponentModel.ISupportInitialize).BeginInit()
        tpCachet.SuspendLayout()
        carteCachet.SuspendLayout()
        CType(picCachet, ComponentModel.ISupportInitialize).BeginInit()
        pnlCachetActions.SuspendLayout()
        barreActions.SuspendLayout()
        pnlBas.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.Controls.Add(tabCabinet)
        pnlForm.Controls.Add(barreActions)
        pnlForm.Controls.Add(enTete)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16, 16, 16, 8)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 0
        ' 
        ' tabCabinet
        ' 
        tabCabinet.Controls.Add(tpCabinet)
        tabCabinet.Controls.Add(tpPraticien)
        tabCabinet.Controls.Add(tpIdentifiants)
        tabCabinet.Controls.Add(tpCachet)
        tabCabinet.CouleurFond = Color.Transparent
        tabCabinet.Dock = DockStyle.Fill
        tabCabinet.DrawMode = TabDrawMode.OwnerDrawFixed
        tabCabinet.Font = New Font("Calibri", 10F)
        tabCabinet.ItemSize = New Size(0, 30)
        tabCabinet.Location = New Point(16, 74)
        tabCabinet.Name = "tabCabinet"
        tabCabinet.Padding = New Point(14, 5)
        tabCabinet.SelectedIndex = 0
        tabCabinet.Size = New Size(965, 629)
        tabCabinet.TabIndex = 1
        ' 
        ' tpCabinet
        ' 
        tpCabinet.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpCabinet.BackgroundImageLayout = ImageLayout.Stretch
        tpCabinet.Controls.Add(carteCabIdentite)
        tpCabinet.Controls.Add(carteCabAdresse)
        tpCabinet.Controls.Add(carteCabCoordonnees)
        tpCabinet.Controls.Add(carteCabFacturation)
        tpCabinet.Controls.Add(grpLogo)
        tpCabinet.Font = New Font("Calibri", 10F)
        tpCabinet.Location = New Point(4, 34)
        tpCabinet.Name = "tpCabinet"
        tpCabinet.Padding = New Padding(12)
        tpCabinet.Size = New Size(957, 591)
        tpCabinet.TabIndex = 0
        tpCabinet.Text = "Cabinet"
        ' 
        ' carteCabIdentite
        ' 
        carteCabIdentite.Controls.Add(lblNomCabinet)
        carteCabIdentite.Controls.Add(txtNomCabinet)
        carteCabIdentite.Controls.Add(lblCabSituation)
        carteCabIdentite.Controls.Add(cboCabSituation)
        carteCabIdentite.Location = New Point(12, 12)
        carteCabIdentite.Name = "carteCabIdentite"
        carteCabIdentite.Padding = New Padding(12, 8, 12, 10)
        carteCabIdentite.Size = New Size(933, 108)
        carteCabIdentite.TabIndex = 0
        carteCabIdentite.TabStop = False
        carteCabIdentite.Titre = "Cabinet"
        ' 
        ' lblNomCabinet
        ' 
        lblNomCabinet.AutoSize = True
        lblNomCabinet.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNomCabinet.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNomCabinet.Location = New Point(14, 44)
        lblNomCabinet.Name = "lblNomCabinet"
        lblNomCabinet.Size = New Size(112, 17)
        lblNomCabinet.TabIndex = 0
        lblNomCabinet.Text = "Nom du cabinet *"
        ' 
        ' txtNomCabinet
        ' 
        txtNomCabinet.BackColor = Color.White
        txtNomCabinet.Font = New Font("Calibri", 11F)
        txtNomCabinet.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNomCabinet.Location = New Point(160, 40)
        txtNomCabinet.MaxLength = 150
        txtNomCabinet.Name = "txtNomCabinet"
        txtNomCabinet.Size = New Size(759, 25)
        txtNomCabinet.TabIndex = 1
        ' 
        ' lblCabSituation
        ' 
        lblCabSituation.AutoSize = True
        lblCabSituation.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabSituation.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabSituation.Location = New Point(14, 76)
        lblCabSituation.Name = "lblCabSituation"
        lblCabSituation.Size = New Size(121, 17)
        lblCabSituation.TabIndex = 2
        lblCabSituation.Text = "Situation juridique"
        ' 
        ' cboCabSituation
        ' 
        cboCabSituation.Font = New Font("Calibri", 11F)
        cboCabSituation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboCabSituation.ItemHeight = 20
        cboCabSituation.Location = New Point(160, 72)
        cboCabSituation.Name = "cboCabSituation"
        cboCabSituation.Size = New Size(400, 26)
        cboCabSituation.TabIndex = 3
        ' 
        ' carteCabAdresse
        ' 
        carteCabAdresse.Controls.Add(lblCabRue)
        carteCabAdresse.Controls.Add(txtCabRue)
        carteCabAdresse.Controls.Add(lblCabNumero)
        carteCabAdresse.Controls.Add(txtCabNumero)
        carteCabAdresse.Controls.Add(lblCabCp)
        carteCabAdresse.Controls.Add(txtCabCp)
        carteCabAdresse.Controls.Add(lblCabLocalite)
        carteCabAdresse.Controls.Add(txtCabLocalite)
        carteCabAdresse.Controls.Add(lblCabPays)
        carteCabAdresse.Controls.Add(cboCabPays)
        carteCabAdresse.Location = New Point(12, 128)
        carteCabAdresse.Name = "carteCabAdresse"
        carteCabAdresse.Padding = New Padding(12, 8, 12, 10)
        carteCabAdresse.Size = New Size(462, 140)
        carteCabAdresse.TabIndex = 1
        carteCabAdresse.TabStop = False
        carteCabAdresse.Titre = "Adresse"
        ' 
        ' lblCabRue
        ' 
        lblCabRue.AutoSize = True
        lblCabRue.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabRue.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabRue.Location = New Point(14, 44)
        lblCabRue.Name = "lblCabRue"
        lblCabRue.Size = New Size(30, 17)
        lblCabRue.TabIndex = 0
        lblCabRue.Text = "Rue"
        ' 
        ' txtCabRue
        ' 
        txtCabRue.BackColor = Color.White
        txtCabRue.Font = New Font("Calibri", 11F)
        txtCabRue.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabRue.Location = New Point(110, 40)
        txtCabRue.MaxLength = 150
        txtCabRue.Name = "txtCabRue"
        txtCabRue.Size = New Size(220, 25)
        txtCabRue.TabIndex = 1
        ' 
        ' lblCabNumero
        ' 
        lblCabNumero.AutoSize = True
        lblCabNumero.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabNumero.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabNumero.Location = New Point(342, 44)
        lblCabNumero.Name = "lblCabNumero"
        lblCabNumero.Size = New Size(23, 17)
        lblCabNumero.TabIndex = 2
        lblCabNumero.Text = "N°"
        ' 
        ' txtCabNumero
        ' 
        txtCabNumero.BackColor = Color.White
        txtCabNumero.Font = New Font("Calibri", 11F)
        txtCabNumero.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabNumero.Location = New Point(372, 40)
        txtCabNumero.MaxLength = 20
        txtCabNumero.Name = "txtCabNumero"
        txtCabNumero.Size = New Size(76, 25)
        txtCabNumero.TabIndex = 3
        ' 
        ' lblCabCp
        ' 
        lblCabCp.AutoSize = True
        lblCabCp.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabCp.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabCp.Location = New Point(14, 76)
        lblCabCp.Name = "lblCabCp"
        lblCabCp.Size = New Size(80, 17)
        lblCabCp.TabIndex = 4
        lblCabCp.Text = "Code postal"
        ' 
        ' txtCabCp
        ' 
        txtCabCp.BackColor = Color.White
        txtCabCp.Font = New Font("Calibri", 11F)
        txtCabCp.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabCp.Location = New Point(110, 72)
        txtCabCp.MaxLength = 20
        txtCabCp.Name = "txtCabCp"
        txtCabCp.Size = New Size(80, 25)
        txtCabCp.TabIndex = 5
        ' 
        ' lblCabLocalite
        ' 
        lblCabLocalite.AutoSize = True
        lblCabLocalite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabLocalite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabLocalite.Location = New Point(202, 76)
        lblCabLocalite.Name = "lblCabLocalite"
        lblCabLocalite.Size = New Size(54, 17)
        lblCabLocalite.TabIndex = 6
        lblCabLocalite.Text = "Localité"
        ' 
        ' txtCabLocalite
        ' 
        txtCabLocalite.BackColor = Color.White
        txtCabLocalite.Font = New Font("Calibri", 11F)
        txtCabLocalite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabLocalite.Location = New Point(262, 72)
        txtCabLocalite.MaxLength = 100
        txtCabLocalite.Name = "txtCabLocalite"
        txtCabLocalite.Size = New Size(186, 25)
        txtCabLocalite.TabIndex = 7
        ' 
        ' lblCabPays
        ' 
        lblCabPays.AutoSize = True
        lblCabPays.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabPays.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabPays.Location = New Point(14, 108)
        lblCabPays.Name = "lblCabPays"
        lblCabPays.Size = New Size(33, 17)
        lblCabPays.TabIndex = 8
        lblCabPays.Text = "Pays"
        ' 
        ' cboCabPays
        ' 
        cboCabPays.Font = New Font("Calibri", 11F)
        cboCabPays.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboCabPays.ItemHeight = 20
        cboCabPays.Location = New Point(110, 104)
        cboCabPays.Name = "cboCabPays"
        cboCabPays.Size = New Size(338, 26)
        cboCabPays.TabIndex = 9
        ' 
        ' carteCabCoordonnees
        ' 
        carteCabCoordonnees.Controls.Add(lblCabTelephone)
        carteCabCoordonnees.Controls.Add(txtCabTelephone)
        carteCabCoordonnees.Controls.Add(lblCabEmail)
        carteCabCoordonnees.Controls.Add(txtCabEmail)
        carteCabCoordonnees.Controls.Add(lblCabSite)
        carteCabCoordonnees.Controls.Add(txtCabSite)
        carteCabCoordonnees.Location = New Point(483, 128)
        carteCabCoordonnees.Name = "carteCabCoordonnees"
        carteCabCoordonnees.Padding = New Padding(12, 8, 12, 10)
        carteCabCoordonnees.Size = New Size(462, 140)
        carteCabCoordonnees.TabIndex = 2
        carteCabCoordonnees.TabStop = False
        carteCabCoordonnees.Titre = "Coordonnées"
        ' 
        ' lblCabTelephone
        ' 
        lblCabTelephone.AutoSize = True
        lblCabTelephone.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabTelephone.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabTelephone.Location = New Point(14, 44)
        lblCabTelephone.Name = "lblCabTelephone"
        lblCabTelephone.Size = New Size(70, 17)
        lblCabTelephone.TabIndex = 0
        lblCabTelephone.Text = "Téléphone"
        ' 
        ' txtCabTelephone
        ' 
        txtCabTelephone.BackColor = Color.White
        txtCabTelephone.Font = New Font("Calibri", 11F)
        txtCabTelephone.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabTelephone.Location = New Point(110, 40)
        txtCabTelephone.MaxLength = 30
        txtCabTelephone.Name = "txtCabTelephone"
        txtCabTelephone.Size = New Size(338, 25)
        txtCabTelephone.TabIndex = 1
        ' 
        ' lblCabEmail
        ' 
        lblCabEmail.AutoSize = True
        lblCabEmail.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabEmail.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabEmail.Location = New Point(14, 76)
        lblCabEmail.Name = "lblCabEmail"
        lblCabEmail.Size = New Size(45, 17)
        lblCabEmail.TabIndex = 2
        lblCabEmail.Text = "E-mail"
        ' 
        ' txtCabEmail
        ' 
        txtCabEmail.BackColor = Color.White
        txtCabEmail.Font = New Font("Calibri", 11F)
        txtCabEmail.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabEmail.Location = New Point(110, 72)
        txtCabEmail.MaxLength = 150
        txtCabEmail.Name = "txtCabEmail"
        txtCabEmail.Size = New Size(338, 25)
        txtCabEmail.TabIndex = 3
        ' 
        ' lblCabSite
        ' 
        lblCabSite.AutoSize = True
        lblCabSite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabSite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabSite.Location = New Point(14, 108)
        lblCabSite.Name = "lblCabSite"
        lblCabSite.Size = New Size(60, 17)
        lblCabSite.TabIndex = 4
        lblCabSite.Text = "Site web"
        ' 
        ' txtCabSite
        ' 
        txtCabSite.BackColor = Color.White
        txtCabSite.Font = New Font("Calibri", 11F)
        txtCabSite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabSite.Location = New Point(110, 104)
        txtCabSite.MaxLength = 255
        txtCabSite.Name = "txtCabSite"
        txtCabSite.Size = New Size(338, 25)
        txtCabSite.TabIndex = 5
        ' 
        ' carteCabFacturation
        ' 
        carteCabFacturation.Controls.Add(lblCabBce)
        carteCabFacturation.Controls.Add(txtCabBce)
        carteCabFacturation.Controls.Add(lblCabBic)
        carteCabFacturation.Controls.Add(txtCabBic)
        carteCabFacturation.Controls.Add(lblCabTva)
        carteCabFacturation.Controls.Add(txtCabTva)
        carteCabFacturation.Controls.Add(lblCabIban)
        carteCabFacturation.Controls.Add(txtCabIban)
        carteCabFacturation.Location = New Point(12, 276)
        carteCabFacturation.Name = "carteCabFacturation"
        carteCabFacturation.Padding = New Padding(12, 8, 12, 10)
        carteCabFacturation.Size = New Size(462, 140)
        carteCabFacturation.TabIndex = 3
        carteCabFacturation.TabStop = False
        carteCabFacturation.Titre = "Facturation"
        ' 
        ' lblCabBce
        ' 
        lblCabBce.AutoSize = True
        lblCabBce.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabBce.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabBce.Location = New Point(14, 44)
        lblCabBce.Name = "lblCabBce"
        lblCabBce.Size = New Size(50, 17)
        lblCabBce.TabIndex = 0
        lblCabBce.Text = "N° BCE"
        ' 
        ' txtCabBce
        ' 
        txtCabBce.BackColor = Color.White
        txtCabBce.Font = New Font("Calibri", 11F)
        txtCabBce.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabBce.Location = New Point(110, 40)
        txtCabBce.MaxLength = 30
        txtCabBce.Name = "txtCabBce"
        txtCabBce.Size = New Size(160, 25)
        txtCabBce.TabIndex = 1
        ' 
        ' lblCabBic
        ' 
        lblCabBic.AutoSize = True
        lblCabBic.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabBic.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabBic.Location = New Point(290, 44)
        lblCabBic.Name = "lblCabBic"
        lblCabBic.Size = New Size(26, 17)
        lblCabBic.TabIndex = 2
        lblCabBic.Text = "BIC"
        ' 
        ' txtCabBic
        ' 
        txtCabBic.BackColor = Color.White
        txtCabBic.Font = New Font("Calibri", 11F)
        txtCabBic.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabBic.Location = New Point(330, 40)
        txtCabBic.MaxLength = 11
        txtCabBic.Name = "txtCabBic"
        txtCabBic.Size = New Size(118, 25)
        txtCabBic.TabIndex = 3
        ' 
        ' lblCabTva
        ' 
        lblCabTva.AutoSize = True
        lblCabTva.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabTva.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabTva.Location = New Point(14, 76)
        lblCabTva.Name = "lblCabTva"
        lblCabTva.Size = New Size(50, 17)
        lblCabTva.TabIndex = 4
        lblCabTva.Text = "N° TVA"
        ' 
        ' txtCabTva
        ' 
        txtCabTva.BackColor = Color.White
        txtCabTva.Font = New Font("Calibri", 11F)
        txtCabTva.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabTva.Location = New Point(110, 72)
        txtCabTva.MaxLength = 30
        txtCabTva.Name = "txtCabTva"
        txtCabTva.Size = New Size(338, 25)
        txtCabTva.TabIndex = 5
        ' 
        ' lblCabIban
        ' 
        lblCabIban.AutoSize = True
        lblCabIban.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCabIban.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCabIban.Location = New Point(14, 108)
        lblCabIban.Name = "lblCabIban"
        lblCabIban.Size = New Size(33, 17)
        lblCabIban.TabIndex = 6
        lblCabIban.Text = "IBAN"
        ' 
        ' txtCabIban
        ' 
        txtCabIban.BackColor = Color.White
        txtCabIban.Font = New Font("Calibri", 11F)
        txtCabIban.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCabIban.Location = New Point(110, 104)
        txtCabIban.MaxLength = 42
        txtCabIban.Name = "txtCabIban"
        txtCabIban.Size = New Size(338, 25)
        txtCabIban.TabIndex = 7
        ' 
        ' grpLogo
        ' 
        grpLogo.Controls.Add(picLogo)
        grpLogo.Controls.Add(btnLogoImporter)
        grpLogo.Location = New Point(483, 276)
        grpLogo.Name = "grpLogo"
        grpLogo.Padding = New Padding(10, 4, 10, 8)
        grpLogo.Size = New Size(462, 220)
        grpLogo.TabIndex = 4
        grpLogo.Titre = "Logo du cabinet"
        ' 
        ' picLogo
        ' 
        picLogo.BorderStyle = BorderStyle.FixedSingle
        picLogo.Location = New Point(10, 34)
        picLogo.Name = "picLogo"
        picLogo.Size = New Size(442, 128)
        picLogo.SizeMode = PictureBoxSizeMode.Zoom
        picLogo.TabIndex = 0
        picLogo.TabStop = False
        ' 
        ' btnLogoImporter
        ' 
        btnLogoImporter.FlatStyle = FlatStyle.Flat
        btnLogoImporter.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnLogoImporter.ForeColor = Color.White
        btnLogoImporter.Image = CType(resources.GetObject("btnLogoImporter.Image"), Image)
        btnLogoImporter.Location = New Point(10, 172)
        btnLogoImporter.Name = "btnLogoImporter"
        btnLogoImporter.Size = New Size(125, 30)
        btnLogoImporter.TabIndex = 1
        btnLogoImporter.Tag = "open_normal"
        btnLogoImporter.Text = "Importer…"
        btnLogoImporter.UseVisualStyleBackColor = False
        btnLogoImporter.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' tpPraticien
        ' 
        tpPraticien.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpPraticien.BackgroundImageLayout = ImageLayout.Stretch
        tpPraticien.Controls.Add(carteIdentite)
        tpPraticien.Controls.Add(carteCoordonnees)
        tpPraticien.Controls.Add(carteAdresse)
        tpPraticien.Controls.Add(pnlImages)
        tpPraticien.Font = New Font("Calibri", 10F)
        tpPraticien.Location = New Point(4, 34)
        tpPraticien.Name = "tpPraticien"
        tpPraticien.Padding = New Padding(12)
        tpPraticien.Size = New Size(957, 593)
        tpPraticien.TabIndex = 1
        tpPraticien.Text = "Praticien"
        ' 
        ' carteIdentite
        ' 
        carteIdentite.Controls.Add(lblPraCivilite)
        carteIdentite.Controls.Add(cboPraCivilite)
        carteIdentite.Controls.Add(lblPraNom)
        carteIdentite.Controls.Add(txtPraNom)
        carteIdentite.Controls.Add(lblPraPrenom)
        carteIdentite.Controls.Add(txtPraPrenom)
        carteIdentite.Controls.Add(lblPraTitre)
        carteIdentite.Controls.Add(txtPraTitre)
        carteIdentite.Location = New Point(12, 12)
        carteIdentite.Name = "carteIdentite"
        carteIdentite.Padding = New Padding(12, 8, 12, 10)
        carteIdentite.Size = New Size(933, 140)
        carteIdentite.TabIndex = 0
        carteIdentite.TabStop = False
        carteIdentite.Titre = "Identité"
        ' 
        ' lblPraCivilite
        ' 
        lblPraCivilite.AutoSize = True
        lblPraCivilite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraCivilite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraCivilite.Location = New Point(14, 44)
        lblPraCivilite.Name = "lblPraCivilite"
        lblPraCivilite.Size = New Size(49, 17)
        lblPraCivilite.TabIndex = 0
        lblPraCivilite.Text = "Civilité"
        ' 
        ' cboPraCivilite
        ' 
        cboPraCivilite.Font = New Font("Calibri", 11F)
        cboPraCivilite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboPraCivilite.ItemHeight = 20
        cboPraCivilite.Location = New Point(160, 40)
        cboPraCivilite.Name = "cboPraCivilite"
        cboPraCivilite.Size = New Size(220, 26)
        cboPraCivilite.TabIndex = 1
        ' 
        ' lblPraNom
        ' 
        lblPraNom.AutoSize = True
        lblPraNom.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraNom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraNom.Location = New Point(14, 76)
        lblPraNom.Name = "lblPraNom"
        lblPraNom.Size = New Size(46, 17)
        lblPraNom.TabIndex = 2
        lblPraNom.Text = "Nom *"
        ' 
        ' txtPraNom
        ' 
        txtPraNom.BackColor = Color.White
        txtPraNom.Font = New Font("Calibri", 11F)
        txtPraNom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraNom.Location = New Point(160, 72)
        txtPraNom.MaxLength = 100
        txtPraNom.Name = "txtPraNom"
        txtPraNom.Size = New Size(300, 25)
        txtPraNom.TabIndex = 3
        ' 
        ' lblPraPrenom
        ' 
        lblPraPrenom.AutoSize = True
        lblPraPrenom.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraPrenom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraPrenom.Location = New Point(480, 76)
        lblPraPrenom.Name = "lblPraPrenom"
        lblPraPrenom.Size = New Size(62, 17)
        lblPraPrenom.TabIndex = 4
        lblPraPrenom.Text = "Prénom *"
        ' 
        ' txtPraPrenom
        ' 
        txtPraPrenom.BackColor = Color.White
        txtPraPrenom.Font = New Font("Calibri", 11F)
        txtPraPrenom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraPrenom.Location = New Point(580, 72)
        txtPraPrenom.MaxLength = 100
        txtPraPrenom.Name = "txtPraPrenom"
        txtPraPrenom.Size = New Size(339, 25)
        txtPraPrenom.TabIndex = 5
        ' 
        ' lblPraTitre
        ' 
        lblPraTitre.AutoSize = True
        lblPraTitre.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraTitre.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraTitre.Location = New Point(14, 108)
        lblPraTitre.Name = "lblPraTitre"
        lblPraTitre.Size = New Size(125, 17)
        lblPraTitre.TabIndex = 6
        lblPraTitre.Text = "Titre professionnel"
        ' 
        ' txtPraTitre
        ' 
        txtPraTitre.BackColor = Color.White
        txtPraTitre.Font = New Font("Calibri", 11F)
        txtPraTitre.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraTitre.Location = New Point(160, 104)
        txtPraTitre.MaxLength = 200
        txtPraTitre.Name = "txtPraTitre"
        txtPraTitre.Size = New Size(759, 25)
        txtPraTitre.TabIndex = 7
        ' 
        ' carteCoordonnees
        ' 
        carteCoordonnees.Controls.Add(lblPraTelephone)
        carteCoordonnees.Controls.Add(txtPraTelephone)
        carteCoordonnees.Controls.Add(lblPraEmail)
        carteCoordonnees.Controls.Add(txtPraEmail)
        carteCoordonnees.Controls.Add(lblPraSite)
        carteCoordonnees.Controls.Add(txtPraSite)
        carteCoordonnees.Location = New Point(12, 160)
        carteCoordonnees.Name = "carteCoordonnees"
        carteCoordonnees.Padding = New Padding(12, 8, 12, 10)
        carteCoordonnees.Size = New Size(462, 140)
        carteCoordonnees.TabIndex = 1
        carteCoordonnees.TabStop = False
        carteCoordonnees.Titre = "Coordonnées"
        ' 
        ' lblPraTelephone
        ' 
        lblPraTelephone.AutoSize = True
        lblPraTelephone.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraTelephone.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraTelephone.Location = New Point(14, 44)
        lblPraTelephone.Name = "lblPraTelephone"
        lblPraTelephone.Size = New Size(70, 17)
        lblPraTelephone.TabIndex = 0
        lblPraTelephone.Text = "Téléphone"
        ' 
        ' txtPraTelephone
        ' 
        txtPraTelephone.BackColor = Color.White
        txtPraTelephone.Font = New Font("Calibri", 11F)
        txtPraTelephone.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraTelephone.Location = New Point(110, 40)
        txtPraTelephone.MaxLength = 30
        txtPraTelephone.Name = "txtPraTelephone"
        txtPraTelephone.Size = New Size(338, 25)
        txtPraTelephone.TabIndex = 1
        ' 
        ' lblPraEmail
        ' 
        lblPraEmail.AutoSize = True
        lblPraEmail.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraEmail.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraEmail.Location = New Point(14, 76)
        lblPraEmail.Name = "lblPraEmail"
        lblPraEmail.Size = New Size(45, 17)
        lblPraEmail.TabIndex = 2
        lblPraEmail.Text = "E-mail"
        ' 
        ' txtPraEmail
        ' 
        txtPraEmail.BackColor = Color.White
        txtPraEmail.Font = New Font("Calibri", 11F)
        txtPraEmail.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraEmail.Location = New Point(110, 72)
        txtPraEmail.MaxLength = 150
        txtPraEmail.Name = "txtPraEmail"
        txtPraEmail.Size = New Size(338, 25)
        txtPraEmail.TabIndex = 3
        ' 
        ' lblPraSite
        ' 
        lblPraSite.AutoSize = True
        lblPraSite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraSite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraSite.Location = New Point(14, 108)
        lblPraSite.Name = "lblPraSite"
        lblPraSite.Size = New Size(60, 17)
        lblPraSite.TabIndex = 4
        lblPraSite.Text = "Site web"
        ' 
        ' txtPraSite
        ' 
        txtPraSite.BackColor = Color.White
        txtPraSite.Font = New Font("Calibri", 11F)
        txtPraSite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraSite.Location = New Point(110, 104)
        txtPraSite.MaxLength = 255
        txtPraSite.Name = "txtPraSite"
        txtPraSite.Size = New Size(338, 25)
        txtPraSite.TabIndex = 5
        ' 
        ' carteAdresse
        ' 
        carteAdresse.Controls.Add(lblPraRue)
        carteAdresse.Controls.Add(txtPraRue)
        carteAdresse.Controls.Add(lblPraNumero)
        carteAdresse.Controls.Add(txtPraNumero)
        carteAdresse.Controls.Add(lblPraCp)
        carteAdresse.Controls.Add(txtPraCp)
        carteAdresse.Controls.Add(lblPraLocalite)
        carteAdresse.Controls.Add(txtPraLocalite)
        carteAdresse.Controls.Add(lblPraPays)
        carteAdresse.Controls.Add(cboPraPays)
        carteAdresse.Location = New Point(483, 160)
        carteAdresse.Name = "carteAdresse"
        carteAdresse.Padding = New Padding(12, 8, 12, 10)
        carteAdresse.Size = New Size(462, 140)
        carteAdresse.TabIndex = 2
        carteAdresse.TabStop = False
        carteAdresse.Titre = "Adresse professionnelle (vide = celle du cabinet)"
        ' 
        ' lblPraRue
        ' 
        lblPraRue.AutoSize = True
        lblPraRue.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraRue.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraRue.Location = New Point(14, 44)
        lblPraRue.Name = "lblPraRue"
        lblPraRue.Size = New Size(30, 17)
        lblPraRue.TabIndex = 0
        lblPraRue.Text = "Rue"
        ' 
        ' txtPraRue
        ' 
        txtPraRue.BackColor = Color.White
        txtPraRue.Font = New Font("Calibri", 11F)
        txtPraRue.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraRue.Location = New Point(110, 40)
        txtPraRue.MaxLength = 150
        txtPraRue.Name = "txtPraRue"
        txtPraRue.Size = New Size(220, 25)
        txtPraRue.TabIndex = 1
        ' 
        ' lblPraNumero
        ' 
        lblPraNumero.AutoSize = True
        lblPraNumero.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraNumero.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraNumero.Location = New Point(342, 44)
        lblPraNumero.Name = "lblPraNumero"
        lblPraNumero.Size = New Size(23, 17)
        lblPraNumero.TabIndex = 2
        lblPraNumero.Text = "N°"
        ' 
        ' txtPraNumero
        ' 
        txtPraNumero.BackColor = Color.White
        txtPraNumero.Font = New Font("Calibri", 11F)
        txtPraNumero.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraNumero.Location = New Point(372, 40)
        txtPraNumero.MaxLength = 20
        txtPraNumero.Name = "txtPraNumero"
        txtPraNumero.Size = New Size(76, 25)
        txtPraNumero.TabIndex = 3
        ' 
        ' lblPraCp
        ' 
        lblPraCp.AutoSize = True
        lblPraCp.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraCp.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraCp.Location = New Point(14, 76)
        lblPraCp.Name = "lblPraCp"
        lblPraCp.Size = New Size(80, 17)
        lblPraCp.TabIndex = 4
        lblPraCp.Text = "Code postal"
        ' 
        ' txtPraCp
        ' 
        txtPraCp.BackColor = Color.White
        txtPraCp.Font = New Font("Calibri", 11F)
        txtPraCp.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraCp.Location = New Point(110, 72)
        txtPraCp.MaxLength = 20
        txtPraCp.Name = "txtPraCp"
        txtPraCp.Size = New Size(80, 25)
        txtPraCp.TabIndex = 5
        ' 
        ' lblPraLocalite
        ' 
        lblPraLocalite.AutoSize = True
        lblPraLocalite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraLocalite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraLocalite.Location = New Point(202, 76)
        lblPraLocalite.Name = "lblPraLocalite"
        lblPraLocalite.Size = New Size(54, 17)
        lblPraLocalite.TabIndex = 6
        lblPraLocalite.Text = "Localité"
        ' 
        ' txtPraLocalite
        ' 
        txtPraLocalite.BackColor = Color.White
        txtPraLocalite.Font = New Font("Calibri", 11F)
        txtPraLocalite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPraLocalite.Location = New Point(262, 72)
        txtPraLocalite.MaxLength = 100
        txtPraLocalite.Name = "txtPraLocalite"
        txtPraLocalite.Size = New Size(186, 25)
        txtPraLocalite.TabIndex = 7
        ' 
        ' lblPraPays
        ' 
        lblPraPays.AutoSize = True
        lblPraPays.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPraPays.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraPays.Location = New Point(14, 108)
        lblPraPays.Name = "lblPraPays"
        lblPraPays.Size = New Size(33, 17)
        lblPraPays.TabIndex = 8
        lblPraPays.Text = "Pays"
        ' 
        ' cboPraPays
        ' 
        cboPraPays.Font = New Font("Calibri", 11F)
        cboPraPays.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboPraPays.ItemHeight = 20
        cboPraPays.Location = New Point(110, 104)
        cboPraPays.Name = "cboPraPays"
        cboPraPays.Size = New Size(338, 26)
        cboPraPays.TabIndex = 9
        ' 
        ' pnlImages
        ' 
        pnlImages.Controls.Add(grpPhoto)
        pnlImages.Controls.Add(grpSignature)
        pnlImages.Controls.Add(grpCompte)
        pnlImages.Location = New Point(12, 308)
        pnlImages.Name = "pnlImages"
        pnlImages.Size = New Size(933, 270)
        pnlImages.TabIndex = 3
        ' 
        ' grpPhoto
        ' 
        grpPhoto.Controls.Add(picPhoto)
        grpPhoto.Controls.Add(btnPhotoImporter)
        grpPhoto.Location = New Point(3, 3)
        grpPhoto.Name = "grpPhoto"
        grpPhoto.Padding = New Padding(10, 4, 10, 8)
        grpPhoto.Size = New Size(200, 262)
        grpPhoto.TabIndex = 0
        grpPhoto.Titre = "Photo"
        ' 
        ' picPhoto
        ' 
        picPhoto.BorderStyle = BorderStyle.FixedSingle
        picPhoto.Location = New Point(10, 34)
        picPhoto.Name = "picPhoto"
        picPhoto.Size = New Size(180, 180)
        picPhoto.SizeMode = PictureBoxSizeMode.Zoom
        picPhoto.TabIndex = 0
        picPhoto.TabStop = False
        ' 
        ' btnPhotoImporter
        ' 
        btnPhotoImporter.FlatStyle = FlatStyle.Flat
        btnPhotoImporter.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnPhotoImporter.ForeColor = Color.White
        btnPhotoImporter.Image = CType(resources.GetObject("btnPhotoImporter.Image"), Image)
        btnPhotoImporter.Location = New Point(10, 222)
        btnPhotoImporter.Name = "btnPhotoImporter"
        btnPhotoImporter.Size = New Size(125, 30)
        btnPhotoImporter.TabIndex = 1
        btnPhotoImporter.Tag = "importer_normal"
        btnPhotoImporter.Text = "Importer…"
        btnPhotoImporter.UseVisualStyleBackColor = False
        btnPhotoImporter.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' grpSignature
        ' 
        grpSignature.Controls.Add(picSignature)
        grpSignature.Controls.Add(btnSignatureImporter)
        grpSignature.Location = New Point(209, 3)
        grpSignature.Name = "grpSignature"
        grpSignature.Padding = New Padding(10, 4, 10, 8)
        grpSignature.Size = New Size(320, 262)
        grpSignature.TabIndex = 1
        grpSignature.Titre = "Signature"
        ' 
        ' picSignature
        ' 
        picSignature.BorderStyle = BorderStyle.FixedSingle
        picSignature.Location = New Point(10, 34)
        picSignature.Name = "picSignature"
        picSignature.Size = New Size(300, 180)
        picSignature.SizeMode = PictureBoxSizeMode.Zoom
        picSignature.TabIndex = 0
        picSignature.TabStop = False
        ' 
        ' btnSignatureImporter
        ' 
        btnSignatureImporter.FlatStyle = FlatStyle.Flat
        btnSignatureImporter.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnSignatureImporter.ForeColor = Color.White
        btnSignatureImporter.Image = CType(resources.GetObject("btnSignatureImporter.Image"), Image)
        btnSignatureImporter.Location = New Point(10, 222)
        btnSignatureImporter.Name = "btnSignatureImporter"
        btnSignatureImporter.Size = New Size(125, 30)
        btnSignatureImporter.TabIndex = 1
        btnSignatureImporter.Tag = "importer_normal"
        btnSignatureImporter.Text = "Importer…"
        btnSignatureImporter.UseVisualStyleBackColor = False
        btnSignatureImporter.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' grpCompte
        ' 
        grpCompte.Controls.Add(lblCompteEtat)
        grpCompte.Controls.Add(btnCompteCreer)
        grpCompte.Controls.Add(btnCompteLier)
        grpCompte.Controls.Add(btnCompteDelier)
        grpCompte.Location = New Point(535, 3)
        grpCompte.Name = "grpCompte"
        grpCompte.Padding = New Padding(10, 4, 10, 8)
        grpCompte.Size = New Size(380, 262)
        grpCompte.TabIndex = 2
        grpCompte.Titre = "Compte de travail (SuperUser)"
        ' 
        ' lblCompteEtat
        ' 
        lblCompteEtat.Font = New Font("Calibri", 10.5F)
        lblCompteEtat.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblCompteEtat.Location = New Point(10, 34)
        lblCompteEtat.Name = "lblCompteEtat"
        lblCompteEtat.Size = New Size(360, 106)
        lblCompteEtat.TabIndex = 0
        lblCompteEtat.Text = "Aucun compte lié."
        ' 
        ' btnCompteCreer
        ' 
        btnCompteCreer.FlatStyle = FlatStyle.Flat
        btnCompteCreer.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnCompteCreer.ForeColor = Color.White
        btnCompteCreer.Image = CType(resources.GetObject("btnCompteCreer.Image"), Image)
        btnCompteCreer.Location = New Point(10, 148)
        btnCompteCreer.Name = "btnCompteCreer"
        btnCompteCreer.Size = New Size(168, 30)
        btnCompteCreer.TabIndex = 1
        btnCompteCreer.Tag = "login_normal"
        btnCompteCreer.Text = "Créer le compte…"
        btnCompteCreer.UseVisualStyleBackColor = False
        btnCompteCreer.Variante = VarianteBouton.Outil
        ' 
        ' btnCompteLier
        ' 
        btnCompteLier.FlatStyle = FlatStyle.Flat
        btnCompteLier.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnCompteLier.ForeColor = Color.White
        btnCompteLier.Image = CType(resources.GetObject("btnCompteLier.Image"), Image)
        btnCompteLier.Location = New Point(10, 184)
        btnCompteLier.Name = "btnCompteLier"
        btnCompteLier.Size = New Size(215, 30)
        btnCompteLier.TabIndex = 2
        btnCompteLier.Tag = "lier_normal"
        btnCompteLier.Text = "Lier un compte existant…"
        btnCompteLier.UseVisualStyleBackColor = False
        btnCompteLier.Variante = VarianteBouton.Outil
        ' 
        ' btnCompteDelier
        ' 
        btnCompteDelier.FlatStyle = FlatStyle.Flat
        btnCompteDelier.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnCompteDelier.ForeColor = Color.White
        btnCompteDelier.Image = CType(resources.GetObject("btnCompteDelier.Image"), Image)
        btnCompteDelier.Location = New Point(10, 220)
        btnCompteDelier.Name = "btnCompteDelier"
        btnCompteDelier.Size = New Size(96, 30)
        btnCompteDelier.TabIndex = 3
        btnCompteDelier.Tag = "delier_normal"
        btnCompteDelier.Text = "Délier"
        btnCompteDelier.UseVisualStyleBackColor = False
        btnCompteDelier.Variante = VarianteBouton.OutilDanger
        ' 
        ' tpIdentifiants
        ' 
        tpIdentifiants.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpIdentifiants.Controls.Add(carteQualifications)
        tpIdentifiants.Controls.Add(carteIdentifiants)
        tpIdentifiants.Font = New Font("Calibri", 10F)
        tpIdentifiants.Location = New Point(4, 34)
        tpIdentifiants.Name = "tpIdentifiants"
        tpIdentifiants.Padding = New Padding(12)
        tpIdentifiants.Size = New Size(957, 593)
        tpIdentifiants.TabIndex = 3
        tpIdentifiants.Text = "Identifiants et qualifications"
        ' 
        ' carteIdentifiants
        ' 
        carteIdentifiants.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        carteIdentifiants.Controls.Add(dgvIdentifiants)
        carteIdentifiants.Controls.Add(lblAideIdentifiants)
        carteIdentifiants.Location = New Point(12, 12)
        carteIdentifiants.Name = "carteIdentifiants"
        carteIdentifiants.Padding = New Padding(12, 8, 12, 12)
        carteIdentifiants.Size = New Size(933, 282)
        carteIdentifiants.TabIndex = 0
        carteIdentifiants.TabStop = False
        carteIdentifiants.Titre = "Identifiants professionnels"
        ' 
        ' dgvIdentifiants
        ' 
        dgvIdentifiants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvIdentifiants.Dock = DockStyle.Fill
        dgvIdentifiants.Location = New Point(12, 67)
        dgvIdentifiants.Name = "dgvIdentifiants"
        dgvIdentifiants.RowHeadersWidth = 28
        dgvIdentifiants.Size = New Size(909, 203)
        dgvIdentifiants.TabIndex = 1
        ' 
        ' lblAideIdentifiants
        ' 
        lblAideIdentifiants.BackColor = Color.Transparent
        lblAideIdentifiants.Dock = DockStyle.Top
        lblAideIdentifiants.Font = New Font("Calibri", 10F, FontStyle.Italic)
        lblAideIdentifiants.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAideIdentifiants.Location = New Point(12, 45)
        lblAideIdentifiants.Name = "lblAideIdentifiants"
        lblAideIdentifiants.Size = New Size(909, 22)
        lblAideIdentifiants.TabIndex = 0
        lblAideIdentifiants.Text = "Dates au format jj/mm/aaaa. Cochez « Cachet » pour faire figurer la ligne sur le cachet ; Suppr supprime la ligne sélectionnée."
        ' 
        ' carteQualifications
        ' 
        carteQualifications.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        carteQualifications.Controls.Add(dgvQualifications)
        carteQualifications.Controls.Add(lblAideQualifications)
        carteQualifications.Location = New Point(12, 302)
        carteQualifications.Name = "carteQualifications"
        carteQualifications.Padding = New Padding(12, 8, 12, 12)
        carteQualifications.Size = New Size(933, 279)
        carteQualifications.TabIndex = 1
        carteQualifications.TabStop = False
        carteQualifications.Titre = "Diplômes et qualifications"
        ' 
        ' dgvQualifications
        ' 
        dgvQualifications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvQualifications.Dock = DockStyle.Fill
        dgvQualifications.Location = New Point(12, 67)
        dgvQualifications.Name = "dgvQualifications"
        dgvQualifications.RowHeadersWidth = 28
        dgvQualifications.Size = New Size(909, 200)
        dgvQualifications.TabIndex = 1
        ' 
        ' lblAideQualifications
        ' 
        lblAideQualifications.BackColor = Color.Transparent
        lblAideQualifications.Dock = DockStyle.Top
        lblAideQualifications.Font = New Font("Calibri", 10F, FontStyle.Italic)
        lblAideQualifications.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAideQualifications.Location = New Point(12, 45)
        lblAideQualifications.Name = "lblAideQualifications"
        lblAideQualifications.Size = New Size(909, 22)
        lblAideQualifications.TabIndex = 0
        lblAideQualifications.Text = "« Ordre » fixe l'ordre d'affichage. Cochez « Cachet » pour faire figurer la qualification sur le cachet."
        ' 
        ' tpCachet
        ' 
        tpCachet.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpCachet.Controls.Add(carteCachet)
        tpCachet.Font = New Font("Calibri", 10F)
        tpCachet.Location = New Point(4, 34)
        tpCachet.Name = "tpCachet"
        tpCachet.Padding = New Padding(12)
        tpCachet.Size = New Size(957, 593)
        tpCachet.TabIndex = 2
        tpCachet.Text = "Cachet"
        ' 
        ' carteCachet
        ' 
        carteCachet.Controls.Add(picCachet)
        carteCachet.Controls.Add(lblCachetInfo)
        carteCachet.Controls.Add(pnlCachetActions)
        carteCachet.Dock = DockStyle.Fill
        carteCachet.Location = New Point(12, 12)
        carteCachet.Name = "carteCachet"
        carteCachet.Padding = New Padding(12, 8, 12, 12)
        carteCachet.Size = New Size(933, 569)
        carteCachet.TabIndex = 2
        ' 
        ' picCachet
        ' 
        picCachet.BackColor = Color.White
        picCachet.BorderStyle = BorderStyle.FixedSingle
        picCachet.Dock = DockStyle.Fill
        picCachet.Location = New Point(12, 82)
        picCachet.Name = "picCachet"
        picCachet.Size = New Size(909, 475)
        picCachet.SizeMode = PictureBoxSizeMode.Zoom
        picCachet.TabIndex = 0
        picCachet.TabStop = False
        ' 
        ' pnlCachetActions
        ' 
        pnlCachetActions.Controls.Add(btnCachetGenerer)
        pnlCachetActions.Controls.Add(btnCachetImporter)
        pnlCachetActions.Dock = DockStyle.Top
        pnlCachetActions.Location = New Point(12, 8)
        pnlCachetActions.Name = "pnlCachetActions"
        pnlCachetActions.Padding = New Padding(10, 6, 10, 6)
        pnlCachetActions.Size = New Size(909, 42)
        pnlCachetActions.TabIndex = 2
        ' 
        ' btnCachetGenerer
        ' 
        btnCachetGenerer.FlatStyle = FlatStyle.Flat
        btnCachetGenerer.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnCachetGenerer.ForeColor = Color.White
        btnCachetGenerer.Image = CType(resources.GetObject("btnCachetGenerer.Image"), Image)
        btnCachetGenerer.Location = New Point(10, 5)
        btnCachetGenerer.Name = "btnCachetGenerer"
        btnCachetGenerer.Size = New Size(168, 30)
        btnCachetGenerer.TabIndex = 0
        btnCachetGenerer.Text = "Générer le cachet"
        btnCachetGenerer.UseVisualStyleBackColor = False
        btnCachetGenerer.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' btnCachetImporter
        ' 
        btnCachetImporter.FlatStyle = FlatStyle.Flat
        btnCachetImporter.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnCachetImporter.ForeColor = Color.White
        btnCachetImporter.Image = CType(resources.GetObject("btnCachetImporter.Image"), Image)
        btnCachetImporter.Location = New Point(182, 5)
        btnCachetImporter.Name = "btnCachetImporter"
        btnCachetImporter.Size = New Size(175, 30)
        btnCachetImporter.TabIndex = 1
        btnCachetImporter.Text = "Importer un scan…"
        btnCachetImporter.UseVisualStyleBackColor = False
        btnCachetImporter.Variante = VarianteBouton.Outil
        ' 
        ' lblCachetInfo
        ' 
        lblCachetInfo.BackColor = Color.Transparent
        lblCachetInfo.Dock = DockStyle.Top
        lblCachetInfo.Font = New Font("Calibri", 10F, FontStyle.Italic)
        lblCachetInfo.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCachetInfo.Location = New Point(12, 50)
        lblCachetInfo.Name = "lblCachetInfo"
        lblCachetInfo.Padding = New Padding(0, 6, 0, 0)
        lblCachetInfo.Size = New Size(909, 32)
        lblCachetInfo.TabIndex = 1
        lblCachetInfo.Text = "Le cachet est généré depuis la saisie en cours (identifiants et qualifications cochés « Cachet »). Vous pouvez aussi importer un scan. Comme les autres images, il est enregistré avec la fiche."
        ' 
        ' barreActions
        ' 
        barreActions.Controls.Add(pnlBas)
        barreActions.CouleurFond = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        barreActions.Dock = DockStyle.Bottom
        barreActions.Location = New Point(16, 703)
        barreActions.Name = "barreActions"
        barreActions.OpaciteFond = 190
        barreActions.Padding = New Padding(16, 8, 16, 12)
        barreActions.Rayon = 12
        barreActions.Size = New Size(965, 60)
        barreActions.TabIndex = 2
        ' 
        ' pnlBas
        ' 
        pnlBas.BackColor = Color.Transparent
        pnlBas.Controls.Add(btnEnregistrer)
        pnlBas.Controls.Add(btnAnnuler)
        pnlBas.Controls.Add(btnModifier)
        pnlBas.Dock = DockStyle.Fill
        pnlBas.FlowDirection = FlowDirection.RightToLeft
        pnlBas.Location = New Point(16, 8)
        pnlBas.Name = "pnlBas"
        pnlBas.Padding = New Padding(0, 1, 0, 0)
        pnlBas.Size = New Size(933, 40)
        pnlBas.TabIndex = 2
        pnlBas.WrapContents = False
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.FlatStyle = FlatStyle.Flat
        btnEnregistrer.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnEnregistrer.ForeColor = Color.White
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(796, 1)
        btnEnregistrer.Margin = New Padding(5, 0, 5, 0)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 40)
        btnEnregistrer.TabIndex = 0
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.UseVisualStyleBackColor = False
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.FlatStyle = FlatStyle.Flat
        btnAnnuler.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnAnnuler.ForeColor = Color.White
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(667, 1)
        btnAnnuler.Margin = New Padding(5, 0, 5, 0)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(119, 40)
        btnAnnuler.TabIndex = 1
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.UseVisualStyleBackColor = False
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' btnModifier
        ' 
        btnModifier.FlatStyle = FlatStyle.Flat
        btnModifier.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnModifier.ForeColor = Color.White
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(538, 1)
        btnModifier.Margin = New Padding(5, 0, 5, 0)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(119, 40)
        btnModifier.TabIndex = 2
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        btnModifier.UseVisualStyleBackColor = False
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Padding = New Padding(16, 4, 16, 4)
        enTete.Size = New Size(965, 58)
        enTete.SousTitre = "Identité du cabinet, fiche du praticien, cachet et identifiants professionnels."
        enTete.TabIndex = 0
        enTete.Text = "Cabinet et praticien"
        ' 
        ' UC_Cabinet
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Name = "UC_Cabinet"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        tabCabinet.ResumeLayout(False)
        tpCabinet.ResumeLayout(False)
        carteCabIdentite.ResumeLayout(False)
        carteCabIdentite.PerformLayout()
        carteCabAdresse.ResumeLayout(False)
        carteCabAdresse.PerformLayout()
        carteCabCoordonnees.ResumeLayout(False)
        carteCabCoordonnees.PerformLayout()
        carteCabFacturation.ResumeLayout(False)
        carteCabFacturation.PerformLayout()
        grpLogo.ResumeLayout(False)
        CType(picLogo, ComponentModel.ISupportInitialize).EndInit()
        tpPraticien.ResumeLayout(False)
        carteIdentite.ResumeLayout(False)
        carteIdentite.PerformLayout()
        carteCoordonnees.ResumeLayout(False)
        carteCoordonnees.PerformLayout()
        carteAdresse.ResumeLayout(False)
        carteAdresse.PerformLayout()
        pnlImages.ResumeLayout(False)
        grpPhoto.ResumeLayout(False)
        CType(picPhoto, ComponentModel.ISupportInitialize).EndInit()
        grpSignature.ResumeLayout(False)
        CType(picSignature, ComponentModel.ISupportInitialize).EndInit()
        grpCompte.ResumeLayout(False)
        tpIdentifiants.ResumeLayout(False)
        carteIdentifiants.ResumeLayout(False)
        carteQualifications.ResumeLayout(False)
        CType(dgvIdentifiants, ComponentModel.ISupportInitialize).EndInit()
        CType(dgvQualifications, ComponentModel.ISupportInitialize).EndInit()
        tpCachet.ResumeLayout(False)
        carteCachet.ResumeLayout(False)
        CType(picCachet, ComponentModel.ISupportInitialize).EndInit()
        pnlCachetActions.ResumeLayout(False)
        barreActions.ResumeLayout(False)
        pnlBas.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents tabCabinet As AltheaOnglets
    Friend WithEvents tpCabinet As TabPage
    Friend WithEvents carteCabIdentite As AltheaCarte
    Friend WithEvents carteCabAdresse As AltheaCarte
    Friend WithEvents carteCabCoordonnees As AltheaCarte
    Friend WithEvents carteCabFacturation As AltheaCarte
    Friend WithEvents lblNomCabinet As Label
    Friend WithEvents txtNomCabinet As TextBox
    Friend WithEvents lblCabSituation As Label
    Friend WithEvents cboCabSituation As AltheaListe
    Friend WithEvents lblCabRue As Label
    Friend WithEvents txtCabRue As TextBox
    Friend WithEvents lblCabNumero As Label
    Friend WithEvents txtCabNumero As TextBox
    Friend WithEvents lblCabCp As Label
    Friend WithEvents txtCabCp As TextBox
    Friend WithEvents lblCabLocalite As Label
    Friend WithEvents txtCabLocalite As TextBox
    Friend WithEvents lblCabPays As Label
    Friend WithEvents cboCabPays As AltheaListe
    Friend WithEvents lblCabTelephone As Label
    Friend WithEvents txtCabTelephone As TextBox
    Friend WithEvents lblCabEmail As Label
    Friend WithEvents txtCabEmail As TextBox
    Friend WithEvents lblCabSite As Label
    Friend WithEvents txtCabSite As TextBox
    Friend WithEvents lblCabBce As Label
    Friend WithEvents txtCabBce As TextBox
    Friend WithEvents lblCabTva As Label
    Friend WithEvents txtCabTva As TextBox
    Friend WithEvents lblCabIban As Label
    Friend WithEvents txtCabIban As TextBox
    Friend WithEvents lblCabBic As Label
    Friend WithEvents txtCabBic As TextBox
    Friend WithEvents grpLogo As AltheaCarte
    Friend WithEvents picLogo As PictureBox
    Friend WithEvents btnLogoImporter As AltheaBouton
    Friend WithEvents tpPraticien As TabPage
    Friend WithEvents carteIdentite As AltheaCarte
    Friend WithEvents carteCoordonnees As AltheaCarte
    Friend WithEvents carteAdresse As AltheaCarte
    Friend WithEvents lblPraCivilite As Label
    Friend WithEvents cboPraCivilite As AltheaListe
    Friend WithEvents lblPraNom As Label
    Friend WithEvents txtPraNom As TextBox
    Friend WithEvents lblPraPrenom As Label
    Friend WithEvents txtPraPrenom As TextBox
    Friend WithEvents lblPraTitre As Label
    Friend WithEvents txtPraTitre As TextBox
    Friend WithEvents lblPraTelephone As Label
    Friend WithEvents txtPraTelephone As TextBox
    Friend WithEvents lblPraEmail As Label
    Friend WithEvents txtPraEmail As TextBox
    Friend WithEvents lblPraSite As Label
    Friend WithEvents txtPraSite As TextBox
    Friend WithEvents lblPraRue As Label
    Friend WithEvents txtPraRue As TextBox
    Friend WithEvents lblPraNumero As Label
    Friend WithEvents txtPraNumero As TextBox
    Friend WithEvents lblPraCp As Label
    Friend WithEvents txtPraCp As TextBox
    Friend WithEvents lblPraLocalite As Label
    Friend WithEvents txtPraLocalite As TextBox
    Friend WithEvents lblPraPays As Label
    Friend WithEvents cboPraPays As AltheaListe
    Friend WithEvents pnlImages As FlowLayoutPanel
    Friend WithEvents grpPhoto As AltheaCarte
    Friend WithEvents picPhoto As PictureBox
    Friend WithEvents btnPhotoImporter As AltheaBouton
    Friend WithEvents grpSignature As AltheaCarte
    Friend WithEvents picSignature As PictureBox
    Friend WithEvents btnSignatureImporter As AltheaBouton
    Friend WithEvents grpCompte As AltheaCarte
    Friend WithEvents lblCompteEtat As Label
    Friend WithEvents btnCompteCreer As AltheaBouton
    Friend WithEvents btnCompteLier As AltheaBouton
    Friend WithEvents btnCompteDelier As AltheaBouton
    Friend WithEvents tpCachet As TabPage
    Friend WithEvents picCachet As PictureBox
    Friend WithEvents lblCachetInfo As Label
    Friend WithEvents carteCachet As AltheaCarte
    Friend WithEvents pnlCachetActions As AltheaBarreOutils
    Friend WithEvents btnCachetGenerer As AltheaBouton
    Friend WithEvents btnCachetImporter As AltheaBouton
    Friend WithEvents tpIdentifiants As TabPage
    Friend WithEvents carteIdentifiants As AltheaCarte
    Friend WithEvents dgvIdentifiants As DataGridView
    Friend WithEvents lblAideIdentifiants As Label
    Friend WithEvents carteQualifications As AltheaCarte
    Friend WithEvents dgvQualifications As DataGridView
    Friend WithEvents lblAideQualifications As Label
    Friend WithEvents barreActions As AltheaBarreActions
    Friend WithEvents pnlBas As FlowLayoutPanel
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete

End Class
