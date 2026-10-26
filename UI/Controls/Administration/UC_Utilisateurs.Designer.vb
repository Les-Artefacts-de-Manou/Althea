<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_Utilisateurs
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

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Utilisateurs))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        dgvUtilisateurs = New DataGridView()
        colEtat = New DataGridViewTextBoxColumn()
        colLoginUtilisateur = New DataGridViewTextBoxColumn()
        colNomAffichage = New DataGridViewTextBoxColumn()
        colRoleUtilisateur = New DataGridViewTextBoxColumn()
        colDernierLogin = New DataGridViewTextBoxColumn()
        colIdUtilisateur = New DataGridViewTextBoxColumn()
        colCodeUtilisateur = New DataGridViewTextBoxColumn()
        colRoleMaxElevation = New DataGridViewTextBoxColumn()
        colActif = New DataGridViewCheckBoxColumn()
        colMustChangePassword = New DataGridViewCheckBoxColumn()
        colNbEchecsLogin = New DataGridViewTextBoxColumn()
        colCompteVerrouille = New DataGridViewCheckBoxColumn()
        colDateVerrouillage = New DataGridViewTextBoxColumn()
        colDateCreation = New DataGridViewTextBoxColumn()
        colDateModification = New DataGridViewTextBoxColumn()
        pnlTop = New AltheaCarte()
        lblAffiner = New Label()
        btnRechercher = New AltheaBouton()
        btnReinitialiserFiltres = New AltheaBouton()
        chkFiltrerDate = New AltheaCase()
        chkAfficherInactifs = New AltheaCase()
        chkCompteVerrouille = New AltheaCase()
        dtpDernierLoginDepuis = New AltheaDate()
        cboFiltreRole = New AltheaListe()
        txtRechercheUtilisateur = New AltheaRecherche()
        pnlActions = New AltheaBarreActions()
        btnActiverDesactiver = New AltheaBouton()
        btnActualiser = New AltheaBouton()
        btnNouveau = New AltheaBouton()
        btnModifier = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        CType(dgvUtilisateurs, ComponentModel.ISupportInitialize).BeginInit()
        pnlTop.SuspendLayout()
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
        pnlForm.TabIndex = 1
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(dgvUtilisateurs)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16, 16, 16, 8)
        pnlCenter.Size = New Size(965, 624)
        pnlCenter.TabIndex = 21
        ' 
        ' dgvUtilisateurs
        ' 
        dgvUtilisateurs.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvUtilisateurs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvUtilisateurs.Columns.AddRange(New DataGridViewColumn() {colCodeUtilisateur, colEtat, colLoginUtilisateur, colNomAffichage, colRoleUtilisateur, colDernierLogin, colIdUtilisateur, colRoleMaxElevation, colActif, colMustChangePassword, colNbEchecsLogin, colCompteVerrouille, colDateVerrouillage, colDateCreation, colDateModification})
        dgvUtilisateurs.Dock = DockStyle.Fill
        dgvUtilisateurs.Location = New Point(16, 128)
        dgvUtilisateurs.Name = "dgvUtilisateurs"
        dgvUtilisateurs.ReadOnly = True
        dgvUtilisateurs.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvUtilisateurs.Size = New Size(933, 488)
        dgvUtilisateurs.TabIndex = 8
        ' 
        ' colEtat
        ' 
        colEtat.HeaderText = "État"
        colEtat.Name = "colEtat"
        colEtat.ReadOnly = True
        colEtat.Width = 115
        ' 
        ' colLoginUtilisateur
        ' 
        colLoginUtilisateur.HeaderText = "Login"
        colLoginUtilisateur.Name = "colLoginUtilisateur"
        colLoginUtilisateur.ReadOnly = True
        colLoginUtilisateur.Width = 180
        ' 
        ' colNomAffichage
        ' 
        colNomAffichage.HeaderText = "Nom affiché"
        colNomAffichage.Name = "colNomAffichage"
        colNomAffichage.ReadOnly = True
        colNomAffichage.Width = 230
        ' 
        ' colRoleUtilisateur
        ' 
        colRoleUtilisateur.HeaderText = "Rôle"
        colRoleUtilisateur.Name = "colRoleUtilisateur"
        colRoleUtilisateur.ReadOnly = True
        colRoleUtilisateur.Width = 170
        ' 
        ' colDernierLogin
        ' 
        colDernierLogin.HeaderText = "Dernière connexion"
        colDernierLogin.Name = "colDernierLogin"
        colDernierLogin.ReadOnly = True
        colDernierLogin.Width = 150
        ' 
        ' colIdUtilisateur
        ' 
        colIdUtilisateur.HeaderText = "Id utilisateur"
        colIdUtilisateur.Name = "colIdUtilisateur"
        colIdUtilisateur.ReadOnly = True
        colIdUtilisateur.Visible = False
        ' 
        ' colCodeUtilisateur
        ' 
        colCodeUtilisateur.HeaderText = "Code"
        colCodeUtilisateur.Name = "colCodeUtilisateur"
        colCodeUtilisateur.ReadOnly = True
        colCodeUtilisateur.Width = 85
        ' 
        ' colRoleMaxElevation
        ' 
        colRoleMaxElevation.HeaderText = "Rôle max élévation"
        colRoleMaxElevation.Name = "colRoleMaxElevation"
        colRoleMaxElevation.ReadOnly = True
        colRoleMaxElevation.Visible = False
        ' 
        ' colActif
        ' 
        colActif.HeaderText = "Actif"
        colActif.Name = "colActif"
        colActif.ReadOnly = True
        colActif.Visible = False
        ' 
        ' colMustChangePassword
        ' 
        colMustChangePassword.HeaderText = "Must change PW"
        colMustChangePassword.Name = "colMustChangePassword"
        colMustChangePassword.ReadOnly = True
        colMustChangePassword.Visible = False
        ' 
        ' colNbEchecsLogin
        ' 
        colNbEchecsLogin.HeaderText = "Nb échecs"
        colNbEchecsLogin.Name = "colNbEchecsLogin"
        colNbEchecsLogin.ReadOnly = True
        colNbEchecsLogin.Visible = False
        ' 
        ' colCompteVerrouille
        ' 
        colCompteVerrouille.HeaderText = "Compte verrouillé"
        colCompteVerrouille.Name = "colCompteVerrouille"
        colCompteVerrouille.ReadOnly = True
        colCompteVerrouille.Visible = False
        ' 
        ' colDateVerrouillage
        ' 
        colDateVerrouillage.HeaderText = "Date verrouillage"
        colDateVerrouillage.Name = "colDateVerrouillage"
        colDateVerrouillage.ReadOnly = True
        colDateVerrouillage.Visible = False
        ' 
        ' colDateCreation
        ' 
        colDateCreation.HeaderText = "Date création"
        colDateCreation.Name = "colDateCreation"
        colDateCreation.ReadOnly = True
        colDateCreation.Visible = False
        ' 
        ' colDateModification
        ' 
        colDateModification.HeaderText = "Date modification"
        colDateModification.Name = "colDateModification"
        colDateModification.ReadOnly = True
        colDateModification.Visible = False
        ' 
        ' pnlTop
        ' 
        pnlTop.Controls.Add(lblAffiner)
        pnlTop.Controls.Add(btnRechercher)
        pnlTop.Controls.Add(btnReinitialiserFiltres)
        pnlTop.Controls.Add(chkFiltrerDate)
        pnlTop.Controls.Add(chkAfficherInactifs)
        pnlTop.Controls.Add(chkCompteVerrouille)
        pnlTop.Controls.Add(dtpDernierLoginDepuis)
        pnlTop.Controls.Add(cboFiltreRole)
        pnlTop.Controls.Add(txtRechercheUtilisateur)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(14, 10, 14, 10)
        pnlTop.Size = New Size(933, 100)
        pnlTop.TabIndex = 7
        ' 
        ' lblAffiner
        ' 
        lblAffiner.AutoSize = True
        lblAffiner.BackColor = Color.Transparent
        lblAffiner.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        lblAffiner.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAffiner.Location = New Point(14, 61)
        lblAffiner.Name = "lblAffiner"
        lblAffiner.TabIndex = 0
        lblAffiner.Text = "Affiner"
        ' 
        ' btnRechercher
        ' 
        btnRechercher.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnRechercher.Image = CType(resources.GetObject("btnRechercher.Image"), Image)
        btnRechercher.Location = New Point(784, 12)
        btnRechercher.Name = "btnRechercher"
        btnRechercher.Size = New Size(135, 32)
        btnRechercher.TabIndex = 19
        btnRechercher.Tag = "recherche_24_normal"
        btnRechercher.Text = "Rechercher"
        ' 
        ' btnReinitialiserFiltres
        ' 
        btnReinitialiserFiltres.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnReinitialiserFiltres.Image = CType(resources.GetObject("btnReinitialiserFiltres.Image"), Image)
        btnReinitialiserFiltres.Location = New Point(644, 12)
        btnReinitialiserFiltres.Name = "btnReinitialiserFiltres"
        btnReinitialiserFiltres.Size = New Size(130, 32)
        btnReinitialiserFiltres.TabIndex = 21
        btnReinitialiserFiltres.Tag = "reinitialiser_24_normal"
        btnReinitialiserFiltres.Text = "Réinitialiser"
        btnReinitialiserFiltres.Variante = VarianteBouton.Clair
        ' 
        ' chkFiltrerDate
        ' 
        chkFiltrerDate.AutoSize = False
        chkFiltrerDate.Location = New Point(80, 58)
        chkFiltrerDate.Name = "chkFiltrerDate"
        chkFiltrerDate.Size = New Size(205, 24)
        chkFiltrerDate.TabIndex = 20
        chkFiltrerDate.Text = "Dernière connexion depuis"
        ' 
        ' chkAfficherInactifs
        ' 
        chkAfficherInactifs.AutoSize = False
        chkAfficherInactifs.Location = New Point(690, 58)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(170, 24)
        chkAfficherInactifs.TabIndex = 17
        chkAfficherInactifs.Text = "Afficher les inactifs"
        ' 
        ' chkCompteVerrouille
        ' 
        chkCompteVerrouille.AutoSize = False
        chkCompteVerrouille.Location = New Point(440, 58)
        chkCompteVerrouille.Name = "chkCompteVerrouille"
        chkCompteVerrouille.Size = New Size(230, 24)
        chkCompteVerrouille.TabIndex = 18
        chkCompteVerrouille.Text = "Comptes verrouillés seulement"
        ' 
        ' dtpDernierLoginDepuis
        ' 
        dtpDernierLoginDepuis.Enabled = False
        dtpDernierLoginDepuis.Font = New Font("Calibri", 11F)
        dtpDernierLoginDepuis.Location = New Point(292, 55)
        dtpDernierLoginDepuis.Name = "dtpDernierLoginDepuis"
        dtpDernierLoginDepuis.Size = New Size(130, 28)
        dtpDernierLoginDepuis.TabIndex = 14
        ' 
        ' cboFiltreRole
        ' 
        cboFiltreRole.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        cboFiltreRole.Etiquette = "Rôle"
        cboFiltreRole.Location = New Point(386, 13)
        cboFiltreRole.Name = "cboFiltreRole"
        cboFiltreRole.Size = New Size(200, 30)
        cboFiltreRole.TabIndex = 13
        ' 
        ' txtRechercheUtilisateur
        ' 
        txtRechercheUtilisateur.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtRechercheUtilisateur.Location = New Point(14, 12)
        txtRechercheUtilisateur.Name = "txtRechercheUtilisateur"
        txtRechercheUtilisateur.Size = New Size(360, 32)
        txtRechercheUtilisateur.TabIndex = 2
        txtRechercheUtilisateur.TexteAide = "Login ou nom affiché…"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnActiverDesactiver)
        pnlActions.Controls.Add(btnActualiser)
        pnlActions.Controls.Add(btnNouveau)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 698)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(965, 60)
        pnlActions.TabIndex = 20
        ' 
        ' btnActiverDesactiver
        ' 
        btnActiverDesactiver.Image = CType(resources.GetObject("btnActiverDesactiver.Image"), Image)
        btnActiverDesactiver.Location = New Point(506, 10)
        btnActiverDesactiver.Name = "btnActiverDesactiver"
        btnActiverDesactiver.Size = New Size(130, 40)
        btnActiverDesactiver.TabIndex = 13
        btnActiverDesactiver.Tag = "activer_normal"
        btnActiverDesactiver.Text = "Désactiver"
        ' 
        ' btnActualiser
        ' 
        btnActualiser.Image = CType(resources.GetObject("btnActualiser.Image"), Image)
        btnActualiser.Location = New Point(646, 10)
        btnActualiser.Name = "btnActualiser"
        btnActualiser.Size = New Size(127, 40)
        btnActualiser.TabIndex = 11
        btnActualiser.Tag = "actualiser_normal"
        btnActualiser.Text = "Actualiser"
        btnActualiser.Variante = VarianteBouton.Clair
        ' 
        ' btnNouveau
        ' 
        btnNouveau.Image = CType(resources.GetObject("btnNouveau.Image"), Image)
        btnNouveau.Location = New Point(247, 10)
        btnNouveau.Name = "btnNouveau"
        btnNouveau.Size = New Size(120, 40)
        btnNouveau.TabIndex = 8
        btnNouveau.Tag = "nouveau_normal"
        btnNouveau.Text = "Nouveau"
        ' 
        ' btnModifier
        ' 
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(377, 10)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(119, 40)
        btnModifier.TabIndex = 7
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
        enTete.TabIndex = 5
        enTete.SousTitre = "Consultez et gérez les comptes utilisateurs, leurs rôles et leurs états de sécurité."
        enTete.Text = "Utilisateurs"
        ' 
        ' UC_Utilisateurs
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_Utilisateurs"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        CType(dgvUtilisateurs, ComponentModel.ISupportInitialize).EndInit()
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents lblAffiner As Label
    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnNouveau As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents Button2 As Button
    Friend WithEvents btnActiverDesactiver As AltheaBouton
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents pnlTop As AltheaCarte
    Friend WithEvents dgvUtilisateurs As DataGridView
    Friend WithEvents txtRechercheUtilisateur As AltheaRecherche
    Friend WithEvents cboFiltreRole As AltheaListe
    Friend WithEvents dtpDernierLoginDepuis As AltheaDate
    Friend WithEvents chkAfficherInactifs As AltheaCase
    Friend WithEvents chkCompteVerrouille As AltheaCase
    Friend WithEvents chkFiltrerDate As AltheaCase
    Friend WithEvents btnRechercher As AltheaBouton
    Friend WithEvents btnReinitialiserFiltres As AltheaBouton
    Friend WithEvents btnActualiser As AltheaBouton
    Friend WithEvents colEtat As DataGridViewTextBoxColumn
    Friend WithEvents colLoginUtilisateur As DataGridViewTextBoxColumn
    Friend WithEvents colNomAffichage As DataGridViewTextBoxColumn
    Friend WithEvents colRoleUtilisateur As DataGridViewTextBoxColumn
    Friend WithEvents colDernierLogin As DataGridViewTextBoxColumn
    Friend WithEvents colIdUtilisateur As DataGridViewTextBoxColumn
    Friend WithEvents colCodeUtilisateur As DataGridViewTextBoxColumn
    Friend WithEvents colRoleMaxElevation As DataGridViewTextBoxColumn
    Friend WithEvents colActif As DataGridViewCheckBoxColumn
    Friend WithEvents colMustChangePassword As DataGridViewCheckBoxColumn
    Friend WithEvents colNbEchecsLogin As DataGridViewTextBoxColumn
    Friend WithEvents colCompteVerrouille As DataGridViewCheckBoxColumn
    Friend WithEvents colDateVerrouillage As DataGridViewTextBoxColumn
    Friend WithEvents colDateCreation As DataGridViewTextBoxColumn
    Friend WithEvents colDateModification As DataGridViewTextBoxColumn

End Class
