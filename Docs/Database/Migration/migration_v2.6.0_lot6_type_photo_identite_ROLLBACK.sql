-- =============================================================================
-- ALTHÉA — ROLLBACK Migration v2.6.0 (Lot 6 — Type de document PHOTO_IDENTITE)
-- =============================================================================
-- Annule la migration v2.6.0 :
--   1. Suppression du type PHOTO_IDENTITE de ref_types_documents
--   2. Suppression de la ligne de version 2.6.0 dans tec_meta_schema
--
-- ATTENTION : à n'exécuter que sur une base de développement. Échouera si des
-- documents référencent encore ce type (FK fk_documents_ref_types_documents).
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

DELETE FROM `ref_types_documents` WHERE `code_type_document` = 'PHOTO_IDENTITE';

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.6.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
