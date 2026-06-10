-- =================================================================================================
-- Migration  : v2.0.1 — AUTO_INCREMENT sur toutes les tables ref_*
-- Projet     : Althéa
-- Date       : 2026-06-10
-- Auteur     : Joëlle (Manou) / Projet Althéa
--
-- Contexte   :
-- Les tables ref_* ont été créées avec un id bigint NOT NULL sans AUTO_INCREMENT.
-- Les données existantes ont été insérées avec des id manuels.
-- Cette migration ajoute AUTO_INCREMENT sur chaque PK, avec un AUTO_INCREMENT
-- positionné juste après le MAX(id) actuel, pour ne pas écraser les données.
--
-- Tables concernées (9) :
--   ref_domaines, ref_liens_patient, ref_roles_intervenant,
--   ref_situations_familiales, ref_statuts_dossier, ref_statuts_seance,
--   ref_types_documents, ref_types_rendez_vous, ref_types_seance
--
-- Rollback   : migration_v2.0.1_autoincrement_refs_ROLLBACK.sql
-- =================================================================================================

-- -----------------------------------------------------------------------------
-- Précaution : désactiver les FK pendant la modification de structure
-- -----------------------------------------------------------------------------
SET FOREIGN_KEY_CHECKS = 0;

-- -----------------------------------------------------------------------------
-- ref_domaines (MAX id = 4 → AUTO_INCREMENT = 5)
-- -----------------------------------------------------------------------------
ALTER TABLE `ref_domaines`
    MODIFY COLUMN `id_domaine` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    AUTO_INCREMENT = 5;

-- -----------------------------------------------------------------------------
-- ref_liens_patient (MAX id = 11 → AUTO_INCREMENT = 12)
-- -----------------------------------------------------------------------------
ALTER TABLE `ref_liens_patient`
    MODIFY COLUMN `id_lien_patient` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    AUTO_INCREMENT = 12;

-- -----------------------------------------------------------------------------
-- ref_roles_intervenant (MAX id = 11 → AUTO_INCREMENT = 12)
-- -----------------------------------------------------------------------------
ALTER TABLE `ref_roles_intervenant`
    MODIFY COLUMN `id_role_intervenant` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    AUTO_INCREMENT = 12;

-- -----------------------------------------------------------------------------
-- ref_situations_familiales (MAX id = 9 → AUTO_INCREMENT = 10)
-- -----------------------------------------------------------------------------
ALTER TABLE `ref_situations_familiales`
    MODIFY COLUMN `id_situation_familiale` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    AUTO_INCREMENT = 10;

-- -----------------------------------------------------------------------------
-- ref_statuts_dossier (MAX id = 4 → AUTO_INCREMENT = 5)
-- -----------------------------------------------------------------------------
ALTER TABLE `ref_statuts_dossier`
    MODIFY COLUMN `id_statut_dossier` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    AUTO_INCREMENT = 5;

-- -----------------------------------------------------------------------------
-- ref_statuts_seance (MAX id = 5 → AUTO_INCREMENT = 6)
-- -----------------------------------------------------------------------------
ALTER TABLE `ref_statuts_seance`
    MODIFY COLUMN `id_statut_seance` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    AUTO_INCREMENT = 6;

-- -----------------------------------------------------------------------------
-- ref_types_documents (MAX id = 7 → AUTO_INCREMENT = 8)
-- -----------------------------------------------------------------------------
ALTER TABLE `ref_types_documents`
    MODIFY COLUMN `id_type_document` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    AUTO_INCREMENT = 8;

-- -----------------------------------------------------------------------------
-- ref_types_rendez_vous (MAX id = 5 → AUTO_INCREMENT = 6)
-- -----------------------------------------------------------------------------
ALTER TABLE `ref_types_rendez_vous`
    MODIFY COLUMN `id_type_rendez_vous` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    AUTO_INCREMENT = 6;

-- -----------------------------------------------------------------------------
-- ref_types_seance (MAX id = 5 → AUTO_INCREMENT = 6)
-- -----------------------------------------------------------------------------
ALTER TABLE `ref_types_seance`
    MODIFY COLUMN `id_type_seance` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    AUTO_INCREMENT = 6;

-- -----------------------------------------------------------------------------
-- Réactiver les FK
-- -----------------------------------------------------------------------------
SET FOREIGN_KEY_CHECKS = 1;

-- -----------------------------------------------------------------------------
-- Mise à jour de la version du schéma
-- -----------------------------------------------------------------------------
INSERT INTO `tec_meta_schema`
    (`version_schema`, `description`)
VALUES
    ('2.0.1', 'AUTO_INCREMENT sur toutes les tables ref_*');

-- =================================================================================================
-- FIN DE LA MIGRATION v2.0.1
-- Vérification recommandée :
--   SHOW CREATE TABLE ref_domaines;
--   SHOW CREATE TABLE ref_liens_patient;
--   (etc. pour chaque table)
-- =================================================================================================
