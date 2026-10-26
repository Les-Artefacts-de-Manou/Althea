<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_Sauvegardes
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Sauvegardes))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        carteHistorique = New AltheaCarte()
        dgvHistorique = New DataGridView()
        colDate = New DataGridViewTextBoxColumn()
        colStatut = New DataGridViewTextBoxColumn()
        colFichier = New DataGridViewTextBoxColumn()
        colTaille = New DataGridViewTextBoxColumn()
        colNbFichiers = New DataGridViewTextBoxColumn()
        colUtilisateur = New DataGridViewTextBoxColumn()
        colMessage = New DataGridViewTextBoxColumn()
        pnlEspace2 = New Panel()
        carteMotDePasse = New AltheaCarte()
        tblMotDePasse = New TableLayoutPanel()
        lblExplicationMotDePasse = New Label()
        lblTitreEtatMotDePasse = New Label()
        lblEtatMotDePasse = New Label()
        lblTitreMotDePasse = New Label()
        txtMotDePasse = New TextBox()
        btnVoirMotDePasse = New AltheaBouton()
        btnDefinirMotDePasse = New AltheaBouton()
        lblTitreConfirmation = New Label()
        txtConfirmation = New TextBox()
        lblReglesMotDePasse = New Label()
        lblTitreVerification = New Label()
        txtVerification = New TextBox()
        btnVoirVerification = New AltheaBouton()
        btnVerifierMotDePasse = New AltheaBouton()
        lblAvertissementMotDePasse = New Label()
        pnlEspace1 = New Panel()
        carteSauvegarde = New AltheaCarte()
        tblSauvegarde = New TableLayoutPanel()
        lblTitreDossier = New Label()
        lblDossier = New Label()
        btnChoisirDossier = New AltheaBouton()
        lblTitreDerniere = New Label()
        lblDerniere = New Label()
        jaugeProgression = New AltheaJauge()
        lblEtape = New Label()
        pnlActions = New AltheaBarreActions()
        btnSauvegarder = New AltheaBouton()
        btnActualiser = New AltheaBouton()
        btnRestaurer = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        carteHistorique.SuspendLayout()
        CType(dgvHistorique, ComponentModel.ISupportInitialize).BeginInit()
        carteMotDePasse.SuspendLayout()
        tblMotDePasse.SuspendLayout()
        carteSauvegarde.SuspendLayout()
        tblSauvegarde.SuspendLayout()
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
        pnlCenter.Controls.Add(carteHistorique)
        pnlCenter.Controls.Add(pnlEspace2)
        pnlCenter.Controls.Add(carteMotDePasse)
        pnlCenter.Controls.Add(pnlEspace1)
        pnlCenter.Controls.Add(carteSauvegarde)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(965, 624)
        pnlCenter.TabIndex = 1
        ' 
        ' carteHistorique
        ' 
        carteHistorique.Controls.Add(dgvHistorique)
        carteHistorique.Dock = DockStyle.Fill
        carteHistorique.Location = New Point(16, 492)
        carteHistorique.Name = "carteHistorique"
        carteHistorique.Padding = New Padding(14, 10, 14, 12)
        carteHistorique.Size = New Size(933, 124)
        carteHistorique.TabIndex = 4
        carteHistorique.Titre = "Historique"
        ' 
        ' dgvHistorique
        ' 
        dgvHistorique.AllowUserToAddRows = False
        dgvHistorique.AllowUserToDeleteRows = False
        dgvHistorique.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvHistorique.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvHistorique.Columns.AddRange(New DataGridViewColumn() {colDate, colStatut, colFichier, colTaille, colNbFichiers, colUtilisateur, colMessage})
        dgvHistorique.Dock = DockStyle.Fill
        dgvHistorique.Location = New Point(14, 44)
        dgvHistorique.Name = "dgvHistorique"
        dgvHistorique.ReadOnly = True
        dgvHistorique.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvHistorique.Size = New Size(905, 138)
        dgvHistorique.TabIndex = 0
        ' 
        ' colDate
        ' 
        colDate.HeaderText = "Date"
        colDate.Name = "colDate"
        colDate.ReadOnly = True
        colDate.Width = 130
        ' 
        ' colStatut
        ' 
        colStatut.HeaderText = "Résultat"
        colStatut.Name = "colStatut"
        colStatut.ReadOnly = True
        colStatut.Width = 115
        ' 
        ' colFichier
        ' 
        colFichier.HeaderText = "Archive"
        colFichier.Name = "colFichier"
        colFichier.ReadOnly = True
        colFichier.Width = 200
        ' 
        ' colTaille
        ' 
        colTaille.HeaderText = "Taille"
        colTaille.Name = "colTaille"
        colTaille.ReadOnly = True
        colTaille.Width = 80
        ' 
        ' colNbFichiers
        ' 
        colNbFichiers.HeaderText = "Fichiers"
        colNbFichiers.Name = "colNbFichiers"
        colNbFichiers.ReadOnly = True
        colNbFichiers.Width = 70
        ' 
        ' colUtilisateur
        ' 
        colUtilisateur.HeaderText = "Par"
        colUtilisateur.Name = "colUtilisateur"
        colUtilisateur.ReadOnly = True
        colUtilisateur.Width = 110
        ' 
        ' colMessage
        ' 
        colMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colMessage.HeaderText = "Détail"
        colMessage.Name = "colMessage"
        colMessage.ReadOnly = True
        ' 
        ' pnlEspace2
        ' 
        pnlEspace2.BackColor = Color.Transparent
        pnlEspace2.Dock = DockStyle.Top
        pnlEspace2.Location = New Point(16, 480)
        pnlEspace2.Name = "pnlEspace2"
        pnlEspace2.Size = New Size(933, 12)
        pnlEspace2.TabIndex = 3
        ' 
        ' carteMotDePasse
        ' 
        carteMotDePasse.Controls.Add(tblMotDePasse)
        carteMotDePasse.Dock = DockStyle.Top
        carteMotDePasse.Location = New Point(16, 208)
        carteMotDePasse.Name = "carteMotDePasse"
        carteMotDePasse.Padding = New Padding(14, 10, 14, 10)
        carteMotDePasse.Size = New Size(933, 276)
        carteMotDePasse.TabIndex = 2
        carteMotDePasse.Titre = "Mot de passe des sauvegardes"
        ' 
        ' tblMotDePasse
        ' 
        tblMotDePasse.BackColor = Color.Transparent
        tblMotDePasse.ColumnCount = 5
        tblMotDePasse.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 200F))
        tblMotDePasse.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 270F))
        tblMotDePasse.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 46F))
        tblMotDePasse.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 170F))
        tblMotDePasse.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        tblMotDePasse.Controls.Add(lblExplicationMotDePasse, 0, 0)
        tblMotDePasse.Controls.Add(lblTitreEtatMotDePasse, 0, 1)
        tblMotDePasse.Controls.Add(lblEtatMotDePasse, 1, 1)
        tblMotDePasse.Controls.Add(lblTitreMotDePasse, 0, 2)
        tblMotDePasse.Controls.Add(txtMotDePasse, 1, 2)
        tblMotDePasse.Controls.Add(btnVoirMotDePasse, 2, 2)
        tblMotDePasse.Controls.Add(lblTitreConfirmation, 0, 3)
        tblMotDePasse.Controls.Add(txtConfirmation, 1, 3)
        tblMotDePasse.Controls.Add(btnDefinirMotDePasse, 3, 3)
        tblMotDePasse.Controls.Add(lblReglesMotDePasse, 1, 4)
        tblMotDePasse.Controls.Add(lblTitreVerification, 0, 5)
        tblMotDePasse.Controls.Add(txtVerification, 1, 5)
        tblMotDePasse.Controls.Add(btnVoirVerification, 2, 5)
        tblMotDePasse.Controls.Add(btnVerifierMotDePasse, 3, 5)
        tblMotDePasse.Controls.Add(lblAvertissementMotDePasse, 0, 6)
        tblMotDePasse.Dock = DockStyle.Fill
        tblMotDePasse.Location = New Point(14, 44)
        tblMotDePasse.Name = "tblMotDePasse"
        tblMotDePasse.RowCount = 7
        tblMotDePasse.RowStyles.Add(New RowStyle(SizeType.Absolute, 26F))
        tblMotDePasse.RowStyles.Add(New RowStyle(SizeType.Absolute, 26F))
        tblMotDePasse.RowStyles.Add(New RowStyle(SizeType.Absolute, 34F))
        tblMotDePasse.RowStyles.Add(New RowStyle(SizeType.Absolute, 34F))
        tblMotDePasse.RowStyles.Add(New RowStyle(SizeType.Absolute, 24F))
        tblMotDePasse.RowStyles.Add(New RowStyle(SizeType.Absolute, 34F))
        tblMotDePasse.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        tblMotDePasse.Size = New Size(905, 222)
        tblMotDePasse.TabIndex = 0
        ' 
        ' lblExplicationMotDePasse
        ' 
        lblExplicationMotDePasse.BackColor = Color.Transparent
        tblMotDePasse.SetColumnSpan(lblExplicationMotDePasse, 5)
        lblExplicationMotDePasse.Dock = DockStyle.Fill
        lblExplicationMotDePasse.Name = "lblExplicationMotDePasse"
        lblExplicationMotDePasse.TabIndex = 0
        lblExplicationMotDePasse.Text = "Mot de passe propre aux sauvegardes, commun au cabinet : il chiffre les archives. Ce n'est pas un mot de passe de connexion."
        lblExplicationMotDePasse.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' lblTitreEtatMotDePasse
        ' 
        lblTitreEtatMotDePasse.Anchor = AnchorStyles.Left
        lblTitreEtatMotDePasse.AutoSize = True
        lblTitreEtatMotDePasse.BackColor = Color.Transparent
        lblTitreEtatMotDePasse.Name = "lblTitreEtatMotDePasse"
        lblTitreEtatMotDePasse.TabIndex = 1
        lblTitreEtatMotDePasse.Text = "État :"
        ' 
        ' lblEtatMotDePasse
        ' 
        lblEtatMotDePasse.Anchor = AnchorStyles.Left
        lblEtatMotDePasse.AutoSize = True
        lblEtatMotDePasse.BackColor = Color.Transparent
        tblMotDePasse.SetColumnSpan(lblEtatMotDePasse, 4)
        lblEtatMotDePasse.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblEtatMotDePasse.Name = "lblEtatMotDePasse"
        lblEtatMotDePasse.TabIndex = 2
        lblEtatMotDePasse.Text = "—"
        ' 
        ' lblTitreMotDePasse
        ' 
        lblTitreMotDePasse.Anchor = AnchorStyles.Left
        lblTitreMotDePasse.AutoSize = True
        lblTitreMotDePasse.BackColor = Color.Transparent
        lblTitreMotDePasse.Name = "lblTitreMotDePasse"
        lblTitreMotDePasse.TabIndex = 3
        lblTitreMotDePasse.Text = "Nouveau mot de passe :"
        ' 
        ' txtMotDePasse
        ' 
        txtMotDePasse.Anchor = AnchorStyles.Left
        txtMotDePasse.Name = "txtMotDePasse"
        txtMotDePasse.Size = New Size(255, 24)
        txtMotDePasse.TabIndex = 4
        txtMotDePasse.UseSystemPasswordChar = True
        ' 
        ' btnVoirMotDePasse
        ' 
        btnVoirMotDePasse.Anchor = AnchorStyles.Left
        btnVoirMotDePasse.Name = "btnVoirMotDePasse"
        btnVoirMotDePasse.Size = New Size(36, 30)
        btnVoirMotDePasse.TabIndex = 5
        btnVoirMotDePasse.TabStop = False
        btnVoirMotDePasse.Tag = "voir_normal"
        btnVoirMotDePasse.Variante = VarianteBouton.Outil
        ' 
        ' lblTitreConfirmation
        ' 
        lblTitreConfirmation.Anchor = AnchorStyles.Left
        lblTitreConfirmation.AutoSize = True
        lblTitreConfirmation.BackColor = Color.Transparent
        lblTitreConfirmation.Name = "lblTitreConfirmation"
        lblTitreConfirmation.TabIndex = 6
        lblTitreConfirmation.Text = "Confirmation :"
        ' 
        ' txtConfirmation
        ' 
        txtConfirmation.Anchor = AnchorStyles.Left
        txtConfirmation.Name = "txtConfirmation"
        txtConfirmation.Size = New Size(255, 24)
        txtConfirmation.TabIndex = 7
        txtConfirmation.UseSystemPasswordChar = True
        ' 
        ' btnDefinirMotDePasse
        ' 
        btnDefinirMotDePasse.Anchor = AnchorStyles.Left
        btnDefinirMotDePasse.Name = "btnDefinirMotDePasse"
        btnDefinirMotDePasse.Size = New Size(160, 30)
        btnDefinirMotDePasse.TabIndex = 8
        btnDefinirMotDePasse.Tag = "enregistrer_normal"
        btnDefinirMotDePasse.Text = "Enregistrer"
        btnDefinirMotDePasse.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' lblReglesMotDePasse
        ' 
        lblReglesMotDePasse.BackColor = Color.Transparent
        tblMotDePasse.SetColumnSpan(lblReglesMotDePasse, 4)
        lblReglesMotDePasse.Dock = DockStyle.Fill
        lblReglesMotDePasse.Font = New Font("Calibri", 9F, FontStyle.Italic)
        lblReglesMotDePasse.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblReglesMotDePasse.Name = "lblReglesMotDePasse"
        lblReglesMotDePasse.TabIndex = 9
        lblReglesMotDePasse.TextAlign = ContentAlignment.TopLeft
        ' 
        ' lblTitreVerification
        ' 
        lblTitreVerification.Anchor = AnchorStyles.Left
        lblTitreVerification.AutoSize = True
        lblTitreVerification.BackColor = Color.Transparent
        lblTitreVerification.Name = "lblTitreVerification"
        lblTitreVerification.TabIndex = 10
        lblTitreVerification.Text = "Vérifier un mot de passe :"
        ' 
        ' txtVerification
        ' 
        txtVerification.Anchor = AnchorStyles.Left
        txtVerification.Name = "txtVerification"
        txtVerification.Size = New Size(255, 24)
        txtVerification.TabIndex = 11
        txtVerification.UseSystemPasswordChar = True
        ' 
        ' btnVoirVerification
        ' 
        btnVoirVerification.Anchor = AnchorStyles.Left
        btnVoirVerification.Name = "btnVoirVerification"
        btnVoirVerification.Size = New Size(36, 30)
        btnVoirVerification.TabIndex = 12
        btnVoirVerification.TabStop = False
        btnVoirVerification.Tag = "voir_normal"
        btnVoirVerification.Variante = VarianteBouton.Outil
        ' 
        ' btnVerifierMotDePasse
        ' 
        btnVerifierMotDePasse.Anchor = AnchorStyles.Left
        btnVerifierMotDePasse.Name = "btnVerifierMotDePasse"
        btnVerifierMotDePasse.Size = New Size(160, 30)
        btnVerifierMotDePasse.TabIndex = 13
        btnVerifierMotDePasse.Tag = "valider_normal"
        btnVerifierMotDePasse.Text = "Vérifier"
        btnVerifierMotDePasse.Variante = VarianteBouton.Outil
        ' 
        ' lblAvertissementMotDePasse
        ' 
        lblAvertissementMotDePasse.BackColor = Color.Transparent
        tblMotDePasse.SetColumnSpan(lblAvertissementMotDePasse, 5)
        lblAvertissementMotDePasse.Dock = DockStyle.Fill
        lblAvertissementMotDePasse.Font = New Font("Calibri", 9F, FontStyle.Italic)
        lblAvertissementMotDePasse.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblAvertissementMotDePasse.Name = "lblAvertissementMotDePasse"
        lblAvertissementMotDePasse.TabIndex = 14
        lblAvertissementMotDePasse.Text = "À noter et à garder en lieu sûr : sans lui, une sauvegarde ne peut pas être restaurée sur un autre poste. Le changer ne modifie pas les archives déjà faites, qui gardent l'ancien."
        ' 
        ' pnlEspace1
        ' 
        pnlEspace1.BackColor = Color.Transparent
        pnlEspace1.Dock = DockStyle.Top
        pnlEspace1.Location = New Point(16, 196)
        pnlEspace1.Name = "pnlEspace1"
        pnlEspace1.Size = New Size(933, 12)
        pnlEspace1.TabIndex = 1
        ' 
        ' carteSauvegarde
        ' 
        carteSauvegarde.Controls.Add(tblSauvegarde)
        carteSauvegarde.Dock = DockStyle.Top
        carteSauvegarde.Location = New Point(16, 16)
        carteSauvegarde.Name = "carteSauvegarde"
        carteSauvegarde.Padding = New Padding(14, 10, 14, 10)
        carteSauvegarde.Size = New Size(933, 180)
        carteSauvegarde.TabIndex = 0
        carteSauvegarde.Titre = "Sauvegarde"
        ' 
        ' tblSauvegarde
        ' 
        tblSauvegarde.BackColor = Color.Transparent
        tblSauvegarde.ColumnCount = 3
        tblSauvegarde.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 200F))
        tblSauvegarde.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        tblSauvegarde.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 150F))
        tblSauvegarde.Controls.Add(lblTitreDossier, 0, 0)
        tblSauvegarde.Controls.Add(lblDossier, 1, 0)
        tblSauvegarde.Controls.Add(btnChoisirDossier, 2, 0)
        tblSauvegarde.Controls.Add(lblTitreDerniere, 0, 1)
        tblSauvegarde.Controls.Add(lblDerniere, 1, 1)
        tblSauvegarde.Controls.Add(jaugeProgression, 0, 2)
        tblSauvegarde.Controls.Add(lblEtape, 0, 3)
        tblSauvegarde.Dock = DockStyle.Fill
        tblSauvegarde.Location = New Point(14, 44)
        tblSauvegarde.Name = "tblSauvegarde"
        tblSauvegarde.RowCount = 4
        tblSauvegarde.RowStyles.Add(New RowStyle(SizeType.Absolute, 36F))
        tblSauvegarde.RowStyles.Add(New RowStyle(SizeType.Absolute, 30F))
        tblSauvegarde.RowStyles.Add(New RowStyle(SizeType.Absolute, 26F))
        tblSauvegarde.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        tblSauvegarde.Size = New Size(905, 126)
        tblSauvegarde.TabIndex = 0
        ' 
        ' lblTitreDossier
        ' 
        lblTitreDossier.Anchor = AnchorStyles.Left
        lblTitreDossier.AutoSize = True
        lblTitreDossier.BackColor = Color.Transparent
        lblTitreDossier.Name = "lblTitreDossier"
        lblTitreDossier.TabIndex = 0
        lblTitreDossier.Text = "Dossier des sauvegardes :"
        ' 
        ' lblDossier
        ' 
        lblDossier.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        lblDossier.AutoEllipsis = True
        lblDossier.BackColor = Color.Transparent
        lblDossier.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblDossier.Name = "lblDossier"
        lblDossier.Size = New Size(549, 20)
        lblDossier.TabIndex = 1
        lblDossier.Text = "—"
        ' 
        ' btnChoisirDossier
        ' 
        btnChoisirDossier.Anchor = AnchorStyles.Left
        btnChoisirDossier.Name = "btnChoisirDossier"
        btnChoisirDossier.Size = New Size(140, 30)
        btnChoisirDossier.TabIndex = 2
        btnChoisirDossier.Tag = "ouvrir_dossier_normal"
        btnChoisirDossier.Text = "Choisir…"
        btnChoisirDossier.Variante = VarianteBouton.Outil
        ' 
        ' lblTitreDerniere
        ' 
        lblTitreDerniere.Anchor = AnchorStyles.Left
        lblTitreDerniere.AutoSize = True
        lblTitreDerniere.BackColor = Color.Transparent
        lblTitreDerniere.Name = "lblTitreDerniere"
        lblTitreDerniere.TabIndex = 3
        lblTitreDerniere.Text = "Dernière sauvegarde réussie :"
        ' 
        ' lblDerniere
        ' 
        lblDerniere.Anchor = AnchorStyles.Left
        lblDerniere.AutoSize = True
        lblDerniere.BackColor = Color.Transparent
        tblSauvegarde.SetColumnSpan(lblDerniere, 2)
        lblDerniere.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblDerniere.Name = "lblDerniere"
        lblDerniere.TabIndex = 4
        lblDerniere.Text = "—"
        ' 
        ' jaugeProgression
        ' 
        jaugeProgression.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        tblSauvegarde.SetColumnSpan(jaugeProgression, 3)
        jaugeProgression.Maximum = 100
        jaugeProgression.Name = "jaugeProgression"
        jaugeProgression.SeuilAlerte = 0
        jaugeProgression.Size = New Size(899, 14)
        jaugeProgression.TabIndex = 5
        jaugeProgression.Valeur = 0
        jaugeProgression.Visible = False
        ' 
        ' lblEtape
        ' 
        lblEtape.BackColor = Color.Transparent
        tblSauvegarde.SetColumnSpan(lblEtape, 3)
        lblEtape.Dock = DockStyle.Fill
        lblEtape.Font = New Font("Calibri", 9F, FontStyle.Italic)
        lblEtape.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblEtape.Name = "lblEtape"
        lblEtape.TabIndex = 6
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnSauvegarder)
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.Controls.Add(btnRestaurer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(965, 60)
        pnlActions.TabIndex = 2
        ' 
        ' btnSauvegarder
        ' 
        btnSauvegarder.Location = New Point(620, 10)
        btnSauvegarder.Name = "btnSauvegarder"
        btnSauvegarder.Size = New Size(200, 40)
        btnSauvegarder.TabIndex = 0
        btnSauvegarder.Tag = "enregistrer_normal"
        btnSauvegarder.Text = "Sauvegarder maintenant"
        ' 
        ' btnActualiser
        ' 
        btnActualiser.Location = New Point(830, 10)
        btnActualiser.Name = "btnActualiser"
        btnActualiser.Size = New Size(120, 40)
        btnActualiser.TabIndex = 1
        btnActualiser.Tag = "actualiser_normal"
        btnActualiser.Text = "Actualiser"
        btnActualiser.Variante = VarianteBouton.Clair
        ' 
        ' btnRestaurer
        ' 
        btnRestaurer.Location = New Point(410, 10)
        btnRestaurer.Name = "btnRestaurer"
        btnRestaurer.Size = New Size(200, 40)
        btnRestaurer.TabIndex = 2
        btnRestaurer.Tag = "importer_normal"
        btnRestaurer.Text = "Restaurer…"
        btnRestaurer.Variante = VarianteBouton.Terre
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 3
        enTete.SousTitre = "Sauvegardez la base et les documents dans une archive chiffrée, hors du dossier de données."
        enTete.Text = "Sauvegardes"
        ' 
        ' UC_Sauvegardes
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_Sauvegardes"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        carteHistorique.ResumeLayout(False)
        CType(dgvHistorique, ComponentModel.ISupportInitialize).EndInit()
        carteMotDePasse.ResumeLayout(False)
        tblMotDePasse.ResumeLayout(False)
        tblMotDePasse.PerformLayout()
        carteSauvegarde.ResumeLayout(False)
        tblSauvegarde.ResumeLayout(False)
        tblSauvegarde.PerformLayout()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnSauvegarder As AltheaBouton
    Friend WithEvents btnActualiser As AltheaBouton
    Friend WithEvents btnRestaurer As AltheaBouton
    Friend WithEvents carteSauvegarde As AltheaCarte
    Friend WithEvents tblSauvegarde As TableLayoutPanel
    Friend WithEvents lblTitreDossier As Label
    Friend WithEvents lblDossier As Label
    Friend WithEvents btnChoisirDossier As AltheaBouton
    Friend WithEvents lblTitreDerniere As Label
    Friend WithEvents lblDerniere As Label
    Friend WithEvents jaugeProgression As AltheaJauge
    Friend WithEvents lblEtape As Label
    Friend WithEvents pnlEspace1 As Panel
    Friend WithEvents carteMotDePasse As AltheaCarte
    Friend WithEvents tblMotDePasse As TableLayoutPanel
    Friend WithEvents lblTitreEtatMotDePasse As Label
    Friend WithEvents lblEtatMotDePasse As Label
    Friend WithEvents lblTitreMotDePasse As Label
    Friend WithEvents txtMotDePasse As TextBox
    Friend WithEvents btnDefinirMotDePasse As AltheaBouton
    Friend WithEvents lblTitreConfirmation As Label
    Friend WithEvents txtConfirmation As TextBox
    Friend WithEvents btnVerifierMotDePasse As AltheaBouton
    Friend WithEvents lblExplicationMotDePasse As Label
    Friend WithEvents btnVoirMotDePasse As AltheaBouton
    Friend WithEvents lblReglesMotDePasse As Label
    Friend WithEvents lblTitreVerification As Label
    Friend WithEvents txtVerification As TextBox
    Friend WithEvents btnVoirVerification As AltheaBouton
    Friend WithEvents lblAvertissementMotDePasse As Label
    Friend WithEvents pnlEspace2 As Panel
    Friend WithEvents carteHistorique As AltheaCarte
    Friend WithEvents dgvHistorique As DataGridView
    Friend WithEvents colDate As DataGridViewTextBoxColumn
    Friend WithEvents colStatut As DataGridViewTextBoxColumn
    Friend WithEvents colFichier As DataGridViewTextBoxColumn
    Friend WithEvents colTaille As DataGridViewTextBoxColumn
    Friend WithEvents colNbFichiers As DataGridViewTextBoxColumn
    Friend WithEvents colUtilisateur As DataGridViewTextBoxColumn
    Friend WithEvents colMessage As DataGridViewTextBoxColumn

End Class
