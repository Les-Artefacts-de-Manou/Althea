' -------------------------------------------------------------------------------------------------
' Classe     : CorrecteurTexte
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 20/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Correcteur d'orthographe de Windows dans une zone de texte (RichTextBox) : soulignement dans la
' langue du clavier, et lignes du clic droit sur un mot inconnu (suggestions, « Ajouter au
' dictionnaire »).
'
' Responsabilités :
' - Partagé : réglage EDITEUR_CORRECTEUR_ORTHOGRAPHE (ReglagePermis), activation (Appliquer),
'   langue du clavier (LangueClavier, EtiquetteLangue).
' - Instance : lignes du mot sous le pointeur, ajoutées en tête du menu du clic droit à son
'   ouverture (Preparer) et retirées à l'ouverture suivante.
'
' Remarques  :
' - Un seul comportement pour l'éditeur riche (UC_RichTextEditor) et pour les zones de texte
'   simples (AltheaZoneTexte) : code sorti de UC_RichTextEditor V2.4.0 (20/10/2026).
' - Windows ne laisse pas enrichir son propre menu : le menu du contrôle le remplace, d'où ces
'   lignes (CorrecteurWindows).
' - Le remplacement par une suggestion est confié au contrôle (remplacerSelection) : l'éditeur
'   riche le fait dans son historique et signale la modification.
' - Pas de « Ignorer » : il ne valait que pour notre vérification, pas pour le soulignement du
'   contrôle (un mot retapé était souligné sans correction proposée) ; résultat incohérent.
' - Aucun mot n'est journalisé (il peut venir d'une note).
'
' Dépendances :
' - CorrecteurWindows, RichTextEditorHelper, GestionParametres / ClesParametres, DialogChoix
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public NotInheritable Class CorrecteurTexte
    Implements IDisposable

#Region "Constantes et variables"

    ' Suggestions proposées au clic droit, au plus.
    Private Const NB_SUGGESTIONS As Integer = 5

    Private ReadOnly _zone As RichTextBox
    Private ReadOnly _menu As ContextMenuStrip
    Private ReadOnly _remplacerSelection As Action(Of String)

    ' Lignes ajoutées au menu pour le mot souligné (retirées à chaque ouverture).
    Private ReadOnly _lignes As New List(Of ToolStripItem)()
    Private _policeSuggestion As Font

#End Region

#Region "Initialisation"

    ' zone : texte vérifié ; menu : menu du clic droit de la zone ; remplacerSelection : remplace le
    ' mot sélectionné par la suggestion choisie (chaque contrôle à sa manière).
    Public Sub New(zone As RichTextBox, menu As ContextMenuStrip, remplacerSelection As Action(Of String))

        ArgumentNullException.ThrowIfNull(zone)
        ArgumentNullException.ThrowIfNull(menu)
        ArgumentNullException.ThrowIfNull(remplacerSelection)

        _zone = zone
        _menu = menu
        _remplacerSelection = remplacerSelection

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Retirer()
        _policeSuggestion?.Dispose()
        _policeSuggestion = Nothing
    End Sub

#End Region

#Region "Partagé : réglage, activation, langue"

    ' Réglage de Paramètres : le correcteur est-il permis dans Althéa ? (à lire hors du Designer)
    Public Shared Function ReglagePermis() As Boolean
        Return GestionParametres.GetBooleen(ClesParametres.EditeurCorrecteurOrthographe,
                                            ClesParametres.DefautEditeurCorrecteurOrthographe)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Appliquer
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Active ou coupe le soulignement ; à l'activation, le texte prend la langue du
    '              clavier (le RTF du contrôle le marque en anglais).
    ' Remarques  : Changer la langue n'est pas une modification du texte : Modified est gardé.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub Appliquer(zone As RichTextBox, actif As Boolean)

        If zone Is Nothing Then Return

        Dim modifie As Boolean = zone.Modified
        RichTextEditorHelper.DefinirCorrecteurOrthographe(zone, actif)
        If actif Then RichTextEditorHelper.DefinirLangueTexte(zone, LangueClavier())
        zone.Modified = modifie

    End Sub

    ' Langue du clavier actif (identifiant Windows) ; 0 si elle ne peut être lue.
    Public Shared Function LangueClavier() As Integer
        Try
            Return InputLanguage.CurrentInputLanguage.Culture.LCID
        Catch ex As ArgumentException
            Return 0
        End Try
    End Function

    ' Langue du clavier en étiquette (« fr-BE ») pour le correcteur de Windows ; à défaut, celle du
    ' format régional.
    Public Shared Function EtiquetteLangue() As String
        Try
            Return InputLanguage.CurrentInputLanguage.Culture.Name
        Catch ex As ArgumentException
            Return Globalization.CultureInfo.CurrentCulture.Name
        End Try
    End Function

#End Region

#Region "Lignes du clic droit"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Preparer
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : À l'ouverture du menu : retire les lignes du mot précédent, puis, si le correcteur
    '              est actif et que le mot (sous le pointeur, ou au curseur si le menu vient du
    '              clavier) est inconnu, ajoute en tête : suggestions (en gras, cinq au plus),
    '              « Ajouter au dictionnaire », puis un séparateur.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Preparer(actif As Boolean)

        Retirer()
        If Not actif Then Return

        Dim debut As Integer
        Dim mot As String = MotSousLePointeur(debut)
        If mot.Length = 0 Then Return

        Dim langue As String = EtiquetteLangue()
        If Not CorrecteurWindows.MotMalOrthographie(langue, mot) Then Return

        If _policeSuggestion Is Nothing Then _policeSuggestion = New Font(_menu.Font, FontStyle.Bold)

        Dim suggestions As List(Of String) = CorrecteurWindows.Suggestions(langue, mot, NB_SUGGESTIONS)

        If suggestions.Count = 0 Then
            _lignes.Add(New ToolStripMenuItem("Aucune suggestion") With {.Enabled = False})
        End If

        For Each suggestion As String In suggestions
            Dim remplacement As String = suggestion
            Dim ligne As New ToolStripMenuItem(suggestion) With {.Font = _policeSuggestion}
            AddHandler ligne.Click, Sub(s, e) RemplacerMot(debut, mot.Length, remplacement)
            _lignes.Add(ligne)
        Next

        Dim ajouter As New ToolStripMenuItem("Ajouter au dictionnaire")
        AddHandler ajouter.Click, Sub(s, e) MettreAJourDictionnaire(CorrecteurWindows.AjouterAuDictionnaire(langue, mot))
        _lignes.Add(ajouter)

        _lignes.Add(New ToolStripSeparator())

        For i As Integer = _lignes.Count - 1 To 0 Step -1
            _menu.Items.Insert(0, _lignes(i))
        Next

    End Sub

    Private Sub Retirer()

        For Each ligne As ToolStripItem In _lignes
            _menu.Items.Remove(ligne)
            ligne.Dispose()
        Next
        _lignes.Clear()

    End Sub

    ' Mot sous le pointeur de la souris (ou au curseur) ; vide s'il n'y en a pas. debut : sa position.
    Private Function MotSousLePointeur(ByRef debut As Integer) As String

        Dim texte As String = _zone.Text
        If texte.Length = 0 Then Return String.Empty

        Dim point As Point = _zone.PointToClient(Cursor.Position)
        Dim index As Integer = If(_zone.ClientRectangle.Contains(point),
                                  _zone.GetCharIndexFromPosition(point),
                                  _zone.SelectionStart)
        index = Math.Min(Math.Max(index, 0), texte.Length - 1)

        ' Curseur juste après le mot (fin de mot, de ligne ou de texte) : on prend le mot précédent.
        If Not EstLettreDeMot(texte(index)) AndAlso index > 0 AndAlso EstLettreDeMot(texte(index - 1)) Then index -= 1
        If Not EstLettreDeMot(texte(index)) Then Return String.Empty

        debut = index
        While debut > 0 AndAlso EstLettreDeMot(texte(debut - 1))
            debut -= 1
        End While

        Dim fin As Integer = index
        While fin < texte.Length AndAlso EstLettreDeMot(texte(fin))
            fin += 1
        End While

        ' Apostrophes et traits d'union au bord ne font pas partie du mot.
        While debut < fin AndAlso Not Char.IsLetter(texte(debut))
            debut += 1
        End While
        While fin > debut AndAlso Not Char.IsLetter(texte(fin - 1))
            fin -= 1
        End While

        Return texte.Substring(debut, fin - debut)

    End Function

    Private Shared Function EstLettreDeMot(c As Char) As Boolean
        Return Char.IsLetter(c) OrElse c = "'"c OrElse c = "’"c OrElse c = "-"c
    End Function

    ' Remplace le mot par la suggestion choisie (par le contrôle : historique, modification).
    Private Sub RemplacerMot(debut As Integer, longueur As Integer, remplacement As String)
        _zone.Select(debut, longueur)
        _remplacerSelection(remplacement)
    End Sub

    ' Après un ajout au dictionnaire : soulignement recalculé, ou message d'échec. Les lignes
    ' n'existent que si le correcteur est actif : il est donc rallumé.
    Private Sub MettreAJourDictionnaire(reussi As Boolean)

        If Not reussi Then
            DialogChoix.Erreur("Le correcteur de Windows n'a pas pu enregistrer ce mot.", "Correcteur d'orthographe")
            Return
        End If

        Dim modifie As Boolean = _zone.Modified
        RichTextEditorHelper.DefinirCorrecteurOrthographe(_zone, False)
        RichTextEditorHelper.DefinirCorrecteurOrthographe(_zone, True)
        _zone.Modified = modifie
        _zone.Focus()

    End Sub

#End Region

End Class
