' -------------------------------------------------------------------------------------------------
' Classe   : TypeRendezVous
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un type de rendez-vous issu de la table référentielle ref_types_rendez_vous
' (ex. Première consultation, Suivi, Bilan, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_type_rendez_vous est généré par la base (AUTO_INCREMENT).
' - code_type_rendez_vous est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' -------------------------------------------------------------------------------------------------
Public Class TypeRendezVous

#Region "Propriétés"

    ' Identifiant unique du type de rendez-vous (clé primaire, généré par la base)
    Public Property IdTypeRendezVous As ULong

    ' Code du type de rendez-vous (majuscules, espaces remplacés par _, unique)
    Public Property CodeTypeRendezVous As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleTypeRendezVous As String

    ' Indique si le type de rendez-vous est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
