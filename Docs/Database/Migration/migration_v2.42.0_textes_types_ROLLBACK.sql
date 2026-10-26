-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.42.0 - textes types
--
-- ATTENTION : supprime les textes types saisis. Les exporter avant si besoin :
--   SELECT code_texte_type, libelle_texte_type, contenu_txt FROM ref_textes_types;
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

DROP TABLE IF EXISTS `ref_textes_types`;
DROP TABLE IF EXISTS `ref_usages_textes_types`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.42.0';
