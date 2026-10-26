-- ---------------------------------------------------------------------------
-- Migration v2.33.0 - Lot 29
-- Organismes étoffés : coordonnées de contact et périodicité d'encodage.
--
-- 1. Référentiel paramétrable ref_periodicites (périodicités d'encodage /
--    de facturation). Attribut métier nb_mois : aucun comportement n'est
--    déduit du code de la périodicité.
-- 2. Colonnes de coordonnées sur ref_organismes (adresse, téléphones,
--    e-mails, site web, plateforme d'encodage, procédure) et rattachement
--    facultatif ref_organismes.id_periodicite.
--
-- Règles : script rejouable (contrôles d'existence) ; aucune ligne existante
-- de ref_organismes n'est modifiée (nouvelles colonnes toutes NULL).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Référentiel ref_periodicites
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_periodicites` (
    `id_periodicite`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `code_periodicite`    varchar(30)  NOT NULL,
    `libelle_periodicite` varchar(100) NOT NULL,
    `nb_mois`             int(11)      NULL COMMENT 'Duree de la periode en mois (NULL = non periodique / a la demande)',
    `ordre_affichage`     int(11)      NOT NULL DEFAULT 0,
    `actif`               tinyint(1)   NOT NULL DEFAULT 1,
    `est_systeme`         tinyint(1)   NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
    `date_creation`       datetime     NOT NULL DEFAULT current_timestamp(),
    `date_modification`   datetime     NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
    PRIMARY KEY (`id_periodicite`),
    UNIQUE KEY `uq_ref_periodicites_code` (`code_periodicite`),
    UNIQUE KEY `uq_ref_periodicites_libelle` (`libelle_periodicite`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Periodicites d encodage / facturation des organismes';

INSERT INTO `ref_periodicites` (`code_periodicite`, `libelle_periodicite`, `nb_mois`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'MENSUELLE', 'Mensuelle', 1, 10, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_periodicites` WHERE `code_periodicite` = 'MENSUELLE');

INSERT INTO `ref_periodicites` (`code_periodicite`, `libelle_periodicite`, `nb_mois`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'TRIMESTRIELLE', 'Trimestrielle', 3, 20, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_periodicites` WHERE `code_periodicite` = 'TRIMESTRIELLE');

INSERT INTO `ref_periodicites` (`code_periodicite`, `libelle_periodicite`, `nb_mois`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'ANNUELLE', 'Annuelle', 12, 30, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_periodicites` WHERE `code_periodicite` = 'ANNUELLE');

INSERT INTO `ref_periodicites` (`code_periodicite`, `libelle_periodicite`, `nb_mois`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'A_LA_DEMANDE', 'À la demande (non périodique)', NULL, 40, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_periodicites` WHERE `code_periodicite` = 'A_LA_DEMANDE');

-- ---------------------------------------------------------------------------
-- SECTION 2 — Coordonnées et périodicité sur ref_organismes
-- ---------------------------------------------------------------------------

SET @col_existe := (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_organismes' AND COLUMN_NAME = 'adresse');

SET @sql := IF(@col_existe = 0,
    'ALTER TABLE ref_organismes
        ADD COLUMN adresse            varchar(250) NULL AFTER libelle_organisme,
        ADD COLUMN telephone          varchar(30)  NULL AFTER adresse,
        ADD COLUMN telephone_2        varchar(30)  NULL AFTER telephone,
        ADD COLUMN email              varchar(150) NULL AFTER telephone_2,
        ADD COLUMN email_2            varchar(150) NULL AFTER email,
        ADD COLUMN site_web           varchar(250) NULL AFTER email_2,
        ADD COLUMN url_plateforme     varchar(250) NULL COMMENT ''Plateforme d encodage / portail de l organisme'' AFTER site_web,
        ADD COLUMN procedure_encodage text         NULL COMMENT ''Procedure d encodage / de facturation (texte libre)'' AFTER url_plateforme,
        ADD COLUMN id_periodicite     bigint(20) unsigned NULL AFTER procedure_encodage,
        ADD KEY IDX_ref_organismes_periodicite (id_periodicite),
        ADD CONSTRAINT fk_ref_organismes_periodicite FOREIGN KEY (id_periodicite)
            REFERENCES ref_periodicites (id_periodicite) ON UPDATE CASCADE ON DELETE SET NULL',
    'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Journalisation
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.33.0',
       'Lot 29 : referentiel ref_periodicites (nb_mois) ; coordonnees (adresse, telephones, emails, site, plateforme, procedure) et id_periodicite sur ref_organismes.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.33.0');
