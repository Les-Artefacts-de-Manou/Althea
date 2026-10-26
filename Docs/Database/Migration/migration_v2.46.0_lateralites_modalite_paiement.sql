-- ---------------------------------------------------------------------------
-- Migration v2.46.0 - Référentiel des latéralités ; modalité de paiement
--                     proposée en paramètre
--
-- Décisions de Joëlle (20/10/2026) : la latéralité du patient et la modalité de
-- paiement proposée à la création d'un dossier ne sont plus écrites dans le code.
--
-- 1. Table ref_lateralites : code, libellé, actif, ordre ;
--    valeurs de départ Droitier, Gaucher, Ambidextre (modifiables, d'autres
--    peuvent être ajoutées depuis Référentiels › Latéralités).
-- 2. Reprise des valeurs déjà saisies : toute latéralité d'un patient absente
--    de la liste devient une ligne du référentiel (rien n'est perdu).
-- 3. Colonne patients.id_lateralite (clé étrangère), renseignée d'après le texte
--    existant. La colonne texte patients.lateralite n'est plus lue par
--    l'application ; elle est CONSERVÉE (vérification avant de la retirer par
--    une migration ultérieure).
-- 4. Groupe de paramètres DOSSIERS et paramètre DOSSIER_MODALITE_PAIEMENT_DEFAUT
--    (texte, « À la séance »), proposé à la création d'un dossier.
--
-- Règles : script rejouable ; aucune donnée existante supprimée. Retour arrière :
-- migration_v2.46.0_lateralites_modalite_paiement_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- 1. Référentiel -------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ref_lateralites` (
  `id_lateralite`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_lateralite`    varchar(30)  NOT NULL,
  `libelle_lateralite` varchar(60)  NOT NULL,
  `actif`              tinyint(1)   NOT NULL DEFAULT 1,
  `ordre_affichage`    int(11)      NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_lateralite`),
  UNIQUE KEY `uq_ref_lateralites_code` (`code_lateralite`),
  UNIQUE KEY `uq_ref_lateralites_libelle` (`libelle_lateralite`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Latéralité du patient (Droitier, Gaucher, Ambidextre…), v2.46.0';

INSERT INTO ref_lateralites (code_lateralite, libelle_lateralite, actif, ordre_affichage)
SELECT v.code, v.libelle, 1, v.ordre
FROM (
    SELECT 'DROITIER' AS code, 'Droitier' AS libelle, 10 AS ordre UNION ALL
    SELECT 'GAUCHER',          'Gaucher',             20 UNION ALL
    SELECT 'AMBIDEXTRE',       'Ambidextre',          30
) AS v
WHERE NOT EXISTS (SELECT 1 FROM ref_lateralites l WHERE l.code_lateralite = v.code OR l.libelle_lateralite = v.libelle);

-- 2. Reprise des valeurs saisies hors liste ------------------------------------
-- Code : libellé en majuscules, espaces remplacés par « _ » (préfixe REPRISE_ si
-- ce code existe déjà) ; ordre après les valeurs de départ.
INSERT IGNORE INTO ref_lateralites (code_lateralite, libelle_lateralite, actif, ordre_affichage)
SELECT LEFT(UPPER(REPLACE(TRIM(p.lateralite), ' ', '_')), 30), LEFT(TRIM(p.lateralite), 60), 1, 100
FROM patients p
WHERE TRIM(COALESCE(p.lateralite, '')) <> ''
  AND NOT EXISTS (SELECT 1 FROM ref_lateralites l WHERE l.libelle_lateralite = TRIM(p.lateralite))
GROUP BY TRIM(p.lateralite);

INSERT IGNORE INTO ref_lateralites (code_lateralite, libelle_lateralite, actif, ordre_affichage)
SELECT LEFT(CONCAT('REPRISE_', UPPER(REPLACE(TRIM(p.lateralite), ' ', '_'))), 30), LEFT(TRIM(p.lateralite), 60), 1, 100
FROM patients p
WHERE TRIM(COALESCE(p.lateralite, '')) <> ''
  AND NOT EXISTS (SELECT 1 FROM ref_lateralites l WHERE l.libelle_lateralite = TRIM(p.lateralite))
GROUP BY TRIM(p.lateralite);

-- 3. Lien patient → latéralité ------------------------------------------------
ALTER TABLE `patients`
  ADD COLUMN IF NOT EXISTS `id_lateralite` bigint(20) unsigned DEFAULT NULL
    COMMENT 'Latéralité → ref_lateralites (v2.46.0) ; remplace la colonne texte lateralite'
    AFTER `lateralite`;

ALTER TABLE `patients`
  ADD CONSTRAINT `fk_patients_ref_lateralites` FOREIGN KEY IF NOT EXISTS (`id_lateralite`)
    REFERENCES `ref_lateralites` (`id_lateralite`);

UPDATE patients p
JOIN ref_lateralites l ON l.libelle_lateralite = TRIM(p.lateralite)
SET p.id_lateralite = l.id_lateralite
WHERE p.id_lateralite IS NULL
  AND TRIM(COALESCE(p.lateralite, '')) <> '';

-- 4. Modalité de paiement proposée ----------------------------------------------
INSERT INTO tec_groupes_parametres (code_groupe, libelle_groupe, ordre_affichage)
SELECT 'DOSSIERS', 'Dossiers', 25
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_groupes_parametres WHERE code_groupe = 'DOSSIERS');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'DOSSIER_MODALITE_PAIEMENT_DEFAUT', 'Modalités de paiement proposées', 'DOSSIERS', 'STRING',
       'À la séance', NULL, NULL,
       'Texte proposé dans « Modalités de paiement » à la création d''un dossier ; il reste modifiable dans chaque dossier. Vide = rien n''est proposé.',
       1, 1, 10
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'DOSSIER_MODALITE_PAIEMENT_DEFAUT');

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.46.0',
       'Referentiel ref_lateralites, patients.id_lateralite ; groupe DOSSIERS et parametre DOSSIER_MODALITE_PAIEMENT_DEFAUT.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.46.0');
