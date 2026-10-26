-- =================================================================================================
-- Script   : nettoyage_tests_domaine_NEW_CARRE.sql
-- Projet   : Althéa
-- Rôle     : Supprime les données de test créées lors des essais de l'écran Domaines
--            (domaine NEW, organisme / tarif / nature / catégorie « CARRE », quotas associés).
--
-- MODE D'EMPLOI
--   1. Ajuster si besoin les variables ci-dessous (codes réellement saisis).
--   2. Exécuter la PARTIE 1 seule : elle LISTE ce qui sera supprimé. Vérifier attentivement.
--   3. Exécuter la PARTIE 2 : suppression dans une transaction.
--      Vérifier les SELECT de contrôle, puis COMMIT (ou ROLLBACK en cas de doute).
--
-- Sécurité :
--   - Rien n'est supprimé si une séance, un dossier ou un patient utilise ces éléments
--     (la clé étrangère bloque et l'erreur s'affiche : faire alors ROLLBACK).
--   - Les filtres sont sur les CODES exacts ; aucun LIKE sur les tables métier existantes.
-- =================================================================================================

-- ---------------------------------------------------------------------------- Variables à vérifier
-- Domaine de test « Nouveau » : identifiant connu.
SET @id_domaine   := 6;

-- L'organisme est retrouvé depuis le domaine (inutile de connaître son code).
SET @id_organisme := (SELECT id_organisme FROM ref_domaines WHERE id_domaine = @id_domaine LIMIT 1);

-- Codes exacts de la nature et du tarif de test (à corriger d'après les requêtes d'aide).
SET @code_nature  := 'CARRE_TRAITEMNT';
SET @code_tarif   := 'CARRE';

SET @id_nature    := (SELECT id_nature_seance FROM ref_natures_seance WHERE code_nature_seance = @code_nature LIMIT 1);
SET @id_tarif     := (SELECT id_prix_seance   FROM ref_prix_seance    WHERE code_prix_seance   = @code_tarif  LIMIT 1);

-- ================================================================================= PARTIE 1 : APERÇU
SELECT @id_domaine AS id_domaine, @id_organisme AS id_organisme, @id_nature AS id_nature, @id_tarif AS id_tarif;

-- Aide : requêtes séparées (pas d'UNION : collations différentes entre tables, erreur 1271)
SELECT id_domaine, code_domaine, libelle_domaine, id_organisme FROM ref_domaines WHERE id_domaine = @id_domaine;
SELECT id_organisme, code_organisme, libelle_organisme FROM ref_organismes WHERE id_organisme = @id_organisme;
SELECT id_nature_seance, code_nature_seance, libelle_nature_seance FROM ref_natures_seance WHERE code_nature_seance LIKE '%CARR%' OR libelle_nature_seance LIKE '%arr%';
SELECT id_prix_seance, code_prix_seance, libelle_prix_seance FROM ref_prix_seance WHERE code_prix_seance LIKE '%CARR%' OR libelle_prix_seance LIKE '%arr%';

-- Éléments rattachés au domaine de test
SELECT 'categorie' AS type, id_categorie_quota AS id, code_categorie AS code, libelle_categorie AS libelle FROM ref_categories_quota WHERE id_domaine = @id_domaine;
SELECT 'quota' AS type, q.* FROM ref_quotas q WHERE q.id_domaine = @id_domaine;
SELECT 'lia_nature' AS type, l.* FROM lia_natures_seance_domaines l WHERE l.id_domaine = @id_domaine OR l.id_nature_seance = @id_nature;
SELECT 'lia_tarif'  AS type, l.* FROM lia_prix_seance_domaines   l WHERE l.id_domaine = @id_domaine OR l.id_prix_seance   = @id_tarif;

-- Blocages éventuels (doivent tous valoir 0)
SELECT
    (SELECT COUNT(*) FROM seances  WHERE id_nature_seance = @id_nature OR id_prix_seance = @id_tarif) AS seances_liees,
    (SELECT COUNT(*) FROM dossiers WHERE id_domaine = @id_domaine)                                     AS dossiers_lies,
    (SELECT COUNT(*) FROM patients WHERE id_prix_seance_defaut = @id_tarif)                            AS patients_tarif_defaut,
    (SELECT COUNT(*) FROM modeles_documents WHERE id_domaine = @id_domaine)                            AS modeles_lies,
    AND id_domaine <> @id_domaine)

-- ============================================================================ PARTIE 2 : SUPPRESSION
START TRANSACTION;

-- Plafonds puis règles de quota du domaine
DELETE p FROM ref_quotas_plafonds p
    JOIN ref_quotas q ON q.id_quota = p.id_quota
    WHERE q.id_domaine = @id_domaine;
DELETE FROM ref_quotas WHERE id_domaine = @id_domaine;

-- Rattachements (natures / tarifs) du domaine et des éléments de test
DELETE FROM lia_natures_seance_domaines WHERE id_domaine = @id_domaine OR id_nature_seance = @id_nature;
DELETE FROM lia_prix_seance_domaines    WHERE id_domaine = @id_domaine OR id_prix_seance   = @id_tarif;

-- Nature de test (avant les catégories : ancienne colonne ref_natures_seance.id_categorie_quota)
DELETE FROM ref_natures_seance WHERE id_nature_seance = @id_nature;
UPDATE ref_natures_seance SET id_categorie_quota = NULL
    WHERE id_categorie_quota IN (SELECT id_categorie_quota FROM ref_categories_quota WHERE id_domaine = @id_domaine);

-- Catégories de quota du domaine
DELETE FROM ref_categories_quota WHERE id_domaine = @id_domaine;

-- Tarif, domaine, puis organisme (seulement s'il n'est plus utilisé)
DELETE FROM ref_prix_seance WHERE id_prix_seance = @id_tarif;
DELETE FROM ref_domaines    WHERE id_domaine     = @id_domaine;
DELETE FROM ref_organismes  WHERE id_organisme   = @id_organisme
    AND NOT EXISTS (SELECT 1 FROM ref_domaines d WHERE d.id_organisme = @id_organisme);

-- Contrôles (tout doit valoir 0)
SELECT
    (SELECT COUNT(*) FROM ref_domaines       WHERE id_domaine       = @id_domaine)   AS domaine_restant,
    (SELECT COUNT(*) FROM ref_natures_seance WHERE id_nature_seance = @id_nature)    AS nature_restante,
    (SELECT COUNT(*) FROM ref_prix_seance    WHERE id_prix_seance   = @id_tarif)     AS tarif_restant,
    (SELECT COUNT(*) FROM ref_organismes     WHERE id_organisme     = @id_organisme) AS organisme_restant;

-- Si tout est correct :
-- COMMIT;
-- Sinon :
-- ROLLBACK;
