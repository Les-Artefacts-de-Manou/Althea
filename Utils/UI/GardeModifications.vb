' -------------------------------------------------------------------------------------------------
' Classe     : GardeModifications
' Projet     : Althéa
' Version    : V1.2.0
' Date       : 20/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Repère les modifications non enregistrées d'un écran ou d'une fenêtre, pour prévenir avant de
' les perdre (« Enregistrer / Quitter sans enregistrer / Rester »).
'
' Responsabilités :
' - Prendre une « photo » des valeurs saisissables au chargement (Photographier) et dire si elles
'   ont changé depuis (EstModifie).
' - Pour une fenêtre (SurveillerFenetre) : photo à l'affichage, contrôle à la fermeture, dialogue
'   à trois choix ; « Enregistrer » déclenche le bouton d'enregistrement de la fenêtre.
' - Pour un UserControl : PeutQuitter (question à trois choix avant de quitter) et ConfirmerAnnulation
'   (bouton Annuler), pour des comportements identiques dans toute l'application.
'
' Remarques  :
' - Demande de Joëlle (19/10/2026, recommandations acceptées) : l'alerte appartient à l'écran et
'   couvre tous ses champs ; l'éditeur de texte n'est qu'un champ parmi d'autres.
' - Comparer une photo plutôt que suivre les événements de chaque champ : rien à brancher champ par
'   champ, donc rien à oublier quand un champ est ajouté ; une valeur remise à l'identique n'est
'   plus une modification.
' - Champs lus : zones de texte modifiables, listes, cases, boutons radio, NumericUpDown,
'   DateTimePicker, AltheaDate, AltheaHeure, UC_RichTextEditor (son RTF), grilles modifiables
'   (valeurs des cellules), listes à cocher (cases cochées). Ignorés : champs de recherche (AltheaRecherche), grilles et zones de
'   texte en lecture seule, contrôles exclus.
' - La photo reste en mémoire (jamais journalisée) : elle peut contenir des données de patient.
' - V1.1.0 (19/10/2026) : listes à cocher (CheckedListBox : usages d'un texte type).
' - V1.2.0 (20/10/2026) : AjouterValeur (valeur tenue par l'écran : logo d'un domaine en attente).
'
' Dépendances :
' - DialogChoix (DemanderAvantDepart), AltheaDate, AltheaHeure, AltheaRecherche, UC_RichTextEditor
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Globalization
Imports System.Text

Public NotInheritable Class GardeModifications

#Region "Variables privées"

    Private ReadOnly _racine As Control
    Private ReadOnly _exclus As HashSet(Of Control)

    ' Valeurs que l'écran ajoute lui-même à la photo (état sans champ : image choisie…).
    Private ReadOnly _valeursEcran As New List(Of KeyValuePair(Of String, Func(Of String)))

    ' Photo de référence ; Nothing = rien à surveiller (consultation, ou modifications abandonnées).
    Private _reference As String

#End Region

#Region "Construction"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur
    ' Paramètres :
    ' - racine : Conteneur dont les champs sont surveillés (fenêtre, onglets…).
    ' - exclus : Conteneurs ou champs à ignorer (onglets de listes, filtres…).
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(racine As Control, ParamArray exclus As Control())
        _racine = racine
        _exclus = New HashSet(Of Control)(If(exclus, Array.Empty(Of Control)()))
    End Sub

#End Region

#Region "Photo et comparaison"

    ' Prend la photo de référence (valeurs actuelles = état enregistré).
    Public Sub Photographier()
        _reference = Instantane()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : PhotographierApresChargement
    ' Rôle       : Prend la photo une fois les traitements du chargement terminés (valeurs par défaut,
    '              listes remplies, calculs déclenchés) : elle est différée par la file des messages.
    ' -------------------------------------------------------------------------------------------------
    Public Sub PhotographierApresChargement()

        If _racine.IsHandleCreated AndAlso Not _racine.IsDisposed Then
            _racine.BeginInvoke(New Action(AddressOf Photographier))
        Else
            Photographier()
        End If

    End Sub

    ' Plus rien à surveiller (retour en consultation, modifications abandonnées ou enregistrées).
    Public Sub Oublier()
        _reference = Nothing
    End Sub

    ' Des valeurs ont changé depuis la photo.
    Public ReadOnly Property EstModifie As Boolean
        Get
            Return _reference IsNot Nothing AndAlso
                   Not String.Equals(_reference, Instantane(), StringComparison.Ordinal)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutQuitter
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Avant de quitter un écran (IUnsavedChangesAware.PeutQuitter) : rien à perdre, ou
    '              Enregistrer / Quitter sans enregistrer / Rester.
    ' Paramètres :
    ' - deQuoi      : Ce qui serait perdu, précédé de « de ».
    ' - enregistrer : Enregistre la saisie ; True si l'enregistrement a abouti.
    ' - abandonner  : Facultatif : remet l'écran dans son état enregistré (« Quitter sans enregistrer »).
    ' Retour     : True si l'on peut quitter.
    ' -------------------------------------------------------------------------------------------------
    Public Function PeutQuitter(deQuoi As String, enregistrer As Func(Of Boolean), Optional abandonner As Action = Nothing) As Boolean

        If Not EstModifie Then Return True

        Select Case DialogChoix.DemanderAvantDepart(deQuoi)
            Case ChoixAvantDepart.Enregistrer
                Return enregistrer()
            Case ChoixAvantDepart.QuitterSansEnregistrer
                Oublier()
                abandonner?.Invoke()
                Return True
            Case Else
                Return False
        End Select

    End Function

    ' Bouton Annuler : True s'il n'y a rien à perdre ou si l'utilisateur confirme l'abandon.
    Public Function ConfirmerAnnulation(deQuoi As String) As Boolean
        Return Not EstModifie OrElse DialogChoix.ConfirmerAbandon(deQuoi)
    End Function

#End Region

#Region "Fenêtres"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : SurveillerFenetre
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Branche la garde sur une fenêtre de saisie : photo à l'affichage (Shown), contrôle à la
    ' fermeture (Annuler, croix, Échap…).
    '
    ' Paramètres :
    ' - fenetre           : La fenêtre (appel dans son constructeur ou son Load).
    ' - boutonEnregistrer : Son bouton Enregistrer : la garde n'agit que s'il est visible et actif
    '                       (une fenêtre en consultation n'a rien à perdre), et « Enregistrer » le
    '                       déclenche.
    ' - deQuoi            : Ce qui serait perdu, pour le message (« de la séance », « du contact »…).
    '
    ' Remarques  :
    ' - Fermeture après un enregistrement réussi (DialogResult = OK) : pas de question.
    ' - « Enregistrer » : la fermeture est annulée puis le bouton est déclenché ; s'il réussit, il
    '   ferme la fenêtre lui-même (OK) ; si la saisie est refusée, la fenêtre reste ouverte avec le
    '   message habituel.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function SurveillerFenetre(fenetre As Form, boutonEnregistrer As Button, deQuoi As String) As GardeModifications

        Dim garde As New GardeModifications(fenetre)

        AddHandler fenetre.Shown, Sub(s, e) garde.PhotographierApresChargement()
        AddHandler fenetre.FormClosing, Sub(s, e) garde.ControlerFermeture(fenetre, boutonEnregistrer, deQuoi, e)

        Return garde

    End Function

    Private Sub ControlerFermeture(fenetre As Form, boutonEnregistrer As Button, deQuoi As String, e As FormClosingEventArgs)

        If e.Cancel OrElse fenetre.DialogResult = DialogResult.OK Then Return
        If Not boutonEnregistrer.Visible OrElse Not boutonEnregistrer.Enabled Then Return
        If Not EstModifie Then Return

        Select Case DialogChoix.DemanderAvantDepart(deQuoi)
            Case ChoixAvantDepart.Enregistrer
                e.Cancel = True
                fenetre.BeginInvoke(New Action(Sub() boutonEnregistrer.PerformClick()))
            Case ChoixAvantDepart.QuitterSansEnregistrer
                Oublier()
            Case Else
                e.Cancel = True
        End Select

    End Sub

#End Region

#Region "Lecture des valeurs"

    ' Valeurs saisissables du conteneur, une ligne par champ (« parent/nom=valeur »).
    Private Function Instantane() As String

        Dim photo As New StringBuilder()
        Lire(_racine, photo)

        For Each valeur In _valeursEcran
            photo.Append("ecran:").Append(valeur.Key).Append("="c).Append(valeur.Value()).Append(ControlChars.Lf)
        Next

        Return photo.ToString()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterValeur
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Ajoute à la photo une valeur tenue par l'écran, hors des champs (ex. logo choisi
    '              mais pas encore enregistré, UC_DomainesHome).
    ' Remarques  : Opt-in : les images des autres écrans (photo patient, cabinet), enregistrées
    '              aussitôt, ne doivent pas compter comme modifications.
    ' -------------------------------------------------------------------------------------------------
    Public Sub AjouterValeur(nom As String, lecture As Func(Of String))
        ArgumentNullException.ThrowIfNull(lecture)
        _valeursEcran.Add(New KeyValuePair(Of String, Func(Of String))(nom, lecture))
    End Sub

    Private Sub Lire(parent As Control, photo As StringBuilder)

        For Each champ As Control In parent.Controls

            If _exclus.Contains(champ) Then Continue For

            Dim valeur As String

            Select Case True
                Case TypeOf champ Is AltheaRecherche
                    Continue For
                Case TypeOf champ Is DataGridView
                    Dim grille As DataGridView = DirectCast(champ, DataGridView)
                    If grille.ReadOnly Then Continue For
                    valeur = ValeursGrille(grille)
                Case TypeOf champ Is UC_RichTextEditor
                    valeur = DirectCast(champ, UC_RichTextEditor).RtfContent
                Case TypeOf champ Is AltheaDate
                    Dim jour As Date? = DirectCast(champ, AltheaDate).Valeur
                    valeur = If(jour.HasValue, jour.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), String.Empty)
                Case TypeOf champ Is AltheaHeure
                    Dim heure As TimeSpan? = DirectCast(champ, AltheaHeure).Valeur
                    valeur = If(heure.HasValue, heure.Value.ToString("c", CultureInfo.InvariantCulture), String.Empty)
                Case TypeOf champ Is TextBoxBase
                    Dim zone As TextBoxBase = DirectCast(champ, TextBoxBase)
                    If zone.ReadOnly Then Continue For
                    valeur = zone.Text
                Case TypeOf champ Is CheckedListBox
                    valeur = String.Join(",", DirectCast(champ, CheckedListBox).CheckedIndices.Cast(Of Integer)())
                Case TypeOf champ Is ComboBox
                    Dim liste As ComboBox = DirectCast(champ, ComboBox)
                    valeur = $"{liste.SelectedValue}|{liste.Text}"
                Case TypeOf champ Is CheckBox
                    valeur = DirectCast(champ, CheckBox).CheckState.ToString()
                Case TypeOf champ Is RadioButton
                    valeur = DirectCast(champ, RadioButton).Checked.ToString()
                Case TypeOf champ Is NumericUpDown
                    valeur = DirectCast(champ, NumericUpDown).Value.ToString(CultureInfo.InvariantCulture)
                Case TypeOf champ Is DateTimePicker
                    Dim selecteur As DateTimePicker = DirectCast(champ, DateTimePicker)
                    valeur = $"{selecteur.Checked}|{selecteur.Value.ToString("o", CultureInfo.InvariantCulture)}"
                Case Else
                    ' Conteneur (carte, panneau, onglets…) : on lit ses champs.
                    Lire(champ, photo)
                    Continue For
            End Select

            photo.Append(parent.Name).Append("/"c).Append(champ.Name).Append("="c).Append(valeur).Append(ControlChars.Lf)

        Next

    End Sub

    ' Valeurs d'une grille modifiable (ex. plafonds d'une règle de quota), ligne par ligne. La cellule
    ' en cours de saisie compte pour sa valeur affichée.
    Private Shared Function ValeursGrille(grille As DataGridView) As String

        Dim valeurs As New StringBuilder()

        For Each ligne As DataGridViewRow In grille.Rows
            If ligne.IsNewRow Then Continue For
            For Each cellule As DataGridViewCell In ligne.Cells
                Dim valeur As Object = If(cellule.IsInEditMode, cellule.EditedFormattedValue, cellule.Value)
                valeurs.Append(Convert.ToString(valeur, CultureInfo.InvariantCulture)).Append(ControlChars.Tab)
            Next
            valeurs.Append("¶"c)
        Next

        Return valeurs.ToString()

    End Function

#End Region

End Class
