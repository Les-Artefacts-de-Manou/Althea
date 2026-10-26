-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.13.0 (Lot 12 — Quota INAMI Realism)
-- =============================================================================
-- Annule migration_v2.13.0_lot12_quota_inami.sql :
--   1. Réaffecte les séances utilisant les natures INAMI vers 'TRAITEMENT'
--      (nature neutre préexistante) afin de ne pas violer la clé étrangère
--   2. Supprime les 2 natures INAMI
--   3. Supprime l'index puis la colonne code_prestation_inami
--   4. Retire la ligne de version dans tec_meta_schema
--
-- ATTENTION : cette opération perd le discriminant soutien/traitement INAMI
-- des séances déjà encodées. À n'exécuter qu'en développement.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Réaffectation des séances vers la nature neutre 'TRAITEMENT'
-- ---------------------------------------------------------------------------

UPDATE `seances`
SET `id_nature_seance` = (
      SELECT `id_nature_seance`
      FROM `ref_natures_seance`
      WHERE `code_nature_seance` = 'TRAITEMENT'
      LIMIT 1
    )
WHERE `id_nature_seance` IN (
  SELECT `id_nature_seance`
  FROM `ref_natures_seance`
  WHERE `code_nature_seance` IN ('SOUTIEN_INAMI', 'TRAITEMENT_INAMI')
);

-- ---------------------------------------------------------------------------
-- SECTION 2 — Suppression des natures INAMI
-- ---------------------------------------------------------------------------

DELETE FROM `ref_natures_seance`
WHERE `code_nature_seance` IN ('SOUTIEN_INAMI', 'TRAITEMENT_INAMI');

-- ---------------------------------------------------------------------------
-- SECTION 3 — Suppression de l'index et de la colonne
-- ---------------------------------------------------------------------------

ALTER TABLE `ref_natures_seance`
  DROP INDEX `IDX_ref_natures_seance_prestation_inami`,
  DROP COLUMN `code_prestation_inami`;

-- ---------------------------------------------------------------------------
-- SECTION 4 — Retrait de la version dans tec_meta_schema
-- ---------------------------------------------------------------------------

DELETE FROM `tec_meta_schema`
WHERE `version_schema` = '2.13.0';

COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40101 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
