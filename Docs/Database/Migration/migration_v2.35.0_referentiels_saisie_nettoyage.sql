-- ---------------------------------------------------------------------------
-- Migration v2.35.0 - Référentiels de saisie - étape R4 (nettoyage)
--
-- Suppression des anciennes colonnes texte remplacées par les référentiels
-- (migration v2.34.0) et plus utilisées par l'application depuis les étapes
-- R2 et R3 (12/10/2026) :
--   - patients         : pays (→ id_pays), mutualite (→ id_organisme_mutualite)
--   - cab_cabinet      : pays (→ id_pays)
--   - cab_praticiens   : pays (→ id_pays), civilite (→ id_civilite)
--   - famille_contacts : pays (→ id_pays)
--   - therapeutes      : pays (→ id_pays)
--
-- ATTENTION — migration DESTRUCTIVE : le texte de ces colonnes est perdu.
-- Les valeurs reconnues ont été converties par la v2.34.0 ; les autres
-- (mutualités de test en texte libre) ne sont reprises nulle part.
-- Faire une sauvegarde de la base avant de l'appliquer. Le ROLLBACK recrée
-- les colonnes et les remplit depuis les référentiels, sans le texte non reconnu.
--
-- Règles : script rejouable (chaque suppression vérifie que la colonne existe).
-- Aucune clé ni index ne porte sur ces colonnes.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- patients.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE patients DROP COLUMN pays', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- patients.mutualite
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'mutualite');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE patients DROP COLUMN mutualite', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- cab_cabinet.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_cabinet' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE cab_cabinet DROP COLUMN pays', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- cab_praticiens.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE cab_praticiens DROP COLUMN pays', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- cab_praticiens.civilite
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'civilite');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE cab_praticiens DROP COLUMN civilite', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- famille_contacts.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'famille_contacts' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE famille_contacts DROP COLUMN pays', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- therapeutes.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'therapeutes' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE therapeutes DROP COLUMN pays', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- Journalisation
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.35.0',
       'Referentiels de saisie (R4) : suppression des colonnes texte pays (patients, cab_cabinet, cab_praticiens, famille_contacts, therapeutes), civilite (cab_praticiens) et mutualite (patients), remplacees en v2.34.0.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.35.0');
