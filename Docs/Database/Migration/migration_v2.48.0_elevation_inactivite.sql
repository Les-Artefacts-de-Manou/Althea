-- ---------------------------------------------------------------------------
-- Migration v2.48.0 - Fin de l'élévation après inactivité
--
-- Décision de Joëlle (20/10/2026, audit A.5) : une élévation de droits (cadenas)
-- prend fin après un temps sans activité au clavier ni à la souris ; la session
-- revient au rôle de base, avec un message. Tant qu'on travaille, elle reste.
--
-- 1. Paramètre SECURITE_ELEVATION_MINUTES (INT, 15, bornes 1 à 240), groupe
--    SECURITE, modifiable par l'Admin seulement.
--
-- Les mots de passe faux pendant une élévation comptent désormais dans le même
-- compteur que la connexion (SECURITE_MAX_ECHECS_LOGIN) : aucune structure à
-- ajouter pour cela.
--
-- Règles : données seulement ; script rejouable. Retour arrière :
-- migration_v2.48.0_elevation_inactivite_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

INSERT INTO tec_parametres (cle_parametre, libelle_parametre, groupe_parametre, type_valeur,
                            valeur_parametre, valeur_min, valeur_max, description_parametre,
                            modifiable_utilisateur, actif, ordre_affichage)
SELECT 'SECURITE_ELEVATION_MINUTES', 'Fin de l''élévation après inactivité (minutes)', 'SECURITE', 'INT',
       '15', 1, 240,
       'Une élévation de droits (cadenas) prend fin après ce nombre de minutes sans activité au clavier ni à la souris : la session revient au rôle de base, avec un message.',
       0, 1, 20
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tec_parametres WHERE cle_parametre = 'SECURITE_ELEVATION_MINUTES');

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.48.0',
       'Parametre SECURITE_ELEVATION_MINUTES (fin de l''elevation apres inactivite).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.48.0');
