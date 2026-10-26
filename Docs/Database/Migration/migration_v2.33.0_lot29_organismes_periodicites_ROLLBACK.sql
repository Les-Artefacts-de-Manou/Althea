-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.33.0 - Lot 29
-- Retire les coordonnées / la périodicité des organismes et le référentiel
-- ref_periodicites. Les organismes eux-mêmes ne sont pas touchés.
-- ---------------------------------------------------------------------------

SET @col_existe := (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_organismes' AND COLUMN_NAME = 'id_periodicite');

SET @sql := IF(@col_existe = 1,
    'ALTER TABLE ref_organismes
        DROP FOREIGN KEY fk_ref_organismes_periodicite,
        DROP KEY IDX_ref_organismes_periodicite,
        DROP COLUMN id_periodicite,
        DROP COLUMN procedure_encodage,
        DROP COLUMN url_plateforme,
        DROP COLUMN site_web,
        DROP COLUMN email_2,
        DROP COLUMN email,
        DROP COLUMN telephone_2,
        DROP COLUMN telephone,
        DROP COLUMN adresse',
    'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

DROP TABLE IF EXISTS `ref_periodicites`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.33.0';
