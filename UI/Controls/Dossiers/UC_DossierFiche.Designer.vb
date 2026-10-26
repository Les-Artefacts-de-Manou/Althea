' -------------------------------------------------------------------------------------------------
' Fichier     : UC_DossierFiche.Designer.vb
' Projet      : Althéa
' Version     : V1.6.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Code généré (layout) du hub dossier UC_DossierFiche : en-tête (code du dossier, patient),
' récapitulatif patient en quatre blocs (Patient / Dossier / Finances / Quotas), TabControl des
' sous-onglets (Fiche dossier / Suivi clinique / Documents / Séances / Paiements) et barre d'actions.
'
' Remarques   :
' - UI-5 : récapitulatif (carteRecap → tblRecap → pnlRecap*) ; onglet Fiche dossier en deux cartes
'   (carteParametres : champs en ligne ; carteConvention : UC_SyntheseDomaine).
' - Les panneaux pnlRecap* sont ancrés dans un TableLayoutPanel : leur Size déclarée doit rester
'   cohérente avec la position de leurs enfants ancrés à droite, sinon ceux-ci débordent.
' - V1.4.0 (17/10/2026) : pnlDossierConsultation / lblDossierConsultation, bandeau « consultation
'   seulement » d'un dossier clôturé ou archivé, entre l'en-tête et le récapitulatif (masqué par
'   défaut ; ordre d'ajout : carteRecap, pnlDossierConsultation, enTete — le dernier ajouté se
'   docke en premier).
' - V1.5.0 (20/10/2026) : picLogoDomaine (logo du domaine, bandeau, à droite du bloc Dossier) ;
'   cboDomaine dessiné par l'écran (logo devant chaque domaine) ; récapitulatif : colonne Dossier
'   élargie (24 / 31 / 25 / 20 %) pour loger le logo.
' - V1.6.0 (23/10/2026, relevé de Joëlle) : colonne « Code » (colPaiementCode) dans la grille des
'   paiements.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_DossierFiche
    Inherits UserControl

    ' UserControl remplace Dispose pour nettoyer la liste des composants.
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

    ' Requis par le concepteur Windows Forms.
    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_DossierFiche))
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        pnlBandeau = New Panel()
        pnlDossierConsultation = New Panel()
        lblDossierConsultation = New Label()
        carteRecap = New AltheaCarte()
        tblRecap = New TableLayoutPanel()
        pnlRecapPatient = New Panel()
        pnlRecapDossier = New Panel()
        pnlRecapFinances = New Panel()
        pnlRecapQuotas = New Panel()
        lblRecapTitrePatient = New Label()
        lblRecapTitreDossier = New Label()
        lblRecapTitreFinances = New Label()
        lblRecapTitreQuotas = New Label()
        picRecapPhoto = New PictureBox()
        lblRecapNaissance = New Label()
        lblRecapAlerte = New Label()
        jaugeQuota = New AltheaJauge()
        lblQuotaAucun = New Label()
        enTete = New AltheaEnTete()
        lblDomaineLegende = New Label()
        lblDomaineValeur = New Label()
        picLogoDomaine = New PictureBox()
        lblStatutLegende = New Label()
        lblStatutValeur = New Label()
        lblDateOuvertureLegende = New Label()
        lblDateOuvertureValeur = New Label()
        lblDateClotureLegende = New Label()
        lblDateClotureValeur = New Label()
        btnChangerStatut = New AltheaBouton()
        lblTarifPatientLegende = New Label()
        lblTarifPatientValeur = New Label()
        lblMontantsDusLegende = New Label()
        lblMontantsDusValeur = New Label()
        lblPrevuLegende = New Label()
        lblPrevuValeur = New Label()
        lblAvoirLegende = New Label()
        lblAvoirValeur = New Label()
        lblQuotaSoutienLegende = New Label()
        lblQuotaSoutienValeur = New Label()
        lblQuotaTraitementLegende = New Label()
        lblQuotaTraitementValeur = New Label()
        lblQuotaPeriodeInami = New Label()
        tabDossier = New AltheaOnglets()
        tabPageFicheDossier = New TabPage()
        lblDomaineEditLegende = New Label()
        cboDomaine = New AltheaListe()
        ucSyntheseDomaine = New UC_SyntheseDomaine()
        carteParametres = New AltheaCarte()
        carteConvention = New AltheaCarte()
        lblDateOuvertureEditLegende = New Label()
        dtpDateOuverture = New AltheaDate()
        lblModalitesLegende = New Label()
        txtModalitesPaiement = New TextBox()
        tabPageSuiviClinique = New TabPage()
        carteZones = New AltheaCarte()
        tlpZones = New TableLayoutPanel()
        tabPageDocuments = New TabPage()
        carteDocuments = New AltheaCarte()
        lblDocumentsAVenir = New Label()
        tabPageSeances = New TabPage()
        dgvSeances = New DataGridView()
        pagSeances = New UC_Pagination()
        colSeanceCode = New DataGridViewTextBoxColumn()
        colSeanceDate = New DataGridViewTextBoxColumn()
        colSeanceHoraires = New DataGridViewTextBoxColumn()
        colSeanceType = New DataGridViewTextBoxColumn()
        colSeancePeriodeInami = New DataGridViewTextBoxColumn()
        colSeanceQuotaInami = New DataGridViewTextBoxColumn()
        colSeanceStatut = New DataGridViewTextBoxColumn()
        colSeanceEtatPaiement = New DataGridViewTextBoxColumn()
        colSeanceTarif = New DataGridViewTextBoxColumn()
        colSeanceRegle = New DataGridViewTextBoxColumn()
        pnlSeancesFiltres = New Panel()
        carteFiltresSeances = New AltheaCarte()
        txtRechercheSeance = New AltheaRecherche()
        cboFiltreSeance = New AltheaListe()
        cboFiltrePeriodeInami = New AltheaListe()
        chkPeriodeSeance = New AltheaCase()
        dtpSeanceDu = New AltheaDate()
        lblSeanceAu = New Label()
        dtpSeanceAu = New AltheaDate()
        carteSeances = New AltheaCarte()
        pnlResumeSeances = New FlowLayoutPanel()
        pnlSeancesActions = New AltheaBarreOutils()
        btnNouvelleSeance = New AltheaBouton()
        btnModifierSeance = New AltheaBouton()
        btnReporterSeance = New AltheaBouton()
        btnSupprimerSeance = New AltheaBouton()
        btnEncaisserSeance = New AltheaBouton()
        btnDetailPaiementsSeance = New AltheaBouton()
        tabPagePaiements = New TabPage()
        dgvPaiements = New DataGridView()
        pagPaiements = New UC_Pagination()
        colPaiementDate = New DataGridViewTextBoxColumn()
        colPaiementCode = New DataGridViewTextBoxColumn()
        colPaiementMouvement = New DataGridViewTextBoxColumn()
        colPaiementSeance = New DataGridViewTextBoxColumn()
        colPaiementMode = New DataGridViewTextBoxColumn()
        colPaiementMontant = New DataGridViewTextBoxColumn()
        colPaiementEtat = New DataGridViewTextBoxColumn()
        pnlResumePaiements = New FlowLayoutPanel()
        pnlPaiementsFiltres = New Panel()
        carteFiltresPaiements = New AltheaCarte()
        txtRecherchePaiement = New AltheaRecherche()
        cboFiltrePaiement = New AltheaListe()
        chkPeriodePaiement = New AltheaCase()
        dtpPaiementDu = New AltheaDate()
        lblPaiementAu = New Label()
        dtpPaiementAu = New AltheaDate()
        cartePaiements = New AltheaCarte()
        pnlPaiementsActions = New AltheaBarreOutils()
        btnSupprimerPaiement = New AltheaBouton()
        pnlActions = New AltheaBarreActions()
        btnFermer = New AltheaBouton()
        btnModifier = New AltheaBouton()
        btnEnregistrer = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        pnlBandeau.SuspendLayout()
        tabDossier.SuspendLayout()
        tabPageFicheDossier.SuspendLayout()
        carteZones.SuspendLayout()
        carteDocuments.SuspendLayout()
        carteParametres.SuspendLayout()
        carteConvention.SuspendLayout()
        tabPageSuiviClinique.SuspendLayout()
        tabPageDocuments.SuspendLayout()
        tabPageSeances.SuspendLayout()
        CType(dgvSeances, ComponentModel.ISupportInitialize).BeginInit()
        pnlSeancesFiltres.SuspendLayout()
        carteFiltresSeances.SuspendLayout()
        pnlSeancesActions.SuspendLayout()
        tabPagePaiements.SuspendLayout()
        CType(dgvPaiements, ComponentModel.ISupportInitialize).BeginInit()
        pnlPaiementsFiltres.SuspendLayout()
        carteFiltresPaiements.SuspendLayout()
        pnlPaiementsActions.SuspendLayout()
        pnlActions.SuspendLayout()
        CType(picLogoDomaine, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlBandeau
        ' 
        pnlBandeau.Controls.Add(carteRecap)
        pnlBandeau.Controls.Add(pnlDossierConsultation)
        pnlBandeau.Controls.Add(enTete)
        pnlBandeau.BackColor = Color.Transparent
        pnlBandeau.Dock = DockStyle.Top
        pnlBandeau.Location = New Point(0, 0)
        pnlBandeau.Name = "pnlBandeau"
        pnlBandeau.Padding = New Padding(16, 8, 16, 8)
        pnlBandeau.Size = New Size(1221, 190)
        pnlBandeau.TabIndex = 0
        ' 
        ' pnlDossierConsultation
        ' 
        pnlDossierConsultation.Controls.Add(lblDossierConsultation)
        pnlDossierConsultation.Dock = DockStyle.Top
        pnlDossierConsultation.Location = New Point(16, 66)
        pnlDossierConsultation.Name = "pnlDossierConsultation"
        pnlDossierConsultation.Padding = New Padding(0, 2, 0, 6)
        pnlDossierConsultation.Size = New Size(1189, 36)
        pnlDossierConsultation.TabIndex = 15
        pnlDossierConsultation.Visible = False
        ' 
        ' lblDossierConsultation
        ' 
        lblDossierConsultation.Dock = DockStyle.Fill
        lblDossierConsultation.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblDossierConsultation.Location = New Point(0, 2)
        lblDossierConsultation.Name = "lblDossierConsultation"
        lblDossierConsultation.Padding = New Padding(10, 0, 10, 0)
        lblDossierConsultation.Size = New Size(1189, 28)
        lblDossierConsultation.TabIndex = 0
        lblDossierConsultation.Text = "Dossier clôturé — consultation seulement."
        lblDossierConsultation.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' carteRecap
        ' 
        carteRecap.Controls.Add(tblRecap)
        carteRecap.Dock = DockStyle.Fill
        carteRecap.Location = New Point(16, 66)
        carteRecap.Size = New Size(1189, 116)
        carteRecap.Name = "carteRecap"
        carteRecap.Padding = New Padding(8, 6, 8, 6)
        carteRecap.TabIndex = 20
        ' 
        ' tblRecap
        ' 
        tblRecap.BackColor = Color.Transparent
        tblRecap.ColumnCount = 4
        tblRecap.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 24F))
        tblRecap.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 31F))
        tblRecap.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25F))
        tblRecap.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20F))
        tblRecap.Controls.Add(pnlRecapPatient, 0, 0)
        tblRecap.Controls.Add(pnlRecapDossier, 1, 0)
        tblRecap.Controls.Add(pnlRecapFinances, 2, 0)
        tblRecap.Controls.Add(pnlRecapQuotas, 3, 0)
        tblRecap.Dock = DockStyle.Fill
        tblRecap.Location = New Point(8, 6)
        tblRecap.Size = New Size(1173, 104)
        tblRecap.Name = "tblRecap"
        tblRecap.RowCount = 1
        tblRecap.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        tblRecap.TabIndex = 0
        ' 
        ' pnlRecapPatient
        ' 
        pnlRecapPatient.Controls.Add(lblRecapTitrePatient)
        pnlRecapPatient.Controls.Add(picRecapPhoto)
        pnlRecapPatient.Controls.Add(lblRecapNaissance)
        pnlRecapPatient.Controls.Add(lblRecapAlerte)
        pnlRecapPatient.BackColor = Color.Transparent
        pnlRecapPatient.Dock = DockStyle.Fill
        pnlRecapPatient.Location = New Point(0, 0)
        pnlRecapPatient.Size = New Size(282, 104)
        pnlRecapPatient.Margin = New Padding(0)
        pnlRecapPatient.Name = "pnlRecapPatient"
        pnlRecapPatient.TabIndex = 0
        ' 
        ' pnlRecapDossier
        ' 
        pnlRecapDossier.Controls.Add(lblRecapTitreDossier)
        pnlRecapDossier.Controls.Add(lblDomaineLegende)
        pnlRecapDossier.Controls.Add(picLogoDomaine)
        pnlRecapDossier.Controls.Add(lblDomaineValeur)
        pnlRecapDossier.Controls.Add(lblStatutLegende)
        pnlRecapDossier.Controls.Add(lblStatutValeur)
        pnlRecapDossier.Controls.Add(lblDateOuvertureLegende)
        pnlRecapDossier.Controls.Add(lblDateOuvertureValeur)
        pnlRecapDossier.Controls.Add(lblDateClotureLegende)
        pnlRecapDossier.Controls.Add(lblDateClotureValeur)
        pnlRecapDossier.Controls.Add(btnChangerStatut)
        pnlRecapDossier.BackColor = Color.Transparent
        pnlRecapDossier.Dock = DockStyle.Fill
        pnlRecapDossier.Location = New Point(282, 0)
        pnlRecapDossier.Size = New Size(317, 104)
        pnlRecapDossier.Margin = New Padding(0)
        pnlRecapDossier.Name = "pnlRecapDossier"
        pnlRecapDossier.TabIndex = 1
        ' 
        ' pnlRecapFinances
        ' 
        pnlRecapFinances.Controls.Add(lblRecapTitreFinances)
        pnlRecapFinances.Controls.Add(lblTarifPatientLegende)
        pnlRecapFinances.Controls.Add(lblTarifPatientValeur)
        pnlRecapFinances.Controls.Add(lblMontantsDusLegende)
        pnlRecapFinances.Controls.Add(lblMontantsDusValeur)
        pnlRecapFinances.Controls.Add(lblPrevuLegende)
        pnlRecapFinances.Controls.Add(lblPrevuValeur)
        pnlRecapFinances.Controls.Add(lblAvoirLegende)
        pnlRecapFinances.Controls.Add(lblAvoirValeur)
        pnlRecapFinances.BackColor = Color.Transparent
        pnlRecapFinances.Dock = DockStyle.Fill
        pnlRecapFinances.Location = New Point(599, 0)
        pnlRecapFinances.Size = New Size(317, 104)
        pnlRecapFinances.Margin = New Padding(0)
        pnlRecapFinances.Name = "pnlRecapFinances"
        pnlRecapFinances.TabIndex = 2
        ' 
        ' pnlRecapQuotas
        ' 
        pnlRecapQuotas.Controls.Add(lblRecapTitreQuotas)
        pnlRecapQuotas.Controls.Add(lblQuotaAucun)
        pnlRecapQuotas.Controls.Add(jaugeQuota)
        pnlRecapQuotas.Controls.Add(lblQuotaSoutienLegende)
        pnlRecapQuotas.Controls.Add(lblQuotaSoutienValeur)
        pnlRecapQuotas.Controls.Add(lblQuotaTraitementLegende)
        pnlRecapQuotas.Controls.Add(lblQuotaTraitementValeur)
        pnlRecapQuotas.Controls.Add(lblQuotaPeriodeInami)
        pnlRecapQuotas.BackColor = Color.Transparent
        pnlRecapQuotas.Dock = DockStyle.Fill
        pnlRecapQuotas.Location = New Point(916, 0)
        pnlRecapQuotas.Size = New Size(257, 104)
        pnlRecapQuotas.Margin = New Padding(0)
        pnlRecapQuotas.Name = "pnlRecapQuotas"
        pnlRecapQuotas.TabIndex = 3
        ' 
        ' lblRecapTitrePatient
        ' 
        lblRecapTitrePatient.AutoSize = True
        lblRecapTitrePatient.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblRecapTitrePatient.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblRecapTitrePatient.Location = New Point(10, 2)
        lblRecapTitrePatient.Name = "lblRecapTitrePatient"
        lblRecapTitrePatient.TabIndex = 0
        lblRecapTitrePatient.Text = "PATIENT"
        ' 
        ' lblRecapTitreDossier
        ' 
        lblRecapTitreDossier.AutoSize = True
        lblRecapTitreDossier.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblRecapTitreDossier.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblRecapTitreDossier.Location = New Point(10, 2)
        lblRecapTitreDossier.Name = "lblRecapTitreDossier"
        lblRecapTitreDossier.TabIndex = 0
        lblRecapTitreDossier.Text = "DOSSIER"
        ' 
        ' lblRecapTitreFinances
        ' 
        lblRecapTitreFinances.AutoSize = True
        lblRecapTitreFinances.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblRecapTitreFinances.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblRecapTitreFinances.Location = New Point(10, 2)
        lblRecapTitreFinances.Name = "lblRecapTitreFinances"
        lblRecapTitreFinances.TabIndex = 0
        lblRecapTitreFinances.Text = "FINANCES"
        ' 
        ' lblRecapTitreQuotas
        ' 
        lblRecapTitreQuotas.AutoSize = True
        lblRecapTitreQuotas.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblRecapTitreQuotas.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblRecapTitreQuotas.Location = New Point(10, 2)
        lblRecapTitreQuotas.Name = "lblRecapTitreQuotas"
        lblRecapTitreQuotas.TabIndex = 0
        lblRecapTitreQuotas.Text = "QUOTAS"
        ' 
        ' picRecapPhoto
        ' 
        picRecapPhoto.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        picRecapPhoto.Location = New Point(10, 24)
        picRecapPhoto.Name = "picRecapPhoto"
        picRecapPhoto.Size = New Size(40, 40)
        picRecapPhoto.SizeMode = PictureBoxSizeMode.Zoom
        picRecapPhoto.TabIndex = 1
        picRecapPhoto.TabStop = False
        ' 
        ' lblRecapNaissance
        ' 
        lblRecapNaissance.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblRecapNaissance.AutoEllipsis = True
        lblRecapNaissance.AutoSize = False
        lblRecapNaissance.Font = New Font("Calibri", 10.5F)
        lblRecapNaissance.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblRecapNaissance.Location = New Point(58, 26)
        lblRecapNaissance.Name = "lblRecapNaissance"
        lblRecapNaissance.Size = New Size(215, 36)
        lblRecapNaissance.TabIndex = 2
        lblRecapNaissance.Text = "—"
        ' 
        ' lblRecapAlerte
        ' 
        lblRecapAlerte.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblRecapAlerte.AutoEllipsis = True
        lblRecapAlerte.AutoSize = False
        lblRecapAlerte.BackColor = Color.FromArgb(CByte(247), CByte(237), CByte(234))
        lblRecapAlerte.Font = New Font("Calibri", 9.75F)
        lblRecapAlerte.ForeColor = Color.FromArgb(CByte(122), CByte(74), CByte(64))
        lblRecapAlerte.Location = New Point(10, 66)
        lblRecapAlerte.Name = "lblRecapAlerte"
        lblRecapAlerte.Padding = New Padding(6, 2, 6, 2)
        lblRecapAlerte.Size = New Size(263, 36)
        lblRecapAlerte.TabIndex = 3
        lblRecapAlerte.Text = "—"
        lblRecapAlerte.Visible = False
        ' 
        ' jaugeQuota
        ' 
        jaugeQuota.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        jaugeQuota.Location = New Point(10, 64)
        jaugeQuota.Name = "jaugeQuota"
        jaugeQuota.Size = New Size(239, 8)
        jaugeQuota.TabIndex = 1
        jaugeQuota.Visible = False
        ' 
        ' lblQuotaAucun
        ' 
        lblQuotaAucun.AutoSize = True
        lblQuotaAucun.Font = New Font("Calibri", 9.75F, FontStyle.Italic)
        lblQuotaAucun.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblQuotaAucun.Location = New Point(10, 24)
        lblQuotaAucun.Name = "lblQuotaAucun"
        lblQuotaAucun.TabIndex = 2
        lblQuotaAucun.Text = "Domaine sans quota"
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 12)
        enTete.Name = "enTete"
        enTete.Size = New Size(1189, 58)
        enTete.TabIndex = 10
        enTete.SousTitre = "Patient : —"
        enTete.Text = "Dossier"
        ' 
        ' lblDomaineLegende
        ' 
        lblDomaineLegende.AutoSize = False
        lblDomaineLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblDomaineLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDomaineLegende.Location = New Point(10, 22)
        lblDomaineLegende.Name = "lblDomaineLegende"
        lblDomaineLegende.Size = New Size(80, 18)
        lblDomaineLegende.TabIndex = 1
        lblDomaineLegende.Text = "Domaine"
        ' 
        ' lblDomaineValeur
        ' 
        lblDomaineValeur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblDomaineValeur.AutoEllipsis = True
        lblDomaineValeur.AutoSize = False
        lblDomaineValeur.Font = New Font("Calibri", 10.5F)
        lblDomaineValeur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDomaineValeur.Location = New Point(92, 22)
        lblDomaineValeur.Name = "lblDomaineValeur"
        lblDomaineValeur.Size = New Size(217, 18)
        lblDomaineValeur.TabIndex = 2
        lblDomaineValeur.Text = "—"
        ' 
        ' picLogoDomaine
        ' 
        picLogoDomaine.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        picLogoDomaine.BackColor = Color.Transparent
        picLogoDomaine.Location = New Point(189, 41)
        picLogoDomaine.Name = "picLogoDomaine"
        picLogoDomaine.Size = New Size(120, 52)
        picLogoDomaine.SizeMode = PictureBoxSizeMode.Zoom
        picLogoDomaine.TabIndex = 11
        picLogoDomaine.TabStop = False
        picLogoDomaine.Visible = False
        ' 
        ' lblStatutLegende
        ' 
        lblStatutLegende.AutoSize = False
        lblStatutLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblStatutLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblStatutLegende.Location = New Point(10, 41)
        lblStatutLegende.Name = "lblStatutLegende"
        lblStatutLegende.Size = New Size(80, 18)
        lblStatutLegende.TabIndex = 3
        lblStatutLegende.Text = "Statut"
        ' 
        ' lblStatutValeur
        ' 
        lblStatutValeur.AutoSize = True
        lblStatutValeur.BackColor = Color.FromArgb(CByte(222), CByte(230), CByte(225))
        lblStatutValeur.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblStatutValeur.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        lblStatutValeur.Location = New Point(92, 41)
        lblStatutValeur.Name = "lblStatutValeur"
        lblStatutValeur.Padding = New Padding(6, 0, 6, 1)
        lblStatutValeur.TabIndex = 4
        lblStatutValeur.Text = "—"
        ' 
        ' lblDateOuvertureLegende
        ' 
        lblDateOuvertureLegende.AutoSize = False
        lblDateOuvertureLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblDateOuvertureLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateOuvertureLegende.Location = New Point(10, 60)
        lblDateOuvertureLegende.Name = "lblDateOuvertureLegende"
        lblDateOuvertureLegende.Size = New Size(80, 18)
        lblDateOuvertureLegende.TabIndex = 5
        lblDateOuvertureLegende.Text = "Ouverture"
        ' 
        ' lblDateOuvertureValeur
        ' 
        lblDateOuvertureValeur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblDateOuvertureValeur.AutoEllipsis = True
        lblDateOuvertureValeur.AutoSize = False
        lblDateOuvertureValeur.Font = New Font("Calibri", 10.5F)
        lblDateOuvertureValeur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDateOuvertureValeur.Location = New Point(92, 60)
        lblDateOuvertureValeur.Name = "lblDateOuvertureValeur"
        lblDateOuvertureValeur.Size = New Size(217, 18)
        lblDateOuvertureValeur.TabIndex = 6
        lblDateOuvertureValeur.Text = "—"
        ' 
        ' lblDateClotureLegende
        ' 
        lblDateClotureLegende.AutoSize = False
        lblDateClotureLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblDateClotureLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateClotureLegende.Location = New Point(10, 79)
        lblDateClotureLegende.Name = "lblDateClotureLegende"
        lblDateClotureLegende.Size = New Size(80, 18)
        lblDateClotureLegende.TabIndex = 7
        lblDateClotureLegende.Text = "Clôture"
        ' 
        ' lblDateClotureValeur
        ' 
        lblDateClotureValeur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblDateClotureValeur.AutoEllipsis = True
        lblDateClotureValeur.AutoSize = False
        lblDateClotureValeur.Font = New Font("Calibri", 10.5F)
        lblDateClotureValeur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDateClotureValeur.Location = New Point(92, 79)
        lblDateClotureValeur.Name = "lblDateClotureValeur"
        lblDateClotureValeur.Size = New Size(217, 18)
        lblDateClotureValeur.TabIndex = 8
        lblDateClotureValeur.Text = "—"
        ' 
        ' btnChangerStatut
        ' 
        btnChangerStatut.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnChangerStatut.Image = CType(resources.GetObject("btnChangerStatut.Image"), Image)
        btnChangerStatut.Location = New Point(134, 0)
        btnChangerStatut.Name = "btnChangerStatut"
        btnChangerStatut.Size = New Size(175, 22)
        btnChangerStatut.TabIndex = 9
        btnChangerStatut.Text = "Changer le statut…"
        btnChangerStatut.Variante = VarianteBouton.Outil
        ' 
        ' lblTarifPatientLegende
        ' 
        lblTarifPatientLegende.AutoSize = False
        lblTarifPatientLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblTarifPatientLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTarifPatientLegende.Location = New Point(10, 22)
        lblTarifPatientLegende.Name = "lblTarifPatientLegende"
        lblTarifPatientLegende.Size = New Size(96, 18)
        lblTarifPatientLegende.TabIndex = 10
        lblTarifPatientLegende.Text = "Tarif patient"
        ' 
        ' lblTarifPatientValeur
        ' 
        lblTarifPatientValeur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblTarifPatientValeur.AutoEllipsis = True
        lblTarifPatientValeur.AutoSize = False
        lblTarifPatientValeur.Font = New Font("Calibri", 10.5F)
        lblTarifPatientValeur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblTarifPatientValeur.Location = New Point(110, 22)
        lblTarifPatientValeur.Name = "lblTarifPatientValeur"
        lblTarifPatientValeur.Size = New Size(199, 18)
        lblTarifPatientValeur.TabIndex = 11
        lblTarifPatientValeur.Text = "—"
        ' 
        ' lblMontantsDusLegende
        ' 
        lblMontantsDusLegende.AutoSize = False
        lblMontantsDusLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblMontantsDusLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblMontantsDusLegende.Location = New Point(10, 41)
        lblMontantsDusLegende.Name = "lblMontantsDusLegende"
        lblMontantsDusLegende.Size = New Size(96, 18)
        lblMontantsDusLegende.TabIndex = 12
        lblMontantsDusLegende.Text = "Montants dus"
        ' 
        ' lblMontantsDusValeur
        ' 
        lblMontantsDusValeur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblMontantsDusValeur.AutoEllipsis = True
        lblMontantsDusValeur.AutoSize = False
        lblMontantsDusValeur.Font = New Font("Calibri", 10.5F)
        lblMontantsDusValeur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblMontantsDusValeur.Location = New Point(110, 41)
        lblMontantsDusValeur.Name = "lblMontantsDusValeur"
        lblMontantsDusValeur.Size = New Size(199, 18)
        lblMontantsDusValeur.TabIndex = 13
        lblMontantsDusValeur.Text = "—"
        ' 
        ' lblPrevuLegende
        ' 
        lblPrevuLegende.AutoSize = False
        lblPrevuLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblPrevuLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPrevuLegende.Location = New Point(10, 60)
        lblPrevuLegende.Name = "lblPrevuLegende"
        lblPrevuLegende.Size = New Size(96, 18)
        lblPrevuLegende.TabIndex = 21
        lblPrevuLegende.Text = "Prévu"
        ' 
        ' lblPrevuValeur
        ' 
        lblPrevuValeur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblPrevuValeur.AutoEllipsis = True
        lblPrevuValeur.AutoSize = False
        lblPrevuValeur.Font = New Font("Calibri", 10.5F)
        lblPrevuValeur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblPrevuValeur.Location = New Point(110, 60)
        lblPrevuValeur.Name = "lblPrevuValeur"
        lblPrevuValeur.Size = New Size(199, 18)
        lblPrevuValeur.TabIndex = 22
        lblPrevuValeur.Text = "—"
        ' 
        ' lblAvoirLegende
        ' 
        lblAvoirLegende.AutoSize = False
        lblAvoirLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblAvoirLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAvoirLegende.Location = New Point(10, 79)
        lblAvoirLegende.Name = "lblAvoirLegende"
        lblAvoirLegende.Size = New Size(96, 18)
        lblAvoirLegende.TabIndex = 19
        lblAvoirLegende.Text = "Avoir"
        lblAvoirLegende.Visible = False
        ' 
        ' lblAvoirValeur
        ' 
        lblAvoirValeur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblAvoirValeur.AutoEllipsis = True
        lblAvoirValeur.AutoSize = False
        lblAvoirValeur.Font = New Font("Calibri", 10.5F)
        lblAvoirValeur.ForeColor = Color.FromArgb(CByte(0), CByte(112), CByte(60))
        lblAvoirValeur.Location = New Point(110, 79)
        lblAvoirValeur.Name = "lblAvoirValeur"
        lblAvoirValeur.Size = New Size(199, 18)
        lblAvoirValeur.TabIndex = 20
        lblAvoirValeur.Text = "—"
        lblAvoirValeur.Visible = False
        ' 
        ' lblQuotaSoutienLegende
        ' 
        lblQuotaSoutienLegende.AutoSize = False
        lblQuotaSoutienLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblQuotaSoutienLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblQuotaSoutienLegende.Location = New Point(10, 22)
        lblQuotaSoutienLegende.Name = "lblQuotaSoutienLegende"
        lblQuotaSoutienLegende.Size = New Size(48, 18)
        lblQuotaSoutienLegende.TabIndex = 14
        lblQuotaSoutienLegende.Text = "Soutien :"
        lblQuotaSoutienLegende.Visible = False
        ' 
        ' lblQuotaSoutienValeur
        ' 
        lblQuotaSoutienValeur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblQuotaSoutienValeur.AutoEllipsis = True
        lblQuotaSoutienValeur.AutoSize = False
        lblQuotaSoutienValeur.Font = New Font("Calibri", 10.5F)
        lblQuotaSoutienValeur.Location = New Point(60, 22)
        lblQuotaSoutienValeur.Name = "lblQuotaSoutienValeur"
        lblQuotaSoutienValeur.Size = New Size(189, 18)
        lblQuotaSoutienValeur.TabIndex = 15
        lblQuotaSoutienValeur.Text = "—"
        lblQuotaSoutienValeur.Visible = False
        ' 
        ' lblQuotaTraitementLegende
        ' 
        lblQuotaTraitementLegende.AutoSize = False
        lblQuotaTraitementLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblQuotaTraitementLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblQuotaTraitementLegende.Location = New Point(10, 41)
        lblQuotaTraitementLegende.Name = "lblQuotaTraitementLegende"
        lblQuotaTraitementLegende.Size = New Size(48, 18)
        lblQuotaTraitementLegende.TabIndex = 16
        lblQuotaTraitementLegende.Text = "Traitement :"
        lblQuotaTraitementLegende.Visible = False
        ' 
        ' lblQuotaTraitementValeur
        ' 
        lblQuotaTraitementValeur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblQuotaTraitementValeur.AutoEllipsis = True
        lblQuotaTraitementValeur.AutoSize = False
        lblQuotaTraitementValeur.Font = New Font("Calibri", 10.5F)
        lblQuotaTraitementValeur.Location = New Point(60, 41)
        lblQuotaTraitementValeur.Name = "lblQuotaTraitementValeur"
        lblQuotaTraitementValeur.Size = New Size(189, 18)
        lblQuotaTraitementValeur.TabIndex = 17
        lblQuotaTraitementValeur.Text = "—"
        lblQuotaTraitementValeur.Visible = False
        ' 
        ' lblQuotaPeriodeInami
        ' 
        lblQuotaPeriodeInami.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblQuotaPeriodeInami.AutoEllipsis = True
        lblQuotaPeriodeInami.AutoSize = False
        lblQuotaPeriodeInami.Font = New Font("Calibri", 9F, FontStyle.Italic)
        lblQuotaPeriodeInami.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblQuotaPeriodeInami.Location = New Point(10, 79)
        lblQuotaPeriodeInami.Name = "lblQuotaPeriodeInami"
        lblQuotaPeriodeInami.Size = New Size(239, 17)
        lblQuotaPeriodeInami.TabIndex = 18
        lblQuotaPeriodeInami.Text = "—"
        lblQuotaPeriodeInami.Visible = False
        ' 
        ' tabDossier
        ' 
        tabDossier.Controls.Add(tabPageFicheDossier)
        tabDossier.Controls.Add(tabPageSuiviClinique)
        tabDossier.Controls.Add(tabPageDocuments)
        tabDossier.Controls.Add(tabPageSeances)
        tabDossier.Controls.Add(tabPagePaiements)
        tabDossier.CouleurFond = Color.Transparent
        tabDossier.Dock = DockStyle.Fill
        tabDossier.Font = New Font("Calibri", 11F)
        tabDossier.Location = New Point(0, 158)
        tabDossier.Name = "tabDossier"
        tabDossier.Padding = New Point(12, 6)
        tabDossier.SelectedIndex = 0
        tabDossier.Size = New Size(1221, 390)
        tabDossier.TabIndex = 1
        ' 
        ' tabPageFicheDossier
        ' 
        tabPageFicheDossier.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageFicheDossier.Controls.Add(carteConvention)
        tabPageFicheDossier.Controls.Add(carteParametres)
        tabPageFicheDossier.Location = New Point(4, 36)
        tabPageFicheDossier.Name = "tabPageFicheDossier"
        tabPageFicheDossier.Padding = New Padding(8)
        tabPageFicheDossier.Size = New Size(1213, 350)
        tabPageFicheDossier.TabIndex = 0
        tabPageFicheDossier.Text = "Fiche Dossier"
        ' 
        ' carteParametres
        ' 
        carteParametres.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        carteParametres.Controls.Add(lblDomaineEditLegende)
        carteParametres.Controls.Add(cboDomaine)
        carteParametres.Controls.Add(lblDateOuvertureEditLegende)
        carteParametres.Controls.Add(dtpDateOuverture)
        carteParametres.Controls.Add(lblModalitesLegende)
        carteParametres.Controls.Add(txtModalitesPaiement)
        carteParametres.Location = New Point(8, 8)
        carteParametres.Name = "carteParametres"
        carteParametres.Padding = New Padding(16, 8, 16, 10)
        carteParametres.Size = New Size(1197, 100)
        carteParametres.TabIndex = 0
        carteParametres.Titre = "Paramètres du dossier"
        ' 
        ' carteConvention
        ' 
        carteConvention.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        carteConvention.Controls.Add(ucSyntheseDomaine)
        carteConvention.Location = New Point(8, 120)
        carteConvention.Name = "carteConvention"
        carteConvention.Padding = New Padding(16, 8, 16, 12)
        carteConvention.Size = New Size(1197, 222)
        carteConvention.TabIndex = 1
        carteConvention.Titre = "Convention du domaine"
        ' 
        ' ucSyntheseDomaine
        ' 
        ucSyntheseDomaine.Dock = DockStyle.Fill
        ucSyntheseDomaine.Location = New Point(16, 37)
        ucSyntheseDomaine.Name = "ucSyntheseDomaine"
        ucSyntheseDomaine.Size = New Size(1165, 173)
        ucSyntheseDomaine.TabIndex = 0
        ucSyntheseDomaine.TabStop = False
        ' 
        ' lblDomaineEditLegende
        ' 
        lblDomaineEditLegende.AutoSize = True
        lblDomaineEditLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblDomaineEditLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDomaineEditLegende.Location = New Point(16, 40)
        lblDomaineEditLegende.Name = "lblDomaineEditLegende"
        lblDomaineEditLegende.Size = New Size(56, 16)
        lblDomaineEditLegende.TabIndex = 0
        lblDomaineEditLegende.Text = "Domaine"
        ' 
        ' cboDomaine
        ' 
        cboDomaine.DessinItemsPersonnalise = True
        cboDomaine.Font = New Font("Calibri", 11F)
        cboDomaine.ItemHeight = 20
        cboDomaine.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboDomaine.Location = New Point(16, 60)
        cboDomaine.Name = "cboDomaine"
        cboDomaine.Size = New Size(340, 26)
        cboDomaine.TabIndex = 1
        ' 
        ' lblDateOuvertureEditLegende
        ' 
        lblDateOuvertureEditLegende.AutoSize = True
        lblDateOuvertureEditLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblDateOuvertureEditLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateOuvertureEditLegende.Location = New Point(380, 40)
        lblDateOuvertureEditLegende.Name = "lblDateOuvertureEditLegende"
        lblDateOuvertureEditLegende.Size = New Size(98, 16)
        lblDateOuvertureEditLegende.TabIndex = 2
        lblDateOuvertureEditLegende.Text = "Date d'ouverture"
        ' 
        ' dtpDateOuverture
        ' 
        dtpDateOuverture.Font = New Font("Calibri", 11F)
        dtpDateOuverture.Location = New Point(380, 60)
        dtpDateOuverture.Name = "dtpDateOuverture"
        dtpDateOuverture.Size = New Size(160, 26)
        dtpDateOuverture.TabIndex = 3
        ' 
        ' lblModalitesLegende
        ' 
        lblModalitesLegende.AutoSize = True
        lblModalitesLegende.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblModalitesLegende.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblModalitesLegende.Location = New Point(564, 40)
        lblModalitesLegende.Name = "lblModalitesLegende"
        lblModalitesLegende.Size = New Size(135, 16)
        lblModalitesLegende.TabIndex = 4
        lblModalitesLegende.Text = "Modalités de paiement"
        ' 
        ' txtModalitesPaiement
        ' 
        txtModalitesPaiement.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtModalitesPaiement.Font = New Font("Calibri", 11F)
        txtModalitesPaiement.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtModalitesPaiement.Location = New Point(564, 60)
        txtModalitesPaiement.Name = "txtModalitesPaiement"
        txtModalitesPaiement.Size = New Size(420, 25)
        txtModalitesPaiement.TabIndex = 5
        ' 
        ' tabPageSuiviClinique
        ' 
        tabPageSuiviClinique.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageSuiviClinique.Controls.Add(carteZones)
        tabPageSuiviClinique.Location = New Point(4, 36)
        tabPageSuiviClinique.Name = "tabPageSuiviClinique"
        tabPageSuiviClinique.Padding = New Padding(8)
        tabPageSuiviClinique.Size = New Size(1213, 350)
        tabPageSuiviClinique.TabIndex = 0
        tabPageSuiviClinique.Text = "Suivi clinique"
        ' 
        ' carteZones
        ' 
        carteZones.Controls.Add(tlpZones)
        carteZones.Dock = DockStyle.Fill
        carteZones.Location = New Point(8, 8)
        carteZones.Name = "carteZones"
        carteZones.Padding = New Padding(12, 8, 12, 10)
        carteZones.Size = New Size(1197, 334)
        carteZones.TabIndex = 0
        carteZones.Titre = "Zones cliniques"
        ' 
        ' tlpZones
        ' 
        tlpZones.AutoScroll = True
        tlpZones.BackColor = Color.Transparent
        tlpZones.ColumnCount = 1
        tlpZones.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        tlpZones.Dock = DockStyle.Fill
        tlpZones.Location = New Point(12, 38)
        tlpZones.Name = "tlpZones"
        tlpZones.Size = New Size(1173, 286)
        tlpZones.TabIndex = 0
        ' 
        ' tabPageDocuments
        ' 
        tabPageDocuments.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageDocuments.Controls.Add(carteDocuments)
        tabPageDocuments.Location = New Point(4, 36)
        tabPageDocuments.Name = "tabPageDocuments"
        tabPageDocuments.Padding = New Padding(8)
        tabPageDocuments.Size = New Size(1213, 350)
        tabPageDocuments.TabIndex = 1
        tabPageDocuments.Text = "Documents"
        ' 
        ' carteDocuments
        ' 
        carteDocuments.Controls.Add(lblDocumentsAVenir)
        carteDocuments.Dock = DockStyle.Fill
        carteDocuments.Location = New Point(8, 8)
        carteDocuments.Name = "carteDocuments"
        carteDocuments.Padding = New Padding(16, 8, 16, 12)
        carteDocuments.Size = New Size(1197, 334)
        carteDocuments.TabIndex = 0
        carteDocuments.Titre = "Documents du dossier"
        ' 
        ' lblDocumentsAVenir
        ' 
        lblDocumentsAVenir.BackColor = Color.Transparent
        lblDocumentsAVenir.Dock = DockStyle.Fill
        lblDocumentsAVenir.Font = New Font("Calibri", 11F, FontStyle.Italic)
        lblDocumentsAVenir.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblDocumentsAVenir.Location = New Point(16, 38)
        lblDocumentsAVenir.Name = "lblDocumentsAVenir"
        lblDocumentsAVenir.Size = New Size(1165, 284)
        lblDocumentsAVenir.TabIndex = 0
        lblDocumentsAVenir.Text = "Les documents du dossier seront disponibles dans une prochaine version."
        lblDocumentsAVenir.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' tabPageSeances
        ' 
        tabPageSeances.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageSeances.Controls.Add(carteSeances)
        tabPageSeances.Controls.Add(pnlSeancesFiltres)
        tabPageSeances.Location = New Point(4, 36)
        tabPageSeances.Name = "tabPageSeances"
        tabPageSeances.Padding = New Padding(8)
        tabPageSeances.Size = New Size(1213, 350)
        tabPageSeances.TabIndex = 2
        tabPageSeances.Text = "Séances"
        ' 
        ' dgvSeances
        ' 
        dgvSeances.AllowUserToAddRows = False
        dgvSeances.AllowUserToDeleteRows = False
        dgvSeances.BackgroundColor = Color.White
        dgvSeances.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvSeances.Columns.AddRange(New DataGridViewColumn() {colSeanceCode, colSeanceDate, colSeanceHoraires, colSeanceType, colSeancePeriodeInami, colSeanceQuotaInami, colSeanceStatut, colSeanceEtatPaiement, colSeanceTarif, colSeanceRegle})
        dgvSeances.Dock = DockStyle.Fill
        dgvSeances.Location = New Point(8, 96)
        dgvSeances.Name = "dgvSeances"
        dgvSeances.ReadOnly = True
        dgvSeances.RowHeadersVisible = False
        dgvSeances.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvSeances.Size = New Size(1197, 246)
        dgvSeances.TabIndex = 1
        ' 
        ' colSeanceCode
        ' 
        colSeanceCode.DataPropertyName = "CodeSeance"
        colSeanceCode.HeaderText = "Code"
        colSeanceCode.Name = "colSeanceCode"
        colSeanceCode.ReadOnly = True
        colSeanceCode.Width = 85
        ' 
        ' colSeanceDate
        ' 
        colSeanceDate.DataPropertyName = "DateSeance"
        colSeanceDate.HeaderText = "Date"
        colSeanceDate.Name = "colSeanceDate"
        colSeanceDate.ReadOnly = True
        colSeanceDate.Width = 120
        ' 
        ' colSeanceHoraires
        ' 
        colSeanceHoraires.DataPropertyName = "HorairesAffichage"
        colSeanceHoraires.HeaderText = "Horaires"
        colSeanceHoraires.Name = "colSeanceHoraires"
        colSeanceHoraires.ReadOnly = True
        colSeanceHoraires.Width = 105
        ' 
        ' colSeanceType
        ' 
        colSeanceType.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colSeanceType.DataPropertyName = "LibelleNatureSeance"
        colSeanceType.HeaderText = "Nature"
        colSeanceType.Name = "colSeanceType"
        colSeanceType.ReadOnly = True
        ' 
        ' colSeancePeriodeInami
        ' 
        colSeancePeriodeInami.DataPropertyName = "LibellePeriodeInami"
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter
        colSeancePeriodeInami.DefaultCellStyle = DataGridViewCellStyle1
        colSeancePeriodeInami.HeaderText = "Periode"
        colSeancePeriodeInami.Name = "colSeancePeriodeInami"
        colSeancePeriodeInami.ReadOnly = True
        colSeancePeriodeInami.ToolTipText = "Periode annuelle de quota INAMI a laquelle la seance se rattache"
        colSeancePeriodeInami.Width = 60
        ' 
        ' colSeanceQuotaInami
        ' 
        colSeanceQuotaInami.DataPropertyName = "LibelleQuotaInami"
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter
        colSeanceQuotaInami.DefaultCellStyle = DataGridViewCellStyle2
        colSeanceQuotaInami.HeaderText = "Quota"
        colSeanceQuotaInami.Name = "colSeanceQuotaInami"
        colSeanceQuotaInami.ReadOnly = True
        colSeanceQuotaInami.ToolTipText = "Rang de la seance dans le quota INAMI annuel (S = soutien, T = traitement)"
        colSeanceQuotaInami.Width = 60
        ' 
        ' colSeanceStatut
        ' 
        colSeanceStatut.DataPropertyName = "LibelleStatutAffichage"
        colSeanceStatut.HeaderText = "Statut"
        colSeanceStatut.Name = "colSeanceStatut"
        colSeanceStatut.ReadOnly = True
        colSeanceStatut.Width = 230
        ' 
        ' colSeanceEtatPaiement
        ' 
        colSeanceEtatPaiement.DataPropertyName = "PaiementAffichage"
        colSeanceEtatPaiement.HeaderText = "Paiement"
        colSeanceEtatPaiement.Name = "colSeanceEtatPaiement"
        colSeanceEtatPaiement.ReadOnly = True
        colSeanceEtatPaiement.Width = 165
        ' 
        ' colSeanceTarif
        ' 
        colSeanceTarif.DataPropertyName = "TarifSeance"
        colSeanceTarif.HeaderText = "Tarif"
        colSeanceTarif.Name = "colSeanceTarif"
        colSeanceTarif.ReadOnly = True
        colSeanceTarif.Width = 70
        ' 
        ' colSeanceRegle
        ' 
        colSeanceRegle.DataPropertyName = "MontantRegle"
        colSeanceRegle.HeaderText = "Réglé"
        colSeanceRegle.Name = "colSeanceRegle"
        colSeanceRegle.ReadOnly = True
        colSeanceRegle.Width = 70
        ' 
        ' pnlSeancesFiltres
        ' 
        pnlSeancesFiltres.Controls.Add(carteFiltresSeances)
        pnlSeancesFiltres.BackColor = Color.Transparent
        pnlSeancesFiltres.Dock = DockStyle.Top
        pnlSeancesFiltres.Location = New Point(8, 56)
        pnlSeancesFiltres.Name = "pnlSeancesFiltres"
        pnlSeancesFiltres.Padding = New Padding(0, 0, 0, 8)
        pnlSeancesFiltres.Size = New Size(1197, 66)
        pnlSeancesFiltres.TabIndex = 1
        ' 
        ' carteFiltresSeances
        ' 
        carteFiltresSeances.Controls.Add(txtRechercheSeance)
        carteFiltresSeances.Controls.Add(cboFiltreSeance)
        carteFiltresSeances.Controls.Add(cboFiltrePeriodeInami)
        carteFiltresSeances.Controls.Add(chkPeriodeSeance)
        carteFiltresSeances.Controls.Add(dtpSeanceDu)
        carteFiltresSeances.Controls.Add(lblSeanceAu)
        carteFiltresSeances.Controls.Add(dtpSeanceAu)
        carteFiltresSeances.Dock = DockStyle.Fill
        carteFiltresSeances.Font = New Font("Calibri", 11F)
        carteFiltresSeances.Location = New Point(0, 0)
        carteFiltresSeances.Name = "carteFiltresSeances"
        carteFiltresSeances.Padding = New Padding(14, 10, 14, 10)
        carteFiltresSeances.Size = New Size(1197, 58)
        carteFiltresSeances.TabIndex = 0
        ' 
        ' txtRechercheSeance
        ' 
        txtRechercheSeance.Location = New Point(14, 13)
        txtRechercheSeance.Name = "txtRechercheSeance"
        txtRechercheSeance.Size = New Size(280, 32)
        txtRechercheSeance.TabIndex = 0
        txtRechercheSeance.TexteAide = "Code, type, statut, date…"
        ' 
        ' cboFiltreSeance
        ' 
        cboFiltreSeance.Etiquette = "Filtre"
        cboFiltreSeance.Location = New Point(306, 14)
        cboFiltreSeance.Name = "cboFiltreSeance"
        cboFiltreSeance.Size = New Size(270, 30)
        cboFiltreSeance.TabIndex = 1
        ' 
        ' cboFiltrePeriodeInami
        ' 
        cboFiltrePeriodeInami.Etiquette = "Période"
        cboFiltrePeriodeInami.Location = New Point(930, 14)
        cboFiltrePeriodeInami.Name = "cboFiltrePeriodeInami"
        cboFiltrePeriodeInami.Size = New Size(240, 30)
        cboFiltrePeriodeInami.TabIndex = 6
        cboFiltrePeriodeInami.Visible = False
        ' 
        ' chkPeriodeSeance
        ' 
        chkPeriodeSeance.AutoSize = False
        chkPeriodeSeance.Location = New Point(588, 17)
        chkPeriodeSeance.Name = "chkPeriodeSeance"
        chkPeriodeSeance.Size = New Size(62, 24)
        chkPeriodeSeance.TabIndex = 2
        chkPeriodeSeance.Text = "Du"
        ' 
        ' dtpSeanceDu
        ' 
        dtpSeanceDu.Enabled = False
        dtpSeanceDu.Font = New Font("Calibri", 11F)
        dtpSeanceDu.Location = New Point(652, 14)
        dtpSeanceDu.Name = "dtpSeanceDu"
        dtpSeanceDu.Size = New Size(118, 30)
        dtpSeanceDu.TabIndex = 3
        ' 
        ' lblSeanceAu
        ' 
        lblSeanceAu.AutoSize = True
        lblSeanceAu.BackColor = Color.Transparent
        lblSeanceAu.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblSeanceAu.Location = New Point(775, 20)
        lblSeanceAu.Name = "lblSeanceAu"
        lblSeanceAu.Size = New Size(22, 18)
        lblSeanceAu.TabIndex = 4
        lblSeanceAu.Text = "au"
        ' 
        ' dtpSeanceAu
        ' 
        dtpSeanceAu.Enabled = False
        dtpSeanceAu.Font = New Font("Calibri", 11F)
        dtpSeanceAu.Location = New Point(800, 14)
        dtpSeanceAu.Name = "dtpSeanceAu"
        dtpSeanceAu.Size = New Size(118, 30)
        dtpSeanceAu.TabIndex = 5
        ' 
        ' pagSeances
        ' 
        pagSeances.BackColor = Color.Transparent
        pagSeances.Dock = DockStyle.Bottom
        pagSeances.Font = New Font("Calibri", 9F)
        pagSeances.Name = "pagSeances"
        pagSeances.Size = New Size(900, 32)
        pagSeances.TabIndex = 3
        ' 
        ' carteSeances
        ' 
        carteSeances.Controls.Add(dgvSeances)
        carteSeances.Controls.Add(pagSeances)
        carteSeances.Controls.Add(pnlResumeSeances)
        carteSeances.Controls.Add(pnlSeancesActions)
        carteSeances.Dock = DockStyle.Fill
        carteSeances.Name = "carteSeances"
        carteSeances.Padding = New Padding(6, 2, 6, 6)
        carteSeances.TabIndex = 0
        ' 
        ' pnlResumeSeances
        ' 
        pnlResumeSeances.BackColor = Color.Transparent
        pnlResumeSeances.Dock = DockStyle.Top
        pnlResumeSeances.Name = "pnlResumeSeances"
        pnlResumeSeances.Padding = New Padding(6, 2, 6, 2)
        pnlResumeSeances.Size = New Size(600, 34)
        pnlResumeSeances.TabIndex = 2
        pnlResumeSeances.WrapContents = False
        ' 
        ' pnlSeancesActions
        ' 
        pnlSeancesActions.Controls.Add(btnNouvelleSeance)
        pnlSeancesActions.Controls.Add(btnModifierSeance)
        pnlSeancesActions.Controls.Add(btnReporterSeance)
        pnlSeancesActions.Controls.Add(btnSupprimerSeance)
        pnlSeancesActions.Controls.Add(btnEncaisserSeance)
        pnlSeancesActions.Controls.Add(btnDetailPaiementsSeance)
        pnlSeancesActions.Dock = DockStyle.Top
        pnlSeancesActions.Name = "pnlSeancesActions"
        pnlSeancesActions.Size = New Size(600, 42)
        pnlSeancesActions.TabIndex = 0
        ' 
        ' btnNouvelleSeance
        ' 
        btnNouvelleSeance.Image = CType(resources.GetObject("btnNouvelleSeance.Image"), Image)
        btnNouvelleSeance.Location = New Point(10, 5)
        btnNouvelleSeance.Name = "btnNouvelleSeance"
        btnNouvelleSeance.Size = New Size(160, 30)
        btnNouvelleSeance.TabIndex = 0
        btnNouvelleSeance.Tag = "seance"
        btnNouvelleSeance.Text = "Nouvelle séance"
        btnNouvelleSeance.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' btnModifierSeance
        ' 
        btnModifierSeance.Image = CType(resources.GetObject("btnModifierSeance.Image"), Image)
        btnModifierSeance.Location = New Point(174, 5)
        btnModifierSeance.Name = "btnModifierSeance"
        btnModifierSeance.Size = New Size(159, 30)
        btnModifierSeance.TabIndex = 1
        btnModifierSeance.Tag = "Modifier"
        btnModifierSeance.Text = "Modifier séance"
        btnModifierSeance.Variante = VarianteBouton.Outil
        ' 
        ' btnReporterSeance
        ' 
        btnReporterSeance.Image = CType(resources.GetObject("btnReporterSeance.Image"), Image)
        btnReporterSeance.Location = New Point(337, 5)
        btnReporterSeance.Name = "btnReporterSeance"
        btnReporterSeance.Size = New Size(160, 30)
        btnReporterSeance.TabIndex = 2
        btnReporterSeance.Tag = "Reporter"
        btnReporterSeance.Text = "Reporter séance"
        btnReporterSeance.Variante = VarianteBouton.Outil
        ' 
        ' btnSupprimerSeance
        ' 
        btnSupprimerSeance.Image = CType(resources.GetObject("btnSupprimerSeance.Image"), Image)
        btnSupprimerSeance.Location = New Point(849, 5)
        btnSupprimerSeance.Name = "btnSupprimerSeance"
        btnSupprimerSeance.Size = New Size(170, 30)
        btnSupprimerSeance.TabIndex = 2
        btnSupprimerSeance.Tag = "Supprimer"
        btnSupprimerSeance.Text = "Supprimer séance"
        btnSupprimerSeance.Variante = VarianteBouton.OutilDanger
        ' 
        ' btnEncaisserSeance
        ' 
        btnEncaisserSeance.Image = CType(resources.GetObject("btnEncaisserSeance.Image"), Image)
        btnEncaisserSeance.Location = New Point(501, 5)
        btnEncaisserSeance.Name = "btnEncaisserSeance"
        btnEncaisserSeance.Size = New Size(162, 30)
        btnEncaisserSeance.TabIndex = 3
        btnEncaisserSeance.Tag = "EncaisserSeance"
        btnEncaisserSeance.Text = "Encaisser séance"
        btnEncaisserSeance.Variante = VarianteBouton.Outil
        ' 
        ' btnDetailPaiementsSeance
        ' 
        btnDetailPaiementsSeance.Image = CType(resources.GetObject("btnDetailPaiementsSeance.Image"), Image)
        btnDetailPaiementsSeance.Location = New Point(667, 5)
        btnDetailPaiementsSeance.Name = "btnDetailPaiementsSeance"
        btnDetailPaiementsSeance.Size = New Size(164, 30)
        btnDetailPaiementsSeance.TabIndex = 5
        btnDetailPaiementsSeance.Tag = "DetailPaiementsSeance"
        btnDetailPaiementsSeance.Text = "Détail paiements"
        btnDetailPaiementsSeance.Variante = VarianteBouton.Outil
        ' 
        ' tabPagePaiements
        ' 
        tabPagePaiements.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPagePaiements.Controls.Add(cartePaiements)
        tabPagePaiements.Controls.Add(pnlPaiementsFiltres)
        tabPagePaiements.Location = New Point(4, 36)
        tabPagePaiements.Name = "tabPagePaiements"
        tabPagePaiements.Padding = New Padding(8)
        tabPagePaiements.Size = New Size(1213, 350)
        tabPagePaiements.TabIndex = 3
        tabPagePaiements.Text = "Paiements"
        ' 
        ' dgvPaiements
        ' 
        dgvPaiements.AllowUserToAddRows = False
        dgvPaiements.AllowUserToDeleteRows = False
        dgvPaiements.BackgroundColor = Color.White
        dgvPaiements.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvPaiements.Columns.AddRange(New DataGridViewColumn() {colPaiementDate, colPaiementCode, colPaiementMouvement, colPaiementSeance, colPaiementMode, colPaiementMontant, colPaiementEtat})
        dgvPaiements.Dock = DockStyle.Fill
        dgvPaiements.Location = New Point(8, 96)
        dgvPaiements.Name = "dgvPaiements"
        dgvPaiements.ReadOnly = True
        dgvPaiements.RowHeadersVisible = False
        dgvPaiements.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPaiements.Size = New Size(1197, 246)
        dgvPaiements.TabIndex = 1
        ' 
        ' colPaiementDate
        ' 
        colPaiementDate.DataPropertyName = "DateMouvement"
        colPaiementDate.HeaderText = "Date"
        colPaiementDate.Name = "colPaiementDate"
        colPaiementDate.ReadOnly = True
        colPaiementDate.Width = 105
        ' 
        ' colPaiementCode
        ' 
        colPaiementCode.DataPropertyName = "CodePaiement"
        colPaiementCode.HeaderText = "Code"
        colPaiementCode.Name = "colPaiementCode"
        colPaiementCode.ReadOnly = True
        colPaiementCode.Width = 95
        ' 
        ' colPaiementMouvement
        ' 
        colPaiementMouvement.DataPropertyName = "MouvementCourt"
        colPaiementMouvement.HeaderText = "Mouvement"
        colPaiementMouvement.Name = "colPaiementMouvement"
        colPaiementMouvement.ReadOnly = True
        colPaiementMouvement.Width = 160
        ' 
        ' colPaiementSeance
        ' 
        colPaiementSeance.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colPaiementSeance.DataPropertyName = "PourAffichage"
        colPaiementSeance.HeaderText = "Pour"
        colPaiementSeance.Name = "colPaiementSeance"
        colPaiementSeance.ReadOnly = True
        ' 
        ' colPaiementMode
        ' 
        colPaiementMode.DataPropertyName = "ModePaiement"
        colPaiementMode.HeaderText = "Mode"
        colPaiementMode.Name = "colPaiementMode"
        colPaiementMode.ReadOnly = True
        colPaiementMode.Width = 120
        ' 
        ' colPaiementMontant
        ' 
        colPaiementMontant.DataPropertyName = "MontantLigne"
        colPaiementMontant.HeaderText = "Montant"
        colPaiementMontant.Name = "colPaiementMontant"
        colPaiementMontant.ReadOnly = True
        colPaiementMontant.Width = 90
        ' 
        ' colPaiementEtat
        ' 
        colPaiementEtat.DataPropertyName = "EtatAffichage"
        colPaiementEtat.HeaderText = "État"
        colPaiementEtat.Name = "colPaiementEtat"
        colPaiementEtat.ReadOnly = True
        colPaiementEtat.Width = 260
        ' 
        ' pnlPaiementsFiltres
        ' 
        pnlPaiementsFiltres.Controls.Add(carteFiltresPaiements)
        pnlPaiementsFiltres.BackColor = Color.Transparent
        pnlPaiementsFiltres.Dock = DockStyle.Top
        pnlPaiementsFiltres.Location = New Point(8, 56)
        pnlPaiementsFiltres.Name = "pnlPaiementsFiltres"
        pnlPaiementsFiltres.Padding = New Padding(0, 0, 0, 8)
        pnlPaiementsFiltres.Size = New Size(1197, 66)
        pnlPaiementsFiltres.TabIndex = 1
        ' 
        ' carteFiltresPaiements
        ' 
        carteFiltresPaiements.Controls.Add(txtRecherchePaiement)
        carteFiltresPaiements.Controls.Add(cboFiltrePaiement)
        carteFiltresPaiements.Controls.Add(chkPeriodePaiement)
        carteFiltresPaiements.Controls.Add(dtpPaiementDu)
        carteFiltresPaiements.Controls.Add(lblPaiementAu)
        carteFiltresPaiements.Controls.Add(dtpPaiementAu)
        carteFiltresPaiements.Dock = DockStyle.Fill
        carteFiltresPaiements.Font = New Font("Calibri", 11F)
        carteFiltresPaiements.Location = New Point(0, 0)
        carteFiltresPaiements.Name = "carteFiltresPaiements"
        carteFiltresPaiements.Padding = New Padding(14, 10, 14, 10)
        carteFiltresPaiements.Size = New Size(1197, 58)
        carteFiltresPaiements.TabIndex = 0
        ' 
        ' txtRecherchePaiement
        ' 
        txtRecherchePaiement.Location = New Point(14, 13)
        txtRecherchePaiement.Name = "txtRecherchePaiement"
        txtRecherchePaiement.Size = New Size(280, 32)
        txtRecherchePaiement.TabIndex = 0
        txtRecherchePaiement.TexteAide = "Code, séance, mode, référence…"
        ' 
        ' cboFiltrePaiement
        ' 
        cboFiltrePaiement.Etiquette = "Filtre"
        cboFiltrePaiement.Location = New Point(306, 14)
        cboFiltrePaiement.Name = "cboFiltrePaiement"
        cboFiltrePaiement.Size = New Size(270, 30)
        cboFiltrePaiement.TabIndex = 1
        ' 
        ' chkPeriodePaiement
        ' 
        chkPeriodePaiement.AutoSize = False
        chkPeriodePaiement.Location = New Point(588, 17)
        chkPeriodePaiement.Name = "chkPeriodePaiement"
        chkPeriodePaiement.Size = New Size(62, 24)
        chkPeriodePaiement.TabIndex = 2
        chkPeriodePaiement.Text = "Du"
        ' 
        ' dtpPaiementDu
        ' 
        dtpPaiementDu.Enabled = False
        dtpPaiementDu.Font = New Font("Calibri", 11F)
        dtpPaiementDu.Location = New Point(652, 14)
        dtpPaiementDu.Name = "dtpPaiementDu"
        dtpPaiementDu.Size = New Size(118, 30)
        dtpPaiementDu.TabIndex = 3
        ' 
        ' lblPaiementAu
        ' 
        lblPaiementAu.AutoSize = True
        lblPaiementAu.BackColor = Color.Transparent
        lblPaiementAu.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblPaiementAu.Location = New Point(775, 20)
        lblPaiementAu.Name = "lblPaiementAu"
        lblPaiementAu.Size = New Size(22, 18)
        lblPaiementAu.TabIndex = 4
        lblPaiementAu.Text = "au"
        ' 
        ' dtpPaiementAu
        ' 
        dtpPaiementAu.Enabled = False
        dtpPaiementAu.Font = New Font("Calibri", 11F)
        dtpPaiementAu.Location = New Point(800, 14)
        dtpPaiementAu.Name = "dtpPaiementAu"
        dtpPaiementAu.Size = New Size(118, 30)
        dtpPaiementAu.TabIndex = 5
        ' 
        ' pagPaiements
        ' 
        pagPaiements.BackColor = Color.Transparent
        pagPaiements.Dock = DockStyle.Bottom
        pagPaiements.Font = New Font("Calibri", 9F)
        pagPaiements.Name = "pagPaiements"
        pagPaiements.Size = New Size(900, 32)
        pagPaiements.TabIndex = 3
        ' 
        ' cartePaiements
        ' 
        cartePaiements.Controls.Add(dgvPaiements)
        cartePaiements.Controls.Add(pagPaiements)
        cartePaiements.Controls.Add(pnlResumePaiements)
        cartePaiements.Controls.Add(pnlPaiementsActions)
        cartePaiements.Dock = DockStyle.Fill
        cartePaiements.Name = "cartePaiements"
        cartePaiements.Padding = New Padding(6, 2, 6, 6)
        cartePaiements.TabIndex = 0
        ' 
        ' pnlResumePaiements
        ' 
        pnlResumePaiements.BackColor = Color.Transparent
        pnlResumePaiements.Dock = DockStyle.Top
        pnlResumePaiements.Name = "pnlResumePaiements"
        pnlResumePaiements.Padding = New Padding(6, 2, 6, 2)
        pnlResumePaiements.Size = New Size(600, 34)
        pnlResumePaiements.TabIndex = 2
        pnlResumePaiements.WrapContents = False
        ' 
        ' pnlPaiementsActions
        ' 
        pnlPaiementsActions.Controls.Add(btnSupprimerPaiement)
        pnlPaiementsActions.Dock = DockStyle.Top
        pnlPaiementsActions.Name = "pnlPaiementsActions"
        pnlPaiementsActions.Size = New Size(600, 42)
        pnlPaiementsActions.TabIndex = 0
        ' 
        ' btnSupprimerPaiement
        ' 
        btnSupprimerPaiement.Image = CType(resources.GetObject("btnSupprimerPaiement.Image"), Image)
        btnSupprimerPaiement.Location = New Point(10, 5)
        btnSupprimerPaiement.Name = "btnSupprimerPaiement"
        btnSupprimerPaiement.Size = New Size(215, 30)
        btnSupprimerPaiement.TabIndex = 0
        btnSupprimerPaiement.Tag = "Supprimer"
        btnSupprimerPaiement.Text = "Annuler l'encaissement"
        btnSupprimerPaiement.Variante = VarianteBouton.OutilDanger
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 548)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(1221, 52)
        pnlActions.TabIndex = 2
        ' 
        ' btnFermer
        ' 
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.Location = New Point(16, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(161, 38)
        btnFermer.TabIndex = 0
        btnFermer.Text = "Retour patients"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' btnModifier
        ' 
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(765, 7)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(119, 38)
        btnModifier.TabIndex = 1
        btnModifier.Text = "Modifier"
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(623, 7)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 38)
        btnEnregistrer.TabIndex = 2
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(765, 7)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(119, 38)
        btnAnnuler.TabIndex = 3
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' UC_DossierFiche
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(tabDossier)
        Controls.Add(pnlActions)
        Controls.Add(pnlBandeau)
        DoubleBuffered = True
        Name = "UC_DossierFiche"
        Size = New Size(1221, 600)
        pnlBandeau.ResumeLayout(False)
        pnlBandeau.PerformLayout()
        tabDossier.ResumeLayout(False)
        carteParametres.ResumeLayout(False)
        carteParametres.PerformLayout()
        carteConvention.ResumeLayout(False)
        carteZones.ResumeLayout(False)
        carteDocuments.ResumeLayout(False)
        tabPageFicheDossier.ResumeLayout(False)
        tabPageFicheDossier.PerformLayout()
        tabPageSuiviClinique.ResumeLayout(False)
        tabPageDocuments.ResumeLayout(False)
        tabPageDocuments.PerformLayout()
        tabPageSeances.ResumeLayout(False)
        CType(dgvSeances, ComponentModel.ISupportInitialize).EndInit()
        carteFiltresSeances.ResumeLayout(False)
        carteFiltresSeances.PerformLayout()
        pnlSeancesFiltres.ResumeLayout(False)
        pnlSeancesFiltres.PerformLayout()
        pnlSeancesActions.ResumeLayout(False)
        tabPagePaiements.ResumeLayout(False)
        CType(dgvPaiements, ComponentModel.ISupportInitialize).EndInit()
        carteFiltresPaiements.ResumeLayout(False)
        carteFiltresPaiements.PerformLayout()
        pnlPaiementsFiltres.ResumeLayout(False)
        pnlPaiementsFiltres.PerformLayout()
        pnlPaiementsActions.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        CType(picLogoDomaine, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub

    Friend WithEvents pnlBandeau As Panel
    Friend WithEvents pnlDossierConsultation As Panel
    Friend WithEvents lblDossierConsultation As Label
    Friend WithEvents carteRecap As AltheaCarte
    Friend WithEvents carteParametres As AltheaCarte
    Friend WithEvents carteZones As AltheaCarte
    Friend WithEvents carteDocuments As AltheaCarte
    Friend WithEvents carteConvention As AltheaCarte
    Friend WithEvents tblRecap As TableLayoutPanel
    Friend WithEvents pnlRecapPatient As Panel
    Friend WithEvents pnlRecapDossier As Panel
    Friend WithEvents pnlRecapFinances As Panel
    Friend WithEvents pnlRecapQuotas As Panel
    Friend WithEvents lblRecapTitrePatient As Label
    Friend WithEvents lblRecapTitreDossier As Label
    Friend WithEvents lblRecapTitreFinances As Label
    Friend WithEvents lblRecapTitreQuotas As Label
    Friend WithEvents picRecapPhoto As PictureBox
    Friend WithEvents lblRecapNaissance As Label
    Friend WithEvents lblRecapAlerte As Label
    Friend WithEvents jaugeQuota As AltheaJauge
    Friend WithEvents lblQuotaAucun As Label
    Friend WithEvents lblDomaineLegende As Label
    Friend WithEvents lblDomaineValeur As Label
    Friend WithEvents picLogoDomaine As PictureBox
    Friend WithEvents lblStatutLegende As Label
    Friend WithEvents lblStatutValeur As Label
    Friend WithEvents lblDateOuvertureLegende As Label
    Friend WithEvents lblDateOuvertureValeur As Label
    Friend WithEvents lblDateClotureLegende As Label
    Friend WithEvents lblDateClotureValeur As Label
    Friend WithEvents btnChangerStatut As AltheaBouton
    Friend WithEvents lblTarifPatientLegende As Label
    Friend WithEvents lblTarifPatientValeur As Label
    Friend WithEvents lblMontantsDusLegende As Label
    Friend WithEvents lblMontantsDusValeur As Label
    Friend WithEvents lblPrevuLegende As Label
    Friend WithEvents lblPrevuValeur As Label
    Friend WithEvents lblAvoirLegende As Label
    Friend WithEvents lblAvoirValeur As Label
    Friend WithEvents tabDossier As AltheaOnglets
    Friend WithEvents tabPageFicheDossier As TabPage
    Friend WithEvents lblDomaineEditLegende As Label
    Friend WithEvents cboDomaine As AltheaListe
    Friend WithEvents ucSyntheseDomaine As UC_SyntheseDomaine
    Friend WithEvents lblDateOuvertureEditLegende As Label
    Friend WithEvents dtpDateOuverture As AltheaDate
    Friend WithEvents lblModalitesLegende As Label
    Friend WithEvents txtModalitesPaiement As TextBox
    Friend WithEvents tabPageSuiviClinique As TabPage
    Friend WithEvents tlpZones As TableLayoutPanel
    Friend WithEvents tabPageDocuments As TabPage
    Friend WithEvents lblDocumentsAVenir As Label
    Friend WithEvents tabPageSeances As TabPage
    Friend WithEvents carteSeances As AltheaCarte
    Friend WithEvents pnlSeancesActions As AltheaBarreOutils
    Friend WithEvents btnNouvelleSeance As AltheaBouton
    Friend WithEvents btnModifierSeance As AltheaBouton
    Friend WithEvents btnReporterSeance As AltheaBouton
    Friend WithEvents btnSupprimerSeance As AltheaBouton
    Friend WithEvents btnEncaisserSeance As AltheaBouton
    Friend WithEvents btnDetailPaiementsSeance As AltheaBouton
    Friend WithEvents dgvSeances As DataGridView
    Friend WithEvents pagSeances As UC_Pagination
    Friend WithEvents colSeanceCode As DataGridViewTextBoxColumn
    Friend WithEvents colSeanceDate As DataGridViewTextBoxColumn
    Friend WithEvents colSeanceHoraires As DataGridViewTextBoxColumn
    Friend WithEvents colSeanceType As DataGridViewTextBoxColumn
    Friend WithEvents lblQuotaSoutienLegende As Label
    Friend WithEvents lblQuotaSoutienValeur As Label
    Friend WithEvents lblQuotaTraitementLegende As Label
    Friend WithEvents lblQuotaTraitementValeur As Label
    Friend WithEvents lblQuotaPeriodeInami As Label
    Friend WithEvents colSeanceQuotaInami As DataGridViewTextBoxColumn
    Friend WithEvents colSeancePeriodeInami As DataGridViewTextBoxColumn
    Friend WithEvents colSeanceStatut As DataGridViewTextBoxColumn
    Friend WithEvents colSeanceTarif As DataGridViewTextBoxColumn
    Friend WithEvents pnlResumeSeances As FlowLayoutPanel
    Friend WithEvents colSeanceRegle As DataGridViewTextBoxColumn
    Friend WithEvents pnlSeancesFiltres As Panel
    Friend WithEvents carteFiltresSeances As AltheaCarte
    Friend WithEvents txtRechercheSeance As AltheaRecherche
    Friend WithEvents cboFiltreSeance As AltheaListe
    Friend WithEvents cboFiltrePeriodeInami As AltheaListe
    Friend WithEvents colSeanceEtatPaiement As DataGridViewTextBoxColumn
    Friend WithEvents tabPagePaiements As TabPage
    Friend WithEvents cartePaiements As AltheaCarte
    Friend WithEvents pnlPaiementsActions As AltheaBarreOutils
    Friend WithEvents btnSupprimerPaiement As AltheaBouton
    Friend WithEvents dgvPaiements As DataGridView
    Friend WithEvents pagPaiements As UC_Pagination
    Friend WithEvents colPaiementDate As DataGridViewTextBoxColumn
    Friend WithEvents colPaiementCode As DataGridViewTextBoxColumn
    Friend WithEvents colPaiementMouvement As DataGridViewTextBoxColumn
    Friend WithEvents colPaiementSeance As DataGridViewTextBoxColumn
    Friend WithEvents colPaiementMode As DataGridViewTextBoxColumn
    Friend WithEvents colPaiementMontant As DataGridViewTextBoxColumn
    Friend WithEvents colPaiementEtat As DataGridViewTextBoxColumn
    Friend WithEvents pnlResumePaiements As FlowLayoutPanel
    Friend WithEvents pnlPaiementsFiltres As Panel
    Friend WithEvents carteFiltresPaiements As AltheaCarte
    Friend WithEvents txtRecherchePaiement As AltheaRecherche
    Friend WithEvents cboFiltrePaiement As AltheaListe
    Friend WithEvents chkPeriodeSeance As AltheaCase
    Friend WithEvents dtpSeanceDu As AltheaDate
    Friend WithEvents lblSeanceAu As Label
    Friend WithEvents dtpSeanceAu As AltheaDate
    Friend WithEvents chkPeriodePaiement As AltheaCase
    Friend WithEvents dtpPaiementDu As AltheaDate
    Friend WithEvents lblPaiementAu As Label
    Friend WithEvents dtpPaiementAu As AltheaDate
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnFermer As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete

End Class
