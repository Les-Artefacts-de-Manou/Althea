-- ---------------------------------------------------------------------------
-- Migration v2.39.0 - Lot 30, étape P3 : restaurations
--
-- tec_restaurations : journal des restaurations (une ligne par tentative).
--   - Une restauration réussie remplace toute la base par celle de l'archive :
--     sa ligne est donc écrite APRÈS le rechargement, dans la base restaurée.
--   - Une restauration arrêtée avant de toucher à la base (contrôles, sauvegarde
--     de sécurité, extraction) est journalisée en ECHEC dans la base actuelle.
--   - Pas de clé étrangère vers sec_utilisateurs : après restauration, les
--     identifiants des comptes sont ceux de l'archive et ne désignent plus
--     forcément la personne qui a restauré. Le login est conservé en trace.
--
-- Règles : script rejouable ; aucune donnée existante modifiée.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

CREATE TABLE IF NOT EXISTS `tec_restaurations` (
    `id_restauration`          bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `date_debut`               datetime     NOT NULL DEFAULT current_timestamp(),
    `date_fin`                 datetime     NULL,
    `statut_restauration`      varchar(20)  NOT NULL COMMENT 'REUSSIE, ECHEC (StatutRestauration)',
    `nom_archive`              varchar(255) NULL COMMENT 'Archive restauree (Althea_AAAAMMJJ_HHMMSS.zip)',
    `dossier_archive`          varchar(500) NULL COMMENT 'Dossier de l archive au moment de la restauration',
    `date_archive`             datetime     NULL COMMENT 'Date de la sauvegarde restauree (manifeste)',
    `version_schema_archive`   varchar(20)  NULL,
    `machine_archive`          varchar(100) NULL COMMENT 'Poste qui avait fait la sauvegarde',
    `nb_fichiers`              int(11) unsigned NULL COMMENT 'Fichiers du dossier de donnees restaures',
    `archive_securite`         varchar(255) NULL COMMENT 'Sauvegarde de securite faite juste avant',
    `dossier_donnees_precedent` varchar(500) NULL COMMENT 'Ancien dossier de donnees, renomme et conserve',
    `login_utilisateur`        varchar(100) NULL COMMENT 'Trace : compte qui a restaure',
    `machine`                  varchar(100) NULL,
    `version_application`      varchar(30)  NULL,
    `message_erreur`           text         NULL COMMENT 'Cause de l echec, sans donnee sensible',
    PRIMARY KEY (`id_restauration`),
    KEY `IDX_tec_restaurations_date` (`date_debut`),
    CONSTRAINT `chk_tec_restaurations_statut` CHECK (`statut_restauration` IN ('REUSSIE', 'ECHEC'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Journal des restaurations (lot 30)';

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.39.0',
       'Lot 30 (P3) : journal tec_restaurations.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.39.0');
