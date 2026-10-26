-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.14.0 (Lot 13 — Paramétrage quotas INAMI)
-- =============================================================================
-- Rôle :
--   Annule la migration migration_v2.14.0_lot13_ref_quotas_inami.sql.
--
-- Conséquence fonctionnelle :
--   Après ce rollback, l'application ne trouve plus aucune règle paramétrée et
--   revient à ses valeurs de repli codées en dur (10 / 20 / 28). Le comportement
--   reste donc correct pour Realism, mais toute règle personnalisée saisie par
--   l'utilisateur depuis l'écran des référentiels sera DÉFINITIVEMENT PERDUE.
--
-- Ordre d'exécution (inverse de la migration) :
--   1. Suppression de la table ref_quotas_inami
--   2. Suppression de la ligne de version dans tec_meta_schema
--
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Suppression de la table
--   Aucune autre table ne référence ref_quotas_inami : la suppression est
--   directe et ne laisse pas de clé orpheline.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `ref_quotas_inami`;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Retrait de la version dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.14.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40101 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
