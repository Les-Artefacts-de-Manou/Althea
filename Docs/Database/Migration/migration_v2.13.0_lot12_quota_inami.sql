-- =============================================================================
-- ALTHÉA — Migration de schéma v2.13.0 (Lot 12 — Quota INAMI Realism)
-- =============================================================================
-- Contexte métier (retour Pearl du 18/09/2026) :
--   Chaque patient suivi dans un domaine INAMI (Realism) dispose d'un quota
--   annuel de prestations remboursées :
--     * 10 séances de SOUTIEN
--     * 20 séances de TRAITEMENT
--     => 30 séances au total, sur une période glissante de 1 an de date à date
--        (à partir de la 1re séance INAMI du dossier).
--   Au-delà de ce quota, les séances supplémentaires sont à charge du patient.
--
--   Pour compter séparément soutien et traitement, la NATURE de la séance doit
--   porter un discriminant de prestation INAMI. Les natures existantes (Bilan,
--   Traitement, etc.) restent neutres (NULL) : elles ne consomment pas le quota.
--
-- Ordre d'exécution :
--   1. ref_natures_seance : ajout de code_prestation_inami (nullable)
--   2. ref_natures_seance : insertion des 2 natures INAMI (soutien/traitement)
--   3. Versionner dans tec_meta_schema
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.13.0_lot12_quota_inami_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — ref_natures_seance : ajout de code_prestation_inami
--   NULL          = nature neutre, ne consomme aucun quota INAMI
--   'SOUTIEN'     = prestation de soutien   (plafond 10 / an)
--   'TRAITEMENT'  = prestation de traitement (plafond 20 / an)
--   Index pour les agrégations de comptage par type de prestation.
-- ---------------------------------------------------------------------------

ALTER TABLE `ref_natures_seance`
  ADD COLUMN `code_prestation_inami` varchar(20) DEFAULT NULL
    COMMENT 'Discriminant de quota INAMI : SOUTIEN, TRAITEMENT ou NULL (hors quota) — D-INA-1'
    AFTER `libelle_nature_seance`,
  ADD INDEX `IDX_ref_natures_seance_prestation_inami` (`code_prestation_inami`);

-- ---------------------------------------------------------------------------
-- SECTION 2 — ref_natures_seance : insertion des natures INAMI
--   Les identifiants ne sont pas forcés : la table est en AUTO_INCREMENT et
--   d'autres natures ont pu être ajoutées à la volée par l'utilisateur.
--   INSERT IGNORE protège contre une double exécution (code et libellé sont
--   couverts par des contraintes d'unicité).
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;

INSERT IGNORE INTO `ref_natures_seance`
  (`code_nature_seance`, `libelle_nature_seance`, `code_prestation_inami`, `actif`, `ordre_affichage`)
VALUES
  ('SOUTIEN_INAMI',    'Soutien (INAMI)',    'SOUTIEN',    1, 60),
  ('TRAITEMENT_INAMI', 'Traitement (INAMI)', 'TRAITEMENT', 1, 70);

COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.13.0', 'Lot 12 quota INAMI : colonne ref_natures_seance.code_prestation_inami et natures Soutien (INAMI) / Traitement (INAMI) pour le comptage des 10 + 20 seances remboursees sur 1 an glissant', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40101 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
