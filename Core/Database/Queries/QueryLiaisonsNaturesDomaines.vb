' -------------------------------------------------------------------------------------------------
' Module      : QueryLiaisonsNaturesDomaines
' Projet      : Althéa
' Version     : V1.1.0
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table de liaison lia_natures_seance_domaines, qui définit
' quelles natures de séance sont proposées à la saisie pour un domaine donné (D-POR-5).
'
' Responsabilités :
' - Sélectionner les natures rattachées à un domaine, et les domaines rattachés à une nature
' - Sélectionner les natures encore disponibles pour un rattachement (évite les doublons à l'écran)
' - Détecter l'existence d'un rattachement et son utilisation réelle par des séances
' - Insérer, activer/désactiver et supprimer un rattachement
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères) : aucun accès base de données ici
' - Cette liaison n'est PAS historisée : une nature est proposée ou non pour un domaine.
'   La désactivation (actif = 0) remplace la suppression dès que des séances existent.
' - La contrainte uq_lia_nature_domaine garantit l'unicité du couple (nature, domaine)
'
' Dépendances :
' - Utilisé par GestionLiaisonsNaturesDomaines
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryLiaisonsNaturesDomaines

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectLiaisonsParDomaine
    ' Type       : String (ReadOnly)
    '
    ' Description : Natures de séance rattachées à un domaine, actives ou non
    '
    ' Paramètres SQL :
    ' - @idDomaine : Identifiant du domaine
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.GetLiaisonsParDomaine
    '
    ' Remarques  :
    ' - Inclut les rattachements désactivés : le filtrage éventuel est fait par l'appelant
    ' - actif_nature permet de signaler à l'écran une nature désactivée dans son référentiel
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectLiaisonsParDomaine As String
        Get
            Return "
SELECT
    lia.id_lia_nature_domaine,
    lia.id_nature_seance,
    lia.id_domaine,
    ns.code_nature_seance,
    ns.libelle_nature_seance,
    lia.id_categorie_quota,
    cq.libelle_categorie AS libelle_categorie_quota,
    ns.actif AS actif_nature,
    dom.code_domaine,
    dom.libelle_domaine,
    lia.actif
FROM lia_natures_seance_domaines lia
INNER JOIN ref_natures_seance ns ON ns.id_nature_seance = lia.id_nature_seance
INNER JOIN ref_domaines dom ON dom.id_domaine = lia.id_domaine
LEFT JOIN ref_categories_quota cq ON cq.id_categorie_quota = lia.id_categorie_quota
WHERE lia.id_domaine = @idDomaine
ORDER BY ns.ordre_affichage, ns.libelle_nature_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectLiaisonsParNature
    ' Type       : String (ReadOnly)
    '
    ' Description : Domaines auxquels une nature de séance est rattachée, actifs ou non
    '
    ' Paramètres SQL :
    ' - @idNatureSeance : Identifiant de la nature de séance
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.GetLiaisonsParNature
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectLiaisonsParNature As String
        Get
            Return "
SELECT
    lia.id_lia_nature_domaine,
    lia.id_nature_seance,
    lia.id_domaine,
    ns.code_nature_seance,
    ns.libelle_nature_seance,
    lia.id_categorie_quota,
    cq.libelle_categorie AS libelle_categorie_quota,
    ns.actif AS actif_nature,
    dom.code_domaine,
    dom.libelle_domaine,
    lia.actif
FROM lia_natures_seance_domaines lia
INNER JOIN ref_natures_seance ns ON ns.id_nature_seance = lia.id_nature_seance
INNER JOIN ref_domaines dom ON dom.id_domaine = lia.id_domaine
LEFT JOIN ref_categories_quota cq ON cq.id_categorie_quota = lia.id_categorie_quota
WHERE lia.id_nature_seance = @idNatureSeance
ORDER BY dom.ordre_affichage, dom.libelle_domaine;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectNaturesDisponiblesPourDomaine
    ' Type       : String (ReadOnly)
    '
    ' Description : Natures de séance actives pas encore rattachées à un domaine
    '
    ' Paramètres SQL :
    ' - @idDomaine : Identifiant du domaine
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.GetNaturesDisponiblesPourDomaine
    '
    ' Remarques  :
    ' - Alimente la liste de sélection du panneau : l'utilisateur ne peut pas créer un doublon
    '   qui serait de toute façon refusé par la contrainte d'unicité
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectNaturesDisponiblesPourDomaine As String
        Get
            Return "
SELECT
    ns.id_nature_seance,
    ns.code_nature_seance,
    ns.libelle_nature_seance,
    ns.actif,
    ns.ordre_affichage
FROM ref_natures_seance ns
WHERE ns.actif = 1
  AND NOT EXISTS (
        SELECT 1
        FROM lia_natures_seance_domaines lia
        WHERE lia.id_nature_seance = ns.id_nature_seance
          AND lia.id_domaine = @idDomaine
      )
ORDER BY ns.ordre_affichage, ns.libelle_nature_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectDomainesDisponiblesPourNature
    ' Type       : String (ReadOnly)
    '
    ' Description : Domaines actifs auxquels une nature de séance n'est pas encore rattachée
    '
    ' Paramètres SQL :
    ' - @idNatureSeance : Identifiant de la nature de séance
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.GetDomainesDisponiblesPourNature
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectDomainesDisponiblesPourNature As String
        Get
            Return "
SELECT
    dom.id_domaine,
    dom.code_domaine,
    dom.libelle_domaine,
    dom.tarif_convention,
    dom.no_show_facturable,
    dom.soumis_quota,
    dom.actif,
    dom.ordre_affichage
FROM ref_domaines dom
WHERE dom.actif = 1
  AND NOT EXISTS (
        SELECT 1
        FROM lia_natures_seance_domaines lia
        WHERE lia.id_domaine = dom.id_domaine
          AND lia.id_nature_seance = @idNatureSeance
      )
ORDER BY dom.ordre_affichage, dom.libelle_domaine;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountLiaisonExistante
    ' Type       : String (ReadOnly)
    '
    ' Description : Nombre de rattachements déjà enregistrés pour un couple nature / domaine
    '
    ' Paramètres SQL :
    ' - @idNatureSeance : Identifiant de la nature de séance
    ' - @idDomaine      : Identifiant du domaine
    ' - @idExclu        : Rattachement à ignorer (0 lors d'une création)
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.LiaisonExiste
    '
    ' Remarques  :
    ' - Permet d'afficher un message clair plutôt que de laisser remonter l'erreur d'unicité SQL
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountLiaisonExistante As String
        Get
            Return "
SELECT COUNT(*)
FROM lia_natures_seance_domaines
WHERE id_nature_seance = @idNatureSeance
  AND id_domaine = @idDomaine
  AND id_lia_nature_domaine <> @idExclu;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountSeancesNatureDomaine
    ' Type       : String (ReadOnly)
    '
    ' Description : Nombre de séances déjà saisies avec cette nature dans ce domaine
    '
    ' Paramètres SQL :
    ' - @idNatureSeance : Identifiant de la nature de séance
    ' - @idDomaine      : Identifiant du domaine
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.LiaisonUtiliseeParSeances
    '
    ' Remarques  :
    ' - Le domaine d'une séance est porté par son dossier : la jointure passe par dossiers
    ' - Utilisé pour interdire la suppression et proposer la désactivation à la place
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountSeancesNatureDomaine As String
        Get
            Return "
SELECT COUNT(*)
FROM seances s
INNER JOIN dossiers d ON d.id_dossier = s.id_dossier
WHERE s.id_nature_seance = @idNatureSeance
  AND d.id_domaine = @idDomaine;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountLiaisonsActivesDomaine
    ' Type       : String (ReadOnly)
    '
    ' Description : Nombre de natures actives rattachées à un domaine
    '
    ' Paramètres SQL :
    ' - @idDomaine : Identifiant du domaine
    ' - @idExclu   : Rattachement à ignorer (celui en cours de désactivation)
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.CompterLiaisonsActivesDomaine
    '
    ' Remarques  :
    ' - Un domaine sans aucune nature active rendrait la saisie de séance impossible :
    '   l'écran doit avertir l'utilisateur avant d'en arriver là
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountLiaisonsActivesDomaine As String
        Get
            Return "
SELECT COUNT(*)
FROM lia_natures_seance_domaines lia
INNER JOIN ref_natures_seance ns ON ns.id_nature_seance = lia.id_nature_seance
WHERE lia.id_domaine = @idDomaine
  AND lia.actif = 1
  AND ns.actif = 1
  AND lia.id_lia_nature_domaine <> @idExclu;
"
        End Get
    End Property

#End Region

#Region "INSERT / UPDATE / DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Crée le rattachement d'une nature de séance à un domaine
    '
    ' Paramètres SQL :
    ' - @idNatureSeance : Identifiant de la nature de séance
    ' - @idDomaine      : Identifiant du domaine
    ' - @idCategorieQuota : Catégorie de quota consommée dans ce domaine (NULL = hors quota) ;
    '                     la clé étrangère composite garantit qu'elle appartient au même domaine
    ' - @actif          : État du rattachement
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.InsertLiaison
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertLiaison As String
        Get
            Return "
INSERT INTO lia_natures_seance_domaines
    (id_nature_seance, id_domaine, id_categorie_quota, actif)
VALUES
    (@idNatureSeance, @idDomaine, @idCategorieQuota, @actif);
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DefinirActivationLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Active ou désactive un rattachement nature / domaine
    '
    ' Paramètres SQL :
    ' - @id    : Identifiant du rattachement
    ' - @actif : Nouvel état
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.DefinirActivationLiaison
    '
    ' Remarques  :
    ' - Désactivation = retrait de la liste de saisie sans toucher aux séances déjà enregistrées
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DefinirActivationLiaison As String
        Get
            Return "
UPDATE lia_natures_seance_domaines
SET actif = @actif
WHERE id_lia_nature_domaine = @id;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DefinirCategorieQuotaLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Fixe la catégorie de quota consommée par la nature dans ce domaine
    '
    ' Paramètres SQL :
    ' - @id                 : Identifiant du rattachement
    ' - @id_categorie_quota : Catégorie (NULL = hors quota) ; la clé étrangère composite impose
    '                         qu'elle appartienne au domaine du rattachement
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DefinirCategorieQuotaLiaison As String
        Get
            Return "
UPDATE lia_natures_seance_domaines
SET id_categorie_quota = @id_categorie_quota
WHERE id_lia_nature_domaine = @id;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SupprimerLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Supprime un rattachement nature / domaine créé par erreur
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du rattachement
    '
    ' Utilisé par :
    ' - GestionLiaisonsNaturesDomaines.SupprimerLiaison
    '
    ' Remarques  :
    ' - À n'exécuter qu'après contrôle par SelectCountSeancesNatureDomaine
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SupprimerLiaison As String
        Get
            Return "
DELETE FROM lia_natures_seance_domaines
WHERE id_lia_nature_domaine = @id;
"
        End Get
    End Property

#End Region

End Module
