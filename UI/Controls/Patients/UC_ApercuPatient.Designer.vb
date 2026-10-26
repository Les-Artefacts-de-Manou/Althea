' -------------------------------------------------------------------------------------------------
' Fichier     : UC_ApercuPatient.Designer.vb
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 17/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Code généré (layout) du panneau d'aperçu patient : une carte qui défile, sections empilées
' (identité, alerte, aujourd'hui, séances, dossier, finances, contacts, documents).
'
' Remarques   :
' - flpContenu (FlowLayoutPanel vertical) empile les sections ; leur largeur suit le panneau
'   (UC_ApercuPatient.AjusterLargeurs).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_ApercuPatient
    Inherits UserControl

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
        carteApercu = New AltheaCarte()
        flpContenu = New FlowLayoutPanel()
        lblVide = New Label()
        pnlIdentite = New Panel()
        picPhoto = New PictureBox()
        lblNom = New Label()
        lblInfos = New Label()
        flpBoutons = New FlowLayoutPanel()
        btnOuvrirFiche = New AltheaBouton()
        btnOuvrirDossier = New AltheaBouton()
        lblAlerte = New Label()
        titreAujourdhui = New AltheaTitreSection()
        lblSeanceJour = New Label()
        btnNotesSeance = New AltheaBouton()
        titreSeances = New AltheaTitreSection()
        dgvSeancesApercu = New DataGridView()
        colApDate = New DataGridViewTextBoxColumn()
        colApLibelle = New DataGridViewTextBoxColumn()
        colApEtat = New DataGridViewTextBoxColumn()
        titreDossier = New AltheaTitreSection()
        lblDossier = New Label()
        titreFinances = New AltheaTitreSection()
        lblFinances = New Label()
        titreContacts = New AltheaTitreSection()
        lblContacts = New Label()
        titreDocuments = New AltheaTitreSection()
        lblDocuments = New Label()
        carteApercu.SuspendLayout()
        flpContenu.SuspendLayout()
        pnlIdentite.SuspendLayout()
        CType(picPhoto, System.ComponentModel.ISupportInitialize).BeginInit()
        flpBoutons.SuspendLayout()
        CType(dgvSeancesApercu, System.ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        '
        ' carteApercu
        '
        carteApercu.Controls.Add(flpContenu)
        carteApercu.Dock = DockStyle.Fill
        carteApercu.Location = New Point(12, 0)
        carteApercu.Name = "carteApercu"
        carteApercu.Padding = New Padding(4)
        carteApercu.Size = New Size(388, 560)
        carteApercu.TabIndex = 0
        '
        ' flpContenu
        '
        flpContenu.AutoScroll = True
        flpContenu.BackColor = Color.Transparent
        flpContenu.Controls.Add(lblVide)
        flpContenu.Controls.Add(pnlIdentite)
        flpContenu.Controls.Add(flpBoutons)
        flpContenu.Controls.Add(lblAlerte)
        flpContenu.Controls.Add(titreAujourdhui)
        flpContenu.Controls.Add(lblSeanceJour)
        flpContenu.Controls.Add(btnNotesSeance)
        flpContenu.Controls.Add(titreSeances)
        flpContenu.Controls.Add(dgvSeancesApercu)
        flpContenu.Controls.Add(titreDossier)
        flpContenu.Controls.Add(lblDossier)
        flpContenu.Controls.Add(titreFinances)
        flpContenu.Controls.Add(lblFinances)
        flpContenu.Controls.Add(titreContacts)
        flpContenu.Controls.Add(lblContacts)
        flpContenu.Controls.Add(titreDocuments)
        flpContenu.Controls.Add(lblDocuments)
        flpContenu.Dock = DockStyle.Fill
        flpContenu.FlowDirection = FlowDirection.TopDown
        flpContenu.Location = New Point(4, 4)
        flpContenu.Name = "flpContenu"
        flpContenu.Padding = New Padding(8, 6, 4, 8)
        flpContenu.Size = New Size(380, 552)
        flpContenu.TabIndex = 0
        flpContenu.WrapContents = False
        '
        ' lblVide
        '
        lblVide.Font = New Font("Calibri", 10F, FontStyle.Italic)
        lblVide.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblVide.Margin = New Padding(0, 6, 0, 6)
        lblVide.Name = "lblVide"
        lblVide.Size = New Size(350, 40)
        lblVide.TabIndex = 0
        lblVide.Text = "Cliquez sur un patient pour voir son aperçu."
        '
        ' pnlIdentite
        '
        pnlIdentite.Controls.Add(picPhoto)
        pnlIdentite.Controls.Add(lblNom)
        pnlIdentite.Controls.Add(lblInfos)
        pnlIdentite.Margin = New Padding(0, 0, 0, 4)
        pnlIdentite.Name = "pnlIdentite"
        pnlIdentite.Size = New Size(350, 60)
        pnlIdentite.TabIndex = 1
        '
        ' picPhoto
        '
        picPhoto.BackColor = Color.FromArgb(CByte(230), CByte(222), CByte(226))
        picPhoto.Location = New Point(0, 4)
        picPhoto.Name = "picPhoto"
        picPhoto.Size = New Size(52, 52)
        picPhoto.SizeMode = PictureBoxSizeMode.Zoom
        picPhoto.TabIndex = 0
        picPhoto.TabStop = False
        '
        ' lblNom
        '
        lblNom.AutoEllipsis = True
        lblNom.Font = New Font("Calibri", 13F, FontStyle.Bold)
        lblNom.ForeColor = Color.FromArgb(CByte(126), CByte(95), CByte(112))
        lblNom.Location = New Point(62, 6)
        lblNom.Name = "lblNom"
        lblNom.Size = New Size(288, 24)
        lblNom.TabIndex = 1
        lblNom.Text = "Nom Prénom"
        '
        ' lblInfos
        '
        lblInfos.AutoEllipsis = True
        lblInfos.Font = New Font("Calibri", 9.75F)
        lblInfos.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblInfos.Location = New Point(62, 32)
        lblInfos.Name = "lblInfos"
        lblInfos.Size = New Size(288, 20)
        lblInfos.TabIndex = 2
        lblInfos.Text = "PA000000 · âge"
        '
        ' flpBoutons
        '
        flpBoutons.AutoSize = True
        flpBoutons.Controls.Add(btnOuvrirFiche)
        flpBoutons.Controls.Add(btnOuvrirDossier)
        flpBoutons.Margin = New Padding(0, 0, 0, 6)
        flpBoutons.MinimumSize = New Size(350, 0)
        flpBoutons.Name = "flpBoutons"
        flpBoutons.Size = New Size(350, 30)
        flpBoutons.TabIndex = 2
        '
        ' btnOuvrirFiche
        '
        btnOuvrirFiche.Margin = New Padding(0, 2, 6, 2)
        btnOuvrirFiche.Name = "btnOuvrirFiche"
        btnOuvrirFiche.Size = New Size(130, 26)
        btnOuvrirFiche.TabIndex = 0
        btnOuvrirFiche.Text = "Ouvrir la fiche"
        btnOuvrirFiche.Variante = VarianteBouton.Clair
        '
        ' btnOuvrirDossier
        '
        btnOuvrirDossier.Margin = New Padding(0, 2, 6, 2)
        btnOuvrirDossier.Name = "btnOuvrirDossier"
        btnOuvrirDossier.Size = New Size(190, 26)
        btnOuvrirDossier.TabIndex = 1
        btnOuvrirDossier.Text = "Ouvrir le dossier"
        btnOuvrirDossier.Variante = VarianteBouton.Clair
        '
        ' lblAlerte
        '
        lblAlerte.AutoSize = True
        lblAlerte.Font = New Font("Calibri", 10F)
        lblAlerte.Margin = New Padding(0, 2, 0, 4)
        lblAlerte.MaximumSize = New Size(350, 0)
        lblAlerte.MinimumSize = New Size(350, 0)
        lblAlerte.Name = "lblAlerte"
        lblAlerte.Padding = New Padding(8, 6, 8, 6)
        lblAlerte.Size = New Size(350, 29)
        lblAlerte.TabIndex = 3
        lblAlerte.Text = "Alerte"
        '
        ' titreAujourdhui
        '
        titreAujourdhui.Font = New Font("Calibri", 10F, FontStyle.Bold)
        titreAujourdhui.Margin = New Padding(0, 8, 0, 2)
        titreAujourdhui.Name = "titreAujourdhui"
        titreAujourdhui.Size = New Size(350, 20)
        titreAujourdhui.TabIndex = 4
        titreAujourdhui.Text = "Aujourd'hui"
        '
        ' lblSeanceJour
        '
        lblSeanceJour.AutoSize = True
        lblSeanceJour.Font = New Font("Calibri", 10F)
        lblSeanceJour.Margin = New Padding(0, 2, 0, 2)
        lblSeanceJour.MaximumSize = New Size(350, 0)
        lblSeanceJour.Name = "lblSeanceJour"
        lblSeanceJour.Size = New Size(100, 17)
        lblSeanceJour.TabIndex = 5
        lblSeanceJour.Text = "14:00 · SE…"
        '
        ' btnNotesSeance
        '
        btnNotesSeance.Margin = New Padding(0, 4, 0, 2)
        btnNotesSeance.Name = "btnNotesSeance"
        btnNotesSeance.Size = New Size(170, 28)
        btnNotesSeance.TabIndex = 6
        btnNotesSeance.Text = "Notes de la séance"
        '
        ' titreSeances
        '
        titreSeances.Font = New Font("Calibri", 10F, FontStyle.Bold)
        titreSeances.Margin = New Padding(0, 10, 0, 2)
        titreSeances.Name = "titreSeances"
        titreSeances.Size = New Size(350, 20)
        titreSeances.TabIndex = 7
        titreSeances.Text = "Séances"
        '
        ' dgvSeancesApercu
        '
        dgvSeancesApercu.AllowUserToAddRows = False
        dgvSeancesApercu.AllowUserToDeleteRows = False
        dgvSeancesApercu.AllowUserToResizeRows = False
        dgvSeancesApercu.BorderStyle = BorderStyle.None
        dgvSeancesApercu.ColumnHeadersVisible = False
        dgvSeancesApercu.Columns.AddRange(New DataGridViewColumn() {colApDate, colApLibelle, colApEtat})
        dgvSeancesApercu.Margin = New Padding(0, 2, 0, 2)
        dgvSeancesApercu.MultiSelect = False
        dgvSeancesApercu.Name = "dgvSeancesApercu"
        dgvSeancesApercu.ReadOnly = True
        dgvSeancesApercu.RowHeadersVisible = False
        dgvSeancesApercu.ScrollBars = ScrollBars.None
        dgvSeancesApercu.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvSeancesApercu.Size = New Size(350, 140)
        dgvSeancesApercu.TabIndex = 8
        '
        ' colApDate
        '
        colApDate.HeaderText = "Date"
        colApDate.Name = "colApDate"
        colApDate.ReadOnly = True
        colApDate.Width = 82
        '
        ' colApLibelle
        '
        colApLibelle.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colApLibelle.HeaderText = "Séance"
        colApLibelle.Name = "colApLibelle"
        colApLibelle.ReadOnly = True
        '
        ' colApEtat
        '
        colApEtat.HeaderText = "État"
        colApEtat.Name = "colApEtat"
        colApEtat.ReadOnly = True
        colApEtat.Width = 118
        '
        ' titreDossier
        '
        titreDossier.Font = New Font("Calibri", 10F, FontStyle.Bold)
        titreDossier.Margin = New Padding(0, 10, 0, 2)
        titreDossier.Name = "titreDossier"
        titreDossier.Size = New Size(350, 20)
        titreDossier.TabIndex = 9
        titreDossier.Text = "Dossier"
        '
        ' lblDossier
        '
        lblDossier.AutoSize = True
        lblDossier.Font = New Font("Calibri", 10F)
        lblDossier.Margin = New Padding(0, 2, 0, 2)
        lblDossier.MaximumSize = New Size(350, 0)
        lblDossier.Name = "lblDossier"
        lblDossier.Size = New Size(50, 17)
        lblDossier.TabIndex = 10
        lblDossier.Text = "Dossier"
        '
        ' titreFinances
        '
        titreFinances.Font = New Font("Calibri", 10F, FontStyle.Bold)
        titreFinances.Margin = New Padding(0, 10, 0, 2)
        titreFinances.Name = "titreFinances"
        titreFinances.Size = New Size(350, 20)
        titreFinances.TabIndex = 11
        titreFinances.Text = "Finances"
        '
        ' lblFinances
        '
        lblFinances.AutoSize = True
        lblFinances.Font = New Font("Calibri", 10F)
        lblFinances.Margin = New Padding(0, 2, 0, 2)
        lblFinances.MaximumSize = New Size(350, 0)
        lblFinances.Name = "lblFinances"
        lblFinances.Size = New Size(50, 17)
        lblFinances.TabIndex = 12
        lblFinances.Text = "À jour"
        '
        ' titreContacts
        '
        titreContacts.Font = New Font("Calibri", 10F, FontStyle.Bold)
        titreContacts.Margin = New Padding(0, 10, 0, 2)
        titreContacts.Name = "titreContacts"
        titreContacts.Size = New Size(350, 20)
        titreContacts.TabIndex = 13
        titreContacts.Text = "Contacts"
        '
        ' lblContacts
        '
        lblContacts.AutoSize = True
        lblContacts.Font = New Font("Calibri", 10F)
        lblContacts.Margin = New Padding(0, 2, 0, 2)
        lblContacts.MaximumSize = New Size(350, 0)
        lblContacts.Name = "lblContacts"
        lblContacts.Size = New Size(50, 17)
        lblContacts.TabIndex = 14
        lblContacts.Text = "—"
        '
        ' titreDocuments
        '
        titreDocuments.Font = New Font("Calibri", 10F, FontStyle.Bold)
        titreDocuments.Margin = New Padding(0, 10, 0, 2)
        titreDocuments.Name = "titreDocuments"
        titreDocuments.Size = New Size(350, 20)
        titreDocuments.TabIndex = 15
        titreDocuments.Text = "Documents récents"
        '
        ' lblDocuments
        '
        lblDocuments.AutoSize = True
        lblDocuments.Font = New Font("Calibri", 9.75F, FontStyle.Italic)
        lblDocuments.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblDocuments.Margin = New Padding(0, 2, 0, 8)
        lblDocuments.MaximumSize = New Size(350, 0)
        lblDocuments.Name = "lblDocuments"
        lblDocuments.Size = New Size(300, 17)
        lblDocuments.TabIndex = 16
        lblDocuments.Text = "Avec la gestion documentaire : derniers documents et leur aperçu PDF."
        '
        ' UC_ApercuPatient
        '
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Transparent
        Controls.Add(carteApercu)
        Font = New Font("Calibri", 10F)
        Name = "UC_ApercuPatient"
        Padding = New Padding(12, 0, 0, 0)
        Size = New Size(400, 560)
        carteApercu.ResumeLayout(False)
        flpContenu.ResumeLayout(False)
        flpContenu.PerformLayout()
        pnlIdentite.ResumeLayout(False)
        CType(picPhoto, System.ComponentModel.ISupportInitialize).EndInit()
        flpBoutons.ResumeLayout(False)
        CType(dgvSeancesApercu, System.ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub

    Friend WithEvents carteApercu As AltheaCarte
    Friend WithEvents flpContenu As FlowLayoutPanel
    Friend WithEvents lblVide As Label
    Friend WithEvents pnlIdentite As Panel
    Friend WithEvents picPhoto As PictureBox
    Friend WithEvents lblNom As Label
    Friend WithEvents lblInfos As Label
    Friend WithEvents flpBoutons As FlowLayoutPanel
    Friend WithEvents btnOuvrirFiche As AltheaBouton
    Friend WithEvents btnOuvrirDossier As AltheaBouton
    Friend WithEvents lblAlerte As Label
    Friend WithEvents titreAujourdhui As AltheaTitreSection
    Friend WithEvents lblSeanceJour As Label
    Friend WithEvents btnNotesSeance As AltheaBouton
    Friend WithEvents titreSeances As AltheaTitreSection
    Friend WithEvents dgvSeancesApercu As DataGridView
    Friend WithEvents colApDate As DataGridViewTextBoxColumn
    Friend WithEvents colApLibelle As DataGridViewTextBoxColumn
    Friend WithEvents colApEtat As DataGridViewTextBoxColumn
    Friend WithEvents titreDossier As AltheaTitreSection
    Friend WithEvents lblDossier As Label
    Friend WithEvents titreFinances As AltheaTitreSection
    Friend WithEvents lblFinances As Label
    Friend WithEvents titreContacts As AltheaTitreSection
    Friend WithEvents lblContacts As Label
    Friend WithEvents titreDocuments As AltheaTitreSection
    Friend WithEvents lblDocuments As Label

End Class
