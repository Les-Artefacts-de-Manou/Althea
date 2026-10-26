-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.56.0 - référentiel des codes postaux belges
--
-- La table n'est référencée par aucune autre : elle est simplement retirée.
-- L'application d'avant la v2.56.0 ne contrôle que le format des codes postaux.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

DROP TABLE IF EXISTS `ref_codes_postaux_be`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.56.0';

COMMIT;
