-- =============================================================================
-- ALTHÉA — Migration de schéma v2.5.0 (Lot 5 — Documents multi-niveaux)
-- =============================================================================
-- Décisions appliquées :
--   Les documents sont stockés selon une hiérarchie patient / dossier / séance.
--   Un même document peut être rattaché à l'un de ces trois niveaux :
--     - niveau patient  : ex. anamnèse (aucun dossier ni séance)
--     - niveau dossier  : ex. export d'une zone clinique
--     - niveau séance   : ex. document produit pendant une séance
--
--   Le niveau est déduit de la hiérarchie renseignée (règle applicative) :
--     id_seance renseigné  -> niveau séance
--     sinon id_dossier      -> niveau dossier
--     sinon id_patient      -> niveau patient
--
-- Corrections de schéma sur la table `documents` :
--   1. Ajout de `id_patient` (nullable) + FK vers `patients` afin de tracer
--      les documents de niveau patient (anamnèse).
--   2. `id_dossier` rendu NULLABLE : un document patient n'a pas de dossier.
--
-- Les colonnes id_seance, id_rendez_vous et id_modele_document sont déjà
-- nullables : aucune correction nécessaire de ce côté.
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.5.0_lot5_documents_niveaux_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — documents : ajout de id_patient (niveau patient)
-- ---------------------------------------------------------------------------

ALTER TABLE `documents`
  ADD COLUMN `id_patient` bigint(20) unsigned NULL DEFAULT NULL AFTER `id_dossier`;

ALTER TABLE `documents`
  ADD CONSTRAINT `fk_documents_patients`
  FOREIGN KEY (`id_patient`) REFERENCES `patients` (`id_patient`)
  ON DELETE RESTRICT ON UPDATE RESTRICT;

-- ---------------------------------------------------------------------------
-- SECTION 2 — documents : id_dossier rendu nullable (document niveau patient)
-- ---------------------------------------------------------------------------

ALTER TABLE `documents`
  MODIFY COLUMN `id_dossier` bigint(20) unsigned NULL DEFAULT NULL;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.5.0', 'Lot 5 documents : ajout id_patient (FK) et id_dossier nullable pour la hiérarchie patient/dossier/séance', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
