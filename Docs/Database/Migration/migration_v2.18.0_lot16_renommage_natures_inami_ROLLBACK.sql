-- =================================================================================================
-- ROLLBACK : v2.18.0 - Lot 16 - Harmonisation des codes de natures de séance INAMI
-- Date      : 19/09/2026
--
-- Objet :
-- Rétablir les codes et libellés d'origine des deux natures de séance conventionnées :
--   INAMI_SOUTIEN    -> SOUTIEN_INAMI    / « Soutien (INAMI) »
--   INAMI_TRAITEMENT -> TRAITEMENT_INAMI / « Traitement (INAMI) »
--
-- Remarques :
-- - Aucune donnée de séance n'est affectée : seuls le code et le libellé sont restaurés.
-- - Le champ code_prestation_inami n'a jamais été modifié par la migration, il n'y a donc
--   rien à restaurer de ce côté.
-- - Le script est rejouable : les UPDATE ne font rien si les codes d'origine sont déjà en place.
-- =================================================================================================

-- -------------------------------------------------------------------------------------------------
-- 1. Contrôle : les codes d'origine doivent être libres
-- -------------------------------------------------------------------------------------------------
SET @nb_conflits = (
    SELECT COUNT(*)
    FROM ref_natures_seance
    WHERE code_nature_seance IN ('SOUTIEN_INAMI', 'TRAITEMENT_INAMI')
       OR libelle_nature_seance IN ('Soutien (INAMI)', 'Traitement (INAMI)')
);

SELECT IF(
    @nb_conflits = 0,
    'OK : les codes et libelles d origine sont libres.',
    'INFORMATION : rollback deja applique ou codes d origine deja utilises, aucune modification ne sera faite.'
) AS controle_prerequis;

-- -------------------------------------------------------------------------------------------------
-- 2. Restauration de la nature « Soutien (INAMI) »
-- -------------------------------------------------------------------------------------------------
UPDATE ref_natures_seance
   SET code_nature_seance    = 'SOUTIEN_INAMI',
       libelle_nature_seance = 'Soutien (INAMI)'
 WHERE code_nature_seance = 'INAMI_SOUTIEN'
   AND @nb_conflits = 0;

-- -------------------------------------------------------------------------------------------------
-- 3. Restauration de la nature « Traitement (INAMI) »
-- -------------------------------------------------------------------------------------------------
UPDATE ref_natures_seance
   SET code_nature_seance    = 'TRAITEMENT_INAMI',
       libelle_nature_seance = 'Traitement (INAMI)'
 WHERE code_nature_seance = 'INAMI_TRAITEMENT'
   AND @nb_conflits = 0;

-- -------------------------------------------------------------------------------------------------
-- 4. Retrait de l'entrée de version
-- -------------------------------------------------------------------------------------------------
DELETE FROM tec_meta_schema
WHERE version_schema = '2.18.0';
