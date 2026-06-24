-- =============================================================================
-- ALTHÉA — Rollback migration v2.2.0 (Lot 2 — Rôle légal des contacts)
-- =============================================================================
-- Annule intégralement migration_v2.2.0_lot2_role_legal.sql dans l'ordre inverse.
-- À exécuter uniquement sur la base de développement.
--
-- Ordre d'exécution (inverse de la migration) :
--   3. Supprimer la ligne de version dans tec_meta_schema
--   2. Rétablir les 4 booléens de famille_contacts (reprise id_role_legal -> booléen),
--      puis retirer la FK et la colonne id_role_legal
--   1. Supprimer la table ref_role_legal
--
-- Remarque : le modèle d'origine était cumulable (plusieurs booléens possibles) ;
-- la migration ayant réduit l'information à un rôle unique, le rollback ne peut
-- restituer qu'un seul booléen par contact (les éventuels cumuls antérieurs
-- sont définitivement perdus). Repli : aucun booléen positionné si le rôle est
-- inconnu (ne devrait pas arriver après une migration normale).
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;


-- ---------------------------------------------------------------------------
-- SECTION 3 — Supprimer la ligne de version
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.2.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 2 — Rétablir les 4 booléens de famille_contacts (BD-14)
-- ---------------------------------------------------------------------------

-- 2a. Retirer la contrainte FK et son index
ALTER TABLE `famille_contacts`
  DROP FOREIGN KEY `fk_famille_contacts_ref_role_legal`,
  DROP KEY `fk_famille_contacts_ref_role_legal`;

-- 2b. Recréer les anciens indicateurs booléens cumulables
ALTER TABLE `famille_contacts`
  ADD COLUMN `autorite_parentale` tinyint(1) NOT NULL DEFAULT 0 AFTER `pays`,
  ADD COLUMN `representant_legal`  tinyint(1) NOT NULL DEFAULT 0 AFTER `autorite_parentale`,
  ADD COLUMN `personne_autorisee`  tinyint(1) NOT NULL DEFAULT 0 AFTER `representant_legal`,
  ADD COLUMN `contact_urgence`     tinyint(1) NOT NULL DEFAULT 0 AFTER `personne_autorisee`;

-- 2c. Reprise des données : rôle unique -> booléen correspondant
UPDATE `famille_contacts`
   SET `autorite_parentale` = CASE WHEN `id_role_legal` = 1 THEN 1 ELSE 0 END,
       `representant_legal`  = CASE WHEN `id_role_legal` = 2 THEN 1 ELSE 0 END,
       `personne_autorisee`  = CASE WHEN `id_role_legal` = 3 THEN 1 ELSE 0 END,
       `contact_urgence`     = CASE WHEN `id_role_legal` = 4 THEN 1 ELSE 0 END;

-- 2d. Supprimer la colonne FK
ALTER TABLE `famille_contacts`
  DROP COLUMN `id_role_legal`;


-- ---------------------------------------------------------------------------
-- SECTION 1 — Supprimer le référentiel ref_role_legal
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_role_legal`;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN migration_v2.2.0_lot2_role_legal_ROLLBACK.sql
-- =============================================================================
