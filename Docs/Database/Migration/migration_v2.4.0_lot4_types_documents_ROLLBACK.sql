-- =============================================================================
-- ALTHÉA — Rollback migration v2.4.0 (Lot 4 — Types de documents traçables)
-- =============================================================================
-- Annule intégralement migration_v2.4.0_lot4_types_documents.sql dans l'ordre
-- inverse. À exécuter uniquement sur la base de développement.
--
-- Ordre d'exécution (inverse de la migration) :
--   2. Supprimer la ligne de version dans tec_meta_schema
--   1. Retirer les types ANAMNESE et ZONE_CLINIQUE de ref_types_documents
--
-- Remarque : la suppression échoue (contrainte FK) si des lignes de la table
-- `documents` référencent déjà ces types. Sur base de développement sans
-- documents tracés, la suppression est sûre.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;


-- ---------------------------------------------------------------------------
-- SECTION 2 — Supprimer la ligne de version
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.4.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 1 — ref_types_documents : retirer les types traçables ajoutés
-- ---------------------------------------------------------------------------

DELETE FROM `ref_types_documents`
WHERE `code_type_document` IN ('ANAMNESE', 'ZONE_CLINIQUE');


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN migration_v2.4.0_lot4_types_documents_ROLLBACK.sql
-- =============================================================================
