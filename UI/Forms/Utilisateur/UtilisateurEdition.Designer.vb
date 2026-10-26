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
        carteAudit = New AltheaCarte()
        tblAudit = New TableLayoutPanel()
        txtDateModification = New TextBox()
        lblDateModification = New Label()
        txtDateCreation = New TextBox()
        lblDateCreation = New Label()
        carteSecurite = New AltheaCarte()
        tblSecurite = New TableLayoutPanel()
        lblNbEchecsLogin = New Label()
        txtNbEchecsLogin = New TextBox()
        chkCompteVerrouille = New AltheaCase()
        txtDateVerrouillage = New TextBox()
        lblDateVerrouillage = New Label()
        txtDernierLogin = New TextBox()
        lblDernierLogin = New Label()
        carteDroits = New AltheaCarte()
        tblDroits = New TableLayoutPanel()
        cboRoleMaxElevation = New AltheaListe()
        cboRoleUtilisateur = New AltheaListe()
        lblRoleUtilisateur = New Label()
        lblRoleMaxElevation = New Label()
        chkActif = New AltheaCase()
        chkMustChangePassword = New AltheaCase()
        carteIdentite = New AltheaCarte()
        tblIdentite = New TableLayoutPanel()
        txtNomAffichage = New TextBox()
        txtLoginUtilisateur = New TextBox()
        lblLoginUtilisateur = New Label()
        txtCodeUtilisateur = New TextBox()
        lblCodeUtilisateur = New Label()
        lblNomAffichage = New Label()
        picPhotoUtilisateur = New PictureBox()
        pnlPhoto = New Panel()
        btnPhotoImporter = New AltheaBouton()
        lblPhotoInfo = New Label()
        pnlActions = New AltheaBarreActions()
        btnResetPassword = New AltheaBouton()
        btnDeverrouiller = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        btnEnregistrer = New AltheaBouton()
        enTete = New AltheaEnTete()
        ttMain = New ToolTip(components)
        errProvider = New ErrorProvider(components)
        carteAudit.SuspendLayout()
        tblAudit.SuspendLayout()
        carteSecurite.SuspendLayout()
        tblSecurite.SuspendLayout()
        carteDroits.SuspendLayout()
        tblDroits.SuspendLayout()
        carteIdentite.SuspendLayout()
        tblIdentite.SuspendLayout()
        pnlPhoto.SuspendLayout()
        pnlActions.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' carteAudit
        ' 
        carteAudit.Controls.Add(tblAudit)
        carteAudit.Location = New Point(450, 339)
        carteAudit.Name = "carteAudit"
        carteAudit.Size = New Size(422, 264)
        carteAudit.TabIndex = 24
        carteAudit.Titre = "Audit"
        ' 
        ' tblAudit
        ' 
        tblAudit.BackColor = Color.Transparent
        tblAudit.ColumnCount = 2
        tblAudit.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35F))
        tblAudit.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65F))
        tblAudit.Controls.Add(txtDateModification, 1, 1)
        tblAudit.Controls.Add(lblDateModification, 0, 1)
        tblAudit.Controls.Add(txtDateCreation, 1, 0)
        tblAudit.Controls.Add(lblDateCreation, 0, 0)
        tblAudit.Dock = DockStyle.Fill
        tblAudit.Location = New Point(0, 0)
        tblAudit.Name = "tblAudit"
        tblAudit.Padding = New Padding(8)
        tblAudit.RowCount = 2
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
        txtDateModification.Font = New Font("Calibri", 11F)
        txtDateModification.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
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
        lblDateModification.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
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
        txtDateCreation.Font = New Font("Calibri", 11F)
        txtDateCreation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
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
        lblDateCreation.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDateCreation.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateCreation.Location = New Point(11, 79)
        lblDateCreation.Name = "lblDateCreation"
        lblDateCreation.Size = New Size(86, 17)
        lblDateCreation.TabIndex = 25
        lblDateCreation.Text = "Date création"
        ' 
        ' carteSecurite
        ' 
        carteSecurite.Controls.Add(tblSecurite)
        carteSecurite.Location = New Point(4, 339)
        carteSecurite.Name = "carteSecurite"
        carteSecurite.Size = New Size(435, 264)
        carteSecurite.TabIndex = 23
        carteSecurite.Titre = "Sécurité"
        ' 
        ' tblSecurite
        ' 
        tblSecurite.BackColor = Color.Transparent
        tblSecurite.ColumnCount = 2
        tblSecurite.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35F))
        tblSecurite.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65F))
        tblSecurite.Controls.Add(lblNbEchecsLogin, 0, 0)
        tblSecurite.Controls.Add(txtNbEchecsLogin, 1, 0)
        tblSecurite.Controls.Add(chkCompteVerrouille, 1, 1)
        tblSecurite.Controls.Add(txtDateVerrouillage, 1, 2)
        tblSecurite.Controls.Add(lblDateVerrouillage, 0, 2)
        tblSecurite.Controls.Add(txtDernierLogin, 1, 3)
        tblSecurite.Controls.Add(lblDernierLogin, 0, 3)
        tblSecurite.Dock = DockStyle.Fill
        tblSecurite.Location = New Point(0, 0)
        tblSecurite.Name = "tblSecurite"
        tblSecurite.Padding = New Padding(8)
        tblSecurite.RowCount = 4
        tblSecurite.RowStyles.Add(New RowStyle(SizeType.Percent, 22.44898F))
        tblSecurite.RowStyles.Add(New RowStyle(SizeType.Percent, 22.44898F))
        tblSecurite.RowStyles.Add(New RowStyle(SizeType.Percent, 22.44898F))
        tblSecurite.RowStyles.Add(New RowStyle(SizeType.Percent, 22.44898F))
        tblSecurite.Size = New Size(435, 264)
        tblSecurite.TabIndex = 2
        ' 
        ' lblNbEchecsLogin
        ' 
        lblNbEchecsLogin.Anchor = AnchorStyles.Left
        lblNbEchecsLogin.AutoSize = True
        lblNbEchecsLogin.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNbEchecsLogin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNbEchecsLogin.Location = New Point(11, 52)
        lblNbEchecsLogin.Name = "lblNbEchecsLogin"
        lblNbEchecsLogin.Size = New Size(98, 17)
        lblNbEchecsLogin.TabIndex = 24
        lblNbEchecsLogin.Text = "Nb Echecs Login"
        ' 
        ' txtNbEchecsLogin
        ' 
        txtNbEchecsLogin.Anchor = AnchorStyles.Left
        txtNbEchecsLogin.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNbEchecsLogin.BorderStyle = BorderStyle.None
        txtNbEchecsLogin.Font = New Font("Calibri", 11F)
        txtNbEchecsLogin.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
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
        chkCompteVerrouille.Font = New Font("Calibri", 11F)
        chkCompteVerrouille.Location = New Point(157, 106)
        chkCompteVerrouille.Name = "chkCompteVerrouille"
        chkCompteVerrouille.Size = New Size(124, 18)
        chkCompteVerrouille.TabIndex = 25
        chkCompteVerrouille.Text = "Compte Verrouillé"
        ' 
        ' txtDateVerrouillage
        ' 
        txtDateVerrouillage.Anchor = AnchorStyles.Left
        txtDateVerrouillage.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtDateVerrouillage.BorderStyle = BorderStyle.None
        txtDateVerrouillage.Font = New Font("Calibri", 11F)
        txtDateVerrouillage.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
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
        lblDateVerrouillage.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
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
        txtDernierLogin.Font = New Font("Calibri", 11F)
        txtDernierLogin.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
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
        lblDernierLogin.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDernierLogin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDernierLogin.Location = New Point(11, 218)
        lblDernierLogin.Name = "lblDernierLogin"
        lblDernierLogin.Size = New Size(81, 17)
        lblDernierLogin.TabIndex = 29
        lblDernierLogin.Text = "Dernier login"
        ' 
        ' carteDroits
        ' 
        carteDroits.Controls.Add(tblDroits)
        carteDroits.Location = New Point(450, 64)
        carteDroits.Name = "carteDroits"
        carteDroits.Size = New Size(422, 264)
        carteDroits.TabIndex = 22
        carteDroits.Titre = "Droits"
        ' 
        ' tblDroits
        ' 
        tblDroits.BackColor = Color.Transparent
        tblDroits.ColumnCount = 2
        tblDroits.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35F))
        tblDroits.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65F))
        tblDroits.Controls.Add(cboRoleMaxElevation, 1, 1)
        tblDroits.Controls.Add(cboRoleUtilisateur, 1, 0)
        tblDroits.Controls.Add(lblRoleUtilisateur, 0, 0)
        tblDroits.Controls.Add(lblRoleMaxElevation, 0, 1)
        tblDroits.Controls.Add(chkActif, 0, 2)
        tblDroits.Controls.Add(chkMustChangePassword, 1, 2)
        tblDroits.Dock = DockStyle.Fill
        tblDroits.Location = New Point(0, 0)
        tblDroits.Name = "tblDroits"
        tblDroits.Padding = New Padding(8)
        tblDroits.RowCount = 3
        tblDroits.RowStyles.Add(New RowStyle(SizeType.Percent, 30F))
        tblDroits.RowStyles.Add(New RowStyle(SizeType.Percent, 30F))
        tblDroits.RowStyles.Add(New RowStyle(SizeType.Percent, 30F))
        tblDroits.Size = New Size(422, 264)
        tblDroits.TabIndex = 1
        ' 
        ' cboRoleMaxElevation
        ' 
        cboRoleMaxElevation.Anchor = AnchorStyles.Left
        cboRoleMaxElevation.Font = New Font("Calibri", 11F)
        cboRoleMaxElevation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboRoleMaxElevation.ItemHeight = 20
        cboRoleMaxElevation.Location = New Point(153, 132)
        cboRoleMaxElevation.Name = "cboRoleMaxElevation"
        cboRoleMaxElevation.Size = New Size(195, 26)
        cboRoleMaxElevation.TabIndex = 4
        ' 
        ' cboRoleUtilisateur
        ' 
        cboRoleUtilisateur.Anchor = AnchorStyles.Left
        cboRoleUtilisateur.Font = New Font("Calibri", 11F)
        cboRoleUtilisateur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboRoleUtilisateur.ItemHeight = 20
        cboRoleUtilisateur.Location = New Point(153, 58)
        cboRoleUtilisateur.Name = "cboRoleUtilisateur"
        cboRoleUtilisateur.Size = New Size(195, 26)
        cboRoleUtilisateur.TabIndex = 3
        ' 
        ' lblRoleUtilisateur
        ' 
        lblRoleUtilisateur.Anchor = AnchorStyles.Left
        lblRoleUtilisateur.AutoSize = True
        lblRoleUtilisateur.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblRoleUtilisateur.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRoleUtilisateur.Location = New Point(11, 60)
        lblRoleUtilisateur.Name = "lblRoleUtilisateur"
        lblRoleUtilisateur.Size = New Size(95, 17)
        lblRoleUtilisateur.TabIndex = 19
        lblRoleUtilisateur.Text = "Rôle Utilisateur"
        ' 
        ' lblRoleMaxElevation
        ' 
        lblRoleMaxElevation.Anchor = AnchorStyles.Left
        lblRoleMaxElevation.AutoSize = True
        lblRoleMaxElevation.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
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
        chkActif.Font = New Font("Calibri", 11F)
        chkActif.Location = New Point(11, 209)
        chkActif.Name = "chkActif"
        chkActif.Size = New Size(49, 18)
        chkActif.TabIndex = 5
        chkActif.Text = "Actif"
        ' 
        ' chkMustChangePassword
        ' 
        chkMustChangePassword.Anchor = AnchorStyles.Left
        chkMustChangePassword.AutoSize = True
        chkMustChangePassword.Font = New Font("Calibri", 11F)
        chkMustChangePassword.Location = New Point(153, 209)
        chkMustChangePassword.Name = "chkMustChangePassword"
        chkMustChangePassword.Size = New Size(163, 18)
        chkMustChangePassword.TabIndex = 22
        chkMustChangePassword.Text = "Forcer le changement PW"
        ' 
        ' carteIdentite
        ' 
        carteIdentite.Controls.Add(tblIdentite)
        carteIdentite.Location = New Point(4, 64)
        carteIdentite.Name = "carteIdentite"
        carteIdentite.Size = New Size(435, 264)
        carteIdentite.TabIndex = 21
        carteIdentite.Titre = "Identité"
        ' 
        ' tblIdentite
        ' 
        tblIdentite.BackColor = Color.Transparent
        tblIdentite.ColumnCount = 2
        tblIdentite.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35F))
        tblIdentite.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65F))
        tblIdentite.Controls.Add(txtNomAffichage, 1, 2)
        tblIdentite.Controls.Add(txtLoginUtilisateur, 1, 1)
        tblIdentite.Controls.Add(lblLoginUtilisateur, 0, 1)
        tblIdentite.Controls.Add(txtCodeUtilisateur, 1, 0)
        tblIdentite.Controls.Add(lblCodeUtilisateur, 0, 0)
        tblIdentite.Controls.Add(lblNomAffichage, 0, 2)
        tblIdentite.Controls.Add(picPhotoUtilisateur, 0, 3)
        tblIdentite.Controls.Add(pnlPhoto, 1, 3)
        tblIdentite.Dock = DockStyle.Fill
        tblIdentite.Location = New Point(0, 0)
        tblIdentite.Name = "tblIdentite"
        tblIdentite.Padding = New Padding(8)
        tblIdentite.RowCount = 4
        tblIdentite.RowStyles.Add(New RowStyle(SizeType.Absolute, 36F))
        tblIdentite.RowStyles.Add(New RowStyle(SizeType.Absolute, 36F))
        tblIdentite.RowStyles.Add(New RowStyle(SizeType.Absolute, 36F))
        tblIdentite.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        tblIdentite.Size = New Size(435, 264)
        tblIdentite.TabIndex = 0
        ' 
        ' txtNomAffichage
        ' 
        txtNomAffichage.Anchor = AnchorStyles.Left
        txtNomAffichage.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNomAffichage.Font = New Font("Calibri", 11F)
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
        txtLoginUtilisateur.Font = New Font("Calibri", 11F)
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
        lblLoginUtilisateur.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
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
        txtCodeUtilisateur.Font = New Font("Calibri", 11F)
        txtCodeUtilisateur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
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
        lblCodeUtilisateur.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCodeUtilisateur.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCodeUtilisateur.Location = New Point(11, 60)
        lblCodeUtilisateur.Name = "lblCodeUtilisateur"
        lblCodeUtilisateur.Size = New Size(98, 17)
        lblCodeUtilisateur.TabIndex = 18
        lblCodeUtilisateur.Text = "Code Utilisateur"
        ' 
        ' lblNomAffichage
        ' 
        lblNomAffichage.Anchor = AnchorStyles.Left
        lblNomAffichage.AutoSize = True
        lblNomAffichage.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNomAffichage.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNomAffichage.Location = New Point(11, 209)
        lblNomAffichage.Name = "lblNomAffichage"
        lblNomAffichage.Size = New Size(91, 17)
        lblNomAffichage.TabIndex = 21
        lblNomAffichage.Text = "Nom Affichage"
        ' 
        ' picPhotoUtilisateur
        ' 
        picPhotoUtilisateur.Anchor = AnchorStyles.Left
        picPhotoUtilisateur.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        picPhotoUtilisateur.BorderStyle = BorderStyle.FixedSingle
        picPhotoUtilisateur.Location = New Point(11, 125)
        picPhotoUtilisateur.Name = "picPhotoUtilisateur"
        picPhotoUtilisateur.Size = New Size(76, 76)
        picPhotoUtilisateur.SizeMode = PictureBoxSizeMode.Zoom
        picPhotoUtilisateur.TabIndex = 23
        picPhotoUtilisateur.TabStop = False
        ' 
        ' pnlPhoto
        ' 
        pnlPhoto.Controls.Add(btnPhotoImporter)
        pnlPhoto.Controls.Add(lblPhotoInfo)
        pnlPhoto.BackColor = Color.Transparent
        pnlPhoto.Dock = DockStyle.Fill
        pnlPhoto.Location = New Point(157, 125)
        pnlPhoto.Name = "pnlPhoto"
        pnlPhoto.Size = New Size(267, 100)
        pnlPhoto.TabIndex = 24
        ' 
        ' btnPhotoImporter
        ' 
        btnPhotoImporter.Image = CType(resources.GetObject("btnPhotoImporter.Image"), Image)
        btnPhotoImporter.Location = New Point(0, 8)
        btnPhotoImporter.Name = "btnPhotoImporter"
        btnPhotoImporter.Size = New Size(125, 30)
        btnPhotoImporter.TabIndex = 0
        btnPhotoImporter.Tag = "importer_normal"
        btnPhotoImporter.Text = "Importer…"
        btnPhotoImporter.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' lblPhotoInfo
        ' 
        lblPhotoInfo.Font = New Font("Calibri", 9F, FontStyle.Italic)
        lblPhotoInfo.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblPhotoInfo.Location = New Point(0, 44)
        lblPhotoInfo.Name = "lblPhotoInfo"
        lblPhotoInfo.Size = New Size(260, 48)
        lblPhotoInfo.TabIndex = 1
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnResetPassword)
        pnlActions.Controls.Add(btnDeverrouiller)
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 607)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(884, 60)
        pnlActions.TabIndex = 19
        ' 
        ' btnResetPassword
        ' 
        btnResetPassword.Image = CType(resources.GetObject("btnResetPassword.Image"), Image)
        btnResetPassword.Location = New Point(554, 10)
        btnResetPassword.Name = "btnResetPassword"
        btnResetPassword.Size = New Size(124, 40)
        btnResetPassword.TabIndex = 18
        btnResetPassword.Tag = "reset_password_normal"
        btnResetPassword.Text = "Reset PW"
        ' 
        ' btnDeverrouiller
        ' 
        btnDeverrouiller.Image = CType(resources.GetObject("btnDeverrouiller.Image"), Image)
        btnDeverrouiller.Location = New Point(398, 10)
        btnDeverrouiller.Name = "btnDeverrouiller"
        btnDeverrouiller.Size = New Size(146, 40)
        btnDeverrouiller.TabIndex = 17
        btnDeverrouiller.Tag = "Verrouiller_normal"
        btnDeverrouiller.Text = "Déverrouiller"
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(274, 10)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 40)
        btnAnnuler.TabIndex = 16
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(132, 10)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 40)
        btnEnregistrer.TabIndex = 15
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(884, 58)
        enTete.TabIndex = 4
        enTete.SousTitre = "Gérez les utilisateurs"
        enTete.Text = "Utilisateurs"
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' UtilisateurEdition
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(884, 668)
        Controls.Add(carteAudit)
        Controls.Add(carteSecurite)
        Controls.Add(carteDroits)
        Controls.Add(carteIdentite)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        Name = "UtilisateurEdition"
        StartPosition = FormStartPosition.CenterParent
        Text = "Edition utilisateurs"
        carteAudit.ResumeLayout(False)
        tblAudit.ResumeLayout(False)
        tblAudit.PerformLayout()
        carteSecurite.ResumeLayout(False)
        tblSecurite.ResumeLayout(False)
        tblSecurite.PerformLayout()
        carteDroits.ResumeLayout(False)
        tblDroits.ResumeLayout(False)
        tblDroits.PerformLayout()
        carteIdentite.ResumeLayout(False)
        tblIdentite.ResumeLayout(False)
        tblIdentite.PerformLayout()
        pnlPhoto.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents lblPhotoInfo As Label
    Friend WithEvents btnPhotoImporter As AltheaBouton
    Friend WithEvents pnlPhoto As Panel
    Friend WithEvents picPhotoUtilisateur As PictureBox
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnResetPassword As AltheaBouton
    Friend WithEvents btnDeverrouiller As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents carteIdentite As AltheaCarte
    Friend WithEvents carteDroits As AltheaCarte
    Friend WithEvents carteSecurite As AltheaCarte
    Friend WithEvents carteAudit As AltheaCarte
    Friend WithEvents tblIdentite As TableLayoutPanel
    Friend WithEvents tblDroits As TableLayoutPanel
    Friend WithEvents tblAudit As TableLayoutPanel
    Friend WithEvents tblSecurite As TableLayoutPanel
    Friend WithEvents lblCodeUtilisateur As Label
    Friend WithEvents txtCodeUtilisateur As TextBox
    Friend WithEvents lblLoginUtilisateur As Label
    Friend WithEvents lblNomAffichage As Label
    Friend WithEvents txtLoginUtilisateur As TextBox
    Friend WithEvents txtNomAffichage As TextBox
    Friend WithEvents lblRoleUtilisateur As Label
    Friend WithEvents lblRoleMaxElevation As Label
    Friend WithEvents chkActif As AltheaCase
    Friend WithEvents chkMustChangePassword As AltheaCase
    Friend WithEvents cboRoleUtilisateur As AltheaListe
    Friend WithEvents cboRoleMaxElevation As AltheaListe
    Friend WithEvents lblNbEchecsLogin As Label
    Friend WithEvents txtNbEchecsLogin As TextBox
    Friend WithEvents chkCompteVerrouille As AltheaCase
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
