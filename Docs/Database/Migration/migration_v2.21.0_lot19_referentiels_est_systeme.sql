-- =============================================================================
-- ALTHÉA — Migration v2.21.0 (Lot 19 — Protection des lignes système)
-- =============================================================================
--
-- Objet :
--   1. Ajout de la colonne est_systeme aux référentiels dont certains codes
--      sont exploités par l'application (statuts, modes, types de document...).
--   2. Marquage des lignes système existantes (est_systeme = 1).
--   3. Journalisation de la version dans tec_meta_schema.
--
-- Règles :
--   - Une ligne est_systeme = 1 ne peut ni changer de code ni être désactivée
--     depuis les écrans de référentiels ; libellé et ordre restent modifiables.
--   - La colonne n'est posée que par migration (jamais depuis l'interface).
--   - Script rejouable (contrôles d'existence).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Procédure temporaire d'ajout de colonne
-- ---------------------------------------------------------------------------

DROP PROCEDURE IF EXISTS tmp_ajouter_est_systeme;

DELIMITER //
CREATE PROCEDURE tmp_ajouter_est_systeme(IN p_table VARCHAR(64))
BEGIN
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = p_table
          AND COLUMN_NAME  = 'est_systeme') = 0 THEN
        SET @sql_col = CONCAT('ALTER TABLE `', p_table,
            '` ADD COLUMN `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT ''1 = ligne utilisee par l application : code et activation verrouilles''');
        PREPARE stmt_col FROM @sql_col;
        EXECUTE stmt_col;
        DEALLOCATE PREPARE stmt_col;
    END IF;
END //
DELIMITER ;

CALL tmp_ajouter_est_systeme('ref_statuts_seance');
CALL tmp_ajouter_est_systeme('ref_statuts_dossier');
CALL tmp_ajouter_est_systeme('ref_modes_paiement');
CALL tmp_ajouter_est_systeme('ref_types_documents');
CALL tmp_ajouter_est_systeme('ref_liens_patient');
CALL tmp_ajouter_est_systeme('ref_role_legal');
CALL tmp_ajouter_est_systeme('ref_prix_seance');

DROP PROCEDURE IF EXISTS tmp_ajouter_est_systeme;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Marquage des lignes système
-- ---------------------------------------------------------------------------

UPDATE ref_statuts_seance SET est_systeme = 1
WHERE code_statut_seance IN ('PLANIFIEE', 'REALISEE', 'ABSENTE', 'REPORTEE',
                             'ANNULEE_PATIENT', 'ANNULEE_PRATICIEN', 'MALADE', 'NO_SHOW');

UPDATE ref_statuts_dossier SET est_systeme = 1
WHERE code_statut_dossier IN ('ACTIF', 'PAUSE', 'CLOTURE', 'ARCHIVE');

UPDATE ref_modes_paiement SET est_systeme = 1
WHERE code_mode_paiement = 'AVOIR';

UPDATE ref_types_documents SET est_systeme = 1
WHERE code_type_document IN ('ANAMNESE', 'ZONE_CLINIQUE', 'PHOTO_IDENTITE');

UPDATE ref_liens_patient SET est_systeme = 1
WHERE code_lien_patient = 'MERE';

UPDATE ref_role_legal SET est_systeme = 1
WHERE code_role_legal = 'AUTORITE_PARENTALE';

UPDATE ref_prix_seance SET est_systeme = 1
WHERE code_prix_seance = 'INAMI';

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Journalisation de la version
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.21.0',
       'Lot 19 : ajout de est_systeme aux referentiels utilises par l application et marquage des lignes systeme (code et activation verrouilles a l ecran).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.21.0');

-- =============================================================================
-- FIN DE LA MIGRATION v2.21.0
-- =============================================================================
