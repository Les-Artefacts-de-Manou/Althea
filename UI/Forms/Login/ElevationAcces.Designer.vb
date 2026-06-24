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
        pnlForm = New Panel()
        pnlCenter = New Panel()
        tlpElevation = New TableLayoutPanel()
        lblPassword = New Label()
        txtPassword = New TextBox()
        btnVoirPassword = New Button()
        lblMessage = New Label()
        cboRoleDemande = New ComboBox()
        lblRole = New Label()
        pnlActions = New Panel()
        btnAnnuler = New Button()
        btnValider = New Button()
        pnlTop = New Panel()
        lblTop = New Label()
        pnlTitre = New Panel()
        lblTitreForm = New Label()
        errProvider = New ErrorProvider(components)
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        tlpElevation.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTop.SuspendLayout()
        pnlTitre.SuspendLayout()
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
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Size = New Size(466, 341)
        pnlForm.TabIndex = 1
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(tlpElevation)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(0, 86)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(4, 6, 4, 6)
        pnlCenter.Size = New Size(466, 208)
        pnlCenter.TabIndex = 23
        ' 
        ' tlpElevation
        ' 
        tlpElevation.ColumnCount = 3
        tlpElevation.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30F))
        tlpElevation.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        tlpElevation.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20F))
        tlpElevation.Controls.Add(lblPassword, 0, 1)
        tlpElevation.Controls.Add(txtPassword, 1, 1)
        tlpElevation.Controls.Add(btnVoirPassword, 2, 1)
        tlpElevation.Controls.Add(lblMessage, 0, 2)
        tlpElevation.Controls.Add(cboRoleDemande, 1, 0)
        tlpElevation.Controls.Add(lblRole, 0, 0)
        tlpElevation.Dock = DockStyle.Fill
        tlpElevation.Location = New Point(4, 6)
        tlpElevation.Name = "tlpElevation"
        tlpElevation.Padding = New Padding(10, 11, 10, 11)
        tlpElevation.RowCount = 3
        tlpElevation.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tlpElevation.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tlpElevation.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tlpElevation.Size = New Size(458, 196)
        tlpElevation.TabIndex = 0
        ' 
        ' lblPassword
        ' 
        lblPassword.AutoSize = True
        lblPassword.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPassword.Location = New Point(13, 69)
        lblPassword.Name = "lblPassword"
        lblPassword.Size = New Size(81, 14)
        lblPassword.TabIndex = 2
        lblPassword.Text = "Mot de passe"
        ' 
        ' txtPassword
        ' 
        txtPassword.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtPassword.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtPassword.Location = New Point(144, 72)
        txtPassword.Name = "txtPassword"
        txtPassword.Size = New Size(212, 22)
        txtPassword.TabIndex = 2
        txtPassword.UseSystemPasswordChar = True
        ' 
        ' btnVoirPassword
        ' 
        btnVoirPassword.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnVoirPassword.Image = CType(resources.GetObject("btnVoirPassword.Image"), Image)
        btnVoirPassword.Location = New Point(363, 72)
        btnVoirPassword.Name = "btnVoirPassword"
        btnVoirPassword.Size = New Size(33, 35)
        btnVoirPassword.TabIndex = 3
        btnVoirPassword.Tag = "voir_normal"
        btnVoirPassword.TextImageRelation = TextImageRelation.ImageAboveText
        btnVoirPassword.UseVisualStyleBackColor = False
        ' 
        ' lblMessage
        ' 
        tlpElevation.SetColumnSpan(lblMessage, 3)
        lblMessage.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblMessage.ForeColor = Color.FromArgb(CByte(194), CByte(106), CByte(118))
        lblMessage.Location = New Point(13, 127)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(430, 37)
        lblMessage.TabIndex = 15
        lblMessage.Text = "Message"
        ' 
        ' cboRoleDemande
        ' 
        cboRoleDemande.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        cboRoleDemande.DropDownStyle = ComboBoxStyle.DropDownList
        cboRoleDemande.FlatStyle = FlatStyle.Flat
        cboRoleDemande.Font = New Font("Calibri", 9F)
        cboRoleDemande.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboRoleDemande.FormattingEnabled = True
        cboRoleDemande.Location = New Point(144, 14)
        cboRoleDemande.Name = "cboRoleDemande"
        cboRoleDemande.Size = New Size(195, 22)
        cboRoleDemande.TabIndex = 1
        ' 
        ' lblRole
        ' 
        lblRole.AutoSize = True
        lblRole.Font = New Font("Calibri", 9F)
        lblRole.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblRole.Location = New Point(13, 11)
        lblRole.Name = "lblRole"
        lblRole.Size = New Size(32, 14)
        lblRole.TabIndex = 17
        lblRole.Text = "Rôle"
        ' 
        ' pnlActions
        ' 
        pnlActions.BackColor = Color.FromArgb(CByte(231), CByte(223), CByte(214))
        pnlActions.BackgroundImage = CType(resources.GetObject("pnlActions.BackgroundImage"), Image)
        pnlActions.BackgroundImageLayout = ImageLayout.Stretch
        pnlActions.BorderStyle = BorderStyle.FixedSingle
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnValider)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(0, 294)
        pnlActions.Name = "pnlActions"
        pnlActions.Padding = New Padding(6, 7, 6, 7)
        pnlActions.Size = New Size(466, 47)
        pnlActions.TabIndex = 22
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnAnnuler.FlatAppearance.BorderSize = 0
        btnAnnuler.FlatStyle = FlatStyle.Flat
        btnAnnuler.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnAnnuler.ForeColor = Color.White
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.ImageAlign = ContentAlignment.MiddleLeft
        btnAnnuler.Location = New Point(253, 6)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(107, 35)
        btnAnnuler.TabIndex = 5
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
        btnValider.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnValider.ForeColor = Color.White
        btnValider.Image = CType(resources.GetObject("btnValider.Image"), Image)
        btnValider.ImageAlign = ContentAlignment.MiddleLeft
        btnValider.Location = New Point(151, 6)
        btnValider.Name = "btnValider"
        btnValider.Size = New Size(96, 35)
        btnValider.TabIndex = 4
        btnValider.Tag = "valider_normal"
        btnValider.Text = "Valider"
        btnValider.TextImageRelation = TextImageRelation.ImageBeforeText
        btnValider.UseVisualStyleBackColor = False
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.Transparent
        pnlTop.Controls.Add(lblTop)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(0, 50)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(6, 7, 6, 7)
        pnlTop.Size = New Size(466, 36)
        pnlTop.TabIndex = 21
        ' 
        ' lblTop
        ' 
        lblTop.Dock = DockStyle.Fill
        lblTop.Font = New Font("Calibri", 10F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(6, 7)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(454, 22)
        lblTop.TabIndex = 0
        lblTop.Text = "Accès protégé requis :"
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(0, 0)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(466, 50)
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
        lblTitreForm.Padding = New Padding(6, 3, 6, 3)
        lblTitreForm.Size = New Size(165, 30)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Élévation d’accès"
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' ElevationAcces
        ' 
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(466, 341)
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 9F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MinimumSize = New Size(480, 380)
        Name = "ElevationAcces"
        StartPosition = FormStartPosition.CenterParent
        Text = "Elevation Acces"
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        tlpElevation.ResumeLayout(False)
        tlpElevation.PerformLayout()
        pnlActions.ResumeLayout(False)
        pnlTop.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents tlpElevation As TableLayoutPanel
    Friend WithEvents lblUserName As Label
    Friend WithEvents lblPassword As Label
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents btnVoirPassword As Button
    Friend WithEvents lblMessage As Label
    Friend WithEvents pnlActions As Panel
    Friend WithEvents btnAnnuler As Button
    Friend WithEvents btnValider As Button
    Friend WithEvents pnlTop As Panel
    Friend WithEvents lblTop As Label
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents cboRoleDemande As ComboBox
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents lblRole As Label
End Class
