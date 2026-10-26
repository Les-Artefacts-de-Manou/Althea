' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaHeure
' Projet     : Althéa
' Version    : V1.0.1
' Date       : 26/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Champ d'heure du kit Althéa (maquette « AltheaHeure », liste A retenue le 16/10/2026) : même
' champ arrondi que AltheaDate, saisie au clavier et liste déroulante des heures proposées ;
' remplace les listes Windows saisissables et le DateTimePicker en mode heure.
'
' Responsabilités :
' - Saisie : chiffres, « : » placé seul ; « h », « . » acceptés comme séparateur ; formats 9, 930,
'   0930, 9:30, 9h30, 9h ; heure impossible ou hors bornes signalée en rose
' - Valeur As TimeSpan? (heures et minutes) : Nothing seulement si Facultative (vide = pas d'heure)
' - Liste des heures proposées (HeuresProposees) : clic sur l'horloge, Alt+Bas ou F4 ; avec une
'   HeureReference (heure de début), chaque ligne rappelle la durée (« 45 min ») et la liste
'   commence à cette heure
' - Haut / Bas : heure suivante / précédente sur le pas (Pas, en minutes)
' - Lecture seule lisible (règle « Consultation lisible ») : texte normal, fond teinté, sans bouton
'
' Remarques  :
' - Contrôle composé (ADR-33), construit sur le modèle d'AltheaDate (même dessin, même comportement
'   de saisie, de sortie et de lecture seule) : une TextBox sans bordure dans un champ dessiné.
' - À la sortie du champ, une saisie invalide est remplacée par la dernière heure valide.
' - Enabled = False est dessiné comme la lecture seule : l'information reste lisible.
' - Évènement ValeurChangee à chaque nouvelle heure valide (saisie complète, liste, croix, flèches).
' - Sans HeuresProposees, la liste propose toute la plage [HeureMin ; HeureMax] au pas choisi.
' - V1.0.1 (26/10/2026, audit D-2, ADR-58) : Interpreter, Tronquer et DeuxPointsAttendus deviennent
'   Friend, pour les tests automatisés (Althea.Tests).
'
' Dépendances :
' - AltheaIconeEtat (teinte d'erreur), UITheme, AltheaDessin, AltheaFondParent
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Globalization

<DesignerCategory("Code")>
<ToolboxItem(True)>
<DefaultEvent("ValeurChangee")>
<Description("Champ d'heure Althéa : saisie au clavier et liste des heures proposées.")>
Public Class AltheaHeure
    Inherits UserControl

#Region "Constantes"

    Private Const RAYON As Single = 9.0F
    Private Const MARGE_GAUCHE As Integer = 10
    Private Const TAILLE_BOUTON As Integer = 26
    Private Const MARGE_DROITE As Integer = 4
    Private Const FORMAT_HEURE As String = "hh\:mm"
    Private Const PAS_PAR_DEFAUT As Integer = 15
    Private Const LIGNES_VISIBLES As Integer = 8
    Private Const LARGEUR_LISTE_MIN As Integer = 150

    Private Shared ReadOnly HEURE_PAR_DEFAUT As New TimeSpan(9, 0, 0)
    Private Shared ReadOnly HEURE_MIN_DEFAUT As TimeSpan = TimeSpan.Zero
    Private Shared ReadOnly HEURE_MAX_DEFAUT As New TimeSpan(23, 59, 0)

#End Region

#Region "Évènements"

    ''' <summary>L'heure a changé (nouvelle heure valide, ou effacée si facultative).</summary>
    Public Event ValeurChangee As EventHandler

#End Region

#Region "Variables privées"

    Private ReadOnly _saisie As New TextBox()
    Private _valeur As TimeSpan? = HEURE_PAR_DEFAUT
    Private _facultative As Boolean
    Private _afficherCroix As Boolean = True
    Private _lectureSeule As Boolean
    Private _heureMin As TimeSpan = HEURE_MIN_DEFAUT
    Private _heureMax As TimeSpan = HEURE_MAX_DEFAUT
    Private _pas As Integer = PAS_PAR_DEFAUT
    Private _heuresProposees As New List(Of TimeSpan)
    Private _heureReference As TimeSpan?
    Private _erreur As Boolean
    Private _survolee As Boolean
    Private _miseAJour As Boolean
    Private _longueurPrecedente As Integer

    ' Fenêtre déroulante de la liste (créée à la première ouverture).
    Private _deroulant As ToolStripDropDown
    Private _hote As ToolStripControlHost
    Private _liste As ListeHeures

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
        Size = New Size(100, 32)

        _saisie.BorderStyle = BorderStyle.None
        _saisie.BackColor = Color.White
        _saisie.ForeColor = UITheme.ColorGrisFonce
        _saisie.Font = Font
        _saisie.MaxLength = 5
        _saisie.PlaceholderText = "hh:mm"

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
    ' Rôle      : Heure affichée (heures et minutes, secondes ignorées). Nothing n'est possible que si
    '             Facultative ; sinon Nothing est ignoré. Hors bornes : ramenée dans [HeureMin ; HeureMax].
    ' -------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Valeur As TimeSpan?
        Get
            Return _valeur
        End Get
        Set(value As TimeSpan?)
            DefinirValeur(value, notifier:=True)
        End Set
    End Property

    <Category("Althéa")>
    <Description("Heure facultative : champ vide = pas d'heure.")>
    <DefaultValue(False)>
    Public Property Facultative As Boolean
        Get
            Return _facultative
        End Get
        Set(value As Boolean)
            _facultative = value
            If Not value AndAlso Not _valeur.HasValue Then DefinirValeur(Borner(HEURE_PAR_DEFAUT), notifier:=False)
            PlacerSaisie()
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Heure facultative et remplie : croix pour effacer. Sans croix, on efface en vidant le champ (place réduite).")>
    <DefaultValue(True)>
    Public Property AfficherCroix As Boolean
        Get
            Return _afficherCroix
        End Get
        Set(value As Boolean)
            _afficherCroix = value
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
    <Description("Pas des flèches Haut / Bas et de la liste par défaut, en minutes (1 à 60).")>
    <DefaultValue(PAS_PAR_DEFAUT)>
    Public Property Pas As Integer
        Get
            Return _pas
        End Get
        Set(value As Integer)
            _pas = Math.Max(1, Math.Min(60, value))
        End Set
    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property HeureMin As TimeSpan
        Get
            Return _heureMin
        End Get
        Set(value As TimeSpan)
            _heureMin = Tronquer(value)
            If _valeur.HasValue AndAlso _valeur.Value < _heureMin Then DefinirValeur(_heureMin, notifier:=True)
        End Set
    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property HeureMax As TimeSpan
        Get
            Return _heureMax
        End Get
        Set(value As TimeSpan)
            _heureMax = Tronquer(value)
            If _valeur.HasValue AndAlso _valeur.Value > _heureMax Then DefinirValeur(_heureMax, notifier:=True)
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : HeuresProposees
    ' Rôle      : Heures de la liste déroulante (ex. GestionSeances.CreneauxProposes). Une heure hors
    '             liste reste saisissable au clavier.
    ' -------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property HeuresProposees As IList(Of TimeSpan)
        Get
            Return _heuresProposees.AsReadOnly()
        End Get
        Set(value As IList(Of TimeSpan))
            _heuresProposees = If(value Is Nothing, New List(Of TimeSpan),
                                  value.Select(AddressOf Tronquer).Distinct().OrderBy(Function(h) h).ToList())
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : HeureReference
    ' Rôle      : Heure de départ (début de séance pour le champ de fin) : la liste commence à cette
    '             heure et chaque ligne rappelle la durée écoulée. Nothing = liste simple.
    ' -------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property HeureReference As TimeSpan?
        Get
            Return _heureReference
        End Get
        Set(value As TimeSpan?)
            _heureReference = If(value.HasValue, Tronquer(value.Value), CType(Nothing, TimeSpan?))
        End Set
    End Property

    <Category("Althéa")>
    <Description("Texte d'aide affiché quand le champ est vide.")>
    <DefaultValue("hh:mm")>
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

    Private Sub DefinirValeur(nouvelle As TimeSpan?, notifier As Boolean)

        Dim bornee As TimeSpan? = If(nouvelle.HasValue, Borner(Tronquer(nouvelle.Value)), CType(Nothing, TimeSpan?))
        If Not bornee.HasValue AndAlso Not _facultative Then bornee = If(_valeur, Borner(HEURE_PAR_DEFAUT))

        Dim change As Boolean = Not Nullable.Equals(bornee, _valeur)
        _valeur = bornee
        _erreur = False
        AfficherValeur()
        PlacerSaisie()
        Invalidate()

        If change AndAlso notifier Then RaiseEvent ValeurChangee(Me, EventArgs.Empty)

    End Sub

    ' Heures et minutes seulement, ramenées dans la journée (une fin calculée après minuit — début
    ' tardif + durée — s'arrête à 23:59 au lieu de repartir à 00:00). Friend : testé (Althea.Tests).
    Friend Shared Function Tronquer(heure As TimeSpan) As TimeSpan
        Dim minutes As Double = Math.Floor(heure.TotalMinutes)
        Return TimeSpan.FromMinutes(Math.Max(0.0, Math.Min(24.0 * 60.0 - 1.0, minutes)))
    End Function

    Private Function Borner(heure As TimeSpan) As TimeSpan
        If heure < _heureMin Then Return _heureMin
        If heure > _heureMax Then Return _heureMax
        Return heure
    End Function

    Private Function DansLesBornes(heure As TimeSpan) As Boolean
        Return heure >= _heureMin AndAlso heure <= _heureMax
    End Function

    ' Affiche la valeur dans la zone de saisie sans déclencher l'interprétation.
    Private Sub AfficherValeur()
        _miseAJour = True
        _saisie.Text = If(_valeur.HasValue, _valeur.Value.ToString(FORMAT_HEURE, CultureInfo.InvariantCulture), String.Empty)
        _longueurPrecedente = _saisie.TextLength
        _miseAJour = False
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Interpreter
    ' Rôle     : Lit une saisie : « 9 » (9:00), « 930 », « 0930 », « 9:30 », « 9h30 », « 9h », « 9. »…
    ' Retour   : l'heure, ou Nothing si la saisie n'est pas une heure possible (0:00 à 23:59).
    ' Accès    : Friend pour les tests automatisés (Althea.Tests, ADR-58).
    ' -------------------------------------------------------------------------------------------------
    Friend Shared Function Interpreter(texte As String) As TimeSpan?

        Dim t As String = texte.Trim().Replace("h"c, ":"c).Replace("H"c, ":"c).Replace("."c, ":"c)
        If t.Length = 0 Then Return Nothing

        Dim heures, minutes As Integer
        Dim parties As String() = t.Split(":"c)

        If parties.Length = 2 Then
            If parties(0).Length = 0 OrElse parties(0).Length > 2 OrElse parties(1).Length > 2 Then Return Nothing
            If Not Integer.TryParse(parties(0), NumberStyles.None, CultureInfo.InvariantCulture, heures) Then Return Nothing
            If parties(1).Length = 0 Then
                minutes = 0
            ElseIf Not Integer.TryParse(parties(1), NumberStyles.None, CultureInfo.InvariantCulture, minutes) Then
                Return Nothing
            End If
        ElseIf parties.Length = 1 Then
            If Not t.All(AddressOf Char.IsDigit) Then Return Nothing
            Select Case t.Length
                Case 1, 2
                    heures = CInt(t)
                    minutes = 0
                Case 3
                    heures = CInt(t.Substring(0, 1))
                    minutes = CInt(t.Substring(1))
                Case 4
                    heures = CInt(t.Substring(0, 2))
                    minutes = CInt(t.Substring(2))
                Case Else
                    Return Nothing
            End Select
        Else
            Return Nothing
        End If

        If heures > 23 OrElse minutes > 59 Then Return Nothing
        Return New TimeSpan(heures, minutes, 0)

    End Function

    ' Heure suivante (sens > 0) ou précédente sur le pas, à partir d'une heure donnée.
    Private Function HeureVoisine(depart As TimeSpan, sens As Integer) As TimeSpan
        Dim minutes As Integer = CInt(depart.TotalMinutes)
        Dim reste As Integer = minutes Mod _pas
        If sens > 0 Then
            minutes += _pas - reste
        Else
            minutes -= If(reste = 0, _pas, reste)
        End If
        Return Borner(TimeSpan.FromMinutes(Math.Max(0, Math.Min(24 * 60 - 1, minutes))))
    End Function

#End Region

#Region "Saisie"

    ' Seuls chiffres, séparateurs (convertis en « : ») et touches d'édition passent.
    Private Sub Saisie_KeyPress(sender As Object, e As KeyPressEventArgs)
        If Char.IsControl(e.KeyChar) Then Return
        If Char.IsDigit(e.KeyChar) Then Return
        If e.KeyChar = ":"c OrElse e.KeyChar = "h"c OrElse e.KeyChar = "H"c OrElse e.KeyChar = "."c Then
            If _saisie.Text.Contains(":"c) Then
                e.Handled = True
            Else
                e.KeyChar = ":"c
            End If
            Return
        End If
        e.Handled = True
    End Sub

    Private Sub Saisie_KeyDown(sender As Object, e As KeyEventArgs)

        If (e.Alt AndAlso e.KeyCode = Keys.Down) OrElse e.KeyCode = Keys.F4 Then
            OuvrirListe()
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            Dim base As TimeSpan = If(Interpreter(_saisie.Text), If(_valeur, Borner(HEURE_PAR_DEFAUT)))
            DefinirValeur(HeureVoisine(base, If(e.KeyCode = Keys.Up, 1, -1)), notifier:=True)
            _saisie.SelectAll()
            e.SuppressKeyPress = True
        End If

        OnKeyDown(e)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Saisie_TextChanged
    ' Rôle      : Place le « : » quand on tape à la fin du champ (après un chiffre d'heure de 3 à 9, ou
    '             après deux chiffres formant une heure), puis interprète la saisie : une heure complète
    '             (h:mm ou hh:mm) et valide devient la valeur ; une saisie complète impossible passe le
    '             contour en rose.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Saisie_TextChanged(sender As Object, e As EventArgs)

        If _miseAJour Then Return

        Dim texte As String = _saisie.Text
        Dim ajout As Boolean = texte.Length > _longueurPrecedente
        _longueurPrecedente = texte.Length

        If ajout AndAlso _saisie.SelectionStart = texte.Length AndAlso Not texte.Contains(":"c) AndAlso
           texte.All(AddressOf Char.IsDigit) AndAlso DeuxPointsAttendus(texte) Then
            _miseAJour = True
            _saisie.Text = texte & ":"
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

        ' Saisie complète : deux chiffres de minutes après le « : », ou quatre chiffres sans séparateur.
        Dim posDeuxPoints As Integer = texte.IndexOf(":"c)
        Dim complete As Boolean = (posDeuxPoints > 0 AndAlso texte.Length - posDeuxPoints - 1 = 2) OrElse
                                  (posDeuxPoints < 0 AndAlso texte.Length = 4)
        If Not complete Then
            _erreur = False
            Invalidate()
            Return
        End If

        Dim lue As TimeSpan? = Interpreter(texte)
        _erreur = Not lue.HasValue OrElse Not DansLesBornes(lue.Value)
        If Not _erreur AndAlso Not Nullable.Equals(lue, _valeur) Then
            _valeur = lue
            RaiseEvent ValeurChangee(Me, EventArgs.Empty)
        End If
        Invalidate()

    End Sub

    ' « 9 » → « 9: » (aucune heure à deux chiffres ne commence par 3 à 9) ; « 14 » → « 14: ».
    ' Friend : testé (Althea.Tests).
    Friend Shared Function DeuxPointsAttendus(chiffres As String) As Boolean
        If chiffres.Length = 1 Then Return chiffres(0) >= "3"c
        If chiffres.Length = 2 Then Return chiffres(0) <= "2"c AndAlso CInt(chiffres) <= 23
        Return False
    End Function

    ' À la sortie : saisie partielle interprétée (« 9 » → 09:00), sinon retour à la dernière heure valide.
    Private Sub Saisie_Leave(sender As Object, e As EventArgs)

        Dim texte As String = _saisie.Text.Trim()

        If texte.Length = 0 Then
            DefinirValeur(Nothing, notifier:=True)
            Return
        End If

        Dim lue As TimeSpan? = Interpreter(texte)
        If lue.HasValue AndAlso DansLesBornes(lue.Value) Then
            DefinirValeur(lue, notifier:=True)
        Else
            DefinirValeur(_valeur, notifier:=False)
        End If

    End Sub

#End Region

#Region "Liste des heures"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirListe
    ' Rôle      : Ouvre la liste des heures sous le champ, positionnée sur l'heure affichée.
    ' -------------------------------------------------------------------------------------------------
    Public Sub OuvrirListe()

        If _lectureSeule OrElse Not Enabled Then Return

        If _deroulant Is Nothing Then
            _liste = New ListeHeures()
            AddHandler _liste.HeureValidee, AddressOf Liste_HeureValidee
            AddHandler _liste.Abandon, AddressOf Liste_Abandon

            _hote = New ToolStripControlHost(_liste) With {.Margin = Padding.Empty, .Padding = Padding.Empty, .AutoSize = False}
            _deroulant = New ToolStripDropDown() With {.Padding = New Padding(1), .DropShadowEnabled = True, .BackColor = UITheme.ColorBeigeFonce}
            _deroulant.Items.Add(_hote)
        End If

        Dim heures As List(Of TimeSpan) = HeuresDeLaListe()
        If heures.Count = 0 Then Return

        _liste.Font = Font
        _liste.Remplir(heures, _heureReference, If(Interpreter(_saisie.Text), _valeur))
        _hote.Size = New Size(Math.Max(Width, LARGEUR_LISTE_MIN),
                              Math.Min(LIGNES_VISIBLES, heures.Count) * _liste.HauteurLigne + 6)

        _deroulant.Show(Me, New Point(0, Height + 2))
        _liste.CentrerSurSelection()
        _liste.Focus()

    End Sub

    ' Heures proposées (ou toute la plage au pas), à partir de l'heure de référence si elle existe.
    Private Function HeuresDeLaListe() As List(Of TimeSpan)

        Dim heures As IEnumerable(Of TimeSpan) = _heuresProposees
        If _heuresProposees.Count = 0 Then
            Dim plage As New List(Of TimeSpan)
            Dim h As TimeSpan = _heureMin
            While h <= _heureMax
                plage.Add(h)
                h = h.Add(TimeSpan.FromMinutes(_pas))
            End While
            heures = plage
        End If

        Return heures.Where(Function(h) DansLesBornes(h) AndAlso
                                        (Not _heureReference.HasValue OrElse h >= _heureReference.Value)).ToList()

    End Function

    Private Sub Liste_HeureValidee(sender As Object, heure As TimeSpan)
        _deroulant.Close()
        DefinirValeur(heure, notifier:=True)
        _saisie.Focus()
        _saisie.SelectAll()
    End Sub

    Private Sub Liste_Abandon(sender As Object, e As EventArgs)
        _deroulant.Close()
        _saisie.Focus()
    End Sub

#End Region

#Region "Mise en page et état"

    ' Boutons dessinés à droite : horloge, et croix si facultative, remplie et affichée.
    Private ReadOnly Property ZoneHorloge As Rectangle
        Get
            Return New Rectangle(Width - MARGE_DROITE - TAILLE_BOUTON, (Height - TAILLE_BOUTON) \ 2, TAILLE_BOUTON, TAILLE_BOUTON)
        End Get
    End Property

    Private ReadOnly Property ZoneCroix As Rectangle
        Get
            Return New Rectangle(ZoneHorloge.Left - TAILLE_BOUTON, ZoneHorloge.Top, TAILLE_BOUTON, TAILLE_BOUTON)
        End Get
    End Property

    Private ReadOnly Property BoutonsVisibles As Boolean
        Get
            Return Not _lectureSeule AndAlso Enabled
        End Get
    End Property

    Private ReadOnly Property CroixVisible As Boolean
        Get
            Return BoutonsVisibles AndAlso _facultative AndAlso _afficherCroix AndAlso _valeur.HasValue
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

        DessinerIconeHorloge(g, ZoneHorloge)
        If CroixVisible Then DessinerCroix(g, ZoneCroix)

    End Sub

    ' Horloge au trait du kit (même épaisseur et même couleur que le calendrier d'AltheaDate).
    Private Shared Sub DessinerIconeHorloge(g As Graphics, zone As Rectangle)

        Dim cx As Single = zone.Left + zone.Width / 2.0F
        Dim cy As Single = zone.Top + zone.Height / 2.0F
        Using stylo As New Pen(UITheme.ColorSaugeFonce, 1.7F) With {.StartCap = LineCap.Round, .EndCap = LineCap.Round}
            g.DrawEllipse(stylo, cx - 8.0F, cy - 8.0F, 16.0F, 16.0F)
            g.DrawLine(stylo, cx, cy - 4.5F, cx, cy)
            g.DrawLine(stylo, cx, cy, cx + 3.2F, cy + 2.2F)
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

        If ZoneHorloge.Contains(e.Location) Then
            OuvrirListe()
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
                                   (ZoneHorloge.Contains(e.Location) OrElse (CroixVisible AndAlso ZoneCroix.Contains(e.Location)))
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

#Region "Liste déroulante (classe interne)"

    ' -------------------------------------------------------------------------------------------------
    ' Classe : ListeHeures
    ' Rôle   : Liste des heures entièrement dessinée aux couleurs du kit : heure choisie en sauge,
    '          survol en vert de gris clair, durée depuis l'heure de référence alignée à droite en gris,
    '          ascenseur fin arrondi (glissable) à la place de la barre grise de Windows.
    '          Clic ou Entrée valident ; Échap abandonne ; Haut / Bas, Page préc. / suiv., Début / Fin
    '          et la molette déplacent la sélection ou font défiler.
    ' -------------------------------------------------------------------------------------------------
    <ToolboxItem(False)>
    <DesignerCategory("Code")>
    Private NotInheritable Class ListeHeures
        Inherits Control

        Private Const HAUTEUR_LIGNE As Integer = 28
        Private Const MARGE_TEXTE As Integer = 12
        Private Const LARGEUR_ASCENSEUR As Integer = 6
        Private Const MARGE_ASCENSEUR As Integer = 3
        Private Const HAUTEUR_CURSEUR_MIN As Integer = 24

        Public Event HeureValidee As EventHandler(Of TimeSpan)
        Public Event Abandon As EventHandler

        Private ReadOnly _heures As New List(Of TimeSpan)
        Private _reference As TimeSpan?
        Private _selection As Integer = -1
        Private _survol As Integer = -1
        Private _premiere As Integer                ' première ligne visible
        Private _glisse As Boolean                  ' curseur de l'ascenseur en cours de glissement
        Private _glisseDepartY As Integer
        Private _glissePremiereDepart As Integer

        Public Sub New()
            SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or
                     ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or
                     ControlStyles.Selectable, True)
            BackColor = Color.White
            ForeColor = UITheme.ColorGrisFonce
        End Sub

        Public ReadOnly Property HauteurLigne As Integer
            Get
                Return HAUTEUR_LIGNE
            End Get
        End Property

        ' Remplit la liste et sélectionne l'heure courante (ou la première heure qui la suit).
        Public Sub Remplir(heures As List(Of TimeSpan), reference As TimeSpan?, courante As TimeSpan?)

            _heures.Clear()
            _heures.AddRange(heures)
            _reference = reference
            _survol = -1
            _premiere = 0

            _selection = If(_heures.Count = 0, -1, 0)
            If courante.HasValue AndAlso _heures.Count > 0 Then
                _selection = _heures.FindIndex(Function(h) h >= courante.Value)
                If _selection < 0 Then _selection = _heures.Count - 1
            End If
            Invalidate()

        End Sub

        ' Place l'heure choisie vers le milieu de la liste.
        Public Sub CentrerSurSelection()
            If _selection >= 0 Then DefinirPremiere(_selection - LignesVisibles \ 2)
        End Sub

        Private ReadOnly Property LignesVisibles As Integer
            Get
                Return Math.Max(1, ClientSize.Height \ HAUTEUR_LIGNE)
            End Get
        End Property

        Private ReadOnly Property PremiereMax As Integer
            Get
                Return Math.Max(0, _heures.Count - LignesVisibles)
            End Get
        End Property

        Private ReadOnly Property AscenseurVisible As Boolean
            Get
                Return _heures.Count > LignesVisibles
            End Get
        End Property

        Private Sub DefinirPremiere(valeur As Integer)
            Dim bornee As Integer = Math.Max(0, Math.Min(PremiereMax, valeur))
            If bornee <> _premiere Then
                _premiere = bornee
                Invalidate()
            End If
        End Sub

        Private Sub DefinirSelection(index As Integer)
            If _heures.Count = 0 Then Return
            _selection = Math.Max(0, Math.Min(_heures.Count - 1, index))
            If _selection < _premiere Then DefinirPremiere(_selection)
            If _selection >= _premiere + LignesVisibles Then DefinirPremiere(_selection - LignesVisibles + 1)
            Invalidate()
        End Sub

        Private Function IndexSous(point As Point) As Integer
            If ZoneLignes.Contains(point) = False Then Return -1
            Dim index As Integer = _premiere + point.Y \ HAUTEUR_LIGNE
            Return If(index < _heures.Count, index, -1)
        End Function

        ' Zones : les lignes, puis le rail de l'ascenseur à droite (s'il y a lieu).
        Private ReadOnly Property ZoneLignes As Rectangle
            Get
                Dim largeur As Integer = ClientSize.Width - If(AscenseurVisible, LARGEUR_ASCENSEUR + 2 * MARGE_ASCENSEUR, 0)
                Return New Rectangle(0, 0, largeur, ClientSize.Height)
            End Get
        End Property

        Private ReadOnly Property ZoneRail As Rectangle
            Get
                Return New Rectangle(ClientSize.Width - LARGEUR_ASCENSEUR - MARGE_ASCENSEUR, MARGE_ASCENSEUR,
                                     LARGEUR_ASCENSEUR, ClientSize.Height - 2 * MARGE_ASCENSEUR)
            End Get
        End Property

        Private ReadOnly Property ZoneCurseur As Rectangle
            Get
                Dim rail As Rectangle = ZoneRail
                Dim hauteur As Integer = Math.Max(HAUTEUR_CURSEUR_MIN, rail.Height * LignesVisibles \ Math.Max(1, _heures.Count))
                Dim course As Integer = rail.Height - hauteur
                Dim y As Integer = rail.Top + If(PremiereMax = 0, 0, course * _premiere \ PremiereMax)
                Return New Rectangle(rail.Left, y, rail.Width, hauteur)
            End Get
        End Property

#Region "Dessin"

        Protected Overrides Sub OnPaint(e As PaintEventArgs)

            Dim g As Graphics = e.Graphics
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.Clear(BackColor)

            Dim lignes As Rectangle = ZoneLignes
            Using petite As New Font(Font.FontFamily, Font.Size * 0.9F)
                Using grasse As New Font(Font, FontStyle.Bold)
                    For i As Integer = _premiere To Math.Min(_heures.Count - 1, _premiere + LignesVisibles)
                        Dim y As Integer = (i - _premiere) * HAUTEUR_LIGNE
                        If y >= lignes.Height Then Exit For
                        DessinerLigne(g, i, New Rectangle(0, y, lignes.Width, HAUTEUR_LIGNE), petite, grasse)
                    Next
                End Using
            End Using

            If AscenseurVisible Then
                Using rail As GraphicsPath = AltheaDessin.CheminArrondi(ZoneRail, LARGEUR_ASCENSEUR / 2.0F)
                    Using pinceau As New SolidBrush(UITheme.ColorBeige)
                        g.FillPath(pinceau, rail)
                    End Using
                End Using
                Using curseur As GraphicsPath = AltheaDessin.CheminArrondi(ZoneCurseur, LARGEUR_ASCENSEUR / 2.0F)
                    Using pinceau As New SolidBrush(If(_glisse, UITheme.ColorSauge, UITheme.ColorSaugeClair))
                        g.FillPath(pinceau, curseur)
                    End Using
                End Using
            End If

        End Sub

        Private Sub DessinerLigne(g As Graphics, index As Integer, zone As Rectangle, petite As Font, grasse As Font)

            Dim choisie As Boolean = index = _selection
            If choisie OrElse index = _survol Then
                Dim fond As New RectangleF(zone.X + 3, zone.Y + 2, zone.Width - 6, zone.Height - 4)
                Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(fond, 6.0F)
                    Using pinceau As New SolidBrush(If(choisie, UITheme.ColorSauge, UITheme.ColorGrisVertClair))
                        g.FillPath(pinceau, chemin)
                    End Using
                End Using
            End If

            Dim texteZone As New Rectangle(zone.X + MARGE_TEXTE, zone.Y, zone.Width - 2 * MARGE_TEXTE, zone.Height)
            TextRenderer.DrawText(g, _heures(index).ToString(FORMAT_HEURE, CultureInfo.InvariantCulture),
                                  If(choisie, grasse, Font), texteZone,
                                  If(choisie, Color.White, UITheme.ColorGrisFonce),
                                  TextFormatFlags.VerticalCenter Or TextFormatFlags.Left Or TextFormatFlags.NoPrefix)

            Dim duree As String = TexteDuree(_heures(index))
            If duree IsNot Nothing Then
                TextRenderer.DrawText(g, duree, petite, texteZone,
                                      If(choisie, Color.White, UITheme.ColorGrisDiscret),
                                      TextFormatFlags.VerticalCenter Or TextFormatFlags.Right Or TextFormatFlags.NoPrefix)
            End If

        End Sub

        ' « début », « 45 min », « 1 h », « 1 h 15 » depuis l'heure de référence ; Nothing sans référence.
        Private Function TexteDuree(heure As TimeSpan) As String
            If Not _reference.HasValue Then Return Nothing
            Dim minutes As Integer = CInt((heure - _reference.Value).TotalMinutes)
            If minutes <= 0 Then Return "début"
            If minutes < 60 Then Return $"{minutes} min"
            Dim reste As Integer = minutes Mod 60
            Return If(reste = 0, $"{minutes \ 60} h", $"{minutes \ 60} h {reste:00}")
        End Function

#End Region

#Region "Souris"

        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
            MyBase.OnMouseDown(e)
            If e.Button <> MouseButtons.Left OrElse Not AscenseurVisible Then Return
            Dim rail As Rectangle = ZoneRail
            rail.Inflate(MARGE_ASCENSEUR, 0)
            If Not rail.Contains(e.Location) Then Return
            If ZoneCurseur.Contains(e.Location) Then
                _glisse = True
                _glisseDepartY = e.Y
                _glissePremiereDepart = _premiere
                Invalidate()
            Else
                ' Clic sur le rail : une page vers le haut ou vers le bas.
                DefinirPremiere(_premiere + If(e.Y < ZoneCurseur.Top, -LignesVisibles, LignesVisibles))
            End If
        End Sub

        Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
            MyBase.OnMouseMove(e)
            If _glisse Then
                Dim course As Integer = ZoneRail.Height - ZoneCurseur.Height
                If course > 0 Then DefinirPremiere(_glissePremiereDepart + (e.Y - _glisseDepartY) * PremiereMax \ course)
                Return
            End If
            Dim index As Integer = IndexSous(e.Location)
            If index <> _survol Then
                _survol = index
                Invalidate()
            End If
        End Sub

        Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
            MyBase.OnMouseUp(e)
            If _glisse Then
                _glisse = False
                Invalidate()
                Return
            End If
            If e.Button <> MouseButtons.Left Then Return
            Dim index As Integer = IndexSous(e.Location)
            If index >= 0 Then RaiseEvent HeureValidee(Me, _heures(index))
        End Sub

        Protected Overrides Sub OnMouseLeave(e As EventArgs)
            MyBase.OnMouseLeave(e)
            _survol = -1
            Invalidate()
        End Sub

        Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
            MyBase.OnMouseWheel(e)
            DefinirPremiere(_premiere - Math.Sign(e.Delta) * 3)
        End Sub

#End Region

#Region "Clavier"

        Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
            Select Case keyData
                Case Keys.Up, Keys.Down, Keys.PageUp, Keys.PageDown, Keys.Home, Keys.End, Keys.Enter, Keys.Escape
                    Return True
            End Select
            Return MyBase.IsInputKey(keyData)
        End Function

        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
            Select Case e.KeyCode
                Case Keys.Up : DefinirSelection(_selection - 1)
                Case Keys.Down : DefinirSelection(_selection + 1)
                Case Keys.PageUp : DefinirSelection(_selection - LignesVisibles)
                Case Keys.PageDown : DefinirSelection(_selection + LignesVisibles)
                Case Keys.Home : DefinirSelection(0)
                Case Keys.End : DefinirSelection(_heures.Count - 1)
                Case Keys.Enter
                    If _selection >= 0 Then RaiseEvent HeureValidee(Me, _heures(_selection))
                Case Keys.Escape
                    RaiseEvent Abandon(Me, EventArgs.Empty)
                Case Else
                    MyBase.OnKeyDown(e)
                    Return
            End Select
            e.Handled = True
        End Sub

#End Region

    End Class

#End Region

End Class
