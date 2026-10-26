-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.40.0 - UI-7 : groupes des paramètres
-- Retire la clé étrangère et la table des groupes, rend à groupe_parametre sa
-- valeur par défaut 'GENERAL'. Les paramètres et leurs valeurs ne changent pas
-- (groupe_parametre garde ses codes). Une application en V2 de UC_Parametres
-- ne peut plus afficher les paramètres après ce retour arrière.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

SET @fk_existe := (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME = 'tec_parametres'
      AND CONSTRAINT_NAME = 'fk_tec_parametres_groupe' AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@fk_existe > 0,
    'ALTER TABLE tec_parametres DROP FOREIGN KEY fk_tec_parametres_groupe',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Index créé par MariaDB avec la clé étrangère
SET @index_existe := (SELECT COUNT(*) FROM information_schema.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tec_parametres'
      AND INDEX_NAME = 'fk_tec_parametres_groupe');
SET @sql := IF(@index_existe > 0,
    'ALTER TABLE tec_parametres DROP INDEX fk_tec_parametres_groupe',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

ALTER TABLE tec_parametres ALTER COLUMN groupe_parametre SET DEFAULT 'GENERAL';

DROP TABLE IF EXISTS `tec_groupes_parametres`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.40.0';
