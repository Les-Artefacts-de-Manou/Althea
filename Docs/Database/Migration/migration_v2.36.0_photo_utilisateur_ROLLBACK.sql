-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.36.0 - photo de l'utilisateur
-- Retire sec_utilisateurs.photo_fichier. Les fichiers photo du dossier de données
-- (Utilisateurs/Utilisateur_{id}/) ne sont pas supprimés.
-- ---------------------------------------------------------------------------

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'sec_utilisateurs' AND COLUMN_NAME = 'photo_fichier');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE sec_utilisateurs DROP COLUMN photo_fichier', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

DELETE FROM tec_meta_schema WHERE version_schema = '2.36.0';
