' -------------------------------------------------------------------------------------------------
' UserControl : UC_Pagination
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 19/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Barre de pagination réutilisable pour un DataGridView lié à une liste chargée en mémoire.
'
' Responsabilités :
' - Conserver la liste complète et n'afficher dans la grille que la page courante
' - Calculer le nombre de lignes par page selon la hauteur visible de la grille
' - Offrir la navigation première / précédente / suivante / dernière page
' - Trier la liste complète (et non la seule page) lors d'un clic sur un en-tête de colonne
'
' Remarques   :
' - Toute nouvelle source (filtre, recherche, actualisation) ramène à la page 1, sauf demande contraire.
' - Un tri ramène à la page 1.
' - Un redimensionnement conserve le premier élément visible dans la page affichée.
' - V1.1.0 (19/10/2026, refonte) : boutons du kit (AltheaBouton Outil, icônes 24 px du Designer) au
'   lieu de boutons Windows à caractères ; plus d'UtilsButtons (il journalisait « Tag invalide » à
'   chaque écran) ; résumé au singulier ou au pluriel (« 1 patient », « Aucun patient ») ; boutons et
'   numéro de page masqués quand tout tient sur une page (le résumé reste) ; taille de page : place
'   réservée par l'écran à ses repères (HauteurReservee) et ascenseur horizontal retiré seulement
'   s'il est affiché ; tri par en-tête facultatif (Attacher, triable).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On
Option Infer On

Imports System.Collections

Public Class UC_Pagination

#Region "Déclarations"

    ' Nombre minimal de lignes par page, quelle que soit la hauteur de la grille.
    Private Const LignesMinimum As Integer = 5

    ' Marque du pluriel dans le libellé fourni par l'écran (« patient(s) »).
    Private Const MarquePluriel As String = "(s)"

    Private _grille As DataGridView
    Private _source As IList = New ArrayList()
    Private _taillePage As Integer = LignesMinimum
    Private _pageCourante As Integer = 1
    Private _libelleSingulier As String = "élément"
    Private _libellePluriel As String = "éléments"
    Private _hauteurReservee As Integer = 0

    ' Déclenché après l'affichage d'une nouvelle page dans la grille.
    Public Event PageChangee As EventHandler

#End Region

#Region "Propriétés"

    ' Page actuellement affichée (base 1).
    Public ReadOnly Property PageCourante As Integer
        Get
            Return _pageCourante
        End Get
    End Property

    ' Nombre total de pages (au moins 1).
    Public ReadOnly Property NombrePages As Integer
        Get
            If _source.Count = 0 Then Return 1
            Return CInt(Math.Ceiling(_source.Count / CDbl(_taillePage)))
        End Get
    End Property

    ' Nombre de lignes par page calculé pour la hauteur actuelle de la grille.
    Public ReadOnly Property TaillePage As Integer
        Get
            Return _taillePage
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : HauteurReservee
    ' Version   : V1.0.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Hauteur (px) que l'écran réserve dans une page à ses repères dessinés sous une ligne
    ' (DividerHeight : « Aujourd'hui », groupes de la liste Patients). Sans elle, une page qui
    ' contient un repère déborde d'une ligne (ascenseur ou dernière ligne coupée).
    ' -------------------------------------------------------------------------------------------------
    <System.ComponentModel.Browsable(False)>
    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property HauteurReservee As Integer
        Get
            Return _hauteurReservee
        End Get
        Set(value As Integer)
            _hauteurReservee = Math.Max(0, value)
            If _grille IsNot Nothing Then RecalculerTaillePage()
        End Set
    End Property

#End Region

#Region "API publique"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Attacher
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Relie la barre de pagination à une grille.
    '
    ' Paramètres :
    ' - grille          : DataGridView à paginer
    ' - libelleElements : Libellé des éléments dans le résumé, marque du pluriel comprise
    '                     (ex : "patient(s)" → « 1 patient », « 12 patients »)
    ' - triable         : False pour une grille dont l'ordre porte le sens (pas de tri par en-tête)
    '
    ' Remarques :
    ' - Branche le tri par en-tête sur la liste complète (UtilsDataGrid).
    ' - Recalcule la taille de page à chaque redimensionnement de la grille.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Attacher(grille As DataGridView, libelleElements As String, Optional triable As Boolean = True)

        If grille Is Nothing Then Return

        If _grille IsNot Nothing Then
            RemoveHandler _grille.Resize, AddressOf Grille_Resize
        End If

        _grille = grille
        DefinirLibelles(libelleElements)

        AddHandler _grille.Resize, AddressOf Grille_Resize

        If triable Then
            UtilsDataGrid.ActiverTriColonnes(_grille)
            UtilsDataGrid.DefinirSourceTri(_grille, Function() _source, Sub() AfficherPage(1))
        End If

        _taillePage = CalculerTaillePage()
        MettreAJourBarre()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirSource
    ' Version   : V1.0.0
    ' Date      : 22/07/2026
    '
    ' Rôle      :
    ' Remplace la liste complète et affiche la page demandée (page 1 par défaut).
    '
    ' Paramètres :
    ' - elements : Liste complète (déjà filtrée) à paginer
    ' - page     : Page à afficher (ramenée dans les bornes valides)
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirSource(elements As IList, Optional page As Integer = 1)

        _source = If(elements, New ArrayList())
        AfficherPage(page)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : PageDeIndex
    ' Version  : V1.0.0
    ' Date     : 19/10/2026
    '
    ' Rôle     :
    ' Page (base 1) qui contient l'élément de rang donné (base 0) dans la liste complète : pour
    ' ouvrir une grille sur la page utile (ex. la ligne « Aujourd'hui » des séances).
    ' -------------------------------------------------------------------------------------------------
    Public Function PageDeIndex(index As Integer) As Integer
        If index < 0 Then Return 1
        Return index \ _taillePage + 1
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherPage
    ' Version   : V1.0.0
    ' Date      : 22/07/2026
    '
    ' Rôle      :
    ' Lie à la grille la tranche de la liste correspondant à la page demandée.
    ' -------------------------------------------------------------------------------------------------
    Public Sub AfficherPage(page As Integer)

        If _grille Is Nothing Then Return

        _pageCourante = Math.Max(1, Math.Min(page, NombrePages))

        Dim debut As Integer = (_pageCourante - 1) * _taillePage
        Dim fin As Integer = Math.Min(debut + _taillePage, _source.Count)

        Dim tranche As New ArrayList()
        For i As Integer = debut To fin - 1
            tranche.Add(_source(i))
        Next

        _grille.DataSource = Nothing
        _grille.DataSource = tranche
        _grille.ClearSelection()

        MettreAJourBarre()

        RaiseEvent PageChangee(Me, EventArgs.Empty)

    End Sub

#End Region

#Region "Calculs et affichage"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CalculerTaillePage
    ' Version  : V1.1.0
    ' Date     : 19/10/2026
    '
    ' Rôle     :
    ' Nombre de lignes entièrement visibles dans la zone de données de la grille, sans ascenseur
    ' vertical.
    '
    ' Remarques :
    ' - V1.1.0 : la place des repères de l'écran (HauteurReservee) est retirée ; celle de l'ascenseur
    '   horizontal seulement s'il est affiché (colonnes en Fill : jamais), au lieu de toujours.
    ' -------------------------------------------------------------------------------------------------
    Private Function CalculerTaillePage() As Integer

        If _grille Is Nothing Then Return LignesMinimum

        Dim hauteurLigne As Integer = Math.Max(1, _grille.RowTemplate.Height)
        Dim entete As Integer = If(_grille.ColumnHeadersVisible, _grille.ColumnHeadersHeight, 0)
        Dim ascenseur As Integer = If(AscenseurHorizontalAffiche(), SystemInformation.HorizontalScrollBarHeight, 0)
        Dim hauteurUtile As Integer = _grille.ClientSize.Height - entete - ascenseur - _hauteurReservee

        Return Math.Max(LignesMinimum, hauteurUtile \ hauteurLigne)

    End Function

    ' La grille montre-t-elle son ascenseur horizontal ? (contrôle enfant de la grille)
    Private Function AscenseurHorizontalAffiche() As Boolean
        For Each controle As Control In _grille.Controls
            If TypeOf controle Is HScrollBar AndAlso controle.Visible Then Return True
        Next
        Return False
    End Function

    ' Nouvelle taille de page (hauteur ou place réservée changée) : garde le premier élément affiché.
    Private Sub RecalculerTaillePage()

        Dim nouvelleTaille As Integer = CalculerTaillePage()
        If nouvelleTaille = _taillePage Then Return

        Dim premierIndex As Integer = (_pageCourante - 1) * _taillePage
        _taillePage = nouvelleTaille

        AfficherPage(premierIndex \ _taillePage + 1)

    End Sub

    ' « patient(s) » → « patient » / « patients » ; un libellé sans marque reste tel quel.
    Private Sub DefinirLibelles(libelleElements As String)

        If String.IsNullOrWhiteSpace(libelleElements) Then Return

        Dim libelle As String = libelleElements.Trim()
        If libelle.EndsWith(MarquePluriel, StringComparison.Ordinal) Then
            _libelleSingulier = libelle.Substring(0, libelle.Length - MarquePluriel.Length)
            _libellePluriel = _libelleSingulier & "s"
        Else
            _libelleSingulier = libelle
            _libellePluriel = libelle
        End If

    End Sub

    Private Sub MettreAJourBarre()

        Dim total As Integer = _source.Count
        Dim pages As Integer = NombrePages

        lblPage.Text = $"Page {_pageCourante} / {pages}"

        If total = 0 Then
            lblResume.Text = $"Aucun {_libelleSingulier}"
        ElseIf pages = 1 Then
            lblResume.Text = $"{total} {If(total = 1, _libelleSingulier, _libellePluriel)}"
        Else
            Dim debut As Integer = (_pageCourante - 1) * _taillePage + 1
            Dim fin As Integer = Math.Min(_pageCourante * _taillePage, total)
            lblResume.Text = $"{debut}–{fin} sur {total} {_libellePluriel}"
        End If

        ' Tout tient sur une page : la navigation n'a pas d'objet (le résumé reste). La hauteur de la
        ' barre ne change pas, donc la taille de page non plus.
        Dim navigation As Boolean = pages > 1
        For Each controle As Control In New Control() {btnPremiere, btnPrecedente, lblPage, btnSuivante, btnDerniere}
            controle.Visible = navigation
        Next

        btnPremiere.Enabled = _pageCourante > 1
        btnPrecedente.Enabled = _pageCourante > 1
        btnSuivante.Enabled = _pageCourante < pages
        btnDerniere.Enabled = _pageCourante < pages

    End Sub

#End Region

#Region "Événements"

    Private Sub Grille_Resize(sender As Object, e As EventArgs)
        RecalculerTaillePage()
    End Sub

    Private Sub btnPremiere_Click(sender As Object, e As EventArgs) Handles btnPremiere.Click
        AfficherPage(1)
    End Sub

    Private Sub btnPrecedente_Click(sender As Object, e As EventArgs) Handles btnPrecedente.Click
        AfficherPage(_pageCourante - 1)
    End Sub

    Private Sub btnSuivante_Click(sender As Object, e As EventArgs) Handles btnSuivante.Click
        AfficherPage(_pageCourante + 1)
    End Sub

    Private Sub btnDerniere_Click(sender As Object, e As EventArgs) Handles btnDerniere.Click
        AfficherPage(NombrePages)
    End Sub

#End Region

End Class
