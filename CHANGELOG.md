# 📌 **Althéa - CHANGELOG**

>  *Dernière mise à jour : 06/06/2026*

---

## 📅 06/06/2026

### 🎨 Module complet d'édition de texte riche (RichTextEditor)

#### Nouveau composant UI réutilisable
- **Création de `UI/Controls/UC_RichTextEditor.vb`** : UserControl d'édition de texte riche complet pour notes patients, anamnèses, bilans, comptes-rendus de consultations
  - **30 boutons de toolbar** : formatage caractères, paragraphes, presse-papiers, actions, insertion, outils
  - **Formatage de caractères** : Gras (Ctrl+B), Italique (Ctrl+I), Souligné (Ctrl+U), Barré, Couleur texte, Surbrillance
  - **Formatage de paragraphes** : Alignement (gauche/centré/droite), Listes à puces, Retraits (augmentation/diminution)
  - **Polices et tailles** : 9 polices courantes, 14 tailles (8 à 72 points)
  - **Actions** : Couper, Copier, Coller, Annuler, Rétablir, Effacer formatage
  - **Insertion** : Date/heure courante (format paramétrable)
  - **Outils** : Mise en page (📄), Impression (🖨), Export PDF (📑), Export Word (📝)
  - **Modes** : ReadOnly (masque toolbar), ShowToolbar (afficher/masquer toolbar)
  - **Sauvegarde double** : RTF (formatage préservé pour affichage) + TXT (texte brut pour recherche full-text SQL)
  - **Marges internes optimisées** : 10px gauche/droite pour meilleure lisibilité
  - **Implémentation `IContextAwareUserControl`** : intégration complète avec le contexte UI partagé

#### Helper centralisé
- **Création de `Utils/Helpers/RichTextEditorHelper.vb`** : Module utilitaire centralisant toute la logique métier
  - **Configuration** : `ConfigurerRichTextBox()` avec thème Althéa
  - **Formatage caractères** : BasculerGras, BasculerItalique, BasculerSouligne, BasculerBarre, ChangerCouleurTexte, ChangerCouleurFond
  - **Formatage paragraphes** : ChangerAlignement, BasculerPuces, AugmenterRetrait, DiminuerRetrait
  - **Police** : ChangerPolice, ChangerTaille
  - **Presse-papiers** : Couper, Copier, Coller
  - **Contenu** : ChargerContenu, ExtraireRtf, ExtraireTxt, EffacerTout
  - **Insertion** : InsererDateHeure avec format personnalisable
  - **Impression Win32** : Print avec API EM_FORMATRANGE (formatage RTF préservé), Imprimer avec PrintDialog, AfficherApercu, ConfigurerMiseEnPage
  - **Export PDF** : ExporterPDF, ExporterPDFAvecDialogue (Syncfusion DocIO + DocToPDFConverter)
  - **Export Word** : ExporterWord, ExporterWordAvecDialogue (Syncfusion DocIO, format .docx éditable)

#### Système d'impression avancé
- **V1.0-V1.2** : Implémentation initiale avec API Windows `EM_FORMATRANGE` pour préserver le formatage RTF natif
- **V1.3** : Résolution du problème de pages blanches via `checkPrint = 0` après chaque impression
- **V1.4** : Suppression de l'aperçu automatique non souhaité → impression directe après `PrintDialog`
  - Nouvelle méthode `AfficherApercu()` optionnelle pour visualisation avant impression
  - Bouton **📄 Mise en page** pour configuration format papier (A4/A3/Letter), marges, orientation (portrait/paysage)
  - Dialogue système Windows natif pour sélection imprimante et paramètres
  - Pagination automatique avec marges configurables

#### Export PDF avec Syncfusion
- **V1.5** : Intégration Syncfusion pour export PDF natif RTF → PDF
  - **Packages NuGet installés** : `Syncfusion.DocIO.WinForms`, `Syncfusion.DocToPDFConverter.WinForms`, `Syncfusion.Licensing` (v33.2.10)
  - **Configuration licence** : Ajout `SyncfusionLicenseProvider.RegisterLicense()` dans `Program.vb`
  - **Bouton toolbar** : 📑 Export PDF avec SaveFileDialog
  - **Formatage préservé** : Gras, italique, couleurs, alignement, puces, polices, retraits, paragraphes
  - **Conversion native** : RTF → WordDocument (DocIO) → PDF (DocToPDFConverter)
  - **Licence Community gratuite** : Pour usage non-commercial ou revenus < 1M USD

#### Export Word avec intégration documentaire
- **V1.6** : Export .docx éditable avec intégration POC système documentaire
  - **Nouveau bouton toolbar** : 📝 Export Word (après Export PDF)
  - **Méthodes** : `ExporterWord()`, `ExporterWordAvecDialogue()`
  - **Conversion** : RTF → WordDocument → Save DOCX (Syncfusion.DocIO)
  - **Intégration POC documentaire** :
    - Architecture locale : `Patients/{id_patient}/{id_dossier}/Documents/`
    - Synchronisation Google Drive automatique
    - Édition possible avec Word (local) ou Google Docs (cloud)
    - Conversion PDF automatique pour archivage (le .docx est la source, le PDF est le dérivé)
  - **Flows compatibles** :
    - Flow 1 (Word local) : Export → Ouvrir Word → Éditer → Upload Drive
    - Flow 2 (Google Docs) : Export → Upload Drive → Ouvrir Google Docs
    - Flow 3 (Admission externe) : Export vers dossier patient existant
  - **Avantages vs PDF** : Éditable, collaboration multi-utilisateurs, standard attendu pour dossiers patients/bilans/comptes-rendus

#### Nettoyage et optimisations
- **Suppression bouton Rechercher** : Recherche se fera au niveau Dossier/Patient via SQL full-text sur colonnes `notes_txt`, pas de besoin de recherche dans une note isolée
- **Optimisation marges** : Augmentation de 8px à 10px (gauche/droite) pour meilleure lisibilité
- **Raccourcis clavier standards** : Ctrl+B (Gras), Ctrl+I (Italique), Ctrl+U (Souligné), Ctrl+Z (Annuler), Ctrl+Y (Rétablir), Ctrl+P (Imprimer)

#### Form de test
- **Création de `UI/Forms/Test/TestRichTextEditor.vb`** : Form de validation manuelle complète
  - Chargement de contenu de test
  - Simulation sauvegarde/chargement depuis base de données
  - Test modes ReadOnly et ShowToolbar
  - Validation de tous les boutons de formatage
  - Tests d'impression et exports (PDF + Word)
  - Bouton d'accès provisoire dans `Home.vb` pour tests

#### Documentation complète
- **`Docs/UC_RichTextEditor_Documentation.md`** (589 lignes) :
  - Vue d'ensemble et architecture
  - Guide d'utilisation complet (chargement, sauvegarde, intégration)
  - Référence exhaustive des fonctionnalités
  - Section impression et mise en page
  - Section export PDF avec Syncfusion
  - Section export Word avec intégration POC documentaire
  - Tableau comparatif PDF vs Word
  - Exemples d'intégration métier (anamnèses, bilans, consultations)
  - Guide maintenance et extension
- **`Docs/Historique_Implementation_RichTextEditor.md`** (468 lignes) :
  - Journal détaillé V1.0 à V1.6 avec toutes les itérations
  - Résolution des 10 problèmes rencontrés (pages blanches, aperçu forcé, etc.)
  - Décisions techniques argumentées
- **`Docs/Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md`** (479 lignes) :
  - Plan de tests exhaustif avec 120+ cas de test
  - 8 sections : Chargement, Formatage caractères, Presse-papiers, Annuler/Rétablir, Formatage paragraphes, Insertion, Modes, Impression/Export
- **`Docs/Guide_Licence_Syncfusion.md`** : Guide complet pour obtention licence Community gratuite

#### Intégration thématique
- **Respect charte graphique Althéa** :
  - Toolbar : `ColorSaugeClair` (178, 197, 186)
  - Boutons actifs : `ColorSauge` (122, 155, 135)
  - Fond éditeur : `ColorBeigeClair` (244, 239, 234)
  - Texte : `ColorTexte` (74, 74, 74)
- **Glyphes Unicode** pour icônes toolbar (léger, pas de dépendances images)

#### Prêt pour l'intégration
- **Modules métier cibles** :
  - `UC_PatientDetails` : Notes générales patient
  - `UC_DossierMedical` : Anamnèses, bilans psychologiques/graphothérapie
  - `UC_Consultation` : Comptes-rendus de consultations/séances
  - `UC_Ordonnance` : Notes prescriptions/recommandations
  - `UC_DocumentsPatient` : Notes associées aux documents
- **Base de données** : Colonnes `notes_rtf` (formatage) + `notes_txt` (recherche full-text) à ajouter aux tables concernées

#### Technologies utilisées
- **WinForms .NET 8.0** avec VB.NET (Option Strict On)
- **Syncfusion Community Edition v33.2.10** (DocIO, DocToPDFConverter, Pdf, Licensing)
- **API Windows** : `EM_FORMATRANGE` pour impression RTF native, `EM_SETMARGINS` pour marges internes
- **PrintDocument** : Gestion impression avec `PrintDialog`, `PrintPreviewDialog`, `PageSetupDialog`

#### Avantages pour les utilisateurs (psychologues, graphothérapeutes)
- ✅ **Édition riche** : Formatage professionnel des bilans et comptes-rendus
- ✅ **Export éditable** : Documents Word pour collaboration (écoles, médecins traitants, parents)
- ✅ **Archivage PDF** : Documents finalisés non modifiables
- ✅ **Templates réutilisables** : Modèles d'anamnèses et bilans pré-formatés
- ✅ **Recherche efficace** : Full-text SQL sur `notes_txt` au niveau Dossier/Patient
- ✅ **Intégration cloud** : Synchronisation automatique Google Drive (POC)
- ✅ **Conformité légale** : Documents patients éditables + archivage PDF sécurisé

---

## 📅 05/06/2026 - 06/06/2026

### Remplacement complet MessageBox → DialogChoix

#### UI / UX
- **Remplacement systématique** de tous les `MessageBox.Show` par le nouveau composant `DialogChoix` dans les modules UI suivants :
  - `UC_AdminHome.vb` : 3 remplacements (erreurs système)
  - `UC_Parametres.vb` : 3 remplacements (erreur, confirmation, validation)
  - `Home.vb` : 4 remplacements (confirmation élévation, erreurs, information debug)
  - `ElevationAcces.vb` : 1 remplacement (erreur authentification)
  - `UC_Utilisateurs.vb` : 4 remplacements (confirmation activation/désactivation, succès, erreurs, avertissements)
  - `UtilisateurEdition.vb` : **19 remplacements** (création/modification utilisateur, validation formulaire, reset mot de passe, déverrouillage compte, gestion erreurs et exceptions)

#### Résultat
- **34 MessageBox remplacés au total** dans tous les modules UI
- Interface utilisateur homogène avec icônes GIF animées personnalisées
- Cohérence visuelle respectant la charte graphique Althéa

---

## 📅 03/06/2026

### Création composant DialogChoix personnalisé

#### Nouveau composant UI
- **Création de `UI/Forms/Communs/DialogChoix.vb`** : Dialog personnalisé remplaçant MessageBox
  - Enum `TypeDialogue` : Information, Warning, Error, Question, Success, Loading, Processing
  - Configuration flexible de 1 à 3 boutons via méthodes `SetBoutons()`
  - Mapping DialogResult : Bouton1=Yes, Bouton2=No, Bouton3=Cancel
  - Méthodes statiques simplifiées : `Information()`, `Erreur()`, `Avertissement()`, `Succes()`, `Confirmer()`, `Question()`
  - Support des icônes GIF animées via `My.Resources`
  - Thématisation complète via `UITheme`
  - Taille adaptative selon le contenu du message

#### Utilitaires
- **Création de `Utils/UtilsIcons.vb`** : Centralisation de la gestion des icônes d'état
  - `IconOK()` : État actif/valide (icône verte)
  - `IconOFF()` : État inactif/désactivé (icône rouge/grise)
  - `IconLock()` : Compte verrouillé (cadenas)
  - `IconNo()` : Refus/interdiction
  - Support multi-tailles : 16x16, 20x20, 26x26, 32x32
  - Chargement centralisé depuis `My.Resources`

#### Intégration
- Refactorisation `UC_Utilisateurs.vb` et `UC_Parametres.vb` pour utiliser `UtilsIcons`
- Gestion prioritaire des icônes d'état : CompteVerrouillé → IconLock, Actif → IconOK, Inactif → IconOFF

---

## 📅 01/06/2026 - 02/06/2026

### Amélioration système de recherche et consultation utilisateurs

#### UC_Utilisateurs - Système de filtres
- **Recherche multi-critères** :
  - Filtre texte : recherche dans login, nom affichage, code utilisateur
  - Filtre par rôle : User, SuperUser, Admin
  - Filtre par date de dernier login (avec gestion `.Date` pour comparaison exacte)
  - Filtre comptes verrouillés uniquement
- **Bouton Réinitialiser filtres** : remise à zéro de tous les critères de recherche
- Gestion d'état du bouton Réinitialiser (activé uniquement si filtres actifs)
- Filtrage en mémoire sur `_utilisateurs` (performance optimisée)

#### UtilisateurEdition - Mode Consultation
- **Nouveau mode `ModeUtilisateurEdition.Consultation`** pour SuperUser :
  - Tous les champs en lecture seule
  - Bouton Enregistrer masqué
  - Accès limité aux actions de maintenance :
    - Déverrouillage de compte (si verrouillé)
    - Réinitialisation de mot de passe
  - Titre et contexte spécifiques : "Consultation utilisateur"

#### UC_AdminHome - Accès SuperUser
- Accès de SuperUser à `UC_Utilisateurs` en mode consultation
- Boutons création/modification masqués pour SuperUser
- Double-clic sur utilisateur → ouverture en mode consultation

---

## 📅 23/05/2026 - 30/05/2026

### Module complet de gestion utilisateurs

#### UI / Forms
- **`UI/Forms/Utilisateur/UtilisateurEdition.vb`** (Form modale) :
  - Enum `ModeUtilisateurEdition` : Création, Modification, Consultation
  - Mode Création : génération et affichage du mot de passe temporaire, copie automatique dans presse-papiers
  - Mode Modification : login en lecture seule, gestion des états de sécurité
  - Actions administratives :
    - Réinitialisation de mot de passe via `ChangePassword` en mode `AdminReset`
    - Déverrouillage de compte avec confirmation
  - Validation exhaustive du formulaire (champs obligatoires, cohérence des rôles)
  - Gestion des états de sécurité : verrouillage, nombre d'échecs login, dates importantes
  - Implémentation `IContextAwareForm` pour intégration contexte UI

- **`UI/Controls/UC_Utilisateurs.vb`** (UserControl d'administration) :
  - Liste complète des utilisateurs avec DataGridView thématisée
  - Colonnes : État (icône), Code, Login, Nom, Rôle, Max Élévation, Actif, Verrouillé, Dernier login
  - Actions : Créer, Modifier, Activer/Désactiver (avec confirmation)
  - Double-clic sur ligne → ouverture `UtilisateurEdition` en modification
  - Gestion des droits selon le rôle connecté (Admin complet, SuperUser consultation)
  - Refresh automatique après chaque action
  - Implémentation `IContextAwareUserControl` pour injection du contexte

#### Métier / Security
- **`Metier/Security/GestionUtilisateurs.vb`** :
  - `CreateUtilisateur()` : création avec mot de passe hashé, gestion séquence MariaDB `LASTVAL(seq_sec_utilisateurs)`
  - `UpdateUtilisateur()` : modification avec vérification des droits (Admin uniquement)
  - `GenererMotDePasseTemporaire()` : génération sécurisée de mots de passe (12 caractères, complexité garantie)
  - `ResetPasswordUtilisateur()` : réinitialisation avec mot de passe temporaire auto-généré
  - `ResetPasswordUtilisateurWithCustomPassword()` : réinitialisation avec mot de passe personnalisé (via admin)
  - `DeverrouillerUtilisateur()` : déverrouillage de compte avec vérification droits (Admin ou SuperUser)
  - `ActiverDesactiverUtilisateur()` : activation/désactivation de compte (Admin uniquement)
  - `GetUtilisateurPourEdition()` : récupération d'un utilisateur complet pour édition/consultation
  - `GetUtilisateursPourListe()` : récupération de la liste complète pour affichage DataGridView
  - Journalisation détaillée de toutes les actions de sécurité via `GestionLog`

#### Gestion mot de passe centralisée
- **`UI/Forms/Login/ChangePassword.vb`** :
  - Refactorisation complète pour supporter deux modes :
    - `ModeChangePassword.UserChange` : changement par l'utilisateur (validation ancien mot de passe)
    - `ModeChangePassword.AdminReset` : réinitialisation par Admin (sans validation ancien mot de passe)
  - Validation de la complexité du nouveau mot de passe via `PasswordSecurityHelper`
  - Double saisie du nouveau mot de passe avec vérification de correspondance
  - Intégration dans `Home.vb` via bouton "Changer mot de passe" (menu principal)
  - Mise à jour automatique du flag `must_change_password` après changement réussi

#### Base de données
- Correction requête création utilisateur : utilisation de `LASTVAL(seq_sec_utilisateurs)` au lieu de `LAST_INSERT_ID()` pour récupération de l'ID généré par la séquence MariaDB
- Validation du type de paramètre pour les rôles : passage en String au lieu de Int32

#### Tests et validation
- **Création de `Docs/Tests/PLAN_TESTS_UC_UTILISATEURS.md`** : plan de tests exhaustif couvrant :
  - Chargement et affichage (liste, icônes, filtres)
  - Création d'utilisateur (validation, génération mot de passe, droits)
  - Modification d'utilisateur (champs, états, validation)
  - Activation/désactivation de compte
  - Réinitialisation de mot de passe
  - Déverrouillage de compte
  - Droits et sécurité (Admin vs SuperUser)
  - Robustesse et gestion d'erreurs
  - Logs et audit
  - Cohérence UI
  - Non-régression

- Tests manuels effectués et corrections appliquées :
  - Correction récupération ID après insertion (MariaDB sequence)
  - Correction type de paramètre pour rôles (String vs Int32)
  - Correction effacement sélection après chargement liste
  - Correction texte dynamique bouton Activer/Désactiver
  - Correction gestion icônes d'état (OK/OFF/Lock)
  - Désactivation bouton Reset Password en mode création
  - Initialisation correcte du contexte UI au chargement

---

## 📅 17/05/2026

### Audit complet du projet et synchronisation documentation

#### Audit et analyse
- **Audit approfondi** de l'ensemble du code source Althéa
- Analyse de la structure, de l'architecture, de la sécurité et de la qualité du code
- Identification des points d'amélioration et des bonnes pratiques déjà en place
- Vérification de la cohérence entre le code et la documentation existante

#### Documentation créée
- **`Docs/RAPPORT_AUDIT_2026-05-17.md`** : Rapport d'audit complet disponible dans `Docs/`
  - État global du projet
  - Points forts identifiés
  - Axes d'amélioration
  - Recommandations pour la suite du développement

#### Documentation mise à jour
- **`CHANGELOG.md`** : Synchronisation avec l'état réel du projet
- **`Docs/ETAT_DU_PROJET.md`** : Mise à jour de l'état actuel et des prochaines étapes

#### Conclusion audit
Projet en excellent état pour reprise :
- Aucun bug critique
- Architecture solide et documentée
- Sécurité robuste
- Code propre et maintenable
- Build qui passe

**Prêt pour la suite du développement**

---

## 📅 31/03/2026
- Création de la database de développement, avec les tables et les relations définies. 
---

## 📅 01/04/2026
- Réflexion sur la gestion des dossiers, documents et fichiers, avec les différentes options possibles.
---

## 📅 09/04/2026
- Etablissement de la charte graphique de l'application, avec les couleurs, les icônes et les éléments visuels.
- Définition des règles de développement et de gestion du projet, avec les bonnes pratiques à suivre.
- Création de la documentation technique pour l'interface utilisateur, avec les composants et les interactions.
- Préparation de la documentation technique de la base de données, avec les schémas et les relations.
---

## 📅 10/04/2026
- Ajout table medecins
- Ajout autres_suivis_patient
- Ajout système documents et modèles
---

## 📅 20/04/2026

### 🎉 Initialisation du projet

#### 🧱 Architecture
- Mise en place de la structure de base de la solution
- Validation des principes fondamentaux :
  - Form unique (Shell)
  - Navigation via UserControls
  - Séparation stricte UI / métier
  - Point d’accès DB unique

#### 🎨 Interface Utilisateur
- Création de la Form principale `Home`
- Implémentation de la structure Shell :
  - Menu gauche
  - Header (contexte)
  - Zone centrale dynamique
  - StatusStrip

- Application de la charte graphique :
  - Palette couleurs validée
  - Icônes intégrées
  - Style professionnel et épuré

#### 🧠 UX / Métier
- Organisation du menu principal :
  - Patients
  - Domaines
  - Agenda
  - Documents
  - Référentiels
  - Paramètres

- Décision :
  - Suppression du menu "Séances" (logique métier respectée)

#### ⚙️ Technique
- Décision d’utiliser le Designer pour le layout
- Mise en place du principe :
  - UI statique (Designer)
  - comportements dynamiques (code)

- Préparation du module `UtilsForm` (à implémenter)

### 📌 État
- UI principale fonctionnelle
- Architecture validée
- Socle prêt pour démarrage du code
---

## 📅 21/04/2026

### Added
- Mise en place du point d’entrée manuel (`Program.vb`)
- Création structure projet (Core / UI / Security)
- Ajout classe `LocalDbConfig`
- Ajout module `ConfigManager` (JSON local)
- Ajout module `DatabaseManager` (connexion MariaDB)
- Ajout module `CryptoManagerDPAPI` (chiffrement DPAPI)

### UI
- Création Form `Home` (shell)
- Création Form `ConfigurationConnexion`
- Mise en place design TableLayoutPanel

---

## 📅 22/04/2026

### Added
- Intégration `GestionLog` (adapté Artefact)
- Ajout gestion état UI :
  - `_isFormDirty`
  - `_isConnectionTestSuccessful`

### Added
- Processus complet :
  - test connexion DB
  - enregistrement config JSON
  - chargement config existante

### Security
- Ajout chiffrement mot de passe via DPAPI
- Mise en place utilisateur DB dédié (`althea_app`)

### UX
- Masquage mot de passe (`********`)
- Validation dynamique des champs
- Désactivation bouton Enregistrer sans test valide

### Fixed
- Gestion correcte host (`127.0.0.1`)
- Correction noms contrôles StatusStrip

### In Progress
- Gestion avancée du mot de passe (conserver/remplacer)
- Intégration complète du logging dans tous les modules
- Mise en place `AppStartupManager`
---

## 📅 22/04/2026

### Added
- Mise en place du point d’entrée manuel (`Program.vb`)
- Création structure projet (Core / UI / Security)
- Ajout classe `LocalDbConfig`
- Ajout module `ConfigManager` (JSON local)
- Ajout module `DatabaseManager` (connexion MariaDB)
- Ajout module `CryptoManagerDPAPI` (chiffrement DPAPI)

### UI
- Création Form `Home` (shell)
- Création Form `ConfigurationConnexion`
- Mise en place design TableLayoutPanel

### Added
- Intégration `GestionLog` (adapté Artefact)
- Ajout gestion état UI :
  - `_isFormDirty`
  - `_isConnectionTestSuccessful`

### Added
- Processus complet :
  - test connexion DB
  - enregistrement config JSON
  - chargement config existante

### Security
- Ajout chiffrement mot de passe via DPAPI
- Mise en place utilisateur DB dédié (`althea_app`)

### UX
- Masquage mot de passe (`********`)
- Validation dynamique des champs
- Désactivation bouton Enregistrer sans test valide

### Fixed
- Gestion correcte host (`127.0.0.1`)
- Correction noms contrôles StatusStrip

### In Progress
- Gestion avancée du mot de passe (conserver/remplacer)
- Intégration complète du logging dans tous les modules
- Mise en place `AppStartupManager`
---

## 📅 23/04/2026 - 24/04/2026 - 25/04/2026 -

### 🧠 Architecture / UI

- Validation complète du modèle **UserControl pour navigation**
- Abandon définitif des Forms pour les écrans principaux
- Mise en place du pattern :
  - Form `Home` = Shell
  - `UserControls` = écrans métier injectés

### 🖥️ UI – Structure

- Clarification du rôle de `pnlCenter` :
  - zone unique d’injection des vues
- Mise en place d’une méthode standard de chargement :

`Private Sub LoadView(view As UserControl)
    pnlCenter.Controls.Clear()
    view.Dock = DockStyle.Fill
    pnlCenter.Controls.Add(view)
End Sub`

### 🎛️ UI – Boutons standards
Validation du modèle unique de boutons :
- Centralisation du comportement via futur module UtilsForm
Décision :
- aucun code métier directement dans les événements boutons
- tout passe par des méthodes métier dédiées

### 🏠 UI – Home
Clarification du rôle :
- uniquement navigation + contexte
- aucune logique métier
- Ajout du concept :
- application verrouillée tant que DB non connectée

### 🗄️ Database
Stabilisation du flux :
- test connexion obligatoire avant sauvegarde config
- Validation du pattern :
- config locale JSON + DPAPI
Confirmation :
- DatabaseManager = point d’accès unique

### 🔐 UX / Sécurité
- Blocage de l’application si connexion invalide
- Validation stricte des champs avant test
- Interdiction d’enregistrer sans test réussi

### 📌 Décisions structurantes
- UI = 100% découplée du métier
- Navigation = centralisée
- DB = accès unique via manager
- Configuration = locale + sécurisée
---

## 📅 26/04/2026

### Added

- Création du `UserControl` `UC_Accueil`
- Création du `UserControl` `UC_AdminHome`
- Ajout bouton "Connexion Database" dans `UC_AdminHome`
- Création classe `NavigationManager`
- Implémentation navigation centralisée via `Navigate`

### Changed

- Refactoring navigation dans `Home` :
  - introduction méthode `NavigateTo`
  - remplacement de `LoadView` par `NavigationManager`
- Chargement automatique de `UC_Accueil` au démarrage
- Mise à jour gestion des boutons menu (cohérence UI complète)

### Removed

- Suppression bouton temporaire `ConfigurationConnexion` dans `Home`
- Suppression `lblContentPlaceholder`
- Suppression entrée "Paramètres" du menu principal

### Architecture

- Mise en place séparation :
  - Shell (`Home`)
  - Navigation (`NavigationManager`)
  - UI (UserControls)
- Préparation intégration future `UC_Parametres`

### UX

- Correction incohérence :
  - bouton sélectionné sans contenu affiché
- Navigation fluide entre écrans
---

## 📅 27/04/2026

### Documentation

#### Changed
- Refonte complète du README :
  - restructuration logique
  - conservation des informations existantes
  - amélioration lisibilité et cohérence

- Refonte complète du fichier Rules :
  - suppression des doublons
  - regroupement par thématiques
  - clarification des règles UI / DB / architecture

### UI

#### Changed
- Suppression du menu "Paramètres" du menu principal
- Intégration des paramètres dans l’espace Admin

#### Added
- Structuration de `UC_AdminHome` en hub (tuiles)
- Création du UserControl `UC_Parametres`
- Ajout TabControl pour gestion des groupes de paramètres

### Architecture

#### Added
- Création classe métier `ParametreApplication`
- Mise en place séparation UI / Métier / DB pour les paramètres

#### Added
- Création module `QueryParametres`
- Première requête SELECT des paramètres actifs

#### Added
- Création module `GestionParametres`
- Implémentation méthode `GetParametresActifs`

#### Changed
- Correction utilisation DatabaseManager :
  - remplacement de `GetConnexionMariaDB`
  - utilisation de `CreateConnection()` + `Open()`

### Structure projet

#### Added
- Nouveau dossier :
  Core/Database/Queries/

- Nouveau dossier :
  Metier/Parametres/

### Technical

#### Decision
- Validation architecture :
  UI → Métier → Query → DatabaseManager

- Adoption stratégie :
  1 module de queries par domaine

### Status

- UC_Parametres structuré
- Couche métier prête
- Lecture DB opérationnelle
- UI dynamique non encore implémentée
---

## 📅 28/04/2026

### Database

#### Added

* Création du module `QueryParametres`
* Implémentation de la requête `SelectParametresActifs`

#### Added

* Insertion de données initiales dans `tec_parametres` :

  * groupes : `GENERAL`, `PATHS`, `TECHNIQUE`
  * paramètres de chemins (APP_DATA, PATH_DOCUMENT, etc.)
  * exemple de paramètre BOOL

### Métier

#### Added

* Création de la classe `ParametreApplication`
* Création du module `GestionParametres`
* Implémentation de la méthode `GetParametresActifs`

#### Fixed

* Correction du mapping Reader → propriétés métier
* Prise en compte des champs NULL (valeur_parametre, description_parametre)

### Core

#### Added

* Ajout de la méthode `OpenConnection()` dans `DatabaseManager`

#### Changed

* Standardisation de l’accès DB :

  * CreateConnection() → création
  * OpenConnection() → ouverture
* Suppression des vérifications inutiles de type :

  * `conn IsNot Nothing`
  * `conn.State`

### UI

#### Added

* Création du UserControl `UC_Parametres`
* Chargement des paramètres depuis la DB
* Affichage du nombre de paramètres chargés

#### Added

* Regroupement des paramètres par `groupe_parametre`

#### Added

* Génération dynamique des onglets (`TabControl`) selon les groupes

#### Added

* Génération dynamique des contrôles :

  * Label + TextBox
  * Label + CheckBox (BOOL)

### UX

#### Added

* Structure UI dynamique basée sur la base de données

#### Identified

* Limites de l'affichage actuel (non adapté à la gestion complète)

### Architecture

#### Decision

* Centralisation des requêtes SQL dans `Core/Database/Queries`
* Séparation stricte :

  * UI
  * Métier
  * DB

#### Decision

* Les chemins sont définis comme segments logiques en base
* Construction des chemins via `Path.Combine`

### Security / Functional

#### Decision

* Interdiction de suppression physique des paramètres
* Gestion future par niveaux d'accès :

  * ADMIN
  * SUPERUSER

### Status

* Chargement DB fonctionnel
* UI dynamique fonctionnelle (version technique)
* Écran de gestion avancée à concevoir
---

## 📅 29/04/2026 & 30/04/2026

### Ajout

- Implémentation complète CRUD logique des paramètres
  - Création (INSERT)
  - Modification (UPDATE)
  - Désactivation (soft delete)
- Gestion des modes d’accès (Admin / SuperUser)
- Ajout du panneau détail dynamique
- Ajout du champ Description multiligne
- Ajout d’un système de validation avant sauvegarde
- Ajout d’un style standard DataGridView via UtilsForm

### Amélioration

- Conservation de la sélection après sauvegarde
- Synchronisation grille ↔ détail
- Amélioration de la gestion des états UI (boutons, édition)
- Amélioration UX (annulation, confirmation, cohérence)

### Correction

- Correction du bug de sauvegarde (valeurs non injectées dans l’objet)
- Correction du focus après rechargement
- Correction incohérences visuelles (labels, sélection)

### Ajout

- Chargement dynamique des paramètres par groupe
- Mise en place du DataGridView et du panneau détail
- Introduction des structures ParametreApplication
- Mise en place des premières règles UI (édition contrôlée)

### Amélioration

- Structuration du code (régions, séparation logique)
- Introduction des outils de log

### Correction

- Correction erreurs de connexion DB
- Correction gestion des contrôles dynamiques (WithEvents)
------

## 📅 01/05/2026

### ✨ Ajouts
- Mise en place UI globale (stsStatus, ttMain, errProvider)
- Début structuration Navigation + Context

### 🔄 Modifications
- suppression MessageBox inutiles
- centralisation feedback utilisateur

------

## 📅 02/05/2026

### ✨ Ajouts

- Ajout gestion affichage paramètres désactivés (CheckBox + SQL)
- Ajout validation Type/Valeur via UtilsValidation
- Ajout normalisation technique via UtilsString
- Ajout contrôle unicité clé paramètre

### 🔄 Modifications

- Remplacement TextBox Type par ComboBox
- Refonte complète validation (errProvider + messages + logs)
- Amélioration gestion contexte (header/status)
- Stabilisation événements UI (SelectionChanged / TabControl)

### 🐛 Corrections

- Bug SelectedTab = Nothing
- Perte du détail après reload
- Type non pris en compte à la sauvegarde
- erreurs persistantes errProvider

### 🧱 Architecture

- séparation stricte Query / Gestion / UI
- factorisation validation
- préparation réutilisation multi-UC

------

## 📅 03/05/2026

#### UC_Parametres

### Added

- Ajout de contrôles dynamiques selon `TypeValeur` :
  - `BOOL` → `CheckBox`
  - `INT` → `NumericUpDown`
  - `DECIMAL` → `NumericUpDown`
  - `DATE` → `DateTimePicker`
  - `STRING` / `PATH` → `TextBox`
- Ajout d’une colonne d’état dans le `DataGridView`.
- Ajout d’icônes actif / désactivé adaptées à la charte Althéa.
- Ajout de tooltips sur l’état des paramètres.
- Ajout d’une ComboBox éditable pour le champ Groupe, avec liste des groupes existants.

### Changed

- Refactorisation de `ChargerParametres`.
- Extraction de :
  - `CreerDataGridParametres`
  - `ConfigurerDataGridParametres`
  - `InitialiserSelection`
- Adaptation de `LireValeursDepuisUI` pour gérer les contrôles dynamiques.
- Stockage des dates en format ISO `yyyy-MM-dd`.
- Normalisation du groupe via `UtilsString.NormalizeTechnicalCode`.

### Fixed

- Correction du faux doublon de clé technique lors de la modification d’un paramètre existant.
- La vérification d’unicité de `CleParametre` est maintenant limitée au mode Nouveau.
- Suppression de doublons dans l’initialisation de sélection.
- Stabilisation du rechargement des détails après changement d’affichage actif/inactif.

### Decisions

- `UC_Parametres` reste le modèle de référence pour les futurs UserControls.
- Pas de factorisation commune prématurée des méthodes spécifiques à UC_Parametres.
- Les méthodes extraites restent locales tant que les répétitions ne sont pas confirmées dans d’autres UC.
---

## 📅 07/05/2026

### Added

* Création table `sec_utilisateurs`
* Création premier compte Admin bootstrap
* Implémentation PBKDF2 + salt
* Création `PasswordSecurityHelper`
* Création `GestionUtilisateurs`
* Création `UtilisateurApplication`
* Création `UserSession`
* Création `AppRole`
* Intégration Login réel dans `Home`

### Changed

* Suppression session DEV_ADMIN
* Session utilisateur désormais issue du Login réel

### Security

* Aucun mot de passe stocké en clair
* Aucun mot de passe dans les logs
* Hash + salt uniques par utilisateur
* SQL sécurité centralisé

### UI

* Création Designer `ChangePassword`
* Préparation UX sécurité
---

## 📅 09/05/2026

### Added

- Classe `AuthenticationResult`

- Gestion des échecs login persistants

- Verrouillage automatique des comptes après 5 échecs

- Colonnes DB :

  - `compte_verrouille`

  - `date_verrouillage`

- Méthodes métier :

  - `IncrementerNbEchecsLogin`

  - `VerrouillerCompteUtilisateur`

  - `ReinitialiserNbEchecsLogin`

  - `ChangerMotDePasse`

- Requêtes SQL sécurité associées

- Messages UI contextualisés :

  - essais restants

  - verrouillage compte

### Changed

- Refactoring complet de `AuthentifierUtilisateur`

- `Login.vb` utilise désormais `AuthenticationResult`

- Mapping utilisateur enrichi :

  - `CompteVerrouille`

  - `DateVerrouillage`

### Security

- Verrouillage persistant DB
- Réinitialisation compteur après succès
- Suppression des retours `Nothing` bruts
- Contrôle centralisé des messages sécurité
---

## 📅 10/05/2026

### Added

- Constructeur sécurisé pour `ChangePassword`
- Validation locale des saisies utilisateur
- Validation règles complexité mot de passe
- Gestion des messages validation :
  - `ErrorProvider`
  - `lblMessage`
  - `stsLabelStatus`
- Gestion changement réel de mot de passe
- Appel métier `GestionUtilisateurs.ChangerMotDePasse`
- Préparation affichage utilisateur connecté dans `Home`

### Changed

- Refactoring architecture `ChangePassword`
- Séparation stricte UI / sécurité / DB
- Amélioration UX validation mot de passe

### Security

- Validation ancien mot de passe
- Validation complexité minimale
- Aucune logique sécurité dans l’UI
- Aucun mot de passe loggé
- `must_change_password` désactivé après succès

### UI

- Ajout futur `lblPasswordRules`
- Ajout `lblUtilisateurConnecte`
- Messages validation désormais visibles dans StatusStrip
---

## 📅 11/05/2026

### Added

- Flux complet `MustChangePassword`
- Validation avancée `ChangePassword`
- Boutons visibilité password
- Affichage utilisateur connecté dans Home
- Contrôle accès `AdminHome`
- Gestion dynamique des droits dans `UC_AdminHome`
- Support `role_max_elevation`
- Architecture élévation temporaire
- Vérification métier `VerifierElevationUtilisateur`
- Formulaire `ElevationAcces`
- Élévation temporaire via `UserSession.ElevateTo`

### Changed

- Refonte architecture élévation
- Suppression logique passwords partagés
- Passage à élévation utilisateur individuelle
- Amélioration UX sécurité
- Mise à jour mapping utilisateurs

### Removed

- `GetUtilisateursActifsByRole`
- `VerifierMotDePasseRole`

### Security

- Validation élévation sécurisée
- Contrôle rôle maximum autorisé
- Aucune modification DB pendant élévation
- Logs sécurité élévation
- Passwords jamais loggués

### Database

- Ajout colonne :
  - `role_max_elevation`
  ---

## 📅 12/05/2026

### Added

- Champ DB `role_max_elevation`
- Architecture élévation utilisateur individuelle
- Validation métier `VerifierElevationUtilisateur`
- Support élévation session temporaire
- Boutons :
  - `btnEleverAcces`
  - `btnRetourRoleBase`
- Méthode `SetContexte`
- Navigation retour Accueil
- Affichage état élévation dans Home
- Bouton Voir dans ElevationAcces

### Changed

- Refonte complète logique élévation
- Suppression logique comptes/passwords partagés
- Synchronisation :
  - lblContexte
  - stsLabelStatus
- Recalcul dynamique droits AdminHome
- Gestion contexte navigation

### Security

- Élévation via password utilisateur personnel
- Contrôle RoleMaxElevation
- Retour sécurisé rôle de base
- Sortie automatique zones interdites
- Logs sécurité élévation
- Aucun password loggué

### UX

- Affichage clair état élévation
- Cohérence navigation/contextes
- Retour automatique Accueil si perte droits
---

## 📅 13/05/2026

## UI Architecture

### Added
- Nouveau module `UITheme`
- Nouveau module `UtilsButtons`
- Nouveau module `UtilsDataGrid`

### Changed
- Extraction complète des responsabilités UI de `UtilsForm`
- Centralisation des couleurs dans `UITheme`
- Migration des boutons vers `UtilsButtons`
- Migration DataGrid vers `UtilsDataGrid`

### Refactoring
- Suppression progressive du monolithe `UtilsForm`
- Séparation :
  - style visuel
  - comportements boutons
  - style DataGrid

## Buttons

### Added
- Gestion centralisée :
  - boutons Home
  - boutons standards 32px
  - boutons larges 48px

### Improved
- Gestion handlers sécurisée
- Gestion images fallback
- Gestion états hover/down/disabled

## UITheme

### Added
- Palette Althéa centralisée
- Couleurs DataGrid
- Couleurs contrôles dynamiques
- Couleurs boutons
- Assets paths
- Image suffixes

## UC_Parametres

### Changed
- Remplacement des couleurs hardcodées
- Utilisation de `UITheme`

## Security

### Decision
- Clôture du Process Sécurité avant démarrage UC_Utilisateurs

### Architecture
- Séparation :
  - sécurité applicative
  - gestion utilisateurs
---

## 📅 16/05/2026

### UI / Design

#### Added
- Ajout d’un module `UtilsControls` pour centraliser le comportement visuel des contrôles WinForms standards.
- Ajout d’un rendu OwnerDraw pour les ComboBox générées dynamiquement.
- Ajout d’un rendu personnalisé des items ComboBox afin d’éviter la sélection bleue Windows dans les listes déroulantes.
- Ajout de `TextRenderer.DrawText` pour améliorer la netteté du texte dans les ComboBox OwnerDraw.
- Ajout d’une initialisation centralisée des ToolTips via `InitializeToolTips()` dans les Forms concernées.

#### Changed
- Clarification de la séparation :
  - `UITheme` = couleurs, constantes, chemins
  - `UtilsButtons` = boutons
  - `UtilsDataGrid` = DataGridView
  - `UtilsControls` = contrôles WinForms standards
- Amélioration du rendu des ComboBox dynamiques de `UC_Parametres`.
- Ajustement du rendu vertical des items ComboBox via `ItemHeight` et dessin dans un Rectangle.
- Regroupement des ToolTips dans des procédures dédiées au lieu d’initialisations dispersées.

#### Fixed
- Correction du texte trop fin / tremblotant dans les ComboBox OwnerDraw.
- Correction de l’alignement vertical du premier item dans les listes ComboBox.
- Correction de la sélection visuelle dans les listes déroulantes ComboBox.

### UI / Contexte global

#### Added
- Création du principe `IContextAwareForm` pour les Forms modales ouvertes depuis `Home`.
- Injection possible de `UserControlContext` dans les Forms modales.
- Utilisation du contexte global pour `ElevationAcces`.
- Utilisation du contexte global pour `ConfigurationConnexion`.

#### Changed
- Clarification entre :
  - Forms autonomes ouvertes avant `Home`
  - Forms modales ouvertes depuis `Home`
- `Login` et `ChangePassword` conservent leur `StatusStrip`, `ToolTip` et `ErrorProvider` locaux.
- `ElevationAcces` et `ConfigurationConnexion` utilisent le contexte global quand elles sont ouvertes depuis `Home`.

#### Removed
- Suppression du `StatusStrip` local dans les écrans où le contexte global de `Home` doit être utilisé.
- Suppression de la logique locale de status dans `UC_AdminHome`.

### Architecture

#### Decision
- Les UserControls hébergés dans `Home` doivent utiliser `IContextAwareUserControl` et `UserControlContext`.
- Les Forms modales ouvertes depuis `Home` peuvent utiliser `IContextAwareForm`.
- Les Forms ouvertes avant `Home` restent autonomes.
- Les ToolTips doivent être regroupés dans une méthode `InitializeToolTips()`.
- Les ToolTips de `UC_AdminHome` sont reportés jusqu’à stabilisation complète de l’écran Admin.

### Documentation à mettre à jour
- `Rules.md`
- `Documentation_technique_UI_Althea.md`
- sections :
  - Home
  - UC_Parametres
  - UC_AdminHome
  - ConfigurationConnexion
  - ElevationAcces
  - Login
  - ChangePassword
  ---

  ## 📅 17/05/2026

  Corrections de quelques bugs relevés par Copilot:

  ### 1. NavigationManager.Navigate provoque des fuites mémoire
  `_pnlContent.Controls.Clear()` enlève les contrôles du panel mais ne les dispose pas. Si un UserControl possède des ressources (handlers, images, connexions), elles ne seront jamais libérées.

 À corriger :

 `For Each ctrl As Control In _pnlContent.Controls
    ctrl.Dispose()
Next
_pnlContent.Controls.Clear() `

  ### 2. PurgeOldLogs appelée à chaque log - dans le SyncLock
La purge est déclenchée à chaque appel de `EcrireLog`, y compris à l'intérieur du verrou critique. Elle accède au système de fichiers à chaque log. Il faut purger une seule fois au démarrage ou une fois par session.

  ### 3. IncrementerNbEchecsLogin : objet en mémoire non synchronisé
Dans `AuthentifierUtilisateur`, après `IncrementerNbEchecsLogin(utilisateur)`, la propriété `utilisateur.NbEchecsLogin` est lue pour calculer les tentatives restantes. Si la méthode ne met pas à jour l'objet en mémoire (seulement la DB), le calcul `MaxEchecsLogin - utilisateur.NbEchecsLogin` sera incorrect.

 ### 4. LocalDbConfig.LastConnectionTestUtc n'est jamais mise à jour
La propriété existe, mais aucun appel à `SaveLocalDbConfig` ne la renseigne après un test de connexion réussi dans `AppStartupManager`. Donnée en dérive.

  ### 5. Validation de port incomplète dans LocalDbConfig
`Port > 0` est vérifié mais la plage valide est 1–65535. Un port à 99999 passerait la validation.

  ### 6. AccessibleName et AccessibleDescription détournés pour stocker l'état UI
Dans `UtilsButtons.vb` :
	• `btn.AccessibleName = imageFolder.ToString()` → stocke le dossier d'image
	• `btn.AccessibleDescription = "selected"/"normal"/"disabled"` → stocke l'état du bouton
Ces propriétés sont faites pour les lecteurs d'écran (NVDA, JAWS…). Leur détournement casse l'accessibilité. La solution propre est d'utiliser un `Dictionary(Of Button, ButtonState)` ou une classe wrapper `ButtonStateManager`.

---

## 📅 20/05/2026 & 21/05/2026

## 🔐 Sécurité & Authentification

### ✅ Séparation des responsabilités sécurité

Refactorisation majeure de la couche sécurité afin de séparer :

- l’authentification utilisateur
- l’administration des utilisateurs

#### Nouveau module

Création de :

- `Metier/Security/GestionAuthentification.vb`

#### Répartition des responsabilités

##### GestionAuthentification

Responsable de :

- authentification utilisateur
- vérification mot de passe
- gestion des échecs login
- verrouillage compte
- mise à jour dernier login
- gestion des sessions

##### GestionUtilisateurs

Responsable de :

- gestion administrative des utilisateurs
- chargement listes utilisateurs
- édition utilisateurs
- activation/désactivation
- reset password
- gestion élévation

### ✅ Correction critique authentification

Correction d’un bug empêchant toute connexion utilisateur.

#### Cause

Le mapper utilisateur utilisé lors de l’authentification ne rechargeait plus :

- `password_hash`
- `password_salt`

Suite à une modification liée à la préparation de `UC_Utilisateurs`.

#### Impact

Erreur :

```text
Value cannot be null. (Parameter 's')
```
dans :
`Convert.FromBase64String(salt)`

### Correctif

Création d’un mapper dédié :

- `MapUtilisateurPourAuthentification`

Séparation stricte :

- mapper authentification
- mapper administration

### ✅ Mise à jour dernier login

Ajout :

- `UpdateDernierLoginUtilisateur`

et mise à jour automatique du champ :

- `dernier_login`

après authentification réussie.

## 👥 Gestion utilisateurs

## ✅ Préparation complète du module utilisateurs

Création de l’architecture du futur module :

- `UC_Utilisateurs`
- `UtilisateurEdition`

## ✅ Design UC_Utilisateurs

Mise en place du design complet :

#### Contrôles principaux

- `dgvUtilisateurs`
- `txtRechercheUtilisateur`
- `chkAfficherInactifs`
- `btnNouveau`
- `btnModifier`
- `btnActiverDesactiver`
- `btnResetPassword`
- `btnActualiser`

#### Décisions UI

- grille lecture seule
- sélection FullRowSelect
- colonnes techniques cachées
- actions désactivées sans sélection
- aucune donnée sensible affichée

### ✅ Structure DataGridView standardisée

Colonnes visibles :

- État
- Login
- Nom affiché
- Rôle
- Dernier login

Colonnes cachées :

- id utilisateur
- rôle max élévation
- sécurité
- audit

### ✅ Binding DataGridView

Ajout des :

- `DataPropertyName`
- binding objets métier
- chargement via `List(Of UtilisateurApplication)`

### ✅ GestionUtilisateurs

Ajout :

- `GetUtilisateursPourListe`
- `MapUtilisateurPourAdministration`

#### Règle importante

Aucune exposition :

- `password_hash`
- `password_salt`

hors authentification.

### ✅ QueryUtilisateurs

Réorganisation complète des requêtes SQL :

#### Authentification

- `SelectUtilisateurPourAuthentification`

#### Administration

- `SelectUtilisateursPourListe`
- `SelectUtilisateurPourEdition`
- `InsertUtilisateur`
- `UpdateUtilisateur`
- `UpdateActifUtilisateur`
- `ResetPasswordUtilisateur`
- `ForcerChangementPasswordUtilisateur`
- `DeverrouillerUtilisateur`

#### ✅ Navigation Admin

Ajout navigation :

- `UC_AdminHome`
   → `UC_Utilisateurs`

via :

```
NavigateToAdminView(
    New UC_Utilisateurs(),
    "Utilisateurs"
)
```
## 🧱 Architecture

### ✅ Cloisonnement renforcé

Validation des principes :

```
UI → Métier → Query → DatabaseManager
```

#### Règles renforcées

- aucun SQL dans UI
- aucun accès DB direct depuis UserControls
- séparation authentification / administration
- un mapper par contexte métier
- réutilisation stricte des helpers UI

## 🎨 UI & Standards

### ✅ Standardisation UtilisateurEdition

Définition des contrôles :

#### Identité

- login
- nom affiché

#### Droits

- rôle
- élévation max
- actif

#### Sécurité

- verrouillage
- échecs login
- dernier login

#### Audit

- création
- modification

------

### ✅ Uniformisation boutons password

Décision :

- même action métier
- même bouton
- même icône
- même tag asset

pour :

```
Reset PW
```
---

## 📅 20/05/2026

### 👥 Gestion Utilisateurs

#### ✅ Stabilisation architecture UC_Utilisateurs / UtilisateurEdition

Validation définitive de l’architecture :

`UC_Utilisateurs` = liste / navigation / actions rapides

`UtilisateurEdition` = fiche complète utilisateur

### Décisions importantes

Le UserControl reste volontairement léger :

- liste utilisateurs
- filtres
- navigation
- actions rapides

Toute la logique détaillée et sécurité utilisateur est déplacée dans :

```
UtilisateurEdition
```

### 🔐 Sécurité utilisateurs

#### ✅ Décision architecture : login immuable

Validation officielle :

```
login_utilisateur
= identifiant fonctionnel permanent
```

##### Conséquences

- login modifiable uniquement à la création
- login readonly en modification
- suppression des risques de rupture audit / logs / traçabilité
- cohérence sécurité long terme

### 🧱 UtilisateurEdition

#### ✅ Création structure complète de la Form

Ajout :

- `ModeUtilisateurEdition`
- constructeur typé
- implémentation `IContextAwareForm`
- gestion contexte Home
- variables privées métier
- organisation par régions

### ✅ Navigation modale

Depuis `UC_Utilisateurs` :

### Nouveau

```
New UtilisateurEdition(
    ModeUtilisateurEdition.Creation
)
```

### Modification

```
New UtilisateurEdition(
    ModeUtilisateurEdition.Modification,
    utilisateur.IdUtilisateur
)
```

## ✅ Gestion contexte Home

Ajout :

- `SetHeader`
- `SetStatus`

pendant l’ouverture de la Form.

#### Restauration automatique

Après fermeture modale :

```
Administration > Utilisateurs
```

restauré automatiquement.

### 🎨 UI Modes Création / Modification

#### ✅ Création

Mise en place :

- login modifiable
- champs audit readonly
- champs sécurité readonly
- verrouillage désactivé
- must_change_password coché par défaut

#### ✅ Modification

Mise en place :

- login readonly
- audit visible
- sécurité visible
- déverrouillage conditionnel

#### ✅ Stabilisation UX

Décision UI importante :

Les champs audit/sécurité ne sont plus masqués en création.

### Motivation

Éviter :

- formulaires “troués”
- décalages UI
- comportement visuel instable

Les champs restent visibles mais désactivés / vides.

### 📋 Chargement utilisateur

#### ✅ Préparation chargement réel

Ajout dans `GestionUtilisateurs` :

- `GetUtilisateurPourEdition`

Ajout dans `UtilisateurEdition` :

- `_utilisateur`
- `ChargerUtilisateur`
- `AlimenterControlesDepuisUtilisateur`

#### ✅ Initialisation combos rôles

Ajout :

- `InitialiserCombos`

pour :

- rôle utilisateur
- rôle max élévation

basés sur :

```
AppRole
```

### 🧪 Validation

#### Fonctionnel

✅ Navigation Home/Admin

 ✅ Navigation Utilisateurs

 ✅ Ouverture modale

 ✅ Retour contexte

 ✅ Chargement grille utilisateurs

 ✅ Double-clic modification

 ✅ États UI selon mode

 ✅ Gestion Header/Status

 ✅ Aucun crash

 ✅ Aucun warning critique

 ✅ Aucun log erreur

### 🚧 Prochaine étape

#### À implémenter

- chargement réel utilisateur dans Form
- sauvegarde création utilisateur
- sauvegarde modification utilisateur
- validations métier
- reset password réel
- déverrouillage réel
- activation/désactivation utilisateur

------

## 📅 17/05/2026

### 🔍 Audit complet du projet

#### Contexte
- Reprise du projet après 2 semaines d'absence
- Audit complet de la solution (60 fichiers analysés)
- Vérification : bugs, améliorations, documentation

#### ✅ Résultats de l'audit

**Bugs critiques** : ✅ Aucun

**Build** : ✅ Génération réussie

**Architecture** : ✅ Solide et cohérente
- Séparation Core / Metier / UI / Utils respectée
- Point d'accès DB unique (DatabaseManager)
- SQL centralisé (Query*)
- Navigation centralisée (NavigationManager)

**Sécurité** : ✅ Robuste
- PBKDF2 SHA256 (100k itérations)
- DPAPI pour secrets locaux
- Logs sans mots de passe
- Élévation temporaire bien tracée

**Documentation** : ✅ Riche et complète
- ETAT_DU_PROJET.md : état des lieux détaillé
- Rules.md : règles claires
- Standards-Commentaires.md : modèles exhaustifs
- Reference_UI_Controles.md : documentation UI détaillée

#### ⚠️ Points d'attention identifiés

1. **UC_Utilisateurs incomplet** (Dette technique DT-02)
   - ✅ Liste implémentée
   - ❌ Détail/édition utilisateur manquant
   - ❌ Actions admin (création, reset MDP, verrouillage, activation) manquantes

2. **TODO dans GestionUtilisateurs.vb** (ligne ~37-44)
   - Commentaire sur chargement auto utilisateur "joelle" en DEBUG
   - À traiter : implémenter ou supprimer

3. **Documentation légèrement désynchronisée**
   - Quelques dates de version à actualiser
   - Statut UC_Utilisateurs à préciser

#### 🚀 Améliorations proposées (13 identifiées)

**Factorisations** :
- Extraire méthode publique `GetUtilisateurByLogin` dans GestionUtilisateurs
- Validation centralisée des paramètres dans GestionParametres

**Optimisations** :
- Cache des paramètres applicatifs
- Query précompilées (MySqlCommand.Prepare)
- Lazy loading des UserControls

**Simplicité/Lisibilité** :
- Simplifier initialisation boutons (`InitializeButtons(ParamArray)`)
- Réduire indentations dans `AuthentifierUtilisateur`

**Souhaitable** :
- Rotation des logs (taille/heure)
- Fallback Event Log pour erreurs d'écriture log

#### 📋 Plan d'action priorisé

**Priorité 1 - CRITIQUE** : ✅ Aucune action critique (code stable)

**Priorité 2 - IMPORTANT** (cette semaine) :
1. Finaliser UC_Utilisateurs (liste + détail + actions admin)
2. Nettoyer TODO dans GestionUtilisateurs
3. Synchroniser documentation (ETAT_DU_PROJET, CHANGELOG)

**Priorité 3 - SOUHAITABLE** (ce mois-ci) :
4. Extraire `GetUtilisateurByLogin` publique
5. Validation centralisée paramètres
6. Simplifier initialisation boutons

**Priorité 4 - OPTIONNEL** (à planifier) :
7. Cache paramètres
8. Rotation logs
9. Refactoring AuthentifierUtilisateur
10. Lazy loading UserControls

#### 📄 Documentation créée
- **RAPPORT_AUDIT_2026-05-17.md** : Rapport d'audit complet disponible dans `Docs/`

#### 🎯 Conclusion
Projet en excellent état pour reprise :
- ✅ Aucun bug critique
- ✅ Architecture solide et documentée
- ✅ Sécurité robuste
- ✅ Code propre et maintenable
- ✅ Build qui passe

**Prêt pour la suite du développement** 🌿

------




> ---
>
> - **Contact** : ***Joëlle (Manou)  - Les Artefacts de Manou***
>
>   Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.
>
>   - Site web P.Nguyen Duy:  https://pearlnguyenduy.be/
>   - mailto: `joelle@nguyen.eu`
>
>   - GitHub privé: Althea    https://github.com/AngeljoNG/Althea
>   - GitHub public : Althea None
>
> ---

[TOC]

