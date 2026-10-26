-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.47.0 - types d'identifiant contrôlés par Althéa
--
-- BCE et TVA redeviennent des lignes ordinaires (état de la v2.29.0) ; INAMI
-- reste une ligne système, comme avant la v2.47.0.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

UPDATE ref_types_identifiant_pro
SET est_systeme = 0
WHERE code_type_identifiant IN ('BCE', 'TVA');

DELETE FROM tec_meta_schema WHERE version_schema = '2.47.0';
