-- =============================================================================
-- ALTHÉA — Migration de schéma v2.15.0 (Lot 14 — Portabilité par référentiels)
-- =============================================================================
-- Contexte métier :
--   L'application doit pouvoir accueillir un NOUVEAU DOMAINE (conventionné ou
--   non) par simple paramétrage, sans modification ni recompilation du code.
--   Trois obstacles subsistaient :
--
--   1. Le tarif du circuit conventionné était résolu par une constante
--      applicative (CODE_PRIX_INAMI = 'INAMI') pointant vers un tarif GLOBAL.
--      Deux domaines conventionnés auraient donc partagé le même montant.
--   2. ref_domaines.est_inami est un booléen unique qui gouverne à lui seul
--      TROIS comportements distincts : résolution du tarif, gratuité du
--      no-show, et soumission au quota. Un domaine conventionné dont le
--      no-show serait facturé était donc inexprimable.
--   3. Les natures de séance relevaient du circuit conventionné via un simple
--      texte libre (code_prestation_inami non vide), sans aucun rattachement
--      au domaine : deux domaines aux nomenclatures différentes n'auraient pas
--      pu être distingués.
--
-- Choix de modélisation :
--   * est_inami est CONSERVÉ et synchronisé, non supprimé. Il reste lu par
--     QueryPatients, QuerySeances et QueryTarification. Le retirer dans ce lot
--     ferait courir un risque de régression silencieuse sur des écrans non
--     couverts par la recette. Sa suppression fera l'objet d'un lot ultérieur,
--     une fois tous les appelants migrés.
--   * Les trois comportements sont éclatés en trois colonnes explicites,
--     initialisées depuis est_inami afin de reproduire À L'IDENTIQUE la
--     situation actuelle. Cette migration ne change donc aucun comportement.
--   * Les tarifs restent rattachés aux domaines en N-N via la table
--     lia_prix_seance_domaines qui EXISTE DÉJÀ : un nouveau domaine peut ainsi
--     partager les tarifs de Psychologie, Grapho ou RCA. Elle est enrichie de
--     dates de validité, sur le même modèle que ref_quotas_inami, afin qu'une
--     révision tarifaire ne modifie pas rétroactivement des séances déjà
--     facturées.
--   * Le rattachement des natures aux domaines suit le même principe N-N :
--     une nature peut servir à plusieurs domaines.
--
-- Ordre d'exécution :
--   1. Attributs de comportement sur ref_domaines
--   2. Historisation de lia_prix_seance_domaines
--   3. Création de lia_natures_seance_domaines et alimentation initiale
--   4. Versionner dans tec_meta_schema
--
-- Prérequis : lot 13 (v2.14.0) appliqué.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.15.0_lot14_portabilite_domaines_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Attributs de comportement sur ref_domaines
--   Chaque colonne exprime UNE règle et une seule, là où est_inami en portait
--   trois. Les valeurs par défaut correspondent au cas général (domaine non
--   conventionné) ; la mise à jour qui suit rétablit le comportement actuel
--   pour les domaines conventionnés existants.
-- ---------------------------------------------------------------------------

ALTER TABLE `ref_domaines`
  ADD COLUMN `tarif_convention` tinyint(1) NOT NULL DEFAULT 0
    COMMENT 'Tarif resolu via lia_prix_seance_domaines et non via le tarif patient du dossier — D-POR-1'
    AFTER `est_inami`,
  ADD COLUMN `no_show_facturable` tinyint(1) NOT NULL DEFAULT 1
    COMMENT 'No-show facture au patient (0 pour un circuit conventionne qui ne rembourse pas) — D-POR-2'
    AFTER `tarif_convention`,
  ADD COLUMN `soumis_quota` tinyint(1) NOT NULL DEFAULT 0
    COMMENT 'Domaine soumis a un quota periodique parametre dans ref_quotas_inami — D-POR-3'
    AFTER `no_show_facturable`;

-- Reprise de l'existant : un domaine aujourd'hui marqué est_inami = 1 se voit
-- attribuer exactement les trois comportements que ce drapeau impliquait.
SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;

UPDATE `ref_domaines`
SET `tarif_convention`   = 1,
    `no_show_facturable` = 0,
    `soumis_quota`       = 1
WHERE `est_inami` = 1;

COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Historisation de lia_prix_seance_domaines
--   La table existait déjà en N-N simple. L'ajout des dates de validité permet
--   de faire évoluer un tarif sans réécrire l'historique : on clôture la ligne
--   courante et on en ouvre une nouvelle.
--
--   La clé d'unicité doit être RECONSTRUITE : uq_lia_prix_domaine interdisait
--   un même couple (prix, domaine) en double, ce qui empêcherait désormais
--   toute succession de périodes. La date de début entre donc dans la clé.
-- ---------------------------------------------------------------------------

ALTER TABLE `lia_prix_seance_domaines`
  ADD COLUMN `date_debut_validite` date NOT NULL DEFAULT '2000-01-01'
    COMMENT 'Premier jour d''application du tarif pour ce domaine — D-POR-4'
    AFTER `id_domaine`,
  ADD COLUMN `date_fin_validite` date DEFAULT NULL
    COMMENT 'Dernier jour d''application, NULL tant que le tarif est en vigueur — D-POR-4'
    AFTER `date_debut_validite`,
  ADD COLUMN `actif` tinyint(1) NOT NULL DEFAULT 1
    COMMENT 'Rattachement actif (desactivation sans perte d''historique) — D-POR-4'
    AFTER `date_fin_validite`;

-- Date de début volontairement ancienne : les rattachements existants doivent
-- rester valides pour toutes les séances déjà encodées, y compris anciennes.
ALTER TABLE `lia_prix_seance_domaines`
  DROP INDEX `uq_lia_prix_domaine`,
  ADD UNIQUE KEY `uq_lia_prix_domaine_debut`
    (`id_prix_seance`, `id_domaine`, `date_debut_validite`),
  ADD CONSTRAINT `ck_lia_prix_domaine_periode`
    CHECK (`date_fin_validite` IS NULL OR `date_fin_validite` >= `date_debut_validite`);

-- ---------------------------------------------------------------------------
-- SECTION 3 — Rattachement des natures de séance aux domaines
--   Remplace le test « code_prestation_inami non vide » qui ne permettait pas
--   de distinguer deux domaines conventionnés.
--   ON DELETE CASCADE : supprimer un domaine ou une nature doit faire
--   disparaître le rattachement, qui n'a aucun sens isolément. C'est une
--   différence assumée avec ref_quotas_inami, où le RESTRICT protège une règle
--   de facturation porteuse de sens métier propre.
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `lia_natures_seance_domaines` (
  `id_lia_nature_domaine` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_nature_seance` bigint(20) unsigned NOT NULL
    COMMENT 'Nature de seance proposee pour le domaine — D-POR-5',
  `id_domaine` bigint(20) unsigned NOT NULL
    COMMENT 'Domaine auquel la nature est rattachee — D-POR-5',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_lia_nature_domaine`),
  UNIQUE KEY `uq_lia_nature_domaine` (`id_nature_seance`, `id_domaine`),
  KEY `fk_lia_nature_domaine_domaine` (`id_domaine`),
  CONSTRAINT `fk_lia_nature_domaine_nature` FOREIGN KEY (`id_nature_seance`)
    REFERENCES `ref_natures_seance` (`id_nature_seance`) ON DELETE CASCADE,
  CONSTRAINT `fk_lia_nature_domaine_domaine` FOREIGN KEY (`id_domaine`)
    REFERENCES `ref_domaines` (`id_domaine`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;

-- Reprise de la règle actuelle, à l'identique :
--   - les natures porteuses d'un code de prestation vont aux domaines
--     conventionnés (elles consomment le quota) ;
--   - les natures neutres vont aux domaines non conventionnés.
INSERT IGNORE INTO `lia_natures_seance_domaines`
  (`id_nature_seance`, `id_domaine`, `actif`)
SELECT ns.`id_nature_seance`, d.`id_domaine`, 1
FROM `ref_natures_seance` ns
CROSS JOIN `ref_domaines` d
WHERE (ns.`code_prestation_inami` IS NOT NULL
       AND ns.`code_prestation_inami` <> ''
       AND d.`tarif_convention` = 1)
   OR ((ns.`code_prestation_inami` IS NULL
        OR ns.`code_prestation_inami` = '')
       AND d.`tarif_convention` = 0);

COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

-- ---------------------------------------------------------------------------
-- SECTION 4 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.15.0', 'Lot 14 portabilite : attributs de comportement sur ref_domaines (tarif_convention, no_show_facturable, soumis_quota), historisation de lia_prix_seance_domaines et liaison natures/domaines', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
