' -------------------------------------------------------------------------------------------------
' Classe   : Pays
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 11/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un pays du référentiel ref_pays (adresses du cabinet, des praticiens, patients,
' contacts et thérapeutes).
'
' Remarques :
' - Aucun accès base de données ici.
' - CodePays est le code ISO 3166-1 alpha-2 : c'est lui (et jamais le libellé) qui sert de clé aux
'   règles de validation (UtilsTelephone, UtilsCodePostal).
' - Les pays dotés de règles de validation sont des lignes système (cadenas).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Pays

#Region "Propriétés"

    Public Property IdPays As ULong

    ' Code ISO 3166-1 alpha-2 (ex. BE, FR)
    Public Property CodePays As String

    Public Property LibellePays As String

    Public Property EstSysteme As Boolean

    Public Property Actif As Boolean

    Public Property OrdreAffichage As Integer

#End Region

    Public Overrides Function ToString() As String
        Return LibellePays
    End Function

End Class
