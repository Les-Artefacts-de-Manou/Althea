' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaEnTete
' Projet     : Althéa
' Version    : V1.1.0
' Date       : 16/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' En-tête d'écran du kit Althéa : icône, titre, sous-titre et trait fin, dessinés par le code.
' Remplace le trio picTitre / lblTitreForm / lblTop (ou lblTitre) posé dans un pnlTitre.
'
' Responsabilités :
' - Dessiner l'icône de l'écran à gauche, à taille fixe
' - Dessiner le titre (Mauve foncé) et, en dessous, le sous-titre (Sauge foncé)
' - Souligner l'en-tête d'un trait fin sur toute la largeur
'
' Remarques  :
' - Hérite de Panel : un écran qui a besoin d'éléments en plus dans son en-tête (fiche
'   Dossier, par exemple) peut toujours y poser des contrôles enfants.
' - Text porte le titre ; SousTitre et Icone se règlent dans le Designer (catégorie « Althéa »)
'   ou par le code, comme le faisaient lblTitreForm.Text, lblTop.Text et picTitre.BackgroundImage.
' - Fond transparent : l'en-tête laisse voir le fond de l'écran et le feuillage.
' - Les textes trop longs sont tronqués avec « … » plutôt que de déborder sur l'icône.
' - V1.1.0 (16/10/2026) : textes dessinés avec AltheaDessin.FLAGS_TEXTE_SOUS_ENFANTS (pas de titre
'   fantôme sous un contrôle à fond recopié posé dans l'en-tête).
'
' Dépendances :
' - UITheme, AltheaDessin
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("En-tête d'écran Althéa : icône, titre, sous-titre et trait fin.")>
Public Class AltheaEnTete
    Inherits Panel

#Region "Constantes"

    ' Espace entre l'icône et les textes.
    Private Const EcartIconeTexte As Integer = 12

#End Region

#Region "Variables privées"

    Private _sousTitre As String = String.Empty
    Private _icone As Image
    Private _tailleIcone As Integer = 48

    Private _couleurTitre As Color = UITheme.ColorMauveFonce
    Private _couleurSousTitre As Color = UITheme.ColorSaugeFonce
    Private _couleurTrait As Color = UITheme.ColorBeigeFonce

    Private _policeTitre As Font = New Font("Calibri", 18.0F, FontStyle.Bold)
    Private _policeSousTitre As Font = New Font("Calibri", 11.0F)

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        MyBase.BackColor = Color.Transparent
        Dock = DockStyle.Top
        Padding = New Padding(16, 4, 16, 4)
        Size = New Size(600, 58)

    End Sub

#End Region

#Region "Propriétés publiques"

    ' Panel masque Text dans le Designer : il est réaffiché ici pour porter le titre.
    <Browsable(True)>
    <EditorBrowsable(EditorBrowsableState.Always)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <Category("Althéa")>
    <Description("Titre de l'écran (Mauve foncé).")>
    Public Overrides Property Text As String
        Get
            Return MyBase.Text
        End Get
        Set(value As String)
            MyBase.Text = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Sous-titre affiché sous le titre (vide = pas de sous-titre).")>
    <DefaultValue("")>
    Public Property SousTitre As String
        Get
            Return _sousTitre
        End Get
        Set(value As String)
            _sousTitre = If(value, String.Empty)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Icône de l'écran (Assets/Titre_Ico), dessinée à gauche.")>
    <DefaultValue(GetType(Image), Nothing)>
    Public Property Icone As Image
        Get
            Return _icone
        End Get
        Set(value As Image)
            _icone = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Côté de l'icône, en pixels (réduit si l'en-tête est moins haut).")>
    <DefaultValue(48)>
    Public Property TailleIcone As Integer
        Get
            Return _tailleIcone
        End Get
        Set(value As Integer)
            _tailleIcone = Math.Max(0, value)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Couleur du titre.")>
    Public Property CouleurTitre As Color
        Get
            Return _couleurTitre
        End Get
        Set(value As Color)
            _couleurTitre = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Couleur du sous-titre.")>
    Public Property CouleurSousTitre As Color
        Get
            Return _couleurSousTitre
        End Get
        Set(value As Color)
            _couleurSousTitre = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Couleur du trait fin sous l'en-tête (Transparent = pas de trait).")>
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
    <Description("Police du titre.")>
    Public Property PoliceTitre As Font
        Get
            Return _policeTitre
        End Get
        Set(value As Font)
            If value Is Nothing Then Return
            _policeTitre = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Police du sous-titre.")>
    Public Property PoliceSousTitre As Font
        Get
            Return _policeSousTitre
        End Get
        Set(value As Font)
            If value Is Nothing Then Return
            _policeSousTitre = value
            Invalidate()
        End Set
    End Property

    ' Le fond reste transparent : la couleur éventuelle posée par un utilitaire est ignorée.
    <Browsable(False)>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overrides Property BackColor As Color
        Get
            Return Color.Transparent
        End Get
        Set(value As Color)
            ' Ignoré volontairement.
        End Set
    End Property

#End Region

#Region "Sérialisation Designer"

    Private Function ShouldSerializeCouleurTitre() As Boolean
        Return _couleurTitre <> UITheme.ColorMauveFonce
    End Function

    Private Sub ResetCouleurTitre()
        CouleurTitre = UITheme.ColorMauveFonce
    End Sub

    Private Function ShouldSerializeCouleurSousTitre() As Boolean
        Return _couleurSousTitre <> UITheme.ColorSaugeFonce
    End Function

    Private Sub ResetCouleurSousTitre()
        CouleurSousTitre = UITheme.ColorSaugeFonce
    End Sub

    Private Function ShouldSerializeCouleurTrait() As Boolean
        Return _couleurTrait <> UITheme.ColorBeigeFonce
    End Function

    Private Sub ResetCouleurTrait()
        CouleurTrait = UITheme.ColorBeigeFonce
    End Sub

    Private Function ShouldSerializePoliceTitre() As Boolean
        Return Not (_policeTitre.Name = "Calibri" AndAlso _policeTitre.Size = 18.0F AndAlso _policeTitre.Style = FontStyle.Bold)
    End Function

    Private Sub ResetPoliceTitre()
        PoliceTitre = New Font("Calibri", 18.0F, FontStyle.Bold)
    End Sub

    Private Function ShouldSerializePoliceSousTitre() As Boolean
        Return Not (_policeSousTitre.Name = "Calibri" AndAlso _policeSousTitre.Size = 11.0F AndAlso _policeSousTitre.Style = FontStyle.Regular)
    End Function

    Private Sub ResetPoliceSousTitre()
        PoliceSousTitre = New Font("Calibri", 11.0F)
    End Sub

#End Region

#Region "Dessin"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnPaint
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Icône centrée verticalement à gauche ; titre et sous-titre empilés, centrés ensemble
    ' verticalement ; trait fin sur la dernière ligne de pixels.
    '
    ' Remarques  :
    ' - Le fond transparent est déjà peint par OnPaintBackground (fond du parent, feuillage
    '   compris).
    ' - Les hauteurs de texte sont mesurées : l'en-tête reste juste à toutes les mises à
    '   l'échelle et pour n'importe quelle police réglée dans le Designer.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        Dim g As Graphics = e.Graphics
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        ' Zone utile : padding déduit, dernière ligne réservée au trait.
        Dim zone As New Rectangle(Padding.Left, Padding.Top,
                                  Math.Max(0, Width - Padding.Horizontal),
                                  Math.Max(0, Height - Padding.Vertical - 1))

        Dim xTexte As Integer = zone.Left

        If _icone IsNot Nothing AndAlso _tailleIcone > 0 Then
            Dim cote As Integer = Math.Min(_tailleIcone, zone.Height)
            Dim rectIcone As New Rectangle(zone.Left, zone.Top + (zone.Height - cote) \ 2, cote, cote)
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            AltheaDessin.DessinerImage(g, _icone, rectIcone, If(Enabled, 1.0F, 0.45F))
            xTexte = rectIcone.Right + EcartIconeTexte
        End If

        DessinerTextes(g, New Rectangle(xTexte, zone.Top, Math.Max(0, zone.Right - xTexte), zone.Height))

        If _couleurTrait.A > 0 Then
            Using stylo As New Pen(_couleurTrait, 1.0F)
                g.DrawLine(stylo, Padding.Left, Height - 1, Width - Padding.Right, Height - 1)
            End Using
        End If

    End Sub

    Private Sub DessinerTextes(g As Graphics, zone As Rectangle)

        If zone.Width <= 0 Then Return

        Dim flags As TextFormatFlags = TextFormatFlags.Left Or
                                       TextFormatFlags.Top Or
                                       TextFormatFlags.SingleLine Or
                                       TextFormatFlags.EndEllipsis Or
                                       TextFormatFlags.NoPrefix Or
                                       TextFormatFlags.NoPadding Or
                                       AltheaDessin.FLAGS_TEXTE_SOUS_ENFANTS

        Dim titre As String = TexteSurUneLigne(Text)
        Dim sousTitre As String = TexteSurUneLigne(_sousTitre)

        Dim hTitre As Integer = If(titre.Length > 0, TextRenderer.MeasureText(g, titre, _policeTitre, Size.Empty, flags).Height, 0)
        Dim hSousTitre As Integer = If(sousTitre.Length > 0, TextRenderer.MeasureText(g, sousTitre, _policeSousTitre, Size.Empty, flags).Height, 0)

        Dim y As Integer = zone.Top + Math.Max(0, (zone.Height - hTitre - hSousTitre) \ 2)

        If hTitre > 0 Then
            TextRenderer.DrawText(g, titre, _policeTitre,
                                  New Rectangle(zone.Left, y, zone.Width, hTitre),
                                  If(Enabled, _couleurTitre, AltheaDessin.CouleurDesactiveTexte), flags)
            y += hTitre
        End If

        If hSousTitre > 0 Then
            TextRenderer.DrawText(g, sousTitre, _policeSousTitre,
                                  New Rectangle(zone.Left, y, zone.Width, hSousTitre),
                                  If(Enabled, _couleurSousTitre, AltheaDessin.CouleurDesactiveTexte), flags)
        End If

    End Sub

    ' Certains anciens titres contenaient un retour à la ligne : il devient une espace.
    Private Shared Function TexteSurUneLigne(texte As String) As String

        If String.IsNullOrWhiteSpace(texte) Then Return String.Empty

        Return String.Join(" ", texte.Split({vbCrLf, vbLf, vbCr}, StringSplitOptions.RemoveEmptyEntries).
                                       Select(Function(morceau) morceau.Trim()))

    End Function

#End Region

End Class
