-- =============================================================================
-- ALTHÉA — Migration v2.23.0 (Lot 21 — Référentiel des organismes)
-- =============================================================================
--
-- Objet :
--   1. Création du référentiel ref_organismes (organismes de remboursement).
--   2. Ajout de ref_domaines.id_organisme, FACULTATIF (NULL = aucun organisme).
--   3. Initialisation : organisme INAMI (ligne système) rattaché au domaine Realism.
--   4. Journalisation de la version dans tec_meta_schema.
--
-- Règles :
--   - Un domaine sans organisme fonctionne simplement (aucun changement de comportement).
--   - Aucun comportement n'est déduit du code de l'organisme : les règles restent portées
--     par les indicateurs du domaine (tarif_convention, no_show_facturable, soumis_quota).
--   - Script rejouable (contrôles d'existence).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Table ref_organismes
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_organismes` (
    `id_organisme`       bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `code_organisme`     varchar(30)  NOT NULL,
    `libelle_organisme`  varchar(100) NOT NULL,
    `est_systeme`        tinyint(1)   NOT NULL DEFAULT 0 COMMENT 'Ligne utilisee par l''application : code non modifiable, non desactivable',
    `actif`              tinyint(1)   NOT NULL DEFAULT 1,
    `ordre_affichage`    int(11)      NOT NULL DEFAULT 0,
    PRIMARY KEY (`id_organisme`),
    UNIQUE KEY `uq_ref_organismes_code` (`code_organisme`),
    UNIQUE KEY `uq_ref_organismes_libelle` (`libelle_organisme`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
  COMMENT='Organismes de remboursement (ex. INAMI), rattaches facultativement aux domaines';

INSERT INTO `ref_organismes` (`code_organisme`, `libelle_organisme`, `est_systeme`, `actif`, `ordre_affichage`)
SELECT 'INAMI', 'INAMI', 1, 1, 10
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_organismes` WHERE `code_organisme` = 'INAMI');

-- ---------------------------------------------------------------------------
-- SECTION 2 — Colonne facultative ref_domaines.id_organisme
-- ---------------------------------------------------------------------------

DROP PROCEDURE IF EXISTS tmp_ajouter_organisme_domaine;

DELIMITER //
CREATE PROCEDURE tmp_ajouter_organisme_domaine()
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ref_domaines'
          AND COLUMN_NAME = 'id_organisme') = 0 THEN

        ALTER TABLE `ref_domaines`
            ADD COLUMN `id_organisme` bigint(20) unsigned NULL DEFAULT NULL
                COMMENT 'Organisme de remboursement (NULL = aucun organisme)' AFTER `libelle_domaine`,
            ADD KEY `IDX_ref_domaines_organisme` (`id_organisme`),
            ADD CONSTRAINT `fk_ref_domaines_organisme` FOREIGN KEY (`id_organisme`)
                REFERENCES `ref_organismes` (`id_organisme`) ON UPDATE CASCADE;
    END IF;
END //
DELIMITER ;

CALL tmp_ajouter_organisme_domaine();
DROP PROCEDURE IF EXISTS tmp_ajouter_organisme_domaine;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Rattachement initial : Realism -> INAMI
-- ---------------------------------------------------------------------------

UPDATE `ref_domaines` d
JOIN `ref_organismes` o ON o.`code_organisme` = 'INAMI'
SET d.`id_organisme` = o.`id_organisme`
WHERE d.`id_organisme` IS NULL
  AND (d.`code_domaine` = 'REA' OR d.`libelle_domaine` = 'Realism');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 4 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.23.0',
       'Lot 21 : referentiel ref_organismes (INAMI systeme) ; ref_domaines.id_organisme facultatif (NULL = aucun organisme) ; Realism rattache a INAMI.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.23.0');
