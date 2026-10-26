' -------------------------------------------------------------------------------------------------
' Classe     : AltheaRenduBarre
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 19/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Rendu du kit Althéa pour les ToolStrip, leurs menus déroulants et les menus contextuels : même
' allure que les barres locales (AltheaBarreOutils) et les boutons Outil d'AltheaBouton.
'
' Responsabilités :
' - Fond de la barre (sa BackColor) et trait fin Beige sous la barre
' - Boutons : transparents, survol vert-de-gris pâle, enfoncé ou coché sauge clair, coins arrondis
' - Image d'un élément désactivé à 40 % d'opacité (au lieu du gris automatique de Windows)
' - Séparateurs fins, flèches des listes déroulantes en sauge très foncé
' - Menus : fond blanc, bord beige foncé, ligne survolée vert-de-gris pâle, coche sauge
' - Libellé dont la couleur de fond est fixée : dessiné en pastille (ex. « Lecture seule »)
'
' Remarques  :
' - Créé pour l'éditeur de texte riche (refonte du 19/10/2026) ; utilisable par toute ToolStrip ou
'   ContextMenuStrip : Renderer = New AltheaRenduBarre().
' - Hérite de ToolStripProfessionalRenderer : ce qui n'est pas redessiné ici (débordement, liste
'   intégrée) garde le rendu standard.
'
' Dépendances :
' - UITheme, AltheaDessin
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing.Drawing2D

Public Class AltheaRenduBarre
    Inherits ToolStripProfessionalRenderer

#Region "Constantes"

    Private Const RayonBouton As Single = 6.0F
    Private Const OpaciteDesactive As Single = 0.4F
    Private Const HauteurSeparateur As Integer = 18

#End Region

#Region "Barre"

    Protected Overrides Sub OnRenderToolStripBackground(e As ToolStripRenderEventArgs)

        If TypeOf e.ToolStrip Is ToolStripDropDown Then
            Using pinceau As New SolidBrush(UITheme.ColorBlanc)
                e.Graphics.FillRectangle(pinceau, e.AffectedBounds)
            End Using
            Return
        End If

        Using pinceau As New SolidBrush(e.BackColor)
            e.Graphics.FillRectangle(pinceau, e.AffectedBounds)
        End Using

    End Sub

    Protected Overrides Sub OnRenderToolStripBorder(e As ToolStripRenderEventArgs)

        If TypeOf e.ToolStrip Is ToolStripDropDown Then
            Using stylo As New Pen(UITheme.ColorBeigeFonce)
                e.Graphics.DrawRectangle(stylo, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1)
            End Using
            Return
        End If

        Using stylo As New Pen(UITheme.ColorBeige)
            Dim y As Integer = e.ToolStrip.Height - 1
            e.Graphics.DrawLine(stylo, 0, y, e.ToolStrip.Width, y)
        End Using

    End Sub

    Protected Overrides Sub OnRenderSeparator(e As ToolStripSeparatorRenderEventArgs)

        Using stylo As New Pen(UITheme.ColorBeigeFonce)

            If e.Vertical Then
                Dim x As Integer = e.Item.Width \ 2
                Dim haut As Integer = Math.Max(0, (e.Item.Height - HauteurSeparateur) \ 2)
                e.Graphics.DrawLine(stylo, x, haut, x, haut + Math.Min(HauteurSeparateur, e.Item.Height))
            Else
                Dim y As Integer = e.Item.Height \ 2
                e.Graphics.DrawLine(stylo, 8, y, e.Item.Width - 8, y)
            End If

        End Using

    End Sub

#End Region

#Region "Boutons"

    Protected Overrides Sub OnRenderButtonBackground(e As ToolStripItemRenderEventArgs)

        Dim bouton As ToolStripButton = TryCast(e.Item, ToolStripButton)
        DessinerFondBouton(e.Graphics, e.Item, bouton IsNot Nothing AndAlso bouton.Checked)

    End Sub

    Protected Overrides Sub OnRenderDropDownButtonBackground(e As ToolStripItemRenderEventArgs)
        DessinerFondBouton(e.Graphics, e.Item, False)
    End Sub

    Protected Overrides Sub OnRenderSplitButtonBackground(e As ToolStripItemRenderEventArgs)

        DessinerFondBouton(e.Graphics, e.Item, False)

        Dim bouton As ToolStripSplitButton = TryCast(e.Item, ToolStripSplitButton)
        If bouton IsNot Nothing Then
            DessinerFleche(e.Graphics, bouton.DropDownButtonBounds, bouton.Enabled)
        End If

    End Sub

    ' Fond d'un bouton de barre : rien au repos, vert-de-gris pâle au survol, sauge clair enfoncé ou
    ' coché (mêmes teintes qu'AltheaBouton, variante Outil).
    Private Sub DessinerFondBouton(g As Graphics, element As ToolStripItem, coche As Boolean)

        If Not element.Enabled Then Return

        Dim fond As Color

        If element.Pressed OrElse coche Then
            fond = UITheme.ColorSaugeClair
        ElseIf element.Selected Then
            fond = UITheme.ColorGrisVertClair
        Else
            Return
        End If

        Dim mode As SmoothingMode = g.SmoothingMode
        g.SmoothingMode = SmoothingMode.AntiAlias

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(New RectangleF(1.0F, 1.0F, element.Width - 2.0F, element.Height - 2.0F), RayonBouton)
            Using pinceau As New SolidBrush(fond)
                g.FillPath(pinceau, chemin)
            End Using
        End Using

        g.SmoothingMode = mode

    End Sub

    Protected Overrides Sub OnRenderItemImage(e As ToolStripItemImageRenderEventArgs)

        If e.Image Is Nothing Then Return

        Dim mode As InterpolationMode = e.Graphics.InterpolationMode
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
        AltheaDessin.DessinerImage(e.Graphics, e.Image, e.ImageRectangle, If(e.Item.Enabled, 1.0F, OpaciteDesactive))
        e.Graphics.InterpolationMode = mode

    End Sub

    Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)

        If Not e.Item.Enabled Then
            e.TextColor = AltheaDessin.CouleurDesactiveTexte
        ElseIf TypeOf e.Item Is ToolStripMenuItem AndAlso e.Item.ForeColor.ToArgb() = SystemColors.ControlText.ToArgb() Then
            ' Ligne de menu sans couleur propre : gris foncé du texte courant.
            e.TextColor = UITheme.ColorGrisFonce
        End If

        MyBase.OnRenderItemText(e)

    End Sub

    Protected Overrides Sub OnRenderArrow(e As ToolStripArrowRenderEventArgs)

        If e.Item IsNot Nothing AndAlso TypeOf e.Item.Owner Is ToolStripDropDownMenu Then
            ' Flèche d'un sous-menu : rendu standard, à la couleur du texte.
            e.ArrowColor = UITheme.ColorSaugeTresFonce
            MyBase.OnRenderArrow(e)
            Return
        End If

        DessinerFleche(e.Graphics, e.ArrowRectangle, e.Item Is Nothing OrElse e.Item.Enabled)

    End Sub

    ' Petit chevron vers le bas, centré dans la zone donnée.
    Private Sub DessinerFleche(g As Graphics, zone As Rectangle, actif As Boolean)

        Dim couleur As Color = If(actif, UITheme.ColorSaugeTresFonce, AltheaDessin.CouleurDesactiveTexte)
        Dim cx As Single = zone.X + zone.Width / 2.0F
        Dim cy As Single = zone.Y + zone.Height / 2.0F

        Dim mode As SmoothingMode = g.SmoothingMode
        g.SmoothingMode = SmoothingMode.AntiAlias

        Using stylo As New Pen(couleur, 1.6F)
            stylo.StartCap = LineCap.Round
            stylo.EndCap = LineCap.Round
            stylo.LineJoin = LineJoin.Round
            g.DrawLines(stylo, {New PointF(cx - 3.0F, cy - 1.5F), New PointF(cx, cy + 1.5F), New PointF(cx + 3.0F, cy - 1.5F)})
        End Using

        g.SmoothingMode = mode

    End Sub

    ' Libellé dont la couleur de fond est fixée : pastille arrondie (ex. « Lecture seule »).
    Protected Overrides Sub OnRenderLabelBackground(e As ToolStripItemRenderEventArgs)

        Dim parent As ToolStrip = e.Item.Owner
        If parent Is Nothing OrElse e.Item.BackColor = parent.BackColor OrElse e.Item.BackColor.A = 0 Then Return

        Dim mode As SmoothingMode = e.Graphics.SmoothingMode
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Dim hauteur As Single = Math.Min(22.0F, e.Item.Height - 4.0F)
        Dim zone As New RectangleF(1.0F, (e.Item.Height - hauteur) / 2.0F, e.Item.Width - 2.0F, hauteur)

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(zone, hauteur / 2.0F)
            Using pinceau As New SolidBrush(e.Item.BackColor)
                e.Graphics.FillPath(pinceau, chemin)
            End Using
        End Using

        e.Graphics.SmoothingMode = mode

    End Sub

#End Region

#Region "Menus"

    Protected Overrides Sub OnRenderMenuItemBackground(e As ToolStripItemRenderEventArgs)

        If Not e.Item.Selected OrElse Not e.Item.Enabled Then Return

        Dim mode As SmoothingMode = e.Graphics.SmoothingMode
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(New RectangleF(3.0F, 1.0F, e.Item.Width - 6.0F, e.Item.Height - 2.0F), RayonBouton)
            Using pinceau As New SolidBrush(UITheme.ColorGrisVertClair)
                e.Graphics.FillPath(pinceau, chemin)
            End Using
        End Using

        e.Graphics.SmoothingMode = mode

    End Sub

    Protected Overrides Sub OnRenderImageMargin(e As ToolStripRenderEventArgs)
        ' Marge des images du menu : même fond blanc que le menu (pas de bande grise).
    End Sub

    Protected Overrides Sub OnRenderItemCheck(e As ToolStripItemImageRenderEventArgs)

        Dim zone As Rectangle = e.ImageRectangle
        Dim mode As SmoothingMode = e.Graphics.SmoothingMode
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Using stylo As New Pen(UITheme.ColorSaugeTresFonce, 1.8F)
            stylo.StartCap = LineCap.Round
            stylo.EndCap = LineCap.Round
            stylo.LineJoin = LineJoin.Round
            Dim cx As Single = zone.X + zone.Width / 2.0F
            Dim cy As Single = zone.Y + zone.Height / 2.0F
            e.Graphics.DrawLines(stylo, {New PointF(cx - 4.0F, cy), New PointF(cx - 1.0F, cy + 3.0F), New PointF(cx + 4.5F, cy - 3.5F)})
        End Using

        e.Graphics.SmoothingMode = mode

    End Sub

#End Region

End Class
