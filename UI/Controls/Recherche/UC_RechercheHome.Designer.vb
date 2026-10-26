' -------------------------------------------------------------------------------------------------
' Designer    : UC_RechercheHome
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Structure visuelle du hub Recherche (ADR-52) : en-tête, carte « Recherche rapide par code »
' (saisie, résultats, ouverture) et carte « Autres recherches » (tuiles, ajoutées étape par étape).
' - V1.1.0 (23/10/2026, R2) : tuile « Séances » (UC_RechercheSeances).
' - V1.2.0 (23/10/2026, R3) : tuile « Paiements » (UC_RecherchePaiements).
' - V1.3.0 (24/10/2026, R4) : tuile « Patients par lieu » (UC_RecherchePatientsLieu).
' - V1.4.0 (24/10/2026, R5) : tuile « Réseau par nom » (UC_RechercheReseau).
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_RechercheHome
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_RechercheHome))
        pnlForm = New Panel()
        carteAutres = New AltheaCarte()
        btnSeances = New AltheaTuile()
        btnPaiements = New AltheaTuile()
        btnPatientsLieu = New AltheaTuile()
        btnReseau = New AltheaTuile()
        lblAutres = New Label()
        carteRapide = New AltheaCarte()
        lblAide = New Label()
        txtCode = New AltheaRecherche()
        btnChercher = New AltheaBouton()
        lblMessage = New Label()
        dgvResultats = New DataGridView()
        colNature = New DataGridViewTextBoxColumn()
        colCode = New DataGridViewTextBoxColumn()
        colDescription = New DataGridViewTextBoxColumn()
        colPatient = New DataGridViewTextBoxColumn()
        btnOuvrir = New AltheaBouton()
        btnOuvrirPatient = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        carteAutres.SuspendLayout()
        carteRapide.SuspendLayout()
        CType(dgvResultats, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.Controls.Add(carteAutres)
        pnlForm.Controls.Add(carteRapide)
        pnlForm.Controls.Add(enTete)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16, 16, 16, 16)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 0
        ' 
        ' carteAutres
        ' 
        carteAutres.Controls.Add(btnSeances)
        carteAutres.Controls.Add(btnPaiements)
        carteAutres.Controls.Add(btnPatientsLieu)
        carteAutres.Controls.Add(btnReseau)
        carteAutres.Controls.Add(lblAutres)
        carteAutres.Dock = DockStyle.Fill
        carteAutres.Location = New Point(16, 396)
        carteAutres.Name = "carteAutres"
        carteAutres.Padding = New Padding(12, 8, 12, 10)
        carteAutres.Size = New Size(965, 359)
        carteAutres.TabIndex = 2
        carteAutres.Titre = "Autres recherches"
        ' 
        ' btnSeances
        ' 
        btnSeances.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnSeances.Location = New Point(16, 44)
        btnSeances.Name = "btnSeances"
        btnSeances.Rayon = 12
        btnSeances.Size = New Size(250, 84)
        btnSeances.TabIndex = 0
        btnSeances.Tag = "rechercheSeances_normal"
        btnSeances.Text = "Séances" & vbCrLf & "D'un jour ou d'une période"
        btnSeances.TextAlign = ContentAlignment.MiddleLeft
        btnSeances.TextImageRelation = TextImageRelation.ImageBeforeText
        ' 
        ' btnPaiements
        ' 
        btnPaiements.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnPaiements.Location = New Point(282, 44)
        btnPaiements.Name = "btnPaiements"
        btnPaiements.Rayon = 12
        btnPaiements.Size = New Size(250, 84)
        btnPaiements.TabIndex = 1
        btnPaiements.Tag = "recherchePaiements_normal"
        btnPaiements.Text = "Paiements" & vbCrLf & "D'un jour ou d'une période"
        btnPaiements.TextAlign = ContentAlignment.MiddleLeft
        btnPaiements.TextImageRelation = TextImageRelation.ImageBeforeText
        ' 
        ' btnPatientsLieu
        ' 
        btnPatientsLieu.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnPatientsLieu.Location = New Point(548, 44)
        btnPatientsLieu.Name = "btnPatientsLieu"
        btnPatientsLieu.Rayon = 12
        btnPatientsLieu.Size = New Size(250, 84)
        btnPatientsLieu.TabIndex = 2
        btnPatientsLieu.Tag = "rechercheLieu_normal"
        btnPatientsLieu.Text = "Patients par lieu" & vbCrLf & "Code postal ou localité"
        btnPatientsLieu.TextAlign = ContentAlignment.MiddleLeft
        btnPatientsLieu.TextImageRelation = TextImageRelation.ImageBeforeText
        ' 
        ' btnReseau
        ' 
        btnReseau.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnReseau.Location = New Point(16, 140)
        btnReseau.Name = "btnReseau"
        btnReseau.Rayon = 12
        btnReseau.Size = New Size(250, 84)
        btnReseau.TabIndex = 3
        btnReseau.Tag = "rechercheReseau_normal"
        btnReseau.Text = "Réseau par nom" & vbCrLf & "Thérapeute, intervenant, contact"
        btnReseau.TextAlign = ContentAlignment.MiddleLeft
        btnReseau.TextImageRelation = TextImageRelation.ImageBeforeText
        ' 
        ' lblAutres
        ' 
        lblAutres.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblAutres.Font = New Font("Calibri", 11F, FontStyle.Italic)
        lblAutres.ForeColor = Color.FromArgb(CByte(110), CByte(110), CByte(110))
        lblAutres.Location = New Point(16, 236)
        lblAutres.Name = "lblAutres"
        lblAutres.Size = New Size(930, 44)
        lblAutres.TabIndex = 4
        lblAutres.Text = "D'autres recherches viendront au fil des besoins (documents, rendez-vous…)."
        ' 
        ' carteRapide
        ' 
        carteRapide.Controls.Add(lblAide)
        carteRapide.Controls.Add(txtCode)
        carteRapide.Controls.Add(btnChercher)
        carteRapide.Controls.Add(lblMessage)
        carteRapide.Controls.Add(dgvResultats)
        carteRapide.Controls.Add(btnOuvrir)
        carteRapide.Controls.Add(btnOuvrirPatient)
        carteRapide.Dock = DockStyle.Top
        carteRapide.Location = New Point(16, 74)
        carteRapide.Name = "carteRapide"
        carteRapide.Padding = New Padding(12, 8, 12, 10)
        carteRapide.Size = New Size(965, 322)
        carteRapide.TabIndex = 1
        carteRapide.Titre = "Recherche rapide par code"
        ' 
        ' lblAide
        ' 
        lblAide.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblAide.Font = New Font("Calibri", 10.5F)
        lblAide.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblAide.Location = New Point(16, 40)
        lblAide.Name = "lblAide"
        lblAide.Size = New Size(930, 22)
        lblAide.TabIndex = 0
        lblAide.Text = "Tapez un code (par exemple SE83 ou SE000083), puis Entrée."
        ' 
        ' txtCode
        ' 
        txtCode.Location = New Point(16, 67)
        txtCode.MaxLength = 20
        txtCode.Name = "txtCode"
        txtCode.Size = New Size(240, 32)
        txtCode.TabIndex = 1
        txtCode.TexteAide = "Code : SE83, PA000005…"
        ' 
        ' btnChercher
        ' 
        btnChercher.Location = New Point(268, 67)
        btnChercher.Name = "btnChercher"
        btnChercher.Size = New Size(130, 32)
        btnChercher.TabIndex = 2
        btnChercher.Text = "Chercher"
        ' 
        ' lblMessage
        ' 
        lblMessage.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblMessage.Font = New Font("Calibri", 10.5F, FontStyle.Italic)
        lblMessage.ForeColor = Color.FromArgb(CByte(110), CByte(110), CByte(110))
        lblMessage.Location = New Point(412, 73)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(534, 22)
        lblMessage.TabIndex = 3
        ' 
        ' dgvResultats
        ' 
        dgvResultats.AllowUserToAddRows = False
        dgvResultats.AllowUserToDeleteRows = False
        dgvResultats.AllowUserToResizeRows = False
        dgvResultats.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        dgvResultats.AutoGenerateColumns = False
        dgvResultats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvResultats.Columns.AddRange(New DataGridViewColumn() {colNature, colCode, colDescription, colPatient})
        dgvResultats.Location = New Point(16, 112)
        dgvResultats.MultiSelect = False
        dgvResultats.Name = "dgvResultats"
        dgvResultats.ReadOnly = True
        dgvResultats.RowHeadersVisible = False
        dgvResultats.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvResultats.Size = New Size(930, 150)
        dgvResultats.TabIndex = 4
        ' 
        ' colNature
        ' 
        colNature.DataPropertyName = "LibelleNature"
        colNature.HeaderText = "Nature"
        colNature.Name = "colNature"
        colNature.ReadOnly = True
        colNature.Width = 100
        ' 
        ' colCode
        ' 
        colCode.DataPropertyName = "Code"
        colCode.HeaderText = "Code"
        colCode.Name = "colCode"
        colCode.ReadOnly = True
        colCode.Width = 110
        ' 
        ' colDescription
        ' 
        colDescription.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colDescription.DataPropertyName = "Description"
        colDescription.HeaderText = "Description"
        colDescription.Name = "colDescription"
        colDescription.ReadOnly = True
        ' 
        ' colPatient
        ' 
        colPatient.DataPropertyName = "PatientAffichage"
        colPatient.HeaderText = "Patient"
        colPatient.Name = "colPatient"
        colPatient.ReadOnly = True
        colPatient.Width = 260
        ' 
        ' btnOuvrir
        ' 
        btnOuvrir.Location = New Point(16, 274)
        btnOuvrir.Name = "btnOuvrir"
        btnOuvrir.Size = New Size(130, 32)
        btnOuvrir.TabIndex = 5
        btnOuvrir.Text = "Ouvrir"
        btnOuvrir.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' btnOuvrirPatient
        ' 
        btnOuvrirPatient.Location = New Point(156, 274)
        btnOuvrirPatient.Name = "btnOuvrirPatient"
        btnOuvrirPatient.Size = New Size(170, 32)
        btnOuvrirPatient.TabIndex = 6
        btnOuvrirPatient.Text = "Ouvrir le patient"
        btnOuvrirPatient.Variante = VarianteBouton.Outil
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 0
        enTete.SousTitre = "Retrouvez un élément et allez-y directement"
        enTete.Text = "Recherche"
        ' 
        ' UC_RechercheHome
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_RechercheHome"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        carteAutres.ResumeLayout(False)
        carteRapide.ResumeLayout(False)
        carteRapide.PerformLayout()
        CType(dgvResultats, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteRapide As AltheaCarte
    Friend WithEvents lblAide As Label
    Friend WithEvents txtCode As AltheaRecherche
    Friend WithEvents btnChercher As AltheaBouton
    Friend WithEvents lblMessage As Label
    Friend WithEvents dgvResultats As DataGridView
    Friend WithEvents colNature As DataGridViewTextBoxColumn
    Friend WithEvents colCode As DataGridViewTextBoxColumn
    Friend WithEvents colDescription As DataGridViewTextBoxColumn
    Friend WithEvents colPatient As DataGridViewTextBoxColumn
    Friend WithEvents btnOuvrir As AltheaBouton
    Friend WithEvents btnOuvrirPatient As AltheaBouton
    Friend WithEvents carteAutres As AltheaCarte
    Friend WithEvents btnSeances As AltheaTuile
    Friend WithEvents btnPaiements As AltheaTuile
    Friend WithEvents btnPatientsLieu As AltheaTuile
    Friend WithEvents btnReseau As AltheaTuile
    Friend WithEvents lblAutres As Label

End Class
