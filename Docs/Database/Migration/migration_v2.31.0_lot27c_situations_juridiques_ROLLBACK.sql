-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.31.0 - Lot 27c
-- ---------------------------------------------------------------------------

SET @col_existe := (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_cabinet' AND COLUMN_NAME = 'id_situation_juridique');

SET @sql := IF(@col_existe = 1,
    'ALTER TABLE cab_cabinet DROP FOREIGN KEY fk_cab_cabinet_situation, DROP KEY IDX_cab_cabinet_situation, DROP COLUMN id_situation_juridique',
    'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

DROP TABLE IF EXISTS `ref_situations_juridiques`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.31.0';
