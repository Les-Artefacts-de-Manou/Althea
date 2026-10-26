' -------------------------------------------------------------------------------------------------
' Classe      : Specialite
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Spécialité d'un thérapeute issue du référentiel ref_specialites (Logopède, Pédiatre…).
'
' Remarques   :
' - Aucun accès base de données.
' - Décisions de Joëlle (23/10/2026) : spécialité choisie dans une liste (une seule par thérapeute)
'   plutôt que tapée librement. Aucun comportement ne dépend d'une valeur : pas de ligne système.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Specialite

    ' Identifiant (clé primaire, généré par la base) ; 0 = entrée « aucune » des listes
    Public Property IdSpecialite As ULong

    ' Code (majuscules, espaces remplacés par _, unique)
    Public Property CodeSpecialite As String = String.Empty

    ' Libellé affiché (unique, casse et accents ignorés)
    Public Property LibelleSpecialite As String = String.Empty

    Public Property Actif As Boolean = True

    Public Property OrdreAffichage As Integer

End Class
