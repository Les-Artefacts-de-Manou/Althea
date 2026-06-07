# 📋 PLAN DE TESTS COMPLET - UC_PARAMETRES

**Module** : Gestion des paramètres applicatifs  
**Version** : V1.0  
**Date** : 07/06/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

---

## 🎯 Objectif des tests

Valider le bon fonctionnement complet du module `UC_Parametres`, incluant :
- Affichage et organisation par groupes (onglets dynamiques)
- Création de nouveaux paramètres
- Modification de paramètres existants
- Désactivation/réactivation de paramètres
- Validation des types et valeurs
- Gestion des droits Admin/SuperUser
- Cohérence UI et traçabilité

---

## ✅ CHECKLIST GÉNÉRALE

### Prérequis

- [x] Base de données MariaDB opérationnelle
- [x] Table `tec_parametres` créée avec données de test
- [x] Au moins 1 utilisateur Admin pour tests complets
- [x] Au moins 1 utilisateur SuperUser pour tests droits
- [x] Logs activés et fichier de log accessible
- [x] Application compilée sans erreur

---

## 📂 SECTION 1 : CHARGEMENT & AFFICHAGE

### 1.1 - Chargement initial du UserControl

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.1.1 | Ouvrir `UC_Parametres` depuis menu Paramètres | Le UC se charge sans erreur | - Header = "Paramètres > [Premier groupe]"<br>- Status = "[N] paramètre(s) chargé(s)."<br>- TabControl créé dynamiquement<br>- Au moins 1 onglet visible | ☐ |
| 1.1.2 | Vérifier la structure UI | Interface construite dynamiquement | - TabControl présent<br>- Chaque onglet = un groupe<br>- DataGridView dans chaque onglet<br>- Panel détail à droite | ☐ |
| 1.1.3 | Vérifier les tooltips | Tooltips présents | - Survoler boutons, checkboxes<br>- Textes cohérents | ☐ |

### 1.2 - Organisation par groupes (onglets)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.2.1 | Compter les onglets créés | Un onglet par groupe distinct en base | - Nombre d'onglets = nombre de groupes<br>- Noms onglets = noms groupes | ☐ |
| 1.2.2 | Cliquer sur chaque onglet | Affichage des paramètres du groupe | - DataGridView affiche paramètres du groupe<br>- Header mis à jour avec nom groupe<br>- Sélection automatique premier paramètre | ☐ |
| 1.2.3 | Vérifier tri des onglets | Onglets triés par ordre alphabétique | - Ordre cohérent | ☐ |

### 1.3 - Affichage DataGridView

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.3.1 | Vérifier les colonnes affichées | Colonnes État, Libellé, Valeur visibles | - colEtat : icône OK/OFF<br>- colLibelle : texte<br>- colValeur : texte formaté selon type | ☐ |
| 1.3.2 | Vérifier icônes d'état | Icônes correctes selon paramètre actif/inactif | - Actif : icône verte (OK_32x32)<br>- Inactif : icône grise (OFF_32x32)<br>- Icônes via UtilsIcons | ☐ |
| 1.3.3 | Vérifier format d'affichage valeurs | Valeurs affichées selon type | - BOOL : "Oui" ou "Non"<br>- INT/DECIMAL : nombre<br>- DATE : dd/MM/yyyy<br>- STRING/PATH : texte | ☐ |

### 1.4 - Affichage du détail

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.4.1 | Sélectionner un paramètre dans la grille | Panneau détail affiché à droite | - Tous les champs présents<br>- Valeurs pré-remplies<br>- Contrôle de saisie adapté au type | ☐ |
| 1.4.2 | Vérifier champs en mode consultation | Champs lecture seule si mode consultation | - Admin : tous champs éditables<br>- SuperUser : seul champ Valeur éditable | ☐ |

---

## 📂 SECTION 2 : CRÉATION DE PARAMÈTRE (Admin uniquement)

### 2.1 - Ouverture mode création

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.1.1 | Cliquer sur `btnNouveau` | Panneau détail passe en mode création | - Titre = "Nouveau paramètre"<br>- Header = "Paramètres > [Groupe] > Nouveau"<br>- Tous champs vides sauf valeurs par défaut<br>- `txtCle` : éditable<br>- `cboGroupe` : actif<br>- `cboType` : actif | ☐ |
| 2.1.2 | Vérifier état boutons | Boutons adaptés au mode | - `btnEnregistrer` : Enabled<br>- `btnAnnuler` : Enabled<br>- `btnNouveau` : Disabled<br>- `btnModifier` : Disabled | ☐ |

### 2.2 - Validation formulaire (cas d'erreur)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.2.1 | Cliquer sur `Enregistrer` sans remplir les champs | Erreur de validation | - ErrorProvider ou MessageBox<br>- Champs obligatoires signalés | ☐ |
| 2.2.2 | Saisir clé vide | Erreur validation | - Message : "La clé est obligatoire."<br>- Focus sur txtCle | ☐ |
| 2.2.3 | Saisir clé déjà existante | Échec création | - MessageBox : "La clé existe déjà."<br>- Log erreur<br>- Focus sur txtCle | ☐ |
| 2.2.4 | Saisir libellé vide | Erreur validation | - Message : "Le libellé est obligatoire." | ☐ |
| 2.2.5 | Saisir groupe vide | Erreur validation | - Message : "Le groupe est obligatoire." | ☐ |
| 2.2.6 | Saisir valeur incompatible avec le type | Erreur validation | - Exemple : type INT, valeur "abc"<br>- Message : "Valeur invalide pour le type [type]" | ☐ |

### 2.3 - Création réussie

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.3.1 | Remplir tous les champs correctement et cliquer sur `Enregistrer` | Paramètre créé | - MessageBox succès<br>- Rechargement complet UC<br>- Nouvel onglet créé si nouveau groupe<br>- Paramètre visible dans la grille<br>- Sélection sur nouveau paramètre | ☐ |
| 2.3.2 | Vérifier le log | Log créé | - Log : "Paramètre créé : clé=[clé], groupe=[groupe], type=[type]"<br>- Catégorie : Application<br>- Niveau : Succinct | ☐ |
| 2.3.3 | Vérifier en base de données | Paramètre créé | - Enregistrement présent dans `tec_parametres`<br>- `cle_parametre` normalisée (MAJUSCULE, sans accents)<br>- `groupe_parametre` normalisé<br>- `actif` = 1<br>- `date_creation` = maintenant | ☐ |

### 2.4 - Création avec nouveau groupe

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.4.1 | Saisir un nouveau nom de groupe dans cboGroupe | Groupe créé à la volée | - Nouveau groupe proposé dans liste<br>- Groupe normalisé automatiquement<br>- Après enregistrement : nouvel onglet créé | ☐ |

### 2.5 - Annulation création

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.5.1 | Cliquer sur `Annuler` | Retour mode consultation | - Panneau détail revient au paramètre précédemment sélectionné<br>- Aucune création en base<br>- Aucun log | ☐ |

---

## 📂 SECTION 3 : MODIFICATION DE PARAMÈTRE

### 3.1 - Mode Admin : modification complète

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.1.1 | Sélectionner un paramètre et cliquer sur `btnModifier` | Passage en mode modification | - Titre = "Modification paramètre"<br>- Header = "Paramètres > [Groupe] > Modification"<br>- `txtCle` : lecture seule (non modifiable)<br>- Autres champs : éditables | ☐ |
| 3.1.2 | Modifier le libellé et enregistrer | Modification réussie | - MessageBox succès<br>- Rechargement<br>- Modification visible dans la grille | ☐ |
| 3.1.3 | Modifier la valeur et enregistrer | Modification réussie | - Validation selon type<br>- MessageBox succès<br>- Log créé | ☐ |
| 3.1.4 | Modifier le groupe | Paramètre déplacé vers autre onglet | - Rechargement complet<br>- Paramètre visible dans le nouvel onglet<br>- Ancien onglet supprimé si plus de paramètres | ☐ |
| 3.1.5 | Modifier le type | Modification du type et réinitialisation valeur | - Valeur vidée ou adaptée<br>- Contrôle de saisie changé selon nouveau type | ☐ |

### 3.2 - Mode SuperUser : modification limitée

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.2.1 | Se connecter en tant que SuperUser, ouvrir paramètres | Boutons Nouveau/Modifier désactivés | - `btnNouveau` : Disabled<br>- `btnModifier` : Disabled<br>- Sélection paramètre : seul champ Valeur éditable<br>- Bouton Enregistrer actif pour modification valeur uniquement | ☐ |
| 3.2.2 | Modifier uniquement la valeur d'un paramètre | Modification réussie | - Champs Clé, Libellé, Groupe, Type : lecture seule<br>- Champ Valeur : éditable<br>- MessageBox succès<br>- Log créé | ☐ |
| 3.2.3 | Tenter de créer un paramètre | Bouton désactivé | - `btnNouveau` reste Disabled | ☐ |

### 3.3 - Validation modification

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.3.1 | Vider un champ obligatoire | Erreur validation | - ErrorProvider sur champ<br>- MessageBox explicatif | ☐ |
| 3.3.2 | Saisir valeur incompatible avec type | Erreur validation | - Message : "Valeur invalide pour le type [type]"<br>- Focus sur champ valeur | ☐ |

### 3.4 - Log de modification

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.4.1 | Vérifier le log après modification | Log créé | - Log : "Paramètre modifié : clé=[clé], ancienne valeur=[old], nouvelle valeur=[new]"<br>- Catégorie : Application<br>- Niveau : Succinct | ☐ |

### 3.5 - Annulation modification

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.5.1 | Modifier des valeurs puis cliquer sur `Annuler` | Retour mode consultation sans sauvegarde | - Valeurs originales restaurées<br>- Aucune modification en base<br>- Aucun log | ☐ |

---

## 📂 SECTION 4 : DÉSACTIVATION / RÉACTIVATION

### 4.1 - Désactivation d'un paramètre (Admin uniquement)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.1.1 | Sélectionner un paramètre actif et cliquer sur `btnDesactiver` | Demande de confirmation | - MessageBox : "Voulez-vous vraiment désactiver ce paramètre ?"<br>- Explication que le paramètre reste en base | ☐ |
| 4.1.2 | Confirmer la désactivation | Paramètre désactivé | - MessageBox succès<br>- Icône devient OFF (grise)<br>- Paramètre reste visible dans la grille | ☐ |
| 4.1.3 | Vérifier le log | Log créé | - Log : "Paramètre désactivé : clé=[clé]"<br>- Catégorie : Application<br>- Niveau : Succinct | ☐ |
| 4.1.4 | Vérifier en base de données | Champ actif mis à jour | - `actif` = 0<br>- `date_modification` = maintenant | ☐ |

### 4.2 - Réactivation d'un paramètre (Admin uniquement)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.2.1 | Sélectionner un paramètre inactif et cliquer sur `btnActiver` | Demande de confirmation | - MessageBox : "Voulez-vous vraiment activer ce paramètre ?" | ☐ |
| 4.2.2 | Confirmer la réactivation | Paramètre activé | - MessageBox succès<br>- Icône devient OK (verte)<br>- Paramètre actif dans la grille | ☐ |
| 4.2.3 | Vérifier le log | Log créé | - Log : "Paramètre activé : clé=[clé]"<br>- Catégorie : Application<br>- Niveau : Succinct | ☐ |
| 4.2.4 | Vérifier en base de données | Champ actif mis à jour | - `actif` = 1<br>- `date_modification` = maintenant | ☐ |

### 4.3 - Filtrage par état

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.3.1 | Cocher "Afficher les paramètres inactifs" | Affichage des inactifs | - Grille inclut paramètres avec `actif` = 0<br>- Icônes grises visibles | ☐ |
| 4.3.2 | Décocher "Afficher les paramètres inactifs" | Masquage des inactifs | - Grille n'affiche que `actif` = 1<br>- Status mis à jour | ☐ |

---

## 📂 SECTION 5 : TYPES DE PARAMÈTRES

### 5.1 - Type BOOL

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.1.1 | Sélectionner un paramètre de type BOOL | Contrôle CheckBox affiché | - CheckBox dans panneau détail<br>- Coché si valeur = "1" ou "True"<br>- Décoché si valeur = "0" ou "False" | ☐ |
| 5.1.2 | Modifier la valeur et enregistrer | Sauvegarde OK | - Valeur en base = "1" ou "0"<br>- Affichage grille = "Oui" ou "Non" | ☐ |

### 5.2 - Type INT

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.2.1 | Sélectionner un paramètre de type INT | Contrôle NumericUpDown affiché | - NumericUpDown dans panneau détail<br>- DecimalPlaces = 0<br>- Valeur chargée | ☐ |
| 5.2.2 | Saisir une valeur décimale | Erreur validation ou arrondi | - Validation refuse les décimaux<br>- Ou arrondi automatique | ☐ |
| 5.2.3 | Saisir une valeur texte | Impossible | - NumericUpDown n'accepte que des nombres | ☐ |

### 5.3 - Type DECIMAL

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.3.1 | Sélectionner un paramètre de type DECIMAL | Contrôle NumericUpDown affiché | - DecimalPlaces = 2 (ou configurable)<br>- Valeur chargée avec décimales | ☐ |
| 5.3.2 | Saisir une valeur avec décimales | Sauvegarde OK | - Valeur en base avec décimales<br>- Format d'affichage cohérent | ☐ |

### 5.4 - Type DATE

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.4.1 | Sélectionner un paramètre de type DATE | Contrôle DateTimePicker affiché | - DateTimePicker dans panneau détail<br>- Format dd/MM/yyyy<br>- Valeur chargée | ☐ |
| 5.4.2 | Modifier la date et enregistrer | Sauvegarde OK | - Valeur en base au format ISO<br>- Affichage grille dd/MM/yyyy | ☐ |

### 5.5 - Type STRING

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.5.1 | Sélectionner un paramètre de type STRING | Contrôle TextBox affiché | - TextBox dans panneau détail<br>- Valeur chargée<br>- Multiline si valeur longue | ☐ |
| 5.5.2 | Saisir un texte long | Sauvegarde OK | - Texte complet enregistré<br>- Affichage grille tronqué si nécessaire | ☐ |

### 5.6 - Type PATH

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.6.1 | Sélectionner un paramètre de type PATH | Contrôle TextBox + bouton parcourir (optionnel) | - TextBox dans panneau détail<br>- Valeur chargée | ☐ |
| 5.6.2 | Saisir un chemin valide | Sauvegarde OK | - Chemin enregistré<br>- Validation chemin (optionnel) | ☐ |

---

## 📂 SECTION 6 : RECHERCHE & NAVIGATION

### 6.1 - Recherche rapide

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.1.1 | Saisir du texte dans un champ de recherche (si implémenté) | Filtrage en temps réel | - Grille affiche uniquement paramètres correspondants<br>- Recherche sur clé, libellé, valeur | ☐ |

### 6.2 - Navigation clavier

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.2.1 | Utiliser flèches haut/bas dans la grille | Sélection suit le clavier | - Paramètre sélectionné change<br>- Panneau détail mis à jour | ☐ |
| 6.2.2 | Appuyer sur Tab dans les champs | Navigation séquentielle | - Focus passe d'un champ à l'autre<br>- Ordre logique | ☐ |

---

## 📂 SECTION 7 : ROBUSTESSE & LIMITES

### 7.1 - Gestion erreurs DB

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.1.1 | Simuler une erreur DB (connexion coupée) | Erreur capturée et affichée | - MessageBox : "Erreur de connexion..."<br>- Log erreur technique<br>- UC reste fonctionnel après reconnexion | ☐ |

### 7.2 - Paramètres orphelins

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.2.1 | Supprimer manuellement un groupe en base (simulation) | Rechargement sans erreur | - Onglet supprimé<br>- Aucune exception<br>- Log si nécessaire | ☐ |

### 7.3 - Valeurs extrêmes

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.3.1 | Saisir valeur INT max (2147483647) | Sauvegarde OK | - Valeur acceptée<br>- Affichage correct | ☐ |
| 7.3.2 | Saisir valeur DECIMAL très grande | Sauvegarde OK ou refus | - Validation cohérente<br>- Message clair si refus | ☐ |
| 7.3.3 | Saisir STRING très long (1000+ caractères) | Sauvegarde OK ou refus | - Limite définie<br>- Message clair si dépassement | ☐ |

---

## 📂 SECTION 8 : INTERFACE & UX

### 8.1 - Cohérence visuelle

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.1.1 | Vérifier respect charte graphique | UI cohérente | - Couleurs UITheme<br>- Boutons via UtilsButtons<br>- Police Calibri<br>- Icônes OK/OFF | ☐ |
| 8.1.2 | Vérifier messages utilisateur | Messages clairs et professionnels | - Pas de messages techniques bruts<br>- Français correct<br>- Aide à l'action | ☐ |

### 8.2 - Responsive

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.2.1 | Redimensionner la fenêtre Home | UC s'adapte | - Pas de dépassement<br>- Scrollbars si nécessaire<br>- Layout cohérent | ☐ |

---

## 🎯 RÉCAPITULATIF DES TESTS

| Catégorie | Total tests | Tests OK | Tests KO | Non testés |
|-----------|-------------|----------|----------|------------|
| Section 1 : Chargement | 11 | - | - | - |
| Section 2 : Création | 11 | - | - | - |
| Section 3 : Modification | 14 | - | - | - |
| Section 4 : Désactivation | 9 | - | - | - |
| Section 5 : Types | 16 | - | - | - |
| Section 6 : Recherche | 3 | - | - | - |
| Section 7 : Robustesse | 6 | - | - | - |
| Section 8 : Interface | 3 | - | - | - |
| **TOTAL** | **73** | **-** | **-** | **-** |

---

## 📝 BUGS IDENTIFIÉS PENDANT LES TESTS

| # | Bug | Priorité | Statut |
|---|-----|----------|--------|
| - | - | - | - |

---

## ✅ VALIDATION FINALE

- [ ] Tous les tests de Section 1 à 8 passés
- [ ] Aucun bug bloquant restant
- [ ] Logs vérifiés (niveau approprié, sans données sensibles)
- [ ] Base de données cohérente après tous les tests
- [ ] Droits Admin/SuperUser validés
- [ ] Documentation mise à jour si nécessaire

---

> **Contact** : ***Joëlle (Manou) - Les Artefacts de Manou***
> 
> Projet Althéa - Tests UC_Parametres
