-- ---------------------------------------------------------------------------
-- Migration v2.36.0 - Suite du lot 28 : photo de l'utilisateur
--
-- sec_utilisateurs.photo_fichier : nom seul du fichier photo du compte
-- (chemin reconstruit : {PATH_GENERAL}/Utilisateurs/Utilisateur_{id}/), sur le
-- modèle de patients.photo_fichier.
--
-- Règle d'affichage (code, PhotosUtilisateur) : un compte lié à un praticien
-- affiche la photo de la fiche praticien ; sinon sa photo propre ; à défaut,
-- ses initiales. La colonne ne sert donc qu'aux comptes non liés.
--
-- Règles : script rejouable ; aucune ligne existante modifiée (colonne NULL).
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

SET @col_existe := (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'sec_utilisateurs' AND COLUMN_NAME = 'photo_fichier');
SET @sql := IF(@col_existe = 0,
    'ALTER TABLE sec_utilisateurs ADD COLUMN photo_fichier varchar(255) NULL
        COMMENT ''Nom seul du fichier photo ; chemin reconstruit (Utilisateurs/Utilisateur_{id}) ; compte lie a un praticien : photo du praticien'' AFTER nom_affichage',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.36.0',
       'Suite lot 28 : sec_utilisateurs.photo_fichier (photo propre des comptes non lies a un praticien).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.36.0');
