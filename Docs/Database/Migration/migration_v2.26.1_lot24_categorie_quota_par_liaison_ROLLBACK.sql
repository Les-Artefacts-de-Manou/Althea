-- =============================================================================
-- ALTHÉA — ROLLBACK v2.26.1 (Lot 24 — Catégorie de quota portée par la liaison)
-- =============================================================================
-- Retire la colonne ajoutée sur lia_natures_seance_domaines et la clé candidate.
-- ref_natures_seance.id_categorie_quota (lot 23) n'est pas touchée.
-- =============================================================================

DROP PROCEDURE IF EXISTS tmp_lot24_rollback;

DELIMITER //
CREATE PROCEDURE tmp_lot24_rollback()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'lia_natures_seance_domaines'
          AND CONSTRAINT_NAME = 'fk_lia_nature_domaine_categorie') > 0 THEN
        ALTER TABLE `lia_natures_seance_domaines` DROP FOREIGN KEY `fk_lia_nature_domaine_categorie`;
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'lia_natures_seance_domaines'
          AND COLUMN_NAME = 'id_categorie_quota') > 0 THEN
        ALTER TABLE `lia_natures_seance_domaines`
            DROP KEY `IDX_lia_nature_domaine_categorie`,
            DROP COLUMN `id_categorie_quota`;
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_categories_quota'
          AND INDEX_NAME = 'uq_ref_categories_quota_id_domaine') > 0 THEN
        ALTER TABLE `ref_categories_quota` DROP KEY `uq_ref_categories_quota_id_domaine`;
    END IF;
END //
DELIMITER ;

CALL tmp_lot24_rollback();
DROP PROCEDURE IF EXISTS tmp_lot24_rollback;

DELETE FROM tec_meta_schema WHERE version_schema = '2.26.1';
