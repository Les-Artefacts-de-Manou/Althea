-- Rollback v2.30.0 - Lot 27b

ALTER TABLE cab_praticiens
    ADD COLUMN `couleur_agenda` varchar(9) NULL COMMENT 'Couleur #RRGGBB pour l''agenda' AFTER `chemin_signature`;

DELETE FROM tec_meta_schema WHERE version_schema = '2.30.0';
