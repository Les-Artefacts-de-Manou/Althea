-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.38.0 - Lot 30, étape P2 : sauvegardes
-- Supprime le journal des sauvegardes (l'historique est perdu ; les archives
-- déjà produites restent dans leur dossier) et les paramètres SAUVEGARDE.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

DROP TABLE IF EXISTS `tec_sauvegardes`;

DELETE FROM tec_parametres
WHERE cle_parametre IN ('SAUVEGARDE_DOSSIER', 'SAUVEGARDE_NB_CONSERVEES',
                        'SAUVEGARDE_RAPPEL_JOURS', 'SAUVEGARDE_DOSSIER_MARIADB');

DELETE FROM tec_meta_schema WHERE version_schema = '2.38.0';
