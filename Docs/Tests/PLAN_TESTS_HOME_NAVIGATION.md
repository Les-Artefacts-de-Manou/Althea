# 📋 PLAN DE TESTS COMPLET - HOME & NAVIGATION

**Module** : Form principale, navigation entre UserControls, gestion du contexte  
**Version** : V1.0  
**Date** : 07/06/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

---

## 🎯 Objectif des tests

Valider le bon fonctionnement complet de la form principale `Home` et de la navigation, incluant :
- Chargement et affichage de Home
- Menu de navigation et accès aux UserControls
- Gestion du contexte (UserControlContext)
- Synchronisation Header / Status / ErrorProvider / ToolTipMain
- Gestion des rôles et visibilité des menus
- Navigation inter-UC (retours, fermetures)
- Fermeture propre de l'application

---

## ✅ CHECKLIST GÉNÉRALE

### Prérequis

- [x] Base de données MariaDB opérationnelle
- [x] Au moins 1 utilisateur Admin actif
- [x] Au moins 1 utilisateur SuperUser actif
- [x] Au moins 1 utilisateur User actif
- [x] Application compilée sans erreur
- [x] Logs activés

---

## 📂 SECTION 1 : CHARGEMENT DE HOME

### 1.1 - Ouverture après connexion réussie

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.1.1 | Se connecter en tant qu'Admin | Home s'ouvre | - Form Login se ferme<br>- Home s'ouvre maximisée (ou taille définie)<br>- Titre = "Althéa - [Nom Utilisateur] ([Rôle])"<br>- lblContexte = "Bienvenue, [Prénom] [Nom]" ou similaire | ☐ |
| 1.1.2 | Vérifier la session utilisateur | UserSession.AuthenticatedUser chargé | - `UserSession.AuthenticatedUser` != Nothing<br>- `UserSession.CurrentRole` = rôle de base<br>- `UserSession.IsElevated` = False | ☐ |
| 1.1.3 | Vérifier le log de chargement Home | Log créé | - Log : "Ouverture de la form Home pour l'utilisateur '[login]'"<br>- Catégorie : Application<br>- Niveau : Succinct | ☐ |

### 1.2 - Interface initiale

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.2.1 | Vérifier le menu principal | Menu visible et complet | - MenuStrip présent<br>- Tous les menus affichés selon rôle<br>- Police Calibri cohérente | ☐ |
| 1.2.2 | Vérifier le panel conteneur UC | Panel vide ou UC_Accueil affiché | - `pnlConteneurUC` présent<br>- Dock = Fill<br>- UC_Accueil chargé par défaut (optionnel) | ☐ |
| 1.2.3 | Vérifier le footer | Footer avec lblContexte, Status, ErrorProvider | - `lblContexte` : texte initial<br>- `lblStatus` : vide ou "Prêt"<br>- `ErrorProvider1` : actif<br>- `ToolTipMain` : actif | ☐ |

---

## 📂 SECTION 2 : MENU & NAVIGATION

### 2.1 - Menu Accueil

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.1.1 | Cliquer sur "Accueil" | UC_Accueil affiché | - UC_Accueil chargé dans `pnlConteneurUC`<br>- Header = "Accueil"<br>- Status = "Bienvenue" ou message pertinent | ☐ |
| 2.1.2 | Vérifier le contenu UC_Accueil | Contenu pertinent affiché | - Résumé activité<br>- Boutons d'accès rapide<br>- Messages de bienvenue | ☐ |

### 2.2 - Menu Administration (Admin / SuperUser uniquement)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.2.1 | Se connecter en Admin, cliquer sur "Administration" | Menu Administration visible | - Sous-menu "Gestion des utilisateurs"<br>- Sous-menu "Paramètres" (si applicable) | ☐ |
| 2.2.2 | Cliquer sur "Gestion des utilisateurs" | UC_AdminHome affiché | - UC_AdminHome chargé<br>- Header = "Administration"<br>- Status = message pertinent | ☐ |
| 2.2.3 | Se connecter en User, vérifier menu Administration | Menu Administration masqué | - Menu Administration non visible<br>- Aucune entrée liée à l'administration | ☐ |

### 2.3 - Menu Paramètres (Admin / SuperUser)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.3.1 | Cliquer sur "Paramètres" | UC_Parametres affiché | - UC_Parametres chargé<br>- Header = "Paramètres > [Groupe]"<br>- Status = "[N] paramètre(s) chargé(s)." | ☐ |
| 2.3.2 | Se connecter en User, vérifier menu Paramètres | Menu Paramètres masqué | - Menu non visible pour User | ☐ |

### 2.4 - Menu Patients (futur)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.4.1 | Vérifier présence menu Patients | Menu affiché (quand implémenté) | - Sous-menus : "Liste des patients", "Nouveau patient"<br>- Visibilité selon rôle | ☐ |

### 2.5 - Menu Dossiers (futur)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.5.1 | Vérifier présence menu Dossiers | Menu affiché (quand implémenté) | - Sous-menus : "Dossiers actifs", "Recherche dossier"<br>- Visibilité selon rôle | ☐ |

### 2.6 - Menu Aide

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.6.1 | Cliquer sur "Aide" | Sous-menu affiché | - "À propos"<br>- "Documentation" (si implémenté)<br>- "Contact" (si applicable) | ☐ |
| 2.6.2 | Cliquer sur "À propos" | Fenêtre À propos affichée | - Dialogue avec informations application<br>- Version, auteur, licence | ☐ |

### 2.7 - Menu Quitter

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.7.1 | Cliquer sur "Quitter" | Demande de confirmation | - DialogChoix : "Voulez-vous vraiment quitter l'application ?"<br>- Boutons Oui/Non | ☐ |
| 2.7.2 | Confirmer Oui | Application se ferme | - Déconnexion propre<br>- Log : "Déconnexion de l'utilisateur '[login]'"<br>- Application.Exit() | ☐ |
| 2.7.3 | Confirmer Non | Home reste ouverte | - Dialogue se ferme<br>- Aucune déconnexion<br>- Home reste active | ☐ |

---

## 📂 SECTION 3 : USERCONTROLCONTEXT

### 3.1 - Création et passage du contexte

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.1.1 | Charger un UC depuis Home | UserControlContext passé correctement | - `UC.SetContext(context)` appelé<br>- Context contient : lblContexte, lblStatus, ErrorProvider, ToolTipMain, UtilisateurApplication | ☐ |
| 3.1.2 | Vérifier contenu du contexte | Toutes les références présentes | - `context.LblContexte` != Nothing<br>- `context.LblStatus` != Nothing<br>- `context.ErrorProvider` != Nothing<br>- `context.ToolTipMain` != Nothing<br>- `context.UtilisateurApplication` != Nothing | ☐ |

### 3.2 - Utilisation du contexte dans les UC

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.2.1 | UC_Utilisateurs : modifier lblContexte | lblContexte de Home mis à jour | - Exemple : "Gestion utilisateurs > Consultation"<br>- Texte affiché dans footer Home | ☐ |
| 3.2.2 | UC_Utilisateurs : modifier lblStatus | lblStatus de Home mis à jour | - Exemple : "3 utilisateurs chargés."<br>- Texte affiché dans footer Home | ☐ |
| 3.2.3 | UtilisateurEdition : définir ErrorProvider | ErrorProvider de Home actif | - Exemple : champ vide → ErrorProvider affiche icône erreur<br>- Tooltip erreur visible | ☐ |
| 3.2.4 | UC_Parametres : utiliser ToolTipMain | ToolTipMain de Home actif | - Survoler contrôle → tooltip affiché<br>- Texte cohérent | ☐ |

### 3.3 - Nettoyage du contexte lors de changement UC

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.3.1 | Naviguer de UC_Utilisateurs vers UC_Parametres | lblContexte et Status réinitialisés | - lblContexte = nouveau contexte<br>- lblStatus = nouveau statut<br>- ErrorProvider réinitialisé (aucune erreur persistante) | ☐ |
| 3.3.2 | Fermer un UC | Footer Home réinitialisé | - lblContexte = "Prêt" ou contexte par défaut<br>- lblStatus = vide ou "Prêt" | ☐ |

---

## 📂 SECTION 4 : GESTION DES RÔLES

### 4.1 - Rôle Admin

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.1.1 | Se connecter en Admin | Tous les menus visibles | - Accueil<br>- Administration (Gestion utilisateurs)<br>- Paramètres<br>- Patients (futur)<br>- Dossiers (futur)<br>- Aide<br>- Quitter | ☐ |
| 4.1.2 | Vérifier titre Home | Titre affiche rôle Admin | - "Althéa - [Nom] (Administrateur)" | ☐ |

### 4.2 - Rôle SuperUser

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.2.1 | Se connecter en SuperUser | Menus admin + opérationnels visibles | - Accueil<br>- Administration (Gestion utilisateurs avec droits limités)<br>- Paramètres (modification valeurs uniquement)<br>- Patients (futur)<br>- Dossiers (futur)<br>- Aide<br>- Quitter | ☐ |
| 4.2.2 | Vérifier titre Home | Titre affiche rôle SuperUser | - "Althéa - [Nom] (SuperUtilisateur)" | ☐ |

### 4.3 - Rôle User

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.3.1 | Se connecter en User | Seuls menus opérationnels visibles | - Accueil<br>- Patients (futur)<br>- Dossiers (futur)<br>- Aide<br>- Quitter<br>- **Pas de menu Administration ni Paramètres** | ☐ |
| 4.3.2 | Vérifier titre Home | Titre affiche rôle User | - "Althéa - [Nom] (Utilisateur)" | ☐ |
| 4.3.3 | Tenter d'accéder à Administration via code | Accès refusé | - Si tentative manuelle : erreur ou redirection<br>- Log : "Tentative d'accès non autorisé" | ☐ |

### 4.4 - Élévation temporaire

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.4.1 | SuperUser : élever vers Admin | Menu mis à jour | - Après élévation réussie : menus Admin disponibles<br>- Titre Home mis à jour : "(Administrateur - Élevé)" | ☐ |
| 4.4.2 | Retour au rôle de base | Menu rétabli | - Menus admin cachés<br>- Titre Home mis à jour : "(SuperUtilisateur)" | ☐ |

---

## 📂 SECTION 5 : NAVIGATION INTER-UC

### 5.1 - Chargement dynamique UC

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.1.1 | Charger UC_Accueil depuis menu | UC_Accueil affiché dans pnlConteneurUC | - Ancien UC supprimé<br>- Nouveau UC chargé<br>- Context passé | ☐ |
| 5.1.2 | Charger UC_AdminHome depuis menu | UC_AdminHome affiché | - UC précédent déchargé proprement<br>- UC_AdminHome chargé<br>- Context passé | ☐ |
| 5.1.3 | Charger UC_Utilisateurs depuis UC_AdminHome | UC_Utilisateurs affiché | - Navigation depuis bouton dans UC_AdminHome<br>- UC_Utilisateurs chargé<br>- Context passé | ☐ |

### 5.2 - Retour arrière (navigation secondaire)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.2.1 | UC_Utilisateurs → bouton Retour | Retour à UC_AdminHome | - UC_Utilisateurs déchargé<br>- UC_AdminHome rechargé<br>- Context réinitialisé | ☐ |
| 5.2.2 | UtilisateurEdition → bouton Annuler | Retour à UC_Utilisateurs | - Form UtilisateurEdition fermée<br>- UC_Utilisateurs reste actif<br>- Liste rafraîchie si nécessaire | ☐ |

### 5.3 - Fermeture Forms modales

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.3.1 | Ouvrir UtilisateurEdition, enregistrer | Form se ferme, retour à UC_Utilisateurs | - DialogResult.OK<br>- Liste UC_Utilisateurs rafraîchie<br>- Nouveau/modifié utilisateur visible | ☐ |
| 5.3.2 | Ouvrir UtilisateurEdition, annuler | Form se ferme sans sauvegarde | - DialogResult.Cancel<br>- Aucune modification en base<br>- Liste UC_Utilisateurs inchangée | ☐ |
| 5.3.3 | Ouvrir ChangePassword, valider | Form se ferme après succès | - DialogResult.OK<br>- MessageBox succès<br>- Session reste active | ☐ |

---

## 📂 SECTION 6 : HEADER & STATUS SYNCHRONISÉS

### 6.1 - Mise à jour dynamique lblContexte

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.1.1 | Charger UC_Utilisateurs | lblContexte = "Gestion utilisateurs > Consultation" | - Texte affiché dans footer Home<br>- Mis à jour automatiquement | ☐ |
| 6.1.2 | Sélectionner un utilisateur dans UC_Utilisateurs | lblContexte = "Gestion utilisateurs > Sélectionné : [Nom]" | - Texte dynamique selon sélection | ☐ |
| 6.1.3 | Passer en mode création dans UC_Utilisateurs | lblContexte = "Gestion utilisateurs > Nouveau" | - Texte mis à jour | ☐ |

### 6.2 - Mise à jour dynamique lblStatus

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.2.1 | Charger UC_Utilisateurs | lblStatus = "3 utilisateur(s) chargé(s)." | - Nombre correct<br>- Texte clair | ☐ |
| 6.2.2 | Filtrer la liste UC_Utilisateurs | lblStatus = "1 utilisateur(s) affiché(s) sur 3." | - Nombre affiché / total<br>- Mis à jour automatiquement | ☐ |
| 6.2.3 | Enregistrer un paramètre dans UC_Parametres | lblStatus = "Paramètre enregistré avec succès." | - Message temporaire<br>- Revient à "Prêt" après quelques secondes (optionnel) | ☐ |

### 6.3 - ErrorProvider synchronisé

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.3.1 | Laisser un champ obligatoire vide dans UtilisateurEdition | ErrorProvider affiche icône erreur | - Icône rouge à côté du champ<br>- Tooltip explicatif | ☐ |
| 6.3.2 | Corriger le champ | ErrorProvider disparaît | - Icône erreur supprimée<br>- Validation OK | ☐ |

### 6.4 - ToolTipMain synchronisé

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.4.1 | Survoler un bouton dans UC_Utilisateurs | ToolTipMain affiche texte explicatif | - Tooltip via ToolTipMain de Home<br>- Texte cohérent | ☐ |
| 6.4.2 | Survoler un contrôle dans UC_Parametres | ToolTipMain affiche texte explicatif | - Tooltip via ToolTipMain de Home<br>- Texte cohérent | ☐ |

---

## 📂 SECTION 7 : FERMETURE APPLICATION

### 7.1 - Fermeture via menu Quitter

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.1.1 | Cliquer sur Quitter, confirmer Oui | Application se ferme proprement | - Dialogue confirmation<br>- Déconnexion user<br>- Log : "Déconnexion de l'utilisateur '[login]'"<br>- Application.Exit() | ☐ |
| 7.1.2 | Vérifier nettoyage session | Session détruite | - `UserSession.AuthenticatedUser` = Nothing<br>- Aucune référence mémoire persistante | ☐ |

### 7.2 - Fermeture via bouton X (croix)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.2.1 | Cliquer sur X de la fenêtre | Demande de confirmation | - DialogChoix : "Voulez-vous vraiment quitter ?"<br>- Boutons Oui/Non | ☐ |
| 7.2.2 | Confirmer Oui | Application se ferme | - Même comportement que menu Quitter<br>- Déconnexion propre | ☐ |
| 7.2.3 | Confirmer Non | Home reste ouverte | - e.Cancel = True<br>- Aucune fermeture | ☐ |

### 7.3 - Fermeture forcée (Alt+F4)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.3.1 | Appuyer sur Alt+F4 | Demande de confirmation | - Même comportement que bouton X<br>- Dialogue confirmation | ☐ |

---

## 📂 SECTION 8 : ROBUSTESSE & PERFORMANCE

### 8.1 - Gestion mémoire

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.1.1 | Naviguer entre 10 UC différents | Pas de fuite mémoire | - Ancien UC.Dispose() appelé<br>- Mémoire libérée<br>- Performance stable | ☐ |

### 8.2 - Réactivité UI

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.2.1 | Charger un UC avec beaucoup de données | UI reste réactive | - Chargement asynchrone si possible<br>- Curseur WaitCursor pendant chargement<br>- Pas de freeze UI | ☐ |

### 8.3 - Gestion erreurs

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.3.1 | Simuler erreur DB pendant chargement UC | Erreur capturée | - MessageBox : "Erreur de connexion..."<br>- Log erreur<br>- Home reste stable | ☐ |

---

## 🎯 RÉCAPITULATIF DES TESTS

| Catégorie | Total tests | Tests OK | Tests KO | Non testés |
|-----------|-------------|----------|----------|------------|
| Section 1 : Chargement Home | 5 | - | - | - |
| Section 2 : Menu & Navigation | 17 | - | - | - |
| Section 3 : UserControlContext | 8 | - | - | - |
| Section 4 : Gestion rôles | 11 | - | - | - |
| Section 5 : Navigation inter-UC | 9 | - | - | - |
| Section 6 : Header & Status | 12 | - | - | - |
| Section 7 : Fermeture | 7 | - | - | - |
| Section 8 : Robustesse | 3 | - | - | - |
| **TOTAL** | **72** | **-** | **-** | **-** |

---

## 📝 BUGS IDENTIFIÉS PENDANT LES TESTS

| # | Bug | Priorité | Statut |
|---|-----|----------|--------|
| - | - | - | - |

---

## ✅ VALIDATION FINALE

- [ ] Tous les tests de Section 1 à 8 passés
- [ ] Aucun bug bloquant restant
- [ ] Navigation fluide entre tous les UC
- [ ] Context synchronisé partout
- [ ] Rôles respectés
- [ ] Fermeture propre sans fuite mémoire
- [ ] Documentation mise à jour si nécessaire

---

> **Contact** : ***Joëlle (Manou) - Les Artefacts de Manou***
> 
> Projet Althéa - Tests Home & Navigation
