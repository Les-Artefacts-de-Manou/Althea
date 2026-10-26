-- =================================================================================================
-- Migration : v2.16.0 - Lot 15 - Suppression du drapeau est_inami
-- Date      : 19/09/2026
--
-- Objet :
-- Retirer ref_domaines.est_inami, devenu inutilisé après le lot 14.
--
-- Motivation :
-- 1. Ce drapeau gouvernait à lui seul trois règles distinctes (tarif, no-show, quota), ce qui
--    interdisait d'exprimer un domaine conventionné aux règles partiellement différentes. Ces
--    règles sont désormais portées par tarif_convention, no_show_facturable et soumis_quota.
-- 2. Son nom désignait un organisme particulier (INAMI) alors que d'autres conventions peuvent
--    régir d'autres domaines. Le schéma ne doit nommer aucun organisme.
--
-- Prérequis :
-- - Migration v2.15.0 (lot 14) appliquée, colonnes de comportement initialisées.
-- - Plus aucun code applicatif ne lit est_inami (vérifié : requêtes séances, patients, tarification).
--
-- Contrôle de sécurité :
-- La suppression n'est faite que si les colonnes de remplacement existent bien.
-- =================================================================================================

-- -------------------------------------------------------------------------------------------------
-- 1. Contrôle : les colonnes de comportement doivent exister avant toute suppression
-- -------------------------------------------------------------------------------------------------
SET @nb_colonnes = (
    SELECT COUNT(*)
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'ref_domaines'
      AND COLUMN_NAME IN ('tarif_convention', 'no_show_facturable', 'soumis_quota')
);

SET @msg = 'ABANDON : les colonnes de comportement du lot 14 sont absentes, appliquez d abord la migration v2.15.0.';

SELECT IF(@nb_colonnes = 3, 'OK', @msg) AS controle_prerequis;

-- -------------------------------------------------------------------------------------------------
-- 2. Suppression du drapeau obsolète
-- -------------------------------------------------------------------------------------------------
SET @sql = IF(
    @nb_colonnes = 3,
    'ALTER TABLE ref_domaines DROP COLUMN est_inami',
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
    '2.16.0',
    'Lot 15 : suppression de ref_domaines.est_inami, remplace par tarif_convention, no_show_facturable et soumis_quota.',
    NOW()
);
