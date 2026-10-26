' -------------------------------------------------------------------------------------------------
' Classe   : Organisme
' Projet   : Althéa
' Version  : V1.1.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un organisme issu de la table référentielle ref_organismes : organisme de
' remboursement (ex. INAMI) ou mutualité (organisme assureur), selon TypeOrganisme.
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Un domaine peut être rattaché à un organisme de remboursement (facultatif : NULL = aucun).
' - Un patient peut être rattaché à une mutualité (référentiels de saisie, v2.34.0).
' - Aucun comportement métier n'est déduit du code de l'organisme.
' - V1.1.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Organisme

#Region "Propriétés"

    ' Identifiant unique de l'organisme (clé primaire, généré par la base)
    Public Property IdOrganisme As ULong

    ' Code court de l'organisme (unique)
    Public Property CodeOrganisme As String

    ' Libellé affiché de l'organisme (unique)
    Public Property LibelleOrganisme As String

    ' Nature de l'organisme (remboursement / mutualité) ; verrouillée sur une ligne système
    Public Property TypeOrganisme As TypeOrganisme

    ' Coordonnées de contact (lot 29, toutes facultatives)
    Public Property Adresse As String
    Public Property Telephone As String
    Public Property Telephone2 As String
    Public Property Email As String
    Public Property Email2 As String
    Public Property SiteWeb As String
    Public Property UrlPlateforme As String
    Public Property ProcedureEncodage As String

    ' Périodicité d'encodage (ref_periodicites) ; 0 = aucune (NULL en base)
    Public Property IdPeriodicite As ULong
    Public Property LibellePeriodicite As String

    ' Ligne système : code, type et activation verrouillés
    Public Property EstSysteme As Boolean

    ' Indique si l'organisme est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
