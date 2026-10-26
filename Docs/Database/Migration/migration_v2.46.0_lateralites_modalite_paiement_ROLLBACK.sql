-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.46.0 - latéralités, modalité de paiement proposée
--
-- La colonne texte patients.lateralite est d'abord remise à jour depuis le
-- référentiel (latéralités saisies après la migration comprises), puis le lien,
-- la table et le paramètre sont retirés. L'application d'avant la v2.46.0
-- relit la colonne texte et reprend « À la séance » écrit dans le code.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

UPDATE patients p
LEFT JOIN ref_lateralites l ON l.id_lateralite = p.id_lateralite
SET p.lateralite = l.libelle_lateralite;

ALTER TABLE `patients` DROP FOREIGN KEY IF EXISTS `fk_patients_ref_lateralites`;
ALTER TABLE `patients` DROP COLUMN IF EXISTS `id_lateralite`;
DROP TABLE IF EXISTS `ref_lateralites`;

DELETE FROM tec_parametres WHERE cle_parametre = 'DOSSIER_MODALITE_PAIEMENT_DEFAUT';
DELETE FROM tec_groupes_parametres
WHERE code_groupe = 'DOSSIERS'
  AND NOT EXISTS (SELECT 1 FROM tec_parametres WHERE groupe_parametre = 'DOSSIERS');

DELETE FROM tec_meta_schema WHERE version_schema = '2.46.0';
