-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.35.0 - Référentiels de saisie - étape R4
-- Recrée les colonnes texte supprimées, à leur place et avec leur définition
-- d'origine, puis les remplit à partir des référentiels (libellé du pays, de la
-- civilité, de la mutualité choisie). Le texte d'origine qui n'avait pas été
-- reconnu (mutualités saisies librement) est perdu : seule la sauvegarde faite
-- avant la migration permet de le retrouver.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- patients.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 0, 'ALTER TABLE patients ADD COLUMN pays varchar(100) DEFAULT NULL AFTER localite', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- patients.mutualite
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'mutualite');
SET @sql := IF(@col_existe = 0, 'ALTER TABLE patients ADD COLUMN mutualite varchar(100) DEFAULT NULL AFTER email', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- cab_cabinet.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_cabinet' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 0, 'ALTER TABLE cab_cabinet ADD COLUMN pays varchar(100) DEFAULT NULL AFTER localite', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- cab_praticiens.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 0, 'ALTER TABLE cab_praticiens ADD COLUMN pays varchar(100) DEFAULT NULL AFTER localite', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- cab_praticiens.civilite
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'civilite');
SET @sql := IF(@col_existe = 0, 'ALTER TABLE cab_praticiens ADD COLUMN civilite varchar(20) DEFAULT NULL AFTER id_utilisateur', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- famille_contacts.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'famille_contacts' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 0, 'ALTER TABLE famille_contacts ADD COLUMN pays varchar(100) DEFAULT NULL AFTER localite', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- therapeutes.pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'therapeutes' AND COLUMN_NAME = 'pays');
SET @sql := IF(@col_existe = 0, 'ALTER TABLE therapeutes ADD COLUMN pays varchar(100) DEFAULT NULL AFTER localite', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Recopie depuis les référentiels (colonnes de rattachement de la v2.34.0)
UPDATE patients t JOIN ref_pays p ON p.id_pays = t.id_pays SET t.pays = p.libelle_pays WHERE t.pays IS NULL;
UPDATE cab_cabinet t JOIN ref_pays p ON p.id_pays = t.id_pays SET t.pays = p.libelle_pays WHERE t.pays IS NULL;
UPDATE cab_praticiens t JOIN ref_pays p ON p.id_pays = t.id_pays SET t.pays = p.libelle_pays WHERE t.pays IS NULL;
UPDATE famille_contacts t JOIN ref_pays p ON p.id_pays = t.id_pays SET t.pays = p.libelle_pays WHERE t.pays IS NULL;
UPDATE therapeutes t JOIN ref_pays p ON p.id_pays = t.id_pays SET t.pays = p.libelle_pays WHERE t.pays IS NULL;
UPDATE cab_praticiens t JOIN ref_civilites c ON c.id_civilite = t.id_civilite SET t.civilite = c.libelle_civilite WHERE t.civilite IS NULL;
UPDATE patients t JOIN ref_organismes o ON o.id_organisme = t.id_organisme_mutualite SET t.mutualite = o.libelle_organisme WHERE t.mutualite IS NULL;

DELETE FROM tec_meta_schema WHERE version_schema = '2.35.0';
