' -------------------------------------------------------------------------------------------------
' Classe   : StatutDossier
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un statut de dossier issu de la table référentielle ref_statuts_dossier
' (ex. En cours, Clôturé, En attente, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_statut_dossier est généré par la base (AUTO_INCREMENT).
' - code_statut_dossier est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' -------------------------------------------------------------------------------------------------
Public Class StatutDossier

#Region "Propriétés"

    ' Identifiant unique du statut de dossier (clé primaire, généré par la base)
    Public Property IdStatutDossier As ULong

    ' Code du statut de dossier (majuscules, espaces remplacés par _, unique)
    Public Property CodeStatutDossier As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleStatutDossier As String

    ' Indique si le statut de dossier est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
