-- ---------------------------------------------------------------------------
-- Migration v2.40.0 - UI-7 : groupes des paramètres
--
-- tec_groupes_parametres : titre et ordre d'affichage de chaque groupe de
-- paramètres (tec_parametres.groupe_parametre). L'écran Paramètres présente une
-- carte par groupe (refonte du 16/10/2026) ; jusqu'ici il affichait le code brut
-- du groupe (PATHS, SEANCES…) dans des onglets triés par ordre alphabétique.
--
-- 1. Création de la table et des 6 groupes existants.
-- 2. Sécurité : tout groupe encore utilisé par tec_parametres et absent de la
--    liste est ajouté avec son code pour titre (la clé étrangère ne peut pas
--    échouer) — à renommer ensuite par une migration.
-- 3. Clé étrangère tec_parametres.groupe_parametre → code_groupe : un paramètre
--    ne peut plus désigner un groupe inexistant.
-- 4. groupe_parametre perd sa valeur par défaut 'GENERAL' : un paramètre naît
--    par migration et nomme toujours son groupe (Rules § 14).
--
-- Règles : script rejouable ; aucune valeur de paramètre modifiée.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- 1. Table des groupes
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `tec_groupes_parametres` (
    `id_groupe_parametre` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `code_groupe`         varchar(50)  NOT NULL COMMENT 'Code technique (MAJUSCULE, sans accents), repris par tec_parametres.groupe_parametre',
    `libelle_groupe`      varchar(100) NOT NULL COMMENT 'Titre de la carte dans l ecran Parametres',
    `ordre_affichage`     int(11)      NOT NULL DEFAULT 0,
    `date_creation`       datetime     NOT NULL DEFAULT current_timestamp(),
    `date_modification`   datetime     NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
    PRIMARY KEY (`id_groupe_parametre`),
    UNIQUE KEY `uq_tec_groupes_parametres_code` (`code_groupe`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Groupes des parametres applicatifs : titre et ordre (UI-7)';

INSERT INTO tec_groupes_parametres (code_groupe, libelle_groupe, ordre_affichage)
SELECT g.code_groupe, g.libelle_groupe, g.ordre_affichage
FROM (
    SELECT 'PATHS'      AS code_groupe, 'Dossier de données' AS libelle_groupe, 10 AS ordre_affichage UNION ALL
    SELECT 'SEANCES',    'Séances',            20 UNION ALL
    SELECT 'SAUVEGARDE', 'Sauvegardes',        30 UNION ALL
    SELECT 'CABINET',    'Cabinet',            40 UNION ALL
    SELECT 'SECURITE',   'Sécurité',           50 UNION ALL
    SELECT 'TECHNIQUE',  'Technique',          60
) AS g
WHERE NOT EXISTS (SELECT 1 FROM tec_groupes_parametres t WHERE t.code_groupe = g.code_groupe);

-- ---------------------------------------------------------------------------
-- 2. Groupes inattendus encore utilisés (titre provisoire = code)
-- ---------------------------------------------------------------------------

INSERT INTO tec_groupes_parametres (code_groupe, libelle_groupe, ordre_affichage)
SELECT DISTINCT p.groupe_parametre, p.groupe_parametre, 900
FROM tec_parametres p
WHERE NOT EXISTS (SELECT 1 FROM tec_groupes_parametres t WHERE t.code_groupe = p.groupe_parametre);

-- ---------------------------------------------------------------------------
-- 3. Clé étrangère
-- ---------------------------------------------------------------------------

SET @fk_existe := (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME = 'tec_parametres'
      AND CONSTRAINT_NAME = 'fk_tec_parametres_groupe' AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@fk_existe = 0,
    'ALTER TABLE tec_parametres ADD CONSTRAINT fk_tec_parametres_groupe
        FOREIGN KEY (groupe_parametre) REFERENCES tec_groupes_parametres (code_groupe)
        ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- 4. Plus de groupe par défaut
-- ---------------------------------------------------------------------------

ALTER TABLE tec_parametres ALTER COLUMN groupe_parametre DROP DEFAULT;

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.40.0',
       'UI-7 : groupes des parametres (tec_groupes_parametres, titre et ordre des cartes).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.40.0');
