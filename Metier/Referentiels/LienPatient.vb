' -------------------------------------------------------------------------------------------------
' Classe   : LienPatient
' Projet   : Althéa
' Version  : V1.1.1
' Date     : 25/10/2026
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
' - V1.1.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : ParDefaut.
' - V1.1.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

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

    ' Ligne système (est_systeme) : code utilisé par l'application, non modifiable ni désactivable
    Public Property EstSysteme As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

    ' Proposé par défaut pour un nouveau contact (par_defaut, v2.57.0 ; une seule valeur à la fois)
    Public Property ParDefaut As Boolean

#End Region

End Class
