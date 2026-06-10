' -------------------------------------------------------------------------------------------------
' Module      : QueryRolesIntervenant
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_roles_intervenant.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les rôles d'intervenant actifs ou tous les rôles d'intervenant
' - Vérifier l'unicité du code et du libellé d'un rôle d'intervenant
' - Vérifier si un rôle d'intervenant est utilisé (autres_suivis_patient)
' - Mettre à jour un rôle d'intervenant existant
' - Désactiver un rôle d'intervenant (soft-delete)
' - Supprimer physiquement un rôle d'intervenant non utilisé
' - Insérer un nouveau rôle d'intervenant
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_role_intervenant est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_role_intervenant est saisi par l'utilisateur (majuscules, espaces remplacés par _), unique
'
' Dépendances :
' - Utilisé par GestionRolesIntervenant pour les opérations CRUD sur ref_roles_intervenant
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryRolesIntervenant

#Region "SELECT"

    Public ReadOnly Property SelectRolesIntervenantActifs As String
        Get
            Return "
SELECT
    id_role_intervenant,
    code_role_intervenant,
    libelle_role_intervenant,
    actif,
    ordre_affichage
FROM ref_roles_intervenant
WHERE actif = 1
ORDER BY ordre_affichage, libelle_role_intervenant;
"
        End Get
    End Property

    Public ReadOnly Property SelectRolesIntervenantTous As String
        Get
            Return "
SELECT
    id_role_intervenant,
    code_role_intervenant,
    libelle_role_intervenant,
    actif,
    ordre_affichage
FROM ref_roles_intervenant
ORDER BY ordre_affichage, libelle_role_intervenant;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeRoleIntervenant As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_roles_intervenant
WHERE code_role_intervenant = @code
AND id_role_intervenant <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleRoleIntervenant As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_roles_intervenant
WHERE libelle_role_intervenant = @libelle
AND id_role_intervenant <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountUsageRoleIntervenant As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM autres_suivis_patient WHERE id_role_intervenant = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateRoleIntervenant As String
        Get
            Return "
        UPDATE ref_roles_intervenant
        SET code_role_intervenant = @code,
            libelle_role_intervenant = @libelle,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_role_intervenant = @id"
        End Get
    End Property

    Public ReadOnly Property DesactiverRoleIntervenant As String
        Get
            Return "
        UPDATE ref_roles_intervenant
        SET actif = 0
        WHERE id_role_intervenant = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    Public ReadOnly Property SupprimerRoleIntervenant As String
        Get
            Return "
        DELETE FROM ref_roles_intervenant
        WHERE id_role_intervenant = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertRoleIntervenant As String
        Get
            Return "
INSERT INTO ref_roles_intervenant
(
    code_role_intervenant,
    libelle_role_intervenant,
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
