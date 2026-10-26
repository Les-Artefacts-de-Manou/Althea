' -------------------------------------------------------------------------------------------------
' Designer    : UC_ReferentielHome
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Définit la structure visuelle du hub de gestion des référentiels (tables ref_*).
' Reprend la charte du hub d'administration (UC_AdminHome) : titre + grille de tuiles + bloc d'élévation.
'
' Remarques   :
' - Une tuile par référentiel : grille de 5 colonnes, quatre groupes (Cabinet, Patient et entourage,
'   Dossier, Séances et facturation) ; activées selon le rôle par le code.
' - Les images des tuiles sont chargées au runtime via UtilsButtons (Tag = xxx_normal).
' - V1.1.0 (20/10/2026) : tuile « Latéralités » (groupe Patient) ; Civilités en première ligne, Types
'   de rendez-vous dans le groupe Séances.
' - V1.2.0 (20/10/2026) : tuile « Domaines » retirée (menu principal) ; première ligne : Thérapeutes,
'   Organismes, Pays, Civilités.
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
        carteMenu = New AltheaCarte()
        tblMenu = New TableLayoutPanel()
        lblGroupeCabinet = New AltheaTitreSection()
        btnTherapeutes = New AltheaTuile()
        btnTypesRendezVous = New AltheaTuile()
        btnOrganismes = New AltheaTuile()
        lblGroupePatient = New AltheaTitreSection()
        btnSituationsFamiliales = New AltheaTuile()
        btnLiensPatient = New AltheaTuile()
        btnRoleLegal = New AltheaTuile()
        btnRolesIntervenant = New AltheaTuile()
        lblGroupeDossier = New AltheaTitreSection()
        btnStatutsDossier = New AltheaTuile()
        btnTypesDocuments = New AltheaTuile()
        btnPeriodicites = New AltheaTuile()
        btnTextesTypes = New AltheaTuile()
        btnUsagesTextes = New AltheaTuile()
        lblGroupeSeances = New AltheaTitreSection()
        btnNaturesSeance = New AltheaTuile()
        btnStatutsSeance = New AltheaTuile()
        btnPrixSeance = New AltheaTuile()
        btnModesPaiement = New AltheaTuile()
        btnPays = New AltheaTuile()
        btnCivilites = New AltheaTuile()
        btnLateralites = New AltheaTuile()
        btnSpecialites = New AltheaTuile()
        pnlTop = New AltheaCarte()
        btnEleverAcces = New AltheaBouton()
        lblRoleCourant = New Label()
        btnRetourRoleBase = New AltheaBouton()
        lblElevation = New Label()
        enTete = New AltheaEnTete()
        ttMenu = New ToolTip(components)
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        carteMenu.SuspendLayout()
        tblMenu.SuspendLayout()
        pnlTop.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
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
        pnlCenter.Controls.Add(pnlTop)
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
        carteMenu.Location = New Point(16, 121)
        carteMenu.Name = "carteMenu"
        carteMenu.Size = New Size(933, 558)
        carteMenu.TabIndex = 9
        ' 
        ' tblMenu
        ' 
        tblMenu.CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        tblMenu.ColumnCount = 5
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0F))
        tblMenu.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0F))
        tblMenu.Controls.Add(lblGroupeCabinet, 0, 0)
        tblMenu.Controls.Add(btnTherapeutes, 0, 1)
        tblMenu.Controls.Add(btnOrganismes, 1, 1)
        tblMenu.Controls.Add(btnPays, 2, 1)
        tblMenu.Controls.Add(lblGroupePatient, 0, 2)
        tblMenu.Controls.Add(btnSituationsFamiliales, 0, 3)
        tblMenu.Controls.Add(btnLiensPatient, 1, 3)
        tblMenu.Controls.Add(btnRoleLegal, 2, 3)
        tblMenu.Controls.Add(btnRolesIntervenant, 3, 3)
        tblMenu.Controls.Add(lblGroupeDossier, 0, 4)
        tblMenu.Controls.Add(btnStatutsDossier, 0, 5)
        tblMenu.Controls.Add(btnTypesDocuments, 1, 5)
        tblMenu.Controls.Add(btnPeriodicites, 2, 5)
        tblMenu.Controls.Add(btnTextesTypes, 3, 5)
        tblMenu.Controls.Add(btnUsagesTextes, 4, 5)
        tblMenu.Controls.Add(lblGroupeSeances, 0, 6)
        tblMenu.Controls.Add(btnNaturesSeance, 0, 7)
        tblMenu.Controls.Add(btnStatutsSeance, 1, 7)
        tblMenu.Controls.Add(btnPrixSeance, 2, 7)
        tblMenu.Controls.Add(btnModesPaiement, 3, 7)
        tblMenu.Controls.Add(btnTypesRendezVous, 4, 7)
        tblMenu.Controls.Add(btnCivilites, 3, 1)
        tblMenu.Controls.Add(btnLateralites, 4, 3)
        tblMenu.Controls.Add(btnSpecialites, 4, 1)
        tblMenu.Dock = DockStyle.Fill
        tblMenu.Location = New Point(16, 121)
        tblMenu.Name = "tblMenu"
        tblMenu.Padding = New Padding(0)
        tblMenu.RowCount = 8
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0F))
        tblMenu.RowStyles.Add(New RowStyle(SizeType.Absolute, 20.0F))
        tblMenu.Size = New Size(933, 558)
        tblMenu.TabIndex = 10
        ' 
        ' lblGroupeCabinet
        ' 
        lblGroupeCabinet.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        tblMenu.SetColumnSpan(lblGroupeCabinet, 5)
        lblGroupeCabinet.Font = New Font("Calibri", 13.0F, FontStyle.Bold)
        lblGroupeCabinet.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblGroupeCabinet.Location = New Point(31, 15)
        lblGroupeCabinet.Margin = New Padding(12, 0, 3, 2)
        lblGroupeCabinet.Name = "lblGroupeCabinet"
        lblGroupeCabinet.Size = New Size(876, 22)
        lblGroupeCabinet.TabIndex = 20
        lblGroupeCabinet.Text = "Cabinet"
        ' 
        ' btnTherapeutes
        ' 
        btnTherapeutes.Anchor = AnchorStyles.None
        btnTherapeutes.BackColor = Color.Transparent
        btnTherapeutes.FlatAppearance.BorderSize = 0
        btnTherapeutes.FlatStyle = FlatStyle.Flat
        btnTherapeutes.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnTherapeutes.ForeColor = Color.White
        btnTherapeutes.Image = CType(resources.GetObject("btnTherapeutes.Image"), Image)
        btnTherapeutes.Location = New Point(22, 46)
        btnTherapeutes.Name = "btnTherapeutes"
        btnTherapeutes.Size = New Size(214, 90)
        btnTherapeutes.TabIndex = 10
        btnTherapeutes.Tag = "therapeutes_normal"
        btnTherapeutes.Text = "Thérapeutes" & vbCr
        btnTherapeutes.TextImageRelation = TextImageRelation.ImageBeforeText
        btnTherapeutes.UseCompatibleTextRendering = True
        btnTherapeutes.UseVisualStyleBackColor = False
        ' 
        ' btnTypesRendezVous
        ' 
        btnTypesRendezVous.Anchor = AnchorStyles.None
        btnTypesRendezVous.BackColor = Color.Transparent
        btnTypesRendezVous.Enabled = False
        btnTypesRendezVous.FlatAppearance.BorderSize = 0
        btnTypesRendezVous.FlatStyle = FlatStyle.Flat
        btnTypesRendezVous.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnTypesRendezVous.ForeColor = Color.White
        btnTypesRendezVous.Image = CType(resources.GetObject("btnTypesRendezVous.Image"), Image)
        btnTypesRendezVous.Location = New Point(468, 46)
        btnTypesRendezVous.Name = "btnTypesRendezVous"
        btnTypesRendezVous.Size = New Size(214, 90)
        btnTypesRendezVous.TabIndex = 7
        btnTypesRendezVous.Tag = "typeRendezvous_normal"
        btnTypesRendezVous.Text = "Types rendez-vous"
        btnTypesRendezVous.TextImageRelation = TextImageRelation.ImageBeforeText
        btnTypesRendezVous.UseCompatibleTextRendering = True
        btnTypesRendezVous.UseVisualStyleBackColor = False
        ' 
        ' btnOrganismes
        ' 
        btnOrganismes.Anchor = AnchorStyles.None
        btnOrganismes.BackColor = Color.Transparent
        btnOrganismes.Enabled = False
        btnOrganismes.FlatAppearance.BorderSize = 0
        btnOrganismes.FlatStyle = FlatStyle.Flat
        btnOrganismes.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnOrganismes.ForeColor = Color.White
        btnOrganismes.Image = CType(resources.GetObject("btnOrganismes.Image"), Image)
        btnOrganismes.Location = New Point(691, 46)
        btnOrganismes.Name = "btnOrganismes"
        btnOrganismes.Size = New Size(216, 90)
        btnOrganismes.TabIndex = 14
        btnOrganismes.Tag = "domaines_normal"
        btnOrganismes.Text = "Organismes" & vbCrLf & "et mutualités"
        btnOrganismes.TextImageRelation = TextImageRelation.ImageBeforeText
        btnOrganismes.UseCompatibleTextRendering = True
        btnOrganismes.UseVisualStyleBackColor = False
        ' 
        ' lblGroupePatient
        ' 
        lblGroupePatient.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        tblMenu.SetColumnSpan(lblGroupePatient, 5)
        lblGroupePatient.Font = New Font("Calibri", 13.0F, FontStyle.Bold)
        lblGroupePatient.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblGroupePatient.Location = New Point(31, 148)
        lblGroupePatient.Margin = New Padding(12, 0, 3, 2)
        lblGroupePatient.Name = "lblGroupePatient"
        lblGroupePatient.Size = New Size(876, 22)
        lblGroupePatient.TabIndex = 21
        lblGroupePatient.Text = "Patient et entourage"
        ' 
        ' btnSituationsFamiliales
        ' 
        btnSituationsFamiliales.Anchor = AnchorStyles.None
        btnSituationsFamiliales.BackColor = Color.Transparent
        btnSituationsFamiliales.Enabled = False
        btnSituationsFamiliales.FlatAppearance.BorderSize = 0
        btnSituationsFamiliales.FlatStyle = FlatStyle.Flat
        btnSituationsFamiliales.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnSituationsFamiliales.ForeColor = Color.White
        btnSituationsFamiliales.Image = CType(resources.GetObject("btnSituationsFamiliales.Image"), Image)
        btnSituationsFamiliales.Location = New Point(22, 179)
        btnSituationsFamiliales.Name = "btnSituationsFamiliales"
        btnSituationsFamiliales.Size = New Size(214, 90)
        btnSituationsFamiliales.TabIndex = 3
        btnSituationsFamiliales.Tag = "situationsFamiliales_normal"
        btnSituationsFamiliales.Text = "Situations familiales" & vbCr
        btnSituationsFamiliales.TextImageRelation = TextImageRelation.ImageBeforeText
        btnSituationsFamiliales.UseCompatibleTextRendering = True
        btnSituationsFamiliales.UseVisualStyleBackColor = False
        ' 
        ' btnLiensPatient
        ' 
        btnLiensPatient.Anchor = AnchorStyles.None
        btnLiensPatient.BackColor = Color.Transparent
        btnLiensPatient.Enabled = False
        btnLiensPatient.FlatAppearance.BorderSize = 0
        btnLiensPatient.FlatStyle = FlatStyle.Flat
        btnLiensPatient.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnLiensPatient.ForeColor = Color.White
        btnLiensPatient.Image = CType(resources.GetObject("btnLiensPatient.Image"), Image)
        btnLiensPatient.Location = New Point(245, 179)
        btnLiensPatient.Name = "btnLiensPatient"
        btnLiensPatient.Size = New Size(214, 90)
        btnLiensPatient.TabIndex = 1
        btnLiensPatient.Tag = "liensPatient_normal"
        btnLiensPatient.Text = "Liens patient" & vbCrLf & "Entourage"
        btnLiensPatient.TextImageRelation = TextImageRelation.ImageBeforeText
        btnLiensPatient.UseCompatibleTextRendering = True
        btnLiensPatient.UseVisualStyleBackColor = False
        ' 
        ' btnRoleLegal
        ' 
        btnRoleLegal.Anchor = AnchorStyles.None
        btnRoleLegal.BackColor = Color.Transparent
        btnRoleLegal.Enabled = False
        btnRoleLegal.FlatAppearance.BorderSize = 0
        btnRoleLegal.FlatStyle = FlatStyle.Flat
        btnRoleLegal.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnRoleLegal.ForeColor = Color.White
        btnRoleLegal.Image = CType(resources.GetObject("btnRoleLegal.Image"), Image)
        btnRoleLegal.Location = New Point(468, 179)
        btnRoleLegal.Name = "btnRoleLegal"
        btnRoleLegal.Size = New Size(214, 90)
        btnRoleLegal.TabIndex = 9
        btnRoleLegal.Tag = "rolesLegaux_normal"
        btnRoleLegal.Text = "Rôles légaux" & vbCrLf & "Contacts"
        btnRoleLegal.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRoleLegal.UseCompatibleTextRendering = True
        btnRoleLegal.UseVisualStyleBackColor = False
        ' 
        ' btnRolesIntervenant
        ' 
        btnRolesIntervenant.Anchor = AnchorStyles.None
        btnRolesIntervenant.BackColor = Color.Transparent
        btnRolesIntervenant.Enabled = False
        btnRolesIntervenant.FlatAppearance.BorderSize = 0
        btnRolesIntervenant.FlatStyle = FlatStyle.Flat
        btnRolesIntervenant.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnRolesIntervenant.ForeColor = Color.White
        btnRolesIntervenant.Image = CType(resources.GetObject("btnRolesIntervenant.Image"), Image)
        btnRolesIntervenant.Location = New Point(691, 179)
        btnRolesIntervenant.Name = "btnRolesIntervenant"
        btnRolesIntervenant.Size = New Size(216, 90)
        btnRolesIntervenant.TabIndex = 2
        btnRolesIntervenant.Tag = "roleIntervenant_normal"
        btnRolesIntervenant.Text = "Rôles intervenant" & vbCrLf
        btnRolesIntervenant.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRolesIntervenant.UseCompatibleTextRendering = True
        btnRolesIntervenant.UseVisualStyleBackColor = False
        ' 
        ' lblGroupeDossier
        ' 
        lblGroupeDossier.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        tblMenu.SetColumnSpan(lblGroupeDossier, 5)
        lblGroupeDossier.Font = New Font("Calibri", 13.0F, FontStyle.Bold)
        lblGroupeDossier.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblGroupeDossier.Location = New Point(31, 281)
        lblGroupeDossier.Margin = New Padding(12, 0, 3, 2)
        lblGroupeDossier.Name = "lblGroupeDossier"
        lblGroupeDossier.Size = New Size(876, 22)
        lblGroupeDossier.TabIndex = 22
        lblGroupeDossier.Text = "Dossier"
        ' 
        ' btnStatutsDossier
        ' 
        btnStatutsDossier.Anchor = AnchorStyles.None
        btnStatutsDossier.BackColor = Color.Transparent
        btnStatutsDossier.Enabled = False
        btnStatutsDossier.FlatAppearance.BorderSize = 0
        btnStatutsDossier.FlatStyle = FlatStyle.Flat
        btnStatutsDossier.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnStatutsDossier.ForeColor = Color.White
        btnStatutsDossier.Image = CType(resources.GetObject("btnStatutsDossier.Image"), Image)
        btnStatutsDossier.Location = New Point(22, 312)
        btnStatutsDossier.Name = "btnStatutsDossier"
        btnStatutsDossier.Size = New Size(214, 90)
        btnStatutsDossier.TabIndex = 4
        btnStatutsDossier.Tag = "statutDossier_normal"
        btnStatutsDossier.Text = "Statuts dossier" & vbCrLf
        btnStatutsDossier.TextImageRelation = TextImageRelation.ImageBeforeText
        btnStatutsDossier.UseCompatibleTextRendering = True
        btnStatutsDossier.UseVisualStyleBackColor = False
        ' 
        ' btnTypesDocuments
        ' 
        btnTypesDocuments.Anchor = AnchorStyles.None
        btnTypesDocuments.BackColor = Color.Transparent
        btnTypesDocuments.Enabled = False
        btnTypesDocuments.FlatAppearance.BorderSize = 0
        btnTypesDocuments.FlatStyle = FlatStyle.Flat
        btnTypesDocuments.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnTypesDocuments.ForeColor = Color.White
        btnTypesDocuments.Image = CType(resources.GetObject("btnTypesDocuments.Image"), Image)
        btnTypesDocuments.Location = New Point(245, 312)
        btnTypesDocuments.Name = "btnTypesDocuments"
        btnTypesDocuments.Size = New Size(214, 90)
        btnTypesDocuments.TabIndex = 6
        btnTypesDocuments.Tag = "typeDocument_normal"
        btnTypesDocuments.Text = "Types documents"
        btnTypesDocuments.TextImageRelation = TextImageRelation.ImageBeforeText
        btnTypesDocuments.UseCompatibleTextRendering = True
        btnTypesDocuments.UseVisualStyleBackColor = False
        ' 
        ' btnPeriodicites
        ' 
        btnPeriodicites.Anchor = AnchorStyles.None
        btnPeriodicites.BackColor = Color.Transparent
        btnPeriodicites.Enabled = False
        btnPeriodicites.FlatAppearance.BorderSize = 0
        btnPeriodicites.FlatStyle = FlatStyle.Flat
        btnPeriodicites.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnPeriodicites.ForeColor = Color.White
        btnPeriodicites.Image = CType(resources.GetObject("btnPeriodicites.Image"), Image)
        btnPeriodicites.Location = New Point(691, 312)
        btnPeriodicites.Name = "btnPeriodicites"
        btnPeriodicites.Size = New Size(216, 90)
        btnPeriodicites.TabIndex = 15
        btnPeriodicites.Tag = "periodicite_normal"
        btnPeriodicites.Text = "Périodicités" & vbCrLf & "d'encodage"
        btnPeriodicites.TextImageRelation = TextImageRelation.ImageBeforeText
        btnPeriodicites.UseCompatibleTextRendering = True
        btnPeriodicites.UseVisualStyleBackColor = False
        ' 
        ' btnTextesTypes
        ' 
        btnTextesTypes.Anchor = AnchorStyles.None
        btnTextesTypes.BackColor = Color.Transparent
        btnTextesTypes.Enabled = False
        btnTextesTypes.FlatAppearance.BorderSize = 0
        btnTextesTypes.FlatStyle = FlatStyle.Flat
        btnTextesTypes.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnTextesTypes.ForeColor = Color.White
        btnTextesTypes.Image = CType(resources.GetObject("btnTextesTypes.Image"), Image)
        btnTextesTypes.Location = New Point(915, 312)
        btnTextesTypes.Name = "btnTextesTypes"
        btnTextesTypes.Size = New Size(216, 90)
        btnTextesTypes.TabIndex = 24
        btnTextesTypes.Tag = "texteType_normal"
        btnTextesTypes.Text = "Textes types"
        btnTextesTypes.TextImageRelation = TextImageRelation.ImageBeforeText
        btnTextesTypes.UseCompatibleTextRendering = True
        btnTextesTypes.UseVisualStyleBackColor = False
        ' 
        ' btnUsagesTextes
        ' 
        btnUsagesTextes.Anchor = AnchorStyles.None
        btnUsagesTextes.BackColor = Color.Transparent
        btnUsagesTextes.Enabled = False
        btnUsagesTextes.FlatAppearance.BorderSize = 0
        btnUsagesTextes.FlatStyle = FlatStyle.Flat
        btnUsagesTextes.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnUsagesTextes.ForeColor = Color.White
        btnUsagesTextes.Image = CType(resources.GetObject("btnUsagesTextes.Image"), Image)
        btnUsagesTextes.Location = New Point(1139, 312)
        btnUsagesTextes.Name = "btnUsagesTextes"
        btnUsagesTextes.Size = New Size(216, 90)
        btnUsagesTextes.TabIndex = 25
        btnUsagesTextes.Tag = "usageTexte_normal"
        btnUsagesTextes.Text = "Usages des" & vbCrLf & "textes types"
        btnUsagesTextes.TextImageRelation = TextImageRelation.ImageBeforeText
        btnUsagesTextes.UseCompatibleTextRendering = True
        btnUsagesTextes.UseVisualStyleBackColor = False
        ' 
        ' lblGroupeSeances
        ' 
        lblGroupeSeances.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        tblMenu.SetColumnSpan(lblGroupeSeances, 5)
        lblGroupeSeances.Font = New Font("Calibri", 13.0F, FontStyle.Bold)
        lblGroupeSeances.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblGroupeSeances.Location = New Point(31, 414)
        lblGroupeSeances.Margin = New Padding(12, 0, 3, 2)
        lblGroupeSeances.Name = "lblGroupeSeances"
        lblGroupeSeances.Size = New Size(876, 22)
        lblGroupeSeances.TabIndex = 23
        lblGroupeSeances.Text = "Séances et facturation"
        ' 
        ' btnNaturesSeance
        ' 
        btnNaturesSeance.Anchor = AnchorStyles.None
        btnNaturesSeance.BackColor = Color.Transparent
        btnNaturesSeance.Enabled = False
        btnNaturesSeance.FlatAppearance.BorderSize = 0
        btnNaturesSeance.FlatStyle = FlatStyle.Flat
        btnNaturesSeance.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnNaturesSeance.ForeColor = Color.White
        btnNaturesSeance.Image = CType(resources.GetObject("btnNaturesSeance.Image"), Image)
        btnNaturesSeance.Location = New Point(22, 447)
        btnNaturesSeance.Name = "btnNaturesSeance"
        btnNaturesSeance.Size = New Size(214, 90)
        btnNaturesSeance.TabIndex = 13
        btnNaturesSeance.Tag = "natureSeance_normal"
        btnNaturesSeance.Text = "Natures séance"
        btnNaturesSeance.TextImageRelation = TextImageRelation.ImageBeforeText
        btnNaturesSeance.UseCompatibleTextRendering = True
        btnNaturesSeance.UseVisualStyleBackColor = False
        ' 
        ' btnStatutsSeance
        ' 
        btnStatutsSeance.Anchor = AnchorStyles.None
        btnStatutsSeance.BackColor = Color.Transparent
        btnStatutsSeance.Enabled = False
        btnStatutsSeance.FlatAppearance.BorderSize = 0
        btnStatutsSeance.FlatStyle = FlatStyle.Flat
        btnStatutsSeance.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnStatutsSeance.ForeColor = Color.White
        btnStatutsSeance.Image = CType(resources.GetObject("btnStatutsSeance.Image"), Image)
        btnStatutsSeance.Location = New Point(245, 447)
        btnStatutsSeance.Name = "btnStatutsSeance"
        btnStatutsSeance.Size = New Size(214, 90)
        btnStatutsSeance.TabIndex = 5
        btnStatutsSeance.Tag = "statutSeance_normal"
        btnStatutsSeance.Text = "Statuts séance"
        btnStatutsSeance.TextImageRelation = TextImageRelation.ImageBeforeText
        btnStatutsSeance.UseCompatibleTextRendering = True
        btnStatutsSeance.UseVisualStyleBackColor = False
        ' 
        ' btnPrixSeance
        ' 
        btnPrixSeance.Anchor = AnchorStyles.None
        btnPrixSeance.BackColor = Color.Transparent
        btnPrixSeance.Enabled = False
        btnPrixSeance.FlatAppearance.BorderSize = 0
        btnPrixSeance.FlatStyle = FlatStyle.Flat
        btnPrixSeance.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnPrixSeance.ForeColor = Color.White
        btnPrixSeance.Image = CType(resources.GetObject("btnPrixSeance.Image"), Image)
        btnPrixSeance.Location = New Point(468, 447)
        btnPrixSeance.Name = "btnPrixSeance"
        btnPrixSeance.Size = New Size(214, 90)
        btnPrixSeance.TabIndex = 12
        btnPrixSeance.Tag = "tarifSeance_normal"
        btnPrixSeance.Text = "Tarifs séance"
        btnPrixSeance.TextImageRelation = TextImageRelation.ImageBeforeText
        btnPrixSeance.UseCompatibleTextRendering = True
        btnPrixSeance.UseVisualStyleBackColor = False
        ' 
        ' btnModesPaiement
        ' 
        btnModesPaiement.Anchor = AnchorStyles.None
        btnModesPaiement.BackColor = Color.Transparent
        btnModesPaiement.Enabled = False
        btnModesPaiement.FlatAppearance.BorderSize = 0
        btnModesPaiement.FlatStyle = FlatStyle.Flat
        btnModesPaiement.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnModesPaiement.ForeColor = Color.White
        btnModesPaiement.Image = CType(resources.GetObject("btnModesPaiement.Image"), Image)
        btnModesPaiement.Location = New Point(691, 447)
        btnModesPaiement.Name = "btnModesPaiement"
        btnModesPaiement.Size = New Size(216, 90)
        btnModesPaiement.TabIndex = 11
        btnModesPaiement.Tag = "modesPaiement_normal"
        btnModesPaiement.Text = "Modes paiement"
        btnModesPaiement.TextImageRelation = TextImageRelation.ImageBeforeText
        btnModesPaiement.UseCompatibleTextRendering = True
        btnModesPaiement.UseVisualStyleBackColor = False
        ' 
        ' btnPays
        ' 
        btnPays.Anchor = AnchorStyles.None
        btnPays.BackColor = Color.Transparent
        btnPays.Enabled = False
        btnPays.FlatAppearance.BorderSize = 0
        btnPays.FlatStyle = FlatStyle.Flat
        btnPays.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnPays.ForeColor = Color.White
        btnPays.Image = CType(resources.GetObject("btnPays.Image"), Image)
        btnPays.Location = New Point(731, 46)
        btnPays.Name = "btnPays"
        btnPays.Size = New Size(216, 90)
        btnPays.TabIndex = 16
        btnPays.Tag = "pays_normal"
        btnPays.Text = "Pays" & vbCrLf & "Adresses"
        btnPays.TextImageRelation = TextImageRelation.ImageBeforeText
        btnPays.UseCompatibleTextRendering = True
        btnPays.UseVisualStyleBackColor = False
        ' 
        ' btnCivilites
        ' 
        btnCivilites.Anchor = AnchorStyles.None
        btnCivilites.BackColor = Color.Transparent
        btnCivilites.Enabled = False
        btnCivilites.FlatAppearance.BorderSize = 0
        btnCivilites.FlatStyle = FlatStyle.Flat
        btnCivilites.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnCivilites.ForeColor = Color.White
        btnCivilites.Image = CType(resources.GetObject("btnCivilites.Image"), Image)
        btnCivilites.Location = New Point(731, 179)
        btnCivilites.Name = "btnCivilites"
        btnCivilites.Size = New Size(216, 90)
        btnCivilites.TabIndex = 17
        btnCivilites.Tag = "civilites_normal"
        btnCivilites.Text = "Civilités" & vbCrLf & "Personnes"
        btnCivilites.TextImageRelation = TextImageRelation.ImageBeforeText
        btnCivilites.UseCompatibleTextRendering = True
        btnCivilites.UseVisualStyleBackColor = False
        ' 
        ' btnLateralites
        ' 
        btnLateralites.Anchor = AnchorStyles.None
        btnLateralites.BackColor = Color.Transparent
        btnLateralites.Enabled = False
        btnLateralites.FlatAppearance.BorderSize = 0
        btnLateralites.FlatStyle = FlatStyle.Flat
        btnLateralites.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnLateralites.ForeColor = Color.White
        btnLateralites.Location = New Point(731, 179)
        btnLateralites.Name = "btnLateralites"
        btnLateralites.Size = New Size(216, 90)
        btnLateralites.TabIndex = 18
        btnLateralites.Tag = "lateralite_normal"
        btnLateralites.Text = "Latéralités" & vbCrLf & "Patients"
        btnLateralites.TextImageRelation = TextImageRelation.ImageBeforeText
        btnLateralites.UseCompatibleTextRendering = True
        btnLateralites.UseVisualStyleBackColor = False
        ' 
        ' btnSpecialites
        ' 
        btnSpecialites.Anchor = AnchorStyles.None
        btnSpecialites.BackColor = Color.Transparent
        btnSpecialites.Enabled = False
        btnSpecialites.FlatAppearance.BorderSize = 0
        btnSpecialites.FlatStyle = FlatStyle.Flat
        btnSpecialites.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnSpecialites.ForeColor = Color.White
        btnSpecialites.Location = New Point(953, 46)
        btnSpecialites.Name = "btnSpecialites"
        btnSpecialites.Size = New Size(216, 90)
        btnSpecialites.TabIndex = 26
        btnSpecialites.Tag = "therapeutes_normal"
        btnSpecialites.Text = "Spécialités" & vbCrLf & "Thérapeutes"
        btnSpecialites.TextImageRelation = TextImageRelation.ImageBeforeText
        btnSpecialites.UseCompatibleTextRendering = True
        btnSpecialites.UseVisualStyleBackColor = False
        ' 
        ' pnlTop
        ' 
        pnlTop.Controls.Add(btnEleverAcces)
        pnlTop.Controls.Add(lblRoleCourant)
        pnlTop.Controls.Add(btnRetourRoleBase)
        pnlTop.Controls.Add(lblElevation)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11.0F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(8)
        pnlTop.Size = New Size(933, 105)
        pnlTop.TabIndex = 5
        ' 
        ' btnEleverAcces
        ' 
        btnEleverAcces.BackColor = Color.Transparent
        btnEleverAcces.FlatAppearance.BorderSize = 0
        btnEleverAcces.FlatStyle = FlatStyle.Flat
        btnEleverAcces.ForeColor = Color.White
        btnEleverAcces.Image = CType(resources.GetObject("btnEleverAcces.Image"), Image)
        btnEleverAcces.ImageAlign = ContentAlignment.TopLeft
        btnEleverAcces.Location = New Point(471, 7)
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
        lblRoleCourant.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        lblRoleCourant.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblRoleCourant.Location = New Point(280, 65)
        lblRoleCourant.Name = "lblRoleCourant"
        lblRoleCourant.Size = New Size(185, 26)
        lblRoleCourant.TabIndex = 10
        lblRoleCourant.Text = "Rôle courant"
        lblRoleCourant.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' btnRetourRoleBase
        ' 
        btnRetourRoleBase.BackColor = Color.Transparent
        btnRetourRoleBase.FlatAppearance.BorderSize = 0
        btnRetourRoleBase.FlatStyle = FlatStyle.Flat
        btnRetourRoleBase.ForeColor = Color.White
        btnRetourRoleBase.Image = CType(resources.GetObject("btnRetourRoleBase.Image"), Image)
        btnRetourRoleBase.ImageAlign = ContentAlignment.TopLeft
        btnRetourRoleBase.Location = New Point(471, 57)
        btnRetourRoleBase.Name = "btnRetourRoleBase"
        btnRetourRoleBase.Size = New Size(220, 44)
        btnRetourRoleBase.TabIndex = 13
        btnRetourRoleBase.Tag = "retourRole_normal"
        btnRetourRoleBase.Variante = VarianteBouton.Clair
        btnRetourRoleBase.Text = "Retour rôle de base"
        btnRetourRoleBase.TextImageRelation = TextImageRelation.ImageBeforeText
        btnRetourRoleBase.UseVisualStyleBackColor = False
        ' 
        ' lblElevation
        ' 
        lblElevation.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        lblElevation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblElevation.Location = New Point(280, 15)
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
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 4
        enTete.SousTitre = "Gérez les valeurs de référence"
        enTete.Text = "Hub Référentiels"
        ' 
        ' UC_ReferentielHome
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10.0F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_ReferentielHome"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        carteMenu.ResumeLayout(False)
        tblMenu.ResumeLayout(False)
        pnlTop.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents carteMenu As AltheaCarte
    Friend WithEvents tblMenu As TableLayoutPanel
    Friend WithEvents btnLiensPatient As AltheaTuile
    Friend WithEvents btnRolesIntervenant As AltheaTuile
    Friend WithEvents btnSituationsFamiliales As AltheaTuile
    Friend WithEvents btnStatutsDossier As AltheaTuile
    Friend WithEvents btnStatutsSeance As AltheaTuile
    Friend WithEvents btnTypesDocuments As AltheaTuile
    Friend WithEvents btnTypesRendezVous As AltheaTuile
    Friend WithEvents btnRoleLegal As AltheaTuile
    Friend WithEvents btnOrganismes As AltheaTuile
    Friend WithEvents btnTherapeutes As AltheaTuile
    Friend WithEvents btnModesPaiement As AltheaTuile
    Friend WithEvents btnPeriodicites As AltheaTuile
    Friend WithEvents btnTextesTypes As AltheaTuile
    Friend WithEvents btnUsagesTextes As AltheaTuile
    Friend WithEvents btnPrixSeance As AltheaTuile
    Friend WithEvents btnNaturesSeance As AltheaTuile
    Friend WithEvents btnPays As AltheaTuile
    Friend WithEvents btnCivilites As AltheaTuile
    Friend WithEvents btnLateralites As AltheaTuile
    Friend WithEvents btnSpecialites As AltheaTuile
    Friend WithEvents pnlTop As AltheaCarte
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents ttMenu As ToolTip
    Friend WithEvents lblGroupeCabinet As AltheaTitreSection
    Friend WithEvents lblGroupePatient As AltheaTitreSection
    Friend WithEvents lblGroupeDossier As AltheaTitreSection
    Friend WithEvents lblGroupeSeances As AltheaTitreSection
    Friend WithEvents btnEleverAcces As AltheaBouton
    Friend WithEvents lblRoleCourant As Label
    Friend WithEvents btnRetourRoleBase As AltheaBouton
    Friend WithEvents lblElevation As Label

End Class
