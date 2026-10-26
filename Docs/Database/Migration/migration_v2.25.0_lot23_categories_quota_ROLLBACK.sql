-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.25.0 (Lot 23 — catégories de quota)
-- =============================================================================
-- Les anciennes colonnes n'ayant pas été touchées, aucune donnée n'est perdue.
-- ATTENTION : l'application doit être revenue à une version antérieure.
-- =============================================================================

DROP PROCEDURE IF EXISTS tmp_rollback_lot23;

DELIMITER //
CREATE PROCEDURE tmp_rollback_lot23()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_natures_seance'
          AND COLUMN_NAME = 'id_categorie_quota') = 1 THEN
        ALTER TABLE `ref_natures_seance` DROP FOREIGN KEY `fk_ref_natures_seance_categorie`;
        ALTER TABLE `ref_natures_seance`
            DROP KEY `IDX_ref_natures_seance_categorie`,
            DROP COLUMN `id_categorie_quota`;
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_quotas'
          AND COLUMN_NAME = 'duree_periode_mois') = 1 THEN
        ALTER TABLE `ref_quotas` DROP CONSTRAINT `chk_ref_quotas_duree`;
        ALTER TABLE `ref_quotas` DROP COLUMN `duree_periode_mois`;
    END IF;
END //
DELIMITER ;

CALL tmp_rollback_lot23();
DROP PROCEDURE IF EXISTS tmp_rollback_lot23;

DROP TABLE IF EXISTS `ref_quotas_plafonds`;
DROP TABLE IF EXISTS `ref_categories_quota`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.25.0';
