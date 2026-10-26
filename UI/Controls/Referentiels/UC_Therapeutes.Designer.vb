<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_Therapeutes
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

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Therapeutes))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        dgvTherapeutes = New DataGridView()
        pagTherapeutes = New UC_Pagination()
        colNom = New DataGridViewTextBoxColumn()
        colPrenom = New DataGridViewTextBoxColumn()
        colSpecialite = New DataGridViewTextBoxColumn()
        colTelephone = New DataGridViewTextBoxColumn()
        colEmail = New DataGridViewTextBoxColumn()
        colLocalite = New DataGridViewTextBoxColumn()
        colActif = New DataGridViewTextBoxColumn()
        colIdTherapeute = New DataGridViewTextBoxColumn()
        colCodeTherapeute = New DataGridViewTextBoxColumn()
        pnlTop = New AltheaCarte()
        btnRechercher = New AltheaBouton()
        btnReinitialiserFiltres = New AltheaBouton()
        chkAfficherInactifs = New AltheaCase()
        txtRecherche = New AltheaRecherche()
        pnlActions = New AltheaBarreActions()
        btnNouveau = New AltheaBouton()
        btnModifier = New AltheaBouton()
        btnActiverDesactiver = New AltheaBouton()
        btnSupprimer = New AltheaBouton()
        btnActualiser = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        CType(dgvTherapeutes, ComponentModel.ISupportInitialize).BeginInit()
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
        pnlForm.TabIndex = 1
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(dgvTherapeutes)
        pnlCenter.Controls.Add(pagTherapeutes)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(965, 624)
        pnlCenter.TabIndex = 21
        ' 
        ' dgvTherapeutes
        ' 
        dgvTherapeutes.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvTherapeutes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvTherapeutes.Columns.AddRange(New DataGridViewColumn() {colCodeTherapeute, colNom, colPrenom, colSpecialite, colTelephone, colEmail, colLocalite, colActif, colIdTherapeute})
        dgvTherapeutes.Dock = DockStyle.Fill
        dgvTherapeutes.Location = New Point(16, 80)
        dgvTherapeutes.Name = "dgvTherapeutes"
        dgvTherapeutes.ReadOnly = True
        dgvTherapeutes.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvTherapeutes.Size = New Size(933, 536)
        dgvTherapeutes.TabIndex = 8
        ' 
        ' pagTherapeutes
        ' 
        pagTherapeutes.BackColor = Color.Transparent
        pagTherapeutes.Dock = DockStyle.Bottom
        pagTherapeutes.Name = "pagTherapeutes"
        pagTherapeutes.Size = New Size(997, 32)
        pagTherapeutes.TabIndex = 9
        ' 
        ' colNom
        ' 
        colNom.HeaderText = "Nom"
        colNom.Name = "colNom"
        colNom.ReadOnly = True
        colNom.Width = 180
        ' 
        ' colPrenom
        ' 
        colPrenom.HeaderText = "Prénom"
        colPrenom.Name = "colPrenom"
        colPrenom.ReadOnly = True
        colPrenom.Width = 150
        ' 
        ' colSpecialite
        ' 
        colSpecialite.HeaderText = "Spécialité"
        colSpecialite.Name = "colSpecialite"
        colSpecialite.ReadOnly = True
        colSpecialite.Width = 180
        ' 
        ' colTelephone
        ' 
        colTelephone.HeaderText = "Téléphone"
        colTelephone.Name = "colTelephone"
        colTelephone.ReadOnly = True
        colTelephone.Width = 130
        ' 
        ' colEmail
        ' 
        colEmail.HeaderText = "E-mail"
        colEmail.Name = "colEmail"
        colEmail.ReadOnly = True
        colEmail.Width = 180
        ' 
        ' colLocalite
        ' 
        colLocalite.HeaderText = "Localité"
        colLocalite.Name = "colLocalite"
        colLocalite.ReadOnly = True
        colLocalite.Width = 130
        ' 
        ' colActif
        ' 
        colActif.HeaderText = "État"
        colActif.Name = "colActif"
        colActif.ReadOnly = True
        colActif.Width = 85
        ' 
        ' colIdTherapeute
        ' 
        colIdTherapeute.HeaderText = "Id thérapeute"
        colIdTherapeute.Name = "colIdTherapeute"
        colIdTherapeute.ReadOnly = True
        colIdTherapeute.Visible = False
        ' 
        ' colCodeTherapeute
        ' 
        colCodeTherapeute.HeaderText = "Code"
        colCodeTherapeute.Name = "colCodeTherapeute"
        colCodeTherapeute.ReadOnly = True
        colCodeTherapeute.Width = 85
        ' 
        ' pnlTop
        ' 
        pnlTop.Controls.Add(btnRechercher)
        pnlTop.Controls.Add(btnReinitialiserFiltres)
        pnlTop.Controls.Add(chkAfficherInactifs)
        pnlTop.Controls.Add(txtRecherche)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(14, 10, 14, 10)
        pnlTop.Size = New Size(933, 58)
        pnlTop.TabIndex = 7
        ' 
        ' btnRechercher
        ' 
        btnRechercher.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnRechercher.Image = CType(resources.GetObject("btnRechercher.Image"), Image)
        btnRechercher.Location = New Point(784, 13)
        btnRechercher.Name = "btnRechercher"
        btnRechercher.Size = New Size(135, 32)
        btnRechercher.TabIndex = 3
        btnRechercher.Text = "Rechercher"
        ' 
        ' btnReinitialiserFiltres
        ' 
        btnReinitialiserFiltres.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnReinitialiserFiltres.Image = CType(resources.GetObject("btnReinitialiserFiltres.Image"), Image)
        btnReinitialiserFiltres.Location = New Point(644, 13)
        btnReinitialiserFiltres.Name = "btnReinitialiserFiltres"
        btnReinitialiserFiltres.Size = New Size(130, 32)
        btnReinitialiserFiltres.TabIndex = 4
        btnReinitialiserFiltres.Tag = "reinitialiser_24_normal"
        btnReinitialiserFiltres.Text = "Réinitialiser"
        btnReinitialiserFiltres.Variante = VarianteBouton.Clair
        ' 
        ' chkAfficherInactifs
        ' 
        chkAfficherInactifs.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        chkAfficherInactifs.AutoSize = False
        chkAfficherInactifs.Location = New Point(462, 17)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(170, 24)
        chkAfficherInactifs.TabIndex = 5
        chkAfficherInactifs.Text = "Afficher les inactifs"
        ' 
        ' txtRecherche
        ' 
        txtRecherche.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtRecherche.Location = New Point(14, 13)
        txtRecherche.Name = "txtRecherche"
        txtRecherche.Size = New Size(440, 32)
        txtRecherche.TabIndex = 2
        txtRecherche.TexteAide = "Nom, prénom, spécialité…"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Controls.Add(btnActiverDesactiver)
        pnlActions.Controls.Add(btnSupprimer)
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(965, 60)
        pnlActions.TabIndex = 20
        ' 
        ' btnNouveau
        ' 
        btnNouveau.Image = CType(resources.GetObject("btnNouveau.Image"), Image)
        btnNouveau.Location = New Point(122, 10)
        btnNouveau.Name = "btnNouveau"
        btnNouveau.Size = New Size(120, 40)
        btnNouveau.TabIndex = 10
        btnNouveau.Tag = "nouveau_normal"
        btnNouveau.Text = "Nouveau"
        ' 
        ' btnModifier
        ' 
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(252, 10)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(119, 40)
        btnModifier.TabIndex = 11
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        ' 
        ' btnActiverDesactiver
        ' 
        btnActiverDesactiver.Image = CType(resources.GetObject("btnActiverDesactiver.Image"), Image)
        btnActiverDesactiver.Location = New Point(381, 10)
        btnActiverDesactiver.Name = "btnActiverDesactiver"
        btnActiverDesactiver.Size = New Size(180, 40)
        btnActiverDesactiver.TabIndex = 12
        btnActiverDesactiver.Tag = "activer_normal"
        btnActiverDesactiver.Text = "Activer/Désactiver"
        ' 
        ' btnSupprimer
        ' 
        btnSupprimer.Image = CType(resources.GetObject("btnSupprimer.Image"), Image)
        btnSupprimer.Location = New Point(571, 10)
        btnSupprimer.Name = "btnSupprimer"
        btnSupprimer.Size = New Size(130, 40)
        btnSupprimer.TabIndex = 13
        btnSupprimer.Tag = "supprimer_normal"
        btnSupprimer.Text = "Supprimer"
        btnSupprimer.Variante = VarianteBouton.Terre
        ' 
        ' btnActualiser
        ' 
        btnActualiser.Image = CType(resources.GetObject("btnActualiser.Image"), Image)
        btnActualiser.Location = New Point(711, 10)
        btnActualiser.Name = "btnActualiser"
        btnActualiser.Size = New Size(127, 40)
        btnActualiser.TabIndex = 14
        btnActualiser.Tag = "actualiser_normal"
        btnActualiser.Text = "Actualiser"
        btnActualiser.Variante = VarianteBouton.Clair
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 22
        enTete.SousTitre = "Consultez et gérez les intervenants et thérapeutes"
        enTete.Text = "Thérapeutes"
        ' 
        ' UC_Therapeutes
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Name = "UC_Therapeutes"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        CType(dgvTherapeutes, ComponentModel.ISupportInitialize).EndInit()
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents dgvTherapeutes As DataGridView
    Friend WithEvents pagTherapeutes As UC_Pagination
    Friend WithEvents colNom As DataGridViewTextBoxColumn
    Friend WithEvents colPrenom As DataGridViewTextBoxColumn
    Friend WithEvents colSpecialite As DataGridViewTextBoxColumn
    Friend WithEvents colTelephone As DataGridViewTextBoxColumn
    Friend WithEvents colEmail As DataGridViewTextBoxColumn
    Friend WithEvents colLocalite As DataGridViewTextBoxColumn
    Friend WithEvents colActif As DataGridViewTextBoxColumn
    Friend WithEvents colIdTherapeute As DataGridViewTextBoxColumn
    Friend WithEvents colCodeTherapeute As DataGridViewTextBoxColumn
    Friend WithEvents pnlTop As AltheaCarte
    Friend WithEvents btnRechercher As AltheaBouton
    Friend WithEvents btnReinitialiserFiltres As AltheaBouton
    Friend WithEvents chkAfficherInactifs As AltheaCase
    Friend WithEvents txtRecherche As AltheaRecherche
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnNouveau As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents btnActiverDesactiver As AltheaBouton
    Friend WithEvents btnSupprimer As AltheaBouton
    Friend WithEvents btnActualiser As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete

End Class
