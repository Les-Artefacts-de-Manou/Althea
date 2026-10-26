-- =============================================================================
-- ALTHÉA — Migration v2.29.0 (Lot 27 — Fondation Cabinet / Praticien)
-- =============================================================================
--
-- Objet :
--   1. ref_types_identifiant_pro : référentiel des types d'identifiant professionnel
--      (INAMI, agrément Compy, BCE, TVA…), extensible par paramétrage.
--   2. cab_cabinet : identité du cabinet (une seule ligne active en V1).
--   3. cab_praticiens : praticiens du cabinet (un seul en V1, schéma multi-praticiens).
--   4. cab_praticien_identifiants : numéros professionnels par praticien, historisés.
--   5. cab_praticien_qualifications : diplômes / titres affichables (cachet, documents).
--
-- Règles :
--   - Aucune donnée propre à un cabinet ou un praticien n'est insérée ici :
--     tout est saisi par l'écran d'administration « Cabinet ».
--   - Images (logo, photo, cachet, signature) : stockées sur disque, seul le chemin
--     relatif est conservé (même principe que la photo patient).
--   - Le lien utilisateur ↔ praticien et la règle SuperUser relèvent du lot 28.
--   - Prérequis : migration v2.28.0 appliquée ; sauvegarde préalable obligatoire.
--   - Script rejouable. Compatible : MariaDB 12.x, InnoDB, utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Référentiel des types d'identifiant professionnel
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_types_identifiant_pro` (
    `id_type_identifiant`    bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `code_type_identifiant`  varchar(30)  NOT NULL,
    `libelle_type_identifiant` varchar(100) NOT NULL,
    `masque_saisie`          varchar(50)  NULL COMMENT 'Masque indicatif de saisie (ex. 0-00000-00-000)',
    `regex_validation`       varchar(200) NULL COMMENT 'Expression de contrôle facultative',
    `cible`                  varchar(20)  NOT NULL DEFAULT 'PRATICIEN' COMMENT 'PRATICIEN ou CABINET',
    `est_systeme`            tinyint(1)   NOT NULL DEFAULT 0 COMMENT 'Ligne utilisee par l''application : code non modifiable, non desactivable',
    `actif`                  tinyint(1)   NOT NULL DEFAULT 1,
    `ordre_affichage`        int(11)      NOT NULL DEFAULT 0,
    PRIMARY KEY (`id_type_identifiant`),
    UNIQUE KEY `uq_ref_types_identifiant_pro_code` (`code_type_identifiant`),
    CONSTRAINT `chk_ref_types_identifiant_pro_cible` CHECK (`cible` IN ('PRATICIEN','CABINET'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
  COMMENT='Types d''identifiant professionnel (INAMI, agrement, BCE, TVA...)';

INSERT INTO `ref_types_identifiant_pro` (`code_type_identifiant`, `libelle_type_identifiant`, `masque_saisie`, `cible`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT 'INAMI', 'Numéro INAMI', '0-00000-00-000', 'PRATICIEN', 1, 1, 10 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_types_identifiant_pro` WHERE `code_type_identifiant` = 'INAMI');

INSERT INTO `ref_types_identifiant_pro` (`code_type_identifiant`, `libelle_type_identifiant`, `masque_saisie`, `cible`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT 'AGREMENT_COMPY', 'Numéro d''agrément Commission des Psychologues', NULL, 'PRATICIEN', 0, 1, 20 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_types_identifiant_pro` WHERE `code_type_identifiant` = 'AGREMENT_COMPY');

INSERT INTO `ref_types_identifiant_pro` (`code_type_identifiant`, `libelle_type_identifiant`, `masque_saisie`, `cible`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT 'BCE', 'Numéro d''entreprise (BCE)', '0000.000.000', 'CABINET', 0, 1, 30 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_types_identifiant_pro` WHERE `code_type_identifiant` = 'BCE');

INSERT INTO `ref_types_identifiant_pro` (`code_type_identifiant`, `libelle_type_identifiant`, `masque_saisie`, `cible`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT 'TVA', 'Numéro de TVA', 'BE0000.000.000', 'CABINET', 0, 1, 40 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_types_identifiant_pro` WHERE `code_type_identifiant` = 'TVA');

-- ---------------------------------------------------------------------------
-- SECTION 2 — Cabinet
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `cab_cabinet` (
    `id_cabinet`        bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `nom_cabinet`       varchar(150) NOT NULL,
    `adresse_rue`       varchar(150) NULL,
    `adresse_numero`    varchar(20)  NULL,
    `code_postal`       varchar(20)  NULL,
    `localite`          varchar(100) NULL,
    `pays`              varchar(100) NULL,
    `telephone`         varchar(30)  NULL,
    `email`             varchar(150) NULL,
    `site_web`          varchar(255) NULL,
    `numero_bce`        varchar(30)  NULL,
    `numero_tva`        varchar(30)  NULL,
    `iban`              varchar(34)  NULL,
    `bic`               varchar(11)  NULL,
    `chemin_logo`       varchar(255) NULL COMMENT 'Chemin relatif du logo dans le stockage applicatif',
    `actif`             tinyint(1)   NOT NULL DEFAULT 1,
    `date_creation`     datetime     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `date_modification` datetime     NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`id_cabinet`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
  COMMENT='Identite du cabinet (V1 : une seule ligne active)';

-- ---------------------------------------------------------------------------
-- SECTION 3 — Praticiens
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `cab_praticiens` (
    `id_praticien`       bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `id_cabinet`         bigint(20) unsigned NOT NULL,
    `civilite`           varchar(20)  NULL,
    `nom`                varchar(100) NOT NULL,
    `prenom`             varchar(100) NOT NULL,
    `titre_professionnel` varchar(200) NULL COMMENT 'Ligne de titres affichee (cachet, documents)',
    `telephone`          varchar(30)  NULL,
    `email`              varchar(150) NULL,
    `site_web`           varchar(255) NULL,
    `adresse_rue`        varchar(150) NULL COMMENT 'Vide = adresse du cabinet',
    `adresse_numero`     varchar(20)  NULL,
    `code_postal`        varchar(20)  NULL,
    `localite`           varchar(100) NULL,
    `pays`               varchar(100) NULL,
    `chemin_photo`       varchar(255) NULL,
    `chemin_cachet`      varchar(255) NULL COMMENT 'Cachet importe ; vide = cachet genere depuis les donnees',
    `chemin_signature`   varchar(255) NULL,
    `couleur_agenda`     varchar(9)   NULL COMMENT 'Couleur #RRGGBB pour l''agenda',
    `actif`              tinyint(1)   NOT NULL DEFAULT 1,
    `date_creation`      datetime     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `date_modification`  datetime     NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`id_praticien`),
    KEY `IDX_cab_praticiens_cabinet` (`id_cabinet`),
    CONSTRAINT `fk_cab_praticiens_cabinet` FOREIGN KEY (`id_cabinet`)
        REFERENCES `cab_cabinet` (`id_cabinet`) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
  COMMENT='Praticiens du cabinet (V1 : un seul ; schema multi-praticiens)';

-- ---------------------------------------------------------------------------
-- SECTION 4 — Identifiants professionnels (historisés)
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `cab_praticien_identifiants` (
    `id_identifiant`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `id_praticien`        bigint(20) unsigned NOT NULL,
    `id_type_identifiant` bigint(20) unsigned NOT NULL,
    `valeur`              varchar(50)  NOT NULL,
    `date_debut`          date         NULL,
    `date_fin`            date         NULL,
    `afficher_sur_cachet` tinyint(1)   NOT NULL DEFAULT 1,
    PRIMARY KEY (`id_identifiant`),
    KEY `IDX_cab_praticien_identifiants_praticien` (`id_praticien`),
    KEY `IDX_cab_praticien_identifiants_type` (`id_type_identifiant`),
    CONSTRAINT `fk_cab_praticien_identifiants_praticien` FOREIGN KEY (`id_praticien`)
        REFERENCES `cab_praticiens` (`id_praticien`) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT `fk_cab_praticien_identifiants_type` FOREIGN KEY (`id_type_identifiant`)
        REFERENCES `ref_types_identifiant_pro` (`id_type_identifiant`) ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT `chk_cab_praticien_identifiants_dates` CHECK (`date_fin` IS NULL OR `date_debut` IS NULL OR `date_fin` >= `date_debut`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
  COMMENT='Numeros professionnels du praticien, avec periode de validite';

-- ---------------------------------------------------------------------------
-- SECTION 5 — Qualifications
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `cab_praticien_qualifications` (
    `id_qualification`    bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `id_praticien`        bigint(20) unsigned NOT NULL,
    `libelle`             varchar(200) NOT NULL COMMENT 'Ex. Master en psychologie clinique',
    `etablissement`       varchar(150) NULL,
    `annee_obtention`     smallint(6)  NULL,
    `afficher_sur_cachet` tinyint(1)   NOT NULL DEFAULT 0,
    `ordre_affichage`     int(11)      NOT NULL DEFAULT 0,
    PRIMARY KEY (`id_qualification`),
    KEY `IDX_cab_praticien_qualifications_praticien` (`id_praticien`),
    CONSTRAINT `fk_cab_praticien_qualifications_praticien` FOREIGN KEY (`id_praticien`)
        REFERENCES `cab_praticiens` (`id_praticien`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
  COMMENT='Diplomes et titres du praticien';

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 6 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.29.0',
       'Lot 27 : fondation cabinet - ref_types_identifiant_pro, cab_cabinet, cab_praticiens, cab_praticien_identifiants, cab_praticien_qualifications (aucune donnee praticien).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.29.0');
