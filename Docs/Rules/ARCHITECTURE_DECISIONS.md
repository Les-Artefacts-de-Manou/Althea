# Althéa - Architecture Decision Records (ADR)

> Ce document regroupe les décisions d'architecture majeures prises pour Althéa.  
> Chaque ADR explique le **contexte**, les **options envisagées**, la **décision retenue** et ses **conséquences**.  
> Une décision actée ne doit être remise en cause que via un nouvel ADR explicite.

> *Dernière mise à jour : 10/06/2026*
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

---

## Table des décisions

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
| [ADR-10](#adr-10--gestion-des-utilisateurs-avec-modes-dédition-distincts) | Gestion des utilisateurs avec modes d'édition distincts | Actée | `UI/Forms/Utilisateur/UtilisateurEdition.vb`, `Metier/Security/GestionUtilisateurs.vb` |
| [ADR-11](#adr-11--dialogchoix-remplacement-systématique-des-messagebox) | DialogChoix : remplacement systématique des MessageBox | Actée | `UI/Forms/Communs/DialogChoix.vb` |
| [ADR-12](#adr-12--centralisation-des-icônes-détat-via-utilsicons) | Centralisation des icônes d'état via UtilsIcons | Actée | `Utils/UI/UtilsIcons.vb` |
| [ADR-13](#adr-13--intégration-du-module-documents-v1-depuis-poc) | Intégration du module Documents V1 depuis POC | À instruire | `Docs/Poc/Gestion_documentaire_Althea.md`, `Docs/ETAT_DU_PROJET.md` |
| [ADR-14](#adr-14--intégration-du-module-agenda-v1-depuis-poc) | Intégration du module Agenda V1 depuis POC | À instruire | `Docs/Poc/Gestion_Calendrier_Althea.md`, `Docs/ETAT_DU_PROJET.md` |
| [ADR-15](#adr-15--composant-standard-uc_richtexteditor-pour-notes-formatées) | Composant standard UC_RichTextEditor pour notes formatées | Actée | `UI/Controls/Communs/UC_RichTextEditor.vb`, `Utils/Helpers/RichTextEditorHelper.vb`, `Docs/UC_RichTextEditor_Documentation.md` |
| [ADR-16](#adr-16--stockage-des-fichiers-hors-base-chemin-déterministe) | Stockage des fichiers hors base, chemin déterministe | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §8, `Docs/Poc/Gestion_documentaire_Althea.md` |
| [ADR-17](#adr-17--renommages-de-schéma-domaines-thérapeutes--réseau-dintervenants) | Renommages de schéma (domaines, thérapeutes) + réseau d'intervenants | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §3-§4 |
| [ADR-18](#adr-18--référentiels-uc-physique--classe-de-base-uc_referentielbase) | Référentiels : UC physique + classe de base `UC_ReferentielBase` | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §7.3 |
| [ADR-19](#adr-19--anticipation-multi-utilisateur-id_utilisateur-nullable) | Anticipation multi-utilisateur (`id_utilisateur` nullable) | Actée | `Docs/Conception/Plan_Conception_Metier_Althea.md` §12 BD-9 |

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

---

## ADR-11 - DialogChoix : remplacement systématique des MessageBox

**Date :** juin 2026  
**Statut :** Actée

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
**Statut :** Actée

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
**Statut :** Actée – Implémentée (V1.0 complète + variante Simple)

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

Licence **Syncfusion Community** (gratuite < 1M USD revenus/an) enregistrée dans `Program.vb` :
```vb
SyncfusionLicenseProvider.RegisterLicense("VOTRE-CLE-SYNCFUSION")
```

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

- **[UC_RichTextEditor_Documentation.md](../UC_RichTextEditor_Documentation.md)** : Guide complet (589 lignes)
- **[Historique_Implementation_RichTextEditor.md](../Historique_Implementation_RichTextEditor.md)** : Journal technique V1.0-V1.6 (468 lignes)
- **[Documentation_technique_UI_Althea.md](../UI/Documentation_technique_UI_Althea.md)** : Section UC_RichTextEditor complète
- **[PLAN_TESTS_UC_RICHTEXTEDITOR.md](../Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md)** : Plan de tests exhaustif (479 lignes)
- **[Guide_Licence_Syncfusion.md](../Guide_Licence_Syncfusion.md)** : Obtention licence Community gratuite

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

> **Contact** : ***Joëlle (Manou)  - Les Artefacts de Manou***
>
> Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.
>
> - Site web P.Nguyen Duy:  https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu`
>
> - GitHub privé: Althea    https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea.git
>
> ---

[TOC]

