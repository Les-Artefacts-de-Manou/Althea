-- =============================================================================
-- ALTHÉA — Migration de schéma v2.1.0 (Lot 1 — Patients)
-- =============================================================================
-- Décisions appliquées :
--   D-Q12 (BD-11) : patients.alerte (text) → alerte_rtf + alerte_txt
--   D-Q13 (BD-12) : ajout patients.photo_fichier (nom seul, chemin déterministe)
--   D-Q7bis (BD-8): famille_contacts.commentaire + autres_suivis_patient.commentaire
--                   → commentaire_rtf + commentaire_txt
--   D-Q16 (BD-13) : abandon du champ libre dossiers.prescripteur (réseau N-N « Adresseur »)
--
-- Ordre d'exécution :
--   1. patients      : photo_fichier + alerte_rtf/alerte_txt (reprise alerte → alerte_txt)
--   2. famille_contacts        : commentaire_rtf/commentaire_txt (reprise → _txt)
--   3. autres_suivis_patient   : commentaire_rtf/commentaire_txt (reprise → _txt)
--   4. dossiers      : suppression de prescripteur
--   5. Versionner dans tec_meta_schema
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.1.0_lot1_patients_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — patients : photo_fichier (BD-12) + alerte enrichie (BD-11)
-- ---------------------------------------------------------------------------

-- 1a. Ajouter les nouvelles colonnes
ALTER TABLE `patients`
  ADD COLUMN `photo_fichier` varchar(255) DEFAULT NULL
    COMMENT 'Nom seul du fichier photo d''identité ; chemin reconstruit (D-Q13)'
    AFTER `id_situation_familiale`,
  ADD COLUMN `alerte_rtf` longtext DEFAULT NULL
    COMMENT 'Alerte patient au format RTF (D-Q12)'
    AFTER `photo_fichier`,
  ADD COLUMN `alerte_txt` longtext DEFAULT NULL
    COMMENT 'Alerte patient en texte brut pour la recherche (D-Q12)'
    AFTER `alerte_rtf`;

-- 1b. Reprise des données : ancienne alerte (text) → alerte_txt
UPDATE `patients`
   SET `alerte_txt` = `alerte`
 WHERE `alerte` IS NOT NULL AND `alerte` <> '';

-- 1c. Supprimer l'ancienne colonne alerte
ALTER TABLE `patients`
  DROP COLUMN `alerte`;


-- ---------------------------------------------------------------------------
-- SECTION 2 — famille_contacts : commentaire enrichi (BD-8)
-- ---------------------------------------------------------------------------

-- 2a. Ajouter les nouvelles colonnes
ALTER TABLE `famille_contacts`
  ADD COLUMN `commentaire_rtf` longtext DEFAULT NULL
    COMMENT 'Commentaire au format RTF (D-Q7bis)'
    AFTER `commentaire`,
  ADD COLUMN `commentaire_txt` longtext DEFAULT NULL
    COMMENT 'Commentaire en texte brut pour la recherche (D-Q7bis)'
    AFTER `commentaire_rtf`;

-- 2b. Reprise des données : ancien commentaire (text) → commentaire_txt
UPDATE `famille_contacts`
   SET `commentaire_txt` = `commentaire`
 WHERE `commentaire` IS NOT NULL AND `commentaire` <> '';

-- 2c. Supprimer l'ancienne colonne commentaire
ALTER TABLE `famille_contacts`
  DROP COLUMN `commentaire`;


-- ---------------------------------------------------------------------------
-- SECTION 3 — autres_suivis_patient : commentaire enrichi (BD-8)
-- ---------------------------------------------------------------------------

-- 3a. Ajouter les nouvelles colonnes
ALTER TABLE `autres_suivis_patient`
  ADD COLUMN `commentaire_rtf` longtext DEFAULT NULL
    COMMENT 'Commentaire au format RTF (D-Q7bis)'
    AFTER `commentaire`,
  ADD COLUMN `commentaire_txt` longtext DEFAULT NULL
    COMMENT 'Commentaire en texte brut pour la recherche (D-Q7bis)'
    AFTER `commentaire_rtf`;

-- 3b. Reprise des données : ancien commentaire (text) → commentaire_txt
UPDATE `autres_suivis_patient`
   SET `commentaire_txt` = `commentaire`
 WHERE `commentaire` IS NOT NULL AND `commentaire` <> '';

-- 3c. Supprimer l'ancienne colonne commentaire
ALTER TABLE `autres_suivis_patient`
  DROP COLUMN `commentaire`;


-- ---------------------------------------------------------------------------
-- SECTION 4 — dossiers : abandon de prescripteur (BD-13)
-- L'adresseur/prescripteur est désormais modélisé via le réseau N-N
-- (rôle « Adresseur » dans autres_suivis_patient / ref_roles_intervenant).
-- ---------------------------------------------------------------------------

ALTER TABLE `dossiers`
  DROP COLUMN `prescripteur`;


-- ---------------------------------------------------------------------------
-- SECTION 5 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.1.0', 'Lot 1 patients : alerte RTF (BD-11), photo_fichier (BD-12), commentaires famille/réseau RTF (BD-8), abandon dossiers.prescripteur (BD-13)', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN migration_v2.1.0_lot1_patients.sql
-- =============================================================================
