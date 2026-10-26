-- ---------------------------------------------------------------------------
-- Migration v2.34.0 - Référentiels de saisie (Pays / Civilités / Mutualités) - étape R1
--
-- 1. Référentiel ref_pays (code ISO 3166-1 alpha-2). Les 5 pays pris en charge
--    par les règles de validation (UtilsTelephone, UtilsCodePostal) sont des
--    lignes système (cadenas) : les règles sont indexées par le code ISO.
-- 2. Référentiel ref_civilites (libellé + abréviation pour les courriers).
-- 3. ref_organismes.type_organisme : REMBOURSEMENT (ex. INAMI, rattaché aux
--    domaines) ou MUTUALITE (organisme assureur, rattaché aux patients).
--    Amorçage des 7 organismes assureurs belges (code = numéro INAMI de l'OA).
-- 4. Nouvelles colonnes de rattachement :
--    - id_pays : cab_cabinet, cab_praticiens, famille_contacts, patients, therapeutes ;
--    - id_civilite : cab_praticiens, patients, famille_contacts, therapeutes,
--      autres_suivis_patient ;
--    - patients : id_organisme_mutualite, numero_affiliation, code_titulaire_1/2.
-- 5. Conversion des valeurs texte existantes (pays, civilité, mutualité) vers
--    les nouvelles colonnes lorsque le libellé est reconnu.
--
-- Règles :
-- - Script rejouable (contrôles d'existence).
-- - Les anciennes colonnes texte (pays, civilite, mutualite) sont CONSERVÉES :
--   l'application les utilise encore jusqu'à l'étape R2/R3. Leur suppression
--   fera l'objet de la migration v2.35.0 (étape R4).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Référentiel ref_pays
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_pays` (
    `id_pays`           bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `code_pays`         char(2)      NOT NULL COMMENT 'Code ISO 3166-1 alpha-2 (cle des regles de validation)',
    `libelle_pays`      varchar(100) NOT NULL,
    `ordre_affichage`   int(11)      NOT NULL DEFAULT 0,
    `actif`             tinyint(1)   NOT NULL DEFAULT 1,
    `est_systeme`       tinyint(1)   NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
    `date_creation`     datetime     NOT NULL DEFAULT current_timestamp(),
    `date_modification` datetime     NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
    PRIMARY KEY (`id_pays`),
    UNIQUE KEY `uq_ref_pays_code` (`code_pays`),
    UNIQUE KEY `uq_ref_pays_libelle` (`libelle_pays`),
    CONSTRAINT `chk_ref_pays_code` CHECK (`code_pays` REGEXP '^[A-Z]{2}$')
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Pays (adresses) ; regles de validation indexees par code ISO dans le code';

INSERT INTO `ref_pays` (`code_pays`, `libelle_pays`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'BE', 'Belgique', 10, 1, 1 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_pays` WHERE `code_pays` = 'BE');

INSERT INTO `ref_pays` (`code_pays`, `libelle_pays`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'FR', 'France', 20, 1, 1 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_pays` WHERE `code_pays` = 'FR');

INSERT INTO `ref_pays` (`code_pays`, `libelle_pays`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'LU', 'Luxembourg', 30, 1, 1 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_pays` WHERE `code_pays` = 'LU');

INSERT INTO `ref_pays` (`code_pays`, `libelle_pays`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'DE', 'Allemagne', 40, 1, 1 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_pays` WHERE `code_pays` = 'DE');

INSERT INTO `ref_pays` (`code_pays`, `libelle_pays`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'NL', 'Pays-Bas', 50, 1, 1 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_pays` WHERE `code_pays` = 'NL');

-- ---------------------------------------------------------------------------
-- SECTION 2 — Référentiel ref_civilites
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_civilites` (
    `id_civilite`          bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `code_civilite`        varchar(20) NOT NULL,
    `libelle_civilite`     varchar(50) NOT NULL,
    `abreviation_civilite` varchar(10) NULL COMMENT 'Forme abregee pour les courriers (Mme, M., ...)',
    `ordre_affichage`      int(11)     NOT NULL DEFAULT 0,
    `actif`                tinyint(1)  NOT NULL DEFAULT 1,
    `est_systeme`          tinyint(1)  NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
    `date_creation`        datetime    NOT NULL DEFAULT current_timestamp(),
    `date_modification`    datetime    NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
    PRIMARY KEY (`id_civilite`),
    UNIQUE KEY `uq_ref_civilites_code` (`code_civilite`),
    UNIQUE KEY `uq_ref_civilites_libelle` (`libelle_civilite`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Civilites des personnes (praticiens, patients, contacts, therapeutes, intervenants)';

INSERT INTO `ref_civilites` (`code_civilite`, `libelle_civilite`, `abreviation_civilite`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'MADAME', 'Madame', 'Mme', 10, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_civilites` WHERE `code_civilite` = 'MADAME');

INSERT INTO `ref_civilites` (`code_civilite`, `libelle_civilite`, `abreviation_civilite`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'MONSIEUR', 'Monsieur', 'M.', 20, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_civilites` WHERE `code_civilite` = 'MONSIEUR');

INSERT INTO `ref_civilites` (`code_civilite`, `libelle_civilite`, `abreviation_civilite`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'MX', 'Mx', 'Mx', 30, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_civilites` WHERE `code_civilite` = 'MX');

-- ---------------------------------------------------------------------------
-- SECTION 3 — Type d'organisme et organismes assureurs
-- ---------------------------------------------------------------------------

SET @col_existe := (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_organismes' AND COLUMN_NAME = 'type_organisme');

SET @sql := IF(@col_existe = 0,
    'ALTER TABLE ref_organismes
        ADD COLUMN type_organisme varchar(20) NOT NULL DEFAULT ''REMBOURSEMENT''
            COMMENT ''REMBOURSEMENT (rattache aux domaines) ou MUTUALITE (rattache aux patients)'' AFTER libelle_organisme,
        ADD KEY IDX_ref_organismes_type (type_organisme),
        ADD CONSTRAINT chk_ref_organismes_type CHECK (type_organisme IN (''REMBOURSEMENT'', ''MUTUALITE''))',
    'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

ALTER TABLE `ref_organismes`
    COMMENT = 'Organismes : remboursement (ex. INAMI, rattaches aux domaines) et mutualites (rattachees aux patients)';

-- Organismes assureurs belges : code = numéro INAMI de l'organisme assureur.
-- Lignes non système (le code ne s'y réfère pas) : coordonnées à compléter par l'utilisatrice.
INSERT INTO `ref_organismes` (`code_organisme`, `libelle_organisme`, `type_organisme`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT '100', 'Mutualités chrétiennes', 'MUTUALITE', 0, 1, 100 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_organismes` WHERE `code_organisme` = '100' OR `libelle_organisme` = 'Mutualités chrétiennes');

INSERT INTO `ref_organismes` (`code_organisme`, `libelle_organisme`, `type_organisme`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT '200', 'Mutualités neutres', 'MUTUALITE', 0, 1, 200 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_organismes` WHERE `code_organisme` = '200' OR `libelle_organisme` = 'Mutualités neutres');

INSERT INTO `ref_organismes` (`code_organisme`, `libelle_organisme`, `type_organisme`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT '300', 'Solidaris', 'MUTUALITE', 0, 1, 300 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_organismes` WHERE `code_organisme` = '300' OR `libelle_organisme` = 'Solidaris');

INSERT INTO `ref_organismes` (`code_organisme`, `libelle_organisme`, `type_organisme`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT '400', 'Mutualités libérales', 'MUTUALITE', 0, 1, 400 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_organismes` WHERE `code_organisme` = '400' OR `libelle_organisme` = 'Mutualités libérales');

INSERT INTO `ref_organismes` (`code_organisme`, `libelle_organisme`, `type_organisme`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT '500', 'Mutualités libres', 'MUTUALITE', 0, 1, 500 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_organismes` WHERE `code_organisme` = '500' OR `libelle_organisme` = 'Mutualités libres');

INSERT INTO `ref_organismes` (`code_organisme`, `libelle_organisme`, `type_organisme`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT '600', 'CAAMI', 'MUTUALITE', 0, 1, 600 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_organismes` WHERE `code_organisme` = '600' OR `libelle_organisme` = 'CAAMI');

INSERT INTO `ref_organismes` (`code_organisme`, `libelle_organisme`, `type_organisme`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT '900', 'HR Rail', 'MUTUALITE', 0, 1, 900 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_organismes` WHERE `code_organisme` = '900' OR `libelle_organisme` = 'HR Rail');

-- ---------------------------------------------------------------------------
-- SECTION 4 — Colonnes id_pays (5 tables)
-- ---------------------------------------------------------------------------

-- cab_cabinet
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_cabinet' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE cab_cabinet ADD COLUMN id_pays bigint(20) unsigned NULL AFTER pays,
        ADD KEY IDX_cab_cabinet_pays (id_pays),
        ADD CONSTRAINT fk_cab_cabinet_pays FOREIGN KEY (id_pays) REFERENCES ref_pays (id_pays) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- cab_praticiens
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE cab_praticiens ADD COLUMN id_pays bigint(20) unsigned NULL AFTER pays,
        ADD KEY IDX_cab_praticiens_pays (id_pays),
        ADD CONSTRAINT fk_cab_praticiens_pays FOREIGN KEY (id_pays) REFERENCES ref_pays (id_pays) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- famille_contacts
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'famille_contacts' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE famille_contacts ADD COLUMN id_pays bigint(20) unsigned NULL AFTER pays,
        ADD KEY IDX_famille_contacts_pays (id_pays),
        ADD CONSTRAINT fk_famille_contacts_pays FOREIGN KEY (id_pays) REFERENCES ref_pays (id_pays) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- patients
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE patients ADD COLUMN id_pays bigint(20) unsigned NULL AFTER pays,
        ADD KEY IDX_patients_pays (id_pays),
        ADD CONSTRAINT fk_patients_pays FOREIGN KEY (id_pays) REFERENCES ref_pays (id_pays) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- therapeutes
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'therapeutes' AND COLUMN_NAME = 'id_pays');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE therapeutes ADD COLUMN id_pays bigint(20) unsigned NULL AFTER pays,
        ADD KEY IDX_therapeutes_pays (id_pays),
        ADD CONSTRAINT fk_therapeutes_pays FOREIGN KEY (id_pays) REFERENCES ref_pays (id_pays) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- SECTION 5 — Colonnes id_civilite (5 tables)
-- ---------------------------------------------------------------------------

-- cab_praticiens
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE cab_praticiens ADD COLUMN id_civilite bigint(20) unsigned NULL AFTER civilite,
        ADD KEY IDX_cab_praticiens_civilite (id_civilite),
        ADD CONSTRAINT fk_cab_praticiens_civilite FOREIGN KEY (id_civilite) REFERENCES ref_civilites (id_civilite) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- patients
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE patients ADD COLUMN id_civilite bigint(20) unsigned NULL AFTER code_patient,
        ADD KEY IDX_patients_civilite (id_civilite),
        ADD CONSTRAINT fk_patients_civilite FOREIGN KEY (id_civilite) REFERENCES ref_civilites (id_civilite) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- famille_contacts
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'famille_contacts' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE famille_contacts ADD COLUMN id_civilite bigint(20) unsigned NULL AFTER id_lien_patient,
        ADD KEY IDX_famille_contacts_civilite (id_civilite),
        ADD CONSTRAINT fk_famille_contacts_civilite FOREIGN KEY (id_civilite) REFERENCES ref_civilites (id_civilite) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- therapeutes
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'therapeutes' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE therapeutes ADD COLUMN id_civilite bigint(20) unsigned NULL AFTER code_therapeute,
        ADD KEY IDX_therapeutes_civilite (id_civilite),
        ADD CONSTRAINT fk_therapeutes_civilite FOREIGN KEY (id_civilite) REFERENCES ref_civilites (id_civilite) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- autres_suivis_patient (intervenant saisi librement, non rattaché à un thérapeute connu)
SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'autres_suivis_patient' AND COLUMN_NAME = 'id_civilite');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE autres_suivis_patient ADD COLUMN id_civilite bigint(20) unsigned NULL AFTER id_therapeute,
        ADD KEY IDX_autres_suivis_civilite (id_civilite),
        ADD CONSTRAINT fk_autres_suivis_civilite FOREIGN KEY (id_civilite) REFERENCES ref_civilites (id_civilite) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- SECTION 6 — Mutualité du patient
-- ---------------------------------------------------------------------------

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'patients' AND COLUMN_NAME = 'id_organisme_mutualite');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE patients
        ADD COLUMN id_organisme_mutualite bigint(20) unsigned NULL COMMENT ''Mutualite (ref_organismes de type MUTUALITE)'' AFTER mutualite,
        ADD COLUMN numero_affiliation varchar(30) NULL COMMENT ''Numero d affiliation aupres de la mutualite'' AFTER id_organisme_mutualite,
        ADD COLUMN code_titulaire_1 char(3) NULL COMMENT ''CT1 (3 chiffres)'' AFTER numero_affiliation,
        ADD COLUMN code_titulaire_2 char(3) NULL COMMENT ''CT2 (3 chiffres)'' AFTER code_titulaire_1,
        ADD KEY IDX_patients_mutualite (id_organisme_mutualite),
        ADD CONSTRAINT fk_patients_mutualite FOREIGN KEY (id_organisme_mutualite) REFERENCES ref_organismes (id_organisme) ON UPDATE CASCADE ON DELETE RESTRICT,
        ADD CONSTRAINT chk_patients_ct1 CHECK (code_titulaire_1 IS NULL OR code_titulaire_1 REGEXP ''^[0-9]{3}$''),
        ADD CONSTRAINT chk_patients_ct2 CHECK (code_titulaire_2 IS NULL OR code_titulaire_2 REGEXP ''^[0-9]{3}$'')',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- SECTION 7 — Conversion des valeurs texte existantes
-- Comparaison sur le libellé ou le code ISO (collation insensible à la casse et
-- aux accents). Les valeurs non reconnues restent NULL ; le texte d'origine est
-- conservé dans l'ancienne colonne jusqu'à la migration v2.35.0.
-- ---------------------------------------------------------------------------

UPDATE cab_cabinet t JOIN ref_pays p
    ON TRIM(t.pays) COLLATE utf8mb4_uca1400_ai_ci IN (p.libelle_pays, p.code_pays)
SET t.id_pays = p.id_pays WHERE t.id_pays IS NULL;

UPDATE cab_praticiens t JOIN ref_pays p
    ON TRIM(t.pays) COLLATE utf8mb4_uca1400_ai_ci IN (p.libelle_pays, p.code_pays)
SET t.id_pays = p.id_pays WHERE t.id_pays IS NULL;

UPDATE famille_contacts t JOIN ref_pays p
    ON TRIM(t.pays) COLLATE utf8mb4_uca1400_ai_ci IN (p.libelle_pays, p.code_pays)
SET t.id_pays = p.id_pays WHERE t.id_pays IS NULL;

UPDATE patients t JOIN ref_pays p
    ON TRIM(t.pays) COLLATE utf8mb4_uca1400_ai_ci IN (p.libelle_pays, p.code_pays)
SET t.id_pays = p.id_pays WHERE t.id_pays IS NULL;

UPDATE therapeutes t JOIN ref_pays p
    ON TRIM(t.pays) COLLATE utf8mb4_uca1400_ai_ci IN (p.libelle_pays, p.code_pays)
SET t.id_pays = p.id_pays WHERE t.id_pays IS NULL;

UPDATE cab_praticiens t JOIN ref_civilites c
    ON TRIM(t.civilite) COLLATE utf8mb4_uca1400_ai_ci IN (c.libelle_civilite, c.abreviation_civilite)
SET t.id_civilite = c.id_civilite WHERE t.id_civilite IS NULL;

UPDATE patients t JOIN ref_organismes o
    ON TRIM(t.mutualite) COLLATE utf8mb4_uca1400_ai_ci = o.libelle_organisme COLLATE utf8mb4_uca1400_ai_ci
   AND o.type_organisme = 'MUTUALITE'
SET t.id_organisme_mutualite = o.id_organisme WHERE t.id_organisme_mutualite IS NULL;

-- ---------------------------------------------------------------------------
-- SECTION 8 — Journalisation
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.34.0',
       'Referentiels de saisie (R1) : ref_pays (ISO, lignes systeme), ref_civilites, ref_organismes.type_organisme + 7 organismes assureurs ; id_pays, id_civilite, mutualite patient (id, affiliation, CT1/CT2) ; conversion des textes existants.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.34.0');
