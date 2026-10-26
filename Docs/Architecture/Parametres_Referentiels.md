# Althéa — Paramètres et référentiels

> Référence technique détaillée des données configurables et de leur administration.
> Dernière mise à jour : 25/10/2026 — lignes système devenues inutiles déverrouillées, v2.58.0

<details>
<summary>Historique des mises à jour</summary>

- 24/10/2026 — règles des statuts de séance et des contacts portées par des cases, v2.57.0
- 24/10/2026 — codes postaux belges en référentiel, v2.56.0
- 23/10/2026 — spécialités des thérapeutes en référentiel, v2.55.0
- 22/10/2026 — référentiels : droit d'écriture et lignes système vérifiés par le métier et le SQL, audit C.1
- 20/10/2026 — correcteur d'orthographe, v2.44.0
- 19/10/2026 — textes types, v2.42.0 et v2.43.0 ; mention de confidentialité des documents, v2.41.0
- 16/10/2026 — groupes des paramètres, écran Paramètres sur une page
- 13/10/2026 — lien vers les concepts métier corrigé
- 12/10/2026 — lot 30 : paramètres lus par le code, bornes, type PATH

</details>

## Objectif

domaines (conventions), organismes, tarifs, natures de séance, catégories et règles de quota, statuts et données de listes.

## Deux familles de configuration

| Famille | Stockage | Usage |
|---|---|---|
| Paramètres applicatifs | `tec_parametres` | Configuration technique ou fonctionnelle globale. |
| Référentiels métier | `ref_*` et `lia_*` | Valeurs, comportements et liaisons applicables au métier. |

## Paramètres applicatifs

`UC_Parametres` et `GestionParametres` gèrent les valeurs de `tec_parametres` sous contrôle de rôle.

- **Un paramètre naît par migration** (clé, libellé, groupe, type, bornes, description, activation, ouverture au SuperUser) ;
- l'écran n'en modifie que la valeur — Admin : tous, SuperUser : paramètres ouverts, droit revérifié dans la requête (ADR-36, 12/10/2026).
- L'application ne supprime aucun paramètre.

**Groupes** (v2.40.0) : chaque paramètre appartient à un groupe déclaré dans `tec_groupes_parametres` (code, titre, ordre ; clé étrangère).

- L'écran Paramètres montre **une carte par groupe**, dans l'ordre des groupes, chaque paramètre sur une ligne avec sa description en aide ;
- un seul « Modifier » ouvre toute la page et « Enregistrer » écrit les valeurs changées **en une transaction** (`GestionParametres.UpdateValeursParametres`, tout ou rien).
- Un nouveau groupe naît par migration, comme un paramètre.

Les chemins documentaires sont stockés sous forme de segments logiques et composés côté code par les helpers de chemins.

### Paramètres lus par le code (lot 30, v2.37.0)

Les clés et les valeurs de secours sont centralisées dans `ClesParametres` (`Metier/Parametres`) ; le code lit les valeurs par `GestionParametres.GetEntier` / `GetBooleen` / `GetTexte` (une valeur texte vide est rendue telle quelle : c'est un choix).

- Base injoignable, paramètre absent, inactif ou invalide : valeur de secours **journalisée** (catégorie Database), l'écran reste utilisable.
- Une valeur hors bornes est ramenée dans les bornes.

| Clé | Type | Défaut | Bornes | Groupe (carte) | Visible SuperUser | Utilisé par |
|---|---|---|---|---|---|---|
| `SEANCE_DUREE_DEFAUT_MIN` | INT | 45 | 10 – 240 | SEANCES | oui | `GestionSeances.DureeSeanceParDefaut` (fin proposée dans `SeanceEditor`, `DialogReportSeance`) |
| `SEANCE_PAS_HORAIRE_MIN` | INT | 15 | 5 – 60 | SEANCES | oui | `GestionSeances.CreneauxProposes` (listes d'heures de `SeanceEditor`) ; réduit automatiquement s'il ne divise pas la durée |
| `SEANCE_HEURE_DEBUT_GRILLE` / `SEANCE_HEURE_FIN_GRILLE` | INT | 8 / 20 | 0 – 23 / 1 – 23 | SEANCES | oui | Plage des listes d'heures ; plage par défaut si la première n'est pas avant la dernière |
| `SECURITE_MAX_ECHECS_LOGIN` | INT | 5 | 3 – 20 | SECURITE | non | `GestionAuthentification` (verrouillage du compte) ; aussi les mots de passe faux d'une élévation (`GestionUtilisateurs.VerifierElevationUtilisateur`, 20/10/2026) |
| `SECURITE_ELEVATION_MINUTES` | INT | 15 | 1 – 240 | SECURITE | non | `Home` : fin de l'élévation après ce temps sans activité (clavier, clic, molette) ; v2.48.0 |
| `LOG_RETENTION_JOURS` | INT | 7 | 1 – 90 | TECHNIQUE | non | `AppStartupManager` → `GestionLog.AppliquerRetention` (purge au démarrage, une fois la base jointe) |
| `CACHET_ENCRE_NOIRE` | BOOL | 0 (bleu nuit) | — | CABINET | oui | `GenerateurCachet` |
| `DOCUMENT_MENTION_CONFIDENTIALITE` | STRING | Confidentiel (vide = pas de mention) | — | DOCUMENTS | oui | `EnTeteDocument.Creer` : pied de page des documents (impression, aperçu, PDF, Word ; v2.41.0) |
| `EDITEUR_CORRECTEUR_ORTHOGRAPHE` | BOOL | 1 (actif) | — | EDITEUR | oui | `UC_RichTextEditor` (chargement) : correcteur de Windows en saisie (v2.44.0, ADR-45) |
| `DOSSIER_MODALITE_PAIEMENT_DEFAUT` | STRING | « À la séance » | — | DOSSIERS | oui | `UC_DossierFiche` (création) : texte proposé dans « Modalités de paiement », modifiable ; vide = rien (v2.46.0) |
| `SAUVEGARDE_DOSSIER` | PATH | vide (pas de secours) | — | SAUVEGARDE | oui | `GestionSauvegardes` : dossier des archives ; refusé dans le dossier de données (`ValiderDossierSauvegarde`, aussi dans l'écran Paramètres) |
| `SAUVEGARDE_NB_CONSERVEES` | INT | 10 | 1 – 100 | SAUVEGARDE | oui | Rotation des archives après une sauvegarde réussie |
| `SAUVEGARDE_RAPPEL_JOURS` | INT | 7 | 1 – 90 | SAUVEGARDE | oui | Rappel à l'ouverture (`Home_Shown`) |
| `SAUVEGARDE_DOSSIER_MARIADB` | PATH | vide | — | SAUVEGARDE | non | Dossier `bin` de MariaDB, recours si `mariadb-dump` n'est pas trouvé dans Program Files |
| `PATH_GENERAL` | PATH | — (pas de secours, obligatoire) | — | PATHS | oui | `ArborescenceDonnees.GetRacineDonnees` (racine du dossier de données) ; le modifier ne déplace pas les fichiers |

**Bornes** : colonnes `valeur_min` / `valeur_max` (v2.37.0), appliquées au champ numérique de `UC_Parametres` et à la lecture. Elles se règlent par SQL (pas dans l'écran).

**Type PATH** : petit bouton dossier (Parcourir) à droite du champ en modification, chemin complet obligatoire (`UtilsValidation.IsValidPathValue`), création du dossier proposée s'il n'existe pas.

**Valeur unique** (ADR-36) : un paramètre n'a qu'une valeur, remplacée à la modification, sans dates de validité ; l'écran rappelle après chaque modification que la valeur s'applique aux prochaines opérations et que les données enregistrées ne changent pas.

**Dossier de données** (ADR-35) :

- seule la racine `PATH_GENERAL` est un paramètre ; les sous-dossiers (`Documents`, `Cabinet`, `Utilisateurs`) sont fixes (`ArborescenceDonnees`).
- Les journaux vivent dans `%APPDATA%\Althea\Logs`, car ils servent avant la connexion à la base.
- Le dossier des sauvegardes est le paramètre `SAUVEGARDE_DOSSIER` (lot 30, P2), refusé à l'intérieur du dossier de données.

**Jeu de référence** : `Docs/Database/Maintenance/reinit_tec_parametres.sql` efface la table, remet la séquence à 1 et recrée les seuls paramètres lus par le code (valeurs par défaut ; la racine `PATH_GENERAL` et les dossiers `SAUVEGARDE_DOSSIER` / `SAUVEGARDE_DOSSIER_MARIADB` déjà saisis sont conservés).

Il sert de réinitialisation en développement et de base aux futurs scripts d'installation : tout nouveau paramètre lu par le code s'ajoute dans une migration **et** dans ce script.

**Suppressions** : l'application ne supprime jamais un paramètre. Exception assumée : la migration v2.37.0 a retiré les paramètres de test (`TESTONG`, `TESTONG2`, `APP_DATA`) et les chemins obsolètes (`PATH_DOCUMENT`, `PATH_DOSSIER`, `PATH_LOG`, `PATH_BACKUP`) — accord du 12/10/2026, recréés par le ROLLBACK.

## Référentiels généraux

Les écrans généraux comprennent notamment les organismes, liens patient, rôles intervenant, situations familiales, statuts de dossier/séance, types de document, types de rendez-vous, rôles légaux, pays et civilités.

Certaines lignes de référentiel sont marquées **système** (`est_systeme = 1`, cadenas dans la grille ; ex. organisme `INAMI`, pays dotés de règles de validation, types d'identifiant `INAMI`, `BCE`, `TVA`) : elles sont utilisées par le code.

- Leur code ne peut être modifié et elles ne peuvent être ni désactivées ni supprimées (ADR-28) ; le type d’un organisme système est verrouillé de la même façon.
- Le libellé, l’ordre et les champs descriptifs restent modifiables (décision du 11/10/2026).
- Le marquage se fait uniquement par SQL ; une future remise à vide de la base devra conserver ces lignes.
- Une ligne cesse d'être système quand plus aucune règle ne la cherche par son code : v2.58.0 (25/10/2026) pour les statuts de séance Absence, Annulées, Maladie, le lien Mère et le rôle Autorité parentale, réglés par des cases depuis la v2.57.0.

Depuis le 22/10/2026 (audit C.1, ADR-28 évolution), cette protection est écrite dans les requêtes elles-mêmes (modification et désactivation) et le métier refuse explicitement une écriture qui n'a concerné aucune ligne.

**Règles portées par des cases** (ADR-54) : plutôt que de reconnaître une ligne par son code, l'application lit une case du référentiel ; un élément ajouté se règle donc comme les autres.

| Référentiel | Case | Effet |
|---|---|---|
| Statuts de séance | Séance due (`est_du`), Consomme le quota (`consomme_quota`, v2.24.0), Absence non excusée (`est_absence`, v2.53.0) | Montant dû, quotas, no-show (tarif, compteur d'absences). |
| Statuts de séance | Non tenue, libère le créneau (`est_non_tenue`, v2.57.0) | Créneau libre au planning, pas de contrôle de chevauchement. Départ : annulées, Reportée, Maladie. |
| Statuts de séance | Proposé à la création d'une séance (`propose_creation`, v2.57.0) | Liste des statuts d'une nouvelle séance. Départ : Planifiée (obligatoire) et Réalisée. |
| Liens patient, rôles légaux | Par défaut : proposé pour un nouveau contact (`par_defaut`, v2.57.0) | Présélection dans la fenêtre Contact ; une seule valeur par référentiel. Départ : Mère, Autorité parentale. |

**Droit d'écriture** : SuperUser pour tous les référentiels (`ReglesReferentiels.ROLE_ECRITURE`, une seule définition, lue par `UC_ReferentielBase.RoleMinimum` et vérifiée par chaque méthode d'écriture du métier).

`ReferentielModalHost` permet l’ajout ponctuel d’une valeur depuis un écran métier, sans quitter le contexte courant, lorsque l’utilisateur possède le droit requis ; un User est invité à s'élever (ouvrir l'ajout aux Users : à cadrer, ToDo).

## Référentiels métier spécialisés

| Sujet | Source de vérité | Administration |
|---|---|---|
| Domaines (conventions) | `ref_domaines` | `UC_DomainesHome` (menu Domaines de la Home ; plus de tuile dans le hub Référentiels depuis le 20/10/2026) |
| Organismes de remboursement et mutualités | `ref_organismes` (`type_organisme` : `REMBOURSEMENT` pour les domaines, `MUTUALITE` pour les patients) | `UC_Organismes` (+ ajout rapide depuis `UC_DomainesHome`, qui ne propose que les organismes de remboursement) |
| Pays (code ISO 3166-1 alpha-2) | `ref_pays` ; règles de validation dans `UtilsTelephone` / `UtilsCodePostal`, indexées par code ISO | `UC_Pays` (colonne « Contrôles » : règles disponibles ou non) |
| Codes postaux belges (v2.56.0, ADR-53) | `ref_codes_postaux_be` : codes et noms de localité acceptés, un nom proposé par code ; contenu généré depuis bpost et le registre BeST, lecture seule | Aucun écran : mise à jour par le script `Docs/Database/Maintenance/codes_postaux_be/generer_codes_postaux_be.py` (procédure : `Database_technique.md`) |
| Civilités | `ref_civilites` (libellé + abréviation pour les courriers) | `UC_Civilites` |
| Types d'identifiant professionnel (v2.29.0, v2.47.0) | `ref_types_identifiant_pro` ; contrôles INAMI, BCE, TVA dans `UtilsIdentifiantBelge` (par le code), expression de contrôle pour les autres | `UC_TypesIdentifiantPro` (hub Administration, SuperUser, ADR-31) ; liste du type dans l'onglet Identifiants de `UC_Cabinet` (types actifs, plus le type désactivé d'une ligne existante) |
| Spécialités des thérapeutes (v2.55.0) | `ref_specialites` | `UC_Specialites` (groupe Cabinet du hub Référentiels) ; liste et bouton [+] de la fiche Thérapeute (`UtilsSaisieReferentiels.ChargerSpecialites`) ; une seule par thérapeute |
| Latéralités (v2.46.0) | `ref_lateralites` | `UC_Lateralites` (groupe Patient du hub Référentiels) ; liste de la fiche patient (`UtilsSaisieReferentiels.ChargerLateralites`) |
| Textes types (v2.42.0, v2.43.0) | `ref_textes_types` (contenu RTF + texte) ; usages `ref_usages_textes_types` (lignes système, codes dans `UsagesTexteType`) ; liaison `lia_textes_types_usages` (plusieurs usages par texte ; aucun = partout) | `UC_TextesTypes` et `UC_UsagesTextesTypes` (libellé et ordre des usages, sans création) — tuiles du groupe Dossier ; insertion par le bouton « Textes types » des éditeurs (`UC_RichTextEditor.UsageTextesTypes`) |
| Natures de séance | `ref_natures_seance` | `UC_NaturesSeance` |
| Prix de séance | `ref_prix_seance` | `UC_PrixSeance` |
| Modes de paiement | `ref_modes_paiement` | `UC_ModesPaiement` |
| Tarifs par domaine | `lia_prix_seance_domaines` | `UC_LiaisonsPrixDomaines` (onglet Tarifs) |
| Natures autorisées par domaine + catégorie de quota consommée | `lia_natures_seance_domaines` | `UC_LiaisonsNaturesDomaines` (onglet Natures) |
| Catégories de quota d’un domaine | `ref_categories_quota` | `CategoriesQuotaEditor` (onglet Catégories & quotas) |
| Règles de quota datées | `ref_quotas` | `UC_DomainesHome` — onglet Catégories & quotas |
| Plafonds par catégorie et par règle | `ref_quotas_plafonds` | `UC_DomainesHome` — grille des plafonds |

Un domaine représente une **convention** : organisme facultatif, tarifs datés, natures autorisées et quotas. Les règles de quota ne se modifient jamais : ajouter une nouvelle règle clôture automatiquement la précédente la veille de sa date de début.

Les relations domaine–tarif sont historisées par période de validité. Les comportements d’un domaine sont explicitement séparés (`tarif_convention`, `no_show_facturable`, `soumis_quota`) : aucun booléen générique ni code de domaine ne doit remplacer ce modèle.

## Architecture

```text
UC_DomainesHome, UC spécialisé ou UC_ReferentielBase
	↓
Gestion<X> / GestionLiaisons<X> / GestionQuotasInami / GestionCategoriesQuota / GestionOrganismes / GestionPays / GestionCivilites
	↓
Query<X>
	↓
ref_* / lia_* / tec_parametres
```

## Règles de maintenance

- Toute nouvelle règle variable doit être modélisée dans les référentiels ou liaisons appropriés.
- Une valeur évolutive doit être datée ; une donnée dérivable doit être calculée.
- Tout référentiel nouveau doit prévoir son administration et un repli journalisé lorsqu’il n’est pas alimenté.
- Consulter [Concepts_Metier_Althea.md](Concepts_Metier_Althea.md) pour le sens métier de ces données et [Rules.md](../Rules/Rules.md) pour les règles obligatoires.





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

