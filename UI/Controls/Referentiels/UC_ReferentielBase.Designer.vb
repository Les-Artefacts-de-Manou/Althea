' -------------------------------------------------------------------------------------------------
' Designer    : UC_ReferentielBase
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Définit la structure visuelle commune à tous les écrans de gestion de référentiels (tables ref_*).
' Reprend la charte Althéa de UC_Utilisateurs : titre + grille + barre d'actions.
' Ajoute un panneau d'édition générique (Code, Libellé, Ordre, Actif) à droite de la grille.
'
' Remarques   :
' - Ce UserControl est destiné à être hérité (UC_Domaines, etc.).
' - Les contrôles sont déclarés Friend WithEvents pour être accessibles aux classes dérivées.
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
        components = New ComponentModel.Container()
        pnlForm = New Panel()
        pnlCenter = New Panel()
        dgvReferentiel = New DataGridView()
        colId = New DataGridViewTextBoxColumn()
        colCode = New DataGridViewTextBoxColumn()
        colLibelle = New DataGridViewTextBoxColumn()
        colOrdre = New DataGridViewTextBoxColumn()
        colActif = New DataGridViewCheckBoxColumn()
        pnlEdition = New Panel()
        chkActif = New CheckBox()
        lblActif = New Label()
        numOrdre = New NumericUpDown()
        lblOrdre = New Label()
        txtLibelle = New TextBox()
        lblLibelle = New Label()
        txtCode = New TextBox()
        lblCode = New Label()
        lblTitreEdition = New Label()
        pnlTop = New Panel()
        chkAfficherInactifs = New CheckBox()
        lblRecherche = New Label()
        txtRecherche = New TextBox()
        pnlActions = New Panel()
        btnEnregistrer = New Button()
        btnAnnuler = New Button()
        btnActiverDesactiver = New Button()
        btnActualiser = New Button()
        btnNouveau = New Button()
        btnModifier = New Button()
        pnlTitre = New Panel()
        lblTop = New Label()
        lblTitreForm = New Label()
        picTitre = New PictureBox()
        errProvider = New ErrorProvider(components)
        ttMain = New ToolTip(components)
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        CType(dgvReferentiel, ComponentModel.ISupportInitialize).BeginInit()
        pnlEdition.SuspendLayout()
        CType(numOrdre, ComponentModel.ISupportInitialize).BeginInit()
        pnlTop.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTitre.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(pnlTitre)
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
        dgvReferentiel.Dock = DockStyle.Fill
        dgvReferentiel.Location = New Point(16, 80)
        dgvReferentiel.Name = "dgvReferentiel"
        dgvReferentiel.ReadOnly = True
        dgvReferentiel.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvReferentiel.Size = New Size(613, 536)
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
        colCode.Width = 130
        ' 
        ' colLibelle
        ' 
        colLibelle.HeaderText = "Libellé"
        colLibelle.Name = "colLibelle"
        colLibelle.ReadOnly = True
        colLibelle.Width = 300
        ' 
        ' colOrdre
        ' 
        colOrdre.HeaderText = "Ordre"
        colOrdre.Name = "colOrdre"
        colOrdre.ReadOnly = True
        colOrdre.Width = 80
        ' 
        ' colActif
        ' 
        colActif.HeaderText = "Actif"
        colActif.Name = "colActif"
        colActif.ReadOnly = True
        colActif.Width = 60
        ' 
        ' pnlEdition
        ' 
        pnlEdition.BackColor = Color.FromArgb(CByte(237), CByte(231), CByte(224))
        pnlEdition.BorderStyle = BorderStyle.FixedSingle
        pnlEdition.Controls.Add(chkActif)
        pnlEdition.Controls.Add(lblActif)
        pnlEdition.Controls.Add(numOrdre)
        pnlEdition.Controls.Add(lblOrdre)
        pnlEdition.Controls.Add(txtLibelle)
        pnlEdition.Controls.Add(lblLibelle)
        pnlEdition.Controls.Add(txtCode)
        pnlEdition.Controls.Add(lblCode)
        pnlEdition.Controls.Add(lblTitreEdition)
        pnlEdition.Dock = DockStyle.Right
        pnlEdition.Location = New Point(629, 80)
        pnlEdition.Name = "pnlEdition"
        pnlEdition.Padding = New Padding(16)
        pnlEdition.Size = New Size(320, 536)
        pnlEdition.TabIndex = 2
        ' 
        ' chkActif
        ' 
        chkActif.AutoSize = True
        chkActif.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkActif.Location = New Point(110, 213)
        chkActif.Name = "chkActif"
        chkActif.Size = New Size(53, 21)
        chkActif.TabIndex = 8
        chkActif.Text = "Actif"
        chkActif.UseVisualStyleBackColor = True
        ' 
        ' lblActif
        ' 
        lblActif.AutoSize = True
        lblActif.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblActif.Location = New Point(20, 214)
        lblActif.Name = "lblActif"
        lblActif.Size = New Size(32, 17)
        lblActif.TabIndex = 7
        lblActif.Text = "État"
        ' 
        ' numOrdre
        ' 
        numOrdre.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        numOrdre.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        numOrdre.Location = New Point(110, 162)
        numOrdre.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        numOrdre.Name = "numOrdre"
        numOrdre.Size = New Size(170, 24)
        numOrdre.TabIndex = 6
        ' 
        ' lblOrdre
        ' 
        lblOrdre.AutoSize = True
        lblOrdre.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblOrdre.Location = New Point(20, 164)
        lblOrdre.Name = "lblOrdre"
        lblOrdre.Size = New Size(41, 17)
        lblOrdre.TabIndex = 5
        lblOrdre.Text = "Ordre"
        ' 
        ' txtLibelle
        ' 
        txtLibelle.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtLibelle.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLibelle.Location = New Point(20, 120)
        txtLibelle.MaxLength = 100
        txtLibelle.Name = "txtLibelle"
        txtLibelle.Size = New Size(260, 24)
        txtLibelle.TabIndex = 4
        ' 
        ' lblLibelle
        ' 
        lblLibelle.AutoSize = True
        lblLibelle.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLibelle.Location = New Point(20, 99)
        lblLibelle.Name = "lblLibelle"
        lblLibelle.Size = New Size(44, 17)
        lblLibelle.TabIndex = 3
        lblLibelle.Text = "Libellé"
        ' 
        ' txtCode
        ' 
        txtCode.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCode.CharacterCasing = CharacterCasing.Upper
        txtCode.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCode.Location = New Point(20, 65)
        txtCode.MaxLength = 10
        txtCode.Name = "txtCode"
        txtCode.Size = New Size(260, 24)
        txtCode.TabIndex = 2
        ' 
        ' lblCode
        ' 
        lblCode.AutoSize = True
        lblCode.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCode.Location = New Point(20, 44)
        lblCode.Name = "lblCode"
        lblCode.Size = New Size(36, 17)
        lblCode.TabIndex = 1
        lblCode.Text = "Code"
        ' 
        ' lblTitreEdition
        ' 
        lblTitreEdition.AutoSize = True
        lblTitreEdition.Font = New Font("Calibri", 12F, FontStyle.Bold)
        lblTitreEdition.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreEdition.Location = New Point(16, 12)
        lblTitreEdition.Name = "lblTitreEdition"
        lblTitreEdition.Size = New Size(49, 19)
        lblTitreEdition.TabIndex = 0
        lblTitreEdition.Text = "Détail"
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.Transparent
        pnlTop.Controls.Add(chkAfficherInactifs)
        pnlTop.Controls.Add(lblRecherche)
        pnlTop.Controls.Add(txtRecherche)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Size = New Size(933, 64)
        pnlTop.TabIndex = 0
        ' 
        ' chkAfficherInactifs
        ' 
        chkAfficherInactifs.AutoSize = True
        chkAfficherInactifs.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkAfficherInactifs.Location = New Point(370, 38)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(114, 21)
        chkAfficherInactifs.TabIndex = 2
        chkAfficherInactifs.Text = "Afficher inactifs"
        chkAfficherInactifs.UseVisualStyleBackColor = True
        ' 
        ' lblRecherche
        ' 
        lblRecherche.AutoSize = True
        lblRecherche.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRecherche.Location = New Point(20, 13)
        lblRecherche.Name = "lblRecherche"
        lblRecherche.Size = New Size(152, 17)
        lblRecherche.TabIndex = 0
        lblRecherche.Text = "Recherche (code + libellé)"
        ' 
        ' txtRecherche
        ' 
        txtRecherche.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtRecherche.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtRecherche.Location = New Point(20, 35)
        txtRecherche.Name = "txtRecherche"
        txtRecherche.Size = New Size(335, 24)
        txtRecherche.TabIndex = 1
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.FromArgb(CByte(237), CByte(231), CByte(224))
        pnlActions.BorderStyle = BorderStyle.Fixed3D
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
        pnlActions.Padding = New Padding(8)
        pnlActions.Size = New Size(965, 57)
        pnlActions.TabIndex = 1
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnEnregistrer.BackgroundImageLayout = ImageLayout.Center
        btnEnregistrer.FlatAppearance.BorderSize = 0
        btnEnregistrer.FlatStyle = FlatStyle.Flat
        btnEnregistrer.ForeColor = Color.White
        btnEnregistrer.ImageAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.Location = New Point(720, 6)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(112, 40)
        btnEnregistrer.TabIndex = 4
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.TextAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEnregistrer.UseVisualStyleBackColor = False
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAnnuler.BackgroundImageLayout = ImageLayout.Center
        btnAnnuler.FlatAppearance.BorderSize = 0
        btnAnnuler.FlatStyle = FlatStyle.Flat
        btnAnnuler.ForeColor = Color.White
        btnAnnuler.ImageAlign = ContentAlignment.MiddleLeft
        btnAnnuler.Location = New Point(838, 6)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(112, 40)
        btnAnnuler.TabIndex = 5
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.TextAlign = ContentAlignment.MiddleLeft
        btnAnnuler.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAnnuler.UseVisualStyleBackColor = False
        ' 
        ' btnActiverDesactiver
        ' 
        btnActiverDesactiver.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnActiverDesactiver.BackgroundImageLayout = ImageLayout.Center
        btnActiverDesactiver.FlatAppearance.BorderSize = 0
        btnActiverDesactiver.FlatStyle = FlatStyle.Flat
        btnActiverDesactiver.ForeColor = Color.White
        btnActiverDesactiver.ImageAlign = ContentAlignment.MiddleLeft
        btnActiverDesactiver.Location = New Point(366, 6)
        btnActiverDesactiver.Name = "btnActiverDesactiver"
        btnActiverDesactiver.Size = New Size(118, 40)
        btnActiverDesactiver.TabIndex = 2
        btnActiverDesactiver.Tag = "activer_normal"
        btnActiverDesactiver.Text = "Désactiver"
        btnActiverDesactiver.TextAlign = ContentAlignment.MiddleLeft
        btnActiverDesactiver.TextImageRelation = TextImageRelation.ImageBeforeText
        btnActiverDesactiver.UseVisualStyleBackColor = False
        ' 
        ' btnActualiser
        ' 
        btnActualiser.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnActualiser.BackgroundImageLayout = ImageLayout.Center
        btnActualiser.FlatAppearance.BorderSize = 0
        btnActualiser.FlatStyle = FlatStyle.Flat
        btnActualiser.ForeColor = Color.White
        btnActualiser.ImageAlign = ContentAlignment.MiddleLeft
        btnActualiser.Location = New Point(247, 6)
        btnActualiser.Name = "btnActualiser"
        btnActualiser.Size = New Size(112, 40)
        btnActualiser.TabIndex = 3
        btnActualiser.Tag = "actualiser_normal"
        btnActualiser.Text = "Actualiser"
        btnActualiser.TextAlign = ContentAlignment.MiddleLeft
        btnActualiser.TextImageRelation = TextImageRelation.ImageBeforeText
        btnActualiser.UseVisualStyleBackColor = False
        ' 
        ' btnNouveau
        ' 
        btnNouveau.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnNouveau.BackgroundImageLayout = ImageLayout.Center
        btnNouveau.FlatAppearance.BorderSize = 0
        btnNouveau.FlatStyle = FlatStyle.Flat
        btnNouveau.ForeColor = Color.White
        btnNouveau.ImageAlign = ContentAlignment.MiddleLeft
        btnNouveau.Location = New Point(10, 6)
        btnNouveau.Name = "btnNouveau"
        btnNouveau.Size = New Size(112, 40)
        btnNouveau.TabIndex = 0
        btnNouveau.Tag = "nouveau_normal"
        btnNouveau.Text = "Nouveau"
        btnNouveau.TextAlign = ContentAlignment.MiddleLeft
        btnNouveau.TextImageRelation = TextImageRelation.ImageBeforeText
        btnNouveau.UseVisualStyleBackColor = False
        ' 
        ' btnModifier
        ' 
        btnModifier.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnModifier.BackgroundImageLayout = ImageLayout.Center
        btnModifier.FlatAppearance.BorderSize = 0
        btnModifier.FlatStyle = FlatStyle.Flat
        btnModifier.ForeColor = Color.White
        btnModifier.ImageAlign = ContentAlignment.MiddleLeft
        btnModifier.Location = New Point(128, 6)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(112, 40)
        btnModifier.TabIndex = 1
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        btnModifier.TextAlign = ContentAlignment.MiddleLeft
        btnModifier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifier.UseVisualStyleBackColor = False
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTop)
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Controls.Add(picTitre)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(16, 16)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(965, 58)
        pnlTitre.TabIndex = 0
        ' 
        ' lblTop
        ' 
        lblTop.Font = New Font("Calibri", 11F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(386, 16)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(563, 26)
        lblTop.TabIndex = 3
        lblTop.Text = "Consultez et gérez les valeurs de référence."
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.Anchor = AnchorStyles.Left
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 18F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(73, 7)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(0, 4, 8, 4)
        lblTitreForm.Size = New Size(133, 37)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Référentiel"
        ' 
        ' picTitre
        ' 
        picTitre.BackgroundImage = My.Resources.Resources.Fond_icone_Transp
        picTitre.BackgroundImageLayout = ImageLayout.Stretch
        picTitre.InitialImage = Nothing
        picTitre.Location = New Point(14, 3)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(60, 52)
        picTitre.TabIndex = 2
        picTitre.TabStop = False
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
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
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).EndInit()
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTop As Label
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents picTitre As PictureBox
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnEnregistrer As Button
    Friend WithEvents btnAnnuler As Button
    Friend WithEvents btnActiverDesactiver As Button
    Friend WithEvents btnActualiser As Button
    Friend WithEvents btnNouveau As Button
    Friend WithEvents btnModifier As Button
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents pnlTop As Panel
    Friend WithEvents lblRecherche As Label
    Friend WithEvents txtRecherche As TextBox
    Friend WithEvents chkAfficherInactifs As CheckBox
    Friend WithEvents dgvReferentiel As DataGridView
    Friend WithEvents colId As DataGridViewTextBoxColumn
    Friend WithEvents colCode As DataGridViewTextBoxColumn
    Friend WithEvents colLibelle As DataGridViewTextBoxColumn
    Friend WithEvents colOrdre As DataGridViewTextBoxColumn
    Friend WithEvents colActif As DataGridViewCheckBoxColumn
    Friend WithEvents pnlEdition As Panel
    Friend WithEvents lblTitreEdition As Label
    Friend WithEvents lblCode As Label
    Friend WithEvents txtCode As TextBox
    Friend WithEvents lblLibelle As Label
    Friend WithEvents txtLibelle As TextBox
    Friend WithEvents lblOrdre As Label
    Friend WithEvents numOrdre As NumericUpDown
    Friend WithEvents lblActif As Label
    Friend WithEvents chkActif As CheckBox
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents ttMain As ToolTip

End Class
