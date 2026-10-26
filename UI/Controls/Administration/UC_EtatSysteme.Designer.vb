<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_EtatSysteme
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
        carteEtat = New AltheaCarte()
        dgvEtat = New DataGridView()
        colElement = New DataGridViewTextBoxColumn()
        colValeur = New DataGridViewTextBoxColumn()
        colEtat = New DataGridViewTextBoxColumn()
        pnlActions = New AltheaBarreActions()
        lblHorodatage = New Label()
        btnCopierRapport = New AltheaBouton()
        btnActualiser = New AltheaBouton()
        btnRealignerDus = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        carteEtat.SuspendLayout()
        CType(dgvEtat, ComponentModel.ISupportInitialize).BeginInit()
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
        pnlCenter.Controls.Add(carteEtat)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(965, 621)
        pnlCenter.TabIndex = 1
        ' 
        ' carteEtat
        ' 
        carteEtat.Controls.Add(dgvEtat)
        carteEtat.Dock = DockStyle.Fill
        carteEtat.Location = New Point(16, 16)
        carteEtat.Name = "carteEtat"
        carteEtat.Padding = New Padding(14, 10, 14, 12)
        carteEtat.Size = New Size(933, 597)
        carteEtat.TabIndex = 0
        carteEtat.Titre = "État du poste et de l'application"
        ' 
        ' dgvEtat
        ' 
        dgvEtat.AllowUserToAddRows = False
        dgvEtat.AllowUserToDeleteRows = False
        dgvEtat.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvEtat.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvEtat.Columns.AddRange(New DataGridViewColumn() {colElement, colValeur, colEtat})
        dgvEtat.Dock = DockStyle.Fill
        dgvEtat.Location = New Point(14, 39)
        dgvEtat.MultiSelect = False
        dgvEtat.Name = "dgvEtat"
        dgvEtat.ReadOnly = True
        dgvEtat.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvEtat.Size = New Size(905, 546)
        dgvEtat.TabIndex = 0
        ' 
        ' colElement
        ' 
        colElement.HeaderText = "Élément"
        colElement.Name = "colElement"
        colElement.ReadOnly = True
        colElement.Width = 240
        ' 
        ' colValeur
        ' 
        colValeur.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colValeur.HeaderText = "Valeur"
        colValeur.Name = "colValeur"
        colValeur.ReadOnly = True
        ' 
        ' colEtat
        ' 
        colEtat.HeaderText = "État"
        colEtat.Name = "colEtat"
        colEtat.ReadOnly = True
        colEtat.Width = 120
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(lblHorodatage)
        pnlActions.Controls.Add(btnCopierRapport)
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.Controls.Add(btnRealignerDus)
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
        ' lblHorodatage
        ' 
        lblHorodatage.AutoSize = True
        lblHorodatage.BackColor = Color.Transparent
        lblHorodatage.Font = New Font("Calibri", 9F, FontStyle.Italic)
        lblHorodatage.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblHorodatage.Location = New Point(16, 22)
        lblHorodatage.Name = "lblHorodatage"
        lblHorodatage.Size = New Size(0, 14)
        lblHorodatage.TabIndex = 0
        ' 
        ' btnCopierRapport
        ' 
        btnCopierRapport.FlatStyle = FlatStyle.Flat
        btnCopierRapport.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnCopierRapport.ForeColor = Color.White
        btnCopierRapport.Location = New Point(600, 10)
        btnCopierRapport.Name = "btnCopierRapport"
        btnCopierRapport.Size = New Size(220, 40)
        btnCopierRapport.TabIndex = 1
        btnCopierRapport.Tag = "generer_normal"
        btnCopierRapport.Text = "Copier le rapport"
        btnCopierRapport.UseVisualStyleBackColor = False
        btnCopierRapport.Variante = VarianteBouton.Clair
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
        ' btnRealignerDus
        ' 
        btnRealignerDus.Enabled = False
        btnRealignerDus.FlatStyle = FlatStyle.Flat
        btnRealignerDus.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnRealignerDus.ForeColor = Color.White
        btnRealignerDus.Location = New Point(300, 10)
        btnRealignerDus.Name = "btnRealignerDus"
        btnRealignerDus.Size = New Size(290, 40)
        btnRealignerDus.TabIndex = 3
        btnRealignerDus.Text = "Réaligner les montants en attente"
        btnRealignerDus.UseVisualStyleBackColor = False
        btnRealignerDus.Variante = VarianteBouton.Ocre
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = My.Resources.Resources.Titre_etatSystem
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Padding = New Padding(16, 4, 16, 4)
        enTete.Size = New Size(965, 58)
        enTete.SousTitre = "Versions, connexion à la base, dossiers, espace disque et sauvegardes, en un coup d'œil."
        enTete.TabIndex = 3
        enTete.Text = "État du système"
        ' 
        ' UC_EtatSysteme
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_EtatSysteme"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        carteEtat.ResumeLayout(False)
        CType(dgvEtat, ComponentModel.ISupportInitialize).EndInit()
        pnlActions.ResumeLayout(False)
        pnlActions.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteEtat As AltheaCarte
    Friend WithEvents dgvEtat As DataGridView
    Friend WithEvents colElement As DataGridViewTextBoxColumn
    Friend WithEvents colValeur As DataGridViewTextBoxColumn
    Friend WithEvents colEtat As DataGridViewTextBoxColumn
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents lblHorodatage As Label
    Friend WithEvents btnCopierRapport As AltheaBouton
    Friend WithEvents btnActualiser As AltheaBouton
    Friend WithEvents btnRealignerDus As AltheaBouton

End Class
