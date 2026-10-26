' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaTuile
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 09/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Tuile du kit Althéa (hubs Référentiels, Administration…) : grand bouton arrondi avec l'icône
' dans une pastille claire, un titre en gras et un sous-titre facultatif.
'
' Responsabilités :
' - Dessiner la tuile (ombre douce, fond arrondi selon la Variante et l'état)
' - Dessiner l'icône 48 px (propriété Image) dans une pastille
' - Dessiner le titre et le sous-titre à partir de la propriété Text
'
' Remarques  :
' - Text sur deux lignes = titre + sous-titre, exactement comme les tuiles actuelles :
'   "Domaines" & vbCrLf & "Prises en charge". Les lignes vides (vbCr final) sont ignorées.
' - Hérite de AltheaBouton : Variante, Rayon, états et focus identiques.
' - L'icône vient de la propriété Image (icônes Boutons_ico_48, _hover/_disabled via UtilsButtons
'   si utilisé). Désactivée et sans icône _disabled, l'icône est simplement estompée.
'
' Dépendances :
' - AltheaBouton, AltheaDessin, UITheme
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Tuile Althéa : icône en pastille, titre et sous-titre (Text sur deux lignes).")>
Public Class AltheaTuile
    Inherits AltheaBouton

#Region "Variables privées"

    ' Police du sous-titre, dérivée de Font (recréée quand Font change).
    Private _policeSousTitre As Font

#End Region

#Region "Initialisation"

    Public Sub New()

        MyBase.New()

        Rayon = 12
        Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        Size = New Size(214, 84)
        TextAlign = ContentAlignment.MiddleLeft

    End Sub

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaint(pevent As PaintEventArgs)

        Dim g As Graphics = pevent.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.InterpolationMode = InterpolationMode.HighQualityBicubic

        Dim fond As Color, bordure As Color, texte As Color
        CalculerCouleurs(fond, bordure, texte)

        ' Marges réservées à l'ombre douce.
        Dim rect As New RectangleF(2.0F, 1.0F, Width - 4.0F, Height - 7.0F)

        If Enabled AndAlso Variante <> VarianteBouton.Clair Then
            Dim intensite As Integer = If(EstSurvole AndAlso Not EstEnfonce, 70, 50)
            AltheaDessin.DessinerOmbre(g, rect, Rayon,
                                       Color.FromArgb(intensite, UITheme.ColorSaugeTresFonce),
                                       3, If(EstEnfonce, 1.0F, 3.0F))
        End If

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rect, Rayon)

            If fond.A > 0 Then
                Using pinceau As New SolidBrush(fond)
                    g.FillPath(pinceau, chemin)
                End Using
            End If

            If bordure.A > 0 Then
                Using stylo As New Pen(bordure, 1.0F)
                    g.DrawPath(stylo, chemin)
                End Using
            End If

        End Using

        Dim zone As Rectangle = Rectangle.Round(rect)

        ' Pastille + icône.
        Dim xTexte As Integer = zone.X + 16

        If Image IsNot Nothing Then

            Dim diametre As Integer = Math.Min(56, Math.Max(0, zone.Height - 20))
            Dim rPastille As New Rectangle(zone.X + 14, zone.Y + (zone.Height - diametre) \ 2, diametre, diametre)

            Dim alphaPastille As Integer = If(Enabled, 56, 150)
            Dim couleurPastille As Color = If(Variante = VarianteBouton.Clair,
                                              UITheme.ColorGrisVertClair,
                                              AltheaDessin.AvecAlpha(Color.White, alphaPastille))

            Using pinceau As New SolidBrush(couleurPastille)
                g.FillEllipse(pinceau, rPastille)
            End Using

            Dim tailleIcone As Integer = Math.Min(44, Math.Max(0, diametre - 12))
            Dim rIcone As New Rectangle(rPastille.X + (diametre - tailleIcone) \ 2,
                                        rPastille.Y + (diametre - tailleIcone) \ 2,
                                        tailleIcone, tailleIcone)

            AltheaDessin.DessinerImage(g, Image, rIcone, If(Enabled, 1.0F, 0.4F))

            xTexte = rPastille.Right + 14

        End If

        DessinerTextes(g, New Rectangle(xTexte, zone.Y + 6, Math.Max(0, zone.Right - 16 - xTexte), zone.Height - 12), fond, texte)

        If Focused AndAlso ShowFocusCues Then
            Dim rFocus As RectangleF = RectangleF.Inflate(rect, -4.0F, -4.0F)
            Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rFocus, Math.Max(0, Rayon - 4))
                Using stylo As New Pen(If(Enabled AndAlso Variante <> VarianteBouton.Clair, Color.FromArgb(170, Color.White), UITheme.ColorSaugeFonce), 1.0F)
                    stylo.DashStyle = DashStyle.Dot
                    g.DrawPath(stylo, chemin)
                End Using
            End Using
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerTextes
    ' Rôle       : Titre (gras, Font) + sous-titre (plus fin), centrés verticalement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerTextes(g As Graphics, zone As Rectangle, fond As Color, couleurTexte As Color)

        If zone.Width <= 0 OrElse zone.Height <= 0 Then Return

        Dim lignes As New List(Of String)
        For Each morceau As String In If(Text, String.Empty).Split({vbCrLf, vbCr, vbLf}, StringSplitOptions.None)
            If Not String.IsNullOrWhiteSpace(morceau) Then lignes.Add(morceau.Trim())
        Next

        If lignes.Count = 0 Then Return

        Dim titre As String = lignes(0)
        Dim sousTitre As String = If(lignes.Count > 1, String.Join(" ", lignes.Skip(1)), String.Empty)

        Dim prefixe As TextFormatFlags = If(ShowKeyboardCues, TextFormatFlags.Default, TextFormatFlags.HidePrefix)
        Dim flagsTitre As TextFormatFlags = TextFormatFlags.Left Or TextFormatFlags.WordBreak Or
                                            TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding Or prefixe
        Dim flagsSous As TextFormatFlags = TextFormatFlags.Left Or TextFormatFlags.SingleLine Or
                                           TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding Or prefixe

        Dim policeSous As Font = PoliceSousTitre()

        Dim hSous As Integer = If(sousTitre.Length > 0, TextRenderer.MeasureText(g, sousTitre, policeSous, zone.Size, flagsSous).Height, 0)
        Dim ecart As Integer = If(hSous > 0, 2, 0)
        Dim hTitreMax As Integer = Math.Max(0, zone.Height - hSous - ecart)
        Dim hTitre As Integer = Math.Min(hTitreMax, TextRenderer.MeasureText(g, titre, Font, New Size(zone.Width, hTitreMax), flagsTitre).Height)

        Dim y As Integer = zone.Y + (zone.Height - (hTitre + ecart + hSous)) \ 2

        TextRenderer.DrawText(g, titre, Font, New Rectangle(zone.X, y, zone.Width, hTitre), couleurTexte, flagsTitre)

        If hSous > 0 Then
            ' Sous-titre légèrement fondu dans la couleur de la tuile.
            Dim couleurSous As Color = If(fond.A > 0 AndAlso Enabled, AltheaDessin.Melanger(couleurTexte, fond, 0.12F), couleurTexte)
            TextRenderer.DrawText(g, sousTitre, policeSous, New Rectangle(zone.X, y + hTitre + ecart, zone.Width, hSous), couleurSous, flagsSous)
        End If

    End Sub

    Private Function PoliceSousTitre() As Font

        If _policeSousTitre Is Nothing Then
            _policeSousTitre = New Font(Font.FontFamily, Math.Max(7.0F, Font.Size - 1.5F), FontStyle.Regular)
        End If

        Return _policeSousTitre

    End Function

    Protected Overrides Sub OnFontChanged(e As EventArgs)

        If _policeSousTitre IsNot Nothing Then
            _policeSousTitre.Dispose()
            _policeSousTitre = Nothing
        End If

        MyBase.OnFontChanged(e)

    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)

        If disposing AndAlso _policeSousTitre IsNot Nothing Then
            _policeSousTitre.Dispose()
            _policeSousTitre = Nothing
        End If

        MyBase.Dispose(disposing)

    End Sub

#End Region

End Class
