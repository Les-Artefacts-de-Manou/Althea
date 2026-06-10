# Architecture technique de la base de données

 *Dernière mise à jour : 10/06/2026 — Lot 0 complet (référentiels + UC_RichTextEditorSimple)*

Document consolidé et dédupliqué à partir des sources SQL (`backup_NoData_althea.sql` et `backup_WithData_althea.sql`).

## Objectif

Permettre de comprendre rapidement la base, table par table, sans outil de gestion de base de données.

## Convention de nommage (uniformisée)

- Noms de tables et colonnes en `snake_case`
- Préfixes de tables: `ref_`, `lia_`, `tec_`, `sec_`
- Identifiants techniques: `id_*`
- Codes lisibles: `code_*`
- Datation standard: `date_creation`, `date_modification` quand présent

## Vue synthétique des tables

### Tables métier

- `autres_suivis_patient`
- `documents`
- `dossiers`
- `famille_contacts`
- `modeles_documents`
- `paiements`
- `patients`
- `rendez_vous`
- `seances`
- `therapeutes`

### Tables référentielles

- `ref_domaines`
- `ref_liens_patient`
- `ref_roles_intervenant`
- `ref_situations_familiales`
- `ref_statuts_dossier`
- `ref_statuts_seance`
- `ref_types_documents`
- `ref_types_rendez_vous`
- `ref_types_seance`

> ⚠️ **Note** : les tables `ref_*` utilisent **`AUTO_INCREMENT`** pour leurs clés primaires, contrairement aux tables métier qui utilisent `LASTVAL(seq_*)`. La table `ref_types_seance` a un champ additionnel `tarif_defaut decimal(10,2) NOT NULL DEFAULT 0.00` absent des autres référentiels.

### Tables de liaison

- `lia_paiements_seances`

### Tables techniques

- `tec_meta_schema`
- `tec_parametres`
- `tec_sync_evenements_google`

### Tables sécurité

- `sec_utilisateurs`

## Relations principales

- `autres_suivis_patient`: 
	- `id_patient` → `patients.id_patient`,
	- `id_role_intervenant` → `ref_roles_intervenant.id_role_intervenant`,
	- `id_therapeute` → `therapeutes.id_therapeute`
- `documents`: 
	- `id_dossier` → `dossiers.id_dossier`, 
	- `id_rendez_vous` → `rendez_vous.id_rendez_vous`, 
	- `id_seance` → `seances.id_seance`, 
	- `id_type_document` → `ref_types_documents.id_type_document`
- `dossiers`: 
	- `id_therapeute_traitant` → `therapeutes.id_therapeute`, 
	- `id_patient` → `patients.id_patient`, 
	- `id_statut_dossier` → `ref_statuts_dossier.id_statut_dossier`, 
	- `id_domaine` → `ref_domaines.id_domaine`
- `famille_contacts`: 
	- `id_lien_patient` → `ref_liens_patient.id_lien_patient`, 
	- `id_patient` → `patients.id_patient`
- `lia_paiements_seances`: 
	- `id_paiement` → `paiements.id_paiement`, 
	- `id_seance` → `seances.id_seance`
- `modeles_documents`: 
	- `id_type_document` → `ref_types_documents.id_type_document`, 
	- `id_domaine` → `ref_domaines.id_domaine`
- `patients`: 
	- `id_situation_familiale` → `ref_situations_familiales.id_situation_familiale`
- `rendez_vous`: 
	- `id_dossier` → `dossiers.id_dossier`, 
	- `id_patient` → `patients.id_patient`, 
	- `id_seance` → `seances.id_seance`, 
	- `id_type_rendez_vous` → `ref_types_rendez_vous.id_type_rendez_vous`
- `seances`: 
	- `id_dossier` → `dossiers.id_dossier`, 
	- `id_statut_seance` → `ref_statuts_seance.id_statut_seance`, 
	- `id_type_seance` → `ref_types_seance.id_type_seance`
- `tec_sync_evenements_google`: 
	- `id_rendez_vous` → `rendez_vous.id_rendez_vous`

## Dictionnaire des tables

### Tables métier

#### `autres_suivis_patient`

**Rôle**: Réseau d'intervenants externes liés au suivi d'un patient (liaison N-N). Chaque ligne associe un patient à un rôle d'intervenant et, optionnellement, à un thérapeute connu dans le référentiel.

**Références sortantes**: 

`id_patient` → `patients.id_patient`,

`id_role_intervenant` → `ref_roles_intervenant.id_role_intervenant`,

`id_therapeute` → `therapeutes.id_therapeute`

**Références entrantes**: —

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_autre_suivi_patient` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_autres_suivis_patient`) | PK | - | - |
| `code_autre_suivi_patient` | `varchar(12) GENERATED` | OUI | concat('AS',lpad(`id_autre_suivi_patient`,6,'0')) | UQ `uq_autres_suivis_patient_code` | - | - |
| `id_patient` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_autres_suivis_patient_patients` | `patients.id_patient` | - |
| `id_role_intervenant` | `bigint(20) unsigned` | OUI | NULL | FK `fk_autres_suivis_ref_roles` | `ref_roles_intervenant.id_role_intervenant` | - |
| `id_therapeute` | `bigint(20) unsigned` | OUI | NULL | FK `fk_autres_suivis_therapeutes` | `therapeutes.id_therapeute` | - |
| `nom_professionnel` | `varchar(150)` | OUI | NULL | - | - | - |
| `specialite` | `varchar(100)` | OUI | NULL | - | - | - |
| `lieu` | `varchar(150)` | OUI | NULL | - | - | - |
| `date_debut` | `date` | OUI | NULL | - | - | - |
| `date_fin` | `date` | OUI | NULL | - | - | - |
| `commentaire` | `text` | OUI | NULL | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

#### `documents`

**Rôle**: Documents métier (locaux/Google/PDF/images) liés au suivi.

**Références sortantes**: 

`id_dossier` → `dossiers.id_dossier`,

`id_rendez_vous` → `rendez_vous.id_rendez_vous`, 

`id_seance` → `seances.id_seance`, 

`id_type_document` → `ref_types_documents.id_type_document`

**Références entrantes**: —

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_document` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_documents`) | PK | - | - |
| `code_document` | `varchar(12) GENERATED ALWAYS AS (concat('DOC',lpad(`id_document`,6,'0'))) STORED` | OUI | Généré: concat('DOC',lpad(`id_document`,6,'0')) | UQ `uq_documents_code_document` | - | - |
| `id_dossier` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_documents_dossiers` | `dossiers.id_dossier` | - |
| `id_seance` | `bigint(20) unsigned` | OUI | NULL | FK `fk_documents_seances` | `seances.id_seance` | - |
| `id_rendez_vous` | `bigint(20) unsigned` | OUI | NULL | FK `fk_documents_rendez_vous` | `rendez_vous.id_rendez_vous` | - |
| `id_type_document` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_documents_ref_types_documents` | `ref_types_documents.id_type_document` | - |
| `type_source_document` | `varchar(50) NOT NULL` | NON | 'LOCAL' | - | - | - |
| `est_document_image_metier` | `tinyint(1) NOT NULL` | NON | 0 | - | - | - |
| `id_modele_document` | `bigint(20) unsigned` | OUI | NULL | - | - | - |
| `nom_document` | `varchar(255) NOT NULL` | NON | - | - | - | - |
| `nom_fichier` | `varchar(255) NOT NULL` | NON | - | - | - | - |
| `extension_fichier` | `varchar(10)` | OUI | NULL | - | - | - |
| `taille_fichier_octets` | `bigint(20)` | OUI | NULL | - | - | - |
| `chemin_relatif` | `varchar(500) NOT NULL` | NON | - | - | - | - |
| `chemin_pdf_relatif` | `text` | OUI | NULL | - | - | - |
| `chemin_miniature_relatif` | `text` | OUI | NULL | - | - | - |
| `google_file_id` | `varchar(255)` | OUI | NULL | - | - | - |
| `google_mime_type` | `varchar(150)` | OUI | NULL | - | - | - |
| `google_web_link` | `text` | OUI | NULL | - | - | - |
| `google_drive_folder_id` | `varchar(255)` | OUI | NULL | - | - | - |
| `statut_sync_google` | `varchar(30) NOT NULL` | NON | 'LOCAL_ONLY' | - | - | - |
| `date_sync_google` | `datetime` | OUI | NULL | - | - | - |
| `date_document` | `date` | OUI | NULL | - | - | - |
| `commentaire` | `text` | OUI | NULL | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

#### `dossiers`

**Rôle**: Dossier de suivi clinique par patient et par domaine métier.

**Références sortantes**: 

`id_therapeute_traitant` → `therapeutes.id_therapeute`, 

`id_patient` → `patients.id_patient`, 

`id_statut_dossier` → `ref_statuts_dossier.id_statut_dossier`, 

`id_domaine` → `ref_domaines.id_domaine`

**Références entrantes**:

`documents.id_dossier`, 

`rendez_vous.id_dossier`, 

`seances.id_dossier`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_dossier` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_dossiers`) | PK | - | - |
| `code_dossier` | `varchar(12) GENERATED` | OUI | concat('DO',lpad(`id_dossier`,6,'0')) | UQ `uq_dossiers_code_dossier` | - | - |
| `id_patient` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_dossiers_patients` | `patients.id_patient` | - |
| `id_domaine` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_dossiers_ref_domaines` | `ref_domaines.id_domaine` | - |
| `id_statut_dossier` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_dossiers_ref_statuts_dossier` | `ref_statuts_dossier.id_statut_dossier` | - |
| `date_ouverture` | `date NOT NULL` | NON | - | - | - | - |
| `date_cloture` | `date` | OUI | NULL | - | - | - |
| `prescripteur` | `varchar(150)` | OUI | NULL | - | - | - |
| `modalites_paiement` | `varchar(150)` | OUI | NULL | - | - | - |
| `id_therapeute_traitant` | `bigint(20) unsigned` | OUI | NULL | FK `fk_dossiers_therapeutes` | `therapeutes.id_therapeute` | - |
| `motif_suivi_rtf` | `longtext` | OUI | NULL | - | - | - |
| `motif_suivi_txt` | `longtext` | OUI | NULL | - | - | - |
| `historique_rtf` | `longtext` | OUI | NULL | - | - | - |
| `historique_txt` | `longtext` | OUI | NULL | - | - | - |
| `antecedents_rtf` | `longtext` | OUI | NULL | - | - | - |
| `antecedents_txt` | `longtext` | OUI | NULL | - | - | - |
| `diagnostic_rtf` | `longtext` | OUI | NULL | - | - | - |
| `diagnostic_txt` | `longtext` | OUI | NULL | - | - | - |
| `traitements_en_cours_rtf` | `longtext` | OUI | NULL | - | - | - |
| `traitements_en_cours_txt` | `longtext` | OUI | NULL | - | - | - |
| `objectifs_suivi_rtf` | `longtext` | OUI | NULL | - | - | - |
| `objectifs_suivi_txt` | `longtext` | OUI | NULL | - | - | - |
| `historique_archive_rtf` | `longtext` | OUI | NULL | - | - | - |
| `historique_archive_txt` | `longtext` | OUI | NULL | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

#### `famille_contacts`

**Rôle**: Contacts liés au patient (famille, représentant légal, urgence).

**Références sortantes**: 

`id_lien_patient` → `ref_liens_patient.id_lien_patient`, 

`id_patient` → `patients.id_patient`

**Références entrantes**: —

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_famille_contact` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_famille_contacts`) | PK | - | - |
| `code_famille_contact` | `varchar(12) GENERATED ALWAYS AS (concat('FC',lpad(`id_famille_contact`,6,'0'))) STORED` | OUI | Généré: concat('FC',lpad(`id_famille_contact`,6,'0')) | UQ `uq_famille_contacts_code_famille_contact` | - | - |
| `id_patient` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_famille_contacts_patients` | `patients.id_patient` | - |
| `id_lien_patient` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_famille_contacts_ref_liens_patient` | `ref_liens_patient.id_lien_patient` | - |
| `nom` | `varchar(100) NOT NULL` | NON | - | - | - | - |
| `prenom` | `varchar(100) NOT NULL` | NON | - | - | - | - |
| `date_naissance` | `date` | OUI | NULL | - | - | - |
| `telephone` | `varchar(50)` | OUI | NULL | - | - | - |
| `email` | `varchar(150)` | OUI | NULL | - | - | - |
| `adresse_ligne1` | `varchar(255)` | OUI | NULL | - | - | - |
| `adresse_ligne2` | `varchar(255)` | OUI | NULL | - | - | - |
| `code_postal` | `varchar(20)` | OUI | NULL | - | - | - |
| `localite` | `varchar(100)` | OUI | NULL | - | - | - |
| `pays` | `varchar(100)` | OUI | NULL | - | - | - |
| `autorite_parentale` | `tinyint(1) NOT NULL` | NON | 0 | - | - | - |
| `representant_legal` | `tinyint(1) NOT NULL` | NON | 0 | - | - | - |
| `personne_autorisee` | `tinyint(1) NOT NULL` | NON | 0 | - | - | - |
| `contact_urgence` | `tinyint(1) NOT NULL` | NON | 0 | - | - | - |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | - |
| `commentaire` | `text` | OUI | NULL | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

#### `therapeutes`

**Rôle**: Référentiel des thérapeutes traitants et prescripteurs (ex-`medecins`).

**Références sortantes**: —

**Références entrantes**: 

`dossiers.id_therapeute_traitant`,

`autres_suivis_patient.id_therapeute`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_therapeute` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_therapeutes`) | PK | - | - |
| `code_therapeute` | `varchar(12) GENERATED` | OUI | concat('TH',lpad(`id_therapeute`,6,'0')) | UQ `uq_therapeutes_code_therapeute` | - | - |
| `nom` | `varchar(100) NOT NULL` | NON | - | - | - | - |
| `prenom` | `varchar(100)` | OUI | NULL | - | - | - |
| `specialite` | `varchar(100)` | OUI | NULL | - | - | - |
| `telephone` | `varchar(50)` | OUI | NULL | - | - | - |
| `email` | `varchar(150)` | OUI | NULL | - | - | - |
| `adresse_ligne1` | `varchar(255)` | OUI | NULL | - | - | - |
| `adresse_ligne2` | `varchar(255)` | OUI | NULL | - | - | - |
| `code_postal` | `varchar(20)` | OUI | NULL | - | - | - |
| `localite` | `varchar(100)` | OUI | NULL | - | - | - |
| `pays` | `varchar(100)` | OUI | NULL | - | - | - |
| `commentaire` | `text` | OUI | NULL | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

#### `modeles_documents`

**Rôle**: Modèles de documents utilisables en génération documentaire.

**Références sortantes**: 

`id_type_document` → `ref_types_documents.id_type_document`, 

`id_domaine` → `ref_domaines.id_domaine`

**Références entrantes**: —

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_modele_document` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_modeles_documents`) | PK | - | - |
| `code_modele_document` | `varchar(12) GENERATED` | OUI | concat('MOD',lpad(`id_modele_document`,6,'0')) | UQ `uq_modeles_documents_code` | - | - |
| `id_type_document` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_modeles_documents_ref_types_documents` | `ref_types_documents.id_type_document` | - |
| `id_domaine` | `bigint(20) unsigned` | OUI | NULL | FK `fk_modeles_documents_ref_domaines` | `ref_domaines.id_domaine` | - |
| `nom_modele` | `varchar(255) NOT NULL` | NON | - | - | - | - |
| `nom_fichier` | `varchar(255) NOT NULL` | NON | - | - | - | - |
| `chemin_relatif` | `varchar(500) NOT NULL` | NON | - | - | - | - |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | - |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | - |
| `commentaire` | `text` | OUI | NULL | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

#### `paiements`

**Rôle**: Encaissements et informations de paiement.

**Références sortantes**: —

**Références entrantes**: 

`lia_paiements_seances.id_paiement`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_paiement` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_paiements`) | PK | - | - |
| `code_paiement` | `varchar(12) GENERATED ALWAYS AS (concat('PAI',lpad(`id_paiement`,6,'0'))) STORED` | OUI | Généré: concat('PAI',lpad(`id_paiement`,6,'0')) | UQ `uq_paiements_code_paiement` | - | - |
| `date_paiement` | `date NOT NULL` | NON | - | - | - | - |
| `montant` | `decimal(10,2) NOT NULL` | NON | - | - | - | - |
| `mode_paiement` | `varchar(50)` | OUI | NULL | - | - | - |
| `reference_paiement` | `varchar(100)` | OUI | NULL | - | - | - |
| `commentaire` | `text` | OUI | NULL | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

#### `patients`

**Rôle**: Données d'identité et de contact du patient.

**Références sortantes**: 

`id_situation_familiale` → `ref_situations_familiales.id_situation_familiale`

**Références entrantes**: 

`autres_suivis_patient.id_patient`,

`dossiers.id_patient`, 

`famille_contacts.id_patient`, 

`rendez_vous.id_patient`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_patient` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_patients`) | PK | - | 1 |
| `code_patient` | `varchar(12) GENERATED ALWAYS AS (concat('PA',lpad(`id_patient`,6,'0'))) STORED` | OUI | Généré: concat('PA',lpad(`id_patient`,6,'0')) | UQ `uq_patients_code_patient` | - | 'PA000001' |
| `nom` | `varchar(100) NOT NULL` | NON | - | - | - | 'bal' |
| `prenom` | `varchar(100) NOT NULL` | NON | - | - | - | 'but' |
| `date_naissance` | `date` | OUI | NULL | - | - | NULL |
| `niss` | `varchar(20)` | OUI | NULL | UQ `uq_patients_niss` | - | NULL |
| `lateralite` | `varchar(30)` | OUI | NULL | - | - | NULL |
| `adresse_ligne1` | `varchar(255)` | OUI | NULL | - | - | NULL |
| `adresse_ligne2` | `varchar(255)` | OUI | NULL | - | - | NULL |
| `code_postal` | `varchar(20)` | OUI | NULL | - | - | NULL |
| `localite` | `varchar(100)` | OUI | NULL | - | - | NULL |
| `pays` | `varchar(100)` | OUI | NULL | - | - | NULL |
| `telephone` | `varchar(50)` | OUI | NULL | - | - | NULL |
| `email` | `varchar(150)` | OUI | NULL | - | - | NULL |
| `mutualite` | `varchar(100)` | OUI | NULL | - | - | NULL |
| `id_situation_familiale` | `bigint(20) unsigned` | OUI | NULL | FK `fk_patients_ref_situations_familiales` | `ref_situations_familiales.id_situation_familiale` | 9 |
| `alerte` | `text` | OUI | NULL | - | - | NULL |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | '2026-03-31 18:20:12' |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | '2026-03-31 18:20:12' |

#### `rendez_vous`

**Rôle**: Événements d'agenda, liés au patient/dossier/séance.

**Références sortantes**: 

`id_dossier` → `dossiers.id_dossier`, 

`id_patient` → `patients.id_patient`, 

`id_seance` → `seances.id_seance`, 

`id_type_rendez_vous` → `ref_types_rendez_vous.id_type_rendez_vous`

**Références entrantes**: 

`documents.id_rendez_vous`, 

`tec_sync_evenements_google.id_rendez_vous`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_rendez_vous` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_rendez_vous`) | PK | - | - |
| `code_rendez_vous` | `varchar(12) GENERATED ALWAYS AS (concat('RDV',lpad(`id_rendez_vous`,6,'0'))) STORED` | OUI | Généré: concat('RDV',lpad(`id_rendez_vous`,6,'0')) | UQ `uq_rdv_code` | - | - |
| `id_patient` | `bigint(20) unsigned` | OUI | NULL | FK `fk_rdv_patient` | `patients.id_patient` | - |
| `id_dossier` | `bigint(20) unsigned` | OUI | NULL | FK `fk_rdv_dossier` | `dossiers.id_dossier` | - |
| `id_seance` | `bigint(20) unsigned` | OUI | NULL | FK `fk_rdv_seance` | `seances.id_seance` | - |
| `id_type_rendez_vous` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_rdv_type` | `ref_types_rendez_vous.id_type_rendez_vous` | - |
| `titre` | `varchar(255) NOT NULL` | NON | - | - | - | - |
| `description` | `text` | OUI | NULL | - | - | - |
| `date_heure_debut` | `datetime NOT NULL` | NON | - | - | - | - |
| `date_heure_fin` | `datetime NOT NULL` | NON | - | - | - | - |
| `est_journee_entiere` | `tinyint(1) NOT NULL` | NON | 0 | - | - | - |
| `google_calendar_id` | `varchar(255)` | OUI | NULL | - | - | - |
| `google_event_id` | `varchar(255)` | OUI | NULL | - | - | - |
| `google_event_etag` | `varchar(255)` | OUI | NULL | - | - | - |
| `google_event_url` | `text` | OUI | NULL | - | - | - |
| `statut_sync_google` | `varchar(30) NOT NULL` | NON | 'NON_LIE' | - | - | - |
| `date_sync_google` | `datetime` | OUI | NULL | - | - | - |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

#### `seances`

**Rôle**: Séances de prise en charge rattachées à un dossier.

**Références sortantes**: 

`id_dossier` → `dossiers.id_dossier`, 

`id_statut_seance` → `ref_statuts_seance.id_statut_seance`, 

`id_type_seance` → `ref_types_seance.id_type_seance`

**Références entrantes**: 

`documents.id_seance`, 

`lia_paiements_seances.id_seance`, 

`rendez_vous.id_seance`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_seance` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_seances`) | PK | - | - |
| `code_seance` | `varchar(12) GENERATED ALWAYS AS (concat('SE',lpad(`id_seance`,6,'0'))) STORED` | OUI | Généré: concat('SE',lpad(`id_seance`,6,'0')) | UQ `uq_seances_code_seance` | - | - |
| `id_dossier` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_seances_dossiers` | `dossiers.id_dossier` | - |
| `id_type_seance` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_seances_ref_types_seance` | `ref_types_seance.id_type_seance` | - |
| `id_statut_seance` | `bigint(20) unsigned NOT NULL` | NON | - | FK `fk_seances_ref_statuts_seance` | `ref_statuts_seance.id_statut_seance` | - |
| `date_seance` | `date NOT NULL` | NON | - | - | - | - |
| `heure_debut` | `time` | OUI | NULL | - | - | - |
| `heure_fin` | `time` | OUI | NULL | - | - | - |
| `duree_minutes` | `smallint(5) unsigned` | OUI | NULL | - | - | - |
| `tarif_seance` | `decimal(10,2) NOT NULL` | NON | 0.00 | - | - | - |
| `notes_seance_rtf` | `longtext` | OUI | NULL | - | - | - |
| `notes_seance_txt` | `longtext` | OUI | NULL | - | - | - |
| `evolution_rtf` | `longtext` | OUI | NULL | - | - | - |
| `evolution_txt` | `longtext` | OUI | NULL | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

### Tables référentielles

#### `ref_liens_patient`

**Rôle**: Référentiel des liens familiaux/proches du patient.

**Références sortantes**: —

**Références entrantes**: 

`famille_contacts.id_lien_patient`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_lien_patient` | `bigint(20) unsigned NOT NULL` | NON | - | PK | - | 1 |
| `code_lien_patient` | `varchar(50) NOT NULL` | NON | - | UQ `uq_ref_liens_patient_code` | - | 'MERE' |
| `libelle_lien_patient` | `varchar(100) NOT NULL` | NON | - | UQ `uq_ref_liens_patient_libelle` | - | 'Mère' |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 10 |

#### `ref_situations_familiales`

**Rôle**: Référentiel des situations familiales.

**Références sortantes**: —

**Références entrantes**: 

`patients.id_situation_familiale`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_situation_familiale` | `bigint(20) unsigned NOT NULL` | NON | - | PK | - | 1 |
| `code_situation_familiale` | `varchar(50) NOT NULL` | NON | - | UQ `uq_ref_situations_familiales_code` | - | 'PARENTS_ENSEMBLE' |
| `libelle_situation_familiale` | `varchar(100) NOT NULL` | NON | - | UQ `uq_ref_situations_familiales_libelle` | - | 'Parents ensemble' |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 10 |

#### `ref_statuts_dossier`

**Rôle**: Référentiel des statuts de dossier.

**Références sortantes**: —

**Références entrantes**: 

`dossiers.id_statut_dossier`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_statut_dossier` | `bigint(20) unsigned NOT NULL` | NON | - | PK | - | 1 |
| `code_statut_dossier` | `varchar(30) NOT NULL` | NON | - | UQ `uq_ref_statuts_dossier_code` | - | 'ACTIF' |
| `libelle_statut_dossier` | `varchar(100) NOT NULL` | NON | - | UQ `uq_ref_statuts_dossier_libelle` | - | 'Actif' |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 10 |

#### `ref_statuts_seance`

**Rôle**: Référentiel des statuts de séance.

**Références sortantes**: —

**Références entrantes**:

`seances.id_statut_seance`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_statut_seance` | `bigint(20) unsigned NOT NULL` | NON | - | PK | - | 1 |
| `code_statut_seance` | `varchar(30) NOT NULL` | NON | - | UQ `uq_ref_statuts_seance_code` | - | 'PLANIFIEE' |
| `libelle_statut_seance` | `varchar(100) NOT NULL` | NON | - | UQ `uq_ref_statuts_seance_libelle` | - | 'Planifiée' |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 10 |

#### `ref_types_documents`

**Rôle**: Référentiel des types de documents.

**Références sortantes**: —

**Références entrantes**: 

`documents.id_type_document`,

`modeles_documents.id_type_document`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_type_document` | `bigint(20) unsigned NOT NULL` | NON | - | PK | - | 1 |
| `code_type_document` | `varchar(30) NOT NULL` | NON | - | UQ `uq_ref_types_documents_code` | - | 'BILAN' |
| `libelle_type_document` | `varchar(100) NOT NULL` | NON | - | UQ `uq_ref_types_documents_libelle` | - | 'Bilan' |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 10 |

#### `ref_types_rendez_vous`

**Rôle**: Référentiel des types de rendez-vous.

**Références sortantes**: —

**Références entrantes**: 

`rendez_vous.id_type_rendez_vous`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_type_rendez_vous` | `bigint(20) unsigned NOT NULL` | NON | - | PK | - | 1 |
| `code_type_rendez_vous` | `varchar(30) NOT NULL` | NON | - | UQ `uq_ref_types_rdv_code` | - | 'SEANCE' |
| `libelle_type_rendez_vous` | `varchar(100) NOT NULL` | NON | - | UQ `uq_ref_types_rdv_libelle` | - | 'Séance' |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 10 |

#### `ref_types_seance`

**Rôle**: Référentiel des types de séance et tarif par défaut.

**Références sortantes**: —

**Références entrantes**: 

`seances.id_type_seance`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_type_seance` | `bigint(20) unsigned NOT NULL` | NON | - | PK | - | 1 |
| `code_type_seance` | `varchar(30) NOT NULL` | NON | - | UQ `uq_ref_types_seance_code` | - | 'SEANCE_STANDARD' |
| `libelle_type_seance` | `varchar(100) NOT NULL` | NON | - | UQ `uq_ref_types_seance_libelle` | - | 'Séance standard' |
| `tarif_defaut` | `decimal(10,2) NOT NULL` | NON | - | - | - | 60.00 |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 10 |

#### `ref_domaines`

**Rôle**: Référentiel des domaines de suivi (ex-`ref_volets`). Définit les spécialités pratiquées (Psychologie, Graphothérapie, etc.).

**Références sortantes**: —

**Références entrantes**:

`dossiers.id_domaine`, 

`modeles_documents.id_domaine`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_domaine` | `bigint(20) unsigned NOT NULL` | NON | - | PK | - | 1 |
| `code_domaine` | `varchar(10) NOT NULL` | NON | - | UQ `uq_ref_domaines_code` | - | 'PSY' |
| `libelle_domaine` | `varchar(100) NOT NULL` | NON | - | UQ `uq_ref_domaines_libelle` | - | 'Psychologie' |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 10 |

#### `ref_roles_intervenant`

**Rôle**: Référentiel des rôles possibles pour un intervenant externe dans le réseau de suivi du patient.

**Références sortantes**: —

**Références entrantes**:

`autres_suivis_patient.id_role_intervenant`

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_role_intervenant` | `bigint(20) unsigned NOT NULL` | NON | - | PK | - | 1 |
| `code_role_intervenant` | `varchar(30) NOT NULL` | NON | - | UQ `uq_ref_roles_intervenant_code` | - | 'MEDECIN_TRAITANT' |
| `libelle_role_intervenant` | `varchar(100) NOT NULL` | NON | - | UQ `uq_ref_roles_intervenant_libelle` | - | 'Médecin traitant' |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 10 |

### Tables de liaison

#### `lia_paiements_seances`

**Rôle**: Affectation d'un paiement à une ou plusieurs séances.

**Références sortantes**: 

`id_paiement` → `paiements.id_paiement`,

`id_seance` → `seances.id_seance`

**Références entrantes**: —

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_lia_paiements_seances` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_lia_paiements_seances`) | PK | - | - |
| `id_paiement` | `bigint(20) unsigned NOT NULL` | NON | - | UQ `uq_lia_paiement_seance`, FK `fk_lia_paiements_seances_paiements` | `paiements.id_paiement` | - |
| `id_seance` | `bigint(20) unsigned NOT NULL` | NON | - | UQ `uq_lia_paiement_seance`, FK `fk_lia_paiements_seances_seances` | `seances.id_seance` | - |
| `montant_affecte` | `decimal(10,2) NOT NULL` | NON | - | - | - | - |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

### Tables techniques

#### `tec_meta_schema`

**Rôle**: Historique des versions du schéma appliquées.

**Références sortantes**: —

**Références entrantes**: —

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_meta_schema` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_tec_meta_schema`) | PK | - | 1 |
| `version_schema` | `varchar(20) NOT NULL` | NON | - | UQ `uq_tec_meta_schema_version` | - | '1.0.0' |
| `description` | `varchar(255)` | OUI | NULL | - | - | 'Initialisation de la base Althea' |
| `date_application` | `datetime NOT NULL` | NON | current_timestamp() | - | - | '2026-04-10 18:05:35' |

#### `tec_parametres`

**Rôle**: Paramètres techniques et fonctionnels de l'application.

**Références sortantes**: —

**Références entrantes**: —

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_parametre` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_tec_parametres`) | PK | - | 1 |
| `code_parametre` | `varchar(12) GENERATED ALWAYS AS (concat('PAR',lpad(`id_parametre`,6,'0'))) STORED` | OUI | Généré: concat('PAR',lpad(`id_parametre`,6,'0')) | UQ `uq_tec_parametres_code_parametre` | - | 'PAR000001' |
| `cle_parametre` | `varchar(100) NOT NULL` | NON | - | UQ `uq_tec_parametres_cle_parametre` | - | 'APP_DATA' |
| `libelle_parametre` | `varchar(150) NOT NULL` | NON | - | - | - | 'Répertoire de l\\'Application' |
| `groupe_parametre` | `varchar(50) NOT NULL` | NON | 'GENERAL' | - | - | 'GENERAL' |
| `type_valeur` | `varchar(30) NOT NULL` | NON | 'STRING' | - | - | 'STRING' |
| `valeur_parametre` | `text` | OUI | NULL | - | - | 'Application' |
| `description_parametre` | `text` | OUI | NULL | - | - | 'Ceci est un test. Patatras' |
| `modifiable_utilisateur` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | - | - | 1 |
| `ordre_affichage` | `int(11) NOT NULL` | NON | 0 | - | - | 0 |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | '2026-04-28 17:14:27' |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | '2026-05-02 14:33:30' |

#### `tec_sync_evenements_google`

**Rôle**: Journal technique des synchronisations Google Calendar.

**Références sortantes**: 

`id_rendez_vous` → `rendez_vous.id_rendez_vous`

**Références entrantes**: —

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_sync_evenement_google` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_tec_sync_evenements_google`) | PK | - | - |
| `code_sync_evenement_google` | `varchar(12) GENERATED ALWAYS AS (concat('SEG',lpad(`id_sync_evenement_google`,6,'0'))) STORED` | OUI | Généré: concat('SEG',lpad(`id_sync_evenement_google`,6,'0')) | UQ `uq_tec_sync_evenements_google_code` | - | - |
| `id_rendez_vous` | `bigint(20) unsigned` | OUI | NULL | FK `fk_tec_sync_evenements_google_rendez_vous` | `rendez_vous.id_rendez_vous` | - |
| `google_calendar_id` | `varchar(255)` | OUI | NULL | - | - | - |
| `google_event_id` | `varchar(255)` | OUI | NULL | - | - | - |
| `operation_sync` | `varchar(20) NOT NULL` | NON | - | - | - | - |
| `statut_sync` | `varchar(30) NOT NULL` | NON | - | - | - | - |
| `message_sync` | `text` | OUI | NULL | - | - | - |
| `date_sync` | `datetime NOT NULL` | NON | current_timestamp() | - | - | - |

### Tables sécurité

#### `sec_utilisateurs`

**Rôle**: Comptes applicatifs, rôles et sécurité d'authentification.

**Références sortantes**: —

**Références entrantes**: —

| Champ | Type SQL | Null | Défaut / Généré | Clé / contrainte | Référence | Exemple |
|---|---|---|---|---|---|---|
| `id_utilisateur` | `bigint(20) unsigned NOT NULL` | NON | nextval(`althea`.`seq_sec_utilisateurs`) | PK | - | 2 |
| `code_utilisateur` | `varchar(20) GENERATED ALWAYS AS (concat('USR',lpad(`id_utilisateur`,6,'0'))) STORED` | OUI | Généré: concat('USR',lpad(`id_utilisateur`,6,'0')) | UQ `uq_sec_utilisateurs_code` | - | 'USR000002' |
| `login_utilisateur` | `varchar(100) NOT NULL` | NON | - | UQ `uq_sec_utilisateurs_login` | - | 'joelle' |
| `nom_affichage` | `varchar(150) NOT NULL` | NON | - | - | - | 'Joëlle - Administratrice' |
| `password_hash` | `varchar(255) NOT NULL` | NON | - | - | - | 'ILHCLqcbUpTNWU3TpTqFFtX46MlW2uLxMAoJK5V2U+E=' |
| `password_salt` | `varchar(255) NOT NULL` | NON | - | - | - | '0kldSj3qCi2uaQGPKB+4PSqLREKaZg5bw1aTvbbJZ5s=' |
| `role_utilisateur` | `varchar(20) NOT NULL` | NON | - | CHECK `chk_sec_utilisateurs_role` | - | 'Admin' |
| `role_max_elevation` | `varchar(20) NOT NULL` | NON | 'User' | CHECK `chk_sec_utilisateurs_role_max_elevation` | - | 'Admin' |
| `actif` | `tinyint(1) NOT NULL` | NON | 1 | CHECK `chk_sec_utilisateurs_actif` | - | 1 |
| `must_change_password` | `tinyint(1) NOT NULL` | NON | 1 | CHECK `chk_sec_utilisateurs_must_change_password` | - | 0 |
| `nb_echecs_login` | `int(11) NOT NULL` | NON | 0 | CHECK `chk_sec_utilisateurs_nb_echecs` | - | 0 |
| `compte_verrouille` | `tinyint(1) NOT NULL` | NON | 0 | - | - | 0 |
| `date_verrouillage` | `datetime` | OUI | NULL | - | - | '2026-05-09 18:08:46' |
| `dernier_login` | `datetime` | OUI | NULL | - | - | NULL |
| `date_creation` | `datetime NOT NULL` | NON | current_timestamp() | - | - | '2026-05-07 16:49:00' |
| `date_modification` | `datetime NOT NULL` | NON | current_timestamp() | - | - | '2026-05-16 19:27:08' |

## Séquences

- `seq_autres_suivis_patient`
- `seq_documents`
- `seq_dossiers`
- `seq_famille_contacts`
- `seq_lia_paiements_seances`
- `seq_therapeutes` *(anciennement `seq_medecins` — renommé ADR-17)*
- `seq_modeles_documents`
- `seq_paiements`
- `seq_patients`
- `seq_rendez_vous`
- `seq_seances`
- `seq_sec_utilisateurs`
- `seq_tec_meta_schema`
- `seq_tec_parametres`
- `seq_tec_sync_evenements_google`

> ⚠️ Les tables `ref_*` n'ont **pas** de séquence dédiée — elles utilisent `AUTO_INCREMENT` natif MariaDB.

## Triggers

- Aucun trigger détecté dans `backup_NoData_althea.sql`

## Views

- Aucune vue détectée dans `backup_NoData_althea.sql`

## Diagrammes disponibles

- Vue globale (toutes tables): `Docs/Database/Diagrams/althea_All.png`
- Vue globale (clés): `Docs/Database/Diagrams/althea_Key.png`
- Diagrammes détaillés par table: dossier `Docs/Database/Diagrams/`
