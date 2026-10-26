' -------------------------------------------------------------------------------------------------
' Module      : UtilsDataGrid
' Projet      : Althéa
' Version     : V1.12.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les comportements UI communs aux DataGridView.
'
' Responsabilités :
' - Appliquer le style basique des DataGridView (InitDataGridBasique)
' - Configurer les couleurs depuis UITheme
' - Configurer les polices et alignements
' - Configurer le mode sélection (FullRowSelect, ReadOnly, etc.)
' - Tri par en-tête (ActiverTriColonnes) ; état par défaut (TriUtilisateurActif, ReinitialiserTri)
' - Dessin dans les cellules : pastilles d'état et séparateur titré (V1.5.0, 17/10/2026) ; pastille
'   « au trait » (contour sans fond, V1.6.0) pour un état à signaler sans le confondre avec un statut
' - Socle commun UI-9 (V1.7.0, 18/10/2026) : double tampon (défilement sans scintillement), polices
'   partagées, alignement des colonnes (AlignerCentre / AlignerDroite), fin contour sur toute
'   pastille pleine
' - V1.8.0 (18/10/2026, UI-9.2) : repère titré sur une ligne réservée (DessinerRepereSurLigne), trait
'   plein ou pointillé — créneaux libres du planning, titres de section
' - V1.9.0 (18/10/2026, UI-9.3) : repère sur une ligne sélectionnable (en-tête de session des
'   journaux) peint dans la couleur de sélection ; DesactiverTri pour les grilles à repères
' - V1.10.0 (21/10/2026) : DessinerPastilleZone (pastille hors grille : éléments des listes de filtre)
'   et RetraitTextePastille (alignement du texte simple sur celui des pastilles).
' - V1.9.1 (19/10/2026) : DessinerPastilleCellule rend l'anticrénelage à la grille (relevé par
'   Joëlle : traits de grille gris et épais sous toute ligne portant une pastille).
' - V1.11.0 (22/10/2026) : ElementSelectionne (ligne choisie par l'utilisatrice, jamais la ligne
'   courante posée d'office au chargement ; décision de Joëlle).
' - V1.12.0 (24/10/2026) : DessinerPastilleAvantTexte (pastille puis texte de la cellule : patient
'   inactif de la grille Patients).
'
' Remarques   :
' - Les couleurs viennent de UITheme
' - Module statique (méthodes partagées)
'
' Dépendances :
' - UITheme (couleurs)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On
Option Infer On

Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

Public Module UtilsDataGrid

    #Region "Mise en forme des DataGridView"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitDataGridBasique
    ' Version    : V1.5.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Applique le style basique Althéa à un DataGridView.
    '
    ' Paramètres :
    ' - dgv : Le DataGridView à styler
    '
    ' Remarques  :
    ' - Configure les couleurs (fond, grille, en-têtes, cellules, sélection, alternance) depuis UITheme
    ' - Configure les polices (Calibri 10pt, Bold pour en-têtes, Regular pour cellules)
    ' - Configure les hauteurs (en-têtes : 32px)
    ' - Configure le mode sélection (FullRowSelect, ReadOnly, MultiSelect=False, etc.)
    ' - Masque les RowHeaders
    ' - V1.5.0 (UI-9) : polices partagées (elles étaient recréées à chaque grille, jamais libérées) ;
    '   double tampon activé (ActiverDoubleTampon)
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitDataGridBasique(
        dgv As DataGridView
    )

        If dgv Is Nothing Then Return

        dgv.BorderStyle = BorderStyle.None
        dgv.BackgroundColor = UITheme.DataGridBack
        dgv.GridColor = UITheme.DataGridGridColor

        dgv.EnableHeadersVisualStyles = False

        dgv.ColumnHeadersDefaultCellStyle.BackColor = UITheme.DataGridHeaderBack
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.DataGridHeaderFore
        dgv.ColumnHeadersDefaultCellStyle.Font = PoliceEntetes
        dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgv.ColumnHeadersHeight = 32

        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = UITheme.DataGridHeaderBack
        dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = UITheme.DataGridHeaderFore

        dgv.DefaultCellStyle.BackColor = UITheme.DataGridCellBack
        dgv.DefaultCellStyle.ForeColor = UITheme.DataGridCellFore
        dgv.DefaultCellStyle.SelectionBackColor = UITheme.DataGridCellSelectionBack
        dgv.DefaultCellStyle.SelectionForeColor = UITheme.DataGridCellSelectionFore
        dgv.DefaultCellStyle.Font = PoliceCellules

        dgv.RowsDefaultCellStyle.BackColor = UITheme.DataGridCellBack
        dgv.RowsDefaultCellStyle.ForeColor = UITheme.DataGridCellFore
        dgv.AlternatingRowsDefaultCellStyle.BackColor = UITheme.DataGridAlternatingBack

        dgv.RowHeadersVisible = False
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.MultiSelect = False
        dgv.ReadOnly = True
        dgv.AutoGenerateColumns = False
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False

        ActiverDoubleTampon(dgv)
        End Sub

    #End Region

    #Region "Tri des données"

        ' -------------------------------------------------------------------------------------------------
        ' Procédure  : InitDataGridSecondaire
        ' Version    : V1.1.0
        ' Date       : 18/10/2026
        '
        ' Rôle       :
        ' Applique le style « grille secondaire » à un DataGridView présentant des données RATTACHÉES
        ' à la ligne sélectionnée dans une grille principale.
        '
        ' Paramètres :
        ' - dgv : Le DataGridView à styler
        '
        ' Remarques  :
        ' - Part du style basique puis le dénature volontairement : en-tête plus sombre, fond beige
        '   plutôt que blanc, police légèrement réduite et lignes moins hautes.
        ' - Objectif d'ergonomie : l'utilisateur doit voir au premier coup d'oeil que cette grille
        '   n'est PAS celle que pilotent les boutons Nouveau / Modifier / Désactiver du bas d'écran.
        '   Sans ce contraste, on croit pouvoir éditer la grille secondaire et on agit par erreur sur
        '   l'élément sélectionné dans la grille principale.
        ' - La couleur de sélection reste marquée afin que la ligne active demeure repérable.
        ' - V1.1.0 (UI-9) : polices partagées.
        ' -------------------------------------------------------------------------------------------------
        Public Sub InitDataGridSecondaire(
            dgv As DataGridView
        )

            If dgv Is Nothing Then Return

            InitDataGridBasique(dgv)

            ' En-tête plus sombre et texte clair : rupture visuelle nette avec la grille principale.
            dgv.ColumnHeadersDefaultCellStyle.BackColor = UITheme.DataGridSecondaireHeaderBack
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.DataGridSecondaireHeaderFore
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = UITheme.DataGridSecondaireHeaderBack
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = UITheme.DataGridSecondaireHeaderFore
            dgv.ColumnHeadersDefaultCellStyle.Font = PoliceEntetesSecondaire
            dgv.ColumnHeadersHeight = 28

            ' Cellules sur fond beige : la grille paraît « posée » sur le panneau, non éditable.
            dgv.BackgroundColor = UITheme.DataGridSecondaireBack
            dgv.DefaultCellStyle.BackColor = UITheme.DataGridSecondaireCellBack
            dgv.DefaultCellStyle.Font = PoliceCellulesSecondaire
            dgv.RowsDefaultCellStyle.BackColor = UITheme.DataGridSecondaireCellBack
            dgv.AlternatingRowsDefaultCellStyle.BackColor = UITheme.DataGridSecondaireAlternatingBack

            dgv.RowTemplate.Height = 22

        End Sub

        ' -------------------------------------------------------------------------------------------------
        ' Procédure  : ActiverEntetesMultiLignes
        ' Version    : V1.0.0
        ' Date       : 20/09/2026
        '
        ' Rôle       :
        ' Autorise les en-têtes de colonnes à s'afficher sur plusieurs lignes, et réduit éventuellement
        ' la police des cellules pour densifier la grille.
        '
        ' Paramètres :
        ' - dgv           : Le DataGridView à adapter
        ' - hauteurEntete : Hauteur en pixels réservée aux en-têtes (deux lignes ≈ 46)
        ' - taillePolice  : Taille de police des cellules ; 0 pour ne pas y toucher
        '
        ' Remarques  :
        ' - Sans retour à la ligne, la largeur minimale d'une colonne est imposée par la longueur de
        '   son titre : une colonne ne contenant qu'un chiffre reste large si son en-tête l'est.
        '   Répartir le titre sur deux lignes libère cette contrainte et permet de resserrer la grille
        '   afin d'embrasser davantage d'informations sans ascenseur horizontal.
        ' - La hauteur est fixée explicitement car le mode AutoSize ignore le WrapMode tant qu'aucune
        '   colonne ne déborde réellement.
        ' -------------------------------------------------------------------------------------------------
        Public Sub ActiverEntetesMultiLignes(
            dgv As DataGridView,
            hauteurEntete As Integer,
            taillePolice As Single
        )

            If dgv Is Nothing Then Return

            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            dgv.ColumnHeadersHeight = hauteurEntete

            If taillePolice > 0 Then

                dgv.DefaultCellStyle.Font = New Font("Calibri", taillePolice, FontStyle.Regular)
                dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Calibri", taillePolice, FontStyle.Bold)

            End If

        End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Champ interne : _etatsTri
    ' Rôle : Mémorise l'état de tri (colonne + sens) pour chaque DataGridView activé via ActiverTriColonnes.
    ' -------------------------------------------------------------------------------------------------
    Private ReadOnly _etatsTri As New Dictionary(Of DataGridView, EtatTri)

    ' État de tri d'une grille : index de colonne triée et sens (ascendant/descendant).
    Private NotInheritable Class EtatTri
        Public Property ColonneIndex As Integer = -1
        Public Property Ascendant As Boolean = True
        ' Grille paginée : liste complète à trier et rafraîchissement de l'affichage (sinon Nothing).
        Public Property FournisseurSource As Func(Of IList)
        Public Property Rafraichir As Action
    End Class

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ActiverTriColonnes
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Active le tri par clic sur l'en-tête de colonne pour un DataGridView lié à une List(Of T).
    ' Chaque clic bascule entre tri ascendant et descendant sur la colonne cliquée, et affiche le
    ' glyphe de tri correspondant dans l'en-tête.
    '
    ' Paramètres :
    ' - dgv : Le DataGridView à rendre triable
    '
    ' Remarques  :
    ' - Le tri s'appuie sur DataPropertyName de la colonne (réflexion sur les éléments de la liste).
    ' - Les colonnes non liées (DataPropertyName vide, ex: colonnes d'icônes) ne sont pas triables.
    ' - Idempotent : un même DataGridView n'est branché qu'une seule fois.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ActiverTriColonnes(dgv As DataGridView)

        If dgv Is Nothing Then Return
        If _etatsTri.ContainsKey(dgv) Then Return

        _etatsTri(dgv) = New EtatTri()
        AddHandler dgv.ColumnHeaderMouseClick, AddressOf DataGrid_ColumnHeaderMouseClick

        ' Tri géré par code : colonnes liées en Programmatic (glyphe manuel), non liées non triables.
        For Each col As DataGridViewColumn In dgv.Columns
            If String.IsNullOrWhiteSpace(col.DataPropertyName) Then
                col.SortMode = DataGridViewColumnSortMode.NotSortable
            Else
                col.SortMode = DataGridViewColumnSortMode.Programmatic
            End If
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirSourceTri
    ' Version    : V1.0.0
    ' Date       : 22/07/2026
    '
    ' Rôle       :
    ' Pour une grille paginée : le tri porte sur la liste complète fournie, puis l'affichage est
    ' délégué au rafraîchissement indiqué (et non à la seule page liée à la grille).
    '
    ' Paramètres :
    ' - dgv         : Grille déjà activée via ActiverTriColonnes
    ' - fournisseur : Renvoie la liste complète à trier
    ' - rafraichir  : Réaffiche la grille après tri
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirSourceTri(dgv As DataGridView, fournisseur As Func(Of IList), rafraichir As Action)

        If dgv Is Nothing Then Return
        If Not _etatsTri.ContainsKey(dgv) Then ActiverTriColonnes(dgv)

        _etatsTri(dgv).FournisseurSource = fournisseur
        _etatsTri(dgv).Rafraichir = rafraichir

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DataGrid_ColumnHeaderMouseClick
    ' Rôle       : Gère le clic sur un en-tête et trie la liste liée (bascule asc/desc + glyphe).
    ' -------------------------------------------------------------------------------------------------
    Private Sub DataGrid_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs)

        Dim dgv As DataGridView = TryCast(sender, DataGridView)
        If dgv Is Nothing OrElse e.ColumnIndex < 0 Then Return

        Dim colonne As DataGridViewColumn = dgv.Columns(e.ColumnIndex)
        Dim propriete As String = colonne.DataPropertyName
        If String.IsNullOrWhiteSpace(propriete) Then Return

        Dim etat As EtatTri = _etatsTri(dgv)

        Dim source As IList =
            If(etat.FournisseurSource IsNot Nothing, etat.FournisseurSource.Invoke(), CType(TryCast(dgv.DataSource, IList), IList))
        If source Is Nothing OrElse source.Count = 0 Then Return

        ' Bascule du sens : même colonne -> inverse, nouvelle colonne -> ascendant.
        If etat.ColonneIndex = e.ColumnIndex Then
            etat.Ascendant = Not etat.Ascendant
        Else
            etat.ColonneIndex = e.ColumnIndex
            etat.Ascendant = True
        End If

        Try
            TrierListe(source, propriete, etat.Ascendant)
        Catch ex As Exception
            GestionLog.EcrireLog(
                $"UtilsDataGrid - Erreur de tri sur la colonne '{propriete}'.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )
            Return
        End Try

        ' Réappliquer la liste triée à la grille (ou déléguer à la pagination).
        If etat.Rafraichir IsNot Nothing Then
            etat.Rafraichir.Invoke()
        Else
            dgv.DataSource = Nothing
            dgv.DataSource = source
            dgv.ClearSelection()
        End If

        ' Mettre à jour les glyphes de tri sur les en-têtes.
        For Each col As DataGridViewColumn In dgv.Columns
            col.HeaderCell.SortGlyphDirection = SortOrder.None
        Next
        colonne.HeaderCell.SortGlyphDirection =
            If(etat.Ascendant, SortOrder.Ascending, SortOrder.Descending)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : TrierListe
    ' Rôle       : Trie in-place une IList d'objets métier selon une propriété (réflexion) et un sens.
    ' -------------------------------------------------------------------------------------------------
    Private Sub TrierListe(source As IList, nomPropriete As String, ascendant As Boolean)

        If source.Count = 0 Then Return

        Dim prop As Reflection.PropertyInfo =
            source(0).GetType().GetProperty(nomPropriete)
        If prop Is Nothing Then Return

        Dim elements As New List(Of Object)
        For Each item In source
            elements.Add(item)
        Next

        elements.Sort(
            Function(a, b)
                Dim va = prop.GetValue(a, Nothing)
                Dim vb = prop.GetValue(b, Nothing)

                If va Is Nothing AndAlso vb Is Nothing Then Return 0
                If va Is Nothing Then Return If(ascendant, -1, 1)
                If vb Is Nothing Then Return If(ascendant, 1, -1)

                Dim cmp As Integer
                Dim ca = TryCast(va, IComparable)
                If ca IsNot Nothing Then
                    cmp = ca.CompareTo(vb)
                Else
                    cmp = String.Compare(va.ToString(), vb.ToString(), StringComparison.CurrentCultureIgnoreCase)
                End If

                Return If(ascendant, cmp, -cmp)
            End Function
        )

        source.Clear()
        For Each item In elements
            source.Add(item)
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverTri
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Rend toutes les colonnes non triables. Pour une grille dont l'ordre porte un sens (sections et
    ' leurs lignes, sessions d'un journal) : un tri automatique de la grille mélangerait les repères
    ' (DessinerRepereSurLigne) et les lignes qu'ils introduisent.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DesactiverTri(dgv As DataGridView)
        If dgv Is Nothing Then Return
        For Each col As DataGridViewColumn In dgv.Columns
            col.SortMode = DataGridViewColumnSortMode.NotSortable
        Next
    End Sub

    #End Region

    #Region "Tri : état par défaut"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TriUtilisateurActif
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Indique si l'utilisateur a trié la grille par un clic d'en-tête (sinon, l'ordre est celui de
    ' l'écran). Sert à n'afficher un repère d'ordre (ligne « Aujourd'hui ») que dans l'ordre prévu.
    ' -------------------------------------------------------------------------------------------------
    Public Function TriUtilisateurActif(dgv As DataGridView) As Boolean
        Dim etat As EtatTri = Nothing
        Return dgv IsNot Nothing AndAlso _etatsTri.TryGetValue(dgv, etat) AndAlso etat.ColonneIndex >= 0
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ElementSelectionne
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Objet de la ligne que l'utilisatrice a sélectionnée, ou Nothing.
    '
    ' Remarques  :
    ' - Une grille pose d'office sa première ligne en ligne courante quand on la remplit, même
    '   quand l'écran efface ensuite la sélection (UC_Pagination) : sans ce contrôle, « Encaisser »,
    '   « Reporter »… agissaient sur une ligne que personne n'avait choisie (relevé de Joëlle,
    '   22/10/2026). Une ligne courante non sélectionnée ne compte donc pas (même règle que les
    '   panneaux de rattachement des domaines).
    ' -------------------------------------------------------------------------------------------------
    Public Function ElementSelectionne(Of T As Class)(dgv As DataGridView) As T

        If dgv Is Nothing OrElse dgv.CurrentRow Is Nothing OrElse Not dgv.CurrentRow.Selected Then
            Return Nothing
        End If

        Return TryCast(dgv.CurrentRow.DataBoundItem, T)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ReinitialiserTri
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Oublie le tri par en-tête (glyphes compris) : à appeler quand l'écran remet sa liste dans son
    ' ordre à lui (rechargement, filtre).
    ' -------------------------------------------------------------------------------------------------
    Public Sub ReinitialiserTri(dgv As DataGridView)
        Dim etat As EtatTri = Nothing
        If dgv Is Nothing OrElse Not _etatsTri.TryGetValue(dgv, etat) Then Return
        etat.ColonneIndex = -1
        etat.Ascendant = True
        For Each col As DataGridViewColumn In dgv.Columns
            col.HeaderCell.SortGlyphDirection = SortOrder.None
        Next
    End Sub

    #End Region

    #Region "Socle commun (UI-9) : polices, double tampon, alignements"

    ' Polices des grilles, partagées : une police est un objet Windows, la recréer à chaque grille
    ' (et ne jamais la libérer) consommait des ressources pour rien. Ne jamais les libérer.
    Private ReadOnly PoliceEntetes As New Font("Calibri", 10.0F, FontStyle.Bold)
    Private ReadOnly PoliceCellules As New Font("Calibri", 10.0F, FontStyle.Regular)
    Private ReadOnly PoliceEntetesSecondaire As New Font("Calibri", 9.5F, FontStyle.Bold)
    Private ReadOnly PoliceCellulesSecondaire As New Font("Calibri", 9.5F, FontStyle.Regular)

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ActiverDoubleTampon
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Fait dessiner la grille hors écran puis l'affiche d'un coup : plus de scintillement au
    ' défilement, surtout dans les grilles qui dessinent dans leurs cellules (pastilles, repères).
    '
    ' Remarques  :
    ' - DoubleBuffered est une propriété protégée de Control : on l'active par réflexion, technique
    '   courante pour un DataGridView. Si elle échoue, la grille fonctionne comme avant (journal).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActiverDoubleTampon(dgv As DataGridView)
        Try
            Dim propriete As Reflection.PropertyInfo =
                GetType(Control).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
            propriete?.SetValue(dgv, True)
        Catch ex As Exception
            GestionLog.EcrireLog($"UI: double tampon non activé pour la grille {dgv.Name}.",
                                 GestionLog.LogLevel.Rapide, GestionLog.LogCategory.UI, ex)
        End Try
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédures : AlignerCentre / AlignerDroite
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Convention d'alignement des grilles (Reference_UI) : dates et heures centrées, montants et
    ' nombres à droite ; le texte reste à gauche (par défaut).
    '
    ' Remarques  :
    ' - Les en-têtes sont déjà centrés par InitDataGridBasique.
    ' - Une colonne Nothing est ignorée (colonne facultative d'un écran).
    ' -------------------------------------------------------------------------------------------------
    Public Sub AlignerCentre(ParamArray colonnes As DataGridViewColumn())
        For Each colonne In colonnes
            If colonne IsNot Nothing Then colonne.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Next
    End Sub

    Public Sub AlignerDroite(ParamArray colonnes As DataGridViewColumn())
        For Each colonne In colonnes
            If colonne IsNot Nothing Then colonne.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        Next
    End Sub

    #End Region

    #Region "Dessin dans les cellules : pastilles et séparateur"

    Private ReadOnly PolicePastille As New Font("Calibri", 9.5F, FontStyle.Bold)
    Private ReadOnly PoliceMention As New Font("Calibri", 9.0F)
    Private ReadOnly PoliceSeparateur As New Font("Calibri", 9.0F, FontStyle.Bold)
    Private Const HAUTEUR_PASTILLE As Integer = 20
    Private Const MARGE_PASTILLE As Integer = 6

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerPastilleCellule
    ' Version    : V1.2.1
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Dessine, dans l'évènement CellPainting, une pastille arrondie (fond pâle, texte foncé en gras)
    ' à la place du texte de la cellule, suivie si besoin d'une petite mention grise.
    '
    ' Paramètres :
    ' - dgv      : grille dessinée (marge basse éventuelle de la ligne)
    ' - e        : arguments de CellPainting (la cellule est marquée « dessinée », e.Handled = True)
    ' - texte    : texte de la pastille (vide = cellule vide, fond seul)
    ' - teinte   : couleurs de la pastille (UtilsPastilles)
    ' - mention  : petite mention grise à droite (facultative)
    ' - attenuee : ligne atténuée (séance non due…) : pastille éclaircie
    ' - contour  : pastille au trait (bord et texte dans la couleur du texte, sans fond), V1.1.0 —
    '              ex. « Aucun RDV » de la grille Patients, à distinguer de la pastille pleine « En pause »
    '
    ' Remarques  :
    ' - Le fond de la cellule (sélection comprise) et ses bordures sont peints par la grille
    '   (PaintBackground) : la pastille reste lisible sur une ligne sélectionnée.
    ' - Texte trop long : coupé avec « … » (TextFormatFlags.EndEllipsis).
    ' - V1.2.0 (UI-9) : toute pastille pleine reçoit un fin contour dans sa teinte (décision de
    '   Joëlle, 18/10/2026). Sans lui, « OK », « Actif », « Réalisée » disparaissaient sur une ligne
    '   alternée vert-de-gris ; un contour seulement sur les fonds proches donnait un rendu irrégulier.
    ' - V1.2.1 (19/10/2026) : le mode d'anticrénelage est rendu à la grille après la pastille. Le
    '   Graphics est partagé par toutes les cellules : laissé en AntiAlias, il élargissait et grisait
    '   les traits de grille de toutes les cellules dessinées ensuite (lignes « encadrées » sous la
    '   première pastille).
    ' -------------------------------------------------------------------------------------------------
    Public Sub DessinerPastilleCellule(dgv As DataGridView,
                                       e As DataGridViewCellPaintingEventArgs,
                                       texte As String,
                                       teinte As UtilsPastilles.TeintePastille,
                                       Optional mention As String = Nothing,
                                       Optional attenuee As Boolean = False,
                                       Optional contour As Boolean = False)

        If dgv Is Nothing OrElse e Is Nothing OrElse e.RowIndex < 0 Then Return

        e.PaintBackground(e.CellBounds, True)
        e.Handled = True
        If String.IsNullOrEmpty(texte) Then Return

        ' Zone utile : la cellule sans la marge basse de la ligne (DividerHeight, ex. ligne
        ' « Aujourd'hui »), que la grille compte dans CellBounds ; sinon la pastille descendrait
        ' dans le séparateur au lieu de s'aligner sur le texte des autres colonnes.
        Dim zone As Rectangle = e.CellBounds
        zone.Height -= dgv.Rows(e.RowIndex).DividerHeight

        Dim g As Graphics = e.Graphics
        Dim modeGrille As Drawing2D.SmoothingMode = g.SmoothingMode
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Try
            DessinerPastille(g, zone, texte, teinte, mention, attenuee, contour)
        Finally
            g.SmoothingMode = modeGrille
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerPastilleZone
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Dessine une pastille (mêmes forme, police et couleurs que dans les grilles) dans une zone
    ' quelconque, alignée à gauche : éléments d'une AltheaListe de filtre (ElementListePastille).
    '
    ' Remarques  :
    ' - L'anticrénelage est posé le temps du dessin puis rendu à l'appelant.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DessinerPastilleZone(g As Graphics, zone As Rectangle, texte As String, teinte As UtilsPastilles.TeintePastille,
                                    Optional contour As Boolean = False)

        If g Is Nothing OrElse String.IsNullOrEmpty(texte) Then Return

        Dim mode As Drawing2D.SmoothingMode = g.SmoothingMode
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Try
            DessinerPastille(g, zone, texte, teinte, Nothing, False, contour)
        Finally
            g.SmoothingMode = mode
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerPastilleAvantTexte
    ' Version    : V1.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Dans l'évènement CellPainting, dessine une pastille (« Inactif ») puis le texte ordinaire de la
    ' cellule à sa suite, dans la police et la couleur de son style (sélection comprise) : nom d'un
    ' patient inactif de la grille Patients.
    '
    ' Remarques  :
    ' - Même pastille que DessinerPastilleCellule ; texte trop long coupé avec « … ».
    ' -------------------------------------------------------------------------------------------------
    Public Sub DessinerPastilleAvantTexte(dgv As DataGridView,
                                          e As DataGridViewCellPaintingEventArgs,
                                          pastille As String,
                                          teinte As UtilsPastilles.TeintePastille)

        If dgv Is Nothing OrElse e Is Nothing OrElse e.RowIndex < 0 OrElse String.IsNullOrEmpty(pastille) Then Return

        e.PaintBackground(e.CellBounds, True)
        e.Handled = True

        Dim zone As Rectangle = e.CellBounds
        zone.Height -= dgv.Rows(e.RowIndex).DividerHeight

        Dim g As Graphics = e.Graphics
        Dim drapeaux As TextFormatFlags = TextFormatFlags.NoPrefix Or TextFormatFlags.NoPadding
        Dim largeurPastille As Integer = TextRenderer.MeasureText(g, pastille, PolicePastille, Size.Empty, drapeaux).Width + 18

        Dim modeGrille As Drawing2D.SmoothingMode = g.SmoothingMode
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Try
            DessinerPastille(g, zone, pastille, teinte, Nothing, False, False)
        Finally
            g.SmoothingMode = modeGrille
        End Try

        Dim x As Integer = zone.X + 2 * MARGE_PASTILLE + largeurPastille
        Dim zoneTexte As New Rectangle(x, zone.Y, zone.Right - x - MARGE_PASTILLE, zone.Height)
        If zoneTexte.Width <= 0 Then Return

        Dim selectionnee As Boolean = (e.State And DataGridViewElementStates.Selected) = DataGridViewElementStates.Selected
        Dim couleur As Color = If(selectionnee, e.CellStyle.SelectionForeColor, e.CellStyle.ForeColor)
        TextRenderer.DrawText(g, Convert.ToString(e.FormattedValue), e.CellStyle.Font, zoneTexte, couleur,
                              drapeaux Or TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

    End Sub

    ' Décalage du texte dans une pastille dessinée par DessinerPastilleZone (marge + retrait
    ' intérieur) : un texte simple de la même liste s'aligne ainsi sur celui des pastilles.
    Public ReadOnly Property RetraitTextePastille As Integer
        Get
            Return MARGE_PASTILLE + 9
        End Get
    End Property

    ' Dessin de la pastille et de sa mention (anticrénelage posé par DessinerPastilleCellule).
    Private Sub DessinerPastille(g As Graphics, zone As Rectangle, texte As String,
                                 teinte As UtilsPastilles.TeintePastille, mention As String,
                                 attenuee As Boolean, contour As Boolean)

        Dim drapeaux As TextFormatFlags = TextFormatFlags.NoPrefix Or TextFormatFlags.NoPadding
        Dim tailleTexte As Size = TextRenderer.MeasureText(g, texte, PolicePastille, Size.Empty, drapeaux)
        Dim hauteur As Integer = Math.Min(HAUTEUR_PASTILLE, zone.Height - 6)
        Dim largeur As Integer = Math.Min(tailleTexte.Width + 18, zone.Width - 2 * MARGE_PASTILLE)
        If largeur <= 0 OrElse hauteur <= 0 Then Return

        Dim pastille As New Rectangle(zone.X + MARGE_PASTILLE,
                                      zone.Y + (zone.Height - hauteur) \ 2,
                                      largeur, hauteur)

        Dim fond As Color = If(attenuee, Color.FromArgb(150, teinte.Fond), teinte.Fond)
        Dim couleurTexte As Color = If(attenuee, Color.FromArgb(170, teinte.Texte), teinte.Texte)

        If contour Then
            ' Rectangle réduit d'un pixel : le trait reste entier dans la cellule.
            Dim cadre As New Rectangle(pastille.X, pastille.Y, pastille.Width - 1, pastille.Height - 1)
            Using chemin As Drawing2D.GraphicsPath = AltheaDessin.CheminArrondi(cadre, cadre.Height / 2.0F)
                Using stylo As New Pen(couleurTexte, 1.0F) With {.DashStyle = Drawing2D.DashStyle.Dash}
                    g.DrawPath(stylo, chemin)
                End Using
            End Using
        Else
            Using chemin As Drawing2D.GraphicsPath = AltheaDessin.CheminArrondi(pastille, hauteur / 2.0F)
                Using pinceau As New SolidBrush(fond)
                    g.FillPath(pinceau, chemin)
                End Using
            End Using

            ' Fin contour dans la teinte de la pastille (texte et fond mélangés) : la forme reste nette
            ' sur tous les fonds de ligne (blanc, alterné, sélectionné), de façon régulière.
            Dim cadre As New Rectangle(pastille.X, pastille.Y, pastille.Width - 1, pastille.Height - 1)
            Using chemin As Drawing2D.GraphicsPath = AltheaDessin.CheminArrondi(cadre, cadre.Height / 2.0F)
                Using stylo As New Pen(AltheaDessin.Melanger(couleurTexte, fond, 0.6F), 1.0F)
                    g.DrawPath(stylo, chemin)
                End Using
            End Using
        End If

        TextRenderer.DrawText(g, texte, PolicePastille, pastille, couleurTexte,
                              drapeaux Or TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or
                              TextFormatFlags.EndEllipsis)

        If String.IsNullOrEmpty(mention) Then Return

        Dim zoneMention As New Rectangle(pastille.Right + MARGE_PASTILLE, zone.Y,
                                         zone.Right - pastille.Right - 2 * MARGE_PASTILLE, zone.Height)
        If zoneMention.Width <= 0 Then Return
        TextRenderer.DrawText(g, mention, PoliceMention, zoneMention, UITheme.ColorGrisDiscret,
                              drapeaux Or TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerSeparateurSousLigne
    ' Version    : V1.0.1
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Dans l'évènement RowPostPaint, dessine un séparateur titré (« Aujourd'hui — … ») dans la marge
    ' basse de la ligne : un trait sauge de part et d'autre du texte.
    '
    ' Remarques  :
    ' - La marge vient de DataGridViewRow.DividerHeight, posée par l'écran sur la ligne qui précède
    '   le séparateur (0 ailleurs) ; sans marge, rien n'est dessiné.
    ' - Aucune ligne factice dans la source : sélection, tri et double-clic ne sont pas touchés.
    ' - V1.0.1 : dessin du trait et du titre mis en commun avec DessinerRepereSurLigne (DessinerRepere).
    ' -------------------------------------------------------------------------------------------------
    Public Sub DessinerSeparateurSousLigne(e As DataGridViewRowPostPaintEventArgs, dgv As DataGridView, texte As String)

        If e Is Nothing OrElse dgv Is Nothing OrElse e.RowIndex < 0 OrElse e.RowIndex >= dgv.Rows.Count Then Return

        Dim marge As Integer = dgv.Rows(e.RowIndex).DividerHeight
        If marge <= 0 Then Return

        Dim zone As New Rectangle(e.RowBounds.Left, e.RowBounds.Bottom - marge, e.RowBounds.Width, marge)
        DessinerRepere(e.Graphics, dgv, zone, texte, dgv.DefaultCellStyle.BackColor, pointille:=False)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerRepereSurLigne
    ' Version    : V1.1.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Dans l'évènement RowPostPaint, dessine sur toute une ligne réservée un repère titré : trait
    ' plein (titre de section) ou pointillé (créneau libre d'un planning), texte au centre.
    '
    ' Paramètres :
    ' - e         : arguments de RowPostPaint de la ligne réservée
    ' - dgv       : grille dessinée
    ' - texte     : titre du repère (« Libre · 10:20 – 14:00 », « Sauvegardes »)
    ' - pointille : True pour un trait pointillé sauge clair (place disponible), False pour un trait
    '               plein sauge (section)
    '
    ' Remarques  :
    ' - Pour un repère qui n'a pas de ligne au-dessus de lui (premier créneau libre de la journée,
    '   première section) : DessinerSeparateurSousLigne ne peut pas servir. Réservé aux grilles en
    '   lecture seule et non triables (DesactiverTri).
    ' - L'écran laisse la cellule vide (CellPainting : fond seul). Une ligne qui n'est qu'un repère
    '   rend sa sélection invisible (SelectionBackColor = BackColor).
    ' - V1.1.0 (UI-9.3) : une ligne sélectionnée est peinte dans sa couleur de sélection — l'en-tête de
    '   session d'un journal se sélectionne pour afficher son détail.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DessinerRepereSurLigne(e As DataGridViewRowPostPaintEventArgs, dgv As DataGridView, texte As String,
                                      Optional pointille As Boolean = False)

        If e Is Nothing OrElse dgv Is Nothing OrElse e.RowIndex < 0 OrElse e.RowIndex >= dgv.Rows.Count Then Return

        Dim ligne As DataGridViewRow = dgv.Rows(e.RowIndex)
        Dim zone As New Rectangle(e.RowBounds.Left, e.RowBounds.Top, e.RowBounds.Width, e.RowBounds.Height - ligne.DividerHeight)
        Dim style As DataGridViewCellStyle = ligne.InheritedStyle
        DessinerRepere(e.Graphics, dgv, zone, texte, If(ligne.Selected, style.SelectionBackColor, style.BackColor), pointille)

    End Sub

    ' Trait de part et d'autre d'un titre centré sur la partie visible de la grille.
    Private Sub DessinerRepere(g As Graphics, dgv As DataGridView, zone As Rectangle, texte As String,
                               couleurFond As Color, pointille As Boolean)

        Using fond As New SolidBrush(couleurFond)
            g.FillRectangle(fond, zone)
        End Using

        Dim drapeaux As TextFormatFlags = TextFormatFlags.NoPrefix Or TextFormatFlags.NoPadding
        Dim tailleTexte As Size = TextRenderer.MeasureText(g, texte, PoliceSeparateur, Size.Empty, drapeaux)
        Dim largeurVisible As Integer = Math.Min(zone.Width, dgv.DisplayRectangle.Width)
        Dim xTexte As Integer = zone.Left + Math.Max(0, (largeurVisible - tailleTexte.Width) \ 2)
        Dim yMilieu As Integer = zone.Top + zone.Height \ 2

        Using stylo As New Pen(If(pointille, UITheme.ColorSaugeClair, UITheme.ColorSauge), 2.0F)
            If pointille Then stylo.DashStyle = Drawing2D.DashStyle.Dash
            g.DrawLine(stylo, zone.Left + 8, yMilieu, xTexte - 10, yMilieu)
            g.DrawLine(stylo, xTexte + tailleTexte.Width + 10, yMilieu, zone.Left + largeurVisible - 8, yMilieu)
        End Using

        TextRenderer.DrawText(g, texte, PoliceSeparateur,
                              New Rectangle(xTexte, zone.Top, tailleTexte.Width + 2, zone.Height),
                              If(pointille, UITheme.ColorSaugeTresFonce, UITheme.ColorSaugeFonce),
                              drapeaux Or TextFormatFlags.VerticalCenter)

    End Sub

    #End Region

End Module
