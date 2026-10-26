-- =============================================================================
-- ALTHÉA — Migration de schéma v2.6.0 (Lot 6 — Type de document PHOTO_IDENTITE)
-- =============================================================================
-- Ajout d'un type de document destiné à la traçabilité de la photo d'identité
-- du patient (import depuis la fiche patient, niveau patient) :
--   - PHOTO_IDENTITE : image d'identité unique par patient (est_document_image_metier = 1
--                      côté application). Nom de fichier déterministe (identite.ext),
--                      upsert par (patient + type) : une seule ligne quelle que soit
--                      l'extension (jpg, png…). Cf. ADR-23.
--
-- Ordre d'exécution :
--   1. ref_types_documents : insertion du nouveau type (id auto)
--   2. Versionner dans tec_meta_schema
--
-- Remarque : les types existants ne sont pas modifiés. L'ordre d'affichage 30 place
-- PHOTO_IDENTITE avant ANAMNESE (35) et ZONE_CLINIQUE (45).
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.6.0_lot6_type_photo_identite_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — ref_types_documents : nouveau type PHOTO_IDENTITE
-- Insertion idempotente (INSERT ... ON DUPLICATE KEY) sur le code (clé unique).
-- ---------------------------------------------------------------------------

INSERT INTO `ref_types_documents`
  (`code_type_document`, `libelle_type_document`, `actif`, `ordre_affichage`)
VALUES
  ('PHOTO_IDENTITE', 'Photo d''identité', 1, 30)
ON DUPLICATE KEY UPDATE
  `libelle_type_document` = VALUES(`libelle_type_document`),
  `actif`                 = VALUES(`actif`),
  `ordre_affichage`       = VALUES(`ordre_affichage`);


-- ---------------------------------------------------------------------------
-- SECTION 2 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.6.0', 'Lot 6 référentiel : ajout du type de document PHOTO_IDENTITE pour la traçabilité de la photo d''identité patient', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
