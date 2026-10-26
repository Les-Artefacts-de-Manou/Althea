-- =============================================================================
-- ALTHÉA — ROLLBACK migration v2.11.0 (Lot 10b — Refonte Tarif / Nature de séance)
-- =============================================================================
-- Annule les changements de migration_v2.11.0_lot10b_refonte_tarif_nature.sql :
--   1. seances       : restauration de id_type_seance (FK ref_types_seance),
--                      suppression de id_prix_seance + id_nature_seance
--   2. paiements     : suppression de statut_paiement, date_paiement NOT NULL
--   3. lia_prix_seance_domaines : suppression de la table
--   4. ref_natures_seance       : suppression de la table
--   5. tec_meta_schema          : retrait de la ligne 2.11.0
--
-- ATTENTION : ce rollback recrée id_type_seance NOT NULL sur une table seances
--             supposée vide (le script de migration a purgé seances/paiements).
--             Si des séances ont été créées entre-temps, les purger d'abord.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- Purge (les colonnes NOT NULL recréées ne tolèrent pas de lignes orphelines)
DELETE FROM `lia_paiements_seances`;
DELETE FROM `paiements`;
DELETE FROM `seances`;

-- ---------------------------------------------------------------------------
-- SECTION 1 — seances : retour à id_type_seance
-- ---------------------------------------------------------------------------
ALTER TABLE `seances`
  DROP FOREIGN KEY `fk_seances_ref_prix_seance`,
  DROP FOREIGN KEY `fk_seances_ref_natures_seance`;

ALTER TABLE `seances`
  DROP KEY `fk_seances_ref_prix_seance`,
  DROP KEY `fk_seances_ref_natures_seance`,
  DROP COLUMN `id_prix_seance`,
  DROP COLUMN `id_nature_seance`;

ALTER TABLE `seances`
  ADD COLUMN `id_type_seance` bigint(20) unsigned NOT NULL
    AFTER `id_dossier`;

ALTER TABLE `seances`
  ADD KEY `fk_seances_ref_types_seance` (`id_type_seance`),
  ADD CONSTRAINT `fk_seances_ref_types_seance`
       FOREIGN KEY (`id_type_seance`) REFERENCES `ref_types_seance` (`id_type_seance`);

-- ---------------------------------------------------------------------------
-- SECTION 2 — paiements : retrait de statut_paiement, date_paiement NOT NULL
-- ---------------------------------------------------------------------------
ALTER TABLE `paiements`
  DROP COLUMN `statut_paiement`;

ALTER TABLE `paiements`
  MODIFY COLUMN `date_paiement` date NOT NULL;

-- ---------------------------------------------------------------------------
-- SECTION 3 — suppression de lia_prix_seance_domaines
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS `lia_prix_seance_domaines`;

-- ---------------------------------------------------------------------------
-- SECTION 4 — suppression de ref_natures_seance
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS `ref_natures_seance`;

-- ---------------------------------------------------------------------------
-- SECTION 5 — dé-versionnage
-- ---------------------------------------------------------------------------
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.11.0';

-- ---------------------------------------------------------------------------
-- Restauration des flags de session
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
