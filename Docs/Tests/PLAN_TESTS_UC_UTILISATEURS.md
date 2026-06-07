# 📋 PLAN DE TESTS COMPLET - UC_UTILISATEURS

**Module** : Gestion des utilisateurs applicatifs  
**Version** : V1.1  
**Date** : 07/06/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

> **Note** : Ce plan a été complété et corrigé suite aux tests réels effectués par Joëlle.  
> Les observations et bugs identifiés sont documentés directement dans les tests.

---

## 🎯 Objectif des tests

Valider le bon fonctionnement complet du module `UC_Utilisateurs` et `UtilisateurEdition`, incluant :
- Affichage et navigation dans la liste des utilisateurs
- Création et modification d'utilisateurs
- Actions administratives (activation/désactivation, déverrouillage, réinitialisation mot de passe)
- Gestion des droits et des erreurs
- Cohérence UI (boutons, contexte, messages)
- Traçabilité via les logs

---

## ✅ CHECKLIST GÉNÉRALE

### Prérequis

- [x] Base de données MariaDB opérationnelle
- [x] Table `sec_utilisateurs` créée et accessible
- [x] Au moins 1 utilisateur Admin existant pour se connecter
- [x] Logs activés et fichier de log accessible
- [x] Application compilée sans erreur

---

## 📂 SECTION 1 : CHARGEMENT & AFFICHAGE (UC_Utilisateurs)

### 1.1 - Chargement initial du UserControl

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.1.1 | Ouvrir `UC_Utilisateurs` depuis `UC_AdminHome` | Le UC se charge sans erreur | - Header = "Administration > Utilisateurs"<br>- Status = "[N] utilisateur(s) chargé(s)."<br>- DataGridView rempli avec utilisateurs actifs<br>- Curseur = Default après chargement | OK |
| 1.1.2 | Vérifier l'état initial des boutons | Boutons correctement activés/désactivés | - `btnNouveau` : Enabled<br>- `btnModifier` : Disabled (aucune sélection)<br>- `btnActiverDesactiver` : Disabled<br>- `btnActualiser` : Enabled | OK |
| 1.1.3 | Vérifier les tooltips | Tous les tooltips sont présents et corrects | - Survoler chaque bouton et la checkbox<br>- Vérifier texte cohérent | OK |

### 1.2 - Affichage des utilisateurs

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.2.1 | Vérifier l'affichage des colonnes | Toutes les colonnes sont visibles | - Code utilisateur<br>- Login<br>- Nom affichage<br>- Rôle<br>- Élévation max<br>- Actif<br>- Doit changer PW<br>- Nb échecs<br>- Verrouillé<br>- Date verrouillage<br>- Dernier login<br>- Date création<br>- Date modification | toutes les col ne sont pas visibles : seules 5 col sont Visibles,: Etat, Login, Nom affiché, Rôle et Dernier Login. C voulu. les autres sont cachées OK |
| 1.2.2 | Vérifier le format des dates | Format `dd/MM/yyyy HH:mm` | - Colonne "Dernier login" | OK |
| 1.2.3 | Vérifier les colonnes visibles par défaut | Seules 5 colonnes visibles | - État, Login, Nom affiché, Rôle, Dernier Login<br>- **Note** : autres colonnes cachées par design | OK |
| 1.2.4 | Vérifier que les utilisateurs inactifs ne s'affichent pas par défaut | Seuls les utilisateurs actifs sont affichés | - Checkbox "Afficher inactifs" décochée<br>- Liste ne contient que des utilisateurs actifs | OK |

### 1.3 - Filtrage des utilisateurs

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.3.1 | Cocher "Afficher les utilisateurs inactifs" | La liste se recharge et inclut les inactifs | - Liste contient des utilisateurs avec `Actif = False`<br>- Status mis à jour<br>- Pas d'erreur | OK |
| 1.3.2 | Décocher "Afficher les utilisateurs inactifs" | La liste se recharge sans les inactifs | - Liste ne contient que des actifs<br>- Status mis à jour | OK |
| 1.3.3 | Saisir du texte dans le champ de recherche | Liste filtrée en temps réel | - Filtre sur Login ou Nom affiché<br>- Status affiche "[N] affichés sur [Total]" | OK |
| 1.3.4 | Cliquer sur `btnReinitialiserFiltres` | Tous filtres réinitialisés | - Champ recherche vidé<br>- Date Login réinitialisée<br>- Liste complète rechargée | OK |

### 1.4 - Sélection et navigation

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.4.1 | Cliquer sur une ligne | La ligne est sélectionnée | - `btnModifier` et `btnActiverDesactiver` deviennent Enabled | OK |
| 1.4.2 | Changer de ligne sélectionnée | La nouvelle ligne est active | - Boutons restent Enabled | OK |
| 1.4.3 | Double-cliquer sur une ligne | La form `UtilisateurEdition` s'ouvre en mode modification | - Form modale ouverte<br>- Données utilisateur chargées | OK |

### 1.5 - Actualisation

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.5.1 | Cliquer sur `btnActualiser` | La liste se recharge | - Curseur = WaitCursor pendant le chargement<br>- Curseur = Default après<br>- Status mis à jour | OK |

---

## 📂 SECTION 2 : CRÉATION D'UTILISATEUR (UtilisateurEdition - Mode Création)

### 2.1 - Ouverture de la form en mode création

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.1.1 | Cliquer sur `btnNouveau` dans `UC_Utilisateurs` | La form `UtilisateurEdition` s'ouvre en mode création | - Titre form = "Nouvel utilisateur"<br>- Header = "Administration > Utilisateurs > Création"<br>- Status = "Création d'un nouvel utilisateur."<br>- Tous les champs vides sauf valeurs par défaut | OK |
| 2.1.2 | Vérifier l'état des contrôles | Contrôles correctement configurés | - `txtLoginUtilisateur` : Éditable<br>- `txtCodeUtilisateur` : Lecture seule, vide<br>- `txtNomAffichage` : Éditable<br>- `cboRoleUtilisateur` : Actif<br>- `cboRoleMaxElevation` : Actif<br>- `chkActif` : Coché par défaut<br>- `chkMustChangePassword` : Coché<br>- `chkCompteVerrouille` : Décoché et désactivé | OK |
| 2.1.3 | Vérifier l'état des boutons admin | Boutons admin désactivés | - `btnResetPassword` : Disabled<br>- `btnDeverrouiller` : Disabled | OK |

### 2.2 - Validation du formulaire (cas d'erreur)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.2.1 | Cliquer sur `Enregistrer` sans remplir les champs | Message d'erreur de validation | - MessageBox "Validation"<br>- Form reste ouverte | OK |
| 2.2.2 | Saisir un login vide | Message d'erreur | - MessageBox "Le login est obligatoire." | OK |
| 2.2.3 | Saisir un nom d'affichage vide | Message d'erreur | - MessageBox "Le nom d'affichage est obligatoire." | OK   |
| 2.2.4 | Saisir un login déjà existant | Échec de création + log | - MessageBox "Erreur lors de la création..."<br>- Log : "login déjà existant" (Security/Succinct) | OK |
| 2.2.5 | Sélectionner un rôle max élévation < rôle utilisateur | Échec de création + log | - Log : "rôle max élévation inférieur au rôle de base" | OK |

### 2.3 - Création réussie

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.3.1 | Remplir tous les champs correctement et cliquer sur `Enregistrer` | Utilisateur créé avec succès | - MessageBox succès avec mot de passe temporaire affiché<br>- Mot de passe copié dans presse-papiers<br>- Message indique que l'utilisateur devra changer le PW<br>- Form se ferme (DialogResult.OK)<br>- Liste `UC_Utilisateurs` rechargée<br>- Nouvel utilisateur visible | ⚠️ **BUG** : Message erreur après création réussie. Form ne se ferme pas. Liste non rafraîchie. **Priorité HAUTE**  <br /><br />OK |
| 2.3.2 | Vérifier le log de création | Log créé avec bon niveau | - Log : "Utilisateur créé avec succès (ID=..., login=..., rôle=...)"<br>- Catégorie : Security<br>- Niveau : Rapide<br>- **AUCUN mot de passe dans le log** | OK |
| 2.3.3 | Vérifier en base de données | Utilisateur créé correctement | - `login_utilisateur` = saisi<br>- `nom_affichage` = saisi<br>- `role_utilisateur` = sélectionné<br>- `actif` = 1<br>- `must_change_password` = 1<br>- `password_hash` et `password_salt` présents<br>- `date_creation` = maintenant | OK |
| 2.3.3 | Vérifier en base de données | Utilisateur créé correctement | - `login_utilisateur` = saisi<br>- `nom_affichage` = saisi<br>- `role_utilisateur` = sélectionné<br>- `actif` = 1<br>- `must_change_password` = 1<br>- `password_hash` et `password_salt` présents<br>- `date_creation` = maintenant | OK |

### 2.4 - Annulation

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.4.1 | Cliquer sur `Annuler` | Form se ferme sans créer d'utilisateur | - DialogResult.Cancel<br>- Liste UC_Utilisateurs non rechargée<br>- Aucun log de création | OK |

---

## 📂 SECTION 3 : MODIFICATION D'UTILISATEUR (UtilisateurEdition - Mode Modification)

### 3.1 - Ouverture de la form en mode modification

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.1.1 | Sélectionner un utilisateur et cliquer sur `btnModifier` | Form s'ouvre en mode modification | - Titre form = "Modification utilisateur"<br>- Header = "Administration > Utilisateurs > Modification"<br>- Status = "Modification utilisateur ID [X]."<br>- Tous les champs pré-remplis | OK |
| 3.1.2 | Vérifier l'état des contrôles | Contrôles correctement configurés | - `txtLoginUtilisateur` : Lecture seule<br>- `txtCodeUtilisateur` : Lecture seule<br>- `txtNomAffichage` : Éditable<br>- `cboRoleUtilisateur` : Actif<br>- `cboRoleMaxElevation` : Actif<br>- `chkActif` : Éditable<br>- `chkMustChangePassword` : Éditable<br>- `chkCompteVerrouille` : Lecture seule | OK |
| 3.1.3 | Vérifier l'état des boutons admin | Boutons admin selon état du compte | - `btnResetPassword` : Enabled<br>- `btnDeverrouiller` : Enabled si compte verrouillé, sinon Disabled | OK |

### 3.2 - Modification réussie

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.2.1 | Modifier le nom d'affichage et cliquer sur `Enregistrer` | Modification réussie | - MessageBox succès<br>- Form se ferme<br>- Liste rechargée<br>- Modification visible dans la liste | ❌ **BUG** : Erreur contrainte `chk_sec_utilisateurs_role` – rôles non passés correctement. **Priorité HAUTE**<br /><br />OK |
| 3.2.2 | Vérifier le log de modification | Log créé avec bon niveau | - Log : "Utilisateur modifié avec succès (ID=..., nom=..., rôle=..., actif=...)"<br>- Catégorie : Security<br>- Niveau : Rapide | OK |
| 3.2.3 | Vérifier en base de données | Modification enregistrée | - `nom_affichage` mis à jour<br>- `date_modification` = maintenant | OK |

> **Observation test 3.2.1** : Logs indiquent :  
> `2026-06-01 14:26:23 [Succinct] [Security] Erreur UpdateUtilisateur. | EX: CONSTRAINT 'chk_sec_utilisateurs_role' failed for 'althea'.'sec_utilisateurs'`  
> `2026-06-01 14:26:23 [Succinct] [Security] Erreur btnEnregistrer_Click. | EX: CONSTRAINT 'chk_sec_utilisateurs_role' failed for 'althea'.'sec_utilisateurs'`  
> **Cause probable** : Les rôles sont passés comme entiers au lieu de chaînes.



### 3.3 - Modification avec erreur de droits

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.3.1 | Tenter de modifier un utilisateur avec un compte non-Admin (simulation) | Exception levée et capturée | - MessageBox "Accès refusé"<br>- Form reste ouverte<br>- Aucune modification en base<br /><br /> | ☐ |

---

## 📂 SECTION 4 : ACTIVATION / DÉSACTIVATION (UC_Utilisateurs)

### 4.1 - Désactivation d'un utilisateur

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.1.1 | Sélectionner un utilisateur actif et cliquer sur `btnActiverDesactiver` | Demande de confirmation | - MessageBox "Voulez-vous vraiment désactiver..." | OK |
| 4.1.2 | Confirmer l'action | Utilisateur désactivé | - MessageBox succès<br>- Liste rechargée<br>- Utilisateur disparaît de la liste (si "Afficher inactifs" non coché)<br>- Status = "Utilisateur désactivé." | OK |
| 4.1.3 | Vérifier le log | Log créé | - Log : "Utilisateur ID=... désactivé par [admin]"<br>- Catégorie : Security<br>- Niveau : Rapide | OK |
| 4.1.4 | Vérifier en base de données | Champ actif mis à jour | - `actif` = 0 | OK   |

### 4.2 - Activation d'un utilisateur

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.2.1 | Cocher "Afficher inactifs", sélectionner un utilisateur inactif et cliquer sur `btnActiverDesactiver` | Demande de confirmation | - MessageBox "Voulez-vous vraiment activer..." | ⚠️ **AMÉLIORATION** : Bouton affiche toujours "Désactiver" même si action = Activer. Changer texte dynamiquement. |
| 4.2.2 | Confirmer l'action | Utilisateur activé | - MessageBox succès<br>- Liste rechargée<br>- Utilisateur marqué actif dans la liste<br>- Status = "Utilisateur activé." | ⚠️ **BUG** : Colonne État (icône) non mise à jour. Utiliser icônes OK/NO (UtilsIcons). **Priorité MOYENNE**<br /><br /><br />OK |
| 4.2.3 | Vérifier le log | Log créé | - Log : "Utilisateur ID=... activé par [admin]"<br>- Catégorie : Security<br>- Niveau : Rapide | OK |

### 4.3 - Cas limite : auto-désactivation

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.3.1 | Tenter de désactiver son propre compte | Exception levée et capturée | - MessageBox "Opération invalide" : "Vous ne pouvez pas vous désactiver vous-même."<br>- Compte reste actif | OK |

---

## 📂 SECTION 5 : RÉINITIALISATION MOT DE PASSE (UtilisateurEdition)

### 5.1 - Réinitialisation via form modale

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.1.1 | Ouvrir un utilisateur en modification et cliquer sur `btnResetPassword` | Form `ChangePassword` s'ouvre en mode AdminReset | - Titre form = "Réinitialisation mot de passe (Admin)"<br>- Champ "Ancien mot de passe" caché<br>- Mot de passe temporaire pré-généré et affiché<br>- Bouton "Générer nouveau" visible | ok |
| 5.1.2 | Valider la réinitialisation | Mot de passe réinitialisé | - MessageBox succès<br>- Mot de passe temporaire affiché<br>- Message indique que l'utilisateur devra changer le PW<br>- Form `ChangePassword` se ferme<br>- Form `UtilisateurEdition` recharge les données | ok |
| 5.1.3 | Vérifier le log | Log créé | - Log : "Mot de passe réinitialisé pour l'utilisateur ID=... (mot de passe personnalisé)"<br>- Catégorie : Security<br>- Niveau : Rapide<br>- **AUCUN mot de passe dans le log** | ok |
| 5.1.4 | Vérifier en base de données | Mot de passe réinitialisé | - `password_hash` et `password_salt` changés<br>- `must_change_password` = 1<br>- Si compte était verrouillé :<br>&nbsp;&nbsp;- `compte_verrouille` = 0<br>&nbsp;&nbsp;- `nb_echecs_login` = 0<br>&nbsp;&nbsp;- `date_verrouillage` = NULL | ok |

### 5.2 - Annulation de la réinitialisation

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.2.1 | Cliquer sur `btnResetPassword` puis `Annuler` dans `ChangePassword` | Aucune modification | - Form `ChangePassword` se ferme<br>- Aucun log de réinitialisation<br>- Mot de passe inchangé en base | ok |

---

## 📂 SECTION 6 : DÉVERROUILLAGE COMPTE (UtilisateurEdition)

### 6.1 - Déverrouillage d'un compte bloqué

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.1.1 | Ouvrir un utilisateur avec compte verrouillé et cliquer sur `btnDeverrouiller` | Demande de confirmation | - MessageBox "Voulez-vous vraiment déverrouiller..." | ok |
| 6.1.2 | Confirmer l'action | Compte déverrouillé | - MessageBox succès<br>- Form `UtilisateurEdition` recharge les données<br>- `chkCompteVerrouille` décoché<br>- `txtNbEchecsLogin` = 0<br>- `txtDateVerrouillage` vide | ok |
| 6.1.3 | Vérifier le log | Log créé | - Log : "Compte déverrouillé pour l'utilisateur ID=... par [admin]"<br>- Catégorie : Security<br>- Niveau : Rapide | ok |
| 6.1.4 | Vérifier en base de données | Déverrouillage effectif | - `compte_verrouille` = 0<br>- `nb_echecs_login` = 0<br>- `date_verrouillage` = NULL | ok |

### 6.2 - Cas limite : déverrouillage d'un compte non verrouillé

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.2.1 | Ouvrir un utilisateur non verrouillé | Bouton `btnDeverrouiller` désactivé | - Bouton grisé, non cliquable | ok |

---

## 📂 SECTION 7 : CHANGEMENT MOT DE PASSE UTILISATEUR (ChangePassword - Mode Normal)

### 7.1 - Changement réussi

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.1.1 | Ouvrir `ChangePassword` en mode normal (utilisateur connecté) | Form s'ouvre avec ancien/nouveau/confirmation | - Champs ancien/nouveau/confirmation visibles<br>- Bouton "Générer nouveau" caché | ⚠️ **NOTE** : ChangePassword n'apparaît que si `MustChangePassword` coché au login. **Amélioration possible** : ajouter menu "Changer mon mot de passe" dans Home pour changement volontaire.<br /><br />OK |
| 7.1.2 | Saisir ancien mot de passe correct + nouveau + confirmation identique | Changement réussi | - MessageBox succès<br>- Form se ferme<br>- Utilisateur peut se reconnecter avec nouveau PW | ☐ |
| 7.1.3 | Vérifier le log | Log créé | - Log : "Mot de passe changé avec succès (ID=...)"<br>- Catégorie : Security<br>- Niveau : Rapide<br>- **AUCUN mot de passe dans le log** | ☐ |

### 7.2 - Changement échoué

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.2.1 | Saisir un ancien mot de passe incorrect | Échec de changement | - MessageBox "Ancien mot de passe incorrect."<br>- Log : "ancien mot de passe invalide" | OK |
| 7.2.2 | Saisir nouveau et confirmation différents | Validation échoue | - MessageBox "Les mots de passe ne correspondent pas." | OK |

---

## 📂 SECTION 8 : GESTION DES DROITS & SÉCURITÉ

### 8.1 - Vérification des droits Admin

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.1.1 | Se connecter avec un compte User ou SuperUser (simulation) | Accès à `UC_Utilisateurs` refusé | - `UC_AdminHome` ne doit pas charger `UC_Utilisateurs`<br>- Ou exception levée si tentative directe | OK |
| 8.1.2 | Tenter une action admin (Create/Update/Activate/Reset/Unlock) avec un compte non-Admin (simulation code) | Exception `UnauthorizedAccessException` levée | - MessageBox "Accès refusé"<br>- Log : "Seuls les administrateurs..." | OK |

### 8.2 - Hiérarchie des rôles

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.2.1 | Tenter de créer un utilisateur Admin alors que l'utilisateur connecté est SuperUser (simulation) | Exception levée | - MessageBox "Vous ne pouvez pas attribuer un rôle supérieur au vôtre." | ⚠️ **NOTE** : SuperUser n'a actuellement pas accès à UC_Utilisateurs. **Question** : Donner accès limité (déverrouillage + reset password uniquement) ?<br /><br />OK |
| 8.2.2 | Vérifier qu'un Admin peut créer des utilisateurs de tout rôle | Création réussie | - Admin peut créer User, SuperUser, Admin | OK |

---

## 📂 SECTION 9 : ROBUSTESSE & GESTION D'ERREUR

### 9.1 - Erreurs base de données

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 9.1.1 | Couper la connexion à la base de données et tenter de charger `UC_Utilisateurs` | Erreur capturée | - MessageBox ou Status : "Erreur lors du chargement..."<br>- Log : exception SQL loguée<br>- Application ne plante pas | ☐ |
| 9.1.2 | Tenter de créer un utilisateur avec la base déconnectée | Exception capturée | - MessageBox "Une erreur inattendue..."<br>- Log : erreur loguée<br>- Form reste ouverte | ☐ |

### 9.2 - Erreurs UI

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 9.2.1 | Tenter d'activer/désactiver sans sélection | Message d'information | - Status = "Aucun utilisateur sélectionné."<br>- Pas d'exception | ⚠️ **NOTE** : La grille garde toujours une ligne sélectionnée (1ère ligne ou dernière sélection). Pas de cas "aucune sélection".<br /><br />OK |
| 9.2.2 | Double-cliquer sur une ligne invalide (ex: en-tête) | Aucune action | - `e.RowIndex < 0` → Return<br>- Pas d'exception | OK |

---

## 📂 SECTION 10 : COHÉRENCE UI & UX

### 10.1 - Contexte UI (Header, Status, ToolTips)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 10.1.1 | Naviguer dans `UC_Utilisateurs` | Header et Status corrects | - Header = "Administration > Utilisateurs"<br>- Status mis à jour après chaque action | OK |
| 10.1.2 | Ouvrir `UtilisateurEdition` en création | Header et Status corrects | - Header = "Administration > Utilisateurs > Création"<br>- Status = "Création d'un nouvel utilisateur." | OK |
| 10.1.3 | Ouvrir `UtilisateurEdition` en modification | Header et Status corrects | - Header = "Administration > Utilisateurs > Modification"<br>- Status = "Modification utilisateur ID [X]." | ok |
| 10.1.4 | Survoler les boutons | Tooltips affichés | - Tous les tooltips présents et pertinents | ok |

### 10.2 - État des boutons dynamique

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 10.2.1 | Sélectionner puis désélectionner une ligne | Boutons activés/désactivés dynamiquement | - `btnModifier` et `btnActiverDesactiver` suivent la sélection | ok |
| 10.2.2 | Ouvrir form en mode Création | Boutons admin désactivés | - `btnResetPassword` et `btnDeverrouiller` Disabled | ok |
| 10.2.3 | Ouvrir form en mode Modification avec compte verrouillé | Bouton déverrouiller actif | - `btnDeverrouiller` Enabled | ok |
| 10.2.4 | Ouvrir form en mode Modification avec compte non verrouillé | Bouton déverrouiller inactif | - `btnDeverrouiller` Disabled | ok |

### 10.3 - Messages utilisateur clairs

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 10.3.1 | Tester chaque action avec succès | MessageBox succès clair | - Texte explicite, icône Information | ok |
| 10.3.2 | Tester chaque action avec échec | MessageBox erreur clair | - Texte explicite, icône Error/Warning selon cas | ok |

---

## 📂 SECTION 11 : LOGS & TRAÇABILITÉ

### 11.1 - Logs de sécurité

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 11.1.1 | Créer un utilisateur | Log création présent | - Message : "Utilisateur créé avec succès (ID=..., login=..., rôle=...)"<br>- Catégorie : Security<br>- Niveau : Rapide<br>- **AUCUN mot de passe** | OK |
| 11.1.2 | Modifier un utilisateur | Log modification présent | - Message : "Utilisateur modifié avec succès (ID=..., nom=..., rôle=..., actif=...)"<br>- Catégorie : Security<br>- Niveau : RapideOK | OK |
| 11.1.3 | Activer/Désactiver un utilisateur | Log activation/désactivation présent | - Message : "Utilisateur ID=... activé/désactivé par [admin]"<br>- Catégorie : Security<br>- Niveau : Rapide | OK |
| 11.1.4 | Réinitialiser un mot de passe | Log reset présent | - Message : "Mot de passe réinitialisé pour l'utilisateur ID=... par [admin]" ou "mot de passe personnalisé"<br>- Catégorie : Security<br>- Niveau : Rapide<br>- **AUCUN mot de passe** | OK |
| 11.1.5 | Déverrouiller un compte | Log déverrouillage présent | - Message : "Compte déverrouillé pour l'utilisateur ID=... par [admin]"<br>- Catégorie : Security<br>- Niveau : Rapide | OK |
| 11.1.6 | Changer son mot de passe (utilisateur) | Log changement présent | - Message : "Mot de passe changé avec succès (ID=...)"<br>- Catégorie : Security<br>- Niveau : Rapide<br>- **AUCUN mot de passe** | OK |

### 11.2 - Logs d'erreur

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 11.2.1 | Provoquer une erreur inattendue | Log erreur présent | - Message : "Erreur [nom_méthode]"<br>- Catégorie : Security ou UI selon contexte<br>- Niveau : Succinct<br>- Exception complète loguée | ☐ |
| 11.2.2 | Tenter une action non autorisée | Log refus présent | - Message explicite du refus<br>- Catégorie : Security<br>- Niveau : Succinct | ☐ |

---

## 📂 SECTION 12 : TESTS DE NON-RÉGRESSION

### 12.1 - Cohérence avec le reste de l'application

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 12.1.1 | Se connecter avec un utilisateur créé via `UC_Utilisateurs` | Connexion réussie | - Login fonctionne avec le mot de passe temporaire<br>- Form `ChangePassword` s'ouvre si `must_change_password = 1` | ☐ |
| 12.1.2 | Modifier le rôle d'un utilisateur et vérifier l'élévation | Rôle correctement appliqué | - Utilisateur peut s'élever jusqu'à `role_max_elevation`<br>- Pas au-delà | ☐ |
| 12.1.3 | Désactiver un utilisateur et tenter de se connecter avec ce compte | Connexion refusée | - Message : "Compte désactivé" ou équivalent | ☐ |

---

## 📝 BUGS IDENTIFIÉS PENDANT LES TESTS

| # | Bug | Priorité | Section | Statut |
|---|-----|----------|---------|--------|
| BUG-001 | Après création utilisateur : message erreur + form ne se ferme pas + liste non rafraîchie | HAUTE | 2.3.1 | **À corriger** |
| BUG-002 | Modification utilisateur : erreur contrainte `chk_sec_utilisateurs_role` (rôles non passés correctement) | HAUTE | 3.2.1 | **À corriger** |
| BUG-003 | Colonne État (icône) non mise à jour après activation/désactivation | MOYENNE | 4.2.2 | **À améliorer** |
| AMÉLIO-001 | Bouton Activer/Désactiver affiche toujours "Désactiver" même si action = Activer | MOYENNE | 4.2.1 | **À améliorer** |
| AMÉLIO-002 | Utiliser icônes OK/NO (UtilsIcons) pour colonne État au lieu de checkbox | BASSE | 4.2.2 | **À améliorer** |
| AMÉLIO-003 | Ajouter menu "Changer mon mot de passe" dans Home pour changement volontaire | BASSE | 7.1.1 | **À considérer** |
| AMÉLIO-004 | Donner accès limité à UC_Utilisateurs pour SuperUser (déverrouillage + reset PW uniquement) | BASSE | 8.2.1 | **À considérer** |

---

## 🎯 RÉCAPITULATIF DES TESTS

| Catégorie | Total tests | Tests OK | Tests KO | Améliorations | Non testés |
|-----------|-------------|----------|----------|---------------|------------|
| Section 1 : Chargement | 11 | 9 | 0 | 2 | 0 |
| Section 2 : Création | 11 | 10 | 1 | 0 | 0 |
| Section 3 : Modification | 4 | 3 | 1 | 0 | 1 |
| Section 4 : Activation | 7 | 5 | 1 | 1 | 0 |
| Section 5 : Reset Password | 6 | 6 | 0 | 0 | 0 |
| Section 6 : Déverrouillage | 3 | 3 | 0 | 0 | 0 |
| Section 7 : ChangePassword | 5 | 2 | 0 | 1 | 3 |
| Section 8 : Sécurité | 4 | 3 | 0 | 1 | 0 |
| Section 9 : Robustesse | 4 | 2 | 0 | 1 | 2 |
| Section 10 : UI & UX | 12 | 12 | 0 | 0 | 0 |
| Section 11 : Logs | 8 | 6 | 0 | 0 | 2 |
| Section 12 : Non-régression | 3 | 0 | 0 | 0 | 3 |
| **TOTAL** | **78** | **61** | **3** | **6** | **11** |

---

## 📝 NOTES & REMARQUES

### Points d'attention particuliers

- **Mot de passe temporaire** : S'assurer qu'il est communiqué de manière sécurisée à l'utilisateur (copie presse-papiers + affichage modal)
- **Verrouillage automatique** : Vérifier que le mécanisme de verrouillage après 5 échecs fonctionne (à tester via `GestionAuthentification`)
- **Cohérence des rôles** : Toujours vérifier que `role_utilisateur <= role_max_elevation`
- **Traçabilité** : Chaque action sensible doit apparaître dans les logs avec l'identité de l'admin

### Scénarios de tests avancés (optionnels)

- Test de charge : créer 100+ utilisateurs et vérifier les performances d'affichage
- Test de concurrence : modifier le même utilisateur depuis deux sessions admin simultanées
- Test d'intégration : créer un utilisateur, le modifier, le désactiver, le réactiver, réinitialiser son mot de passe, le déverrouiller

### Corrections prioritaires avant validation

1. **BUG-001** : Corriger le flow après création (fermeture form, refresh liste, message succès propre)
2. **BUG-002** : Passer les rôles comme chaînes ("Admin", "SuperUser", "User") au lieu d'entiers
3. **BUG-003** : Mettre à jour la colonne État après activation/désactivation

---

## ✅ VALIDATION FINALE

- [ ] Tous les bugs HIGH/MEDIUM corrigés
- [ ] Tous les tests de la checklist sont ✅
- [ ] Aucune erreur critique détectée
- [ ] Tous les logs sont corrects et sans mot de passe
- [ ] Améliorations UI considérées et priorisées
- [ ] La documentation technique est à jour
- [ ] Le CHANGELOG est mis à jour

---

**Date de validation** : _______________  
**Testeur** : _______________  
**Signature** : _______________

