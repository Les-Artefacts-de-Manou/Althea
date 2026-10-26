' -------------------------------------------------------------------------------------------------
' Classe      : SaisieLocaliteBelge
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Aide à la saisie du couple code postal / localité d'une adresse, branchée sur les deux zones de
' texte d'un écran. Pour une adresse belge, dans les deux sens (décisions de Joëlle, 24/10/2026) :
' du code vers la localité, et de la localité vers le code — on connaît plus souvent la localité.
'
' Fonctionnement :
' - Code d'abord : code quitté après une modification, localité vide → la localité proposée est
'   remplie (« 4000 » → « Liège ») ; pendant la frappe, le champ Localité suggère les noms du code
'   (« A » → Abolens, Avernas-le-Bauduin, Avin pour 4280).
' - Localité d'abord (code vide) : pendant la frappe, le champ Localité suggère « Liège · 4000 »,
'   « Liège · 4020 »… ; en quittant le champ, un choix est séparé et remplit les deux champs.
'   Localité tapée sans choisir : un seul code → il est rempli (« Avin » → 4280) ; plusieurs →
'   icône sur le champ Code (« « Liège » a plusieurs codes postaux (4000, 4020…) ») et le champ
'   Code suggère ces codes.
' - Localité qui ne correspond à aucun nom du code : icône d'avertissement sur le champ, puis
'   ConfirmerLocalite avant l'enregistrement (« Enregistrer quand même » / « Corriger »).
'
' Utilisation :
' - New SaisieLocaliteBelge(txtCodePostal, txtLocalite, AddressOf CodePaysSaisi) au chargement de
'   l'écran ; Actualiser() après avoir rempli les champs ou changé de pays ; MessageCodePostal()
'   pour l'erreur du code ; ConfirmerLocalite() avant d'enregistrer (LocaliteAConfirmer() permet
'   d'afficher d'abord l'onglet de l'adresse) ; Dispose avec l'écran.
'
' Remarques   :
' - Règles : GestionCodesPostauxBe (référentiel ref_codes_postaux_be). Autres pays : rien n'est
'   proposé ni signalé, seul le format du code est contrôlé.
' - Un champ n'est rempli que s'il est vide et modifiable (jamais en consultation), et seulement
'   après une modification de l'autre champ par l'utilisateur.
' - Un choix « localité · code » n'est séparé qu'en quittant le champ : parcourir la liste des
'   suggestions au clavier change le texte du champ à chaque ligne, et ne doit rien remplir. Les
'   boutons d'enregistrement prennent le focus : le champ est donc quitté avant l'enregistrement.
' - Les suggestions d'un champ ne changent jamais pendant qu'on y tape (WinForms le supporte mal) :
'   elles sont mises à jour quand on le quitte.
' - Si la liste ne peut pas être lue, l'aide se met en retrait (journal) : la saisie reste possible,
'   le contrôle du métier refusera l'enregistrement avec son propre message.
' - Rien de ce qui est saisi n'est journalisé.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public NotInheritable Class SaisieLocaliteBelge
    Implements IDisposable

    Private ReadOnly _txtCodePostal As TextBox
    Private ReadOnly _txtLocalite As TextBox
    Private ReadOnly _pays As Func(Of String)
    Private ReadOnly _avertissement As ErrorProvider

    ' Contenu de chaque champ à l'entrée (un remplissage ne suit qu'une modification).
    Private _codeALEntree As String = String.Empty
    Private _localiteALEntree As String = String.Empty

    ' Champ où l'on tape : ses suggestions ne sont pas changées avant qu'on le quitte.
    Private _codeActif As Boolean
    Private _localiteActive As Boolean

    ' Suggestions en place (clé de ce qu'elles contiennent), pour ne les reconstruire qu'au besoin.
    Private _sourceLocalite As String = Nothing
    Private _sourceCode As String = Nothing

    ' Liste illisible (base) : l'aide ne fait plus rien pour le reste de la session de l'écran.
    Private _horsService As Boolean

    Public Sub New(txtCodePostal As TextBox, txtLocalite As TextBox, pays As Func(Of String))

        _txtCodePostal = txtCodePostal
        _txtLocalite = txtLocalite
        _pays = pays

        _avertissement = New ErrorProvider() With {
            .Icon = New Icon(SystemIcons.Warning, 16, 16),
            .BlinkStyle = ErrorBlinkStyle.NeverBlink
        }

        _txtLocalite.AutoCompleteSource = AutoCompleteSource.CustomSource
        _txtCodePostal.AutoCompleteSource = AutoCompleteSource.CustomSource

        AddHandler _txtCodePostal.Enter, AddressOf CodePostal_Enter
        AddHandler _txtCodePostal.Leave, AddressOf CodePostal_Leave
        AddHandler _txtCodePostal.TextChanged, AddressOf CodePostal_TextChanged
        AddHandler _txtLocalite.Enter, AddressOf Localite_Enter
        AddHandler _txtLocalite.Leave, AddressOf Localite_Leave

    End Sub

#Region "Méthodes publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Actualiser
    ' Version   : V1.0.0
    ' Date      : 24/10/2026
    '
    ' Rôle      : Met à jour les suggestions et les avertissements d'après les champs, sans rien
    '             remplir : après le chargement d'une fiche, une remise à zéro ou un changement de pays.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Actualiser()
        MettreAJourSuggestions()
        MettreAJourAvertissements()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : MessageCodePostal
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Erreur du code postal saisi (format, et existence en Belgique) ; chaîne vide s'il est
    '            correct. Pour le signalement immédiat de l'écran (ErrorProvider).
    ' -------------------------------------------------------------------------------------------------
    Public Function MessageCodePostal() As String

        Dim message As String = String.Empty

        If _horsService Then
            UtilsCodePostal.Valider(_txtCodePostal.Text, _pays(), message)
            Return message
        End If

        Try
            GestionCodesPostauxBe.Valider(_txtCodePostal.Text, _pays(), message)
        Catch ex As Exception
            MettreHorsService(ex)
            UtilsCodePostal.Valider(_txtCodePostal.Text, _pays(), message)
        End Try

        Return message

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConfirmerLocalite
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Avant l'enregistrement : si la localité ne correspond pas au code postal belge, le
    '            demande (« Enregistrer quand même » / « Corriger », qui ramène au champ).
    '
    ' Retour   :
    ' - Boolean : True si l'enregistrement peut continuer.
    ' -------------------------------------------------------------------------------------------------
    Public Function ConfirmerLocalite() As Boolean

        If Not LocaliteAConfirmer() Then Return True

        Dim reponse As DialogResult = DialogChoix.Confirmer(
            GestionCodesPostauxBe.MessageLocaliteHorsCode(_txtCodePostal.Text, _txtLocalite.Text) & Environment.NewLine &
            "Voulez-vous enregistrer cette adresse quand même ?",
            "Localité", "Enregistrer quand même", "Corriger")

        If reponse = DialogResult.Yes Then Return True

        _txtLocalite.Focus()
        _txtLocalite.SelectAll()
        Return False

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LocaliteAConfirmer
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : True si la localité ne correspond à aucun nom du code postal belge saisi (l'écran
    '            peut d'abord afficher l'onglet de l'adresse avant ConfirmerLocalite).
    ' -------------------------------------------------------------------------------------------------
    Public Function LocaliteAConfirmer() As Boolean
        Return Executer(Function() GestionCodesPostauxBe.LocaliteHorsCode(_txtCodePostal.Text, _txtLocalite.Text, _pays()), False)
    End Function

#End Region

#Region "Événements des champs"

    Private Sub CodePostal_Enter(sender As Object, e As EventArgs)
        _codeActif = True
        _codeALEntree = _txtCodePostal.Text.Trim()
    End Sub

    Private Sub CodePostal_TextChanged(sender As Object, e As EventArgs)
        MettreAJourSuggestions()
    End Sub

    Private Sub CodePostal_Leave(sender As Object, e As EventArgs)

        _codeActif = False
        If _txtCodePostal.Text.Trim() <> _codeALEntree Then RemplirLocaliteProposee()
        MettreAJourSuggestions()
        MettreAJourAvertissements()

    End Sub

    Private Sub Localite_Enter(sender As Object, e As EventArgs)
        _localiteActive = True
        _localiteALEntree = _txtLocalite.Text.Trim()
    End Sub

    Private Sub Localite_Leave(sender As Object, e As EventArgs)

        _localiteActive = False
        AppliquerChoix()
        If _txtLocalite.Text.Trim() <> _localiteALEntree Then RemplirCodeUnique()
        MettreAJourSuggestions()
        MettreAJourAvertissements()

    End Sub

#End Region

#Region "Remplissage"

    ' Localité vide, zone modifiable, code belge connu : le nom proposé du code.
    Private Sub RemplirLocaliteProposee()

        If Not EstModifiableEtVide(_txtLocalite) Then Return

        Dim proposee As String = Executer(Function()
                                              If Not GestionCodesPostauxBe.EstBelge(_pays()) Then Return String.Empty
                                              Return GestionCodesPostauxBe.LocaliteProposee(_txtCodePostal.Text)
                                          End Function, String.Empty)

        If proposee.Length > 0 Then _txtLocalite.Text = proposee

    End Sub

    ' Code vide, zone modifiable, localité belge qui n'a qu'un code : ce code.
    Private Sub RemplirCodeUnique()

        If Not EstModifiableEtVide(_txtCodePostal) Then Return

        Dim codes As IReadOnlyList(Of String) = Executer(Function() CodesDeLaLocaliteSaisie(), Nothing)
        If codes IsNot Nothing AndAlso codes.Count = 1 Then _txtCodePostal.Text = codes(0)

    End Sub

    ' « Liège · 4020 » → localité « Liège » et, si le code est vide et modifiable, code 4020.
    Private Sub AppliquerChoix()

        Dim localite As String = Nothing
        Dim code As String = Nothing
        If Not GestionCodesPostauxBe.LireChoix(_txtLocalite.Text, localite, code) Then Return

        _txtLocalite.Text = localite
        If EstModifiableEtVide(_txtCodePostal) Then _txtCodePostal.Text = code

    End Sub

    Private Shared Function EstModifiableEtVide(zone As TextBox) As Boolean
        Return Not zone.ReadOnly AndAlso zone.Enabled AndAlso zone.Text.Trim().Length = 0
    End Function

#End Region

#Region "Suggestions et avertissements"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourSuggestions
    ' Version   : V1.0.0
    ' Date      : 24/10/2026
    '
    ' Rôle      : Suggestions de chaque champ d'une adresse belge :
    '             - Localité : code connu → les noms du code ; code vide → « localité · code » ;
    '             - Code : code vide → les codes de la localité saisie.
    '             Autre pays : aucune. Le champ où l'on tape garde les siennes jusqu'à ce qu'on le quitte.
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourSuggestions()

        Dim belge As Boolean = Executer(Function() GestionCodesPostauxBe.EstBelge(_pays()), False)
        Dim code As String = UtilsCodePostal.CleCodePostal(_txtCodePostal.Text)

        If Not _localiteActive Then
            Dim source As String = If(Not belge, String.Empty, If(code.Length = 0, "*", code))
            If source <> _sourceLocalite Then
                _sourceLocalite = source
                Select Case source
                    Case String.Empty
                        DefinirSuggestions(_txtLocalite, Nothing, AutoCompleteMode.None)
                    Case "*"
                        ' Suggest seul : un ajout automatique choisirait le premier code à la place de l'utilisateur.
                        DefinirSuggestions(_txtLocalite, Executer(Function() GestionCodesPostauxBe.ChoixLocalites(), Nothing), AutoCompleteMode.Suggest)
                    Case Else
                        DefinirSuggestions(_txtLocalite, Executer(Function() GestionCodesPostauxBe.Localites(code), Nothing), AutoCompleteMode.SuggestAppend)
                End Select
            End If
        End If

        If Not _codeActif Then
            Dim source As String = If(belge AndAlso code.Length = 0, UtilsCodePostal.CleLocalite(_txtLocalite.Text), String.Empty)
            If source <> _sourceCode Then
                _sourceCode = source
                Dim codes As IReadOnlyList(Of String) = Nothing
                If source.Length > 0 Then codes = Executer(Function() CodesDeLaLocaliteSaisie(), Nothing)
                DefinirSuggestions(_txtCodePostal, codes, AutoCompleteMode.Suggest)
            End If
        End If

    End Sub

    Private Shared Sub DefinirSuggestions(zone As TextBox, valeurs As IReadOnlyList(Of String), mode As AutoCompleteMode)

        Dim suggestions As New AutoCompleteStringCollection()
        If valeurs IsNot Nothing Then suggestions.AddRange(valeurs.ToArray())

        zone.AutoCompleteMode = If(suggestions.Count = 0, AutoCompleteMode.None, mode)
        zone.AutoCompleteCustomSource = suggestions

    End Sub

    ' Localité hors du code : sur le champ Localité ; code vide et localité à plusieurs codes : sur
    ' le champ Code.
    Private Sub MettreAJourAvertissements()

        Dim messageLocalite As String = String.Empty
        If LocaliteAConfirmer() Then
            messageLocalite = GestionCodesPostauxBe.MessageLocaliteHorsCode(_txtCodePostal.Text, _txtLocalite.Text)
        End If
        _avertissement.SetError(_txtLocalite, messageLocalite)

        Dim messageCode As String = Executer(Function()
                                                 If _txtCodePostal.Text.Trim().Length > 0 OrElse Not GestionCodesPostauxBe.EstBelge(_pays()) Then Return String.Empty
                                                 Return GestionCodesPostauxBe.MessagePlusieursCodes(_txtLocalite.Text)
                                             End Function, String.Empty)
        _avertissement.SetError(_txtCodePostal, messageCode)

    End Sub

    Private Function CodesDeLaLocaliteSaisie() As IReadOnlyList(Of String)
        If Not GestionCodesPostauxBe.EstBelge(_pays()) Then Return Array.Empty(Of String)()
        Return GestionCodesPostauxBe.CodesDeLocalite(_txtLocalite.Text)
    End Function

    ' Exécute une lecture de la liste ; en cas d'échec, l'aide se met en retrait et rend la valeur
    ' de repli.
    Private Function Executer(Of T)(lecture As Func(Of T), repli As T) As T

        If _horsService Then Return repli

        Try
            Return lecture()
        Catch ex As Exception
            MettreHorsService(ex)
            Return repli
        End Try

    End Function

    Private Sub MettreHorsService(ex As Exception)
        _horsService = True
        GestionLog.EcrireLog("Codes postaux belges : liste illisible, aide à la saisie de la localité désactivée.",
                             GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
    End Sub

#End Region

    Public Sub Dispose() Implements IDisposable.Dispose
        RemoveHandler _txtCodePostal.Enter, AddressOf CodePostal_Enter
        RemoveHandler _txtCodePostal.Leave, AddressOf CodePostal_Leave
        RemoveHandler _txtCodePostal.TextChanged, AddressOf CodePostal_TextChanged
        RemoveHandler _txtLocalite.Enter, AddressOf Localite_Enter
        RemoveHandler _txtLocalite.Leave, AddressOf Localite_Leave
        _avertissement.Dispose()
    End Sub

End Class
