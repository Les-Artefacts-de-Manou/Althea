' -------------------------------------------------------------------------------------------------
' Fichier     : DialogRestauration.Designer.vb
' Projet      : Althéa
'
' Rôle        :
' Fichier de conception de DialogRestauration. Porte la disposition et l'apparence des
' contrôles (Designer obligatoire). Les valeurs dynamiques restent dans le code-behind.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DialogRestauration
    Inherits Form

    Private components As System.ComponentModel.IContainer

    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogRestauration))
        enTete = New AltheaEnTete()
        carteArchive = New AltheaCarte()
        lblTitreArchive = New Label()
        cboArchives = New AltheaListe()
        btnParcourir = New AltheaBouton()
        lblTitreMotDePasseArchive = New Label()
        chkAutreMotDePasse = New AltheaCase()
        txtMotDePasseArchive = New TextBox()
        btnVoirArchive = New AltheaBouton()
        carteCompte = New AltheaCarte()
        lblTitreCompte = New Label()
        txtCompteAdmin = New TextBox()
        lblTitreMotDePasseAdmin = New Label()
        txtMotDePasseAdmin = New TextBox()
        btnVoirAdmin = New AltheaBouton()
        lblExplicationCompte = New Label()
        carteVerification = New AltheaCarte()
        btnVerifier = New AltheaBouton()
        lblVerdict = New Label()
        lblResume = New Label()
        lblAvertissement = New Label()
        jaugeProgression = New AltheaJauge()
        lblEtape = New Label()
        carteHistorique = New AltheaCarte()
        dgvHistorique = New DataGridView()
        colDate = New DataGridViewTextBoxColumn()
        colStatut = New DataGridViewTextBoxColumn()
        colArchive = New DataGridViewTextBoxColumn()
        colPar = New DataGridViewTextBoxColumn()
        colDetail = New DataGridViewTextBoxColumn()
        pnlActions = New AltheaBarreActions()
        btnRestaurer = New AltheaBouton()
        btnFermer = New AltheaBouton()
        carteArchive.SuspendLayout()
        carteCompte.SuspendLayout()
        carteVerification.SuspendLayout()
        carteHistorique.SuspendLayout()
        CType(dgvHistorique, System.ComponentModel.ISupportInitialize).BeginInit()
        pnlActions.SuspendLayout()
        SuspendLayout()
        '
        ' enTete
        '
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(780, 62)
        enTete.TabIndex = 10
        enTete.SousTitre = "Remplace la base et les documents actuels par ceux d'une archive de sauvegarde"
        enTete.Text = "Restaurer une sauvegarde"
        '
        ' carteArchive
        '
        carteArchive.Controls.Add(lblTitreArchive)
        carteArchive.Controls.Add(cboArchives)
        carteArchive.Controls.Add(btnParcourir)
        carteArchive.Controls.Add(lblTitreMotDePasseArchive)
        carteArchive.Controls.Add(chkAutreMotDePasse)
        carteArchive.Controls.Add(txtMotDePasseArchive)
        carteArchive.Controls.Add(btnVoirArchive)
        carteArchive.Location = New Point(12, 70)
        carteArchive.Name = "carteArchive"
        carteArchive.Padding = New Padding(12, 8, 12, 10)
        carteArchive.Size = New Size(756, 145)
        carteArchive.TabIndex = 0
        carteArchive.TabStop = False
        carteArchive.Titre = "Archive à restaurer"
        '
        ' lblTitreArchive
        '
        lblTitreArchive.AutoSize = True
        lblTitreArchive.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTitreArchive.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreArchive.Location = New Point(14, 44)
        lblTitreArchive.Name = "lblTitreArchive"
        lblTitreArchive.Size = New Size(49, 17)
        lblTitreArchive.TabIndex = 0
        lblTitreArchive.Text = "Archive"
        '
        ' cboArchives
        '
        cboArchives.Font = New Font("Calibri", 11F)
        cboArchives.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboArchives.ItemHeight = 20
        cboArchives.Location = New Point(200, 40)
        cboArchives.Name = "cboArchives"
        cboArchives.Size = New Size(380, 26)
        cboArchives.TabIndex = 1
        '
        ' btnParcourir
        '
        btnParcourir.Location = New Point(586, 38)
        btnParcourir.Name = "btnParcourir"
        btnParcourir.Size = New Size(140, 30)
        btnParcourir.TabIndex = 2
        btnParcourir.Tag = "ouvrir_dossier_normal"
        btnParcourir.Text = "Parcourir…"
        btnParcourir.Variante = VarianteBouton.Outil
        '
        ' lblTitreMotDePasseArchive
        '
        lblTitreMotDePasseArchive.AutoSize = True
        lblTitreMotDePasseArchive.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTitreMotDePasseArchive.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreMotDePasseArchive.Location = New Point(14, 80)
        lblTitreMotDePasseArchive.Name = "lblTitreMotDePasseArchive"
        lblTitreMotDePasseArchive.Size = New Size(160, 17)
        lblTitreMotDePasseArchive.TabIndex = 3
        lblTitreMotDePasseArchive.Text = "Mot de passe de l'archive"
        '
        ' chkAutreMotDePasse
        '
        chkAutreMotDePasse.AutoSize = True
        chkAutreMotDePasse.Font = New Font("Calibri", 11F)
        chkAutreMotDePasse.Location = New Point(200, 78)
        chkAutreMotDePasse.Name = "chkAutreMotDePasse"
        chkAutreMotDePasse.Size = New Size(400, 21)
        chkAutreMotDePasse.TabIndex = 4
        chkAutreMotDePasse.Text = "Saisir un autre mot de passe que celui enregistré sur ce poste"
        '
        ' txtMotDePasseArchive
        '
        txtMotDePasseArchive.BackColor = Color.White
        txtMotDePasseArchive.Enabled = False
        txtMotDePasseArchive.Font = New Font("Calibri", 11F)
        txtMotDePasseArchive.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtMotDePasseArchive.Location = New Point(200, 106)
        txtMotDePasseArchive.Name = "txtMotDePasseArchive"
        txtMotDePasseArchive.Size = New Size(380, 25)
        txtMotDePasseArchive.TabIndex = 5
        txtMotDePasseArchive.UseSystemPasswordChar = True
        '
        ' btnVoirArchive
        '
        btnVoirArchive.Enabled = False
        btnVoirArchive.Image = CType(resources.GetObject("btnVoirArchive.Image"), Image)
        btnVoirArchive.Location = New Point(586, 104)
        btnVoirArchive.Name = "btnVoirArchive"
        btnVoirArchive.Size = New Size(30, 30)
        btnVoirArchive.TabIndex = 6
        btnVoirArchive.TabStop = False
        btnVoirArchive.Tag = "voir_normal"
        btnVoirArchive.Variante = VarianteBouton.Outil
        '
        ' carteCompte
        '
        carteCompte.Controls.Add(lblTitreCompte)
        carteCompte.Controls.Add(txtCompteAdmin)
        carteCompte.Controls.Add(lblTitreMotDePasseAdmin)
        carteCompte.Controls.Add(txtMotDePasseAdmin)
        carteCompte.Controls.Add(btnVoirAdmin)
        carteCompte.Controls.Add(lblExplicationCompte)
        carteCompte.Location = New Point(12, 223)
        carteCompte.Name = "carteCompte"
        carteCompte.Padding = New Padding(12, 8, 12, 10)
        carteCompte.Size = New Size(756, 150)
        carteCompte.TabIndex = 1
        carteCompte.TabStop = False
        carteCompte.Titre = "Compte d'administration MariaDB"
        '
        ' lblTitreCompte
        '
        lblTitreCompte.AutoSize = True
        lblTitreCompte.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTitreCompte.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreCompte.Location = New Point(14, 44)
        lblTitreCompte.Name = "lblTitreCompte"
        lblTitreCompte.Size = New Size(52, 17)
        lblTitreCompte.TabIndex = 0
        lblTitreCompte.Text = "Compte"
        '
        ' txtCompteAdmin
        '
        txtCompteAdmin.BackColor = Color.White
        txtCompteAdmin.Font = New Font("Calibri", 11F)
        txtCompteAdmin.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCompteAdmin.Location = New Point(200, 40)
        txtCompteAdmin.Name = "txtCompteAdmin"
        txtCompteAdmin.Size = New Size(200, 25)
        txtCompteAdmin.TabIndex = 1
        '
        ' lblTitreMotDePasseAdmin
        '
        lblTitreMotDePasseAdmin.AutoSize = True
        lblTitreMotDePasseAdmin.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTitreMotDePasseAdmin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreMotDePasseAdmin.Location = New Point(14, 78)
        lblTitreMotDePasseAdmin.Name = "lblTitreMotDePasseAdmin"
        lblTitreMotDePasseAdmin.Size = New Size(79, 17)
        lblTitreMotDePasseAdmin.TabIndex = 2
        lblTitreMotDePasseAdmin.Text = "Mot de passe"
        '
        ' txtMotDePasseAdmin
        '
        txtMotDePasseAdmin.BackColor = Color.White
        txtMotDePasseAdmin.Font = New Font("Calibri", 11F)
        txtMotDePasseAdmin.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtMotDePasseAdmin.Location = New Point(200, 74)
        txtMotDePasseAdmin.Name = "txtMotDePasseAdmin"
        txtMotDePasseAdmin.Size = New Size(380, 25)
        txtMotDePasseAdmin.TabIndex = 3
        txtMotDePasseAdmin.UseSystemPasswordChar = True
        '
        ' btnVoirAdmin
        '
        btnVoirAdmin.Image = CType(resources.GetObject("btnVoirAdmin.Image"), Image)
        btnVoirAdmin.Location = New Point(586, 72)
        btnVoirAdmin.Name = "btnVoirAdmin"
        btnVoirAdmin.Size = New Size(30, 30)
        btnVoirAdmin.TabIndex = 4
        btnVoirAdmin.TabStop = False
        btnVoirAdmin.Tag = "voir_normal"
        btnVoirAdmin.Variante = VarianteBouton.Outil
        '
        ' lblExplicationCompte
        '
        lblExplicationCompte.Font = New Font("Calibri", 10F, FontStyle.Italic)
        lblExplicationCompte.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblExplicationCompte.Location = New Point(14, 106)
        lblExplicationCompte.Name = "lblExplicationCompte"
        lblExplicationCompte.Size = New Size(728, 34)
        lblExplicationCompte.TabIndex = 5
        lblExplicationCompte.Text = "Recréer la base demande un compte d'administration de MariaDB (souvent « root »), différent du compte de l'application. Il sert à cette seule opération et n'est jamais enregistré."
        '
        ' carteVerification
        '
        carteVerification.Controls.Add(btnVerifier)
        carteVerification.Controls.Add(lblVerdict)
        carteVerification.Controls.Add(lblResume)
        carteVerification.Controls.Add(lblAvertissement)
        carteVerification.Controls.Add(jaugeProgression)
        carteVerification.Controls.Add(lblEtape)
        carteVerification.Location = New Point(12, 381)
        carteVerification.Name = "carteVerification"
        carteVerification.Padding = New Padding(12, 8, 12, 10)
        carteVerification.Size = New Size(756, 250)
        carteVerification.TabIndex = 2
        carteVerification.TabStop = False
        carteVerification.Titre = "Vérification"
        '
        ' btnVerifier
        '
        btnVerifier.Location = New Point(14, 40)
        btnVerifier.Name = "btnVerifier"
        btnVerifier.Size = New Size(200, 30)
        btnVerifier.TabIndex = 0
        btnVerifier.Tag = "valider_normal"
        btnVerifier.Text = "Vérifier l'archive"
        btnVerifier.Variante = VarianteBouton.OutilPrincipal
        '
        ' lblVerdict
        '
        lblVerdict.Font = New Font("Calibri", 13F, FontStyle.Bold)
        lblVerdict.Location = New Point(226, 38)
        lblVerdict.Name = "lblVerdict"
        lblVerdict.Size = New Size(516, 34)
        lblVerdict.TabIndex = 1
        lblVerdict.TextAlign = ContentAlignment.MiddleLeft
        '
        ' lblResume
        '
        lblResume.Font = New Font("Calibri", 10.5F)
        lblResume.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblResume.Location = New Point(14, 76)
        lblResume.Name = "lblResume"
        lblResume.Size = New Size(728, 80)
        lblResume.TabIndex = 2
        lblResume.Text = "Choisissez une archive et vérifiez-la : rien n'est modifié tant que vous n'avez pas cliqué sur « Restaurer »."
        '
        ' lblAvertissement
        '
        lblAvertissement.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblAvertissement.ForeColor = Color.FromArgb(CByte(120), CByte(40), CByte(40))
        lblAvertissement.Location = New Point(14, 160)
        lblAvertissement.Name = "lblAvertissement"
        lblAvertissement.Size = New Size(728, 40)
        lblAvertissement.TabIndex = 3
        lblAvertissement.Text = "La restauration remplace TOUTES les données actuelles (patients, dossiers, séances, paiements, documents). Une sauvegarde de sécurité est faite juste avant ; l'ancien dossier de données est conservé, renommé."
        '
        ' jaugeProgression
        '
        jaugeProgression.Location = New Point(14, 204)
        jaugeProgression.Maximum = 100
        jaugeProgression.Name = "jaugeProgression"
        jaugeProgression.SeuilAlerte = 0
        jaugeProgression.Size = New Size(728, 14)
        jaugeProgression.TabIndex = 4
        jaugeProgression.Valeur = 0
        jaugeProgression.Visible = False
        '
        ' lblEtape
        '
        lblEtape.Font = New Font("Calibri", 9.75F, FontStyle.Italic)
        lblEtape.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblEtape.Location = New Point(14, 222)
        lblEtape.Name = "lblEtape"
        lblEtape.Size = New Size(728, 20)
        lblEtape.TabIndex = 5
        '
        ' carteHistorique
        '
        carteHistorique.Controls.Add(dgvHistorique)
        carteHistorique.Location = New Point(12, 639)
        carteHistorique.Name = "carteHistorique"
        carteHistorique.Padding = New Padding(12, 8, 12, 10)
        carteHistorique.Size = New Size(756, 161)
        carteHistorique.TabIndex = 3
        carteHistorique.TabStop = False
        carteHistorique.Titre = "Dernières restaurations"
        '
        ' dgvHistorique
        '
        dgvHistorique.AllowUserToAddRows = False
        dgvHistorique.AllowUserToDeleteRows = False
        dgvHistorique.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvHistorique.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvHistorique.Columns.AddRange(New DataGridViewColumn() {colDate, colStatut, colArchive, colPar, colDetail})
        dgvHistorique.Dock = DockStyle.Fill
        dgvHistorique.Location = New Point(12, 45)
        dgvHistorique.Name = "dgvHistorique"
        dgvHistorique.ReadOnly = True
        dgvHistorique.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvHistorique.Size = New Size(732, 106)
        dgvHistorique.TabIndex = 0
        dgvHistorique.TabStop = False
        '
        ' colDate
        '
        colDate.HeaderText = "Date"
        colDate.Name = "colDate"
        colDate.ReadOnly = True
        colDate.Width = 120
        '
        ' colStatut
        '
        colStatut.HeaderText = "Résultat"
        colStatut.Name = "colStatut"
        colStatut.ReadOnly = True
        colStatut.Width = 100
        '
        ' colArchive
        '
        colArchive.HeaderText = "Archive restaurée"
        colArchive.Name = "colArchive"
        colArchive.ReadOnly = True
        colArchive.Width = 190
        '
        ' colPar
        '
        colPar.HeaderText = "Par"
        colPar.Name = "colPar"
        colPar.ReadOnly = True
        colPar.Width = 90
        '
        ' colDetail
        '
        colDetail.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colDetail.HeaderText = "Détail"
        colDetail.Name = "colDetail"
        colDetail.ReadOnly = True
        '
        ' pnlActions
        '
        pnlActions.Controls.Add(btnRestaurer)
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 808)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(780, 52)
        pnlActions.TabIndex = 4
        '
        ' btnRestaurer
        '
        btnRestaurer.Enabled = False
        btnRestaurer.Location = New Point(500, 7)
        btnRestaurer.Name = "btnRestaurer"
        btnRestaurer.Size = New Size(150, 38)
        btnRestaurer.TabIndex = 0
        btnRestaurer.Tag = "importer_normal"
        btnRestaurer.Text = "Restaurer"
        btnRestaurer.Variante = VarianteBouton.Terre
        '
        ' btnFermer
        '
        btnFermer.Location = New Point(660, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(108, 38)
        btnFermer.TabIndex = 1
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        '
        ' DialogRestauration
        '
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        CancelButton = btnFermer
        ClientSize = New Size(780, 860)
        Controls.Add(carteArchive)
        Controls.Add(carteCompte)
        Controls.Add(carteVerification)
        Controls.Add(carteHistorique)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "DialogRestauration"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Restaurer une sauvegarde"
        carteArchive.ResumeLayout(False)
        carteArchive.PerformLayout()
        carteCompte.ResumeLayout(False)
        carteCompte.PerformLayout()
        carteVerification.ResumeLayout(False)
        carteHistorique.ResumeLayout(False)
        CType(dgvHistorique, System.ComponentModel.ISupportInitialize).EndInit()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteArchive As AltheaCarte
    Friend WithEvents lblTitreArchive As Label
    Friend WithEvents cboArchives As AltheaListe
    Friend WithEvents btnParcourir As AltheaBouton
    Friend WithEvents lblTitreMotDePasseArchive As Label
    Friend WithEvents chkAutreMotDePasse As AltheaCase
    Friend WithEvents txtMotDePasseArchive As TextBox
    Friend WithEvents btnVoirArchive As AltheaBouton
    Friend WithEvents carteCompte As AltheaCarte
    Friend WithEvents lblTitreCompte As Label
    Friend WithEvents txtCompteAdmin As TextBox
    Friend WithEvents lblTitreMotDePasseAdmin As Label
    Friend WithEvents txtMotDePasseAdmin As TextBox
    Friend WithEvents btnVoirAdmin As AltheaBouton
    Friend WithEvents lblExplicationCompte As Label
    Friend WithEvents carteVerification As AltheaCarte
    Friend WithEvents btnVerifier As AltheaBouton
    Friend WithEvents lblVerdict As Label
    Friend WithEvents lblResume As Label
    Friend WithEvents lblAvertissement As Label
    Friend WithEvents jaugeProgression As AltheaJauge
    Friend WithEvents lblEtape As Label
    Friend WithEvents carteHistorique As AltheaCarte
    Friend WithEvents dgvHistorique As DataGridView
    Friend WithEvents colDate As DataGridViewTextBoxColumn
    Friend WithEvents colStatut As DataGridViewTextBoxColumn
    Friend WithEvents colArchive As DataGridViewTextBoxColumn
    Friend WithEvents colPar As DataGridViewTextBoxColumn
    Friend WithEvents colDetail As DataGridViewTextBoxColumn
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnRestaurer As AltheaBouton
    Friend WithEvents btnFermer As AltheaBouton

End Class
