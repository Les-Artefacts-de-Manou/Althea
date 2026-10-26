-- ---------------------------------------------------------------------------
-- Migration v2.49.0 - Une seule définition du solde d'une séance et du dossier
--                     dominant (vues SQL)
--
-- Audit du 18/10/2026, lot B.4 ; décisions de Joëlle (21/10/2026) : vues SQL ;
-- dossier dominant = dossier vivant d'abord, puis celui dont une séance est la
-- plus proche d'aujourd'hui.
--
-- 1. v_seances_solde : pour chaque séance, le réglé (paiements PAYE), le caractère
--    dû et facturable, le reste dû, « échue » (passée) et l'avoir. Le reste dû
--    était écrit cinq fois avec des résultats différents : une absence non
--    facturable (domaine sans facturation des absences) gardait un dû en attente
--    et était proposée à l'encaissement.
-- 2. v_dossier_dominant : un dossier par patient, celui que l'on montre comme
--    son suivi (statut et domaine de la grille Patients, aperçu, fiche). Il y en
--    avait trois définitions.
--
-- Règles :
-- - SQL SECURITY INVOKER : la vue s'exécute avec les droits du compte qui la lit
--   (le compte de l'application a déjà SELECT et SHOW VIEW sur la base).
-- - Codes écrits ici comme dans les requêtes existantes : 'PAYE', 'ABSENTE'
--   (absence non excusée ; drapeau prévu au lot B.6), 'ACTIF' / 'PAUSE'.
-- - « Échue » : fin de la séance passée (à défaut le début ; sans heure, à partir
--   du lendemain) — même règle que SeanceListeItem.EstPassee.
-- - Structure seulement (aucune donnée modifiée) ; script rejouable (CREATE OR
--   REPLACE). Retour arrière : migration_v2.49.0_vues_soldes_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

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
                  AND NOT (COALESCE(dom.no_show_facturable, 1) = 0 AND ss.code_statut_seance = 'ABSENTE')
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

CREATE OR REPLACE SQL SECURITY INVOKER VIEW v_dossier_dominant AS
SELECT x.id_patient, x.id_dossier
FROM (
    SELECT
        d.id_patient,
        d.id_dossier,
        ROW_NUMBER() OVER (
            PARTITION BY d.id_patient
            ORDER BY
                CASE WHEN sd.code_statut_dossier IN ('ACTIF', 'PAUSE') THEN 0 ELSE 1 END,
                COALESCE(ps.ecart_jours, 999999),
                d.date_ouverture DESC,
                d.id_dossier DESC
        ) AS rang
    FROM dossiers d
    LEFT JOIN ref_statuts_dossier sd ON sd.id_statut_dossier = d.id_statut_dossier
    LEFT JOIN (
        SELECT s.id_dossier, MIN(ABS(DATEDIFF(s.date_seance, CURDATE()))) AS ecart_jours
        FROM seances s
        GROUP BY s.id_dossier
    ) ps ON ps.id_dossier = d.id_dossier
) x
WHERE x.rang = 1;

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.49.0',
       'Vues v_seances_solde et v_dossier_dominant (une seule definition du solde et du dossier dominant).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.49.0');
