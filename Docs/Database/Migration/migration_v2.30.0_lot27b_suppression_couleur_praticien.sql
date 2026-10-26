-- ---------------------------------------------------------------------------
-- Migration v2.30.0 - Lot 27b
-- Suppression de cab_praticiens.couleur_agenda : la couleur de l'agenda dépendra
-- du domaine et du type de rendez-vous, pas du praticien.
-- ---------------------------------------------------------------------------

SET @col_existe := (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'couleur_agenda');

SET @sql := IF(@col_existe = 1, 'ALTER TABLE cab_praticiens DROP COLUMN couleur_agenda', 'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.30.0',
       'Lot 27b : suppression de cab_praticiens.couleur_agenda (couleur agenda portee par domaine et type de RV).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.30.0');
