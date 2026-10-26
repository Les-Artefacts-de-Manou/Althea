-- =============================================================================
-- ALTHÉA — Migration de schéma v2.9.0 (Lot 9 — Patient : tarif par défaut)
-- =============================================================================
-- Ajoute un champ `id_prix_seance_defaut` à la table `patients` pour stocker
-- le tarif préféré/historique du patient. NULL = utiliser le tarif courant
-- (60 EUR). Cela permet de distinguer les anciens (50), semi-anciens (55)
-- et nouveaux patients (60) lors de la création de séance.
--
-- Ordre d'exécution :
--   1. patients : ajout de id_prix_seance_defaut (nullable, FK vers prix_seance)
--   2. Versionner dans tec_meta_schema
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.9.0_lot9_patient_tarif_defaut_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_KEY_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — patients : ajout de id_prix_seance_defaut
--   Clé étrangère vers prix_seance (nullable = pas de tarif par défaut défini)
--   Index pour les recherches rapides par tarif
-- ---------------------------------------------------------------------------

ALTER TABLE `patients`
  ADD COLUMN `id_prix_seance_defaut` bigint unsigned DEFAULT NULL AFTER `date_desactivation`,
  ADD CONSTRAINT FK_patients_prix_seance_defaut 
    FOREIGN KEY (`id_prix_seance_defaut`) REFERENCES `ref_prix_seance`(`id_prix_seance`)
    ON DELETE SET NULL ON UPDATE RESTRICT,
  ADD INDEX IDX_patients_id_prix_seance_defaut (`id_prix_seance_defaut`);

-- ---------------------------------------------------------------------------
-- SECTION 2 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.9.0', 'Lot 9 patients : ajout de id_prix_seance_defaut pour tracer le tarif préféré/historique du patient (anciens 50, semi-anciens 55, nouveaux 60)', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40101 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
