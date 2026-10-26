-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.8.0 (Lot 8 — Patient : is_actif)
-- =============================================================================
-- Annule migration_v2.8.0_lot8_patient_is_actif.sql :
--   1. patients : suppression de is_actif, date_activation, date_desactivation
--   2. patients : réintroduction de suivi_en_cours (tinyint DEFAULT 1) et
--      recalcul depuis les dossiers (NULL si aucun dossier, 1 si ACTIF/PAUSE, sinon 0)
--   3. Suppression de la ligne de version 2.8.0 dans tec_meta_schema
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — patients : suppression des colonnes d'activation
-- ---------------------------------------------------------------------------

ALTER TABLE `patients`
  DROP COLUMN `date_desactivation`,
  DROP COLUMN `date_activation`,
  DROP COLUMN `is_actif`;

-- ---------------------------------------------------------------------------
-- SECTION 2 — patients : réintroduction de suivi_en_cours et recalcul
-- ---------------------------------------------------------------------------

ALTER TABLE `patients`
  ADD COLUMN `suivi_en_cours` tinyint(4) DEFAULT 1 AFTER `anamnese_txt`;

UPDATE `patients` p
SET `suivi_en_cours` = (
  SELECT CASE
           WHEN COUNT(d.`id_dossier`) = 0 THEN NULL
           WHEN SUM(CASE WHEN d.`code_statut` IN ('ACTIF', 'PAUSE') THEN 1 ELSE 0 END) > 0 THEN 1
           ELSE 0
         END
  FROM `dossiers` d
  WHERE d.`id_patient` = p.`id_patient`
);

-- ---------------------------------------------------------------------------
-- SECTION 3 — Suppression de la version 2.8.0
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.8.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
