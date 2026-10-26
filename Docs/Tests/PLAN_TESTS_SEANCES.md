# Plan de tests — Séances

> Version : 1.13 — Dernière mise à jour : 22/10/2026 — ligne choisie avant d'agir ; « Nouveau paiement » et colonne « Solde » retirés ; bandeau et pastilles plus clairs : S-83 à S-91
> Périmètre : onglet Séances de `UC_DossierFiche`, `SeanceEditor`, `PlanningJourViewer`, `GestionSeances`.

<details>
<summary>Historique des mises à jour</summary>

- 1.12 du même jour — audit C.2, étape 1 : règles des actions et chevauchement jugés par le métier, report refusé sur un créneau occupé : S-77 à S-82
- 1.11 du même jour — montant libre, séance encaissée et dossier figé vérifiés par le métier, audit C.1 étape 2 : S-68 à S-76
- 1.10 du même jour — absence non excusée réglée par une case du référentiel, audit B.6 : S-63 à S-66
- 1.9 du 21/10/2026 — audit B.4 : soldes lus dans la vue `v_seances_solde`, S-62
- 1.8 du même jour — filtres : Aujourd'hui, Reportées, À venir corrigé, couleurs ; S-58 à S-61
- 1.7 du même jour — audit B.3, ADR-47 : séances « À confirmer », S-53 à S-57 ; S-31 et S-43 revus
- 1.6 du même jour — audit B.2 : report d'un seul tenant, S-50 à S-52
- 1.5 du même jour — audit B.2 : enregistrement d'un seul tenant, S-45 à S-49
- 18/10/2026 — erreur de résolution du tarif à l'ouverture, S-44 ; planning du jour : pastilles, créneaux libres, S-39 à S-43
- 17/10/2026 — grille des séances V2 : pastilles, ligne « Aujourd'hui », résumé cliquable, cas S-30 à S-38
- 15/10/2026 — UI-7 : fenêtres Séance, Report et Planning du jour aux contrôles du kit, cas S-23 à S-29

</details>

## Objectif

Valider la saisie des séances, le filtrage des natures par domaine, les horaires, le contrôle de chevauchement, le verrouillage et l’intégration avec tarification, quota et paiements.

## Prérequis et données de test

- Dossier actif dans un domaine ayant au moins deux natures autorisées et un tarif applicable.
- Un autre dossier actif avec une séance existante pour créer un cas de chevauchement.
- Un dossier archivé et une séance entièrement soldée.
- Planning de test : séance A, 09:00–09:45 ; créneaux libre et chevauchant disponibles.

## Cas de test

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| S-01 | Charger l’onglet Séances d’un dossier actif. | Grille avec code, date, horaires (« 09:00 - 09:45 », début seul si pas de fin, vide si aucun), nature, statut, tarif, réglé et reste dû. | Tri et affichage lisibles ; la recherche trouve aussi une heure (ex. « 09:00 »). | ☐ |
| S-02 | Créer une séance avec nature autorisée, date, horaires et statut planifié. | Séance créée et visible dans la grille. | `id_dossier`, nature, statut, horaires et tarif persistés. | ☐ |
| S-03 | Vérifier les natures proposées pour deux domaines différents. | Seules les natures actives liées au domaine courant sont proposées. | Aucune nature étrangère au domaine. | ☐ |
| S-04 | Tenter d’enregistrer une nature non liée/inactive via contexte modifié. | Refus métier ou absence de choix ; aucune séance invalide. | Base inchangée. | ☐ |
| S-05 | Créer une séance sur un créneau libre. | Enregistrement accepté. | Planning et grille rafraîchis. | ☐ |
| S-06 | Créer ou modifier une séance avec chevauchement. | Enregistrement bloqué, message explicite. | Aucune écriture partielle. | ☐ |
| S-07 | Ouvrir le planning du jour. | Créneaux occupés cohérents avec les séances de la journée. | La modale reste informative/lecture seule. | ☐ |
| S-08 | Modifier horaires, nature ou statut d’une séance non soldée. | Modification acceptée si les validations restent satisfaites. | Valeurs affichées et persistées. | ☐ |
| S-09 | Supprimer une séance sans paiement encaissé. | Confirmation puis suppression conforme. | Ligne retirée de la grille et de la base. | ☐ |
| S-10 | Ouvrir une séance soldée puis tenter une modification. | Verrouillage appliqué ; déverrouillage manuel réservé au rôle autorisé. | Aucune modification non autorisée. | ☐ |
| S-11 | Ouvrir/créer depuis un dossier archivé. | Lecture seule ou action indisponible. | Aucune écriture. | ☐ |
| S-12 | Annuler l’édition ou fermer sans enregistrer. | Données initiales conservées. | Base inchangée. | ☐ |
| S-13 | Ajouter une nature/prix/statut via `[+]` avec droits suffisants. | Référentiel ouvert, valeur créée puis présélectionnée si applicable. | Retour explicite sans casser l’éditeur. | ☐ |
| S-14 | Répéter l’action `[+]` sans droits ou hors contexte Home. | Bouton masqué/désactivé ou accès refusé selon le contexte. | Aucun contournement des droits. | ☐ |
| S-15 | Créer une séance. | Un dû En attente au montant de la séance est créé. | Onglet Paiements. | ☐ |
| S-16 | Passer une séance non encaissée à un statut non dû (Annulée…). | Dû En attente annulé (trace conservée). | Pas d’avoir. | ☐ |
| S-17 | Repasser cette séance à un statut dû. | Dû réactivé au reste dû (pas de doublon). | — | ☐ |
| S-18 | Reporter une séance Planifiée. | Origine Reportée (lecture seule, non supprimable) ; nouvelle séance Planifiée avec même tarif et montant, dû propre. | Colonne Origine « Report de SExxx ». | ☐ |
| S-19 | Tenter de reporter une séance non Planifiée. | Refus explicite. | — | ☐ |
| S-20 | Supprimer une séance portant un encaissement (même annulé) ou origine d’un report. | Refus explicite. | Base inchangée. | ☐ |
| S-21 | Supprimer une séance sans encaissement. | Séance et dus En attente supprimés ensemble (transaction). | Aucune liaison orpheline. | ☐ |
| S-22 | Voir aussi TQ-11 à TQ-11d (cadenas, cohérence montant/tarif) et TQ-15 (quota). | — | — | ☐ |
| S-23 | `SeanceEditor` (UI-7) : ouvrir en création et en modification. | Fond beige clair et petite feuille ; cartes « Prestation » et « Rendez-vous » ; Tarif, Nature, Statut en listes du kit ; date du kit (saisie au clavier ou calendrier) ; libellés et champs alignés. | Changer la date recharge les tarifs proposés. | ✅ |
| S-24 | Boutons [+] et cadenas. | Icône centrée, survol vert pâle ; icône atténuée quand le bouton est inactif. Cadenas : fermé = montant teinté non modifiable ; clic → ouvert, montant blanc modifiable ; reclic → refermé, montant réaligné sur le tarif. | — | ✅ |
| S-25 | « Voir planning du jour ». | Bouton du kit (clair, icône œil) ; ouvre le planning de la date saisie. | — | ✅ |
| S-26 | Durée. | Toujours sur fond teinté, non modifiable, y compris par les flèches ; recalculée quand les heures changent. | Comportement voulu : la durée se calcule depuis les heures. | ✅ |
| S-27 | Consultation : séance soldée (verrou), séance reportée, dossier archivé. | Listes, date et montants lisibles (texte normal, fond teinté) ; listes d'heures encore grisées (champ d'heure du kit à venir). Déverrouillage SuperUser : la saisie se rouvre. | `DefinirSaisieActive`. | ✅ |
| S-28 | `DialogReportSeance` (UI-7) : reporter une séance Planifiée. | Fond beige et petite feuille, carte « Nouvelle date de la séance » ; date du kit ; case « Préciser les horaires » du kit qui active les heures (encore Windows) ; Reporter fonctionne comme avant. | Voir S-18. | ✅ |
| S-29 | `PlanningJourViewer` (UI-7) : « Voir planning du jour » depuis une séance, puis changer de jour. | Fond beige et petite feuille, carte « Séances du jour » ; date du kit ; la grille se recharge à chaque nouveau jour ; fenêtre redimensionnable, carte et grille suivent. | — | ✅ |
| S-30 | Grille V2 : ouvrir l'onglet Séances d'un dossier avec des séances passées, à venir, reportées, annulées. | Colonnes : Date (avec le jour), Horaires, Nature, Statut, Paiement, Tarif, Réglé, Solde (+ Période et Quota pour un domaine sous quota). | Plus de Code, Origine, Prévisionnel, Exigible, Reste dû, Avoir. | ☐ |
| S-31 | Pastille Statut. | Bleu à venir, ocre « À confirmer » pour une séance planifiée passée (21/10/2026, avant : « Réalisée »), vert-de-gris réalisée, orange absence non excusée, gris non due ; « → date · code » à droite d'une séance reportée (date et code de la remplaçante, 21/10/2026), « report de S… » sur sa remplaçante. | — | ☐ |
| S-32 | Pastille Paiement. | Réglée / Payée d'avance (vert), À régler · montant et Reste · montant (rose), Prévue · montant (bleu), Avoir · montant (mauve), Non due (gris) ; absence non facturable par la convention : « Non facturée (convention) ». | Montants = tarif − réglé. | ☐ |
| S-33 | Solde. | « −60,00 » en rose foncé pour une séance due passée non réglée ; « +60,00 » en mauve pour un avoir ; vide sinon (séance à venir, réglée, non due sans avoir). | — | ☐ |
| S-34 | Lignes non dues. | Annulée, malade, reportée : texte gris, pastilles éclaircies. | — | ☐ |
| S-35 | Ligne « Aujourd'hui ». | Entre la dernière séance à venir et la première passée, avec la date du jour ; absente s'il n'y a que des séances passées ou à venir ; disparaît après un tri par en-tête, revient au changement de filtre. | Sélection, double-clic et boutons inchangés. | ☐ |
| S-36 | Résumé cliquable. | Nombres et montants justes (calculés sur tout le dossier) ; pastilles nulles absentes ; clic = filtre correspondant, pastille cerclée et en gras ; reclic = « Toutes ». | Cohérent avec la liste « Filtre » (« Réalisées » et « Avec avoir » ajoutés). | ☐ |
| S-37 | Infobulle d'une ligne. | Code de la séance, tarif, période et quota (domaine sous quota), mention de report. | — | ☐ |
| S-38 | Recherche texte. | Trouve aussi les libellés des pastilles (« à régler », « réalisée »…). | — | ☐ |
| S-39 | Planning du jour (aujourd'hui ou plus tard) avec deux séances séparées par un long trou. | Repère pointillé « Libre · hh:mm – hh:mm » entre elles, et avant / après si la journée le permet (plage SEANCE_HEURE_DEBUT_GRILLE – FIN). | Un trou plus court qu'une séance par défaut n'apparaît pas. | ☐ |
| S-40 | Planning du jour d'aujourd'hui en cours de journée. | Aucun créneau libre avant l'heure qu'il est (arrondie au pas des heures). | | ☐ |
| S-41 | Planning d'un jour passé. | Séances seules, aucun créneau libre. | | ☐ |
| S-42 | Séance annulée ou reportée dans la journée. | Ligne affichée (pastille grise) mais sa place compte comme libre ; le contrôle de chevauchement accepte une nouvelle séance sur ce créneau. | Même règle des deux côtés (`OccupeSonCreneau`). | ☐ |
| S-43 | Planning : statut et présentation. | Statut en pastille (couleurs de la grille des séances, « À confirmer » pour une séance planifiée passée — 21/10/2026) ; heures centrées, tarif à droite ; cliquer un créneau libre ne le sélectionne pas. | Erreur de chargement : message `DialogChoix` (plus de boîte Windows). | ☐ |
| S-44 | Ouvrir une séance existante (Modifier), puis en créer une ; changer le statut (ex. Réalisée → Annulée par le patient). | Le tarif suit le statut comme avant ; **aucune** ligne « Erreur résolution tarif séance (SeanceEditor) » dans les journaux. | Administration › Journaux, « Erreurs seulement » (correction du 18/10/2026). | ✅ |

### Enregistrement d'un seul tenant (audit B.2, 21/10/2026)

Patient et dossier **de test**. Journaux : Administration › Journaux.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| S-45 | **Nouvelle séance** : date passée, une ligne dans Notes et une dans Évolution, statut **Réalisée** → Enregistrer. | Fenêtre fermée ; la séance apparaît « Réalisée », pastille « À régler · 60 € ». | Rouvrir : notes, évolution et statut présents. **Détail paiements** : une seule ligne, En attente 60 €. Journal : « Dû en attente réaligné à 60,00 € (séance ID=…) » puis « Séance créée (ID=…, id_dossier=…, statut REALISEE) ». | ☐ |
| S-46 | **Nouvelle séance** directement au statut **Annulée par le praticien** → Enregistrer. | Séance « Annulée », montant 0 €, pastille « Non due ». | **Détail paiements** : aucune ligne (comme avant). | ☐ |
| S-47 | **Modifier** une séance planifiée non encaissée : SuperUser, cadenas, montant 50 € ; modifier les notes ; statut Réalisée → Enregistrer. | Tout est enregistré en une fois : montant 50 €, notes, statut. | Détail paiements : une ligne En attente **50 €**. Journal : une seule ligne « Séance enregistrée (ID=…, statut REALISEE) ». | ☐ |
| S-48 | **Incident** (poste de développement) : ouvrir Nouvelle séance, tout saisir ; **arrêter le service MariaDB** (Services Windows) ; Enregistrer. Redémarrer le service ; Enregistrer à nouveau. | 1er essai : fenêtre « La séance n'a pas pu être enregistrée ; rien n'a été modifié… » ; la fenêtre Séance **reste ouverte** avec la saisie. 2e essai : séance créée. | **Une seule** séance créée (pas de doublon), avec son dû. Journal : l'erreur du 1er essai. | ☐ |
| S-49 | Non-régression : **Reporter** une séance planifiée non encaissée ; puis passer une séance **encaissée** de Planifiée à Annulée dans la fenêtre Séance. | Report : origine « Reportée » sans dû actif, nouvelle séance Planifiée avec son dû. Annulation : la séance est enregistrée, **puis** la réaffectation est proposée (depuis le 21/10/2026 : l'utilisation de l'avoir, plan Paiements P-65 à P-70). | Fermer sans valider (« Garder l'avoir ») : l'encaissement reste en avoir, le statut Annulée est bien enregistré. | ☐ |
| S-50 | **Reporter** une séance planifiée **non encaissée** (60 €) au 7e jour suivant. | Origine « Reportée → jj/mm · S… » ; nouvelle séance Planifiée « report de S… », même montant. | Détail paiements : origine sans dû actif (ligne « Clos »), nouvelle séance une ligne En attente 60 €. Journal : « Séance reportée (origine ID=… → nouvelle ID=…) », aucune erreur. | ☐ |
| S-51 | **Écran périmé** : lancer Althéa deux fois (2e : `Althea.exe` de `bin\Debug\…`), même dossier de test. Fenêtre B : passer une séance planifiée en **Réalisée**. Fenêtre A, sans recharger : **Reporter** cette séance. | Message « Seule une séance planifiée peut être reportée. » (statut relu en base) ; rien n'est modifié. | Recharger dans A : la séance est Réalisée, **aucune** séance de remplacement créée. (Le message « Cette séance a changé entre-temps… » couvre le cas, impossible à provoquer à la main, d'un changement dans la fraction de seconde du report.) | ☐ |
| S-52 | **Incident** (poste de développement) : sélectionner une séance planifiée, Reporter, choisir la date ; **arrêter le service MariaDB** avant de valider ; valider. Redémarrer le service. | Message « Le report n'a pas pu être enregistré ; rien n'a été modifié (la séance reste planifiée)… ». | Après redémarrage : séance toujours Planifiée, aucune remplaçante ; un nouveau report fonctionne. | ☐ |

### Séance passée non confirmée — « À confirmer » (audit B.3, ADR-47, 21/10/2026)

Dossier **de test**. Requête de contrôle (identifiants seulement) : `SELECT s.code_seance, ss.code_statut_seance FROM seances s JOIN ref_statuts_seance ss ON ss.id_statut_seance = s.id_statut_seance WHERE s.id_dossier = <id>;`

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| S-53 | Créer une séance **Planifiée** hier (encodage rétroactif) ; fermer puis rouvrir le dossier. | Pastille ocre **« À confirmer »** ; résumé « 1 à confirmer » ; clic sur cette pastille du résumé (ou filtre « À confirmer ») : la séance seule. | Requête : la séance est toujours **PLANIFIEE** en base (avant : passée en REALISEE à l'ouverture). Pastille Paiement : « À régler · 60 € » (toujours due). | ☐ |
| S-54 | Ouvrir cette séance, choisir **Réalisée**, Enregistrer. | Pastille vert-de-gris « Réalisée » ; plus comptée « à confirmer ». | Requête : REALISEE. | ☐ |
| S-55 | Séance planifiée d'hier **encaissée** (60 €) : l'ouvrir. | Pas de verrou (saisie ouverte) tant qu'elle est « À confirmer » ; passer en Réalisée et enregistrer, la rouvrir : **verrou** (seul un SuperUser le lève). | Règle du verrou : statut réel « Réalisée » et encaissée. | ☐ |
| S-56 | Séance planifiée d'hier, non encaissée : **Reporter**. | Report accepté (origine « Reportée », nouvelle séance planifiée). | — | ☐ |
| S-57 | Planning du jour (aujourd'hui, avec une séance du matin passée et non pointée) ; aperçu du patient (carte « Aujourd'hui »). | Planning : pastille « À confirmer » ; aperçu : « … · à confirmer ». | — | ☐ |

### Filtres de la grille (demandes de Joëlle, 21/10/2026)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| S-58 | Dossier avec des séances à venir, dont une **reportée** et une **annulée** dans le futur : cliquer la pastille « à venir » du résumé, puis choisir le filtre « À venir ». | Seules les séances **dues** à venir ; ni la reportée ni l'annulée. | Le nombre de lignes égale le nombre de la pastille du résumé. | ✅ |
| S-59 | Filtre **« Aujourd'hui »**. | Les séances du jour, quel que soit leur statut (à venir, à confirmer, réalisée, annulée…). | — | ✅ |
| S-60 | Filtre **« Reportées »**. | Seules les séances au statut Reportée (leur remplaçante n'y est pas). | Mention « → date · code » visible. | ✅ |
| S-61 | Ouvrir la liste des filtres (Séances, puis Paiements) ; choisir un axe coloré. | Chaque axe en pastille aux couleurs de la grille et du résumé (À venir bleu, À confirmer ocre, Réalisées vert-de-gris, Absences orange, Reportées et Non dues gris, Exigibles rose, Réglées vert, Avec avoir mauve) ; « Aujourd'hui » en bleu plein, « Passées » au trait (pointillé gris) ; « Toutes » en texte, aligné. Champ fermé : la pastille après « Filtre ». | Ordre des séances : Toutes, Aujourd'hui, À venir, À confirmer, Passées, Réalisées, Absences, Reportées, Non dues, Exigibles, Réglées, Avec avoir. | ✅ |

### Soldes lus en base (audit B.4, ADR-48, 21/10/2026)

Migration v2.49.0 appliquée (plan Paiements, P-71).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| S-62 | Dossier de test avec des séances soldée, partielle, à venir non réglée, annulée encaissée, reportée ; puis le **planning du jour** (séance de test aujourd'hui). | Colonnes Réglé, pastilles Paiement (Soldée, Reste 40 €, Prévu, Avoir · 60 €…) et résumé (à régler, avoir) **comme avant la migration** ; « À confirmer » inchangé. | Après un encaissement, réglé et reste dû se mettent à jour aussitôt. Le planning du jour montre les mêmes pastilles de paiement. | ✅ |

### Absence non excusée réglée dans le référentiel (audit B.6, 22/10/2026)

**Avant tout** : appliquer `Docs/Database/Migration/migration_v2.53.0_statut_est_absence.sql` dans HeidiSQL, en `root`. **Patient de test** ; domaine de test **D-NF** (« Absence non excusée facturable » décochée).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| S-63 | Référentiels › Statuts de séance : ouvrir « Absence non excusée », puis « Réalisée ». | Nouvelle case « Absence non excusée (no-show) » : **cochée** pour la première, décochée pour la seconde ; infobulle explicative. | `SELECT code_statut_seance, est_absence FROM ref_statuts_seance;` → 1 pour ABSENTE seulement. | ✅ |
| S-64 | Non-régression : séance de test passée dans D-NF → statut Absence non excusée (comme P-72). | Pastille « Absence » ; non facturée (aucun à régler, pas proposée dans « Nouveau paiement ») ; grille Patients : colonne Abs. +1 ; « Dernière séance » ne la prend pas. | Filtre « Absences » de l'onglet Séances : elle y est. | ✅ |
| S-65 | Créer un statut de test « Absence tardive » (code ABSENCE_TEST), **dû**, case « Absence non excusée » **cochée** ; l'appliquer à une autre séance passée de D-NF. | Même comportement que S-64 : non facturée, comptée dans les absences, filtre « Absences ». À la création d'une séance avec ce statut, montant proposé 0 dans D-NF. | Puis **décocher** la case et rouvrir le dossier : la séance redevient une séance due ordinaire (à régler). | ✅ |
| S-66 | Nettoyage : remettre l'autre séance à un statut ordinaire, désactiver le statut de test. | Plus aucune trace de comportement d'absence pour le statut de test. | La case de « Absence non excusée » reste cochée. | ✅ |
| S-67 | (Demande de Joëlle, 22/10/2026) Séance de test **Annulée (patient)** à 10:00 ; planifier une **autre** séance de test à 10:00 le même jour ; rouvrir la séance annulée → statut **Absence non excusée** → Enregistrer. Puis la passer en **Maladie**. | Enregistrement accepté les deux fois, sans message de conflit d'horaire. | Non-régression : la repasser en **Planifiée** → refus « Ce créneau chevauche une séance déjà planifiée ». | ✅ |

> Le « tout ou rien » face à une panne **au milieu** de l'enregistrement (séance écrite, dû pas encore) ne se provoque pas à la main ; il sera couvert par les tests automatisés (lot D). S-48 vérifie le message et l'absence de doublon.

### Montant libre, séance encaissée et dossier figé côté métier (audit C.1, étape 2, 22/10/2026)

**Patient de test** uniquement ; un dossier de test **actif** ; un compte **User** de test et un compte **SuperUser** (ou Admin). Les refus du métier lui-même ne se provoquent pas depuis les écrans, qui grisent déjà les boutons : ils seront couverts par les tests automatisés (lot D). Ces cas vérifient les écrans et la non-régression.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| S-68 | Compte **User** : dossier de test › Nouvelle séance. | Cadenas du montant **grisé** ; au survol : « Modifier le montant d'une séance est réservé au SuperUser ou à l'administrateur. » | Compte SuperUser (ou Admin) : cadenas actif, comme avant. | ✅ |
| S-69 | **SuperUser** : séance de test planifiée ; ouvrir le cadenas, montant libre (ex. 45 € au lieu du tarif) ; Enregistrer. Puis compte **User** : rouvrir la séance, statut **Réalisée**, Enregistrer. | Accepté ; le montant reste **45 €** (fenêtre et grille). | Avant : le montant repassait au tarif sans prévenir. Le dû suit 45 €. | ✅ |
| S-70 | Compte **User**, même séance : choisir un **autre tarif** dans la liste ; Enregistrer. | Blocage « Le montant indiqué … ne correspond pas au tarif sélectionné … faire appel à un SuperUser » ; rien enregistré. | Fermer sans enregistrer. | ✅ |
| S-71 | **SuperUser**, même séance : ouvrir puis **refermer** le cadenas ; Enregistrer. | En refermant, le montant du tarif réapparaît ; il est enregistré. | Le montant libre est abandonné. | ✅ |
| S-72 | Non-régression : séance de test **réalisée et encaissée**. Compte User : l'ouvrir. SuperUser : l'ouvrir, **Déverrouiller**, changer l'heure de fin, Enregistrer. | User : consultation, pas de bouton Déverrouiller. SuperUser : accepté. | Comme avant ; la règle est aussi vérifiée par le métier. | ✅ |
| S-73 | Dossier de test **clôturé** (Changer le statut… › Clôturé). | Boutons des onglets Séances et Paiements grisés, raison au survol (« Dossier clôturé : séances et paiements en consultation seulement. Pour reprendre le suivi : « Changer le statut… ». ») ; **Modifier** la fiche reste actif ; zones cliniques modifiables. | Rouvrir (› Actif) : tout redevient actif tout de suite, sans recharger le dossier. | ✅ |
| S-74 | Dossier de test **archivé** (⚠️ définitif : un dossier créé pour ce test, sur le patient de test). | **Modifier** la fiche grisé, survol « Dossier archivé : consultation seulement (archivage définitif). » ; zones, séances et paiements en consultation. | Juste après l'archivage, sans recharger le dossier. | ✅ |
| S-75 | Non-régression, dossier de test actif : créer, modifier, reporter, supprimer une séance ; encaisser ; utiliser un avoir ; annuler un encaissement (SuperUser). | Tout comme avant. | Journal : aucune nouvelle ligne d'erreur. | ✅ |
| S-76 | (Facultatif) Arrêter le service MariaDB ; dans une zone clinique de test, taper un texte, Enregistrer ; puis Encaisser séance › Enregistrer. Redémarrer MariaDB. | Fenêtre d'erreur « La zone n'a pas pu être enregistrée ; rien n'a été modifié et votre texte est toujours là… » ; la fenêtre reste ouverte. Même chose pour le paiement. | Avant : seulement un message dans la barre d'état. | ☐ |

### Règles des actions et chevauchement jugés par le métier (audit C.2, étape 1, 22/10/2026)

**Patient de test** uniquement ; un dossier de test **actif** ; pour S-77, deux séances de test **Planifiées** le même jour à venir : **A** de 10:00 à 10:45 et **B** de 14:00 à 14:45.

Les refus du métier qui doublent ceux de la fenêtre (statut « Reportée » choisi, créneau occupé à l'enregistrement) ne se provoquent pas depuis l'écran, qui les arrête avant : ils seront couverts par les tests automatisés (lot D).

Le refus du **report** sur un créneau occupé est nouveau et se teste ici.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| S-77 | **Reporter** A au **même jour**, de 14:15 à 15:00. | Refus « Ce créneau chevauche la séance <code de B> (14:00 - 14:45) : choisissez un autre horaire. » (fenêtre « Report impossible »). | A reste **Planifiée**, aucune nouvelle séance dans la grille ; rien d'autre n'a changé. Puis reporter A de 15:00 à 15:45 : accepté, comme avant. | ✅ |
| S-78 | Reporter une séance planifiée **à la même heure le lendemain**, sans autre séance ce jour-là. | Accepté, comme avant (la nouvelle séance est planifiée, l'origine reportée). | Non-régression du report d'une séance encaissée : l'utilisation de l'avoir est proposée comme avant. | ✅ |
| S-79 | **Nouvelle séance** : ouvrir la liste Statut. Puis ouvrir une séance **planifiée** existante : liste Statut. Puis ouvrir une séance **reportée**. | Nouvelle : **Planifiée** et **Réalisée** seulement. Existante : tous les statuts actifs **sauf Reportée**. Reportée : consultation, statut « Reportée » affiché. | Comme avant (règle déplacée dans le métier). | ✅ |
| S-80 | Fenêtre Séance : déplacer une séance de test sur un créneau occupé par une autre séance ; Enregistrer. Puis refaire S-67. | Message de la fenêtre comme avant : « Ce créneau chevauche une séance déjà planifiée : <patient> — hh:mm à hh:mm (statut) … » ; la fenêtre reste ouverte. S-67 : même résultat qu'au 22/10. | Règle « reste non tenue » désormais dans le métier. | ✅ |
| S-81 | Onglet Séances : sélectionner tour à tour une séance **Planifiée**, **Réalisée**, **Reportée**, **Annulée**, **soldée**. | Reporter : actif sur la Planifiée seulement ; ailleurs grisé, survol « Seule une séance planifiée peut être reportée. » Supprimer : grisé sur la Reportée (« Une séance reportée reste dans le dossier… »). Encaisser : actif sur une séance due non soldée seulement (« Seule une séance due et pas encore soldée peut être encaissée. »). | Comme avant. | ✅ |
| S-82 | Non-régression, dossier de test actif : créer, modifier (statut, horaires), supprimer une séance jamais encaissée. | Tout comme avant. | Journal : aucune nouvelle ligne d'erreur. | ✅ |

### Onglets Séances et Paiements : ligne choisie, « Nouveau paiement » et « Solde » retirés, bandeau et pastilles plus clairs (relevés de Joëlle au test P-93, 22/10/2026)

**Patient de test** uniquement ; un dossier de test actif avec des séances de plusieurs statuts, dont une encaissée et une due non réglée ; un avoir sur le patient pour S-86.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| S-83 | Ouvrir le dossier de test, onglet **Séances**, sans rien cliquer. | **Aucune** ligne en surbrillance. Modifier, Reporter, Encaisser, Détail paiements, Supprimer **grisés**, survol « Sélectionnez une séance dans la liste. » ; Nouvelle séance active. | Cliquer une ligne : les boutons suivent les règles habituelles (S-81). | ✅ |
| S-84 | Sélectionner une séance due → **Encaisser séance** → enregistrer ; puis changer de page et de filtre. | Après l'enregistrement, la grille se recharge **sans** ligne sélectionnée ; boutons grisés. Idem après un changement de page ou de filtre. | Avant : la première ligne était prise d'office. | ✅ |
| S-85 | Onglet **Paiements**, sans rien cliquer. | Plus de bouton « Nouveau paiement » ; « Annuler l'encaissement » à gauche de la barre, grisé, survol « Sélectionnez un encaissement dans la liste. » | Cliquer une ligne encaissée (SuperUser) : le bouton s'active comme avant. | ✅ |
| S-86 | Onglet Paiements : **double-clic** sur la ligne en attente d'une séance due, le patient ayant un avoir. | La proposition d'utiliser l'avoir s'ouvre, puis la fenêtre Paiement sur **cette** séance (code dans l'en-tête, rappel sous « Séance ») ; pas de liste de séances. | Non-régression de l'encaissement (P-91, P-92). | ✅ |
| S-87 | Onglet Séances : regarder les colonnes. | Plus de colonne **Solde** ; Tarif et Réglé alignés à droite ; les montants à régler, restes et avoirs se lisent dans la pastille **Paiement**. | La grille tient sans ascenseur horizontal. | ✅ |
| S-88 | Non-régression : modifier, reporter, supprimer une séance ; annuler un encaissement (SuperUser) ; Détail paiements. | Tout comme avant, une fois la ligne choisie. | Journal : aucune nouvelle ligne d'erreur. | ✅ |
| S-89 | Bandeau d'un dossier de test dont le patient a **un autre dossier** avec un montant dû (cas de DO000031). | **Montants dus** : « 0 € ici · 68,00 € tous dossiers » ; **Prévu** : le reste à payer des séances à venir (ex. « 28,00 € ») ; infobulle de chaque montant : détail par dossier. | Patient à un seul dossier : un seul montant, sans « ici / tous dossiers ». | ✅ |
| S-90 | Bandeau d'un dossier dont le patient a un **avoir**, puis d'un dossier sans avoir. | Avoir : 4ᵉ ligne du bloc Finances, sous Prévu, sans chevauchement ; absente sans avoir. | Texte de l'avoir comme avant (« dont … ici », « (autre dossier) »). | ✅ |
| S-91 | Onglet **Paiements** : encaissement partiel d'une séance **à venir** (cas SE000083), puis d'une séance **passée**. | « **Prévue · reste 45 €** » (bleu) ; « **Reste · 20 €** » (rose) — les mêmes mots que la pastille Paiement de l'onglet Séances. | Avant : « Reste 45 € » et « Reste 20 € dû ». | ✅ |

## Non-régression transversale

- La création/modification relance la résolution tarifaire et le contrôle quota prévus.
- Le montant figé n’est pas rétroactivement modifié par une évolution de référentiel.
- Les valeurs Réglé et Reste dû sont rafraîchies après une opération de paiement.

## Validation finale

- [x] S-45 à S-49 (audit B.2 : enregistrement d'un seul tenant) — validés par Joëlle le 21/10/2026.
- [x] S-50 à S-52 (audit B.2 : report d'un seul tenant) — validés par Joëlle le 21/10/2026.
- [x] S-53 à S-57 (audit B.3 : séances « À confirmer ») — validés par Joëlle le 21/10/2026.
- [x] S-58 à S-61 (filtres : Aujourd'hui, Reportées, À venir, couleurs) — validés par Joëlle le 21/10/2026.
- [x] S-63 à S-67 (audit B.6 : absence non excusée réglée par une case du référentiel ; pas de contrôle d'horaire entre statuts non tenus) — validés par Joëlle le 22/10/2026.
- [x] S-62 (audit B.4 : soldes lus dans la vue) — validé par Joëlle le 21/10/2026.
- [x] S-68 à S-75 (audit C.1, étape 2 : montant libre, séance encaissée, dossier figé côté métier) — validés par Joëlle le 22/10/2026 ; S-76 facultatif.
- [x] S-77 à S-82 (audit C.2, étape 1 : règles des actions et chevauchement jugés par le métier ; report refusé sur un créneau occupé) — validés par Joëlle le 22/10/2026.
- [x] S-83 à S-91 (ligne choisie avant d'agir ; « Nouveau paiement » et colonne « Solde » retirés ; bandeau « ici / tous dossiers » et « Prévu » ; pastilles État alignées) — validés par Joëlle le 22/10/2026.
- [ ] Tous les cas S-01 à S-29 sont exécutés.
- [ ] Aucun chevauchement ne peut être persisté.
- [ ] Les états archivé et soldé empêchent les écritures non autorisées.
