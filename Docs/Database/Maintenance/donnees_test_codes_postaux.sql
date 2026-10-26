-- =================================================================================================
-- Script   : donnees_test_codes_postaux.sql
-- Projet   : Althéa
-- Date     : 24/10/2026
-- Rôle     : BASE DE TEST UNIQUEMENT. Remplace les codes postaux et localités fantaisistes des
--            données de test (patients, famille / contacts, thérapeutes) par de vrais couples belges,
--            pour valider la recherche des patients par lieu (R4) puis le contrôle code postal /
--            localité (codes postaux belges, ToDo).
--
-- MODE D'EMPLOI
--   1. Vérifier d'être connecté à la BASE DE DÉVELOPPEMENT (jamais à celle du cabinet).
--   2. Exécuter tout le script : le remplacement est enregistré (COMMIT en fin de script) ; les
--      SELECT de contrôle montrent les couples code postal / localité avant et après (aucun nom).
--   Le script peut être relancé : il donne le même résultat sur les mêmes données.
--
-- Règles :
--   - Seules les adresses en Belgique (ou sans pays) qui ont déjà un code postal ou une localité
--     sont modifiées ; une adresse vide le reste (cas « sans adresse » à garder pour les tests).
--   - Les couples sont attribués à tour de rôle, dans l'ordre des identifiants : le résultat est le
--     même à chaque exécution sur les mêmes données. Plusieurs couples partagent un code postal
--     (4317 Faimes et ses sections ; 4000 / 4020 Liège) pour tester les deux sens de la recherche.
--   - date_modification est conservée (sinon l'ordre de la liste des patients changerait).
--   - Rues et numéros ne sont pas touchés. Cabinet et praticien ne sont pas touchés.
-- =================================================================================================

SET @id_belgique := (SELECT id_pays FROM ref_pays WHERE code_pays = 'BE' LIMIT 1);

-- ================================================================================= PARTIE 1 : APERÇU
SELECT @id_belgique AS id_belgique;

SELECT 'patients' AS table_source, COUNT(*) AS adresses_concernees
FROM patients
WHERE (id_pays = @id_belgique OR id_pays IS NULL)
  AND (COALESCE(TRIM(code_postal), '') <> '' OR COALESCE(TRIM(localite), '') <> '');

SELECT 'famille_contacts' AS table_source, COUNT(*) AS adresses_concernees
FROM famille_contacts
WHERE (id_pays = @id_belgique OR id_pays IS NULL)
  AND (COALESCE(TRIM(code_postal), '') <> '' OR COALESCE(TRIM(localite), '') <> '');

SELECT 'therapeutes' AS table_source, COUNT(*) AS adresses_concernees
FROM therapeutes
WHERE (id_pays = @id_belgique OR id_pays IS NULL)
  AND (COALESCE(TRIM(code_postal), '') <> '' OR COALESCE(TRIM(localite), '') <> '');

-- Couples actuels (codes et localités seulement)
SELECT code_postal, localite, COUNT(*) AS nb FROM patients
WHERE id_pays = @id_belgique OR id_pays IS NULL
GROUP BY code_postal, localite ORDER BY code_postal, localite;

-- ========================================================================= PARTIE 2 : REMPLACEMENT
DROP TEMPORARY TABLE IF EXISTS tmp_lieux;
CREATE TEMPORARY TABLE tmp_lieux (
    rang        INT          NOT NULL PRIMARY KEY,
    code_postal VARCHAR(20)  NOT NULL,
    localite    VARCHAR(100) NOT NULL
);

-- Hesbaye (cabinet à Faimes) en tête : plusieurs patients par lieu ; quelques villes plus loin,
-- dont trois en néerlandais.
INSERT INTO tmp_lieux (rang, code_postal, localite) VALUES
    ( 1, '4317', 'Faimes'),
    ( 2, '4300', 'Waremme'),
    ( 3, '4317', 'Aineffe'),
    ( 4, '4280', 'Hannut'),
    ( 5, '4317', 'Les Waleffes'),
    ( 6, '4350', 'Remicourt'),
    ( 7, '4000', 'Liège'),
    ( 8, '4300', 'Waremme'),
    ( 9, '4317', 'Borlez'),
    (10, '4360', 'Oreye'),
    (11, '4250', 'Geer'),
    (12, '4020', 'Liège'),
    (13, '4340', 'Awans'),
    (14, '4500', 'Huy'),
    (15, '4520', 'Wanze'),
    (16, '4317', 'Faimes'),
    (17, '4100', 'Seraing'),
    (18, '4460', 'Grâce-Hollogne'),
    (19, '5000', 'Namur'),
    (20, '1348', 'Louvain-la-Neuve'),
    (21, '1300', 'Wavre'),
    (22, '1050', 'Ixelles'),
    (23, '4800', 'Verviers'),
    (24, '3700', 'Tongeren'),
    (25, '3000', 'Leuven'),
    (26, '9000', 'Gent');

SET @nb_lieux := (SELECT COUNT(*) FROM tmp_lieux);

START TRANSACTION;

-- ---- patients
DROP TEMPORARY TABLE IF EXISTS tmp_rangs;
CREATE TEMPORARY TABLE tmp_rangs AS
SELECT id_patient AS id_ligne, ROW_NUMBER() OVER (ORDER BY id_patient) AS rn
FROM patients
WHERE (id_pays = @id_belgique OR id_pays IS NULL)
  AND (COALESCE(TRIM(code_postal), '') <> '' OR COALESCE(TRIM(localite), '') <> '');

UPDATE patients p
INNER JOIN tmp_rangs r ON r.id_ligne = p.id_patient
INNER JOIN tmp_lieux l ON l.rang = MOD(r.rn - 1, @nb_lieux) + 1
SET p.code_postal = l.code_postal,
    p.localite = l.localite,
    p.id_pays = @id_belgique,
    p.date_modification = p.date_modification;

-- ---- famille / contacts (décalés de 3 pour ne pas tous habiter chez le patient)
DROP TEMPORARY TABLE IF EXISTS tmp_rangs;
CREATE TEMPORARY TABLE tmp_rangs AS
SELECT id_famille_contact AS id_ligne, ROW_NUMBER() OVER (ORDER BY id_famille_contact) AS rn
FROM famille_contacts
WHERE (id_pays = @id_belgique OR id_pays IS NULL)
  AND (COALESCE(TRIM(code_postal), '') <> '' OR COALESCE(TRIM(localite), '') <> '');

UPDATE famille_contacts c
INNER JOIN tmp_rangs r ON r.id_ligne = c.id_famille_contact
INNER JOIN tmp_lieux l ON l.rang = MOD(r.rn + 2, @nb_lieux) + 1
SET c.code_postal = l.code_postal,
    c.localite = l.localite,
    c.id_pays = @id_belgique,
    c.date_modification = c.date_modification;

-- ---- thérapeutes (décalés de 6)
DROP TEMPORARY TABLE IF EXISTS tmp_rangs;
CREATE TEMPORARY TABLE tmp_rangs AS
SELECT id_therapeute AS id_ligne, ROW_NUMBER() OVER (ORDER BY id_therapeute) AS rn
FROM therapeutes
WHERE (id_pays = @id_belgique OR id_pays IS NULL)
  AND (COALESCE(TRIM(code_postal), '') <> '' OR COALESCE(TRIM(localite), '') <> '');

UPDATE therapeutes t
INNER JOIN tmp_rangs r ON r.id_ligne = t.id_therapeute
INNER JOIN tmp_lieux l ON l.rang = MOD(r.rn + 5, @nb_lieux) + 1
SET t.code_postal = l.code_postal,
    t.localite = l.localite,
    t.id_pays = @id_belgique,
    t.date_modification = t.date_modification;

-- ---- Contrôle : couples après remplacement (codes et localités seulement)
SELECT 'patients' AS table_source, code_postal, localite, COUNT(*) AS nb FROM patients
WHERE id_pays = @id_belgique GROUP BY code_postal, localite ORDER BY code_postal, localite;

SELECT 'famille_contacts' AS table_source, code_postal, localite, COUNT(*) AS nb FROM famille_contacts
WHERE id_pays = @id_belgique GROUP BY code_postal, localite ORDER BY code_postal, localite;

SELECT 'therapeutes' AS table_source, code_postal, localite, COUNT(*) AS nb FROM therapeutes
WHERE id_pays = @id_belgique GROUP BY code_postal, localite ORDER BY code_postal, localite;

-- Enregistrement définitif (sans COMMIT, la fermeture de la connexion annulerait tout).
COMMIT;

DROP TEMPORARY TABLE IF EXISTS tmp_rangs;
DROP TEMPORARY TABLE IF EXISTS tmp_lieux;
