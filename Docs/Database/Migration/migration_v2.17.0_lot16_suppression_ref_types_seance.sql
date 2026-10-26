-- =================================================================================================
-- Migration : v2.17.0 - Lot 16 - Suppression de la table ref_types_seance
-- Date      : 19/09/2026
--
-- Objet :
-- Retirer la table référentielle ref_types_seance, devenue orpheline après le lot 10b.
--
-- Motivation :
-- 1. La migration v2.11.0 (lot 10b) a supprimé seances.id_type_seance et sa clé étrangère.
--    Depuis, plus aucune table ne référence ref_types_seance : elle n'est plus alimentée
--    ni consultée par le flux séance.
-- 2. La notion a été scindée en deux concepts distincts et mieux nommés :
--    - ref_natures_seance : la nature de la prestation (avec son code de prestation éventuel) ;
--    - ref_prix_seance + lia_prix_seance_domaines : le tarif, désormais historisé par période,
--      ce qui remplace l'ancien champ figé ref_types_seance.tarif_defaut.
-- 3. Conserver les deux référentiels côte à côte prête à confusion lors des évolutions futures :
--    un développeur pourrait rattacher une règle à la mauvaise table.
--
-- Prérequis :
-- - Migration v2.11.0 (lot 10b) appliquée : seances.id_type_seance supprimée.
-- - Code applicatif nettoyé : UC_TypesSeance, GestionTypesSeance, QueryTypesSeance et TypeSeance
--   ont été supprimés, ainsi que la tuile correspondante de l'accueil des référentiels.
--
-- Contrôle de sécurité :
-- La suppression n'est faite que si aucune clé étrangère ne pointe encore vers la table.
-- =================================================================================================

-- -------------------------------------------------------------------------------------------------
-- 1. Contrôle : aucune dépendance référentielle ne doit subsister
-- -------------------------------------------------------------------------------------------------
SET @nb_dependances = (
    SELECT COUNT(*)
    FROM information_schema.KEY_COLUMN_USAGE
    WHERE TABLE_SCHEMA = DATABASE()
      AND REFERENCED_TABLE_NAME = 'ref_types_seance'
);

SET @msg = 'ABANDON : des cles etrangeres pointent encore vers ref_types_seance, appliquez d abord la migration v2.11.0.';

SELECT IF(@nb_dependances = 0, 'OK', @msg) AS controle_prerequis;

-- -------------------------------------------------------------------------------------------------
-- 2. Suppression de la table orpheline
-- -------------------------------------------------------------------------------------------------
SET @sql = IF(
    @nb_dependances = 0,
    'DROP TABLE IF EXISTS ref_types_seance',
    'SELECT ''Suppression annulee : prerequis non satisfaits'' AS resultat'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- -------------------------------------------------------------------------------------------------
-- 3. Journalisation de la version
-- -------------------------------------------------------------------------------------------------
INSERT INTO tec_meta_schema (version_schema, description, date_application)
VALUES (
    '2.17.0',
    'Lot 16 : suppression de la table orpheline ref_types_seance, remplacee par ref_natures_seance et ref_prix_seance.',
    NOW()
);
