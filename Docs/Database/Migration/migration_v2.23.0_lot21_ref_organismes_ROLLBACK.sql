-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.23.0 (Lot 21 — ref_organismes)
-- =============================================================================
--
-- Objet :
--   Retirer ref_domaines.id_organisme, supprimer ref_organismes, puis retirer la
--   ligne de version 2.23.0 dans tec_meta_schema.
-- ATTENTION : l'application doit être revenue à une version antérieure.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

DROP PROCEDURE IF EXISTS tmp_retirer_organisme_domaine;

DELIMITER //
CREATE PROCEDURE tmp_retirer_organisme_domaine()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_domaines'
          AND COLUMN_NAME = 'id_organisme') = 1 THEN

        ALTER TABLE `ref_domaines`
            DROP FOREIGN KEY `fk_ref_domaines_organisme`;

        ALTER TABLE `ref_domaines`
            DROP KEY `IDX_ref_domaines_organisme`,
            DROP COLUMN `id_organisme`;
    END IF;
END //
DELIMITER ;

CALL tmp_retirer_organisme_domaine();
DROP PROCEDURE IF EXISTS tmp_retirer_organisme_domaine;

DROP TABLE IF EXISTS `ref_organismes`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.23.0';
