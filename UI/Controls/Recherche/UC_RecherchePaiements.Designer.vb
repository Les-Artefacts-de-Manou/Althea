' -------------------------------------------------------------------------------------------------
' Designer    : UC_RecherchePaiements
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Structure visuelle de la recherche des paiements d'une période (ADR-52, R3) : en-tête, carte
' « Période » (dates, raccourcis, filtre, mode, texte), carte « Paiements » (résumé cliquable,
' grille, totaux par mode) et barre d'actions (retour, ouvrir).
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_RecherchePaiements
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_RecherchePaiements))
        pnlForm = New Panel()
        carteResultats = New AltheaCarte()
        dgvPaiements = New DataGridView()
        colDate = New DataGridViewTextBoxColumn()
        colPatient = New DataGridViewTextBoxColumn()
        colMouvement = New DataGridViewTextBoxColumn()
        colPour = New DataGridViewTextBoxColumn()
        colMode = New DataGridViewTextBoxColumn()
        colMontant = New DataGridViewTextBoxColumn()
        colEtat = New DataGridViewTextBoxColumn()
        colCode = New DataGridViewTextBoxColumn()
        colDossier = New DataGridViewTextBoxColumn()
        pnlResume = New FlowLayoutPanel()
        lblTotaux = New Label()
        carteCriteres = New AltheaCarte()
        lblDu = New Label()
        dtpDu = New AltheaDate()
        lblAu = New Label()
        dtpAu = New AltheaDate()
        btnPrecedent = New AltheaBouton()
        btnSuivant = New AltheaBouton()
        btnAujourdhui = New AltheaBouton()
        btnSemaine = New AltheaBouton()
        btnMois = New AltheaBouton()
        cboFiltre = New AltheaListe()
        cboMode = New AltheaListe()
        txtFiltre = New AltheaRecherche()
        lblMessage = New Label()
        pnlActions = New AltheaBarreActions()
        btnRetour = New AltheaBouton()
        btnOuvrir = New AltheaBouton()
        btnOuvrirPatient = New AltheaBouton()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        carteResultats.SuspendLayout()
        CType(dgvPaiements, ComponentModel.ISupportInitialize).BeginInit()
        carteCriteres.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.Controls.Add(carteResultats)
        pnlForm.Controls.Add(pnlActions)
        pnlForm.Controls.Add(carteCriteres)
        pnlForm.Controls.Add(enTete)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16, 16, 16, 16)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 0
        ' 
        ' carteResultats
        ' 
        carteResultats.Controls.Add(dgvPaiements)
        carteResultats.Controls.Add(lblTotaux)
        carteResultats.Controls.Add(pnlResume)
        carteResultats.Dock = DockStyle.Fill
        carteResultats.Location = New Point(16, 194)
        carteResultats.Name = "carteResultats"
        carteResultats.Padding = New Padding(6, 2, 6, 6)
        carteResultats.Size = New Size(965, 509)
        carteResultats.TabIndex = 2
        carteResultats.Titre = "Paiements"
        ' 
        ' dgvPaiements
        ' 
        dgvPaiements.AllowUserToAddRows = False
        dgvPaiements.AllowUserToDeleteRows = False
        dgvPaiements.AllowUserToResizeRows = False
        dgvPaiements.AutoGenerateColumns = False
        dgvPaiements.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvPaiements.Columns.AddRange(New DataGridViewColumn() {colDate, colPatient, colMouvement, colPour, colMode, colMontant, colEtat, colCode, colDossier})
        dgvPaiements.Dock = DockStyle.Fill
        dgvPaiements.Location = New Point(6, 70)
        dgvPaiements.MultiSelect = False
        dgvPaiements.Name = "dgvPaiements"
        dgvPaiements.ReadOnly = True
        dgvPaiements.RowHeadersVisible = False
        dgvPaiements.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPaiements.Size = New Size(953, 403)
        dgvPaiements.TabIndex = 1
        ' 
        ' colDate
        ' 
        colDate.DataPropertyName = "DateMouvement"
        colDate.HeaderText = "Date"
        colDate.Name = "colDate"
        colDate.ReadOnly = True
        colDate.Width = 110
        ' 
        ' colPatient
        ' 
        colPatient.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colPatient.DataPropertyName = "PatientAffichage"
        colPatient.HeaderText = "Patient"
        colPatient.MinimumWidth = 160
        colPatient.Name = "colPatient"
        colPatient.ReadOnly = True
        ' 
        ' colMouvement
        ' 
        colMouvement.DataPropertyName = "MouvementCourt"
        colMouvement.HeaderText = "Mouvement"
        colMouvement.Name = "colMouvement"
        colMouvement.ReadOnly = True
        colMouvement.Width = 140
        ' 
        ' colPour
        ' 
        colPour.DataPropertyName = "PourAffichage"
        colPour.HeaderText = "Pour"
        colPour.Name = "colPour"
        colPour.ReadOnly = True
        colPour.Width = 220
        ' 
        ' colMode
        ' 
        colMode.DataPropertyName = "ModePaiement"
        colMode.HeaderText = "Mode"
        colMode.Name = "colMode"
        colMode.ReadOnly = True
        colMode.Width = 110
        ' 
        ' colMontant
        ' 
        colMontant.DataPropertyName = "MontantLigne"
        colMontant.HeaderText = "Montant"
        colMontant.Name = "colMontant"
        colMontant.ReadOnly = True
        colMontant.Width = 85
        ' 
        ' colEtat
        ' 
        colEtat.DataPropertyName = "EtatAffichage"
        colEtat.HeaderText = "État"
        colEtat.Name = "colEtat"
        colEtat.ReadOnly = True
        colEtat.Width = 200
        ' 
        ' colCode
        ' 
        colCode.DataPropertyName = "CodePaiement"
        colCode.HeaderText = "Code"
        colCode.Name = "colCode"
        colCode.ReadOnly = True
        colCode.Width = 90
        ' 
        ' colDossier
        ' 
        colDossier.DataPropertyName = "CodeDossier"
        colDossier.HeaderText = "Dossier"
        colDossier.Name = "colDossier"
        colDossier.ReadOnly = True
        colDossier.Width = 85
        ' 
        ' pnlResume
        ' 
        pnlResume.BackColor = Color.Transparent
        pnlResume.Dock = DockStyle.Top
        pnlResume.Location = New Point(6, 36)
        pnlResume.Name = "pnlResume"
        pnlResume.Padding = New Padding(6, 2, 6, 2)
        pnlResume.Size = New Size(953, 34)
        pnlResume.TabIndex = 0
        pnlResume.WrapContents = False
        ' 
        ' lblTotaux
        ' 
        lblTotaux.BackColor = Color.Transparent
        lblTotaux.Dock = DockStyle.Bottom
        lblTotaux.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTotaux.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblTotaux.Location = New Point(6, 473)
        lblTotaux.Name = "lblTotaux"
        lblTotaux.Padding = New Padding(8, 0, 8, 0)
        lblTotaux.Size = New Size(953, 30)
        lblTotaux.TabIndex = 2
        lblTotaux.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' carteCriteres
        ' 
        carteCriteres.Controls.Add(lblDu)
        carteCriteres.Controls.Add(dtpDu)
        carteCriteres.Controls.Add(lblAu)
        carteCriteres.Controls.Add(dtpAu)
        carteCriteres.Controls.Add(btnPrecedent)
        carteCriteres.Controls.Add(btnSuivant)
        carteCriteres.Controls.Add(btnAujourdhui)
        carteCriteres.Controls.Add(btnSemaine)
        carteCriteres.Controls.Add(btnMois)
        carteCriteres.Controls.Add(cboFiltre)
        carteCriteres.Controls.Add(cboMode)
        carteCriteres.Controls.Add(txtFiltre)
        carteCriteres.Controls.Add(lblMessage)
        carteCriteres.Dock = DockStyle.Top
        carteCriteres.Location = New Point(16, 74)
        carteCriteres.Name = "carteCriteres"
        carteCriteres.Padding = New Padding(12, 8, 12, 10)
        carteCriteres.Size = New Size(965, 120)
        carteCriteres.TabIndex = 1
        carteCriteres.Titre = "Période"
        ' 
        ' lblDu
        ' 
        lblDu.AutoSize = True
        lblDu.BackColor = Color.Transparent
        lblDu.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDu.Location = New Point(16, 46)
        lblDu.Name = "lblDu"
        lblDu.Size = New Size(22, 17)
        lblDu.TabIndex = 0
        lblDu.Text = "Du"
        ' 
        ' dtpDu
        ' 
        dtpDu.Font = New Font("Calibri", 11F)
        dtpDu.Location = New Point(44, 40)
        dtpDu.Name = "dtpDu"
        dtpDu.Size = New Size(120, 30)
        dtpDu.TabIndex = 1
        ' 
        ' lblAu
        ' 
        lblAu.AutoSize = True
        lblAu.BackColor = Color.Transparent
        lblAu.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblAu.Location = New Point(172, 46)
        lblAu.Name = "lblAu"
        lblAu.Size = New Size(20, 17)
        lblAu.TabIndex = 2
        lblAu.Text = "au"
        ' 
        ' dtpAu
        ' 
        dtpAu.Font = New Font("Calibri", 11F)
        dtpAu.Location = New Point(198, 40)
        dtpAu.Name = "dtpAu"
        dtpAu.Size = New Size(120, 30)
        dtpAu.TabIndex = 3
        ' 
        ' btnPrecedent
        ' 
        btnPrecedent.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnPrecedent.Location = New Point(332, 40)
        btnPrecedent.Name = "btnPrecedent"
        btnPrecedent.Size = New Size(36, 30)
        btnPrecedent.TabIndex = 4
        btnPrecedent.Text = "‹"
        btnPrecedent.Variante = VarianteBouton.Outil
        ' 
        ' btnSuivant
        ' 
        btnSuivant.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnSuivant.Location = New Point(372, 40)
        btnSuivant.Name = "btnSuivant"
        btnSuivant.Size = New Size(36, 30)
        btnSuivant.TabIndex = 5
        btnSuivant.Text = "›"
        btnSuivant.Variante = VarianteBouton.Outil
        ' 
        ' btnAujourdhui
        ' 
        btnAujourdhui.Location = New Point(422, 40)
        btnAujourdhui.Name = "btnAujourdhui"
        btnAujourdhui.Size = New Size(110, 30)
        btnAujourdhui.TabIndex = 6
        btnAujourdhui.Text = "Aujourd'hui"
        btnAujourdhui.Variante = VarianteBouton.Outil
        ' 
        ' btnSemaine
        ' 
        btnSemaine.Location = New Point(538, 40)
        btnSemaine.Name = "btnSemaine"
        btnSemaine.Size = New Size(120, 30)
        btnSemaine.TabIndex = 7
        btnSemaine.Text = "Cette semaine"
        btnSemaine.Variante = VarianteBouton.Outil
        ' 
        ' btnMois
        ' 
        btnMois.Location = New Point(664, 40)
        btnMois.Name = "btnMois"
        btnMois.Size = New Size(100, 30)
        btnMois.TabIndex = 8
        btnMois.Text = "Ce mois"
        btnMois.Variante = VarianteBouton.Outil
        ' 
        ' cboFiltre
        ' 
        cboFiltre.Etiquette = "Filtre"
        cboFiltre.Location = New Point(16, 78)
        cboFiltre.Name = "cboFiltre"
        cboFiltre.Size = New Size(250, 30)
        cboFiltre.TabIndex = 9
        ' 
        ' cboMode
        ' 
        cboMode.Etiquette = "Mode"
        cboMode.Location = New Point(278, 78)
        cboMode.Name = "cboMode"
        cboMode.Size = New Size(220, 30)
        cboMode.TabIndex = 10
        ' 
        ' txtFiltre
        ' 
        txtFiltre.Location = New Point(510, 77)
        txtFiltre.MaxLength = 50
        txtFiltre.Name = "txtFiltre"
        txtFiltre.Size = New Size(240, 32)
        txtFiltre.TabIndex = 11
        txtFiltre.TexteAide = "Patient, code, séance, mode…"
        ' 
        ' lblMessage
        ' 
        lblMessage.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblMessage.BackColor = Color.Transparent
        lblMessage.Font = New Font("Calibri", 10.5F, FontStyle.Italic)
        lblMessage.ForeColor = Color.FromArgb(CByte(110), CByte(110), CByte(110))
        lblMessage.Location = New Point(762, 84)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(190, 22)
        lblMessage.TabIndex = 12
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnRetour)
        pnlActions.Controls.Add(btnOuvrir)
        pnlActions.Controls.Add(btnOuvrirPatient)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(16, 703)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(965, 52)
        pnlActions.TabIndex = 3
        ' 
        ' btnRetour
        ' 
        btnRetour.Image = CType(resources.GetObject("btnRetour.Image"), Image)
        btnRetour.Location = New Point(16, 7)
        btnRetour.Name = "btnRetour"
        btnRetour.Size = New Size(190, 38)
        btnRetour.TabIndex = 0
        btnRetour.Text = "Retour à la recherche"
        btnRetour.Variante = VarianteBouton.Clair
        ' 
        ' btnOuvrir
        ' 
        btnOuvrir.Location = New Point(222, 7)
        btnOuvrir.Name = "btnOuvrir"
        btnOuvrir.Size = New Size(130, 38)
        btnOuvrir.TabIndex = 1
        btnOuvrir.Text = "Ouvrir"
        btnOuvrir.Variante = VarianteBouton.OutilPrincipal
        ' 
        ' btnOuvrirPatient
        ' 
        btnOuvrirPatient.Location = New Point(362, 7)
        btnOuvrirPatient.Name = "btnOuvrirPatient"
        btnOuvrirPatient.Size = New Size(170, 38)
        btnOuvrirPatient.TabIndex = 2
        btnOuvrirPatient.Text = "Ouvrir le patient"
        btnOuvrirPatient.Variante = VarianteBouton.Outil
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 0
        enTete.SousTitre = "Les paiements d'un jour ou d'une période, tous patients confondus"
        enTete.Text = "Paiements"
        ' 
        ' UC_RecherchePaiements
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_RecherchePaiements"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        carteResultats.ResumeLayout(False)
        CType(dgvPaiements, ComponentModel.ISupportInitialize).EndInit()
        carteCriteres.ResumeLayout(False)
        carteCriteres.PerformLayout()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteCriteres As AltheaCarte
    Friend WithEvents lblDu As Label
    Friend WithEvents dtpDu As AltheaDate
    Friend WithEvents lblAu As Label
    Friend WithEvents dtpAu As AltheaDate
    Friend WithEvents btnPrecedent As AltheaBouton
    Friend WithEvents btnSuivant As AltheaBouton
    Friend WithEvents btnAujourdhui As AltheaBouton
    Friend WithEvents btnSemaine As AltheaBouton
    Friend WithEvents btnMois As AltheaBouton
    Friend WithEvents cboFiltre As AltheaListe
    Friend WithEvents cboMode As AltheaListe
    Friend WithEvents txtFiltre As AltheaRecherche
    Friend WithEvents lblMessage As Label
    Friend WithEvents carteResultats As AltheaCarte
    Friend WithEvents pnlResume As FlowLayoutPanel
    Friend WithEvents lblTotaux As Label
    Friend WithEvents dgvPaiements As DataGridView
    Friend WithEvents colDate As DataGridViewTextBoxColumn
    Friend WithEvents colPatient As DataGridViewTextBoxColumn
    Friend WithEvents colMouvement As DataGridViewTextBoxColumn
    Friend WithEvents colPour As DataGridViewTextBoxColumn
    Friend WithEvents colMode As DataGridViewTextBoxColumn
    Friend WithEvents colMontant As DataGridViewTextBoxColumn
    Friend WithEvents colEtat As DataGridViewTextBoxColumn
    Friend WithEvents colCode As DataGridViewTextBoxColumn
    Friend WithEvents colDossier As DataGridViewTextBoxColumn
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnRetour As AltheaBouton
    Friend WithEvents btnOuvrir As AltheaBouton
    Friend WithEvents btnOuvrirPatient As AltheaBouton

End Class
