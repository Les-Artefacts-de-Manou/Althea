-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.44.0 - réglage du correcteur d'orthographe
--
-- Retire le paramètre, puis le groupe EDITEUR s'il n'est plus utilisé. Sans le
-- paramètre, l'application garde le correcteur actif (valeur de secours) et le
-- journalise.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

DELETE FROM tec_parametres WHERE cle_parametre = 'EDITEUR_CORRECTEUR_ORTHOGRAPHE';

DELETE FROM tec_groupes_parametres
WHERE code_groupe = 'EDITEUR'
  AND NOT EXISTS (SELECT 1 FROM tec_parametres WHERE groupe_parametre = 'EDITEUR');

DELETE FROM tec_meta_schema WHERE version_schema = '2.44.0';
