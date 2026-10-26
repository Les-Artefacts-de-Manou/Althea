-- ---------------------------------------------------------------------------
-- Migration v2.42.0 - Éditeur de texte, temps 2 : textes types
--
-- Un texte type est une phrase ou une trame réutilisable (ex. « Trame de bilan
-- initial ») que l'on insère dans un éditeur de texte d'Althéa. Il est proposé
-- selon son usage : chaque éditeur déclare le sien (anamnèse, notes de séance…),
-- et ne propose que les textes de cet usage, plus ceux proposés partout.
--
-- Choix de Joëlle (19/10/2026) : textes proposés par usage, liste des usages en
-- base ; un texte sans usage est proposé partout.
--
-- 1. ref_usages_textes_types : les usages, lignes système (leur code est repris
--    par le code de l'application, module UsagesTexteType) ; le libellé reste
--    modifiable.
-- 2. ref_textes_types : les textes (code, libellé, usage facultatif, contenu en
--    RTF et en texte brut, ordre, activation).
--
-- À ne pas confondre avec modeles_documents (modèles Word complets, en fichiers).
--
-- Règles : script rejouable ; aucune table existante modifiée.
-- Retour arrière : migration_v2.42.0_textes_types_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- 1. Usages
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_usages_textes_types` (
    `id_usage_texte_type` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `code_usage`          varchar(30)  NOT NULL COMMENT 'Code repris par l application (UsagesTexteType)',
    `libelle_usage`       varchar(100) NOT NULL,
    `actif`               tinyint(1)   NOT NULL DEFAULT 1,
    `ordre_affichage`     int(11)      NOT NULL DEFAULT 0,
    `est_systeme`         tinyint(1)   NOT NULL DEFAULT 1 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
    PRIMARY KEY (`id_usage_texte_type`),
    UNIQUE KEY `uq_ref_usages_textes_types_code` (`code_usage`),
    UNIQUE KEY `uq_ref_usages_textes_types_libelle` (`libelle_usage`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Usages des textes types : editeurs de texte ou un texte est propose';

INSERT INTO ref_usages_textes_types (code_usage, libelle_usage, ordre_affichage, est_systeme)
SELECT u.code_usage, u.libelle_usage, u.ordre_affichage, 1
FROM (
    SELECT 'ANAMNESE'         AS code_usage, 'Anamnèse'            AS libelle_usage, 10 AS ordre_affichage UNION ALL
    SELECT 'ZONE_DOSSIER',     'Zones du dossier',     20 UNION ALL
    SELECT 'NOTES_SEANCE',     'Notes de séance',      30 UNION ALL
    SELECT 'EVOLUTION_SEANCE', 'Évolution de séance',  40 UNION ALL
    SELECT 'COMMENTAIRE',      'Commentaires (contacts, intervenants, thérapeutes)', 50
) AS u
WHERE NOT EXISTS (SELECT 1 FROM ref_usages_textes_types t WHERE t.code_usage = u.code_usage);

-- ---------------------------------------------------------------------------
-- 2. Textes types
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_textes_types` (
    `id_texte_type`       bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `code_texte_type`     varchar(30)  NOT NULL,
    `libelle_texte_type`  varchar(100) NOT NULL,
    `id_usage_texte_type` bigint(20) unsigned DEFAULT NULL COMMENT 'NULL = propose dans tous les editeurs',
    `contenu_rtf`         longtext DEFAULT NULL COMMENT 'Texte au format RTF (insertion, mise en forme)',
    `contenu_txt`         longtext DEFAULT NULL COMMENT 'Texte brut (recherche, repli si le RTF est illisible)',
    `actif`               tinyint(1)   NOT NULL DEFAULT 1,
    `ordre_affichage`     int(11)      NOT NULL DEFAULT 0,
    `date_creation`       datetime     NOT NULL DEFAULT current_timestamp(),
    `date_modification`   datetime     NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
    PRIMARY KEY (`id_texte_type`),
    UNIQUE KEY `uq_ref_textes_types_code` (`code_texte_type`),
    UNIQUE KEY `uq_ref_textes_types_libelle` (`libelle_texte_type`),
    KEY `fk_ref_textes_types_usage` (`id_usage_texte_type`),
    CONSTRAINT `fk_ref_textes_types_usage` FOREIGN KEY (`id_usage_texte_type`)
        REFERENCES `ref_usages_textes_types` (`id_usage_texte_type`) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Textes types inseres dans les editeurs de texte (temps 2)';

-- ---------------------------------------------------------------------------
-- Journalisation
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.42.0',
       'Editeur temps 2 : textes types (ref_textes_types) et leurs usages (ref_usages_textes_types).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.42.0');
