-- =============================================================================
-- ALTHÉA — Migration de schéma v2.8.0 (Lot 8 — Patient : is_actif + retrait suivi_en_cours)
-- =============================================================================
-- Deux notions distinctes sont désormais séparées :
--   1. Le « suivi » d'un patient = état de son DERNIER dossier (Actif/Pause/Clôturé/
--      Archivé) ou « pas de dossier ». Cette information est DÉRIVÉE des dossiers et
--      calculée à la volée (sous-requête dans SelectPatientsListe) : la colonne
--      patients.suivi_en_cours devient inutile et est SUPPRIMÉE.
--   2. L'activation du patient = décision MANUELLE de l'utilisateur (is_actif), avec
--      dates d'activation / désactivation. Indépendante du cycle de vie des dossiers :
--      archiver un dossier ne désactive plus le patient.
--
-- Ordre d'exécution :
--   1. patients : suppression de suivi_en_cours
--   2. patients : ajout de is_actif (NOT NULL DEFAULT 1), date_activation, date_desactivation
--   3. Versionner dans tec_meta_schema
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.8.0_lot8_patient_is_actif_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — patients : suppression de suivi_en_cours (désormais dérivé)
-- ---------------------------------------------------------------------------

ALTER TABLE `patients`
  DROP COLUMN `suivi_en_cours`;

-- ---------------------------------------------------------------------------
-- SECTION 2 — patients : ajout de is_actif + dates d'activation/désactivation
--   is_actif : bascule manuelle (1 = actif par défaut).
--   date_activation : renseignée par défaut à la création (aujourd'hui pour l'existant).
--   date_desactivation : renseignée uniquement lors d'une désactivation.
-- ---------------------------------------------------------------------------

ALTER TABLE `patients`
  ADD COLUMN `is_actif` tinyint(1) NOT NULL DEFAULT 1 AFTER `anamnese_txt`,
  ADD COLUMN `date_activation` date DEFAULT NULL AFTER `is_actif`,
  ADD COLUMN `date_desactivation` date DEFAULT NULL AFTER `date_activation`;

-- Initialisation de l'existant : tous actifs, date d'activation = aujourd'hui.
UPDATE `patients`
SET `date_activation` = CURRENT_DATE()
WHERE `date_activation` IS NULL;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.8.0', 'Lot 8 patients : suppression de suivi_en_cours (dérivé du dernier dossier) et ajout de is_actif + date_activation/date_desactivation (activation manuelle indépendante des dossiers)', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
