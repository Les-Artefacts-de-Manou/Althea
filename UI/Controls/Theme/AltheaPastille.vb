' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaPastille
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 11/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Pastille du kit Althéa : court libellé sur fond arrondi (statut, code, catégorie, nature de
' séance…), dessinée en code pour rester nette à toutes les tailles et sur tous les fonds.
'
' Remarques  :
' - Hérite de Label : AutoSize, Text, Font, ForeColor et Padding gardent leur sens habituel ;
'   la taille automatique inclut le Padding, qui sert de marge intérieure de la pastille.
' - BackColor reste transparent : la couleur de la pastille est CouleurFond, le coin extérieur
'   laisse voir le parent (AltheaFondParent).
' - CouleurBordure = Color.Empty : pas de contour.
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
<Description("Pastille Althéa : libellé court sur fond arrondi.")>
Public Class AltheaPastille
    Inherits Label

#Region "Variables privées"

    Private _couleurFond As Color = UITheme.ColorGrisVertClair
    Private _couleurBordure As Color = Color.Empty

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        BackColor = Color.Transparent
        AutoSize = True
        Padding = New Padding(8, 2, 8, 2)
        ForeColor = UITheme.ColorSaugeTresFonce

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Couleur de fond de la pastille.")>
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
    <Description("Couleur du contour (Empty = pas de contour).")>
    Public Property CouleurBordure As Color
        Get
            Return _couleurBordure
        End Get
        Set(value As Color)
            _couleurBordure = value
            Invalidate()
        End Set
    End Property

#End Region

#Region "Sérialisation Designer"

    Private Function ShouldSerializeCouleurFond() As Boolean
        Return _couleurFond <> UITheme.ColorGrisVertClair
    End Function

    Private Sub ResetCouleurFond()
        CouleurFond = UITheme.ColorGrisVertClair
    End Sub

    Private Function ShouldSerializeCouleurBordure() As Boolean
        Return Not _couleurBordure.IsEmpty
    End Function

    Private Sub ResetCouleurBordure()
        CouleurBordure = Color.Empty
    End Sub

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        AltheaFondParent.Peindre(Me, e)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim cadre As New RectangleF(0, 0, Width - 1, Height - 1)
        Dim rayon As Single = Math.Max(1.0F, Math.Min(cadre.Height / 2.0F, 12.0F))

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(cadre, rayon)
            Using pinceau As New SolidBrush(_couleurFond)
                g.FillPath(pinceau, chemin)
            End Using
            If Not _couleurBordure.IsEmpty Then
                Using stylo As New Pen(_couleurBordure)
                    g.DrawPath(stylo, chemin)
                End Using
            End If
        End Using

        Dim zoneTexte As Rectangle = Rectangle.FromLTRB(Padding.Left, Padding.Top,
                                                        Width - Padding.Right, Height - Padding.Bottom)
        TextRenderer.DrawText(g, Text, Font, zoneTexte, If(Enabled, ForeColor, UITheme.ColorGrisMoyen),
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or
                              TextFormatFlags.SingleLine Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)

    End Sub

#End Region

End Class
