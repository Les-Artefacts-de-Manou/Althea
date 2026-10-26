-- =============================================================================
-- ALTHÉA — ROLLBACK de la migration v2.15.0 (Lot 14 — Portabilité)
-- =============================================================================
-- Rôle :
--   Rétablir le schéma dans l'état antérieur au lot 14.
--
-- AVERTISSEMENT — perte de données :
--   Ce rollback SUPPRIME des informations qui n'existaient pas avant le lot 14
--   et qui ne sont reconstituables depuis aucune autre source :
--     * les rattachements natures ↔ domaines ;
--     * l'historisation des tarifs (dates de validité) ;
--     * les comportements de domaine exprimés séparément.
--   Les domaines dont les trois comportements auraient été paramétrés
--   DIFFÉREMMENT de est_inami (par exemple un domaine conventionné dont le
--   no-show serait facturé) perdront cette distinction : elle est précisément
--   inexprimable dans l'ancien modèle.
--
-- PRÉREQUIS BLOQUANT :
--   La restauration de l'ancienne clé uq_lia_prix_domaine (id_prix_seance,
--   id_domaine) ÉCHOUERA si des tarifs historisés ont été créés, c'est-à-dire
--   si un même couple (prix, domaine) existe sur plusieurs périodes.
--   Exécuter au préalable le contrôle de la SECTION 0 et arbitrer manuellement
--   les doublons : il n'appartient pas à un script de rollback de choisir
--   quelle période tarifaire conserver.
--
-- Ordre d'exécution : inverse de la migration.
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 0 — Contrôle préalable (à exécuter et à lire AVANT la suite)
--   Doit retourner 0 ligne. Toute ligne remontée signale un couple
--   (prix, domaine) présent sur plusieurs périodes, qui empêchera la
--   restauration de l'ancienne contrainte d'unicité.
-- ---------------------------------------------------------------------------

SELECT `id_prix_seance`, `id_domaine`, COUNT(*) AS nb_periodes
FROM `lia_prix_seance_domaines`
GROUP BY `id_prix_seance`, `id_domaine`
HAVING COUNT(*) > 1;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Suppression de la liaison natures ↔ domaines
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `lia_natures_seance_domaines`;

-- ---------------------------------------------------------------------------
-- SECTION 2 — Retour de lia_prix_seance_domaines à une liaison N-N simple
--   La contrainte CHECK et la clé incluant la date sont retirées avant les
--   colonnes dont elles dépendent, sans quoi la suppression échouerait.
-- ---------------------------------------------------------------------------

ALTER TABLE `lia_prix_seance_domaines`
  DROP CONSTRAINT `ck_lia_prix_domaine_periode`;

ALTER TABLE `lia_prix_seance_domaines`
  DROP INDEX `uq_lia_prix_domaine_debut`;

ALTER TABLE `lia_prix_seance_domaines`
  DROP COLUMN `date_debut_validite`,
  DROP COLUMN `date_fin_validite`,
  DROP COLUMN `actif`;

-- Restauration de l'ancienne unicité (échoue si la SECTION 0 remontait des lignes).
ALTER TABLE `lia_prix_seance_domaines`
  ADD UNIQUE KEY `uq_lia_prix_domaine` (`id_prix_seance`, `id_domaine`);

-- ---------------------------------------------------------------------------
-- SECTION 3 — Retrait des attributs de comportement de ref_domaines
--   est_inami n'a jamais été supprimé par la migration : il reste la source de
--   vérité de l'ancien modèle et aucune réécriture n'est nécessaire.
-- ---------------------------------------------------------------------------

ALTER TABLE `ref_domaines`
  DROP COLUMN `tarif_convention`,
  DROP COLUMN `no_show_facturable`,
  DROP COLUMN `soumis_quota`;

-- ---------------------------------------------------------------------------
-- SECTION 4 — Retrait de la version
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
DELETE FROM `tec_meta_schema` WHERE `version_schema` = '2.15.0';
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
