-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.48.0 - fin de l'élévation après inactivité
--
-- L'application d'avant la v2.48.0 ne lit pas ce paramètre (élévation sans fin).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

DELETE FROM tec_parametres WHERE cle_parametre = 'SECURITE_ELEVATION_MINUTES';

DELETE FROM tec_meta_schema WHERE version_schema = '2.48.0';
