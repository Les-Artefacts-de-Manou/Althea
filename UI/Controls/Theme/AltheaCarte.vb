' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaCarte
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 09/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Panneau du kit Althéa : carte translucide à coins arrondis, avec ombre douce et titre de
' section facultatif. Remplace les panneaux habillés par une image de fond étirée.
'
' Responsabilités :
' - Peindre le fond du parent (transparence), puis la carte translucide par-dessus
' - Dessiner l'ombre, la bordure fine et le titre facultatif (texte + trait)
' - Réserver la place du titre : les contrôles enfants (même en Dock) se placent dessous
'
' Remarques  :
' - BackColor est forcé à Transparent : la couleur de la carte se règle avec CouleurFond et
'   OpaciteFond (0 = invisible, 255 = opaque). Par défaut : blanc à ~55 %, le fond et le
'   feuillage restent visibles à travers.
' - Le titre est dessiné à l'intérieur, en haut : plus de titre masqué par la bordure d'une image.
' - Les enfants transparents (Label, Panel…) affichent la carte derrière eux.
'
' Dépendances :
' - UITheme, AltheaDessin
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Carte Althéa : panneau translucide arrondi avec ombre douce et titre facultatif.")>
Public Class AltheaCarte
    Inherits Panel

#Region "Variables privées"

    Private _rayon As Integer = 14
    Private _titre As String = String.Empty
    Private _couleurFond As Color = Color.White
    Private _opaciteFond As Integer = 140
    Private _couleurBordure As Color = UITheme.ColorBeigeFonce
    Private _ombre As Boolean = True
    Private _policeTitre As Font = New Font("Calibri", 12.0F, FontStyle.Bold)
    Private _couleurTitre As Color = UITheme.ColorMauveFonce

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        MyBase.BackColor = Color.Transparent
        Padding = New Padding(18, 14, 18, 16)

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Rayon des coins arrondis, en pixels.")>
    <DefaultValue(14)>
    Public Property Rayon As Integer
        Get
            Return _rayon
        End Get
        Set(value As Integer)
            _rayon = Math.Max(0, value)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Titre de section dessiné en haut de la carte (vide = pas de titre).")>
    <DefaultValue("")>
    Public Property Titre As String
        Get
            Return _titre
        End Get
        Set(value As String)
            _titre = If(value, String.Empty)
            PerformLayout()
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Couleur de la carte (combinée avec OpaciteFond).")>
    Public Property CouleurFond As Color
        Get
            Return _couleurFond
        End Get
        Set(value As Color)
            _couleurFond = value
            Invalidate(True)
        End Set
    End Property

    <Category("Althéa")>
    <Description("Opacité de la carte : 0 = invisible, 255 = opaque. Défaut 140 (translucide).")>
    <DefaultValue(140)>
    Public Property OpaciteFond As Integer
        Get
            Return _opaciteFond
        End Get
        Set(value As Integer)
            _opaciteFond = Math.Max(0, Math.Min(255, value))
            Invalidate(True)
        End Set
    End Property

    <Category("Althéa")>
    <Description("Couleur de la bordure fine (Transparent = pas de bordure).")>
    Public Property CouleurBordure As Color
        Get
            Return _couleurBordure
        End Get
        Set(value As Color)
            _couleurBordure = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Affiche une ombre douce sous la carte.")>
    <DefaultValue(True)>
    Public Property Ombre As Boolean
        Get
            Return _ombre
        End Get
        Set(value As Boolean)
            _ombre = value
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Police du titre de section.")>
    Public Property PoliceTitre As Font
        Get
            Return _policeTitre
        End Get
        Set(value As Font)
            If value IsNot Nothing Then
                _policeTitre = value
                PerformLayout()
                Invalidate()
            End If
        End Set
    End Property

    <Category("Althéa")>
    <Description("Couleur du titre de section (Mauve foncé par défaut).")>
    Public Property CouleurTitre As Color
        Get
            Return _couleurTitre
        End Get
        Set(value As Color)
            _couleurTitre = value
            Invalidate()
        End Set
    End Property

    ' Toujours transparent : la carte se règle avec CouleurFond / OpaciteFond.
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

    ' Zone des enfants : sous le titre lorsqu'il y en a un.
    Public Overrides ReadOnly Property DisplayRectangle As Rectangle
        Get
            Dim r As Rectangle = MyBase.DisplayRectangle
            Dim h As Integer = HauteurTitre()
            If h > 0 Then
                r = New Rectangle(r.X, r.Y + h, r.Width, Math.Max(0, r.Height - h))
            End If
            Return r
        End Get
    End Property

#End Region

#Region "Sérialisation Designer (couleurs et police)"

    Private Function ShouldSerializeCouleurFond() As Boolean
        Return _couleurFond <> Color.White
    End Function

    Private Sub ResetCouleurFond()
        CouleurFond = Color.White
    End Sub

    Private Function ShouldSerializeCouleurBordure() As Boolean
        Return _couleurBordure <> UITheme.ColorBeigeFonce
    End Function

    Private Sub ResetCouleurBordure()
        CouleurBordure = UITheme.ColorBeigeFonce
    End Sub

    Private Function ShouldSerializeCouleurTitre() As Boolean
        Return _couleurTitre <> UITheme.ColorMauveFonce
    End Function

    Private Sub ResetCouleurTitre()
        CouleurTitre = UITheme.ColorMauveFonce
    End Sub

    Private Function ShouldSerializePoliceTitre() As Boolean
        Return Not (_policeTitre.Name = "Calibri" AndAlso _policeTitre.Size = 12.0F AndAlso _policeTitre.Style = FontStyle.Bold)
    End Function

    Private Sub ResetPoliceTitre()
        PoliceTitre = New Font("Calibri", 12.0F, FontStyle.Bold)
    End Sub

#End Region

#Region "Dessin"

    ' Rectangle de la carte : marges réservées à l'ombre.
    Protected Function RectangleCarte() As RectangleF

        If _ombre Then
            Return New RectangleF(3.0F, 1.0F, Width - 6.0F, Height - 8.0F)
        End If

        Return New RectangleF(0.5F, 0.5F, Width - 1.0F, Height - 1.0F)

    End Function

    Protected Function HauteurTitre() As Integer

        If String.IsNullOrEmpty(_titre) Then Return 0

        Return TextRenderer.MeasureText("Ag", _policeTitre).Height + 10

    End Function

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)

        ' Fond du parent (transparence), puis la carte par-dessus.
        MyBase.OnPaintBackground(e)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.PixelOffsetMode = PixelOffsetMode.HighQuality

        Dim rect As RectangleF = RectangleCarte()
        If rect.Width <= 0 OrElse rect.Height <= 0 Then Return

        If _ombre Then
            AltheaDessin.DessinerOmbre(g, rect, _rayon, Color.FromArgb(40, UITheme.ColorSaugeTresFonce), 4, 3.0F)
        End If

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rect, _rayon)

            If _opaciteFond > 0 Then
                Using pinceau As New SolidBrush(AltheaDessin.AvecAlpha(_couleurFond, _opaciteFond))
                    g.FillPath(pinceau, chemin)
                End Using
            End If

            If _couleurBordure.A > 0 Then
                Using stylo As New Pen(AltheaDessin.AvecAlpha(_couleurBordure, 215), 1.0F)
                    g.DrawPath(stylo, chemin)
                End Using
            End If

            DessinerDecor(g, rect, chemin)

        End Using

        If Not String.IsNullOrEmpty(_titre) Then
            Dim zoneTitre As New Rectangle(Padding.Left, Padding.Top, Math.Max(0, Width - Padding.Horizontal), HauteurTitre() - 10)
            AltheaDessin.DessinerTitreSection(g, _titre, _policeTitre, _couleurTitre, UITheme.ColorBeigeFonce, zoneTitre)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerDecor
    ' Rôle       : Point d'extension pour les classes dérivées (ex. vague d'AltheaBarreActions),
    '              appelé après le fond et la bordure, avec le contour de la carte.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub DessinerDecor(g As Graphics, rect As RectangleF, contour As GraphicsPath)
    End Sub

#End Region

End Class
