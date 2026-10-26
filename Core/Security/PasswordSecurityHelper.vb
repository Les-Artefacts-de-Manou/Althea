' -------------------------------------------------------------------------------------------------
' Module      : PasswordSecurityHelper
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 07/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise la gestion sécurisée des mots de passe utilisateurs avec PBKDF2 SHA256.
'
' Responsabilités :
' - Génération de sel cryptographique
' - Génération de hash PBKDF2
' - Vérification de mot de passe
' - Comparaison sécurisée des hash
'
' Explications techniques :
'
' SEL (Salt) :
' Valeur aléatoire unique (32 bytes) ajoutée à chaque mot de passe avant hachage.
' Garantit que deux utilisateurs avec le même mot de passe auront des hash différents,
' protégeant contre les rainbow tables et attaques par dictionnaire.
'
' PBKDF2 (Password-Based Key Derivation Function 2) :
' Algorithme de dérivation de clé qui transforme un mot de passe en hash sécurisé.
' Intentionnellement lent (100 000 itérations) pour résister aux attaques par force brute.
' Standard industriel recommandé par le NIST.
'
' SHA256 (Secure Hash Algorithm) :
' Algorithme de hachage cryptographique produisant un hash de 256 bits (32 bytes).
' Unidirectionnel (impossible de retrouver le mot de passe) et déterministe.
'
' Rfc2898DeriveBytes :
' Classe .NET implémentant PBKDF2 selon la RFC 2898 (standard IETF).
' Combine mot de passe + sel avec SHA256 sur 100 000 itérations.
'
' Processus de sécurisation :
' 1. Création compte : Mot de passe + Sel aléatoire → PBKDF2 → Hash (stocké en base)
' 2. Connexion : Mot de passe saisi + Sel stocké → PBKDF2 → Comparaison avec hash stocké
'
' Remarques   :
' - Aucun mot de passe n'est jamais stocké en clair.
' - La comparaison utilise FixedTimeEquals pour éviter les timing attacks.
' - Les mots de passe doivent respecter une politique de complexité suffisante.
'
' Imports     :
' - System.Security.Cryptography (PBKDF2, SHA256, RandomNumberGenerator)
' - System.Text (encodage)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Security.Cryptography
Imports System.Text

Public Module PasswordSecurityHelper

#Region "Constantes"

    ' Taille du sel en bytes (32 bytes = 256 bits)
    Private Const SaltSize As Integer = 32

    ' Taille du hash en bytes (32 bytes = 256 bits)
    Private Const HashSize As Integer = 32

    ' Nombre d'itérations de PBKDF2
    Private Const IterationCount As Integer = 100000

#End Region

#Region "Génération de sel"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GenerateSalt
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Génère un sel cryptographique aléatoire de 32 bytes.
    '
    ' Retour     :
    ' - String : Sel encodé en Base64
    '
    ' Remarques  :
    ' - Utilise RandomNumberGenerator pour garantir une valeur cryptographiquement sûre.
    ' - Appelé lors de la création ou modification d'un mot de passe utilisateur.
    ' -------------------------------------------------------------------------------------------------
    Public Function GenerateSalt() As String

        Dim saltBytes(SaltSize - 1) As Byte

        Using rng As RandomNumberGenerator = RandomNumberGenerator.Create()

            rng.GetBytes(saltBytes)

        End Using

        Return Convert.ToBase64String(saltBytes)

    End Function

#End Region

#Region "Génération de hash"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : HashPassword
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Génère un hash sécurisé du mot de passe avec PBKDF2 SHA256.
    '
    ' Paramètres :
    ' - password : Le mot de passe en clair à hasher.
    ' - salt     : Le sel en Base64 (généré par GenerateSalt).
    '
    ' Retour     :
    ' - String : Hash PBKDF2 SHA256 encodé en Base64 (32 bytes).
    '
    ' Remarques  :
    ' - Utilise 100 000 itérations pour résister aux attaques par force brute.
    ' - Le hash résultant doit être stocké en base de données avec le sel.
    ' - Ne jamais stocker le mot de passe en clair.
    ' -------------------------------------------------------------------------------------------------
    Public Function HashPassword(password As String, salt As String) As String

        Dim saltBytes As Byte() = Convert.FromBase64String(salt)

        Using pbkdf2 As New Rfc2898DeriveBytes(
            password,
            saltBytes,
            IterationCount,
            HashAlgorithmName.SHA256)

            Dim hashBytes As Byte() = pbkdf2.GetBytes(HashSize)

            Return Convert.ToBase64String(hashBytes)

        End Using

    End Function

#End Region

#Region "Vérification"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : VerifyPassword
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Vérifie qu'un mot de passe saisi correspond au hash stocké en base de données.
    '
    ' Paramètres :
    ' - password   : Le mot de passe saisi par l'utilisateur.
    ' - storedHash : Le hash PBKDF2 stocké en base (Base64).
    ' - storedSalt : Le sel stocké en base (Base64).
    '
    ' Retour     :
    ' - Boolean : True si le mot de passe est valide, False sinon.
    '
    ' Remarques  :
    ' - Utilise CryptographicOperations.FixedTimeEquals pour éviter les timing attacks.
    ' - Le mot de passe est re-hashé avec le sel stocké puis comparé au hash stocké.
    ' - La comparaison prend le même temps quel que soit le résultat (sécurité).
    ' -------------------------------------------------------------------------------------------------
    Public Function VerifyPassword(
        password As String,
        storedHash As String,
        storedSalt As String
    ) As Boolean

        Dim computedHash As String =
            HashPassword(password, storedSalt)

        Return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computedHash),
            Convert.FromBase64String(storedHash)
        )

    End Function

#End Region

End Module
