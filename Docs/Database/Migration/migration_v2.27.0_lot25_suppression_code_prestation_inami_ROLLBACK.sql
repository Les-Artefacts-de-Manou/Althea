-- =============================================================================
-- ALTHÉA — ROLLBACK Migration v2.27.0 (Lot 25)
-- =============================================================================
-- Recrée la colonne ref_natures_seance.code_prestation_inami (VIDE) et son index.
-- Les anciennes valeurs doivent être restaurées depuis la sauvegarde si nécessaire.
-- =============================================================================

DROP PROCEDURE IF EXISTS tmp_lot25_rollback;

DELIMITER //
CREATE PROCEDURE tmp_lot25_rollback()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_natures_seance'
          AND COLUMN_NAME = 'code_prestation_inami') = 0 THEN
        ALTER TABLE `ref_natures_seance`
            ADD COLUMN `code_prestation_inami` varchar(20) DEFAULT NULL
                COMMENT 'Discriminant de quota INAMI : SOUTIEN, TRAITEMENT ou NULL (hors quota) — D-INA-1'
                AFTER `libelle_nature_seance`,
            ADD INDEX `IDX_ref_natures_seance_prestation_inami` (`code_prestation_inami`);
    END IF;
END //
DELIMITER ;

CALL tmp_lot25_rollback();
DROP PROCEDURE IF EXISTS tmp_lot25_rollback;

DELETE FROM tec_meta_schema WHERE version_schema = '2.27.0';
