# 📋 PLAN DE TESTS COMPLET - LOGIN & AUTHENTIFICATION

**Module** : Authentification utilisateur et gestion session  
**Version** : V1.0  
**Date** : 07/06/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

---

## 🎯 Objectif des tests

Valider le bon fonctionnement complet du système d'authentification, incluant :
- Form Login (saisie identifiants, validation)
- Authentification utilisateur (vérification hash/salt)
- Gestion des échecs de connexion et verrouillage automatique
- ChangePassword (changement obligatoire et volontaire)
- Création de session utilisateur
- Gestion des rôles et élévation temporaire
- Traçabilité via les logs (sans exposition données sensibles)

---

## ✅ CHECKLIST GÉNÉRALE

### Prérequis

- [x] Base de données MariaDB opérationnelle
- [x] Table `sec_utilisateurs` avec au moins 1 utilisateur Admin actif
- [x] Logs activés et fichier de log accessible
- [x] Application compilée sans erreur
- [x] Mot de passe de test connu pour l'utilisateur Admin

---

## 📂 SECTION 1 : FORM LOGIN - AFFICHAGE & NAVIGATION

### 1.1 - Chargement initial

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.1.1 | Lancer l'application | Form Login s'affiche | - Titre = "Althéa - Connexion"<br>- Splash screen affiché brièvement<br>- Login centré à l'écran<br>- Curseur dans `txtUserName` | ☐ |
| 1.1.2 | Vérifier l'état initial des contrôles | Contrôles correctement configurés | - `txtUserName` : vide, éditable, focus<br>- `txtPassword` : vide, masqué (UseSystemPasswordChar = True)<br>- `btnConnexion` : Enabled<br>- `btnAnnuler` : Enabled<br>- `btnVoirPassword` : Enabled | ☐ |
| 1.1.3 | Vérifier les tooltips | Tous les tooltips présents | - Survoler chaque contrôle<br>- Textes cohérents et utiles | ☐ |

### 1.2 - Bouton "Voir mot de passe"

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.2.1 | Saisir un mot de passe et maintenir le clic sur `btnVoirPassword` | Mot de passe visible pendant le clic | - `UseSystemPasswordChar` = False pendant MouseDown<br>- Texte visible en clair | ☐ |
| 1.2.2 | Relâcher le clic | Mot de passe masqué à nouveau | - `UseSystemPasswordChar` = True après MouseUp<br>- Texte masqué | ☐ |
| 1.2.3 | Cliquer et sortir la souris du bouton | Mot de passe masqué immédiatement | - `UseSystemPasswordChar` = True après MouseLeave | ☐ |

---

## 📂 SECTION 2 : VALIDATION FORMULAIRE

### 2.1 - Champs vides

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.1.1 | Cliquer sur `Connexion` sans saisir de login | Message d'erreur | - MessageBox ou label erreur : "Le nom d'utilisateur est requis."<br>- Focus sur `txtUserName`<br>- Aucune tentative de connexion | ☐ |
| 2.1.2 | Saisir login, laisser password vide, cliquer sur `Connexion` | Message d'erreur | - MessageBox ou label erreur : "Le mot de passe est requis."<br>- Focus sur `txtPassword`<br>- Aucune tentative de connexion | ☐ |

### 2.2 - Validation OK

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.2.1 | Saisir login et password, cliquer sur `Connexion` | Tentative d'authentification lancée | - Curseur = WaitCursor pendant validation<br>- Boutons désactivés temporairement | ☐ |

---

## 📂 SECTION 3 : AUTHENTIFICATION - CAS D'ERREUR

### 3.1 - Login incorrect

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.1.1 | Saisir login inexistant + password | Échec authentification | - MessageBox : "Identifiants incorrects."<br>- Aucune indication sur login ou password faux<br>- Form Login reste ouverte<br>- Champs vidés ou focus sur txtUserName | ☐ |
| 3.1.2 | Vérifier le log | Log créé sans info sensible | - Log : "Échec de connexion pour l'utilisateur '[login]'"<br>- Catégorie : Security<br>- Niveau : Rapide<br>- **AUCUN mot de passe dans le log** | ☐ |

### 3.2 - Password incorrect

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.2.1 | Saisir login correct + password incorrect | Échec authentification + incrément échecs | - MessageBox : "Identifiants incorrects."<br>- Form Login reste ouverte | ☐ |
| 3.2.2 | Vérifier le log | Log créé | - Log : "Échec de connexion pour l'utilisateur '[login]'"<br>- Catégorie : Security<br>- Niveau : Rapide | ☐ |
| 3.2.3 | Vérifier en base de données | Compteur échecs incrémenté | - `nb_echecs_login` += 1<br>- `dernier_login` non modifié | ☐ |

### 3.3 - Compte inactif

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.3.1 | Saisir login/password correct pour un compte inactif | Échec authentification | - MessageBox : "Compte désactivé. Contactez un administrateur."<br>- Form Login reste ouverte | ☐ |
| 3.3.2 | Vérifier le log | Log créé | - Log : "Tentative de connexion sur compte inactif '[login]'"<br>- Catégorie : Security<br>- Niveau : Rapide | ☐ |

### 3.4 - Compte verrouillé

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.4.1 | Saisir login/password correct pour un compte verrouillé | Échec authentification | - MessageBox : "Compte verrouillé suite à trop de tentatives. Contactez un administrateur."<br>- Form Login reste ouverte | ☐ |
| 3.4.2 | Vérifier le log | Log créé | - Log : "Tentative de connexion sur compte verrouillé '[login]'"<br>- Catégorie : Security<br>- Niveau : Rapide | ☐ |

### 3.5 - Verrouillage automatique après N échecs

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.5.1 | Effectuer 5 tentatives échouées consécutives sur un compte actif | Compte verrouillé automatiquement après le 5ème échec | - MessageBox après 5ème tentative : "Compte verrouillé..."<br>- 6ème tentative refuse la connexion | ☐ |
| 3.5.2 | Vérifier le log | Log de verrouillage créé | - Log : "Compte verrouillé automatiquement pour '[login]' (5 échecs)"<br>- Catégorie : Security<br>- Niveau : Rapide | ☐ |
| 3.5.3 | Vérifier en base de données | Champs mis à jour | - `compte_verrouille` = 1<br>- `nb_echecs_login` = 5<br>- `date_verrouillage` = maintenant | ☐ |

---

## 📂 SECTION 4 : AUTHENTIFICATION RÉUSSIE

### 4.1 - Connexion normale (sans changement password obligatoire)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.1.1 | Saisir login/password correct pour compte actif normal | Authentification réussie, ouverture Home | - Form Login se ferme<br>- Form Home s'ouvre<br>- Titre Home contient nom utilisateur<br>- Menu adapté au rôle utilisateur | ☐ |
| 4.1.2 | Vérifier la session créée | UserSession correctement initialisée | - `UserSession.AuthenticatedUser` != Nothing<br>- `UserSession.CurrentRole` = rôle de base utilisateur<br>- `UserSession.IsElevated` = False | ☐ |
| 4.1.3 | Vérifier le log de connexion | Log créé | - Log : "Connexion réussie pour l'utilisateur '[login]' (rôle: [role])"<br>- Catégorie : Security<br>- Niveau : Succinct | ☐ |
| 4.1.4 | Vérifier en base de données | Mise à jour dernier login | - `dernier_login` = maintenant<br>- `nb_echecs_login` = 0 (réinitialisé) | ☐ |

### 4.2 - Connexion avec changement password obligatoire

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.2.1 | Saisir login/password pour compte avec `must_change_password = 1` | Authentification OK, mais ouverture ChangePassword avant Home | - Form Login reste visible<br>- Form ChangePassword s'ouvre en mode modal<br>- Message : "Vous devez changer votre mot de passe avant de continuer." | ☐ |
| 4.2.2 | Annuler le changement de password | Retour au Login | - Form ChangePassword se ferme<br>- Form Login reste ouverte<br>- Session non créée<br>- Log : "Changement de mot de passe annulé pour '[login]'" | ☐ |
| 4.2.3 | Compléter le changement de password | Changement réussi, ouverture Home | - MessageBox succès<br>- Form ChangePassword se ferme<br>- Form Login se ferme<br>- Form Home s'ouvre<br>- `must_change_password` = 0 en base | ☐ |

---

## 📂 SECTION 5 : CHANGEPASSWORD - CHANGEMENT OBLIGATOIRE

### 5.1 - Affichage form en mode obligatoire

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.1.1 | Connexion avec `must_change_password = 1` | Form ChangePassword s'ouvre | - Titre : "Althéa - Changement de mot de passe obligatoire"<br>- Message explicatif visible<br>- `txtOldPassword` : éditable<br>- `txtNewPassword` : éditable<br>- `txtConfirmation` : éditable<br>- Tous champs masqués par défaut | ☐ |
| 5.1.2 | Vérifier tooltips | Tooltips présents et utiles | - Règles de complexité expliquées<br>- Conseils sécurité | ☐ |

### 5.2 - Validation du formulaire

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.2.1 | Cliquer sur `Valider` sans remplir les champs | Erreur validation | - MessageBox ou labels erreur : "Tous les champs sont requis."<br>- Focus sur premier champ vide | ☐ |
| 5.2.2 | Saisir ancien password incorrect | Erreur validation | - MessageBox : "L'ancien mot de passe est incorrect."<br>- Champs nouveau PW vidés<br>- Focus sur txtOldPassword | ☐ |
| 5.2.3 | Saisir nouveau password différent de confirmation | Erreur validation | - MessageBox : "Le nouveau mot de passe et la confirmation ne correspondent pas."<br>- Focus sur txtConfirmation | ☐ |
| 5.2.4 | Saisir nouveau password identique à l'ancien | Erreur validation | - MessageBox : "Le nouveau mot de passe doit être différent de l'ancien."<br>- Champs PW vidés | ☐ |
| 5.2.5 | Saisir nouveau password trop faible (< 8 caractères) | Erreur validation | - MessageBox avec règles de complexité<br>- Focus sur txtNewPassword | ☐ |

### 5.3 - Changement réussi

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.3.1 | Saisir ancien PW correct, nouveau PW valide, confirmation OK | Changement réussi | - MessageBox succès<br>- Form ChangePassword se ferme<br>- Retour au processus login (ouverture Home) | ☐ |
| 5.3.2 | Vérifier le log | Log créé sans mot de passe | - Log : "Mot de passe changé avec succès pour '[login]'"<br>- Catégorie : Security<br>- Niveau : Rapide<br>- **AUCUN mot de passe dans le log** | ☐ |
| 5.3.3 | Vérifier en base de données | Mot de passe mis à jour | - `password_hash` modifié<br>- `password_salt` modifié<br>- `must_change_password` = 0<br>- `date_modification` = maintenant | ☐ |

---

## 📂 SECTION 6 : CHANGEPASSWORD - CHANGEMENT VOLONTAIRE

### 6.1 - Ouverture depuis Home

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.1.1 | Une fois connecté, cliquer sur "Changer mon mot de passe" depuis Home | Form ChangePassword s'ouvre en mode volontaire | - Titre : "Althéa - Changement de mot de passe"<br>- Même formulaire que mode obligatoire<br>- Bouton Annuler actif (ferme sans changer) | ☐ |
| 6.1.2 | Annuler le changement | Form se ferme sans changement | - DialogResult.Cancel<br>- Aucune modification en base<br>- Aucun log de changement | ☐ |
| 6.1.3 | Compléter le changement | Changement réussi | - MessageBox succès<br>- Form se ferme<br>- Session reste active<br>- Log créé | ☐ |

---

## 📂 SECTION 7 : ÉLÉVATION TEMPORAIRE (ElevationAcces)

### 7.1 - Ouverture form élévation

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.1.1 | Depuis UC_AdminHome, cliquer sur `btnEleverAcces` | Form ElevationAcces s'ouvre | - Titre : "Althéa - Élévation temporaire"<br>- ComboBox rôles remplie avec rôles accessibles<br>- Password masqué par défaut<br>- Message explicatif visible | ☐ |
| 7.1.2 | Vérifier les rôles disponibles | Seuls rôles supérieurs au rôle courant et <= RoleMaxElevation | - Exemple : User (max SuperUser) → seul SuperUser proposé<br>- Exemple : SuperUser (max Admin) → seul Admin proposé<br>- Exemple : Admin → aucun rôle (bouton élever désactivé) | ☐ |

### 7.2 - Élévation échouée

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.2.1 | Saisir password incorrect | Échec élévation | - MessageBox : "Mot de passe incorrect."<br>- Form reste ouverte<br>- Pas de modification session | ☐ |
| 7.2.2 | Vérifier le log | Log créé | - Log : "Échec d'élévation pour '[login]' vers rôle [role]"<br>- Catégorie : Security<br>- Niveau : Rapide | ☐ |

### 7.3 - Élévation réussie

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.3.1 | Sélectionner rôle cible, saisir password correct, valider | Élévation réussie | - MessageBox succès<br>- Form se ferme<br>- UC_AdminHome mis à jour (nouveaux boutons visibles)<br>- Label rôle courant actualisé | ☐ |
| 7.3.2 | Vérifier la session | Session élevée | - `UserSession.CurrentRole` = rôle élevé<br>- `UserSession.IsElevated` = True | ☐ |
| 7.3.3 | Vérifier le log | Log créé | - Log : "Élévation réussie pour '[login]' : [role_base] → [role_élevé]"<br>- Catégorie : Security<br>- Niveau : Rapide | ☐ |

### 7.4 - Retour au rôle de base

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.4.1 | Cliquer sur `btnRetourRoleBase` dans UC_AdminHome | Retour au rôle naturel | - Confirmation demandée<br>- Session réinitialisée<br>- UC_AdminHome mis à jour (boutons admin cachés si nécessaire)<br>- Si User : redirection automatique vers Accueil | ☐ |
| 7.4.2 | Vérifier la session | Session normale | - `UserSession.CurrentRole` = rôle de base<br>- `UserSession.IsElevated` = False | ☐ |
| 7.4.3 | Vérifier le log | Log créé | - Log : "Retour au rôle de base pour '[login]' : [role_élevé] → [role_base]"<br>- Catégorie : Security<br>- Niveau : Rapide | ☐ |

---

## 📂 SECTION 8 : DÉCONNEXION & FERMETURE

### 8.1 - Déconnexion normale

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.1.1 | Fermer la Form Home via le bouton Quitter | Confirmation demandée | - MessageBox : "Voulez-vous vraiment quitter l'application ?"<br>- Oui → Application se ferme<br>- Non → Home reste ouverte | ☐ |
| 8.1.2 | Vérifier le log de déconnexion | Log créé | - Log : "Déconnexion de l'utilisateur '[login]'"<br>- Catégorie : Security<br>- Niveau : Succinct | ☐ |
| 8.1.3 | Vérifier la session | Session détruite | - `UserSession.AuthenticatedUser` = Nothing | ☐ |

---

## 📂 SECTION 9 : TESTS DE SÉCURITÉ

### 9.1 - Injection SQL

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 9.1.1 | Saisir `' OR '1'='1` dans login et password | Échec authentification (aucune injection) | - MessageBox : "Identifiants incorrects."<br>- Aucune connexion établie<br>- Log normal d'échec | ☐ |

### 9.2 - Tentatives de force brute

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 9.2.1 | Effectuer 10 tentatives échouées rapides | Verrouillage après 5 échecs | - Compte verrouillé au 5ème échec<br>- Tentatives 6 à 10 refusées immédiatement<br>- Log de verrouillage créé | ☐ |

### 9.3 - Mots de passe en mémoire

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 9.3.1 | Vérifier que les mots de passe ne sont jamais stockés en clair | Aucun mot de passe en clair | - Variables password effacées après usage<br>- Logs ne contiennent jamais de mots de passe<br>- Base contient uniquement hash/salt | ☐ |

---

## 🎯 RÉCAPITULATIF DES TESTS

| Catégorie | Total tests | Tests OK | Tests KO | Non testés |
|-----------|-------------|----------|----------|------------|
| Section 1 : Form Login | 3 | - | - | - |
| Section 2 : Validation | 3 | - | - | - |
| Section 3 : Erreurs auth | 15 | - | - | - |
| Section 4 : Auth réussie | 8 | - | - | - |
| Section 5 : ChangePassword obligatoire | 9 | - | - | - |
| Section 6 : ChangePassword volontaire | 3 | - | - | - |
| Section 7 : Élévation | 10 | - | - | - |
| Section 8 : Déconnexion | 3 | - | - | - |
| Section 9 : Sécurité | 3 | - | - | - |
| **TOTAL** | **57** | **-** | **-** | **-** |

---

## 📝 BUGS IDENTIFIÉS PENDANT LES TESTS

| # | Bug | Priorité | Statut |
|---|-----|----------|--------|
| - | - | - | - |

---

## ✅ VALIDATION FINALE

- [ ] Tous les tests de Section 1 à 9 passés
- [ ] Aucun bug bloquant restant
- [ ] Logs vérifiés (aucun mot de passe exposé)
- [ ] Base de données cohérente après tous les tests
- [ ] Documentation mise à jour si nécessaire

---

> **Contact** : ***Joëlle (Manou) - Les Artefacts de Manou***
> 
> Projet Althéa - Tests d'authentification et sécurité
