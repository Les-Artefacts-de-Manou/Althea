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
        btnAccueil = New AltheaBoutonMenu()
        btnPatients = New AltheaBoutonMenu()
        btnRecherche = New AltheaBoutonMenu()
        btnDomaines = New AltheaBoutonMenu()
        btnAgenda = New AltheaBoutonMenu()
        btnDocuments = New AltheaBoutonMenu()
        btnReferentiels = New AltheaBoutonMenu()
        btnAdmin = New AltheaBoutonMenu()
        btnChangerMotDePasse = New AltheaBoutonMenu()
        stsStatus = New StatusStrip()
        stsLabelStatus = New ToolStripStatusLabel()
        pnlForm = New Panel()
        pnlContent = New Panel()
        pnlMenu = New Panel()
        pnlHeader = New Panel()
        lblUtilisateurConnecte = New AltheaPuceUtilisateur()
        lblContexte = New AltheaFilAriane()
        picTitre = New PictureBox()
        picLogoCabinet = New PictureBox()
        lblCabinet = New Label()
        lblCabinetSeparateur = New Label()
        lblPraticien = New Label()
        CType(errProvider, ComponentModel.ISupportInitialize).BeginInit()
        stsStatus.SuspendLayout()
        pnlForm.SuspendLayout()
        pnlMenu.SuspendLayout()
        pnlHeader.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        CType(picLogoCabinet, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' errProvider
        ' 
        errProvider.ContainerControl = Me
        ' 
        ' btnAccueil
        ' 
        btnAccueil.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnAccueil.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnAccueil.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnAccueil.Image = CType(resources.GetObject("btnAccueil.Image"), Image)
        btnAccueil.Location = New Point(12, 24)
        btnAccueil.Name = "btnAccueil"
        btnAccueil.Padding = New Padding(4, 0, 0, 0)
        btnAccueil.Size = New Size(165, 50)
        btnAccueil.TabIndex = 1
        btnAccueil.Tag = "accueil"
        btnAccueil.Text = "Accueil"
        errProvider.SetIconAlignment(btnAccueil, ErrorIconAlignment.MiddleLeft)
        ' 
        ' btnPatients
        ' 
        btnPatients.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnPatients.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnPatients.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnPatients.Image = CType(resources.GetObject("btnPatients.Image"), Image)
        btnPatients.Location = New Point(12, 78)
        btnPatients.Name = "btnPatients"
        btnPatients.Padding = New Padding(4, 0, 0, 0)
        btnPatients.Size = New Size(165, 50)
        btnPatients.TabIndex = 2
        btnPatients.Tag = "patients"
        btnPatients.Text = "Patients"
        errProvider.SetIconAlignment(btnPatients, ErrorIconAlignment.MiddleLeft)
        ' 
        ' btnRecherche
        ' 
        btnRecherche.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnRecherche.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnRecherche.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnRecherche.Location = New Point(12, 132)
        btnRecherche.Name = "btnRecherche"
        btnRecherche.Padding = New Padding(4, 0, 0, 0)
        btnRecherche.Size = New Size(165, 50)
        btnRecherche.TabIndex = 3
        btnRecherche.Tag = "recherche"
        btnRecherche.Text = "Recherche"
        errProvider.SetIconAlignment(btnRecherche, ErrorIconAlignment.MiddleLeft)
        ' 
        ' btnDomaines
        ' 
        btnDomaines.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnDomaines.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnDomaines.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnDomaines.Image = CType(resources.GetObject("btnDomaines.Image"), Image)
        btnDomaines.Location = New Point(12, 186)
        btnDomaines.Name = "btnDomaines"
        btnDomaines.Padding = New Padding(4, 0, 0, 0)
        btnDomaines.Size = New Size(165, 50)
        btnDomaines.TabIndex = 4
        btnDomaines.Tag = "domaines"
        btnDomaines.Text = "Domaines"
        errProvider.SetIconAlignment(btnDomaines, ErrorIconAlignment.MiddleLeft)
        ' 
        ' btnAgenda
        ' 
        btnAgenda.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnAgenda.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnAgenda.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnAgenda.Image = CType(resources.GetObject("btnAgenda.Image"), Image)
        btnAgenda.Location = New Point(12, 240)
        btnAgenda.Name = "btnAgenda"
        btnAgenda.Padding = New Padding(4, 0, 0, 0)
        btnAgenda.Size = New Size(165, 50)
        btnAgenda.TabIndex = 5
        btnAgenda.Tag = "agenda"
        btnAgenda.Text = "Agenda"
        errProvider.SetIconAlignment(btnAgenda, ErrorIconAlignment.MiddleLeft)
        ' 
        ' btnDocuments
        ' 
        btnDocuments.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnDocuments.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnDocuments.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnDocuments.Image = CType(resources.GetObject("btnDocuments.Image"), Image)
        btnDocuments.Location = New Point(12, 294)
        btnDocuments.Name = "btnDocuments"
        btnDocuments.Padding = New Padding(4, 0, 0, 0)
        btnDocuments.Size = New Size(165, 50)
        btnDocuments.TabIndex = 6
        btnDocuments.Tag = "documents"
        btnDocuments.Text = "Documents"
        errProvider.SetIconAlignment(btnDocuments, ErrorIconAlignment.MiddleLeft)
        ' 
        ' btnReferentiels
        ' 
        btnReferentiels.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnReferentiels.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnReferentiels.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnReferentiels.Image = CType(resources.GetObject("btnReferentiels.Image"), Image)
        btnReferentiels.Location = New Point(12, 348)
        btnReferentiels.Name = "btnReferentiels"
        btnReferentiels.Padding = New Padding(4, 0, 0, 0)
        btnReferentiels.Size = New Size(165, 50)
        btnReferentiels.TabIndex = 7
        btnReferentiels.Tag = "referentiels"
        btnReferentiels.Text = "Référentiels"
        errProvider.SetIconAlignment(btnReferentiels, ErrorIconAlignment.MiddleLeft)
        ' 
        ' btnAdmin
        ' 
        btnAdmin.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnAdmin.Font = New Font("Calibri", 13F, FontStyle.Bold)
        btnAdmin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnAdmin.Image = CType(resources.GetObject("btnAdmin.Image"), Image)
        btnAdmin.Location = New Point(12, 690)
        btnAdmin.Name = "btnAdmin"
        btnAdmin.Padding = New Padding(4, 0, 0, 0)
        btnAdmin.Size = New Size(165, 48)
        btnAdmin.TabIndex = 8
        btnAdmin.Tag = "outils_admin"
        btnAdmin.Text = "Outils/Admin"
        errProvider.SetIconAlignment(btnAdmin, ErrorIconAlignment.MiddleLeft)
        errProvider.SetIconPadding(btnAdmin, 4)
        ' 
        ' btnChangerMotDePasse
        ' 
        btnChangerMotDePasse.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        btnChangerMotDePasse.Font = New Font("Calibri", 10F)
        btnChangerMotDePasse.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        btnChangerMotDePasse.Image = CType(resources.GetObject("btnChangerMotDePasse.Image"), Image)
        btnChangerMotDePasse.Location = New Point(12, 744)
        btnChangerMotDePasse.Name = "btnChangerMotDePasse"
        btnChangerMotDePasse.Padding = New Padding(4, 0, 0, 0)
        btnChangerMotDePasse.Size = New Size(165, 40)
        btnChangerMotDePasse.TabIndex = 6
        btnChangerMotDePasse.TailleIcone = 24
        btnChangerMotDePasse.Text = "Changer mon mot de passe"
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
        pnlForm.BackgroundImage = My.Resources.Resources.Home_Fond_2560x1440
        pnlForm.BackgroundImageLayout = ImageLayout.None
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
        pnlMenu.Controls.Add(btnChangerMotDePasse)
        pnlMenu.Controls.Add(btnAdmin)
        pnlMenu.Controls.Add(btnReferentiels)
        pnlMenu.Controls.Add(btnDocuments)
        pnlMenu.Controls.Add(btnAgenda)
        pnlMenu.Controls.Add(btnDomaines)
        pnlMenu.Controls.Add(btnRecherche)
        pnlMenu.Controls.Add(btnPatients)
        pnlMenu.Controls.Add(btnAccueil)
        pnlMenu.Dock = DockStyle.Left
        pnlMenu.Location = New Point(0, 94)
        pnlMenu.Name = "pnlMenu"
        pnlMenu.Padding = New Padding(8)
        pnlMenu.Size = New Size(189, 835)
        pnlMenu.TabIndex = 1
        ' 
        ' pnlHeader
        ' 
        pnlHeader.BackColor = Color.Transparent
        pnlHeader.Controls.Add(lblUtilisateurConnecte)
        pnlHeader.Controls.Add(lblContexte)
        pnlHeader.Controls.Add(picTitre)
        pnlHeader.Controls.Add(picLogoCabinet)
        pnlHeader.Controls.Add(lblCabinet)
        pnlHeader.Controls.Add(lblCabinetSeparateur)
        pnlHeader.Controls.Add(lblPraticien)
        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Location = New Point(0, 0)
        pnlHeader.Name = "pnlHeader"
        pnlHeader.Size = New Size(1584, 94)
        pnlHeader.TabIndex = 0
        ' 
        ' lblUtilisateurConnecte
        ' 
        lblUtilisateurConnecte.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        lblUtilisateurConnecte.Location = New Point(1316, 24)
        lblUtilisateurConnecte.Name = "lblUtilisateurConnecte"
        lblUtilisateurConnecte.Size = New Size(240, 46)
        lblUtilisateurConnecte.TabIndex = 5
        ' 
        ' lblContexte
        ' 
        lblContexte.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblContexte.Font = New Font("Calibri", 11F)
        lblContexte.Location = New Point(300, 52)
        lblContexte.Name = "lblContexte"
        lblContexte.Size = New Size(970, 28)
        lblContexte.TabIndex = 4
        lblContexte.Text = "Accueil"
        ' 
        ' picTitre
        ' 
        picTitre.BackgroundImage = My.Resources.Resources.Home_Logo_Althea_253x70
        picTitre.BackgroundImageLayout = ImageLayout.Center
        picTitre.Location = New Point(14, 12)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(253, 70)
        picTitre.TabIndex = 3
        picTitre.TabStop = False
        ' 
        ' picLogoCabinet
        ' 
        picLogoCabinet.BackColor = Color.Transparent
        picLogoCabinet.Location = New Point(300, 14)
        picLogoCabinet.Name = "picLogoCabinet"
        picLogoCabinet.Size = New Size(30, 30)
        picLogoCabinet.SizeMode = PictureBoxSizeMode.Zoom
        picLogoCabinet.TabIndex = 6
        picLogoCabinet.TabStop = False
        picLogoCabinet.Visible = False
        ' 
        ' lblCabinet
        ' 
        lblCabinet.AutoSize = True
        lblCabinet.BackColor = Color.Transparent
        lblCabinet.Font = New Font("Calibri", 12F, FontStyle.Bold)
        lblCabinet.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        lblCabinet.Location = New Point(338, 18)
        lblCabinet.Name = "lblCabinet"
        lblCabinet.Size = New Size(0, 19)
        lblCabinet.TabIndex = 7
        ' 
        ' lblCabinetSeparateur
        ' 
        lblCabinetSeparateur.AutoSize = True
        lblCabinetSeparateur.BackColor = Color.Transparent
        lblCabinetSeparateur.Font = New Font("Calibri", 12F)
        lblCabinetSeparateur.ForeColor = Color.FromArgb(CByte(218), CByte(201), CByte(184))
        lblCabinetSeparateur.Location = New Point(500, 18)
        lblCabinetSeparateur.Name = "lblCabinetSeparateur"
        lblCabinetSeparateur.Size = New Size(12, 19)
        lblCabinetSeparateur.TabIndex = 8
        lblCabinetSeparateur.Text = "|"
        lblCabinetSeparateur.Visible = False
        ' 
        ' lblPraticien
        ' 
        lblPraticien.AutoSize = True
        lblPraticien.BackColor = Color.Transparent
        lblPraticien.Font = New Font("Calibri", 11F)
        lblPraticien.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPraticien.Location = New Point(520, 19)
        lblPraticien.Name = "lblPraticien"
        lblPraticien.Size = New Size(0, 18)
        lblPraticien.TabIndex = 9
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
        CType(picLogoCabinet, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents ttMain As ToolTip
    Friend WithEvents errProvider As ErrorProvider
    Friend WithEvents stsStatus As StatusStrip
    Friend WithEvents stsLabelStatus As ToolStripStatusLabel
    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlHeader As Panel
    Friend WithEvents picTitre As PictureBox
    Friend WithEvents picLogoCabinet As PictureBox
    Friend WithEvents lblCabinet As Label
    Friend WithEvents lblCabinetSeparateur As Label
    Friend WithEvents lblPraticien As Label
    Friend WithEvents pnlMenu As Panel
    Friend WithEvents btnAccueil As AltheaBoutonMenu
    Friend WithEvents btnPatients As AltheaBoutonMenu
    Friend WithEvents btnRecherche As AltheaBoutonMenu
    Friend WithEvents btnAgenda As AltheaBoutonMenu
    Friend WithEvents btnDomaines As AltheaBoutonMenu
    Friend WithEvents btnDocuments As AltheaBoutonMenu
    Friend WithEvents btnAdmin As AltheaBoutonMenu
    Friend WithEvents btnReferentiels As AltheaBoutonMenu
    Friend WithEvents pnlContent As Panel
    Friend WithEvents lblContexte As AltheaFilAriane
    Friend WithEvents lblUtilisateurConnecte As AltheaPuceUtilisateur
    Friend WithEvents btnChangerMotDePasse As AltheaBoutonMenu

End Class
