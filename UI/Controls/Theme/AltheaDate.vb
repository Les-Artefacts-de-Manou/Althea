' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaDate
' Projet     : Althéa
' Version    : V1.1.1
' Date       : 26/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Champ de date du kit Althéa (maquette « AltheaDate », retenue le 14/10/2026) : champ arrondi
' comme la recherche et les listes, saisie au clavier et calendrier du kit (AltheaCalendrier)
' dans une fenêtre déroulante ; remplace le DateTimePicker de Windows, impossible à habiller.
'
' Responsabilités :
' - Saisie : chiffres et « / » (les « / » se placent seuls), formats jj/mm/aaaa, jj/mm/aa, jjmmaa,
'   jjmmaaaa, jj/mm (année en cours) ; date impossible ou hors bornes signalée en rose
' - Valeur As Date? : Nothing seulement si Facultative (vide = pas de date, la croix efface)
' - Calendrier : clic sur l'icône, Alt+Bas ou F4 ; Haut / Bas changent la date d'un jour
' - Lecture seule lisible (règle « Consultation lisible ») : texte normal, fond teinté, sans bouton
'
' Remarques  :
' - Contrôle composé (ADR-33) : une TextBox sans bordure dans un champ dessiné.
' - À la sortie du champ, une saisie invalide est remplacée par la dernière date valide.
' - Enabled = False est dessiné comme la lecture seule : l'information reste lisible.
' - Évènement ValeurChangee à chaque nouvelle date valide (saisie complète, calendrier, croix).
' - V1.1.0 (23/10/2026) : CorrigerBornes (période « du … au … » jamais inversée), sortie de
'   UC_DossierFiche pour servir aussi à la recherche des séances d'une période.
' - V1.1.1 (26/10/2026, audit D-2, ADR-58) : Interpreter devient Friend, pour les tests
'   automatisés (Althea.Tests).
'
' Dépendances :
' - AltheaCalendrier, AltheaIconeEtat (teinte d'erreur), UITheme, AltheaDessin, AltheaFondParent
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Globalization

<DesignerCategory("Code")>
<ToolboxItem(True)>
<DefaultEvent("ValeurChangee")>
<Description("Champ de date Althéa : saisie au clavier et calendrier du kit.")>
Public Class AltheaDate
    Inherits UserControl

#Region "Constantes"

    Private Const RAYON As Single = 9.0F
    Private Const MARGE_GAUCHE As Integer = 10
    Private Const TAILLE_BOUTON As Integer = 26
    Private Const MARGE_DROITE As Integer = 4
    Private Const FORMAT_DATE As String = "dd/MM/yyyy"

    Private Shared ReadOnly Culture As New CultureInfo("fr-BE")
    Private Shared ReadOnly FormatsAcceptes As String() =
        {"d/M/yyyy", "d/M/yy", "ddMMyyyy", "ddMMyy", "d-M-yyyy", "d.M.yyyy"}

#End Region

#Region "Évènements"

    ''' <summary>La date a changé (nouvelle date valide, ou effacée si facultative).</summary>
    Public Event ValeurChangee As EventHandler

#End Region

#Region "Variables privées"

    Private ReadOnly _saisie As New TextBox()
    Private _valeur As Date? = Date.Today
    Private _facultative As Boolean
    Private _lectureSeule As Boolean
    Private _dateMin As Date = New Date(1900, 1, 1)
    Private _dateMax As Date = New Date(2099, 12, 31)
    Private _erreur As Boolean
    Private _survolee As Boolean
    Private _miseAJour As Boolean
    Private _longueurPrecedente As Integer

    ' Fenêtre déroulante du calendrier (créée à la première ouverture).
    Private _deroulant As ToolStripDropDown
    Private _calendrier As AltheaCalendrier

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        MyBase.BackColor = Color.Transparent
        Font = New Font("Calibri", 11.0F)
        Size = New Size(140, 32)

        _saisie.BorderStyle = BorderStyle.None
        _saisie.BackColor = Color.White
        _saisie.ForeColor = UITheme.ColorGrisFonce
        _saisie.Font = Font
        _saisie.MaxLength = 10
        _saisie.PlaceholderText = "jj/mm/aaaa"

        AddHandler _saisie.KeyPress, AddressOf Saisie_KeyPress
        AddHandler _saisie.KeyDown, AddressOf Saisie_KeyDown
        AddHandler _saisie.TextChanged, AddressOf Saisie_TextChanged
        AddHandler _saisie.Leave, AddressOf Saisie_Leave
        AddHandler _saisie.GotFocus, AddressOf Saisie_FocusChange
        AddHandler _saisie.LostFocus, AddressOf Saisie_FocusChange
        AddHandler _saisie.MouseEnter, AddressOf Saisie_MouseEnter
        AddHandler _saisie.MouseLeave, AddressOf Saisie_MouseLeave

        Controls.Add(_saisie)
        AfficherValeur()
        PlacerSaisie()

    End Sub

#End Region

#Region "Propriétés publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : Valeur
    ' Rôle      : Date affichée. Nothing n'est possible que si Facultative ; sinon Nothing est ignoré.
    '             Une date hors bornes est ramenée dans [DateMin ; DateMax].
    ' -------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Valeur As Date?
        Get
            Return _valeur
        End Get
        Set(value As Date?)
            DefinirValeur(value, notifier:=True)
        End Set
    End Property

    <Category("Althéa")>
    <Description("Date facultative : champ vide = pas de date ; une croix permet d'effacer.")>
    <DefaultValue(False)>
    Public Property Facultative As Boolean
        Get
            Return _facultative
        End Get
        Set(value As Boolean)
            _facultative = value
            If Not value AndAlso Not _valeur.HasValue Then DefinirValeur(Borner(Date.Today), notifier:=False)
            PlacerSaisie()
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Consultation : texte normal sur fond teinté, sans bouton ni saisie.")>
    <DefaultValue(False)>
    Public Property LectureSeule As Boolean
        Get
            Return _lectureSeule
        End Get
        Set(value As Boolean)
            _lectureSeule = value
            AppliquerEtatSaisie()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Première date acceptée.")>
    Public Property DateMin As Date
        Get
            Return _dateMin
        End Get
        Set(value As Date)
            _dateMin = value.Date
            If _valeur.HasValue AndAlso _valeur.Value < _dateMin Then DefinirValeur(_dateMin, notifier:=True)
        End Set
    End Property

    <Category("Althéa")>
    <Description("Dernière date acceptée.")>
    Public Property DateMax As Date
        Get
            Return _dateMax
        End Get
        Set(value As Date)
            _dateMax = value.Date
            If _valeur.HasValue AndAlso _valeur.Value > _dateMax Then DefinirValeur(_dateMax, notifier:=True)
        End Set
    End Property

    Private Function ShouldSerializeDateMin() As Boolean
        Return _dateMin <> New Date(1900, 1, 1)
    End Function

    Private Function ShouldSerializeDateMax() As Boolean
        Return _dateMax <> New Date(2099, 12, 31)
    End Function

    <Category("Althéa")>
    <Description("Texte d'aide affiché quand le champ est vide.")>
    <DefaultValue("jj/mm/aaaa")>
    Public Property TexteAide As String
        Get
            Return _saisie.PlaceholderText
        End Get
        Set(value As String)
            _saisie.PlaceholderText = If(value, String.Empty)
        End Set
    End Property

    ' Le fond reste transparent : seuls les coins laissent voir le parent.
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

#End Region

#Region "Valeur"

    Private Sub DefinirValeur(nouvelle As Date?, notifier As Boolean)

        Dim bornee As Date? = If(nouvelle.HasValue, Borner(nouvelle.Value.Date), CType(Nothing, Date?))
        If Not bornee.HasValue AndAlso Not _facultative Then bornee = If(_valeur, Borner(Date.Today))

        Dim change As Boolean = Not Nullable.Equals(bornee, _valeur)
        _valeur = bornee
        _erreur = False
        AfficherValeur()
        PlacerSaisie()
        Invalidate()

        If change AndAlso notifier Then RaiseEvent ValeurChangee(Me, EventArgs.Empty)

    End Sub

    Private Function Borner(jour As Date) As Date
        If jour < _dateMin Then Return _dateMin
        If jour > _dateMax Then Return _dateMax
        Return jour
    End Function

    ' Affiche la valeur dans la zone de saisie sans déclencher l'interprétation.
    Private Sub AfficherValeur()
        _miseAJour = True
        _saisie.Text = If(_valeur.HasValue, _valeur.Value.ToString(FORMAT_DATE, Culture), String.Empty)
        _longueurPrecedente = _saisie.TextLength
        _miseAJour = False
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Interpreter
    ' Rôle     : Lit une saisie (jj/mm/aaaa, jj/mm/aa, jjmmaa, jjmmaaaa, jj/mm). Année sur deux chiffres :
    '            siècle actuel, sauf si cela donne une date à plus de 20 ans dans le futur (« 87 » → 1987).
    ' Retour   : la date, ou Nothing si la saisie n'est pas une date possible.
    ' Accès    : Friend pour les tests automatisés (Althea.Tests, ADR-58).
    ' -------------------------------------------------------------------------------------------------
    Friend Shared Function Interpreter(texte As String) As Date?

        Dim t As String = texte.Trim()
        If t.Length = 0 Then Return Nothing

        ' jj/mm : année en cours.
        Dim parties As String() = t.Split("/"c)
        If parties.Length = 2 AndAlso parties(1).Length > 0 Then t &= "/" & Date.Today.Year.ToString(Culture)

        Dim resultat As Date
        If Not Date.TryParseExact(t, FormatsAcceptes, Culture, DateTimeStyles.None, resultat) Then Return Nothing

        ' Année sur deux chiffres : la règle du poste est remplacée par une règle explicite.
        Dim chiffresAnnee As String = If(t.Contains("/"c) OrElse t.Contains("-"c) OrElse t.Contains("."c),
                                         t.Split("/"c, "-"c, "."c).Last(), t.Substring(4))
        If chiffresAnnee.Length = 2 Then
            Dim annee As Integer = 2000 + CInt(chiffresAnnee)
            If annee > Date.Today.Year + 20 Then annee -= 100
            If Not DateValide(annee, resultat.Month, resultat.Day) Then Return Nothing
            resultat = New Date(annee, resultat.Month, resultat.Day)
        End If

        Return resultat.Date

    End Function

    Private Shared Function DateValide(annee As Integer, mois As Integer, jour As Integer) As Boolean
        Return annee >= 1 AndAlso annee <= 9999 AndAlso mois >= 1 AndAlso mois <= 12 AndAlso
               jour >= 1 AndAlso jour <= Date.DaysInMonth(annee, mois)
    End Function

    Private Function DansLesBornes(jour As Date) As Boolean
        Return jour >= _dateMin AndAlso jour <= _dateMax
    End Function

#End Region

#Region "Saisie"

    ' Seuls chiffres, séparateurs et touches d'édition passent.
    Private Sub Saisie_KeyPress(sender As Object, e As KeyPressEventArgs)
        If Char.IsControl(e.KeyChar) Then Return
        If Char.IsDigit(e.KeyChar) Then Return
        If e.KeyChar = "/"c OrElse e.KeyChar = "-"c OrElse e.KeyChar = "."c Then
            e.KeyChar = "/"c
            Return
        End If
        e.Handled = True
    End Sub

    Private Sub Saisie_KeyDown(sender As Object, e As KeyEventArgs)

        If (e.Alt AndAlso e.KeyCode = Keys.Down) OrElse e.KeyCode = Keys.F4 Then
            OuvrirCalendrier()
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            Dim base As Date = If(Interpreter(_saisie.Text), If(_valeur, Date.Today))
            DefinirValeur(base.AddDays(If(e.KeyCode = Keys.Up, 1, -1)), notifier:=True)
            _saisie.SelectAll()
            e.SuppressKeyPress = True
        End If

        OnKeyDown(e)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Saisie_TextChanged
    ' Rôle      : Place les « / » après le jour et le mois quand on tape à la fin du champ, puis
    '             interprète la saisie : une date complète et valide devient la valeur ; une saisie
    '             complète impossible passe le contour en rose.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Saisie_TextChanged(sender As Object, e As EventArgs)

        If _miseAJour Then Return

        Dim texte As String = _saisie.Text
        Dim ajout As Boolean = texte.Length > _longueurPrecedente
        _longueurPrecedente = texte.Length

        If ajout AndAlso _saisie.SelectionStart = texte.Length AndAlso
           (texte.Length = 2 OrElse texte.Length = 5) AndAlso Char.IsDigit(texte(texte.Length - 1)) AndAlso
           texte.Count(Function(c) c = "/"c) = If(texte.Length = 2, 0, 1) Then
            _miseAJour = True
            _saisie.Text = texte & "/"
            _saisie.SelectionStart = _saisie.TextLength
            _longueurPrecedente = _saisie.TextLength
            _miseAJour = False
            texte = _saisie.Text
        End If

        If texte.Length = 0 Then
            _erreur = False
            If _facultative AndAlso _valeur.HasValue Then
                _valeur = Nothing
                RaiseEvent ValeurChangee(Me, EventArgs.Empty)
            End If
            Invalidate()
            Return
        End If

        ' Saisie complète (jj/mm/aaaa, ou 6 / 8 chiffres sans séparateur) : on juge tout de suite.
        Dim complete As Boolean = texte.Length = 10 OrElse
                                  (Not texte.Contains("/"c) AndAlso (texte.Length = 6 OrElse texte.Length = 8))
        If Not complete Then
            _erreur = False
            Invalidate()
            Return
        End If

        Dim lue As Date? = Interpreter(texte)
        _erreur = Not lue.HasValue OrElse Not DansLesBornes(lue.Value)
        If Not _erreur AndAlso Not Nullable.Equals(lue, _valeur) Then
            _valeur = lue
            RaiseEvent ValeurChangee(Me, EventArgs.Empty)
        End If
        Invalidate()

    End Sub

    ' À la sortie : saisie partielle interprétée, sinon retour à la dernière date valide.
    Private Sub Saisie_Leave(sender As Object, e As EventArgs)

        Dim texte As String = _saisie.Text.Trim()

        If texte.Length = 0 Then
            DefinirValeur(Nothing, notifier:=True)
            Return
        End If

        Dim lue As Date? = Interpreter(texte)
        If lue.HasValue AndAlso DansLesBornes(lue.Value) Then
            DefinirValeur(lue, notifier:=True)
        Else
            DefinirValeur(_valeur, notifier:=False)
        End If

    End Sub

#End Region

#Region "Période"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : CorrigerBornes
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Garde une période « du … au … » dans le bon ordre : quand une borne vient de dépasser l'autre,
    ' l'autre la suit (la date que l'on vient de choisir est respectée).
    '
    ' Paramètres :
    ' - dtpDu, dtpAu : champs de début et de fin (non facultatifs)
    ' - source       : champ qui vient de changer (sender de ValeurChangee)
    '
    ' Remarques :
    ' - La borne corrigée déclenche à son tour ValeurChangee ; l'appel suivant ne change plus rien.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub CorrigerBornes(dtpDu As AltheaDate, dtpAu As AltheaDate, source As Object)
        If dtpDu Is Nothing OrElse dtpAu Is Nothing Then Return
        If Not dtpDu.Valeur.HasValue OrElse Not dtpAu.Valeur.HasValue Then Return
        If dtpDu.Valeur.Value <= dtpAu.Valeur.Value Then Return
        If source Is dtpDu Then
            dtpAu.Valeur = dtpDu.Valeur
        Else
            dtpDu.Valeur = dtpAu.Valeur
        End If
    End Sub

#End Region

#Region "Calendrier"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirCalendrier
    ' Rôle      : Ouvre AltheaCalendrier sous le champ, sur la date affichée (ou aujourd'hui).
    ' -------------------------------------------------------------------------------------------------
    Public Sub OuvrirCalendrier()

        If _lectureSeule OrElse Not Enabled Then Return

        If _deroulant Is Nothing Then
            _calendrier = New AltheaCalendrier()
            AddHandler _calendrier.DateValidee, AddressOf Calendrier_DateValidee
            AddHandler _calendrier.Abandon, AddressOf Calendrier_Abandon

            Dim hote As New ToolStripControlHost(_calendrier) With {.Margin = Padding.Empty, .Padding = Padding.Empty, .AutoSize = False,
                                                                    .Size = AltheaCalendrier.TailleConseillee()}
            _deroulant = New ToolStripDropDown() With {.Padding = New Padding(1), .DropShadowEnabled = True, .BackColor = UITheme.ColorBeigeFonce}
            _deroulant.Items.Add(hote)
        End If

        _calendrier.DateMin = _dateMin
        _calendrier.DateMax = _dateMax
        _calendrier.DateChoisie = If(Interpreter(_saisie.Text), _valeur)

        _deroulant.Show(Me, New Point(0, Height + 2))
        _calendrier.Focus()

    End Sub

    Private Sub Calendrier_DateValidee(sender As Object, choix As Date)
        _deroulant.Close()
        DefinirValeur(choix, notifier:=True)
        _saisie.Focus()
        _saisie.SelectAll()
    End Sub

    Private Sub Calendrier_Abandon(sender As Object, e As EventArgs)
        _deroulant.Close()
        _saisie.Focus()
    End Sub

#End Region

#Region "Mise en page et état"

    ' Boutons dessinés à droite : calendrier, et croix si facultative et remplie.
    Private ReadOnly Property ZoneCalendrier As Rectangle
        Get
            Return New Rectangle(Width - MARGE_DROITE - TAILLE_BOUTON, (Height - TAILLE_BOUTON) \ 2, TAILLE_BOUTON, TAILLE_BOUTON)
        End Get
    End Property

    Private ReadOnly Property ZoneCroix As Rectangle
        Get
            Return New Rectangle(ZoneCalendrier.Left - TAILLE_BOUTON, ZoneCalendrier.Top, TAILLE_BOUTON, TAILLE_BOUTON)
        End Get
    End Property

    Private ReadOnly Property BoutonsVisibles As Boolean
        Get
            Return Not _lectureSeule AndAlso Enabled
        End Get
    End Property

    Private ReadOnly Property CroixVisible As Boolean
        Get
            Return BoutonsVisibles AndAlso _facultative AndAlso _valeur.HasValue
        End Get
    End Property

    Private Sub PlacerSaisie()

        Dim droite As Integer = If(BoutonsVisibles, MARGE_DROITE + TAILLE_BOUTON * If(CroixVisible, 2, 1) + 2, MARGE_GAUCHE)
        Dim h As Integer = _saisie.PreferredHeight
        _saisie.SetBounds(MARGE_GAUCHE, Math.Max(0, (Height - h) \ 2), Math.Max(10, Width - MARGE_GAUCHE - droite), h)

    End Sub

    Private Sub AppliquerEtatSaisie()
        _saisie.ReadOnly = _lectureSeule OrElse Not Enabled
        _saisie.BackColor = FondChamp()
        _saisie.TabStop = Not _saisie.ReadOnly
        PlacerSaisie()
        Invalidate()
    End Sub

    Private Function FondChamp() As Color
        Return If(_lectureSeule OrElse Not Enabled, UITheme.ColorChampLectureSeule, Color.White)
    End Function

    Protected Overrides Sub OnLayout(e As LayoutEventArgs)
        MyBase.OnLayout(e)
        PlacerSaisie()
    End Sub

    Protected Overrides Sub OnFontChanged(e As EventArgs)
        MyBase.OnFontChanged(e)
        _saisie.Font = Font
        PlacerSaisie()
    End Sub

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        MyBase.OnEnabledChanged(e)
        AppliquerEtatSaisie()
    End Sub

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        AltheaFondParent.Peindre(Me, e)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim actif As Boolean = BoutonsVisibles
        Dim focus As Boolean = actif AndAlso _saisie.Focused
        Dim epaisseur As Single = If(focus OrElse (_erreur AndAlso actif), 2.0F, 1.0F)
        Dim rect As New RectangleF(epaisseur / 2.0F, epaisseur / 2.0F, Width - epaisseur, Height - epaisseur)
        Dim teinteErreur = AltheaIconeEtat.Teintes(EtatIcone.Erreur)

        Dim bordure As Color
        If _erreur AndAlso actif Then
            bordure = teinteErreur.Base
        ElseIf focus Then
            bordure = UITheme.ColorSauge
        ElseIf actif AndAlso _survolee Then
            bordure = UITheme.ColorSaugeClair
        Else
            bordure = UITheme.ColorBeigeFonce
        End If

        _saisie.ForeColor = If(_erreur AndAlso actif, teinteErreur.Foncee, UITheme.ColorGrisFonce)

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rect, RAYON)
            Using pinceau As New SolidBrush(FondChamp())
                g.FillPath(pinceau, chemin)
            End Using
            Using stylo As New Pen(bordure, epaisseur)
                g.DrawPath(stylo, chemin)
            End Using
        End Using

        If Not actif Then Return

        DessinerIconeCalendrier(g, ZoneCalendrier)
        If CroixVisible Then DessinerCroix(g, ZoneCroix)

    End Sub

    Private Shared Sub DessinerIconeCalendrier(g As Graphics, zone As Rectangle)

        Dim x As Single = zone.Left + 5.0F
        Dim y As Single = zone.Top + 5.5F
        Using stylo As New Pen(UITheme.ColorSaugeFonce, 1.7F) With {.StartCap = LineCap.Round, .EndCap = LineCap.Round}
            Using cadre As GraphicsPath = AltheaDessin.CheminArrondi(New RectangleF(x, y + 1.5F, 16.0F, 14.0F), 2.5F)
                g.DrawPath(stylo, cadre)
            End Using
            g.DrawLine(stylo, x, y + 6.0F, x + 16.0F, y + 6.0F)
            g.DrawLine(stylo, x + 4.5F, y - 0.5F, x + 4.5F, y + 3.0F)
            g.DrawLine(stylo, x + 11.5F, y - 0.5F, x + 11.5F, y + 3.0F)
        End Using

    End Sub

    Private Shared Sub DessinerCroix(g As Graphics, zone As Rectangle)

        Dim cx As Single = zone.Left + zone.Width / 2.0F
        Dim cy As Single = zone.Top + zone.Height / 2.0F
        Using stylo As New Pen(UITheme.ColorGrisDiscret, 1.9F) With {.StartCap = LineCap.Round, .EndCap = LineCap.Round}
            g.DrawLine(stylo, cx - 4.5F, cy - 4.5F, cx + 4.5F, cy + 4.5F)
            g.DrawLine(stylo, cx + 4.5F, cy - 4.5F, cx - 4.5F, cy + 4.5F)
        End Using

    End Sub

#End Region

#Region "Souris et focus"

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)

        MyBase.OnMouseDown(e)
        If Not BoutonsVisibles OrElse e.Button <> MouseButtons.Left Then Return

        If ZoneCalendrier.Contains(e.Location) Then
            OuvrirCalendrier()
        ElseIf CroixVisible AndAlso ZoneCroix.Contains(e.Location) Then
            DefinirValeur(Nothing, notifier:=True)
            _saisie.Focus()
        Else
            _saisie.Focus()
        End If

    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        Dim surBouton As Boolean = BoutonsVisibles AndAlso
                                   (ZoneCalendrier.Contains(e.Location) OrElse (CroixVisible AndAlso ZoneCroix.Contains(e.Location)))
        Cursor = If(surBouton, Cursors.Hand, Cursors.Default)
    End Sub

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        _survolee = True
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        _survolee = ClientRectangle.Contains(PointToClient(Cursor.Position))
        Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

    Private Sub Saisie_MouseEnter(sender As Object, e As EventArgs)
        _survolee = True
        Invalidate()
    End Sub

    Private Sub Saisie_MouseLeave(sender As Object, e As EventArgs)
        _survolee = ClientRectangle.Contains(PointToClient(Cursor.Position))
        Invalidate()
    End Sub

    Private Sub Saisie_FocusChange(sender As Object, e As EventArgs)
        Invalidate()
    End Sub

    Protected Overrides Sub OnEnter(e As EventArgs)
        If Not _saisie.ReadOnly Then _saisie.Focus()
        MyBase.OnEnter(e)
    End Sub

#End Region

#Region "Libération"

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _deroulant?.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#End Region

End Class
