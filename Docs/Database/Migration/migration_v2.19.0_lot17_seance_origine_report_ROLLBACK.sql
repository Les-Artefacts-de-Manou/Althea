-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.19.0 (Lot 17 — Traçabilité des reports)
-- =============================================================================
-- Date : 25/09/2026
--
-- Objet :
--   Annuler les changements introduits par
--   migration_v2.19.0_lot17_seance_origine_report.sql :
--     1. Suppression de la contrainte FK auto-référencée + index.
--     2. Suppression de la colonne seances.id_seance_origine.
--     3. Retrait de la ligne de version 2.19.0 dans tec_meta_schema.
--
-- ATTENTION : la suppression de la colonne id_seance_origine efface
--             définitivement la trace des reports enregistrés.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Suppression de la contrainte FK et de son index (si présents)
-- ---------------------------------------------------------------------------

SET @fk_existe = (
    SELECT COUNT(*)
    FROM information_schema.TABLE_CONSTRAINTS
    WHERE TABLE_SCHEMA    = DATABASE()
      AND TABLE_NAME      = 'seances'
      AND CONSTRAINT_NAME = 'fk_seances_seance_origine'
);

SET @sql_fk = IF(
    @fk_existe = 1,
    'ALTER TABLE `seances` DROP FOREIGN KEY `fk_seances_seance_origine`',
    'SELECT ''INFORMATION : la contrainte fk_seances_seance_origine est absente, suppression ignoree.'' AS info'
);

PREPARE stmt_fk FROM @sql_fk;
EXECUTE stmt_fk;
DEALLOCATE PREPARE stmt_fk;

SET @idx_existe = (
    SELECT COUNT(*)
    FROM information_schema.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'seances'
      AND INDEX_NAME   = 'fk_seances_seance_origine'
);

SET @sql_idx = IF(
    @idx_existe > 0,
    'ALTER TABLE `seances` DROP INDEX `fk_seances_seance_origine`',
    'SELECT ''INFORMATION : l index fk_seances_seance_origine est absent, suppression ignoree.'' AS info'
);

PREPARE stmt_idx FROM @sql_idx;
EXECUTE stmt_idx;
DEALLOCATE PREPARE stmt_idx;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Suppression de la colonne id_seance_origine (si présente)
-- ---------------------------------------------------------------------------

SET @col_existe = (
    SELECT COUNT(*)
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'seances'
      AND COLUMN_NAME  = 'id_seance_origine'
);

SET @sql_col = IF(
    @col_existe = 1,
    'ALTER TABLE `seances` DROP COLUMN `id_seance_origine`',
    'SELECT ''INFORMATION : la colonne id_seance_origine est absente, suppression ignoree.'' AS info'
);

PREPARE stmt_col FROM @sql_col;
EXECUTE stmt_col;
DEALLOCATE PREPARE stmt_col;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Retrait de la ligne de version dans tec_meta_schema
-- ---------------------------------------------------------------------------

DELETE FROM tec_meta_schema
WHERE version_schema = '2.19.0';

-- =============================================================================
-- FIN DU ROLLBACK v2.19.0
-- =============================================================================
