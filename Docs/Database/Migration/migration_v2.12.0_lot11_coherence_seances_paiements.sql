-- =============================================================================
-- ALTHÉA — Migration de schéma v2.12.0 (Lot 11 — Cohérence séances / paiements)
-- =============================================================================
-- Objectif :
--   Rendre cohérent le caractère « dû » des séances avec la réalité métier et
--   permettre l'annulation d'un paiement sans perte de traçabilité.
--
-- Détail des changements :
--   1. ANNULEE_PATIENT : est_du 1 → 0
--      Une annulation signalée par le patient (ex. la veille) ne doit pas être
--      facturée. Seule l'absence non prévenue reste due.
--   2. ABSENTE : libellé « Absence patient » → « Absence patient (no show) »
--      est_du reste à 1 : c'est le seul véritable no show, il reste facturable.
--   3. paiements.statut_paiement : la valeur « ANNULE » devient admise.
--      La colonne est un varchar(20) sans contrainte ENUM/CHECK : aucune
--      modification de structure n'est nécessaire, seul le commentaire est
--      mis à jour pour documenter le nouvel état.
--
-- Règle métier associée (implémentée côté applicatif) :
--   - statut de séance dû      → paiement EN_ATTENTE au tarif de la séance
--   - statut de séance non dû  → paiement ANNULE, montant ramené à 0
--   - paiement déjà encaissé   → conservé (PAYE), jamais écrasé automatiquement
--
-- Prérequis : base en développement. Les séances déjà enregistrées avec le
--             statut ANNULEE_PATIENT deviendront non dues rétroactivement.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.12.0_lot11_coherence_seances_paiements_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Annulation prévenue par le patient : séance non due
-- ---------------------------------------------------------------------------

UPDATE `ref_statuts_seance`
SET `est_du` = 0
WHERE `code_statut_seance` = 'ANNULEE_PATIENT';

-- ---------------------------------------------------------------------------
-- SECTION 2 — Absence non prévenue : clarification du libellé (reste due)
-- ---------------------------------------------------------------------------

UPDATE `ref_statuts_seance`
SET `libelle_statut_seance` = 'Absence patient (no show)',
    `est_du`                = 1
WHERE `code_statut_seance` = 'ABSENTE';

-- ---------------------------------------------------------------------------
-- SECTION 3 — Documentation du nouvel état de paiement « ANNULE »
-- ---------------------------------------------------------------------------

ALTER TABLE `paiements`
    MODIFY COLUMN `statut_paiement` varchar(20) NOT NULL DEFAULT 'EN_ATTENTE'
    COMMENT 'EN_ATTENTE (séance créée, non encaissée), PAYE (encaissée) ou ANNULE (séance non due) — D-PAY-4';

-- ---------------------------------------------------------------------------
-- SECTION 4 — Mise en cohérence des paiements existants
--   Les paiements en attente rattachés à une séance devenue non due sont
--   annulés (montant à 0) plutôt que supprimés, afin de conserver l'historique.
-- ---------------------------------------------------------------------------

UPDATE `paiements` p
INNER JOIN `lia_paiements_seances` lps ON lps.`id_paiement` = p.`id_paiement`
INNER JOIN `seances` s                 ON s.`id_seance`     = lps.`id_seance`
INNER JOIN `ref_statuts_seance` ss     ON ss.`id_statut_seance` = s.`id_statut_seance`
SET p.`statut_paiement` = 'ANNULE',
    p.`montant`         = 0
WHERE p.`statut_paiement` = 'EN_ATTENTE'
  AND ss.`est_du` = 0;

UPDATE `lia_paiements_seances` lps
INNER JOIN `paiements` p ON p.`id_paiement` = lps.`id_paiement`
SET lps.`montant_affecte` = 0
WHERE p.`statut_paiement` = 'ANNULE';

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN DE LA MIGRATION v2.12.0
-- =============================================================================
