# 📌 **Althéa - CHANGELOG**

> *Dernière mise à jour : 25/10/2026*

## 📅 26/10/2026 — Audit D-3 : intégration continue sur GitHub

- **Décision de Joëlle** (ADR-59) : pas de vérification à chaque push (minutes gratuites limitées sur un dépôt privé), mais **une fois par jour, le soir**, s'il y a eu un commit dans les 24 heures, et **à la demande** (bouton « Run workflow » de l'onglet Actions). Joëlle continue de pousser chaque commit sur `master`.
- **`.github/workflows/ci.yml`** : machine Windows de GitHub, .NET 8 ; compilation de l'application et des tests en **Release** (vérifiée d'abord sur le poste : 621 tests), puis tests. Aucun secret ; droits en lecture seule ; le test de fumée s'y ignore (pas de base).
- ADR-59, stratégie de tests, environnement technique, structure de la solution, audit § 9, ToDo, état du projet, plan de tests du parcours (D3-01, D3-02).

## 📅 26/10/2026 — Audit D-2, étape 3 : test de fumée des requêtes — D-2 terminé

- **Base vide dédiée `althea_tests`** (décision de Joëlle), avec un compte MariaDB limité à cette base : script `Docs/Database/Maintenance/creer_base_tests.sql` (une fois par poste) ; chaîne de connexion dans la variable d'environnement `ALTHEA_TESTS_CONNEXION`, jamais dans le dépôt.
- **Test de fumée** : à chaque série, la structure est recréée depuis `backup_NoData_althea.sql` (vérifiée au passage), puis chaque requête des modules `Query*` est préparée par MariaDB, jamais exécutée. Les requêtes sont recensées automatiquement : une nouvelle requête est testée sans rien écrire. Garde-fou : toute autre base que `althea_tests` est refusée.
- **Résultat** : 389 requêtes acceptées ; une seule à déclarer comme modèle (`SelectPlafondsParQuotas`, repère `{IDS}` remplacé à l'exécution). 621 tests au vert ; sans base configurée, le test de fumée est simplement ignoré.
- **Constat pour l'installation** : la sauvegarde de structure cite la base `althea` dans les valeurs par défaut de 15 tables (séquences) — noté au ToDo.
- ADR-58, stratégie de tests, architecture de la base, ToDo, audit § 9, état du projet, plan de tests du parcours (D2-07, D2-08). **Point D-2 terminé.**

## 📅 26/10/2026 — Audit D-2, étape 2 : dates, heures et quota INAMI testés

- **Saisie des dates et heures** : les interprètes de `AltheaDate` et `AltheaHeure` (et, pour l'heure, la mise dans la journée et le « : » automatique) passent en `Friend`, visibles du seul projet de tests (`InternalsVisibleTo`).
- **Calcul du quota** (décision de Joëlle) : `GestionQuotaInami.CalculerQuota` reçoit la règle en vigueur par une fonction « règle à telle date » au lieu de la lire en base au milieu du calcul. Même calcul, même résultat ; l'onglet Séances, l'aperçu du patient et l'alerte de la fenêtre Séance lui passent la lecture habituelle.
- **75 nouveaux tests** (229 au total, tous au vert) : formats de date, année sur deux chiffres, 29 février ; saisies d'heure ; numérotation du quota par catégorie, renouvellement de date à date, période décrite, règle fixée à l'ouverture, plafonds.
- ADR-58, ADR-27 (traçabilité), concepts métier, stratégie de tests, plan de tests du parcours (D2-04 à D2-06, validés le 26/10/2026).

## 📅 26/10/2026 — Audit D-2, étape 1 : premiers tests automatisés

- **Décisions de Joëlle** (ADR-58) : projet de tests en VB.NET, xUnit v3 ; fonctions privées ouvertes aux seuls tests (`Friend` + `InternalsVisibleTo`) ; calcul du quota alimenté par une fonction « règle de telle date » ; test de fumée des requêtes sur une base vide dédiée `althea_tests`.
- **Projet `Tests/Althea.Tests`** (dans `Althea.slnx`, exclu de la compilation de l'application) et **`Tools\test.cmd`** (journal `obj\_tests.log`).
- **154 tests**, tous au vert : identifiants (NISS belge, français, luxembourgeois ; INAMI, BCE, TVA, IBAN, BIC, code titulaire), sécurité (politique de mot de passe ; empreinte PBKDF2 figée), saisie (téléphone, code postal, localités, comparaison de textes). Données fictives ; clés de contrôle et empreinte attendues calculées hors d'Althéa.
- Règle `Rules.md` § 3 ; ADR-58, stratégie de tests, structure de la solution, environnement technique (paquets du projet de tests), plan de tests du parcours (D2-01 à D2-03, validés le 26/10/2026).

## 📅 25/10/2026 — Audit D-1, étape 2 : .editorconfig et analyseurs .NET — D-1 terminé

- **`.editorconfig` à la racine** : il reprend les réglages de Visual Studio de Joëlle, exportés en juin dans `My Project/.editorconfig`, où il ne servait à rien (il ne s'appliquait qu'à ce dossier et le projet l'excluait). Ajout : `.vb` en UTF-8 avec BOM (un fichier sans BOM en reçoit un à son prochain enregistrement) ; indentation de 2 pour les fichiers XML du projet. Volontairement absents : fin de ligne (`.gitattributes` s'en charge), suppression des espaces de fin de ligne, saut de ligne final.
- **Analyseurs .NET** (jeu « Recommended ») : inventaire de 545 avertissements sur 26 règles ; 3 règles écartées, contraires aux conventions du projet (CA1707, CA1305, CA1725) ; les 23 autres au niveau « suggestion », visibles dans l'éditeur sans encombrer la compilation ; toute nouvelle règle qui se déclencherait sort en avertissement, jamais bloquant.
- `NeutralLanguage` = `fr` (CA1824). Compilation : 0 erreur, 0 avertissement.
- ADR-57, Rules § 7, environnement technique, structure de la solution, ToDo (cas des analyseurs à corriger en priorité), plan de tests du parcours (D1-09, D1-10, validés le 25/10/2026). **Point D-1 terminé.**

## 📅 25/10/2026 — Audit D-1, étape 1 : Option Strict On pour tout le projet

- **Décisions de Joëlle** (ADR-57) : inventaire par une compilation d'essai avant de corriger ; `Option Strict On` dans `Althea.vbproj` ; les lignes `Option` restent en tête de chaque fichier ; `.editorconfig` (`utf-8-bom` pour les `.vb`) et analyseurs .NET (niveau suggestion) à l'étape suivante.
- **Inventaire** : 12 erreurs seulement, aucune dans les Designer. Dans 11 référentiels et `GestionParametres`, l'identifiant (`BIGINT UNSIGNED`, propriété `ULong`) était lu par `GetInt64` / `GetInt32` : il est lu par `GetUInt64`.
- `Option Strict On` / `Option Explicit On` en tête des 38 fichiers qui ne les avaient pas (référentiels, sécurité, cabinet, paramètres, paiements, navigation, utilitaires).
- **Avertissements du compilateur** corrigés : `UC_DomainesHome.EnregistrerLogo` (retour explicite après un refus de droit, comportement inchangé) ; `UtilsButtons.GetStandardButtonAssetFolder` (dossier 32 px par défaut, comme annoncé, au lieu de `Nothing`).
- `Tools\build.cmd` V1.1.0 : transmet ses options à `dotnet build` (ex. `-p:OptionStrict=On --no-incremental`).
- Aucun changement de comportement attendu. ADR-57, Rules § 7 et § 22, environnement technique, plan de tests du parcours (D1-01 à D1-08, validés le 25/10/2026).

## 📅 25/10/2026 — Grands documents scindés (audit lot D, point 4)

- **Décisions de Joëlle** : scinder la doc UI (≈ 8 900 lignes), les processus (≈ 3 250) et le CHANGELOG (≈ 4 500) ; la doc UI par domaine ; le registre des ADR reste d'un seul tenant.
- **Documentation technique UI** : `Documentation_technique_UI_Althea.md` devient le sommaire (organisation, vue d'ensemble des composants avec la partie de chacun) ; les fiches sont dans `Docs/Architecture/UI/` : `UI_Socle`, `UI_Administration`, `UI_Referentiels_Domaines`, `UI_Patients_Dossiers`, `UI_Recherche`. Fiche ajoutée au socle : le contrat `IFenetrePrincipale` (ADR-55).
- **Processus** : `Process_Althea.md` devient le sommaire (processus 01 à 24) ; les processus sont dans `Docs/Architecture/Processus/` : socle technique (01–09), métier (10–20), administration et recherche (21–24).
- **CHANGELOG** : seul le mois en cours reste ici ; mars à septembre 2026 sont dans `Docs/Archives/CHANGELOG_2026.md`, sans réécriture.
- Contenu déplacé à l'identique (aucune ligne perdue, vérifié par comparaison) ; liens et images recalés sur leur nouvel emplacement ; liens ciblés des autres documents redirigés vers la bonne partie.
- Règle `Rules.md` § 13 (où ranger une nouvelle fiche ou un nouveau processus, archive mensuelle du CHANGELOG) ; bibliothèque de documentation, rapport d'audit, ToDo, état du projet.

## 📅 25/10/2026 — Audit C.5 : les deux grosses fiches réparties par onglet

- **Décisions de Joëlle** (ADR-56) : un fichier partiel (`Partial Class`) par onglet lourd, et seulement pour les deux fiches ; les autres gros fichiers, d'un seul tenant, restent tels quels.
- **Fiche dossier** : `UC_DossierFiche.ZonesCliniques.vb`, `.Seances.vb`, `.Paiements.vb` (fichier principal : 1 997 lignes au lieu de 4 266). **Fiche patient** : `UC_PatientFiche.Famille.vb`, `.Intervenants.vb`, `.Dossiers.vb` (2 739 au lieu de 4 073).
- Code déplacé à l'identique (comparaison avec la version précédente) ; fichiers rangés sous leur fiche (`Althea.vbproj`). Aucun changement de comportement.
- Règle `Rules.md` § 3 ; ADR-56, structure de la solution, doc de l'interface, plan de tests du parcours (C5-01 à C5-04, validés le 25/10/2026). Lot C.5 terminé.

## 📅 25/10/2026 — Audit C.4, étape 3 : recherches, hubs et élévation d'accès réunie

- **Recherches et hubs** : la fenêtre principale est lue dans le contexte (`IFenetrePrincipale`) ; dans les hubs Administration et Référentiels, les 27 blocs « retrouver Home, puis naviguer » deviennent une ligne (`Ouvrir`). Plus aucun écran ni fenêtre ne connaît la classe `Home` (sauf l'outil de capture, Debug).
- **Élévation d'accès réunie** (décision de Joëlle) : écrite six fois (quatre dans `Home`, une par hub), elle ne s'ouvre plus qu'à un endroit (`Home.OuvrirFenetreElevation`) ; les hubs la demandent (`DemanderElevation`). La fenêtre d'élévation reçoit partout le contexte (infobulles) ; le journal des hubs, qui doublait celui de la fenêtre, est retiré. Elle n'écrit plus « Administration > Élévation d'accès » dans le fil d'Ariane (faux depuis les Référentiels, et resté affiché après une élévation demandée depuis une fiche) : c'est l'appelant qui le pose.
- **Garde « modifications non enregistrées »** vérifiée : rien à changer.
- **Règle** (`Rules.md` § 9) : jamais la classe `Home` dans un écran ou une fenêtre ; élévation uniquement par la fenêtre principale.
- ADR-55, sécurité, doc de l'interface, plan de tests Home & navigation (NAV-C4-11 à 15, validés le 25/10/2026). Lot C.4 terminé.

## 📅 25/10/2026 — Audit C.4, étape 2 : les écrans lisent la fenêtre principale dans leur contexte

- Écran Patients, fiche patient, dossier, Domaines, Cabinet, Utilisateurs et référentiel Thérapeutes : `_context.FenetrePrincipale` (contrat `IFenetrePrincipale`, ADR-55) au lieu de `TryCast(FindForm(), Home)` — ouverture des fiches et du dossier, Retour, référentiels en fenêtre, fenêtres Contact / Intervenant / Thérapeute / Séance / Paiement, bandeau (cabinet, photo).
- Aucun changement visible. Plan de tests Home & navigation : NAV-C4-06 à 10 (validés le 25/10/2026).
- **Domaines, étapes Tarifs et Natures** (relevé de Joëlle au test) : l'action « Créer un nouveau tarif… » / « Créer une nouvelle nature… », à droite de la barre, n'apparaissait pas — sa place était calculée avant l'affichage de l'étape, puis l'ancrage à droite la repoussait hors de la barre (`UC_LiaisonsPrixDomaines` V2.3.1, `UC_LiaisonsNaturesDomaines` V2.2.1).

## 📅 25/10/2026 — Audit C.4, étape 1 : la fenêtre principale par un contrat

- **Décisions de Joëlle** (ADR-55) : un contrat `IFenetrePrincipale` que `Home` implémente, trouvé par les écrans dans leur contexte (`UserControlContext.FenetrePrincipale`) et reçu en paramètre par les fenêtres ; l'élévation d'accès, écrite six fois, sera réunie (étape 3).
- **Étape 1** : le contrat (naviguer, revenir, référentiel en fenêtre, création d'un thérapeute, fil d'Ariane, bandeau) ; les fenêtres Contact, Intervenant, Thérapeute, Séance et Paiement reçoivent la fenêtre principale par ce contrat au lieu de `Home` (boutons [+]).
- Aucun changement visible. ADR-55, architecture technique, structure de la solution, doc de l'interface, plan de tests Home & navigation (section 9, NAV-C4-01 à 05 validés le 25/10/2026).

## 📅 25/10/2026 — Lignes système devenues inutiles déverrouillées (v2.58.0)

- **Décision de Joëlle** (24/10) : depuis la v2.57.0, ces lignes ne sont plus cherchées par leur code ; leur verrou n'a plus de raison d'être.
- **Déverrouillées** (code modifiable, désactivation possible) : statuts de séance Absence patient (`ABSENTE`), Annulée (patient), Annulée (praticien), Maladie (et `NO_SHOW` s'il existe) ; lien Mère ; rôle légal Autorité parentale. Leurs cases (absence, non tenue, par défaut) ne changent pas.
- **Restent verrouillés** : Planifiée, Réalisée, Reportée, statuts de dossier, types de document, mode Avoir, tarif INAMI.
- Migration de données seulement (pas de changement de structure ni de code). ADR-54, base de données, référentiels, doc de l'interface, plan de tests du parcours (C3-17 à C3-19, validés le 25/10/2026).

## 📅 25/10/2026 — Illustrations refaites et relevés aux captures

- **Illustrations** (`Docs/Illustrations/Doc_UI`) : 15 captures refaites après les changements du 24/10 (codes visibles, pastille « Inactif », recherches, cases des référentiels) ; nouvelles : Patients par lieu, Réseau par nom ; téléphone et e-mail du praticien floutés (décision de Joëlle).
- **Ligne inactive sélectionnée lisible** : écran Patients, Patients par lieu et Réseau par nom écrivaient le texte d'un patient inactif (ou d'un thérapeute désactivé) sélectionné en gris très clair, illisible sur le fond de la sélection ; même gris que les autres grilles (`ColorGrisDiscret`).
- **Liens patient, rôles légaux** : la case « par défaut » devient « Proposé pour un nouveau contact » (libellé coupé dans le panneau Détail).
- **Fenêtres Contact et Intervenant en consultation** : focus sur « Fermer », plus de texte sélectionné dans le premier champ.
- **Réseau par nom** : liste des personnes élargie ; la colonne « Patients » n'est plus coupée (barre de défilement horizontale).
- Plan de tests Patients : PAT-133 à PAT-136 (validés le 25/10/2026).

## 📅 24/10/2026 — Audit C.3, étape 2 : les règles lues dans les référentiels (v2.57.0)

- **Décision de Joëlle** (ADR-54) : une règle qui peut se dire par une case du référentiel y est lue plutôt que tirée d'un code. Question de Joëlle : un statut ajouté (TEST_C1, mêmes cases que Maladie) se comporte-t-il comme Maladie, jusqu'à libérer le créneau ? Désormais oui, par sa case.
- **Statuts de séance** (migration v2.57.0) :
  - « **Non tenue, libère le créneau** » (`est_non_tenue`) remplace les listes de codes qui ne concordaient pas : pas de contrôle de chevauchement, créneau libre au Planning du jour ; cochée au départ pour Annulée patient, Annulée praticien, Reportée et **Maladie** (qui occupait jusqu'ici son créneau) ;
  - une **absence non excusée** garde son créneau ; passer d'un statut non tenu à un autre (annulée → absence) reste sans contrôle ;
  - « **Proposé à la création d'une séance** » (`propose_creation`) : Planifiée (obligatoire, refus sinon) et Réalisée au départ.
- **Liens patient et rôles légaux** : case « **Proposé par défaut pour un nouveau contact** » (`par_defaut`, une seule à la fois) ; la fenêtre Contact présélectionne ces valeurs au lieu de « Mère » et « Autorité parentale » écrites dans le code.
- Valeurs de départ posées en l'absence de Joëlle, modifiables dans Paramètres › Référentiels. `CodesSysteme` perd les codes devenus inutiles (Absente, Malade, Annulées).
- ADR-54, base de données, paramètres et référentiels, documentation de l'interface, rapport d'audit, plan de tests du parcours (C3-07 à C3-16, validés le 24/10/2026). Constat au test : le Planning du jour d'une séance créée sans quitter le dossier montre encore la séance libérée (affichage seulement, noté au ToDo).

## 📅 24/10/2026 — Écran Patients : patients inactifs visibles et filtrables

- **Relevé de Joëlle** pendant les tests : rien ne distinguait un patient inactif (texte à peine plus gris, mention « (inactif) » coupée par la largeur de la colonne Prénom), et l'on ne pouvait pas n'afficher qu'eux.
- **Décisions de Joëlle** : une pastille grise **Inactif** devant le nom (comme « Désactivé » dans les Utilisateurs), et la case « Afficher les inactifs » remplacée par la liste **Patients : Actifs / Inactifs / Tous** (Actifs par défaut, retenue au retour de la fiche).
- Nouveau `UtilsDataGrid.DessinerPastilleAvantTexte` (pastille puis texte de la cellule). Plan de tests Patients : PAT-130 à PAT-132.
- **Recherches** (relevé de Joëlle au test) : la même pastille devant le patient inactif dans **Patients par lieu** et dans les liens du **Réseau par nom** (RCH-72, RCH-73).

## 📅 24/10/2026 — Audit C.3, étape 1 : les codes système en un seul endroit

- **Décisions de Joëlle** (ADR-54) : un point unique `CodesSysteme` ; dans le SQL, le texte est bâti sur ces constantes ; ensuite (étape 2) des cases du référentiel à la place des règles tirées d'un code.
- **`CodesSysteme`** (`Metier/Referentiels`) : statuts de séance, de dossier et de paiement, types et sources de document. Les constantes `CODE_STATUT_*` de `GestionSeances` et `GestionDossiers` y ont déménagé ; les statuts de paiement, qui n'en avaient pas, en ont une.
- **Plus de code en clair** : environ trente `'PAYE'`, `'EN_ATTENTE'`, `'ANNULE'`, `'CLOTURE'`, `'ARCHIVE'`, `'ACTIF'` dans `QueryPaiements`, `QuerySeances` et `QueryPatients` ; `"ANAMNESE"`, `"ZONE_CLINIQUE"`, `"PHOTO_IDENTITE"`, `"LOCAL_DOCX"`, `"PDF"` dans la fiche patient et la zone du dossier ; `"PAYE"`, `"ANNULE"` dans `PaiementListeItem`.
- **Code mort retiré** : `Seance.EstRealisee`, `EstPlanifiee` et `EstAnnulee` (l'une des trois définitions divergentes de la séance « non tenue »).
- Sans migration ni changement de comportement. ADR-54, règle « lignes système » de `Rules.md`, rapport d'audit, plan de tests du parcours (C3-01 à C3-06).

## 📅 24/10/2026 — Codes postaux belges : code existant et localité correspondante (v2.56.0)

- **Demande de Joëlle** (23/10) ; **décisions du 24/10** : liste bpost complétée par le registre officiel des adresses (BeST, SPF BOSA) ; localité remplie et suggérée ; localité hors du code signalée puis confirmée ; mise à jour par un script de maintenance (ADR-53).
- **Référentiel** `ref_codes_postaux_be` (migration v2.56.0) : 4 083 noms pour 1 189 codes — « Bruxelles » et « Brussel », « Mouscron » et « Moeskroen », les sections (« 4280 Avin »), « Laeken » ajouté à la main ; un nom proposé par code (la commune principale, dans la langue de sa région). Bpost seule donnait les noms bruxellois et même « Moeskroen » en néerlandais.
- **Adresse en Belgique** (fiche patient, contact, thérapeute, cabinet, praticien) :
  - un code inexistant est refusé (« Ce code postal n'existe pas en Belgique. ») ;
  - en quittant le code, une localité vide est remplie (« 4000 » → « Liège ») ; pendant la frappe, le champ Localité suggère les noms du code ;
  - dans l'autre sens, le plus courant (relevé de Joëlle) : code vide, le champ Localité suggère « Liège · 4000 », « Liège · 4020 »… et un choix remplit les deux champs ; une localité à un seul code le remplit (« Avin » → 4280) ; à plusieurs codes, une icône sur le champ Code les cite et le champ les suggère ;
  - une localité qui ne correspond à aucun nom du code reçoit une icône d'avertissement, puis la question « Enregistrer quand même / Corriger » à l'enregistrement — pas un refus.
- **Comparaison des noms** : accents, majuscules, traits d'union, points, « œ » et « Saint / Sint / St » indifférents (`UtilsCodePostal.CleLocalite`) ; la recherche des patients par lieu en profite (« St-Georges » et « Saint-Georges » réunis).
- **Nouveaux** : `GestionCodesPostauxBe` (règles, liste lue une fois par session), `QueryCodesPostauxBe`, `SaisieLocaliteBelge` (aide de saisie partagée par les cinq adresses) ; script `Docs/Database/Maintenance/codes_postaux_be/generer_codes_postaux_be.py` (procédure dans `Database_technique.md`).
- Autres pays : seul le format reste contrôlé. Les adresses déjà saisies ne sont pas modifiées : elles sont contrôlées à leur prochaine modification.
- ADR-53, base de données, paramètres et référentiels, utilitaires transverses, documentation de l'interface, plans de tests Patients (PAT-113 à PAT-129) et Cabinet (4.15 à 4.18).

## 📅 24/10/2026 — Le code toujours visible : règle et tour de l'application

- **Demande de Joëlle** : noter qu'en ouvrant ou modifiant une fiche, et dans les grilles, le code doit toujours apparaître, et vérifier que c'est le cas partout.
- **Règle** (`Rules.md` § 15, `Reference_UI.md` § 5.6) : la règle « nommer par le code » du 17/10 devient « le code toujours visible » — titre ou sous-titre des fenêtres sur un élément existant, colonne Code visible dans les grilles d'éléments codés, rappels et listes de choix, annonces d'enregistrement ; exception : l'aperçu compact du patient (code dans l'infobulle).
- **Listes où l'on choisit par le nom** (décision de Joëlle) : le code suit le nom (« Marie DUPONT (Logopède) · TH000003 ») pour que la frappe d'une lettre saute encore au bon nom.
- **Écarts corrigés** : colonne Code dans le Planning du jour (fenêtre élargie), dans la grille des utilisateurs (colonne qui existait masquée) et dans la liste des personnes du réseau (TH…) ; fenêtre Utilisateur nommée par le code (« Modifier l'utilisateur USR000003 », barre d'état par le code et non plus l'identifiant ; code cité à la création) ; liste des thérapeutes de la fenêtre Intervenant et des comptes à lier au praticien avec le code ; « Compte lié : USR000002 — … » dans l'onglet Cabinet.
- **`AltheaListe`** : la liste ouverte prend la largeur de son plus long élément, pour lire le code en entier au moment du choix.
- Vérifiés sans changement : patients, dossiers, séances, paiements (grilles, fenêtres, choix de l'avoir), contacts, intervenants, thérapeutes, recherches.

## 📅 24/10/2026 — Recherche transversale (R5) : le réseau par nom

- **Décisions de Joëlle** : une ligne par personne ; double-clic → le patient, et un bouton « Fiche de la personne » ; suivis terminés inclus et signalés, patients inactifs comme dans l'écran Patients.
- **Tuile « Réseau par nom »** dans le hub Recherche → nouvel écran (`UC_RechercheReseau`) : chaque mot tapé est le début d'un mot du nom ou du prénom (« dup », « dupont ma »), sans tenir compte des accents ni des majuscules ; à gauche les personnes (Thérapeute, Intervenant, Contact ; spécialité ou lien ; nombre de patients), à droite leurs patients (lien ou rôle, depuis, « en cours » / « terminé le … », fiche FC… / AS…).
- **Une personne** : un thérapeute du référentiel (ses suivis, même sans patient) ; les intervenants saisis à la main de même nom ; les contacts de même nom et prénom (la mère de deux enfants suivis) ; un intervenant choisi dans le référentiel compte sous son thérapeute.
- **Aller** : double-clic sur un patient → sa fiche, onglet Famille / Contacts ou Intervenants, Retour → la liste avec ses critères, la personne et le lien resélectionnés ; « Fiche de la personne » → le contact ou l'intervenant en consultation, ou la fenêtre du thérapeute ; la liste est relue après un enregistrement.
- **Factorisation** : comparaison sans accents ni majuscules dans `Utils/Helpers/UtilsTexte.vb`, reprise par `UtilsCodePostal.CleLocalite` (R4) ; réseau chargé une fois, recherche en mémoire, le journal ne note que le nombre de personnes.
- **Codes visibles** (relevé de Joëlle au test, comme pour les paiements le 23/10) : colonne « Code » dans les grilles des contacts (FC…) et des intervenants (AS…) de la fiche patient et du référentiel Thérapeutes (TH…, colonne qui existait masquée) ; code au titre des fenêtres Contact, Intervenant et Thérapeute (« Modifier le contact — FC000004 ») et dans l'annonce de l'enregistrement (« Contact FC000012 créé. »).
- **Code TH… dans la recherche par code** (décision de Joëlle) : il ouvre le réseau par nom sur ce thérapeute et ses patients ; aide du hub raccourcie pour tenir sur une ligne.
- ADR-52, processus 24, guide Recherche, documentation de l'interface, utilitaires transverses, plan de tests (RCH-51 à RCH-66, validés le 24/10/2026).

## 📅 24/10/2026 — Recherche transversale (R4) : les patients par lieu

- **Décisions de Joëlle** : une zone « code postal ou localité », la liste des **lieux** avec leur nombre de patients et les **patients** du lieu choisi ; inactifs comme dans l'écran Patients.
- **Tuile « Patients par lieu »** dans le hub Recherche → nouvel écran (`UC_RecherchePatientsLieu`) : un mot en chiffres cherche le début du code postal (« 43 » → 4300, 4317…), un autre mot une partie de la localité, sans tenir compte des accents ni des majuscules ; « Tous les lieux trouvés » en tête, « Sans adresse » en dernier ; patients avec code postal, localité, suivi, prochain rendez-vous, téléphone ; double-clic → la fiche, Retour → la liste avec son texte, sa case, le lieu et le patient resélectionnés.
- **Un seul lieu** pour deux écritures d'une même localité (« Faimes », « faimes ») ; une adresse sans pays est du pays du cabinet, qui n'est pas répété (« 75001 Paris (FR) » pour un autre pays).
- **Réutilisation** : la liste des patients de l'écran Patients porte désormais code postal, localité et pays (`QueryPatients`, `PatientListeItem`) ; comparaison des codes postaux et des localités dans `UtilsCodePostal` (servira à la validation code postal / localité) ; patients chargés une fois, recherche en mémoire, le journal ne note que le nombre de patients.
- **Données de test** : `Docs/Database/Maintenance/donnees_test_codes_postaux.sql` remplace, sur la base de développement seulement et de façon définitive, les codes postaux et localités fantaisistes des patients, contacts et thérapeutes par de vrais couples belges (demande de Joëlle).
- ADR-52, processus 24, guide Recherche, documentation de l'interface, plan de tests (RCH-40 à RCH-50).

## 📅 24/10/2026 — Outil de capture d'écran (Debug) ; illustrations de la documentation

- **Demande de Joëlle** : produire des illustrations nettes et fidèles (une capture faite depuis l'extérieur de l'application était floue, aux couleurs faussées).
- **Nouveau** (`UI/Context/FiltreCaptureEcran.vb`, compilé en Debug seulement, installé par `Home` V1.20.0) : **Ctrl+Maj+F8** capture le contenu (l'écran affiché dans Home ou la fenêtre active), **Ctrl+Maj+F7** la fenêtre entière ; PNG à la taille réelle dans `Docs/Illustrations/Doc_UI/_captures/` (ignoré par git), captures d'un même écran numérotées `_2`, `_3`… ; la fenêtre se dessine elle-même (`PrintWindow`), même recouverte.
- **Illustrations** (base de test) : 89 images faites ou refaites dans `Docs/Illustrations/Doc_UI` — tous les écrans, onglets et fenêtres de `Documentation_technique_UI_Althea.md` (Paramètres et État du système en plusieurs parties, variantes de `DialogChoix`) ; les liens vers les anciennes images supprimées le 16/10 (`Docs/Illustrations/*.png`) pointent sur les nouvelles ; emplacements « Illustration à ajouter » remplis ; guide Recherche (`Docs/Illustrations/Guide_Recherche/`) et guide de l'éditeur illustrés.
- **Complétées par Joëlle** : `Login`, `ElevationAcces`, `DialogUtilisationAvoir`, `DialogComptePraticien`, `UC_ApercuPatient`, vue SuperUser du hub Administration, `UC_Accueil`. Restent : vue User du hub Administration, 6 images de la Charte graphique.
- **Confidentialité** : l'IBAN du cabinet est flouté sur la capture de l'onglet Cabinet (décision de Joëlle).
- **`.gitignore`** : bloc « FodyWeavers / .tmp.driveupload » réparé (un passage enregistré en UTF-16 faisait traiter le fichier comme binaire).

## 📅 24/10/2026 — Documentation : lisibilité

- **Demande de Joëlle** : les lignes « Dernière mise à jour » à rallonge et les longs paragraphes compacts rendaient plusieurs documents quasi illisibles.
- **Mise à jour** : dans 27 documents, une ligne courte (dernière mise à jour) et un bloc repliable « Historique des mises à jour » (une date par puce, sous-puces pour les entrées longues) — décision de Joëlle ; règle ajoutée à `Rules.md` § 13.
- **Paragraphes** : ≈ 380 passages restructurés en listes, sous-listes et paragraphes courts dans 38 documents (pilotage, architecture, documentation de l'interface, ADR, rapports d'audit, règles, guides, plans de tests, README), sans changer le texte — contrôle automatique mot à mot sur chaque passage. Tableaux, blocs de code et CHANGELOG inchangés.
- **Process_Althea** : flowcharts des processus 23 (État du système) et 24 (Recherche transversale) ; processus 23 : le contrôle de la version de la base au démarrage, annoncé comme « étape ultérieure », existe depuis le 20/10.

## 📅 23/10/2026 — Clé Syncfusion renouvelée ; captures de la documentation de l'interface

- **Sécurité** : nouvelle clé de licence Syncfusion mise en place par Joëlle (les anciennes restent lisibles dans l'historique du miroir public, à purger) — ToDo, checklist, état du projet (DT-15), rapport d'audit § 9.
- **Documentation** : les captures de `Documentation_technique_UI_Althea.md` (`Docs/Illustrations/Doc_UI/`, 19 images) sont désormais suivies dans le dépôt (décision de Joëlle).

## 📅 23/10/2026 — Documentation : revue complète

- **Mis à jour** d'après le code et les entrées du 17 au 23/10/2026 : état du projet (modules, décisions D-34 à D-44, dettes DT-11 à DT-16, prochaines étapes), checklist V1, planning, architecture technique (briques transverses du métier), base de données (remarques de `ref_civilites` remises à leur place, groupes de paramètres, lecteurs de `v_seances_solde`), concepts métier (recherche transversale, règles de saisie ADR-51), sécurité (confidentialité de la recherche), utilitaires transverses, environnement technique, règles (droits côté métier, réinitialisation et déverrouillage : Admin), charte graphique (icônes du menu `_normal` / `_selected` / `_disabled`, icônes des tuiles Recherche), README, index de la documentation, stratégie de tests, plan Home et navigation (menu avec Recherche, mini-pile), guides Séances et paiements et Domaines, rapport d'audit § 9 (parenthèse « recherche » et suite du plan).
- **Documentation technique de l'interface** (`Documentation_technique_UI_Althea.md`, à jour au 20/10 jusqu'ici, désormais commitée avec le reste, décision de Joëlle) : changements du 21 au 23/10 reportés dans les sections existantes (Home, fiches patient et dossier, Domaines, Thérapeutes, fenêtres Contact, Intervenant, Thérapeute, Utilisateur, mot de passe, élévation, Séance, Paiement, dialogues, référentiels) ; nouvelles sections : 1.3 Recherche transversale (`UC_RechercheHome`, `UC_RechercheSeances`, `UC_RecherchePaiements`), `UC_Lateralites`, `UC_Specialites`, `DialogComptePraticien`, fenêtres de test, `SuiviActivite`, `IUnsavedChangesAware`, `ResumeFiltre`, `FiltresSeances` / `FiltresPaiements`, `SelecteurPeriode`, `OuvertureResultat` ; emplacements des illustrations à faire (« Illustration à ajouter : `Docs/Illustrations/Doc_UI/…` »).
- **Nouveau** : `Docs/Guide/Guide_Utilisateur_Recherche.md` (recherche par code, séances et paiements d'une période, retour, confidentialité), avec les emplacements des illustrations à faire (`Docs/Illustrations/Guide_Recherche/`).
- **Relevé au passage** (ToDo, section E) : la fiche Utilisateur active « Réinitialiser le mot de passe » pour un SuperUser, que le métier refuse.

## 📅 23/10/2026 — Recherche transversale (R3) : les paiements d'un jour ou d'une période

- **Décisions de Joëlle** : la liste est le **relevé de l'onglet Paiements du dossier**, tous patients — encaissements à leur date, paiements attendus à la date de leur séance, annulations — et elle donne les **totaux encaissés par mode** sous la grille.
- **Tuile « Paiements »** dans le hub Recherche → nouvel écran : date, patient, mouvement, pour (séance), mode, montant, pastille État (les mêmes que dans le dossier, infobulle comprise), codes du paiement et du dossier ; le plus récent en haut.
- **Période** comme pour les séances (dates, Aujourd'hui, Cette semaine, Ce mois, ‹ ›). **Affiner** : filtre et résumé cliquable du dossier (Encaissé, En attente), liste **Mode** (tous les modes ou un mode du référentiel), zone de texte (patient, code, séance, mode, référence).
- **Totaux** : « Encaissé par mode : Espèces … € · Virement … € — total … € » sur la période et le mode choisi ; un règlement par avoir n'est pas de l'argent reçu : affiché à part, hors total.
- **Aller au paiement** : double-clic → le dossier, onglet Paiements, le paiement sélectionné ; Retour ramène à la liste avec ses critères et la ligne resélectionnée. Plafond 1000 lignes ; le journal ne reçoit que le nombre de résultats.
- **Partagé sans changement de comportement** : relevé, axes du filtre, filtrage et résumé des paiements sortis de l'onglet Paiements du dossier dans `FiltresPaiements` (nouveau) ; infobulle d'une ligne dans `PaiementListeItem.InfobulleLigne` ; une seule requête pour les listes de paiements (`QueryPaiements.SelectPaiementsBase` : dossier, séance, période) ; règle « séance passée » dans `SeanceListeItem.EstMomentPasse` ; période des recherches dans `SelecteurPeriode` (nouveau, aussi pour la recherche des séances).
- Nouveaux : `UC_RecherchePaiements`, `FiltresPaiements`, `SelecteurPeriode` ; icônes `recherchePaiements_*` (provisoires). `QueryPaiements` V1.12.0, `GestionPaiements` V1.16.0 (`GetPaiementsParPeriode`), `PaiementListeItem` V1.9.0 (patient et dossier de la ligne), `SeanceListeItem` V1.10.0, `GestionRecherche` V1.2.0 (`RechercherPaiements`, `TotauxEncaissesParMode`, `ResultatDePaiement`), `UC_RechercheSeances` V1.1.0, `UC_RechercheHome` V1.2.0, `UC_DossierFiche` V1.23.0. Pas de changement de base de données.
- **Documentation** : ADR-52, interface, structure de la solution, processus 24, Reference_UI, ToDo, plan de tests Recherche (RCH-28 à RCH-39).

## 📅 23/10/2026 — Recherche transversale (R2) : les séances d'un jour ou d'une période

- **Tuile « Séances »** dans le hub Recherche : un nouvel écran liste les séances d'un jour ou d'une période, **tous patients confondus**, dans l'ordre du calendrier — date (avec le jour), horaires, patient, statut et paiement en pastilles (les mêmes que dans le dossier), nature, domaine, codes de la séance et du dossier.
- **Période** : dates « du … au … » (jamais inversées), boutons Aujourd'hui, Cette semaine, Ce mois, et **‹ ›** pour la période précédente ou suivante (un mois entier passe au mois voisin entier, une autre période glisse de sa durée).
- **Affiner** : la liste **Filtre** et le **résumé cliquable** de l'onglet Séances du dossier (à venir, à confirmer, réalisées, absences, non dues ; à régler, prévu, avoir) — par exemple les impayés du mois, tous patients ; une zone de texte (patient, code, nature, domaine).
- **Aller à la séance** : double-clic, Entrée ou « Ouvrir » → le dossier, onglet Séances, séance sélectionnée ; « Ouvrir le patient » → sa fiche. **Retour** ramène à la liste avec sa période, son filtre et la séance resélectionnée ; « Retour à la recherche » ramène au hub.
- **Plafond** : 1000 séances (au-delà, « réduisez la période »). Le journal ne reçoit que le nombre de résultats, jamais la période.
- **Partagé sans changement de comportement** : axes du filtre, filtrage et résumé des séances sortis de l'onglet Séances du dossier dans `FiltresSeances` (nouveau, `UI/Controls/Communs`) ; correction des bornes d'une période dans `AltheaDate.CorrigerBornes` ; colonnes du planning du jour et de la nouvelle recherche dans une seule requête (`QuerySeances.SelectSeancesPlanning`).
- Nouveaux : `UC_RechercheSeances`, `FiltresSeances` ; icônes `rechercheSeances_*` (provisoires). `QuerySeances` V1.5.0 (`SelectSeancesParPeriode`), `GestionSeances` V1.16.0 (`GetSeancesParPeriode`), `SeancePlanningItem` V1.1.0, `GestionRecherche` V1.1.0 (`RechercherSeances`, `ResultatDeSeance`), `OuvertureResultat` V1.1.0, `UC_RechercheHome` V1.1.0, `UC_DossierFiche` V1.22.0, `AltheaDate` V1.1.0, `Home` V1.19.0 (`NavigateToRecherche`). Pas de changement de base de données.
- **Documentation** : ADR-52, interface, structure de la solution, processus 24, ToDo, plan de tests Recherche (RCH-14 à RCH-27 ; RCH-13 validé).

## 📅 23/10/2026 — Recherche transversale (R1) : bouton « Recherche », recherche rapide par code

- **Décisions de Joëlle** (ADR-52) : bouton **Recherche** dans le menu, sous Patients ; un hub avec une recherche rapide par code et, dessous, une tuile par recherche (séances d'un jour, paiements d'un jour, patients par lieu ou thérapeute aux étapes suivantes, puis au fil des besoins) ; un double-clic va à l'élément — le dossier au bon onglet pour une séance ou un paiement.
- **Recherche par code** : PA (patient), DO (dossier), SE (séance), PAI (paiement), FC (contact), AS (intervenant) ; saisie tolérante (« se83 » = SE000083). Le résultat dit ce que c'est (date, statut, domaine, montant, dossier) et de quel patient ; Entrée ou double-clic y va, « Ouvrir le patient » ouvre sa fiche. Retour ramène à la recherche.
- **Ouverture ciblée** : le dossier s'ouvre sur l'onglet Séances ou Paiements, la ligne sélectionnée sur sa page (`UC_DossierFiche` V1.21.0) ; la fiche patient sur l'onglet Famille / Contacts ou Intervenants (`UC_PatientFiche` V1.10.0).
- **Confidentialité** : le journal ne reçoit que le type de recherche et le nombre de résultats, jamais ce qui a été cherché.
- **Relevé de Joëlle** : le code d'un paiement (PAI…) ne se voyait nulle part — colonne « Code » dans l'onglet Paiements du dossier et dans le détail des paiements d'une séance ; « Paiement PAI000123 enregistré. » dans la barre d'état après un encaissement (`DialogPaiementsSeance` V1.4.0, `PaiementEditor` V2.1.0).
- Nouveaux : `QueryRecherche`, `GestionRecherche`, `ResultatRecherche`, `UC_RechercheHome`, `OuvertureResultat` ; icônes du menu `recherche_*` (provisoires, à redessiner si besoin). `Home` V1.18.0. Pas de changement de base de données.
- **Documentation** : ADR-52, interface, structure de la solution, processus, ToDo, nouveau plan de tests Recherche (RCH-01 à RCH-13).

## 📅 23/10/2026 — Audit, lot C.2 (étape 4) : comptes — règle du mot de passe et du login dans le métier ; lot C.2 terminé

- **Mot de passe** : la règle (au moins 10 caractères, une majuscule, une minuscule, un chiffre) est définie une fois, `PolitiqueMotDePasse` (nouveau, `Metier/Security`). **Corrigé** : `ChangerMotDePasse` et la réinitialisation par l'administrateur acceptaient n'importe quel mot de passe non vide (« a ») — seule la fenêtre le vérifiait ; ils l'appliquent, et le changement refuse aussi un nouveau mot de passe identique à l'ancien. Le texte de la règle affiché par la fenêtre vient de la règle elle-même.
- Le **mot de passe des sauvegardes** suit la même règle avec sa longueur propre (12, inchangée) : la copie de `GestionSauvegardes` est retirée.
- **Compte** : `GestionUtilisateurs.ValiderCompte` (login d'au moins 3 caractères et unique, nom d'affichage, rôle de base au plus égal à l'élévation maximale), lu par la fiche Utilisateur et par la création du compte du praticien, appliqué par `CreateUtilisateur`. **Corrigé** : la fiche Utilisateur acceptait un login d'un caractère ; un login déjà pris n'était signalé que par « Erreur lors de la création… Consultez les logs » — le message le dit maintenant. Le login n'est jugé qu'à la création (il ne change plus ensuite : un ancien login plus court reste valable).
- **Affichage** : fiche Utilisateur — toutes les erreurs d'un coup, sur leurs champs et dans une fenêtre récapitulative (`AffichageErreursSaisie`), au lieu d'une fenêtre par erreur ; compte du praticien et changement de mot de passe gardent leur zone de message.
- `PolitiqueMotDePasse` V1.0.0, `GestionUtilisateurs` V1.8.0, `GestionSauvegardes` V1.3.0, `ChangePassword` V1.6.0, `UtilisateurEdition` V1.9.0, `DialogComptePraticien` V1.4.0. Pas de changement de base de données.
- **Documentation** : ADR-51 (étape 4, lot C.2 terminé), sécurité et utilisateurs, structure de la solution, rapport d'audit § 9, ToDo, plans de tests Utilisateurs (section 21) et Cabinet (10.8).

## 📅 23/10/2026 — Audit, lot C.2 (étape 3) : domaines — la saisie et la complétude sont jugées par le métier

- **Saisie d'un domaine** : `GestionDomaines.ValiderDomaine` (code de 3 lettres et unique, libellé obligatoire et unique, quota pas retiré tant que des règles de quota sont actives), lu par l'écran Domaines et appliqué par `InsertDomaine` / `UpdateDomaine`. **Corrigé** : ces deux méthodes n'en vérifiaient aucune.
- **Affichage** comme dans les autres fiches (`AffichageErreursSaisie`) : toutes les erreurs d'un coup, icône sur chaque champ (« Soumis à quota » compris : elle disparaît dès qu'on coche ou décoche, `AffichageErreursSaisie` V1.2.0), fenêtre récapitulative, focus — au lieu d'une fenêtre par erreur, et seulement pour la première. Un refus du métier s'affiche dans une fenêtre.
- **Complétude** (Complet / À compléter, état des étapes, points d'attention de la synthèse) : les règles passent dans le métier (`GestionDomaines.CalculerManques`, `ManqueDomaine` et `EtapeDomaine` dans `Metier/Referentiels`), inchangées ; l'écran et sa synthèse ne font plus que les afficher. Les règles de quota d'un domaine sont lues par une seule méthode (`GestionQuotasInami.GetQuotasParDomaine`), au lieu d'une copie dans chacun des deux écrans.
- `GestionDomaines` V1.3.0, `GestionQuotasInami` V1.6.0, `UC_DomainesHome` V1.13.0, `UC_SyntheseDomaine` V2.5.0. Pas de changement de base de données.
- **Documentation** : ADR-51 (étape 3), ADR-40, guide des domaines, concepts, processus, structure de la solution, rapport d'audit § 9, ToDo, plan de tests Domaines (section 13).

## 📅 23/10/2026 — Spécialités des thérapeutes : choisies dans un référentiel

- **Base de données** (migration **v2.55.0**, à appliquer avant de lancer l'application ; requête de contrôle en tête du script) : table `ref_specialites` (code, libellé, actif, ordre) avec une liste de départ des spécialités courantes ; les spécialités déjà saisies sont reprises ; `therapeutes.id_specialite` (clé étrangère) renseignée d'après le texte. La colonne texte `therapeutes.specialite` n'est plus lue ni écrite (conservée jusqu'à vérification). La copie figée des suivis (`autres_suivis_patient.specialite`) ne change pas.
- **Décisions de Joëlle** : **une seule** spécialité par thérapeute ; liste de départ **et** reprise des valeurs saisies.
- **Fiche Thérapeute** : la spécialité se choisit dans une liste, complétée par le bouton **[+]** (référentiel en fenêtre, nouvelle valeur sélectionnée d'office). Une spécialité désactivée n'est plus proposée mais reste affichée chez le thérapeute qui l'a.
- **Hub Référentiels** : tuile **Spécialités** dans le groupe Cabinet (`UC_Specialites`) ; icônes provisoires (ToDo).
- Nouveaux : `QuerySpecialites`, `GestionSpecialites`, `Specialite`, `UC_Specialites`. `QueryTherapeutes` V1.2.0, `GestionTherapeutes` V1.5.0, `Therapeute` V1.4.0, `UtilsSaisieReferentiels` V1.2.0 (`ChargerSpecialites`), `TherapeuteEdition` V1.6.0 (reçoit Home), `UC_Therapeutes` V1.2.1, `UC_ReferentielHome` V1.6.0, `Home` V1.17.1 ; `VersionSchema` 2.55.0 ; README (schéma v2.55.0).
- **Documentation** : base de données, paramètres et référentiels, concepts, processus, ToDo, plans de tests Référentiels (section 21) et Patients (PAT-110 à PAT-112).

## 📅 23/10/2026 — Audit, lot C.2 (étape 2b) : contacts, intervenants et thérapeutes jugés par le métier ; homonyme de thérapeute accepté après confirmation ; fiche Intervenant complétée

- **Règles des fiches** dans le métier, lues par la fenêtre et appliquées par l'écriture (ADR-51) : `GestionFamilleContacts.ValiderContact` (lien et rôle légal choisis, nom et prénom, téléphone et code postal selon le pays, e-mail), `GestionSuivisIntervenants.ValiderSuivi` (thérapeute choisi, fin pas avant le début), `GestionTherapeutes.ValiderTherapeute` (nom, téléphone et code postal selon le pays, e-mail). **Corrigé** : `CreateContact` / `UpdateContact`, `CreateSuivi` / `UpdateSuivi`, `CreateTherapeute` / `UpdateTherapeute` n'en vérifiaient aucune.
- **Affichage des erreurs** comme dans la fiche patient (`AffichageErreursSaisie`) : icône sur chaque champ, fenêtre récapitulative, focus ; un refus du métier s'affiche dans une fenêtre (la saisie reste ouverte). L'icône d'une date du kit disparaît aussi dès sa correction (`AffichageErreursSaisie` V1.1.0).
- **Décision de Joëlle** : un **homonyme** de thérapeute (mêmes nom et prénom) n'est plus refusé — la fenêtre le signale et demande confirmation (« Homonyme ») ; pas de nouvelle question quand on modifie la fiche d'un homonyme déjà accepté sans changer son nom.
- **Relevé de Joëlle** : la fiche **Intervenant** montre aussi l'**e-mail** et l'**adresse complète** actuels du thérapeute choisi (lecture seule, comme le téléphone) ; la case « Lieu » devient « Adresse ». Le lieu enregistré avec le suivi reste la localité du thérapeute (`Therapeute.AdresseUneLigne`).
- La grille **Intervenants** de la fiche patient grise une intervention terminée selon le suivi lui-même (`SuiviIntervenant.EstTerminee`) au lieu de relire la cellule.
- `GestionFamilleContacts` V1.3.0, `GestionSuivisIntervenants` V1.3.0, `GestionTherapeutes` V1.4.0, `SuiviIntervenant` V1.2.0, `Therapeute` V1.3.0 ; `ContactEdition` V1.5.0, `IntervenantEdition` V1.6.0, `TherapeuteEdition` V1.5.0, `UC_PatientFiche` V1.9.1. Pas de changement de base de données.
- **Documentation** : ADR-51 (étape 2b), concepts, processus, rapport d'audit § 9, ToDo, plan de tests Patients (PAT-99 à PAT-109).

## 📅 23/10/2026 — Audit, lot C.2 (étape 2a) : la fiche patient est jugée par le métier ; NISS enregistré sous une seule forme

- **Base de données** (migration **v2.54.0**, à appliquer avant de lancer l'application, après la requête de contrôle écrite en tête du script) : les NISS existants sont ramenés à leur forme normalisée (sans espaces, points, tirets ni barres ; majuscules), sans toucher à la date de modification des patients.
- **Décisions de Joëlle** : le type d'identifiant (Belgique, France, Luxembourg, Autre) reste non enregistré mais est **transmis** au métier (`Patient.TypeIdentifiantNiss`) ; le NISS est **enregistré normalisé** et **affiché mis en forme** (« 85.07.30-033.28 », `UtilsNiss.Normaliser` / `Formater`) ; les erreurs s'affichent sur leurs champs **et dans une fenêtre récapitulative** (`AffichageErreursSaisie`, commun aux fiches).
- **Règles de la fiche** dans `GestionPatients.ValiderPatient` (nom, prénom, téléphone et code postal selon le pays, e-mail, NISS selon son type puis unicité, CT1/CT2), lues par la fiche et appliquées par `CreatePatient` / `UpdatePatient`. **Corrigé** : ces deux méthodes n'en vérifiaient aucune ; un même NISS tapé avec ou sans séparateurs passait pour deux numéros (l'unicité, index compris, était contournée). La recherche de l'écran Patients trouve un NISS avec ou sans séparateurs (`UC_PatientHome` V1.6.0).
- Le doublon nom + prénom + date de naissance reste une **confirmation** (deux homonymes nés le même jour existent).
- **Relevés de Joëlle au test** : un homonyme **sans date de naissance** (d'un côté ou de l'autre) est maintenant signalé (`QueryPatients.SelectCountDoublon` V1.1.0) ; le numéro de sécurité sociale **français** est accepté **sans sa clé** (13 caractères ; la clé est vérifiée quand elle est donnée) et un matricule luxembourgeois reste reconnu comme tel ; l'icône d'erreur d'un champ disparaît **dès sa correction** et après un enregistrement réussi.
- `GestionPatients` V1.5.0, `Patient` V1.4.0, `GestionPays` V1.2.0, `UtilsNiss` V1.1.0, `UC_PatientFiche` V1.9.0 ; `VersionSchema` 2.54.0 ; README (schéma v2.54.0).
- **Documentation** : ADR-51 (étape 2a), règles de développement, base de données, concepts, processus, structure de la solution, rapport d'audit § 9, ToDo, plan de tests Patients (PAT-87 à PAT-95).

## 📅 22/10/2026 — Onglets Séances et Paiements : ligne choisie avant d'agir, « Nouveau paiement » et colonne « Solde » retirés, bandeau et pastilles plus clairs, montants en attente réalignés

- **Relevés de Joëlle** (test P-93), **décisions de Joëlle** :
  - **Ligne choisie avant d'agir** : une grille pose d'office sa première ligne en ligne courante ; « Encaisser », « Reporter », « Détail paiements »… agissaient sur elle sans qu'elle ait été choisie (et sans surbrillance). Elles exigent maintenant une ligne sélectionnée ; sinon les boutons restent grisés (« Sélectionnez une séance dans la liste. »). Nouvelle aide `UtilsDataGrid.ElementSelectionne` (V1.11.0) ; les autres grilles de l'application sont notées au ToDo.
  - **Bouton « Nouveau paiement » retiré** de l'onglet Paiements : il encaissait la séance sélectionnée **dans l'onglet Séances**, invisible depuis Paiements, et ne proposait pas l'avoir ; son mode « choisir la séance dans la liste » n'était jamais atteint. On encaisse par « Encaisser séance » (ou double-clic sur une ligne de l'onglet Paiements). La fenêtre Paiement reçoit toujours sa séance (`PaiementEditor` V2.0.0 : liste des séances et paramètre `idDossier` supprimés).
  - **Colonne « Solde » retirée** de la grille des séances : elle répétait la pastille « Paiement » (« À régler », « Reste », « Avoir »).
  - **Bandeau du dossier plus clair** : « 0 € ici · 68,00 € tous dossiers » au lieu de « 0 € · patient : 68,00 € » ; nouvelle ligne **Prévu** (reste à payer des séances à venir, qui n'est pas un dû et n'apparaissait que dans le résumé de l'onglet Séances) ; l'avoir passe à la ligne suivante (`GestionSeances.CalculerSoldesPatient` V1.1.0, `SoldeDossier.MontantPrevu`).
  - **Montants en attente faux, impossibles à corriger** : quelques séances gardaient une ligne « En attente » qui ne correspondait plus à leur reste dû (opérations d'avant le lot B ; SE000004, reportée donc en lecture seule, ne pouvait même pas être réenregistrée). **État du système** a une nouvelle section **Cohérence des données** qui les compte, et un bouton **Réaligner les montants en attente** (Admin, confirmation, journal par identifiants) qui applique la règle unique à chacune (`GestionPaiements.RealignerDusDesalignes`, `QueryPaiements.SelectSeancesDuDesaligne`, `GestionEtatSysteme` V1.4.0, `UC_EtatSysteme` V1.2.0).
  - **Pastille « État » de l'onglet Paiements** : un encaissement partiel dit « Reste · 20 € » (séance passée) ou « Prévue · reste 20 € » (séance à venir), comme la pastille Paiement de l'onglet Séances, au lieu de « Reste 20 € dû » / « Reste 20 € » (`PaiementListeItem` V1.8.0).
- **Code mort** : `OuvrirPaiementLibre`, `GestionPaiements.GetSeancesNonSoldees` (V1.14.0), `QueryPaiements.SelectSeancesNonSoldeesParDossier` (V1.10.0), la classe `SeanceNonSoldee`, `SeanceListeItem.Solde` (V1.9.0). `UC_DossierFiche` V1.20.0.
- **Documentation** : sécurité (tableau rôle × opération), règles de développement (séance d'un encaissement, réalignement), concepts (dû / prévu), processus 20, guide séances et paiements, structure de la solution, ToDo, plans de tests Administration (section 7), Séances (S-83 à S-91), Paiements (P-05, P-31, P-39, P-80, parcours) et éditeur de texte (RTE-140).

## 📅 22/10/2026 — Audit, lot C.2 (étape 1) : séances et paiements — les règles des actions et de l'encaissement sortent des écrans

- **Décisions de Joëlle** (ADR-51) : une règle de saisie se définit une fois, dans le métier — un valideur rend la **liste des erreurs par champ** (`ErreurSaisie`, nouveau dossier `Metier/Validation`), l'écran l'affiche, l'écriture l'applique et refuse (`ErreurValidationException`) ; le **chevauchement d'horaire** est refusé par le métier. Inventaire et découpage du lot C.2 validés : séances et paiements → patient et fiches liées → domaines → comptes.
- **Séances** : reporter, supprimer, encaisser et les statuts proposés suivent les règles de `GestionSeances` (`StatutPermetReport`, `StatutPermetSuppression`, `StatutPermetEncaissement`, `StatutEstProposable`, `StatutsProposes`), lues par les boutons (`SeanceListeItem.EstReportable`, `EstSupprimable`, `EstEncaissable`, `RaisonNonEncaissable`) et par la fenêtre Séance. Le contrôle de chevauchement (`ChercherChevauchement`, règle « reste non tenue » comprise) sert la fenêtre et l'écriture, qui le relit dans sa transaction.
- **Corrigé** : `EnregistrerSeance` acceptait le statut « Reportée » en modification (séance reportée sans séance de remplacement) et un créneau occupé ; le **report** ne contrôlait pas l'horaire de la nouvelle séance — il est refusé si le créneau est occupé (`GestionSeances` V1.14.0).
- **Paiements** : la saisie d'un encaissement est jugée par `GestionPaiements.ValiderEncaissement` (séance à encaisser, montant positif et au plus égal au reste dû, mode). **Corrigé** : `CreatePaiement` acceptait tout montant sur toute séance (non due, soldée, au-delà du reste dû) ; il applique la règle sur le reste dû relu dans sa transaction et y réaligne le dû (il le faisait après coup, sur une autre connexion, un échec n'étant que journalisé). Une date de paiement vide est signalée au lieu d'une erreur technique (`GestionPaiements` V1.13.0, `QueryPaiements` V1.9.0, `PaiementEditor` V1.9.0).
- **Écrans** : `SeanceEditor` V1.10.0, `UC_DossierFiche` V1.19.0 — plus de copie des règles ; le message du report impossible dit la règle réelle (« Seule une séance planifiée peut être reportée. »).
- **Code mort** : `ExisteChevauchement`, `ResynchroniserDuSeanceSansErreur`, `SeanceEditor.ChargerStatutsProposes` / `ResteNonTenue`, `UC_DossierFiche.SeanceReportable` / `SeanceEncaissable`. Le repli « liste complète » des statuts proposés est retiré (Planifiée et Réalisée sont des lignes système protégées).
- **Documentation** : ADR-51, règles de développement (§ 4, § 16), structure de la solution, concepts, processus, guide séances et paiements, rapport d'audit (§ 6, § 9), ToDo, plans de tests Séances (S-77 à S-82) et Paiements (P-91 à P-95).

## 📅 22/10/2026 — Audit, lot C.1 (étape 3) : référentiels — droit d'écriture et lignes système vérifiés par le métier et le SQL

- **Droit d'écriture** : les 82 écritures des 25 référentiels (statuts, tarifs, natures, modes de paiement, domaines, règles et catégories de quota, rattachements, pays, civilités, organismes, thérapeutes, textes types…) reçoivent le `ContexteOperation` et vérifient le rôle **SuperUser** côté métier — la règle déjà appliquée par les écrans (nouveau module `ReglesReferentiels`, une seule définition du rôle, lue aussi par `UC_ReferentielBase` ; 20 surcharges identiques retirées).
- **Décision de Joëlle** : l'ajout « à la volée » d'une valeur depuis une fiche (bouton [+]) reste soumis à l'élévation pour un User ; l'ouvrir aux Users est noté au ToDo, à cadrer.
- **Lignes système** (ADR-28, évolution) : la condition est écrite dans les requêtes des 13 tables concernées — code inchangé et ligne active pour la modifier, désactivation exclue — et le métier refuse explicitement une écriture qui n'a concerné aucune ligne. **Corrigé** : dans 10 de ces tables, le code n'était protégé que par l'écran (et l'activation dans 7) ; les 4 protections silencieuses existantes sont remplacées par ce refus. Pas de migration.
- **Base** : `UseAffectedRows = False` rendu explicite dans la chaîne de connexion (`DatabaseManager` V1.4.0) : les écritures conditionnelles comptent les lignes trouvées, même inchangées.
- **Écrans** : `UC_ReferentielBase` affiche les refus tels quels et passe ses boîtes Windows (`MessageBox`) aux fenêtres Althéa (`DialogChoix`) ; les écrans spécialisés (domaines, quotas, rattachements, catégories, thérapeutes) affichent un refus de droit tel quel. Les panneaux de rattachement reçoivent le contexte de leur écran parent (`FournisseurContexte`).
- **Code mort** (décision de Joëlle) : 15 écritures jamais appelées supprimées (13 `Supprimer…` — les écrans désactivent, ils ne suppriment pas —, `InsertQuotaInami`, `UpdateLiaison` des tarifs) et leurs requêtes devenues inutiles.
- **Le lot C.1 est terminé** (droits côté métier : étapes 1 à 3).
- **Documentation** : ADR-50 (implémentée), ADR-28 (évolution), sécurité (tableau rôle × opération), paramètres et référentiels, base de données, rapport d'audit § 9, ToDo (ajout par un User à cadrer), plan de tests Référentiels (section 20).

## 📅 22/10/2026 — Audit, lot C.1 (étape 2) : montant libre, séance encaissée et dossier figé vérifiés par le métier

- **Montant libre** : un montant de séance différent du tarif attendu est réservé au SuperUser par le métier (`GestionSeances.EnregistrerSeance` V2.0.0, `GestionTarification.CalculerMontantAttendu`). **Corrigé** : le cadenas de la fenêtre Séance était ouvert à tous ; il est grisé pour un utilisateur simple, avec la raison au survol (`SeanceEditor` V1.9.0).
- **Décision de Joëlle** : un montant libre déjà enregistré (prix d'ami) est **conservé** quand un utilisateur modifie autre chose. **Corrigé** : un changement de statut le remplaçait par le tarif sans prévenir ; et laissé tel quel, il bloquait l'enregistrement d'un utilisateur simple.
- **Séance réalisée et encaissée** : sa modification est réservée au SuperUser par le métier aussi (`SeanceEstVerrouillee`, une règle pour l'écran et le métier).
- **Dossier figé** : clôturé, séances et paiements sont refusés ; archivé, **tout** est refusé, fiche et zones cliniques comprises (**décision de Joëlle** ; la fiche d'un dossier archivé restait modifiable). Le statut est relu en base dans la transaction de chaque écriture : séance, report, suppression, paiement, annulation, utilisation d'avoir, fiche, zone (`GestionDossiers` V1.6.0, `GestionPaiements` V1.12.0, `QueryDossiers` V1.4.0). Le texte du refus et celui des boutons grisés viennent de la même fonction (`RaisonDossierFige`).
- **Corrigé** : l'enregistrement de la fiche du dossier, d'un paiement et d'une zone clinique n'annonçait un refus ou un échec que dans la barre d'état ; une fenêtre le dit maintenant, et la saisie reste ouverte (`UC_DossierFiche` V1.18.0, `PaiementEditor` V1.8.0, `DossierZoneEditor` V1.7.0). L'utilisation d'un avoir affichait le message technique d'une erreur ; elle affiche un message clair et journalise le détail.
- **Code mort** (décision de Joëlle) : `GestionSeances.CreateSeance` et `ChangerStatutSeance`, plus appelées depuis B.2, supprimées ; `UpdateSeance` devient privée ; `AppliquerDateHeureSeance` (agenda) reçoit le `ContexteOperation`.
- **Documentation** : ADR-50 (étape 2), sécurité (tableau rôle × opération, règles d'état), processus, concepts, guide séances et paiements, rapport d'audit § 9, ToDo, plan de tests Séances (S-68 à S-76).

## 📅 22/10/2026 — Audit, lot C.1 (étape 1) : droits côté métier — un contexte d'opération, une garde unique

- **Décision de Joëlle** (ADR-50) : les méthodes métier sensibles reçoivent un **`ContexteOperation`** (utilisateur authentifié + rôle courant, élévation comprise), construit par l'écran (`UserControlContext.ContexteOperation`), au lieu de paires de paramètres différentes selon les méthodes (rôle seul, rôle + utilisateur, rôle + identifiant).
- **Garde unique** `Autorisation.Exiger` / `ExigerUtilisateur` / `Possede` (nouveau module) : un refus est une `UnauthorizedAccessException` au message rédigé pour l'utilisatrice.
- **Migrées, comportement inchangé** : comptes utilisateurs (créer, modifier, déverrouiller, activer, photo, réinitialiser), compte du praticien, sauvegardes (lancer, dossier, mot de passe, rappel), restauration (vérifier, restaurer), clé de licence, annulation d'un encaissement, utilisation d'un avoir (`GestionUtilisateurs` V1.7.0, `GestionCabinet` V1.6.0, `GestionSauvegardes` V1.2.0, `GestionRestaurations` V1.1.0, `GestionLicences` V1.1.0, `GestionPaiements` V1.11.0 et leurs écrans ; `UC_Parametres.RoleSession`, devenue inutile, supprimée).
- **Corrigé** (test 20.2) : l'écran Utilisateurs montrait Nouveau / Modifier selon le rôle **de base** du compte ; un SuperUser élevé en Admin n'avait que la consultation. Il suit maintenant le rôle courant, élévation comprise (`UC_Utilisateurs.AppliquerDroitsUtilisateur` V2.0.0).
- **Lot C** : découpage validé (C.1 droits → C.2 règles → C.3 codes → C.4 découplage → C.5 découpage ; C.6 au module Documents).
- **Documentation** : ADR-50, sécurité et utilisateurs (garde unique, tableau rôle × opération), rapport d'audit (§ 6 découpage, § 9), ToDo, plan de tests Utilisateurs (section 20).

## 📅 22/10/2026 — Audit, lot B.6 (2/2) : l'absence non excusée se règle dans le référentiel des statuts

- **Base de données** (migration **v2.53.0**, à appliquer avant de lancer l'application) : colonne `ref_statuts_seance.est_absence`, cochée pour « Absence non excusée » (`ABSENTE`) ; la vue `v_seances_solde` est recréée pour lire cette case au lieu du code.
- **Décision de Joëlle** : une case dans le référentiel, comme « Séance due » et « Consomme le quota ». **Référentiels › Statuts de séance** : nouvelle case « Absence non excusée (no-show) » (`UC_StatutsSeance` V1.2.0).
- La case gouverne : le tarif (absence non facturée dans un domaine qui ne facture pas les absences), la facturation (reste dû, dû en attente), le compteur d'absences et la « dernière séance » de la grille Patients, la pastille et le filtre « Absences » de l'onglet Séances, l'aperçu du patient. Un second statut d'absence pourra être créé et coché.
- Le code `ABSENTE` n'est plus écrit dans aucune requête ni dans le calcul du tarif (`QueryPatients` V1.9.0, `QuerySeances` V1.4.0, `QueryTarification` V1.1.0, `GestionTarification` V1.3.0 — `ResoudreTarifSeance` V2.0.0 ne reçoit plus le code du statut —, `SeanceListeItem` V1.7.0, `GestionSeances` V1.12.0, `SeanceEditor` V1.8.3 ; référentiel : `StatutSeance`, `GestionStatutsSeance`, `QueryStatutsSeance` V1.1.0).
- **Demande de Joëlle** (relevée pendant les tests) : passer d'un statut « non tenu » à un autre — annulée (patient ou praticien), maladie, reportée, ou tout statut coché « Absence non excusée » — ne déclenche plus le contrôle de chevauchement de l'horaire. Une séance annulée dont le créneau a été repris ne pouvait pas être requalifiée en absence (`GestionSeances.EstStatutNonTenu`, `SeanceEditor.ValiderChevauchement` V1.1.0, `SeanceEditor` V1.8.4).
- **Le lot B est terminé** (intégrité des données : B.1 à B.6).
- **Documentation** : ADR-48 (évolution), base de données, concepts, guide, règles, rapport d'audit § 9, ToDo, plan de tests Séances (S-63 à S-66).

## 📅 22/10/2026 — Audit, lot B.6 (1/2) : un seul calcul du quota ; règle relue après un changement de domaine

- **Un seul calcul du quota** (renouvellement de date à date, celui de l'onglet Séances) : l'aperçu du patient (section Quota) et l'alerte « cette séance dépasse le quota » de la fenêtre Séance l'utilisent aussi (`GestionQuotaInami` V3.0.0, `CalculerQuota`).
- **Corrigé** : ces deux écrans calculaient en SQL la seule **première** période, ouverte par la toute première séance du dossier. Dès la 2e période, l'aperçu affichait d'autres chiffres que l'onglet Séances, et l'alerte comptait des séances d'une période terminée (dépassement annoncé à tort, ou dépassement réel non vu).
- **Corrigé** : l'alerte comptait la nouvelle séance dans la période d'aujourd'hui ; elle la compte maintenant **à sa date** (une séance planifiée dans la période suivante ouvre ou rejoint celle-ci) (`SimulerAjoutSeance` V3.0.0, `SeanceEditor` V1.8.2).
- **Corrigé** : la règle de quota était gardée en mémoire **par dossier** ; un dossier changé de domaine gardait la règle de l'ancien jusqu'au redémarrage. Elle est lue d'après le domaine actuel du dossier et gardée en mémoire par domaine (`GestionQuotasInami` V1.4.0).
- **Corrigé** (trouvé au test TQ-25) : après un changement de domaine enregistré dans la fiche du dossier, le bloc Quotas du bandeau restait sur l'ancien domaine jusqu'à la réouverture du dossier ; les séances sont relues (`UC_DossierFiche` V1.17.5).
- **Supprimé** : les requêtes de la première période (`SelectDateDebutPeriodeInami`, `SelectCompteursQuotaInami`) et `SelectQuotaInamiParDossier` (`QueryQuotaInami` V2.0.0, `QueryQuotasInami` V1.3.0).
- Aucune modification de la base.
- **Documentation** : ADR-27 (évolution), concepts, processus, rapport d'audit § 9, plan de tests Tarification et quotas (TQ-22 à TQ-25).

## 📅 22/10/2026 — Audit, lot B.5 (3/3) : chaque utilisation d'un avoir est reliée à l'encaissement qui l'a financée

- **Base de données** (migration **v2.52.0**, à appliquer avant de lancer l'application) : table `lia_paiements_avoirs` — pour chaque règlement par avoir, l'affectation encaissée qu'il a diminuée, le montant pris, la date et l'utilisateur (identifiant). Structure seulement ; le retour arrière recopie les utilisations dans les commentaires.
- **Utilisation d'un avoir** (encaissement d'une séance, ou après un report / un statut non dû) : le lien est enregistré dans cette table, dans la même transaction ; plus de mentions écrites dans les commentaires (« [Avoir … utilisé le … par … pour S…] » sur l'encaissement d'origine, « Avoir utilisé : PAI… (S…) … » sur le règlement). Les utilisations d'avant restent lues dans les commentaires (décision de Joëlle, ADR-49).
- **Affichage** : inchangé sur l'encaissement d'origine (« Avoir utilisé → S… », mention dans l'infobulle). **Nouveau** : l'infobulle d'un règlement par avoir dit d'où vient l'argent (« Avoir utilisé : PAI… (S…) 20,00 € ») ; cette phrase existait déjà en commentaire mais ne s'affichait nulle part.
- **Confidentialité** : le journal note « utilisateur ID=… » au lieu du nom affiché ; l'utilisateur est transmis par son identifiant partout (`UtiliserAvoirPourSeance`, `UtiliserAvoirDeSeance`, `DialogUtilisationAvoir.ProposerApresChangementStatut`).
- Code : `QueryPaiements` V1.8.0 (`InsertUtilisationAvoir`, `SelectUtilisationsAvoirParDossier` / `ParSeance`, `DiminuerAffectationAvoir` V2.0.0), `GestionPaiements` V1.10.0 (`ImputerAvoir` V2.0.0, `AttacherUtilisationsAvoir`), nouvelle classe `UtilisationAvoir`, `PaiementListeItem` V1.7.0 (`MentionOrigineAvoir`), `DialogUtilisationAvoir` V2.0.1, `SeanceEditor` V1.8.1, `UC_DossierFiche` V1.17.4.
- **Le lot B.5 est terminé** : mode de paiement par identifiant, annulation et utilisation d'avoir enregistrées dans des colonnes et une table, journal sans nom.
- **Documentation** : ADR-49 (implémentée), base de données, concepts, guide (§ 5.3, 5.5, 7, 9.12, 10), processus, rapport d'audit § 9, plan de tests Paiements (P-86 à P-90).

## 📅 22/10/2026 — Audit, lot B.5 (2/3) : l'annulation d'un encaissement est enregistrée dans des colonnes

- **Base de données** (migration **v2.51.0**, à appliquer avant de lancer l'application) : colonnes `paiements.date_annulation`, `id_utilisateur_annulation` (clé étrangère vers `sec_utilisateurs`) et `motif_annulation` (500 caractères). Structure seulement.
- **Annulation** : la date, l'utilisateur et le motif s'enregistrent dans ces colonnes ; plus de mention ajoutée au commentaire. L'écran affiche la même phrase qu'avant (« Annulé le … par … : motif »), le nom de l'utilisateur étant lu à l'affichage. Les annulations d'avant gardent leur mention dans le commentaire et s'affichent comme avant (décision de Joëlle, ADR-49).
- **Confidentialité** : l'auteur est enregistré par son **identifiant** ; le journal note « utilisateur ID=… » au lieu du nom affiché (règle du projet : identifiants seulement).
- **D'un seul tenant** (ADR-46) : contrôles (règlement par avoir, part déjà utilisée en avoir), annulation et dû de la séance dans une seule transaction. Avant, le dû était réaligné après coup, sur une autre connexion, et un échec n'était que journalisé.
- **Motif** : 500 caractères au plus ; la saisie s'arrête à cette limite (`DialogMotifAnnulation` V1.5.0).
- Code : `QueryPaiements` V1.7.0 (`SelectIdsSeancesParPaiement`, requête qui était écrite dans le module métier), `GestionPaiements` V1.9.0 (`AnnulerPaiementEncaisse` V2.0.0, `LONGUEUR_MAX_MOTIF_ANNULATION`), `PaiementListeItem` V1.6.0, `UC_DossierFiche` V1.17.3.
- **Documentation** : ADR-49, base de données, guide séances et paiements (§ 9.11), processus, règles, rapport d'audit § 9, plan de tests Paiements (P-18 revu ; P-81 à P-85).

## 📅 21/10/2026 — Audit, lot B.5 (1/3) : le mode d'un paiement est relié au référentiel

- **Base de données** (migration **v2.50.0**, à appliquer avant de lancer l'application) : colonne `paiements.id_mode_paiement`, clé étrangère vers `ref_modes_paiement`, renseignée d'après le libellé existant. Un ancien libellé inconnu du référentiel devient un mode **inactif** (rien n'est perdu). La colonne texte `mode_paiement` n'est plus lue ; elle est conservée.
- **Décision de Joëlle** (ADR-49) : identifiant et clé étrangère, plutôt que le code (celui d'un mode non système reste modifiable). Renommer un mode ne coupe plus le lien avec l'historique : les anciens paiements affichent le nouveau libellé.
- **Corrigé** : un règlement par avoir n'était reconnu que par le libellé « Avoir » ; renommer ce mode rendait ses règlements annulables. Il est reconnu par le référentiel (case « Mode avoir ») (`QueryPaiements.SelectEstReglementParAvoir`, `PaiementListeItem.EstReglementParAvoir`).
- Un mode utilisé ne peut plus être supprimé (la base le refuse) ; il se désactive.
- **Supprimé** : le contrôle « mode déjà utilisé » (`GestionModesPaiement.ModePaiementEstUtilise`), qui cherchait le code alors que la table contenait le libellé et n'était appelé nulle part.
- Code : `QueryPaiements` V1.6.0, `QueryModesPaiement` V1.1.0, `GestionModesPaiement` V1.1.0, `Paiement` V1.1.0, `PaiementListeItem` V1.5.0, `GestionPaiements` V1.8.0, `PaiementEditor` V1.7.0, `UC_DossierFiche` V1.17.2.
- **Documentation** : ADR-49, base de données, rapport d'audit § 9, plan de tests Paiements (P-76 à P-80).

## 📅 21/10/2026 — Audit, lot B.4 : une seule définition du solde d'une séance et du dossier dominant

- **Base de données** (migration **v2.49.0**, à appliquer avant de lancer l'application) : deux vues SQL, structure seulement, aucune donnée modifiée.
  - `v_seances_solde` : pour chaque séance, le réglé (paiements encaissés), « due », « facturable », le reste dû, « échue » (passée) et l'avoir.
  - `v_dossier_dominant` : le dossier que l'on montre comme suivi d'un patient.
- **Reste dû** : il était écrit cinq fois, avec des résultats différents. Toutes les lectures passent désormais par la vue : onglets Séances et Paiements, grille Patients, aperçu, règle unique du dû, liste « Nouveau paiement » (`QuerySeances` V1.3.0, `QueryPaiements` V1.5.0, `QueryPatients` V1.8.0, `SeanceListeItem` V1.6.0, `GestionSeances` V1.11.0, `GestionPaiements` V1.7.0).
- **Corrigé** : une absence non facturable (domaine qui ne facture pas les absences) gardait un dû « en attente » et était proposée dans « Nouveau paiement ». Elle ne doit plus rien partout. Une ligne en attente déjà créée s'annule à la prochaine resynchronisation de la séance (l'ouvrir et l'enregistrer).
- **Corrigé** : le reste dû d'une séance non due (annulée, reportée) affiché sur une ligne de paiement n'était pas à 0.
- **Dossier dominant** (décision de Joëlle) : un dossier vivant (actif ou en pause) d'abord, puis celui dont une séance est la plus proche d'aujourd'hui, puis le plus récemment ouvert. Avant, la grille Patients prenait le statut d'un dossier et le domaine d'un autre (deux règles), et l'aperçu une troisième règle (`QueryDossiers` V1.3.0, `GestionDossiers.ChoisirDossierDominant` V2.0.0, `GestionApercuPatient` V1.0.1).
- **Restauration** : la base contient désormais des vues, donc le compte d'administration doit aussi avoir `CREATE VIEW` et `SHOW VIEW` (c'est le cas de `root`). Ce contrôle existait déjà.
- **Documentation** :
  - ADR-48 ; base de données (§ Vues) ; concepts ; guide séances et paiements (§ 1, 7, 10) ; processus ; rapport d'audit § 9 ;
  - plans de tests Paiements (P-71 à P-74), Séances (S-62), Patients (PAT-84 à PAT-86) et Sauvegardes (7.6 revu, 7.18).

## 📅 21/10/2026 — Filtres des grilles Séances et Paiements (demandes de Joëlle)

- **Nouveaux filtres** des séances : « Aujourd'hui » (séances du jour, tout statut) et « Reportées ». Ordre revu : le temps, puis le statut, puis l'argent.
- **Corrigé** : le filtre « À venir » (et la pastille « à venir » du résumé) montrait aussi les séances reportées ou annulées à venir, alors que la pastille n'en comptait pas autant. Il ne garde plus que les séances **dues** à venir.
- **Couleurs** : chaque axe des filtres Séances et Paiements s'affiche en pastille, aux couleurs de la grille et du résumé (« Aujourd'hui » en bleu plein comme le rendez-vous du jour de la grille Patients, « Passées » au trait) (`ElementListePastille`, `AltheaListe` V1.2.0, `UtilsDataGrid.DessinerPastilleZone` ; `UC_DossierFiche` V1.17.0).
- **Documentation** : guide séances et paiements, référence UI (§ 7.8), plan de tests Séances (S-58 à S-61).

## 📅 21/10/2026 — Audit, lot B.3 : une séance passée non confirmée s'affiche « À confirmer »

- **Décision de Joëlle** (ADR-47) : une séance planifiée dont l'heure est passée n'est plus basculée en « Réalisée » quand on ouvre l'onglet Séances d'un dossier. Lire un onglet modifiait la base, sans trace, et effaçait l'information « confirmée ou présumée » dont l'Agenda aura besoin (`GestionSeances.GetSeancesParDossier` V1.1.0 ; `QuerySeances.BasculerPlanifieesEchues` supprimée).
- **Pastille « À confirmer »** (ocre) au lieu de « Réalisée » présumée : grille des séances, résumé (« N à confirmer », cliquable), filtre « À confirmer », planning du jour, aperçu du patient (`SeanceListeItem.EstAConfirmer`, famille `AConfirmer`, `UtilsPastilles` V1.7.0, `UC_DossierFiche` V1.16.0).
- Le pointage se fait dans la fenêtre Séance (choisir le statut). Une séance à confirmer reste due (facturation et quotas inchangés), n'est pas verrouillée même encaissée (seul le statut réel « Réalisée » verrouille) et peut encore être reportée.
- Les séances déjà basculées restent « Réalisée » (on ne peut pas savoir lesquelles l'ont été automatiquement).
- **Documentation** : ADR-47, concepts, guide, POC Agenda, rapport d'audit § 9, plan de tests Séances (S-31, S-43 revus ; S-53 à S-57).

## 📅 21/10/2026 — Audit, lot B.2 (5/5) : le compte du praticien se crée d'un seul tenant

- **Compte de travail du praticien** (Administration › Cabinet) : le compte et son lien au praticien sont créés dans une seule transaction (`GestionCabinet.CreerCompteSuperUser` V1.1.0). Avant, si la liaison échouait, le compte restait créé, orphelin, avec un mot de passe temporaire jamais affiché.
- Le praticien doit être enregistré **avant** la création (le refus arrivait après la création du compte).
- `GestionUtilisateurs.CreateUtilisateur` : contrôle du login et insertion dans une transaction, variante dans la transaction d'un appelant.
- **Journal** : le lien praticien ↔ compte est noté par identifiants (le nom du praticien y figurait).
- Message d'échec de la fenêtre « Compte de travail » : « rien n'a été enregistré » (`DialogComptePraticien` V1.2.1).
- **Le lot B.2 est terminé** : les cinq opérations relevées par l'audit (§ 4.2) sont d'un seul tenant (ADR-46).
- **Documentation** : ADR-46, sécurité et utilisateurs, rapport d'audit § 9, plan de tests Cabinet (10.20 à 10.22).

## 📅 21/10/2026 — Audit, lot B.2 (4/5) : une nouvelle règle de quota s'enregistre d'un seul tenant

- **Règles de quota** (écran Domaines, étape 3) : l'arrêt de la règle ouverte à la veille et l'ajout de la nouvelle règle se font dans une seule transaction (`GestionQuotasInami.AjouterRegleEnChaine`). Avant, l'écran arrêtait la règle, contrôlait, insérait, puis la rétablissait à la main en cas d'échec — rétablissement qui pouvait lui-même échouer et laisser une période sans règle.
- Le chevauchement est contrôlé **dans** la transaction, après l'arrêt de la règle précédente ; la règle précédente n'est arrêtée que si elle est encore ouverte en base (écran périmé : refus).
- La cohérence de la saisie (plafonds, seuil, dates) est contrôlée **avant** le récapitulatif : on ne fait plus confirmer une règle qui sera refusée (`GestionQuotasInami.ValiderCoherence`, `UC_DomainesHome` V1.11.0).
- **Messages** : un refus s'affiche avec sa raison ; un échec annonce « Rien n'a été modifié (la règle actuelle reste en vigueur) ».
- **Documentation** : ADR-40 et ADR-46, concepts, base de données, rapport d'audit § 9, plan de tests Domaines (section 12, D-101 à D-104).

## 📅 21/10/2026 — Audit, lot B.2 (3/5) : la réaffectation est remplacée par l'avoir

- **Décision de Joëlle** : « sécuriser et repasser par l'avoir, dont on garde ainsi l'historique ». Quand une séance déjà encaissée est reportée ou passe à un statut non dû, l'encaissement **reste sur sa séance** et devient un avoir ; l'application propose aussitôt de l'utiliser (ADR-26, évolution du 21/10/2026).
- **Fenêtre « Utilisation de l'avoir »** (`DialogUtilisationAvoir`, ex-`DialogPaiementReaffectation`) : « Avoir disponible : 60,00 € (séance S…) », choix parmi les séances **dues non soldées du dossier** (reste dû affiché ; la séance de remplacement présélectionnée après un report), boutons **Utiliser l'avoir** / **Garder l'avoir**. Seul l'avoir de la séance concernée est proposé ; sans séance due, un message dit qu'il est conservé (plus de proposition de créer une séance). Après utilisation, un message dit ce qui a été réglé et ce qui reste en avoir.
- **Utilisation** : `GestionPaiements.UtiliserAvoirDeSeance` (une transaction, dû réaligné dans la même) — le mécanisme existant de l'avoir (liaison source diminuée avec mention, paiement « Avoir » sur la séance réglée) ; plus de répartition automatique de l'excédent. `UtiliserAvoirPourSeance` (encaissement) partage le même code (`ImputerAvoir`) et devient lui aussi d'un seul tenant.
- **Supprimés** (ne servaient qu'à la réaffectation) : `GestionSeances.ReaffecterPaiementVersSeance`, `GestionPaiements.RepartirPaiementSurSeances`, `GetPaiementsEncaissesParSeance`, `GetResteDuSeance`, `GetAffectationsParPaiement`, les classes `PaiementEncaisse`, `AffectationPaiement`, `ResultatReaffectation` et trois requêtes. Le déroulé, écrit deux fois (onglet Séances, fenêtre Séance), est réuni (`DialogUtilisationAvoir.ProposerApresChangementStatut`).
- Les réaffectations faites avant le 21/10/2026 restent valables (paiement réparti) et s'affichent comme avant.
- Renommages : `NecessiteReaffectationPaiement` → `CreeraitUnAvoir` ; `SeanceListeItem.LibelleReaffectation` → `LibelleChoixSeance` (reste dû au lieu du tarif).
- **Documentation** : ADR-26 (évolution) et ADR-46, guide séances et paiements (5.2, 5.3, 9.4, 9.7, 9.10), processus 20 et 20b, concepts, structure, interface, charte, règles, rapport d'audit § 9, plans de tests Paiements (P-65 à P-70 ; anciens cas de réaffectation remplacés ; tableau réparé : une ligne vide le coupait après P-47), Séances (même réparation après S-49), Parcours.

## 📅 21/10/2026 — Audit, lot B.2 (2/5) : le report d'une séance est d'un seul tenant

- **Report** : la séance d'origine passe « Reportée » (son dû annulé) et la séance de remplacement est créée avec son dû, dans une seule transaction (`GestionSeances.ReporterSeance` V2.0.0). Avant, si la création échouait, une « compensation » tentait de remettre l'origine en Planifiée ; si elle échouait aussi, la séance restait reportée sans remplaçante, bloquée. La compensation est supprimée.
- L'origine n'est marquée reportée que si elle est encore **planifiée** en base : un écran périmé (autre fenêtre, autre poste) est refusé, sans double report (`QuerySeances.UpdateStatutSeanceSiInchange`).
- **Messages** : un refus s'affiche tel quel ; un échec annonce « rien n'a été modifié (la séance reste planifiée) » (`UC_DossierFiche` V1.14.2).
- La proposition de réaffecter un encaissement reste juste après le report (étape 3 : elle passera par l'avoir, décision de Joëlle).
- **Documentation** : ADR-46, guide séances et paiements (9.7), rapport d'audit § 9, plan de tests Séances (S-50 à S-52).

## 📅 21/10/2026 — Audit, lot B.2 (1/5) : une séance s'enregistre d'un seul tenant

- **Unité de travail** : `DatabaseManager.ExecuterEnTransaction` ouvre une connexion et une transaction, exécute les écritures d'une opération et valide ; au moindre échec, tout est annulé (ADR-46).
- **Fenêtre Séance** : identité, notes et évolution, statut et dû s'enregistrent en une seule transaction (`GestionSeances.EnregistrerSeance`). Avant, l'écran enchaînait trois écritures, chacune sur sa connexion : un incident pouvait laisser une séance sans ses notes, sans son statut, ou **sans son dû** — l'échec du dû était avalé et l'écran annonçait un succès (`SeanceEditor` V1.7.0).
- **Message à l'écran** : un refus ou un échec d'enregistrement s'affiche dans une fenêtre (il ne restait que dans la barre d'état) ; la fenêtre Séance reste ouverte avec la saisie, et rien n'a été modifié.
- `CreateSeance`, `UpdateSeance` et `ChangerStatutSeance` écrivent aussi la séance et son dû ensemble (`GestionSeances` V1.7.0) ; la règle unique du dû lit et écrit dans une même transaction, ou dans celle de l'appelant (`GestionPaiements.ResynchroniserDuSeance` V2.1.0).
- Onglet Séances (demande de Joëlle) : une séance reportée affiche la date **et le code** de sa remplaçante (« → 31/10 · S000045 ») (`SeanceListeItem` V1.3.0, `UC_DossierFiche` V1.14.1).
- Nettoyage : `UpdateNotesSeance`, remplacée, supprimée ; `CreatePaiementEnAttente` et `AnnulerPaiementsNonEncaissesParSeance` deviennent internes à la règle unique.
- **Documentation** : ADR-46, règles de développement, architecture, processus, rapport d'audit § 9, plan de tests Séances (S-45 à S-49).

## 📅 21/10/2026 — Audit, lot B.1 : intégrité des paiements et du statut des dossiers

- **Corrigé — report d'un encaissement** : seule la part encore portée par la séance d'origine est reportée. Si une partie avait servi d'avoir, elle était reportée une seconde fois (60 € reportés pour 40 € disponibles). La fenêtre de réaffectation l'indique (« 40,00 € (sur 60,00 € encaissés) ») (`GestionSeances.ReaffecterPaiementVersSeance` V3.0.0, `PaiementEncaisse` V1.2.0).
- **Corrigé — paiement réparti sur plusieurs séances** : reporter une de ses parts effaçait les autres (une séance réglée redevenait due). Elles restent en place ; chaque séance touchée est réalignée par la règle unique (le dû d'une séance couverte en partie ne l'était pas) (`GestionPaiements.GetAffectationsParPaiement`).
- **Annulation refusée** (décision de Joëlle) : un encaissement dont une partie a déjà servi d'avoir ne s'annule plus — la séance réglée par cet avoir resterait payée avec un montant qui n'existe plus. Le refus s'affiche avant la saisie du motif (`GestionPaiements` V1.4.0, `UC_DossierFiche` V1.14.0).
- **Une seule ligne de dû par séance** : après plusieurs allers-retours de statut, tous les dus annulés étaient réactivés et le dû multiplié. La règle unique ne garde qu'une ligne et annule les doublons, y compris ceux déjà en base, à la prochaine action sur leur séance (`ResynchroniserDuSeance` V2.0.0 ; `QueryPaiements` V1.3.0 : `UpdateDuSeance`, `AnnulerAutresPaiementsEnAttenteParSeance`).
- **Statut du dossier** : la transition est validée sur le statut relu en base, plus sur celui affiché ; un écran périmé (autre fenêtre, autre poste) est refusé avec un message, et l'écriture n'a lieu que si le statut n'a pas changé — un dossier archivé ne peut plus être réactivé par erreur (`GestionDossiers.ChangerStatutDossier` V2.0.0, `QueryDossiers` V1.2.0).
- **Corrigé** : l'archivage effaçait la date de clôture du dossier ; elle est conservée (les dossiers déjà archivés l'ont perdue).
- Nettoyage : `SynchroniserMontantDuParSeance`, `ReactiverPaiementsAnnulesParSeance` et leurs requêtes « toutes lignes » remplacées ; `SelectIdsPaiementsEnAttenteParSeance` (inutilisée) supprimée ; `GetResteDuSeance` sans paiement exclu.
- **Documentation** : guide séances et paiements, processus, concepts métier, rapport d'audit § 9, plans de tests Paiements (P-60 à P-64) et Dossiers (D-34, D-35).

## 📅 20/10/2026 — Audit, lot A : élévation limitée (essais et inactivité)

- **Essais comptés** (décision de Joëlle) : un mot de passe faux au cadenas compte comme à la connexion ; au seuil (`SECURITE_MAX_ECHECS_LOGIN`), le compte est verrouillé — la session en cours continue au rôle de base, un administrateur déverrouille. Le message dit combien d'essais il reste. Le compte est relu en base à chaque demande (verrouillé ou mot de passe changé entre-temps) (`GestionUtilisateurs.VerifierElevationUtilisateur` V2.0.0, `GestionAuthentification` V1.2.0, `ElevationAcces` V1.3.0).
- **Fin après inactivité** (décision de Joëlle) : sans activité au clavier ni à la souris pendant 15 minutes (paramètre « Fin de l'élévation après inactivité », Admin), l'élévation prend fin : message, retour à l'accueil depuis l'Administration ou les Référentiels. Tant qu'on travaille, elle reste ; jamais pendant qu'une fenêtre est ouverte par-dessus (seules les fenêtres affichées comptent, pas une fenêtre cachée comme l'aperçu d'une séance). Le journal note la surveillance (« Élévation surveillée : fin après N min ») et la fin (`Home` V1.16.1, `SuiviActivite`).
- **Base de données — migration v2.48.0** (`migration_v2.48.0_elevation_inactivite.sql`, + ROLLBACK) : paramètre `SECURITE_ELEVATION_MINUTES`. `VersionSchema.Attendue` = 2.48.0 ; `reinit_tec_parametres.sql` (16 paramètres).
- **Documentation** : sécurité et utilisateurs, processus (élévation), paramètres, base de données, structure, rapport d'audit § 9, plan de tests Utilisateurs section 19.

## 📅 20/10/2026 — Audit, lot A : droits vérifiés par le métier, fichiers de configuration écrits sans risque

- **Droits sur le rôle courant** (décision de Joëlle) : l'élévation compte partout. Un compte élevé en Admin peut gérer les utilisateurs (avant, l'écran s'ouvrait mais le métier refusait, alors que les Sauvegardes acceptaient). Les opérations sensibles vérifient elles-mêmes le droit, au lieu de se fier à l'écran : comptes (création, modification, déverrouillage, photo, activation, réinitialisation du mot de passe), compte du praticien, annulation d'un encaissement (`GestionUtilisateurs`, `GestionCabinet` V1.4.0, `GestionPaiements` V1.3.0 ; `UserControlContext.RoleCourant`).
- **Garde-fous** (décision de Joëlle) : on ne modifie pas son propre rôle, son élévation ni l'état de son compte ; aucune modification ne peut laisser la base sans administrateur actif. Refus expliqués à l'écran (`UtilisateurEdition` V1.7.0).
- **Compte de travail du praticien** réservé à l'administrateur : un SuperUser pouvait créer un compte dont l'élévation va jusqu'à Admin. Pour lui, les boutons sont inactifs et disent pourquoi (`UC_Cabinet` V1.8.0, `DialogComptePraticien` V1.2.0).
- **Fichiers de configuration** (connexion, licence, mot de passe des sauvegardes) : écrits dans un fichier temporaire qui remplace ensuite l'ancien ; un plantage pendant l'écriture ne laisse plus un fichier vide (`ConfigManager` V1.1.0).
- Nettoyage : `ResetPasswordUtilisateur`, jamais appelée, supprimée ; une boîte de message Windows remplacée par `DialogChoix`.
- **Documentation** : sécurité et utilisateurs, rapport d'audit § 9, plans de tests (Utilisateurs section 18, Cabinet 9.7).

## 📅 20/10/2026 — Audit, lot A : erreurs imprévues et version de la base au démarrage

- **Erreurs imprévues** : une erreur que rien n'interceptait affichait le détail technique de .NET (pile, noms de tables) et n'était pas journalisée. Elle est maintenant journalisée et l'utilisateur reçoit un message clair qui dit quoi faire ; l'application continue (ou annonce sa fermeture si l'erreur est hors de l'interface) (`GestionErreursGlobales`, `Program` V1.2.0). Bouton de test « Provoquer une erreur » dans la carte Outils de test de l'accueil (Debug, `UC_Accueil`).
- **Version de la base au démarrage** (décisions de Joëlle) : base en retard d'une migration → message (version, marche à suivre) puis fermeture ; base plus récente que l'application ou version illisible → avertissement, puis démarrage (`AppStartupManager` V1.4.0).
- La version de la base est la plus haute migration appliquée, comparée comme un numéro — l'écran État du système triait par date et pouvait se tromper si une ancienne migration était rejouée (`GestionEtatSysteme` V1.3.0, `QueryEtatSysteme` V1.1.0).
- **Documentation** : processus 01 (démarrage), architecture, structure, rapport d'audit § 9, plan de tests Administration section 6.

## 📅 20/10/2026 — Audit, lot A : plus de mot de passe dans le code, plus de traces de débogage

- **Sécurité** : l'outil « hash » de l'accueil (version Debug) demande le mot de passe dans une petite fenêtre à saisie masquée ; le mot de passe qui était écrit dans le code est retiré (décision de Joëlle : garder l'outil, utile pour amorcer un compte) (`UC_Accueil` V1.6.0).
- **Sécurité** : le mot de passe temporaire (nouveau compte, réinitialisation) est tiré sans biais par le générateur cryptographique actuel ; plus de mot de passe de secours identique pour tous en cas d'erreur (`GestionUtilisateurs.GenererMotDePasseTemporaire` V2.0.0).
- **Nettoyage** : trois traces « [DEBUG] » du journal et un ancien bloc de commentaires « TODO DEBUG » retirés (`GestionUtilisateurs` V1.5.0, `UtilisateurEdition` V1.6.1).
- **Documentation** : rapport d'audit § 9, plan de tests Utilisateurs section 17.

## 📅 20/10/2026 — Audit, lot A : bugs B9 à B12

- **Corrigé (B9)** : la règle « no-show non facturable » ne s'appliquait jamais à la création d'une séance — elle cherchait un statut « NO_SHOW » qui n'existe pas ; l'absence non excusée est `ABSENTE`. Une séance « Absence non excusée » dans un domaine où le no-show n'est pas facturable reçoit maintenant un tarif proposé de 0 (`GestionTarification` V1.2.0). Les codes des statuts de séance sont définis une seule fois (`GestionSeances.CODE_STATUT_*`, V1.5.0) et repris partout (séance, listes, fiche du dossier, fenêtre Séance).
- **Sécurité (B10)** : l'écran Domaines donnait le droit de modifier quand il ne connaissait pas l'utilisateur ; il le refuse désormais (`UC_DomainesHome` V1.10.1).
- **Sans objet (B11)** : la fausse alerte sur une zone vide a disparu avec la garde des modifications du 19/10.
- **Corrigé (B12)** : le téléphone de l'intervenant d'un patient restait vide à la réouverture ; c'est le numéro actuel du thérapeute lié qui s'affiche (décision de Joëlle, `IntervenantEdition` V1.5.0).
- **Documentation** : rapport d'audit § 9, plans de tests (Tarification TQ-07 et TQ-08, Patients PAT-83), ToDo (drapeaux des statuts de séance).

## 📅 20/10/2026 — Audit, lot A : bugs B3 à B6

- **Corrigé (B3, RGPD)** : à chaque navigation, le journal détaillé notait le texte du fil d'Ariane, qui contient le nom du patient ouvert ; il note désormais le nom du bouton du menu (`Home` V1.15.0). Même passe : le nom des intervenants d'un patient ne figure plus dans le journal, seulement des identifiants (`GestionSuivisIntervenants` V1.2.0).
- **Corrigé (B4)** : « Afficher les inactifs » provoquait une erreur dans Natures de séance et Statuts de séance (requête incomplète).
- **Corrigé (B5)** : l'enregistrement d'une trace de document renvoyait toujours l'identifiant 0 ; il renvoie le vrai numéro (prérequis du module Documents).
- **Nettoyage (B6)** : trois méthodes jamais appelées et cassées supprimées de `GestionUtilisateurs` (V1.5.0), avec leur requête.
- **Documentation** : rapport d'audit § 9, plans de tests (Administration 3.4 et 3.5, Référentiels section 19).

## 📅 20/10/2026 — Fiche Cabinet : tout s'enregistre ensemble (audit § 4.2)

- **Images et cachet attendent Enregistrer** (décision de Joëlle : « tout différer ») : logo, photo, signature, cachet importé ou généré s'affichent aussitôt et activent Enregistrer, mais ne sont écrits qu'avec la fiche ; Annuler les abandonne, quitter l'écran pose la question habituelle. Plus de demande « remplacer ? », rien n'étant définitif avant Enregistrer — comme le logo des domaines (`UC_Cabinet` V1.7.0).
- **Cachet généré depuis la saisie en cours** : nom, titre, adresse, lignes d'identifiants et de qualifications cochées « Cachet », même pas encore enregistrées (avant : données en base).
- **Tout ou rien** : cabinet, praticien, identifiants et qualifications s'écrivent en une seule transaction (`GestionCabinet.EnregistrerFiche`, V1.3.0) ; en cas d'échec, rien n'est enregistré et le message le dit, avec la marche à suivre. Les images s'écrivent ensuite, fichier puis nom en base (`EnregistrerImages`, `QueryCabinet` V1.3.0) ; si l'une échoue, la fiche reste enregistrée et un avertissement l'indique.
- Grilles Identifiants et Qualifications : saisie limitée à la longueur des colonnes (la base refusait l'enregistrement).
- **Documentation** : plan de tests Cabinet (sections 6, 7, 9), rapport d'audit § 9, ToDo.

## 📅 20/10/2026 — Types d'identifiant professionnel : écran de gestion ; tuile Domaines retirée des Référentiels

- **Nouvel écran « Types d'identifiant »** (hub Administration, à côté des situations juridiques — décision de Joëlle ; SuperUser et Admin) : les numéros professionnels proposés dans l'onglet Identifiants de la fiche Cabinet (INAMI, agrément, BCE, TVA…) se gèrent sans script SQL. Code, libellé, **porté par** (Praticien / Cabinet, préparé pour un cabinet de plusieurs praticiens), **masque** indicatif, **expression de contrôle** — vérifiée avant d'enregistrer : une expression mal formée était ignorée sans que personne ne s'en aperçoive. Colonne « Contrôle » : Althéa, Expression ou saisie libre (`UC_TypesIdentifiantPro`, `GestionTypesIdentifiantPro`, `QueryTypesIdentifiantPro` ; `UC_AdminHome` V1.6.0, Utilisateurs passe en quatrième position).
- **INAMI, BCE et TVA** : leur contrôle est écrit dans l'application et choisi par le code ; ils sont tous trois lignes système (code et activation verrouillés, expression en lecture seule). Garde-fou aussi dans les requêtes (`UtilsIdentifiantBelge.EstControleParAlthea`, V1.2.0).
- **Fiche Cabinet** : un type désactivé n'est plus proposé pour une nouvelle ligne, mais reste affiché sur les identifiants qui l'utilisent (la cellule restait vide) ; une valeur refusée rappelle le masque du type. **IBAN et BIC** : exemple de format dans le champ vide, IBAN présenté par groupes de 4 et BIC en majuscules à l'affichage et en sortant du champ ; IBAN enregistré sans espace (`UC_Cabinet` V1.6.0, `UtilsIdentifiantBelge.FormaterIban`, `IbanCompact`, `FormaterBic`). Lecture du référentiel en un seul endroit (`GestionCabinet` V1.2.0, `QueryCabinet` V1.2.0).
- **Hub Référentiels** : tuile Domaines retirée (les Domaines s'ouvrent par le menu principal de la Home — remarque de Joëlle) ; première ligne Thérapeutes, Organismes, Pays, Civilités (`UC_ReferentielHome` V1.5.0).
- Situations juridiques : chemin affiché « Administration > Situations juridiques » (`UC_SituationsJuridiques` V1.2.0).
- **Base de données — migration v2.47.0** (`migration_v2.47.0_types_identifiant_systeme.sql`, + ROLLBACK) : `BCE` et `TVA` marqués `est_systeme` (INAMI l'était déjà) ; données seulement. `VersionSchema.Attendue` = 2.47.0.
- **Documentation** : base de données, paramètres et référentiels, structure, interface, charte (icônes des types d'identifiant et des latéralités), ADR-31, plans de tests (Référentiels section 18 et 17.1, Cabinet 4.10, 4.14, 8.7 et 8.8), état du projet, ToDo (dont l'idée d'un hub Documents).

## 📅 20/10/2026 — Latéralités en référentiel ; modalités de paiement proposées en paramètre

- **Latéralités** (décision de Joëlle) : la liste de la fiche patient n'est plus écrite dans le code. Nouveau référentiel « Latéralités » (Référentiels, groupe Patient ; `UC_Lateralites`, `GestionLateralites`) : Droitier, Gaucher, Ambidextre au départ, d'autres valeurs possibles (gaucher contrarié…). La fiche patient lit la liste du référentiel ; une latéralité désactivée reste affichée chez le patient qui l'a (`UC_PatientFiche` V1.8.0, `UtilsSaisieReferentiels` V1.1.0).
- Hub Référentiels : tuile « Latéralités » dans le groupe Patient ; Civilités rejoint la première ligne (à côté de Pays), Types de rendez-vous le groupe Séances (`UC_ReferentielHome` V1.4.0).
- **Modalités de paiement proposées** à la création d'un dossier : paramètre « Modalités de paiement proposées » (Paramètres › Dossiers ; « À la séance » à la livraison) au lieu d'une valeur écrite dans le code (`UC_DossierFiche`).
- **Base de données — migration v2.46.0** (`migration_v2.46.0_lateralites_modalite_paiement.sql`, + ROLLBACK) : table `ref_lateralites` ; colonne `patients.id_lateralite` (clé étrangère), renseignée d'après les latéralités déjà saisies — toute valeur hors liste devient une ligne du référentiel, rien n'est perdu ; la colonne texte `patients.lateralite` n'est plus lue mais conservée jusqu'à vérification. Groupe `DOSSIERS`, paramètre `DOSSIER_MODALITE_PAIEMENT_DEFAUT`. `VersionSchema.Attendue` = 2.46.0 ; `reinit_tec_parametres.sql` (15 paramètres).
- **Documentation** : base de données, paramètres et référentiels, structure, plans de tests (Référentiels section 17, Patients PAT-82, Dossiers D-33), rapport d'audit § 9, ToDo.

## 📅 20/10/2026 — Plus aucun champ de date Windows (et filtre « Dernière connexion depuis » corrigé)

- Les trois derniers `DateTimePicker` passent au champ de date du kit (`AltheaDate` : saisie au clavier, calendrier, consultation lisible) : filtre « Dernière connexion depuis » des Utilisateurs (`UC_Utilisateurs` V1.7.0), dates des règles de quota (`UC_DomainesHome`), dates des tarifs rattachés à un domaine (`UC_LiaisonsPrixDomaines` V2.2.0).
- **Corrigé** (audit B7) : « Dernière connexion depuis » ne retenait que les comptes connectés le jour exact ; il retient désormais cette date et les suivantes.
- **Documentation** : Reference_UI § 8, plans de tests (Utilisateurs section 16, Domaines D-99 et D-100), rapport d'audit § 9, ToDo.

## 📅 20/10/2026 — Boutons inactifs : lisibles, et ils disent pourquoi

- **Lisibles** (relevé sur la vitrine) : un bouton désactivé garde un texte gris-vert foncé (contraste d'environ 3,9 pour 1 au lieu de 2) et une icône à 55 % ; il reste visiblement inactif (`AltheaBouton` V1.3.0, `AltheaDessin` V1.2.0).
- **Ils disent pourquoi** (relevé de Joëlle) : au survol d'un bouton inactif, une infobulle donne la raison (`RaisonInactif`, `DefinirDisponibilite`). Une infobulle Windows ne s'affiche pas sur un contrôle désactivé : le bouton écoute son parent. Premiers écrans : barres Séances et Paiements du dossier (dossier clôturé ou archivé, pas de sélection, séance non planifiée, non due, déjà reportée, encaissement réservé au SuperUser — `UC_DossierFiche` V1.12.0).
- Vitrine du kit : boutons désactivés avec une raison d'exemple (`TestKitAlthea` V2.2.0).
- **Documentation** : Reference_UI § 5.4 et § 7.1, charte, plans de tests (kit section 9, Dossiers D-31 et D-32), ToDo.

## 📅 20/10/2026 — Logo des domaines

- **Logo facultatif pour chaque domaine** (demande de Joëlle) : carte « Logo du domaine » à l'étape 1 de l'écran Domaines (Importer… / Retirer, en Modifier ou à la création), enregistré **avec** le domaine (Enregistrer), abandonné par Annuler, compté dans les modifications non enregistrées (`UC_DomainesHome` V1.10.0, `GardeModifications.AjouterValeur`).
- **Affiché** dans la convention du domaine (sous les règles, jusqu'à 180 × 80 : écran Domaines et fiche du dossier, `UC_SyntheseDomaine` V2.4.0), dans le bandeau du dossier (à droite du bloc Dossier, jusqu'à 120 × 52) et devant chaque domaine de la liste « Domaine » de la fiche du dossier, création comprise (jusqu'à 56 × 18, `UC_DossierFiche` V1.11.0). Des cadres en longueur et non des carrés : les logos sont souvent larges (relevé de Joëlle : en 18 × 18, illisibles).
- **Logo du cabinet lisible** dans le bandeau de la Home : réduit dans un cadre 180 × 40 au lieu d'un carré de 30 px, centré sur la ligne du nom (`Home` V1.14.0).
- **Rangement** : PNG réduit à 256 pixels au plus (proportions et transparence gardées), dans le dossier de données `Domaines/Domaine_{id}/Logo.png` — compris dans les sauvegardes, indépendant du libellé (`CheminsDomaineHelper`, `ArborescenceDonnees` V1.1.0) ; miniatures gardées en mémoire (`LogosDomaines`).
- **Images** : une nouvelle image est écrite avant que l'ancienne soit retirée (une copie ratée ne fait plus perdre l'image en place, audit § 4.5) ; réduction en haute qualité (`ImagesHelper` V1.1.0 : `Reduire`, `ReduireDans`, `SupprimerImage`).
- Liste des domaines de la fiche du dossier : membres posés avant la source (ordre recommandé, ToDo).
- **Base de données — migration v2.45.0** (`migration_v2.45.0_logos_domaines.sql`, + ROLLBACK) : colonne `ref_domaines.logo_fichier` (nom du fichier seul, NULL = aucun). `VersionSchema.Attendue` = 2.45.0.
- **Documentation** : base de données, ADR-35, Rules, Reference_UI, structure, utilitaires, guide des domaines, plans de tests (Domaines section 11, Dossiers D-28 à D-30), rapport d'audit § 9, ToDo.

## 📅 20/10/2026 — Configuration de connexion : test, enregistrement et messages (audit B1-B2, § 3.3, § 4.5)

- **Corrigé** : « Tester la connexion » affichait une erreur sans rien tester (bloc d'essai oublié) ; comme Enregistrer exige un test réussi, aucune configuration ne pouvait être enregistrée (`ConfigurationConnexion` V1.2.1).
- **Corrigé** : en gardant le mot de passe stocké (sans « Modifier »), l'enregistrement le remplaçait par un mot de passe vide chiffré ; plus de connexion au démarrage suivant.
- **Messages d'aide et d'erreur** (demande de Joëlle : la barre d'état seule laissait croire qu'il ne se passait rien) : champs manquants nommés ; échec du test avec sa cause et quoi vérifier (serveur injoignable, compte refusé, base inconnue, options invalides, mot de passe enregistré illisible sur ce poste) ; test requis ; enregistrement réussi ou impossible ; configuration illisible. Règle générale ajoutée (Reference_UI § 5.4).
- **Le test ne touche plus la connexion de l'application** : un essai raté depuis l'Administration la laissait pointer sur les valeurs testées ; la configuration est appliquée à l'enregistrement (`DatabaseManager` V1.2.0 : `TesterConfiguration`, délai de 8 s, cause journalisée sans mot de passe).
- Chaîne de connexion construite par `MySqlConnectionStringBuilder` : un mot de passe contenant « ; » ou « " » ne la casse plus (audit § 3.3).
- **Documentation** : suivi des corrections de l'audit (rapport § 9), plan de tests Login section 11 (11.1 à 11.12), Reference_UI § 5.4, ToDo.

## 📅 20/10/2026 — Correcteur d'orthographe dans les notes courtes

- **Commentaire d'un paiement et motif d'une annulation** : correcteur de Windows, comme dans l'éditeur (soulignement, suggestions et « Ajouter au dictionnaire » au clic droit), dans une nouvelle zone de texte du kit, `AltheaZoneTexte` (`PaiementEditor` V1.6.0, `DialogMotifAnnulation` V1.4.0). Ils restent en **texte simple** (décision de Joëlle) : Althéa complète elle-même le commentaire d'un paiement (« [Annulé le … : motif] », utilisation d'un avoir). Aucune modification de la base.
- Dans ces champs : collage en texte seul, raccourcis de mise en forme sans effet ; Entrée valide toujours la fenêtre d'annulation (Maj+Entrée : nouvelle ligne).
- **Code partagé** : réglage, langue du clavier et lignes du clic droit du correcteur dans `CorrecteurTexte` (nouveau, `Utils/UI`), utilisé par l'éditeur (`UC_RichTextEditor` V2.4.1, comportement inchangé) et par `AltheaZoneTexte`.
- **Confidentialité** : le motif d'une annulation n'est plus écrit dans le journal (`GestionPaiements` V1.2.0) ; il reste dans le commentaire du paiement.
- Ressources : icônes Couper / Copier / Coller de l'éditeur (`Editeur_couper`, `Editeur_copier`, `Editeur_coller`) pour le menu de la zone de texte.
- **Documentation** : ADR-45 (complété), Reference_UI § 7.24, charte, guide de l'éditeur, plan de tests (section 15, RTE-140 à RTE-148), structure, utilitaires, historique, ToDo.

## 📅 20/10/2026 — Éditeur de texte, temps 2 : correcteur d'orthographe

- **Correcteur de Windows dans tous les éditeurs**, en saisie (`UC_RichTextEditor` V2.4.0, `RichTextEditorHelper` V2.3.0) : mots inconnus soulignés en rouge pendant la frappe, dans la langue du clavier ; un texte chargé y est rattaché (le RTF le marquait en anglais). Orthographe seulement, pas de grammaire. Essai concluant sur le poste de Joëlle (ADR-45).
- **Clic droit sur un mot souligné** : suggestions, « Ajouter au dictionnaire » (dictionnaire de Windows de l'utilisateur) (`CorrecteurWindows`, nouveau). « Ignorer » essayé puis retiré : le mot retapé était de nouveau souligné, sans correction proposée.
- **Réglage** : Paramètres › Éditeur de texte › « Correcteur d'orthographe ».
- **Base de données — migration v2.44.0** (`migration_v2.44.0_correcteur_orthographe.sql`, + ROLLBACK) : groupe `EDITEUR`, paramètre `EDITEUR_CORRECTEUR_ORTHOGRAPHE` ; aucune structure modifiée. `VersionSchema.Attendue` = 2.44.0 ; `reinit_tec_parametres.sql` (14 paramètres).
- Fenêtre de test de l'éditeur : case « Correcteur » et diagnostic (clavier, langues prises en charge).
- **Documentation** : ADR-45, guide de l'éditeur (V2.5, temps 2 terminé), plan de tests (section 14, RTE-130 à RTE-137), Reference_UI § 7.23, base de données, paramètres, structure, utilitaires, historique, ToDo.

## 📅 19/10/2026 — Textes types : plusieurs usages, tous les éditeurs, écran des usages

- **Un texte, plusieurs usages** : cases à cocher dans Textes types (`UC_TextesTypes` V1.1.0) ; rien de coché = proposé partout ; colonne « Usages » dans la grille.
- **Tous les éditeurs ont un usage** : l'alerte de la fiche patient propose ses textes ; les commentaires sont séparés (contact, intervenant, thérapeute). Règle pour tout nouvel éditeur (Reference_UI § 7.23).
- **Écran « Usages des textes types »** (`UC_UsagesTextesTypes`, tuile du groupe Dossier) : libellé et ordre ; pas de création (`UC_ReferentielBase` V1.7.0 : `CreationPermise`).
- `GardeModifications` V1.1.0 : les listes à cocher comptent dans les modifications non enregistrées.
- **Base de données — migration v2.43.0** (`migration_v2.43.0_textes_types_usages.sql`, + ROLLBACK) : table `lia_textes_types_usages`, usages `ALERTE_PATIENT`, `COMMENTAIRE_CONTACT`, `COMMENTAIRE_INTERVENANT`, `COMMENTAIRE_THERAPEUTE` ; l'usage de chaque texte est repris dans la liaison, puis la colonne `ref_textes_types.id_usage_texte_type` et l'usage `COMMENTAIRE` sont supprimés. `VersionSchema.Attendue` = 2.43.0.
- **Documentation** : ADR-44 (complété), guide de l'éditeur V2.4, plan de tests (RTE-119 à RTE-124), Reference_UI § 7.23, base de données technique, paramètres et référentiels, structure, charte, ToDo.

## 📅 19/10/2026 — Grilles : traits « encadrés » sous les pastilles

- **Corrigé** (relevé par Joëlle) : sous la première ligne portant une pastille (« Système », états…), toutes les lignes suivantes paraissaient encadrées de gris. La pastille passait le dessin en anticrénelage et ne le rendait pas : la grille dessinait ensuite ses traits flous et plus épais (`UtilsDataGrid.DessinerPastilleCellule` V1.2.1).

## 📅 19/10/2026 — Éditeur de texte, temps 2 : textes types

- **Nouveau référentiel « Textes types »** (Référentiels, groupe Dossier ; `UC_TextesTypes`) : phrases et trames réutilisables, écrites dans un éditeur avec « Ouvrir en grand », chacune avec un **usage** (anamnèse, zones du dossier, notes de séance, évolution de séance, commentaires) ou proposée **partout** (ADR-44).
- **Dans les éditeurs** : bouton « Textes types » dans la zone insertion (`UC_RichTextEditor` V2.3.0, `UsageTextesTypes`) ; il propose les textes de l'usage de l'éditeur et ceux proposés partout, relus à chaque ouverture, et les insère au curseur avec leur mise en forme (`RichTextEditorHelper.InsererRtf`). Pas de bouton dans l'alerte de la fiche patient.
- **Base de données — migration v2.42.0** (`migration_v2.42.0_textes_types.sql`, + ROLLBACK) : tables `ref_textes_types` et `ref_usages_textes_types` (5 usages système) ; aucune table existante modifiée. `VersionSchema.Attendue` = 2.42.0.
- Icônes : tuile `texteType_*` (48 px), titre `textesTypes`, barre de l'éditeur `texteType_20_normal`.
- **Documentation** : ADR-44, guide de l'éditeur (V2.3), plan de tests (section 13, RTE-110 à RTE-118), Reference_UI § 7.23, base de données technique, paramètres et référentiels, structure, charte, historique, ToDo.

## 📅 19/10/2026 — Éditeur de texte, temps 2 : en-tête et pied de page des documents

- **Chaque page imprimée ou exportée** (anamnèse, zones du dossier ; impression, aperçu, PDF, Word) porte un **en-tête** — cabinet, praticien, titre du document, patient « Prénom NOM (P…) » — et un **pied de page** — mention, « Édité le … », « Page n / N ». Cabinet et praticien sont relus à chaque sortie (`EnTeteDocument`, nouveau ; `RichTextEditorHelper` V2.1.0 ; `UC_RichTextEditor` V2.2.0, `PatientDocument`).
- **Mention paramétrable** : carte « Documents imprimés » de l'écran Paramètres, « Confidentiel » par défaut, vide = pas de mention (`GestionParametres.GetTexte`).
- Les zones du dossier titrent leurs documents « nom de la zone — dossier D… » et reçoivent le patient (`DossierZoneEditor` V1.5.0, `UC_DossierFiche`).
- **Export Word en français** : le document prend la langue de Windows ; Word vérifiait l'orthographe en anglais (relevé par Joëlle).
- **Base de données — migration v2.41.0** (`migration_v2.41.0_mention_documents.sql`, + ROLLBACK) : groupe `DOCUMENTS` et paramètre `DOCUMENT_MENTION_CONFIDENTIALITE` ; aucune structure modifiée. `VersionSchema.Attendue` = 2.41.0 ; `reinit_tec_parametres.sql` mis à jour (13 paramètres).
- **Documentation** : guide de l'éditeur (V2.2), plan de tests (section 12, RTE-100 à RTE-107), Reference_UI § 7.23, ADR-42, base de données technique, paramètres et référentiels, structure, historique, ToDo.

## 📅 19/10/2026 — Éditeur de texte, temps 2 : « Ouvrir en grand »

- **Notes et commentaires en grand** (`UC_RichTextEditor` V2.1.0, `EditeurTexteAgrandi` nouveau) : un bouton à droite de la barre compacte (alerte, notes et évolution de séance, commentaires) ouvre le même texte dans une grande fenêtre, avec toute la barre (styles, listes, recherche…). **Valider** reporte le texte dans le champ, qui s'enregistre ensuite avec sa fiche ; fermer après une modification pose la question habituelle à trois choix.
- Chaque éditeur compact porte maintenant un titre (« Notes de séance », « Alerte / Notes »…), repris par la grande fenêtre.
- Nouvelles propriétés de l'éditeur : `AfficherSorties` (aperçu, impression, export masquables), `EstModifie`. Nouvelle icône `Assets/Editor_Ico/agrandir_20_normal.png`.
- **Documentation** : guide de l'éditeur (V2.1), plan de tests (section 11, RTE-94 à RTE-99), Reference_UI § 7.23, ADR-42, structure de la solution, ToDo (plan du temps 2).

## 📅 19/10/2026 — Modifications non enregistrées : on est prévenu avant de les perdre

- **Fiche patient** (`UC_PatientFiche` V1.7.0) : quitter la fiche (Retour, menu de Home, fermeture d'Althéa) ou l'onglet Anamnèse en cours de modification propose **Enregistrer / Quitter sans enregistrer / Rester** ; la fiche en création ou en modification est surveillée pour tous ses champs. L'anamnèse n'avertissait qu'au bouton Retour, la fiche jamais.
- **Fenêtres** Séance, Contact, Intervenant, Thérapeute (elles fermaient sans rien dire) et Zone du dossier (question Oui / Non) : même question à la fermeture (Annuler, croix, Échap) ; aucune en consultation ni sans modification.
- **Partout ailleurs, même comportement** (demande de Joëlle : des comportements identiques dans toute l'application) : fiche du dossier, Domaines (domaine et règle de quota), détail des référentiels (dans Home et en fenêtre), Cabinet et Paramètres (leur question Oui / Non devient la question à trois choix), fenêtres Paiement et Utilisateur.
- **Annuler** qui reste sur l'écran (fiches, détail, domaine, règle de quota, anamnèse, Cabinet, Paramètres) : « Abandonner les modifications … ? » — **Abandonner / Continuer la saisie**, seulement s'il y a des modifications. La fiche patient abandonnait sans rien demander.
- **Outil commun** `GardeModifications` (photo des champs, grilles modifiables comprises, comparée au départ ; une ligne par fenêtre) et `DialogChoix.DemanderAvantDepart` / `ConfirmerAbandon` (V2.2.0). ADR-43.
- **Domaines** (`UC_DomainesHome` V1.9.0, relevé par Joëlle) : **Modifier** n'est plus proposé que sur l'étape 1 (Général), comme sur la fiche du dossier ; cliqué depuis Tarifs, Catégories & quotas ou Natures, il renvoyait sans prévenir sur Général. Les étapes 2 à 4 disent qu'elles se modifient directement, sans passer par Modifier.
- **Documentation** : Reference_UI § 5.7 et § 5.8, Rules, ADR-43, plan de tests du kit (section 8, KM-01 à KM-20), plan de tests des domaines (D-89, D-90), structure de la solution, utilitaires, ToDo.

## 📅 19/10/2026 — Éditeur de texte refondu (temps 1)

- **Un seul éditeur, deux profils** (`UC_RichTextEditor` V2.0.0, ADR-42) : **Complet** pour l'anamnèse et les zones du dossier, **Compact** pour l'alerte, les notes et l'évolution de séance et les commentaires (contact, intervenant, thérapeute). `UC_RichTextEditorSimple` est supprimé.
- **Barre du kit** (`AltheaRenduBarre`, nouveau) : boutons Outil, 33 icônes redessinées (20 px, `Assets/Editor_Ico`, les 22 anciennes retirées), boutons désactivés réellement grisés.
- **Nouveau** : styles de paragraphe (Titre, Sous-titre, Normal, Citation, Petit) à la place de la police et de la taille libres ; **G** pour gras (Ctrl+G ou Ctrl+B) ; couleurs et surligneurs limités à la charte (`UITheme` V1.5.0) ; liste numérotée ; texte justifié ; date, date et heure, horodatage signé ; recherche et remplacement (Ctrl+F, Ctrl+H, F3) ; aperçu avant impression ; collage sans mise en forme ; menu du clic droit ; ligne d'état (mots, « Modifié », zoom).
- **Consultation** : l'anamnèse et les zones gardent une barre pour rechercher, copier, imprimer et exporter (la barre entière disparaissait) ; aucun raccourci ne modifie le texte.
- **Corrigé** : boucle des listes de police et de taille (la taille arrondie était réappliquée), mise en page ignorée à l'impression, Ctrl+I qui insérait une tabulation, mise en forme seule non vue comme une modification, `ShowToolbar` écrasé par la lecture seule, Échap qui fermait la fenêtre au lieu de la recherche ; messages en `DialogChoix` au lieu de `MessageBox` ; RTF illisible journalisé ; aucun chemin de fichier patient dans les journaux.
- **Documentation** : guide de l'éditeur et plan de tests réécrits (V2.0), ADR-42, Reference_UI (§ 7.23), documentation UI, charte graphique, processus 08 et 09, structure de la solution, ToDo.

## 📅 19/10/2026 — Pagination refondue ; aperçu du patient stabilisé

- **Barre de pagination** (`UC_Pagination` V1.1.0) : reprise dans le thème — boutons Outil du kit avec chevrons sauge (`Assets/Ico_divers/page*_24_normal`), « Page 2 / 5 », résumé « 21–40 sur 87 patients » (singulier ou pluriel, « Aucun patient ») ; navigation masquée s'il n'y a qu'une page ; plus de ligne coupée en bas de grille (en-tête, ascenseur et place des repères retirés du calcul) ; la page garde le premier élément affiché quand la grille change de taille. Elle prend aussi en charge le tri par en-tête.
- **Séances et paiements du dossier** (`UC_DossierFiche` V1.9.0) : grilles paginées ; les séances s'ouvrent sur la page qui contient aujourd'hui.
- **Aperçu de la liste des patients** (`UC_PatientHome` V1.5.1) : il se rechargeait en boucle (clignotement) ; il n'est plus relu que si le patient sélectionné change.
- **Documentation** : Reference_UI (§ 7.22), plans de tests du kit (KP-01 à KP-05) et des dossiers (D-24 à D-27), plan de refonte UI, ToDo.

## 📅 18/10/2026 — UI-9, étape 4 : référentiels et Domaines

- **Référentiels** (`UC_ReferentielBase` V1.5.0) : la case « Actif » et la colonne du cadenas deviennent une colonne **État** — pastille « Système » (bleu, avec son infobulle) ou « Inactif » (gris), rien pour une ligne normale ; ligne inactive atténuée ; ordre à droite. État porté par `ReferentielLigne.Etat` (V1.1.0). Vaut pour tous les référentiels simples.
- **Thérapeutes** (`UC_Therapeutes` V1.1.0) : même colonne État (« Inactif »), ligne atténuée.
- **Rattachements** tarif / domaine et nature / domaine (`UC_LiaisonsPrixDomaines`, `UC_LiaisonsNaturesDomaines` V2.1.0) : état en pastille — « Nature désactivée » en ocre (rattachée mais pas proposée à la saisie) ; rattachement clôturé, désactivé ou non proposé atténué. Familles `EtatLiaisonPrix` / `EtatLiaisonNature` dans les modèles (V1.1.0).
- **Domaines** (`UC_DomainesHome` V1.7.0) : état du domaine (Complet, À compléter, Inactif) et des règles de quota (`QuotaInamiRegle.Etat`, V1.2.0) en pastilles ; période sans règle « Sans règle » en ocre ; catégorie inactive en pastille grise, aussi dans « Gérer les catégories… » (`CategoriesQuotaEditor` V1.3.0).
- **Supprimés** : les 16 icônes `Assets/Tech_Ico` (OK, NO, OFF, Lock), leurs ressources et `UtilsIcons.IconOK / IconOFF / IconLock / IconNo` (`UtilsIcons` V1.2.0) ; aucune image embarquée n'en était la copie (contrôle pixel à pixel). ADR-12 marquée remplacée.
- **Documentation** : documentation UI, Reference_UI, charte graphique, Rules § 18, ADR-12, plans de tests des référentiels (§ 16) et des domaines (D-84 à D-88), plan de refonte UI, ToDo.

## 📅 18/10/2026 — Correction : erreur de tarif à l'ouverture d'une séance

- **Séance** (`SeanceEditor` V1.5.3) : chaque ouverture inscrivait dans les journaux « Erreur résolution tarif séance … StatutSeance to IConvertible ». Les listes Tarif, Nature et Statut recevaient leur source avant leurs membres d'affichage et de valeur : le recalcul du tarif, déclenché par la liste des statuts, lisait l'objet au lieu de l'identifiant. Ordre rétabli ; aucune donnée touchée (le tarif enregistré était relu juste après).

## 📅 18/10/2026 — UI-9, étape 3 : grilles d'administration

- **Utilisateurs** (`UC_Utilisateurs` V1.6.0) : état du compte en pastille — Actif, Verrouillé (avec une infobulle : nombre d'échecs, déverrouiller depuis la fiche), Désactivé — à la place des icônes PNG ; rôle Admin et SuperUser en pastille mauve ; compte désactivé atténué ; **tri par en-tête** ; « Dernière connexion » centrée. Règle de priorité (verrouillé d'abord) déplacée dans `UtilisateurApplication.EtatCompte` (V1.2.0).
- **État du système** (`UC_EtatSysteme` V1.1.0) : appréciation en pastille à trois tons — « À vérifier » passe à l'ocre, il était rouge comme « Problème » ; titres de section en repères titrés ; rapport copié inchangé (`GestionEtatSysteme` V1.2.0).
- **Journaux** (`UC_Journaux` V1.1.0) : pastille « Erreur » dans la colonne Niveau (le niveau en mention) au lieu de la ligne en rouge ; démarrages de session en repères titrés, toujours sélectionnables pour leur détail.
- **Historiques des sauvegardes et des restaurations** (`UC_Sauvegardes` V1.2.0, `DialogRestauration` V1.3.0) : résultat en pastille (Réussie, Échec, Interrompue) au lieu de la ligne en rouge ; **tri juste** par date, taille et nombre de fichiers (il triait les dates comme du texte).
- **Grilles à repères non triables** (État du système, Journaux) : un clic d'en-tête mélangeait les sections et les sessions (`UtilsDataGrid.DesactiverTri`, V1.9.0).
- **Documentation** : documentation UI, Reference_UI, structure de la solution, plans de tests utilisateurs (§ 15), administration (§ 5) et sauvegardes (§ 8), plan de refonte UI, ToDo.

## 📅 18/10/2026 — UI-9, étape 2 : grilles du patient et du dossier

- **Planning du jour** (`PlanningJourViewer` V1.3.0) : statut en pastille, comme dans la grille des séances ; **créneaux libres** affichés en repères pointillés « Libre · 10:20 – 14:00 » dans la plage de la journée paramétrée (pas pour un jour passé, rien avant l'heure qu'il est aujourd'hui, pas de trou plus court qu'une séance) ; heures centrées, tarif à droite ; erreur de chargement en `DialogChoix`.
- **Règle partagée** (`GestionSeances` V1.4.0) : une séance annulée ou reportée libère son créneau, pour le planning comme pour le contrôle de chevauchement (`OccupeSonCreneau`) ; calcul dans `GetPlanningJour`, lignes portées par `LignePlanningJour` (nouveau).
- **Détail des paiements d'une séance** (`DialogPaiementsSeance` V1.3.0) : colonne « État » en pastille, mêmes couleurs que le relevé du dossier ; ligne neutralisée en gris du thème, montant annulé barré (la police n'est plus recréée à chaque cellule).
- **Fiche patient** (`UC_PatientFiche` V1.6.0) : statut des dossiers en pastille (couleurs de la colonne « Suivi » des patients), dossier clôturé ou archivé atténué ; intervention terminée atténuée ; dates centrées.
- **Outil** : `UtilsDataGrid.DessinerRepereSurLigne` (V1.8.0), repère sur une ligne réservée, plein ou pointillé.
- **Documentation** : documentation UI, Reference_UI, structure de la solution, plans de tests séances (S-39 à S-43), paiements (P-54, P-55) et patients (PAT-80, PAT-81), plan de refonte UI, ToDo.

## 📅 18/10/2026 — UI-9, étape 1 : socle des grilles (tons sémantiques, double tampon)

- **Maquette « Grilles — états lisibles »** validée (tons sémantiques, Utilisateurs, État du système, Planning du jour avec créneaux libres, référentiel) ; ajoutée à `Reference_UI` avec la convention d'alignement.
- **Tons sémantiques** (`UtilsPastilles` V1.4.0) : `TonEtat` (OK, à surveiller, problème, inactif, information, rôle) et `Teinte(ton)`, pris dans les teintes existantes ; les familles des séances, paiements, dossiers et rendez-vous s'appuient sur eux là où leurs couleurs sont les mêmes (aucune couleur changée).
- **Socle des grilles** (`UtilsDataGrid` V1.7.0) : double tampon (défilement sans scintillement), polices partagées (elles étaient recréées à chaque grille sans être libérées), `AlignerCentre` / `AlignerDroite` (dates et heures centrées, montants à droite).
- **Correction** : une pastille de la couleur du fond de sa ligne (« Actif », « Réalisée », « OK » sur une ligne alternée ou sélectionnée) perdait sa forme une ligne sur deux ; toute pastille pleine porte désormais un fin contour dans sa teinte, sur tous les fonds (`DessinerPastilleCellule` V1.2.0).
- **Documentation** : Reference_UI (maquette, tons, conventions, outils), charte graphique, plan de tests du kit (section 6), plan de refonte UI (UI-9), ToDo.

## 📅 18/10/2026 — Nettoyage UI-8, catégorie « docs »

- **Liens corrigés** : 14 liens vers des documents déplacés (guide de licence, documentation et historique du RichTextEditor, décisions d'architecture, architecture technique, dictionnaire de la base, `TherapeuteEdition`) ; 2 lignes vers des fichiers disparus retirées (Bibliothèque de la documentation, Process).
- **Supprimés** : les 51 diagrammes de la base (`Docs/Database/Diagrams`, 14/06/2026), périmés (25 tables sur 45) et affichés nulle part ; le dictionnaire `Database_technique.md` et la sauvegarde de structure restent la référence.
- **Retirés du suivi Git et ignorés** : `desktop.ini` et `settings.VisualStudio.json`, propres au poste (chemins personnels).
- **Gardés, par décision** : les 79 liens vers les captures d'écran supprimées le 16/10, en attente des captures de la refonte UI (ToDo).
- **Documentation** : dictionnaire de la base, bibliothèque de la documentation, plan de refonte UI (avancement et mesures UI-8), ToDo.

## 📅 18/10/2026 — Sécurité : clé de licence Syncfusion hors du dépôt

- **Clé retirée du code** : `Program.vb` (V1.1.0) contenait en clair la clé Syncfusion (et une ancienne en commentaire), versionnée. Les deux restent dans l'historique Git : à renouveler chez Syncfusion.
- **Saisie dans l'application** : Administration > Paramètres, carte « Licences — propre à ce poste » (Admin ; `UC_Parametres` V2.1.0). La clé s'enregistre avec le reste de la page (**Modifier / Enregistrer**), vérifiée avant toute écriture ; elle n'est jamais réaffichée — une pastille donne l'état (définie, absente, illisible).
- **Stockage** : `%APPDATA%\Althea\althea.syncfusion.json`, chiffrée DPAPI pour le compte Windows courant (`LicenceSyncfusion`, nouveau, sur le modèle du mot de passe des sauvegardes), jamais en base ni dans les journaux.
- **Démarrage** : `GestionLicences` (nouveau) remet la clé à Syncfusion (`AppStartupManager` V1.3.0) ; sans clé, l'application démarre normalement et seuls les exports portent l'avertissement de licence. Une clé saisie sert aussitôt, sans redémarrage.
- **État du système** (`GestionEtatSysteme` V1.1.0) : ligne « Licence Syncfusion (exports PDF et Word) ».
- **Documentation** : ADR-41, guide de licence Syncfusion réécrit, sécurité, architecture technique, structure de la solution, documentation UI (Paramètres, État du système), plan de tests Paramètres (section 9), état du projet, ToDo.

## 📅 18/10/2026 — Nettoyage UI-8, catégorie « code »

- **Supprimés** (audit des types déclarés et de leurs usages) : l'ancien écran `UC_Domaines` (+ `.resx`, archivé et exclu de la compilation depuis le 02/10/2026, avec ses lignes du `.vbproj`) ; `ApplicationEvents.vb` (modèle vide de Visual Studio, inutile avec le `Sub Main` de `Program.vb`) ; `Utils/UI/UtilsString.vb` (fonctions jamais appelées).
- **Commentaires** qui citaient `UC_Domaines` en exemple mis à jour : `Home` V1.13.2, `UC_ReferentielHome` V1.1.2, `UC_ReferentielBase` V1.4.1, `UC_DomainesHome` V1.6.1, `ReferentielLigne` V1.0.1.
- **Gardées** : les fenêtres de test de l'Accueil (mode Debug seulement).
- **Relevé de sécurité** : la clé de licence Syncfusion est écrite en clair dans `Program.vb` ; correction prévue à part (ToDo).
- **Documentation** : documentation UI, structure de la solution, utilitaires transverses, processus, Rules, plan de tests Référentiels, checklist, plan de refonte UI, ToDo.

## 📅 18/10/2026 — Nettoyage UI-8, catégorie « resx »

- **Ancien fond étiré des UserControls** (`Fond_1000x770_FeuilleCoupee1`) retiré des Designers de 11 écrans (la Home le retirait déjà à l'exécution), puis ressource et fichier supprimés.
- **Fenêtre d'ajout « en passant »** d'un référentiel (`ReferentielModalHost` V1.2.0, bouton [+]) : la vue hébergée reçoit le fond du kit (`AltheaFondFenetre`, beige clair et feuille dessinée) ; elle montrait encore l'ancien fond étiré. `Home` V1.13.1 : commentaire à jour.
- **Gardés, par décision** : les icônes de boutons intégrées en double dans les `.resx` (aperçu dans le Designer) et les familles d'icônes complètes (`_normal`, `_hover`, `_disabled`) — règle ajoutée à la charte.
- **Documentation** : charte graphique, documentation UI (`ReferentielModalHost`), décisions d'architecture, plan de refonte UI, ToDo.

## 📅 17/10/2026 — Nettoyage UI-8, catégorie « assets »

- **Assets supprimés** (audit des trois chemins de chargement : nom sur disque, `My.Resources`, images intégrées par le Designer) : PNG de statut de l'ancienne grille Patients, GIF de l'ancien `DialogChoix`, fonds arrondis `ui-backgrounds`, anciens fonds étirés (`Fond_1000x800_Feuille4`, `Fond_500x400_Feuille4`, `Fond_480x240`, `Althea_Bandeau_Haut_Trans`, `Fond_icone_Transp`), icônes jamais branchées. 20 ressources retirées de `Resources.resx` ; 146 entrées mortes retirées d'`Althea.vbproj` ; chemin absolu de `.editorconfig` rendu relatif. `Assets` : 356 → 305 fichiers suivis. Les PNG sources d'images intégrées par le Designer sont gardés (vérification au pixel).
- **Photo de la praticienne** (`Assets/Praticien`) retirée du suivi Git et ignorée (donnée personnelle) ; elle reste dans l'historique.
- **Corrections** : tuile Thérapeutes sans survol ni état désactivé (`therapeute_hover/disabled` renommés `therapeutes_*`) ; `UC_AdminHome` V1.5.1 — tuile Cabinet sur l'icône `cabinet_normal` (elle reprenait celle des thérapeutes), tuile Utilisateurs sur `utilisateurs_normal` (elle visait un fichier inexistant) ; `UC_ReferentielHome` V1.1.1 — tuiles Natures séance (`natureSeance_normal`) et Prix séance (`tarifSeance_normal`) : leur `Tag` visait un fichier absent, d'où l'absence de survol.
- **Documentation** : charte graphique (sections des images supprimées retirées, ressources des icônes de titre corrigées, règle « fichiers sources »), Rules (habillage par le kit), Interface utilisateur, Reference_UI, décisions d'architecture, documentation UI, plan de refonte UI (avancement UI-8), ToDo.

## 📅 17/10/2026 — Écran Patients, étape 5 : panneau d'aperçu

- **Panneau d'aperçu** à droite de la grille (`UC_ApercuPatient`, nouveau ; `UC_PatientHome` V1.5.0 ; maquette « Aperçu patient » validée) : ouvert par défaut, rempli au clic sur une ligne — identité (photo ou initiales, âge), alerte en entier, séance du jour avec « Notes de la séance », 2 prochaines et 3 dernières séances (présence des notes, jamais leur texte ; pastille de paiement ou absence ; double-clic = ouvrir la séance), dossier et quota INAMI, finances, contacts et entourage, place réservée aux documents récents. Boutons « Ouvrir la fiche » / « Ouvrir le dossier DO… » ; « Masquer / Afficher l'aperçu » ; Âge, Dernière séance, Abs. et Téléphone masqués quand le panneau est ouvert ; patient resélectionné après l'ouverture d'une séance.
- **Métier** : `GestionApercuPatient` / `ApercuPatient` (nouveaux) assemblent l'aperçu à partir des modules existants, sans requête propre ; `GestionDossiers` V1.2.0, `ChoisirDossierDominant` (codes de statut repris des constantes).
- **Documentation** : documentation UI (nouvelle section `UC_ApercuPatient`), structure de la solution, plan de tests Patients (PAT-72 à PAT-79), ToDo (5b : mémoriser l'état du panneau, encaisser depuis l'aperçu, documents récents), état du projet.

## 📅 17/10/2026 — Grille Patients, étape 4 : icônes d'état dessinées

- **Icônes de statut de dossier** dessinées par le code (`UtilsIcons` V1.1.0, maquette « Icônes d'état ») dans le style d'`AltheaIconeEtat` et aux couleurs de la pastille de statut : ▶ vert-de-gris (Actif), ‖ ocre (En pause), coche et boîte d'archive gris foncé (Clôturé, Archivé), cercle pointillé gris (Sans dossier — l'octogone « ! » magenta faisait penser à une alerte) ; liste « Dossier » de l'écran Patients, icônes à 18 px nettes (`UC_PatientHome` V1.4.2).
- **Polices de la grille Patients** : nom, prénom et en-têtes reviennent à la taille des autres grilles (Calibri 10, en-têtes 10 gras sur une ligne).
- `IconPatientActif`, `IconPatientEnCours`, `IconPatientNonEnCours` retirés ; neuf PNG devenus inutiles notés pour le nettoyage UI-8.
- **Documentation** : charte, documentation UI, plan de tests Patients (PAT-71), ToDo.

## 📅 17/10/2026 — Grille Patients, étape 3 : résumé cliquable et filtres

- **Résumé cliquable** au-dessus de la grille (`UC_PatientHome` V1.4.0) : « N aujourd'hui », « N sous 7 jours », « N suivis sans rendez-vous », « À régler X € », « Avoir X € », « N avec absences », « N alertes » — chaque pastille filtre la grille, un second clic revient à tous ; les nombres suivent la recherche et les filtres.
- **Filtre « Dossier »** limité au statut du dossier dominant (Tous, Actif, En pause, Clôturé, Archivé, Sans dossier), libellés sans « Dossier : » ; élément choisi dessiné sur une ligne (il se coupait en deux), liste ouverte aux couleurs du kit.
- **Patients inactifs masqués par défaut** (case « Afficher les inactifs ») ; la barre d'état signale les inactifs masqués qui correspondent à la recherche. Situation et case restaurées au retour d'une fiche.
- **`ResumeFiltre`** V1.1.0 sorti de `UC_DossierFiche` (V1.8.1) dans `UI/Controls/Communs`, partagé par les grilles du dossier et la grille Patients ; une pastille choisie qui tombe à zéro reste affichée.
- **Documentation** : documentation UI, structure de la solution, plan de tests Patients (PAT-63 à PAT-70), ToDo.

## 📅 17/10/2026 — Grille Patients : raccourci vers la séance

- **Prise de notes depuis la liste** (demande de Joëlle ; `UC_PatientHome` V1.3.0) : double-clic sur « Prochain RDV » (séance du jour, à venir, ou du jour déjà passée) ou sur « Dernière séance » = la séance s'ouvre dans la fenêtre Séance, en consultation seulement si le dossier est clôturé ou archivé ; ailleurs, la fiche patient comme avant ; infobulle « Double-clic : ouvrir la séance SE… ».
- **Données** : `SelectPatientsListe` V2.5.0 (identifiant, code et dossier des deux séances — `QueryPatients` V1.6.0), `PatientListeItem` V1.3.0 (`RepereSeance`, `SeanceDuRendezVous`), `GestionPatients` V1.3.1.
- **Documentation** : documentation UI, plan de tests Patients (PAT-60 à PAT-62), ToDo (étape 2b).

## 📅 17/10/2026 — Grille Patients, étape 2 : la nouvelle grille

- **Grille Patients** (`UC_PatientHome` V1.2.0 et Designer) : colonnes Code, Nom, Prénom, Âge, Suivi, Prochain RDV, Dernière séance, Paiements, Abs., Alerte, Téléphone — Naissance, NISS, Email, Photo, Actif et Dossiers retirés de l'affichage (NISS et email restent dans la recherche) ; ordre par défaut en trois groupes séparés par une marge titrée (attendus aujourd'hui, à venir, sans rendez-vous à venir) ; ligne d'un patient attendu aujourd'hui teintée ; patient inactif grisé ; pastilles Suivi, Prochain RDV (« Aucun RDV » au trait), Paiements, absences ; icône d'alerte avec le texte en infobulle ; infobulles par colonne.
- **« En pause » en ocre** partout (`StatutPause`), y compris la pastille de l'en-tête du dossier : l'orange reste aux absences (`UtilsPastilles` V1.3.0, `UITheme` V1.4.0 : `StatutPause`, `StatutAujourdhui`, `LigneAujourdhuiBack`).
- **Kit** : `UtilsDataGrid` V1.6.0, pastille au trait (`contour`) ; `PatientListeItem` V1.2.0, textes et familles d'affichage (`APhoto`, `StatutPaiementAffichage`, `DossierAffichage` retirés).
- **Documentation** : documentation UI, charte, Reference_UI, plan de tests Patients V1.4 (PAT-50 à PAT-59), ToDo.

## 📅 17/10/2026 — Grille Patients, étape 1 : les rendez-vous du patient

- **Maquette « Patients — proposition »** et décisions de Joëlle : la grille Patients devient le hub de la journée (prochain rendez-vous, séance du jour, dernière séance, suivi en pastilles, résumé cliquable, puis panneau d'aperçu) ; plan en cinq étapes dans le ToDo (C1).
- **Données** (`QueryPatients` V1.5.0, `SelectPatientsListe` V2.4.0 ; `PatientListeItem` V1.1.0 ; `GestionPatients` V1.3.0) : prochain rendez-vous (première séance due pas encore passée, tous dossiers), dernière séance où le patient a été vu (absence non excusée exclue), séances du jour, dossiers au statut Actif ; helpers `ASeanceAujourdhui`, `ProchaineSeanceEstAujourdhui`, `EstSuiviSansRendezVous`, `Age`. Rien ne change encore à l'écran (étape 2).
- **`DbHelper`** V1.1.0 : `LireTimeNullable` partagé (retiré de `GestionSeances`, V1.3.1).

## 📅 17/10/2026 — Clôture d'un dossier : règles et état visible

- **Clôture contrôlée** (décision de Joëlle du 17/10/2026 ; `GestionDossiers` V1.1.0, `ControleClotureDossier` nouveau, `UC_DossierFiche` V1.8.0) : un dossier ne se clôture plus tant qu'une séance due est **à venir**, payée d'avance ou non — le message cite les séances et invite à les annuler (un paiement reçu devient un avoir du patient) ; des séances passées **restant à régler** sont signalées dans la confirmation (encadré, bouton « Clôturer quand même »). `ChangerStatutDossier` refait le contrôle bloquant ; un refus métier s'affiche dans une fenêtre. La mise en pause reste toujours possible ; l'archivage, qui part d'un dossier clôturé, est couvert.
- **État du dossier visible** : pastille de statut colorée — vert-de-gris actif, ocre en pause, gris foncé texte blanc clôturé ou archivé (`UtilsPastilles` V1.2.0, `TeinteStatutDossier`) ; bandeau d'information sous l'en-tête d'un dossier clôturé (« séances et paiements en consultation seulement », comment le rouvrir) ou archivé (« archivage définitif ») — `UC_DossierFiche.Designer` V1.4.0.
- **Codes de statut** `ACTIF` / `PAUSE` / `CLOTURE` / `ARCHIVE` : une seule définition publique dans `GestionDossiers`, reprise par `Dossier` V1.1.0 (`EstEnPause` ajouté) et `UC_DossierFiche` (constantes privées retirées), puis par `DossierListeItem` V1.0.1, `DossierStatutEditor` V1.3.1, `UC_PatientHome` V1.1.1 et `UtilsIcons` V1.0.2 ; seules les requêtes SQL de `QueryPatients` les écrivent encore en clair (commentaire de renvoi).
- **Documentation** : guide séances et paiements (§ 5.9), Concepts (Dossiers), Process 18, documentation UI, Reference_UI, charte, structure de la solution, plans de tests Dossiers V1.3 (D-19 à D-23 validés) et Paiements V1.8 (étapes 5 à 7 validées, P-15 facultatif ouvert), ToDo, état du projet.

## 📅 17/10/2026 — Bandeau et grille Patients : le patient dans son ensemble

- **Bandeau du dossier** (`UC_DossierFiche` V1.7.0, décision de Joëlle « A ») : montants dus du dossier suivis du total du patient s'il diffère ; avoir **du patient** (tous dossiers), avec la part du dossier (« (autre dossier) », « dont … ici ») ; détail par dossier en infobulle. Calcul unique `GestionSeances.CalculerSoldesPatient` (`SoldeDossier`, nouveau) aux règles des grilles ; l'ancien calcul par codes `REALISEE` / `ABSENTE` écrits en dur est retiré (une séance passée restée « Planifiée » compte désormais, comme dans l'onglet Séances).
- **Grille Patients** (`QueryPatients` V1.4.0, `UC_PatientHome` V1.1.0) : le montant dû suit la même règle (heure de fin comprise) ; infobulle « tous dossiers », avoir rappelé même quand un montant est dû (la cellule reste un résumé).
- **Annulation d'un règlement par avoir** (`GestionPaiements` V1.1.0, `UC_DossierFiche`) : refus annoncé par un message avant la saisie du motif (il ne figurait que dans la barre d'état) et maintenu même si le mode « Avoir » a été désactivé ; tout refus ou échec d'annulation s'affiche dans une fenêtre.
- **Documentation** : guide (§ 4, § 7), documentation UI, Concepts, Rules, ToDo (« Montant dû » soldé, reste le code `ABSENTE`), plan de tests Paiements V1.7 (P-57 à P-59 ; étape 4 validée).

## 📅 17/10/2026 — Codes des séances et paiements visibles

- **Grille des séances** : colonne « Code » rétablie (les dates seules ne distinguent pas deux séances).
- **Fenêtres du dossier** : le sous-titre de l'en-tête nomme l'élément par son code — séance (modification, report), paiement (encaissement, annulation, réaffectation, détail des paiements), statut du dossier ; rappels et listes de choix commencent par le code (`SeanceNonSoldee.Libelle`, `SeanceListeItem.LibelleReaffectation`). `UC_DossierFiche` V1.6.4, `SeanceEditor` V1.5.2, `PaiementEditor` V1.4.0, `DialogReportSeance` V1.5.0, `DialogMotifAnnulation` V1.3.0, `DialogPaiementReaffectation` V1.3.0, `DialogPaiementsSeance` V1.2.0, `DossierStatutEditor` V1.3.0.
- **Règle** : Reference_UI § 5.6 et Rules (« fenêtre sur un élément existant : nommer par le code ») ; ToDo pour les autres écrans. Plan de tests Paiements V1.6 (P-56 ; étapes 0 à 3 validées).

## 📅 17/10/2026 — Paiements : retours du premier passage de tests

- **Relevé des paiements** (`PaiementListeItem` V1.4.0, `UC_DossierFiche` V1.6.3) : le dû en attente que l'application clôt d'elle-même (séance réglée, devenue non due ou reportée) n'apparaît plus comme une ligne « Paiement annulé » après chaque encaissement ; il reste visible, statut « Clos », dans **Détail paiements** (« Paiement attendu clos — séance réglée »). Le filtre « Annulés » ne montre plus que les encaissements annulés.
- **Fenêtre Paiement** (`PaiementEditor` V1.3.0) : une saisie refusée (séance, montant nul ou supérieur au reste dû, mode) est expliquée dans la fenêtre — phrase rose sous les champs, montant teinté — et plus seulement dans la barre d'état.
- **Documentation** : guide (§ 3, 5.4, 9.10), documentation UI, Process, ADR-25, Rules, charte, plan de tests Paiements V1.5 (parcours recommandé ; P-54, P-55).

## 📅 17/10/2026 — Séances et paiements : montants cohérents, guide unique

- **Montant d'une ligne de paiement = montant affecté à la séance** (`PaiementListeItem` V1.2.0, `MontantLigne`) : un paiement réparti par une réaffectation apparaît sur une ligne par séance, chacune avec sa part ; le résumé « Encaissé » ne compte plus deux fois le même argent. Avoir en partie utilisé : « Avoir · 15 € » (reste) ; entièrement utilisé : « Avoir utilisé → S… », ligne grisée ; infobulle « Paiement de X €, dont Y € sur cette séance » et mentions d'utilisation d'avoir.
- **Avoir : une seule définition** (correctif) : l'encaissement resté sur une séance **reportée** compte comme avoir partout — grille Patients (`QueryPatients` V1.2.0) et avoirs proposés à la réutilisation (`QueryPaiements` V1.1.0) l'excluaient (code `REPORTEE` écrit en dur), alors que le bandeau, la grille des séances et le message de l'application le comptaient.
- **Reliquat de réaffectation** (correctif, `GestionSeances` V1.2.0) : l'excédent qui ne trouve aucune séance due reste sur la séance d'origine (avoir ordinaire) au lieu d'être accroché à la séance de remplacement, où il disparaissait de tous les totaux. Compte rendu commun (`ResultatReaffectation.CompteRendu`), affiché aussi depuis l'onglet Séances (`SeanceEditor` V1.5.1, `UC_DossierFiche` V1.6.1).
- **Trop-perçu d'une séance encore due = avoir** (décision de Joëlle, option A ; `SeanceListeItem` V1.2.0, `QueryPatients` V1.3.0, `QueryPaiements` V1.2.0, `PaiementListeItem` V1.3.0, `UC_DossierFiche` V1.6.2) : quand le montant d'une séance encaissée est baissé sous l'encaissé, la part en trop est comptée en avoir partout (bandeau, grilles, grille Patients, réutilisation) — « Réglée · avoir 10 € », solde « +10,00 », filtre Avoirs des paiements ; jusqu'ici la confirmation l'annonçait, mais aucun total ne la comptait.
- **Onglets du dossier** : bouton « Annuler l'encaissement » (il s'intitulait « Supprimer paiement ») ; « Encaisser séance » inactif sur une séance soldée, comme prévu par la règle.
- **Documentation** : nouveau **Guide séances et paiements** (`Docs/Guide/Guide_Seances_Paiements.md`) — utilisation des deux onglets et règles de maintenance ; il remplace `Releve_Consequences_Seances_Paiements.md`, supprimé. Mises à jour : Concepts métier, Process (19, 20, 20b), ADR-26 (évolution), ADR-25, Rules, documentation UI, charte, Bibliothèque, plan de tests Paiements V1.3 (P-48 à P-52), ToDo (« Montant dû » : trois définitions à réunir), état du projet.

## 📅 17/10/2026 — Grille des paiements en relevé chronologique

- **Grille des paiements** (`UC_DossierFiche` V1.6.0, maquette « Paiements — A ») : un paiement par ligne, le plus récent en haut ; colonnes Date (« — » pour un paiement attendu), Mouvement, Pour (séance : code · jour date · nature), Mode, Montant, **État** en pastille aux couleurs de la grille des séances (soldée, payée d'avance, reste dû, à régler, en attente, conservé en avoir, annulé, reporté) ; ligne annulée en gris, montant barré ; code, référence et phrase complète du mouvement en infobulle.
- **Fin du regroupement par séance** (lignes d'en-tête repliables) : le tri par en-tête est rétabli.
- **Résumé cliquable** : Encaissé, En attente, Avoir disponible, Reste à régler (avoir et reste calculés sur les séances, comme l'onglet Séances) ; nouveau filtre « À régler ». Le résumé des deux onglets est construit par une même classe (`ResumeFiltre`) ; le changement de filtre au clic est différé (la pastille cliquée était détruite pendant son propre événement).
- **`PaiementListeItem`** V1.1.0 : `MouvementCourt`, `PourAffichage`, `FamilleEtat` / `EtatAffichage` / `MentionEtat`, `EstARegler`, `SeancePassee` (renseignée par l'écran depuis la séance), `DateMouvement` ; `IndicateurEncaisse`, `SeanceAffichage`, `ResteDuAffichage` et les lignes d'en-tête de groupe retirés.
- **Correctif** : un encaissement laissé sur une séance **reportée** (réaffectation refusée) est désormais traité comme un avoir dans la grille des paiements (filtre Avoirs, pastille « Conservé en avoir »), comme le disent le message de l'application et l'avoir du bandeau ; il apparaissait auparavant comme « séance soldée ».
- **Documentation** : documentation UI, Process, Rules, ADR-25 (évolution), Reference_UI (illustration `grille_paiements.png`), charte, plan de tests Paiements V1.2 (P-38 à P-47), ToDo (tests approfondis des paiements ; « Montant dû » du bandeau à aligner sur le reste à régler), état du projet.

## 📅 17/10/2026 — Grille des séances lisible au premier coup d'œil

- **Maquettes** (17/10/2026) : grille des Séances retenue telle que proposée ; Paiements en relevé chronologique (variante A, à réaliser ensuite).
- **Grille des séances** (`UC_DossierFiche` V1.5.0) : 10 colonnes au lieu de 15 ; **statut** et **paiement** en pastilles colorées par famille (à venir, réalisée, absence, non due ; réglée, à régler, prévue, avoir) ; **solde** unique (− dû, + avoir) ; lignes non dues atténuées ; ligne **« Aujourd'hui »** entre l'avenir et le passé ; **résumé cliquable** au-dessus de la grille (nombres et montants du dossier, filtre rapide) ; code, tarif, période et quota en infobulle ; filtres « Réalisées » et « Avec avoir ».
- **`SeanceListeItem`** V1.1.0 : familles d'affichage (`FamilleStatut`, `FamillePaiement`), textes des pastilles, solde, mention de report ; `EtatPaiement` et `AvoirAffichage` retirés.
- **Kit** : `UtilsDataGrid` V1.5.0 (pastilles dans les cellules, séparateur titré, état du tri) ; nouveau module `UtilsPastilles` (couleurs par famille).
- **Documentation** : documentation UI, Reference_UI (liste à états, illustration), charte, plan de tests Séances V1.2 (S-30 à S-38), ToDo.

## 📅 16/10/2026 — UI-7 : champ d'heure du kit

- **`AltheaHeure`** (nouveau contrôle, maquette « AltheaHeure », liste A retenue) : même champ arrondi que la date ; saisie au clavier avec « : » placé seul (9, 930, 0930, 9:30, 9h30) ; heure impossible en rose ; Haut / Bas au pas ; **liste déroulante des heures proposées**, ouverte sur l'heure du champ, qui rappelle la **durée** pour une heure de fin (`HeureReference`) ; croix facultative (`AfficherCroix`) ; lecture seule et désactivé lisibles.
- **Fenêtre Séance** (V1.5.0) : Début / Fin en `AltheaHeure` (`hreDebut`, `hreFin`, format hh:mm au lieu de HH:mm:ss), heures proposées et pas de la grille paramétrée ; « la fin suit le début » inchangé.
- **Reporter la séance** (V1.4.0) : mêmes champs à la place des `DateTimePicker` Windows.
- **`UtilsControls`** V1.4.0 : `DefinirLectureSeule` traite `AltheaHeure` ; **vitrine du kit** V2.1.0 : ligne « Heure ».
- **Documentation** : Reference_UI (§ 5.2 et § 7.10, nommage `hre`), charte, Rules, documentation UI, plan de tests du kit (§ 5, KH-01 à KH-12) ; ToDo : trois dates encore en `DateTimePicker` (Utilisateurs, règles de quota, tarifs liés) ; infobulle expliquant pourquoi un bouton est inactif ; grille des Séances à revoir avec celle des Paiements (lisibilité au premier coup d'œil).
- **Liste des heures** entièrement dessinée, avec un ascenseur fin aux couleurs du kit (glissable) au lieu de la barre grise de Windows.

## 📅 16/10/2026 — UI-7 : écran Paramètres sur une page

- **`UC_Parametres` V2.0.0** (maquette « Paramètres — A ») : plus d'onglets, de grille ni de panneau de détail — une seule page qui défile, **une carte par groupe**, chaque paramètre sur une ligne (libellé, valeur, bornes, description en aide sous la valeur) ; pastilles « Admin » (paramètre fermé au SuperUser, en mode Admin) et « Désactivé » ; clé technique en infobulle du libellé (Admin).
- **Un seul Modifier pour la page** ; **Enregistrer** valide puis écrit les valeurs changées **en une transaction** (tout ou rien) ; Annuler recharge la page ; confirmation avant de quitter l'écran avec des valeurs non enregistrées (`IUnsavedChangesAware`). Molette sans effet sur les nombres.
- **Base de données (v2.40.0, + ROLLBACK)** : table `tec_groupes_parametres` (titre et ordre des cartes : Dossier de données, Séances, Sauvegardes, Cabinet, Sécurité, Technique), clé étrangère depuis `tec_parametres.groupe_parametre`, plus de groupe par défaut `GENERAL` ; `VersionSchema.Attendue` = 2.40.0 ; `reinit_tec_parametres.sql` vérifie la table des groupes. Migration essayée sur une base MariaDB 10.11 de test chargée avec le schéma (rejeu, clé étrangère, retour arrière, réapplication).
- **Couche métier** : `GestionParametres.UpdateValeursParametres` (transaction) remplace `UpdateValeurParametre` ; `ParametreApplication` porte le titre et l'ordre du groupe ; listes triées par ordre de groupe (`QueryParametres` V1.4.0).
- **Kit** : icône Outil `dossier_24_normal` (bouton Parcourir des chemins) ; `UITheme` V1.3.0 : couleurs `Dynamic*` des anciens onglets retirées.
- **Documentation** : Database_technique, Parametres_Referentiels, documentation UI (fiche `UC_Parametres` réécrite), Processus 03, ADR-36 (complément), Rules § 14, Reference_UI (page de réglages), charte, plan de tests `UC_Parametres` V2.0 (53 tests), plans et ToDo.
- **Relevé (ToDo, décision de Joëlle)** : lot « Journal des modifications — qui fait quoi » remonté avant la mise en service (table en base, valeurs avant / après pour les données d'administration) et révision complète des journaux techniques de tous les écrans (manques constatés : Domaines, catégories de quota, Cabinet).

## 📅 16/10/2026 — UI-7 : panneau « Détail » des référentiels

- **`UC_ReferentielBase` V1.4.0** : la carte « Détail » des 16 écrans de référentiel suit la règle des fenêtres — libellé à gauche (gras sauge), champ à droite (Calibri 11, fond blanc en saisie), une ligne tous les 34 px ; Code, Libellé, Ordre, champs propres, puis **État en dernier** (case du kit).
- **Consultation lisible** : texte normal sur fond teinté au lieu de champs grisés ; le verrou des lignes système (code, activation) reste.
- **Correctif** : Tarifs et Natures de séance clignotaient au chargement — la grille levait des changements de sélection en série (pendant son remplissage, puis à chaque retour du focus quand le panneau des domaines activait ou désactivait ses boutons), et chacun rechargeait les données liées depuis la base, ce qui en relançait un autre (boucle entre les deux grilles, mise en évidence par des traces). Désormais : signaux ignorés pendant le remplissage, et un changement de sélection sur la même ligne ne recharge plus rien.
- **Correctif (tous les écrans)** : `NavigationManager` V1.2 — chaque écran se mettait en place sous les yeux de l'utilisateur (et le panneau Détail des référentiels sautait vers la droite) : ajouté masqué, il faisait son chargement à la taille du Designer avant de se replacer, et Windows redessinait les parties déjà créées pendant les lectures en base. Désormais l'écran reçoit sa taille définitive avant d'être ajouté, et l'affichage de la zone de contenu est gelé pendant toute la navigation puis redessiné en une fois (nouvel utilitaire `UtilsAffichage` : `SuspendreDessin` / `ReprendreDessin`, message Windows `WM_SETREDRAW`).
- **Champs propres** : nouvelle méthode `AjouterLigneDetail` et fabriques `CreerZoneTexteDetail`, `CreerListeDetail`, `CreerCaseDetail`, `CreerNombreDetail` ; Civilités, Modes de paiement, Organismes, Périodicités, Tarifs de séance, Situations juridiques et Statuts de séance les utilisent (listes et cases du kit) — plus aucune couleur ni position écrite dans ces écrans ; libellé « Porteur BCE » (explication dans l'infobulle).
- **`UC_NaturesSeance` V1.2.0** : retrait du cache des natures (`NatureDeLaLigne` / `_naturesParId`), code mort depuis la disparition du code de prestation ; la documentation de l'écran ne mentionne plus ce champ.
- **Documentation** : documentation UI, référence UI (exemple), charte, plan UI, plan de tests Référentiels V1.1 (section 15).

## 📅 16/10/2026 — Vitrine du kit

- **`TestKitAlthea` V2.0.0** : la fenêtre de démonstration devient la vitrine du kit — six onglets (palette et polices, boutons, structure, champs, indicateurs, grilles), chaque contrôle dans ses états (normal, désactivé, consultation, erreur…) ; couleurs de `UITheme` lues dans le code (toujours à jour) ; fiche type qui bascule entre saisie et consultation. Ouverture : bouton « Vitrine du kit » de l'accueil (Debug, `UC_Accueil` V1.5.0).
- **Correctif du kit** : sous un contrôle à fond recopié (étapes, pastilles, puce utilisateur, fil d'Ariane…) posé dans une carte titrée, le titre de la carte réapparaissait en fantôme (`TextRenderer` ignorait le décalage du dessin) ; `AltheaDessin.FLAGS_TEXTE_SOUS_ENFANTS` appliqué aux titres d'`AltheaCarte`, d'`AltheaBarreOutils` et aux textes d'`AltheaEnTete`.
- **`Reference_UI.md`** : 12 captures de la vitrine remplacent les maquettes pour les contrôles (palette, polices, boutons, tuiles et lignes, structure, barres, champs, consultation, indicateurs, puce et calendrier, grilles).
- **Relevé (ToDo)** : boutons d'action désactivés peu lisibles.

## 📅 16/10/2026 — UI-7 : Cabinet, onglets Identifiants et Cachet

- **`UC_Cabinet` V1.4.0, onglet Identifiants** : deux cartes titrées, « Identifiants professionnels » et « Diplômes et qualifications », chacune avec sa consigne (dates, colonne « Cachet », Suppr ; colonne « Ordre ») au-dessus de la grille ; le `SplitContainer` est retiré.
- **Onglet Cachet** : consigne dans la carte, sous la barre d'outils ; nouvelle icône tampon `cachet_24_normal` pour « Générer le cachet », icône d'import pour « Importer un scan… » (sans `UtilsButtons`).
- **Messages** : les 7 `MessageBox` restants passent par `DialogChoix` ; les erreurs n'affichent plus le message technique de l'exception (il reste au journal).
- **Relevé (ToDo)** : images enregistrées immédiatement hors Modifier / Enregistrer, cachet généré depuis la base, enregistrement sans transaction.
- **Documentation** : documentation UI, charte, plan UI, plan de tests Cabinet V1.2 (10.17 à 10.19).

## 📅 16/10/2026 — UI-7 : DialogChoix en petite fenêtre du kit

- **`DialogChoix` V2.1.0** (maquette « C ») : la boîte de message, qui jurait avec les fenêtres en cartes, devient une petite fenêtre du kit comme les autres — barre de titre et fond beige à la feuille (`AltheaFondFenetre`), en-tête `AltheaEnTete` (le type se lit sur l'icône seule, titre mauve), message dans une `AltheaCarte`, boutons dans une `AltheaBarreActions`.
- **Ordre des boutons** : celui du reste de l'application — l'action à gauche, Annuler / Non à droite.
- **Croix de la barre de titre** : même résultat qu'Échap (dernier bouton), au lieu du `Cancel` imposé par Windows.
- **Message long** : il défile dans sa carte, sans que la barre de défilement recouvre le texte.
- **Inchangé** : méthodes, propriétés et résultats (`Yes` / `No` / `Cancel` / `OK`) ; aucun appel à modifier.
- **Kit** : `AltheaIconeEtat` V1.1.0 (`Dessiner` partagé et `CreerImage`) ; `AltheaFondFenetre` V1.3.0 (`LargeurZone` publique).
- **Documentation** : documentation UI, référence UI (illustration remplacée par la maquette C), charte, plan UI, plan de tests DialogChoix V2.1 (DC-15 croix, DC-18 titre long).

## 📅 15/10/2026 — Documentation : référence UI réécrite

- **`Docs/Rules/Reference_UI.md`** (nouveau) : un seul document pour construire un écran — fondamentaux (palette `UITheme`, polices, grille d'espacement, icônes), les sept familles d'écrans illustrées par les maquettes validées de la refonte, recettes « nouvelle fenêtre » et « nouveau UserControl » (Designer et code), comportements communs (consultation lisible, dates, mots de passe, erreurs, infobulles), ce qu'on ne fait plus, puis catalogue des contrôles du kit (valeurs par défaut et valeurs d'usage), contrôles Windows restants, utilitaires et nommage.
- **Illustrations** : 17 images dans `Docs/Illustrations/Reference_UI/` (maquettes de la refonte rendues en image, plus une « fenêtre type » et les « états d'un champ » dessinées pour ce document) ; à remplacer par des captures de la vitrine du kit (ToDo).
- Remplace `Reference_UI_Guide_Utilisation.md` et `Reference_UI_Proprietes.md` ; liens mis à jour (Rules, Interface utilisateur, bibliothèque de la documentation, plan UI).

## 📅 15/10/2026 — UI-7 : Cabinet, onglets Cabinet et Praticien

- **Onglet Cabinet** : la grande carte et son `TableLayoutPanel` laissent la place à quatre cartes — « Cabinet » (nom en pleine largeur, situation juridique), « Adresse », « Coordonnées », « Facturation » (BCE, BIC, TVA, IBAN) — à côté de la carte « Logo du cabinet » ; Situation juridique et Pays en `AltheaListe` ; consultation lisible, n° BCE compris (il n'est désactivé qu'en saisie, quand le praticien porte le BCE).
- **`UC_Cabinet` V1.3.0, onglet Praticien** : le `TableLayoutPanel` laisse la place à trois cartes — « Identité » (civilité, nom, prénom, titre), « Coordonnées » (téléphone, e-mail, site) et « Adresse professionnelle (vide = celle du cabinet) » ; Civilité et Pays en `AltheaListe` ; libellés en gras 10,5, champs en 11.
- **Consultation lisible** : `UtilsControls.DefinirLectureSeule` sur les trois cartes (texte normal sur fond teinté au lieu du gris pâle).
- **Icônes redessinées** (24 px, trait sombre, famille de l'œil et du cadenas) : `importer_24_normal`, `compte_24_normal`, `lier_24_normal`, `delier_24_normal` ; Importer (logo, photo, signature), Créer le compte, Lier, Délier n'utilisent plus `UtilsButtons`.
- **Documentation** : documentation UI, charte, plan UI, plan de tests Cabinet (10.15).

## 📅 15/10/2026 — UI-7 : compte praticien (dernière fenêtre)

- **`DialogComptePraticien` V1.1.0** : carte « Compte de travail » (explication, login et nom d'affichage en création, liste des comptes en `AltheaListe` en liaison) ; fond `AltheaFondFenetre` ; libellés en gras 10,5, champs en 11.
- **Erreurs** : aux couleurs de la charte (au lieu de Firebrick) ; une erreur inattendue n'affiche plus son message technique — message général, détail (avec l'exception) au journal.
- **UI-7** : toutes les fenêtres suivent désormais le schéma des fiches. Restent le champ d'heure du kit, les `NumericUpDown` et la grille des Paiements.
- **Documentation** : documentation UI, charte, plan UI, ToDo, plan de tests Cabinet V1.1 (10.14).

## 📅 15/10/2026 — UI-7 : restauration

- **`DialogRestauration` V1.2.0** : le `TableLayoutPanel` laisse la place à quatre cartes — « Archive à restaurer », « Compte d'administration MariaDB », « Vérification » (verdict à droite du bouton), « Dernières restaurations » ; fond `AltheaFondFenetre` (image étirée retirée) ; fenêtre un peu plus haute (860 px) pour laisser de la place à l'historique.
- **Champs du kit** : archives en `AltheaListe`, « autre mot de passe » en `AltheaCase` ; libellés en gras 10,5, champs en 11 ; boutons « œil » avec l'icône `oeil_24_normal`, sans `UtilsButtons`.
- **Documentation** : documentation UI, charte, plan UI, plan de tests Sauvegardes V1.2 (7.17).

## 📅 15/10/2026 — UI-7 : zone clinique

- **`DossierZoneEditor` V1.3.0** : l'éditeur riche est posé dans une carte ancrée sur les quatre bords, titrée par le nom de la zone (« (consultation) » ajouté en lecture seule) ; l'en-tête rappelle le dossier (« Zone clinique — dossier *code* ») ; fond `AltheaFondFenetre` (image étirée retirée).
- **Nettoyage** : `Tag` des boutons corrigés (inversés) et `Tag` inutile retiré de l'éditeur.
- **Documentation** : documentation UI, charte, plan UI, plan de tests Dossiers V1.2 (D-18).

## 📅 15/10/2026 — UI-7 : catégories de quota

- **`CategoriesQuotaEditor` V1.2.0** (règle « fenêtres en cartes ») : carte de la liste avec barre locale « Catégories » (Désactiver en Outil, Supprimer en OutilDanger), aide et grille, comme les cartes de l'écran Domaines ; carte « Nouvelle catégorie » (Code, Libellé, Ajouter) ; fond `AltheaFondFenetre` (image de fond et icônes devenues inutiles retirées du `.resx`, 57 → 12 Ko). Plus aucun `GroupBox` dans les fenêtres.
- **Suppression** confirmée par `DialogChoix.ConfirmerSuppression` (« Supprimer » en terre, Annuler par défaut).
- **Documentation** : documentation UI, charte, plan UI, plan de tests Domaines V2.1 (D-47).

## 📅 15/10/2026 — UI-7 : fenêtres de connexion

- **Une carte par zone, fond `AltheaFondFenetre`** (règle « fenêtres en cartes ») : `Login` V1.2.0 (« Veuillez vous connecter »), `ChangePassword` V1.3.0 (« Mot de passe »), `ElevationAcces` V1.2.0 (« Accès protégé requis », rôle en `AltheaListe`), `ConfigurationConnexion` V1.2.0 (« Paramètres MariaDB » et « Test de connexion »). Panneaux racines (`pnlForm`), tableaux de mise en page et bandeaux d'introduction supprimés : plus aucune fenêtre n'a de `pnlForm`.
- **Boutons « œil »** en `AltheaBouton` Outil icône seule (5 boutons), sans `UtilsButtons`, avec une nouvelle icône `oeil_24_normal` (œil plein sombre, `Assets/Ico_divers`) : l'ancienne, fine et grise, disparaissait ; « Modifier » du mot de passe MariaDB en bouton du kit (Clair).
- **`ElevationAcces`** : les boutons sont thématisés avant les sorties anticipées (sans élévation possible, ils perdaient leur icône) ; gestionnaire vide `pnlTop_Paint` retiré.
- **Restent Windows** : le port MariaDB (`NumericUpDown`).
- **Sécurité** : `Login` ne contient plus d'identifiants en clair. Le pré-remplissage de développement (build Debug seulement) lit les variables d'environnement du poste `ALTHEA_DEV_LOGIN` et `ALTHEA_DEV_PASSWORD` (`PreRemplirIdentifiantsDev`) ; à retirer en fin de développement. L'ancien mot de passe reste dans l'historique Git : il est changé.
- **Documentation** : documentation UI, charte, plan UI, plan de tests Login V1.1 (section 10), ToDo.

## 📅 15/10/2026 — UI-7 : barre de titre aux couleurs du kit

- **Nouvel outil du kit `AltheaBarreTitre`** (demande de Joëlle) : la barre de titre Windows de chaque fenêtre prend les couleurs d'Althéa — fond beige clair, titre sauge très foncé, bordure sauge clair — au lieu de la couleur d'accentuation de Windows (souvent bleue). Windows peint toujours sa propre barre (déplacement, ancrage, boutons, accessibilité inchangés) ; Windows 11 et plus, sans effet avant.
- **Partout d'un coup** : `AltheaFondFenetre` V1.2.0 l'applique aux fenêtres reprises ; appel direct dans la Home, la connexion (`Login`, `ChangePassword`, `ElevationAcces`), `ConfigurationConnexion`, `DialogRestauration`, `DialogComptePraticien`, `CategoriesQuotaEditor`, `DossierZoneEditor` et la fenêtre des référentiels (`ReferentielModalHost`).
- **Documentation** : charte, Rules, références UI, structure, interface, plan UI, plan de tests du kit (KF-04, KF-05).

## 📅 15/10/2026 — UI-7 : dialogues du dossier

- **Une carte par dialogue, fond `AltheaFondFenetre`** (images de fond retirées des `.resx`) : `DialogMotifAnnulation` V1.2.0 (« Motif de l'annulation » ; couleurs d'erreur de la charte au lieu de Firebrick / MistyRose), `DialogPaiementReaffectation` V1.2.0 (« Report du paiement », listes `AltheaListe`), `DialogPaiementsSeance` V1.1.0 (carte titrée par la séance), `PlanningJourViewer` V1.2.0 (« Séances du jour », date `AltheaDate`).
- **Correction** : dans le détail des paiements d'une séance, le titre (code, date, statut de la séance) était caché sous l'en-tête depuis le lot UI-3 ; il devient le titre de la carte.
- **Documentation** : documentation UI, charte, plans de tests Paiements (P-37) et Séances (S-29), pilotage.

## 📅 15/10/2026 — UI-7 : statut du dossier et report de séance

- **`DossierStatutEditor` V1.2.0** : carte « Statut du dossier », fond `AltheaFondFenetre`, statut cible en `AltheaListe`, date de clôture en `AltheaDate` bornée à aujourd'hui (`DateMax`) ; son libellé n'est plus grisé quand la date ne s'applique pas.
- **`DialogReportSeance` V1.3.0** : carte « Nouvelle date de la séance », fond du kit, date en `AltheaDate`, « Préciser les horaires » en `AltheaCase` ; les deux heures restent des contrôles Windows jusqu'au champ d'heure du kit.
- **Correction (régression du lot UI-4, 10/10/2026)** : en passant les boutons au kit, le lot UI-4 avait perdu leur `DialogResult` dans six fenêtres. Conséquence visible : **« Reporter » (report de séance) et « Reporter le paiement » (réaffectation d'un paiement) ne faisaient plus rien**. Rétabli : `DialogReportSeance` (Reporter, Annuler), `DialogPaiementReaffectation` (Reporter, Conserver), `PlanningJourViewer`, `DialogPaiementsSeance`, `DialogMotifAnnulation`, `DialogComptePraticien` (Fermer / Annuler, qui fonctionnaient encore grâce à `CancelButton`).
- **Documentation** : documentation UI, charte, plans de tests Dossiers (D-17) et Séances (S-28), Rules (conserver `DialogResult`), pilotage.

## 📅 15/10/2026 — UI-7 : fenêtre Paiement

- **`PaiementEditor` V1.2.0** (règle « fenêtres en cartes ») : cartes « Encaissement » (séance, date, montant, mode de paiement [+], référence) et « Commentaire » ; fond `AltheaFondFenetre` (image de fond retirée du `.resx`) ; séance et mode en `AltheaListe`, date en `AltheaDate` ; [+] en `AltheaBouton` Outil à icône seule ; libellés et champs aux tailles des fiches.
- **Consultation lisible** : `AppliquerEtatSaisie` remplace `AppliquerLectureSeule` (zones blanches en saisie, tout lisible si le dossier est archivé).
- **Documentation** : documentation UI, charte, plan de tests Paiements V1.1 (P-34 à P-36), pilotage.

## 📅 15/10/2026 — UI-7 : fenêtre Séance

- **`SeanceEditor` V1.4.0** (maquette « Dialogue Séance ») : fond `AltheaFondFenetre` ; champs regroupés dans deux cartes « Prestation » (tarif, nature, montant et cadenas) et « Rendez-vous » (date, heures, planning du jour, durée, statut), comme les fiches ; Tarif, Nature et Statut en `AltheaListe` ; date en `AltheaDate` (propriété `DateSeanceSaisie`) ; boutons [+] et cadenas du tarif en `AltheaBouton` Outil à icône seule ; « Voir planning du jour » en `AltheaBouton` Clair ; libellés et champs aux tailles des fiches, lignes alignées.
- **Saisie à un seul endroit** : `DefinirSaisieActive` remplace les trois listes de contrôles recopiées (dossier archivé, séance reportée, séance soldée et son déverrouillage) ; consultation lisible. La durée, calculée depuis les heures, n'est plus modifiable par ses flèches (elles restaient actives malgré la lecture seule).
- **Cadenas dessinés** : icônes `cadenas_24_normal` / `cadenas_ouvert_24_normal` (`Assets/Ico_divers`, ajoutées au projet) à la place des caractères 🔒 / 🔓.
- **`UtilsControls` V1.3.0** : `DefinirChampLectureSeule` (un champ seul), utilisé aussi par `DefinirLectureSeule`.
- **Petits boutons du kit** (aussi fenêtres Contact, Intervenant et fiche Patient) : plus d'`UtilsButtons` — ses icônes de survol, claires, disparaissaient sur le survol vert pâle du kit ; l'icône du Designer reste, atténuée quand le bouton est inactif.
- **Règle** (Joëlle, 15/10/2026) : toutes les fenêtres suivent le schéma des fiches — champs regroupés dans des cartes titrées, fond `AltheaFondFenetre`, contrôles du kit (Rules § 15).
- **Restent Windows** : les deux listes d'heures (champ d'heure du kit à venir) et les deux `NumericUpDown`.
- **Fiche dossier** (`UC_DossierFiche` V1.4.0) : « Modifier » de la barre d'actions montré seulement sur l'onglet Fiche Dossier — sur les autres onglets il passait en saisie une fiche invisible, sans rien ouvrir ; Séances et Paiements ont leurs propres boutons au-dessus de la grille.
- **Documentation** : documentation UI, charte, références UI, Rules, utilitaires, plans de tests Séances V1.1 (S-23 à S-27) et Dossiers (D-16), pilotage.

## 📅 14/10/2026 — UI-7 : fiche utilisateur

- **`UtilisateurEdition` V1.5.0** : panneau racine `pnlForm` supprimé, contenu posé directement sur la fenêtre (comme les fenêtres récentes) ; fond du kit (`AltheaFondFenetre` V1.1.0 accepte aussi un panneau, pour les fenêtres qui gardent le leur) ; rôles en `AltheaListe` ; Actif, Forcer le changement et Compte verrouillé en `AltheaCase` ; libellés et champs aux tailles des fiches.
- **Consultation lisible** : `AppliquerFondsChamps` — zones en lecture seule sur fond teinté, zones modifiables en blanc, selon le mode ; texte gris foncé à la place du vert.
- **Hubs Administration et Référentiels** : boutons « Élever accès » et « Retour rôle de base » élargis (181 → 220 px) — « Retour rôle de base » s'affichait sur deux lignes.
- **Documentation** : documentation UI, charte, références UI, plan de tests Utilisateurs V1.3 (section 14), pilotage.

## 📅 14/10/2026 — UI-7 : fenêtres Contact, Intervenant et Thérapeute

- **Cartes du kit** : les 10 `GroupBox` des trois fenêtres deviennent des `AltheaCarte` (titre dessiné) ; libellés et champs aux tailles des fiches (libellés en gras 10,5, champs en 11), lignes régulières.
- **Champs du kit** : 8 listes en `AltheaListe` ; dates en `AltheaDate` facultatives (naissance du contact, période de suivi de l'intervenant — remplacent les cases à cocher Windows) ; « Actif » du thérapeute en `AltheaCase`.
- **Consultation lisible** : `ContactEdition` et `IntervenantEdition` passent par `UtilsControls.DefinirLectureSeule` ; les champs de trace de l'intervenant restent toujours en lecture seule, en fond teinté ; `TherapeuteEdition` (toujours en saisie) a des zones de texte blanches.
- **Fond des fenêtres** (décision du 14/10/2026) : nouvel outil du kit `AltheaFondFenetre` — beige clair uni et petite feuille dessinée dans le coin haut droit, à la place de l'image étirée ; posé sur les trois fenêtres.
- **Petits boutons** : [+], loupe et copie d'adresse passent en `AltheaBouton` variante Outil, icône seule (`AltheaBouton` V1.2.0 centre une icône sans texte) — aussi les deux boutons de la fiche Patient (situation familiale, fiche de la mutualité).
- **ToDo** : décider, après la révision des éditeurs riches, du sort des champs de notes encore en zone de texte Windows.
- **Documentation** : documentation UI (trois fenêtres), charte (fond des fenêtres, bouton icône seule), références UI, structure, plan de tests Patients V1.3 (PAT-29a à 29e), plan de tests du kit (KF-01 à KF-03), pilotage.

## 📅 14/10/2026 — UI-7 : fiche dossier

- **Champs du kit** : domaine en `AltheaListe` (26 px) ; date d'ouverture et filtres « Du … au … » des onglets Séances et Paiements en `AltheaDate`. Une borne « du » postérieure à la borne « au » déplace l'autre borne (`CorrigerBornes`, commun aux deux onglets).
- **Consultation lisible** : `AppliquerMode` V1.1.0 applique `UtilsControls.DefinirLectureSeule` à la carte « Paramètres du dossier ».
- **Correction après essai** : bandeaux de filtres — case « Du » élargie (le texte était coupé), libellé « au » en gris foncé (peu lisible à côté des dates inactives).
- **Relevé** : grille des Paiements à revoir (regroupement par séance peu clair, états encaissé / en attente / dû / avoir, couleurs de la charte) — ToDo C4.
- **Documentation** : documentation UI (`UC_DossierFiche`), charte, plan de tests Dossiers V1.1 (D-15a à D-15d), plan de tests du kit V1.1, pilotage.

## 📅 14/10/2026 — UI-7 : fiche patient, onglet Identité

- **Champs du kit** : les 7 listes (civilité, type d'identifiant, latéralité, pays, mutualité, situation familiale, tarif par défaut) passent en `AltheaListe` (26 px, alignées sur les zones de texte) ; la date de naissance passe en `AltheaDate` **facultative** (vide = inconnue, croix pour effacer — remplace la case à cocher Windows). Le chargement et l'enregistrement utilisent `Valeur` (`Date?`).
- **Consultation lisible** : `DefinirEtatChampsIdentite` V2.0.0 applique `UtilsControls.DefinirLectureSeule` aux trois cartes ; zones de texte blanches en saisie, fond teinté en consultation ; « Fiche de la mutualité » reste consultable.
- **Relevé** : valeurs de latéralité codées en dur (ToDo).
- **Documentation** : documentation UI (`UC_PatientFiche`), charte, plan de tests Patients V1.2 (PAT-18a à 18c, validés), pilotage.

## 📅 14/10/2026 — UI-7 : champ de date du kit

- **`AltheaDate`** (maquette « AltheaDate », contrôle composé — ADR-33) : champ arrondi comme la recherche ; saisie au clavier (« / » placés seuls ; jj/mm/aaaa, jj/mm/aa, jjmmaa, jjmmaaaa, jj/mm), date impossible ou hors bornes en rose puis retour à la dernière date valide ; calendrier par l'icône, Alt+Bas ou F4 ; Haut / Bas = ± 1 jour ; `Facultative` (vide = pas de date, croix pour effacer — remplace la case à cocher Windows) ; `LectureSeule` lisible ; `Valeur` (`Date?`), `DateMin` / `DateMax`, évènement `ValeurChangee`.
- **`AltheaCalendrier`** (calendrier A) : mois dessiné dans le style du kit, semaine du lundi, week-end atténué, aujourd'hui entouré, date choisie en sauge, grille des années, « Aujourd'hui », clavier complet ; utilisable seul.
- **`UtilsControls.DefinirLectureSeule`** V1.1.0 : prend en charge `AltheaDate`.
- **Démonstration** : `TestChampDate` (nouveau bouton « Champ de date » de l'accueil, Debug). Correction après essai : « Aujourd'hui » réaffiche aussi le mois du jour.
- **Documentation** : Rules (« Dates »), ADR-33 (note), charte, référence des propriétés, structure, nouveau plan de tests `PLAN_TESTS_KIT_ALTHEA.md`, pilotage. Les `DateTimePicker` des écrans seront remplacés écran par écran.

## 📅 14/10/2026 — UI-7 : DialogChoix V2 et icônes d'état dessinées

- **`DialogChoix` V2.0.0** (maquette « DialogChoix — B ») : en-tête teinté selon le type, corps blanc, boutons du kit à droite (l'action en dernier), sans barre de titre Windows (déplacement par l'en-tête, ombre portée). La fenêtre suit son texte : 420 à 640 px de large, 70 % de l'écran au plus, puis défilement ; recentrée sur la fenêtre appelante. Entrée = bouton par défaut, Échap = dernier bouton.
- **Parties structurées facultatives** : liste « libellé : valeur » (`AjouterDetail`), complément, encadré d'attention, remarque, question finale ; `VarianteAction` (Sauge, Ocre, Terre). **Boutons qui disent l'action** : `Confirmer(message, titre, texteAction, texteAnnuler[, variante[, attention]])`, `ConfirmerSuppression` (« Supprimer » en terre, Annuler par défaut). Les 202 appels existants fonctionnent sans modification.
- **Nouveau contrôle du kit `AltheaIconeEtat`** : six icônes d'état dessinées par le code (style 2 : fond pâle, contour et signe en couleur ; l'Attente tourne), couleurs dérivées de la charte (Information en bleu pour la distinguer du succès). Les GIF animés ne sont plus utilisés (suppression au lot UI-8).
- **Adoption** : dialogues des règles de quota des Domaines (récapitulatif structuré, « Enregistrer », « Clôturer », « Supprimer », encadré d'attention pour les périodes sans règle).
- **Démonstration** : fenêtre `TestDialogChoix` (bouton de test DialogChoix de l'accueil, Debug) — un bouton par cas, résultat renvoyé, planche des icônes.
- **Décisions** : ordre d'UI-7 (DialogChoix, champ de date du kit, puis écrans un par un) ; le tour « lecture seule lisible » se fait avec UI-7.
- **Documentation** : documentation UI (DialogChoix), Rules § 16, charte (icônes d'état, kit), référence des propriétés, ADR-11 (note), structure, plan de tests DialogChoix V2.0, pilotage.

## 📅 14/10/2026 — Refonte UI-6 : écran Domaines

- **Étapes** (6a) : nouveau contrôle du kit `AltheaEtapes` à la place de la bande d'onglets (`AltheaOnglets.MasquerOnglets`, V1.2.0). Chaque étape montre son état — fait, à compléter (avec le détail), non nécessaire — calculé par `UC_SyntheseDomaine.CalculerManquesParEtape` (V2.1.0), seule source de la complétude ; pastille d'état du domaine ; après la création, ouverture de la première étape à compléter. Général en deux cartes (les `GroupBox` disparaissent), organisme en `AltheaListe`.
- **Consultation lisible** (règle structurante, Rules) : un champ consulté n'est plus grisé — texte normal, fond `UITheme.ColorChampLectureSeule` ; `UtilsControls.DefinirLectureSeule` ; `AltheaCase` et `AltheaListe` (V1.1.0) se dessinent lisibles une fois désactivées. Tour des autres écrans noté au ToDo.
- **Fiche et listes rattachées** (règle, Rules) : Modifier / Enregistrer ne concerne que l'étape 1 ; les étapes 2 à 4 se modifient action par action (enregistrement immédiat) et ne sont pas accessibles pendant Modifier — expliqué dans les consignes.
- **Tarifs et Natures** (6b) : `UC_LiaisonsPrixDomaines` et `UC_LiaisonsNaturesDomaines` V2.0.0 — carte, barre locale (ADR-34), formulaire au pied de la carte, action « Créer un nouveau … » à droite de la barre, catégorie de quota choisie au rattachement et modifiable dans la grille, messages par `DialogChoix`. Le rattachement d'une nature enregistre directement sa catégorie (`InsertLiaison`).
- **Catégories & quotas** (6c, ADR-40) : catégories en pastilles, règles en carte avec barre locale ; nouvelle règle guidée (pré-remplie, dates proposées pour couvrir une période sans règle, date de début passée admise) et **récapitulatif** avant tout enregistrement ; une règle **qui n'a pas servi** se corrige (« Modifier la règle ») ou se supprime — la suppression rouvre, dans la même transaction, la règle précédente qu'elle avait clôturée ; une règle qui a servi se clôture ou se désactive seulement ; périodes sans règle signalées.
- **Synthèse** (6d) : présentée dans une carte « Convention du domaine », comme dans la fiche dossier ; en mode colonnes, le quota passe en bande pleine largeur sous les trois colonnes, champs côte à côte (`UC_SyntheseDomaine` V2.3.0, profite aussi à la fiche dossier). Bouton « Modifier la règle » élargi.
- **Périodes sans règle** (6d, décision de Joëlle) : jamais comblées automatiquement ; ligne d'alerte (ocre sur fond jaune pâle) à leur place dans la grille des règles, que l'on sélectionne pour que « Nouvelle règle » propose ses dates ; une période entre deux règles rend l'étape 3 et le domaine « à compléter » (synthèse V2.2.0) ; nouvelle règle, correction et clôture annoncent dans leur confirmation l'écart qu'elles créeraient.
- **`DialogChoix` V1.1.0** : la hauteur s'adapte au texte (plus de message tronqué), dans la limite de 70 % de l'écran. Refonte complète (mise en page, icônes) prévue avant UI-7.
- **Décision** : ADR-40 remplace le point 8 d'ADR-27 (« une règle de quota ne se modifie jamais ») ; table des ADR complétée de 29 à 40.
- **Documentation** : guide utilisateur Domaines revu, plan de tests Domaines V2.0, concepts métier, processus 16, Rules, documentation UI (`UC_DomainesHome`, panneaux de liaison, `UC_SyntheseDomaine`, `DialogChoix`), charte et référence des propriétés (`AltheaEtapes`, `MasquerOnglets`, `ColorChampLectureSeule`), structure, base de données, pilotage. Aucune modification de schéma.

## 📅 14/10/2026 — Icônes des écrans d'administration

- **Icônes** dessinées par Joëlle : tuiles `journaux` et `etatSystem` (`Boutons_ico_48`, remplacent `logs` et l'icône provisoire `parametres`), titres `journaux` et `etatSystem` (`Titre_Ico`, ressources `Titre_journaux` / `Titre_etatSystem`).
- **Hub Administration** : la tuile « Logs » devient « Journaux » (le contrôle garde son nom `btnLogs`).
- **Documentation** : charte graphique (inventaire des icônes), documentation UI, processus 02, sécurité, plan de tests, pilotage.

## 📅 13/10/2026 — Documentation : revue de cohérence de fin de journée

- **Concepts métier reconstitué** : depuis le 26/09/2026, `Concepts_Metier_Althea.md` contenait par erreur une copie de `ARCHITECTURE_DECISIONS.md` (rien d'unique n'y avait été écrit). Le document est repris de sa dernière version saine (25/09) et mis à jour : domaine = convention, quotas par catégories, cycle de vie des dossiers (dont En pause), report, règle unique séances ↔ paiements et avoirs, organismes et mutualités, pays et civilités, cabinet / praticien. La description des grilles renvoie désormais à la documentation UI.
- **Pilotage** : checklist V1 réécrite (sa synthèse datait de juin) ; ToDo (scripts métier, sauvegarde et restauration cochés — la sauvegarde automatique quotidienne reste à décider, un rappel est en place) ; planning (jalons J4 / J5 atteints, indicateurs, priorité suivante UI-6 / UI-7) ; état du projet (ligne des référentiels de tarification réparée, réalisations du 13/10).
- **Cohérence** : convention des clés alignée dans `Database_technique.md` et l'état du projet ; README (état actuel, ligne Journaux et état du système) ; lien vers les concepts métier corrigé dans `Parametres_Referentiels.md`.

## 📅 13/10/2026 — Administration : journaux et état du système

- **Journaux** (Admin, tuile « Logs ») : nouvel écran `UC_Journaux` — jour, filtres niveau / catégorie / texte / erreurs seulement, entrées les plus récentes en premier, débuts de session en gras, erreurs en couleur, détail complet (exception, piles d'appels). Lecture seule. `LecteurJournaux` relit les fichiers ; le format d'une ligne est défini une seule fois dans `GestionLog`.
- **État du système** (Admin, nouvelle tuile du groupe Technique) : `UC_EtatSysteme` — application, base de données (connexion, version de MariaDB, **version de la base comparée à la version attendue**), dossiers et espace disque (dont : sauvegardes sur un autre disque que les données), sauvegardes (dernière réussie, mot de passe, outils MariaDB) ; « Copier le rapport » sans aucun secret.
- **Règles** : clés par séquences pour les tables métier, `AUTO_INCREMENT` admis pour les référentiels, liaisons de paramétrage et tables techniques (Rules.md aligné sur la pratique).
- **Version de base attendue** : `VersionSchema.Attendue` (2.39.0), à mettre à jour avec chaque migration (Rules.md) — première brique du contrôle au démarrage.
- **Revue des journaux (ADR-39)** : les noms et prénoms des patients, contacts familiaux et thérapeutes ne sont plus écrits dans les journaux (identifiants seulement) ; la valeur citée par MariaDB dans une erreur de doublon est masquée (`Duplicate entry '***'`).
- **Documentation** : ADR-39, processus 02 et 23, plan de tests `PLAN_TESTS_ADMINISTRATION.md` (validé le 13/10/2026). Convention de version de l'application retenue pour l'installation (ToDo).

## 📅 13/10/2026 — Lot 30 (étape P3) : restauration d'une sauvegarde

- **Restauration** (ADR-38, Admin) : nouvelle fenêtre `DialogRestauration`, ouverte par « Restaurer… » dans Sauvegardes. Choix de l'archive, du mot de passe (celui du poste ou un autre) et d'un **compte d'administration MariaDB** (`root`), jamais enregistré — le compte de l'application garde ses seuls droits sur les données. « Vérifier l'archive » contrôle tout sans rien modifier : mot de passe, manifeste, **même version du schéma**, outil `mariadb`, droits du compte d'administration, espace disque.
- **Déroulé** : sauvegarde de sécurité (sans rotation) → documents extraits dans un dossier provisoire (chemins contrôlés, tailles vérifiées) → dossier actuel renommé `…_avant_restauration_…` et **conservé** → base vidée puis rechargée depuis l'archive chiffrée (jamais de dump en clair) → documents mis en place → chemins propres au poste rétablis → **redémarrage**.
- **Échec** : rechargement automatique de la base depuis la sauvegarde de sécurité et ancien dossier remis ; message précis selon l'étape. Les messages du client `mariadb` sont réduits au code et à la ligne (ils recopient sinon des données de patients).
- **Base de données (v2.39.0)** : table `tec_restaurations` (journal, écrit dans la base restaurée).
- **Mutualisation** : `OutilsMariaDB` (recherche des outils, fichier d'options temporaire) commun à la sauvegarde et à la restauration ; `DatabaseManager.OpenConnectionAvecCompte` ; `GestionSauvegardes.Sauvegarder` accepte une sauvegarde sans rotation.
- **Corrections pendant les tests** : verdict de la vérification affiché en gros et en couleur ; « Vérifier » avec un mot de passe faux ne fige plus l'application sous Visual Studio (`VerifierArchive` rend l'erreur au lieu de la lever) ; journal des sauvegardes remis d'aplomb après une restauration (archive restaurée « Réussie », sauvegarde de sécurité réinscrite).
- **Constat** : sur le poste de développement, les comptes `root` acceptent aussi l'identification Windows (`gssapi`) — un mot de passe root faux y est accepté par MariaDB ; à régler à l'installation chez Pearl (ToDo).
- **Tests** : section 7 validée le 13/10/2026, sauf 7.4 et 7.5 à revérifier ; 7.15 (remise en état automatique) reporté, non testé à ce jour.
- **Documentation** : ADR-38, processus 22, guide utilisateur des sauvegardes (dont la restauration manuelle sur poste neuf ou d'une archive plus ancienne, et ce que l'archive ne contient pas : `%APPDATA%\Althea`), plan de tests (section 7).

## 📅 12/10/2026 — Documentation : mise à jour complète après les lots 27 à 30 (P2)

- **Processus** : processus 03 (paramètres) réécrit selon ADR-35 / ADR-36 ; nouveau processus 21 (sauvegarde) ; conservation des journaux réglable (processus 02) ; liens vers les sources corrigés.
- **Architecture** : architecture technique (paramètres, dossier de données, sauvegarde, données locales du poste, dépendances), interface utilisateur (kit Althéa, opérations longues), structure de la solution (dossier `Theme/`, helpers, arborescence réparée), base de données (toutes les fiches de tables au même modèle — rôle, tableau Colonne / Rôle, Contraintes, Remarques, Historique — vérifiées contre le dump de structure v2.38.0), paramètres (dossier des sauvegardes).
- **Règles** : paramètres nés par migration et lus par `ClesParametres`, chemins et dossier de données, opérations d'écriture jamais incomplètes en silence, icône d'en-tête choisie dans le Designer ; charte graphique : couleurs des icônes de titre (noir et vert) et icônes Sauvegarde, Pays, Civilités.
- **Pilotage** : état du projet (modules, décisions D-26 à D-29, réalisations du 06 au 12/10), checklist V1 (sauvegarde faite, restauration à venir) ; README actualisé (état, sauvegarde, liens réparés).

## 📅 12/10/2026 — Lot 30 (étape P2) : sauvegarde complète chiffrée

- **Sauvegarde** (ADR-37) : une archive `Althea_AAAAMMJJ_HHMMSS.zip` chiffrée **AES-256** (ouvrable avec 7-Zip et le mot de passe) contenant le dump complet de la base (`mariadb-dump`), tout le dossier de données et un manifeste. Le dump passe directement dans l'archive chiffrée (jamais en clair sur le disque) ; le mot de passe de la base est transmis par un fichier d'options temporaire. Écriture sous nom provisoire puis renommage, rotation des N dernières, journal de chaque tentative, rappel à l'ouverture. Un fichier illisible (document ouvert dans Word) fait échouer la sauvegarde avec un message qui le nomme, plutôt que de produire une archive incomplète ; les verrous Office `~$…` ne sont pas archivés.
- **Base de données — migration v2.38.0** (+ ROLLBACK) : table `tec_sauvegardes` (journal) ; paramètres `SAUVEGARDE_DOSSIER`, `SAUVEGARDE_NB_CONSERVEES` (10), `SAUVEGARDE_RAPPEL_JOURS` (7), `SAUVEGARDE_DOSSIER_MARIADB` ; ajoutés au script `reinit_tec_parametres.sql` (12 paramètres, chemins du poste conservés).
- **Mot de passe des sauvegardes** — propre aux archives, commun au cabinet, distinct des mots de passe de connexion : défini par l'Admin (12 caractères minimum dont une majuscule, une minuscule et un chiffre, deux saisies, œil « maintenir pour voir »), conservé chiffré par Windows (DPAPI) dans `%APPDATA%\Althea\althea.sauvegarde.json`, jamais en base ni dans les journaux ; vérification possible.
- **Écran `UC_Sauvegardes`** (tuile « Sauvegardes », désormais ouverte au SuperUser) : dossier des archives (refusé dans le dossier de données), sauvegarde en arrière-plan avec progression, navigation et fermeture bloquées pendant l'opération, mot de passe, historique.
- **Paramètres** : `GestionParametres.UpdateValeurParametreParCle` ; l'écran Paramètres refuse aussi un dossier de sauvegarde situé dans le dossier de données.
- **Dépendance** : SharpZipLib 1.4.2 (MIT).
- **Documentation** : ADR-37, Database_technique, Paramètres et référentiels, Sécurité, Structure, Environnement technique, documentation UI, plan de tests `PLAN_TESTS_SAUVEGARDES.md`, ToDo, Planning.
- **Validation** : migration v2.38.0 testée sur une copie (rejeu, contrainte de statut, retour arrière, réapplication) ; commande `mariadb-dump` testée (fichier d'options avec mot de passe à caractères spéciaux, sortie en flux) et **rechargement du dump vérifié** (tables et séquences identiques) — c'est ce test qui a imposé `--skip-add-locks`. Premier essai réel : `--events` retiré (droit `EVENT` absent du compte de l'application, et aucun événement dans la base). Premier essai réel le 12/10 : archive produite dans le bon dossier, ouverte avec WinRAR et le mot de passe (l'explorateur Windows ne lit pas l'AES).

## 📅 12/10/2026 — Lot 30 (étape P1) : paramètres enrichis

- **Base de données — migration v2.37.0** (+ ROLLBACK ; rejouable, y compris sur une base qui a reçu la première version du script) :
  - `tec_parametres.valeur_min` / `valeur_max` : bornes des paramètres numériques ;
  - nouveaux paramètres, jusqu'ici écrits dans le code : durée par défaut d'une séance (45 min, 10 à 240), pas des heures proposées (15 min, 5 à 60), première et dernière heure proposées (8 et 20), échecs de connexion avant verrouillage (5, 3 à 20), conservation des journaux (7 jours, 1 à 90), cachet à l'encre noire (non : bleu nuit) ;
  - `PATH_GENERAL` passe au type `PATH` ;
  - suppression des paramètres de test (`TESTONG`, `TESTONG2`, `APP_DATA`) et des chemins obsolètes (`PATH_DOCUMENT`, `PATH_DOSSIER`, `PATH_LOG`, `PATH_BACKUP`) — accord du 12/10.
- **Dossier de données (ADR-35)** : seule la racine reste un paramètre ; les sous-dossiers `Documents`, `Cabinet`, `Utilisateurs` sont fixes et réunis dans `ArborescenceDonnees` (le nom `Documents` est inscrit dans les chemins enregistrés : le modifier aurait rendu des fichiers introuvables). `CheminsPatientHelper.GetRacineDonnees` / `GetSousDossierDocuments` remplacés par `ArborescenceDonnees`.
- **Lecture des paramètres (ADR-36)** : `ClesParametres` (clés et valeurs de secours) et `GestionParametres.GetEntier` / `GetBooleen` (bornes appliquées ; base injoignable ou paramètre invalide : valeur de secours journalisée). Une seule valeur par paramètre, sans dates de validité.
- **Changements de comportement** :
  - la constante `GestionSeances.DUREE_SEANCE_MINUTES` devient `DureeSeanceParDefaut()` ;
  - les listes d'heures de la fiche séance suivent la grille paramétrée (`GestionSeances.CreneauxProposes`) ; si le pas ne divise pas la durée, il est réduit pour que l'heure de fin proposée et la séance suivante restent sur la grille ;
  - les journaux anciens ne sont plus purgés au premier message mais au démarrage, une fois la base jointe, avec la durée choisie (sans base : pas de purge pendant la session) ;
  - le seuil de verrouillage et l'encre du cachet suivent leurs paramètres.
- **Écran Paramètres** :
  - **seule la valeur se modifie** : bouton « Nouveau » retiré (un paramètre créé à la main n'était lu par aucun code — doublon constaté en recette), libellé, groupe, type, description, activation et ouverture au SuperUser en lecture seule ; droit du SuperUser revérifié dans la requête (`GestionParametres.UpdateValeurParametre`, qui remplace `UpdateParametre`, `InsertParametre`, `DesactiverParametre` et `CleParametreExiste`) ;
  - champs numériques limités aux bornes ; chemins avec bouton « Parcourir… », contrôle du chemin complet, proposition de créer le dossier absent, racine des données obligatoire ;
  - après une modification, avis rappelant que la valeur s'applique aux prochaines opérations sans modifier les données enregistrées ;
  - **correction** : le paramètre enregistré pouvait ne pas être celui affiché (paramètre courant resté sur le dernier onglet construit ou sur un onglet quitté) ; l'ancien enregistrement recopiait alors tout le panneau sur un autre paramètre — constaté en recette sur `SECURITE_MAX_ECHECS_LOGIN` et `LOG_RETENTION_JOURS`. Le panneau mémorise désormais le paramètre qu'il affiche, le paramètre courant suit l'onglet, et changer d'onglet pendant une modification demande confirmation.
- **Réinitialisation de `tec_parametres`** (demande du 12/10, base de développement) : nouveau script `Docs/Database/Maintenance/reinit_tec_parametres.sql` — table effacée, séquence remise à 1, recréation des 8 paramètres lus par le code (PAR000001 à PAR000008) avec leurs valeurs par défaut, racine `PATH_GENERAL` conservée ; `APP_MODE_TEST`, lu par aucun code, n'est pas recréé. S'arrête avant tout effacement si la v2.37.0 manque. Jeu de référence des futurs scripts d'installation.
- **Documentation** : ADR-35 et ADR-36, Paramètres et référentiels, Database_technique, Utilitaires transverses, Règles, Concepts métier, Structure, documentation UI, plan de tests UC_Parametres (section 9), ToDo (dont la préparation de l'installation), Planning.
- **Validation** : migration testée sur une copie de la base (MariaDB 10.11) : base ayant reçu la première version (rejouée deux fois), retour arrière (rejoué), base d'origine — même état final ; script de réinitialisation testé (base à jour, rejeu de la migration, base sans v2.37.0 : arrêt avant effacement). Appliqués sur la base de développement ; build réussi, tests validés par Joëlle.

## 📅 12/10/2026 — Photo des comptes utilisateurs et habillage de la fiche utilisateur (fin du lot 28)

- **Règle d'affichage** (décision du 12/10) : un compte lié à un praticien affiche la photo de la fiche praticien ; sinon sa photo propre ; à défaut, ses initiales. Un seul endroit pour cette règle : nouveau module `PhotosUtilisateur` (`Metier/Security`).
- **Base de données — migration v2.36.0** (+ ROLLBACK) : `sec_utilisateurs.photo_fichier` (nom seul du fichier, colonne nullable ; aucune ligne modifiée).
- **Fichiers** : nouveau `CheminsUtilisateurHelper` — photo rangée dans `Utilisateurs/Utilisateur_{id}/` du dossier de données, sous un nom fixe (par identifiant, le login pouvant changer), via l'import d'image unifié.
- **Fiche utilisateur (`UtilisateurEdition`)** : photo dans la carte Identité, bouton « Importer… » pour un compte non lié (mode Modification), aide contextuelle ; la photo s'enregistre dès l'import. Écriture réservée à l'Admin, contrôlée côté métier (`GestionUtilisateurs.UpdatePhotoUtilisateur`) et journalisée.
- **Habillage UI-7** : les panneaux et titres de la fiche deviennent quatre cartes du kit (Identité, Droits, Sécurité, Audit). Le fond et les champs suivront avec le reste d'UI-7.
- **Bandeau de la Home** : la pastille utilisateur (`AltheaPuceUtilisateur.Photo`) affiche la photo en rond à la place des initiales ; rafraîchie avec le bandeau et à la fermeture de la fiche utilisateur.
- **Correction** : le titre professionnel du praticien n'apparaissait pas dans le bandeau pour un compte lié (requête `SelectPraticienParUtilisateur` incomplète).
- **Documentation** : Database_technique, Utilitaires_Transverses, Securite_Utilisateurs, Structure_Solution, documentation UI, Référence des propriétés, Charte, Plan de refonte UI, plan de tests (section 13), ToDo, Planning.
- **Validation** : migration testée sur une copie de la base (MariaDB 10.11) : application, rejeu, retour arrière. Build réussi ; tests de la section 13 validés par Joëlle.

## 📅 12/10/2026 — Référentiels de saisie (étape R4) : suppression des anciennes colonnes texte

- **Base de données — migration v2.35.0** (+ ROLLBACK), **destructive** (confirmée le 12/10) : suppression de `pays` (patients, cab_cabinet, cab_praticiens, famille_contacts, therapeutes), `civilite` (cab_praticiens) et `mutualite` (patients), remplacées en v2.34.0 et plus utilisées depuis R2 / R3. Le ROLLBACK recrée les colonnes et les remplit depuis les référentiels ; le texte non reconnu (mutualités de test saisies librement) n'est pas restituable sans la sauvegarde.
- **Groupe « Référentiels de saisie » terminé** (R1 à R4).
- **Documentation** : Database_technique, plan de tests (§14.23), ToDo, Planning.
- **Validation** : migration testée sur une copie de la base (MariaDB 10.11, collation remplacée pour le test) : application, rejeu, retour arrière, réapplication. Appliquée sur la base de développement par Joëlle le 12/10 ; vérifications à l'exécution (§14.23) à confirmer.

## 📅 12/10/2026 — Référentiels de saisie (étape R3) : mutualité sur la fiche Patient

- **Fiche Patient, carte Administratif** : la mutualité devient une liste (organismes de type Mutualité), complétée par une **loupe** qui ouvre la fiche de la mutualité choisie en fenêtre modale (coordonnées, plateforme, procédure) ou permet d'en ajouter une, sans quitter la fiche ; nouveaux champs **numéro d'affiliation** et **CT1 / CT2** (3 chiffres, contrôlés). La loupe reste disponible en consultation ; en saisie, une mutualité ajoutée est sélectionnée.
- **`UC_Organismes` ouvrable en contexte** : type imposé (liste filtrée, type verrouillé pour les créations), titre adapté, organisme courant présélectionné (`UC_ReferentielBase.SelectionnerElement`, nouvelle méthode protégée). Le bouton [+] organisme des Domaines l'ouvre désormais limité aux organismes de remboursement.
- **Données** : lecture / écriture de `id_organisme_mutualite`, `numero_affiliation`, `code_titulaire_1` / `code_titulaire_2` ; le texte `mutualite` n'est plus alimenté (suppression en v2.35.0).
- **Utilitaires** : `UtilsSaisieReferentiels.ChargerMutualites` ; `UtilsIdentifiantBelge.ValiderCodeTitulaire`.
- **Documentation** : base, utilitaires, UI, plans de tests (§14.22, PAT-12), ToDo, Planning.
- **Validation** : build réussi ; tests §14.22 validés par Joëlle.

## 📅 12/10/2026 — Référentiels de saisie (étape R2) : pays et civilité dans les écrans de saisie

- **Écrans** : le pays et la civilité deviennent des listes fermées alimentées par `ref_pays` / `ref_civilites` dans Cabinet (cabinet et praticien), la fiche Patient (civilité en tête de la carte « Identité civile »), la fiche Contact et la fiche Thérapeute. La fiche Intervenant affiche la civilité reprise du thérapeute, en lecture seule, comme les autres champs de trace. La liste des civilités codée en dur dans Cabinet disparaît.
- **Nouveau module** `UtilsSaisieReferentiels` (`Utils/UI`) : un seul endroit pour charger ces listes (valeurs actives + valeur courante désactivée), proposer le pays par défaut et lire la saisie.
- **Changements de comportement** :
  - le pays proposé par défaut est celui du cabinet (au lieu de « Belgique » écrit dans le code) ;
  - un praticien sans pays propre est contrôlé selon le pays du cabinet (auparavant : aucun contrôle) ;
  - un pays ajouté au référentiel sans règle est accepté sans contrôle de format.
- **Données** : l'application lit et écrit désormais `id_pays` / `id_civilite` (patients, contacts, thérapeutes, intervenants, cabinet, praticien) ; le texte `pays` n'est plus alimenté (suppression en v2.35.0). Le code ISO est lu par sous-requête et transmis aux règles de `UtilsTelephone` / `UtilsCodePostal`. Le cachet imprime le libellé du référentiel.
- **Nettoyage** : `UtilsTelephone.LibellesPays`, `LibellePaysParDefaut`, `NormaliserLibellePays` et le libellé canonique des règles (code mort), les `SelectionnerPays` des trois écrans, la constante `PaysParDefaut` de la fiche Patient et une trace de débogage à chaque enregistrement de patient (`AjouterParametresPatient`).
- **Documentation** : Database_technique, Utilitaires_Transverses, documentation UI, Process, plan de tests (§14.21), ToDo, Planning.
- **Validation** : build réussi ; tests §14.21 validés par Joëlle.

## 📅 11/10/2026 — Référentiels de saisie (étape R1) : pays, civilités, type d'organisme

- **Base de données — migration v2.34.0** (+ ROLLBACK) :
  - `ref_pays` : code ISO 3166-1 alpha-2 et libellé ; les 5 pays dotés de règles de validation (BE, FR, LU, DE, NL) sont des lignes système, puisque ces règles restent dans le code, indexées par le code ISO ;
  - `ref_civilites` : libellé et abréviation pour les courriers (Madame / Mme, Monsieur / M., Mx) ;
  - `ref_organismes.type_organisme` : `REMBOURSEMENT` (domaines) ou `MUTUALITE` (patients), avec une contrainte CHECK ; amorçage des 7 organismes assureurs belges, le code étant le numéro INAMI (100 à 900) ;
  - colonnes de rattachement : `id_pays` (cabinet, praticiens, patients, contacts, thérapeutes), `id_civilite` (praticiens, patients, contacts, thérapeutes, intervenants), mutualité du patient (`id_organisme_mutualite`, `numero_affiliation`, `code_titulaire_1` / `code_titulaire_2`, 3 chiffres) ;
  - conversion des textes reconnus : le pays du cabinet et du praticien → Belgique, la civilité du praticien → Madame. Les colonnes texte restent en place jusqu'à la v2.35.0.
- **Écrans** : nouveaux référentiels `UC_Pays` (code ISO contrôlé, colonne « Contrôles ») et `UC_Civilites` (abréviation), avec leurs tuiles (5e colonne des groupes Cabinet et Patient) et leurs icônes. `UC_Organismes` gagne une liste et une colonne **Type**, verrouillées sur une ligne système ; la tuile devient « Organismes et mutualités ». **Changement de comportement** : la liste Organisme des Domaines ne propose plus que les organismes de remboursement.
- **Cadenas** (décision du 11/10) : on garde le fonctionnement actuel. Le code, l'activation et désormais le type d'organisme sont verrouillés ; le libellé, l'ordre et les champs descriptifs restent modifiables. Pour les nouveaux référentiels, ce verrou est aussi assuré dans les requêtes `UPDATE`.
- **Corrections** : 21 icônes de boutons (cabinet, périodicité, situation juridique, lier / délier / générer / importer) n'étaient pas copiées à la sortie, faute d'entrée dans `Althea.vbproj`. La tuile Périodicités visait aussi un nom d'icône inexistant (`periodicites_normal`). Leurs icônes de survol et désactivées s'affichent désormais.
- **Documentation** : Database_technique, Parametres_Referentiels, documentation UI, plan de tests des référentiels (§14.17 à §14.20), ToDo et Planning (numéros de migration des lots 30 à 32 décalés : v2.36.0 à v2.38.0).
- **Validation** : la migration a été testée sur une copie de la base sous MariaDB 10.11, avec la collation remplacée pour ce test : application, rejeu sans effet, retour arrière, réapplication et contraintes CHECK. Sur la base de développement : build réussi, migration appliquée, tests §14.17 à §14.20 validés par Joëlle.

## 📅 11/10/2026 — Import d'image unifié

- **Nouveaux modules** : `ImagesHelper` (`Utils/Helpers`, sans interface — formats acceptés, contrôle du fichier, copie ou enregistrement PNG sous un nom fixe, chargement sans verrou) et `UtilsImages` (`Utils/UI` — dialogue de choix commun, confirmation de remplacement, remplacement d'une image affichée).
- **Factorisation** : la photo patient, le logo du cabinet et les images du praticien (photo, signature, cachet) passent par ces modules ; `CheminsPatientHelper.ImporterPhotoIdentite` remplace `GetNomFichierPhotoIdentite` + `SupprimerAnciennesPhotosIdentite` ; `CheminsCabinetHelper` délègue copie, enregistrement et chargement. Quatre copies du même code supprimées (fiche Patient, Cabinet, récapitulatif de la fiche Dossier, bandeau de la Home). Le logo de domaine (Suite lot 27) s'y branchera.
- **Changements de comportement (validés le 11/10)** : le fichier choisi doit être **réellement lisible comme image** (un fichier renommé ou abîmé est refusé avec un message) ; **mêmes formats partout** (JPG, PNG, BMP, GIF, TIFF — le Cabinet n'acceptait que PNG, JPG, BMP) ; **confirmation avant de remplacer** une image existante, désormais aussi pour le logo, la photo, la signature et le cachet (import ou génération).
- **ToDo** : réduction des très grandes images à l'import, notée pour plus tard.
- **Validation** : build réussi ; imports, remplacements avec confirmation, refus d'un faux fichier image et affichages vérifiés par Joëlle.

## 📅 11/10/2026 — Home : bandeau et menu redessinés, outils de test dans l'Accueil

- **Bandeau de la Home** (maquette « Home A ») : logo Althéa ; logo et nom du cabinet, praticien (« Prénom Nom · Titre », praticien lié au compte connecté sinon praticien principal), mis à jour après un enregistrement dans Cabinet ; **fil d'Ariane** dessiné (`AltheaFilAriane`) à partir du texte que les écrans envoient déjà — chevrons, dernier segment en gras, précisions atténuées, repli « … » ; **pastille du domaine** du dossier ouvert ; **pastille de l'utilisateur** (`AltheaPuceUtilisateur` : initiales, nom, rôle, « Élévation »). Les cadres en relief disparaissent.
- **API** : `UserControlContext.SetHeader(message, pastille)` — second paramètre facultatif ; un appel sans pastille l'efface. La fiche Dossier ne met plus le domaine dans le texte du fil.
- **Menu de gauche** : boutons `AltheaBoutonMenu` (pastilles arrondies, écran actif Sauge foncé, survol Gris-vert clair — `UITheme.ButtonHomeBackHover` modifié) ; Outils / Admin et « Changer mon mot de passe » sous un filet. États et icônes toujours gérés par `UtilsButtons`.
- **Outils de test** (Hash mot de passe, DialogChoix, RichTextEditor) : retirés du menu, placés dans une carte de l'Accueil **visible uniquement en Debug**.
- **ToDo sécurité** : mot de passe en clair dans l'outil de test « Hash » et clé Syncfusion dans `Program.vb`, à traiter lors de la revue de code.
- **Validation** : build réussi (une erreur de compilation corrigée : variable masquant une constante, VB ne distinguant pas la casse) ; rendu validé par Joëlle.

## 📅 11/10/2026 — UI-5 terminé : Cabinet, Paramètres, texte en normal

- **Cabinet** : les quatre `GroupBox` (Logo du cabinet, Photo, Signature, Compte de travail) deviennent des cartes du kit ; leurs boutons passent en variantes Outil (Importer… sur fond gris-vert, Délier en terre ; les trois boutons du compte sont empilés). L'image de panneau arrondi étirée derrière la grille de l'onglet Cabinet est remplacée par une carte et retirée du `.resx`.
- **Texte des pages en normal** sur Cabinet (4 onglets) et Domaines (5 onglets) : le gras hérité de l'ancienne police d'onglets disparaît ; les libellés volontairement gras le restent (décision du 10/10).
- **Paramètres** : case « Afficher les paramètres désactivés » du kit (`AltheaCase`).
- **Plan UI** : UI-5 clos ; reportés par décision du 11/10 — `GroupBox` de Domaines (UI-6) et des fenêtres (UI-7), `UC_Accueil` (lot 31).
- **Validation** : build réussi ; Cabinet (quatre onglets) et Domaines vérifiés à l'exécution. La case de Paramètres, convertie ensuite, reste à vérifier.

## 📅 11/10/2026 — UI-5 (suite) : fiche Patient, Suivi clinique, bandeaux de recherche des onglets

- **Fiche Patient** (maquette « Fiche patient ») : le bandeau identité devient une carte — photo, nom, code et statut en pastilles (« Actif · dossier Actif », pastille beige si inactif), naissance et âge, boutons Outil « Photo… » et « Désactiver » (terre) / « Activer » (sauge), bloc « Alerte / Notes » à droite sur fond terre très léger s'il existe une note. Onglet Identité : les quatre `GroupBox` deviennent des cartes ; **Alerte / Notes passe sous Coordonnées** (en pleine largeur, il ne lui restait qu'environ 40 px). Le cadre « Anamnèse » redondant avec la carte devient un simple panneau (`pnlAnamnese`).
- **Suivi clinique** (maquette « Suivi clinique A ») : nouveau contrôle `AltheaLigne` — une ligne cliquable par zone (icône, libellé, aperçu, « Ouvrir »), dans une carte ; l'aperçu riche au survol s'ouvre désormais sous la ligne. Comportement inchangé (ouverture de l'éditeur, lignes inactives avant enregistrement, lecture seule si archivé). **Correction** : la dernière zone s'étirait sur toute la hauteur libre.
- **Onglet Documents** : carte avec message d'attente.
- **Bandeaux de recherche des onglets** au modèle UI-4 : Famille / Contacts, Intervenants, Séances et Paiements (`AltheaRecherche`, `AltheaListe` à étiquette, `AltheaCase` « Du » ; la liste « Période » des séances, souvent masquée, passe en dernier). Les sélecteurs de date seront habillés en UI-7.
- **Planning** : mutualité — numéro d'affiliation du patient, consultation de la fiche complète et ajout d'une mutualité depuis la fiche Patient (référentiels de saisie, après UI-5).
- **Documentation** : plan UI, charte (kit, écrans habillés), référence des propriétés (`AltheaLigne`), règles (ligne de remplissage des `TableLayoutPanel`), documentation UI (fiche Patient, fiche Dossier).
- **Validation** : build réussi ; fiche Patient (bandeau, Identité, Anamnèse), Suivi clinique, Documents et les quatre bandeaux de recherche vérifiés à l'exécution.

## 📅 11/10/2026 — UI-5 : récapitulatif patient et onglet Fiche dossier

- **Récapitulatif patient de la fiche Dossier** (maquette « Récapitulatif A, blocs groupés ») : sous l'en-tête, une carte à quatre blocs toujours visible — **Patient** (photo ou initiales, nom, naissance et âge, note d'alerte), **Dossier** (domaine, statut, ouverture, clôture, Changer le statut…), **Finances** (tarif, montants dus mis en évidence, avoir), **Quotas**. Nouveau contrôle `AltheaJauge`.
- **Changement de comportement — bloc Quotas** : piloté par le domaine et non plus par la présence de séances. Titre « QUOTAS — <organisme> » pour un domaine soumis à quota (l'INAMI n'est plus nommé en dur), « Domaine sans quota » sinon ; un dossier encore vide d'un domaine à quota affiche « Aucune séance comptabilisée ».
- **Onglet Fiche dossier** (maquette « Fiche dossier B ») : carte « Paramètres du dossier » (trois champs en ligne) et carte « Convention du domaine ». **`UC_SyntheseDomaine` V2.0.0** : la RichTextBox est remplacée par des blocs du kit (Règles, Tarifs en vigueur, Natures en pastilles, Quota applicable, Points d'attention), en trois colonnes si la largeur le permet, empilés sinon — l'onglet Synthèse des Domaines en profite. Nouveau contrôle `AltheaPastille`. Emplacement de la miniature du logo de domaine prévu (Suite lot 27).
- **Factorisation** : `Patient.CalculerAge` et `CheminsPatientHelper.ChargerImagePatient` (fichier non verrouillé) partagés entre la fiche Patient et le récapitulatif.
- **Charte** : nouvelle teinte `UITheme.ColorGrisDiscret` (#767676, texte secondaire lisible).
- **Correction** : dans le récapitulatif, les panneaux sans taille déclarée faisaient déborder l'alerte et masquaient le bouton « Changer le statut… » ; règle ajoutée dans `Rules.md`.
- **Décision (logo de domaine, à réaliser en Suite lot 27)** : fichier nommé par l'identifiant du domaine (`Domaines/Domaine_{id}/Logo.png`) dans le dossier de données, nom de fichier seul en base (`ref_domaines.logo_fichier`), comme les images du cabinet — ni dans `Assets` (dossier d'installation), ni d'après le libellé (renommable).
- **Documentation** : plan UI (avancement UI-5, kit), charte (kit, écrans habillés, palette), référence des propriétés, règles, documentation UI (fiche Dossier, `UC_SyntheseDomaine`), planning et ToDo (logo de domaine).
- **Validation** : build réussi ; récapitulatif vérifié à l'exécution (alerte, bouton, largeurs, Quotas avec et sans organisme) ; onglet Fiche dossier et synthèse vérifiés. Après vérification, les trois teintes de la synthèse ont été rattachées à `UITheme` (même rendu, à recompiler).

## 📅 11/10/2026 — Barres locales, point Phase 4 bis, sauvegarde avec données retirée du dépôt

- **Barres locales distinctes de la barre de l'écran** (ADR-34) : maquette « Barres locales » (version B retenue). Nouveau contrôle `AltheaBarreOutils` et variantes `Outil`, `OutilPrincipal`, `OutilDanger` d'`AltheaBouton` (V1.1.0). Posées sur 7 listes : fiche Patient (Anamnèse, Famille, Intervenants, Dossiers), fiche Dossier (Séances, Paiements), Cabinet (Cachet) — la grille et ses boutons sont réunis dans une carte, Supprimer en terre et en dernier, boutons alternés sur la même place. Code des écrans inchangé. Vérifié à l'exécution (largeur des boutons corrigée après le premier essai).
- **Planning consolidé** : reste à faire de la Phase 4 bis regroupé d'après les plans Copilot (logo de domaine, photo / préférences / audit, référentiels Mutualités / Pays / Civilités, import d'image unifié, bandeau Home, sauvegarde) ; UI-5 précisé (bandeau identité, onglets formulaires, Suivi clinique) ; UI-6 : refonte complète de Domaines.

- **Sécurité / RGPD** : `Docs/Database/backup_WithData_althea.sql` (sauvegarde complète, données comprises) n'est plus suivi par Git et figure dans `.gitignore` ; seule la sauvegarde de structure reste versionnée. Le fichier reste sur le disque local. Les commits antérieurs le contiennent encore (dépôt privé `AngeljoNG/Althea`).
- **Planning** : tableau de la Phase 4 bis réparé (lignes 27 et 29 à 32 tronquées depuis leur création, reconstituées et signalées comme telles) ; statut « En cours » ; lot 31 ajouté au ToDo.

## 📅 10/10/2026 — Kit d'interface : hubs habillés, onglets redessinés, en-tête commun, bandeau de recherche, barres d'actions, scintillement corrigé, situations juridiques en Administration

- **Plan de chantier** : `Docs/Todo/Plan_Refonte_UI_Althea.md` (lots UI-0 à UI-8, documentation associée, nettoyage final). Référencé depuis le ToDo, le planning et la bibliothèque de documentation.
- **Kit d'interface validé (lot UI-0)** : les contrôles de `UI/Controls/Theme/` compilent et sont manipulables dans le concepteur. **Correction** : `UITheme.ColorRougeMoyenClair` valait `(168, 20, 110)` — un magenta — au lieu de `(168, 120, 110)` prévu par la charte.
- **Les deux hubs habillés (lot UI-1)** : `UC_ReferentielHome` (15 tuiles, 4 sections) et `UC_AdminHome` (7 tuiles, 2 sections « Cabinet et accès » / « Technique », grille 4 colonnes commune). Quadrillage `InsetDouble` supprimé, images de fond retirées des Designers **et** des `.resx` (91 → 38 Ko et 65 → 27 Ko, soit 1 106 lignes de base64 en moins). Titre redondant `lblTop` supprimé dans `UC_AdminHome`. Les fichiers métier n'ont pas été modifiés pour l'habillage : `UtilsButtons` continue de charger les icônes par le `Tag`.
- **Scintillement à l'ouverture d'un écran corrigé** : composition de la fenêtre (`WS_EX_COMPOSITED` sur `Home`, V1.8), chargement de la vue masquée et mise en page suspendue dans `NavigationManager`, dessin du feuillage limité à la zone réellement repeinte, ombres du kit allégées. **Profite à tous les écrans**, pas seulement aux hubs.
- **Changement fonctionnel — Situations juridiques** : la tuile passe des Référentiels à l'**Administration** (section « Cabinet et accès »), avec les mêmes droits que la fiche Cabinet (Admin et SuperUser) ; aucun utilisateur ne perd l'accès. `UC_SituationsJuridiques` reste dans `UI/Controls/Referentiels` et hérite toujours de `UC_ReferentielBase` : seul le rattachement fonctionnel change. Recette effectuée avec les profils Admin et SuperUser.
- **Onglets redessinés (lot UI-2)** : nouveau contrôle `AltheaOnglets` (`TabControl` hérité et dessiné, ADR-32) — onglets au repos sur fond beige arrondi, onglet actif en gras souligné de sauge, trait fin à la place du cadre en relief. Validé sur `SeanceEditor`, puis posé sur `UC_DossierFiche`, `UC_PatientFiche`, `UC_Cabinet`, `UC_Parametres`, `UC_DomainesHome` et la zone « données liées » de `UC_ReferentielBase`. Seul le type des contrôles change : pages, contenu et code inchangés. Syncfusion n'est pas ajouté. Sur `UC_Cabinet` et `UC_DomainesHome`, le gras de l'ancienne police d'onglets est reporté sur les pages pour que leur contenu ne bouge pas.
- **Scintillement du feuillage corrigé dans l'onglet Séances de `UC_DossierFiche`** : la bande d'onglets Windows repeignait la feuille coupée directement à l'écran ; `AltheaOnglets` en `CouleurFond = Transparent` la recopie dans son propre tampon. Double tampon activé aussi sur les surfaces qui portent le feuillage dans la Home (`pnlForm`, `pnlHeader`, `pnlContent`, UC hébergé), et invalidation limitée à la zone du feuillage.
- **En-tête commun sur tous les écrans (lot UI-3)** : nouveau contrôle `AltheaEnTete` (icône, titre Mauve foncé, sous-titre Sauge foncé sous le titre, trait fin), qui remplace le trio `picTitre` / `lblTitreForm` / `lblTop`. Posé sur 12 UserControls et 19 Forms ; les 15 référentiels suivent via `UC_ReferentielBase`. Icônes conservées dans les `.resx` (clé renommée `enTete.Icone`). En-tête **rétabli** sur `DialogComptePraticien`. `UC_DossierFiche` : titre = code du dossier, sous-titre = patient. Textes corrigés : « Connexion » (au lieu de « Connection / database »), Login et Élévation d'accès titrés par leur fonction, « Modifier la séance » au lieu d'un doublon « Séance / Séance », coquilles.
- **Bandeau de recherche (lot UI-4, première partie)** : maquettes validées (artifact « Althéa – Bandeaux »), puis modèle commun posé sur Patients, Thérapeutes, Utilisateurs (avec une ligne « Affiner »), les 15 référentiels via `UC_ReferentielBase` (carte calée sur la largeur de la liste) et la liste des Domaines. Nouveaux champs du kit, avancés de UI-7 : `AltheaRecherche` (champ arrondi avec loupe et texte d'aide), `AltheaListe` (liste à étiquette intégrée, héritée de `ComboBox`), `AltheaCase` (case arrondie, héritée de `CheckBox`) — ADR-33. Les anciens labels au-dessus des champs deviennent le texte d'aide ; libellés harmonisés (« Afficher les inactifs », « Dernière connexion depuis », « Comptes verrouillés seulement »). Outil partagé `AltheaFondParent` pour la recopie du fond (utilisé aussi par les onglets).
- **Barres d'actions du kit sur tous les écrans (lot UI-4, seconde partie)** : les panneaux à image étirée et cadre en relief deviennent des `AltheaBarreActions` (vague de la charte) et leurs boutons des `AltheaBouton` sur 28 écrans — listes, fiches (Cabinet, Paramètres, fiche Patient, fiche Dossier) et fenêtres. Couleur selon le rôle (Enregistrer ocre, Supprimer terre, Annuler / Actualiser / Fermer clair, autres sauge) ; largeur de chaque bouton calculée sur son texte ; boutons qui alternent (Modifier / Enregistrer, Fermer / Annuler) sur une même place ; Cabinet garde son alignement automatique à droite. Images de fond retirées des `.resx`. Code des écrans inchangé ; seule `DialogChoix` garde ses boutons.
- **Panneau « Détail » des référentiels** : carte du kit avec le titre intégré (« Détail », « Nouvel élément », « Modification ») ; le label séparé `lblTitreEdition` et son recalage par le code disparaissent ; les champs remontés pour rester visibles s'arrêtent sous le titre.
- **Outil** : `Tools/build.cmd` — génère la solution et écrit le résultat dans `obj\_build.log` (ignoré par Git).
- **Documentation** : ADR-30 (contrôles dessinés), ADR-31 (place d'une tuile : l'usage métier prime sur l'héritage technique) ADR-32 (onglets) et ADR-33 (champs du kit) ; référence des propriétés (section kit, anciens `pnlTitre` / `lblTitreForm` / `lblTop` marqués remplacés), guide UI, règles, charte graphique (écrans habillés, note sur le scintillement), documentation UI, ToDo, planning, bibliothèque.
- **Validation** : build réussi ; les deux hubs vérifiés à l'exécution (affichage franc, sans reconstruction visible) ; droits testés. Onglets vérifiés à l'exécution sur les sept zones, y compris un onglet de paramètres créé à la volée ; plus aucun scintillement de la feuille dans l'onglet Séances. En-tête validé sur trois écrans pilotes puis contrôlé sur chaque famille d'écrans (hub, liste, fiche, référentiel, dialogues, Login) ; build réussi après conversion des 31 écrans. Bandeau de recherche vérifié sur les cinq écrans (recherche, Entrée, listes, cases, réinitialisation) ; texte fantôme sous les cases corrigé (style `Opaque` hérité de `ButtonBase`). Barres d'actions vérifiées sur les listes, puis par sondage sur les fiches et fenêtres (séance, paiement, Contact, Login, Cabinet, fiche Dossier) ; largeur des boutons corrigée après le premier essai (textes tronqués).

## 📅 09/10/2026 — Habillage : fond fixe de la Home, feuillage continu, kit d'interface Althéa

- **Home – fond fixe 2560x1440** : nouveau `Home_Fond_2560x1440.png` (Beige clair + vague fine) en `BackgroundImageLayout = None` sur `pnlForm` : plus aucun étirement, le fond est simplement rogné. Remplace `Fond_1000x800_Feuille4` (conservé pour les autres écrans).
- **Logo de la Home** : `Home_Logo_Althea_253x70.png` (médaillon « A » + mot Althéa, bon ratio) dans `picTitre` (`Layout = Center`, 253x70, position 14;12) ; variantes `_x2` (écrans 150 % et plus) et `Home_Titre_Althea_179x48.png` (mot seul). Le texte « Althéa » est retiré du fond.
- **Feuillage du coin haut droit** : `Home_Feuillage_CoinHautDroit.png` dessiné à taille réelle par `pnlForm_Paint`, toujours collé au bord droit quelle que soit la largeur de la fenêtre.
- **Feuille coupée automatique** : la Home redessine sur chaque UserControl chargé dans `pnlContent` la portion de feuillage qu'il recouvre, exactement alignée (`AttacherFeuillageCoupe` / `SurfaceUC_Paint`) ; l'ancien fond étiré `Fond_1000x770_FeuilleCoupee1` des UC hébergés est ignoré à l'exécution (encore présent dans leurs Designers, à retirer lors de la revue des fonds).
- **`Home.vb` V1.7** : nouvelle région « Habillage du shell ». Ressources ajoutées (`Home_Fond_2560x1440`, `Home_Feuillage_CoinHautDroit`, `Home_Logo_Althea_253x70`) et copie des nouveaux PNG vers la sortie (`Althea.vbproj`).
- **Hub Référentiels** : essai d'habillage par images de fond (`Assets/ui-backgrounds/` : boutons, panneau central, bandeau) ; jugé difficile à ajuster, il sera remplacé par le kit ci-dessous.
- **Kit d'interface Althéa (préparé, pas encore posé dans les écrans)** : maquettes validées (kit, hubs Référentiels et Administration, liste Patients, dialogue Séance, Domaines, Cabinet) et nouveaux contrôles dessinés par le code dans `UI/Controls/Theme/` : `AltheaBouton` (variantes Sauge / Ocre / Terre / Clair), `AltheaTuile` (pastille d'icône, titre + sous-titre), `AltheaCarte` (panneau translucide arrondi, titre facultatif), `AltheaBarreActions` (vague dessinée), `AltheaTitreSection` (titre + trait), outils `AltheaDessin`. Fenêtre de démonstration `UI/Forms/Test/TestKitAlthea`.
- **Documentation** : `Charte_Graphique.md` (sections « Home (shell) – fond fixe » et « Kit d'interface Althéa »).
- **Validation** : Home et feuillage vérifiés à l'exécution ; kit non encore compilé (build à faire).

## 📅 07/10/2026 II — Organismes étoffés et périodicités d'encodage (lot 29)

- **Migration v2.33.0** (+ ROLLBACK) : nouvelle table `ref_periodicites` (code, libellé, `nb_mois` nullable = durée du cycle, NULL = à la demande ; seeds MENSUELLE, TRIMESTRIELLE, ANNUELLE, A_LA_DEMANDE) ; `ref_organismes` enrichie de colonnes nullables `adresse`, `telephone`, `telephone_2`, `email`, `email_2`, `site_web`, `url_plateforme`, `procedure_encodage` et `id_periodicite` (FK `ON DELETE SET NULL`). La périodicité est portée par l'organisme, jamais par le dossier ou le patient ; aucun comportement n'est déduit d'un code.
- **Nouveau référentiel `UC_Periodicites`** (tuile « Périodicités d'encodage » du hub Référentiels, icône `Titre_periodicite`) : CRUD sur `UC_ReferentielBase`, champ « Durée (mois) » + case « Non périodique », colonne « Mois ». Couche `Periodicite` / `GestionPeriodicites` / `QueryPeriodicites`.
- **`UC_Organismes`** : panneau Détail complété par Périodicité (liste, « (aucune) » = NULL), Téléphone / Téléphone 2, E-mail / E-mail 2, Site web, Plateforme, Adresse, Procédure (multiligne) ; validation souple tél./e-mail à l'enregistrement (avertissement ciblé, fenêtre non fermée) ; champs vides enregistrés en NULL ; colonne « Périodicité » dans la liste. `Organisme` / `GestionOrganismes` / `QueryOrganismes` étendus (LEFT JOIN périodicité).
- **`ReferentielLigne.Complement`** : nouvelle propriété texte générique pour une information complémentaire en liste (évite de détourner `Tarif`).
- **Tests et documentation** : `PLAN_TESTS_REFERENTIELS.md` (14.12.4–14.12.8, nouvelle section 14.16, 135 tests), `Database_technique.md`, documentation UI, charte graphique (icône), ToDo, état du projet. Build réussi ; recette manuelle à exécuter.

## 📅 07/10/2026 I — Compte de travail du praticien (lot 28), mode consultation / Modifier de la fiche Cabinet

- **Migration v2.32.0** (+ ROLLBACK) : `cab_praticiens.id_utilisateur` → `sec_utilisateurs`, nullable, unique (un compte → un seul praticien), `ON DELETE SET NULL`. Aucun compte n'est créé, modifié ni supprimé par la migration.
- **`UC_Cabinet` › onglet Praticien** : nouvelle section « Compte de travail (SuperUser) » avec état du compte lié et actions **Créer le compte…**, **Lier un compte existant…**, **Délier** ; actives dès que le praticien est enregistré, indépendamment du mode Modifier.
- **Nouveau `DialogComptePraticien`** (`UI/Forms/Cabinet`, Designer) : création d'un compte `SuperUser` avec `role_max_elevation = Admin` (élévation par le cadenas) et mot de passe temporaire à changer à la première connexion ; ou liaison d'un SuperUser actif non encore lié. **Le compte Admin technique n'est jamais proposé ni touché.**
- **`GestionCabinet`** : `GetUtilisateursLiables`, `LierUtilisateur` (journalisé, refuse un compte déjà lié ailleurs), `CreerCompteSuperUser`, `GetPraticienParUtilisateur` ; `Praticien` enrichi des informations du compte lié.
- **`UC_Cabinet`** : mode **consultation / Modifier** (champs en lecture seule, grilles verrouillées, imports désactivés hors édition) ; **Annuler** désormais toujours actif en édition (retour direct en consultation sans modification) ; correction du bouton d'import du logo (`grpLogo` désactivé dans le Designer).
- **Tests et documentation** : `PLAN_TESTS_UC_CABINET.md` (72 tests, section 10 compte de travail), documentation UI de `UC_Cabinet`, base de données, planning, ToDo, état du projet.

## 📅 06/10/2026 — Fondation Cabinet / praticien (lot 27), situations juridiques

- **Migrations** : v2.29.0 (`cab_cabinet`, `cab_praticiens`, `cab_praticien_identifiants`, `cab_praticien_qualifications`, `ref_types_identifiant_pro`), v2.30.0 (suppression `cab_praticiens.couleur_agenda`), v2.31.0 (`ref_situations_juridiques` avec `porteur_bce`, `cab_cabinet.id_situation_juridique`) — avec ROLLBACK.
- **Nouveau `UC_Cabinet`** (tuile « Cabinet » de l’Administration) : onglets Cabinet, Praticien, Identifiants, Cachet ; import de la photo praticien et **cachet généré** (`GenerateurCachet`) validés ; protection des modifications non enregistrées (`IUnsavedChangesAware`).
- **Nouveau référentiel `UC_SituationsJuridiques`** (tuile dans les Référentiels) avec champ « Porteur du BCE » ; le BCE est saisi à un seul endroit (cabinet ou praticien) et tout déplacement est proposé avec confirmation.
- **Contrôles** : `UtilsIdentifiantBelge` étendu (IBAN mod 97, BIC) en plus d’INAMI, BCE et TVA.
- **Habillage** : styles de `UC_Cabinet` déplacés dans le Designer (seules les grilles restent stylées par code) ; nouvelles icônes `cabinet` et `situationJuridique` (titre et tuiles).
- **`UC_ReferentielBase`** : panneau Détail élargi (320 → 410 px) pour supprimer l’ascenseur horizontal des champs supplémentaires.
- **Corrections** : requêtes `QueryCabinet` rétablies complètes (lecture/écriture de toutes les colonnes).
- **Documentation** : base de données, structure de la solution, UI, charte graphique, règles, ADR-29, planning (lots 28 à 32 renumérotés v2.32.0 → v2.36.0), ToDo et état du projet.

## 📅 05/10/2026 — Synthèse de domaine partagée, renommage des modales de paiement

- **Nouveau `UC_SyntheseDomaine`** (`UI/Controls/Domaines`) : synthèse mise en forme d’un domaine (convention, tarifs en vigueur, natures, quota, complétude), source unique des règles de complétude. Remplace la liste de l’onglet Synthèse de `UC_DomainesHome`.
- **`UC_DossierFiche`** : encadré de synthèse du domaine à droite de l’onglet Fiche Dossier, actualisé à chaque choix de domaine.
- **Renommage** : `PaiementsSeanceDialog` → `DialogPaiementsSeance`, `PaiementReaffectationDialog` → `DialogPaiementReaffectation` (code, Designer, resx, illustrations, documentation).
- **Documentation UI** : explication des onglets de `UC_DomainesHome`, mécanisme du cadenas (lignes système) et lignes verrouillées par référentiel, section `UC_SyntheseDomaine`.

## 📅 02/10/2026 — Sécurisation des saisies par pays

- **Nouveaux utilitaires** : `UtilsCodePostal` (Belgique 4 chiffres, France 5 chiffres, Luxembourg 4 chiffres avec « L- » toléré ; autres pays non contrôlés) et `UtilsNiss` (NISS belge clé modulo 97 y compris naissances 2000+, NIR français avec clé et Corse 2A/2B, matricule luxembourgeois Luhn + Verhoeff ; type « Autre » non contrôlé).
- **`UC_PatientFiche`** : téléphone désormais validé selon le pays (`UtilsTelephone.Valider`, comme Contact/Thérapeute) ; code postal validé selon le pays ; nouveau sélecteur de type d’identifiant à côté du NISS (Belgique par défaut, déduit au chargement, non stocké) ; contrôle du format avant l’unicité.
- **`ContactEdition` / `TherapeuteEdition`** : validation bloquante du code postal selon le pays.
- **Moment du contrôle** : erreur signalée dès la sortie du champ et à chaque changement de pays/type ; blocage à l’enregistrement. Liste des pays inchangée.
- **E-mail** : `UtilsValidation.IsValidEmail` durci (points consécutifs ou en bord, espaces, domaine sans extension, extension d’une lettre refusés ; 254 caractères max) ; erreur affichée dès la sortie du champ dans la Fiche patient, Contact et Thérapeute, toujours bloquante à l’enregistrement.
v1.1 (PAT-35 à PAT-49)
- **Validation** : build réussi ; recette manuelle à exécuter.
- **`UC_Domaines` archivé** : déplacé dans `UI/Forms/Test` et exclu de la compilation (`Althea.vbproj`) ; la tuile Référentiels › Domaines ouvre désormais `UC_DomainesHome` (écran unique). Plans de tests et documentation alignés.
- **`SeanceEditor`** : statut documentaire passé à « Complet » (recette effectuée).

## 📅 01/10/2026 — Domaine = convention : finalisation, nettoyage du modèle quota (v2.27.0 → v2.28.0), documentation

- **Migrations v2.27.0 / v2.28.0** (lots 25-26, + ROLLBACK) : suppression des colonnes héritées de l’ancien modèle quota (`ref_natures_seance.code_prestation_inami`, `ref_natures_seance.id_categorie_quota`, `ref_quotas.plafond_soutien` / `plafond_traitement`).
- **Enchaînement des règles de quota** : une règle n’est jamais modifiée ; l’ajout d’une nouvelle règle clôture la règle ouverte la veille de sa date de début.
- **`UC_DomainesHome`** finalisé (Général / Tarifs / Catégories & quotas / Natures / Synthèse, assistant) avec `CategoriesQuotaEditor` ; ajout rapide d’organisme.
- **Guide utilisateur** `Docs/Guide/Guide_Utilisateur_Domaines.md`.
- **Documentation** (refonte complète) : ADR-26 (séances ↔ paiements) fusionné dans `Concepts_Metier_Althea.md`, ADR-27 (Domaine = convention) et ADR-28 (lignes système `est_systeme`) ; `ARCHITECTURE_DECISIONS.md` resynchronisé ; mise à jour de `Database_technique.md`, `Documentation_technique_UI_Althea.md`, `Structure_Solution_Althea.md`, `Parametres_Referentiels.md`, `Process_Althea.md`, `Interface_Utilisateur.md`, `Architecture_Technique.md`, `Utilitaires_Transverses.md`, `Bibliotheque_Documentation.md`, `Rules.md`, `ToDo.md`, `ETAT_DU_PROJET.md`, `Planning_actions_Althéa.md`, `README.md`.
- **Plans de tests** : nouveau `PLAN_TESTS_DOMAINES.md` (D-01 à D-82) ; `PLAN_TESTS_TARIFICATION_QUOTAS.md` v3.0 (catégories, plafonds, statuts consommateurs, TQ-13a à TQ-21) ; `PLAN_TESTS_REFERENTIELS.md` (organismes, lignes système, `consomme_quota`) ; `PLAN_TESTS_PARCOURS_SUIVI.md` ; `STRATEGIE_TESTS_ALTHEA.md`.
- **Validation** : build et tests réussis.

plafonds de quota (v2.21.0 → v2.26.1)

- **Modèle** : un domaine = tarif + organisme + quotas (ADR-27). Nouveau référentiel `ref_organismes` (INAMI ligne système), `ref_quotas_inami` renommée `ref_quotas`, catégories de quota par domaine `ref_categories_quota`, plafonds par règle `ref_quotas_plafonds`, catégorie consommée portée par la liaison nature–domaine, statuts consommateurs `ref_statuts_seance.consomme_quota`.
- **Protection des lignes système** des référentiels (`est_systeme`) : ni suppression ni modification du code.
- **UI** : nouvel écran `UC_DomainesHome` (menu Home › Domaines), référentiel `UC_Organismes`, contrôle commun `UC_Pagination` ; `UC_Domaines` conservé transitoirement côté Référentiels.
- **Calcul des quotas** (`GestionQuotaInami`) généralisé à N catégories, alerte de dépassement inchangée (non bloquante).

---

## 🗄️ Mois précédents

Les entrées de mars à septembre 2026 sont dans [Docs/Archives/CHANGELOG_2026.md](Docs/Archives/CHANGELOG_2026.md). Au début de chaque mois, les entrées du mois précédent y rejoignent les autres (une archive par année).

[TOC]
