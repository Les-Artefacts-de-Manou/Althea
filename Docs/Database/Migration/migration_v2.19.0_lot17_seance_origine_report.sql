-- =============================================================================
-- ALTHÉA — Migration de schéma v2.19.0 (Lot 17 — Traçabilité des reports de séance)
-- =============================================================================
-- Date : 25/09/2026
--
-- Objectif :
--   Tracer de manière structurée le report d'une séance et, par extension, le
--   report du paiement associé. Lorsqu'une séance est reportée, une nouvelle
--   séance de remplacement est créée : cette migration ajoute sur la nouvelle
--   séance un lien vers la séance d'origine, afin que l'application puisse
--   afficher « issue d'un report de SExxxxxx » dans la grille des séances et
--   reconstruire les libellés de mouvement de paiement (origine / destination)
--   sans recourir à du texte libre.
--
-- Détail des changements :
--   1. seances.id_seance_origine : nouvelle colonne bigint(20) unsigned nullable.
--      - NULL  : séance ordinaire (non issue d'un report).
--      - RENSEIGNÉ : la séance est le résultat du report de la séance pointée.
--   2. Contrainte FK auto-référencée fk_seances_seance_origine.
--      ON DELETE SET NULL : la suppression de la séance d'origine ne casse pas
--      la séance de remplacement, elle perd simplement la trace de son origine.
--
-- Motivation :
--   Un champ dédié FK est plus robuste qu'un commentaire libre : le libellé
--   d'affichage (« Report de SE000036 ») est recalculé à partir du code réel de
--   la séance liée et reste juste dans le temps. Il permet aussi une navigation
--   future « voir la séance liée ».
--
-- Prérequis : base en développement. La colonne est nullable, aucune donnée
--             existante n'est impactée (toutes les séances actuelles restent
--             considérées comme non issues d'un report).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.19.0_lot17_seance_origine_report_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Ajout de la colonne de traçabilité d'origine
--   Contrôle de rejouabilité : la colonne n'est ajoutée que si elle n'existe
--   pas déjà, afin que le script puisse être relancé sans erreur.
-- ---------------------------------------------------------------------------

SET @col_existe = (
    SELECT COUNT(*)
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'seances'
      AND COLUMN_NAME  = 'id_seance_origine'
);

SET @sql_col = IF(
    @col_existe = 0,
    'ALTER TABLE `seances`
        ADD COLUMN `id_seance_origine` bigint(20) unsigned DEFAULT NULL
        COMMENT ''Séance dont celle-ci est issue par report (NULL si séance ordinaire) — D-SEA-2''
        AFTER `id_statut_seance`',
    'SELECT ''INFORMATION : la colonne id_seance_origine existe deja, ajout ignore.'' AS info'
);

PREPARE stmt_col FROM @sql_col;
EXECUTE stmt_col;
DEALLOCATE PREPARE stmt_col;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Index + contrainte de clé étrangère auto-référencée
-- ---------------------------------------------------------------------------

SET @fk_existe = (
    SELECT COUNT(*)
    FROM information_schema.TABLE_CONSTRAINTS
    WHERE TABLE_SCHEMA    = DATABASE()
      AND TABLE_NAME      = 'seances'
      AND CONSTRAINT_NAME = 'fk_seances_seance_origine'
);

SET @sql_fk = IF(
    @fk_existe = 0,
    'ALTER TABLE `seances`
        ADD KEY `fk_seances_seance_origine` (`id_seance_origine`),
        ADD CONSTRAINT `fk_seances_seance_origine`
            FOREIGN KEY (`id_seance_origine`) REFERENCES `seances` (`id_seance`)
            ON DELETE SET NULL',
    'SELECT ''INFORMATION : la contrainte fk_seances_seance_origine existe deja, ajout ignore.'' AS info'
);

PREPARE stmt_fk FROM @sql_fk;
EXECUTE stmt_fk;
DEALLOCATE PREPARE stmt_fk;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
VALUES (
    '2.19.0',
    'Lot 17 : ajout de seances.id_seance_origine (FK auto-referencee) pour tracer le report d une seance et reconstituer les libelles de mouvement de paiement.',
    NOW()
);

-- =============================================================================
-- FIN DE LA MIGRATION v2.19.0
-- =============================================================================
