' -------------------------------------------------------------------------------------------------
' Designer    : UC_ReferentielBase
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 16/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Définit la structure visuelle commune à tous les écrans de gestion de référentiels (tables ref_*).
' Reprend la charte Althéa de UC_Utilisateurs : titre + grille + barre d'actions.
' Ajoute un panneau d'édition générique (Code, Libellé, Ordre, Actif) à droite de la grille.
'
' Remarques   :
' - Ce UserControl est destiné à être hérité (UC_Pays, UC_NaturesSeance, etc.).
' - Les contrôles sont déclarés Friend WithEvents pour être accessibles aux classes dérivées.
' - V1.1.0 (16/10/2026) : panneau « Détail » à la règle des fenêtres (libellé à gauche, champ à
'   x = 112, une ligne tous les 34 px à partir de y = 40, fond blanc) ; État en AltheaCase.
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_ReferentielBase
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_ReferentielBase))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        dgvReferentiel = New DataGridView()
        colId = New DataGridViewTextBoxColumn()
        colCode = New DataGridViewTextBoxColumn()
        colLibelle = New DataGridViewTextBoxColumn()
        colOrdre = New DataGridViewTextBoxColumn()
        colActif = New DataGridViewTextBoxColumn()
        pagReferentiel = New UC_Pagination()
        pnlEdition = New AltheaCarte()
        chkActif = New AltheaCase()
        lblActif = New Label()
        numOrdre = New NumericUpDown()
        lblOrdre = New Label()
        txtLibelle = New TextBox()
        lblLibelle = New Label()
        txtCode = New TextBox()
        lblCode = New Label()
        pnlTop = New Panel()
        carteRecherche = New AltheaCarte()
        chkAfficherInactifs = New AltheaCase()
        txtRecherche = New AltheaRecherche()
        pnlActions = New AltheaBarreActions()
        btnEnregistrer = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        btnActiverDesactiver = New AltheaBouton()
        btnActualiser = New AltheaBouton()
        btnNouveau = New AltheaBouton()
        btnModifier = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        CType(dgvReferentiel, ComponentModel.ISupportInitialize).BeginInit()
        pnlEdition.SuspendLayout()
        CType(numOrdre, ComponentModel.ISupportInitialize).BeginInit()
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
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 0
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(dgvReferentiel)
        pnlCenter.Controls.Add(pagReferentiel)
        pnlCenter.Controls.Add(pnlEdition)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(965, 624)
        pnlCenter.TabIndex = 2
        ' 
        ' dgvReferentiel
        ' 
        dgvReferentiel.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvReferentiel.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvReferentiel.Columns.AddRange(New DataGridViewColumn() {colId, colCode, colLibelle, colOrdre, colActif})
        dgvReferentiel.Dock = DockStyle.Left
        dgvReferentiel.Location = New Point(16, 80)
        dgvReferentiel.Name = "dgvReferentiel"
        dgvReferentiel.ReadOnly = True
        dgvReferentiel.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvReferentiel.Size = New Size(523, 504)
        dgvReferentiel.TabIndex = 1
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
        colCode.Width = 160
        ' 
        ' colLibelle
        ' 
        colLibelle.HeaderText = "Libellé"
        colLibelle.Name = "colLibelle"
        colLibelle.ReadOnly = True
        colLibelle.Width = 280
        ' 
        ' colOrdre
        ' 
        colOrdre.HeaderText = "Ordre"
        colOrdre.Name = "colOrdre"
        colOrdre.ReadOnly = True
        colOrdre.Width = 60
        ' 
        ' colActif
        ' 
        colActif.HeaderText = "État"
        colActif.Name = "colActif"
        colActif.ReadOnly = True
        colActif.Width = 90
        ' 
        ' pagReferentiel
        ' 
        pagReferentiel.BackColor = Color.Transparent
        pagReferentiel.Dock = DockStyle.Bottom
        pagReferentiel.Location = New Point(16, 584)
        pagReferentiel.Name = "pagReferentiel"
        pagReferentiel.Size = New Size(933, 32)
        pagReferentiel.TabIndex = 2
        ' 
        ' pnlEdition
        ' 
        pnlEdition.Controls.Add(chkActif)
        pnlEdition.Controls.Add(lblActif)
        pnlEdition.Controls.Add(numOrdre)
        pnlEdition.Controls.Add(lblOrdre)
        pnlEdition.Controls.Add(txtLibelle)
        pnlEdition.Controls.Add(lblLibelle)
        pnlEdition.Controls.Add(txtCode)
        pnlEdition.Controls.Add(lblCode)
        pnlEdition.Location = New Point(539, 80)
        pnlEdition.Name = "pnlEdition"
        pnlEdition.Padding = New Padding(12, 8, 12, 10)
        pnlEdition.Size = New Size(410, 536)
        pnlEdition.TabIndex = 2
        pnlEdition.TabStop = False
        pnlEdition.Titre = "Détail"
        ' 
        ' chkActif
        ' 
        chkActif.AutoSize = True
        chkActif.BackColor = Color.Transparent
        chkActif.Font = New Font("Calibri", 11F)
        chkActif.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        chkActif.Location = New Point(112, 143)
        chkActif.Name = "chkActif"
        chkActif.Size = New Size(56, 22)
        chkActif.TabIndex = 8
        chkActif.Text = "Actif"
        ' 
        ' lblActif
        ' 
        lblActif.AutoSize = True
        lblActif.BackColor = Color.Transparent
        lblActif.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblActif.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblActif.Location = New Point(12, 146)
        lblActif.Name = "lblActif"
        lblActif.Size = New Size(34, 17)
        lblActif.TabIndex = 7
        lblActif.Text = "État"
        ' 
        ' numOrdre
        ' 
        numOrdre.Font = New Font("Calibri", 11F)
        numOrdre.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        numOrdre.Location = New Point(112, 108)
        numOrdre.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        numOrdre.Name = "numOrdre"
        numOrdre.Size = New Size(90, 25)
        numOrdre.TabIndex = 6
        numOrdre.TextAlign = HorizontalAlignment.Right
        ' 
        ' lblOrdre
        ' 
        lblOrdre.AutoSize = True
        lblOrdre.BackColor = Color.Transparent
        lblOrdre.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblOrdre.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblOrdre.Location = New Point(12, 112)
        lblOrdre.Name = "lblOrdre"
        lblOrdre.Size = New Size(42, 17)
        lblOrdre.TabIndex = 5
        lblOrdre.Text = "Ordre"
        ' 
        ' txtLibelle
        ' 
        txtLibelle.Font = New Font("Calibri", 11F)
        txtLibelle.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLibelle.Location = New Point(112, 74)
        txtLibelle.MaxLength = 100
        txtLibelle.Name = "txtLibelle"
        txtLibelle.Size = New Size(270, 25)
        txtLibelle.TabIndex = 4
        ' 
        ' lblLibelle
        ' 
        lblLibelle.AutoSize = True
        lblLibelle.BackColor = Color.Transparent
        lblLibelle.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblLibelle.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLibelle.Location = New Point(12, 78)
        lblLibelle.Name = "lblLibelle"
        lblLibelle.Size = New Size(48, 17)
        lblLibelle.TabIndex = 3
        lblLibelle.Text = "Libellé"
        ' 
        ' txtCode
        ' 
        txtCode.CharacterCasing = CharacterCasing.Upper
        txtCode.Font = New Font("Calibri", 11F)
        txtCode.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCode.Location = New Point(112, 40)
        txtCode.MaxLength = 10
        txtCode.Name = "txtCode"
        txtCode.Size = New Size(270, 25)
        txtCode.TabIndex = 2
        ' 
        ' lblCode
        ' 
        lblCode.AutoSize = True
        lblCode.BackColor = Color.Transparent
        lblCode.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCode.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCode.Location = New Point(12, 44)
        lblCode.Name = "lblCode"
        lblCode.Size = New Size(38, 17)
        lblCode.TabIndex = 1
        lblCode.Text = "Code"
        ' 
        ' pnlTop
        ' 
        pnlTop.Controls.Add(carteRecherche)
        pnlTop.BackColor = Color.Transparent
        pnlTop.Dock = DockStyle.Top
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Size = New Size(933, 60)
        pnlTop.TabIndex = 0
        ' 
        ' carteRecherche
        ' 
        carteRecherche.Controls.Add(txtRecherche)
        carteRecherche.Controls.Add(chkAfficherInactifs)
        carteRecherche.Location = New Point(0, 4)
        carteRecherche.Name = "carteRecherche"
        carteRecherche.Padding = New Padding(14, 10, 14, 10)
        carteRecherche.Size = New Size(520, 52)
        carteRecherche.TabIndex = 0
        ' 
        ' chkAfficherInactifs
        ' 
        chkAfficherInactifs.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        chkAfficherInactifs.AutoSize = False
        chkAfficherInactifs.Location = New Point(330, 14)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(170, 24)
        chkAfficherInactifs.TabIndex = 2
        chkAfficherInactifs.Text = "Afficher les inactifs"
        ' 
        ' txtRecherche
        ' 
        txtRecherche.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtRecherche.Location = New Point(14, 10)
        txtRecherche.Name = "txtRecherche"
        txtRecherche.Size = New Size(300, 32)
        txtRecherche.TabIndex = 1
        txtRecherche.TexteAide = "Code ou libellé…"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnActiverDesactiver)
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(965, 60)
        pnlActions.TabIndex = 1
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(693, 10)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 40)
        btnEnregistrer.TabIndex = 4
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(835, 10)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 40)
        btnAnnuler.TabIndex = 5
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' btnActiverDesactiver
        ' 
        btnActiverDesactiver.Image = CType(resources.GetObject("btnActiverDesactiver.Image"), Image)
        btnActiverDesactiver.Location = New Point(406, 10)
        btnActiverDesactiver.Name = "btnActiverDesactiver"
        btnActiverDesactiver.Size = New Size(130, 40)
        btnActiverDesactiver.TabIndex = 2
        btnActiverDesactiver.Tag = "activer_normal"
        btnActiverDesactiver.Text = "Désactiver"
        ' 
        ' btnActualiser
        ' 
        btnActualiser.Image = CType(resources.GetObject("btnActualiser.Image"), Image)
        btnActualiser.Location = New Point(269, 10)
        btnActualiser.Name = "btnActualiser"
        btnActualiser.Size = New Size(127, 40)
        btnActualiser.TabIndex = 3
        btnActualiser.Tag = "actualiser_normal"
        btnActualiser.Text = "Actualiser"
        btnActualiser.Variante = VarianteBouton.Clair
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
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 0
        enTete.SousTitre = "Consultez et gérez les valeurs de référence."
        enTete.Text = "Référentiel"
        ' 
        ' UC_ReferentielBase
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_ReferentielBase"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        CType(dgvReferentiel, ComponentModel.ISupportInitialize).EndInit()
        pnlEdition.ResumeLayout(False)
        pnlEdition.PerformLayout()
        CType(numOrdre, ComponentModel.ISupportInitialize).EndInit()
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents carteRecherche As AltheaCarte
    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents btnActiverDesactiver As AltheaBouton
    Friend WithEvents btnActualiser As AltheaBouton
    Friend WithEvents btnNouveau As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents pnlTop As Panel
    Friend WithEvents txtRecherche As AltheaRecherche
    Friend WithEvents chkAfficherInactifs As AltheaCase
    Friend WithEvents dgvReferentiel As DataGridView
    Friend WithEvents pagReferentiel As UC_Pagination
    Friend WithEvents pnlEdition As AltheaCarte
    Friend WithEvents lblCode As Label
    Friend WithEvents txtCode As TextBox
    Friend WithEvents lblLibelle As Label
    Friend WithEvents txtLibelle As TextBox
    Friend WithEvents lblOrdre As Label
    Friend WithEvents numOrdre As NumericUpDown
    Friend WithEvents lblActif As Label
    Friend WithEvents chkActif As AltheaCase
    Friend WithEvents colId As DataGridViewTextBoxColumn
    Friend WithEvents colCode As DataGridViewTextBoxColumn
    Friend WithEvents colLibelle As DataGridViewTextBoxColumn
    Friend WithEvents colOrdre As DataGridViewTextBoxColumn
    Friend WithEvents colActif As DataGridViewTextBoxColumn

End Class
