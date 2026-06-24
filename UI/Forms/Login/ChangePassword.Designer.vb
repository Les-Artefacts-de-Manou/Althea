<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ChangePassword
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ChangePassword))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        tlpChangePW = New TableLayoutPanel()
        lblPasswordRules = New Label()
        btnVoirConfirmation = New Button()
        lblNewPassword = New Label()
        btnVoirOldPassword = New Button()
        txtOldPassword = New TextBox()
        lblOldPassword = New Label()
        txtNewPassword = New TextBox()
        btnVoirNewPassword = New Button()
        lblMessage = New Label()
        txtConfirmation = New TextBox()
        lblConfirmation = New Label()
        pnlActions = New Panel()
        btnAnnuler = New Button()
        btnValider = New Button()
        stsStatus = New StatusStrip()
        stsLabelStatus = New ToolStripStatusLabel()
        pnlTitre = New Panel()
        lblTitreForm = New Label()
        errProvider = New ErrorProvider(components)
        ttMain = New ToolTip(components)
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        tlpChangePW.SuspendLayout()
        pnlActions.SuspendLayout()
        stsStatus.SuspendLayout()
        pnlTitre.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackgroundImage = My.Resources.Resources.Fond_1000x800_Feuille4
        pnlForm.BackgroundImageLayout = ImageLayout.Stretch
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(stsStatus)
        pnlForm.Controls.Add(pnlTitre)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Font = New Font("Calibri", 9F)
        pnlForm.Location = New Point(0, 0)
        pnlForm.Margin = New Padding(2, 3, 2, 3)
        pnlForm.Name = "pnlForm"
        pnlForm.Size = New Size(464, 361)
        pnlForm.TabIndex = 0
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(tlpChangePW)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(0, 50)
        pnlCenter.Margin = New Padding(2, 3, 2, 3)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(4, 6, 4, 6)
        pnlCenter.Size = New Size(464, 241)
        pnlCenter.TabIndex = 24
        ' 
        ' tlpChangePW
        ' 
        tlpChangePW.ColumnCount = 3
        tlpChangePW.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30F))
        tlpChangePW.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        tlpChangePW.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20F))
        tlpChangePW.Controls.Add(lblPasswordRules, 0, 2)
        tlpChangePW.Controls.Add(btnVoirConfirmation, 2, 3)
        tlpChangePW.Controls.Add(lblNewPassword, 0, 1)
        tlpChangePW.Controls.Add(btnVoirOldPassword, 2, 0)
        tlpChangePW.Controls.Add(txtOldPassword, 1, 0)
        tlpChangePW.Controls.Add(lblOldPassword, 0, 0)
        tlpChangePW.Controls.Add(txtNewPassword, 1, 1)
        tlpChangePW.Controls.Add(btnVoirNewPassword, 2, 1)
        tlpChangePW.Controls.Add(lblMessage, 0, 4)
        tlpChangePW.Controls.Add(txtConfirmation, 1, 3)
        tlpChangePW.Controls.Add(lblConfirmation, 0, 3)
        tlpChangePW.Dock = DockStyle.Fill
        tlpChangePW.Location = New Point(4, 6)
        tlpChangePW.Margin = New Padding(2, 3, 2, 3)
        tlpChangePW.Name = "tlpChangePW"
        tlpChangePW.Padding = New Padding(10, 11, 10, 11)
        tlpChangePW.RowCount = 5
        tlpChangePW.RowStyles.Add(New RowStyle(SizeType.Percent, 22.2222214F))
        tlpChangePW.RowStyles.Add(New RowStyle(SizeType.Percent, 22.2222214F))
        tlpChangePW.RowStyles.Add(New RowStyle(SizeType.Percent, 10.10101F))
        tlpChangePW.RowStyles.Add(New RowStyle(SizeType.Percent, 22.2222214F))
        tlpChangePW.RowStyles.Add(New RowStyle(SizeType.Percent, 23.2323227F))
        tlpChangePW.RowStyles.Add(New RowStyle(SizeType.Absolute, 19F))
        tlpChangePW.Size = New Size(456, 229)
        tlpChangePW.TabIndex = 0
        ' 
        ' lblPasswordRules
        ' 
        lblPasswordRules.AutoSize = True
        tlpChangePW.SetColumnSpan(lblPasswordRules, 3)
        lblPasswordRules.Dock = DockStyle.Fill
        lblPasswordRules.FlatStyle = FlatStyle.Flat
        lblPasswordRules.Font = New Font("Calibri", 8F)
        lblPasswordRules.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPasswordRules.Location = New Point(12, 103)
        lblPasswordRules.Margin = New Padding(2, 0, 2, 0)
        lblPasswordRules.Name = "lblPasswordRules"
        lblPasswordRules.Size = New Size(432, 20)
        lblPasswordRules.TabIndex = 25
        lblPasswordRules.Text = "Minimum 10 caractères, avec au moins 1 majuscule, 1 minuscule et 1 chiffre."
        lblPasswordRules.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' btnVoirConfirmation
        ' 
        btnVoirConfirmation.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnVoirConfirmation.Image = CType(resources.GetObject("btnVoirConfirmation.Image"), Image)
        btnVoirConfirmation.Location = New Point(360, 123)
        btnVoirConfirmation.Margin = New Padding(2, 0, 2, 3)
        btnVoirConfirmation.Name = "btnVoirConfirmation"
        btnVoirConfirmation.Size = New Size(32, 35)
        btnVoirConfirmation.TabIndex = 6
        btnVoirConfirmation.Tag = "voir_normal"
        btnVoirConfirmation.TextImageRelation = TextImageRelation.ImageAboveText
        btnVoirConfirmation.UseVisualStyleBackColor = False
        ' 
        ' lblNewPassword
        ' 
        lblNewPassword.AutoSize = True
        lblNewPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNewPassword.Location = New Point(12, 68)
        lblNewPassword.Margin = New Padding(2, 11, 2, 0)
        lblNewPassword.Name = "lblNewPassword"
        lblNewPassword.Size = New Size(99, 28)
        lblNewPassword.TabIndex = 20
        lblNewPassword.Text = "Nouveau mot de passe"
        ' 
        ' btnVoirOldPassword
        ' 
        btnVoirOldPassword.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnVoirOldPassword.Image = CType(resources.GetObject("btnVoirOldPassword.Image"), Image)
        btnVoirOldPassword.Location = New Point(360, 11)
        btnVoirOldPassword.Margin = New Padding(2, 0, 2, 3)
        btnVoirOldPassword.Name = "btnVoirOldPassword"
        btnVoirOldPassword.Size = New Size(32, 35)
        btnVoirOldPassword.TabIndex = 2
        btnVoirOldPassword.Tag = "voir_normal"
        btnVoirOldPassword.TextImageRelation = TextImageRelation.ImageAboveText
        btnVoirOldPassword.UseVisualStyleBackColor = False
        ' 
        ' txtOldPassword
        ' 
        txtOldPassword.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtOldPassword.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtOldPassword.Location = New Point(142, 22)
        txtOldPassword.Margin = New Padding(2, 11, 2, 3)
        txtOldPassword.Name = "txtOldPassword"
        txtOldPassword.Size = New Size(213, 22)
        txtOldPassword.TabIndex = 1
        txtOldPassword.UseSystemPasswordChar = True
        ' 
        ' lblOldPassword
        ' 
        lblOldPassword.AutoSize = True
        lblOldPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblOldPassword.Location = New Point(12, 22)
        lblOldPassword.Margin = New Padding(2, 11, 2, 0)
        lblOldPassword.Name = "lblOldPassword"
        lblOldPassword.Size = New Size(121, 14)
        lblOldPassword.TabIndex = 17
        lblOldPassword.Text = "Ancien mot de passe"
        ' 
        ' txtNewPassword
        ' 
        txtNewPassword.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtNewPassword.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNewPassword.Location = New Point(142, 68)
        txtNewPassword.Margin = New Padding(2, 11, 2, 3)
        txtNewPassword.Name = "txtNewPassword"
        txtNewPassword.Size = New Size(213, 22)
        txtNewPassword.TabIndex = 3
        txtNewPassword.UseSystemPasswordChar = True
        ' 
        ' btnVoirNewPassword
        ' 
        btnVoirNewPassword.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnVoirNewPassword.Image = CType(resources.GetObject("btnVoirNewPassword.Image"), Image)
        btnVoirNewPassword.Location = New Point(360, 57)
        btnVoirNewPassword.Margin = New Padding(2, 0, 2, 3)
        btnVoirNewPassword.Name = "btnVoirNewPassword"
        btnVoirNewPassword.Size = New Size(32, 35)
        btnVoirNewPassword.TabIndex = 4
        btnVoirNewPassword.Tag = "voir_normal"
        btnVoirNewPassword.TextImageRelation = TextImageRelation.ImageAboveText
        btnVoirNewPassword.UseVisualStyleBackColor = False
        ' 
        ' lblMessage
        ' 
        tlpChangePW.SetColumnSpan(lblMessage, 3)
        lblMessage.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblMessage.ForeColor = Color.FromArgb(CByte(194), CByte(106), CByte(118))
        lblMessage.Location = New Point(12, 169)
        lblMessage.Margin = New Padding(2, 0, 2, 0)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(432, 37)
        lblMessage.TabIndex = 16
        lblMessage.Text = "Message"
        ' 
        ' txtConfirmation
        ' 
        txtConfirmation.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtConfirmation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtConfirmation.Location = New Point(142, 134)
        txtConfirmation.Margin = New Padding(2, 11, 2, 3)
        txtConfirmation.Name = "txtConfirmation"
        txtConfirmation.Size = New Size(213, 22)
        txtConfirmation.TabIndex = 5
        txtConfirmation.UseSystemPasswordChar = True
        ' 
        ' lblConfirmation
        ' 
        lblConfirmation.AutoSize = True
        lblConfirmation.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblConfirmation.Location = New Point(12, 134)
        lblConfirmation.Margin = New Padding(2, 11, 2, 0)
        lblConfirmation.Name = "lblConfirmation"
        lblConfirmation.Size = New Size(75, 14)
        lblConfirmation.TabIndex = 22
        lblConfirmation.Text = "Confirmation"
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.FromArgb(CByte(231), CByte(223), CByte(214))
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.Fixed3D
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnValider)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 291)
        pnlActions.Margin = New Padding(2, 3, 2, 3)
        pnlActions.Name = "pnlActions"
        pnlActions.Padding = New Padding(6, 7, 6, 7)
        pnlActions.Size = New Size(464, 47)
        pnlActions.TabIndex = 23
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAnnuler.FlatAppearance.BorderSize = 0
        btnAnnuler.FlatStyle = FlatStyle.Flat
        btnAnnuler.Font = New Font("Calibri", 9F, FontStyle.Bold)
        btnAnnuler.ForeColor = Color.White
        errProvider.SetIconAlignment(btnAnnuler, ErrorIconAlignment.TopLeft)
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.ImageAlign = ContentAlignment.MiddleLeft
        btnAnnuler.Location = New Point(235, 5)
        btnAnnuler.Margin = New Padding(2, 3, 2, 3)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(97, 36)
        btnAnnuler.TabIndex = 8
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAnnuler.UseVisualStyleBackColor = False
        ' 
        ' btnValider
        ' 
        btnValider.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnValider.BackgroundImageLayout = ImageLayout.Center
        btnValider.FlatAppearance.BorderSize = 0
        btnValider.FlatStyle = FlatStyle.Flat
        btnValider.Font = New Font("Calibri", 9F, FontStyle.Bold)
        btnValider.ForeColor = Color.White
        errProvider.SetIconAlignment(btnValider, ErrorIconAlignment.MiddleLeft)
        btnValider.Image = CType(resources.GetObject("btnValider.Image"), Image)
        btnValider.ImageAlign = ContentAlignment.MiddleLeft
        btnValider.Location = New Point(134, 6)
        btnValider.Margin = New Padding(2, 3, 2, 3)
        btnValider.Name = "btnValider"
        btnValider.Size = New Size(97, 36)
        btnValider.TabIndex = 7
        btnValider.Tag = "valider_normal"
        btnValider.Text = "Valider"
        btnValider.TextImageRelation = TextImageRelation.ImageBeforeText
        btnValider.UseVisualStyleBackColor = False
        ' 
        ' stsStatus
        ' 
        stsStatus.AllowItemReorder = True
        stsStatus.AutoSize = False
        stsStatus.BackColor = Color.FromArgb(CByte(218), CByte(201), CByte(184))
        stsStatus.Items.AddRange(New ToolStripItem() {stsLabelStatus})
        stsStatus.Location = New Point(0, 338)
        stsStatus.Name = "stsStatus"
        stsStatus.Padding = New Padding(1, 0, 8, 0)
        stsStatus.Size = New Size(464, 23)
        stsStatus.TabIndex = 22
        stsStatus.Text = "StatusStrip1"
        ' 
        ' stsLabelStatus
        ' 
        stsLabelStatus.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        stsLabelStatus.Font = New Font("Calibri", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        stsLabelStatus.ForeColor = Color.FromArgb(CByte(11), CByte(95), CByte(125))
        stsLabelStatus.Name = "stsLabelStatus"
        stsLabelStatus.Size = New Size(43, 18)
        stsLabelStatus.Text = "Althéa"
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(0, 0)
        pnlTitre.Margin = New Padding(2, 3, 2, 3)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(464, 50)
        pnlTitre.TabIndex = 21
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.Anchor = AnchorStyles.Left
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 15F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(30, 9)
        lblTitreForm.Margin = New Padding(2, 0, 2, 0)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(6, 4, 6, 4)
        lblTitreForm.Size = New Size(271, 32)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Changement de mot de passe"
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' ChangePassword
        ' 
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(464, 361)
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 9F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(2, 3, 2, 3)
        MinimumSize = New Size(480, 400)
        Name = "ChangePassword"
        StartPosition = FormStartPosition.CenterParent
        Text = "Changement de Mot de passe"
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        tlpChangePW.ResumeLayout(False)
        tlpChangePW.PerformLayout()
        pnlActions.ResumeLayout(False)
        stsStatus.ResumeLayout(False)
        stsStatus.PerformLayout()
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents stsStatus As StatusStrip
    Friend WithEvents stsLabelStatus As ToolStripStatusLabel
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnAnnuler As Button
    Friend WithEvents btnValider As Button
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents tlpChangePW As TableLayoutPanel
    Friend WithEvents lblPassword As Label
    Friend WithEvents txtNewPassword As TextBox
    Friend WithEvents btnVoirNewPassword As Button
    Friend WithEvents btnVoirOldPassword As Button
    Friend WithEvents txtOldPassword As TextBox
    Friend WithEvents lblOldPassword As Label
    Friend WithEvents lblMessage As Label
    Friend WithEvents lblNewPassword As Label
    Friend WithEvents txtConfirmation As TextBox
    Friend WithEvents lblConfirmation As Label
    Friend WithEvents btnVoirConfirmation As Button
    Friend WithEvents lblPasswordRules As Label
End Class
