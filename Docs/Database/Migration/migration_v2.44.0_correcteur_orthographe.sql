-- ---------------------------------------------------------------------------
-- Migration v2.44.0 - Éditeur de texte, temps 2 : correcteur d'orthographe
--
-- Le correcteur d'orthographe de Windows est actif dans tous les éditeurs de
-- texte, en saisie (choix de Joëlle, 20/10/2026, après essai). Un réglage permet
-- de le couper si les soulignements gênent.
--
-- 1. Groupe de paramètres EDITEUR (« Éditeur de texte »), après Documents imprimés.
-- 2. Paramètre EDITEUR_CORRECTEUR_ORTHOGRAPHE (booléen, 1 = actif), modifiable
--    par le SuperUser.
--
-- Règles : script rejouable ; aucune structure modifiée, aucune donnée existante
-- touchée. Retour arrière : migration_v2.44.0_correcteur_orthographe_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

INSERT INTO tec_groupes_parametres (code_groupe, libelle_groupe, ordre_affichage)
SELECT 'EDITEUR', 'Éditeur de texte', 46
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_groupes_parametres WHERE code_groupe = 'EDITEUR');

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'EDITEUR_CORRECTEUR_ORTHOGRAPHE', 'Correcteur d''orthographe', 'EDITEUR', 'BOOL',
       '1', NULL, NULL,
       'Coché : les mots inconnus sont soulignés en rouge pendant la saisie, avec des suggestions au clic droit (correcteur de Windows, dans la langue du clavier). Décoché : pas de soulignement. S''applique aux écrans ouverts ensuite.',
       1, 1, 10
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'EDITEUR_CORRECTEUR_ORTHOGRAPHE');

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.44.0',
       'Editeur temps 2 : groupe EDITEUR et parametre EDITEUR_CORRECTEUR_ORTHOGRAPHE.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.44.0');
