-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.34.0 - Référentiels de saisie (étape R1)
-- Retire les colonnes de rattachement (pays, civilité, mutualité patient), le type
-- d'organisme, les 7 organismes assureurs amorcés et les référentiels ref_pays /
-- ref_civilites. Les anciennes colonnes texte n'ayant pas été touchées par la
-- migration, aucune donnée d'origine n'est perdue.
--
-- Attention : les organismes assureurs ne sont supprimés que s'ils ne sont
-- rattachés à aucun patient ni domaine (sinon la suppression échoue sur la FK,
-- d'où le retrait préalable des colonnes patients).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- Mutualité du patient
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'id_organisme_mutualite');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE patients
        DROP CONSTRAINT chk_patients_ct2,
        DROP CONSTRAINT chk_patients_ct1,
        DROP FOREIGN KEY fk_patients_mutualite,
        DROP KEY IDX_patients_mutualite,
        DROP COLUMN code_titulaire_2,
        DROP COLUMN code_titulaire_1,
        DROP COLUMN numero_affiliation,
        DROP COLUMN id_organisme_mutualite',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Colonnes id_civilite
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'autres_suivis_patient' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE autres_suivis_patient DROP FOREIGN KEY fk_autres_suivis_civilite, DROP KEY IDX_autres_suivis_civilite, DROP COLUMN id_civilite',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'therapeutes' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE therapeutes DROP FOREIGN KEY fk_therapeutes_civilite, DROP KEY IDX_therapeutes_civilite, DROP COLUMN id_civilite',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'famille_contacts' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE famille_contacts DROP FOREIGN KEY fk_famille_contacts_civilite, DROP KEY IDX_famille_contacts_civilite, DROP COLUMN id_civilite',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE patients DROP FOREIGN KEY fk_patients_civilite, DROP KEY IDX_patients_civilite, DROP COLUMN id_civilite',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE cab_praticiens DROP FOREIGN KEY fk_cab_praticiens_civilite, DROP KEY IDX_cab_praticiens_civilite, DROP COLUMN id_civilite',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Colonnes id_pays
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'therapeutes' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE therapeutes DROP FOREIGN KEY fk_therapeutes_pays, DROP KEY IDX_therapeutes_pays, DROP COLUMN id_pays',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE patients DROP FOREIGN KEY fk_patients_pays, DROP KEY IDX_patients_pays, DROP COLUMN id_pays',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'famille_contacts' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE famille_contacts DROP FOREIGN KEY fk_famille_contacts_pays, DROP KEY IDX_famille_contacts_pays, DROP COLUMN id_pays',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE cab_praticiens DROP FOREIGN KEY fk_cab_praticiens_pays, DROP KEY IDX_cab_praticiens_pays, DROP COLUMN id_pays',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_cabinet' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE cab_cabinet DROP FOREIGN KEY fk_cab_cabinet_pays, DROP KEY IDX_cab_cabinet_pays, DROP COLUMN id_pays',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Organismes assureurs amorcés (seulement s'ils ne sont rattachés à aucun domaine)
DELETE o FROM ref_organismes o
WHERE o.code_organisme IN ('100', '200', '300', '400', '500', '600', '900')
  AND o.type_organisme = 'MUTUALITE'
  AND NOT EXISTS (SELECT 1 FROM ref_domaines d WHERE d.id_organisme = o.id_organisme);

-- Type d'organisme
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_organismes' AND COLUMN_NAME = 'type_organisme');
SET @sql := IF(@col_existe = 1,
    'ALTER TABLE ref_organismes DROP CONSTRAINT chk_ref_organismes_type, DROP KEY IDX_ref_organismes_type, DROP COLUMN type_organisme',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

ALTER TABLE `ref_organismes`
    COMMENT = 'Organismes de remboursement (ex. INAMI), rattaches facultativement aux domaines';

DROP TABLE IF EXISTS `ref_civilites`;
DROP TABLE IF EXISTS `ref_pays`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.34.0';
