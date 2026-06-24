' -------------------------------------------------------------------------------------------------
' Classe   : RoleLegal
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 14/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un rôle légal d'un contact de l'entourage du patient, issu de la table
' référentielle ref_role_legal (ex. Autorité parentale, Représentant légal, Personne autorisée,
' Contact d'urgence).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_role_legal est généré par la base (AUTO_INCREMENT).
' - code_role_legal est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' - Un contact possède un rôle légal unique (FK obligatoire famille_contacts.id_role_legal).
' -------------------------------------------------------------------------------------------------
Public Class RoleLegal

#Region "Propriétés"

    ' Identifiant unique du rôle légal (clé primaire, généré par la base)
    Public Property IdRoleLegal As ULong

    ' Code du rôle légal (majuscules, espaces remplacés par _, unique)
    Public Property CodeRoleLegal As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleRoleLegal As String

    ' Indique si le rôle légal est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
