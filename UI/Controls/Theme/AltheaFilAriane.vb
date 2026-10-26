' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaFilAriane
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 11/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Fil d'Ariane du bandeau de la Home (maquette « Home A ») : dessine le texte de contexte reçu
' (« Patients > Jean Dupont (P-0012) > Identité ») en segments séparés par un chevron, le dernier
' en gras, les précisions entre parenthèses ou crochets atténuées ; pastille facultative à la fin
' (domaine du dossier ouvert).
'
' Remarques  :
' - Hérite de Label : le texte reste une simple chaîne, alimentée comme avant par
'   UserControlContext.SetHeader / Home.SetContexte ; aucun écran n'a à changer sa façon d'écrire.
' - Séparateurs reconnus : « > » et « › » entourés d'espaces.
' - Texte trop long : les segments intermédiaires sont remplacés par « … », le premier et le
'   dernier restent visibles.
'
' Dépendances :
' - UITheme, AltheaDessin, AltheaFondParent
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Text.RegularExpressions

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Fil d'Ariane Althéa : segments, chevrons et pastille facultative.")>
Public Class AltheaFilAriane
    Inherits Label

#Region "Constantes"

    Private Const CHEVRON As String = "›"
    Private Const ELLIPSE As String = "…"
    Private Const ECART As Integer = 6
    Private Const FLAGS As TextFormatFlags =
        TextFormatFlags.NoPadding Or TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix Or TextFormatFlags.VerticalCenter

    Private Shared ReadOnly SeparateurSegments As New Regex("\s+[>›]\s+", RegexOptions.Compiled)

    ' Précision en fin de segment : « (…) » ou « […] », éventuellement plusieurs.
    Private Shared ReadOnly Precision As New Regex("\s*(\([^)]*\)|\[[^\]]*\])", RegexOptions.Compiled)

#End Region

#Region "Variables privées"

    Private _pastille As String = String.Empty
    Private _policeGras As Font

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        BackColor = Color.Transparent
        AutoSize = False
        ForeColor = UITheme.ColorGrisDiscret
        Font = New Font("Calibri", 11.0F)

    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then _policeGras?.Dispose()
        MyBase.Dispose(disposing)
    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Texte de la pastille affichée après le fil (vide = pas de pastille).")>
    <DefaultValue("")>
    Public Property Pastille As String
        Get
            Return _pastille
        End Get
        Set(value As String)
            _pastille = If(value, String.Empty).Trim()
            Invalidate()
        End Set
    End Property

#End Region

#Region "Dessin"

    Protected Overrides Sub OnFontChanged(e As EventArgs)
        _policeGras?.Dispose()
        _policeGras = Nothing
        MyBase.OnFontChanged(e)
    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        AltheaFondParent.Peindre(Me, e)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        If _policeGras Is Nothing Then _policeGras = New Font(Font, FontStyle.Bold)

        Dim segments As List(Of String) =
            SeparateurSegments.Split(If(Text, String.Empty)).Select(Function(t) t.Trim()).
            Where(Function(t) t.Length > 0).ToList()

        Dim largeurPastille As Integer = 0
        Dim policePastille As Font = Nothing
        If _pastille.Length > 0 Then
            policePastille = New Font(Font.FontFamily, Math.Max(8.0F, Font.Size - 1.25F), FontStyle.Bold)
            largeurPastille = TextRenderer.MeasureText(g, _pastille, policePastille, Size.Empty, FLAGS).Width + 20 + 2 * ECART
        End If

        Try
            Dim disponible As Integer = Width - Padding.Horizontal - largeurPastille
            Dim visibles As List(Of String) = Ajuster(g, segments, disponible)

            Dim x As Integer = Padding.Left
            For i As Integer = 0 To visibles.Count - 1
                Dim dernier As Boolean = i = visibles.Count - 1
                x = DessinerSegment(g, visibles(i), dernier, x)
                If Not dernier Then
                    x = DessinerTexte(g, CHEVRON, Font, ForeColor, x + ECART) + ECART
                End If
            Next

            If policePastille IsNot Nothing Then DessinerPastille(g, policePastille, x + 2 * ECART)
        Finally
            policePastille?.Dispose()
        End Try

    End Sub

    ' Remplace les segments intermédiaires par « … » tant que le fil dépasse la largeur disponible.
    Private Function Ajuster(g As Graphics, segments As List(Of String), disponible As Integer) As List(Of String)

        Dim visibles As New List(Of String)(segments)
        Do While visibles.Count > 2 AndAlso LargeurTotale(g, visibles) > disponible
            If visibles(1) = ELLIPSE Then
                If visibles.Count <= 3 Then Exit Do
                visibles.RemoveAt(2)
            Else
                visibles(1) = ELLIPSE
            End If
        Loop
        Return visibles

    End Function

    Private Function LargeurTotale(g As Graphics, segments As List(Of String)) As Integer

        ' VB ne distingue pas la casse : « chevron » masquerait la constante CHEVRON.
        Dim largeurChevron As Integer = TextRenderer.MeasureText(g, CHEVRON, Font, Size.Empty, FLAGS).Width + 2 * ECART
        Dim total As Integer = 0
        For i As Integer = 0 To segments.Count - 1
            Dim police As Font = If(i = segments.Count - 1, _policeGras, Font)
            total += TextRenderer.MeasureText(g, segments(i), police, Size.Empty, FLAGS).Width
            If i < segments.Count - 1 Then total += largeurChevron
        Next
        Return total

    End Function

    Private Function DessinerSegment(g As Graphics, segment As String, dernier As Boolean, x As Integer) As Integer

        ' Partie principale puis précisions « (…) » / « […] » atténuées.
        Dim m As Match = Precision.Match(segment)
        Dim principal As String = If(m.Success, segment.Substring(0, m.Index), segment)
        Dim precisions As String = If(m.Success, segment.Substring(m.Index), String.Empty)

        Dim police As Font = If(dernier, _policeGras, Font)
        Dim couleur As Color = If(dernier, UITheme.ColorGrisFonce, ForeColor)
        x = DessinerTexte(g, principal, police, couleur, x)
        If precisions.Length > 0 Then
            x = DessinerTexte(g, precisions, Font, UITheme.ColorGrisMoyenFonce, x)
        End If
        Return x

    End Function

    Private Function DessinerTexte(g As Graphics, texte As String, police As Font, couleur As Color, x As Integer) As Integer

        Dim largeur As Integer = TextRenderer.MeasureText(g, texte, police, Size.Empty, FLAGS).Width
        Dim restant As Integer = Math.Max(0, Width - Padding.Right - x)
        TextRenderer.DrawText(g, texte, police, New Rectangle(x, 0, Math.Min(largeur, restant), Height), couleur,
                              FLAGS Or TextFormatFlags.EndEllipsis)
        Return x + largeur

    End Function

    Private Sub DessinerPastille(g As Graphics, police As Font, x As Integer)

        Dim taille As Size = TextRenderer.MeasureText(g, _pastille, police, Size.Empty, FLAGS)
        Dim hauteur As Integer = Math.Min(Height - 2, taille.Height + 6)
        Dim cadre As New Rectangle(x, (Height - hauteur) \ 2, taille.Width + 20, hauteur)
        If cadre.Right > Width Then Return

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(cadre, hauteur / 2.0F)
            Using pinceau As New SolidBrush(UITheme.ColorGrisVertClair)
                g.FillPath(pinceau, chemin)
            End Using
        End Using
        TextRenderer.DrawText(g, _pastille, police, cadre, UITheme.ColorSaugeTresFonce,
                              FLAGS Or TextFormatFlags.HorizontalCenter)

    End Sub

#End Region

End Class
