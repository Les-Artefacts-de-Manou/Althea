<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CategoriesQuotaEditor
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CategoriesQuotaEditor))
        enTete = New AltheaEnTete()
        carteCategories = New AltheaCarte()
        dgvCategories = New DataGridView()
        colCode = New DataGridViewTextBoxColumn()
        colLibelle = New DataGridViewTextBoxColumn()
        colEtat = New DataGridViewTextBoxColumn()
        lblAide = New Label()
        barreCategories = New AltheaBarreOutils()
        btnActiver = New AltheaBouton()
        btnSupprimer = New AltheaBouton()
        carteNouvelle = New AltheaCarte()
        lblCode = New Label()
        txtCode = New TextBox()
        lblLibelle = New Label()
        txtLibelle = New TextBox()
        btnAjouter = New AltheaBouton()
        pnlActions = New AltheaBarreActions()
        btnFermer = New AltheaBouton()
        carteCategories.SuspendLayout()
        CType(dgvCategories, ComponentModel.ISupportInitialize).BeginInit()
        barreCategories.SuspendLayout()
        carteNouvelle.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(500, 62)
        enTete.TabIndex = 7
        enTete.SousTitre = "Gérez les catégories de quotas"
        enTete.Text = "Quota"
        ' 
        ' carteCategories
        ' 
        carteCategories.Controls.Add(dgvCategories)
        carteCategories.Controls.Add(lblAide)
        carteCategories.Controls.Add(barreCategories)
        carteCategories.Location = New Point(12, 70)
        carteCategories.Name = "carteCategories"
        carteCategories.Padding = New Padding(12, 8, 12, 12)
        carteCategories.Size = New Size(476, 270)
        carteCategories.TabIndex = 0
        ' 
        ' barreCategories
        ' 
        barreCategories.Controls.Add(btnSupprimer)
        barreCategories.Controls.Add(btnActiver)
        barreCategories.Dock = DockStyle.Top
        barreCategories.Location = New Point(12, 8)
        barreCategories.Name = "barreCategories"
        barreCategories.Size = New Size(452, 42)
        barreCategories.TabIndex = 0
        barreCategories.Titre = "Catégories"
        ' 
        ' btnActiver
        ' 
        btnActiver.Location = New Point(130, 6)
        btnActiver.Name = "btnActiver"
        btnActiver.Size = New Size(125, 30)
        btnActiver.TabIndex = 0
        btnActiver.Tag = "activer_normal"
        btnActiver.Text = "Désactiver"
        btnActiver.Variante = VarianteBouton.Outil
        ' 
        ' btnSupprimer
        ' 
        btnSupprimer.Location = New Point(259, 6)
        btnSupprimer.Name = "btnSupprimer"
        btnSupprimer.Size = New Size(120, 30)
        btnSupprimer.TabIndex = 1
        btnSupprimer.Tag = "supprimer_normal"
        btnSupprimer.Text = "Supprimer"
        btnSupprimer.Variante = VarianteBouton.OutilDanger
        ' 
        ' lblAide
        ' 
        lblAide.Dock = DockStyle.Top
        lblAide.Font = New Font("Calibri", 10F, FontStyle.Italic)
        lblAide.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAide.Location = New Point(12, 50)
        lblAide.Name = "lblAide"
        lblAide.Padding = New Padding(0, 6, 0, 6)
        lblAide.Size = New Size(452, 62)
        lblAide.TabIndex = 1
        lblAide.Text = "Une catégorie = un compteur de quota propre à ce domaine (ex. Soutien). Les natures y sont rangées dans l'onglet 4. Un même nom peut exister dans un autre domaine, avec ses propres plafond et durée."
        ' 
        ' dgvCategories
        ' 
        dgvCategories.AllowUserToAddRows = False
        dgvCategories.AllowUserToDeleteRows = False
        dgvCategories.AllowUserToResizeRows = False
        dgvCategories.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvCategories.Columns.AddRange(New DataGridViewColumn() {colCode, colLibelle, colEtat})
        dgvCategories.Dock = DockStyle.Fill
        dgvCategories.Location = New Point(12, 112)
        dgvCategories.MultiSelect = False
        dgvCategories.Name = "dgvCategories"
        dgvCategories.ReadOnly = True
        dgvCategories.RowHeadersVisible = False
        dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvCategories.Size = New Size(452, 146)
        dgvCategories.TabIndex = 2
        ' 
        ' colCode
        ' 
        colCode.HeaderText = "Code"
        colCode.Name = "colCode"
        colCode.ReadOnly = True
        colCode.Width = 90
        ' 
        ' colLibelle
        ' 
        colLibelle.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colLibelle.HeaderText = "Libellé"
        colLibelle.Name = "colLibelle"
        colLibelle.ReadOnly = True
        ' 
        ' colEtat
        ' 
        colEtat.HeaderText = "État"
        colEtat.Name = "colEtat"
        colEtat.ReadOnly = True
        colEtat.Width = 70
        ' 
        ' carteNouvelle
        ' 
        carteNouvelle.Controls.Add(lblCode)
        carteNouvelle.Controls.Add(txtCode)
        carteNouvelle.Controls.Add(lblLibelle)
        carteNouvelle.Controls.Add(txtLibelle)
        carteNouvelle.Controls.Add(btnAjouter)
        carteNouvelle.Location = New Point(12, 348)
        carteNouvelle.Name = "carteNouvelle"
        carteNouvelle.Padding = New Padding(12, 8, 12, 10)
        carteNouvelle.Size = New Size(476, 100)
        carteNouvelle.TabIndex = 1
        carteNouvelle.TabStop = False
        carteNouvelle.Titre = "Nouvelle catégorie"
        ' 
        ' lblCode
        ' 
        lblCode.AutoSize = True
        lblCode.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCode.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCode.Location = New Point(14, 38)
        lblCode.Name = "lblCode"
        lblCode.Size = New Size(50, 17)
        lblCode.TabIndex = 0
        lblCode.Text = "Code *"
        ' 
        ' txtCode
        ' 
        txtCode.BackColor = Color.White
        txtCode.CharacterCasing = CharacterCasing.Upper
        txtCode.Font = New Font("Calibri", 11F)
        txtCode.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCode.Location = New Point(14, 60)
        txtCode.MaxLength = 30
        txtCode.Name = "txtCode"
        txtCode.Size = New Size(110, 25)
        txtCode.TabIndex = 1
        ' 
        ' lblLibelle
        ' 
        lblLibelle.AutoSize = True
        lblLibelle.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblLibelle.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLibelle.Location = New Point(136, 38)
        lblLibelle.Name = "lblLibelle"
        lblLibelle.Size = New Size(57, 17)
        lblLibelle.TabIndex = 2
        lblLibelle.Text = "Libellé *"
        ' 
        ' txtLibelle
        ' 
        txtLibelle.BackColor = Color.White
        txtLibelle.Font = New Font("Calibri", 11F)
        txtLibelle.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLibelle.Location = New Point(136, 60)
        txtLibelle.MaxLength = 100
        txtLibelle.Name = "txtLibelle"
        txtLibelle.Size = New Size(200, 25)
        txtLibelle.TabIndex = 3
        ' 
        ' btnAjouter
        ' 
        btnAjouter.Image = CType(resources.GetObject("btnAjouter.Image"), Image)
        btnAjouter.Location = New Point(346, 56)
        btnAjouter.Name = "btnAjouter"
        btnAjouter.Size = New Size(116, 32)
        btnAjouter.TabIndex = 4
        btnAjouter.Tag = "nouveau_normal"
        btnAjouter.Text = "Ajouter"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 456)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(500, 52)
        pnlActions.TabIndex = 2
        ' 
        ' btnFermer
        ' 
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.Location = New Point(352, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(131, 38)
        btnFermer.TabIndex = 6
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' CategoriesQuotaEditor
        ' 
        AcceptButton = btnAjouter
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        CancelButton = btnFermer
        ClientSize = New Size(500, 508)
        Controls.Add(carteCategories)
        Controls.Add(carteNouvelle)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "CategoriesQuotaEditor"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Catégories de quota"
        carteCategories.ResumeLayout(False)
        CType(dgvCategories, ComponentModel.ISupportInitialize).EndInit()
        barreCategories.ResumeLayout(False)
        carteNouvelle.ResumeLayout(False)
        carteNouvelle.PerformLayout()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteCategories As AltheaCarte
    Friend WithEvents barreCategories As AltheaBarreOutils
    Friend WithEvents btnActiver As AltheaBouton
    Friend WithEvents btnSupprimer As AltheaBouton
    Friend WithEvents lblAide As Label
    Friend WithEvents dgvCategories As DataGridView
    Friend WithEvents colCode As DataGridViewTextBoxColumn
    Friend WithEvents colLibelle As DataGridViewTextBoxColumn
    Friend WithEvents colEtat As DataGridViewTextBoxColumn
    Friend WithEvents carteNouvelle As AltheaCarte
    Friend WithEvents lblCode As Label
    Friend WithEvents txtCode As TextBox
    Friend WithEvents lblLibelle As Label
    Friend WithEvents txtLibelle As TextBox
    Friend WithEvents btnAjouter As AltheaBouton
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnFermer As AltheaBouton

End Class
