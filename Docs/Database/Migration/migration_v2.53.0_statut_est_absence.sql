-- ---------------------------------------------------------------------------
-- Migration v2.53.0 - L'absence non excusée (« no-show ») se règle dans le
--                     référentiel des statuts de séance
--
-- Audit du 18/10/2026, lot B.6 (étape 2) ; décision de Joëlle (22/10/2026) :
-- une case dans le référentiel plutôt que le code ABSENTE écrit en dur.
--
-- 1. ref_statuts_seance.est_absence : le statut traduit une absence non excusée
--    du patient. Cochée pour ABSENTE (seul statut concerné aujourd'hui) ; d'autres
--    statuts pourront l'être depuis Référentiels › Statuts de séance.
-- 2. Vue v_seances_solde (v2.49.0) recréée : « facturable » lit est_absence au
--    lieu du code 'ABSENTE' (une absence n'est pas facturée dans un domaine qui
--    ne facture pas les absences). Définition inchangée par ailleurs.
--
-- Ce que la case gouverne dans l'application : tarif (absence non facturable),
-- facturation (vue), compteur d'absences et « dernière séance » de la grille
-- Patients, pastille et filtre « Absences » de l'onglet Séances, aperçu.
--
-- Règles : structure + une valeur de départ (ABSENTE) ; script rejouable.
-- Retour arrière : migration_v2.53.0_statut_est_absence_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- 1. Case « absence non excusée » ---------------------------------------------
ALTER TABLE `ref_statuts_seance`
  ADD COLUMN IF NOT EXISTS `est_absence` tinyint(1) NOT NULL DEFAULT 0
    COMMENT '1 = absence non excusée du patient (no-show) : non facturée si le domaine ne facture pas les absences, comptée dans les absences (v2.53.0)'
    AFTER `consomme_quota`;

UPDATE ref_statuts_seance
SET est_absence = 1
WHERE code_statut_seance = 'ABSENTE';

-- 2. Vue des soldes : « facturable » lit la case ------------------------------
CREATE OR REPLACE SQL SECURITY INVOKER VIEW v_seances_solde AS
SELECT
    b.id_seance,
    b.id_dossier,
    b.tarif_seance,
    b.montant_regle,
    b.est_du,
    b.est_facturable,
    CASE WHEN b.est_facturable = 1 THEN GREATEST(b.tarif_seance - b.montant_regle, 0) ELSE 0 END AS reste_du,
    b.est_echue,
    CASE WHEN b.est_du = 0 THEN b.montant_regle
         ELSE GREATEST(b.montant_regle - b.tarif_seance, 0)
    END AS montant_avoir
FROM (
    SELECT
        s.id_seance,
        s.id_dossier,
        s.tarif_seance,
        COALESCE(r.regle, 0) AS montant_regle,
        COALESCE(ss.est_du, 0) AS est_du,
        CASE WHEN COALESCE(ss.est_du, 0) = 1
                  AND NOT (COALESCE(dom.no_show_facturable, 1) = 0 AND COALESCE(ss.est_absence, 0) = 1)
             THEN 1 ELSE 0 END AS est_facturable,
        CASE WHEN COALESCE(s.heure_fin, s.heure_debut) IS NOT NULL
             THEN CASE WHEN TIMESTAMP(s.date_seance, COALESCE(s.heure_fin, s.heure_debut)) <= NOW() THEN 1 ELSE 0 END
             ELSE CASE WHEN s.date_seance < CURDATE() THEN 1 ELSE 0 END
        END AS est_echue
    FROM seances s
    LEFT JOIN ref_statuts_seance ss ON ss.id_statut_seance = s.id_statut_seance
    LEFT JOIN dossiers d ON d.id_dossier = s.id_dossier
    LEFT JOIN ref_domaines dom ON dom.id_domaine = d.id_domaine
    LEFT JOIN (
        SELECT lps.id_seance, SUM(lps.montant_affecte) AS regle
        FROM lia_paiements_seances lps
        INNER JOIN paiements p ON p.id_paiement = lps.id_paiement
        WHERE p.statut_paiement = 'PAYE'
        GROUP BY lps.id_seance
    ) r ON r.id_seance = s.id_seance
) b;

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.53.0',
       'ref_statuts_seance.est_absence (cochee pour ABSENTE) ; v_seances_solde lit la case au lieu du code.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.53.0');
