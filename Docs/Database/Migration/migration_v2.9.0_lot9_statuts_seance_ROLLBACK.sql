-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.9.0 (Lot 9 — Séances : statuts)
-- =============================================================================
-- Annule migration_v2.9.0_lot9_statuts_seance.sql :
--   1. Suppression des statuts ajoutés (ANNULEE_PRATICIEN, MALADE)
--   2. Restauration de l'ancien statut générique (ANNULEE_PATIENT → ANNULEE)
--   3. Suppression de la ligne de version 2.9.0 dans tec_meta_schema
--
-- Prérequis : aucune séance ne doit référencer les statuts supprimés
--             (base DEV, table seances vide au moment de la migration).
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Suppression des statuts ajoutés en v2.9.0
-- ---------------------------------------------------------------------------

DELETE FROM `ref_statuts_seance`
WHERE `code_statut_seance` IN ('ANNULEE_PRATICIEN', 'MALADE');

-- ---------------------------------------------------------------------------
-- SECTION 2 — Restauration de l'ancien statut générique « Annulée »
--   ANNULEE_PATIENT → ANNULEE
-- ---------------------------------------------------------------------------

UPDATE `ref_statuts_seance`
SET `code_statut_seance`    = 'ANNULEE',
    `libelle_statut_seance` = 'Annulée',
    `ordre_affichage`       = 30
WHERE `code_statut_seance` = 'ANNULEE_PATIENT';

-- ---------------------------------------------------------------------------
-- SECTION 3 — Suppression de la version 2.9.0
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.9.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
