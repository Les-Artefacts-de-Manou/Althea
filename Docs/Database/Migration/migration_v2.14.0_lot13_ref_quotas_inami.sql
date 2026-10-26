-- =============================================================================
-- ALTHÉA — Migration de schéma v2.14.0 (Lot 13 — Paramétrage des quotas INAMI)
-- =============================================================================
-- Contexte métier :
--   Le lot 12 a introduit le comptage du quota annuel INAMI, mais avec des
--   plafonds figés dans le code applicatif (10 séances de soutien, 20 de
--   traitement, alerte à 28). Ces valeurs sont des règles de convention INAMI :
--   elles peuvent évoluer, et un autre domaine conventionné pourrait un jour
--   relever de plafonds différents.
--
--   Cette migration externalise donc ces règles dans une table de référentiel
--   administrable depuis l'application.
--
-- Choix de modélisation :
--   * Une ligne par domaine et par période de validité. Les deux plafonds
--     (soutien / traitement) figurent sur la MÊME ligne : ils forment un tout
--     cohérent et ne doivent jamais être modifiés indépendamment l'un de
--     l'autre.
--   * Historisation par dates : date_debut_validite est obligatoire,
--     date_fin_validite reste NULL tant que la règle est en vigueur. Quand
--     l'INAMI révisera ses plafonds, on clôture la ligne courante et on en
--     ajoute une nouvelle. L'historique permet de recalculer correctement un
--     quota passé avec les règles qui s'appliquaient à l'époque.
--   * Le plafond TOTAL n'est pas stocké : c'est la somme des deux plafonds.
--     Le stocker ouvrirait la porte à une incohérence (total <> soutien +
--     traitement) impossible à arbitrer côté applicatif.
--
-- Ordre d'exécution :
--   1. Création de ref_quotas_inami
--   2. Insertion de la règle en vigueur pour le domaine Realism
--   3. Versionner dans tec_meta_schema
--
-- Prérequis : ref_domaines doit contenir le domaine Realism (est_inami = 1).
-- Compatible : MariaDB 12.x, moteur InnoDB, charset utf8mb4.
-- Script de rollback : migration_v2.14.0_lot13_ref_quotas_inami_ROLLBACK.sql
-- =============================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- SECTION 1 — Création de la table ref_quotas_inami
--   uq_ref_quotas_inami_domaine_debut : interdit deux règles démarrant le même
--   jour pour un même domaine. Le non-chevauchement des périodes ne peut pas
--   s'exprimer en contrainte SQL : il est contrôlé par la couche métier.
--   ON DELETE RESTRICT : supprimer un domaine encore paramétré doit échouer
--   plutôt que de faire disparaître silencieusement sa règle de facturation.
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_quotas_inami` (
  `id_quota_inami` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_domaine` bigint(20) unsigned NOT NULL
    COMMENT 'Domaine conventionné auquel s''applique la règle — D-INA-2',
  `plafond_soutien` int(11) NOT NULL DEFAULT 10
    COMMENT 'Nombre de séances de soutien remboursées par période — D-INA-2',
  `plafond_traitement` int(11) NOT NULL DEFAULT 20
    COMMENT 'Nombre de séances de traitement remboursées par période — D-INA-2',
  `seuil_alerte_total` int(11) NOT NULL DEFAULT 28
    COMMENT 'Total à partir duquel l''utilisateur est averti de l''approche du plafond — D-INA-2',
  `date_debut_validite` date NOT NULL
    COMMENT 'Premier jour d''application de la règle — D-INA-2',
  `date_fin_validite` date DEFAULT NULL
    COMMENT 'Dernier jour d''application, NULL si la règle est toujours en vigueur — D-INA-2',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_quota_inami`),
  UNIQUE KEY `uq_ref_quotas_inami_domaine_debut` (`id_domaine`, `date_debut_validite`),
  KEY `IDX_ref_quotas_inami_validite` (`id_domaine`, `date_debut_validite`, `date_fin_validite`),
  CONSTRAINT `fk_ref_quotas_inami_domaine`
    FOREIGN KEY (`id_domaine`) REFERENCES `ref_domaines` (`id_domaine`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `chk_ref_quotas_inami_plafonds`
    CHECK (`plafond_soutien` >= 0 AND `plafond_traitement` >= 0),
  CONSTRAINT `chk_ref_quotas_inami_periode`
    CHECK (`date_fin_validite` IS NULL OR `date_fin_validite` >= `date_debut_validite`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Règles de quota annuel de prestations remboursées, par domaine conventionné';

-- ---------------------------------------------------------------------------
-- SECTION 2 — Règle en vigueur pour les domaines INAMI
--   Reprise à l'identique des valeurs jusqu'ici codées en dur, afin que le
--   comportement applicatif soit strictement inchangé après migration.
--   La sélection porte sur est_inami = 1 plutôt que sur le code 'REA' : si un
--   second domaine conventionné existe déjà, il doit lui aussi être paramétré,
--   faute de quoi son quota ne serait plus calculé.
--   INSERT IGNORE + clé d'unicité protègent contre une double exécution.
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;

INSERT IGNORE INTO `ref_quotas_inami`
  (`id_domaine`, `plafond_soutien`, `plafond_traitement`, `seuil_alerte_total`,
   `date_debut_validite`, `date_fin_validite`, `actif`)
SELECT `id_domaine`, 10, 20, 28, '2026-01-01', NULL, 1
FROM `ref_domaines`
WHERE `est_inami` = 1;

COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

-- ---------------------------------------------------------------------------
-- SECTION 3 — Versionnage dans tec_meta_schema
-- ---------------------------------------------------------------------------

SET @OLD_AUTOCOMMIT=@@AUTOCOMMIT, @@AUTOCOMMIT=0;
INSERT INTO `tec_meta_schema` (`version_schema`, `description`, `date_application`)
VALUES ('2.14.0', 'Lot 13 parametrage quotas INAMI : table ref_quotas_inami (plafonds soutien/traitement, seuil alerte, periode de validite par domaine) remplacant les plafonds codes en dur', NOW());
COMMIT;
SET AUTOCOMMIT=@OLD_AUTOCOMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
