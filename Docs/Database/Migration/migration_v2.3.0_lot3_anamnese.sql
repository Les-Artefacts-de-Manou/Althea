-- =============================================================================
-- ALTHÉA — Migration de schéma v2.3.0 (Lot 3 — Anamnèse patient)
-- =============================================================================
-- Décisions appliquées :
--   D-Q19 : ajout de l'anamnèse au niveau Patient, en double format
--           (anamnese_rtf + anamnese_txt), sur le modèle de l'alerte (BD-11).
--
-- Justification : l'anamnèse est un texte clinique rédigé librement, rattaché
-- au patient (et non au dossier). Elle est saisie via l'éditeur enrichi
-- (UC_RichTextEditor) : le RTF conserve le formatage, le texte brut sert à la
-- recherche et à l'affichage simple — exactement comme patients.alerte_*.
--
-- Ordre d'exécution :
--   1. patients : ajout anamnese_rtf + anamnese_txt (après alerte_txt)
--   2. Versionner dans tec_meta_schema
--
-- Remarque : les paramètres de chemin (PATH_GENERAL, PATH_DOCUMENT) existent
-- déjà dans tec_parametres (groupe PATHS) et sont gérés par l'utilisateur
-- dans UC_Parametres ; cette migration ne les touche pas.
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.3.0_lot3_anamnese_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — patients : anamnèse enrichie (D-Q19)
-- Double format RTF + texte brut, sur le modèle de alerte_rtf / alerte_txt.
-- ---------------------------------------------------------------------------

ALTER TABLE `patients`
  ADD COLUMN `anamnese_rtf` longtext DEFAULT NULL
    COMMENT 'Anamnèse patient au format RTF (D-Q19)'
    AFTER `alerte_txt`,
  ADD COLUMN `anamnese_txt` longtext DEFAULT NULL
    COMMENT 'Anamnèse patient en texte brut pour la recherche (D-Q19)'
    AFTER `anamnese_rtf`;


-- ---------------------------------------------------------------------------
-- SECTION 2 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.3.0', 'Lot 3 patients : anamnèse enrichie RTF + texte brut (anamnese_rtf / anamnese_txt) au niveau patient (D-Q19)', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN migration_v2.3.0_lot3_anamnese.sql
-- =============================================================================
