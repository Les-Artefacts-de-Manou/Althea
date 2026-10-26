-- ---------------------------------------------------------------------------
-- Migration v2.58.0 - Lignes système devenues inutiles : déverrouillage
--
-- Suite de l'audit C.3 étape 2 (v2.57.0, ADR-54 ; décision de Joëlle du
-- 24/10/2026, faite le 25/10/2026) : ces lignes étaient verrouillées
-- (est_systeme = 1, ADR-28) parce que l'application les cherchait par leur
-- code. Depuis la v2.57.0, leur comportement est porté par des cases du
-- référentiel (est_absence, est_non_tenue, par_defaut) : plus aucune règle ne
-- les cherche par leur code. Elles redeviennent des lignes ordinaires : code
-- modifiable, désactivation possible, comme un statut ajouté par l'utilisatrice.
--
-- Déverrouillées :
--   ref_statuts_seance : ABSENTE, ANNULEE_PATIENT, ANNULEE_PRATICIEN, MALADE,
--                        NO_SHOW (marquée en v2.21.0, si la ligne existe)
--   ref_liens_patient  : MERE
--   ref_role_legal     : AUTORITE_PARENTALE
--
-- Restent verrouillées, toujours cherchées par leur code (CodesSysteme) ou
-- indispensables : PLANIFIEE, REALISEE, REPORTEE ; statuts de dossier ; types de
-- document ANAMNESE, ZONE_CLINIQUE, PHOTO_IDENTITE ; mode de paiement AVOIR
-- (seul mode marqué est_avoir) ; tarif INAMI ; et les autres référentiels.
--
-- Les cases (absence, non tenue, par défaut) ne changent pas : seul le verrou
-- est levé. Aucune donnée supprimée. Script rejouable. Retour arrière :
-- migration_v2.58.0_lignes_systeme_deverrouillees_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

UPDATE ref_statuts_seance SET est_systeme = 0
WHERE code_statut_seance IN ('ABSENTE', 'ANNULEE_PATIENT', 'ANNULEE_PRATICIEN', 'MALADE', 'NO_SHOW');

UPDATE ref_liens_patient SET est_systeme = 0
WHERE code_lien_patient = 'MERE';

UPDATE ref_role_legal SET est_systeme = 0
WHERE code_role_legal = 'AUTORITE_PARENTALE';

-- Version du schéma ------------------------------------------------------------
INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.58.0',
       'Lignes systeme devenues inutiles deverrouillees : statuts ABSENTE, ANNULEE_*, MALADE, NO_SHOW ; lien MERE ; role AUTORITE_PARENTALE (audit C.3).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.58.0');

COMMIT;

-- Contrôle :
-- SELECT 'statut' AS ref, code_statut_seance AS code, est_systeme FROM ref_statuts_seance
-- UNION ALL SELECT 'lien', code_lien_patient, est_systeme FROM ref_liens_patient WHERE est_systeme = 1 OR code_lien_patient = 'MERE'
-- UNION ALL SELECT 'role', code_role_legal, est_systeme FROM ref_role_legal WHERE est_systeme = 1 OR code_role_legal = 'AUTORITE_PARENTALE';
