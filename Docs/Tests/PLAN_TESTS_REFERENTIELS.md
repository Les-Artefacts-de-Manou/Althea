# 📋 PLAN DE TESTS — RÉFÉRENTIELS (UC_ReferentielBase + 9 dérivés)

**Module** : Gestion des référentiels Althéa  
**Version** : V1.0  
**Date** : 11/06/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

---

## 🎯 Objectif des tests

Valider le bon fonctionnement de tous les écrans de référentiel, qui partagent la même base
`UC_ReferentielBase`, en couvrant :
- Affichage, chargement et filtrage de la liste
- Modes Consultation / Création / Modification et leurs transitions
- Validation des champs (obligatoires, longueur, unicité, normalisation du code)
- Activation / désactivation (soft-delete)
- Gestion des droits (SuperUser requis)
- Journalisation (GestionLog)
- Spécificités propres à chaque référentiel

---

## 📦 Référentiels couverts

| UC | Table | Code max | Espaces → _ | Champ supplémentaire |
|----|-------|:---------:|:-----------:|----------------------|
| `UC_Domaines` | `ref_domaines` | **3** | ❌ Non | — |
| `UC_LiensPatient` | `ref_liens_patient` | **50** | ✅ Oui | — |
| `UC_RolesIntervenant` | `ref_roles_intervenant` | **30** | ✅ Oui | — |
| `UC_SituationsFamiliales` | `ref_situations_familiales` | **50** | ✅ Oui | — |
| `UC_StatutsDossier` | `ref_statuts_dossier` | **30** | ✅ Oui | — |
| `UC_StatutsSeance` | `ref_statuts_seance` | **30** | ✅ Oui | — |
| `UC_TypesDocuments` | `ref_types_documents` | **30** | ✅ Oui | — |
| `UC_TypesRendezVous` | `ref_types_rendez_vous` | **30** | ✅ Oui | — |
| `UC_TypesSeance` | `ref_types_seance` | **30** | ✅ Oui | `tarif_defaut` (décimal 10,2) |

---

## ✅ CHECKLIST GÉNÉRALE

### Prérequis

- [ ] Application compilée sans erreur
- [ ] Base de données accessible (MySQL / MySqlConnector)
- [ ] Tables `ref_*` présentes et alimentées avec au moins 3 éléments actifs et 1 inactif
- [ ] Compte SuperUser disponible pour les tests de modification
- [ ] Compte utilisateur simple (rôle < SuperUser) disponible pour les tests de droits
- [ ] Navigation vers `UC_ReferentielHome` fonctionnelle
- [ ] GestionLog initialisé

---

## 📂 SECTION 1 — Accès et affichage initial

### 1.1 — Navigation et en-tête

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 1.1.1 | Naviguer vers chaque référentiel depuis `UC_ReferentielHome` | L'UC s'affiche | - Aucune exception<br>- Titre correct (ex. "Domaines", "Liens patient"…) | ☐ |
| 1.1.2 | Vérifier le fil d'Ariane de chaque UC | Fil d'Ariane correct | - "Référentiels > Domaines"<br>- "Référentiels > Liens patient"<br>- "Référentiels > Rôles d'intervenant"<br>- "Référentiels > Situations familiales"<br>- "Référentiels > Statuts de dossier"<br>- "Référentiels > Statuts de séance"<br>- "Référentiels > Types de document"<br>- "Référentiels > Types de rendez-vous"<br>- "Référentiels > Types de séance" | ☐ |
| 1.1.3 | Vérifier le sous-titre de chaque UC | Sous-titre descriptif affiché | - Texte non vide, cohérent avec le référentiel | ☐ |
| 1.1.4 | Vérifier le titre du panneau d'édition à l'ouverture | Affiche "Détail" | - `lblTitreEdition.Text = "Détail"` | ☐ |

### 1.2 — État initial des contrôles

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 1.2.1 | À l'ouverture, aucune ligne sélectionnée | Panneau d'édition vide | - txtCode, txtLibelle vides<br>- numOrdre = 0<br>- chkActif coché | ☐ |
| 1.2.2 | Boutons Modifier et Activer/Désactiver à l'ouverture | Désactivés (aucune sélection) | - `btnModifier.Enabled = False`<br>- `btnActiverDesactiver.Enabled = False` | ☐ |
| 1.2.3 | Boutons Enregistrer et Annuler à l'ouverture | Désactivés | - Uniquement actifs en mode édition | ☐ |
| 1.2.4 | Boutons Nouveau et Actualiser à l'ouverture | Activés | - `btnNouveau.Enabled = True`<br>- `btnActualiser.Enabled = True` | ☐ |

---

## 📂 SECTION 2 — Chargement de la liste

### 2.1 — Données et grille

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 2.1.1 | Ouvrir chaque UC avec des données en base | Liste remplie | - La grille affiche les éléments actifs<br>- Colonnes : Id, Code, Libellé, Ordre, Actif visibles | ☐ |
| 2.1.2 | Vérifier la barre de statut après chargement | Affiche le nombre d'éléments | - "N élément(s) affiché(s)." | ☐ |
| 2.1.3 | Ouvrir un UC avec table vide | Grille vide, pas d'exception | - Message statut : "0 élément(s) affiché(s)." | ☐ |
| 2.1.4 | Vérifier que les éléments inactifs sont masqués par défaut | Inactifs non visibles | - `chkAfficherInactifs` décoché par défaut | ☐ |

### 2.2 — Affichage des inactifs

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 2.2.1 | Cocher "Afficher les inactifs" | La liste recharge et inclut les inactifs | - Rechargement immédiat<br>- Éléments inactifs visibles dans la grille | ☐ |
| 2.2.2 | Décocher "Afficher les inactifs" | La liste recharge sans les inactifs | - Éléments inactifs retirés | ☐ |
| 2.2.3 | Cocher/décocher inactifs pendant une édition | Impossible | - La checkbox est désactivée pendant les modes Création/Modification | ☐ |

---

## 📂 SECTION 3 — Recherche et filtrage

### 3.1 — Filtre en mémoire

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 3.1.1 | Saisir un texte présent dans un code | Liste filtrée sur le code | - Seules les lignes contenant le texte dans le code sont affichées | ☐ |
| 3.1.2 | Saisir un texte présent dans un libellé | Liste filtrée sur le libellé | - Seules les lignes contenant le texte dans le libellé sont affichées | ☐ |
| 3.1.3 | Vérifier l'insensibilité à la casse | Filtre indépendant de la casse | - Saisir "dom", "DOM", "Dom" retourne les mêmes résultats | ☐ |
| 3.1.4 | Vider le champ de recherche | Liste complète rétablie | - Tous les éléments actifs de nouveau visibles | ☐ |
| 3.1.5 | Saisir un texte inexistant | Grille vide | - "0 élément(s) affiché(s)." dans la barre de statut | ☐ |
| 3.1.6 | Le filtre ne déclenche pas de rechargement BDD | Filtrage purement en mémoire | - Aucun appel réseau lors de la frappe | ☐ |
| 3.1.7 | Tenter de filtrer pendant une édition | Champ de recherche désactivé | - `txtRecherche.Enabled = False` en mode Création/Modification | ☐ |

---

## 📂 SECTION 4 — Sélection et affichage du détail

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 4.1 | Cliquer sur une ligne active | Détail affiché dans le panneau | - txtCode, txtLibelle, numOrdre, chkActif remplis | ☐ |
| 4.2 | Cliquer sur une ligne inactive | Détail affiché, chkActif décoché | - Cohérent avec l'état BDD | ☐ |
| 4.3 | Changer de sélection | Détail mis à jour | - Panneau reflète la nouvelle ligne | ☐ |
| 4.4 | Cliquer hors de toute ligne | Panneau vidé | - txtCode, txtLibelle vides | ☐ |
| 4.5 | Sélection → bouton Modifier s'active | `btnModifier.Enabled = True` | - Uniquement si `PeutModifier()` est vrai | ☐ |
| 4.6 | Sélection ligne active → libellé bouton "Désactiver" | Texte = "Désactiver" | - `btnActiverDesactiver.Text = "Désactiver"` | ☐ |
| 4.7 | Sélection ligne inactive → libellé bouton "Réactiver" | Texte = "Réactiver" | - `btnActiverDesactiver.Text = "Réactiver"` | ☐ |

---

## 📂 SECTION 5 — Mode Création

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 5.1 | Cliquer sur "Nouveau" | Mode Création activé | - Panneau vidé<br>- `lblTitreEdition.Text = "Nouvel élément"` | ☐ |
| 5.2 | Focus automatique après clic "Nouveau" | Focus sur txtCode | - Curseur dans txtCode | ☐ |
| 5.3 | En mode Création, contrôles d'édition activés | txtCode, txtLibelle, numOrdre, chkActif saisissables | - Panneau d'édition interactif | ☐ |
| 5.4 | En mode Création, grille désactivée | `dgvReferentiel.Enabled = False` | - Impossible de changer la sélection | ☐ |
| 5.5 | En mode Création, boutons Nouveau/Modifier/ActDés/Actualiser désactivés | Ces 4 boutons grisés | - Seuls Enregistrer et Annuler sont actifs | ☐ |
| 5.6 | En mode Création, chkActif coché par défaut | Nouvel élément actif par défaut | - `chkActif.Checked = True` | ☐ |

---

## 📂 SECTION 6 — Validation des champs

### 6.1 — Champ Code : contraintes communes

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 6.1.1 | Laisser le code vide et enregistrer | Erreur affichée | - ErrorProvider sur txtCode : "Le code est obligatoire." | ☐ |
| 6.1.2 | Saisir un code existant (même UC) et enregistrer | Erreur doublon | - ErrorProvider sur txtCode : "Ce code est déjà utilisé." | ☐ |
| 6.1.3 | Saisir en minuscules | Code converti en majuscules automatiquement | - `CharacterCasing.Upper` actif | ☐ |
| 6.1.4 | Saisir des caractères spéciaux (!, @, #…) | Caractères refusés à la frappe | - `txtCode_KeyPress` filtre (seuls lettres/chiffres autorisés) | ☐ |
| 6.1.5 | Coller un texte avec caractères spéciaux | Caractères illégaux retirés | - `txtCode_TextChanged` nettoie le texte | ☐ |

### 6.2 — Champ Code : longueur max (par UC)

| # | UC | Test | Résultat attendu | ✅ |
|---|----|------|------------------|----|
| 6.2.1 | `UC_Domaines` | Saisir plus de 3 caractères | Bloqué à 3 (`MaxLength = 3`) | ☐ |
| 6.2.2 | `UC_LiensPatient` | Saisir plus de 50 caractères | Bloqué à 50 | ☐ |
| 6.2.3 | `UC_SituationsFamiliales` | Saisir plus de 50 caractères | Bloqué à 50 | ☐ |
| 6.2.4 | Autres UC (×6) | Saisir plus de 30 caractères | Bloqué à 30 | ☐ |

### 6.3 — Normalisation du code (espaces → underscore)

| # | UC | Test | Résultat attendu | ✅ |
|---|----|------|------------------|----|
| 6.3.1 | `UC_Domaines` | Saisir un espace | Espace refusé à la frappe | ☐ |
| 6.3.2 | `UC_Domaines` | Coller "AB C" | Le code résultant est "ABC" (espace retiré, pas de `_`) | ☐ |
| 6.3.3 | Tous les autres UC | Saisir "MOT CLEF" | Code normalisé en "MOT_CLEF" en temps réel | ☐ |
| 6.3.4 | Tous les autres UC | Coller "MOT CLEF" | Code normalisé en "MOT_CLEF" | ☐ |
| 6.3.5 | Tous les autres UC | Saisir "A__B" | Underscores conservés | ☐ |

### 6.4 — Champ Libellé

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 6.4.1 | Laisser le libellé vide et enregistrer | Erreur affichée | - ErrorProvider sur txtLibelle : "Le libellé est obligatoire." | ☐ |
| 6.4.2 | Saisir un libellé existant (même UC) et enregistrer | Erreur doublon | - ErrorProvider sur txtLibelle : "Ce libellé est déjà utilisé." | ☐ |
| 6.4.3 | Modifier un élément en conservant son propre libellé | Enregistrement accepté | - L'unicité exclut l'élément lui-même (`idExclu`) | ☐ |

### 6.5 — Champ Ordre

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 6.5.1 | Laisser l'ordre à 0 | Enregistrement accepté | - Ordre 0 est valide | ☐ |
| 6.5.2 | Saisir un ordre positif | Enregistrement accepté | - Valeur respectée en BDD | ☐ |

### 6.6 — Erreurs multiples simultanées

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 6.6.1 | Code vide ET libellé vide → Enregistrer | Les deux erreurs affichées | - ErrorProvider sur les deux champs simultanément | ☐ |
| 6.6.2 | Corriger le code → Enregistrer | Seule l'erreur libellé reste | - L'erreur code disparaît | ☐ |

---

## 📂 SECTION 7 — Mode Modification

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 7.1 | Sélectionner un élément puis cliquer "Modifier" | Mode Modification activé | - `lblTitreEdition.Text = "Modification"`<br>- Champs remplis avec les valeurs de l'élément | ☐ |
| 7.2 | Focus automatique en mode Modification | Focus sur txtLibelle | - Le code est rarement modifié | ☐ |
| 7.3 | En mode Modification, les mêmes contrôles que Création sont actifs | txtCode, txtLibelle, numOrdre, chkActif éditables | - Cohérent avec mode Création | ☐ |
| 7.4 | Modifier un code par un code déjà pris par un autre élément | Erreur unicité | - ErrorProvider code | ☐ |
| 7.5 | Modifier le libellé par le libellé d'un autre élément | Erreur unicité | - ErrorProvider libellé | ☐ |
| 7.6 | Conserver le même code sur l'élément en cours | Enregistrement accepté | - L'`idExclu` s'applique correctement | ☐ |
| 7.7 | Conserver le même libellé sur l'élément en cours | Enregistrement accepté | - L'`idExclu` s'applique correctement | ☐ |

---

## 📂 SECTION 8 — Annulation

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 8.1 | Cliquer "Annuler" en mode Création | Retour mode Consultation | - Panneau retrouve les valeurs de la sélection courante (ou vide si aucune) | ☐ |
| 8.2 | Cliquer "Annuler" en mode Modification | Retour mode Consultation avec données d'origine | - Aucune modification enregistrée | ☐ |
| 8.3 | Après annulation, la grille est réactivée | `dgvReferentiel.Enabled = True` | - Navigation dans la liste possible | ☐ |
| 8.4 | Après annulation, les erreurs sont effacées | ErrorProvider vide | - Aucun badge rouge visible | ☐ |

---

## 📂 SECTION 9 — Enregistrement

### 9.1 — Création

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 9.1.1 | Saisir un code et libellé valides → Enregistrer | Élément créé en BDD | - Liste rechargée<br>- Nouvel élément visible<br>- Statut : "Enregistrement effectué." | ☐ |
| 9.1.2 | Après création, mode revient en Consultation | `lblTitreEdition.Text = "Détail"` | - Boutons Nouveau/Actualiser réactivés | ☐ |
| 9.1.3 | Vérifier la journalisation de la création | GestionLog enregistre l'action | - Log : "Référentiel 'X' : création de l'élément 'CODE'." | ☐ |

### 9.2 — Modification

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 9.2.1 | Modifier libellé/ordre/actif → Enregistrer | Mise à jour en BDD | - Liste rechargée<br>- Valeur modifiée visible<br>- Statut : "Enregistrement effectué." | ☐ |
| 9.2.2 | Vérifier la journalisation de la modification | GestionLog enregistre l'action | - Log : "Référentiel 'X' : modification de l'élément Id=N ('CODE')." | ☐ |

### 9.3 — Gestion d'erreur BDD

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 9.3.1 | Simuler une perte de connexion BDD lors de l'enregistrement | Message d'erreur clair | - MessageBox "Impossible d'enregistrer l'élément."<br>- Exception tracée dans GestionLog | ☐ |

---

## 📂 SECTION 10 — Activer / Désactiver

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 10.1 | Sélectionner un élément actif → cliquer "Désactiver" | Boîte de confirmation affichée | - MessageBox : "Voulez-vous désactiver l'élément « Libellé » ?" | ☐ |
| 10.2 | Confirmer la désactivation | Élément désactivé en BDD | - Liste rechargée<br>- Élément masqué si `chkAfficherInactifs` décoché | ☐ |
| 10.3 | Annuler la confirmation | Aucun changement | - Liste inchangée | ☐ |
| 10.4 | Cocher "Afficher inactifs" puis sélectionner l'élément désactivé → cliquer "Réactiver" | Boîte de confirmation affichée | - MessageBox : "Voulez-vous réactiver l'élément « Libellé » ?" | ☐ |
| 10.5 | Confirmer la réactivation | Élément réactivé en BDD | - Élément de nouveau actif<br>- Liste rechargée | ☐ |
| 10.6 | Vérifier la journalisation de l'activation/désactivation | GestionLog enregistre l'action | - Log : "Référentiel 'X' : élément Id=N désactiver/réactiver." | ☐ |
| 10.7 | Simuler une erreur BDD lors de l'activation | Message d'erreur clair | - MessageBox "Impossible de désactiver/réactiver l'élément." | ☐ |

---

## 📂 SECTION 11 — Actualiser

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 11.1 | Cliquer "Actualiser" | Liste rechargée depuis la BDD | - Compteur mis à jour dans la barre de statut | ☐ |
| 11.2 | Modifier un élément en base externe puis actualiser | Modification reflétée dans la grille | - Cohérence BDD/IHM | ☐ |
| 11.3 | Actualiser pendant une édition | Impossible | - `btnActualiser.Enabled = False` en mode Création/Modification | ☐ |

---

## 📂 SECTION 12 — Droits d'accès

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 12.1 | Ouvrir un UC avec un compte de rôle < SuperUser | Boutons de modification désactivés | - `btnNouveau`, `btnModifier`, `btnActiverDesactiver` grisés | ☐ |
| 12.2 | Barre de statut avec droits insuffisants | Message explicite | - "Consultation seule : droits insuffisants pour modifier ce référentiel." | ☐ |
| 12.3 | Ouvrir avec un compte SuperUser | Tous les boutons actifs | - Création, modification, activation disponibles | ☐ |
| 12.4 | Tentative de modification directe sans droit (appel programmatique) | `PeutModifier()` bloque | - Aucune action exécutée | ☐ |

---

## 📂 SECTION 13 — Journalisation (GestionLog)

| # | Test | Action déclenchante | Log attendu | ✅ |
|---|------|---------------------|-------------|----|
| 13.1 | Chargement liste | Ouverture UC / Actualiser | "Référentiel 'X' chargé : N élément(s)." (Succinct, UI) | ☐ |
| 13.2 | Création | Enregistrer en mode Création | "Référentiel 'X' : création de l'élément 'CODE'." (Succinct, Process) | ☐ |
| 13.3 | Modification | Enregistrer en mode Modification | "Référentiel 'X' : modification de l'élément Id=N ('CODE')." (Succinct, Process) | ☐ |
| 13.4 | Désactivation | Confirmer désactivation | "Référentiel 'X' : élément Id=N désactiver." (Succinct, Process) | ☐ |
| 13.5 | Réactivation | Confirmer réactivation | "Référentiel 'X' : élément Id=N réactiver." (Succinct, Process) | ☐ |
| 13.6 | Erreur chargement | Exception lors de `ChargerElements` | Exception tracée (Complet, UI) | ☐ |
| 13.7 | Erreur enregistrement | Exception lors de l'INSERT/UPDATE | Exception tracée (Complet, Process) | ☐ |

---

## 📂 SECTION 14 — Spécificités par référentiel

### 14.1 — UC_Domaines *(code max 3 — pas d'underscore)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.1.1 | Saisir "AB" (2 car.) → Enregistrer | Accepté | - Code à 2 caractères valide | ☐ |
| 14.1.2 | Saisir "ABCD" (4 car.) | Bloqué à 3 caractères | - `MaxLength = 3` | ☐ |
| 14.1.3 | Saisir un espace dans le code | Refusé à la frappe | - `RemplacerEspacesParUnderscore = False` → espace non autorisé | ☐ |
| 14.1.4 | Coller "A B" | Résultat = "AB" (espace supprimé, pas de _) | - Nettoyage dans `txtCode_TextChanged` | ☐ |
| 14.1.5 | Tooltip du champ code | "Code en majuscules (3 caractères ; lettres ou chiffres)." | - Pas de mention underscore | ☐ |

### 14.2 — UC_LiensPatient *(code max 50)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.2.1 | Saisir un code de 50 caractères | Accepté | - Limite respectée | ☐ |
| 14.2.2 | Saisir un code de 51 caractères | Bloqué à 50 | - `MaxLength = 50` | ☐ |
| 14.2.3 | Saisir "LIEN PROCHE" | Code normalisé "LIEN_PROCHE" | - Espace → underscore | ☐ |

### 14.3 — UC_RolesIntervenant *(code max 30)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.3.1 | Créer un rôle "Médecin traitant" | Code "MEDECIN_TRAITANT" (normalisé) | - Espaces → _ | ☐ |
| 14.3.2 | Saisir un code de 31 caractères | Bloqué à 30 | - `MaxLength = 30` | ☐ |

### 14.4 — UC_SituationsFamiliales *(code max 50)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.4.1 | Créer "EN COUPLE" | Code "EN_COUPLE" | - Espaces → _ | ☐ |
| 14.4.2 | Saisir un code de 51 caractères | Bloqué à 50 | - `MaxLength = 50` | ☐ |

### 14.5 — UC_StatutsDossier *(code max 30)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.5.1 | Créer "EN COURS" | Code "EN_COURS" | - Espaces → _ | ☐ |
| 14.5.2 | Désactiver un statut utilisé par des dossiers existants | Désactivation possible (soft-delete) | - Aucune contrainte FK bloquante côté UI | ☐ |

### 14.6 — UC_StatutsSeance *(code max 30)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.6.1 | Créer "NON REALISE" | Code "NON_REALISE" | - Espaces → _ | ☐ |
| 14.6.2 | Désactiver un statut | Désactivation possible | - Soft-delete appliqué | ☐ |

### 14.7 — UC_TypesDocuments *(code max 30)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.7.1 | Créer "COMPTE RENDU" | Code "COMPTE_RENDU" | - Espaces → _ | ☐ |
| 14.7.2 | Saisir un code de 31 caractères | Bloqué à 30 | - `MaxLength = 30` | ☐ |

### 14.8 — UC_TypesRendezVous *(code max 30)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.8.1 | Créer "PREMIER CONTACT" | Code "PREMIER_CONTACT" | - Espaces → _ | ☐ |
| 14.8.2 | Saisir un code de 31 caractères | Bloqué à 30 | - `MaxLength = 30` | ☐ |

### 14.9 — UC_TypesSeance *(code max 30 + champ `tarif_defaut`)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.9.1 | Ouvrir UC_TypesSeance | Champ `tarif_defaut` (NumericUpDown) visible dans le panneau d'édition | - Libellé "Tarif par défaut (€)" présent | ☐ |
| 14.9.2 | En mode Consultation, le champ tarif est en lecture seule | `_numTarif.Enabled = False` | - Non éditable hors mode édition | ☐ |
| 14.9.3 | Cliquer "Nouveau" | Le champ tarif est activé et initialisé à 0,00 | - `ViderChampSupplementaire()` appelé | ☐ |
| 14.9.4 | Saisir un tarif négatif | Refusé ou bloqué | - `ValiderChampSupplementaire()` retourne False | ☐ |
| 14.9.5 | Saisir un tarif valide (ex. 55,00) → Enregistrer | Tarif sauvegardé en BDD | - `decimal(10,2)` respecté<br>- Valeur visible à la sélection | ☐ |
| 14.9.6 | Modifier un type de séance, changer le tarif | Tarif mis à jour en BDD | - `MettreAJourElement` transmet le tarif | ☐ |
| 14.9.7 | Sélectionner un type de séance existant | Tarif affiché dans le champ | - `AfficherChampSupplementaire()` renseigne `_numTarif` | ☐ |
| 14.9.8 | Annuler une création avec tarif saisi | Tarif vidé, retour consultation | - `ViderChampSupplementaire()` appelé à l'annulation | ☐ |
| 14.9.9 | Saisir un tarif avec décimales (ex. 47,50) | Arrondi à 2 décimales | - Format décimal respecté | ☐ |

---

## 📊 Récapitulatif

| Section | Nb tests | ✅ OK | ❌ KO | ⚠️ Partiel |
|---------|:--------:|:-----:|:-----:|:----------:|
| 1 — Accès et affichage | 8 | | | |
| 2 — Chargement | 7 | | | |
| 3 — Recherche | 7 | | | |
| 4 — Sélection / Détail | 7 | | | |
| 5 — Mode Création | 6 | | | |
| 6 — Validation champs | 17 | | | |
| 7 — Mode Modification | 7 | | | |
| 8 — Annulation | 4 | | | |
| 9 — Enregistrement | 6 | | | |
| 10 — Activer/Désactiver | 7 | | | |
| 11 — Actualiser | 3 | | | |
| 12 — Droits | 4 | | | |
| 13 — Journalisation | 7 | | | |
| 14 — Spécificités (×9) | 26 | | | |
| **TOTAL** | **116** | | | |

---

*Plan généré le 11/06/2026 *