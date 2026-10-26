-- ---------------------------------------------------------------------------
-- ROLLBACK de la migration v2.52.0 - liaison des avoirs utilisés
--
-- Les utilisations faites après la migration sont d'abord recopiées dans les
-- commentaires, sous la forme que lisait l'application d'avant :
-- - sur l'encaissement source : « [Avoir X € utilisé le jj/mm/aaaa par <nom> pour S…] » ;
-- - sur le règlement par avoir : « Avoir utilisé : PAI… (S…) X € » ;
-- puis la table est retirée : rien n'est perdu.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- Encaissements sources : une mention par utilisation, dans l'ordre chronologique.
UPDATE paiements p
JOIN (
    SELECT src.id_paiement,
           GROUP_CONCAT(
               CONCAT('[Avoir ', FORMAT(la.montant, 2, 'fr_FR'), ' € utilisé le ',
                      DATE_FORMAT(la.date_utilisation, '%d/%m/%Y'),
                      COALESCE(CONCAT(' par ', u.nom_affichage), ''),
                      ' pour ', sr.code_seance, ']')
               ORDER BY la.date_utilisation, la.id_lia_paiements_avoirs SEPARATOR ' ') AS mentions
    FROM lia_paiements_avoirs la
    JOIN lia_paiements_seances src ON src.id_lia_paiements_seances = la.id_lia_paiements_seances_source
    JOIN lia_paiements_seances lr  ON lr.id_paiement = la.id_paiement_avoir
    JOIN seances sr ON sr.id_seance = lr.id_seance
    LEFT JOIN sec_utilisateurs u ON u.id_utilisateur = la.id_utilisateur
    GROUP BY src.id_paiement
) m ON m.id_paiement = p.id_paiement
SET p.commentaire = TRIM(CONCAT(COALESCE(p.commentaire, ''), ' ', m.mentions));

-- Règlements par avoir : leurs origines.
UPDATE paiements p
JOIN (
    SELECT la.id_paiement_avoir,
           CONCAT('Avoir utilisé : ',
                  GROUP_CONCAT(CONCAT(ps.code_paiement, ' (', ss.code_seance, ') ',
                                      FORMAT(la.montant, 2, 'fr_FR'), ' €')
                               ORDER BY la.id_lia_paiements_avoirs SEPARATOR ', ')) AS origines
    FROM lia_paiements_avoirs la
    JOIN lia_paiements_seances src ON src.id_lia_paiements_seances = la.id_lia_paiements_seances_source
    JOIN paiements ps ON ps.id_paiement = src.id_paiement
    JOIN seances ss ON ss.id_seance = src.id_seance
    GROUP BY la.id_paiement_avoir
) o ON o.id_paiement_avoir = p.id_paiement
SET p.commentaire = TRIM(CONCAT(COALESCE(p.commentaire, ''), ' ', o.origines));

DROP TABLE IF EXISTS `lia_paiements_avoirs`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.52.0';
