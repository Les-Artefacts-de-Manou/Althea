' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaBarreActions
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 09/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Barre d'actions du kit Althéa (bas des écrans et des fenêtres) : carte beige translucide
' traversée par la vague fine de la charte, dessinée par le code.
'
' Responsabilités :
' - Reprendre l'apparence d'AltheaCarte (fond, bordure, ombre)
' - Dessiner la vague sauge clair à travers toute la largeur, nette à toutes les tailles
'
' Remarques  :
' - Remplace l'image Althea_Bandeau_Haut_Trans.png étirée.
' - Les boutons se posent dedans comme aujourd'hui (FlowLayoutPanel, positions libres…).
'
' Dépendances :
' - AltheaCarte, UITheme
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Barre d'actions Althéa : carte beige translucide avec la vague de la charte.")>
Public Class AltheaBarreActions
    Inherits AltheaCarte

    Private _vague As Boolean = True

    Public Sub New()

        MyBase.New()

        CouleurFond = UITheme.ColorBeige
        OpaciteFond = 190
        Rayon = 12
        Padding = New Padding(16, 8, 16, 12)
        Size = New Size(900, 64)

    End Sub

    <Category("Althéa")>
    <Description("Affiche la vague fine à travers la barre.")>
    <DefaultValue(True)>
    Public Property Vague As Boolean
        Get
            Return _vague
        End Get
        Set(value As Boolean)
            _vague = value
            Invalidate()
        End Set
    End Property

    Protected Overrides Sub DessinerDecor(g As Graphics, rect As RectangleF, contour As GraphicsPath)

        If Not _vague OrElse rect.Width < 40 Then Return

        Dim w As Single = rect.Width
        Dim h As Single = rect.Height
        Dim x0 As Single = rect.X
        Dim y0 As Single = rect.Y

        ' Deux courbes de Bézier enchaînées : creux à gauche, bosse au centre, remontée à droite.
        Dim points As PointF() = {
            New PointF(x0, y0 + h * 0.66F),
            New PointF(x0 + w * 0.17F, y0 + h * 0.5F),
            New PointF(x0 + w * 0.32F, y0 + h * 0.86F),
            New PointF(x0 + w * 0.5F, y0 + h * 0.62F),
            New PointF(x0 + w * 0.66F, y0 + h * 0.4F),
            New PointF(x0 + w * 0.83F, y0 + h * 0.36F),
            New PointF(x0 + w, y0 + h * 0.55F)
        }

        Dim etatClip As Region = g.Clip
        g.SetClip(contour, CombineMode.Intersect)

        Using stylo As New Pen(UITheme.ColorSaugeClair, 1.4F)
            g.DrawBeziers(stylo, points)
        End Using

        g.Clip = etatClip
        etatClip.Dispose()

    End Sub

End Class
