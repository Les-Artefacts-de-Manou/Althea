' -------------------------------------------------------------------------------------------------
' Designer    : UC_RecherchePatientsLieu
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Structure visuelle de la recherche des patients par lieu (ADR-52, R4) : en-tête, carte « Lieu »
' (zone code postal ou localité, case des inactifs), carte « Lieux » à gauche, carte « Patients » et
' barre d'actions (retour, ouvrir la fiche).
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_RecherchePatientsLieu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_RecherchePatientsLieu))
        pnlForm = New Panel()
        carteResultats = New AltheaCarte()
        dgvPatients = New DataGridView()
        colCode = New DataGridViewTextBoxColumn()
        colNom = New DataGridViewTextBoxColumn()
        colPrenom = New DataGridViewTextBoxColumn()
        colCodePostal = New DataGridViewTextBoxColumn()
        colLocalite = New DataGridViewTextBoxColumn()
        colSuivi = New DataGridViewTextBoxColumn()
        colRendezVous = New DataGridViewTextBoxColumn()
        colTelephone = New DataGridViewTextBoxColumn()
        pnlEspace = New Panel()
        carteLieux = New AltheaCarte()
        dgvLieux = New DataGridView()
        colLieu = New DataGridViewTextBoxColumn()
        colNbPatients = New DataGridViewTextBoxColumn()
        carteCriteres = New AltheaCarte()
        txtLieu = New AltheaRecherche()
        chkAfficherInactifs = New AltheaCase()
        lblMessage = New Label()
        pnlActions = New AltheaBarreActions()
        btnRetour = New AltheaBouton()
        btnOuvrir = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        carteResultats.SuspendLayout()
        CType(dgvPatients, ComponentModel.ISupportInitialize).BeginInit()
        carteLieux.SuspendLayout()
        CType(dgvLieux, ComponentModel.ISupportInitialize).BeginInit()
        carteCriteres.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        '
        ' pnlForm
        '
        pnlForm.Controls.Add(carteResultats)
        pnlForm.Controls.Add(pnlEspace)
        pnlForm.Controls.Add(carteLieux)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(carteCriteres)
        pnlForm.Controls.Add(enTete)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16, 16, 16, 16)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 0
        '
        ' carteResultats
        '
        carteResultats.Controls.Add(dgvPatients)
        carteResultats.Dock = DockStyle.Fill
        carteResultats.Location = New Point(358, 170)
        carteResultats.Name = "carteResultats"
        carteResultats.Padding = New Padding(6, 2, 6, 6)
        carteResultats.Size = New Size(623, 533)
        carteResultats.TabIndex = 3
        carteResultats.Titre = "Patients"
        '
        ' dgvPatients
        '
        dgvPatients.AllowUserToAddRows = False
        dgvPatients.AllowUserToDeleteRows = False
        dgvPatients.AllowUserToResizeRows = False
        dgvPatients.AutoGenerateColumns = False
        dgvPatients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvPatients.Columns.AddRange(New DataGridViewColumn() {colCode, colNom, colPrenom, colCodePostal, colLocalite, colSuivi, colRendezVous, colTelephone})
        dgvPatients.Dock = DockStyle.Fill
        dgvPatients.Location = New Point(6, 36)
        dgvPatients.MultiSelect = False
        dgvPatients.Name = "dgvPatients"
        dgvPatients.ReadOnly = True
        dgvPatients.RowHeadersVisible = False
        dgvPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPatients.Size = New Size(611, 491)
        dgvPatients.TabIndex = 0
        '
        ' colCode
        '
        colCode.DataPropertyName = "CodePatient"
        colCode.HeaderText = "Code"
        colCode.Name = "colCode"
        colCode.ReadOnly = True
        colCode.Width = 85
        '
        ' colNom
        '
        colNom.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colNom.DataPropertyName = "Nom"
        colNom.HeaderText = "Nom"
        colNom.MinimumWidth = 120
        colNom.Name = "colNom"
        colNom.ReadOnly = True
        '
        ' colPrenom
        '
        colPrenom.DataPropertyName = "Prenom"
        colPrenom.HeaderText = "Prénom"
        colPrenom.Name = "colPrenom"
        colPrenom.ReadOnly = True
        colPrenom.Width = 120
        '
        ' colCodePostal
        '
        colCodePostal.DataPropertyName = "CodePostal"
        colCodePostal.HeaderText = "CP"
        colCodePostal.Name = "colCodePostal"
        colCodePostal.ReadOnly = True
        colCodePostal.Width = 60
        '
        ' colLocalite
        '
        colLocalite.DataPropertyName = "Localite"
        colLocalite.HeaderText = "Localité"
        colLocalite.Name = "colLocalite"
        colLocalite.ReadOnly = True
        colLocalite.Width = 130
        '
        ' colSuivi
        '
        colSuivi.DataPropertyName = "SuiviAffichage"
        colSuivi.HeaderText = "Suivi"
        colSuivi.Name = "colSuivi"
        colSuivi.ReadOnly = True
        colSuivi.Width = 130
        '
        ' colRendezVous
        '
        colRendezVous.DataPropertyName = "RendezVousAffichage"
        colRendezVous.HeaderText = "Prochain RDV"
        colRendezVous.Name = "colRendezVous"
        colRendezVous.ReadOnly = True
        colRendezVous.Width = 130
        '
        ' colTelephone
        '
        colTelephone.DataPropertyName = "Telephone"
        colTelephone.HeaderText = "Téléphone"
        colTelephone.Name = "colTelephone"
        colTelephone.ReadOnly = True
        colTelephone.Width = 115
        '
        ' pnlEspace
        '
        pnlEspace.BackColor = Color.Transparent
        pnlEspace.Dock = DockStyle.Left
        pnlEspace.Location = New Point(346, 170)
        pnlEspace.Name = "pnlEspace"
        pnlEspace.Size = New Size(12, 533)
        pnlEspace.TabIndex = 2
        '
        ' carteLieux
        '
        carteLieux.Controls.Add(dgvLieux)
        carteLieux.Dock = DockStyle.Left
        carteLieux.Location = New Point(16, 170)
        carteLieux.Name = "carteLieux"
        carteLieux.Padding = New Padding(6, 2, 6, 6)
        carteLieux.Size = New Size(330, 533)
        carteLieux.TabIndex = 2
        carteLieux.Titre = "Lieux"
        '
        ' dgvLieux
        '
        dgvLieux.AllowUserToAddRows = False
        dgvLieux.AllowUserToDeleteRows = False
        dgvLieux.AllowUserToResizeRows = False
        dgvLieux.AutoGenerateColumns = False
        dgvLieux.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvLieux.Columns.AddRange(New DataGridViewColumn() {colLieu, colNbPatients})
        dgvLieux.Dock = DockStyle.Fill
        dgvLieux.Location = New Point(6, 36)
        dgvLieux.MultiSelect = False
        dgvLieux.Name = "dgvLieux"
        dgvLieux.ReadOnly = True
        dgvLieux.RowHeadersVisible = False
        dgvLieux.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvLieux.Size = New Size(318, 491)
        dgvLieux.TabIndex = 0
        '
        ' colLieu
        '
        colLieu.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colLieu.DataPropertyName = "Libelle"
        colLieu.HeaderText = "Lieu"
        colLieu.Name = "colLieu"
        colLieu.ReadOnly = True
        colLieu.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        ' colNbPatients
        '
        colNbPatients.DataPropertyName = "NbPatients"
        colNbPatients.HeaderText = "Patients"
        colNbPatients.Name = "colNbPatients"
        colNbPatients.ReadOnly = True
        colNbPatients.SortMode = DataGridViewColumnSortMode.NotSortable
        colNbPatients.Width = 75
        '
        ' carteCriteres
        '
        carteCriteres.Controls.Add(txtLieu)
        carteCriteres.Controls.Add(chkAfficherInactifs)
        carteCriteres.Controls.Add(lblMessage)
        carteCriteres.Dock = DockStyle.Top
        carteCriteres.Location = New Point(16, 74)
        carteCriteres.Name = "carteCriteres"
        carteCriteres.Padding = New Padding(12, 8, 12, 10)
        carteCriteres.Size = New Size(965, 84)
        carteCriteres.TabIndex = 1
        carteCriteres.Titre = "Lieu"
        '
        ' txtLieu
        '
        txtLieu.Location = New Point(16, 40)
        txtLieu.MaxLength = 50
        txtLieu.Name = "txtLieu"
        txtLieu.Size = New Size(320, 32)
        txtLieu.TabIndex = 0
        txtLieu.TexteAide = "Code postal ou localité…"
        '
        ' chkAfficherInactifs
        '
        chkAfficherInactifs.AutoSize = False
        chkAfficherInactifs.Location = New Point(352, 40)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(180, 32)
        chkAfficherInactifs.TabIndex = 1
        chkAfficherInactifs.Text = "Afficher les inactifs"
        '
        ' lblMessage
        '
        lblMessage.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblMessage.BackColor = Color.Transparent
        lblMessage.Font = New Font("Calibri", 10.5F, FontStyle.Italic)
        lblMessage.ForeColor = Color.FromArgb(CByte(110), CByte(110), CByte(110))
        lblMessage.Location = New Point(548, 46)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(400, 22)
        lblMessage.TabIndex = 2
        '
        ' pnlActions
        '
        pnlActions.Controls.Add(btnRetour)
        pnlActions.Controls.Add(btnOuvrir)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(16, 703)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(965, 52)
        pnlActions.TabIndex = 4
        '
        ' btnRetour
        '
        btnRetour.Image = CType(resources.GetObject("btnRetour.Image"), Image)
        btnRetour.Location = New Point(16, 7)
        btnRetour.Name = "btnRetour"
        btnRetour.Size = New Size(190, 38)
        btnRetour.TabIndex = 0
        btnRetour.Text = "Retour à la recherche"
        btnRetour.Variante = VarianteBouton.Clair
        '
        ' btnOuvrir
        '
        btnOuvrir.Location = New Point(222, 7)
        btnOuvrir.Name = "btnOuvrir"
        btnOuvrir.Size = New Size(150, 38)
        btnOuvrir.TabIndex = 1
        btnOuvrir.Text = "Ouvrir la fiche"
        btnOuvrir.Variante = VarianteBouton.OutilPrincipal
        '
        ' enTete
        '
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 0
        enTete.SousTitre = "Les patients d'un code postal ou d'une localité, et l'accès à leur fiche"
        enTete.Text = "Patients par lieu"
        '
        ' UC_RecherchePatientsLieu
        '
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_RecherchePatientsLieu"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        carteResultats.ResumeLayout(False)
        CType(dgvPatients, ComponentModel.ISupportInitialize).EndInit()
        carteLieux.ResumeLayout(False)
        CType(dgvLieux, ComponentModel.ISupportInitialize).EndInit()
        carteCriteres.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteCriteres As AltheaCarte
    Friend WithEvents txtLieu As AltheaRecherche
    Friend WithEvents chkAfficherInactifs As AltheaCase
    Friend WithEvents lblMessage As Label
    Friend WithEvents carteLieux As AltheaCarte
    Friend WithEvents dgvLieux As DataGridView
    Friend WithEvents colLieu As DataGridViewTextBoxColumn
    Friend WithEvents colNbPatients As DataGridViewTextBoxColumn
    Friend WithEvents pnlEspace As Panel
    Friend WithEvents carteResultats As AltheaCarte
    Friend WithEvents dgvPatients As DataGridView
    Friend WithEvents colCode As DataGridViewTextBoxColumn
    Friend WithEvents colNom As DataGridViewTextBoxColumn
    Friend WithEvents colPrenom As DataGridViewTextBoxColumn
    Friend WithEvents colCodePostal As DataGridViewTextBoxColumn
    Friend WithEvents colLocalite As DataGridViewTextBoxColumn
    Friend WithEvents colSuivi As DataGridViewTextBoxColumn
    Friend WithEvents colRendezVous As DataGridViewTextBoxColumn
    Friend WithEvents colTelephone As DataGridViewTextBoxColumn
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnRetour As AltheaBouton
    Friend WithEvents btnOuvrir As AltheaBouton

End Class
