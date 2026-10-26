-- =============================================================================
-- ALTHÉA — Migration de schéma v2.7.0 (Lot 7 — suivi_en_cours nullable)
-- =============================================================================
-- Le statut « suivi en cours » d'un patient est désormais DÉRIVÉ de ses dossiers
-- et non plus une valeur saisie/par défaut :
--   - NULL : le patient n'a AUCUN dossier (état neutre, pastille distincte)
--   - 1    : le patient a au moins un dossier vivant (ACTIF ou PAUSE)
--   - 0    : le patient n'a que des dossiers non vivants (CLOTURE / ARCHIVE)
--
-- Ordre d'exécution :
--   1. patients.suivi_en_cours : rendre la colonne nullable (défaut NULL)
--   2. Recalcul des valeurs existantes à partir des dossiers
--   3. Versionner dans tec_meta_schema
--
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.7.0_lot7_suivi_en_cours_nullable_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — patients.suivi_en_cours : colonne nullable, défaut NULL
-- ---------------------------------------------------------------------------

ALTER TABLE `patients`
  MODIFY COLUMN `suivi_en_cours` tinyint(4) DEFAULT NULL;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Recalcul des valeurs existantes à partir des dossiers
--   NULL si aucun dossier, 1 si au moins un dossier ACTIF/PAUSE, sinon 0.
-- ---------------------------------------------------------------------------

UPDATE `patients` p
LEFT JOIN (
    SELECT
        d.id_patient,
        COUNT(d.id_dossier) AS nb_dossiers,
        SUM(
            CASE WHEN sd.code_statut_dossier IN ('ACTIF', 'PAUSE') THEN 1 ELSE 0 END
        ) AS nb_vivants
    FROM `dossiers` d
    LEFT JOIN `ref_statuts_dossier` sd ON sd.id_statut_dossier = d.id_statut_dossier
    GROUP BY d.id_patient
) x ON x.id_patient = p.id_patient
SET p.`suivi_en_cours` =
    CASE
        WHEN x.nb_dossiers IS NULL OR x.nb_dossiers = 0 THEN NULL
        WHEN x.nb_vivants > 0 THEN 1
        ELSE 0
    END;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.7.0', 'Lot 7 patients : suivi_en_cours rendu nullable (NULL = aucun dossier) et dérivé des dossiers (1 = ACTIF/PAUSE, 0 = CLOTURE/ARCHIVE)', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- Restauration des flags
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
