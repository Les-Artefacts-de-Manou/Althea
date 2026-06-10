' -------------------------------------------------------------------------------------------------
' Classe   : Domaine
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un domaine de prise en charge issu de la table référentielle ref_domaines
' (ex. Psychologie, Graphothérapie, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_domaine est généré par la base (AUTO_INCREMENT).
' - code_domaine est saisi par l'utilisateur (3 lettres majuscules) et doit être unique.
' -------------------------------------------------------------------------------------------------
Public Class Domaine

#Region "Propriétés"

    ' Identifiant unique du domaine (clé primaire, généré par la base)
    Public Property IdDomaine As ULong

    ' Code court du domaine (3 lettres majuscules, unique)
    Public Property CodeDomaine As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleDomaine As String

    ' Indique si le domaine est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
