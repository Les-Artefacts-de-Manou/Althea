-- ---------------------------------------------------------------------------
-- Retour arrière de la migration v2.54.0 - NISS des patients normalisé
--
-- Retire seulement la version du schéma. Les NISS restent sous leur forme
-- normalisée : l'écriture d'origine (points, tirets) n'est pas conservée, et
-- les versions antérieures de l'application les lisent et les acceptent
-- telles quelles (le numéro est le même).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

DELETE FROM tec_meta_schema WHERE version_schema = '2.54.0';
