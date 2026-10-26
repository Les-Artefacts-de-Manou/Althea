-- ---------------------------------------------------------------------------
-- Migration v2.32.0 - Lot 28
-- Lien praticien <-> compte utilisateur (cab_praticiens.id_utilisateur).
--
-- Règles :
--   - Le compte lié est le compte de travail du praticien (rôle de base
--     SuperUser, élévation Admin par cadenas). Le compte Admin technique
--     n'est jamais lié à un praticien et n'est pas modifié par cette migration.
--   - Lien facultatif (NULL) pour rester compatible avec les praticiens
--     existants ; un compte ne peut être lié qu'à un seul praticien (UNIQUE).
--   - ON DELETE SET NULL : supprimer un compte ne supprime pas le praticien.
--   - V1 : un seul praticien lié ; le schéma reste ouvert pour V2/V3.
-- ---------------------------------------------------------------------------

SET @col_existe := (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_praticiens' AND COLUMN_NAME = 'id_utilisateur');

SET @sql := IF(@col_existe = 0,
    'ALTER TABLE cab_praticiens ADD COLUMN id_utilisateur bigint(20) unsigned NULL COMMENT ''Compte de travail du praticien (sec_utilisateurs) ; NULL = non lie'' AFTER id_cabinet, ADD UNIQUE KEY uq_cab_praticiens_utilisateur (id_utilisateur), ADD CONSTRAINT fk_cab_praticiens_utilisateur FOREIGN KEY (id_utilisateur) REFERENCES sec_utilisateurs (id_utilisateur) ON UPDATE CASCADE ON DELETE SET NULL',
    'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.32.0',
       'Lot 28 : lien praticien - compte utilisateur (cab_praticiens.id_utilisateur, nullable, unique, FK sec_utilisateurs).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.32.0');
