-- ---------------------------------------------------------------------------
-- Migration v2.52.0 - Chaque utilisation d'un avoir est reliée à l'encaissement
--                     qui l'a financée (table lia_paiements_avoirs)
--
-- Audit du 18/10/2026, lot B.5 (étape 3) ; décisions de Joëlle (21/10/2026) :
-- liaison des avoirs ; historique laissé dans le commentaire.
--
-- Régler une séance avec un avoir diminue l'affectation d'un ou plusieurs
-- encaissements (sources) et crée un paiement au mode « Avoir » sur la séance
-- réglée. Jusqu'ici, le lien entre les deux n'était écrit qu'en texte, dans les
-- commentaires (« [Avoir 20,00 € utilisé le … par <nom affiché> pour S…] » sur
-- la source, « Avoir utilisé : PAI… (S…) 20,00 € » sur le règlement) : aucune
-- requête ne pouvait dire quel encaissement a financé quel règlement.
--
-- Table lia_paiements_avoirs : une ligne par source utilisée.
-- - id_paiement_avoir : règlement par avoir créé (paiements) ;
-- - id_lia_paiements_seances_source : affectation encaissée diminuée (elle donne
--   l'encaissement et la séance d'origine) ;
-- - montant : part prise sur cette source ;
-- - date_utilisation, id_utilisateur : quand et par qui (sec_utilisateurs ; un
--   compte n'est jamais supprimé, son nom est lu à l'affichage).
-- Clés étrangères sans suppression en cascade : un encaissement ne se supprime
-- jamais (il s'annule).
--
-- Les utilisations faites avant cette migration gardent leurs mentions dans les
-- commentaires ; l'application lit les deux.
--
-- Règles : structure seulement, aucune donnée modifiée ; script rejouable.
-- Retour arrière : migration_v2.52.0_lia_paiements_avoirs_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

CREATE TABLE IF NOT EXISTS `lia_paiements_avoirs` (
  `id_lia_paiements_avoirs`          bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_paiement_avoir`                bigint(20) unsigned NOT NULL COMMENT 'Règlement par avoir (paiement au mode est_avoir)',
  `id_lia_paiements_seances_source`  bigint(20) unsigned NOT NULL COMMENT 'Affectation encaissée diminuée (encaissement et séance d''origine)',
  `montant`                          decimal(10,2)       NOT NULL COMMENT 'Part prise sur cette source',
  `date_utilisation`                 datetime            NOT NULL DEFAULT current_timestamp(),
  `id_utilisateur`                   bigint(20) unsigned DEFAULT NULL COMMENT 'Utilisateur → sec_utilisateurs',
  PRIMARY KEY (`id_lia_paiements_avoirs`),
  UNIQUE KEY `uq_lia_paiements_avoirs` (`id_paiement_avoir`, `id_lia_paiements_seances_source`),
  KEY `idx_lia_paiements_avoirs_source` (`id_lia_paiements_seances_source`),
  KEY `idx_lia_paiements_avoirs_utilisateur` (`id_utilisateur`),
  CONSTRAINT `fk_lia_paiements_avoirs_paiement` FOREIGN KEY (`id_paiement_avoir`) REFERENCES `paiements` (`id_paiement`),
  CONSTRAINT `fk_lia_paiements_avoirs_source` FOREIGN KEY (`id_lia_paiements_seances_source`) REFERENCES `lia_paiements_seances` (`id_lia_paiements_seances`),
  CONSTRAINT `fk_lia_paiements_avoirs_utilisateur` FOREIGN KEY (`id_utilisateur`) REFERENCES `sec_utilisateurs` (`id_utilisateur`),
  CONSTRAINT `chk_lia_paiements_avoirs_montant` CHECK (`montant` > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci
  COMMENT='Utilisation d''un avoir : règlement par avoir ↔ affectation encaissée qui l''a financé (v2.52.0)';

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.52.0',
       'Table lia_paiements_avoirs (reglement par avoir relie a chaque affectation encaissee utilisee).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.52.0');
