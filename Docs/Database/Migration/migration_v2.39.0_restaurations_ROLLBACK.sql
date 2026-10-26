-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.39.0 - Lot 30, étape P3 : restaurations
-- Supprime le journal des restaurations (l'historique est perdu ; les archives
-- et les anciens dossiers de données renommés restent sur le disque).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

DROP TABLE IF EXISTS `tec_restaurations`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.39.0';
