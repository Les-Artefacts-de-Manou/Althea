<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_Journaux
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

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        pnlForm = New Panel()
        pnlCenter = New Panel()
        carteJournal = New AltheaCarte()
        dgvJournal = New DataGridView()
        colHeure = New DataGridViewTextBoxColumn()
        colNiveau = New DataGridViewTextBoxColumn()
        colCategorie = New DataGridViewTextBoxColumn()
        colMessage = New DataGridViewTextBoxColumn()
        pnlEspace2 = New Panel()
        carteDetail = New AltheaCarte()
        txtDetail = New TextBox()
        pnlEspace1 = New Panel()
        carteFiltres = New AltheaCarte()
        tblFiltres = New TableLayoutPanel()
        lblTitreJour = New Label()
        cboJour = New ComboBox()
        lblTitreNiveau = New Label()
        cboNiveau = New ComboBox()
        lblTitreCategorie = New Label()
        cboCategorie = New ComboBox()
        lblTitreRecherche = New Label()
        txtRecherche = New TextBox()
        chkErreursSeules = New CheckBox()
        pnlActions = New AltheaBarreActions()
        lblCompteur = New Label()
        btnOuvrirDossier = New AltheaBouton()
        btnActualiser = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        carteJournal.SuspendLayout()
        CType(dgvJournal, ComponentModel.ISupportInitialize).BeginInit()
        carteDetail.SuspendLayout()
        carteFiltres.SuspendLayout()
        tblFiltres.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(enTete)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 0
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(carteJournal)
        pnlCenter.Controls.Add(pnlEspace2)
        pnlCenter.Controls.Add(carteDetail)
        pnlCenter.Controls.Add(pnlEspace1)
        pnlCenter.Controls.Add(carteFiltres)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(965, 621)
        pnlCenter.TabIndex = 1
        ' 
        ' carteJournal
        ' 
        carteJournal.Controls.Add(dgvJournal)
        carteJournal.Dock = DockStyle.Fill
        carteJournal.Location = New Point(16, 120)
        carteJournal.Name = "carteJournal"
        carteJournal.Padding = New Padding(14, 10, 14, 12)
        carteJournal.Size = New Size(933, 293)
        carteJournal.TabIndex = 2
        carteJournal.Titre = "Entrées (les plus récentes en premier)"
        ' 
        ' dgvJournal
        ' 
        dgvJournal.AllowUserToAddRows = False
        dgvJournal.AllowUserToDeleteRows = False
        dgvJournal.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvJournal.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvJournal.Columns.AddRange(New DataGridViewColumn() {colHeure, colNiveau, colCategorie, colMessage})
        dgvJournal.Dock = DockStyle.Fill
        dgvJournal.Location = New Point(14, 39)
        dgvJournal.MultiSelect = False
        dgvJournal.Name = "dgvJournal"
        dgvJournal.ReadOnly = True
        dgvJournal.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvJournal.Size = New Size(905, 242)
        dgvJournal.TabIndex = 0
        ' 
        ' colHeure
        ' 
        colHeure.HeaderText = "Heure"
        colHeure.Name = "colHeure"
        colHeure.ReadOnly = True
        colHeure.Width = 80
        ' 
        ' colNiveau
        ' 
        colNiveau.HeaderText = "Niveau"
        colNiveau.Name = "colNiveau"
        colNiveau.ReadOnly = True
        colNiveau.Width = 150
        ' 
        ' colCategorie
        ' 
        colCategorie.HeaderText = "Catégorie"
        colCategorie.Name = "colCategorie"
        colCategorie.ReadOnly = True
        colCategorie.Width = 90
        ' 
        ' colMessage
        ' 
        colMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colMessage.HeaderText = "Message"
        colMessage.Name = "colMessage"
        colMessage.ReadOnly = True
        ' 
        ' pnlEspace2
        ' 
        pnlEspace2.BackColor = Color.Transparent
        pnlEspace2.Dock = DockStyle.Bottom
        pnlEspace2.Location = New Point(16, 413)
        pnlEspace2.Name = "pnlEspace2"
        pnlEspace2.Size = New Size(933, 12)
        pnlEspace2.TabIndex = 3
        ' 
        ' carteDetail
        ' 
        carteDetail.Controls.Add(txtDetail)
        carteDetail.Dock = DockStyle.Bottom
        carteDetail.Location = New Point(16, 425)
        carteDetail.Name = "carteDetail"
        carteDetail.Padding = New Padding(14, 10, 14, 12)
        carteDetail.Size = New Size(933, 188)
        carteDetail.TabIndex = 4
        carteDetail.Titre = "Détail de l'entrée"
        ' 
        ' txtDetail
        ' 
        txtDetail.BackColor = Color.FromArgb(CByte(250), CByte(247), CByte(243))
        txtDetail.BorderStyle = BorderStyle.None
        txtDetail.Dock = DockStyle.Fill
        txtDetail.Font = New Font("Consolas", 9.5F)
        txtDetail.Location = New Point(14, 39)
        txtDetail.Multiline = True
        txtDetail.Name = "txtDetail"
        txtDetail.ReadOnly = True
        txtDetail.ScrollBars = ScrollBars.Vertical
        txtDetail.Size = New Size(905, 137)
        txtDetail.TabIndex = 0
        ' 
        ' pnlEspace1
        ' 
        pnlEspace1.BackColor = Color.Transparent
        pnlEspace1.Dock = DockStyle.Top
        pnlEspace1.Location = New Point(16, 108)
        pnlEspace1.Name = "pnlEspace1"
        pnlEspace1.Size = New Size(933, 12)
        pnlEspace1.TabIndex = 1
        ' 
        ' carteFiltres
        ' 
        carteFiltres.Controls.Add(tblFiltres)
        carteFiltres.Dock = DockStyle.Top
        carteFiltres.Location = New Point(16, 16)
        carteFiltres.Name = "carteFiltres"
        carteFiltres.Padding = New Padding(14, 10, 14, 10)
        carteFiltres.Size = New Size(933, 92)
        carteFiltres.TabIndex = 0
        carteFiltres.Titre = "Journal du jour"
        ' 
        ' tblFiltres
        ' 
        tblFiltres.BackColor = Color.Transparent
        tblFiltres.ColumnCount = 9
        tblFiltres.ColumnStyles.Add(New ColumnStyle())
        tblFiltres.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 220F))
        tblFiltres.ColumnStyles.Add(New ColumnStyle())
        tblFiltres.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 110F))
        tblFiltres.ColumnStyles.Add(New ColumnStyle())
        tblFiltres.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 110F))
        tblFiltres.ColumnStyles.Add(New ColumnStyle())
        tblFiltres.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        tblFiltres.ColumnStyles.Add(New ColumnStyle())
        tblFiltres.Controls.Add(lblTitreJour, 0, 0)
        tblFiltres.Controls.Add(cboJour, 1, 0)
        tblFiltres.Controls.Add(lblTitreNiveau, 2, 0)
        tblFiltres.Controls.Add(cboNiveau, 3, 0)
        tblFiltres.Controls.Add(lblTitreCategorie, 4, 0)
        tblFiltres.Controls.Add(cboCategorie, 5, 0)
        tblFiltres.Controls.Add(lblTitreRecherche, 6, 0)
        tblFiltres.Controls.Add(txtRecherche, 7, 0)
        tblFiltres.Controls.Add(chkErreursSeules, 8, 0)
        tblFiltres.Dock = DockStyle.Fill
        tblFiltres.Location = New Point(14, 39)
        tblFiltres.Name = "tblFiltres"
        tblFiltres.RowCount = 1
        tblFiltres.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        tblFiltres.Size = New Size(905, 43)
        tblFiltres.TabIndex = 0
        ' 
        ' lblTitreJour
        ' 
        lblTitreJour.Anchor = AnchorStyles.Left
        lblTitreJour.AutoSize = True
        lblTitreJour.BackColor = Color.Transparent
        lblTitreJour.Location = New Point(3, 13)
        lblTitreJour.Name = "lblTitreJour"
        lblTitreJour.Size = New Size(38, 17)
        lblTitreJour.TabIndex = 0
        lblTitreJour.Text = "Jour :"
        ' 
        ' cboJour
        ' 
        cboJour.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        cboJour.DropDownStyle = ComboBoxStyle.DropDownList
        cboJour.Location = New Point(47, 10)
        cboJour.Name = "cboJour"
        cboJour.Size = New Size(214, 23)
        cboJour.TabIndex = 1
        ' 
        ' lblTitreNiveau
        ' 
        lblTitreNiveau.Anchor = AnchorStyles.Left
        lblTitreNiveau.AutoSize = True
        lblTitreNiveau.BackColor = Color.Transparent
        lblTitreNiveau.Location = New Point(276, 13)
        lblTitreNiveau.Margin = New Padding(12, 0, 3, 0)
        lblTitreNiveau.Name = "lblTitreNiveau"
        lblTitreNiveau.Size = New Size(54, 17)
        lblTitreNiveau.TabIndex = 2
        lblTitreNiveau.Text = "Niveau :"
        ' 
        ' cboNiveau
        ' 
        cboNiveau.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        cboNiveau.DropDownStyle = ComboBoxStyle.DropDownList
        cboNiveau.Location = New Point(336, 10)
        cboNiveau.Name = "cboNiveau"
        cboNiveau.Size = New Size(104, 23)
        cboNiveau.TabIndex = 3
        ' 
        ' lblTitreCategorie
        ' 
        lblTitreCategorie.Anchor = AnchorStyles.Left
        lblTitreCategorie.AutoSize = True
        lblTitreCategorie.BackColor = Color.Transparent
        lblTitreCategorie.Location = New Point(455, 13)
        lblTitreCategorie.Margin = New Padding(12, 0, 3, 0)
        lblTitreCategorie.Name = "lblTitreCategorie"
        lblTitreCategorie.Size = New Size(70, 17)
        lblTitreCategorie.TabIndex = 4
        lblTitreCategorie.Text = "Catégorie :"
        ' 
        ' cboCategorie
        ' 
        cboCategorie.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        cboCategorie.DropDownStyle = ComboBoxStyle.DropDownList
        cboCategorie.Location = New Point(531, 10)
        cboCategorie.Name = "cboCategorie"
        cboCategorie.Size = New Size(104, 23)
        cboCategorie.TabIndex = 5
        ' 
        ' lblTitreRecherche
        ' 
        lblTitreRecherche.Anchor = AnchorStyles.Left
        lblTitreRecherche.AutoSize = True
        lblTitreRecherche.BackColor = Color.Transparent
        lblTitreRecherche.Location = New Point(650, 13)
        lblTitreRecherche.Margin = New Padding(12, 0, 3, 0)
        lblTitreRecherche.Name = "lblTitreRecherche"
        lblTitreRecherche.Size = New Size(80, 17)
        lblTitreRecherche.TabIndex = 6
        lblTitreRecherche.Text = "Rechercher :"
        ' 
        ' txtRecherche
        ' 
        txtRecherche.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        txtRecherche.Location = New Point(736, 9)
        txtRecherche.Name = "txtRecherche"
        txtRecherche.Size = New Size(21, 24)
        txtRecherche.TabIndex = 7
        ' 
        ' chkErreursSeules
        ' 
        chkErreursSeules.Anchor = AnchorStyles.Left
        chkErreursSeules.AutoSize = True
        chkErreursSeules.BackColor = Color.Transparent
        chkErreursSeules.Location = New Point(772, 11)
        chkErreursSeules.Margin = New Padding(12, 3, 3, 3)
        chkErreursSeules.Name = "chkErreursSeules"
        chkErreursSeules.Size = New Size(130, 21)
        chkErreursSeules.TabIndex = 8
        chkErreursSeules.Text = "Erreurs seulement"
        chkErreursSeules.UseVisualStyleBackColor = False
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(lblCompteur)
        pnlActions.Controls.Add(btnOuvrirDossier)
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.CouleurFond = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 695)
        pnlActions.Name = "pnlActions"
        pnlActions.OpaciteFond = 190
        pnlActions.Padding = New Padding(16, 8, 16, 12)
        pnlActions.Rayon = 12
        pnlActions.Size = New Size(965, 60)
        pnlActions.TabIndex = 2
        ' 
        ' lblCompteur
        ' 
        lblCompteur.AutoSize = True
        lblCompteur.BackColor = Color.Transparent
        lblCompteur.Font = New Font("Calibri", 9F, FontStyle.Italic)
        lblCompteur.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblCompteur.Location = New Point(16, 22)
        lblCompteur.Name = "lblCompteur"
        lblCompteur.Size = New Size(0, 14)
        lblCompteur.TabIndex = 0
        ' 
        ' btnOuvrirDossier
        ' 
        btnOuvrirDossier.FlatStyle = FlatStyle.Flat
        btnOuvrirDossier.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnOuvrirDossier.ForeColor = Color.White
        btnOuvrirDossier.Location = New Point(560, 10)
        btnOuvrirDossier.Name = "btnOuvrirDossier"
        btnOuvrirDossier.Size = New Size(260, 40)
        btnOuvrirDossier.TabIndex = 1
        btnOuvrirDossier.Tag = "ouvrir_dossier_normal"
        btnOuvrirDossier.Text = "Ouvrir le dossier des journaux"
        btnOuvrirDossier.UseVisualStyleBackColor = False
        btnOuvrirDossier.Variante = VarianteBouton.Clair
        ' 
        ' btnActualiser
        ' 
        btnActualiser.FlatStyle = FlatStyle.Flat
        btnActualiser.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnActualiser.ForeColor = Color.White
        btnActualiser.Location = New Point(830, 10)
        btnActualiser.Name = "btnActualiser"
        btnActualiser.Size = New Size(120, 40)
        btnActualiser.TabIndex = 2
        btnActualiser.Tag = "actualiser_normal"
        btnActualiser.Text = "Actualiser"
        btnActualiser.UseVisualStyleBackColor = False
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = My.Resources.Resources.Titre_journaux
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Padding = New Padding(16, 4, 16, 4)
        enTete.Size = New Size(965, 58)
        enTete.SousTitre = "Journal technique de l'application, en lecture seule : démarrages, connexions, erreurs, opérations importantes."
        enTete.TabIndex = 3
        enTete.Text = "Journaux"
        ' 
        ' UC_Journaux
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_Journaux"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        carteJournal.ResumeLayout(False)
        CType(dgvJournal, ComponentModel.ISupportInitialize).EndInit()
        carteDetail.ResumeLayout(False)
        carteDetail.PerformLayout()
        carteFiltres.ResumeLayout(False)
        tblFiltres.ResumeLayout(False)
        tblFiltres.PerformLayout()
        pnlActions.ResumeLayout(False)
        pnlActions.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteFiltres As AltheaCarte
    Friend WithEvents tblFiltres As TableLayoutPanel
    Friend WithEvents lblTitreJour As Label
    Friend WithEvents cboJour As ComboBox
    Friend WithEvents lblTitreNiveau As Label
    Friend WithEvents cboNiveau As ComboBox
    Friend WithEvents lblTitreCategorie As Label
    Friend WithEvents cboCategorie As ComboBox
    Friend WithEvents lblTitreRecherche As Label
    Friend WithEvents txtRecherche As TextBox
    Friend WithEvents chkErreursSeules As CheckBox
    Friend WithEvents pnlEspace1 As Panel
    Friend WithEvents carteJournal As AltheaCarte
    Friend WithEvents dgvJournal As DataGridView
    Friend WithEvents colHeure As DataGridViewTextBoxColumn
    Friend WithEvents colNiveau As DataGridViewTextBoxColumn
    Friend WithEvents colCategorie As DataGridViewTextBoxColumn
    Friend WithEvents colMessage As DataGridViewTextBoxColumn
    Friend WithEvents pnlEspace2 As Panel
    Friend WithEvents carteDetail As AltheaCarte
    Friend WithEvents txtDetail As TextBox
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents lblCompteur As Label
    Friend WithEvents btnOuvrirDossier As AltheaBouton
    Friend WithEvents btnActualiser As AltheaBouton

End Class
