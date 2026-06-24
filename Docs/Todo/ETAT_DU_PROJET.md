# Althéa - Etat du projet

> Document de synthèse vivant. Donne en un coup d'œil le statut réel du projet, les décisions actées, les dettes techniques connues et les prochaines étapes.
> Dernière mise à jour : 10 juin 2026 (Lot 0 socle métier terminé : référentiels + éditeurs de texte).

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
| Helper chemins patients | `Utils/Helpers/CheminsPatientHelper.vb` | ✓ | Refactoré : `code_patient`, `PATH_GENERAL\PATH_DOCUMENT\{code}`, `AssurerDossierPatient`, `GetNomFichierPhotoIdentite` (ADR-20) |

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
| Éditeur texte riche complet | `UC_RichTextEditor.vb` | ✓ | 30 boutons, impression Win32 natif, exports PDF/Word Syncfusion, délégation export via événements (ADR-21) |
| Éditeur texte riche simple | `UC_RichTextEditorSimple.vb` | ✓ | Variante allégée (7 boutons), double format RTF+TXT, contexte optionnel |
| Hub référentiels | `UC_ReferentielHome.vb` | ✓ | 9 tuiles, droits SuperUser/Admin, navigation vers chaque référentiel |
| Base commune référentiels | `UC_ReferentielBase.vb` | ✓ | Classe de base héritable : CRUD, droits, validation, hooks champ supplémentaire |
| Référentiel Domaines | `UC_Domaines.vb` | ✓ | Pile complète Query+Modèle+Gestion+UC |
| Référentiel Liens patient | `UC_LiensPatient.vb` | ✓ | Pile complète |
| Référentiel Rôles intervenant | `UC_RolesIntervenant.vb` | ✓ | Pile complète |
| Référentiel Situations familiales | `UC_SituationsFamiliales.vb` | ✓ | Pile complète |
| Référentiel Statuts dossier | `UC_StatutsDossier.vb` | ✓ | Pile complète |
| Référentiel Statuts séance | `UC_StatutsSeance.vb` | ✓ | Pile complète |
| Référentiel Types documents | `UC_TypesDocuments.vb` | ✓ | Pile complète |
| Référentiel Types rendez-vous | `UC_TypesRendezVous.vb` | ✓ | Pile complète |
| Référentiel Types séance | `UC_TypesSeance.vb` | ✓ | Pile complète + `tarif_defaut` via hook champ supplémentaire |
| Recherche patients | `UC_PatientHome.vb` | ✓ | Recherche multi-critères + liste rapide + bouton Nouveau |
| Fiche patient | `UC_PatientFiche.vb` | ✓ | Bandeau identité + onglets Identité / Anamnèse / Famille-Contacts / Intervenants / Dossiers ; modes Consultation/Création/Modification |

### 🧠 Cœur métier

| Module | Statut | Notes |
|--------|--------|-------|
| Patients (recherche, fiche, identité) | ✓ | `UC_PatientHome` + `UC_PatientFiche` : identité, alerte, famille/contacts, validations NISS, anti-doublon |
| Patients (anamnèse) | ✓ | Onglet Anamnèse avec `UC_RichTextEditor`, sauvegarde double format, export PDF/Word contextuel horodaté (ADR-21) |
| Patients (photo d'identité) | ✓ | Upload bouton, formats GDI+ autorisés, nom fixe `Identite.ext`, `AssurerDossierPatient`, MAJ `patients.photo_fichier` (ADR-20) |
| Patients (intervenants) | ✓ | Réseau N-N via `autres_suivis_patient` + `ref_roles_intervenant` — à venir |
| Dossiers (statuts, cycle de vie) | ⚠ | Réouverture si même domaine ; transfert inter-domaines actés (D-Q5, D-Q10) ; devra piloter `patients.suivi_en_cours` (déjà lu par le filtre de `UC_PatientHome`) |
| Séances (saisie, historique) | ⚠ | Créée dès planification du RDV lié, statut via `ref_statuts_seance` (D-Q6) |
| Paiements (liaison séances) | ⚠ | |
| Documents | 🧪 → périmètre acté | POC Drive/DocIO validé ; **V1 actée** : fichiers hors DB, chemin déterministe, Word local **et** Google Docs (D-Q4/D-Q7). Points d'ancrage posés dans le code (anamnèse export + upload photo). |
| Agenda | 🧪 → périmètre acté | POC Google Calendar/Scheduler validé ; **V1 actée** : Google = pilier, sync bidirectionnelle, reprise assistée (D-Q8) |

### 🗄️ Base de données

| Périmètre | Statut | Notes |
|-----------|--------|-------|
| Tables techniques (`tec_*`) | ✓ | Paramètres, méta-schéma ; `PATH_GENERAL` / `PATH_DOCUMENT` opérationnels |
| Tables sécurité (`sec_*`) | ✓ | `sec_utilisateurs` avec gestion complète des rôles, verrouillage, élévation |
| Tables référentielles (`ref_*`) | ✓ schéma + ✓ applicatif | 9 tables ref_* : schéma existant + piles applicatives complètes (Query+Modèle+Gestion+UC) |
| Tables métier patients | ✓ schéma + ✓ branché | v2.1 contacts/rôles légaux, v2.2 `photo_fichier`, v2.3 `anamnese_rtf/txt` — tous migrés et connectés |
| Tables métier (dossiers, séances, paiements, documents) | ✓ schéma / ⚠ branché | Schéma SQL existant ; modules applicatifs non connectés |
| Scripts versionnés complets | ⚠ | Technique et sécurité OK ; partie métier (dossiers/séances/paiements/documents) à compléter |
| Séquences MariaDB | ✓ | Tables métier/techniques via `LASTVAL(seq_*)` ; tables ref_* via `AUTO_INCREMENT` |

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
| D-18 | UserControl dédié par référentiel sur une base commune (`UC_ReferentielBase`) — **9 UC concrets implémentés** | ✓ Actée + ✓ Implémentée |
| D-19 | Anticipation multi-utilisateur / multi-agenda dès la V1 (modèle préparé, activation différée) | ✓ Actée |
| D-20 | Stockage fichiers patient sous `{PATH_GENERAL}\{PATH_DOCUMENT}\{code_patient}` — chemin déterministe via `CheminsPatientHelper` | ✓ Actée + ✓ Implémentée (ADR-20) |
| D-21 | Export `UC_RichTextEditor` par délégation d'événements (`ExportRequested`/`ExportCompleted`) — contrôle générique, logique métier dans le conteneur | ✓ Actée + ✓ Implémentée (ADR-21) |

> ⚠ Détail et justification de chaque décision : voir [`ARCHITECTURE_DECISIONS.md`](../Rules/ARCHITECTURE_DECISIONS.md) et le [plan de conception métier](../Conception/Plan_Conception_Metier_Althea.md).

---

## 3. Dettes techniques connues

| Réf | Description | Priorité | Impact |
|-----|-------------|----------|--------|
| DT-01 | Build `.NET` bloqué en environnement Linux (NETSDK1100, projet WinForms cible Windows) | Faible | Uniquement pour CI non-Windows ; aucun impact sur l'exécution réelle |
| DT-02 | ~~`UC_Utilisateurs` non finalisé~~ | ~~Haute~~ | ✓ **RÉSOLU** - Module complet et opérationnel |
| DT-03 | Scripts SQL métier non entièrement versionnés pour les modules dossiers/séances/paiements/documents | Moyenne | Complexifie les mises à jour de schéma futures |
| DT-04 | Scénarios d'exploitation (tests erreurs/permissions) non durcis pour modules métier | Moyenne | Risque de comportements inattendus aux limites |
| DT-05 | ~~Intégration des POC documents et agenda non planifiée~~ | ~~Moyenne~~ | ✓ **RÉSOLU** - Périmètre V1 acté (D-13, D-14) ; reste l'implémentation |
| DT-06 | Absence de campagne de tests fonctionnels transverses | Haute | Requise avant tout gel de la V1 |
| DT-07 | Documentation illustrations manquantes | Faible | Images à ajouter pour DialogChoix, UtilisateurEdition, UC_Utilisateurs |
| DT-08 | ~~Migration de schéma à réaliser (Lot 0)~~ | ~~Haute~~ | ✓ **RÉSOLU** — Schéma migré + 9 piles applicatives référentiels complètes |
| DT-09 | Coexistence possible de photos d'identité multiples (ex. `identite.jpg` + `identite.png`) dans le dossier patient | Faible | Ambiguïté sur la photo active ; nettoyage à implémenter avec la brique Documents |
| DT-10 | Onglet Intervenants de `UC_PatientFiche` non implémenté | Haute | Réseau N-N `autres_suivis_patient` acté mais absent de l'UI |

---

## 4. Prochaines étapes (ordre de priorité)

### ~~Priorité 1 - Finaliser l'administration~~ ✓ **TERMINÉ**
1. ~~Terminer `UC_Utilisateurs` (liste + détail + actions admin + contrôle d'accès + journalisation)~~ ✓
2. ~~Relire et fiabiliser le flux complet Login → Home → Admin~~ ✓
3. ~~Implémenter DialogChoix pour remplacer tous les MessageBox~~ ✓
4. ~~Centraliser les icônes d'état via UtilsIcons~~ ✓

### ~~Priorité 2 - Lot 0 : socle métier (migration de schéma)~~ ✓ **TERMINÉ**
5. ~~Versionner et appliquer la migration : `volets`→`domaines`, `medecins`→`therapeutes`~~ ✓
6. ~~Créer la liaison N-N `autres_suivis_patient` + le référentiel `ref_roles_intervenant`~~ ✓
7. ~~Créer/aligner `ref_statuts_seance` (cycle de vie séance)~~ ✓
8. ~~Créer la base UI commune `UC_ReferentielBase` + composants `UC_RichTextEditor` / `UC_RichTextEditorSimple`~~ ✓
   - ~~9 UC référentiels concrets déployés~~ ✓

### ~~Priorité 3 - Lot 1 : Patients (fiche de base)~~ ✓ **TERMINÉ** (14/06/2026)
9. ~~Créer `UC_PatientHome` (recherche + liste + bouton Nouveau)~~ ✓
10. ~~Créer `UC_PatientFiche` (identité, alerte, famille/contacts, validations)~~ ✓
11. ~~Implémenter onglet Anamnèse avec `UC_RichTextEditor` + export contextuel horodaté~~ ✓
12. ~~Implémenter upload photo d'identité (`CheminsPatientHelper`, `AssurerDossierPatient`, MAJ DB)~~ ✓
13. ~~Refactorer `CheminsPatientHelper` : `code_patient`, `PATH_GENERAL\PATH_DOCUMENT`~~ ✓
14. ~~Migrations DB v2.1/v2.2/v2.3 (contacts, photo_fichier, anamnese_rtf/txt)~~ ✓

### Priorité 4 - Lot 1 suite : Intervenants & Dossiers
15. ~~Implémenter l'onglet **Intervenants** dans `UC_PatientFiche` (réseau N-N `autres_suivis_patient` + `ref_roles_intervenant`, rôle *Adresseur* inclus)~~
16. Créer les modules **Dossiers** (métier + UI) avec statuts, transitions, réouverture et transfert inter-domaines

### Priorité 5 - Lot 2 : Suivi et paiements
17. Implémenter la gestion des **séances** (création dès planification RDV + historique + statuts)
18. Implémenter la gestion des **paiements** (liaison séances + suivi financier minimal)

### Priorité 6 - Lot 3 : Documents et agenda (intégration depuis POC)
19. Intégrer le module **documents** V1 (fichiers hors DB, chemin déterministe, Word local + Google Docs, export PDF) — brancher les points d'ancrage existants dans le code
20. Intégrer le module **agenda** V1 (Google Calendar pilier, sync bidirectionnelle, reprise assistée)

### Priorité 7 - Exploitation et stabilisation
21. Industrialiser la sauvegarde/restauration DB + fichiers
22. Préparer le package d'installation avec configuration initiale guidée
23. Exécuter la campagne de tests fonctionnels transverses
24. Effectuer les corrections UX finales et durcir les cas limites
25. Clôturer le changelog et préparer la note de version V1

---

## 5. Risques identifiés

| Risque | Probabilité | Impact | Mitigation |
|--------|-------------|--------|------------|
| Périmètre V1 qui s'étend (feature creep) | Moyenne | Élevé | Gel explicite du périmètre par module avant codage |
| Qualité du cœur métier insuffisante sans tests | Haute | Élevé | Planifier la recette fonctionnelle dès la phase 3 |
| Reprise du projet par un tiers sans documentation suffisante | Faible | Élevé | Maintenir ce fichier et les ADR à jour à chaque évolution majeure |
| Dépendance à des services externes (Google Drive, Google Calendar) dans les POC | Moyenne | Moyen | Définir une alternative locale si intégration impossible en V1 |

---

## 6. Réalisations récentes

### Lot 1 — Patients : anamnèse, photo, export contextuel ✓ (13–14/06/2026)
- **Onglet Anamnèse** dans `UC_PatientFiche` : `UC_RichTextEditor` complet, sauvegarde double format RTF+TXT, export PDF/Word contextuel horodaté + ouverture automatique (ADR-21)
- **Upload photo d'identité** : bouton `btnUploadPhoto`, filtre formats GDI+ (jpg/jpeg/png/gif/bmp), nom fixe `Identite.ext`, `AssurerDossierPatient`, MAJ `patients.photo_fichier` (ADR-20)
- **Délégation export** : `ExportRequested` / `ExportCompleted` dans `UC_RichTextEditor` — contrôle générique, logique patient dans `UC_PatientFiche` (nommage horodaté, ouverture auto)
- **`CheminsPatientHelper` refactoré** : `code_patient` formaté (PA000003), dossier sous `PATH_GENERAL\PATH_DOCUMENT\{code}`, méthodes `AssurerDossierPatient` et `GetNomFichierPhotoIdentite`
- **Migrations DB** v2.1 (contacts/rôles légaux), v2.2 (`photo_fichier`), v2.3 (`anamnese_rtf/txt`) appliquées
- **Points d'ancrage TODO** posés dans le code pour la future traçabilité `documents` (export anamnèse + upload photo)
- **Build complet validé** sans erreur
- **Documentation mise à jour** : README, CHANGELOG, ARCHITECTURE_DECISIONS (ADR-20/21), Rules (§23/24), ToDo

### Lot 1 — Patients : fiche de base ✓ (12–13/06/2026)
- **`UC_PatientHome`** : recherche multi-critères + liste rapide + bouton Nouveau
- **`UC_PatientFiche`** : bandeau identité + onglets Identité / Anamnèse / Famille-Contacts / Intervenants / Dossiers ; modes Consultation/Création/Modification + activation progressive
- **Alerte patient** RTF via `UC_RichTextEditorSimple` (bandeau)
- **Validations** : NISS, anti-doublon nom+prénom+naissance
- **Migrations DB** v2.1 contacts/famille avec rôles légaux

### Lot 0 : socle métier terminé ✓ (09–10/06/2026)
- **9 référentiels implémentés** (pile complète Query + Modèle + Gestion + UC)
- **`UC_ReferentielBase`** : classe de base héritable avec hooks champ supplémentaire
- **`UC_ReferentielHome`** : hub 9 tuiles, droits SuperUser/Admin, navigation activée
- **`UC_RichTextEditorSimple`** : variante allégée (7 boutons), double format RTF+TXT
- **Build complet validé**

### Cadrage de la phase métier ✓ (08/06/2026)
- **Plan de conception métier** rédigé et validé : `Plan_Conception_Metier_Althea.md`
- **Décisions actées** (D-13 à D-19) : documents hors DB, Google Docs/Calendar piliers, renommages, réseau N-N, séance dès planification, UC par référentiel, multi-utilisateur anticipé
- **ADR mis à jour** : ARCHITECTURE_DECISIONS.md (ADR-13 à ADR-19) et Rules.md

### Gestion utilisateurs complète ✓
- **UC_Utilisateurs** : liste avec recherche/filtres avancés, actions admin complètes
- **UtilisateurEdition** : Form modale avec 3 modes, génération MDP temporaire, reset, déverrouillage
- **GestionUtilisateurs** : couche métier complète (CRUD, activation/désactivation, gestion séquences MariaDB)

### UI/UX cohérente ✓
- **DialogChoix** : remplacement complet de MessageBox, icônes animées GIF, thématisation UITheme
- **UtilsIcons** : centralisation des icônes d'état avec priorité (verrouillé > actif > inactif)
- **UtilsButtons** : gestion cohérente des styles boutons dans toute l'application





------

> **Contact** : ***Joëlle (Manou) - Les Artefacts de Manou***
>
> Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.
>
> - Site web P.Nguyen Duy: https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu`
> - GitHub privé: Althea https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea

------



[TOC]

