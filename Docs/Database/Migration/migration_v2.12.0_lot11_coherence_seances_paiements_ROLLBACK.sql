-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.12.0 (Lot 11)
-- =============================================================================
-- Objectif :
--   Rétablir l'état antérieur à migration_v2.12.0_lot11_coherence_seances_paiements.sql
--
-- Détail des restaurations :
--   1. ANNULEE_PATIENT : est_du 0 → 1
--   2. ABSENTE : libellé « Absence patient (no show) » → « Absence patient »
--   3. paiements.statut_paiement : commentaire d'origine restauré
--   4. Paiements annulés repassés en EN_ATTENTE au tarif de leur séance
--
-- Avertissement : les montants des paiements annulés sont recalculés depuis le
--   tarif de la séance liée. Un paiement annulé dont la séance a changé de
--   tarif entretemps retrouvera le tarif courant, pas sa valeur historique.
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Restauration des paiements annulés en attente
-- ---------------------------------------------------------------------------

UPDATE `paiements` p
INNER JOIN `lia_paiements_seances` lps ON lps.`id_paiement` = p.`id_paiement`
INNER JOIN `seances` s                 ON s.`id_seance`     = lps.`id_seance`
SET p.`statut_paiement` = 'EN_ATTENTE',
    p.`montant`         = s.`tarif_seance`,
    lps.`montant_affecte` = s.`tarif_seance`
WHERE p.`statut_paiement` = 'ANNULE';

-- ---------------------------------------------------------------------------
-- SECTION 2 — Restauration du commentaire de colonne
-- ---------------------------------------------------------------------------

ALTER TABLE `paiements`
    MODIFY COLUMN `statut_paiement` varchar(20) NOT NULL DEFAULT 'EN_ATTENTE'
    COMMENT 'EN_ATTENTE (séance créée, non encaissée) ou PAYE — D-PAY-4';

-- ---------------------------------------------------------------------------
-- SECTION 3 — Restauration du référentiel des statuts de séance
-- ---------------------------------------------------------------------------

UPDATE `ref_statuts_seance`
SET `est_du` = 1
WHERE `code_statut_seance` = 'ANNULEE_PATIENT';

UPDATE `ref_statuts_seance`
SET `libelle_statut_seance` = 'Absence patient'
WHERE `code_statut_seance` = 'ABSENTE';

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- =============================================================================
-- FIN DU ROLLBACK v2.12.0
-- =============================================================================
