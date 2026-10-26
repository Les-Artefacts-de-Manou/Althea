-- ---------------------------------------------------------------------------
-- Migration v2.47.0 - Types d'identifiant professionnel contrôlés par Althéa
--
-- Contexte (20/10/2026) : l'écran Administration › Types d'identifiant rend le
-- référentiel ref_types_identifiant_pro modifiable. Trois types ont un contrôle
-- écrit dans l'application, choisi d'après leur CODE (UtilsIdentifiantBelge) :
-- INAMI, BCE et TVA. L'écran Cabinet cherche aussi le type BCE par son code
-- (porteur du numéro BCE selon la situation juridique).
--
-- Seul INAMI était marqué « ligne système » (v2.29.0). BCE et TVA le deviennent :
-- leur code ne peut plus être modifié et ils ne peuvent plus être désactivés
-- (sens de la colonne est_systeme). Libellé, masque et ordre restent modifiables.
--
-- Règles : données seulement, aucune structure modifiée ; script rejouable.
-- Retour arrière : migration_v2.47.0_types_identifiant_systeme_ROLLBACK.sql.
-- ---------------------------------------------------------------------------

/*!40101 SET NAMES utf8mb4 */;

UPDATE ref_types_identifiant_pro
SET est_systeme = 1,
    actif = 1
WHERE code_type_identifiant IN ('INAMI', 'BCE', 'TVA');

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.47.0',
       'Types d''identifiant INAMI, BCE et TVA marques lignes systeme (controles par l''application).',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.47.0');
