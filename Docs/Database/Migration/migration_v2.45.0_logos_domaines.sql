-- ---------------------------------------------------------------------------
-- Migration v2.45.0 - Logo facultatif des domaines
--
-- Chaque domaine peut recevoir un logo (décision de Joëlle, 20/10/2026 : écran
-- Domaines, convention du domaine, bandeau et liste des domaines du dossier).
--
-- 1. Colonne ref_domaines.logo_fichier : nom du fichier seul (« Logo.png »).
--    Le fichier est rangé dans le dossier de données, sous
--    {PATH_GENERAL}/Domaines/Domaine_{id}/ (nommé par l'identifiant : le libellé
--    peut changer) ; il est donc compris dans les sauvegardes.
--
-- Règles : script rejouable ; aucune donnée existante touchée (NULL = pas de
-- logo). Retour arrière : migration_v2.45.0_logos_domaines_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

ALTER TABLE `ref_domaines`
  ADD COLUMN IF NOT EXISTS `logo_fichier` varchar(100) DEFAULT NULL
    COMMENT 'Nom du fichier logo (NULL = aucun) ; fichier dans {PATH_GENERAL}/Domaines/Domaine_{id}/'
    AFTER `ordre_affichage`;

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.45.0',
       'Logo facultatif des domaines : colonne ref_domaines.logo_fichier.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.45.0');
