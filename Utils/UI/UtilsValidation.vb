' -------------------------------------------------------------------------------------------------
' Module      : UtilsValidation
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 02/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fournit des fonctions de validation réutilisables dans l'application.
'
' Responsabilités :
' - Valider qu'une valeur correspond au type attendu (IsValidValueForType)
' - Valider les valeurs booléennes (IsValidBooleanValue)
' - Valider les valeurs entières (IsValidIntegerValue)
' - Valider les valeurs décimales (IsValidDecimalValue)
' - Valider les valeurs de type date (IsValidDateValue)
'
' Types supportés :
' - STRING, PATH : Toujours valide
' - BOOL : 0, 1, true, false (case-insensitive)
' - INT : Nombres entiers (Integer.TryParse)
' - DECIMAL : Nombres décimaux (Decimal.TryParse avec culture courante)
' - DATE : Dates valides (Date.TryParse avec culture courante)
'
' Remarques   :
' - Module statique (fonctions partagées)
' - Réutilisable dans les UserControls et formulaires
'
' Dépendances :
' - Utilisé par UC_Parametres pour valider les valeurs avant UPDATE/INSERT

' Imports :
' - System.Globalization
' -------------------------------------------------------------------------------------------------
Imports System.Globalization

Public Module UtilsValidation

#Region "Validation type / valeur"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsValidValueForType

    ' Version    : V1.0.0
    ' Date       : 02/05/2026

    ' Rôle       : Valide qu’une valeur correspond au type de valeur attendu

    ' Paramètres  : value - La valeur à valider.
    '               typeValeur - Le type attendu de la valeur.
    '               messageErreur - Message d'erreur en cas de validation échouée.

    ' Retour     : True si la valeur est valide, False sinon.
    '-------------------------------------------------------------------------------------------------
    Public Function IsValidValueForType(value As String,
                                        typeValeur As String,
                                        ByRef messageErreur As String) As Boolean

        messageErreur = String.Empty

        Dim normalizedType As String = If(typeValeur, "").Trim().ToUpperInvariant()
        Dim normalizedValue As String = If(value, "").Trim()

        Select Case normalizedType

            Case "STRING", "PATH"
                Return True

            Case "BOOL"
                Return IsValidBooleanValue(normalizedValue, messageErreur)

            Case "INT"
                Return IsValidIntegerValue(normalizedValue, messageErreur)

            Case "DECIMAL"
                Return IsValidDecimalValue(normalizedValue, messageErreur)

            Case "DATE"
                Return IsValidDateValue(normalizedValue, messageErreur)

            Case Else
                messageErreur = "Le type de valeur n’est pas reconnu."
                Return False

        End Select

    End Function

#End Region

#Region "Validations spécialisées"
    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsValidBooleanValue

    ' Version    : V1.0.0
    ' Date       : 02/05/2026

    ' Rôle       : Valide qu’une valeur correspond au type de valeur attendu, Boolean dans ce cas.

    ' Paramètres  : value - La valeur à valider.
    '               typeValeur - Le type attendu de la valeur.
    '               messageErreur - Message d'erreur en cas de validation échouée.

    ' Retour     : True si la valeur est valide, False sinon.
    '-------------------------------------------------------------------------------------------------
    Public Function IsValidBooleanValue(value As String,
                                        ByRef messageErreur As String) As Boolean

        If value = "0" OrElse value = "1" OrElse
           value.Equals("true", StringComparison.OrdinalIgnoreCase) OrElse
           value.Equals("false", StringComparison.OrdinalIgnoreCase) Then

            Return True

        End If

        messageErreur = "La valeur doit être un booléen : 0, 1, true ou false."
        Return False

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsValidIntegerValue
    ' Version    : V1.0.0
    ' Date       : 02/05/2026

    ' Rôle       : Valide qu’une valeur correspond au type de valeur attendu, Integer dans ce cas.

    ' Paramètres  : value - La valeur à valider.
    '               typeValeur - Le type attendu de la valeur.
    '               messageErreur - Message d'erreur en cas de validation échouée.

    ' Retour     : True si la valeur est valide, False sinon.
    '-------------------------------------------------------------------------------------------------
    Public Function IsValidIntegerValue(value As String,
                                        ByRef messageErreur As String) As Boolean

        Dim result As Integer

        If Integer.TryParse(value, result) Then
            Return True
        End If

        messageErreur = "La valeur doit être un nombre entier."
        Return False

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsValidDecimalValue

    ' Version    : V1.0.0
    ' Date       : 02/05/2026

    ' Rôle       : Valide qu’une valeur correspond au type de valeur attendu, Decimal dans ce cas.

    ' Paramètres  : value - La valeur à valider.
    '               typeValeur - Le type attendu de la valeur.
    '               messageErreur - Message d'erreur en cas de validation échouée.

    ' Retour     : True si la valeur est valide, False sinon.
    '-------------------------------------------------------------------------------------------------
    Public Function IsValidDecimalValue(value As String,
                                        ByRef messageErreur As String) As Boolean

        Dim result As Decimal

        If Decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, result) Then
            Return True
        End If

        messageErreur = "La valeur doit être un nombre décimal."
        Return False

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsValidDateValue

    ' Version    : V1.0.0
    ' Date       : 02/05/2026

    ' Rôle       : Valide qu’une valeur correspond au type de valeur attendu, Date dans ce cas.

    ' Paramètres  : value - La valeur à valider.
    '               typeValeur - Le type attendu de la valeur.
    '               messageErreur - Message d'erreur en cas de validation échouée.

    ' Retour     : True si la valeur est valide, False sinon.
    '-------------------------------------------------------------------------------------------------
    Public Function IsValidDateValue(value As String,
                                     ByRef messageErreur As String) As Boolean

        Dim result As Date

        If Date.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, result) Then
            Return True
        End If

        messageErreur = "La valeur doit être une date valide."
        Return False

    End Function

#End Region

End Module
