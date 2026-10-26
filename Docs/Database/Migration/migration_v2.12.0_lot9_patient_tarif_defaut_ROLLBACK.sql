-- =============================================================================
-- ALTHÉA — Rollback de migration v2.9.0 (Lot 9 — Patient : tarif par défaut)
-- =============================================================================
-- Supprime la colonne `id_prix_seance_defaut` et sa contrainte FK de la table
-- `patients`, et supprime l'entrée de versionnage correspondante.
--
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_KEY_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Suppression de la colonne et de sa contrainte
-- ---------------------------------------------------------------------------

ALTER TABLE `patients`
  DROP FOREIGN KEY FK_patients_prix_seance_defaut;

ALTER TABLE `patients`
  DROP COLUMN `id_prix_seance_defaut`;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Suppression du versionnage
-- ---------------------------------------------------------------------------

DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.9.0';

/*!40101 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
