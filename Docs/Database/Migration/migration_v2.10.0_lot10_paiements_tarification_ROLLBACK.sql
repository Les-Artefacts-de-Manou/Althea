-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.10.0 (Lot 10 — Paiements & Tarification)
-- =============================================================================
-- Annule migration_v2.10.0_lot10_paiements_tarification.sql (ordre inverse) :
--   1. ref_statuts_seance : suppression de la colonne est_du
--   2. ref_domaines       : suppression de la colonne est_inami
--   3. dossiers           : suppression de la FK + colonne id_prix_seance
--   4. ref_prix_seance    : suppression de la table
--   5. ref_modes_paiement : suppression de la table
--   6. Suppression de la ligne de version 2.10.0 dans tec_meta_schema
--
-- Prérequis : aucune séance/dossier ne doit dépendre des objets supprimés
--             (base DEV). Le rollback échoue si des paiements référencent encore
--             un mode via mode_paiement — mode_paiement étant un varchar libre,
--             aucune FK ne bloque, mais vérifier la cohérence des données.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — ref_statuts_seance : retrait de est_du
-- ---------------------------------------------------------------------------

ALTER TABLE `ref_statuts_seance`
  DROP COLUMN `est_du`;

-- ---------------------------------------------------------------------------
-- SECTION 2 — ref_domaines : retrait de est_inami
-- ---------------------------------------------------------------------------

ALTER TABLE `ref_domaines`
  DROP COLUMN `est_inami`;

-- ---------------------------------------------------------------------------
-- SECTION 3 — dossiers : retrait de la FK puis de id_prix_seance
-- ---------------------------------------------------------------------------

ALTER TABLE `dossiers`
  DROP FOREIGN KEY `fk_dossiers_ref_prix_seance`;
ALTER TABLE `dossiers`
  DROP KEY `fk_dossiers_ref_prix_seance`;
ALTER TABLE `dossiers`
  DROP COLUMN `id_prix_seance`;

-- ---------------------------------------------------------------------------
-- SECTION 4 — Suppression de ref_prix_seance
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_prix_seance`;

-- ---------------------------------------------------------------------------
-- SECTION 5 — Suppression de ref_modes_paiement
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_modes_paiement`;

-- ---------------------------------------------------------------------------
-- SECTION 6 — Suppression de la version 2.10.0
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.10.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

