' -------------------------------------------------------------------------------------------------
' Module      : QueryStatutsDossier
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_statuts_dossier.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les statuts de dossier actifs ou tous les statuts de dossier
' - Vérifier l'unicité du code et du libellé d'un statut de dossier
' - Vérifier si un statut de dossier est utilisé (dossiers)
' - Mettre à jour un statut de dossier existant
' - Désactiver un statut de dossier (soft-delete)
' - Supprimer physiquement un statut de dossier non utilisé
' - Insérer un nouveau statut de dossier
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_statut_dossier est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_statut_dossier est saisi par l'utilisateur (majuscules, espaces remplacés par _), unique
'
' Dépendances :
' - Utilisé par GestionStatutsDossier pour les opérations CRUD sur ref_statuts_dossier
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryStatutsDossier

#Region "SELECT"

    Public ReadOnly Property SelectStatutsDossierActifs As String
        Get
            Return "
SELECT
    id_statut_dossier,
    code_statut_dossier,
    libelle_statut_dossier,
    actif,
    ordre_affichage
FROM ref_statuts_dossier
WHERE actif = 1
ORDER BY ordre_affichage, libelle_statut_dossier;
"
        End Get
    End Property

    Public ReadOnly Property SelectStatutsDossierTous As String
        Get
            Return "
SELECT
    id_statut_dossier,
    code_statut_dossier,
    libelle_statut_dossier,
    actif,
    ordre_affichage
FROM ref_statuts_dossier
ORDER BY ordre_affichage, libelle_statut_dossier;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeStatutDossier As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_statuts_dossier
WHERE code_statut_dossier = @code
AND id_statut_dossier <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleStatutDossier As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_statuts_dossier
WHERE libelle_statut_dossier = @libelle
AND id_statut_dossier <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountUsageStatutDossier As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM dossiers WHERE id_statut_dossier = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateStatutDossier As String
        Get
            Return "
        UPDATE ref_statuts_dossier
        SET code_statut_dossier = @code,
            libelle_statut_dossier = @libelle,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_statut_dossier = @id"
        End Get
    End Property

    Public ReadOnly Property DesactiverStatutDossier As String
        Get
            Return "
        UPDATE ref_statuts_dossier
        SET actif = 0
        WHERE id_statut_dossier = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    Public ReadOnly Property SupprimerStatutDossier As String
        Get
            Return "
        DELETE FROM ref_statuts_dossier
        WHERE id_statut_dossier = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertStatutDossier As String
        Get
            Return "
INSERT INTO ref_statuts_dossier
(
    code_statut_dossier,
    libelle_statut_dossier,
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
