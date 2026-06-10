' -------------------------------------------------------------------------------------------------
' Module      : QueryStatutsSeance
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_statuts_seance.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les statuts de séance actifs ou tous les statuts de séance
' - Vérifier l'unicité du code et du libellé d'un statut de séance
' - Vérifier si un statut de séance est utilisé (seances)
' - Mettre à jour un statut de séance existant
' - Désactiver un statut de séance (soft-delete)
' - Supprimer physiquement un statut de séance non utilisé
' - Insérer un nouveau statut de séance
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_statut_seance est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_statut_seance est saisi par l'utilisateur (majuscules, espaces remplacés par _), unique
'
' Dépendances :
' - Utilisé par GestionStatutsSeance pour les opérations CRUD sur ref_statuts_seance
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryStatutsSeance

#Region "SELECT"

    Public ReadOnly Property SelectStatutsSeanceActifs As String
        Get
            Return "
SELECT
    id_statut_seance,
    code_statut_seance,
    libelle_statut_seance,
    actif,
    ordre_affichage
FROM ref_statuts_seance
WHERE actif = 1
ORDER BY ordre_affichage, libelle_statut_seance;
"
        End Get
    End Property

    Public ReadOnly Property SelectStatutsSeanceTous As String
        Get
            Return "
SELECT
    id_statut_seance,
    code_statut_seance,
    libelle_statut_seance,
    actif,
    ordre_affichage
FROM ref_statuts_seance
ORDER BY ordre_affichage, libelle_statut_seance;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeStatutSeance As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_statuts_seance
WHERE code_statut_seance = @code
AND id_statut_seance <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleStatutSeance As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_statuts_seance
WHERE libelle_statut_seance = @libelle
AND id_statut_seance <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountUsageStatutSeance As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM seances WHERE id_statut_seance = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateStatutSeance As String
        Get
            Return "
        UPDATE ref_statuts_seance
        SET code_statut_seance = @code,
            libelle_statut_seance = @libelle,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_statut_seance = @id"
        End Get
    End Property

    Public ReadOnly Property DesactiverStatutSeance As String
        Get
            Return "
        UPDATE ref_statuts_seance
        SET actif = 0
        WHERE id_statut_seance = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    Public ReadOnly Property SupprimerStatutSeance As String
        Get
            Return "
        DELETE FROM ref_statuts_seance
        WHERE id_statut_seance = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertStatutSeance As String
        Get
            Return "
INSERT INTO ref_statuts_seance
(
    code_statut_seance,
    libelle_statut_seance,
    actif,
    ordre_affichage
)
VALUES
(
    @code,
    @libelle,
    @actif,
    @ordre
);"
        End Get
    End Property

#End Region

End Module
