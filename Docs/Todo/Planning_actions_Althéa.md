# 🗓️ Althéa — Planning d'actions V1 (roadmap consolidée)

> Planning macro par lots, synchronisé avec :
> - `Plan_Conception_Metier_Althea.md` (plan directeur de la phase métier, source de vérité)
> - `Checklist_projet_V1.md` (référence détaillée)
> - `ToDo.md` (actions opérationnelles)
>
> Dernière mise à jour : 10 juin 2026
>
> ℹ️ **Rythme de travail** : développement solo, **sans cadence imposée**. La progression se mesure « fait / pas fait » par lot, chaque lot étant clôturé par sa documentation. Aucune estimation en semaines ni jalon daté pour les travaux à venir.

---

## Légende

- ✅ Réalisé
- 🟡 En cours
- 🔜 À lancer

---

## Phase 0 — Socle technique et sécurité

**Statut : ✅ Réalisé (complet)**

- Architecture du projet structurée
- Démarrage applicatif orchestré
- Configuration locale + sécurité secrets
- Connexion DB centralisée
- Logging centralisé
- Authentification/rôles/session/élévation

**Date de clôture : 17/05/2026**

---

## Phase 1 — Coque UI et administration

**Statut : ✅ Réalisé (complet)**

- `Home`, navigation centralisée, contexte UI : ✅
- `UC_Accueil`, `UC_AdminHome`, `UC_Parametres` : ✅
- `UC_Utilisateurs` + `UtilisateurEdition` : ✅
- `DialogChoix` personnalisé : ✅
- `UtilsIcons` centralisé : ✅
- Documentation exhaustive : ✅

**Objectif de sortie de phase :**
- Administration utilisable de bout en bout (paramètres + utilisateurs) ✅ **ATTEINT**

**Date de clôture : 07/06/2026**

**Livrables :**
- Module administration complet et opérationnel
- UI cohérente avec DialogChoix et icônes centralisées
- Documentation technique complète et à jour
- Plan de tests et validation effectuée

---

## Phase 2 — Lot 0 : socle métier (migration de schéma + UC transverses)

**Statut : ✅ Réalisé (complet)**

- Migration versionnée `volets` → `domaines` : ✅
- Migration versionnée `medecins` → `therapeutes` : ✅
- Liaison N-N `autres_suivis_patient` + référentiel `ref_roles_intervenant` : ✅
- Référentiel `ref_statuts_seance` (cycle de vie séance) : ✅
- Base UI commune `UC_ReferentielBase` (classe héritable + hooks champ supplémentaire) : ✅
- Hub `UC_ReferentielHome` (9 tuiles, droits, navigation) : ✅
- 9 UC référentiels concrets (Domaines, LiensPatient, RolesIntervenant, SituationsFamiliales, StatutsDossier, StatutsSeance, TypesDocuments, TypesRendezVous, TypesSeance) : ✅
- Composant `UC_RichTextEditor` (éditeur complet 30 boutons, impression, PDF/Word) : ✅
- Composant `UC_RichTextEditorSimple` (variante allégée 7 boutons, double format) : ✅

**Objectif de sortie de phase :** ✅ **ATTEINT**
- Schéma et briques UI transverses alignés avec le plan de conception, prêts pour le codage métier

**Date de clôture : 10/06/2026**

**Livrables :**
- 9 piles applicatives référentiels complètes (Query / Modèle / Gestion / UC)
- Architecture générique extensible (`UC_ReferentielBase` + hooks)
- Cas spécial `UC_TypesSeance` avec `tarif_defaut` géré par hook sans modifier la base
- Deux éditeurs de texte riche réutilisables (`UC_RichTextEditor` + `UC_RichTextEditorSimple`)
- Build complet validé sans erreur

---

## Phase 3 — Lot 1 : cœur métier patients/dossiers

**Statut : 🔜 À lancer**

**Périmètre :**
- Module métier `GestionPatients` + `QueryPatients`
- Recherche patients (filtres avancés)
- Fiche patient multi-onglets (identité, administratif, famille, suivi, documents, facturation, archive)
- Réseau d'intervenants externes (N-N via `autres_suivis_patient` + `ref_roles_intervenant`)
- Module métier `GestionDossiers` + `QueryDossiers`
- Gestion dossiers + statuts (actif/pause/clôturé/archivé)
- Transitions de statuts sécurisées, réouverture (même domaine) et transfert inter-domaines
- À brancher : les transitions de statut devront mettre à jour `patients.suivi_en_cours` (1 = au moins un dossier en cours, 0 = tous clôturés/archivés), colonne déjà exploitée par le filtre de `UC_PatientHome`

**Objectif de sortie de phase :**
- Parcours patient opérationnel minimal en production interne
- Gestion de base des dossiers avec cycle de vie complet

---

## Phase 4 — Lot 2 : suivi clinique et paiements

**Statut : 🔜 À lancer**

**Périmètre :**
- Module métier `GestionSeances` + `QuerySeances`
- Séance créée dès la planification du rendez-vous lié, statut piloté par `ref_statuts_seance`
- Gestion des séances (saisie, notes, synthèse)
- Historique de suivi avec tri/filtrage
- Module métier `GestionPaiements` + `QueryPaiements`
- Gestion paiements liée aux séances
- Suivi financier (solde patient, paiements en attente)
- Exports de base (liste paiements, balance)

**Objectif de sortie de phase :**
- Parcours de suivi complet du patient (hors documents/agenda avancés)
- Gestion financière opérationnelle

---

## Phase 5 — Lot 3 : intégration documents et agenda (depuis POC)

**Statut : 🔜 À lancer**

**Périmètre documents :**
- Module métier `GestionDocuments`
- Stockage **hors base de données** (système de fichiers), chemin déterministe calculé
- Flux Word local **et** Google Docs, synchronisation à la sauvegarde, export PDF
- Catégorisation par type (rapport, synthèse, courrier)
- Liaison documents ↔ patients/dossiers
- Recherche documents
- Envoi par email + historique

**Périmètre agenda :**
- Module métier `GestionAgenda`
- Google Calendar comme **pilier** (synchronisation bidirectionnelle, reprise assistée)
- Gestion rendez-vous de base (date, heure, patient, type)
- Vue calendrier mensuelle/hebdomadaire (Scheduler)
- Création rapide depuis fiche patient
- Notifications/rappels
- Liaison rendez-vous ↔ séances
- Modèle préparé pour le multi-utilisateur / multi-agenda (activation différée)

**Objectif de sortie de phase :**
- Couverture fonctionnelle globale V1 avec modules historiques prioritaires
- Intégration POC dans application principale

---

## Phase 6 — Exploitation, installation, stabilisation

**Statut : 🔜 À lancer**

**Périmètre :**
- Sauvegarde/restauration complète (DB + fichiers)
  - Sauvegarde automatique quotidienne
  - Sauvegarde manuelle à la demande
  - Localisation sécurisée
  - Tests de restauration
- Installation/configuration initiale
  - Assistant de configuration DB
  - Création utilisateur admin initial
  - Import/export paramètres
- Campagne de tests fonctionnels
  - Tests parcours complets (patients, dossiers, séances, paiements, documents, agenda)
  - Tests sécurité et permissions
  - Tests limites et erreurs
- Corrections UX finales
- Finalisation documentation utilisateur

**Objectif de sortie de phase :**
- Version V1 stabilisée, installable, maintenable
- Application prête pour déploiement production

---

## Jalons de pilotage

| Jalon | Description | Statut |
|-------|-------------|--------|
| **J0** | Socle technique complet | ✅ Atteint (17/05/2026) |
| **J1** | Administration complète | ✅ Atteint (07/06/2026) |
| **J2** | Cadrage de la phase métier (décisions actées) | ✅ Atteint (08/06/2026) |
| **J3** | Lot 0 : socle métier migré (schéma + UC transverses + référentiels) | ✅ Atteint (10/06/2026) |
| **J4** | Lot 1 : patient/dossier minimal viable | 🔜 À lancer |
| **J5** | Lot 2 : suivi métier complet (séances + paiements) | 🔜 À lancer |
| **J6** | Lot 3 : couverture transverse (documents + agenda) | 🔜 À lancer |
| **J7** | Go V1 interne | 🔜 À lancer |

---

## Indicateurs d'avancement

### Modules implémentés : 6/9 (67%)

- ✅ Socle technique (démarrage, config, DB, logs, sécurité)
- ✅ Navigation et shell UI
- ✅ Administration (paramètres, utilisateurs)
- ✅ UI cohérente (DialogChoix, UtilsIcons)
- ✅ Documentation exhaustive
- ✅ Lot 0 : socle métier (migration schéma + UC référentiels + éditeurs de texte)
- 🔜 Patients + Dossiers
- 🔜 Séances + Paiements
- 🔜 Documents + Agenda

### Documentation : 100%

- ✅ README, Rules, Process, Database, Standards
- ✅ Architecture Decisions (19 ADR)
- ✅ Plan de conception métier (décisions D-13 à D-19 actées)
- ✅ Documentation UI complète
- ✅ CHANGELOG détaillé
- ✅ Plans de pilotage (Checklist, ToDo, Planning, État du projet)

### Tests : 20%

- ✅ Tests administration (UC_Utilisateurs validé)
- 🔜 Tests modules métier (patients, dossiers, séances, paiements)
- 🔜 Tests documents + agenda
- 🔜 Tests transverses et stabilisation

---

## Notes de suivi

- **Phase 1 achevée avec succès** : Le module d'administration est complet, documenté et validé.
- **Cadrage métier acté (08/06/2026)** : décisions D-13 à D-19 figées dans le [plan de conception](../Conception/Plan_Conception_Metier_Althea.md), les ADR et Rules.
- **Lot 0 achevé (10/06/2026)** : schéma migré, 9 référentiels déployés (piles complètes), `UC_ReferentielBase` avec hooks extensibles, `UC_RichTextEditorSimple` ajouté. Build validé.
- **Priorité suivante** : Lot 1 (patients + dossiers), le socle métier étant désormais prêt.
- **Rythme** : travail solo sans cadence ; chaque lot est clôturé par sa documentation avant de passer au suivant.
- **Contrainte connue** : Build/test .NET bloqués sur Linux pour ce projet WinForms cible Windows (à prendre en compte pour CI et validation).
- **Risque feature creep** : Gel du périmètre V1 recommandé avant de démarrer chaque lot métier.

---

## Réalisations Phase 2 – Lot 0 (09-10/06/2026)

### Référentiels complets ✅
- `UC_ReferentielBase` : base générique héritable avec hooks champ supplémentaire
- `UC_ReferentielHome` : hub 9 tuiles, droits, navigation
- 9 UC référentiels concrets déployés avec pile complète (Query/Modèle/Gestion/UC)
- Cas spécial `UC_TypesSeance` : `tarif_defaut` géré par hook sans modifier la base commune
- Build validé sans erreur

### Éditeur de texte simple ✅
- `UC_RichTextEditorSimple` : variante allégée (7 boutons), double format RTF+TXT, contexte optionnel

---

## Réalisations Phase 1 (23/05-07/06/2026)

### Gestion utilisateurs complète ✅
- UC_Utilisateurs : liste, recherche/filtres, actions admin
- UtilisateurEdition : 3 modes (création/modification/consultation)
- GestionUtilisateurs : CRUD complet, reset password, déverrouillage
- Gestion rôles Admin/SuperUser

### UI/UX cohérente ✅
- DialogChoix : remplacement complet MessageBox (34 occurrences)
- UtilsIcons : centralisation icônes d'état avec priorités
- Support icônes animées GIF

### Documentation exhaustive ✅
- Mise à jour complète de tous les documents de référence
- Nouvelle documentation UI (DialogChoix, UtilisateurEdition, UC_Utilisateurs)
- Plan de tests créé et validé

### Tests et validation ✅
- Tests manuels complets effectués
- Bugs identifiés et corrigés
- Workflows validés







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

