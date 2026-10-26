' -------------------------------------------------------------------------------------------------
' Fichier     : PlanningJourViewer.Designer.vb
' Projet      : Althéa
'
' Rôle        :
' Fichier de conception généré pour PlanningJourViewer. Porte la disposition et l'apparence des
' contrôles (Designer obligatoire — cf. Rules.md §9) : sélecteur de date, grille de consultation
' en lecture seule et bouton de fermeture. Le chargement des données et la date affichée restent
' dans le code-behind.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms

Partial Class PlanningJourViewer
    Inherits Form

    ' Conteneur des composants du concepteur.
    Private components As System.ComponentModel.IContainer

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Dispose
    ' Rôle      : Libère les ressources utilisées par le formulaire.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitializeComponent
    ' Rôle      : Compose la fenêtre (titre, sélecteur de date, grille de consultation et bouton de
    '             fermeture) ainsi que les colonnes statiques de la grille. Disposition et apparence
    '             uniquement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PlanningJourViewer))
        dtpDate = New AltheaDate()
        dgvPlanning = New DataGridView()
        colHeureDebut = New DataGridViewTextBoxColumn()
        colHeureFin = New DataGridViewTextBoxColumn()
        colPatient = New DataGridViewTextBoxColumn()
        colDomaine = New DataGridViewTextBoxColumn()
        colCodeSeance = New DataGridViewTextBoxColumn()
        colStatut = New DataGridViewTextBoxColumn()
        colTarif = New DataGridViewTextBoxColumn()
        btnFermer = New AltheaBouton()
        enTete = New AltheaEnTete()
        cartePlanning = New AltheaCarte()
        pnlActions = New AltheaBarreActions()
        CType(dgvPlanning, System.ComponentModel.ISupportInitialize).BeginInit()
        pnlActions.SuspendLayout()
        cartePlanning.SuspendLayout()
        SuspendLayout()
        ' 
        ' dtpDate
        ' 
        dtpDate.Font = New Font("Calibri", 11F)
        dtpDate.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        dtpDate.Location = New Point(14, 40)
        dtpDate.Name = "dtpDate"
        dtpDate.Size = New Size(150, 26)
        dtpDate.TabIndex = 0
        ' 
        ' dgvPlanning
        ' 
        dgvPlanning.AllowUserToAddRows = False
        dgvPlanning.AllowUserToDeleteRows = False
        dgvPlanning.AllowUserToResizeRows = False
        dgvPlanning.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvPlanning.BackgroundColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        dgvPlanning.BorderStyle = BorderStyle.None
        dgvPlanning.Columns.AddRange(New DataGridViewColumn() {colHeureDebut, colHeureFin, colPatient, colDomaine, colCodeSeance, colStatut, colTarif})
        dgvPlanning.Location = New Point(14, 74)
        dgvPlanning.MultiSelect = False
        dgvPlanning.Name = "dgvPlanning"
        dgvPlanning.ReadOnly = True
        dgvPlanning.RowHeadersVisible = False
        dgvPlanning.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPlanning.Size = New Size(748, 308)
        dgvPlanning.TabIndex = 1
        ' 
        ' colHeureDebut
        ' 
        colHeureDebut.DataPropertyName = "HeureDebut"
        colHeureDebut.HeaderText = "Début"
        colHeureDebut.Name = "colHeureDebut"
        colHeureDebut.ReadOnly = True
        colHeureDebut.Width = 70
        ' 
        ' colHeureFin
        ' 
        colHeureFin.DataPropertyName = "HeureFin"
        colHeureFin.HeaderText = "Fin"
        colHeureFin.Name = "colHeureFin"
        colHeureFin.ReadOnly = True
        colHeureFin.Width = 70
        ' 
        ' colPatient
        ' 
        colPatient.DataPropertyName = "NomCompletPatient"
        colPatient.HeaderText = "Patient"
        colPatient.Name = "colPatient"
        colPatient.ReadOnly = True
        colPatient.Width = 180
        ' 
        ' colDomaine
        ' 
        colDomaine.DataPropertyName = "LibelleDomaine"
        colDomaine.HeaderText = "Domaine"
        colDomaine.Name = "colDomaine"
        colDomaine.ReadOnly = True
        colDomaine.Width = 140
        ' 
        ' colCodeSeance
        ' 
        colCodeSeance.DataPropertyName = "CodeSeance"
        colCodeSeance.HeaderText = "Code"
        colCodeSeance.Name = "colCodeSeance"
        colCodeSeance.ReadOnly = True
        colCodeSeance.Width = 85
        ' 
        ' colStatut
        ' 
        colStatut.DataPropertyName = "LibelleStatutSeance"
        colStatut.HeaderText = "Statut"
        colStatut.Name = "colStatut"
        colStatut.ReadOnly = True
        colStatut.Width = 120
        ' 
        ' colTarif
        ' 
        colTarif.DataPropertyName = "TarifSeance"
        colTarif.HeaderText = "Tarif"
        colTarif.Name = "colTarif"
        colTarif.ReadOnly = True
        colTarif.Width = 80
        ' 
        ' btnFermer
        ' 
        btnFermer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.DialogResult = DialogResult.OK
        btnFermer.Location = New Point(661, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(109, 38)
        btnFermer.TabIndex = 3
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(800, 53)
        enTete.TabIndex = 9
        enTete.SousTitre = "Vue planification par journée"
        enTete.Text = "Séance"
        ' 
        ' cartePlanning
        ' 
        cartePlanning.Controls.Add(dtpDate)
        cartePlanning.Controls.Add(dgvPlanning)
        cartePlanning.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        cartePlanning.Location = New Point(12, 64)
        cartePlanning.Name = "cartePlanning"
        cartePlanning.Padding = New Padding(12, 8, 12, 10)
        cartePlanning.Size = New Size(776, 392)
        cartePlanning.TabIndex = 1
        cartePlanning.TabStop = False
        cartePlanning.Titre = "Séances du jour"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 464)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(800, 52)
        pnlActions.TabIndex = 10
        ' 
        ' PlanningJourViewer
        ' 
        AcceptButton = btnFermer
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        CancelButton = btnFermer
        ClientSize = New Size(800, 516)
        Controls.Add(cartePlanning)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 11F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MinimizeBox = False
        MinimumSize = New Size(560, 320)
        Name = "PlanningJourViewer"
        StartPosition = FormStartPosition.CenterParent
        Text = "Planning du jour"
        CType(dgvPlanning, System.ComponentModel.ISupportInitialize).EndInit()
        pnlActions.ResumeLayout(False)
        cartePlanning.ResumeLayout(False)
        cartePlanning.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents dtpDate As AltheaDate
    Friend WithEvents dgvPlanning As DataGridView
    Friend WithEvents btnFermer As AltheaBouton
    Friend WithEvents colHeureDebut As DataGridViewTextBoxColumn
    Friend WithEvents colHeureFin As DataGridViewTextBoxColumn
    Friend WithEvents colPatient As DataGridViewTextBoxColumn
    Friend WithEvents colDomaine As DataGridViewTextBoxColumn
    Friend WithEvents colCodeSeance As DataGridViewTextBoxColumn
    Friend WithEvents colStatut As DataGridViewTextBoxColumn
    Friend WithEvents colTarif As DataGridViewTextBoxColumn
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents cartePlanning As AltheaCarte
    Friend WithEvents pnlActions As AltheaBarreActions

End Class
