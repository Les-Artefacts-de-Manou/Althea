-- =============================================================================
-- ALTHÉA — Rollback migration v2.0.0 (Lot 0)
-- =============================================================================
-- Annule intégralement migration_v2.0.0_lot0.sql dans l'ordre inverse.
-- À exécuter uniquement sur la base de développement.
--
-- Ordre d'exécution (inverse de la migration) :
--   5. Supprimer la ligne de version dans tec_meta_schema
--   4. Retirer les colonnes ajoutées dans autres_suivis_patient
--   3. Renommer therapeutes → medecins  + séquence + FKs
--   2. Renommer ref_domaines → ref_volets + FKs
--   1. Supprimer ref_roles_intervenant
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;


-- ---------------------------------------------------------------------------
-- SECTION 5 — Supprimer la ligne de version
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.0.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 4 — Retirer l'enrichissement de autres_suivis_patient
-- ---------------------------------------------------------------------------

ALTER TABLE `autres_suivis_patient`
  DROP FOREIGN KEY `fk_autres_suivis_ref_roles`,
  DROP FOREIGN KEY `fk_autres_suivis_therapeutes`,
  DROP KEY  `fk_autres_suivis_ref_roles`,
  DROP KEY  `fk_autres_suivis_therapeutes`,
  DROP COLUMN `id_role_intervenant`,
  DROP COLUMN `id_therapeute`;


-- ---------------------------------------------------------------------------
-- SECTION 3 — therapeutes → medecins
-- ---------------------------------------------------------------------------

-- 3a. Supprimer la FK de dossiers vers therapeutes
ALTER TABLE `dossiers` DROP FOREIGN KEY `fk_dossiers_therapeutes`;

-- 3b. Rétablir la colonne FK dans dossiers
ALTER TABLE `dossiers`
  DROP KEY  `fk_dossiers_therapeutes`,
  CHANGE `id_therapeute_traitant` `id_medecin_traitant` bigint(20) unsigned DEFAULT NULL,
  ADD  KEY `fk_dossiers_medecins` (`id_medecin_traitant`),
  ADD  CONSTRAINT `fk_dossiers_medecins`
       FOREIGN KEY (`id_medecin_traitant`) REFERENCES `medecins` (`id_medecin`);

-- 3c. Renommer la table
RENAME TABLE `therapeutes` TO `medecins`;

-- 3d. Revert la colonne générée
ALTER TABLE `medecins` DROP COLUMN `code_therapeute`;
ALTER TABLE `medecins`
  CHANGE `id_therapeute` `id_medecin` bigint(20) unsigned NOT NULL;

-- 3e. Recréer la séquence d'origine
DROP SEQUENCE IF EXISTS `seq_medecins`;
CREATE SEQUENCE `seq_medecins`
  START WITH 1 MINVALUE 1 MAXVALUE 9223372036854775806
  INCREMENT BY 1 CACHE 1 NOCYCLE ENGINE=InnoDB;

-- 3f. Rétablir la valeur par défaut sur la PK
ALTER TABLE `medecins`
  MODIFY `id_medecin` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_medecins`);

-- 3g. Recréer la colonne générée originale
ALTER TABLE `medecins`
  ADD COLUMN `code_medecin` varchar(12)
    GENERATED ALWAYS AS (concat('ME', lpad(`id_medecin`, 6, '0'))) STORED
    AFTER `id_medecin`;

-- 3h. Recréer la clé unique originale
ALTER TABLE `medecins`
  ADD UNIQUE KEY `uq_medecins_code_medecin` (`code_medecin`);

-- 3i. Supprimer la nouvelle séquence
DROP SEQUENCE IF EXISTS `seq_therapeutes`;


-- ---------------------------------------------------------------------------
-- SECTION 2 — ref_domaines → ref_volets
-- ---------------------------------------------------------------------------

-- 2a. Supprimer les FKs qui référencent ref_domaines
ALTER TABLE `dossiers`          DROP FOREIGN KEY `fk_dossiers_ref_domaines`;
ALTER TABLE `modeles_documents` DROP FOREIGN KEY `fk_modeles_documents_ref_domaines`;

-- 2b. Rétablir les colonnes FK dans dossiers
ALTER TABLE `dossiers`
  DROP KEY  `fk_dossiers_ref_domaines`,
  CHANGE `id_domaine` `id_volet` bigint(20) unsigned NOT NULL,
  ADD  KEY `fk_dossiers_ref_volets` (`id_volet`),
  ADD  CONSTRAINT `fk_dossiers_ref_volets`
       FOREIGN KEY (`id_volet`) REFERENCES `ref_volets` (`id_volet`);

-- 2c. Rétablir les colonnes FK dans modeles_documents
ALTER TABLE `modeles_documents`
  DROP KEY  `fk_modeles_documents_ref_domaines`,
  CHANGE `id_domaine` `id_volet` bigint(20) unsigned DEFAULT NULL,
  ADD  KEY `fk_modeles_documents_ref_volets` (`id_volet`),
  ADD  CONSTRAINT `fk_modeles_documents_ref_volets`
       FOREIGN KEY (`id_volet`) REFERENCES `ref_volets` (`id_volet`);

-- 2d. Renommer la table
RENAME TABLE `ref_domaines` TO `ref_volets`;

-- 2e. Renommer les colonnes (ref_volets)
ALTER TABLE `ref_volets`
  CHANGE `id_domaine`      `id_volet`      bigint(20) unsigned NOT NULL,
  CHANGE `code_domaine`    `code_volet`    varchar(10)         NOT NULL,
  CHANGE `libelle_domaine` `libelle_volet` varchar(100)        NOT NULL;

-- 2f. Recréer les clés uniques originales
ALTER TABLE `ref_volets`
  DROP KEY  `uq_ref_domaines_code`,
  DROP KEY  `uq_ref_domaines_libelle`,
  ADD  UNIQUE KEY `uq_ref_volets_code`    (`code_volet`),
  ADD  UNIQUE KEY `uq_ref_volets_libelle` (`libelle_volet`);


-- ---------------------------------------------------------------------------
-- SECTION 1 — Supprimer ref_roles_intervenant
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_roles_intervenant`;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN migration_v2.0.0_lot0_ROLLBACK.sql
-- =============================================================================
