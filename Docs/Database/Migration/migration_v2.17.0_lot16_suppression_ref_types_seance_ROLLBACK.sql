-- =================================================================================================
-- Migration : v2.17.0 - Lot 16 - ROLLBACK - Restauration de la table ref_types_seance
-- Date      : 19/09/2026
--
-- Objet :
-- Recréer la table ref_types_seance telle qu'elle existait avant la migration v2.17.0.
--
-- Important :
-- Ce rollback restaure la STRUCTURE de la table, pas son CONTENU. Les types de séance qui
-- s'y trouvaient sont définitivement perdus si aucune sauvegarde n'a été conservée avant
-- l'exécution de la migration.
--
-- La table était déjà orpheline au moment de sa suppression : aucune clé étrangère n'est
-- donc recréée ici, et aucune colonne n'est réintroduite dans la table seances.
-- =================================================================================================

-- -------------------------------------------------------------------------------------------------
-- 1. Recréation de la table dans sa structure d'origine
-- -------------------------------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ref_types_seance` (
  `id_type_seance` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_type_seance` varchar(30) NOT NULL,
  `libelle_type_seance` varchar(100) NOT NULL,
  `tarif_defaut` decimal(10,2) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_type_seance`),
  UNIQUE KEY `uq_ref_types_seance_code` (`code_type_seance`),
  UNIQUE KEY `uq_ref_types_seance_libelle` (`libelle_type_seance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- -------------------------------------------------------------------------------------------------
-- 2. Retrait de la version du journal de schéma
-- -------------------------------------------------------------------------------------------------
DELETE FROM tec_meta_schema
WHERE version_schema = '2.17.0';
