-- =============================================================================
-- ALTHÉA — Migration de schéma v2.11.0 (Lot 10b — Refonte Tarif / Nature de séance)
-- =============================================================================
-- Objectif :
--   Dissocier définitivement les deux notions historiquement fusionnées dans
--   « Type de séance » :
--     * le TARIF de la séance  → désormais porté par ref_prix_seance
--       (50/55/60 €, INAMI…), filtré selon le domaine du dossier ;
--     * la NATURE de la séance → nouveau référentiel ref_natures_seance
--       (Bilan, Traitement, Fin de traitement, RV intervenant, Autre),
--       SANS aucune incidence sur le prix.
--
-- Décisions appliquées (session de conception du jour) :
--   D-TAR-4 : le tarif d'une séance provient de ref_prix_seance (plus de
--             ref_types_seance.tarif_defaut dans le flux séance).
--   D-TAR-5 : rattachement tarif ↔ domaine via table de liaison
--             lia_prix_seance_domaines (many-to-many, flexible) : un même
--             tarif peut servir plusieurs domaines ; Realism = tarif INAMI.
--   D-NAT-1 : la nature de séance (Bilan, Traitement, …) devient un référentiel
--             dédié, sans prix.
--   D-SEA-1 : seances.id_type_seance est remplacée par id_prix_seance +
--             id_nature_seance. tarif_seance (snapshot) est conservé.
--   D-PAY-4 : à la création d'une séance, une ligne de paiement « en attente »
--             est ouverte (résolue plus tard, patient OU INAMI). paiements
--             reçoit statut_paiement + date_paiement rendue nullable.
--
-- Ordre d'exécution :
--   1. ref_natures_seance      : création + seed (5 natures)
--   2. lia_prix_seance_domaines: création + seed data-driven (INAMI vs autres)
--   3. seances                 : remplacement de id_type_seance par
--                                id_prix_seance + id_nature_seance
--   4. paiements               : statut_paiement + date_paiement nullable
--   5. Versionnage dans tec_meta_schema
--
-- Prérequis : base en développement (les rares séances de test sont supprimées).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.11.0_lot10b_refonte_tarif_nature_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- Nettoyage préalable — séances et paiements de test
-- (base de développement : aucune donnée métier réelle à préserver)
-- ---------------------------------------------------------------------------
DELETE FROM `lia_paiements_seances`;
DELETE FROM `paiements`;
DELETE FROM `seances`;


-- ---------------------------------------------------------------------------
-- SECTION 1 — Création de ref_natures_seance (D-NAT-1)
-- Nature clinique de la séance, SANS prix. Ajoutable à la volée par le user.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_natures_seance`;
CREATE TABLE `ref_natures_seance` (
  `id_nature_seance`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_nature_seance`    varchar(30)         NOT NULL,
  `libelle_nature_seance` varchar(100)        NOT NULL,
  `actif`                 tinyint(1)          NOT NULL DEFAULT 1,
  `ordre_affichage`       int(11)             NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_nature_seance`),
  UNIQUE KEY `uq_ref_natures_seance_code`    (`code_nature_seance`),
  UNIQUE KEY `uq_ref_natures_seance_libelle` (`libelle_nature_seance`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
LOCK TABLES `ref_natures_seance` WRITE;
INSERT INTO `ref_natures_seance` VALUES
  (1, 'BILAN',            'Bilan',              1, 10),
  (2, 'TRAITEMENT',       'Traitement',         1, 20),
  (3, 'FIN_TRAITEMENT',   'Fin de traitement',  1, 30),
  (4, 'RV_INTERVENANT',   'RV intervenant',     1, 40),
  (5, 'AUTRE',            'Autre',              1, 50);
UNLOCK TABLES;
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 2 — Création de lia_prix_seance_domaines (D-TAR-5)
-- Rattache chaque tarif (ref_prix_seance) à un ou plusieurs domaines
-- (ref_domaines) pour filtrer les tarifs proposés selon le domaine du dossier.
-- Seed data-driven : le tarif INAMI est rattaché aux domaines est_inami = 1 ;
-- les tarifs patient (non INAMI) sont rattachés aux domaines est_inami = 0.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `lia_prix_seance_domaines`;
CREATE TABLE `lia_prix_seance_domaines` (
  `id_lia_prix_seance_domaine` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_prix_seance`             bigint(20) unsigned NOT NULL,
  `id_domaine`                 bigint(20) unsigned NOT NULL,
  PRIMARY KEY (`id_lia_prix_seance_domaine`),
  UNIQUE KEY `uq_lia_prix_domaine` (`id_prix_seance`,`id_domaine`),
  KEY `fk_lia_prix_domaine_domaine` (`id_domaine`),
  CONSTRAINT `fk_lia_prix_domaine_prix`
       FOREIGN KEY (`id_prix_seance`) REFERENCES `ref_prix_seance` (`id_prix_seance`)
       ON DELETE CASCADE,
  CONSTRAINT `fk_lia_prix_domaine_domaine`
       FOREIGN KEY (`id_domaine`)     REFERENCES `ref_domaines` (`id_domaine`)
       ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;

-- Tarif INAMI ↔ domaines INAMI (Realism)
INSERT INTO `lia_prix_seance_domaines` (`id_prix_seance`, `id_domaine`)
SELECT p.id_prix_seance, d.id_domaine
FROM `ref_prix_seance` p
CROSS JOIN `ref_domaines` d
WHERE p.code_prix_seance = 'INAMI'
  AND d.est_inami = 1;

-- Tarifs patient (non INAMI) ↔ domaines non INAMI (Psy, TSA, Grapho…)
INSERT INTO `lia_prix_seance_domaines` (`id_prix_seance`, `id_domaine`)
SELECT p.id_prix_seance, d.id_domaine
FROM `ref_prix_seance` p
CROSS JOIN `ref_domaines` d
WHERE p.code_prix_seance <> 'INAMI'
  AND d.est_inami = 0;

COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 3 — Refonte de seances (D-SEA-1)
-- Remplacement de id_type_seance par id_prix_seance + id_nature_seance.
-- La table est vide (nettoyage préalable) : suppression et recréation des
-- colonnes/FK sans reprise de données.
-- ---------------------------------------------------------------------------

ALTER TABLE `seances`
  DROP FOREIGN KEY `fk_seances_ref_types_seance`;

ALTER TABLE `seances`
  DROP KEY `fk_seances_ref_types_seance`,
  DROP COLUMN `id_type_seance`;

ALTER TABLE `seances`
  ADD COLUMN `id_prix_seance` bigint(20) unsigned NOT NULL
    COMMENT 'Tarif appliqué à la séance (ref_prix_seance) — D-SEA-1'
    AFTER `id_dossier`,
  ADD COLUMN `id_nature_seance` bigint(20) unsigned NOT NULL
    COMMENT 'Nature clinique de la séance (ref_natures_seance) — D-SEA-1'
    AFTER `id_prix_seance`;

ALTER TABLE `seances`
  ADD KEY `fk_seances_ref_prix_seance`   (`id_prix_seance`),
  ADD KEY `fk_seances_ref_natures_seance` (`id_nature_seance`),
  ADD CONSTRAINT `fk_seances_ref_prix_seance`
       FOREIGN KEY (`id_prix_seance`)   REFERENCES `ref_prix_seance` (`id_prix_seance`),
  ADD CONSTRAINT `fk_seances_ref_natures_seance`
       FOREIGN KEY (`id_nature_seance`) REFERENCES `ref_natures_seance` (`id_nature_seance`);


-- ---------------------------------------------------------------------------
-- SECTION 4 — paiements : ligne « en attente » (D-PAY-4)
-- Une séance ouvre une ligne de paiement en attente, résolue plus tard.
-- date_paiement devient nullable (NULL tant que non encaissé) ; ajout d'un
-- statut_paiement (EN_ATTENTE / PAYE).
-- ---------------------------------------------------------------------------

ALTER TABLE `paiements`
  MODIFY COLUMN `date_paiement` date DEFAULT NULL
    COMMENT 'Date d''encaissement — NULL tant que le paiement est en attente';

ALTER TABLE `paiements`
  ADD COLUMN `statut_paiement` varchar(20) NOT NULL DEFAULT 'EN_ATTENTE'
    COMMENT 'EN_ATTENTE (séance créée, non encaissée) ou PAYE — D-PAY-4'
    AFTER `montant`;


-- ---------------------------------------------------------------------------
-- SECTION 5 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.11.0', 'Lot 10b refonte tarif/nature : ref_natures_seance, lia_prix_seance_domaines, seances.id_prix_seance + id_nature_seance, paiements.statut_paiement', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

-- ---------------------------------------------------------------------------
-- Restauration des flags de session
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
