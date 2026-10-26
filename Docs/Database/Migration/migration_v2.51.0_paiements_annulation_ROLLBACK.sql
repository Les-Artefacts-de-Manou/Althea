-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.51.0 - annulation d'un encaissement en colonnes
--
-- Les annulations faites après la migration sont d'abord recopiées dans le
-- commentaire, sous la forme que lisait l'application d'avant
-- (« [Annulé le jj/mm/aaaa par <nom affiché> : motif] »), puis les colonnes sont
-- retirées : rien n'est perdu.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

UPDATE paiements p
LEFT JOIN sec_utilisateurs u ON u.id_utilisateur = p.id_utilisateur_annulation
SET p.commentaire = TRIM(CONCAT(COALESCE(p.commentaire, ''), ' ',
        '[Annulé le ', DATE_FORMAT(p.date_annulation, '%d/%m/%Y'),
        COALESCE(CONCAT(' par ', u.nom_affichage), ''),
        ' : ', COALESCE(p.motif_annulation, ''), ']'))
WHERE p.date_annulation IS NOT NULL;

ALTER TABLE `paiements` DROP FOREIGN KEY IF EXISTS `fk_paiements_utilisateur_annulation`;
ALTER TABLE `paiements` DROP COLUMN IF EXISTS `motif_annulation`;
ALTER TABLE `paiements` DROP COLUMN IF EXISTS `id_utilisateur_annulation`;
ALTER TABLE `paiements` DROP COLUMN IF EXISTS `date_annulation`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.51.0';
