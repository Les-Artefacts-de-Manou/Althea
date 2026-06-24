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
        lblTitreForm = New Label()
        grpIntervenant = New GroupBox()
        btnVoirTherapeutes = New Button()
        txtTelephone = New TextBox()
        lblTelephone = New Label()
        lblTherapeute = New Label()
        cboTherapeute = New ComboBox()
        btnAjouterTherapeute = New Button()
        lblRole = New Label()
        cboRole = New ComboBox()
        btnAjouterRole = New Button()
        lblNomProfessionnel = New Label()
        txtNomProfessionnel = New TextBox()
        lblSpecialite = New Label()
        txtSpecialite = New TextBox()
        lblLieu = New Label()
        txtLieu = New TextBox()
        grpPeriode = New GroupBox()
        lblDateDebut = New Label()
        dtpDateDebut = New DateTimePicker()
        lblDateFin = New Label()
        dtpDateFin = New DateTimePicker()
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
        grpIntervenant.SuspendLayout()
        grpPeriode.SuspendLayout()
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
        lblTitreForm.Location = New Point(61, 18)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Size = New Size(104, 23)
        lblTitreForm.TabIndex = 0
        lblTitreForm.Text = "Intervenant"
        ' 
        ' grpIntervenant
        ' 
        grpIntervenant.BackColor = Color.Transparent
        grpIntervenant.Controls.Add(btnVoirTherapeutes)
        grpIntervenant.Controls.Add(txtTelephone)
        grpIntervenant.Controls.Add(lblTelephone)
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
        grpIntervenant.Controls.Add(lblLieu)
        grpIntervenant.Controls.Add(txtLieu)
        grpIntervenant.FlatStyle = FlatStyle.Flat
        grpIntervenant.Font = New Font("Calibri", 10F)
        grpIntervenant.ForeColor = Color.White
        grpIntervenant.Location = New Point(16, 64)
        grpIntervenant.Name = "grpIntervenant"
        grpIntervenant.Size = New Size(688, 236)
        grpIntervenant.TabIndex = 1
        grpIntervenant.TabStop = False
        grpIntervenant.Text = "Intervenant"
        ' 
        ' btnVoirTherapeutes
        ' 
        btnVoirTherapeutes.FlatAppearance.BorderColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        btnVoirTherapeutes.FlatAppearance.BorderSize = 0
        btnVoirTherapeutes.FlatStyle = FlatStyle.Flat
        btnVoirTherapeutes.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnVoirTherapeutes.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnVoirTherapeutes.Image = CType(resources.GetObject("btnVoirTherapeutes.Image"), Image)
        btnVoirTherapeutes.Location = New Point(397, 30)
        btnVoirTherapeutes.Name = "btnVoirTherapeutes"
        btnVoirTherapeutes.Size = New Size(27, 27)
        btnVoirTherapeutes.TabIndex = 14
        btnVoirTherapeutes.Tag = "recherche_24_normal"
        btnVoirTherapeutes.Text = "="
        btnVoirTherapeutes.UseVisualStyleBackColor = False
        ' 
        ' txtTelephone
        ' 
        txtTelephone.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtTelephone.Font = New Font("Calibri", 10F)
        txtTelephone.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtTelephone.Location = New Point(130, 198)
        txtTelephone.MaxLength = 150
        txtTelephone.Name = "txtTelephone"
        txtTelephone.ReadOnly = True
        txtTelephone.Size = New Size(239, 24)
        txtTelephone.TabIndex = 13
        ' 
        ' lblTelephone
        ' 
        lblTelephone.AutoSize = True
        lblTelephone.Font = New Font("Calibri", 10F)
        lblTelephone.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTelephone.Location = New Point(18, 201)
        lblTelephone.Name = "lblTelephone"
        lblTelephone.Size = New Size(66, 17)
        lblTelephone.TabIndex = 12
        lblTelephone.Text = "Téléphone"
        ' 
        ' lblTherapeute
        ' 
        lblTherapeute.AutoSize = True
        lblTherapeute.Font = New Font("Calibri", 10F)
        lblTherapeute.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTherapeute.Location = New Point(16, 33)
        lblTherapeute.Name = "lblTherapeute"
        lblTherapeute.Size = New Size(74, 17)
        lblTherapeute.TabIndex = 0
        lblTherapeute.Text = "Thérapeute"
        ' 
        ' cboTherapeute
        ' 
        cboTherapeute.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboTherapeute.DropDownStyle = ComboBoxStyle.DropDownList
        cboTherapeute.Font = New Font("Calibri", 10F)
        cboTherapeute.Location = New Point(130, 30)
        cboTherapeute.Name = "cboTherapeute"
        cboTherapeute.Size = New Size(230, 23)
        cboTherapeute.TabIndex = 1
        ' 
        ' btnAjouterTherapeute
        ' 
        btnAjouterTherapeute.FlatAppearance.BorderColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        btnAjouterTherapeute.FlatAppearance.BorderSize = 0
        btnAjouterTherapeute.FlatStyle = FlatStyle.Flat
        btnAjouterTherapeute.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnAjouterTherapeute.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnAjouterTherapeute.Image = CType(resources.GetObject("btnAjouterTherapeute.Image"), Image)
        btnAjouterTherapeute.Location = New Point(364, 29)
        btnAjouterTherapeute.Name = "btnAjouterTherapeute"
        btnAjouterTherapeute.Size = New Size(27, 27)
        btnAjouterTherapeute.TabIndex = 2
        btnAjouterTherapeute.Tag = "plus_24_normal.png"
        btnAjouterTherapeute.Text = "+"
        btnAjouterTherapeute.UseVisualStyleBackColor = True
        ' 
        ' lblRole
        ' 
        lblRole.AutoSize = True
        lblRole.Font = New Font("Calibri", 10F)
        lblRole.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRole.Location = New Point(16, 66)
        lblRole.Name = "lblRole"
        lblRole.Size = New Size(33, 17)
        lblRole.TabIndex = 3
        lblRole.Text = "Rôle"
        ' 
        ' cboRole
        ' 
        cboRole.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList
        cboRole.Font = New Font("Calibri", 10F)
        cboRole.Location = New Point(130, 63)
        cboRole.Name = "cboRole"
        cboRole.Size = New Size(230, 23)
        cboRole.TabIndex = 4
        ' 
        ' btnAjouterRole
        ' 
        btnAjouterRole.FlatAppearance.BorderColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        btnAjouterRole.FlatStyle = FlatStyle.Flat
        btnAjouterRole.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnAjouterRole.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnAjouterRole.Image = CType(resources.GetObject("btnAjouterRole.Image"), Image)
        btnAjouterRole.Location = New Point(364, 62)
        btnAjouterRole.Name = "btnAjouterRole"
        btnAjouterRole.Size = New Size(27, 27)
        btnAjouterRole.TabIndex = 5
        btnAjouterRole.Tag = "plus_24_normal.png"
        btnAjouterRole.Text = "+"
        btnAjouterRole.UseVisualStyleBackColor = True
        ' 
        ' lblNomProfessionnel
        ' 
        lblNomProfessionnel.AutoSize = True
        lblNomProfessionnel.Font = New Font("Calibri", 10F)
        lblNomProfessionnel.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNomProfessionnel.Location = New Point(16, 100)
        lblNomProfessionnel.Name = "lblNomProfessionnel"
        lblNomProfessionnel.Size = New Size(96, 17)
        lblNomProfessionnel.TabIndex = 6
        lblNomProfessionnel.Text = "Nom / praticien"
        ' 
        ' txtNomProfessionnel
        ' 
        txtNomProfessionnel.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNomProfessionnel.Font = New Font("Calibri", 10F)
        txtNomProfessionnel.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNomProfessionnel.Location = New Point(130, 97)
        txtNomProfessionnel.MaxLength = 150
        txtNomProfessionnel.Name = "txtNomProfessionnel"
        txtNomProfessionnel.ReadOnly = True
        txtNomProfessionnel.Size = New Size(542, 24)
        txtNomProfessionnel.TabIndex = 7
        ' 
        ' lblSpecialite
        ' 
        lblSpecialite.AutoSize = True
        lblSpecialite.Font = New Font("Calibri", 10F)
        lblSpecialite.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblSpecialite.Location = New Point(16, 133)
        lblSpecialite.Name = "lblSpecialite"
        lblSpecialite.Size = New Size(62, 17)
        lblSpecialite.TabIndex = 8
        lblSpecialite.Text = "Spécialité"
        ' 
        ' txtSpecialite
        ' 
        txtSpecialite.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtSpecialite.Font = New Font("Calibri", 10F)
        txtSpecialite.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtSpecialite.Location = New Point(130, 130)
        txtSpecialite.MaxLength = 100
        txtSpecialite.Name = "txtSpecialite"
        txtSpecialite.ReadOnly = True
        txtSpecialite.Size = New Size(542, 24)
        txtSpecialite.TabIndex = 9
        ' 
        ' lblLieu
        ' 
        lblLieu.AutoSize = True
        lblLieu.Font = New Font("Calibri", 10F)
        lblLieu.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLieu.Location = New Point(18, 166)
        lblLieu.Name = "lblLieu"
        lblLieu.Size = New Size(31, 17)
        lblLieu.TabIndex = 10
        lblLieu.Text = "Lieu"
        ' 
        ' txtLieu
        ' 
        txtLieu.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtLieu.Font = New Font("Calibri", 10F)
        txtLieu.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLieu.Location = New Point(130, 163)
        txtLieu.MaxLength = 150
        txtLieu.Name = "txtLieu"
        txtLieu.ReadOnly = True
        txtLieu.Size = New Size(542, 24)
        txtLieu.TabIndex = 11
        ' 
        ' grpPeriode
        ' 
        grpPeriode.Controls.Add(lblDateDebut)
        grpPeriode.Controls.Add(dtpDateDebut)
        grpPeriode.Controls.Add(lblDateFin)
        grpPeriode.Controls.Add(dtpDateFin)
        grpPeriode.Font = New Font("Calibri", 10F, FontStyle.Bold)
        grpPeriode.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpPeriode.Location = New Point(16, 306)
        grpPeriode.Name = "grpPeriode"
        grpPeriode.Size = New Size(688, 58)
        grpPeriode.TabIndex = 2
        grpPeriode.TabStop = False
        grpPeriode.Text = "Période de suivi"
        ' 
        ' lblDateDebut
        ' 
        lblDateDebut.AutoSize = True
        lblDateDebut.Font = New Font("Calibri", 10F)
        lblDateDebut.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateDebut.Location = New Point(16, 25)
        lblDateDebut.Name = "lblDateDebut"
        lblDateDebut.Size = New Size(46, 17)
        lblDateDebut.TabIndex = 0
        lblDateDebut.Text = "Depuis"
        ' 
        ' dtpDateDebut
        ' 
        dtpDateDebut.CalendarForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        dtpDateDebut.CalendarMonthBackground = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dtpDateDebut.CalendarTitleForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        dtpDateDebut.Font = New Font("Calibri", 10F)
        dtpDateDebut.Format = DateTimePickerFormat.Short
        dtpDateDebut.Location = New Point(130, 22)
        dtpDateDebut.Name = "dtpDateDebut"
        dtpDateDebut.ShowCheckBox = True
        dtpDateDebut.Size = New Size(150, 24)
        dtpDateDebut.TabIndex = 1
        ' 
        ' lblDateFin
        ' 
        lblDateFin.AutoSize = True
        lblDateFin.Font = New Font("Calibri", 10F)
        lblDateFin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateFin.Location = New Point(360, 25)
        lblDateFin.Name = "lblDateFin"
        lblDateFin.Size = New Size(55, 17)
        lblDateFin.TabIndex = 2
        lblDateFin.Text = "Jusqu'au"
        ' 
        ' dtpDateFin
        ' 
        dtpDateFin.CalendarForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        dtpDateFin.CalendarMonthBackground = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dtpDateFin.CalendarTitleForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        dtpDateFin.Font = New Font("Calibri", 10F)
        dtpDateFin.Format = DateTimePickerFormat.Short
        dtpDateFin.Location = New Point(522, 22)
        dtpDateFin.Name = "dtpDateFin"
        dtpDateFin.ShowCheckBox = True
        dtpDateFin.Size = New Size(150, 24)
        dtpDateFin.TabIndex = 3
        ' 
        ' grpCommentaire
        ' 
        grpCommentaire.Controls.Add(rteCommentaire)
        grpCommentaire.Font = New Font("Calibri", 10F, FontStyle.Bold)
        grpCommentaire.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        grpCommentaire.Location = New Point(16, 371)
        grpCommentaire.Name = "grpCommentaire"
        grpCommentaire.Size = New Size(688, 156)
        grpCommentaire.TabIndex = 3
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
        pnlAction.Location = New Point(0, 533)
        pnlAction.Name = "pnlAction"
        pnlAction.Size = New Size(720, 62)
        pnlAction.TabIndex = 4
        ' 
        ' btnModifier
        ' 
        btnModifier.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnModifier.FlatAppearance.BorderSize = 0
        btnModifier.FlatStyle = FlatStyle.Flat
        btnModifier.Font = New Font("Calibri", 10F)
        btnModifier.ForeColor = Color.White
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.ImageAlign = ContentAlignment.MiddleLeft
        btnModifier.Location = New Point(240, 14)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(117, 40)
        btnModifier.TabIndex = 8
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        btnModifier.TextAlign = ContentAlignment.MiddleLeft
        btnModifier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifier.UseVisualStyleBackColor = False
        ' 
        ' btnFermer
        ' 
        btnFermer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnFermer.FlatAppearance.BorderSize = 0
        btnFermer.FlatStyle = FlatStyle.Flat
        btnFermer.Font = New Font("Calibri", 10F)
        btnFermer.ForeColor = Color.White
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.ImageAlign = ContentAlignment.MiddleLeft
        btnFermer.Location = New Point(369, 14)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(112, 40)
        btnFermer.TabIndex = 9
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.TextAlign = ContentAlignment.MiddleLeft
        btnFermer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnFermer.UseVisualStyleBackColor = False
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
        btnEnregistrer.Location = New Point(240, 14)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(117, 40)
        btnEnregistrer.TabIndex = 6
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.TextAlign = ContentAlignment.MiddleLeft
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
        btnAnnuler.Location = New Point(369, 14)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(112, 40)
        btnAnnuler.TabIndex = 7
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
        pnlTitre.TabIndex = 7
        ' 
        ' lblTop
        ' 
        lblTop.Font = New Font("Calibri", 12F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(349, 18)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(352, 26)
        lblTop.TabIndex = 4
        lblTop.Text = "Gestion des intervenants dans le suivi des patients"
        ' 
        ' IntervenantEdition
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        BackgroundImage = My.Resources.Resources.Fond_1000x800_Feuille4
        BackgroundImageLayout = ImageLayout.Stretch
        ClientSize = New Size(720, 595)
        Controls.Add(pnlTitre)
        Controls.Add(pnlAction)
        Controls.Add(grpIntervenant)
        Controls.Add(grpPeriode)
        Controls.Add(grpCommentaire)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        FormBorderStyle = FormBorderStyle.FixedDialog
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
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        ResumeLayout(False)

    End Sub

    Friend WithEvents lblTitreForm As Label
    Friend WithEvents grpIntervenant As GroupBox
    Friend WithEvents lblTherapeute As Label
    Friend WithEvents cboTherapeute As ComboBox
    Friend WithEvents btnAjouterTherapeute As Button
    Friend WithEvents lblRole As Label
    Friend WithEvents cboRole As ComboBox
    Friend WithEvents btnAjouterRole As Button
    Friend WithEvents lblNomProfessionnel As Label
    Friend WithEvents txtNomProfessionnel As TextBox
    Friend WithEvents lblSpecialite As Label
    Friend WithEvents txtSpecialite As TextBox
    Friend WithEvents lblLieu As Label
    Friend WithEvents txtLieu As TextBox
    Friend WithEvents grpPeriode As GroupBox
    Friend WithEvents lblDateDebut As Label
    Friend WithEvents dtpDateDebut As DateTimePicker
    Friend WithEvents lblDateFin As Label
    Friend WithEvents dtpDateFin As DateTimePicker
    Friend WithEvents grpCommentaire As GroupBox
    Friend WithEvents rteCommentaire As UC_RichTextEditorSimple
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents lblTelephone As Label
    Friend WithEvents txtTelephone As TextBox
    Friend WithEvents pnlAction As Panel
    Friend WithEvents btnEnregistrer As Button
    Friend WithEvents btnAnnuler As Button
    Friend WithEvents btnModifier As Button
    Friend WithEvents btnFermer As Button
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTop As Label
    Friend WithEvents btnVoirTherapeutes As Button

End Class
