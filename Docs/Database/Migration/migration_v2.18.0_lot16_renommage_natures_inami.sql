-- =================================================================================================
-- Migration : v2.18.0 - Lot 16 - Harmonisation des codes de natures de séance INAMI
-- Date      : 19/09/2026
--
-- Objet :
-- Renommer les deux natures de séance conventionnées afin que leur code et leur libellé
-- commencent par le préfixe INAMI :
--   SOUTIEN_INAMI    -> INAMI_SOUTIEN     / « INAMI - Soutien »
--   TRAITEMENT_INAMI -> INAMI_TRAITEMENT  / « INAMI - Traitement »
--
-- Motivation :
-- Dans les listes déroulantes de saisie de séance, « Traitement » et « Traitement (INAMI) »
-- se ressemblent trop : les deux libellés commencent par le même mot et ne se distinguent
-- qu'en fin de chaîne. Une sélection erronée sort silencieusement la séance du quota INAMI
-- sans aucun message d'erreur, car les deux natures sont parfaitement valides.
-- En plaçant le discriminant INAMI EN TÊTE du code et du libellé, les natures conventionnées
-- se regroupent visuellement et deviennent impossibles à confondre avec leurs homologues
-- hors convention.
--
-- Portée :
-- Cette migration ne touche QUE les colonnes d'affichage (code et libellé).
-- Les identifiants, le champ code_prestation_inami et toutes les séances rattachées sont
-- inchangés : aucun impact sur le décompte des quotas.
--
-- Prérequis :
-- - Migration v2.13.0 (lot 12) appliquée : les natures INAMI existent.
--
-- Contrôle de sécurité :
-- Les renommages ne sont appliqués que si le code cible n'est pas déjà utilisé, afin de ne
-- jamais violer les contraintes uq_ref_natures_seance_code et uq_ref_natures_seance_libelle.
-- Le script est donc rejouable sans erreur.
-- =================================================================================================

-- -------------------------------------------------------------------------------------------------
-- 1. Contrôle : les codes cibles doivent être libres
-- -------------------------------------------------------------------------------------------------
SET @nb_conflits = (
    SELECT COUNT(*)
    FROM ref_natures_seance
    WHERE code_nature_seance IN ('INAMI_SOUTIEN', 'INAMI_TRAITEMENT')
       OR libelle_nature_seance IN ('INAMI - Soutien', 'INAMI - Traitement')
);

SELECT IF(
    @nb_conflits = 0,
    'OK : les codes et libelles cibles sont libres.',
    'INFORMATION : renommage deja applique ou codes cibles deja utilises, aucune modification ne sera faite.'
) AS controle_prerequis;

-- -------------------------------------------------------------------------------------------------
-- 2. Renommage de la nature « Soutien (INAMI) »
-- -------------------------------------------------------------------------------------------------
UPDATE ref_natures_seance
   SET code_nature_seance    = 'INAMI_SOUTIEN',
       libelle_nature_seance = 'INAMI - Soutien'
 WHERE code_nature_seance = 'SOUTIEN_INAMI'
   AND @nb_conflits = 0;

-- -------------------------------------------------------------------------------------------------
-- 3. Renommage de la nature « Traitement (INAMI) »
-- -------------------------------------------------------------------------------------------------
UPDATE ref_natures_seance
   SET code_nature_seance    = 'INAMI_TRAITEMENT',
       libelle_nature_seance = 'INAMI - Traitement'
 WHERE code_nature_seance = 'TRAITEMENT_INAMI'
   AND @nb_conflits = 0;

-- -------------------------------------------------------------------------------------------------
-- 4. Vérification du résultat
--    Le code_prestation_inami doit être resté intact : c'est lui qui pilote les quotas.
-- -------------------------------------------------------------------------------------------------
SELECT
    id_nature_seance,
    code_nature_seance,
    libelle_nature_seance,
    code_prestation_inami,
    actif,
    ordre_affichage
FROM ref_natures_seance
ORDER BY ordre_affichage, libelle_nature_seance;

-- -------------------------------------------------------------------------------------------------
-- 5. Journalisation de la version
-- -------------------------------------------------------------------------------------------------
INSERT INTO tec_meta_schema (version_schema, description, date_application)
VALUES (
    '2.18.0',
    'Lot 16 : renommage des natures de seance conventionnees en INAMI_SOUTIEN et INAMI_TRAITEMENT pour eviter toute confusion a la saisie avec les natures hors convention.',
    NOW()
);
