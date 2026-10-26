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
        enTete = New AltheaEnTete()
        carteConnexion = New AltheaCarte()
        lblUserName = New Label()
        txtUserName = New TextBox()
        lblPassword = New Label()
        txtPassword = New TextBox()
        btnVoirPassword = New AltheaBouton()
        lblMessage = New Label()
        pnlActions = New AltheaBarreActions()
        btnAnnuler = New AltheaBouton()
        btnConnexion = New AltheaBouton()
        stsStatus = New StatusStrip()
        stsLabelStatus = New ToolStripStatusLabel()
        ttMain = New ToolTip(components)
        errProvider = New ErrorProvider(components)
        carteConnexion.SuspendLayout()
        pnlActions.SuspendLayout()
        stsStatus.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(464, 62)
        enTete.TabIndex = 20
        enTete.SousTitre = "Identifiez-vous pour accéder à Althéa"
        enTete.Text = "Connexion"
        ' 
        ' carteConnexion
        ' 
        carteConnexion.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        carteConnexion.Controls.Add(lblUserName)
        carteConnexion.Controls.Add(txtUserName)
        carteConnexion.Controls.Add(lblPassword)
        carteConnexion.Controls.Add(txtPassword)
        carteConnexion.Controls.Add(btnVoirPassword)
        carteConnexion.Controls.Add(lblMessage)
        carteConnexion.Location = New Point(12, 70)
        carteConnexion.Name = "carteConnexion"
        carteConnexion.Padding = New Padding(12, 8, 12, 10)
        carteConnexion.Size = New Size(440, 208)
        carteConnexion.TabIndex = 0
        carteConnexion.TabStop = False
        carteConnexion.Titre = "Veuillez vous connecter"
        ' 
        ' lblUserName
        ' 
        lblUserName.AutoSize = True
        lblUserName.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblUserName.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblUserName.Location = New Point(14, 44)
        lblUserName.Name = "lblUserName"
        lblUserName.Size = New Size(65, 17)
        lblUserName.TabIndex = 0
        lblUserName.Text = "Utilisateur"
        ' 
        ' txtUserName
        ' 
        txtUserName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtUserName.BackColor = Color.White
        txtUserName.Font = New Font("Calibri", 11F)
        txtUserName.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtUserName.Location = New Point(130, 40)
        txtUserName.Name = "txtUserName"
        txtUserName.Size = New Size(240, 25)
        txtUserName.TabIndex = 1
        ' 
        ' lblPassword
        ' 
        lblPassword.AutoSize = True
        lblPassword.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPassword.Location = New Point(14, 76)
        lblPassword.Name = "lblPassword"
        lblPassword.Size = New Size(81, 17)
        lblPassword.TabIndex = 2
        lblPassword.Text = "Mot de passe"
        ' 
        ' txtPassword
        ' 
        txtPassword.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtPassword.BackColor = Color.White
        txtPassword.Font = New Font("Calibri", 11F)
        txtPassword.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPassword.Location = New Point(130, 72)
        txtPassword.Name = "txtPassword"
        txtPassword.Size = New Size(240, 25)
        txtPassword.TabIndex = 3
        txtPassword.UseSystemPasswordChar = True
        ' 
        ' btnVoirPassword
        ' 
        btnVoirPassword.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnVoirPassword.Image = CType(resources.GetObject("btnVoirPassword.Image"), Image)
        btnVoirPassword.Location = New Point(374, 70)
        btnVoirPassword.Name = "btnVoirPassword"
        btnVoirPassword.Size = New Size(30, 30)
        btnVoirPassword.TabIndex = 4
        btnVoirPassword.Tag = "voir_normal"
        btnVoirPassword.Variante = VarianteBouton.Outil
        ' 
        ' lblMessage
        ' 
        lblMessage.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblMessage.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblMessage.ForeColor = Color.FromArgb(CByte(194), CByte(106), CByte(118))
        lblMessage.Location = New Point(14, 112)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(412, 80)
        lblMessage.TabIndex = 15
        lblMessage.Text = "Message"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnConnexion)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 286)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(464, 50)
        pnlActions.TabIndex = 22
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(334, 7)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 36)
        btnAnnuler.TabIndex = 5
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        errProvider.SetIconAlignment(btnAnnuler, ErrorIconAlignment.TopLeft)
        ' 
        ' btnConnexion
        ' 
        btnConnexion.Image = CType(resources.GetObject("btnConnexion.Image"), Image)
        btnConnexion.Location = New Point(226, 7)
        btnConnexion.Name = "btnConnexion"
        btnConnexion.Size = New Size(98, 36)
        btnConnexion.TabIndex = 4
        btnConnexion.Tag = "login_normal"
        btnConnexion.Text = "Login"
        errProvider.SetIconAlignment(btnConnexion, ErrorIconAlignment.MiddleLeft)
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
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(464, 361)
        Controls.Add(carteConnexion)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        Controls.Add(stsStatus)
        Font = New Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MinimumSize = New Size(480, 400)
        Name = "Login"
        StartPosition = FormStartPosition.CenterParent
        Text = "Login Althea"
        carteConnexion.ResumeLayout(False)
        carteConnexion.PerformLayout()
        pnlActions.ResumeLayout(False)
        stsStatus.ResumeLayout(False)
        stsStatus.PerformLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents stsStatus As StatusStrip
    Friend WithEvents stsLabelStatus As ToolStripStatusLabel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents carteConnexion As AltheaCarte
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnConnexion As AltheaBouton
    Friend WithEvents lblUserName As Label
    Friend WithEvents txtUserName As TextBox
    Friend WithEvents lblPassword As Label
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents btnVoirPassword As AltheaBouton
    Friend WithEvents lblMessage As Label
    Friend WithEvents btnAnnuler As AltheaBouton
End Class
