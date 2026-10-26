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
        carteMenu = New AltheaCarte()
        tblMenu = New TableLayoutPanel()
        lblGroupeCabinetAcces = New AltheaTitreSection()
        btnCabinet = New AltheaTuile()
        btnSituationsJuridiques = New AltheaTuile()
        btnTypesIdentifiant = New AltheaTuile()
        btnUtilisateurs = New AltheaTuile()
        lblGroupeTechnique = New AltheaTitreSection()
        btnParametres = New AltheaTuile()
        btnConnexionDatabase = New AltheaTuile()
        btnSauvegardes = New AltheaTuile()
        btnLogs = New AltheaTuile()
        btnEtatSysteme = New AltheaTuile()
        pnlRoleCourant = New AltheaCarte()
        btnEleverAcces = New AltheaBouton()
        lblRoleCourant = New Label()
        btnRetourRoleBase = New AltheaBouton()
        lblElevation = New Label()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        carteMenu.SuspendLayout()
        tblMenu.SuspendLayout()
        pnlRoleCourant.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(enTete)
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
        pnlCenter.Controls.Add(carteMenu)
        pnlCenter.Controls.Add(pnlRoleCourant)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16)
        pnlCenter.Size = New Size(965, 695)
        pnlCenter.TabIndex = 21
        ' 
        ' carteMenu
        ' 
        carteMenu.Controls.Add(tblMenu)
        carteMenu.Dock = DockStyle.Fill
        carteMenu.Location = New Point(16, 112)
        carteMenu.Name = "carteMenu"
        carteMenu.Padding = New Padding(18, 14, 18, 16)
        carteMenu.Size = New Size(933, 567)
        carteMenu.TabIndex = 7
        ' 
        ' tblMenu
        ' 
        tblMenu.ColumnCount = 4
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25F))
        tblMenu.Controls.Add(lblGroupeCabinetAcces, 0, 0)
        tblMenu.Controls.Add(btnCabinet, 0, 1)
        tblMenu.Controls.Add(btnSituationsJuridiques, 1, 1)
        tblMenu.Controls.Add(btnTypesIdentifiant, 2, 1)
        tblMenu.Controls.Add(btnUtilisateurs, 3, 1)
        tblMenu.Controls.Add(lblGroupeTechnique, 0, 2)
        tblMenu.Controls.Add(btnParametres, 0, 3)
        tblMenu.Controls.Add(btnConnexionDatabase, 1, 3)
        tblMenu.Controls.Add(btnSauvegardes, 2, 3)
        tblMenu.Controls.Add(btnLogs, 3, 3)
        tblMenu.Controls.Add(btnEtatSysteme, 0, 4)
        tblMenu.Dock = DockStyle.Fill
        tblMenu.Location = New Point(18, 14)
        tblMenu.Name = "tblMenu"
        tblMenu.Padding = New Padding(12, 12, 17, 18)
        tblMenu.RowCount = 6
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Absolute, 28F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 33.33F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Absolute, 28F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 33.33F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 33.33F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
        tblMenu.Size = New Size(897, 537)
        tblMenu.TabIndex = 10
        ' 
        ' lblGroupeCabinetAcces
        ' 
        lblGroupeCabinetAcces.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        lblGroupeCabinetAcces.BackColor = Color.Transparent
        tblMenu.SetColumnSpan(lblGroupeCabinetAcces, 4)
        lblGroupeCabinetAcces.Font = New Font("Calibri", 13F, FontStyle.Bold)
        lblGroupeCabinetAcces.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblGroupeCabinetAcces.Location = New Point(24, 16)
        lblGroupeCabinetAcces.Margin = New Padding(12, 0, 3, 2)
        lblGroupeCabinetAcces.Name = "lblGroupeCabinetAcces"
        lblGroupeCabinetAcces.Size = New Size(853, 22)
        lblGroupeCabinetAcces.TabIndex = 14
        lblGroupeCabinetAcces.Text = "Cabinet et accès"
        ' 
        ' btnCabinet
        ' 
        btnCabinet.Anchor = AnchorStyles.None
        btnCabinet.FlatAppearance.BorderSize = 0
        btnCabinet.FlatStyle = FlatStyle.Flat
        btnCabinet.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnCabinet.ForeColor = Color.White
        btnCabinet.Image = CType(resources.GetObject("btnCabinet.Image"), Image)
        btnCabinet.Location = New Point(15, 66)
        btnCabinet.Name = "btnCabinet"
        btnCabinet.Rayon = 12
        btnCabinet.Size = New Size(211, 90)
        btnCabinet.TabIndex = 14
        btnCabinet.Tag = "cabinet_normal"
        btnCabinet.Text = "Cabinet" & vbCrLf & "Praticien, cachet"
        btnCabinet.TextAlign = ContentAlignment.MiddleLeft
        btnCabinet.TextImageRelation = TextImageRelation.ImageBeforeText
        btnCabinet.UseVisualStyleBackColor = False
        ' 
        ' btnSituationsJuridiques
        ' 
        btnSituationsJuridiques.Anchor = AnchorStyles.None
        btnSituationsJuridiques.FlatAppearance.BorderSize = 0
        btnSituationsJuridiques.FlatStyle = FlatStyle.Flat
        btnSituationsJuridiques.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnSituationsJuridiques.ForeColor = Color.White
        btnSituationsJuridiques.Image = CType(resources.GetObject("btnSituationsJuridiques.Image"), Image)
        btnSituationsJuridiques.Location = New Point(232, 66)
        btnSituationsJuridiques.Name = "btnSituationsJuridiques"
        btnSituationsJuridiques.Rayon = 12
        btnSituationsJuridiques.Size = New Size(211, 90)
        btnSituationsJuridiques.TabIndex = 16
        btnSituationsJuridiques.Tag = "situationJuridique_normal"
        btnSituationsJuridiques.Text = "Situations" & vbCrLf & "juridiques"
        btnSituationsJuridiques.TextAlign = ContentAlignment.MiddleLeft
        btnSituationsJuridiques.TextImageRelation = TextImageRelation.ImageBeforeText
        btnSituationsJuridiques.UseCompatibleTextRendering = True
        btnSituationsJuridiques.UseVisualStyleBackColor = False
        ' 
        ' btnTypesIdentifiant
        ' 
        btnTypesIdentifiant.Anchor = AnchorStyles.None
        btnTypesIdentifiant.FlatAppearance.BorderSize = 0
        btnTypesIdentifiant.FlatStyle = FlatStyle.Flat
        btnTypesIdentifiant.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnTypesIdentifiant.ForeColor = Color.White
        btnTypesIdentifiant.Location = New Point(449, 66)
        btnTypesIdentifiant.Name = "btnTypesIdentifiant"
        btnTypesIdentifiant.Rayon = 12
        btnTypesIdentifiant.Size = New Size(211, 90)
        btnTypesIdentifiant.TabIndex = 17
        btnTypesIdentifiant.Tag = "typeIdentifiant_normal"
        btnTypesIdentifiant.Text = "Types" & vbCrLf & "d'identifiant"
        btnTypesIdentifiant.TextAlign = ContentAlignment.MiddleLeft
        btnTypesIdentifiant.TextImageRelation = TextImageRelation.ImageBeforeText
        btnTypesIdentifiant.UseCompatibleTextRendering = True
        btnTypesIdentifiant.UseVisualStyleBackColor = False
        ' 
        ' btnUtilisateurs
        ' 
        btnUtilisateurs.Anchor = AnchorStyles.None
        btnUtilisateurs.FlatAppearance.BorderSize = 0
        btnUtilisateurs.FlatStyle = FlatStyle.Flat
        btnUtilisateurs.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnUtilisateurs.ForeColor = Color.White
        btnUtilisateurs.Image = CType(resources.GetObject("btnUtilisateurs.Image"), Image)
        btnUtilisateurs.Location = New Point(666, 66)
        btnUtilisateurs.Name = "btnUtilisateurs"
        btnUtilisateurs.Rayon = 12
        btnUtilisateurs.Size = New Size(211, 90)
        btnUtilisateurs.TabIndex = 12
        btnUtilisateurs.Tag = "utilisateurs_normal"
        btnUtilisateurs.Text = "Utilisateurs" & vbCrLf & "Gestion, Rôles"
        btnUtilisateurs.TextAlign = ContentAlignment.MiddleLeft
        btnUtilisateurs.TextImageRelation = TextImageRelation.ImageBeforeText
        btnUtilisateurs.UseVisualStyleBackColor = False
        ' 
        ' lblGroupeTechnique
        ' 
        lblGroupeTechnique.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        lblGroupeTechnique.BackColor = Color.Transparent
        tblMenu.SetColumnSpan(lblGroupeTechnique, 4)
        lblGroupeTechnique.Font = New Font("Calibri", 13F, FontStyle.Bold)
        lblGroupeTechnique.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblGroupeTechnique.Location = New Point(24, 187)
        lblGroupeTechnique.Margin = New Padding(12, 0, 3, 2)
        lblGroupeTechnique.Name = "lblGroupeTechnique"
        lblGroupeTechnique.Size = New Size(853, 22)
        lblGroupeTechnique.TabIndex = 15
        lblGroupeTechnique.Text = "Technique"
        ' 
        ' btnParametres
        ' 
        btnParametres.Anchor = AnchorStyles.None
        btnParametres.FlatAppearance.BorderSize = 0
        btnParametres.FlatStyle = FlatStyle.Flat
        btnParametres.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnParametres.ForeColor = Color.White
        btnParametres.Image = CType(resources.GetObject("btnParametres.Image"), Image)
        btnParametres.Location = New Point(15, 237)
        btnParametres.Name = "btnParametres"
        btnParametres.Rayon = 12
        btnParametres.Size = New Size(211, 90)
        btnParametres.TabIndex = 9
        btnParametres.Tag = "parametres_normal"
        btnParametres.Text = "Paramètres" & vbCrLf & "Chemins, stockage, options"
        btnParametres.TextAlign = ContentAlignment.MiddleLeft
        btnParametres.TextImageRelation = TextImageRelation.ImageBeforeText
        btnParametres.UseCompatibleTextRendering = True
        btnParametres.UseVisualStyleBackColor = False
        ' 
        ' btnConnexionDatabase
        ' 
        btnConnexionDatabase.Anchor = AnchorStyles.None
        btnConnexionDatabase.FlatAppearance.BorderSize = 0
        btnConnexionDatabase.FlatStyle = FlatStyle.Flat
        btnConnexionDatabase.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnConnexionDatabase.ForeColor = Color.White
        btnConnexionDatabase.Image = CType(resources.GetObject("btnConnexionDatabase.Image"), Image)
        btnConnexionDatabase.Location = New Point(232, 237)
        btnConnexionDatabase.Name = "btnConnexionDatabase"
        btnConnexionDatabase.Rayon = 12
        btnConnexionDatabase.Size = New Size(211, 90)
        btnConnexionDatabase.TabIndex = 8
        btnConnexionDatabase.Tag = "configurationDatabase_normal"
        btnConnexionDatabase.Text = "Configuration Database" & vbCrLf & "Configurer MariaDB"
        btnConnexionDatabase.TextAlign = ContentAlignment.MiddleLeft
        btnConnexionDatabase.TextImageRelation = TextImageRelation.ImageBeforeText
        btnConnexionDatabase.UseVisualStyleBackColor = False
        ' 
        ' btnSauvegardes
        ' 
        btnSauvegardes.Anchor = AnchorStyles.None
        btnSauvegardes.FlatAppearance.BorderSize = 0
        btnSauvegardes.FlatStyle = FlatStyle.Flat
        btnSauvegardes.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnSauvegardes.ForeColor = Color.White
        btnSauvegardes.Image = CType(resources.GetObject("btnSauvegardes.Image"), Image)
        btnSauvegardes.Location = New Point(449, 237)
        btnSauvegardes.Name = "btnSauvegardes"
        btnSauvegardes.Rayon = 12
        btnSauvegardes.Size = New Size(211, 90)
        btnSauvegardes.TabIndex = 13
        btnSauvegardes.Tag = "sauvegarde_normal"
        btnSauvegardes.Text = "Sauvegardes" & vbCrLf & "Backup / Restauration"
        btnSauvegardes.TextAlign = ContentAlignment.MiddleLeft
        btnSauvegardes.TextImageRelation = TextImageRelation.ImageBeforeText
        btnSauvegardes.UseVisualStyleBackColor = False
        ' 
        ' btnLogs
        ' 
        btnLogs.Anchor = AnchorStyles.None
        btnLogs.FlatAppearance.BorderSize = 0
        btnLogs.FlatStyle = FlatStyle.Flat
        btnLogs.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnLogs.ForeColor = Color.White
        btnLogs.Image = CType(resources.GetObject("btnLogs.Image"), Image)
        btnLogs.Location = New Point(666, 237)
        btnLogs.Name = "btnLogs"
        btnLogs.Rayon = 12
        btnLogs.Size = New Size(211, 90)
        btnLogs.TabIndex = 10
        btnLogs.Tag = "journaux_normal"
        btnLogs.Text = "Journaux" & vbCrLf & "Consulter les journaux"
        btnLogs.TextAlign = ContentAlignment.MiddleLeft
        btnLogs.TextImageRelation = TextImageRelation.ImageBeforeText
        btnLogs.UseVisualStyleBackColor = False
        ' 
        ' btnEtatSysteme
        ' 
        btnEtatSysteme.Anchor = AnchorStyles.None
        btnEtatSysteme.FlatAppearance.BorderSize = 0
        btnEtatSysteme.FlatStyle = FlatStyle.Flat
        btnEtatSysteme.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnEtatSysteme.ForeColor = Color.White
        btnEtatSysteme.Image = CType(resources.GetObject("btnEtatSysteme.Image"), Image)
        btnEtatSysteme.Location = New Point(15, 380)
        btnEtatSysteme.Name = "btnEtatSysteme"
        btnEtatSysteme.Rayon = 12
        btnEtatSysteme.Size = New Size(211, 90)
        btnEtatSysteme.TabIndex = 17
        btnEtatSysteme.Tag = "etatSystem_normal"
        btnEtatSysteme.Text = "État du système" & vbCrLf & "Versions, disques, outils"
        btnEtatSysteme.TextAlign = ContentAlignment.MiddleLeft
        btnEtatSysteme.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEtatSysteme.UseVisualStyleBackColor = False
        ' 
        ' pnlRoleCourant
        ' 
        pnlRoleCourant.Controls.Add(btnEleverAcces)
        pnlRoleCourant.Controls.Add(lblRoleCourant)
        pnlRoleCourant.Controls.Add(btnRetourRoleBase)
        pnlRoleCourant.Controls.Add(lblElevation)
        pnlRoleCourant.Dock = DockStyle.Top
        pnlRoleCourant.Font = New Font("Calibri", 11F)
        pnlRoleCourant.Location = New Point(16, 16)
        pnlRoleCourant.Name = "pnlRoleCourant"
        pnlRoleCourant.Padding = New Padding(8)
        pnlRoleCourant.Size = New Size(933, 96)
        pnlRoleCourant.TabIndex = 6
        ' 
        ' btnEleverAcces
        ' 
        btnEleverAcces.FlatAppearance.BorderSize = 0
        btnEleverAcces.FlatStyle = FlatStyle.Flat
        btnEleverAcces.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnEleverAcces.ForeColor = Color.White
        btnEleverAcces.Image = CType(resources.GetObject("btnEleverAcces.Image"), Image)
        btnEleverAcces.ImageAlign = ContentAlignment.TopLeft
        btnEleverAcces.Location = New Point(471, 5)
        btnEleverAcces.Name = "btnEleverAcces"
        btnEleverAcces.Size = New Size(220, 44)
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
        btnRetourRoleBase.FlatAppearance.BorderSize = 0
        btnRetourRoleBase.FlatStyle = FlatStyle.Flat
        btnRetourRoleBase.Font = New Font("Calibri", 11F, FontStyle.Bold)
        btnRetourRoleBase.ForeColor = Color.White
        btnRetourRoleBase.Image = CType(resources.GetObject("btnRetourRoleBase.Image"), Image)
        btnRetourRoleBase.ImageAlign = ContentAlignment.TopLeft
        btnRetourRoleBase.Location = New Point(471, 50)
        btnRetourRoleBase.Name = "btnRetourRoleBase"
        btnRetourRoleBase.Size = New Size(220, 44)
        btnRetourRoleBase.TabIndex = 13
        btnRetourRoleBase.Tag = "retourRole_normal"
        btnRetourRoleBase.Text = "Retour rôle de base"
        btnRetourRoleBase.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRetourRoleBase.UseVisualStyleBackColor = False
        btnRetourRoleBase.Variante = VarianteBouton.Clair
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
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Padding = New Padding(16, 4, 16, 4)
        enTete.Size = New Size(965, 58)
        enTete.SousTitre = "Gérez les paramètres administratifs de l'application"
        enTete.TabIndex = 4
        enTete.Text = "Hub Administration"
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
        carteMenu.ResumeLayout(False)
        tblMenu.ResumeLayout(False)
        pnlRoleCourant.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents Label2 As Label
    Friend WithEvents btnVersionBase As Button
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents carteMenu As AltheaCarte
    Friend WithEvents tblMenu As TableLayoutPanel
    Friend WithEvents lblGroupeCabinetAcces As AltheaTitreSection
    Friend WithEvents lblGroupeTechnique As AltheaTitreSection
    Friend WithEvents btnSituationsJuridiques As AltheaTuile
    Friend WithEvents btnTypesIdentifiant As AltheaTuile
    Friend WithEvents btnParametres As AltheaTuile
    Friend WithEvents btnUtilisateurs As AltheaTuile
    Friend WithEvents btnLogs As AltheaTuile
    Friend WithEvents btnEtatSysteme As AltheaTuile
    Friend WithEvents btnSauvegardes As AltheaTuile
    Friend WithEvents btnConnexionDatabase As AltheaTuile
    Friend WithEvents btnCabinet As AltheaTuile
    Friend WithEvents pnlRoleCourant As AltheaCarte
    Friend WithEvents btnEleverAcces As AltheaBouton
    Friend WithEvents lblRoleCourant As Label
    Friend WithEvents btnRetourRoleBase As AltheaBouton
    Friend WithEvents lblElevation As Label

End Class
