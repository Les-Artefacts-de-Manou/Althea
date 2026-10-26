' -------------------------------------------------------------------------------------------------
' Designer    : UC_DomainesHome
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Structure visuelle de l'espace dédié Domaines : liste des domaines à gauche, fiche du domaine à
' droite. Depuis UI-6 (14/10/2026), la navigation passe par les étapes (AltheaEtapes) ; les pages
' restent des onglets du kit, dont la bande est masquée à l'exécution (MasquerOnglets).
' V1.2.0 (14/10/2026, UI-6 6d) : la synthèse est présentée dans une carte « Convention du domaine »,
' comme dans la fiche dossier ; « Modifier la règle » élargi (texte sur une ligne).
' V1.3.0 (20/10/2026) : carte « Logo du domaine » sous l'identité et le comportement (étape 1) ;
' dates des règles de quota en AltheaDate (champ du kit).
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_DomainesHome
    Inherits System.Windows.Forms.UserControl

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_DomainesHome))
        pnlForm = New Panel()
        splMain = New SplitContainer()
        dgvDomaines = New DataGridView()
        colId = New DataGridViewTextBoxColumn()
        colCode = New DataGridViewTextBoxColumn()
        colLibelle = New DataGridViewTextBoxColumn()
        colEtat = New DataGridViewTextBoxColumn()
        pnlListeTop = New AltheaCarte()
        chkAfficherInactifs = New AltheaCase()
        txtRecherche = New AltheaRecherche()
        tabDomaine = New AltheaOnglets()
        tabGeneral = New TabPage()
        tblGeneral = New TableLayoutPanel()
        carteComportement = New AltheaCarte()
        lblAideQuota = New Label()
        lblAideNoShow = New Label()
        lblAideTarif = New Label()
        chkSoumisQuota = New AltheaCase()
        chkNoShowFacturable = New AltheaCase()
        chkTarifConvention = New AltheaCase()
        carteIdentite = New AltheaCarte()
        carteLogo = New AltheaCarte()
        picLogo = New PictureBox()
        btnLogoImporter = New AltheaBouton()
        btnLogoRetirer = New AltheaBouton()
        lblAideLogo = New Label()
        btnNouvelOrganisme = New AltheaBouton()
        cboOrganisme = New AltheaListe()
        lblOrganisme = New Label()
        chkActif = New AltheaCase()
        numOrdre = New NumericUpDown()
        lblOrdre = New Label()
        txtLibelle = New TextBox()
        lblLibelle = New Label()
        txtCode = New TextBox()
        lblCode = New Label()
        tabTarifs = New TabPage()
        tabQuotas = New TabPage()
        dgvQuotas = New DataGridView()
        colQuotaPeriode = New DataGridViewTextBoxColumn()
        colQuotaPlafonds = New DataGridViewTextBoxColumn()
        colQuotaAlerte = New DataGridViewTextBoxColumn()
        colQuotaEtat = New DataGridViewTextBoxColumn()
        carteCategories = New AltheaCarte()
        lblCategoriesTitre = New Label()
        flpCategories = New FlowLayoutPanel()
        pnlEspaceQuotas = New Panel()
        carteRegles = New AltheaCarte()
        barreRegles = New AltheaBarreOutils()
        lblQuotaZone = New Label()
        lblPlafondsTitre = New Label()
        lblQuotaTotal = New Label()
        btnQuotaValider = New AltheaBouton()
        btnQuotaAnnuler = New AltheaBouton()
        carteQuotaNonNecessaire = New AltheaCarte()
        lblQuotaNonNecessaire = New Label()
        pnlQuotaEdition = New Panel()
        btnQuotaCategories = New AltheaBouton()
        btnQuotaSupprimer = New AltheaBouton()
        btnQuotaActiver = New AltheaBouton()
        btnQuotaCloturer = New AltheaBouton()
        btnQuotaAjouter = New AltheaBouton()
        btnQuotaModifier = New AltheaBouton()
        dtpQuotaFin = New AltheaDate()
        chkQuotaFin = New AltheaCase()
        dtpQuotaDebut = New AltheaDate()
        lblQuotaDebut = New Label()
        numQuotaAlerte = New NumericUpDown()
        lblQuotaAlerte = New Label()
        numQuotaDuree = New NumericUpDown()
        lblQuotaDuree = New Label()
        dgvQuotaPlafonds = New DataGridView()
        colPlafondCategorie = New DataGridViewTextBoxColumn()
        colPlafondValeur = New DataGridViewTextBoxColumn()
        lblAideQuotas = New Label()
        tabNatures = New TabPage()
        tabSynthese = New TabPage()
        carteSynthese = New AltheaCarte()
        ucSynthese = New UC_SyntheseDomaine()
        pnlAssistant = New AltheaCarte()
        lblAssistantEtape = New Label()
        btnAssistantPrecedent = New AltheaBouton()
        btnAssistantSuivant = New AltheaBouton()
        etapes = New AltheaEtapes()
        lblDomaineCourant = New Label()
        pstEtatDomaine = New AltheaPastille()
        pnlActions = New AltheaBarreActions()
        btnAnnuler = New AltheaBouton()
        btnEnregistrer = New AltheaBouton()
        btnActiverDesactiver = New AltheaBouton()
        btnModifier = New AltheaBouton()
        btnNouveau = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        CType(splMain, ComponentModel.ISupportInitialize).BeginInit()
        splMain.Panel1.SuspendLayout()
        splMain.Panel2.SuspendLayout()
        splMain.SuspendLayout()
        CType(dgvDomaines, ComponentModel.ISupportInitialize).BeginInit()
        pnlListeTop.SuspendLayout()
        tabDomaine.SuspendLayout()
        tabGeneral.SuspendLayout()
        tblGeneral.SuspendLayout()
        carteComportement.SuspendLayout()
        carteIdentite.SuspendLayout()
        carteLogo.SuspendLayout()
        CType(picLogo, ComponentModel.ISupportInitialize).BeginInit()
        CType(numOrdre, ComponentModel.ISupportInitialize).BeginInit()
        tabTarifs.SuspendLayout()
        tabQuotas.SuspendLayout()
        CType(dgvQuotas, ComponentModel.ISupportInitialize).BeginInit()
        carteCategories.SuspendLayout()
        carteRegles.SuspendLayout()
        barreRegles.SuspendLayout()
        carteQuotaNonNecessaire.SuspendLayout()
        pnlQuotaEdition.SuspendLayout()
        CType(numQuotaAlerte, ComponentModel.ISupportInitialize).BeginInit()
        CType(numQuotaDuree, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgvQuotaPlafonds, ComponentModel.ISupportInitialize).BeginInit()
        tabNatures.SuspendLayout()
        tabSynthese.SuspendLayout()
        carteSynthese.SuspendLayout()
        pnlAssistant.SuspendLayout()
        lblDomaineCourant.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        pnlForm.Controls.Add(splMain)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(enTete)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 0
        ' 
        ' splMain
        ' 
        splMain.BackColor = Color.Transparent
        splMain.Dock = DockStyle.Fill
        splMain.FixedPanel = FixedPanel.Panel1
        splMain.Location = New Point(16, 74)
        splMain.Name = "splMain"
        ' 
        ' splMain.Panel1
        ' 
        splMain.Panel1.Controls.Add(dgvDomaines)
        splMain.Panel1.Controls.Add(pnlListeTop)
        splMain.Panel1.Padding = New Padding(0, 8, 8, 8)
        ' 
        ' splMain.Panel2
        ' 
        splMain.Panel2.Controls.Add(tabDomaine)
        splMain.Panel2.Controls.Add(pnlAssistant)
        splMain.Panel2.Controls.Add(etapes)
        splMain.Panel2.Controls.Add(lblDomaineCourant)
        splMain.Panel2.Padding = New Padding(8, 8, 0, 8)
        splMain.Size = New Size(965, 624)
        splMain.SplitterDistance = 340
        splMain.TabIndex = 1
        ' 
        ' dgvDomaines
        ' 
        dgvDomaines.AllowUserToAddRows = False
        dgvDomaines.AllowUserToDeleteRows = False
        dgvDomaines.AllowUserToResizeRows = False
        dgvDomaines.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvDomaines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvDomaines.Columns.AddRange(New DataGridViewColumn() {colId, colCode, colLibelle, colEtat})
        dgvDomaines.Dock = DockStyle.Fill
        dgvDomaines.Location = New Point(0, 92)
        dgvDomaines.MultiSelect = False
        dgvDomaines.Name = "dgvDomaines"
        dgvDomaines.ReadOnly = True
        dgvDomaines.RowHeadersVisible = False
        dgvDomaines.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvDomaines.Size = New Size(332, 524)
        dgvDomaines.TabIndex = 1
        ' 
        ' colId
        ' 
        colId.HeaderText = "Id"
        colId.Name = "colId"
        colId.ReadOnly = True
        colId.Visible = False
        ' 
        ' colCode
        ' 
        colCode.HeaderText = "Code"
        colCode.Name = "colCode"
        colCode.ReadOnly = True
        colCode.Width = 55
        ' 
        ' colLibelle
        ' 
        colLibelle.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colLibelle.HeaderText = "Libellé"
        colLibelle.Name = "colLibelle"
        colLibelle.ReadOnly = True
        ' 
        ' colEtat
        ' 
        colEtat.HeaderText = "État"
        colEtat.Name = "colEtat"
        colEtat.ReadOnly = True
        colEtat.Width = 95
        ' 
        ' pnlListeTop
        ' 
        pnlListeTop.Controls.Add(chkAfficherInactifs)
        pnlListeTop.Controls.Add(txtRecherche)
        pnlListeTop.Dock = DockStyle.Top
        pnlListeTop.Location = New Point(0, 8)
        pnlListeTop.Name = "pnlListeTop"
        pnlListeTop.Padding = New Padding(12, 10, 12, 10)
        pnlListeTop.Size = New Size(332, 84)
        pnlListeTop.TabIndex = 0
        ' 
        ' chkAfficherInactifs
        ' 
        chkAfficherInactifs.AutoSize = False
        chkAfficherInactifs.Location = New Point(12, 50)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(170, 24)
        chkAfficherInactifs.TabIndex = 2
        chkAfficherInactifs.Text = "Afficher les inactifs"
        ' 
        ' txtRecherche
        ' 
        txtRecherche.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtRecherche.Location = New Point(12, 10)
        txtRecherche.Name = "txtRecherche"
        txtRecherche.Size = New Size(308, 32)
        txtRecherche.TabIndex = 1
        txtRecherche.TexteAide = "Code ou libellé…"
        ' 
        ' tabDomaine
        ' 
        tabDomaine.Controls.Add(tabGeneral)
        tabDomaine.Controls.Add(tabTarifs)
        tabDomaine.Controls.Add(tabQuotas)
        tabDomaine.Controls.Add(tabNatures)
        tabDomaine.Controls.Add(tabSynthese)
        tabDomaine.CouleurFond = Color.Transparent
        tabDomaine.Dock = DockStyle.Fill
        tabDomaine.Font = New Font("Calibri", 10F)
        tabDomaine.Location = New Point(8, 178)
        tabDomaine.MasquerOnglets = True
        tabDomaine.Name = "tabDomaine"
        tabDomaine.SelectedIndex = 0
        tabDomaine.Size = New Size(613, 438)
        tabDomaine.TabIndex = 0
        ' 
        ' tabGeneral
        ' 
        tabGeneral.BackColor = Color.FromArgb(CByte(237), CByte(231), CByte(224))
        tabGeneral.Controls.Add(tblGeneral)
        tabGeneral.Font = New Font("Calibri", 10F)
        tabGeneral.Location = New Point(4, 27)
        tabGeneral.Name = "tabGeneral"
        tabGeneral.Padding = New Padding(12)
        tabGeneral.Size = New Size(605, 491)
        tabGeneral.TabIndex = 0
        tabGeneral.Text = "1. Général"
        ' 
        ' tblGeneral
        ' 
        tblGeneral.BackColor = Color.Transparent
        tblGeneral.ColumnCount = 2
        tblGeneral.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        tblGeneral.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        tblGeneral.Controls.Add(carteIdentite, 0, 0)
        tblGeneral.Controls.Add(carteComportement, 1, 0)
        tblGeneral.Controls.Add(carteLogo, 0, 1)
        tblGeneral.Dock = DockStyle.Top
        tblGeneral.Location = New Point(12, 12)
        tblGeneral.Name = "tblGeneral"
        tblGeneral.RowCount = 2
        tblGeneral.RowStyles.Add(New RowStyle(SizeType.Absolute, 256F))
        tblGeneral.RowStyles.Add(New RowStyle(SizeType.Absolute, 172F))
        tblGeneral.SetColumnSpan(carteLogo, 2)
        tblGeneral.Size = New Size(581, 428)
        tblGeneral.TabIndex = 0
        ' 
        ' carteIdentite
        ' 
        carteIdentite.Controls.Add(chkActif)
        carteIdentite.Controls.Add(numOrdre)
        carteIdentite.Controls.Add(lblOrdre)
        carteIdentite.Controls.Add(btnNouvelOrganisme)
        carteIdentite.Controls.Add(cboOrganisme)
        carteIdentite.Controls.Add(lblOrganisme)
        carteIdentite.Controls.Add(txtLibelle)
        carteIdentite.Controls.Add(lblLibelle)
        carteIdentite.Controls.Add(txtCode)
        carteIdentite.Controls.Add(lblCode)
        carteIdentite.Dock = DockStyle.Fill
        carteIdentite.Location = New Point(0, 0)
        carteIdentite.Margin = New Padding(0, 0, 6, 0)
        carteIdentite.Name = "carteIdentite"
        carteIdentite.Size = New Size(284, 256)
        carteIdentite.TabIndex = 0
        carteIdentite.Titre = "Identité du domaine"
        ' 
        ' lblCode
        ' 
        lblCode.AutoSize = True
        lblCode.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblCode.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCode.Location = New Point(18, 50)
        lblCode.Name = "lblCode"
        lblCode.Size = New Size(106, 17)
        lblCode.TabIndex = 0
        lblCode.Text = "Code (3 lettres) *"
        ' 
        ' txtCode
        ' 
        txtCode.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCode.CharacterCasing = CharacterCasing.Upper
        txtCode.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCode.Location = New Point(18, 72)
        txtCode.MaxLength = 3
        txtCode.Name = "txtCode"
        txtCode.Size = New Size(80, 24)
        txtCode.TabIndex = 1
        ' 
        ' lblLibelle
        ' 
        lblLibelle.AutoSize = True
        lblLibelle.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblLibelle.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLibelle.Location = New Point(110, 50)
        lblLibelle.Name = "lblLibelle"
        lblLibelle.Size = New Size(55, 17)
        lblLibelle.TabIndex = 2
        lblLibelle.Text = "Libellé *"
        ' 
        ' txtLibelle
        ' 
        txtLibelle.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtLibelle.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtLibelle.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLibelle.Location = New Point(110, 72)
        txtLibelle.MaxLength = 100
        txtLibelle.Name = "txtLibelle"
        txtLibelle.Size = New Size(156, 24)
        txtLibelle.TabIndex = 3
        ' 
        ' lblOrganisme
        ' 
        lblOrganisme.AutoSize = True
        lblOrganisme.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblOrganisme.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblOrganisme.Location = New Point(18, 108)
        lblOrganisme.Name = "lblOrganisme"
        lblOrganisme.Size = New Size(188, 17)
        lblOrganisme.TabIndex = 4
        lblOrganisme.Text = "Organisme de remboursement"
        ' 
        ' cboOrganisme
        ' 
        cboOrganisme.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        cboOrganisme.Location = New Point(18, 127)
        cboOrganisme.Name = "cboOrganisme"
        cboOrganisme.Size = New Size(124, 30)
        cboOrganisme.TabIndex = 5
        ' 
        ' btnNouvelOrganisme
        ' 
        btnNouvelOrganisme.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnNouvelOrganisme.Location = New Point(148, 127)
        btnNouvelOrganisme.Name = "btnNouvelOrganisme"
        btnNouvelOrganisme.Size = New Size(118, 30)
        btnNouvelOrganisme.TabIndex = 6
        btnNouvelOrganisme.Tag = "nouveau_normal"
        btnNouvelOrganisme.Text = "Nouveau…"
        btnNouvelOrganisme.Variante = VarianteBouton.Outil
        ' 
        ' lblOrdre
        ' 
        lblOrdre.AutoSize = True
        lblOrdre.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblOrdre.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblOrdre.Location = New Point(18, 168)
        lblOrdre.Name = "lblOrdre"
        lblOrdre.Size = New Size(110, 17)
        lblOrdre.TabIndex = 7
        lblOrdre.Text = "Ordre d'affichage"
        ' 
        ' numOrdre
        ' 
        numOrdre.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        numOrdre.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        numOrdre.Location = New Point(18, 190)
        numOrdre.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        numOrdre.Name = "numOrdre"
        numOrdre.Size = New Size(80, 24)
        numOrdre.TabIndex = 8
        ' 
        ' chkActif
        ' 
        chkActif.AutoSize = False
        chkActif.Checked = True
        chkActif.CheckState = CheckState.Checked
        chkActif.Location = New Point(110, 190)
        chkActif.Name = "chkActif"
        chkActif.Size = New Size(150, 24)
        chkActif.TabIndex = 9
        chkActif.Text = "Domaine actif"
        ' 
        ' carteLogo
        ' 
        carteLogo.Controls.Add(lblAideLogo)
        carteLogo.Controls.Add(btnLogoRetirer)
        carteLogo.Controls.Add(btnLogoImporter)
        carteLogo.Controls.Add(picLogo)
        carteLogo.Dock = DockStyle.Fill
        carteLogo.Location = New Point(0, 268)
        carteLogo.Margin = New Padding(0, 12, 0, 0)
        carteLogo.Name = "carteLogo"
        carteLogo.Size = New Size(581, 160)
        carteLogo.TabIndex = 2
        carteLogo.Titre = "Logo du domaine"
        ' 
        ' picLogo
        ' 
        picLogo.BackColor = Color.White
        picLogo.BorderStyle = BorderStyle.FixedSingle
        picLogo.Location = New Point(18, 46)
        picLogo.Name = "picLogo"
        picLogo.Size = New Size(96, 96)
        picLogo.SizeMode = PictureBoxSizeMode.Zoom
        picLogo.TabIndex = 0
        picLogo.TabStop = False
        ' 
        ' btnLogoImporter
        ' 
        btnLogoImporter.Location = New Point(132, 46)
        btnLogoImporter.Name = "btnLogoImporter"
        btnLogoImporter.Size = New Size(120, 30)
        btnLogoImporter.TabIndex = 1
        btnLogoImporter.Tag = "importer_normal"
        btnLogoImporter.Text = "Importer…"
        btnLogoImporter.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' btnLogoRetirer
        ' 
        btnLogoRetirer.Location = New Point(132, 84)
        btnLogoRetirer.Name = "btnLogoRetirer"
        btnLogoRetirer.Size = New Size(120, 30)
        btnLogoRetirer.TabIndex = 2
        btnLogoRetirer.Tag = "retirer_normal"
        btnLogoRetirer.Text = "Retirer"
        btnLogoRetirer.Variante = VarianteBouton.Outil
        ' 
        ' lblAideLogo
        ' 
        lblAideLogo.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblAideLogo.Font = New Font("Calibri", 9.75F)
        lblAideLogo.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblAideLogo.Location = New Point(272, 46)
        lblAideLogo.Name = "lblAideLogo"
        lblAideLogo.Size = New Size(290, 96)
        lblAideLogo.TabIndex = 3
        lblAideLogo.Text = "Facultatif. Affiché dans le bandeau et la liste des domaines des dossiers, et dans la convention du domaine. Image PNG, JPG… enregistrée réduite (256 pixels au plus)."
        ' 
        ' carteComportement
        ' 
        carteComportement.Controls.Add(lblAideQuota)
        carteComportement.Controls.Add(chkSoumisQuota)
        carteComportement.Controls.Add(lblAideNoShow)
        carteComportement.Controls.Add(chkNoShowFacturable)
        carteComportement.Controls.Add(lblAideTarif)
        carteComportement.Controls.Add(chkTarifConvention)
        carteComportement.Dock = DockStyle.Fill
        carteComportement.Location = New Point(290, 0)
        carteComportement.Margin = New Padding(6, 0, 0, 0)
        carteComportement.Name = "carteComportement"
        carteComportement.Size = New Size(291, 256)
        carteComportement.TabIndex = 1
        carteComportement.Titre = "Comportement (convention)"
        ' 
        ' chkTarifConvention
        ' 
        chkTarifConvention.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        chkTarifConvention.AutoSize = False
        chkTarifConvention.Font = New Font("Calibri", 10F, FontStyle.Bold)
        chkTarifConvention.Location = New Point(18, 48)
        chkTarifConvention.Name = "chkTarifConvention"
        chkTarifConvention.Size = New Size(255, 24)
        chkTarifConvention.TabIndex = 0
        chkTarifConvention.Text = "Tarif conventionné"
        ' 
        ' lblAideTarif
        ' 
        lblAideTarif.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblAideTarif.Font = New Font("Calibri", 9.75F)
        lblAideTarif.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblAideTarif.Location = New Point(38, 72)
        lblAideTarif.Name = "lblAideTarif"
        lblAideTarif.Size = New Size(235, 34)
        lblAideTarif.TabIndex = 1
        lblAideTarif.Text = "Le tarif vient des tarifs rattachés au domaine (étape 2), pas du tarif patient du dossier."
        ' 
        ' chkNoShowFacturable
        ' 
        chkNoShowFacturable.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        chkNoShowFacturable.AutoSize = False
        chkNoShowFacturable.Checked = True
        chkNoShowFacturable.CheckState = CheckState.Checked
        chkNoShowFacturable.Font = New Font("Calibri", 10F, FontStyle.Bold)
        chkNoShowFacturable.Location = New Point(18, 110)
        chkNoShowFacturable.Name = "chkNoShowFacturable"
        chkNoShowFacturable.Size = New Size(255, 24)
        chkNoShowFacturable.TabIndex = 2
        chkNoShowFacturable.Text = "Absence non excusée facturable"
        ' 
        ' lblAideNoShow
        ' 
        lblAideNoShow.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblAideNoShow.Font = New Font("Calibri", 9.75F)
        lblAideNoShow.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblAideNoShow.Location = New Point(38, 134)
        lblAideNoShow.Name = "lblAideNoShow"
        lblAideNoShow.Size = New Size(235, 34)
        lblAideNoShow.TabIndex = 3
        lblAideNoShow.Text = "Une absence non excusée reste due par le patient."
        ' 
        ' chkSoumisQuota
        ' 
        chkSoumisQuota.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        chkSoumisQuota.AutoSize = False
        chkSoumisQuota.Font = New Font("Calibri", 10F, FontStyle.Bold)
        chkSoumisQuota.Location = New Point(18, 172)
        chkSoumisQuota.Name = "chkSoumisQuota"
        chkSoumisQuota.Size = New Size(255, 24)
        chkSoumisQuota.TabIndex = 4
        chkSoumisQuota.Text = "Soumis à un quota de séances"
        ' 
        ' lblAideQuota
        ' 
        lblAideQuota.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblAideQuota.Font = New Font("Calibri", 9.75F)
        lblAideQuota.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblAideQuota.Location = New Point(38, 196)
        lblAideQuota.Name = "lblAideQuota"
        lblAideQuota.Size = New Size(235, 34)
        lblAideQuota.TabIndex = 5
        lblAideQuota.Text = "Les séances sont décomptées selon les règles de l'étape 3."
        ' 
        ' tabTarifs
        ' 
        tabTarifs.BackColor = Color.FromArgb(CByte(237), CByte(231), CByte(224))
        tabTarifs.Font = New Font("Calibri", 10F)
        tabTarifs.Location = New Point(4, 27)
        tabTarifs.Name = "tabTarifs"
        tabTarifs.Padding = New Padding(12)
        tabTarifs.Size = New Size(605, 491)
        tabTarifs.TabIndex = 1
        tabTarifs.Text = "2. Tarifs"
        ' 
        ' tabQuotas
        ' 
        tabQuotas.BackColor = Color.FromArgb(CByte(237), CByte(231), CByte(224))
        tabQuotas.Controls.Add(carteRegles)
        tabQuotas.Controls.Add(carteQuotaNonNecessaire)
        tabQuotas.Controls.Add(lblAideQuotas)
        tabQuotas.Controls.Add(pnlEspaceQuotas)
        tabQuotas.Controls.Add(carteCategories)
        tabQuotas.Font = New Font("Calibri", 10F)
        tabQuotas.Location = New Point(4, 27)
        tabQuotas.Name = "tabQuotas"
        tabQuotas.Padding = New Padding(12)
        tabQuotas.Size = New Size(605, 491)
        tabQuotas.TabIndex = 2
        tabQuotas.Text = "3. Catégories && quotas"
        ' 
        ' carteCategories
        ' 
        carteCategories.Controls.Add(flpCategories)
        carteCategories.Controls.Add(btnQuotaCategories)
        carteCategories.Controls.Add(lblCategoriesTitre)
        carteCategories.Dock = DockStyle.Top
        carteCategories.Location = New Point(12, 12)
        carteCategories.Name = "carteCategories"
        carteCategories.Padding = New Padding(16, 8, 10, 10)
        carteCategories.Size = New Size(581, 58)
        carteCategories.TabIndex = 0
        ' 
        ' lblCategoriesTitre
        ' 
        lblCategoriesTitre.BackColor = Color.Transparent
        lblCategoriesTitre.Dock = DockStyle.Left
        lblCategoriesTitre.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblCategoriesTitre.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblCategoriesTitre.Location = New Point(16, 8)
        lblCategoriesTitre.Name = "lblCategoriesTitre"
        lblCategoriesTitre.Size = New Size(160, 40)
        lblCategoriesTitre.TabIndex = 0
        lblCategoriesTitre.Text = "Catégories de quota"
        lblCategoriesTitre.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' flpCategories
        ' 
        flpCategories.BackColor = Color.Transparent
        flpCategories.Dock = DockStyle.Fill
        flpCategories.Location = New Point(176, 8)
        flpCategories.Name = "flpCategories"
        flpCategories.Padding = New Padding(0, 9, 0, 0)
        flpCategories.Size = New Size(205, 40)
        flpCategories.TabIndex = 1
        flpCategories.WrapContents = False
        ' 
        ' btnQuotaCategories
        ' 
        btnQuotaCategories.Dock = DockStyle.Right
        btnQuotaCategories.Location = New Point(381, 8)
        btnQuotaCategories.Name = "btnQuotaCategories"
        btnQuotaCategories.Size = New Size(190, 40)
        btnQuotaCategories.TabIndex = 2
        btnQuotaCategories.Tag = "modifier_normal"
        btnQuotaCategories.Text = "Gérer les catégories…"
        btnQuotaCategories.Variante = VarianteBouton.Outil
        ' 
        ' pnlEspaceQuotas
        ' 
        pnlEspaceQuotas.BackColor = Color.Transparent
        pnlEspaceQuotas.Dock = DockStyle.Top
        pnlEspaceQuotas.Location = New Point(12, 70)
        pnlEspaceQuotas.Name = "pnlEspaceQuotas"
        pnlEspaceQuotas.Size = New Size(581, 10)
        pnlEspaceQuotas.TabIndex = 1
        ' 
        ' carteRegles
        ' 
        carteRegles.Controls.Add(dgvQuotas)
        carteRegles.Controls.Add(pnlQuotaEdition)
        carteRegles.Controls.Add(barreRegles)
        carteRegles.Dock = DockStyle.Fill
        carteRegles.Location = New Point(12, 80)
        carteRegles.Name = "carteRegles"
        carteRegles.Padding = New Padding(12, 8, 12, 12)
        carteRegles.Size = New Size(581, 359)
        carteRegles.TabIndex = 2
        ' 
        ' barreRegles
        ' 
        barreRegles.Controls.Add(btnQuotaSupprimer)
        barreRegles.Controls.Add(btnQuotaActiver)
        barreRegles.Controls.Add(btnQuotaCloturer)
        barreRegles.Controls.Add(btnQuotaModifier)
        barreRegles.Controls.Add(btnQuotaAjouter)
        barreRegles.Location = New Point(12, 8)
        barreRegles.Name = "barreRegles"
        barreRegles.Size = New Size(557, 42)
        barreRegles.TabIndex = 0
        barreRegles.Titre = "Règles de quota"
        ' 
        ' btnQuotaAjouter
        ' 
        btnQuotaAjouter.Location = New Point(132, 6)
        btnQuotaAjouter.Name = "btnQuotaAjouter"
        btnQuotaAjouter.Size = New Size(150, 30)
        btnQuotaAjouter.TabIndex = 0
        btnQuotaAjouter.Tag = "nouveau_normal"
        btnQuotaAjouter.Text = "Nouvelle règle"
        btnQuotaAjouter.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' btnQuotaModifier
        ' 
        btnQuotaModifier.Location = New Point(286, 6)
        btnQuotaModifier.Name = "btnQuotaModifier"
        btnQuotaModifier.Size = New Size(178, 30)
        btnQuotaModifier.TabIndex = 4
        btnQuotaModifier.Tag = "modifier_normal"
        btnQuotaModifier.Text = "Modifier la règle"
        btnQuotaModifier.Variante = VarianteBouton.Outil
        ' 
        ' btnQuotaCloturer
        ' 
        btnQuotaCloturer.Location = New Point(468, 6)
        btnQuotaCloturer.Name = "btnQuotaCloturer"
        btnQuotaCloturer.Size = New Size(160, 30)
        btnQuotaCloturer.TabIndex = 1
        btnQuotaCloturer.Tag = "cloturer_normal"
        btnQuotaCloturer.Text = "Clôturer la règle"
        btnQuotaCloturer.Variante = VarianteBouton.Outil
        ' 
        ' btnQuotaActiver
        ' 
        btnQuotaActiver.Location = New Point(632, 6)
        btnQuotaActiver.Name = "btnQuotaActiver"
        btnQuotaActiver.Size = New Size(125, 30)
        btnQuotaActiver.TabIndex = 2
        btnQuotaActiver.Tag = "activer_normal"
        btnQuotaActiver.Text = "Désactiver"
        btnQuotaActiver.Variante = VarianteBouton.Outil
        ' 
        ' btnQuotaSupprimer
        ' 
        btnQuotaSupprimer.Location = New Point(767, 6)
        btnQuotaSupprimer.Name = "btnQuotaSupprimer"
        btnQuotaSupprimer.Size = New Size(120, 30)
        btnQuotaSupprimer.TabIndex = 3
        btnQuotaSupprimer.Tag = "supprimer_normal"
        btnQuotaSupprimer.Text = "Supprimer"
        btnQuotaSupprimer.Variante = VarianteBouton.OutilDanger
        ' 
        ' dgvQuotas
        ' 
        dgvQuotas.AllowUserToAddRows = False
        dgvQuotas.AllowUserToDeleteRows = False
        dgvQuotas.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvQuotas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvQuotas.Columns.AddRange(New DataGridViewColumn() {colQuotaPeriode, colQuotaPlafonds, colQuotaAlerte, colQuotaEtat})
        dgvQuotas.Dock = DockStyle.Fill
        dgvQuotas.Location = New Point(12, 50)
        dgvQuotas.MultiSelect = False
        dgvQuotas.Name = "dgvQuotas"
        dgvQuotas.ReadOnly = True
        dgvQuotas.RowHeadersVisible = False
        dgvQuotas.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvQuotas.Size = New Size(557, 93)
        dgvQuotas.TabIndex = 1
        ' 
        ' colQuotaPeriode
        ' 
        colQuotaPeriode.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colQuotaPeriode.FillWeight = 140F
        colQuotaPeriode.HeaderText = "Période de validité"
        colQuotaPeriode.Name = "colQuotaPeriode"
        colQuotaPeriode.ReadOnly = True
        ' 
        ' colQuotaPlafonds
        ' 
        colQuotaPlafonds.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colQuotaPlafonds.FillWeight = 200F
        colQuotaPlafonds.HeaderText = "Plafonds"
        colQuotaPlafonds.Name = "colQuotaPlafonds"
        colQuotaPlafonds.ReadOnly = True
        ' 
        ' colQuotaAlerte
        ' 
        colQuotaAlerte.HeaderText = "Alerte"
        colQuotaAlerte.Name = "colQuotaAlerte"
        colQuotaAlerte.ReadOnly = True
        colQuotaAlerte.Width = 70
        ' 
        ' colQuotaEtat
        ' 
        colQuotaEtat.HeaderText = "État"
        colQuotaEtat.Name = "colQuotaEtat"
        colQuotaEtat.ReadOnly = True
        colQuotaEtat.Width = 110
        ' 
        ' pnlQuotaEdition
        ' 
        pnlQuotaEdition.BackColor = Color.Transparent
        pnlQuotaEdition.Controls.Add(btnQuotaAnnuler)
        pnlQuotaEdition.Controls.Add(btnQuotaValider)
        pnlQuotaEdition.Controls.Add(lblQuotaTotal)
        pnlQuotaEdition.Controls.Add(dgvQuotaPlafonds)
        pnlQuotaEdition.Controls.Add(lblPlafondsTitre)
        pnlQuotaEdition.Controls.Add(dtpQuotaFin)
        pnlQuotaEdition.Controls.Add(chkQuotaFin)
        pnlQuotaEdition.Controls.Add(dtpQuotaDebut)
        pnlQuotaEdition.Controls.Add(lblQuotaDebut)
        pnlQuotaEdition.Controls.Add(numQuotaAlerte)
        pnlQuotaEdition.Controls.Add(lblQuotaAlerte)
        pnlQuotaEdition.Controls.Add(numQuotaDuree)
        pnlQuotaEdition.Controls.Add(lblQuotaDuree)
        pnlQuotaEdition.Controls.Add(lblQuotaZone)
        pnlQuotaEdition.Dock = DockStyle.Bottom
        pnlQuotaEdition.Location = New Point(12, 143)
        pnlQuotaEdition.Name = "pnlQuotaEdition"
        pnlQuotaEdition.Size = New Size(557, 204)
        pnlQuotaEdition.TabIndex = 2
        ' 
        ' lblQuotaZone
        ' 
        lblQuotaZone.AutoSize = True
        lblQuotaZone.BackColor = Color.Transparent
        lblQuotaZone.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblQuotaZone.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblQuotaZone.Location = New Point(0, 10)
        lblQuotaZone.Name = "lblQuotaZone"
        lblQuotaZone.Size = New Size(124, 17)
        lblQuotaZone.TabIndex = 0
        lblQuotaZone.Text = "Règle sélectionnée"
        ' 
        ' lblQuotaDuree
        ' 
        lblQuotaDuree.AutoSize = True
        lblQuotaDuree.BackColor = Color.Transparent
        lblQuotaDuree.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblQuotaDuree.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblQuotaDuree.Location = New Point(0, 42)
        lblQuotaDuree.Name = "lblQuotaDuree"
        lblQuotaDuree.Size = New Size(160, 17)
        lblQuotaDuree.TabIndex = 1
        lblQuotaDuree.Text = "Période glissante (mois)"
        ' 
        ' numQuotaDuree
        ' 
        numQuotaDuree.Location = New Point(180, 40)
        numQuotaDuree.Maximum = New Decimal(New Integer() {120, 0, 0, 0})
        numQuotaDuree.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numQuotaDuree.Name = "numQuotaDuree"
        numQuotaDuree.Size = New Size(80, 24)
        numQuotaDuree.TabIndex = 2
        numQuotaDuree.Value = New Decimal(New Integer() {12, 0, 0, 0})
        ' 
        ' lblQuotaAlerte
        ' 
        lblQuotaAlerte.AutoSize = True
        lblQuotaAlerte.BackColor = Color.Transparent
        lblQuotaAlerte.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblQuotaAlerte.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblQuotaAlerte.Location = New Point(0, 74)
        lblQuotaAlerte.Name = "lblQuotaAlerte"
        lblQuotaAlerte.Size = New Size(120, 17)
        lblQuotaAlerte.TabIndex = 3
        lblQuotaAlerte.Text = "Alerte à (séances)"
        ' 
        ' numQuotaAlerte
        ' 
        numQuotaAlerte.Location = New Point(180, 72)
        numQuotaAlerte.Maximum = New Decimal(New Integer() {1998, 0, 0, 0})
        numQuotaAlerte.Name = "numQuotaAlerte"
        numQuotaAlerte.Size = New Size(80, 24)
        numQuotaAlerte.TabIndex = 4
        ' 
        ' lblQuotaDebut
        ' 
        lblQuotaDebut.AutoSize = True
        lblQuotaDebut.BackColor = Color.Transparent
        lblQuotaDebut.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblQuotaDebut.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblQuotaDebut.Location = New Point(0, 106)
        lblQuotaDebut.Name = "lblQuotaDebut"
        lblQuotaDebut.Size = New Size(71, 17)
        lblQuotaDebut.TabIndex = 5
        lblQuotaDebut.Text = "À partir du"
        ' 
        ' dtpQuotaDebut
        ' 
        dtpQuotaDebut.Font = New Font("Calibri", 10.5F)
        dtpQuotaDebut.Location = New Point(180, 102)
        dtpQuotaDebut.Name = "dtpQuotaDebut"
        dtpQuotaDebut.Size = New Size(130, 28)
        dtpQuotaDebut.TabIndex = 6
        ' 
        ' chkQuotaFin
        ' 
        chkQuotaFin.AutoSize = False
        chkQuotaFin.Font = New Font("Calibri", 10F, FontStyle.Bold)
        chkQuotaFin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkQuotaFin.Location = New Point(0, 134)
        chkQuotaFin.Name = "chkQuotaFin"
        chkQuotaFin.Size = New Size(150, 26)
        chkQuotaFin.TabIndex = 7
        chkQuotaFin.Text = "Jusqu'au"
        ' 
        ' dtpQuotaFin
        ' 
        dtpQuotaFin.Enabled = False
        dtpQuotaFin.Font = New Font("Calibri", 10.5F)
        dtpQuotaFin.Location = New Point(180, 134)
        dtpQuotaFin.Name = "dtpQuotaFin"
        dtpQuotaFin.Size = New Size(130, 28)
        dtpQuotaFin.TabIndex = 8
        ' 
        ' lblPlafondsTitre
        ' 
        lblPlafondsTitre.AutoSize = True
        lblPlafondsTitre.BackColor = Color.Transparent
        lblPlafondsTitre.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblPlafondsTitre.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblPlafondsTitre.Location = New Point(330, 10)
        lblPlafondsTitre.Name = "lblPlafondsTitre"
        lblPlafondsTitre.Size = New Size(150, 17)
        lblPlafondsTitre.TabIndex = 9
        lblPlafondsTitre.Text = "Plafonds par catégorie"
        ' 
        ' dgvQuotaPlafonds
        ' 
        dgvQuotaPlafonds.AllowUserToAddRows = False
        dgvQuotaPlafonds.AllowUserToDeleteRows = False
        dgvQuotaPlafonds.AllowUserToResizeRows = False
        dgvQuotaPlafonds.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        dgvQuotaPlafonds.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvQuotaPlafonds.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvQuotaPlafonds.Columns.AddRange(New DataGridViewColumn() {colPlafondCategorie, colPlafondValeur})
        dgvQuotaPlafonds.Location = New Point(330, 36)
        dgvQuotaPlafonds.Name = "dgvQuotaPlafonds"
        dgvQuotaPlafonds.RowHeadersVisible = False
        dgvQuotaPlafonds.Size = New Size(227, 104)
        dgvQuotaPlafonds.TabIndex = 10
        ' 
        ' colPlafondCategorie
        ' 
        colPlafondCategorie.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colPlafondCategorie.HeaderText = "Catégorie"
        colPlafondCategorie.Name = "colPlafondCategorie"
        colPlafondCategorie.ReadOnly = True
        ' 
        ' colPlafondValeur
        ' 
        colPlafondValeur.HeaderText = "Plafond"
        colPlafondValeur.Name = "colPlafondValeur"
        colPlafondValeur.Width = 80
        ' 
        ' lblQuotaTotal
        ' 
        lblQuotaTotal.AutoSize = True
        lblQuotaTotal.BackColor = Color.Transparent
        lblQuotaTotal.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblQuotaTotal.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        lblQuotaTotal.Location = New Point(330, 146)
        lblQuotaTotal.Name = "lblQuotaTotal"
        lblQuotaTotal.Size = New Size(120, 17)
        lblQuotaTotal.TabIndex = 11
        lblQuotaTotal.Text = "Total (calculé) : 0"
        ' 
        ' btnQuotaValider
        ' 
        btnQuotaValider.Location = New Point(0, 170)
        btnQuotaValider.Name = "btnQuotaValider"
        btnQuotaValider.Size = New Size(150, 30)
        btnQuotaValider.TabIndex = 12
        btnQuotaValider.Text = "Ajouter la règle"
        btnQuotaValider.Variante = VarianteBouton.OutilPrincipal
        btnQuotaValider.Visible = False
        ' 
        ' btnQuotaAnnuler
        ' 
        btnQuotaAnnuler.Location = New Point(156, 170)
        btnQuotaAnnuler.Name = "btnQuotaAnnuler"
        btnQuotaAnnuler.Size = New Size(100, 30)
        btnQuotaAnnuler.TabIndex = 13
        btnQuotaAnnuler.Text = "Annuler"
        btnQuotaAnnuler.Variante = VarianteBouton.Outil
        btnQuotaAnnuler.Visible = False
        ' 
        ' carteQuotaNonNecessaire
        ' 
        carteQuotaNonNecessaire.Controls.Add(lblQuotaNonNecessaire)
        carteQuotaNonNecessaire.Dock = DockStyle.Fill
        carteQuotaNonNecessaire.Location = New Point(12, 80)
        carteQuotaNonNecessaire.Name = "carteQuotaNonNecessaire"
        carteQuotaNonNecessaire.Size = New Size(581, 359)
        carteQuotaNonNecessaire.TabIndex = 3
        carteQuotaNonNecessaire.Visible = False
        ' 
        ' lblQuotaNonNecessaire
        ' 
        lblQuotaNonNecessaire.BackColor = Color.Transparent
        lblQuotaNonNecessaire.Dock = DockStyle.Fill
        lblQuotaNonNecessaire.Font = New Font("Calibri", 11F)
        lblQuotaNonNecessaire.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblQuotaNonNecessaire.Location = New Point(18, 14)
        lblQuotaNonNecessaire.Name = "lblQuotaNonNecessaire"
        lblQuotaNonNecessaire.Size = New Size(545, 329)
        lblQuotaNonNecessaire.TabIndex = 0
        lblQuotaNonNecessaire.Text = "Étape non nécessaire : ce domaine n'est pas soumis à quota (voir l'étape 1. Général)."
        lblQuotaNonNecessaire.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lblAideQuotas
        ' 
        lblAideQuotas.BackColor = Color.Transparent
        lblAideQuotas.Dock = DockStyle.Bottom
        lblAideQuotas.Font = New Font("Calibri", 9.75F)
        lblAideQuotas.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        lblAideQuotas.Location = New Point(12, 439)
        lblAideQuotas.Name = "lblAideQuotas"
        lblAideQuotas.Padding = New Padding(2, 6, 2, 0)
        lblAideQuotas.Size = New Size(581, 40)
        lblAideQuotas.TabIndex = 4
        lblAideQuotas.Text = "Une règle pas encore utilisée se corrige avec « Modifier la règle » ; une règle qui a servi ne se modifie plus."
        ' 
        ' tabNatures
        ' 
        tabNatures.BackColor = Color.FromArgb(CByte(237), CByte(231), CByte(224))
        tabNatures.Font = New Font("Calibri", 10F)
        tabNatures.Location = New Point(4, 27)
        tabNatures.Name = "tabNatures"
        tabNatures.Padding = New Padding(12)
        tabNatures.Size = New Size(605, 491)
        tabNatures.TabIndex = 3
        tabNatures.Text = "4. Natures de séance"
        ' 
        ' tabSynthese
        ' 
        tabSynthese.BackColor = Color.FromArgb(CByte(237), CByte(231), CByte(224))
        tabSynthese.Controls.Add(carteSynthese)
        tabSynthese.Font = New Font("Calibri", 10F)
        tabSynthese.Location = New Point(4, 27)
        tabSynthese.Name = "tabSynthese"
        tabSynthese.Padding = New Padding(12)
        tabSynthese.Size = New Size(605, 491)
        tabSynthese.TabIndex = 4
        tabSynthese.Text = "5. Synthèse"
        ' 
        ' carteSynthese
        ' 
        carteSynthese.Controls.Add(ucSynthese)
        carteSynthese.Dock = DockStyle.Fill
        carteSynthese.Location = New Point(12, 12)
        carteSynthese.Name = "carteSynthese"
        carteSynthese.Padding = New Padding(16, 8, 16, 12)
        carteSynthese.Size = New Size(581, 467)
        carteSynthese.TabIndex = 0
        carteSynthese.Titre = "Convention du domaine"
        ' 
        ' ucSynthese
        ' 
        ucSynthese.Dock = DockStyle.Fill
        ucSynthese.Location = New Point(16, 37)
        ucSynthese.Name = "ucSynthese"
        ucSynthese.Size = New Size(549, 418)
        ucSynthese.TabIndex = 0
        ' 
        ' pnlAssistant
        ' 
        pnlAssistant.Controls.Add(lblAssistantEtape)
        pnlAssistant.Controls.Add(btnAssistantPrecedent)
        pnlAssistant.Controls.Add(btnAssistantSuivant)
        pnlAssistant.CouleurBordure = Color.Transparent
        pnlAssistant.CouleurFond = Color.FromArgb(CByte(230), CByte(238), CByte(233))
        pnlAssistant.Dock = DockStyle.Top
        pnlAssistant.Location = New Point(8, 130)
        pnlAssistant.Name = "pnlAssistant"
        pnlAssistant.Ombre = False
        pnlAssistant.OpaciteFond = 255
        pnlAssistant.Padding = New Padding(12, 6, 8, 6)
        pnlAssistant.Rayon = 10
        pnlAssistant.Size = New Size(613, 48)
        pnlAssistant.TabIndex = 2
        ' 
        ' lblAssistantEtape
        ' 
        lblAssistantEtape.BackColor = Color.Transparent
        lblAssistantEtape.Dock = DockStyle.Fill
        lblAssistantEtape.Font = New Font("Calibri", 10F)
        lblAssistantEtape.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        lblAssistantEtape.Location = New Point(12, 6)
        lblAssistantEtape.Name = "lblAssistantEtape"
        lblAssistantEtape.Size = New Size(383, 36)
        lblAssistantEtape.TabIndex = 0
        lblAssistantEtape.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' btnAssistantPrecedent
        ' 
        btnAssistantPrecedent.Dock = DockStyle.Right
        btnAssistantPrecedent.Location = New Point(395, 6)
        btnAssistantPrecedent.Name = "btnAssistantPrecedent"
        btnAssistantPrecedent.Size = New Size(110, 36)
        btnAssistantPrecedent.TabIndex = 1
        btnAssistantPrecedent.Text = "◂ Précédent"
        btnAssistantPrecedent.Variante = VarianteBouton.Outil
        ' 
        ' btnAssistantSuivant
        ' 
        btnAssistantSuivant.Dock = DockStyle.Right
        btnAssistantSuivant.Location = New Point(505, 6)
        btnAssistantSuivant.Name = "btnAssistantSuivant"
        btnAssistantSuivant.Size = New Size(100, 36)
        btnAssistantSuivant.TabIndex = 2
        btnAssistantSuivant.Text = "Suivant ▸"
        btnAssistantSuivant.Variante = VarianteBouton.Outil
        ' 
        ' etapes
        ' 
        etapes.Dock = DockStyle.Top
        etapes.Etapes = New String() {"1. Général", "2. Tarifs", "3. Catégories & quotas", "4. Natures de séance", "5. Synthèse"}
        etapes.Location = New Point(8, 38)
        etapes.Name = "etapes"
        etapes.Padding = New Padding(0, 4, 0, 4)
        etapes.Size = New Size(613, 92)
        etapes.TabIndex = 1
        ' 
        ' lblDomaineCourant
        ' 
        lblDomaineCourant.BackColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDomaineCourant.Controls.Add(pstEtatDomaine)
        lblDomaineCourant.Dock = DockStyle.Top
        lblDomaineCourant.Font = New Font("Calibri", 12F, FontStyle.Bold)
        lblDomaineCourant.ForeColor = Color.White
        lblDomaineCourant.Location = New Point(8, 8)
        lblDomaineCourant.Name = "lblDomaineCourant"
        lblDomaineCourant.Padding = New Padding(8, 0, 0, 0)
        lblDomaineCourant.Size = New Size(613, 30)
        lblDomaineCourant.TabIndex = 0
        lblDomaineCourant.Text = "Aucun domaine sélectionné"
        lblDomaineCourant.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' pstEtatDomaine
        ' 
        pstEtatDomaine.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        pstEtatDomaine.AutoSize = True
        pstEtatDomaine.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        pstEtatDomaine.Location = New Point(500, 5)
        pstEtatDomaine.Name = "pstEtatDomaine"
        pstEtatDomaine.Size = New Size(105, 20)
        pstEtatDomaine.TabIndex = 0
        pstEtatDomaine.Text = "Domaine complet"
        pstEtatDomaine.Visible = False
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Controls.Add(btnActiverDesactiver)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(965, 60)
        pnlActions.TabIndex = 2
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(835, 10)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 40)
        btnAnnuler.TabIndex = 4
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(693, 10)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 40)
        btnEnregistrer.TabIndex = 3
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnActiverDesactiver
        ' 
        btnActiverDesactiver.Image = CType(resources.GetObject("btnActiverDesactiver.Image"), Image)
        btnActiverDesactiver.Location = New Point(269, 10)
        btnActiverDesactiver.Name = "btnActiverDesactiver"
        btnActiverDesactiver.Size = New Size(130, 40)
        btnActiverDesactiver.TabIndex = 2
        btnActiverDesactiver.Tag = "activer_normal"
        btnActiverDesactiver.Text = "Désactiver"
        ' 
        ' btnModifier
        ' 
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(140, 10)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(119, 40)
        btnModifier.TabIndex = 1
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        ' 
        ' btnNouveau
        ' 
        btnNouveau.Image = CType(resources.GetObject("btnNouveau.Image"), Image)
        btnNouveau.Location = New Point(10, 10)
        btnNouveau.Name = "btnNouveau"
        btnNouveau.Size = New Size(120, 40)
        btnNouveau.TabIndex = 0
        btnNouveau.Tag = "nouveau_normal"
        btnNouveau.Text = "Nouveau"
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 0
        enTete.SousTitre = "Créez un domaine puis complétez ses natures, tarifs et quotas : la synthèse vous guide."
        enTete.Text = "Domaines"
        ' 
        ' UC_DomainesHome
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Name = "UC_DomainesHome"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        splMain.Panel1.ResumeLayout(False)
        splMain.Panel2.ResumeLayout(False)
        CType(splMain, ComponentModel.ISupportInitialize).EndInit()
        splMain.ResumeLayout(False)
        CType(dgvDomaines, ComponentModel.ISupportInitialize).EndInit()
        pnlListeTop.ResumeLayout(False)
        pnlListeTop.PerformLayout()
        tabDomaine.ResumeLayout(False)
        tabGeneral.ResumeLayout(False)
        tblGeneral.ResumeLayout(False)
        carteComportement.ResumeLayout(False)
        carteIdentite.ResumeLayout(False)
        carteIdentite.PerformLayout()
        carteLogo.ResumeLayout(False)
        CType(picLogo, ComponentModel.ISupportInitialize).EndInit()
        CType(numOrdre, ComponentModel.ISupportInitialize).EndInit()
        tabTarifs.ResumeLayout(False)
        tabQuotas.ResumeLayout(False)
        CType(dgvQuotas, ComponentModel.ISupportInitialize).EndInit()
        carteCategories.ResumeLayout(False)
        carteRegles.ResumeLayout(False)
        barreRegles.ResumeLayout(False)
        carteQuotaNonNecessaire.ResumeLayout(False)
        pnlQuotaEdition.ResumeLayout(False)
        pnlQuotaEdition.PerformLayout()
        CType(numQuotaAlerte, ComponentModel.ISupportInitialize).EndInit()
        CType(numQuotaDuree, ComponentModel.ISupportInitialize).EndInit()
        CType(dgvQuotaPlafonds, ComponentModel.ISupportInitialize).EndInit()
        tabNatures.ResumeLayout(False)
        tabSynthese.ResumeLayout(False)
        carteSynthese.ResumeLayout(False)
        pnlAssistant.ResumeLayout(False)
        lblDomaineCourant.ResumeLayout(False)
        lblDomaineCourant.PerformLayout()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents splMain As SplitContainer
    Friend WithEvents dgvDomaines As DataGridView
    Friend WithEvents colId As DataGridViewTextBoxColumn
    Friend WithEvents colCode As DataGridViewTextBoxColumn
    Friend WithEvents colLibelle As DataGridViewTextBoxColumn
    Friend WithEvents colEtat As DataGridViewTextBoxColumn
    Friend WithEvents pnlListeTop As AltheaCarte
    Friend WithEvents chkAfficherInactifs As AltheaCase
    Friend WithEvents txtRecherche As AltheaRecherche
    Friend WithEvents tabDomaine As AltheaOnglets
    Friend WithEvents tabGeneral As TabPage
    Friend WithEvents tblGeneral As TableLayoutPanel
    Friend WithEvents carteIdentite As AltheaCarte
    Friend WithEvents carteLogo As AltheaCarte
    Friend WithEvents picLogo As PictureBox
    Friend WithEvents btnLogoImporter As AltheaBouton
    Friend WithEvents btnLogoRetirer As AltheaBouton
    Friend WithEvents lblAideLogo As Label
    Friend WithEvents lblOrganisme As Label
    Friend WithEvents cboOrganisme As AltheaListe
    Friend WithEvents lblDomaineCourant As Label
    Friend WithEvents pnlAssistant As AltheaCarte
    Friend WithEvents etapes As AltheaEtapes
    Friend WithEvents pstEtatDomaine As AltheaPastille
    Friend WithEvents lblAssistantEtape As Label
    Friend WithEvents btnQuotaCategories As AltheaBouton
    Friend WithEvents btnNouvelOrganisme As AltheaBouton
    Friend WithEvents lblCode As Label
    Friend WithEvents txtCode As TextBox
    Friend WithEvents lblLibelle As Label
    Friend WithEvents txtLibelle As TextBox
    Friend WithEvents lblOrdre As Label
    Friend WithEvents numOrdre As NumericUpDown
    Friend WithEvents chkActif As AltheaCase
    Friend WithEvents carteComportement As AltheaCarte
    Friend WithEvents chkTarifConvention As AltheaCase
    Friend WithEvents lblAideTarif As Label
    Friend WithEvents chkNoShowFacturable As AltheaCase
    Friend WithEvents lblAideNoShow As Label
    Friend WithEvents chkSoumisQuota As AltheaCase
    Friend WithEvents lblAideQuota As Label
    Friend WithEvents tabNatures As TabPage
    Friend WithEvents tabTarifs As TabPage
    Friend WithEvents tabQuotas As TabPage
    Friend WithEvents lblAideQuotas As Label
    Friend WithEvents dgvQuotas As DataGridView
    Friend WithEvents colQuotaPeriode As DataGridViewTextBoxColumn
    Friend WithEvents colQuotaPlafonds As DataGridViewTextBoxColumn
    Friend WithEvents colQuotaAlerte As DataGridViewTextBoxColumn
    Friend WithEvents colQuotaEtat As DataGridViewTextBoxColumn
    Friend WithEvents pnlQuotaEdition As Panel
    Friend WithEvents dgvQuotaPlafonds As DataGridView
    Friend WithEvents colPlafondCategorie As DataGridViewTextBoxColumn
    Friend WithEvents colPlafondValeur As DataGridViewTextBoxColumn
    Friend WithEvents lblQuotaDuree As Label
    Friend WithEvents numQuotaDuree As NumericUpDown
    Friend WithEvents lblQuotaAlerte As Label
    Friend WithEvents numQuotaAlerte As NumericUpDown
    Friend WithEvents lblQuotaDebut As Label
    Friend WithEvents dtpQuotaDebut As AltheaDate
    Friend WithEvents chkQuotaFin As AltheaCase
    Friend WithEvents dtpQuotaFin As AltheaDate
    Friend WithEvents btnQuotaAjouter As AltheaBouton
    Friend WithEvents btnQuotaModifier As AltheaBouton
    Friend WithEvents btnQuotaCloturer As AltheaBouton
    Friend WithEvents btnQuotaActiver As AltheaBouton
    Friend WithEvents btnQuotaSupprimer As AltheaBouton
    Friend WithEvents carteCategories As AltheaCarte
    Friend WithEvents lblCategoriesTitre As Label
    Friend WithEvents flpCategories As FlowLayoutPanel
    Friend WithEvents pnlEspaceQuotas As Panel
    Friend WithEvents carteRegles As AltheaCarte
    Friend WithEvents barreRegles As AltheaBarreOutils
    Friend WithEvents lblQuotaZone As Label
    Friend WithEvents lblPlafondsTitre As Label
    Friend WithEvents lblQuotaTotal As Label
    Friend WithEvents btnQuotaValider As AltheaBouton
    Friend WithEvents btnQuotaAnnuler As AltheaBouton
    Friend WithEvents carteQuotaNonNecessaire As AltheaCarte
    Friend WithEvents lblQuotaNonNecessaire As Label
    Friend WithEvents tabSynthese As TabPage
    Friend WithEvents carteSynthese As AltheaCarte
    Friend WithEvents ucSynthese As UC_SyntheseDomaine
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnNouveau As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents btnActiverDesactiver As AltheaBouton
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents btnAssistantPrecedent As AltheaBouton
    Friend WithEvents btnAssistantSuivant As AltheaBouton

End Class
