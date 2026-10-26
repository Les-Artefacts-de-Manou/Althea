' -------------------------------------------------------------------------------------------------
' UserControl : UC_LiaisonsNaturesDomaines
' Projet      : Althéa
' Version     : V2.2.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Panneau intégré d'administration des rattachements entre natures de séance et domaines
' (lia_natures_seance_domaines). Conçu pour être hébergé dans un onglet d'un écran parent,
' sans passer par une fenêtre modale (D-POR-5).
'
' Responsabilités :
' - Afficher les rattachements de l'élément parent (un domaine, ou une nature de séance)
' - Proposer à l'ajout uniquement les éléments pas encore rattachés (aucun doublon possible)
' - Choisir la catégorie de quota consommée par chaque nature dans le domaine (sens « par domaine »)
' - Retirer ou rétablir un rattachement, avec avertissement si le domaine se retrouverait
'   sans aucune nature proposée à la saisie
' - Supprimer un rattachement créé par erreur, sauf s'il a déjà servi à des séances
'
' Présentation (V2.0.0, UI-6 — maquette « Domaines — liaisons », ADR-34) :
' - Une carte du kit : barre locale en haut, grille sur toute la largeur, note d'aide, puis un
'   formulaire « Nouveau rattachement » au pied de la carte, ouvert par « Rattacher … ».
' - Sens « par domaine » : la catégorie de quota se choisit directement dans la grille (liste dans
'   la cellule de la ligne sélectionnée) et, à la création, dans le formulaire.
' - L'action « Créer une nouvelle … » de l'écran hôte se place à droite de la barre (TexteCreation,
'   événement CreationDemandee).
' - Remplace la V1 (boutons empilés, liste de catégorie séparée sous la grille, MessageBox).
' - V2.1.0 (18/10/2026, UI-9.4) : état en pastille (LiaisonNatureDomaine.Etat — « Nature désactivée »
'   en ocre : rattachée mais pas proposée), rattachement non proposé atténué.
' - V2.2.0 (22/10/2026, audit C.1, étape 3) : FournisseurContexte (ContexteOperation du parent)
'   transmis aux écritures, vérifiées par le métier.
' - V2.2.1 (25/10/2026, relevé de Joëlle au test NAV-C4-09) : l'action « Créer un nouveau … »
'   n'apparaissait pas : sa place était calculée d'après _btnCreer.Visible (False tant que l'écran
'   n'est pas affiché) et l'ancrage à droite la repoussait hors de la barre. Drapeau _avecCreation,
'   place recalculée à l'affichage, plus d'ancrage.
'
' Remarques   :
' - Aucun Designer : les contrôles sont créés par code (exception admise, Rules § 9), car le même
'   panneau sert dans deux écrans et deux sens de lecture.
' - Aucun accès direct à la base : tout passe par GestionLiaisonsNaturesDomaines.
' - La grille est alimentée par liaison de données ; la colonne de catégorie, non liée, est
'   remplie après chaque chargement (DataBindingComplete).
'
' Dépendances :
' - GestionLiaisonsNaturesDomaines, LiaisonNatureDomaine, GestionCategoriesQuota, CategorieQuota
' - Kit Althéa (AltheaCarte, AltheaBarreOutils, AltheaBouton, AltheaListe)
' - UtilsDataGrid, UtilsButtons, UITheme, UtilsPastilles, DialogChoix, GestionLog
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_LiaisonsNaturesDomaines
    Inherits UserControl

#Region "Énumération"

    ' -------------------------------------------------------------------------------------------------
    ' Énumération : SensLiaison
    ' Rôle        : Indique quel est l'élément parent et donc ce que l'utilisateur rattache.
    ' -------------------------------------------------------------------------------------------------
    Public Enum SensLiaison
        ' Parent = un domaine : on rattache des natures de séance
        ParDomaine = 0
        ' Parent = une nature de séance : on rattache des domaines
        ParNature = 1
    End Enum

#End Region

#Region "Événements"

    ''' <summary>L'utilisateur demande la création d'un nouvel élément (bouton TexteCreation).</summary>
    Public Event CreationDemandee As EventHandler

    ''' <summary>Un rattachement vient d'être créé, modifié ou supprimé (l'hôte peut recalculer
    ''' l'état de l'élément parent).</summary>
    Public Event RattachementsModifies As EventHandler

#End Region

#Region "Constantes"

    Private Const NomColonneCategorie As String = "colCategorie"

    ' Clé de l'entrée « Hors quota » dans les listes de catégories (aucune catégorie).
    Private Const CleHorsQuota As ULong = 0UL

#End Region

#Region "Variables privées"

    ' Carte et barre locale
    Private ReadOnly _carte As New AltheaCarte()
    Private ReadOnly _barre As New AltheaBarreOutils()
    Private ReadOnly _btnRattacher As New AltheaBouton()
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
    Private ReadOnly _lblCategorieAjout As New Label()
    Private ReadOnly _cboCategorieAjout As New AltheaListe()
    Private ReadOnly _btnValider As New AltheaBouton()
    Private ReadOnly _btnAnnulerFormulaire As New AltheaBouton()

    Private ReadOnly _toolTip As New ToolTip()

    ' Empêche l'écriture pendant le remplissage programmatique de la colonne de catégorie
    Private _chargementCategorie As Boolean = False

    ' Catégories proposées pour le domaine parent (« Hors quota » en tête)
    Private _choixCategories As New List(Of KeyValuePair(Of ULong, String))

    ' Identifiant de l'élément parent affiché (0 = aucune sélection)
    Private _idParent As ULong = 0

    ' Rattachements actuellement affichés
    Private _liaisons As New List(Of LiaisonNatureDomaine)

    ' Empêche toute écriture lorsque l'écran parent est en édition ou en consultation seule
    Private _lectureSeule As Boolean = False

    ' Sens de lecture courant (voir la propriété Mode)
    Private _mode As SensLiaison = SensLiaison.ParDomaine

#End Region

#Region "Propriétés"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : Mode
    ' Rôle      : Détermine si le parent est un domaine ou une nature de séance.
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
    ' Rôle      : Libellé de l'action « Créer une nouvelle … » à droite de la barre (vide = masquée).
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

    Private ReadOnly Property ParDomaine As Boolean
        Get
            Return _mode = SensLiaison.ParDomaine
        End Get
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

        PreparerBouton(_btnRattacher, "Rattacher une nature", "rattacher_normal", VarianteBouton.OutilPrincipal)
        PreparerBouton(_btnBasculer, "Retirer", "retirer_normal", VarianteBouton.Outil)
        PreparerBouton(_btnSupprimer, "Supprimer", "supprimer_normal", VarianteBouton.OutilDanger)
        PreparerBouton(_btnCreer, String.Empty, Nothing, VarianteBouton.Outil)
        _btnCreer.Visible = False

        _barre.Controls.AddRange(New Control() {_btnRattacher, _btnBasculer, _btnSupprimer, _btnCreer})

        ' Grille : seule la colonne de catégorie est modifiable (sens « par domaine »).
        _dgv.Dock = DockStyle.Fill
        _dgv.AutoGenerateColumns = False
        _dgv.MultiSelect = False
        _dgv.AllowUserToAddRows = False
        _dgv.AllowUserToDeleteRows = False
        _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        UtilsDataGrid.InitDataGridSecondaire(_dgv)
        _dgv.ReadOnly = False
        _dgv.EditMode = DataGridViewEditMode.EditOnEnter

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
        AddHandler _btnRattacher.Click, Sub() AfficherFormulaire(True)
        AddHandler _btnBasculer.Click, AddressOf BtnBasculer_Click
        AddHandler _btnSupprimer.Click, AddressOf BtnSupprimer_Click
        AddHandler _btnCreer.Click, Sub() RaiseEvent CreationDemandee(Me, EventArgs.Empty)
        AddHandler _btnValider.Click, AddressOf BtnValider_Click
        AddHandler _btnAnnulerFormulaire.Click, Sub() AfficherFormulaire(False)
        AddHandler _dgv.SelectionChanged, AddressOf Dgv_SelectionChanged
        AddHandler _dgv.DataBindingComplete, AddressOf Dgv_DataBindingComplete
        AddHandler _dgv.CurrentCellDirtyStateChanged, AddressOf Dgv_CurrentCellDirtyStateChanged
        AddHandler _dgv.CellValueChanged, AddressOf Dgv_CellValueChanged
        AddHandler _dgv.DataError, AddressOf Dgv_DataError
        AddHandler _dgv.CellFormatting, AddressOf Dgv_CellFormatting
        AddHandler _dgv.CellPainting, AddressOf Dgv_CellPainting

        InitialiserToolTips()
        AfficherFormulaire(False)

    End Sub

    Private Sub ConstruireFormulaire()

        _pied.Dock = DockStyle.Bottom
        _pied.AutoSize = True
        _pied.AutoSizeMode = AutoSizeMode.GrowAndShrink
        _pied.WrapContents = True
        _pied.BackColor = Color.Transparent
        _pied.Padding = New Padding(0, 6, 0, 0)

        PreparerLibelle(_lblFormulaire, "Nouveau rattachement")
        _lblFormulaire.ForeColor = UITheme.ColorMauveFonce

        _cboAjout.Size = New Size(260, 30)
        _cboAjout.Margin = New Padding(3, 3, 12, 3)

        PreparerLibelle(_lblCategorieAjout, "Catégorie de quota")
        _cboCategorieAjout.Size = New Size(200, 30)
        _cboCategorieAjout.Margin = New Padding(3, 3, 12, 3)

        PreparerBouton(_btnValider, "Rattacher", Nothing, VarianteBouton.OutilPrincipal)
        PreparerBouton(_btnAnnulerFormulaire, "Annuler", Nothing, VarianteBouton.Outil)
        _btnValider.Margin = New Padding(12, 3, 3, 3)

        _pied.Controls.AddRange(New Control() {
            _lblFormulaire, _cboAjout, _lblCategorieAjout, _cboCategorieAjout,
            _btnValider, _btnAnnulerFormulaire})

    End Sub

    Private Shared Sub PreparerLibelle(lbl As Label, texte As String)

        lbl.AutoSize = False
        lbl.Text = texte
        lbl.ForeColor = UITheme.ColorSaugeFonce
        lbl.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        lbl.TextAlign = ContentAlignment.MiddleLeft
        lbl.Height = 30
        lbl.Width = TextRenderer.MeasureText(texte, lbl.Font).Width + 6
        lbl.Margin = New Padding(3)

    End Sub

    ' Bouton léger du kit (variantes Outil) ; l'icône vient du Tag (…_normal, UtilsButtons).
    Private Shared Sub PreparerBouton(btn As AltheaBouton, texte As String, tagIcone As String,
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

    ' Aligne les actions après le titre de la barre ; Supprimer est séparé ; création calée à droite.
    Private Sub PlacerBoutonsBarre()

        Dim y As Integer = Math.Max(0, (_barre.Height - 30) \ 2)
        Dim x As Integer = _barre.MargeTitre

        For Each btn As AltheaBouton In {_btnRattacher, _btnBasculer, _btnSupprimer}
            If btn Is _btnSupprimer Then x += 10
            btn.Location = New Point(x, y)
            x += btn.Width + 4
        Next

        If _avecCreation Then
            AjusterLargeur(_btnCreer)
            _btnCreer.Location = New Point(Math.Max(x + 10, _barre.Width - _barre.Padding.Right - _btnCreer.Width), y)
        End If

    End Sub

    Private Sub AppliquerLibellesMode()

        If ParDomaine Then
            _barre.Titre = "Natures de séance proposées"
            _btnRattacher.Text = "Rattacher une nature"
        Else
            _barre.Titre = "Domaines où cette nature est proposée"
            _btnRattacher.Text = "Rattacher un domaine"
        End If

        _lblCategorieAjout.Visible = ParDomaine
        _cboCategorieAjout.Visible = ParDomaine

        AjusterLargeur(_btnRattacher)
        PlacerBoutonsBarre()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Rôle      : Explique le rôle de chaque action (le vocabulaire « Retirer » / « Supprimer »
    '             recouvre des effets très différents).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        _toolTip.AutoPopDelay = 15000
        _toolTip.InitialDelay = 400
        _toolTip.ReshowDelay = 100

        _toolTip.SetToolTip(_btnRattacher,
            "Ouvre, au bas de la carte, le choix de l'élément à proposer à la saisie des séances.")

        _toolTip.SetToolTip(_btnSupprimer,
            "Supprime définitivement le rattachement." & Environment.NewLine &
            "Possible uniquement si aucune séance ne l'a encore utilisé." & Environment.NewLine &
            "Pour simplement le masquer à la saisie sans perdre l'historique, utilisez plutôt « Retirer ».")

        _toolTip.SetToolTip(_cboCategorieAjout,
            "Catégorie de quota consommée par les séances de cette nature dans ce domaine." & Environment.NewLine &
            "« Hors quota » : les séances ne sont pas décomptées.")

        ' _btnBasculer bascule entre « Retirer » et « Rétablir » : infobulle ajustée dynamiquement.

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConstruireColonnes
    ' Rôle      : Définit les colonnes de la grille selon le sens de lecture.
    ' Remarques : Sens « par domaine » : colonne de catégorie modifiable (liste) ; sens « par
    '             nature » : simple texte, les catégories dépendant de chaque domaine.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConstruireColonnes()

        _dgv.Columns.Clear()

        Dim libelleCible As String = If(ParDomaine, "Nature de séance", "Domaine")
        Dim propCode As String = If(ParDomaine,
                                    NameOf(LiaisonNatureDomaine.CodeNatureSeance),
                                    NameOf(LiaisonNatureDomaine.CodeDomaine))
        Dim propLibelle As String = If(ParDomaine,
                                       NameOf(LiaisonNatureDomaine.LibelleNatureSeance),
                                       NameOf(LiaisonNatureDomaine.LibelleDomaine))

        _dgv.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colCode", .HeaderText = "Code", .DataPropertyName = propCode,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 80, .ReadOnly = True})

        _dgv.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colLibelle", .HeaderText = libelleCible, .DataPropertyName = propLibelle,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 160, .ReadOnly = True})

        If ParDomaine Then
            _dgv.Columns.Add(New DataGridViewComboBoxColumn With {
                .Name = NomColonneCategorie, .HeaderText = "Catégorie de quota",
                .ValueType = GetType(ULong), .ValueMember = "Key", .DisplayMember = "Value",
                .DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                .DisplayStyleForCurrentCellOnly = True, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 120, .ReadOnly = True})
        Else
            _dgv.Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "colQuota", .HeaderText = "Quota",
                .DataPropertyName = NameOf(LiaisonNatureDomaine.LibellePrestation),
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 120, .ReadOnly = True})
        End If

        _dgv.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colEtat", .HeaderText = "État",
            .DataPropertyName = NameOf(LiaisonNatureDomaine.LibelleEtat),
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 110, .ReadOnly = True})

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédures : Dgv_CellFormatting / Dgv_CellPainting
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    ' Rôle       : Rattachement non proposé à la saisie atténué ; état en pastille.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Dgv_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 Then Return
        Dim liaison As LiaisonNatureDomaine = TryCast(_dgv.Rows(e.RowIndex).DataBoundItem, LiaisonNatureDomaine)
        If liaison IsNot Nothing AndAlso Not liaison.EstProposeASaisie Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If
    End Sub

    Private Sub Dgv_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse _dgv.Columns(e.ColumnIndex).Name <> "colEtat" Then Return
        Dim liaison As LiaisonNatureDomaine = TryCast(_dgv.Rows(e.RowIndex).DataBoundItem, LiaisonNatureDomaine)
        If liaison Is Nothing Then Return
        UtilsDataGrid.DessinerPastilleCellule(_dgv, e, liaison.LibelleEtat, UtilsPastilles.Teinte(liaison.Etat))
    End Sub

#End Region

#Region "Chargement"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChargerPour
    ' Rôle       : Charge les rattachements de l'élément parent sélectionné.
    ' Paramètres : idParent - identifiant du domaine ou de la nature ; 0 pour vider le panneau
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerPour(idParent As ULong)

        _idParent = idParent
        AfficherFormulaire(False)

        If _dgv.Columns.Count = 0 Then ConstruireColonnes()

        If _idParent = 0 Then
            _liaisons = New List(Of LiaisonNatureDomaine)
            _choixCategories = New List(Of KeyValuePair(Of ULong, String))
            _dgv.DataSource = Nothing
            _cboAjout.DataSource = Nothing
            ActualiserBoutons()
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            _liaisons = If(ParDomaine,
                           GestionLiaisonsNaturesDomaines.GetLiaisonsParDomaine(_idParent),
                           GestionLiaisonsNaturesDomaines.GetLiaisonsParNature(_idParent))

            If ParDomaine Then ChargerCategories()

            _dgv.DataSource = Nothing
            _dgv.DataSource = _liaisons
            _dgv.ClearSelection()

            ChargerListeAjout()
            ActualiserBoutons()

        Catch ex As Exception
            GestionLog.EcrireException("Erreur de chargement des rattachements nature / domaine.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            DialogChoix.Avertissement("Impossible de charger les rattachements." & Environment.NewLine &
                                      "Consultez le journal pour le détail.", "Rattachements")
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerCategories
    ' Rôle      : « Hors quota » + toutes les catégories du domaine parent (les inactives sont
    '             marquées : elles restent affichables sur les lignes qui les portent encore).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerCategories()

        _choixCategories = New List(Of KeyValuePair(Of ULong, String)) From {
            New KeyValuePair(Of ULong, String)(CleHorsQuota, "Hors quota")
        }

        For Each c As CategorieQuota In GestionCategoriesQuota.GetCategoriesParDomaine(_idParent, True)
            _choixCategories.Add(New KeyValuePair(Of ULong, String)(c.IdCategorieQuota,
                                 If(c.Actif, c.LibelleCategorie, c.LibelleCategorie & " (inactive)")))
        Next

        Dim colonne As DataGridViewComboBoxColumn = ColonneCategorie()
        If colonne IsNot Nothing Then colonne.DataSource = _choixCategories

        ' Formulaire : seules les catégories actives sont proposées pour un nouveau rattachement.
        Dim actives As List(Of KeyValuePair(Of ULong, String)) =
            _choixCategories.Where(Function(kv) Not kv.Value.EndsWith(" (inactive)")).ToList()
        _cboCategorieAjout.DataSource = Nothing
        _cboCategorieAjout.DisplayMember = "Value"
        _cboCategorieAjout.ValueMember = "Key"
        _cboCategorieAjout.DataSource = actives

    End Sub

    Private Function ColonneCategorie() As DataGridViewComboBoxColumn

        If Not _dgv.Columns.Contains(NomColonneCategorie) Then Return Nothing
        Return TryCast(_dgv.Columns(NomColonneCategorie), DataGridViewComboBoxColumn)

    End Function

    ' Remplit la colonne de catégorie (non liée) d'après chaque rattachement.
    Private Sub Dgv_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs)

        Dim colonne As DataGridViewComboBoxColumn = ColonneCategorie()
        If colonne Is Nothing Then Return

        _chargementCategorie = True
        Try
            For Each row As DataGridViewRow In _dgv.Rows
                Dim l As LiaisonNatureDomaine = TryCast(row.DataBoundItem, LiaisonNatureDomaine)
                If l IsNot Nothing Then row.Cells(colonne.Index).Value = If(l.IdCategorieQuota, CleHorsQuota)
            Next
        Finally
            _chargementCategorie = False
        End Try

    End Sub

    ' Alimente la liste du formulaire avec les seuls éléments encore rattachables.
    Private Sub ChargerListeAjout()

        _cboAjout.DataSource = Nothing

        If ParDomaine Then
            _cboAjout.DisplayMember = NameOf(NatureSeance.LibelleNatureSeance)
            _cboAjout.ValueMember = NameOf(NatureSeance.IdNatureSeance)
            _cboAjout.DataSource = GestionLiaisonsNaturesDomaines.GetNaturesDisponiblesPourDomaine(_idParent)
        Else
            _cboAjout.DisplayMember = NameOf(Domaine.LibelleDomaine)
            _cboAjout.ValueMember = NameOf(Domaine.IdDomaine)
            _cboAjout.DataSource = GestionLiaisonsNaturesDomaines.GetDomainesDisponiblesPourNature(_idParent)
        End If

        _cboAjout.SelectedIndex = -1

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirLectureSeule
    ' Rôle       : Neutralise les actions d'écriture du panneau (édition du parent, droits).
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirLectureSeule(lectureSeule As Boolean)

        _lectureSeule = lectureSeule
        If lectureSeule Then AfficherFormulaire(False)
        ActualiserBoutons()

    End Sub

    Private Function LiaisonSelectionnee() As LiaisonNatureDomaine

        If _dgv.CurrentRow Is Nothing OrElse Not _dgv.CurrentRow.Selected Then Return Nothing
        Return TryCast(_dgv.CurrentRow.DataBoundItem, LiaisonNatureDomaine)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActualiserBoutons
    ' Rôle      : Ajuste l'état des boutons, de la colonne de catégorie et de l'aide.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActualiserBoutons()

        Dim modifiable As Boolean = Not _lectureSeule AndAlso _idParent > 0
        Dim liaison As LiaisonNatureDomaine = LiaisonSelectionnee()

        _btnRattacher.Enabled = modifiable AndAlso _cboAjout.Items.Count > 0
        _btnBasculer.Enabled = modifiable AndAlso liaison IsNot Nothing
        _btnSupprimer.Enabled = modifiable AndAlso liaison IsNot Nothing
        _btnCreer.Enabled = Not _lectureSeule

        ' Sans catégorie définie pour le domaine, la liste n'offre que « Hors quota » : non modifiable.
        Dim colonne As DataGridViewComboBoxColumn = ColonneCategorie()
        If colonne IsNot Nothing Then colonne.ReadOnly = Not modifiable OrElse _choixCategories.Count <= 1

        Dim texteBascule As String = If(liaison IsNot Nothing AndAlso Not liaison.Actif, "Rétablir", "Retirer")
        If _btnBasculer.Text <> texteBascule Then
            _btnBasculer.Text = texteBascule
            AjusterLargeur(_btnBasculer)
            PlacerBoutonsBarre()
        End If

        If liaison IsNot Nothing AndAlso Not liaison.Actif Then
            _toolTip.SetToolTip(_btnBasculer,
                "Rétablit ce rattachement : l'élément est de nouveau proposé à la saisie des séances.")
        Else
            _toolTip.SetToolTip(_btnBasculer,
                "Retire ce rattachement de la liste proposée à la saisie." & Environment.NewLine &
                "Les séances déjà enregistrées ne sont pas modifiées ; le rattachement peut être rétabli à tout moment.")
        End If

        _lblAide.Text = ConstruireTexteAide(liaison)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConstruireTexteAide
    ' Rôle     : Explique en clair l'effet des actions possibles sur la ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Function ConstruireTexteAide(liaison As LiaisonNatureDomaine) As String

        If _idParent = 0 Then
            Return "Sélectionnez un élément dans la liste pour voir ses rattachements."
        End If

        If liaison Is Nothing Then
            If Not ParDomaine Then
                Return "Les domaines rattachés ici sont ceux dans lesquels cette nature de séance pourra être choisie."
            End If
            If _choixCategories.Count <= 1 Then
                Return "Les natures rattachées ici sont proposées à la saisie des séances du domaine. " &
                       "Ce domaine n'a pas de catégorie de quota (étape 3) : toutes sont hors quota."
            End If
            Return "Les natures rattachées ici sont proposées à la saisie des séances du domaine. " &
                   "La catégorie de quota se choisit directement dans la grille."
        End If

        If Not liaison.Actif Then
            Return "Ce rattachement est retiré : l'élément n'est plus proposé à la saisie. " &
                   "« Rétablir » le remet dans la liste."
        End If

        If Not liaison.ActifNature Then
            Return "La nature de séance est désactivée dans son référentiel : elle ne sera pas " &
                   "proposée, même si le rattachement est actif."
        End If

        Return "« Retirer » masque l'élément à la saisie sans toucher aux séances déjà " &
               "enregistrées. « Supprimer » n'est possible que si aucune séance ne l'a utilisé."

    End Function

#End Region

#Region "Catégorie de quota dans la grille"

    ' La liste de la cellule valide son choix dès la sélection (sans attendre de quitter la cellule).
    Private Sub Dgv_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)

        Dim colonne As DataGridViewComboBoxColumn = ColonneCategorie()
        If colonne Is Nothing OrElse _dgv.CurrentCell Is Nothing Then Return

        If _dgv.CurrentCell.ColumnIndex = colonne.Index AndAlso _dgv.IsCurrentCellDirty Then
            _dgv.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    Private Sub Dgv_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)

        If _chargementCategorie OrElse e.RowIndex < 0 Then Return

        Dim colonne As DataGridViewComboBoxColumn = ColonneCategorie()
        If colonne Is Nothing OrElse e.ColumnIndex <> colonne.Index Then Return

        Dim row As DataGridViewRow = _dgv.Rows(e.RowIndex)
        Dim liaison As LiaisonNatureDomaine = TryCast(row.DataBoundItem, LiaisonNatureDomaine)
        If liaison Is Nothing Then Return

        Dim cle As ULong = Convert.ToUInt64(If(row.Cells(colonne.Index).Value, CleHorsQuota))

        ' Le rechargement de la grille ne peut pas se faire pendant l'événement de cellule :
        ' il est différé à la fin du traitement en cours.
        BeginInvoke(Sub() AppliquerCategorie(liaison, cle))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerCategorie
    ' Rôle      : Enregistre la catégorie choisie pour un rattachement, puis recharge la grille.
    ' Remarques : - Le changement modifie le décompte des dossiers déjà ouverts : confirmation si
    '               des séances existent.
    '             - Une catégorie inactive ne peut pas être choisie (elle reste seulement affichée).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerCategorie(liaison As LiaisonNatureDomaine, cle As ULong)

        Dim idParentCourant As ULong = _idParent
        Dim nouvelle As ULong? = If(cle = CleHorsQuota, CType(Nothing, ULong?), cle)

        If Nullable.Equals(nouvelle, liaison.IdCategorieQuota) Then Return

        Try
            Dim choix As KeyValuePair(Of ULong, String) = _choixCategories.FirstOrDefault(Function(kv) kv.Key = cle)
            If choix.Value IsNot Nothing AndAlso choix.Value.EndsWith(" (inactive)") Then
                DialogChoix.Avertissement("Cette catégorie est désactivée : choisissez une catégorie active ou « Hors quota ».",
                                          "Catégorie de quota")
                ChargerPour(idParentCourant)
                SelectionnerLiaison(liaison.IdLiaison)
                Return
            End If

            If GestionLiaisonsNaturesDomaines.LiaisonUtiliseeParSeances(liaison.IdNatureSeance, liaison.IdDomaine) AndAlso
               DialogChoix.Confirmer("Des séances existent déjà avec cette nature dans ce domaine." & Environment.NewLine &
                                     "Changer la catégorie modifiera le décompte de quota de ces dossiers, y compris pour le passé." &
                                     Environment.NewLine & Environment.NewLine & "Confirmer le changement ?",
                                     "Catégorie de quota") <> DialogResult.Yes Then
                ChargerPour(idParentCourant)
                SelectionnerLiaison(liaison.IdLiaison)
                Return
            End If

            GestionLiaisonsNaturesDomaines.DefinirCategorieQuota(Contexte(), liaison.IdLiaison, nouvelle)

            GestionLog.EcrireLog(
                $"Rattachement nature / domaine {liaison.IdLiaison} : catégorie de quota = {If(nouvelle.HasValue, nouvelle.Value.ToString(), "aucune")}.",
                GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)

            RaiseEvent RattachementsModifies(Me, EventArgs.Empty)

        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireException("Erreur lors du changement de catégorie de quota.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            DialogChoix.Erreur("La catégorie de quota n'a pas pu être enregistrée.", "Catégorie de quota")
        End Try

        ChargerPour(idParentCourant)
        SelectionnerLiaison(liaison.IdLiaison)

    End Sub

    ' Une valeur absente de la liste (catégorie supprimée entre-temps…) ne doit pas faire planter
    ' la grille : l'erreur est journalisée, la cellule reste affichée telle quelle.
    Private Sub Dgv_DataError(sender As Object, e As DataGridViewDataErrorEventArgs)

        e.ThrowException = False
        GestionLog.EcrireException($"UC_LiaisonsNaturesDomaines : valeur de cellule invalide (ligne {e.RowIndex}, colonne {e.ColumnIndex}).",
                                   e.Exception, GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)

    End Sub

    Private Sub SelectionnerLiaison(idLiaison As ULong)

        For Each row As DataGridViewRow In _dgv.Rows
            Dim l As LiaisonNatureDomaine = TryCast(row.DataBoundItem, LiaisonNatureDomaine)
            If l IsNot Nothing AndAlso l.IdLiaison = idLiaison Then
                row.Selected = True
                _dgv.CurrentCell = row.Cells(0)
                Exit For
            End If
        Next

    End Sub

#End Region

#Region "Formulaire au pied de la carte"

    Private Sub AfficherFormulaire(ouvrir As Boolean)

        _pied.SuspendLayout()

        If ouvrir Then
            _cboAjout.SelectedIndex = -1
            If _cboCategorieAjout.Items.Count > 0 Then _cboCategorieAjout.SelectedIndex = 0
            Dim avecCategories As Boolean = ParDomaine AndAlso _cboCategorieAjout.Items.Count > 1
            _lblCategorieAjout.Visible = avecCategories
            _cboCategorieAjout.Visible = avecCategories
        End If

        _pied.Visible = ouvrir
        _pied.ResumeLayout(True)

        If ouvrir Then _cboAjout.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : BtnValider_Click
    ' Rôle      : Crée le rattachement choisi dans le formulaire (avec sa catégorie de quota).
    ' -------------------------------------------------------------------------------------------------
    Private Sub BtnValider_Click(sender As Object, e As EventArgs)

        If _cboAjout.SelectedValue Is Nothing Then
            DialogChoix.Information("Choisissez d'abord l'élément à rattacher dans la liste.", "Rattachement")
            Return
        End If

        Dim idCible As ULong = Convert.ToUInt64(_cboAjout.SelectedValue)

        Dim categorie As ULong? = Nothing
        If ParDomaine AndAlso _cboCategorieAjout.Visible AndAlso _cboCategorieAjout.SelectedValue IsNot Nothing Then
            Dim cle As ULong = Convert.ToUInt64(_cboCategorieAjout.SelectedValue)
            If cle <> CleHorsQuota Then categorie = cle
        End If

        Dim liaison As New LiaisonNatureDomaine With {
            .IdNatureSeance = If(ParDomaine, idCible, _idParent),
            .IdDomaine = If(ParDomaine, _idParent, idCible),
            .IdCategorieQuota = categorie,
            .Actif = True
        }

        Dim erreur As String = GestionLiaisonsNaturesDomaines.ValiderLiaison(liaison)

        If Not String.IsNullOrEmpty(erreur) Then
            DialogChoix.Avertissement(erreur, "Rattachement impossible")
            Return
        End If

        Try
            GestionLiaisonsNaturesDomaines.InsertLiaison(Contexte(), liaison)

            GestionLog.EcrireLog(
                $"Rattachement nature {liaison.IdNatureSeance} / domaine {liaison.IdDomaine} créé " &
                $"(catégorie de quota : {If(categorie.HasValue, categorie.Value.ToString(), "aucune")}).",
                GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)

            ChargerPour(_idParent)
            RaiseEvent RattachementsModifies(Me, EventArgs.Empty)

        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireException("Erreur lors de la création d'un rattachement nature / domaine.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            DialogChoix.Erreur("Le rattachement n'a pas pu être enregistré.", "Rattachement")
        End Try

    End Sub

#End Region

#Region "Actions"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : BtnBasculer_Click
    ' Rôle      : Retire ou rétablit le rattachement sélectionné.
    ' Remarques : Avertit si le retrait priverait le domaine de toute nature proposée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub BtnBasculer_Click(sender As Object, e As EventArgs)

        Dim liaison As LiaisonNatureDomaine = LiaisonSelectionnee()
        If liaison Is Nothing Then Return

        Dim nouvelEtat As Boolean = Not liaison.Actif

        If Not nouvelEtat Then
            Dim avertissement As String = GestionLiaisonsNaturesDomaines.AvertissementRetraitLiaison(liaison)
            If Not String.IsNullOrEmpty(avertissement) AndAlso
               DialogChoix.Confirmer(avertissement, "Retrait du rattachement") <> DialogResult.Yes Then
                Return
            End If
        End If

        Try
            GestionLiaisonsNaturesDomaines.DefinirActivationLiaison(Contexte(), liaison.IdLiaison, nouvelEtat)

            GestionLog.EcrireLog(
                $"Rattachement nature / domaine {liaison.IdLiaison} : actif = {nouvelEtat}.",
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
    ' Rôle      : Supprime définitivement un rattachement créé par erreur.
    ' Remarques : Refuse la suppression dès qu'une séance a utilisé le couple nature / domaine.
    ' -------------------------------------------------------------------------------------------------
    Private Sub BtnSupprimer_Click(sender As Object, e As EventArgs)

        Dim liaison As LiaisonNatureDomaine = LiaisonSelectionnee()
        If liaison Is Nothing Then Return

        If GestionLiaisonsNaturesDomaines.LiaisonUtiliseeParSeances(liaison.IdNatureSeance, liaison.IdDomaine) Then
            DialogChoix.Information(
                $"Des séances ont déjà été enregistrées avec la nature « {liaison.LibelleNatureSeance} » " &
                $"dans le domaine « {liaison.LibelleDomaine} »." & Environment.NewLine & Environment.NewLine &
                "La suppression est impossible afin de préserver l'historique." & Environment.NewLine &
                "Utilisez « Retirer » pour ne plus la proposer à la saisie.",
                "Suppression impossible")
            Return
        End If

        If DialogChoix.Confirmer(
            "Supprimer définitivement ce rattachement ?" & Environment.NewLine &
            "Cette action est irréversible.", "Suppression") <> DialogResult.Yes Then
            Return
        End If

        Try
            GestionLiaisonsNaturesDomaines.SupprimerLiaison(Contexte(), liaison.IdLiaison)

            GestionLog.EcrireLog(
                $"Rattachement nature / domaine {liaison.IdLiaison} supprimé.",
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

    Private Sub Dgv_SelectionChanged(sender As Object, e As EventArgs)
        ActualiserBoutons()
    End Sub

#End Region

End Class
