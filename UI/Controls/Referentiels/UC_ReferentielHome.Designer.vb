' -------------------------------------------------------------------------------------------------
' Designer    : UC_ReferentielHome
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Définit la structure visuelle du hub de gestion des référentiels (tables ref_*).
' Reprend la charte du hub d'administration (UC_AdminHome) : titre + grille de tuiles + bloc d'élévation.
'
' Remarques   :
' - Une tuile par référentiel (3 colonnes x 3 lignes).
' - Seule la tuile "Domaines" est active pour l'instant ; les autres sont désactivées (à venir).
' - Les images des tuiles sont chargées au runtime via UtilsButtons (Tag = xxx_normal).
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_ReferentielHome
    Inherits System.Windows.Forms.UserControl

    'UserControl remplace la méthode Dispose pour nettoyer la liste des composants.
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

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_ReferentielHome))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        tblMenu = New TableLayoutPanel()
        btnDomaines = New Button()
        btnLiensPatient = New Button()
        btnRolesIntervenant = New Button()
        btnSituationsFamiliales = New Button()
        btnStatutsDossier = New Button()
        btnStatutsSeance = New Button()
        btnTypesDocuments = New Button()
        btnTypesRendezVous = New Button()
        btnTypesSeance = New Button()
        pnlElevation = New Panel()
        btnRetourRoleBase = New Button()
        lblElevation = New Label()
        pnlRoleCourant = New Panel()
        btnEleverAcces = New Button()
        lblRoleCourant = New Label()
        pnlTop = New Panel()
        lblTop = New Label()
        pnlTitre = New Panel()
        lblTitreForm = New Label()
        picTitre = New PictureBox()
        ttMenu = New ToolTip(components)
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        tblMenu.SuspendLayout()
        pnlElevation.SuspendLayout()
        pnlRoleCourant.SuspendLayout()
        pnlTop.SuspendLayout()
        pnlTitre.SuspendLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
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
        pnlCenter.Controls.Add(pnlElevation)
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
        tblMenu.ColumnCount = 3
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.3333321F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.3333321F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.3333321F))
        tblMenu.Controls.Add(btnDomaines, 0, 0)
        tblMenu.Controls.Add(btnLiensPatient, 1, 0)
        tblMenu.Controls.Add(btnRolesIntervenant, 2, 0)
        tblMenu.Controls.Add(btnSituationsFamiliales, 0, 1)
        tblMenu.Controls.Add(btnStatutsDossier, 1, 1)
        tblMenu.Controls.Add(btnStatutsSeance, 2, 1)
        tblMenu.Controls.Add(btnTypesDocuments, 0, 2)
        tblMenu.Controls.Add(btnTypesRendezVous, 1, 2)
        tblMenu.Controls.Add(btnTypesSeance, 2, 2)
        tblMenu.Dock = DockStyle.Fill
        tblMenu.Location = New Point(16, 142)
        tblMenu.Name = "tblMenu"
        tblMenu.RowCount = 3
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))
        tblMenu.Size = New Size(933, 537)
        tblMenu.TabIndex = 10
        ' 
        ' btnDomaines
        ' 
        btnDomaines.Anchor = AnchorStyles.None
        btnDomaines.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnDomaines.BackgroundImageLayout = ImageLayout.Center
        btnDomaines.FlatAppearance.BorderSize = 0
        btnDomaines.FlatStyle = FlatStyle.Flat
        btnDomaines.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnDomaines.ForeColor = Color.White
        btnDomaines.Image = CType(resources.GetObject("btnDomaines.Image"), Image)
        btnDomaines.Location = New Point(37, 44)
        btnDomaines.Name = "btnDomaines"
        btnDomaines.Size = New Size(235, 90)
        btnDomaines.TabIndex = 0
        btnDomaines.Tag = "domaines_normal"
        btnDomaines.Text = "Domaines" & vbCrLf & "Prises en charge"
        btnDomaines.TextImageRelation = TextImageRelation.ImageBeforeText
        btnDomaines.UseCompatibleTextRendering = True
        btnDomaines.UseVisualStyleBackColor = False
        ' 
        ' btnLiensPatient
        ' 
        btnLiensPatient.Anchor = AnchorStyles.None
        btnLiensPatient.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnLiensPatient.BackgroundImageLayout = ImageLayout.Center
        btnLiensPatient.Enabled = False
        btnLiensPatient.FlatAppearance.BorderSize = 0
        btnLiensPatient.FlatStyle = FlatStyle.Flat
        btnLiensPatient.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnLiensPatient.ForeColor = Color.White
        btnLiensPatient.Image = CType(resources.GetObject("btnLiensPatient.Image"), Image)
        btnLiensPatient.Location = New Point(347, 44)
        btnLiensPatient.Name = "btnLiensPatient"
        btnLiensPatient.Size = New Size(235, 90)
        btnLiensPatient.TabIndex = 1
        btnLiensPatient.Tag = "liensPatient_normal"
        btnLiensPatient.Text = "Liens patient" & vbCrLf & "Entourage (à venir)"
        btnLiensPatient.TextImageRelation = TextImageRelation.ImageBeforeText
        btnLiensPatient.UseCompatibleTextRendering = True
        btnLiensPatient.UseVisualStyleBackColor = False
        ' 
        ' btnRolesIntervenant
        ' 
        btnRolesIntervenant.Anchor = AnchorStyles.None
        btnRolesIntervenant.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnRolesIntervenant.BackgroundImageLayout = ImageLayout.Center
        btnRolesIntervenant.Enabled = False
        btnRolesIntervenant.FlatAppearance.BorderSize = 0
        btnRolesIntervenant.FlatStyle = FlatStyle.Flat
        btnRolesIntervenant.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnRolesIntervenant.ForeColor = Color.White
        btnRolesIntervenant.Image = CType(resources.GetObject("btnRolesIntervenant.Image"), Image)
        btnRolesIntervenant.Location = New Point(659, 44)
        btnRolesIntervenant.Name = "btnRolesIntervenant"
        btnRolesIntervenant.Size = New Size(235, 90)
        btnRolesIntervenant.TabIndex = 2
        btnRolesIntervenant.Tag = "roleIntervenant_normal"
        btnRolesIntervenant.Text = "Rôles intervenant" & vbCrLf & "(à venir)"
        btnRolesIntervenant.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRolesIntervenant.UseCompatibleTextRendering = True
        btnRolesIntervenant.UseVisualStyleBackColor = False
        ' 
        ' btnSituationsFamiliales
        ' 
        btnSituationsFamiliales.Anchor = AnchorStyles.None
        btnSituationsFamiliales.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnSituationsFamiliales.BackgroundImageLayout = ImageLayout.Center
        btnSituationsFamiliales.Enabled = False
        btnSituationsFamiliales.FlatAppearance.BorderSize = 0
        btnSituationsFamiliales.FlatStyle = FlatStyle.Flat
        btnSituationsFamiliales.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnSituationsFamiliales.ForeColor = Color.White
        btnSituationsFamiliales.Image = CType(resources.GetObject("btnSituationsFamiliales.Image"), Image)
        btnSituationsFamiliales.Location = New Point(37, 223)
        btnSituationsFamiliales.Name = "btnSituationsFamiliales"
        btnSituationsFamiliales.Size = New Size(235, 90)
        btnSituationsFamiliales.TabIndex = 3
        btnSituationsFamiliales.Tag = "situationFamiliale_normal"
        btnSituationsFamiliales.Text = "Situations familiales" & vbCrLf & "(à venir)"
        btnSituationsFamiliales.TextImageRelation = TextImageRelation.ImageBeforeText
        btnSituationsFamiliales.UseCompatibleTextRendering = True
        btnSituationsFamiliales.UseVisualStyleBackColor = False
        ' 
        ' btnStatutsDossier
        ' 
        btnStatutsDossier.Anchor = AnchorStyles.None
        btnStatutsDossier.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnStatutsDossier.BackgroundImageLayout = ImageLayout.Center
        btnStatutsDossier.Enabled = False
        btnStatutsDossier.FlatAppearance.BorderSize = 0
        btnStatutsDossier.FlatStyle = FlatStyle.Flat
        btnStatutsDossier.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnStatutsDossier.ForeColor = Color.White
        btnStatutsDossier.Image = CType(resources.GetObject("btnStatutsDossier.Image"), Image)
        btnStatutsDossier.Location = New Point(347, 223)
        btnStatutsDossier.Name = "btnStatutsDossier"
        btnStatutsDossier.Size = New Size(235, 90)
        btnStatutsDossier.TabIndex = 4
        btnStatutsDossier.Tag = "statutDossier_normal"
        btnStatutsDossier.Text = "Statuts dossier" & vbCrLf & "(à venir)"
        btnStatutsDossier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnStatutsDossier.UseCompatibleTextRendering = True
        btnStatutsDossier.UseVisualStyleBackColor = False
        ' 
        ' btnStatutsSeance
        ' 
        btnStatutsSeance.Anchor = AnchorStyles.None
        btnStatutsSeance.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnStatutsSeance.BackgroundImageLayout = ImageLayout.Center
        btnStatutsSeance.Enabled = False
        btnStatutsSeance.FlatAppearance.BorderSize = 0
        btnStatutsSeance.FlatStyle = FlatStyle.Flat
        btnStatutsSeance.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnStatutsSeance.ForeColor = Color.White
        btnStatutsSeance.Image = CType(resources.GetObject("btnStatutsSeance.Image"), Image)
        btnStatutsSeance.Location = New Point(659, 223)
        btnStatutsSeance.Name = "btnStatutsSeance"
        btnStatutsSeance.Size = New Size(235, 90)
        btnStatutsSeance.TabIndex = 5
        btnStatutsSeance.Tag = "statutSeance_normal"
        btnStatutsSeance.Text = "Statuts séance" & vbCrLf & "(à venir)"
        btnStatutsSeance.TextImageRelation = TextImageRelation.ImageBeforeText
        btnStatutsSeance.UseCompatibleTextRendering = True
        btnStatutsSeance.UseVisualStyleBackColor = False
        ' 
        ' btnTypesDocuments
        ' 
        btnTypesDocuments.Anchor = AnchorStyles.None
        btnTypesDocuments.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnTypesDocuments.BackgroundImageLayout = ImageLayout.Center
        btnTypesDocuments.Enabled = False
        btnTypesDocuments.FlatAppearance.BorderSize = 0
        btnTypesDocuments.FlatStyle = FlatStyle.Flat
        btnTypesDocuments.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnTypesDocuments.ForeColor = Color.White
        btnTypesDocuments.Image = CType(resources.GetObject("btnTypesDocuments.Image"), Image)
        btnTypesDocuments.Location = New Point(37, 402)
        btnTypesDocuments.Name = "btnTypesDocuments"
        btnTypesDocuments.Size = New Size(235, 90)
        btnTypesDocuments.TabIndex = 6
        btnTypesDocuments.Tag = "typeDocument_normal"
        btnTypesDocuments.Text = "Types documents" & vbCrLf & "(à venir)"
        btnTypesDocuments.TextImageRelation = TextImageRelation.ImageBeforeText
        btnTypesDocuments.UseCompatibleTextRendering = True
        btnTypesDocuments.UseVisualStyleBackColor = False
        ' 
        ' btnTypesRendezVous
        ' 
        btnTypesRendezVous.Anchor = AnchorStyles.None
        btnTypesRendezVous.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnTypesRendezVous.BackgroundImageLayout = ImageLayout.Center
        btnTypesRendezVous.Enabled = False
        btnTypesRendezVous.FlatAppearance.BorderSize = 0
        btnTypesRendezVous.FlatStyle = FlatStyle.Flat
        btnTypesRendezVous.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnTypesRendezVous.ForeColor = Color.White
        btnTypesRendezVous.Image = CType(resources.GetObject("btnTypesRendezVous.Image"), Image)
        btnTypesRendezVous.Location = New Point(347, 402)
        btnTypesRendezVous.Name = "btnTypesRendezVous"
        btnTypesRendezVous.Size = New Size(235, 90)
        btnTypesRendezVous.TabIndex = 7
        btnTypesRendezVous.Tag = "typeRendezvous_normal"
        btnTypesRendezVous.Text = "Types rendez-vous" & vbCrLf & "(à venir)"
        btnTypesRendezVous.TextImageRelation = TextImageRelation.ImageBeforeText
        btnTypesRendezVous.UseCompatibleTextRendering = True
        btnTypesRendezVous.UseVisualStyleBackColor = False
        ' 
        ' btnTypesSeance
        ' 
        btnTypesSeance.Anchor = AnchorStyles.None
        btnTypesSeance.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnTypesSeance.BackgroundImageLayout = ImageLayout.Center
        btnTypesSeance.Enabled = False
        btnTypesSeance.FlatAppearance.BorderSize = 0
        btnTypesSeance.FlatStyle = FlatStyle.Flat
        btnTypesSeance.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnTypesSeance.ForeColor = Color.White
        btnTypesSeance.Image = CType(resources.GetObject("btnTypesSeance.Image"), Image)
        btnTypesSeance.Location = New Point(659, 402)
        btnTypesSeance.Name = "btnTypesSeance"
        btnTypesSeance.Size = New Size(235, 90)
        btnTypesSeance.TabIndex = 8
        btnTypesSeance.Tag = "typeSeance_normal"
        btnTypesSeance.Text = "Types séance" & vbCrLf & "(à venir)"
        btnTypesSeance.TextImageRelation = TextImageRelation.ImageBeforeText
        btnTypesSeance.UseCompatibleTextRendering = True
        btnTypesSeance.UseVisualStyleBackColor = False
        ' 
        ' pnlElevation
        ' 
        pnlElevation.BackColor = Color.Transparent
        pnlElevation.Controls.Add(btnRetourRoleBase)
        pnlElevation.Controls.Add(lblElevation)
        pnlElevation.Dock = DockStyle.Top
        pnlElevation.Font = New Font("Calibri", 11F)
        pnlElevation.Location = New Point(16, 100)
        pnlElevation.Name = "pnlElevation"
        pnlElevation.Padding = New Padding(8)
        pnlElevation.Size = New Size(933, 42)
        pnlElevation.TabIndex = 7
        ' 
        ' btnRetourRoleBase
        ' 
        btnRetourRoleBase.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnRetourRoleBase.BackgroundImageLayout = ImageLayout.Center
        btnRetourRoleBase.FlatAppearance.BorderSize = 0
        btnRetourRoleBase.FlatStyle = FlatStyle.Flat
        btnRetourRoleBase.ForeColor = Color.White
        btnRetourRoleBase.ImageAlign = ContentAlignment.MiddleLeft
        btnRetourRoleBase.Location = New Point(636, 0)
        btnRetourRoleBase.Name = "btnRetourRoleBase"
        btnRetourRoleBase.Size = New Size(181, 37)
        btnRetourRoleBase.TabIndex = 9
        btnRetourRoleBase.Tag = "retourRole_normal"
        btnRetourRoleBase.Text = "Retour rôle de base"
        btnRetourRoleBase.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRetourRoleBase.UseVisualStyleBackColor = False
        ' 
        ' lblElevation
        ' 
        lblElevation.Font = New Font("Calibri", 12F, FontStyle.Bold)
        lblElevation.ForeColor = Color.FromArgb(CByte(11), CByte(95), CByte(125))
        lblElevation.Location = New Point(8, 8)
        lblElevation.Name = "lblElevation"
        lblElevation.Size = New Size(465, 26)
        lblElevation.TabIndex = 1
        lblElevation.Text = "Elevation"
        lblElevation.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' pnlRoleCourant
        ' 
        pnlRoleCourant.BackColor = Color.Transparent
        pnlRoleCourant.Controls.Add(btnEleverAcces)
        pnlRoleCourant.Controls.Add(lblRoleCourant)
        pnlRoleCourant.Dock = DockStyle.Top
        pnlRoleCourant.Font = New Font("Calibri", 11F)
        pnlRoleCourant.Location = New Point(16, 58)
        pnlRoleCourant.Name = "pnlRoleCourant"
        pnlRoleCourant.Padding = New Padding(8)
        pnlRoleCourant.Size = New Size(933, 42)
        pnlRoleCourant.TabIndex = 6
        ' 
        ' btnEleverAcces
        ' 
        btnEleverAcces.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btnEleverAcces.BackgroundImageLayout = ImageLayout.Center
        btnEleverAcces.FlatAppearance.BorderSize = 0
        btnEleverAcces.FlatStyle = FlatStyle.Flat
        btnEleverAcces.ForeColor = Color.White
        btnEleverAcces.ImageAlign = ContentAlignment.MiddleLeft
        btnEleverAcces.Location = New Point(636, 0)
        btnEleverAcces.Name = "btnEleverAcces"
        btnEleverAcces.Size = New Size(181, 37)
        btnEleverAcces.TabIndex = 8
        btnEleverAcces.Tag = "eleverAcces_normal"
        btnEleverAcces.Text = "Elever Accès"
        btnEleverAcces.TextImageRelation = TextImageRelation.ImageBeforeText
        btnEleverAcces.UseVisualStyleBackColor = False
        ' 
        ' lblRoleCourant
        ' 
        lblRoleCourant.Font = New Font("Calibri", 12F, FontStyle.Bold)
        lblRoleCourant.ForeColor = Color.FromArgb(CByte(11), CByte(95), CByte(125))
        lblRoleCourant.Location = New Point(8, 8)
        lblRoleCourant.Name = "lblRoleCourant"
        lblRoleCourant.Size = New Size(465, 26)
        lblRoleCourant.TabIndex = 0
        lblRoleCourant.Text = "Rôle courant"
        lblRoleCourant.TextAlign = ContentAlignment.MiddleCenter
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
        lblTop.Font = New Font("Calibri", 11F)
        lblTop.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTop.Location = New Point(8, 8)
        lblTop.Name = "lblTop"
        lblTop.Size = New Size(917, 26)
        lblTop.TabIndex = 0
        lblTop.Text = "Gestion des valeurs de référence"
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
        lblTitreForm.Size = New Size(143, 37)
        lblTitreForm.TabIndex = 1
        lblTitreForm.Text = "Référentiels"
        ' 
        ' picTitre
        ' 
        picTitre.BackgroundImageLayout = ImageLayout.Stretch
        picTitre.Location = New Point(14, 3)
        picTitre.Name = "picTitre"
        picTitre.Size = New Size(60, 52)
        picTitre.TabIndex = 2
        picTitre.TabStop = False
        ' 
        ' UC_ReferentielHome
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_ReferentielHome"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        tblMenu.ResumeLayout(False)
        pnlElevation.ResumeLayout(False)
        pnlRoleCourant.ResumeLayout(False)
        pnlTop.ResumeLayout(False)
        pnlTitre.ResumeLayout(False)
        pnlTitre.PerformLayout()
        CType(picTitre, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents tblMenu As TableLayoutPanel
    Friend WithEvents btnDomaines As Button
    Friend WithEvents btnLiensPatient As Button
    Friend WithEvents btnRolesIntervenant As Button
    Friend WithEvents btnSituationsFamiliales As Button
    Friend WithEvents btnStatutsDossier As Button
    Friend WithEvents btnStatutsSeance As Button
    Friend WithEvents btnTypesDocuments As Button
    Friend WithEvents btnTypesRendezVous As Button
    Friend WithEvents btnTypesSeance As Button
    Friend WithEvents pnlElevation As Panel
    Friend WithEvents btnRetourRoleBase As Button
    Friend WithEvents lblElevation As Label
    Friend WithEvents pnlRoleCourant As Panel
    Friend WithEvents btnEleverAcces As Button
    Friend WithEvents lblRoleCourant As Label
    Friend WithEvents pnlTop As Panel
    Friend WithEvents lblTop As Label
    Friend WithEvents pnlTitre As Panel
    Friend WithEvents lblTitreForm As Label
    Friend WithEvents picTitre As PictureBox
    Friend WithEvents ttMenu As ToolTip

End Class
