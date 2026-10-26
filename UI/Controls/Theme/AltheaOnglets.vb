' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaOnglets
' Projet     : Althéa
' Version    : V1.2.0
' Date       : 14/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Onglets du kit Althéa : barre d'onglets soulignés, dessinée par le code, sans le cadre en
' relief du TabControl Windows.
'
' Responsabilités :
' - Dessiner la bande d'onglets (texte, état actif, survol) et le trait fin qui la souligne
' - Souligner l'onglet actif d'un trait sauge
' - Neutraliser le cadre 3D que le contrôle Windows dessine autour de la zone de contenu
'
' Remarques  :
' - Hérite de `TabControl` : les `TabPage` du Designer, leur contenu, leur ordre et le code
'   existant (`SelectedIndexChanged`, `SelectedTab`…) restent inchangés. La migration d'un
'   écran se limite au type déclaré, et s'annule aussi simplement.
' - `ControlStyles.UserPaint` reprend le dessin de la zone d'onglets. Les `TabPage` sont des
'   fenêtres enfants : elles continuent de s'afficher et de se peindre elles-mêmes.
' - La zone de contenu n'est plus encadrée : c'est le trait sous la bande qui sépare.
' - La police n'est pas imposée : comme un TabControl, le contrôle hérite celle de son parent,
'   et les pages la transmettent à leur contenu. L'imposer ici changerait le texte des pages.
' - `CouleurFond = Transparent` laisse voir le fond du parent (feuillage de la Home compris)
'   dans la bande d'onglets : il est recopié dans le tampon du contrôle, sans scintillement.
' - Les couleurs viennent de `UITheme` ; seules les teintes d'état désactivé viennent
'   d'`AltheaDessin`, comme pour le reste du kit.
' - `MasquerOnglets` (14/10/2026) : à l'exécution, la bande disparaît et les pages occupent tout
'   le contrôle ; la navigation est alors confiée à un autre contrôle (AltheaEtapes pour les
'   Domaines). Dans le Designer, les onglets restent visibles pour pouvoir éditer chaque page.
'
' Dépendances :
' - UITheme, AltheaDessin, AltheaFondParent
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Onglets Althéa : barre soulignée dessinée par le code, sans cadre en relief.")>
Public Class AltheaOnglets
    Inherits TabControl

#Region "Variables privées"

    ' Onglet actuellement survolé (-1 si aucun).
    Private _indexSurvole As Integer = -1

    ' Police de l'onglet actif, dérivée de Font (recréée quand Font change).
    Private _policeActive As Font

    ' TabControl ignore sa propriété BackColor : elle renvoie toujours le gris système
    ' (240; 240; 240), quelle que soit la valeur affectée. La couleur de fond est donc
    ' portée par une propriété propre au contrôle.
    Private _couleurFond As Color = UITheme.ColorBeigeClair

    Private _couleurTrait As Color = UITheme.ColorBeigeFonce
    Private _couleurSoulignement As Color = UITheme.ColorSauge
    Private _epaisseurSoulignement As Integer = 3
    Private _masquerOnglets As Boolean

    ' Message Windows par lequel le contrôle calcule la zone des pages à partir de sa taille.
    Private Const TCM_ADJUSTRECT As Integer = &H1328

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw, True)

        ' Le dessin des onglets est entièrement repris ici ; le mode propriétaire évite que
        ' Windows peigne les siens par-dessus.
        DrawMode = TabDrawMode.OwnerDrawFixed
        Appearance = TabAppearance.Normal
        SizeMode = TabSizeMode.Normal
        Alignment = TabAlignment.Top

        ' Respiration autour du libellé de chaque onglet.
        Padding = New Point(14, 5)
        ItemSize = New Size(0, 30)

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Couleur de fond de la barre d'onglets et de la marge autour des pages (Transparent = fond du parent).")>
    Public Property CouleurFond As Color
        Get
            Return _couleurFond
        End Get
        Set(value As Color)
            _couleurFond = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Couleur du trait fin qui souligne toute la barre d'onglets.")>
    Public Property CouleurTrait As Color
        Get
            Return _couleurTrait
        End Get
        Set(value As Color)
            _couleurTrait = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Couleur du soulignement de l'onglet actif.")>
    Public Property CouleurSoulignement As Color
        Get
            Return _couleurSoulignement
        End Get
        Set(value As Color)
            _couleurSoulignement = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Épaisseur, en pixels, du soulignement de l'onglet actif.")>
    <DefaultValue(3)>
    Public Property EpaisseurSoulignement As Integer
        Get
            Return _epaisseurSoulignement
        End Get
        Set(value As Integer)
            _epaisseurSoulignement = Math.Max(1, value)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("À l'exécution : masque la bande d'onglets, les pages occupent tout le contrôle (la navigation est confiée à un autre contrôle). Sans effet dans le Designer.")>
    <DefaultValue(False)>
    Public Property MasquerOnglets As Boolean
        Get
            Return _masquerOnglets
        End Get
        Set(value As Boolean)
            If _masquerOnglets = value Then Return
            _masquerOnglets = value
            ' La zone des pages est calculée par Windows à la création du contrôle.
            If IsHandleCreated Then RecreateHandle()
            Invalidate()
        End Set
    End Property

#End Region

#Region "Sérialisation Designer"

    Private Function ShouldSerializeCouleurFond() As Boolean
        Return _couleurFond <> UITheme.ColorBeigeClair
    End Function

    Private Sub ResetCouleurFond()
        CouleurFond = UITheme.ColorBeigeClair
    End Sub

    Private Function ShouldSerializeCouleurTrait() As Boolean
        Return _couleurTrait <> UITheme.ColorBeigeFonce
    End Function

    Private Sub ResetCouleurTrait()
        CouleurTrait = UITheme.ColorBeigeFonce
    End Sub

    Private Function ShouldSerializeCouleurSoulignement() As Boolean
        Return _couleurSoulignement <> UITheme.ColorSauge
    End Function

    Private Sub ResetCouleurSoulignement()
        CouleurSoulignement = UITheme.ColorSauge
    End Sub

#End Region

#Region "Dessin"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : BasDeLaBande
    ' Rôle       : Ordonnée du trait de séparation, sous la barre d'onglets.
    ' -------------------------------------------------------------------------------------------------
    Private Function BasDeLaBande() As Integer

        If TabCount > 0 Then
            Return GetTabRect(0).Bottom
        End If

        Return ItemSize.Height

    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        ' Fond : le contrôle Windows ne peint plus rien de lui-même. Une couleur (semi-)
        ' transparente laisse d'abord voir le parent.
        g.SmoothingMode = SmoothingMode.None

        If _couleurFond.A < 255 Then
            AltheaFondParent.Peindre(Me, e)
        End If

        If _couleurFond.A > 0 Then
            Using pinceau As New SolidBrush(_couleurFond)
                g.FillRectangle(pinceau, ClientRectangle)
            End Using
        End If

        If BandeMasquee() Then Return

        Dim bas As Integer = BasDeLaBande()

        ' Les onglets au repos reçoivent un fond légèrement plus soutenu : sans lui, un
        ' libellé seul « flotte » et ne se lit pas comme un onglet.
        g.SmoothingMode = SmoothingMode.AntiAlias
        For i As Integer = 0 To TabCount - 1
            If i <> SelectedIndex Then DessinerFondOnglet(g, i, bas)
        Next
        g.SmoothingMode = SmoothingMode.None

        ' Trait fin sur toute la largeur : il remplace le cadre d'origine.
        If _couleurTrait.A > 0 Then
            Using stylo As New Pen(_couleurTrait, 1.0F)
                g.DrawLine(stylo, 0, bas, Width, bas)
            End Using
        End If

        For i As Integer = 0 To TabCount - 1
            DessinerOnglet(g, i, bas)
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerFondOnglet
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Fond arrondi d'un onglet au repos ou survolé.
    '
    ' Remarques  :
    ' - Le rectangle déborde sous le trait de séparation : seuls les coins hauts se voient,
    '   l'onglet paraît ainsi rattaché à la bande.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerFondOnglet(g As Graphics, index As Integer, bas As Integer)

        If Not Enabled Then Return

        Dim zone As Rectangle = GetTabRect(index)
        Dim couleur As Color = If(index = _indexSurvole, UITheme.ColorGrisVertClair, UITheme.ColorBeige)

        Dim forme As New RectangleF(zone.X + 1.0F, 3.0F, zone.Width - 2.0F, bas + 4.0F)

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(forme, 6.0F)
            Using pinceau As New SolidBrush(couleur)
                g.FillPath(pinceau, chemin)
            End Using
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerOnglet
    ' Rôle       : Libellé d'un onglet, et soulignement sauge lorsqu'il est actif.
    '
    ' Remarques  :
    ' - `GetTabRect` renvoie un rectangle légèrement plus haut pour l'onglet sélectionné : la
    '   hauteur est ramenée à celle de la bande pour que tous les libellés s'alignent.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerOnglet(g As Graphics, index As Integer, bas As Integer)

        Dim zone As Rectangle = GetTabRect(index)
        zone = New Rectangle(zone.X, 0, zone.Width, bas)

        Dim actif As Boolean = (index = SelectedIndex)
        Dim survole As Boolean = (index = _indexSurvole) AndAlso Not actif

        Dim couleur As Color

        If Not Enabled Then
            couleur = AltheaDessin.CouleurDesactiveTexte
        ElseIf actif Then
            couleur = UITheme.ColorSaugeTresFonce
        ElseIf survole Then
            couleur = UITheme.ColorSaugeTresFonce
        Else
            couleur = UITheme.ColorGrisFonce
        End If

        Dim police As Font = If(actif, PoliceActive(), Font)

        Dim flags As TextFormatFlags = TextFormatFlags.HorizontalCenter Or
                                       TextFormatFlags.VerticalCenter Or
                                       TextFormatFlags.SingleLine Or
                                       TextFormatFlags.EndEllipsis Or
                                       TextFormatFlags.NoPadding

        TextRenderer.DrawText(g, TabPages(index).Text, police, zone, couleur, flags)

        If actif AndAlso Enabled AndAlso _couleurSoulignement.A > 0 Then

            Using stylo As New Pen(_couleurSoulignement, CSng(_epaisseurSoulignement))
                Dim y As Single = bas - _epaisseurSoulignement / 2.0F
                g.DrawLine(stylo, zone.Left + 2, y, zone.Right - 2, y)
            End Using

        End If

    End Sub

    Private Function BandeMasquee() As Boolean
        Return _masquerOnglets AndAlso Not DesignMode
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : WndProc
    ' Rôle       : Bande masquée : la zone des pages devient tout le contrôle (TCM_ADJUSTRECT
    '              est laissé sans calcul de marge).
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub WndProc(ByRef m As Message)

        If m.Msg = TCM_ADJUSTRECT AndAlso BandeMasquee() Then
            m.Result = New IntPtr(1)
            Return
        End If

        MyBase.WndProc(m)

    End Sub

    Private Function PoliceActive() As Font

        If _policeActive Is Nothing Then
            _policeActive = New Font(Font, FontStyle.Bold)
        End If

        Return _policeActive

    End Function

#End Region

#Region "Événements"

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)

        Dim trouve As Integer = -1

        For i As Integer = 0 To TabCount - 1
            If GetTabRect(i).Contains(e.Location) Then
                trouve = i
                Exit For
            End If
        Next

        If trouve <> _indexSurvole Then
            _indexSurvole = trouve
            Invalidate()
        End If

        MyBase.OnMouseMove(e)

    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)

        If _indexSurvole <> -1 Then
            _indexSurvole = -1
            Invalidate()
        End If

        MyBase.OnMouseLeave(e)

    End Sub

    ' Le mode UserPaint ne rafraîchit pas la bande de lui-même au changement d'onglet.
    Protected Overrides Sub OnSelectedIndexChanged(e As EventArgs)

        Invalidate()
        MyBase.OnSelectedIndexChanged(e)

    End Sub

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)

        _indexSurvole = -1
        Invalidate()
        MyBase.OnEnabledChanged(e)

    End Sub

    Protected Overrides Sub OnFontChanged(e As EventArgs)

        If _policeActive IsNot Nothing Then
            _policeActive.Dispose()
            _policeActive = Nothing
        End If

        MyBase.OnFontChanged(e)

    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)

        If disposing AndAlso _policeActive IsNot Nothing Then
            _policeActive.Dispose()
            _policeActive = Nothing
        End If

        MyBase.Dispose(disposing)

    End Sub

#End Region

End Class
