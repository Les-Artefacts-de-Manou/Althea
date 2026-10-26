' -------------------------------------------------------------------------------------------------
' Module      : GestionCodesPostauxBe
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Contrôle d'une adresse belge par le référentiel des codes postaux (ref_codes_postaux_be,
' v2.56.0) : code postal existant, localités acceptées pour un code, localité proposée.
'
' Règles (décisions de Joëlle, 24/10/2026) :
' - En Belgique, un code postal inexistant est refusé (Valider) ;
' - la localité est proposée dès la saisie du code (LocaliteProposee, Localites) et, dans l'autre
'   sens, le code dès la saisie de la localité (CodesDeLocalite ; liste « Liège · 4020 » des
'   suggestions, ChoixLocalites / LireChoix — décision de Joëlle, 24/10/2026 : on connaît plus
'   souvent la localité que le code) ;
' - une localité qui ne correspond à aucun nom du code est signalée, sans être refusée
'   (LocaliteHorsCode, MessageLocaliteHorsCode) : l'écran avertit et demande confirmation.
'
' Remarques   :
' - Autres pays : seul le format est contrôlé (UtilsCodePostal.Valider) ; une adresse sans pays
'   n'est pas contrôlée, comme avant.
' - Les noms se comparent par UtilsCodePostal.CleLocalite (accents, majuscules, traits d'union,
'   « Saint / Sint / St ») : « grace hollogne » correspond à « Grâce-Hollogne ».
' - La liste est lue une fois par session puis gardée en mémoire (lecture seule, ≈ 4 000 noms).
' - SQL dans QueryCodesPostauxBe ; accès par DatabaseManager.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Globalization
Imports System.Text
Imports MySqlConnector

Public Module GestionCodesPostauxBe

    ' Noms cités au plus dans un message (« Liège, Glain, Rocourt… »).
    Private Const NB_NOMS_CITES As Integer = 4

    ' Codes cités au plus dans un message (Charleroi en a quatorze).
    Private Const NB_CODES_CITES As Integer = 6

    ' Séparateur d'un choix « localité · code » de la liste des suggestions.
    Private Const SEPARATEUR_CHOIX As String = " · "

    ' Localités d'un code postal : le nom proposé (vide s'il n'y en a pas) et tous les noms acceptés.
    Private NotInheritable Class LocalitesDuCode
        Public Property Proposee As String = String.Empty
        Public ReadOnly Property Noms As New List(Of String)
    End Class

    ' La liste lue, sous les trois formes utiles : par code, par localité (clé de comparaison → codes)
    ' et en choix « localité · code » triés.
    Private NotInheritable Class Referentiel
        Public ReadOnly Property ParCode As New Dictionary(Of String, LocalitesDuCode)(StringComparer.Ordinal)
        Public ReadOnly Property ParLocalite As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)
        Public Property Choix As String() = Array.Empty(Of String)()
    End Class

    Private _referentiel As Referentiel
    Private ReadOnly _verrou As New Object()

#Region "Lecture"

    ' Liste chargée à la première utilisation.
    Private Function Liste() As Referentiel

        SyncLock _verrou

            If _referentiel IsNot Nothing Then Return _referentiel

            Dim ref As New Referentiel()
            Dim choix As New HashSet(Of String)(StringComparer.Ordinal)

            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryCodesPostauxBe.SelectCodesPostauxBe, conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim code As String = reader.GetString("code_postal")
                            Dim localite As String = reader.GetString("localite")

                            Dim entree As LocalitesDuCode = Nothing
                            If Not ref.ParCode.TryGetValue(code, entree) Then
                                entree = New LocalitesDuCode()
                                ref.ParCode.Add(code, entree)
                            End If

                            entree.Noms.Add(localite)
                            If reader.GetBoolean("est_proposee") Then entree.Proposee = localite

                            Dim cle As String = UtilsCodePostal.CleLocalite(localite)
                            Dim codes As List(Of String) = Nothing
                            If Not ref.ParLocalite.TryGetValue(cle, codes) Then
                                codes = New List(Of String)
                                ref.ParLocalite.Add(cle, codes)
                            End If
                            If Not codes.Contains(code) Then codes.Add(code)

                            ' La liste des suggestions de WinForms ne compare pas sans accents : « liege »
                            ' doit aussi trouver « Liège » (le choix est remis à l'écriture officielle).
                            choix.Add(localite & SEPARATEUR_CHOIX & code)
                            choix.Add(SansAccents(localite) & SEPARATEUR_CHOIX & code)
                        End While
                    End Using
                End Using
            End Using

            ' Lecture triée par code : chaque liste de codes d'une localité l'est donc aussi.
            ref.Choix = choix.OrderBy(Function(c) c, StringComparer.CurrentCultureIgnoreCase).ToArray()

            GestionLog.EcrireLog($"Codes postaux belges : {ref.ParCode.Count} codes chargés.",
                                 GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Database)
            _referentiel = ref
            Return _referentiel

        End SyncLock

    End Function

    ' « Liège » → « Liege », « Rœulx » → « Roeulx » (majuscules et ponctuation gardées).
    Private Function SansAccents(texte As String) As String
        Dim sb As New StringBuilder(texte.Length)
        For Each c As Char In texte.Normalize(NormalizationForm.FormD)
            If CharUnicodeInfo.GetUnicodeCategory(c) <> UnicodeCategory.NonSpacingMark Then sb.Append(c)
        Next
        Return sb.ToString().Normalize(NormalizationForm.FormC).Replace("œ", "oe").Replace("Œ", "Oe").Replace("æ", "ae").Replace("Æ", "Ae")
    End Function

    ' Entrée du code dans la liste, ou Nothing (code inconnu ou mal formé).
    Private Function Entree(codePostal As String) As LocalitesDuCode
        Dim entreeCode As LocalitesDuCode = Nothing
        Liste().ParCode.TryGetValue(UtilsCodePostal.CleCodePostal(codePostal), entreeCode)
        Return entreeCode
    End Function

#End Region

#Region "Règles"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : EstBelge
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : True si le pays de l'adresse est la Belgique (code ISO ou libellé reconnu).
    ' -------------------------------------------------------------------------------------------------
    Public Function EstBelge(pays As String) As Boolean
        Return UtilsTelephone.DeduirePays(pays) = "BE"
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Valider
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Contrôle un code postal selon le pays : le format (UtilsCodePostal.Valider) et, en
    '            Belgique, l'existence du code.
    '
    ' Retour   :
    ' - Boolean : True si vide, pays non contrôlé ou code correct ; False sinon (messageErreur dit
    '             pourquoi).
    ' -------------------------------------------------------------------------------------------------
    Public Function Valider(codePostal As String, pays As String, ByRef messageErreur As String) As Boolean

        If Not UtilsCodePostal.Valider(codePostal, pays, messageErreur) Then Return False

        Dim code As String = UtilsCodePostal.CleCodePostal(codePostal)
        If code.Length = 0 OrElse Not EstBelge(pays) Then Return True

        ' Le message ne recopie pas la valeur saisie : un refus peut passer par le journal.
        If Entree(code) Is Nothing Then
            messageErreur = "Ce code postal n'existe pas en Belgique."
            Return False
        End If

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Localites
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Les noms acceptés pour un code postal belge (suggestions de la saisie), par ordre
    '            alphabétique ; liste vide pour un code inconnu.
    ' -------------------------------------------------------------------------------------------------
    Public Function Localites(codePostal As String) As IReadOnlyList(Of String)
        Dim entreeCode As LocalitesDuCode = Entree(codePostal)
        If entreeCode Is Nothing Then Return Array.Empty(Of String)()
        Return entreeCode.Noms
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LocaliteProposee
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Le nom à remplir d'office à la saisie d'un code postal belge (« 4000 » → « Liège ») ;
    '            chaîne vide pour un code inconnu ou sans nom proposé.
    ' -------------------------------------------------------------------------------------------------
    Public Function LocaliteProposee(codePostal As String) As String
        Dim entreeCode As LocalitesDuCode = Entree(codePostal)
        Return If(entreeCode Is Nothing, String.Empty, entreeCode.Proposee)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LocaliteHorsCode
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : True si, pour une adresse belge au code connu, la localité saisie ne correspond à
    '            aucun nom du code (« 4000 Namur ») ; False dans tous les autres cas (pays non belge,
    '            code vide ou inconnu — signalé par Valider —, localité vide ou correcte).
    ' -------------------------------------------------------------------------------------------------
    Public Function LocaliteHorsCode(codePostal As String, localite As String, pays As String) As Boolean

        Dim cleSaisie As String = UtilsCodePostal.CleLocalite(localite)
        If cleSaisie.Length = 0 OrElse Not EstBelge(pays) Then Return False

        Dim entreeCode As LocalitesDuCode = Entree(codePostal)
        If entreeCode Is Nothing Then Return False

        Return Not entreeCode.Noms.Any(Function(nom) UtilsCodePostal.CleLocalite(nom) = cleSaisie)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : MessageLocaliteHorsCode
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : « La localité « Namur » ne correspond pas au code postal 4000 (Liège, Glain, Luik,
    '            Lüttich…). » — le nom proposé en premier, quelques autres ensuite.
    '
    ' Remarques :
    ' - Recopie l'adresse saisie : à afficher (avertissement, question), jamais à journaliser.
    ' -------------------------------------------------------------------------------------------------
    Public Function MessageLocaliteHorsCode(codePostal As String, localite As String) As String

        Dim entreeCode As LocalitesDuCode = Entree(codePostal)
        Dim code As String = UtilsCodePostal.CleCodePostal(codePostal)
        If entreeCode Is Nothing Then Return "Ce code postal n'existe pas en Belgique."

        Dim noms As List(Of String) = entreeCode.Noms.
            OrderBy(Function(nom) If(nom = entreeCode.Proposee, 0, 1)).
            ThenBy(Function(nom) nom, StringComparer.CurrentCulture).
            ToList()

        Dim cites As String = String.Join(", ", noms.Take(NB_NOMS_CITES)) & If(noms.Count > NB_NOMS_CITES, "…", String.Empty)

        Return $"La localité « {localite.Trim()} » ne correspond pas au code postal {code} ({cites})."

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodesDeLocalite
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Les codes postaux belges d'une localité, par ordre croissant (« Avin » → 4280 ;
    '            « liege » → 4000, 4020, 4030, 4031, 4032) ; liste vide pour un nom inconnu.
    ' -------------------------------------------------------------------------------------------------
    Public Function CodesDeLocalite(localite As String) As IReadOnlyList(Of String)
        Dim codes As List(Of String) = Nothing
        If Liste().ParLocalite.TryGetValue(UtilsCodePostal.CleLocalite(localite), codes) Then Return codes
        Return Array.Empty(Of String)()
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : MessagePlusieursCodes
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : « « Liège » a plusieurs codes postaux (4000, 4020, 4030, 4031, 4032) : choisissez le
    '            bon. » (six codes au plus, puis « … ») ; chaîne vide si la localité n'a pas plusieurs
    '            codes.
    '
    ' Remarques :
    ' - Recopie la localité saisie : à afficher, jamais à journaliser.
    ' -------------------------------------------------------------------------------------------------
    Public Function MessagePlusieursCodes(localite As String) As String
        Dim codes As IReadOnlyList(Of String) = CodesDeLocalite(localite)
        If codes.Count < 2 Then Return String.Empty
        Dim cites As String = String.Join(", ", codes.Take(NB_CODES_CITES)) & If(codes.Count > NB_CODES_CITES, "…", String.Empty)
        Return $"« {localite.Trim()} » a plusieurs codes postaux ({cites}) : choisissez le bon."
    End Function

#End Region

#Region "Choix « localité · code »"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChoixLocalites
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Tous les noms acceptés suivis de leur code (« Liège · 4000 », « Liège · 4020 »…), triés :
    '            suggestions du champ Localité quand le code n'est pas encore connu. Un nom accentué y
    '            figure aussi sans accents (« Liege · 4000 ») : la liste de WinForms compare les accents.
    ' -------------------------------------------------------------------------------------------------
    Public Function ChoixLocalites() As IReadOnlyList(Of String)
        Return Liste().Choix
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LireChoix
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Sépare un choix « Liège · 4020 » en localité (écriture du référentiel : « Liege · 4020 »
    '            donne « Liège ») et code postal.
    '
    ' Retour   :
    ' - Boolean : True si le texte a la forme d'un choix (nom, séparateur, code à 4 chiffres).
    ' -------------------------------------------------------------------------------------------------
    Public Function LireChoix(texte As String, ByRef localite As String, ByRef codePostal As String) As Boolean

        localite = String.Empty
        codePostal = String.Empty

        Dim brut As String = If(texte, String.Empty).Trim()
        Dim position As Integer = brut.LastIndexOf(SEPARATEUR_CHOIX.Trim(), StringComparison.Ordinal)
        If position <= 0 Then Return False

        Dim nom As String = brut.Substring(0, position).Trim()
        Dim code As String = brut.Substring(position + SEPARATEUR_CHOIX.Trim().Length).Trim()
        If nom.Length = 0 OrElse code.Length <> 4 OrElse Not code.All(AddressOf Char.IsDigit) Then Return False

        localite = EcritureOfficielle(nom, code)
        codePostal = code
        Return True

    End Function

    ' L'écriture du référentiel d'un nom du code (« Liege », 4000 → « Liège ») ; le nom tel quel s'il
    ' n'en fait pas partie.
    Private Function EcritureOfficielle(localite As String, codePostal As String) As String
        Dim entreeCode As LocalitesDuCode = Entree(codePostal)
        If entreeCode Is Nothing Then Return localite
        Dim cle As String = UtilsCodePostal.CleLocalite(localite)
        Return If(entreeCode.Noms.FirstOrDefault(Function(nom) UtilsCodePostal.CleLocalite(nom) = cle), localite)
    End Function

#End Region

End Module
