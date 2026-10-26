' -------------------------------------------------------------------------------------------------
' Designer    : UC_RechercheReseau
' Projet      : Althéa
' Version     : V1.1.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Structure visuelle de la recherche du réseau par nom (ADR-52, R5) : en-tête, carte « Nom » (zone,
' case des inactifs), carte « Personnes » à gauche, carte « Patients » (liens de la personne) et
' barre d'actions (retour, ouvrir le patient, fiche de la personne).
' - V1.1.0 (24/10/2026) : colonne Code (TH…) en tête de la liste des personnes.
' - V1.1.1 (25/10/2026, relevé aux captures) : carte « Personnes » élargie (580) : depuis la colonne
'   Code, la colonne « Patients » était coupée et une barre de défilement horizontale apparaissait.
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_RechercheReseau
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_RechercheReseau))
        pnlForm = New Panel()
        carteResultats = New AltheaCarte()
        dgvLiens = New DataGridView()
        colCodePatient = New DataGridViewTextBoxColumn()
        colPatient = New DataGridViewTextBoxColumn()
        colRole = New DataGridViewTextBoxColumn()
        colDepuis = New DataGridViewTextBoxColumn()
        colEtat = New DataGridViewTextBoxColumn()
        colFiche = New DataGridViewTextBoxColumn()
        pnlEspace = New Panel()
        cartePersonnes = New AltheaCarte()
        dgvPersonnes = New DataGridView()
        colCodePersonne = New DataGridViewTextBoxColumn()
        colPersonne = New DataGridViewTextBoxColumn()
        colNature = New DataGridViewTextBoxColumn()
        colPrecision = New DataGridViewTextBoxColumn()
        colNbPatients = New DataGridViewTextBoxColumn()
        carteCriteres = New AltheaCarte()
        txtNom = New AltheaRecherche()
        chkAfficherInactifs = New AltheaCase()
        lblMessage = New Label()
        pnlActions = New AltheaBarreActions()
        btnRetour = New AltheaBouton()
        btnOuvrir = New AltheaBouton()
        btnFichePersonne = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        carteResultats.SuspendLayout()
        CType(dgvLiens, ComponentModel.ISupportInitialize).BeginInit()
        cartePersonnes.SuspendLayout()
        CType(dgvPersonnes, ComponentModel.ISupportInitialize).BeginInit()
        carteCriteres.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        '
        ' pnlForm
        '
        pnlForm.Controls.Add(carteResultats)
        pnlForm.Controls.Add(pnlEspace)
        pnlForm.Controls.Add(cartePersonnes)
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
        carteResultats.Controls.Add(dgvLiens)
        carteResultats.Dock = DockStyle.Fill
        carteResultats.Location = New Point(608, 170)
        carteResultats.Name = "carteResultats"
        carteResultats.Padding = New Padding(6, 2, 6, 6)
        carteResultats.Size = New Size(373, 533)
        carteResultats.TabIndex = 3
        carteResultats.Titre = "Patients"
        '
        ' dgvLiens
        '
        dgvLiens.AllowUserToAddRows = False
        dgvLiens.AllowUserToDeleteRows = False
        dgvLiens.AllowUserToResizeRows = False
        dgvLiens.AutoGenerateColumns = False
        dgvLiens.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvLiens.Columns.AddRange(New DataGridViewColumn() {colCodePatient, colPatient, colRole, colDepuis, colEtat, colFiche})
        dgvLiens.Dock = DockStyle.Fill
        dgvLiens.Location = New Point(6, 36)
        dgvLiens.MultiSelect = False
        dgvLiens.Name = "dgvLiens"
        dgvLiens.ReadOnly = True
        dgvLiens.RowHeadersVisible = False
        dgvLiens.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvLiens.Size = New Size(421, 491)
        dgvLiens.TabIndex = 0
        '
        ' colCodePatient
        '
        colCodePatient.DataPropertyName = "CodePatient"
        colCodePatient.HeaderText = "Code"
        colCodePatient.Name = "colCodePatient"
        colCodePatient.ReadOnly = True
        colCodePatient.Width = 85
        '
        ' colPatient
        '
        colPatient.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colPatient.DataPropertyName = "PatientAffichage"
        colPatient.HeaderText = "Patient"
        colPatient.MinimumWidth = 140
        colPatient.Name = "colPatient"
        colPatient.ReadOnly = True
        '
        ' colRole
        '
        colRole.DataPropertyName = "Role"
        colRole.HeaderText = "Lien / rôle"
        colRole.Name = "colRole"
        colRole.ReadOnly = True
        colRole.Width = 130
        '
        ' colDepuis
        '
        colDepuis.DataPropertyName = "DateDebut"
        colDepuis.HeaderText = "Depuis"
        colDepuis.Name = "colDepuis"
        colDepuis.ReadOnly = True
        colDepuis.Width = 90
        '
        ' colEtat
        '
        colEtat.DataPropertyName = "EtatAffichage"
        colEtat.HeaderText = "Suivi"
        colEtat.Name = "colEtat"
        colEtat.ReadOnly = True
        colEtat.Width = 140
        '
        ' colFiche
        '
        colFiche.DataPropertyName = "CodeElement"
        colFiche.HeaderText = "Fiche"
        colFiche.Name = "colFiche"
        colFiche.ReadOnly = True
        colFiche.Width = 85
        '
        ' pnlEspace
        '
        pnlEspace.BackColor = Color.Transparent
        pnlEspace.Dock = DockStyle.Left
        pnlEspace.Location = New Point(536, 170)
        pnlEspace.Name = "pnlEspace"
        pnlEspace.Size = New Size(12, 533)
        pnlEspace.TabIndex = 2
        '
        ' cartePersonnes
        '
        cartePersonnes.Controls.Add(dgvPersonnes)
        cartePersonnes.Dock = DockStyle.Left
        cartePersonnes.Location = New Point(16, 170)
        cartePersonnes.Name = "cartePersonnes"
        cartePersonnes.Padding = New Padding(6, 2, 6, 6)
        cartePersonnes.Size = New Size(580, 533)
        cartePersonnes.TabIndex = 2
        cartePersonnes.Titre = "Personnes"
        '
        ' dgvPersonnes
        '
        dgvPersonnes.AllowUserToAddRows = False
        dgvPersonnes.AllowUserToDeleteRows = False
        dgvPersonnes.AllowUserToResizeRows = False
        dgvPersonnes.AutoGenerateColumns = False
        dgvPersonnes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvPersonnes.Columns.AddRange(New DataGridViewColumn() {colCodePersonne, colPersonne, colNature, colPrecision, colNbPatients})
        dgvPersonnes.Dock = DockStyle.Fill
        dgvPersonnes.Location = New Point(6, 36)
        dgvPersonnes.MultiSelect = False
        dgvPersonnes.Name = "dgvPersonnes"
        dgvPersonnes.ReadOnly = True
        dgvPersonnes.RowHeadersVisible = False
        dgvPersonnes.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPersonnes.Size = New Size(568, 491)
        dgvPersonnes.TabIndex = 0
        '
        ' colCodePersonne
        '
        colCodePersonne.DataPropertyName = "CodeTherapeute"
        colCodePersonne.HeaderText = "Code"
        colCodePersonne.Name = "colCodePersonne"
        colCodePersonne.ReadOnly = True
        colCodePersonne.SortMode = DataGridViewColumnSortMode.NotSortable
        colCodePersonne.Width = 85
        '
        ' colPersonne
        '
        colPersonne.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colPersonne.DataPropertyName = "NomComplet"
        colPersonne.HeaderText = "Personne"
        colPersonne.MinimumWidth = 140
        colPersonne.Name = "colPersonne"
        colPersonne.ReadOnly = True
        colPersonne.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        ' colNature
        '
        colNature.DataPropertyName = "NatureAffichage"
        colNature.HeaderText = "Nature"
        colNature.Name = "colNature"
        colNature.ReadOnly = True
        colNature.SortMode = DataGridViewColumnSortMode.NotSortable
        colNature.Width = 115
        '
        ' colPrecision
        '
        colPrecision.DataPropertyName = "Precision"
        colPrecision.HeaderText = "Spécialité / lien"
        colPrecision.Name = "colPrecision"
        colPrecision.ReadOnly = True
        colPrecision.SortMode = DataGridViewColumnSortMode.NotSortable
        colPrecision.Width = 130
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
        carteCriteres.Controls.Add(txtNom)
        carteCriteres.Controls.Add(chkAfficherInactifs)
        carteCriteres.Controls.Add(lblMessage)
        carteCriteres.Dock = DockStyle.Top
        carteCriteres.Location = New Point(16, 74)
        carteCriteres.Name = "carteCriteres"
        carteCriteres.Padding = New Padding(12, 8, 12, 10)
        carteCriteres.Size = New Size(965, 84)
        carteCriteres.TabIndex = 1
        carteCriteres.Titre = "Nom"
        '
        ' txtNom
        '
        txtNom.Location = New Point(16, 40)
        txtNom.MaxLength = 50
        txtNom.Name = "txtNom"
        txtNom.Size = New Size(320, 32)
        txtNom.TabIndex = 0
        txtNom.TexteAide = "Nom ou début du nom…"
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
        pnlActions.Controls.Add(btnFichePersonne)
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
        btnOuvrir.Size = New Size(170, 38)
        btnOuvrir.TabIndex = 1
        btnOuvrir.Text = "Ouvrir le patient"
        btnOuvrir.Variante = VarianteBouton.OutilPrincipal
        '
        ' btnFichePersonne
        '
        btnFichePersonne.Location = New Point(402, 7)
        btnFichePersonne.Name = "btnFichePersonne"
        btnFichePersonne.Size = New Size(190, 38)
        btnFichePersonne.TabIndex = 2
        btnFichePersonne.Text = "Fiche de la personne"
        btnFichePersonne.Variante = VarianteBouton.Outil
        '
        ' enTete
        '
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 0
        enTete.SousTitre = "Un thérapeute, un intervenant ou un contact par son nom, et les patients qui en relèvent"
        enTete.Text = "Réseau par nom"
        '
        ' UC_RechercheReseau
        '
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_RechercheReseau"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        carteResultats.ResumeLayout(False)
        CType(dgvLiens, ComponentModel.ISupportInitialize).EndInit()
        cartePersonnes.ResumeLayout(False)
        CType(dgvPersonnes, ComponentModel.ISupportInitialize).EndInit()
        carteCriteres.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteCriteres As AltheaCarte
    Friend WithEvents txtNom As AltheaRecherche
    Friend WithEvents chkAfficherInactifs As AltheaCase
    Friend WithEvents lblMessage As Label
    Friend WithEvents cartePersonnes As AltheaCarte
    Friend WithEvents dgvPersonnes As DataGridView
    Friend WithEvents colCodePersonne As DataGridViewTextBoxColumn
    Friend WithEvents colPersonne As DataGridViewTextBoxColumn
    Friend WithEvents colNature As DataGridViewTextBoxColumn
    Friend WithEvents colPrecision As DataGridViewTextBoxColumn
    Friend WithEvents colNbPatients As DataGridViewTextBoxColumn
    Friend WithEvents pnlEspace As Panel
    Friend WithEvents carteResultats As AltheaCarte
    Friend WithEvents dgvLiens As DataGridView
    Friend WithEvents colCodePatient As DataGridViewTextBoxColumn
    Friend WithEvents colPatient As DataGridViewTextBoxColumn
    Friend WithEvents colRole As DataGridViewTextBoxColumn
    Friend WithEvents colDepuis As DataGridViewTextBoxColumn
    Friend WithEvents colEtat As DataGridViewTextBoxColumn
    Friend WithEvents colFiche As DataGridViewTextBoxColumn
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnRetour As AltheaBouton
    Friend WithEvents btnOuvrir As AltheaBouton
    Friend WithEvents btnFichePersonne As AltheaBouton

End Class
