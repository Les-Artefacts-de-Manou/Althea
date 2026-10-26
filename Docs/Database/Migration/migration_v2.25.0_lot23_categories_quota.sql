-- =============================================================================
-- ALTHÉA — Migration v2.25.0 (Lot 23 — Catégories de quota paramétrables)
-- =============================================================================
--
-- Objet :
--   1. ref_categories_quota : catégories de quota définies PAR DOMAINE
--      (ex. Realism : Soutien, Traitement). Nombre libre.
--   2. ref_quotas_plafonds : plafond de chaque catégorie pour une règle datée
--      (ref_quotas reste l'en-tête : domaine, seuil d'alerte, validité).
--   3. ref_quotas.duree_periode_mois : durée de la période glissante (défaut 12).
--   4. ref_natures_seance.id_categorie_quota : catégorie consommée par la nature
--      (remplacera code_prestation_inami).
--   5. Conversion des données existantes (SOUTIEN / TRAITEMENT, plafonds actuels).
--
-- Règles :
--   - Migration ADDITIVE : les anciennes colonnes (plafond_soutien, plafond_traitement,
--     code_prestation_inami) sont conservées ; l'application actuelle fonctionne à
--     l'identique. Leur suppression fera l'objet d'une migration ultérieure.
--   - Plafonds historisés : une évolution passe par clôture + nouvelle règle.
--   - Script rejouable. Compatible : MariaDB 12.x, InnoDB, utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — ref_categories_quota
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_categories_quota` (
    `id_categorie_quota`   bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `id_domaine`           bigint(20) unsigned NOT NULL COMMENT 'Domaine (convention) auquel appartient la categorie',
    `code_categorie`       varchar(30)  NOT NULL,
    `libelle_categorie`    varchar(100) NOT NULL,
    `actif`                tinyint(1)   NOT NULL DEFAULT 1,
    `ordre_affichage`      int(11)      NOT NULL DEFAULT 0,
    PRIMARY KEY (`id_categorie_quota`),
    UNIQUE KEY `uq_ref_categories_quota_code` (`id_domaine`, `code_categorie`),
    UNIQUE KEY `uq_ref_categories_quota_libelle` (`id_domaine`, `libelle_categorie`),
    CONSTRAINT `fk_ref_categories_quota_domaine` FOREIGN KEY (`id_domaine`)
        REFERENCES `ref_domaines` (`id_domaine`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Categories de quota propres a chaque domaine (ex. Soutien, Traitement)';

-- Alignement de collation si la table a été créée par une exécution antérieure
ALTER TABLE `ref_categories_quota` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_uca1400_ai_ci;

-- ---------------------------------------------------------------------------
-- SECTION 2 — ref_quotas_plafonds
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_quotas_plafonds` (
    `id_quota_plafond`     bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `id_quota`             bigint(20) unsigned NOT NULL COMMENT 'Regle datee (ref_quotas)',
    `id_categorie_quota`   bigint(20) unsigned NOT NULL COMMENT 'Categorie plafonnee',
    `plafond`              int(11) NOT NULL COMMENT 'Nombre de seances remboursees par periode',
    PRIMARY KEY (`id_quota_plafond`),
    UNIQUE KEY `uq_ref_quotas_plafonds` (`id_quota`, `id_categorie_quota`),
    KEY `IDX_ref_quotas_plafonds_categorie` (`id_categorie_quota`),
    CONSTRAINT `fk_ref_quotas_plafonds_quota` FOREIGN KEY (`id_quota`)
        REFERENCES `ref_quotas` (`id_quota`) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT `fk_ref_quotas_plafonds_categorie` FOREIGN KEY (`id_categorie_quota`)
        REFERENCES `ref_categories_quota` (`id_categorie_quota`) ON UPDATE CASCADE,
    CONSTRAINT `chk_ref_quotas_plafonds_positif` CHECK (`plafond` >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Plafond par categorie pour une regle de quota datee';

ALTER TABLE `ref_quotas_plafonds` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_uca1400_ai_ci;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Colonnes ajoutées
-- ---------------------------------------------------------------------------

DROP PROCEDURE IF EXISTS tmp_lot23_colonnes;

DELIMITER //
CREATE PROCEDURE tmp_lot23_colonnes()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_quotas'
          AND COLUMN_NAME = 'duree_periode_mois') = 0 THEN
        ALTER TABLE `ref_quotas`
            ADD COLUMN `duree_periode_mois` int(11) NOT NULL DEFAULT 12
                COMMENT 'Duree de la periode glissante ouverte a la 1re seance consommante' AFTER `seuil_alerte_total`,
            ADD CONSTRAINT `chk_ref_quotas_duree` CHECK (`duree_periode_mois` > 0);
    END IF;

    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_natures_seance'
          AND COLUMN_NAME = 'id_categorie_quota') = 0 THEN
        ALTER TABLE `ref_natures_seance`
            ADD COLUMN `id_categorie_quota` bigint(20) unsigned NULL DEFAULT NULL
                COMMENT 'Categorie de quota consommee (NULL = hors quota)' AFTER `code_prestation_inami`,
            ADD KEY `IDX_ref_natures_seance_categorie` (`id_categorie_quota`),
            ADD CONSTRAINT `fk_ref_natures_seance_categorie` FOREIGN KEY (`id_categorie_quota`)
                REFERENCES `ref_categories_quota` (`id_categorie_quota`) ON UPDATE CASCADE;
    END IF;
END //
DELIMITER ;

CALL tmp_lot23_colonnes();
DROP PROCEDURE IF EXISTS tmp_lot23_colonnes;

-- ---------------------------------------------------------------------------
-- SECTION 4 — Conversion des données existantes
-- ---------------------------------------------------------------------------

-- 4.1 Catégories : pour chaque domaine possédant une règle de quota
INSERT INTO `ref_categories_quota` (`id_domaine`, `code_categorie`, `libelle_categorie`, `actif`, `ordre_affichage`)
SELECT DISTINCT q.`id_domaine`, 'SOUTIEN', 'Soutien', 1, 10
FROM `ref_quotas` q
WHERE NOT EXISTS (SELECT 1 FROM `ref_categories_quota` c
                  WHERE c.`id_domaine` = q.`id_domaine` AND c.`code_categorie` = 'SOUTIEN');

INSERT INTO `ref_categories_quota` (`id_domaine`, `code_categorie`, `libelle_categorie`, `actif`, `ordre_affichage`)
SELECT DISTINCT q.`id_domaine`, 'TRAITEMENT', 'Traitement', 1, 20
FROM `ref_quotas` q
WHERE NOT EXISTS (SELECT 1 FROM `ref_categories_quota` c
                  WHERE c.`id_domaine` = q.`id_domaine` AND c.`code_categorie` = 'TRAITEMENT');

-- 4.2 Plafonds : reprise des colonnes plafond_soutien / plafond_traitement
INSERT INTO `ref_quotas_plafonds` (`id_quota`, `id_categorie_quota`, `plafond`)
SELECT q.`id_quota`, c.`id_categorie_quota`, q.`plafond_soutien`
FROM `ref_quotas` q
JOIN `ref_categories_quota` c ON c.`id_domaine` = q.`id_domaine` AND c.`code_categorie` = 'SOUTIEN'
WHERE NOT EXISTS (SELECT 1 FROM `ref_quotas_plafonds` p
                  WHERE p.`id_quota` = q.`id_quota` AND p.`id_categorie_quota` = c.`id_categorie_quota`);

INSERT INTO `ref_quotas_plafonds` (`id_quota`, `id_categorie_quota`, `plafond`)
SELECT q.`id_quota`, c.`id_categorie_quota`, q.`plafond_traitement`
FROM `ref_quotas` q
JOIN `ref_categories_quota` c ON c.`id_domaine` = q.`id_domaine` AND c.`code_categorie` = 'TRAITEMENT'
WHERE NOT EXISTS (SELECT 1 FROM `ref_quotas_plafonds` p
                  WHERE p.`id_quota` = q.`id_quota` AND p.`id_categorie_quota` = c.`id_categorie_quota`);

-- 4.3 Natures : rattachement à la catégorie du domaine soumis à quota auquel la
--     nature est liée (lia_natures_seance_domaines), selon code_prestation_inami.
UPDATE `ref_natures_seance` ns
JOIN `lia_natures_seance_domaines` l ON l.`id_nature_seance` = ns.`id_nature_seance`
JOIN `ref_domaines` d ON d.`id_domaine` = l.`id_domaine` AND d.`soumis_quota` = 1
JOIN `ref_categories_quota` c ON c.`id_domaine` = d.`id_domaine`
                              AND c.`code_categorie` = ns.`code_prestation_inami`
SET ns.`id_categorie_quota` = c.`id_categorie_quota`
WHERE ns.`code_prestation_inami` IS NOT NULL
  AND ns.`id_categorie_quota` IS NULL;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 5 — Contrôle (à lire après exécution)
-- ---------------------------------------------------------------------------
-- Natures portant un code prestation mais non converties (doit retourner 0 ligne) :
SELECT id_nature_seance, code_nature_seance, code_prestation_inami
FROM ref_natures_seance
WHERE code_prestation_inami IS NOT NULL AND id_categorie_quota IS NULL;

-- ---------------------------------------------------------------------------
-- SECTION 6 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.25.0',
       'Lot 23 : ref_categories_quota (par domaine), ref_quotas_plafonds, ref_quotas.duree_periode_mois, ref_natures_seance.id_categorie_quota ; conversion SOUTIEN/TRAITEMENT. Migration additive.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.25.0');
