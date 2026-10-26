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
        grpIdentite = New AltheaCarte()
        lblLien = New Label()
        cboLien = New AltheaListe()
        btnAjouterLien = New AltheaBouton()
        lblNom = New Label()
        txtNom = New TextBox()
        lblPrenom = New Label()
        txtPrenom = New TextBox()
        lblDateNaissance = New Label()
        dtpDateNaissance = New AltheaDate()
        cboCivilite = New AltheaListe()
        lblCivilite = New Label()
        grpCoordonnees = New AltheaCarte()
        lblTelephone = New Label()
        txtTelephone = New TextBox()
        lblEmail = New Label()
        txtEmail = New TextBox()
        lblPays = New Label()
        cboPays = New AltheaListe()
        lblAdresseLigne1 = New Label()
        txtAdresseLigne1 = New TextBox()
        btnCopierAdressePatient = New AltheaBouton()
        lblAdresseLigne2 = New Label()
        txtAdresseLigne2 = New TextBox()
        lblCodePostal = New Label()
        txtCodePostal = New TextBox()
        lblLocalite = New Label()
        txtLocalite = New TextBox()
        grpRoles = New AltheaCarte()
        lblRoleLegal = New Label()
        cboRoleLegal = New AltheaListe()
        btnAjouterRole = New AltheaBouton()
        grpCommentaire = New AltheaCarte()
        rteCommentaire = New UC_RichTextEditor()
        ttMain = New ToolTip(components)
        errProvider = New ErrorProvider(components)
        pnlAction = New AltheaBarreActions()
        btnModifier = New AltheaBouton()
        btnFermer = New AltheaBouton()
        btnEnregistrer = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        enTete = New AltheaEnTete()
        grpIdentite.SuspendLayout()
        grpCoordonnees.SuspendLayout()
        grpRoles.SuspendLayout()
        grpCommentaire.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        pnlAction.SuspendLayout()
        SuspendLayout()
        ' 
        ' grpIdentite
        ' 
        grpIdentite.Controls.Add(lblCivilite)
        grpIdentite.Controls.Add(cboCivilite)
        grpIdentite.Controls.Add(lblLien)
        grpIdentite.Controls.Add(cboLien)
        grpIdentite.Controls.Add(btnAjouterLien)
        grpIdentite.Controls.Add(lblNom)
        grpIdentite.Controls.Add(txtNom)
        grpIdentite.Controls.Add(lblPrenom)
        grpIdentite.Controls.Add(txtPrenom)
        grpIdentite.Controls.Add(lblDateNaissance)
        grpIdentite.Controls.Add(dtpDateNaissance)
        grpIdentite.Location = New Point(16, 70)
        grpIdentite.Name = "grpIdentite"
        grpIdentite.Padding = New Padding(12, 8, 12, 10)
        grpIdentite.Size = New Size(688, 138)
        grpIdentite.TabIndex = 1
        grpIdentite.TabStop = False
        grpIdentite.Titre = "Identité du contact"
        ' 
        ' lblCivilite
        ' 
        lblCivilite.AutoSize = True
        lblCivilite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCivilite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCivilite.Location = New Point(400, 43)
        lblCivilite.Name = "lblCivilite"
        lblCivilite.Size = New Size(50, 17)
        lblCivilite.TabIndex = 9
        lblCivilite.Text = "Civilité"
        ' 
        ' cboCivilite
        ' 
        cboCivilite.Font = New Font("Calibri", 11F)
        cboCivilite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboCivilite.ItemHeight = 20
        cboCivilite.Location = New Point(530, 40)
        cboCivilite.Name = "cboCivilite"
        cboCivilite.Size = New Size(142, 26)
        cboCivilite.TabIndex = 10
        ' 
        ' lblLien
        ' 
        lblLien.AutoSize = True
        lblLien.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblLien.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLien.Location = New Point(16, 43)
        lblLien.Name = "lblLien"
        lblLien.Size = New Size(31, 17)
        lblLien.TabIndex = 0
        lblLien.Text = "Lien"
        ' 
        ' cboLien
        ' 
        cboLien.Font = New Font("Calibri", 11F)
        cboLien.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboLien.ItemHeight = 20
        cboLien.Location = New Point(130, 40)
        cboLien.Name = "cboLien"
        cboLien.Size = New Size(230, 26)
        cboLien.TabIndex = 1
        ' 
        ' btnAjouterLien
        ' 
        btnAjouterLien.Image = CType(resources.GetObject("btnAjouterLien.Image"), Image)
        btnAjouterLien.Location = New Point(364, 38)
        btnAjouterLien.Name = "btnAjouterLien"
        btnAjouterLien.Size = New Size(30, 30)
        btnAjouterLien.TabIndex = 2
        btnAjouterLien.Tag = "plus_24_normal"
        btnAjouterLien.Variante = VarianteBouton.Outil
        ' 
        ' lblNom
        ' 
        lblNom.AutoSize = True
        lblNom.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNom.Location = New Point(16, 74)
        lblNom.Name = "lblNom"
        lblNom.Size = New Size(35, 17)
        lblNom.TabIndex = 3
        lblNom.Text = "Nom"
        ' 
        ' txtNom
        ' 
        txtNom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNom.Font = New Font("Calibri", 11F)
        txtNom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNom.Location = New Point(130, 71)
        txtNom.MaxLength = 100
        txtNom.Name = "txtNom"
        txtNom.Size = New Size(230, 25)
        txtNom.TabIndex = 4
        ' 
        ' lblPrenom
        ' 
        lblPrenom.AutoSize = True
        lblPrenom.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPrenom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPrenom.Location = New Point(16, 105)
        lblPrenom.Name = "lblPrenom"
        lblPrenom.Size = New Size(52, 17)
        lblPrenom.TabIndex = 5
        lblPrenom.Text = "Prénom"
        ' 
        ' txtPrenom
        ' 
        txtPrenom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtPrenom.Font = New Font("Calibri", 11F)
        txtPrenom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPrenom.Location = New Point(130, 102)
        txtPrenom.MaxLength = 100
        txtPrenom.Name = "txtPrenom"
        txtPrenom.Size = New Size(230, 25)
        txtPrenom.TabIndex = 6
        ' 
        ' lblDateNaissance
        ' 
        lblDateNaissance.AutoSize = True
        lblDateNaissance.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDateNaissance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateNaissance.Location = New Point(400, 74)
        lblDateNaissance.Name = "lblDateNaissance"
        lblDateNaissance.Size = New Size(110, 17)
        lblDateNaissance.TabIndex = 7
        lblDateNaissance.Text = "Date de naissance"
        ' 
        ' dtpDateNaissance
        ' 
        dtpDateNaissance.Facultative = True
        dtpDateNaissance.Font = New Font("Calibri", 11F)
        dtpDateNaissance.Location = New Point(530, 71)
        dtpDateNaissance.Name = "dtpDateNaissance"
        dtpDateNaissance.Size = New Size(142, 26)
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
        grpCoordonnees.Location = New Point(16, 216)
        grpCoordonnees.Name = "grpCoordonnees"
        grpCoordonnees.Padding = New Padding(12, 8, 12, 10)
        grpCoordonnees.Size = New Size(688, 200)
        grpCoordonnees.TabIndex = 2
        grpCoordonnees.TabStop = False
        grpCoordonnees.Titre = "Coordonnées"
        ' 
        ' lblTelephone
        ' 
        lblTelephone.AutoSize = True
        lblTelephone.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTelephone.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTelephone.Location = New Point(16, 74)
        lblTelephone.Name = "lblTelephone"
        lblTelephone.Size = New Size(66, 17)
        lblTelephone.TabIndex = 2
        lblTelephone.Text = "Téléphone"
        ' 
        ' txtTelephone
        ' 
        txtTelephone.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtTelephone.Font = New Font("Calibri", 11F)
        txtTelephone.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtTelephone.Location = New Point(130, 71)
        txtTelephone.MaxLength = 50
        txtTelephone.Name = "txtTelephone"
        txtTelephone.Size = New Size(230, 25)
        txtTelephone.TabIndex = 3
        ' 
        ' lblEmail
        ' 
        lblEmail.AutoSize = True
        lblEmail.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblEmail.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblEmail.Location = New Point(400, 74)
        lblEmail.Name = "lblEmail"
        lblEmail.Size = New Size(43, 17)
        lblEmail.TabIndex = 4
        lblEmail.Text = "E-mail"
        ' 
        ' txtEmail
        ' 
        txtEmail.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtEmail.Font = New Font("Calibri", 11F)
        txtEmail.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtEmail.Location = New Point(460, 71)
        txtEmail.MaxLength = 150
        txtEmail.Name = "txtEmail"
        txtEmail.Size = New Size(212, 25)
        txtEmail.TabIndex = 5
        ' 
        ' lblPays
        ' 
        lblPays.AutoSize = True
        lblPays.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPays.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPays.Location = New Point(16, 43)
        lblPays.Name = "lblPays"
        lblPays.Size = New Size(33, 17)
        lblPays.TabIndex = 0
        lblPays.Text = "Pays"
        ' 
        ' cboPays
        ' 
        cboPays.Font = New Font("Calibri", 11F)
        cboPays.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboPays.ItemHeight = 20
        cboPays.Location = New Point(130, 40)
        cboPays.Name = "cboPays"
        cboPays.Size = New Size(230, 26)
        cboPays.TabIndex = 1
        ' 
        ' lblAdresseLigne1
        ' 
        lblAdresseLigne1.AutoSize = True
        lblAdresseLigne1.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblAdresseLigne1.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAdresseLigne1.Location = New Point(16, 105)
        lblAdresseLigne1.Name = "lblAdresseLigne1"
        lblAdresseLigne1.Size = New Size(52, 17)
        lblAdresseLigne1.TabIndex = 6
        lblAdresseLigne1.Text = "Adresse"
        ' 
        ' txtAdresseLigne1
        ' 
        txtAdresseLigne1.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdresseLigne1.Font = New Font("Calibri", 11F)
        txtAdresseLigne1.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdresseLigne1.Location = New Point(130, 102)
        txtAdresseLigne1.MaxLength = 255
        txtAdresseLigne1.Name = "txtAdresseLigne1"
        txtAdresseLigne1.Size = New Size(505, 25)
        txtAdresseLigne1.TabIndex = 7
        ' 
        ' btnCopierAdressePatient
        ' 
        btnCopierAdressePatient.Image = CType(resources.GetObject("btnCopierAdressePatient.Image"), Image)
        btnCopierAdressePatient.Location = New Point(642, 100)
        btnCopierAdressePatient.Name = "btnCopierAdressePatient"
        btnCopierAdressePatient.Size = New Size(30, 30)
        btnCopierAdressePatient.TabIndex = 8
        btnCopierAdressePatient.Tag = "flechebas_24_normal"
        btnCopierAdressePatient.Variante = VarianteBouton.Outil
        ' 
        ' lblAdresseLigne2
        ' 
        lblAdresseLigne2.AutoSize = True
        lblAdresseLigne2.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblAdresseLigne2.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAdresseLigne2.Location = New Point(16, 136)
        lblAdresseLigne2.Name = "lblAdresseLigne2"
        lblAdresseLigne2.Size = New Size(80, 17)
        lblAdresseLigne2.TabIndex = 8
        lblAdresseLigne2.Text = "Complément"
        ' 
        ' txtAdresseLigne2
        ' 
        txtAdresseLigne2.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdresseLigne2.Font = New Font("Calibri", 11F)
        txtAdresseLigne2.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdresseLigne2.Location = New Point(130, 133)
        txtAdresseLigne2.MaxLength = 255
        txtAdresseLigne2.Name = "txtAdresseLigne2"
        txtAdresseLigne2.Size = New Size(542, 25)
        txtAdresseLigne2.TabIndex = 9
        ' 
        ' lblCodePostal
        ' 
        lblCodePostal.AutoSize = True
        lblCodePostal.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCodePostal.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCodePostal.Location = New Point(16, 167)
        lblCodePostal.Name = "lblCodePostal"
        lblCodePostal.Size = New Size(73, 17)
        lblCodePostal.TabIndex = 10
        lblCodePostal.Text = "Code postal"
        ' 
        ' txtCodePostal
        ' 
        txtCodePostal.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCodePostal.Font = New Font("Calibri", 11F)
        txtCodePostal.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCodePostal.Location = New Point(130, 164)
        txtCodePostal.MaxLength = 20
        txtCodePostal.Name = "txtCodePostal"
        txtCodePostal.Size = New Size(120, 25)
        txtCodePostal.TabIndex = 11
        ' 
        ' lblLocalite
        ' 
        lblLocalite.AutoSize = True
        lblLocalite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblLocalite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLocalite.Location = New Point(286, 167)
        lblLocalite.Name = "lblLocalite"
        lblLocalite.Size = New Size(52, 17)
        lblLocalite.TabIndex = 12
        lblLocalite.Text = "Localité"
        ' 
        ' txtLocalite
        ' 
        txtLocalite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtLocalite.Font = New Font("Calibri", 11F)
        txtLocalite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLocalite.Location = New Point(346, 164)
        txtLocalite.MaxLength = 100
        txtLocalite.Name = "txtLocalite"
        txtLocalite.Size = New Size(326, 25)
        txtLocalite.TabIndex = 13
        ' 
        ' grpRoles
        ' 
        grpRoles.Controls.Add(lblRoleLegal)
        grpRoles.Controls.Add(cboRoleLegal)
        grpRoles.Controls.Add(btnAjouterRole)
        grpRoles.Location = New Point(16, 424)
        grpRoles.Name = "grpRoles"
        grpRoles.Padding = New Padding(12, 8, 12, 10)
        grpRoles.Size = New Size(688, 76)
        grpRoles.TabIndex = 3
        grpRoles.TabStop = False
        grpRoles.Titre = "Rôle légal"
        ' 
        ' lblRoleLegal
        ' 
        lblRoleLegal.AutoSize = True
        lblRoleLegal.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblRoleLegal.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRoleLegal.Location = New Point(16, 43)
        lblRoleLegal.Name = "lblRoleLegal"
        lblRoleLegal.Size = New Size(63, 17)
        lblRoleLegal.TabIndex = 0
        lblRoleLegal.Text = "Rôle légal"
        ' 
        ' cboRoleLegal
        ' 
        cboRoleLegal.Font = New Font("Calibri", 11F)
        cboRoleLegal.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboRoleLegal.ItemHeight = 20
        cboRoleLegal.Location = New Point(130, 40)
        cboRoleLegal.Name = "cboRoleLegal"
        cboRoleLegal.Size = New Size(230, 26)
        cboRoleLegal.TabIndex = 1
        ' 
        ' btnAjouterRole
        ' 
        btnAjouterRole.Image = CType(resources.GetObject("btnAjouterRole.Image"), Image)
        btnAjouterRole.Location = New Point(364, 38)
        btnAjouterRole.Name = "btnAjouterRole"
        btnAjouterRole.Size = New Size(30, 30)
        btnAjouterRole.TabIndex = 2
        btnAjouterRole.Tag = "plus_24_normal"
        btnAjouterRole.Variante = VarianteBouton.Outil
        ' 
        ' grpCommentaire
        ' 
        grpCommentaire.Controls.Add(rteCommentaire)
        grpCommentaire.Location = New Point(16, 508)
        grpCommentaire.Name = "grpCommentaire"
        grpCommentaire.Padding = New Padding(12, 8, 12, 10)
        grpCommentaire.Size = New Size(688, 140)
        grpCommentaire.TabIndex = 4
        grpCommentaire.TabStop = False
        grpCommentaire.Titre = "Commentaire"
        ' 
        ' rteCommentaire
        ' 
        rteCommentaire.BackColor = Color.Transparent
        rteCommentaire.Dock = DockStyle.Fill
        rteCommentaire.Location = New Point(12, 45)
        rteCommentaire.Margin = New Padding(4, 3, 4, 3)
        rteCommentaire.Name = "rteCommentaire"
        rteCommentaire.Profil = ProfilEditeur.Compact
        rteCommentaire.TitreDocument = "Commentaire du contact"
        rteCommentaire.Size = New Size(664, 85)
        rteCommentaire.TabIndex = 0
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' pnlAction
        ' 
        pnlAction.Controls.Add(btnModifier)
        pnlAction.Controls.Add(btnFermer)
        pnlAction.Controls.Add(btnEnregistrer)
        pnlAction.Controls.Add(btnAnnuler)
        pnlAction.Dock = DockStyle.Bottom
        pnlAction.Location = New Point(0, 656)
        pnlAction.Name = "pnlAction"
        pnlAction.Size = New Size(720, 62)
        pnlAction.TabIndex = 5
        ' 
        ' btnModifier
        ' 
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(215, 11)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(132, 40)
        btnModifier.TabIndex = 9
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        ' 
        ' btnFermer
        ' 
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.Location = New Point(357, 11)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(114, 40)
        btnFermer.TabIndex = 10
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(215, 11)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 40)
        btnEnregistrer.TabIndex = 7
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(357, 11)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 40)
        btnAnnuler.TabIndex = 8
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(720, 62)
        enTete.TabIndex = 6
        enTete.SousTitre = "Gérez les contacts et l'entourage du patient"
        enTete.Text = "Contact"
        ' 
        ' ContactEdition
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(720, 718)
        Controls.Add(enTete)
        Controls.Add(pnlAction)
        Controls.Add(grpIdentite)
        Controls.Add(grpCoordonnees)
        Controls.Add(grpRoles)
        Controls.Add(grpCommentaire)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
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
        ResumeLayout(False)

    End Sub
    Friend WithEvents lblCivilite As Label
    Friend WithEvents cboCivilite As AltheaListe
    Friend WithEvents grpIdentite As AltheaCarte
    Friend WithEvents lblLien As Label
    Friend WithEvents cboLien As AltheaListe
    Friend WithEvents btnAjouterLien As AltheaBouton
    Friend WithEvents lblNom As Label
    Friend WithEvents txtNom As TextBox
    Friend WithEvents lblPrenom As Label
    Friend WithEvents txtPrenom As TextBox
    Friend WithEvents lblDateNaissance As Label
    Friend WithEvents dtpDateNaissance As AltheaDate
    Friend WithEvents grpCoordonnees As AltheaCarte
    Friend WithEvents lblTelephone As Label
    Friend WithEvents txtTelephone As TextBox
    Friend WithEvents lblEmail As Label
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents lblPays As Label
    Friend WithEvents cboPays As AltheaListe
    Friend WithEvents lblAdresseLigne1 As Label
    Friend WithEvents txtAdresseLigne1 As TextBox
    Friend WithEvents btnCopierAdressePatient As AltheaBouton
    Friend WithEvents lblAdresseLigne2 As Label
    Friend WithEvents txtAdresseLigne2 As TextBox
    Friend WithEvents lblCodePostal As Label
    Friend WithEvents txtCodePostal As TextBox
    Friend WithEvents lblLocalite As Label
    Friend WithEvents txtLocalite As TextBox
    Friend WithEvents grpRoles As AltheaCarte
    Friend WithEvents lblRoleLegal As Label
    Friend WithEvents cboRoleLegal As AltheaListe
    Friend WithEvents btnAjouterRole As AltheaBouton
    Friend WithEvents grpCommentaire As AltheaCarte
    Friend WithEvents rteCommentaire As UC_RichTextEditor
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents pnlAction As AltheaBarreActions
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents btnFermer As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete

End Class
