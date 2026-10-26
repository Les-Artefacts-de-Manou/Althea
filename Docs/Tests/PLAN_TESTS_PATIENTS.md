# Plan de tests — Patients

> Version : 1.14 — Dernière mise à jour : 25/10/2026 — relevés aux captures : PAT-133 à PAT-136
> Périmètre : `UC_PatientHome`, `UC_PatientFiche`, `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`, `GestionPatients`, `GestionFamilleContacts` et `GestionSuivisIntervenants`.

<details>
<summary>Historique des mises à jour</summary>

- 24/10/2026 — codes postaux belges, v2.56.0 : PAT-113 à PAT-129
- 23/10/2026 — spécialité d'un thérapeute choisie dans un référentiel, v2.55.0 : PAT-110 à PAT-112
- 23/10/2026 — audit C.2 étape 2b : contacts, intervenants et thérapeutes jugés par le métier, homonyme de thérapeute confirmé, fiche Intervenant complétée : PAT-99 à PAT-109
- 23/10/2026 — audit C.2 étape 2a : fiche jugée par le métier, NISS normalisé, fenêtre des erreurs : PAT-87 à PAT-98
- 21/10/2026 — dossier dominant et montants lus dans les vues, audit B.4 : PAT-84 à PAT-86
- 20/10/2026 — téléphone de l'intervenant : PAT-83 ; latéralité du référentiel : PAT-82
- 18/10/2026 — fiche patient, grilles Dossiers et Intervenants : PAT-80, PAT-81
- 17/10/2026 — grille Patients « hub de la journée », étape 2 : PAT-50 à PAT-59, validés ; raccourci vers la séance : PAT-60 à PAT-62, validés ; étape 3, résumé et filtres : PAT-63 à PAT-70, validés ; étape 4, icônes : PAT-71, validé ; étape 5, panneau d'aperçu : PAT-72 à PAT-79, validés
- 1.3 du 14/10/2026 — UI-7 : champs du kit et consultation lisible de l'onglet Identité, puis des fenêtres Contact, Intervenant et Thérapeute — PAT-29a à 29e ; 02/10/2026 : contrôles de saisie par pays

</details>

## Objectif

Valider le parcours de gestion d’un patient : recherche, création, consultation, modification, activation, identité, données administratives, alertes, anamnèse, photo, contacts, intervenants et accès aux dossiers.

## Prérequis et données de test

- Application compilée, base MariaDB accessible et journalisation disponible.
- Comptes de test autorisé et non autorisé selon les actions contrôlées.
- Référentiels actifs : situations familiales, liens patient, rôles légaux, rôles d’intervenant, thérapeutes et domaines.
- Un patient actif avec dossiers, contacts, intervenants, alerte, anamnèse et photo ; un patient inactif ; un patient sans dossier.
- Réserver les préfixes `TEST_PAT_`, `TEST_CONTACT_` et `TEST_INTERV_` aux données créées pendant la recette.

## Cas de test — Liste, recherche et navigation

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-01 | Ouvrir l’accueil Patients. | Liste chargée, triée par dernière modification décroissante. | Aucun accès base direct depuis l’UI ; statut de chargement cohérent. | ☐ |
| PAT-02 | Vérifier les colonnes et indicateurs de la liste. | Code, identité, NISS, domaine/statut dominant et alertes sont cohérents. | Domaine déterminé à partir des dossiers conformément aux règles métier. | ☐ |
| PAT-03 | Rechercher par nom, prénom, NISS, code et contenu de contact. | Recherche insensible à la casse, résultats cohérents pour chaque critère. | Aucun résultat hors correspondance. | ☐ |
| PAT-04 | Rechercher un texte absent puis réinitialiser. | Liste vide puis liste complète restaurée ; bouton Réinitialiser cohérent. | Critères et état des boutons remis à zéro. | ☐ |
| PAT-05 | Filtrer par état de dossier : actif, pause, clôturé, archivé et sans dossier. | Seuls les patients correspondant au filtre sont affichés. | Les filtres de suivi restent distincts de l’activation patient. | ☐ |
| PAT-06 | Filtrer par patient actif/inactif. | Les états d’activation manuelle sont correctement séparés. | Un dossier clôturé/archivé ne désactive pas implicitement le patient. | ☐ |
| PAT-07 | Filtrer par domaine, impayés et absence sans prévenir. | Résultats cohérents avec dossiers, paiements et séances concernés. | Combinaison avec une recherche textuelle. | ☐ |
| PAT-08 | Ouvrir/modifier un patient sélectionné puis revenir à la liste. | La sélection et les critères de recherche sont restaurés. | Navigation et contexte Home conservés. | ☐ |
| PAT-09 | Cliquer sur Ouvrir ou Modifier sans sélection. | Action désactivée ou message maîtrisé. | Aucune exception ni navigation invalide. | ☐ |

## Cas de test — Identité et données administratives

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-10 | Créer un patient avec nom et prénom obligatoires. | Patient créé ; code généré ; fiche en consultation après enregistrement. | Ligne `patients` créée, dates renseignées. | ☐ |
| PAT-11 | Tenter de créer sans nom ou sans prénom. | Validation visible, enregistrement refusé. | Aucune ligne créée. | ☐ |
| PAT-12 | Renseigner ou modifier date de naissance, latéralité, adresse, téléphone, e-mail, mutualité (liste, n° d'affiliation, CT1 / CT2 — voir aussi `PLAN_TESTS_REFERENTIELS.md` §14.22) et situation familiale. | Valeurs persistées et restituées après rechargement. | FKs de référentiel valides. | ☐ |
| PAT-13 | Créer avec un NISS unique, puis modifier le même patient sans changer son NISS. | Création et modification autorisées. | Contrôle exclut le patient courant. | ☐ |
| PAT-14 | Créer ou modifier avec le NISS d’un autre patient. | Enregistrement refusé avec message clair. | Unicité respectée en UI et base. | ☐ |
| PAT-15 | Créer un homonyme avec même nom, prénom et date de naissance. | Doublon potentiel signalé conformément au flux prévu. | Aucun doublon silencieux. | ☐ |
| PAT-16 | Annuler une création ou une modification. | Données non enregistrées abandonnées ; données existantes restaurées. | Base inchangée. | ☐ |
| PAT-17 | Modifier les données d’un patient existant. | Modification sauvegardée ; date de modification actualisée. | Liste et bandeau reflètent les données courantes. | ☐ |
| PAT-18 | Vérifier le tarif par défaut proposé pour un patient lorsque ce champ est disponible. | Seuls les prix actifs sont proposés ; l’absence de sélection reste possible. | La tarification effective d’une séance reste testée par le plan Tarification et quotas. | ☐ |
| PAT-18a | Consultation d'un patient existant (UI-7). | Toutes les valeurs lisibles (texte normal, fond teinté) : zones de texte, listes du kit, date de naissance ; rien de modifiable ; « Fiche de la mutualité » utilisable. | `DefinirLectureSeule` sur les trois cartes. | ✅ |
| PAT-18b | Modifier puis Annuler / Enregistrer. | Zones de texte blanches, listes et date utilisables ; Annuler rétablit les valeurs. | — | ✅ |
| PAT-18c | Date de naissance facultative : saisie (« 030587 »), effacement par la croix, enregistrement. | Âge recalculé à chaque changement ; date vide enregistrée comme inconnue (NULL) ; nouveau patient : date vide. | `AltheaDate.Facultative`. | ✅ |

## Cas de test — Bandeau, alerte, photo et anamnèse

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-19 | Consulter le bandeau d’un patient. | Nom complet, code, indicateur d’alerte et photo reflètent le patient chargé. | Aucun résidu d’un patient précédent. | ☐ |
| PAT-20 | Créer/modifier/supprimer une alerte enrichie. | Le contenu est persisté en RTF et texte ; l’indicateur de la liste évolue. | Recherche/affichage simple possibles avec le texte brut. | ☐ |
| PAT-21 | Importer une photo dans un format autorisé. | Image affichée et fichier associé au patient via le chemin déterministe. | Fermeture/réouverture de fiche conserve l’image. | ☐ |
| PAT-22 | Tenter d’importer un format non autorisé ou fichier illisible. | Import refusé avec message clair ; photo existante préservée. | Aucun fichier invalide associé. | ☐ |
| PAT-23 | Modifier puis enregistrer l’anamnèse. | Cycle local Modifier/Enregistrer terminé sans modifier les autres champs de la fiche. | RTF et texte brut persistés. | ☐ |
| PAT-24 | Modifier l’anamnèse puis annuler ou fermer. | Avertissement en cas de modification non enregistrée ; contenu initial restauré si abandon confirmé. | Aucun écrasement involontaire. | ☐ |

## Cas de test — Contacts, intervenants et dossiers

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-25 | Ouvrir une nouvelle fiche avant son premier enregistrement. | Les onglets dépendants du patient sont indisponibles ou expliqués. | Aucun contact, intervenant ou dossier orphelin possible. | ☐ |
| PAT-26 | Créer, modifier, rechercher et supprimer/désactiver un contact familial. | Grille et données persistent ; lien patient/rôle légal cohérent. | Retour/annulation sans écriture partielle. | ☐ |
| PAT-27 | Créer, modifier et retirer un intervenant associé. | Rôle, thérapeute et dates de suivi sont persistés. | Aucune association invalide ou doublon non contrôlé. | ☐ |
| PAT-28 | Ouvrir l’onglet Dossiers d’un patient enregistré. | Dossiers du seul patient courant affichés ; création/ouverture respecte le contexte patient. | Relais vers le plan Dossiers. | ☐ |
| PAT-29 | Consulter un patient sans dossier, contact ni intervenant. | Écrans vides explicites, sans erreur. | Les actions de création restent disponibles selon droits. | ☐ |
| PAT-29a | `ContactEdition` (UI-7) : consulter un contact (double-clic), puis Modifier. | Quatre cartes du kit ; en consultation, valeurs lisibles (texte normal, fond teinté), rien de modifiable ; en saisie, zones de texte blanches, listes et date utilisables. | `DefinirLectureSeule` sur trois cartes ; commentaire en lecture seule propre. | ✅ |
| PAT-29b | `ContactEdition` : date de naissance facultative (saisie, croix, enregistrement) ; boutons [+] et copie d'adresse. | Date vide enregistrée comme inconnue ; [+] et copie d'adresse inchangés. | `AltheaDate.Facultative`. | ✅ |
| PAT-29c | `IntervenantEdition` (UI-7) : consulter, modifier, dates « Depuis » / « Jusqu'au » facultatives, fin avant début. | Champs de trace toujours en lecture seule (fond teinté, y compris en saisie) ; dates effaçables ; fin antérieure refusée (erreur sur la date de fin). | Thérapeute, rôle et dates persistés. | ✅ |
| PAT-29d | `TherapeuteEdition` (UI-7) : créer et modifier un thérapeute, case « Actif ». | Cartes du kit, zones de texte blanches, listes et case du kit ; enregistrement inchangé. | — | ✅ |
| PAT-29e | Petits boutons du kit : [+] (lien, rôle, thérapeute, situation familiale), loupe (thérapeutes, fiche de la mutualité), copie d'adresse. | Icône centrée, fond vert pâle au survol ; même action qu'avant ; en consultation, icône atténuée et bouton inactif (sauf fiche de la mutualité). | Infobulles inchangées. | ✅ |

## Cas de test — Activation, droits et robustesse

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-30 | Désactiver un patient actif. | État inactif et date de désactivation enregistrés. | Les dossiers ne changent pas de statut. | ☐ |
| PAT-31 | Réactiver un patient. | État actif et date d’activation mis à jour ; désactivation réinitialisée selon règle. | Filtre actif/inactif actualisé. | ☐ |
| PAT-32 | Vérifier les actions avec un compte sans droit requis. | Bouton masqué/désactivé ou action refusée selon le contrôle prévu. | Aucune écriture malgré un appel non autorisé. | ☐ |
| PAT-33 | Simuler une indisponibilité de base à la création, modification ou activation. | Erreur contrôlée et journalisée ; interface cohérente. | Aucune écriture partielle. | ☐ |
| PAT-34 | Fermer une fiche avec modifications non enregistrées. | Confirmation explicite ; navigation respectant le choix utilisateur. | Aucune perte silencieuse. | ☐ |

## Cas de test — Contrôles de saisie par pays

Règles : téléphone et code postal sont validés selon le **pays de l’adresse** (code postal : Belgique, France, Luxembourg uniquement ; autres pays non contrôlés).

Le NISS est validé selon le **type d’identifiant** choisi à côté du champ (Belgique par défaut, France, Luxembourg, Autre = non contrôlé).

L’erreur s’affiche dès la sortie du champ ou au changement de pays/type ; l’enregistrement est bloqué tant qu’elle subsiste.

Utilitaires : `UtilsTelephone`, `UtilsCodePostal`, `UtilsNiss`.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-35 | Fiche patient, type Belgique, NISS `85.07.30-033.28`. | Accepté. | Séparateurs ignorés ; valeur persistée. | ☐ |
| PAT-36 | Même NISS avec le dernier chiffre modifié, puis 10 chiffres. | Icône d’erreur à la sortie du champ ; enregistrement bloqué, statut « le NISS est invalide ». | Clé modulo 97 (y compris naissances 2000+). | ☐ |
| PAT-37 | NISS français valide (15 caractères, Corse 2A/2B comprise) avec type France, puis le même avec type Belgique. | Accepté en France ; refusé en Belgique. | Le changement de type relance immédiatement le contrôle. | ☐ |
| PAT-38 | Matricule luxembourgeois valide (13 chiffres) avec type Luxembourg ; puis un chiffre de contrôle modifié. | Accepté puis refusé. | Contrôles Luhn + Verhoeff. | ☐ |
| PAT-39 | Type Autre avec une valeur quelconque. | Accepté sans contrôle de format. | L’unicité du NISS reste contrôlée (PAT-14). | ☐ |
| PAT-40 | Rouvrir un patient existant dont le NISS ne respecte aucun format. | Type affiché « Autre » ; enregistrement possible. | Type déduit au chargement, non stocké en base. | ☐ |
| PAT-41 | Pays Belgique, code postal `75001` ; puis passer le pays à France. | Erreur en Belgique ; l’erreur disparaît au passage à France sans ressaisie. | BE : 4 chiffres 1000-9999 ; FR : 5 chiffres 01000-98999. | ☐ |
| PAT-42 | Pays Luxembourg, codes `1234`, `L-1234`, `12345`. | Deux premiers acceptés, troisième refusé. | Préfixe « L- » toléré. | ☐ |
| PAT-43 | Pays Allemagne ou Pays-Bas, code postal quelconque. | Aucun contrôle. | Les autres pays restent sélectionnables. | ☐ |
| PAT-44 | Fiche patient, pays Belgique, téléphone trop court (ex. `0475 12`). | Erreur dès la sortie du champ ; enregistrement bloqué. | Validation selon le pays, comme Contact et Thérapeute. | ☐ |
| PAT-45 | Saisir le téléphone et le code postal **avant** de choisir le pays, puis changer le pays. | Le téléphone est reformaté et les erreurs sont recalculées selon le nouveau pays. | Ordre de saisie sans effet sur le résultat final. | ☐ |
| PAT-46 | `ContactEdition` et `TherapeuteEdition` : rejouer PAT-41 à PAT-45. | Même comportement ; enregistrement bloqué, focus sur le premier champ en erreur. | ErrorProvider de la fenêtre. | ☐ |
| PAT-47 | Laisser vides téléphone, code postal, NISS et e-mail. | Enregistrement autorisé. | Champs facultatifs. | ☐ |
| PAT-48 | E-mail `jean.dupont@exemple.be`, `j-o'neil+psy@mail.co.uk`. | Acceptés. | Tous pays confondus (pas de règle par pays). | ☐ |
| PAT-49 | E-mails `jean@exemple`, `jean..dupont@ex.be`, `jean@ex..be`, `jean dupont@ex.be`, `jean@ex.b`, `@ex.be`. | Icône d’erreur dès la sortie du champ ; enregistrement bloqué (Fiche patient, Contact, Thérapeute). | `UtilsValidation.IsValidEmail` (254 caractères max). | ☐ |

## Cas de test — Grille Patients, hub de la journée (17/10/2026)

Données de test : un patient avec une séance plus tard aujourd'hui, un avec une séance du jour déjà passée, un avec un rendez-vous dans la semaine, un dossier actif sans rendez-vous, un dossier en pause, un patient sans dossier, un patient inactif, un patient avec deux dossiers, une absence non excusée, une alerte.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-50 | Ouvrir l'écran Patients. | Colonnes Code, Nom, Prénom, Âge, Suivi, Prochain RDV, Dernière séance, Paiements, Abs., Alerte, Téléphone ; plus de Naissance, NISS, Email, Photo, Actif, Dossiers. | Nom élastique ; lignes de 30 px. | ✅ |
| PAT-51 | Ordre par défaut. | D'abord les patients attendus aujourd'hui (par heure, lignes teintées en bleu très léger), marge « À venir » puis rendez-vous à venir (par date), marge « Sans rendez-vous à venir » puis les autres (par nom). | Marges seulement entre groupes présents. | ✅ |
| PAT-52 | Colonne Prochain RDV. | « Aujourd'hui 14:00 » bleu plein ; « Aujourd'hui 09:00 · passée » atténuée ; « Demain 09:00 » / « lun. 19/10 · 17:00 » bleu pâle ; « Aucun RDV » au trait ocre pour un dossier **actif** sans rendez-vous ; rien pour un dossier en pause, clôturé ou sans dossier. | Infobulle : date et heure en toutes lettres. | ✅ |
| PAT-53 | Colonne Dernière séance. | « 26/09 · il y a 3 sem. » ; une absence non excusée n'est pas prise comme dernière séance. | Infobulle « Dernière séance où le patient est venu ». | ✅ |
| PAT-54 | Colonne Suivi. | « Actif · PSY » sauge, « En pause · GRA » ocre, « Clôturé · PSY » / « Archivé · PSY » gris pâle, « +1 » si deux dossiers, « Sans dossier » en gris italique. | Infobulle : nombre de dossiers. | ✅ |
| PAT-55 | Colonnes Paiements et Abs. | « À régler 60 € » rose, « Avoir 30 € » mauve, sinon « À jour » ; absences en pastille orange, vide si aucune. | Montants identiques à l'ancienne colonne (tous dossiers). | ✅ |
| PAT-56 | Colonne Alerte. | Icône d'avertissement ; l'infobulle montre le texte de l'alerte (coupé au-delà de 400 caractères). | Rien pour un patient sans alerte. | ✅ |
| PAT-57 | Patient inactif. | Ligne grisée, « (inactif) » après le prénom, pastilles atténuées. | Toujours ouvrable. | ✅ |
| PAT-58 | Trier par un en-tête (Nom, Âge, Prochain RDV, Paiements…), puis changer de filtre. | Tri appliqué, marges retirées ; un nouveau filtre revient à l'ordre par défaut avec ses marges. | Pagination : page 1. | ✅ |
| PAT-59 | Couleur « En pause » dans l'en-tête d'un dossier. | Pastille ocre (plus l'orange des absences). | `UtilsPastilles.TeinteStatutDossier`. | ✅ |
| PAT-60 | Double-clic sur « Aujourd'hui 14:00 », sur « Aujourd'hui 09:00 · passée », puis sur un rendez-vous à venir. | La fenêtre Séance s'ouvre sur la bonne séance (code dans l'en-tête), Notes et Évolution modifiables ; à la fermeture, la liste revient sur la même page. | Infobulle « Double-clic : ouvrir la séance SE… ». | ✅ |
| PAT-61 | Double-clic sur « Dernière séance ». | Ouvre la dernière séance où le patient est venu (pas une absence). | Même fenêtre. | ✅ |
| PAT-62 | Double-clic sur « Aucun RDV », une cellule vide ou une autre colonne ; puis sur la séance d'un dossier clôturé. | Fiche patient comme avant ; séance d'un dossier clôturé ouverte en consultation seulement. | Rien n'est modifiable en consultation. | ✅ |
| PAT-63 | Résumé au-dessus de la grille. | Pastilles « N aujourd'hui », « N sous 7 jours », « N suivis sans rendez-vous », puis « À régler X € », « Avoir X € », « N avec absences », « N alertes » ; une pastille à zéro n'apparaît pas. | Nombres cohérents avec la grille. | ✅ |
| PAT-64 | Cliquer chaque pastille, puis la recliquer. | La grille ne montre que ces patients (pastille cerclée, en gras) ; le second clic revient à tous. | « Réinitialiser » s'active. | ✅ |
| PAT-65 | Choisir une pastille, puis taper une recherche qui la vide. | La pastille choisie reste affichée (« 0 … ») et permet de revenir à tous d'un clic. | — | ✅ |
| PAT-66 | Filtre « Dossier ». | Étiquette « Dossier » ; Tous, Actif, En pause, Clôturé, Archivé, Sans dossier (plus d'entrées patient actif / inactif, impayés, absences). | Icônes à gauche des libellés ; élément choisi sur une ligne dans le champ fermé. | ✅ |
| PAT-67 | Patients inactifs. | Masqués par défaut ; la barre d'état signale « N patient(s) inactif(s) masqué(s) » quand la recherche en trouve ; cocher « Afficher les inactifs » les montre, grisés. | Le résumé suit la case. | ✅ |
| PAT-68 | Combiner recherche, Dossier, Domaine, inactifs et une pastille, ouvrir une fiche, revenir. | Tout est restauré (texte, filtres, case, pastille, page). | Mini-pile D-Q15. | ✅ |
| PAT-69 | « Réinitialiser ». | Recherche vidée, Dossier et Domaine à « Tous », case décochée, aucune pastille choisie. | Bouton grisé ensuite. | ✅ |
| PAT-70 | Onglets Séances et Paiements d'un dossier : résumés cliquables. | Fonctionnent comme avant (même classe) ; une pastille choisie qui tombe à zéro reste affichée. | Non-régression. | ✅ |
| PAT-71 | Ouvrir la liste « Dossier ». | Icônes dessinées : ▶ vert-de-gris (Actif), ‖ ocre (En pause), coche grise (Clôturé), boîte grise (Archivé), cercle pointillé (Sans dossier) ; nettes, mêmes couleurs que les pastilles de la grille. | Champ fermé : icône de l'élément choisi. | ✅ |
| PAT-72 | Ouvrir l'écran Patients. | Panneau d'aperçu ouvert à droite avec « Cliquez sur un patient… » ; Âge, Dernière séance, Abs. et Téléphone masqués dans la grille. | Bouton « Masquer l'aperçu ». | ✅ |
| PAT-73 | Cliquer une ligne, puis parcourir la liste au clavier. | Le panneau montre le patient sélectionné (court délai, pas de clignotement à chaque ligne). | Identité, âge, photo ou initiales. | ✅ |
| PAT-74 | Patient avec alerte, puis sans alerte. | Alerte en entier dans l'encadré jaune ; encadré absent sans alerte. | — | ✅ |
| PAT-75 | Patient qui vient aujourd'hui. | Section « Aujourd'hui — … » : heure, code, nature, dossier ; « Notes de la séance SE… » ouvre la séance (prochaine du jour, sinon la dernière passée). | Patient sans séance aujourd'hui : section absente. | ✅ |
| PAT-76 | Section Séances. | 2 prochaines puis 3 dernières séances dues ; « notes ✓ / sans notes » (jamais le texte) ; pastille de paiement ou « Absence » ; double-clic ouvre la séance ; au retour, le patient reste sélectionné et l'aperçu est à jour. | Infobulle : code de séance et dossier. | ✅ |
| PAT-77 | Sections Dossier, Finances, Contacts, Documents. | Dossier dominant (statut, domaine, ouverture, quota INAMI s'il y a lieu, autres dossiers) ; à régler / à jour, avoir, absences — mêmes montants que la grille ; téléphone, e-mail, entourage ; Documents : texte d'attente. | Patient sans dossier : « Aucun dossier. », bouton dossier masqué. | ✅ |
| PAT-78 | « Ouvrir la fiche », « Ouvrir le dossier DO… », puis retour. | Fiche / hub du dossier ouverts ; le retour restaure l'écran Patients (filtres, page). | Mini-pile D-Q15. | ✅ |
| PAT-79 | « Masquer l'aperçu », puis « Afficher l'aperçu ». | Panneau fermé, colonnes Âge, Dernière séance, Abs., Téléphone revenues ; réouverture : aperçu du patient sélectionné. | — | ✅ |

## Cas de test — Fiche patient, grilles (UI-9.2, 18/10/2026)

| # | Test | Résultat attendu | Vérification | ✅ |
| --- | --- | --- | --- | --- |
| PAT-80 | Onglet Dossiers d'un patient avec un dossier actif, un en pause et un clôturé. | Statut en pastille : sauge (actif), ocre (en pause), gris pâle (clôturé) — comme la colonne « Suivi » de la grille Patients ; la ligne du dossier clôturé est atténuée. | Dates d'ouverture et de clôture centrées ; tri et double-clic inchangés. | ☐ |
| PAT-81 | Onglet Intervenants avec une intervention terminée (« Jusqu'au » passé) et une en cours. | La ligne terminée est atténuée (gris), l'autre normale. | Dates centrées. | ☐ |
| PAT-83 | Intervenant de suivi lié à un thérapeute qui a un téléphone : enregistrer, rouvrir ; puis changer le téléphone dans la fiche du thérapeute et rouvrir le suivi (20/10/2026, audit B12). | À la réouverture, le téléphone est affiché (il restait vide) ; c'est le numéro **actuel** du thérapeute. | Infobulle de la case : téléphone actuel, non enregistré avec le suivi ; thérapeute désactivé depuis : son numéro reste affiché. | ☐ |
| PAT-82 | Fiche patient (migration v2.46.0) : patient qui avait une latéralité, puis nouveau patient ; choisir « Gaucher contrarié » (ajouté au référentiel), enregistrer, rouvrir. | La latéralité d'avant la migration est retrouvée ; la liste propose le référentiel (« — » = aucune) ; le choix est relu à l'ouverture | Latéralité désactivée depuis : toujours affichée chez le patient, plus proposée aux autres ; Annuler / modifications non enregistrées comme avant | ☐ |

## Cas de test — Dossier dominant et montants (audit B.4, ADR-48, 21/10/2026)

Migration v2.49.0 appliquée (plan Paiements, P-71). **Patients de test** uniquement.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-84 | Patient de test avec **deux dossiers actifs** de domaines différents : DA ouvert aujourd'hui sans séance, DB ouvert il y a un mois avec une séance la semaine prochaine. | Colonne « Suivi » : statut et domaine de **DB** (séance la plus proche). Aperçu : section Dossier sur DB ; « Ouvrir le dossier DO… » ouvre DB. | Filtre « Domaine » : le patient apparaît sous le domaine de DB. Puis planifier une séance demain dans DA : DA devient le dossier dominant (grille et aperçu). | ✅ |
| PAT-85 | Patient de test avec un dossier **clôturé** (séance hier) et un dossier **en pause** (aucune séance). | « En pause · … » : un dossier vivant passe avant la séance la plus proche. | Statut et domaine viennent du **même** dossier (domaine du dossier en pause). | ✅ |
| PAT-86 | Patient de test des cas P-72 et P-75 (plan Paiements). | Colonne « Paiements » et résumé (« À régler X € », « Avoir X € ») identiques au bandeau des dossiers ; l'absence non facturable n'y est pas comptée. | Colonnes « Prochain RDV » et « Dernière séance » inchangées (une séance passée de quelques minutes n'est plus « prochaine »). | ✅ |

## Cas de test — Fiche patient jugée par le métier, NISS normalisé (audit C.2 étape 2a, 23/10/2026)

**Avant tout** : dans HeidiSQL, lancer la requête de contrôle écrite en tête de `Docs/Database/Migration/migration_v2.54.0_niss_normalise.sql` (codes patients seulement) — elle doit ne rien rendre — puis appliquer la migration, en `root`. **Patients de test** uniquement.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-87 | Après la migration : `SELECT COUNT(*) FROM patients WHERE niss REGEXP '[[:space:]./-]';` puis démarrer Althéa. | **0** ; démarrage normal (base 2.54.0). | État du système : version de la base 2.54.0, OK. | ✅ |
| PAT-88 | Ouvrir un patient de test qui a un NISS belge. | NISS affiché **mis en forme** (« 85.07.30-033.28 »), type Belgique. | Rien n'est marqué modifié à l'ouverture (quitter ne demande rien). | ✅ |
| PAT-89 | Modifier : retaper le NISS **sans séparateurs**, sortir du champ ; Enregistrer. Puis écran Patients : chercher « 85.07.30 », puis « 850730 ». | À la sortie du champ, le NISS est remis en forme ; enregistrement accepté. Les **deux** recherches trouvent le patient. | — | ✅ |
| PAT-90 | Second patient de test : saisir le **même** NISS écrit autrement (« 85 07 30 033 28 ») ; Enregistrer. | **Refus** : icône sur le NISS et fenêtre « Veuillez corriger ce point : • Ce NISS est déjà utilisé par un autre patient. » | Avant : accepté (deux écritures du même numéro). | ✅ |
| PAT-91 | Fiche d'un patient de test : vider **nom** et **prénom**, téléphone `0475 12` (Belgique), CT1 `12` ; Enregistrer. | **Une** fenêtre liste les 4 points ; icône sur chacun des 4 champs ; focus sur le Nom ; barre d'état « Fiche patient : 4 point(s) à corriger. » | Avant : icônes et barre d'état seulement. | ✅ |
| PAT-92 | Type **France** avec un NIR valide ; puis type **Autre** avec une valeur quelconque (ex. `AB-12/34`). | France : accepté, affiché « 1 85 07 2A 033 033 28 » (groupes) ; Autre : accepté, affiché sans séparateurs. | PAT-37 et PAT-39 inchangés. | ✅ |
| PAT-93 | Créer un patient de test homonyme d'un autre (même nom, prénom et date de naissance). | Confirmation « Doublon potentiel » comme avant ; « Oui » enregistre. | Le doublon reste une confirmation, pas un refus. | ✅ |
| PAT-94 | Non-régression : rejouer PAT-35 à PAT-49 (fiche patient). | Mêmes résultats ; les refus s'affichent en plus dans la fenêtre récapitulative. | — | ✅ |
| PAT-96 | (Relevé de Joëlle) Type **France**, numéro de sécurité sociale **sans la clé** (13 caractères) ; puis le même avec sa clé (15) ; puis avec une clé fausse. Rouvrir la fiche après chaque enregistrement. | 13 : accepté, affiché « 1 85 07 99 123 456 », type **France** à la réouverture ; 15 : accepté, clé affichée en dernier groupe ; clé fausse : refusé (« clé de contrôle incorrecte »). | Un matricule luxembourgeois (13 chiffres) rouvert reste de type **Luxembourg**. | ✅ |
| PAT-97 | (Relevé de Joëlle) Créer un patient de test de **mêmes nom et prénom** qu'un autre, **sans** date de naissance ; puis un autre avec une date, l'existant n'en ayant pas ; puis deux dates connues et **différentes**. | Les deux premiers cas : confirmation « Un patient portant les mêmes nom et prénom existe déjà (même date de naissance, ou date non renseignée)… » ; le troisième : aucune confirmation. | Avant : sans date, aucun avertissement. | ✅ |
| PAT-98 | (Relevé de Joëlle) Provoquer une erreur (nom vide) ; corriger le champ **sans** enregistrer ; puis enregistrer. | L'icône disparaît **dès la correction** du champ ; après l'enregistrement, aucune icône ne reste. | Avant : l'icône restait. | ✅ |
| PAT-95 | Créer un patient de test complet (adresse, téléphone, e-mail, mutualité, CT1/CT2) ; puis le modifier. | Enregistré ; onglets Contacts, Intervenants, Dossiers activés comme avant. | Journal : identifiants seulement. | ✅ |

## Cas de test — Contacts, intervenants et thérapeutes jugés par le métier (audit C.2 étape 2b, 23/10/2026)

Patients, contacts et thérapeutes **de test** uniquement (`TEST_PAT_`). Pas de migration.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-99 | Contact d'un patient de test : vider **nom** et **prénom**, téléphone `0475 12` (Belgique), e-mail `jean@exemple` ; Enregistrer. | **Une** fenêtre « Contact » liste les 4 points ; icône sur chacun des 4 champs ; focus sur le Nom ; la fenêtre reste ouverte. | Rien n'est enregistré (rouvrir le contact). | ✅ |
| PAT-100 | Suite de PAT-99 : corriger les champs un à un, puis Enregistrer. | Chaque icône disparaît **dès la correction** de son champ ; enregistrement accepté. | Journal : identifiants seulement. | ✅ |
| PAT-101 | Contact : lien ou rôle légal sans choix (si la liste le permet) ; Enregistrer. | Le point est dans la fenêtre ; l'icône se place sur le bouton **[+]** de la liste, le focus sur la liste. | — | ✅ |
| PAT-102 | Intervenant d'un patient de test : laisser « (sélectionner…) » comme thérapeute ; Enregistrer. | Fenêtre « Intervenant » : « Le thérapeute est obligatoire. » ; icône sur la liste des thérapeutes. | — | ✅ |
| PAT-103 | Intervenant : date de fin **avant** la date de début ; Enregistrer ; puis corriger la date de fin. | Fenêtre : « La date de fin ne peut pas précéder la date de début. » ; icône sur la date de fin, qui disparaît dès qu'on change cette date. | Enregistrement accepté ensuite. | ✅ |
| PAT-104 | (Relevé de Joëlle) Intervenant : choisir un thérapeute de test qui a téléphone, e-mail et adresse complète (2 lignes, code postal, localité). | En lecture seule : **Téléphone**, **E-mail** et **Adresse** « ligne 1, ligne 2, code postal localité » (la case « Lieu » s'appelle « Adresse »). | Choisir un thérapeute sans adresse : cases vides ; revenir à « (sélectionner…) » : tout se vide. | ✅ |
| PAT-105 | Suite de PAT-104 : enregistrer, rouvrir le suivi ; puis changer l'e-mail et l'adresse dans la fiche du thérapeute et rouvrir le suivi. | À la réouverture, e-mail et adresse **actuels** du thérapeute (comme le téléphone, PAT-83). | Le lieu enregistré du suivi reste la localité : `SELECT lieu FROM autres_suivis_patient WHERE id_autre_suivi_patient = …;` | ✅ |
| PAT-106 | Onglet Intervenants : un suivi terminé **hier**, un qui finit **aujourd'hui**, un sans date de fin. | Seul le premier est atténué (gris). | Comme PAT-81. | ✅ |
| PAT-107 | Thérapeute de test : vider le nom, code postal `12` (Belgique), e-mail `jean@exemple` ; Enregistrer. | Une fenêtre « Thérapeute » liste les 3 points ; icône sur chacun ; focus sur le Nom. | — | ✅ |
| PAT-108 | (Décision de Joëlle) Créer un thérapeute de test de **mêmes nom et prénom** qu'un autre ; « Non », puis à nouveau Enregistrer et « Oui ». Puis modifier ce nouveau thérapeute sans toucher au nom (ex. téléphone). | Question « Homonyme » ; « Non » : rien n'est enregistré, la fenêtre reste ouverte ; « Oui » : enregistré, les deux apparaissent dans la liste. La modification sans changement de nom ne repose **pas** la question. | Changer ensuite son prénom puis le remettre : la question revient au retour à l'homonymie. | ✅ |
| PAT-109 | Non-régression : rejouer PAT-26, PAT-27, PAT-41 à PAT-49 (dont PAT-46 : contact et thérapeute), PAT-83. | Mêmes résultats ; les refus s'affichent en plus dans la fenêtre récapitulative. | — | ✅ |

## Cas de test — Spécialité d'un thérapeute choisie dans un référentiel (v2.55.0, 23/10/2026)

Prérequis : migration v2.55.0 appliquée (plan Référentiels, section 21). Thérapeutes de test uniquement.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-110 | Référentiels › Thérapeutes : ouvrir chacun des thérapeutes existants. | La spécialité de chacun est **sélectionnée** dans la liste (reprise de la migration) ; colonne Spécialité de la grille inchangée. | Rien n'est marqué modifié à l'ouverture (fermer ne demande rien). | ✅ |
| PAT-111 | Thérapeute de test : bouton **[+]** à droite de la spécialité, ajouter « Orthodontiste », fermer le référentiel. | La liste est rechargée et « Orthodontiste » est sélectionnée ; Enregistrer, rouvrir : retrouvée. | `SELECT t.code_therapeute, s.libelle_specialite FROM therapeutes t LEFT JOIN ref_specialites s ON s.id_specialite = t.id_specialite;` | ✅ |
| PAT-112 | Désactiver « Orthodontiste » dans le référentiel ; rouvrir le thérapeute de PAT-111, puis un autre. Puis fiche Intervenant d'un patient de test : choisir ce thérapeute. Enfin, Nouvel intervenant › [+] (créer un thérapeute) : la fenêtre a sa liste et son [+]. | Chez lui, « Orthodontiste » reste affichée ; chez l'autre, elle n'est plus proposée. Intervenant : Spécialité « Orthodontiste » recopiée. Création depuis l'intervenant : liste et [+] actifs. | Choisir « — » enregistre sans spécialité. | ✅ |

## Cas de test — Codes postaux belges : code existant et localité correspondante (v2.56.0, 24/10/2026)

Prérequis : migration `migration_v2.56.0_codes_postaux_be.sql` appliquée (HeidiSQL), puis contrôle `SELECT COUNT(*), SUM(est_proposee) FROM ref_codes_postaux_be;` → **4083** et **1189**. Patients, contacts et thérapeutes de test ; pays **Belgique** sauf mention contraire. Règles : ADR-53.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-113 | Démarrer Althéa avant puis après la migration. | Avant : le contrôle de version de la base signale la migration manquante (2.56.0 attendue). Après : démarrage normal. | `tec_meta_schema` : ligne 2.56.0. | ✅ |
| PAT-114 | Nouveau patient : code **4000**, Tab ; puis effacer la localité, code **4280**, Tab ; dans Localité, taper « A ». | « Liège » remplie ; puis « Hannut » ; la frappe de « A » suggère Abolens, Avernas-le-Bauduin, Avin. | La localité n'est remplie que si elle est vide et que le code vient d'être changé. | ✅ |
| PAT-115 | Code **4999**, quitter le champ ; Enregistrer. | Icône d'erreur rouge « Ce code postal n'existe pas en Belgique. » ; à l'enregistrement, la fenêtre des erreurs le reprend, rien n'est enregistré. | Le journal ne contient pas le code saisi. | ✅ |
| PAT-116 | Code **4000**, localité « Namur », quitter le champ ; Enregistrer → « Corriger » ; Enregistrer → « Enregistrer quand même ». | Icône d'avertissement (jaune) sur Localité : « La localité « Namur » ne correspond pas au code postal 4000 (Liège, Glain, Luik, Lüttich…). » ; question à l'enregistrement ; « Corriger » ramène au champ sans rien enregistrer ; « Enregistrer quand même » enregistre. | Barre d'état « Enregistrement annulé : localité à corriger. » après « Corriger ». | ✅ |
| PAT-117 | Écritures acceptées sans avertissement : « 1000 Bruxelles », « 1000 Brussel », « 1020 Laeken », « 1050 Ixelles », « 4460 grace hollogne », « 1640 Rhode-St-Genèse », « 3800 St.-Truiden », « 7700 Mouscron », « 7090 Petit-Roeulx-lez-Braine ». | Aucune icône, aucune question. | Accents, majuscules, traits d'union, points, « œ » et « Saint / Sint / St » indifférents. | ✅ |
| PAT-118 | Patient avec « 4000 Liège » : changer le code en **5000**, Tab. | La localité « Liège » est gardée (pas remplacée) ; avertissement sur Localité. | — | ✅ |
| PAT-119 | Pays **France** : « 75001 Paris », puis « 75001 Namur » ; pays vide : « 4999 Truc » ; repasser en Belgique avec « 4000 Namur ». | France : aucune proposition, suggestion ni avertissement (format français seul) ; pays vide : aucun contrôle ; retour en Belgique : avertissement sur Localité. | — | ✅ |
| PAT-120 | Ouvrir en consultation une fiche dont la localité ne correspond pas au code ; passer par le champ Code postal. Puis « Modifier » une fiche dont le code belge n'existe pas, Enregistrer. | Consultation : l'avertissement s'affiche, rien n'est rempli ni modifié. Modification : enregistrement refusé tant que le code n'est pas corrigé. | Les adresses existantes ne sont pas modifiées par la migration. | ✅ |
| PAT-121 | Fenêtre **Contact** : code 4280 → Hannut ; « Copier l'adresse du patient » d'un patient « 4000 Namur » ; Enregistrer. | Hannut remplie ; après la copie, avertissement sur Localité ; question à l'enregistrement. | — | ✅ |
| PAT-122 | Fenêtre **Thérapeute** : homonyme d'un thérapeute existant avec « 4000 Namur » ; Enregistrer. | Question de l'homonyme, puis question de la localité ; « Corriger » ramène au champ Localité. | — | ✅ |
| PAT-123 | Recherche › Patients par lieu : deux patients de test à « 4470 St-Georges-sur-Meuse » et « 4470 Saint-Georges-sur-Meuse ». | Un seul lieu, 2 patients. | Non-régression R4 (RCH-40 à RCH-50). | ✅ |
| PAT-124 | Journal après avoir ouvert plusieurs fiches. | « Codes postaux belges : 1189 codes chargés. » une seule fois par session ; aucune adresse dans le journal. | — | ✅ |
| PAT-125 | Nouveau patient, code vide : dans Localité, taper « liè » ; cliquer « Liège · 4020 », Tab. Recommencer en tapant « lie » (sans accent) et en choisissant « Liege · 4030 ». | Suggestions « Liège · 4000 », « Liège · 4020 »… ; en quittant le champ : Localité « Liège », Code « 4020 ». Sans accent : les noms sont aussi proposés sans accents (avec Liedekerke, Lierde…) ; le choix redonne l'écriture officielle « Liège », code « 4030 ». | Parcourir la liste aux flèches ne remplit rien tant qu'on n'a pas quitté le champ. | ✅ |
| PAT-126 | Code vide : taper « Avin » (sans choisir dans la liste), Tab ; puis effacer les deux champs et taper « hannut ». | Code « 4280 » rempli dans les deux cas (une localité, un seul code ; accents et majuscules indifférents). | — | ✅ |
| PAT-127 | Code vide : taper « Liège » sans choisir, Tab ; aller dans Code, taper « 40 ». | Code laissé vide ; icône d'avertissement sur Code : « « Liège » a plusieurs codes postaux (4000, 4020, 4030, 4031, 4032) : choisissez le bon. » ; le champ Code suggère ces codes ; une fois le code choisi, l'icône disparaît. « Charleroi » : six codes cités puis « … ». | — | ✅ |
| PAT-128 | Code « 4000 » déjà saisi : dans Localité, taper « Gl ». | Suggestions des noms du code seulement (Glain), sans code ; rien ne change le code. | Le sens code → localité reste celui de PAT-114. | ✅ |
| PAT-129 | Fenêtres Contact et Thérapeute, onglet Cabinet (cabinet et praticien) : refaire PAT-125 et PAT-127 rapidement ; puis une fiche en consultation dont le code est vide. | Même comportement partout ; en consultation, rien n'est rempli (l'icône « plusieurs codes » peut s'afficher). | — | ✅ |

## Cas de test — Patients inactifs visibles et filtrables (relevé de Joëlle, 24/10/2026)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-130 | Écran Patients : liste « Patients » à droite du résumé ; choisir « Tous ». | Les patients inactifs apparaissent avec une pastille grise **Inactif** devant le nom, texte de la ligne grisé, pastilles atténuées ; plus de « (inactif) » après le prénom. Sélectionner la ligne : pastille et nom restent lisibles. | — | ✅ |
| PAT-131 | Choisir « Inactifs », puis « Actifs » ; avec « Actifs », chercher le nom d'un patient inactif. | « Inactifs » : seulement les patients inactifs ; « Actifs » (par défaut) : aucun inactif, barre d'état « — n patient(s) inactif(s) masqué(s) : choisir « Patients : Tous » ou « Inactifs » ». Le bouton « Réinitialiser » s'active hors « Actifs » et remet « Actifs ». | Résumé cliquable recalculé sur la liste choisie. | ✅ |
| PAT-132 | « Inactifs » : ouvrir la fiche d'un patient inactif, puis Retour. | La liste revient sur « Patients : Inactifs », avec la même recherche et la même page. | — | ✅ |

## Cas de test — Relevés aux captures (25/10/2026)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| PAT-133 | Écran Patients, « Tous » : cliquer sur la ligne d'un patient inactif. | Ligne sélectionnée lisible : texte gris moyen sur le fond de la sélection (il était presque blanc). | Une ligne active sélectionnée : inchangée. | ✅ |
| PAT-134 | Recherche › Patients par lieu (inactifs cochés) et Réseau par nom : sélectionner un patient inactif, un thérapeute désactivé, un suivi terminé. | Même gris lisible sur la sélection. Réseau par nom : colonne « Patients » entière, sans barre de défilement horizontale sous la liste des personnes. | Fenêtre à sa taille habituelle puis agrandie. | ✅ |
| PAT-135 | Fiche patient › Famille / Contacts et Intervenants : double-clic sur une ligne (consultation). | La fenêtre s'ouvre sans texte sélectionné ; Entrée ferme la fenêtre (focus sur « Fermer ») ; « Modifier » passe en édition comme avant. | Création et modification : comme avant. | ✅ |
| PAT-136 | Paramètres › Référentiels › Liens patient puis Rôles légaux. | Ligne « Par défaut » : case « Proposé pour un nouveau contact », libellé entier. | — | ✅ |

## Non-régression transversale

- Un patient inactif reste consultable et son historique (dossiers, séances, paiements) demeure intact.
- L’accès depuis la fiche Patient vers le dossier, puis le retour, conserve le patient courant.
- Les règles de tarifs et quotas ne sont pas validées dans ce plan : exécuter `PLAN_TESTS_TARIFICATION_QUOTAS.md` et `PLAN_TESTS_PARCOURS_SUIVI.md` après PAT-28.

## Nettoyage et validation finale

- Supprimer ou désactiver les données `TEST_PAT_` en respectant les contraintes relationnelles ; ne jamais supprimer les référentiels partagés.
PAT-01 à PAT-79 sont exécutés.
- [x] PAT-87 à PAT-98 (audit C.2 étape 2a : fiche patient jugée par le métier, NISS normalisé) — validés par Joëlle le 23/10/2026.
- [x] PAT-99 à PAT-109 (audit C.2 étape 2b : contacts, intervenants et thérapeutes jugés par le métier, homonyme de thérapeute confirmé) — validés par Joëlle le 23/10/2026.
- [x] PAT-113 à PAT-129 (codes postaux belges, v2.56.0 ; dans les deux sens) — validés par Joëlle le 24/10/2026.
- [x] PAT-130 à PAT-132 (patients inactifs visibles et filtrables) — validés par Joëlle le 24/10/2026.
- [x] PAT-133 à PAT-136 (relevés aux captures du 25/10/2026) — validés par Joëlle le 25/10/2026.
- [ ] Aucune anomalie bloquante sur l’unicité NISS, l’identité, l’activation ou la navigation.
- [ ] Les anomalies sont consignées avec preuve et reliées au plan spécialisé concerné.
