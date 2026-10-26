' -------------------------------------------------------------------------------------------------
' Module      : UtilsIdentifiantBelge
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Valide et formate les identifiants belges : identifiants professionnels (écran Cabinet) et codes
' titulaire de la mutualité du patient (CT1 / CT2).
'
' Règles appliquées :
' - INAMI : 11 chiffres « P-NNNNN-CC-QQQ » (P = profession, NNNNN = numéro d'ordre,
'           CC = clé de contrôle = 97 - (PNNNNN mod 97), QQQ = code de qualification).
'           Les 8 premiers chiffres identifient la personne ; les 3 derniers sa qualification.
' - BCE   : 10 chiffres « 0NNN.NNN.NNN » ou « 1NNN.NNN.NNN », clé = 97 - (8 premiers mod 97).
' - TVA   : « BE » + numéro BCE valide.
' - CT1 / CT2 (codes titulaire de l'assurance soins de santé) : 3 chiffres chacun.
' - Autres types : expression régulière du référentiel (ref_types_identifiant_pro.regex_validation)
'           si elle est renseignée, sinon saisie libre.
'
' Remarques   :
' - Le choix de la règle se fait sur le code technique du type (INAMI, BCE, TVA), repli documenté
'   sur la regex paramétrée pour tout nouveau type.
' - Module statique sans dépendance UI.
' - V1.2.0 (20/10/2026) : EstControleParAlthea (écran Administration › Types d'identifiant : ces
'   types sont des lignes système et leur expression de contrôle n'est pas utilisée) ;
'   FormaterIban, FormaterBic (présentation à la sortie du champ, fiche Cabinet), IbanCompact
'   (forme enregistrée, sans espace : 34 caractères au plus).
' -------------------------------------------------------------------------------------------------
Option Strict On
Option Explicit On

Imports System.Text.RegularExpressions

Public Module UtilsIdentifiantBelge

    Public Const CodeInami As String = "INAMI"
    Public Const CodeBce As String = "BCE"
    Public Const CodeTva As String = "TVA"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : EstControleParAlthea
    ' Version  : V1.0.0
    ' Date     : 20/10/2026
    ' Rôle     : True si le contrôle de ce type est écrit ici (INAMI, BCE, TVA) : l'expression de
    '            contrôle du référentiel ne lui est pas appliquée.
    ' -------------------------------------------------------------------------------------------------
    Public Function EstControleParAlthea(codeType As String) As Boolean
        Select Case If(codeType, "").Trim().ToUpperInvariant()
            Case CodeInami, CodeBce, CodeTva
                Return True
            Case Else
                Return False
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Valider
    ' Rôle     : Valide un identifiant selon son type ; True si vide ou conforme.
    ' -------------------------------------------------------------------------------------------------
    Public Function Valider(codeType As String,
                            regexReferentiel As String,
                            saisie As String,
                            ByRef messageErreur As String) As Boolean

        messageErreur = String.Empty
        Dim brut As String = If(saisie, "").Trim()
        If brut.Length = 0 Then Return True

        Select Case If(codeType, "").Trim().ToUpperInvariant()
            Case CodeInami
                Return ValiderInami(brut, messageErreur)
            Case CodeBce
                Return ValiderBce(brut, messageErreur)
            Case CodeTva
                Return ValiderTva(brut, messageErreur)
        End Select

        If Not String.IsNullOrWhiteSpace(regexReferentiel) Then
            Try
                If Not Regex.IsMatch(brut, regexReferentiel) Then
                    messageErreur = "La valeur ne respecte pas le format défini dans le référentiel."
                    Return False
                End If
            Catch ex As ArgumentException
                ' Regex de référentiel invalide : on n'empêche pas la saisie.
                Return True
            End Try
        End If

        Return True

    End Function

    Public Function ValiderInami(saisie As String, ByRef messageErreur As String) As Boolean
        messageErreur = String.Empty
        Dim chiffres = Chiffres_(saisie)
        If chiffres.Length <> 11 OrElse Not Regex.IsMatch(saisie.Trim(), "^[0-9 .\-/]+$") Then
            messageErreur = "Le numéro INAMI doit comporter 11 chiffres (format 0-00000-00-000)."
            Return False
        End If
        If Not CleModulo97Valide(chiffres.Substring(0, 6), chiffres.Substring(6, 2)) Then
            messageErreur = "Le numéro INAMI est invalide (clé de contrôle incorrecte)."
            Return False
        End If
        Return True
    End Function

    Public Function ValiderBce(saisie As String, ByRef messageErreur As String) As Boolean
        messageErreur = String.Empty
        Dim chiffres = Chiffres_(saisie)
        If chiffres.Length = 9 Then chiffres = "0" & chiffres
        If chiffres.Length <> 10 OrElse Not Regex.IsMatch(saisie.Trim(), "^[0-9 .\-]+$") OrElse
           (chiffres(0) <> "0"c AndAlso chiffres(0) <> "1"c) Then
            messageErreur = "Le numéro d'entreprise (BCE) doit comporter 10 chiffres et commencer par 0 ou 1 (format 0000.000.000)."
            Return False
        End If
        If Not CleModulo97Valide(chiffres.Substring(0, 8), chiffres.Substring(8, 2)) Then
            messageErreur = "Le numéro d'entreprise (BCE) est invalide (clé de contrôle incorrecte)."
            Return False
        End If
        Return True
    End Function

    Public Function ValiderTva(saisie As String, ByRef messageErreur As String) As Boolean
        messageErreur = String.Empty
        Dim brut = saisie.Trim().ToUpperInvariant().Replace(" ", "")
        If Not brut.StartsWith("BE", StringComparison.Ordinal) Then
            messageErreur = "Le numéro de TVA belge doit commencer par BE (format BE0000.000.000)."
            Return False
        End If
        If Not ValiderBce(brut.Substring(2), messageErreur) Then
            messageErreur = "Le numéro de TVA est invalide : " & messageErreur
            Return False
        End If
        Return True
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : FormaterNumeroPersonneInami
    ' Rôle     : Retourne la partie « personne » d'un INAMI (8 premiers chiffres, format 0-00000-00),
    '            sans le code de qualification. Retourne Nothing si l'INAMI n'est pas valide.
    ' -------------------------------------------------------------------------------------------------
    Public Function FormaterNumeroPersonneInami(saisie As String) As String
        Dim message As String = Nothing
        If String.IsNullOrWhiteSpace(saisie) OrElse Not ValiderInami(saisie, message) Then Return Nothing
        Dim c = Chiffres_(saisie)
        Return $"{c.Substring(0, 1)}-{c.Substring(1, 5)}-{c.Substring(6, 2)}"
    End Function

    Private Function CleModulo97Valide(baseChiffres As String, cle As String) As Boolean
        Dim n As Long = Long.Parse(baseChiffres)
        Return (97 - (n Mod 97)) = Integer.Parse(cle)
    End Function

    Private Function Chiffres_(saisie As String) As String
        Return New String(If(saisie, "").Where(Function(ch) Char.IsDigit(ch)).ToArray())
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderIban
    ' Rôle     : Contrôle ISO 13616 (clé modulo 97) ; un IBAN belge (BE) doit compter 16 caractères.
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderIban(saisie As String, ByRef messageErreur As String) As Boolean
        messageErreur = String.Empty
        Dim iban = New String(If(saisie, "").Where(Function(ch) Not Char.IsWhiteSpace(ch)).ToArray()).ToUpperInvariant()
        If iban.Length = 0 Then Return True

        If Not Regex.IsMatch(iban, "^[A-Z]{2}[0-9]{2}[A-Z0-9]{10,30}$") Then
            messageErreur = "L'IBAN doit commencer par le code pays et 2 chiffres (ex. BE68 5390 0754 7034)."
            Return False
        End If
        If iban.StartsWith("BE") AndAlso iban.Length <> 16 Then
            messageErreur = "Un IBAN belge comporte 16 caractères (BE + 14 chiffres)."
            Return False
        End If

        Dim reste As Integer = 0
        For Each ch In iban.Substring(4) & iban.Substring(0, 4)
            Dim valeur = If(Char.IsDigit(ch), (AscW(ch) - 48).ToString(), (AscW(ch) - 55).ToString())
            For Each c In valeur
                reste = (reste * 10 + (AscW(c) - 48)) Mod 97
            Next
        Next
        If reste <> 1 Then
            messageErreur = "L'IBAN est invalide (clé de contrôle incorrecte)."
            Return False
        End If
        Return True
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : FormaterIban
    ' Version  : V1.0.0
    ' Date     : 20/10/2026
    ' Rôle     : Présentation lisible d'un IBAN : majuscules, groupes de 4 (BE68 5390 0754 7034).
    ' Remarque : Ne valide rien (ValiderIban ignore les espaces) ; un IBAN se lit et se recopie par
    '            groupes de 4 quel que soit le pays, d'où pas de masque de saisie à longueur fixe.
    ' -------------------------------------------------------------------------------------------------
    Public Function FormaterIban(saisie As String) As String
        Dim iban = IbanCompact(saisie)
        Dim groupes = Enumerable.Range(0, (iban.Length + 3) \ 4).
            Select(Function(i) iban.Substring(i * 4, Math.Min(4, iban.Length - i * 4)))
        Return String.Join(" ", groupes)
    End Function

    ' Forme enregistrée d'un IBAN : majuscules, sans espace (BE68539007547034).
    Public Function IbanCompact(saisie As String) As String
        Return New String(If(saisie, "").Where(Function(ch) Not Char.IsWhiteSpace(ch)).ToArray()).ToUpperInvariant()
    End Function

    ' Présentation d'un BIC : majuscules, sans espace (GEBABEBB).
    Public Function FormaterBic(saisie As String) As String
        Return If(saisie, "").Replace(" ", "").ToUpperInvariant()
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderBic
    ' Rôle     : Format BIC/SWIFT : 8 ou 11 caractères (ex. GEBABEBB).
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderBic(saisie As String, ByRef messageErreur As String) As Boolean
        messageErreur = String.Empty
        Dim bic = If(saisie, "").Replace(" ", "").ToUpperInvariant()
        If bic.Length = 0 Then Return True
        If Not Regex.IsMatch(bic, "^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}([A-Z0-9]{3})?$") Then
            messageErreur = "Le BIC doit comporter 8 ou 11 caractères (ex. GEBABEBB)."
            Return False
        End If
        Return True
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderCodeTitulaire
    ' Rôle     : Valide un code titulaire (CT1 ou CT2) de la mutualité : 3 chiffres ; True si vide.
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderCodeTitulaire(saisie As String, ByRef messageErreur As String) As Boolean
        messageErreur = String.Empty
        Dim brut = If(saisie, "").Trim()
        If brut.Length = 0 OrElse Regex.IsMatch(brut, "^[0-9]{3}$") Then Return True
        messageErreur = "Le code titulaire doit comporter 3 chiffres (ex. 110)."
        Return False
    End Function

End Module
