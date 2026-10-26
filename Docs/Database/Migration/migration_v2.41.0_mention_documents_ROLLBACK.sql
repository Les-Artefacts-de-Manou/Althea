-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.41.0 - mention de confidentialité des documents
--
-- Retire le paramètre DOCUMENT_MENTION_CONFIDENTIALITE, puis le groupe DOCUMENTS
-- s'il n'est plus utilisé par aucun paramètre, et la ligne de tec_meta_schema.
-- Sans le paramètre, l'application imprime la mention de secours « Confidentiel »
-- (ClesParametres) et le journalise.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

DELETE FROM tec_parametres WHERE cle_parametre = 'DOCUMENT_MENTION_CONFIDENTIALITE';

DELETE FROM tec_groupes_parametres
WHERE code_groupe = 'DOCUMENTS'
  AND NOT EXISTS (SELECT 1 FROM tec_parametres WHERE groupe_parametre = 'DOCUMENTS');

DELETE FROM tec_meta_schema WHERE version_schema = '2.41.0';
