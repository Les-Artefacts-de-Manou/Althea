-- ---------------------------------------------------------------------------
-- Migration v2.57.0 - Drapeaux de référentiel : séance non tenue, statut proposé
-- à la création, lien et rôle légal par défaut d'un contact
--
-- Audit C.3 étape 2 (ADR-54, décisions de Joëlle du 24/10/2026) : ces
-- comportements étaient tirés des codes des statuts ou écrits dans la fenêtre
-- Contact ; ils se règlent désormais par une case du référentiel.
--
-- 1. ref_statuts_seance.est_non_tenue : « Non tenue, libère le créneau » — la
--    séance n'occupe pas son horaire (planning du jour, contrôle de
--    chevauchement) et passer d'un statut non tenu à un autre ne recontrôle
--    pas l'horaire. Remplace les trois listes de codes qui ne concordaient pas.
--    Valeurs de départ : Annulée (patient), Annulée (praticien), Reportée et
--    Maladie (qui occupait jusqu'ici son créneau). L'absence non excusée garde
--    sa case est_absence (non tenue pour la règle des statuts, mais elle occupe
--    son créneau, comme avant).
-- 2. ref_statuts_seance.propose_creation : « Proposé à la création » d'une
--    séance. Valeurs de départ : Planifiée et Réalisée (comme avant).
-- 3. ref_liens_patient.par_defaut et ref_role_legal.par_defaut : valeur
--    présélectionnée à la création d'un contact (une seule ; l'application
--    décoche l'ancienne). Valeurs de départ : Mère, Autorité parentale.
--
-- Les autres statuts (dont ceux ajoutés par l'utilisatrice) partent décochés :
-- ils se règlent dans Référentiels › Statuts de séance.
--
-- Règles : script rejouable ; aucune donnée existante supprimée. Retour arrière :
-- migration_v2.57.0_drapeaux_referentiels_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- 1 et 2. Statuts de séance ----------------------------------------------------
ALTER TABLE `ref_statuts_seance`
  ADD COLUMN IF NOT EXISTS `est_non_tenue` tinyint(1) NOT NULL DEFAULT 0
    COMMENT '1 = seance non tenue : libere son creneau (planning, chevauchement), v2.57.0'
    AFTER `est_absence`;

ALTER TABLE `ref_statuts_seance`
  ADD COLUMN IF NOT EXISTS `propose_creation` tinyint(1) NOT NULL DEFAULT 0
    COMMENT '1 = statut propose a la creation d une seance, v2.57.0'
    AFTER `est_non_tenue`;

UPDATE ref_statuts_seance
SET est_non_tenue = 1
WHERE code_statut_seance IN ('ANNULEE_PATIENT', 'ANNULEE_PRATICIEN', 'REPORTEE', 'MALADE');

UPDATE ref_statuts_seance
SET propose_creation = 1
WHERE code_statut_seance IN ('PLANIFIEE', 'REALISEE');

-- 3. Valeur par défaut d'un contact ------------------------------------------
ALTER TABLE `ref_liens_patient`
  ADD COLUMN IF NOT EXISTS `par_defaut` tinyint(1) NOT NULL DEFAULT 0
    COMMENT '1 = lien preselectionne a la creation d un contact (un seul), v2.57.0'
    AFTER `ordre_affichage`;

ALTER TABLE `ref_role_legal`
  ADD COLUMN IF NOT EXISTS `par_defaut` tinyint(1) NOT NULL DEFAULT 0
    COMMENT '1 = role legal preselectionne a la creation d un contact (un seul), v2.57.0'
    AFTER `ordre_affichage`;

UPDATE ref_liens_patient SET par_defaut = 1
WHERE code_lien_patient = 'MERE'
  AND NOT EXISTS (SELECT 1 FROM (SELECT id_lien_patient FROM ref_liens_patient WHERE par_defaut = 1) d);

UPDATE ref_role_legal SET par_defaut = 1
WHERE code_role_legal = 'AUTORITE_PARENTALE'
  AND NOT EXISTS (SELECT 1 FROM (SELECT id_role_legal FROM ref_role_legal WHERE par_defaut = 1) d);

-- Version du schéma ------------------------------------------------------------
INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.57.0',
       'Drapeaux de referentiel : ref_statuts_seance.est_non_tenue et propose_creation, par_defaut des liens et roles legaux (audit C.3).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.57.0');

COMMIT;

-- Contrôle : SELECT code_statut_seance, est_non_tenue, propose_creation FROM ref_statuts_seance ORDER BY ordre_affichage;
