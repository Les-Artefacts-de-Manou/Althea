-- =============================================================================
-- ALTHÉA — Migration de schéma v2.2.0 (Lot 2 — Rôle légal des contacts)
-- =============================================================================
-- Décisions appliquées :
--   D-Q18 (BD-14) : famille_contacts — les 4 indicateurs booléens cumulables
--                   (autorite_parentale, representant_legal, personne_autorisee,
--                   contact_urgence) sont remplacés par un référentiel mono-valeur
--                   ref_role_legal lié par une FK obligatoire id_role_legal.
--
-- Justification : un contact possède un seul rôle légal (« soit l'autorité,
-- soit un simple contact d'urgence »). Le modèle cumulable n'est plus pertinent.
--
-- Ordre d'exécution :
--   1. ref_role_legal      : création du référentiel + jeu de données initial
--   2. famille_contacts    : ajout id_role_legal, reprise des 4 booléens,
--                            passage NOT NULL, FK, puis suppression des booléens
--   3. Versionner dans tec_meta_schema
--
-- Règle de reprise (cumulable -> mono-valeur), priorité décroissante :
--   autorite_parentale > representant_legal > personne_autorisee > contact_urgence
--   Repli : Contact d'urgence si aucun booléen n'était positionné (garantit NOT NULL).
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.2.0_lot2_role_legal_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Création de ref_role_legal
-- Référentiel des rôles légaux d'un contact (mono-valeur par contact).
-- Table créée directement en AUTO_INCREMENT (cohérent avec l'état post-v2.0.1).
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_role_legal`;
CREATE TABLE `ref_role_legal` (
  `id_role_legal`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_role_legal`    varchar(30)         NOT NULL,
  `libelle_role_legal` varchar(100)        NOT NULL,
  `actif`              tinyint(1)          NOT NULL DEFAULT 1,
  `ordre_affichage`    int(11)             NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_role_legal`),
  UNIQUE KEY `uq_ref_role_legal_code`    (`code_role_legal`),
  UNIQUE KEY `uq_ref_role_legal_libelle` (`libelle_role_legal`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- Données initiales (reprise des 4 anciens indicateurs)
SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
LOCK TABLES `ref_role_legal` WRITE;
INSERT INTO `ref_role_legal` VALUES
  (1, 'AUTORITE_PARENTALE', 'Autorité parentale', 1, 10),
  (2, 'REPRESENTANT_LEGAL', 'Représentant légal', 1, 20),
  (3, 'PERSONNE_AUTORISEE', 'Personne autorisée', 1, 30),
  (4, 'CONTACT_URGENCE',    'Contact d''urgence', 1, 40);
UNLOCK TABLES;
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 2 — famille_contacts : bascule vers id_role_legal (BD-14)
-- ---------------------------------------------------------------------------

-- 2a. Ajouter la colonne FK (temporairement nullable pour permettre la reprise)
ALTER TABLE `famille_contacts`
  ADD COLUMN `id_role_legal` bigint(20) unsigned DEFAULT NULL
    COMMENT 'Rôle légal unique du contact (ref_role_legal) — D-Q18'
    AFTER `pays`;

-- 2b. Reprise des données : 4 booléens cumulables -> rôle unique (priorité décroissante)
UPDATE `famille_contacts`
   SET `id_role_legal` = CASE
        WHEN `autorite_parentale` = 1 THEN 1
        WHEN `representant_legal`  = 1 THEN 2
        WHEN `personne_autorisee`  = 1 THEN 3
        WHEN `contact_urgence`     = 1 THEN 4
        ELSE 4
   END;

-- 2c. Rendre la colonne obligatoire (FK NOT NULL)
ALTER TABLE `famille_contacts`
  MODIFY COLUMN `id_role_legal` bigint(20) unsigned NOT NULL
    COMMENT 'Rôle légal unique du contact (ref_role_legal) — D-Q18';

-- 2d. Index + contrainte de clé étrangère
ALTER TABLE `famille_contacts`
  ADD KEY `fk_famille_contacts_ref_role_legal` (`id_role_legal`),
  ADD CONSTRAINT `fk_famille_contacts_ref_role_legal`
       FOREIGN KEY (`id_role_legal`) REFERENCES `ref_role_legal` (`id_role_legal`);

-- 2e. Supprimer les anciens indicateurs booléens cumulables
ALTER TABLE `famille_contacts`
  DROP COLUMN `autorite_parentale`,
  DROP COLUMN `representant_legal`,
  DROP COLUMN `personne_autorisee`,
  DROP COLUMN `contact_urgence`;


-- ---------------------------------------------------------------------------
-- SECTION 3 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.2.0', 'Lot 2 contacts : ref_role_legal (référentiel) + famille_contacts.id_role_legal (FK NOT NULL) en remplacement des 4 booléens cumulables (BD-14)', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN migration_v2.2.0_lot2_role_legal.sql
-- =============================================================================
