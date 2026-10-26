-- =============================================================================
-- ALTHÉA — Migration v2.24.0 (Lot 22 — Statuts consommant le quota)
-- =============================================================================
--
-- Objet :
--   1. Ajout de ref_statuts_seance.consomme_quota : le statut d'une séance indique
--      par paramétrage si elle consomme le quota du domaine (remplace la liste
--      codée en dur 'PLANIFIEE', 'REALISEE').
--   2. Initialisation : PLANIFIEE et REALISEE consomment le quota (comportement actuel).
--   3. Journalisation de la version dans tec_meta_schema.
--
-- Script rejouable. Compatible : MariaDB 12.x, InnoDB, utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

DROP PROCEDURE IF EXISTS tmp_ajouter_consomme_quota;

DELIMITER //
CREATE PROCEDURE tmp_ajouter_consomme_quota()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_statuts_seance'
          AND COLUMN_NAME = 'consomme_quota') = 0 THEN

        ALTER TABLE `ref_statuts_seance`
            ADD COLUMN `consomme_quota` tinyint(1) NOT NULL DEFAULT 0
                COMMENT 'Une seance dans ce statut consomme le quota du domaine' AFTER `est_du`;

        UPDATE `ref_statuts_seance`
        SET `consomme_quota` = 1
        WHERE `code_statut_seance` IN ('PLANIFIEE', 'REALISEE');
    END IF;
END //
DELIMITER ;

CALL tmp_ajouter_consomme_quota();
DROP PROCEDURE IF EXISTS tmp_ajouter_consomme_quota;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.24.0',
       'Lot 22 : ref_statuts_seance.consomme_quota (PLANIFIEE, REALISEE = 1) ; les statuts consommant le quota deviennent parametrables.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.24.0');
