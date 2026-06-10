# Althéa - README

> *Dernière mise à jour : 10/06/2026*

## Application de gestion des dossiers patients

**Althéa** est une application professionnelle dédiée à la gestion des dossiers patients dans le cadre d'une pratique psychologique et thérapeutique.

Elle vise à fournir un outil **fiable, structuré et sécurisé**, permettant de centraliser l'ensemble des informations nécessaires au suivi des patients, tout en restant simple et agréable à utiliser.

---

## Origine du nom

**Althéa** est un prénom féminin d'origine grecque (Ἀλθαία – *Althāía*), dérivé du verbe *altho* signifiant :
« guérir » ou « être en bonne santé »

Il peut être interprété comme : **« celle qui guérit »**

Son nom est parfois interprété comme portant la promesse de réconfort et de protection.

Ce nom incarne parfaitement l'objectif de l'application : accompagner le travail thérapeutique avec rigueur, douceur et efficacité.

---

### Icône

 <img src="Assets/Appli_Ico/Althea_A _96x96.ico" alt="Althea" style="zoom:100%;" />

 ### Splash

  <img src="Assets/Splash/Splash_Althea_Main_from_image.png" alt="Splash Althea" style="zoom:80%;" />

---

## Objectifs

- Gestion complète des **patients et dossiers**
- Suivi des **séances et de l'évolution**
- Centralisation des **documents** (bilans, comptes-rendus, anamnèses, courriers…)
- Gestion des **paiements**
- Accès rapide aux informations essentielles
- Respect strict de la **confidentialité**

---

## Organisation métier

Le modèle repose sur une structure claire :

- **Patient** → personne
- **Dossier** → contexte rattaché à un **domaine** (psychologie, graphothérapie…)
- **Séance** → unité de suivi (créée dès la planification du rendez-vous lié)
- **Paiement** → encaissement
- **Document** → fichier externe lié (stocké hors base de données, à chemin déterministe)
- **Thérapeute / réseau d'intervenants** → suivis externes associés au patient (relation N-N)

Relations :

- 1 patient → n dossiers  
- 1 dossier → n séances  
- 1 paiement → n séances  
- 1 patient ↔ n intervenants externes (réseau de suivi)

> 📐 Le cadrage détaillé de la phase métier (modèle de données, cycle de vie des dossiers, intégration documents/agenda) est figé dans le [plan de conception métier](Docs/Conception/Plan_Conception_Metier_Althea.md) et les [décisions d'architecture](Docs/Rules/ARCHITECTURE_DECISIONS.md).

---

## Architecture technique

#### Application

- VB.NET
- .NET 8 LTS
- Windows Forms

#### Base de données

- MariaDB
- UTF8MB4
- séparation stricte :
  - données métier
  - données techniques
  - données UI

#### Principes fondamentaux

- point d'accès DB unique (`DatabaseManager`)
- aucune connexion sauvage
- logique métier séparée
- code lisible et explicite

---

## Environnement technique

### Outils de développement

| Outil | Usage | Version | Date |
| ----- | ----- | ----- | ----- |
| **Visual Studio Community** | IDE principal - VB.NET / .NET 8 LTS / WinForms | 18.6.2 | Mai 2026 |
| **MariaDB** | Base de données (charset `utf8mb4`, collation `utf8mb4_uca1400_ai_ci`, base `Althea`) | 12.3.2 | 28 mai 2026 |
| **HeidiSQL** | Administration et visualisation de la base MariaDB | 12.17.0 | 21 mai 2026 |
| **DBeaver** | Visualisation de la base MariaDB, génération de diagrammes | 26.1.0 | 01/06/26 |
| **Typora** | Rédaction de la documentation Markdown | 1.13.7 | 10/06/26 |
| **LordIcon** | Icônes pour l'interface utilisateur – https://lordicon.com/ |  |  |

> 

### Packages NuGet

| Package | Version | Date |  |
| ------- | ------- | ----- | ------- |
| **MySqlConnector** | 2.6.0 | 03/06/26 | Bradley Grainger - <br />A truly async MySQL ADO.NET provider, supporting MySQL Server, MariaDB, Amazon Aurora, Azure Database for MySQL, Google Cloud SQL, and more.<br />https://mysqlconnector.net/ |
| **Syncfusion.DocIO.WinForms** | 33.2.12 | 10/06/26 | Avec licence gratuite. The Syncfusion® [WinForms Word library](https://www.syncfusion.com/word-framework/net/word-library?utm_source=nuget&utm_medium=listing&utm_campaign=winforms-docio-nuget) (Essential® DocIO) is a feature-rich and high-performance .NET Word library that is used to create, read, and edit Word documents programmatically without Microsoft Office dependencies. |
| **Syncfusion.DocToPDFConverter.WinForm** | 33.2.12 | 10/06/26 | The Syncfusion® [WinForms Word library](https://www.syncfusion.com/word-framework/net/word-library?utm_source=nuget&utm_medium=listing&utm_campaign=winforms-doctopdfconverter-nuget) (Essential® DocIO) converts a [Word document to PDF](https://www.syncfusion.com/word-framework/net/word-to-pdf-conversion?utm_source=nuget&utm_medium=listing&utm_campaign=winforms-doctopdfconverter-nuget) with just five lines of code and also it does not require Adobe and Microsoft Word application to be installed in the machine. It preserves the original appearance of the Word document in the converted PDF document |

### Versioning

- **GitHub (privé)** : https://github.com/AngeljoNG/Althea - mis à jour en continu
- **GitHub (public)** : https://github.com/Les-Artefacts-de-Manou/Althea.git 
---

## Architecture UI

### Principe fondamental

- 1 seule Form principale (Home)
- Des UserControls injectés dynamiquement
- Aucun accès direct à la base depuis l'UI
- Aucun traitement métier dans les Forms ou UserControls
- Toute navigation passe par `NavigationManager`

### Structure

- `Home` = Shell applicatif
- `NavigationManager` = gestion centralisée
- `UserControls` = écrans métier

### Navigation

- chargement dynamique dans `pnlContent`
- un seul écran actif
- navigation centralisée via `NavigationManager`

### Organisation fonctionnelle

- Menu principal = métier
- Admin = technique (config, outils)

### Context UI centralisé

L'ensemble des UserControls repose désormais sur un système de contexte UI partagé permettant :

- la gestion centralisée des messages utilisateur (`stsStatus`)
- la gestion homogène des erreurs de validation (`errProvider`)
- la gestion des aides contextuelles (`ttMain`)
- la mise à jour cohérente du contexte écran (`lblContexte`)

Cette architecture évite les accès directs aux contrôles globaux depuis les UserControls et garantit un comportement homogène dans toute l'application.

Le contexte est encapsulé dans `UserControlContext`.

---

## Gestion des utilisateurs

L'application intègre un système complet de gestion des utilisateurs applicatifs avec :

### Fonctionnalités principales

- **Création d'utilisateurs** : génération automatique de mot de passe temporaire, configuration des rôles
- **Modification d'utilisateurs** : mise à jour des informations, gestion des états de sécurité
- **Consultation** : accès en lecture seule pour les SuperUsers
- **Actions de maintenance** :
  - Réinitialisation de mot de passe (via formulaire centralisé `ChangePassword`)
  - Déverrouillage de compte
  - Activation/désactivation de compte
- **Recherche et filtrage multi-critères** :
  - Recherche textuelle (login, nom affichage, code)
  - Filtrage par rôle (User, SuperUser, Admin)
  - Filtrage par date de dernier login
  - Filtrage des comptes verrouillés uniquement
  - Bouton de réinitialisation des filtres

### Sécurité

- Contrôle d'accès par rôle (Admin, SuperUser, User)
- Validation exhaustive des formulaires
- Gestion des états de verrouillage
- Journalisation détaillée de toutes les actions de sécurité
- Mots de passe hashés via PBKDF2-SHA256
- Flag `must_change_password` pour forcer le changement à la première connexion

### Modules impliqués

- `UI/Controls/UC_Utilisateurs.vb` : UserControl de liste et gestion
- `UI/Forms/Utilisateur/UtilisateurEdition.vb` : Form modale d'édition (Création, Modification, Consultation)
- `Metier/Security/GestionUtilisateurs.vb` : Logique métier centralisée
- `UI/Forms/Login/ChangePassword.vb` : Formulaire de changement de mot de passe (UserChange / AdminReset)
- `Core/Database/Queries/QueryUtilisateurs.vb` : Requêtes SQL centralisées

---

## Gestion des paramètres

L'application intègre un système de paramètres centralisés basé sur la table `tec_parametres`.

Ces paramètres permettent de configurer dynamiquement :

- les chemins de stockage
- les comportements applicatifs
- certaines options fonctionnelles

Les paramètres sont organisés par groupe et typés, permettant une génération dynamique de l'interface utilisateur.

La gestion est sécurisée :

- aucune suppression physique
- modification contrôlée selon le niveau d'accès
- séparation entre paramètres techniques et paramètres métier

#### UC_Parametres

Le module **UC_Parametres** constitue désormais une référence pour les futurs UserControls.

Il intègre :

- une gestion complète du contexte UI (header + status)
- une validation métier avancée et centralisée
- une normalisation stricte des données techniques
- une architecture claire (UI / métier / DB)
- une gestion propre des erreurs (errProvider + logs)
- UC_Parametres n'est accessible qu'aux rôles de SuperUser (limité) et Admin (complet)

Ce module sert de **modèle de conception** pour l'ensemble de l'application.

---

## Interface utilisateur personnalisée

### DialogChoix - Boîte de dialogue personnalisée

L'application dispose d'un composant de dialogue personnalisé (`UI/Forms/Communs/DialogChoix.vb`) remplaçant les MessageBox standards de Windows Forms.

**Avantages** :

- Cohérence visuelle avec la charte graphique Althéa
- Support des icônes GIF animées
- Configuration flexible de 1 à 3 boutons
- Thématisation complète via `UITheme`
- Taille adaptative selon le contenu

**Types de dialogue** :

- `Information` : messages informatifs
- `Warning` : avertissements
- `Error` : erreurs
- `Question` : questions à l'utilisateur
- `Success` : confirmations de succès
- `Loading` / `Processing` : opérations en cours

**Utilisation** :

```vb
' Méthodes statiques simples
DialogChoix.Information("Opération réussie")
DialogChoix.Erreur("Une erreur s'est produite")

' Confirmation Yes/No
If DialogChoix.Confirmer("Continuer ?") = DialogResult.Yes Then
	' action
End If

' Configuration avancée
Dim dlg As New DialogChoix()
dlg.Titre = "Suppression"
dlg.Message = "Confirmer la suppression ?"
dlg.TypeDialogue = TypeDialogue.Warning
dlg.SetBoutons("Supprimer", "Annuler")
If dlg.ShowDialog() = DialogResult.Yes Then
	' action
End If
```

**Remplacement systématique** : tous les `MessageBox.Show` de l'application UI ont été remplacés par `DialogChoix` (34 occurrences au total).

---

## UC_RichTextEditor - Éditeur de texte riche

L'application intègre un **éditeur de texte riche réutilisable** (`UI/Controls/UC_RichTextEditor.vb`) pour la saisie et le formatage des notes patients, anamnèses, bilans psychologiques/graphothérapeutiques, et comptes-rendus de consultations.

### Fonctionnalités principales

**Toolbar complète (30 boutons)** :

- **Formatage caractères** : Gras (Ctrl+B), Italique (Ctrl+I), Souligné (Ctrl+U), Barré, Couleur texte, Surbrillance
- **Formatage paragraphes** : Alignement (gauche/centré/droite), Listes à puces, Retraits
- **Polices et tailles** : 9 polices courantes, 14 tailles (8 à 72 points)
- **Actions** : Couper, Copier, Coller, Annuler, Rétablir, Effacer formatage
- **Insertion** : Date/heure courante
- **Outils** : 
  - **📄 Mise en page** : Configuration format papier (A4/A3/Letter), marges, orientation
  - **🖨️ Impression** : Dialogue système Windows, pagination automatique, formatage RTF préservé
  - **📑 Export PDF** : Conversion RTF → PDF avec Syncfusion (archivage, envoi)
  - **📝 Export Word** : Conversion RTF → .docx éditable (collaboration, modification)

**Modes d'utilisation** :

- Mode édition complet (toolbar visible)
- Mode lecture seule (`ReadOnlyMode`) masquant automatiquement la toolbar
- Affichage/masquage manuel de la toolbar (`ShowToolbar`)

**Sauvegarde intelligente** :

- **RTF** : Formatage complet préservé pour affichage
- **TXT** : Texte brut pour recherche full-text SQL au niveau Dossier/Patient

### Impression et exports avancés

**Système d'impression Win32 natif** :

- API `EM_FORMATRANGE` préservant le formatage RTF complet
- Dialogue système Windows (`PrintDialog`) pour sélection imprimante
- Configuration mise en page complète (format, marges, orientation)
- Aperçu optionnel via `AfficherApercu()`
- Pagination automatique avec respect des marges

**Export PDF (Syncfusion)** :

- Conversion native RTF → PDF via Syncfusion.DocIO + DocToPDFConverter
- Formatage préservé : gras, couleurs, alignement, puces, polices, retraits
- Usage : Archivage, envoi par email, documents finalisés non modifiables
- Licence Community gratuite (revenus < 1M USD)

**Export Word (.docx)** :

- Conversion native RTF → DOCX éditable via Syncfusion.DocIO
- **Intégration complète avec POC système documentaire** :
  - Architecture locale : `Patients/{id_patient}/{id_dossier}/Documents/`
  - Synchronisation automatique Google Drive
  - Édition possible avec Word (local) ou Google Docs (cloud)
  - Conversion PDF automatique pour archivage (le .docx est la source)
- Usage : Collaboration (écoles, médecins, parents), documents évolutifs, templates réutilisables

**Différence PDF vs Word** :

| Critère | PDF (📑) | Word (📝) |
|---------|----------|----------|
| **Édition** | ❌ Non éditable | ✅ Éditable |
| **Usage** | Archivage, envoi, impression | Collaboration, modification |
| **Compatibilité** | Universel (lecture seule) | Office + Google Docs |
| **Gestion Althéa** | Dérivé local uniquement | Source principale (POC) |

### Architecture technique

**Helper centralisé** (`Utils/Helpers/RichTextEditorHelper.vb`) :

- Toute la logique métier séparée de l'UI
- Méthodes réutilisables : formatage, impression, exports
- Configuration standardisée du RichTextBox

**Intégration contexte UI** :

- Implémente `IContextAwareUserControl`
- Accès au contexte partagé (StatusBar, ToolTips, ErrorProvider)
- Navigation centralisée via `NavigationManager`

**Thématisation Althéa** :

- Toolbar : `ColorSaugeClair` (178, 197, 186)
- Boutons actifs : `ColorSauge` (122, 155, 135)
- Fond éditeur : `ColorBeigeClair` (244, 239, 234)
- Marges internes optimisées : 10px gauche/droite

### Utilisation typique

```vb
' Chargement depuis base de données
ucEditor.RtfContent = patient.NotesRtf

' Sauvegarde en base de données
patient.NotesRtf = ucEditor.RtfContent  ' Formatage préservé
patient.NotesTxt = ucEditor.TextContent ' Texte brut pour recherche

' Export PDF pour archivage
RichTextEditorHelper.ExporterPDFAvecDialogue(ucEditor, "Bilan_Patient_20260517.pdf")

' Export Word pour collaboration
RichTextEditorHelper.ExporterWordAvecDialogue(ucEditor, "Compte_rendu_20260517.docx")

' Mode lecture seule pour historique
ucEditor.ReadOnlyMode = True
```

### Modules métier cibles

- **Anamnèses** : Saisie formatée de l'historique patient
- **Bilans psychologiques/graphothérapeutiques** : Comptes-rendus détaillés avec mise en forme
- **Comptes-rendus de consultations** : Notes de séances structurées
- **Plans d'accompagnement** : Documents évolutifs partagés avec familles/écoles
- **Correspondances** : Courriers aux confrères, médecins traitants

### Documentation complète

- [UC_RichTextEditor_Documentation.md](Docs/UC_RichTextEditor_Documentation.md) (589 lignes) : Guide d'utilisation complet, exemples d'intégration
- [Historique_Implementation_RichTextEditor.md](Docs/Historique_Implementation_RichTextEditor.md) (468 lignes) : Journal technique V1.0 à V1.6
- [PLAN_TESTS_UC_RICHTEXTEDITOR.md](Docs/Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md) (479 lignes) : 120+ cas de test
- [Guide_Licence_Syncfusion.md](Docs/Guide_Licence_Syncfusion.md) : Obtention licence Community gratuite

### Technologies utilisées

- **Syncfusion Community v33.2.10** : DocIO.WinForms, DocToPDFConverter.WinForms, Licensing
- **API Windows** : `EM_FORMATRANGE` (impression RTF), `EM_SETMARGINS` (marges internes)
- **PrintDocument** : `PrintDialog`, `PrintPreviewDialog`, `PageSetupDialog`

---

## UC_RichTextEditorSimple - Éditeur de texte riche allégé

Variante compacte de `UC_RichTextEditor`, destinée aux zones de notes courtes embarquées dans d'autres UserControls ou Forms (référentiels, fiches patient, dossiers, contacts…).

**Toolbar (7 boutons uniquement)** : Gras, Italique, Souligné, Annuler, Rétablir, Effacer format, Date/Heure

**Même règle de sauvegarde double format** que `UC_RichTextEditor` : `RtfContent` (RTF) + `TextContent` (texte brut) — **obligatoire**.

**Différences avec UC_RichTextEditor** :

| Fonctionnalité | UC_RichTextEditor | UC_RichTextEditorSimple |
|---|---|---|
| Toolbar | 30 boutons | 7 boutons |
| Impression, PDF, Word | ✅ | ❌ |
| Contexte UI | ✅ obligatoire | ✅ optionnel |
| Taille | Fixe (grande) | Pilotée par le parent |

```vb
' Intégration
Dim editorNotes As New UC_RichTextEditorSimple()
editorNotes.Dock = DockStyle.Fill
pnlNotes.Controls.Add(editorNotes)

' Sauvegarde (double format obligatoire)
cmd.Parameters.AddWithValue("@notes_rtf", editorNotes.RtfContent)
cmd.Parameters.AddWithValue("@notes_txt", editorNotes.TextContent)

' Lecture seule
editorNotes.ReadOnlyMode = True
editorNotes.ShowToolbar = False
```

---

## Gestion des référentiels

Le Lot 0 a livré un système complet de gestion des tables de référence (`ref_*`). Ces données alimentent les listes déroulantes, filtres et catégorisations de toute l'application.

**Architecture (héritage + noyau commun)** :
- `UC_ReferentielBase` : classe de base héritable qui centralise chargement, modes, CRUD via hooks, validation (unicité code + libellé), droits, journalisation, soft-delete.
- `UC_ReferentielHome` : hub des 9 tuiles de navigation.
- 9 UCs concrets héritent de la base et n'implémentent que leurs spécificités.

**Les 9 référentiels (Lot 0)** :

| UC | Table | Particularité |
|---|---|---|
| `UC_Domaines` | `ref_domaines` | — |
| `UC_LiensPatient` | `ref_liens_patient` | — |
| `UC_RolesIntervenant` | `ref_roles_intervenant` | — |
| `UC_SituationsFamiliales` | `ref_situations_familiales` | — |
| `UC_StatutsDossier` | `ref_statuts_dossier` | — |
| `UC_StatutsSeance` | `ref_statuts_seance` | — |
| `UC_TypesDocuments` | `ref_types_documents` | — |
| `UC_TypesRendezVous` | `ref_types_rendez_vous` | — |
| `UC_TypesSeance` | `ref_types_seance` | ⭐ `tarif_defaut decimal(10,2)` via hooks |

**Pile technique par référentiel** :
```
Core\Database\Queries\Query<X>.vb     → SQL (SELECT, INSERT, UPDATE, soft/hard delete)
Metier\Referentiels\<X>.vb            → Modèle (id, code, libelle, actif, ordre [+ tarif])
Metier\Referentiels\Gestion<X>.vb     → Service CRUD + unicité + EstUtilise()
UI\Controls\Referentiels\UC_<X>.vb    → UserControl concret (hérite UC_ReferentielBase)
```

**Règles clés** :
- Tables `ref_*` → `AUTO_INCREMENT` (pas de séquences)
- Soft-delete si le référentiel est utilisé dans les données métier, hard-delete sinon
- Unicité vérifiée côté service + contrainte UNIQUE en base
- Aucun accès DB dans l'UC (tout passe par `Gestion<X>`)

---

## Utilitaires UI

Les modules utilitaires UI centralisent

### UITheme

Centralise :

- palette graphique
- couleurs applicatives
- constantes visuelles
- chemins d'assets UI
- cohérence graphique globale

#### Utilisation de UITheme

Toutes les couleurs UI pilotées par code doivent être centralisées dans `UITheme`.

*Cela concerne :*

- boutons standards
- boutons Home
- DataGridView
- labels dynamiques
- panneaux générés par code
- contrôles ajoutés dynamiquement
- états disabled / hover / selected
- couleurs d'erreur et d'information

Aucune couleur ne doit être hardcodée directement dans les Forms ou UserControls, sauf exception explicitement documentée.

Pour l'instant la plupart des contrôles sont créés dans le Designer, pour contrôler la présentation. Mais quand on passera à la phase "Branding", toutes les couleurs seront pilotées par code, dans toutes les forms et UC.

*Objectifs :*

- cohérence graphique
- maintenance simplifiée
- évolution visuelle centralisée
- réduction des duplications

### UtilsButtons

Centralise :

- styles des boutons
- comportements hover / selected
- états enabled / disabled
- initialisation standardisée des boutons

### UtilsDataGrid

Centralise :

- style standard des DataGridView
- configuration commune des grilles
- cohérence visuelle des listes

### UtilsValidation

Centralise :

- validation des champs
- validation typée des données
- règles métier réutilisables

### UtilsString

Centralise :

- normalisation technique
- gestion des clés et codes
- helpers de chaînes

### UtilsIcons

Centralise l'accès aux icônes d'état de l'application (`Utils/UtilsIcons.vb`).

**Fonctions disponibles** :

- `IconOK(Optional size As Integer = 32)` : État actif/valide (icône verte)
- `IconOFF(Optional size As Integer = 32)` : État inactif/désactivé (icône rouge/grise)
- `IconLock(Optional size As Integer = 32)` : Compte verrouillé (cadenas)
- `IconNo(Optional size As Integer = 32)` : Refus/interdiction

**Support multi-tailles** : 16x16, 20x20, 26x26, 32x32

**Chargement centralisé** : depuis `My.Resources`, évitant les chargements multiples depuis les fichiers.

**Utilisé par** : `UC_Utilisateurs`, `UC_Parametres` et autres contrôles affichant des états.

**Objectif** :
éviter toute duplication et garantir une cohérence globale UI et métier.

---

## Mode verrouillé

L'application peut démarrer en mode verrouillé tant que :

- la configuration n'est pas valide
- la connexion DB échoue

Dans cet état, aucune navigation n'est possible.

---

## Démarrage & connexion DB

### Principe

Le démarrage est **bloquant volontairement** :

1. Lecture config JSON locale
2. Test connexion via `DatabaseManager`
3. Si OK → déverrouillage
4. Sinon → ouverture configuration

### Sécurité

- mot de passe chiffré via DPAPI
- aucune donnée sensible en clair
- aucune connexion hors DatabaseManager

Voir Processus complet : [Process_Althea.md](Docs/Process_Althea.md) - Processus N°5

---

## Gestion des documents *(prévu - POC réalisé)*

> **Cette fonctionnalité n'est pas encore implémentée dans le code principal.** Un POC a été réalisé (voir [Docs/Poc/](Docs/Poc/)).

Principe retenu (validé par le POC) :

- fichiers stockés hors base
- base = métadonnées uniquement

Stockage envisagé :

- local
- Google Drive

Fonctionnalités cibles :

- DOCX / PDF
- synchronisation cloud
- visualisation

---

## Agenda *(prévu - POC réalisé)*

> **Cette fonctionnalité n'est pas encore implémentée dans le code principal.** Un POC a été réalisé (voir [Docs/Poc/](Docs/Poc/)).

- intégration Google Calendar
- gestion des événements
- séance = événement

---

## Sécurité

- aucune donnée sensible dans les logs
- chiffrement des secrets (DPAPI)
- gestion des erreurs centralisée
- architecture pensée pour confidentialité
- gestion des accès utilisateur via rôles : User, SuperUser et Admin
- authentification robuste avec PBKDF2-SHA256
- verrouillage automatique après échecs de connexion
- élévation temporaire de droits avec ré-authentification

---

## POC (Proof of Concept)

> Ces POC ont été réalisés dans un **projet séparé** (`Althea.POC.SyncfusionDocuments`). Ils ne font **pas partie du code principal actuel**. Ils définissent les choix techniques retenus pour les futures phases de développement.

### Documents

- Syncfusion DocIO / WinForms / DocToPdf
- Google Drive / Docs API

### Agenda

- Syncfusion Scheduler WinForms
- Google Calendar API

Documentation détaillée : [Gestion_documentaire_Althea.md](Docs/Poc/Gestion_documentaire_Althea.md) et [Gestion_Calendrier_Althea.md](Docs/Poc/Gestion_Calendrier_Althea.md)

---

## Structure du projet Althea

```text
Althea/
│
├── Program.vb                  → Point d'entrée (Main) + licence Syncfusion
├── ApplicationEvents.vb        → Événements application WinForms
├── App.config                  → Configuration application .NET
├── Althea.vbproj               → Fichier projet Visual Studio
├── README.md                   → Documentation principale du projet
├── CHANGELOG.md                → Historique des changements
│
├── Core/                       → Couche technique/infrastructure
│   ├── Configuration/
│   │   ├── ConfigManager.vb        → Chargement config locale (JSON)
│   │   └── LocalDbConfig.vb        → Modèle configuration DB
│   │
│   ├── Database/               → Accès base de données
│   │   ├── DatabaseManager.vb      → Point d'accès unique à MariaDB
│   │   └── Queries/                → Requêtes SQL centralisées
│   │       ├── QueryParametres.vb
│   │       ├── QueryUtilisateurs.vb
│   │       ├── QueryDomaines.vb
│   │       ├── QueryLiensPatient.vb
│   │       ├── QueryRolesIntervenant.vb
│   │       ├── QuerySituationsFamiliales.vb
│   │       ├── QueryStatutsDossier.vb
│   │       ├── QueryStatutsSeance.vb
│   │       ├── QueryTypesDocuments.vb
│   │       ├── QueryTypesRendezVous.vb
│   │       └── QueryTypesSeance.vb
│   │
│   ├── Logging/                → Journalisation et gestion des logs
│   │   └── GestionLog.vb           → Journalisation (niveaux : Succinct, Complet)
│   │
│   ├── Security/               → Sécurité et gestion des utilisateurs
│   │   ├── AppRole.vb              → Enum rôles (User=0, SuperUser=1, Admin=2)
│   │   ├── CryptoManagerDPAPI.vb   → Chiffrement/déchiffrement DPAPI
│   │   ├── PasswordSecurityHelper.vb → Helpers sécurité mot de passe
│   │   └── UserSession.vb          → Session utilisateur courante
│   │
│   └── Startup/                → Démarrage de l'application
│       └── AppStartupManager.vb    → Orchestration du démarrage
│
├── Metier/                     → Couche logique métier
│   ├── Parametres/             → Gestion des paramètres applicatifs
│   │   ├── GestionParametres.vb        → Logique métier des paramètres
│   │   └── ParametreApplication.vb     → Modèle de paramètre applicatif
│   │
│   ├── Referentiels/           → Logique métier des référentiels (Lot 0)
│   │   ├── ReferentielLigne.vb         → Modèle de présentation générique
│   │   ├── Domaine.vb + GestionDomaines.vb
│   │   ├── LienPatient.vb + GestionLiensPatient.vb
│   │   ├── RoleIntervenant.vb + GestionRolesIntervenant.vb
│   │   ├── SituationFamiliale.vb + GestionSituationsFamiliales.vb
│   │   ├── StatutDossier.vb + GestionStatutsDossier.vb
│   │   ├── StatutSeance.vb + GestionStatutsSeance.vb
│   │   ├── TypeDocument.vb + GestionTypesDocuments.vb
│   │   ├── TypeRendezVous.vb + GestionTypesRendezVous.vb
│   │   └── TypeSeance.vb + GestionTypesSeance.vb  (+ TarifDefaut)
│   │
│   └── Security/               → Logique métier liée à la sécurité
│       ├── AuthenticationResult.vb     → Résultat de l'authentification
│       ├── GestionAuthentification.vb  → Logique métier de l'authentification
│       ├── GestionUtilisateurs.vb      → Logique métier des utilisateurs
│       └── UtilisateurApplication.vb   → Modèle de l'utilisateur applicatif
│
├── UI/                         → Interface utilisateur
│   ├── Context/                → Contexte partagé pour les UserControls
│   │   └── UserControlContext.vb       → Contexte partagé injecté dans les UC
│   │
│   ├── Controls/               → UserControls métier
│   │   ├── Administration/         → Contrôles d'administration
│   │   │   ├── UC_AdminHome.vb         → Accueil des fonctions d'administration
│   │   │   ├── UC_Parametres.vb        → Gestion des paramètres applicatifs
│   │   │   └── UC_Utilisateurs.vb      → Gestion des utilisateurs
│   │   │
│   │   ├── Communs/                → Composants UI réutilisables
│   │   │   ├── UC_RichTextEditor.vb    → Éditeur de texte riche (30 boutons, exports PDF/Word)
│   │   │   └── UC_RichTextEditorSimple.vb → Éditeur allégé (7 boutons, ré-embeddable)
│   │   │
│   │   ├── Referentiels/           → Gestion des référentiels (Lot 0)
│   │   │   ├── UC_ReferentielBase.vb   → Classe de base héritable (logique commune)
│   │   │   ├── UC_ReferentielHome.vb   → Hub des 9 référentiels
│   │   │   ├── UC_Domaines.vb
│   │   │   ├── UC_LiensPatient.vb
│   │   │   ├── UC_RolesIntervenant.vb
│   │   │   ├── UC_SituationsFamiliales.vb
│   │   │   ├── UC_StatutsDossier.vb
│   │   │   ├── UC_StatutsSeance.vb
│   │   │   ├── UC_TypesDocuments.vb
│   │   │   ├── UC_TypesRendezVous.vb
│   │   │   └── UC_TypesSeance.vb       → + tarif_defaut (hook champ supplémentaire)
│   │   │
│   │   └── UC_Accueil.vb           → Accueil de l'application
│   │
│   ├── Forms/                  → Formulaires de l'application
│   │   ├── Home.vb                 → Formulaire principal (shell)
│   │   │
│   │   ├── Communs/                → Composants UI réutilisables
│   │   │   └── DialogChoix.vb          → Boîte de dialogue personnalisée
│   │   │
│   │   ├── Login/                  → Formulaires liés à l'authentification
│   │   │   ├── Login.vb                → Formulaire de connexion utilisateur
│   │   │   ├── ChangePassword.vb       → Formulaire de changement de mot de passe
│   │   │   └── ElevationAcces.vb       → Formulaire d'élévation de droits
│   │   │
│   │   ├── Utilisateur/            → Formulaires liés aux utilisateurs
│   │   │   └── UtilisateurEdition.vb   → Formulaire d'édition des utilisateurs
│   │   │
│   │   ├── Database/               → Formulaires liés à la configuration DB
│   │   │   └── ConfigurationConnexion.vb → Configuration de la connexion DB
│   │   │
│   │   └── Test/                   → Formulaires de test
│   │       └── TestRichTextEditor.vb   → Test manuel du RichTextEditor
│   │
│   └── Navigation/             → Gestion de la navigation
│       ├── NavigationManager.vb            → Gestion centralisée de la navigation
│       ├── IContextAwareForm.vb            → Interface pour Forms modales
│       └── IContextAwareUserControl.vb     → Interface pour UserControls
│
├── Utils/                      → Utilitaires divers
│   ├── Helpers/                → Modules helper métier
│   │   └── RichTextEditorHelper.vb     → Helper RichTextEditor (impression Win32, exports)
│   │
│   └── UI/                     → Utilitaires UI
│       ├── UITheme.vb              → Thème visuel centralisé
│       ├── UtilsButtons.vb         → Styles et comportements des boutons
│       ├── UtilsControls.vb        → Helpers contrôles WinForms divers
│       ├── UtilsDataGrid.vb        → Style standard des DataGridView
│       ├── UtilsIcons.vb           → Centralisation des icônes d'état
│       ├── UtilsString.vb          → Helpers de normalisation et manipulation
│       └── UtilsValidation.vb      → Helpers de validation
│
├── Assets/                     → Ressources graphiques et visuelles
│   ├── Appli_Ico/              → Icônes de l'application
│   ├── Boutons_Home/           → Icônes des boutons d'accueil
│   ├── Boutons_ico_32/         → Icônes de boutons 32x32
│   ├── Boutons_ico_48/         → Icônes de boutons 48x48
│   ├── Fond/                   → Images de fond
│   ├── Gif_Animated/           → GIF animés (loading, success, error...)
│   ├── Logos/                  → Logos de l'application
│   ├── Splash/                 → Images de démarrage (splash screen)
│   └── Tech_Ico/               → Icônes techniques (lock, ok, no...)
│
├── My Project/                 → Propriétés du projet VB.NET
│   ├── Application.Designer.vb
│   ├── Resources.Designer.vb
│   └── Settings.Designer.vb
│
└── Docs/                       → Documentation du projet
    ├── Database/               → Documentation base de données
    │   ├── Database_technique.md
    │   └── Diagrams/               → Diagrammes de base de données
    │
    ├── Divers/                   → Divers documents
    │   ├── Guide_Licence_Syncfusion.md                             → Guide d'obtention de la licence gratuite Syncfusion Community
    │   ├── Historique_Implementation_RichTextEditor.md             → Historique des implémentations du RichTextEditor
    │   └── RAPPORT_AUDIT_2026-05-17.md                             → Rapport d'audit du 17 mai 2026
    │
    ├── Help/                   → Aide utilisateur
    │   └── UC_RichTextEditor_Documentation    
    │
    ├── Illustrations/          → Illustrations de la documentation
    │
    ├── Poc/                    → Proof of Concept
    │   ├── Gestion_documentaire_Althea.md                             → Doc POC gestion documentaire
    │   └── Gestion_Calendrier_Althea.md                               → Doc POC gestion calendrier
    │
    ├── Rules/                  → Règles et standards du projet
    │   ├── ARCHITECTURE_DECISIONS.md           → Décisions d'architecture majeures    
    │   ├── Reference_UI_Guide_Utilisation.md   → Guide de référence pour l'utilisation des composants UI (contexte, status, tooltips)
    │   ├── Reference_UI_Proprietes.md          → Guide de référence pour les propriétés des composants UI
    │   ├── Rules.md                            → Règles générales du projet
    │   └── Standards-Commentaires.md           → Standards de commentaires dans le code
    │
    ├── Tests/                  → Plans de tests
    │   ├── STRATEGIE_TESTS_ALTHEA.md              → Stratégie globale de tests pour Althéa
    │   ├── PLAN_TESTS_LOGIN_AUTHENTIFICATION.md   → Tests liés à l'authentification et à la gestion des utilisateurs
    │   ├── PLAN_TESTS_UC_PARAMETRES.md            → Tests liés au module UC_Parametres
    │   ├── PLAN_TESTS_UC_UTILISATEURS.md          → Tests liés au module UC_Utilisateurs
    │   ├── PLAN_TESTS_UC_RICHTEXTEDITOR.md        → Tests liés au module UC_RichTextEditor
    │   ├── PLAN_TESTS_DIALOGCHOIX.md              → Tests liés au module DialogChoix
    │   └── PLAN_TESTS_HOME_NAVIGATION.md          → Tests liés à la navigation et au module Home
    │
	├── UI/                  → Documentation UI
    │   ├── Charte_Graphique.md                             → Charte graphique de l'application
    │   ├── Documentation_technique_UI_Althea.md            → Documentation technique des composants UI
    │   └── Process_Althea.md                               → Processus et workflows UI
    │
    ├── Todo/                   → Planification et suivi
    │   ├── cahier_des_charges_patients.pdf                             → Cahier des charges pour le module Patients
    │   ├── Checklist_projet_V1.md                                      → Checklist du projet V1
    │   ├── ETAT_DU_PROJET.md                                           → État actuel du projet
    │   ├── Planning_actions_Althéa.md                                  → Planning des actions pour Althéa
    │   └── ToDo.md                                                     → Liste des tâches à réaliser
    │
    └── 🌿 Qu'est-ce qu'Althéa_160526.md
```

### Forms

| Noms                   | Usage                                                        | Emplacement |
| ---------------------- | ------------------------------------------------------------ | ----------- |
| Home                   | Formulaire de départ, accès rapide aux fonctionnalités principales | /UI/Forms   |
| ConfigurationConnexion | Formulaire de configuration de la connexion                  | /UI/Forms/Database   |
| DialogChoix            | Boîte de dialogue personnalisée                              | /UI/Forms/Communs   |
| ElevationAcces         | Formulaire de gestion de l'élévation des droits d'accès      | /UI/Forms/Login   |
| Login                  | Formulaire de connexion utilisateur                          | /UI/Forms/Login   |
| ChangePassword         | Formulaire de changement de mot de passe utilisateur         | /UI/Forms/Login   |
| UtilisateurEdition     | Formulaire d'édition des utilisateurs (création/modification/consultation) | /UI/Forms/Utilisateur |

### Modules

| Noms                    | Usage                                                        | Emplacement            |
| ----------------------- | ------------------------------------------------------------ | ---------------------- |
| ConfigManager           | Gestion de la configuration de l'application                 | /Core/Configuration    |
| DatabaseManager         | Gestion des connexions et des opérations sur la base de données | /Core/Database         |
| QueryParametres         | Centralisation des requêtes SQL sur la table tec_parametres  | /Core/Database/Queries |
| QueryUtilisateurs       | Centralisation des requêtes SQL sur la table sec_utilisateurs | /Core/Database/Queries |
| Query\<X\> ×9          | Requêtes SQL référentiels (Domaines, LiensPatient, RolesIntervenant, SituationsFamiliales, StatutsDossier, StatutsSeance, TypesDocuments, TypesRendezVous, TypesSeance) | /Core/Database/Queries |
| GestionLog              | Gestion des journaux et des événements                       | /Core/Logging          |
| CryptoManagerDPAPI      | Gestion de la cryptographie et de la sécurité                | /Core/Security         |
| PasswordSecurityHelper  | Aide à la gestion sécurité des mots de passe (PBKDF2)        | /Core/Security         |
| GestionParametres       | Gestion des paramètres et configurations métier              | /Metier/Parametres     |
| Gestion\<X\> ×9        | Services CRUD + unicité + EstUtilise() pour les 9 référentiels | /Metier/Referentiels |
| GestionAuthentification | Gestion de l'authentification utilisateur                    | /Metier/Security       |
| GestionUtilisateurs     | Logique métier des utilisateurs applicatifs                  | /Metier/Security       |
| UITheme                 | Thème visuel centralisé (couleurs, constantes, assets)       | /Utils                 |
| UtilsButtons            | Styles et comportements des boutons (hover, selected, disabled) | /Utils                 |
| UtilsDataGrid           | Style standard des DataGridView                              | /Utils                 |
| UtilsIcons              | Centralisation des icônes d'état (OK, OFF, Lock, No)         | /Utils                 |
| UtilsString             | Helpers de normalisation et manipulation de chaînes          | /Utils                 |
| UtilsValidation         | Helpers de validation des champs et données                  | /Utils                 |
| UtilsControls           | Helpers contrôles WinForms divers                            | /Utils                 |
| RichTextEditorHelper    | Helper centralisé pour édition texte riche (formatage, impression, exports) | /Utils/Helpers         |
| Program                 | Point d'entrée de l'application                              | root                   |

### Classes

| Noms                   | Usage                                                        | Emplacement         |
| ---------------------- | ------------------------------------------------------------ | ------------------- |
| LocalDbConfig          | Gestion de la configuration de la base de données locale     | /Core/Configuration |
| UserSession            | Gestion de la session utilisateur                            | /Core/Security      |
| AppStartupManager      | Gestion du démarrage de l'application                        | /Core/Startup       |
| ParametreApplication   | Paramètres applicatifs : lecture depuis la base, transformation en objets métier | /Metier/Parametres  |
| ReferentielLigne       | Modèle de présentation générique pour tous les référentiels  | /Metier/Referentiels |
| \<X\> ×9              | Modèles métier référentiels (Domaine, LienPatient, RoleIntervenant, SituationFamiliale, StatutDossier, StatutSeance, TypeDocument, TypeRendezVous, TypeSeance) | /Metier/Referentiels |
| UtilisateurApplication | Modèle de l'utilisateur applicatif (login, rôle, MustChangePassword, etc.) | /Metier/Security    |
| AuthenticationResult   | Résultat de l'authentification utilisateur                   | /Metier/Security    |
| UserControlContext     | Gestion du contexte utilisateur, rôles et accès              | /UI/Context         |
| NavigationManager      | Gestion de la navigation entre les écrans principaux         | /UI/Navigation      |

### Enums

| Noms              | Usage                                                    | Emplacement         |
| ----------------- | -------------------------------------------------------- | ------------------- |
| AppRole           | Définition des rôles globaux de l'application (User, SuperUser, Admin) | /Core/Security      |
| TypeDialogue      | Types de dialogue pour DialogChoix (Information, Warning, Error, Question, Success, Loading, Processing) | /UI/Forms/Communs   |
| ModeUtilisateurEdition | Modes d'édition utilisateur (Creation, Modification, Consultation) | /UI/Forms/Utilisateur |
| ModeChangePassword | Modes de changement de mot de passe (UserChange, AdminReset) | /UI/Forms/Login     |

### UserControls

| Noms | Usage | Emplacement |
| --- | --- | --- |
| UC_Accueil | Accueil de l'application, point d'entrée principal | /UI/Controls/Administration |
| UC_AdminHome | Accueil des fonctions d'administration | /UI/Controls/Administration |
| UC_Parametres | Gestion des paramètres applicatifs | /UI/Controls/Administration |
| UC_Utilisateurs | Gestion des utilisateurs (liste, recherche, actions) | /UI/Controls/Administration |
| UC_RichTextEditor | **Éditeur de texte riche réutilisable** (notes, anamnèses, bilans, comptes-rendus) | /UI/Controls/Communs |
| UC_RichTextEditorSimple | **Éditeur allégé ré-embeddable** (notes courtes, commentaires) | /UI/Controls/Communs |
| UC_ReferentielBase | **Classe de base héritable** pour tous les référentiels | /UI/Controls/Referentiels |
| UC_ReferentielHome | **Hub des 9 référentiels** (navigation, droits) | /UI/Controls/Referentiels |
| UC_Domaines | Référentiel Domaines | /UI/Controls/Referentiels |
| UC_LiensPatient | Référentiel Liens patient | /UI/Controls/Referentiels |
| UC_RolesIntervenant | Référentiel Rôles intervenant | /UI/Controls/Referentiels |
| UC_SituationsFamiliales | Référentiel Situations familiales | /UI/Controls/Referentiels |
| UC_StatutsDossier | Référentiel Statuts dossier | /UI/Controls/Referentiels |
| UC_StatutsSeance | Référentiel Statuts séance | /UI/Controls/Referentiels |
| UC_TypesDocuments | Référentiel Types documents | /UI/Controls/Referentiels |
| UC_TypesRendezVous | Référentiel Types rendez-vous | /UI/Controls/Referentiels |
| UC_TypesSeance | Référentiel Types séance **(+ tarif_defaut)** | /UI/Controls/Referentiels |

### Interfaces 

| Noms                     | Usage                                                    | Emplacement    |
| ------------------------ | -------------------------------------------------------- | -------------- |
| IContextAwareUserControl | Interface pour les UserControls sensibles au contexte utilisateur | /UI/Navigation |
| IContextAwareForm        | Interface pour les Forms modales sensibles au contexte utilisateur | /UI/Navigation |

---

## État du projet

### Phase actuelle

**Implémenté et fonctionnel** :

- Infrastructure : Config locale JSON, chiffrement DPAPI, démarrage bloquant, logs
- Base de données : `DatabaseManager`, requêtes centralisées (`QueryParametres`, `QueryUtilisateurs`, Query* référentiels ×9)
- Sécurité : Authentification (login / PBKDF2 / verrouillage), changement de mot de passe obligatoire, élévation temporaire de droits
- Session utilisateur : `UserSession`, `AppRole` (User / SuperUser / Admin)
- Navigation : `NavigationManager`, `UserControlContext`, `IContextAwareUserControl`, `IContextAwareForm`
- UserControls : `UC_Accueil`, `UC_AdminHome`, `UC_Parametres`, `UC_Utilisateurs`
- Gestion des paramètres applicatifs : CRUD complet avec contrôle d'accès par rôle
- **Gestion des utilisateurs : CRUD complet avec contrôle d'accès par rôle**
  - Création, modification, consultation
  - Recherche et filtrage multi-critères
  - Actions de maintenance (reset password, déverrouillage, activation/désactivation)
  - Modes d'édition différenciés (Admin vs SuperUser)
- **Interface utilisateur personnalisée**
  - DialogChoix : remplacement complet des MessageBox (34 occurrences)
  - UtilsIcons : centralisation des icônes d'état
- **Éditeurs de texte riche** :
  - `UC_RichTextEditor` : 30 boutons, impression Win32, exports PDF/Word Syncfusion
  - `UC_RichTextEditorSimple` : 7 boutons, allégé, ré-embeddable, même double format RTF+TXT
- **Gestion des référentiels (Lot 0 complet)** :
  - `UC_ReferentielBase` : classe de base héritable (CRUD, validation, droits, journalisation, hooks)
  - `UC_ReferentielHome` : hub de navigation (9 tuiles, droits)
  - 9 référentiels complets : Domaines, Liens patient, Rôles intervenant, Situations familiales, Statuts dossier, Statuts séance, Types documents, Types rendez-vous, Types séance
  - `UC_TypesSeance` : champ additionnel `tarif_defaut` via hooks dédiés

**Prévus (non démarrés dans le code principal)** :

- Patients / Dossiers / Séances / Paiements
- Gestion documentaire *(POC réalisé - voir [Docs/Poc/](Docs/Poc/))*
- Agenda *(POC réalisé - voir [Docs/Poc/](Docs/Poc/))*

> Voir le détail par module dans la section [Statut des modules](#statut-des-modules) ci-dessous.

---

## Statut des modules

> Légende : ✅ Implémenté | 🟡 Partiel / En cours | 🔜 Prévu | 🧪 POC uniquement

### Infrastructure & Core

| Module | Statut | Notes |
| ------ | ------ | ----- |
| `AppStartupManager` | ✅ | Démarrage bloquant, config → DB → Home |
| `ConfigManager` | ✅ | Lecture/écriture config JSON locale (AppData) |
| `DatabaseManager` | ✅ | Point d'accès unique à MariaDB |
| `QueryParametres` | ✅ | Requêtes CRUD sur `tec_parametres` |
| `QueryUtilisateurs` | ✅ | Requêtes CRUD sur `sec_utilisateurs` |
| `Query<X>` ×9 | ✅ | Requêtes SQL des 9 référentiels (SELECT actifs/tous, INSERT, UPDATE, soft/hard delete, unicité, usage) |
| `GestionLog` | ✅ | Journalisation avec niveaux (Succinct, Rapide, Complet) |
| `CryptoManagerDPAPI` | ✅ | Chiffrement/déchiffrement DPAPI |
| `PasswordSecurityHelper` | ✅ | Hachage et vérification PBKDF2-SHA256 |
| `UserSession` | ✅ | Session en mémoire (rôle courant, élévation) |
| `AppRole` | ✅ | Enum rôles : User=0, SuperUser=1, Admin=2 |

### Métier

| Module | Statut | Notes |
| ------ | ------ | ----- |
| `GestionAuthentification` | ✅ | Auth, changement MDP, élévation, verrouillage |
| `GestionUtilisateurs` | ✅ | **CRUD complet** : création, modification, reset password, déverrouillage, activation/désactivation |
| `GestionParametres` | ✅ | CRUD paramètres applicatifs avec contrôle rôle |
| `Gestion<X>` référentiels (×9) | ✅ | **Lot 0 complet** : Domaines, LiensPatient, RolesIntervenant, SituationsFamiliales, StatutsDossier, StatutsSeance, TypesDocuments, TypesRendezVous, TypesSeance |
| Gestion patients | 🔜 | Non démarré |
| Gestion dossiers | 🔜 | Non démarré |
| Gestion séances | 🔜 | Non démarré |
| Gestion paiements | 🔜 | Non démarré |
| Gestion documents | 🧪 | POC réalisé, intégration à planifier |
| Gestion agenda | 🧪 | POC réalisé, intégration à planifier |

### Interface utilisateur

| Composant | Statut | Notes |
| --------- | ------ | ----- |
| `Home` (shell) | ✅ | Form principale, menu, navigation, header/footer |
| `Login` | ✅ | Authentification, validation, ErrorProvider |
| `ChangePassword` | ✅ | Changement de mot de passe (UserChange / AdminReset) |
| `ElevationAcces` | ✅ | Élévation temporaire de droits |
| `ConfigurationConnexion` | ✅ | Configuration connexion DB (UI + test + sauvegarde) |
| `DialogChoix` | ✅ | **Boîte de dialogue personnalisée** (remplace tous les MessageBox) |
| `UC_Accueil` | ✅ | Écran d'accueil |
| `UC_AdminHome` | ✅ | Tableau de bord administration (boutons, élévation) |
| `UC_Parametres` | ✅ | Gestion des paramètres (liste + détail + CRUD) |
| `UC_Utilisateurs` | ✅ | **Gestion des utilisateurs complète** : liste, recherche/filtrage, création/modification/consultation, actions |
| `UtilisateurEdition` | ✅ | **Form modale d'édition utilisateur** : Création, Modification, Consultation |
| `UC_RichTextEditor` | ✅ | **Éditeur de texte riche complet** : 30 boutons, impression Win32, exports PDF/Word |
| `UC_RichTextEditorSimple` | ✅ | **Éditeur allégé ré-embeddable** : 7 boutons, double format RTF+TXT, optionnellement contextuel |
| `UC_ReferentielHome` | ✅ | **Hub des 9 référentiels** (navigation, droits) |
| `UC_ReferentielBase` | ✅ | **Classe de base héritable** : CRUD, validation, droits, journalisation, hooks |
| `UC_Domaines` | ✅ | Référentiel Domaines |
| `UC_LiensPatient` | ✅ | Référentiel Liens patient |
| `UC_RolesIntervenant` | ✅ | Référentiel Rôles intervenant |
| `UC_SituationsFamiliales` | ✅ | Référentiel Situations familiales |
| `UC_StatutsDossier` | ✅ | Référentiel Statuts dossier |
| `UC_StatutsSeance` | ✅ | Référentiel Statuts séance |
| `UC_TypesDocuments` | ✅ | Référentiel Types documents |
| `UC_TypesRendezVous` | ✅ | Référentiel Types rendez-vous |
| `UC_TypesSeance` | ✅ | Référentiel Types séance **(+ tarif_defaut)** |
| UC patients / dossiers / séances | 🔜 | Non démarrés |

### Utilitaires

| Module | Statut | Notes |
| ------ | ------ | ----- |
| `UITheme` | ✅ | Centralisation des couleurs et constantes visuelles |
| `UtilsButtons` | ✅ | Styles et comportements des boutons |
| `UtilsDataGrid` | ✅ | Style standard des DataGridView |
| `UtilsValidation` | ✅ | Helpers de validation |
| `UtilsString` | ✅ | Helpers de normalisation et manipulation |
| `UtilsControls` | ✅ | Helpers contrôles WinForms divers |
| `UtilsIcons` | ✅ | **Centralisation des icônes d'état** (OK, OFF, Lock, No) avec support multi-tailles |
| `RichTextEditorHelper` | ✅ | **Helper centralisé pour édition texte riche** (formatage, impression Win32, exports PDF/Word) |

---

## Documentation

### Documentation principale

- [C_est_quoi_Althéa_160526.md](Docs/C_est_quoi_Althéa_160526.md) : Présentation générale du projet Althéa, ses objectifs et sa vision.
- [ETAT_DU_PROJET.md](Docs/ToDo/ETAT_DU_PROJET.md) : État actuel du projet, avancement et prochaines étapes.
- [RAPPORT_AUDIT_2026-05-17.md](Docs/Divers/RAPPORT_AUDIT_2026-05-17.md) : Rapport d'audit complet du projet effectué le 17/05/2026.
- [CHANGELOG.md](CHANGELOG.md) : Journal des modifications et des versions de l'application.

### Documentation technique

- [Documentation_technique_UI_Althea.md](Docs/UI/Documentation_technique_UI_Althea.md) : Documentation technique des Forms, UserControls Althéa, avec les rôles, responsabilités et interactions des différents éléments.
- [UC_RichTextEditor_Documentation.md](Docs/Help/UC_RichTextEditor_Documentation.md) : Documentation complète de l'éditeur de texte riche réutilisable (formatage, impression, exports PDF/Word, intégration POC documentaire).
- [Historique_Implementation_RichTextEditor.md](Docs/Divers/Historique_Implementation_RichTextEditor.md) : Journal technique détaillé de l'implémentation V1.0 à V1.6 (résolution problèmes, décisions techniques).
- [Process_Althea.md](Docs/UI/Process_Althea.md) : Documentation détaillée des différents processus avec les étapes, les règles et les flux associés.

### Standards et règles

- [Standards-Commentaires.md](Docs/Rules/Standards-Commentaires.md) : Documentation des standards de commentaires pour le code Althéa, avec les bonnes pratiques à suivre.
- [Rules.md](Docs/Rules/Rules.md) : Règles de développement et de gestion du projet, avec les bonnes pratiques à suivre.
- [Reference_UI_Guide_Utilisation.md](Docs/Rules/Reference_UI_Guide_Utilisation.md) : Guide d'utilisation des contrôles UI personnalisés.
- [Reference_UI_Proprietes.md](Docs/Rules/Reference_UI_Proprietes.md) : Référence des propriétés personnalisées des contrôles UI de l'application.
- [Charte_Graphique.md](Docs/UI/Charte_Graphique.md) : Charte graphique de l'application, avec les couleurs, les icônes et les éléments visuels.

### Base de données

- [Database_technique.md](Docs/Database/Database_technique.md) : Documentation technique de la base de données, avec les schémas et les relations.
- [althea_All.png](Docs/Database/Diagrams/althea_All.png) : Diagramme complet de la base de données Althéa avec toutes les tables et relations.
- [althea_Key.png](Docs/Database/Diagrams/althea_Key.png) : Diagramme de la base de données Althéa avec les clés primaires et étrangères.

### POC (Proof of Concept)

- [Gestion_documentaire_Althea.md](Docs/Poc/Gestion_documentaire_Althea.md) : Documentation des tests effectués dans le POC `Althea.POC.SyncfusionDocuments` sur la gestion documentaire dans Althéa avec les API Google Drive et Docs, les composants Syncfusion DocIO.WinForms DocToPdf et Office Word 365.
- [Gestion_Calendrier_Althea.md](Docs/Poc/Gestion_Calendrier_Althea.md) : Documentation des tests effectués dans le POC `Althea.POC.SyncfusionDocuments` sur la gestion de l'agenda dans Althéa avec les API Google Calendar et les composants Syncfusion Scheduler.WinForms.

### Technologies spécifiques

- [Guide_Licence_Syncfusion.md](Docs/Divers/Guide_Licence_Syncfusion.md) : Guide d'obtention de la licence Syncfusion Community gratuite pour exports PDF/Word.

### Planification et Todo

- [cahier_des_charges_patients.pdf](Docs/Todo/cahier_des_charges_patients.pdf) : Cahier des charges fixées par le demandeur, avec les exigences fonctionnelles et techniques.
- [Checklist_projet_V1.md](Docs/Todo/Checklist_projet_V1.md) : 1ère version de la checklist de projet, avec les étapes clés et les tâches à réaliser.
- [ToDo.md](Docs/Todo/ToDo.md) : Liste des tâches à réaliser, avec les priorités et les statuts.
- [Planning_actions_Althéa.md](Docs/Todo/Planning_actions_Althéa.md) : Planning d'actions pour la version 1, avec les phases de développement et les objectifs associés.

---
## Plan de tests

| Module | Plan de tests |
|--------|---------------|
|Strategie Tests|[STRATEGIE_TESTS_ALTHEA.md](Docs/Tests/STRATEGIE_TESTS_ALTHEA.md)|
| Login & Authentification |  [PLAN_TESTS_LOGIN_AUTHENTIFICATION.md](Docs/Tests/PLAN_TESTS_LOGIN_AUTHENTIFICATION.md) |
| UC_Parametres | [PLAN_TESTS_UC_PARAMETRES.md](Docs/Tests/PLAN_TESTS_UC_PARAMETRES.md) |
| UC_Utilisateurs | [PLAN_TESTS_UC_UTILISATEURS.md](Docs/Tests/PLAN_TESTS_UC_UTILISATEURS.md) |
| UC_RichTextEditor | [PLAN_TESTS_UC_RICHTEXTEDITOR.md](Docs/Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md) |
| DialogChoix | [PLAN_TESTS_DIALOGCHOIX.md](Docs/Tests/PLAN_TESTS_DIALOGCHOIX.md) |
| Home & Navigation | [PLAN_TESTS_HOME_NAVIGATION.md](Docs/Tests/PLAN_TESTS_HOME_NAVIGATION.md) |


## Objectif du projet

Althéa est conçue comme un outil :

- professionnel
- durable
- évolutif
- centré sur l'usage réel

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
