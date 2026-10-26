# Althéa — Structure détaillée de la solution

> Vue de maintenance dérivée de `Althea.vbproj`. Elle décrit les sources applicatives ; les répertoires `Assets/` et `Docs/` sont documentés séparément.
> Dernière mise à jour : 26/10/2026 — intégration continue `.github/workflows/ci.yml` (ADR-59)

<details>
<summary>Historique des mises à jour</summary>

- 26/10/2026 — projet de tests `Tests/Althea.Tests`, `Tools/` (ADR-58)
- 24/10/2026 — `CodesSysteme` (codes système en un seul endroit, audit C.3)
- 24/10/2026 — codes postaux belges : `QueryCodesPostauxBe`, `GestionCodesPostauxBe`, `SaisieLocaliteBelge`
- 24/10/2026 — recherche R5 : `UC_RechercheReseau`, `PersonneReseau`, `UtilsTexte`
- 24/10/2026 — recherche R4 : `UC_RecherchePatientsLieu`, `LieuPatients`
- 24/10/2026 — `FiltreCaptureEcran` (outil de capture d'écran, Debug)
- 23/10/2026 — recherche R3 : `UC_RecherchePaiements`, `SelecteurPeriode`, `FiltresPaiements` partagé avec le dossier
- 23/10/2026 — recherche R2 : `UC_RechercheSeances`, `FiltresSeances` partagé avec le dossier
- 23/10/2026 — recherche transversale : `QueryRecherche`, `Metier/Recherche`, `UI/Controls/Recherche`, ADR-52
- 23/10/2026 — `PolitiqueMotDePasse`, audit C.2
- 23/10/2026 — `ManqueDomaine` : complétude d'un domaine dans le métier, audit C.2
- 20/10/2026 — `CorrecteurWindows`, `CorrecteurTexte`, `AltheaZoneTexte` ; logos des domaines : `CheminsDomaineHelper`, `LogosDomaines`
- 19/10/2026
  - textes types : `UC_TextesTypes`, `UC_UsagesTextesTypes`, `GestionTextesTypes`, `QueryTextesTypes`, `TexteType`, `UsagesTexteType`
  - `EnTeteDocument`
  - `EditeurTexteAgrandi`
  - `GardeModifications`
  - refonte de l'éditeur : `UC_RichTextEditorSimple` supprimé, profil Compact de `UC_RichTextEditor`
  - `AltheaRenduBarre`
- 18/10/2026 — `LicenceSyncfusion`, `GestionLicences` ; UI-8 : `ApplicationEvents.vb`, `UtilsString` et l'ancien `UC_Domaines` supprimés
- 17/10/2026 — `ControleClotureDossier`, `ResumeFiltre` partagé, aperçu patient
- 14/10/2026 — kit d'interface : `AltheaEtapes`, `AltheaIconeEtat`, `AltheaDate`, `AltheaCalendrier`, `AltheaFondFenetre`, `AltheaBarreTitre` ; `DialogChoix` V2 ; paramètres lus par le code, sauvegarde et restauration

</details>

## 1. Localisation

#### Racine

| Élément | Rôle |
|---|---|
| `Althea.vbproj` | Projet WinForms SDK .NET, cible `net8.0-windows7.0`. |
| `Program.vb` | Point d’entrée applicatif (`Sub Main`, `StartupObject`). |
| `App.config` / `My Project/` | Configuration et ressources générées par Visual Studio. |
| `.github/` | Instructions de contribution/outillage. |

#### `Core/` — infrastructure technique

| Sous-répertoire | Contenu principal |
|---|---|
| `Configuration/` | `ConfigManager`, `LocalDbConfig` : configuration locale chiffrée. |
| `Database/` | `DatabaseManager`, `DbHelper` et `Queries/Query*.vb` : accès MariaDB et SQL centralisé. |
| `Logging/` | `GestionLog` : journalisation structurée. |
| `Security/` | Rôles, session, DPAPI, PBKDF2. |
| `Startup/` | `AppStartupManager` : démarrage bloquant et vérifications préalables (connexion, version de la base) ; `GestionErreursGlobales` : filet des erreurs non interceptées. |

#### `Metier/` — modèles et règles métier

| Sous-répertoire | Responsabilités |
|---|---|
| `Patients/` | Patient, contacts familiaux, intervenants, recherche et fiche. |
| `Dossiers/` | Dossier, liste, zones cliniques, cycle de vie. |
| `Seances/` | Séance, liste/planning, contrôle de chevauchement, décompte des quotas de domaine. |
| `Paiements/` | Paiement, liste contextuelle, séances non soldées, avoirs. |
| `Tarification/` | Résolution du tarif à la date de la séance selon domaine et dossier. |
| `Referentiels/` | Domaines, organismes, natures, prix, liaisons domaine–tarif/nature, catégories et règles de quota, statuts et autres référentiels ; `CodesSysteme`, point unique des codes système (statuts de séance, de dossier, de paiement, types et sources de document, ADR-54). |
| `Documents/` | Modèle et gestion des traces documentaires. |
| `Security/` | Authentification et gestion des utilisateurs. |
| `Validation/` | Erreur de saisie trouvée par un valideur métier (`ErreurSaisie`) et refus d'écriture qui la porte (`ErreurValidationException`) — ADR-51. |
| `Parametres/` | Paramètres applicatifs. |

#### `UI/` — interface WinForms

| Sous-répertoire | Contenu principal |
|---|---|
| `Forms/` | Form principale `Home`, connexion, configuration, modales communes et éditeurs patients/dossiers/utilisateurs. |
| `Forms/Dossiers/` | `SeanceEditor`, `PaiementEditor`, `PlanningJourViewer`, `DossierStatutEditor`, `DossierZoneEditor`, utilisation de l'avoir et report. |
| `Forms/Domaines/` | `CategoriesQuotaEditor` : catégories de quota d’un domaine. |
| `Controls/Patients/` | `UC_PatientHome`, `UC_ApercuPatient`, `UC_PatientFiche`. |
| `Controls/Dossiers/` | `UC_DossierFiche` : identité, suivi clinique, séances et paiements du dossier. |
| `UC_DomainesHome` : écran unique de paramétrage d’un domaine-convention (5 onglets + assistant) ; `UC_SyntheseDomaine` : synthèse mise en forme d’un domaine (partagée avec `UC_DossierFiche`). |
| `Controls/Referentiels/` | Hub, base commune et écrans des référentiels (domaines transitoire, organismes, natures, prix, liaisons, etc.). |
| `Controls/Administration/` | Hub administration, cabinet, paramètres, sauvegardes, utilisateurs. |
| `Controls/Theme/` | Kit d'interface Althéa (contrôles dessinés de la refonte UI, ADR-30 à ADR-34). |
| `Controls/Communs/` | Éditeurs RTF complet et allégé, barre de pagination `UC_Pagination`, résumé cliquable `ResumeFiltre` (pastilles qui filtrent une grille). |
| `Navigation/` / `Context/` | Navigation centralisée, historique et contexte partagé des écrans. |

#### `Utils/` — services transverses

| Sous-répertoire | Responsabilités |
|---|---|
| `Database/` | `UtilsDataReader` : lecture tolérante des colonnes optionnelles d’un `DbDataReader`. |
| `Helpers/` | Chemins patient/dossier et export RTF. |
| `UI/` | Thème, boutons, grilles, icônes, validation, chaînes et téléphone. |

## Conventions de localisation

- **SQL** : uniquement dans `Core/Database/Queries/`.
- **Accès base** : uniquement via `DatabaseManager` depuis les services métier.
- **Règles métier** : dans `Metier/`, jamais dans un événement UI.
- **Écrans principaux** : UserControls ; **interactions ponctuelles** : Forms modales.
- Chaque Form ou UserControl avec designer possède normalement le triplet `.vb`, `.Designer.vb`, `.resx`.

## Structure du projet Althea

```text
Althea/
│
├── Program.vb                           → Point d’entrée (Sub Main)
├── App.config / Althea.vbproj           → Configuration .NET et projet WinForms
├── .editorconfig                        → Conventions d'écriture et niveaux des analyseurs (ADR-57)
├── README.md / CHANGELOG.md              → Portail et historique du projet
├── My Project/                           → Propriétés et ressources générées VB.NET
├── .github/                              → Instructions et outillage ; workflows/ci.yml (intégration continue, ADR-59)
├── Tools/                                → build.cmd (compilation), test.cmd (tests automatisés) ; journaux dans obj/
├── Tests/Althea.Tests/                   → Tests automatisés (xUnit, VB) : projet séparé, exclu de la compilation de l'application (ADR-58)
│
├── Core/                                 → Infrastructure technique
│   ├── Configuration/
│   │   ├── ConfigManager.vb
│   │   ├── LicenceSyncfusion.vb         → clé de licence Syncfusion (DPAPI, poste)
│   │   ├── LocalDbConfig.vb
│   │   └── MotDePasseSauvegarde.vb      → mot de passe des archives (DPAPI, poste)
│   ├── Database/
│   │   ├── DatabaseManager.vb
│   │   ├── VersionSchema.vb             → version de base attendue par l'application
│   │   ├── DbHelper.vb
│   │   └── Queries/
│   │       ├── QueryPatients.vb / QueryDossiers.vb / QuerySeances.vb
│   │       ├── QueryPaiements.vb / QueryTarification.vb
│   │       ├── QueryDomaines.vb / QueryNaturesSeance.vb / QueryPrixSeance.vb
│   │       ├── QueryModesPaiement.vb / QueryQuotaInami.vb / QueryQuotasInami.vb
│   │       ├── QueryOrganismes.vb / QueryCategoriesQuota.vb
│   │       ├── QueryLiaisonsPrixDomaines.vb / QueryLiaisonsNaturesDomaines.vb
│   │       ├── QueryFamilleContacts.vb / QuerySuivisIntervenants.vb / QueryTherapeutes.vb
│   │       ├── QueryRecherche.vb          → Recherche transversale (ADR-52)
│   │       ├── QueryParametres.vb / QueryUtilisateurs.vb
│   │       ├── QuerySauvegardes.vb / QueryRestaurations.vb / QueryEtatSysteme.vb
│   │       └── Query<référentiel>.vb     → Liens, rôles, statuts, situations et types
│   ├── Logging/GestionLog.vb, LecteurJournaux.vb, ModelesJournaux.vb
│   ├── Security/AppRole.vb, CryptoManagerDPAPI.vb, PasswordSecurityHelper.vb, UserSession.vb
│   └── Startup/AppStartupManager.vb, GestionErreursGlobales.vb
│
├── Metier/                               → Modèles et règles fonctionnelles
│   ├── Patients/                          → Patient, contacts, intervenants et services associés
│   ├── Dossiers/                          → Dossier, liste, zones cliniques et cycle de vie
│   ├── Seances/                           → Séances, planning et informations de quota
│   ├── Paiements/                         → Paiements, affectations, solde et avoirs
│   ├── Tarification/GestionTarification.vb
│   ├── Referentiels/                      → Domaines, organismes, natures, prix, modes, catégories/règles/plafonds de quota, liaisons et référentiels généraux
│   ├── Documents/                         → Document, gestion documentaire, EnTeteDocument
│   ├── Parametres/                        → Paramètres applicatifs, licences du poste (GestionLicences)
│   ├── Recherche/                         → Recherche transversale : GestionRecherche, ResultatRecherche, LieuPatients, PersonneReseau (ADR-52)
│   ├── Sauvegardes/                       → Sauvegarde et restauration complètes (base + dossier de données), journaux, outils MariaDB
│   ├── Security/                          → Authentification et utilisateurs
│   ├── Systeme/                           → État du système (Administration)
│   └── Validation/                        → ErreurSaisie, ErreurValidationException (valideurs métier, ADR-51)
│
├── UI/                                   → Interface WinForms
│   ├── Context/UserControlContext.vb, SuiviActivite.vb, FiltreCaptureEcran.vb (Debug)
│   ├── Navigation/                        → NavigationManager, NavigationEntry et contrats de contexte
│   ├── Controls/
│   │   ├── UC_Accueil.vb
│   │   ├── Administration/                → UC_AdminHome, UC_Cabinet, UC_Parametres, UC_Sauvegardes, UC_Utilisateurs,
│   │   │                                    UC_Journaux, UC_EtatSysteme
│   │   ├── Communs/                       → UC_RichTextEditor (profils Complet / Compact), UC_Pagination, ResumeFiltre,
│   │   │                                    FiltresSeances, FiltresPaiements (relevé, axes, filtrage et résumé : dossier et recherche)
│   │   ├── Patients/                      → UC_PatientHome, UC_ApercuPatient, UC_PatientFiche
│   │   ├── Dossiers/                      → UC_DossierFiche
│   │   ├── Domaines/                      → UC_DomainesHome, UC_SyntheseDomaine
│   │   ├── Recherche/                     → UC_RechercheHome (hub), UC_RechercheSeances, UC_RecherchePaiements (d'une période),
│   │   │                                    UC_RecherchePatientsLieu (patients par lieu), UC_RechercheReseau (réseau par nom),
│   │   │                                    SelecteurPeriode (période des recherches), OuvertureResultat (aller à un élément trouvé)
│   │   ├── Theme/                         → kit d'interface Althéa : AltheaEnTete, AltheaCarte, AltheaBouton, AltheaOnglets,
│   │   │                                    AltheaJauge, AltheaRecherche, AltheaBarreActions, AltheaBarreOutils, AltheaRenduBarre, AltheaTuile, AltheaEtapes, AltheaIconeEtat, AltheaDate, AltheaCalendrier, AltheaZoneTexte, AltheaFondFenetre, AltheaBarreTitre…
│   │   └── Referentiels/                  → Hub, base, domaines (transitoire), organismes, liaisons, natures, prix, modes et référentiels généraux
│   └── Forms/
│       ├── Home.vb
│       ├── Communs/                       → DialogChoix, ReferentielModalHost, EditeurTexteAgrandi
│       ├── Database/                      → ConfigurationConnexion
│       ├── Login/                         → Login, ChangePassword, ElevationAcces
│       ├── Patients/                      → ContactEdition, IntervenantEdition, TherapeuteEdition
│       ├── Dossiers/                      → DossierStatutEditor, DossierZoneEditor, SeanceEditor, PlanningJourViewer,
│       │                                    PaiementEditor, DialogUtilisationAvoir, DialogReportSeance
│       ├── Domaines/                      → CategoriesQuotaEditor
│       ├── Sauvegardes/                   → DialogRestauration
│       ├── Utilisateur/                   → UtilisateurEdition
│       └── Test/                          → TestRichTextEditor, TestKitAlthea (vitrine du kit), TestDialogChoix, TestChampDate (démonstrations, Debug)
│
└── Utils/                                 → Utilitaires transverses
    ├── Database/                          → UtilsDataReader
    ├── Helpers/                           → ArborescenceDonnees, Chemins*Helper (patient, dossier, cabinet, utilisateur), ImagesHelper, RichTextEditorHelper, UtilsTexte
    └── UI/                                → UITheme, boutons, contrôles, grilles, icônes, chaînes, téléphone, validation

Docs/ et Assets/ sont volontairement exclus de cet arbre : leurs contenus sont documentés dans leurs références dédiées.

Le projet de tests `Tests/Althea.Tests` (dans `Althea.slnx`, dossier « Tests ») reprend les dossiers de l'application : `Utils/`, `Core/`, `Metier/`, `UI/`, une classe de tests `<Module>Tests` par module testé. Il est décrit dans `Docs/Tests/STRATEGIE_TESTS_ALTHEA.md`.
```



## Détails des composants

#### Forms

| Noms | Usage | Emplacement |
|---|---|---|
| `Home` | Shell principal : navigation, contexte UI et hébergement des UserControls. | `/UI/Forms` |
| `ConfigurationConnexion` | Configuration locale et test de connexion MariaDB. | `/UI/Forms/Database` |
| `Login` / `ChangePassword` / `ElevationAcces` | Authentification, changement de mot de passe et élévation temporaire. | `/UI/Forms/Login` |
| `UtilisateurEdition` | Création, consultation et administration d’un compte. | `/UI/Forms/Utilisateur` |
| `DialogChoix` / `ReferentielModalHost` | Dialogue applicatif standard et hôte d’ajout contextuel de référentiel. | `/UI/Forms/Communs` |
| `EditeurTexteAgrandi` | « Ouvrir en grand » d'un éditeur au profil Compact (même texte, profil Complet, Valider le reporte). | `/UI/Forms/Communs` |
| `ContactEdition` / `IntervenantEdition` / `TherapeuteEdition` | Édition du réseau patient et des thérapeutes. | `/UI/Forms/Patients` |
| `DossierStatutEditor` / `DossierZoneEditor` | Transition de statut et édition d’une zone clinique. | `/UI/Forms/Dossiers` |
| `SeanceEditor` / `PlanningJourViewer` / `DialogReportSeance` | Édition d’une séance, consultation du planning et report. | `/UI/Forms/Dossiers` |
| `DialogRestauration` | Restauration d'une archive (Admin) : choix, vérification, confirmation, progression, historique. | `/UI/Forms/Sauvegardes` |
| `PaiementEditor` / `DialogUtilisationAvoir` | Saisie et modification des paiements ; utilisation de l'avoir d'une séance devenue non due. | `/UI/Forms/Dossiers` |
| `CategoriesQuotaEditor` | Ajout, activation et suppression des catégories de quota d’un domaine. | `/UI/Forms/Domaines` |
| `TestRichTextEditor` | Formulaire de test manuel de l’éditeur riche. | `/UI/Forms/Test` |
| `TestDialogChoix` | Démonstration de tous les cas de `DialogChoix` V2 et des icônes d'état (tests, captures). | `/UI/Forms/Test` |
| `TestChampDate` | Démonstration du champ de date du kit (`AltheaDate`) et du calendrier seul (`AltheaCalendrier`). | `/UI/Forms/Test` |

#### Modules et services

| Noms | Usage | Emplacement |
|---|---|---|
| `ConfigManager` / `DatabaseManager` / `DbHelper` / `AppStartupManager` | Configuration locale, accès MariaDB, helpers de données et démarrage contrôlé. | `/Core` |
| `GestionLog` | Journalisation applicative. | `/Core/Logging` |
| `CryptoManagerDPAPI` / `PasswordSecurityHelper` | Protection des secrets locaux et mots de passe PBKDF2. | `/Core/Security` |
| `QueryPatients`, `QueryDossiers`, `QuerySeances`, `QueryPaiements` | Requêtes SQL des flux patients, dossiers, séances et paiements. | `/Core/Database/Queries` |
| `QueryTextesTypes` | Requêtes des textes types et de leurs usages (v2.42.0). | `/Core/Database/Queries` |
| `QueryTarification`, `QueryQuotaInami`, `QueryQuotasInami` | Requêtes de résolution tarifaire, de décompte des quotas et des règles/plafonds (`ref_quotas`, `ref_quotas_plafonds`). | `/Core/Database/Queries` |
| `QueryOrganismes`, `QueryCategoriesQuota` | Organismes de remboursement et catégories de quota par domaine. | `/Core/Database/Queries` |
| `QueryDomaines`, `QueryNaturesSeance`, `QueryPrixSeance`, `QueryModesPaiement` | Requêtes des référentiels spécialisés. | `/Core/Database/Queries` |
| `QueryLiaisonsPrixDomaines`, `QueryLiaisonsNaturesDomaines` | Requêtes des liaisons paramétrables par domaine. | `/Core/Database/Queries` |
| `QueryFamilleContacts`, `QuerySuivisIntervenants`, `QueryTherapeutes` | Requêtes du réseau patient et thérapeutes. | `/Core/Database/Queries` |
| `QueryParametres`, `QueryUtilisateurs`, `Query<référentiel>` | Paramètres, sécurité et référentiels généraux. | `/Core/Database/Queries` |
| `QuerySauvegardes`, `QueryRestaurations` | Journaux `tec_sauvegardes` / `tec_restaurations`, version du schéma, inventaire des objets à vider, droits du compte d'administration. | `/Core/Database/Queries` |
| `GestionPatients`, `GestionFamilleContacts`, `GestionSuivisIntervenants`, `GestionApercuPatient` | Cas d’usage du patient et de son réseau ; aperçu du patient (assemblé à partir des autres modules). | `/Metier/Patients` |
| `GestionDossiers`, `GestionSeances`, `GestionQuotaInami` | Dossiers, séances, chevauchements et consommation des quotas. | `/Metier/Dossiers`, `/Metier/Seances` |
| `GestionPaiements` | Paiements, affectations, solde et avoirs. | `/Metier/Paiements` |
| `GestionTarification` | Résolution du tarif à la date de l’événement. | `/Metier/Tarification` |
| `GestionDomaines`, `GestionNaturesSeance`, `GestionPrixSeance`, `GestionModesPaiement`, `GestionQuotasInami` | Référentiels spécialisés de comportement, tarification, paiement et règles de quota (tout domaine, nom historique conservé) ; `GestionDomaines` porte aussi la validation d'un domaine (`ValiderDomaine`) et sa complétude (`CalculerManques`). | `/Metier/Referentiels` |
| `GestionOrganismes`, `GestionCategoriesQuota` | Organismes (lignes système protégées) et catégories de quota d’un domaine. | `/Metier/Referentiels` |
| `GestionLiaisonsPrixDomaines`, `GestionLiaisonsNaturesDomaines` | Maintenance des liaisons domaine–prix et domaine–nature. | `/Metier/Referentiels` |
| `GestionLiensPatient`, `GestionRolesIntervenant`, `GestionRoleLegal`, `GestionSituationsFamiliales`, `GestionStatutsDossier`, `GestionStatutsSeance`, `GestionTherapeutes`, `GestionTypesDocuments`, `GestionTypesRendezVous`, `GestionTextesTypes` | Services des référentiels généraux (textes types : v2.42.0). | `/Metier/Referentiels` |
| `GestionSauvegardes` | Sauvegarde complète en archive ZIP AES-256 (mariadb-dump + dossier de données + manifeste), rotation, journal `tec_sauvegardes`, rappel, mot de passe des archives. | `/Metier/Sauvegardes` |
| `GestionRestaurations` | Restauration d'une archive : contrôles, sauvegarde de sécurité, documents, base vidée puis rechargée, remise en état en cas d'échec, journal `tec_restaurations`. | `/Metier/Sauvegardes` |
| `GestionEtatSysteme` | État du système par sections (application, base, dossiers, sauvegardes), sans exception ni secret ; rapport texte. | `/Metier/Systeme` |
| `OutilsMariaDB` | Recherche des outils `mariadb-dump` / `mariadb` et fichier d'options temporaire (compte transmis sans ligne de commande). | `/Metier/Sauvegardes` |
| `GestionParametres`, `GestionLicences`, `GestionAuthentification`, `GestionUtilisateurs`, `PolitiqueMotDePasse`, `PhotosUtilisateur`, `GestionDocuments` | Paramètres, licences du poste (clé Syncfusion), sécurité (dont la règle d’affichage de la photo d’un compte, les règles d'un compte et celle du mot de passe) et documents. | `/Metier` |
| `UITheme`, `GardeModifications`, `CorrecteurTexte`, `LogosDomaines`, `UtilsAffichage`, `UtilsButtons`, `UtilsControls`, `UtilsDataGrid`, `UtilsIcons`, `UtilsPastilles`, `UtilsTelephone`, `UtilsValidation`, `AffichageErreursSaisie`, `SaisieLocaliteBelge` (code postal / localité d'une adresse belge) | Conventions et comportements UI réutilisables (dont le correcteur d'orthographe d'une zone de texte : `CorrecteurTexte`). | `/Utils/UI` |
| `ArborescenceDonnees`, `CheminsPatientHelper`, `CheminsDossierHelper`, `CheminsCabinetHelper`, `CheminsUtilisateurHelper`, `CheminsDomaineHelper`, `ImagesHelper`, `RichTextEditorHelper`, `CorrecteurWindows` | Racine et sous-dossiers fixes du dossier de données (ADR-35), chemins déterministes (patients, dossiers, images du cabinet, photo des comptes), import d’image et services associés au texte riche (dont le correcteur d'orthographe de Windows). | `/Utils/Helpers` |
| `UtilsDataReader` | Lecture tolérante de colonnes optionnelles (ex. `est_systeme`). | `/Utils/Database` |

#### Classes et modèles

| Noms | Usage | Emplacement |
|---|---|---|
| `LocalDbConfig`, `UserSession`, `AuthenticationResult`, `UtilisateurApplication` (et `EtatCompteUtilisateur`) | Configuration locale, session et authentification ; état résumé d'un compte (actif, verrouillé, désactivé). | `/Core/Configuration`, `/Core/Security`, `/Metier/Security` |
| `Patient`, `PatientListeItem`, `FamilleContact`, `SuiviIntervenant`, `ApercuPatient` | Modèles patient et réseau de suivi ; contenu du panneau d'aperçu. | `/Metier/Patients` |
| `Dossier`, `DossierListeItem`, `ZoneDossier`, `ControleClotureDossier` | Modèles de dossier et zones cliniques ; résultat du contrôle avant clôture (séances à venir, montants à régler). | `/Metier/Dossiers` |
| `Seance`, `SeanceListeItem`, `SeancePlanningItem`, `LignePlanningJour`, `QuotaInamiInfo` | Séance, liste, planning (séances et créneaux libres du jour) et résultat de quota. | `/Metier/Seances` |
| `Paiement`, `PaiementListeItem`, `AvoirDisponible`, `EtatPaiementsSeance` | Modèle financier et données d’affichage. | `/Metier/Paiements` |
| `Domaine`, `ManqueDomaine` / `EtapeDomaine` (complétude), `Organisme`, `NatureSeance`, `PrixSeance`, `ModePaiement`, `QuotaInamiRegle`, `QuotaPlafond`, `CategorieQuota` | Référentiels spécialisés du parcours séance et des conventions. | `/Metier/Referentiels` |
| `LiaisonPrixDomaine`, `LiaisonNatureDomaine`, `ReferentielLigne` | Liaisons paramétrables et présentation générique de référentiel. | `/Metier/Referentiels` |
| `LienPatient`, `RoleIntervenant`, `RoleLegal`, `SituationFamiliale`, `StatutDossier`, `StatutSeance`, `Therapeute`, `TypeDocument`, `TypeRendezVous`, `TexteType` / `UsageTexteType`, `UsagesTexteType` (codes d'usage) | Modèles des référentiels généraux. | `/Metier/Referentiels` |
| `ParametreApplication`, `ClesParametres`, `Document` | Paramètres (dont les clés lues par le code et leurs valeurs de secours) et métadonnées documentaires. | `/Metier/Parametres`, `/Metier/Documents` |
| `EnTeteDocument` | En-tête et pied de page des documents imprimés ou exportés (cabinet, praticien, titre, patient, mention, date). | `/Metier/Documents` |
| `UserControlContext`, `NavigationManager`, `NavigationEntry` | Contexte et navigation UI. | `/UI/Context`, `/UI/Navigation` |
| `SuiviActivite` | Note l'activité (clavier, clic, molette) pour la fin de l'élévation après inactivité. | `/UI/Context` |
| `FiltreCaptureEcran` | Debug seulement : captures d'écran au clavier (Ctrl+Maj+F8 contenu, Ctrl+Maj+F7 fenêtre) pour les illustrations de la documentation. | `/UI/Context` |
| `Cabinet`, `Praticien`, `IdentifiantPraticien`, `QualificationPraticien`, `TypeIdentifiantPro`, `GestionCabinet`, `GenerateurCachet` | Modèle Cabinet/praticien, persistance et génération du cachet. | `/Metier/Cabinet` |
| `SituationJuridique`, `GestionSituationsJuridiques` | Référentiel des situations juridiques (`PorteurBce`). | `/Metier/Referentiels` |
| `GestionTypesIdentifiantPro` | Référentiel des types d'identifiant professionnel (seule lecture du référentiel, utilisée aussi par la fiche Cabinet). | `/Metier/Referentiels` |
| `QueryCabinet`, `QuerySituationsJuridiques`, `QueryTypesIdentifiantPro` | Requêtes SQL centralisées. | `/Core/Database/Queries` |
| `UtilsIdentifiantBelge` | Contrôles INAMI, BCE, TVA, IBAN, BIC ; présentation IBAN / BIC (`FormaterIban`, `IbanCompact`, `FormaterBic`) ; `EstControleParAlthea` (types dont le contrôle est écrit dans l'application). | `/Utils/UI` |
| `CheminsCabinetHelper` | Résolution des chemins des images cabinet/praticien (logo, photo, cachet, signature). | `/Utils/Helpers` |
| `ImagesHelper`, `UtilsImages` | Import d'image unifié : formats, contrôle, copie sous nom fixe, chargement sans verrou / dialogue, confirmation, affichage. | `/Utils/Helpers`, `/Utils/UI` |

#### Enums

| Noms | Usage | Emplacement |
|---|---|---|
| `AppRole` | Rôles globaux `User`, `SuperUser`, `Admin`. | `/Core/Security` |
| `TypeDialogue` | Types de dialogue de `DialogChoix`. | `/UI/Forms/Communs` |
| `EtatIcone` | États dessinés par `AltheaIconeEtat` (Information, Avertissement, Erreur, Question, Succès, Attente). | `/UI/Controls/Theme` |
| `ModeUtilisateurEdition`, `ModeChangePassword` | Modes des éditeurs utilisateur et mot de passe. | `/UI/Forms` |

#### UserControls

| Noms | Usage | Emplacement |
|---|---|---|
| `UC_Accueil` | Accueil applicatif. | `/UI/Controls` |
| `UC_AdminHome`, `UC_Parametres`, `UC_Utilisateurs` | Hub et écrans d’administration. | `/UI/Controls/Administration` |
| `UC_Journaux` | Consultation des journaux (Admin, lecture seule) : jour, filtres, détail. | `/UI/Controls/Administration` |
| `UC_EtatSysteme` | État du système (Admin) : sections, appréciations, copie du rapport. | `/UI/Controls/Administration` |
| `UC_Sauvegardes` | Sauvegarde (archive chiffrée), mot de passe des archives, historique ; implémente `IUnsavedChangesAware` (pas de sortie pendant une sauvegarde). | `/UI/Controls/Administration` |
| `UC_Cabinet` | Fiche Cabinet et praticien (onglets Cabinet, Praticien, Identifiants, Cachet) ; implémente `IUnsavedChangesAware`. | `/UI/Controls/Administration` |
| `UC_SituationsJuridiques` | Référentiel des situations juridiques (porteur du BCE). | `/UI/Controls/Referentiels` |
| `UC_TypesIdentifiantPro` | Référentiel des types d'identifiant professionnel (porté par, masque, expression de contrôle), ouvert depuis le hub Administration. | `/UI/Controls/Referentiels` |
| `UC_RichTextEditor` | Éditeur de texte riche, profils Complet et Compact (ADR-42). | `/UI/Controls/Communs` |
| `UC_Pagination` | Barre de pagination réutilisable des grilles longues. | `/UI/Controls/Communs` |
| `UC_PatientHome` | Hub de la journée : grille des patients (rendez-vous, suivi, paiements), résumé cliquable, filtres. | `/UI/Controls/Patients` |
| `UC_ApercuPatient` | Panneau d'aperçu du patient sélectionné (identité, alerte, séances, dossier, finances, contacts). | `/UI/Controls/Patients` |
| `UC_PatientFiche` | Fiche patient et onglets Identité, contacts, intervenants et dossiers ; fichiers partiels `.Famille`, `.Intervenants`, `.Dossiers` (ADR-56). | `/UI/Controls/Patients` |
| `UC_DossierFiche` | Hub dossier : fiche, suivi clinique, documents, séances et paiements ; fichiers partiels `.ZonesCliniques`, `.Seances`, `.Paiements` (ADR-56). | `/UI/Controls/Dossiers` |
| `UC_ReferentielBase`, `UC_ReferentielHome` | Base commune et hub des référentiels. | `/UI/Controls/Referentiels` |
| `UC_DomainesHome` | Écran de référence d’un domaine-convention : Général, Tarifs, Catégories & quotas, Natures, Synthèse + assistant. | `/UI/Controls/Domaines` |
| `UC_SyntheseDomaine` | Synthèse en lecture seule d’un domaine (convention, tarifs, natures, quota, complétude — règles de `GestionDomaines.CalculerManques`) ; utilisée par `UC_DomainesHome` et `UC_DossierFiche`. | `/UI/Controls/Domaines` |
| `UC_Organismes` | Référentiel des organismes de remboursement. | `/UI/Controls/Referentiels` |
| `UC_LiaisonsPrixDomaines`, `UC_LiaisonsNaturesDomaines` | Maintenance des relations datées domaine–prix et domaine–nature. | `/UI/Controls/Referentiels` |
| `UC_NaturesSeance`, `UC_PrixSeance`, `UC_ModesPaiement` | Référentiels spécialisés de séance et paiement. | `/UI/Controls/Referentiels` |
| `UC_LiensPatient`, `UC_RolesIntervenant`, `UC_RoleLegal`, `UC_SituationsFamiliales` | Référentiels du patient et de son réseau. | `/UI/Controls/Referentiels` |
| `UC_StatutsDossier`, `UC_StatutsSeance`, `UC_TypesDocuments`, `UC_TypesRendezVous`, `UC_TextesTypes`, `UC_UsagesTextesTypes` | Référentiels de cycle de vie, documents, agenda, textes types et leurs usages. | `/UI/Controls/Referentiels` |
| `UC_Therapeutes` | Entité riche des thérapeutes. | `/UI/Controls/Referentiels` |

#### Interfaces

| Noms | Usage | Emplacement |
|---|---|---|
| `IContextAwareUserControl` | Contrat de réception du contexte partagé par les UserControls. | `/UI/Navigation` |
| `IContextAwareForm` | Contrat de réception du contexte partagé par les Forms modales. | `/UI/Navigation` |
| `IUnsavedChangesAware` | Contrat de protection des modifications non enregistrées avant navigation (`NavigationManager`). | `/UI/Navigation` |
| `IFenetrePrincipale` | Ce que la fenêtre principale offre aux écrans et fenêtres (navigation, référentiels en fenêtre, fil d'Ariane, bandeau) ; implémenté par `Home`, fourni par `UserControlContext.FenetrePrincipale` (ADR-55). | `/UI/Navigation` |

## 



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



[TOC]

