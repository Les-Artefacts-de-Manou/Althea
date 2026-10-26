-- =============================================================================
-- ALTHÉA — Migration v2.20.0 (Lot 18 — Réutilisation des avoirs patient)
-- =============================================================================
-- Date : 27/09/2026
--
-- Objet :
--   1. Ajout de l'attribut ref_modes_paiement.est_avoir : identifie, par
--      paramétrage, le mode de paiement servant à tracer l'utilisation d'un
--      avoir (aucune constante de code pivot).
--   2. Insertion du mode « Avoir » (code AVOIR) marqué est_avoir = 1.
--   3. Journalisation de la version dans tec_meta_schema.
--
-- Règles :
--   - Un mode est_avoir = 1 n'est jamais proposé à la saisie manuelle d'un
--     paiement : il est réservé à l'utilisation d'un avoir.
--   - Script rejouable (contrôles d'existence).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Colonne est_avoir (si absente)
-- ---------------------------------------------------------------------------

SET @col_existe = (
    SELECT COUNT(*)
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'ref_modes_paiement'
      AND COLUMN_NAME  = 'est_avoir'
);

SET @sql_col = IF(
    @col_existe = 0,
    'ALTER TABLE `ref_modes_paiement` ADD COLUMN `est_avoir` tinyint(1) NOT NULL DEFAULT 0 COMMENT ''1 = mode reserve a l utilisation d un avoir patient (non propose a la saisie manuelle)'' AFTER `libelle_mode_paiement`',
    'SELECT ''INFORMATION : la colonne est_avoir existe deja, ajout ignore.'' AS info'
);

PREPARE stmt_col FROM @sql_col;
EXECUTE stmt_col;
DEALLOCATE PREPARE stmt_col;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Mode de paiement « Avoir »
-- ---------------------------------------------------------------------------

INSERT INTO ref_modes_paiement (code_mode_paiement, libelle_mode_paiement, est_avoir, actif, ordre_affichage)
SELECT 'AVOIR', 'Avoir', 1, 1, 90
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1 FROM ref_modes_paiement WHERE code_mode_paiement = 'AVOIR'
);

UPDATE ref_modes_paiement
SET est_avoir = 1
WHERE code_mode_paiement = 'AVOIR';

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
VALUES (
    '2.20.0',
    'Lot 18 : ajout de ref_modes_paiement.est_avoir et du mode Avoir pour tracer la reutilisation d un avoir patient lors d un encaissement.',
    NOW()
);

-- =============================================================================
-- FIN DE LA MIGRATION v2.20.0
-- =============================================================================
