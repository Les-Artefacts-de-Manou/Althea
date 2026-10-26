-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.20.0 (Lot 18 — Avoirs patient)
-- =============================================================================
-- Date : 27/09/2026
--
-- Objet :
--   Annuler migration_v2.20.0_lot18_mode_paiement_avoir.sql :
--     1. Suppression du mode AVOIR (uniquement s'il n'a jamais été utilisé).
--     2. Suppression de la colonne ref_modes_paiement.est_avoir.
--     3. Retrait de la ligne de version 2.20.0 dans tec_meta_schema.
--
-- ATTENTION : si des paiements utilisent déjà le mode Avoir, la ligne AVOIR
--             est conservée (traçabilité) ; seule la colonne est supprimée.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Suppression du mode AVOIR s'il est inutilisé
-- ---------------------------------------------------------------------------

DELETE FROM ref_modes_paiement
WHERE code_mode_paiement = 'AVOIR'
  AND NOT EXISTS (
      SELECT 1 FROM paiements p
      WHERE p.mode_paiement IN ('AVOIR', 'Avoir')
  );

-- ---------------------------------------------------------------------------
-- SECTION 2 — Suppression de la colonne est_avoir (si présente)
-- ---------------------------------------------------------------------------

SET @col_existe = (
    SELECT COUNT(*)
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'ref_modes_paiement'
      AND COLUMN_NAME  = 'est_avoir'
);

SET @sql_col = IF(
    @col_existe = 1,
    'ALTER TABLE `ref_modes_paiement` DROP COLUMN `est_avoir`',
    'SELECT ''INFORMATION : la colonne est_avoir est absente, suppression ignoree.'' AS info'
);

PREPARE stmt_col FROM @sql_col;
EXECUTE stmt_col;
DEALLOCATE PREPARE stmt_col;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Retrait de la ligne de version
-- ---------------------------------------------------------------------------

DELETE FROM tec_meta_schema
WHERE version_schema = '2.20.0';

-- =============================================================================
-- FIN DU ROLLBACK v2.20.0
-- =============================================================================
