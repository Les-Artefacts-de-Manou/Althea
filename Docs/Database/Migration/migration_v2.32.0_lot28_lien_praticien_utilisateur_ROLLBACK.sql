-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.32.0 - Lot 28
-- Retire le lien praticien <-> compte utilisateur. Les comptes ne sont pas touchés.
-- ---------------------------------------------------------------------------

SET @col_existe := (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'id_utilisateur');

SET @sql := IF(@col_existe = 1,
    'ALTER TABLE cab_praticiens DROP FOREIGN KEY fk_cab_praticiens_utilisateur, DROP KEY uq_cab_praticiens_utilisateur, DROP COLUMN id_utilisateur',
    'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

DELETE FROM tec_meta_schema WHERE version_schema = '2.32.0';
