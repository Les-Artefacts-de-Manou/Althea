-- =============================================================================
-- ALTHÉA — Migration v2.26.1 (Lot 24 — Catégorie de quota portée par la liaison nature/domaine)
-- =============================================================================
--
-- Objet :
--   1. lia_natures_seance_domaines.id_categorie_quota : la catégorie consommée est
--      définie pour le couple (nature, domaine). Une même nature peut ainsi consommer
--      une catégorie différente selon l'organisme / la convention du dossier.
--   2. Intégrité : la catégorie doit appartenir au MÊME domaine que la liaison
--      (clé étrangère composite id_categorie_quota + id_domaine).
--   3. Reprise des valeurs de ref_natures_seance.id_categorie_quota (lot 23).
--
-- Règles :
--   - Migration ADDITIVE : ref_natures_seance.id_categorie_quota est conservée ;
--     sa suppression est prévue dans la migration de nettoyage (avec
--     plafond_soutien, plafond_traitement, code_prestation_inami).
--   - Prérequis : migration v2.25.0 appliquée.
--   - Script rejouable. Compatible : MariaDB 12.x, InnoDB, utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Structure
-- ---------------------------------------------------------------------------

DROP PROCEDURE IF EXISTS tmp_lot24_colonnes;

DELIMITER //
CREATE PROCEDURE tmp_lot24_colonnes()
BEGIN
    -- Clé candidate (catégorie, domaine) servant de cible à la clé étrangère composite
    IF (SELECT COUNT(*) FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_categories_quota'
          AND INDEX_NAME = 'uq_ref_categories_quota_id_domaine') = 0 THEN
        ALTER TABLE `ref_categories_quota`
            ADD UNIQUE KEY `uq_ref_categories_quota_id_domaine` (`id_categorie_quota`, `id_domaine`);
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'lia_natures_seance_domaines'
          AND COLUMN_NAME = 'id_categorie_quota') = 0 THEN
        ALTER TABLE `lia_natures_seance_domaines`
            ADD COLUMN `id_categorie_quota` bigint(20) unsigned NULL DEFAULT NULL
                COMMENT 'Categorie de quota consommee par la nature dans ce domaine (NULL = hors quota)' AFTER `id_domaine`,
            ADD KEY `IDX_lia_nature_domaine_categorie` (`id_categorie_quota`, `id_domaine`),
            ADD CONSTRAINT `fk_lia_nature_domaine_categorie` FOREIGN KEY (`id_categorie_quota`, `id_domaine`)
                REFERENCES `ref_categories_quota` (`id_categorie_quota`, `id_domaine`) ON UPDATE CASCADE;
    END IF;
END //
DELIMITER ;

CALL tmp_lot24_colonnes();
DROP PROCEDURE IF EXISTS tmp_lot24_colonnes;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Reprise des données du lot 23
-- ---------------------------------------------------------------------------
-- Uniquement lorsque la catégorie de la nature appartient au domaine de la liaison.

UPDATE `lia_natures_seance_domaines` l
JOIN `ref_natures_seance` ns ON ns.`id_nature_seance` = l.`id_nature_seance`
JOIN `ref_categories_quota` c ON c.`id_categorie_quota` = ns.`id_categorie_quota`
                              AND c.`id_domaine` = l.`id_domaine`
SET l.`id_categorie_quota` = c.`id_categorie_quota`
WHERE l.`id_categorie_quota` IS NULL;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Contrôle (à lire après exécution)
-- ---------------------------------------------------------------------------
-- Natures catégorisées dont aucune liaison n'a reçu la catégorie (doit retourner 0 ligne) :
SELECT ns.id_nature_seance, ns.code_nature_seance, ns.id_categorie_quota
FROM ref_natures_seance ns
WHERE ns.id_categorie_quota IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM lia_natures_seance_domaines l
                  WHERE l.id_nature_seance = ns.id_nature_seance
                    AND l.id_categorie_quota = ns.id_categorie_quota);

-- Vue d'ensemble des liaisons catégorisées :
SELECT d.code_domaine, ns.code_nature_seance, c.libelle_categorie
FROM lia_natures_seance_domaines l
JOIN ref_domaines d ON d.id_domaine = l.id_domaine
JOIN ref_natures_seance ns ON ns.id_nature_seance = l.id_nature_seance
LEFT JOIN ref_categories_quota c ON c.id_categorie_quota = l.id_categorie_quota
ORDER BY d.code_domaine, ns.code_nature_seance;

-- ---------------------------------------------------------------------------
-- SECTION 4 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.26.1',
       'Lot 24 : lia_natures_seance_domaines.id_categorie_quota (categorie de quota par couple nature/domaine, FK composite meme domaine) ; reprise depuis ref_natures_seance. Migration additive.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.26.1');
