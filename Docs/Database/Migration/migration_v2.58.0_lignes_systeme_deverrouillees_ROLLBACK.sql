-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.58.0 - lignes système déverrouillées
--
-- Remet le verrou (est_systeme = 1) sur les lignes déverrouillées en v2.58.0,
-- comme en v2.21.0. À n'utiliser que si leur code n'a pas été modifié entre-temps
-- (la condition porte sur le code).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

UPDATE ref_statuts_seance SET est_systeme = 1
WHERE code_statut_seance IN ('ABSENTE', 'ANNULEE_PATIENT', 'ANNULEE_PRATICIEN', 'MALADE', 'NO_SHOW');

UPDATE ref_liens_patient SET est_systeme = 1
WHERE code_lien_patient = 'MERE';

UPDATE ref_role_legal SET est_systeme = 1
WHERE code_role_legal = 'AUTORITE_PARENTALE';

DELETE FROM tec_meta_schema WHERE version_schema = '2.58.0';

COMMIT;
