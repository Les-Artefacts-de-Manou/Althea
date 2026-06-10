-- =================================================================================================
-- Rollback    : v2.0.1 — Suppression de l'AUTO_INCREMENT sur toutes les tables ref_*
-- Projet      : Althéa
-- Date        : 2026-06-10
-- Auteur      : Joëlle (Manou) / Projet Althéa
--
-- Contexte    :
-- Annule la migration v2.0.1 en retirant l'AUTO_INCREMENT des PK des tables ref_*.
-- Les données existantes et leurs id ne sont PAS modifiés.
-- L'id redevient bigint NOT NULL sans auto-incrémentation (état d'avant la migration).
--
-- ⚠️  ATTENTION : après rollback, toute insertion en UI sera impossible sans fournir l'id.
--     N'appliquer qu'en cas de problème avéré avec la migration.
-- =================================================================================================

-- -----------------------------------------------------------------------------
-- Précaution : désactiver les FK pendant la modification de structure
-- -----------------------------------------------------------------------------
SET FOREIGN_KEY_CHECKS = 0;

ALTER TABLE `ref_domaines`
    MODIFY COLUMN `id_domaine` bigint(20) unsigned NOT NULL;

ALTER TABLE `ref_liens_patient`
    MODIFY COLUMN `id_lien_patient` bigint(20) unsigned NOT NULL;

ALTER TABLE `ref_roles_intervenant`
    MODIFY COLUMN `id_role_intervenant` bigint(20) unsigned NOT NULL;

ALTER TABLE `ref_situations_familiales`
    MODIFY COLUMN `id_situation_familiale` bigint(20) unsigned NOT NULL;

ALTER TABLE `ref_statuts_dossier`
    MODIFY COLUMN `id_statut_dossier` bigint(20) unsigned NOT NULL;

ALTER TABLE `ref_statuts_seance`
    MODIFY COLUMN `id_statut_seance` bigint(20) unsigned NOT NULL;

ALTER TABLE `ref_types_documents`
    MODIFY COLUMN `id_type_document` bigint(20) unsigned NOT NULL;

ALTER TABLE `ref_types_rendez_vous`
    MODIFY COLUMN `id_type_rendez_vous` bigint(20) unsigned NOT NULL;

ALTER TABLE `ref_types_seance`
    MODIFY COLUMN `id_type_seance` bigint(20) unsigned NOT NULL;

-- -----------------------------------------------------------------------------
-- Réactiver les FK
-- -----------------------------------------------------------------------------
SET FOREIGN_KEY_CHECKS = 1;

-- -----------------------------------------------------------------------------
-- Suppression de la ligne de version v2.0.1
-- -----------------------------------------------------------------------------
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.0.1';

-- =================================================================================================
-- FIN DU ROLLBACK v2.0.1
-- =================================================================================================
