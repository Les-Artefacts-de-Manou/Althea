-- =============================================================================
-- ALTHÉA — Migration de schéma v2.0.0 (Lot 0)
-- =============================================================================
-- Décisions appliquées :
--   D-16 : renommage ref_volets → ref_domaines
--   D-16 : renommage medecins  → therapeutes
--   D-16 : création ref_roles_intervenant
--   D-16 : enrichissement autres_suivis_patient (liaison N-N réseau d'intervenants)
--   D-17 : ref_statuts_seance déjà en place (aucune action)
--
-- Ordre d'exécution imposé par InnoDB :
--   1. Créer ref_roles_intervenant (nouveau référentiel)
--   2. Renommer ref_volets → ref_domaines  + répercuter FKs
--   3. Renommer medecins  → therapeutes    + répercuter FKs + séquence
--   4. Enrichir autres_suivis_patient
--   5. Versionner dans tec_meta_schema
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.0.0_lot0_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Création de ref_roles_intervenant
-- Référentiel des rôles possibles pour un intervenant externe (réseau de suivi)
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_roles_intervenant`;
CREATE TABLE `ref_roles_intervenant` (
  `id_role_intervenant`     bigint(20) unsigned NOT NULL,
  `code_role_intervenant`   varchar(30)         NOT NULL,
  `libelle_role_intervenant` varchar(100)        NOT NULL,
  `actif`                   tinyint(1)           NOT NULL DEFAULT 1,
  `ordre_affichage`         int(11)              NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_role_intervenant`),
  UNIQUE KEY `uq_ref_roles_intervenant_code`    (`code_role_intervenant`),
  UNIQUE KEY `uq_ref_roles_intervenant_libelle` (`libelle_role_intervenant`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- Données initiales
SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
LOCK TABLES `ref_roles_intervenant` WRITE;
INSERT INTO `ref_roles_intervenant` VALUES
  (1,  'MEDECIN_TRAITANT',   'Médecin traitant',          1, 10),
  (2,  'PSYCHIATRE',         'Psychiatre',                1, 20),
  (3,  'PSYCHOLOGUE',        'Psychologue',               1, 30),
  (4,  'ORTHOPHONISTE',      'Orthophoniste',             1, 40),
  (5,  'KINESITHERAPEUTE',   'Kinésithérapeute',          1, 50),
  (6,  'ERGOTHERAPEUTE',     'Ergothérapeute',            1, 60),
  (7,  'LOGOPEDE',           'Logopède',                  1, 70),
  (8,  'NEUROPEDIATRE',      'Neuropédiatre',             1, 80),
  (9,  'ASSISTANT_SOCIAL',   'Assistant(e) social(e)',    1, 90),
  (10, 'ENSEIGNANT',         'Enseignant(e)',              1, 100),
  (11, 'AUTRE',              'Autre intervenant',         1, 200);
UNLOCK TABLES;
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 2 — ref_volets → ref_domaines
-- ---------------------------------------------------------------------------

-- 2a. Supprimer les FKs qui référencent ref_volets
ALTER TABLE `dossiers`          DROP FOREIGN KEY `fk_dossiers_ref_volets`;
ALTER TABLE `modeles_documents` DROP FOREIGN KEY `fk_modeles_documents_ref_volets`;

-- 2b. Renommer la table
RENAME TABLE `ref_volets` TO `ref_domaines`;

-- 2c. Renommer les colonnes (ref_domaines)
ALTER TABLE `ref_domaines`
  CHANGE `id_volet`        `id_domaine`        bigint(20) unsigned NOT NULL,
  CHANGE `code_volet`      `code_domaine`      varchar(10)         NOT NULL,
  CHANGE `libelle_volet`   `libelle_domaine`   varchar(100)        NOT NULL;

-- 2d. Recréer les clés uniques sous nouveaux noms
ALTER TABLE `ref_domaines`
  DROP KEY  `uq_ref_volets_code`,
  DROP KEY  `uq_ref_volets_libelle`,
  ADD  UNIQUE KEY `uq_ref_domaines_code`    (`code_domaine`),
  ADD  UNIQUE KEY `uq_ref_domaines_libelle` (`libelle_domaine`);

-- 2e. Mettre à jour dossiers : renommer la colonne FK + l'index
ALTER TABLE `dossiers`
  DROP KEY  `fk_dossiers_ref_volets`,
  CHANGE `id_volet` `id_domaine` bigint(20) unsigned NOT NULL,
  ADD  KEY `fk_dossiers_ref_domaines` (`id_domaine`),
  ADD  CONSTRAINT `fk_dossiers_ref_domaines`
       FOREIGN KEY (`id_domaine`) REFERENCES `ref_domaines` (`id_domaine`);

-- 2f. Mettre à jour modeles_documents : renommer la colonne FK + l'index
ALTER TABLE `modeles_documents`
  DROP KEY  `fk_modeles_documents_ref_volets`,
  CHANGE `id_volet` `id_domaine` bigint(20) unsigned DEFAULT NULL,
  ADD  KEY `fk_modeles_documents_ref_domaines` (`id_domaine`),
  ADD  CONSTRAINT `fk_modeles_documents_ref_domaines`
       FOREIGN KEY (`id_domaine`) REFERENCES `ref_domaines` (`id_domaine`);


-- ---------------------------------------------------------------------------
-- SECTION 3 — medecins → therapeutes
-- ---------------------------------------------------------------------------

-- 3a. Supprimer la FK de dossiers vers medecins
ALTER TABLE `dossiers` DROP FOREIGN KEY `fk_dossiers_medecins`;

-- 3b. Renommer la table
RENAME TABLE `medecins` TO `therapeutes`;

-- 3c. La colonne générée STORED doit être droppée avant de renommer id_medecin
ALTER TABLE `therapeutes` DROP COLUMN `code_medecin`;

-- 3d. Renommer la colonne PK
ALTER TABLE `therapeutes`
  CHANGE `id_medecin` `id_therapeute` bigint(20) unsigned NOT NULL;

-- 3e. Recréer la colonne générée sous le nouveau nom et le nouveau préfixe ('TH')
ALTER TABLE `therapeutes`
  ADD COLUMN `code_therapeute` varchar(12)
    GENERATED ALWAYS AS (concat('TH', lpad(`id_therapeute`, 6, '0'))) STORED
    AFTER `id_therapeute`;

-- 3f. Recréer la clé unique sur la colonne générée
ALTER TABLE `therapeutes`
  DROP KEY  `uq_medecins_code_medecin`,
  ADD  UNIQUE KEY `uq_therapeutes_code_therapeute` (`code_therapeute`);

-- 3g. Créer la nouvelle séquence
DROP SEQUENCE IF EXISTS `seq_therapeutes`;
CREATE SEQUENCE `seq_therapeutes`
  START WITH 1 MINVALUE 1 MAXVALUE 9223372036854775806
  INCREMENT BY 1 CACHE 1 NOCYCLE ENGINE=InnoDB;

-- 3h. Remplacer la valeur par défaut (séquence) sur la PK
ALTER TABLE `therapeutes`
  MODIFY `id_therapeute` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_therapeutes`);

-- 3i. Supprimer l'ancienne séquence
DROP SEQUENCE IF EXISTS `seq_medecins`;

-- 3j. Mettre à jour dossiers : renommer la colonne FK + l'index + la contrainte
ALTER TABLE `dossiers`
  DROP KEY   `fk_dossiers_medecins`,
  CHANGE `id_medecin_traitant` `id_therapeute_traitant` bigint(20) unsigned DEFAULT NULL,
  ADD  KEY  `fk_dossiers_therapeutes` (`id_therapeute_traitant`),
  ADD  CONSTRAINT `fk_dossiers_therapeutes`
       FOREIGN KEY (`id_therapeute_traitant`) REFERENCES `therapeutes` (`id_therapeute`);


-- ---------------------------------------------------------------------------
-- SECTION 4 — Enrichissement de autres_suivis_patient
-- Passage d'une table de notes libres à une vraie liaison N-N
-- Les colonnes existantes (nom_professionnel, specialite, etc.) sont conservées
-- pour la rétrocompatibilité ; les nouvelles colonnes permettent de lier à
-- un thérapeute connu (FK nullable) et à un rôle d'intervenant.
-- ---------------------------------------------------------------------------

ALTER TABLE `autres_suivis_patient`
  ADD COLUMN `id_role_intervenant` bigint(20) unsigned DEFAULT NULL
    COMMENT 'Rôle de cet intervenant dans le réseau de suivi (ref_roles_intervenant)'
    AFTER `id_patient`,
  ADD COLUMN `id_therapeute` bigint(20) unsigned DEFAULT NULL
    COMMENT 'Thérapeute connu dans le référentiel therapeutes (nullable)'
    AFTER `id_role_intervenant`,
  ADD KEY  `fk_autres_suivis_ref_roles`   (`id_role_intervenant`),
  ADD KEY  `fk_autres_suivis_therapeutes` (`id_therapeute`),
  ADD CONSTRAINT `fk_autres_suivis_ref_roles`
       FOREIGN KEY (`id_role_intervenant`) REFERENCES `ref_roles_intervenant` (`id_role_intervenant`),
  ADD CONSTRAINT `fk_autres_suivis_therapeutes`
       FOREIGN KEY (`id_therapeute`) REFERENCES `therapeutes` (`id_therapeute`);


-- ---------------------------------------------------------------------------
-- SECTION 5 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.0.0', 'Lot 0 : ref_volets→ref_domaines, medecins→therapeutes, ref_roles_intervenant, enrichissement autres_suivis_patient', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN migration_v2.0.0_lot0.sql
-- =============================================================================
