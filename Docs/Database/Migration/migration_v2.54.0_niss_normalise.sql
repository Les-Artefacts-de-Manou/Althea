-- ---------------------------------------------------------------------------
-- Migration v2.54.0 - NISS des patients enregistré sous une seule forme
--
-- Audit du 18/10/2026, lot C.2 (étape 2) ; décision de Joëlle (23/10/2026).
--
-- Le NISS était enregistré tel qu'il avait été tapé : « 85.07.30-033.28 » et
-- « 85073003328 » passaient pour deux numéros différents, et l'unicité (index
-- uq_patients_niss) pouvait être contournée. L'application l'enregistre
-- désormais sans séparateurs, en majuscules (Corse 2A/2B) — UtilsNiss.Normaliser —
-- et l'affiche mis en forme.
--
-- Ce script ramène les NISS existants à cette forme : espaces, points, tirets
-- et barres retirés ; vide → NULL. La date de modification des patients n'est
-- pas touchée (ce n'est pas une modification de leur fiche).
--
-- AVANT de l'appliquer : lancer la requête de contrôle ci-dessous. Elle doit ne
-- rien rendre. Si elle rend des lignes, deux fiches portent le même NISS sous
-- deux écritures : les corriger d'abord dans l'application (sinon la mise à
-- jour s'arrête sur l'index d'unicité, sans rien modifier).
--
--   SELECT GROUP_CONCAT(code_patient ORDER BY code_patient) AS patients_concernes
--   FROM patients
--   WHERE niss IS NOT NULL
--   GROUP BY REGEXP_REPLACE(UPPER(TRIM(niss)), '[[:space:]./-]', '')
--   HAVING COUNT(*) > 1;
--
-- Règles : données seulement ; script rejouable (une seconde passe ne change rien).
-- Retour arrière : migration_v2.54.0_niss_normalise_ROLLBACK.sql (la forme
-- d'origine, avec ses séparateurs, n'est pas restituable : rien n'est perdu du
-- numéro lui-même).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

UPDATE patients
SET niss = NULLIF(REGEXP_REPLACE(UPPER(TRIM(niss)), '[[:space:]./-]', ''), ''),
    date_modification = date_modification
WHERE niss IS NOT NULL
  AND NOT (niss <=> NULLIF(REGEXP_REPLACE(UPPER(TRIM(niss)), '[[:space:]./-]', ''), ''));

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.54.0',
       'patients.niss enregistre sous forme normalisee (sans separateurs, majuscules) ; affichage mis en forme par l application.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.54.0');
