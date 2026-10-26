-- ---------------------------------------------------------------------------
-- Migration v2.37.0 - Lot 30, étape P1 : paramètres enrichis
--
-- 1. tec_parametres.valeur_min / valeur_max : bornes des paramètres numériques,
--    appliquées à la saisie (UC_Parametres) et à la lecture (GestionParametres).
-- 2. Nouveaux paramètres, jusqu'ici écrits dans le code :
--    - SEANCE_DUREE_DEFAUT_MIN    (45, était GestionSeances.DUREE_SEANCE_MINUTES)
--    - SEANCE_PAS_HORAIRE_MIN     (15, pas de la grille d'heures de la fiche séance)
--    - SEANCE_HEURE_DEBUT_GRILLE  (8) et SEANCE_HEURE_FIN_GRILLE (20) : plage proposée
--    - SECURITE_MAX_ECHECS_LOGIN  (5, était une constante de GestionAuthentification)
--    - LOG_RETENTION_JOURS        (7, était une constante de GestionLog)
--    - CACHET_ENCRE_NOIRE         (0 = bleu nuit, décision du 11/10/2026)
-- 3. PATH_GENERAL passe au type PATH (bouton « Parcourir… », contrôle d'existence)
--    et sa description précise qu'il est obligatoire et ne déplace pas les fichiers.
-- 4. Suppression des paramètres obsolètes (accord de Joëlle le 12/10/2026, base
--    de développement ; le ROLLBACK les recrée) :
--    - TESTONG, TESTONG2, APP_DATA : paramètres de test ;
--    - PATH_DOCUMENT : le sous-dossier « Documents » devient une constante
--      (ArborescenceDonnees) — il est inscrit dans documents.chemin_relatif et
--      ne doit jamais changer ;
--    - PATH_DOSSIER, PATH_LOG, PATH_BACKUP : jamais utilisés par le code (le
--      dossier des sauvegardes sera créé avec le lot 30, étape P2).
--
-- Règles : script rejouable, y compris sur une base qui a reçu la première
-- version de ce script (même état final) ; aucune donnée métier touchée.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- 1. Bornes
-- ---------------------------------------------------------------------------

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tec_parametres' AND COLUMN_NAME = 'valeur_min');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE tec_parametres ADD COLUMN valeur_min decimal(18,4) NULL
        COMMENT ''Borne basse (INT / DECIMAL) ; NULL = sans borne'' AFTER valeur_parametre',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tec_parametres' AND COLUMN_NAME = 'valeur_max');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE tec_parametres ADD COLUMN valeur_max decimal(18,4) NULL
        COMMENT ''Borne haute (INT / DECIMAL) ; NULL = sans borne'' AFTER valeur_min',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- 2. Nouveaux paramètres
-- ---------------------------------------------------------------------------

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SEANCE_DUREE_DEFAUT_MIN', 'Durée par défaut d''une séance (minutes)', 'SEANCES', 'INT',
       '45', 10, 240,
       'Heure de fin proposée = heure de début + cette durée (fiche séance, report de séance). Les séances s''enchaînent sans battement.',
       1, 1, 10
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SEANCE_DUREE_DEFAUT_MIN');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SEANCE_PAS_HORAIRE_MIN', 'Pas des heures proposées (minutes)', 'SEANCES', 'INT',
       '15', 5, 60,
       'Écart entre deux heures des listes de la fiche séance. Choisir un pas qui divise la durée par défaut (45 min : 15 ; 50 min : 10) ; sinon il est réduit automatiquement pour rester cohérent.',
       1, 1, 20
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SEANCE_PAS_HORAIRE_MIN');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SEANCE_HEURE_DEBUT_GRILLE', 'Première heure proposée', 'SEANCES', 'INT',
       '8', 0, 23,
       'Heure (entière) de début des listes d''heures de la fiche séance. Une autre heure reste saisissable.',
       1, 1, 30
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SEANCE_HEURE_DEBUT_GRILLE');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SEANCE_HEURE_FIN_GRILLE', 'Dernière heure proposée', 'SEANCES', 'INT',
       '20', 1, 23,
       'Heure (entière) de fin des listes d''heures de la fiche séance ; doit être après la première heure.',
       1, 1, 40
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SEANCE_HEURE_FIN_GRILLE');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SECURITE_MAX_ECHECS_LOGIN', 'Échecs de connexion avant verrouillage', 'SECURITE', 'INT',
       '5', 3, 20,
       'Nombre de mots de passe erronés consécutifs après lequel le compte est verrouillé (déverrouillage par un Admin).',
       0, 1, 10
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SECURITE_MAX_ECHECS_LOGIN');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'LOG_RETENTION_JOURS', 'Conservation des journaux (jours)', 'TECHNIQUE', 'INT',
       '7', 1, 90,
       'Les fichiers de log plus anciens sont supprimés au démarrage. Pris en compte au prochain lancement.',
       0, 1, 20
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'LOG_RETENTION_JOURS');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'CACHET_ENCRE_NOIRE', 'Cachet généré à l''encre noire', 'CABINET', 'BOOL',
       '0', NULL, NULL,
       'Décoché : encre bleu nuit (par défaut). Coché : encre noire. S''applique au prochain cachet généré.',
       1, 1, 10
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'CACHET_ENCRE_NOIRE');

-- ---------------------------------------------------------------------------
-- 3. Racine des données
-- ---------------------------------------------------------------------------

UPDATE tec_parametres
SET type_valeur = 'PATH',
    description_parametre = 'Dossier (chemin complet) qui contient toutes les données métier : Documents, Cabinet, Utilisateurs. Obligatoire. Le modifier ne déplace pas les fichiers : déplacer d''abord le dossier, puis indiquer ici son nouvel emplacement.'
WHERE cle_parametre = 'PATH_GENERAL';

-- ---------------------------------------------------------------------------
-- 4. Paramètres obsolètes
-- ---------------------------------------------------------------------------

DELETE FROM tec_parametres
WHERE cle_parametre IN ('TESTONG', 'TESTONG2', 'APP_DATA',
                        'PATH_DOCUMENT', 'PATH_DOSSIER', 'PATH_LOG', 'PATH_BACKUP');

-- ---------------------------------------------------------------------------
-- Journalisation
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.37.0',
       'Lot 30 (P1) : bornes, duree et grille des seances, echecs de connexion, conservation des logs, encre du cachet, type PATH, suppression des parametres obsoletes.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.37.0');

-- Base où une première version de ce script (12/10/2026, avant l'ajout de la grille et
-- des suppressions de chemins) a déjà été appliquée : description mise à jour.
UPDATE tec_meta_schema
SET description = 'Lot 30 (P1) : bornes, duree et grille des seances, echecs de connexion, conservation des logs, encre du cachet, type PATH, suppression des parametres obsoletes.'
WHERE version_schema = '2.37.0';
