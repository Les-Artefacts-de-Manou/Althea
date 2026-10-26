-- =================================================================================================
-- ROLLBACK : v2.16.0 - Lot 15 - Suppression du drapeau est_inami
-- Date     : 19/09/2026
--
-- Objet :
-- Restaurer ref_domaines.est_inami et le réalimenter à partir des colonnes de comportement.
--
-- Remarque :
-- La reconstitution se fait depuis soumis_quota, qui correspond au périmètre historique du
-- drapeau. Si des domaines ont été paramétrés finement depuis le lot 14, la valeur restaurée est
-- une approximation : ce rollback ne doit servir qu'en cas de retour arrière immédiat.
-- =================================================================================================

-- -------------------------------------------------------------------------------------------------
-- 1. Restauration de la colonne
-- -------------------------------------------------------------------------------------------------
ALTER TABLE ref_domaines
    ADD COLUMN est_inami TINYINT(1) NOT NULL DEFAULT 0
        COMMENT 'OBSOLETE - restaure par rollback v2.16.0' AFTER libelle_domaine;

-- -------------------------------------------------------------------------------------------------
-- 2. Réalimentation depuis le comportement de quota
-- -------------------------------------------------------------------------------------------------
UPDATE ref_domaines
SET est_inami = 1
WHERE soumis_quota = 1;

-- -------------------------------------------------------------------------------------------------
-- 3. Retrait de la version
-- -------------------------------------------------------------------------------------------------
DELETE FROM tec_meta_schema
WHERE version_schema = '2.16.0';
