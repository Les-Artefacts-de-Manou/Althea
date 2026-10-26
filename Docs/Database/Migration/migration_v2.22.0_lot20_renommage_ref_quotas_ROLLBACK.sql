-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.22.0 (Lot 20 — ref_quotas)
-- =============================================================================
--
-- Objet :
--   Rétablir ref_quotas_inami (clé id_quota_inami) et ses contraintes d'origine,
--   puis retirer la ligne de version 2.22.0 dans tec_meta_schema.
-- ATTENTION : l'application doit être revenue à une version antérieure.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

DROP PROCEDURE IF EXISTS tmp_retablir_ref_quotas_inami;

DELIMITER //
CREATE PROCEDURE tmp_retablir_ref_quotas_inami()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.TABLES
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_quotas') = 1 THEN

        ALTER TABLE `ref_quotas`
            DROP FOREIGN KEY `fk_ref_quotas_domaine`,
            DROP CONSTRAINT `chk_ref_quotas_plafonds`,
            DROP CONSTRAINT `chk_ref_quotas_periode`;

        RENAME TABLE `ref_quotas` TO `ref_quotas_inami`;

        ALTER TABLE `ref_quotas_inami`
            CHANGE COLUMN `id_quota` `id_quota_inami` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
            RENAME INDEX `uq_ref_quotas_domaine_debut` TO `uq_ref_quotas_inami_domaine_debut`,
            RENAME INDEX `IDX_ref_quotas_validite` TO `IDX_ref_quotas_inami_validite`,
            COMMENT = 'Regles de quota annuel de prestations remboursees, par domaine conventionne';

        ALTER TABLE `ref_quotas_inami`
            ADD CONSTRAINT `fk_ref_quotas_inami_domaine` FOREIGN KEY (`id_domaine`)
                REFERENCES `ref_domaines` (`id_domaine`) ON UPDATE CASCADE,
            ADD CONSTRAINT `chk_ref_quotas_inami_plafonds`
                CHECK (`plafond_soutien` >= 0 AND `plafond_traitement` >= 0),
            ADD CONSTRAINT `chk_ref_quotas_inami_periode`
                CHECK (`date_fin_validite` IS NULL OR `date_fin_validite` >= `date_debut_validite`);

        ALTER TABLE `ref_domaines`
            MODIFY COLUMN `soumis_quota` tinyint(1) NOT NULL DEFAULT 0
                COMMENT 'Domaine soumis a un quota periodique parametre dans ref_quotas_inami — D-POR-3';
    END IF;
END //
DELIMITER ;

CALL tmp_retablir_ref_quotas_inami();
DROP PROCEDURE IF EXISTS tmp_retablir_ref_quotas_inami;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

DELETE FROM tec_meta_schema
WHERE version_schema = '2.22.0';
