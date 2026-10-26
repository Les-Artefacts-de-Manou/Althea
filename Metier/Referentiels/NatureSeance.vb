' -------------------------------------------------------------------------------------------------
' Classe   : NatureSeance
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 15/09/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente la nature clinique d'une séance issue de la table référentielle ref_natures_seance
' (ex. Bilan, Traitement, Fin de traitement, RV intervenant, Autre).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_nature_seance est généré par la base (AUTO_INCREMENT).
' - code_nature_seance est saisi par l'utilisateur (majuscules, espaces remplacés par _) et unique.
' - PARTICULARITÉ : la nature de séance N'A AUCUNE incidence sur le prix (le tarif provient
'   exclusivement de ref_prix_seance).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class NatureSeance

#Region "Propriétés"

    ' Identifiant unique de la nature de séance (clé primaire, généré par la base)
    Public Property IdNatureSeance As ULong

    ' Code de la nature de séance (majuscules, espaces remplacés par _, unique)
    Public Property CodeNatureSeance As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleNatureSeance As String

    ' Indique si la nature de séance est active (0 = désactivée, 1 = active)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
