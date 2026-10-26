# 💶 Althéa — Plan de conception du module **Paiements & Tarification**

> **Type** : Document de conception fonctionnelle et technique (feuille de route d'un lot)
> **Date de rédaction** : 15/09/2026 — **révisé le 15/09/2026** (cahier des charges Pearl)
> **Statut** : 🟢 Cahier des charges Pearl intégré — arbitrages actés, prêt à démarrer les migrations
> **Auteur** : Joëlle (Manou) — assisté
> **Portée** : Lot 2 (suite) — briques **6 (Paiements)** et **7 (Tarification)**
> **Sources de vérité** : `Docs/Conception/Plan_Conception_Metier_Althea.md`, `Docs/Database/Database_technique.md`, `Docs/Rules/ARCHITECTURE_DECISIONS.md`, `Docs/Todo/ETAT_DU_PROJET.md`

---

## 0. Comment lire ce document

Ce document fige le **périmètre et la découpe** des briques Paiements et Tarification **avant** d'écrire du code, conformément à la règle d'or du projet (*réfléchir avant de coder, valider étape par étape*).

> ⚠️ **Révision majeure** : la première version supposait une tarification par `(domaine × type de séance)`. Le cahier des charges transmis par **Pearl** a invalidé ce modèle. La tarification réelle dépend du **patient**, du **domaine** et du **statut** de la séance. Ce document reflète le modèle corrigé.

---

## 1. Point de départ

- La brique **Séances** (Lot 2 — brique 5) est terminée : `UC_DossierFiche` possède un onglet **Séances** opérationnel (liste `dgvSeances`, CRUD via `SeanceEditor`, notes/évolution RTF).
- La grille des séances **calcule déjà** le *montant réglé* et le *reste dû* par séance (`SeanceListeItem.MontantRegle` / `ResteDu` / `EstSoldee`), agrégés depuis `lia_paiements_seances`.
- **Mais** il n'existe encore **aucun moyen de créer un paiement** : ces montants restent à zéro. C'est l'objet de la brique 6.
- La brique 7 porte sur la **tarification réelle** telle que décrite par Pearl (voir §2).

---

## 2. Cahier des charges Pearl (règles métier réelles)

### 2.1 Le tarif dépend du **patient**, pas du type de séance

- Certains **anciens patients sont à 50 €**, d'autres à **55 €**, les **nouveaux à 60 €**.
- Ce tarif est **unique par patient**, valable pour les domaines **Psychologie, Graphothérapie et TSA**.
- Autrement dit, deux patients avec le même type de séance dans le même domaine peuvent payer des montants différents selon leur historique.

### 2.2 Le domaine **Realism** = circuit **INAMI** (cas spécial)

- Les patients du domaine **Realism** **ne paient pas directement**.
- **Pearl facture l'INAMI 91,13 €** par séance ; l'INAMI **la rembourse en fin de mois**.
- Dans Althéa : la séance Realism reste **non payée** jusqu'à réception du versement INAMI, encaissé via un **mode de paiement « INAMI »**.
- **Un No-show en Realism n'est PAS facturé** (l'INAMI ne rembourse pas une séance non honorée).
- Besoin futur : **extraire, une fois par mois et à la demande, la liste des séances Realism à facturer à l'INAMI, par patient**.

### 2.3 Le « dû » est déduit du **statut** de la séance

- Le fait qu'une séance soit **due ou non** découle de son **statut** (`ref_statuts_seance`), pas d'un champ « nature » séparé.
- Un **No-show** (annulation tardive) est **dû au tarif plein** — **sauf en Realism** (non facturé).
- Il faut pouvoir **ajouter des statuts à la volée** (le choix doit rester clair pour l'utilisatrice).

### 2.4 Un tarif est un **instantané figé**

- Le prix appliqué à une séance est **lu dans la grille tarifaire au moment de la création**, puis **détaché** : c'est la valeur stockée dans `seances.tarif_seance`.
- Si les tarifs changent l'année suivante, on modifie la grille, **mais les séances déjà comptées conservent leur ancien prix**. Vrai pour **tous** les tarifs (patient, INAMI, no-show).

---

## 3. Modèle de données corrigé

### 3.1 Constats du schéma existant

| Constat | Preuve dans le schéma | Conséquence |
|---|---|---|
| Tables paiements présentes | `paiements` (`code_paiement` auto `PAIxxxxxx`, `date_paiement`, `montant`, `mode_paiement`, `reference_paiement`, `commentaire`) | Aucune migration nécessaire sur `paiements` |
| Liaison N-N présente | `lia_paiements_seances` : `montant_affecte` + unicité `(id_paiement, id_seance)` | 1 paiement ↔ 1 séance en V1 (D-PAY-1), N-N réservé pour plus tard |
| Snapshot tarif déjà prévu | `seances.tarif_seance decimal(10,2) NOT NULL DEFAULT 0.00` | Support natif du tarif figé (§2.4) |
| Mode de paiement libre | `paiements.mode_paiement varchar(50)` | Fiabilisé par un référentiel `ref_modes_paiement` |
| Statut de séance déjà référentiel | `ref_statuts_seance` (`code_statut_seance`, `libelle`, `actif`, `ordre`) | Support du « dû » par statut (§2.3) — ajout d'un flag `est_du` |
| Domaine déjà référentiel | `ref_domaines` (`code_domaine`, `libelle_domaine`, `actif`, `ordre`) | Ajout d'un flag `est_inami` pour isoler Realism |

### 3.2 Nouvelles tables / colonnes (migrations v2.10.x)

**a) `ref_modes_paiement`** *(nouvelle table)* — modes d'encaissement, dont « INAMI ».

| Champ | Type | Rôle |
|---|---|---|
| `id_mode_paiement` | PK auto | Identifiant |
| `code_mode_paiement` | `varchar(30)` UQ | Ex. `ESPECES`, `VIREMENT`, `INAMI` |
| `libelle_mode_paiement` | `varchar(100)` UQ | Ex. « Espèces », « Virement », « INAMI » |
| `actif` | `tinyint(1)` | Activation |
| `ordre_affichage` | `int` | Tri |

> Seed initial : `ESPECES`, `VIREMENT`, `INAMI` (ajoutables à la volée via `[+]`).

**b) `ref_prix_seance`** *(nouvelle table — grille tarifaire modifiable)* — source des montants proposés (patient, no-show, INAMI…).

| Champ | Type | Rôle |
|---|---|---|
| `id_prix_seance` | PK auto | Identifiant |
| `code_prix_seance` | `varchar(30)` UQ | Ex. `PATIENT_50`, `PATIENT_55`, `PATIENT_60`, `INAMI` |
| `libelle_prix_seance` | `varchar(100)` UQ | Ex. « Tarif patient 50 € », « Séance INAMI » |
| `montant` | `decimal(10,2)` | Montant courant (modifiable) |
| `actif` | `tinyint(1)` | Activation |
| `ordre_affichage` | `int` | Tri |

> Seed provisoire : `PATIENT_50=50`, `PATIENT_55=55`, `PATIENT_60=60`, `INAMI=91,13`.
> Modifier ce montant **n'affecte pas** les séances déjà créées (snapshot §2.4).

**c) Rattachement tarif ⇄ dossier** *(colonne `dossiers.id_prix_seance` — Option A actée, cf. T-1)* — un dossier étant ouvert sous un domaine donné pour un patient, le tarif du patient y est porté naturellement.

| Champ ajouté | Type | Rôle |
|---|---|---|
| `dossiers.id_prix_seance` | `bigint(20) unsigned` NULL, FK `ref_prix_seance` | Tarif patient applicable aux séances de ce dossier (NULL en Realism / repli) |

**d) `ref_domaines.est_inami`** *(nouvelle colonne `tinyint(1)` DEFAULT 0)* — vrai pour **Realism**. Pilote le circuit INAMI (pas de paiement patient, no-show non facturé, extraction mensuelle).

**e) `ref_statuts_seance.est_du`** *(nouvelle colonne `tinyint(1)` DEFAULT 1)* — indique si une séance dans ce statut est **due**. Ex. `PLANIFIEE`, `HONOREE`, `NO_SHOW` ⇒ dû ; `ANNULEE_A_TEMPS`, `ANNULEE_PRATICIEN` ⇒ non dû. Ajoutable à la volée avec ce flag.

---

## 4. ✅ Points techniques tranchés (15/09/2026)

| Réf | Question | Décision actée |
|---|---|---|
| T-1 | Rattachement du tarif patient | **Option A — colonne `id_prix_seance` sur `dossiers`** : un dossier est ouvert sous un domaine donné, le tarif y est naturel |
| T-2 | Montant No-show | **Tarif plein du patient** (même `id_prix_seance` que la séance normale) ; seul le **statut** change |
| T-3 | Couleur de l'état No-show dû échu | **Violet** 🟣 (distinct du rouge « dû » classique) |

---

## 5. Brique 6 — Paiements

### 5.1 Couche métier (pattern éprouvé Séances)

| Fichier | Rôle |
|---|---|
| `Metier/Paiements/Paiement.vb` | DTO complet (identité + montant + mode + référence + commentaire + séance liée) |
| `Metier/Paiements/PaiementListeItem.vb` | Vue allégée pour la grille de l'onglet Paiements |
| `Core/Database/Queries/QueryPaiements.vb` | SQL : SELECT par dossier, SELECT par id, INSERT (+ liaison), UPDATE, DELETE |
| `Metier/Paiements/GestionPaiements.vb` | Service métier : CRUD + affectation `lia_paiements_seances` |

**Règles de persistance :**
- Création : insert `paiements`, récupération id via `LASTVAL(seq_paiements)`, puis insert `lia_paiements_seances` avec `montant_affecte = montant` (D-PAY-1).
- Suppression : suppression de la liaison puis du paiement.
- Le solde **n'est jamais stocké**, toujours recalculé (règle projet).
- **INAMI** est un mode de paiement comme un autre : encaisser une séance Realism = créer un paiement `mode = INAMI` à réception du versement.

### 5.2 UI — `PaiementEditor` (modale, calquée sur `SeanceEditor`)

| Contrôle | Rôle |
|---|---|
| `dtpDatePaiement` | Date du paiement (défaut : aujourd'hui) |
| `numMontant` | Montant encaissé |
| `cboModePaiement` + `btnAjouterModePaiement` `[+]` | Mode de paiement (`ref_modes_paiement`, INAMI inclus) |
| `txtReference` | Référence libre (n° virement, décompte INAMI…) |
| `txtCommentaire` | Commentaire |
| `cboSeance` (ou séance pré-sélectionnée) | Séance réglée |
| `btnEnregistrer` / `btnFermer` | Actions |

- Lecture seule forcée si dossier archivé.
- Séance pré-remplie si ouverte depuis une séance sélectionnée.

### 5.3 UI — Onglet **Paiements** dans `UC_DossierFiche`

- 5ᵉ onglet « Paiements » (pattern onglet Séances) : grille `dgvPaiements` (Code, Date, Montant, Mode, Référence, Séance, Commentaire), boutons Nouveau / Modifier / Supprimer + double-clic.
- Verrouillage complet si dossier archivé.

---

## 6. Brique 6 (suite) — Visualisation financière dans la grille Séances

### 6.1 Colonne « État paiement » (pastille couleur)

| Pastille | Signification | Condition |
|---|---|---|
| 🟢 Vert | Soldée | `MontantRegle >= TarifSeance` (tarif > 0) |
| 🟠 Orange | Partiellement réglée | `0 < MontantRegle < TarifSeance` |
| 🔴 Rouge | Due et échue | statut `est_du`, séance échue, `ResteDu > 0` |
| 🟣 Violet | **No-show dû** | statut No-show (dû) échu impayé — couleur tranchée (T-3) |
| 🔵 Bleu | À venir, non encore due | statut `est_du`, séance à venir, `ResteDu > 0` |
| ⚪ Neutre | Non due | statut `est_du = 0` (annulée à temps) **ou** séance Realism non honorée |

### 6.2 Colonne « Échéance »

| Valeur | Condition |
|---|---|
| `À venir` | `DateSeance > aujourd'hui` |
| `Aujourd'hui` | `DateSeance = aujourd'hui` |
| `Échue` | `DateSeance < aujourd'hui` |

### 6.3 Impact code

- `SeanceListeItem` : helpers `EstEchue`, `EstDuJour`, `EstAVenir`, `EstDue` (depuis `est_du`), `EstInami` (depuis `ref_domaines.est_inami`), `EtatPaiement` (enum), `LibelleEcheance`.
- `QuerySeances` : joindre `est_du` (statut) et `est_inami` (domaine) au SELECT liste.
- Couleur portée par la logique métier ; affichage (pastille image / cellule colorée) côté grille.

---

## 7. Brique 7 — Tarification

### 7.1 Référentiel « Prix de séance » — `UC_PrixSeance`

- Sur `UC_ReferentielBase` : liste + édition des lignes `ref_prix_seance` (libellé + montant modifiable + actif).
- Ajout à la volée via `[+]`.
- Modifier un montant **ne réécrit jamais** les séances passées (snapshot).

### 7.2 Résolution du tarif à la création d'une séance

Dans `SeanceEditor.AppliquerTarifDefaut()` :

1. Récupérer le **domaine** du dossier.
2. Si `domaine.est_inami` (Realism) → proposer le tarif **INAMI** (`ref_prix_seance` code `INAMI`).
3. Sinon → proposer le **tarif du patient** rattaché (Option A/B, cf. T-1).
4. Selon le **statut** choisi : si statut non dû (`est_du = 0`) → tarif 0 ou informatif ; si No-show → tarif plein patient (sauf Realism : non facturé → 0).
5. **Repli** sur `ref_types_seance.tarif_defaut` si aucun tarif patient n'est renseigné (compatibilité ascendante).
6. Le montant proposé reste **modifiable manuellement** (cas particulier / prix d'ami).

### 7.3 Extraction mensuelle INAMI *(brique 7 — livrable différé)*

- Fonction : lister, pour un mois donné, toutes les séances **Realism** honorées **non encore encaissées INAMI**, groupées par patient, avec total à facturer.
- Déclenchée à la demande de l'utilisatrice (bouton dédié, sortie document).
- **Non prioritaire** : peut être livrée après le socle Paiements.

---

## 8. Découpe d'implémentation (ordre indicatif)

1. **Migrations SQL** : `ref_modes_paiement` (+seed), `ref_prix_seance` (+seed), `ref_domaines.est_inami`, `ref_statuts_seance.est_du`, rattachement tarif patient (Option retenue), + rollbacks.
2. **Référentiels** : `UC_ModesPaiement`, `UC_PrixSeance` (Query + Modèle + Gestion + UC).
3. **Métier Paiements** : `Paiement`, `PaiementListeItem`, `QueryPaiements`, `GestionPaiements`.
4. **UI Paiements** : `PaiementEditor` + onglet Paiements dans `UC_DossierFiche`.
5. **Grille Séances** : helpers `SeanceListeItem` (échéance/état/dû/inami) + colonnes pastille & échéance + jointures `QuerySeances`.
6. **Tarification** : résolution patient/INAMI/statut branchée dans `SeanceEditor`, repli sûr.
7. **Extraction mensuelle INAMI** (différable).
8. **Build + tests** brique par brique.
9. **Documentation** : README, CHANGELOG, ETAT_DU_PROJET, Documentation_technique_UI, Database_technique.

---

## 9. Ce que ce lot ne fait PAS (hors périmètre V1)

- Ventilation d'**un** paiement sur **plusieurs** séances via l'UI (schéma prêt, reporté — D-PAY-1).
- Génération de reçus / factures PDF patients (relève de la brique Documents).
- Rapprochement bancaire automatique.
- Gestion d'un historique de tarifs versionné dans la grille (le snapshot sur la séance suffit à préserver l'historique).

---

## 10. Récapitulatif des décisions

| Réf | Décision |
|---|---|
| D-PAY-1 | Un paiement ↔ une séance en V1 (schéma N-N conservé pour l'avenir) |
| D-PAY-2 | Onglet Paiements dans `UC_DossierFiche` |
| D-PAY-3 | Mode de paiement = référentiel `ref_modes_paiement` (INAMI inclus), ajout `[+]` |
| D-TAR-1 | Tarif porté par le **patient** (50/55/60 €), via référentiel `ref_prix_seance` |
| D-TAR-2 | **Realism = INAMI** : flag `ref_domaines.est_inami`, séance non payée jusqu'au versement INAMI, no-show non facturé |
| D-TAR-3 | « Dû » déterminé par le **statut** (`ref_statuts_seance.est_du`), statuts ajoutables à la volée |
| D-TAR-4 | `seances.tarif_seance` = **snapshot figé**, insensible aux changements ultérieurs de grille |
| D-TAR-5 | Extraction mensuelle INAMI par patient (livrable différé) |

---

> **Contact** : ***Joëlle (Manou) — Les Artefacts de Manou***
>
> Modèle validé selon le cahier des charges Pearl (15/09/2026). Points techniques T-1/T-2/T-3 (§4) à confirmer avant migrations.
