-- =============================================================================
-- ALTHÉA — Rollback migration v2.1.0 (Lot 1 — Patients)
-- =============================================================================
-- Annule intégralement migration_v2.1.0_lot1_patients.sql dans l'ordre inverse.
-- À exécuter uniquement sur la base de développement.
--
-- Ordre d'exécution (inverse de la migration) :
--   5. Supprimer la ligne de version dans tec_meta_schema
--   4. Rétablir dossiers.prescripteur
--   3. Rétablir autres_suivis_patient.commentaire (reprise commentaire_txt → commentaire)
--   2. Rétablir famille_contacts.commentaire    (reprise commentaire_txt → commentaire)
--   1. Rétablir patients.alerte (reprise alerte_txt → alerte) + retirer photo_fichier
--
-- Remarque : le formatage RTF (colonnes *_rtf) est perdu au rollback ; seul
-- le contenu texte (*_txt) est restitué dans les anciennes colonnes text.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;


-- ---------------------------------------------------------------------------
-- SECTION 5 — Supprimer la ligne de version
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.1.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 4 — Rétablir dossiers.prescripteur (BD-13)
-- ---------------------------------------------------------------------------

ALTER TABLE `dossiers`
  ADD COLUMN `prescripteur` varchar(150) DEFAULT NULL
    AFTER `date_cloture`;


-- ---------------------------------------------------------------------------
-- SECTION 3 — Rétablir autres_suivis_patient.commentaire (BD-8)
-- ---------------------------------------------------------------------------

-- 3a. Recréer l'ancienne colonne text
ALTER TABLE `autres_suivis_patient`
  ADD COLUMN `commentaire` text DEFAULT NULL
    AFTER `date_fin`;

-- 3b. Reprise des données : commentaire_txt → commentaire
UPDATE `autres_suivis_patient`
   SET `commentaire` = `commentaire_txt`
 WHERE `commentaire_txt` IS NOT NULL AND `commentaire_txt` <> '';

-- 3c. Supprimer les colonnes enrichies
ALTER TABLE `autres_suivis_patient`
  DROP COLUMN `commentaire_rtf`,
  DROP COLUMN `commentaire_txt`;


-- ---------------------------------------------------------------------------
-- SECTION 2 — Rétablir famille_contacts.commentaire (BD-8)
-- ---------------------------------------------------------------------------

-- 2a. Recréer l'ancienne colonne text
ALTER TABLE `famille_contacts`
  ADD COLUMN `commentaire` text DEFAULT NULL
    AFTER `ordre_affichage`;

-- 2b. Reprise des données : commentaire_txt → commentaire
UPDATE `famille_contacts`
   SET `commentaire` = `commentaire_txt`
 WHERE `commentaire_txt` IS NOT NULL AND `commentaire_txt` <> '';

-- 2c. Supprimer les colonnes enrichies
ALTER TABLE `famille_contacts`
  DROP COLUMN `commentaire_rtf`,
  DROP COLUMN `commentaire_txt`;


-- ---------------------------------------------------------------------------
-- SECTION 1 — Rétablir patients.alerte + retirer photo_fichier (BD-11 / BD-12)
-- ---------------------------------------------------------------------------

-- 1a. Recréer l'ancienne colonne alerte (text)
ALTER TABLE `patients`
  ADD COLUMN `alerte` text DEFAULT NULL
    AFTER `id_situation_familiale`;

-- 1b. Reprise des données : alerte_txt → alerte
UPDATE `patients`
   SET `alerte` = `alerte_txt`
 WHERE `alerte_txt` IS NOT NULL AND `alerte_txt` <> '';

-- 1c. Supprimer les colonnes ajoutées par la migration
ALTER TABLE `patients`
  DROP COLUMN `alerte_rtf`,
  DROP COLUMN `alerte_txt`,
  DROP COLUMN `photo_fichier`;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN migration_v2.1.0_lot1_patients_ROLLBACK.sql
-- =============================================================================
