-- =============================================================================
-- ALTHÉA — ROLLBACK Migration v2.7.0 (Lot 7 — suivi_en_cours nullable)
-- =============================================================================
-- Annule la migration v2.7.0 :
--   1. Restauration des valeurs NULL à 1 (valeur par défaut historique)
--   2. Colonne suivi_en_cours à nouveau NOT NULL DEFAULT 1
--   3. Suppression de la ligne de version 2.7.0 dans tec_meta_schema
--
-- ATTENTION : à n'exécuter que sur une base de développement.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

UPDATE `patients` SET `suivi_en_cours` = 1 WHERE `suivi_en_cours` IS NULL;

ALTER TABLE `patients`
  MODIFY COLUMN `suivi_en_cours` tinyint(4) NOT NULL DEFAULT 1;

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.7.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
