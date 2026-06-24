# 📌 Althéa — ToDo opérationnel (backlog V1)

> Backlog actionnable, aligné sur l'état réel du dépôt.
> Plan directeur de la phase métier : `../Conception/Plan_Conception_Metier_Althea.md` (décisions actées D-13 à D-19).
> Référence d'avancement détaillée : `Checklist_projet_V1.md`.
> >  *Dernière mise à jour : 14/06/2026*

---

## Légende

- [x] Terminé
- [~] En cours / partiel
- [ ] À faire

---

## A. ~~Priorité immédiate (stabilisation fonctionnelle)~~ ✅ **TERMINÉ**

- [x] Finaliser module **Utilisateurs**
  - [x] Implémenter `UC_Utilisateurs` (liste + détail + actions admin)
  - [x] Implémenter `UtilisateurEdition` (création/modification/consultation)
  - [x] Appliquer contrôle d'accès complet selon rôles (Admin/SuperUser)
  - [x] Journaliser actions sensibles (création, activation, réinitialisation, déverrouillage)
  - [x] Gérer mots de passe temporaires sécurisés
  - [x] Implémenter recherche et filtres avancés
  - [x] Afficher icônes d'état (actif/inactif/verrouillé) avec priorités

- [x] Fiabiliser parcours administration
  - [x] Relecture complète des flux Login → Home → Admin
  - [x] Vérifier cohérence messages statut / erreurs UI
  - [x] Vérifier garde-fous élévation de droits
  - [x] Remplacer tous les MessageBox par DialogChoix
  - [x] Centraliser icônes d'état via UtilsIcons

- [x] Documentation administration complète
  - [x] Mise à jour CHANGELOG.md (chronologie inversée)
  - [x] Mise à jour README.md (structure professionnelle)
  - [x] Mise à jour Rules.md (règles DialogChoix, UtilsIcons, modes utilisateur)
  - [x] Mise à jour Process_Althea.md (Processus 06 et 07 + diagrammes Mermaid)
  - [x] Mise à jour ARCHITECTURE_DECISIONS.md (ADR-10, ADR-11, ADR-12)
  - [x] Mise à jour Documentation_technique_UI_Althea.md (DialogChoix, UtilisateurEdition, UC_Utilisateurs)

---

## B. ~~Lot 0 — Socle métier (migration de schéma)~~ ✅ **TERMINÉ**

> Réalisé et versionné. Prérequis bloquant des lots suivants : levé.
> Référence : décisions D-16 à D-18 du plan de conception.

### B0a. Migration de schéma ✅

- [x] Versionner et appliquer le renommage `volets` → `domaines`
- [x] Versionner et appliquer le renommage `medecins` → `therapeutes`
- [x] Créer la table de liaison N-N `autres_suivis_patient` (patient ↔ thérapeute externe)
- [x] Créer le référentiel `ref_roles_intervenant`
- [x] Créer/aligner le référentiel `ref_statuts_seance` (cycle de vie séance)
- [x] Mettre à jour les scripts SQL versionnés et la documentation `Database`

### B0b. Briques UI transverses ✅

- [x] Créer la base commune `UC_ReferentielBase` (UserControl générique par référentiel) avec hooks champ supplémentaire
- [x] Créer `UC_ReferentielHome` (hub 9 tuiles, droits SuperUser/Admin, navigation)
- [x] Décliner 9 UC référentiels concrets :
  - [x] `UC_Domaines` (ref_domaines, code max 10)
  - [x] `UC_LiensPatient` (ref_liens_patient, code max 50)
  - [x] `UC_RolesIntervenant` (ref_roles_intervenant, code max 30)
  - [x] `UC_SituationsFamiliales` (ref_situations_familiales, code max 50)
  - [x] `UC_StatutsDossier` (ref_statuts_dossier, code max 30)
  - [x] `UC_StatutsSeance` (ref_statuts_seance, code max 30)
  - [x] `UC_TypesDocuments` (ref_types_documents, code max 30)
  - [x] `UC_TypesRendezVous` (ref_types_rendez_vous, code max 30)
  - [x] `UC_TypesSeance` (ref_types_seance, code max 30, **+ tarif_defaut via hook**)
- [x] Créer le composant riche `UC_RichTextEditor` (30 boutons, impression Win32, exports PDF/Word Syncfusion)
- [x] Créer la variante compacte `UC_RichTextEditorSimple` (7 boutons, double format RTF+TXT, contexte optionnel)

---

## C. Cœur métier V1 (objectif principal restant)

### C0. Briques transverses (pré-requis cœur métier)

- [ ] Ajouter une **mini-pile de navigation** (Push/Pop + conservation du dernier filtre) dans `NavigationManager` (D-Q15)
- [ ] Créer un **hôte modal générique de référentiel** réutilisant les `UC_Ref<X>` existants, déclenché par bouton `[+]` près des combos, avec contrôle de droit + élévation (D-Q17)
- [x] **Helper de chemins déterministes** `CheminsPatientHelper` (refactoré : `code_patient` PA000003, `PATH_GENERAL\PATH_DOCUMENT\{code}`, `AssurerDossierPatient`, `GetNomFichierPhotoIdentite`) (D-Q13 / ADR-20)
- [x] **Migrations DB préalables** : `patients.alerte_rtf/alerte_txt` ✅, `patients.photo_fichier` ✅, `patients.anamnese_rtf/anamnese_txt` ✅ (v2.3), `tec_parametres PATH_GENERAL/PATH_DOCUMENT` ✅

### C1. Patients

- [x] Créer module métier gestion patients (`GestionPatients.vb`) + `UpdatePhotoPatient`
- [x] Créer requêtes SQL patients (`QueryPatients.vb`) — incl. `anamnese_rtf/txt`
- [x] Créer UI `UC_PatientHome` (recherche multi-critères + liste rapide + bouton Nouveau)
- [x] Créer UI fiche patient `UC_PatientFiche` — bandeau identité + onglets **Identité / Anamnèse / Famille-Contacts / Intervenants / Dossiers** ; modes Consultation/Création/Modification + activation progressive (D-Q11, D-Q14)
- [x] Implémenter l'alerte patient RTF (`alerte_rtf/txt` via `UC_RichTextEditorSimple`, bandeau) (D-Q12)
- [x] **Onglet Anamnèse** : `UC_RichTextEditor` complet, sauvegarde double format, export PDF/Word contextuel horodaté + ouverture auto (ADR-21)
- [x] **Upload photo d'identité** : `btnUploadPhoto`, filtre formats GDI+, nom fixe `Identite.ext`, `AssurerDossierPatient`, MAJ `patients.photo_fichier` (D-Q13 / ADR-20)
- [x] Implémenter validations et règles de saisie (NISS, anti-doublon nom+prénom+naissance)
- [x] Implémenter le réseau d'intervenants externes (N-N via `autres_suivis_patient` + `ref_roles_intervenant`, rôle *Adresseur* inclus) (D-Q16) — **onglet à venir**
- [ ] Implémenter recherche rapide depuis accueil

### C1bis. Thérapeutes (référentiel riche — hub Référentiels)

> Brique **Gestion des thérapeutes** livrée (modèle `Therapeute` + `QueryTherapeutes` + `GestionTherapeutes` + `UC_Therapeutes` + `TherapeuteEdition` + tuile hub). Source du futur combo des Intervenants.
> Points d'ancrage `TODO` posés dans le code : `Therapeute.vb`, `TherapeuteEdition.vb`, `QueryTherapeutes.vb`.

- [x] Brique Gestion des thérapeutes (liste + modale, soft-delete, contrôle d'usage, tuile hub Référentiels)
- [x] **Commentaire thérapeute en RichTextEditorSimple** : transformer le commentaire (TextBox) en `UC_RichTextEditorSimple`. Implique d'ajouter un champ `.rtf` (`commentaire_rtf`, + texte brut pour la recherche) dans la table `therapeutes` et d'adapter les queries de gestion (`QueryTherapeutes` INSERT/UPDATE/SELECT) ainsi que `TherapeuteEdition`.
- [ ] **Spécialité en table (idée à creuser — décision à prendre)** : mettre la spécialité en référentiel pour unifier la saisie (éviter les variantes). Si oui : créer/migrer la table côté base, remplacer le champ texte `specialite` par une FK, et adapter `TherapeuteEdition` (combo type-ahead + bouton `[+]`).

### C2. Dossiers

- [ ] Créer module métier gestion dossiers (`GestionDossiers.vb`)
- [ ] Créer requêtes SQL dossiers (`QueryDossiers.vb`)
- [ ] Créer UI gestion dossiers liés au patient
- [ ] Implémenter statuts dossier (actif/pause/clôturé/archivé)
- [ ] Gérer transitions de statut sécurisées avec validation
- [ ] Gérer réouverture après clôture (même domaine) et transfert inter-domaines
- [ ] Implémenter zone notes via `UC_RichTextEditor` + historique lisible
- [ ] Implémenter recherche et filtrage par statut/date/domaine

### C3. Séances

- [ ] Créer module métier gestion séances (`GestionSeances.vb`)
- [ ] Créer requêtes SQL séances (`QuerySeances.vb`)
- [ ] Créer UI gestion séances
- [ ] Créer la séance dès la planification du rendez-vous lié (statut via `ref_statuts_seance`)
- [ ] Ajouter historique lisible + tri/filtrage
- [ ] Implémenter zone notes/synthèse par séance (`UC_RichTextEditorSimple`)
- [ ] Lier séances aux dossiers
- [ ] Implémenter saisie rapide de séance

### C4. Paiements

- [ ] Créer module métier gestion paiements (`GestionPaiements.vb`)
- [ ] Créer requêtes SQL paiements (`QueryPaiements.vb`)
- [ ] Créer UI gestion paiements
- [ ] Lier paiements ↔ séances
- [ ] Implémenter suivi financier (solde patient, paiements en attente)
- [ ] Prévoir exports de suivi financier V1 (minimum : liste paiements, balance)
- [ ] Gérer modes de paiement (espèces, virement, chèque, etc.)

---

## D. Documents et agenda (à intégrer depuis POC)

### D1. Documents

> **Règle fondamentale (à implémenter avec la table `documents`)** : tout document **créé OU uploadé** dans
> l'application doit avoir une **trace dans `documents`**, couplée à un **modèle de document** et à un **niveau**.
>
> **Niveau de document** (décision à trancher au démarrage de la brique) : `patient` (ex. anamnèse, photo
> d'identité), `dossier`, `seance`, etc. Deux options envisagées :
> - **Enum publique** dans le code (simple, typé), ou
> - **Type de paramètre dans `tec_parametres`** (plus cohérent avec le reste de l'appli, valeurs configurables).
>
> **Points d'ancrage déjà posés dans le code** (commentaires `TODO`, à compléter à ce moment) :
> - `UC_PatientFiche.rteAnamnese_ExportCompleted` — trace de l'export anamnèse (PDF/Word), niveau = patient.
> - `UC_PatientFiche.btnUploadPhoto_Click` — trace de la photo d'identité, niveau = patient.
>
> ⚠️ **Ne pas oublier de revenir sur ces deux points** lors de la construction de la brique Documents.

- [ ] **Ajouter le champ `niveau` à la table `documents`** (selon décision Enum vs paramètre ci-dessus)
- [ ] **Brancher la traçabilité aux points d'ancrage existants** (export anamnèse + upload photo d'identité)
- [ ] Implémenter le stockage **hors base de données** (système de fichiers, chemin déterministe calculé)
  - [ ] Flux Word local + synchronisation Google Docs à la sauvegarde
  - [ ] Export PDF
  - [ ] Catégorisation par type (rapport, synthèse, courrier, etc.)
  - [ ] Liaison documents ↔ patients/dossiers (chemin reconstituable)
- [ ] Créer module métier gestion documents (`GestionDocuments.vb`)
- [ ] Intégrer création/liaison documents dans code principal
- [ ] Implémenter recherche documents par patient/dossier/type
- [ ] Prévoir envoi documents par email (ex. synthèse de séance)
- [ ] Historique des envois en lien avec le patient/dossier

#### Points différés (notés en cours de développement, à reprendre avec la table `documents`)

- [ ] **Indicateur « dernier export » dans l'onglet Anamnèse** : afficher la date du dernier export documentaire (PDF/Word) + bouton d'ouverture directe. À alimenter par la table `documents` (et non par un scan du dossier patient), pour rester valable pour tous les types de documents. Distinguer clairement « dernier export » (fichier disque, potentiellement périmé) de la « sauvegarde » du contenu (`patients.anamnese_rtf/txt`).
- [ ] **Nettoyage des photos d'identité multiples** : à l'upload d'une photo d'extension différente (ex. `identite.png` sur `identite.jpg` existant), les deux fichiers coexistent dans `…/Documents/{code_patient}/`. Supprimer les anciennes `identite.*` lors d'un nouvel upload pour lever toute ambiguïté.


### D2. Agenda

- [ ] Intégrer Google Calendar comme **pilier** (synchronisation bidirectionnelle, reprise assistée)
  - [ ] Gestion rendez-vous de base (date, heure, patient, type)
  - [ ] Vue calendrier mensuelle/hebdomadaire (Scheduler)
  - [ ] Création rapide de rendez-vous depuis fiche patient
- [ ] Créer module métier gestion agenda (`GestionAgenda.vb`)
- [ ] Implémenter notifications/rappels rendez-vous
- [ ] Lier rendez-vous ↔ séances réalisées (séance créée dès planification)
- [ ] Préparer le modèle multi-utilisateur / multi-agenda (activation différée)

---

## E. Technique & exploitation

- [ ] Finaliser scripts SQL versionnés pour modules métier restants
  - [ ] Scripts création/migration tables patients
  - [ ] Scripts création/migration tables dossiers
  - [ ] Scripts création/migration tables séances
  - [ ] Scripts création/migration tables paiements
  - [ ] Scripts création/migration tables documents (métadonnées ; fichiers stockés hors DB)
  - [ ] Scripts création/migration tables agenda
- [ ] Mettre en place procédure de sauvegarde DB + fichiers
  - [ ] Sauvegarde automatique quotidienne
  - [ ] Sauvegarde manuelle à la demande
  - [ ] Localisation sécurisée des sauvegardes
- [ ] Tester procédure de restauration complète
  - [ ] Restauration base de données
  - [ ] Restauration fichiers locaux
  - [ ] Validation intégrité après restauration
- [ ] Préparer installation guidée (configuration initiale)
  - [ ] Assistant de configuration DB
  - [ ] Création utilisateur admin initial
  - [ ] Import/export paramètres

---

## F. Qualité & documentation

- [x] Base documentaire technique existante (README/Rules/Process/Database)
- [x] Consolidation des documents de pilotage (`Checklist`, `ToDo`, `Planning`)
- [x] Documentation UI complète (`Documentation_technique_UI_Althea.md`)
- [x] Plan de conception métier rédigé et décisions actées (D-13 à D-19)
- [x] Ajouter illustrations manquantes dans `Documentation_technique_UI_Althea.md`
  - [x] DialogChoix (Information, Warning, Error, Success, Question)
  - [x] UtilisateurEdition (Création, Modification, Consultation)
  - [x] UC_Utilisateurs (vue Admin, vue SuperUser)
- [] Construire checklist de tests fonctionnels V1 par parcours utilisateur
  - [ ] Tests parcours patients (création, recherche, modification)
  - [ ] Tests parcours dossiers (création, transitions statuts)
  - [ ] Tests parcours séances (saisie, historique)
  - [ ] Tests parcours paiements (enregistrement, suivi)
  - [ ] Tests parcours documents (upload, liaison, envoi)
  - [ ] Tests parcours agenda (création RV, notifications)
- [ ] Exécuter recette finale de stabilisation V1
- [ ] Mettre à jour changelog et note de version V1

---

## G. Vue "fait / en cours / reste"

### ✅ Déjà fait

- [x] Socle technique (startup, config, DB manager, logs)
- [x] Sécurité de base (auth, rôles, session, changement MDP, élévation)
- [x] Navigation et shell UI principal
- [x] Module paramètres opérationnel
- [x] **Module utilisateurs complet** (liste, création, modification, consultation, actions admin)
- [x] **DialogChoix personnalisé** (remplacement complet MessageBox)
- [x] **UtilsIcons centralisé** (icônes d'état avec priorités)
- [x] **Documentation complète administration** (tous les documents de référence à jour)
- [x] POC documents et agenda réalisés
- [x] **Lot 0 : socle métier complet**
  - [x] Migration schéma `domaines`/`therapeutes`/N-N
  - [x] 9 UC référentiels déployés (piles complètes Query/Modèle/Gestion/UC)
  - [x] `UC_ReferentielBase` avec hooks extensibles
  - [x] `UC_RichTextEditor` (complet) + `UC_RichTextEditorSimple` (allégé)

### 🔜 Reste à faire

- [ ] Domaine métier principal (patients, dossiers, séances, paiements)
- [ ] Intégration POC documents/agenda dans l'application principale (stockage hors DB, Google Calendar pilier)
- [ ] Sauvegarde/restauration/installation
- [ ] Stabilisation et validation fonctionnelle V1

---

## H. Idées pour la suite (post-V1)

**1. Sur l'accueil, on pourrait trouver des genres de Widgets** 
- RV du jour
- Nouveau RV à ajouter (Quick)
- Recherche patient (Quick)
- Dernier patient/dossier traité
- Todo (il faudra en introduire la notion dans l'appli)
- Lien avec le site Web de Pearl (et peut-être d'autres liens dont elle peut avoir besoin souvent)
- Notifications / alertes importantes
- On pourrait aussi introduire la notion de "favoris" pour les écrans les plus utilisés, avec un accès rapide depuis l'accueil ou une barre latérale.
- Dashboard avec statistiques (nombre de patients actifs, séances du mois, chiffre d'affaires, etc.)

**2. Lien avec le site Web**
Il existe un site web pour Pearl : https://pearlnguyenduy.be/

- Dans ce site, Pearl publie des articles. Il faudrait un système de publication simple et rapide à partir de l'appli.
- J'aimerais aussi rapatrier le site : en effet celui-ci a été construit avec Bolt, et reste dépendant entièrement de Bolt, Netlify, Stackblitz, Supabase
- L'url a été acheté sur Combell et est pointée vers Netlify.
- Je voudrais rendre ce site plus simple à changer, ajouter des pages, des photos etc… et en lien avec l'appli Althea que l'on développe ici. (cela fera partie de la version 2)
- Intégration d'un CMS simple dans Althéa pour gérer le contenu du site web

**3. IA**
- Je voudrais intégrer une IA dans l'appli, pour aider à la rédaction de documents, de notes, de synthèses, etc…
- Il faudrait aussi que cette IA puisse être utilisée pour faire des analyses de données, des prévisions, etc… à partir des données de l'appli (patients, dossiers, séances, paiements, etc…)
- Suggestions automatiques de diagnostics ou d'observations basées sur les notes de séances
- Génération automatique de synthèses de suivi patient
- Analyse de tendances (évolution du nombre de patients, types de consultations, etc.)

**4. Améliorations UX/UI**
- Mode sombre / clair
- Personnalisation des thèmes
- Raccourcis clavier avancés
- Multi-écrans / multi-fenêtres pour productivité
- Barre de recherche globale (patients, dossiers, documents, paramètres)

**5. Fonctionnalités métier avancées**
- Génération automatique de factures
- Gestion des mutuelles et remboursements
- Statistiques avancées et rapports personnalisés
- Export données pour comptabilité
- Gestion de la TVA
- Archivage automatique des dossiers anciens
- Alertes sur rendez-vous non confirmés
- Suivi des impayés

**6. Sécurité et conformité**
- Audit trail complet (qui a fait quoi, quand)
- Anonymisation/pseudonymisation des données pour RGPD
- Export/suppression données patient (droit à l'oubli)
- Chiffrement renforcé des données sensibles
- Authentification à deux facteurs (2FA)
- Sauvegarde cloud sécurisée (optionnelle)

**7. Mobilité**
- Application mobile compagnon (consultation agenda, fiche patient simplifiée)
- Synchronisation locale/cloud
- Mode hors-ligne avec sync automatique

---

**Note importante** : Ces idées seront évaluées et priorisées après la livraison de la V1 stable.



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

