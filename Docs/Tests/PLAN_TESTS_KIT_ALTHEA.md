# Plan de tests — Contrôles du kit Althéa

> Version : 1.6 — Dernière mise à jour : 20/10/2026 — boutons inactifs lisibles et qui disent pourquoi, section 9
> Périmètre : contrôles de `UI/Controls/Theme/` qui portent un comportement propre (saisie, navigation, animation). Les contrôles purement visuels (cartes, pastilles, barres) se vérifient dans les écrans qui les utilisent.
> Premier contenu : champ de date `AltheaDate` et calendrier `AltheaCalendrier` (UI-7). Plan à compléter à chaque nouveau contrôle du kit à comportement.
> Références : ADR-33 (champs du kit), Rules « Consultation lisible » et « Dates », Charte (kit), maquette « AltheaDate » (calendrier A, date facultative « vide + croix »).

<details>
<summary>Historique des mises à jour</summary>

- 19/10/2026 — modifications non enregistrées, section 8 ; barre de pagination refondue, section 7
- 18/10/2026 — socle des grilles, UI-9.1, section 6
- 16/10/2026 — champ d'heure `AltheaHeure`, section 5
- 14/10/2026 — écrans migrés : fiches patient et dossier

</details>

## Prérequis

- Version compilée en **Debug** : accueil › carte « Outils de test » › **Champ de date** ouvre `TestChampDate` (date obligatoire, facultative, bornée d'aujourd'hui à + 1 an, filtre du / au, case « Lecture seule », journal du dernier changement, calendrier seul).

## 1. AltheaDate — saisie

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| KD-01 | Taper « 141026 ». | 14/10/2026. | Journal : `ValeurChangee`. | ✅ |
| KD-02 | Taper « 14 » puis « 10 ». | Les « / » se placent seuls : « 14/10/ ». | — | ✅ |
| KD-03 | Taper « 3/5/87 » puis sortir du champ. | 03/05/1987 (année sur deux chiffres : siècle actuel sauf futur lointain). | — | ✅ |
| KD-04 | Taper « 14/10 » puis sortir. | Année en cours. | — | ✅ |
| KD-05 | Taper « 31/02/2026 ». | Contour et texte roses dès la saisie complète ; à la sortie, la dernière date valide revient. | Pas de `ValeurChangee`. | ✅ |
| KD-06 | Champ borné : date d'hier. | Contour rose, puis retour à la date valide. | — | ✅ |
| KD-07 | Lettres ou autres caractères. | Refusés ; « - » et « . » deviennent « / ». | — | ✅ |
| KD-08 | Haut / Bas. | + 1 jour / − 1 jour, dans les bornes. | — | ✅ |

## 2. AltheaDate — calendrier et date facultative

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| KD-10 | Clic sur l'icône, Alt+Bas, F4. | Le calendrier s'ouvre sous le champ, sur la date affichée. | — | ✅ |
| KD-11 | Choisir un jour. | Le calendrier se ferme, la date s'inscrit, le focus revient au champ. | — | ✅ |
| KD-12 | Échap ou clic ailleurs. | Fermeture sans changement. | — | ✅ |
| KD-13 | Champ facultatif : vider le texte. | Pas de date (journal : « (aucune date) »). | — | ✅ |
| KD-14 | Champ facultatif rempli : croix. | Date effacée ; la croix disparaît. | — | ✅ |
| KD-15 | Champ obligatoire : vider puis sortir. | La dernière date revient (jamais vide). | — | ✅ |
| KD-16 | Case « Lecture seule ». | Les cinq champs : texte normal, fond teinté, sans icône ; non modifiables ; décocher rétablit la saisie. | Aussi par `UtilsControls.DefinirLectureSeule`. | ✅ |

## 3. AltheaCalendrier

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| KC-01 | Affichage. | Lundi → dimanche, week-end atténué, jours hors du mois pâles, aujourd'hui entouré, date choisie en pastille sauge. | Noms français. | ✅ |
| KC-02 | Flèches de l'en-tête. | Mois précédent / suivant. | — | ✅ |
| KC-03 | Clic sur l'année. | Grille de 12 années ; flèches : page précédente / suivante ; clic : retour au mois dans l'année choisie. | — | ✅ |
| KC-04 | Clavier : flèches, Page haut / bas, Origine, Entrée, Échap. | Jour / semaine, mois, aujourd'hui, choix, abandon. | — | ✅ |
| KC-05 | « Aujourd'hui » après avoir changé de mois. | La date du jour est choisie **et** son mois s'affiche. | Corrigé et revérifié le 14/10/2026. | ✅ |
| KC-06 | Dates hors bornes (champ borné). | Grisées, non sélectionnables. | — | ✅ |

## 4. AltheaFondFenetre (fond des fenêtres)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| KF-01 | Ouvrir `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`. | Fond beige clair uni ; petite feuille sauge dans le coin haut droit, derrière l'en-tête ; plus d'image étirée. | Titre et sous-titre lisibles, sans chevauchement. | ✅ |
| KF-02 | Déplacer une autre fenêtre par-dessus, réduire l'application puis revenir. | La feuille se redessine proprement, sans clignotement ni trace. | — | ✅ |
| KF-03 | Mise à l'échelle Windows 125 % (si possible). | Feuille proportionnée, toujours dans le coin. | `DeviceDpi`. | ☐ |
| KF-04 | Barre de titre (`AltheaBarreTitre`, Windows 11) : Home, connexion, puis une fenêtre reprise (Séance) et une pas encore reprise (ex. catégories de quota). | Barre beige clair, titre sauge très foncé, fine bordure sauge ; plus de bleu ; idem fenêtre inactive. | Déplacer, agrandir (double-clic), ancrer (Win+flèches), réduire, fermer : comportements Windows inchangés. | ✅ |
| KF-05 | Couleur d'accentuation Windows activée ou non (Paramètres › Personnalisation › Couleurs). | Les barres d'Althéa restent beiges dans les deux cas. | Les autres applications ne changent pas. | ☐ |

## 5. AltheaHeure (champ d'heure, 16/10/2026)

Où : fenêtre Séance (Début / Fin), « Reporter la séance », vitrine du kit (onglet Champs, ligne « Heure »).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| KH-01 | Taper « 930 », « 9h30 », « 14 », « 1415 », puis Tab. | 09:30, 09:30, 14:00, 14:15. | Le « : » se place seul après « 9 » et après « 14 ». | ☐ |
| KH-02 | Taper « 25:70 ». | Contour et texte roses dès la saisie complète ; à la sortie, la dernière heure valide revient. | — | ☐ |
| KH-03 | Lettres, deuxième « : ». | Refusés ; « h » et « . » deviennent « : ». | — | ☐ |
| KH-04 | Haut / Bas. | Heure suivante / précédente sur le pas (15 min, ou pas paramétré) ; depuis 09:37 : 09:45 / 09:30. | — | ☐ |
| KH-05 | Clic sur l'horloge, Alt+Bas, F4. | La liste s'ouvre sous le champ, sur l'heure affichée (ou la suivante). | Heures = grille paramétrée (première / dernière heure, pas). | ☐ |
| KH-06 | Liste de la **Fin** (Séance, Report). | Commence à l'heure de début ; durée à droite de chaque ligne (« début », « 45 min », « 1 h 15 »). | Change si le début change. | ☐ |
| KH-07 | Choisir dans la liste : clic, puis Entrée ; Échap. | L'heure s'inscrit, focus au champ ; Échap ferme sans rien changer. | Survol en sauge pâle, heure choisie en sauge. | ☐ |
| KH-07b | Ascenseur de la liste : molette, glisser le curseur, clic sur le rail ; Haut / Bas, Page préc. / suiv., Début / Fin. | Ascenseur fin arrondi (plus de barre grise Windows) ; défilement fluide ; la sélection reste visible. | — | ☐ |
| KH-08 | Séance : changer le Début. | La Fin suit (durée par défaut si la fin est vide ou antérieure, sinon même durée). | Durée recalculée. | ☐ |
| KH-09 | Séance : vider le Début et la Fin, enregistrer. | Séance sans horaire, comme avant. | Pas de croix dans cette fenêtre (`AfficherCroix = False`, place réduite). | ☐ |
| KH-10 | Séance soldée ou reportée, dossier archivé. | Heures lisibles : texte normal, fond teinté, sans horloge. | — | ☐ |
| KH-11 | Report : case « Horaires » décochée puis cochée. | Champs lisibles mais inactifs, puis actifs. | Heures reprises de la séance d'origine. | ☐ |
| KH-12 | Vitrine, ligne « Heure ». | Saisie / consultation / désactivé / facultative avec croix. | La croix efface l'heure. | ☐ |

## 6. Socle des grilles (UI-9.1, 18/10/2026)

> `UtilsDataGrid` V1.7.0 et `UtilsPastilles` V1.4.0 : aucune couleur ne change ; ce qui change se voit surtout sur les lignes alternées et la ligne sélectionnée.

| # | Test | Résultat attendu | Vérification | ✅ |
| --- | --- | --- | --- | --- |
| KG-01 | Grille Patients : regarder les pastilles (Suivi, Prochain RDV, Paiements) sur plusieurs lignes. | Chaque pastille pleine porte un fin contour dans sa teinte, sur ligne blanche comme sur ligne alternée : « Actif » reste visible sur le fond vert-de-gris. | Couleurs inchangées ; « Aucun RDV » reste au trait pointillé. | ☐ |
| KG-02 | Grille des séances d'un dossier, aperçu patient : « Réalisée », « Prévue » ; sélectionner une ligne. | Même contour fin partout, ligne sélectionnée (fond mauve) comprise ; pastilles toujours lisibles. | | ☐ |
| KG-03 | Faire défiler vite une longue grille à pastilles (Patients, séances) à la molette et à l'ascenseur. | Pas de scintillement, pas de morceaux de pastilles en double. | | ☐ |
| KG-04 | Ouvrir et fermer une vingtaine de fois des écrans à grilles (Patients, Utilisateurs, Référentiels). | Polices identiques ; pas de ralentissement. | Optionnel : nombre d'objets GDI stable dans le Gestionnaire des tâches (colonne « Objets GDI »). | ☐ |
| KG-05 | Toutes les grilles (liste principale et grilles dans une carte). | Polices, couleurs, en-têtes inchangés par rapport à avant. | Non-régression. | ☐ |

## 7. Barre de pagination (UC_Pagination V1.1.0, 19/10/2026)

| # | Test | Résultat attendu | Vérification | ✅ |
| --- | --- | --- | --- | --- |
| KP-01 | Grille Patients avec plus de patients qu'une page. | Boutons sans cadre avec chevrons sauge (survol vert-de-gris, désactivés pâles sur la première ou la dernière page), « Page 1 / 3 » au centre gauche, résumé gris à droite « 1–14 sur 40 patients ». | Infobulles « Première page »… | ✅ |
| KP-02 | Liste qui tient sur une page (ex. Famille d'un patient, un référentiel court). | Seul le résumé reste : « 3 contacts », « 1 contact » ; liste vide : « Aucun contact ». | Plus de « (s) ». | ✅ |
| KP-03 | Grille Patients dans l'ordre par défaut avec les groupes « À venir » et « Sans rendez-vous à venir » sur la même page. | Aucune ligne coupée en bas, pas d'ascenseur vertical. | Agrandir puis réduire la fenêtre : la page se recalcule, le premier patient affiché reste visible. | ✅ |
| KP-04 | Cliquer un en-tête de colonne (Patients, un référentiel). | Tri de **toute** la liste, retour en page 1, flèche dans l'en-tête. | | ✅ |
| KP-05 | Ouvrir plusieurs écrans paginés, puis Administration › Journaux. | Plus de lignes « Tag invalide ou manquant » à chaque ouverture d'écran paginé. | | ✅ |

## 8. Modifications non enregistrées (GardeModifications, 19/10/2026)

Message attendu : « Les modifications … n'ont pas été enregistrées. Voulez-vous les enregistrer avant de quitter ? », boutons **Enregistrer** / **Quitter sans enregistrer** / **Rester**.

| # | Test | Résultat attendu | Vérification | ✅ |
| --- | --- | --- | --- | --- |
| KM-01 | Fiche patient : Anamnèse › Modifier, taper un mot, cliquer l'onglet Identité. | Question (« de l'anamnèse »). Rester : on reste sur l'anamnèse, texte intact. | Échap ou la croix = Rester. | ☐ |
| KM-02 | Même situation : Quitter sans enregistrer. | L'onglet change, l'anamnèse revient à son contenu enregistré, en consultation. | — | ☐ |
| KM-03 | Même situation : Enregistrer. | Anamnèse enregistrée, puis changement d'onglet. | Rouvrir la fiche : texte présent. | ☐ |
| KM-04 | Anamnèse modifiée, puis menu de Home (ex. Domaines), puis Retour, puis fermer Althéa. | Question à chaque fois ; Rester annule la navigation ou la fermeture. | Une seule question par action (pas de doublon au Retour). | ☐ |
| KM-05 | Anamnèse en modification **sans** rien changer, puis changer d'onglet ou de menu. | Aucune question. | — | ☐ |
| KM-06 | Fiche patient › Modifier, changer le téléphone, puis Retour (ou menu). | Question « de la fiche du patient » ; Enregistrer = même enregistrement que le bouton (message de réussite) puis navigation ; saisie refusée = on reste. | — | ☐ |
| KM-07 | Fiche › Modifier, changer une valeur puis la remettre telle quelle, puis Retour. | Aucune question (photo identique). | — | ☐ |
| KM-08 | Nouveau patient, saisir un nom, puis Annuler ou Retour. | Question « du nouveau patient ». Sans aucune saisie : aucune question. | — | ☐ |
| KM-09 | Séance : changer le statut ou une note, puis Annuler, la croix ou Échap. | Question « de la séance » ; Enregistrer enregistre et ferme ; Rester garde la fenêtre. | Séance verrouillée ou dossier archivé (consultation) : aucune question. | ☐ |
| KM-10 | Séance modifiée avec une saisie que la fenêtre refuse (champ obligatoire vidé), puis Annuler › Enregistrer. | La fenêtre reste ouverte avec le message de validation habituel. | — | ☐ |
| KM-11 | Contact, intervenant, thérapeute : modifier un champ ou le commentaire, puis Annuler. | Question (« du contact », « de l'intervention », « du thérapeute »). | Contact ou intervenant en consultation : aucune question. | ☐ |
| KM-12 | Zone du dossier : modifier, puis fermer. | Question à trois boutons (« de la zone « … » ») ; zone d'archive : aucune question. | Remplace l'ancienne question Oui / Non. | ☐ |
| KM-13 | Ouvrir une séance, un contact, une zone et fermer **sans rien toucher**. | Aucune question. | Valeurs par défaut d'une nouvelle séance comprises. | ☐ |
| KM-14 | Fiche patient › Modifier, changer un champ, puis **Annuler**. | « Abandonner les modifications de la fiche du patient ? » — Abandonner (Terre) / Continuer la saisie (par défaut, Entrée et Échap). Sans modification : aucune question. | Même question pour Annuler de l'anamnèse. | ☐ |
| KM-15 | Fiche du dossier › Modifier, changer un paramètre ; puis Annuler, puis Retour / menu. | Annuler : question Abandonner / Continuer ; Retour ou menu : question à trois choix (« de la fiche du dossier »). | — | ☐ |
| KM-16 | Domaines : Modifier un domaine (ou Nouvelle règle de quota), changer une valeur, puis Annuler, puis changer de menu. | Annuler : Abandonner / Continuer ; menu : trois choix (« du domaine », « de la règle de quota ») ; Enregistrer passe par la validation habituelle. | Les plafonds modifiés comptent comme une modification. | ☐ |
| KM-17 | Un référentiel (Administration) : Modifier une ligne, changer le libellé, puis Annuler, puis changer de menu. | Mêmes questions (« de l'élément », « du nouvel élément » en création). | — | ☐ |
| KM-18 | Référentiel ouvert en fenêtre depuis une fiche (bouton [+]) : saisir un nouvel élément, puis Fermer ou la croix. | Question à trois choix ; Enregistrer enregistre et referme la fenêtre comme avant. | — | ☐ |
| KM-19 | Cabinet, Paramètres : modifier puis changer de menu ; modifier puis Annuler. | Menu : trois choix (au lieu de Oui / Non) ; Annuler : Abandonner / Continuer. | — | ☐ |
| KM-20 | Paiement, Utilisateur : modifier un champ puis fermer. | Question à trois choix (« du paiement », « de l'utilisateur ») ; consultation : aucune. | — | ☐ |

## 9. Boutons inactifs (20/10/2026)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| KB-01 | Vitrine › Boutons : colonne « Désactivé » des boutons d'action (Sauge, Ocre, Terre, Clair). | Texte lisible sans effort, bouton toujours visiblement inactif ; icône atténuée mais reconnaissable | - Comparer avec la colonne « Normal » | ☐ |
| KB-02 | Vitrine › Boutons : boutons Outil désactivés (texte et icône seule). | Même lisibilité sur le fond de la carte | — | ☐ |
| KB-03 | Survoler un bouton désactivé de la vitrine. | Infobulle « Exemple : sélectionnez d'abord un élément dans la liste. » sous le bouton ; elle disparaît en quittant le bouton | - Passer d'un bouton désactivé à un autre : une seule infobulle, celle du bouton survolé | ☐ |
| KB-04 | Survoler un bouton actif. | Son infobulle habituelle (s'il en a une), pas la raison | — | ☐ |

## Validation

- [x] Démonstration `TestChampDate` validée par Joëlle le 14/10/2026 (KC-05 corrigé et revérifié le même jour).
- [x] Barre de pagination (KP-01 à KP-05) validée par Joëlle le 19/10/2026.
- [ ] Contrôles vérifiés dans chaque écran migré au fil d'UI-7 : fiche patient ✅ (PAT-18a à c), fiche dossier ✅ (D-15a à c ; D-15d à revérifier), fenêtres ☐.
