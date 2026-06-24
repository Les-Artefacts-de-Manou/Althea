<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TherapeuteEdition
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TherapeuteEdition))
        lblTitreForm = New Label()
        grpIdentite = New GroupBox()
        lblNom = New Label()
        txtNom = New TextBox()
        lblPrenom = New Label()
        txtPrenom = New TextBox()
        lblSpecialite = New Label()
        txtSpecialite = New TextBox()
        chkActif = New CheckBox()
        grpCoordonnees = New GroupBox()
        lblTelephone = New Label()
        txtTelephone = New TextBox()
        lblEmail = New Label()
        txtEmail = New TextBox()
        lblPays = New Label()
        cboPays = New ComboBox()
        lblAdresseLigne1 = New Label()
        txtAdresseLigne1 = New TextBox()
        lblAdresseLigne2 = New Label()
        txtAdresseLigne2 = New TextBox()
        lblCodePostal = New Label()
        txtCodePostal = New TextBox()
        lblLocalite = New Label()
        txtLocalite = New TextBox()
        btnEnregistrer = New Button()
        btnAnnuler = New Button()
        ttMain = New ToolTip(components)
        errProvider = New ErrorProvider(components)
        pnlAction = New Panel()
        pnlTitre = New Panel()
        lblTop = New Label()
        grpCommentaire = New GroupBox()
        rteCommentaire = New UC_RichTextEditorSimple()
        grpIdentite.SuspendLayout()
        grpCoordonnees.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        pnlAction.SuspendLayout()
        pnlTitre.SuspendLayout()
        grpCommentaire.SuspendLayout()
        SuspendLayout()
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.AutoSize = True
        lblTitreForm.Font = New Font("Calibri", 14F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(58, 19)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Size = New Size(102, 23)
        lblTitreForm.TabIndex = 0
        lblTitreForm.Text = "Thérapeute"
        ' 
        ' grpIdentite
        ' 
        grpIdentite.Controls.Add(lblNom)
        grpIdentite.Controls.Add(txtNom)
        grpIdentite.Controls.Add(lblPrenom)
        grpIdentite.Controls.Add(txtPrenom)
        grpIdentite.Controls.Add(lblSpecialite)
        grpIdentite.Controls.Add(txtSpecialite)
        grpIdentite.Controls.Add(chkActif)
        grpIdentite.Font = New Font("Calibri", 10F, FontStyle.Bold)
        grpIdentite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpIdentite.Location = New Point(16, 68)
        grpIdentite.Name = "grpIdentite"
        grpIdentite.Size = New Size(688, 130)
        grpIdentite.TabIndex = 1
        grpIdentite.TabStop = False
        grpIdentite.Text = "Identité du thérapeute"
        ' 
        ' lblNom
        ' 
        lblNom.AutoSize = True
        lblNom.Font = New Font("Calibri", 10F)
        lblNom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNom.Location = New Point(16, 33)
        lblNom.Name = "lblNom"
        lblNom.Size = New Size(35, 17)
        lblNom.TabIndex = 0
        lblNom.Text = "Nom"
        ' 
        ' txtNom
        ' 
        txtNom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNom.Font = New Font("Calibri", 10F)
        txtNom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNom.Location = New Point(130, 30)
        txtNom.MaxLength = 100
        txtNom.Name = "txtNom"
        txtNom.Size = New Size(230, 24)
        txtNom.TabIndex = 1
        ' 
        ' lblPrenom
        ' 
        lblPrenom.AutoSize = True
        lblPrenom.Font = New Font("Calibri", 10F)
        lblPrenom.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPrenom.Location = New Point(16, 66)
        lblPrenom.Name = "lblPrenom"
        lblPrenom.Size = New Size(52, 17)
        lblPrenom.TabIndex = 2
        lblPrenom.Text = "Prénom"
        ' 
        ' txtPrenom
        ' 
        txtPrenom.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtPrenom.Font = New Font("Calibri", 10F)
        txtPrenom.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPrenom.Location = New Point(130, 63)
        txtPrenom.MaxLength = 100
        txtPrenom.Name = "txtPrenom"
        txtPrenom.Size = New Size(230, 24)
        txtPrenom.TabIndex = 3
        ' 
        ' lblSpecialite
        ' 
        lblSpecialite.AutoSize = True
        lblSpecialite.Font = New Font("Calibri", 10F)
        lblSpecialite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblSpecialite.Location = New Point(16, 99)
        lblSpecialite.Name = "lblSpecialite"
        lblSpecialite.Size = New Size(62, 17)
        lblSpecialite.TabIndex = 4
        lblSpecialite.Text = "Spécialité"
        ' 
        ' txtSpecialite
        ' 
        txtSpecialite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtSpecialite.Font = New Font("Calibri", 10F)
        txtSpecialite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtSpecialite.Location = New Point(130, 96)
        txtSpecialite.MaxLength = 100
        txtSpecialite.Name = "txtSpecialite"
        txtSpecialite.Size = New Size(230, 24)
        txtSpecialite.TabIndex = 5
        ' 
        ' chkActif
        ' 
        chkActif.AutoSize = True
        chkActif.Font = New Font("Calibri", 10F)
        chkActif.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        chkActif.Location = New Point(410, 32)
        chkActif.Name = "chkActif"
        chkActif.Size = New Size(53, 21)
        chkActif.TabIndex = 6
        chkActif.Text = "Actif"
        chkActif.UseVisualStyleBackColor = True
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
        grpCoordonnees.Controls.Add(lblAdresseLigne2)
        grpCoordonnees.Controls.Add(txtAdresseLigne2)
        grpCoordonnees.Controls.Add(lblCodePostal)
        grpCoordonnees.Controls.Add(txtCodePostal)
        grpCoordonnees.Controls.Add(lblLocalite)
        grpCoordonnees.Controls.Add(txtLocalite)
        grpCoordonnees.Font = New Font("Calibri", 10F, FontStyle.Bold)
        grpCoordonnees.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpCoordonnees.Location = New Point(16, 204)
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
        txtAdresseLigne1.Size = New Size(542, 24)
        txtAdresseLigne1.TabIndex = 7
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
        ' btnEnregistrer
        ' 
        btnEnregistrer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnEnregistrer.FlatAppearance.BorderSize = 0
        btnEnregistrer.FlatStyle = FlatStyle.Flat
        btnEnregistrer.Font = New Font("Calibri", 10F)
        btnEnregistrer.ForeColor = Color.White
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.ImageAlign = ContentAlignment.MiddleLeft
        btnEnregistrer.Location = New Point(243, 10)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(112, 40)
        btnEnregistrer.TabIndex = 4
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEnregistrer.UseVisualStyleBackColor = False
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAnnuler.FlatAppearance.BorderSize = 0
        btnAnnuler.FlatStyle = FlatStyle.Flat
        btnAnnuler.Font = New Font("Calibri", 10F)
        btnAnnuler.ForeColor = Color.White
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.ImageAlign = ContentAlignment.MiddleLeft
        btnAnnuler.Location = New Point(367, 10)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(112, 40)
        btnAnnuler.TabIndex = 5
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAnnuler.UseVisualStyleBackColor = False
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
        pnlAction.Controls.Add(btnAnnuler)
        pnlAction.Controls.Add(btnEnregistrer)
        pnlAction.Dock = DockStyle.Bottom
        pnlAction.Location = New Point(0, 568)
        pnlAction.Name = "pnlAction"
        pnlAction.Size = New Size(720, 62)
        pnlAction.TabIndex = 6
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
        pnlTitre.TabIndex = 8
        ' 
        ' lblTop
        ' 
        lblTop.Font = New Font("Calibri", 12F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(341, 9)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(367, 44)
        lblTop.TabIndex = 4
        lblTop.Text = "Gestion des informations et coordonnées des différents thérapeutes qui suivent le patient"
        ' 
        ' grpCommentaire
        ' 
        grpCommentaire.Controls.Add(rteCommentaire)
        grpCommentaire.Font = New Font("Calibri", 10F, FontStyle.Bold)
        grpCommentaire.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpCommentaire.Location = New Point(16, 406)
        grpCommentaire.Name = "grpCommentaire"
        grpCommentaire.Size = New Size(688, 156)
        grpCommentaire.TabIndex = 9
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
        rteCommentaire.Size = New Size(682, 133)
        rteCommentaire.TabIndex = 0
        ' 
        ' TherapeuteEdition
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        BackgroundImage = My.Resources.Resources.Fond_1000x800_Feuille4
        BackgroundImageLayout = ImageLayout.Stretch
        ClientSize = New Size(720, 630)
        Controls.Add(grpCommentaire)
        Controls.Add(pnlTitre)
        Controls.Add(pnlAction)
        Controls.Add(grpIdentite)
        Controls.Add(grpCoordonnees)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "TherapeuteEdition"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Thérapeute"
        grpIdentite.ResumeLayout(False)
        grpIdentite.PerformLayout()
        grpCoordonnees.ResumeLayout(False)
        grpCoordonnees.PerformLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        pnlAction.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        grpCommentaire.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

    Friend WithEvents lblTitreForm As Label
    Friend WithEvents grpIdentite As GroupBox
    Friend WithEvents lblNom As Label
    Friend WithEvents txtNom As TextBox
    Friend WithEvents lblPrenom As Label
    Friend WithEvents txtPrenom As TextBox
    Friend WithEvents lblSpecialite As Label
    Friend WithEvents txtSpecialite As TextBox
    Friend WithEvents chkActif As CheckBox
    Friend WithEvents grpCoordonnees As GroupBox
    Friend WithEvents lblTelephone As Label
    Friend WithEvents txtTelephone As TextBox
    Friend WithEvents lblEmail As Label
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents lblPays As Label
    Friend WithEvents cboPays As ComboBox
    Friend WithEvents lblAdresseLigne1 As Label
    Friend WithEvents txtAdresseLigne1 As TextBox
    Friend WithEvents lblAdresseLigne2 As Label
    Friend WithEvents txtAdresseLigne2 As TextBox
    Friend WithEvents lblCodePostal As Label
    Friend WithEvents txtCodePostal As TextBox
    Friend WithEvents lblLocalite As Label
    Friend WithEvents txtLocalite As TextBox
    Friend WithEvents btnEnregistrer As Button
    Friend WithEvents btnAnnuler As Button
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents pnlAction As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents grpCommentaire As GroupBox
    Friend WithEvents rteCommentaire As UC_RichTextEditorSimple
    Friend WithEvents lblTop As Label

End Class
