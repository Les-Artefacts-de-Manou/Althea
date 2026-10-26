-- =============================================================================
-- ALTHÉA — Migration v2.27.0 (Lot 25 — Suppression de ref_natures_seance.code_prestation_inami)
-- =============================================================================
--
-- Objet :
--   Suppression de la colonne héritée ref_natures_seance.code_prestation_inami
--   (et de son index), remplacée par lia_natures_seance_domaines.id_categorie_quota
--   (lot 24). Le code applicatif ne lit ni n'écrit plus cette colonne.
--
-- Règles :
--   - Prérequis : migration v2.26.1 appliquée.
--   - Script rejouable. Compatible : MariaDB 12.x, InnoDB, utf8mb4.
--   - Les valeurs supprimées restent consultables dans la sauvegarde préalable ;
--     le rollback recrée la colonne VIDE.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Contrôle préalable (à lire avant exécution)
-- ---------------------------------------------------------------------------
-- Natures portant encore un code de prestation (information uniquement) :
SELECT id_nature_seance, code_nature_seance, code_prestation_inami
FROM ref_natures_seance
WHERE code_prestation_inami IS NOT NULL;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Structure
-- ---------------------------------------------------------------------------

DROP PROCEDURE IF EXISTS tmp_lot25_suppression;

DELIMITER //
CREATE PROCEDURE tmp_lot25_suppression()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_natures_seance'
          AND INDEX_NAME = 'IDX_ref_natures_seance_prestation_inami') > 0 THEN
        ALTER TABLE `ref_natures_seance` DROP INDEX `IDX_ref_natures_seance_prestation_inami`;
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_natures_seance'
          AND COLUMN_NAME = 'code_prestation_inami') > 0 THEN
        ALTER TABLE `ref_natures_seance` DROP COLUMN `code_prestation_inami`;
    END IF;
END //
DELIMITER ;

CALL tmp_lot25_suppression();
DROP PROCEDURE IF EXISTS tmp_lot25_suppression;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.27.0',
       'Lot 25 : suppression de ref_natures_seance.code_prestation_inami et de son index (remplace par la categorie de quota portee par la liaison nature/domaine, lot 24).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.27.0');
