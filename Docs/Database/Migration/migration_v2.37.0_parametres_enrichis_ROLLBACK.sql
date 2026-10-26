-- ---------------------------------------------------------------------------
-- ROLLBACK Migration v2.37.0 - Lot 30, étape P1 : paramètres enrichis
--
-- Recrée les paramètres supprimés (valeurs d'origine, identifiants nouveaux),
-- rend à PATH_GENERAL son type d'origine, supprime les nouveaux paramètres et
-- les colonnes de bornes. PATH_DOCUMENT est indispensable au code d'avant la
-- v2.37.0 (CheminsPatientHelper le lisait).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, description_parametre, modifiable_utilisateur, actif, ordre_affichage)
SELECT 'APP_DATA', 'Répertoire de l''Application', 'GENERAL', 'STRING', 'Application', 'Ceci est un test. Patatras', 1, 1, 0
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'APP_DATA');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, description_parametre, modifiable_utilisateur, actif, ordre_affichage)
SELECT 'PATH_LOG', 'Répertoire des logs', 'TECHNIQUE', 'STRING', 'Logs', 'Sous-Répertoire contenant les fichiers de log.', 1, 0, 0
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'PATH_LOG');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, description_parametre, modifiable_utilisateur, actif, ordre_affichage)
SELECT 'PATH_BACKUP', 'Répertoire des sauvegardes', 'TECHNIQUE', 'STRING', 'Backup', 'Sous-Répertoire contenant les sauvegardes applicatives.', 1, 1, 0
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'PATH_BACKUP');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, description_parametre, modifiable_utilisateur, actif, ordre_affichage)
SELECT 'PATH_DOCUMENT', 'Répertoire des documents', 'PATHS', 'STRING', 'Documents', 'Sous-Répertoire contenant les documents.', 1, 1, 0
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'PATH_DOCUMENT');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, description_parametre, modifiable_utilisateur, actif, ordre_affichage)
SELECT 'PATH_DOSSIER', 'Répertoire des dossiers patients', 'PATHS', 'STRING', 'Dossiers', 'Sous-Répertoire contenant les dossiers patients. Test', 1, 1, 0
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'PATH_DOSSIER');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, description_parametre, modifiable_utilisateur, actif, ordre_affichage)
SELECT 'TESTONG', 'Test d''onglet', 'GENERAL', 'STRING', 'Test onglet ', '', 1, 1, 0
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'TESTONG');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, description_parametre, modifiable_utilisateur, actif, ordre_affichage)
SELECT 'TESTONG2', 'Test des Onglet 2', 'TESTONGLET', 'STRING', 'Test Test', 'test de création d''onglet à la volée', 1, 1, 0
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'TESTONG2');

-- Base qui avait reçu la première version de ce script : PATH_BACKUP et PATH_LOG
-- existent encore, modifiés ; ils retrouvent leur état d'origine.
UPDATE tec_parametres
SET type_valeur = 'STRING',
    groupe_parametre = 'TECHNIQUE',
    valeur_parametre = COALESCE(NULLIF(valeur_parametre, ''), 'Backup'),
    description_parametre = 'Sous-Répertoire contenant les sauvegardes applicatives.'
WHERE cle_parametre = 'PATH_BACKUP';

UPDATE tec_parametres SET description_parametre = 'Sous-Répertoire contenant les fichiers de log.'
WHERE cle_parametre = 'PATH_LOG';

UPDATE tec_parametres
SET type_valeur = 'STRING',
    description_parametre = 'Répertoire racine contenant toutes les données métiers (documents patients).'
WHERE cle_parametre = 'PATH_GENERAL';

DELETE FROM tec_parametres
WHERE cle_parametre IN ('SEANCE_DUREE_DEFAUT_MIN', 'SEANCE_PAS_HORAIRE_MIN', 'SEANCE_HEURE_DEBUT_GRILLE',
                        'SEANCE_HEURE_FIN_GRILLE', 'SECURITE_MAX_ECHECS_LOGIN', 'LOG_RETENTION_JOURS',
                        'CACHET_ENCRE_NOIRE');

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tec_parametres' AND COLUMN_NAME = 'valeur_max');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE tec_parametres DROP COLUMN valeur_max', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tec_parametres' AND COLUMN_NAME = 'valeur_min');
SET @sql := IF(@col_existe = 1, 'ALTER TABLE tec_parametres DROP COLUMN valeur_min', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

DELETE FROM tec_meta_schema WHERE version_schema = '2.37.0';
