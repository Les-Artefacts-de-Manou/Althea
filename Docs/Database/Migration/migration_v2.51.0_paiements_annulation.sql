-- ---------------------------------------------------------------------------
-- Migration v2.51.0 - L'annulation d'un encaissement est enregistrée dans des
--                     colonnes (date, utilisateur, motif)
--
-- Audit du 18/10/2026, lot B.5 (étape 2) ; décisions de Joëlle (21/10/2026) :
-- colonnes typées ; historique laissé dans le commentaire.
--
-- Jusqu'ici, l'annulation n'était tracée que par une mention ajoutée au
-- commentaire (« [Annulé le … par <nom affiché> : motif] ») : aucune requête ne
-- pouvait la lire de façon fiable, et le nom de l'utilisateur était recopié.
--
-- 1. paiements.date_annulation : date et heure de l'annulation.
-- 2. paiements.id_utilisateur_annulation : utilisateur qui l'a faite → clé
--    étrangère vers sec_utilisateurs (un compte n'est jamais supprimé, il se
--    désactive ; son nom est lu à l'affichage).
-- 3. paiements.motif_annulation : motif saisi (500 caractères au plus, limite
--    reprise par la fenêtre de saisie).
--
-- Les annulations faites avant cette migration gardent leur mention dans le
-- commentaire (colonnes vides) ; l'application lit les deux.
--
-- Règles : structure seulement, aucune donnée modifiée ; script rejouable.
-- Retour arrière : migration_v2.51.0_paiements_annulation_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

ALTER TABLE `paiements`
  ADD COLUMN IF NOT EXISTS `date_annulation` datetime DEFAULT NULL
    COMMENT 'Annulation d''un encaissement : date et heure (v2.51.0) ; NULL si non annulé ou annulé avant la v2.51.0 (mention dans le commentaire)'
    AFTER `commentaire`,
  ADD COLUMN IF NOT EXISTS `id_utilisateur_annulation` bigint(20) unsigned DEFAULT NULL
    COMMENT 'Annulation : utilisateur → sec_utilisateurs (v2.51.0)'
    AFTER `date_annulation`,
  ADD COLUMN IF NOT EXISTS `motif_annulation` varchar(500) DEFAULT NULL
    COMMENT 'Annulation : motif saisi (v2.51.0)'
    AFTER `id_utilisateur_annulation`;

ALTER TABLE `paiements`
  ADD CONSTRAINT `fk_paiements_utilisateur_annulation` FOREIGN KEY IF NOT EXISTS (`id_utilisateur_annulation`)
    REFERENCES `sec_utilisateurs` (`id_utilisateur`);

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.51.0',
       'paiements.date_annulation, id_utilisateur_annulation (cle etrangere vers sec_utilisateurs), motif_annulation.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.51.0');
