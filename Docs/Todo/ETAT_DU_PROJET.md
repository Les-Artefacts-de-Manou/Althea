# Althéa - Etat du projet

> Document de synthèse vivant. Donne en un coup d'œil le statut réel du projet, les décisions actées, les dettes techniques connues et les prochaines étapes.
> Dernière mise à jour : 08 juin 2026 (post-cadrage métier : décisions de conception actées).

> 📐 **Plan directeur de la phase métier** : [`Docs/Conception/Plan_Conception_Metier_Althea.md`](../Conception/Plan_Conception_Metier_Althea.md) — source de vérité détaillée des décisions de cadrage (D-Q1 à D-Q10).

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
| Patients (recherche, fiche) | ⚠ | Tables DB prêtes ; UI et logique métier à créer. Réseau d'intervenants N-N acté (D-Q1bis) |
| Dossiers (statuts, cycle de vie) | ⚠ | Réouverture si même domaine ; transfert inter-domaines actés (D-Q5, D-Q10) |
| Séances (saisie, historique) | ⚠ | Créée dès planification du RDV lié, statut via `ref_statuts_seance` (D-Q6) |
| Paiements (liaison séances) | ⚠ | |
| Documents | 🧪 → périmètre acté | POC Drive/DocIO validé ; **V1 actée** : fichiers hors DB, chemin déterministe, Word local **et** Google Docs (D-Q4/D-Q7) |
| Agenda | 🧪 → périmètre acté | POC Google Calendar/Scheduler validé ; **V1 actée** : Google = pilier, sync bidirectionnelle, reprise assistée (D-Q8) |

### 🗄️ Base de données

| Périmètre | Statut | Notes |
|-----------|--------|-------|
| Tables techniques (`tec_*`) | ✓ | Paramètres, méta-schéma |
| Tables sécurité (`sec_*`) | ✓ | `sec_utilisateurs` avec gestion complète des rôles, verrouillage, élévation |
| Tables référentielles (`ref_*`) | ✓ schéma / ⚠ à migrer | Statuts, types, **domaines** (ex-`volets`, D-Q1) ; à créer : `ref_roles_intervenant` (D-Q1bis) |
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
| D-13 | Intégration documents V1 : stockage **hors DB** (système de fichiers), chemin déterministe, flux Word local + Google Docs, export PDF | ✓ Actée |
| D-14 | Intégration agenda V1 : Google Calendar comme pilier, synchronisation bidirectionnelle, reprise assistée | ✓ Actée |
| D-15 | Composant `UC_RichTextEditor` + variante compacte `UC_RichTextEditorSimple` (double format) | ✓ Actée |
| D-16 | Renommages métier : `volets`→`domaines`, `medecins`→`therapeutes`, réseau de suivi en N-N (`autres_suivis_patient` + `ref_roles_intervenant`) | ✓ Actée |
| D-17 | Séance créée dès la planification du rendez-vous lié, statut piloté par `ref_statuts_seance` | ✓ Actée |
| D-18 | UserControl dédié par référentiel sur une base commune (`UC_ReferentielBase`) | ✓ Actée |
| D-19 | Anticipation multi-utilisateur / multi-agenda dès la V1 (modèle préparé, activation différée) | ✓ Actée |

> ⚠ Détail et justification de chaque décision : voir [`ARCHITECTURE_DECISIONS.md`](../Rules/ARCHITECTURE_DECISIONS.md) (ADR-13 à ADR-19) et le [plan de conception métier](../Conception/Plan_Conception_Metier_Althea.md).

---

## 3. Dettes techniques connues

| Réf | Description | Priorité | Impact |
|-----|-------------|----------|--------|
| DT-01 | Build `.NET` bloqué en environnement Linux (NETSDK1100, projet WinForms cible Windows) | Faible | Uniquement pour CI non-Windows ; aucun impact sur l'exécution réelle |
| DT-02 | ~~`UC_Utilisateurs` non finalisé~~ | ~~Haute~~ | ✓ **RÉSOLU** - Module complet et opérationnel |
| DT-03 | Scripts SQL métier non entièrement versionnés pour les modules patients/dossiers/séances/paiements | Moyenne | Complexifie les mises à jour de schéma futures |
| DT-04 | Scénarios d'exploitation (tests erreurs/permissions) non durcis pour modules métier | Moyenne | Risque de comportements inattendus aux limites |
| DT-05 | ~~Intégration des POC documents et agenda non planifiée~~ | ~~Moyenne~~ | ✓ **RÉSOLU** - Périmètre V1 acté (D-13, D-14) ; reste l'implémentation |
| DT-06 | Absence de campagne de tests fonctionnels transverses | Haute | Requise avant tout gel de la V1 |
| DT-07 | Documentation illustrations manquantes | Faible | Images à ajouter pour DialogChoix, UtilisateurEdition, UC_Utilisateurs |
| DT-08 | Migration de schéma à réaliser (Lot 0) : `volets`→`domaines`, `medecins`→`therapeutes`, liaison N-N `autres_suivis_patient`, `ref_roles_intervenant`, `ref_statuts_seance` | Haute | Prérequis bloquant des modules métier ; à versionner avant tout codage Lot 1 |

---

## 4. Prochaines étapes (ordre de priorité)

### ~~Priorité 1 - Finaliser l'administration~~ ✓ **TERMINÉ**
1. ~~Terminer `UC_Utilisateurs` (liste + détail + actions admin + contrôle d'accès + journalisation)~~ ✓
2. ~~Relire et fiabiliser le flux complet Login → Home → Admin~~ ✓
3. ~~Implémenter DialogChoix pour remplacer tous les MessageBox~~ ✓
4. ~~Centraliser les icônes d'état via UtilsIcons~~ ✓

### Priorité 2 - Lot 0 : socle métier (migration de schéma) ⏳ **PRÉREQUIS BLOQUANT**
5. Versionner et appliquer la migration : `volets`→`domaines`, `medecins`→`therapeutes`
6. Créer la liaison N-N `autres_suivis_patient` + le référentiel `ref_roles_intervenant`
7. Créer/aligner `ref_statuts_seance` (cycle de vie séance)
8. Créer la base UI commune `UC_ReferentielBase` + composants `UC_RichTextEditor` / `UC_RichTextEditorSimple`

### Priorité 3 - Lot 1 : Patients & dossiers
9. Créer les modules métier Patients (recherche + fiche multi-onglets + réseau d'intervenants)
10. Créer la gestion des dossiers avec leurs statuts, transitions, réouverture et transfert inter-domaines

### Priorité 4 - Lot 2 : Suivi et paiements
11. Implémenter la gestion des séances (création dès planification RDV + historique + statuts)
12. Implémenter la gestion des paiements (liaison séances + suivi financier minimal)

### Priorité 5 - Lot 3 : Documents et agenda (intégration depuis POC)
13. Intégrer le module documents V1 (fichiers hors DB, chemin déterministe, Word local + Google Docs, export PDF)
14. Intégrer le module agenda V1 (Google Calendar pilier, sync bidirectionnelle, reprise assistée)

### Priorité 6 - Exploitation et stabilisation
15. Industrialiser la sauvegarde/restauration DB + fichiers
16. Préparer le package d'installation avec configuration initiale guidée
17. Exécuter la campagne de tests fonctionnels transverses
18. Effectuer les corrections UX finales et durcir les cas limites
19. Clôturer le changelog et préparer la note de version V1

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

### Cadrage de la phase métier ✓ (08/06/2026)
- **Plan de conception métier** rédigé et validé : [`Plan_Conception_Metier_Althea.md`](../Conception/Plan_Conception_Metier_Althea.md)
- **Décisions actées** (D-13 à D-19) : stockage documents hors DB à chemin déterministe, Google Docs/Drive/Calendar piliers V1, renommages `domaines`/`therapeutes`, réseau d'intervenants N-N, séance créée dès la planification, UC dédié par référentiel, anticipation multi-utilisateur
- **ADR mis à jour** : ARCHITECTURE_DECISIONS.md (ADR-13 à ADR-19) et Rules.md (terminologie + double format éditeur de texte)

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
