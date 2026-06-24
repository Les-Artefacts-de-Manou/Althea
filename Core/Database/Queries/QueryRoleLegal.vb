' -------------------------------------------------------------------------------------------------
' Module      : QueryRoleLegal
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 14/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_role_legal.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les rôles légaux actifs ou tous les rôles légaux
' - Vérifier l'unicité du code et du libellé d'un rôle légal
' - Vérifier si un rôle légal est utilisé (contacts de la famille du patient)
' - Mettre à jour un rôle légal existant
' - Désactiver un rôle légal (soft-delete)
' - Supprimer physiquement un rôle légal non utilisé
' - Insérer un nouveau rôle légal
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_role_legal est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_role_legal est saisi par l'utilisateur (majuscules, espaces remplacés par _), unique
'
' Dépendances :
' - Utilisé par GestionRoleLegal pour les opérations CRUD sur ref_role_legal
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryRoleLegal

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectRolesLegauxActifs
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner les rôles légaux actifs de la table ref_role_legal
    '
    ' Rôle       :
    ' Retourne tous les rôles légaux avec actif = 1, triés par ordre d'affichage et libellé.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionRoleLegal.GetRolesLegauxActifs
    '
    ' Remarques  :
    ' - Les rôles légaux inactifs ne sont pas retournés
    ' - Utilisé notamment pour alimenter les listes déroulantes métier
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectRolesLegauxActifs As String
        Get
            Return "
SELECT
    id_role_legal,
    code_role_legal,
    libelle_role_legal,
    actif,
    ordre_affichage
FROM ref_role_legal
WHERE actif = 1
ORDER BY ordre_affichage, libelle_role_legal;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectRolesLegauxTous
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner tous les rôles légaux de la table ref_role_legal
    '
    ' Rôle       :
    ' Retourne tous les rôles légaux, actifs et inactifs, triés par ordre d'affichage et libellé.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionRoleLegal.GetRolesLegaux
    '
    ' Remarques  :
    ' - Inclut les rôles légaux inactifs (actif = 0)
    ' - Utilisé par l'écran de gestion du référentiel
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectRolesLegauxTous As String
        Get
            Return "
SELECT
    id_role_legal,
    code_role_legal,
    libelle_role_legal,
    actif,
    ordre_affichage
FROM ref_role_legal
ORDER BY ordre_affichage, libelle_role_legal;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountCodeRoleLegal
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier l'existence d'un code de rôle légal
    '
    ' Rôle       :
    ' Vérifie l'unicité d'un code de rôle légal, en excluant un identifiant donné.
    '
    ' Paramètres SQL :
    ' - @code : Code de rôle légal à vérifier
    ' - @id   : Identifiant du rôle légal à exclure (utile lors de la mise à jour)
    '
    ' Utilisé par :
    ' - GestionRoleLegal.CodeRoleLegalExiste
    '
    ' Remarques  :
    ' - Permet de vérifier l'unicité du code lors de la création ou modification
    ' - Retourne un COUNT(*), 0 = code disponible, >0 = code déjà utilisé
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountCodeRoleLegal As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_role_legal
WHERE code_role_legal = @code
AND id_role_legal <> @id;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountLibelleRoleLegal
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier l'existence d'un libellé de rôle légal
    '
    ' Rôle       :
    ' Vérifie l'unicité d'un libellé de rôle légal, en excluant un identifiant donné.
    '
    ' Paramètres SQL :
    ' - @libelle : Libellé de rôle légal à vérifier
    ' - @id      : Identifiant du rôle légal à exclure (utile lors de la mise à jour)
    '
    ' Utilisé par :
    ' - GestionRoleLegal.LibelleRoleLegalExiste
    '
    ' Remarques  :
    ' - Permet de vérifier l'unicité du libellé lors de la création ou modification
    ' - Retourne un COUNT(*), 0 = libellé disponible, >0 = libellé déjà utilisé
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountLibelleRoleLegal As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_role_legal
WHERE libelle_role_legal = @libelle
AND id_role_legal <> @id;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountUsageRoleLegal
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier si un rôle légal est référencé ailleurs
    '
    ' Rôle       :
    ' Compte le nombre total de références à un rôle légal dans les tables qui en dépendent
    ' (famille_contacts).
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du rôle légal à vérifier
    '
    ' Utilisé par :
    ' - GestionRoleLegal.RoleLegalEstUtilise
    '
    ' Remarques  :
    ' - Retourne un COUNT total, 0 = rôle légal non utilisé (suppression physique possible),
    '   >0 = rôle légal utilisé (privilégier la désactivation / soft-delete)
    ' - Doit lister toutes les tables possédant une FK vers ref_role_legal(id_role_legal)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountUsageRoleLegal As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM famille_contacts WHERE id_role_legal = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateRoleLegal
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour un rôle légal existant
    '
    ' Rôle       :
    ' Met à jour les champs modifiables d'un rôle légal identifié par son id.
    '
    ' Paramètres SQL :
    ' - @id      : Identifiant du rôle légal à mettre à jour
    ' - @code    : Nouveau code du rôle légal (majuscules, espaces remplacés par _)
    ' - @libelle : Nouveau libellé du rôle légal
    ' - @actif   : Indique si le rôle légal est actif (1 ou 0)
    ' - @ordre   : Nouvel ordre d'affichage
    '
    ' Utilisé par :
    ' - GestionRoleLegal.UpdateRoleLegal
    '
    ' Remarques :
    ' - id_role_legal sert d'identifiant technique et n'est jamais modifié.
    ' - L'unicité du code et du libellé est vérifiée en amont dans la couche métier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateRoleLegal As String
        Get
            Return "
        UPDATE ref_role_legal
        SET code_role_legal = @code,
            libelle_role_legal = @libelle,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_role_legal = @id"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DesactiverRoleLegal
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour désactiver un rôle légal (soft-delete)
    '
    ' Rôle       :
    ' Désactive un rôle légal sans suppression physique en base de données.
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du rôle légal à désactiver
    '
    ' Utilisé par :
    ' - GestionRoleLegal.DesactiverRoleLegal
    '
    ' Remarques  :
    ' - À privilégier lorsqu'un rôle légal est déjà utilisé (FK existantes)
    ' - Le rôle légal reste présent en base avec actif = 0
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DesactiverRoleLegal As String
        Get
            Return "
        UPDATE ref_role_legal
        SET actif = 0
        WHERE id_role_legal = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SupprimerRoleLegal
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour supprimer physiquement un rôle légal
    '
    ' Rôle       :
    ' Supprime définitivement un rôle légal de la base de données.
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du rôle légal à supprimer
    '
    ' Utilisé par :
    ' - GestionRoleLegal.SupprimerRoleLegal
    '
    ' Remarques  :
    ' - À n'utiliser que pour un rôle légal NON utilisé (voir SelectCountUsageRoleLegal)
    ' - Si le rôle légal est référencé, privilégier DesactiverRoleLegal (soft-delete)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SupprimerRoleLegal As String
        Get
            Return "
        DELETE FROM ref_role_legal
        WHERE id_role_legal = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertRoleLegal
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer un nouveau rôle légal
    '
    ' Rôle       :
    ' Insère un nouveau rôle légal dans la table ref_role_legal.
    '
    ' Paramètres SQL :
    ' - @code    : Code du rôle légal (majuscules, espaces remplacés par _)
    ' - @libelle : Libellé du rôle légal
    ' - @actif   : Indique si le rôle légal est actif (1 ou 0)
    ' - @ordre   : Ordre d'affichage du rôle légal
    '
    ' Utilisé par :
    ' - GestionRoleLegal.InsertRoleLegal
    '
    ' Remarques :
    ' - id_role_legal est généré par la base (AUTO_INCREMENT).
    ' - Aucune valeur id ne doit être fournie par l'application.
    ' - L'unicité du code et du libellé est vérifiée en amont dans la couche métier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertRoleLegal As String
        Get
            Return "
INSERT INTO ref_role_legal
(
    code_role_legal,
    libelle_role_legal,
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
