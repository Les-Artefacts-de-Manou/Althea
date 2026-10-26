-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.21.0 (Lot 19 — Lignes système)
-- =============================================================================
--
-- Objet :
--   Annuler migration_v2.21.0_lot19_referentiels_est_systeme.sql :
--     1. Suppression de la colonne est_systeme des référentiels concernés.
--     2. Retrait de la ligne de version 2.21.0 dans tec_meta_schema.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

DROP PROCEDURE IF EXISTS tmp_retirer_est_systeme;

DELIMITER //
CREATE PROCEDURE tmp_retirer_est_systeme(IN p_table VARCHAR(64))
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = p_table
          AND COLUMN_NAME  = 'est_systeme') = 1 THEN
        SET @sql_col = CONCAT('ALTER TABLE `', p_table, '` DROP COLUMN `est_systeme`');
        PREPARE stmt_col FROM @sql_col;
        EXECUTE stmt_col;
        DEALLOCATE PREPARE stmt_col;
    END IF;
END //
DELIMITER ;

CALL tmp_retirer_est_systeme('ref_statuts_seance');
CALL tmp_retirer_est_systeme('ref_statuts_dossier');
CALL tmp_retirer_est_systeme('ref_modes_paiement');
CALL tmp_retirer_est_systeme('ref_types_documents');
CALL tmp_retirer_est_systeme('ref_liens_patient');
CALL tmp_retirer_est_systeme('ref_role_legal');
CALL tmp_retirer_est_systeme('ref_prix_seance');

DROP PROCEDURE IF EXISTS tmp_retirer_est_systeme;

DELETE FROM tec_meta_schema
WHERE version_schema = '2.21.0';
