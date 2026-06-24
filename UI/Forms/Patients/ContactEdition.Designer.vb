<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ContactEdition
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
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

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ContactEdition))
        lblTitreForm = New Label()
        grpIdentite = New GroupBox()
        lblLien = New Label()
        cboLien = New ComboBox()
        btnAjouterLien = New Button()
        lblNom = New Label()
        txtNom = New TextBox()
        lblPrenom = New Label()
        txtPrenom = New TextBox()
        lblDateNaissance = New Label()
        dtpDateNaissance = New DateTimePicker()
        grpCoordonnees = New GroupBox()
        lblTelephone = New Label()
        txtTelephone = New TextBox()
        lblEmail = New Label()
        txtEmail = New TextBox()
        lblPays = New Label()
        cboPays = New ComboBox()
        lblAdresseLigne1 = New Label()
        txtAdresseLigne1 = New TextBox()
        btnCopierAdressePatient = New Button()
        lblAdresseLigne2 = New Label()
        txtAdresseLigne2 = New TextBox()
        lblCodePostal = New Label()
        txtCodePostal = New TextBox()
        lblLocalite = New Label()
        txtLocalite = New TextBox()
        grpRoles = New GroupBox()
        lblRoleLegal = New Label()
        cboRoleLegal = New ComboBox()
        btnAjouterRole = New Button()
        grpCommentaire = New GroupBox()
        rteCommentaire = New UC_RichTextEditorSimple()
        ttMain = New ToolTip(components)
        errProvider = New ErrorProvider(components)
        pnlAction = New Panel()
        btnModifier = New Button()
        btnFermer = New Button()
        btnEnregistrer = New Button()
        btnAnnuler = New Button()
        pnlTitre = New Panel()
        lblTop = New Label()
        grpIdentite.SuspendLayout()
        grpCoordonnees.SuspendLayout()
        grpRoles.SuspendLayout()
        grpCommentaire.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        pnlAction.SuspendLayout()
        pnlTitre.SuspendLayout()
        SuspendLayout()
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.AutoSize = True
        lblTitreForm.Font = New Font("Calibri", 14F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(57, 20)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Size = New Size(71, 23)
        lblTitreForm.TabIndex = 0
        lblTitreForm.Text = "Contact"
        ' 
        ' grpIdentite
        ' 
        grpIdentite.BackColor = Color.Transparent
        grpIdentite.Controls.Add(lblLien)
        grpIdentite.Controls.Add(cboLien)
        grpIdentite.Controls.Add(btnAjouterLien)
        grpIdentite.Controls.Add(lblNom)
        grpIdentite.Controls.Add(txtNom)
        grpIdentite.Controls.Add(lblPrenom)
        grpIdentite.Controls.Add(txtPrenom)
        grpIdentite.Controls.Add(lblDateNaissance)
        grpIdentite.Controls.Add(dtpDateNaissance)
        grpIdentite.Font = New Font("Calibri", 10F, FontStyle.Bold)
        grpIdentite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpIdentite.Location = New Point(16, 68)
        grpIdentite.Name = "grpIdentite"
        grpIdentite.Size = New Size(688, 130)
        grpIdentite.TabIndex = 1
        grpIdentite.TabStop = False
        grpIdentite.Text = "Identité du contact"
        ' 
        ' lblLien
        ' 
        lblLien.AutoSize = True
        lblLien.Font = New Font("Calibri", 10F)
        lblLien.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLien.Location = New Point(16, 33)
        lblLien.Name = "lblLien"
        lblLien.Size = New Size(31, 17)
        lblLien.TabIndex = 0
        lblLien.Text = "Lien"
        ' 
        ' cboLien
        ' 
        cboLien.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboLien.DropDownStyle = ComboBoxStyle.DropDownList
        cboLien.Font = New Font("Calibri", 10F)
        cboLien.Location = New Point(130, 30)
        cboLien.Name = "cboLien"
        cboLien.Size = New Size(230, 23)
        cboLien.TabIndex = 1
        ' 
        ' btnAjouterLien
        ' 
        btnAjouterLien.BackColor = Color.Transparent
        btnAjouterLien.FlatAppearance.BorderSize = 0
        btnAjouterLien.FlatStyle = FlatStyle.Flat
        btnAjouterLien.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnAjouterLien.Image = CType(resources.GetObject("btnAjouterLien.Image"), Image)
        btnAjouterLien.Location = New Point(364, 29)
        btnAjouterLien.Name = "btnAjouterLien"
        btnAjouterLien.Size = New Size(27, 27)
        btnAjouterLien.TabIndex = 2
        btnAjouterLien.Tag = "plus_24_normal.png"
        btnAjouterLien.Text = "+"
        btnAjouterLien.UseVisualStyleBackColor = False
        ' 
        ' lblNom
        ' 
        lblNom.AutoSize = True
        lblNom.Font = New Font("Calibri", 10F)
        lblNom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNom.Location = New Point(16, 67)
        lblNom.Name = "lblNom"
        lblNom.Size = New Size(35, 17)
        lblNom.TabIndex = 3
        lblNom.Text = "Nom"
        ' 
        ' txtNom
        ' 
        txtNom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNom.Font = New Font("Calibri", 10F)
        txtNom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNom.Location = New Point(130, 64)
        txtNom.MaxLength = 100
        txtNom.Name = "txtNom"
        txtNom.Size = New Size(230, 24)
        txtNom.TabIndex = 4
        ' 
        ' lblPrenom
        ' 
        lblPrenom.AutoSize = True
        lblPrenom.Font = New Font("Calibri", 10F)
        lblPrenom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPrenom.Location = New Point(16, 100)
        lblPrenom.Name = "lblPrenom"
        lblPrenom.Size = New Size(52, 17)
        lblPrenom.TabIndex = 5
        lblPrenom.Text = "Prénom"
        ' 
        ' txtPrenom
        ' 
        txtPrenom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtPrenom.Font = New Font("Calibri", 10F)
        txtPrenom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPrenom.Location = New Point(130, 97)
        txtPrenom.MaxLength = 100
        txtPrenom.Name = "txtPrenom"
        txtPrenom.Size = New Size(230, 24)
        txtPrenom.TabIndex = 6
        ' 
        ' lblDateNaissance
        ' 
        lblDateNaissance.AutoSize = True
        lblDateNaissance.Font = New Font("Calibri", 10F)
        lblDateNaissance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateNaissance.Location = New Point(410, 67)
        lblDateNaissance.Name = "lblDateNaissance"
        lblDateNaissance.Size = New Size(110, 17)
        lblDateNaissance.TabIndex = 7
        lblDateNaissance.Text = "Date de naissance"
        ' 
        ' dtpDateNaissance
        ' 
        dtpDateNaissance.CalendarForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        dtpDateNaissance.CalendarMonthBackground = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dtpDateNaissance.CalendarTitleBackColor = Color.FromArgb(CByte(230), CByte(222), CByte(226))
        dtpDateNaissance.CalendarTitleForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        dtpDateNaissance.Font = New Font("Calibri", 10F)
        dtpDateNaissance.Format = DateTimePickerFormat.Short
        dtpDateNaissance.Location = New Point(522, 64)
        dtpDateNaissance.Name = "dtpDateNaissance"
        dtpDateNaissance.ShowCheckBox = True
        dtpDateNaissance.Size = New Size(150, 24)
        dtpDateNaissance.TabIndex = 8
        ' 
        ' grpCoordonnees
        ' 
        grpCoordonnees.Controls.Add(lblTelephone)
        grpCoordonnees.Controls.Add(txtTelephone)
        grpCoordonnees.Controls.Add(lblEmail)
        grpCoordonnees.Controls.Add(txtEmail)
        grpCoordonnees.Controls.Add(lblPays)
        grpCoordonnees.Controls.Add(cboPays)
        grpCoordonnees.Controls.Add(lblAdresseLigne1)
        grpCoordonnees.Controls.Add(txtAdresseLigne1)
        grpCoordonnees.Controls.Add(btnCopierAdressePatient)
        grpCoordonnees.Controls.Add(lblAdresseLigne2)
        grpCoordonnees.Controls.Add(txtAdresseLigne2)
        grpCoordonnees.Controls.Add(lblCodePostal)
        grpCoordonnees.Controls.Add(txtCodePostal)
        grpCoordonnees.Controls.Add(lblLocalite)
        grpCoordonnees.Controls.Add(txtLocalite)
        grpCoordonnees.Font = New Font("Calibri", 10F, FontStyle.Bold)
        grpCoordonnees.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpCoordonnees.Location = New Point(16, 206)
        grpCoordonnees.Name = "grpCoordonnees"
        grpCoordonnees.Size = New Size(688, 196)
        grpCoordonnees.TabIndex = 2
        grpCoordonnees.TabStop = False
        grpCoordonnees.Text = "Coordonnées"
        ' 
        ' lblTelephone
        ' 
        lblTelephone.AutoSize = True
        lblTelephone.Font = New Font("Calibri", 10F)
        lblTelephone.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTelephone.Location = New Point(16, 66)
        lblTelephone.Name = "lblTelephone"
        lblTelephone.Size = New Size(66, 17)
        lblTelephone.TabIndex = 2
        lblTelephone.Text = "Téléphone"
        ' 
        ' txtTelephone
        ' 
        txtTelephone.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtTelephone.Font = New Font("Calibri", 10F)
        txtTelephone.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtTelephone.Location = New Point(130, 63)
        txtTelephone.MaxLength = 50
        txtTelephone.Name = "txtTelephone"
        txtTelephone.Size = New Size(230, 24)
        txtTelephone.TabIndex = 3
        ' 
        ' lblEmail
        ' 
        lblEmail.AutoSize = True
        lblEmail.Font = New Font("Calibri", 10F)
        lblEmail.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblEmail.Location = New Point(410, 66)
        lblEmail.Name = "lblEmail"
        lblEmail.Size = New Size(43, 17)
        lblEmail.TabIndex = 4
        lblEmail.Text = "E-mail"
        ' 
        ' txtEmail
        ' 
        txtEmail.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtEmail.Font = New Font("Calibri", 10F)
        txtEmail.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtEmail.Location = New Point(460, 63)
        txtEmail.MaxLength = 150
        txtEmail.Name = "txtEmail"
        txtEmail.Size = New Size(212, 24)
        txtEmail.TabIndex = 5
        ' 
        ' lblPays
        ' 
        lblPays.AutoSize = True
        lblPays.Font = New Font("Calibri", 10F)
        lblPays.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPays.Location = New Point(16, 33)
        lblPays.Name = "lblPays"
        lblPays.Size = New Size(33, 17)
        lblPays.TabIndex = 0
        lblPays.Text = "Pays"
        ' 
        ' cboPays
        ' 
        cboPays.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboPays.Font = New Font("Calibri", 10F)
        cboPays.Location = New Point(130, 30)
        cboPays.Name = "cboPays"
        cboPays.Size = New Size(230, 23)
        cboPays.TabIndex = 1
        ' 
        ' lblAdresseLigne1
        ' 
        lblAdresseLigne1.AutoSize = True
        lblAdresseLigne1.Font = New Font("Calibri", 10F)
        lblAdresseLigne1.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAdresseLigne1.Location = New Point(16, 99)
        lblAdresseLigne1.Name = "lblAdresseLigne1"
        lblAdresseLigne1.Size = New Size(52, 17)
        lblAdresseLigne1.TabIndex = 6
        lblAdresseLigne1.Text = "Adresse"
        ' 
        ' txtAdresseLigne1
        ' 
        txtAdresseLigne1.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdresseLigne1.Font = New Font("Calibri", 10F)
        txtAdresseLigne1.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdresseLigne1.Location = New Point(130, 96)
        txtAdresseLigne1.MaxLength = 255
        txtAdresseLigne1.Name = "txtAdresseLigne1"
        txtAdresseLigne1.Size = New Size(505, 24)
        txtAdresseLigne1.TabIndex = 7
        ' 
        ' btnCopierAdressePatient
        ' 
        btnCopierAdressePatient.FlatAppearance.BorderSize = 0
        btnCopierAdressePatient.FlatStyle = FlatStyle.Flat
        btnCopierAdressePatient.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnCopierAdressePatient.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnCopierAdressePatient.Image = CType(resources.GetObject("btnCopierAdressePatient.Image"), Image)
        btnCopierAdressePatient.Location = New Point(645, 95)
        btnCopierAdressePatient.Name = "btnCopierAdressePatient"
        btnCopierAdressePatient.Size = New Size(27, 27)
        btnCopierAdressePatient.TabIndex = 8
        btnCopierAdressePatient.Tag = "flechebas_24_normal"
        btnCopierAdressePatient.UseVisualStyleBackColor = True
        ' 
        ' lblAdresseLigne2
        ' 
        lblAdresseLigne2.AutoSize = True
        lblAdresseLigne2.Font = New Font("Calibri", 10F)
        lblAdresseLigne2.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAdresseLigne2.Location = New Point(16, 132)
        lblAdresseLigne2.Name = "lblAdresseLigne2"
        lblAdresseLigne2.Size = New Size(80, 17)
        lblAdresseLigne2.TabIndex = 8
        lblAdresseLigne2.Text = "Complément"
        ' 
        ' txtAdresseLigne2
        ' 
        txtAdresseLigne2.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdresseLigne2.Font = New Font("Calibri", 10F)
        txtAdresseLigne2.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdresseLigne2.Location = New Point(130, 129)
        txtAdresseLigne2.MaxLength = 255
        txtAdresseLigne2.Name = "txtAdresseLigne2"
        txtAdresseLigne2.Size = New Size(542, 24)
        txtAdresseLigne2.TabIndex = 9
        ' 
        ' lblCodePostal
        ' 
        lblCodePostal.AutoSize = True
        lblCodePostal.Font = New Font("Calibri", 10F)
        lblCodePostal.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCodePostal.Location = New Point(16, 165)
        lblCodePostal.Name = "lblCodePostal"
        lblCodePostal.Size = New Size(73, 17)
        lblCodePostal.TabIndex = 10
        lblCodePostal.Text = "Code postal"
        ' 
        ' txtCodePostal
        ' 
        txtCodePostal.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCodePostal.Font = New Font("Calibri", 10F)
        txtCodePostal.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCodePostal.Location = New Point(130, 162)
        txtCodePostal.MaxLength = 20
        txtCodePostal.Name = "txtCodePostal"
        txtCodePostal.Size = New Size(120, 24)
        txtCodePostal.TabIndex = 11
        ' 
        ' lblLocalite
        ' 
        lblLocalite.AutoSize = True
        lblLocalite.Font = New Font("Calibri", 10F)
        lblLocalite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLocalite.Location = New Point(286, 165)
        lblLocalite.Name = "lblLocalite"
        lblLocalite.Size = New Size(52, 17)
        lblLocalite.TabIndex = 12
        lblLocalite.Text = "Localité"
        ' 
        ' txtLocalite
        ' 
        txtLocalite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtLocalite.Font = New Font("Calibri", 10F)
        txtLocalite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLocalite.Location = New Point(346, 162)
        txtLocalite.MaxLength = 100
        txtLocalite.Name = "txtLocalite"
        txtLocalite.Size = New Size(326, 24)
        txtLocalite.TabIndex = 13
        ' 
        ' grpRoles
        ' 
        grpRoles.Controls.Add(lblRoleLegal)
        grpRoles.Controls.Add(cboRoleLegal)
        grpRoles.Controls.Add(btnAjouterRole)
        grpRoles.Font = New Font("Calibri", 10F, FontStyle.Bold)
        grpRoles.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpRoles.Location = New Point(16, 410)
        grpRoles.Name = "grpRoles"
        grpRoles.Size = New Size(688, 58)
        grpRoles.TabIndex = 3
        grpRoles.TabStop = False
        grpRoles.Text = "Rôle légal"
        ' 
        ' lblRoleLegal
        ' 
        lblRoleLegal.AutoSize = True
        lblRoleLegal.Font = New Font("Calibri", 10F)
        lblRoleLegal.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRoleLegal.Location = New Point(16, 25)
        lblRoleLegal.Name = "lblRoleLegal"
        lblRoleLegal.Size = New Size(63, 17)
        lblRoleLegal.TabIndex = 0
        lblRoleLegal.Text = "Rôle légal"
        ' 
        ' cboRoleLegal
        ' 
        cboRoleLegal.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboRoleLegal.DropDownStyle = ComboBoxStyle.DropDownList
        cboRoleLegal.Font = New Font("Calibri", 10F)
        cboRoleLegal.Location = New Point(130, 22)
        cboRoleLegal.Name = "cboRoleLegal"
        cboRoleLegal.Size = New Size(230, 23)
        cboRoleLegal.TabIndex = 1
        ' 
        ' btnAjouterRole
        ' 
        btnAjouterRole.FlatAppearance.BorderColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        btnAjouterRole.FlatAppearance.BorderSize = 0
        btnAjouterRole.FlatStyle = FlatStyle.Flat
        btnAjouterRole.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnAjouterRole.Image = CType(resources.GetObject("btnAjouterRole.Image"), Image)
        btnAjouterRole.Location = New Point(364, 21)
        btnAjouterRole.Name = "btnAjouterRole"
        btnAjouterRole.Size = New Size(27, 27)
        btnAjouterRole.TabIndex = 2
        btnAjouterRole.Tag = "plus_24_normal.png"
        btnAjouterRole.Text = "+"
        btnAjouterRole.UseVisualStyleBackColor = True
        ' 
        ' grpCommentaire
        ' 
        grpCommentaire.Controls.Add(rteCommentaire)
        grpCommentaire.Font = New Font("Calibri", 10F, FontStyle.Bold)
        grpCommentaire.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpCommentaire.Location = New Point(16, 476)
        grpCommentaire.Name = "grpCommentaire"
        grpCommentaire.Size = New Size(688, 150)
        grpCommentaire.TabIndex = 4
        grpCommentaire.TabStop = False
        grpCommentaire.Text = "Commentaire"
        ' 
        ' rteCommentaire
        ' 
        rteCommentaire.BackColor = Color.Transparent
        rteCommentaire.Dock = DockStyle.Fill
        rteCommentaire.Location = New Point(3, 20)
        rteCommentaire.Margin = New Padding(4, 3, 4, 3)
        rteCommentaire.Name = "rteCommentaire"
        rteCommentaire.ReadOnlyMode = False
        rteCommentaire.RtfContent = ""
        rteCommentaire.ShowToolbar = True
        rteCommentaire.Size = New Size(682, 127)
        rteCommentaire.TabIndex = 0
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' pnlAction
        ' 
        pnlAction.BackColor = Color.FromArgb(CByte(231), CByte(223), CByte(214))
        pnlAction.BackgroundImage = CType(resources.GetObject("pnlAction.BackgroundImage"), Image)
        pnlAction.BackgroundImageLayout = ImageLayout.Stretch
        pnlAction.Controls.Add(btnModifier)
        pnlAction.Controls.Add(btnFermer)
        pnlAction.Controls.Add(btnEnregistrer)
        pnlAction.Controls.Add(btnAnnuler)
        pnlAction.Dock = DockStyle.Bottom
        pnlAction.Location = New Point(0, 632)
        pnlAction.Name = "pnlAction"
        pnlAction.Size = New Size(720, 62)
        pnlAction.TabIndex = 5
        ' 
        ' btnModifier
        ' 
        btnModifier.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnModifier.FlatStyle = FlatStyle.Flat
        btnModifier.Font = New Font("Calibri", 10F)
        btnModifier.ForeColor = Color.White
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.ImageAlign = ContentAlignment.MiddleLeft
        btnModifier.Location = New Point(235, 10)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(121, 40)
        btnModifier.TabIndex = 9
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        btnModifier.TextAlign = ContentAlignment.MiddleLeft
        btnModifier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifier.UseVisualStyleBackColor = False
        ' 
        ' btnFermer
        ' 
        btnFermer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnFermer.FlatStyle = FlatStyle.Flat
        btnFermer.Font = New Font("Calibri", 10F)
        btnFermer.ForeColor = Color.White
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.ImageAlign = ContentAlignment.MiddleLeft
        btnFermer.Location = New Point(362, 10)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(109, 40)
        btnFermer.TabIndex = 10
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.TextAlign = ContentAlignment.MiddleLeft
        btnFermer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnFermer.UseVisualStyleBackColor = False
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnEnregistrer.FlatStyle = FlatStyle.Flat
        btnEnregistrer.Font = New Font("Calibri", 10F)
        btnEnregistrer.ForeColor = Color.White
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.ImageAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.Location = New Point(235, 10)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(121, 40)
        btnEnregistrer.TabIndex = 7
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.TextAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEnregistrer.UseVisualStyleBackColor = False
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAnnuler.FlatStyle = FlatStyle.Flat
        btnAnnuler.Font = New Font("Calibri", 10F)
        btnAnnuler.ForeColor = Color.White
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.ImageAlign = ContentAlignment.MiddleLeft
        btnAnnuler.Location = New Point(362, 10)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(109, 40)
        btnAnnuler.TabIndex = 8
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.TextAlign = ContentAlignment.MiddleLeft
        btnAnnuler.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAnnuler.UseVisualStyleBackColor = False
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTop)
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(0, 0)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(720, 62)
        pnlTitre.TabIndex = 6
        ' 
        ' lblTop
        ' 
        lblTop.Font = New Font("Calibri", 12F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(268, 20)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(334, 26)
        lblTop.TabIndex = 4
        lblTop.Text = "Gestion des contacts et de l'entourage du patient"
        ' 
        ' ContactEdition
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        BackgroundImage = My.Resources.Resources.Fond_1000x800_Feuille4
        BackgroundImageLayout = ImageLayout.Stretch
        ClientSize = New Size(720, 694)
        Controls.Add(pnlTitre)
        Controls.Add(pnlAction)
        Controls.Add(grpIdentite)
        Controls.Add(grpCoordonnees)
        Controls.Add(grpRoles)
        Controls.Add(grpCommentaire)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "ContactEdition"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Contact"
        grpIdentite.ResumeLayout(False)
        grpIdentite.PerformLayout()
        grpCoordonnees.ResumeLayout(False)
        grpCoordonnees.PerformLayout()
        grpRoles.ResumeLayout(False)
        grpRoles.PerformLayout()
        grpCommentaire.ResumeLayout(False)
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        pnlAction.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        ResumeLayout(False)

    End Sub

    Friend WithEvents lblTitreForm As Label
    Friend WithEvents grpIdentite As GroupBox
    Friend WithEvents lblLien As Label
    Friend WithEvents cboLien As ComboBox
    Friend WithEvents btnAjouterLien As Button
    Friend WithEvents lblNom As Label
    Friend WithEvents txtNom As TextBox
    Friend WithEvents lblPrenom As Label
    Friend WithEvents txtPrenom As TextBox
    Friend WithEvents lblDateNaissance As Label
    Friend WithEvents dtpDateNaissance As DateTimePicker
    Friend WithEvents grpCoordonnees As GroupBox
    Friend WithEvents lblTelephone As Label
    Friend WithEvents txtTelephone As TextBox
    Friend WithEvents lblEmail As Label
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents lblPays As Label
    Friend WithEvents cboPays As ComboBox
    Friend WithEvents lblAdresseLigne1 As Label
    Friend WithEvents txtAdresseLigne1 As TextBox
    Friend WithEvents btnCopierAdressePatient As Button
    Friend WithEvents lblAdresseLigne2 As Label
    Friend WithEvents txtAdresseLigne2 As TextBox
    Friend WithEvents lblCodePostal As Label
    Friend WithEvents txtCodePostal As TextBox
    Friend WithEvents lblLocalite As Label
    Friend WithEvents txtLocalite As TextBox
    Friend WithEvents grpRoles As GroupBox
    Friend WithEvents lblRoleLegal As Label
    Friend WithEvents cboRoleLegal As ComboBox
    Friend WithEvents btnAjouterRole As Button
    Friend WithEvents grpCommentaire As GroupBox
    Friend WithEvents rteCommentaire As UC_RichTextEditorSimple
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents pnlAction As Panel
    Friend WithEvents btnEnregistrer As Button
    Friend WithEvents btnAnnuler As Button
    Friend WithEvents btnModifier As Button
    Friend WithEvents btnFermer As Button
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTop As Label

End Class
