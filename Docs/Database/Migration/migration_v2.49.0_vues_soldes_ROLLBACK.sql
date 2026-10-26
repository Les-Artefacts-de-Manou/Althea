-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.49.0 - vues v_seances_solde et v_dossier_dominant
--
-- À n'utiliser qu'avec l'application d'avant la v2.49.0 : celle-ci lit les vues.
-- Aucune donnée n'est touchée.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

DROP VIEW IF EXISTS v_dossier_dominant;
DROP VIEW IF EXISTS v_seances_solde;

DELETE FROM tec_meta_schema WHERE version_schema = '2.49.0';
