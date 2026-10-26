# Plan de tests — Gestion des domaines (Domaine = convention)

> Version : 2.6 — Dernière mise à jour : 23/10/2026 — section 13 : saisie et complétude jugées par le métier, audit C.2 étape 3
> Périmètre : écran `UC_DomainesHome` (accueil › **Domaines**), `AltheaEtapes`, `UC_LiaisonsPrixDomaines` et `UC_LiaisonsNaturesDomaines` (V2), `UC_SyntheseDomaine`, `CategoriesQuotaEditor`, ajout rapide d'organisme / tarif / nature, `GestionDomaines`, `GestionCategoriesQuota`, `GestionQuotasInami`, `GestionLiaisonsNaturesDomaines`, `GestionLiaisonsPrixDomaines`.
> Références : ADR-27 (Domaine = convention), ADR-28 (lignes système), ADR-34 (barres locales), ADR-40 (étapes, règles de quota corrigeables), Rules « Consultation lisible » et « Fiche et listes rattachées », [Guide_Utilisateur_Domaines.md](../Guide/Guide_Utilisateur_Domaines.md). Schéma 2.39.0 et suivants.

<details>
<summary>Historique des mises à jour</summary>

- 21/10/2026 — section 12 : nouvelle règle de quota d'un seul tenant, audit B.2
- 20/10/2026 — section 11 : logo du domaine, migration v2.45.0
- 19/10/2026 — section 10 : Modifier seulement sur l'étape 1
- 18/10/2026 — section 9 : états en pastilles, UI-9.4
- 15/10/2026 — UI-7 : dialogue des catégories en cartes, D-47 ; V2.0 du 14/10/2026 : étapes, barres locales, règles de quota corrigeables

</details>

## Objectif

Valider la création et le paramétrage complet d'un domaine-convention (organisme, tarifs, catégories et règles de quota, natures), la navigation par étapes et leur état, la synthèse, le cycle de vie des règles de quota (correction tant qu'elles n'ont pas servi, protection ensuite) et les protections (désactivation plutôt que suppression, lignes système).

## Prérequis et jeu de données

- Utilisateur SuperUser ou Admin ; un utilisateur simple pour les tests de droits.
- Organisme système `INAMI` (`est_systeme = 1`) présent dans `ref_organismes`.
- Au moins deux tarifs actifs dans `ref_prix_seance`, trois natures actives dans `ref_natures_seance`.
- Statuts de séance avec `consomme_quota` paramétré (au moins un consommateur, un non consommateur).
- Un domaine existant utilisé par des dossiers et des séances (ex. Realism), pour les tests de protection.
- Un domaine de test soumis à quota **sans aucune séance**, pour les corrections et suppressions de règles.

## 1. Accès, navigation et lecture seule

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-01 | Ouvrir accueil › **Domaines** en SuperUser/Admin. | `UC_DomainesHome` s'affiche, liste des domaines à gauche, fiche à droite. | Fil d'Ariane, aucune exception. | ☐ |
| D-02 | Ouvrir l'écran avec un utilisateur simple. | Accès refusé ou consultation seule, conforme aux droits. | Aucun bouton de modification actif, barres locales comprises. | ☐ |
| D-03 | Sélectionner un domaine existant. | Les 5 étapes (Général / Tarifs / Catégories & quotas / Natures / Synthèse) se chargent pour ce domaine ; aucune bande d'onglets visible. | Données conformes à la base. | ☐ |
| D-04 | Changer d'étape : clic sur une étape, **◂ Précédent** / **Suivant ▸**, flèches gauche / droite. | L'étape affichée change, entourée d'un anneau ; la consigne suit. | Premier / dernier : bouton correspondant inactif. | ☐ |
| D-05 | État des étapes d'un domaine incomplet. | Étape concernée « À compléter » (pastille « ! », détail ocre) ; étape inutile « Non nécessaire » (pointillés). | Cohérent avec la synthèse. | ☐ |
| D-06 | Pastille d'état à côté du nom du domaine. | *Domaine complet*, *À compléter : n point(s)* ou *Inactif*. | — | ☐ |
| D-07 | Consultation de l'étape 1. | Champs lisibles (fond beige clair, texte normal), non modifiables ; listes et cases lisibles, pas grisées. | `UtilsControls.DefinirLectureSeule`. | ☐ |
| D-08 | Liste longue de domaines / liaisons. | Défilement / pagination fonctionnels. | — | ☐ |

## 2. Création et modification — Étape 1. Général

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-10 | **Nouveau**, saisir code, libellé, ordre, cocher les comportements, **Enregistrer**. | Domaine créé ; l'écran passe à la **première étape à compléter**. | `ref_domaines` : `tarif_convention`, `no_show_facturable`, `soumis_quota`, `id_organisme`. | ☐ |
| D-11 | Avant enregistrement, cliquer sur les autres étapes. | Étapes inaccessibles tant que le domaine n'est pas enregistré ; consigne explicite. | — | ☐ |
| D-12 | Code ou libellé déjà existant. | Refus avec message clair. | Unicité contrôlée. | ☐ |
| D-13 | Code de plus de 3 caractères / avec espace. | Saisie bloquée / espace refusé. | `MaxLength = 3`. | ☐ |
| D-14 | Organisme facultatif : enregistrer sans organisme. | Accepté. | `id_organisme` NULL. | ☐ |
| D-15 | Bouton **Nouveau…** à côté de l'organisme : créer un organisme puis revenir. | Nouvel organisme créé et présélectionné. | `ref_organismes`. | ☐ |
| D-16 | **Annuler** pendant la création. | Aucun domaine créé ; retour à l'état précédent. | — | ☐ |
| D-17 | **Modifier** un domaine existant. | Seule l'étape 1 est accessible ; consigne « Enregistrez ou annulez pour retrouver les étapes 2 à 4 » ; liste des domaines inactive. | Après **Enregistrer** / **Annuler** : étapes de nouveau accessibles. | ☐ |

## 3. Étape 5. Synthèse

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-20 | Afficher l'étape Synthèse. | Carte **Convention du domaine** : en-tête (libellé, code, organisme), comportement, tarifs, natures, quota, points d'attention. | Même présentation que dans la fiche dossier. | ☐ |
| D-21 | Domaine ni conventionné ni soumis au quota. | Étapes 2 et 3 « Non nécessaire ». | — | ☐ |
| D-22 | Domaine conventionné sans tarif en vigueur. | « Domaine conventionné sans tarif en vigueur aujourd'hui (étape 2. Tarifs). » | Étape 2 « À compléter ». | ☐ |
| D-23 | Domaine soumis au quota sans catégorie. | « Aucune catégorie de quota créée (étape 3…) ». | Étape 3 « À compléter ». | ☐ |
| D-24 | Domaine soumis au quota sans règle en vigueur. | « … sans règle en vigueur aujourd'hui (étape 3…) ». | — | ☐ |
| D-25 | Sans nature active / sans nature consommatrice. | Messages correspondants, étape 4 « À compléter ». | — | ☐ |
| D-26 | Domaine complètement paramétré. | « Domaine complet : il peut être utilisé pour ouvrir des dossiers » ; pastille *Domaine complet*. | — | ☐ |

## 4. Étape 2. Tarifs

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-30 | **Rattacher un tarif** : formulaire au bas de la carte, tarif + **À partir du**, **Rattacher**. | Liaison créée, en vigueur aujourd'hui ; formulaire refermé ; étapes et synthèse actualisées. | `lia_prix_seance_domaines`. | ☐ |
| D-31 | **Créer un nouveau tarif…** puis le rattacher. | Catalogue ouvert ; au retour, le tarif est proposé. | `ref_prix_seance`. | ☐ |
| D-32 | **Clôturer la période** (dernier jour au bas de la carte) puis rattacher un nouveau tarif. | Périodes successives sans chevauchement. | Date de clôture ≥ début. | ☐ |
| D-33 | Clôturer avant la date de début. | Refusé avec message. | — | ☐ |
| D-34 | Réactiver un tarif alors qu'un autre est actif sur la même période. | Refusé. | Aucun chevauchement. | ☐ |
| D-35 | **Supprimer** un rattachement utilisé par des séances. | Avertissement recommandant **Désactiver**. | Séances historiques intactes. | ☐ |
| D-36 | **Annuler** dans le formulaire. | Formulaire refermé, rien d'enregistré. | — | ☐ |

## 5. Étape 3. Catégories & quotas

### 5.1 Catégories (`CategoriesQuotaEditor`)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-40 | Créer `SOUTIEN` / Soutien et `TRAITEMENT` / Traitement. | Catégories en pastilles dans la carte du haut. | `ref_categories_quota.id_domaine`. | ☐ |
| D-41 | Créer une catégorie de même code ou libellé dans le **même** domaine. | Refusé. | — | ☐ |
| D-42 | Créer « Soutien » dans un **autre** domaine. | Accepté ; compteurs indépendants. | — | ☐ |
| D-43 | Renommer une catégorie. | Impossible (désactiver + recréer). | — | ☐ |
| D-44 | Désactiver puis réactiver une catégorie. | Réversible. | — | ☐ |
| D-45 | Supprimer une catégorie utilisée par une nature ou une règle. | Refusé avec message. | — | ☐ |
| D-46 | Supprimer une catégorie inutilisée. | Supprimée. | — | ☐ |
| D-47 | Ouvrir **Gérer les catégories…** (UI-7). | Dialogue au schéma des fenêtres. | Fond beige à la feuille, barre de titre beige ; carte « Catégories » (barre locale Désactiver / Supprimer, aide, grille) ; carte « Nouvelle catégorie » (Code, Libellé, Ajouter, Entrée = Ajouter) ; « Supprimer » confirmé par un bouton terre ; Échap = Fermer. | ☐ |

### 5.2 Règles de quota

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-50 | **Nouvelle règle** (première règle) : plafonds, période glissante, alerte, début, **Ajouter la règle**. | Récapitulatif complet et lisible, puis règle créée et en vigueur. | `ref_quotas` + `ref_quotas_plafonds`. | ☐ |
| D-51 | **Changer un plafond** : Nouvelle règle « À partir du » date future. | Zone pré-remplie depuis la règle en vigueur ; le récapitulatif annonce l'arrêt de l'ancienne règle **la veille**. | Deux lignes ; ancienne règle : `date_fin_validite` = veille. | ☐ |
| D-52 | Nouvelle règle avec début ≤ début de la règle actuelle. | Refusé. | — | ☐ |
| D-53 | Nouvelle règle avec début **dans le passé** (après le début de la règle actuelle). | Acceptée ; le récapitulatif prévient que les séances depuis cette date seront décomptées selon la nouvelle règle. | Décompte des dossiers recalculé (voir D-81). | ☐ |
| D-54 | **Clôturer la règle** sans remplacement. | Règle arrêtée à la date choisie ; période suivante signalée « sans règle ». | — | ☐ |
| D-55 | Désactiver puis réactiver une règle qui chevaucherait une autre. | Réactivation refusée. | — | ☐ |
| D-56 | **Modifier la règle** sur une règle **sans séance** : changer un plafond, la durée, l'alerte, une date ; **Enregistrer la correction**. | Récapitulatif « règle corrigée », puis mise à jour de la même ligne. | Même `id_quota`, aucune nouvelle ligne. | ☐ |
| D-57 | **Modifier la règle** sur une règle **qui a servi** (séance du domaine dans sa période). | Refus expliqué, orientation vers **Nouvelle règle** / **Clôturer la règle**. | Rien de modifié. | ☐ |
| D-58 | Correction d'une règle inutilisée créant un chevauchement avec une autre règle. | Refusé (« Une autre règle de ce domaine couvre déjà… »). | — | ☐ |
| D-59 | **Supprimer** une règle **qui a servi**. | Refus expliqué (Clôturer / Désactiver). | Règle intacte. | ☐ |
| D-60 | **Supprimer** une règle sans séance qui avait clôturé la précédente. | La confirmation annonce la réouverture de la règle précédente ; après confirmation, la précédente est rouverte (fin = fin de la règle supprimée, ou aucune). | Une seule transaction : en cas d'erreur, rien n'est modifié. | ☐ |
| D-61 | **Supprimer** une règle sans séance, sans règle précédente, créant une période sans règle. | La confirmation signale la période qui ne sera plus couverte. | — | ☐ |
| D-62 | Période sans règle aujourd'hui (ou entre deux règles). | Ligne ocre sur fond jaune pâle « Aucune règle du … au … », état *Sans règle*, à sa place dans la grille ; note ocre sous la carte. | `PeriodesSansRegle`. | ☐ |
| D-63 | Domaine non soumis à quota. | Une seule carte « Étape non nécessaire… », sans grille ni boutons. | — | ☐ |
| D-64 | Décocher « Soumis à un quota de séances » (étape 1) avec une règle active. | Refusé tant que la règle est active. | — | ☐ |
| D-65 | Sélectionner la ligne « Sans règle », puis **Nouvelle règle**. | Dates de la période proposées ; boutons Modifier / Clôturer / Désactiver / Supprimer indisponibles sur cette ligne. | Après ajout : la ligne disparaît. | ☐ |
| D-66 | Période entre deux règles (passée ou future). | Étape 3 « À compléter » ; synthèse : « Aucune règle du … au … » ; pastille « À compléter ». | Arrêt voulu (clôture sans remplacement, date future) : ligne affichée, domaine non incomplet. | ☐ |
| D-67 | Correction (date de début plus tardive), nouvelle règle à fin bornée ou clôture qui crée un écart. | La confirmation contient « ATTENTION : … aucune règle ne couvrira … » ; la fin de l'autre règle n'est **pas** déplacée. | `ref_quotas` : seule la règle visée change. | ☐ |

## 6. Étape 4. Natures de séance

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-68 | **Rattacher une nature** (formulaire au bas de la carte, avec sa catégorie) et via **Créer une nouvelle nature…**. | Nature proposée à la saisie de séance pour ce domaine, avec la catégorie choisie. | `lia_natures_seance_domaines.id_categorie_quota`. | ☐ |
| D-69 | Changer la catégorie d'une nature dans la grille (colonne *Catégorie de quota*) / *Hors quota*. | Enregistrée immédiatement sur la **liaison**, pas sur la nature. | — | ☐ |
| D-70 | Domaine sans catégorie de quota. | Note « toutes sont hors quota » ; pas de choix de catégorie. | — | ☐ |
| D-71 | Choisir une catégorie désactivée. | Refusé avec message. | Valeur précédente conservée. | ☐ |
| D-72 | Changer la catégorie d'une nature déjà utilisée. | Confirmation : impact sur le décompte passé. | Décompte recalculé. | ☐ |
| D-73 | **Retirer** puis **Rétablir** une nature. | Masquée à la saisie, historique conservé ; réversible. | Avertissement si plus aucune nature. | ☐ |
| D-74 | **Supprimer** une nature utilisée par des séances du domaine. | Refusé (utiliser Retirer). | — | ☐ |

## 7. Protection et cycle de vie

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-75 | **Désactiver** un domaine utilisé par des dossiers. | Confirmation ; dossiers inchangés ; domaine non proposé pour un nouveau dossier. | — | ☐ |
| D-76 | Réactiver le domaine. | Domaine de nouveau proposé. | — | ☐ |
| D-77 | Chercher une action de suppression de domaine. | Impossible. | — | ☐ |
| D-78 | Organisme système `INAMI` : tenter suppression ou changement de code. | Refusé (ADR-28). | `est_systeme = 1`. | ☐ |
| D-79 | Journalisation (création, clôture, ajout / correction / suppression de règle, désactivation). | Entrées présentes dans le journal applicatif, sans donnée personnelle. | `GestionLog` (ADR-39). | ☐ |

## 8. Effets sur le suivi (contrôle croisé)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-80 | Ouvrir un dossier sur un domaine complet, créer des séances consommatrices. | Compteurs par catégorie affichés dans `UC_DossierFiche`. | Plafonds issus de la règle en vigueur. | ☐ |
| D-81 | Séances de part et d'autre de la date d'effet d'une nouvelle règle (y compris date de début passée). | Chaque séance évaluée selon la règle applicable à sa date. | Voir TQ-18. | ☐ |
| D-82 | Statut non consommateur (`consomme_quota = 0`). | Non décompté. | Voir TQ-16. | ☐ |
| D-83 | Après D-56 (correction) : dossier d'un autre domaine. | Aucun effet hors du domaine corrigé. | — | ☐ |

## 9. États en pastilles (UI-9.4, 18/10/2026)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-84 | Liste des domaines, avec « Afficher les inactifs ». | Colonne État en pastille : **Complet** (sauge), **À compléter** (ocre), **Inactif** (gris, ligne atténuée) | - L'état se met à jour après un rattachement (comme avant) | ☐ |
| D-85 | Étape Catégories & quotas : grille des règles. | État en pastille : **En vigueur** (sauge), **À venir** (bleu), **Terminée** / **Désactivée** (gris ; règle désactivée atténuée) ; ligne de période sans règle : pastille **Sans règle** (ocre) sur sa ligne d'alerte | - Alerte et plafonds alignés à droite | ☐ |
| D-86 | Catégorie désactivée (« Gérer les catégories… »). | Dans l'écran : pastille grise « … (inactive) » ; dans le dialogue : pastille **Inactive**, rien pour une catégorie active, ligne atténuée | | ☐ |
| D-87 | Étape Tarifs : rattachements. | État en pastille : **En vigueur** (sauge), **À venir** (bleu), **Clôturé** / **Désactivé** (gris, ligne atténuée) | - Même panneau depuis Référentiels › Tarifs de séance | ☐ |
| D-88 | Étape Natures : rattachements, dont une nature désactivée dans son référentiel. | **Proposée** (sauge), **Nature désactivée** (ocre), **Retirée du domaine** (gris) ; ligne non proposée atténuée | - La liste « Catégorie de quota » reste modifiable | ☐ |

## 10. Modifier seulement sur l'étape 1 (19/10/2026)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-89 | En consultation (SuperUser ou Admin), parcourir les étapes 1 à 5. | **Modifier** visible sur l'étape 1 (Général) seulement ; ailleurs masqué, **Désactiver** prend sa place (pas de trou dans la barre) | - Nouveau, Désactiver restent disponibles sur toutes les étapes | ☐ |
| D-90 | Étapes 2 à 4 (Tarifs, Catégories & quotas, Natures) en consultation. | Consigne terminée par « Cette étape se modifie directement : chaque action est enregistrée aussitôt, sans passer par Modifier. » ; barres locales actives (« Gérer les catégories… » compris) | - Étape « non nécessaire » : pas de cette phrase ; utilisateur simple : pas de phrase, barres inactives | ☐ |

## 11. Logo du domaine (20/10/2026, migration v2.45.0)

Prérequis : migration `migration_v2.45.0_logos_domaines.sql` appliquée (base 2.45.0) ; une image PNG avec fond transparent et une grande photo JPG (plusieurs Mo).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-91 | Étape 1 en consultation. | Carte « Logo du domaine » sous l'identité et le comportement ; Importer… et Retirer inactifs | - Domaine sans logo : cadre vide | ☐ |
| D-92 | Modifier, Importer… le PNG transparent, Enregistrer. | Logo affiché dans la carte, puis à l'étape 5 (Synthèse) sous les règles (jusqu'à 180 × 80, lisible) | - Fichier `{dossier de données}/Domaines/Domaine_{id}/Logo.png`, 256 px au plus, transparence gardée ; `logo_fichier` = Logo.png | ☐ |
| D-93 | Modifier, Importer… la grande photo, puis Annuler. | « Abandonner / Continuer la saisie » ; Abandonner : l'ancien logo revient | - Fichier sur le disque inchangé | ☐ |
| D-94 | Modifier, Importer…, puis cliquer un autre écran du menu. | Question Enregistrer / Quitter sans enregistrer / Rester | - Seul le logo a changé : la question est posée quand même | ☐ |
| D-95 | Modifier, Retirer, Enregistrer. | Plus de logo (carte, synthèse) | - `logo_fichier` NULL, fichier Logo.* retiré | ☐ |
| D-96 | Nouveau domaine avec logo choisi avant le premier Enregistrer. | Domaine créé avec son logo | - Dossier `Domaine_{nouvel id}` créé | ☐ |
| D-97 | Importer un fichier qui n'est pas une image (ex. .txt renommé .png). | Message « Ce fichier n'est pas une image lisible… » ; rien ne change | — | ☐ |
| D-98 | Sauvegarde puis restauration (base de test). | Le logo revient avec les données | - Dossier `Domaines/` compris dans l'archive | ☐ |
| D-99 | Étape 3 : Nouvelle règle, puis Modifier la règle. | « À partir du » et « Jusqu'au » sont des champs de date du kit (saisie, calendrier) ; « Jusqu'au » inactif mais lisible tant que la case n'est pas cochée | - Enregistrement et contrôles des dates inchangés | ☐ |
| D-100 | Étape 2 : rattacher un tarif avec une date de début et une date de fin. | Champs de date du kit dans la barre de saisie ; fin avant début : ramenée au début comme avant | — | ☐ |

## 12. Nouvelle règle de quota d'un seul tenant (audit B.2, 21/10/2026)

Domaine **de test** soumis au quota, avec une règle ouverte R1 (sans date de fin).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-101 | Étape 3 : **Nouvelle règle** R2 qui commence dans un mois, plafonds valides → Enregistrer. | Récapitulatif « La règle actuelle (…) s'arrêtera le [veille] » ; après Enregistrer : R1 se termine la veille du début de R2, R2 ouverte. | Barre d'état « … la règle précédente est arrêtée la veille ». Journal : « Règle de quota N ajoutée (domaine=…) ; règle M arrêtée au jj/mm/aaaa. » | ✅ |
| D-102 | Nouvelle règle avec un **seuil d'alerte supérieur au plafond total** (ex. plafonds 10 + 20, alerte 40). | Message « Le seuil d'alerte (40) ne peut pas dépasser le plafond total (30). » **avant** le récapitulatif ; rien n'est enregistré. | R1 inchangée. | ✅ |
| D-103 | Après D-101 : nouvelle règle qui commence **dans la période de R1** (fermée), avant le début de R2. | Message « Une autre règle de ce domaine couvre déjà tout ou partie de cette période… » après le récapitulatif ; **rien n'est enregistré**. | R1 et R2 inchangées. | ✅ |
| D-104 | **Incident** (poste de développement) : saisir une nouvelle règle, aller jusqu'au récapitulatif, **arrêter le service MariaDB**, Enregistrer ; redémarrer le service. | Message « La règle n'a pas pu être enregistrée. Rien n'a été modifié (la règle actuelle reste en vigueur). » | Après redémarrage : la règle ouverte est toujours ouverte, aucune nouvelle règle. | ✅ |

## 13. Saisie et complétude jugées par le métier (audit C.2 étape 3, 23/10/2026)

**Avant tout** : `SELECT code_domaine FROM ref_domaines WHERE code_domaine NOT REGEXP '^[[:alpha:]]{3}$';` doit ne rien rendre (un domaine existant au code d'une autre forme ne pourrait plus être réactivé ni modifié sans corriger son code). Domaines **de test** seulement.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-105 | Nouveau domaine : code `T1`, libellé vide ; Enregistrer. | **Une** fenêtre « Domaine » liste les 2 points (code de 3 lettres, libellé obligatoire) ; icône sur les deux champs ; focus sur le code. | Avant : une fenêtre pour la première erreur seulement. | ✅ |
| D-106 | Suite : corriger le code (`TST`), puis le libellé avec celui d'un domaine existant ; Enregistrer. | L'icône du code disparaît dès la correction ; refus « Ce libellé est déjà utilisé par un autre domaine. » | Code d'un autre domaine : « Ce code est déjà utilisé… ». | ✅ |
| D-107 | Domaine de test soumis à quota avec une règle **active** : Modifier, décocher « Soumis à quota », Enregistrer ; puis recocher. | Refus : « Ce domaine possède encore des règles de quota actives : désactivez-les d'abord (étape 3)… » ; icône sur la case, qui disparaît quand on la recoche. | Rien n'est enregistré. | ✅ |
| D-108 | Créer un domaine de test complet (étapes 1 à 4), puis le désactiver et le réactiver. | Enregistré ; étapes et pastilles comme avant ; réactivation acceptée. | — | ✅ |
| D-109 | Non-régression de la complétude : domaine conventionné sans tarif en vigueur ; domaine soumis à quota sans catégorie, sans règle en vigueur, avec une période sans règle entre deux règles ; domaine sans nature active. | Mêmes états « À compléter », mêmes messages dans la synthèse (Points d'attention) et la même étape ouverte après la création, qu'avant. | Fiche Dossier › carte « Convention du domaine » : inchangée (sans points d'attention). | ✅ |

## Validation finale

- [x] D-101 à D-104 (audit B.2 : nouvelle règle de quota d'un seul tenant) — validés par Joëlle le 21/10/2026.
- [x] D-105 à D-109 (audit C.2 étape 3 : saisie et complétude jugées par le métier) — validés par Joëlle le 23/10/2026.
- [ ] Tous les cas D-01 à D-100 sont exécutés.
- [ ] Aucune période sans règle n'a été comblée sans action de l'utilisateur.
- [ ] Un domaine simple, un domaine conventionné et un domaine soumis au quota ont été créés de bout en bout en suivant les étapes.
- [ ] Aucune règle **qui a servi** n'a pu être modifiée ou supprimée ; l'historique des décomptes est intact.
- [ ] Les lignes système restent intactes.
