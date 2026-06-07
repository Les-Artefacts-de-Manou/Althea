/*M!999999\- enable the sandbox mode */ 
-- MariaDB dump 10.19-12.3.2-MariaDB, for Win64 (AMD64)
--
-- Host: localhost    Database: althea
-- ------------------------------------------------------
-- Server version	12.3.2-MariaDB

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
DO SETVAL(`seq_autres_suivis_patient`, 1, 0);

--
-- Sequence structure for `seq_documents`
--

DROP SEQUENCE IF EXISTS `seq_documents`;
CREATE SEQUENCE `seq_documents` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_documents`, 1, 0);

--
-- Sequence structure for `seq_dossiers`
--

DROP SEQUENCE IF EXISTS `seq_dossiers`;
CREATE SEQUENCE `seq_dossiers` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_dossiers`, 1, 0);

--
-- Sequence structure for `seq_famille_contacts`
--

DROP SEQUENCE IF EXISTS `seq_famille_contacts`;
CREATE SEQUENCE `seq_famille_contacts` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_famille_contacts`, 1, 0);

--
-- Sequence structure for `seq_lia_paiements_seances`
--

DROP SEQUENCE IF EXISTS `seq_lia_paiements_seances`;
CREATE SEQUENCE `seq_lia_paiements_seances` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_lia_paiements_seances`, 1, 0);

--
-- Sequence structure for `seq_medecins`
--

DROP SEQUENCE IF EXISTS `seq_medecins`;
CREATE SEQUENCE `seq_medecins` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_medecins`, 1, 0);

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
DO SETVAL(`seq_paiements`, 1, 0);

--
-- Sequence structure for `seq_patients`
--

DROP SEQUENCE IF EXISTS `seq_patients`;
CREATE SEQUENCE `seq_patients` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_patients`, 2, 0);

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
DO SETVAL(`seq_seances`, 1, 0);

--
-- Sequence structure for `seq_sec_utilisateurs`
--

DROP SEQUENCE IF EXISTS `seq_sec_utilisateurs`;
CREATE SEQUENCE `seq_sec_utilisateurs` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_sec_utilisateurs`, 16, 0);

--
-- Sequence structure for `seq_tec_meta_schema`
--

DROP SEQUENCE IF EXISTS `seq_tec_meta_schema`;
CREATE SEQUENCE `seq_tec_meta_schema` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_tec_meta_schema`, 2, 0);

--
-- Sequence structure for `seq_tec_parametres`
--

DROP SEQUENCE IF EXISTS `seq_tec_parametres`;
CREATE SEQUENCE `seq_tec_parametres` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_tec_parametres`, 26, 0);

--
-- Sequence structure for `seq_tec_sync_evenements_google`
--

DROP SEQUENCE IF EXISTS `seq_tec_sync_evenements_google`;
CREATE SEQUENCE `seq_tec_sync_evenements_google` start with 1 minvalue 1 maxvalue 9223372036854775806 increment by 1 cache 1 nocycle ENGINE=InnoDB;
DO SETVAL(`seq_tec_sync_evenements_google`, 1, 0);

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
  `nom_professionnel` varchar(150) DEFAULT NULL,
  `specialite` varchar(100) DEFAULT NULL,
  `lieu` varchar(150) DEFAULT NULL,
  `date_debut` date DEFAULT NULL,
  `date_fin` date DEFAULT NULL,
  `commentaire` text DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_autre_suivi_patient`),
  UNIQUE KEY `uq_autres_suivis_patient_code` (`code_autre_suivi_patient`),
  KEY `fk_autres_suivis_patient_patients` (`id_patient`),
  CONSTRAINT `fk_autres_suivis_patient_patients` FOREIGN KEY (`id_patient`) REFERENCES `patients` (`id_patient`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
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
  `id_dossier` bigint(20) unsigned NOT NULL,
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
  CONSTRAINT `fk_documents_dossiers` FOREIGN KEY (`id_dossier`) REFERENCES `dossiers` (`id_dossier`),
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
  `id_volet` bigint(20) unsigned NOT NULL,
  `id_statut_dossier` bigint(20) unsigned NOT NULL,
  `date_ouverture` date NOT NULL,
  `date_cloture` date DEFAULT NULL,
  `prescripteur` varchar(150) DEFAULT NULL,
  `modalites_paiement` varchar(150) DEFAULT NULL,
  `id_medecin_traitant` bigint(20) unsigned DEFAULT NULL,
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
  KEY `fk_dossiers_ref_volets` (`id_volet`),
  KEY `fk_dossiers_ref_statuts_dossier` (`id_statut_dossier`),
  KEY `fk_dossiers_medecins` (`id_medecin_traitant`),
  CONSTRAINT `fk_dossiers_medecins` FOREIGN KEY (`id_medecin_traitant`) REFERENCES `medecins` (`id_medecin`),
  CONSTRAINT `fk_dossiers_patients` FOREIGN KEY (`id_patient`) REFERENCES `patients` (`id_patient`),
  CONSTRAINT `fk_dossiers_ref_statuts_dossier` FOREIGN KEY (`id_statut_dossier`) REFERENCES `ref_statuts_dossier` (`id_statut_dossier`),
  CONSTRAINT `fk_dossiers_ref_volets` FOREIGN KEY (`id_volet`) REFERENCES `ref_volets` (`id_volet`)
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
  `nom` varchar(100) NOT NULL,
  `prenom` varchar(100) NOT NULL,
  `date_naissance` date DEFAULT NULL,
  `telephone` varchar(50) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `adresse_ligne1` varchar(255) DEFAULT NULL,
  `adresse_ligne2` varchar(255) DEFAULT NULL,
  `code_postal` varchar(20) DEFAULT NULL,
  `localite` varchar(100) DEFAULT NULL,
  `pays` varchar(100) DEFAULT NULL,
  `autorite_parentale` tinyint(1) NOT NULL DEFAULT 0,
  `representant_legal` tinyint(1) NOT NULL DEFAULT 0,
  `personne_autorisee` tinyint(1) NOT NULL DEFAULT 0,
  `contact_urgence` tinyint(1) NOT NULL DEFAULT 0,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  `commentaire` text DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_famille_contact`),
  UNIQUE KEY `uq_famille_contacts_code_famille_contact` (`code_famille_contact`),
  KEY `fk_famille_contacts_patients` (`id_patient`),
  KEY `fk_famille_contacts_ref_liens_patient` (`id_lien_patient`),
  CONSTRAINT `fk_famille_contacts_patients` FOREIGN KEY (`id_patient`) REFERENCES `patients` (`id_patient`),
  CONSTRAINT `fk_famille_contacts_ref_liens_patient` FOREIGN KEY (`id_lien_patient`) REFERENCES `ref_liens_patient` (`id_lien_patient`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
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
-- Table structure for table `medecins`
--

DROP TABLE IF EXISTS `medecins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `medecins` (
  `id_medecin` bigint(20) unsigned NOT NULL DEFAULT nextval(`althea`.`seq_medecins`),
  `code_medecin` varchar(12) GENERATED ALWAYS AS (concat('ME',lpad(`id_medecin`,6,'0'))) STORED,
  `nom` varchar(100) NOT NULL,
  `prenom` varchar(100) DEFAULT NULL,
  `specialite` varchar(100) DEFAULT NULL,
  `telephone` varchar(50) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `adresse_ligne1` varchar(255) DEFAULT NULL,
  `adresse_ligne2` varchar(255) DEFAULT NULL,
  `code_postal` varchar(20) DEFAULT NULL,
  `localite` varchar(100) DEFAULT NULL,
  `pays` varchar(100) DEFAULT NULL,
  `commentaire` text DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_medecin`),
  UNIQUE KEY `uq_medecins_code_medecin` (`code_medecin`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
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
  `id_volet` bigint(20) unsigned DEFAULT NULL,
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
  KEY `fk_modeles_documents_ref_volets` (`id_volet`),
  CONSTRAINT `fk_modeles_documents_ref_types_documents` FOREIGN KEY (`id_type_document`) REFERENCES `ref_types_documents` (`id_type_document`),
  CONSTRAINT `fk_modeles_documents_ref_volets` FOREIGN KEY (`id_volet`) REFERENCES `ref_volets` (`id_volet`)
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
  `date_paiement` date NOT NULL,
  `montant` decimal(10,2) NOT NULL,
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
  `nom` varchar(100) NOT NULL,
  `prenom` varchar(100) NOT NULL,
  `date_naissance` date DEFAULT NULL,
  `niss` varchar(20) DEFAULT NULL,
  `lateralite` varchar(30) DEFAULT NULL,
  `adresse_ligne1` varchar(255) DEFAULT NULL,
  `adresse_ligne2` varchar(255) DEFAULT NULL,
  `code_postal` varchar(20) DEFAULT NULL,
  `localite` varchar(100) DEFAULT NULL,
  `pays` varchar(100) DEFAULT NULL,
  `telephone` varchar(50) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `mutualite` varchar(100) DEFAULT NULL,
  `id_situation_familiale` bigint(20) unsigned DEFAULT NULL,
  `alerte` text DEFAULT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_modification` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_patient`),
  UNIQUE KEY `uq_patients_code_patient` (`code_patient`),
  UNIQUE KEY `uq_patients_niss` (`niss`),
  KEY `fk_patients_ref_situations_familiales` (`id_situation_familiale`),
  CONSTRAINT `fk_patients_ref_situations_familiales` FOREIGN KEY (`id_situation_familiale`) REFERENCES `ref_situations_familiales` (`id_situation_familiale`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_liens_patient`
--

DROP TABLE IF EXISTS `ref_liens_patient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_liens_patient` (
  `id_lien_patient` bigint(20) unsigned NOT NULL,
  `code_lien_patient` varchar(50) NOT NULL,
  `libelle_lien_patient` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_lien_patient`),
  UNIQUE KEY `uq_ref_liens_patient_code` (`code_lien_patient`),
  UNIQUE KEY `uq_ref_liens_patient_libelle` (`libelle_lien_patient`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_situations_familiales`
--

DROP TABLE IF EXISTS `ref_situations_familiales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_situations_familiales` (
  `id_situation_familiale` bigint(20) unsigned NOT NULL,
  `code_situation_familiale` varchar(50) NOT NULL,
  `libelle_situation_familiale` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_situation_familiale`),
  UNIQUE KEY `uq_ref_situations_familiales_code` (`code_situation_familiale`),
  UNIQUE KEY `uq_ref_situations_familiales_libelle` (`libelle_situation_familiale`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_statuts_dossier`
--

DROP TABLE IF EXISTS `ref_statuts_dossier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_statuts_dossier` (
  `id_statut_dossier` bigint(20) unsigned NOT NULL,
  `code_statut_dossier` varchar(30) NOT NULL,
  `libelle_statut_dossier` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_statut_dossier`),
  UNIQUE KEY `uq_ref_statuts_dossier_code` (`code_statut_dossier`),
  UNIQUE KEY `uq_ref_statuts_dossier_libelle` (`libelle_statut_dossier`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_statuts_seance`
--

DROP TABLE IF EXISTS `ref_statuts_seance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_statuts_seance` (
  `id_statut_seance` bigint(20) unsigned NOT NULL,
  `code_statut_seance` varchar(30) NOT NULL,
  `libelle_statut_seance` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_statut_seance`),
  UNIQUE KEY `uq_ref_statuts_seance_code` (`code_statut_seance`),
  UNIQUE KEY `uq_ref_statuts_seance_libelle` (`libelle_statut_seance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_types_documents`
--

DROP TABLE IF EXISTS `ref_types_documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_types_documents` (
  `id_type_document` bigint(20) unsigned NOT NULL,
  `code_type_document` varchar(30) NOT NULL,
  `libelle_type_document` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_type_document`),
  UNIQUE KEY `uq_ref_types_documents_code` (`code_type_document`),
  UNIQUE KEY `uq_ref_types_documents_libelle` (`libelle_type_document`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_types_rendez_vous`
--

DROP TABLE IF EXISTS `ref_types_rendez_vous`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_types_rendez_vous` (
  `id_type_rendez_vous` bigint(20) unsigned NOT NULL,
  `code_type_rendez_vous` varchar(30) NOT NULL,
  `libelle_type_rendez_vous` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_type_rendez_vous`),
  UNIQUE KEY `uq_ref_types_rdv_code` (`code_type_rendez_vous`),
  UNIQUE KEY `uq_ref_types_rdv_libelle` (`libelle_type_rendez_vous`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_types_seance`
--

DROP TABLE IF EXISTS `ref_types_seance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_types_seance` (
  `id_type_seance` bigint(20) unsigned NOT NULL,
  `code_type_seance` varchar(30) NOT NULL,
  `libelle_type_seance` varchar(100) NOT NULL,
  `tarif_defaut` decimal(10,2) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_type_seance`),
  UNIQUE KEY `uq_ref_types_seance_code` (`code_type_seance`),
  UNIQUE KEY `uq_ref_types_seance_libelle` (`libelle_type_seance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ref_volets`
--

DROP TABLE IF EXISTS `ref_volets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `ref_volets` (
  `id_volet` bigint(20) unsigned NOT NULL,
  `code_volet` varchar(10) NOT NULL,
  `libelle_volet` varchar(100) NOT NULL,
  `actif` tinyint(1) NOT NULL DEFAULT 1,
  `ordre_affichage` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id_volet`),
  UNIQUE KEY `uq_ref_volets_code` (`code_volet`),
  UNIQUE KEY `uq_ref_volets_libelle` (`libelle_volet`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
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
  `id_type_seance` bigint(20) unsigned NOT NULL,
  `id_statut_seance` bigint(20) unsigned NOT NULL,
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
  KEY `fk_seances_ref_types_seance` (`id_type_seance`),
  KEY `fk_seances_ref_statuts_seance` (`id_statut_seance`),
  CONSTRAINT `fk_seances_dossiers` FOREIGN KEY (`id_dossier`) REFERENCES `dossiers` (`id_dossier`),
  CONSTRAINT `fk_seances_ref_statuts_seance` FOREIGN KEY (`id_statut_seance`) REFERENCES `ref_statuts_seance` (`id_statut_seance`),
  CONSTRAINT `fk_seances_ref_types_seance` FOREIGN KEY (`id_type_seance`) REFERENCES `ref_types_seance` (`id_type_seance`)
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
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*M!100616 SET NOTE_VERBOSITY=@OLD_NOTE_VERBOSITY */;

-- Dump completed on 2026-06-07 10:04:30
