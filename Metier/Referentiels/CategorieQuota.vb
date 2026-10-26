' -------------------------------------------------------------------------------------------------
' Classe   : CategorieQuota
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 20/09/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente une catégorie de quota propre à un domaine (ex. Realism : Soutien, Traitement),
' issue de la table référentielle ref_categories_quota.
'
' Remarques :
' - Aucun accès base de données.
' - Le nombre de catégories d'un domaine est libre : aucune catégorie n'est codée en dur.
' - Une nature consomme au plus une catégorie par domaine (lia_natures_seance_domaines.id_categorie_quota).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class CategorieQuota

#Region "Propriétés"

    ' Identifiant unique de la catégorie (généré par la base)
    Public Property IdCategorieQuota As ULong

    ' Domaine (convention) auquel appartient la catégorie
    Public Property IdDomaine As ULong

    ' Code court, unique au sein du domaine
    Public Property CodeCategorie As String = String.Empty

    ' Libellé affiché, unique au sein du domaine
    Public Property LibelleCategorie As String = String.Empty

    Public Property Actif As Boolean = True

    Public Property OrdreAffichage As Integer

#End Region

#Region "Helpers"

    ' Abréviation affichée dans la grille des séances (« 1 S », « 2 T ») : initiale du libellé.
    Public ReadOnly Property Abreviation As String
        Get
            Return AbreviationDe(LibelleCategorie)
        End Get
    End Property

    Public Shared Function AbreviationDe(libelle As String) As String
        If String.IsNullOrWhiteSpace(libelle) Then Return "?"
        Return libelle.Trim().Substring(0, 1).ToUpperInvariant()
    End Function

    Public Overrides Function ToString() As String
        Return LibelleCategorie
    End Function

#End Region

End Class
