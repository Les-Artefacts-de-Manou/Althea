/*M!999999\- enable the sandbox mode */ 
-- MariaDB dump 10.20-12.3.3-MariaDB, for Win64 (AMD64)
--
-- Host: localhost    Database: althea
-- ------------------------------------------------------
-- Server version	12.3.3-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*M!100616 SET @OLD_NOTE_VERBOSITY=@@NOTE_VERBOSITY, NOTE_VERBOSITY=0 */;

--
-- Sequence structure for `seq_autres_suivis_patient`
--

DROP SEQUENCE IF EXISTS `seq_autres_suivis_patient`;
CREATE SEQUENCE `seq_autres_suivis_patient` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_autres_suivis_patient`, 9, 0);

--
-- Sequence structure for `seq_documents`
--

DROP SEQUENCE IF EXISTS `seq_documents`;
CREATE SEQUENCE `seq_documents` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_documents`, 15, 0);

--
-- Sequence structure for `seq_dossiers`
--

DROP SEQUENCE IF EXISTS `seq_dossiers`;
CREATE SEQUENCE `seq_dossiers` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_dossiers`, 27, 0);

--
-- Sequence structure for `seq_famille_contacts`
--

DROP SEQUENCE IF EXISTS `seq_famille_contacts`;
CREATE SEQUENCE `seq_famille_contacts` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_famille_contacts`, 11, 0);

--
-- Sequence structure for `seq_lia_paiements_seances`
--

DROP SEQUENCE IF EXISTS `seq_lia_paiements_seances`;
CREATE SEQUENCE `seq_lia_paiements_seances` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_lia_paiements_seances`, 95, 0);

--
-- Sequence structure for `seq_modeles_documents`
--

DROP SEQUENCE IF EXISTS `seq_modeles_documents`;
CREATE SEQUENCE `seq_modeles_documents` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_modeles_documents`, 1, 0);

--
-- Sequence structure for `seq_paiements`
--

DROP SEQUENCE IF EXISTS `seq_paiements`;
CREATE SEQUENCE `seq_paiements` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_paiements`, 89, 0);

--
-- Sequence structure for `seq_patients`
--

DROP SEQUENCE IF EXISTS `seq_patients`;
CREATE SEQUENCE `seq_patients` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_patients`, 25, 0);

--
-- Sequence structure for `seq_rendez_vous`
--

DROP SEQUENCE IF EXISTS `seq_rendez_vous`;
CREATE SEQUENCE `seq_rendez_vous` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_rendez_vous`, 1, 0);

--
-- Sequence structure for `seq_seances`
--

DROP SEQUENCE IF EXISTS `seq_seances`;
CREATE SEQUENCE `seq_seances` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_seances`, 66, 0);

--
-- Sequence structure for `seq_sec_utilisateurs`
--

DROP SEQUENCE IF EXISTS `seq_sec_utilisateurs`;
CREATE SEQUENCE `seq_sec_utilisateurs` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_sec_utilisateurs`, 17, 0);

--
-- Sequence structure for `seq_tec_meta_schema`
--

DROP SEQUENCE IF EXISTS `seq_tec_meta_schema`;
CREATE SEQUENCE `seq_tec_meta_schema` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_tec_meta_schema`, 44, 0);

--
-- Sequence structure for `seq_tec_parametres`
--

DROP SEQUENCE IF EXISTS `seq_tec_parametres`;
CREATE SEQUENCE `seq_tec_parametres` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_tec_parametres`, 13, 0);

--
-- Sequence structure for `seq_tec_sync_evenements_google`
--

DROP SEQUENCE IF EXISTS `seq_tec_sync_evenements_google`;
CREATE SEQUENCE `seq_tec_sync_evenements_google` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_tec_sync_evenements_google`, 1, 0);

--
-- Sequence structure for `seq_therapeutes`
--

DROP SEQUENCE IF EXISTS `seq_therapeutes`;
CREATE SEQUENCE `seq_therapeutes` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_therapeutes`, 9, 0);

--
-- Table structure for table `autres_suivis_patient`
--

DROP TABLE IF EXISTS `autres_suivis_patient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `autres_suivis_patient` (
  `id_autre_suivi_patient` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_autres_suivis_patient`),
  `code_autre_suivi_patient` varchar(12) GENERATED ALWAYS AS (concat('AS',lpad(`id_autre_suivi_patient`,6,'0'))) STORED,
  `id_patient` bigint(20) unsigned NOT NULL,
  `id_role_intervenant` bigint(20) unsigned DEFAULT NULL COMMENT 'Rôle de cet intervenant dans le réseau de suivi (ref_roles_intervenant)',
  `id_therapeute` bigint(20) unsigned DEFAULT NULL COMMENT 'Thérapeute connu dans le référentiel therapeutes (nullable)',
  `id_civilite` bigint(20) unsigned DEFAULT NULL,
  `nom_professionnel` varchar(150) DEFAULT NULL,
  `specialite` varchar(100) DEFAULT NULL,
  `lieu` varchar(150) DEFAULT NULL,
  `telephone` varchar(50) DEFAULT NULL,
  `date_debut` date DEFAULT NULL,
  `date_fin` date DEFAULT NULL,
  `commentaire_rtf` longtext DEFAULT NULL COMMENT 'Commentaire au format RTF (D-Q7bis)',
  `commentaire_txt` longtext DEFAULT NULL COMMENT 'Commentaire en texte brut pour la recherche (D-Q7bis)',
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_autre_suivi_patient`),
  UNIQUE KEY `uq_autres_suivis_patient_code` (`code_autre_suivi_patient`),
  KEY `fk_autres_suivis_patient_patients` (`id_patient`),
  KEY `fk_autres_suivis_ref_roles` (`id_role_intervenant`),
  KEY `fk_autres_suivis_therapeutes` (`id_therapeute`),
  KEY `IDX_autres_suivis_civilite` (`id_civilite`),
  CONSTRAINT `fk_autres_suivis_civilite` FOREIGN KEY (`id_civilite`) REFERENCES `ref_civilites` (`id_civilite`) ON UPDATE CASCADE,
  CONSTRAINT `fk_autres_suivis_patient_patients` FOREIGN KEY (`id_patient`) REFERENCES `patients` (`id_patient`),
  CONSTRAINT `fk_autres_suivis_ref_roles` FOREIGN KEY (`id_role_intervenant`) REFERENCES `ref_roles_intervenant` (`id_role_intervenant`),
  CONSTRAINT `fk_autres_suivis_therapeutes` FOREIGN KEY (`id_therapeute`) REFERENCES `therapeutes` (`id_therapeute`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cab_cabinet`
--

DROP TABLE IF EXISTS `cab_cabinet`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `cab_cabinet` (
  `id_cabinet` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `nom_cabinet` varchar(150) NOT NULL,
  `id_situation_juridique` bigint(20) unsigned DEFAULT NULL,
  `adresse_rue` varchar(150) DEFAULT NULL,
  `adresse_numero` varchar(20) DEFAULT NULL,
  `code_postal` varchar(20) DEFAULT NULL,
  `localite` varchar(100) DEFAULT NULL,
  `id_pays` bigint(20) unsigned DEFAULT NULL,
  `telephone` varchar(30) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `site_web` varchar(255) DEFAULT NULL,
  `numero_bce` varchar(30) DEFAULT NULL,
  `numero_tva` varchar(30) DEFAULT NULL,
  `iban` varchar(34) DEFAULT NULL,
  `bic` varchar(11) DEFAULT NULL,
  `chemin_logo` varchar(255) DEFAULT NULL COMMENT 'Chemin relatif du logo dans le stockage applicatif',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime DEFAULT NULL ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_cabinet`),
  KEY `IDX_cab_cabinet_situation` (`id_situation_juridique`),
  KEY `IDX_cab_cabinet_pays` (`id_pays`),
  CONSTRAINT `fk_cab_cabinet_pays` FOREIGN KEY (`id_pays`) REFERENCES `ref_pays` (`id_pays`) ON UPDATE CASCADE,
  CONSTRAINT `fk_cab_cabinet_situation` FOREIGN KEY (`id_situation_juridique`) REFERENCES `ref_situations_juridiques` (`id_situation_juridique`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Identite du cabinet (V1 : une seule ligne active)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cab_praticien_identifiants`
--

DROP TABLE IF EXISTS `cab_praticien_identifiants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `cab_praticien_identifiants` (
  `id_identifiant` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_praticien` bigint(20) unsigned NOT NULL,
  `id_type_identifiant` bigint(20) unsigned NOT NULL,
  `valeur` varchar(50) NOT NULL,
  `date_debut` date DEFAULT NULL,
  `date_fin` date DEFAULT NULL,
  `afficher_sur_cachet` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_identifiant`),
  KEY `IDX_cab_praticien_identifiants_praticien` (`id_praticien`),
  KEY `IDX_cab_praticien_identifiants_type` (`id_type_identifiant`),
  CONSTRAINT `fk_cab_praticien_identifiants_praticien` FOREIGN KEY (`id_praticien`) REFERENCES `cab_praticiens` (`id_praticien`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_cab_praticien_identifiants_type` FOREIGN KEY (`id_type_identifiant`) REFERENCES `ref_types_identifiant_pro` (`id_type_identifiant`) ON UPDATE CASCADE,
  CONSTRAINT `chk_cab_praticien_identifiants_dates` CHECK (`date_fin` is null or `date_debut` is null or `date_fin` >= `date_debut`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Numeros professionnels du praticien, avec periode de validite';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cab_praticien_qualifications`
--

DROP TABLE IF EXISTS `cab_praticien_qualifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `cab_praticien_qualifications` (
  `id_qualification` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_praticien` bigint(20) unsigned NOT NULL,
  `libelle` varchar(200) NOT NULL COMMENT 'Ex. Master en psychologie clinique',
  `etablissement` varchar(150) DEFAULT NULL,
  `annee_obtention` smallint(6) DEFAULT NULL,
  `afficher_sur_cachet` tinyint(1) NOT NULL DEFAULT 0,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_qualification`),
  KEY `IDX_cab_praticien_qualifications_praticien` (`id_praticien`),
  CONSTRAINT `fk_cab_praticien_qualifications_praticien` FOREIGN KEY (`id_praticien`) REFERENCES `cab_praticiens` (`id_praticien`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Diplomes et titres du praticien';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cab_praticiens`
--

DROP TABLE IF EXISTS `cab_praticiens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `cab_praticiens` (
  `id_praticien` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_cabinet` bigint(20) unsigned NOT NULL,
  `id_utilisateur` bigint(20) unsigned DEFAULT NULL COMMENT 'Compte de travail du praticien (sec_utilisateurs) ; NULL = non lie',
  `id_civilite` bigint(20) unsigned DEFAULT NULL,
  `nom` varchar(100) NOT NULL,
  `prenom` varchar(100) NOT NULL,
  `titre_professionnel` varchar(200) DEFAULT NULL COMMENT 'Ligne de titres affichee (cachet, documents)',
  `telephone` varchar(30) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `site_web` varchar(255) DEFAULT NULL,
  `adresse_rue` varchar(150) DEFAULT NULL COMMENT 'Vide = adresse du cabinet',
  `adresse_numero` varchar(20) DEFAULT NULL,
  `code_postal` varchar(20) DEFAULT NULL,
  `localite` varchar(100) DEFAULT NULL,
  `id_pays` bigint(20) unsigned DEFAULT NULL,
  `chemin_photo` varchar(255) DEFAULT NULL,
  `chemin_cachet` varchar(255) DEFAULT NULL COMMENT 'Cachet importe ; vide = cachet genere depuis les donnees',
  `chemin_signature` varchar(255) DEFAULT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime DEFAULT NULL ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_praticien`),
  UNIQUE KEY `uq_cab_praticiens_utilisateur` (`id_utilisateur`),
  KEY `IDX_cab_praticiens_cabinet` (`id_cabinet`),
  KEY `IDX_cab_praticiens_pays` (`id_pays`),
  KEY `IDX_cab_praticiens_civilite` (`id_civilite`),
  CONSTRAINT `fk_cab_praticiens_cabinet` FOREIGN KEY (`id_cabinet`) REFERENCES `cab_cabinet` (`id_cabinet`) ON UPDATE CASCADE,
  CONSTRAINT `fk_cab_praticiens_civilite` FOREIGN KEY (`id_civilite`) REFERENCES `ref_civilites` (`id_civilite`) ON UPDATE CASCADE,
  CONSTRAINT `fk_cab_praticiens_pays` FOREIGN KEY (`id_pays`) REFERENCES `ref_pays` (`id_pays`) ON UPDATE CASCADE,
  CONSTRAINT `fk_cab_praticiens_utilisateur` FOREIGN KEY (`id_utilisateur`) REFERENCES `sec_utilisateurs` (`id_utilisateur`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Praticiens du cabinet (V1 : un seul ; schema multi-praticiens)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `documents`
--

DROP TABLE IF EXISTS `documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `documents` (
  `id_document` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_documents`),
  `code_document` varchar(12) GENERATED ALWAYS AS (concat('DOC',lpad(`id_document`,6,'0'))) STORED,
  `id_dossier` bigint(20) unsigned DEFAULT NULL,
  `id_patient` bigint(20) unsigned DEFAULT NULL,
  `id_seance` bigint(20) unsigned DEFAULT NULL,
  `id_rendez_vous` bigint(20) unsigned DEFAULT NULL,
  `id_type_document` bigint(20) unsigned NOT NULL,
  `type_source_document` varchar(50) NOT NULL DEFAULT 'LOCAL' COMMENT 'LOCAL_DOCX, GOOGLE_DOC, PDF, IMAGE, AUTRE',
  `est_document_image_metier` tinyint(1) NOT NULL DEFAULT 0,
  `id_modele_document` bigint(20) unsigned DEFAULT NULL,
  `nom_document` varchar(255) NOT NULL,
  `nom_fichier` varchar(255) NOT NULL,
  `extension_fichier` varchar(10) DEFAULT NULL,
  `taille_fichier_octets` bigint(20) DEFAULT NULL,
  `chemin_relatif` varchar(500) NOT NULL,
  `chemin_pdf_relatif` text DEFAULT NULL,
  `chemin_miniature_relatif` text DEFAULT NULL,
  `google_file_id` varchar(255) DEFAULT NULL,
  `google_mime_type` varchar(150) DEFAULT NULL,
  `google_web_link` text DEFAULT NULL,
  `google_drive_folder_id` varchar(255) DEFAULT NULL,
  `statut_sync_google` varchar(30) NOT NULL DEFAULT 'LOCAL_ONLY' COMMENT 'LOCAL_ONLY, SYNC_OK, A_RESYNC, ERREUR_SYNC',
  `date_sync_google` datetime DEFAULT NULL,
  `date_document` date DEFAULT NULL,
  `commentaire` text DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_document`),
  UNIQUE KEY `uq_documents_code_document` (`code_document`),
  KEY `fk_documents_dossiers` (`id_dossier`),
  KEY `fk_documents_seances` (`id_seance`),
  KEY `fk_documents_ref_types_documents` (`id_type_document`),
  KEY `fk_documents_rendez_vous` (`id_rendez_vous`),
  KEY `fk_documents_patients` (`id_patient`),
  CONSTRAINT `fk_documents_dossiers` FOREIGN KEY (`id_dossier`) REFERENCES `dossiers` (`id_dossier`),
  CONSTRAINT `fk_documents_patients` FOREIGN KEY (`id_patient`) REFERENCES `patients` (`id_patient`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_documents_ref_types_documents` FOREIGN KEY (`id_type_document`) REFERENCES `ref_types_documents` (`id_type_document`),
  CONSTRAINT `fk_documents_rendez_vous` FOREIGN KEY (`id_rendez_vous`) REFERENCES `rendez_vous` (`id_rendez_vous`),
  CONSTRAINT `fk_documents_seances` FOREIGN KEY (`id_seance`) REFERENCES `seances` (`id_seance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dossiers`
--

DROP TABLE IF EXISTS `dossiers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `dossiers` (
  `id_dossier` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_dossiers`),
  `code_dossier` varchar(12) GENERATED ALWAYS AS (concat('DO',lpad(`id_dossier`,6,'0'))) STORED,
  `id_patient` bigint(20) unsigned NOT NULL,
  `id_domaine` bigint(20) unsigned NOT NULL,
  `id_prix_seance` bigint(20) unsigned DEFAULT NULL COMMENT 'Tarif patient applicable aux séances de ce dossier (ref_prix_seance) — D-TAR-1/T-1',
  `id_statut_dossier` bigint(20) unsigned NOT NULL,
  `date_ouverture` date NOT NULL,
  `date_cloture` date DEFAULT NULL,
  `date_archivage` date DEFAULT NULL,
  `modalites_paiement` varchar(150) DEFAULT NULL,
  `id_therapeute_traitant` bigint(20) unsigned DEFAULT NULL,
  `motif_suivi_rtf` longtext DEFAULT NULL,
  `motif_suivi_txt` longtext DEFAULT NULL,
  `historique_rtf` longtext DEFAULT NULL,
  `historique_txt` longtext DEFAULT NULL,
  `antecedents_rtf` longtext DEFAULT NULL,
  `antecedents_txt` longtext DEFAULT NULL,
  `diagnostic_rtf` longtext DEFAULT NULL,
  `diagnostic_txt` longtext DEFAULT NULL,
  `traitements_en_cours_rtf` longtext DEFAULT NULL,
  `traitements_en_cours_txt` longtext DEFAULT NULL,
  `objectifs_suivi_rtf` longtext DEFAULT NULL,
  `objectifs_suivi_txt` longtext DEFAULT NULL,
  `historique_archive_rtf` longtext DEFAULT NULL,
  `historique_archive_txt` longtext DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_dossier`),
  UNIQUE KEY `uq_dossiers_code_dossier` (`code_dossier`),
  KEY `fk_dossiers_patients` (`id_patient`),
  KEY `fk_dossiers_ref_statuts_dossier` (`id_statut_dossier`),
  KEY `fk_dossiers_ref_domaines` (`id_domaine`),
  KEY `fk_dossiers_therapeutes` (`id_therapeute_traitant`),
  KEY `fk_dossiers_ref_prix_seance` (`id_prix_seance`),
  CONSTRAINT `fk_dossiers_patients` FOREIGN KEY (`id_patient`) REFERENCES `patients` (`id_patient`),
  CONSTRAINT `fk_dossiers_ref_domaines` FOREIGN KEY (`id_domaine`) REFERENCES `ref_domaines` (`id_domaine`),
  CONSTRAINT `fk_dossiers_ref_prix_seance` FOREIGN KEY (`id_prix_seance`) REFERENCES `ref_prix_seance` (`id_prix_seance`),
  CONSTRAINT `fk_dossiers_ref_statuts_dossier` FOREIGN KEY (`id_statut_dossier`) REFERENCES `ref_statuts_dossier` (`id_statut_dossier`),
  CONSTRAINT `fk_dossiers_therapeutes` FOREIGN KEY (`id_therapeute_traitant`) REFERENCES `therapeutes` (`id_therapeute`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `famille_contacts`
--

DROP TABLE IF EXISTS `famille_contacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `famille_contacts` (
  `id_famille_contact` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_famille_contacts`),
  `code_famille_contact` varchar(12) GENERATED ALWAYS AS (concat('FC',lpad(`id_famille_contact`,6,'0'))) STORED,
  `id_patient` bigint(20) unsigned NOT NULL,
  `id_lien_patient` bigint(20) unsigned NOT NULL,
  `id_civilite` bigint(20) unsigned DEFAULT NULL,
  `nom` varchar(100) NOT NULL,
  `prenom` varchar(100) NOT NULL,
  `date_naissance` date DEFAULT NULL,
  `telephone` varchar(50) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `adresse_ligne1` varchar(255) DEFAULT NULL,
  `adresse_ligne2` varchar(255) DEFAULT NULL,
  `code_postal` varchar(20) DEFAULT NULL,
  `localite` varchar(100) DEFAULT NULL,
  `id_pays` bigint(20) unsigned DEFAULT NULL,
  `id_role_legal` bigint(20) unsigned NOT NULL COMMENT 'Rôle légal unique du contact (ref_role_legal) — D-Q18',
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `commentaire_rtf` longtext DEFAULT NULL COMMENT 'Commentaire au format RTF (D-Q7bis)',
  `commentaire_txt` longtext DEFAULT NULL COMMENT 'Commentaire en texte brut pour la recherche (D-Q7bis)',
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_famille_contact`),
  UNIQUE KEY `uq_famille_contacts_code_famille_contact` (`code_famille_contact`),
  KEY `fk_famille_contacts_patients` (`id_patient`),
  KEY `fk_famille_contacts_ref_liens_patient` (`id_lien_patient`),
  KEY `fk_famille_contacts_ref_role_legal` (`id_role_legal`),
  KEY `IDX_famille_contacts_pays` (`id_pays`),
  KEY `IDX_famille_contacts_civilite` (`id_civilite`),
  CONSTRAINT `fk_famille_contacts_civilite` FOREIGN KEY (`id_civilite`) REFERENCES `ref_civilites` (`id_civilite`) ON UPDATE CASCADE,
  CONSTRAINT `fk_famille_contacts_patients` FOREIGN KEY (`id_patient`) REFERENCES `patients` (`id_patient`),
  CONSTRAINT `fk_famille_contacts_pays` FOREIGN KEY (`id_pays`) REFERENCES `ref_pays` (`id_pays`) ON UPDATE CASCADE,
  CONSTRAINT `fk_famille_contacts_ref_liens_patient` FOREIGN KEY (`id_lien_patient`) REFERENCES `ref_liens_patient` (`id_lien_patient`),
  CONSTRAINT `fk_famille_contacts_ref_role_legal` FOREIGN KEY (`id_role_legal`) REFERENCES `ref_role_legal` (`id_role_legal`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `lia_natures_seance_domaines`
--

DROP TABLE IF EXISTS `lia_natures_seance_domaines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `lia_natures_seance_domaines` (
  `id_lia_nature_domaine` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_nature_seance` bigint(20) unsigned NOT NULL COMMENT 'Nature de seance proposee pour le domaine — D-POR-5',
  `id_domaine` bigint(20) unsigned NOT NULL COMMENT 'Domaine auquel la nature est rattachee — D-POR-5',
  `id_categorie_quota` bigint(20) unsigned DEFAULT NULL COMMENT 'Categorie de quota consommee par la nature dans ce domaine (NULL = hors quota)',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_lia_nature_domaine`),
  UNIQUE KEY `uq_lia_nature_domaine` (`id_nature_seance`,`id_domaine`),
  KEY `fk_lia_nature_domaine_domaine` (`id_domaine`),
  KEY `IDX_lia_nature_domaine_categorie` (`id_categorie_quota`,`id_domaine`),
  CONSTRAINT `fk_lia_nature_domaine_categorie` FOREIGN KEY (`id_categorie_quota`, `id_domaine`) REFERENCES `ref_categories_quota` (`id_categorie_quota`, `id_domaine`) ON UPDATE CASCADE,
  CONSTRAINT `fk_lia_nature_domaine_domaine` FOREIGN KEY (`id_domaine`) REFERENCES `ref_domaines` (`id_domaine`) ON DELETE CASCADE,
  CONSTRAINT `fk_lia_nature_domaine_nature` FOREIGN KEY (`id_nature_seance`) REFERENCES `ref_natures_seance` (`id_nature_seance`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=49 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `lia_paiements_seances`
--

DROP TABLE IF EXISTS `lia_paiements_seances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `lia_paiements_seances` (
  `id_lia_paiements_seances` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_lia_paiements_seances`),
  `id_paiement` bigint(20) unsigned NOT NULL,
  `id_seance` bigint(20) unsigned NOT NULL,
  `montant_affecte` decimal(10,2) NOT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_lia_paiements_seances`),
  UNIQUE KEY `uq_lia_paiement_seance` (`id_paiement`,`id_seance`),
  KEY `fk_lia_paiements_seances_seances` (`id_seance`),
  CONSTRAINT `fk_lia_paiements_seances_paiements` FOREIGN KEY (`id_paiement`) REFERENCES `paiements` (`id_paiement`),
  CONSTRAINT `fk_lia_paiements_seances_seances` FOREIGN KEY (`id_seance`) REFERENCES `seances` (`id_seance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `lia_prix_seance_domaines`
--

DROP TABLE IF EXISTS `lia_prix_seance_domaines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `lia_prix_seance_domaines` (
  `id_lia_prix_seance_domaine` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_prix_seance` bigint(20) unsigned NOT NULL,
  `id_domaine` bigint(20) unsigned NOT NULL,
  `date_debut_validite` date NOT NULL DEFAULT '2000-01-01' COMMENT 'Premier jour d''application du tarif pour ce domaine — D-POR-4',
  `date_fin_validite` date DEFAULT NULL COMMENT 'Dernier jour d''application, NULL tant que le tarif est en vigueur — D-POR-4',
  `actif` tinyint(1) NOT NULL DEFAULT 1 COMMENT 'Rattachement actif (desactivation sans perte d''historique) — D-POR-4',
  PRIMARY KEY (`id_lia_prix_seance_domaine`),
  UNIQUE KEY `uq_lia_prix_domaine_debut` (`id_prix_seance`,`id_domaine`,`date_debut_validite`),
  KEY `fk_lia_prix_domaine_domaine` (`id_domaine`),
  CONSTRAINT `fk_lia_prix_domaine_domaine` FOREIGN KEY (`id_domaine`) REFERENCES `ref_domaines` (`id_domaine`) ON DELETE CASCADE,
  CONSTRAINT `fk_lia_prix_domaine_prix` FOREIGN KEY (`id_prix_seance`) REFERENCES `ref_prix_seance` (`id_prix_seance`) ON DELETE CASCADE,
  CONSTRAINT `ck_lia_prix_domaine_periode` CHECK (`date_fin_validite` is null or `date_fin_validite` >= `date_debut_validite`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `modeles_documents`
--

DROP TABLE IF EXISTS `modeles_documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `modeles_documents` (
  `id_modele_document` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_modeles_documents`),
  `code_modele_document` varchar(12) GENERATED ALWAYS AS (concat('MOD',lpad(`id_modele_document`,6,'0'))) STORED,
  `id_type_document` bigint(20) unsigned NOT NULL,
  `id_domaine` bigint(20) unsigned DEFAULT NULL,
  `nom_modele` varchar(255) NOT NULL,
  `nom_fichier` varchar(255) NOT NULL,
  `chemin_relatif` varchar(500) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `commentaire` text DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_modele_document`),
  UNIQUE KEY `uq_modeles_documents_code` (`code_modele_document`),
  KEY `fk_modeles_documents_ref_types_documents` (`id_type_document`),
  KEY `fk_modeles_documents_ref_domaines` (`id_domaine`),
  CONSTRAINT `fk_modeles_documents_ref_domaines` FOREIGN KEY (`id_domaine`) REFERENCES `ref_domaines` (`id_domaine`),
  CONSTRAINT `fk_modeles_documents_ref_types_documents` FOREIGN KEY (`id_type_document`) REFERENCES `ref_types_documents` (`id_type_document`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `paiements`
--

DROP TABLE IF EXISTS `paiements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `paiements` (
  `id_paiement` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_paiements`),
  `code_paiement` varchar(12) GENERATED ALWAYS AS (concat('PAI',lpad(`id_paiement`,6,'0'))) STORED,
  `date_paiement` date DEFAULT NULL COMMENT 'Date d''encaissement — NULL tant que le paiement est en attente',
  `montant` decimal(10,2) NOT NULL,
  `statut_paiement` varchar(20) NOT NULL DEFAULT 'EN_ATTENTE' COMMENT 'EN_ATTENTE (séance créée, non encaissée), PAYE (encaissée) ou ANNULE (séance non due) — D-PAY-4',
  `mode_paiement` varchar(50) DEFAULT NULL,
  `reference_paiement` varchar(100) DEFAULT NULL,
  `commentaire` text DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_paiement`),
  UNIQUE KEY `uq_paiements_code_paiement` (`code_paiement`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `patients`
--

DROP TABLE IF EXISTS `patients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `patients` (
  `id_patient` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_patients`),
  `code_patient` varchar(12) GENERATED ALWAYS AS (concat('PA',lpad(`id_patient`,6,'0'))) STORED,
  `id_civilite` bigint(20) unsigned DEFAULT NULL,
  `nom` varchar(100) NOT NULL,
  `prenom` varchar(100) NOT NULL,
  `date_naissance` date DEFAULT NULL,
  `niss` varchar(20) DEFAULT NULL,
  `lateralite` varchar(30) DEFAULT NULL,
  `adresse_ligne1` varchar(255) DEFAULT NULL,
  `adresse_ligne2` varchar(255) DEFAULT NULL,
  `code_postal` varchar(20) DEFAULT NULL,
  `localite` varchar(100) DEFAULT NULL,
  `id_pays` bigint(20) unsigned DEFAULT NULL,
  `telephone` varchar(50) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `id_organisme_mutualite` bigint(20) unsigned DEFAULT NULL COMMENT 'Mutualite (ref_organismes de type MUTUALITE)',
  `numero_affiliation` varchar(30) DEFAULT NULL COMMENT 'Numero d affiliation aupres de la mutualite',
  `code_titulaire_1` char(3) DEFAULT NULL COMMENT 'CT1 (3 chiffres)',
  `code_titulaire_2` char(3) DEFAULT NULL COMMENT 'CT2 (3 chiffres)',
  `id_situation_familiale` bigint(20) unsigned DEFAULT NULL,
  `photo_fichier` varchar(255) DEFAULT NULL COMMENT 'Nom seul du fichier photo d''identité ; chemin reconstruit (D-Q13)',
  `alerte_rtf` longtext DEFAULT NULL COMMENT 'Alerte patient au format RTF (D-Q12)',
  `alerte_txt` longtext DEFAULT NULL COMMENT 'Alerte patient en texte brut pour la recherche (D-Q12)',
  `anamnese_rtf` longtext DEFAULT NULL COMMENT 'Anamnèse patient au format RTF (D-Q19)',
  `anamnese_txt` longtext DEFAULT NULL COMMENT 'Anamnèse patient en texte brut pour la recherche (D-Q19)',
  `is_actif` tinyint(1) NOT NULL DEFAULT 1,
  `date_activation` date DEFAULT NULL,
  `date_desactivation` date DEFAULT NULL,
  `id_prix_seance_defaut` bigint(20) unsigned DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_patient`),
  UNIQUE KEY `uq_patients_code_patient` (`code_patient`),
  UNIQUE KEY `uq_patients_niss` (`niss`),
  KEY `fk_patients_ref_situations_familiales` (`id_situation_familiale`),
  KEY `IDX_patients_id_prix_seance_defaut` (`id_prix_seance_defaut`) USING BTREE,
  KEY `IDX_patients_pays` (`id_pays`),
  KEY `IDX_patients_civilite` (`id_civilite`),
  KEY `IDX_patients_mutualite` (`id_organisme_mutualite`),
  CONSTRAINT `FK_patients_prix_seance_defaut` FOREIGN KEY (`id_prix_seance_defaut`) REFERENCES `ref_prix_seance` (`id_prix_seance`) ON DELETE SET NULL ON UPDATE NO ACTION,
  CONSTRAINT `fk_patients_civilite` FOREIGN KEY (`id_civilite`) REFERENCES `ref_civilites` (`id_civilite`) ON UPDATE CASCADE,
  CONSTRAINT `fk_patients_mutualite` FOREIGN KEY (`id_organisme_mutualite`) REFERENCES `ref_organismes` (`id_organisme`) ON UPDATE CASCADE,
  CONSTRAINT `fk_patients_pays` FOREIGN KEY (`id_pays`) REFERENCES `ref_pays` (`id_pays`) ON UPDATE CASCADE,
  CONSTRAINT `fk_patients_ref_situations_familiales` FOREIGN KEY (`id_situation_familiale`) REFERENCES `ref_situations_familiales` (`id_situation_familiale`),
  CONSTRAINT `chk_patients_ct1` CHECK (`code_titulaire_1` is null or `code_titulaire_1` regexp '^[0-9]{3}$'),
  CONSTRAINT `chk_patients_ct2` CHECK (`code_titulaire_2` is null or `code_titulaire_2` regexp '^[0-9]{3}$')
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_categories_quota`
--

DROP TABLE IF EXISTS `ref_categories_quota`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_categories_quota` (
  `id_categorie_quota` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_domaine` bigint(20) unsigned NOT NULL COMMENT 'Domaine (convention) auquel appartient la categorie',
  `code_categorie` varchar(30) NOT NULL,
  `libelle_categorie` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_categorie_quota`),
  UNIQUE KEY `uq_ref_categories_quota_code` (`id_domaine`,`code_categorie`),
  UNIQUE KEY `uq_ref_categories_quota_libelle` (`id_domaine`,`libelle_categorie`),
  UNIQUE KEY `uq_ref_categories_quota_id_domaine` (`id_categorie_quota`,`id_domaine`),
  CONSTRAINT `fk_ref_categories_quota_domaine` FOREIGN KEY (`id_domaine`) REFERENCES `ref_domaines` (`id_domaine`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci COMMENT='Categories de quota propres a chaque domaine (ex. Soutien, Traitement)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_civilites`
--

DROP TABLE IF EXISTS `ref_civilites`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_civilites` (
  `id_civilite` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_civilite` varchar(20) NOT NULL,
  `libelle_civilite` varchar(50) NOT NULL,
  `abreviation_civilite` varchar(10) DEFAULT NULL COMMENT 'Forme abregee pour les courriers (Mme, M., ...)',
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_civilite`),
  UNIQUE KEY `uq_ref_civilites_code` (`code_civilite`),
  UNIQUE KEY `uq_ref_civilites_libelle` (`libelle_civilite`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci COMMENT='Civilites des personnes (praticiens, patients, contacts, therapeutes, intervenants)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_domaines`
--

DROP TABLE IF EXISTS `ref_domaines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_domaines` (
  `id_domaine` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_domaine` varchar(10) NOT NULL,
  `libelle_domaine` varchar(100) NOT NULL,
  `id_organisme` bigint(20) unsigned DEFAULT NULL COMMENT 'Organisme de remboursement (NULL = aucun organisme)',
  `tarif_convention` tinyint(1) NOT NULL DEFAULT 0 COMMENT 'Tarif resolu via lia_prix_seance_domaines et non via le tarif patient du dossier — D-POR-1',
  `no_show_facturable` tinyint(1) NOT NULL DEFAULT 1 COMMENT 'No-show facture au patient (0 pour un circuit conventionne qui ne rembourse pas) — D-POR-2',
  `soumis_quota` tinyint(1) NOT NULL DEFAULT 0 COMMENT 'Domaine soumis a un quota periodique parametre dans ref_quotas — D-POR-3',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_domaine`),
  UNIQUE KEY `uq_ref_domaines_code` (`code_domaine`),
  UNIQUE KEY `uq_ref_domaines_libelle` (`libelle_domaine`),
  KEY `IDX_ref_domaines_organisme` (`id_organisme`),
  CONSTRAINT `fk_ref_domaines_organisme` FOREIGN KEY (`id_organisme`) REFERENCES `ref_organismes` (`id_organisme`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_liens_patient`
--

DROP TABLE IF EXISTS `ref_liens_patient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_liens_patient` (
  `id_lien_patient` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_lien_patient` varchar(50) NOT NULL,
  `libelle_lien_patient` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  PRIMARY KEY (`id_lien_patient`),
  UNIQUE KEY `uq_ref_liens_patient_code` (`code_lien_patient`),
  UNIQUE KEY `uq_ref_liens_patient_libelle` (`libelle_lien_patient`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_modes_paiement`
--

DROP TABLE IF EXISTS `ref_modes_paiement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_modes_paiement` (
  `id_mode_paiement` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_mode_paiement` varchar(30) NOT NULL,
  `libelle_mode_paiement` varchar(100) NOT NULL,
  `est_avoir` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = mode reserve a l utilisation d un avoir patient (non propose a la saisie manuelle)',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  PRIMARY KEY (`id_mode_paiement`),
  UNIQUE KEY `uq_ref_modes_paiement_code` (`code_mode_paiement`),
  UNIQUE KEY `uq_ref_modes_paiement_libelle` (`libelle_mode_paiement`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_natures_seance`
--

DROP TABLE IF EXISTS `ref_natures_seance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_natures_seance` (
  `id_nature_seance` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_nature_seance` varchar(30) NOT NULL,
  `libelle_nature_seance` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_nature_seance`),
  UNIQUE KEY `uq_ref_natures_seance_code` (`code_nature_seance`),
  UNIQUE KEY `uq_ref_natures_seance_libelle` (`libelle_nature_seance`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_organismes`
--

DROP TABLE IF EXISTS `ref_organismes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_organismes` (
  `id_organisme` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_organisme` varchar(30) NOT NULL,
  `libelle_organisme` varchar(100) NOT NULL,
  `type_organisme` varchar(20) NOT NULL DEFAULT 'REMBOURSEMENT' COMMENT 'REMBOURSEMENT (rattache aux domaines) ou MUTUALITE (rattache aux patients)',
  `adresse` varchar(250) DEFAULT NULL,
  `telephone` varchar(30) DEFAULT NULL,
  `telephone_2` varchar(30) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `email_2` varchar(150) DEFAULT NULL,
  `site_web` varchar(250) DEFAULT NULL,
  `url_plateforme` varchar(250) DEFAULT NULL COMMENT 'Plateforme d encodage / portail de l organisme',
  `procedure_encodage` text DEFAULT NULL COMMENT 'Procedure d encodage / de facturation (texte libre)',
  `id_periodicite` bigint(20) unsigned DEFAULT NULL,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT 'Ligne utilisee par l''application : code non modifiable, non desactivable',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_organisme`),
  UNIQUE KEY `uq_ref_organismes_code` (`code_organisme`),
  UNIQUE KEY `uq_ref_organismes_libelle` (`libelle_organisme`),
  KEY `IDX_ref_organismes_periodicite` (`id_periodicite`),
  KEY `IDX_ref_organismes_type` (`type_organisme`),
  CONSTRAINT `fk_ref_organismes_periodicite` FOREIGN KEY (`id_periodicite`) REFERENCES `ref_periodicites` (`id_periodicite`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `chk_ref_organismes_type` CHECK (`type_organisme` in ('REMBOURSEMENT','MUTUALITE'))
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Organismes : remboursement (ex. INAMI, rattaches aux domaines) et mutualites (rattachees aux patients)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_pays`
--

DROP TABLE IF EXISTS `ref_pays`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_pays` (
  `id_pays` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_pays` char(2) NOT NULL COMMENT 'Code ISO 3166-1 alpha-2 (cle des regles de validation)',
  `libelle_pays` varchar(100) NOT NULL,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_pays`),
  UNIQUE KEY `uq_ref_pays_code` (`code_pays`),
  UNIQUE KEY `uq_ref_pays_libelle` (`libelle_pays`),
  CONSTRAINT `chk_ref_pays_code` CHECK (`code_pays` regexp '^[A-Z]{2}$')
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci COMMENT='Pays (adresses) ; regles de validation indexees par code ISO dans le code';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_periodicites`
--

DROP TABLE IF EXISTS `ref_periodicites`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_periodicites` (
  `id_periodicite` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_periodicite` varchar(30) NOT NULL,
  `libelle_periodicite` varchar(100) NOT NULL,
  `nb_mois` int(11) DEFAULT NULL COMMENT 'Duree de la periode en mois (NULL = non periodique / a la demande)',
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_periodicite`),
  UNIQUE KEY `uq_ref_periodicites_code` (`code_periodicite`),
  UNIQUE KEY `uq_ref_periodicites_libelle` (`libelle_periodicite`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci COMMENT='Periodicites d encodage / facturation des organismes';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_prix_seance`
--

DROP TABLE IF EXISTS `ref_prix_seance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_prix_seance` (
  `id_prix_seance` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_prix_seance` varchar(30) NOT NULL,
  `libelle_prix_seance` varchar(100) NOT NULL,
  `montant` decimal(10,2) NOT NULL DEFAULT 0.00,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  PRIMARY KEY (`id_prix_seance`),
  UNIQUE KEY `uq_ref_prix_seance_code` (`code_prix_seance`),
  UNIQUE KEY `uq_ref_prix_seance_libelle` (`libelle_prix_seance`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_quotas`
--

DROP TABLE IF EXISTS `ref_quotas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_quotas` (
  `id_quota` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_domaine` bigint(20) unsigned NOT NULL COMMENT 'Domaine conventionné auquel s''applique la règle — D-INA-2',
  `seuil_alerte_total` int(11) NOT NULL DEFAULT 28 COMMENT 'Total à partir duquel l''utilisateur est averti de l''approche du plafond — D-INA-2',
  `duree_periode_mois` int(11) NOT NULL DEFAULT 12 COMMENT 'Duree de la periode glissante ouverte a la 1re seance consommante',
  `date_debut_validite` date NOT NULL COMMENT 'Premier jour d''application de la règle — D-INA-2',
  `date_fin_validite` date DEFAULT NULL COMMENT 'Dernier jour d''application, NULL si la règle est toujours en vigueur — D-INA-2',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_quota`),
  UNIQUE KEY `uq_ref_quotas_domaine_debut` (`id_domaine`,`date_debut_validite`),
  KEY `IDX_ref_quotas_validite` (`id_domaine`,`date_debut_validite`,`date_fin_validite`),
  CONSTRAINT `fk_ref_quotas_domaine` FOREIGN KEY (`id_domaine`) REFERENCES `ref_domaines` (`id_domaine`) ON UPDATE CASCADE,
  CONSTRAINT `chk_ref_quotas_periode` CHECK (`date_fin_validite` is null or `date_fin_validite` >= `date_debut_validite`),
  CONSTRAINT `chk_ref_quotas_duree` CHECK (`duree_periode_mois` > 0)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci COMMENT='Regles de quota periodique de prestations remboursees, par domaine (convention)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_quotas_plafonds`
--

DROP TABLE IF EXISTS `ref_quotas_plafonds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_quotas_plafonds` (
  `id_quota_plafond` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `id_quota` bigint(20) unsigned NOT NULL COMMENT 'Regle datee (ref_quotas)',
  `id_categorie_quota` bigint(20) unsigned NOT NULL COMMENT 'Categorie plafonnee',
  `plafond` int(11) NOT NULL COMMENT 'Nombre de seances remboursees par periode',
  PRIMARY KEY (`id_quota_plafond`),
  UNIQUE KEY `uq_ref_quotas_plafonds` (`id_quota`,`id_categorie_quota`),
  KEY `IDX_ref_quotas_plafonds_categorie` (`id_categorie_quota`),
  CONSTRAINT `fk_ref_quotas_plafonds_categorie` FOREIGN KEY (`id_categorie_quota`) REFERENCES `ref_categories_quota` (`id_categorie_quota`) ON UPDATE CASCADE,
  CONSTRAINT `fk_ref_quotas_plafonds_quota` FOREIGN KEY (`id_quota`) REFERENCES `ref_quotas` (`id_quota`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `chk_ref_quotas_plafonds_positif` CHECK (`plafond` >= 0)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci COMMENT='Plafond par categorie pour une regle de quota datee';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_role_legal`
--

DROP TABLE IF EXISTS `ref_role_legal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_role_legal` (
  `id_role_legal` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_role_legal` varchar(30) NOT NULL,
  `libelle_role_legal` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  PRIMARY KEY (`id_role_legal`),
  UNIQUE KEY `uq_ref_role_legal_code` (`code_role_legal`),
  UNIQUE KEY `uq_ref_role_legal_libelle` (`libelle_role_legal`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_roles_intervenant`
--

DROP TABLE IF EXISTS `ref_roles_intervenant`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_roles_intervenant` (
  `id_role_intervenant` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_role_intervenant` varchar(30) NOT NULL,
  `libelle_role_intervenant` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_role_intervenant`),
  UNIQUE KEY `uq_ref_roles_intervenant_code` (`code_role_intervenant`),
  UNIQUE KEY `uq_ref_roles_intervenant_libelle` (`libelle_role_intervenant`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_situations_familiales`
--

DROP TABLE IF EXISTS `ref_situations_familiales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_situations_familiales` (
  `id_situation_familiale` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_situation_familiale` varchar(50) NOT NULL,
  `libelle_situation_familiale` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_situation_familiale`),
  UNIQUE KEY `uq_ref_situations_familiales_code` (`code_situation_familiale`),
  UNIQUE KEY `uq_ref_situations_familiales_libelle` (`libelle_situation_familiale`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_situations_juridiques`
--

DROP TABLE IF EXISTS `ref_situations_juridiques`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_situations_juridiques` (
  `id_situation_juridique` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_situation_juridique` varchar(50) NOT NULL,
  `libelle_situation_juridique` varchar(150) NOT NULL,
  `porteur_bce` varchar(20) NOT NULL DEFAULT 'CABINET' COMMENT 'CABINET ou PRATICIEN : entite qui porte le numero BCE',
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_situation_juridique`),
  UNIQUE KEY `uq_ref_situations_juridiques_code` (`code_situation_juridique`),
  UNIQUE KEY `uq_ref_situations_juridiques_libelle` (`libelle_situation_juridique`),
  CONSTRAINT `chk_ref_situations_juridiques_porteur` CHECK (`porteur_bce` in ('CABINET','PRATICIEN'))
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_statuts_dossier`
--

DROP TABLE IF EXISTS `ref_statuts_dossier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_statuts_dossier` (
  `id_statut_dossier` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_statut_dossier` varchar(30) NOT NULL,
  `libelle_statut_dossier` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  PRIMARY KEY (`id_statut_dossier`),
  UNIQUE KEY `uq_ref_statuts_dossier_code` (`code_statut_dossier`),
  UNIQUE KEY `uq_ref_statuts_dossier_libelle` (`libelle_statut_dossier`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_statuts_seance`
--

DROP TABLE IF EXISTS `ref_statuts_seance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_statuts_seance` (
  `id_statut_seance` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_statut_seance` varchar(30) NOT NULL,
  `libelle_statut_seance` varchar(100) NOT NULL,
  `est_du` tinyint(1) NOT NULL DEFAULT 1 COMMENT 'Séance due (facturable) pour ce statut — No-show = dû, annulation à temps = non dû — D-TAR-3',
  `consomme_quota` tinyint(1) NOT NULL DEFAULT 0 COMMENT 'Une seance dans ce statut consomme le quota du domaine',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  PRIMARY KEY (`id_statut_seance`),
  UNIQUE KEY `uq_ref_statuts_seance_code` (`code_statut_seance`),
  UNIQUE KEY `uq_ref_statuts_seance_libelle` (`libelle_statut_seance`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_types_documents`
--

DROP TABLE IF EXISTS `ref_types_documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_types_documents` (
  `id_type_document` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_type_document` varchar(30) NOT NULL,
  `libelle_type_document` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT '1 = ligne utilisee par l application : code et activation verrouilles',
  PRIMARY KEY (`id_type_document`),
  UNIQUE KEY `uq_ref_types_documents_code` (`code_type_document`),
  UNIQUE KEY `uq_ref_types_documents_libelle` (`libelle_type_document`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_types_identifiant_pro`
--

DROP TABLE IF EXISTS `ref_types_identifiant_pro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_types_identifiant_pro` (
  `id_type_identifiant` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_type_identifiant` varchar(30) NOT NULL,
  `libelle_type_identifiant` varchar(100) NOT NULL,
  `masque_saisie` varchar(50) DEFAULT NULL COMMENT 'Masque indicatif de saisie (ex. 0-00000-00-000)',
  `regex_validation` varchar(200) DEFAULT NULL COMMENT 'Expression de contrôle facultative',
  `cible` varchar(20) NOT NULL DEFAULT 'PRATICIEN' COMMENT 'PRATICIEN ou CABINET',
  `est_systeme` tinyint(1) NOT NULL DEFAULT 0 COMMENT 'Ligne utilisee par l''application : code non modifiable, non desactivable',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_type_identifiant`),
  UNIQUE KEY `uq_ref_types_identifiant_pro_code` (`code_type_identifiant`),
  CONSTRAINT `chk_ref_types_identifiant_pro_cible` CHECK (`cible` in ('PRATICIEN','CABINET'))
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Types d''identifiant professionnel (INAMI, agrement, BCE, TVA...)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_types_rendez_vous`
--

DROP TABLE IF EXISTS `ref_types_rendez_vous`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_types_rendez_vous` (
  `id_type_rendez_vous` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `code_type_rendez_vous` varchar(30) NOT NULL,
  `libelle_type_rendez_vous` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_type_rendez_vous`),
  UNIQUE KEY `uq_ref_types_rdv_code` (`code_type_rendez_vous`),
  UNIQUE KEY `uq_ref_types_rdv_libelle` (`libelle_type_rendez_vous`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rendez_vous`
--

DROP TABLE IF EXISTS `rendez_vous`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `rendez_vous` (
  `id_rendez_vous` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_rendez_vous`),
  `code_rendez_vous` varchar(12) GENERATED ALWAYS AS (concat('RDV',lpad(`id_rendez_vous`,6,'0'))) STORED,
  `id_patient` bigint(20) unsigned DEFAULT NULL,
  `id_dossier` bigint(20) unsigned DEFAULT NULL,
  `id_seance` bigint(20) unsigned DEFAULT NULL,
  `id_type_rendez_vous` bigint(20) unsigned NOT NULL,
  `titre` varchar(255) NOT NULL,
  `description` text DEFAULT NULL,
  `date_heure_debut` datetime NOT NULL,
  `date_heure_fin` datetime NOT NULL,
  `est_journee_entiere` tinyint(1) NOT NULL DEFAULT 0,
  `google_calendar_id` varchar(255) DEFAULT NULL,
  `google_event_id` varchar(255) DEFAULT NULL,
  `google_event_etag` varchar(255) DEFAULT NULL,
  `google_event_url` text DEFAULT NULL,
  `statut_sync_google` varchar(30) NOT NULL DEFAULT 'NON_LIE',
  `date_sync_google` datetime DEFAULT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_rendez_vous`),
  UNIQUE KEY `uq_rdv_code` (`code_rendez_vous`),
  KEY `fk_rdv_patient` (`id_patient`),
  KEY `fk_rdv_dossier` (`id_dossier`),
  KEY `fk_rdv_seance` (`id_seance`),
  KEY `fk_rdv_type` (`id_type_rendez_vous`),
  CONSTRAINT `fk_rdv_dossier` FOREIGN KEY (`id_dossier`) REFERENCES `dossiers` (`id_dossier`),
  CONSTRAINT `fk_rdv_patient` FOREIGN KEY (`id_patient`) REFERENCES `patients` (`id_patient`),
  CONSTRAINT `fk_rdv_seance` FOREIGN KEY (`id_seance`) REFERENCES `seances` (`id_seance`),
  CONSTRAINT `fk_rdv_type` FOREIGN KEY (`id_type_rendez_vous`) REFERENCES `ref_types_rendez_vous` (`id_type_rendez_vous`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `seances`
--

DROP TABLE IF EXISTS `seances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `seances` (
  `id_seance` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_seances`),
  `code_seance` varchar(12) GENERATED ALWAYS AS (concat('SE',lpad(`id_seance`,6,'0'))) STORED,
  `id_dossier` bigint(20) unsigned NOT NULL,
  `id_prix_seance` bigint(20) unsigned NOT NULL COMMENT 'Tarif appliqué à la séance (ref_prix_seance) — D-SEA-1',
  `id_nature_seance` bigint(20) unsigned NOT NULL COMMENT 'Nature clinique de la séance (ref_natures_seance) — D-SEA-1',
  `id_statut_seance` bigint(20) unsigned NOT NULL,
  `id_seance_origine` bigint(20) unsigned DEFAULT NULL COMMENT 'Séance dont celle-ci est issue par report (NULL si séance ordinaire) — D-SEA-2',
  `date_seance` date NOT NULL,
  `heure_debut` time DEFAULT NULL,
  `heure_fin` time DEFAULT NULL,
  `duree_minutes` smallint(5) unsigned DEFAULT NULL,
  `tarif_seance` decimal(10,2) NOT NULL DEFAULT 0.00,
  `notes_seance_rtf` longtext DEFAULT NULL,
  `notes_seance_txt` longtext DEFAULT NULL,
  `evolution_rtf` longtext DEFAULT NULL,
  `evolution_txt` longtext DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_seance`),
  UNIQUE KEY `uq_seances_code_seance` (`code_seance`),
  KEY `fk_seances_dossiers` (`id_dossier`),
  KEY `fk_seances_ref_statuts_seance` (`id_statut_seance`),
  KEY `fk_seances_ref_prix_seance` (`id_prix_seance`),
  KEY `fk_seances_ref_natures_seance` (`id_nature_seance`),
  KEY `fk_seances_seance_origine` (`id_seance_origine`),
  CONSTRAINT `fk_seances_dossiers` FOREIGN KEY (`id_dossier`) REFERENCES `dossiers` (`id_dossier`),
  CONSTRAINT `fk_seances_ref_natures_seance` FOREIGN KEY (`id_nature_seance`) REFERENCES `ref_natures_seance` (`id_nature_seance`),
  CONSTRAINT `fk_seances_ref_prix_seance` FOREIGN KEY (`id_prix_seance`) REFERENCES `ref_prix_seance` (`id_prix_seance`),
  CONSTRAINT `fk_seances_ref_statuts_seance` FOREIGN KEY (`id_statut_seance`) REFERENCES `ref_statuts_seance` (`id_statut_seance`),
  CONSTRAINT `fk_seances_seance_origine` FOREIGN KEY (`id_seance_origine`) REFERENCES `seances` (`id_seance`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sec_utilisateurs`
--

DROP TABLE IF EXISTS `sec_utilisateurs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `sec_utilisateurs` (
  `id_utilisateur` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_sec_utilisateurs`),
  `code_utilisateur` varchar(20) GENERATED ALWAYS AS (concat('USR',lpad(`id_utilisateur`,6,'0'))) STORED,
  `login_utilisateur` varchar(100) NOT NULL,
  `nom_affichage` varchar(150) NOT NULL,
  `photo_fichier` varchar(255) DEFAULT NULL COMMENT 'Nom seul du fichier photo ; chemin reconstruit (Utilisateurs/Utilisateur_{id}) ; compte lie a un praticien : photo du praticien',
  `password_hash` varchar(255) NOT NULL,
  `password_salt` varchar(255) NOT NULL,
  `role_utilisateur` varchar(20) NOT NULL,
  `role_max_elevation` varchar(20) NOT NULL DEFAULT 'User',
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `must_change_password` tinyint(1) NOT NULL DEFAULT 1,
  `nb_echecs_login` int(11) NOT NULL DEFAULT 0,
  `compte_verrouille` tinyint(1) NOT NULL DEFAULT 0,
  `date_verrouillage` datetime DEFAULT NULL,
  `dernier_login` datetime DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_utilisateur`),
  UNIQUE KEY `uq_sec_utilisateurs_login` (`login_utilisateur`),
  UNIQUE KEY `uq_sec_utilisateurs_code` (`code_utilisateur`),
  CONSTRAINT `chk_sec_utilisateurs_role` CHECK (`role_utilisateur` in ('User','SuperUser','Admin')),
  CONSTRAINT `chk_sec_utilisateurs_actif` CHECK (`actif` in (0,1)),
  CONSTRAINT `chk_sec_utilisateurs_must_change_password` CHECK (`must_change_password` in (0,1)),
  CONSTRAINT `chk_sec_utilisateurs_nb_echecs` CHECK (`nb_echecs_login` >= 0),
  CONSTRAINT `chk_sec_utilisateurs_role_max_elevation` CHECK (`role_max_elevation` in ('User','SuperUser','Admin'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tec_meta_schema`
--

DROP TABLE IF EXISTS `tec_meta_schema`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `tec_meta_schema` (
  `id_meta_schema` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_tec_meta_schema`),
  `version_schema` varchar(20) NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `date_application` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id_meta_schema`),
  UNIQUE KEY `uq_tec_meta_schema_version` (`version_schema`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tec_parametres`
--

DROP TABLE IF EXISTS `tec_parametres`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `tec_parametres` (
  `id_parametre` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_tec_parametres`),
  `code_parametre` varchar(12) GENERATED ALWAYS AS (concat('PAR',lpad(`id_parametre`,6,'0'))) STORED,
  `cle_parametre` varchar(100) NOT NULL,
  `libelle_parametre` varchar(150) NOT NULL,
  `groupe_parametre` varchar(50) NOT NULL DEFAULT 'GENERAL',
  `type_valeur` varchar(30) NOT NULL DEFAULT 'STRING',
  `valeur_parametre` text DEFAULT NULL,
  `valeur_min` decimal(18,4) DEFAULT NULL COMMENT 'Borne basse (INT / DECIMAL) ; NULL = sans borne',
  `valeur_max` decimal(18,4) DEFAULT NULL COMMENT 'Borne haute (INT / DECIMAL) ; NULL = sans borne',
  `description_parametre` text DEFAULT NULL,
  `modifiable_utilisateur` tinyint(1) NOT NULL DEFAULT 1,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_parametre`),
  UNIQUE KEY `uq_tec_parametres_cle_parametre` (`cle_parametre`),
  UNIQUE KEY `uq_tec_parametres_code_parametre` (`code_parametre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tec_restaurations`
--

DROP TABLE IF EXISTS `tec_restaurations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `tec_restaurations` (
  `id_restauration` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `date_debut` datetime NOT NULL DEFAULT current_timestamp(),
  `date_fin` datetime DEFAULT NULL,
  `statut_restauration` varchar(20) NOT NULL COMMENT 'REUSSIE, ECHEC (StatutRestauration)',
  `nom_archive` varchar(255) DEFAULT NULL COMMENT 'Archive restauree (Althea_AAAAMMJJ_HHMMSS.zip)',
  `dossier_archive` varchar(500) DEFAULT NULL COMMENT 'Dossier de l archive au moment de la restauration',
  `date_archive` datetime DEFAULT NULL COMMENT 'Date de la sauvegarde restauree (manifeste)',
  `version_schema_archive` varchar(20) DEFAULT NULL,
  `machine_archive` varchar(100) DEFAULT NULL COMMENT 'Poste qui avait fait la sauvegarde',
  `nb_fichiers` int(11) unsigned DEFAULT NULL COMMENT 'Fichiers du dossier de donnees restaures',
  `archive_securite` varchar(255) DEFAULT NULL COMMENT 'Sauvegarde de securite faite juste avant',
  `dossier_donnees_precedent` varchar(500) DEFAULT NULL COMMENT 'Ancien dossier de donnees, renomme et conserve',
  `login_utilisateur` varchar(100) DEFAULT NULL COMMENT 'Trace : compte qui a restaure',
  `machine` varchar(100) DEFAULT NULL,
  `version_application` varchar(30) DEFAULT NULL,
  `message_erreur` text DEFAULT NULL COMMENT 'Cause de l echec, sans donnee sensible',
  PRIMARY KEY (`id_restauration`),
  KEY `IDX_tec_restaurations_date` (`date_debut`),
  CONSTRAINT `chk_tec_restaurations_statut` CHECK (`statut_restauration` in ('REUSSIE','ECHEC'))
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci COMMENT='Journal des restaurations (lot 30)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tec_sauvegardes`
--

DROP TABLE IF EXISTS `tec_sauvegardes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `tec_sauvegardes` (
  `id_sauvegarde` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `date_debut` datetime NOT NULL DEFAULT current_timestamp(),
  `date_fin` datetime DEFAULT NULL,
  `statut_sauvegarde` varchar(20) NOT NULL DEFAULT 'EN_COURS' COMMENT 'EN_COURS, REUSSIE, ECHEC (StatutSauvegarde)',
  `nom_fichier` varchar(255) DEFAULT NULL COMMENT 'Nom de l archive (Althea_AAAAMMJJ_HHMMSS.zip)',
  `dossier_sauvegarde` varchar(500) DEFAULT NULL COMMENT 'Dossier au moment de la sauvegarde',
  `taille_octets` bigint(20) unsigned DEFAULT NULL,
  `nb_fichiers` int(11) unsigned DEFAULT NULL COMMENT 'Fichiers du dossier de donnees inclus',
  `version_schema` varchar(20) DEFAULT NULL COMMENT 'Derniere migration appliquee au moment de la sauvegarde',
  `version_application` varchar(30) DEFAULT NULL,
  `id_utilisateur` bigint(20) unsigned DEFAULT NULL,
  `login_utilisateur` varchar(100) DEFAULT NULL COMMENT 'Trace : login au moment de la sauvegarde',
  `machine` varchar(100) DEFAULT NULL,
  `message_erreur` text DEFAULT NULL COMMENT 'Cause de l echec, sans donnee sensible',
  PRIMARY KEY (`id_sauvegarde`),
  KEY `IDX_tec_sauvegardes_statut_date` (`statut_sauvegarde`,`date_debut`),
  KEY `IDX_tec_sauvegardes_utilisateur` (`id_utilisateur`),
  CONSTRAINT `fk_tec_sauvegardes_utilisateur` FOREIGN KEY (`id_utilisateur`) REFERENCES `sec_utilisateurs` (`id_utilisateur`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `chk_tec_sauvegardes_statut` CHECK (`statut_sauvegarde` in ('EN_COURS','REUSSIE','ECHEC'))
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci COMMENT='Journal des sauvegardes (lot 30)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tec_sync_evenements_google`
--

DROP TABLE IF EXISTS `tec_sync_evenements_google`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `tec_sync_evenements_google` (
  `id_sync_evenement_google` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_tec_sync_evenements_google`),
  `code_sync_evenement_google` varchar(12) GENERATED ALWAYS AS (concat('SEG',lpad(`id_sync_evenement_google`,6,'0'))) STORED,
  `id_rendez_vous` bigint(20) unsigned DEFAULT NULL,
  `google_calendar_id` varchar(255) DEFAULT NULL,
  `google_event_id` varchar(255) DEFAULT NULL,
  `operation_sync` varchar(20) NOT NULL,
  `statut_sync` varchar(30) NOT NULL,
  `message_sync` text DEFAULT NULL,
  `date_sync` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id_sync_evenement_google`),
  UNIQUE KEY `uq_tec_sync_evenements_google_code` (`code_sync_evenement_google`),
  KEY `fk_tec_sync_evenements_google_rendez_vous` (`id_rendez_vous`),
  CONSTRAINT `fk_tec_sync_evenements_google_rendez_vous` FOREIGN KEY (`id_rendez_vous`) REFERENCES `rendez_vous` (`id_rendez_vous`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `therapeutes`
--

DROP TABLE IF EXISTS `therapeutes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `therapeutes` (
  `id_therapeute` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_therapeutes`),
  `code_therapeute` varchar(12) GENERATED ALWAYS AS (concat('TH',lpad(`id_therapeute`,6,'0'))) STORED,
  `id_civilite` bigint(20) unsigned DEFAULT NULL,
  `nom` varchar(100) NOT NULL,
  `prenom` varchar(100) DEFAULT NULL,
  `specialite` varchar(100) DEFAULT NULL,
  `telephone` varchar(50) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `adresse_ligne1` varchar(255) DEFAULT NULL,
  `adresse_ligne2` varchar(255) DEFAULT NULL,
  `code_postal` varchar(20) DEFAULT NULL,
  `localite` varchar(100) DEFAULT NULL,
  `id_pays` bigint(20) unsigned DEFAULT NULL,
  `commentaire_txt` longtext DEFAULT NULL,
  `commentaire_rtf` longtext DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `Actif` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_therapeute`),
  UNIQUE KEY `uq_therapeutes_code_therapeute` (`code_therapeute`),
  KEY `IDX_therapeutes_pays` (`id_pays`),
  KEY `IDX_therapeutes_civilite` (`id_civilite`),
  CONSTRAINT `fk_therapeutes_civilite` FOREIGN KEY (`id_civilite`) REFERENCES `ref_civilites` (`id_civilite`) ON UPDATE CASCADE,
  CONSTRAINT `fk_therapeutes_pays` FOREIGN KEY (`id_pays`) REFERENCES `ref_pays` (`id_pays`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*M!100616 SET NOTE_VERBOSITY=@OLD_NOTE_VERBOSITY */;

-- Dump completed on 2026-10-15  9:46:15
