<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ConfigurationConnexion
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ConfigurationConnexion))
        ttMain = New ToolTip(components)
        errProvider = New ErrorProvider(components)
        btnTesterConnexion = New Button()
        btnEnregistrerConnexion = New Button()
        btnFermer = New Button()
        btnModifierPassword = New Button()
        pnlForm = New Panel()
        pnlCenter = New Panel()
        tlpConnexion = New TableLayoutPanel()
        txtHost = New TextBox()
        lblHost = New Label()
        lblPort = New Label()
        nudPort = New NumericUpDown()
        lblDatabaseName = New Label()
        txtDatabaseName = New TextBox()
        lblUserName = New Label()
        txtUserName = New TextBox()
        lblPassword = New Label()
        txtPassword = New TextBox()
        lblAdditionalOptions = New Label()
        txtAdditionalOptions = New TextBox()
        btnVoirPassword = New Button()
        Panel1 = New Panel()
        lblConnectionResult = New Label()
        pnlActions = New Panel()
        pnlTop = New Panel()
        lblTop = New Label()
        pnlTitre = New Panel()
        lblTitreForm = New Label()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        tlpConnexion.SuspendLayout()
        CType(nudPort, ComponentModel.ISupportInitialize).BeginInit()
        Panel1.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTop.SuspendLayout()
        pnlTitre.SuspendLayout()
        SuspendLayout()
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' btnTesterConnexion
        ' 
        btnTesterConnexion.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnTesterConnexion.BackgroundImageLayout = ImageLayout.Center
        btnTesterConnexion.FlatAppearance.BorderSize = 0
        btnTesterConnexion.FlatStyle = FlatStyle.Flat
        btnTesterConnexion.ForeColor = Color.White
        errProvider.SetIconAlignment(btnTesterConnexion, ErrorIconAlignment.TopLeft)
        btnTesterConnexion.Image = CType(resources.GetObject("btnTesterConnexion.Image"), Image)
        btnTesterConnexion.ImageAlign = ContentAlignment.MiddleLeft
        btnTesterConnexion.Location = New Point(174, 7)
        btnTesterConnexion.Name = "btnTesterConnexion"
        btnTesterConnexion.Size = New Size(145, 40)
        btnTesterConnexion.TabIndex = 8
        btnTesterConnexion.Tag = "testerConnexion_normal"
        btnTesterConnexion.Text = "tester Connexion"
        btnTesterConnexion.TextAlign = ContentAlignment.MiddleLeft
        btnTesterConnexion.TextImageRelation = TextImageRelation.ImageBeforeText
        btnTesterConnexion.UseVisualStyleBackColor = False
        ' 
        ' btnEnregistrerConnexion
        ' 
        btnEnregistrerConnexion.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnEnregistrerConnexion.FlatAppearance.BorderSize = 0
        btnEnregistrerConnexion.FlatStyle = FlatStyle.Flat
        btnEnregistrerConnexion.ForeColor = Color.White
        errProvider.SetIconAlignment(btnEnregistrerConnexion, ErrorIconAlignment.TopLeft)
        btnEnregistrerConnexion.Image = CType(resources.GetObject("btnEnregistrerConnexion.Image"), Image)
        btnEnregistrerConnexion.ImageAlign = ContentAlignment.TopLeft
        btnEnregistrerConnexion.Location = New Point(325, 7)
        btnEnregistrerConnexion.Name = "btnEnregistrerConnexion"
        btnEnregistrerConnexion.Size = New Size(129, 40)
        btnEnregistrerConnexion.TabIndex = 9
        btnEnregistrerConnexion.Tag = "enregistrer_normal"
        btnEnregistrerConnexion.Text = "Enregistrer"
        btnEnregistrerConnexion.TextAlign = ContentAlignment.MiddleLeft
        btnEnregistrerConnexion.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEnregistrerConnexion.UseVisualStyleBackColor = False
        ' 
        ' btnFermer
        ' 
        btnFermer.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnFermer.FlatAppearance.BorderSize = 0
        btnFermer.FlatStyle = FlatStyle.Flat
        btnFermer.ForeColor = Color.White
        errProvider.SetIconAlignment(btnFermer, ErrorIconAlignment.TopLeft)
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.ImageAlign = ContentAlignment.MiddleLeft
        btnFermer.Location = New Point(459, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(108, 40)
        btnFermer.TabIndex = 10
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.TextAlign = ContentAlignment.MiddleLeft
        btnFermer.TextImageRelation = TextImageRelation.ImageBeforeText
        btnFermer.UseVisualStyleBackColor = False
        ' 
        ' btnModifierPassword
        ' 
        btnModifierPassword.Anchor = AnchorStyles.Top
        btnModifierPassword.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnModifierPassword.FlatAppearance.BorderSize = 0
        btnModifierPassword.FlatStyle = FlatStyle.Flat
        btnModifierPassword.ForeColor = Color.White
        errProvider.SetIconAlignment(btnModifierPassword, ErrorIconAlignment.TopLeft)
        btnModifierPassword.Image = CType(resources.GetObject("btnModifierPassword.Image"), Image)
        btnModifierPassword.ImageAlign = ContentAlignment.MiddleLeft
        btnModifierPassword.Location = New Point(582, 188)
        btnModifierPassword.Margin = New Padding(3, 0, 3, 8)
        btnModifierPassword.Name = "btnModifierPassword"
        btnModifierPassword.Size = New Size(126, 36)
        btnModifierPassword.TabIndex = 11
        btnModifierPassword.Tag = "modifierPW_normal"
        btnModifierPassword.Text = "Modifier PW"
        btnModifierPassword.TextAlign = ContentAlignment.MiddleLeft
        btnModifierPassword.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModifierPassword.UseVisualStyleBackColor = False
        ' 
        ' pnlForm
        ' 
        pnlForm.BackgroundImage = My.Resources.Resources.Fond_1000x800_Feuille4
        pnlForm.BackgroundImageLayout = ImageLayout.Stretch
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(pnlTop)
        pnlForm.Controls.Add(pnlTitre)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(4, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Size = New Size(736, 501)
        pnlForm.TabIndex = 19
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(tlpConnexion)
        pnlCenter.Controls.Add(Panel1)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(0, 100)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(6)
        pnlCenter.Size = New Size(736, 347)
        pnlCenter.TabIndex = 19
        ' 
        ' tlpConnexion
        ' 
        tlpConnexion.ColumnCount = 4
        tlpConnexion.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 19F))
        tlpConnexion.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 55F))
        tlpConnexion.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 7F))
        tlpConnexion.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 19F))
        tlpConnexion.Controls.Add(txtHost, 1, 0)
        tlpConnexion.Controls.Add(lblHost, 0, 0)
        tlpConnexion.Controls.Add(lblPort, 0, 1)
        tlpConnexion.Controls.Add(nudPort, 1, 1)
        tlpConnexion.Controls.Add(lblDatabaseName, 0, 2)
        tlpConnexion.Controls.Add(txtDatabaseName, 1, 2)
        tlpConnexion.Controls.Add(lblUserName, 0, 3)
        tlpConnexion.Controls.Add(txtUserName, 1, 3)
        tlpConnexion.Controls.Add(lblPassword, 0, 4)
        tlpConnexion.Controls.Add(txtPassword, 1, 4)
        tlpConnexion.Controls.Add(lblAdditionalOptions, 0, 5)
        tlpConnexion.Controls.Add(txtAdditionalOptions, 1, 5)
        tlpConnexion.Controls.Add(btnVoirPassword, 2, 4)
        tlpConnexion.Controls.Add(btnModifierPassword, 3, 4)
        tlpConnexion.Dock = DockStyle.Top
        tlpConnexion.Font = New Font("Calibri", 10F)
        tlpConnexion.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        tlpConnexion.Location = New Point(6, 6)
        tlpConnexion.Name = "tlpConnexion"
        tlpConnexion.Padding = New Padding(12)
        tlpConnexion.RowCount = 6
        tlpConnexion.RowStyles.Add(New RowStyle(SizeType.Percent, 16.6666679F))
        tlpConnexion.RowStyles.Add(New RowStyle(SizeType.Percent, 16.6666679F))
        tlpConnexion.RowStyles.Add(New RowStyle(SizeType.Percent, 16.6666679F))
        tlpConnexion.RowStyles.Add(New RowStyle(SizeType.Percent, 16.6666679F))
        tlpConnexion.RowStyles.Add(New RowStyle(SizeType.Percent, 16.6666679F))
        tlpConnexion.RowStyles.Add(New RowStyle(SizeType.Percent, 16.6666679F))
        tlpConnexion.Size = New Size(724, 294)
        tlpConnexion.TabIndex = 2
        ' 
        ' txtHost
        ' 
        txtHost.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtHost.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtHost.Location = New Point(148, 15)
        txtHost.Name = "txtHost"
        txtHost.Size = New Size(379, 24)
        txtHost.TabIndex = 1
        ' 
        ' lblHost
        ' 
        lblHost.AutoSize = True
        lblHost.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblHost.Location = New Point(15, 12)
        lblHost.Name = "lblHost"
        lblHost.Size = New Size(51, 17)
        lblHost.TabIndex = 11
        lblHost.Text = "Serveur"
        ' 
        ' lblPort
        ' 
        lblPort.AutoSize = True
        lblPort.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPort.Location = New Point(15, 56)
        lblPort.Name = "lblPort"
        lblPort.Size = New Size(32, 17)
        lblPort.TabIndex = 0
        lblPort.Text = "Port"
        ' 
        ' nudPort
        ' 
        nudPort.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        nudPort.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        nudPort.Location = New Point(148, 59)
        nudPort.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
        nudPort.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        nudPort.Name = "nudPort"
        nudPort.Size = New Size(69, 24)
        nudPort.TabIndex = 2
        nudPort.Value = New Decimal(New Integer() {3306, 0, 0, 0})
        ' 
        ' lblDatabaseName
        ' 
        lblDatabaseName.AutoSize = True
        lblDatabaseName.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDatabaseName.Location = New Point(15, 100)
        lblDatabaseName.Name = "lblDatabaseName"
        lblDatabaseName.Size = New Size(102, 17)
        lblDatabaseName.TabIndex = 2
        lblDatabaseName.Text = "Base de données"
        ' 
        ' txtDatabaseName
        ' 
        txtDatabaseName.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtDatabaseName.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtDatabaseName.Location = New Point(148, 103)
        txtDatabaseName.Name = "txtDatabaseName"
        txtDatabaseName.Size = New Size(379, 24)
        txtDatabaseName.TabIndex = 3
        ' 
        ' lblUserName
        ' 
        lblUserName.AutoSize = True
        lblUserName.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblUserName.Location = New Point(15, 144)
        lblUserName.Name = "lblUserName"
        lblUserName.Size = New Size(67, 17)
        lblUserName.TabIndex = 4
        lblUserName.Text = "Utilisateur"
        ' 
        ' txtUserName
        ' 
        txtUserName.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtUserName.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtUserName.Location = New Point(148, 147)
        txtUserName.Name = "txtUserName"
        txtUserName.Size = New Size(379, 24)
        txtUserName.TabIndex = 4
        ' 
        ' lblPassword
        ' 
        lblPassword.AutoSize = True
        lblPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPassword.Location = New Point(15, 188)
        lblPassword.Name = "lblPassword"
        lblPassword.Size = New Size(83, 17)
        lblPassword.TabIndex = 6
        lblPassword.Text = "Mot de passe"
        ' 
        ' txtPassword
        ' 
        txtPassword.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtPassword.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPassword.Location = New Point(148, 191)
        txtPassword.Name = "txtPassword"
        txtPassword.Size = New Size(379, 24)
        txtPassword.TabIndex = 5
        txtPassword.UseSystemPasswordChar = True
        ' 
        ' lblAdditionalOptions
        ' 
        lblAdditionalOptions.AutoSize = True
        lblAdditionalOptions.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAdditionalOptions.Location = New Point(15, 232)
        lblAdditionalOptions.Name = "lblAdditionalOptions"
        lblAdditionalOptions.Size = New Size(113, 17)
        lblAdditionalOptions.TabIndex = 9
        lblAdditionalOptions.Text = "Options (facultatif)"
        ' 
        ' txtAdditionalOptions
        ' 
        txtAdditionalOptions.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtAdditionalOptions.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdditionalOptions.Location = New Point(148, 235)
        txtAdditionalOptions.Name = "txtAdditionalOptions"
        txtAdditionalOptions.Size = New Size(379, 24)
        txtAdditionalOptions.TabIndex = 7
        ' 
        ' btnVoirPassword
        ' 
        btnVoirPassword.Anchor = AnchorStyles.Top
        btnVoirPassword.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnVoirPassword.Image = CType(resources.GetObject("btnVoirPassword.Image"), Image)
        btnVoirPassword.Location = New Point(536, 188)
        btnVoirPassword.Margin = New Padding(0)
        btnVoirPassword.Name = "btnVoirPassword"
        btnVoirPassword.Size = New Size(37, 37)
        btnVoirPassword.TabIndex = 6
        btnVoirPassword.Tag = "voir_normal"
        btnVoirPassword.TextImageRelation = TextImageRelation.ImageAboveText
        btnVoirPassword.UseVisualStyleBackColor = False
        ' 
        ' Panel1
        ' 
        Panel1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Panel1.Controls.Add(lblConnectionResult)
        Panel1.Location = New Point(0, 304)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(748, 43)
        Panel1.TabIndex = 0
        ' 
        ' lblConnectionResult
        ' 
        lblConnectionResult.BackColor = Color.Transparent
        lblConnectionResult.BorderStyle = BorderStyle.Fixed3D
        lblConnectionResult.Dock = DockStyle.Fill
        lblConnectionResult.Font = New Font("Calibri", 10F)
        lblConnectionResult.ForeColor = Color.FromArgb(CByte(194), CByte(106), CByte(118))
        lblConnectionResult.Location = New Point(0, 0)
        lblConnectionResult.Name = "lblConnectionResult"
        lblConnectionResult.Padding = New Padding(12)
        lblConnectionResult.Size = New Size(748, 43)
        lblConnectionResult.TabIndex = 0
        lblConnectionResult.Text = "Aucun test effectué"
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.FromArgb(CByte(231), CByte(223), CByte(214))
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.Fixed3D
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Controls.Add(btnEnregistrerConnexion)
        pnlActions.Controls.Add(btnTesterConnexion)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 447)
        pnlActions.Name = "pnlActions"
        pnlActions.Padding = New Padding(8)
        pnlActions.Size = New Size(736, 54)
        pnlActions.TabIndex = 18
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
        pnlTop.Size = New Size(736, 42)
        pnlTop.TabIndex = 4
        ' 
        ' lblTop
        ' 
        lblTop.Dock = DockStyle.Fill
        lblTop.Font = New Font("Calibri", 12F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(8, 8)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(720, 26)
        lblTop.TabIndex = 0
        lblTop.Text = "Renseignez les paramètres MariaDB, testez la connexion, puis enregistrez uniquement si le test est valide."
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(0, 0)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(736, 58)
        pnlTitre.TabIndex = 3
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.Anchor = AnchorStyles.Left
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 15F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(49, 9)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(8, 4, 8, 4)
        lblTitreForm.Size = New Size(298, 32)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Connexion à la base de données"
        ' 
        ' ConfigurationConnexion
        ' 
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(744, 501)
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 9F)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(2, 3, 2, 3)
        MaximizeBox = False
        MinimizeBox = False
        MinimumSize = New Size(760, 540)
        Name = "ConfigurationConnexion"
        Padding = New Padding(4, 0, 4, 0)
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Configuration de la connexion MariaDB"
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        tlpConnexion.ResumeLayout(False)
        tlpConnexion.PerformLayout()
        CType(nudPort, ComponentModel.ISupportInitialize).EndInit()
        Panel1.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        pnlTop.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents pnlTop As Panel
    Friend WithEvents lblTop As Label
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnClose As Button
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents tlpConnexion As TableLayoutPanel
    Friend WithEvents lblPort As Label
    Friend WithEvents nudPort As NumericUpDown
    Friend WithEvents lblDatabaseName As Label
    Friend WithEvents txtDatabaseName As TextBox
    Friend WithEvents lblUserName As Label
    Friend WithEvents txtUserName As TextBox
    Friend WithEvents lblPassword As Label
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents lblAdditionalOptions As Label
    Friend WithEvents lblConnectionResult As Label
    Friend WithEvents btnTesterConnexion As Button
    Friend WithEvents btnFermer As Button
    Friend WithEvents btnEnregistrerConnexion As Button
    Friend WithEvents lblHost As Label
    Friend WithEvents txtHost As TextBox
    Friend WithEvents txtAdditionalOptions As TextBox
    Friend WithEvents btnVoirPassword As Button
    Friend WithEvents btnModifierPassword As Button
End Class
