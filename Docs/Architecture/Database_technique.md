# Architecture technique de la base de données

> Dernière mise à jour : 26/10/2026 — base de test `althea_tests` (test de fumée des requêtes, ADR-58)

<details>
<summary>Historique des mises à jour</summary>

- 25/10/2026 — v2.58.0 : lignes système devenues inutiles déverrouillées
- 25/10/2026 — v2.58.0 : `est_systeme` levé sur les statuts `ABSENTE`, `ANNULEE_PATIENT`, `ANNULEE_PRATICIEN`, `MALADE`, `NO_SHOW`, le lien `MERE` et le rôle `AUTORITE_PARENTALE` (audit C.3 ; données seulement)
- 24/10/2026 — v2.57.0 : `ref_statuts_seance.est_non_tenue` et `propose_creation`, `ref_liens_patient.par_defaut`, `ref_role_legal.par_defaut` (audit C.3, étape 2)
- 24/10/2026 — v2.56.0 : référentiel `ref_codes_postaux_be` (codes postaux belges et localités acceptées) et sa procédure de mise à jour
- 23/10/2026 — contrôle d'ensemble au schéma v2.55.0 : remarques des civilités replacées, groupes de paramètres livrés, lecteurs de `v_seances_solde`
- 23/10/2026 — v2.55.0 : référentiel `ref_specialites`, `therapeutes.id_specialite`
- 23/10/2026 — v2.54.0 : `patients.niss` normalisé
- 22/10/2026 — lignes système : condition dans les requêtes, audit C.1 ; pas de migration
- 22/10/2026 — v2.53.0 : `ref_statuts_seance.est_absence`
- 22/10/2026 — v2.52.0 : table `lia_paiements_avoirs`
- 22/10/2026 — v2.51.0 : colonnes de l'annulation d'un encaissement
- 21/10/2026 — v2.50.0 : `paiements.id_mode_paiement`
- 21/10/2026 — v2.49.0 : vues `v_seances_solde` et `v_dossier_dominant`
- 20/10/2026 — v2.48.0 : paramètre `SECURITE_ELEVATION_MINUTES`
- 20/10/2026 — v2.47.0 : types d'identifiant `BCE` et `TVA` en lignes système
- 20/10/2026 — v2.46.0 : référentiel `ref_lateralites`, `patients.id_lateralite` ; groupe `DOSSIERS`, `DOSSIER_MODALITE_PAIEMENT_DEFAUT`
- 20/10/2026 — v2.45.0 : logo des domaines, `ref_domaines.logo_fichier`
- 20/10/2026 — v2.44.0 : groupe `EDITEUR`, réglage du correcteur d'orthographe
- 19/10/2026 — v2.43.0 : plusieurs usages par texte type, `lia_textes_types_usages` ; v2.42.0 : textes types `ref_textes_types`, `ref_usages_textes_types` ; v2.41.0 : groupe `DOCUMENTS`, mention de confidentialité des documents
- 18/10/2026 — anciens diagrammes supprimés
- 16/10/2026 — schéma contrôlé sur les migrations jusqu’à v2.40.0 (groupes des paramètres, UI-7)

</details>

## Objet et sources de vérité

Ce document décrit le schéma MariaDB **actuel** d’Althéa. Il est destiné aux mainteneurs et doit être mis à jour après toute migration structurelle.

| Source | Usage |
|---|---|
| `backup_NoData_althea.sql` | Source de vérité de la structure finale : tables, colonnes, index et contraintes. |
| `backup_WithData_althea.sql` | Vérification des données de référence et des usages réels. |
| `Migration/` | Historique des évolutions et scripts de déploiement/rollback. |
| `tec_meta_schema` | Versions réellement appliquées à une base donnée. |

Le schéma utilise InnoDB, `utf8mb4` et la collation `utf8mb4_uca1400_ai_ci`.

## Conventions

- tables et colonnes en `snake_case` ;
- préfixes : `ref_` (référentiel), `lia_` (liaison), `tec_` (technique), `sec_` (sécurité) ;
- identifiants techniques : `id_*` ; codes lisibles générés ou administrables : `code_*` ;
- tables métier : identifiants issus de séquences `seq_*` et codes générés ;
- référentiels, liaisons de paramétrage et tables techniques : clés `AUTO_INCREMENT` admises (les tables techniques anciennes et `lia_paiements_seances` utilisent des séquences) ;
- audit standard : `date_creation` et `date_modification` lorsque la table est historisée.

## Vue synthétique

| Famille | Tables |
|---|---|
| Métier | `patients`, `dossiers`, `seances`, `paiements`, `lia_paiements_seances`, `lia_paiements_avoirs` (v2.52.0), `famille_contacts`, `autres_suivis_patient`, `therapeutes`, `documents`, `modeles_documents`, `rendez_vous` |
| Référentiels généraux | `ref_domaines`, `ref_liens_patient`, `ref_role_legal`, `ref_roles_intervenant`, `ref_situations_familiales`, `ref_statuts_dossier`, `ref_statuts_seance`, `ref_types_documents`, `ref_types_rendez_vous` |
| Référentiels tarification/paiement | `ref_natures_seance`, `ref_prix_seance`, `ref_modes_paiement` |
| Référentiels convention / quotas | `ref_organismes`, `ref_quotas` (ex-`ref_quotas_inami`), `ref_categories_quota`, `ref_quotas_plafonds` |
| Liaisons de paramétrage | `lia_natures_seance_domaines`, `lia_prix_seance_domaines` |
| Cabinet / praticiens (v2.29.0) | `cab_cabinet`, `cab_praticiens`, `cab_praticien_identifiants`, `cab_praticien_qualifications` |
| Référentiels cabinet | `ref_types_identifiant_pro` (v2.29.0), `ref_situations_juridiques` (v2.31.0) |
| Référentiels de saisie (v2.34.0) | `ref_pays`, `ref_civilites`, `ref_lateralites` (v2.46.0), `ref_specialites` (v2.55.0), `ref_codes_postaux_be` (v2.56.0) ; mutualités dans `ref_organismes` (`type_organisme = MUTUALITE`) |
| Textes types (v2.42.0, v2.43.0) | `ref_textes_types`, `ref_usages_textes_types`, `lia_textes_types_usages` |
| Technique | `tec_meta_schema`, `tec_parametres`, `tec_groupes_parametres` (v2.40.0), `tec_sauvegardes`, `tec_restaurations`, `tec_sync_evenements_google` |
| Sécurité | `sec_utilisateurs` |

> `ref_types_seance` a été supprimée en v2.17.0. Ses responsabilités sont réparties entre `ref_natures_seance`, `ref_prix_seance` et `lia_prix_seance_domaines`.

## Relations structurantes

```text
patients 1 ── n dossiers 1 ── n seances 1 ── n lia_paiements_seances n ── 1 paiements
                 │                │
                 │                ├── 1 ref_natures_seance
                 │                ├── 1 ref_prix_seance
                 │                └── 1 ref_statuts_seance
                 ├── 1 ref_domaines
                 ├── 0..1 ref_prix_seance (tarif patient du dossier)
                 └── 1 ref_statuts_dossier

ref_organismes 1 ── n ref_domaines        (id_organisme facultatif, type REMBOURSEMENT)
ref_organismes 1 ── n patients            (id_organisme_mutualite facultatif, type MUTUALITE — v2.34.0)
ref_pays       1 ── n cab_cabinet, cab_praticiens, patients, famille_contacts, therapeutes (id_pays — v2.34.0)
ref_civilites  1 ── n cab_praticiens, patients, famille_contacts, therapeutes, autres_suivis_patient (id_civilite — v2.34.0)
ref_domaines n ── n ref_prix_seance     via lia_prix_seance_domaines (validité)
ref_domaines n ── n ref_natures_seance  via lia_natures_seance_domaines (→ 0..1 ref_categories_quota)
ref_domaines 1 ── n ref_categories_quota
ref_domaines 1 ── n ref_quotas          (validité) 1 ── n ref_quotas_plafonds n ── 1 ref_categories_quota
```

Relations complémentaires :

- `famille_contacts` appartient à un patient et référence un lien patient ainsi qu’un rôle légal ;
- `autres_suivis_patient` lie un patient à un thérapeute et, facultativement, à un rôle d’intervenant ;
- `documents` peut être rattaché à un patient, un dossier, une séance ou un rendez-vous ;
- `rendez_vous` peut référencer patient, dossier, séance et type de rendez-vous ;
- `modeles_documents` référence un type de document et éventuellement un domaine ;
- `tec_sync_evenements_google` référence un rendez-vous.

## Parcours Patient / Domaine / Dossier / Séance / Paiement

### `patients`

Patient administratif et clinique : identité, coordonnées, mutualité, situation familiale, alerte, anamnèse, photo et activation manuelle. Le suivi en cours n’est pas stocké : il est dérivé des dossiers (le domaine dominant de la grille est calculé de la même façon).

| Colonne | Rôle |
|---|---|
| `id_patient`, `code_patient` | Identifiant issu de `seq_patients` ; code généré `PAxxxxxx` (colonne calculée). |
| `id_civilite` | (v2.34.0, nullable) Civilité → `ref_civilites`. |
| `nom`, `prenom`, `date_naissance` | Identité. |
| `id_lateralite` | (v2.46.0, nullable) Latéralité → `ref_lateralites`. La colonne texte `lateralite`, reprise par la migration, n'est plus lue : conservée jusqu'à vérification, puis retirée par une migration (le ROLLBACK la recalcule). |
| `niss` | Numéro national, unique, contrôlé à la saisie ; enregistré **normalisé** (sans espaces, points, tirets ni barres, majuscules — `UtilsNiss.Normaliser`, v2.54.0), affiché mis en forme selon son type (`UtilsNiss.Formater`). Le type d'identifiant (Belgique, France, Luxembourg, Autre) n'est pas stocké : déduit au chargement, transmis par l'écran au contrôle (`Patient.TypeIdentifiantNiss`). |
| `adresse_ligne1`, `adresse_ligne2`, `code_postal`, `localite`, `id_pays` | Adresse ; `id_pays` (v2.34.0) → `ref_pays`, code postal contrôlé selon le code ISO du pays. |
| `telephone`, `email` | Coordonnées. |
| `id_organisme_mutualite` | (v2.34.0, nullable) Mutualité → `ref_organismes` de type `MUTUALITE`. |
| `numero_affiliation` | (v2.34.0) Numéro d’affiliation auprès de la mutualité (30). |
| `code_titulaire_1`, `code_titulaire_2` | (v2.34.0) Codes titulaire CT1 / CT2, 3 chiffres. |
| `id_situation_familiale` | Situation familiale → `ref_situations_familiales`. |
| `photo_fichier` | Nom seul du fichier photo d’identité ; chemin reconstruit par `CheminsPatientHelper` (ADR-20). |
| `alerte_rtf`, `alerte_txt` | Alerte patient (RTF + texte pour la recherche). |
| `anamnese_rtf`, `anamnese_txt` | Anamnèse (RTF + texte pour la recherche). |
| `is_actif`, `date_activation`, `date_desactivation` | Activation manuelle. |
| `id_prix_seance_defaut` | (nullable) Tarif par défaut du patient, proposé à ses séances → `ref_prix_seance`. |
| `date_creation`, `date_modification` | Audit. |

**Contraintes** : uniques `uq_patients_code_patient`, `uq_patients_niss` ; CHECK `chk_patients_ct1` / `chk_patients_ct2` (NULL ou 3 chiffres) ; FK civilité, pays et mutualité en `ON UPDATE CASCADE` (suppression refusée tant qu’un patient y est rattaché), FK tarif par défaut en `ON DELETE SET NULL`.

**Historique** : les anciennes colonnes texte `pays` et `mutualite` ont été supprimées en v2.35.0 (étape R4), après le passage de l’application aux colonnes de rattachement (étapes R2 et R3).

### `dossiers`

Contexte de suivi d’un patient dans un domaine. Il contient le statut, les dates de cycle de vie, le tarif patient éventuellement choisi, le thérapeute traitant et les sept zones cliniques RTF/TXT.

| Colonne | Rôle |
|---|---|
| `id_patient` | Patient porteur du dossier. |
| `id_domaine` | Domaine de suivi. |
| `id_prix_seance` | Tarif patient applicable aux séances non conventionnées ; nullable. |
| `id_statut_dossier` | Statut piloté par `ref_statuts_dossier`. |
| `date_ouverture`, `date_cloture`, `date_archivage` | Cycle de vie. |
| `modalites_paiement` | Information libre au niveau du dossier. |
| `id_therapeute_traitant` | Thérapeute traitant (nullable) → `therapeutes`. |
| `*_rtf`, `*_txt` | Zones cliniques : motif, historique, antécédents, diagnostic, traitements, objectifs, historique archivé. |

**Contraintes** : FK vers `patients`, `ref_domaines`, `ref_prix_seance`, `ref_statuts_dossier` et `therapeutes`.

### `ref_domaines`

Référentiel des domaines de prise en charge. **Un domaine représente une convention** (tarif + organisme + quotas) ; un nouvel organisme de remboursement se traduit par un nouveau domaine. Il remplace toute règle portée par le code d’un domaine.

| Colonne | Rôle |
|---|---|
| `code_domaine`, `libelle_domaine` | Identité unique du domaine. |
| `id_organisme` | Organisme de remboursement (nullable) → `ref_organismes` (v2.23.0). Aucun comportement n’est déduit de son code. |
| `tarif_convention` | Le tarif est résolu via les liaisons domaine–prix plutôt que depuis le tarif patient du dossier. |
| `no_show_facturable` | Détermine si une absence non excusée reste due. |
| `soumis_quota` | Active le contrôle de quota pour le domaine. |
| `actif`, `ordre_affichage` | Gestion du référentiel. |
| `logo_fichier` | (v2.45.0) Nom du fichier logo seul (`Logo.png`), NULL = aucun ; le fichier est dans le dossier de données, `{PATH_GENERAL}/Domaines/Domaine_{id}/` (`CheminsDomaineHelper`), donc compris dans les sauvegardes. |

**Historique** : ces colonnes remplacent le booléen supprimé `est_inami` (v2.16.0).

### `seances`

Unité de suivi planifiée ou réalisée, liée obligatoirement à un dossier. Son tarif est résolu à l’enregistrement puis figé ; il ne change ensuite que via le cadenas (SuperUser/Admin).

| Colonne | Rôle |
|---|---|
| `code_seance` | Code généré `SExxxxxx`. |
| `id_dossier` | Dossier de rattachement. |
| `id_prix_seance` | Prix référentiel sélectionné pour la traçabilité. |
| `id_nature_seance` | Nature clinique de la séance. |
| `id_statut_seance` | Statut (planifiée, réalisée, annulée, no-show…). |
| `date_seance`, `heure_debut`, `heure_fin`, `duree_minutes` | Planification. |
| `tarif_seance` | Snapshot monétaire ; il ne doit pas être recalculé rétroactivement. |
| `notes_seance_*`, `evolution_*` | Contenu clinique RTF et index texte. |
| `id_seance_origine` | Séance d’origine lorsque celle-ci est issue d’un report ; NULL sinon (v2.19.0). |

**Contraintes** : FK vers `dossiers`, `ref_prix_seance`, `ref_natures_seance`, `ref_statuts_seance` et auto-référence `fk_seances_seance_origine` → `seances` (`ON DELETE SET NULL`).

### `paiements`

Événement financier : montant, date, statut et mode de règlement. Son affectation à une séance est portée par `lia_paiements_seances`.

| Colonne | Rôle |
|---|---|
| `id_paiement`, `code_paiement` | Identifiant issu de `seq_paiements` ; code généré `PAIxxxxxx`. |
| `date_paiement` | Date du règlement ; NULL tant que le paiement est en attente. |
| `montant` | Montant du paiement. |
| `statut_paiement` | `EN_ATTENTE` (défaut), `PAYE`, `ANNULE`. |
| `id_mode_paiement` | Mode de règlement → `ref_modes_paiement` (v2.50.0, FK `fk_paiements_ref_modes_paiement`, suppression d’un mode utilisé refusée) ; NULL pour un dû en attente. |
| `mode_paiement` | Ancien libellé du mode, texte libre ; n’est plus lu ni écrit depuis la v2.50.0 (conservé, à retirer par une migration ultérieure). |
| `reference_paiement`, `commentaire` | Informations libres. Les annulations d’avant la v2.51.0 y ont leur mention « [Annulé le … par … : motif] » (toujours lue). |
| `date_annulation`, `id_utilisateur_annulation`, `motif_annulation` | (v2.51.0, ADR-49) Annulation d’un encaissement : date et heure, utilisateur → `sec_utilisateurs` (FK `fk_paiements_utilisateur_annulation` ; son nom est lu à l’affichage, jamais recopié), motif (500 caractères). NULL si le paiement n’a pas été annulé, ou l’a été avant la v2.51.0. |
| `date_creation`, `date_modification` | Audit. |

### `lia_paiements_seances`

Affectation d’un paiement à une séance, avec le montant affecté.

| Colonne | Rôle |
|---|---|
| `id_paiement`, `id_seance` | Paiement et séance liés ; couple unique. |
| `montant_affecte` | Part du paiement affectée à la séance. |

**Remarques** :

- l’interface V1 affecte un paiement à une seule séance, mais la liaison garde une structure extensible.
- Le **reste dû** est dérivé — `seances.tarif_seance` moins la somme des affectations des paiements `PAYE` — et n’est jamais stocké ; les paiements en attente ou annulés ne réduisent pas le solde.
- Depuis la v2.49.0, il se lit dans la vue `v_seances_solde` (§ Séquences, triggers et vues).

### `lia_paiements_avoirs` (v2.52.0, ADR-49)

Utilisation d’un avoir : relie un règlement par avoir (paiement au mode `est_avoir`, créé sur la séance réglée) à chaque affectation encaissée qu’il a diminuée. Une ligne par source utilisée.

| Colonne | Rôle |
|---|---|
| `id_lia_paiements_avoirs` | Clé (`AUTO_INCREMENT`, comme les liaisons de paramétrage). |
| `id_paiement_avoir` | Règlement par avoir → `paiements` (FK `fk_lia_paiements_avoirs_paiement`). |
| `id_lia_paiements_seances_source` | Affectation encaissée diminuée → `lia_paiements_seances` (FK `fk_lia_paiements_avoirs_source`) : elle donne l’encaissement et la séance d’origine. |
| `montant` | Part prise sur cette source (CHECK `> 0`). |
| `date_utilisation`, `id_utilisateur` | Quand (défaut `current_timestamp()`) et par qui → `sec_utilisateurs` (FK ; le nom est lu à l’affichage, jamais recopié). |

- **Contraintes** : unicité (`id_paiement_avoir`, `id_lia_paiements_seances_source`) ; aucune suppression en cascade (un encaissement ne se supprime pas, il s’annule).
- **Écriture** : `GestionPaiements.ImputerAvoir`, dans la transaction qui diminue la source et crée le règlement.
- **Historique** : les utilisations d’avant la v2.52.0 n’y figurent pas ; leurs mentions restent dans `paiements.commentaire` et sont toujours lues.

## Référentiels de tarification, natures et quotas

### `ref_prix_seance`

Catalogue administrable des prix de séance.

| Colonne | Rôle |
|---|---|
| `code_prix_seance`, `libelle_prix_seance` | Identité ; uniques. |
| `montant` | Prix proposé (decimal 10,2). |
| `actif`, `ordre_affichage`, `est_systeme` | Gestion standard du référentiel (ADR-28). |

**Remarques** : modifier un montant change les propositions futures, jamais `seances.tarif_seance` déjà enregistré.

### `ref_natures_seance`

Catalogue des natures cliniques de séance.

| Colonne | Rôle |
|---|---|
| `code_nature_seance`, `libelle_nature_seance` | Identité ; uniques. |
| `actif`, `ordre_affichage` | Gestion standard du référentiel. |

**Historique** : la nature ne porte plus d’information de quota — `code_prestation_inami` supprimée en v2.27.0, `id_categorie_quota` en v2.28.0. Le rangement dans un compteur dépend du domaine et est porté par `lia_natures_seance_domaines`.

### `lia_prix_seance_domaines`

Association N–N historisée entre un prix et un domaine : prix applicables dans le domaine, par période.

| Colonne | Rôle |
|---|---|
| `id_prix_seance`, `id_domaine` | Prix et domaine liés. |
| `date_debut_validite`, `date_fin_validite` | Période d’applicabilité ; fin >= début. |
| `actif` | Désactivation sans perte d’historique. |

**Contraintes** : unique `(id_prix_seance, id_domaine, date_debut_validite)` (pas de doublon de période initiale) ; FK prix et domaine en `ON DELETE CASCADE`.

**Remarques** : le non-chevauchement des périodes est validé par la couche métier ; il ne peut pas être exprimé complètement par une contrainte MariaDB standard.

### `lia_natures_seance_domaines`

Association N–N entre une nature et un domaine : la nature est proposée dans le domaine, et y consomme éventuellement une catégorie de quota.

| Colonne | Rôle |
|---|---|
| `id_nature_seance`, `id_domaine` | Nature et domaine liés ; couple unique. |
| `id_categorie_quota` | (v2.26.1, nullable) Catégorie de quota consommée par la nature **dans ce domaine** ; NULL = hors quota. |
| `actif` | Nature disponible ou non dans le domaine. |

**Contraintes** : unique `(id_nature_seance, id_domaine)` ; FK nature et domaine en `ON DELETE CASCADE` ; FK composite `fk_lia_nature_domaine_categorie (id_categorie_quota, id_domaine)` → `ref_categories_quota (id_categorie_quota, id_domaine)` : la catégorie appartient forcément au même domaine.

### `ref_organismes` (v2.23.0, v2.33.0, v2.34.0)

Organismes de remboursement (rattachés aux domaines) et mutualités (rattachées aux patients), avec leurs coordonnées et leur procédure d’encodage.

| Colonne | Rôle |
|---|---|
| `code_organisme`, `libelle_organisme` | Identité ; uniques. Pour les mutualités, code = numéro INAMI de l’organisme assureur. |
| `type_organisme` | (v2.34.0) `REMBOURSEMENT` (défaut, proposé aux domaines via `ref_domaines.id_organisme`) ou `MUTUALITE` (proposé aux patients via `patients.id_organisme_mutualite`). |
| `adresse` | (v2.33.0) Adresse postale, texte libre (250). |
| `telephone`, `telephone_2` | (v2.33.0) Téléphones (30) ; validation souple `UtilsValidation.IsValidTelephone`. |
| `email`, `email_2` | (v2.33.0) E-mails (150) ; validation `UtilsValidation.IsValidEmail`. |
| `site_web` | (v2.33.0) Site public (250). |
| `url_plateforme` | (v2.33.0) Plateforme d’encodage en ligne (250). |
| `procedure_encodage` | (v2.33.0) Mémo libre (TEXT) : marche à suivre pour l’encodage. |
| `id_periodicite` | (v2.33.0, nullable) Périodicité d’encodage → `ref_periodicites` ; portée par l’organisme, jamais par le dossier ou le patient. |
| `actif`, `ordre_affichage`, `est_systeme` | Gestion standard du référentiel ; INAMI est une ligne système. |

**Contraintes** : CHECK `chk_ref_organismes_type` ; FK `fk_ref_organismes_periodicite` en `ON UPDATE CASCADE ON DELETE SET NULL`. Suppression physique seulement si aucun domaine ni patient n’y est rattaché.

**Remarques** :

- les colonnes v2.33.0 sont nullables (NULL = non renseigné ; l’UI convertit les champs vides en NULL).
- Le type pilote les listes proposées ; c’est une énumération du code (`TypeOrganisme`, module `TypesOrganisme`), pas un référentiel.
- Sur une ligne système, il est verrouillé comme le code (garde-fou dans l’`UPDATE` : `IF(est_systeme = 1, type_organisme, @type)`).
- Amorçage v2.34.0 des 7 organismes assureurs belges :
  - `100` Mutualités chrétiennes,
  - `200` Mutualités neutres,
  - `300` Solidaris,
  - `400` Mutualités libérales,
  - `500` Mutualités libres,
  - `600` CAAMI,
  - `900` HR Rail

  — actifs, non système, coordonnées à compléter.

### `ref_periodicites` (v2.33.0, lot 29)

Périodicités d’encodage auprès des organismes. Aucun comportement n’est déduit du code : il dépend uniquement de `nb_mois`.

| Colonne | Rôle |
|---|---|
| `code_periodicite`, `libelle_periodicite` | Identité ; uniques. |
| `nb_mois` | (nullable) Durée du cycle en mois ; NULL = non périodique / à la demande. |
| `actif`, `ordre_affichage`, `est_systeme` | Gestion standard du référentiel. |
| `date_creation`, `date_modification` | Audit. |

**Remarques** : valeurs initiales `MENSUELLE` (1), `TRIMESTRIELLE` (3), `ANNUELLE` (12), `A_LA_DEMANDE` (NULL), actives et non système. Désactivation seulement ; les organismes rattachés conservent leur périodicité. Administration : tuile **Périodicités d’encodage** des Référentiels (`UC_Periodicites`).

### `ref_pays` (v2.34.0)

Pays des adresses. Les règles de validation du téléphone et du code postal (`UtilsTelephone`, `UtilsCodePostal`) restent dans le code et sont **indexées par le code ISO, jamais par le libellé**.

| Colonne | Rôle |
|---|---|
| `code_pays` | Code ISO 3166-1 alpha-2 (char 2) ; unique, CHECK `chk_ref_pays_code` (`^[A-Z]{2}$`). |
| `libelle_pays` | Libellé ; unique. |
| `actif`, `ordre_affichage`, `est_systeme` | Gestion standard du référentiel. |
| `date_creation`, `date_modification` | Audit. |

**Utilisée par** : `id_pays` (nullable, `ON UPDATE CASCADE`, suppression refusée si utilisé) dans `cab_cabinet`, `cab_praticiens`, `patients`, `famille_contacts` et `therapeutes`.

**Remarques** :

- les 5 pays dotés de règles — `BE`, `FR`, `LU`, `DE`, `NL` — sont des lignes système ; un pays ajouté par l’interface est accepté sans contrôle de format.
- Les requêtes lisent le code ISO par sous-requête (`(SELECT rp.code_pays FROM ref_pays rp WHERE rp.id_pays = t.id_pays) AS code_pays`) plutôt que par jointure, pour éviter l’ambiguïté de colonnes communes (`date_creation`…).
- Administration : tuile **Pays** des Référentiels (`UC_Pays`).

**Historique** : la v2.34.0 a converti les textes existants reconnus (libellé ou code ISO) ; les anciennes colonnes texte `pays` ont été supprimées en v2.35.0.

### `ref_civilites` (v2.34.0)

Civilités des personnes, avec leur forme abrégée pour les courriers.

| Colonne | Rôle |
|---|---|
| `code_civilite`, `libelle_civilite` | Identité ; uniques. |
| `abreviation_civilite` | (nullable) Forme abrégée pour les courriers (« Mme », « M. »). |
| `actif`, `ordre_affichage`, `est_systeme` | Gestion standard du référentiel. |
| `date_creation`, `date_modification` | Audit. |

**Utilisée par** : `id_civilite` (nullable, `ON UPDATE CASCADE`, suppression refusée si utilisé) dans `cab_praticiens`, `patients`, `famille_contacts`, `therapeutes` et `autres_suivis_patient` (intervenant saisi librement).

**Remarques** : valeurs initiales `MADAME` (Mme), `MONSIEUR` (M.), `MX` (Mx), non système. Administration : tuile **Civilités** des Référentiels (`UC_Civilites`).

**Historique** : la v2.34.0 a converti `cab_praticiens.civilite` ; l’ancienne colonne texte a été supprimée en v2.35.0.

### `ref_lateralites` (v2.46.0)

Latéralités proposées dans la fiche patient (décision de Joëlle, 20/10/2026 : référentiel plutôt qu'une liste écrite dans le code ; en graphothérapie, d'autres valeurs peuvent compter).

| Colonne | Rôle |
|---|---|
| `code_lateralite`, `libelle_lateralite` | Identité ; uniques. Valeurs de départ : Droitier, Gaucher, Ambidextre ; les valeurs déjà saisies hors de cette liste ont été reprises par la migration. |
| `actif`, `ordre_affichage` | Gestion standard ; pas de ligne système (aucun comportement ne dépend d'une valeur). |

**Utilisée par** : `patients.id_lateralite` (nullable, clé étrangère `fk_patients_ref_lateralites`).

### `ref_specialites` (v2.55.0)

Spécialités des thérapeutes (décisions de Joëlle, 23/10/2026 : choisies dans une liste plutôt que tapées librement ; **une seule** par thérapeute).

| Colonne | Rôle |
|---|---|
| `code_specialite`, `libelle_specialite` | Identité ; uniques (casse et accents ignorés, collation de la table). Liste de départ : les spécialités courantes (généraliste, pédiatre, neuropédiatre, logopède, kinésithérapeute, ergothérapeute, psychomotricien, orthoptiste, neuropsychologue, graphothérapeute…) ; les valeurs déjà saisies hors de cette liste ont été reprises par la migration. |
| `actif`, `ordre_affichage` | Gestion standard ; ordre 0 au départ (liste triée par libellé) ; pas de ligne système (aucun comportement ne dépend d'une valeur). |

**Utilisée par** : `therapeutes.id_specialite` (nullable, clé étrangère `fk_therapeutes_ref_specialites`).

- La colonne texte `therapeutes.specialite`, reprise par la migration, n'est plus lue ni écrite : conservée jusqu'à vérification, puis retirée par une migration (le ROLLBACK la recalcule).
- `autres_suivis_patient.specialite` reste du texte : copie figée de la spécialité au moment du suivi.

### `ref_codes_postaux_be` (v2.56.0, ADR-53)

Codes postaux belges et noms de localité acceptés pour chacun (demande de Joëlle, décisions du 24/10/2026). Une ligne par nom accepté : « 1000 Bruxelles » et « 1000 Brussel », « 4280 Hannut » et ses sections (Avin, Merdorp…).

| Colonne | Rôle |
|---|---|
| `code_postal` | Code à 4 chiffres (char 4) ; CHECK `chk_ref_codes_postaux_be_code` (`^[1-9][0-9]{3}$`). |
| `localite` | Nom accepté ; unique avec le code (`uq_ref_codes_postaux_be`, casse et accents ignorés). |
| `est_proposee` | 1 = le nom rempli d'office à la saisie du code (un par code : la commune qui regroupe l'essentiel des adresses du code, dans la langue de sa région). |
| `date_creation` | Audit. |

**Utilisée par** : `GestionCodesPostauxBe` (lecture seule, liste gardée en mémoire le temps de la session), pour les adresses des patients, contacts, thérapeutes, du cabinet et du praticien dont le pays est la Belgique : un code inexistant est refusé, la localité est proposée et suggérée depuis le code, le code depuis la localité (choix « Liège · 4020 », ou code unique rempli), une localité hors du code est signalée puis confirmée (pas un refus). Aucune clé étrangère : les adresses gardent leur texte.

**Contenu** : produit par `Docs/Database/Maintenance/codes_postaux_be/generer_codes_postaux_be.py` à partir de deux fichiers publics :

- la liste de bpost (codes officiels ; un seul nom par localité, en néerlandais pour Bruxelles et les communes à facilités) ;
- le registre des adresses BeST du SPF BOSA (noms des communes en français, néerlandais et allemand, sections et noms postaux), dont seules les communes et leurs noms postaux sont retenus — ni rues ni numéros.

Au 24/10/2026 : 4 083 noms pour 1 189 codes (bpost du 18/02/2026, BeST du 21/10/2026), plus « Laeken » ajouté à la main.

**Mise à jour** (rare : nouveau code, fusion de communes) :

1. télécharger la liste de bpost par code postal (`zipcodes_num_fr_AAAA.xls`, page « Outil de validation de codes postaux ») et les trois archives BeST (`openaddress-bebru.zip`, `-bewal.zip`, `-bevlg.zip`, opendata.bosa.be) dans un dossier de travail hors du dépôt ;
2. lancer `python generer_codes_postaux_be.py <fichier bpost> <dossier des archives> <sortie.sql>` (Python 3 et le module `xlrd`) ;
3. ranger le SQL produit dans `Docs/Database/Maintenance/` (nom daté) et l'appliquer : il remplace tout le contenu de la table, dans une transaction terminée par `COMMIT` ;
4. relancer l'application (la liste est relue au démarrage de la session).

La règle de comparaison des noms du script (`cle()`) doit rester celle de `UtilsCodePostal.CleLocalite`.

### `ref_textes_types` (v2.42.0, v2.43.0, éditeur temps 2)

Phrases et trames réutilisables insérées dans les éditeurs de texte (bouton « Textes types »). À ne pas confondre avec `modeles_documents` (modèles Word complets, en fichiers).

| Colonne | Rôle |
|---|---|
| `code_texte_type`, `libelle_texte_type` | Identité ; uniques (code saisi, majuscules, espaces remplacés par `_`). |
| `contenu_rtf`, `contenu_txt` | Texte en RTF (insertion avec mise en forme) et en texte brut (recherche, repli) — double format, Rules § 21. |
| `actif`, `ordre_affichage` | Gestion ; un texte inactif n'est plus proposé (pas de suppression depuis l'interface). |
| `date_creation`, `date_modification` | Audit. |

**Usages** : table `lia_textes_types_usages` (v2.43.0) ; **aucun usage = proposé dans tous les éditeurs**. La colonne `id_usage_texte_type` (un seul usage, v2.42.0) a été reprise dans la liaison puis supprimée par la v2.43.0.

**Administration** : tuile **Textes types** des Référentiels (`UC_TextesTypes`, groupe Dossier).

### `ref_usages_textes_types` (v2.42.0, v2.43.0)

Éditeurs où un texte type est proposé. Lignes **système** : leur code est repris par le code (`UsagesTexteType`) et chaque éditeur déclare le sien (`UC_RichTextEditor.UsageTextesTypes`).

| Colonne | Rôle |
|---|---|
| `code_usage`, `libelle_usage` | Identité ; uniques. Codes : `ANAMNESE`, `ALERTE_PATIENT`, `ZONE_DOSSIER`, `NOTES_SEANCE`, `EVOLUTION_SEANCE`, `COMMENTAIRE_CONTACT`, `COMMENTAIRE_INTERVENANT`, `COMMENTAIRE_THERAPEUTE` (v2.43.0 : alerte ajoutée, `COMMENTAIRE` séparé en trois). |
| `actif`, `ordre_affichage`, `est_systeme` | Gestion ; un usage inactif n'est plus proposé dans la liste et ses textes ne sont plus proposés. |

**Remarques** : tous les éditeurs de texte ont un usage (v2.43.0). Écran **Usages des textes types** (`UC_UsagesTextesTypes`) : libellé et ordre ; pas de création (un nouvel usage naît par migration, avec sa constante et sa déclaration dans l'écran de l'éditeur).

### `lia_textes_types_usages` (v2.43.0)

Usages d'un texte type (plusieurs possibles). Clé primaire (`id_texte_type`, `id_usage_texte_type`) ; texte → `ON DELETE CASCADE`, usage → `ON DELETE RESTRICT` ; `ON UPDATE CASCADE` des deux côtés. Écrite avec le texte dans une transaction (`GestionTextesTypes.InsertTexteType` / `UpdateTexteType`).

### `ref_quotas`

Règles de quota par domaine et période de validité (renommée depuis `ref_quotas_inami` en v2.22.0 ; PK `id_quota`).

| Colonne | Rôle |
|---|---|
| `id_domaine` | Domaine auquel s’applique la règle. |
| `duree_periode_mois` | (v2.25.0) Durée de la période glissante de décompte (> 0, défaut 12). |
| `seuil_alerte_total` | Seuil d’avertissement paramétré pour la règle. |
| `date_debut_validite`, `date_fin_validite` | Historisation de la règle. |
| `actif` | Activation de la règle. |

**Contraintes** : unicité `(id_domaine, date_debut_validite)`, fin de validité postérieure ou égale au début, durée > 0, FK domaine (suppression refusée si utilisé).

**Remarques** :

- les colonnes `plafond_soutien` / `plafond_traitement` ont été supprimées en v2.28.0.
- Le non-chevauchement reste contrôlé côté métier ; l’ajout d’une règle clôture automatiquement la règle ouverte à la veille, dans la même transaction (`GestionQuotasInami.AjouterRegleEnChaine`, 21/10/2026).
- Une règle qui a servi (séance du domaine dans sa période) ne se modifie ni ne se supprime ; une règle qui n'a pas servi peut être corrigée (`UPDATE`) ou supprimée, la règle précédente étant alors rouverte (`UpdateFinValiditeQuota`, même transaction) — ADR-40.
- Aucune modification de schéma.

### `ref_categories_quota` (v2.25.0)

Compteurs de quota d’un domaine (ex. Soutien, Traitement), en nombre libre.

| Colonne | Rôle |
|---|---|
| `id_domaine` | Domaine (convention) auquel appartient la catégorie → `ref_domaines`. |
| `code_categorie`, `libelle_categorie` | Identité ; uniques par domaine. |
| `actif`, `ordre_affichage` | Gestion standard du référentiel. |

**Contraintes** : uniques `(id_domaine, code_categorie)` et `(id_domaine, libelle_categorie)` ; clé unique `(id_categorie_quota, id_domaine)`, support de la FK composite de `lia_natures_seance_domaines`.

**Remarques** : une catégorie utilisée (liaison ou plafond) ne peut être que désactivée.

### `ref_quotas_plafonds` (v2.25.0)

Plafond d’une catégorie pour une règle de quota.

| Colonne | Rôle |
|---|---|
| `id_quota` | Règle datée → `ref_quotas` (`ON DELETE CASCADE`). |
| `id_categorie_quota` | Catégorie plafonnée → `ref_categories_quota`. |
| `plafond` | Nombre de séances remboursées par période (>= 0). |

**Contraintes** : unique `(id_quota, id_categorie_quota)` ; CHECK `chk_ref_quotas_plafonds_positif`.

**Remarques** : le total de la règle est calculé (somme des plafonds), jamais stocké.

### `ref_modes_paiement`

Modes de règlement proposés à la saisie d’un paiement.

- `paiements.id_mode_paiement` y renvoie par clé étrangère (v2.50.0, ADR-49) : libellé et code se modifient sans toucher l’historique ; un mode utilisé se désactive, il ne se supprime pas.
- Avant la v2.50.0, `paiements.mode_paiement` stockait le **libellé** ; les libellés inconnus ont été repris en modes inactifs (ordre 900).

| Colonne | Rôle |
|---|---|
| `code_mode_paiement`, `libelle_mode_paiement` | Identité ; uniques. |
| `est_avoir` | (v2.20.0) Mode réservé au règlement d’une séance par un avoir existant du patient (mode `AVOIR`) ; non proposé à la saisie manuelle, utilisé uniquement par `GestionPaiements.UtiliserAvoirPourSeance`. Un règlement par avoir se reconnaît à ce drapeau (`QueryPaiements.SelectEstReglementParAvoir`), jamais au libellé. |
| `actif`, `ordre_affichage`, `est_systeme` | Gestion standard du référentiel. |

## Autres tables métier

| Table | Rôle | Références principales |
|---|---|---|
| `famille_contacts` | Contacts familiaux et légaux d’un patient. | Patient, lien patient, rôle légal. |
| `autres_suivis_patient` | Réseau pluridisciplinaire lié au patient. | Patient, rôle intervenant, thérapeute. |
| `therapeutes` | Entité riche des thérapeutes, réutilisée comme traitant ou intervenant ; spécialité → `ref_specialites` (`id_specialite`, v2.55.0). | Référencée par dossiers et suivis externes. |
| `documents` | Métadonnées des documents liés à patient/dossier/séance/rendez-vous. | Type de document et hiérarchie métier. |
| `modeles_documents` | Modèles de documents actifs et ordonnés. | Type de document, domaine facultatif. |
| `rendez_vous` | Préparation agenda ; peut être lié à patient, dossier et séance. | Type de rendez-vous. |

## Référentiels généraux

Tous ces référentiels suivent le même modèle : PK `AUTO_INCREMENT`, code unique, libellé unique, `actif` et `ordre_affichage`.

| Table | Utilisée par |
|---|---|
| `ref_liens_patient` | `famille_contacts.id_lien_patient` ; `par_defaut` (v2.57.0) désigne le lien présélectionné pour un nouveau contact (une seule ligne à la fois, garanti par l’application) — `MERE` au départ. |
| `ref_role_legal` | `famille_contacts.id_role_legal` ; `par_defaut` (v2.57.0) désigne le rôle présélectionné pour un nouveau contact (une seule ligne à la fois) — `AUTORITE_PARENTALE` au départ. |
| `ref_roles_intervenant` | `autres_suivis_patient.id_role_intervenant` |
| `ref_specialites` | `therapeutes.id_specialite` (v2.55.0) |
| `ref_situations_familiales` | `patients.id_situation_familiale` |
| `ref_statuts_dossier` | `dossiers.id_statut_dossier` |
| `ref_statuts_seance` | `seances.id_statut_seance` ; `est_du` guide le caractère dû d’une séance ; `consomme_quota` (v2.24.0) désigne les statuts décomptés dans les quotas ; `est_absence` (v2.53.0) désigne une absence non excusée (no-show : facturation selon `ref_domaines.no_show_facturable`, compteur d’absences, exclue de la « dernière séance ») — cochée pour `ABSENTE` ; `est_non_tenue` (v2.57.0) désigne une séance qui n’a pas eu lieu et libère son créneau (pas de contrôle de chevauchement) — cochée au départ pour `ANNULEE_PATIENT`, `ANNULEE_PRATICIEN`, `REPORTEE` et `MALADE` ; une absence (`est_absence`) compte comme non tenue pour la règle entre statuts mais garde son créneau ; `propose_creation` (v2.57.0) désigne les statuts proposés à la création d’une séance — `PLANIFIEE` (obligatoire) et `REALISEE`. |
| `ref_types_documents` | `documents`, `modeles_documents` |
| `ref_types_rendez_vous` | `rendez_vous` |

**Lignes système (v2.21.0, ADR-28)** : `ref_statuts_seance`, `ref_statuts_dossier`, `ref_modes_paiement`, `ref_types_documents`, `ref_liens_patient`, `ref_role_legal`, `ref_prix_seance` et `ref_organismes` portent `est_systeme`, ainsi que `ref_periodicites`, `ref_situations_juridiques`, `ref_types_identifiant_pro`, `ref_pays` et `ref_civilites`.

- Positionnée uniquement par SQL (migration), elle interdit changement de code, désactivation et suppression depuis l’interface (cadenas visible dans la grille) ; pour `ref_organismes`, le type est aussi verrouillé.
- Le libellé, l’ordre et les champs descriptifs restent modifiables (décision du 11/10/2026 : par exemple compléter les coordonnées de l’INAMI).
- Une ligne n’est système que tant que l’application la cherche par son code : en v2.58.0 (25/10/2026, ADR-54), les statuts `ABSENTE`, `ANNULEE_PATIENT`, `ANNULEE_PRATICIEN`, `MALADE`, `NO_SHOW`, le lien `MERE` et le rôle `AUTORITE_PARENTALE`, réglés par des cases depuis la v2.57.0, sont déverrouillés ; `ref_liens_patient` et `ref_role_legal` n’ont plus de ligne système (la colonne reste).
- Depuis le 22/10/2026 (audit C.1), la condition est écrite dans chaque requête de modification et de désactivation (`WHERE … AND (est_systeme = 0 OR (code inchangé AND actif = 1))`, `AND est_systeme = 0`) et l’application refuse explicitement une écriture qui n’a concerné aucune ligne ; aucune modification de schéma.

**Remise à vide de la base** (script à écrire) : conserver les lignes système telles quelles et rejouer les amorçages des migrations, écrits pour être rejoués sans doublon.

## Cabinet et praticiens (lot 27)

```text
ref_situations_juridiques 1 ── n cab_cabinet 1 ── n cab_praticiens 1 ── n cab_praticien_identifiants n ── 1 ref_types_identifiant_pro
                                                           └── n cab_praticien_qualifications
```

**V1** : un cabinet = une application = un praticien = un seul SuperUser (contrainte côté application).

- Le schéma accepte déjà plusieurs praticiens par cabinet (V2/V3).
- Les images (logo, photo, cachet, signature) ne sont **pas stockées en base** : seules leurs chemins relatifs le sont, les fichiers vivent dans le stockage applicatif (`CheminsCabinetHelper`).

### `cab_cabinet` (v2.29.0, v2.31.0)

Entité juridique / de facturation. Un seul cabinet actif est lu (`actif = 1`, plus petit id).

| Colonne | Rôle |
|---|---|
| `nom_cabinet` | Obligatoire. Nom du praticien ou nom commun d’un cabinet de groupe. |
| `id_situation_juridique` | (v2.31.0, nullable) → `ref_situations_juridiques`, `ON DELETE RESTRICT`. Détermine le porteur du numéro BCE. |
| `adresse_rue`, `adresse_numero`, `code_postal`, `localite`, `id_pays` | Adresse ; `id_pays` → `ref_pays` (v2.34.0, texte `pays` supprimé en v2.35.0) ; code postal contrôlé selon le code ISO du pays (`UtilsCodePostal`). |
| `telephone`, `email`, `site_web` | Coordonnées. |
| `numero_bce` | BCE de l’entité qui facture, **utilisé seulement si la situation désigne le cabinet comme porteur**. Clé modulo 97 contrôlée. |
| `numero_tva` | Uniquement si assujetti. |
| `iban`, `bic` | Compte bancaire ; IBAN contrôlé (ISO 13616, BE = 16 caractères), BIC 8 ou 11 caractères. |
| `chemin_logo` | Chemin relatif du logo. |

### `cab_praticiens` (v2.29.0, v2.30.0, v2.32.0)

Praticien rattaché au cabinet. Pas de suppression : activation / désactivation.

| Colonne | Rôle |
|---|---|
| `id_cabinet` | Cabinet de rattachement → `cab_cabinet`. |
| `id_utilisateur` | (v2.32.0, lot 28, nullable) Compte de travail du praticien → `sec_utilisateurs` ; unique (un compte → un seul praticien). |
| `id_civilite` | (v2.34.0, nullable) Civilité → `ref_civilites`. |
| `nom`, `prenom` | Identité. |
| `titre_professionnel` | Titre affiché sur le cachet. |
| `telephone`, `email`, `site_web` | Coordonnées. |
| `adresse_rue`, `adresse_numero`, `code_postal`, `localite`, `id_pays` | Adresse propre (vide = adresse du cabinet) ; `id_pays` → `ref_pays` (v2.34.0). |
| `chemin_photo`, `chemin_cachet`, `chemin_signature` | Chemins relatifs des images ; `chemin_cachet` vide = cachet généré. |
| `actif` | Activation. |
| `date_creation`, `date_modification` | Audit. |

**Contraintes** : unique `uq_cab_praticiens_utilisateur` ; FK utilisateur en `ON UPDATE CASCADE ON DELETE SET NULL` (supprimer un compte ne supprime pas le praticien) ; FK cabinet, civilité et pays en `ON UPDATE CASCADE`.

**Remarques** : la migration v2.32.0 ne touche aucun compte ; le compte **Admin technique n’est jamais lié**. L’UI (`UC_Cabinet` › Compte de travail) ne propose que des `SuperUser` actifs non liés et crée les nouveaux comptes en `SuperUser` / `role_max_elevation = Admin` / `must_change_password = 1`.

**Historique** : `couleur_agenda` supprimée en v2.30.0 (la couleur dans l’agenda dépendra du type de rendez-vous ou du domaine) ; texte `civilite` remplacé par `id_civilite` (v2.34.0, supprimé en v2.35.0).

### `cab_praticien_identifiants` (v2.29.0)

Identifiants professionnels datables d’un praticien (INAMI, COMPY, BCE…).

| Colonne | Rôle |
|---|---|
| `id_praticien` | Praticien → `cab_praticiens` (`ON DELETE CASCADE`). |
| `id_type_identifiant` | Type → `ref_types_identifiant_pro`. |
| `valeur` | Valeur de l’identifiant (50). |
| `date_debut`, `date_fin` | Période de validité (nullables). |
| `afficher_sur_cachet` | Affichage sur le cachet généré (défaut 1). |

**Contraintes** : CHECK `chk_cab_praticien_identifiants_dates` (fin >= début).

**Remarques** : les valeurs INAMI, BCE et TVA sont contrôlées par `UtilsIdentifiantBelge` ; les autres types par la regex du référentiel.

### `cab_praticien_qualifications` (v2.29.0)

Diplômes et titres d’un praticien.

| Colonne | Rôle |
|---|---|
| `id_praticien` | Praticien → `cab_praticiens` (`ON DELETE CASCADE`). |
| `libelle`, `etablissement`, `annee_obtention` | Qualification. |
| `afficher_sur_cachet` | Affichage sur le cachet généré (défaut 0). |
| `ordre_affichage` | Ordre d’affichage. |

### `ref_types_identifiant_pro` (v2.29.0)

Types d’identifiant professionnel (INAMI, COMPY, BCE, TVA…), gérés dans Administration › Types d'identifiant (`UC_TypesIdentifiantPro`, SuperUser, 20/10/2026) ; proposés dans l'onglet Identifiants de la fiche Cabinet.

| Colonne | Rôle |
|---|---|
| `code_type_identifiant`, `libelle_type_identifiant` | Identité ; code unique. |
| `masque_saisie`, `regex_validation` | Masque indicatif (rappelé quand une valeur est refusée) et expression régulière de contrôle, vérifiée valide avant enregistrement. Non utilisée pour `INAMI`, `BCE`, `TVA` : contrôle écrit dans l'application (`UtilsIdentifiantBelge`, choisi par le code). |
| `cible` | `PRATICIEN` (défaut) ou `CABINET` ; CHECK `chk_ref_types_identifiant_pro_cible`. Qui porte le numéro ; préparé pour la V2 (plusieurs praticiens), la V1 propose tous les types actifs. |
| `actif`, `ordre_affichage`, `est_systeme` | Gestion standard du référentiel. Lignes système : `INAMI` (v2.29.0), `BCE` et `TVA` (v2.47.0) — l'application les reconnaît par leur code. Le garde-fou est aussi dans les requêtes : `UPDATE` ne change ni leur code ni leur activation. |

Un type désactivé n'est plus proposé pour une nouvelle ligne d'identifiant ; les identifiants qui l'utilisent l'affichent toujours.

### `ref_situations_juridiques` (v2.31.0)

Situation juridique du cabinet. **Aucun comportement n’est déduit du code** : il dépend uniquement des attributs.

| Colonne | Rôle |
|---|---|
| `code_situation_juridique`, `libelle_situation_juridique` | Uniques. |
| `porteur_bce` | `CABINET` (BCE saisi dans `cab_cabinet.numero_bce`) ou `PRATICIEN` (BCE saisi comme identifiant de chaque praticien). CHECK sur ces deux valeurs. |
| `actif`, `est_systeme`, `ordre_affichage` | Gestion standard du référentiel. |

**Remarques** : valeurs initiales `INDEPENDANT_PP` (cabinet), `SOCIETE` (cabinet), `CABINET_GROUPE` (praticien). Elles restent distinctes même si leur comportement BCE est identique, car la facturation future pourra les différencier.

**Règle de saisie unique du BCE** (`UC_Cabinet`) : le BCE n’existe qu’à un seul endroit, désigné par `porteur_bce`. À l’enregistrement, un BCE placé au mauvais endroit est **proposé** au déplacement (jamais déplacé ni supprimé sans confirmation) ; en situation `PRATICIEN`, l’enregistrement est refusé tant que le praticien n’a pas d’identifiant BCE.

## Tables techniques et sécurité

### `tec_meta_schema`

Journal des migrations appliquées à la base. Toujours le vérifier avant de choisir le numéro d’une nouvelle migration.

| Colonne | Rôle |
|---|---|
| `id_meta_schema` | Identifiant issu de `seq_tec_meta_schema`. |
| `version_schema` | Version appliquée (ex. `2.38.0`) ; unique. |
| `description` | Résumé de la migration (255 au plus). |
| `date_application` | Date d’application. |

### `tec_parametres`

Paramètres applicatifs. Un paramètre naît par migration ; l’écran n’en modifie que la valeur et l’application ne le supprime jamais (ADR-36). Détail des clés : [Paramètres et référentiels](Parametres_Referentiels.md).

| Colonne | Rôle |
|---|---|
| `id_parametre`, `code_parametre` | Identifiant issu de `seq_tec_parametres` ; code généré `PARxxxxxx`. |
| `cle_parametre` | Clé technique, unique et immuable ; lue par le code via `ClesParametres`. |
| `libelle_parametre`, `description_parametre` | Textes affichés dans `UC_Parametres`. |
| `groupe_parametre` | Code du groupe → `tec_groupes_parametres.code_groupe` (clé étrangère `fk_tec_parametres_groupe`, v2.40.0) ; sans valeur par défaut depuis v2.40.0 : un paramètre nomme toujours son groupe. |
| `type_valeur` | `STRING`, `INT`, `DECIMAL`, `BOOL`, `DATE`, `PATH`. |
| `valeur_parametre` | Valeur unique (texte), sans dates de validité. |
| `valeur_min`, `valeur_max` | (v2.37.0, nullables) Bornes des paramètres numériques, appliquées à la saisie et à la lecture (`GestionParametres.GetEntier`). |
| `modifiable_utilisateur` | 1 = valeur modifiable par le SuperUser ; 0 = Admin seulement. |
| `actif`, `ordre_affichage` | Gestion. |
| `date_creation`, `date_modification` | Audit. |

**Contraintes** : uniques `uq_tec_parametres_cle_parametre`, `uq_tec_parametres_code_parametre` ; clé étrangère `fk_tec_parametres_groupe` (`ON UPDATE CASCADE`, `ON DELETE RESTRICT`).

**Historique** :
- **v2.37.0 (lot 30, P1)** :
  - colonnes `valeur_min` / `valeur_max` ;
  - paramètres `SEANCE_DUREE_DEFAUT_MIN`, `SEANCE_PAS_HORAIRE_MIN`, `SEANCE_HEURE_DEBUT_GRILLE`, `SEANCE_HEURE_FIN_GRILLE`, `SECURITE_MAX_ECHECS_LOGIN`, `LOG_RETENTION_JOURS`, `CACHET_ENCRE_NOIRE` ;
  - `PATH_GENERAL` au type `PATH` ;
  - suppression des paramètres de test (`TESTONG`, `TESTONG2`, `APP_DATA`) et des chemins obsolètes (`PATH_DOCUMENT` — sous-dossier `Documents` désormais fixe, ADR-35 —, `PATH_DOSSIER`, `PATH_LOG`, `PATH_BACKUP`).
- **v2.38.0 (lot 30, P2)** : groupe `SAUVEGARDE` — `SAUVEGARDE_DOSSIER`, `SAUVEGARDE_NB_CONSERVEES`, `SAUVEGARDE_RAPPEL_JOURS`, `SAUVEGARDE_DOSSIER_MARIADB` (Admin).
- **v2.40.0 (UI-7)** : groupes déclarés dans `tec_groupes_parametres` (clé étrangère) ; `groupe_parametre` perd sa valeur par défaut `GENERAL`.
- **v2.48.0** : `SECURITE_ELEVATION_MINUTES` (INT, 15, 1 – 240, Admin) : fin de l'élévation après inactivité. Aucune structure modifiée.
- **v2.46.0** : groupe `DOSSIERS` (« Dossiers », ordre 25) ; `DOSSIER_MODALITE_PAIEMENT_DEFAUT` (STRING, « À la séance », SuperUser) : texte proposé à la création d'un dossier.
- **v2.44.0 (éditeur, temps 2)** : groupe `EDITEUR` (« Éditeur de texte », ordre 46) ; `EDITEUR_CORRECTEUR_ORTHOGRAPHE` (BOOL, 1, SuperUser). Aucune structure modifiée.
- **v2.41.0 (éditeur, temps 2)** : groupe `DOCUMENTS` (« Documents imprimés », ordre 45) ; `DOCUMENT_MENTION_CONFIDENTIALITE` (STRING, « Confidentiel », SuperUser), mention du pied de page des documents. Aucune structure modifiée.
- **Jeu de référence** : `Maintenance/reinit_tec_parametres.sql` (table vidée, séquence remise à 1, paramètres lus par le code recréés ; chemins déjà saisis conservés), appliqué sur la base de développement le 12/10/2026 ; les groupes ne sont pas effacés (prérequis v2.40.0).

### `tec_groupes_parametres` (v2.40.0, UI-7)

Groupes des paramètres : chaque groupe est une carte de l’écran Paramètres (`UC_Parametres` V2), avec son titre et sa place. Un groupe naît par migration, comme un paramètre (Rules § 14).

| Colonne | Rôle |
|---|---|
| `id_groupe_parametre` | Identifiant (`AUTO_INCREMENT`, table technique). |
| `code_groupe` | Code technique (MAJUSCULE, sans accents), unique ; repris par `tec_parametres.groupe_parametre`. |
| `libelle_groupe` | Titre de la carte. |
| `ordre_affichage` | Place de la carte dans la page. |
| `date_creation`, `date_modification` | Audit. |

**Contraintes** : unique `uq_tec_groupes_parametres_code` ; un groupe utilisé par un paramètre ne peut pas être supprimé, et un changement de code est reporté sur ses paramètres (`fk_tec_parametres_groupe`).

**Groupes livrés** :

- `PATHS` « Dossier de données » (10),
- `SEANCES` « Séances » (20),
- `SAUVEGARDE` « Sauvegardes » (30),
- `CABINET` « Cabinet » (40),
- `SECURITE` « Sécurité » (50),
- `TECHNIQUE` « Technique » (60) ;
- ajoutés ensuite :
  - `DOCUMENTS` « Documents imprimés » (45, v2.41.0),
  - `EDITEUR` « Éditeur de texte » (46, v2.44.0),
  - `DOSSIERS` « Dossiers » (25, v2.46.0).

Un groupe inattendu trouvé dans `tec_parametres` au passage de la migration est créé avec son code pour titre et l’ordre 900, à renommer par une migration.

### `tec_sauvegardes` (v2.38.0, lot 30)

Journal des sauvegardes : une ligne par tentative, créée au lancement (`EN_COURS`) puis clôturée. Alimente l’historique de `UC_Sauvegardes` et le rappel à l’ouverture (dernière `REUSSIE`).

| Colonne | Rôle |
|---|---|
| `id_sauvegarde` | Clé `AUTO_INCREMENT`. |
| `date_debut`, `date_fin` | Début et fin de la tentative. |
| `statut_sauvegarde` | `EN_COURS` (défaut), `REUSSIE`, `ECHEC` (enum `StatutSauvegarde`). |
| `nom_fichier` | Nom de l’archive (`Althea_AAAAMMJJ_HHMMSS.zip`). |
| `dossier_sauvegarde` | Dossier au moment de la sauvegarde (trace). |
| `taille_octets`, `nb_fichiers` | Taille de l’archive ; nombre de fichiers du dossier de données. |
| `version_schema`, `version_application` | Versions au moment de la sauvegarde. |
| `id_utilisateur`, `login_utilisateur` | Auteur → `sec_utilisateurs` (`ON DELETE SET NULL`) et login conservé en trace. |
| `machine` | Poste qui a sauvegardé. |
| `message_erreur` | Cause d’un échec, sans donnée sensible. |

**Contraintes** : CHECK `chk_tec_sauvegardes_statut` ; index `(statut_sauvegarde, date_debut)`.

**Remarques** :

- une ligne restée `EN_COURS` signale une sauvegarde interrompue par l’arrêt de l’application.
- Une archive contient sa propre ligne à l’état `EN_COURS` (écrite avant l’export de la base, clôturée après) : après une restauration, `GestionRestaurations` la clôture et réinscrit la sauvegarde de sécurité, journalisée dans la base remplacée.
- Le mot de passe des archives n’est **pas** en base (DPAPI sur le poste, ADR-37).

### `tec_restaurations` (v2.39.0, lot 30)

Journal des restaurations : une ligne par tentative (ADR-38). Une restauration réussie remplace toute la base : sa ligne est donc écrite **après** le rechargement, dans la base restaurée ; un échec est écrit dans la base actuelle, intacte ou remise en état. Alimente l'historique de `DialogRestauration`.

| Colonne | Rôle |
|---|---|
| `id_restauration` | Clé `AUTO_INCREMENT`. |
| `date_debut`, `date_fin` | Début et fin de la tentative. |
| `statut_restauration` | `REUSSIE` ou `ECHEC` (enum `StatutRestauration`) ; pas de statut « en cours », qu'un rechargement effacerait. |
| `nom_archive`, `dossier_archive` | Archive restaurée et son dossier au moment de la restauration. |
| `date_archive`, `version_schema_archive`, `machine_archive` | Date, version de la base et poste de la sauvegarde restaurée (manifeste). |
| `nb_fichiers` | Fichiers du dossier de données restaurés. |
| `archive_securite` | Sauvegarde de sécurité faite juste avant. |
| `dossier_donnees_precedent` | Ancien dossier de données, renommé et conservé. |
| `login_utilisateur`, `machine`, `version_application` | Auteur (trace), poste, version de l'application. |
| `message_erreur` | Cause d'un échec, sans donnée sensible (code d'erreur MariaDB et ligne seulement). |

**Contraintes** : CHECK `chk_tec_restaurations_statut` ; index sur `date_debut`.

**Remarques** : pas de clé étrangère vers `sec_utilisateurs` — après restauration, les identifiants des comptes sont ceux de l'archive et ne désignent plus forcément la personne qui a restauré ; le login est conservé en trace.

### `tec_sync_evenements_google`

État de synchronisation d’un rendez-vous avec Google Calendar. La brique agenda reste à intégrer ; la structure est conservée.

| Colonne | Rôle |
|---|---|
| `id_sync_evenement_google`, `code_sync_evenement_google` | Identifiant issu de `seq_tec_sync_evenements_google` ; code généré `SEGxxxxxx`. |
| `id_rendez_vous` | Rendez-vous synchronisé → `rendez_vous`. |
| `google_calendar_id`, `google_event_id` | Références Google. |
| `operation_sync`, `statut_sync`, `message_sync` | Opération, résultat et message. |
| `date_sync` | Date de la synchronisation. |

### `sec_utilisateurs`

Comptes locaux de l’application. Les mots de passe ne sont jamais stockés en clair.

| Colonne | Rôle |
|---|---|
| `id_utilisateur`, `code_utilisateur` | Identifiant issu de `seq_sec_utilisateurs` ; code généré `USRxxxxxx`. |
| `login_utilisateur` | Login ; unique. |
| `nom_affichage` | Nom affiché. |
| `photo_fichier` | (v2.36.0, nullable) Nom seul du fichier photo du compte. |
| `password_hash`, `password_salt` | Empreinte et sel PBKDF2. |
| `role_utilisateur` | `User`, `SuperUser` ou `Admin`. |
| `role_max_elevation` | Rôle maximal accessible par élévation temporaire (défaut `User`). |
| `actif`, `must_change_password` | Activation ; changement de mot de passe obligatoire à la prochaine connexion. |
| `nb_echecs_login`, `compte_verrouille`, `date_verrouillage` | Verrouillage après `SECURITE_MAX_ECHECS_LOGIN` échecs (connexion, et depuis le 20/10/2026 mot de passe d'une élévation). |
| `dernier_login` | Dernière connexion réussie. |
| `date_creation`, `date_modification` | Audit. |

**Contraintes** : uniques login et code ; CHECK sur les rôles (`chk_sec_utilisateurs_role`, `chk_sec_utilisateurs_role_max_elevation`), `actif`, `must_change_password` et `nb_echecs_login >= 0`.

**Remarques (`photo_fichier`)** :

- le chemin est reconstruit par `CheminsUtilisateurHelper` (`{PATH_GENERAL}/Utilisateurs/Utilisateur_{id}/Photo.<ext>`, rangé par identifiant car le login peut changer).
- Règle d’affichage (`PhotosUtilisateur`) : un compte lié à un praticien (`cab_praticiens.id_utilisateur`) affiche la photo de la fiche praticien ; sinon sa photo propre ; à défaut, ses initiales.
- Écriture réservée à l’Admin (`GestionUtilisateurs.UpdatePhotoUtilisateur`, journalisée en catégorie Security).
- Le ROLLBACK v2.36.0 retire la colonne sans supprimer les fichiers.

## Séquences, triggers et vues

- Les séquences `seq_*` alimentent les identifiants des tables métier (`patients`, `dossiers`, `seances`, paiements, documents, rendez-vous, etc.).
- Aucun trigger applicatif.
- Les mises à jour de `date_modification` sont gérées par `ON UPDATE current_timestamp()` sur les tables concernées.

### Vues (v2.49.0, ADR-48)

Deux vues portent les règles de calcul qui étaient recopiées dans plusieurs requêtes.

- Elles sont créées en `SQL SECURITY INVOKER` : elles s’exécutent avec les droits du compte qui les lit (le compte de l’application a déjà `SELECT` et `SHOW VIEW`).
- Recréer une vue demande `CREATE VIEW` : migration en `root`, et compte d’administration de la restauration.

**`v_seances_solde`** — une ligne par séance.

| Colonne | Règle |
|---|---|
| `id_seance`, `id_dossier`, `tarif_seance` | Repris de `seances`. |
| `montant_regle` | Somme des `lia_paiements_seances.montant_affecte` des paiements `PAYE` ; 0 sinon. |
| `est_du` | `ref_statuts_seance.est_du`. |
| `est_facturable` | 1 si la séance est due, sauf absence non excusée (statut coché `est_absence`, v2.53.0 ; le code `ABSENTE` jusque-là) d’un domaine dont `no_show_facturable = 0`. |
| `reste_du` | `max(0, tarif − réglé)` si facturable ; sinon 0. |
| `est_echue` | 1 si la fin de la séance est passée (à défaut le début ; sans heure, à partir du lendemain) — même règle que `SeanceListeItem.EstPassee` (`EstMomentPasse`). |
| `montant_avoir` | Séance non due : tout le réglé ; séance due : `max(0, réglé − tarif)`. |

Lue par :

- `QuerySeances` (liste par dossier ; planning du jour et séances d'une période, `SelectSeancesPlanning`),
- `QueryPaiements` (lignes de paiement par dossier, par séance et par période, `SelectPaiementsBase` ; contrôle d'un encaissement ; synthèse de la règle unique du dû ; séances au dû désaligné),
- `QueryPatients` (grille Patients : montant dû, avoir, prochaine et dernière séance).

**`v_dossier_dominant`** — une ligne par patient qui a au moins un dossier : `id_patient`, `id_dossier`. Ordre de choix : dossier vivant (`ACTIF`, `PAUSE`) d’abord, puis celui dont une séance est la plus proche d’aujourd’hui, puis la date d’ouverture la plus récente, puis l’identifiant le plus grand.

Lue par : `QueryPatients` (statut et domaine de la colonne « Suivi » et des filtres), `QueryDossiers.SelectIdDossierDominant` (aperçu du patient, `GestionDossiers.GetDossierDominant`).

## Diagrammes et maintenance

Les anciens diagrammes exportés (`Docs/Database/Diagrams/`, 14/06/2026, 25 tables sur 45) ont été supprimés le 18/10/2026 (UI-8) : ils étaient périmés. La référence du schéma est ce dictionnaire, avec la sauvegarde de structure (`backup_NoData_althea.sql`) et les migrations. De nouveaux diagrammes pourront être régénérés depuis la base au besoin.

Avant toute évolution :

1. vérifier la dernière version dans `Migration/` et dans `tec_meta_schema` ;
2. documenter la décision et prévoir le rollback ;
3. conserver les règles variables dans les référentiels et les liaisons datées ;
4. mettre `VersionSchema.Attendue` (`Core/Database/VersionSchema.vb`) à la version de la migration ;
5. appliquer la migration, régénérer les sauvegardes, lancer `Tools\test.cmd` (le test de fumée prépare toutes les requêtes contre la nouvelle structure) et mettre à jour ce document.

### Base de test `althea_tests` (tests automatisés, ADR-58)

- Base **vide** dédiée au test de fumée des requêtes : aucune donnée, même fictive. Créée une fois par poste avec son compte (`Maintenance/creer_base_tests.sql`) ; chaîne de connexion dans la variable d'environnement `ALTHEA_TESTS_CONNEXION`.
- À chaque série de tests, sa structure est recréée depuis `backup_NoData_althea.sql` : une sauvegarde de structure qui ne se recharge plus dans une base neuve fait échouer les tests.
- Pour la recharger ailleurs que dans `althea`, les tests retirent la base citée dans les valeurs par défaut des séquences (`` DEFAULT nextval(`althea`.`seq_…`) ``, 15 tables) et la clause `DEFINER` des vues. **À traiter pour l'installation** (ToDo) : une base installée sous un autre nom garderait ces références à `althea`.

Références associées : [Concepts métier](Concepts_Metier_Althea.md), [Paramètres et référentiels](../Architecture/Parametres_Referentiels.md), [Décisions d’architecture](../Rules/ARCHITECTURE_DECISIONS.md).







> ---
>
> **Contact** : ***Joëlle De Poorter - Les Artefacts de Manou***
>
> Projet réalisé pour soutenir une pratique de psychologie et de graphologie dans la gestion fiable et évolutive des patients et documents.
>
> - Site web Auteur/Les Artefacts de Manou/Joëlle De Poorter : https://lesartefactsdemanou.be
>
> - Site web Psychologue & Graphologue:  https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu` et `lesartefactsdemanou@gmail.com`
>
> - GitHub privé: Althea    https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea
>
> ---





[TOC]

