# Althéa - Concepts métier essentiels

> Référence métier détaillée, complément du README destiné aux développeurs et repreneurs.
> Dernière mise à jour : 26/10/2026 — quota : le calcul est vérifié par des tests automatisés (ADR-58)

<details>
<summary>Historique des mises à jour</summary>

- 24/10/2026 — séance non tenue : créneau libéré selon le référentiel, v2.57.0
- 23/10/2026 — recherche transversale ; règles de saisie et d'action définies dans le métier
- 23/10/2026 — domaines : saisie et complétude jugées par le métier
- 23/10/2026 — spécialité d'un thérapeute choisie dans un référentiel
- 23/10/2026 — contacts, intervenants et thérapeutes : règles dans le métier, homonyme de thérapeute confirmé
- 23/10/2026 — patient : règles de la fiche dans le métier, NISS normalisé
- 22/10/2026 — dû et prévu distingués, bandeau du dossier
- 22/10/2026 — séances et paiements : règles des actions, chevauchement et encaissement vérifiés par le métier, audit C.2
- 22/10/2026 — dossier archivé : fiche comprise ; montant libre conservé ; règles vérifiées par le métier, audit C.1
- 22/10/2026 — paiements : mode par identifiant, annulation et avoirs tracés en colonnes, ADR-49
- 21/10/2026 — reste dû, avoir et dossier dominant : une seule définition, vues SQL, ADR-48
- 17/10/2026

</details>

Ce document est la source de vérité du parcours **Patient → Domaine → Dossier → Séance → Tarif / Quota → Paiement**. Il décrit le sens des données et les règles fonctionnelles ;

- le détail des tables est dans [Database_technique.md](Database_technique.md),
- les décisions et leur justification dans [ARCHITECTURE_DECISIONS.md](../Rules/ARCHITECTURE_DECISIONS.md),
- les écrans dans [Documentation_technique_UI_Althea.md](Documentation_technique_UI_Althea.md).

> 🗂️ **Historique du document** : entre le 26/09 et le 13/10/2026, ce fichier contenait par erreur une copie de `ARCHITECTURE_DECISIONS.md`.
> Il a été reconstitué le 13/10/2026 à partir de sa dernière version saine (25/09) et mis à jour (domaine = convention, quotas par catégories, cohérence séances ↔ paiements, organismes et mutualités, pays et civilités, cabinet).

---

## Règle structurante : pilotage par les référentiels

**Principe** : toute règle métier variable vit dans un référentiel en base de données, **jamais en dur dans le code**. Un repreneur doit pouvoir adapter l'application à un nouveau besoin par paramétrage, sans recompiler.

**Exemples** :
- ✅ Les comportements d'un domaine sont des attributs explicites (`tarif_convention`, `no_show_facturable`, `soumis_quota`) et des liaisons datées.
- ❌ `Const DOMAINE_PSY = "PSY"` servant de pivot à une règle.
- ✅ Les tarifs vivent dans `ref_prix_seance` et sont rattachés aux domaines par période.
- ❌ `Const TARIF_DEFAUT = 50.0`.
- ✅ Les quotas sont des règles datées, avec des catégories et des plafonds paramétrés par domaine.
- ❌ `Const MAX_SEANCES_INAMI = 60`.

**Lignes système** : certains codes sont exploités par l'application (statuts, modes de paiement, organisme INAMI, pays dotés de règles…). Ils portent `est_systeme`, posé uniquement par migration : code, activation et suppression verrouillés, libellé et ordre modifiables (ADR-28).

**Règles de saisie et d'action** (ADR-51, audit C.2, terminé le 23/10/2026) : chaque règle se définit une fois, dans le métier.

- Un valideur rend la liste des erreurs par champ ; la fenêtre les affiche toutes d'un coup (icône sur chaque champ et fenêtre récapitulative), l'enregistrement applique la même règle et refuse une saisie invalide, quel que soit l'écran qui écrit.
- Ce qu'un élément permet (reporter, supprimer, encaisser, statuts proposés) est aussi une règle du métier, lue par les boutons.
- Restent à l'écran les confirmations (doublon possible, dépassement de quota) et les aides de saisie.

---

## Patients

**Définition** : personne suivie au cabinet. Elle porte son identité, ses coordonnées, sa situation administrative, une alerte, l'anamnèse et une photo d'identité.

**Règles** :
- Un patient a de 0 à N dossiers, chacun dans un domaine.
- Le NISS est unique et contrôlé selon son type (Belgique, France, Luxembourg ; « Autre » sans contrôle de format) ;
  - il est enregistré sous une seule forme, sans séparateurs (v2.54.0), pour que l'unicité joue quelle que soit la façon de le taper, et affiché mis en forme selon son type (« 85.07.30-033.28 »).
  - Le type n'est pas enregistré : il est déduit du numéro au chargement.
  - Un doublon nom + prénom + date de naissance est **signalé** (confirmation), pas refusé : deux homonymes nés le même jour existent.
- Les règles de la fiche (nom et prénom obligatoires, téléphone et code postal selon le pays, e-mail, NISS, CT1 / CT2) sont celles du métier (`GestionPatients.ValiderPatient`, audit C.2) : la fiche les affiche, l'enregistrement les applique.
- **Civilité** et **pays** sont choisis dans des référentiels (`ref_civilites`, `ref_pays`). Les contrôles de code postal et de téléphone dépendent du **code ISO** du pays, jamais de son libellé ; un pays ajouté par l'interface est accepté sans contrôle de format.
- La **mutualité** est un organisme de type `MUTUALITE` (`ref_organismes`), complétée par le numéro d'affiliation et les codes titulaire CT1 / CT2.
- L'**activation** (`is_actif`) est manuelle et indépendante du cycle de vie des dossiers. Le **suivi en cours** n'est pas stocké : il est dérivé de l'état du dernier dossier.
- Les fichiers du patient (photo, exports) sont rangés sous `{PATH_GENERAL}\Documents\{code_patient}` ; la base ne stocke que des noms de fichiers (ADR-16, ADR-20).

**Réseau du patient** :
- **Famille et contacts** (`famille_contacts`) : lien avec le patient (`ref_liens_patient`) et rôle légal (`ref_role_legal`), tous deux obligatoires ; nom et prénom obligatoires.
- **Intervenants** (`autres_suivis_patient`) : réseau pluridisciplinaire, avec un rôle facultatif (`ref_roles_intervenant`) ;
  - un intervenant est toujours un **thérapeute** enregistré (`therapeutes`), réutilisable comme thérapeute traitant d'un dossier.
  - Le suivi garde une trace figée de son identité (nom, spécialité, localité) ; ses coordonnées (téléphone, e-mail, adresse) sont affichées telles qu'elles sont aujourd'hui.
  - Une intervention est **terminée** quand sa date de fin est passée ; la fin ne précède jamais le début.
- **Thérapeutes** : nom obligatoire ; spécialité facultative, **une seule**, choisie dans le référentiel `ref_specialites` (v2.55.0) ; deux thérapeutes peuvent porter les mêmes nom et prénom — l'homonyme est **signalé** (confirmation), pas refusé.
- Ces règles, avec les formats (téléphone et code postal selon le pays, e-mail), sont celles du métier (`ValiderContact`, `ValiderSuivi`, `ValiderTherapeute`, audit C.2) : les fenêtres les affichent, l'enregistrement les applique.

---

## Domaines (`ref_domaines`) — un domaine est une convention

**Définition** : un domaine est le cadre dans lequel un patient est suivi, et il représente une **convention** : un tarif, éventuellement un organisme de remboursement, éventuellement des quotas. Un nouvel organisme de remboursement se traduit par un **nouveau domaine** (ADR-27).

**Comportements portés par le domaine** :
- `tarif_convention` : le tarif est celui de la convention (liaisons domaine–tarif datées) plutôt que le tarif patient du dossier ;
- `no_show_facturable` : une absence non excusée reste due ;
- `soumis_quota` : les séances sont décomptées selon les règles de quota du domaine.

**Autres rattachements** :
- **Organisme** (`id_organisme`, facultatif) : organisme de type `REMBOURSEMENT`. Aucun comportement n'est déduit de son code ; l'organisme porte ses coordonnées, sa plateforme, sa procédure d'encodage et sa périodicité (`ref_periodicites`).
- **Tarifs** : `lia_prix_seance_domaines`, N-N et daté.
- **Natures autorisées** : `lia_natures_seance_domaines` ; la liaison indique aussi la catégorie de quota consommée par la nature **dans ce domaine**.

**Dossier dominant** (statut et domaine de la grille Patients, aperçu du patient) : un dossier vivant (Actif ou En pause) d'abord, puis celui dont une séance est la plus proche d'aujourd'hui, puis le plus récemment ouvert (décision de Joëlle, 21/10/2026). Une seule définition, la vue `v_dossier_dominant` (ADR-48).

**Règles** (métier, audit C.2) : code de 3 lettres et libellé, tous deux uniques ; le quota ne se retire pas tant que des règles de quota sont actives (`GestionDomaines.ValiderDomaine`).

Un domaine est **complet** quand rien ne manque pour l'utiliser sans surprise —

- tarif en vigueur s'il est conventionné ;
- catégorie, règle en vigueur et aucune période sans règle s'il est soumis à quota ;
- au moins une nature active, dont une qui consomme le quota s'il y en a un (`GestionDomaines.CalculerManques`, lu par l'écran et sa synthèse).

**Écran de référence** : `UC_DomainesHome`, parcouru en cinq étapes (Général / Tarifs / Catégories & quotas / Natures / Synthèse) qui indiquent chacune ce qui reste à faire ; guide : [Guide_Utilisateur_Domaines.md](../Guide/Guide_Utilisateur_Domaines.md).

---

## Dossiers (`dossiers`, `ref_statuts_dossier`)

**Définition** : contexte de suivi d'un patient dans un domaine. Il regroupe les zones cliniques, les séances, les documents et les paiements.

**Cycle de vie** (transitions autorisées par une liste blanche dans `GestionDossiers`) :

| Statut courant | Transitions possibles |
|---|---|
| Actif | En pause, Clôturé |
| En pause | Actif, Clôturé |
| Clôturé | Actif (réouverture), Archivé |
| Archivé | aucune (terminal) |

**Règles essentielles** :
- Un dossier est toujours créé **Actif** ; son code est généré par la base (ADR-22).
- Un dossier clôturé a ses **séances et paiements figés** (consultation seulement) ; sa fiche et ses zones cliniques restent modifiables (note de fin de suivi). Pour planifier ou encaisser à nouveau, on le rouvre.
- **Clôture** (décision du 17/10/2026, `GestionDossiers.ControlerCloture`) :
  - **refusée** tant qu'une séance due est encore **à venir**, payée d'avance ou non : il faut d'abord l'annuler (un report recréerait une séance à venir dans le même dossier) ; un paiement déjà reçu devient alors un avoir du patient, utilisable dans un autre dossier ;
  - **montants à régler** (séances passées non soldées) : signalés dans la confirmation, la clôture reste possible (« Clôturer quand même ») — une dette peut ne jamais être payée, la décision revient à l'utilisatrice ;
  - la **mise en pause** reste toujours possible ; l'archivage ne part que d'un dossier clôturé, il est donc couvert par le contrôle de la clôture.
- Un dossier archivé est entièrement en lecture seule, fiche du dossier comprise (décision de Joëlle, 22/10/2026), et ne peut pas être rouvert.
- Ces règles sont vérifiées par le métier à chaque écriture, d'après le statut en base (audit C.1) ; l'écran grise les boutons et en donne la raison.
- L'état se voit dans le bandeau du dossier : pastille de statut colorée (actif vert-de-gris, en pause ocre, clôturé / archivé gris foncé) et, pour un dossier clôturé ou archivé, bandeau « consultation » sous l'en-tête.
- Les **7 zones cliniques** (motif, historique, antécédents, diagnostic, traitements, objectifs, historique archivé) sont enregistrées en double format RTF + texte ; l'historique archivé est protégé en écriture.
- Le tarif patient du dossier (`id_prix_seance`) sert aux domaines non conventionnés ; le thérapeute traitant est facultatif.

---

## Séances (`seances`, `ref_statuts_seance`, `ref_natures_seance`)

**Définition** : unité de suivi facturable, liée obligatoirement à un dossier, créée dès la planification du rendez-vous (D-17).

**Statuts** : pilotés par `ref_statuts_seance` (Planifiée, Réalisée, Annulée par le patient ou par le praticien, Maladie, No-show, Reportée…). Trois attributs du référentiel portent les règles :
- `est_du` : la séance est due (Planifiée, Réalisée… ; pas Reportée, Annulée…) ;
- `consomme_quota` : la séance est décomptée dans les quotas ;
- `est_absence` (22/10/2026, audit B.6, décision de Joëlle) : absence non excusée du patient (no-show) — non facturée dans un domaine qui ne facture pas les absences, comptée dans les absences, jamais prise comme « dernière séance ». Cochée pour « Absence non excusée » ; aucun code de statut n'est plus écrit dans les règles.

> **Séance passée non confirmée — « À confirmer »** (21/10/2026, ADR-47, décision de Joëlle)
> Une séance `Planifiée` dont l'heure est dépassée **reste `Planifiée` en base** (plus de bascule automatique en `Réalisée`) et s'affiche **« À confirmer »** (pastille ocre, `SeanceListeItem.EstAConfirmer`).
> Son issue se pointe dans la fenêtre Séance (Réalisée, absence, report…) ; l'Agenda offrira plus tard un **pointage** groupé, appuyé sur les widgets de la page d'accueil.
> Elle reste due (facturation et quotas inchangés) ; le verrou d'une séance encaissée ne s'active que sur le statut **réel** `Réalisée` ; elle peut encore être reportée.

**Report** : seule une séance `Planifiée` se reporte, et seulement par le bouton « Reporter » (le statut `Reportée` n'est jamais choisi dans la fenêtre Séance ; une nouvelle séance est enregistrée planifiée ou réalisée).

- Une séance reportée passe en lecture seule et n'est plus supprimable ;
- la nouvelle séance garde le lien vers l'origine (`id_seance_origine`) et recopie tarif et montant (le tarif dépend du patient, pas de la date).

**Contrôles** :
- **Non-chevauchement** bloquant (un seul thérapeute), à l'enregistrement comme au **report** (nouvelle séance) ; le métier le vérifie dans la transaction de l'écriture (audit C.2). Une séance dont le statut est coché « Non tenue, libère le créneau » dans le référentiel (annulées, Reportée, Maladie au départ ; v2.57.0) n'occupe pas son créneau ; une absence non excusée l'occupe. Passer d'un statut non tenu à un autre ne le contrôle pas. Le planning du jour est une aide visuelle, pas le garant de la règle.
- **Montant ≠ tarif** : blocage ; le montant ne se modifie que par le cadenas (SuperUser / Admin). Un montant déjà enregistré et laissé tel quel (prix d'ami, ancien tarif figé) est conservé quand un utilisateur modifie autre chose (22/10/2026).
- **Quota dépassé** ou **tarif changé sur une séance encaissée** : avertissement bien visible, la décision reste à l'utilisateur (ADR-26).

---

## Tarifs (`ref_prix_seance`, `lia_prix_seance_domaines`)

**Règles** :
- Les tarifs sont des données de référentiel ; aucun montant ni code de tarif ne pilote une règle dans le code.
- Le lien domaine–tarif est N-N et daté ; une révision ouvre une nouvelle période sans modifier le passé. Le non-chevauchement des périodes est contrôlé par la couche métier.
- Ordre de résolution (`GestionTarification`) : statut non dû → 0 ; no-show non facturable → 0 ; tarif de la convention si le domaine est conventionné ; sinon tarif patient du dossier (l'éditeur propose le tarif par défaut du patient).
- Le montant résolu est **figé** dans `seances.tarif_seance` : modifier un référentiel ne change jamais une séance enregistrée.
- Les natures conventionnées sont nommées avec le préfixe `INAMI -` pour limiter les erreurs de sélection.

---

## Quotas (`ref_quotas`, `ref_categories_quota`, `ref_quotas_plafonds`)

**Règles** (ADR-27, ADR-40) :
- Un quota ne s'applique qu'à un domaine `soumis_quota`.
- Chaque domaine définit ses **catégories** de quota, en nombre libre (ex. Soutien, Traitement) ; un même libellé dans deux domaines donne deux compteurs indépendants.
- Une **règle de quota** est datée : durée de la période glissante (en mois), seuil d'alerte, et un **plafond par catégorie**. Le total est calculé.
- La catégorie consommée par une nature est portée par la **liaison nature–domaine** (NULL = hors quota) : une même nature peut compter différemment selon la convention.
- Seules les séances dont le statut `consomme_quota` sont décomptées.
- **Un seul calcul** (22/10/2026, audit B.6) :
  - la 1re séance consommante ouvre une période (durée de la règle en vigueur à cette date) ; la 1re séance après son échéance ouvre la suivante (renouvellement de date à date).
  - L'onglet Séances, l'aperçu du patient et l'alerte de dépassement de la fenêtre Séance (qui ajoute la séance **à sa date**) partagent ce calcul (`GestionQuotaInami.CalculerQuota`).
  - La règle est lue d'après le domaine **actuel** du dossier.
  - Le calcul reçoit la règle par une fonction « règle en vigueur à telle date » : il est vérifié par des tests automatisés, sans base (`GestionQuotaInamiTests`, 26/10/2026, ADR-58).
- Ajouter une règle clôture automatiquement la règle ouverte à la veille de son début (les deux ensemble ou pas du tout, 21/10/2026). La date de début peut être passée (changement saisi en retard) : les compteurs n'étant jamais stockés, le décompte suit la date des séances.
- Une règle **a servi** dès qu'une séance du domaine tombe dans sa période de validité. Elle ne peut alors être **ni modifiée ni supprimée**, seulement clôturée ou désactivée, pour que l'historique des décomptes reste exact.
- Une règle **qui n'a pas servi** peut être corrigée ou supprimée ; supprimer une règle qui avait clôturé la précédente **rouvre** celle-ci.
- Une période couverte par aucune règle est signalée (dans la grille, l'étape et la synthèse) et n'est jamais comblée automatiquement : ses séances ne sont soumises à aucun quota tant qu'une règle intermédiaire n'est pas créée. Une période entre deux règles rend le domaine « à compléter ».
- Une catégorie utilisée ne peut être que désactivée.
- Changer la catégorie d'une nature déjà utilisée modifie le décompte des dossiers existants (avertissement affiché).

Les noms techniques `GestionQuotaInami`, `QuotaInamiRegle`… sont conservés pour l'historique ; ils traitent tout quota de domaine.

---

## Paiements (`paiements`, `lia_paiements_seances`)

**Définition** : mouvement financier rattaché à une séance par `lia_paiements_seances`.

- En V1, un paiement saisi couvre une seule séance ; une séance peut recevoir plusieurs paiements (règlements partiels).
- L'utilisation d'un avoir modifie ensuite la liaison (liaison source **diminuée**, et reliée au règlement par avoir dans `lia_paiements_avoirs` depuis le 22/10/2026, ADR-49) ;
- des réaffectations faites avant le 21/10/2026 ont pu **répartir** un paiement sur plusieurs séances (une liaison chacune).
- Les montants se lisent donc toujours dans `montant_affecte`.

**Statuts** : `EN_ATTENTE` (dû en attente), `PAYE` (encaissé), `ANNULE` (ligne conservée pour la trace).

**Vocabulaire** :

| Terme | Signification |
|---|---|
| Encaissé | Somme des paiements `PAYE` de la séance ; les lignes en attente ou annulées ne comptent jamais. |
| Reste dû | `max(0, tarif − encaissé)` si la séance est due et facturable, sinon 0 (une absence non facturable ne doit rien). Calculé, jamais stocké ; une seule définition, la vue `v_seances_solde` (ADR-48, 21/10/2026). |
| Dû (« à régler ») / prévu | Le reste dû d'une séance due est **dû** (à régler) une fois son heure de fin passée (sans heure : le lendemain) ; avant, il est **prévu** — un acompte peut le réduire, mais personne ne doit encore rien (`SeanceListeItem.MontantExigible` / `MontantPrevisionnel`). Le bandeau du dossier montre les deux, pour ce dossier et pour tous les dossiers du patient (« 0 € ici · 68,00 € tous dossiers », 22/10/2026). |
| Avoir | Tout l'encaissé resté sur une séance qui n'est plus due (annulée, maladie, reportée), ou la part encaissée au-delà du montant d'une séance encore due. Calculé, jamais stocké ; une seule définition pour le bandeau, les grilles, la grille Patients et la réutilisation (17/10/2026), tenue par la vue `v_seances_solde` depuis le 21/10/2026 ; réutilisable pour régler une autre séance du patient (mode de paiement `est_avoir`). |

**Règle unique** (ADR-26) : après **toute** action sur une séance ou un paiement, `GestionPaiements.ResynchroniserDuSeance` réaligne la ligne « dû en attente » sur le reste dû (création, mise à jour, réactivation ou annulation).

- Elle ne touche jamais aux paiements `PAYE`.
- Une séance a **au plus une** ligne de dû : les doublons éventuels sont annulés (21/10/2026).

**Traçabilité** :
- Aucun paiement n'est supprimé.
- L'annulation d'un encaissement est réservée au SuperUser / Admin, avec motif obligatoire, auteur et date. Elle est refusée pour un règlement par avoir et pour un encaissement dont une partie a déjà servi d'avoir.
- Un encaissement resté sur une séance devenue non due (report, annulation) est un avoir ; son utilisation est proposée aussitôt pour une séance due du dossier (21/10/2026, plus de réaffectation).
- Un **encaissement** n'est accepté que sur une séance due, non reportée et pas soldée, pour un montant strictement positif et au plus égal au reste dû, avec un mode de paiement (`GestionPaiements.ValiderEncaissement` ; le métier relit le reste dû dans sa transaction, audit C.2).
- Le statut d'un paiement et le solde de la séance sont deux informations **indépendantes**.

Pour chaque action et chaque situation de départ, le comportement attendu est détaillé dans le [Guide séances et paiements](../Guide/Guide_Seances_Paiements.md) (partie 2).

---

## Cabinet et praticien (`cab_*`)

**Définition** : identité légale du cabinet (situation juridique, BCE, TVA, IBAN / BIC, coordonnées, logo) et du praticien (identifiants professionnels datés INAMI, COMPY…, qualifications, photo, cachet).

**Règles** :
- **V1** : un cabinet = une installation = un praticien = un seul compte de travail SuperUser. Le schéma accepte déjà plusieurs praticiens.
- Le numéro **BCE** est saisi à un seul endroit, selon la situation juridique (cabinet ou praticien) (ADR-29).
- Le praticien est lié à son **compte de travail** (SuperUser) ; le compte Admin technique n'est jamais lié.
- Le **cachet** est généré à partir des identifiants et qualifications marqués « afficher sur le cachet », sauf si une image de cachet est fournie.
- Les images sont des fichiers ; la base ne stocke que des chemins relatifs.

---

## Recherche transversale (ADR-52)

**Définition** : retrouver un élément sans passer par son patient, puis y aller directement (décisions de Joëlle, 23/10/2026). Le bouton **Recherche** du menu, sous Patients, ouvre un hub : la recherche rapide par code en haut, une tuile par recherche dessous. Les recherches ne font que lire : aucune n'écrit en base ni n'ajoute de table.

**Recherche par code** : `PA` (patient), `DO` (dossier), `SE` (séance), `PAI` (paiement), `FC` (contact), `AS` (intervenant) — les codes générés par la base (préfixe + identifiant sur 6 chiffres).

- La saisie est tolérante (casse, espaces, zéros de tête : « se83 » = SE000083) ; le préfixe le plus long l'emporte (`PAI` avant `PA`).
- Le résultat dit ce qu'est l'élément (date, statut, domaine, montant, dossier) et de quel patient il relève ; un paiement est rattaché au dossier de sa première séance affectée.

**Séances d'une période** : les séances d'un jour ou d'une période, tous patients, dans l'ordre du calendrier, avec les pastilles, le filtre et le résumé de l'onglet Séances du dossier (par exemple les impayés du mois).

**Paiements d'une période** : le relevé de l'onglet Paiements du dossier, tous patients — encaissement à sa date, paiement attendu à la date de sa séance, annulation — avec un filtre par mode et les **totaux encaissés par mode**. Un règlement par avoir n'est pas de l'argent reçu : il est affiché à part, hors total.

**Ouverture ciblée** : un double-clic va à l'élément —

- patient : sa fiche ;
- contact ou intervenant : la fiche du patient, onglet Famille / Contacts ou Intervenants ;
- dossier : son hub ;
- séance ou paiement : le dossier, onglet Séances ou Paiements, la ligne sélectionnée.

« Ouvrir le patient » ouvre toujours sa fiche. Retour ramène à la recherche, critères et résultats compris (la ligne choisie est resélectionnée dans les listes de séances et de paiements).

**Confidentialité** : le journal ne reçoit que le type de recherche et le nombre de résultats, jamais ce qui a été cherché ni la période ; les critères ne vivent qu'en mémoire, le temps de la session ; une liste est plafonnée à 1000 lignes (au-delà, l'écran le dit et demande de réduire la période) ; les droits sont ceux des écrans ouverts.

Recherches prévues ensuite (ADR-52) : patients par lieu (R4), contact, thérapeute ou intervenant par son nom et leurs patients (R5).

---

## Écrans et grilles

La description des grilles (colonnes, filtres, codes couleur) relève de l'interface et vit dans la documentation technique UI, partie [patients et dossiers](UI/UI_Patients_Dossiers.md) :

- `UC_PatientHome` (recherche, filtre Suivi, domaine dominant),
- `UC_DossierFiche` (séances et paiements en grilles à pastilles, résumés cliquables, relevé chronologique des paiements).

L'utilisation des deux onglets est décrite dans le [Guide séances et paiements](../Guide/Guide_Seances_Paiements.md).

Le bandeau du dossier montre les montants dus du dossier (et le total du patient) et l'avoir **du patient**, qui sert dans tous ses dossiers ; la grille Patients résume tous les dossiers.





> ---
>
> **Contact** : ***Joëlle De Poorter - Les Artefacts de Manou***
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
