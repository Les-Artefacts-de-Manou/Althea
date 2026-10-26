-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.43.0 - plusieurs usages par texte
--
-- Revient à un seul usage par texte : chaque texte garde le premier de ses
-- usages (ordre d'affichage) ; un texte lié aux usages de commentaire revient
-- à l'usage COMMENTAIRE, recréé. Les usages ajoutés par la v2.43.0 sont retirés.
-- ATTENTION : un texte lié à plusieurs usages perd les usages suivants.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

INSERT INTO ref_usages_textes_types (code_usage, libelle_usage, ordre_affichage, est_systeme)
SELECT 'COMMENTAIRE', 'Commentaires (contacts, intervenants, thérapeutes)', 50, 1
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM ref_usages_textes_types WHERE code_usage = 'COMMENTAIRE');

SET @colonne_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_textes_types' AND COLUMN_NAME = 'id_usage_texte_type');
SET @sql := IF(@colonne_existe = 0,
    'ALTER TABLE ref_textes_types ADD COLUMN id_usage_texte_type bigint(20) unsigned DEFAULT NULL COMMENT ''NULL = propose dans tous les editeurs'' AFTER libelle_texte_type',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Premier usage de chaque texte ; les usages de commentaire reviennent à COMMENTAIRE.
UPDATE ref_textes_types t
SET t.id_usage_texte_type = (
    SELECT CASE WHEN u.code_usage LIKE 'COMMENTAIRE\_%'
                THEN (SELECT c.id_usage_texte_type FROM ref_usages_textes_types c WHERE c.code_usage = 'COMMENTAIRE')
                ELSE u.id_usage_texte_type END
    FROM lia_textes_types_usages l
    JOIN ref_usages_textes_types u ON u.id_usage_texte_type = l.id_usage_texte_type
    WHERE l.id_texte_type = t.id_texte_type
    ORDER BY u.ordre_affichage
    LIMIT 1);

SET @fk_existe := (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_textes_types'
      AND CONSTRAINT_NAME = 'fk_ref_textes_types_usage' AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@fk_existe = 0,
    'ALTER TABLE ref_textes_types ADD CONSTRAINT fk_ref_textes_types_usage FOREIGN KEY (id_usage_texte_type)
        REFERENCES ref_usages_textes_types (id_usage_texte_type) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

DROP TABLE IF EXISTS `lia_textes_types_usages`;

-- Un texte dont le premier usage était l'alerte (usage retiré) devient « proposé partout ».
UPDATE ref_textes_types
SET id_usage_texte_type = NULL
WHERE id_usage_texte_type IN (SELECT id_usage_texte_type FROM ref_usages_textes_types WHERE code_usage = 'ALERTE_PATIENT');

DELETE FROM ref_usages_textes_types
WHERE code_usage IN ('ALERTE_PATIENT', 'COMMENTAIRE_CONTACT', 'COMMENTAIRE_INTERVENANT', 'COMMENTAIRE_THERAPEUTE');

DELETE FROM tec_meta_schema WHERE version_schema = '2.43.0';
