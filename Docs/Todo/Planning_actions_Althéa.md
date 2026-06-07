# 🗓️ Althéa — Planning d'actions V1 (roadmap consolidée)

> Planning macro par phases, synchronisé avec :
> - `Checklist_projet_V1.md` (référence détaillée)
> - `ToDo.md` (actions opérationnelles)
> 
> Dernière mise à jour : 07 juin 2026

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

## Phase 2 — Cœur métier patients/dossiers

**Statut : 🔜 À lancer**

**Périmètre :**
- Module métier `GestionPatients` + `QueryPatients`
- Recherche patients (filtres avancés)
- Fiche patient multi-onglets (identité, administratif, famille, suivi, documents, facturation, archive)
- Module métier `GestionDossiers` + `QueryDossiers`
- Gestion dossiers + statuts (actif/pause/clôturé/archivé)
- Transitions de statuts sécurisées

**Objectif de sortie de phase :**
- Parcours patient opérationnel minimal en production interne
- Gestion de base des dossiers avec cycle de vie

**Estimation : 3-4 semaines**

---

## Phase 3 — Suivi clinique et paiements

**Statut : 🔜 À lancer**

**Périmètre :**
- Module métier `GestionSeances` + `QuerySeances`
- Gestion des séances (saisie, notes, synthèse)
- Historique de suivi avec tri/filtrage
- Module métier `GestionPaiements` + `QueryPaiements`
- Gestion paiements liée aux séances
- Suivi financier (solde patient, paiements en attente)
- Exports de base (liste paiements, balance)

**Objectif de sortie de phase :**
- Parcours de suivi complet du patient (hors documents/agenda avancés)
- Gestion financière opérationnelle

**Estimation : 2-3 semaines**

---

## Phase 4 — Intégration documents et agenda (depuis POC)

**Statut : 🔜 À lancer**

**Périmètre documents :**
- Module métier `GestionDocuments`
- Upload/stockage local de documents (PDF, images, Word)
- Catégorisation par type (rapport, synthèse, courrier)
- Liaison documents ↔ patients/dossiers
- Recherche documents
- Envoi par email + historique

**Périmètre agenda :**
- Module métier `GestionAgenda`
- Gestion rendez-vous de base (date, heure, patient, type)
- Vue calendrier mensuelle/hebdomadaire
- Création rapide depuis fiche patient
- Notifications/rappels
- Liaison rendez-vous ↔ séances

**Objectif de sortie de phase :**
- Couverture fonctionnelle globale V1 avec modules historiques prioritaires
- Intégration POC dans application principale

**Estimation : 3-4 semaines**

---

## Phase 5 — Exploitation, installation, stabilisation

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

**Estimation : 2-3 semaines**

---

## Jalons de pilotage

| Jalon | Description | Statut | Date |
|-------|-------------|--------|------|
| **J0** | Socle technique complet | ✅ Atteint | 17/05/2026 |
| **J1** | Administration complète | ✅ Atteint | 07/06/2026 |
| **J2** | Patient minimal viable | 🔜 À lancer | T3 2026 |
| **J3** | Suivi métier complet | 🔜 À lancer | T3 2026 |
| **J4** | Couverture transverse | 🔜 À lancer | T4 2026 |
| **J5** | Go V1 interne | 🔜 À lancer | T4 2026 |

---

## Indicateurs d'avancement

### Modules implémentés : 5/9 (56%)

- ✅ Socle technique (démarrage, config, DB, logs, sécurité)
- ✅ Navigation et shell UI
- ✅ Administration (paramètres, utilisateurs)
- ✅ UI cohérente (DialogChoix, UtilsIcons)
- ✅ Documentation exhaustive
- 🔜 Patients
- 🔜 Dossiers
- 🔜 Séances + Paiements
- 🔜 Documents + Agenda

### Documentation : 100%

- ✅ README, Rules, Process, Database, Standards
- ✅ Architecture Decisions (14 ADR)
- ✅ Documentation UI complète
- ✅ CHANGELOG détaillé
- ✅ Plans de pilotage (Checklist, ToDo, Planning)

### Tests : 20%

- ✅ Tests administration (UC_Utilisateurs validé)
- 🔜 Tests modules métier (patients, dossiers, séances, paiements)
- 🔜 Tests documents + agenda
- 🔜 Tests transverses et stabilisation

---

## Notes de suivi

- **Phase 1 achevée avec succès** : Le module d'administration est complet, documenté et validé.
- **Priorité suivante** : Lancement de la Phase 2 (patients/dossiers) dès que possible.
- **Contrainte connue** : Build/test .NET bloqués sur Linux pour ce projet WinForms cible Windows (à prendre en compte pour CI et validation).
- **Risque feature creep** : Gel du périmètre V1 recommandé avant de démarrer chaque phase métier.

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
