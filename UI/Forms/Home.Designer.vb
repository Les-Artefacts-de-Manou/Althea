<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Home
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Home))
        ttMain = New ToolTip(components)
        errProvider = New ErrorProvider(components)
        btnAccueil = New Button()
        btnPatients = New Button()
        btnDomaines = New Button()
        btnAgenda = New Button()
        btnDocuments = New Button()
        btnReferentiels = New Button()
        btnAdmin = New Button()
        btnChangerMotDePasse = New Button()
        stsStatus = New StatusStrip()
        stsLabelStatus = New ToolStripStatusLabel()
        pnlForm = New Panel()
        pnlContent = New Panel()
        pnlMenu = New Panel()
        Button2 = New Button()
        Button1 = New Button()
        butTempHashPW = New Button()
        pnlHeader = New Panel()
        lblUtilisateurConnecte = New Label()
        lblContexte = New Label()
        picTitre = New PictureBox()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        stsStatus.SuspendLayout()
        pnlForm.SuspendLayout()
        pnlMenu.SuspendLayout()
        pnlHeader.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' btnAccueil
        ' 
        btnAccueil.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnAccueil.BackgroundImageLayout = ImageLayout.None
        btnAccueil.FlatAppearance.BorderSize = 0
        btnAccueil.FlatStyle = FlatStyle.Flat
        btnAccueil.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnAccueil.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        errProvider.SetIconAlignment(btnAccueil, ErrorIconAlignment.MiddleLeft)
        btnAccueil.Image = CType(resources.GetObject("btnAccueil.Image"), Image)
        btnAccueil.ImageAlign = ContentAlignment.MiddleLeft
        btnAccueil.Location = New Point(10, 40)
        btnAccueil.Name = "btnAccueil"
        btnAccueil.Padding = New Padding(10, 0, 0, 0)
        btnAccueil.Size = New Size(169, 60)
        btnAccueil.TabIndex = 1
        btnAccueil.Tag = "accueil"
        btnAccueil.Text = "Accueil"
        btnAccueil.TextAlign = ContentAlignment.MiddleLeft
        btnAccueil.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAccueil.UseVisualStyleBackColor = False
        ' 
        ' btnPatients
        ' 
        btnPatients.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnPatients.BackgroundImageLayout = ImageLayout.Stretch
        btnPatients.FlatAppearance.BorderSize = 0
        btnPatients.FlatStyle = FlatStyle.Flat
        btnPatients.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnPatients.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        errProvider.SetIconAlignment(btnPatients, ErrorIconAlignment.MiddleLeft)
        btnPatients.Image = CType(resources.GetObject("btnPatients.Image"), Image)
        btnPatients.ImageAlign = ContentAlignment.MiddleLeft
        btnPatients.Location = New Point(10, 100)
        btnPatients.Name = "btnPatients"
        btnPatients.Padding = New Padding(10, 0, 0, 0)
        btnPatients.Size = New Size(169, 60)
        btnPatients.TabIndex = 2
        btnPatients.Tag = "patients"
        btnPatients.Text = "Patients"
        btnPatients.TextAlign = ContentAlignment.MiddleLeft
        btnPatients.TextImageRelation = TextImageRelation.ImageBeforeText
        btnPatients.UseVisualStyleBackColor = False
        ' 
        ' btnDomaines
        ' 
        btnDomaines.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnDomaines.BackgroundImageLayout = ImageLayout.Stretch
        btnDomaines.FlatAppearance.BorderSize = 0
        btnDomaines.FlatStyle = FlatStyle.Flat
        btnDomaines.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnDomaines.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        errProvider.SetIconAlignment(btnDomaines, ErrorIconAlignment.MiddleLeft)
        btnDomaines.Image = CType(resources.GetObject("btnDomaines.Image"), Image)
        btnDomaines.ImageAlign = ContentAlignment.MiddleLeft
        btnDomaines.Location = New Point(10, 160)
        btnDomaines.Name = "btnDomaines"
        btnDomaines.Padding = New Padding(10, 0, 0, 0)
        btnDomaines.Size = New Size(169, 60)
        btnDomaines.TabIndex = 3
        btnDomaines.Tag = "domaines"
        btnDomaines.Text = "Domaines"
        btnDomaines.TextAlign = ContentAlignment.MiddleLeft
        btnDomaines.TextImageRelation = TextImageRelation.ImageBeforeText
        btnDomaines.UseVisualStyleBackColor = False
        ' 
        ' btnAgenda
        ' 
        btnAgenda.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnAgenda.BackgroundImageLayout = ImageLayout.Stretch
        btnAgenda.FlatAppearance.BorderSize = 0
        btnAgenda.FlatStyle = FlatStyle.Flat
        btnAgenda.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnAgenda.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        errProvider.SetIconAlignment(btnAgenda, ErrorIconAlignment.MiddleLeft)
        btnAgenda.Image = CType(resources.GetObject("btnAgenda.Image"), Image)
        btnAgenda.ImageAlign = ContentAlignment.MiddleLeft
        btnAgenda.Location = New Point(10, 220)
        btnAgenda.Name = "btnAgenda"
        btnAgenda.Padding = New Padding(10, 0, 0, 0)
        btnAgenda.Size = New Size(169, 60)
        btnAgenda.TabIndex = 4
        btnAgenda.Tag = "agenda"
        btnAgenda.Text = "Agenda"
        btnAgenda.TextAlign = ContentAlignment.MiddleLeft
        btnAgenda.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAgenda.UseVisualStyleBackColor = False
        ' 
        ' btnDocuments
        ' 
        btnDocuments.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnDocuments.BackgroundImageLayout = ImageLayout.Stretch
        btnDocuments.FlatAppearance.BorderSize = 0
        btnDocuments.FlatStyle = FlatStyle.Flat
        btnDocuments.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnDocuments.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        errProvider.SetIconAlignment(btnDocuments, ErrorIconAlignment.MiddleLeft)
        btnDocuments.Image = CType(resources.GetObject("btnDocuments.Image"), Image)
        btnDocuments.ImageAlign = ContentAlignment.MiddleLeft
        btnDocuments.Location = New Point(10, 280)
        btnDocuments.Name = "btnDocuments"
        btnDocuments.Padding = New Padding(10, 0, 0, 0)
        btnDocuments.Size = New Size(169, 60)
        btnDocuments.TabIndex = 5
        btnDocuments.Tag = "documents"
        btnDocuments.Text = "Documents"
        btnDocuments.TextAlign = ContentAlignment.MiddleLeft
        btnDocuments.TextImageRelation = TextImageRelation.ImageBeforeText
        btnDocuments.UseVisualStyleBackColor = False
        ' 
        ' btnReferentiels
        ' 
        btnReferentiels.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnReferentiels.BackgroundImageLayout = ImageLayout.Stretch
        btnReferentiels.FlatAppearance.BorderSize = 0
        btnReferentiels.FlatStyle = FlatStyle.Flat
        btnReferentiels.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnReferentiels.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        errProvider.SetIconAlignment(btnReferentiels, ErrorIconAlignment.MiddleLeft)
        btnReferentiels.Image = CType(resources.GetObject("btnReferentiels.Image"), Image)
        btnReferentiels.ImageAlign = ContentAlignment.MiddleLeft
        btnReferentiels.Location = New Point(10, 340)
        btnReferentiels.Name = "btnReferentiels"
        btnReferentiels.Padding = New Padding(10, 0, 0, 0)
        btnReferentiels.Size = New Size(169, 60)
        btnReferentiels.TabIndex = 6
        btnReferentiels.Tag = "referentiels"
        btnReferentiels.Text = "Référentiels"
        btnReferentiels.TextAlign = ContentAlignment.MiddleLeft
        btnReferentiels.TextImageRelation = TextImageRelation.ImageBeforeText
        btnReferentiels.UseVisualStyleBackColor = False
        ' 
        ' btnAdmin
        ' 
        btnAdmin.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnAdmin.BackgroundImageLayout = ImageLayout.Stretch
        btnAdmin.FlatAppearance.BorderSize = 0
        btnAdmin.FlatStyle = FlatStyle.Flat
        btnAdmin.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnAdmin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        errProvider.SetIconAlignment(btnAdmin, ErrorIconAlignment.MiddleLeft)
        errProvider.SetIconPadding(btnAdmin, 4)
        btnAdmin.Image = CType(resources.GetObject("btnAdmin.Image"), Image)
        btnAdmin.ImageAlign = ContentAlignment.MiddleLeft
        btnAdmin.Location = New Point(8, 674)
        btnAdmin.Name = "btnAdmin"
        btnAdmin.Padding = New Padding(4, 0, 0, 0)
        btnAdmin.Size = New Size(175, 60)
        btnAdmin.TabIndex = 8
        btnAdmin.Tag = "outils_admin"
        btnAdmin.Text = "Outils/Admin"
        btnAdmin.TextAlign = ContentAlignment.MiddleLeft
        btnAdmin.TextImageRelation = TextImageRelation.ImageBeforeText
        btnAdmin.UseVisualStyleBackColor = False
        ' 
        ' btnChangerMotDePasse
        ' 
        btnChangerMotDePasse.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnChangerMotDePasse.FlatAppearance.BorderSize = 0
        btnChangerMotDePasse.FlatStyle = FlatStyle.Flat
        btnChangerMotDePasse.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnChangerMotDePasse.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnChangerMotDePasse.Image = CType(resources.GetObject("btnChangerMotDePasse.Image"), Image)
        btnChangerMotDePasse.ImageAlign = ContentAlignment.MiddleLeft
        btnChangerMotDePasse.Location = New Point(8, 752)
        btnChangerMotDePasse.Name = "btnChangerMotDePasse"
        btnChangerMotDePasse.Size = New Size(175, 60)
        btnChangerMotDePasse.TabIndex = 6
        btnChangerMotDePasse.Text = "Changer mon password"
        btnChangerMotDePasse.TextAlign = ContentAlignment.MiddleLeft
        btnChangerMotDePasse.TextImageRelation = TextImageRelation.ImageBeforeText
        btnChangerMotDePasse.UseVisualStyleBackColor = False
        ' 
        ' stsStatus
        ' 
        stsStatus.AllowItemReorder = True
        stsStatus.AutoSize = False
        stsStatus.BackColor = Color.FromArgb(CByte(218), CByte(201), CByte(184))
        stsStatus.Items.AddRange(New ToolStripItem() {stsLabelStatus})
        stsStatus.Location = New Point(0, 929)
        stsStatus.Name = "stsStatus"
        stsStatus.Padding = New Padding(1, 0, 12, 0)
        stsStatus.Size = New Size(1584, 32)
        stsStatus.TabIndex = 17
        stsStatus.Text = "StatusStrip1"
        ' 
        ' stsLabelStatus
        ' 
        stsLabelStatus.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        stsLabelStatus.Font = New Font("Calibri", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        stsLabelStatus.ForeColor = Color.FromArgb(CByte(11), CByte(95), CByte(125))
        stsLabelStatus.Name = "stsLabelStatus"
        stsLabelStatus.Size = New Size(43, 27)
        stsLabelStatus.Text = "Althea"
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        pnlForm.BackgroundImage = My.Resources.Resources.Fond_1000x800_Feuille4
        pnlForm.BackgroundImageLayout = ImageLayout.Stretch
        pnlForm.Controls.Add(pnlContent)
        pnlForm.Controls.Add(pnlMenu)
        pnlForm.Controls.Add(pnlHeader)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Font = New Font("Calibri", 11F)
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Size = New Size(1584, 929)
        pnlForm.TabIndex = 18
        ' 
        ' pnlContent
        ' 
        pnlContent.AllowDrop = True
        pnlContent.BackColor = Color.Transparent
        pnlContent.Dock = DockStyle.Fill
        pnlContent.Location = New Point(189, 94)
        pnlContent.Name = "pnlContent"
        pnlContent.Padding = New Padding(8)
        pnlContent.Size = New Size(1395, 835)
        pnlContent.TabIndex = 3
        ' 
        ' pnlMenu
        ' 
        pnlMenu.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        pnlMenu.Controls.Add(Button2)
        pnlMenu.Controls.Add(Button1)
        pnlMenu.Controls.Add(btnChangerMotDePasse)
        pnlMenu.Controls.Add(butTempHashPW)
        pnlMenu.Controls.Add(btnAdmin)
        pnlMenu.Controls.Add(btnReferentiels)
        pnlMenu.Controls.Add(btnDocuments)
        pnlMenu.Controls.Add(btnAgenda)
        pnlMenu.Controls.Add(btnDomaines)
        pnlMenu.Controls.Add(btnPatients)
        pnlMenu.Controls.Add(btnAccueil)
        pnlMenu.Dock = DockStyle.Left
        pnlMenu.Location = New Point(0, 94)
        pnlMenu.Name = "pnlMenu"
        pnlMenu.Padding = New Padding(8)
        pnlMenu.Size = New Size(189, 835)
        pnlMenu.TabIndex = 1
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(11, 588)
        Button2.Name = "Button2"
        Button2.Size = New Size(166, 29)
        Button2.TabIndex = 12
        Button2.Text = "Test RichTextEditor"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(11, 553)
        Button1.Name = "Button1"
        Button1.Size = New Size(166, 29)
        Button1.TabIndex = 10
        Button1.Text = "Test DialogChoix"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' butTempHashPW
        ' 
        butTempHashPW.Location = New Point(11, 518)
        butTempHashPW.Name = "butTempHashPW"
        butTempHashPW.Size = New Size(166, 29)
        butTempHashPW.TabIndex = 9
        butTempHashPW.Text = "Temp Hash PW"
        butTempHashPW.UseVisualStyleBackColor = True
        ' 
        ' pnlHeader
        ' 
        pnlHeader.BackColor = Color.Transparent
        pnlHeader.Controls.Add(lblUtilisateurConnecte)
        pnlHeader.Controls.Add(lblContexte)
        pnlHeader.Controls.Add(picTitre)
        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Location = New Point(0, 0)
        pnlHeader.Name = "pnlHeader"
        pnlHeader.Size = New Size(1584, 94)
        pnlHeader.TabIndex = 0
        ' 
        ' lblUtilisateurConnecte
        ' 
        lblUtilisateurConnecte.BorderStyle = BorderStyle.Fixed3D
        lblUtilisateurConnecte.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblUtilisateurConnecte.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblUtilisateurConnecte.Location = New Point(325, 9)
        lblUtilisateurConnecte.Name = "lblUtilisateurConnecte"
        lblUtilisateurConnecte.Padding = New Padding(6)
        lblUtilisateurConnecte.Size = New Size(600, 32)
        lblUtilisateurConnecte.TabIndex = 5
        lblUtilisateurConnecte.Text = "Connecté : -"
        ' 
        ' lblContexte
        ' 
        lblContexte.BorderStyle = BorderStyle.Fixed3D
        lblContexte.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblContexte.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblContexte.Location = New Point(325, 50)
        lblContexte.Name = "lblContexte"
        lblContexte.Padding = New Padding(6)
        lblContexte.Size = New Size(600, 32)
        lblContexte.TabIndex = 4
        lblContexte.Text = "Contexte courant ou nom du patient/dossier"
        ' 
        ' picTitre
        ' 
        picTitre.BackgroundImage = CType(resources.GetObject("picTitre.BackgroundImage"), Image)
        picTitre.BackgroundImageLayout = ImageLayout.Stretch
        picTitre.Location = New Point(56, 12)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(191, 48)
        picTitre.TabIndex = 3
        picTitre.TabStop = False
        ' 
        ' Home
        ' 
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1584, 961)
        Controls.Add(pnlForm)
        Controls.Add(stsStatus)
        Font = New Font("Calibri", 9F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MinimizeBox = False
        MinimumSize = New Size(945, 703)
        Name = "Home"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Althéa"
        CType(errProvider, ComponentModel.ISupportInitialize).EndInit()
        stsStatus.ResumeLayout(False)
        stsStatus.PerformLayout()
        pnlForm.ResumeLayout(False)
        pnlMenu.ResumeLayout(False)
        pnlHeader.ResumeLayout(False)
        CType(picTitre, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents stsStatus As StatusStrip
    Friend WithEvents stsLabelStatus As ToolStripStatusLabel
    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlHeader As Panel
    Friend WithEvents picTitre As PictureBox
    Friend WithEvents pnlMenu As Panel
    Friend WithEvents btnAccueil As Button
    Friend WithEvents btnPatients As Button
    Friend WithEvents btnAgenda As Button
    Friend WithEvents btnDomaines As Button
    Friend WithEvents btnDocuments As Button
    Friend WithEvents btnAdmin As Button
    Friend WithEvents btnReferentiels As Button
    Friend WithEvents pnlContent As Panel
    Friend WithEvents lblContexte As Label
    Friend WithEvents lblUtilisateurConnecte As Label
    Friend WithEvents btnChangerMotDePasse As Button
    Friend WithEvents butTempHashPW As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button

End Class
