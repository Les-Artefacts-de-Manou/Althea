' -------------------------------------------------------------------------------------------------
' Classe      : Lateralite
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Latéralité du patient issue du référentiel ref_lateralites (Droitier, Gaucher, Ambidextre…).
'
' Remarques   :
' - Aucun accès base de données.
' - Décision de Joëlle (20/10/2026) : référentiel en base plutôt qu'une liste écrite dans le code ;
'   en graphothérapie, d'autres valeurs peuvent compter (gaucher contrarié…). Aucun comportement
'   ne dépend d'une valeur : pas de ligne système.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Lateralite

    ' Identifiant (clé primaire, généré par la base) ; 0 = entrée « aucune » des listes
    Public Property IdLateralite As ULong

    ' Code (majuscules, espaces remplacés par _, unique)
    Public Property CodeLateralite As String = String.Empty

    ' Libellé affiché (unique)
    Public Property LibelleLateralite As String = String.Empty

    Public Property Actif As Boolean = True

    Public Property OrdreAffichage As Integer

End Class
