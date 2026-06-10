' -------------------------------------------------------------------------------------------------
' Classe   : SituationFamiliale
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente une situation familiale issue de la table référentielle ref_situations_familiales
' (ex. Célibataire, Marié(e), Divorcé(e), ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_situation_familiale est généré par la base (AUTO_INCREMENT).
' - code_situation_familiale est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' -------------------------------------------------------------------------------------------------
Public Class SituationFamiliale

#Region "Propriétés"

    ' Identifiant unique de la situation familiale (clé primaire, généré par la base)
    Public Property IdSituationFamiliale As ULong

    ' Code de la situation familiale (majuscules, espaces remplacés par _, unique)
    Public Property CodeSituationFamiliale As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleSituationFamiliale As String

    ' Indique si la situation familiale est active (0 = désactivée, 1 = active)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
