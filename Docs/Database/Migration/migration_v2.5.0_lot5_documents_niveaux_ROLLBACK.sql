-- =============================================================================
-- ALTHÉA — ROLLBACK Migration v2.5.0 (Lot 5 — Documents multi-niveaux)
-- =============================================================================
-- Annule la migration v2.5.0 :
--   1. Suppression de la FK fk_documents_patients puis de la colonne id_patient
--   2. Rétablissement de id_dossier en NOT NULL
--   3. Suppression de la ligne de version 2.5.0 dans tec_meta_schema
--
-- ATTENTION : le rétablissement de id_dossier en NOT NULL échouera s'il existe
-- des documents de niveau patient (id_dossier NULL). À n'exécuter que sur une
-- base de développement sans données métier réelles.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — documents : suppression FK + colonne id_patient
-- ---------------------------------------------------------------------------

ALTER TABLE `documents` DROP FOREIGN KEY `fk_documents_patients`;
ALTER TABLE `documents` DROP COLUMN `id_patient`;

-- ---------------------------------------------------------------------------
-- SECTION 2 — documents : id_dossier de nouveau NOT NULL
-- ---------------------------------------------------------------------------

ALTER TABLE `documents`
  MODIFY COLUMN `id_dossier` bigint(20) unsigned NOT NULL;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Retrait de la version dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.5.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
