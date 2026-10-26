-- ---------------------------------------------------------------------------
-- Migration v2.41.0 - Éditeur de texte, temps 2 : en-tête et pied de page
--
-- Les documents imprimés ou exportés depuis l'éditeur de texte (anamnèse, zones
-- du dossier) portent un en-tête (cabinet, praticien, titre, patient) et un pied
-- de page (mention, date d'édition, page n / N). La mention est un paramètre :
-- elle peut changer (« Confidentiel », « Confidentiel — secret professionnel »…).
--
-- 1. Groupe de paramètres DOCUMENTS (« Documents imprimés »), après Cabinet.
-- 2. Paramètre DOCUMENT_MENTION_CONFIDENTIALITE (texte, « Confidentiel »),
--    modifiable par le SuperUser ; vide = pas de mention.
--
-- Règles : script rejouable ; aucune structure modifiée, aucune donnée existante
-- touchée. Retour arrière : migration_v2.41.0_mention_documents_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

-- ---------------------------------------------------------------------------
-- 1. Groupe DOCUMENTS
-- ---------------------------------------------------------------------------

INSERT INTO tec_groupes_parametres (code_groupe, libelle_groupe, ordre_affichage)
SELECT 'DOCUMENTS', 'Documents imprimés', 45
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_groupes_parametres WHERE code_groupe = 'DOCUMENTS');

-- ---------------------------------------------------------------------------
-- 2. Mention de confidentialité
-- ---------------------------------------------------------------------------

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'DOCUMENT_MENTION_CONFIDENTIALITE', 'Mention en pied de page', 'DOCUMENTS', 'STRING',
       'Confidentiel', NULL, NULL,
       'Imprimée à gauche du pied de page des documents (impression, aperçu, PDF, Word), à côté de la date d''édition et du numéro de page. Vide = pas de mention.',
       1, 1, 10
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'DOCUMENT_MENTION_CONFIDENTIALITE');

-- ---------------------------------------------------------------------------
-- Journalisation
-- ---------------------------------------------------------------------------

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.41.0',
       'Editeur temps 2 : groupe DOCUMENTS et parametre DOCUMENT_MENTION_CONFIDENTIALITE (pied de page des documents).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.41.0');
