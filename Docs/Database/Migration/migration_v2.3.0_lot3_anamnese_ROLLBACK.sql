-- =============================================================================
-- ALTHÉA — Rollback migration v2.3.0 (Lot 3 — Anamnèse patient)
-- =============================================================================
-- Annule intégralement migration_v2.3.0_lot3_anamnese.sql dans l'ordre inverse.
-- À exécuter uniquement sur la base de développement.
--
-- Ordre d'exécution (inverse de la migration) :
--   2. Supprimer la ligne de version dans tec_meta_schema
--   1. Retirer patients.anamnese_rtf + patients.anamnese_txt
--
-- Remarque : le contenu de l'anamnèse (RTF et texte) est définitivement perdu
-- au rollback (suppression des colonnes).
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;


-- ---------------------------------------------------------------------------
-- SECTION 2 — Supprimer la ligne de version
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.3.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 1 — patients : retirer l'anamnèse enrichie (D-Q19)
-- ---------------------------------------------------------------------------

ALTER TABLE `patients`
  DROP COLUMN `anamnese_rtf`,
  DROP COLUMN `anamnese_txt`;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN migration_v2.3.0_lot3_anamnese_ROLLBACK.sql
-- =============================================================================
