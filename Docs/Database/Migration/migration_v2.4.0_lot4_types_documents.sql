-- =============================================================================
-- ALTHÉA — Migration de schéma v2.4.0 (Lot 4 — Types de documents traçables)
-- =============================================================================
-- Décisions appliquées :
--   Traçabilité des documents : tout document créé OU uploadé dans l'application
--   doit avoir une trace dans la table `documents`, reliée à un type de document
--   du référentiel `ref_types_documents`.
--
-- Ajout de deux types de documents destinés aux exports générés par l'éditeur
-- enrichi (UC_RichTextEditor) :
--   - ANAMNESE      : export de l'anamnèse patient (niveau patient)
--   - ZONE_CLINIQUE : export d'une zone clinique de dossier (motif, historique,
--                     antécédents, diagnostic, traitements, objectifs…).
--                     Le nom de fichier identifie déjà la zone précise, un type
--                     générique suffit donc pour tous ces exports (choix Manou).
--
-- Ordre d'exécution :
--   1. ref_types_documents : insertion des 2 nouveaux types (ids auto 8 et 9)
--   2. Versionner dans tec_meta_schema
--
-- Remarque : les 7 types existants (BILAN, COMPTE_RENDU, COURRIER,
-- DOCUMENT_FOURNI, ATTESTATION, QUESTIONNAIRE, AUTRE — ids 1 à 7, ordres 10 à 70)
-- ne sont pas modifiés. Les nouveaux ordres d'affichage s'intercalent avant AUTRE.
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.4.0_lot4_types_documents_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — ref_types_documents : nouveaux types traçables
-- Insertion idempotente (INSERT ... ON DUPLICATE KEY) sur le code (clé unique).
-- ---------------------------------------------------------------------------

INSERT INTO `ref_types_documents`
  (`code_type_document`, `libelle_type_document`, `actif`, `ordre_affichage`)
VALUES
  ('ANAMNESE',      'Anamnèse',       1, 35),
  ('ZONE_CLINIQUE', 'Zone clinique',  1, 45)
ON DUPLICATE KEY UPDATE
  `libelle_type_document` = VALUES(`libelle_type_document`),
  `actif`                 = VALUES(`actif`),
  `ordre_affichage`       = VALUES(`ordre_affichage`);


-- ---------------------------------------------------------------------------
-- SECTION 2 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.4.0', 'Lot 4 référentiel : ajout des types de documents ANAMNESE et ZONE_CLINIQUE pour la traçabilité des exports RTF', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
