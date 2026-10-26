' -------------------------------------------------------------------------------------------------
' Classe   : QuotaPlafond
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 20/09/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Plafond d'une catégorie de quota pour une règle datée (table ref_quotas_plafonds).
'
' Remarques :
' - Aucun accès base de données.
' - Les libellés de catégorie sont issus de la jointure, en lecture seule.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class QuotaPlafond

    Public Property IdCategorieQuota As ULong

    Public Property CodeCategorie As String = String.Empty

    Public Property LibelleCategorie As String = String.Empty

    Public Property OrdreAffichage As Integer

    ' Nombre de séances remboursées par période pour la catégorie
    Public Property Plafond As Integer

    Public ReadOnly Property Abreviation As String
        Get
            Return CategorieQuota.AbreviationDe(LibelleCategorie)
        End Get
    End Property

End Class
