-- ---------------------------------------------------------------------------
-- Migration v2.55.0 - Référentiel des spécialités des thérapeutes
--
-- Décisions de Joëlle (23/10/2026) : la spécialité d'un thérapeute est choisie
-- dans une liste (une seule par thérapeute), complétée au besoin par le bouton
-- [+] de la fiche Thérapeute, au lieu d'être tapée librement (variantes
-- d'écriture, fautes).
--
-- 1. Table ref_specialites : code, libellé, actif, ordre ; liste de départ des
--    spécialités les plus courantes (modifiables ou désactivables depuis
--    Référentiels › Spécialités).
-- 2. Reprise des valeurs déjà saisies : toute spécialité d'un thérapeute absente
--    de la liste devient une ligne du référentiel (rien n'est perdu).
-- 3. Colonne therapeutes.id_specialite (clé étrangère), renseignée d'après le
--    texte existant. La colonne texte therapeutes.specialite n'est plus lue ni
--    écrite par l'application ; elle est CONSERVÉE (vérification avant de la
--    retirer par une migration ultérieure).
--    autres_suivis_patient.specialite n'est pas touchée : c'est la copie figée
--    de la spécialité au moment du suivi (trace).
--
-- APRÈS l'avoir appliquée, contrôle : la requête suivante doit rendre 0.
--
--   SELECT COUNT(*) AS non_reprises
--   FROM therapeutes
--   WHERE TRIM(COALESCE(specialite, '')) <> '' AND id_specialite IS NULL;
--
-- Règles : script rejouable ; aucune donnée existante supprimée. Retour arrière :
-- migration_v2.55.0_specialites_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- 1. Référentiel -------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ref_specialites` (
  `id_specialite`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_specialite`    varchar(30)  NOT NULL,
  `libelle_specialite` varchar(100) NOT NULL,
  `actif`              tinyint(1)   NOT NULL DEFAULT 1,
  `ordre_affichage`    int(11)      NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_specialite`),
  UNIQUE KEY `uq_ref_specialites_code` (`code_specialite`),
  UNIQUE KEY `uq_ref_specialites_libelle` (`libelle_specialite`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Spécialité d''un thérapeute (Logopède, Pédiatre…), v2.55.0';

-- Ordre 0 partout : la liste est triée par libellé (ordre_affichage, puis libellé).
INSERT INTO ref_specialites (code_specialite, libelle_specialite, actif, ordre_affichage)
SELECT v.code, v.libelle, 1, 0
FROM (
    SELECT 'ASSISTANT_SOCIAL' AS code, 'Assistant social' AS libelle UNION ALL
    SELECT 'DIETETICIEN',          'Diététicien'           UNION ALL
    SELECT 'EDUCATEUR_SPECIALISE', 'Éducateur spécialisé'  UNION ALL
    SELECT 'ERGOTHERAPEUTE',       'Ergothérapeute'        UNION ALL
    SELECT 'GENERALISTE',          'Généraliste'           UNION ALL
    SELECT 'GRAPHOTHERAPEUTE',     'Graphothérapeute'      UNION ALL
    SELECT 'KINESITHERAPEUTE',     'Kinésithérapeute'      UNION ALL
    SELECT 'LOGOPEDE',             'Logopède'              UNION ALL
    SELECT 'NEUROLOGUE',           'Neurologue'            UNION ALL
    SELECT 'NEUROPEDIATRE',        'Neuropédiatre'         UNION ALL
    SELECT 'NEUROPSYCHOLOGUE',     'Neuropsychologue'      UNION ALL
    SELECT 'OPHTALMOLOGUE',        'Ophtalmologue'         UNION ALL
    SELECT 'ORL',                  'ORL'                   UNION ALL
    SELECT 'ORTHOPTISTE',          'Orthoptiste'           UNION ALL
    SELECT 'OSTEOPATHE',           'Ostéopathe'            UNION ALL
    SELECT 'PEDIATRE',             'Pédiatre'              UNION ALL
    SELECT 'PEDOPSYCHIATRE',       'Pédopsychiatre'        UNION ALL
    SELECT 'PSYCHIATRE',           'Psychiatre'            UNION ALL
    SELECT 'PSYCHOLOGUE',          'Psychologue'           UNION ALL
    SELECT 'PSYCHOMOTRICIEN',      'Psychomotricien'       UNION ALL
    SELECT 'PSYCHOTHERAPEUTE',     'Psychothérapeute'
) AS v
WHERE NOT EXISTS (SELECT 1 FROM ref_specialites s WHERE s.code_specialite = v.code OR s.libelle_specialite = v.libelle);

-- 2. Reprise des valeurs saisies hors liste ------------------------------------
-- Code : libellé en majuscules, espaces remplacés par « _ » (préfixe REPRISE_ si
-- ce code existe déjà). La comparaison des libellés ignore casse et accents
-- (collation de la table) : « psychologue » rejoint « Psychologue ».
INSERT IGNORE INTO ref_specialites (code_specialite, libelle_specialite, actif, ordre_affichage)
SELECT LEFT(UPPER(REPLACE(TRIM(t.specialite), ' ', '_')), 30), LEFT(TRIM(t.specialite), 100), 1, 0
FROM therapeutes t
WHERE TRIM(COALESCE(t.specialite, '')) <> ''
  AND NOT EXISTS (SELECT 1 FROM ref_specialites s WHERE s.libelle_specialite = TRIM(t.specialite))
GROUP BY TRIM(t.specialite);

INSERT IGNORE INTO ref_specialites (code_specialite, libelle_specialite, actif, ordre_affichage)
SELECT LEFT(CONCAT('REPRISE_', UPPER(REPLACE(TRIM(t.specialite), ' ', '_'))), 30), LEFT(TRIM(t.specialite), 100), 1, 0
FROM therapeutes t
WHERE TRIM(COALESCE(t.specialite, '')) <> ''
  AND NOT EXISTS (SELECT 1 FROM ref_specialites s WHERE s.libelle_specialite = TRIM(t.specialite))
GROUP BY TRIM(t.specialite);

-- 3. Lien thérapeute → spécialité -----------------------------------------------
ALTER TABLE `therapeutes`
  ADD COLUMN IF NOT EXISTS `id_specialite` bigint(20) unsigned DEFAULT NULL
    COMMENT 'Spécialité → ref_specialites (v2.55.0) ; remplace la colonne texte specialite'
    AFTER `specialite`;

ALTER TABLE `therapeutes`
  ADD CONSTRAINT `fk_therapeutes_ref_specialites` FOREIGN KEY IF NOT EXISTS (`id_specialite`)
    REFERENCES `ref_specialites` (`id_specialite`);

-- La date de modification des thérapeutes n'est pas touchée : ce n'est pas une
-- modification de leur fiche.
UPDATE therapeutes t
JOIN ref_specialites s ON s.libelle_specialite = TRIM(t.specialite)
SET t.id_specialite = s.id_specialite,
    t.date_modification = t.date_modification
WHERE t.id_specialite IS NULL
  AND TRIM(COALESCE(t.specialite, '')) <> '';

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.55.0',
       'Referentiel ref_specialites, therapeutes.id_specialite (colonne texte specialite conservee, plus lue).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.55.0');
