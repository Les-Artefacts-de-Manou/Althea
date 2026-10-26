-- =============================================================================
-- ALTHÉA — Migration de schéma v2.10.0 (Lot 10 — Paiements & Tarification)
-- =============================================================================
-- Objectif :
--   Poser le socle de données de la brique Paiements (Lot 2, brique 6) et de la
--   Tarification (brique 7), conformément au cahier des charges Pearl.
--
-- Décisions appliquées (cf. Docs/Conception/Plan_Conception_Paiements_Althea.md) :
--   D-PAY-3  : mode de paiement fiabilisé par un référentiel ref_modes_paiement
--              (Espèces, Virement, INAMI) — ajoutable à la volée.
--   D-TAR-1  : tarif porté par le PATIENT (50/55/60 €) via un référentiel
--              modifiable ref_prix_seance (montants modifiables sans impacter
--              les séances déjà comptées — snapshot sur seances.tarif_seance).
--   D-TAR-2  : domaine Realism = circuit INAMI → colonne ref_domaines.est_inami.
--              La séance Realism n'est pas payée par le patient (facturation INAMI).
--   D-TAR-3  : le caractère « dû » d'une séance découle de son statut →
--              colonne ref_statuts_seance.est_du (No-show = dû, sauf Realism).
--   T-1      : rattachement du tarif patient via dossiers.id_prix_seance
--              (un dossier = un patient dans un domaine donné).
--
-- Ordre d'exécution :
--   1. ref_modes_paiement  : création du référentiel + jeu de données initial
--   2. ref_prix_seance     : création du référentiel tarifaire + seed provisoire
--   3. dossiers            : ajout de id_prix_seance (FK nullable vers ref_prix_seance)
--   4. ref_domaines        : ajout de est_inami (Realism = 1)
--   5. ref_statuts_seance  : ajout de est_du (+ positionnement selon les statuts)
--   6. Versionner dans tec_meta_schema
--
-- Montants provisoires (ref_prix_seance) : à valider/ajuster par Pearl.
-- Prérequis : base en développement (pas de données métier réelles).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.10.0_lot10_paiements_tarification_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Création de ref_modes_paiement (D-PAY-3)
-- Référentiel des modes d'encaissement. INAMI est un mode à part entière :
-- encaisser une séance Realism = créer un paiement mode INAMI à réception du
-- versement de fin de mois.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_modes_paiement`;
CREATE TABLE `ref_modes_paiement` (
  `id_mode_paiement`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_mode_paiement`    varchar(30)         NOT NULL,
  `libelle_mode_paiement` varchar(100)        NOT NULL,
  `actif`                 tinyint(1)          NOT NULL DEFAULT 1,
  `ordre_affichage`       int(11)             NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_mode_paiement`),
  UNIQUE KEY `uq_ref_modes_paiement_code`    (`code_mode_paiement`),
  UNIQUE KEY `uq_ref_modes_paiement_libelle` (`libelle_mode_paiement`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
LOCK TABLES `ref_modes_paiement` WRITE;
INSERT INTO `ref_modes_paiement` VALUES
  (1, 'ESPECES',  'Espèces',  1, 10),
  (2, 'VIREMENT', 'Virement', 1, 20),
  (3, 'INAMI',    'INAMI',    1, 30);
UNLOCK TABLES;
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 2 — Création de ref_prix_seance (D-TAR-1)
-- Grille tarifaire modifiable. Le montant appliqué à une séance est LU ici à
-- la création puis figé dans seances.tarif_seance (snapshot). Modifier un
-- montant ici n'affecte donc jamais les séances déjà comptées.
-- Montants PROVISOIRES — à valider par Pearl.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_prix_seance`;
CREATE TABLE `ref_prix_seance` (
  `id_prix_seance`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_prix_seance`    varchar(30)         NOT NULL,
  `libelle_prix_seance` varchar(100)        NOT NULL,
  `montant`             decimal(10,2)       NOT NULL DEFAULT 0.00,
  `actif`               tinyint(1)          NOT NULL DEFAULT 1,
  `ordre_affichage`     int(11)             NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_prix_seance`),
  UNIQUE KEY `uq_ref_prix_seance_code`    (`code_prix_seance`),
  UNIQUE KEY `uq_ref_prix_seance_libelle` (`libelle_prix_seance`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
LOCK TABLES `ref_prix_seance` WRITE;
INSERT INTO `ref_prix_seance` VALUES
  (1, 'PATIENT_50', 'Tarif patient 50 €', 50.00, 1, 10),
  (2, 'PATIENT_55', 'Tarif patient 55 €', 55.00, 1, 20),
  (3, 'PATIENT_60', 'Tarif patient 60 €', 60.00, 1, 30),
  (4, 'INAMI',      'Séance INAMI',        91.13, 1, 40);
UNLOCK TABLES;
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;


-- ---------------------------------------------------------------------------
-- SECTION 3 — dossiers.id_prix_seance (T-1)
-- Le tarif du patient est porté par le dossier (un dossier = patient × domaine).
-- Nullable : NULL en Realism (circuit INAMI) ou tant que non renseigné (repli
-- applicatif sur ref_types_seance.tarif_defaut).
-- ---------------------------------------------------------------------------

ALTER TABLE `dossiers`
  ADD COLUMN `id_prix_seance` bigint(20) unsigned DEFAULT NULL
    COMMENT 'Tarif patient applicable aux séances de ce dossier (ref_prix_seance) — D-TAR-1/T-1'
    AFTER `id_domaine`;

ALTER TABLE `dossiers`
  ADD KEY `fk_dossiers_ref_prix_seance` (`id_prix_seance`),
  ADD CONSTRAINT `fk_dossiers_ref_prix_seance`
       FOREIGN KEY (`id_prix_seance`) REFERENCES `ref_prix_seance` (`id_prix_seance`);


-- ---------------------------------------------------------------------------
-- SECTION 4 — ref_domaines.est_inami (D-TAR-2)
-- Isole le circuit INAMI (Realism) : pas de paiement patient, no-show non
-- facturé, extraction mensuelle des séances à facturer à l'INAMI.
-- ---------------------------------------------------------------------------

ALTER TABLE `ref_domaines`
  ADD COLUMN `est_inami` tinyint(1) NOT NULL DEFAULT 0
    COMMENT 'Domaine facturé à l''INAMI (Realism) — le patient ne paie pas directement — D-TAR-2'
    AFTER `libelle_domaine`;

UPDATE `ref_domaines`
   SET `est_inami` = 1
 WHERE `code_domaine` = 'REALISM' OR `libelle_domaine` = 'Realism';


-- ---------------------------------------------------------------------------
-- SECTION 5 — ref_statuts_seance.est_du (D-TAR-3)
-- Le caractère « dû » d'une séance découle de son statut. Par défaut dû (1) ;
-- les statuts d'annulation non imputables au patient sont non dus (0).
-- Les statuts ajoutés ultérieurement à la volée porteront aussi ce flag.
-- ---------------------------------------------------------------------------

ALTER TABLE `ref_statuts_seance`
  ADD COLUMN `est_du` tinyint(1) NOT NULL DEFAULT 1
    COMMENT 'Séance due (facturable) pour ce statut — No-show = dû, annulation à temps = non dû — D-TAR-3'
    AFTER `libelle_statut_seance`;

-- Statuts non dus : annulation praticien et maladie (non imputables au patient).
UPDATE `ref_statuts_seance`
   SET `est_du` = 0
 WHERE `code_statut_seance` IN ('ANNULEE_PRATICIEN', 'MALADE', 'REPORTEE');


-- ---------------------------------------------------------------------------
-- SECTION 6 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.10.0', 'Lot 10 paiements/tarification : ref_modes_paiement (dont INAMI), ref_prix_seance (tarifs patient 50/55/60 + INAMI 91,13), dossiers.id_prix_seance, ref_domaines.est_inami (Realism), ref_statuts_seance.est_du', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

-- ---------------------------------------------------------------------------
-- Restauration des flags de session
-- ---------------------------------------------------------------------------
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
