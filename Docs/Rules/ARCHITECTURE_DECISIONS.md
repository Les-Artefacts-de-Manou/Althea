# Althéa - Architecture Decision Records (ADR)

> Ce document regroupe les décisions d'architecture majeures prises pour Althéa.  
> Chaque ADR explique le **contexte**, les **options envisagées**, la **décision retenue** et ses **conséquences**.  
> Une décision actée ne doit être remise en cause que via un nouvel ADR explicite.

> *Dernière mise à jour : 25/10/2026*
---

## Cadre d'utilisation (gouvernance ADR)

### Objectif

Ce document sert de référence pour :

- préserver la cohérence technique du projet,
- faciliter la reprise/maintenance par un tiers,
- tracer les choix structurants et leurs impacts.

### Statuts ADR

- **Actée** : décision validée et appliquée dans le code
- **À instruire** : décision identifiée mais arbitrage final non acté
- **Abandonnée** : piste étudiée puis écartée (conservée pour historique)

### Sources de vérité associées

- Code : `Core/`, `Metier/`, `UI/`, `Utils/`
- Synthèse projet : `Docs/ETAT_DU_PROJET.md`
- Processus : `Docs/Process_Althea.md`
- Base de données : `Docs/Database/`
- Guide utilisateur Domaines (création, catégories, quotas, natures) : `Docs/Guide/Guide_Utilisateur_Domaines.md`

---

## Table des décisions

<!-- TOC-->
  - [ADR-01 - Base de données centrale MariaDB](#adr-01---base-de-données-centrale-mariadb)
    - [Contexte](#contexte)
    - [Options envisagées](#options-envisagées)
    - [Décision](#décision)
    - [Conséquences](#conséquences)
  - [ADR-02 - Chiffrement des secrets locaux par DPAPI](#adr-02---chiffrement-des-secrets-locaux-par-dpapi)
    - [Contexte](#contexte-1)
    - [Options envisagées](#options-envisagées-1)
    - [Décision](#décision-1)
    - [Conséquences](#conséquences-1)
  - [ADR-03 - Hachage des mots de passe PBKDF2](#adr-03---hachage-des-mots-de-passe-pbkdf2)
    - [Contexte](#contexte-2)
    - [Options envisagées](#options-envisagées-2)
    - [Décision](#décision-2)
    - [Conséquences](#conséquences-2)
  - [ADR-04 - Architecture en couches Core / Metier / UI / Utils](#adr-04---architecture-en-couches-core--metier--ui--utils)
    - [Contexte](#contexte-3)
    - [Options envisagées](#options-envisagées-3)
    - [Décision](#décision-3)
    - [Conséquences](#conséquences-3)
  - [ADR-05 - Point d'accès DB unique (`DatabaseManager`)](#adr-05---point-daccès-db-unique-databasemanager)
    - [Contexte](#contexte-4)
    - [Options envisagées](#options-envisagées-4)
    - [Décision](#décision-4)
    - [Conséquences](#conséquences-4)
  - [ADR-06 - SQL centralisé dans des modules Query dédiés](#adr-06---sql-centralisé-dans-des-modules-query-dédiés)
    - [Contexte](#contexte-5)
    - [Options envisagées](#options-envisagées-5)
    - [Décision](#décision-5)
    - [Conséquences](#conséquences-5)
  - [ADR-07 - Démarrage applicatif bloquant](#adr-07---démarrage-applicatif-bloquant)
    - [Contexte](#contexte-6)
    - [Options envisagées](#options-envisagées-6)
    - [Décision](#décision-6)
    - [Conséquences](#conséquences-6)
  - [ADR-08 - Système de rôles à 3 niveaux](#adr-08---système-de-rôles-à-3-niveaux)
    - [Contexte](#contexte-7)
    - [Options envisagées](#options-envisagées-7)
    - [Décision](#décision-7)
    - [Conséquences](#conséquences-7)
  - [ADR-09 - Navigation centralisée via `NavigationManager`](#adr-09---navigation-centralisée-via-navigationmanager)
    - [Contexte](#contexte-8)
    - [Options envisagées](#options-envisagées-8)
    - [Décision](#décision-8)
    - [Conséquences](#conséquences-8)
  - [ADR-10 - Gestion des utilisateurs avec modes d'édition distincts](#adr-10---gestion-des-utilisateurs-avec-modes-dédition-distincts)
    - [Contexte](#contexte-9)
    - [Options envisagées](#options-envisagées-9)
    - [Décision](#décision-9)
    - [Conséquences](#conséquences-9)
    - [Application — Motif « consultation d'abord » des dialogues patients (18/06/2026)](#application--motif--consultation-dabord--des-dialogues-patients-18062026)
    - [Application — Harmonisation du contexte UI des dialogues modaux (01/07/2026)](#application--harmonisation-du-contexte-ui-des-dialogues-modaux-01072026)
  - [ADR-11 - DialogChoix : remplacement systématique des MessageBox](#adr-11---dialogchoix--remplacement-systématique-des-messagebox)
    - [Contexte](#contexte-10)
    - [Options envisagées](#options-envisagées-10)
    - [Décision](#décision-10)
    - [Conséquences](#conséquences-10)
  - [ADR-12 - Centralisation des icônes d'état via UtilsIcons](#adr-12---centralisation-des-icônes-détat-via-utilsicons)
    - [Contexte](#contexte-11)
    - [Options envisagées](#options-envisagées-11)
    - [Décision](#décision-11)
    - [Conséquences](#conséquences-11)
  - [ADR-13 - Intégration du module Documents V1 depuis POC](#adr-13---intégration-du-module-documents-v1-depuis-poc)
    - [Contexte](#contexte-12)
    - [Décision](#décision-12)
    - [Points de validation attendus](#points-de-validation-attendus)
  - [ADR-14 - Intégration du module Agenda V1 depuis POC](#adr-14---intégration-du-module-agenda-v1-depuis-poc)
    - [Décision](#décision-13)
  - [ADR-15 - Composants standard UC_RichTextEditor / UC_RichTextEditorSimple pour notes formatées](#adr-15---composants-standard-uc_richtexteditor--uc_richtexteditorsimple-pour-notes-formatées)
    - [Contexte](#contexte-13)
    - [Options envisagées](#options-envisagées-12)
    - [Décision](#décision-14)
    - [Architecture retenue](#architecture-retenue)
      - [Séparation UI / Logique métier](#séparation-ui--logique-métier)
      - [Sauvegarde double format (règle stricte)](#sauvegarde-double-format-règle-stricte)
      - [Système d'impression avancé](#système-dimpression-avancé)
      - [Exports professionnels (Syncfusion)](#exports-professionnels-syncfusion)
      - [Packages Syncfusion requis](#packages-syncfusion-requis)
    - [Conséquences](#conséquences-12)
      - [✅ Positives](#-positives)
      - [⚠️ Points de vigilance](#-points-de-vigilance)
      - [📋 Règles d'utilisation (voir `Docs/Rules/Rules.md` §21)](#-règles-dutilisation-voir-docsrulesrulesmd-21)
      - [📚 Documentation de référence](#-documentation-de-référence)
      - [Variante allégée : UC_RichTextEditorSimple (ajout 10/06/2026)](#variante-allégée--uc_richtexteditorsimple-ajout-10062026)
    - [Traçabilité](#traçabilité)
  - [ADR-16 - Stockage des fichiers hors base, chemin déterministe](#adr-16---stockage-des-fichiers-hors-base-chemin-déterministe)
    - [Contexte](#contexte-14)
    - [Options envisagées](#options-envisagées-13)
    - [Décision](#décision-15)
    - [Conséquences](#conséquences-13)
  - [ADR-17 - Renommages de schéma (domaines, thérapeutes) + réseau d'intervenants](#adr-17---renommages-de-schéma-domaines-thérapeutes--réseau-dintervenants)
    - [Contexte](#contexte-15)
    - [Décision](#décision-16)
    - [Conséquences](#conséquences-14)
  - [ADR-18 - Référentiels : UC physique + classe de base `UC_ReferentielBase`](#adr-18---référentiels--uc-physique--classe-de-base-uc_referentielbase)
    - [Contexte](#contexte-16)
    - [Décision](#décision-17)
    - [Conséquences](#conséquences-15)
    - [Traçabilité](#traçabilité-1)
  - [ADR-19 - Anticipation multi-utilisateur (`id_utilisateur` nullable)](#adr-19---anticipation-multi-utilisateur-id_utilisateur-nullable)
    - [Contexte](#contexte-17)
    - [Décision](#décision-18)
    - [Conséquences](#conséquences-16)
  - [ADR-20 - Identification des fichiers patients par `code_patient` (PA000003) et non par `id`](#adr-20---identification-des-fichiers-patients-par-code_patient-pa000003-et-non-par-id)
    - [Contexte](#contexte-18)
    - [Décision](#décision-19)
    - [Justification](#justification)
    - [Règle associée](#règle-associée)
    - [Conséquences](#conséquences-17)
  - [ADR-21 - Export documentaire délégué par événement dans `UC_RichTextEditor`](#adr-21---export-documentaire-délégué-par-événement-dans-uc_richtexteditor)
    - [Contexte](#contexte-19)
    - [Décision](#décision-20)
    - [Classes introduites](#classes-introduites)
    - [Application côté `UC_PatientFiche`](#application-côté-uc_patientfiche)
    - [Règle d'extension future](#règle-dextension-future)
    - [Conséquences](#conséquences-18)
  - [ADR-22 - Dossiers : identité générée par la base, statut Actif forcé et référent au niveau patient](#adr-22---dossiers--identité-générée-par-la-base-statut-actif-forcé-et-référent-au-niveau-patient)
    - [Contexte](#contexte-20)
    - [Options envisagées](#options-envisagées-14)
    - [Décision](#décision-21)
    - [Conséquences](#conséquences-19)
  - [ADR-23 - Documents exportés uniques par contexte (nom déterministe + upsert)](#adr-23---documents-exportés-uniques-par-contexte-nom-déterministe--upsert)
    - [Contexte](#contexte-21)
    - [Options envisagées](#options-envisagées-15)
    - [Décision](#décision-22)
    - [Conséquences](#conséquences-20)
  - [ADR-24 - Règles de tarification et de quota portées par le domaine](#adr-24---règles-de-tarification-et-de-quota-portées-par-le-domaine)
    - [Contexte](#contexte-22)
    - [Décision](#décision-23)
    - [Conséquences](#conséquences-21)
    - [Traçabilité](#traçabilité-2)
  - [ADR-25 - Paiement contextualisé par la séance et distinction statut / solde](#adr-25---paiement-contextualisé-par-la-séance-et-distinction-statut--solde)
    - [Contexte](#contexte-23)
    - [Décision](#décision-24)
    - [Conséquences](#conséquences-22)
    - [Traçabilité](#traçabilité-3)
  - [ADR-26 - Cohérence séances ↔ paiements : règle unique, traçabilité et décision utilisateur](#adr-26---cohérence-séances--paiements--règle-unique-traçabilité-et-décision-utilisateur)
  - [ADR-27 - Domaine = convention : organisme, catégories de quota et plafonds paramétrables](#adr-27---domaine--convention--organisme-catégories-de-quota-et-plafonds-paramétrables)
  - [ADR-28 - Protection des lignes système des référentiels (`est_systeme`)](#adr-28---protection-des-lignes-système-des-référentiels-est_systeme)
  - [Processus de mise à jour ADR](#processus-de-mise-à-jour-adr)
    - [Déclencheurs typiques de mise à jour](#déclencheurs-typiques-de-mise-à-jour)
    - [Règles de maintenance](#règles-de-maintenance)
- [Contact](#contact)
<!-- TOC -->

| # | Titre | Statut | Traçabilité principale |
|---|-------|--------|------------------------|
| [ADR-01](#adr-01--base-de-données-centrale-mariadb) | Base de données centrale MariaDB | Actée | `Docs/Database/`, `Core/Database/DatabaseManager.vb` |
| [ADR-02](#adr-02--chiffrement-des-secrets-locaux-par-dpapi) | Chiffrement des secrets locaux par DPAPI | Actée | `Core/Security/CryptoManagerDPAPI.vb`, `Core/Configuration/ConfigManager.vb` |
| [ADR-03](#adr-03--hachage-des-mots-de-passe-pbkdf2) | Hachage des mots de passe PBKDF2 | Actée | `Core/Security/PasswordSecurityHelper.vb` |
| [ADR-04](#adr-04--architecture-en-couches-core--metier--ui--utils) | Architecture en couches Core / Metier / UI / Utils | Actée | Arborescence dépôt + `README.md` |
| [ADR-05](#adr-05--point-daccès-db-unique-databasemanager) | Point d'accès DB unique (`DatabaseManager`) | Actée | `Core/Database/DatabaseManager.vb` |
| [ADR-06](#adr-06--sql-centralisé-dans-des-modules-query-dédiés) | SQL centralisé dans des modules Query dédiés | Actée | `Core/Database/Queries/Query*.vb` |
| [ADR-07](#adr-07--démarrage-applicatif-bloquant) | Démarrage applicatif bloquant (DB obligatoire) | Actée | `Core/Startup/AppStartupManager.vb`, `UI/Forms/Home.vb` |
| [ADR-08](#adr-08--système-de-rôles-à-3-niveaux) | Système de rôles à 3 niveaux | Actée | `Core/Security/AppRole.vb`, `Core/Security/UserSession.vb` |
| [ADR-09](#adr-09--navigation-centralisée-via-navigationmanager) | Navigation centralisée via `NavigationManager` | Actée | `UI/Navigation/NavigationManager.vb`, `UI/Context/UserControlContext.vb` |
| [ADR-10](#adr-10--gestion-des-utilisateurs-avec-modes-dédition-distincts) | Gestion des utilisateurs avec modes d'édition distincts | Actée | `UI/Forms/Utilisateur/UtilisateurEdition.vb`, `Metier/Security/GestionUtilisateurs.vb`, `UI/Forms/Patients/ContactEdition.vb`, `UI/Forms/Patients/IntervenantEdition.vb`, `UI/Forms/Patients/TherapeuteEdition.vb` |
| [ADR-11](#adr-11--dialogchoix-remplacement-systématique-des-messagebox) | DialogChoix : remplacement systématique des MessageBox | Actée | `UI/Forms/Communs/DialogChoix.vb` |
| [ADR-12](#adr-12--centralisation-des-icônes-détat-via-utilsicons) | Centralisation des icônes d'état via UtilsIcons | Remplacée (18/10/2026, UI-9 : pastilles) | `Utils/UI/UtilsIcons.vb`, `Utils/UI/UtilsPastilles.vb` |
| [ADR-13](#adr-13--intégration-du-module-documents-v1-depuis-poc) | Intégration du module Documents V1 depuis POC | À instruire | `Docs/Poc/Gestion_documentaire_Althea.md`, `Docs/ETAT_DU_PROJET.md` |
| [ADR-14](#adr-14--intégration-du-module-agenda-v1-depuis-poc) | Intégration du module Agenda V1 depuis POC | À instruire | `Docs/Poc/Gestion_Calendrier_Althea.md`, `Docs/ETAT_DU_PROJET.md` |
| [ADR-15](#adr-15--composant-standard-uc_richtexteditor-pour-notes-formatées) | Composant standard UC_RichTextEditor pour notes formatées | Actée | `UI/Controls/Communs/UC_RichTextEditor.vb`, `Utils/Helpers/RichTextEditorHelper.vb`, `Docs/UC_RichTextEditor_Documentation.md` |
| [ADR-16](#adr-16--stockage-des-fichiers-hors-base-chemin-déterministe) | Stockage des fichiers hors base, chemin déterministe | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §8, `Docs/Poc/Gestion_documentaire_Althea.md` |
| [ADR-17](#adr-17--renommages-de-schéma-domaines-thérapeutes--réseau-dintervenants) | Renommages de schéma (domaines, thérapeutes) + réseau d'intervenants | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §3-§4 |
| [ADR-18](#adr-18--référentiels-uc-physique--classe-de-base-uc_referentielbase) | Référentiels : UC physique + classe de base `UC_ReferentielBase` | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §7.3 |
| [ADR-19](#adr-19--anticipation-multi-utilisateur-id_utilisateur-nullable) | Anticipation multi-utilisateur (`id_utilisateur` nullable) | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §12 BD-9 |
| [ADR-20](#adr-20--identification-des-fichiers-patients-par-code_patient-pa000003-et-non-par-id) | Identification des fichiers patients par `code_patient` (PA000003) et non par `id` | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §8.1, `Docs/Rules/Rules.md` §23, `Utils/Helpers/CheminsPatientHelper.vb` |
| [ADR-21](#adr-21--export-documentaire-délégué-par-événement-dans-uc_richtexteditor) | Export documentaire délégué par événement dans `UC_RichTextEditor` | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §8.3, `Docs/Rules/Rules.md` §24, `UI/Controls/Communs/UC_RichTextEditor.vb` |
| [ADR-22](#adr-22--dossiers--identité-générée-par-la-base-statut-actif-forcé-et-référent-au-niveau-patient) | Dossiers : identité générée par la base, statut Actif forcé et référent au niveau patient | Actée | `Core/Database/Queries/QueryDossiers.vb`, `Metier/Dossiers/GestionDossiers.vb`, `UI/Controls/Dossiers/UC_DossierFiche.vb`, `Docs/Conception/Plan_Conception_Dossiers_Althea.md` |
| [ADR-23](#adr-23--documents-exportés-uniques-par-contexte-nom-déterministe--upsert) | Documents exportés uniques par contexte (nom déterministe + upsert) | Actée | `Metier/Documents/GestionDocuments.vb`, `Core/Database/Queries/QueryDocuments.vb`, `UI/Forms/Dossiers/DossierZoneEditor.vb`, `UI/Controls/Patients/UC_PatientFiche.vb`, `Docs/Poc/Gestion_documentaire_Althea.md` |
| [ADR-24](#adr-24--règles-de-tarification-et-de-quota-portées-par-le-domaine) | Règles de tarification et de quota portées par le domaine | Actée — points 3 et 4 remplacés par ADR-27 | `ref_domaines`, `lia_prix_seance_domaines`, `lia_natures_seance_domaines`, `ref_quotas` (ex-`ref_quotas_inami`) |
| [ADR-25](#adr-25--paiement-contextualisé-par-la-séance-et-distinction-statut--solde) | Paiement contextualisé par la séance et distinction statut / solde | Actée | `QueryPaiements`, `PaiementListeItem`, `PaiementEditor`, `UC_DossierFiche` |
| [ADR-26](#adr-26---cohérence-séances--paiements--règle-unique-traçabilité-et-décision-utilisateur) | Cohérence séances ↔ paiements : règle unique, traçabilité et décision utilisateur | Actée | `GestionPaiements.ResynchroniserDuSeance`, `SeanceEditor`, `Guide_Seances_Paiements.md` |
| [ADR-27](#adr-27---domaine--convention--organisme-catégories-de-quota-et-plafonds-paramétrables) | Domaine = convention : organisme, catégories de quota et plafonds paramétrables | Actée — point 8 remplacé par ADR-40 | `ref_organismes`, `ref_categories_quota`, `ref_quotas`, `ref_quotas_plafonds`, `lia_natures_seance_domaines.id_categorie_quota`, `UC_DomainesHome` |
| [ADR-28](#adr-28---protection-des-lignes-système-des-référentiels-est_systeme) | Protection des lignes système des référentiels (`est_systeme`) | Actée | `UC_ReferentielBase`, `ReferentielLigne`, `UtilsDataReader`, migration v2.21.0 |
| [ADR-29](#adr-29---fondation-cabinet--praticien-situation-juridique-et-porteur-du-bce) | Fondation Cabinet / praticien, situation juridique et porteur du BCE | Actée | `Metier/Cabinet/`, `UC_Cabinet`, migrations v2.29.0 à v2.31.0 |
| [ADR-30](#adr-30---habillage-par-contrôles-dessinés-plutôt-que-par-images-de-fond) | Habillage par contrôles dessinés plutôt que par images de fond | Actée | `UI/Controls/Theme/Althea*.vb`, `AltheaDessin`, `UITheme` |
| [ADR-31](#adr-31---place-dune-tuile-dans-un-hub--lusage-métier-prime-sur-lhéritage-technique) | Place d'une tuile dans un hub : l'usage métier prime sur l'héritage technique | Actée | `UC_AdminHome`, `UC_ReferentielHome` |
| [ADR-32](#adr-32---onglets--tabcontrol-hérité-et-dessiné-sans-contrôle-tiers) | Onglets : `TabControl` hérité et dessiné, sans contrôle tiers | Actée | `UI/Controls/Theme/AltheaOnglets.vb` |
| [ADR-33](#adr-33---champs-du-kit--hériter-du-contrôle-windows-quand-cest-possible-composer-sinon) | Champs du kit : hériter du contrôle Windows quand c'est possible, composer sinon | Actée | `AltheaListe`, `AltheaCase`, `AltheaRecherche` |
| [ADR-34](#adr-34---deux-niveaux-de-boutons--barre-dactions-de-lécran-et-barres-locales) | Deux niveaux de boutons : barre d'actions de l'écran et barres locales | Actée | `AltheaBarreOutils`, `AltheaBouton` (variantes `Outil*`) |
| [ADR-35](#adr-35---dossier-de-données--seule-la-racine-est-paramétrable) | Dossier de données : seule la racine est paramétrable | Actée | `ArborescenceDonnees`, migration v2.37.0 |
| [ADR-36](#adr-36---paramètres-applicatifs--une-valeur-des-bornes-une-valeur-de-secours) | Paramètres applicatifs : une valeur, des bornes, une valeur de secours | Actée | `GestionParametres`, `UC_Parametres` |
| [ADR-37](#adr-37---sauvegarde--une-archive-zip-chiffrée-aes-256-contenant-la-base-et-le-dossier-de-données) | Sauvegarde : une archive ZIP chiffrée AES-256 contenant la base et le dossier de données | Actée | `GestionSauvegardes`, migration v2.38.0 |
| [ADR-38](#adr-38---restauration--depuis-lécran-même-version-sauvegarde-de-sécurité-et-compte-dadministration-saisi) | Restauration : depuis l'écran, même version, sauvegarde de sécurité et compte d'administration saisi | Actée | `GestionRestaurations`, migration v2.39.0 |
| [ADR-39](#adr-39---journaux-sans-données-personnelles-version-de-schéma-attendue-déclarée-dans-le-code) | Journaux sans données personnelles, version de schéma attendue déclarée dans le code | Actée | `GestionLog`, `VersionSchema`, `UC_Journaux`, `UC_EtatSysteme` |
| [ADR-40](#adr-40---domaines--étapes-guidées-et-règles-de-quota-corrigeables-tant-quelles-nont-pas-servi) | Domaines : étapes guidées et règles de quota corrigeables tant qu'elles n'ont pas servi | Actée | `UC_DomainesHome`, `AltheaEtapes`, `GestionQuotasInami` |
| [ADR-41](#adr-41---clés-de-licence-des-composants-tiers--saisies-dans-lapplication-chiffrées-sur-le-poste) | Clés de licence des composants tiers : saisies dans l'application, chiffrées sur le poste | Actée | `LicenceSyncfusion`, `GestionLicences`, `UC_Parametres` |
| [ADR-42](#adr-42---éditeur-de-texte--un-seul-contrôle-à-deux-profils-styles-de-paragraphe-et-teintes-de-la-charte) | Éditeur de texte : un seul contrôle à deux profils, styles de paragraphe et teintes de la charte | Actée — à valider | `UC_RichTextEditor`, `RichTextEditorHelper`, `AltheaRenduBarre` |
| [ADR-43](#adr-43---modifications-non-enregistrées--alerte-tenue-par-lécran-photo-des-champs-trois-choix) | Modifications non enregistrées : alerte tenue par l'écran, photo des champs, trois choix | Actée — à valider | `GardeModifications`, `DialogChoix.DemanderAvantDepart`, `IUnsavedChangesAware` |
| [ADR-44](#adr-44---textes-types--référentiel-en-base-proposés-selon-lusage-de-léditeur) | Textes types : référentiel en base, proposés selon l'usage de l'éditeur | Actée — à valider | `ref_textes_types`, `ref_usages_textes_types`, `UC_TextesTypes`, `UC_RichTextEditor.UsageTextesTypes` |
| [ADR-45](#adr-45---correcteur-dorthographe--celui-de-windows-dans-la-langue-du-clavier) | Correcteur d'orthographe : celui de Windows, dans la langue du clavier | Actée — à valider | `RichTextEditorHelper.DefinirCorrecteurOrthographe`, `CorrecteurWindows`, `EDITEUR_CORRECTEUR_ORTHOGRAPHE` |
| [ADR-46](#adr-46---opérations-en-plusieurs-écritures--une-transaction-tout-ou-rien) | Opérations en plusieurs écritures : une transaction, tout ou rien | Actée | `DatabaseManager.ExecuterEnTransaction`, `GestionSeances.EnregistrerSeance` |
| [ADR-47](#adr-47---statut-dune-séance-passée--la-base-garde-le-statut-saisi-laffichage-dit--à-confirmer-) | Statut d'une séance passée : la base garde le statut saisi, l'affichage dit « À confirmer » | Actée | `SeanceListeItem.EstAConfirmer`, `FamilleStatutSeance.AConfirmer` |
| [ADR-48](#adr-48---solde-dune-séance-et-dossier-dominant--une-seule-définition-dans-des-vues-sql) | Solde d'une séance et dossier dominant : une seule définition, dans des vues SQL | Actée | `v_seances_solde`, `v_dossier_dominant` (migration v2.49.0) |
| [ADR-49](#adr-49---paiements--mode-relié-par-identifiant-annulation-et-utilisation-davoir-tracées-en-colonnes) | Paiements : mode relié par identifiant, annulation et utilisation d'avoir tracées en colonnes | Actée | `paiements.id_mode_paiement`, colonnes d'annulation, `lia_paiements_avoirs` (v2.50.0 à v2.52.0) |
| [ADR-50](#adr-50---droits-côté-métier--un-contexteoperation-transmis-une-garde-unique-autorisation) | Droits côté métier : un ContexteOperation transmis, une garde unique Autorisation | Actée | `ContexteOperation`, `Autorisation` |
| [ADR-51](#adr-51---règles-de-saisie-et-daction--un-valideur-métier-lu-par-lécran-appliqué-par-lécriture) | Règles de saisie et d'action : un valideur métier, lu par l'écran, appliqué par l'écriture | Actée | `ErreurSaisie`, `ErreurValidationException`, `Gestion*.Valider…` |
| [ADR-52](#adr-52---recherche-transversale--un-hub-des-recherches-spécialisées-un-résultat-et-une-ouverture-communs) | Recherche transversale : un hub, des recherches spécialisées, un résultat et une ouverture communs | Actée | `UC_RechercheHome`, `ResultatRecherche`, `OuvertureResultat`, `GestionRecherche` |
| [ADR-53](#adr-53---codes-postaux-belges--un-référentiel-généré-depuis-bpost-et-best-en-lecture-seule) | Codes postaux belges : un référentiel généré depuis bpost et BeST, en lecture seule | Actée | `ref_codes_postaux_be`, `GestionCodesPostauxBe`, `SaisieLocaliteBelge` |
| [ADR-54](#adr-54---codes-système--un-point-unique-codessysteme-et-des-drapeaux-de-référentiel-pour-les-règles) | Codes système : un point unique (CodesSysteme) et des drapeaux de référentiel pour les règles | Actée | `CodesSysteme`, `Query*`, `ref_statuts_seance` |
| [ADR-55](#adr-55---fenêtre-principale--un-contrat-ifenetreprincipale-au-lieu-de-la-classe-home) | Fenêtre principale : un contrat (IFenetrePrincipale) au lieu de la classe Home | Actée | `IFenetrePrincipale`, `UserControlContext`, `Home` |
| [ADR-56](#adr-56---grosses-fiches-à-onglets--un-fichier-partiel-par-onglet) | Grosses fiches à onglets : un fichier partiel par onglet | Actée | `UC_DossierFiche`, `UC_PatientFiche` |
| [ADR-57](#adr-57---option-strict-on-pour-tout-le-projet-editorconfig-et-analyseurs-net) | Option Strict On pour tout le projet, .editorconfig et analyseurs .NET | Actée | `Althea.vbproj`, 38 fichiers `.vb`, `.editorconfig` |
| [ADR-58](#adr-58---tests-automatisés--un-projet-xunit-en-vb-à-côté-de-lapplication) | Tests automatisés : un projet xUnit en VB, à côté de l'application | Actée | `Tests/Althea.Tests`, `Tools/test.cmd` |
| [ADR-59](#adr-59---intégration-continue--une-vérification-github-par-jour-à-la-demande) | Intégration continue : une vérification GitHub par jour, à la demande | Actée | `.github/workflows/ci.yml` |

---

## ADR-01 - Base de données centrale MariaDB

**Date :** avril 2026  
**Statut :** Actée

### Contexte

Althéa doit persister des données médicales sensibles (patients, séances, paiements, documents) ainsi que des données techniques (configuration, utilisateurs, logs d'audit). Une solution de stockage structurée, robuste et centralisée est nécessaire.

### Options envisagées

| Option | Avantages | Inconvénients |
|--------|-----------|---------------|
| **MariaDB** | Mature, performant, relationnel, SQL standard, encodage UTF8MB4 natif | Nécessite une instance serveur locale |
| SQLite | Zéro serveur, simple à déployer | Moins adapté pour la concurrence ou une future évolution multi-poste |
| SQL Server LocalDB | Intégration .NET native | Licence Microsoft, plus lourd pour un usage mono-poste |

### Décision

**MariaDB** est retenu comme SGBD unique d'Althéa.

- Encodage **UTF8MB4** (support complet des caractères accentués et émojis).
- Séparation stricte des données : préfixes `tec_` (technique), `sec_` (sécurité), `ref_` (référentiel), `lia_` (liaison), sans préfixe pour les tables métier.
- Nommage `snake_case` uniformisé.
- Utilisation de **séquences** pour la génération des identifiants (plutôt que AUTO_INCREMENT).

### Conséquences

- L'application ne démarre pas sans instance MariaDB accessible (voir ADR-07).
- Le déploiement requiert une installation MariaDB sur le poste ou le réseau local.
- Les scripts SQL doivent être versionnés et maintenus dans `Docs/Database/`.
- Récupération des ID générés via `LASTVAL(nom_sequence)` au lieu de `LAST_INSERT_ID()`.

---

## ADR-02 - Chiffrement des secrets locaux par DPAPI

**Date :** avril 2026  
**Statut :** Actée

### Contexte

La configuration de connexion à la base de données (hôte, port, utilisateur, mot de passe DB) est stockée localement sur le poste dans un fichier JSON (`%APPDATA%\Althea\althea.local.json`). Ces informations sont sensibles et ne doivent pas être lisibles en clair.

### Options envisagées

| Option | Avantages | Inconvénients |
|--------|-----------|---------------|
| **DPAPI** (`DataProtectionScope.CurrentUser`) | Natif Windows, aucune clé à gérer, lié à la session Windows de l'utilisateur | Spécifique Windows ; secret non portable entre sessions/postes |
| AES avec clé hardcodée | Portable | La clé dans le binaire annule la protection réelle |
| Coffre-fort tiers (ex : Windows Credential Manager) | Robuste | Complexité d'intégration supérieure pour un gain limité en contexte mono-poste |

### Décision

**DPAPI avec `DataProtectionScope.CurrentUser`** est utilisé pour chiffrer et déchiffrer les secrets locaux.  
Module : `Core/Security/CryptoManagerDPAPI.vb`.

- Les données sont chiffrées en Base64 avant écriture sur disque.
- Seul l'utilisateur Windows ayant chiffré les données peut les déchiffrer.

### Conséquences

- Protection transparente sans gestion de clé applicative.
- Le fichier de configuration n'est pas portable entre comptes Windows ou postes différents (reconfiguration requise lors d'un changement de poste ou d'utilisateur Windows).
- Aucune dépendance externe supplémentaire.

---

## ADR-03 - Hachage des mots de passe PBKDF2

**Date :** avril 2026  
**Statut :** Actée

### Contexte

Les mots de passe des utilisateurs d'Althéa sont stockés en base. Ils ne doivent jamais être stockés en clair ni récupérables.

### Options envisagées

| Option | Avantages | Inconvénients |
|--------|-----------|---------------|
| **PBKDF2-SHA256** | Standard recommandé, sel intégré, paramétrable (itérations) | Légèrement plus complexe à implémenter qu'un simple SHA |
| BCrypt | Très sûr, facteur de coût adaptatif | Non natif .NET ; dépendance externe nécessaire |
| SHA-256 simple | Simple | Sans sel : vulnérable aux attaques par table arc-en-ciel |

### Décision

**PBKDF2-SHA256** est utilisé pour hacher les mots de passe.  
Module : `Core/Security/PasswordSecurityHelper.vb`.

- Sel aléatoire unique par mot de passe (16 octets).
- Nombre d'itérations : 100 000 (recommandation OWASP 2023).
- Hash de sortie : 32 octets (256 bits).

### Conséquences

- Aucune dépendance externe (classe native `Rfc2898DeriveBytes` de .NET).
- Les mots de passe ne sont jamais récupérables ; seule la vérification par comparaison de haché est possible.
- En cas de mise à jour du nombre d'itérations, les anciens haché restent vérifiables tant que le paramètre est mémorisé avec le haché.
- Performance acceptable pour l'authentification (quelques millisecondes par vérification).

---

## ADR-04 - Architecture en couches Core / Metier / UI / Utils

**Date :** avril 2026  
**Statut :** Actée

### Contexte

Althéa doit rester maintenable et lisible sur le long terme, y compris par un développeur qui reprendrait le projet. La séparation des responsabilités est un prérequis pour éviter l'enchevêtrement du code.

### Options envisagées

| Option | Description |
|--------|-------------|
| **Couches strictes** | `Core` (technique), `Metier` (logique), `UI` (interface), `Utils` (utilitaires) |
| Approche "tout dans les Forms" | Simplicité immédiate, dette technique rapide |
| Architecture DDD complète | Robuste, sur-dimensionné pour un projet mono-développeur |

### Décision

Architecture en **quatre couches** avec séparation stricte :

| Couche | Rôle | Exemples |
|--------|------|---------|
| `Core` | Infrastructure technique pure | DB, Config, Logging, Security, Startup |
| `Metier` | Logique métier, règles de gestion | Modules patients, paramètres, sécurité métier |
| `UI` | Présentation uniquement | Forms, UserControls, Navigation, Context |
| `Utils` | Helpers transverses sans logique métier | UITheme, UtilsValidation, UtilsDataGrid, UtilsIcons |

**Règle absolue :** aucun mélange de couches. L'UI n'accède pas directement à la DB. La logique métier n'importe pas de composants UI.

### Conséquences

- Lisibilité et maintenabilité accrues.
- Toute nouvelle fonctionnalité doit respecter ce découpage.
- Les tests (futurs) peuvent porter sur `Metier` et `Core` sans dépendance UI.

---

## ADR-05 - Point d'accès DB unique (`DatabaseManager`)

**Date :** avril 2026  
**Statut :** Actée

### Contexte

Sans discipline d'accès à la base, les connexions prolifèrent, les ressources ne sont pas libérées correctement et la traçabilité est impossible.

### Options envisagées

| Option | Description |
|--------|-------------|
| **`DatabaseManager` singleton** | Un seul point d'entrée, connexion gérée centralement |
| Accès DB direct depuis chaque module | Simple mais incontrôlable à l'échelle |

### Décision

**`DatabaseManager`** (`Core/Database/DatabaseManager.vb`) est le **seul composant autorisé** à ouvrir et fermer une connexion à MariaDB. Tout autre module passe par lui.

- Méthode principale : `OpenConnection()` retournant une `MySqlConnection` valide ou levant une exception.
- Utilisation systématique de `Using` pour garantir la libération des ressources.
- Aucune vérification de connexion (`conn.State`, `conn IsNot Nothing`) hors `DatabaseManager`.

### Conséquences

- Gestion uniforme des connexions (ouverture, fermeture, gestion des erreurs).
- Point de supervision unique en cas de problème de connexion.
- Facilite les évolutions futures (pool de connexions, reconnexion automatique).

---

## ADR-06 - SQL centralisé dans des modules Query dédiés

**Date :** avril 2026  
**Statut :** Actée

### Contexte

Si les requêtes SQL sont dispersées dans les formulaires ou les modules métier, la cohérence et la maintenabilité deviennent rapidement impossibles à garantir.

### Options envisagées

| Option | Description |
|--------|-------------|
| **Modules `Query*` dédiés** | Tout le SQL dans des modules séparés par domaine |
| ORM (Entity Framework) | Abstraction SQL complète, overhead et complexité pour un projet VB.NET mono-développeur |
| SQL inline dans les Forms | Simple mais non maintenable |

### Décision

Chaque domaine dispose d'un module **`Query<Domaine>.vb`** (`Core/Database/Queries/`) qui centralise toutes les requêtes SQL du domaine.

Exemples existants :
- `QueryParametres.vb`
- `QueryUtilisateurs.vb`

Règles :
- Requêtes paramétrées obligatoires (protection injection SQL).
- Aucun SQL inline dans les modules `Metier` ou `UI`.
- Documentation des requêtes complexes (commentaires expliquant le besoin métier).

### Conséquences

- Toute modification SQL est localisée dans un seul fichier par domaine.
- Les modules `Metier` et `UI` appellent les fonctions des modules `Query` sans écrire de SQL.
- Les nouveaux modules métier (Patients, Dossiers, Séances, Paiements) devront avoir leur propre module `Query`.

---

## ADR-07 - Démarrage applicatif bloquant

**Date :** avril 2026  
**Statut :** Actée

### Contexte

Althéa est une application de gestion médicale : toute donnée affichée ou saisie doit être immédiatement persistée. Démarrer sans base de données accessible expose à des pertes de données silencieuses.

### Options envisagées

| Option | Description |
|--------|-------------|
| **Démarrage bloquant** | L'application refuse de s'ouvrir sans connexion DB valide |
| Mode dégradé hors-ligne | Certaines fonctions disponibles sans DB |
| Avertissement non bloquant | L'utilisateur est prévenu mais peut continuer |

### Décision

Le démarrage est **bloquant par conception** :

1. La Form `Home` se charge et verrouille immédiatement l'interface.
2. `AppStartupManager` lit la configuration et teste la connexion DB.
3. En cas d'échec, `ConfigurationConnexion` s'ouvre pour correction.
4. Le cycle se répète jusqu'à succès ou abandon explicite.
5. L'interface n'est déverrouillée qu'après connexion réussie.

Module : `Core/Startup/AppStartupManager.vb`.

### Conséquences

- Garantit qu'aucune opération ne peut être effectuée sans base de données.
- Simplifie le diagnostic (un seul point de défaillance connu).
- L'expérience utilisateur est directe : si l'application s'ouvre, elle fonctionne.
- Nécessite une procédure claire de configuration initiale.

---

## ADR-08 - Système de rôles à 3 niveaux

**Date :** mai 2026  
**Statut :** Actée

### Contexte

Althéa peut être utilisée par plusieurs personnes avec des niveaux d'accès différents (praticien, assistant, administrateur système). Un modèle de rôles est nécessaire pour contrôler l'accès aux fonctionnalités sensibles.

### Options envisagées

| Option | Description |
|--------|-------------|
| **3 rôles fixes** | `User`, `SuperUser`, `Admin` - simple et suffisant |
| Permissions granulaires | Flexible mais complexité excessive pour le contexte |
| 2 rôles (User/Admin) | Trop limité si plusieurs niveaux de délégation sont utiles |

### Décision

**3 rôles** définis dans `Core/Security/AppRole.vb` :

| Rôle | Valeur | Usage |
|------|--------|-------|
| `User` | 0 | Accès standard (consultation, saisie courante) |
| `SuperUser` | 1 | Accès étendu (certaines actions admin déléguées, consultation utilisateurs, maintenance limitée) |
| `Admin` | 2 | Accès complet (gestion utilisateurs, paramètres système, toutes les actions) |

**Élévation temporaire** : un utilisateur peut élever temporairement ses droits avec authentification, sans changement de rôle permanent.

**RoleMaxElevation** : chaque utilisateur possède un rôle de base (`RoleUtilisateur`) et un rôle maximal autorisé (`RoleMaxElevation`). L'élévation ne peut jamais dépasser `RoleMaxElevation`.

### Conséquences

- Contrôle d'accès clair et facile à appliquer dans l'UI et la logique métier.
- L'élévation temporaire évite de multiplier les comptes Admin.
- Tout nouveau module doit systématiquement vérifier le rôle avant d'exposer des actions sensibles.
- Les actions de sécurité (création/modification utilisateur, reset password) sont tracées dans les logs.

---

## ADR-09 - Navigation centralisée via `NavigationManager`

**Date :** mai 2026  
**Statut :** Actée

### Contexte

Dans une application WinForms multi-écrans avec contexte utilisateur partagé (session, rôle, état courant), une navigation décentralisée conduit rapidement à des incohérences d'état et à un code dupliqué dans chaque Form.

### Options envisagées

| Option | Description |
|--------|-------------|
| **`NavigationManager` centralisé** | Un seul composant gère les transitions d'écran et le contexte partagé |
| Navigation directe Form → Form | Simple mais non maintenable au-delà de quelques écrans |

### Décision

**`NavigationManager`** (`UI/Navigation/NavigationManager.vb`) centralise toutes les transitions entre écrans.  
**`UserControlContext`** (`UI/Context/UserControlContext.vb`) porte le contexte partagé (session utilisateur, état navigation, StatusStrip, ToolTip, ErrorProvider, lblContexte).

Interfaces :
- `IContextAwareUserControl` : pour les UserControls chargés dans `Home`.
- `IContextAwareForm` : pour les Forms modales ouvertes depuis `Home`.

### Conséquences

- Les Forms et UserControls ne se connaissent pas directement : elles passent toutes par `NavigationManager`.
- L'état de la session utilisateur est cohérent sur tout le cycle de navigation.
- Tout nouvel écran doit être enregistré et routé via `NavigationManager`.
- Le contexte UI global (StatusStrip, ToolTip, ErrorProvider, lblContexte) est accessible via injection du contexte, évitant les accès directs.

---

## ADR-10 - Gestion des utilisateurs avec modes d'édition distincts

**Date :** juin 2026  
**Statut :** Actée

### Contexte

La gestion des utilisateurs applicatifs nécessite des workflows différenciés selon le rôle de l'utilisateur connecté et l'action à effectuer. Un Admin doit pouvoir créer et modifier des utilisateurs, un SuperUser doit pouvoir consulter et effectuer des actions de maintenance limitées, et un User ne doit avoir aucun accès à ce module.

### Options envisagées

| Option | Description |
|--------|-------------|
| **3 modes distincts** | `Creation`, `Modification`, `Consultation` avec droits différenciés |
| 2 modes (Creation/Modification) | Plus simple mais ne permet pas la délégation de maintenance au SuperUser |
| Mode unique avec masquage conditionnel | Complexe à maintenir, logique dispersée |

### Décision

**3 modes d'édition** définis dans l'enum `ModeUtilisateurEdition` :

| Mode | Rôle autorisé | Comportement |
|------|---------------|--------------|
| `Creation` | Admin uniquement | Tous les champs modifiables, génération automatique de mot de passe temporaire, copie dans presse-papiers, `must_change_password = True` |
| `Modification` | Admin uniquement | Login en lecture seule, autres champs modifiables, gestion des états de sécurité |
| `Consultation` | SuperUser | Tous les champs en lecture seule, bouton Enregistrer masqué, actions de maintenance accessibles (reset password, déverrouillage) |

**Actions de maintenance** :
- Reset mot de passe : Admin ou SuperUser via `ChangePassword` en mode `AdminReset`.
- Déverrouillage de compte : Admin ou SuperUser.
- Activation/désactivation : Admin uniquement.

**Recherche et filtrage** :
- Texte : recherche dans login, nom affichage, code utilisateur.
- Rôle : filtre par User, SuperUser, Admin.
- Date dernier login : filtre par date exacte.
- Comptes verrouillés uniquement.
- Bouton Réinitialiser filtres (activé uniquement si filtres actifs).

### Conséquences

- Séparation claire des responsabilités Admin / SuperUser.
- Workflow de création sécurisé avec mot de passe temporaire auto-généré.
- Délégation de la maintenance (déverrouillage, reset password) au SuperUser sans donner accès à la création/modification.
- Login immuable après création (garantit la traçabilité).
- Toutes les actions de sécurité sont journalisées avec catégorie `Security`.
- Validation exhaustive du formulaire (champs obligatoires, cohérence rôles).
- Utilisation de `LASTVAL(seq_sec_utilisateurs)` pour récupérer l'ID généré par la séquence MariaDB.

### Application — Motif « consultation d'abord » des dialogues patients (18/06/2026)

Le principe de modes d'édition distincts est généralisé aux dialogues modaux de la fiche patient (`ContactEdition`, `IntervenantEdition`) sous une forme **allégée**, sans différenciation par rôle :

- **Drapeau booléen** `_consultation` (et non l'enum `ModeUtilisateurEdition`), piloté par un paramètre constructeur `ouvrirEnConsultation As Boolean = False`.
- **Double-clic** sur une ligne de grille → ouverture en **consultation** (lecture seule) ; les boutons **Ajouter** (création) et **Modifier** (de la grille) ouvrent directement en édition.
- Bascule consultation → édition via le bouton **Modifier** interne au dialogue ; sortie via **Fermer** (consultation) ou **Annuler** (édition).
- Verrouillage centralisé par `AppliquerMode()` / `DefinirEtatChamps(editable)` (champs de saisie, boutons annexes `[+]`, `ReadOnlyMode` du commentaire). Les champs *snapshot* d'`IntervenantEdition` restent en lecture seule permanente (trace pilotée par la sélection du thérapeute).

Ce motif reste cohérent avec la décision : la consultation par défaut sur double-clic et l'enregistrement explicite après passage en édition s'appliquent désormais aussi aux entités liées du patient.

### Application — Harmonisation du contexte UI des dialogues modaux (01/07/2026)

Les dialogues modaux ouverts en contexte (`ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`, `UtilisateurEdition`) sont harmonisés sur le contexte partagé `UserControlContext` fourni par `Home` :

- **Infobulles et statut via `_context`** : les modales implémentent `IContextAwareForm` ; le statut (`SetStatus`) est routé vers la barre partagée de `Home` (messages affichés **toujours au même endroit**) et les infobulles via `SetToolTip`. Un repli sur le `ToolTip` local est conservé si aucun contexte n'est injecté (robustesse).
- **`ErrorProvider` conservé local** : contrairement aux UserControls hébergés dans `Home`, une Form modale `ShowDialog` garde son `ErrorProvider` local. Son `ContainerControl` est la modale elle-même : un provider partagé positionnerait les icônes d'erreur sur `Home` en arrière-plan. C'est une exception **assumée** à la règle « pas de composants locaux ».
- **Timing d'injection** : l'initialisation des infobulles est déplacée dans le handler `Load` (le contexte n'est injecté qu'après le constructeur, via `SetContext` appelé avant `ShowDialog`).

---

## ADR-11 - DialogChoix : remplacement systématique des MessageBox

**Date :** juin 2026  
**Statut :** Actée — présentation refaite en V2.0.0 le 14/10/2026 (UI-7 : kit Althéa, icônes dessinées `AltheaIconeEtat`, parties structurées, boutons qui disent l'action) ; la décision est inchangée, les appels existants aussi.

### Contexte

Les `MessageBox` standards de Windows Forms ne respectent pas la charte graphique d'Althéa et ne permettent pas une personnalisation suffisante (icônes animées, thématisation). De plus, leur utilisation dispersée dans le code rend difficile toute évolution future de l'expérience utilisateur.

### Options envisagées

| Option | Description |
|--------|-------------|
| **DialogChoix personnalisé** | Composant réutilisable respectant UITheme, icônes GIF, configuration flexible |
| MessageBox standard | Simple mais limité, esthétique Windows générique |
| Bibliothèque tierce (ex : MetroFramework) | Riche mais dépendance externe + overhead |

### Décision

**DialogChoix** (`UI/Forms/Communs/DialogChoix.vb`) remplace systématiquement tous les `MessageBox.Show` de l'application.

**Caractéristiques** :
- Enum `TypeDialogue` : Information, Warning, Error, Question, Success, Loading, Processing.
- Configuration flexible : 1 à 3 boutons via méthodes surchargées `SetBoutons()`.
- Mapping DialogResult fixe : Bouton1=Yes, Bouton2=No, Bouton3=Cancel.
- Méthodes statiques simplifiées : `Information()`, `Erreur()`, `Avertissement()`, `Succes()`, `Confirmer()`, `Question()`.
- Thématisation via `UITheme` (couleurs, polices).
- Support icônes GIF animées chargées depuis `My.Resources`.
- Taille adaptative selon le contenu (min 200px, max 600px).
- Centrage parent automatique (`CenterParent`).

**Remplacement systématique** :
- 34 occurrences remplacées dans 6 modules UI :
  - `UC_AdminHome.vb` : 3
  - `UC_Parametres.vb` : 3
  - `Home.vb` : 4
  - `ElevationAcces.vb` : 1
  - `UC_Utilisateurs.vb` : 4
  - `UtilisateurEdition.vb` : 19

**Exception** : `MessageBox` toléré uniquement pour les erreurs critiques système (démarrage, avant chargement des ressources).

### Conséquences

- Cohérence visuelle complète avec la charte graphique Althéa.
- Expérience utilisateur homogène dans toute l'application.
- Maintenance facilitée : un seul point de modification pour l'évolution des dialogues.
- Support des GIF animés pour les opérations longues (Loading, Processing).
- Les confirmations sont maintenant cohérentes (Yes/No via `Confirmer()`).

---

## ADR-12 - Centralisation des icônes d'état via UtilsIcons

**Date :** juin 2026  
**Statut :** Remplacée le 18/10/2026 (UI-9) — les états des grilles sont des pastilles dessinées (`UtilsPastilles`, tons sémantiques, `Reference_UI.md`) ; `IconOK`, `IconOFF`, `IconLock`, `IconNo` et les PNG `Assets/Tech_Ico` sont retirés. `UtilsIcons` ne garde que les icônes dessinées des filtres Patients. Décision conservée ci-dessous pour l'historique.

### Contexte

Les icônes d'état (actif/inactif, verrouillé, etc.) sont utilisées dans plusieurs modules UI (UC_Utilisateurs, UC_Parametres). Sans centralisation, chaque module doit charger et gérer ses propres icônes, ce qui conduit à des duplications de code et des incohérences visuelles.

### Options envisagées

| Option | Description |
|--------|-------------|
| **Module centralisé UtilsIcons** | Chargement unique depuis `My.Resources`, accesseurs statiques |
| Chargement direct depuis `My.Resources` dans chaque UC | Simple mais duplication de code |
| Chargement depuis fichiers à chaque usage | Performances dégradées, risque de fichiers manquants |

### Décision

**UtilsIcons** (`Utils/UtilsIcons.vb`) centralise l'accès aux icônes d'état de l'application.

**Fonctions disponibles** :
- `IconOK(Optional size As Integer = 32)` : État actif/valide (icône verte).
- `IconOFF(Optional size As Integer = 32)` : État inactif/désactivé (icône rouge/grise).
- `IconLock(Optional size As Integer = 32)` : Compte verrouillé (cadenas).
- `IconNo(Optional size As Integer = 32)` : Refus/interdiction.

**Support multi-tailles** : 16x16, 20x20, 26x26, 32x32 (valeur par défaut : 32).

**Chargement centralisé** : depuis `My.Resources`, évitant les chargements multiples depuis les fichiers.

**Utilisation** :
```vb
' Dans DataGridView CellFormatting
If utilisateur.CompteVerrouille Then
	e.Value = UtilsIcons.IconLock(32)
ElseIf utilisateur.Actif Then
	e.Value = UtilsIcons.IconOK(32)
Else
	e.Value = UtilsIcons.IconOFF(32)
End If
```

### Conséquences

- Évite les chargements multiples des mêmes icônes.
- Garantit la cohérence visuelle (mêmes icônes dans tous les modules).
- Facilite l'évolution (changement d'icône dans un seul endroit).
- Performance optimisée (chargement unique depuis `My.Resources`).
- Priorité d'affichage claire : Verrouillé > Actif > Inactif.
- Taille standard 32x32 pour les DataGridView (cohérence visuelle).

---

## ADR-13 - Intégration du module Documents V1 depuis POC

**Date :** mai 2026 — **actée le 08/06/2026**  
**Statut :** Actée
### Contexte

Un POC documentaire existe déjà (Drive/Docs, génération de documents).  
Le besoin V1 est d'intégrer un périmètre documentaire minimal et robuste dans le cœur applicatif sans casser les principes d'architecture existants.

### Décision

Périmètre V1 **acté** (cf. `Plan_Conception_Metier_Althea.md` §8, D-Q4/D-Q7) :

- **Stockage 100 % fichiers, jamais en base** : la DB ne stocke que le **nom de fichier** ; le **chemin est déterministe**, reconstruit depuis *(nature du document, `id_patient`, `id_dossier`)* + racine paramétrée (`tec_parametres`). Voir **ADR-16**.
- **Deux flux d'édition de premier plan** : **Flow 1 — Word local** (synchro Drive dès la sauvegarde + export PDF) **et Flow 2 — Google Docs** (mobilité, l'appli n'étant pas encore web). Autres applications : hors V1.
- **PDF = dérivé reconstructible** (jamais source), **images métier = données** (jamais remplacées par leur PDF), miniatures **locales uniquement**.
- Documents **niveau patient** (photo d'identité) **et** niveau dossier → `documents.id_patient` + `id_dossier` nullable.

### Points de validation attendus

- Respect strict des couches (`UI` / `Metier` / `Core`)
- Traçabilité et journalisation des opérations documentaires sensibles
- Stratégie de sauvegarde/restauration cohérente avec l'exploitation V1

---

## ADR-14 - Intégration du module Agenda V1 depuis POC

**Date :** mai 2026 — **actée le 08/06/2026**  
**Statut :** Actée

### Décision

Périmètre V1 **acté** (cf. `Plan_Conception_Metier_Althea.md` §6, D-Q8) :

- **Modèle connecté Google = pilier V1** : synchronisation **bidirectionnelle** Calendar (et Drive/Docs côté documents), non optionnelle.
- **Robustesse** : file de synchro + retry + suivi via `statut_sync_google` ; **mode dégradé hors-ligne** avec resynchronisation différée.
- **Reprise assistée** des RDV existants à l'installation : semi-automatique, **validée par l'utilisatrice**, avec liaison à un patient/dossier.
- **Séparation stricte** : données métier dans `Tag`, présentation dans `Content` ; ne jamais faire dépendre la logique de la couleur/du titre d'un événement.
---

## ADR-15 - Composants standard UC_RichTextEditor / UC_RichTextEditorSimple pour notes formatées

**Date :** 06-10 juin 2026  
**Statut :** Actée – Implémentée (V1.0 complète + variante Simple) ; variante Simple remplacée et barre précisée par ADR-42 (19/10/2026)

### Contexte

L'application Althéa nécessite la saisie de nombreux documents formatés dans le cadre professionnel psychologique et graphothérapeutique :

- **Anamnèses** : historique complet du patient avec mise en forme structurée
- **Bilans psychologiques/graphothérapeutiques** : comptes-rendus détaillés avec formatage professionnel
- **Comptes-rendus de consultations** : notes de séances formatées et structurées
- **Plans d'accompagnement** : documents évolutifs partagés avec familles, écoles, confrères
- **Correspondances professionnelles** : courriers aux médecins traitants, établissements scolaires

Ces documents doivent :
- Être **éditables avec formatage riche** (gras, couleurs, listes, retraits...)
- Être **imprimables** avec préservation fidèle du formatage
- Être **exportables en PDF** pour archivage et envoi
- Être **exportables en Word/DOCX** pour collaboration et modification
- Être **recherchables** via full-text SQL au niveau Dossier/Patient
- S'intégrer dans l'architecture UI existante (`IContextAwareUserControl`, thématisation Althéa)

Le `RichTextBox` standard WinForms ne suffit pas : pas de toolbar intégrée, impression limitée, pas d'export natif PDF/Word.

### Options envisagées

| Option | Avantages | Inconvénients |
|--------|-----------|---------------|
| **RichTextBox + Toolbar custom + Syncfusion** | Contrôle natif .NET, toolbar sur mesure, exports professionnels via Syncfusion, architecture cohérente | Développement complet nécessaire, dépendance Syncfusion |
| Composant tiers tout-en-un (DevExpress, Telerik) | Toolbar intégrée, fonctionnalités riches | Coût élevé, dépendance forte, surcharge fonctionnelle |
| WebView2 + éditeur HTML (Quill, TinyMCE) | Rendu moderne, éditeurs matures | Complexité intégration, format HTML vs RTF, latence, dépendance réseau |
| Syncfusion RichTextBoxAdv | Composant complet Syncfusion | Coût, complexité, moins de contrôle architectural |

### Décision

**Création d'un UserControl réutilisable `UC_RichTextEditor`** combinant :
- **RichTextBox natif WinForms** comme contrôle d'édition de base
- **Toolbar custom (30 boutons)** pour formatage complet (caractères, paragraphes, polices, presse-papiers, insertion, outils)
- **Helper centralisé `RichTextEditorHelper`** séparant UI et logique métier
- **Impression Win32 native** via API `EM_FORMATRANGE` préservant 100% du formatage RTF
- **Export PDF via Syncfusion.DocIO + DocToPDFConverter** pour archivage
- **Export Word/DOCX via Syncfusion.DocIO** pour collaboration (intégration POC documentaire)
- **Sauvegarde double format** : RTF (formatage préservé) + TXT (recherche full-text SQL)
- **Intégration contexte UI** : implémente `IContextAwareUserControl`
- **Thématisation Althéa** : couleurs toolbar/boutons/fond selon `UITheme`

### Architecture retenue

#### Séparation UI / Logique métier

**UserControl `UC_RichTextEditor`** (`UI/Controls/Communs/UC_RichTextEditor.vb`) :
- Structure UI : toolbar + RichTextBox
- Handlers d'événements déléguant au helper
- Gestion état visuel boutons (enfoncés/désactivés)
- Implémentation `IContextAwareUserControl`
- Exposition propriétés publiques : `RtfContent`, `TextContent`, `ReadOnlyMode`, `ShowToolbar`

**Module `RichTextEditorHelper`** (`Utils/Helpers/RichTextEditorHelper.vb`) :
- Configuration et thématisation
- Méthodes formatage caractères/paragraphes/polices
- Impression Win32 (API `EM_FORMATRANGE` via interop)
- Export PDF (Syncfusion.DocIO → PDF)
- Export Word/DOCX (Syncfusion.DocIO → DOCX)
- Extraction contenu RTF/TXT
- Insertion date/heure

#### Sauvegarde double format (règle stricte)

Toute table contenant des notes doit avoir **deux champs obligatoires** :

```sql
CREATE TABLE xxx_bilans (
    id_bilan INT PRIMARY KEY,
    -- autres champs métier
    bilan_rtf MEDIUMTEXT,        -- Formatage complet préservé
    bilan_txt MEDIUMTEXT,        -- Texte brut pour recherche full-text
    -- ...
);

-- Index full-text obligatoire
CREATE FULLTEXT INDEX idx_bilan_txt ON xxx_bilans(bilan_txt);
```

Code de sauvegarde type :
```vb
bilan.BilanRtf = ucEditor.RtfContent  ' Formatage
bilan.BilanTxt = ucEditor.TextContent ' Recherche
GestionBilans.Enregistrer(bilan)
```

#### Système d'impression avancé

Utilisation de l'API Windows native **`EM_FORMATRANGE`** via interop pour préserver 100% du formatage RTF (gras, couleurs, alignement, puces, polices, retraits).

Processus :
1. Configuration mise en page : `PageSetupDialog` (format papier, marges, orientation)
2. Dialogue impression : `PrintDialog` Windows natif (sélection imprimante)
3. Pagination automatique avec respect des marges
4. Impression page par page via `EM_FORMATRANGE`

#### Exports professionnels (Syncfusion)

**Export PDF** (archivage, envoi) :
```vb
Dim doc As New WordDocument()
doc.AddSection()
doc.LastSection.AddParagraph().AppendRTF(rtfContent)
Dim converter As New DocToPDFConverter()
Dim pdfDoc As PdfDocument = converter.ConvertToPDF(doc)
pdfDoc.Save(cheminFichier)
```

**Export Word/DOCX** (collaboration, modification) :
```vb
Dim doc As New WordDocument()
doc.AddSection()
doc.LastSection.AddParagraph().AppendRTF(rtfContent)
doc.Save(cheminFichier, FormatType.Docx)
```

Intégration avec **POC système documentaire** :
- Stockage local : `Patients/{id_patient}/{id_dossier}/Documents/`
- Synchronisation Google Drive automatique
- DOCX = source principale (éditable Word local ou Google Docs cloud)
- PDF = dérivé (archivage)

#### Packages Syncfusion requis

```xml
<PackageReference Include="Syncfusion.DocIO.WinForms" Version="33.2.10" />
<PackageReference Include="Syncfusion.DocToPDFConverter.WinForms" Version="33.2.10" />
<PackageReference Include="Syncfusion.Licensing" Version="33.2.10" />
```

Licence **Syncfusion Community** (gratuite < 1M USD revenus/an). *Emplacement de la clé : voir ADR-41 (18/10/2026) — saisie dans Paramètres, chiffrée sur le poste ; elle n'est plus dans `Program.vb`.*

### Conséquences

#### ✅ Positives

- **Contrôle architectural complet** : séparation UI/logique, intégration contexte, thématisation
- **Formatage professionnel** : 30 boutons toolbar, raccourcis clavier, formatage riche complet
- **Impression fidèle** : API Win32 `EM_FORMATRANGE` préserve 100% du formatage RTF
- **Exports professionnels** : PDF (archivage) et DOCX (collaboration) via Syncfusion
- **Recherche performante** : sauvegarde double format RTF+TXT avec index full-text SQL
- **Réutilisabilité** : composant standard pour tous les modules nécessitant des notes
- **Maintenance centralisée** : toute logique métier dans `RichTextEditorHelper`
- **Documentation complète** : 589 lignes doc + 468 lignes historique + 479 lignes tests

#### ⚠️ Points de vigilance

- **Dépendance Syncfusion obligatoire** : licence Community gratuite mais enregistrement requis
- **Règle stricte sauvegarde double format** : vérification obligatoire structure table avant intégration UC_RichTextEditor
- **Vérification pré-intégration** : toute table recevant des notes doit avoir champs `xxx_rtf` + `xxx_txt`
- **Maintenance API Win32** : l'impression via `EM_FORMATRANGE` utilise interop, sensible aux modifications RichTextBox
- **Coût licence Syncfusion** : si revenus > 1M USD/an, migration vers licence commerciale nécessaire

#### 📋 Règles d'utilisation (voir `Docs/Rules/Rules.md` §21)

1. **Interdiction de stocker uniquement RTF** : sans TXT, pas de recherche possible
2. **Interdiction de stocker uniquement TXT** : perte du formatage professionnel
3. **Vérification création tables** : lors de l'ajout de notes dans un écran, toujours vérifier que la table sous-jacente possède les deux champs (`xxx_rtf` + `xxx_txt`)
4. **Cohérence noms champs** : même préfixe (ex. : `anamnese_rtf` / `anamnese_txt`, pas `anamnese_rtf` / `notes_txt`)
5. **Types de champs** : `TEXT` (64KB max) ou `MEDIUMTEXT` (16MB max), **jamais VARCHAR**
6. **Index full-text obligatoire** : créer index full-text sur champ `xxx_txt`

#### 📚 Documentation de référence

- **[UC_RichTextEditor_Documentation.md](../Guide/UC_RichTextEditor_Documentation.md)** : Guide complet (589 lignes)
- **[Historique_Implementation_RichTextEditor.md](../Conception/Historique_Implementation_RichTextEditor.md)** : Journal technique V1.0-V1.6 (468 lignes)
- **[Documentation_technique_UI_Althea.md](../Architecture/Documentation_technique_UI_Althea.md)** : Section UC_RichTextEditor complète
- **[PLAN_TESTS_UC_RICHTEXTEDITOR.md](../Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md)** : Plan de tests exhaustif (479 lignes)
- **[Guide_Licence_Syncfusion.md](../Conception/Guide_Licence_Syncfusion.md)** : Obtention licence Community gratuite

#### Variante allégée : UC_RichTextEditorSimple (ajout 10/06/2026)

Pour les zones de notes courtes embarquées dans d'autres UserControls, une variante allégée a été créée :
- **7 boutons** (Gras, Italique, Souligné, Annuler, Rétablir, Effacer format, Date/Heure)
- Réutilise 100 % `RichTextEditorHelper` (aucune logique dupliquée)
- Sauvegarde double format **identique** (RTF + TXT — règle inchangée)
- `IContextAwareUserControl` **optionnel** (fonctionne sans contexte)
- Taille pilotée entièrement par le parent (Dock/Anchor)
- Pas d'impression, pas d'export PDF/Word

### Traçabilité

| Élément | Fichier |
|---------|---------|
| UserControl complet | `UI/Controls/Communs/UC_RichTextEditor.vb` |
| Designer complet | `UI/Controls/Communs/UC_RichTextEditor.Designer.vb` |
| UserControl simple | `UI/Controls/Communs/UC_RichTextEditorSimple.vb` |
| Helper partagé | `Utils/Helpers/RichTextEditorHelper.vb` |
| Form test | `UI/Forms/Test/TestRichTextEditor.vb` |
| Documentation | `Docs/UC_RichTextEditor_Documentation.md` |
| Historique | `Docs/Historique_Implementation_RichTextEditor.md` |
| Tests | `Docs/Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md` |
| Règles | `Docs/Rules/Rules.md` §21 |
| ADR | `Docs/Rules/ARCHITECTURE_DECISIONS.md` ADR-15 |

---

## ADR-16 - Stockage des fichiers hors base, chemin déterministe

**Date :** 08 juin 2026  
**Statut :** Actée

### Contexte

Les documents et images (photos d'identité, bilans, courriers, pièces externes) doivent être stockés de façon fiable, sauvegardable et synchronisable avec Google Drive, sans alourdir la base.

### Options envisagées

| Option | Description | Inconvénient |
|--------|-------------|--------------|
| Contenu binaire en DB (BLOB) | Tout en base | DB volumineuse, sauvegardes lourdes, sync Drive complexe |
| Chemin absolu en DB | Stocker le chemin complet | Fragile (déplacement de racine = liens cassés) |
| **Fichiers sur disque + nom seul en DB** | Chemin **reconstruit** depuis la nature + IDs | Nécessite un helper de chemins centralisé |

### Décision

**Aucun fichier en base.** Fichiers sur **disque** (+ miroir Google Drive). La DB stocke le **nom de fichier** ; le **chemin est déterministe**, recalculé depuis *(nature, `id_patient`, `id_dossier`)* + racine paramétrée (`tec_parametres`, clé `DOC_RACINE_STOCKAGE`).

- **Nommage** : `{Type}_{id_patient}_{id_dossier}_{yyyyMMdd_HHmmss}[_index].{ext}` ; exception **photo d'identité** = nom fixe (`Identite`, sans timestamp, écrase la précédente).
- **Le nom de fichier n'est jamais un identifiant métier** : l'identité = `id_document` + `google_file_id`.

### Conséquences

- Sauvegardes DB légères ; fichiers sauvegardés séparément.
- Robustesse au déplacement de racine (chemin recalculable).
- Impact schéma : `documents.id_patient` + `id_dossier` nullable (BD-1).

---

## ADR-17 - Renommages de schéma (domaines, thérapeutes) + réseau d'intervenants

**Date :** 08 juin 2026  
**Statut :** Actée

### Contexte

Le vocabulaire initial (`volets`, `medecins`) est trop restrictif. « Volet » prête à confusion ; « médecin » exclut les autres intervenants (logopèdes, kinés, adresseurs…). Le suivi externe (`autres_suivis_patient`) était dénormalisé (texte libre).

### Décision

Réalisé **tôt** (Lot 0), car aucun code métier n'en dépend encore :

- `ref_volets → ref_domaines` (+ `id_volet → id_domaine`, code, libellé, séquence, FK, index). `domaine` n'est pas réservé en MariaDB lorsqu'il est préfixé.
- `medecins → therapeutes` (`id_therapeute`, code `TH`, ajout `profession`). `dossiers.id_medecin_traitant → id_therapeute_traitant`.
- **Nouveau référentiel `ref_roles_intervenant`** (adresseur, médecin traitant, logopède…).
- `autres_suivis_patient` **devient la liaison N-N** `patient ↔ therapeute ↔ ref_roles_intervenant` (rôle porté par la liaison).
- **Tarifs par domaine** : ajout `ref_types_seance.id_domaine`. Un dossier transféré de domaine conserve documents/notes/séances ; tarifs passés **figés**.

### Conséquences

- Migrations **versionnées** (BD-0, BD-6, BD-7) + reprise des données + régénération des diagrammes.
- Vocabulaire officiel projet : **Domaine** et **Thérapeute**.

---

## ADR-18 - Référentiels : UC physique + classe de base `UC_ReferentielBase`

**Date :** 08 juin 2026 — Implémentée 09/06/2026 (Lot 0 complet)  
**Statut :** Actée – Implémentée

### Contexte

Les référentiels simples partagent la même structure (`id/code/libelle/actif/ordre_affichage`). Un composant 100 % générique réduirait la maintenance mais retirerait la **maîtrise du design** de chaque écran, à laquelle la praticienne tient.

### Décision

**Un UserControl physique par référentiel** (design libre, `.Designer.vb` propre à chaque écran), **mais** logique mutualisée dans une **classe de base non visuelle `UC_ReferentielBase`** (hérite de `UserControl`) : chargement, CRUD via `GestionReferentiel`, binding, validation, droits, journalisation.

### Conséquences

- Liberté visuelle totale **sans** dupliquer la logique 9 fois.
- Chaque UC hérite de la base et ne gère que ses spécificités métier (métadonnées + branchement `Gestion<X>`).
- Les noms réels des UCs sont `UC_Domaines`, `UC_LiensPatient`, etc. (pas de préfixe `Ref` dans le nom de fichier).
- `UC_ReferentielBase` expose des **hooks champ supplémentaire** (`ConfigurerChampSupplementaire`, `AfficherChampSupplementaire`, `ViderChampSupplementaire`, `ActiverChampSupplementaire`, `ValiderChampSupplementaire`) pour les référentiels avec champ additionnel (`UC_TypesSeance` → `tarif_defaut`).
- `ReferentielLigne` est le modèle de présentation générique (transporte id/code/libellé/actif/ordre + `Tarif?` optionnel).

### Traçabilité

| Élément | Fichier |
|---------|---------|
| Base héritable | `UI/Controls/Referentiels/UC_ReferentielBase.vb` |
| Hub accueil | `UI/Controls/Referentiels/UC_ReferentielHome.vb` |
| UC_Domaines | `UI/Controls/Referentiels/UC_Domaines.vb` |
| UC_LiensPatient | `UI/Controls/Referentiels/UC_LiensPatient.vb` |
| UC_RolesIntervenant | `UI/Controls/Referentiels/UC_RolesIntervenant.vb` |
| UC_SituationsFamiliales | `UI/Controls/Referentiels/UC_SituationsFamiliales.vb` |
| UC_StatutsDossier | `UI/Controls/Referentiels/UC_StatutsDossier.vb` |
| UC_StatutsSeance | `UI/Controls/Referentiels/UC_StatutsSeance.vb` |
| UC_TypesDocuments | `UI/Controls/Referentiels/UC_TypesDocuments.vb` |
| UC_TypesRendezVous | `UI/Controls/Referentiels/UC_TypesRendezVous.vb` |
| UC_TypesSeance (⭐ tarif) | `UI/Controls/Referentiels/UC_TypesSeance.vb` |
| Modèle générique | `Metier/Referentiels/ReferentielLigne.vb` |
| Services métier | `Metier/Referentiels/Gestion<X>.vb` (×9) |
| Requêtes SQL | `Core/Database/Queries/Query<X>.vb` (×9) |
| ADR | `Docs/Rules/ARCHITECTURE_DECISIONS.md` ADR-18 |

---

## ADR-19 - Anticipation multi-utilisateur (`id_utilisateur` nullable)

**Date :** 08 juin 2026  
**Statut :** Actée

### Contexte

La V1 cible une praticienne **seule**, mais un usage multi-utilisateur / multi-agenda est probable à terme. Une migration tardive (ajout d'attribution sur des données existantes) serait coûteuse.

### Décision

Anticipation **à coût minimal**, sans implémenter le partage en V1 :

- Ajout `id_utilisateur` **nullable** sur `rendez_vous` et `dossiers` (FK `sec_utilisateurs`).
- Prévoir un `google_calendar_id` **par utilisateur** côté configuration.
- Aucun mécanisme de droits par enregistrement / partage en V1.

### Conséquences

- Bascule multi-user future **sans migration douloureuse**.
- Colonnes inertes en V1 (toujours l'utilisateur unique).

---

## ADR-20 - Identification des fichiers patients par `code_patient` (PA000003) et non par `id`

**Date** : 14/06/2026
**Statut** : ✅ Acté

### Contexte

Les fichiers liés à un patient (photo, exports, documents) doivent être rangés dans un dossier déterministe sur le système de fichiers. La question se posait : nommer ce dossier avec l'`id_patient` (entier) ou avec le `code_patient` (ex. `PA000003`).

### Décision

**Les dossiers et chemins physiques utilisent le `code_patient`** (valeur calculée `PA` + identifiant 6 chiffres zéro-paddés), jamais l'`id_patient` nu.

Arborescence : `{PATH_GENERAL}\Documents\{code_patient}\` → ex. `D:\Althea_Data\Documents\PA000003\`

### Justification

- Plus **lisible** pour l'utilisateur qui explore le système de fichiers.
- Reproduit la colonne générée MariaDB (`concat('PA', lpad(id_patient, 6, '0'))`), donc toujours recalculable.
- Cohérent avec la convention du projet (affichage du code patient dans le bandeau, grilles, etc.).
- `CheminsPatientHelper.FormaterCodePatient(idPatient)` fournit le code quand seul l'`id` est disponible (ex. après création avant rechargement).

### Règle associée

La DB ne stocke **jamais de chemin absolu** (ADR-16). Elle stocke le nom de fichier seul (ex. `patients.photo_fichier = 'Identite.jpg'`). Le chemin complet est toujours recalculé depuis (`code_patient`, `nom_fichier`) via `CheminsPatientHelper.GetCheminFichierPatient`.

### Conséquences

- Tout nouveau fichier patient est écrit dans `{PATH_GENERAL}\Documents\{code_patient}\`.
- La photo d'identité est nommée de façon déterministe `Identite.{ext}` (nom fixe, remplacement à l'upload).
- Les exports documentaires utilisent un nom horodaté : `anamnese_{code}_{yyyyMMdd_HHmmss}.{ext}`.
- `CheminsPatientHelper` est le **point d'entrée unique** pour tout calcul de chemin patient.

---

## ADR-21 - Export documentaire délégué par événement dans `UC_RichTextEditor`

**Date** : 14/06/2026
**Statut** : ✅ Acté

### Contexte

`UC_RichTextEditor` est un contrôle réutilisable générique. Ses boutons d'export (PDF/Word) ouvraient jusqu'ici un `SaveFileDialog` directement dans le contrôle. Pour l'anamnèse patient, l'export doit aller dans un dossier déterministe avec un nom horodaté — sans coupler le contrôle à la logique métier patient.

### Décision

**Mécanisme d'export délégué par événements** :

1. Le contrôle expose un événement `ExportRequested(sender, e As ExportRequestedEventArgs)` avant l'export.
2. Le conteneur s'abonne (`Handles`) et renseigne `e.DestinationPath` (chemin calculé côté conteneur).
3. Si `DestinationPath` est fourni → **export direct** vers ce chemin.
4. Si personne n'abonne ou si `DestinationPath` reste vide → **fallback `SaveFileDialog`** (comportement générique inchangé).
5. Après un export contextuel réussi, le contrôle lève `ExportCompleted(sender, e As ExportCompletedEventArgs)` que le conteneur peut utiliser (ouverture du fichier, traçabilité future dans `documents`).

### Classes introduites

| Classe | Rôle |
|---|---|
| `ExportFormat` (enum) | `Pdf` / `Word` |
| `ExportRequestedEventArgs` | `Format`, `NomFichierInitial`, `DestinationPath` (rempli par le conteneur) |
| `ExportCompletedEventArgs` | `Format`, `DestinationPath`, `Success` |

### Application côté `UC_PatientFiche`

- `rteAnamnese_ExportRequested` : calcule `…\Documents\{code_patient}\anamnese_{code}_{yyyyMMdd_HHmmss}.ext`, garantit l'existence du dossier (`AssurerDossierPatient`).
- `rteAnamnese_ExportCompleted` : ouvre le fichier exporté (`Process.Start`, `UseShellExecute=True`).
- Abonnement déclaratif via `Handles` (idiomatique VB.NET WinForms, pas de risque d'abonnement multiple).

### Règle d'extension future

Quand la brique **Documents** sera construite, `rteAnamnese_ExportCompleted` sera le point naturel pour tracer le fichier dans la table `documents` (cf. commentaires `TODO` dans `UC_PatientFiche.vb` et `Docs/Todo/ToDo.md §D1`). Le contrôle restera inchangé.

### Conséquences

- `UC_RichTextEditor` reste **100 % générique** : aucune dépendance à la logique patient.
- Le form `TestRichTextEditor` (jetable) continue de fonctionner avec le fallback dialogue.
- Tout futur conteneur peut implémenter un export contextuel en s'abonnant à `ExportRequested`.

---

## ADR-22 - Dossiers : identité générée par la base, statut Actif forcé et référent au niveau patient

**Date :** 08/07/2026  
**Statut :** Actée

### Contexte

La brique **B3** des dossiers ajoute la saisie et la persistance de l'identité d'un dossier (domaine, date d'ouverture, modalités de paiement), avec création et modification depuis le hub `UC_DossierFiche`.

Trois questions structurantes se sont posées :

- qui génère l'identifiant et le code du dossier
- quel statut à la création
- et faut-il un « référent » au niveau du dossier.

### Options envisagées

| Sujet | Option retenue | Option écartée |
|-------|----------------|----------------|
| Identité (`id_dossier`, `code_dossier`) | **Générés par la base** (`seq_dossiers` + colonne GENERATED `DOxxxxxx`) | Code calculé côté application |
| Statut à la création | **Forcé à « Actif »** par la couche métier | Statut choisi dans l'UI |
| Référent du dossier | **Aucun** au niveau dossier ; réseau/adresseur géré **au niveau patient** (Intervenants) | Référent (`id_therapeute_traitant`) saisi et affiché sur le dossier |
| Modalités de paiement | **Texte libre** (rythme, défaut « À la séance ») | Liste fermée de modes de paiement |

### Décision

1. **L'application ne fournit jamais** `id_dossier` ni `code_dossier` : ils sont produits par MariaDB, l'id généré étant récupéré via `SELECT LASTVAL(seq_dossiers)` sur la même connexion (cohérent avec ADR-20 côté patients).
2. **Le statut initial est forcé à « Actif »** par `GestionDossiers` (résolveur `ResoudreIdStatutActif`, constante `CODE_STATUT_ACTIF = "ACTIF"`), indépendamment de toute valeur fournie par l'appelant. Les transitions de cycle de vie (En pause / Clôturé / Archivé) sont réservées à la brique B6.
3. **La notion de « référent » est retirée de la couche dossier.** Pearl exerçant seule, l'adresseur (association, thérapeute, instituteur, famille…) relève du **réseau du patient** (onglet Intervenants), pas du dossier.
   - La colonne `dossiers.id_therapeute_traitant` reste en base (nullable) mais **n'est plus alimentée ni affichée** par l'UI dossier (label bandeau et colonne de grille supprimés).
4. **Les modalités de paiement sont un texte libre** décrivant le rythme (défaut « À la séance »), et non un mode de paiement figé qui varierait d'une séance à l'autre.

### Conséquences

- `QueryDossiers.UpdateDossier` met à jour `date_modification` **explicitement** (`current_timestamp()`) car la table `dossiers` n'a pas de clause `ON UPDATE`.
- La colonne `id_therapeute_traitant` reste présente dans le schéma et le mapping de lecture, sans usage UI : aucune migration destructive n'est requise, la décision est réversible via un futur ADR.
- Le hub `UC_DossierFiche` gère les modes Consultation / Création / Modification et recharge le bandeau après enregistrement ; la grille dossiers de la fiche patient se rafraîchit au retour.

---

## ADR-23 - Documents exportés uniques par contexte (nom déterministe + upsert)

**Date :** 11/07/2026  
**Statut :** Actée

### Contexte

La brique **B4** (zones cliniques) et la traçabilité documentaire permettent d'exporter des documents depuis l'éditeur riche (`UC_RichTextEditor`) : anamnèse (niveau patient), zones cliniques (niveau dossier).

La première implémentation nommait les fichiers avec un **horodatage** (`anamnese_PA000003_20260711_101500.pdf`) et **insérait systématiquement** une ligne dans `documents`.

À l'usage, ce comportement crée un nouveau fichier et une nouvelle ligne à **chaque** export : réexporter plusieurs fois la même anamnèse (avec ou sans modification) accumule des dizaines de fichiers/lignes, rendant le suivi impossible.

### Options envisagées

| Sujet | Option retenue | Option écartée |
|-------|----------------|----------------|
| Nature du document exporté | **Unique par contexte + type** (1 anamnèse/patient, 1 zone/dossier) | Historique versionné (une copie par export) |
| Nom de fichier | **Déterministe, sans horodatage** → écrasement | Nom horodaté → accumulation |
| Écriture en base | **Upsert** (INSERT puis UPDATE) sur clé `chemin_relatif` | INSERT systématique |

### Décision

1. **Un document exporté est unique** dans l'application pour un contexte donné : il évolue (modifications, compléments) mais ne se duplique jamais. Ce n'est pas un historique versionné.
2. **Nom de fichier déterministe, sans horodatage** : `anamnese_{code_patient}.{ext}` (niveau patient), `suivi_{code_dossier}_{zone}.{ext}` (niveau dossier). Chaque export **écrase** le fichier existant : seule la dernière version est conservée sur disque.
3. **Traçabilité en upsert** : `GestionDocuments.TracerDocument` recherche un document existant par son **`chemin_relatif`** (clé d'unicité naturelle, stable puisque le nom est déterministe). Absent → `INSERT` ; présent → `UPDATE` (source, extension, taille, `date_document`, `date_modification = NOW()`). Une seule ligne par document dans `documents`.

### Conséquences

- `QueryDocuments` fournit `SelectIdDocumentParCheminRelatif` et `UpdateDocument` en complément de `InsertDocument`.
- L'unicité par `chemin_relatif` **inclut l'extension** : un export PDF puis Word du même document cohabitent (deux lignes) tant que le doublement automatique DOCX + PDF n'est pas implémenté. À ce moment-là (Flow POC), on pourra fusionner en une ligne source DOCX + `chemin_pdf_relatif` (colonne déjà prévue au schéma).
- Comportement *best-effort* : une erreur de traçabilité en base n'empêche pas l'écriture ni l'ouverture du fichier exporté.
- Cohérent avec ADR-16 (stockage hors base, chemin déterministe) et ADR-21 (export délégué par événement).

---

## ADR-24 - Règles de tarification et de quota portées par le domaine

**Date :** 19/09/2026
**Statut :** Actée — points 3 et 4 **remplacés par [ADR-27](#adr-27---domaine--convention--organisme-catégories-de-quota-et-plafonds-paramétrables)** (01/10/2026)

> Historique conservé ci-dessous. Le code de prestation porté par la nature, `ref_quotas_inami` et les plafonds soutien/traitement fixes n'existent plus depuis le schéma 2.28.0.

### Contexte

Le précédent modèle utilisait

### Décision

1. Les comportements variables sont portés par `ref_domaines` au moyen de trois attributs distincts : `tarif_convention`, `no_show_facturable` et `soumis_quota`.
2. Les tarifs applicables sont liés aux domaines par `lia_prix_seance_domaines`, avec dates de validité et activation. La résolution est effectuée à la date de la séance ; le montant résolu est ensuite figé dans `seances.tarif_seance`.
3. Les natures autorisées sont rattachées aux domaines via `lia_natures_seance_domaines`. La nature porte, si nécessaire, le code de prestation utile au quota.
4. Les plafonds INAMI sont paramétrés dans `ref_quotas_inami` par domaine et période de validité. Les plafonds soutien/traitement et le seuil d’alerte sont séparés ; le total est calculé.
5. `GestionTarification` et `GestionQuotaInami` sont les points de résolution métier. Les écrans ne déduisent aucune règle à partir d’un code de domaine ou d’un montant.

### Conséquences

- Un nouveau domaine, conventionné ou non, est intégrable par paramétrage et liaisons de référentiel.
- Une révision de tarif ou de quota ne modifie pas rétroactivement une séance passée.
- L’ancien indicateur `est_inami` reste transitoirement présent pour compatibilité ; il ne constitue plus le pivot des nouvelles règles.
- Les natures conventionnées sont nommées avec le préfixe `INAMI -` afin de réduire le risque de sélection erronée.

### Traçabilité

- Migrations `v2.13.0` à `v2.18.0` dans `Docs/Database/Migration/`.
- `Metier/Tarification/GestionTarification.vb`.
- `Metier/Seances/GestionQuotaInami.vb`.
- `Metier/Referentiels/GestionLiaisonsPrixDomaines.vb`, `GestionLiaisonsNaturesDomaines.vb`, `GestionQuotasInami.vb`.

---

## ADR-25 - Paiement contextualisé par la séance et distinction statut / solde

**Date :** 20/09/2026
**Statut :** Actée

### Contexte

Un paiement isolé n’indiquait pas suffisamment la séance réglée. De plus, l’affichage « Soldé » dans une colonne de reste dû pouvait faire confondre le statut d’encaissement du paiement avec le solde réel de la séance, notamment pour les paiements en attente ou annulés.

### Décision

1. La liste des paiements joint la séance, son tarif, sa nature et calcule le reste dû à partir des seuls paiements `PAYE`.
2. En V1, un paiement est affecté à une séance ; une séance peut avoir plusieurs paiements pour couvrir les règlements partiels.
3. La grille affiche la séance au format `jj/mm/aaaa (Nature)` et permet de rechercher par ce contexte.
4. L’affichage de la colonne « Reste dû » applique la priorité suivante : `ANNULE` → « Annulé » ; paiement non encaissé → « Prévision » ; séance soldée après encaissement → « Soldé » ; sinon montant restant.
5. L’éditeur propose les séances non soldées du dossier quand aucune séance n’est imposée par le contexte d’ouverture.

### Conséquences

- Les décisions financières sont lisibles sans ouvrir chaque séance.
- Le statut de paiement et le solde de séance restent deux informations indépendantes et non ambiguës.
- Le reste dû est calculé, jamais stocké, ce qui préserve la cohérence après un règlement partiel ou une annulation.

### Évolution (17/10/2026 — grille des paiements en relevé chronologique)

- Le point 3 devient la colonne « Pour » : `S000043 · ven. 10/10/2026 · Traitement` (recherche sur ce texte).
- Le point 4 devient la pastille « État » (`PaiementListeItem.FamilleEtat` / `EtatAffichage`), qui garde la même priorité :
  - encaissement annulé → « Annulé » (gris ; les dus en attente clos par l'application ne sont plus affichés)
  - non encaissé → « En attente », ou « À régler » si la séance due est passée
  - encaissé → solde de la séance (« Soldée », « Payée d'avance », « Reste 20 € dû »), ou « Avoir · X € » / « Avoir utilisé » si la séance n'est plus due.
  - Le statut du paiement et le solde de la séance restent distincts.

### Traçabilité

- `Core/Database/Queries/QueryPaiements.vb`.
- `Metier/Paiements/PaiementListeItem.vb`, `SeanceNonSoldee.vb`, `GestionPaiements.vb`.
- `UI/Controls/Dossiers/UC_DossierFiche.vb` et `UI/Forms/Dossiers/PaiementEditor.vb`.

---

## ADR-26 - Cohérence séances ↔ paiements : règle unique, traçabilité et décision utilisateur

**Date :** 26/09/2026
**Statut :** Actée

### Contexte

Plusieurs chemins (création, modification, statut, report, encaissement, annulation) ajustaient les dus chacun à leur manière, et des paiements annulés pouvaient gonfler le réglé ou l’avoir.

### Décision

1. Une seule règle, `GestionPaiements.ResynchroniserDuSeance` : dû en attente = max(0, tarif − encaissés PAYE) ; annulé si la séance n’est pas due ou est soldée. Les lignes PAYE ne sont jamais modifiées.
2. Aucun paiement n’est supprimé ; un encaissement s’annule (SuperUser/Admin) avec motif obligatoire, auteur et date.
3. L’avoir est calculé, jamais stocké.
4. Le montant d’une séance ne se modifie que via le cadenas ; incohérence montant/tarif = blocage avec message selon le rôle.
5. Les situations à risque non illégales (quota dépassé, tarif changé sur séance encaissée) donnent un avertissement bien visible et laissent la décision à l’utilisateur.
6. Le report recopie tarif et montant : le tarif dépend du patient, pas de la date.

### Conséquences

- Un seul endroit à examiner en cas d’écart de dû.
- Historique financier intégralement traçable.
- Référence des conséquences de chaque action : [Guide séances et paiements](../Guide/Guide_Seances_Paiements.md), partie 2 (remplace le relevé des conséquences le 17/10/2026).

### Évolution (17/10/2026)

- **Une seule définition de l'avoir** : tout l'encaissé resté sur une séance non due, **séances reportées comprises**, pour le bandeau, les grilles, la grille Patients et la réutilisation (les exclusions `REPORTEE` des requêtes sont retirées). Un encaissement que l'on ne réaffecte pas au report est bien « conservé en avoir », comme l'annonce l'application.
- **Reliquat de réaffectation** : il reste sur la séance d'origine (non due) au lieu d'être accroché à la séance cible, due, où il sortait de tous les totaux.
- **Montants** : tout affichage ou total de paiement part de `montant_affecte` (un paiement réparti a plusieurs liaisons, un avoir utilisé a une liaison diminuée).
- **Trop-perçu d'une séance encore due** (montant baissé sous l'encaissé, décision de Joëlle) : compté en avoir partout et réutilisable ; la réutilisation diminue la dernière affectation encaissée de la séance, plafonnée au trop-perçu, de sorte que la créance de la séance n'est jamais entamée.

### Traçabilité

- `Metier/Paiements/GestionPaiements.vb`, `Metier/Seances/GestionSeances.vb`.
- `UI/Forms/Dossiers/SeanceEditor.vb`, `DialogMotifAnnulation.vb`, `DialogPaiementsSeance.vb`, `DialogUtilisationAvoir.vb` (21/10/2026), `UI/Controls/Dossiers/UC_DossierFiche.vb`.

### Évolution (21/10/2026) — la réaffectation est remplacée par l'avoir

Décision de Joëlle (audit B.2) : « sécuriser et repasser par l'avoir, dont on garde ainsi l'historique ».

- **Plus de réaffectation** : un encaissement resté sur une séance devenue non due (report, annulation, maladie) **reste sur sa séance**, où il est un avoir. Ses liaisons ne sont plus réécrites ; l'excédent n'est plus réparti automatiquement.
- **Proposition aussitôt** : une fois le statut non dû enregistré, `DialogUtilisationAvoir` propose d'utiliser **l'avoir de cette séance seulement** pour une séance **due non soldée du même dossier** (la séance de remplacement présélectionnée après un report)
  - sans séance due, l'avoir est conservé (message).
  - Utiliser = `GestionPaiements.UtiliserAvoirDeSeance` (une transaction ; mécanisme de l'avoir du 27/09/2026) ; le surplus reste en avoir.
- **Un seul mécanisme** pour déplacer de l'argent, une seule trace (« Avoir … utilisé le … pour S… ») ; `ReaffecterPaiementVersSeance`, `RepartirPaiementSurSeances` et leurs classes sont supprimées. Les réaffectations antérieures restent valables (liaisons réparties) et s'affichent comme avant.

---

## ADR-27 - Domaine = convention : organisme, catégories de quota et plafonds paramétrables

**Date :** 30/09/2026 — 01/10/2026
**Statut :** Actée – Implémentée (schéma 2.22.0 à 2.28.0) ; point 8 remplacé et point 9 précisé par ADR-40 (14/10/2026)
**Remplace :** ADR-24 points 3 et 4

### Contexte

Le modèle ADR-24 figeait deux compteurs (soutien / traitement) dans des colonnes de `ref_quotas_inami` et rangeait la nature dans un compteur via `code_prestation_inami`.

Un nouvel organisme ou une convention avec d'autres compteurs exigeait une modification du code et du schéma, contraire à la règle structurante « toute règle métier variable est pilotée par un référentiel ». De plus, une même nature ne pouvait pas compter différemment selon la convention.

### Décision

1. **Un domaine représente une convention** : tarif + organisme + quotas. Un nouvel organisme de remboursement = un nouveau domaine.
2. **Organisme** : référentiel `ref_organismes` ; `ref_domaines.id_organisme` est **facultatif**. Aucun comportement n'est déduit du code de l'organisme (INAMI est une ligne système).
3. **Quotas génériques** : `ref_quotas_inami` est renommée `ref_quotas` (`id_quota`). Une règle porte le domaine, la durée de période glissante (`duree_periode_mois`), le seuil d'alerte et la validité.
4. **Catégories de quota** : `ref_categories_quota`, définies **par domaine**, en nombre libre (ex. Soutien, Traitement). Un même libellé peut exister dans plusieurs domaines (compteurs indépendants).
5. **Plafonds** : `ref_quotas_plafonds`, un plafond par (règle, catégorie). Le total est calculé.
6. **Rangement des natures** : la catégorie consommée est portée par la **liaison** `lia_natures_seance_domaines.id_categorie_quota` (NULL = hors quota), avec clé étrangère composite garantissant que la catégorie appartient au même domaine.
7. **Statuts consommant le quota** : `ref_statuts_seance.consomme_quota` remplace la liste codée en dur (PLANIFIEE, REALISEE).
8. **Historisation** *(remplacé par ADR-40)* : une règle de quota ne se modifie jamais. Ajouter une règle clôture automatiquement la règle ouverte à la veille de son début ; pas de bouton « Corriger ».
9. **Écran unique** : `UC_DomainesHome` regroupe en onglets Général / Tarifs / Catégories & quotas / Natures / Synthèse, avec un bandeau assistant (étapes 1 à 5) ; les catégories se gèrent dans `CategoriesQuotaEditor`.
10. Les anciennes colonnes (`code_prestation_inami`, `ref_natures_seance.id_categorie_quota`, `plafond_soutien`, `plafond_traitement`) sont supprimées (v2.27.0, v2.28.0).

### Conséquences

- Un nouvel organisme, une nouvelle catégorie ou un nouveau plafond s'ajoutent **par paramétrage**, sans recompilation.
- Une même nature peut consommer une catégorie différente selon le domaine du dossier.
- Changer la catégorie d'une nature déjà utilisée modifie le décompte des dossiers existants (avertissement affiché).
- Une catégorie utilisée (liaison ou plafond) ne peut être que désactivée.
- Les noms techniques `GestionQuotaInami`, `GestionQuotasInami`, `QuotaInamiRegle`, `QuotaInamiInfo` sont conservés pour limiter le renommage ; ils traitent désormais tout quota de domaine.

### Évolution (22/10/2026, audit B.6)

- **Un seul calcul du décompte** : `GestionQuotaInami.CalculerQuota` (renouvellement de date à date : la 1re séance après l'échéance ouvre la période suivante, de la durée de la règle en vigueur à son ouverture).
  - L'onglet Séances, l'aperçu du patient et l'alerte de dépassement de la fenêtre Séance l'utilisent
  - l'alerte ajoute la séance projetée **à sa date** et regarde la période qui la contient.
  - Le calcul SQL qui ne connaissait que la première période est supprimé.
- **Règle applicable** : lue d'après le domaine **actuel** du dossier, gardée en mémoire par domaine (vidée à chaque écriture de règle) ; elle l'était par dossier, et un dossier changé de domaine gardait l'ancienne.

### Traçabilité

| Élément | Fichier |
|---|---|
| Migrations | `migration_v2.22.0` (renommage `ref_quotas`) à `migration_v2.28.0` (nettoyage) |
| Écran | `UI/Controls/Domaines/UC_DomainesHome.vb` |
| Dialogue catégories | `UI/Forms/Domaines/CategoriesQuotaEditor.vb` |
| Référentiel organismes | `UI/Controls/Referentiels/UC_Organismes.vb`, `Metier/Referentiels/GestionOrganismes.vb`, `Organisme.vb`, `Core/Database/Queries/QueryOrganismes.vb` |
| Catégories | `Metier/Referentiels/GestionCategoriesQuota.vb`, `CategorieQuota.vb`, `Core/Database/Queries/QueryCategoriesQuota.vb` |
| Règles / plafonds | `Metier/Referentiels/GestionQuotasInami.vb`, `QuotaInamiRegle.vb`, `QuotaPlafond.vb` |
| Décompte | `Metier/Seances/GestionQuotaInami.vb` (V3.1.0, `CalculerQuota` : règle fournie par une fonction, testé par `Tests/Althea.Tests/Metier/GestionQuotaInamiTests.vb`, ADR-58), `QuotaInamiInfo.vb` |
| Guide utilisateur | `Docs/Guide/Guide_Utilisateur_Domaines.md` |

---

## ADR-28 - Protection des lignes système des référentiels (`est_systeme`)

**Date :** 30/09/2026
**Statut :** Actée – Implémentée (schéma 2.21.0)

### Contexte

Certains codes de référentiels sont exploités par l'application (statuts, modes de paiement, types de document, organisme INAMI…). Les changer ou les désactiver depuis l'interface casserait des traitements.

### Décision

- Colonne `est_systeme` sur les référentiels concernés, posée **uniquement par migration**.
- Une ligne système ne peut ni changer de code, ni être désactivée, ni être supprimée ; libellé et ordre restent modifiables.
- Contrôle centralisé dans `UC_ReferentielBase` (via `ReferentielLigne`) ; lecture tolérante de la colonne optionnelle via `UtilsDataReader`.

### Conséquences

- Les référentiels restent paramétrables sans risque de casser un code pivot.
- Un nouveau code système nécessite une migration.

### Évolution (22/10/2026, audit C.1, ADR-50)

- La protection ne dépend plus de l'écran :
  - dans les 13 tables qui portent `est_systeme` (et les usages des textes types), la requête de modification n'agit que si la ligne n'est pas système ou si son code est inchangé et qu'elle reste active (`ref_organismes` : type inchangé aussi)
  - la désactivation exclut les lignes système.
  - Les anciennes protections silencieuses (`IF(est_systeme = 1, …)`, présentes dans 4 tables) sont remplacées par ce refus.
- Aucune ligne concernée → refus explicite (`ReglesReferentiels.ExecuterSurLigneModifiable`, `InvalidOperationException`), affiché tel quel par l'écran.

### Traçabilité

- `Docs/Database/Migration/migration_v2.21.0_lot19_referentiels_est_systeme.sql`
- `UI/Controls/Referentiels/UC_ReferentielBase.vb`, `Metier/Referentiels/ReferentielLigne.vb`, `Utils/Database/UtilsDataReader.vb`
- Évolution : `Metier/Referentiels/ReglesReferentiels.vb`, requêtes `Query*` des tables concernées.

---

## ADR-29 - Fondation Cabinet / praticien, situation juridique et porteur du BCE

**Date :** 06/10/2026
**Statut :** Actée – Implémentée (schémas 2.29.0 à 2.31.0)

### Contexte

Les documents, le cachet et la future facturation nécessitent l’identité légale du cabinet et du praticien. Selon la forme juridique, le numéro BCE appartient au cabinet (indépendant, société) ou à chaque praticien (cabinet de groupe).

### Décision

- Tables `cab_cabinet`, `cab_praticiens`, `cab_praticien_identifiants`, `cab_praticien_qualifications` et référentiel `ref_types_identifiant_pro`. Schéma multi-praticiens ; **V1 = un cabinet, un praticien, un SuperUser**. Pas de lien dossier ↔ praticien tant que la V2 n’est pas cadrée.
- Référentiel `ref_situations_juridiques` avec l’attribut `porteur_bce` (`CABINET` / `PRATICIEN`) ; aucun comportement déduit du code.
- Le BCE est saisi à **un seul endroit** ; l’écran propose le déplacement d’un BCE mal placé et n’agit qu’après confirmation.
- `couleur_agenda` retirée du praticien (v2.30.0) : la couleur d’agenda relèvera du type de rendez-vous ou du domaine.
- Images stockées en fichiers (chemins relatifs), cachet généré à partir des données si aucun cachet n’est importé.
- Habillage des écrans défini dans le Designer (hors `DataGridView`).

### Conséquences

- Ajouter une forme juridique = paramétrage, sans recompilation.
- La V2 multi-praticiens n’exigera que l’interface (liste, nouveau, actif/inactif) et le rattachement dossier ↔ praticien.

### Traçabilité

- `Docs/Database/Migration/migration_v2.29.0_lot27_fondation_cabinet.sql`, `migration_v2.30.0_lot27b_suppression_couleur_praticien.sql`, `migration_v2.31.0_lot27c_situations_juridiques.sql` (+ ROLLBACK)
- `Metier/Cabinet/`, `Metier/Referentiels/SituationJuridique.vb`, `UI/Controls/Administration/UC_Cabinet.vb`, `UI/Controls/Referentiels/UC_SituationsJuridiques.vb`, `Utils/UI/UtilsIdentifiantBelge.vb`

---
---

## Processus de mise à jour ADR

Mettre à jour ce fichier à chaque évolution structurante, idéalement dans la même PR que le changement technique.

### Déclencheurs typiques de mise à jour

- nouveau composant transverse (sécurité, config, startup, navigation, persistance),
- changement d'un choix déjà acté (base de données, stratégie sécurité, découpage couches),
- ajout d'une dépendance structurante,
- arbitrage majeur sur les modules cœur (Patients, Dossiers, Séances, Paiements, Documents, Agenda).

### Règles de maintenance

1. Ne jamais réécrire l'historique d'une décision actée ; créer un nouvel ADR qui la remplace explicitement.
2. Maintenir la table des décisions synchronisée avec les statuts réels.
3. Garder une traçabilité code/doc (fichiers de référence) pour chaque ADR.
4. Vérifier la cohérence avec `Docs/ETAT_DU_PROJET.md` après chaque mise à jour ADR.

---

# Contact

> ***Joëlle De Poorter - Les Artefacts de Manou***
>
> Projet réalisé pour soutenir une pratique de psychologie et de graphologie dans la gestion fiable et évolutive des patients et documents.
>
> - Site web Auteur/Les Artefacts de Manou/Joëlle De Poorter : https://lesartefactsdemanou.be
>
> - Site web Psychologue & Graphologue:  https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu` et `lesartefactsdemanou@gmail.com`
>
> - GitHub privé: Althea    https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea
>
> ---



---

## ADR-30 - Habillage par contrôles dessinés plutôt que par images de fond

**Date :** 10/10/2026
**Statut :** Actée – En cours de déploiement (chantier UI, lots UI-0 à UI-3 livrés, UI-4 en cours)

### Contexte

L'habillage reposait sur des images PNG posées en `BackgroundImage` et étirées (`Stretch`).

- Une même image change d'épaisseur de bord, d'arrondi et d'ombre selon la taille du contrôle
- elle doit être posée contrôle par contrôle
- toute retouche de style oblige à repasser sur chaque écran.

Les images embarquées en base64 alourdissaient de surcroît les fichiers `.resx`.

### Décision

- Boutons, tuiles, panneaux, barres d'actions et titres de section deviennent des **contrôles dessinés par le code**, regroupés dans `UI/Controls/Theme/` (`AltheaBouton`, `AltheaTuile`, `AltheaCarte`, `AltheaBarreActions`, `AltheaTitreSection`, outils `AltheaDessin`).
- Ils se règlent dans le Designer, catégorie « Althéa ». Les couleurs proviennent exclusivement de `UITheme`.
- Les contrôles translucides imposent la **composition de la fenêtre** (`WS_EX_COMPOSITED` sur `Home`), le chargement de la vue masquée dans `NavigationManager` et un dessin du feuillage limité à la zone concernée.
- `UtilsButtons` reste en service : il charge les icônes depuis le `Tag`. Les couleurs qu'il pose sont ignorées par les contrôles du kit, dont le `BackColor` est forcé à `Transparent`.

### Conséquences

- Une couleur, un arrondi ou une ombre se changent à un seul endroit et tous les écrans suivent.
- Les images de `Assets/ui-backgrounds/` et les anciens fonds étirés deviennent inutiles : supprimés au lot UI-8 (17 et 18/10/2026).
- Allègement mesuré sur les deux premiers écrans : `.resx` de 91 → 38 Ko et de 65 → 27 Ko.
- Tout nouvel écran hérite gratuitement du remède au scintillement.

### Alternatives écartées

- **Conserver les images** : coût de maintenance et déformations persistantes.
- **Bibliothèque tierce pour le seul habillage** : dépendance disproportionnée au besoin.

### Traçabilité

- `UI/Controls/Theme/`, `UI/Forms/Test/TestKitAlthea`, `Docs/Todo/Plan_Refonte_UI_Althea.md`, `Docs/Rules/Charte_Graphique.md`.

---

## ADR-31 - Place d'une tuile dans un hub : l'usage métier prime sur l'héritage technique

**Date :** 10/10/2026
**Statut :** Actée – Implémentée

### Contexte

`UC_SituationsJuridiques` hérite de `UC_ReferentielBase` : techniquement, c'est un référentiel, et sa tuile figurait donc dans le hub Référentiels. Fonctionnellement, la situation juridique détermine le **porteur du numéro BCE** (voir ADR-29) : c'est un paramètre de configuration du cabinet, pas une donnée manipulée au quotidien.

### Décision

- La tuile est rattachée au hub **Administration**, section « Cabinet et accès », avec **les mêmes droits que la fiche Cabinet** (Admin et SuperUser).
- Le UserControl reste dans `UI/Controls/Referentiels` et continue d'hériter de `UC_ReferentielBase` : c'est le **rattachement fonctionnel** qui change, pas la nature technique de l'écran.
- Règle générale : **la place d'une tuile dans un hub suit l'usage métier, pas l'héritage technique du UserControl.**

### Conséquences

- Aucun utilisateur ne perd l'accès : la fiche Cabinet était déjà ouverte au SuperUser.
- Le hub Administration est structuré en sections (« Cabinet et accès », « Technique ») pour absorber cette tuile et les ajouts à venir sans déséquilibre visuel.
- Les futurs référentiels de configuration pourront suivre le même chemin sans nouvelle discussion.
- **20/10/2026** : application de la règle — `UC_TypesIdentifiantPro` (types d'identifiant professionnel) rejoint la section « Cabinet et accès » du hub Administration (décision de Joëlle)
  - la tuile Domaines quitte le hub Référentiels (les Domaines s'ouvrent par le menu principal de la Home).
  - À voir avec la gestion documentaire : un éventuel hub Documents (ToDo, D1).

### Traçabilité

- `UI/Controls/Administration/UC_AdminHome.vb` et son Designer, `UI/Controls/Referentiels/UC_ReferentielHome.vb` et son Designer.
- 20/10/2026 : `UI/Controls/Referentiels/UC_TypesIdentifiantPro.vb`.

---

## ADR-32 - Onglets : `TabControl` hérité et dessiné, sans contrôle tiers

**Date :** 10/10/2026
**Statut :** Actée – Implémentée (lot UI-2)

### Contexte

Sept écrans utilisent des onglets :

- six `TabControl` posés dans le Designer (`SeanceEditor`, `UC_DossierFiche`, `UC_PatientFiche`, `UC_Cabinet`, `UC_Parametres`, `UC_DomainesHome`)
- et un créé par le code dans `UC_ReferentielBase` (zone « données liées »).

Le contrôle Windows dessine ses onglets selon le thème du système et encadre le contenu d'un relief 3D ; ni l'un ni l'autre ne suivent la charte Sauge.

Trois voies ont été comparées dans le plan de refonte (§ 4) : un contrôle maison complet (A), un `TabControl` hérité et dessiné par le code (B), le `TabControlAdv` de Syncfusion (C).

### Décision

- Voie **B** : `AltheaOnglets` (`UI/Controls/Theme/`) hérite de `TabControl` et dessine lui-même la bande (`UserPaint`, double tampon, `OwnerDrawFixed`) : onglets au repos sur fond beige arrondi, survol, onglet actif en gras souligné de sauge, trait fin sous la bande à la place du cadre.
- Les `TabPage`, leur contenu et le code existant (`SelectedIndexChanged`, `SelectedTab`…) restent inchangés. Migrer un écran revient à changer le type déclaré et à retirer `Appearance`.
- `TabControl` ignore `BackColor` : le fond est porté par la propriété `CouleurFond`. Sur les écrans posés sur le fond beige, elle vaut **`Transparent`** : le contrôle recopie le fond de son parent (feuillage compris) dans son propre tampon.
- La police n'est pas imposée par le contrôle : elle est héritée du parent, comme pour un `TabControl`, afin de ne pas modifier le texte du contenu des pages.
- Syncfusion `Tools.WinForms` n'est **pas** ajouté au projet.

### Conséquences

- Migration réversible écran par écran : remettre `New TabControl()` suffit.
- La bande d'onglets native, qui repeignait le fond du parent directement à l'écran, faisait scintiller la feuille coupée dans `UC_DossierFiche` (onglet Séances) ; dessinée dans le tampon du contrôle, elle ne scintille plus.
- Sur `UC_Cabinet` et `UC_DomainesHome`, la police gras de l'ancien `TabControl` est reportée sur les `TabPage` pour ne pas modifier leur contenu ; la barre repasse en normal pour que l'onglet actif se distingue.
- Pas de nouvelle dépendance, pas de licence supplémentaire à gérer.

### Alternatives écartées

- **A — contrôle maison complet** : réécriture des 21 `TabPage` en `Panel` et de chaque Designer, pour un gain de rendu nul par rapport à B.
- **C — Syncfusion `TabControlAdv`** : Designers à réécrire (`TabPageAdv`), dépendance supplémentaire ; inutile puisque le prototype B donne le rendu attendu sur tous les écrans.

### Traçabilité

- `UI/Controls/Theme/AltheaOnglets.vb` ; Designers des six écrans cités ; `UI/Controls/Referentiels/UC_ReferentielBase.vb` ; `Docs/Todo/Plan_Refonte_UI_Althea.md` (§ 4, lot UI-2).

---

## ADR-33 - Champs du kit : hériter du contrôle Windows quand c'est possible, composer sinon

**Date :** 10/10/2026
**Statut :** Actée – Implémentée (lot UI-4, bandeau de recherche) ; appliquée le 14/10/2026 au champ de date `AltheaDate` (composé, le `DateTimePicker` ne pouvant pas être repris) et à son calendrier dessiné `AltheaCalendrier` (UI-7)

### Contexte

Le bandeau de recherche validé sur maquette demande des champs arrondis : recherche avec loupe, listes déroulantes à étiquette intégrée, cases à cocher sauge. Ces contrôles étaient prévus au lot UI-7 ; le bandeau en a besoin dès UI-4. Les contrôles Windows correspondants ne savent ni arrondir leurs coins ni afficher d'icône, et ignorent la transparence.

### Décision

- **Hériter du contrôle Windows dès que son dessin peut être repris** :
  - `AltheaListe` (`ComboBox`, champ fermé repeint sur `WM_PAINT` dans un tampon, éléments de la liste dessinés par le kit ou par l'écran via `DessinItemsPersonnalise`)
  - et `AltheaCase` (`CheckBox`, `UserPaint`).
  - Le code des écrans (`Items`, `SelectedIndex`, `SelectedIndexChanged`, `Checked`…) ne change pas : migrer un écran revient à changer le type déclaré, comme pour les onglets (ADR-32).
- **Composer seulement quand l'héritage est impossible** : `AltheaRecherche` (`UserControl`) héberge une `TextBox` sans bordure et relaie ce que les écrans utilisent (`Text`, `TextChanged`, `KeyDown` / `KeyPress` / `KeyUp` avec les mêmes arguments, `Clear`, `SelectAll`, `MaxLength`, `TexteAide`).
- Les trois contrôles sont avancés de UI-7 à UI-4 ; UI-7 les réutilisera pour les fiches et les dialogues. Le `DateTimePicker` reste natif jusqu'à UI-7.
- La recopie du fond du parent (coins arrondis, feuillage) est centralisée dans `AltheaFondParent`, utilisé par les onglets, la liste et la recherche.

### Conséquences

- Une seule implémentation par type de champ, réversible écran par écran.
- Deux pièges consignés : un contrôle dérivé de `ButtonBase` (`CheckBox`, `Button`) doit retirer le style `Opaque`, sinon le tampon partagé garde l'image du dernier contrôle dessiné ; dans un bandeau, une case ancrée à droite prend une largeur fixe (`AutoSize = False`), sinon son ajustement automatique la décale vers ses voisins.
- Une infobulle posée sur `AltheaRecherche` ne s'affiche que sur la loupe et les marges, pas sur la zone de saisie (limite connue du contrôle composé).

### Alternatives écartées

- **Tout composer** (`UserControl` autour de chaque contrôle Windows) : chaque écran aurait dû être adapté (événements, propriétés), sans bénéfice visuel.
- **Attendre UI-7** : le bandeau de recherche serait resté natif dans un écran par ailleurs habillé.

### Traçabilité

- `UI/Controls/Theme/AltheaListe.vb`, `AltheaCase.vb`, `AltheaRecherche.vb`, `AltheaFondParent.vb` ; Designers de `UC_PatientHome`, `UC_Therapeutes`, `UC_Utilisateurs`, `UC_ReferentielBase`, `UC_DomainesHome`.

---

## ADR-34 - Deux niveaux de boutons : barre d'actions de l'écran et barres locales

**Date :** 11/10/2026
**Statut :** Actée – Implémentée (fiches Patient, Dossier et Cabinet ; Domaines au lot UI-6, 14/10/2026)

### Contexte

Une fois toutes les barres d'actions habillées (lot UI-4), les boutons qui agissent sur une seule liste (ajouter un contact, encaisser une séance…) ressemblaient à ceux qui agissent sur l'écran entier (Enregistrer, Annuler, Retour). En recette, Joëlle a constaté que cela pouvait tromper l'utilisateur sur la portée d'un bouton.

### Décision

- **Barre d'actions de l'écran** (`AltheaBarreActions`, en bas, vague de la charte) : boutons pleins (`Sauge`, `Ocre`, `Terre`, `Clair`). Elle agit sur l'écran ou la fiche entière.
- **Barre locale** (`AltheaBarreOutils`) : posée **en haut d'une `AltheaCarte` qui contient aussi la grille** (ou l'éditeur, l'image) sur laquelle elle agit ; nom de la liste facultatif.
  - Ses boutons utilisent les variantes **`Outil`** (sans cadre ni ombre), **`OutilPrincipal`** (fond vert pâle, action principale) et **`OutilDanger`** (texte terre, placé en dernier).
- Choix sur maquette (« Barres locales », version B) : c'est l'emplacement dans la carte, plus encore que le style, qui dit à quoi les boutons appartiennent.

### Conséquences

- Les boutons d'une barre locale ne sont jamais pleins ni posés sur la vague : aucune confusion possible avec la barre de l'écran.
- La recherche d'un onglet reste au-dessus de la carte, la pagination en dessous.
- Deux boutons qui alternent (Modifier / Enregistrer) partagent la même place ; Supprimer est toujours en dernier.
- Les panneaux de liaison construits par le code (`UC_LiaisonsNaturesDomaines`, `UC_LiaisonsPrixDomaines`) suivront au lot UI-6 (fait le 14/10/2026, V2.0.0).

### Alternatives écartées

- **Boutons contour sous la grille** (maquette, version A) : lisible, mais la différence ne tenait qu'au style ; les boutons descendaient avec la grille.
- **Même apparence que la barre de l'écran** : source de confusion constatée.

### Traçabilité

- `UI/Controls/Theme/AltheaBarreOutils.vb`, `AltheaBouton.vb` (V1.1.0) ; Designers de `UC_PatientFiche`, `UC_DossierFiche`, `UC_Cabinet`.

---

## ADR-35 - Dossier de données : seule la racine est paramétrable

**Date :** 12/10/2026
**Statut :** Actée – Implémentée (lot 30, P1, migration v2.37.0)

### Contexte

Le nom du sous-dossier des documents était un paramètre modifiable (`PATH_DOCUMENT`), à côté d'autres sous-dossiers écrits en dur dans les helpers (`Cabinet`, `Utilisateurs`).

Or la base enregistre des chemins qui contiennent ce nom (`documents.chemin_relatif = Documents/PA…/DO…/fichier`) et retrouve les photos en recalculant leur dossier. Modifier le paramètre après la saisie de patients aurait coupé les données en deux dossiers et rendu les photos introuvables.

### Décision

- **Seule la racine `PATH_GENERAL` est un paramètre** : on doit pouvoir déplacer tout le dossier de données (autre disque, autre machine) ; les chemins enregistrés lui sont relatifs.
- **Les sous-dossiers sont la structure du programme** : constantes réunies dans un seul module, `ArborescenceDonnees` (`Documents`, `Cabinet/Praticien_{id}`, `Utilisateurs/Utilisateur_{id}`), qui documente l'arborescence complète. Tout nouveau sous-dossier (ex. `Domaines/`) s'y ajoute.
- `PATH_DOCUMENT` est supprimé, avec `PATH_DOSSIER`, `PATH_LOG` et `PATH_BACKUP`, jamais utilisés (accord du 12/10/2026, base de développement).

### Conséquences

- Plus aucun moyen, depuis l'interface, de rendre des fichiers introuvables en renommant un sous-dossier.
- La règle « pas de valeurs codées en dur » vise ce qui peut changer ; ces noms ne le doivent jamais, une constante centralisée est donc la bonne forme.
- Le dossier des sauvegardes est un réglage distinct, hors du dossier de données (lot 30, P2).

### Alternatives écartées

- **Sous-dossiers en paramètres verrouillés (cadenas)** : lectures en base sans bénéfice, puisque la valeur ne peut pas changer.
- **Garder `PATH_DOCUMENT` modifiable** : risque décrit ci-dessus.

### Traçabilité

- `Utils/Helpers/ArborescenceDonnees.vb`, `CheminsPatientHelper.vb`, `CheminsDossierHelper.vb`, `CheminsCabinetHelper.vb`, `CheminsUtilisateurHelper.vb` ; `migration_v2.37.0_parametres_enrichis.sql`.
- 20/10/2026 : `Domaines/Domaine_{id}/` (logo des domaines, `CheminsDomaineHelper.vb`, migration v2.45.0).

---

## ADR-36 - Paramètres applicatifs : une valeur, des bornes, une valeur de secours

**Date :** 12/10/2026
**Statut :** Actée – Implémentée (lot 30, P1)

### Contexte

Plusieurs valeurs du cabinet étaient écrites dans le code (durée de séance, grille des heures, seuil de verrouillage, conservation des journaux, encre du cachet). En les passant en paramètres, deux questions se posaient : faut-il des dates de validité, et que faire si la base ne répond pas ?

### Décision

- **Une seule valeur par paramètre**, remplacée à la modification ; **pas de dates de validité**.
  - Ces paramètres ne servent qu'aux opérations à venir (une heure de fin proposée, un seuil, une purge) : chaque séance enregistre ses propres heures et sa durée.
  - Les valeurs qui recalculent le passé (tarifs, quotas, conventions) restent datées dans les référentiels.
- Après une modification, l'écran Paramètres **rappelle la portée** : la valeur s'applique désormais, les données enregistrées ne changent pas (description du paramètre à l'appui).
- **Un paramètre naît par migration** (clé, libellé, groupe, type, bornes, description, activation, ouverture au SuperUser).
  - L'écran ne crée plus de paramètre et n'en modifie que la **valeur** : un paramètre créé à la main n'était lu par aucun code (doublon constaté en recette le 12/10/2026), et changer le type, l'activation ou l'ouverture d'un paramètre lu par le code pouvait le neutraliser ou l'exposer.
  - Le droit du SuperUser est revérifié dans la requête (`GestionParametres.UpdateValeurParametre`).
- **Bornes en base** (`valeur_min` / `valeur_max`), appliquées à la saisie et à la lecture.
- **Lecture typée** (`GestionParametres.GetEntier` / `GetBooleen`) avec une **valeur de secours** centralisée (`ClesParametres`) : base injoignable, paramètre absent, inactif ou invalide → valeur de secours **journalisée**, l'écran reste utilisable.
  - Exception : la racine du dossier de données, sans repli (une erreur claire vaut mieux que des fichiers écrits ailleurs).
- Les règles de cohérence entre paramètres sont appliquées à l'usage (ex. pas de la grille réduit pour rester compatible avec la durée de séance), pas dans l'écran générique.

### Complément du 16/10/2026 (UI-7, migration v2.40.0)

- **Les groupes naissent aussi par migration** : `tec_groupes_parametres` (code, titre, ordre), clé étrangère depuis `tec_parametres.groupe_parametre` ; le titre et l'ordre des cartes de l'écran ne sont pas écrits dans le code. Choix de Joëlle parmi deux options (table en base ou constantes dans le code).
- **L'écran Paramètres tient sur une page** (maquette A retenue) :
  - une carte par groupe, la description de chaque paramètre visible sous sa valeur
  - un seul « Modifier » pour la page, et **les valeurs changées s'enregistrent en une transaction** (`GestionParametres.UpdateValeursParametres`, tout ou rien), qui remplace l'écriture paramètre par paramètre (`UpdateValeurParametre`, retirée).

### Conséquences

- Un paramètre cassé se voit dans les journaux sans bloquer le cabinet.
- Ajouter un paramètre lu par le code : une ligne de migration (avec bornes et groupe existant), une clé et une valeur de secours dans `ClesParametres`, une lecture typée ; l'écran l'affiche sans modification.

### Traçabilité

- `Metier/Parametres/ClesParametres.vb`, `GestionParametres.vb` (`UpdateValeursParametres` depuis le 16/10/2026) ; `UC_Parametres` (V2.0.0 : `AppliquerBornes`, `VerifierDossierChemin`, `AvertirPortee` ; bouton « Nouveau » retiré le 12/10/2026) ; `GestionSeances.CreneauxProposes`.
- Migration v2.40.0 (`tec_groupes_parametres`).

---

## ADR-37 - Sauvegarde : une archive ZIP chiffrée AES-256 contenant la base et le dossier de données

**Date :** 12/10/2026
**Statut :** Actée – Implémentée (lot 30, P2, migration v2.38.0) ; restauration au lot 30, P3

### Contexte

Les données d'Althéa sont dans deux endroits : la base MariaDB et le dossier de données (documents, images). Une sauvegarde utile doit contenir les deux, cohérents entre eux, et protéger des données de santé (RGPD) même si l'archive est copiée sur un disque externe ou ailleurs.

### Décision

- **Une archive par sauvegarde**, `Althea_AAAAMMJJ_HHMMSS.zip`, contenant `base/althea.sql` (dump complet), `donnees/…` (tout le dossier de données) et `manifeste.json` (date, versions de l'application et du schéma, machine, compte, volumes).
- **Format ZIP chiffré WinZip AES-256** (bibliothèque SharpZipLib) : l'archive s'ouvre avec 7-Zip ou WinRAR et le mot de passe, même sans Althéa — secours en cas de panne (l'explorateur Windows, qui ne lit pas l'AES, ne convient pas). Choix de Joëlle le 12/10/2026.
- **Dump par `mariadb-dump`** (l'outil de MariaDB) : séquences, colonnes générées, déclencheurs et routines correctement exportés.
  - Options :
    - `--single-transaction` (instantané cohérent)
    - **`--skip-add-locks`** (indispensable : un `LOCK TABLES` sur une table dont une colonne vaut `DEFAULT nextval(seq_…)` fait échouer la restauration — constaté en test)
    - `--routines --triggers --hex-blob --default-character-set=utf8mb4`
    - sans `--events` (aucun événement planifié dans la base, et l'option exige le droit `EVENT` que le compte de l'application n'a pas — moindre privilège, constaté le 12/10/2026)
    - sans `--databases` (la restauration choisit la base cible).
  - Outil trouvé dans `Program Files\MariaDB…\bin` (version la plus récente) ou via `SAUVEGARDE_DOSSIER_MARIADB`.
- **Aucune donnée en clair sur le disque** : la sortie de `mariadb-dump` est copiée directement dans l'entrée chiffrée. Le mot de passe de la base passe par un fichier d'options temporaire (dossier temporaire du compte, supprimé aussitôt), jamais par la ligne de commande.
- **Mot de passe des archives mémorisé sur le poste**, chiffré par Windows (DPAPI, compte courant), hors base et hors `althea.local.json`
  - propre aux archives et commun au cabinet — pas un mot de passe de connexion, qui n'est conservé qu'en empreinte et ne peut donc pas chiffrer —, défini par l'Admin (12 caractères minimum dont une majuscule, une minuscule et un chiffre, saisi deux fois)
  - à noter sur papier (indispensable pour restaurer ailleurs).
  - Le changer ne rechiffre pas les archives existantes.
- **Hors archive, volontairement** : `%APPDATA%\Althea` (`althea.local.json`, `althea.sauvegarde.json`, journaux). Ces fichiers décrivent le poste, et leurs secrets sont chiffrés DPAPI pour un compte Windows sur un PC : illisibles ailleurs. Ils se ressaisissent sur un nouveau poste (question de Joëlle du 13/10/2026).
- **Dossier des archives** : chemin complet, refusé à l'intérieur du dossier de données (les archives s'y emboîteraient et partiraient avec le disque).
- **Fiabilité** : écriture sous un nom provisoire (`.zip.tmp`) puis renommage ; échec = archive provisoire supprimée ; rotation des N plus récentes (motif exact du nom, aucun autre fichier touché) ; journal `tec_sauvegardes` pour chaque tentative ; rappel à l'ouverture.
- **Jamais d'archive incomplète en silence** :
  - chaque fichier est lu en lecture partagée et le nombre d'octets lus est comparé à sa taille
  - un fichier verrouillé ou qui se lit vide (document ouvert dans Word — constaté le 12/10/2026 : 0 octet lu) fait échouer la sauvegarde avec un message qui nomme le fichier et demande de le fermer.
  - Les fichiers « propriétaire » de Word/Excel (`~$…`), simples verrous temporaires, ne sont pas archivés.
- **Droits** : sauvegarder = SuperUser et Admin ; mot de passe et restauration = Admin.

### Conséquences

- Un poste neuf peut tout récupérer avec l'archive et le mot de passe (P3), ou à la main avec 7-Zip et le client MariaDB.
- Une dépendance ajoutée (SharpZipLib, MIT) ; MariaDB doit être installé sur le poste qui sauvegarde (c'est le cas du poste du cabinet).
- La cohérence base / fichiers suppose qu'on ne modifie pas de documents pendant la sauvegarde (application mono-utilisateur ; l'écran bloque la navigation pendant l'opération).

### Alternatives écartées

- **Fichier chiffré propre à Althéa** (AES de .NET, sans bibliothèque) : illisible sans l'application.
- **Mot de passe saisi à chaque sauvegarde** : plus contraignant, empêche un usage régulier.
- **Bibliothèque de dump en .NET** : prise en charge incertaine des séquences MariaDB.

### Traçabilité

- `Metier/Sauvegardes/GestionSauvegardes.vb`, `StatutSauvegarde.vb`, `ModelesSauvegarde.vb` ; `Core/Configuration/MotDePasseSauvegarde.vb` ; `Core/Database/Queries/QuerySauvegardes.vb` ; `UI/Controls/Administration/UC_Sauvegardes` ; `migration_v2.38.0_sauvegardes.sql`.

---

## ADR-38 - Restauration : depuis l'écran, même version, sauvegarde de sécurité et compte d'administration saisi

**Date :** 13/10/2026
**Statut :** Actée – Implémentée et validée (lot 30, P3, migration v2.39.0, 13/10/2026) ; remise en état automatique non encore testée (test 7.15)

### Contexte

Une archive (ADR-37) doit pouvoir remettre Althéa dans un état antérieur sans manipulation technique, mais l'opération remplace toutes les données de patients : elle doit être sûre, réversible en cas d'échec et traçable.

Le compte MariaDB de l'application n'a volontairement que des droits sur les données (`SELECT, INSERT, UPDATE, DELETE, EXECUTE, SHOW VIEW`) : il ne peut pas recréer des tables. Althéa ne contrôle pas encore la version du schéma au démarrage.

### Décision (choix de Joëlle du 13/10/2026)

- **Restauration depuis l'écran Sauvegardes**, réservée à l'**Admin**, sur une installation existante. Le **poste neuf** (pas de base, pas de compte) relève d'une **procédure manuelle documentée** (`Guide_Utilisateur_Sauvegardes.md`, chapitre 6) en attendant l'assistant d'installation (lot 33).
- **Même version du schéma seulement** : une archive d'une autre version est refusée ; une archive plus ancienne se restaure à la main, migrations rejouées ensuite.
- **Compte d'administration MariaDB saisi** (en général `root`) pour vider et recharger la base, utilisé pour cette seule opération et **jamais conservé** ; le compte de l'application garde ses droits minimaux. Ses droits sont contrôlés avant de commencer (`SHOW GRANTS`).
- **Ordre des opérations** :
  1. contrôles sans rien modifier
  2. **sauvegarde de sécurité** (sans rotation, qui pourrait supprimer l'archive à restaurer)
  3. extraction des documents dans un dossier provisoire voisin
  4. dossier actuel **renommé et conservé** (`…_avant_restauration_<horodatage>`, supprimé par l'utilisateur quand il est sûr)
  5. base **vidée puis rechargée** (cas validé à la main)
  6. dossier provisoire mis en place
  7. paramètres propres au poste rétablis (`PATH_GENERAL`, `SAUVEGARDE_DOSSIER`, `SAUVEGARDE_DOSSIER_MARIADB`)
  8. **redémarrage** (la session ne correspond plus à la base).
- **Échec après avoir touché la base** : rechargement automatique depuis la sauvegarde de sécurité et ancien dossier remis en place ; si cette remise en état échoue aussi, message d'alerte avec la marche à suivre.
- **Journal `tec_restaurations`** (migration v2.39.0) : une ligne écrite **après** le rechargement, dans la base restaurée (une ligne écrite avant serait effacée) ; un échec est écrit dans la base actuelle (intacte ou remise en état). Pas de clé étrangère vers `sec_utilisateurs` : après restauration, les identifiants de comptes sont ceux de l'archive.
- **Pas de donnée en clair** : le dump va de l'entrée chiffrée au client `mariadb` par son entrée standard ; les chemins de l'archive sont contrôlés (aucun fichier hors du dossier provisoire) ; les messages d'erreur du client, qui recopient la requête fautive (donc des données), sont réduits au code d'erreur et au numéro de ligne.

### Conséquences

- Une restauration ratée ne laisse jamais Althéa sans base, sauf double échec (signalé comme tel).
- Après chaque migration, les archives antérieures ne sont plus restaurables depuis l'écran : il faut sauvegarder juste après une mise à jour.
- L'administrateur doit connaître le mot de passe `root` de MariaDB (déjà nécessaire pour appliquer les migrations).
- L'ancien dossier de données reste sur le disque jusqu'à sa suppression manuelle.

### Alternatives écartées

- **Élargir les droits du compte de l'application** (CREATE, DROP…) : si l'application est compromise, elle peut tout effacer.
- **Compte de restauration dédié enregistré sur le poste** : une configuration de plus pour un usage rare.
- **Accepter les archives plus anciennes** avec avertissement : sans migrations automatiques, l'application risquerait de planter sur un schéma ancien.
- **Recharger par-dessus la base sans la vider** : laisserait des objets étrangers à l'archive.

### Traçabilité

- `Metier/Sauvegardes/GestionRestaurations.vb`, `OutilsMariaDB.vb`, `StatutRestauration.vb`, `ModelesRestauration.vb` ; `Core/Database/Queries/QueryRestaurations.vb` ; `DatabaseManager.OpenConnectionAvecCompte` ; `UI/Forms/Sauvegardes/DialogRestauration` ; `migration_v2.39.0_restaurations.sql` ; `Docs/Guide/Guide_Utilisateur_Sauvegardes.md`.

---

## ADR-39 - Journaux sans données personnelles, version de schéma attendue déclarée dans le code

**Date :** 13/10/2026
**Statut :** Actée – Implémentée et validée (écrans Journaux et État du système, 13/10/2026)

### Contexte

L'écran Journaux rend les journaux faciles à lire ; la revue demandée par le planning a montré que des créations et modifications y écrivaient le nom et le prénom des patients, des contacts familiaux et des thérapeutes, et qu'une erreur de doublon MariaDB y recopiait la valeur en cause (un NISS, par exemple).

Ces fichiers sont en clair dans `%APPDATA%\Althea\Logs`.

Par ailleurs, l'application ne savait pas quelle version de base elle attend : impossible de signaler une migration manquante.

### Décision

- **Journaux** : identifiants seulement (`ID=…`, codes générés), jamais de nom, prénom, NISS, coordonnées ni contenu clinique.
  - Messages existants corrigés (`GestionPatients`, `GestionFamilleContacts`, `GestionTherapeutes`)
  - `GestionLog` masque en plus la valeur d'un `Duplicate entry '…'`.
  - Le nom du praticien (professionnel, compte de travail) reste admis.
- **Consultation** des journaux réservée à l'Admin, en lecture seule ; format d'une ligne défini une seule fois (constantes de `GestionLog`).
- **`VersionSchema.Attendue`** (`Core/Database/VersionSchema.vb`) : seule source de la version de base attendue par l'application, mise à jour avec chaque migration ; comparée à `tec_meta_schema` par l'écran État du système. Le blocage au démarrage viendra plus tard.

### Conséquences

- Pour retrouver une personne à partir d'un journal, il faut passer par l'application (ID → fiche) : c'est voulu.
- Une migration oubliée dans `VersionSchema` fait afficher « base plus récente » : la règle est dans Rules.md et dans la procédure de `Database_technique.md`.

### Traçabilité

- `Core/Logging/GestionLog.vb`, `LecteurJournaux.vb`, `ModelesJournaux.vb` ; `UI/Controls/Administration/UC_Journaux`, `UC_EtatSysteme` ; `Metier/Systeme/GestionEtatSysteme.vb`, `ModelesEtatSysteme.vb` ; `Core/Database/VersionSchema.vb`, `Queries/QueryEtatSysteme.vb`.

---

## ADR-40 - Domaines : étapes guidées et règles de quota corrigeables tant qu'elles n'ont pas servi

**Date :** 14/10/2026
**Statut :** Actée – Implémentée (lot UI-6, sous-étapes 6a à 6d)
**Remplace :** ADR-27 point 8 ; précise ADR-27 point 9

### Contexte

En recette de la refonte de l'écran Domaines (UI-6), deux difficultés sont apparues :

- l'écran en onglets ne disait pas ce qu'il restait à faire, ni qu'une partie de l'écran s'enregistre par **Modifier / Enregistrer** et une autre action par action ;
- la règle d'ADR-27 « une règle de quota ne se modifie jamais » obligeait, pour une simple faute de frappe dans une règle jamais utilisée, à multiplier les règles ou à supprimer, avec le risque de laisser une période sans règle. Joëlle a constaté aussi le cas d'un changement saisi en retard (règle qui aurait dû commencer quelques jours plus tôt).

### Décision

1. **Étapes** : l'écran Domaines se parcourt en cinq étapes (`AltheaEtapes`) qui remplacent la bande d'onglets (pages conservées, bande masquée par `AltheaOnglets.MasquerOnglets`).
   - L'état de chaque étape (fait, à compléter, non nécessaire) vient de `GestionDomaines.CalculerManques` (`UC_SyntheseDomaine.CalculerManquesParEtape` jusqu'au 23/10/2026, audit C.2), **seule source** des règles de complétude.
2. **Fiche et listes rattachées** : **Modifier / Enregistrer** ne concerne que l'étape 1 (identité et comportement). Les étapes 2 à 4 se modifient en consultation, par la barre locale de leur carte (ADR-34), avec enregistrement immédiat ; elles ne sont pas accessibles pendant **Modifier**.
3. **Règle de quota « qui a servi »** : au moins une séance du domaine a une date comprise dans sa période de validité. Une telle règle ne peut être **ni modifiée ni supprimée** : seulement clôturée ou désactivée (l'historique des décomptes reste exact).
4. **Règle qui n'a pas servi** : elle peut être **corrigée** (plafonds, durée, alerte, dates) ou **supprimée**.
5. **Suppression d'une règle erronée** : si elle avait clôturé automatiquement la règle précédente, celle-ci est **rouverte** (jusqu'à la fin de la règle supprimée, ou sans fin), dans la **même transaction**.
6. **Date de début dans le passé** : admise, avec un avertissement dans le récapitulatif. Les compteurs n'étant jamais stockés, le décompte se recalcule par date de séance.
7. **Clôture automatique** de la règle ouverte à la veille du début d'une nouvelle règle : inchangée (ADR-27). Depuis le 21/10/2026, l'arrêt et l'ajout forment une seule transaction (`GestionQuotasInami.AjouterRegleEnChaine`, ADR-46).
8. **Périodes sans règle** : détectées (`GestionQuotasInami.PeriodesSansRegle`) et **jamais comblées automatiquement** (décision de Joëlle, 14/10/2026 : déplacer d'office la fin d'une autre règle peut fausser les périodes sans que personne ne le voie).
   - Elles sont rendues très visibles : ligne d'alerte à leur place dans la grille des règles, note en ocre, étape 3 « À compléter » et point d'attention de la synthèse pour une période entre deux règles
   - « Nouvelle règle » propose leurs dates
   - toute action qui en créerait une (nouvelle règle, correction, clôture, suppression) l'annonce avant confirmation.

### Conséquences

- Une erreur de saisie se corrige là où elle a été faite, sans ligne supplémentaire ni suppression risquée.
- Le passage de « corrigeable » à « figée » est automatique : dès la première séance concernée.
- Les contrôles sont dans `GestionQuotasInami` (`ControlerModification`, `ControlerSuppression`) : l'écran ne fait qu'afficher le refus.
- Le contrôle porte sur la règle telle qu'elle est enregistrée : élargir la période d'une règle inutilisée vers des séances existantes est permis (comme une date de début passée) ; la règle est ensuite figée.
- Corriger la date de début d'une règle ne déplace pas la fin de la règle précédente qu'elle avait clôturée (point 8) :
  - un chevauchement est refusé
  - un écart est annoncé dans le récapitulatif, puis signalé comme « période sans règle » jusqu'à ce qu'une règle intermédiaire le couvre.
  - Autre voie : supprimer la règle erronée (la précédente est rouverte) puis la recréer à la bonne date.
- Un arrêt voulu du quota (clôture sans remplacement) laisse une période ouverte : elle reste affichée dans la grille mais ne rend le domaine incomplet que lorsqu'elle contient aujourd'hui.

### Alternatives écartées

- **Règle jamais modifiable** (ADR-27 point 8) : trop rigide pour les fautes de frappe, source de lignes inutiles et de suppressions intempestives.
- **Règle toujours modifiable** : fausserait rétroactivement les décomptes déjà présentés et facturés.
- **Interdire une date de début passée** : empêcherait de rattraper un changement saisi en retard.
- **Déplacer automatiquement la fin de la règle voisine** lors d'une correction : écarté le 14/10/2026, risque d'erreurs de périodes silencieuses ; le trou est montré, l'utilisateur décide.
- **Signal clignotant** : écarté (fatigue visuelle, accessibilité) au profit de signaux fixes répétés à plusieurs endroits.

### Traçabilité

- `Metier/Referentiels/GestionQuotasInami.vb` (`PeriodesSansRegle`, `ControlerModification`, `ModifierRegleInutilisee`, `ControlerSuppression`, `ReglePrecedenteArretee`, `SupprimerRegleErronee`) ; `Core/Database/Queries/QueryQuotasInami.vb` (`SelectCountSeancesSurPeriodeRegle`, `UpdateFinValiditeQuota`).
- `UI/Controls/Domaines/UC_DomainesHome.vb` et son Designer, `UC_SyntheseDomaine.vb` (V2.1.0) ; `UI/Controls/Theme/AltheaEtapes.vb`, `AltheaOnglets.vb` (V1.2.0) ; `UI/Controls/Referentiels/UC_LiaisonsNaturesDomaines.vb`, `UC_LiaisonsPrixDomaines.vb` (V2.0.0).
- `Docs/Guide/Guide_Utilisateur_Domaines.md`, `Docs/Tests/PLAN_TESTS_DOMAINES.md`.

## ADR-41 - Clés de licence des composants tiers : saisies dans l'application, chiffrées sur le poste

**Date :** 18/10/2026
**Statut :** Actée – Implémentée (validée le 18/10/2026)
**Précise :** ADR des exports Syncfusion (emplacement de la clé) ; applique ADR-02 (secrets locaux par DPAPI)

### Contexte

L'audit du lot UI-8 a trouvé la clé de licence Syncfusion (exports PDF et Word de l'éditeur de texte) écrite en clair dans `Program.vb`, avec une ancienne clé en commentaire, et versionnée — contre le guide de licence et la règle du projet : aucun secret dans le dépôt.

### Décision

1. Une clé de licence n'est **jamais** écrite dans le code, la base ou les journaux.
2. Elle est **saisie dans l'application** par l'Admin (Administration > Paramètres, carte « Licences — propre à ce poste ») et rangée sur le poste, **chiffrée DPAPI pour le compte Windows courant** (`%APPDATA%\Althea\althea.syncfusion.json`, `LicenceSyncfusion`), sur le modèle du mot de passe des sauvegardes (ADR-37).
3. Elle est remise à Syncfusion **au démarrage** (`GestionLicences.AppliquerLicenceSyncfusion`, appelée par `AppStartupManager.Run`) et **dès sa saisie**, sans redémarrage.
4. Son absence **ne bloque rien** : l'application démarre, seuls les exports portent l'avertissement de licence ; le démarrage le journalise et l'État du système le signale (« à vérifier »).
5. La clé n'est jamais réaffichée : le champ reste vide, une pastille donne l'état (définie, absente, illisible sur ce compte Windows).

### Conséquences

- La clé se saisit une fois **par poste et par compte Windows** (poste de développement, poste du cabinet).
- Les clés présentes dans l'historique Git sont à considérer comme publiques : en demander une nouvelle chez Syncfusion.
- Un futur composant sous licence suivra le même chemin (`GestionLicences`, une ligne de plus dans la carte « Licences »).

### Alternatives écartées

- **Variable d'environnement** : sans écran, à régler à la main dans Windows sur chaque poste ; peu praticable au cabinet.
- **Fichier de clé en clair à côté du programme** (ignoré par Git) : la clé reste lisible par quiconque accède au poste.
- **Clé en base** (`tec_parametres`) : elle suivrait les sauvegardes et serait lisible par tout compte de l'application ; une licence décrit le poste, pas le cabinet.

### Traçabilité

- `Core/Configuration/LicenceSyncfusion.vb`, `Metier/Parametres/GestionLicences.vb`, `Program.vb` (V1.1.0), `Core/Startup/AppStartupManager.vb` (V1.3.0), `UI/Controls/Administration/UC_Parametres.vb` (V2.1.0), `Metier/Systeme/GestionEtatSysteme.vb` (V1.1.0).
- `Docs/Conception/Guide_Licence_Syncfusion.md`, `Docs/Tests/PLAN_TESTS_UC_PARAMETRES.md` (section 9).

## ADR-42 - Éditeur de texte : un seul contrôle à deux profils, styles de paragraphe et teintes de la charte

**Date :** 19/10/2026
**Statut :** Actée – Implémentée (temps 1), à valider par les tests
**Remplace :** la variante `UC_RichTextEditorSimple` d'ADR-15 ; **précise** ADR-15 (barre, police) et ADR-21 (messages de l'export)

### Contexte

L'audit des éditeurs (19/10/2026) a relevé :

- deux contrôles presque jumeaux (`UC_RichTextEditor`, `UC_RichTextEditorSimple`) qui dupliquaient leur code et leurs appels Win32
- une barre hors kit (bande sauge pleine, glyphes « B » / « S », icônes en image de fond jamais grisées)
- une boucle entre les listes de police / taille et la sélection
- des raccourcis mal absorbés (Ctrl+I insérait une tabulation)
- une mise en page ignorée à l'impression
- des `MessageBox` dans le module technique
- et une barre qui disparaissait en lecture seule (impossible d'imprimer une note en consultation).

Maquette « Éditeurs de texte — refonte » validée par Joëlle le 19/10/2026.

### Décision

1. **Un seul contrôle, deux profils** : `UC_RichTextEditor.Profil` = `Complet` (anamnèse, zones du dossier : barre entière, recherche, ligne d'état) ou `Compact` (alerte, notes de séance, commentaires : barre réduite). `UC_RichTextEditorSimple` est supprimé.
2. **Styles de paragraphe au lieu de la police et de la taille libres** : Titre, Sous-titre, Normal, Citation, Petit, en tailles relatives à la police de l'éditeur. Une note garde la police d'Althéa.
3. **Couleurs limitées à la charte** : six teintes de texte et trois surligneurs pâles (`UITheme`, région « Éditeur de texte ») ; pas de sélecteur de couleur libre.
4. **Barre du kit** : `ToolStrip` dessinée par `AltheaRenduBarre` (allure des barres locales et des boutons Outil) ; icônes PNG 20 px d'un seul style (`Assets/Editor_Ico/*_20_normal.png`, source SVG unique), état désactivé obtenu par opacité, sans fichiers `_hover` / `_disabled`.
5. **Consultation** : le profil Complet garde une barre de consultation (rechercher, copier, aperçu, imprimer, exporter) ; aucun raccourci ne modifie le texte en lecture seule.
6. **Module technique sans dialogue** : `RichTextEditorHelper` journalise et renvoie un résultat ; l'éditeur informe l'utilisateur par `DialogChoix`. Aucun contenu de note ni chemin de fichier (code patient) dans les journaux.
7. **Mise en forme = modification** : toute action de la barre lève `ContentChanged` (le `RichTextBox` ne signale que le texte).

### Conséquences

- Les écrans déclarent `UC_RichTextEditor` avec `Profil = Compact` là où ils utilisaient la variante simple
  - l'API publique est inchangée (`RtfContent`, `TextContent`, `ReadOnlyMode`, `ShowToolbar`, `ChargerContenu`, `ContentChanged`, `ExportRequested` / `ExportCompleted`)
  - `ContentSaved`, jamais levé, est retiré
  - ajouts : `Profil`, `TitreDocument`, `NomAuteur`, `MarquerEnregistre`.
- `ShowToolbar` est désormais respectée en toutes circonstances.
- Un contenu existant mis en forme avec d'autres polices s'affiche tel quel ; seuls les styles appliqués ensuite suivent la règle.
- Temps 2 (ToDo) :
  - correcteur d'orthographe (fait le 20/10/2026, ADR-45)
  - textes types (fait le 19/10/2026, ADR-44)
  - en-tête et pied de page (fait le 19/10/2026 : `EnTeteDocument`, cabinet, praticien, titre, patient ; mention paramétrable `DOCUMENT_MENTION_CONFIDENTIALITE`, date d'édition, page n / N ; impression, aperçu, PDF et Word, v2.41.0)
  - « Ouvrir en grand » du profil Compact (fait le 19/10/2026 : `EditeurTexteAgrandi`, profil Complet sans sorties, Valider reporte le texte dans le champ).

### Alternatives écartées

- **Garder deux contrôles** : double maintenance, comportements qui divergent.
- **Garder police et taille libres** en plus des styles : notes hétérogènes, et la boucle des listes à corriger sans bénéfice pour la praticienne.
- **Composant d'édition tiers** (Syncfusion RichTextBoxAdv, éditeur HTML) : déjà écarté par ADR-15 ; la refonte reste sur le `RichTextBox` natif et le format RTF en base.

### Traçabilité

- `UI/Controls/Communs/UC_RichTextEditor.vb` (V2.0.0), `Utils/Helpers/RichTextEditorHelper.vb` (V2.0.0), `UI/Controls/Theme/AltheaRenduBarre.vb` (V1.0.0), `Utils/UI/UITheme.vb` (V1.5.0), `Assets/Editor_Ico/`.
- `Docs/Guide/UC_RichTextEditor_Documentation.md` (V2.0), `Docs/Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md` (V2.0).

---

## ADR-43 - Modifications non enregistrées : alerte tenue par l'écran, photo des champs, trois choix

**Date :** 19/10/2026
**Statut :** Actée – Implémentée, à valider par les tests
**Précise :** l'usage d'`IUnsavedChangesAware` (Cabinet, Paramètres, Sauvegardes)

### Contexte

Joëlle a constaté qu'on quittait l'onglet Anamnèse sans enregistrer sans être prévenu.

Le relevé a montré que l'anamnèse n'avertissait qu'au bouton Retour, que la fiche patient en modification et les fenêtres Séance, Contact, Intervenant et Thérapeute perdaient leurs modifications sans rien dire, et que seule la fenêtre Zone du dossier prévenait (deux boutons, suivi par l'éditeur).

### Décision

1. **L'écran pose la question, pas le champ** : l'éditeur de texte ne connaît ni l'enregistrement ni ce que « quitter » veut dire ; il reste un champ parmi d'autres. L'alerte couvre **tous** les champs de l'écran.
2. **Photo des champs** (`GardeModifications`) : les valeurs saisissables sont relevées au passage en saisie, puis comparées au départ ; pas d'événement à brancher champ par champ, une valeur remise à l'identique n'est plus une modification.
3. **Trois choix** (`DialogChoix.DemanderAvantDepart`) : Enregistrer (par défaut), Quitter sans enregistrer, Rester (Échap et croix : on ne perd rien).
4. **Fenêtres** : `GardeModifications.SurveillerFenetre(Me, btnEnregistrer, …)` dans le `Load` ; « Enregistrer » déclenche le bouton de la fenêtre (validation et messages habituels).
5. **UserControls** : `IUnsavedChangesAware.PeutQuitter`, appelé par Home avant toute navigation et à la fermeture d'Althéa (et par `ReferentielModalHost` pour un référentiel ouvert en fenêtre) ; une édition locale (anamnèse) se contrôle aussi au changement d'onglet.
6. **Annuler** qui reste sur l'écran : `DialogChoix.ConfirmerAbandon` (Abandonner / Continuer la saisie) s'il y a des modifications, aucune question sinon ; Annuler qui ferme une fenêtre est un départ (trois choix). Demande de Joëlle : des comportements identiques dans toute l'application.
7. **Périmètre** : toutes les saisies (fiches patient et dossier, Domaines, référentiels, Cabinet, Paramètres, fenêtres d'édition) ; pas les dialogues d'action (les fermer, c'est renoncer) ni les panneaux de rattachement à enregistrement immédiat.

### Conséquences

- Une ligne par nouvelle fenêtre de saisie ; un nouveau champ est couvert sans rien faire.
- La photo contient des données saisies : elle reste en mémoire, jamais journalisée.
- Les champs de recherche, les grilles et les zones en lecture seule sont ignorés ; un écran exclut ses onglets de listes à la construction de la garde.

### Alternatives écartées

- **Alerte dans l'éditeur** : incohérente (un commentaire protégé, pas un téléphone) et sans connaissance du contexte.
- **Drapeau « modifié » par événements** : à brancher sur chaque champ, facile à oublier, faux positifs quand une valeur revient à l'identique.
- **Deux boutons** (Quitter sans enregistrer ? Oui / Non) : obligeait à rester puis à chercher Enregistrer.

### Traçabilité

- `Utils/UI/GardeModifications.vb` (V1.0.0)
- `UI/Forms/Communs/DialogChoix.vb` (V2.2.0)
- `UC_PatientFiche` (V1.7.0)
- `UC_DossierFiche` (V1.10.0)
- `UC_DomainesHome` (V1.8.0)
- `UC_ReferentielBase` (V1.6.0)
- `UC_Cabinet` (V1.5.0)
- `UC_Parametres` (V2.2.0)
- `ReferentielModalHost` (V1.3.0)
- `SeanceEditor` (V1.6.0)
- `PaiementEditor` (V1.5.0)
- `ContactEdition` / `IntervenantEdition` (V1.4.0)
- `TherapeuteEdition` (V1.3.0)
- `UtilisateurEdition` (V1.6.0)
- `DossierZoneEditor` (V1.4.0).
- `Docs/Rules/Reference_UI.md` § 5.8, `Docs/Rules/Rules.md`, `Docs/Tests/PLAN_TESTS_KIT_ALTHEA.md` § 8.

---

## ADR-44 - Textes types : référentiel en base, proposés selon l'usage de l'éditeur

**Date :** 19/10/2026
**Statut :** Actée – Implémentée, à valider par les tests
**Complète :** ADR-42 (temps 2 de l'éditeur)

### Contexte

Temps 2 de l'éditeur : pouvoir insérer des phrases ou des trames réutilisables (« Trame de bilan initial », « Séance annulée par le patient »…). Choix de Joëlle : chaque texte est proposé selon son **usage** (liste en base), ou partout.

### Décision

1. **Référentiel en base** `ref_textes_types` (code, libellé, contenu RTF + texte brut, usage facultatif, ordre, activation), géré par `UC_TextesTypes` sur `UC_ReferentielBase` (même mécanique que les autres référentiels) ; le texte s'écrit dans un éditeur au profil Compact du panneau Détail, avec « Ouvrir en grand ».
2. **Usages** `ref_usages_textes_types` : lignes système dont le code est repris par le code (`UsagesTexteType`) ; aucun usage = proposé partout.
   *Complété le 19/10/2026 (v2.43.0, propositions acceptées par Joëlle)* :
   - **plusieurs usages par texte** (`lia_textes_types_usages`, cases à cocher)
   - **tous les éditeurs ont un usage** (alerte ajoutée, commentaires séparés en contact / intervenant / thérapeute) et c'est une règle pour tout nouvel éditeur (Reference_UI § 7.23)
   - écran **Usages des textes types** (libellé, ordre ; pas de création).
3. **L'écran déclare l'usage de l'éditeur** (`UC_RichTextEditor.UsageTextesTypes`, renseigné par le code, jamais par le Designer) ; sans usage, pas de bouton. Le menu est relu à chaque ouverture : un texte ajouté est proposé tout de suite.
4. **Insertion au curseur, mise en forme comprise** (`RichTextEditorHelper.InsererRtf`), texte brut si le RTF est illisible ; c'est une modification ordinaire du champ (annulable, garde des modifications).

### Conséquences

- Nouvel usage : migration (ligne système) + constante + déclaration dans l'écran concerné.
- Pas de suppression depuis l'interface : un texte qui ne sert plus est désactivé.
- Les textes types ne contiennent pas de données de patient : leur début s'affiche en infobulle dans le menu ; le journal n'en écrit jamais le contenu.

### Alternatives écartées

- **Textes proposés partout** sans usage : liste vite trop longue (option proposée à Joëlle, non retenue).
- **Usages en liste fixe dans le code** : contraire à la règle « pas de valeurs métier codées en dur » ; seuls les codes, utilisés par le code, y figurent.
- **Modèles Word** (`modeles_documents`) : documents complets en fichiers, autre besoin.

### Traçabilité

- Migration `migration_v2.42.0_textes_types.sql`
- `QueryTextesTypes`, `GestionTextesTypes`, `TexteType` / `UsageTexteType`, `UsagesTexteType`
- `UC_TextesTypes`, `UC_ReferentielHome` (V1.2.0)
- `UC_RichTextEditor` (V2.3.0), `RichTextEditorHelper` (V2.2.0)
- écrans qui déclarent l'usage (`UC_PatientFiche`, `DossierZoneEditor`, `SeanceEditor`, `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`).

---

## ADR-45 - Correcteur d'orthographe : celui de Windows, dans la langue du clavier

**Date :** 20/10/2026
**Statut :** Actée – Implémentée et testée (20/10/2026) ; complétée le 20/10/2026 (zones de texte simples, point 5)
**Complète :** ADR-42 (temps 2 de l'éditeur)

### Contexte

Correcteur d'orthographe demandé lors de la refonte de l'éditeur. Deux techniques :

- le correcteur de Windows (le contrôle de texte natif sait souligner, Windows 8 et suivants)
- ou Syncfusion SpellCheckerAdv (nouveau paquet, dictionnaire Hunspell français à fournir et à tenir à jour).

Joëlle craignait qu'un Windows français non belge pose problème ; un essai sur son poste a tranché (20/10/2026) : « pacient » souligné, en français.

### Décision

1. **Correcteur de Windows** : soulignement pendant la frappe par le contrôle lui-même (`RichTextEditorHelper.DefinirCorrecteurOrthographe` : options de langue `IMF_SPELLCHECKING` et services de texte TSF) ; rien n'est envoyé hors du poste.
2. **Langue du clavier** : Windows vérifie chaque mot dans la langue qui lui est attachée ; le RTF du contrôle marquant le texte en anglais, le texte est rattaché à la langue du clavier à l'activation et au chargement (`DefinirLangueTexte`), sans compter comme une modification. Le pays de Windows n'importe pas.
3. **Partout, en saisie** : tous les éditeurs (`CorrecteurOrthographe`, vrai par défaut) ; pas de soulignement en consultation. Réglage `EDITEUR_CORRECTEUR_ORTHOGRAPHE` (carte « Éditeur de texte » de Paramètres, migration v2.44.0) pour le couper.
4. **Clic droit** : Windows ne laisse pas enrichir son menu, que le nôtre remplace : sur un mot inconnu, nos lignes « suggestions » (cinq au plus) et « Ajouter au dictionnaire » (dictionnaire de Windows de l'utilisateur, partagé avec les autres applications) viennent de l'API Spell Checking de Windows (`CorrecteurWindows`, COM `ISpellCheckerFactory` / `ISpellChecker`).
   - « Ignorer » a été essayé puis retiré : il ne valait que pour notre vérification, le contrôle soulignait de nouveau le mot retapé sans proposer de correction (relevé par Joëlle, 20/10/2026).
5. **Zones de texte simples** (20/10/2026, recommandation acceptée par Joëlle) : les notes courtes enregistrées en texte — commentaire d'un paiement, motif d'une annulation — ne passent pas à l'éditeur riche (Althéa complète le commentaire d'un paiement en SQL : mentions d'annulation et d'avoir) mais reçoivent le même correcteur, dans `AltheaZoneTexte` (`RichTextBox` en texte seul).
   - Réglage, langue et clic droit sont dans `CorrecteurTexte`, commun aux deux contrôles : un seul comportement.

### Conséquences

- Aucun paquet ni dictionnaire à livrer ; il faut la langue (avec ses outils de saisie) installée dans Windows, ce qui est le cas d'un Windows en français.
- Orthographe seulement, pas de grammaire (« il est venus » n'est pas signalé) — comme Syncfusion.
- Le RTF enregistré porte désormais la langue du clavier (export Word cohérent).

### Alternatives écartées

- **Syncfusion SpellCheckerAdv** : paquet lourd, dictionnaire Hunspell à maintenir, fenêtre de correction à part.
- **Menu natif de Windows** : il remplacerait notre menu (couper, coller sans mise en forme…).

### Traçabilité

- `RichTextEditorHelper` (V2.3.0), `CorrecteurWindows` (V1.1.0), `CorrecteurTexte` (V1.0.0), `UC_RichTextEditor` (V2.4.1), `AltheaZoneTexte` (V1.0.0), `PaiementEditor` (V1.6.0), `DialogMotifAnnulation` (V1.4.0), `ClesParametres` (V1.3.0), migration `migration_v2.44.0_correcteur_orthographe.sql`, `TestRichTextEditor` (V1.2, diagnostic).

---

## ADR-46 - Opérations en plusieurs écritures : une transaction, tout ou rien

**Date :** 21/10/2026
**Statut :** Actée – Implémentée (audit B.2, 21/10/2026 : séance, report, avoir, règle de quota, compte du praticien)
**Complète :** ADR sur `DatabaseManager` (point d'accès unique) ; ADR-26 (règle unique du dû)

### Contexte

L'audit du 18/10/2026 (§ 4.2) relève des opérations faites de plusieurs écritures, chacune sur sa propre connexion :

- création d'une séance puis de son dû (échec du dû avalé)
- enchaînement séance → notes → statut piloté par l'écran
- report
- réaffectation
- règles de quota
- compte du praticien.

Un incident au milieu laisse la base dans un état que l'application ne sait pas réparer (séance sans dû, séance reportée sans remplaçante).

`GestionCabinet.EnregistrerFiche` montrait déjà la bonne forme : une transaction, des écritures privées qui reçoivent `(conn, tx)`.

### Décision

1. **Unité de travail** : `DatabaseManager.ExecuterEnTransaction(travail)` (variantes avec ou sans valeur rendue) ouvre la connexion et la transaction, exécute le travail, valide ; toute exception annule l'ensemble puis remonte. Un échec du Rollback lui-même est journalisé sans masquer l'erreur d'origine.
2. **Écritures unitaires `(conn, tx)`** : privées dans leur module métier (ex. `InsererSeance`, `EcrireNotesSeance`), elles n'ouvrent jamais leur connexion. Une règle partagée par plusieurs modules expose une variante `Friend` qui s'exécute dans la transaction de l'appelant (`GestionPaiements.ResynchroniserDuSeance(idSeance, conn, tx)`).
3. **Une méthode métier par opération de l'utilisatrice** (ex. `GestionSeances.EnregistrerSeance` pour la fenêtre Séance) : l'écran n'enchaîne plus les écritures.
4. **Lectures de référentiel et contrôles avant la transaction** (statuts, droits, état courant) : la transaction reste courte.
5. **Ce qui demande une décision de l'utilisatrice reste hors transaction** (ex. utilisation de l'avoir proposée après l'enregistrement d'une séance ou un report) : c'est une opération distincte, elle-même atomique.
6. **Plus d'échec avalé** dans une opération : il annule tout et s'affiche (refus métier : `InvalidOperationException` au message rédigé pour l'utilisatrice ; incident technique : message générique, détail au journal).

### Conséquences

- Un échec n'enregistre rien : l'utilisatrice peut réessayer sans doublon ni état bancal.
- Les journaux d'une opération annulée peuvent contenir des lignes « réalisé » écrites avant l'annulation ; la ligne d'erreur qui suit fait foi.
- Mise en œuvre opération par opération (lot B.2) : enregistrement d'une séance, report, utilisation de l'avoir (qui remplace la réaffectation, ADR-26 évolution du 21/10/2026), nouvelle règle de quota, compte du praticien — 21/10/2026. Les nouvelles opérations en plusieurs écritures suivent la même forme.
- Un contrôle qui dépend d'une écriture de la même opération se fait dans la transaction, sur sa connexion (ex. chevauchement d'une règle de quota après l'arrêt de la précédente) : vu d'une autre connexion, l'écriture n'existe pas encore.
- Une écriture qui suppose un état lu avant la transaction le vérifie elle-même (`UPDATE … WHERE` statut attendu) : aucune ligne modifiée = refus, tout est annulé (report d'une séance, statut d'un dossier).

### Alternatives écartées

- **`TransactionScope`** (.NET) : transactions ambiantes, implicites et peu lisibles avec MySqlConnector ; une transaction explicite passée en paramètre se lit mieux.
- **Compensation à la main** (défaire ce qui a été fait) : c'était le cas de `ReporterSeance` jusqu'au 21/10/2026 ; elle peut elle-même échouer.

### Traçabilité

- `DatabaseManager` (V1.3.0), `GestionSeances` (V1.7.0 : `EnregistrerSeance`, `CreateSeance`, `UpdateSeance`, `ChangerStatutSeance` ; V1.8.0 : `ReporterSeance`), `QuerySeances` (V1.1.0), `GestionPaiements` (V1.5.0), `SeanceEditor` (V1.7.0), `UC_DossierFiche` (V1.14.2) ; rapport d'audit § 4.2 et § 9.

---

## ADR-47 - Statut d'une séance passée : la base garde le statut saisi, l'affichage dit « À confirmer »

**Date :** 21/10/2026
**Statut :** Actée – Implémentée (audit B.3 ; décision de Joëlle)
**Prépare :** le pointage des rendez-vous passés du module Agenda (`Docs/Poc/Gestion_Calendrier_Althea.md`)

### Contexte

Depuis le 17/09/2026, `GestionSeances.GetSeancesParDossier` faisait passer en base au statut « Réalisée » toutes les séances « Planifiée » dont l'heure était passée (`BasculerPlanifieesEchues`), à chaque ouverture de l'onglet Séances d'un dossier, tandis que l'affichage les présumait « Réalisée » (`StatutEffectifCode`).

L'audit du 18/10/2026 (§ 4.3) relève :

- une **lecture qui écrit**, sans trace ni transaction avec le dû
- une base dont l'état dépend des écrans ouverts (pas de bascule depuis le planning ou la grille Patients)
- surtout, l'information « séance confirmée ou seulement présumée » **détruite**, alors que le futur Agenda doit justement faire pointer l'issue des rendez-vous passés.

### Options envisagées

| Option | Description |
| --- | --- |
| **Calcul pur, « À confirmer »** | Rien n'est écrit à la lecture ; la séance reste « Planifiée » jusqu'au pointage ; l'affichage dit qu'elle est à confirmer. |
| Calcul pur, « Réalisée » présumée | Même chose en base, mais l'écran continue d'afficher « Réalisée » — alors que la séance n'est ni verrouillée ni confirmée. |
| Bascule explicite au démarrage | Une seule écriture, journalisée — mais l'information « confirmée ou présumée » reste perdue. |

### Décision

1. **Aucune écriture à la lecture** : `BasculerPlanifieesEchues` est supprimée ; une séance planifiée dont l'heure est passée reste « Planifiée » en base.
2. **Pastille « À confirmer »** (ocre, ton « à surveiller ») : `SeanceListeItem.EstAConfirmer` (Planifiée et passée), famille `FamilleStatutSeance.AConfirmer`, libellé `LIBELLE_A_CONFIRMER`. Grille et résumé des séances (avec un filtre « À confirmer »), planning du jour, aperçu du patient. « Techniquement strictement réel » (Joëlle).
3. **Pointage** : choisir le statut dans la fenêtre Séance (Réalisée, absence, report…) ; plus tard, l'Agenda proposera un pointage groupé.
4. **Conséquences métier assumées** : facturation et quotas inchangés (Planifiée est un statut dû) ; le **verrou** d'une séance encaissée ne s'applique qu'au statut réel « Réalisée » (règle du 25/09/2026, désormais cohérente avec l'affichage) ; une séance passée non confirmée **peut être reportée** (rendez-vous déplacé saisi après coup).
5. **Historique** : les séances déjà basculées restent « Réalisée » (impossible de distinguer celles qui l'ont été automatiquement) ; aucune migration.

### Conséquences

- La base ne change plus en lisant ; l'état ne dépend plus des écrans ouverts.
- Pearl voit les séances à pointer ; une séance non pointée reste exigible (dû) comme avant.
- Le module Agenda dispose de l'information dont il a besoin (séances passées non pointées).

### Traçabilité

- `GestionSeances` (V1.10.0), `QuerySeances` (V1.2.0), `SeanceListeItem` (V1.5.0), `UtilsPastilles` (V1.7.0), `UC_DossierFiche` (V1.16.0), `UC_ApercuPatient`, `LignePlanningJour` ; rapport d'audit § 4.3 et § 9.

---

## ADR-48 - Solde d'une séance et dossier dominant : une seule définition, dans des vues SQL

**Date :** 21/10/2026
**Statut :** Actée – Implémentée (audit B.4, migration v2.49.0 ; décisions de Joëlle)
**Complète :** ADR-26 (règle unique du dû) ; ADR-47 (séance passée)

### Contexte

L'audit du 18/10/2026 (§ 4.4) relève des règles écrites plusieurs fois, qui divergent :

- le **reste dû** d'une séance était calculé cinq fois (liste des séances, ligne de paiement, grille Patients, règle unique du dû, liste « Nouveau paiement »). Une absence non facturable gardait un dû en attente et était proposée à l'encaissement ; la ligne de paiement d'une séance non due affichait un reste dû ;
- la sous-requête « encaissé d'une séance » était recopiée neuf fois, la formule « séance échue » quatre fois en SQL (et une fois en VB) ;
- le **dossier dominant** avait trois définitions : la grille Patients prenait le statut d'un dossier et le domaine d'un autre ; l'aperçu suivait une troisième règle.

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **Vues SQL** | Une définition, lue par toutes les requêtes ; calculée à chaque lecture, jamais stockée ; servira telle quelle à une future version web | Une migration ; le compte de restauration doit pouvoir recréer des vues |
| Fonctions VB communes | Pas de migration | Les requêtes de liste (grille Patients) ont besoin du calcul **en SQL** pour trier et totaliser : la règle resterait écrite deux fois |
| Colonnes calculées stockées | Lecture directe | Contraire à la règle « calculer plutôt que stocker le dérivable » ; à tenir à jour à chaque écriture |

### Décision

1. **`v_seances_solde`** (une ligne par séance) :
   - `montant_regle` (somme des affectations des paiements `PAYE`)
   - `est_du`
   - `est_facturable` (due, sauf absence non excusée d'un domaine sans facturation des absences)
   - `reste_du` (`max(0, montant − réglé)` si facturable, sinon 0)
   - `est_echue` (fin de la séance passée, à défaut le début ; sans heure, à partir du lendemain)
   - `montant_avoir` (tout le réglé d'une séance non due ; sinon `max(0, réglé − montant)`).
2. **`v_dossier_dominant`** (une ligne par patient qui a un dossier) : un dossier vivant (actif ou en pause) d'abord, puis celui dont une séance est la plus proche d'aujourd'hui, puis le plus récemment ouvert (décision de Joëlle).
3. **Toutes les lectures passent par les vues** : `QuerySeances`, `QueryPaiements` (lignes de paiement, séances non soldées, synthèse de la règle unique), `QueryPatients` (grille Patients : statut et domaine du même dossier, montants), `QueryDossiers.SelectIdDossierDominant`. `SeanceListeItem.ResteDu`, `AvoirSeance` et `EstFacturable` sont lus, plus recalculés.
4. **SQL SECURITY INVOKER** : la vue s'exécute avec les droits du compte qui la lit. Le compte de l'application n'a pas besoin de nouveau droit (il a déjà `SELECT` et `SHOW VIEW`) ; seul le compte d'administration de la restauration doit pouvoir créer des vues.
5. **Ce qui reste en VB** : `SeanceListeItem.EstPassee`, recalculé à l'affichage pour suivre l'heure qui tourne (même règle que `est_echue`) ; les sources réutilisables d'avoir (`SelectAvoirsDisponiblesPatient`), qui raisonnent par affectation et non par séance, suivent la même définition de l'avoir.

### Conséquences

- Une règle de solde se modifie à un seul endroit : une migration qui remplace la vue (`CREATE OR REPLACE`).
- L'absence non facturable ne doit plus rien partout. Les lignes en attente déjà créées s'annulent à la prochaine resynchronisation de leur séance.
- La sauvegarde contient les deux vues (`mariadb-dump` les exporte) ; la restauration vérifie déjà les droits `CREATE VIEW` et `SHOW VIEW` dès que la base contient une vue.
- Les codes `PAYE`, `ACTIF` et `PAUSE` sont écrits dans les vues comme dans les requêtes. **Évolution (22/10/2026, audit B.6, migration v2.53.0)** : l'absence non excusée n'est plus reconnue par le code `ABSENTE` mais par la case `ref_statuts_seance.est_absence` (décision de Joëlle) ; `v_seances_solde` est recréée en conséquence.
- L'heure de référence est celle du serveur MariaDB (`NOW()`, `CURDATE()`), comme le reste du SQL ; l'harmonisation des horloges reste ouverte (§ 4.4).

### Traçabilité

- Migration `migration_v2.49.0_vues_soldes.sql` (+ ROLLBACK), `VersionSchema` V1.0.10
- `QuerySeances` V1.3.0, `QueryPaiements` V1.5.0, `QueryPatients` V1.8.0, `QueryDossiers` V1.3.0, `SeanceListeItem` V1.6.0, `GestionSeances` V1.11.0, `GestionPaiements` V1.7.0, `GestionDossiers` V1.5.0, `GestionApercuPatient` V1.0.1, `UC_DossierFiche` V1.17.1
- rapport d'audit § 4.4 et § 9.

---

## ADR-49 - Paiements : mode relié par identifiant, annulation et utilisation d'avoir tracées en colonnes

**Date :** 21/10/2026
**Statut :** Actée – Implémentée (audit B.5, 21-22/10/2026 : mode par identifiant, v2.50.0 ; annulation en colonnes, v2.51.0 ; liaison des avoirs, v2.52.0)
**Complète :** ADR-26 (règle unique du dû), ADR-28 (lignes système), ADR-48 (solde d'une séance)

### Contexte

L'audit du 18/10/2026 (§ 4.1, § 5.4) relève :

- `paiements.mode_paiement` contenait le **libellé** du mode. Un règlement par avoir n'était reconnu que par le libellé « Avoir » : renommer ce mode (permis) rendait ses règlements annulables. Le contrôle « mode utilisé » comparait le code au libellé et ne trouvait jamais rien.
- L'annulation d'un encaissement et l'utilisation d'un avoir ne sont tracées que par des **mentions** ajoutées au commentaire (« [Annulé le … par … : motif] », « Avoir utilisé : PAI… (S…) 20 € »). Aucune requête ne retrouve sûrement quel encaissement a financé quel règlement par avoir ; annuler une utilisation d'avoir est impossible.
- L'auteur de ces opérations est le **nom affiché** de l'utilisateur, recopié jusque dans le journal.

### Options envisagées

| Sujet | Options | Retenue |
| --- | --- | --- |
| Mode de paiement | Identifiant + clé étrangère ; code du mode | **Identifiant** : le code d'un mode non système reste modifiable (ADR-28) ; l'identifiant ne change jamais. |
| Traçabilité | Colonnes d'annulation + table de liaison des avoirs ; une table de mouvements | **Colonnes + liaison** : une annulation est un état du paiement (une fois au plus) ; une utilisation d'avoir relie un règlement à une ou plusieurs sources, avec un montant chacune. |
| Historique | Le laisser dans le commentaire ; le reprendre par migration | **Le laisser** : lire du texte libre pour remplir des colonnes est fragile ; seules les nouvelles opérations remplissent les colonnes. |

### Décision (décisions de Joëlle, 21/10/2026)

1. **Mode** : `paiements.id_mode_paiement`, clé étrangère vers `ref_modes_paiement` (v2.50.0). Le libellé affiché est celui du référentiel ; un règlement par avoir se reconnaît au drapeau `est_avoir` du mode. Les anciens libellés inconnus du référentiel deviennent des modes inactifs. La colonne texte est conservée, plus lue.
2. **Annulation** : colonnes typées sur `paiements` (`date_annulation`, `id_utilisateur_annulation`, `motif_annulation`, v2.51.0) ; l'auteur est l'**identifiant** de l'utilisateur, son nom est lu à l'affichage. L'annulation (contrôles, écriture, dû des séances) est d'un seul tenant (ADR-46). *(étape 2)*
3. **Utilisation d'un avoir** : table `lia_paiements_avoirs` (v2.52.0) entre le règlement par avoir et chaque affectation source, avec le montant, la date et l'utilisateur, écrite dans la transaction de l'utilisation. L'infobulle d'un règlement par avoir dit d'où vient l'argent. *(étape 3)*
4. **Journal** : identifiants seulement (plus de nom d'utilisateur).
5. **Historique** : les mentions déjà écrites restent dans le commentaire et restent affichées.

### Conséquences

- Renommer ou recoder un mode ne touche plus l'historique ; un mode utilisé ne peut plus être supprimé (la base le refuse), il se désactive.
- Annuler une utilisation d'avoir deviendra possible (la liaison dit exactement quoi rendre à quelle source) : fonctionnalité à décider plus tard.

### Traçabilité

- Étape 1 : migration `migration_v2.50.0_paiements_mode_id.sql` (+ ROLLBACK), `VersionSchema` V1.0.11 ; `QueryPaiements` V1.6.0, `QueryModesPaiement` V1.1.0, `GestionModesPaiement` V1.1.0, `Paiement` V1.1.0, `PaiementListeItem` V1.5.0, `GestionPaiements` V1.8.0, `PaiementEditor` V1.7.0, `UC_DossierFiche` V1.17.2 ; rapport d'audit § 4.1 et § 9.
- Étape 2 : migration `migration_v2.51.0_paiements_annulation.sql` (+ ROLLBACK, qui recopie les annulations dans le commentaire), `VersionSchema` V1.0.12 ; `QueryPaiements` V1.7.0, `GestionPaiements` V1.9.0 (`AnnulerPaiementEncaisse` V2.0.0), `PaiementListeItem` V1.6.0, `DialogMotifAnnulation` V1.5.0, `UC_DossierFiche` V1.17.3.
- Étape 3 :
  - migration `migration_v2.52.0_lia_paiements_avoirs.sql` (+ ROLLBACK, qui recopie les utilisations dans les commentaires), `VersionSchema` V1.0.13
  - `QueryPaiements` V1.8.0, `GestionPaiements` V1.10.0 (`ImputerAvoir` V2.0.0, `AttacherUtilisationsAvoir`), `UtilisationAvoir`, `PaiementListeItem` V1.7.0, `DialogUtilisationAvoir` V2.0.1, `SeanceEditor` V1.8.1, `UC_DossierFiche` V1.17.4.

---

## ADR-50 - Droits côté métier : un ContexteOperation transmis, une garde unique Autorisation

**Date :** 22/10/2026
**Statut :** Actée – Implémentée (audit C.1 : étape 1 socle et opérations déjà protégées ; étape 2 séances, paiements et dossier figé ; étape 3 référentiels)
**Complète :** ADR-46 (une opération = une méthode métier), ADR-49 (auteur enregistré par identifiant)

### Contexte

L'audit du 18/10/2026 (§ 3.1) relève que les droits étaient vérifiés dans les écrans. Le lot A (A.3) a ajouté un paramètre `roleCourant` à quelques méthodes ; le lot B.5 un `idUtilisateur` pour tracer l'auteur.

Chaque méthode écrivait son propre `If roleCourant <> …` et recevait une paire de paramètres différente (rôle seul ; rôle et utilisateur authentifié ; rôle et identifiant).

Une version web devra recevoir « qui fait l'opération » de la requête, pas d'un écran.

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **Un objet `ContexteOperation`** (utilisateur + rôle courant) | Une seule chose à transmettre ; une signature stable si l'on ajoute une information (poste, horloge…) ; prêt pour le web | Toutes les signatures concernées changent une fois |
| Garder `roleCourant` + `idUtilisateur` | Pas de nouveau type | Paires de paramètres hétérogènes ; chaque ajout touche toutes les signatures |

### Décision (décision de Joëlle, 22/10/2026)

1. **`ContexteOperation`** (Metier/Security) : utilisateur authentifié et rôle courant (élévation comprise), immuable. Construit par l'écran : `UserControlContext.ContexteOperation`, à l'instant de l'appel ; jamais conservé.
2. **`Autorisation`** : `Exiger(contexte, rôle, message)` refuse par `UnauthorizedAccessException` (message pour l'utilisatrice) ; `ExigerUtilisateur(contexte)` refuse une opération tracée sans utilisateur identifié ; `Possede(contexte, rôle)` pour un choix d'affichage.
3. Les méthodes métier sensibles reçoivent un `ContexteOperation` et commencent par la garde ; les paires (roleCourant, idUtilisateur / utilisateur) disparaissent.
4. Le tableau rôle × opération est tenu dans `Securite_Utilisateurs.md`.

### Conséquences

- Étape 1 (22/10/2026) : comptes (Admin), compte du praticien, sauvegardes, restauration, licence, annulation d'un encaissement, utilisation d'un avoir — comportement inchangé, garde unifiée.
- Étape 2 (22/10/2026) : **montant libre** d'une séance (différent du tarif attendu) et **séance réalisée et encaissée** réservés au SuperUser par `GestionSeances.EnregistrerSeance` / `AppliquerDateHeureSeance` (le cadenas de la fenêtre Séance était ouvert à tous)
  - un montant enregistré laissé tel quel, sur le même tarif, est accepté (décision de Joëlle).
- Les **règles d'état** ne sont pas des droits :
  - un dossier clôturé (séances et paiements) ou archivé (tout, fiche comprise — décision de Joëlle) refuse l'écriture par une `InvalidOperationException`, vérifiée dans la transaction d'après le statut relu en base (`GestionDossiers.ExigerDossierModifiable`, `ExigerSeanceModifiable`)
  - le texte du refus et celui des boutons inactifs viennent de la même fonction (`RaisonDossierFige`).
- Étape 3 (22/10/2026) : les **82 écritures** des 25 référentiels reçoivent le `ContexteOperation` et commencent par `ReglesReferentiels.ExigerDroitEcriture` (SuperUser, la règle déjà appliquée par les écrans ; `ROLE_ECRITURE` est aussi lu par `UC_ReferentielBase.RoleMinimum`, dont les 20 surcharges identiques sont retirées).
  - Lignes système : la condition est écrite dans les requêtes des 13 tables concernées et un refus explicite remplace la protection silencieuse (ADR-28, évolution).
  - L'ajout « à la volée » par un User reste soumis à l'élévation (décision de Joëlle ; à cadrer, ToDo).
  - 15 écritures inutilisées supprimées (décision de Joëlle).
- Les écritures conditionnelles (« 0 ligne = refus ») reposent sur `UseAffectedRows = False`, rendu explicite dans `DatabaseManager` : une ligne trouvée compte même si ses valeurs ne changent pas.
- Un refus n'est pas journalisé par la garde : l'appelant journalise s'il y a lieu (identifiants seulement).

### Traçabilité

- `Metier/Security/ContexteOperation.vb`, `Metier/Security/Autorisation.vb`, `UI/Context/UserControlContext.vb` (V1.3.0)
- `GestionUtilisateurs` (V1.7.0), `GestionCabinet` (V1.6.0), `GestionSauvegardes` (V1.2.0), `GestionRestaurations` (V1.1.0), `GestionLicences` (V1.1.0), `GestionPaiements` (V1.11.0) et leurs écrans
- `Securite_Utilisateurs.md` (tableau rôle × opération).
- Étape 2 : `GestionSeances` (V1.13.0), `GestionDossiers` (V1.6.0), `GestionPaiements` (V1.12.0), `GestionTarification` (V1.4.0, `CalculerMontantAttendu`), `QueryDossiers` (V1.4.0) ; `SeanceEditor` (V1.9.0), `UC_DossierFiche` (V1.18.0), `PaiementEditor` (V1.8.0), `DossierZoneEditor` (V1.7.0).
- Étape 3 :
  - `Metier/Referentiels/ReglesReferentiels.vb` (nouveau), les 25 `Metier/Referentiels/Gestion*.vb`, 20 `Core/Database/Queries/Query*.vb`, `DatabaseManager` (V1.4.0)
  - `UC_ReferentielBase` et ses écrans, `UC_DomainesHome`, `UC_LiaisonsNaturesDomaines` / `UC_LiaisonsPrixDomaines` (`FournisseurContexte`), `CategoriesQuotaEditor`, `UC_Therapeutes`, `TherapeuteEdition`.

---

## ADR-51 - Règles de saisie et d'action : un valideur métier, lu par l'écran, appliqué par l'écriture

**Date :** 22/10/2026
**Statut :** Actée – Implémentée (audit C.2, terminé le 23/10/2026 : étape 1 séances et paiements ; étapes 2a et 2b fiche patient, contacts, intervenants et thérapeutes ; étape 3 domaines ; étape 4 comptes)
**Complète :** ADR-46 (une opération = une méthode métier), ADR-50 (droits côté métier)

### Contexte

L'audit du 18/10/2026 (§ 5.2, lot C point 2) relève des règles métier écrites dans les écrans :

- validation du patient, d'un domaine, d'un mot de passe
- séance reportable ou encaissable
- statuts proposés
- chevauchement d'horaire.

Les méthodes d'écriture ne les vérifiaient pas :

- `CreatePatient`, `InsertDomaine`, `ChangerMotDePasse` acceptaient une saisie que l'écran aurait refusée
- `CreatePaiement` acceptait n'importe quel montant sur n'importe quelle séance
- `EnregistrerSeance` acceptait « Reportée » (séance reportée sans remplaçante) et un créneau occupé.

Un autre chemin d'écriture (Agenda, version web) aurait contourné toutes ces règles.

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **Valideur métier rendant la liste des erreurs par champ** | L'écran garde son affichage (chaque erreur sur son contrôle, toutes d'un coup) ; l'écriture applique la même règle | Un petit type à introduire (`ErreurSaisie`) |
| Valideur rendant un seul message (comme `ValiderRegle`) | Rien à introduire | L'écran ne signale plus toutes les erreurs d'un coup, ni sur quel champ |

### Décision (décision de Joëlle, 22/10/2026)

1. Une règle de **saisie** se définit une fois, dans le module métier : `Valider…(objet[, contexte lu en base]) As List(Of ErreurSaisie)`, liste vide si l'objet est valide.
2. `ErreurSaisie` porte le **champ** — nom de la propriété de l'objet métier, écrit avec `NameOf` (aucun nom de contrôle d'écran dans le métier) — et le **message** pour l'utilisatrice.
3. L'écran appelle le valideur avant d'enregistrer et place les erreurs sur ses contrôles ; la méthode d'écriture l'appelle aussi, sur les données relues dans sa transaction, et refuse par `ErreurValidationException` (`LeverSiErreurs`), qui hérite d'`InvalidOperationException` : les écrans affichent ce refus comme les autres refus métier.
4. Une règle d'**action** (ce qu'un élément permet : reporter, supprimer, encaisser, statuts proposés) est une fonction du métier, lue par les boutons et appliquée par l'écriture.
5. Une règle bloquante qui dépend des autres données (chevauchement d'horaire, reste dû) est relue **dans la transaction** de l'écriture.
6. Reste à l'écran : ce qui n'est pas une règle — confirmations (doublon possible, date passée, dépassement de quota), aides de saisie (tarifs et natures filtrés par domaine, avec repli), contrôle d'une saisie impossible à transmettre (date vide).

### Conséquences

- Étape 1 (22/10/2026) — séances et paiements :
  - `GestionSeances.StatutPermetReport` / `StatutPermetSuppression` / `StatutPermetEncaissement` / `StatutEstProposable` / `StatutsProposes` / `ChercherChevauchement`
  - `SeanceListeItem.EstReportable` / `EstSupprimable` / `EstEncaissable` / `RaisonNonEncaissable`
  - `GestionPaiements.ValiderEncaissement`.
  - `EnregistrerSeance` refuse un statut non proposable et un chevauchement
  - `ReporterSeance` refuse un créneau occupé pour la nouvelle séance (décision de Joëlle : le chevauchement est une règle du métier)
  - `CreatePaiement` refuse un encaissement invalide sur le reste dû relu et réaligne le dû dans sa transaction.
- Étape 2a (23/10/2026) — fiche patient :
  - `GestionPatients.ValiderPatient` (nom, prénom, téléphone et code postal selon le pays, e-mail, NISS selon son type puis unicité, CT1/CT2), appliqué par `CreatePatient` / `UpdatePatient` qui n'en vérifiaient aucune.
  - Le type d'identifiant, choisi dans la fiche mais non enregistré, est **transmis** au métier (`Patient.TypeIdentifiantNiss`, décision de Joëlle : pas de colonne).
  - Le NISS est **enregistré normalisé** (`UtilsNiss.Normaliser`, migration v2.54.0 ; décision de Joëlle) : deux écritures d'un même numéro contournaient l'unicité.
  - Affichage commun des erreurs : `AffichageErreursSaisie.Signaler` — icône sur chaque champ, fenêtre récapitulative, focus (décision de Joëlle : un refus doit se voir).
- Étape 2b (23/10/2026) — fiches liées au patient :
  - `GestionFamilleContacts.ValiderContact`, `GestionSuivisIntervenants.ValiderSuivi`, `GestionTherapeutes.ValiderTherapeute`, appliqués par leurs `Create…` / `Update…` qui n'en vérifiaient aucune
  - les trois fenêtres affichent les erreurs par `AffichageErreursSaisie`.
  - L'**homonymie d'un thérapeute** n'est plus un refus mais une confirmation de l'écran (point 6 ; décision de Joëlle : les homonymes sont fréquents parmi les thérapeutes).
  - « Intervention terminée » devient une règle du modèle (`SuiviIntervenant.EstTerminee`), lue par la grille.
- Étape 3 (23/10/2026) — domaines :
  - `GestionDomaines.ValiderDomaine` (code, libellé, unicité, quota encore actif), appliqué par `InsertDomaine` / `UpdateDomaine` qui n'en vérifiaient aucune
  - l'écran affiche toutes les erreurs par `AffichageErreursSaisie` (une fenêtre par erreur auparavant).
  - Les règles de **complétude** d'un domaine (`GestionDomaines.CalculerManques`, `ManqueDomaine`) quittent `UC_SyntheseDomaine` pour le métier, sans changement (point 4 : règle lue par l'écran ; elle ne bloque aucune écriture).
- Étape 4 (23/10/2026) — comptes :
  - `PolitiqueMotDePasse` (règle unique ; longueur 10 pour les comptes, 12 pour les archives de sauvegarde), appliquée par `ChangerMotDePasse`, la réinitialisation et `CreateUtilisateur` (le métier acceptait « a ») et lue par `ChangePassword` et `GestionSauvegardes`
  - `GestionUtilisateurs.ValiderCompte` (login d'au moins 3 caractères et unique, nom d'affichage, rôles), lue par la fiche Utilisateur et la création du compte du praticien.
  - Les refus de saisie de `CreateUtilisateur` lèvent `ErreurValidationException` au lieu de rendre -1.
  - Le login n'est jugé qu'à la création (il est immuable ensuite).
- Les contrôles de **format** sans état (NISS, téléphone, code postal, e-mail, IBAN…) restent dans `Utils/UI` (`UtilsNiss`, `UtilsTelephone`…) : ils ne dépendent pas de WinForms et les valideurs métier les appellent. Leur rangement hors de `Utils/UI` est un rangement, pas une règle (lot C.5 ou D).
- Les refus du métier ne se provoquent en général pas depuis les écrans, qui appellent le même valideur avant : ils seront couverts par les tests automatisés (lot D).

### Traçabilité

- `Metier/Validation/ErreurSaisie.vb`, `Metier/Validation/ErreurValidationException.vb` (nouveaux).
- Étape 1 : `GestionSeances` (V1.14.0), `SeanceListeItem` (V1.8.0), `GestionPaiements` (V1.13.0), `QueryPaiements` (V1.9.0, `SelectEncaissementSeance`) ; `SeanceEditor` (V1.10.0), `UC_DossierFiche` (V1.19.0), `PaiementEditor` (V1.9.0).
- Étape 2a : `GestionPatients` (V1.5.0), `Patient` (V1.4.0), `GestionPays` (V1.2.0, `CodePaysParId`), `UtilsNiss` (V1.1.0), `Utils/UI/AffichageErreursSaisie.vb` (nouveau) ; `UC_PatientFiche` (V1.9.0), `UC_PatientHome` (V1.6.0) ; migration v2.54.0.
- Étape 2b : `GestionFamilleContacts` (V1.3.0), `GestionSuivisIntervenants` (V1.3.0), `GestionTherapeutes` (V1.4.0), `SuiviIntervenant` (V1.2.0), `Therapeute` (V1.3.0, `AdresseUneLigne`), `AffichageErreursSaisie` (V1.1.0) ; `ContactEdition` (V1.5.0), `IntervenantEdition` (V1.6.0), `TherapeuteEdition` (V1.5.0), `UC_PatientFiche` (V1.9.1).
- Étape 3 : `GestionDomaines` (V1.3.0), `Metier/Referentiels/ManqueDomaine.vb` (nouveau, `ManqueDomaine` et `EtapeDomaine` déplacés), `GestionQuotasInami` (V1.6.0, `GetQuotasParDomaine`), `AffichageErreursSaisie` (V1.2.0) ; `UC_DomainesHome` (V1.13.0), `UC_SyntheseDomaine` (V2.5.0).
- Étape 4 : `Metier/Security/PolitiqueMotDePasse.vb` (nouveau), `GestionUtilisateurs` (V1.8.0), `GestionSauvegardes` (V1.3.0) ; `ChangePassword` (V1.6.0), `UtilisateurEdition` (V1.9.0), `DialogComptePraticien` (V1.4.0).

---

## ADR-52 - Recherche transversale : un hub, des recherches spécialisées, un résultat et une ouverture communs

**Date :** 23/10/2026
**Statut :** Actée – Implémentée et validée de R1 à R5 (24/10/2026) (R1 recherche par code ; R2 séances d'un jour ou d'une période ; R3 paiements d'un jour ou d'une période ; R4 patients par lieu ; R5 réseau par nom — contact, thérapeute, intervenant et leurs patients ; puis au fil des besoins)

### Contexte

Un paiement, une séance ou un dossier ne se retrouvaient qu'en passant par leur patient, ou en SQL (cas du « SE000083 » introuvable, 22/10/2026).

Joëlle veut « tout » pouvoir chercher — une date (les séances du jour avec leur patient), un code (quoi, de quel patient), un paiement, les patients d'une localité ou d'un thérapeute… — avec à chaque fois un accès direct, et des recherches qui s'ajouteront au fil des besoins.

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **Hub de tuiles + un écran par recherche, résultat et ouverture communs** | Chaque recherche a des critères adaptés ; ajouter une recherche = une requête, un écran, une tuile | Un écran de plus par recherche |
| Une seule zone « tout chercher » | Un seul écran | Critères ambigus (une date ? un nom ? un montant ?), résultats mêlés difficiles à lire |

### Décision (décisions de Joëlle, 23/10/2026)

1. Bouton **Recherche** dans le menu, sous Patients ; il ouvre un **hub** (`UC_RechercheHome`) : en haut une **recherche rapide par code**, dessous une tuile par recherche (écrans `UC_Recherche…`).
2. **Un résultat commun** (`ResultatRecherche`) : nature (patient, dossier, séance, paiement, contact, intervenant…), code, description lisible, patient, dossier, séance, paiement. Chaque écran choisit ses colonnes.
3. **Une ouverture commune** (`OuvertureResultat`) selon la nature : patient → sa fiche ; contact / intervenant → la fiche, onglet Famille / Intervenants ; dossier → son hub ; séance → le hub, onglet Séances, séance sélectionnée ; paiement → le hub, onglet Paiements, paiement sélectionné. « Ouvrir le patient » ouvre toujours sa fiche.
4. L'écran ouvert est **empilé sur la recherche** (`Home.NavigateDepuisRecherche`) : Retour y ramène, critères et résultats compris (la recherche est recréée et relancée).
5. **Métier en lecture seule** : `GestionRecherche` + `QueryRecherche` (requêtes paramétrées).
6. **Confidentialité** : rien de ce qui est cherché n'est écrit dans le journal (type de recherche et nombre de résultats seulement) ; les critères ne vivent qu'en mémoire, le temps de la session ; les listes seront plafonnées (« précisez la recherche ») ; les droits sont ceux des écrans ouverts, comme depuis l'écran Patients.
7. **Codes** : ceux générés par la base (préfixe + identifiant sur 6 chiffres) ; la saisie est tolérante (casse, espaces, zéros de tête : « se83 » = SE000083) ; le préfixe le plus long l'emporte (PAI avant PA).

### Conséquences

- R1 (23/10/2026) : bouton du menu, hub, recherche rapide par code (PA, DO, SE, PAI, FC, AS), ouverture ciblée du hub dossier (`UC_DossierFiche.CiblerAuChargement`) et de la fiche patient (`UC_PatientFiche.SelectionnerOngletAuChargement`). Un paiement est rattaché au dossier de sa première séance affectée.
- Relevé de Joëlle au test de R1 : le code d'un paiement ne se voyait nulle part — colonne « Code » dans la grille des paiements du dossier et dans le détail des paiements d'une séance ; la barre d'état annonce le code d'un paiement enregistré (`GestionRecherche.CodeElement`).
- Idée de Joëlle (23/10/2026) : R5 — chercher un contact, un thérapeute ou un intervenant par son nom (ou son début), voir sa fiche et la liste des patients qui en relèvent ; elle remplace « patients par thérapeute » de R4.
- R2 (23/10/2026) : tuile « Séances » → `UC_RechercheSeances` : séances d'une période, tous patients (`GestionRecherche.RechercherSeances`, lecture par `GestionSeances.GetSeancesParPeriode`, même requête et même mapping que le planning du jour : `QuerySeances.SelectSeancesPlanning`).
  - L'écran affiche ses propres éléments (`SeancePlanningItem` : statut, paiement, solde) et ne convertit en `ResultatRecherche` que pour l'ouverture (`GestionRecherche.ResultatDeSeance`) : une recherche spécialisée peut lister ses objets métier, l'ouverture reste commune.
  - Filtre et résumé partagés avec l'onglet Séances du dossier (`FiltresSeances`).
  - Plafond des listes : `GestionRecherche.LIMITE_RESULTATS` (1000).
  - Le contexte d'une recherche spécialisée (« Recherche > Séances ») est réaffiché au retour (`Home.NavigateDepuisRecherche`, paramètre contexteRetour).
- R3 (23/10/2026, décisions de Joëlle) : tuile « Paiements » → `UC_RecherchePaiements` : le **relevé de l'onglet Paiements du dossier**, tous patients (date de mouvement : encaissement, ou séance d'un paiement attendu), avec un filtre par mode et les **totaux encaissés par mode** (`GestionRecherche.TotauxEncaissesParMode` ; un règlement par avoir hors total).
  - Lecture par `GestionPaiements.GetPaiementsParPeriode` (requête commune des listes de paiements, `QueryPaiements.SelectPaiementsBase`)
  - relevé, filtre et résumé partagés avec le dossier (`FiltresPaiements`)
  - ouverture par `GestionRecherche.ResultatDePaiement`.
  - La période de toutes les recherches est gérée par `SelecteurPeriode`.
- R4 (24/10/2026, décisions de Joëlle) : tuile « Patients par lieu » → `UC_RecherchePatientsLieu` : une zone « code postal ou localité », les **lieux** trouvés avec leur nombre de patients (« Tous les lieux trouvés » en tête, « Sans adresse » en dernier) et les **patients** du lieu choisi ; inactifs comme dans l'écran Patients.
  - Les patients sont lus une fois par la liste de l'écran Patients (`GestionPatients.GetPatients`, qui porte désormais code postal, localité et pays : `QueryPatients.SelectPatientsListe` V2.7.0) plutôt que par une seconde requête (`GestionRecherche.ChargerPatientsPourLieux`) ; la recherche se fait en mémoire à chaque frappe (`GestionRecherche.GrouperParLieu`), sans journal.
  - Un mot en chiffres = début du code postal, un autre = partie de la localité ; accents, majuscules et traits d'union indifférents (`UtilsCodePostal.CleCodePostal` / `CleLocalite`, reprises par la validation code postal / localité) ; deux écritures d'une même localité forment un seul lieu (`LieuPatients`).
  - Une adresse sans pays est du pays du cabinet, qui n'est pas répété dans les libellés.
- R5 (24/10/2026, décisions de Joëlle) : tuile « Réseau par nom » → `UC_RechercheReseau` : un thérapeute, un intervenant ou un contact par son nom (chaque mot tapé = début d'un mot du nom ou du prénom) ; **une ligne par personne** — un thérapeute du référentiel, les intervenants saisis de même nom, les contacts de même nom et prénom (`PersonneReseau`, `LienReseau`) ; un intervenant lié à un thérapeute compte sous ce thérapeute.
  - À droite les patients qui en relèvent (lien ou rôle, période, état du suivi : **suivis terminés inclus et signalés**) ; double-clic → la fiche du patient, onglet Famille / Contacts ou Intervenants (`GestionRecherche.ResultatDeLien`, `OuvertureResultat`) ; « Fiche de la personne » → le contact ou l'intervenant **en consultation**, ou la fenêtre du thérapeute ; le réseau est relu après un enregistrement.
  - Réseau lu une fois (`GestionRecherche.ChargerReseau`, trois requêtes de `QueryRecherche` sans UNION : collations différentes), recherche en mémoire (`FiltrerReseau`), sans journal ; patients inactifs comme l'écran Patients.
  - Comparaison sans accents ni majuscules réunie dans `UtilsTexte` (`CleComparaison`, `Mots`), reprise par `UtilsCodePostal.CleLocalite`.
  - Relevé de Joëlle au test : les codes FC…, AS… et TH… se voient désormais (colonne « Code » des grilles, titre des fenêtres, annonce de l'enregistrement) ; décision de Joëlle : le code **TH…** est reconnu par la recherche par code (nature `Therapeute`, `QueryRecherche.SelectTherapeuteParId`) et ouvre le réseau par nom sur ce thérapeute (`OuvertureResultat.OuvrirTherapeute`).
- Une recherche nouvelle n'ajoute ni table ni migration : une requête de `QueryRecherche`, une fonction de `GestionRecherche`, un écran et une tuile.
- Les préfixes des codes sont une connaissance du schéma (colonnes STORED) : centralisés dans `GestionRecherche` ; à revoir avec les codes système (lot C.3) si un préfixe changeait.

### Traçabilité

- R5 :
  - `UI/Controls/Recherche/UC_RechercheReseau.vb` (+ Designer, resx), `Metier/Recherche/PersonneReseau.vb`, `Utils/Helpers/UtilsTexte.vb` (nouveaux)
  - `QueryRecherche` (V1.1.0), `GestionRecherche` (V1.5.0), `ResultatRecherche` (V1.1.0), `OuvertureResultat` (V1.2.0), `UtilsCodePostal` (V1.3.0), `UC_RechercheHome` (V1.4.0)
  - codes visibles : `UC_PatientFiche` (V1.11.0), `UC_Therapeutes` (V1.3.0), `ContactEdition` (V1.6.0), `IntervenantEdition` et `TherapeuteEdition` (V1.7.0)
  - icônes `Assets/Boutons_ico_48/rechercheReseau_*.png` (provisoires).
- R4 :
  - `UI/Controls/Recherche/UC_RecherchePatientsLieu.vb` (+ Designer, resx), `Metier/Recherche/LieuPatients.vb` (nouveaux)
  - `QueryPatients` (V1.11.0), `GestionPatients` (V1.6.0), `PatientListeItem` (V1.4.0), `GestionRecherche` (V1.3.0), `UtilsCodePostal` (V1.2.0), `UC_RechercheHome` (V1.3.0)
  - icônes `Assets/Boutons_ico_48/rechercheLieu_*.png` (provisoires) ; données de test : `Docs/Database/Maintenance/donnees_test_codes_postaux.sql`.
- R3 :
  - `UI/Controls/Recherche/UC_RecherchePaiements.vb` (+ Designer, resx), `UI/Controls/Recherche/SelecteurPeriode.vb`, `UI/Controls/Communs/FiltresPaiements.vb` (nouveaux)
  - `QueryPaiements` (V1.12.0), `GestionPaiements` (V1.16.0), `PaiementListeItem` (V1.9.0), `SeanceListeItem` (V1.10.0), `GestionRecherche` (V1.2.0), `UC_RechercheSeances` (V1.1.0), `UC_RechercheHome` (V1.2.0), `UC_DossierFiche` (V1.23.0)
  - icônes `Assets/Boutons_ico_48/recherchePaiements_*.png`.
- R2 :
  - `UI/Controls/Recherche/UC_RechercheSeances.vb` (+ Designer, resx), `UI/Controls/Communs/FiltresSeances.vb` (nouveaux)
  - `QuerySeances` (V1.5.0), `GestionSeances` (V1.16.0), `SeancePlanningItem` (V1.1.0), `GestionRecherche` (V1.1.0), `OuvertureResultat` (V1.1.0), `UC_RechercheHome` (V1.1.0), `UC_DossierFiche` (V1.22.0), `AltheaDate` (V1.1.0), `Home` (V1.19.0)
  - icônes `Assets/Boutons_ico_48/rechercheSeances_*.png`.
- R1 :
  - `Core/Database/Queries/QueryRecherche.vb`, `Metier/Recherche/ResultatRecherche.vb`, `Metier/Recherche/GestionRecherche.vb`, `UI/Controls/Recherche/UC_RechercheHome.vb` (+ Designer, resx), `UI/Controls/Recherche/OuvertureResultat.vb` (nouveaux)
  - `Home` (V1.18.0), `UC_DossierFiche` (V1.21.0), `UC_PatientFiche` (V1.10.0)
  - icônes `Assets/Boutons_Home/recherche_*.png`.

---

## ADR-53 - Codes postaux belges : un référentiel généré depuis bpost et BeST, en lecture seule

**Date :** 24/10/2026
**Statut :** Actée – Implémentée et validée (v2.56.0, 24/10/2026)

### Contexte

Seul le format des codes postaux était contrôlé (4 chiffres en Belgique). Joëlle veut, pour une adresse belge, un code qui existe et la localité qui lui correspond (demande du 23/10/2026).

La liste officielle de bpost ne donne qu'un nom par localité, en néerlandais pour Bruxelles et les communes à facilités (« Brussel », « Elsene », et même « Moeskroen » pour Mouscron) : seule, elle signalerait « 1000 Bruxelles » comme une erreur.

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **bpost + registre des adresses BeST (SPF BOSA), contenu généré par un script** | Codes officiels ; noms des communes en français, néerlandais et allemand, sections et noms postaux réels | Deux sources à télécharger pour une mise à jour |
| bpost seule | Une seule source | Faux avertissements à Bruxelles et dans les communes bilingues |
| Écran d'import dans l'application | Mise à jour sans script | Une bibliothèque Excel en plus ; ne lirait pas BeST (archives de 230 Mo) |

### Décision (décisions de Joëlle, 24/10/2026)

1. **Référentiel** `ref_codes_postaux_be` : une ligne par nom accepté (code, localité) et, pour chaque code, un nom **proposé**. Aucune clé étrangère : les adresses gardent leur texte.
2. **Contenu généré** par `Docs/Database/Maintenance/codes_postaux_be/generer_codes_postaux_be.py` (bpost + BeST + quelques ajouts faits à la main), livré par migration puis, au besoin, par un SQL de maintenance qui remplace tout le contenu. L'application ne l'écrit jamais.
3. **Règles** (`GestionCodesPostauxBe`, adresses dont le pays est la Belgique) :
   - code inexistant : **refusé** (valideurs métier des patients, contacts, thérapeutes ; contrôle de l'écran Cabinet) ;
   - localité **proposée** quand on quitte le code et que la localité est vide, et **suggérée** pendant la frappe ;
   - dans l'autre sens — on connaît plus souvent la localité que le code (relevé de Joëlle au premier essai) — code vide : la localité suggère « Liège · 4000 », « Liège · 4020 »… et un choix remplit les deux champs ; une localité à un seul code le remplit (« Avin » → 4280) ; à plusieurs codes, le champ Code les signale et les suggère ;
   - localité qui ne correspond à aucun nom du code : **avertissement** sur le champ, puis question à l'enregistrement (« Enregistrer quand même » / « Corriger ») — pas un refus.
4. **Comparaison des noms** : `UtilsCodePostal.CleLocalite` (accents, majuscules, traits d'union, points, « œ », « Saint / Sint / St ») ; le script applique la même règle.
5. **Une aide de saisie partagée** (`SaisieLocaliteBelge`) branchée sur les deux zones de chaque adresse : fiche patient, contact, thérapeute, cabinet, praticien.

### Conséquences

- Autres pays : seul le format reste contrôlé ; une adresse sans pays n'est pas contrôlée.
- Les adresses déjà saisies ne sont pas corrigées : elles sont contrôlées à leur prochaine modification.
- La liste est lue une fois par session (≈ 4 000 noms) ; si elle est illisible, l'aide de saisie se met en retrait (journal) et le valideur métier refuse l'enregistrement.
- Mise à jour rare, par le script (procédure : `Database_technique.md`, `ref_codes_postaux_be`).
- La recherche des patients par lieu (R4) groupe désormais « St-Georges » et « Saint-Georges » (même clé de localité).

### Traçabilité

- Migration `Docs/Database/Migration/migration_v2.56.0_codes_postaux_be.sql` (+ ROLLBACK), script `Docs/Database/Maintenance/codes_postaux_be/generer_codes_postaux_be.py`, `VersionSchema` (V1.0.17)
- `Core/Database/Queries/QueryCodesPostauxBe.vb`, `Metier/Referentiels/GestionCodesPostauxBe.vb`, `Utils/UI/SaisieLocaliteBelge.vb` (nouveaux)
- `UtilsCodePostal` (V1.4.0), `GestionPatients` (V1.7.0), `GestionFamilleContacts` (V1.4.0), `GestionTherapeutes` (V1.6.0), `UC_PatientFiche` (V1.12.0), `ContactEdition` (V1.7.0), `TherapeuteEdition` (V1.8.0), `UC_Cabinet` (V1.11.0)

---

## ADR-54 - Codes système : un point unique (CodesSysteme) et des drapeaux de référentiel pour les règles

**Date :** 24/10/2026
**Statut :** Actée – Étape 1 implémentée et validée (24/10/2026) ; étape 2 (drapeaux, v2.57.0) implémentée et validée le 24/10/2026 ; lignes système devenues inutiles déverrouillées (v2.58.0, 25/10/2026)

### Contexte

Audit du 18/10/2026, § 5.1 et lot C.3 : environ quarante codes métier écrits en clair dans le SQL (`'PAYE'`, `'EN_ATTENTE'`, `'CLOTURE'`…), d'autres dans le code (`"ANAMNESE"`, `"PDF"`, `"MERE"`…), des constantes réparties entre `GestionSeances` et `GestionDossiers` et aucune pour les paiements. Une faute de frappe dans un code ne se voit qu'à l'exécution (audit B9 : le tarif cherchait « NO_SHOW », qui n'existe pas).

Certaines règles dépendent du code d'un statut alors qu'elles pourraient se dire par une case du référentiel : ce qui fait une séance « non tenue » est écrit en trois listes qui ne concordent pas (ToDo).

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **Un module `CodesSysteme`, une section par famille** | Un seul endroit à lire ; le compilateur vérifie chaque usage | Les anciennes constantes déménagent (appels à mettre à jour) |
| Constantes chez leur propriétaire (`GestionSeances`, `GestionDossiers`, `GestionPaiements`) | Moins de fichiers touchés | Familles dispersées ; rien pour ce qui n'a pas de propriétaire évident |
| Dans le SQL : paramètres `@statut…` | Forme la plus formelle | Une quarantaine d'appels à modifier, sans gain de sécurité (ce sont des constantes) |

### Décision (décisions de Joëlle, 24/10/2026)

1. **`CodesSysteme`** (`Metier/Referentiels/CodesSysteme.vb`) : statuts de séance, de dossier et de paiement, types et sources de document. Une classe (et non un module) dont les familles portent des noms au pluriel (`CodesSysteme.StatutsSeance.Planifiee`), pour ne pas se confondre avec les classes du métier (`StatutSeance`).
2. Les petites familles déjà rangées chez leur propriétaire y restent : usages des textes types (`UsagesTexteType`), types d'organisme (`TypeOrganisme`), statuts des sauvegardes, types d'identifiant (`UtilsIdentifiantBelge`), clés de paramètres (`ClesParametres`).
3. **SQL** : le texte des requêtes est bâti sur ces constantes (`Private Const STATUT_PAYE As String = CodesSysteme.StatutsPaiement.Paye`, puis `'{STATUT_PAYE}'`). Ce n'est jamais une saisie : les requêtes restent paramétrées pour tout ce qui vient de l'utilisateur.
4. **Une règle qui peut se dire par une case du référentiel y est lue** plutôt que tirée d'un code (étape 2, migration v2.57.0, Joëlle décidant pour chaque valeur) :
   - statuts de séance : « non tenue, libère le créneau » (à la place des trois listes) et « proposé à la création » ;
   - liens et rôles légaux d'un contact : « par défaut » (à la place de « Mère » et « Autorité parentale » écrits dans la fenêtre Contact).

### Conséquences

- Les valeurs de `CodesSysteme` existent en base (lignes système verrouillées, ADR-28, ou valeurs d'une colonne) : les changer exige une migration.
- La vue `v_seances_solde` (v2.49.0, définie en base) garde `'PAYE'` en clair.
- Le code mort qui portait une règle de statut est retiré (`Seance.EstRealisee`, `EstPlanifiee`, `EstAnnulee`).
- Règle ajoutée à `Rules.md` (lignes système) : pas de code système en clair, ni dans le code ni dans le SQL.
- Étape 2 — valeurs de départ de la migration v2.57.0 (posées pendant une absence de Joëlle, à revoir dans les référentiels) : non tenue pour Annulée patient, Annulée praticien, Reportée et **Maladie**, qui libère désormais son créneau ; proposé à la création pour Planifiée et Réalisée ; par défaut Mère et Autorité parentale. Les statuts ajoutés par Joëlle (TEST_C1) ne sont pas touchés.
- **Absence non excusée** : la case « Absence » (v2.53.0) suffit à la rendre « non tenue » pour la règle entre statuts (passer d'une annulée à une absence sans contrôle de créneau), mais elle garde son créneau : la personne était attendue à cette heure.
- **Planifiée reste proposée à la création** (refus à l'enregistrement du référentiel) : c'est le statut d'une nouvelle séance ; « Reportée » reste écartée en modification par son code (elle naît du report).
- **Par défaut** : une seule valeur par référentiel, garantie par l'application (les autres décochées dans la même transaction) ; aucune n'est obligatoire.
- **Une ligne n'est système que tant qu'on la cherche par son code** (décision de Joëlle du 24/10/2026) : migration v2.58.0, verrou levé sur `ABSENTE`, `ANNULEE_PATIENT`, `ANNULEE_PRATICIEN`, `MALADE`, `NO_SHOW`, `MERE` et `AUTORITE_PARENTALE`. Restent verrouillés `PLANIFIEE`, `REALISEE`, `REPORTEE`, les statuts de dossier, les types de document, le mode `AVOIR` (seul mode marqué « avoir » : le désactiver bloquerait les avoirs) et le tarif `INAMI`.

### Traçabilité

- Étape 1 (24/10/2026) : `Metier/Referentiels/CodesSysteme.vb` (nouveau) ; `QueryPaiements` (V1.12.1), `QueryPatients` (V1.11.1), `QuerySeances` (V1.5.1), `GestionSeances` (V1.16.1), `GestionDossiers` (V1.6.1), `Seance` (V1.1.0), `SeanceListeItem`, `PaiementListeItem`, `Dossier`, `DossierListeItem`, `Document`, `FiltresSeances`, `UC_DossierFiche`, `UC_PatientFiche`, `UC_PatientHome`, `DossierStatutEditor`, `DossierZoneEditor`, `SeanceEditor`, `UtilsIcons`, `UtilsPastilles`.
- Étape 2 (24/10/2026) : migration `migration_v2.57.0_drapeaux_referentiels.sql` (+ rollback), `VersionSchema` (V1.0.18) ; `CodesSysteme` (V1.1.0 : codes Absente, Malade et Annulée retirés, devenus inutiles) ; `QueryStatutsSeance` (V1.3.0), `StatutSeance` (V1.2.0), `GestionStatutsSeance` (V1.3.0), `UC_StatutsSeance` (V1.4.0) ; `GestionSeances` (V1.17.0 : `EstStatutNonTenu`, `OccupeSonCreneau`, `StatutEstProposable`, `EnregistrerSeance`), `QuerySeances` (V1.6.0), `SeanceListeItem` (V1.11.0) ; `QueryLiensPatient`, `LienPatient`, `GestionLiensPatient`, `UC_LiensPatient`, `QueryRoleLegal`, `RoleLegal`, `GestionRoleLegal`, `UC_RoleLegal` ; `ContactEdition` (V1.8.0). Plan de tests du parcours : C3-07 à C3-16.
- Déverrouillage (25/10/2026) : migration `migration_v2.58.0_lignes_systeme_deverrouillees.sql` (+ rollback), `VersionSchema` (V1.0.19). Plan de tests du parcours : C3-17 à C3-19.

---

## ADR-55 - Fenêtre principale : un contrat (IFenetrePrincipale) au lieu de la classe Home

**Date :** 25/10/2026
**Statut :** Actée – Implémentée et validée le 25/10/2026 (étapes 1 à 3, NAV-C4-01 à 15)

### Contexte

Audit du 18/10/2026, § 5.2 et lot C.4 : une soixantaine d'endroits dans 20 fichiers atteignent la fenêtre principale par sa classe — `TryCast(FindForm(), Home)` dans les écrans, paramètre `homeForm As Home` dans les fenêtres (Contact, Intervenant, Thérapeute, Séance, Paiement) — pour naviguer, revenir, ouvrir un référentiel en fenêtre (bouton [+]) ou rafraîchir le bandeau. Chaque écran dépend ainsi de la fenêtre entière, ne peut être ouvert ni testé ailleurs, et la navigation reste mêlée aux écrans (frein à un futur client web). Le bloc « élévation d'accès » est en outre écrit six fois (quatre dans `Home`, une dans chaque hub).

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **Une interface `IFenetrePrincipale`, implémentée par `Home`, fournie par le contexte** | Un seul contrat lisible ; les écrans l'ont déjà sous la main (`UserControlContext`) ; aucun changement visible | `Home` garde plusieurs rôles (navigation, fenêtres, bandeau) |
| Plusieurs petits services (navigation, fenêtres, bandeau) | Séparation plus fine | Trois objets là où un suffit aujourd'hui |
| Les fenêtres reçoivent le contexte entier, qui expose la navigation | Un paramètre de moins à nommer | Le contexte deviendrait un fourre-tout |

### Décision (décisions de Joëlle, 25/10/2026)

1. **`IFenetrePrincipale`** (`UI/Navigation`) : naviguer (menu, écran d'administration ou de référentiel, fiche avec retour, ouverture depuis la recherche), revenir, ouvrir un référentiel ou la création d'un thérapeute en fenêtre (droits et élévation gérés par la fenêtre principale), tenir le fil d'Ariane, rafraîchir le bandeau. `Home` l'implémente.
2. **Les écrans** la trouvent dans `UserControlContext.FenetrePrincipale` (fournie par `Home`) ; **les fenêtres** la reçoivent en paramètre (`fenetrePrincipale As IFenetrePrincipale`). Nothing reste une valeur normale (écran hébergé dans une fenêtre de référentiel, fenêtre ouverte hors navigation) : ce qui en dépend est masqué, comme avant.
3. **L'élévation d'accès** est réunie en une méthode de la fenêtre principale, appelée par les hubs au lieu de refaire le bloc (étape 3).
4. Découpage : étape 1 contrat, contexte et fenêtres ; étape 2 écrans Patients, Dossier, Domaines, Cabinet, Utilisateurs, Thérapeutes ; étape 3 recherches, hubs et élévation, puis vérification de la garde « modifications non enregistrées » (`IUnsavedChangesAware`, déjà en place).

### Conséquences

- Aucun changement visible : mêmes écrans, mêmes boutons, mêmes droits.
- Un écran ou une fenêtre n'importe plus la classe `Home` ; seul l'outil de capture d'écran (Debug, `FiltreCaptureEcran`, installé par `Home`) la connaît encore.
- Une nouvelle capacité de la fenêtre principale s'ajoute au contrat, pas comme méthode publique isolée de `Home`.

### Traçabilité

- Étape 1 (25/10/2026) : `UI/Navigation/IFenetrePrincipale.vb` (nouveau) ; `UserControlContext` (V1.4.0, `FenetrePrincipale`) ; `Home` (V1.21.0, `Implements IFenetrePrincipale`) ; `ContactEdition` (V1.9.0), `IntervenantEdition` (V1.8.0), `TherapeuteEdition` (V1.9.0), `SeanceEditor` (V1.11.0), `PaiementEditor` (V2.2.0). Plan de tests Home & navigation : section 9 (NAV-C4-01 à 05).
- Étape 2 (25/10/2026) : la fenêtre principale est lue dans `_context.FenetrePrincipale` par `UC_PatientHome` (V1.7.2), `UC_PatientFiche` (V1.12.2), `UC_DossierFiche` (V1.23.2), `UC_DomainesHome` (V1.13.1), `UC_Cabinet` (V1.11.1), `UC_Utilisateurs` (V1.9.1), `UC_Therapeutes` (V1.3.1). NAV-C4-06 à 10.
- Étape 3 (25/10/2026) : `OuvertureResultat` (V1.2.1) et les recherches `UC_RechercheHome`, `UC_RechercheSeances`, `UC_RecherchePaiements`, `UC_RecherchePatientsLieu`, `UC_RechercheReseau` ; hubs `UC_AdminHome` (V1.7.0) et `UC_ReferentielHome` (V1.7.0) — ouverture des écrans par `Ouvrir` (20 + 7 blocs recopiés réunis), élévation par `DemanderElevation` ; `Home` (V1.22.0) : `DemanderElevation`, `ObtenirElevationPourZone`, `OuvrirFenetreElevation` (six blocs réunis) ; `IFenetrePrincipale` (V1.1.0) ; `ElevationAcces` (V1.3.1, n'écrit plus le fil d'Ariane). Garde « modifications non enregistrées » vérifiée : tous les écrans à bouton Enregistrer implémentent `IUnsavedChangesAware` (fiches patient et dossier, Domaines, Cabinet, Paramètres, Sauvegardes, socle des référentiels), contrôlée par `NavigationManager` avant chaque navigation, Retour et fermeture ; aucun changement. Règle ajoutée à `Rules.md` § 9. NAV-C4-11 à 15.

---

## ADR-56 - Grosses fiches à onglets : un fichier partiel par onglet

**Date :** 25/10/2026
**Statut :** Actée – Implémentée et validée le 25/10/2026 (C5-01 à C5-04)

### Contexte

Audit du 18/10/2026, § 5.1 et lot C.5 : `UC_DossierFiche` (4 266 lignes) et `UC_PatientFiche` (4 073) dépassent de loin la taille où l'on se repère dans un fichier. Leur code est déjà rangé en régions, une par onglet, aux responsabilités distinctes (dossier : Séances, Paiements, Suivi clinique ; patient : Famille / Contacts, Intervenants, Dossiers). D'autres fichiers sont gros (`Home`, `SeanceEditor`, `UC_DomainesHome`, `UC_PatientHome`, `GestionSeances`) mais d'un seul tenant.

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **`Partial Class` par onglet** | Code déplacé tel quel, aucun changement de comportement ; Designer intact ; un onglet = un fichier lisible | La classe reste une seule classe (champs partagés) |
| Un UserControl par onglet | Découplage réel | Échanges entre onglets à réécrire (événements, état) : risque élevé pour un gain de lisibilité comparable |
| Ne pas découper | Rien à faire | Fichiers difficiles à parcourir |

### Décision (décisions de Joëlle, 25/10/2026)

1. **Un fichier partiel par onglet lourd** : `UC_DossierFiche.ZonesCliniques.vb`, `.Seances.vb`, `.Paiements.vb` ; `UC_PatientFiche.Famille.vb`, `.Intervenants.vb`, `.Dossiers.vb`. Chaque région est déplacée à l'identique (contrôle par comparaison avec la version précédente) ; les procédures gardent leur en-tête et leur historique.
2. Le fichier principal garde les champs, le contexte, le chargement commun, le bandeau, les modes, l'édition, la navigation et la garde des modifications.
3. **Périmètre : les deux fiches seulement.** Les autres gros fichiers sont d'un seul tenant : les couper par « Initialisation / Actions » n'aiderait pas.
4. Les fichiers partiels sont rangés sous la fiche dans l'Explorateur de solutions (`DependentUpon`) et s'ouvrent comme du code (`SubType` Code).

### Conséquences

- Fiche dossier : 1 997 lignes dans le fichier principal (au lieu de 4 266) ; fiche patient : 2 739 (au lieu de 4 073).
- Règle ajoutée à `Rules.md` § 3 (structure) pour les fiches à venir (Documents, Agenda).

### Traçabilité

- 25/10/2026 : `UC_DossierFiche` (V1.24.0) et ses trois fichiers partiels ; `UC_PatientFiche` (V1.13.0) et ses trois fichiers partiels ; `Althea.vbproj` (regroupement). Plan de tests du parcours : C5-01 à C5-04.

---

## ADR-57 - Option Strict On pour tout le projet, .editorconfig et analyseurs .NET

**Date :** 25/10/2026
**Statut :** Actée – étape 1 (`Option Strict On`) implémentée et validée le 25/10/2026 (D1-01 à D1-08) ; étape 2 (`.editorconfig`, analyseurs) implémentée et validée le 25/10/2026 (D1-09, D1-10)

### Contexte

Audit du 18/10/2026, § 5.1 et lot D, point 2 : le projet compilait avec `<OptionStrict>Off</OptionStrict>` (`Althea.vbproj`).

- Sur 335 fichiers `.vb` hors Designer, 38 n'avaient pas `Option Strict On` en tête : référentiels de l'ancienne génération, sécurité, cabinet, paramètres, quelques utilitaires.
- Les 54 `.Designer.vb` n'ont aucune ligne `Option`.
- Tous ces fichiers compilaient en liaison tardive, conversions implicites permises.
- Pas de `.editorconfig` : encodage mélangé (51 `.vb` sans BOM).

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **Inventaire par une compilation d'essai, puis corrections** | Liste réelle des erreurs avant de toucher au code ; étapes taillées sur la réalité | Une compilation de plus |
| Ajouter `Option Strict On` lot par lot, sans inventaire | Pas de préparation | Nombre de lots fixé sur une estimation |
| Garder `Off` au niveau du projet | Rien à faire | Un nouveau fichier qui oublie la ligne compile sans contrôle ; Designer jamais contrôlés |

### Décision (décisions de Joëlle, 25/10/2026)

1. **Inventaire d'abord** : `Tools\build.cmd -p:OptionStrict=On --no-incremental` (le script transmet ses options à `dotnet build`).
   - Résultat : 12 erreurs seulement, toutes BC30512 — un identifiant `BIGINT UNSIGNED` lu par `GetInt64` / `GetInt32` dans une propriété `ULong` ; aucune dans les Designer.
   - Corrigées en une seule étape au lieu des trois lots prévus.
2. **`Option Strict On` au niveau du projet** (`Althea.vbproj`) : plus aucune conversion implicite restrictive ni liaison tardive, Designer compris.
3. **Les lignes `Option Strict On` / `Option Explicit On` restent en tête de chaque fichier**, nouveau fichier compris : aucun changement massif, et chaque fichier reste explicite même copié ailleurs.
4. **`.editorconfig` à la racine** : il reprend les réglages exportés de Visual Studio le 22/06/2026, rangés jusque-là dans `My Project/.editorconfig` — fichier inerte : il ne s'appliquait qu'à ce dossier et le `.vbproj` l'excluait (`EditorConfigFiles Remove`). L'ancien fichier et ses deux lignes du `.vbproj` sont retirés. Ajouts et retraits :
   - encodage `utf-8-bom` pour les `.vb`, indentation 4 ;
   - pas de règle `end_of_line` : `.gitattributes` (`* text=auto`) range tout en LF dans le dépôt, une fin de ligne changée par VS ne crée aucune différence ;
   - ni `trim_trailing_whitespace` ni `insert_final_newline` : VS réécrirait des lignes à chaque enregistrement.
5. **Analyseurs .NET** : jeu « Recommended » (`<AnalysisMode>Recommended</AnalysisMode>`), niveaux réglés règle par règle dans `.editorconfig`.
   - Inventaire (`Tools\build.cmd -p:AnalysisMode=Recommended --no-incremental`) : 545 avertissements, 26 règles ; aucun avertissement du compilateur.
   - Écartées (`none`), contraires aux conventions du projet : CA1707 (traits de soulignement des constantes et gestionnaires), CA1305 (culture implicite : Althéa travaille en culture française), CA1725 (nom de paramètre `e` des redéfinitions).
   - Les 23 autres règles rencontrées passent au niveau « suggestion » : la dette existante ne pollue pas le journal, elle reste visible dans l'éditeur.
   - Toute autre règle du jeu qui se déclencherait dans du code nouveau sort en avertissement : visible, jamais bloquant.
   - Une règle n'est durcie (`warning`) qu'après correction de ses cas.
   - CA1824 corrigée au lieu d'être réglée : `<NeutralLanguage>fr</NeutralLanguage>` (langue des ressources de l'assembly).

### Conséquences

- Un identifiant `BIGINT UNSIGNED` se lit par `reader.GetUInt64(...)` (forme des référentiels récents).
- Deux avertissements du compilateur corrigés en passant : `UC_DomainesHome.EnregistrerLogo` (retour explicite), `UtilsButtons.GetStandardButtonAssetFolder` (dossier 32 px par défaut au lieu de `Nothing`).
- `Rules.md` § 7 et § 22, environnement technique, structure de la solution.
- Cas des analyseurs à corriger en priorité, notés au ToDo : un `TryParse` non vérifié (CA1806), 7 comparaisons de textes sans mode précisé (CA1304, CA1309, CA1310), un champ supprimable jamais libéré dans une fenêtre de test (CA1001).

### Traçabilité

- 25/10/2026 : 38 fichiers (`Option Strict On`), dont 12 lectures d'identifiant corrigées ; `Althea.vbproj` ; `Tools/build.cmd` V1.1.0 ; `UC_DomainesHome` V1.13.2, `UtilsButtons` V1.0.1. Plan de tests du parcours : D1-01 à D1-08.
- 25/10/2026 : `.editorconfig` V1.0.0 (racine), `My Project/.editorconfig` retiré ; `Althea.vbproj` (`AnalysisMode`, `NeutralLanguage`). Plan de tests du parcours : D1-09, D1-10.

---

## ADR-58 - Tests automatisés : un projet xUnit en VB, à côté de l'application

**Date :** 26/10/2026
**Statut :** Actée – étape 1 (socle, identifiants, sécurité, saisie) implémentée et validée le 26/10/2026 (D2-01 à D2-03) ; étape 2 (dates, heures, quota) implémentée et validée le 26/10/2026 (D2-04 à D2-06) ; étape 3 (test de fumée des requêtes) implémentée et validée le 26/10/2026 (D2-07, D2-08)

### Contexte

Audit du 18/10/2026, § 5.4 et lot D, point 1 : aucun test automatisé, aucune intégration continue ; les régressions ne se voient qu'aux tests manuels des plans de tests.

- `Althea.vbproj` est à la racine du dépôt et compile tous les `.vb` de ses sous-dossiers : un projet de tests rangé dessous y serait compilé si on ne l'exclut pas.
- Une partie du métier est déjà testable telle quelle : fonctions de validation et de calcul sans écran ni base (identifiants, mot de passe, téléphone, codes postaux…).
- Deux calculs ne le sont pas encore : les interprètes de saisie de `AltheaDate` / `AltheaHeure` (privés) et le calcul du quota INAMI (règle lue en base au milieu du calcul).

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| **Projet VB `Tests/Althea.Tests`, xUnit v3, exclu de l'application** | Même langage que le code ; aucun déplacement de l'application ; outil standard (Explorateur de tests, `dotnet test`, CI) | Exclusion à poser dans `Althea.vbproj` |
| Projet de tests en C# | Plus d'exemples en ligne | Second langage dans le dépôt |
| Déplacer l'application dans `src/` | Arborescence classique | Déplacement massif de fichiers, historique Git plus difficile à suivre |

### Décision (décisions de Joëlle, 26/10/2026)

1. **Projet `Tests/Althea.Tests` en VB.NET**, xUnit v3 (ligne 3.2, éprouvée ; la 4.x est très récente) avec l'adaptateur VSTest (`xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`).
   - Même cible que l'application (`net8.0-windows7.0`, WinForms), référence directe à `Althea.vbproj`, `MyType` Empty (le point d'entrée est généré par xUnit).
   - Lancé par `Tools\test.cmd` (journal `obj\_tests.log`) ou l'Explorateur de tests de Visual Studio.
2. **Exclu de l'application** (`Compile` / `EmbeddedResource` / `None Remove="Tests\**"` dans `Althea.vbproj`) et ajouté à `Althea.slnx` (dossier « Tests »).
3. **Fonctions privées à tester** : `Friend` + `InternalsVisibleTo` réservé à `Althea.Tests`, plutôt que `Public` (étape 2).
4. **Calcul du quota** : la règle en vigueur est fournie au calcul par une fonction (date → règle) au lieu d'être lue en base au milieu du calcul ; même calcul, même résultat (étape 2).
5. **Test de fumée des modules `Query*`** (étape 3) :
   - chaque requête est préparée par MariaDB (`PREPARE`), jamais exécutée ;
   - sur une base vide dédiée `althea_tests`, créée depuis `backup_NoData_althea.sql` (structure seule), avec un compte MariaDB limité à cette base ;
   - chaîne de connexion par la variable d'environnement `ALTHEA_TESTS_CONNEXION`, jamais dans le dépôt ; test ignoré si elle manque (poste sans base, intégration continue).
   - Mise en œuvre : la structure est recréée dans `althea_tests` à chaque série de tests, depuis la sauvegarde de structure (séquences citées `` `althea`.`seq_…` `` ramenées à la base courante, clause `DEFINER` retirée) ; refus de toute autre base que `althea_tests` (la préparation y supprime toutes les tables) ; requêtes recensées par réflexion dans les modules `Query*` (plus les zones cliniques) ; paramètres `@nom` préparés comme `?` ; un modèle à repère remplacé à l'exécution (`{IDS}`) est déclaré un par un, jamais accepté en bloc.
6. **Conventions** :
   - une classe de tests par module testé, rangée comme lui (`Utils/`, `Core/`, `Metier/`) ;
   - noms `Methode_Cas_Attendu` ;
   - données fictives uniquement ;
   - quand le résultat attendu est un calcul (clé de contrôle, empreinte), il est calculé hors du code testé.

### Conséquences

- Étape 1 : 154 tests (identifiants, sécurité, saisie), exécutés en 3 secondes.
- L'empreinte des mots de passe est figée par un test : un changement accidentel des paramètres (algorithme, itérations, taille) empêcherait toute connexion des comptes existants.
- Règle ajoutée à `Rules.md` § 3 ; stratégie de tests, structure de la solution, environnement technique.
- L'intégration continue (D-3) lancera ces tests à chaque envoi sur GitHub ; le test de fumée, qui demande la base, reste local.
- Étape 3 : 389 requêtes préparées sans erreur (une seule à déclarer comme modèle) ; la sauvegarde de structure est vérifiée à chaque série (elle se recharge dans une base neuve) ; 621 tests en tout.
- Constat pour l'installation : la sauvegarde de structure cite la base `althea` dans les valeurs par défaut de 15 tables (`DEFAULT nextval(`althea`.`seq_…`)`) ; une base installée sous un autre nom pointerait vers les séquences d'`althea` — noté au ToDo (préparation de l'installation).

### Traçabilité

- 26/10/2026 : étape 1 — `Tests/Althea.Tests` (7 classes de tests), `Tools/test.cmd` V1.0.0, `Althea.vbproj` (exclusion), `Althea.slnx`. Plan de tests du parcours : D2-01 à D2-03.
- 26/10/2026 : étape 3 — `Tests/Althea.Tests/Core` : `BaseDeTests`, `CatalogueRequetes`, `RequetesFumeeTests` ; `Docs/Database/Maintenance/creer_base_tests.sql` ; 621 tests. Plan de tests du parcours : D2-07, D2-08.
- 26/10/2026 : étape 2 — `Althea.vbproj` (`InternalsVisibleTo`), `AltheaDate` V1.1.1 et `AltheaHeure` V1.0.1 (interprètes `Friend`), `GestionQuotaInami` V3.1.0 (`CalculerQuota` V2.0.0, règle fournie par une fonction, `Friend`) ; classes `AltheaDateTests`, `AltheaHeureTests`, `GestionQuotaInamiTests` ; 229 tests. Plan de tests du parcours : D2-04 à D2-06.

---

## ADR-59 - Intégration continue : une vérification GitHub par jour, à la demande

**Date :** 26/10/2026
**Statut :** Actée – implémentée le 26/10/2026 ; premier passage à la demande au vert le 26/10/2026 (D3-01) ; passage du soir à constater (D3-02)

### Contexte

Audit du 18/10/2026, § 5.4 et lot D, point 1 : aucune intégration continue. Les tests automatisés existent depuis D-2 (ADR-58, 621 tests) mais ne tournent que sur le poste de Joëlle.

- Dépôt privé `AngeljoNG/Althea` : 2 000 minutes gratuites par mois, une minute Windows coûte plus qu'une minute Linux ; l'application (WinForms) ne se compile que sous Windows.
- Joëlle pousse chaque commit sur `master` au fil de la journée (c'est aussi sa sauvegarde hors du poste) : 5 à 10 push un jour de travail.
- La garde au quotidien est `Tools\test.cmd` sur le poste, avant chaque commit (Rules § 3). Ce qu'elle ne voit pas : un fichier oublié dans un commit (le dépôt en ligne ne compile plus) et la configuration Release.

### Options envisagées

| Option | Avantages | Inconvénients |
| --- | --- | --- |
| À chaque push sur `master` | Retour immédiat | Le plus coûteux en minutes ; peut approcher le plafond |
| Branche de travail, fusion sur `master` le soir | Un passage par jour | Une fusion chaque jour dans Visual Studio ; changement d'habitude |
| Ne pousser qu'en fin de journée | Un passage par jour | Plus de sauvegarde hors du poste pendant la journée |
| **Un passage par jour, le soir, s'il y a eu un commit ; et à la demande** | Habitudes inchangées ; coût borné | Retour le soir (ou à la demande) |

### Décision (décision de Joëlle, 26/10/2026)

1. **Workflow `.github/workflows/ci.yml`**, déclenché :
   - chaque soir à 20:30 UTC (21:30 à Bruxelles en hiver, 22:30 en été), **sauté s'il n'y a eu aucun commit depuis 24 h** (contrôle préalable sur une machine Linux, quelques secondes) ;
   - à la demande : bouton « Run workflow » de l'onglet Actions.
2. **Machine Windows** (`windows-latest`), .NET 8 ; restauration, compilation de l'application et des tests en **Release**, tests (`dotnet test` sur `Tests/Althea.Tests`).
3. **Aucun secret** : la clé Syncfusion n'est lue qu'à l'exécution ; le test de fumée s'ignore sans `ALTHEA_TESTS_CONNEXION`.
4. **Droits minimaux** (`contents: read`) ; une exécution plus récente annule celle en cours.
5. Joëlle continue de pousser chaque commit sur `master`.

### Conséquences

- Coût borné : au plus un passage Windows par jour de travail, plus les passages demandés. Mesuré au premier passage (26/10/2026) : 2 min 07 s en tout, dont 1 min 53 s sur Windows — large marge sous les 2 000 minutes mensuelles ; un passage à chaque push resterait possible si le besoin s'en faisait sentir.
- Le résultat se lit dans l'onglet « Actions » du dépôt ; en cas d'échec, Joëlle copie les dernières lignes de l'étape en rouge.
- Stratégie de tests, environnement technique, structure de la solution.

### Traçabilité

- 26/10/2026 : `.github/workflows/ci.yml` V1.0.0. Plan de tests du parcours : D3-01, D3-02.

---

[TOC]
