' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaBoutonMenu
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 11/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Bouton du menu principal de la Home (maquette « Home A ») : pastille arrondie dont la couleur
' suit l'état du bouton — rien au repos, Gris-vert clair au survol, Sauge foncé pour l'écran actif.
'
' Remarques  :
' - Les états restent gérés par UtilsButtons (InitHomeMenuButton / SetSelectedHomeMenuButton), qui
'   fixe BackColor, ForeColor et l'icône (Tag) : ce contrôle se contente de les dessiner arrondis.
'   La pastille n'est donc peinte que si BackColor diffère du fond du menu (le parent).
' - Un bouton non géré par UtilsButtons (ex. Changer mon mot de passe) reçoit le survol lui-même.
' - Texte sur deux lignes au besoin (« Changer mon mot de passe »).
' - ButtonBase se déclare « Opaque » : désactivé pour laisser voir le parent (cf. AltheaBouton).
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
<Description("Bouton du menu principal Althéa : pastille arrondie selon l'état.")>
Public Class AltheaBoutonMenu
    Inherits Button

#Region "Variables privées"

    Private _rayon As Integer = 12
    Private _tailleIcone As Integer = 32
    Private _survol As Boolean

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw, True)
        SetStyle(ControlStyles.Opaque, False)

        FlatStyle = FlatStyle.Flat
        FlatAppearance.BorderSize = 0
        UseVisualStyleBackColor = False
        Cursor = Cursors.Hand

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Rayon des coins de la pastille, en pixels.")>
    <DefaultValue(12)>
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
    <Description("Taille d'affichage de l'icône (les icônes de la Home font 48 px : elles sont réduites).")>
    <DefaultValue(32)>
    Public Property TailleIcone As Integer
        Get
            Return _tailleIcone
        End Get
        Set(value As Integer)
            _tailleIcone = Math.Max(8, value)
            Invalidate()
        End Set
    End Property

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        AltheaFondParent.Peindre(Me, pevent)
    End Sub

    Protected Overrides Sub OnBackColorChanged(e As EventArgs)
        MyBase.OnBackColorChanged(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(pevent As PaintEventArgs)

        Dim g As Graphics = pevent.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.InterpolationMode = InterpolationMode.HighQualityBicubic

        Dim fondMenu As Color = If(Parent IsNot Nothing, Parent.BackColor, Color.Empty)
        Dim teinte As Color = Color.Empty
        If BackColor <> fondMenu AndAlso BackColor.A > 0 Then
            teinte = BackColor
        ElseIf _survol AndAlso Enabled Then
            teinte = UITheme.ButtonHomeBackHover
        End If

        Dim cadre As New RectangleF(0.5F, 1.5F, Width - 1.0F, Height - 3.0F)

        If Not teinte.IsEmpty Then
            Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(cadre, _rayon)
                Using pinceau As New SolidBrush(teinte)
                    g.FillPath(pinceau, chemin)
                End Using
            End Using
        End If

        If Focused AndAlso ShowFocusCues Then
            Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(cadre, _rayon)
                Using stylo As New Pen(UITheme.ColorSaugeClair)
                    g.DrawPath(stylo, chemin)
                End Using
            End Using
        End If

        Dim x As Integer = Padding.Left + 8
        If Image IsNot Nothing Then
            Dim y As Integer = (Height - _tailleIcone) \ 2
            AltheaDessin.DessinerImage(g, Image, New Rectangle(x, y, _tailleIcone, _tailleIcone), If(Enabled, 1.0F, 0.5F))
            x += _tailleIcone + 10
        End If

        ' Centrage vertical calculé : GDI ne centre verticalement qu'un texte sur une seule ligne.
        Const Flags As TextFormatFlags = TextFormatFlags.Left Or TextFormatFlags.WordBreak Or TextFormatFlags.NoPrefix
        Dim largeur As Integer = Math.Max(0, Width - x - 6)
        Dim hauteur As Integer = Math.Min(Height, TextRenderer.MeasureText(g, Text, Font, New Size(largeur, Integer.MaxValue), Flags).Height)
        TextRenderer.DrawText(g, Text, Font, New Rectangle(x, (Height - hauteur) \ 2, largeur, hauteur),
                              If(Enabled, ForeColor, UITheme.ButtonHomeForeDisabled), Flags)

    End Sub

#End Region

#Region "Survol"

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        _survol = True
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        _survol = False
        Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        Invalidate()
        MyBase.OnGotFocus(e)
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        Invalidate()
        MyBase.OnLostFocus(e)
    End Sub

#End Region

End Class
