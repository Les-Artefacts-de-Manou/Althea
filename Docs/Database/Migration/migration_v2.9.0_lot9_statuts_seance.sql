-- =============================================================================
-- ALTHÉA — Migration de schéma v2.9.0 (Lot 9 — Séances : enrichissement des statuts)
-- =============================================================================
-- Objectif :
--   Enrichir le référentiel ref_statuts_seance pour permettre des statistiques
--   d'annulation fines, en distinguant l'origine de l'annulation et l'absence
--   pour maladie. Le jeu initial (Lot 0) comportait un unique statut « Annulée »
--   sans distinction, insuffisant pour le suivi souhaité.
--
-- Détail des changements :
--   - id 3 : ANNULEE            → ANNULEE_PATIENT   « Annulée (patient) »
--            (renommage : annulation à l'initiative du patient)
--   - ajout ANNULEE_PRATICIEN   « Annulée (praticien) » (ordre 35)
--   - ajout MALADE              « Maladie »            (ordre 45)
--
-- États existants conservés :
--   1 PLANIFIEE « Planifiée » (10)
--   2 REALISEE  « Réalisée »  (20)  — équivaut à « honorée »
--   4 ABSENTE   « Absence patient » (40) — absence non prévenue
--   5 REPORTEE  « Reportée »  (50)
--
-- Prérequis : base en développement, ref_statuts_seance non référencé par des
--             séances (table seances vide) — aucun remap de données nécessaire.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.9.0_lot9_statuts_seance_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Renommage de l'ancien statut générique « Annulée »
--   ANNULEE → ANNULEE_PATIENT (annulation à l'initiative du patient)
-- ---------------------------------------------------------------------------

UPDATE `ref_statuts_seance`
SET `code_statut_seance`    = 'ANNULEE_PATIENT',
    `libelle_statut_seance` = 'Annulée (patient)',
    `ordre_affichage`       = 30
WHERE `code_statut_seance` = 'ANNULEE';

-- ---------------------------------------------------------------------------
-- SECTION 2 — Ajout des nouveaux statuts (idempotent via INSERT ... SELECT)
--   ANNULEE_PRATICIEN « Annulée (praticien) » (ordre 35)
--   MALADE            « Maladie »             (ordre 45)
-- ---------------------------------------------------------------------------

INSERT INTO `ref_statuts_seance` (`code_statut_seance`, `libelle_statut_seance`, `actif`, `ordre_affichage`)
SELECT 'ANNULEE_PRATICIEN', 'Annulée (praticien)', 1, 35
WHERE NOT EXISTS (
    SELECT 1 FROM `ref_statuts_seance` WHERE `code_statut_seance` = 'ANNULEE_PRATICIEN'
);

INSERT INTO `ref_statuts_seance` (`code_statut_seance`, `libelle_statut_seance`, `actif`, `ordre_affichage`)
SELECT 'MALADE', 'Maladie', 1, 45
WHERE NOT EXISTS (
    SELECT 1 FROM `ref_statuts_seance` WHERE `code_statut_seance` = 'MALADE'
);

-- ---------------------------------------------------------------------------
-- SECTION 3 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.9.0', 'Lot 9 séances : enrichissement de ref_statuts_seance (ANNULEE renommé ANNULEE_PATIENT, ajout ANNULEE_PRATICIEN et MALADE) pour statistiques d''annulation fines', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN DE LA MIGRATION v2.9.0
-- =============================================================================
