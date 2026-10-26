' -------------------------------------------------------------------------------------------------
' Classe   : ModePaiement
' Projet   : Althéa
' Version  : V1.0.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un mode de paiement issu de la table référentielle ref_modes_paiement
' (ex. Espèces, Virement, INAMI).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_mode_paiement est généré par la base (AUTO_INCREMENT).
' - code_mode_paiement est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' - V1.0.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ModePaiement

#Region "Propriétés"

    ' Identifiant unique du mode de paiement (clé primaire, généré par la base)
    Public Property IdModePaiement As ULong

    ' Code du mode de paiement (majuscules, espaces remplacés par _, unique)
    Public Property CodeModePaiement As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleModePaiement As String

    ' Mode réservé à l'utilisation d'un avoir patient (ref_modes_paiement.est_avoir) : jamais proposé à la saisie manuelle.
    Public Property EstAvoir As Boolean

    ' Indique si le mode de paiement est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ligne système (est_systeme) : code utilisé par l'application, non modifiable ni désactivable
    Public Property EstSysteme As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region

End Class
