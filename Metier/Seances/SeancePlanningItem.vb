' -------------------------------------------------------------------------------------------------
' Classe      : SeancePlanningItem
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Modèle de présentation pour la grille de consultation du planning du jour (PlanningJourViewer)
' et pour le contrôle métier de chevauchement de créneaux (GestionSeances.ChercherChevauchement).
'
' Responsabilités :
' - Étendre SeanceListeItem avec l'identité du patient et le libellé du domaine du dossier, afin
'   de permettre l'affichage d'un planning global (tous dossiers confondus, un seul thérapeute).
'
' Remarques   :
' - Cette classe ne contient aucun accès base de données.
' - Elle est produite par GestionSeances à partir de la requête SelectSeancesParDate.
' - V1.1.0 (23/10/2026, recherche transversale R2, ADR-52) : sert aussi la recherche des séances
'   d'une période (SelectSeancesParPeriode) ; identifiant et code du patient, code du dossier (aller
'   au dossier, à la fiche) et PatientAffichage (« Nom Prénom », tri par nom).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class SeancePlanningItem
    Inherits SeanceListeItem

#Region "Propriétés"

    ' Nom du patient (issu de la jointure dossiers -> patients)
    Public Property NomPatient As String

    ' Prénom du patient (issu de la jointure dossiers -> patients)
    Public Property PrenomPatient As String

    ' Libellé du domaine du dossier (issu de la jointure dossiers -> ref_domaines)
    Public Property LibelleDomaine As String

    ' Patient et dossier de la séance (aller à la fiche, au dossier : recherche transversale)
    Public Property IdPatient As Long
    Public Property CodePatient As String
    Public Property CodeDossier As String

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : NomCompletPatient
    ' Version    : V1.0.0
    ' Date       : 16/01/2025
    '
    ' Rôle       :
    ' Retourne le nom complet du patient (prénom + nom) pour l'affichage en grille.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property NomCompletPatient As String
        Get
            Return $"{PrenomPatient} {NomPatient}".Trim()
        End Get
    End Property

    ' Patient pour une liste triable par nom : « Nom Prénom » (comme la recherche par code).
    Public ReadOnly Property PatientAffichage As String
        Get
            Return $"{NomPatient} {PrenomPatient}".Trim()
        End Get
    End Property

#End Region

End Class
