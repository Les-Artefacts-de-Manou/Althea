-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.53.0 - absence non excusée réglée par une case
--
-- La vue v_seances_solde reprend sa définition v2.49.0 (code 'ABSENTE'), puis la
-- colonne est retirée. Un statut coché « absence » autre qu'ABSENTE redevient un
-- statut ordinaire pour l'application d'avant.
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

ALTER TABLE `ref_statuts_seance` DROP COLUMN IF EXISTS `est_absence`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.53.0';
