' -------------------------------------------------------------------------------------------------
' Module      : CryptoManagerDPAPI
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 22/04/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gère le chiffrement et le déchiffrement des données sensibles via DPAPI.
'
' Responsabilités :
' - Chiffrer une chaîne en Base64
' - Déchiffrer une chaîne Base64
'
' Remarques   :
' - Utilise DataProtectionScope.CurrentUser
' - Le secret protégé n’est lisible que par le même utilisateur Windows
' - Aucun lien direct avec l’UI ou MariaDB

' Imports     :
' - System
' - System.Security.Cryptography
' - System.Text
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System
Imports System.Security.Cryptography
Imports System.Text

Public Module CryptoManagerDPAPI

#Region "Chiffrement"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Protect
    ' Version     : V1.0.0
    ' Date        : 22/04/2026

    ' Rôle       :
    ' - Chiffre une chaîne en utilisant DPAPI et retourne le résultat en Base64.
    '
    ' Paramètres :
    ' - plainText : texte à chiffrer
    '
    ' Retour     :
    ' - Chaîne Base64 chiffrée

    ' Remarques  :
    ' - Utilise DataProtectionScope.CurrentUser

    ' Exception :
    ' - ArgumentException : si le texte à chiffrer est null ou vide
    ' -------------------------------------------------------------------------------------------------
    Public Function Protect(plainText As String) As String

        If String.IsNullOrWhiteSpace(plainText) Then
            Throw New ArgumentException("Le texte à chiffrer ne peut pas être vide.", NameOf(plainText))
        End If

        Dim plainBytes As Byte() = Encoding.UTF8.GetBytes(plainText)
        Dim protectedBytes As Byte() = ProtectedData.Protect(plainBytes, Nothing, DataProtectionScope.CurrentUser)

        Return Convert.ToBase64String(protectedBytes)

    End Function

#End Region

#Region "Déchiffrement"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Unprotect
    ' Version     : V1.0.0
    ' Date        : 22/04/2026

    ' Rôle       :
    ' - Déchiffre une chaîne Base64 protégée via DPAPI.
    ' 
    ' Paramètres :
    ' - protectedBase64 : texte chiffré en Base64
    '
    ' Retour     :
    ' - String :  Chaîne déchiffrée

    ' Remarques  : 
    ' - Utilise DataProtectionScope.CurrentUser

    ' Exception :
    ' - ArgumentException : si le texte chiffré est null ou vide
    ' -------------------------------------------------------------------------------------------------
    Public Function Unprotect(protectedBase64 As String) As String

        If String.IsNullOrWhiteSpace(protectedBase64) Then
            Throw New ArgumentException("Le texte chiffré ne peut pas être vide.", NameOf(protectedBase64))
        End If

        Dim protectedBytes As Byte() = Convert.FromBase64String(protectedBase64)
        Dim plainBytes As Byte() = ProtectedData.Unprotect(protectedBytes, Nothing, DataProtectionScope.CurrentUser)

        Return Encoding.UTF8.GetString(plainBytes)

    End Function

#End Region

End Module
