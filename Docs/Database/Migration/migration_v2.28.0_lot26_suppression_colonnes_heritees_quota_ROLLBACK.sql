-- =============================================================================
-- ALTHÉA — ROLLBACK Migration v2.28.0 (Lot 26)
-- =============================================================================
-- Recrée les colonnes héritées avec leurs valeurs par défaut. Les anciennes
-- valeurs doivent être restaurées depuis la sauvegarde si nécessaire.
-- =============================================================================

DROP PROCEDURE IF EXISTS tmp_lot26_rollback;

DELIMITER //
CREATE PROCEDURE tmp_lot26_rollback()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_natures_seance'
          AND COLUMN_NAME = 'id_categorie_quota') = 0 THEN
        ALTER TABLE `ref_natures_seance`
            ADD COLUMN `id_categorie_quota` bigint(20) unsigned DEFAULT NULL
                COMMENT 'Categorie de quota consommee (NULL = hors quota)' AFTER `libelle_nature_seance`,
            ADD KEY `IDX_ref_natures_seance_categorie` (`id_categorie_quota`),
            ADD CONSTRAINT `fk_ref_natures_seance_categorie` FOREIGN KEY (`id_categorie_quota`)
                REFERENCES `ref_categories_quota` (`id_categorie_quota`) ON UPDATE CASCADE;
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_quotas'
          AND COLUMN_NAME = 'plafond_soutien') = 0 THEN
        ALTER TABLE `ref_quotas`
            ADD COLUMN `plafond_soutien` int(11) NOT NULL DEFAULT 10
                COMMENT 'Nombre de séances de soutien remboursées par période — D-INA-2' AFTER `id_domaine`,
            ADD COLUMN `plafond_traitement` int(11) NOT NULL DEFAULT 20
                COMMENT 'Nombre de séances de traitement remboursées par période — D-INA-2' AFTER `plafond_soutien`,
            ADD CONSTRAINT `chk_ref_quotas_plafonds` CHECK (`plafond_soutien` >= 0 AND `plafond_traitement` >= 0);
    END IF;
END //
DELIMITER ;

CALL tmp_lot26_rollback();
DROP PROCEDURE IF EXISTS tmp_lot26_rollback;

DELETE FROM tec_meta_schema WHERE version_schema = '2.28.0';
