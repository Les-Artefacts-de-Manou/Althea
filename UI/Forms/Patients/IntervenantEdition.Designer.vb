<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class IntervenantEdition
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IntervenantEdition))
        grpIntervenant = New AltheaCarte()
        btnVoirTherapeutes = New AltheaBouton()
        txtTelephone = New TextBox()
        lblTelephone = New Label()
        txtEmail = New TextBox()
        lblEmail = New Label()
        lblTherapeute = New Label()
        cboTherapeute = New AltheaListe()
        btnAjouterTherapeute = New AltheaBouton()
        lblRole = New Label()
        cboRole = New AltheaListe()
        btnAjouterRole = New AltheaBouton()
        lblNomProfessionnel = New Label()
        txtNomProfessionnel = New TextBox()
        lblSpecialite = New Label()
        txtSpecialite = New TextBox()
        lblAdresse = New Label()
        txtAdresse = New TextBox()
        txtCivilite = New TextBox()
        lblCivilite = New Label()
        grpPeriode = New AltheaCarte()
        lblDateDebut = New Label()
        dtpDateDebut = New AltheaDate()
        lblDateFin = New Label()
        dtpDateFin = New AltheaDate()
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
        grpIntervenant.SuspendLayout()
        grpPeriode.SuspendLayout()
        grpCommentaire.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        pnlAction.SuspendLayout()
        SuspendLayout()
        ' 
        ' grpIntervenant
        ' 
        grpIntervenant.Controls.Add(lblCivilite)
        grpIntervenant.Controls.Add(txtCivilite)
        grpIntervenant.Controls.Add(btnVoirTherapeutes)
        grpIntervenant.Controls.Add(txtTelephone)
        grpIntervenant.Controls.Add(lblTelephone)
        grpIntervenant.Controls.Add(lblEmail)
        grpIntervenant.Controls.Add(txtEmail)
        grpIntervenant.Controls.Add(lblTherapeute)
        grpIntervenant.Controls.Add(cboTherapeute)
        grpIntervenant.Controls.Add(btnAjouterTherapeute)
        grpIntervenant.Controls.Add(lblRole)
        grpIntervenant.Controls.Add(cboRole)
        grpIntervenant.Controls.Add(btnAjouterRole)
        grpIntervenant.Controls.Add(lblNomProfessionnel)
        grpIntervenant.Controls.Add(txtNomProfessionnel)
        grpIntervenant.Controls.Add(lblSpecialite)
        grpIntervenant.Controls.Add(txtSpecialite)
        grpIntervenant.Controls.Add(lblAdresse)
        grpIntervenant.Controls.Add(txtAdresse)
        grpIntervenant.Location = New Point(16, 70)
        grpIntervenant.Name = "grpIntervenant"
        grpIntervenant.Padding = New Padding(12, 8, 12, 10)
        grpIntervenant.Size = New Size(688, 232)
        grpIntervenant.TabIndex = 1
        grpIntervenant.TabStop = False
        grpIntervenant.Titre = "Intervenant"
        ' 
        ' lblCivilite
        ' 
        lblCivilite.AutoSize = True
        lblCivilite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCivilite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCivilite.Location = New Point(440, 74)
        lblCivilite.Name = "lblCivilite"
        lblCivilite.Size = New Size(50, 17)
        lblCivilite.TabIndex = 15
        lblCivilite.Text = "Civilité"
        ' 
        ' txtCivilite
        ' 
        txtCivilite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCivilite.Font = New Font("Calibri", 11F)
        txtCivilite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtCivilite.Location = New Point(522, 71)
        txtCivilite.Name = "txtCivilite"
        txtCivilite.ReadOnly = True
        txtCivilite.Size = New Size(150, 25)
        txtCivilite.TabIndex = 16
        ' 
        ' btnVoirTherapeutes
        ' 
        btnVoirTherapeutes.Image = CType(resources.GetObject("btnVoirTherapeutes.Image"), Image)
        btnVoirTherapeutes.Location = New Point(398, 38)
        btnVoirTherapeutes.Name = "btnVoirTherapeutes"
        btnVoirTherapeutes.Size = New Size(30, 30)
        btnVoirTherapeutes.TabIndex = 14
        btnVoirTherapeutes.Tag = "recherche_24_normal"
        btnVoirTherapeutes.Variante = VarianteBouton.Outil
        ' 
        ' txtTelephone
        ' 
        txtTelephone.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtTelephone.Font = New Font("Calibri", 11F)
        txtTelephone.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtTelephone.Location = New Point(130, 195)
        txtTelephone.MaxLength = 150
        txtTelephone.Name = "txtTelephone"
        txtTelephone.ReadOnly = True
        txtTelephone.Size = New Size(239, 25)
        txtTelephone.TabIndex = 13
        ' 
        ' lblTelephone
        ' 
        lblTelephone.AutoSize = True
        lblTelephone.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTelephone.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTelephone.Location = New Point(16, 198)
        lblTelephone.Name = "lblTelephone"
        lblTelephone.Size = New Size(66, 17)
        lblTelephone.TabIndex = 12
        lblTelephone.Text = "Téléphone"
        ' 
        ' lblEmail
        ' 
        lblEmail.AutoSize = True
        lblEmail.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblEmail.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblEmail.Location = New Point(385, 198)
        lblEmail.Name = "lblEmail"
        lblEmail.Size = New Size(45, 17)
        lblEmail.TabIndex = 17
        lblEmail.Text = "E-mail"
        ' 
        ' txtEmail
        ' 
        txtEmail.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtEmail.Font = New Font("Calibri", 11F)
        txtEmail.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtEmail.Location = New Point(440, 195)
        txtEmail.Name = "txtEmail"
        txtEmail.ReadOnly = True
        txtEmail.Size = New Size(232, 25)
        txtEmail.TabIndex = 18
        ' 
        ' lblTherapeute
        ' 
        lblTherapeute.AutoSize = True
        lblTherapeute.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTherapeute.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTherapeute.Location = New Point(16, 43)
        lblTherapeute.Name = "lblTherapeute"
        lblTherapeute.Size = New Size(74, 17)
        lblTherapeute.TabIndex = 0
        lblTherapeute.Text = "Thérapeute"
        ' 
        ' cboTherapeute
        ' 
        cboTherapeute.Font = New Font("Calibri", 11F)
        cboTherapeute.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboTherapeute.ItemHeight = 20
        cboTherapeute.Location = New Point(130, 40)
        cboTherapeute.Name = "cboTherapeute"
        cboTherapeute.Size = New Size(230, 26)
        cboTherapeute.TabIndex = 1
        ' 
        ' btnAjouterTherapeute
        ' 
        btnAjouterTherapeute.Image = CType(resources.GetObject("btnAjouterTherapeute.Image"), Image)
        btnAjouterTherapeute.Location = New Point(364, 38)
        btnAjouterTherapeute.Name = "btnAjouterTherapeute"
        btnAjouterTherapeute.Size = New Size(30, 30)
        btnAjouterTherapeute.TabIndex = 2
        btnAjouterTherapeute.Tag = "plus_24_normal"
        btnAjouterTherapeute.Variante = VarianteBouton.Outil
        ' 
        ' lblRole
        ' 
        lblRole.AutoSize = True
        lblRole.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblRole.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRole.Location = New Point(16, 74)
        lblRole.Name = "lblRole"
        lblRole.Size = New Size(33, 17)
        lblRole.TabIndex = 3
        lblRole.Text = "Rôle"
        ' 
        ' cboRole
        ' 
        cboRole.Font = New Font("Calibri", 11F)
        cboRole.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboRole.ItemHeight = 20
        cboRole.Location = New Point(130, 71)
        cboRole.Name = "cboRole"
        cboRole.Size = New Size(230, 26)
        cboRole.TabIndex = 4
        ' 
        ' btnAjouterRole
        ' 
        btnAjouterRole.Image = CType(resources.GetObject("btnAjouterRole.Image"), Image)
        btnAjouterRole.Location = New Point(364, 69)
        btnAjouterRole.Name = "btnAjouterRole"
        btnAjouterRole.Size = New Size(30, 30)
        btnAjouterRole.TabIndex = 5
        btnAjouterRole.Tag = "plus_24_normal"
        btnAjouterRole.Variante = VarianteBouton.Outil
        ' 
        ' lblNomProfessionnel
        ' 
        lblNomProfessionnel.AutoSize = True
        lblNomProfessionnel.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNomProfessionnel.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNomProfessionnel.Location = New Point(16, 105)
        lblNomProfessionnel.Name = "lblNomProfessionnel"
        lblNomProfessionnel.Size = New Size(96, 17)
        lblNomProfessionnel.TabIndex = 6
        lblNomProfessionnel.Text = "Nom / praticien"
        ' 
        ' txtNomProfessionnel
        ' 
        txtNomProfessionnel.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNomProfessionnel.Font = New Font("Calibri", 11F)
        txtNomProfessionnel.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNomProfessionnel.Location = New Point(130, 102)
        txtNomProfessionnel.MaxLength = 150
        txtNomProfessionnel.Name = "txtNomProfessionnel"
        txtNomProfessionnel.ReadOnly = True
        txtNomProfessionnel.Size = New Size(542, 25)
        txtNomProfessionnel.TabIndex = 7
        ' 
        ' lblSpecialite
        ' 
        lblSpecialite.AutoSize = True
        lblSpecialite.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblSpecialite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblSpecialite.Location = New Point(16, 136)
        lblSpecialite.Name = "lblSpecialite"
        lblSpecialite.Size = New Size(62, 17)
        lblSpecialite.TabIndex = 8
        lblSpecialite.Text = "Spécialité"
        ' 
        ' txtSpecialite
        ' 
        txtSpecialite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtSpecialite.Font = New Font("Calibri", 11F)
        txtSpecialite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtSpecialite.Location = New Point(130, 133)
        txtSpecialite.MaxLength = 100
        txtSpecialite.Name = "txtSpecialite"
        txtSpecialite.ReadOnly = True
        txtSpecialite.Size = New Size(542, 25)
        txtSpecialite.TabIndex = 9
        ' 
        ' lblAdresse
        ' 
        lblAdresse.AutoSize = True
        lblAdresse.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblAdresse.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAdresse.Location = New Point(16, 167)
        lblAdresse.Name = "lblAdresse"
        lblAdresse.Size = New Size(54, 17)
        lblAdresse.TabIndex = 10
        lblAdresse.Text = "Adresse"
        ' 
        ' txtAdresse
        ' 
        txtAdresse.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdresse.Font = New Font("Calibri", 11F)
        txtAdresse.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdresse.Location = New Point(130, 164)
        txtAdresse.Name = "txtAdresse"
        txtAdresse.ReadOnly = True
        txtAdresse.Size = New Size(542, 25)
        txtAdresse.TabIndex = 11
        ' 
        ' grpPeriode
        ' 
        grpPeriode.Controls.Add(lblDateDebut)
        grpPeriode.Controls.Add(dtpDateDebut)
        grpPeriode.Controls.Add(lblDateFin)
        grpPeriode.Controls.Add(dtpDateFin)
        grpPeriode.Location = New Point(16, 310)
        grpPeriode.Name = "grpPeriode"
        grpPeriode.Padding = New Padding(12, 8, 12, 10)
        grpPeriode.Size = New Size(688, 76)
        grpPeriode.TabIndex = 2
        grpPeriode.TabStop = False
        grpPeriode.Titre = "Période de suivi"
        ' 
        ' lblDateDebut
        ' 
        lblDateDebut.AutoSize = True
        lblDateDebut.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDateDebut.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateDebut.Location = New Point(16, 43)
        lblDateDebut.Name = "lblDateDebut"
        lblDateDebut.Size = New Size(46, 17)
        lblDateDebut.TabIndex = 0
        lblDateDebut.Text = "Depuis"
        ' 
        ' dtpDateDebut
        ' 
        dtpDateDebut.Facultative = True
        dtpDateDebut.Font = New Font("Calibri", 11F)
        dtpDateDebut.Location = New Point(130, 40)
        dtpDateDebut.Name = "dtpDateDebut"
        dtpDateDebut.Size = New Size(150, 26)
        dtpDateDebut.TabIndex = 1
        ' 
        ' lblDateFin
        ' 
        lblDateFin.AutoSize = True
        lblDateFin.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDateFin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateFin.Location = New Point(400, 43)
        lblDateFin.Name = "lblDateFin"
        lblDateFin.Size = New Size(55, 17)
        lblDateFin.TabIndex = 2
        lblDateFin.Text = "Jusqu'au"
        ' 
        ' dtpDateFin
        ' 
        dtpDateFin.Facultative = True
        dtpDateFin.Font = New Font("Calibri", 11F)
        dtpDateFin.Location = New Point(522, 40)
        dtpDateFin.Name = "dtpDateFin"
        dtpDateFin.Size = New Size(150, 26)
        dtpDateFin.TabIndex = 3
        ' 
        ' grpCommentaire
        ' 
        grpCommentaire.Controls.Add(rteCommentaire)
        grpCommentaire.Location = New Point(16, 394)
        grpCommentaire.Name = "grpCommentaire"
        grpCommentaire.Padding = New Padding(12, 8, 12, 10)
        grpCommentaire.Size = New Size(688, 150)
        grpCommentaire.TabIndex = 3
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
        rteCommentaire.TitreDocument = "Commentaire de l'intervenant"
        rteCommentaire.Size = New Size(664, 95)
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
        pnlAction.Location = New Point(0, 552)
        pnlAction.Name = "pnlAction"
        pnlAction.Size = New Size(720, 62)
        pnlAction.TabIndex = 4
        ' 
        ' btnModifier
        ' 
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(225, 11)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(132, 40)
        btnModifier.TabIndex = 8
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        ' 
        ' btnFermer
        ' 
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.Location = New Point(367, 11)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(114, 40)
        btnFermer.TabIndex = 9
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(225, 11)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 40)
        btnEnregistrer.TabIndex = 6
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(367, 11)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 40)
        btnAnnuler.TabIndex = 7
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
        enTete.TabIndex = 7
        enTete.SousTitre = "Gérez les intervenants dans le suivi des patients"
        enTete.Text = "Intervenant"
        ' 
        ' IntervenantEdition
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(720, 614)
        Controls.Add(enTete)
        Controls.Add(pnlAction)
        Controls.Add(grpIntervenant)
        Controls.Add(grpPeriode)
        Controls.Add(grpCommentaire)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "IntervenantEdition"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Intervenant"
        grpIntervenant.ResumeLayout(False)
        grpIntervenant.PerformLayout()
        grpPeriode.ResumeLayout(False)
        grpPeriode.PerformLayout()
        grpCommentaire.ResumeLayout(False)
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        pnlAction.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

    Friend WithEvents lblCivilite As Label
    Friend WithEvents txtCivilite As TextBox
    Friend WithEvents grpIntervenant As AltheaCarte
    Friend WithEvents lblTherapeute As Label
    Friend WithEvents cboTherapeute As AltheaListe
    Friend WithEvents btnAjouterTherapeute As AltheaBouton
    Friend WithEvents lblRole As Label
    Friend WithEvents cboRole As AltheaListe
    Friend WithEvents btnAjouterRole As AltheaBouton
    Friend WithEvents lblNomProfessionnel As Label
    Friend WithEvents txtNomProfessionnel As TextBox
    Friend WithEvents lblSpecialite As Label
    Friend WithEvents txtSpecialite As TextBox
    Friend WithEvents lblAdresse As Label
    Friend WithEvents txtAdresse As TextBox
    Friend WithEvents grpPeriode As AltheaCarte
    Friend WithEvents lblDateDebut As Label
    Friend WithEvents dtpDateDebut As AltheaDate
    Friend WithEvents lblDateFin As Label
    Friend WithEvents dtpDateFin As AltheaDate
    Friend WithEvents grpCommentaire As AltheaCarte
    Friend WithEvents rteCommentaire As UC_RichTextEditor
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents lblTelephone As Label
    Friend WithEvents txtTelephone As TextBox
    Friend WithEvents lblEmail As Label
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents pnlAction As AltheaBarreActions
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents btnFermer As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents btnVoirTherapeutes As AltheaBouton

End Class
