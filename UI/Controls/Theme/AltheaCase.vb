' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaCase
' Projet     : Althéa
' Version    : V1.1.0
' Date       : 14/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Case à cocher du kit Althéa : case arrondie, cochée en Sauge foncé, dessinée par le code.
'
' Responsabilités :
' - Dessiner la case (normale, survolée, cochée, indéterminée, désactivée) et son libellé
' - Afficher le repère de focus clavier autour de la case
'
' Remarques  :
' - Hérite de CheckBox : Checked, CheckState, CheckedChanged et le code existant restent
'   inchangés. La migration d'un écran se limite au type déclaré.
' - Fond transparent : la case se pose sur une carte ou sur le fond de l'écran.
' - Dans un bandeau, préférer AutoSize = False et une largeur fixe : avec un ancrage à droite,
'   l'ajustement automatique de la largeur décale la case vers ses voisins.
' - Désactivée (V1.1.0, 14/10/2026) : dessinée comme un champ en lecture seule — libellé dans sa
'   couleur normale, case beige (ou sauge si cochée). Un champ consulté doit rester lisible.
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
<Description("Case à cocher Althéa : case arrondie cochée en sauge.")>
Public Class AltheaCase
    Inherits CheckBox

#Region "Constantes"

    Private Const TailleCase As Integer = 18
    Private Const RayonCase As Single = 5.0F
    Private Const EcartCaseTexte As Integer = 8

#End Region

#Region "Variables privées"

    Private _survolee As Boolean

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        ' ButtonBase se déclare « Opaque » : sans fond peint, le tampon partagé garderait l'image du
        ' dernier contrôle dessiné (texte fantôme sous la case). Même correction qu'AltheaBouton.
        SetStyle(ControlStyles.Opaque, False)

        BackColor = Color.Transparent
        ForeColor = UITheme.ColorGrisFonce
        Font = New Font("Calibri", 11.0F)

    End Sub

#End Region

#Region "Taille"

    ' Taille calculée sur la case dessinée, pas sur le glyphe Windows.
    Public Overrides Function GetPreferredSize(proposedSize As Size) As Size

        Dim texte As Size = TextRenderer.MeasureText(Text, Font)
        Return New Size(Padding.Horizontal + TailleCase + If(Text.Length > 0, EcartCaseTexte + texte.Width, 0),
                        Padding.Vertical + Math.Max(TailleCase + 2, texte.Height + 2))

    End Function

#End Region

#Region "Dessin"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnPaint
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Case arrondie centrée verticalement, puis libellé à sa droite.
    '
    ' Remarques  :
    ' - Le fond transparent est déjà peint par OnPaintBackground.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim rectCase As New RectangleF(Padding.Left + 0.5F,
                                       (Height - TailleCase) / 2.0F + 0.5F,
                                       TailleCase - 1, TailleCase - 1)

        Dim cochee As Boolean = CheckState <> CheckState.Unchecked

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rectCase, RayonCase)

            Dim fond As Color
            Dim bordure As Color

            If Not Enabled Then
                ' Lecture seule : la case reste lisible (cochée = sauge, sinon fond beige).
                fond = If(cochee, UITheme.ColorSauge, UITheme.ColorChampLectureSeule)
                bordure = If(cochee, UITheme.ColorSauge, UITheme.ColorBeigeFonce)
            ElseIf cochee Then
                fond = If(_survolee, UITheme.ColorSaugeTresFonce, UITheme.ColorSaugeFonce)
                bordure = fond
            Else
                fond = Color.White
                bordure = If(_survolee, UITheme.ColorSauge, UITheme.ColorSaugeClair)
            End If

            Using pinceau As New SolidBrush(fond)
                g.FillPath(pinceau, chemin)
            End Using

            Using stylo As New Pen(bordure, 1.5F)
                g.DrawPath(stylo, chemin)
            End Using

        End Using

        If cochee Then
            DessinerMarque(g, rectCase)
        End If

        If Focused AndAlso ShowFocusCues Then
            Dim rectFocus As RectangleF = RectangleF.Inflate(rectCase, 3, 3)
            Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rectFocus, RayonCase + 3)
                Using stylo As New Pen(AltheaDessin.AvecAlpha(UITheme.ColorSauge, 140), 2.0F)
                    g.DrawPath(stylo, chemin)
                End Using
            End Using
        End If

        If Text.Length > 0 Then
            Dim xTexte As Integer = Padding.Left + TailleCase + EcartCaseTexte
            Dim zoneTexte As New Rectangle(xTexte, 0, Math.Max(0, Width - xTexte - Padding.Right), Height)
            TextRenderer.DrawText(g, Text, Font, zoneTexte, ForeColor,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or
                                  TextFormatFlags.SingleLine Or TextFormatFlags.EndEllipsis)
        End If

    End Sub

    ' Coche blanche (état coché) ou trait horizontal (état indéterminé).
    Private Sub DessinerMarque(g As Graphics, rectCase As RectangleF)

        Dim couleur As Color = Color.White

        Using stylo As New Pen(couleur, 2.2F)
            stylo.StartCap = LineCap.Round
            stylo.EndCap = LineCap.Round
            stylo.LineJoin = LineJoin.Round

            If CheckState = CheckState.Indeterminate Then
                Dim y As Single = rectCase.Top + rectCase.Height / 2.0F
                g.DrawLine(stylo, rectCase.Left + 4.5F, y, rectCase.Right - 4.5F, y)
            Else
                g.DrawLines(stylo, {
                    New PointF(rectCase.Left + 4.0F, rectCase.Top + rectCase.Height * 0.52F),
                    New PointF(rectCase.Left + rectCase.Width * 0.42F, rectCase.Bottom - 4.5F),
                    New PointF(rectCase.Right - 3.8F, rectCase.Top + 4.5F)
                })
            End If
        End Using

    End Sub

#End Region

#Region "Événements"

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        _survolee = True
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        _survolee = False
        Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

#End Region

End Class
