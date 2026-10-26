-- =============================================================================
-- ALTHÉA — Migration v2.22.0 (Lot 20 — Renommage ref_quotas_inami en ref_quotas)
-- =============================================================================
--
-- Objet :
--   1. Renommage de la table ref_quotas_inami en ref_quotas : les quotas sont
--      rattachés au domaine (convention = tarif + organisme + quotas) et ne
--      sont pas propres à l'INAMI.
--   2. Renommage de la clé id_quota_inami en id_quota.
--   3. Renommage des index et contraintes associés.
--   4. Journalisation de la version dans tec_meta_schema.
--
-- Règles :
--   - Aucune donnée n'est modifiée ; seul le schéma est renommé.
--   - Script à exécuter une seule fois (contrôle d'existence de la table).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

DROP PROCEDURE IF EXISTS tmp_renommer_ref_quotas;

DELIMITER //
CREATE PROCEDURE tmp_renommer_ref_quotas()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.TABLES
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_quotas_inami') = 1
       AND (SELECT COUNT(*) FROM information_schema.TABLES
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_quotas') = 0 THEN

        -- SECTION 1 — Contraintes nommées d'après l'ancienne table
        ALTER TABLE `ref_quotas_inami`
            DROP FOREIGN KEY `fk_ref_quotas_inami_domaine`,
            DROP CONSTRAINT `chk_ref_quotas_inami_plafonds`,
            DROP CONSTRAINT `chk_ref_quotas_inami_periode`;

        -- SECTION 2 — Table et clé primaire
        RENAME TABLE `ref_quotas_inami` TO `ref_quotas`;

        ALTER TABLE `ref_quotas`
            CHANGE COLUMN `id_quota_inami` `id_quota` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
            RENAME INDEX `uq_ref_quotas_inami_domaine_debut` TO `uq_ref_quotas_domaine_debut`,
            RENAME INDEX `IDX_ref_quotas_inami_validite` TO `IDX_ref_quotas_validite`,
            COMMENT = 'Regles de quota periodique de prestations remboursees, par domaine (convention)';

        -- SECTION 3 — Contraintes renommées
        ALTER TABLE `ref_quotas`
            ADD CONSTRAINT `fk_ref_quotas_domaine` FOREIGN KEY (`id_domaine`)
                REFERENCES `ref_domaines` (`id_domaine`) ON UPDATE CASCADE,
            ADD CONSTRAINT `chk_ref_quotas_plafonds`
                CHECK (`plafond_soutien` >= 0 AND `plafond_traitement` >= 0),
            ADD CONSTRAINT `chk_ref_quotas_periode`
                CHECK (`date_fin_validite` IS NULL OR `date_fin_validite` >= `date_debut_validite`);

        ALTER TABLE `ref_domaines`
            MODIFY COLUMN `soumis_quota` tinyint(1) NOT NULL DEFAULT 0
                COMMENT 'Domaine soumis a un quota periodique parametre dans ref_quotas — D-POR-3';
    ELSE
        SELECT 'INFORMATION : ref_quotas existe deja ou ref_quotas_inami est absente, renommage ignore.' AS info;
    END IF;
END //
DELIMITER ;

CALL tmp_renommer_ref_quotas();
DROP PROCEDURE IF EXISTS tmp_renommer_ref_quotas;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 4 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.22.0',
       'Lot 20 : renommage de ref_quotas_inami en ref_quotas (cle id_quota) ; les quotas restent rattaches au domaine avec periode de validite.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.22.0');

-- =============================================================================
-- FIN DE LA MIGRATION v2.22.0
-- =============================================================================
