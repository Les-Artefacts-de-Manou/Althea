-- ---------------------------------------------------------------------------
-- Migration v2.31.0 - Lot 27c
-- Référentiel des situations juridiques du cabinet (ref_situations_juridiques)
-- et rattachement facultatif au cabinet (cab_cabinet.id_situation_juridique).
--
-- Règle : le comportement ne dépend jamais du code de la situation mais de ses
-- attributs. porteur_bce indique qui porte le numéro d'entreprise (BCE) :
--   CABINET   : l'entité cabinet facture (indépendant personne physique, société)
--   PRATICIEN : chaque praticien facture en son nom (cabinet de groupe)
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `ref_situations_juridiques` (
    `id_situation_juridique`      bigint(20) unsigned NOT NULL AUTO_INCREMENT,
    `code_situation_juridique`    varchar(50)  NOT NULL,
    `libelle_situation_juridique` varchar(150) NOT NULL,
    `porteur_bce`                 varchar(20)  NOT NULL DEFAULT 'CABINET' COMMENT 'CABINET ou PRATICIEN : entite qui porte le numero BCE',
    `ordre_affichage`             int(11)      NOT NULL DEFAULT 0,
    `actif`                       tinyint(1)   NOT NULL DEFAULT 1,
    `est_systeme`                 tinyint(1)   NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
    `date_creation`               datetime     NOT NULL DEFAULT current_timestamp(),
    `date_modification`           datetime     NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
    PRIMARY KEY (`id_situation_juridique`),
    UNIQUE KEY `uq_ref_situations_juridiques_code` (`code_situation_juridique`),
    UNIQUE KEY `uq_ref_situations_juridiques_libelle` (`libelle_situation_juridique`),
    CONSTRAINT `chk_ref_situations_juridiques_porteur` CHECK (`porteur_bce` IN ('CABINET', 'PRATICIEN'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

INSERT INTO `ref_situations_juridiques` (`code_situation_juridique`, `libelle_situation_juridique`, `porteur_bce`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'INDEPENDANT_PP', 'Praticien indépendant (personne physique)', 'CABINET', 10, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_situations_juridiques` WHERE `code_situation_juridique` = 'INDEPENDANT_PP');

INSERT INTO `ref_situations_juridiques` (`code_situation_juridique`, `libelle_situation_juridique`, `porteur_bce`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'SOCIETE', 'Société (SRL, SC...)', 'CABINET', 20, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_situations_juridiques` WHERE `code_situation_juridique` = 'SOCIETE');

INSERT INTO `ref_situations_juridiques` (`code_situation_juridique`, `libelle_situation_juridique`, `porteur_bce`, `ordre_affichage`, `actif`, `est_systeme`)
SELECT 'CABINET_GROUPE', 'Cabinet de groupe (chaque praticien facture en son nom)', 'PRATICIEN', 30, 1, 0 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `ref_situations_juridiques` WHERE `code_situation_juridique` = 'CABINET_GROUPE');

SET @col_existe := (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cab_cabinet' AND COLUMN_NAME = 'id_situation_juridique');

SET @sql := IF(@col_existe = 0,
    'ALTER TABLE cab_cabinet ADD COLUMN id_situation_juridique bigint(20) unsigned NULL AFTER nom_cabinet, ADD KEY IDX_cab_cabinet_situation (id_situation_juridique), ADD CONSTRAINT fk_cab_cabinet_situation FOREIGN KEY (id_situation_juridique) REFERENCES ref_situations_juridiques (id_situation_juridique) ON UPDATE CASCADE ON DELETE RESTRICT',
    'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

INSERT INTO tec_meta_schema (version_schema, description, date_application)
SELECT '2.31.0',
       'Lot 27c : referentiel ref_situations_juridiques (porteur_bce) et cab_cabinet.id_situation_juridique.',
       NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tec_meta_schema WHERE version_schema = '2.31.0');
