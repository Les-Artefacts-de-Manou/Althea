' -------------------------------------------------------------------------------------------------
' Classe   : Periodicite
' Projet   : Althéa
' Version  : V1.0.0
'
' Rôle :
' Représente une périodicité d'encodage / de facturation issue du référentiel ref_periodicites
' (ex. Mensuelle, Trimestrielle). Rattachée facultativement à un organisme.
'
' Remarques :
' - Aucun accès base de données ici.
' - Aucun comportement n'est déduit du code : la durée est portée par NbMois
'   (Nothing = non périodique / à la demande).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Periodicite

#Region "Propriétés"

    Public Property IdPeriodicite As ULong

    Public Property CodePeriodicite As String

    Public Property LibellePeriodicite As String

    ' Durée de la période en mois ; Nothing = non périodique
    Public Property NbMois As Integer?

    Public Property EstSysteme As Boolean

    Public Property Actif As Boolean

    Public Property OrdreAffichage As Integer

#End Region

    Public Overrides Function ToString() As String
        Return LibellePeriodicite
    End Function

End Class
