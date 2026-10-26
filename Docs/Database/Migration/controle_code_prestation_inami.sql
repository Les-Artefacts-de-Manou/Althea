-- =================================================================================================
-- Script     : controle_code_prestation_inami.sql
-- Version    : V1.0.0
-- Date       : 19/09/2026
--
-- Rôle       :
-- Script de CONTRÔLE (lecture seule) du champ ref_natures_seance.code_prestation_inami.
--
-- Contexte   :
-- Avant le correctif de UC_NaturesSeance, l'écran de modification d'une nature de séance ne
-- reprenait pas le champ code_prestation_inami : toute modification d'une nature (libellé,
-- ordre, état) remettait donc ce code à NULL sans aucun avertissement.
-- Or ce code est le discriminant utilisé par GestionQuotaInami pour compter les séances
-- SOUTIEN et TRAITEMENT. Une valeur perdue fausse silencieusement le décompte des quotas.
--
-- Remarques  :
-- - AUCUNE écriture : ce script ne fait que des SELECT. Il peut être exécuté sans risque.
-- - Les requêtes de correction sont fournies en commentaire en fin de fichier, à n'exécuter
--   qu'après avoir constaté et compris les anomalies.
-- =================================================================================================

-- -------------------------------------------------------------------------------------------------
-- 1. Photographie complète du référentiel des natures de séance
--    Permet de vérifier d'un coup d'oeil quelles natures portent un code de prestation.
-- -------------------------------------------------------------------------------------------------
SELECT
    id_nature_seance,
    code_nature_seance,
    libelle_nature_seance,
    code_prestation_inami,
    CASE
        WHEN code_prestation_inami IS NULL THEN 'Hors quota INAMI'
        ELSE CONCAT('Quota : ', code_prestation_inami)
    END AS interpretation,
    actif,
    ordre_affichage
FROM ref_natures_seance
ORDER BY ordre_affichage, libelle_nature_seance;


-- -------------------------------------------------------------------------------------------------
-- 2. Natures sans code de prestation mais DÉJÀ UTILISÉES par des séances
--    C'est le cas le plus suspect : des séances existent, mais elles ne sont comptées dans
--    aucun quota. Si l'une de ces natures aurait dû valoir SOUTIEN ou TRAITEMENT, le quota
--    affiché est sous-évalué.
-- -------------------------------------------------------------------------------------------------
SELECT
    n.id_nature_seance,
    n.code_nature_seance,
    n.libelle_nature_seance,
    COUNT(s.id_seance)  AS nb_seances,
    MIN(s.date_seance)  AS premiere_seance,
    MAX(s.date_seance)  AS derniere_seance
FROM ref_natures_seance AS n
INNER JOIN seances AS s
    ON s.id_nature_seance = n.id_nature_seance
WHERE n.code_prestation_inami IS NULL
   OR TRIM(n.code_prestation_inami) = ''
GROUP BY
    n.id_nature_seance,
    n.code_nature_seance,
    n.libelle_nature_seance
ORDER BY nb_seances DESC;


-- -------------------------------------------------------------------------------------------------
-- 3. Valeurs non conformes
--    Seules trois valeurs sont admises : NULL, 'SOUTIEN' et 'TRAITEMENT'.
--    Tout autre contenu (casse différente, espaces, ancien libellé) empêche la reconnaissance
--    du quota par GestionQuotaInami.
-- -------------------------------------------------------------------------------------------------
SELECT
    id_nature_seance,
    code_nature_seance,
    libelle_nature_seance,
    CONCAT('[', code_prestation_inami, ']') AS valeur_brute,
    LENGTH(code_prestation_inami)           AS longueur
FROM ref_natures_seance
WHERE code_prestation_inami IS NOT NULL
  AND UPPER(TRIM(code_prestation_inami)) NOT IN ('SOUTIEN', 'TRAITEMENT')
ORDER BY libelle_nature_seance;


-- -------------------------------------------------------------------------------------------------
-- 4. Répartition réelle des séances par code de prestation
--    Vue de synthèse : compare ce total au quota attendu pour détecter une anomalie globale.
-- -------------------------------------------------------------------------------------------------
SELECT
    COALESCE(n.code_prestation_inami, '(hors quota)') AS code_prestation,
    COUNT(s.id_seance)                                AS nb_seances,
    COUNT(DISTINCT s.id_dossier)                      AS nb_dossiers
FROM seances AS s
INNER JOIN ref_natures_seance AS n
    ON n.id_nature_seance = s.id_nature_seance
GROUP BY COALESCE(n.code_prestation_inami, '(hors quota)')
ORDER BY nb_seances DESC;


-- =================================================================================================
-- CORRECTION MANUELLE (à n'exécuter qu'après analyse des résultats ci-dessus)
-- =================================================================================================
-- Ne décommenter qu'après avoir identifié précisément la ou les natures concernées, et après
-- avoir réalisé une sauvegarde. Remplacer l'identifiant et la valeur par les vôtres.
--
-- UPDATE ref_natures_seance
--    SET code_prestation_inami = 'SOUTIEN'      -- ou 'TRAITEMENT', ou NULL pour hors quota
--  WHERE id_nature_seance = <id_a_corriger>;
--
-- Normalisation de la casse et des espaces parasites (sans risque sur les valeurs valides) :
--
-- UPDATE ref_natures_seance
--    SET code_prestation_inami = UPPER(TRIM(code_prestation_inami))
--  WHERE code_prestation_inami IS NOT NULL
--    AND code_prestation_inami <> UPPER(TRIM(code_prestation_inami));
-- =================================================================================================
