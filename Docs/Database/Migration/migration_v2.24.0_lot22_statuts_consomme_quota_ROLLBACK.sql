-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.24.0 (Lot 22 — consomme_quota)
-- =============================================================================
-- ATTENTION : l'application doit être revenue à une version antérieure.
-- =============================================================================

DROP PROCEDURE IF EXISTS tmp_retirer_consomme_quota;

DELIMITER //
CREATE PROCEDURE tmp_retirer_consomme_quota()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_statuts_seance'
          AND COLUMN_NAME = 'consomme_quota') = 1 THEN
        ALTER TABLE `ref_statuts_seance` DROP COLUMN `consomme_quota`;
    END IF;
END //
DELIMITER ;

CALL tmp_retirer_consomme_quota();
DROP PROCEDURE IF EXISTS tmp_retirer_consomme_quota;

DELETE FROM tec_meta_schema WHERE version_schema = '2.24.0';
