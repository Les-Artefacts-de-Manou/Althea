-- ---------------------------------------------------------------------------
-- Migration v2.50.0 - Le mode d'un paiement est relié au référentiel par son
--                     identifiant
--
-- Audit du 18/10/2026, lot B.5 (étape 1) ; décision de Joëlle (21/10/2026) :
-- identifiant + clé étrangère.
--
-- paiements.mode_paiement contenait le LIBELLÉ du mode (« Espèces », « Avoir »…) :
-- renommer un mode coupait le lien avec l'historique, et un règlement par avoir
-- n'était reconnu que par ce libellé. Le code ne convient pas mieux : celui d'un
-- mode non système reste modifiable dans le référentiel.
--
-- 1. Reprise des valeurs inconnues du référentiel (mode renommé ou supprimé
--    depuis) : elles deviennent des modes INACTIFS (non proposés à la saisie) ;
--    rien n'est perdu.
-- 2. Colonne paiements.id_mode_paiement (clé étrangère, RESTRICT : un mode
--    utilisé ne peut plus être supprimé, il se désactive), renseignée d'après le
--    libellé (à défaut le code) existant.
-- 3. La colonne texte paiements.mode_paiement n'est plus lue ni écrite par
--    l'application ; elle est CONSERVÉE (vérification avant de la retirer par une
--    migration ultérieure), comme patients.lateralite en v2.46.0.
--
-- Règles : script rejouable ; aucune donnée existante supprimée. Retour arrière :
-- migration_v2.50.0_paiements_mode_id_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- 1. Reprise des modes inconnus -----------------------------------------------
-- Code : libellé en majuscules, espaces remplacés par « _ » (préfixe REPRISE_ si
-- ce code existe déjà) ; inactif, ordre après les modes existants.
INSERT IGNORE INTO ref_modes_paiement (code_mode_paiement, libelle_mode_paiement, est_avoir, actif, ordre_affichage)
SELECT LEFT(UPPER(REPLACE(TRIM(p.mode_paiement), ' ', '_')), 30), LEFT(TRIM(p.mode_paiement), 100), 0, 0, 900
FROM paiements p
WHERE TRIM(COALESCE(p.mode_paiement, '')) <> ''
  AND NOT EXISTS (SELECT 1 FROM ref_modes_paiement m
                  WHERE m.libelle_mode_paiement = TRIM(p.mode_paiement)
                     OR m.code_mode_paiement = TRIM(p.mode_paiement))
GROUP BY TRIM(p.mode_paiement);

INSERT IGNORE INTO ref_modes_paiement (code_mode_paiement, libelle_mode_paiement, est_avoir, actif, ordre_affichage)
SELECT LEFT(CONCAT('REPRISE_', UPPER(REPLACE(TRIM(p.mode_paiement), ' ', '_'))), 30), LEFT(TRIM(p.mode_paiement), 100), 0, 0, 900
FROM paiements p
WHERE TRIM(COALESCE(p.mode_paiement, '')) <> ''
  AND NOT EXISTS (SELECT 1 FROM ref_modes_paiement m
                  WHERE m.libelle_mode_paiement = TRIM(p.mode_paiement)
                     OR m.code_mode_paiement = TRIM(p.mode_paiement))
GROUP BY TRIM(p.mode_paiement);

-- 2. Lien paiement → mode -------------------------------------------------------
ALTER TABLE `paiements`
  ADD COLUMN IF NOT EXISTS `id_mode_paiement` bigint(20) unsigned DEFAULT NULL
    COMMENT 'Mode de règlement → ref_modes_paiement (v2.50.0) ; NULL pour un dû en attente ; remplace la colonne texte mode_paiement'
    AFTER `mode_paiement`;

ALTER TABLE `paiements`
  ADD CONSTRAINT `fk_paiements_ref_modes_paiement` FOREIGN KEY IF NOT EXISTS (`id_mode_paiement`)
    REFERENCES `ref_modes_paiement` (`id_mode_paiement`);

-- Libellé d'abord (c'est ce que l'application écrivait), code ensuite.
UPDATE paiements p
JOIN ref_modes_paiement m ON m.libelle_mode_paiement = TRIM(p.mode_paiement)
SET p.id_mode_paiement = m.id_mode_paiement
WHERE p.id_mode_paiement IS NULL
  AND TRIM(COALESCE(p.mode_paiement, '')) <> '';

UPDATE paiements p
JOIN ref_modes_paiement m ON m.code_mode_paiement = TRIM(p.mode_paiement)
SET p.id_mode_paiement = m.id_mode_paiement
WHERE p.id_mode_paiement IS NULL
  AND TRIM(COALESCE(p.mode_paiement, '')) <> '';

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.50.0',
       'paiements.id_mode_paiement (cle etrangere vers ref_modes_paiement) ; la colonne texte mode_paiement n''est plus lue.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.50.0');
