<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_AdminHome
    Inherits System.Windows.Forms.UserControl

    'UserControl remplace la méthode Dispose pour nettoyer la liste des composants.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_AdminHome))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        tblMenu = New TableLayoutPanel()
        btnParametres = New Button()
        btnUtilisateurs = New Button()
        btnLogs = New Button()
        btnSauvegardes = New Button()
        btnConnexionDatabase = New Button()
        pnlRoleCourant = New Panel()
        btnEleverAcces = New Button()
        lblRoleCourant = New Label()
        btnRetourRoleBase = New Button()
        lblElevation = New Label()
        pnlTop = New Panel()
        lblTop = New Label()
        pnlTitre = New Panel()
        lblTitreForm = New Label()
        picTitre = New PictureBox()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        tblMenu.SuspendLayout()
        pnlRoleCourant.SuspendLayout()
        pnlTop.SuspendLayout()
        pnlTitre.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackgroundImage = CType(resources.GetObject("pnlForm.BackgroundImage"), Image)
        pnlForm.BackgroundImageLayout = ImageLayout.Stretch
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(pnlTitre)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16, 16, 16, 2)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 1
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(tblMenu)
        pnlCenter.Controls.Add(pnlRoleCourant)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16)
        pnlCenter.Size = New Size(965, 695)
        pnlCenter.TabIndex = 21
        ' 
        ' tblMenu
        ' 
        tblMenu.ColumnCount = 2
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        tblMenu.Controls.Add(btnParametres, 0, 0)
        tblMenu.Controls.Add(btnUtilisateurs, 1, 0)
        tblMenu.Controls.Add(btnLogs, 0, 2)
        tblMenu.Controls.Add(btnSauvegardes, 0, 1)
        tblMenu.Controls.Add(btnConnexionDatabase, 1, 1)
        tblMenu.Dock = DockStyle.Fill
        tblMenu.Location = New Point(16, 146)
        tblMenu.Name = "tblMenu"
        tblMenu.RowCount = 3
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tblMenu.Size = New Size(933, 533)
        tblMenu.TabIndex = 10
        ' 
        ' btnParametres
        ' 
        btnParametres.Anchor = AnchorStyles.None
        btnParametres.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnParametres.BackgroundImageLayout = ImageLayout.Center
        btnParametres.FlatAppearance.BorderSize = 0
        btnParametres.FlatStyle = FlatStyle.Flat
        btnParametres.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnParametres.ForeColor = Color.White
        btnParametres.Image = CType(resources.GetObject("btnParametres.Image"), Image)
        btnParametres.Location = New Point(115, 43)
        btnParametres.Name = "btnParametres"
        btnParametres.Size = New Size(235, 90)
        btnParametres.TabIndex = 9
        btnParametres.Tag = "parametres_normal"
        btnParametres.Text = "Paramètres" & vbCrLf & "Chemins, stockage, options"
        btnParametres.TextImageRelation = TextImageRelation.ImageBeforeText
        btnParametres.UseCompatibleTextRendering = True
        btnParametres.UseVisualStyleBackColor = False
        ' 
        ' btnUtilisateurs
        ' 
        btnUtilisateurs.Anchor = AnchorStyles.None
        btnUtilisateurs.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnUtilisateurs.BackgroundImageLayout = ImageLayout.Center
        btnUtilisateurs.FlatAppearance.BorderSize = 0
        btnUtilisateurs.FlatStyle = FlatStyle.Flat
        btnUtilisateurs.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnUtilisateurs.ForeColor = Color.White
        btnUtilisateurs.Image = CType(resources.GetObject("btnUtilisateurs.Image"), Image)
        btnUtilisateurs.Location = New Point(582, 43)
        btnUtilisateurs.Name = "btnUtilisateurs"
        btnUtilisateurs.Size = New Size(235, 90)
        btnUtilisateurs.TabIndex = 12
        btnUtilisateurs.Tag = "tester_connexion_normal"
        btnUtilisateurs.Text = "Utilisateurs" & vbCrLf & "Gestion, Rôles"
        btnUtilisateurs.TextImageRelation = TextImageRelation.ImageBeforeText
        btnUtilisateurs.UseVisualStyleBackColor = True
        ' 
        ' btnLogs
        ' 
        btnLogs.Anchor = AnchorStyles.None
        btnLogs.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnLogs.BackgroundImageLayout = ImageLayout.Center
        btnLogs.FlatAppearance.BorderSize = 0
        btnLogs.FlatStyle = FlatStyle.Flat
        btnLogs.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnLogs.ForeColor = Color.White
        btnLogs.Image = CType(resources.GetObject("btnLogs.Image"), Image)
        btnLogs.Location = New Point(115, 398)
        btnLogs.Name = "btnLogs"
        btnLogs.Size = New Size(235, 90)
        btnLogs.TabIndex = 10
        btnLogs.Tag = "logs_normal"
        btnLogs.Text = "Logs" & vbCrLf & "Consulter les journaux"
        btnLogs.TextImageRelation = TextImageRelation.ImageBeforeText
        btnLogs.UseVisualStyleBackColor = False
        ' 
        ' btnSauvegardes
        ' 
        btnSauvegardes.Anchor = AnchorStyles.None
        btnSauvegardes.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnSauvegardes.BackgroundImageLayout = ImageLayout.Center
        btnSauvegardes.FlatAppearance.BorderSize = 0
        btnSauvegardes.FlatStyle = FlatStyle.Flat
        btnSauvegardes.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnSauvegardes.ForeColor = Color.White
        btnSauvegardes.Image = CType(resources.GetObject("btnSauvegardes.Image"), Image)
        btnSauvegardes.Location = New Point(115, 220)
        btnSauvegardes.Name = "btnSauvegardes"
        btnSauvegardes.Size = New Size(235, 90)
        btnSauvegardes.TabIndex = 13
        btnSauvegardes.Tag = "sauvegarde_normal"
        btnSauvegardes.Text = "Sauvegardes" & vbCrLf & "Backup / Restauration"
        btnSauvegardes.TextImageRelation = TextImageRelation.ImageBeforeText
        btnSauvegardes.UseVisualStyleBackColor = False
        ' 
        ' btnConnexionDatabase
        ' 
        btnConnexionDatabase.Anchor = AnchorStyles.None
        btnConnexionDatabase.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnConnexionDatabase.BackgroundImageLayout = ImageLayout.Center
        btnConnexionDatabase.FlatAppearance.BorderSize = 0
        btnConnexionDatabase.FlatStyle = FlatStyle.Flat
        btnConnexionDatabase.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnConnexionDatabase.ForeColor = Color.White
        btnConnexionDatabase.Image = CType(resources.GetObject("btnConnexionDatabase.Image"), Image)
        btnConnexionDatabase.Location = New Point(582, 220)
        btnConnexionDatabase.Name = "btnConnexionDatabase"
        btnConnexionDatabase.Size = New Size(235, 90)
        btnConnexionDatabase.TabIndex = 8
        btnConnexionDatabase.Tag = "configurationDatabase_normal"
        btnConnexionDatabase.Text = "Configuration Database" & vbCrLf & "Configurer MariaDB"
        btnConnexionDatabase.TextImageRelation = TextImageRelation.ImageBeforeText
        btnConnexionDatabase.UseVisualStyleBackColor = False
        ' 
        ' pnlRoleCourant
        ' 
        pnlRoleCourant.BackColor = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        pnlRoleCourant.Controls.Add(btnEleverAcces)
        pnlRoleCourant.Controls.Add(lblRoleCourant)
        pnlRoleCourant.Controls.Add(btnRetourRoleBase)
        pnlRoleCourant.Controls.Add(lblElevation)
        pnlRoleCourant.Dock = DockStyle.Top
        pnlRoleCourant.Font = New Font("Calibri", 11F)
        pnlRoleCourant.Location = New Point(16, 58)
        pnlRoleCourant.Name = "pnlRoleCourant"
        pnlRoleCourant.Padding = New Padding(8)
        pnlRoleCourant.Size = New Size(933, 88)
        pnlRoleCourant.TabIndex = 6
        ' 
        ' btnEleverAcces
        ' 
        btnEleverAcces.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnEleverAcces.BackgroundImageLayout = ImageLayout.Center
        btnEleverAcces.FlatAppearance.BorderSize = 0
        btnEleverAcces.FlatStyle = FlatStyle.Flat
        btnEleverAcces.ForeColor = Color.White
        btnEleverAcces.Image = CType(resources.GetObject("btnEleverAcces.Image"), Image)
        btnEleverAcces.ImageAlign = ContentAlignment.MiddleLeft
        btnEleverAcces.Location = New Point(471, 5)
        btnEleverAcces.Name = "btnEleverAcces"
        btnEleverAcces.Size = New Size(181, 37)
        btnEleverAcces.TabIndex = 12
        btnEleverAcces.Tag = "eleverAcces_normal"
        btnEleverAcces.Text = "Elever Accès"
        btnEleverAcces.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEleverAcces.UseVisualStyleBackColor = False
        ' 
        ' lblRoleCourant
        ' 
        lblRoleCourant.Font = New Font("Calibri", 12F, FontStyle.Bold)
        lblRoleCourant.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblRoleCourant.Location = New Point(280, 52)
        lblRoleCourant.Name = "lblRoleCourant"
        lblRoleCourant.Size = New Size(185, 26)
        lblRoleCourant.TabIndex = 10
        lblRoleCourant.Text = "Rôle courant"
        lblRoleCourant.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' btnRetourRoleBase
        ' 
        btnRetourRoleBase.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnRetourRoleBase.BackgroundImageLayout = ImageLayout.Center
        btnRetourRoleBase.FlatAppearance.BorderSize = 0
        btnRetourRoleBase.FlatStyle = FlatStyle.Flat
        btnRetourRoleBase.ForeColor = Color.White
        btnRetourRoleBase.Image = CType(resources.GetObject("btnRetourRoleBase.Image"), Image)
        btnRetourRoleBase.ImageAlign = ContentAlignment.MiddleLeft
        btnRetourRoleBase.Location = New Point(471, 45)
        btnRetourRoleBase.Name = "btnRetourRoleBase"
        btnRetourRoleBase.Size = New Size(181, 37)
        btnRetourRoleBase.TabIndex = 13
        btnRetourRoleBase.Tag = "retourRole_normal"
        btnRetourRoleBase.Text = "Retour rôle de base"
        btnRetourRoleBase.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRetourRoleBase.UseVisualStyleBackColor = False
        ' 
        ' lblElevation
        ' 
        lblElevation.Font = New Font("Calibri", 12F, FontStyle.Bold)
        lblElevation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblElevation.Location = New Point(280, 16)
        lblElevation.Name = "lblElevation"
        lblElevation.Size = New Size(185, 26)
        lblElevation.TabIndex = 11
        lblElevation.Text = "Elevation"
        lblElevation.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.Transparent
        pnlTop.Controls.Add(lblTop)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(8)
        pnlTop.Size = New Size(933, 42)
        pnlTop.TabIndex = 5
        ' 
        ' lblTop
        ' 
        lblTop.Dock = DockStyle.Fill
        lblTop.Font = New Font("Calibri", 14F, FontStyle.Bold)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(8, 8)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(917, 26)
        lblTop.TabIndex = 0
        lblTop.Text = "Administration de l'application"
        ' 
        ' pnlTitre
        ' 
        pnlTitre.BackColor = Color.Transparent
        pnlTitre.Controls.Add(lblTitreForm)
        pnlTitre.Controls.Add(picTitre)
        pnlTitre.Dock = DockStyle.Top
        pnlTitre.Location = New Point(16, 16)
        pnlTitre.Name = "pnlTitre"
        pnlTitre.Size = New Size(965, 58)
        pnlTitre.TabIndex = 4
        ' 
        ' lblTitreForm
        ' 
        lblTitreForm.Anchor = AnchorStyles.Left
        lblTitreForm.AutoSize = True
        lblTitreForm.BackColor = Color.Transparent
        lblTitreForm.Font = New Font("Calibri", 18F, FontStyle.Bold)
        lblTitreForm.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTitreForm.Location = New Point(80, 3)
        lblTitreForm.Name = "lblTitreForm"
        lblTitreForm.Padding = New Padding(0, 4, 8, 4)
        lblTitreForm.Size = New Size(218, 37)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Hub Administration"
        ' 
        ' picTitre
        ' 
        picTitre.BackgroundImage = CType(resources.GetObject("picTitre.BackgroundImage"), Image)
        picTitre.BackgroundImageLayout = ImageLayout.Stretch
        picTitre.Location = New Point(14, 3)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(60, 52)
        picTitre.TabIndex = 2
        picTitre.TabStop = False
        ' 
        ' UC_AdminHome
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_AdminHome"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        tblMenu.ResumeLayout(False)
        pnlRoleCourant.ResumeLayout(False)
        pnlTop.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents picTitre As PictureBox
    Friend WithEvents Label2 As Label
    Friend WithEvents btnVersionBase As Button
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents tblMenu As TableLayoutPanel
    Friend WithEvents btnParametres As Button
    Friend WithEvents btnUtilisateurs As Button
    Friend WithEvents btnLogs As Button
    Friend WithEvents btnSauvegardes As Button
    Friend WithEvents btnConnexionDatabase As Button
    Friend WithEvents pnlTop As Panel
    Friend WithEvents lblTop As Label
    Friend WithEvents pnlRoleCourant As Panel
    Friend WithEvents btnEleverAcces As Button
    Friend WithEvents lblRoleCourant As Label
    Friend WithEvents btnRetourRoleBase As Button
    Friend WithEvents lblElevation As Label

End Class
