# 📋 PLAN DE TESTS COMPLET - HOME & NAVIGATION

**Module** : Form principale, navigation entre UserControls, gestion du contexte  
**Version** : V1.2  
**Date** : 25/10/2026 (section 9 : fenêtre principale par contrat, audit C.4) — 23/10/2026 (menu de gauche : ordre des boutons et bouton Recherche sous Patients, mini-pile de navigation et Retour : 1.2.1, 2.0, 2.2.3, 2.4, 2.5, 4.x.1, 5.4) — 07/06/2026  
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

> **Note (23/10/2026)** : ce plan a été écrit le 07/06/2026, quand `Home` avait une barre de menus.
>
> Le menu est aujourd'hui une colonne de boutons à gauche (`pnlMenu`, boutons `AltheaBoutonMenu`), visibles pour tous les rôles ; l'accès à Outils/Admin et à Référentiels est contrôlé au clic (élévation).
>
> Les cas 1.2.1, 2.0, 2.2.3, 2.4, 2.5, 4.1.1, 4.2.1, 4.3.1 et 5.4 décrivent le menu actuel ; les autres (menus Aide et Quitter, en particulier) sont à revoir à la prochaine exécution du plan.
>
> La recherche elle-même est testée dans `PLAN_TESTS_RECHERCHE.md`.

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
| 1.2.1 | Vérifier le menu principal | Menu de gauche visible et complet | - Boutons dans l'ordre du cas 2.0.1<br>- Tous les boutons affichés, quel que soit le rôle<br>- Police Calibri cohérente | ☐ |
| 1.2.2 | Vérifier le panel conteneur UC | Panel vide ou UC_Accueil affiché | - `pnlConteneurUC` présent<br>- Dock = Fill<br>- UC_Accueil chargé par défaut (optionnel) | ☐ |
| 1.2.3 | Vérifier le footer | Footer avec lblContexte, Status, ErrorProvider | - `lblContexte` : texte initial<br>- `lblStatus` : vide ou "Prêt"<br>- `ErrorProvider1` : actif<br>- `ToolTipMain` : actif | ☐ |

---

## 📂 SECTION 2 : MENU & NAVIGATION

### 2.0 - Menu de gauche : ordre des boutons

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.0.1 | Se connecter (User, puis Admin) et regarder le menu de gauche | De haut en bas : **Accueil**, **Patients**, **Recherche**, **Domaines**, **Agenda**, **Documents**, **Référentiels** ; en bas : **Outils/Admin**, **Changer mon mot de passe** | - **Recherche** juste sous Patients (23/10/2026), les boutons suivants décalés d'un cran<br>- Rien ne se chevauche<br>- Mêmes boutons pour tous les rôles | ☐ |
| 2.0.2 | Cliquer chaque bouton tour à tour | Le bouton cliqué passe à l'état sélectionné, les autres reviennent à l'état normal | - Survol visible<br>- **Agenda** et **Documents** : bouton sélectionné et barre d'état « Agenda » / « Documents », l'écran ne change pas (modules à venir) | ☐ |

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
| 2.2.3 | Se connecter en User, cliquer sur **Outils/Admin** | Élévation demandée | - DialogChoix « Cette zone nécessite une élévation de privilèges… »<br>- Non : barre d'état « Accès Administration annulé. », l'écran ne change pas | ☐ |

### 2.3 - Menu Paramètres (Admin / SuperUser)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.3.1 | Cliquer sur "Paramètres" | UC_Parametres affiché | - UC_Parametres chargé<br>- Header = "Paramètres > [Groupe]"<br>- Status = "[N] paramètre(s) chargé(s)." | ☐ |
| 2.3.2 | Se connecter en User, vérifier menu Paramètres | Menu Paramètres masqué | - Menu non visible pour User | ☐ |

### 2.4 - Bouton Patients

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.4.1 | Cliquer sur **Patients** | Liste des patients (`UC_PatientHome`) affichée | - Contexte « Patients »<br>- Bouton Patients sélectionné | ☐ |
| 2.4.2 | Ouvrir un dossier | Pas de bouton Dossiers dans le menu : un dossier s'ouvre depuis la fiche patient (onglet Dossiers) ou depuis la Recherche | - Détail : `PLAN_TESTS_DOSSIERS.md` | ☐ |

### 2.5 - Bouton Recherche (23/10/2026)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.5.1 | Cliquer sur **Recherche** | Hub Recherche (`UC_RechercheHome`) affiché | - Contexte « Recherche »<br>- Bouton Recherche sélectionné<br>- Contenu du hub : `PLAN_TESTS_RECHERCHE.md`, RCH-01 et RCH-02 | ☐ |
| 2.5.2 | Depuis la recherche, ouvrir un élément (fiche patient, dossier), puis **Retour** | L'élément s'ouvre, le bouton **Recherche** reste sélectionné ; Retour ramène à la recherche, critères et résultats compris | - Contexte « Recherche > Nom Prénom … », puis « Recherche » au retour (`Home.NavigateDepuisRecherche`)<br>- Détail par élément : RCH-03 à RCH-08, RCH-22, RCH-23, RCH-37 | ☐ |

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
| 4.1.1 | Se connecter en Admin | Tous les boutons du menu visibles | - Boutons du cas 2.0.1<br>- Outils/Admin et Référentiels s'ouvrent sans demande d'élévation | ☐ |
| 4.1.2 | Vérifier titre Home | Titre affiche rôle Admin | - "Althéa - [Nom] (Administrateur)" | ☐ |

### 4.2 - Rôle SuperUser

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.2.1 | Se connecter en SuperUser | Tous les boutons du menu visibles | - Boutons du cas 2.0.1<br>- Outils/Admin et Référentiels s'ouvrent sans demande d'élévation ; droits limités dans les écrans d'administration (`PLAN_TESTS_UC_UTILISATEURS.md`, `PLAN_TESTS_UC_PARAMETRES.md`) | ☐ |
| 4.2.2 | Vérifier titre Home | Titre affiche rôle SuperUser | - "Althéa - [Nom] (SuperUtilisateur)" | ☐ |

### 4.3 - Rôle User

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.3.1 | Se connecter en User | Mêmes boutons visibles ; accès contrôlé au clic | - Boutons du cas 2.0.1, Recherche compris<br>- Outils/Admin et Référentiels demandent une élévation (cas 2.2.3) | ☐ |
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

### 5.4 - Mini-pile de navigation (Retour)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.4.1 | Patients › fiche › onglet Dossiers › un dossier › **Retour**, puis **Retour** | Retour sur la fiche (onglet Dossiers), puis sur la liste des patients avec ses filtres | - Bouton Patients sélectionné tout du long<br>- Même parcours que RCH-12 | ☐ |
| 5.4.2 | Depuis la recherche, ouvrir un dossier ; cliquer **Patients** dans le menu ; ouvrir une fiche, puis **Retour** | Retour ramène à la liste des patients, pas à la recherche | - Un clic dans le menu repart d'une chaîne de retour vide (`NavigateTo` vide la mini-pile) | ☐ |
| 5.4.3 | Recherche › tuile **Séances** (ou **Paiements**) › **Retour à la recherche** | Retour au hub Recherche | - Contexte « Recherche »<br>- Si la pile est vide, le hub est rouvert (`Home.NavigateToRecherche`) | ☐ |

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

## 📂 SECTION 9 : FENÊTRE PRINCIPALE PAR CONTRAT (audit C.4, ADR-55)

Aucun comportement ne doit changer : les fenêtres reçoivent la fenêtre principale par le contrat `IFenetrePrincipale` au lieu de la classe `Home`. Données de test uniquement ; fermer les référentiels sans enregistrer, sauf ajout volontaire d'une valeur de test.

### 9.1 - Étape 1 : fenêtres (25/10/2026 — validés par Joëlle)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| NAV-C4-01 | Fiche patient › Famille / Contacts : nouveau contact, puis un contact existant en « Modifier ». Boutons [+] du lien et du rôle légal. | [+] visibles et actifs en édition, grisés en consultation ; chaque [+] ouvre le référentiel en fenêtre ; à la fermeture, la liste est relue (valeur ajoutée sélectionnée). | Compte User : l'élévation est demandée comme avant. | ✅ |
| NAV-C4-02 | Fiche patient › Intervenants : nouveau suivi. [+] du rôle, [+] du thérapeute (création), loupe (liste des thérapeutes). | Comme avant : référentiel des rôles, fenêtre de création d'un thérapeute (le nouveau est sélectionné), liste des thérapeutes en fenêtre. | — | ✅ |
| NAV-C4-03 | Référentiels › Thérapeutes : modifier un thérapeute, [+] de la spécialité ; puis Recherche › Réseau par nom › « Fiche de la personne » sur un thérapeute. | [+] actif dans les deux cas, référentiel des spécialités en fenêtre. | Depuis Intervenant › loupe › un thérapeute ouvert dans cette fenêtre : [+] inactif, comme avant (pas de fenêtre principale dans un référentiel en fenêtre). | ✅ |
| NAV-C4-04 | Dossier › Séances : nouvelle séance et séance existante ; écran Patients : ouvrir une séance depuis l'aperçu. [+] du tarif, de la nature, du statut. | [+] visibles et fonctionnels ; dossier archivé : [+] sans effet (lecture seule), comme avant. | — | ✅ |
| NAV-C4-05 | Dossier › Séances : encaisser une séance ; [+] du mode de paiement. Journal de la session. | Référentiel des modes en fenêtre, liste relue ; aucune erreur au journal. | — | ✅ |

### 9.2 - Étape 2 : écrans (25/10/2026 — validés par Joëlle)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| NAV-C4-06 | Écran Patients : double-clic sur un patient, « Ouvrir » ; aperçu : « Ouvrir la fiche », « Ouvrir le dossier DO… » ; aperçu : double-clic sur une séance. | Fiche, dossier et fenêtre Séance (avec ses [+]) s'ouvrent comme avant ; Retour revient à la liste avec ses critères. | — | ✅ |
| NAV-C4-07 | Fiche patient : [+] Situation familiale et [+] Mutualité (onglet Identité, en Modifier) ; Famille / Contacts et Intervenants : ajouter / ouvrir ; onglet Dossiers : ouvrir un dossier ; « Retour patients ». | Référentiels en fenêtre et listes relues ; fenêtres Contact et Intervenant avec leurs [+] ; dossier ouvert, Retour à la fiche ; « Retour patients » revient à la liste. | Modification en cours puis Retour : la question « modifications non enregistrées » est posée comme avant. | ✅ |
| NAV-C4-08 | Dossier : nouvelle séance, ouvrir une séance, encaisser ; « Retour patients ». | Fenêtres Séance et Paiement avec leurs [+] ; Retour comme avant. | — | ✅ |
| NAV-C4-09 | Domaines, étape **2. Tarifs** : à droite de la barre (après Supprimer), **Créer un nouveau tarif…** ; créer un tarif de test dans le catalogue, fermer ; puis **Rattacher un tarif** et le choisir dans « Nouveau rattachement ». Même chose à l'étape **4. Natures de séance** (**Créer une nouvelle nature…**). Onglet Général : « Nouvel organisme ». | L'action de création est visible dès l'ouverture de l'étape (elle n'apparaissait pas : corrigé le 25/10/2026) ; le catalogue s'ouvre en fenêtre ; au retour, le nouveau tarif (la nouvelle nature) est proposé dans la liste du rattachement. | Fenêtre agrandie puis restaurée : l'action reste à droite de la barre. Supprimer ensuite le tarif et la nature de test (ou les désactiver). | ✅ |
| NAV-C4-10 | Cabinet : modifier puis enregistrer le nom du cabinet (le remettre ensuite) ; Utilisateurs : changer sa propre photo ; Référentiels › Thérapeutes : modifier un thérapeute ([+] spécialité). | Le bandeau suit (nom du cabinet, photo de l'utilisateur) ; [+] actif dans Thérapeute. Depuis Intervenant › loupe › un thérapeute : [+] inactif, comme avant. | — | ✅ |

### 9.3 - Étape 3 : recherches, hubs, élévation (25/10/2026 — validés par Joëlle)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| NAV-C4-11 | Recherche : par code (un PA, un DO, un SE, un TH), puis Séances, Paiements, Patients par lieu, Réseau par nom : ouvrir un élément, Retour ; « Retour à la recherche » ; Réseau : « Fiche de la personne » (contact, intervenant, thérapeute). | Mêmes écrans et onglets qu'avant ; Retour revient à la recherche avec ses critères ; fenêtres de la personne avec leurs [+]. | — | ✅ |
| NAV-C4-12 | Hub Référentiels : ouvrir chacune des 20 tuiles (au moins une par groupe) ; hub Administration : Paramètres, Utilisateurs, Cabinet, Situations juridiques, Types d'identifiant, Sauvegardes, Journaux, État du système, Connexion DB (fermer sans rien changer). | Chaque écran s'ouvre avec son fil d'Ariane (« Référentiels > … », « Administration > … ») ; Connexion DB : fil d'Ariane temporaire puis « Administration ». | — | ✅ |
| NAV-C4-13 | Compte User : menu Référentiels puis Administration → « Non » ; puis « Oui » et mot de passe faux, puis juste. | « Non » : « Accès Référentiels annulé. » / « Accès Administration annulé. » ; faux : « Élévation refusée. » ; juste : l'écran s'ouvre, le bandeau montre le rôle élevé. | Journal : « Session élevée : … » une fois par élévation. | ✅ |
| NAV-C4-14 | Hub Référentiels (et Administration) : « Élever accès » (annuler, puis accepter) ; « Retour rôle de base ». | Pendant la fenêtre : fil d'Ariane « Référentiels > Élévation d'accès » ; annulé : « Élévation annulée ou refusée. » ; accordé : boutons réactivés, « Rôle courant : … », bandeau à jour. Retour rôle de base : bandeau à jour, retour à l'accueil si le rôle de base ne suffit plus. | La fenêtre d'élévation affiche ses infobulles (contexte transmis). | ✅ |
| NAV-C4-15 | Compte User : fiche Contact › [+] lien ; fiche Intervenant › [+] thérapeute → « Non », puis « Oui ». | « Ajout de référentiel annulé. » / « Ajout de thérapeute annulé. » ; après élévation, la fenêtre s'ouvre ; le fil d'Ariane reste celui de la fiche (il passait à « Administration > Élévation d'accès » et y restait). | — | ✅ |

---

## 🎯 RÉCAPITULATIF DES TESTS

| Catégorie | Total tests | Tests OK | Tests KO | Non testés |
|-----------|-------------|----------|----------|------------|
| Section 1 : Chargement Home | 6 | - | - | - |
| Section 2 : Menu & Navigation | 18 | - | - | - |
| Section 3 : UserControlContext | 8 | - | - | - |
| Section 4 : Gestion rôles | 9 | - | - | - |
| Section 5 : Navigation inter-UC | 11 | - | - | - |
| Section 6 : Header & Status | 10 | - | - | - |
| Section 7 : Fermeture | 6 | - | - | - |
| Section 8 : Robustesse | 3 | - | - | - |
| **TOTAL** | **71** | **-** | **-** | **-** |

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
