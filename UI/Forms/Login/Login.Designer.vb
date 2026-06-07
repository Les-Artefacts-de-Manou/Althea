<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Login
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Login))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        tlpLogin = New TableLayoutPanel()
        lblUserName = New Label()
        txtUserName = New TextBox()
        lblPassword = New Label()
        txtPassword = New TextBox()
        btnVoirPassword = New Button()
        lblMessage = New Label()
        pnlActions = New Panel()
        btnAnnuler = New Button()
        btnConnexion = New Button()
        pnlTop = New Panel()
        lblTop = New Label()
        pnlTitre = New Panel()
        lblTitreForm = New Label()
        stsStatus = New StatusStrip()
        stsLabelStatus = New ToolStripStatusLabel()
        ttMain = New ToolTip(components)
        errProvider = New ErrorProvider(components)
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        tlpLogin.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTop.SuspendLayout()
        pnlTitre.SuspendLayout()
        stsStatus.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackgroundImage = CType(resources.GetObject("pnlForm.BackgroundImage"), Image)
        pnlForm.BackgroundImageLayout = ImageLayout.Stretch
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(pnlTop)
        pnlForm.Controls.Add(pnlTitre)
        pnlForm.Controls.Add(stsStatus)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Size = New Size(464, 361)
        pnlForm.TabIndex = 0
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(tlpLogin)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(0, 93)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(5, 6, 5, 6)
        pnlCenter.Size = New Size(464, 193)
        pnlCenter.TabIndex = 23
        ' 
        ' tlpLogin
        ' 
        tlpLogin.ColumnCount = 3
        tlpLogin.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30F))
        tlpLogin.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        tlpLogin.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20F))
        tlpLogin.Controls.Add(lblUserName, 0, 0)
        tlpLogin.Controls.Add(txtUserName, 1, 0)
        tlpLogin.Controls.Add(lblPassword, 0, 1)
        tlpLogin.Controls.Add(txtPassword, 1, 1)
        tlpLogin.Controls.Add(btnVoirPassword, 2, 1)
        tlpLogin.Controls.Add(lblMessage, 0, 2)
        tlpLogin.Dock = DockStyle.Fill
        tlpLogin.Location = New Point(5, 6)
        tlpLogin.Name = "tlpLogin"
        tlpLogin.Padding = New Padding(12)
        tlpLogin.RowCount = 3
        tlpLogin.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tlpLogin.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tlpLogin.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tlpLogin.Size = New Size(454, 181)
        tlpLogin.TabIndex = 0
        ' 
        ' lblUserName
        ' 
        lblUserName.AutoSize = True
        lblUserName.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblUserName.Location = New Point(15, 12)
        lblUserName.Name = "lblUserName"
        lblUserName.Size = New Size(65, 14)
        lblUserName.TabIndex = 0
        lblUserName.Text = "Utilisateur"
        ' 
        ' txtUserName
        ' 
        txtUserName.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtUserName.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtUserName.Location = New Point(144, 15)
        txtUserName.Name = "txtUserName"
        txtUserName.Size = New Size(209, 22)
        txtUserName.TabIndex = 1
        ' 
        ' lblPassword
        ' 
        lblPassword.AutoSize = True
        lblPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPassword.Location = New Point(15, 64)
        lblPassword.Name = "lblPassword"
        lblPassword.Size = New Size(81, 14)
        lblPassword.TabIndex = 2
        lblPassword.Text = "Mot de passe"
        ' 
        ' txtPassword
        ' 
        txtPassword.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtPassword.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPassword.Location = New Point(144, 67)
        txtPassword.Name = "txtPassword"
        txtPassword.Size = New Size(209, 22)
        txtPassword.TabIndex = 2
        txtPassword.UseSystemPasswordChar = True
        ' 
        ' btnVoirPassword
        ' 
        btnVoirPassword.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnVoirPassword.Image = CType(resources.GetObject("btnVoirPassword.Image"), Image)
        btnVoirPassword.Location = New Point(359, 67)
        btnVoirPassword.Name = "btnVoirPassword"
        btnVoirPassword.Size = New Size(38, 38)
        btnVoirPassword.TabIndex = 3
        btnVoirPassword.Tag = "voir_normal"
        btnVoirPassword.TextImageRelation = TextImageRelation.ImageAboveText
        btnVoirPassword.UseVisualStyleBackColor = False
        ' 
        ' lblMessage
        ' 
        tlpLogin.SetColumnSpan(lblMessage, 3)
        lblMessage.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblMessage.ForeColor = Color.FromArgb(CByte(194), CByte(106), CByte(118))
        lblMessage.Location = New Point(15, 116)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(424, 40)
        lblMessage.TabIndex = 15
        lblMessage.Text = "Message"
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.Transparent
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.Fixed3D
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnConnexion)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 286)
        pnlActions.Name = "pnlActions"
        pnlActions.Padding = New Padding(12, 7, 12, 7)
        pnlActions.Size = New Size(464, 50)
        pnlActions.TabIndex = 22
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAnnuler.FlatAppearance.BorderSize = 0
        btnAnnuler.FlatStyle = FlatStyle.Flat
        btnAnnuler.ForeColor = Color.White
        errProvider.SetIconAlignment(btnAnnuler, ErrorIconAlignment.TopLeft)
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.ImageAlign = ContentAlignment.MiddleLeft
        btnAnnuler.Location = New Point(233, 6)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(93, 37)
        btnAnnuler.TabIndex = 5
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAnnuler.UseVisualStyleBackColor = False
        ' 
        ' btnConnexion
        ' 
        btnConnexion.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnConnexion.BackgroundImageLayout = ImageLayout.Center
        btnConnexion.FlatAppearance.BorderSize = 0
        btnConnexion.FlatStyle = FlatStyle.Flat
        btnConnexion.ForeColor = Color.White
        errProvider.SetIconAlignment(btnConnexion, ErrorIconAlignment.MiddleLeft)
        btnConnexion.Image = CType(resources.GetObject("btnConnexion.Image"), Image)
        btnConnexion.ImageAlign = ContentAlignment.MiddleLeft
        btnConnexion.Location = New Point(135, 6)
        btnConnexion.Name = "btnConnexion"
        btnConnexion.Size = New Size(93, 37)
        btnConnexion.TabIndex = 4
        btnConnexion.Tag = "login_normal"
        btnConnexion.Text = "Login"
        btnConnexion.TextImageRelation = TextImageRelation.ImageBeforeText
        btnConnexion.UseVisualStyleBackColor = False
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.Transparent
        pnlTop.Controls.Add(lblTop)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(0, 54)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(7)
        pnlTop.Size = New Size(464, 39)
        pnlTop.TabIndex = 21
        ' 
        ' lblTop
        ' 
        lblTop.Dock = DockStyle.Fill
        lblTop.Font = New Font("Calibri", 10F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(7, 7)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(450, 25)
        lblTop.TabIndex = 0
        lblTop.Text = "Veuillez vous connecter :"
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(0, 0)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(464, 54)
        pnlTitre.TabIndex = 20
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.Anchor = AnchorStyles.Left
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 15F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(32, 9)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(7, 4, 7, 4)
        lblTitreForm.Size = New Size(174, 32)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Connexion Althéa"
        ' 
        ' stsStatus
        ' 
        stsStatus.AllowItemReorder = True
        stsStatus.AutoSize = False
        stsStatus.BackColor = Color.FromArgb(CByte(218), CByte(201), CByte(184))
        stsStatus.Items.AddRange(New ToolStripItem() {stsLabelStatus})
        stsStatus.Location = New Point(0, 336)
        stsStatus.Name = "stsStatus"
        stsStatus.Padding = New Padding(1, 0, 9, 0)
        stsStatus.Size = New Size(464, 25)
        stsStatus.TabIndex = 19
        stsStatus.Text = "StatusStrip1"
        ' 
        ' stsLabelStatus
        ' 
        stsLabelStatus.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        stsLabelStatus.Font = New Font("Calibri", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        stsLabelStatus.ForeColor = Color.FromArgb(CByte(11), CByte(95), CByte(125))
        stsLabelStatus.Name = "stsLabelStatus"
        stsLabelStatus.Size = New Size(43, 20)
        stsLabelStatus.Text = "Althéa"
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' Login
        ' 
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(464, 361)
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MinimumSize = New Size(480, 400)
        Name = "Login"
        StartPosition = FormStartPosition.CenterParent
        Text = "Login Althea"
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        tlpLogin.ResumeLayout(False)
        tlpLogin.PerformLayout()
        pnlActions.ResumeLayout(False)
        pnlTop.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        stsStatus.ResumeLayout(False)
        stsStatus.PerformLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents stsStatus As StatusStrip
    Friend WithEvents stsLabelStatus As ToolStripStatusLabel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents pnlTop As Panel
    Friend WithEvents lblTop As Label
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnConnexion As Button
    Friend WithEvents tlpLogin As TableLayoutPanel
    Friend WithEvents lblUserName As Label
    Friend WithEvents txtUserName As TextBox
    Friend WithEvents lblPassword As Label
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents btnVoirPassword As Button
    Friend WithEvents lblMessage As Label
    Friend WithEvents btnAnnuler As Button
End Class
