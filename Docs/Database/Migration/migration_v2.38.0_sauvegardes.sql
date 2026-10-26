-- ---------------------------------------------------------------------------
-- Migration v2.38.0 - Lot 30, étape P2 : sauvegardes
--
-- 1. tec_sauvegardes : journal des sauvegardes (une ligne par tentative, créée
--    au démarrage puis complétée : réussie ou en échec). Sert à l'historique de
--    l'écran Sauvegardes et au rappel « dernière sauvegarde réussie il y a N jours ».
--    Le fichier est désigné par son nom et son dossier au moment de la sauvegarde
--    (trace : le dossier peut changer ensuite).
-- 2. Paramètres du groupe SAUVEGARDE (lus par le code, ADR-36) :
--    - SAUVEGARDE_DOSSIER        : dossier des archives (chemin complet, hors
--                                  dossier de données) ; vide = pas encore choisi
--    - SAUVEGARDE_NB_CONSERVEES  : nombre d'archives gardées (rotation), 10
--    - SAUVEGARDE_RAPPEL_JOURS   : délai du rappel, 7 jours
--    - SAUVEGARDE_DOSSIER_MARIADB: dossier bin de MariaDB, recours si l'outil
--                                  mariadb-dump n'est pas trouvé automatiquement
--
-- Le mot de passe des archives n'est PAS en base : il est conservé chiffré par
-- Windows (DPAPI) dans la configuration locale du poste.
--
-- Règles : script rejouable ; aucune donnée existante modifiée.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- 1. Journal
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `tec_sauvegardes` (
    `id_sauvegarde`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `date_debut`         datetime     NOT NULL DEFAULT current_timestamp(),
    `date_fin`           datetime     NULL,
    `statut_sauvegarde`  varchar(20)  NOT NULL DEFAULT 'EN_COURS' COMMENT 'EN_COURS, REUSSIE, ECHEC (StatutSauvegarde)',
    `nom_fichier`        varchar(255) NULL COMMENT 'Nom de l archive (Althea_AAAAMMJJ_HHMMSS.zip)',
    `dossier_sauvegarde` varchar(500) NULL COMMENT 'Dossier au moment de la sauvegarde',
    `taille_octets`      bigint(20) unsigned NULL,
    `nb_fichiers`        int(11) unsigned NULL COMMENT 'Fichiers du dossier de donnees inclus',
    `version_schema`     varchar(20)  NULL COMMENT 'Derniere migration appliquee au moment de la sauvegarde',
    `version_application` varchar(30) NULL,
    `id_utilisateur`     bigint(20) unsigned NULL,
    `login_utilisateur`  varchar(100) NULL COMMENT 'Trace : login au moment de la sauvegarde',
    `machine`            varchar(100) NULL,
    `message_erreur`     text         NULL COMMENT 'Cause de l echec, sans donnee sensible',
    PRIMARY KEY (`id_sauvegarde`),
    KEY `IDX_tec_sauvegardes_statut_date` (`statut_sauvegarde`, `date_debut`),
    KEY `IDX_tec_sauvegardes_utilisateur` (`id_utilisateur`),
    CONSTRAINT `fk_tec_sauvegardes_utilisateur` FOREIGN KEY (`id_utilisateur`)
        REFERENCES `sec_utilisateurs` (`id_utilisateur`) ON UPDATE CASCADE ON DELETE SET NULL,
    CONSTRAINT `chk_tec_sauvegardes_statut` CHECK (`statut_sauvegarde` IN ('EN_COURS', 'REUSSIE', 'ECHEC'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Journal des sauvegardes (lot 30)';

-- ---------------------------------------------------------------------------
-- 2. Paramètres
-- ---------------------------------------------------------------------------

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SAUVEGARDE_DOSSIER', 'Dossier des sauvegardes', 'SAUVEGARDE', 'PATH',
       NULL, NULL, NULL,
       'Dossier (chemin complet) où sont rangées les archives de sauvegarde. Refusé s''il est à l''intérieur du dossier de données ; idéalement sur un autre disque (disque externe). Vide = pas encore choisi.',
       1, 1, 10
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SAUVEGARDE_DOSSIER');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SAUVEGARDE_NB_CONSERVEES', 'Nombre de sauvegardes conservées', 'SAUVEGARDE', 'INT',
       '10', 1, 100,
       'Après une sauvegarde réussie, les archives les plus anciennes au-delà de ce nombre sont supprimées du dossier des sauvegardes.',
       1, 1, 20
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SAUVEGARDE_NB_CONSERVEES');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SAUVEGARDE_RAPPEL_JOURS', 'Rappel de sauvegarde (jours)', 'SAUVEGARDE', 'INT',
       '7', 1, 90,
       'À l''ouverture de l''application, un rappel s''affiche si la dernière sauvegarde réussie date de plus de ce nombre de jours.',
       1, 1, 30
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SAUVEGARDE_RAPPEL_JOURS');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SAUVEGARDE_DOSSIER_MARIADB', 'Dossier des outils MariaDB (recours)', 'SAUVEGARDE', 'PATH',
       NULL, NULL, NULL,
       'À renseigner seulement si l''outil mariadb-dump n''est pas trouvé automatiquement dans Program Files\\MariaDB… : indiquer son dossier bin.',
       0, 1, 40
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SAUVEGARDE_DOSSIER_MARIADB');

-- ---------------------------------------------------------------------------
-- Journalisation
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.38.0',
       'Lot 30 (P2) : journal tec_sauvegardes et parametres SAUVEGARDE (dossier, nombre conserve, rappel, outils MariaDB).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.38.0');
