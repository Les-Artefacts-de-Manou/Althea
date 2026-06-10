' -------------------------------------------------------------------------------------------------
' Classe   : StatutSeance
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un statut de séance issu de la table référentielle ref_statuts_seance
' (ex. Planifiée, Réalisée, Annulée, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_statut_seance est généré par la base (AUTO_INCREMENT).
' - code_statut_seance est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' -------------------------------------------------------------------------------------------------
Public Class StatutSeance

#Region "Propriétés"

    ' Identifiant unique du statut de séance (clé primaire, généré par la base)
    Public Property IdStatutSeance As ULong

    ' Code du statut de séance (majuscules, espaces remplacés par _, unique)
    Public Property CodeStatutSeance As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleStatutSeance As String

    ' Indique si le statut de séance est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
