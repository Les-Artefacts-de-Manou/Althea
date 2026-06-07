# 📋 STRATÉGIE DE TESTS - PROJET ALTHÉA

**Projet** : Althéa - Gestion cabinet libéral  
**Version** : V1.0  
**Date** : 07/06/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

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
| **Login & Authentification** | `PLAN_TESTS_LOGIN_AUTHENTIFICATION.md` | ✅ Créé | HAUTE | 07/06/2026 |
| **UC_Parametres** | `PLAN_TESTS_UC_PARAMETRES.md` | ✅ Créé | HAUTE | 07/06/2026 |
| **UC_Utilisateurs** | `PLAN_TESTS_UC_UTILISATEURS.md` | ✅ Complété avec bugs identifiés | HAUTE | 07/06/2026 |
| **DialogChoix** | `PLAN_TESTS_DIALOGCHOIX.md` | ✅ Créé | MOYENNE | 07/06/2026 |
| **Home & Navigation** | `PLAN_TESTS_HOME_NAVIGATION.md` | ✅ Créé | HAUTE | 07/06/2026 |

### 🔜 Modules à couvrir (futur)

| Module | Plan de tests | État | Priorité | Échéance |
|--------|---------------|------|----------|----------|
| **Patients** | `PLAN_TESTS_PATIENTS.md` | ⏳ À créer | HAUTE | Phase 2 |
| **Dossiers** | `PLAN_TESTS_DOSSIERS.md` | ⏳ À créer | HAUTE | Phase 3 |
| **Séances** | `PLAN_TESTS_SEANCES.md` | ⏳ À créer | HAUTE | Phase 3 |
| **Paiements** | `PLAN_TESTS_PAIEMENTS.md` | ⏳ À créer | HAUTE | Phase 4 |
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

- ⏳ Créer `PLAN_TESTS_PATIENTS.md`
- ⏳ Couvrir CRUD patients
- ⏳ Recherche, filtres, archivage

### Phase 3 : Dossiers & Séances

- ⏳ Créer `PLAN_TESTS_DOSSIERS.md`
- ⏳ Créer `PLAN_TESTS_SEANCES.md`
- ⏳ Tests intégrés Patient → Dossier → Séance

### Phase 4 : Paiements & Documents

- ⏳ Créer `PLAN_TESTS_PAIEMENTS.md`
- ⏳ Créer `PLAN_TESTS_DOCUMENTS.md`
- ⏳ Tests de facturation, exports

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

**Dernière mise à jour** : 07/06/2026  
**Prochaine révision** : À la fin de Phase 2 (Patients)
