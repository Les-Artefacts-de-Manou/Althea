-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.50.0 - mode d'un paiement relié par identifiant
--
-- La colonne texte paiements.mode_paiement est d'abord remise à jour depuis le
-- référentiel (paiements saisis après la migration compris, au libellé, comme
-- l'écrivait l'application d'avant), puis le lien est retiré. Les modes repris
-- (inactifs, codes REPRISE_… ou issus d'anciens libellés) restent dans le
-- référentiel : ils peuvent avoir été utilisés depuis.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

UPDATE paiements p
JOIN ref_modes_paiement m ON m.id_mode_paiement = p.id_mode_paiement
SET p.mode_paiement = m.libelle_mode_paiement;

ALTER TABLE `paiements` DROP FOREIGN KEY IF EXISTS `fk_paiements_ref_modes_paiement`;
ALTER TABLE `paiements` DROP COLUMN IF EXISTS `id_mode_paiement`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.50.0';
