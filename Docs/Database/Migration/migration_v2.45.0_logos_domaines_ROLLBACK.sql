-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.45.0 - logo facultatif des domaines
--
-- Retire la colonne ref_domaines.logo_fichier. Les fichiers Logo.png restent
-- dans {PATH_GENERAL}/Domaines/ (à supprimer à la main si besoin) : l'application
-- d'avant la v2.45.0 les ignore.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

ALTER TABLE `ref_domaines` DROP COLUMN IF EXISTS `logo_fichier`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.45.0';
