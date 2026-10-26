# Plan de tests — Parcours de suivi transversal

> Version : 1.11 — Dernière mise à jour : 26/10/2026 — tests automatisés (audit D-2) : D2-01 à D2-08 ; intégration continue (audit D-3) : D3-01, D3-02.
> Référence : Processus 15 à 20 de [`Docs/Architecture/Processus/Processus_Metier.md`](../Architecture/Processus/Processus_Metier.md).

## Objectif

Recetter de bout en bout le parcours **Patient → Domaine → Dossier → Séance → Tarif → Quota → Paiement**, y compris les effets des évolutions de référentiel sur le futur sans altérer l’historique.

## Préparation

- Créer un patient de recette identifié `TEST_PARCOURS`.
- Préparer un domaine standard et un domaine conventionné soumis au quota via l’écran **Domaines** (`UC_DomainesHome`, synthèse « Domaine complet »), avec organisme, natures rangées par catégorie, tarifs, périodes et modes de paiement actifs.
- Préparer une règle de quota avec plafonds par catégorie et seuil atteignable en quelques séances de test.
- Conserver les identifiants et montants paramétrés dans la fiche d’exécution ; ne pas utiliser de code ou montant supposé dans les résultats attendus.

## Scénarios de recette

| # | Parcours | Étapes | Résultats attendus | État |
|---|---|---|---|---|
| X-01 | Suivi standard complet | Créer patient, dossier standard, séance réalisée, paiement total encaissé. | Tarif patient du dossier figé ; paiement lié ; séance Soldé. | ☐ |
| X-02 | Paiement partiel | Créer séance due, encaisser deux montants partiels. | Reste dû calculé après chaque paiement puis Soldé. | ☐ |
| X-03 | Prévision puis annulation | Créer paiement en attente, puis paiement annulé. | Ces paiements ne diminuent jamais le reste dû ; affichages conformes. | ☐ |
| X-04 | No-show non facturable | Créer no-show dans domaine dont le comportement l’interdit. | Tarif 0, aucun paiement dû, aucune consommation quota. | ☐ |
| X-05 | Domaine conventionné | Créer séance dans la période tarifaire active. | Tarif de liaison domaine–prix appliqué à la date de séance. | ☐ |
| X-06 | Alerte quota | Enregistrer des séances consommantes jusqu’au seuil. | Compteurs corrects et alerte conforme au référentiel. | ☐ |
| X-07 | Évolution de tarif | Modifier le référentiel pour une période future, créer séance avant/après changement. | Chaque séance conserve le tarif de sa date ; l’ancienne ne change pas. | ☐ |
| X-08 | Évolution de quota | Ajouter une nouvelle règle de quota (l’ancienne se clôture la veille), créer séances avant/après date d’effet. | Règle applicable déterminée par date, historique explicable.
| X-09 | Cycle de vie dossier | Clôturer, réouvrir puis archiver après les contrôles nécessaires. | Transitions autorisées seulement ; archivé entièrement en lecture seule. | ☐ |
| X-10 | Avoir après report | Reporter une séance encaissée et utiliser l'avoir pour la nouvelle séance (21/10/2026 : remplace la réaffectation). | Paiement d'origine sur sa séance avec la mention ; paiement « Avoir » sur la nouvelle ; deux soldes recalculés. | ☐ |
| X-11 | Droits et traçabilité | Répéter une création/modification avec compte insuffisamment autorisé. | Action refusée ; données inchangées ; événement journalisé si prévu. | ☐ |
| X-12 | Résilience | Simuler indisponibilité de la base lors d’une séance et d’un paiement. | Message maîtrisé, aucun enregistrement partiel ni liaison orpheline. | ☐ |

## Non-régression — codes système en un seul endroit (audit C.3 étape 1, 24/10/2026)

Aucun comportement ne doit changer : seuls les codes des statuts et des documents sont lus à un autre endroit (`CodesSysteme`, ADR-54). Données de test uniquement.

| # | Scénario | Résultat attendu | État |
|---|---|---|---|
| C3-01 | Dossier de test : Actif → En pause → Actif → Clôturer (date) → Réouvrir → Clôturer → Archiver. Écran Patients : filtre Suivi (Actif, En pause, Clôturé, Archivé). | Mêmes transitions proposées qu'avant ; date de clôture demandée seulement pour Clôturé ; pastilles et icônes de statut ; le filtre retrouve chaque patient. | ✅ |
| C3-02 | Séances : créer (statuts proposés), reporter une séance planifiée, ouvrir la séance reportée ; filtre « Reportées » ; Planning du jour avec une annulée et une reportée. | Création : Planifiée et Réalisée seulement ; « Reportée » jamais proposée en modification ; une reportée ne se supprime ni ne s'encaisse ; le planning montre les créneaux libérés par l'annulée et la reportée. | ✅ |
| C3-03 | Paiements : encaisser une séance, annuler l'encaissement (motif), utiliser un avoir ; onglet Paiements et recherche des paiements ; écran Patients, colonne Paiements. | Statuts, totaux par mode, montant dû, compteurs encaissés / en attente / annulés identiques à avant ; « Réaligner » ne signale rien d'anormal. | ✅ |
| C3-04 | Écran Patients : patients avec dossier actif, en pause, clôturé, archivé, sans dossier. | Dossier dominant et icônes comme avant (un dossier clôturé ou archivé n'est pas « vivant »). | ✅ |
| C3-05 | Exporter l'anamnèse en PDF puis en Word ; exporter la zone clinique d'un dossier ; importer une photo d'identité. | Documents enregistrés ; contrôle : `SELECT d.id_document, t.code_type_document, d.type_source_document FROM documents d JOIN ref_types_documents t ON t.id_type_document = d.id_type_document ORDER BY d.id_document DESC LIMIT 4;` → ANAMNESE / PDF, ANAMNESE / LOCAL_DOCX, ZONE_CLINIQUE…, PHOTO_IDENTITE / LOCAL. | ✅ |
| C3-06 | Journal de la session. | Aucune erreur SQL. | ✅ |

## Drapeaux des référentiels (audit C.3 étape 2, v2.57.0, 24/10/2026)

Les règles « séance non tenue », « statut proposé à la création » et « lien / rôle par défaut d'un contact » sont lues dans le référentiel (ADR-54). Valeurs de départ posées par la migration, modifiables ensuite. Données de test uniquement.

| # | Scénario | Résultat attendu | État |
|---|---|---|---|
| C3-07 | Appliquer `Docs/Database/Migration/migration_v2.57.0_drapeaux_referentiels.sql` (HeidiSQL), puis démarrer Althéa. Contrôle : `SELECT code_statut_seance, est_absence, est_non_tenue, propose_creation FROM ref_statuts_seance ORDER BY ordre_affichage;` | Démarrage sans alerte de version (2.57.0). Non tenue : ANNULEE_PATIENT, ANNULEE_PRATICIEN, REPORTEE, MALADE ; proposé à la création : PLANIFIEE, REALISEE ; ABSENTE : `est_absence` seul ; TEST_C1 : rien de coché dans les deux nouvelles colonnes. | ✅ |
| C3-08 | Paramètres › Référentiels › **Statuts de séance** : parcourir les lignes. | Deux nouvelles cases : « Non tenue, libère le créneau » (ligne Créneau) et « Proposé à la création d'une séance » (ligne Création), avec infobulle, cochées selon C3-07 ; un nouvel élément les a décochées. | ✅ |
| C3-09 | Décocher « Proposé à la création » sur **Planifiée**, enregistrer. | Refus : « « Planifiée » reste proposé à la création : c'est le statut d'une nouvelle séance. » ; rien n'est modifié. | ✅ |
| C3-10 | Cocher « Proposé à la création » sur **TEST_C1** ; nouvelle séance. Puis le décocher. | TEST_C1 apparaît dans la liste des statuts d'une nouvelle séance, puis n'y apparaît plus. En modification : tous les statuts actifs sauf « Reportée » (sauf si c'est le statut courant). | ✅ |
| C3-11 | Cocher « Non tenue, libère le créneau » sur **TEST_C1**. Mettre une séance de test en TEST_C1, puis créer une autre séance au même horaire ; ouvrir le Planning du jour. | La nouvelle séance est acceptée (pas de message de chevauchement) ; le Planning du jour montre le créneau de la séance TEST_C1 comme libre. Remettre ensuite la séance TEST_C1 en Planifiée : refus « Ce créneau chevauche la séance SE… ». Constat de Joëlle (24/10) : si la nouvelle séance est créée sans quitter le dossier, le Planning du jour affiche encore la séance libérée (affichage seulement, l'enregistrement passe) — ToDo. | ✅ |
| C3-12 | Séance de test en **Maladie**, puis une autre séance au même horaire. | Acceptée : Maladie libère désormais son créneau (valeur de départ, comme les annulées et la reportée). | ✅ |
| C3-13 | Séance de test en **Absence non excusée**, puis une autre séance au même horaire ; puis passer une séance annulée en Absence alors que son créneau a été repris. | Absence : le créneau reste occupé (refus de chevauchement pour la nouvelle séance). Annulée → Absence sur un créneau repris : accepté, sans contrôle (passage d'un statut non tenu à un autre, règle du 22/10). | ✅ |
| C3-14 | Paramètres › Référentiels › **Liens patient** puis **Rôles légaux** : case « Par défaut : Proposé pour un nouveau contact » (libellé raccourci le 25/10/2026). Cocher un autre lien (ex. Père), enregistrer, revenir sur Mère. | Mère et Autorité parentale cochées au départ. Après avoir coché Père : Mère n'est plus cochée (une seule valeur par défaut) ; de même pour les rôles. | ✅ |
| C3-15 | Fiche patient › Famille / Contacts › nouveau contact ; puis ouvrir un contact existant. Remettre ensuite Mère par défaut. | Nouveau contact : le lien et le rôle par défaut du référentiel sont présélectionnés (Père après C3-14) ; aucun par défaut coché : la liste reste sur sa première entrée. Contact existant : ses propres valeurs, inchangées. | ✅ |
| C3-16 | Journal de la session ; non-régression de C3-02 (report, filtre « Reportées », séances grisées et montant dû des statuts non dus). | Aucune erreur SQL ; comportements de C3-02 inchangés. | ✅ |

## Lignes système déverrouillées (v2.58.0, 25/10/2026)

| # | Scénario | Résultat attendu | État |
|---|---|---|---|
| C3-17 | Appliquer `Docs/Database/Migration/migration_v2.58.0_lignes_systeme_deverrouillees.sql` (HeidiSQL), puis démarrer Althéa ; requête de contrôle en fin de script. | Démarrage sans alerte de version (2.58.0). `est_systeme` = 0 pour ABSENTE, ANNULEE_PATIENT, ANNULEE_PRATICIEN, MALADE, MERE, AUTORITE_PARENTALE ; = 1 pour PLANIFIEE, REALISEE, REPORTEE. | ✅ |
| C3-18 | Référentiels › Statuts de séance, Liens patient, Rôles légaux. | Pastille « Système » seulement sur Planifiée, Réalisée, Reportée ; plus aucune dans Liens patient et Rôles légaux. Sur Maladie (ou Mère) : « Modifier » rend le code et la case Actif modifiables, « Désactiver » est disponible (ne pas enregistrer). Planifiée : code et Actif toujours verrouillés. | ✅ |
| C3-19 | Non-régression : une séance en Maladie libère toujours son créneau ; une absence reste comptée ; un nouveau contact reprend Mère et Autorité parentale. | Comportements de C3-11 à C3-15 inchangés (les cases ne bougent pas). | ✅ |

## Non-régression — fiches réparties en fichiers partiels (audit C.5, 25/10/2026)

Code déplacé à l'identique (vérifié par comparaison avec la version précédente) : rien ne doit changer. Données de test uniquement.

| # | Scénario | Résultat attendu | État |
|---|---|---|---|
| C5-01 | Compiler ; Explorateur de solutions. | Compilation sans erreur ni nouvel avertissement ; sous `UC_DossierFiche.vb` et `UC_PatientFiche.vb`, leurs fichiers partiels (ouverts comme du code, sans Designer) ; le Designer des deux fiches s'ouvre comme avant. | ✅ |
| C5-02 | Dossier : onglets Fiche, Suivi clinique (survol et ouverture d'une zone), Séances (filtre, page, nouvelle séance, report, encaissement) et Paiements (filtre, résumé cliquable, annulation d'un encaissement de test). | Comme avant. | ✅ |
| C5-03 | Fiche patient : Identité (modifier, enregistrer), Famille / Contacts et Intervenants (ajouter, ouvrir, supprimer une fiche de test), Dossiers (nouveau dossier, ouvrir, Retour). | Comme avant ; la question « modifications non enregistrées » est posée comme avant. | ✅ |
| C5-04 | Recherche par code d'un SE et d'un PAI (ouverture ciblée sur l'onglet et la ligne) ; journal de la session. | Le dossier s'ouvre sur le bon onglet, ligne sélectionnée ; aucune erreur au journal. | ✅ |

## Non-régression — Option Strict On pour tout le projet (audit D-1, 25/10/2026)

12 lectures d'identifiant corrigées (`GetUInt64`) et deux avertissements du compilateur : rien ne doit changer à l'usage. Données de test uniquement.

| # | Scénario | Résultat attendu | État |
|---|---|---|---|
| D1-01 | `Tools\build.cmd` (Visual Studio fermé ou sans fichier modifié ouvert). | « génération réussie » ; 0 erreur, 0 avertissement dans `obj\_build.log`. | ✅ (journal relu : aucun avertissement du compilateur, même en mode « Recommended ») |
| D1-02 | Connexion avec le bon mot de passe ; puis, sur un compte de test, un mauvais mot de passe puis le bon. | Connexion ; message d'échec avec les essais restants ; compteur remis à zéro à la connexion suivante. | ✅ |
| D1-03 | Élévation d'accès depuis Administration : mot de passe faux une fois, puis juste. | Refus puis élévation accordée ; le hub montre les tuiles réservées. | ✅ |
| D1-04 | Administration › Utilisateurs : liste, ouvrir un compte de test, modifier son nom d'affichage, enregistrer. | Liste complète (codes, états) ; enregistrement accepté, code dans l'annonce. | ✅ |
| D1-05 | Cabinet et praticien : ouvrir, enregistrer sans changement. Paramètres : changer une valeur, enregistrer, la rétablir. | Données comme avant ; enregistrements acceptés ; paramètres rangés par carte avec leurs valeurs. | ✅ |
| D1-06 | Référentiels : Liens patient, Rôles légaux, Rôles intervenant, Situations familiales, Statuts de dossier, Statuts de séance, Types de documents, Types de rendez-vous, Modes de paiement, Organismes, Prix de séance ; « inactifs » affichés ; dans l'un, modifier le libellé d'une valeur de test puis le rétablir. | Listes complètes, codes visibles ; la modification porte sur la bonne ligne. | ✅ |
| D1-07 | Domaines : ouvrir un domaine (synthèse, tarifs, natures) ; logo : en choisir un, enregistrer, puis le retirer, enregistrer. | Comme avant ; logo enregistré puis retiré. | ✅ |
| D1-08 | Dossier de test : nouvelle séance (statut, nature, tarif), encaissement (mode de paiement) ; nouveau contact du patient (lien, rôle légal) ; journal de la session. | Listes de choix remplies comme avant, valeurs par défaut comprises ; aucune erreur au journal. | ✅ |
| D1-09 | `.editorconfig` et analyseurs : `Tools\build.cmd --no-incremental`. | « génération réussie » ; 0 erreur, 0 avertissement (journal relu). | ✅ compilé dans Visual Studio ; attribut de langue (CA1824) vérifié dans le code généré |
| D1-10 | Visual Studio : ouvrir `UI/Controls/Administration/UC_Cabinet.vb`, puis la liste d'erreurs, bouton « Messages » activé. | Des messages CA… s'affichent (dont CA1806 pour `LireQualifications`) ; aucun avertissement ; trois petits points gris sous les morceaux de code concernés. | ✅ |

## Tests automatisés (audit D-2, ADR-58)

| # | Scénario | Résultat attendu | État |
|---|---|---|---|
| D2-01 | `Tools\test.cmd`. | Compilation des deux projets sans avertissement ; « Série de tests réussie », 154 tests réussis (journal `obj\_tests.log`). | ✅ (journal relu le 26/10/2026) |
| D2-02 | Visual Studio : recharger la solution ; *Test › Explorateur de tests* › Exécuter tout. | Projet `Althea.Tests` sous le dossier « Tests » de la solution ; 154 tests verts, rangés par classe. | ✅ |
| D2-03 | Lancer Althéa (F5) ; Explorateur de solutions, projet `Althea`. | L'application démarre comme avant ; le dossier `Tests` n'apparaît pas dans le projet `Althea` (il n'est pas compilé dans l'application). | ✅ |
| D2-04 | Étape 2 : `Tools\test.cmd`. | « Série de tests réussie », 229 tests (dont `AltheaDateTests`, `AltheaHeureTests`, `GestionQuotaInamiTests`). | ✅ (journal relu le 26/10/2026) |
| D2-05 | Quota (calcul réorganisé) : dossier de test d'un domaine soumis au quota, avec plusieurs séances. Onglet Séances (colonnes rang et période, bandeau du quota) ; aperçu du patient ; nouvelle séance proche du plafond (alerte). | Rangs, périodes, compteurs et alerte identiques à avant (comparer avec un dossier connu). | ✅ |
| D2-06 | Champs de date et d'heure (fenêtre Séance, fiche patient) : saisir « 300785 », « 15/03 », « 930 », « 9h ». | Lus comme avant (30/07/1985, 15/03 de l'année, 09:30, 09:00). | ✅ |
| D2-07 | Étape 3 : base `althea_tests` et variable `ALTHEA_TESTS_CONNEXION` en place ; `Tools\test.cmd`. | 621 tests réussis, aucun ignoré ; `RequetesFumeeTests` : une ligne par requête (389), toutes acceptées. | ✅ (journal relu le 26/10/2026) |
| D2-08 | Même lancement dans une fenêtre où la variable n'est pas visible. | Le test de fumée apparaît une seule fois, « ignoré » ; les autres tests passent. | ✅ (constaté le 26/10/2026 : 232 réussis, 1 ignoré) |
| D3-01 | Après le push : GitHub, dépôt `AngeljoNG/Althea`, onglet **Actions** › « Compilation et tests » › **Run workflow** (branche `master`). | Les deux étapes passent au vert : « Nouveau commit depuis 24 h ? » puis « Compilation et tests (Windows) » ; dans « Lancer les tests », 620 réussis et 1 ignoré (le test de fumée). Noter la durée. | ✅ 26/10/2026 : exécution n° 1 au vert, 2 min 07 s (contrôle 6 s, Windows 1 min 53 s) |
| D3-02 | Le lendemain d'un jour avec push : onglet Actions. | Une exécution « schedule » de la veille au soir, au vert ; un soir sans commit : l'étape Windows est sautée. | ⬜ |

## Contrôles de clôture

- Vérifier les grilles Patient, Dossier, Séances et Paiements après chaque scénario.
- Comparer les montants affichés aux tarifs figés dans les séances et aux seuls paiements encaissés.
- Vérifier que tous les choix métier proviennent des référentiels actifs et datés.
- Nettoyer les données `TEST_PARCOURS` selon les contraintes relationnelles, sans altérer les référentiels partagés.

## Validation finale

- [ ] Tous les scénarios X-01 à X-12 ont été exécutés sur une base de test.
- [x] C3-01 à C3-06 (codes système, audit C.3 étape 1) — validés par Joëlle le 24/10/2026.
- [x] C3-07 à C3-16 (drapeaux des référentiels, audit C.3 étape 2, v2.57.0) — validés par Joëlle le 24/10/2026.
- [x] C3-17 à C3-19 (lignes système déverrouillées, v2.58.0) — validés par Joëlle le 25/10/2026.
- [x] C5-01 à C5-04 (fiches réparties en fichiers partiels, audit C.5) — validés par Joëlle le 25/10/2026.
- [x] D1-01 à D1-08 (`Option Strict On` pour tout le projet, audit D-1) — validés par Joëlle le 25/10/2026.
- [x] D1-09, D1-10 (`.editorconfig` et analyseurs .NET, audit D-1) — validés par Joëlle le 25/10/2026.
- [x] D2-01 à D2-03 (tests automatisés, audit D-2, étape 1) — validés par Joëlle le 26/10/2026.
- [x] D2-04 à D2-06 (dates, heures, quota, audit D-2, étape 2) — validés par Joëlle le 26/10/2026.
- [x] D2-07, D2-08 (test de fumée des requêtes, audit D-2, étape 3) — 26/10/2026.
- [ ] D3-01, D3-02 (intégration continue, audit D-3) — D3-01 validé le 26/10/2026 ; D3-02 au premier passage du soir.
- [ ] Aucun écart entre la traçabilité affichée, les soldes calculés et les données persistées.
- [ ] Les défauts relevés sont reliés au plan spécialisé concerné (Dossiers, Séances, Tarification/quotas ou Paiements).
