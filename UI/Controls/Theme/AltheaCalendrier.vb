' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaCalendrier
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 14/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Calendrier mensuel du kit Althéa, dessiné en code (maquette « AltheaDate », calendrier A,
' retenu le 14/10/2026) : utilisé par AltheaDate dans sa fenêtre déroulante, utilisable seul.
'
' Responsabilités :
' - Afficher un mois (semaine du lundi au dimanche, week-end atténué, jours hors du mois pâles)
' - Marquer aujourd'hui (anneau) et la date choisie (pastille sauge)
' - Changer de mois (flèches, Page haut / Page bas) ou d'année (clic sur l'année : grille des années)
' - Choisir une date à la souris ou au clavier (flèches, Entrée) ; « Aujourd'hui » en un clic
'
' Remarques  :
' - Noms des mois et des jours : culture française (fr-BE), indépendante du poste.
' - Les dates hors de [DateMin ; DateMax] sont grisées et ne se choisissent pas.
' - Évènements : DateValidee (choix fait), Abandon (Échap) — la fenêtre hôte se ferme sur l'un ou l'autre.
'
' Dépendances :
' - UITheme, AltheaDessin, AltheaFondParent
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Globalization

<DesignerCategory("Code")>
<ToolboxItem(True)>
<DefaultEvent("DateValidee")>
<Description("Calendrier Althéa : mois dessiné, choix d'une date à la souris ou au clavier.")>
Public Class AltheaCalendrier
    Inherits Control

#Region "Constantes de présentation"

    Private Const MARGE As Integer = 10
    Private Const HAUTEUR_ENTETE As Integer = 32
    Private Const HAUTEUR_JOURS_SEMAINE As Integer = 22
    Private Const LARGEUR_CASE As Integer = 36
    Private Const HAUTEUR_CASE As Integer = 30
    Private Const HAUTEUR_PIED As Integer = 38
    Private Const RAYON_CASE As Single = 7.0F
    Private Const ANNEES_PAR_PAGE As Integer = 12

    Private Shared ReadOnly Culture As New CultureInfo("fr-BE")

#End Region

#Region "Évènements"

    ''' <summary>Une date a été choisie (clic, Entrée ou « Aujourd'hui »).</summary>
    Public Event DateValidee(sender As Object, choix As Date)

    ''' <summary>L'utilisateur renonce (Échap).</summary>
    Public Event Abandon As EventHandler

#End Region

#Region "Variables privées"

    Private _moisAffiche As Date = New Date(Date.Today.Year, Date.Today.Month, 1)
    Private _dateChoisie As Date? = Nothing
    Private _dateClavier As Date = Date.Today
    Private _dateMin As Date = New Date(1900, 1, 1)
    Private _dateMax As Date = New Date(2099, 12, 31)
    Private _modeAnnees As Boolean
    Private _premiereAnneePage As Integer
    Private _survol As Point = New Point(-1, -1)

    Private ReadOnly _policeEntete As New Font("Calibri", 11.25F, FontStyle.Bold)
    Private ReadOnly _policeJoursSemaine As New Font("Calibri", 9.75F, FontStyle.Bold)
    Private ReadOnly _policeCase As New Font("Calibri", 10.5F)
    Private ReadOnly _policeCaseChoisie As New Font("Calibri", 10.5F, FontStyle.Bold)

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.Selectable Or
                 ControlStyles.SupportsTransparentBackColor, True)

        BackColor = Color.White
        Size = TailleConseillee()
        TabStop = True

    End Sub

    ' Taille qui contient exactement le mois, l'en-tête et le pied.
    Public Shared Function TailleConseillee() As Size
        Return New Size(2 * MARGE + 7 * LARGEUR_CASE,
                        2 * MARGE + HAUTEUR_ENTETE + HAUTEUR_JOURS_SEMAINE + 6 * HAUTEUR_CASE + HAUTEUR_PIED)
    End Function

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Date sélectionnée (Nothing = aucune).")>
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property DateChoisie As Date?
        Get
            Return _dateChoisie
        End Get
        Set(value As Date?)
            _dateChoisie = If(value.HasValue, value.Value.Date, CType(Nothing, Date?))
            _dateClavier = If(_dateChoisie, Date.Today)
            AfficherMois(_dateClavier)
        End Set
    End Property

    <Category("Althéa")>
    <Description("Première date sélectionnable.")>
    Public Property DateMin As Date
        Get
            Return _dateMin
        End Get
        Set(value As Date)
            _dateMin = value.Date
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Dernière date sélectionnable.")>
    Public Property DateMax As Date
        Get
            Return _dateMax
        End Get
        Set(value As Date)
            _dateMax = value.Date
            Invalidate()
        End Set
    End Property

    Private Function ShouldSerializeDateMin() As Boolean
        Return _dateMin <> New Date(1900, 1, 1)
    End Function

    Private Function ShouldSerializeDateMax() As Boolean
        Return _dateMax <> New Date(2099, 12, 31)
    End Function

#End Region

#Region "Navigation"

    ' Affiche le mois qui contient la date donnée (mode jours).
    Private Sub AfficherMois(jour As Date)
        _moisAffiche = New Date(jour.Year, jour.Month, 1)
        _modeAnnees = False
        Invalidate()
    End Sub

    Private Sub ChangerMois(decalage As Integer)
        Dim cible As Date = _moisAffiche.AddMonths(decalage)
        If cible.AddMonths(1).AddDays(-1) < _dateMin OrElse cible > _dateMax Then Return
        _moisAffiche = cible
        _dateClavier = Borner(New Date(cible.Year, cible.Month, Math.Min(_dateClavier.Day, Date.DaysInMonth(cible.Year, cible.Month))))
        Invalidate()
    End Sub

    Private Sub OuvrirAnnees()
        _modeAnnees = True
        _premiereAnneePage = _moisAffiche.Year - (_moisAffiche.Year Mod ANNEES_PAR_PAGE)
        Invalidate()
    End Sub

    Private Function Borner(jour As Date) As Date
        If jour < _dateMin Then Return _dateMin
        If jour > _dateMax Then Return _dateMax
        Return jour
    End Function

    Private Function Selectionnable(jour As Date) As Boolean
        Return jour >= _dateMin AndAlso jour <= _dateMax
    End Function

    ' Choix d'une date : le mois qui la contient s'affiche (« Aujourd'hui » depuis un autre mois).
    Private Sub Valider(jour As Date)
        If Not Selectionnable(jour) Then Return
        _dateChoisie = jour
        _dateClavier = jour
        AfficherMois(jour)
        RaiseEvent DateValidee(Me, jour)
    End Sub

#End Region

#Region "Géométrie"

    Private ReadOnly Property ZoneEntete As Rectangle
        Get
            Return New Rectangle(MARGE, MARGE, 7 * LARGEUR_CASE, HAUTEUR_ENTETE)
        End Get
    End Property

    Private ReadOnly Property FlechePrecedente As Rectangle
        Get
            Return New Rectangle(MARGE, MARGE + 2, 28, 28)
        End Get
    End Property

    Private ReadOnly Property FlecheSuivante As Rectangle
        Get
            Return New Rectangle(MARGE + 7 * LARGEUR_CASE - 28, MARGE + 2, 28, 28)
        End Get
    End Property

    ' Zone cliquable de l'année dans l'en-tête (calculée au dessin).
    Private _zoneAnnee As Rectangle

    Private ReadOnly Property OrigineGrille As Point
        Get
            Return New Point(MARGE, MARGE + HAUTEUR_ENTETE + HAUTEUR_JOURS_SEMAINE)
        End Get
    End Property

    Private ReadOnly Property BoutonAujourdhui As Rectangle
        Get
            Dim y As Integer = OrigineGrille.Y + 6 * HAUTEUR_CASE + 8
            Return New Rectangle(MARGE, y, 104, 28)
        End Get
    End Property

    ' Premier jour affiché (le lundi de la semaine qui contient le 1er du mois).
    Private Function PremierJourGrille() As Date
        Dim decalage As Integer = (CInt(_moisAffiche.DayOfWeek) + 6) Mod 7
        Return _moisAffiche.AddDays(-decalage)
    End Function

    Private Function RectangleCase(ligne As Integer, colonne As Integer) As Rectangle
        Return New Rectangle(OrigineGrille.X + colonne * LARGEUR_CASE + 2, OrigineGrille.Y + ligne * HAUTEUR_CASE + 1,
                             LARGEUR_CASE - 4, HAUTEUR_CASE - 2)
    End Function

    ' Case (ligne ; colonne) sous un point, ou (-1 ; -1). Mode années : grille 3 × 4.
    Private Function CaseSous(p As Point) As Point
        Dim o As Point = OrigineGrille
        If _modeAnnees Then
            Dim largeur As Integer = 7 * LARGEUR_CASE \ 3
            Dim hauteur As Integer = 6 * HAUTEUR_CASE \ 4
            If p.X < o.X OrElse p.Y < o.Y OrElse p.X >= o.X + 3 * largeur OrElse p.Y >= o.Y + 4 * hauteur Then Return New Point(-1, -1)
            Return New Point((p.Y - o.Y) \ hauteur, (p.X - o.X) \ largeur)
        End If
        If p.X < o.X OrElse p.Y < o.Y OrElse p.X >= o.X + 7 * LARGEUR_CASE OrElse p.Y >= o.Y + 6 * HAUTEUR_CASE Then Return New Point(-1, -1)
        Return New Point((p.Y - o.Y) \ HAUTEUR_CASE, (p.X - o.X) \ LARGEUR_CASE)
    End Function

    Private Function RectangleAnnee(ligne As Integer, colonne As Integer) As Rectangle
        Dim largeur As Integer = 7 * LARGEUR_CASE \ 3
        Dim hauteur As Integer = 6 * HAUTEUR_CASE \ 4
        Return New Rectangle(OrigineGrille.X + colonne * largeur + 4, OrigineGrille.Y + ligne * hauteur + 4, largeur - 8, hauteur - 8)
    End Function

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.Clear(BackColor)

        DessinerEntete(g)
        If _modeAnnees Then
            DessinerAnnees(g)
        Else
            DessinerJoursSemaine(g)
            DessinerJours(g)
        End If
        DessinerPied(g)

    End Sub

    Private Sub DessinerEntete(g As Graphics)

        DessinerFleche(g, FlechePrecedente, True)
        DessinerFleche(g, FlecheSuivante, False)

        Dim zone As Rectangle = ZoneEntete
        If _modeAnnees Then
            Dim texte As String = $"{_premiereAnneePage} – {_premiereAnneePage + ANNEES_PAR_PAGE - 1}"
            TextRenderer.DrawText(g, texte, _policeEntete, zone, UITheme.ColorSaugeTresFonce,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            _zoneAnnee = Rectangle.Empty
            Return
        End If

        Dim mois As String = Culture.TextInfo.ToTitleCase(Culture.DateTimeFormat.GetMonthName(_moisAffiche.Month))
        Dim annee As String = $"{_moisAffiche.Year} ▾"
        Dim tMois As Size = TextRenderer.MeasureText(mois, _policeEntete)
        Dim tAnnee As Size = TextRenderer.MeasureText(annee, _policeEntete)
        Dim x As Integer = zone.Left + (zone.Width - tMois.Width - 4 - tAnnee.Width) \ 2
        Dim y As Integer = zone.Top + (zone.Height - tMois.Height) \ 2

        TextRenderer.DrawText(g, mois, _policeEntete, New Point(x, y), UITheme.ColorSaugeTresFonce)

        ' L'année se clique : petite pastille vert pâle.
        _zoneAnnee = New Rectangle(x + tMois.Width + 4, y - 2, tAnnee.Width, tAnnee.Height + 4)
        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(_zoneAnnee, 6.0F),
              fond As New SolidBrush(UITheme.ColorGrisVertClair)
            g.FillPath(fond, chemin)
        End Using
        TextRenderer.DrawText(g, annee, _policeEntete, New Point(_zoneAnnee.X, y), UITheme.ColorSaugeTresFonce)

    End Sub

    Private Shared Sub DessinerFleche(g As Graphics, zone As Rectangle, versGauche As Boolean)

        Dim cx As Single = zone.Left + zone.Width / 2.0F
        Dim cy As Single = zone.Top + zone.Height / 2.0F
        Using stylo As New Pen(UITheme.ColorSaugeTresFonce, 2.4F) With {.StartCap = LineCap.Round, .EndCap = LineCap.Round, .LineJoin = LineJoin.Round}
            If versGauche Then
                g.DrawLines(stylo, {New PointF(cx + 3, cy - 6), New PointF(cx - 3, cy), New PointF(cx + 3, cy + 6)})
            Else
                g.DrawLines(stylo, {New PointF(cx - 3, cy - 6), New PointF(cx + 3, cy), New PointF(cx - 3, cy + 6)})
            End If
        End Using

    End Sub

    Private Sub DessinerJoursSemaine(g As Graphics)

        Dim noms As String() = {"lu", "ma", "me", "je", "ve", "sa", "di"}
        Dim y As Integer = MARGE + HAUTEUR_ENTETE
        For i As Integer = 0 To 6
            Dim zone As New Rectangle(MARGE + i * LARGEUR_CASE, y, LARGEUR_CASE, HAUTEUR_JOURS_SEMAINE)
            TextRenderer.DrawText(g, noms(i), _policeJoursSemaine, zone,
                                  If(i >= 5, UITheme.ColorGrisDiscret, UITheme.ColorSaugeFonce),
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        Next

    End Sub

    Private Sub DessinerJours(g As Graphics)

        Dim jour As Date = PremierJourGrille()

        For ligne As Integer = 0 To 5
            For colonne As Integer = 0 To 6

                Dim zone As Rectangle = RectangleCase(ligne, colonne)
                Dim dansMois As Boolean = jour.Month = _moisAffiche.Month
                Dim possible As Boolean = Selectionnable(jour)
                Dim choisie As Boolean = _dateChoisie.HasValue AndAlso _dateChoisie.Value = jour
                Dim survolee As Boolean = possible AndAlso _survol = New Point(colonne, ligne)
                Dim clavier As Boolean = Focused AndAlso jour = _dateClavier AndAlso Not choisie

                If choisie Then
                    RemplirCase(g, zone, UITheme.ColorSauge)
                ElseIf survolee OrElse clavier Then
                    RemplirCase(g, zone, UITheme.ColorGrisVertClair)
                End If

                If jour = Date.Today AndAlso Not choisie Then
                    Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(zone, RAYON_CASE),
                          stylo As New Pen(UITheme.ColorSaugeClair, 1.6F)
                        g.DrawPath(stylo, chemin)
                    End Using
                End If

                Dim couleur As Color
                If choisie Then
                    couleur = Color.White
                ElseIf Not possible Then
                    couleur = AltheaDessin.CouleurDesactiveTexte
                ElseIf Not dansMois Then
                    couleur = Color.FromArgb(181, 181, 181)
                ElseIf colonne >= 5 Then
                    couleur = UITheme.ColorGrisDiscret
                Else
                    couleur = UITheme.ColorGrisFonce
                End If

                TextRenderer.DrawText(g, jour.Day.ToString(Culture), If(choisie, _policeCaseChoisie, _policeCase), zone, couleur,
                                      TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

                jour = jour.AddDays(1)

            Next
        Next

    End Sub

    Private Sub DessinerAnnees(g As Graphics)

        For i As Integer = 0 To ANNEES_PAR_PAGE - 1

            Dim annee As Integer = _premiereAnneePage + i
            Dim ligne As Integer = i \ 3
            Dim colonne As Integer = i Mod 3
            Dim zone As Rectangle = RectangleAnnee(ligne, colonne)
            Dim possible As Boolean = annee >= _dateMin.Year AndAlso annee <= _dateMax.Year
            Dim courante As Boolean = annee = _moisAffiche.Year

            If courante Then
                RemplirCase(g, zone, UITheme.ColorSauge)
            ElseIf possible AndAlso _survol = New Point(colonne, ligne) Then
                RemplirCase(g, zone, UITheme.ColorGrisVertClair)
            End If

            TextRenderer.DrawText(g, annee.ToString(Culture), If(courante, _policeCaseChoisie, _policeCase), zone,
                                  If(courante, Color.White, If(possible, UITheme.ColorGrisFonce, AltheaDessin.CouleurDesactiveTexte)),
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        Next

    End Sub

    Private Sub DessinerPied(g As Graphics)

        Dim yFilet As Integer = OrigineGrille.Y + 6 * HAUTEUR_CASE + 2
        Using stylo As New Pen(UITheme.ColorBeige)
            g.DrawLine(stylo, MARGE, yFilet, MARGE + 7 * LARGEUR_CASE, yFilet)
        End Using

        Dim bouton As Rectangle = BoutonAujourdhui
        RemplirCase(g, bouton, UITheme.ColorGrisVertClair)
        TextRenderer.DrawText(g, "Aujourd'hui", _policeJoursSemaine, bouton, UITheme.ColorSaugeTresFonce,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

    End Sub

    Private Shared Sub RemplirCase(g As Graphics, zone As Rectangle, couleur As Color)
        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(zone, RAYON_CASE),
              fond As New SolidBrush(couleur)
            g.FillPath(fond, chemin)
        End Using
    End Sub

#End Region

#Region "Souris"

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        Dim c As Point = CaseSous(e.Location)
        Dim nouveau As New Point(c.Y, c.X)
        If nouveau <> _survol Then
            _survol = nouveau
            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        _survol = New Point(-1, -1)
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)

        MyBase.OnMouseDown(e)
        Focus()
        If e.Button <> MouseButtons.Left Then Return

        If FlechePrecedente.Contains(e.Location) Then
            If _modeAnnees Then _premiereAnneePage -= ANNEES_PAR_PAGE : Invalidate() Else ChangerMois(-1)
            Return
        End If
        If FlecheSuivante.Contains(e.Location) Then
            If _modeAnnees Then _premiereAnneePage += ANNEES_PAR_PAGE : Invalidate() Else ChangerMois(1)
            Return
        End If
        If Not _modeAnnees AndAlso _zoneAnnee.Contains(e.Location) Then
            OuvrirAnnees()
            Return
        End If
        If BoutonAujourdhui.Contains(e.Location) Then
            Valider(Date.Today)
            Return
        End If

        Dim c As Point = CaseSous(e.Location)
        If c.X < 0 Then Return

        If _modeAnnees Then
            Dim annee As Integer = _premiereAnneePage + c.X * 3 + c.Y
            If annee < _dateMin.Year OrElse annee > _dateMax.Year Then Return
            Dim jour As Date = Borner(New Date(annee, _moisAffiche.Month, Math.Min(_dateClavier.Day, Date.DaysInMonth(annee, _moisAffiche.Month))))
            _dateClavier = jour
            AfficherMois(jour)
        Else
            Valider(PremierJourGrille().AddDays(c.X * 7 + c.Y))
        End If

    End Sub

#End Region

#Region "Clavier"

    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        Select Case keyData And Keys.KeyCode
            Case Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.PageUp, Keys.PageDown, Keys.Enter, Keys.Escape, Keys.Home
                Return True
        End Select
        Return MyBase.IsInputKey(keyData)
    End Function

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        MyBase.OnKeyDown(e)

        Select Case e.KeyCode
            Case Keys.Left : DeplacerClavier(-1)
            Case Keys.Right : DeplacerClavier(1)
            Case Keys.Up : DeplacerClavier(-7)
            Case Keys.Down : DeplacerClavier(7)
            Case Keys.PageUp : DeplacerClavier(_dateClavier.AddMonths(-1) - _dateClavier)
            Case Keys.PageDown : DeplacerClavier(_dateClavier.AddMonths(1) - _dateClavier)
            Case Keys.Home : DeplacerClavier(Date.Today - _dateClavier)
            Case Keys.Enter : Valider(_dateClavier)
            Case Keys.Escape : RaiseEvent Abandon(Me, EventArgs.Empty)
            Case Else : Return
        End Select

        e.Handled = True

    End Sub

    Private Sub DeplacerClavier(jours As Integer)
        _dateClavier = Borner(_dateClavier.AddDays(jours))
        AfficherMois(_dateClavier)
    End Sub

    Private Sub DeplacerClavier(ecart As TimeSpan)
        DeplacerClavier(CInt(ecart.TotalDays))
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        MyBase.OnGotFocus(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        MyBase.OnLostFocus(e)
        Invalidate()
    End Sub

#End Region

#Region "Libération"

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _policeEntete.Dispose()
            _policeJoursSemaine.Dispose()
            _policeCase.Dispose()
            _policeCaseChoisie.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#End Region

End Class
