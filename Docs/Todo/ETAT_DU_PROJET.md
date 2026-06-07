# Althéa - Etat du projet

> Document de synthèse vivant. Donne en un coup d'œil le statut réel du projet, les décisions actées, les dettes techniques connues et les prochaines étapes.
> Dernière mise à jour : 07 juin 2026 (post-implémentation gestion utilisateurs complète).

---

## Légende

| Icône | Signification |
|-------|---------------|
| ✓ | Implémenté et stable |
| ⚠ | Présent mais incomplet / en cours |
| ❌ | Non démarré |
| 🧪 | POC réalisé, non intégré au cœur |

---

## 1. Statut des modules

### 🏗️ Socle technique

| Module | Fichier(s) principal(aux) | Statut | Notes |
|--------|--------------------------|--------|-------|
| Démarrage applicatif | `Core/Startup/AppStartupManager.vb` | ✓ | Orchestration complète ; bloquant par conception |
| Configuration locale | `Core/Configuration/ConfigManager.vb`, `LocalDbConfig.vb` | ✓ | Fichier JSON chiffré dans `%APPDATA%\Althea` |
| Connexion base de données | `Core/Database/DatabaseManager.vb` | ✓ | Point d'accès DB unique, connexion MariaDB |
| Logging centralisé | `Core/Logging/GestionLog.vb` | ✓ | Sans exposition de données sensibles |
| Chiffrement secrets (DPAPI) | `Core/Security/CryptoManagerDPAPI.vb` | ✓ | Scope `CurrentUser`, Base64 |
| Hachage mots de passe | `Core/Security/PasswordSecurityHelper.vb` | ✓ | PBKDF2 |
| Rôles et session | `Core/Security/AppRole.vb`, `UserSession.vb` | ✓ | 3 rôles : User / SuperUser / Admin |

### 🧩 Interface utilisateur

| Module | Fichier(s) principal(aux) | Statut | Notes |
|--------|--------------------------|--------|-------|
| Shell principal | `UI/Forms/Home.vb` | ✓ | Form principale + verrouillage démarrage |
| Navigation centralisée | `UI/Navigation/NavigationManager.vb` | ✓ | Contexte partagé `UserControlContext` |
| Dialogue personnalisé | `UI/Forms/Communs/DialogChoix.vb` | ✓ | Remplace tous les MessageBox, support icônes animées, thématisation UITheme |
| Écran connexion | `UI/Forms/Login/` | ✓ | Authentification, changement MDP |
| Écran configuration DB | `UI/Forms/Database/ConfigurationConnexion.vb` | ✓ | |
| Accueil / AdminHome | `UC_Accueil`, `UC_AdminHome` | ✓ | Hub d'administration complet avec élévation de droits |
| Paramètres | `UC_Parametres` | ✓ | CRUD complet + contrôle d'accès |
| Utilisateurs | `UC_Utilisateurs` | ✓ | Liste, recherche/filtres, création, modification, consultation, activation/désactivation |
| Édition utilisateur | `UtilisateurEdition.vb` | ✓ | Form modale : création/modification/consultation, reset password, déverrouillage |
| Utils UI centralisés | `UtilsIcons.vb`, `UtilsButtons.vb`, `UtilsDataGrid.vb` | ✓ | Centralisation icônes d'état, styles boutons, config grilles |

### 🧠 Cœur métier

| Module | Statut | Notes |
|--------|--------|-------|
| Patients (recherche, fiche) | ⚠ | Tables DB prêtes ; UI et logique métier à créer |
| Dossiers (statuts, cycle de vie) | ⚠ | |
| Séances (saisie, historique) | ⚠ | |
| Paiements (liaison séances) | ⚠ | |
| Documents | 🧪 | POC Drive/DocIO validé ; intégration non faite |
| Agenda | 🧪 | POC Google Calendar / Scheduler validé ; intégration non faite |

### 🗄️ Base de données

| Périmètre | Statut | Notes |
|-----------|--------|-------|
| Tables techniques (`tec_*`) | ✓ | Paramètres, méta-schéma |
| Tables sécurité (`sec_*`) | ✓ | `sec_utilisateurs` avec gestion complète des rôles, verrouillage, élévation |
| Tables référentielles (`ref_*`) | ✓ | Statuts, types, volets |
| Tables métier (patients, dossiers, séances, paiements, documents) | ✓ schéma / ⚠ branché | Schéma SQL existant ; modules applicatifs non connectés |
| Scripts versionnés complets | ⚠ | Partie technique et sécurité OK ; partie métier à compléter |
| Séquences MariaDB | ✓ | Gestion correcte via `LASTVAL(seq_sec_utilisateurs)` |

### 🧩 Qualité & exploitation

| Élément | Statut | Notes |
|---------|--------|-------|
| Documentation technique | ✓ | README, Process, Database, Rules, Standards, Architecture Decisions |
| Documentation UI complète | ✓ | `Documentation_technique_UI_Althea.md` avec toutes les Forms et UserControls |
| Documents de pilotage (Checklist / ToDo / Planning) | ✓ | Vues synchronisées dans `Docs/Todo/` |
| Plan de tests utilisateurs | ✓ | `PLAN_TESTS_UC_UTILISATEURS.md` complet |
| Tests fonctionnels transverses | ⚠ | Campagne à lancer avant gel V1 |
| Sauvegarde / restauration | ⚠ | Stratégie à industrialiser |
| Package d'installation | ⚠ | Configuration initiale guidée à prévoir |

---

## 2. Décisions actées

| # | Décision | Statut |
|---|----------|--------|
| D-01 | Base de données centrale MariaDB (UTF8MB4) | ✓ Actée |
| D-02 | Chiffrement des secrets locaux par DPAPI (`CurrentUser`) | ✓ Actée |
| D-03 | Hachage mots de passe PBKDF2 | ✓ Actée |
| D-04 | Architecture en couches : `Core` / `Metier` / `UI` / `Utils` | ✓ Actée |
| D-05 | Point d'accès DB unique (`DatabaseManager`) | ✓ Actée |
| D-06 | SQL centralisé dans des modules `Query*` dédiés | ✓ Actée |
| D-07 | Démarrage applicatif bloquant (DB obligatoire à l'ouverture) | ✓ Actée |
| D-08 | Système de rôles à 3 niveaux (User / SuperUser / Admin) | ✓ Actée |
| D-09 | Navigation centralisée via `NavigationManager` | ✓ Actée |
| D-10 | Modes utilisateur distincts (Création/Modification/Consultation) | ✓ Actée |
| D-11 | Remplacement complet de MessageBox par DialogChoix personnalisé | ✓ Actée |
| D-12 | Centralisation des icônes d'état via UtilsIcons | ✓ Actée |
| D-13 | Intégration documents V1 depuis POC (périmètre à définir) | ⚠ À décider |
| D-14 | Intégration agenda V1 depuis POC (périmètre à définir) | ⚠ À décider |

> ⚠ Détail et justification de chaque décision : voir [`ARCHITECTURE_DECISIONS.md`](./ARCHITECTURE_DECISIONS.md).

---

## 3. Dettes techniques connues

| Réf | Description | Priorité | Impact |
|-----|-------------|----------|--------|
| DT-01 | Build `.NET` bloqué en environnement Linux (NETSDK1100, projet WinForms cible Windows) | Faible | Uniquement pour CI non-Windows ; aucun impact sur l'exécution réelle |
| DT-02 | ~~`UC_Utilisateurs` non finalisé~~ | ~~Haute~~ | ✓ **RÉSOLU** - Module complet et opérationnel |
| DT-03 | Scripts SQL métier non entièrement versionnés pour les modules patients/dossiers/séances/paiements | Moyenne | Complexifie les mises à jour de schéma futures |
| DT-04 | Scénarios d'exploitation (tests erreurs/permissions) non durcis pour modules métier | Moyenne | Risque de comportements inattendus aux limites |
| DT-05 | Intégration des POC documents et agenda non planifiée précisément | Moyenne | Dépendance à une décision de périmètre (D-13, D-14) |
| DT-06 | Absence de campagne de tests fonctionnels transverses | Haute | Requise avant tout gel de la V1 |
| DT-07 | Documentation illustrations manquantes | Faible | Images à ajouter pour DialogChoix, UtilisateurEdition, UC_Utilisateurs |

---

## 4. Prochaines étapes (ordre de priorité)

### ~~Priorité 1 - Finaliser l'administration~~ ✓ **TERMINÉ**
1. ~~Terminer `UC_Utilisateurs` (liste + détail + actions admin + contrôle d'accès + journalisation)~~ ✓
2. ~~Relire et fiabiliser le flux complet Login → Home → Admin~~ ✓
3. ~~Implémenter DialogChoix pour remplacer tous les MessageBox~~ ✓
4. ~~Centraliser les icônes d'état via UtilsIcons~~ ✓

### Priorité 2 - Premier lot métier (patients/dossiers)
5. Créer les modules métier Patients (recherche + fiche multi-onglets)
6. Créer la gestion des dossiers avec leurs statuts et transitions

### Priorité 3 - Suivi et paiements
7. Implémenter la gestion des séances (saisie + historique)
8. Implémenter la gestion des paiements (liaison séances + suivi financier minimal)

### Priorité 4 - Documents et agenda (intégration depuis POC)
9. Définir le périmètre V1 minimal pour les documents
10. Définir le périmètre V1 minimal pour l'agenda
11. Intégrer les deux modules dans le code principal

### Priorité 5 - Exploitation et stabilisation
12. Industrialiser la sauvegarde/restauration DB + fichiers
13. Préparer le package d'installation avec configuration initiale guidée
14. Exécuter la campagne de tests fonctionnels transverses
15. Effectuer les corrections UX finales et durcir les cas limites
16. Clôturer le changelog et préparer la note de version V1

---

## 5. Risques identifiés

| Risque | Probabilité | Impact | Mitigation |
|--------|-------------|--------|------------|
| Périmètre V1 qui s'étend (feature creep) | Moyenne | Élevé | Gel explicite du périmètre par module avant codage |
| Qualité du cœur métier insuffisante sans tests | Haute | Élevé | Planifier la recette fonctionnelle dès la phase 3 |
| Reprise du projet par un tiers sans documentation suffisante | Faible | Élevé | Maintenir ce fichier et les ADR à jour à chaque évolution majeure |
| Dépendance à des services externes (Google Drive, Google Calendar) dans les POC | Moyenne | Moyen | Définir une alternative locale si intégration impossible en V1 |

---

## 6. Réalisations récentes (depuis audit 17/05/2026)

### Gestion utilisateurs complète ✓
- **UC_Utilisateurs** : liste avec recherche/filtres avancés (nom, login, rôle, état, date), affichage icônes d'état (actif/inactif/verrouillé), actions admin complètes
- **UtilisateurEdition** : Form modale avec 3 modes (Création/Modification/Consultation), génération mot de passe temporaire, reset password, déverrouillage
- **GestionUtilisateurs** : couche métier complète (CRUD, activation/désactivation, reset password, unlock, gestion séquences MariaDB)
- **Gestion des rôles** : distinction Admin (modification complète) / SuperUser (consultation + actions sécurité)

### UI/UX cohérente ✓
- **DialogChoix** : remplacement complet de MessageBox, support icônes animées GIF, thématisation UITheme, 1-3 boutons configurables
- **UtilsIcons** : centralisation des icônes d'état avec priorité (verrouillé > actif > inactif)
- **UtilsButtons** : gestion cohérente des styles boutons dans toute l'application

### Documentation exhaustive ✓
- Mise à jour complète de **CHANGELOG.md** (chronologie inversée, détails depuis 17/05)
- Mise à jour **README.md** (ton professionnel, structure claire)
- Mise à jour **Rules.md** (règles DialogChoix, UtilsIcons, modes utilisateur, contextes)
- Mise à jour **Process_Althea.md** (Processus 06 Gestion utilisateurs, Processus 07 DialogChoix, diagrammes Mermaid)
- Mise à jour **ARCHITECTURE_DECISIONS.md** (ADR-10 modes utilisateur, ADR-11 DialogChoix, ADR-12 UtilsIcons)
- **Documentation_technique_UI_Althea.md** : sections complètes pour DialogChoix, UtilisateurEdition, UC_Utilisateurs

### Tests et validation ✓
- Plan de tests exhaustif créé (`PLAN_TESTS_UC_UTILISATEURS.md`)
- Tests manuels effectués et validés
- Corrections des bugs identifiés pendant les tests
- Validation des workflows création/modification/consultation/reset/unlock
