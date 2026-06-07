# Althea” Checklist projet V1 (référence consolidée)


> Document de référence unique pour l'état d'avancement V1.  
> Les vues opérationnelles et planning sont synchronisées dans :
> - `Docs/Todo/ToDo.md` (backlog actionnable)
> - `Docs/Todo/Planning_actions_Althéa.md` (feuille de route)
> 
>  *Dernière mise à jour : 07/06/2026*

---

## Légende

- ✅ **Fait** : implémenté dans le code principal
- ⚠️ **Partiel / en cours** : présent mais incomplet
- ❌ **À faire** : non démarré dans le code principal
- 🛠️ **POC** : exploré dans un projet séparé, non intégré au cœur

---

## 0) Synthèse globale (état actuel)

### ✅ Déjà accompli (socle V1)

- Architecture technique en place : `Core`, `UI`, `Metier`, `Utils`, `Docs`
- Démarrage applicatif sécurisé : `Program` + `AppStartupManager`
- Configuration locale + chiffrement DPAPI : `ConfigManager`, `CryptoManagerDPAPI`
- Accès base centralisé : `DatabaseManager` + requêtes centralisées (`QueryParametres`, `QueryUtilisateurs`)
- Logging central : `GestionLog`
- Navigation centralisée : `NavigationManager` + contexte partagé (`UserControlContext`)
- Shell UI principal : `Home`
- Sécurité utilisateurs : Login, changement mot de passe, élévation d'accès, session/rôles
- Module Paramètres : gestion complète (CRUD + contrôles d'accès)
- **Module Utilisateurs complet** : `UC_Utilisateurs` + `UtilisateurEdition` (création/modification/consultation, recherche/filtres, reset password, déverrouillage)
- **DialogChoix personnalisé** : remplacement complet de MessageBox, icônes animées, thématisation
- **Utils UI centralisés** : `UtilsIcons`, `UtilsButtons`, `UtilsDataGrid`, `UtilsValidation`
- Documentation technique exhaustive (README, Process, Database, Rules, Standards, Architecture Decisions, Documentation UI)

### ❌ Reste majeur à implémenter

- Domaine métier principal : **Patients, Dossiers, Séances, Paiements**
- Modules UI métier correspondants
- Workflow documentaire intégré au cœur (hors POC)
- Agenda intégré au cœur (hors POC)
- Installation, sauvegarde/restauration, tests de stabilisation finaux

### 🛠️ POC déjà réalisés (hors code principal)

- Gestion documentaire (Drive/Docs, DocIO/DocToPdf)
- Agenda (Google Calendar / Scheduler)

---

## 1) Checklist détaillée V1

## 1. Cadrage fonctionnel V1

- ✅ Modèle métier de base clarifié (Patient / Dossier / Séance / Paiement / Document)
- ✅ Orientation produit explicitée dans le README et les docs process
- ⚠️ Clarification finale du volet "Realism" (contenu métier précis à formaliser)
- ❌ Gel final du périmètre V1 (in/out définitif par module)

## 2. Architecture et socle applicatif

- ✅ Solution/projet WinForms .NET 8 structuré
- ✅ Dossiers techniques et séparation des responsabilités en place
- ✅ Modules socle créés : `DatabaseManager`, `ConfigManager`, `GestionLog`, `AppStartupManager`
- ✅ Navigation centralisée (`NavigationManager`) et contexte UI partagé (`UserControlContext`)
- ✅ Helpers UI mutualisés (`UITheme`, `UtilsButtons`, `UtilsControls`, `UtilsDataGrid`, `UtilsValidation`, `UtilsString`, `UtilsIcons`)
- ✅ DialogChoix personnalisé implémenté et déployé dans toute l'application

## 3. Base de données

- ✅ Schéma/tables techniques exploitées pour la configuration/sécurité/paramètres
- ✅ Tables utilisateurs complètes avec gestion rôles, verrouillage, élévation
- ✅ Gestion séquences MariaDB opérationnelle (`LASTVAL(seq_sec_utilisateurs)`)
- ✅ Documentation DB disponible
- ⚠️ Tables métier complètes V1 (patients, dossiers, suivis, paiements, documents) à finaliser/brancher
- ⚠️ Scripts SQL versionnés de bout en bout à compléter pour la partie métier

## 4. Sécurité et accès

- ✅ Authentification utilisateur opérationnelle
- ✅ Gestion rôles (`User`, `SuperUser`, `Admin`) et élévation temporaire
- ✅ Chiffrement des secrets (DPAPI) + hachage mots de passe (PBKDF2)
- ✅ Journalisation sans exposition de données sensibles (principes en place)
- ✅ Gestion complète utilisateurs (création, modification, consultation selon rôles)
- ✅ Actions admin sécurisées (reset password, déverrouillage compte)
- ✅ Gestion mots de passe temporaires sécurisés
- ⚠️ Durcir les scénarios d'exploitation (tests d'erreurs/permissions complets pour modules métier)

## 5. UI — coque et administration

- ✅ Form principale `Home` opérationnelle
- ✅ Écrans d'accès : login, changement mot de passe, configuration connexion, élévation accès
- ✅ `UC_Accueil`, `UC_AdminHome`, `UC_Parametres` opérationnels
- ✅ `UC_Utilisateurs` complet (liste, recherche/filtres, création, modification, consultation, activation/désactivation)
- ✅ `UtilisateurEdition` complet (Form modale avec 3 modes, reset password, déverrouillage)
- ✅ `DialogChoix` opérationnel (remplacement complet MessageBox)
- ✅ Icônes d'état centralisées (`UtilsIcons`) avec priorités (verrouillé > actif > inactif)
- ⚠️ Écrans métier patients/dossiers/séances/paiements à créer

## 6. Métier patients/dossiers/suivi

- ⚠️ Recherche patients
- ⚠️ Fiche patient multi-onglets
- ⚠️ Gestion dossiers (statuts, cycle de vie)
- ⚠️ Gestion séances (historique + saisie)
- ⚠️ Gestion paiements et liaison aux séances

## 7. Documents & agenda

- ⚠️ POC documents validé, intégration applicative non faite
- ⚠️ POC agenda validé, intégration applicative non faite
- ⚠️ Décider lot d'intégration V1 (minimum viable)

## 8. Sauvegarde, restauration, installation

- ⚠️ Stratégie backup DB + fichiers à industrialiser
- ⚠️ Procédure de restauration testée de bout en bout
- ⚠️ Package d'installation + initialisation guidée

## 9. Qualité, tests, stabilisation

- ✅ Base documentaire structurée et exhaustive
- ✅ Documentation UI complète (`Documentation_technique_UI_Althea.md`)
- ✅ Documentation architecture à jour (ADR, Process, Rules)
- ✅ Plan de tests utilisateurs créé (`PLAN_TESTS_UC_UTILISATEURS.md`)
- ✅ Tests manuels UC_Utilisateurs réalisés et validés
- ⚠️ Campagne de tests fonctionnels transverses avant gel V1
- ⚠️ Corrections UX finales et durcissement erreurs limites

---

## 2) Risques/contraintes identifiés

- Le build/test `dotnet` échoue en environnement Linux pour ce projet WinForms cible Windows (contrainte d'environnement à prendre en compte pour CI non-Windows).
- Forte dépendance à l'implémentation du cœur métier (patients/dossiers/séances/paiements) pour obtenir une V1 réellement exploitable.
- Illustrations manquantes dans la documentation UI (DialogChoix, UtilisateurEdition, UC_Utilisateurs).

---

## 3) Prochaine cible prioritaire

1. ~~Finaliser `UC_Utilisateurs`.~~ ✅ **TERMINÉ**
2. Livrer un premier lot métier utilisable : recherche + fiche patient + dossier.
3. Enchaîner sur séances + paiements.
4. Intégrer ensuite documents/agenda (au moins périmètre V1 minimal).
5. Terminer par sauvegarde/restauration + installation + stabilisation.

---

## 4) Réalisations récentes (depuis audit 17/05/2026)

### ✅ Administration complète (23/05-07/06/2026)

- **Gestion utilisateurs** : module complet opérationnel
  - `UC_Utilisateurs` : liste, recherche/filtres avancés, actions admin
  - `UtilisateurEdition` : création/modification/consultation, reset password, déverrouillage
  - `GestionUtilisateurs` : couche métier complète
  - Gestion rôles Admin/SuperUser avec droits différenciés

- **UI/UX cohérente**
  - `DialogChoix` : remplacement de tous les MessageBox (34 occurrences)
  - `UtilsIcons` : centralisation icônes d'état avec priorités
  - Support icônes animées GIF

- **Documentation exhaustive**
  - Mise à jour complète CHANGELOG, README, Rules, Process_Althea, Architecture_Decisions
  - Nouvelle documentation UI complète pour DialogChoix, UtilisateurEdition, UC_Utilisateurs
  - Plan de tests exhaustif créé et exécuté

- **Tests et corrections**
  - Tests manuels complets effectués
  - Bugs identifiés et corrigés (séquences MariaDB, rôles, filtres, sélection, états)
  - Validation workflows création/modification/consultation/reset/unlock
