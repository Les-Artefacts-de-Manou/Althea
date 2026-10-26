# 📋 STRATÉGIE DE TESTS - PROJET ALTHÉA

**Projet** : Althéa - Gestion cabinet libéral  
**Version** : V1.1  
**Date** : 26/10/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

> *Dernière mise à jour : 26/10/2026 — intégration continue sur GitHub (audit D-3, ADR-59)*

---

## 🎯 Objectif de ce document

Définir la stratégie globale de tests pour le projet Althéa, afin de garantir :
- La **qualité** et la **robustesse** du code
- La **traçabilité** des tests effectués
- La **non-régression** lors des évolutions futures
- La **maintenabilité** à long terme

> **Philosophie** : "Ne plus tester au petit bonheur la chance", mais avec une approche structurée, reproductible et documentée.

---

## 📚 Plans de tests existants

### ✅ Modules déjà couverts

| Module | Plan de tests | État | Priorité | Dernière mise à jour |
|--------|---------------|------|----------|----------------------|
| **Login & Authentification** | `PLAN_TESTS_LOGIN_AUTHENTIFICATION.md` | ✅ Créé — V1.2 | HAUTE | 20/10/2026 |
| **UC_Parametres** | `PLAN_TESTS_UC_PARAMETRES.md` | ✅ Créé — V2.1 (écran sur une page le 16/10/2026, carte Licences) | HAUTE | 18/10/2026 |
| **UC_Utilisateurs** | `PLAN_TESTS_UC_UTILISATEURS.md` | ✅ Créé — V1.7 (droits côté métier, règle du mot de passe et du login) | HAUTE | 23/10/2026 |
| **UC_Sauvegardes** / **DialogRestauration** | `PLAN_TESTS_SAUVEGARDES.md` | ✅ Sauvegarde validée (P2) ; ✅ restauration validée (P3, section 7) (sauf 7.4 / 7.5 à revérifier ; 7.15 reporté) — V1.4 | HAUTE | 21/10/2026 |
| **UC_Journaux** / **UC_EtatSysteme** | `PLAN_TESTS_ADMINISTRATION.md` | ✅ Validé — V1.2 (cohérence des montants en attente) | MOYENNE | 22/10/2026 |
| **DialogChoix** | `PLAN_TESTS_DIALOGCHOIX.md` | ✅ V2.1 — démonstration validée | MOYENNE | 16/10/2026 |
| **Contrôles du kit** (`AltheaDate`, `AltheaCalendrier`…) | `PLAN_TESTS_KIT_ALTHEA.md` | ✅ Créé — V1.6, démonstration validée | MOYENNE | 20/10/2026 |
| **Home & Navigation** | `PLAN_TESTS_HOME_NAVIGATION.md` | ✅ Créé — V1.1 (menu de gauche, bouton Recherche, Retour) | HAUTE | 23/10/2026 |
| **Patients** | `PLAN_TESTS_PATIENTS.md` | ✅ V1.13 — PAT-113 à PAT-132 validés le 24/10/2026 (codes postaux belges, patients inactifs) | HAUTE | 24/10/2026 |
| **Dossiers** | `PLAN_TESTS_DOSSIERS.md` | ✅ Créé — V1.7 | HAUTE | 21/10/2026 |
| **Séances** | `PLAN_TESTS_SEANCES.md` | ✅ Créé — V1.13 | HAUTE | 22/10/2026 |
| **Tarification & quotas** | `PLAN_TESTS_TARIFICATION_QUOTAS.md` | ✅ Créé — V3.2 | HAUTE | 22/10/2026 |
| **Gestion des domaines** | `PLAN_TESTS_DOMAINES.md` | ✅ Créé — V2.6 | HAUTE | 23/10/2026 |
| **Paiements** | `PLAN_TESTS_PAIEMENTS.md` | ✅ Créé — V1.17 | HAUTE | 22/10/2026 |
| **Recherche transversale** (`UC_RechercheHome`, `UC_RechercheSeances`, `UC_RecherchePaiements`, `UC_RecherchePatientsLieu`, `UC_RechercheReseau`) | `PLAN_TESTS_RECHERCHE.md` | ✅ V1.5 : RCH-01 à RCH-71 validés les 23 et 24/10/2026 (R1 à R5, le code toujours visible) | HAUTE | 24/10/2026 |
| **Parcours de suivi transversal** | `PLAN_TESTS_PARCOURS_SUIVI.md` | ✅ V1.2 — C3-01 à C3-06 (non-régression des codes système, audit C.3) validés le 24/10/2026 | HAUTE | 24/10/2026 |
| **Cabinet & praticien (UC_Cabinet)** | `PLAN_TESTS_UC_CABINET.md` | ✅ V1.6 — 4.15 à 4.18 (codes postaux belges) validés le 24/10/2026 | HAUTE | 24/10/2026 |
| **Référentiels** | `PLAN_TESTS_REFERENTIELS.md` | ✅ Créé — V1.5 (droits et lignes système vérifiés par le métier, spécialités des thérapeutes) | HAUTE | 23/10/2026 |
| **Éditeur riche** (`UC_RichTextEditor`, textes types, correcteur) | `PLAN_TESTS_UC_RICHTEXTEDITOR.md` | ✅ Créé — V2.6 | MOYENNE | 20/10/2026 |
| **Tarif patient par défaut** (archive) | `PLAN_TEST_TARIF_PATIENT.md` | 🗄️ Archive — remplacé par `PLAN_TESTS_TARIFICATION_QUOTAS.md` | — | — |

### 🔜 Modules à couvrir (futur)

| Module | Plan de tests | État | Priorité | Échéance |
|--------|---------------|------|----------|----------|
| **Documents** | `PLAN_TESTS_DOCUMENTS.md` | ⏳ À créer | MOYENNE | Phase 4 |
| **Agenda** | `PLAN_TESTS_AGENDA.md` | ⏳ À créer | MOYENNE | Phase 5 |

---

## 🏗️ Structure d'un plan de tests type

Chaque plan de tests Althéa suit une structure standardisée :

### 1. En-tête

- **Module** concerné
- **Version** du plan
- **Date** de création/mise à jour
- **Auteur**
- **Objectif** général des tests

### 2. Checklist générale (prérequis)

Liste des conditions nécessaires avant de commencer les tests :
- Base de données opérationnelle
- Utilisateurs de test créés
- Logs activés
- Application compilée sans erreur

### 3. Sections fonctionnelles

Chaque section couvre un aspect du module :
- **Section 1** : Chargement & affichage
- **Section 2** : Création
- **Section 3** : Modification
- **Section 4** : Actions spécifiques (activation, désactivation, etc.)
- **Section 5+** : Tests avancés (robustesse, sécurité, performance, UI/UX)

### 4. Format des tests

Chaque test est documenté dans un tableau structuré :

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| X.Y.Z | Description du test | Ce qui doit se passer | - Point de contrôle 1<br>- Point de contrôle 2 | État |

**États possibles** :
- `☐` : Non testé
- `OK` : Test réussi
- `⚠️` : Test réussi avec observations/améliorations
- `❌` : Test échoué (bug identifié)

### 5. Récapitulatif & bugs

- **Récapitulatif** : tableau de synthèse (tests OK/KO/non testés)
- **Bugs identifiés** : tableau des bugs avec priorité, statut, section
- **Validation finale** : checklist pour signer le plan

---

## 🔄 Processus de test

### Quand tester ?

1. **Après chaque implémentation majeure**
   - Créer ou mettre à jour le plan de tests
   - Exécuter tous les tests de la section concernée

2. **Avant chaque commit important**
   - Exécuter les tests de non-régression
   - Vérifier les logs

3. **Avant chaque release**
   - Exécuter TOUS les plans de tests
   - Exécuter le parcours transversal de suivi après les plans Domaines, Dossiers, Séances, Tarification & quotas et Paiements
   - Exécuter le plan Recherche après les plans Séances et Paiements : il réutilise leurs filtres, leur résumé et le relevé des paiements (`FiltresSeances`, `FiltresPaiements`)
   - Corriger tous les bugs HIGH/MEDIUM
   - Documenter les bugs LOW restants

### Comment tester ?

1. **Préparation**
   - Lire le plan de tests complet
   - Préparer l'environnement de test (base de données, utilisateurs)
   - Ouvrir le fichier de log

2. **Exécution**
   - Suivre l'ordre des sections
   - Cocher chaque test au fur et à mesure
   - Noter les observations directement dans le plan

3. **Documentation**
   - Remplir le tableau "Bugs identifiés"
   - Mettre à jour le récapitulatif
   - Ajouter des notes/remarques si nécessaire

4. **Suivi**
   - Créer des issues pour les bugs HIGH/MEDIUM
   - Corriger les bugs
   - Retester après correction

---

## 🔍 Types de tests couverts

### Tests fonctionnels

- **Chargement** : affichage initial, état des contrôles
- **CRUD** : création, lecture, modification, suppression/désactivation
- **Actions métier** : activation, déverrouillage, réinitialisation, etc.
- **Validation** : contrôles de saisie, messages d'erreur
- **Navigation** : transitions entre écrans, retours, annulations

### Tests de sécurité

- **Authentification** : connexion, déconnexion, verrouillage
- **Autorisation** : vérification des droits (Admin, SuperUser, User)
- **Traçabilité** : logs créés pour chaque action sensible
- **Protection données** : aucun mot de passe dans les logs

### Tests de robustesse

- **Erreurs DB** : connexion coupée, timeout, contraintes
- **Erreurs UI** : saisies invalides, clics multiples, états incohérents
- **Performance** : chargement de grandes listes, mémoire
- **Concurrence** : modifications simultanées (optionnel)

### Tests UI/UX

- **Charte graphique** : couleurs UITheme, police Calibri, icônes
- **Cohérence** : messages clairs, tooltips, état boutons
- **Navigation clavier** : Tab, Entrée, Échap
- **Responsive** : redimensionnement, scrollbars
- **Accessibilité** : lecteurs d'écran (optionnel)

---

## 🤖 Tests automatisés (xUnit, ADR-58)

Les plans ci-dessus se jouent à la main, dans l'application. Les tests automatisés les complètent : de petits programmes qui vérifient le code sans ouvrir l'application, relancés en quelques secondes après chaque modification.

### Où et comment

- **Projet** : `Tests/Althea.Tests` (VB.NET, xUnit v3), dans la solution `Althea.slnx` (dossier « Tests ») ; exclu de la compilation de l'application.
- **Lancer** :
  - `Tools\test.cmd` : compile, lance tout, écrit `obj\_tests.log` (lisible par Claude) ;
  - ou Visual Studio, *Test › Explorateur de tests* (coche verte / croix rouge par test).
- **Rangement** : une classe `<Module>Tests` par module testé, dans le dossier qui reprend le sien (`Utils/`, `Core/`, `Metier/`, `UI/`).
- **Fonction privée à tester** : la passer en `Friend` (visible du seul projet de tests, `InternalsVisibleTo` dans `Althea.vbproj`), jamais en `Public`.
- **Calcul qui lit la base en cours de route** : lui fournir ce qu'il lit par un paramètre (ex. la règle de quota par une fonction « date → règle ») pour le tester sans base.
- **Noms** : `Methode_Cas_Attendu` (ex. `Valider_NissBelgeAvecBonneCle_Accepte`).
- **Données** : fictives uniquement. Un résultat attendu qui est un calcul (clé de contrôle, empreinte) est calculé hors du code testé, pour ne pas vérifier le code par lui-même.

### Ce qui est couvert

| Classe de tests | Ce qu'elle vérifie |
| --- | --- |
| `UtilsNissTests` | NISS belge (dont naissances 2000+), numéro français (avec ou sans clé, Corse), matricule luxembourgeois ; forme enregistrée, affichage, type déduit |
| `UtilsIdentifiantBelgeTests` | INAMI, BCE, TVA, IBAN, BIC, code titulaire ; expression du référentiel pour les autres types |
| `PolitiqueMotDePasseTests` | Longueur (10 / 12 pour les archives), majuscule, minuscule, chiffre |
| `PasswordSecurityHelperTests` | Sel aléatoire, empreinte PBKDF2 **figée** (un changement de paramètres bloquerait tous les comptes), vérification |
| `UtilsTelephoneTests` | Pays reconnu par son nom, forme internationale, affichage, nombre de chiffres |
| `UtilsCodePostalTests` | Code postal par pays ; rapprochement des localités (« St.-Truiden » = « Sint-Truiden ») |
| `UtilsTexteTests` | Comparaison sans accents ni majuscules, découpage en mots |
| `AltheaDateTests` | Saisie d'une date : formats acceptés, année sur deux chiffres (« 85 » → 1985), 29 février, dates impossibles |
| `AltheaHeureTests` | Saisie d'une heure (« 9 », « 930 », « 9h30 »…), heures impossibles, fin après minuit ramenée à 23:59, « : » automatique |
| `GestionQuotaInamiTests` | Calcul du quota : numérotation par catégorie, séances non comptées, renouvellement de date à date, période décrite, règle fixée à l'ouverture, absence de règle, plafonds |
| `RequetesFumeeTests` | **Test de fumée** : chaque requête des modules `Query*` (389) préparée par MariaDB, jamais exécutée, sur la base vide `althea_tests` ; découpage des requêtes et recensement vérifiés sans base |

621 tests au 26/10/2026, tous au vert (environ 3 secondes) ; sans base de test configurée : 233 tests, le test de fumée apparaît une fois comme « ignoré ».

### Le test de fumée et sa base

- **Base dédiée et vide** `althea_tests` (jamais de données, même fictives), avec un compte MariaDB limité à cette base : `Docs/Database/Maintenance/creer_base_tests.sql` (une fois par poste, mode d'emploi en tête).
- **Chaîne de connexion** dans la variable d'environnement Windows `ALTHEA_TESTS_CONNEXION` de l'utilisateur, jamais dans le dépôt. Absente : le test est ignoré.
- **À chaque série**, la structure est recréée depuis `Docs/Database/backup_NoData_althea.sql` : la sauvegarde de structure est donc vérifiée en même temps (elle doit se recharger dans une base neuve).
- **Garde-fou** : toute autre base que `althea_tests` est refusée (la préparation y supprime les tables).
- **Une requête refusée** : le test affiche le message de MariaDB et la requête. Une requête « modèle » dont un repère est remplacé à l'exécution (ex. `{IDS}`) se déclare dans `CatalogueRequetes` (une par une).

### À venir (audit D-2)

### Intégration continue (GitHub, ADR-59)

- `.github/workflows/ci.yml` : une machine Windows de GitHub récupère `master`, compile l'application et les tests en **Release**, lance les tests (le test de fumée y est ignoré : pas de base).
- **Quand** : chaque soir (21:30 à Bruxelles en hiver, 22:30 en été) s'il y a eu un commit depuis 24 h ; à tout moment par le bouton « Run workflow » de l'onglet **Actions**.
- **Lire le résultat** : onglet Actions du dépôt — coche verte, ou croix rouge : cliquer, ouvrir l'étape en rouge, copier ses dernières lignes.
- Ce qu'elle vérifie de plus que `Tools\test.cmd` : le dépôt en ligne se compile depuis zéro (rien d'oublié dans un commit), en configuration Release.

### Quand ajouter un test

- Après une migration ou une modification de requête : régénérer la sauvegarde de structure, puis `Tools\test.cmd` (le test de fumée vérifie toutes les requêtes contre la nouvelle structure).

- Une fonction de calcul ou de validation sans écran ni base, nouvelle ou modifiée.
- Un défaut corrigé dans une telle fonction : un test qui l'aurait signalé (non-régression).

---

## 📊 Métriques de qualité

### Couverture des tests

| Métrique | Cible | Actuel (07/06/2026) |
|----------|-------|---------------------|
| **Modules couverts** | 100% modules développés | 5/5 modules existants (100%) |
| **Tests exécutés** | 80% minimum | ~85% (UC_Utilisateurs testé réellement) |
| **Bugs HIGH corrigés** | 100% avant release | 3 bugs HIGH identifiés, 0 corrigés |
| **Bugs MEDIUM corrigés** | 90% avant release | 1 bug MEDIUM identifié, 0 corrigé |

### Qualité du code

| Métrique | Cible | Actuel |
|----------|-------|--------|
| **Logs sans mot de passe** | 100% | ✅ Vérifié |
| **Gestion erreurs** | Try/Catch sur toutes méthodes DB | ✅ OK |
| **MessageBox remplacés** | 100% par DialogChoix | ✅ OK |
| **Compilation sans warning** | 100% | ✅ OK |

---

## 🛠️ Outils et environnement

### Base de données de test

- **Serveur** : MariaDB local ou distant
- **Base** : `althea_test` (copie de `althea` avec données anonymisées)
- **Utilisateurs** : 1 Admin, 1 SuperUser, 1 User actifs + 1 inactif + 1 verrouillé

### Logs

- **Niveau** : Succinct minimum (Rapide pour sécurité)
- **Localisation** : `C:\Logs\Althea\` ou répertoire configuré
- **Vérification** : Ouvrir après chaque session de tests

### Documentation

- **Plans de tests** : `Docs\Tests\`
- **Bugs** : Notés dans les plans + issues GitHub (futur)
- **Rapport de tests** : Généré avant chaque release (optionnel)

---

## 🎓 Bonnes pratiques

### Pour le testeur

1. **Tester dans l'ordre** : Suivre les sections du plan
2. **Ne pas sauter de tests** : Même si ça semble évident
3. **Noter immédiatement** : Cocher, commenter, documenter en direct
4. **Reproduire les bugs** : Tenter 2-3 fois pour confirmer
5. **Vérifier les logs** : Après chaque action importante
6. **Ne pas corriger en testant** : Noter, puis corriger après

### Pour le développeur

1. **Créer le plan avant ou pendant le dev** : Pas après
2. **Exécuter les tests soi-même** : Avant de demander validation
3. **Corriger les bugs HIGH en priorité** : Avant de continuer
4. **Mettre à jour le plan** : Si comportement modifié
5. **Ajouter des tests** : Pour chaque bug corrigé (non-régression)
6. **Documenter les limitations** : Si fonctionnalité incomplète

---

## 🚀 Évolution de la stratégie

### Phase 1 (Actuelle) : Administration & Infrastructure

- ✅ Login/Authentification
- ✅ Gestion utilisateurs
- ✅ Paramètres
- ✅ DialogChoix
- ✅ Home/Navigation

### Phase 2 : Patients

- ✅ `PLAN_TESTS_PATIENTS.md` créé
- ✅ CRUD, identité, activation, contacts, intervenants, alerte, photo et anamnèse couverts
- ✅ Recherche, filtres et accès aux dossiers couverts

### Phase 3 : Dossiers & Séances

- ✅ `PLAN_TESTS_DOSSIERS.md` créé
- ✅ `PLAN_TESTS_SEANCES.md` créé
- ✅ Parcours intégré couvert dans `PLAN_TESTS_PARCOURS_SUIVI.md`

### Phase 4 : Paiements & Documents

- ✅ `PLAN_TESTS_PAIEMENTS.md` créé
- ⏳ Créer `PLAN_TESTS_DOCUMENTS.md`
- ⏳ Tests de facturation, exports

### Tests automatisés (audit D-2, ADR-58)

- ✅ Étape 1 (26/10/2026) : projet `Tests/Althea.Tests`, `Tools\test.cmd`, 154 tests (identifiants, sécurité, saisie)
- ✅ Étape 2 (26/10/2026) : dates, heures, quota INAMI — 229 tests
- ✅ Étape 3 (26/10/2026) : test de fumée des requêtes (base `althea_tests`) — 621 tests
- ✅ D-3 (26/10/2026) : intégration continue (GitHub Actions, une fois par jour et à la demande)

### Phase 5 : Agenda & Finalisation

- ⏳ Créer `PLAN_TESTS_AGENDA.md`
- ⏳ Tests de planification, rappels
- ⏳ Tests de performance globaux
- ⏳ Tests d'acceptation utilisateur (UAT)

---

## 📞 Contact & Support

Pour toute question sur les tests ou les plans :

- **Joëlle (Manou)** - Les Artefacts de Manou
- **GitHub** : https://github.com/AngeljoNG/Althea
- **Documentation** : `Docs\Tests\`

---

> **"La qualité ne s'améliore pas par accident, elle s'améliore par intention."**  
> *— Philip Crosby*

---

**Dernière mise à jour** : 23/10/2026 (tableau des plans à jour, plan Recherche)  
**Prochaine révision** : à la création du plan Documents ou Agenda
