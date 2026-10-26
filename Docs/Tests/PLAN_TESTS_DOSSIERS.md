# Plan de tests — Dossiers

> Version : 1.7 — Dernière mise à jour : 21/10/2026 — audit B.1 : statut relu en base, date de clôture conservée à l'archivage : D-34, D-35
> Périmètre : `UC_DossierFiche`, `DossierStatutEditor`, `DossierZoneEditor`, `GestionDossiers`.

<details>
<summary>Historique des mises à jour</summary>

- 20/10/2026 — boutons inactifs qui disent pourquoi : D-31, D-32
- 20/10/2026 — logo du domaine : D-28 à D-30
- 19/10/2026 — grilles Séances et Paiements paginées : D-24 à D-27
- 17/10/2026 — règles de clôture et état visible du dossier : D-19 à D-23, validés
- 1.2 du 15/10/2026 — UI-7 : fenêtre de zone clinique en carte, D-18 ; V1.1 du 14/10/2026 : champs du kit et consultation lisible, D-15a à D-15d

</details>

## Objectif

Valider la création, la consultation, la modification et le cycle de vie d’un dossier, ainsi que ses zones cliniques et ses restrictions de lecture seule.

## Prérequis et données de test

- Application compilée, base accessible et utilisateur SuperUser ou Admin.
- Patient de test actif, domaine actif et statuts de dossier configurés.
- Préparer un dossier actif, un dossier clôturé et un dossier archivé du même patient.
- Préfixer les données créées par `TEST_DOS_` et les supprimer après recette.

## Cas de test

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| D-01 | Ouvrir un dossier existant depuis la fiche Patient. | Le bandeau affiche code, domaine, statut et dates du bon dossier. | Données cohérentes avec `dossiers`. | ☐ |
| D-02 | Créer un dossier avec domaine actif et date d’ouverture. | Création réussie avec statut initial actif et code généré. | Nouvelle ligne `dossiers`, FKs valides. | ☐ |
| D-03 | Tenter d’enregistrer sans domaine ou avec donnée obligatoire invalide. | Enregistrement refusé ; message clair et aucune ligne créée. | Validation UI et base inchangée. | ☐ |
| D-04 | Modifier domaine, date d’ouverture et modalités d’un dossier modifiable. | Modification persistée et bandeau rafraîchi. | Valeurs mises à jour, date de modification évolue. | ☐ |
| D-05 | Ouvrir un dossier lié à un domaine devenu inactif. | Le domaine historique reste affiché, sans perte de valeur. | Consultation possible ; aucune substitution. | ☐ |
| D-06 | Éditer chacune des zones cliniques. | Le contenu RTF/TXT est sauvegardé et restitué. | Zones motif, historique, antécédents, diagnostic, traitements, objectifs, historique archivé. | ☐ |
| D-07 | Annuler l’édition d’une zone clinique. | Aucun contenu non validé n’est persistant. | Base inchangée. | ☐ |
| D-08 | Passer un dossier actif à clôturé. | Seules les transitions proposées sont acceptées ; date de clôture demandée si nécessaire. | Statut et date cohérents en base. | ☐ |
| D-09 | Réouvrir un dossier clôturé par une transition autorisée. | Retour au statut autorisé ; la modification redevient possible. | Actions d’édition réactivées. | ☐ |
| D-10 | Tenter une transition non proposée. | Action indisponible ou refusée par la couche métier. | Statut inchangé. | ☐ |
| D-11 | Archiver un dossier. | Le dossier devient entièrement consultable en lecture seule. | Zones, séances et paiements verrouillés. | ☐ |
| D-12 | Tenter de modifier, créer une séance ou créer un paiement sur un dossier archivé. | Toutes les écritures sont refusées. | Aucune ligne modifiée/créée. | ☐ |
| D-13 | Vérifier les cinq onglets. | Fiche, Suivi clinique, Documents, Séances et Paiements sont accessibles selon l’état du dossier. | Documents reste une coquille si non intégré. | ☐ |
| D-14 | Déconnecter MariaDB pendant un chargement ou enregistrement. | Erreur contrôlée, journalisée, sans état UI incohérent. | Log et absence d’écriture partielle. | ☐ |
| D-15a | Consulter un dossier (UI-7). | Carte « Paramètres du dossier » lisible : domaine, date d'ouverture et modalités en texte normal sur fond teinté, rien de modifiable. | `DefinirLectureSeule` sur `carteParametres`. | ✅ |
| D-15b | Créer / modifier un dossier : domaine (`AltheaListe`), date d'ouverture (`AltheaDate`, saisie au clavier ou calendrier). | Champs utilisables, synthèse du domaine rafraîchie, date enregistrée. | Contrôles du kit : `PLAN_TESTS_KIT_ALTHEA.md`. | ✅ |
| D-15c | Onglets Séances et Paiements : cocher « Du », choisir les deux dates, puis une date « du » postérieure à la date « au ». | La grille se filtre sur la période ; l'autre borne suit la date saisie ; décocher retire le filtre. | `CorrigerBornes`. | ✅ |
| D-17 | `DossierStatutEditor` (UI-7) : changer le statut (vers Clôturé, puis vers un autre statut). | Fond beige et petite feuille, carte « Statut du dossier » ; liste du kit ; date de clôture du kit, active seulement pour « Clôturé », libellé toujours lisible ; dates futures refusées par le calendrier. | Transition appliquée comme avant. | ✅ |
| D-16 | Bouton « Modifier » de la barre d'actions selon l'onglet. | Visible seulement sur l'onglet Fiche Dossier (en consultation) ; caché sur Suivi clinique, Documents, Séances, Paiements. En saisie : Enregistrer / Annuler visibles sur tous les onglets. | `AfficherBoutonModifier`. | ✅ |
| D-18 | `DossierZoneEditor` (UI-7) : ouvrir une zone (onglet Suivi clinique), l'agrandir, puis ouvrir l'historique archivé. | Fond beige et petite feuille ; carte titrée par le nom de la zone qui suit la fenêtre ; en-tête « Zone clinique — dossier *code* ». | Archive : titre « … (consultation) », pas d'Enregistrer ; enregistrement et export inchangés (D-06, D-07). | ☐ |
| D-15d | Bandeaux de filtres Séances et Paiements, case décochée. | « Du » entièrement visible et « au » lisible (gris foncé) à côté des dates inactives. | Corrigé le 14/10/2026. | ☐ |
| D-19 | Clôturer un dossier qui a une séance due **à venir** (planifiée ; refaire avec une séance payée d'avance). | Refus « Clôture impossible — DO… » qui cite les séances (code et date) et invite à les annuler ; aucune confirmation demandée. | Statut inchangé en base ; barre d'état « refusée ». | ✅ |
| D-20 | Annuler la séance à venir (D-19), puis clôturer. | Clôture possible ; si la séance était payée, son encaissement devient un avoir du patient. | Statut et date de clôture en base ; avoir visible (bandeau, grilles). | ✅ |
| D-21 | Clôturer un dossier dont une séance **passée** reste à régler. | Confirmation « Confirmez-vous la clôture du dossier DO… ? » avec encadré « Il reste X € à régler sur ce dossier (SE… du …) » ; boutons « Clôturer quand même » (ocre) / « Annuler ». Annuler : rien ne change. | Montant = celui du bandeau pour ce dossier. | ✅ |
| D-22 | Pastille de statut et bandeau selon l'état : Actif, En pause, Clôturé, Archivé. | Vert-de-gris ; ocre ; gris foncé texte blanc (Clôturé et Archivé). Clôturé : bandeau « Dossier clôturé le … — séances et paiements en consultation seulement. Pour le rouvrir : « Changer le statut… » » ; Archivé : « Dossier archivé le … — consultation seulement (archivage définitif) ». | Le récapitulatif garde sa hauteur quand le bandeau apparaît. | ✅ |
| D-23 | Rouvrir le dossier clôturé (Clôturé → Actif). | Bandeau retiré, pastille vert-de-gris, boutons Séances / Paiements réactivés. | Hauteur de la zone d'en-tête revenue à l'identique. | ✅ |
| D-24 | Dossier avec plus de séances qu'une page, dont des séances à venir. | Barre de pagination sous la grille ; ouverture sur la page de la ligne « Aujourd'hui » (et non forcément la page 1). | Aucune ligne coupée, pas d'ascenseur vertical, ligne « Aujourd'hui » entière. | ✅ |
| D-25 | Onglet Séances : filtrer (« Passées », recherche, période), puis changer de page. | Le résumé de la barre suit la liste filtrée ; le résumé cliquable et le quota restent calculés comme avant. | Encaisser / modifier une séance d'une autre page : la grille se recharge sans erreur. | ✅ |
| D-26 | Onglet Paiements avec plus de mouvements qu'une page. | Pagination, page 1 = les plus récents ; filtres et tri par en-tête sur toute la liste. | | ✅ |
| D-27 | Dossier court (quelques séances). | Seul le résumé « 4 séances » reste sous la grille. | | ✅ |
| D-28 | Dossier d'un domaine avec logo (Graphothérapie) puis d'un domaine sans logo (Psychologie). | Bandeau, bloc Dossier : logo lisible (jusqu'à 120 × 52) centré sous « Changer le statut… » (colonne Dossier élargie, Finances et Quotas un peu plus étroites), sur les lignes Statut à Clôture ; rien sans logo | - Logo en longueur et logo carré ; aucune valeur masquée | ☐ |
| D-29 | Nouveau dossier, puis Modifier un dossier : ouvrir la liste « Domaine ». | Logo (jusqu'à 56 × 18) devant chaque domaine qui en a un ; libellés alignés (place gardée sans logo) ; le domaine choisi montre son logo dans le champ | - Convention du domaine : logo sous les règles, jusqu'à 180 × 80 | ☐ |
| D-30 | Changer le logo d'un domaine (écran Domaines), puis rouvrir un dossier de ce domaine. | Nouveau logo partout | — | ☐ |
| D-31 | Onglet Séances, sans sélection, puis sur une séance réalisée, une séance reportée, une séance soldée : survoler les boutons gris. | « Sélectionnez une séance dans la liste. » ; Reporter : « Seule une séance planifiée peut être reportée. » ; Supprimer (reportée) : « … garde la trace du report. » ; Encaisser : « Seule une séance due et pas encore soldée peut être encaissée. » | - Dossier clôturé : « Dossier clôturé : … « Changer le statut… » » sur Nouvelle séance et les actions ; archivé : « Dossier archivé : … » | ☐ |
| D-32 | Onglet Paiements : sans sélection, sur un montant dû, sur un encaissement avec un compte non SuperUser. | Annuler l'encaissement : « Sélectionnez un encaissement… », « Seul un paiement encaissé peut être annulé… », « … réservé au SuperUser. » | - Dossier clôturé : raison du dossier sur Nouveau et Annuler | ☐ |
| D-33 | Paramètres › Dossiers : changer « Modalités de paiement proposées » (ex. « Par mois »), puis créer un dossier. | Le nouveau dossier propose « Par mois » ; un dossier existant garde sa valeur | Paramètre vide : champ vide à la création | ☐ |
| D-34 | Statut périmé : lancer Althéa **deux fois** (la seconde en double-cliquant `Althea.exe` du dossier `bin\Debug\…`). Dans les deux, ouvrir le même dossier de test **Actif**. Fenêtre B : le mettre **En pause**. Fenêtre A, sans rouvrir le dossier : Changer le statut… › Clôturé, confirmer. | Message « Le statut de ce dossier a été modifié entre-temps (depuis une autre fenêtre ou un autre poste). Fermez puis rouvrez le dossier… » ; rien n'est modifié. | Rouvert dans A : « En pause ». Journal : « Changement de statut refusé : statut du dossier modifié entre-temps (ID=…) ». | ☐ |
| D-35 | Dossier de test clôturé à une date connue → Changer le statut… › Archivé. | Archivage réussi. | `SELECT date_cloture, date_archivage FROM dossiers WHERE id_dossier = <id>;` : les **deux** dates sont renseignées (avant le 21/10/2026, la date de clôture était effacée). | ☐ |

## Contrôles de non-régression

- Les onglets Séances et Paiements se rechargent après leurs opérations sans perdre le dossier courant.
- Le retour vers `UC_PatientFiche` conserve le contexte du patient.
- Aucun accès SQL direct ne doit être nécessaire depuis l’UI.

## Nettoyage

Supprimer les dossiers de test seulement après suppression ou archivage conforme de leurs dépendances ; ne jamais supprimer de données partagées de référentiel.

## Validation finale

- [ ] Tous les cas D-01 à D-23 sont exécutés.
- [x] D-24 à D-27 (pagination des séances et des paiements) validés par Joëlle le 19/10/2026.
- [ ] D-28 à D-30 (logo du domaine).
- [ ] D-31, D-32 (boutons inactifs qui disent pourquoi).
- [ ] D-33 (modalités de paiement proposées).
- [ ] D-34, D-35 (audit B.1 : statut relu en base, date de clôture conservée).
- [ ] Aucun défaut bloquant sur les transitions ou la lecture seule.
- [ ] Les anomalies et leurs preuves sont consignées dans le suivi projet.
