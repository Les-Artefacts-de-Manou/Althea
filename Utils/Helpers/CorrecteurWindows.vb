' -------------------------------------------------------------------------------------------------
' Module     : CorrecteurWindows
' Projet     : Althéa
' Version    : V1.1.0
' Date       : 20/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Accès au correcteur d'orthographe de Windows (API Spell Checking, Windows 8 et suivants) pour le
' clic droit de l'éditeur : mot mal orthographié ?, suggestions, ajout au dictionnaire.
'
' Remarques  :
' - Le soulignement pendant la frappe est fait par le contrôle de texte lui-même
'   (RichTextEditorHelper.DefinirCorrecteurOrthographe) ; ce module complète le clic droit, que
'   Windows ne permet pas d'enrichir (notre menu remplace le sien).
' - Choix de Joëlle (20/10/2026, essai concluant) : correcteur de Windows ; dictionnaire de Windows
'   de l'utilisateur (« Ajouter au dictionnaire » sert aussi à Edge, au Bloc-notes… et survit aux
'   mises à jour d'Althéa).
' - Interfaces COM de spellcheck.h, déclarées dans l'ordre de leur table de méthodes (ne pas
'   réordonner). Un correcteur par langue, créé à la première demande et gardé ensuite.
' - Rien n'est envoyé hors du poste ; aucun mot n'est journalisé (il peut venir d'une note).
' - Toute erreur (Windows trop ancien, langue absente) : réponse neutre (mot correct, aucune
'   suggestion), journalisée sans le mot.
' - V1.1.0 (20/10/2026) : suggestions, ajout au dictionnaire (V1.0.0 : diagnostic seul). « Ignorer »
'   (ISpellChecker.Ignore) essayé puis retiré : il ne vaut que pour ce correcteur, pas pour le
'   soulignement du contrôle de texte (résultat incohérent, relevé par Joëlle).
'
' Dépendances :
' - GestionLog
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes

Public Module CorrecteurWindows

#Region "Interop (spellcheck.h)"

    Private Const S_OK As Integer = 0

    ' Fabrique des correcteurs de Windows (CLSID_SpellCheckerFactory).
    <ComImport, Guid("7AB36653-1796-484B-BDFA-E74F1DB7C1DC")>
    Private Class SpellCheckerFactory
    End Class

    <ComImport, Guid("8E018A9D-2415-4677-BF08-794EA61F94BB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Private Interface ISpellCheckerFactory
        Function SupportedLanguages() As IEnumString
        Function IsSupported(<MarshalAs(UnmanagedType.LPWStr)> languageTag As String) As <MarshalAs(UnmanagedType.Bool)> Boolean
        Function CreateSpellChecker(<MarshalAs(UnmanagedType.LPWStr)> languageTag As String) As ISpellChecker
    End Interface

    ' ISpellChecker : seules les quatre premières méthodes sont déclarées (ordre de la table).
    <ComImport, Guid("B6FD0B71-E2BC-4653-8D05-F197E412770B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Private Interface ISpellChecker
        Function LanguageTag() As IntPtr
        Function Check(<MarshalAs(UnmanagedType.LPWStr)> text As String) As IEnumSpellingError
        Function Suggest(<MarshalAs(UnmanagedType.LPWStr)> word As String) As IEnumString
        Sub Add(<MarshalAs(UnmanagedType.LPWStr)> word As String)
    End Interface

    ' Next rend S_FALSE (1) quand il n'y a plus d'erreur : HRESULT lu tel quel.
    <ComImport, Guid("803E3BD4-2828-4410-8290-418D1D73C762"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Private Interface IEnumSpellingError
        <PreserveSig>
        Function [Next](<Out> ByRef erreur As ISpellingError) As Integer
    End Interface

    <ComImport, Guid("B7C82D61-FBE8-4B47-9B27-6C0D2E0DE0A3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Private Interface ISpellingError
    End Interface

#End Region

#Region "Correcteurs par langue"

    ' Correcteurs créés (langue → correcteur) ; Nothing = langue non prise en charge sur le poste.
    Private ReadOnly _correcteurs As New Dictionary(Of String, ISpellChecker)(StringComparer.OrdinalIgnoreCase)

    Private Function Correcteur(langue As String) As ISpellChecker

        If String.IsNullOrWhiteSpace(langue) Then Return Nothing

        Dim existant As ISpellChecker = Nothing
        If _correcteurs.TryGetValue(langue, existant) Then Return existant

        Dim cree As ISpellChecker = Nothing
        Try
            Dim fabrique As ISpellCheckerFactory = DirectCast(New SpellCheckerFactory(), ISpellCheckerFactory)
            If fabrique.IsSupported(langue) Then cree = fabrique.CreateSpellChecker(langue)
        Catch ex As Exception
            GestionLog.EcrireException($"Correcteur de Windows indisponible (langue {langue}).", ex,
                                       category:=GestionLog.LogCategory.UI)
        End Try

        _correcteurs(langue) = cree
        Return cree

    End Function

#End Region

#Region "Fonctions publiques"

    ' Le correcteur de Windows sait-il vérifier cette langue (« fr-BE », « fr-FR »…) ?
    Public Function LangueDisponible(langue As String) As Boolean
        Return Correcteur(langue) IsNot Nothing
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MotMalOrthographie
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    ' Rôle       : Le mot est-il inconnu du correcteur (dictionnaire de la langue et de l'utilisateur) ?
    ' Retour     : False si la langue n'est pas prise en charge ou en cas d'erreur.
    ' -------------------------------------------------------------------------------------------------
    Public Function MotMalOrthographie(langue As String, mot As String) As Boolean

        Dim verificateur As ISpellChecker = Correcteur(langue)
        If verificateur Is Nothing OrElse String.IsNullOrWhiteSpace(mot) Then Return False

        Try
            Dim erreurs As IEnumSpellingError = verificateur.Check(mot)
            Dim erreur As ISpellingError = Nothing
            Return erreurs.Next(erreur) = S_OK
        Catch ex As Exception
            GestionLog.EcrireException("Correcteur de Windows : vérification d'un mot impossible.", ex,
                                       category:=GestionLog.LogCategory.UI)
            Return False
        End Try

    End Function

    ' Suggestions du correcteur pour un mot (au plus « maximum »), dans l'ordre de Windows.
    Public Function Suggestions(langue As String, mot As String, maximum As Integer) As List(Of String)

        Dim resultat As New List(Of String)
        Dim verificateur As ISpellChecker = Correcteur(langue)
        If verificateur Is Nothing OrElse String.IsNullOrWhiteSpace(mot) Then Return resultat

        Try
            Dim liste As IEnumString = verificateur.Suggest(mot)
            Dim lu(0) As String
            While resultat.Count < maximum AndAlso liste.Next(1, lu, IntPtr.Zero) = S_OK
                If Not String.IsNullOrEmpty(lu(0)) AndAlso Not resultat.Contains(lu(0)) Then resultat.Add(lu(0))
            End While
        Catch ex As Exception
            GestionLog.EcrireException("Correcteur de Windows : suggestions impossibles.", ex,
                                       category:=GestionLog.LogCategory.UI)
        End Try

        Return resultat

    End Function

    ' Ajoute le mot au dictionnaire de Windows de l'utilisateur ; False en cas d'échec (journalisé).
    Public Function AjouterAuDictionnaire(langue As String, mot As String) As Boolean
        Return Appliquer(langue, mot, Sub(v, m) v.Add(m), "ajout au dictionnaire")
    End Function

    Private Function Appliquer(langue As String, mot As String, action As Action(Of ISpellChecker, String), libelle As String) As Boolean

        Dim verificateur As ISpellChecker = Correcteur(langue)
        If verificateur Is Nothing OrElse String.IsNullOrWhiteSpace(mot) Then Return False

        Try
            action(verificateur, mot)
            Return True
        Catch ex As Exception
            GestionLog.EcrireException($"Correcteur de Windows : {libelle} impossible.", ex,
                                       category:=GestionLog.LogCategory.UI)
            Return False
        End Try

    End Function

#End Region

End Module
