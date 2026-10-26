' -------------------------------------------------------------------------------------------------
' Module      : QueryDomaines
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_domaines.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les domaines actifs ou tous les domaines
' - Vérifier l'unicité du code et du libellé d'un domaine
' - Vérifier si un domaine est utilisé (dossiers, modèles de documents)
' - Mettre à jour un domaine existant
' - Désactiver un domaine (soft-delete)
' - Supprimer physiquement un domaine non utilisé
' - Insérer un nouveau domaine
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_domaine est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_domaine est saisi par l'utilisateur (3 lettres majuscules), unique
' - V1.1.0 (20/10/2026, v2.45.0) : logo_fichier lu avec le domaine ; UpdateLogoDomaine.
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : SupprimerDomaine, plus appelée par le métier,
'   supprimée.
'
' Dépendances :
' - Utilisé par GestionDomaines pour les opérations CRUD sur ref_domaines
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryDomaines

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectDomainesActifs
    ' Version    : V1.1.0
    ' Date       : 20/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner les domaines actifs de la table ref_domaines
    '
    ' Rôle       :
    ' Retourne tous les domaines avec actif = 1, triés par ordre d'affichage et libellé.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionDomaines.GetDomainesActifs
    '
    ' Remarques  :
    ' - Les domaines inactifs ne sont pas retournés
    ' - Utilisé notamment pour alimenter les listes déroulantes métier
    ' - V1.1.0 (20/10/2026) : logo_fichier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectDomainesActifs As String
        Get
            Return "
SELECT
    id_domaine,
    code_domaine,
    libelle_domaine,
    tarif_convention,
    no_show_facturable,
    d.soumis_quota,
    d.id_organisme,
    o.libelle_organisme,
    d.actif,
    d.ordre_affichage,
    d.logo_fichier
FROM ref_domaines d
LEFT JOIN ref_organismes o ON o.id_organisme = d.id_organisme
WHERE d.actif = 1
ORDER BY d.ordre_affichage, d.libelle_domaine;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectDomainesTous
    ' Version    : V1.1.0
    ' Date       : 20/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner tous les domaines de la table ref_domaines
    '
    ' Rôle       :
    ' Retourne tous les domaines, actifs et inactifs, triés par ordre d'affichage et libellé.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionDomaines.GetDomaines
    '
    ' Remarques  :
    ' - Inclut les domaines inactifs (actif = 0)
    ' - Utilisé par l'écran de gestion du référentiel
    ' - V1.1.0 (20/10/2026) : logo_fichier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectDomainesTous As String
        Get
            Return "
SELECT
    id_domaine,
    code_domaine,
    libelle_domaine,
    tarif_convention,
    no_show_facturable,
    d.soumis_quota,
    d.id_organisme,
    o.libelle_organisme,
    d.actif,
    d.ordre_affichage,
    d.logo_fichier
FROM ref_domaines d
LEFT JOIN ref_organismes o ON o.id_organisme = d.id_organisme
ORDER BY d.ordre_affichage, d.libelle_domaine;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountCodeDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier l'existence d'un code de domaine
    '
    ' Rôle       :
    ' Vérifie l'unicité d'un code de domaine, en excluant un identifiant donné.
    '
    ' Paramètres SQL :
    ' - @code : Code de domaine à vérifier
    ' - @id   : Identifiant du domaine à exclure (utile lors de la mise à jour)
    '
    ' Utilisé par :
    ' - GestionDomaines.CodeDomaineExiste
    '
    ' Remarques  :
    ' - Permet de vérifier l'unicité du code lors de la création ou modification
    ' - Retourne un COUNT(*), 0 = code disponible, >0 = code déjà utilisé
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountCodeDomaine As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_domaines
WHERE code_domaine = @code
AND id_domaine <> @id;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountLibelleDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier l'existence d'un libellé de domaine
    '
    ' Rôle       :
    ' Vérifie l'unicité d'un libellé de domaine, en excluant un identifiant donné.
    '
    ' Paramètres SQL :
    ' - @libelle : Libellé de domaine à vérifier
    ' - @id      : Identifiant du domaine à exclure (utile lors de la mise à jour)
    '
    ' Utilisé par :
    ' - GestionDomaines.LibelleDomaineExiste
    '
    ' Remarques  :
    ' - Permet de vérifier l'unicité du libellé lors de la création ou modification
    ' - Retourne un COUNT(*), 0 = libellé disponible, >0 = libellé déjà utilisé
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountLibelleDomaine As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_domaines
WHERE libelle_domaine = @libelle
AND id_domaine <> @id;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountQuotasActifsDomaine
    ' Version    : V1.0.0
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL comptant les règles de quota actives rattachées à un domaine
    '
    ' Rôle       :
    ' Permet de détecter une incohérence avant de retirer l'indicateur soumis_quota d'un domaine
    ' qui possède encore des règles de quota actives dans ref_quotas.
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du domaine à contrôler
    '
    ' Utilisé par :
    ' - GestionDomaines.DomainePossedeQuotasActifs
    '
    ' Remarques  :
    ' - Retourne un COUNT(*), 0 = aucune règle active, >0 = retrait de soumis_quota incohérent
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountQuotasActifsDomaine As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_quotas
WHERE id_domaine = @id
AND actif = 1;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountUsageDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier si un domaine est référencé ailleurs
    '
    ' Rôle       :
    ' Compte le nombre total de références à un domaine dans les tables qui en dépendent
    ' (dossiers et modeles_documents).
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du domaine à vérifier
    '
    ' Utilisé par :
    ' - GestionDomaines.DomaineEstUtilise
    '
    ' Remarques  :
    ' - Retourne un COUNT total, 0 = domaine non utilisé (suppression physique possible),
    '   >0 = domaine utilisé (privilégier la désactivation / soft-delete)
    ' - Doit lister toutes les tables possédant une FK vers ref_domaines(id_domaine)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountUsageDomaine As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM dossiers WHERE id_domaine = @id)
    + (SELECT COUNT(*) FROM modeles_documents WHERE id_domaine = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour un domaine existant
    '
    ' Rôle       :
    ' Met à jour les champs modifiables d'un domaine identifié par son id.
    '
    ' Paramètres SQL :
    ' - @id      : Identifiant du domaine à mettre à jour
    ' - @code    : Nouveau code du domaine (3 lettres majuscules)
    ' - @libelle : Nouveau libellé du domaine
    ' - @actif   : Indique si le domaine est actif (1 ou 0)
    ' - @ordre   : Nouvel ordre d'affichage
    '
    ' Utilisé par :
    ' - GestionDomaines.UpdateDomaine
    '
    ' Remarques :
    ' - id_domaine sert d'identifiant technique et n'est jamais modifié.
    ' - L'unicité du code et du libellé est vérifiée en amont dans la couche métier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateDomaine As String
        Get
            Return "
        UPDATE ref_domaines
        SET code_domaine = @code,
            libelle_domaine = @libelle,
            tarif_convention = @tarifConvention,
            no_show_facturable = @noShowFacturable,
            soumis_quota = @soumisQuota,
            id_organisme = @idOrganisme,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_domaine = @id"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DesactiverDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour désactiver un domaine (soft-delete)
    '
    ' Rôle       :
    ' Désactive un domaine sans suppression physique en base de données.
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du domaine à désactiver
    '
    ' Utilisé par :
    ' - GestionDomaines.DesactiverDomaine
    '
    ' Remarques  :
    ' - À privilégier lorsqu'un domaine est déjà utilisé (FK existantes)
    ' - Le domaine reste présent en base avec actif = 0
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DesactiverDomaine As String
        Get
            Return "
        UPDATE ref_domaines
        SET actif = 0
        WHERE id_domaine = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer un nouveau domaine
    '
    ' Rôle       :
    ' Insère un nouveau domaine dans la table ref_domaines.
    '
    ' Paramètres SQL :
    ' - @code             : Code du domaine (3 lettres majuscules)
    ' - @libelle          : Libellé du domaine
    ' - @tarifConvention  : Tarif résolu par convention plutôt que par le tarif patient (D-POR-1)
    ' - @noShowFacturable : No-show facturé au patient (D-POR-2)
    ' - @soumisQuota      : Domaine soumis à un quota périodique (D-POR-3)
    ' - @actif            : Indique si le domaine est actif (1 ou 0)
    ' - @ordre            : Ordre d'affichage du domaine
    '
    ' Utilisé par :
    ' - GestionDomaines.InsertDomaine
    '
    ' Remarques :
    ' - id_domaine est généré par la base (AUTO_INCREMENT).
    ' - Aucune valeur id ne doit être fournie par l'application.
    ' - L'unicité du code et du libellé est vérifiée en amont dans la couche métier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertDomaine As String
        Get
            Return "
INSERT INTO ref_domaines
(
    code_domaine,
    libelle_domaine,
    tarif_convention,
    no_show_facturable,
    soumis_quota,
    id_organisme,
    actif,
    ordre_affichage
)
VALUES
(
    @code,
    @libelle,
    @tarifConvention,
    @noShowFacturable,
    @soumisQuota,
    @idOrganisme,
    @actif,
    @ordre
);"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateLogoDomaine
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Enregistre le nom du fichier logo d'un domaine (@logo NULL = pas de logo).
    ' Remarques  : À part de UpdateDomaine : le fichier est écrit d'abord, son nom ensuite.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateLogoDomaine As String
        Get
            Return "
UPDATE ref_domaines
SET logo_fichier = @logo
WHERE id_domaine = @id;"
        End Get
    End Property

#End Region

End Module
