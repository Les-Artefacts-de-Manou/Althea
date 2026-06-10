' -------------------------------------------------------------------------------------------------
' Classe   : LienPatient
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un lien de parenté/relation avec le patient issu de la table référentielle
' ref_liens_patient (ex. Mère, Père, Tuteur légal, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_lien_patient est généré par la base (AUTO_INCREMENT).
' - code_lien_patient est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' -------------------------------------------------------------------------------------------------
Public Class LienPatient

#Region "Propriétés"

    ' Identifiant unique du lien patient (clé primaire, généré par la base)
    Public Property IdLienPatient As ULong

    ' Code du lien patient (majuscules, espaces remplacés par _, unique)
    Public Property CodeLienPatient As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleLienPatient As String

    ' Indique si le lien patient est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
