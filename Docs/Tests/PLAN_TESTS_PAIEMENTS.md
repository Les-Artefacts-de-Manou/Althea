# Plan de tests — Paiements

> Version : 1.17 — Dernière mise à jour : 22/10/2026 — bouton « Nouveau paiement » retiré : P-31, P-80 et le parcours adaptés ; tests des finitions : S-83 à S-88 du plan Séances
> Périmètre : onglet Paiements de `UC_DossierFiche`, `PaiementEditor`, `DialogUtilisationAvoir`, `GestionPaiements`.

<details>
<summary>Historique des mises à jour</summary>

- 1.16 du même jour — audit C.2, étape 1 : encaissement jugé par le métier, P-91 à P-95
- 1.15 du même jour — audit B.5 : utilisation d'un avoir reliée à son encaissement, P-86 à P-90
- 1.14 du même jour — audit B.5 : annulation enregistrée dans des colonnes, P-81 à P-85 ; P-18 revu
- 1.13 du 21/10/2026 — audit B.5 : mode de paiement relié par identifiant, P-76 à P-80
- 1.12 du même jour — audit B.4 : reste dû et avoir lus dans la vue `v_seances_solde`, absence non facturable, P-71 à P-75
- 1.11 du même jour — audit B.2 : la réaffectation est remplacée par l'avoir, P-65 à P-70 ; P-13b, P-48 à P-50, P-61, P-62 remplacés
- 1.10 du même jour — audit B.1 : report de la seule part restante, paiement réparti, annulation après utilisation en avoir, une seule ligne de dû : P-60 à P-64
- 18/10/2026 — détail des paiements d'une séance : pastille État, P-54 et P-55
- 17/10/2026 — étapes 5 à 7 validées, P-15 facultatif reste ouvert ; P-14 et P-36 : clôture contrôlée, voir Dossiers D-19 à D-23
- 1.7 du même jour — bandeau et grille Patients : P-57, P-58 ; refus d'annuler un règlement par avoir : P-59 ; étape 4 validée
- 1.6 du même jour — codes dans les fenêtres : P-56 ; étapes 0 à 3 validées
- 1.5 du même jour — attentes closes masquées, refus de saisie dans la fenêtre : P-07, P-13a, P-40 revus, cas P-54 et P-55
- 1.4 du même jour — parcours recommandé ; P-08 précisé, P-11 et P-12 sans objet, P-50 corrigé, P-53 ✅
- 1.3 du même jour — montants affectés, avoirs alignés, reliquat de réaffectation, trop-perçu : P-21, P-24, P-39, P-45 revus, cas P-48 à P-53
- 1.2 du même jour — grille en relevé chronologique : P-01, P-04 à P-07 revus, P-22 et P-23 remplacés, cas P-38 à P-47
- 1.1 du 15/10/2026 — UI-7 : fenêtres Paiement et dialogues de paiement aux contrôles du kit, cas P-34 à P-37

</details>

## Objectif

Valider l’affectation des paiements à une séance, les règlements partiels, les statuts, le reste dû calculé, les avoirs et les restrictions métier.

## Prérequis et données de test

- Dossier actif ayant : une séance de 60 € non réglée, une séance de 40 € non réglée et une séance soldée.
- Modes de paiement actifs et statuts `En attente`, `Encaissé`, `Annulé` configurés.
- Utilisateur avec droits de création/modification ; un compte sans les droits pour le test d’autorisation.

## Cas de test

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-01 | Charger l’onglet Paiements. | Colonnes Date, Mouvement, Pour, Mode, Montant, État cohérentes ; plus récent en haut. | Recherche et filtres initiaux. | ✅ |
| P-02 | Créer un paiement depuis le dossier sans séance imposée. | Seules les séances non soldées sont proposées ; montant proposé = reste dû. | Contexte séance `jj/mm/aaaa (Nature)`. | ✅ |
| P-03 | Ouvrir le paiement depuis une séance non soldée. | La séance est préselectionnée et non ambiguë. | Aucun autre dossier accessible. | ✅ |
| P-04 | Enregistrer un encaissement total de 60 €. | Paiement créé ; pastille « Soldée » (« Payée d'avance » si la séance est à venir). | Somme des seuls `PAYE` = tarif figé. | ✅ |
| P-05 | Enregistrer deux encaissements partiels de 20 € puis 40 €. | Pastille « Reste · 40 € » (« Reste 40 € dû » avant le 22/10/2026) puis « Soldée » ; mouvement « Paiement partiel ». | Calcul exact après chaque enregistrement. | ✅ |
| P-06 | Enregistrer un paiement En attente. | Date « — », pastille « En attente » bleue (« À régler » rose si la séance due est passée) ; reste dû financier inchangé. | Il n’est pas compté dans les encaissements. | ✅ |
| P-07 | Annuler un encaissement (SuperUser). | Ligne grise, montant barré, pastille « Annulé » atténuée (motif à droite) ; montant exclu du reste dû. | Priorité d’affichage ADR-25 respectée. | ✅ |
| P-08 | Double-cliquer un paiement (filtré ou ancien). | La fenêtre Paiement s'ouvre pour **encaisser** la séance liée et rappelle sa date et sa nature ; message « déjà soldée » ou « n'est pas due » sinon. | Il n'existe pas de modification d'un paiement existant. | ✅ |
| P-09 | Tenter de payer une séance entièrement soldée. | Séance absente des choix ou enregistrement refusé. | Aucun surpaiement. | ✅ |
| P-10 | Tenter un montant nul, négatif, invalide ou supérieur au reste dû. | Validation claire ; aucune affectation invalide. | Base inchangée. | ✅ |
| ~~P-11~~ | ~~Réaffecter un paiement vers une autre séance non soldée.~~ | Sans objet (17/10/2026) : l'interface ne modifie pas un paiement existant ; la réaffectation ne se fait qu'au report ou au passage en statut non dû (P-13b, P-48 à P-50). | — | — |
| ~~P-12~~ | ~~Réaffectation vers autre dossier ou séance soldée.~~ | Sans objet (voir P-11) ; la fenêtre de réaffectation ne propose que les séances dues du dossier. | — | — |
| P-13 | Filtrer par statut et par séances soldées/restant dues. | Le résultat correspond aux critères et aux calculs réels. | Recherche libre inclut séance, nature, mode et référence. | ✅ |
| P-13a | Annuler sans report une séance non payée. | Son paiement attendu disparaît du relevé (dans tous les filtres) ; il reste dans **Détail paiements** de la séance, statut « Clos », « Paiement attendu clos — séance non due ». | Statut de séance non dû (`est_du = 0`) ; paiement en attente annulé sans encaissement. | ✅ |
| ~~P-13b~~ | ~~Reporter ou annuler une séance déjà payée, puis réaffecter explicitement l'encaissement à une autre séance.~~ | Remplacé le 21/10/2026 (réaffectation supprimée, remplacée par l'avoir — décision de Joëlle) : voir P-65 à P-70. | — | — |
| P-14 | Créer/modifier depuis un dossier clôturé ou archivé, ou avec un compte non autorisé. | Écriture interdite et message cohérent ; le bandeau du dossier annonce « consultation seulement ». Un dossier ne se clôture pas tant que des séances sont à venir (Dossiers D-19). | Aucun contournement des droits. | ✅ |
| P-15 | Provoquer une erreur base pendant l’enregistrement. | Message maîtrisé, journalisation, aucune liaison orpheline. | Transaction ou rollback vérifiable. | ☐ |

## Cas de test — Fiabilisation 25–26/09/2026

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-16 | Utilisateur simple : sélectionner un encaissement. | Bouton d’annulation inactif. | Aucun paiement supprimable. | ✅ |
| P-17 | SuperUser : annuler un encaissement, valider sans motif. | Champ signalé, message, fenêtre ouverte. | Base inchangée. | ✅ |
| P-18 | SuperUser : annuler un encaissement de 60 € avec motif. | Ligne Annulée (grisée/barrée) avec « Annulé le … par … : motif » (infobulle) ; nouveau dû En attente 60 €. | `statut_paiement = ANNULE`, colonnes de l'annulation (P-81, depuis le 22/10/2026), log. | ✅ |
| P-19 | Encaissement partiel 20 € sur 60 €. | Dû En attente réaligné à 40 € (une seule ligne). | Pas de doublon En attente. | ✅ |
| P-20 | Annuler l’encaissement de 20 € ci-dessus. | Dû En attente repasse à 60 €. | Réglé = 0 ; annulé non compté. | ✅ |
| P-21 | Séance encaissée 60 € passe Annulée, avoir gardé (« Garder l'avoir »). | Avoir 60 € affiché (en-tête dossier, grilles, filtre Avoirs, grille Patients). | Aucun dû En attente actif. | ✅ |
| ~~P-22~~ | ~~Grille groupée : cliquer un en-tête de séance.~~ | Remplacé le 17/10/2026 (plus de regroupement) : voir P-38. | — | — |
| ~~P-23~~ | ~~Cliquer un en-tête de colonne : aucun tri.~~ | Remplacé le 17/10/2026 (tri rétabli) : voir P-42. | — | — |
| P-24 | Filtres Avoirs et Séances reportées. | Avoirs : lignes « Avoir · X € » seulement ; Séances reportées : lignes d'une séance reportée ou issue d'un report. | Avoir utilisé absent du filtre Avoirs. | ✅ |
| P-25 | Bouton « Détail des paiements » sur une séance. | `DialogPaiementsSeance` : synthèse tarif/réglé/reste/avoir + tous les mouvements. | Lecture seule ; double-clic séance ouvre toujours `SeanceEditor`. | ✅ |
| P-26 | Bouton Encaisser sur séance reportée, non due ou soldée. | Inactif. | — | ✅ |
| P-27 | Migration v2.20.0 appliquée. | Mode `AVOIR` présent, `est_avoir = 1` ; ligne 2.20.0 dans `tec_meta_schema`. | Mode « Avoir » absent de la liste de `PaiementEditor`. | ✅ |
| P-28 | Avoir 60 € ; Encaisser une séance due 45 € d’un autre dossier du patient, répondre Oui. | Séance soldée ; paiement « Avoir » 45 €. | Avoir restant 15 € ; mention sur le paiement d’origine. | ✅ |
| P-29 | Avoir 15 € ; Encaisser une séance due 45 €, répondre Oui. | 15 € imputés ; saisie ouverte sur 30 €. | Avoir restant 0. | ✅ |
| P-30 | Répondre Non à la proposition d’avoir. | Saisie normale. | Avoir inchangé. | ✅ |
| P-31 | Créer une séance / la passer Réalisée / Nouveau paiement sans séance sélectionnée, avec avoir existant. *(Bouton « Nouveau paiement » retiré le 22/10/2026 : seuls les deux premiers cas restent.)* | Aucune proposition d’avoir. | — | ✅ |
| P-32 | Annuler un paiement « Avoir ». | Refus avec message. | Rien modifié. | ✅ |
| P-33 | Mode « Avoir » désactivé, accepter la proposition. | Message d’erreur clair. | Rien enregistré. | ✅ |
| P-34 | `PaiementEditor` (UI-7) : nouveau paiement depuis une séance, puis sans séance imposée. | Fond beige et petite feuille ; cartes « Encaissement » et « Commentaire » ; séance rappelée (texte) ou à choisir (liste du kit) ; date du kit ; mode en liste du kit avec [+] (icône seule, survol visible) ; zones de texte blanches. | Montant pré-rempli avec le reste dû ; enregistrement inchangé. | ✅ |
| P-35 | Date du paiement : saisie au clavier, calendrier. | Date enregistrée telle que saisie. | — | ✅ |
| P-37 | Dialogues de paiement (UI-7) : annuler un encaissement (motif vide puis rempli), utiliser l'avoir après report (réaffectation jusqu'au 20/10/2026), ouvrir « Détail paiements ». | Fond beige et petite feuille, une carte chacun ; motif vide : champ rose pâle et message aux couleurs de la charte ; réaffectation : listes du kit, « Reporter » applique le report ; détail : carte titrée par la séance (code, date, statut), grille et fenêtre redimensionnables. | — | ✅ |
| P-36 | Paiement depuis un dossier clôturé ou archivé (si l'écran le permet). | Tout lisible en consultation (fond teinté, texte normal) ; Enregistrer masqué. | `AppliquerEtatSaisie`. | ✅ |

## Cas de test — Relevé chronologique (17/10/2026)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-38 | Ouvrir l'onglet sur un dossier avec encaissements, attentes, avoir et annulation. | Un paiement par ligne, du plus récent au plus ancien ; « — » pour un paiement attendu ; Mouvement, Pour (code · jour date · nature), Mode, Montant, pastille État. | Aucune ligne d'en-tête de séance. | ✅ |
| P-39 | Lire les pastilles État. | Soldée / Payée d'avance (vert), Reste · X € / À régler (rose), En attente / Prévue · reste X € (bleu) — libellés du 22/10/2026, Avoir · X € (mauve), Annulé / Avoir utilisé (gris atténué). | Mêmes couleurs que la pastille Paiement de l'onglet Séances. | ✅ |
| P-40 | Encaissement annulé. | Ligne grise, montant barré ; motif en gris à droite. | Infobulle : mention d'annulation complète. | ✅ |
| P-54 | Encaisser une séance (totalité, puis en deux fois). | Aucune ligne « annulée » n'apparaît dans le relevé ; dans **Détail paiements**, le dû en attente clos porte le statut « Clos » (« Paiement attendu clos — séance réglée »). | Résumé inchangé. | ✅ |
| P-55 | Fenêtre Paiement : montant 0, puis supérieur au reste dû, puis mode vide. | Phrase rose sous les champs, montant teinté en rose, curseur sur le champ ; le message disparaît dès la correction. | Aussi dans la barre d'état. | ✅ |
| P-56 | Ouvrir chaque fenêtre sur une séance, un paiement ou le dossier. | Grille Séances : colonne Code ; en-têtes « Modifier la séance SE… », « Nouvelle date pour la séance SE… », « Encaissement de la séance SE… », « Annulez l'encaissement PAI… », « Avoir de la séance SE… » (21/10/2026 ; « Encaissement de la séance SE… à reporter » avant), « Détail des paiements de la séance SE… », « Changer le statut du dossier DO… ». | Rappels et listes (séances à encaisser, à régler avec l'avoir) commencent par le code. | ✅ |
| P-57 | Bandeau d'un dossier dont le patient a un autre dossier avec dû et avoir. | Montants dus = « À régler » de l'onglet Séances, suivi de « · patient : … » si le total diffère ; Avoir du patient avec « (autre dossier) » ou « dont … ici » ; détail par dossier en infobulle. | Une séance passée restée « Planifiée » compte dans les montants dus. | ✅ |
| P-58 | Grille Patients, colonne Paiements. | Montant dû = somme des « À régler » de tous les dossiers ; infobulle « tous dossiers », avoir rappelé même s'il y a un dû. | Séance du jour comptée seulement après son heure de fin. | ✅ |
| P-59 | Annuler un règlement par avoir (bouton « Annuler l'encaissement »). | Message de refus immédiat, sans fenêtre de motif ; refus maintenu même si le mode « Avoir » est désactivé. | `GestionPaiements.EstReglementParAvoir`. | ✅ |
| P-41 | Résumé : comparer aux autres totaux, puis cliquer chaque pastille. | Avoir disponible = avoir du bandeau ; Reste à régler = « À régler » de l'onglet Séances ; clic = filtre correspondant (pastille cerclée), nouveau clic = Tous. | Encaissé n'inclut pas les annulés. | ✅ |
| P-42 | Trier par un en-tête, puis changer de filtre. | Tri ascendant / descendant avec glyphe ; le filtre rétablit l'ordre de l'écran. | — | ✅ |
| P-43 | Double-clic, Annuler l'encaissement (SuperUser). | Comportements inchangés. | Sélection = vrai paiement. | ✅ |
| P-44 | Infobulle d'une ligne. | Code du paiement, phrase complète du mouvement, référence, mention d'annulation. | — | ✅ |
| P-45 | Séance encaissée, reportée, « Garder l'avoir ». | Pastille « Avoir · 60 € » ; ligne visible dans le filtre Avoirs ; « Avoir · 60 € » dans l'onglet Séances. | Même montant dans le bandeau et dans la grille Patients ; avoir proposé à l'encaissement d'une autre séance. | ✅ |
| P-46 | Filtre « À régler ». | Paiements d'une séance due, passée, pas entièrement réglée (attendu en retard ou reste dû). | Le jour de la séance, l'heure de fin décide comme dans l'onglet Séances. | ✅ |
| P-47 | Recherche « Paiement partiel », un code de paiement, une référence. | Lignes correspondantes, même pour les colonnes retirées (code, référence). | — | ✅ |
| ~~P-48~~ | ~~Reporter une séance payée 60 € et valider la réaffectation vers une séance qui doit 45 €, une autre séance due de 15 € existant.~~ | Remplacé le 21/10/2026 (réaffectation supprimée, remplacée par l'avoir — décision de Joëlle) : voir P-65 à P-70. | — | — |
| ~~P-49~~ | ~~Même report sans autre séance due (reliquat 15 €).~~ | Remplacé le 21/10/2026 (réaffectation supprimée, remplacée par l'avoir — décision de Joëlle) : voir P-65 à P-70. | — | — |
| ~~P-50~~ | ~~Passer une séance encaissée au statut Annulée **dans la fenêtre Séance** et valider la réaffectation.~~ | Remplacé le 21/10/2026 (réaffectation supprimée, remplacée par l'avoir — décision de Joëlle) : voir P-65 à P-70. | — | — |
| P-51 | Avoir 60 € ; encaisser une séance de 45 € avec l'avoir, puis une autre avec le reste. | Ligne source « Avoir · 15 € », puis « Avoir utilisé → S… » grisée (montant d'origine) ; lignes « Avoir » soldant les séances. | « Encaissé » inchangé par ces deux utilisations ; avoir du bandeau 15 € puis 0. | ✅ |
| P-52 | Bouton du relevé sur un encaissement. | Libellé « Annuler l'encaissement ». | — | ✅ |
| P-53 | Séance réalisée encaissée 60 € ; SuperUser : cadenas, montant 50 €, confirmer. | Séances : « Réglée · avoir 10 € », solde « +10,00 » ; Paiements : « Soldée » + « avoir 10 € », ligne dans le filtre Avoirs ; avoir 10 € au bandeau et dans la grille Patients. | Encaisser une autre séance due : l'avoir de 10 € est proposé ; après utilisation, la séance d'origine reste « Réglée », sans dû. | ✅ |
| P-54 | « Détail des paiements » d'une séance avec un paiement annulé et un encaissement. | Colonne « État » en pastille, mêmes couleurs et précisions que le relevé du dossier ; ligne annulée en gris, montant barré. | Date centrée, montants à droite. | ☐ |
| P-55 | Même dialogue, séance avec un paiement en attente et un avoir. | Pastilles « Prévue » (bleu) et avoir (mauve), comme dans le relevé du dossier. | | ☐ |

## Cas de test — Intégrité des avoirs et du dû (audit B.1, 21/10/2026)

À faire sur un **patient de test**, dans un dossier **D3** créé pour l'occasion (séances à 60 €, toutes Planifiées, dates croissantes S1 < S2 < … ; aucune autre séance due dans D3, sinon l'excédent d'un report y part aussi).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-60 | S1 encaissée 60 € ; SuperUser : cadenas, montant de S1 à 40 € (avoir 20 €) ; encaisser S2 et accepter l'avoir (20 €). Puis, sur l'encaissement de 60 € de S1 : **Annuler l'encaissement**. | Refus **avant** la fenêtre du motif : « Paiement PAI… : 20,00 € de cet encaissement ont déjà servi d'avoir pour régler une autre séance : il ne peut plus être annulé… ». | Rien n'a changé : S1 « Réglée », S2 réglée 20 € par avoir, reste 40 €. Journal : aucune erreur. | ☐ |
| ~~P-61~~ | ~~Suite de P-60 : S1 → statut **Annulée** ; dans la fenêtre de réaffectation, choisir S3 (rien d'encaissé).~~ | Remplacé le 21/10/2026 (réaffectation supprimée, remplacée par l'avoir — décision de Joëlle) : voir P-65 à P-70. | — | — |
| ~~P-62~~ | ~~S4 encaissée 60 € ; S5 encaissée 20 € ; S6 et S7 rien. S4 → **Annulée**, réaffectation vers S5. Puis S5 → **Annulée**, réaffectation de l'encaissement venu de S4 vers S7.~~ | Remplacé le 21/10/2026 (réaffectation supprimée, remplacée par l'avoir — décision de Joëlle) : voir P-65 à P-70. | — | — |
| P-63 | **Doublons existants** (base de développement) : exécuter la requête de contrôle ci-dessous. Puis, sur une séance de test jamais encaissée : Annulée → Planifiée → Maladie → Planifiée. | La séance de test a **une seule** ligne En attente de 60 € (Détail paiements). | Si la requête listait des séances : ouvrir l'une d'elles et l'enregistrer (ou changer son statut) suffit à annuler ses doublons ; elle disparaît de la requête. Journal : « Dû en attente réaligné à … €, N ligne(s) de dû en double annulée(s). » | ☐ |
| P-64 | Non-régression : SuperUser, annuler un encaissement simple (séance encaissée 60 €, aucun avoir utilisé), avec motif. | Encaissement annulé ; la séance redevient due de 60 € avec **une** ligne En attente (ancien dû réactivé). | Pas de seconde ligne En attente ; mention d'annulation dans l'infobulle. | ☐ |

Requête de contrôle (identifiants seulement) :

```sql
SELECT lps.id_seance, COUNT(*) AS nb_lignes_en_attente
FROM lia_paiements_seances lps
INNER JOIN paiements p ON p.id_paiement = lps.id_paiement
WHERE p.statut_paiement = 'EN_ATTENTE'
GROUP BY lps.id_seance
HAVING COUNT(*) > 1;
```

> 21/10/2026 : la requête ne renvoie aucune ligne sur la base de développement (aucun doublon existant ; pas de nettoyage ponctuel nécessaire).

## Cas de test — Avoir après un report ou un statut non dû (audit B.2, 21/10/2026)

Remplace la réaffectation (décision de Joëlle). **Patient de test**, dossier **D4** créé pour l'occasion, séances Planifiées à 60 €. Comptes : SuperUser (mentions « par … »).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-65 | S1 encaissée 60 € → **Reporter** (onglet Séances). Dans la fenêtre « Utilisation de l'avoir » : **Utiliser l'avoir**. | Fenêtre : « Avoir disponible : 60,00 € (séance S1) » ; la séance de remplacement présélectionnée (« S… · date … (reste dû 60,00 €) »). Message « Avoir de 60,00 € utilisé pour la séance S… : elle est réglée. » | Paiements : le paiement d'origine reste sur S1, « Avoir utilisé → S… », mention « [Avoir 60,00 € utilisé le … par … pour S…] » ; la nouvelle séance a un paiement au mode « Avoir » de 60 €, « Soldée », aucune ligne En attente. Avoir du bandeau : 0. | ✅ |
| P-66 | S2 encaissée 60 € → Reporter → **Garder l'avoir**. Puis **Encaisser** la séance de remplacement. | Pas de message ; S2 « Reportée », « Avoir · 60 € » ; la remplaçante doit 60 €. À l'encaissement, l'avoir de 60 € est proposé (§ 5.5). | Bandeau et grille Patients : avoir 60 € tant qu'il n'est pas utilisé. | ✅ |
| P-67 | S3 encaissée 60 € ; S4 encaissée 20 € (reste 40 €). **Fenêtre Séance** : S3 → Annulée, Enregistrer ; dans la fenêtre de l'avoir, choisir **S4**. | Message « Avoir de 40,00 € utilisé pour la séance S4 : elle est réglée. 20,00 € restent en avoir sur la séance S3. » | S3 « Avoir · 20 € » ; **aucune autre séance touchée** (plus de répartition automatique). | ✅ |
| P-68 | S5 encaissée 20 € (partiel) → Annulée ; choisir une séance due de 60 € (rien d'encaissé). | « Avoir de 20,00 € utilisé pour la séance … : il lui reste 40,00 € à régler. » | La séance choisie : une seule ligne En attente de **40 €**. | ✅ |
| P-69 | Le patient a déjà un avoir ailleurs (S2 de P-66, 60 €). S6 encaissée 60 € → Reporter. | La fenêtre propose **60,00 € (séance S6)**, pas 120 €. | Après « Utiliser l'avoir » : l'avoir de S2 est intact (60 €). | ✅ |
| P-70 | Dossier de test avec **une seule** séance, encaissée → Annulée (fenêtre Séance). | Pas de fenêtre de choix ; message « L'encaissement de la séance … est conservé en avoir… ». | Avoir 60 € au bandeau. | ✅ |

> Référence des résultats attendus : [Guide séances et paiements](../Guide/Guide_Seances_Paiements.md), partie 2.

## Cas de test — Une seule définition du reste dû et de l'avoir (audit B.4, 21/10/2026)

**Avant tout** : appliquer la migration `Docs/Database/Migration/migration_v2.49.0_vues_soldes.sql` dans HeidiSQL, connecté en `root`, sur la base `althea`.

**Patient de test** uniquement, séances à 60 €.

- Domaine de test **D-NF** : étape « Général », case « Absence non excusée facturable » **décochée**
- domaine de test **D-F** : case **cochée**.

Les requêtes de contrôle ne renvoient que des identifiants et des montants.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-71 | Migration appliquée, lancer l'application. | Démarrage normal, aucun message sur la version de la base. | `SELECT version_schema FROM tec_meta_schema WHERE version_schema = '2.49.0';` → 1 ligne. `SHOW FULL TABLES WHERE Table_type = 'VIEW';` → `v_dossier_dominant` et `v_seances_solde`. | ✅ |
| P-72 | Dossier du domaine **D-NF** : séance planifiée hier, rien d'encaissé (« À régler 60 € »). Fenêtre Séance : statut **Absence (no show)**, Enregistrer. | Grille Séances : « Non facturée (convention) », plus de « À régler ». Onglet Paiements : la ligne « En attente » de 60 € passe à « Clos » (grisée). | « Nouveau paiement » (sans séance sélectionnée) : la séance **n'est pas proposée**. Bandeau du dossier, grille Patients et aperçu : pas de 60 € à régler pour elle. Contrôle : `SELECT reste_du, est_facturable FROM v_seances_solde WHERE id_seance = <id>;` → 0 et 0. | ✅ |
| P-73 | Même scénario dans le domaine **D-F** (non-régression). | « À régler 60 € » ; une seule ligne « En attente » de 60 €. | La séance **est proposée** dans « Nouveau paiement », montant 60 €. | ✅ |
| P-74 | Séance planifiée encaissée **20 €** (reste 40 €) → statut **Annulée**, « Garder l'avoir ». | Grille Séances : « Avoir · 20 € » ; onglet Paiements : la ligne de 20 € en « Avoir · 20 € », mention « conservé en avoir », aucun « reste … dû ». | Bandeau : avoir 20 €, rien à régler pour cette séance. | ✅ |
| P-75 | Patient de test avec plusieurs séances (une soldée, une partielle passée, une à venir non réglée, une annulée encaissée) : comparer les montants. | Bandeau du dossier, colonne « Paiements » de la grille Patients et section Finances de l'aperçu donnent les **mêmes** « À régler » et « Avoir ». | Contrôle : `SELECT id_seance, tarif_seance, montant_regle, reste_du, montant_avoir, est_echue FROM v_seances_solde WHERE id_dossier = <id du dossier de test>;` → somme des `reste_du` des lignes `est_echue = 1` = « À régler » ; somme des `montant_avoir` = « Avoir ». | ✅ |

> Une ligne « En attente » créée **avant** la migration pour une absence non facturable reste active jusqu'à la prochaine resynchronisation de sa séance (l'ouvrir et l'enregistrer).
>
> Requête de contrôle (identifiants seulement) : `SELECT l.id_seance, p.id_paiement FROM paiements p JOIN lia_paiements_seances l ON l.id_paiement = p.id_paiement JOIN v_seances_solde v ON v.id_seance = l.id_seance WHERE p.statut_paiement = 'EN_ATTENTE' AND v.est_du = 1 AND v.est_facturable = 0;`

## Cas de test — Mode de paiement relié au référentiel (audit B.5, étape 1, 21/10/2026)

**Avant tout** : appliquer `Docs/Database/Migration/migration_v2.50.0_paiements_mode_id.sql` dans HeidiSQL, en `root`. **Patient de test** uniquement. Remettre chaque libellé modifié à la fin du cas.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-76 | Migration appliquée, lancer l'application ; ouvrir l'onglet Paiements d'un dossier qui a des encaissements anciens. | Démarrage normal ; colonne « Mode » remplie comme avant (Espèces, Virement, Avoir…). | `SELECT COUNT(*) FROM paiements WHERE TRIM(COALESCE(mode_paiement, '')) <> '' AND id_mode_paiement IS NULL;` → **0**. Modes repris : `SELECT code_mode_paiement, libelle_mode_paiement, actif FROM ref_modes_paiement WHERE ordre_affichage = 900;` (souvent aucun ; sinon inactifs). | ✅ |
| P-77 | Encaisser une séance de test en **Virement**. | Ligne « Virement » dans le relevé. | `SELECT id_mode_paiement, mode_paiement FROM paiements WHERE id_paiement = <id>;` → identifiant renseigné, `mode_paiement` vide. | ✅ |
| P-78 | Référentiels › Modes de paiement : renommer « Virement » en « Virement bancaire » ; revenir au relevé. | Les anciens virements affichent **« Virement bancaire »** (le lien suit le mode). | Remettre « Virement ». | ✅ |
| P-79 | Renommer le mode **« Avoir »** (ex. « Avoir patient ») ; sur un règlement par avoir existant, **Annuler l'encaissement**. | Refus « … règle la séance … avec un avoir : il ne peut pas être annulé… » (avant la correction, le renommage le rendait annulable). | Remettre « Avoir ». | ✅ |
| P-80 | Désactiver un mode utilisé (ex. « Chèque ») ; ouvrir « Encaisser séance » (« Nouveau paiement » avant le 22/10/2026) ; puis le réactiver. | Inactif : absent de la liste de saisie ; les anciens paiements l'affichent toujours. | Utiliser un avoir (comme P-65) : le nouveau paiement est au mode « Avoir ». | ✅ |

## Cas de test — Annulation enregistrée dans des colonnes (audit B.5, étape 2, 22/10/2026)

**Avant tout** : appliquer `Docs/Database/Migration/migration_v2.51.0_paiements_annulation.sql` dans HeidiSQL, en `root`. **Patient de test**, compte SuperUser ou Admin.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-81 | Encaisser 60 € sur une séance de test passée, puis **Annuler l'encaissement** avec le motif « test colonnes ». | Ligne grisée « Annulé » ; à droite de la pastille, « test colonnes » ; infobulle « Annulé le jj/mm/aaaa par <ton nom> : test colonnes » ; la séance redoit 60 € (une ligne En attente). | `SELECT statut_paiement, date_annulation, id_utilisateur_annulation, motif_annulation, commentaire FROM paiements WHERE id_paiement = <id>;` → ANNULE, date et heure, ton identifiant, le motif ; **commentaire inchangé** (pas de mention ajoutée). | ✅ |
| P-82 | Journal du jour. | « Paiement encaissé annulé (ID=…, utilisateur ID=…). » | **Aucun nom** d'utilisateur dans la ligne. | ✅ |
| P-83 | Paiement annulé **avant** la migration (s'il y en a un). | Affichage inchangé : motif à droite de la pastille, phrase complète dans l'infobulle. | Ses colonnes d'annulation sont vides ; la mention reste dans le commentaire. | ✅ |
| P-84 | Annuler un encaissement : coller un motif très long (plus de 500 caractères). | La saisie s'arrête à 500 caractères ; l'annulation se fait. | `CHAR_LENGTH(motif_annulation)` = 500 au plus. | ✅ |
| P-85 | Non-régression des refus : un règlement par avoir ; un encaissement dont une partie a servi d'avoir. | Refus annoncés comme avant, avant la saisie du motif ; rien n'est modifié. | Aucune ligne « annulé » au journal. | ✅ |

## Cas de test — Utilisation d'un avoir reliée à son encaissement (audit B.5, étape 3, 22/10/2026)

**Avant tout** : appliquer `Docs/Database/Migration/migration_v2.52.0_lia_paiements_avoirs.sql` dans HeidiSQL, en `root`. **Patient de test**, séances à 60 €.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-86 | S1 encaissée 60 € → Annulée → « Utiliser l'avoir » pour S2 (due, rien d'encaissé). | Comme P-67 : S2 réglée ; S1 « Avoir utilisé → S2 ». Infobulle de S1 : « Avoir 60,00 € utilisé le … par <ton nom> pour S2 ». Infobulle du règlement de S2 : « Avoir utilisé : PAI… (S1) 60,00 € ». | `SELECT la.* FROM lia_paiements_avoirs la WHERE la.id_paiement_avoir = <id du règlement>;` → 1 ligne, 60,00, ton identifiant. Commentaires de l'encaissement de S1 et du règlement : **vides de toute mention**. | ✅ |
| P-87 | S3 encaissée 20 € → Annulée → « Garder l'avoir » ; S4 encaissée 30 € → Annulée → « Garder l'avoir » ; puis **Encaisser** S5 (60 €) → « Oui » à l'avoir. | 50 € d'avoir utilisés (S3 puis S4) ; la saisie s'ouvre sur 10 €. | `lia_paiements_avoirs` : **2 lignes** pour le même règlement (20,00 et 30,00). Infobulle du règlement : « Avoir utilisé : PAI… (S3) 20,00 €, PAI… (S4) 30,00 € ». | ✅ |
| P-88 | Trop-perçu : S6 encaissée 60 €, montant baissé à 40 € (cadenas) → avoir 20 € ; **Encaisser** une autre séance due → « Oui » à l'avoir. | 20 € utilisés ; S6 « Soldée », plus de mention « avoir 20 € » ; infobulle de l'encaissement de S6 : « Avoir 20,00 € utilisé le … par … pour S… ». | Une ligne dans `lia_paiements_avoirs`, 20,00. | ✅ |
| P-89 | Utilisations faites **avant** la migration (P-65 à P-70). | Affichage inchangé (« Avoir utilisé → S… », mentions dans l'infobulle) ; l'infobulle du règlement montre désormais ses origines (« Avoir utilisé : … », relue dans son commentaire). | Rien dans `lia_paiements_avoirs` pour elles. | ✅ |
| P-90 | Journal du jour ; puis tenter d'annuler un règlement par avoir de P-86. | « Avoir de 60,00 € imputé sur la séance ID=… (paiement ID=…, utilisateur ID=…). » ; **aucun nom**. Annulation refusée comme avant. | — | ✅ |

## Cas de test — Encaissement jugé par le métier (audit C.2, étape 1, 22/10/2026)

**Patient de test**, dossier de test actif, séances à 60 €. Pas de migration.

La fenêtre Paiement appelle désormais la règle du métier (`ValiderEncaissement`) : ses messages sont les mêmes qu'avant.

Le refus de `CreatePaiement` lui-même (encaissement sur une séance non due, soldée, ou au-delà du reste dû) ne se provoque pas depuis l'écran, qui l'arrête avant : il sera couvert par les tests automatisés (lot D).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| P-91 | **Encaisser** une séance due de 60 € (rien d'encaissé) : montant **0** → Enregistrer ; puis **70** → Enregistrer ; puis **vider la date** → Enregistrer. | 0 : « Le montant doit être supérieur à zéro. », montant teinté. 70 : « Le montant ne peut pas dépasser le reste dû de la séance (60,00 €). » Date vide : « Indiquez la date du paiement. » La fenêtre reste ouverte, rien n'est enregistré. | Date vide : **nouveau** (avant, une erreur technique) ; le reste comme avant. | ✅ |
| P-92 | Même séance : encaisser **20 €** (date du jour, mode Espèces). | Accepté. Séance : réglé 20 €, reste dû 40 €. | **Détail paiements** : une seule ligne En attente, de **40 €** (le dû est réaligné dans la même opération que l'encaissement). | ✅ |
| P-93 | Onglet Paiements, **aucune séance sélectionnée** : Nouveau paiement → Enregistrer sans choisir de séance. Puis choisir la séance de P-92. | « Choisissez la séance à laquelle ce paiement se rapporte. » Puis le montant proposé est **40 €**. | **Non conforme, comportement antérieur à C.2** (relevé de Joëlle) : aucune séance n'est jamais « non sélectionnée » — la grille Séances sélectionne d'office sa première ligne, et « Nouveau paiement » de l'onglet Paiements encaisse la séance sélectionnée **dans l'onglet Séances**, sans autre choix. Décision de Joëlle : bouton retiré, aucune ligne sélectionnée d'office (finitions, commit suivant). | ➖ |
| P-94 | Onglet Paiements : **double-clic** sur la ligne d'un encaissement d'une séance **soldée** ; puis sur une ligne d'une séance **annulée** (avoir). | « La séance S… est déjà soldée : aucun encaissement supplémentaire n'est possible. » ; « La séance S… n'est pas due (Annulée …) : elle ne peut pas être encaissée. » (fenêtre « Encaissement impossible »). | Comme avant (messages venus du métier). | ✅ |
| P-95 | Non-régression : encaisser avec **utilisation d'un avoir** proposée (« Oui »), puis annuler un encaissement (SuperUser). | Comme avant (P-86, P-81). | Journal du jour : aucune nouvelle ligne d'erreur ; requête de contrôle de P-63 : aucune séance avec deux lignes En attente. | ✅ |

## Parcours recommandé (17/10/2026)

Les cas s'enchaînent sur un **patient de test** (jamais un vrai patient) qui a **deux dossiers** (D1, D2) d'un même domaine, au tarif de 60 € (45 € pour les séances de D2 créées en étape 4, au cadenas). Comptes : un SuperUser, un utilisateur simple. Cocher chaque cas dans les tableaux au fil de l'eau ; noter tout écart (capture + numéro du cas).

| Étape | Mise en place | Cas |
|---|---|---|
| **0. Vérifications** | Référentiel *Modes de paiement* : mode « Avoir » actif ; base : ligne 2.20.0 dans `tec_meta_schema`. | P-27 |
| **1. Six séances dans D1** | S1 il y a 7 jours (Réalisée), S2 il y a 14 jours, S5 il y a 21 jours ; S3 dans 7 jours, S4 dans 14 jours, S6 dans 21 jours. | P-01, P-06, P-38, P-46 (3 lignes « À régler », 3 « En attente ») |
| **2. Encaisser** | S1 : 60 € ; S2 : 20 € puis 40 € ; montants invalides ; S1 à nouveau. | P-02 à P-05, P-09, P-10, P-19, P-26, P-08, P-47 |
| **3. Annuler un encaissement** | Utilisateur simple, puis SuperUser : S5 encaissée 20 € → annuler ; puis 60 € → annuler avec motif, sans motif d'abord. | P-16, P-17, P-20, P-18, P-07, P-40, P-52 |
| **4. Avoirs** | S4 encaissée 60 € (d'avance) → statut Annulée, « Garder l'avoir » ; dans D2, deux séances de 45 € : encaisser la 1re (Oui), la 2e (Oui), puis une 3e (Non) ; annuler le paiement « Avoir » ; désactiver le mode Avoir le temps de P-33 (**le réactiver ensuite**). | P-21, P-24, P-28 à P-33, P-51 |
| **5. Reports** | Depuis le 21/10/2026 : parcours P-65 à P-70 (avoir après report ou statut non dû) ; S6 encaissée 60 € → Reporter, « Garder l'avoir ». | P-45, P-65 à P-70 |
| **6. Lecture** | Filtres, résumé, tri, infobulles, Détail paiements, période, recherche. | P-13, P-13a, P-25, P-39, P-41 à P-44 |
| **7. Droits et erreurs** | Dossier clôturé / archivé (règles de clôture : plan Dossiers D-19 à D-23) ; utilisateur simple ; enregistrement pendant un arrêt du service MariaDB (poste de développement uniquement). | P-14, P-36, P-15 |

## Règles de contrôle

- Le reste dû est calculé : tarif figé de la séance − somme des seuls paiements encaissés.
- Le statut d’un paiement ne doit jamais être confondu avec le solde de sa séance.
- En V1, un paiement est lié à une seule séance ; une séance peut recevoir plusieurs paiements.
- Une séance due non soldée porte exactement une ligne En attente égale à son reste dû.
- Aucun paiement n’est supprimé ; un encaissement s’annule avec motif.

## Validation finale

- [ ] P-60 à P-64 (audit B.1 : avoirs et dû).
- [x] P-65 à P-70 (audit B.2 : avoir après report ou statut non dû) — validés par Joëlle le 21/10/2026.
- [x] P-71 à P-75 (audit B.4 : une seule définition du reste dû et de l'avoir) — validés par Joëlle le 21/10/2026.
- [x] P-76 à P-80 (audit B.5, étape 1 : mode de paiement relié au référentiel) — validés par Joëlle le 21/10/2026.
- [x] P-81 à P-85 (audit B.5, étape 2 : annulation enregistrée dans des colonnes) — validés par Joëlle le 22/10/2026.
- [x] P-86 à P-90 (audit B.5, étape 3 : utilisation d'un avoir reliée à son encaissement) — validés par Joëlle le 22/10/2026.
- [x] P-91, P-92, P-94, P-95 (audit C.2, étape 1 : encaissement jugé par le métier) — validés par Joëlle le 22/10/2026 ; P-93 sans objet (voir la ligne).
- [ ] Tous les cas P-01 à P-59 sont exécutés — **tests approfondis à faire** maintenant que la grille est revue (17/10/2026 ; ToDo C4, décision de Joëlle du 15/10/2026).
- [ ] Aucun surpaiement, paiement orphelin ou calcul de solde erroné.
- [ ] Les pastilles « État » (en attente, à régler, reste dû, soldée, avoir, annulé) suivent la priorité définie (ADR-25, évolution du 17/10/2026).
