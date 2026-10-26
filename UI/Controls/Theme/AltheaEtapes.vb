' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaEtapes
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 14/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Étapes numérotées du kit Althéa : une rangée de pastilles cliquables qui montre, pour chaque
' étape d'un assistant, son état (faite, à compléter, non nécessaire, à faire) et l'étape affichée.
'
' Responsabilités :
' - Dessiner chaque étape : pastille, libellé, ligne d'état (« Fait », « À compléter »…)
' - Signaler l'étape affichée (fond vert pâle et anneau autour de la pastille)
' - Prévenir le parent quand l'utilisateur choisit une étape (souris ou clavier)
'
' Remarques  :
' - Décision du 14/10/2026 (maquette « Domaines — A ») : les étapes remplacent la bande
'   d'onglets de l'écran Domaines. Le contrôle ne connaît aucune règle métier : l'écran calcule
'   les états et les lui donne (DefinirEtat).
' - Les libellés se règlent dans le Designer (propriété Etapes, une ligne par étape) ; le nombre
'   d'étapes en découle.
' - Clavier : Gauche / Droite déplacent l'étape affichée, comme le feraient des onglets.
' - Le contrôle ne change pas l'étape affichée de lui-même au clic : il lève EtapeChoisie et
'   l'écran décide (il peut refuser, par exemple pendant une saisie en cours).
'
' Dépendances :
' - UITheme, AltheaDessin, AltheaFondParent
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

' État d'une étape d'AltheaEtapes.
Public Enum EtatEtape
    ''' <summary>Étape pas encore traitée : numéro dans un cercle.</summary>
    AFaire = 0
    ''' <summary>Étape complète : coche sur fond sauge.</summary>
    Fait = 1
    ''' <summary>Étape à reprendre : point d'exclamation ocre.</summary>
    ACompleter = 2
    ''' <summary>Étape sans objet dans ce cas : cercle pointillé et tiret, texte gris.</summary>
    NonNecessaire = 3
End Enum

<DesignerCategory("Code")>
<ToolboxItem(True)>
<DefaultEvent("EtapeChoisie")>
<Description("Étapes Althéa : pastilles cliquables qui montrent l'état de chaque étape d'un assistant.")>
Public Class AltheaEtapes
    Inherits Control

#Region "Constantes"

    Private Const DiametrePastille As Integer = 30
    Private Const MargeHaute As Integer = 8
    Private Const RayonFond As Single = 10.0F

#End Region

#Region "Variables privées"

    Private _etapes As String() = {"Étape 1", "Étape 2", "Étape 3"}
    Private _etats As EtatEtape() = New EtatEtape(2) {}
    Private _details As String() = New String(2) {}
    Private _indexCourant As Integer = 0
    Private _indexSurvole As Integer = -1

    Private _policeLibelle As Font = New Font("Calibri", 11.0F, FontStyle.Bold)
    Private _policeDetail As Font = New Font("Calibri", 9.5F)
    Private _policePastille As Font = New Font("Calibri", 11.0F, FontStyle.Bold)

#End Region

#Region "Événements"

    ''' <summary>L'utilisateur a choisi une étape (clic ou clavier).</summary>
    Public Event EtapeChoisie(sender As Object, index As Integer)

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.Selectable Or
                 ControlStyles.SupportsTransparentBackColor, True)

        BackColor = Color.Transparent
        Cursor = Cursors.Hand
        TabStop = True
        Size = New Size(600, 84)

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Libellés des étapes, une ligne par étape (ex. « 1. Général »).")>
    Public Property Etapes As String()
        Get
            Return _etapes
        End Get
        Set(value As String())
            _etapes = If(value, Array.Empty(Of String)())
            ReDim Preserve _etats(Math.Max(0, _etapes.Length - 1))
            ReDim Preserve _details(Math.Max(0, _etapes.Length - 1))
            _indexCourant = Math.Min(_indexCourant, Math.Max(0, _etapes.Length - 1))
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Index de l'étape affichée (0 = première).")>
    <DefaultValue(0)>
    Public Property IndexCourant As Integer
        Get
            Return _indexCourant
        End Get
        Set(value As Integer)
            Dim v As Integer = Math.Max(0, Math.Min(value, _etapes.Length - 1))
            If v <> _indexCourant Then
                _indexCourant = v
                Invalidate()
            End If
        End Set
    End Property

    <Browsable(False)>
    Public ReadOnly Property NombreEtapes As Integer
        Get
            Return _etapes.Length
        End Get
    End Property

#End Region

#Region "Sérialisation Designer"

    Private Function ShouldSerializeEtapes() As Boolean
        Return True
    End Function

#End Region

#Region "États"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirEtat
    ' Version    : V1.0.0
    ' Date       : 14/10/2026
    '
    ' Rôle       :
    ' Donne l'état d'une étape et, facultativement, le texte de sa ligne d'état.
    '
    ' Paramètres :
    ' - index  : Étape concernée (0 = première)
    ' - etat   : État à afficher
    ' - detail : Texte sous le libellé ; vide = texte par défaut de l'état (« Fait »…)
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirEtat(index As Integer, etat As EtatEtape, Optional detail As String = Nothing)

        If index < 0 OrElse index >= _etapes.Length Then Return

        _etats(index) = etat
        _details(index) = detail
        Invalidate()

    End Sub

    ' Remet toutes les étapes à « À faire », sans texte.
    Public Sub ReinitialiserEtats()

        For i As Integer = 0 To _etats.Length - 1
            _etats(i) = EtatEtape.AFaire
            _details(i) = Nothing
        Next

        Invalidate()

    End Sub

    ' Libellé d'une étape (pour l'en-tête ou la barre d'état de l'écran).
    Public Function LibelleEtape(index As Integer) As String

        If index < 0 OrElse index >= _etapes.Length Then Return String.Empty
        Return _etapes(index)

    End Function

    Private Function TexteEtat(index As Integer) As String

        If Not String.IsNullOrEmpty(_details(index)) Then Return _details(index)

        Select Case _etats(index)
            Case EtatEtape.Fait
                Return "Fait"
            Case EtatEtape.ACompleter
                Return "À compléter"
            Case EtatEtape.NonNecessaire
                Return "Non nécessaire"
            Case Else
                Return String.Empty
        End Select

    End Function

#End Region

#Region "Géométrie"

    Private Function ZoneEtape(index As Integer) As Rectangle

        If _etapes.Length = 0 Then Return Rectangle.Empty

        Dim largeur As Integer = Math.Max(1, (Width - Padding.Horizontal) \ _etapes.Length)
        Return New Rectangle(Padding.Left + index * largeur, Padding.Top, largeur, Height - Padding.Vertical)

    End Function

    Private Function IndexSous(point As Point) As Integer

        For i As Integer = 0 To _etapes.Length - 1
            If ZoneEtape(i).Contains(point) Then Return i
        Next

        Return -1

    End Function

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        AltheaFondParent.Peindre(Me, e)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        For i As Integer = 0 To _etapes.Length - 1
            DessinerEtape(g, i)
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerEtape
    ' Version    : V1.0.0
    ' Date       : 14/10/2026
    '
    ' Rôle       :
    ' Fond (étape affichée ou survolée), pastille, libellé et ligne d'état d'une étape.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerEtape(g As Graphics, index As Integer)

        Dim zone As Rectangle = ZoneEtape(index)
        Dim courante As Boolean = (index = _indexCourant)
        Dim survolee As Boolean = (index = _indexSurvole) AndAlso Not courante AndAlso Enabled

        ' Fond de l'étape affichée (vert pâle) ou survolée (beige).
        If (courante AndAlso Enabled) OrElse survolee Then
            Dim fond As New RectangleF(zone.X + 3.0F, zone.Y + 1.0F, zone.Width - 6.0F, zone.Height - 2.0F)
            Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(fond, RayonFond)
                Using pinceau As New SolidBrush(If(courante, UITheme.ColorGrisVertClair, UITheme.ColorBeige))
                    g.FillPath(pinceau, chemin)
                End Using
            End Using
        End If

        Dim cercle As New RectangleF(zone.X + (zone.Width - DiametrePastille) / 2.0F, zone.Y + MargeHaute,
                                     DiametrePastille, DiametrePastille)
        DessinerPastille(g, index, cercle, courante)

        ' Libellé et ligne d'état.
        Dim etat As EtatEtape = _etats(index)
        Dim couleurLibelle As Color
        Dim couleurDetail As Color

        If Not Enabled Then
            couleurLibelle = AltheaDessin.CouleurDesactiveTexte
            couleurDetail = AltheaDessin.CouleurDesactiveTexte
        ElseIf etat = EtatEtape.NonNecessaire Then
            couleurLibelle = UITheme.ColorGrisDiscret
            couleurDetail = UITheme.ColorGrisDiscret
        ElseIf etat = EtatEtape.ACompleter Then
            couleurLibelle = UITheme.ColorSaugeTresFonce
            couleurDetail = CouleurOcreTexte()
        Else
            couleurLibelle = UITheme.ColorSaugeTresFonce
            couleurDetail = UITheme.ColorSaugeTresFonce
        End If

        Dim yLibelle As Integer = CInt(cercle.Bottom) + 5
        Dim hLibelle As Integer = TextRenderer.MeasureText("Ag", _policeLibelle).Height
        Dim flags As TextFormatFlags = TextFormatFlags.HorizontalCenter Or TextFormatFlags.Top Or
                                       TextFormatFlags.SingleLine Or TextFormatFlags.EndEllipsis Or
                                       TextFormatFlags.NoPrefix

        TextRenderer.DrawText(g, _etapes(index), _policeLibelle,
                              New Rectangle(zone.X + 4, yLibelle, zone.Width - 8, hLibelle), couleurLibelle, flags)

        Dim detail As String = TexteEtat(index)
        If detail.Length > 0 Then
            TextRenderer.DrawText(g, detail, _policeDetail,
                                  New Rectangle(zone.X + 4, yLibelle + hLibelle, zone.Width - 8, hLibelle), couleurDetail, flags)
        End If

        If Focused AndAlso courante AndAlso ShowFocusCues Then
            Dim cadre As Rectangle = Rectangle.Inflate(zone, -6, -4)
            ControlPaint.DrawFocusRectangle(g, cadre)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerPastille
    ' Rôle       : Cercle de l'étape selon son état ; anneau sauge foncé si c'est l'étape affichée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerPastille(g As Graphics, index As Integer, cercle As RectangleF, courante As Boolean)

        Dim etat As EtatEtape = _etats(index)

        If Not Enabled Then
            Using pinceau As New SolidBrush(AltheaDessin.CouleurDesactiveFond)
                g.FillEllipse(pinceau, cercle)
            End Using
            Using stylo As New Pen(AltheaDessin.CouleurDesactiveBordure, 1.5F)
                g.DrawEllipse(stylo, cercle)
            End Using
            DessinerTexteCentre(g, (index + 1).ToString(), cercle, AltheaDessin.CouleurDesactiveTexte)
            Return
        End If

        ' Anneau de l'étape affichée : blanc puis sauge foncé autour de la pastille.
        If courante Then
            Dim anneau As RectangleF = RectangleF.Inflate(cercle, 4.0F, 4.0F)
            Using stylo As New Pen(UITheme.ColorSaugeTresFonce, 2.0F)
                g.DrawEllipse(stylo, anneau)
            End Using
        End If

        Select Case etat

            Case EtatEtape.Fait
                Using pinceau As New SolidBrush(If(courante, UITheme.ColorSaugeTresFonce, UITheme.ColorSauge))
                    g.FillEllipse(pinceau, cercle)
                End Using
                DessinerCoche(g, cercle)

            Case EtatEtape.ACompleter
                Using pinceau As New SolidBrush(Color.White)
                    g.FillEllipse(pinceau, cercle)
                End Using
                Using stylo As New Pen(UITheme.ColorJauneModere, 2.0F)
                    g.DrawEllipse(stylo, RectangleF.Inflate(cercle, -1.0F, -1.0F))
                End Using
                DessinerTexteCentre(g, "!", cercle, CouleurOcreTexte())

            Case EtatEtape.NonNecessaire
                Using stylo As New Pen(UITheme.ColorRoseBeige, 2.0F) With {.DashStyle = DashStyle.Dash}
                    g.DrawEllipse(stylo, RectangleF.Inflate(cercle, -1.0F, -1.0F))
                End Using
                DessinerTexteCentre(g, "–", cercle, UITheme.ColorGrisDiscret)

            Case Else
                Using pinceau As New SolidBrush(Color.White)
                    g.FillEllipse(pinceau, cercle)
                End Using
                Using stylo As New Pen(UITheme.ColorSauge, 2.0F)
                    g.DrawEllipse(stylo, RectangleF.Inflate(cercle, -1.0F, -1.0F))
                End Using
                DessinerTexteCentre(g, (index + 1).ToString(), cercle, UITheme.ColorSaugeTresFonce)

        End Select

    End Sub

    Private Sub DessinerCoche(g As Graphics, cercle As RectangleF)

        Dim cx As Single = cercle.X + cercle.Width / 2.0F
        Dim cy As Single = cercle.Y + cercle.Height / 2.0F
        Dim points As PointF() = {
            New PointF(cx - 6.5F, cy + 0.5F),
            New PointF(cx - 2.0F, cy + 5.0F),
            New PointF(cx + 7.0F, cy - 4.5F)
        }

        Using stylo As New Pen(Color.White, 3.0F) With {
            .StartCap = LineCap.Round, .EndCap = LineCap.Round, .LineJoin = LineJoin.Round}
            g.DrawLines(stylo, points)
        End Using

    End Sub

    Private Sub DessinerTexteCentre(g As Graphics, texte As String, cercle As RectangleF, couleur As Color)

        TextRenderer.DrawText(g, texte, _policePastille, Rectangle.Round(cercle), couleur,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or
                              TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix Or TextFormatFlags.NoPadding)

    End Sub

    ' Ocre assombri : le texte « À compléter » reste lisible sur fond clair (contraste ≥ 4,5:1).
    Private Shared Function CouleurOcreTexte() As Color
        Return AltheaDessin.Assombrir(UITheme.ColorOrangeMoyenFonce, 0.3F)
    End Function

#End Region

#Region "Souris et clavier"

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)

        Dim trouve As Integer = IndexSous(e.Location)
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

    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)

        MyBase.OnMouseClick(e)

        If e.Button <> MouseButtons.Left Then Return
        Focus()

        Dim index As Integer = IndexSous(e.Location)
        If index >= 0 Then RaiseEvent EtapeChoisie(Me, index)

    End Sub

    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean

        If keyData = Keys.Left OrElse keyData = Keys.Right Then Return True
        Return MyBase.IsInputKey(keyData)

    End Function

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        MyBase.OnKeyDown(e)

        If e.KeyCode = Keys.Left AndAlso _indexCourant > 0 Then
            RaiseEvent EtapeChoisie(Me, _indexCourant - 1)
            e.Handled = True
        ElseIf e.KeyCode = Keys.Right AndAlso _indexCourant < _etapes.Length - 1 Then
            RaiseEvent EtapeChoisie(Me, _indexCourant + 1)
            e.Handled = True
        End If

    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        Invalidate()
        MyBase.OnGotFocus(e)
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        Invalidate()
        MyBase.OnLostFocus(e)
    End Sub

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        _indexSurvole = -1
        Invalidate()
        MyBase.OnEnabledChanged(e)
    End Sub

#End Region

#Region "Nettoyage"

    Protected Overrides Sub Dispose(disposing As Boolean)

        If disposing Then
            _policeLibelle.Dispose()
            _policeDetail.Dispose()
            _policePastille.Dispose()
        End If

        MyBase.Dispose(disposing)

    End Sub

#End Region

End Class
