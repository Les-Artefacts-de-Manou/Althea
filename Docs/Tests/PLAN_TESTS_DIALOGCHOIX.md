# 📋 PLAN DE TESTS COMPLET - DIALOGCHOIX

**Module** : Boîte de dialogue personnalisée Althéa  
**Version** : V1.0  
**Date** : 07/06/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

---

## 🎯 Objectif des tests

Valider le bon fonctionnement complet du système de dialogues personnalisés `DialogChoix`, incluant :
- Affichage des 4 types de dialogues (Information, Warning, Error, Question)
- Gestion des boutons dynamiques (OK, Oui/Non, Oui/Non/Annuler, OK/Annuler)
- Affichage des icônes animées (GIF) selon le type
- Respect de la charte graphique Althéa
- Retour des résultats corrects (DialogResult)
- Centrage et positionnement
- Utilisation cohérente dans toute l'application

---

## ✅ CHECKLIST GÉNÉRALE

### Prérequis

- [x] Assets GIF présents dans `Assets\Dialog\`
- [x] DialogChoix.vb compilé sans erreur
- [x] UITheme et UtilsButtons disponibles
- [x] Application compilée sans erreur

---

## 📂 SECTION 1 : AFFICHAGE & INITIALISATION

### 1.1 - Création et affichage initial

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.1.1 | Appeler `DialogChoix.Show(...)` avec type Information | Dialogue s'affiche | - Form modale centrée<br>- Titre = titre fourni ou "Information"<br>- Icône Info animée (GIF)<br>- Bouton OK visible<br>- Couleurs UITheme appliquées | ☐ |
| 1.1.2 | Appeler avec type Warning | Dialogue Warning affiché | - Titre = "Attention" (ou custom)<br>- Icône Warning animée (GIF)<br>- Couleur accent UITheme.Warning | ☐ |
| 1.1.3 | Appeler avec type Error | Dialogue Error affiché | - Titre = "Erreur" (ou custom)<br>- Icône Error animée (GIF)<br>- Couleur accent UITheme.Error | ☐ |
| 1.1.4 | Appeler avec type Question | Dialogue Question affiché | - Titre = "Question" (ou custom)<br>- Icône Question animée (GIF)<br>- Couleurs UITheme standards | ☐ |

### 1.2 - Titre personnalisé

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.2.1 | Appeler avec paramètre titre personnalisé | Titre personnalisé affiché | - `DialogChoix.Show("Mon titre", ...)` affiche "Mon titre" | ☐ |
| 1.2.2 | Appeler sans titre (Nothing ou "") | Titre par défaut affiché | - Type Info → "Information"<br>- Type Warning → "Attention"<br>- Type Error → "Erreur"<br>- Type Question → "Question" | ☐ |

### 1.3 - Message

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.3.1 | Appeler avec message court | Message affiché | - Label message visible<br>- Texte complet affiché<br>- Police Calibri 11pt<br>- Couleur UITheme.TextPrimary | ☐ |
| 1.3.2 | Appeler avec message long (plusieurs lignes) | Message multiline affiché | - Label AutoSize ou Multiline<br>- Hauteur dialogue ajustée si nécessaire<br>- Scrollbar si texte très long | ☐ |
| 1.3.3 | Appeler avec message vide | Dialogue affiché sans message | - Espace message présent mais vide<br>- Pas d'erreur | ☐ |

---

## 📂 SECTION 2 : TYPES DE BOUTONS

### 2.1 - Type OK (par défaut)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.1.1 | Appeler avec `MessageBoxButtons.OK` | Seul bouton OK visible | - 1 bouton : "OK"<br>- Centré en bas du dialogue<br>- AcceptButton = OK<br>- CancelButton = OK | ☐ |
| 2.1.2 | Cliquer sur OK | Dialogue se ferme avec DialogResult.OK | - Form se ferme<br>- Retour = DialogResult.OK | ☐ |
| 2.1.3 | Appuyer sur Entrée | Dialogue se ferme avec OK | - AcceptButton activé<br>- Retour = DialogResult.OK | ☐ |
| 2.1.4 | Appuyer sur Échap | Dialogue se ferme avec OK | - CancelButton activé<br>- Retour = DialogResult.OK | ☐ |

### 2.2 - Type OKCancel

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.2.1 | Appeler avec `MessageBoxButtons.OKCancel` | Deux boutons visibles | - 2 boutons : "OK", "Annuler"<br>- AcceptButton = OK<br>- CancelButton = Annuler | ☐ |
| 2.2.2 | Cliquer sur OK | Dialogue se ferme avec DialogResult.OK | - Form se ferme<br>- Retour = DialogResult.OK | ☐ |
| 2.2.3 | Cliquer sur Annuler | Dialogue se ferme avec DialogResult.Cancel | - Form se ferme<br>- Retour = DialogResult.Cancel | ☐ |
| 2.2.4 | Appuyer sur Entrée | Dialogue se ferme avec OK | - AcceptButton activé<br>- Retour = DialogResult.OK | ☐ |
| 2.2.5 | Appuyer sur Échap | Dialogue se ferme avec Cancel | - CancelButton activé<br>- Retour = DialogResult.Cancel | ☐ |

### 2.3 - Type YesNo

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.3.1 | Appeler avec `MessageBoxButtons.YesNo` | Deux boutons visibles | - 2 boutons : "Oui", "Non"<br>- AcceptButton = Oui<br>- CancelButton = Non | ☐ |
| 2.3.2 | Cliquer sur Oui | Dialogue se ferme avec DialogResult.Yes | - Form se ferme<br>- Retour = DialogResult.Yes | ☐ |
| 2.3.3 | Cliquer sur Non | Dialogue se ferme avec DialogResult.No | - Form se ferme<br>- Retour = DialogResult.No | ☐ |
| 2.3.4 | Appuyer sur Entrée | Dialogue se ferme avec Yes | - AcceptButton activé<br>- Retour = DialogResult.Yes | ☐ |
| 2.3.5 | Appuyer sur Échap | Dialogue se ferme avec No | - CancelButton activé<br>- Retour = DialogResult.No | ☐ |

### 2.4 - Type YesNoCancel

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.4.1 | Appeler avec `MessageBoxButtons.YesNoCancel` | Trois boutons visibles | - 3 boutons : "Oui", "Non", "Annuler"<br>- AcceptButton = Oui<br>- CancelButton = Annuler | ☐ |
| 2.4.2 | Cliquer sur Oui | Dialogue se ferme avec DialogResult.Yes | - Form se ferme<br>- Retour = DialogResult.Yes | ☐ |
| 2.4.3 | Cliquer sur Non | Dialogue se ferme avec DialogResult.No | - Form se ferme<br>- Retour = DialogResult.No | ☐ |
| 2.4.4 | Cliquer sur Annuler | Dialogue se ferme avec DialogResult.Cancel | - Form se ferme<br>- Retour = DialogResult.Cancel | ☐ |
| 2.4.5 | Appuyer sur Entrée | Dialogue se ferme avec Yes | - AcceptButton activé<br>- Retour = DialogResult.Yes | ☐ |
| 2.4.6 | Appuyer sur Échap | Dialogue se ferme avec Cancel | - CancelButton activé<br>- Retour = DialogResult.Cancel | ☐ |

---

## 📂 SECTION 3 : ICÔNES ANIMÉES (GIF)

### 3.1 - Chargement et affichage des GIF

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.1.1 | Afficher dialogue Information | GIF Info animé | - PictureBox affiche `info.gif`<br>- Animation fluide sans acoup<br>- Taille icône cohérente (ex: 64x64) | ☐ |
| 3.1.2 | Afficher dialogue Warning | GIF Warning animé | - PictureBox affiche `warning.gif`<br>- Animation fluide | ☐ |
| 3.1.3 | Afficher dialogue Error | GIF Error animé | - PictureBox affiche `error.gif`<br>- Animation fluide | ☐ |
| 3.1.4 | Afficher dialogue Question | GIF Question animé | - PictureBox affiche `question.gif`<br>- Animation fluide | ☐ |

### 3.2 - Gestion fichiers manquants

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.2.1 | Supprimer un fichier GIF et afficher le dialogue | Dialogue affiché sans icône ou icône par défaut | - Aucune exception<br>- Message ou placeholder affiché<br>- Log erreur (optionnel) | ☐ |

---

## 📂 SECTION 4 : CHARTE GRAPHIQUE

### 4.1 - Couleurs UITheme

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.1.1 | Vérifier BackColor du dialogue | Fond = UITheme.BackgroundPrimary | - Couleur de fond correcte | ☐ |
| 4.1.2 | Vérifier ForeColor des labels | Texte = UITheme.TextPrimary | - Couleur texte correcte | ☐ |
| 4.1.3 | Vérifier couleur barre titre (si custom) | Barre titre = UITheme.AccentPrimary ou selon type | - Information/Question : AccentPrimary<br>- Warning : Warning<br>- Error : Error | ☐ |
| 4.1.4 | Vérifier style boutons | Boutons via UtilsButtons | - AppliquerStyleBouton(...) appliqué<br>- Couleurs cohérentes | ☐ |

### 4.2 - Police et tailles

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.2.1 | Vérifier police titre | Calibri 12pt Bold | - lblTitre : Calibri, 12pt, Bold | ☐ |
| 4.2.2 | Vérifier police message | Calibri 11pt Regular | - lblMessage : Calibri, 11pt, Regular | ☐ |
| 4.2.3 | Vérifier police boutons | Calibri 10pt Regular | - Tous boutons : Calibri, 10pt | ☐ |

---

## 📂 SECTION 5 : POSITIONNEMENT & LAYOUT

### 5.1 - Centrage du dialogue

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.1.1 | Afficher dialogue sans Form parent | Dialogue centré à l'écran | - StartPosition = CenterScreen<br>- Centré sur écran principal | ☐ |
| 5.1.2 | Afficher dialogue avec Form parent (owner) | Dialogue centré sur le parent | - StartPosition = CenterParent<br>- Centré sur la form appelante | ☐ |

### 5.2 - Taille du dialogue

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.2.1 | Afficher dialogue avec message court | Taille minimale cohérente | - Hauteur/largeur minimales définies<br>- Pas de dialogue trop petit | ☐ |
| 5.2.2 | Afficher dialogue avec message long | Taille ajustée | - Hauteur augmentée si nécessaire<br>- Largeur max définie<br>- Scrollbar si besoin | ☐ |

### 5.3 - Ordre de tabulation

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.3.1 | Appuyer sur Tab dans le dialogue | Ordre de focus logique | - Focus passe entre boutons<br>- Ordre : bouton principal → boutons secondaires → bouton annulation | ☐ |

---

## 📂 SECTION 6 : UTILISATION DANS L'APPLICATION

### 6.1 - Remplacement des MessageBox existants

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.1.1 | Vérifier tous les anciens MessageBox remplacés | Aucun MessageBox.Show natif restant | - Recherche code source : "MessageBox.Show"<br>- Tous remplacés par DialogChoix | ☐ |

### 6.2 - Exemples d'utilisation

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.2.1 | UC_Utilisateurs : confirmation suppression | DialogChoix Question avec Oui/Non | - `DialogChoix.Show("Voulez-vous vraiment supprimer...", TypeDialogChoix.Question, MessageBoxButtons.YesNo)`<br>- Retour traité correctement | ☐ |
| 6.2.2 | UtilisateurEdition : succès enregistrement | DialogChoix Information avec OK | - `DialogChoix.Show("Utilisateur enregistré avec succès.", TypeDialogChoix.Information)`<br>- Dialogue se ferme sur OK | ☐ |
| 6.2.3 | Login : erreur authentification | DialogChoix Error avec OK | - `DialogChoix.Show("Identifiants incorrects.", TypeDialogChoix.Error)`<br>- Icône erreur animée<br>- Couleur rouge | ☐ |
| 6.2.4 | UC_Parametres : avertissement modification | DialogChoix Warning avec OK/Annuler | - `DialogChoix.Show("Cette modification peut affecter...", TypeDialogChoix.Warning, MessageBoxButtons.OKCancel)`<br>- Retour = OK ou Cancel | ☐ |

### 6.3 - Cohérence des messages

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.3.1 | Vérifier messages clairs et professionnels | Messages cohérents | - Pas de messages techniques bruts<br>- Français correct<br>- Aide à l'action (ex: "Contactez un administrateur") | ☐ |
| 6.3.2 | Vérifier pas de doublons de dialogues | Pas de cascade de dialogues | - Un seul dialogue à la fois<br>- Pas de "OK" puis "Confirmation" redondant | ☐ |

---

## 📂 SECTION 7 : ROBUSTESSE & LIMITES

### 7.1 - Gestion des erreurs

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.1.1 | Appeler DialogChoix avec paramètres invalides | Gestion gracieuse | - Aucune exception<br>- Valeurs par défaut utilisées | ☐ |
| 7.1.2 | Appeler DialogChoix depuis plusieurs threads (simulation) | Un seul dialogue à la fois | - Mutex ou lock si nécessaire<br>- Pas de conflit UI | ☐ |

### 7.2 - Performance

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.2.1 | Afficher 10 dialogues successifs | Pas de ralentissement | - GIF animés sans lag<br>- Chargement rapide<br>- Mémoire libérée après fermeture | ☐ |

---

## 📂 SECTION 8 : ACCESSIBILITÉ & UX

### 8.1 - Navigation clavier

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.1.1 | Utiliser uniquement le clavier | Dialogue entièrement utilisable | - Tab pour naviguer<br>- Entrée pour valider<br>- Échap pour annuler | ☐ |

### 8.2 - Lecture d'écran (optionnel)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.2.1 | Utiliser un lecteur d'écran (NVDA, JAWS) | Dialogue lisible | - Titre annoncé<br>- Message annoncé<br>- Boutons annoncés | ☐ |

---

## 🎯 RÉCAPITULATIF DES TESTS

| Catégorie | Total tests | Tests OK | Tests KO | Non testés |
|-----------|-------------|----------|----------|------------|
| Section 1 : Affichage | 11 | - | - | - |
| Section 2 : Boutons | 19 | - | - | - |
| Section 3 : Icônes animées | 5 | - | - | - |
| Section 4 : Charte graphique | 7 | - | - | - |
| Section 5 : Positionnement | 5 | - | - | - |
| Section 6 : Utilisation | 8 | - | - | - |
| Section 7 : Robustesse | 3 | - | - | - |
| Section 8 : Accessibilité | 2 | - | - | - |
| **TOTAL** | **60** | **-** | **-** | **-** |

---

## 📝 BUGS IDENTIFIÉS PENDANT LES TESTS

| # | Bug | Priorité | Statut |
|---|-----|----------|--------|
| - | - | - | - |

---

## ✅ VALIDATION FINALE

- [ ] Tous les tests de Section 1 à 8 passés
- [ ] Aucun bug bloquant restant
- [ ] Tous les MessageBox natifs remplacés
- [ ] GIF animés fluides sans lag
- [ ] Charte graphique Althéa respectée partout
- [ ] Navigation clavier complète
- [ ] Documentation mise à jour si nécessaire

---

> 
>
> ------
>
> > **Contact** : ***Joëlle (Manou) - Les Artefacts de Manou***
> >
> > Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.
> >
> > - Site web P.Nguyen Duy: https://pearlnguyenduy.be/
> > - mailto: `joelle@nguyen.eu`
> > - GitHub privé: Althea https://github.com/AngeljoNG/Althea
> > - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea
>
> ------
>

[TOC]

