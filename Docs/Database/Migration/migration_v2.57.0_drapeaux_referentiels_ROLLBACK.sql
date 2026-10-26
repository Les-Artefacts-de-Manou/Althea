-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.57.0 - drapeaux de référentiel
--
-- Les colonnes sont retirées : l'application d'avant la v2.57.0 retrouve ses
-- listes de codes (annulées et reportée libèrent le créneau ; Maladie l'occupe)
-- et les valeurs Mère / Autorité parentale écrites dans la fenêtre Contact.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

ALTER TABLE `ref_statuts_seance` DROP COLUMN IF EXISTS `propose_creation`;
ALTER TABLE `ref_statuts_seance` DROP COLUMN IF EXISTS `est_non_tenue`;
ALTER TABLE `ref_liens_patient` DROP COLUMN IF EXISTS `par_defaut`;
ALTER TABLE `ref_role_legal` DROP COLUMN IF EXISTS `par_defaut`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.57.0';

COMMIT;
