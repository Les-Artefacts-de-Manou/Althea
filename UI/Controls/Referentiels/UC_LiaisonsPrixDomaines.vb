' -------------------------------------------------------------------------------------------------
' UserControl : UC_LiaisonsPrixDomaines
' Projet      : Althéa
' Version     : V2.3.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Panneau intégré d'administration des rattachements historisés entre tarifs de séance et
' domaines (lia_prix_seance_domaines). Conçu pour être hébergé dans un onglet d'un écran
' parent, sans passer par une fenêtre modale (D-POR-4).
'
' Responsabilités :
' - Afficher l'historique des rattachements de l'élément parent (un tarif, ou un domaine)
' - Créer un nouveau rattachement avec sa période de validité
' - Clôturer un rattachement en vigueur à une date choisie
' - Retirer ou rétablir un rattachement, et supprimer une ligne créée par erreur
'
' Présentation (V2.0.0, UI-6 — maquette « Domaines — liaisons », ADR-34) :
' - Une carte du kit : barre locale en haut (actions de la liste), grille sur toute la largeur,
'   note d'aide, puis un formulaire au pied de la carte qui n'apparaît qu'à la demande :
'   « Rattacher un tarif » (nouveau rattachement) ou « Clôturer la période » (date de fin).
' - L'action « Créer un nouveau … » de l'écran hôte se place à droite de la barre (TexteCreation,
'   événement CreationDemandee) : l'hôte ouvre le référentiel concerné.
' - Remplace la V1 (boutons empilés à droite ou sous la grille, bandeau sombre, MessageBox).
' - V2.1.0 (18/10/2026, UI-9.4) : état en pastille (LiaisonPrixDomaine.Etat : en vigueur, à venir,
'   clôturé, désactivé), rattachement clôturé ou désactivé atténué.
'
' Remarques   :
' - Aucun Designer : les contrôles sont créés par code (exception admise, Rules § 9), car le même
'   panneau sert dans deux écrans et deux sens de lecture.
' - Aucun accès direct à la base : tout passe par GestionLiaisonsPrixDomaines.
' - La validation métier (chevauchement, ambiguïté de tarif conventionné, cohérence des dates)
'   est déléguée à GestionLiaisonsPrixDomaines.ValiderLiaison afin que tous les écrans
'   affichent exactement les mêmes messages.
'
' Dépendances :
' - GestionLiaisonsPrixDomaines, LiaisonPrixDomaine, GestionDomaines, GestionPrixSeance
' - Kit Althéa (AltheaCarte, AltheaBarreOutils, AltheaBouton, AltheaListe, AltheaCase)
' - UtilsDataGrid, UtilsButtons, UITheme, UtilsPastilles, DialogChoix, GestionLog
' - V2.2.0 (20/10/2026) : dates de validité en AltheaDate (champ du kit, au lieu des DateTimePicker).
' - V2.3.0 (22/10/2026, audit C.1, étape 3) : FournisseurContexte (ContexteOperation du parent)
'   transmis aux écritures, vérifiées par le métier.
' - V2.3.1 (25/10/2026, relevé de Joëlle au test NAV-C4-09) : l'action « Créer un nouveau … »
'   n'apparaissait pas : sa place était calculée d'après _btnCreer.Visible (False tant que l'écran
'   n'est pas affiché) et l'ancrage à droite la repoussait hors de la barre. Drapeau _avecCreation,
'   place recalculée à l'affichage, plus d'ancrage.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_LiaisonsPrixDomaines
    Inherits UserControl

#Region "Énumérations"

    ' -------------------------------------------------------------------------------------------------
    ' Énumération : SensLiaison
    ' Rôle        : Indique quel est l'élément parent et donc ce que l'utilisateur rattache.
    ' -------------------------------------------------------------------------------------------------
    Public Enum SensLiaison
        ' Parent = un tarif de séance : on rattache des domaines
        ParPrix = 0
        ' Parent = un domaine : on rattache des tarifs de séance
        ParDomaine = 1
    End Enum

    ' Contenu du formulaire au pied de la carte.
    Private Enum ModeFormulaire
        Ferme
        Rattachement
        Cloture
    End Enum

#End Region

#Region "Événements"

    ''' <summary>L'utilisateur demande la création d'un nouvel élément (bouton TexteCreation).</summary>
    Public Event CreationDemandee As EventHandler

    ''' <summary>Un rattachement vient d'être créé, modifié ou supprimé (l'hôte peut recalculer
    ''' l'état de l'élément parent).</summary>
    Public Event RattachementsModifies As EventHandler

#End Region

#Region "Variables privées"

    ' Carte et barre locale
    Private ReadOnly _carte As New AltheaCarte()
    Private ReadOnly _barre As New AltheaBarreOutils()
    Private ReadOnly _btnRattacher As New AltheaBouton()
    Private ReadOnly _btnCloturer As New AltheaBouton()
    Private ReadOnly _btnBasculer As New AltheaBouton()
    Private ReadOnly _btnSupprimer As New AltheaBouton()
    Private ReadOnly _btnCreer As New AltheaBouton()

    ' Action « Créer un nouveau … » demandée par l'hôte (TexteCreation). Un drapeau et non
    ' _btnCreer.Visible, qui vaut False tant que l'écran n'est pas affiché (onglet, étape).
    Private _avecCreation As Boolean

    ' Contenu de la carte
    Private ReadOnly _dgv As New DataGridView()
    Private ReadOnly _lblAide As New Label()

    ' Formulaire au pied de la carte
    Private ReadOnly _pied As New FlowLayoutPanel()
    Private ReadOnly _lblFormulaire As New Label()
    Private ReadOnly _cboAjout As New AltheaListe()
    Private ReadOnly _lblDebut As New Label()
    Private ReadOnly _dtpDebut As New AltheaDate()
    Private ReadOnly _chkFin As New AltheaCase()
    Private ReadOnly _dtpFin As New AltheaDate()
    Private ReadOnly _btnValider As New AltheaBouton()
    Private ReadOnly _btnAnnulerFormulaire As New AltheaBouton()

    Private ReadOnly _toolTip As New ToolTip()

    ' Identifiant de l'élément parent affiché (0 = aucune sélection)
    Private _idParent As ULong = 0

    ' Rattachements actuellement affichés
    Private _liaisons As New List(Of LiaisonPrixDomaine)

    ' Empêche toute écriture lorsque l'écran parent est en édition ou en consultation seule
    Private _lectureSeule As Boolean = False

    ' Sens de lecture courant (voir la propriété Mode)
    Private _mode As SensLiaison = SensLiaison.ParPrix

    Private _modeFormulaire As ModeFormulaire = ModeFormulaire.Ferme

#End Region

#Region "Propriétés"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : Mode
    ' Rôle      : Détermine si le parent est un tarif de séance ou un domaine.
    ' Remarques : Renseignée par l'écran parent APRÈS la construction du contrôle : sa modification
    '             reconstruit les colonnes et les libellés.
    ' -------------------------------------------------------------------------------------------------
    Public Property Mode As SensLiaison
        Get
            Return _mode
        End Get
        Set(value As SensLiaison)

            If _mode = value Then Return

            _mode = value
            ConstruireColonnes()
            AppliquerLibellesMode()

        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : FournisseurContexte
    ' Rôle      : Donne, à l'instant d'une écriture, le ContexteOperation de l'écran parent (qui
    '             reçoit son contexte après avoir construit ce panneau). Renseignée par le parent.
    ' Remarques : Sans fournisseur, le métier refuse l'écriture (utilisateur inconnu) — audit C.1.
    ' -------------------------------------------------------------------------------------------------
    Public Property FournisseurContexte As Func(Of ContexteOperation)

    Private Function Contexte() As ContexteOperation
        Return FournisseurContexte?.Invoke()
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : TexteCreation
    ' Rôle      : Libellé de l'action « Créer un nouveau … » à droite de la barre (vide = masquée).
    ' -------------------------------------------------------------------------------------------------
    Public Property TexteCreation As String
        Get
            Return _btnCreer.Text
        End Get
        Set(value As String)
            _btnCreer.Text = If(value, String.Empty)
            _avecCreation = _btnCreer.Text.Length > 0
            _btnCreer.Visible = _avecCreation
            PlacerBoutonsBarre()
        End Set
    End Property

#End Region

#Region "Construction"

    Public Sub New()

        Dock = DockStyle.Fill
        BackColor = Color.Transparent
        Padding = New Padding(0)

        ConstruireInterface()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConstruireInterface
    ' Rôle      : Crée la carte, sa barre locale, la grille, la note d'aide et le formulaire.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConstruireInterface()

        _carte.Dock = DockStyle.Fill
        _carte.Padding = New Padding(12, 8, 12, 12)

        ' Barre locale (ADR-34) : boutons Outil, Supprimer en dernier, création à droite.
        PreparerBoutonBarre(_btnRattacher, "Rattacher un tarif", "rattacher_normal", VarianteBouton.OutilPrincipal)
        PreparerBoutonBarre(_btnCloturer, "Clôturer la période", "cloturer_normal", VarianteBouton.Outil)
        PreparerBoutonBarre(_btnBasculer, "Désactiver", "activer_normal", VarianteBouton.Outil)
        PreparerBoutonBarre(_btnSupprimer, "Supprimer", "supprimer_normal", VarianteBouton.OutilDanger)
        PreparerBoutonBarre(_btnCreer, String.Empty, Nothing, VarianteBouton.Outil)
        _btnCreer.Visible = False

        _barre.Controls.AddRange(New Control() {_btnRattacher, _btnCloturer, _btnBasculer, _btnSupprimer, _btnCreer})

        ' Grille
        _dgv.Dock = DockStyle.Fill
        _dgv.AutoGenerateColumns = False
        _dgv.ReadOnly = True
        _dgv.MultiSelect = False
        _dgv.AllowUserToAddRows = False
        _dgv.AllowUserToDeleteRows = False
        _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        UtilsDataGrid.InitDataGridSecondaire(_dgv)

        ' Note d'aide contextuelle, sous la grille
        _lblAide.Dock = DockStyle.Bottom
        _lblAide.Height = 38
        _lblAide.BackColor = Color.Transparent
        _lblAide.ForeColor = UITheme.ColorSaugeTresFonce
        _lblAide.Font = New Font("Calibri", 9.75F)
        _lblAide.Padding = New Padding(2, 4, 2, 0)
        _lblAide.TextAlign = ContentAlignment.MiddleLeft

        ConstruireFormulaire()

        ' Ordre d'ancrage : le dernier ajouté se place en premier (barre en haut, formulaire en bas).
        _carte.Controls.Add(_dgv)
        _carte.Controls.Add(_lblAide)
        _carte.Controls.Add(_pied)
        _carte.Controls.Add(_barre)
        Controls.Add(_carte)

        ConstruireColonnes()
        AppliquerLibellesMode()

        AddHandler _barre.Resize, Sub() PlacerBoutonsBarre()
        AddHandler _barre.VisibleChanged, Sub() PlacerBoutonsBarre()
        AddHandler _btnRattacher.Click, AddressOf BtnRattacher_Click
        AddHandler _btnCloturer.Click, AddressOf BtnCloturer_Click
        AddHandler _btnBasculer.Click, AddressOf BtnBasculer_Click
        AddHandler _btnSupprimer.Click, AddressOf BtnSupprimer_Click
        AddHandler _btnCreer.Click, Sub() RaiseEvent CreationDemandee(Me, EventArgs.Empty)
        AddHandler _btnValider.Click, AddressOf BtnValider_Click
        AddHandler _btnAnnulerFormulaire.Click, Sub() AfficherFormulaire(ModeFormulaire.Ferme)
        AddHandler _chkFin.CheckedChanged, AddressOf ChkFin_CheckedChanged
        AddHandler _dgv.SelectionChanged, AddressOf Dgv_SelectionChanged
        AddHandler _dgv.CellFormatting, AddressOf Dgv_CellFormatting
        AddHandler _dgv.CellPainting, AddressOf Dgv_CellPainting

        InitialiserToolTips()
        AfficherFormulaire(ModeFormulaire.Ferme)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConstruireFormulaire
    ' Rôle      : Formulaire au pied de la carte, sur une ligne (passe à la ligne si l'écran est étroit).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConstruireFormulaire()

        _pied.Dock = DockStyle.Bottom
        _pied.AutoSize = True
        _pied.AutoSizeMode = AutoSizeMode.GrowAndShrink
        _pied.WrapContents = True
        _pied.BackColor = Color.Transparent
        _pied.Padding = New Padding(0, 6, 0, 0)

        PreparerLibelle(_lblFormulaire, String.Empty)
        _lblFormulaire.ForeColor = UITheme.ColorMauveFonce
        _lblFormulaire.Font = New Font("Calibri", 10.0F, FontStyle.Bold)

        _cboAjout.Size = New Size(260, 30)
        _cboAjout.Margin = New Padding(3, 3, 12, 3)

        PreparerLibelle(_lblDebut, "À partir du")
        PreparerDate(_dtpDebut)

        _chkFin.AutoSize = False
        _chkFin.Size = New Size(100, 30)
        _chkFin.Text = "Jusqu'au"
        _chkFin.Font = New Font("Calibri", 10.0F)
        _chkFin.Margin = New Padding(3, 3, 0, 3)
        PreparerDate(_dtpFin)

        PreparerBoutonBarre(_btnValider, "Rattacher", Nothing, VarianteBouton.OutilPrincipal)
        PreparerBoutonBarre(_btnAnnulerFormulaire, "Annuler", Nothing, VarianteBouton.Outil)
        _btnValider.Margin = New Padding(12, 3, 3, 3)

        _pied.Controls.AddRange(New Control() {
            _lblFormulaire, _cboAjout, _lblDebut, _dtpDebut, _chkFin, _dtpFin,
            _btnValider, _btnAnnulerFormulaire})

    End Sub

    Private Shared Sub PreparerLibelle(lbl As Label, texte As String)

        lbl.AutoSize = False
        lbl.Text = texte
        lbl.ForeColor = UITheme.ColorSaugeFonce
        lbl.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        lbl.TextAlign = ContentAlignment.MiddleLeft
        lbl.Height = 30
        lbl.Width = Math.Max(10, TextRenderer.MeasureText(texte, lbl.Font).Width + 6)
        lbl.Margin = New Padding(3, 3, 3, 3)

    End Sub

    ' Champ de date du kit (AltheaDate), à la hauteur des autres champs de la barre.
    Private Shared Sub PreparerDate(dtp As AltheaDate)

        dtp.Font = New Font("Calibri", 10.5F)
        dtp.Size = New Size(130, 28)
        dtp.Margin = New Padding(3, 2, 12, 3)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : PreparerBoutonBarre
    ' Rôle      : Bouton léger du kit (variantes Outil) ; l'icône vient du Tag (…_normal, UtilsButtons).
    ' -------------------------------------------------------------------------------------------------
    Private Shared Sub PreparerBoutonBarre(btn As AltheaBouton, texte As String, tagIcone As String,
                                           variante As VarianteBouton)

        btn.Variante = variante
        btn.Text = texte
        btn.Height = 30
        btn.Margin = New Padding(3)

        If Not String.IsNullOrEmpty(tagIcone) Then
            btn.Tag = tagIcone
            UtilsButtons.InitStandardButton(btn)
        End If

        AjusterLargeur(btn)

    End Sub

    ' Largeur au plus juste : icône + texte + marges intérieures d'AltheaBouton.
    Private Shared Sub AjusterLargeur(btn As AltheaBouton)

        Dim icone As Integer = If(btn.Image Is Nothing, 0, 18 + 8)
        btn.Width = TextRenderer.MeasureText(btn.Text, btn.Font).Width + icone + 26

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : PlacerBoutonsBarre
    ' Rôle      : Aligne les actions après le titre de la barre ; Supprimer est séparé des autres ;
    '             l'action de création est calée à droite.
    ' -------------------------------------------------------------------------------------------------
    Private Sub PlacerBoutonsBarre()

        Dim y As Integer = Math.Max(0, (_barre.Height - 30) \ 2)
        Dim x As Integer = _barre.MargeTitre

        For Each btn As AltheaBouton In {_btnRattacher, _btnCloturer, _btnBasculer, _btnSupprimer}
            If btn Is _btnSupprimer Then x += 10
            btn.Location = New Point(x, y)
            x += btn.Width + 4
        Next

        If _avecCreation Then
            AjusterLargeur(_btnCreer)
            _btnCreer.Location = New Point(Math.Max(x + 10, _barre.Width - _barre.Padding.Right - _btnCreer.Width), y)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerLibellesMode
    ' Rôle      : Titre de la barre et libellé du bouton principal selon ce que l'on rattache.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerLibellesMode()

        If Mode = SensLiaison.ParPrix Then
            _barre.Titre = "Domaines où ce tarif s'applique"
            _btnRattacher.Text = "Rattacher un domaine"
        Else
            _barre.Titre = "Tarifs rattachés"
            _btnRattacher.Text = "Rattacher un tarif"
        End If

        AjusterLargeur(_btnRattacher)
        PlacerBoutonsBarre()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Rôle      : Explique le rôle de chaque action. Zone délicate : un tarif évolue par périodes de
    '             validité historisées ; il ne faut jamais « modifier » un montant en place mais
    '             clôturer la période courante et créer un nouveau rattachement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        _toolTip.AutoPopDelay = 20000
        _toolTip.InitialDelay = 400
        _toolTip.ReshowDelay = 100

        _toolTip.SetToolTip(_btnRattacher,
            "Ouvre, au bas de la carte, la saisie d'un nouveau rattachement et de sa période de validité.")

        _toolTip.SetToolTip(_btnCloturer,
            "Ferme la période de validité de la ligne sélectionnée à une date donnée, sans la supprimer." & Environment.NewLine &
            "Pour faire évoluer un tarif : clôturez à la veille de la nouvelle date," & Environment.NewLine &
            "puis rattachez le nouveau tarif. L'historique est préservé.")

        _toolTip.SetToolTip(_btnSupprimer,
            "Supprime définitivement ce rattachement et son historique de période." & Environment.NewLine &
            "Réservé aux erreurs de saisie." & Environment.NewLine &
            "Pour arrêter un tarif tout en gardant l'historique, préférez « Clôturer la période » ou « Désactiver ».")

        _toolTip.SetToolTip(_chkFin,
            "Cochez pour borner la période par une date de fin." & Environment.NewLine &
            "Laissez décoché pour un rattachement à durée indéterminée (« Depuis le … »).")

        ' _btnBasculer bascule entre « Désactiver » et « Réactiver » : infobulle ajustée dynamiquement.

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConstruireColonnes
    ' Rôle      : Définit les colonnes de la grille selon le sens de lecture.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConstruireColonnes()

        _dgv.Columns.Clear()

        Dim libelleCible As String = If(Mode = SensLiaison.ParPrix, "Domaine", "Tarif de séance")
        Dim propLibelle As String =
            If(Mode = SensLiaison.ParPrix,
               NameOf(LiaisonPrixDomaine.LibelleDomaine),
               NameOf(LiaisonPrixDomaine.LibellePrixSeance))

        _dgv.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colLibelle", .HeaderText = libelleCible, .DataPropertyName = propLibelle,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 200, .ReadOnly = True})

        _dgv.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colMontant", .HeaderText = "Montant",
            .DataPropertyName = NameOf(LiaisonPrixDomaine.MontantAffichage),
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 90, .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleRight}})

        _dgv.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colPeriode", .HeaderText = "Période de validité",
            .DataPropertyName = NameOf(LiaisonPrixDomaine.LibellePeriodeValidite),
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 160, .ReadOnly = True})

        _dgv.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colEtat", .HeaderText = "État",
            .DataPropertyName = NameOf(LiaisonPrixDomaine.LibelleEtat),
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 90, .ReadOnly = True})

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédures : Dgv_CellFormatting / Dgv_CellPainting
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    ' Rôle       : Rattachement clôturé ou désactivé atténué ; état en pastille.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Dgv_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 Then Return
        Dim liaison As LiaisonPrixDomaine = TryCast(_dgv.Rows(e.RowIndex).DataBoundItem, LiaisonPrixDomaine)
        If liaison IsNot Nothing AndAlso (liaison.Etat = EtatLiaisonPrix.Cloture OrElse liaison.Etat = EtatLiaisonPrix.Desactive) Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If
    End Sub

    Private Sub Dgv_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse _dgv.Columns(e.ColumnIndex).Name <> "colEtat" Then Return
        Dim liaison As LiaisonPrixDomaine = TryCast(_dgv.Rows(e.RowIndex).DataBoundItem, LiaisonPrixDomaine)
        If liaison Is Nothing Then Return
        UtilsDataGrid.DessinerPastilleCellule(_dgv, e, liaison.LibelleEtat, UtilsPastilles.Teinte(liaison.Etat))
    End Sub

#End Region

#Region "Chargement"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChargerPour
    ' Rôle       : Charge les rattachements de l'élément parent sélectionné.
    ' Paramètres : idParent - identifiant du tarif ou du domaine ; 0 pour vider le panneau
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerPour(idParent As ULong)

        _idParent = idParent
        AfficherFormulaire(ModeFormulaire.Ferme)

        If _dgv.Columns.Count = 0 Then ConstruireColonnes()

        If _idParent = 0 Then
            _liaisons = New List(Of LiaisonPrixDomaine)
            _dgv.DataSource = Nothing
            _cboAjout.DataSource = Nothing
            ActualiserBoutons()
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            _liaisons = If(Mode = SensLiaison.ParPrix,
                           GestionLiaisonsPrixDomaines.GetLiaisonsParPrix(_idParent),
                           GestionLiaisonsPrixDomaines.GetLiaisonsParDomaine(_idParent))

            _dgv.DataSource = Nothing
            _dgv.DataSource = _liaisons
            _dgv.ClearSelection()

            ChargerListeAjout()
            ActualiserBoutons()

        Catch ex As Exception
            GestionLog.EcrireException("Erreur de chargement des rattachements tarif / domaine.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            DialogChoix.Avertissement("Impossible de charger les rattachements." & Environment.NewLine &
                                      "Consultez le journal pour le détail.", "Rattachements")
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerListeAjout
    ' Rôle      : Alimente la liste des éléments rattachables.
    ' Remarques : L'historisation autorise plusieurs lignes pour un même couple : la liste reste
    '             complète et c'est la validation métier qui refuse les chevauchements.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerListeAjout()

        _cboAjout.DataSource = Nothing

        If Mode = SensLiaison.ParPrix Then
            _cboAjout.DisplayMember = NameOf(Domaine.LibelleDomaine)
            _cboAjout.ValueMember = NameOf(Domaine.IdDomaine)
            _cboAjout.DataSource = GestionDomaines.GetDomaines(False)
        Else
            _cboAjout.DisplayMember = NameOf(PrixSeance.LibellePrixSeance)
            _cboAjout.ValueMember = NameOf(PrixSeance.IdPrixSeance)
            _cboAjout.DataSource = GestionPrixSeance.GetPrixSeance(False)
        End If

        _cboAjout.SelectedIndex = -1

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirLectureSeule
    ' Rôle       : Neutralise les actions d'écriture du panneau (édition du parent, droits).
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirLectureSeule(lectureSeule As Boolean)

        _lectureSeule = lectureSeule
        If lectureSeule Then AfficherFormulaire(ModeFormulaire.Ferme)
        ActualiserBoutons()

    End Sub

    Private Function LiaisonSelectionnee() As LiaisonPrixDomaine

        If _dgv.CurrentRow Is Nothing OrElse Not _dgv.CurrentRow.Selected Then Return Nothing
        Return TryCast(_dgv.CurrentRow.DataBoundItem, LiaisonPrixDomaine)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActualiserBoutons
    ' Rôle      : Ajuste l'état et le libellé des boutons selon la sélection et le mode.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActualiserBoutons()

        Dim modifiable As Boolean = Not _lectureSeule AndAlso _idParent > 0
        Dim liaison As LiaisonPrixDomaine = LiaisonSelectionnee()

        _btnRattacher.Enabled = modifiable AndAlso _cboAjout.Items.Count > 0
        _btnCloturer.Enabled = modifiable AndAlso liaison IsNot Nothing AndAlso Not liaison.DateFinValidite.HasValue
        _btnBasculer.Enabled = modifiable AndAlso liaison IsNot Nothing
        _btnSupprimer.Enabled = modifiable AndAlso liaison IsNot Nothing
        _btnCreer.Enabled = Not _lectureSeule

        Dim texteBascule As String = If(liaison IsNot Nothing AndAlso Not liaison.Actif, "Réactiver", "Désactiver")
        If _btnBasculer.Text <> texteBascule Then
            _btnBasculer.Text = texteBascule
            AjusterLargeur(_btnBasculer)
            PlacerBoutonsBarre()
        End If

        If liaison IsNot Nothing AndAlso Not liaison.Actif Then
            _toolTip.SetToolTip(_btnBasculer,
                "Réactive ce rattachement : le tarif redevient applicable pour la période concernée.")
        Else
            _toolTip.SetToolTip(_btnBasculer,
                "Désactive ce rattachement sans supprimer son historique de période." & Environment.NewLine &
                "Le tarif n'est plus appliqué mais reste consultable ; il peut être réactivé à tout moment.")
        End If

        _lblAide.Text = ConstruireTexteAide(liaison)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConstruireTexteAide
    ' Rôle     : Explique en clair l'effet des actions possibles sur la ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Function ConstruireTexteAide(liaison As LiaisonPrixDomaine) As String

        If _idParent = 0 Then
            Return "Sélectionnez un élément dans la liste pour voir ses rattachements."
        End If

        If liaison Is Nothing Then
            Return "Le tarif conventionné d'un domaine est déterminé par la date de la séance. " &
                   "Pour faire évoluer un tarif : clôturez sa période à la veille de la nouvelle date, puis rattachez le nouveau tarif."
        End If

        If Not liaison.Actif Then
            Return "Ce rattachement est désactivé : il n'est plus utilisé pour résoudre le tarif, " &
                   "mais l'historique est conservé."
        End If

        If Not liaison.DateFinValidite.HasValue Then
            Return "Pour faire évoluer le tarif : clôturez cette période à la veille de la nouvelle " &
                   "date d'application, puis rattachez le nouveau tarif."
        End If

        Return "Cette période est bornée. « Supprimer » n'est à utiliser que pour corriger une " &
               "saisie erronée, jamais pour effacer un historique réellement appliqué."

    End Function

#End Region

#Region "Formulaire au pied de la carte"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherFormulaire
    ' Rôle      : Ouvre le formulaire pour un nouveau rattachement ou une clôture, ou le referme.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherFormulaire(contenu As ModeFormulaire)

        _modeFormulaire = contenu
        Dim rattachement As Boolean = contenu = ModeFormulaire.Rattachement

        _pied.SuspendLayout()

        _cboAjout.Visible = rattachement
        _lblDebut.Visible = rattachement
        _dtpDebut.Visible = rattachement
        _chkFin.Visible = rattachement

        If rattachement Then
            _lblFormulaire.Text = "Nouveau rattachement"
            _btnValider.Text = "Rattacher"
            _cboAjout.SelectedIndex = -1
            _dtpDebut.Valeur = Date.Today
            _chkFin.Checked = False
            _dtpFin.Enabled = False
            _dtpFin.Valeur = Date.Today

        ElseIf contenu = ModeFormulaire.Cloture Then
            Dim liaison As LiaisonPrixDomaine = LiaisonSelectionnee()
            Dim libelle As String = If(liaison Is Nothing, String.Empty,
                                       If(Mode = SensLiaison.ParPrix, liaison.LibelleDomaine, liaison.LibellePrixSeance))
            _lblFormulaire.Text = $"Clôture de « {libelle} » — dernier jour :"
            _btnValider.Text = "Clôturer"
            _dtpFin.Enabled = True
            _dtpFin.Valeur = If(liaison IsNot Nothing AndAlso liaison.DateDebutValidite.Date > Date.Today,
                               liaison.DateDebutValidite.Date, Date.Today)
        End If

        _lblFormulaire.Width = TextRenderer.MeasureText(_lblFormulaire.Text, _lblFormulaire.Font).Width + 6
        AjusterLargeur(_btnValider)

        _pied.Visible = contenu <> ModeFormulaire.Ferme
        _pied.ResumeLayout(True)

        If rattachement Then _cboAjout.Focus()

    End Sub

    Private Sub ChkFin_CheckedChanged(sender As Object, e As EventArgs)

        _dtpFin.Enabled = _chkFin.Checked

        If _chkFin.Checked AndAlso _dtpFin.Valeur.Value.Date < _dtpDebut.Valeur.Value.Date Then
            _dtpFin.Valeur = _dtpDebut.Valeur
        End If

    End Sub

    Private Sub BtnRattacher_Click(sender As Object, e As EventArgs)
        AfficherFormulaire(ModeFormulaire.Rattachement)
    End Sub

    Private Sub BtnCloturer_Click(sender As Object, e As EventArgs)
        If LiaisonSelectionnee() IsNot Nothing Then AfficherFormulaire(ModeFormulaire.Cloture)
    End Sub

    Private Sub BtnValider_Click(sender As Object, e As EventArgs)

        If _modeFormulaire = ModeFormulaire.Rattachement Then
            CreerRattachement()
        ElseIf _modeFormulaire = ModeFormulaire.Cloture Then
            CloturerRattachement()
        End If

    End Sub

#End Region

#Region "Actions"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : CreerRattachement
    ' Rôle      : Crée un rattachement tarif / domaine sur la période saisie.
    ' Remarques : La cohérence (chevauchement, ambiguïté, ordre des dates) est vérifiée par
    '             GestionLiaisonsPrixDomaines.ValiderLiaison.
    ' -------------------------------------------------------------------------------------------------
    Private Sub CreerRattachement()

        If _cboAjout.SelectedValue Is Nothing Then
            DialogChoix.Information("Choisissez d'abord l'élément à rattacher dans la liste.", "Rattachement")
            Return
        End If

        Dim idCible As ULong = Convert.ToUInt64(_cboAjout.SelectedValue)

        Dim liaison As New LiaisonPrixDomaine With {
            .IdPrixSeance = If(Mode = SensLiaison.ParPrix, _idParent, idCible),
            .IdDomaine = If(Mode = SensLiaison.ParPrix, idCible, _idParent),
            .DateDebutValidite = _dtpDebut.Valeur.Value.Date,
            .DateFinValidite = If(_chkFin.Checked, CType(_dtpFin.Valeur.Value.Date, Date?), Nothing),
            .Actif = True
        }

        Dim erreur As String = GestionLiaisonsPrixDomaines.ValiderLiaison(liaison)

        If Not String.IsNullOrEmpty(erreur) Then
            DialogChoix.Avertissement(erreur, "Rattachement impossible")
            Return
        End If

        Try
            GestionLiaisonsPrixDomaines.InsertLiaison(Contexte(), liaison)

            GestionLog.EcrireLog(
                $"Rattachement tarif {liaison.IdPrixSeance} / domaine {liaison.IdDomaine} créé " &
                $"à partir du {liaison.DateDebutValidite:yyyy-MM-dd}.",
                GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)

            ChargerPour(_idParent)
            RaiseEvent RattachementsModifies(Me, EventArgs.Empty)

        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireException("Erreur lors de la création d'un rattachement tarif / domaine.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            DialogChoix.Erreur("Le rattachement n'a pas pu être enregistré.", "Rattachement")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : CloturerRattachement
    ' Rôle      : Clôture la période de la ligne sélectionnée au dernier jour saisi.
    ' Remarques : Refuse une date de fin antérieure au début de la période.
    ' -------------------------------------------------------------------------------------------------
    Private Sub CloturerRattachement()

        Dim liaison As LiaisonPrixDomaine = LiaisonSelectionnee()
        If liaison Is Nothing Then
            AfficherFormulaire(ModeFormulaire.Ferme)
            Return
        End If

        Dim dateFin As Date = _dtpFin.Valeur.Value.Date

        If dateFin < liaison.DateDebutValidite.Date Then
            DialogChoix.Avertissement(
                $"La date de clôture ne peut pas être antérieure au début de la période " &
                $"({liaison.DateDebutValidite:dd/MM/yyyy}).", "Clôture impossible")
            Return
        End If

        If DialogChoix.Confirmer(
            $"Clôturer ce rattachement au {dateFin:dd/MM/yyyy} ?" & Environment.NewLine &
            "Les séances postérieures utiliseront le tarif de la période suivante.",
            "Clôture") <> DialogResult.Yes Then
            Return
        End If

        Try
            GestionLiaisonsPrixDomaines.CloturerLiaison(Contexte(), liaison.IdLiaison, dateFin)

            GestionLog.EcrireLog(
                $"Rattachement tarif / domaine {liaison.IdLiaison} clôturé au {dateFin:yyyy-MM-dd}.",
                GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)

            ChargerPour(_idParent)
            RaiseEvent RattachementsModifies(Me, EventArgs.Empty)

        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireException("Erreur lors de la clôture d'un rattachement.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            DialogChoix.Erreur("La clôture n'a pas pu être enregistrée.", "Clôture")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : BtnBasculer_Click
    ' Rôle      : Active ou désactive le rattachement sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub BtnBasculer_Click(sender As Object, e As EventArgs)

        Dim liaison As LiaisonPrixDomaine = LiaisonSelectionnee()
        If liaison Is Nothing Then Return

        Dim nouvelEtat As Boolean = Not liaison.Actif

        ' Réactiver peut recréer une ambiguïté de tarif conventionné sur la période.
        If nouvelEtat AndAlso
           GestionLiaisonsPrixDomaines.AutreTarifActifSurPeriode(liaison.IdPrixSeance,
                                                                 liaison.IdDomaine,
                                                                 liaison.DateDebutValidite,
                                                                 liaison.DateFinValidite,
                                                                 liaison.IdLiaison) Then

            DialogChoix.Avertissement(
                "Un autre tarif est déjà actif pour ce domaine sur cette période." & Environment.NewLine &
                "Clôturez-le d'abord, sinon le tarif conventionné deviendrait ambigu.",
                "Réactivation impossible")
            Return
        End If

        Try
            GestionLiaisonsPrixDomaines.DefinirActivationLiaison(Contexte(), liaison.IdLiaison, nouvelEtat)

            GestionLog.EcrireLog(
                $"Rattachement tarif / domaine {liaison.IdLiaison} : actif = {nouvelEtat}.",
                GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)

            ChargerPour(_idParent)
            RaiseEvent RattachementsModifies(Me, EventArgs.Empty)

        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireException("Erreur lors du changement d'état d'un rattachement.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            DialogChoix.Erreur("L'état du rattachement n'a pas pu être modifié.", "Rattachement")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : BtnSupprimer_Click
    ' Rôle      : Supprime un rattachement créé par erreur.
    ' Remarques : Avertit explicitement si des séances existent sur la période concernée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub BtnSupprimer_Click(sender As Object, e As EventArgs)

        Dim liaison As LiaisonPrixDomaine = LiaisonSelectionnee()
        If liaison Is Nothing Then Return

        If GestionLiaisonsPrixDomaines.PeriodeComporteSeances(liaison.IdDomaine,
                                                               liaison.DateDebutValidite,
                                                               liaison.DateFinValidite) Then

            If DialogChoix.Confirmer(
                "Des séances ont déjà été enregistrées dans ce domaine sur cette période." &
                Environment.NewLine & Environment.NewLine &
                "Supprimer ce rattachement effacerait la trace du tarif alors appliqué." &
                Environment.NewLine &
                "Préférez « Désactiver », qui conserve l'historique." & Environment.NewLine &
                Environment.NewLine &
                "Supprimer quand même ?",
                "Historique tarifaire concerné") <> DialogResult.Yes Then
                Return
            End If

        ElseIf DialogChoix.Confirmer(
            "Supprimer définitivement ce rattachement ?" & Environment.NewLine &
            "Cette action est irréversible.", "Suppression") <> DialogResult.Yes Then
            Return
        End If

        Try
            GestionLiaisonsPrixDomaines.SupprimerLiaison(Contexte(), liaison.IdLiaison)

            GestionLog.EcrireLog(
                $"Rattachement tarif / domaine {liaison.IdLiaison} supprimé.",
                GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)

            ChargerPour(_idParent)
            RaiseEvent RattachementsModifies(Me, EventArgs.Empty)

        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireException("Erreur lors de la suppression d'un rattachement.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            DialogChoix.Erreur("Le rattachement n'a pas pu être supprimé.", "Rattachement")
        End Try

    End Sub

    ' La sélection change : boutons et aide suivent ; une clôture en cours visait l'ancienne ligne.
    Private Sub Dgv_SelectionChanged(sender As Object, e As EventArgs)

        If _modeFormulaire = ModeFormulaire.Cloture Then AfficherFormulaire(ModeFormulaire.Ferme)
        ActualiserBoutons()

    End Sub

#End Region

End Class
