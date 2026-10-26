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
        enTete = New AltheaEnTete()
        carteParametres = New AltheaCarte()
        lblIntro = New Label()
        lblHost = New Label()
        txtHost = New TextBox()
        lblPort = New Label()
        nudPort = New NumericUpDown()
        lblDatabaseName = New Label()
        txtDatabaseName = New TextBox()
        lblUserName = New Label()
        txtUserName = New TextBox()
        lblPassword = New Label()
        txtPassword = New TextBox()
        btnVoirPassword = New AltheaBouton()
        btnModifierPassword = New AltheaBouton()
        lblAdditionalOptions = New Label()
        txtAdditionalOptions = New TextBox()
        carteTest = New AltheaCarte()
        lblConnectionResult = New Label()
        pnlActions = New AltheaBarreActions()
        btnTesterConnexion = New AltheaBouton()
        btnEnregistrerConnexion = New AltheaBouton()
        btnFermer = New AltheaBouton()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        carteParametres.SuspendLayout()
        CType(nudPort, ComponentModel.ISupportInitialize).BeginInit()
        carteTest.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(744, 62)
        enTete.TabIndex = 3
        enTete.SousTitre = "Gérez la connexion à la base de données"
        enTete.Text = "Connexion"
        ' 
        ' carteParametres
        ' 
        carteParametres.Controls.Add(lblIntro)
        carteParametres.Controls.Add(lblHost)
        carteParametres.Controls.Add(txtHost)
        carteParametres.Controls.Add(lblPort)
        carteParametres.Controls.Add(nudPort)
        carteParametres.Controls.Add(lblDatabaseName)
        carteParametres.Controls.Add(txtDatabaseName)
        carteParametres.Controls.Add(lblUserName)
        carteParametres.Controls.Add(txtUserName)
        carteParametres.Controls.Add(lblPassword)
        carteParametres.Controls.Add(txtPassword)
        carteParametres.Controls.Add(btnVoirPassword)
        carteParametres.Controls.Add(btnModifierPassword)
        carteParametres.Controls.Add(lblAdditionalOptions)
        carteParametres.Controls.Add(txtAdditionalOptions)
        carteParametres.Location = New Point(12, 70)
        carteParametres.Name = "carteParametres"
        carteParametres.Padding = New Padding(12, 8, 12, 10)
        carteParametres.Size = New Size(720, 277)
        carteParametres.TabIndex = 0
        carteParametres.TabStop = False
        carteParametres.Titre = "Paramètres MariaDB"
        ' 
        ' lblIntro
        ' 
        lblIntro.Font = New Font("Calibri", 10F, FontStyle.Italic)
        lblIntro.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblIntro.Location = New Point(14, 40)
        lblIntro.Name = "lblIntro"
        lblIntro.Size = New Size(692, 20)
        lblIntro.TabIndex = 20
        lblIntro.Text = "Renseignez les paramètres MariaDB, testez la connexion, puis enregistrez uniquement si le test est valide."
        ' 
        ' lblHost
        ' 
        lblHost.AutoSize = True
        lblHost.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblHost.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblHost.Location = New Point(14, 74)
        lblHost.Name = "lblHost"
        lblHost.Size = New Size(45, 17)
        lblHost.TabIndex = 21
        lblHost.Text = "Serveur"
        ' 
        ' txtHost
        ' 
        txtHost.BackColor = Color.White
        txtHost.Font = New Font("Calibri", 11F)
        txtHost.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtHost.Location = New Point(150, 70)
        txtHost.Name = "txtHost"
        txtHost.Size = New Size(380, 25)
        txtHost.TabIndex = 1
        ' 
        ' lblPort
        ' 
        lblPort.AutoSize = True
        lblPort.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPort.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPort.Location = New Point(14, 108)
        lblPort.Name = "lblPort"
        lblPort.Size = New Size(25, 17)
        lblPort.TabIndex = 22
        lblPort.Text = "Port"
        ' 
        ' nudPort
        ' 
        nudPort.BackColor = Color.White
        nudPort.Font = New Font("Calibri", 11F)
        nudPort.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        nudPort.Location = New Point(150, 104)
        nudPort.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
        nudPort.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        nudPort.Name = "nudPort"
        nudPort.Size = New Size(80, 25)
        nudPort.TabIndex = 2
        nudPort.Value = New Decimal(New Integer() {3306, 0, 0, 0})
        ' 
        ' lblDatabaseName
        ' 
        lblDatabaseName.AutoSize = True
        lblDatabaseName.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDatabaseName.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDatabaseName.Location = New Point(14, 142)
        lblDatabaseName.Name = "lblDatabaseName"
        lblDatabaseName.Size = New Size(98, 17)
        lblDatabaseName.TabIndex = 23
        lblDatabaseName.Text = "Base de données"
        ' 
        ' txtDatabaseName
        ' 
        txtDatabaseName.BackColor = Color.White
        txtDatabaseName.Font = New Font("Calibri", 11F)
        txtDatabaseName.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtDatabaseName.Location = New Point(150, 138)
        txtDatabaseName.Name = "txtDatabaseName"
        txtDatabaseName.Size = New Size(380, 25)
        txtDatabaseName.TabIndex = 3
        ' 
        ' lblUserName
        ' 
        lblUserName.AutoSize = True
        lblUserName.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblUserName.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblUserName.Location = New Point(14, 176)
        lblUserName.Name = "lblUserName"
        lblUserName.Size = New Size(61, 17)
        lblUserName.TabIndex = 24
        lblUserName.Text = "Utilisateur"
        ' 
        ' txtUserName
        ' 
        txtUserName.BackColor = Color.White
        txtUserName.Font = New Font("Calibri", 11F)
        txtUserName.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtUserName.Location = New Point(150, 172)
        txtUserName.Name = "txtUserName"
        txtUserName.Size = New Size(380, 25)
        txtUserName.TabIndex = 4
        ' 
        ' lblPassword
        ' 
        lblPassword.AutoSize = True
        lblPassword.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPassword.Location = New Point(14, 210)
        lblPassword.Name = "lblPassword"
        lblPassword.Size = New Size(78, 17)
        lblPassword.TabIndex = 25
        lblPassword.Text = "Mot de passe"
        ' 
        ' txtPassword
        ' 
        txtPassword.BackColor = Color.White
        txtPassword.Font = New Font("Calibri", 11F)
        txtPassword.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPassword.Location = New Point(150, 206)
        txtPassword.Name = "txtPassword"
        txtPassword.Size = New Size(380, 25)
        txtPassword.TabIndex = 5
        txtPassword.UseSystemPasswordChar = True
        ' 
        ' btnVoirPassword
        ' 
        btnVoirPassword.Image = CType(resources.GetObject("btnVoirPassword.Image"), Image)
        btnVoirPassword.Location = New Point(534, 204)
        btnVoirPassword.Name = "btnVoirPassword"
        btnVoirPassword.Size = New Size(30, 30)
        btnVoirPassword.TabIndex = 6
        btnVoirPassword.Tag = "voir_normal"
        btnVoirPassword.Variante = VarianteBouton.Outil
        ' 
        ' btnModifierPassword
        ' 
        errProvider.SetIconAlignment(btnModifierPassword, ErrorIconAlignment.TopLeft)
        btnModifierPassword.Image = CType(resources.GetObject("btnModifierPassword.Image"), Image)
        btnModifierPassword.Location = New Point(570, 204)
        btnModifierPassword.Name = "btnModifierPassword"
        btnModifierPassword.Size = New Size(120, 30)
        btnModifierPassword.TabIndex = 7
        btnModifierPassword.Tag = "modifierPW_normal"
        btnModifierPassword.Text = "Modifier"
        btnModifierPassword.Variante = VarianteBouton.Clair
        ' 
        ' lblAdditionalOptions
        ' 
        lblAdditionalOptions.AutoSize = True
        lblAdditionalOptions.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblAdditionalOptions.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAdditionalOptions.Location = New Point(14, 244)
        lblAdditionalOptions.Name = "lblAdditionalOptions"
        lblAdditionalOptions.Size = New Size(110, 17)
        lblAdditionalOptions.TabIndex = 26
        lblAdditionalOptions.Text = "Options (facultatif)"
        ' 
        ' txtAdditionalOptions
        ' 
        txtAdditionalOptions.BackColor = Color.White
        txtAdditionalOptions.Font = New Font("Calibri", 11F)
        txtAdditionalOptions.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtAdditionalOptions.Location = New Point(150, 240)
        txtAdditionalOptions.Name = "txtAdditionalOptions"
        txtAdditionalOptions.Size = New Size(380, 25)
        txtAdditionalOptions.TabIndex = 8
        ' 
        ' carteTest
        ' 
        carteTest.Controls.Add(lblConnectionResult)
        carteTest.Location = New Point(12, 355)
        carteTest.Name = "carteTest"
        carteTest.Padding = New Padding(12, 8, 12, 10)
        carteTest.Size = New Size(720, 80)
        carteTest.TabIndex = 1
        carteTest.TabStop = False
        carteTest.Titre = "Test de connexion"
        ' 
        ' lblConnectionResult
        ' 
        lblConnectionResult.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblConnectionResult.ForeColor = Color.FromArgb(CByte(194), CByte(106), CByte(118))
        lblConnectionResult.Location = New Point(14, 40)
        lblConnectionResult.Name = "lblConnectionResult"
        lblConnectionResult.Size = New Size(692, 28)
        lblConnectionResult.TabIndex = 0
        lblConnectionResult.Text = "Aucun test effectué"
        lblConnectionResult.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Controls.Add(btnEnregistrerConnexion)
        pnlActions.Controls.Add(btnTesterConnexion)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 447)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(744, 54)
        pnlActions.TabIndex = 18
        ' 
        ' btnTesterConnexion
        ' 
        btnTesterConnexion.Image = CType(resources.GetObject("btnTesterConnexion.Image"), Image)
        btnTesterConnexion.Location = New Point(136, 7)
        btnTesterConnexion.Name = "btnTesterConnexion"
        btnTesterConnexion.Size = New Size(170, 40)
        btnTesterConnexion.TabIndex = 9
        btnTesterConnexion.Tag = "testerConnexion_normal"
        btnTesterConnexion.Text = "Tester la connexion"
        errProvider.SetIconAlignment(btnTesterConnexion, ErrorIconAlignment.TopLeft)
        ' 
        ' btnEnregistrerConnexion
        ' 
        btnEnregistrerConnexion.Image = CType(resources.GetObject("btnEnregistrerConnexion.Image"), Image)
        btnEnregistrerConnexion.Location = New Point(316, 7)
        btnEnregistrerConnexion.Name = "btnEnregistrerConnexion"
        btnEnregistrerConnexion.Size = New Size(132, 40)
        btnEnregistrerConnexion.TabIndex = 10
        btnEnregistrerConnexion.Tag = "enregistrer_normal"
        btnEnregistrerConnexion.Text = "Enregistrer"
        btnEnregistrerConnexion.Variante = VarianteBouton.Ocre
        errProvider.SetIconAlignment(btnEnregistrerConnexion, ErrorIconAlignment.TopLeft)
        ' 
        ' btnFermer
        ' 
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.Location = New Point(458, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(109, 40)
        btnFermer.TabIndex = 11
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        errProvider.SetIconAlignment(btnFermer, ErrorIconAlignment.TopLeft)
        ' 
        ' ConfigurationConnexion
        ' 
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(744, 501)
        Controls.Add(carteParametres)
        Controls.Add(carteTest)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        Font = New Font("Calibri", 9F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(2, 3, 2, 3)
        MaximizeBox = False
        MinimizeBox = False
        MinimumSize = New Size(760, 540)
        Name = "ConfigurationConnexion"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Configuration de la connexion MariaDB"
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        carteParametres.ResumeLayout(False)
        carteParametres.PerformLayout()
        CType(nudPort, ComponentModel.ISupportInitialize).EndInit()
        carteTest.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteParametres As AltheaCarte
    Friend WithEvents lblIntro As Label
    Friend WithEvents lblHost As Label
    Friend WithEvents txtHost As TextBox
    Friend WithEvents lblPort As Label
    Friend WithEvents nudPort As NumericUpDown
    Friend WithEvents lblDatabaseName As Label
    Friend WithEvents txtDatabaseName As TextBox
    Friend WithEvents lblUserName As Label
    Friend WithEvents txtUserName As TextBox
    Friend WithEvents lblPassword As Label
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents btnVoirPassword As AltheaBouton
    Friend WithEvents btnModifierPassword As AltheaBouton
    Friend WithEvents lblAdditionalOptions As Label
    Friend WithEvents txtAdditionalOptions As TextBox
    Friend WithEvents carteTest As AltheaCarte
    Friend WithEvents lblConnectionResult As Label
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnTesterConnexion As AltheaBouton
    Friend WithEvents btnEnregistrerConnexion As AltheaBouton
    Friend WithEvents btnFermer As AltheaBouton
End Class
