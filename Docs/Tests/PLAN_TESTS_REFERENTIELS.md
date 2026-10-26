# 📋 PLAN DE TESTS — RÉFÉRENTIELS (UC_ReferentielBase + dérivés)

**Module** : Gestion des référentiels Althéa  
**Version** : V1.5  
**Date** : 11/06/2026 — mise à jour 23/10/2026 — spécialités des thérapeutes, v2.55.0, section 21  
**Auteur** : Joëlle (Manou) / Projet Althéa

<details>
<summary>Historique des mises à jour</summary>

- 22/10/2026 — droits et lignes système vérifiés par le métier, audit C.1 étape 3, section 20
- 20/10/2026 — latéralités, section 17 ; tuile Domaines retirée du hub ; types d'identifiant, section 18
- 18/10/2026 — colonne État en pastilles, section 16
- 16/10/2026 — panneau « Détail » à la règle des fenêtres, section 15
- 12/10/2026 — pays, civilité et mutualité dans les écrans de saisie
- 11/06/2026 — création du plan

</details>

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
| `UC_Domaines` *(supprimé le 18/10/2026 ; tests de domaine : [PLAN_TESTS_DOMAINES.md](PLAN_TESTS_DOMAINES.md))* | `ref_domaines` | **3** | ❌ Non | — |
| `UC_Organismes` | `ref_organismes` | selon modèle | ✅ Oui | Ligne système INAMI protégée |
| `UC_LiensPatient` | `ref_liens_patient` | **50** | ✅ Oui | — |
| `UC_RolesIntervenant` | `ref_roles_intervenant` | **30** | ✅ Oui | — |
| `UC_Specialites` | `ref_specialites` | **30** | ✅ Oui | — (section 21) |
| `UC_SituationsFamiliales` | `ref_situations_familiales` | **50** | ✅ Oui | — |
| `UC_StatutsDossier` | `ref_statuts_dossier` | **30** | ✅ Oui | — |
| `UC_StatutsSeance` | `ref_statuts_seance` | **30** | ✅ Oui | — |
| `UC_TypesDocuments` | `ref_types_documents` | **30** | ✅ Oui | — |
| `UC_TypesRendezVous` | `ref_types_rendez_vous` | **30** | ✅ Oui | — |
| `UC_NaturesSeance` | `ref_natures_seance` | selon modèle | ✅ Oui | — (catégorie de quota portée par la liaison nature–domaine) |
| `UC_PrixSeance` | `ref_prix_seance` | selon modèle | ✅ Oui | Montant, type tarif et activation |
| `UC_ModesPaiement` | `ref_modes_paiement` | selon modèle | ✅ Oui | — |
| `UC_SituationsJuridiques` | `ref_situations_juridiques` | **50** | ✅ Oui | Porteur du n° BCE (Cabinet / Praticien) ; lignes système protégées |
| `UC_TypesIdentifiantPro` *(hub Administration)* | `ref_types_identifiant_pro` | **30** | ✅ Oui | Porté par, masque, expression de contrôle ; INAMI, BCE, TVA lignes système (section 18) |

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

### 14.1 — UC_Domaines *(supprimé — sans objet)*

> Écran archivé le 02/10/2026 (non compilé), supprimé le 18/10/2026 (UI-8).
>
> Les cas 6.2.1, 6.3.1, 6.3.2 et 14.1.x sont **sans objet** : la gestion des domaines (dont code max 3, sans underscore) est testée par `PLAN_TESTS_DOMAINES.md`.
>
> La tuile Référentiels › Domaines est retirée le 20/10/2026 (décision de Joëlle : les Domaines s'ouvrent par le menu principal de la Home) : vérifier seulement qu'elle n'apparaît plus (17.1).

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

### 14.9 — UC_NaturesSeance

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.9.1 | Créer une nature active. | Nature visible dans les choix autorisés après liaison au domaine. | Code normalisé et unicité contrôlée. | ☐ |
| 14.9.2 | Vérifier l’absence de champ quota (code prestation, catégorie). | Aucun champ quota sur la nature ; la catégorie se choisit dans le domaine (onglet Natures). | Colonnes supprimées en v2.27.0 / v2.28.0. | ☐ |
| 14.9.3 | Désactiver une nature utilisée. | Historique conservé, nouvelle sélection exclue. | Soft-delete, aucune FK cassée. | ☐ |

### 14.10 — UC_PrixSeance

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.10.1 | Créer un prix avec montant valide. | Prix enregistré et restitué avec son type. | Montant non négatif, code unique. | ☐ |
| 14.10.2 | Désactiver un prix déjà employé. | Historique préservé ; prix exclu des nouveaux choix. | Pas de modification des séances historiques. | ☐ |

### 14.11 — UC_ModesPaiement

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.11.1 | Créer puis désactiver un mode de paiement. | Les paiements historiques l’affichent ; le mode inactif n’est plus proposé. | Conservation de l’historique. | ☐ |

### 14.12 — UC_Organismes

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.12.1 | Créer un organisme (ex. « MUTUELLE X »). | Organisme créé, proposé dans l’onglet Général des domaines. | `ref_organismes`, unicité code/libellé. | ☐ |
| 14.12.2 | Désactiver un organisme utilisé par un domaine. | Désactivation possible, domaine inchangé. | Soft-delete. | ☐ |
| 14.12.3 | Tenter de modifier le code ou supprimer l’organisme système INAMI. | Refusé (ADR-28). | `est_systeme = 1`. | ☐ |
| 14.12.4 | Sélectionner un organisme hors édition. | Périodicité, téléphones, e-mails, site, plateforme, adresse et procédure affichés en lecture seule sous les champs génériques. | Lot 29 (v2.33.0) : champs supplémentaires de `UC_Organismes`. | ☐ |
| 14.12.5 | Modifier : choisir une périodicité, saisir téléphone et e-mail, enregistrer. | Colonne « Périodicité » mise à jour dans la liste ; valeurs relues après Actualiser. | `ref_organismes.id_periodicite`, `telephone`, `email`. | ☐ |
| 14.12.6 | Saisir un e-mail `abc@` puis un téléphone contenant des lettres. | Avertissement ciblé (focus sur le champ fautif), fenêtre non fermée, aucun enregistrement. | `UtilsValidation.IsValidEmail` / `IsValidTelephone` ; champs vides acceptés. | ☐ |
| 14.12.7 | Choisir « (aucune) » comme périodicité et vider tous les champs de contact, enregistrer. | Colonnes `NULL` en base (pas de chaînes vides). | Conversion vide → NULL dans `GestionOrganismes`. | ☐ |
| 14.12.8 | Désactiver la périodicité utilisée par un organisme, revenir sur l’organisme. | L’organisme conserve sa périodicité (libellé affiché) ; la liste déroulante ne propose plus la périodicité inactive pour un autre organisme. | FK `ON DELETE SET NULL`, soft-delete. | ☐ |

### 14.13 — Lignes système et statuts consommateurs

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.13.1 | Sur tout référentiel, tenter de modifier le code d’une ligne `est_systeme`. | Code verrouillé, libellé modifiable selon règle. | ADR-28. | ☐ |
| 14.13.2 | `UC_StatutsSeance` : cocher/décocher « consomme quota ». | Valeur restituée ; décompte des quotas adapté. | `ref_statuts_seance.consomme_quota`, voir TQ-16a. | ☐ |

### 14.14 — Thérapeutes (`TherapeuteEdition`) : saisies contrôlées par pays

> Règles détaillées et cas complets : `PLAN_TESTS_PATIENTS.md`, section « Contrôles de saisie par pays » (PAT-35 à PAT-47).

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.14.1 | Pays Belgique, code postal `75001`, puis pays France. | Erreur immédiate en Belgique, levée en France ; enregistrement bloqué tant que l’erreur subsiste. | `UtilsCodePostal` (BE/FR/LU seulement). | ☐ |
| 14.14.2 | Pays Allemagne, code postal et téléphone quelconques au format libre. | Code postal non contrôlé ; téléphone contrôlé selon les règles du pays. | Autres pays toujours sélectionnables. | ☐ |

### 14.15 — UC_SituationsJuridiques *(code max 50)*

> Référentiel consommé par `UC_Cabinet` (onglet Cabinet, liste « Situation juridique »). Le champ **Porteur du n° BCE** détermine si le numéro BCE est saisi sur la fiche Cabinet ou sur la fiche Praticien (voir `PLAN_TESTS_UC_CABINET.md`, section 5).

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.15.1 | Ouvrir Administration › Situations juridiques. | Trois lignes système présentes : INDEPENDANT_PP (porteur Praticien), SOCIETE (porteur Cabinet), CABINET_GROUPE (porteur Cabinet). | Alimentation par la migration v2.31.0, `est_systeme = 1`. | ☐ |
| 14.15.2 | Sélectionner une ligne système, cliquer Modifier. | Code et case Actif verrouillés ; libellé, porteur BCE et ordre modifiables. | ADR-28. | ☐ |
| 14.15.3 | Créer « ASSOCIATION DE FAIT » avec porteur Praticien. | Code normalisé `ASSOCIATION_DE_FAIT`, ligne proposée dans la liste de `UC_Cabinet` après actualisation. | Espaces → _, `MaxLength = 50`. | ☐ |
| 14.15.4 | Cliquer Nouveau : vérifier la valeur par défaut du porteur BCE. | « Cabinet » présélectionné ; aucune situation ne peut être enregistrée sans porteur. | Liste à deux valeurs (Cabinet / Praticien), pas de valeur vide. | ☐ |
| 14.15.5 | Modifier le porteur BCE d'une situation utilisée par le cabinet, puis rouvrir `UC_Cabinet`. | Le champ BCE bascule vers le nouveau porteur ; la valeur existante est proposée au déplacement (confirmation). | Voir `PLAN_TESTS_UC_CABINET.md` 5.3. | ☐ |
| 14.15.6 | Désactiver une situation non système utilisée par le cabinet. | Désactivation possible ; `UC_Cabinet` continue d'afficher la situation courante (libellé inactif inclus). | Soft-delete, aucune FK cassée. | ☐ |

### 14.16 — UC_Periodicites *(code max 30)*

> Référentiel consommé par `UC_Organismes` (liste « Périodicité »). L’attribut porteur de comportement est `nb_mois` ; aucun comportement n’est déduit du code.

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.16.1 | Ouvrir Référentiels › Périodicités d’encodage. | Icône de titre affichée ; quatre lignes : MENSUELLE (1), TRIMESTRIELLE (3), ANNUELLE (12), A_LA_DEMANDE (—). | Alimentation par la migration v2.33.0 ; colonne « Mois ». | ☐ |
| 14.16.2 | Créer « SEMESTRIELLE » avec 6 mois. | Ligne créée, proposée dans `UC_Organismes` après actualisation. | `ref_periodicites.nb_mois = 6`. | ☐ |
| 14.16.3 | Créer une périodicité en cochant « Non périodique ». | Champ mois désactivé ; colonne « Mois » affiche « — ». | `nb_mois = NULL`. | ☐ |
| 14.16.4 | Modifier le nombre de mois d’une périodicité existante. | Valeur restituée après enregistrement et Actualiser. | `UpdatePeriodicite`. | ☐ |
| 14.16.5 | Désactiver puis réactiver une périodicité. | Soft-delete ; les organismes rattachés ne sont pas modifiés. | Voir 14.12.8. | ☐ |

### 14.17 — Migration v2.34.0 (référentiels de saisie, étape R1)

> Vérifiée le 11/10/2026 sur une copie de la base (MariaDB 10.11, collation remplacée pour le test) : application, rejeu sans effet, retour arrière puis réapplication, contraintes CHECK. À rejouer sur la base de développement (MariaDB 12).

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.17.1 | Appliquer `migration_v2.34.0_referentiels_saisie.sql`. | Aucune erreur. | `tec_meta_schema` contient 2.34.0 ; tables `ref_pays` (5 lignes système), `ref_civilites` (3 lignes). | ✅ |
| 14.17.2 | Vérifier la conversion des données réelles. | Cabinet et praticien : `id_pays` = Belgique ; praticien : `id_civilite` = Madame. | Les colonnes texte `pays` / `civilite` / `mutualite` sont inchangées. | ✅ |
| 14.17.3 | Rejouer la migration. | Aucune erreur, aucun doublon. | Script rejouable. | ✅ |

### 14.18 — UC_Pays *(code ISO, 2 lettres)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.18.1 | Ouvrir Référentiels › Pays. | Tuile et icône de titre affichées ; 5 pays avec cadenas, colonne « Contrôles » = Oui. | Ordre : Belgique, France, Luxembourg, Allemagne, Pays-Bas. | ✅ |
| 14.18.2 | Créer « CH » Suisse. | Ligne créée, sans cadenas, « Contrôles » = —. | `ref_pays.code_pays = 'CH'`. | ✅ |
| 14.18.3 | Saisir un code d'une seule lettre, ou avec un chiffre. | Erreur « 2 lettres » sur le champ Code ; pas d'enregistrement. | `ValiderChampSupplementaire`. | ✅ |
| 14.18.4 | Modifier Belgique. | Code et case Actif grisés ; libellé et ordre modifiables. | Désactiver refusé (message ligne système). | ✅ |
| 14.18.5 | Désactiver puis réactiver la Suisse. | Soft-delete puis réactivation. | — | ✅ |

### 14.19 — UC_Civilites *(code max 20)*

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.19.1 | Ouvrir Référentiels › Civilités. | Tuile et icône de titre ; Madame (Mme), Monsieur (M.), Mx (Mx). | Colonne « Abrév. ». | ✅ |
| 14.19.2 | Créer « DOCTEUR » Docteur, abréviation « Dr ». | Ligne créée et restituée. | `abreviation_civilite = 'Dr'`. | ✅ |
| 14.19.3 | Effacer l'abréviation et enregistrer. | Colonne « Abrév. » vide. | `abreviation_civilite = NULL`. | ✅ |

### 14.20 — UC_Organismes : type d'organisme (v2.34.0)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.20.1 | Ouvrir Référentiels › Organismes. | Colonne « Type » ; INAMI et organismes existants = Remboursement ; 7 mutualités (100 à 900) = Mutualité. | Libellé de la tuile : « Organismes et mutualités ». | ✅ |
| 14.20.2 | Modifier INAMI. | Liste Type grisée (ligne système) ; coordonnées modifiables. | `type_organisme` inchangé. | ✅ |
| 14.20.3 | Créer un organisme de type Mutualité puis le repasser en Remboursement. | Type restitué après Actualiser. | `type_organisme`. | ✅ |
| 14.20.4 | Domaines › onglet Général › liste Organisme. | Seuls les organismes de remboursement actifs sont proposés (plus celui déjà rattaché). | Aucune mutualité proposée. | ✅ |
| 14.20.5 | Domaines › bouton [+] organisme : créer un organisme. | Type proposé par défaut : Remboursement ; le nouvel organisme est sélectionné. | — | ✅ |

### 14.21 — Étape R2 : pays et civilité dans les écrans de saisie

> Écrans : Cabinet (cabinet et praticien), fiche Patient, ContactEdition, TherapeuteEdition, IntervenantEdition. Listes fermées alimentées par `ref_pays` / `ref_civilites` (`UtilsSaisieReferentiels`).

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.21.1 | Administration › Cabinet : ouvrir, Modifier. | Pays du cabinet = Belgique ; civilité du praticien = Madame ; pays du praticien = « (pays du cabinet) ». | Valeurs converties par la v2.34.0. | ✅ |
| 14.21.2 | Changer la civilité du praticien, enregistrer, rouvrir. | Valeur restituée ; plus de liste codée en dur. | `cab_praticiens.id_civilite`. | ✅ |
| 14.21.3 | Praticien sans pays propre : saisir un code postal belge invalide (« 12 »). | Refus : contrôle selon le pays du cabinet. | Avant R2 : aucun contrôle. | ✅ |
| 14.21.4 | Nouveau patient. | Pays proposé = pays du cabinet ; civilité « — ». | `UtilsSaisieReferentiels.ChargerPays`. | ✅ |
| 14.21.5 | Patient : choisir France, saisir 0612345678, sortir du champ. | Téléphone mis en forme +33 6 12 34 56 78 ; code postal à 5 chiffres exigé. | Règles retrouvées par le code ISO « FR ». | ✅ |
| 14.21.6 | Patient : choisir civilité et pays, enregistrer, recharger. | Valeurs restituées. | `patients.id_civilite`, `patients.id_pays`. | ✅ |
| 14.21.7 | Ajouter « CH » Suisse au référentiel, l'attribuer à un patient avec un téléphone libre. | Accepté sans contrôle de format (pays sans règle). | Repli souple. | ✅ |
| 14.21.8 | Désactiver la Suisse, rouvrir ce patient. | La Suisse reste affichée pour ce patient ; elle n'est plus proposée à un nouveau patient. | Valeur courante désactivée conservée. | ✅ |
| 14.21.9 | Contact : civilité + pays, « Copier l'adresse du patient ». | Le pays du patient est repris ; enregistrement et rechargement corrects. | `famille_contacts.id_pays`, `id_civilite`. | ✅ |
| 14.21.10 | Thérapeute : civilité + pays, enregistrer, rouvrir. | Valeurs restituées. | `therapeutes.id_civilite`, `id_pays`. | ✅ |
| 14.21.11 | Intervenant : choisir ce thérapeute. | Champ Civilité (lecture seule) repris du thérapeute, comme le nom et la spécialité. | `autres_suivis_patient.id_civilite` à l'enregistrement. | ✅ |
| 14.21.12 | Modifier ensuite la civilité du thérapeute, rouvrir l'intervenant existant. | L'intervenant garde la civilité de sa trace. | Snapshot non recalculé. | ✅ |
| 14.21.13 | Générer le cachet. | Le pays imprimé est le libellé du référentiel. | `GenerateurCachet` → `LibellePays`. | ✅ |

### 14.22 — Étape R3 : mutualité sur la fiche Patient

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.22.1 | Fiche Patient, carte Administratif. | Lignes : Mutualité (liste + loupe), Affiliation + CT1 / CT2, Situation familiale, Tarif par défaut. | Liste : « — » puis les mutualités actives. | ✅ |
| 14.22.2 | Modifier : choisir Solidaris, affiliation, CT1 110, CT2 110 ; enregistrer, recharger. | Valeurs restituées. | `patients.id_organisme_mutualite`, `numero_affiliation`, `code_titulaire_1/2`. | ✅ |
| 14.22.3 | Saisir CT1 « 11 » ou « 1a0 ». | Erreur sur le champ, enregistrement refusé. | `ValiderCodeTitulaire`. | ✅ |
| 14.22.4 | En consultation, cliquer la loupe. | Fenêtre « Mutualités » : seules les mutualités, celle du patient sélectionnée avec sa fiche. | Élévation demandée si le rôle ne suffit pas. | ✅ |
| 14.22.5 | Depuis cette fenêtre, compléter le site web de la mutualité, fermer. | Fiche mise à jour ; la mutualité du patient ne change pas. | — | ✅ |
| 14.22.6 | En saisie, loupe → Nouveau : créer une mutualité, fermer. | Type imposé « Mutualité » (liste grisée) ; la nouvelle mutualité est sélectionnée dans la fiche. | Message d'état. | ✅ |
| 14.22.7 | Désactiver la mutualité d'un patient, rouvrir ce patient. | Elle reste affichée pour lui ; elle n'est plus proposée aux autres. | Valeur courante conservée. | ✅ |
| 14.22.8 | Domaines › bouton [+] organisme. | Seuls les organismes de remboursement ; type imposé « Remboursement ». | `UC_Organismes(TypeOrganisme.Remboursement)`. | ✅ |

### 14.23 — Migration v2.35.0 (étape R4 : suppression des colonnes texte)

> Vérifiée le 12/10/2026 sur une copie de la base (MariaDB 10.11, collation remplacée pour le test) : application après la v2.34.0, rejeu sans effet, retour arrière (colonnes recréées et remplies depuis les référentiels), réapplication.

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 14.23.1 | Sauvegarder la base, puis appliquer `migration_v2.35.0_referentiels_saisie_nettoyage.sql`. | Aucune erreur. | `tec_meta_schema` contient 2.35.0 ; plus aucune colonne `pays`, `civilite`, `mutualite` dans les 5 tables. | ☐ |
| 14.23.2 | Ouvrir Cabinet, une fiche Patient (avec mutualité), un contact, un thérapeute, un intervenant ; modifier et enregistrer chacun. | Aucun message d'erreur ; valeurs restituées. | L'application n'utilise plus les colonnes supprimées. | ☐ |
| 14.23.3 | Générer le cachet. | Pays du cabinet imprimé. | — | ☐ |

---

## 📂 SECTION 15 — Panneau « Détail » (UI-7, 16/10/2026)

> `UC_ReferentielBase` V1.4.0 : libellé à gauche, champ à droite, une ligne tous les 34 px ; champs propres posés par `AjouterLigneDetail` ; État toujours en dernier ; consultation lisible.

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 15.1 | Référentiel simple (ex. Pays), sélectionner une ligne. | Carte « Détail » : Code, Libellé, Ordre, État, libellés à gauche en gras sauge, champs alignés. | Consultation : texte normal sur fond teinté, case « Actif » lisible (pas de gris pâle). | ☐ |
| 15.2 | Nouveau, puis Modifier. | Titre « Nouvel élément » / « Modification » ; champs blancs, saisie possible. | Annuler : retour à la consultation lisible. | ☐ |
| 15.3 | Modifier une ligne système (cadenas). | Code et case « Actif » restent en lecture seule (fond teinté), le reste est modifiable. | Comme avant (verrou système inchangé). | ☐ |
| 15.4 | Civilités. | Ligne « Abréviation » sous Ordre, puis État. | Saisie et enregistrement de l'abréviation. | ☐ |
| 15.5 | Modes de paiement. | Ligne « Avoir » : case « Mode avoir (remboursement / crédit) ». | Case du kit ; enregistrement. | ☐ |
| 15.6 | Organismes. | Type, Périodicité (listes du kit), coordonnées, Procédure sur deux lignes, puis État ; tout reste visible (le panneau s'allonge ou remonte ses champs si besoin). | Ligne système ou ouverture avec type imposé : Type en lecture seule en modification. | ☐ |
| 15.7 | Périodicités. | « Durée (mois) » puis « Rythme » (case « Non périodique ») ; en saisie, cocher la case passe la durée en lecture seule, la décocher la rend modifiable. | Enregistrement : durée NULL si non périodique (inchangé). | ☐ |
| 15.8 | Tarifs de séance. | « Montant (€) » sous Ordre, État en dernier ; onglet des domaines rattachés inchangé. | Plus de lignes resserrées. | ☐ |
| 15.9 | Situations juridiques. | Ligne « Porteur BCE » (liste du kit Cabinet / Praticien) ; l'infobulle explique le porteur. | Effet sur `UC_Cabinet` inchangé (5.x du plan Cabinet). | ☐ |
| 15.10 | Statuts de séance. | Lignes « Paiement » (Séance due) et « Quota » (Consomme le quota). | Enregistrement des deux drapeaux. | ☐ |

## 📂 SECTION 16 — Colonne État en pastilles (UI-9.4, 18/10/2026)

> `UC_ReferentielBase` V1.5.0, `UC_Therapeutes` V1.1.0 : la case « Actif » et le cadenas des lignes système deviennent la colonne « État ».

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 16.1 | Statuts de séance (lignes système), puis un référentiel sans ligne système (ex. Pays). | Ligne système : pastille **Système** (bleu) dans la colonne État, infobulle « Ligne système : code et activation verrouillés. » ; ligne normale : rien ; plus de cadenas ni de case « Actif » | - Ordre aligné à droite<br>- Pastilles nettes sur les lignes alternées et la ligne sélectionnée | ☐ |
| 16.2 | Cocher « Afficher les inactifs » sur un référentiel ayant une ligne désactivée. | Pastille **Inactif** (gris) ; ligne atténuée (texte gris) | - Désactiver / Réactiver met la pastille à jour | ☐ |
| 16.3 | Cliquer l'en-tête « État », puis Code, Libellé, Ordre. | Tri ascendant puis descendant (lignes normales, puis Inactif, puis Système) | - Le détail suit la ligne choisie | ☐ |
| 16.4 | Référentiels avec colonnes propres : Tarifs de séance (Montant), Organismes (Type), Pays, Civilités, Périodicités. | Colonnes propres toujours avant « État » ; la carte « Détail » reste à droite de la liste | | ☐ |
| 16.5 | Thérapeutes, avec « Afficher les inactifs ». | Colonne État : pastille **Inactif** pour un thérapeute désactivé, rien sinon ; ligne atténuée | - Tri par en-tête, double-clic et boutons inchangés | ☐ |

## 📂 SECTION 17 — Latéralités (20/10/2026, migration v2.46.0)

| # | Test | Résultat attendu | Vérifications | ✅ |
|------|------|------------------|---------------|----|
| 17.1 | Hub Référentiels. | Tuile « Latéralités » dans le groupe Patient ; Types de rendez-vous dans le groupe Séances. **Plus de tuile Domaines** (20/10/2026) : première ligne Thérapeutes, Organismes, Pays, Civilités | - Icône (flèche gauche-droite et crayon), survol et état désactivé (utilisateur simple)<br>- Domaines toujours accessibles par le menu principal | ☐ |
| 17.2 | Ouvrir Latéralités. | Droitier, Gaucher, Ambidextre (et les valeurs déjà saisies hors liste, reprises par la migration) | - Titre et icône de l'écran | ☐ |
| 17.3 | Ajouter « Gaucher contrarié », modifier un libellé, désactiver une ligne. | Comme les autres référentiels (code unique, libellé unique, pastille Inactif) | — | ☐ |

## 📂 SECTION 18 — Types d'identifiant professionnel (20/10/2026, migration v2.47.0)

> Prérequis : migration v2.47.0 appliquée. Écran ouvert depuis le hub **Administration**.

| # | Test | Résultat attendu | Vérifications | ✅ |
|------|------|------------------|---------------|----|
| 18.1 | Hub Administration en SuperUser, puis en Admin, puis en utilisateur simple. | Tuile « Types d'identifiant » à côté de « Situations juridiques », Utilisateurs en quatrième position ; active pour SuperUser et Admin, grisée sinon | - Icône (carte avec #), survol, infobulle | ☐ |
| 18.2 | Ouvrir l'écran. | Titre « Types d'identifiant », chemin « Administration > Types d'identifiant » ; INAMI, agrément Compy, BCE, TVA ; colonne « Contrôle » : Althéa pour INAMI, BCE, TVA, « — » pour l'agrément | - Cadenas (Système) sur INAMI, BCE, TVA | ☐ |
| 18.3 | Modifier BCE. | Libellé, porté par, masque et ordre modifiables ; code, état et **contrôle** en lecture seule | - Désactiver BCE : refusé (ligne système) | ☐ |
| 18.4 | Nouveau type « Numéro de membre », porté par Praticien, masque `000000`, contrôle `^[0-9]{6}$`. | Enregistré ; colonne Contrôle = Expression | - En base : `cible = PRATICIEN`, masque et expression renseignés | ☐ |
| 18.5 | Modifier ce type : contrôle `^[0-9{6}$` (crochet non fermé). | Refusé : message sous le champ Contrôle, rien n'est enregistré | — | ☐ |
| 18.6 | Vider masque et contrôle, Enregistrer. | Accepté ; en base, les deux colonnes à NULL ; Contrôle = — | — | ☐ |
| 18.7 | Fiche Cabinet › Identifiants : ajouter une ligne du nouveau type (contrôle `^[0-9]{6}$`) avec « 12A ». | Refusé, avec rappel « Format attendu : 000000. » | Voir aussi `PLAN_TESTS_UC_CABINET` 8.8 | ☐ |
| 18.8 | Désactiver le nouveau type, puis rouvrir la fiche Cabinet. | Le type n'est plus proposé pour une nouvelle ligne ; la ligne existante l'affiche toujours (cellule non vide) et s'enregistre | `PLAN_TESTS_UC_CABINET` 8.7 | ☐ |

## 📂 SECTION 19 — Correctif « Afficher les inactifs » (20/10/2026, audit B4)

| # | Test | Résultat attendu | Vérifications | ✅ |
|------|------|------------------|---------------|----|
| 19.1 | Natures de séance : cocher « Afficher les inactifs ». | La liste se recharge, inactifs compris, triée par ordre puis libellé | Avant : erreur SQL (requête incomplète) | ☐ |
| 19.2 | Statuts de séance : cocher « Afficher les inactifs », puis désactiver et réactiver une ligne non système. | Idem, sans erreur | — | ☐ |

## 📂 SECTION 20 — Droits et lignes système côté métier (22/10/2026, audit C.1 étape 3)

> Non-régression : la règle ne change pas (écrire = SuperUser), elle est maintenant vérifiée par le métier ; les lignes système sont protégées dans les requêtes.
>
> Créer des lignes **de test** (code `TEST_C1`…), les désactiver ensuite.
>
> Les refus du métier lui-même ne se provoquent pas depuis les écrans, qui grisent déjà les actions (tests automatisés, lot D).

| # | Test | Résultat attendu | Vérifications | ✅ |
|------|------|------------------|---------------|----|
| 20.1 | **SuperUser** › Statuts de séance : créer `TEST_C1`, changer son libellé, le désactiver, le réactiver. Même chose dans Pays, Modes de paiement et Types de documents. | Tout est accepté, comme avant | Journal : lignes « Référentiel '…' : création / modification … » comme avant | ✅ |
| 20.2 | **SuperUser** › Statuts de séance : ligne système « Réalisée » → Modifier : changer l'**ordre**, Enregistrer ; puis remettre l'ordre d'origine. | Accepté ; code et case Actif restent verrouillés (comme avant) | `SELECT code_statut_seance, actif FROM ref_statuts_seance WHERE est_systeme = 1;` → codes inchangés, tous actifs | ✅ |
| 20.3 | Même ligne système → **Désactiver**. | Fenêtre Althéa (plus la boîte Windows) « … est une ligne système utilisée par l'application : il ne peut pas être désactivé. » | La confirmation « Voulez-vous désactiver… » est aussi une fenêtre Althéa | ✅ |
| 20.4 | **SuperUser** › Organismes : INAMI → compléter un champ de coordonnées, Enregistrer ; puis remettre la valeur. Types d'identifiant : INAMI → changer le libellé puis le remettre. | Accepté ; type de l'organisme et code verrouillés | — | ✅ |
| 20.5 | Non-régression, écrans spécialisés (données de test) : **Domaines** (modifier un domaine de test, changer son logo), **règle de quota** de test (ajouter, désactiver), **rattachements** nature / tarif ↔ domaine (ajouter, désactiver, supprimer), **catégories de quota** (ajouter, désactiver), **thérapeute** de test (créer, modifier, désactiver), **texte type** de test, **usage** d'un texte type (libellé). | Tout est accepté, comme avant | Les panneaux de rattachement aussi depuis Natures de séance et Prix de séance | ✅ |
| 20.6 | Compte **User**, fiche d'un patient de test : bouton **[+]** à côté de Pays (ou Civilité). | Demande d'élévation (comme avant) ; après élévation, ajout possible et la valeur est proposée dans la liste | Sans élévation : rien n'est ajouté | ✅ |
| 20.7 | Compte **User** : menu **Référentiels**. | Demande d'élévation, comme avant ; sans élévation, le hub ne s'ouvre pas | — | ✅ |

## 📂 SECTION 21 — Spécialités des thérapeutes (23/10/2026, migration v2.55.0)

> Avant la migration (données de test seulement) : corriger les deux valeurs fantaisistes relevées par la requête de relevé, puis appliquer `migration_v2.55.0_specialites.sql` et lancer la requête de contrôle écrite en tête du script.

| # | Test | Résultat attendu | Vérifications | ✅ |
|------|------|------------------|---------------|----|
| 21.1 | Après la migration : requête de contrôle du script ; démarrer Althéa. | **0** ; démarrage normal (base 2.55.0). | État du système : version de la base 2.55.0, OK. | ✅ |
| 21.2 | Hub Référentiels en SuperUser. | Tuile « Spécialités / Thérapeutes » dans le groupe Cabinet, à droite de Civilités ; active (grisée pour un compte non élevé). | Icône provisoire (celle des thérapeutes). | ✅ |
| 21.3 | Ouvrir Spécialités. | Liste de départ (Assistant social … Psychothérapeute) triée par libellé, sans doublon avec les 6 spécialités reprises (Graphothérapeute, Généraliste, Neurologue, Psychiatre, Psychologue, Psychothérapeute). | Titre « Spécialités », chemin « Référentiels > Spécialités ». | ✅ |
| 21.4 | Ajouter « Orthodontiste » ; essayer d'ajouter « logopede » ; modifier un libellé ; désactiver une ligne de test. | Ajout accepté ; « logopede » refusé (libellé déjà utilisé : casse et accents ignorés) ; le reste comme les autres référentiels. | — | ✅ |

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
| 14 — Spécificités | 85 | | | |
| 15 — Panneau « Détail » (UI-7) | 10 | | | |
| **TOTAL** | **185** | | | |

---

*Plan généré le 11/06/2026 *