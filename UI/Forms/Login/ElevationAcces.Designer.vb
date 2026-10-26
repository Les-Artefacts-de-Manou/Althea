<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ElevationAcces
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

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ElevationAcces))
        enTete = New AltheaEnTete()
        carteElevation = New AltheaCarte()
        lblRole = New Label()
        cboRoleDemande = New AltheaListe()
        lblPassword = New Label()
        txtPassword = New TextBox()
        btnVoirPassword = New AltheaBouton()
        lblMessage = New Label()
        pnlActions = New AltheaBarreActions()
        btnAnnuler = New AltheaBouton()
        btnValider = New AltheaBouton()
        errProvider = New ErrorProvider(components)
        carteElevation.SuspendLayout()
        pnlActions.SuspendLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(466, 62)
        enTete.TabIndex = 20
        enTete.SousTitre = "Confirmez votre identité pour une action protégée"
        enTete.Text = "Élévation d’accès"
        ' 
        ' carteElevation
        ' 
        carteElevation.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        carteElevation.Controls.Add(lblRole)
        carteElevation.Controls.Add(cboRoleDemande)
        carteElevation.Controls.Add(lblPassword)
        carteElevation.Controls.Add(txtPassword)
        carteElevation.Controls.Add(btnVoirPassword)
        carteElevation.Controls.Add(lblMessage)
        carteElevation.Location = New Point(12, 70)
        carteElevation.Name = "carteElevation"
        carteElevation.Padding = New Padding(12, 8, 12, 10)
        carteElevation.Size = New Size(442, 216)
        carteElevation.TabIndex = 0
        carteElevation.TabStop = False
        carteElevation.Titre = "Accès protégé requis"
        ' 
        ' lblRole
        ' 
        lblRole.AutoSize = True
        lblRole.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblRole.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRole.Location = New Point(14, 44)
        lblRole.Name = "lblRole"
        lblRole.Size = New Size(32, 17)
        lblRole.TabIndex = 0
        lblRole.Text = "Rôle"
        ' 
        ' cboRoleDemande
        ' 
        cboRoleDemande.Font = New Font("Calibri", 11F)
        cboRoleDemande.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboRoleDemande.ItemHeight = 20
        cboRoleDemande.Location = New Point(130, 40)
        cboRoleDemande.Name = "cboRoleDemande"
        cboRoleDemande.Size = New Size(230, 26)
        cboRoleDemande.TabIndex = 1
        ' 
        ' lblPassword
        ' 
        lblPassword.AutoSize = True
        lblPassword.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPassword.Location = New Point(14, 78)
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
        txtPassword.Location = New Point(130, 74)
        txtPassword.Name = "txtPassword"
        txtPassword.Size = New Size(230, 25)
        txtPassword.TabIndex = 3
        txtPassword.UseSystemPasswordChar = True
        ' 
        ' btnVoirPassword
        ' 
        btnVoirPassword.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnVoirPassword.Image = CType(resources.GetObject("btnVoirPassword.Image"), Image)
        btnVoirPassword.Location = New Point(364, 72)
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
        lblMessage.Size = New Size(414, 80)
        lblMessage.TabIndex = 15
        lblMessage.Text = "Message"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnValider)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 294)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(466, 47)
        pnlActions.TabIndex = 22
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(336, 7)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 33)
        btnAnnuler.TabIndex = 5
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' btnValider
        ' 
        btnValider.Image = CType(resources.GetObject("btnValider.Image"), Image)
        btnValider.Location = New Point(218, 7)
        btnValider.Name = "btnValider"
        btnValider.Size = New Size(108, 33)
        btnValider.TabIndex = 4
        btnValider.Tag = "valider_normal"
        btnValider.Text = "Valider"
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' ElevationAcces
        ' 
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(466, 341)
        Controls.Add(carteElevation)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        Font = New Font("Calibri", 9F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MinimumSize = New Size(480, 380)
        Name = "ElevationAcces"
        StartPosition = FormStartPosition.CenterParent
        Text = "Elevation Acces"
        carteElevation.ResumeLayout(False)
        carteElevation.PerformLayout()
        pnlActions.ResumeLayout(False)
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteElevation As AltheaCarte
    Friend WithEvents lblRole As Label
    Friend WithEvents cboRoleDemande As AltheaListe
    Friend WithEvents lblPassword As Label
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents btnVoirPassword As AltheaBouton
    Friend WithEvents lblMessage As Label
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents btnValider As AltheaBouton
    Friend WithEvents errProvider As ErrorProvider
End Class
