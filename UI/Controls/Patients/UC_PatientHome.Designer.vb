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
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_PatientHome))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        dgvPatients = New DataGridView()
        pagPatients = New UC_Pagination()
        pnlBarreResume = New Panel()
        pnlResumePatients = New FlowLayoutPanel()
        pnlFiltrePatients = New Panel()
        cboFiltrePatients = New AltheaListe()
        btnApercu = New AltheaBouton()
        ucApercu = New UC_ApercuPatient()
        colStatutPaiement = New DataGridViewTextBoxColumn()
        colNbNoShow = New DataGridViewTextBoxColumn()
        colCodePatient = New DataGridViewTextBoxColumn()
        colNom = New DataGridViewTextBoxColumn()
        colPrenom = New DataGridViewTextBoxColumn()
        colTelephone = New DataGridViewTextBoxColumn()
        colAlerte = New DataGridViewImageColumn()
        colAge = New DataGridViewTextBoxColumn()
        colSuivi = New DataGridViewTextBoxColumn()
        colProchainRdv = New DataGridViewTextBoxColumn()
        colDerniereSeance = New DataGridViewTextBoxColumn()
        colIdPatient = New DataGridViewTextBoxColumn()
        pnlTop = New AltheaCarte()
        btnRechercher = New AltheaBouton()
        btnReinitialiserFiltres = New AltheaBouton()
        cboFiltreSuivi = New AltheaListe()
        cboFiltreDomaine = New AltheaListe()
        txtRecherchePatient = New AltheaRecherche()
        pnlActions = New AltheaBarreActions()
        btnActualiser = New AltheaBouton()
        btnOuvrir = New AltheaBouton()
        btnNouveau = New AltheaBouton()
        btnModifier = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        CType(dgvPatients, ComponentModel.ISupportInitialize).BeginInit()
        pnlTop.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(enTete)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16)
        pnlForm.Size = New Size(1061, 771)
        pnlForm.TabIndex = 1
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(dgvPatients)
        pnlCenter.Controls.Add(pagPatients)
        pnlCenter.Controls.Add(ucApercu)
        pnlCenter.Controls.Add(pnlBarreResume)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(1029, 624)
        pnlCenter.TabIndex = 21
        ' 
        ' dgvPatients
        ' 
        dgvPatients.AllowUserToOrderColumns = True
        dgvPatients.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvPatients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvPatients.Columns.AddRange(New DataGridViewColumn() {colCodePatient, colNom, colPrenom, colAge, colSuivi, colProchainRdv, colDerniereSeance, colStatutPaiement, colNbNoShow, colAlerte, colTelephone, colIdPatient})
        DataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = SystemColors.Window
        DataGridViewCellStyle4.Font = New Font("Calibri", 10F)
        DataGridViewCellStyle4.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        DataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = DataGridViewTriState.False
        dgvPatients.DefaultCellStyle = DataGridViewCellStyle4
        dgvPatients.Dock = DockStyle.Fill
        dgvPatients.Location = New Point(16, 86)
        dgvPatients.Name = "dgvPatients"
        dgvPatients.ReadOnly = True
        dgvPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPatients.Size = New Size(997, 530)
        dgvPatients.TabIndex = 8
        ' 
        ' pagPatients
        ' 
        pagPatients.BackColor = Color.Transparent
        pagPatients.Dock = DockStyle.Bottom
        pagPatients.Location = New Point(16, 584)
        pagPatients.Name = "pagPatients"
        pagPatients.Size = New Size(997, 32)
        pagPatients.TabIndex = 9
        ' 
        ' pnlBarreResume
        ' 
        pnlBarreResume.BackColor = Color.Transparent
        pnlBarreResume.Controls.Add(pnlResumePatients)
        pnlBarreResume.Controls.Add(pnlFiltrePatients)
        pnlBarreResume.Controls.Add(btnApercu)
        pnlBarreResume.Dock = DockStyle.Top
        pnlBarreResume.Location = New Point(16, 74)
        pnlBarreResume.Name = "pnlBarreResume"
        pnlBarreResume.Padding = New Padding(0, 4, 0, 2)
        pnlBarreResume.Size = New Size(997, 40)
        pnlBarreResume.TabIndex = 7
        ' 
        ' pnlResumePatients
        ' 
        pnlResumePatients.BackColor = Color.Transparent
        pnlResumePatients.Dock = DockStyle.Fill
        pnlResumePatients.Location = New Point(0, 4)
        pnlResumePatients.Name = "pnlResumePatients"
        pnlResumePatients.Padding = New Padding(2, 0, 2, 0)
        pnlResumePatients.Size = New Size(817, 34)
        pnlResumePatients.TabIndex = 0
        pnlResumePatients.WrapContents = False
        ' 
        ' pnlFiltrePatients
        ' 
        pnlFiltrePatients.BackColor = Color.Transparent
        pnlFiltrePatients.Controls.Add(cboFiltrePatients)
        pnlFiltrePatients.Dock = DockStyle.Right
        pnlFiltrePatients.Location = New Point(817, 4)
        pnlFiltrePatients.Name = "pnlFiltrePatients"
        pnlFiltrePatients.Padding = New Padding(0, 2, 0, 0)
        pnlFiltrePatients.Size = New Size(180, 34)
        pnlFiltrePatients.TabIndex = 1
        ' 
        ' cboFiltrePatients
        ' 
        cboFiltrePatients.Dock = DockStyle.Top
        cboFiltrePatients.Etiquette = "Patients"
        cboFiltrePatients.Location = New Point(0, 2)
        cboFiltrePatients.Name = "cboFiltrePatients"
        cboFiltrePatients.Size = New Size(180, 30)
        cboFiltrePatients.TabIndex = 0
        ' 
        ' btnApercu
        ' 
        btnApercu.Dock = DockStyle.Right
        btnApercu.Location = New Point(857, 4)
        btnApercu.Margin = New Padding(8, 0, 0, 0)
        btnApercu.Name = "btnApercu"
        btnApercu.Size = New Size(140, 34)
        btnApercu.TabIndex = 2
        btnApercu.Text = "Masquer l'aperçu"
        btnApercu.Variante = VarianteBouton.Outil
        ' 
        ' ucApercu
        ' 
        ucApercu.BackColor = Color.Transparent
        ucApercu.Dock = DockStyle.Right
        ucApercu.Location = New Point(613, 114)
        ucApercu.Name = "ucApercu"
        ucApercu.Padding = New Padding(12, 0, 0, 0)
        ucApercu.Size = New Size(400, 502)
        ucApercu.TabIndex = 10
        ' 
        ' colStatutPaiement
        ' 
        colStatutPaiement.HeaderText = "Paiements"
        colStatutPaiement.Name = "colStatutPaiement"
        colStatutPaiement.ReadOnly = True
        colStatutPaiement.Width = 118
        ' 
        ' colNbNoShow
        ' 
        colNbNoShow.HeaderText = "Abs."
        colNbNoShow.Name = "colNbNoShow"
        colNbNoShow.ReadOnly = True
        colNbNoShow.Resizable = DataGridViewTriState.False
        colNbNoShow.Width = 48
        ' 
        ' colCodePatient
        ' 
        colCodePatient.HeaderText = "Code"
        colCodePatient.Name = "colCodePatient"
        colCodePatient.ReadOnly = True
        colCodePatient.Width = 72
        ' 
        ' colNom
        ' 
        colNom.HeaderText = "Nom"
        colNom.Name = "colNom"
        colNom.ReadOnly = True
        colNom.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colNom.MinimumWidth = 120
        colNom.Width = 140
        ' 
        ' colPrenom
        ' 
        colPrenom.HeaderText = "Prénom"
        colPrenom.Name = "colPrenom"
        colPrenom.ReadOnly = True
        colPrenom.Width = 120
        ' 
        ' colTelephone
        ' 
        colTelephone.HeaderText = "Téléphone"
        colTelephone.Name = "colTelephone"
        colTelephone.ReadOnly = True
        colTelephone.Width = 118
        ' 
        ' colAlerte
        ' 
        colAlerte.HeaderText = "Alerte"
        colAlerte.Name = "colAlerte"
        colAlerte.ReadOnly = True
        colAlerte.Resizable = DataGridViewTriState.False
        colAlerte.Width = 50
        ' 
        ' colAge
        ' 
        colAge.HeaderText = "Âge"
        colAge.Name = "colAge"
        colAge.ReadOnly = True
        colAge.Width = 58
        ' 
        ' colSuivi
        ' 
        colSuivi.HeaderText = "Suivi"
        colSuivi.Name = "colSuivi"
        colSuivi.ReadOnly = True
        colSuivi.Width = 150
        ' 
        ' colProchainRdv
        ' 
        colProchainRdv.HeaderText = "Prochain RDV"
        colProchainRdv.Name = "colProchainRdv"
        colProchainRdv.ReadOnly = True
        colProchainRdv.Width = 168
        ' 
        ' colDerniereSeance
        ' 
        colDerniereSeance.HeaderText = "Dernière séance"
        colDerniereSeance.Name = "colDerniereSeance"
        colDerniereSeance.ReadOnly = True
        colDerniereSeance.Width = 150
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
        pnlTop.Controls.Add(btnRechercher)
        pnlTop.Controls.Add(btnReinitialiserFiltres)
        pnlTop.Controls.Add(cboFiltreSuivi)
        pnlTop.Controls.Add(cboFiltreDomaine)
        pnlTop.Controls.Add(txtRecherchePatient)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(14, 10, 14, 10)
        pnlTop.Size = New Size(997, 58)
        pnlTop.TabIndex = 7
        ' 
        ' btnRechercher
        ' 
        btnRechercher.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnRechercher.Image = CType(resources.GetObject("btnRechercher.Image"), Image)
        btnRechercher.Location = New Point(848, 13)
        btnRechercher.Name = "btnRechercher"
        btnRechercher.Size = New Size(135, 32)
        btnRechercher.TabIndex = 3
        btnRechercher.Tag = ""
        btnRechercher.Text = "Rechercher"
        ' 
        ' btnReinitialiserFiltres
        ' 
        btnReinitialiserFiltres.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnReinitialiserFiltres.Image = CType(resources.GetObject("btnReinitialiserFiltres.Image"), Image)
        btnReinitialiserFiltres.Location = New Point(708, 13)
        btnReinitialiserFiltres.Name = "btnReinitialiserFiltres"
        btnReinitialiserFiltres.Size = New Size(130, 32)
        btnReinitialiserFiltres.TabIndex = 4
        btnReinitialiserFiltres.Tag = "reinitialiser_24_normal"
        btnReinitialiserFiltres.Text = "Réinitialiser"
        btnReinitialiserFiltres.Variante = VarianteBouton.Clair
        ' 
        ' cboFiltreSuivi
        ' 
        cboFiltreSuivi.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        cboFiltreSuivi.DessinItemsPersonnalise = True
        cboFiltreSuivi.Etiquette = "Dossier"
        cboFiltreSuivi.Location = New Point(296, 14)
        cboFiltreSuivi.Name = "cboFiltreSuivi"
        cboFiltreSuivi.Size = New Size(200, 30)
        cboFiltreSuivi.TabIndex = 5
        ' 
        ' cboFiltreDomaine
        ' 
        cboFiltreDomaine.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        cboFiltreDomaine.Etiquette = "Domaine"
        cboFiltreDomaine.Location = New Point(506, 14)
        cboFiltreDomaine.Name = "cboFiltreDomaine"
        cboFiltreDomaine.Size = New Size(190, 30)
        cboFiltreDomaine.TabIndex = 6
        ' 
        ' txtRecherchePatient
        ' 
        txtRecherchePatient.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtRecherchePatient.Location = New Point(14, 13)
        txtRecherchePatient.Name = "txtRecherchePatient"
        txtRecherchePatient.Size = New Size(272, 32)
        txtRecherchePatient.TabIndex = 2
        txtRecherchePatient.TexteAide = "Nom, code, téléphone, NISS, domaine…"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.Controls.Add(btnOuvrir)
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(1029, 60)
        pnlActions.TabIndex = 20
        ' 
        ' btnActualiser
        ' 
        btnActualiser.Image = CType(resources.GetObject("btnActualiser.Image"), Image)
        btnActualiser.Location = New Point(628, 10)
        btnActualiser.Name = "btnActualiser"
        btnActualiser.Size = New Size(127, 40)
        btnActualiser.TabIndex = 11
        btnActualiser.Tag = "actualiser_normal"
        btnActualiser.Text = "Actualiser"
        btnActualiser.Variante = VarianteBouton.Clair
        ' 
        ' btnOuvrir
        ' 
        btnOuvrir.Image = CType(resources.GetObject("btnOuvrir.Image"), Image)
        btnOuvrir.Location = New Point(506, 10)
        btnOuvrir.Name = "btnOuvrir"
        btnOuvrir.Size = New Size(112, 40)
        btnOuvrir.TabIndex = 10
        btnOuvrir.Tag = "open_normal"
        btnOuvrir.Text = "Ouvrir"
        ' 
        ' btnNouveau
        ' 
        btnNouveau.Image = CType(resources.GetObject("btnNouveau.Image"), Image)
        btnNouveau.Location = New Point(247, 10)
        btnNouveau.Name = "btnNouveau"
        btnNouveau.Size = New Size(120, 40)
        btnNouveau.TabIndex = 8
        btnNouveau.Tag = "nouveau_normal"
        btnNouveau.Text = "Nouveau"
        ' 
        ' btnModifier
        ' 
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(377, 10)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(119, 40)
        btnModifier.TabIndex = 9
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(1029, 58)
        enTete.TabIndex = 5
        enTete.SousTitre = "Recherchez un patient, consultez la liste et ouvrez une fiche."
        enTete.Text = "Patients"
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
        Size = New Size(1061, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        CType(dgvPatients, ComponentModel.ISupportInitialize).EndInit()
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnNouveau As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents btnOuvrir As AltheaBouton
    Friend WithEvents btnActualiser As AltheaBouton
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents pnlTop As AltheaCarte
    Friend WithEvents dgvPatients As DataGridView
    Friend WithEvents pagPatients As UC_Pagination
    Friend WithEvents pnlBarreResume As Panel
    Friend WithEvents pnlResumePatients As FlowLayoutPanel
    Friend WithEvents pnlFiltrePatients As Panel
    Friend WithEvents cboFiltrePatients As AltheaListe
    Friend WithEvents btnApercu As AltheaBouton
    Friend WithEvents ucApercu As UC_ApercuPatient
    Friend WithEvents txtRecherchePatient As AltheaRecherche
    Friend WithEvents cboFiltreSuivi As AltheaListe
    Friend WithEvents cboFiltreDomaine As AltheaListe
    Friend WithEvents btnRechercher As AltheaBouton
    Friend WithEvents btnReinitialiserFiltres As AltheaBouton
    Friend WithEvents colCodePatient As DataGridViewTextBoxColumn
    Friend WithEvents colNom As DataGridViewTextBoxColumn
    Friend WithEvents colPrenom As DataGridViewTextBoxColumn
    Friend WithEvents colTelephone As DataGridViewTextBoxColumn
    Friend WithEvents colAlerte As DataGridViewImageColumn
    Friend WithEvents colStatutPaiement As DataGridViewTextBoxColumn
    Friend WithEvents colNbNoShow As DataGridViewTextBoxColumn
    Friend WithEvents colAge As DataGridViewTextBoxColumn
    Friend WithEvents colSuivi As DataGridViewTextBoxColumn
    Friend WithEvents colProchainRdv As DataGridViewTextBoxColumn
    Friend WithEvents colDerniereSeance As DataGridViewTextBoxColumn
    Friend WithEvents colIdPatient As DataGridViewTextBoxColumn

End Class
