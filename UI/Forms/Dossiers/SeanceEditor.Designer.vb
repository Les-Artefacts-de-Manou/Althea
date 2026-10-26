' -------------------------------------------------------------------------------------------------
' Fichier     : SeanceEditor.Designer.vb
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 19/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Code généré (layout) de la modale SeanceEditor : bandeau titre, champs d'identité de la séance
' (type, date, horaires, durée, tarif, statut), deux éditeurs riches (Notes / Évolution) dans un
' TabControl, et panneau d'actions (Enregistrer / Fermer).
'
' Remarques   :
' - Brique 5 (Lot 2) : modale de création/édition d'une séance depuis le hub Dossier.
' - Boutons colorés cohérents avec la charte → pas de ressource image, pas de .resx.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SeanceEditor
    Inherits System.Windows.Forms.Form

    ' Form remplace Dispose pour nettoyer la liste des composants.
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

    ' Requis par le concepteur Windows Forms.
    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SeanceEditor))
        enTete = New AltheaEnTete()
        cartePrestation = New AltheaCarte()
        carteRendezVous = New AltheaCarte()
        lblPrixSeance = New Label()
        cboPrixSeance = New AltheaListe()
        lblNatureSeance = New Label()
        cboNatureSeance = New AltheaListe()
        btnDeverrouillerTarif = New AltheaBouton()
        btnAjouterNatureSeance = New AltheaBouton()
        lblDateSeance = New Label()
        dtpDateSeance = New AltheaDate()
        btnVoirPlanningJour = New AltheaBouton()
        lblHeureDebut = New Label()
        hreDebut = New AltheaHeure()
        lblHeureFin = New Label()
        hreFin = New AltheaHeure()
        lblDureeMinutes = New Label()
        numDureeMinutes = New NumericUpDown()
        lblTarifSeance = New Label()
        numTarifSeance = New NumericUpDown()
        lblStatutSeance = New Label()
        cboStatutSeance = New AltheaListe()
        btnAjouterPrixSeance = New AltheaBouton()
        btnAjouterStatutSeance = New AltheaBouton()
        tabNotes = New AltheaOnglets()
        tabPageNotes = New TabPage()
        rteNotes = New UC_RichTextEditor()
        tabPageEvolution = New TabPage()
        rteEvolution = New UC_RichTextEditor()
        pnlActions = New AltheaBarreActions()
        btnEnregistrer = New AltheaBouton()
        btnDeverrouillerSeance = New AltheaBouton()
        btnFermer = New AltheaBouton()
        CType(numDureeMinutes, ComponentModel.ISupportInitialize).BeginInit()
        CType(numTarifSeance, ComponentModel.ISupportInitialize).BeginInit()
        cartePrestation.SuspendLayout()
        carteRendezVous.SuspendLayout()
        tabNotes.SuspendLayout()
        tabPageNotes.SuspendLayout()
        tabPageEvolution.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(762, 57)
        enTete.TabIndex = 0
        enTete.SousTitre = "Gérez les séances et les notes de séances"
        enTete.Text = "Séance"
        ' 
        ' cartePrestation
        ' 
        cartePrestation.Controls.Add(lblPrixSeance)
        cartePrestation.Controls.Add(cboPrixSeance)
        cartePrestation.Controls.Add(btnAjouterPrixSeance)
        cartePrestation.Controls.Add(lblNatureSeance)
        cartePrestation.Controls.Add(cboNatureSeance)
        cartePrestation.Controls.Add(btnAjouterNatureSeance)
        cartePrestation.Controls.Add(lblTarifSeance)
        cartePrestation.Controls.Add(numTarifSeance)
        cartePrestation.Controls.Add(btnDeverrouillerTarif)
        cartePrestation.Location = New Point(12, 64)
        cartePrestation.Name = "cartePrestation"
        cartePrestation.Padding = New Padding(12, 8, 12, 10)
        cartePrestation.Size = New Size(738, 108)
        cartePrestation.TabIndex = 1
        cartePrestation.TabStop = False
        cartePrestation.Titre = "Prestation"
        ' 
        ' carteRendezVous
        ' 
        carteRendezVous.Controls.Add(lblDateSeance)
        carteRendezVous.Controls.Add(dtpDateSeance)
        carteRendezVous.Controls.Add(lblHeureDebut)
        carteRendezVous.Controls.Add(hreDebut)
        carteRendezVous.Controls.Add(lblHeureFin)
        carteRendezVous.Controls.Add(hreFin)
        carteRendezVous.Controls.Add(btnVoirPlanningJour)
        carteRendezVous.Controls.Add(lblDureeMinutes)
        carteRendezVous.Controls.Add(numDureeMinutes)
        carteRendezVous.Controls.Add(lblStatutSeance)
        carteRendezVous.Controls.Add(cboStatutSeance)
        carteRendezVous.Controls.Add(btnAjouterStatutSeance)
        carteRendezVous.Location = New Point(12, 180)
        carteRendezVous.Name = "carteRendezVous"
        carteRendezVous.Padding = New Padding(12, 8, 12, 10)
        carteRendezVous.Size = New Size(738, 108)
        carteRendezVous.TabIndex = 2
        carteRendezVous.TabStop = False
        carteRendezVous.Titre = "Rendez-vous"
        ' 
        ' lblPrixSeance
        ' 
        lblPrixSeance.AutoSize = True
        lblPrixSeance.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblPrixSeance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblPrixSeance.Location = New Point(14, 44)
        lblPrixSeance.Name = "lblPrixSeance"
        lblPrixSeance.Size = New Size(40, 17)
        lblPrixSeance.TabIndex = 1
        lblPrixSeance.Text = "Tarif :"
        ' 
        ' cboPrixSeance
        ' 
        cboPrixSeance.Font = New Font("Calibri", 11F)
        cboPrixSeance.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboPrixSeance.ItemHeight = 20
        cboPrixSeance.Location = New Point(118, 40)
        cboPrixSeance.Name = "cboPrixSeance"
        cboPrixSeance.Size = New Size(230, 26)
        cboPrixSeance.TabIndex = 0
        ' 
        ' lblNatureSeance
        ' 
        lblNatureSeance.AutoSize = True
        lblNatureSeance.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNatureSeance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNatureSeance.Location = New Point(398, 44)
        lblNatureSeance.Name = "lblNatureSeance"
        lblNatureSeance.Size = New Size(55, 17)
        lblNatureSeance.TabIndex = 20
        lblNatureSeance.Text = "Nature :"
        ' 
        ' cboNatureSeance
        ' 
        cboNatureSeance.Font = New Font("Calibri", 11F)
        cboNatureSeance.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboNatureSeance.ItemHeight = 20
        cboNatureSeance.Location = New Point(458, 40)
        cboNatureSeance.Name = "cboNatureSeance"
        cboNatureSeance.Size = New Size(220, 26)
        cboNatureSeance.TabIndex = 2
        ' 
        ' btnDeverrouillerTarif
        ' 
        btnDeverrouillerTarif.Location = New Point(212, 70)
        btnDeverrouillerTarif.Name = "btnDeverrouillerTarif"
        btnDeverrouillerTarif.Size = New Size(30, 30)
        btnDeverrouillerTarif.TabIndex = 5
        btnDeverrouillerTarif.Variante = VarianteBouton.Outil
        ' 
        ' btnAjouterNatureSeance
        ' 
        btnAjouterNatureSeance.Image = CType(resources.GetObject("btnAjouterNatureSeance.Image"), Image)
        btnAjouterNatureSeance.Location = New Point(682, 38)
        btnAjouterNatureSeance.Name = "btnAjouterNatureSeance"
        btnAjouterNatureSeance.Size = New Size(30, 30)
        btnAjouterNatureSeance.TabIndex = 3
        btnAjouterNatureSeance.Tag = "plus_24_normal"
        btnAjouterNatureSeance.Variante = VarianteBouton.Outil
        ' 
        ' lblDateSeance
        ' 
        lblDateSeance.AutoSize = True
        lblDateSeance.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDateSeance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateSeance.Location = New Point(14, 44)
        lblDateSeance.Name = "lblDateSeance"
        lblDateSeance.Size = New Size(43, 17)
        lblDateSeance.TabIndex = 3
        lblDateSeance.Text = "Date :"
        ' 
        ' dtpDateSeance
        ' 
        dtpDateSeance.Font = New Font("Calibri", 11F)
        dtpDateSeance.Location = New Point(118, 40)
        dtpDateSeance.Name = "dtpDateSeance"
        dtpDateSeance.Size = New Size(130, 26)
        dtpDateSeance.TabIndex = 0
        ' 
        ' btnVoirPlanningJour
        ' 
        btnVoirPlanningJour.Location = New Point(538, 37)
        btnVoirPlanningJour.Name = "btnVoirPlanningJour"
        btnVoirPlanningJour.Size = New Size(188, 32)
        btnVoirPlanningJour.TabIndex = 3
        btnVoirPlanningJour.Tag = "voir_normal"
        btnVoirPlanningJour.Text = "Voir planning du jour"
        btnVoirPlanningJour.Variante = VarianteBouton.Clair
        ' 
        ' lblHeureDebut
        ' 
        lblHeureDebut.AutoSize = True
        lblHeureDebut.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblHeureDebut.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblHeureDebut.Location = New Point(262, 44)
        lblHeureDebut.Name = "lblHeureDebut"
        lblHeureDebut.Size = New Size(50, 17)
        lblHeureDebut.TabIndex = 4
        lblHeureDebut.Text = "Début :"
        ' 
        ' hreDebut
        ' 
        hreDebut.AfficherCroix = False
        hreDebut.Facultative = True
        hreDebut.Location = New Point(314, 40)
        hreDebut.Name = "hreDebut"
        hreDebut.Size = New Size(86, 26)
        hreDebut.TabIndex = 1
        ' 
        ' lblHeureFin
        ' 
        lblHeureFin.AutoSize = True
        lblHeureFin.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblHeureFin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblHeureFin.Location = New Point(410, 44)
        lblHeureFin.Name = "lblHeureFin"
        lblHeureFin.Size = New Size(31, 17)
        lblHeureFin.TabIndex = 5
        lblHeureFin.Text = "Fin :"
        ' 
        ' hreFin
        ' 
        hreFin.AfficherCroix = False
        hreFin.Facultative = True
        hreFin.Location = New Point(444, 40)
        hreFin.Name = "hreFin"
        hreFin.Size = New Size(86, 26)
        hreFin.TabIndex = 2
        ' 
        ' lblDureeMinutes
        ' 
        lblDureeMinutes.AutoSize = True
        lblDureeMinutes.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDureeMinutes.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDureeMinutes.Location = New Point(14, 76)
        lblDureeMinutes.Name = "lblDureeMinutes"
        lblDureeMinutes.Size = New Size(82, 17)
        lblDureeMinutes.TabIndex = 6
        lblDureeMinutes.Text = "Durée (min) :"
        ' 
        ' numDureeMinutes
        ' 
        numDureeMinutes.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        numDureeMinutes.Font = New Font("Calibri", 11F)
        numDureeMinutes.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        numDureeMinutes.Location = New Point(118, 72)
        numDureeMinutes.Maximum = New Decimal(New Integer() {600, 0, 0, 0})
        numDureeMinutes.Name = "numDureeMinutes"
        numDureeMinutes.Size = New Size(80, 26)
        numDureeMinutes.TabIndex = 4
        ' 
        ' lblTarifSeance
        ' 
        lblTarifSeance.AutoSize = True
        lblTarifSeance.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblTarifSeance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblTarifSeance.Location = New Point(14, 76)
        lblTarifSeance.Name = "lblTarifSeance"
        lblTarifSeance.Size = New Size(58, 17)
        lblTarifSeance.TabIndex = 7
        lblTarifSeance.Text = "Tarif (€) :"
        ' 
        ' numTarifSeance
        ' 
        numTarifSeance.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        numTarifSeance.DecimalPlaces = 2
        numTarifSeance.Font = New Font("Calibri", 11F)
        numTarifSeance.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        numTarifSeance.Location = New Point(118, 72)
        numTarifSeance.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        numTarifSeance.Name = "numTarifSeance"
        numTarifSeance.ReadOnly = True
        numTarifSeance.Size = New Size(90, 26)
        numTarifSeance.TabIndex = 4
        ' 
        ' lblStatutSeance
        ' 
        lblStatutSeance.AutoSize = True
        lblStatutSeance.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblStatutSeance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblStatutSeance.Location = New Point(270, 76)
        lblStatutSeance.Name = "lblStatutSeance"
        lblStatutSeance.Size = New Size(50, 17)
        lblStatutSeance.TabIndex = 2
        lblStatutSeance.Text = "Statut :"
        ' 
        ' cboStatutSeance
        ' 
        cboStatutSeance.Font = New Font("Calibri", 11F)
        cboStatutSeance.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboStatutSeance.ItemHeight = 20
        cboStatutSeance.Location = New Point(323, 72)
        cboStatutSeance.Name = "cboStatutSeance"
        cboStatutSeance.Size = New Size(230, 26)
        cboStatutSeance.TabIndex = 5
        ' 
        ' btnAjouterPrixSeance
        ' 
        btnAjouterPrixSeance.Image = CType(resources.GetObject("btnAjouterPrixSeance.Image"), Image)
        btnAjouterPrixSeance.Location = New Point(352, 38)
        btnAjouterPrixSeance.Name = "btnAjouterPrixSeance"
        btnAjouterPrixSeance.Size = New Size(30, 30)
        btnAjouterPrixSeance.TabIndex = 1
        btnAjouterPrixSeance.Tag = "plus_24_normal"
        btnAjouterPrixSeance.Variante = VarianteBouton.Outil
        ' 
        ' btnAjouterStatutSeance
        ' 
        btnAjouterStatutSeance.Image = CType(resources.GetObject("btnAjouterStatutSeance.Image"), Image)
        btnAjouterStatutSeance.Location = New Point(557, 70)
        btnAjouterStatutSeance.Name = "btnAjouterStatutSeance"
        btnAjouterStatutSeance.Size = New Size(30, 30)
        btnAjouterStatutSeance.TabIndex = 6
        btnAjouterStatutSeance.Tag = "plus_24_normal"
        btnAjouterStatutSeance.Variante = VarianteBouton.Outil
        ' 
        ' tabNotes
        ' 
        tabNotes.Controls.Add(tabPageNotes)
        tabNotes.Controls.Add(tabPageEvolution)
        tabNotes.Font = New Font("Calibri", 10F)
        tabNotes.Location = New Point(12, 298)
        tabNotes.Name = "tabNotes"
        tabNotes.SelectedIndex = 0
        tabNotes.Size = New Size(738, 260)
        tabNotes.TabIndex = 3
        ' 
        ' tabPageNotes
        ' 
        tabPageNotes.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageNotes.Controls.Add(rteNotes)
        tabPageNotes.Location = New Point(4, 27)
        tabPageNotes.Name = "tabPageNotes"
        tabPageNotes.Padding = New Padding(6)
        tabPageNotes.Size = New Size(730, 229)
        tabPageNotes.TabIndex = 0
        tabPageNotes.Text = "Notes de séance"
        ' 
        ' rteNotes
        ' 
        rteNotes.BackColor = Color.Transparent
        rteNotes.Dock = DockStyle.Fill
        rteNotes.Location = New Point(6, 6)
        rteNotes.Margin = New Padding(4, 3, 4, 3)
        rteNotes.Name = "rteNotes"
        rteNotes.Profil = ProfilEditeur.Compact
        rteNotes.TitreDocument = "Notes de séance"
        rteNotes.Size = New Size(701, 217)
        rteNotes.TabIndex = 0
        ' 
        ' tabPageEvolution
        ' 
        tabPageEvolution.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tabPageEvolution.Controls.Add(rteEvolution)
        tabPageEvolution.Location = New Point(4, 27)
        tabPageEvolution.Name = "tabPageEvolution"
        tabPageEvolution.Padding = New Padding(6)
        tabPageEvolution.Size = New Size(730, 229)
        tabPageEvolution.TabIndex = 1
        tabPageEvolution.Text = "Évolution"
        ' 
        ' rteEvolution
        ' 
        rteEvolution.BackColor = Color.Transparent
        rteEvolution.Dock = DockStyle.Fill
        rteEvolution.Location = New Point(6, 6)
        rteEvolution.Margin = New Padding(4, 3, 4, 3)
        rteEvolution.Name = "rteEvolution"
        rteEvolution.Profil = ProfilEditeur.Compact
        rteEvolution.TitreDocument = "Évolution de la séance"
        rteEvolution.Size = New Size(701, 217)
        rteEvolution.TabIndex = 0
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Controls.Add(btnDeverrouillerSeance)
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 566)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(762, 52)
        pnlActions.TabIndex = 8
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(487, 7)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 38)
        btnEnregistrer.TabIndex = 0
        btnEnregistrer.Tag = "Enregistrer"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnDeverrouillerSeance
        ' 
        btnDeverrouillerSeance.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        btnDeverrouillerSeance.Image = CType(resources.GetObject("btnDeverrouillerSeance.Image"), Image)
        btnDeverrouillerSeance.Location = New Point(19, 7)
        btnDeverrouillerSeance.Name = "btnDeverrouillerSeance"
        btnDeverrouillerSeance.Size = New Size(224, 38)
        btnDeverrouillerSeance.TabIndex = 2
        btnDeverrouillerSeance.Tag = "DeverrouillerSeance"
        btnDeverrouillerSeance.Text = "Déverrouiller (SuperUser)"
        btnDeverrouillerSeance.Visible = False
        ' 
        ' btnFermer
        ' 
        btnFermer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.Location = New Point(629, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(109, 38)
        btnFermer.TabIndex = 1
        btnFermer.Tag = "Fermer"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' SeanceEditor
        ' 
        AutoScaleDimensions = New SizeF(8F, 18F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(762, 618)
        Controls.Add(carteRendezVous)
        Controls.Add(cartePrestation)
        Controls.Add(tabNotes)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 11F)
        ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "SeanceEditor"
        StartPosition = FormStartPosition.CenterParent
        Text = "Séance"
        CType(numDureeMinutes, ComponentModel.ISupportInitialize).EndInit()
        CType(numTarifSeance, ComponentModel.ISupportInitialize).EndInit()
        cartePrestation.ResumeLayout(False)
        cartePrestation.PerformLayout()
        carteRendezVous.ResumeLayout(False)
        carteRendezVous.PerformLayout()
        tabNotes.ResumeLayout(False)
        tabPageNotes.ResumeLayout(False)
        tabPageEvolution.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()

    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents cartePrestation As AltheaCarte
    Friend WithEvents carteRendezVous As AltheaCarte
    Friend WithEvents lblPrixSeance As Label
    Friend WithEvents cboPrixSeance As AltheaListe
    Friend WithEvents lblNatureSeance As Label
    Friend WithEvents cboNatureSeance As AltheaListe
    Friend WithEvents btnAjouterPrixSeance As AltheaBouton
    Friend WithEvents btnAjouterNatureSeance As AltheaBouton
    Friend WithEvents btnDeverrouillerTarif As AltheaBouton
    Friend WithEvents btnAjouterStatutSeance As AltheaBouton
    Friend WithEvents lblStatutSeance As Label
    Friend WithEvents cboStatutSeance As AltheaListe
    Friend WithEvents lblDateSeance As Label
    Friend WithEvents dtpDateSeance As AltheaDate
    Friend WithEvents btnVoirPlanningJour As AltheaBouton
    Friend WithEvents lblHeureDebut As Label
    Friend WithEvents hreDebut As AltheaHeure
    Friend WithEvents lblHeureFin As Label
    Friend WithEvents hreFin As AltheaHeure
    Friend WithEvents lblDureeMinutes As Label
    Friend WithEvents numDureeMinutes As NumericUpDown
    Friend WithEvents lblTarifSeance As Label
    Friend WithEvents numTarifSeance As NumericUpDown
    Friend WithEvents tabNotes As AltheaOnglets
    Friend WithEvents tabPageNotes As TabPage
    Friend WithEvents rteNotes As UC_RichTextEditor
    Friend WithEvents tabPageEvolution As TabPage
    Friend WithEvents rteEvolution As UC_RichTextEditor
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnDeverrouillerSeance As AltheaBouton
    Friend WithEvents btnFermer As AltheaBouton

End Class
