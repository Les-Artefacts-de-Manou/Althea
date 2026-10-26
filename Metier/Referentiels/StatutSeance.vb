' -------------------------------------------------------------------------------------------------
' Classe   : StatutSeance
' Projet   : Althéa
' Version  : V1.2.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un statut de séance issu de la table référentielle ref_statuts_seance
' (ex. Planifiée, Réalisée, Annulée, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_statut_seance est généré par la base (AUTO_INCREMENT).
' - code_statut_seance est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' - V1.1.0 (22/10/2026, audit B.6, v2.53.0) : EstAbsence (absence non excusée).
' - V1.2.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : EstNonTenue et ProposeCreation.
' - V1.2.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class StatutSeance

#Region "Propriétés"

    ' Identifiant unique du statut de séance (clé primaire, généré par la base)
    Public Property IdStatutSeance As ULong

    ' Code du statut de séance (majuscules, espaces remplacés par _, unique)
    Public Property CodeStatutSeance As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleStatutSeance As String

    ' Indique si le statut de séance est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ligne système (est_systeme) : code utilisé par l'application, non modifiable ni désactivable
    Public Property EstSysteme As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

    ' Indique si une seance portant ce statut est due (facturable)
    Public Property EstDu As Boolean

    ' Indique si une séance portant ce statut consomme le quota du domaine
    Public Property ConsommeQuota As Boolean

    ' Absence non excusée du patient (no-show) : non facturée si le domaine ne facture pas les
    ' absences, comptée dans les absences (ref_statuts_seance.est_absence, v2.53.0)
    Public Property EstAbsence As Boolean

    ' Séance non tenue : elle libère son créneau (planning, contrôle de chevauchement) et passer d'un
    ' statut non tenu à un autre ne recontrôle pas l'horaire (ref_statuts_seance.est_non_tenue,
    ' v2.57.0 ; remplace les listes de codes, audit C.3)
    Public Property EstNonTenue As Boolean

    ' Statut proposé à la création d'une séance (ref_statuts_seance.propose_creation, v2.57.0)
    Public Property ProposeCreation As Boolean

#End Region

End Class
