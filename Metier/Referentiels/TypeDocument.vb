' -------------------------------------------------------------------------------------------------
' Classe   : TypeDocument
' Projet   : Althéa
' Version  : V1.0.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un type de document issu de la table référentielle ref_types_documents
' (ex. Rapport, Bilan, Courrier, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_type_document est généré par la base (AUTO_INCREMENT).
' - code_type_document est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' - V1.0.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class TypeDocument

#Region "Propriétés"

    ' Identifiant unique du type de document (clé primaire, généré par la base)
    Public Property IdTypeDocument As ULong

    ' Code du type de document (majuscules, espaces remplacés par _, unique)
    Public Property CodeTypeDocument As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleTypeDocument As String

    ' Indique si le type de document est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ligne système (est_systeme) : code utilisé par l'application, non modifiable ni désactivable
    Public Property EstSysteme As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
