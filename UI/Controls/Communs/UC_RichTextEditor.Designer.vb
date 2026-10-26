<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_RichTextEditor
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_RichTextEditor))
        toolStrip = New ToolStrip()
        lblLectureSeule = New ToolStripLabel()
        sepLecture = New ToolStripSeparator()
        btnAnnuler = New ToolStripButton()
        btnRetablir = New ToolStripButton()
        sepHistorique = New ToolStripSeparator()
        btnStyle = New ToolStripDropDownButton()
        sepStyle = New ToolStripSeparator()
        btnGras = New ToolStripButton()
        btnItalique = New ToolStripButton()
        btnSouligne = New ToolStripButton()
        btnBarre = New ToolStripButton()
        btnCouleur = New ToolStripDropDownButton()
        btnSurligneur = New ToolStripDropDownButton()
        btnEffacer = New ToolStripButton()
        sepCaractere = New ToolStripSeparator()
        btnPuces = New ToolStripButton()
        btnNumeros = New ToolStripButton()
        btnRetraitMoins = New ToolStripButton()
        btnRetraitPlus = New ToolStripButton()
        btnAlignement = New ToolStripDropDownButton()
        sepParagraphe = New ToolStripSeparator()
        btnDate = New ToolStripDropDownButton()
        btnTextesTypes = New ToolStripDropDownButton()
        sepInsertion = New ToolStripSeparator()
        btnRechercher = New ToolStripButton()
        btnCopier = New ToolStripButton()
        btnExporter = New ToolStripDropDownButton()
        btnImprimer = New ToolStripButton()
        btnApercu = New ToolStripButton()
        btnOuvrirEnGrand = New ToolStripButton()
        mnuAlignGauche = New ToolStripMenuItem()
        mnuAlignCentre = New ToolStripMenuItem()
        mnuAlignDroite = New ToolStripMenuItem()
        mnuAlignJustifier = New ToolStripMenuItem()
        mnuExportPdf = New ToolStripMenuItem()
        mnuExportWord = New ToolStripMenuItem()
        sepExport = New ToolStripSeparator()
        mnuMiseEnPage = New ToolStripMenuItem()
        pnlRecherche = New Panel()
        txtRecherche = New AltheaRecherche()
        lblOccurrences = New Label()
        btnOccPrecedente = New AltheaBouton()
        btnOccSuivante = New AltheaBouton()
        chkCasse = New AltheaCase()
        btnAfficherRemplacer = New AltheaBouton()
        btnFermerRecherche = New AltheaBouton()
        pnlRemplacer = New Panel()
        txtRemplacer = New TextBox()
        btnRemplacer = New AltheaBouton()
        btnRemplacerTout = New AltheaBouton()
        pnlEtat = New Panel()
        lblMots = New Label()
        lblModifie = New Label()
        lblZoom = New Label()
        rtbEditor = New RichTextBox()
        menuContextuel = New ContextMenuStrip(components)
        mnuCouper = New ToolStripMenuItem()
        mnuCopier = New ToolStripMenuItem()
        mnuColler = New ToolStripMenuItem()
        mnuCollerTexte = New ToolStripMenuItem()
        sepContexte = New ToolStripSeparator()
        mnuToutSelectionner = New ToolStripMenuItem()
        infobulle = New ToolTip(components)
        toolStrip.SuspendLayout()
        pnlRecherche.SuspendLayout()
        pnlRemplacer.SuspendLayout()
        pnlEtat.SuspendLayout()
        menuContextuel.SuspendLayout()
        SuspendLayout()
        ' 
        ' toolStrip
        ' 
        toolStrip.AutoSize = False
        toolStrip.BackColor = Color.White
        toolStrip.Font = New Font("Calibri", 10F)
        toolStrip.GripStyle = ToolStripGripStyle.Hidden
        toolStrip.ImageScalingSize = New Size(20, 20)
        toolStrip.Items.AddRange(New ToolStripItem() {lblLectureSeule, sepLecture, btnAnnuler, btnRetablir, sepHistorique, btnStyle, sepStyle, btnGras, btnItalique, btnSouligne, btnBarre, btnCouleur, btnSurligneur, btnEffacer, sepCaractere, btnPuces, btnNumeros, btnRetraitMoins, btnRetraitPlus, btnAlignement, sepParagraphe, btnDate, btnTextesTypes, sepInsertion, btnRechercher, btnCopier, btnExporter, btnImprimer, btnApercu, btnOuvrirEnGrand})
        toolStrip.Location = New Point(0, 0)
        toolStrip.Name = "toolStrip"
        toolStrip.Padding = New Padding(4, 1, 4, 2)
        toolStrip.Size = New Size(1030, 36)
        toolStrip.TabIndex = 0
        toolStrip.Text = "Barre d'outils"
        ' 
        ' lblLectureSeule
        ' 
        lblLectureSeule.Image = CType(resources.GetObject("lblLectureSeule.Image"), Image)
        lblLectureSeule.BackColor = Color.FromArgb(CByte(230), CByte(230), CByte(230))
        lblLectureSeule.ForeColor = Color.FromArgb(CByte(110), CByte(110), CByte(110))
        lblLectureSeule.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblLectureSeule.Name = "lblLectureSeule"
        lblLectureSeule.Padding = New Padding(6, 0, 8, 0)
        lblLectureSeule.Size = New Size(122, 33)
        lblLectureSeule.Text = "Lecture seule"
        ' 
        ' sepLecture
        ' 
        sepLecture.Name = "sepLecture"
        sepLecture.Size = New Size(6, 33)
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.AutoSize = False
        btnAnnuler.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(30, 30)
        btnAnnuler.ToolTipText = "Annuler (Ctrl+Z)"
        ' 
        ' btnRetablir
        ' 
        btnRetablir.Image = CType(resources.GetObject("btnRetablir.Image"), Image)
        btnRetablir.AutoSize = False
        btnRetablir.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnRetablir.Name = "btnRetablir"
        btnRetablir.Size = New Size(30, 30)
        btnRetablir.ToolTipText = "Rétablir (Ctrl+Y)"
        ' 
        ' sepHistorique
        ' 
        sepHistorique.Name = "sepHistorique"
        sepHistorique.Size = New Size(6, 33)
        ' 
        ' btnStyle
        ' 
        btnStyle.AutoSize = False
        btnStyle.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnStyle.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        btnStyle.Name = "btnStyle"
        btnStyle.Size = New Size(112, 30)
        btnStyle.Text = "Normal"
        btnStyle.TextAlign = ContentAlignment.MiddleLeft
        btnStyle.ToolTipText = "Style du paragraphe"
        ' 
        ' sepStyle
        ' 
        sepStyle.Name = "sepStyle"
        sepStyle.Size = New Size(6, 33)
        ' 
        ' btnGras
        ' 
        btnGras.Image = CType(resources.GetObject("btnGras.Image"), Image)
        btnGras.AutoSize = False
        btnGras.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnGras.Name = "btnGras"
        btnGras.Size = New Size(30, 30)
        btnGras.ToolTipText = "Gras (Ctrl+G)"
        ' 
        ' btnItalique
        ' 
        btnItalique.Image = CType(resources.GetObject("btnItalique.Image"), Image)
        btnItalique.AutoSize = False
        btnItalique.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnItalique.Name = "btnItalique"
        btnItalique.Size = New Size(30, 30)
        btnItalique.ToolTipText = "Italique (Ctrl+I)"
        ' 
        ' btnSouligne
        ' 
        btnSouligne.Image = CType(resources.GetObject("btnSouligne.Image"), Image)
        btnSouligne.AutoSize = False
        btnSouligne.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnSouligne.Name = "btnSouligne"
        btnSouligne.Size = New Size(30, 30)
        btnSouligne.ToolTipText = "Souligné (Ctrl+U)"
        ' 
        ' btnBarre
        ' 
        btnBarre.Image = CType(resources.GetObject("btnBarre.Image"), Image)
        btnBarre.AutoSize = False
        btnBarre.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnBarre.Name = "btnBarre"
        btnBarre.Size = New Size(30, 30)
        btnBarre.ToolTipText = "Barré"
        ' 
        ' btnCouleur
        ' 
        btnCouleur.Image = CType(resources.GetObject("btnCouleur.Image"), Image)
        btnCouleur.AutoSize = False
        btnCouleur.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnCouleur.Name = "btnCouleur"
        btnCouleur.Size = New Size(40, 30)
        btnCouleur.ToolTipText = "Couleur du texte"
        ' 
        ' btnSurligneur
        ' 
        btnSurligneur.Image = CType(resources.GetObject("btnSurligneur.Image"), Image)
        btnSurligneur.AutoSize = False
        btnSurligneur.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnSurligneur.Name = "btnSurligneur"
        btnSurligneur.Size = New Size(40, 30)
        btnSurligneur.ToolTipText = "Surligneur"
        ' 
        ' btnEffacer
        ' 
        btnEffacer.Image = CType(resources.GetObject("btnEffacer.Image"), Image)
        btnEffacer.AutoSize = False
        btnEffacer.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnEffacer.Name = "btnEffacer"
        btnEffacer.Size = New Size(30, 30)
        btnEffacer.ToolTipText = "Effacer la mise en forme"
        ' 
        ' sepCaractere
        ' 
        sepCaractere.Name = "sepCaractere"
        sepCaractere.Size = New Size(6, 33)
        ' 
        ' btnPuces
        ' 
        btnPuces.Image = CType(resources.GetObject("btnPuces.Image"), Image)
        btnPuces.AutoSize = False
        btnPuces.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnPuces.Name = "btnPuces"
        btnPuces.Size = New Size(30, 30)
        btnPuces.ToolTipText = "Liste à puces"
        ' 
        ' btnNumeros
        ' 
        btnNumeros.Image = CType(resources.GetObject("btnNumeros.Image"), Image)
        btnNumeros.AutoSize = False
        btnNumeros.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnNumeros.Name = "btnNumeros"
        btnNumeros.Size = New Size(30, 30)
        btnNumeros.ToolTipText = "Liste numérotée"
        ' 
        ' btnRetraitMoins
        ' 
        btnRetraitMoins.Image = CType(resources.GetObject("btnRetraitMoins.Image"), Image)
        btnRetraitMoins.AutoSize = False
        btnRetraitMoins.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnRetraitMoins.Name = "btnRetraitMoins"
        btnRetraitMoins.Size = New Size(30, 30)
        btnRetraitMoins.ToolTipText = "Diminuer le retrait"
        ' 
        ' btnRetraitPlus
        ' 
        btnRetraitPlus.Image = CType(resources.GetObject("btnRetraitPlus.Image"), Image)
        btnRetraitPlus.AutoSize = False
        btnRetraitPlus.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnRetraitPlus.Name = "btnRetraitPlus"
        btnRetraitPlus.Size = New Size(30, 30)
        btnRetraitPlus.ToolTipText = "Augmenter le retrait"
        ' 
        ' btnAlignement
        ' 
        btnAlignement.Image = CType(resources.GetObject("btnAlignement.Image"), Image)
        btnAlignement.DropDownItems.AddRange(New ToolStripItem() {mnuAlignGauche, mnuAlignCentre, mnuAlignDroite, mnuAlignJustifier})
        btnAlignement.AutoSize = False
        btnAlignement.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnAlignement.Name = "btnAlignement"
        btnAlignement.Size = New Size(40, 30)
        btnAlignement.ToolTipText = "Alignement"
        ' 
        ' sepParagraphe
        ' 
        sepParagraphe.Name = "sepParagraphe"
        sepParagraphe.Size = New Size(6, 33)
        ' 
        ' btnDate
        ' 
        btnDate.Image = CType(resources.GetObject("btnDate.Image"), Image)
        btnDate.AutoSize = False
        btnDate.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnDate.Name = "btnDate"
        btnDate.Size = New Size(40, 30)
        btnDate.ToolTipText = "Insérer la date"
        ' 
        ' btnTextesTypes
        ' 
        btnTextesTypes.Image = CType(resources.GetObject("btnTextesTypes.Image"), Image)
        btnTextesTypes.AutoSize = False
        btnTextesTypes.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnTextesTypes.Name = "btnTextesTypes"
        btnTextesTypes.Size = New Size(40, 30)
        btnTextesTypes.ToolTipText = "Insérer un texte type"
        ' 
        ' sepInsertion
        ' 
        sepInsertion.Name = "sepInsertion"
        sepInsertion.Size = New Size(6, 33)
        ' 
        ' btnRechercher
        ' 
        btnRechercher.Image = CType(resources.GetObject("btnRechercher.Image"), Image)
        btnRechercher.AutoSize = False
        btnRechercher.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnRechercher.Name = "btnRechercher"
        btnRechercher.Size = New Size(30, 30)
        btnRechercher.ToolTipText = "Rechercher (Ctrl+F)"
        ' 
        ' btnCopier
        ' 
        btnCopier.Image = CType(resources.GetObject("btnCopier.Image"), Image)
        btnCopier.AutoSize = False
        btnCopier.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnCopier.Name = "btnCopier"
        btnCopier.Size = New Size(30, 30)
        btnCopier.ToolTipText = "Copier (Ctrl+C)"
        ' 
        ' btnExporter
        ' 
        btnExporter.Image = CType(resources.GetObject("btnExporter.Image"), Image)
        btnExporter.Alignment = ToolStripItemAlignment.Right
        btnExporter.DropDownItems.AddRange(New ToolStripItem() {mnuExportPdf, mnuExportWord, sepExport, mnuMiseEnPage})
        btnExporter.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        btnExporter.Name = "btnExporter"
        btnExporter.Size = New Size(98, 30)
        btnExporter.Text = "Exporter"
        btnExporter.ToolTipText = "Exporter"
        ' 
        ' btnImprimer
        ' 
        btnImprimer.Image = CType(resources.GetObject("btnImprimer.Image"), Image)
        btnImprimer.Alignment = ToolStripItemAlignment.Right
        btnImprimer.AutoSize = False
        btnImprimer.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnImprimer.Name = "btnImprimer"
        btnImprimer.Size = New Size(30, 30)
        btnImprimer.ToolTipText = "Imprimer (Ctrl+P)"
        ' 
        ' btnApercu
        ' 
        btnApercu.Image = CType(resources.GetObject("btnApercu.Image"), Image)
        btnApercu.Alignment = ToolStripItemAlignment.Right
        btnApercu.AutoSize = False
        btnApercu.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnApercu.Name = "btnApercu"
        btnApercu.Size = New Size(30, 30)
        btnApercu.ToolTipText = "Aperçu avant impression"
        ' 
        ' btnOuvrirEnGrand
        ' 
        btnOuvrirEnGrand.Image = CType(resources.GetObject("btnOuvrirEnGrand.Image"), Image)
        btnOuvrirEnGrand.Alignment = ToolStripItemAlignment.Right
        btnOuvrirEnGrand.AutoSize = False
        btnOuvrirEnGrand.DisplayStyle = ToolStripItemDisplayStyle.Image
        btnOuvrirEnGrand.Name = "btnOuvrirEnGrand"
        btnOuvrirEnGrand.Size = New Size(30, 30)
        btnOuvrirEnGrand.ToolTipText = "Ouvrir en grand : écrire dans une grande fenêtre"
        ' 
        ' mnuAlignGauche
        ' 
        mnuAlignGauche.Image = CType(resources.GetObject("mnuAlignGauche.Image"), Image)
        mnuAlignGauche.Name = "mnuAlignGauche"
        mnuAlignGauche.ShortcutKeyDisplayString = "Ctrl+L"
        mnuAlignGauche.Size = New Size(220, 26)
        mnuAlignGauche.Text = "Aligner à gauche"
        ' 
        ' mnuAlignCentre
        ' 
        mnuAlignCentre.Image = CType(resources.GetObject("mnuAlignCentre.Image"), Image)
        mnuAlignCentre.Name = "mnuAlignCentre"
        mnuAlignCentre.ShortcutKeyDisplayString = "Ctrl+E"
        mnuAlignCentre.Size = New Size(220, 26)
        mnuAlignCentre.Text = "Centrer"
        ' 
        ' mnuAlignDroite
        ' 
        mnuAlignDroite.Image = CType(resources.GetObject("mnuAlignDroite.Image"), Image)
        mnuAlignDroite.Name = "mnuAlignDroite"
        mnuAlignDroite.ShortcutKeyDisplayString = "Ctrl+R"
        mnuAlignDroite.Size = New Size(220, 26)
        mnuAlignDroite.Text = "Aligner à droite"
        ' 
        ' mnuAlignJustifier
        ' 
        mnuAlignJustifier.Image = CType(resources.GetObject("mnuAlignJustifier.Image"), Image)
        mnuAlignJustifier.Name = "mnuAlignJustifier"
        mnuAlignJustifier.ShortcutKeyDisplayString = "Ctrl+J"
        mnuAlignJustifier.Size = New Size(220, 26)
        mnuAlignJustifier.Text = "Justifier"
        ' 
        ' mnuExportPdf
        ' 
        mnuExportPdf.Image = CType(resources.GetObject("mnuExportPdf.Image"), Image)
        mnuExportPdf.Name = "mnuExportPdf"
        mnuExportPdf.Size = New Size(200, 26)
        mnuExportPdf.Text = "PDF"
        ' 
        ' mnuExportWord
        ' 
        mnuExportWord.Image = CType(resources.GetObject("mnuExportWord.Image"), Image)
        mnuExportWord.Name = "mnuExportWord"
        mnuExportWord.Size = New Size(200, 26)
        mnuExportWord.Text = "Document Word"
        ' 
        ' sepExport
        ' 
        sepExport.Name = "sepExport"
        sepExport.Size = New Size(200, 6)
        ' 
        ' mnuMiseEnPage
        ' 
        mnuMiseEnPage.Image = CType(resources.GetObject("mnuMiseEnPage.Image"), Image)
        mnuMiseEnPage.Name = "mnuMiseEnPage"
        mnuMiseEnPage.Size = New Size(200, 26)
        mnuMiseEnPage.Text = "Mise en page…"
        ' 
        ' pnlRecherche
        ' 
        pnlRecherche.BackColor = Color.FromArgb(CByte(250), CByte(250), CByte(250))
        pnlRecherche.Controls.Add(btnFermerRecherche)
        pnlRecherche.Controls.Add(btnAfficherRemplacer)
        pnlRecherche.Controls.Add(chkCasse)
        pnlRecherche.Controls.Add(btnOccSuivante)
        pnlRecherche.Controls.Add(btnOccPrecedente)
        pnlRecherche.Controls.Add(lblOccurrences)
        pnlRecherche.Controls.Add(txtRecherche)
        pnlRecherche.Dock = DockStyle.Top
        pnlRecherche.Location = New Point(0, 36)
        pnlRecherche.Name = "pnlRecherche"
        pnlRecherche.Size = New Size(1030, 38)
        pnlRecherche.TabIndex = 1
        pnlRecherche.Visible = False
        ' 
        ' txtRecherche
        ' 
        txtRecherche.Font = New Font("Calibri", 10.5F)
        txtRecherche.Location = New Point(8, 4)
        txtRecherche.Name = "txtRecherche"
        txtRecherche.Size = New Size(240, 30)
        txtRecherche.TabIndex = 0
        txtRecherche.TexteAide = "Rechercher dans le texte"
        ' 
        ' lblOccurrences
        ' 
        lblOccurrences.Font = New Font("Calibri", 10F)
        lblOccurrences.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblOccurrences.Location = New Point(254, 4)
        lblOccurrences.Name = "lblOccurrences"
        lblOccurrences.Size = New Size(118, 30)
        lblOccurrences.TabIndex = 1
        lblOccurrences.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' btnOccPrecedente
        ' 
        btnOccPrecedente.Image = CType(resources.GetObject("btnOccPrecedente.Image"), Image)
        btnOccPrecedente.Location = New Point(374, 4)
        btnOccPrecedente.Name = "btnOccPrecedente"
        btnOccPrecedente.Size = New Size(30, 30)
        btnOccPrecedente.TabIndex = 2
        btnOccPrecedente.Tag = "occPrecedente_20_normal"
        infobulle.SetToolTip(btnOccPrecedente, "Occurrence précédente (Maj+F3)")
        btnOccPrecedente.Variante = VarianteBouton.Outil
        ' 
        ' btnOccSuivante
        ' 
        btnOccSuivante.Image = CType(resources.GetObject("btnOccSuivante.Image"), Image)
        btnOccSuivante.Location = New Point(406, 4)
        btnOccSuivante.Name = "btnOccSuivante"
        btnOccSuivante.Size = New Size(30, 30)
        btnOccSuivante.TabIndex = 3
        btnOccSuivante.Tag = "occSuivante_20_normal"
        infobulle.SetToolTip(btnOccSuivante, "Occurrence suivante (F3)")
        btnOccSuivante.Variante = VarianteBouton.Outil
        ' 
        ' chkCasse
        ' 
        chkCasse.AutoSize = True
        chkCasse.Font = New Font("Calibri", 10F)
        chkCasse.Location = New Point(446, 9)
        chkCasse.Name = "chkCasse"
        chkCasse.Size = New Size(130, 21)
        chkCasse.TabIndex = 4
        chkCasse.Text = "Respecter la casse"
        ' 
        ' btnAfficherRemplacer
        ' 
        btnAfficherRemplacer.Location = New Point(590, 4)
        btnAfficherRemplacer.Name = "btnAfficherRemplacer"
        btnAfficherRemplacer.Size = New Size(104, 30)
        btnAfficherRemplacer.TabIndex = 5
        btnAfficherRemplacer.Text = "Remplacer…"
        infobulle.SetToolTip(btnAfficherRemplacer, "Rechercher et remplacer (Ctrl+H)")
        btnAfficherRemplacer.Variante = VarianteBouton.Outil
        ' 
        ' btnFermerRecherche
        ' 
        btnFermerRecherche.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnFermerRecherche.Image = CType(resources.GetObject("btnFermerRecherche.Image"), Image)
        btnFermerRecherche.Location = New Point(992, 4)
        btnFermerRecherche.Name = "btnFermerRecherche"
        btnFermerRecherche.Size = New Size(30, 30)
        btnFermerRecherche.TabIndex = 6
        btnFermerRecherche.Tag = "fermer_20_normal"
        infobulle.SetToolTip(btnFermerRecherche, "Fermer la recherche (Échap)")
        btnFermerRecherche.Variante = VarianteBouton.Outil
        ' 
        ' pnlRemplacer
        ' 
        pnlRemplacer.BackColor = Color.FromArgb(CByte(250), CByte(250), CByte(250))
        pnlRemplacer.Controls.Add(btnRemplacerTout)
        pnlRemplacer.Controls.Add(btnRemplacer)
        pnlRemplacer.Controls.Add(txtRemplacer)
        pnlRemplacer.Dock = DockStyle.Top
        pnlRemplacer.Location = New Point(0, 74)
        pnlRemplacer.Name = "pnlRemplacer"
        pnlRemplacer.Size = New Size(1030, 38)
        pnlRemplacer.TabIndex = 2
        pnlRemplacer.Visible = False
        ' 
        ' txtRemplacer
        ' 
        txtRemplacer.Font = New Font("Calibri", 11F)
        txtRemplacer.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtRemplacer.Location = New Point(8, 6)
        txtRemplacer.Name = "txtRemplacer"
        txtRemplacer.PlaceholderText = "Remplacer par"
        txtRemplacer.Size = New Size(240, 25)
        txtRemplacer.TabIndex = 0
        ' 
        ' btnRemplacer
        ' 
        btnRemplacer.Location = New Point(254, 4)
        btnRemplacer.Name = "btnRemplacer"
        btnRemplacer.Size = New Size(100, 30)
        btnRemplacer.TabIndex = 1
        btnRemplacer.Text = "Remplacer"
        infobulle.SetToolTip(btnRemplacer, "Remplacer l'occurrence sélectionnée (Entrée)")
        btnRemplacer.Variante = VarianteBouton.Outil
        ' 
        ' btnRemplacerTout
        ' 
        btnRemplacerTout.Location = New Point(358, 4)
        btnRemplacerTout.Name = "btnRemplacerTout"
        btnRemplacerTout.Size = New Size(120, 30)
        btnRemplacerTout.TabIndex = 2
        btnRemplacerTout.Text = "Tout remplacer"
        btnRemplacerTout.Variante = VarianteBouton.Outil
        ' 
        ' pnlEtat
        ' 
        pnlEtat.BackColor = Color.FromArgb(CByte(250), CByte(250), CByte(250))
        pnlEtat.Controls.Add(lblModifie)
        pnlEtat.Controls.Add(lblMots)
        pnlEtat.Controls.Add(lblZoom)
        pnlEtat.Dock = DockStyle.Bottom
        pnlEtat.Location = New Point(0, 438)
        pnlEtat.Name = "pnlEtat"
        pnlEtat.Padding = New Padding(8, 1, 8, 0)
        pnlEtat.Size = New Size(1030, 24)
        pnlEtat.TabIndex = 4
        ' 
        ' lblMots
        ' 
        lblMots.AutoSize = True
        lblMots.Dock = DockStyle.Left
        lblMots.Font = New Font("Calibri", 9.75F)
        lblMots.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblMots.Name = "lblMots"
        lblMots.Padding = New Padding(0, 4, 0, 0)
        lblMots.TabIndex = 0
        lblMots.Text = "0 mot"
        ' 
        ' lblModifie
        ' 
        lblModifie.AutoSize = True
        lblModifie.Dock = DockStyle.Left
        lblModifie.Font = New Font("Calibri", 9.75F)
        lblModifie.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblModifie.Name = "lblModifie"
        lblModifie.Padding = New Padding(6, 4, 0, 0)
        lblModifie.TabIndex = 0
        lblModifie.Text = ""
        ' 
        ' lblZoom
        ' 
        lblZoom.AutoSize = True
        lblZoom.Cursor = Cursors.Hand
        lblZoom.Dock = DockStyle.Right
        lblZoom.Font = New Font("Calibri", 9.75F)
        lblZoom.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblZoom.Name = "lblZoom"
        lblZoom.Padding = New Padding(0, 4, 0, 0)
        lblZoom.TabIndex = 0
        lblZoom.Text = "Zoom 100 %"
        infobulle.SetToolTip(lblZoom, "Ctrl + molette pour zoomer ; clic : 100 %")
        ' 
        ' rtbEditor
        ' 
        rtbEditor.AcceptsTab = True
        rtbEditor.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        rtbEditor.BorderStyle = BorderStyle.None
        rtbEditor.ContextMenuStrip = menuContextuel
        rtbEditor.Dock = DockStyle.Fill
        rtbEditor.EnableAutoDragDrop = True
        rtbEditor.Font = New Font("Calibri", 11F)
        rtbEditor.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        rtbEditor.HideSelection = False
        rtbEditor.Location = New Point(0, 112)
        rtbEditor.Name = "rtbEditor"
        rtbEditor.ScrollBars = RichTextBoxScrollBars.Vertical
        rtbEditor.Size = New Size(1030, 326)
        rtbEditor.TabIndex = 3
        rtbEditor.Text = ""
        ' 
        ' menuContextuel
        ' 
        menuContextuel.Font = New Font("Calibri", 10.5F)
        menuContextuel.ImageScalingSize = New Size(20, 20)
        menuContextuel.Items.AddRange(New ToolStripItem() {mnuCouper, mnuCopier, mnuColler, mnuCollerTexte, sepContexte, mnuToutSelectionner})
        menuContextuel.Name = "menuContextuel"
        menuContextuel.Size = New Size(260, 160)
        ' 
        ' mnuCouper
        ' 
        mnuCouper.Image = CType(resources.GetObject("mnuCouper.Image"), Image)
        mnuCouper.Name = "mnuCouper"
        mnuCouper.ShortcutKeyDisplayString = "Ctrl+X"
        mnuCouper.Size = New Size(259, 26)
        mnuCouper.Text = "Couper"
        ' 
        ' mnuCopier
        ' 
        mnuCopier.Image = CType(resources.GetObject("mnuCopier.Image"), Image)
        mnuCopier.Name = "mnuCopier"
        mnuCopier.ShortcutKeyDisplayString = "Ctrl+C"
        mnuCopier.Size = New Size(259, 26)
        mnuCopier.Text = "Copier"
        ' 
        ' mnuColler
        ' 
        mnuColler.Image = CType(resources.GetObject("mnuColler.Image"), Image)
        mnuColler.Name = "mnuColler"
        mnuColler.ShortcutKeyDisplayString = "Ctrl+V"
        mnuColler.Size = New Size(259, 26)
        mnuColler.Text = "Coller"
        ' 
        ' mnuCollerTexte
        ' 
        mnuCollerTexte.Image = CType(resources.GetObject("mnuCollerTexte.Image"), Image)
        mnuCollerTexte.Name = "mnuCollerTexte"
        mnuCollerTexte.ShortcutKeyDisplayString = "Ctrl+Maj+V"
        mnuCollerTexte.Size = New Size(259, 26)
        mnuCollerTexte.Text = "Coller sans mise en forme"
        ' 
        ' sepContexte
        ' 
        sepContexte.Name = "sepContexte"
        sepContexte.Size = New Size(256, 6)
        ' 
        ' mnuToutSelectionner
        ' 
        mnuToutSelectionner.Name = "mnuToutSelectionner"
        mnuToutSelectionner.ShortcutKeyDisplayString = "Ctrl+A"
        mnuToutSelectionner.Size = New Size(259, 26)
        mnuToutSelectionner.Text = "Tout sélectionner"
        ' 
        ' UC_RichTextEditor
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Transparent
        Controls.Add(rtbEditor)
        Controls.Add(pnlEtat)
        Controls.Add(pnlRemplacer)
        Controls.Add(pnlRecherche)
        Controls.Add(toolStrip)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_RichTextEditor"
        Size = New Size(1030, 462)
        toolStrip.ResumeLayout(False)
        toolStrip.PerformLayout()
        pnlRecherche.ResumeLayout(False)
        pnlRecherche.PerformLayout()
        pnlRemplacer.ResumeLayout(False)
        pnlRemplacer.PerformLayout()
        pnlEtat.ResumeLayout(False)
        pnlEtat.PerformLayout()
        menuContextuel.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents toolStrip As ToolStrip
    Friend WithEvents lblLectureSeule As ToolStripLabel
    Friend WithEvents sepLecture As ToolStripSeparator
    Friend WithEvents btnAnnuler As ToolStripButton
    Friend WithEvents btnRetablir As ToolStripButton
    Friend WithEvents sepHistorique As ToolStripSeparator
    Friend WithEvents btnStyle As ToolStripDropDownButton
    Friend WithEvents sepStyle As ToolStripSeparator
    Friend WithEvents btnGras As ToolStripButton
    Friend WithEvents btnItalique As ToolStripButton
    Friend WithEvents btnSouligne As ToolStripButton
    Friend WithEvents btnBarre As ToolStripButton
    Friend WithEvents btnCouleur As ToolStripDropDownButton
    Friend WithEvents btnSurligneur As ToolStripDropDownButton
    Friend WithEvents btnEffacer As ToolStripButton
    Friend WithEvents sepCaractere As ToolStripSeparator
    Friend WithEvents btnPuces As ToolStripButton
    Friend WithEvents btnNumeros As ToolStripButton
    Friend WithEvents btnRetraitMoins As ToolStripButton
    Friend WithEvents btnRetraitPlus As ToolStripButton
    Friend WithEvents btnAlignement As ToolStripDropDownButton
    Friend WithEvents sepParagraphe As ToolStripSeparator
    Friend WithEvents btnDate As ToolStripDropDownButton
    Friend WithEvents btnTextesTypes As ToolStripDropDownButton
    Friend WithEvents sepInsertion As ToolStripSeparator
    Friend WithEvents btnRechercher As ToolStripButton
    Friend WithEvents btnCopier As ToolStripButton
    Friend WithEvents btnExporter As ToolStripDropDownButton
    Friend WithEvents btnImprimer As ToolStripButton
    Friend WithEvents btnApercu As ToolStripButton
    Friend WithEvents btnOuvrirEnGrand As ToolStripButton
    Friend WithEvents mnuAlignGauche As ToolStripMenuItem
    Friend WithEvents mnuAlignCentre As ToolStripMenuItem
    Friend WithEvents mnuAlignDroite As ToolStripMenuItem
    Friend WithEvents mnuAlignJustifier As ToolStripMenuItem
    Friend WithEvents mnuExportPdf As ToolStripMenuItem
    Friend WithEvents mnuExportWord As ToolStripMenuItem
    Friend WithEvents sepExport As ToolStripSeparator
    Friend WithEvents mnuMiseEnPage As ToolStripMenuItem
    Friend WithEvents pnlRecherche As Panel
    Friend WithEvents txtRecherche As AltheaRecherche
    Friend WithEvents lblOccurrences As Label
    Friend WithEvents btnOccPrecedente As AltheaBouton
    Friend WithEvents btnOccSuivante As AltheaBouton
    Friend WithEvents chkCasse As AltheaCase
    Friend WithEvents btnAfficherRemplacer As AltheaBouton
    Friend WithEvents btnFermerRecherche As AltheaBouton
    Friend WithEvents pnlRemplacer As Panel
    Friend WithEvents txtRemplacer As TextBox
    Friend WithEvents btnRemplacer As AltheaBouton
    Friend WithEvents btnRemplacerTout As AltheaBouton
    Friend WithEvents pnlEtat As Panel
    Friend WithEvents lblMots As Label
    Friend WithEvents lblModifie As Label
    Friend WithEvents lblZoom As Label
    Friend WithEvents rtbEditor As RichTextBox
    Friend WithEvents menuContextuel As ContextMenuStrip
    Friend WithEvents mnuCouper As ToolStripMenuItem
    Friend WithEvents mnuCopier As ToolStripMenuItem
    Friend WithEvents mnuColler As ToolStripMenuItem
    Friend WithEvents mnuCollerTexte As ToolStripMenuItem
    Friend WithEvents sepContexte As ToolStripSeparator
    Friend WithEvents mnuToutSelectionner As ToolStripMenuItem
    Friend WithEvents infobulle As ToolTip

End Class
