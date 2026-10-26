-- =================================================================================================
-- Script   : reinit_tec_parametres.sql
-- Projet   : Althéa
-- Date     : 12/10/2026 — mise à jour 16/10/2026 (groupes des paramètres, v2.40.0) — 19/10/2026
--            (mention de confidentialité des documents, v2.41.0) — 20/10/2026 (correcteur
--            d'orthographe de l'éditeur, v2.44.0) — 20/10/2026 (modalités de paiement proposées,
--            v2.46.0) — 20/10/2026 (fin de l'élévation après inactivité, v2.48.0)
-- Rôle     : Remet tec_parametres à son état de référence : tous les paramètres sont effacés,
--            la séquence repart à 1 (PAR000001…) et seuls les paramètres lus par le code sont
--            recréés, avec leur définition et leur valeur par défaut (ADR-36).
--
-- Contexte :
--   - Décision du 12/10/2026 (base de développement) : après les essais de l'écran Paramètres,
--     des lignes avaient été écrasées (bug corrigé) ; plutôt que de réparer ligne à ligne, on
--     repart d'une table propre.
--   - Ce jeu de paramètres est la RÉFÉRENCE : il servira aux scripts d'installation
--     (préparation de l'installation, ToDo). Tout nouveau paramètre lu par le code s'ajoute ici
--     ET dans une migration.
--
-- Ce qui est conservé : les chemins propres au poste, relus avant l'effacement — PATH_GENERAL
-- (racine du dossier de données), SAUVEGARDE_DOSSIER et SAUVEGARDE_DOSSIER_MARIADB (v2.38.0).
-- Toutes les autres valeurs reprennent leur valeur par défaut.
-- Ce qui n'est pas recréé : les paramètres qu'aucun code ne lit (dont APP_MODE_TEST).
--
-- MODE D'EMPLOI
--   1. Prérequis : migrations v2.37.0 (colonnes valeur_min / valeur_max), v2.38.0, v2.40.0
--      (tec_groupes_parametres) et v2.46.0 (groupe DOSSIERS) appliquées. La première requête échoue sinon, AVANT tout effacement.
--   2. Sauvegarder la base, puis exécuter le script en une fois (même session : la variable
--      @racine doit survivre jusqu'à l'insertion).
--   3. Vérifier le SELECT de contrôle final : 16 lignes, PAR000001 à PAR000016.
--
-- Sécurité :
--   - Aucune table ne référence tec_parametres : l'effacement ne touche aucune autre donnée.
--     tec_parametres référence tec_groupes_parametres (v2.40.0) : les groupes ne sont pas effacés
--     et chaque paramètre recréé doit nommer un groupe existant.
--   - ALTER SEQUENCE valide implicitement ce qui précède (pas de retour arrière possible après) :
--     d'où la sauvegarde préalable.
-- =================================================================================================

/*!40101 SET NAMES utf8mb4 */;

-- ------------------------------------------------------------------------------------ 1. Prérequis
-- Échoue si la v2.37.0, la v2.38.0 ou la v2.40.0 n'est pas appliquée : le script s'arrête alors sans rien effacer.
SELECT valeur_min, valeur_max FROM tec_parametres LIMIT 0;
SELECT 1 FROM tec_groupes_parametres LIMIT 0;
SELECT 1 FROM tec_sauvegardes LIMIT 0;

-- Groupe DOCUMENTS (v2.41.0) : créé s'il manque, pour que la recréation ne puisse pas échouer
-- après l'effacement (clé étrangère vers tec_groupes_parametres).
INSERT INTO tec_groupes_parametres (code_groupe, libelle_groupe, ordre_affichage)
SELECT 'DOCUMENTS', 'Documents imprimés', 45
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_groupes_parametres WHERE code_groupe = 'DOCUMENTS');

-- Groupe EDITEUR (v2.44.0) : même précaution.
INSERT INTO tec_groupes_parametres (code_groupe, libelle_groupe, ordre_affichage)
SELECT 'EDITEUR', 'Éditeur de texte', 46
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_groupes_parametres WHERE code_groupe = 'EDITEUR');

-- ------------------------------------------------------ 2. Valeurs à conserver (chemins du poste)
SET @racine := (SELECT valeur_parametre FROM tec_parametres WHERE cle_parametre = 'PATH_GENERAL' LIMIT 1);
SET @dossier_sauvegarde := (SELECT valeur_parametre FROM tec_parametres WHERE cle_parametre = 'SAUVEGARDE_DOSSIER' LIMIT 1);
SET @dossier_mariadb := (SELECT valeur_parametre FROM tec_parametres WHERE cle_parametre = 'SAUVEGARDE_DOSSIER_MARIADB' LIMIT 1);

-- ------------------------------------------------------------------ 3. Effacement et séquence
DELETE FROM tec_parametres;

ALTER SEQUENCE seq_tec_parametres RESTART WITH 1;

-- --------------------------------------------------------------------------- 4. Recréation
-- Ordre d'insertion = identifiants 1 à 14.

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
VALUES
('PATH_GENERAL', 'Répertoire racine des données', 'PATHS', 'PATH',
 @racine, NULL, NULL,
 'Dossier (chemin complet) qui contient toutes les données métier : Documents, Cabinet, Utilisateurs. Obligatoire. Le modifier ne déplace pas les fichiers : déplacer d''abord le dossier, puis indiquer ici son nouvel emplacement.',
 1, 1, 10),

('SEANCE_DUREE_DEFAUT_MIN', 'Durée par défaut d''une séance (minutes)', 'SEANCES', 'INT',
 '45', 10, 240,
 'Heure de fin proposée = heure de début + cette durée (fiche séance, report de séance). Les séances s''enchaînent sans battement.',
 1, 1, 10),

('SEANCE_PAS_HORAIRE_MIN', 'Pas des heures proposées (minutes)', 'SEANCES', 'INT',
 '15', 5, 60,
 'Écart entre deux heures des listes de la fiche séance. Choisir un pas qui divise la durée par défaut (45 min : 15 ; 50 min : 10) ; sinon il est réduit automatiquement pour rester cohérent.',
 1, 1, 20),

('SEANCE_HEURE_DEBUT_GRILLE', 'Première heure proposée', 'SEANCES', 'INT',
 '8', 0, 23,
 'Heure (entière) de début des listes d''heures de la fiche séance. Une autre heure reste saisissable.',
 1, 1, 30),

('SEANCE_HEURE_FIN_GRILLE', 'Dernière heure proposée', 'SEANCES', 'INT',
 '20', 1, 23,
 'Heure (entière) de fin des listes d''heures de la fiche séance ; doit être après la première heure.',
 1, 1, 40),

('SECURITE_MAX_ECHECS_LOGIN', 'Échecs de connexion avant verrouillage', 'SECURITE', 'INT',
 '5', 3, 20,
 'Nombre de mots de passe erronés consécutifs après lequel le compte est verrouillé (déverrouillage par un Admin).',
 0, 1, 10),

('SECURITE_ELEVATION_MINUTES', 'Fin de l''élévation après inactivité (minutes)', 'SECURITE', 'INT',
 '15', 1, 240,
 'Une élévation de droits (cadenas) prend fin après ce nombre de minutes sans activité au clavier ni à la souris : la session revient au rôle de base, avec un message.',
 0, 1, 20),

('LOG_RETENTION_JOURS', 'Conservation des journaux (jours)', 'TECHNIQUE', 'INT',
 '7', 1, 90,
 'Les fichiers de log plus anciens sont supprimés au démarrage. Pris en compte au prochain lancement.',
 0, 1, 10),

('CACHET_ENCRE_NOIRE', 'Cachet généré à l''encre noire', 'CABINET', 'BOOL',
 '0', NULL, NULL,
 'Décoché : encre bleu nuit (par défaut). Coché : encre noire. S''applique au prochain cachet généré.',
 1, 1, 10),

('SAUVEGARDE_DOSSIER', 'Dossier des sauvegardes', 'SAUVEGARDE', 'PATH',
 @dossier_sauvegarde, NULL, NULL,
 'Dossier (chemin complet) où sont rangées les archives de sauvegarde. Refusé s''il est à l''intérieur du dossier de données ; idéalement sur un autre disque (disque externe). Vide = pas encore choisi.',
 1, 1, 10),

('SAUVEGARDE_NB_CONSERVEES', 'Nombre de sauvegardes conservées', 'SAUVEGARDE', 'INT',
 '10', 1, 100,
 'Après une sauvegarde réussie, les archives les plus anciennes au-delà de ce nombre sont supprimées du dossier des sauvegardes.',
 1, 1, 20),

('SAUVEGARDE_RAPPEL_JOURS', 'Rappel de sauvegarde (jours)', 'SAUVEGARDE', 'INT',
 '7', 1, 90,
 'À l''ouverture de l''application, un rappel s''affiche si la dernière sauvegarde réussie date de plus de ce nombre de jours.',
 1, 1, 30),

('SAUVEGARDE_DOSSIER_MARIADB', 'Dossier des outils MariaDB (recours)', 'SAUVEGARDE', 'PATH',
 @dossier_mariadb, NULL, NULL,
 'À renseigner seulement si l''outil mariadb-dump n''est pas trouvé automatiquement dans Program Files\\MariaDB… : indiquer son dossier bin.',
 0, 1, 40),

('DOCUMENT_MENTION_CONFIDENTIALITE', 'Mention en pied de page', 'DOCUMENTS', 'STRING',
 'Confidentiel', NULL, NULL,
 'Imprimée à gauche du pied de page des documents (impression, aperçu, PDF, Word), à côté de la date d''édition et du numéro de page. Vide = pas de mention.',
 1, 1, 10),

('EDITEUR_CORRECTEUR_ORTHOGRAPHE', 'Correcteur d''orthographe', 'EDITEUR', 'BOOL',
 '1', NULL, NULL,
 'Coché : les mots inconnus sont soulignés en rouge pendant la saisie, avec des suggestions au clic droit (correcteur de Windows, dans la langue du clavier). Décoché : pas de soulignement. S''applique aux écrans ouverts ensuite.',
 1, 1, 10),

('DOSSIER_MODALITE_PAIEMENT_DEFAUT', 'Modalités de paiement proposées', 'DOSSIERS', 'STRING',
 'À la séance', NULL, NULL,
 'Texte proposé dans « Modalités de paiement » à la création d''un dossier ; il reste modifiable dans chaque dossier. Vide = rien n''est proposé.',
 1, 1, 10);

-- ------------------------------------------------------------------------ 5. Contrôle
-- 16 lignes, codes PAR000001 à PAR000016 ; chemins du poste avec leur valeur d'origine.
SELECT id_parametre, code_parametre, cle_parametre, groupe_parametre, type_valeur,
       valeur_parametre, valeur_min, valeur_max, modifiable_utilisateur, actif
FROM tec_parametres
ORDER BY id_parametre;
