' -------------------------------------------------------------------------------------------------
' Classe   : RoleIntervenant
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un rôle d'intervenant issu de la table référentielle ref_roles_intervenant
' (ex. Médecin traitant, Psychologue, Logopède, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_role_intervenant est généré par la base (AUTO_INCREMENT).
' - code_role_intervenant est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' -------------------------------------------------------------------------------------------------
Public Class RoleIntervenant

#Region "Propriétés"

    ' Identifiant unique du rôle d'intervenant (clé primaire, généré par la base)
    Public Property IdRoleIntervenant As ULong

    ' Code du rôle d'intervenant (majuscules, espaces remplacés par _, unique)
    Public Property CodeRoleIntervenant As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleRoleIntervenant As String

    ' Indique si le rôle d'intervenant est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
