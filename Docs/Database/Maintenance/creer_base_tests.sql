-- =================================================================================================
-- Script   : creer_base_tests.sql
-- Projet   : Althéa
-- Date     : 26/10/2026
-- Rôle     : Crée la base vide « althea_tests » et son compte MariaDB, pour le test de fumée des
--            requêtes (tests automatisés, audit D-2, ADR-58). À exécuter UNE fois par poste de
--            développement, en administrateur (root), par exemple dans HeidiSQL.
--
-- MODE D'EMPLOI
--   1. Remplacer <MOT_DE_PASSE> ci-dessous par un mot de passe choisi pour ce compte de test
--      (ne pas enregistrer le script modifié, ne jamais le commiter).
--   2. Exécuter tout le script.
--   3. Déclarer la chaîne de connexion dans une variable d'environnement Windows de l'utilisateur
--      (même mot de passe), en PowerShell — une commande par ligne, apostrophes comprises :
--        [Environment]::SetEnvironmentVariable("ALTHEA_TESTS_CONNEXION", 'Server=localhost;Port=3306;Database=althea_tests;User ID=althea_tests;Password=<MOT_DE_PASSE>', "User")
--        $env:ALTHEA_TESTS_CONNEXION = [Environment]::GetEnvironmentVariable("ALTHEA_TESTS_CONNEXION", "User")
--      La 1re l'enregistre pour toujours, la 2e la rend visible dans la fenêtre ouverte. Visual
--      Studio, déjà ouvert, ne la voit qu'après avoir été fermé puis rouvert.
--      Vérifier (sans afficher le mot de passe) :
--        if ([Environment]::GetEnvironmentVariable("ALTHEA_TESTS_CONNEXION", "User")) { "enregistree" } else { "absente" }
--   4. Lancer Tools\test.cmd : la structure est recréée dans althea_tests à chaque série de tests,
--      depuis Docs/Database/backup_NoData_althea.sql (aucune donnée).
--
-- Règles :
--   - Base dédiée et vide : jamais de données de patients, même fictives.
--   - Le compte « althea_tests » n'a de droits que sur cette base ; les tests refusent toute autre
--     base (ils y suppriment puis recréent toutes les tables).
--   - Pour tout retirer : DROP DATABASE althea_tests; DROP USER 'althea_tests'@'localhost';
--     puis supprimer la variable ALTHEA_TESTS_CONNEXION (Paramètres système › Variables
--     d'environnement).
-- =================================================================================================

CREATE DATABASE IF NOT EXISTS althea_tests
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_uca1400_ai_ci;

CREATE USER IF NOT EXISTS 'althea_tests'@'localhost' IDENTIFIED BY '<MOT_DE_PASSE>';

GRANT ALL PRIVILEGES ON althea_tests.* TO 'althea_tests'@'localhost';

-- Contrôle : droits du compte de test (une seule base).
SHOW GRANTS FOR 'althea_tests'@'localhost';
