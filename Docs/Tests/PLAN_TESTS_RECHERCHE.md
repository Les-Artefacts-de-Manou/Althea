# Plan de tests — Recherche transversale

> Version : 1.6 — Dernière mise à jour : 24/10/2026 — pastille « Inactif » dans les recherches : RCH-72 à RCH-73
> Périmètre : `UC_RechercheHome`, `UC_RechercheSeances`, `UC_RecherchePaiements`, `UC_RecherchePatientsLieu`, `LieuPatients`, `UC_RechercheReseau`, `PersonneReseau`, `SelecteurPeriode`, `OuvertureResultat`, `GestionRecherche`, `QueryRecherche`, `FiltresSeances`, `FiltresPaiements` ; ouverture ciblée de `UC_DossierFiche` et `UC_PatientFiche`. Référence : ADR-52, processus 24.

<details>
<summary>Historique des mises à jour</summary>

- 24/10/2026 — R5 : réseau par nom, RCH-51 à RCH-66
- 24/10/2026 — R4 : patients par lieu, RCH-40 à RCH-50
- 23/10/2026 — R3 : paiements d'un jour ou d'une période, RCH-28 à RCH-39
- 23/10/2026 — R2 : séances d'un jour ou d'une période, RCH-14 à RCH-27
- 23/10/2026 — R1 : bouton « Recherche », recherche rapide par code, ouverture ciblée, code des paiements : RCH-01 à RCH-13

</details>

## Objectif

Vérifier

- qu'un élément se retrouve par son code sans passer par son patient, que la recherche dit ce que c'est et de quel patient, et qu'un geste y mène (au bon onglet, la ligne sélectionnée), Retour ramenant à la recherche
- que les séances d'un jour ou d'une période se listent, tous patients confondus, avec les mêmes statuts, paiements et filtres que dans le dossier (R2)
- que les paiements d'une période forment le même relevé que le dossier, avec les totaux encaissés par mode (R3)
- que les patients se retrouvent par leur code postal ou leur localité, lieu par lieu (R4)
- qu'un thérapeute, un intervenant ou un contact se retrouve par son nom, avec les patients qui en relèvent et l'accès à sa fiche (R5).

## Prérequis et données de test

- Application compilée, base accessible ; patients, dossiers, séances et paiements **de test** (par exemple le patient PA000005 et la séance SE000083 du 22/10/2026).
- Relever avant les tests quelques codes réels : un patient (PA…), un dossier (DO…), une séance ancienne (SE…, sur une page autre que la première de son dossier), un paiement (PAI…), un contact (FC…) et un intervenant (AS…) — par exemple dans les grilles des fiches.

## Cas de test — R1 : recherche rapide par code (23/10/2026)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RCH-01 | Menu de gauche, compte **User** puis **Admin**. | Bouton **Recherche** sous Patients (icône loupe), les suivants décalés d'un cran ; Outils/Admin en bas, rien ne se chevauche. Clic : le hub s'ouvre, le bouton est sélectionné. | Survol et état sélectionné de l'icône. | ✅ |
| RCH-02 | Hub Recherche. | En-tête « Recherche » ; carte « Recherche rapide par code » avec l'aide (« PA patient, DO dossier, SE séance, PAI paiement, FC contact, AS intervenant ») ; carte « Autres recherches » (« À venir… »). Le curseur est dans la zone de code. | Ouvrir / Ouvrir le patient grisés tant qu'il n'y a pas de résultat. | ✅ |
| RCH-03 | Code d'un patient (ex. `pa5`, puis `PA000005`) ; Entrée ; puis Entrée à nouveau. | Une ligne « Patient », description « né(e) le … · localité » ; Entrée sur la ligne ouvre la **fiche** (onglet Identité). Retour : le hub revient avec le code et la ligne. | — | ✅ |
| RCH-04 | Code d'un dossier ; double-clic sur la ligne. | « Dossier · domaine · statut · ouvert le … », patient ; le **hub du dossier** s'ouvre (fiche du dossier). Retour : le hub Recherche. | — | ✅ |
| RCH-05 | Code d'une séance **ancienne** (ex. SE000083) ; Entrée, Entrée. | « le JJ/MM/AAAA à HH:MM · statut · nature · domaine · tarif · DO… » ; le dossier s'ouvre sur l'onglet **Séances**, la séance **sélectionnée** sur sa page (pas la page d'aujourd'hui). | Les boutons de l'onglet (Encaisser, Modifier…) agissent sur elle. | ✅ |
| RCH-06 | Code d'un paiement. | « montant · statut (Encaissé / En attente / Annulé / Clos) · date · mode · séance · DO… » ; le dossier s'ouvre sur l'onglet **Paiements**, le paiement sélectionné. | — | ✅ |
| RCH-07 | Code d'un contact (FC…), puis d'un intervenant (AS…). | Contact : « nom prénom · lien » → fiche, onglet **Famille / Contacts** ; intervenant : « nom · spécialité · rôle » → fiche, onglet **Intervenants**. | — | ✅ |
| RCH-08 | Résultat d'une séance : **Ouvrir le patient**. | La fiche du patient (onglet Identité) ; Retour : le hub Recherche. | — | ✅ |
| RCH-09 | Saisies : `se 000083`, `Se83` ; `XY12` ; `SE` ; `SE999999` ; zone vide. | Les deux premières : même résultat ; `XY12` et `SE` : « n'est pas un code reconnu (AS, DO, FC, PA, SE, PAI suivi de chiffres) » ; `SE999999` : « Aucun élément ne porte ce code. » ; vide : « Tapez un code. » | Aucune erreur technique. | ✅ |
| RCH-10 | Séance d'un dossier **clôturé** ou **archivé**. | Le dossier s'ouvre en lecture seule, comme depuis la fiche patient ; la séance est sélectionnée. | — | ✅ |
| RCH-11 | Après quelques recherches : Outils/Admin › Journaux. | Lignes « Recherche par code (Seance) : 1 résultat(s). » — **jamais** le code tapé ni un nom. | — | ✅ |
| RCH-12 | Non-régression : Patients › fiche › onglet Dossiers › un dossier › Retour ; fiche › Contacts / Intervenants. | Retour sur la fiche, onglet **Dossiers** (comme avant) ; Retour à la liste des patients avec ses filtres. | Rien ne change pour le parcours Patients. | ✅ |
| RCH-13 | (Relevé de Joëlle) Dossier de test › onglet Paiements ; Séances › « Détail paiements » ; puis encaisser une séance. | Colonne **Code** (PAI…) dans les deux grilles, encaissements prévus compris ; après l'encaissement, la barre d'état dit « Paiement PAI… enregistré. » ; ce code retrouve le paiement par la recherche. | La recherche dans l'onglet Paiements trouve aussi par code (comme avant). | ✅ |

## Cas de test — R2 : séances d'un jour ou d'une période (23/10/2026)

Prérequis : des séances de test sur plusieurs jours (aujourd'hui, la semaine, le mois, le mois précédent), de statuts variés (à venir, planifiée passée « à confirmer », réalisée, absence, annulée, reportée) et de paiements variés (réglée, à régler, prévue, avec avoir), dans au moins deux domaines.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RCH-14 | Hub Recherche. | Carte « Autres recherches » : tuile **Séances — D'un jour ou d'une période** (icône calendrier + loupe, survol) ; « À venir : les paiements… » dessous. Clic : écran **Séances**, contexte « Recherche > Séances », période = **aujourd'hui**, séances du jour dans l'ordre des heures. | — | ✅ |
| RCH-15 | Boutons **Cette semaine**, **Ce mois**, **Aujourd'hui**. | Semaine : lundi → dimanche de la semaine en cours ; mois : 1er → dernier jour ; aujourd'hui : du = au = aujourd'hui. Liste par date puis heure, relancée à chaque fois. | — | ✅ |
| RCH-16 | Boutons **‹** et **›** sur un jour, sur une semaine, sur le mois en cours, puis sur une période libre (ex. 10 jours). | Jour → jour voisin ; semaine → semaine voisine ; mois entier → **mois voisin entier** (octobre → 1er–30 septembre) ; période libre → glisse de sa durée. | Infobulles des boutons. | ✅ |
| RCH-17 | Dates tapées : « Du » après « Au », puis « Au » avant « Du ». | L'autre borne suit la date choisie : jamais de période inversée ; la liste est relancée. | — | ✅ |
| RCH-18 | Colonnes de la liste ; comparer une séance avec l'onglet Séances de son dossier. | Date avec le jour (« ven. 23/10/2026 »), Horaires, Patient (« Nom Prénom »), **Statut** et **Paiement** en pastilles (mêmes textes et couleurs que dans le dossier : « À confirmer », « À régler · 60 € »…), Nature, Domaine, Séance, Dossier ; séances non dues atténuées. | Tri par clic sur un en-tête (Patient, Date…). | ✅ |
| RCH-19 | **Résumé** au-dessus de la grille (période d'un mois). | Pastilles « n à venir · n à confirmer · n réalisées · n absences · n non dues | À régler … € · Prévu … € · Avoir … € », calculées sur toute la période ; clic sur « À régler » → filtre « Exigibles (impayées) », titre « Séances (n sur m) » ; nouveau clic → toutes. | — | ✅ |
| RCH-20 | Liste **Filtre**. | Mêmes axes et couleurs que l'onglet Séances du dossier (Toutes, Aujourd'hui, À venir, À confirmer, Passées, Réalisées, Absences sans prévenir, Reportées, Non dues, Exigibles, Réglées, Avec avoir) ; « Exigibles » sur le mois = les impayés du mois, tous patients. | — | ✅ |
| RCH-21 | Zone de texte : début de nom d'un patient, puis un code SE… / DO…, une nature, un domaine. | La liste se réduit à chaque frappe, combinée avec le filtre ; effacer le texte rend la liste. | — | ✅ |
| RCH-22 | Double-clic (puis Entrée, puis « Ouvrir ») sur une séance ; **Retour** depuis le dossier. | Le dossier s'ouvre sur l'onglet **Séances**, la séance **sélectionnée** sur sa page ; contexte « Recherche > Séances > Nom Prénom > DO… ». Retour : l'écran Séances revient avec **la même période, le filtre, le texte**, la séance resélectionnée et visible. | — | ✅ |
| RCH-23 | « Ouvrir le patient » ; Retour. Puis « Retour à la recherche ». | La fiche du patient (Identité) ; Retour : l'écran Séances. « Retour à la recherche » : le hub, avec le code qu'il avait éventuellement. | — | ✅ |
| RCH-24 | Un jour sans séance ; une période sans séance. | « Aucune séance ce jour-là. » / « Aucune séance sur cette période. » ; Ouvrir et Ouvrir le patient grisés. | Aucune erreur technique. | ✅ |
| RCH-25 | Période très longue (ex. 01/01/2000 → 31/12/2030). | Toutes les séances listées ; s'il y en a plus de 1000 : les 1000 premières et « Plus de 1000 séances : seules les premières sont listées, réduisez la période. » | Selon le volume de la base de test. | ✅ |
| RCH-26 | Outils/Admin › Journaux, après quelques recherches. | « Recherche des séances d'une période : n résultat(s). » — **jamais** les dates ni un nom. | — | ✅ |
| RCH-27 | Non-régression du dossier : onglet Séances (liste Filtre, résumé et clic sur ses pastilles, case « Du … au … » avec bornes inversées) ; onglet Paiements (période) ; fenêtre « Planning du jour » ; une séance qui chevauche une autre. | Tout comme avant : mêmes axes, mêmes nombres, bornes corrigées ; le planning du jour s'affiche ; le chevauchement est refusé avec le même message. | Filtre et résumé sortis dans `FiltresSeances`, bornes dans `AltheaDate.CorrigerBornes`, requête du planning partagée. | ✅ |

## Cas de test — R3 : paiements d'un jour ou d'une période (23/10/2026)

Prérequis : des paiements de test sur plusieurs jours et plusieurs patients — encaissements de modes différents (espèces, virement…), un paiement attendu (séance à venir et séance passée), un encaissement annulé, un règlement par avoir, un paiement partiel.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RCH-28 | Hub Recherche. | Deuxième tuile **Paiements — D'un jour ou d'une période** (icône billets + loupe) ; « À venir : les patients par lieu… ». Clic : écran **Paiements**, contexte « Recherche > Paiements », période = aujourd'hui, la ligne la plus récente en haut. | — | ✅ |
| RCH-29 | Période : Aujourd'hui, Cette semaine, Ce mois, ‹ ›, dates tapées (bornes inversées). | Comme la recherche des séances (RCH-15 à RCH-17). Un geste = une recherche : une seule ligne au journal par clic. | — | ✅ |
| RCH-30 | Comparer, pour un patient de test, la liste (texte = son nom) avec l'onglet Paiements de son dossier (même période cochée). | Mêmes lignes ; mêmes Mouvement, Pour, Mode, Montant, pastille État (couleur, mention) ; montant annulé barré, ligne neutralisée en gris ; même **infobulle**. Colonnes en plus : Patient, Code (PAI…), Dossier. | — | ✅ |
| RCH-31 | Paiement attendu (non encaissé). | Listé à la **date de sa séance**, colonne Date « — » ; pastille « Prévue » ou « À régler » (séance passée), comme dans le dossier. | — | ✅ |
| RCH-32 | **Résumé** au-dessus de la grille. | « Encaissé … € · En attente … € » sur la période ; clic → filtre « Encaissés » / « En attente », titre « Paiements (n sur m) » ; nouveau clic → tous. | Pas d'« Avoir disponible » ni de « Reste à régler » ici (ce sont des soldes de dossier). | ✅ |
| RCH-33 | Liste **Filtre**. | Mêmes axes et couleurs que l'onglet Paiements du dossier : Tous, Encaissés, En attente, Annulés, Séances soldées, Séances restant dues, Avoirs, Séances reportées, À régler. | — | ✅ |
| RCH-34 | Liste **Mode** : « Tous les modes », puis un mode (ex. Espèces). | Les modes du référentiel (désactivés compris). Un mode choisi : seules ses lignes ; les paiements attendus (sans mode) disparaissent ; le résumé et les totaux suivent. | — | ✅ |
| RCH-35 | **Totaux** sous la grille, sur un mois. | « Encaissé par mode : Espèces … € · Virement … € — total … € » = encaissements non annulés de la période (et du mode choisi) ; un règlement par avoir : « · réglé par avoir … € (hors total) » ; un jour sans encaissement : « Aucun encaissement sur la période. » | Total = somme des lignes « Encaissés » (hors avoir). Infobulle des totaux. | ✅ |
| RCH-36 | Zone de texte : nom d'un patient, PAI…, SE…, un mode, une référence. | La liste se réduit à chaque frappe, combinée avec le filtre et le mode. | — | ✅ |
| RCH-37 | Double-clic (Entrée, « Ouvrir ») ; Retour ; « Ouvrir le patient » ; « Retour à la recherche ». | Le dossier s'ouvre sur l'onglet **Paiements**, le paiement sélectionné ; contexte « Recherche > Paiements > Nom Prénom > DO… ». Retour : la liste avec **sa période, son filtre, son mode, son texte**, la ligne resélectionnée. Fiche du patient ; hub. | — | ✅ |
| RCH-38 | Jour sans paiement ; période très longue ; Journaux. | « Aucun paiement ce jour-là. » / « sur cette période. » ; au-delà de 1000 lignes : « seules les plus récentes sont listées, réduisez la période » ; journal « Recherche des paiements d'une période : n résultat(s). » — jamais les dates ni un nom. | — | ✅ |
| RCH-39 | Non-régression : onglet Paiements du dossier (filtre, résumé à 4 pastilles et leurs clics, infobulle, période) ; détail des paiements d'une séance ; encaisser puis annuler un encaissement ; recherche des séances (raccourcis, ‹ ›, Retour). | Tout comme avant. | Relevé, filtre et résumé sortis dans `FiltresPaiements`, infobulle dans `PaiementListeItem.InfobulleLigne`, requête des listes de paiements commune, période des recherches dans `SelecteurPeriode`. | ✅ |

## Cas de test — R4 : patients par lieu (24/10/2026)

Prérequis : adresses de test réalistes (script `Docs/Database/Maintenance/donnees_test_codes_postaux.sql`, base de test) — plusieurs patients dans un même lieu (4317 Faimes), plusieurs localités sous un même code postal (4317 Faimes, Aineffe, Borlez, Les Waleffes ; 4000 et 4020 Liège), au moins un patient sans adresse et un patient inactif. Pour RCH-45, modifier à la main la localité d'un patient de test (« faimes » ou « FAIMES ») puis la remettre.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RCH-40 | Hub Recherche. | Troisième tuile **Patients par lieu — Code postal ou localité** (icône repère + loupe) ; « À venir : un contact, un thérapeute ou un intervenant par son nom… ». Clic : écran **Patients par lieu**, contexte « Recherche > Patients par lieu », curseur dans la zone. | Survol et état désactivé de l'icône lisibles (icône provisoire). | ✅ |
| RCH-41 | Ouverture, zone vide. | À gauche **Lieux (n)** : « Tous les lieux trouvés » en tête (sélectionné), puis chaque lieu « code postal localité » avec son nombre de patients, par code postal puis localité, « Sans adresse » en dernier ; à droite **Patients — Tous les lieux trouvés (n)**. | Colonnes : Code, Nom, Prénom, CP, Localité, Suivi, Prochain RDV, Téléphone ; barre d'état « Patients par lieu : n lieu(x), m patient(s). » | ✅ |
| RCH-42 | Clic sur un lieu (4317 Faimes), puis sur « Tous les lieux trouvés ». | Les patients de Faimes seuls, par nom puis prénom, titre « Patients — 4317 Faimes (n) » ; puis de nouveau tous. | Le nombre du lieu = le nombre de patients listés. | ✅ |
| RCH-43 | Code postal : taper « 4317 », puis « 43 », puis « 4 ». | « 4317 » : les lieux 4317 (Faimes, Aineffe, Borlez, Les Waleffes) ; « 43 » : aussi Waremme, Remicourt, Oreye, Awans… ; « 4 » : tous les codes qui commencent par 4. La liste suit chaque frappe. | « Sans adresse » n'apparaît plus dès qu'un texte est tapé. | ✅ |
| RCH-44 | Localité : « liège », « LIEGE », « lie », « waleffes », « grace ». | Liège apparaît sous **deux lieux** (4000 et 4020) ; accents et majuscules indifférents ; une partie du nom suffit (Les Waleffes, Grâce-Hollogne). | — | ✅ |
| RCH-45 | Deux écritures d'une même localité (« faimes » sur un patient, « Faimes » sur les autres). | Un seul lieu « 4317 Faimes » (l'écriture la plus fréquente), qui compte les deux patients. | Remettre la localité d'origine ensuite. | ✅ |
| RCH-46 | Code et localité ensemble : « 4317 aineffe », « 4000 liège », « 4020 aineffe ». | « 4317 aineffe » : Aineffe seul ; « 4000 liège » : 4000 Liège seul ; « 4020 aineffe » : aucun lieu, message « Aucun lieu ne correspond. » | — | ✅ |
| RCH-47 | Inactifs : un lieu qui compte un patient inactif ; cocher puis décocher « Afficher les inactifs ». | Décoché : le patient inactif n'est pas compté, barre d'état « — 1 patient(s) inactif(s) masqué(s) : cocher « Afficher les inactifs ». » ; coché : il est compté et listé en gris. Si un lieu n'a que des inactifs, il n'apparaît qu'une fois la case cochée (message « Seulement des patients inactifs… » si c'est le seul). | — | ✅ |
| RCH-48 | Adresse à l'étranger (un patient de test avec un pays autre que la Belgique, si la base en a un). | Le lieu porte le code du pays : « 75001 Paris (FR) » ; un patient belge, aucun code. | Sans pays renseigné : comme le pays du cabinet. | ✅ |
| RCH-49 | Double-clic (Entrée, « Ouvrir la fiche ») sur un patient d'un lieu choisi, puis Retour ; « Retour à la recherche ». | La fiche du patient (onglet Identité) ; contexte « Recherche > Patients par lieu > Nom Prénom ». Retour : la liste avec **son texte, sa case, le même lieu et le patient resélectionné**. « Retour à la recherche » ramène au hub. | Un clic sur un bouton du menu repart de zéro. | ✅ |
| RCH-50 | Journaux ; non-régression. | Journal : « Recherche des patients par lieu : n patient(s) chargé(s). » une fois à l'ouverture, **aucun** code postal ni localité, rien à chaque frappe. Écran Patients (liste, filtres, aperçu, ouverture d'une fiche), recherche rapide par code d'un patient (PA…), recherches des séances et des paiements : comme avant. | — | ✅ |

## Cas de test — R5 : réseau par nom (24/10/2026)

Prérequis : un thérapeute du référentiel suivi par au moins deux patients (fiche Intervenants, thérapeute choisi dans la liste) et un thérapeute sans patient ; un intervenant **saisi à la main** (sans thérapeute du référentiel) chez deux patients, avec la même écriture ou une écriture qui ne diffère que par les accents ou les majuscules ; un contact de même nom et prénom chez deux patients (ex. la mère de deux enfants) ; un suivi terminé (date de fin passée) ; un patient inactif qui a un contact.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RCH-51 | Hub Recherche. | Quatrième tuile **Réseau par nom — Thérapeute, intervenant, contact** (icône personnes + loupe), sous la première rangée ; « D'autres recherches viendront au fil des besoins… ». Clic : écran **Réseau par nom**, contexte « Recherche > Réseau », curseur dans la zone. | Icône provisoire lisible (survol, désactivé). | ✅ |
| RCH-52 | Ouverture, zone vide. | À gauche **Personnes (n)** : tout le réseau par nom puis prénom — colonnes Personne, Nature (Thérapeute / Intervenant / Contact), Spécialité / lien, Patients ; la première sélectionnée ; à droite **Patients — Nom Prénom (n)**. | Colonnes de droite : Code, Patient, Lien / rôle, Depuis, Suivi, Fiche (FC… / AS…). Barre d'état « Réseau par nom : n personne(s). » | ✅ |
| RCH-53 | Taper le début d'un nom (« dup »), puis nom et début du prénom (« dupont ma »), sans accents ni majuscules. | La liste suit chaque frappe ; chaque mot doit commencer un mot du nom ou du prénom ; « Aucune personne de ce nom. » si rien ne répond. | Un nom composé (« Van den Berg », « D'Hondt ») se retrouve par chacun de ses mots. | ✅ |
| RCH-54 | Thérapeute du référentiel suivi par deux patients. | **Une ligne** « Thérapeute », spécialité du référentiel, 2 patients ; à droite ses deux suivis (rôle, depuis, « en cours »). Un thérapeute sans patient apparaît aussi, 0 patient. | Un thérapeute désactivé : « Thérapeute (inactif) », ligne grisée. | ✅ |
| RCH-55 | Intervenant saisi à la main chez deux patients (écritures qui ne diffèrent que par les accents / majuscules). | **Une ligne** « Intervenant », 2 patients ; l'écriture la plus fréquente est affichée ; sa spécialité saisie. | Un intervenant lié à un thérapeute du référentiel n'a pas de ligne « Intervenant » : il compte sous le thérapeute. | ✅ |
| RCH-56 | Contact de même nom et prénom chez deux patients. | **Une ligne** « Contact », 2 patients ; Spécialité / lien = ses liens (« Mère », ou « Mère, Tutrice » s'ils diffèrent) ; à droite un lien par patient (FC…). | Deux contacts de noms différents restent deux lignes. | ✅ |
| RCH-57 | Suivi terminé. | Listé, colonne Suivi « terminé le jj/mm/aaaa » (texte gris) ; un suivi avec une date de fin à venir : « jusqu'au … » ; sans date de fin : « en cours » ; un contact : vide. | — | ✅ |
| RCH-58 | Patient inactif ; cocher puis décocher « Afficher les inactifs ». | Décoché : ses liens ne sont pas listés, barre d'état « — n lien(s) avec des patients inactifs masqué(s) » ; une personne dont tous les patients sont inactifs n'apparaît pas (sauf un thérapeute, toujours listé). Coché : liens listés, patient en gris. | — | ✅ |
| RCH-59 | Double-clic (Entrée, « Ouvrir le patient ») sur un lien contact, puis sur un lien de suivi ; Retour ; « Retour à la recherche ». | Contact : fiche du patient, onglet **Famille / Contacts** ; suivi : onglet **Intervenants** ; contexte « Recherche > Réseau > Nom Prénom ». Retour : la liste avec **son texte, sa case, la même personne et le même lien resélectionnés**. « Retour à la recherche » ramène au hub. | — | ✅ |
| RCH-60 | « Fiche de la personne » (ou double-clic sur la personne) : un contact, un intervenant saisi, un thérapeute. | Contact : sa fenêtre Contact **en consultation** (celle du lien sélectionné à droite, sinon du premier) ; intervenant : sa fenêtre Intervenant en consultation ; thérapeute : sa fenêtre Thérapeute (comme le référentiel Thérapeutes). Fermer sans rien changer : la liste reste telle quelle. | Droits : ceux des fenêtres habituelles. | ✅ |
| RCH-61 | Depuis « Fiche de la personne », modifier puis enregistrer (ex. le prénom d'un contact de test, la spécialité d'un thérapeute). | Après l'enregistrement, la liste est relue et montre la modification, la personne restant sélectionnée si elle répond toujours au texte. | Remettre la valeur d'origine ensuite. | ✅ |
| RCH-63 | Codes visibles (relevé de Joëlle au test de R5) : fiche patient, onglets **Famille / Contacts** et **Intervenants** ; référentiel **Thérapeutes**. | Colonne **Code** à droite des grilles des contacts (FC…) et des intervenants (AS…) ; colonne **Code** (TH…) en tête de la grille des thérapeutes. | Tri par la colonne ; largeur raisonnable, pas de colonne coupée. | ✅ |
| RCH-64 | Ouvrir un contact, un intervenant (consultation, puis « Modifier ») et un thérapeute existants. | Le titre porte le code : « Consulter le contact — FC000004 », « Modifier l'intervenant — AS000007 », « Modifier le thérapeute — TH000003 ». Un nouveau contact / intervenant / thérapeute : titre sans code. | — | ✅ |
| RCH-65 | Créer puis modifier un contact, un intervenant et un thérapeute de test. | Barre d'état : « Contact FC… créé. » / « … mis à jour. », de même « Intervenant AS… », « Thérapeute TH… » ; le code annoncé est celui de la grille. | Supprimer les fiches de test ensuite. | ✅ |
| RCH-66 | Hub : recherche par code « th3 », « TH000003 », un TH inexistant. | L'aide cite « TH thérapeute ». La ligne : Nature « Thérapeute », code TH000003, description « NOM Prénom · spécialité » (« · inactif » s'il est désactivé), colonne Patient vide, « Ouvrir le patient » grisé. Ouvrir (Entrée, double-clic) : écran **Réseau par nom**, zone = son nom, **ce thérapeute sélectionné** et ses patients à droite ; Retour → le hub avec le code. Un TH inexistant : « Aucun élément ». | Journal : « Recherche par code (Therapeute) : n résultat(s). » sans le code. | ✅ |
| RCH-62 | Journaux ; non-régression. | Journal : « Recherche du réseau par nom : n personne(s) chargée(s). » à l'ouverture, **aucun nom**, rien à chaque frappe. Fiche patient (onglets Famille / Contacts et Intervenants, ajout / modification), référentiel Thérapeutes, recherche par code (FC…, AS…), patients par lieu (accents, « Grâce-Hollogne ») : comme avant. | — | ✅ |

## Cas de test — Le code toujours visible (24/10/2026)

Règle : `Rules.md` § 15, `Reference_UI.md` § 5.6 (demande de Joëlle, 24/10/2026).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RCH-67 | Fenêtre **Planning du jour** (depuis une séance) sur un jour avec des séances et un créneau libre. | Colonne **Code** (SE…) après Domaine ; vide sur la ligne « Libre · … » ; fenêtre plus large, sans barre de défilement horizontale ; pastilles de statut et tarif inchangés. | — | ✅ |
| RCH-68 | Administration › **Utilisateurs**. | Colonne **Code** (USR…) en tête de grille ; les autres colonnes tiennent sans défilement ; tri, pastilles d'état et de rôle inchangés. | — | ✅ |
| RCH-69 | Ouvrir un utilisateur en consultation (SuperUser) puis en modification ; créer un utilisateur de test. | Sous-titre « Consulter l'utilisateur USR… » / « Modifier l'utilisateur USR… » ; barre d'état « Modification de l'utilisateur USR…. » (plus d'identifiant) ; création : « L'utilisateur '…' (USR…) a été créé avec succès. » ; création : sous-titre « Gérez les utilisateurs ». | — | ✅ |
| RCH-70 | Fenêtre **Intervenant** : ouvrir la liste des thérapeutes, taper une lettre ; onglet **Cabinet** : « Lier un compte… », puis un praticien avec un compte lié. | Thérapeutes « Prénom NOM (spécialité) · TH… », « (sélectionner…) » sans code ; une lettre saute au premier nom qui commence par elle ; la liste ouverte est assez large pour le code. Comptes : « Nom (login) · USR… » ; « Compte lié : USR… — Nom ». | — | ✅ |
| RCH-71 | Recherche **Réseau par nom** ; non-régression des autres listes déroulantes (filtres de la fiche patient et du dossier, pastilles). | Colonne **Code** en tête de la liste des personnes : TH… pour un thérapeute, vide pour un intervenant saisi ou un contact (leur code est dans la colonne Fiche des liens). Les autres listes s'ouvrent au moins à la largeur du champ, pastilles alignées comme avant. | — | ✅ |

## Cas de test — Pastille « Inactif » (24/10/2026)

Relevé de Joëlle au test des patients inactifs (PAT-130 à PAT-132) : la même pastille que l'écran Patients.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RCH-72 | **Patients par lieu** : case « inactifs » cochée, un lieu qui compte un patient inactif. | Pastille grise **Inactif** devant le nom du patient inactif, aussi sur une ligne sélectionnée ; rien pour les actifs. | Tri par la colonne, largeur : le nom reste lisible. | ✅ |
| RCH-73 | **Réseau par nom** : une personne liée à un patient inactif. | Pastille **Inactif** devant le nom du patient dans la grille de droite ; double-clic → sa fiche comme avant. | — | ✅ |

## Validation finale

- [x] RCH-01 à RCH-12 (R1 : recherche rapide par code) — validés par Joëlle le 23/10/2026.
- [x] RCH-13 (code des paiements, ajouté après ces tests) — validé par Joëlle le 23/10/2026.
- [x] RCH-14 à RCH-27 (R2 : séances d'un jour ou d'une période) — validés par Joëlle le 23/10/2026.
- [x] RCH-28 à RCH-39 (R3 : paiements d'un jour ou d'une période) — validés par Joëlle le 23/10/2026.
- [x] RCH-40 à RCH-50 (R4 : patients par lieu) — validés par Joëlle le 24/10/2026.
- [x] RCH-51 à RCH-62 (R5 : réseau par nom) ; RCH-63 à RCH-66 (codes des contacts, intervenants et thérapeutes visibles ; code TH…) — validés par Joëlle le 24/10/2026.
- [x] RCH-67 à RCH-71 (le code toujours visible) — validés par Joëlle le 24/10/2026.
- [x] RCH-72 à RCH-73 (pastille « Inactif ») — validés par Joëlle le 24/10/2026.
