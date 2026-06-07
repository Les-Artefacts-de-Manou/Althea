<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UtilisateurEdition
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UtilisateurEdition))
        pnlForm = New Panel()
        pnlAudit = New Panel()
        tblAudit = New TableLayoutPanel()
        txtDateModification = New TextBox()
        lblDateModification = New Label()
        txtDateCreation = New TextBox()
        lblDateCreation = New Label()
        lblAudit = New Label()
        pnlSecurite = New Panel()
        TableLayoutPanel1 = New TableLayoutPanel()
        lblNbEchecsLogin = New Label()
        lblSecurite = New Label()
        txtNbEchecsLogin = New TextBox()
        chkCompteVerrouille = New CheckBox()
        txtDateVerrouillage = New TextBox()
        lblDateVerrouillage = New Label()
        txtDernierLogin = New TextBox()
        lblDernierLogin = New Label()
        pnlDroits = New Panel()
        tblDroits = New TableLayoutPanel()
        cboRoleMaxElevation = New ComboBox()
        cboRoleUtilisateur = New ComboBox()
        lblRoleUtilisateur = New Label()
        lblDroits = New Label()
        lblRoleMaxElevation = New Label()
        chkActif = New CheckBox()
        chkMustChangePassword = New CheckBox()
        pnlCenter = New Panel()
        tblIdentite = New TableLayoutPanel()
        txtNomAffichage = New TextBox()
        txtLoginUtilisateur = New TextBox()
        lblLoginUtilisateur = New Label()
        txtCodeUtilisateur = New TextBox()
        lblCodeUtilisateur = New Label()
        lblIdentite = New Label()
        lblNomAffichage = New Label()
        pnlTop = New Panel()
        lblTop = New Label()
        pnlActions = New Panel()
        btnResetPassword = New Button()
        btnDeverrouiller = New Button()
        btnAnnuler = New Button()
        btnEnregistrer = New Button()
        pnlTitre = New Panel()
        lblTitreForm = New Label()
        ttMain = New ToolTip(components)
        errProvider = New ErrorProvider(components)
        pnlForm.SuspendLayout()
        pnlAudit.SuspendLayout()
        tblAudit.SuspendLayout()
        pnlSecurite.SuspendLayout()
        TableLayoutPanel1.SuspendLayout()
        pnlDroits.SuspendLayout()
        tblDroits.SuspendLayout()
        pnlCenter.SuspendLayout()
        tblIdentite.SuspendLayout()
        pnlTop.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTitre.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        pnlForm.BackgroundImage = My.Resources.Resources.Fond_1000x800_Feuille4
        pnlForm.BackgroundImageLayout = ImageLayout.Stretch
        pnlForm.Controls.Add(pnlAudit)
        pnlForm.Controls.Add(pnlSecurite)
        pnlForm.Controls.Add(pnlDroits)
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlTop)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(pnlTitre)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Font = New Font("Calibri", 9F)
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Size = New Size(884, 711)
        pnlForm.TabIndex = 0
        ' 
        ' pnlAudit
        ' 
        pnlAudit.Controls.Add(tblAudit)
        pnlAudit.Location = New Point(454, 381)
        pnlAudit.Name = "pnlAudit"
        pnlAudit.Size = New Size(422, 264)
        pnlAudit.TabIndex = 24
        ' 
        ' tblAudit
        ' 
        tblAudit.BackColor = Color.Transparent
        tblAudit.ColumnCount = 2
        tblAudit.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35F))
        tblAudit.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65F))
        tblAudit.Controls.Add(txtDateModification, 1, 2)
        tblAudit.Controls.Add(lblDateModification, 0, 2)
        tblAudit.Controls.Add(txtDateCreation, 1, 1)
        tblAudit.Controls.Add(lblDateCreation, 0, 1)
        tblAudit.Controls.Add(lblAudit, 0, 0)
        tblAudit.Dock = DockStyle.Fill
        tblAudit.Location = New Point(0, 0)
        tblAudit.Name = "tblAudit"
        tblAudit.Padding = New Padding(8)
        tblAudit.RowCount = 3
        tblAudit.RowStyles.Add(New RowStyle(SizeType.Percent, 10F))
        tblAudit.RowStyles.Add(New RowStyle(SizeType.Percent, 45F))
        tblAudit.RowStyles.Add(New RowStyle(SizeType.Percent, 45F))
        tblAudit.Size = New Size(422, 264)
        tblAudit.TabIndex = 1
        ' 
        ' txtDateModification
        ' 
        txtDateModification.Anchor = AnchorStyles.Left
        txtDateModification.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtDateModification.BorderStyle = BorderStyle.None
        txtDateModification.Font = New Font("Calibri", 10F)
        txtDateModification.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        txtDateModification.Location = New Point(153, 191)
        txtDateModification.Name = "txtDateModification"
        txtDateModification.ReadOnly = True
        txtDateModification.Size = New Size(195, 17)
        txtDateModification.TabIndex = 28
        ' 
        ' lblDateModification
        ' 
        lblDateModification.Anchor = AnchorStyles.Left
        lblDateModification.AutoSize = True
        lblDateModification.Font = New Font("Calibri", 10F)
        lblDateModification.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateModification.Location = New Point(11, 191)
        lblDateModification.Name = "lblDateModification"
        lblDateModification.Size = New Size(109, 17)
        lblDateModification.TabIndex = 27
        lblDateModification.Text = "Date modification"
        ' 
        ' txtDateCreation
        ' 
        txtDateCreation.Anchor = AnchorStyles.Left
        txtDateCreation.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtDateCreation.BorderStyle = BorderStyle.None
        txtDateCreation.Font = New Font("Calibri", 10F)
        txtDateCreation.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        txtDateCreation.Location = New Point(153, 79)
        txtDateCreation.Name = "txtDateCreation"
        txtDateCreation.ReadOnly = True
        txtDateCreation.Size = New Size(195, 17)
        txtDateCreation.TabIndex = 26
        ' 
        ' lblDateCreation
        ' 
        lblDateCreation.Anchor = AnchorStyles.Left
        lblDateCreation.AutoSize = True
        lblDateCreation.Font = New Font("Calibri", 10F)
        lblDateCreation.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateCreation.Location = New Point(11, 79)
        lblDateCreation.Name = "lblDateCreation"
        lblDateCreation.Size = New Size(86, 17)
        lblDateCreation.TabIndex = 25
        lblDateCreation.Text = "Date création"
        ' 
        ' lblAudit
        ' 
        lblAudit.AutoSize = True
        tblAudit.SetColumnSpan(lblAudit, 2)
        lblAudit.Font = New Font("Calibri", 14F, FontStyle.Bold)
        lblAudit.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAudit.Location = New Point(11, 8)
        lblAudit.Name = "lblAudit"
        lblAudit.Size = New Size(54, 23)
        lblAudit.TabIndex = 2
        lblAudit.Text = "Audit"
        ' 
        ' pnlSecurite
        ' 
        pnlSecurite.Controls.Add(TableLayoutPanel1)
        pnlSecurite.Location = New Point(8, 381)
        pnlSecurite.Name = "pnlSecurite"
        pnlSecurite.Size = New Size(435, 264)
        pnlSecurite.TabIndex = 23
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.BackColor = Color.Transparent
        TableLayoutPanel1.ColumnCount = 2
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65F))
        TableLayoutPanel1.Controls.Add(lblNbEchecsLogin, 0, 1)
        TableLayoutPanel1.Controls.Add(lblSecurite, 0, 0)
        TableLayoutPanel1.Controls.Add(txtNbEchecsLogin, 1, 1)
        TableLayoutPanel1.Controls.Add(chkCompteVerrouille, 1, 2)
        TableLayoutPanel1.Controls.Add(txtDateVerrouillage, 1, 3)
        TableLayoutPanel1.Controls.Add(lblDateVerrouillage, 0, 3)
        TableLayoutPanel1.Controls.Add(txtDernierLogin, 1, 4)
        TableLayoutPanel1.Controls.Add(lblDernierLogin, 0, 4)
        TableLayoutPanel1.Dock = DockStyle.Fill
        TableLayoutPanel1.Location = New Point(0, 0)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.Padding = New Padding(8)
        TableLayoutPanel1.RowCount = 5
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 10.2040815F))
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 22.44898F))
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 22.44898F))
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 22.44898F))
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 22.44898F))
        TableLayoutPanel1.Size = New Size(435, 264)
        TableLayoutPanel1.TabIndex = 2
        ' 
        ' lblNbEchecsLogin
        ' 
        lblNbEchecsLogin.Anchor = AnchorStyles.Left
        lblNbEchecsLogin.AutoSize = True
        lblNbEchecsLogin.Font = New Font("Calibri", 10F)
        lblNbEchecsLogin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNbEchecsLogin.Location = New Point(11, 52)
        lblNbEchecsLogin.Name = "lblNbEchecsLogin"
        lblNbEchecsLogin.Size = New Size(98, 17)
        lblNbEchecsLogin.TabIndex = 24
        lblNbEchecsLogin.Text = "Nb Echecs Login"
        ' 
        ' lblSecurite
        ' 
        lblSecurite.AutoSize = True
        TableLayoutPanel1.SetColumnSpan(lblSecurite, 2)
        lblSecurite.Font = New Font("Calibri", 14F, FontStyle.Bold)
        lblSecurite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblSecurite.Location = New Point(11, 8)
        lblSecurite.Name = "lblSecurite"
        lblSecurite.Size = New Size(76, 23)
        lblSecurite.TabIndex = 1
        lblSecurite.Text = "Sécurité"
        ' 
        ' txtNbEchecsLogin
        ' 
        txtNbEchecsLogin.Anchor = AnchorStyles.Left
        txtNbEchecsLogin.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNbEchecsLogin.BorderStyle = BorderStyle.None
        txtNbEchecsLogin.Font = New Font("Calibri", 10F)
        txtNbEchecsLogin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        txtNbEchecsLogin.Location = New Point(157, 52)
        txtNbEchecsLogin.Name = "txtNbEchecsLogin"
        txtNbEchecsLogin.ReadOnly = True
        txtNbEchecsLogin.Size = New Size(75, 17)
        txtNbEchecsLogin.TabIndex = 23
        ' 
        ' chkCompteVerrouille
        ' 
        chkCompteVerrouille.Anchor = AnchorStyles.Left
        chkCompteVerrouille.AutoSize = True
        chkCompteVerrouille.Enabled = False
        chkCompteVerrouille.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkCompteVerrouille.Location = New Point(157, 106)
        chkCompteVerrouille.Name = "chkCompteVerrouille"
        chkCompteVerrouille.Size = New Size(124, 18)
        chkCompteVerrouille.TabIndex = 25
        chkCompteVerrouille.Text = "Compte Verrouillé"
        chkCompteVerrouille.TextAlign = ContentAlignment.MiddleRight
        chkCompteVerrouille.UseVisualStyleBackColor = True
        ' 
        ' txtDateVerrouillage
        ' 
        txtDateVerrouillage.Anchor = AnchorStyles.Left
        txtDateVerrouillage.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtDateVerrouillage.BorderStyle = BorderStyle.None
        txtDateVerrouillage.Font = New Font("Calibri", 10F)
        txtDateVerrouillage.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        txtDateVerrouillage.Location = New Point(157, 162)
        txtDateVerrouillage.Name = "txtDateVerrouillage"
        txtDateVerrouillage.ReadOnly = True
        txtDateVerrouillage.Size = New Size(170, 17)
        txtDateVerrouillage.TabIndex = 26
        ' 
        ' lblDateVerrouillage
        ' 
        lblDateVerrouillage.Anchor = AnchorStyles.Left
        lblDateVerrouillage.AutoSize = True
        lblDateVerrouillage.Font = New Font("Calibri", 10F)
        lblDateVerrouillage.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateVerrouillage.Location = New Point(11, 162)
        lblDateVerrouillage.Name = "lblDateVerrouillage"
        lblDateVerrouillage.Size = New Size(107, 17)
        lblDateVerrouillage.TabIndex = 27
        lblDateVerrouillage.Text = "Date Verrouillage"
        ' 
        ' txtDernierLogin
        ' 
        txtDernierLogin.Anchor = AnchorStyles.Left
        txtDernierLogin.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtDernierLogin.BorderStyle = BorderStyle.None
        txtDernierLogin.Font = New Font("Calibri", 10F)
        txtDernierLogin.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        txtDernierLogin.Location = New Point(157, 218)
        txtDernierLogin.Name = "txtDernierLogin"
        txtDernierLogin.ReadOnly = True
        txtDernierLogin.Size = New Size(170, 17)
        txtDernierLogin.TabIndex = 28
        ' 
        ' lblDernierLogin
        ' 
        lblDernierLogin.Anchor = AnchorStyles.Left
        lblDernierLogin.AutoSize = True
        lblDernierLogin.Font = New Font("Calibri", 10F)
        lblDernierLogin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDernierLogin.Location = New Point(11, 218)
        lblDernierLogin.Name = "lblDernierLogin"
        lblDernierLogin.Size = New Size(81, 17)
        lblDernierLogin.TabIndex = 29
        lblDernierLogin.Text = "Dernier login"
        ' 
        ' pnlDroits
        ' 
        pnlDroits.Controls.Add(tblDroits)
        pnlDroits.Location = New Point(454, 106)
        pnlDroits.Name = "pnlDroits"
        pnlDroits.Size = New Size(422, 264)
        pnlDroits.TabIndex = 22
        ' 
        ' tblDroits
        ' 
        tblDroits.BackColor = Color.Transparent
        tblDroits.ColumnCount = 2
        tblDroits.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35F))
        tblDroits.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65F))
        tblDroits.Controls.Add(cboRoleMaxElevation, 1, 2)
        tblDroits.Controls.Add(cboRoleUtilisateur, 1, 1)
        tblDroits.Controls.Add(lblRoleUtilisateur, 0, 1)
        tblDroits.Controls.Add(lblDroits, 0, 0)
        tblDroits.Controls.Add(lblRoleMaxElevation, 0, 2)
        tblDroits.Controls.Add(chkActif, 0, 3)
        tblDroits.Controls.Add(chkMustChangePassword, 1, 3)
        tblDroits.Dock = DockStyle.Fill
        tblDroits.Location = New Point(0, 0)
        tblDroits.Name = "tblDroits"
        tblDroits.Padding = New Padding(8)
        tblDroits.RowCount = 4
        tblDroits.RowStyles.Add(New RowStyle(SizeType.Percent, 10F))
        tblDroits.RowStyles.Add(New RowStyle(SizeType.Percent, 30F))
        tblDroits.RowStyles.Add(New RowStyle(SizeType.Percent, 30F))
        tblDroits.RowStyles.Add(New RowStyle(SizeType.Percent, 30F))
        tblDroits.Size = New Size(422, 264)
        tblDroits.TabIndex = 1
        ' 
        ' cboRoleMaxElevation
        ' 
        cboRoleMaxElevation.Anchor = AnchorStyles.Left
        cboRoleMaxElevation.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboRoleMaxElevation.DropDownStyle = ComboBoxStyle.DropDownList
        cboRoleMaxElevation.FlatStyle = FlatStyle.Flat
        cboRoleMaxElevation.Font = New Font("Calibri", 9F)
        cboRoleMaxElevation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboRoleMaxElevation.FormattingEnabled = True
        cboRoleMaxElevation.Location = New Point(153, 132)
        cboRoleMaxElevation.Name = "cboRoleMaxElevation"
        cboRoleMaxElevation.Size = New Size(195, 22)
        cboRoleMaxElevation.TabIndex = 4
        ' 
        ' cboRoleUtilisateur
        ' 
        cboRoleUtilisateur.Anchor = AnchorStyles.Left
        cboRoleUtilisateur.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboRoleUtilisateur.DropDownStyle = ComboBoxStyle.DropDownList
        cboRoleUtilisateur.FlatStyle = FlatStyle.Flat
        cboRoleUtilisateur.Font = New Font("Calibri", 9F)
        cboRoleUtilisateur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboRoleUtilisateur.FormattingEnabled = True
        cboRoleUtilisateur.Location = New Point(153, 58)
        cboRoleUtilisateur.Name = "cboRoleUtilisateur"
        cboRoleUtilisateur.Size = New Size(195, 22)
        cboRoleUtilisateur.TabIndex = 3
        ' 
        ' lblRoleUtilisateur
        ' 
        lblRoleUtilisateur.Anchor = AnchorStyles.Left
        lblRoleUtilisateur.AutoSize = True
        lblRoleUtilisateur.Font = New Font("Calibri", 10F)
        lblRoleUtilisateur.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRoleUtilisateur.Location = New Point(11, 60)
        lblRoleUtilisateur.Name = "lblRoleUtilisateur"
        lblRoleUtilisateur.Size = New Size(95, 17)
        lblRoleUtilisateur.TabIndex = 19
        lblRoleUtilisateur.Text = "Rôle Utilisateur"
        ' 
        ' lblDroits
        ' 
        lblDroits.AutoSize = True
        tblDroits.SetColumnSpan(lblDroits, 2)
        lblDroits.Font = New Font("Calibri", 14F, FontStyle.Bold)
        lblDroits.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDroits.Location = New Point(11, 8)
        lblDroits.Name = "lblDroits"
        lblDroits.Size = New Size(59, 23)
        lblDroits.TabIndex = 1
        lblDroits.Text = "Droits"
        ' 
        ' lblRoleMaxElevation
        ' 
        lblRoleMaxElevation.Anchor = AnchorStyles.Left
        lblRoleMaxElevation.AutoSize = True
        lblRoleMaxElevation.Font = New Font("Calibri", 10F)
        lblRoleMaxElevation.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRoleMaxElevation.Location = New Point(11, 134)
        lblRoleMaxElevation.Name = "lblRoleMaxElevation"
        lblRoleMaxElevation.Size = New Size(116, 17)
        lblRoleMaxElevation.TabIndex = 20
        lblRoleMaxElevation.Text = "Rôle Max Elevation"
        ' 
        ' chkActif
        ' 
        chkActif.Anchor = AnchorStyles.Left
        chkActif.AutoSize = True
        chkActif.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkActif.Location = New Point(11, 209)
        chkActif.Name = "chkActif"
        chkActif.Size = New Size(49, 18)
        chkActif.TabIndex = 5
        chkActif.Text = "Actif"
        chkActif.TextAlign = ContentAlignment.MiddleRight
        chkActif.UseVisualStyleBackColor = True
        ' 
        ' chkMustChangePassword
        ' 
        chkMustChangePassword.Anchor = AnchorStyles.Left
        chkMustChangePassword.AutoSize = True
        chkMustChangePassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkMustChangePassword.Location = New Point(153, 209)
        chkMustChangePassword.Name = "chkMustChangePassword"
        chkMustChangePassword.Size = New Size(163, 18)
        chkMustChangePassword.TabIndex = 22
        chkMustChangePassword.Text = "Forcer le changement PW"
        chkMustChangePassword.TextAlign = ContentAlignment.MiddleRight
        chkMustChangePassword.UseVisualStyleBackColor = True
        ' 
        ' pnlCenter
        ' 
        pnlCenter.Controls.Add(tblIdentite)
        pnlCenter.Location = New Point(8, 106)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Size = New Size(435, 264)
        pnlCenter.TabIndex = 21
        ' 
        ' tblIdentite
        ' 
        tblIdentite.BackColor = Color.Transparent
        tblIdentite.ColumnCount = 2
        tblIdentite.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35F))
        tblIdentite.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65F))
        tblIdentite.Controls.Add(txtNomAffichage, 1, 3)
        tblIdentite.Controls.Add(txtLoginUtilisateur, 1, 2)
        tblIdentite.Controls.Add(lblLoginUtilisateur, 0, 2)
        tblIdentite.Controls.Add(txtCodeUtilisateur, 1, 1)
        tblIdentite.Controls.Add(lblCodeUtilisateur, 0, 1)
        tblIdentite.Controls.Add(lblIdentite, 0, 0)
        tblIdentite.Controls.Add(lblNomAffichage, 0, 3)
        tblIdentite.Dock = DockStyle.Fill
        tblIdentite.Location = New Point(0, 0)
        tblIdentite.Name = "tblIdentite"
        tblIdentite.Padding = New Padding(8)
        tblIdentite.RowCount = 4
        tblIdentite.RowStyles.Add(New RowStyle(SizeType.Percent, 10F))
        tblIdentite.RowStyles.Add(New RowStyle(SizeType.Percent, 30F))
        tblIdentite.RowStyles.Add(New RowStyle(SizeType.Percent, 30F))
        tblIdentite.RowStyles.Add(New RowStyle(SizeType.Percent, 30F))
        tblIdentite.Size = New Size(435, 264)
        tblIdentite.TabIndex = 0
        ' 
        ' txtNomAffichage
        ' 
        txtNomAffichage.Anchor = AnchorStyles.Left
        txtNomAffichage.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNomAffichage.Font = New Font("Calibri", 10F)
        txtNomAffichage.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNomAffichage.Location = New Point(157, 206)
        txtNomAffichage.Name = "txtNomAffichage"
        txtNomAffichage.Size = New Size(267, 24)
        txtNomAffichage.TabIndex = 1
        ' 
        ' txtLoginUtilisateur
        ' 
        txtLoginUtilisateur.Anchor = AnchorStyles.Left
        txtLoginUtilisateur.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtLoginUtilisateur.Font = New Font("Calibri", 10F)
        txtLoginUtilisateur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLoginUtilisateur.Location = New Point(157, 131)
        txtLoginUtilisateur.Name = "txtLoginUtilisateur"
        txtLoginUtilisateur.Size = New Size(267, 24)
        txtLoginUtilisateur.TabIndex = 0
        ' 
        ' lblLoginUtilisateur
        ' 
        lblLoginUtilisateur.Anchor = AnchorStyles.Left
        lblLoginUtilisateur.AutoSize = True
        lblLoginUtilisateur.Font = New Font("Calibri", 10F)
        lblLoginUtilisateur.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLoginUtilisateur.Location = New Point(11, 134)
        lblLoginUtilisateur.Name = "lblLoginUtilisateur"
        lblLoginUtilisateur.Size = New Size(100, 17)
        lblLoginUtilisateur.TabIndex = 20
        lblLoginUtilisateur.Text = "Login Utilisateur"
        ' 
        ' txtCodeUtilisateur
        ' 
        txtCodeUtilisateur.Anchor = AnchorStyles.Left
        txtCodeUtilisateur.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCodeUtilisateur.BorderStyle = BorderStyle.None
        txtCodeUtilisateur.Font = New Font("Calibri", 10F)
        txtCodeUtilisateur.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        txtCodeUtilisateur.Location = New Point(157, 60)
        txtCodeUtilisateur.Name = "txtCodeUtilisateur"
        txtCodeUtilisateur.ReadOnly = True
        txtCodeUtilisateur.Size = New Size(128, 17)
        txtCodeUtilisateur.TabIndex = 19
        ' 
        ' lblCodeUtilisateur
        ' 
        lblCodeUtilisateur.Anchor = AnchorStyles.Left
        lblCodeUtilisateur.AutoSize = True
        lblCodeUtilisateur.Font = New Font("Calibri", 10F)
        lblCodeUtilisateur.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCodeUtilisateur.Location = New Point(11, 60)
        lblCodeUtilisateur.Name = "lblCodeUtilisateur"
        lblCodeUtilisateur.Size = New Size(98, 17)
        lblCodeUtilisateur.TabIndex = 18
        lblCodeUtilisateur.Text = "Code Utilisateur"
        ' 
        ' lblIdentite
        ' 
        lblIdentite.AutoSize = True
        tblIdentite.SetColumnSpan(lblIdentite, 2)
        lblIdentite.Font = New Font("Calibri", 14F, FontStyle.Bold)
        lblIdentite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblIdentite.Location = New Point(11, 8)
        lblIdentite.Name = "lblIdentite"
        lblIdentite.Size = New Size(73, 23)
        lblIdentite.TabIndex = 0
        lblIdentite.Text = "Identité"
        ' 
        ' lblNomAffichage
        ' 
        lblNomAffichage.Anchor = AnchorStyles.Left
        lblNomAffichage.AutoSize = True
        lblNomAffichage.Font = New Font("Calibri", 10F)
        lblNomAffichage.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNomAffichage.Location = New Point(11, 209)
        lblNomAffichage.Name = "lblNomAffichage"
        lblNomAffichage.Size = New Size(91, 17)
        lblNomAffichage.TabIndex = 21
        lblNomAffichage.Text = "Nom Affichage"
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.Transparent
        pnlTop.Controls.Add(lblTop)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(0, 58)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(8)
        pnlTop.Size = New Size(884, 42)
        pnlTop.TabIndex = 20
        ' 
        ' lblTop
        ' 
        lblTop.Dock = DockStyle.Fill
        lblTop.Font = New Font("Calibri", 10F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(8, 8)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(868, 26)
        lblTop.TabIndex = 0
        lblTop.Text = "Informations utilisateurs"
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.Transparent
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.Fixed3D
        pnlActions.Controls.Add(btnResetPassword)
        pnlActions.Controls.Add(btnDeverrouiller)
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 651)
        pnlActions.Name = "pnlActions"
        pnlActions.Padding = New Padding(8)
        pnlActions.Size = New Size(884, 60)
        pnlActions.TabIndex = 19
        ' 
        ' btnResetPassword
        ' 
        btnResetPassword.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnResetPassword.BackgroundImageLayout = ImageLayout.Center
        btnResetPassword.FlatAppearance.BorderSize = 0
        btnResetPassword.FlatStyle = FlatStyle.Flat
        btnResetPassword.ForeColor = Color.White
        btnResetPassword.Image = CType(resources.GetObject("btnResetPassword.Image"), Image)
        btnResetPassword.ImageAlign = ContentAlignment.MiddleLeft
        btnResetPassword.Location = New Point(566, 9)
        btnResetPassword.Name = "btnResetPassword"
        btnResetPassword.Size = New Size(112, 40)
        btnResetPassword.TabIndex = 18
        btnResetPassword.Tag = "reset_password_normal"
        btnResetPassword.Text = "Reset PW"
        btnResetPassword.TextAlign = ContentAlignment.MiddleLeft
        btnResetPassword.TextImageRelation = TextImageRelation.ImageBeforeText
        btnResetPassword.UseVisualStyleBackColor = False
        ' 
        ' btnDeverrouiller
        ' 
        btnDeverrouiller.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnDeverrouiller.BackgroundImageLayout = ImageLayout.Center
        btnDeverrouiller.FlatAppearance.BorderSize = 0
        btnDeverrouiller.FlatStyle = FlatStyle.Flat
        btnDeverrouiller.ForeColor = Color.White
        btnDeverrouiller.Image = CType(resources.GetObject("btnDeverrouiller.Image"), Image)
        btnDeverrouiller.ImageAlign = ContentAlignment.MiddleLeft
        btnDeverrouiller.Location = New Point(433, 9)
        btnDeverrouiller.Name = "btnDeverrouiller"
        btnDeverrouiller.Size = New Size(130, 40)
        btnDeverrouiller.TabIndex = 17
        btnDeverrouiller.Tag = "Verrouiller_normal"
        btnDeverrouiller.Text = "Déverrouiller"
        btnDeverrouiller.TextAlign = ContentAlignment.MiddleLeft
        btnDeverrouiller.TextImageRelation = TextImageRelation.ImageBeforeText
        btnDeverrouiller.UseVisualStyleBackColor = False
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAnnuler.BackgroundImageLayout = ImageLayout.Center
        btnAnnuler.FlatAppearance.BorderSize = 0
        btnAnnuler.FlatStyle = FlatStyle.Flat
        btnAnnuler.ForeColor = Color.White
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.ImageAlign = ContentAlignment.MiddleLeft
        btnAnnuler.Location = New Point(318, 9)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(112, 40)
        btnAnnuler.TabIndex = 16
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.TextAlign = ContentAlignment.MiddleLeft
        btnAnnuler.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAnnuler.UseVisualStyleBackColor = False
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnEnregistrer.BackgroundImageLayout = ImageLayout.Center
        btnEnregistrer.FlatAppearance.BorderSize = 0
        btnEnregistrer.FlatStyle = FlatStyle.Flat
        btnEnregistrer.ForeColor = Color.White
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.ImageAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.Location = New Point(203, 9)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(112, 40)
        btnEnregistrer.TabIndex = 15
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.TextAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEnregistrer.UseVisualStyleBackColor = False
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(0, 0)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(884, 58)
        pnlTitre.TabIndex = 4
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.Anchor = AnchorStyles.Left
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 15F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(42, 9)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(8, 4, 8, 4)
        lblTitreForm.Size = New Size(224, 32)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Gestion des utilisateurs"
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' UtilisateurEdition
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(884, 711)
        Controls.Add(pnlForm)
        Name = "UtilisateurEdition"
        StartPosition = FormStartPosition.CenterParent
        Text = "Edition utilisateurs"
        pnlForm.ResumeLayout(False)
        pnlAudit.ResumeLayout(False)
        tblAudit.ResumeLayout(False)
        tblAudit.PerformLayout()
        pnlSecurite.ResumeLayout(False)
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        pnlDroits.ResumeLayout(False)
        tblDroits.ResumeLayout(False)
        tblDroits.PerformLayout()
        pnlCenter.ResumeLayout(False)
        tblIdentite.ResumeLayout(False)
        tblIdentite.PerformLayout()
        pnlTop.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnResetPassword As Button
    Friend WithEvents btnDeverrouiller As Button
    Friend WithEvents btnAnnuler As Button
    Friend WithEvents btnEnregistrer As Button
    Friend WithEvents pnlTop As Panel
    Friend WithEvents lblTop As Label
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents pnlDroits As Panel
    Friend WithEvents pnlSecurite As Panel
    Friend WithEvents pnlAudit As Panel
    Friend WithEvents tblIdentite As TableLayoutPanel
    Friend WithEvents tblDroits As TableLayoutPanel
    Friend WithEvents tblAudit As TableLayoutPanel
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents lblIdentite As Label
    Friend WithEvents lblSecurite As Label
    Friend WithEvents lblDroits As Label
    Friend WithEvents lblAudit As Label
    Friend WithEvents lblCodeUtilisateur As Label
    Friend WithEvents txtCodeUtilisateur As TextBox
    Friend WithEvents lblLoginUtilisateur As Label
    Friend WithEvents lblNomAffichage As Label
    Friend WithEvents txtLoginUtilisateur As TextBox
    Friend WithEvents txtNomAffichage As TextBox
    Friend WithEvents lblRoleUtilisateur As Label
    Friend WithEvents lblRoleMaxElevation As Label
    Friend WithEvents chkActif As CheckBox
    Friend WithEvents chkMustChangePassword As CheckBox
    Friend WithEvents cboRoleUtilisateur As ComboBox
    Friend WithEvents cboRoleMaxElevation As ComboBox
    Friend WithEvents lblNbEchecsLogin As Label
    Friend WithEvents txtNbEchecsLogin As TextBox
    Friend WithEvents chkCompteVerrouille As CheckBox
    Friend WithEvents txtDateVerrouillage As TextBox
    Friend WithEvents lblDateVerrouillage As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtDernierLogin As TextBox
    Friend WithEvents lblDernierLogin As Label
    Friend WithEvents lblDateCreation As Label
    Friend WithEvents lblDateModification As Label
    Friend WithEvents txtDateCreation As TextBox
    Friend WithEvents txtDateModification As TextBox
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider

End Class
