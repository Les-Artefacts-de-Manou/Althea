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
        enTete = New AltheaEnTete()
        carteMotDePasse = New AltheaCarte()
        lblOldPassword = New Label()
        txtOldPassword = New TextBox()
        btnVoirOldPassword = New AltheaBouton()
        lblNewPassword = New Label()
        txtNewPassword = New TextBox()
        btnVoirNewPassword = New AltheaBouton()
        lblPasswordRules = New Label()
        lblConfirmation = New Label()
        txtConfirmation = New TextBox()
        btnVoirConfirmation = New AltheaBouton()
        lblMessage = New Label()
        pnlActions = New AltheaBarreActions()
        btnAnnuler = New AltheaBouton()
        btnValider = New AltheaBouton()
        stsStatus = New StatusStrip()
        stsLabelStatus = New ToolStripStatusLabel()
        errProvider = New ErrorProvider(components)
        ttMain = New ToolTip(components)
        carteMotDePasse.SuspendLayout()
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
        enTete.TabIndex = 21
        enTete.SousTitre = "Changez de mot de passe"
        enTete.Text = "Mot de passe"
        ' 
        ' carteMotDePasse
        ' 
        carteMotDePasse.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        carteMotDePasse.Controls.Add(lblOldPassword)
        carteMotDePasse.Controls.Add(txtOldPassword)
        carteMotDePasse.Controls.Add(btnVoirOldPassword)
        carteMotDePasse.Controls.Add(lblNewPassword)
        carteMotDePasse.Controls.Add(txtNewPassword)
        carteMotDePasse.Controls.Add(btnVoirNewPassword)
        carteMotDePasse.Controls.Add(lblPasswordRules)
        carteMotDePasse.Controls.Add(lblConfirmation)
        carteMotDePasse.Controls.Add(txtConfirmation)
        carteMotDePasse.Controls.Add(btnVoirConfirmation)
        carteMotDePasse.Controls.Add(lblMessage)
        carteMotDePasse.Location = New Point(12, 70)
        carteMotDePasse.Name = "carteMotDePasse"
        carteMotDePasse.Padding = New Padding(12, 8, 12, 10)
        carteMotDePasse.Size = New Size(440, 223)
        carteMotDePasse.TabIndex = 0
        carteMotDePasse.TabStop = False
        carteMotDePasse.Titre = "Mot de passe"
        ' 
        ' lblOldPassword
        ' 
        lblOldPassword.AutoSize = True
        lblOldPassword.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblOldPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblOldPassword.Location = New Point(14, 44)
        lblOldPassword.Name = "lblOldPassword"
        lblOldPassword.Size = New Size(121, 17)
        lblOldPassword.TabIndex = 0
        lblOldPassword.Text = "Ancien mot de passe"
        ' 
        ' txtOldPassword
        ' 
        txtOldPassword.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtOldPassword.BackColor = Color.White
        txtOldPassword.Font = New Font("Calibri", 11F)
        txtOldPassword.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtOldPassword.Location = New Point(160, 40)
        txtOldPassword.Name = "txtOldPassword"
        txtOldPassword.Size = New Size(230, 25)
        txtOldPassword.TabIndex = 1
        txtOldPassword.UseSystemPasswordChar = True
        ' 
        ' btnVoirOldPassword
        ' 
        btnVoirOldPassword.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnVoirOldPassword.Image = CType(resources.GetObject("btnVoirOldPassword.Image"), Image)
        btnVoirOldPassword.Location = New Point(394, 38)
        btnVoirOldPassword.Name = "btnVoirOldPassword"
        btnVoirOldPassword.Size = New Size(30, 30)
        btnVoirOldPassword.TabIndex = 2
        btnVoirOldPassword.Tag = "voir_normal"
        btnVoirOldPassword.Variante = VarianteBouton.Outil
        ' 
        ' lblNewPassword
        ' 
        lblNewPassword.AutoSize = True
        lblNewPassword.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNewPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNewPassword.Location = New Point(14, 78)
        lblNewPassword.Name = "lblNewPassword"
        lblNewPassword.Size = New Size(133, 17)
        lblNewPassword.TabIndex = 3
        lblNewPassword.Text = "Nouveau mot de passe"
        ' 
        ' txtNewPassword
        ' 
        txtNewPassword.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtNewPassword.BackColor = Color.White
        txtNewPassword.Font = New Font("Calibri", 11F)
        txtNewPassword.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNewPassword.Location = New Point(160, 74)
        txtNewPassword.Name = "txtNewPassword"
        txtNewPassword.Size = New Size(230, 25)
        txtNewPassword.TabIndex = 4
        txtNewPassword.UseSystemPasswordChar = True
        ' 
        ' btnVoirNewPassword
        ' 
        btnVoirNewPassword.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnVoirNewPassword.Image = CType(resources.GetObject("btnVoirNewPassword.Image"), Image)
        btnVoirNewPassword.Location = New Point(394, 72)
        btnVoirNewPassword.Name = "btnVoirNewPassword"
        btnVoirNewPassword.Size = New Size(30, 30)
        btnVoirNewPassword.TabIndex = 5
        btnVoirNewPassword.Tag = "voir_normal"
        btnVoirNewPassword.Variante = VarianteBouton.Outil
        ' 
        ' lblPasswordRules
        ' 
        lblPasswordRules.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblPasswordRules.Font = New Font("Calibri", 9F, FontStyle.Italic)
        lblPasswordRules.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPasswordRules.Location = New Point(14, 104)
        lblPasswordRules.Name = "lblPasswordRules"
        lblPasswordRules.Size = New Size(412, 18)
        lblPasswordRules.TabIndex = 6
        lblPasswordRules.Text = "Minimum 10 caractères, avec au moins 1 majuscule, 1 minuscule et 1 chiffre."
        lblPasswordRules.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' lblConfirmation
        ' 
        lblConfirmation.AutoSize = True
        lblConfirmation.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblConfirmation.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblConfirmation.Location = New Point(14, 130)
        lblConfirmation.Name = "lblConfirmation"
        lblConfirmation.Size = New Size(77, 17)
        lblConfirmation.TabIndex = 7
        lblConfirmation.Text = "Confirmation"
        ' 
        ' txtConfirmation
        ' 
        txtConfirmation.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtConfirmation.BackColor = Color.White
        txtConfirmation.Font = New Font("Calibri", 11F)
        txtConfirmation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtConfirmation.Location = New Point(160, 126)
        txtConfirmation.Name = "txtConfirmation"
        txtConfirmation.Size = New Size(230, 25)
        txtConfirmation.TabIndex = 8
        txtConfirmation.UseSystemPasswordChar = True
        ' 
        ' btnVoirConfirmation
        ' 
        btnVoirConfirmation.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnVoirConfirmation.Image = CType(resources.GetObject("btnVoirConfirmation.Image"), Image)
        btnVoirConfirmation.Location = New Point(394, 124)
        btnVoirConfirmation.Name = "btnVoirConfirmation"
        btnVoirConfirmation.Size = New Size(30, 30)
        btnVoirConfirmation.TabIndex = 9
        btnVoirConfirmation.Tag = "voir_normal"
        btnVoirConfirmation.Variante = VarianteBouton.Outil
        ' 
        ' lblMessage
        ' 
        lblMessage.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblMessage.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblMessage.ForeColor = Color.FromArgb(CByte(194), CByte(106), CByte(118))
        lblMessage.Location = New Point(14, 162)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(412, 50)
        lblMessage.TabIndex = 10
        lblMessage.Text = "Message"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnValider)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 301)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(464, 57)
        pnlActions.TabIndex = 23
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(334, 8)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 40)
        btnAnnuler.TabIndex = 8
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        errProvider.SetIconAlignment(btnAnnuler, ErrorIconAlignment.TopLeft)
        ' 
        ' btnValider
        ' 
        btnValider.Image = CType(resources.GetObject("btnValider.Image"), Image)
        btnValider.Location = New Point(216, 8)
        btnValider.Name = "btnValider"
        btnValider.Size = New Size(108, 40)
        btnValider.TabIndex = 7
        btnValider.Tag = "valider_normal"
        btnValider.Text = "Valider"
        errProvider.SetIconAlignment(btnValider, ErrorIconAlignment.MiddleLeft)
        ' 
        ' stsStatus
        ' 
        stsStatus.AllowItemReorder = True
        stsStatus.AutoSize = False
        stsStatus.BackColor = Color.FromArgb(CByte(218), CByte(201), CByte(184))
        stsStatus.Items.AddRange(New ToolStripItem() {stsLabelStatus})
        stsStatus.Location = New Point(0, 358)
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
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' ChangePassword
        ' 
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(464, 381)
        Controls.Add(carteMotDePasse)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        Controls.Add(stsStatus)
        Font = New Font("Calibri", 9F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(2, 3, 2, 3)
        MinimumSize = New Size(480, 400)
        Name = "ChangePassword"
        StartPosition = FormStartPosition.CenterParent
        Text = "Changement de Mot de passe"
        carteMotDePasse.ResumeLayout(False)
        carteMotDePasse.PerformLayout()
        pnlActions.ResumeLayout(False)
        stsStatus.ResumeLayout(False)
        stsStatus.PerformLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents stsStatus As StatusStrip
    Friend WithEvents stsLabelStatus As ToolStripStatusLabel
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents ttMain As ToolTip
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents btnValider As AltheaBouton
    Friend WithEvents carteMotDePasse As AltheaCarte
    Friend WithEvents lblOldPassword As Label
    Friend WithEvents txtOldPassword As TextBox
    Friend WithEvents btnVoirOldPassword As AltheaBouton
    Friend WithEvents lblNewPassword As Label
    Friend WithEvents txtNewPassword As TextBox
    Friend WithEvents btnVoirNewPassword As AltheaBouton
    Friend WithEvents lblPasswordRules As Label
    Friend WithEvents lblConfirmation As Label
    Friend WithEvents txtConfirmation As TextBox
    Friend WithEvents btnVoirConfirmation As AltheaBouton
    Friend WithEvents lblMessage As Label
End Class
