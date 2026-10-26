-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.55.0 - référentiel des spécialités
--
-- La colonne texte therapeutes.specialite est d'abord remise à jour depuis le
-- référentiel (spécialités choisies après la migration comprises), puis le lien
-- et la table sont retirés. L'application d'avant la v2.55.0 relit la colonne
-- texte.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

UPDATE therapeutes t
LEFT JOIN ref_specialites s ON s.id_specialite = t.id_specialite
SET t.specialite = s.libelle_specialite,
    t.date_modification = t.date_modification;

ALTER TABLE `therapeutes` DROP FOREIGN KEY IF EXISTS `fk_therapeutes_ref_specialites`;
ALTER TABLE `therapeutes` DROP COLUMN IF EXISTS `id_specialite`;
DROP TABLE IF EXISTS `ref_specialites`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.55.0';
