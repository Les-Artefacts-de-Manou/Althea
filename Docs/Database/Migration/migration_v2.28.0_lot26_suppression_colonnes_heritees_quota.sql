-- =============================================================================
-- ALTHÉA — Migration v2.28.0 (Lot 26 — Suppression des colonnes héritées du modèle de quota)
-- =============================================================================
--
-- Objet :
--   1. ref_natures_seance.id_categorie_quota (lot 23) : remplacée par
--      lia_natures_seance_domaines.id_categorie_quota (lot 24). Suppression de la
--      clé étrangère, de l'index et de la colonne.
--   2. ref_quotas.plafond_soutien / plafond_traitement : remplacées par
--      ref_quotas_plafonds (plafond par catégorie). Suppression de la contrainte
--      chk_ref_quotas_plafonds puis des colonnes.
--
-- Règles :
--   - Prérequis : migration v2.27.0 appliquée ; sauvegarde préalable obligatoire.
--   - Le code applicatif ne lit ni n'écrit plus ces colonnes.
--   - Script rejouable. Compatible : MariaDB 12.x, InnoDB, utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Contrôle préalable (à lire avant exécution)
-- ---------------------------------------------------------------------------
-- Natures catégorisées sans liaison portant la même catégorie (doit retourner 0 ligne) :
SELECT ns.id_nature_seance, ns.code_nature_seance, ns.id_categorie_quota
FROM ref_natures_seance ns
WHERE ns.id_categorie_quota IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM lia_natures_seance_domaines l
                  WHERE l.id_nature_seance = ns.id_nature_seance
                    AND l.id_categorie_quota = ns.id_categorie_quota);

-- Règles de quota sans aucun plafond par catégorie (doit retourner 0 ligne) :
SELECT q.id_quota, q.id_domaine, q.plafond_soutien, q.plafond_traitement
FROM ref_quotas q
WHERE NOT EXISTS (SELECT 1 FROM ref_quotas_plafonds p WHERE p.id_quota = q.id_quota);

-- ---------------------------------------------------------------------------
-- SECTION 2 — Structure
-- ---------------------------------------------------------------------------

DROP PROCEDURE IF EXISTS tmp_lot26_suppression;

DELIMITER //
CREATE PROCEDURE tmp_lot26_suppression()
BEGIN
    -- ref_natures_seance.id_categorie_quota
    IF (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_natures_seance'
          AND CONSTRAINT_NAME = 'fk_ref_natures_seance_categorie') > 0 THEN
        ALTER TABLE `ref_natures_seance` DROP FOREIGN KEY `fk_ref_natures_seance_categorie`;
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_natures_seance'
          AND INDEX_NAME = 'IDX_ref_natures_seance_categorie') > 0 THEN
        ALTER TABLE `ref_natures_seance` DROP INDEX `IDX_ref_natures_seance_categorie`;
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_natures_seance'
          AND COLUMN_NAME = 'id_categorie_quota') > 0 THEN
        ALTER TABLE `ref_natures_seance` DROP COLUMN `id_categorie_quota`;
    END IF;

    -- ref_quotas.plafond_soutien / plafond_traitement
    IF (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_quotas'
          AND CONSTRAINT_NAME = 'chk_ref_quotas_plafonds') > 0 THEN
        ALTER TABLE `ref_quotas` DROP CONSTRAINT `chk_ref_quotas_plafonds`;
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_quotas'
          AND COLUMN_NAME = 'plafond_soutien') > 0 THEN
        ALTER TABLE `ref_quotas` DROP COLUMN `plafond_soutien`;
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_quotas'
          AND COLUMN_NAME = 'plafond_traitement') > 0 THEN
        ALTER TABLE `ref_quotas` DROP COLUMN `plafond_traitement`;
    END IF;
END //
DELIMITER ;

CALL tmp_lot26_suppression();
DROP PROCEDURE IF EXISTS tmp_lot26_suppression;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.28.0',
       'Lot 26 : suppression des colonnes heritees ref_natures_seance.id_categorie_quota (FK, index) et ref_quotas.plafond_soutien / plafond_traitement (contrainte chk_ref_quotas_plafonds).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.28.0');
