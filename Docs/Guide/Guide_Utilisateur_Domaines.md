# Althéa - Guide utilisateur : créer et paramétrer un domaine

> Public : administrateur / SuperUser d'Althéa.
> Écran concerné : accueil › **Domaines**.
> Version applicative : refonte UI-6 (octobre 2026), schéma 2.39.0 et suivants.
> Mise à jour : 23/10/2026 — erreurs de saisie toutes affichées d'un coup, quota pas retiré tant que des règles sont actives : audit C.2 étape 3

<details>
<summary>Historique des mises à jour</summary>

- 20/10/2026 — logo du domaine
- 14/10/2026 — étapes, barres d'actions locales, règles de quota guidées et corrigeables

</details>

---

## 1. Les notions à connaître

| Notion | Question à laquelle elle répond | Exemple | Où la définir |
|---|---|---|---|
| **Domaine** | Sous quelle convention travaille-t-on ? (tarif + organisme + quotas) | Realism, Logopédie privée | Étape 1. Général |
| **Organisme** | Qui rembourse ? (facultatif) | INAMI | Étape 1, bouton **Nouveau…** à côté de la liste |
| **Tarif** | Combien coûte une séance, à quelle date ? | Tarif INAMI 2026 | Étape 2. Tarifs |
| **Catégorie de quota** | Dans quel *compteur* la séance est-elle décomptée ? | Soutien, Traitement | Étape 3, bouton **Gérer les catégories…** |
| **Règle de quota** | Combien de séances par catégorie, sur quelle durée, à partir de quand ? | Soutien 10 / Traitement 20 sur 12 mois | Étape 3, **Nouvelle règle** |
| **Nature de séance** | Quel *type* de séance le thérapeute saisit-il ? | Bilan, Séance individuelle | Étape 4. Natures de séance |

### Nature ≠ Catégorie

- La **nature** est ce que le thérapeute choisit à la saisie d'une séance. Elle est commune à tous les domaines.
- La **catégorie** est le compteur de quota. Elle n'apparaît pas à la saisie, elle sert uniquement au décompte. Elle appartient à **un** domaine.
- Le lien entre les deux se fait **dans le domaine** (étape 4) : chaque nature rattachée est rangée dans une catégorie, ou laissée **Hors quota**.

Exemple Realism :

| Nature | Catégorie |
|---|---|
| Bilan | Soutien |
| Séance individuelle | Traitement |
| Entretien administratif | Hors quota |

### Un même nom de catégorie dans plusieurs domaines

C'est autorisé. « Soutien » peut exister dans Realism (plafond 10, 12 mois) **et** dans un autre domaine (plafond 6, 6 mois). Ce sont deux compteurs indépendants.
Seule interdiction : deux catégories de même code ou de même libellé **dans le même domaine**.

---

## 2. L'écran en un coup d'œil

- **À gauche** : la liste des domaines, avec leur état (Complet / À compléter / Inactif). **Afficher les inactifs** montre aussi les domaines désactivés.
- **En haut à droite** : le nom du domaine sélectionné et une pastille d'état (**Domaine complet**, **À compléter : n point(s)**, **Inactif**).
- **Les étapes** : une rangée de cinq étapes, *1. Général*, *2. Tarifs*, *3. Catégories & quotas*, *4. Natures de séance*, *5. Synthèse*. Chacune montre son état :
  - **Fait** (pastille verte cochée) : rien ne manque ;
  - **À compléter** (pastille « ! », texte ocre) : il manque quelque chose, le détail est indiqué sous l'étape ;
  - **Non nécessaire** (pastille « – » en pointillés, grisée) : l'étape ne concerne pas ce domaine (par exemple *Tarifs* pour un domaine non conventionné).
  L'étape affichée est entourée d'un anneau.
- **La consigne** : sous les étapes, une phrase explique ce qu'il faut faire dans l'étape affichée, avec **◂ Précédent** / **Suivant ▸**.
- On change d'étape en cliquant dessus, avec **◂ Précédent** / **Suivant ▸**, ou avec les flèches gauche / droite du clavier.

L'ordre est important : **on ne peut pas choisir une catégorie pour une nature tant que la catégorie n'existe pas** (étape 3 avant l'étape 4).

### Deux façons de modifier

| Où | Comment | Enregistrement |
|---|---|---|
| **Étape 1. Général** (fiche du domaine) | **Modifier** → corriger → **Enregistrer** ou **Annuler** | Au clic sur **Enregistrer** |
| **Étapes 2 à 4** (listes rattachées) | Directement, avec les boutons de la **barre d'actions de la carte** (Rattacher, Clôturer, Retirer…) | **Immédiat**, action par action |

Pendant **Modifier**, seule l'étape 1 est accessible : les étapes 2 à 4 se rouvrent dès que vous enregistrez ou annulez. C'est voulu : on ne mélange pas une fiche en cours de saisie avec des enregistrements immédiats.

En consultation, les champs restent **lisibles** (fond beige clair) : ils ne sont simplement pas modifiables.

---

## 3. Créer un domaine pas à pas

### Étape 1 — Général
1. Cliquer sur **Nouveau**.
2. Carte **Identité du domaine** : code (3 lettres), libellé, organisme de remboursement (bouton **Nouveau…** pour en créer un), ordre d'affichage, *Domaine actif*.
3. Carte **Comportement (convention)** :
   - **Tarif conventionné** : le prix vient d'un tarif rattaché au domaine (étape 2), pas du tarif patient du dossier.
   - **Absence non excusée facturable** : une absence non excusée reste due par le patient.
   - **Soumis à un quota de séances** : les séances sont décomptées selon les règles de l'étape 3.
4. Carte **Logo du domaine** (facultatif) :
   - **Importer…** une image (PNG, JPG…), ou **Retirer** le logo.
   - Il n'est enregistré qu'avec le domaine (**Enregistrer**) ; **Annuler** rend le logo précédent.
   - Il apparaît ensuite dans la convention du domaine, dans le bandeau du dossier et dans la liste des domaines de la fiche dossier.
   - Pour changer le logo d'un domaine existant : **Modifier**, puis Importer ou Retirer.
5. **Enregistrer**. L'écran passe automatiquement à la **première étape qui reste à compléter**.
   S'il y a quelque chose à corriger (code qui n'a pas 3 lettres ou déjà pris, libellé vide ou déjà pris, quota retiré alors que des règles de quota sont encore actives), une fenêtre liste **tous** les points d'un coup et une icône rouge les marque ; elle disparaît dès que le champ est corrigé.

> Tant que le domaine n'est pas enregistré, les étapes 2 à 5 ne sont pas accessibles.

### Étape 2 — Tarifs *(si conventionné)*
1. **Rattacher un tarif** : un petit formulaire s'ouvre **au bas de la carte**.
2. Choisir le tarif, la date **À partir du** (et éventuellement **Jusqu'au**), puis **Rattacher** (ou **Annuler**).
3. Si le tarif n'existe pas encore : **Créer un nouveau tarif…** (à droite de la barre) ouvre le catalogue des tarifs ; au retour, le nouveau tarif est proposé.
4. Un tarif doit être **en vigueur aujourd'hui**.

### Étape 3 — Catégories & quotas *(si soumis à quota)*

L'écran suit l'ordre du travail : **d'abord les catégories (carte du haut), ensuite les règles (carte du bas)**.

1. **Gérer les catégories…** : créer les compteurs (code + libellé), ex. `SOUTIEN` / Soutien. Elles apparaissent en pastilles dans la carte du haut.
2. **Nouvelle règle** : la zone du bas devient modifiable, pré-remplie à partir de la règle en vigueur (ou de la plus récente). Remplir dans l'ordre indiqué par la note :
   1. le **plafond de chaque catégorie** (colonne *Plafond* ; le total est calculé) ;
   2. la **période glissante** (en mois) et l'**alerte** (nombre de séances, 0 = pas d'alerte) ;
   3. la date **À partir du** (elle peut être **dans le passé**, voir 6.3) et, si besoin, **Jusqu'au** ;
   4. **Ajouter la règle**.
3. Un **récapitulatif** s'affiche avant l'enregistrement : plafonds, alerte, validité, et, s'il y a lieu, la date à laquelle la règle actuelle s'arrêtera. Confirmer ou revenir à la saisie.
4. Une règle doit être **en vigueur aujourd'hui** pour que le domaine soit complet.

> Si un domaine n'est pas soumis à quota, l'étape affiche simplement « Étape non nécessaire » (voir l'étape 1. Général).

### Étape 4 — Natures de séance
1. **Rattacher une nature** : le formulaire s'ouvre au bas de la carte. Choisir la nature et, si le domaine est soumis à quota, sa **catégorie de quota** (ou *Hors quota*), puis **Rattacher**.
2. Si la nature n'existe pas : **Créer une nouvelle nature…** ouvre le référentiel des natures.
3. La catégorie d'une nature déjà rattachée se change **directement dans la grille**, colonne *Catégorie de quota*.
4. **Retirer** masque la nature à la saisie sans perdre l'historique ; **Supprimer** n'est possible que si aucune séance ne l'a utilisée.

> Changer la catégorie d'une nature déjà utilisée modifie le décompte des dossiers existants, y compris pour le passé : une confirmation est demandée.

### Étape 5 — Synthèse
La carte **Convention du domaine** résume le domaine : comportement, tarifs en vigueur, natures et catégories, règle de quota applicable, puis les **points d'attention**, chacun avec l'étape où le corriger.
Le domaine est prêt quand s'affiche **« Domaine complet : il peut être utilisé pour ouvrir des dossiers »** (et la pastille *Domaine complet* en haut).

---

## 4. Points d'attention de la synthèse

| Message | Que faire |
|---|---|
| Domaine conventionné sans tarif en vigueur aujourd'hui | Étape 2 : rattacher un tarif valable aujourd'hui |
| Aucune catégorie de quota créée | Étape 3 : **Gérer les catégories…** |
| Domaine soumis à quota sans règle en vigueur aujourd'hui | Étape 3 : **Nouvelle règle** (les dates proposées couvrent la période manquante) |
| Aucune règle du … au … | Étape 3 : sélectionner la ligne en ocre, puis **Nouvelle règle** (règle intermédiaire) |
| Aucune nature de séance active n'est rattachée | Étape 4 : **Rattacher une nature** |
| Aucune nature ne consomme le quota | Étape 4 : choisir une catégorie pour au moins une nature (colonne *Catégorie de quota*) |

Les mêmes manques colorent les étapes concernées en ocre.

---

## 5. Cas types

### Domaine simple (sans organisme, sans quota)
Étape 1 (ni convention, ni quota) → les étapes 2 et 3 sont marquées *non nécessaire* → Étape 4 (rattacher les natures) → Synthèse.

### Domaine conventionné avec quota (ex. Realism / INAMI)
Les 5 étapes, dans l'ordre.

### Nouvel organisme de remboursement
Créer un **nouveau domaine** (un domaine = une convention). Ses catégories, règles de quota et tarifs lui sont propres, même s'ils portent les mêmes noms que ceux d'un autre domaine.

### Changement de plafond au 1er janvier
Étape 3 → **Nouvelle règle** → nouveaux plafonds → **À partir du** 01/01 → **Ajouter la règle**. La règle actuelle s'arrête automatiquement le 31/12.

### Changement oublié (la nouvelle règle aurait dû commencer il y a quelques jours)
Même démarche, avec une date **À partir du** dans le passé. Le récapitulatif prévient que les séances enregistrées depuis cette date seront décomptées selon la nouvelle règle. Rien d'autre à faire : les compteurs ne sont pas stockés, ils sont recalculés à partir des dates des séances.

---

## 6. Modifier, désactiver, supprimer : que faire dans chaque étape ?

### Principe général

| Action | Effet | Quand l'utiliser |
|---|---|---|
| **Clôturer** | L'élément s'arrête à une date ; il reste valable pour le passé. | Changement prévu (nouveau tarif, nouveau plafond). |
| **Désactiver / Retirer** | L'élément n'est plus proposé ; l'historique est conservé. Réversible (**Réactiver / Rétablir**). | L'élément a déjà servi et ne doit plus être utilisé. |
| **Supprimer** | Effacement définitif. | **Uniquement** pour une erreur de saisie jamais utilisée. |

> En cas de doute : **désactivez**, ne supprimez pas.
> Ces actions demandent le rôle SuperUser. Elles sont enregistrées immédiatement et ne sont pas disponibles pendant la modification de la fiche (Modifier… Enregistrer / Annuler).

### 6.1 Étape 1 — Général (le domaine lui-même)

| Je veux… | Comment | Restrictions |
|---|---|---|
| Changer le libellé, l'organisme, les cases à cocher | **Modifier** → corriger → **Enregistrer** | On ne peut pas retirer « Soumis à un quota de séances » tant qu'une règle de quota active existe : l'enregistrement est refusé et la case est marquée en rouge ; désactivez d'abord les règles (étape 3). |
| Ne plus proposer le domaine | **Désactiver** | Si des dossiers l'utilisent, une confirmation s'affiche ; ces dossiers ne sont pas modifiés. Réversible avec **Activer**. |
| Supprimer le domaine | — | **Impossible** : un domaine se désactive uniquement. |

### 6.2 Étape 2 — Tarifs

| Je veux… | Comment | Restrictions |
|---|---|---|
| Changer de tarif à une date | Sélectionner le tarif actuel, **Clôturer la période** : indiquer le dernier jour au bas de la carte, **Clôturer**. Puis **Rattacher un tarif** pour le nouveau, à partir du lendemain. | La date de clôture ne peut pas précéder le début du rattachement. |
| Ne plus utiliser un tarif | **Désactiver** | Réactivation refusée si un autre tarif est déjà actif sur la même période (clôturez-le d'abord). |
| Effacer un rattachement erroné | **Supprimer** | Si des séances existent sur la période, un avertissement recommande **Désactiver** (la suppression effacerait la trace du tarif appliqué). |

> Le tarif lui-même (montant) se gère dans le catalogue **Tarifs de séance** (bouton **Créer un nouveau tarif…**), pas ici.

### 6.3 Étape 3 — Catégories & quotas

**Catégories** (bouton **Gérer les catégories…**)

| Je veux… | Comment | Restrictions |
|---|---|---|
| Renommer une catégorie | — | **Impossible** : désactivez-la et créez-en une nouvelle. |
| Ne plus utiliser une catégorie | **Désactiver** | Réversible avec **Réactiver**. |
| Effacer une catégorie | **Supprimer** | Refusé si une nature y est rangée ou si une règle de quota l'utilise : désactivez-la plutôt. |

**Règles de quota** (barre *Règles de quota*)

Une règle **a servi** dès qu'une séance du domaine tombe dans sa période de validité. Cette distinction décide de ce qui est permis :

| Je veux… | Règle pas encore utilisée | Règle qui a servi |
|---|---|---|
| **Corriger** une erreur (plafond, durée, alerte, dates) | **Modifier la règle** → corriger → **Enregistrer la correction** (récapitulatif, puis confirmation). Corriger une date ne déplace pas celle de la règle voisine : si un écart apparaît, il est annoncé, puis à couvrir par une règle intermédiaire. | Refusé : l'historique des quotas doit rester exact. Passer par **Nouvelle règle**. |
| **Changer un plafond à partir d'une date** (cas courant) | **Nouvelle règle** (voir étape 3 ci-dessus), ou corriger directement si la règle actuelle n'a jamais servi. | **Nouvelle règle** avec la date du changement : la règle en cours s'arrête **automatiquement la veille**. |
| **Arrêter** le quota sans le remplacer | **Clôturer la règle** : indiquer le dernier jour au bas de la carte, **Clôturer**. | Idem. |
| **Neutraliser** une règle | **Désactiver** (réversible avec **Réactiver** ; refusé si elle chevauche une autre règle). | Idem. |
| **Effacer** une règle saisie par erreur | **Supprimer** (confirmation). | Refusé : **Clôturer** ou **Désactiver**. |

Ce que fait la suppression d'une règle erronée :

- Si cette règle avait arrêté automatiquement la règle précédente, **la règle précédente est rouverte** (jusqu'à la fin de la règle supprimée, ou sans date de fin). La confirmation l'annonce.
- Si, après la suppression, une période n'est plus couverte par aucune règle, la confirmation le signale : pensez à la couvrir par une **Nouvelle règle**.

**Période sans règle.** Si une période n'est couverte par aucune règle, ses séances ne sont soumises à aucun quota. Althéa ne la comble **jamais d'elle-même** (elle ne déplace pas la date d'une autre règle) ; elle la montre partout :

- dans la grille des règles, une **ligne en ocre sur fond jaune pâle** « Aucune règle du … au … », état *Sans règle*, à sa place entre les règles ;
- dans la note sous la carte, en ocre ;
- pour une période entre deux règles (ou qui contient aujourd'hui) : l'étape 3 passe **À compléter** et la synthèse l'indique dans ses points d'attention.

Pour la couvrir : sélectionner la ligne en ocre, puis **Nouvelle règle** — les dates de la période sont proposées. Une action qui créerait une période sans règle (nouvelle règle, correction, clôture, suppression) l'annonce dans sa confirmation, en commençant par « ATTENTION ».

**Date de début dans le passé.** Elle est acceptée (cas d'un changement saisi en retard). Le récapitulatif prévient que les séances enregistrées depuis cette date seront décomptées selon la nouvelle règle.

### 6.4 Étape 4 — Natures de séance

| Je veux… | Comment | Restrictions |
|---|---|---|
| Changer la catégorie d'une nature | Dans la grille, colonne *Catégorie de quota*, choisir la catégorie (ou *Hors quota*). | Effet immédiat, **y compris sur le décompte passé** des dossiers du domaine : confirmation si des séances existent. Une catégorie désactivée ne peut pas être choisie. |
| Ne plus proposer une nature dans ce domaine | **Retirer** | L'historique des séances est conservé. Réversible avec **Rétablir**. Avertissement si le domaine n'aurait plus aucune nature. |
| Effacer un rattachement erroné | **Supprimer** | **Refusé** si des séances utilisent cette nature dans ce domaine : utilisez **Retirer**. |

> Le libellé d'une nature se gère dans le référentiel **Natures de séance** (bouton **Créer une nouvelle nature…**) ; il est commun à tous les domaines.

### 6.5 Étape 5 — Synthèse

Lecture seule. Après toute modification, vérifiez que la pastille indique toujours **Domaine complet**.

---

## 7. Rappels

- Un dossier clôturé ou désactivé ne consomme plus de quota.
- Seuls les statuts de séance marqués « consomme quota » sont décomptés.
- Les quotas sont rattachés au **domaine**, jamais au patient ou au dossier.
- Les compteurs de quota ne sont jamais stockés : ils sont recalculés à partir des séances et des dates, ce qui permet de corriger une règle ou une date sans « recalcul » manuel.
