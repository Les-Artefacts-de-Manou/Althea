' -------------------------------------------------------------------------------------------------
' Classe   : TypeSeance
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un type de séance issu de la table référentielle ref_types_seance
' (ex. Consultation individuelle, Bilan, Séance de groupe, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_type_seance est généré par la base (AUTO_INCREMENT).
' - code_type_seance est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' - PARTICULARITÉ : possède un tarif par défaut (tarif_defaut decimal(10,2) NOT NULL).
' -------------------------------------------------------------------------------------------------
Public Class TypeSeance

#Region "Propriétés"

    ' Identifiant unique du type de séance (clé primaire, généré par la base)
    Public Property IdTypeSeance As ULong

    ' Code du type de séance (majuscules, espaces remplacés par _, unique)
    Public Property CodeTypeSeance As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleTypeSeance As String

    ' Tarif par défaut appliqué pour ce type de séance (decimal(10,2), NOT NULL)
    Public Property TarifDefaut As Decimal

    ' Indique si le type de séance est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
