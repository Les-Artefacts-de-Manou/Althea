# Plan de tests — Tarification et quotas

> Version : 3.2 — Dernière mise à jour : 22/10/2026 — un seul calcul du quota, audit B.6 : TQ-22 à TQ-25
> Périmètre : référentiels tarifs/domaines, `GestionTarification`, `GestionQuotaInami` (calcul par catégories) et éditeur de séance. Le paramétrage des domaines lui-même est couvert par [PLAN_TESTS_DOMAINES.md](PLAN_TESTS_DOMAINES.md).

<details>
<summary>Historique des mises à jour</summary>

- 20/10/2026 — TQ-07 et TQ-08 : le no-show est le statut « Absence non excusée », audit B9
- 01/10/2026

</details>

## Objectif

Valider la résolution d’un tarif à la date d’une séance, son gel historique, la facturation des no-shows et le contrôle des quotas paramétrés par domaine.

## Prérequis et jeu de données

- Deux domaines actifs : un domaine standard et un domaine avec `tarif_convention`, `no_show_facturable` et `soumis_quota` paramétrés indépendamment.
- Au moins deux tarifs actifs dans `ref_prix_seance`, des liaisons actives dans `lia_prix_seance_domaines` et deux périodes de validité non chevauchantes.
- Natures actives, liées aux domaines ; dans le domaine soumis au quota, chaque liaison nature–domaine est rangée dans une catégorie (`lia_natures_seance_domaines.id_categorie_quota`) ou laissée hors quota.
- Domaine soumis au quota avec au moins deux catégories (`ref_categories_quota`, ex. Soutien / Traitement) et une règle active `ref_quotas` (durée de période, seuil d’alerte) avec plafonds par catégorie (`ref_quotas_plafonds`), plus un domaine sans quota.
- Statuts de séance avec `consomme_quota` paramétré (au moins un consommateur et un non consommateur).
- Dossiers de test actifs, dont un dossier standard avec tarif patient renseigné.

## Cas de test — Référentiels et résolution tarifaire

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| TQ-01 | Créer/modifier un tarif de référentiel actif. | Libellé, montant, code et ordre sont restitués. | `ref_prix_seance` ; aucun montant pivot en code. | ☐ |
| TQ-02 | Lier un tarif à un domaine avec période de validité. | Liaison sauvegardée et sélectionnable à la date couverte. | `lia_prix_seance_domaines`. | ☐ |
| TQ-03 | Tenter deux périodes applicables qui se chevauchent pour le même contexte. | Validation empêchant l’ambiguïté ou signalement contrôlé. | Aucune résolution silencieusement ambiguë. | ☐ |
| TQ-04 | Créer une séance conventionnée à une date dans la période A puis B. | Le tarif correspondant à chaque date est résolu. | Montant attendu dans `seances.tarif_seance`. | ☐ |
| TQ-05 | Créer une séance standard avec tarif patient du dossier. | Le tarif patient est utilisé. | Montant figé conforme. | ☐ |
| TQ-06 | Créer une séance de statut non dû. | Tarif résolu à 0. | Règle prioritaire sur toute autre tarification. | ☐ |
| TQ-07 | Créer une séance au statut « Absence non excusée » (`ABSENTE`) dans un domaine dont le no-show n'est pas facturable. | Tarif proposé à 0 (20/10/2026 : avant, la règle cherchait un code « NO_SHOW » inexistant et le tarif du domaine était proposé). | Comportement issu du domaine (`no_show_facturable = 0`). | ☐ |
| TQ-08 | Même séance (`ABSENTE`) dans un domaine dont le no-show est facturable. | Tarif résolu selon les règles du domaine. | Pas de règle déduite du code de domaine. | ☐ |
| TQ-09 | Créer une séance sans tarif applicable. | Repli explicite et journalisé ; comportement lisible. | Journal applicatif et absence de montant inventé. | ☐ |
| TQ-10 | Modifier tarif/référentiel après une séance enregistrée. | La séance historique conserve son tarif figé. | `seances.tarif_seance` inchangé. | ☐ |
| TQ-11 | Modifier le tarif (liste) d’une séance existante, cadenas fermé, puis enregistrer. | Le montant **n’est pas** réaligné ; blocage « montant ≠ tarif » : utilisateur simple → appeler un SuperUser ; SuperUser/Admin → utiliser le cadenas. Fenêtre ouverte. | Base inchangée. | ☐ |
| TQ-11a | SuperUser : ouvrir le cadenas, saisir un montant libre, enregistrer. | Enregistré sans blocage ; dû En attente réaligné. | `tarif_seance` = saisie. | ☐ |
| TQ-11b | SuperUser : ouvrir puis refermer le cadenas. | Montant du tarif sélectionné réappliqué. | — | ☐ |
| TQ-11c | Baisser le tarif d’une séance encaissée 60 → 50 €. | Confirmation annonçant un avoir de 10 € ; après Oui, avoir 10 €, encaissement inchangé. | Refus = rien enregistré. | ☐ |
| TQ-11d | Augmenter le tarif d’une séance encaissée 60 → 70 €. | Confirmation annonçant un reste dû de 10 € ; dû En attente 10 € créé. | — | ☐ |

## Cas de test — Quotas

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| TQ-12 | Créer une séance dans un domaine non soumis au quota. | Aucun contrôle ou avertissement quota. | Pas de consommation calculée. | ☐ |
| TQ-13 | Créer la première séance consommante. | Début de période glissante ; consommation correctement initialisée. | Nature rangée dans une catégorie pour ce domaine, statut consommateur. | ☐ |
| TQ-13a | Créer une séance d’une nature *Hors quota*. | Non décomptée. | Aucune catégorie sur la liaison. | ☐ |
| TQ-14 | Enregistrer des séances de chaque catégorie jusqu’au seuil. | Un compteur par catégorie et alerte au seuil configuré. | Plafonds issus de `ref_quotas_plafonds`, seuil de `ref_quotas`. | ☐ |
| TQ-14a | Domaine avec 1 seule catégorie, puis avec 3 catégories. | L’affichage du dossier et le calcul s’adaptent au nombre de catégories. | Aucun compteur codé en dur. | ☐ |
| TQ-15 | Créer une séance qui dépasse un plafond (catégorie ou total). | Avertissement bien visible « Enregistrer quand même / Annuler », non bloquant. | Annuler = fenêtre ouverte, rien enregistré ; Enregistrer = séance créée. | ☐ |
| TQ-16 | Enregistrer une séance avec un statut non consommateur (no-show, annulée…). | Elle ne consomme pas le quota. | `ref_statuts_seance.consomme_quota = 0`. | ☐ |
| TQ-16a | Basculer `consomme_quota` d’un statut dans son référentiel. | Le décompte suit le paramétrage, sans recompilation. | — | ☐ |
| TQ-17 | Créer une séance hors période glissante (durée de la règle). | Elle n’est plus comptée dans la consommation courante. | `ref_quotas.duree_periode_mois`. | ☐ |
| TQ-18 | Ajouter une nouvelle règle de quota (nouvelle date d’effet). | L’ancienne règle est clôturée la veille ; les séances utilisent la règle de leur date ; aucune règle modifiée. | Pas de recalcul rétroactif implicite. | ☐ |
| TQ-19 | Règle de quota absente ou inactive pour domaine soumis. | Repli explicite/journalisé, sans crash ni plafond codé. | Log et message maîtrisé. | ☐ |
| TQ-20 | Dossier clôturé ou désactivé. | Ne consomme plus de quota. | — | ☐ |
| TQ-21 | Même libellé de catégorie dans deux domaines. | Compteurs indépendants. | Catégorie rattachée au domaine. | ☐ |

### Un seul calcul du quota (audit B.6, 22/10/2026)

Préparer un **dossier de test** d'un domaine soumis au quota, avec une règle de durée **courte** (par exemple 1 mois ; sur un domaine de test, pas sur celui de Pearl), plafond de 2 séances dans une catégorie. Séances de cette catégorie : S1 il y a 6 semaines, S2 il y a 5 semaines (période 1, close) ; S3 il y a 1 semaine (ouvre la période 2).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| TQ-22 | Onglet Séances, puis aperçu du patient (écran Patients). | Mêmes compteurs : période 2, **1 / 2**. | Avant la correction, l'aperçu montrait la période 1 (2 / 2). | ✅ |
| TQ-23 | Nouvelle séance de la catégorie **demain**. | Pas d'alerte (2 / 2 atteint, pas dépassé). Puis une 3e séance dans la période 2 : **alerte** « dépasse le quota ». | Avant : l'alerte comptait la période 1 (déjà pleine) et prévenait dès la première. | ✅ |
| TQ-24 | Nouvelle séance planifiée **après** la fin de la période 2. | Pas d'alerte : elle ouvre la période 3 (1 / 2). | L'onglet Séances la numérote « 1 » dans la période 3. | ✅ |
| TQ-25 | Changer le **domaine** du dossier de test (fiche du dossier) pour un domaine soumis avec une autre règle, sans redémarrer ; rouvrir l'onglet Séances et l'aperçu. | Dès l'enregistrement, le bloc Quotas du bandeau (titre, compteurs) et l'onglet Séances suivent la règle du **nouveau** domaine (durée, plafonds). | Le bandeau restait sur l'ancien domaine jusqu'à la réouverture du dossier (corrigé le 22/10/2026). Remettre le domaine d'origine ensuite. | ✅ |

## Validation finale

- [ ] Tous les cas TQ-01 à TQ-21 sont exécutés.
- [x] TQ-22 à TQ-25 (audit B.6 : un seul calcul du quota, règle du domaine actuel) — validés par Joëlle le 22/10/2026.
- [ ] Les montants et plafonds sont toujours issus des référentiels datés.
- [ ] Le tarif historique et les consommations de quota correspondent aux données de test.
