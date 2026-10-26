-- ---------------------------------------------------------------------------
-- Migration v2.43.0 - Textes types : plusieurs usages par texte, tous les éditeurs
--
-- Décisions de Joëlle (19/10/2026, propositions 1 à 4 acceptées) :
-- 1. Tous les éditeurs existants ont un usage : ajout de l'alerte du patient ;
--    les commentaires sont séparés (contact, intervenant, thérapeute).
-- 2. (Règle de développement : tout nouvel éditeur reçoit son usage.)
-- 3. Écran de gestion des usages (libellé, ordre) : aucune structure à ajouter.
-- 4. Un texte peut servir à plusieurs usages : table de liaison
--    lia_textes_types_usages ; un texte sans usage reste proposé partout.
--
-- Étapes :
-- 1. Nouveaux usages (lignes système) et ordre d'affichage de l'ensemble.
-- 2. Table lia_textes_types_usages.
-- 3. Reprise de l'usage de chaque texte (ref_textes_types.id_usage_texte_type) :
--    un texte « Commentaires » est rattaché aux trois nouveaux usages de
--    commentaire.
-- 4. Suppression de la colonne ref_textes_types.id_usage_texte_type (et de sa
--    clé étrangère), remplacée par la liaison, une fois l'usage repris.
-- 5. Suppression de l'usage COMMENTAIRE, remplacé par les trois usages séparés
--    (plus aucun texte ne s'y rattache après l'étape 3).
--
-- Règles : script rejouable ; les textes saisis sont conservés avec leur usage.
-- Retour arrière : migration_v2.43.0_textes_types_usages_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- 1. Usages
-- ---------------------------------------------------------------------------

INSERT INTO ref_usages_textes_types (code_usage, libelle_usage, ordre_affichage, est_systeme)
SELECT u.code_usage, u.libelle_usage, u.ordre_affichage, 1
FROM (
    SELECT 'ALERTE_PATIENT'          AS code_usage, 'Alerte du patient'          AS libelle_usage, 20 AS ordre_affichage UNION ALL
    SELECT 'COMMENTAIRE_CONTACT',     'Commentaire d''un contact',      60 UNION ALL
    SELECT 'COMMENTAIRE_INTERVENANT', 'Commentaire d''un intervenant',  70 UNION ALL
    SELECT 'COMMENTAIRE_THERAPEUTE',  'Commentaire d''un thérapeute',   80
) AS u
WHERE NOT EXISTS (SELECT 1 FROM ref_usages_textes_types t WHERE t.code_usage = u.code_usage);

-- Ordre des usages existants (aucun écran ne permettait de le changer avant cette version).
UPDATE ref_usages_textes_types SET ordre_affichage = 10 WHERE code_usage = 'ANAMNESE';
UPDATE ref_usages_textes_types SET ordre_affichage = 30 WHERE code_usage = 'ZONE_DOSSIER';
UPDATE ref_usages_textes_types SET ordre_affichage = 40 WHERE code_usage = 'NOTES_SEANCE';
UPDATE ref_usages_textes_types SET ordre_affichage = 50 WHERE code_usage = 'EVOLUTION_SEANCE';

-- ---------------------------------------------------------------------------
-- 2. Liaison texte - usages
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `lia_textes_types_usages` (
    `id_texte_type`       bigint(20) unsigned NOT NULL,
    `id_usage_texte_type` bigint(20) unsigned NOT NULL,
    PRIMARY KEY (`id_texte_type`, `id_usage_texte_type`),
    KEY `fk_lia_textes_types_usages_usage` (`id_usage_texte_type`),
    CONSTRAINT `fk_lia_textes_types_usages_texte` FOREIGN KEY (`id_texte_type`)
        REFERENCES `ref_textes_types` (`id_texte_type`) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT `fk_lia_textes_types_usages_usage` FOREIGN KEY (`id_usage_texte_type`)
        REFERENCES `ref_usages_textes_types` (`id_usage_texte_type`) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Usages d un texte type ; aucun usage = propose dans tous les editeurs';

-- ---------------------------------------------------------------------------
-- 3 à 5. Reprise de l'usage unique, puis suppression de la colonne et de COMMENTAIRE
--        (seulement si la colonne existe encore : rejouable)
-- ---------------------------------------------------------------------------

SET @colonne_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_textes_types' AND COLUMN_NAME = 'id_usage_texte_type');

-- 3a. Textes à usage ordinaire : même usage.
SET @sql := IF(@colonne_existe = 1,
    'INSERT IGNORE INTO lia_textes_types_usages (id_texte_type, id_usage_texte_type)
     SELECT t.id_texte_type, t.id_usage_texte_type
     FROM ref_textes_types t
     JOIN ref_usages_textes_types u ON u.id_usage_texte_type = t.id_usage_texte_type
     WHERE u.code_usage <> ''COMMENTAIRE''',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 3b. Textes « Commentaires » : les trois usages de commentaire.
SET @sql := IF(@colonne_existe = 1,
    'INSERT IGNORE INTO lia_textes_types_usages (id_texte_type, id_usage_texte_type)
     SELECT t.id_texte_type, n.id_usage_texte_type
     FROM ref_textes_types t
     JOIN ref_usages_textes_types u ON u.id_usage_texte_type = t.id_usage_texte_type
     JOIN ref_usages_textes_types n ON n.code_usage IN (''COMMENTAIRE_CONTACT'', ''COMMENTAIRE_INTERVENANT'', ''COMMENTAIRE_THERAPEUTE'')
     WHERE u.code_usage = ''COMMENTAIRE''',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 4. Clé étrangère puis colonne.
SET @fk_existe := (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_textes_types'
      AND CONSTRAINT_NAME = 'fk_ref_textes_types_usage' AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@fk_existe = 1, 'ALTER TABLE ref_textes_types DROP FOREIGN KEY fk_ref_textes_types_usage', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @sql := IF(@colonne_existe = 1, 'ALTER TABLE ref_textes_types DROP COLUMN id_usage_texte_type', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 5. Usage COMMENTAIRE remplacé (refusé par la clé étrangère s'il restait lié : sécurité).
DELETE FROM ref_usages_textes_types
WHERE code_usage = 'COMMENTAIRE'
  AND NOT EXISTS (SELECT 1 FROM lia_textes_types_usages l WHERE l.id_usage_texte_type = ref_usages_textes_types.id_usage_texte_type);

-- ---------------------------------------------------------------------------
-- Journalisation
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.43.0',
       'Textes types : plusieurs usages par texte (lia_textes_types_usages), usages alerte et commentaires separes.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.43.0');
