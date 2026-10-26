# 📋 PLAN DE TESTS - JOURNAUX ET ÉTAT DU SYSTÈME (UC_Journaux, UC_EtatSysteme)

**Module** : Administration — consultation des journaux, page « État du système »  
**Version** : V1.2  
**Date** : 22/10/2026 (section 7 : cohérence des montants en attente) — 18/10/2026 (section 5 : grilles en pastilles et repères, UI-9.3) — 13/10/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

> Références : ADR-39, processus 02 (journaux) et 23 (état du système).

---

## 🎯 Objectif des tests

Valider que l'administrateur lit facilement les journaux (jour, filtres, détail) sans pouvoir les modifier, que les journaux ne contiennent plus de données personnelles, et que la page « État du système » dit vrai — y compris quand quelque chose ne va pas.

---

## ✅ PRÉREQUIS

- [x] Build réussi (aucune migration pour cette étape)
- [x] Un compte Admin, un compte SuperUser
- [x] Quelques jours de journaux dans `%APPDATA%\Althea\Logs`

---

## 📂 SECTION 1 : ACCÈS

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.1 | Hub Administration en **SuperUser** | Tuiles « Journaux » (alors « Logs ») et « État du système » grisées | | ✅ |
| 1.2 | Hub en **Admin** | Les deux tuiles actives ; « État du système » sous « Paramètres », groupe Technique | - Rien ne déborde, tuiles lisibles | ✅ |

## 📂 SECTION 2 : JOURNAUX

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.1 | Ouvrir « Journaux » (alors « Logs ») | Jour le plus récent sélectionné ; liste des jours « jour jj/mm/aaaa (n Ko) » | - Entrées les plus récentes en haut | ✅ |
| 2.2 | Démarrages de session | Lignes « Session » en gras sur fond vert pâle | | ✅ |
| 2.3 | Choisir un autre jour | Le journal de ce jour s'affiche | | ✅ |
| 2.4 | Filtre Niveau = Succinct, puis Catégorie = Security | Seules ces entrées ; compteur « n entrée(s) affichée(s) sur N » ; lignes « Session » masquées | | ✅ |
| 2.5 | Recherche « Restauration » | Entrées dont le message ou l'erreur contient le mot | | ✅ |
| 2.6 | « Erreurs seulement » | Seules les entrées avec une erreur, en rouge | | ✅ |
| 2.7 | Cliquer une entrée avec erreur | Détail : date, niveau / catégorie, ligne, message, « Erreur : … », pile d'appels sur plusieurs lignes (niveau Complet) | | ✅ |
| 2.8 | Faire une action (ex. ouvrir Paramètres), revenir, « Actualiser » | La nouvelle entrée apparaît en haut ; le jour reste sélectionné | | ✅ |
| 2.9 | « Ouvrir le dossier des journaux » | L'explorateur s'ouvre sur `%APPDATA%\Althea\Logs` | | ✅ |
| 2.10 | Lecture seule | Aucun moyen de modifier ou supprimer un journal depuis l'écran | | ✅ |

## 📂 SECTION 3 : JOURNAUX SANS DONNÉE PERSONNELLE

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.1 | Créer puis modifier un patient de test | Journal : « Patient créé avec succès (ID=…) », « Patient mis à jour (ID=…) » — **sans** nom ni prénom | | ✅ |
| 3.2 | Idem pour un contact familial et un thérapeute | Identifiants seulement | | ✅ |
| 3.3 | Créer un patient avec un NISS déjà utilisé (si l'écran laisse passer jusqu'à la base) | Si une erreur est journalisée : « Duplicate entry '***' » — le NISS n'apparaît pas | - Sinon : contrôle fait avant la base, rien à vérifier | ✅ |
| 3.4 | Niveau de journal détaillé (Rapide) : ouvrir la fiche d'un patient de test, puis un de ses dossiers (20/10/2026, audit B3) | Lignes « Menu Home sélectionné : btnPatients » — **sans** le nom du patient (le fil d'Ariane n'est plus journalisé) | | ☐ |
| 3.5 | Ajouter puis modifier un intervenant de suivi d'un patient de test (20/10/2026) | « Suivi intervenant créé avec succès (ID=…, patient ID=…) », « … mis à jour (ID=…) » — **sans** nom de l'intervenant | | ☐ |

## 📂 SECTION 4 : ÉTAT DU SYSTÈME

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.1 | Ouvrir « État du système » | Sections Application, Base de données, Dossiers et espace disque, Sauvegardes ; heure de lecture en bas | - Barre d'état : « tout est en ordre » ou le nombre de problèmes / points à vérifier | ✅ |
| 4.2 | Base | Connexion « ✓ OK » ; version de MariaDB ; **version de la base 2.39.0 — ✓ OK** ; version attendue 2.39.0 | | ✅ |
| 4.3 | Dossiers | Chemins réels, espace libre ; dossier des sauvegardes = celui choisi ; « Sauvegardes sur un autre disque » : « ⚠ Non » si données et sauvegardes sont sur le même disque (cas du poste de dev : C:), « ✓ Oui » sinon | | ✅ |
| 4.4 | Sauvegardes | Dernière sauvegarde réussie (date, ancienneté), mot de passe défini, chemins de `mariadb-dump` et `mariadb` | | ✅ |
| 4.5 | Débrancher le disque des sauvegardes (ou renommer temporairement le dossier), « Actualiser » | Dossier des sauvegardes « introuvable (disque débranché ?) — ⚠ À vérifier » | - Remettre ensuite en place | ✅ |
| 4.6 | « Copier le rapport », coller dans le Bloc-notes | Rapport complet, lisible, **aucun mot de passe** | - Le coller à Claude pour contrôle | ✅ |
| 4.7 | *(Facultatif)* Base de test en retard : sur une copie, supprimer la ligne 2.39.0 de `tec_meta_schema` et pointer Althéa dessus | « 2.38.0 — l'application attend la 2.39.0 : migration(s) à appliquer — ✗ Problème » | - À ne pas faire sur la base de travail | ⏭ facultatif, non fait |

## 📂 SECTION 5 : GRILLES — PASTILLES ET REPÈRES (UI-9.3, 18/10/2026)

> Remplace l'aspect décrit en 2.2, 2.6 et dans la section 4 (sessions en gras, erreurs en rouge, « ✓ / ⚠ / ✗ » en texte coloré).

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.1 | État du système | Colonne « État » en pastille : **OK** (sauge), **À vérifier** (ocre), **Problème** (rose) ; rien pour une simple information | - « À vérifier » n'est plus rouge comme « Problème » | ☐ |
| 5.2 | État du système : titres de section | Chaque section (Application, Base de données…) est un repère titré (trait sauge de part et d'autre du titre) ; cliquer un titre ne le sélectionne pas | - Cliquer un en-tête de colonne ne trie pas (l'ordre des sections est gardé) | ☐ |
| 5.3 | État du système : « Copier le rapport » | Rapport identique à avant : « [✓ OK] », « [⚠ À vérifier] », « [✗ Problème] » | | ☐ |
| 5.4 | Journaux : démarrages de session | Repère titré « Session · hh:mm:ss · Démarrage application … » ; cliquer le repère le sélectionne (fond mauve pâle) et affiche son détail | - Heure des entrées centrée | ☐ |
| 5.5 | Journaux : entrée avec une erreur (« Erreurs seulement ») | Pastille **Erreur** (rose) dans la colonne Niveau, le niveau en petite mention grise à côté ; le reste de la ligne en texte normal | - Détail inchangé (2.7) | ☐ |
| 5.6 | Journaux : cliquer un en-tête de colonne | Pas de tri (les sessions restent à leur place) | | ☐ |

---

## 📂 SECTION 6 : ERREURS IMPRÉVUES ET VERSION DE LA BASE AU DÉMARRAGE (20/10/2026)

> Tester **hors du débogueur** (Ctrl+F5) : sous Visual Studio, le débogueur s'arrête d'abord sur l'erreur.

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.1 | Version Debug, accueil › Outils de test › « Provoquer une erreur » | Message « Erreur inattendue » clair (vérifier la dernière action, journal), **sans** pile .NET ; l'application continue | - Journal : « Erreur non interceptée (interface). » avec le détail technique | ☐ |
| 6.2 | Démarrer avec la base à jour (2.47.0) | Démarrage normal, aucun message | - Écran État du système : version de la base 2.47.0 | ☐ |
| 6.3 | Base de test : `DELETE FROM tec_meta_schema WHERE version_schema = '2.47.0';` puis démarrer | Message « Base de données à mettre à jour » (2.46.0, attendue 2.47.0, marche à suivre), puis Althéa se ferme | - Remettre la ligne : rejouer `migration_v2.47.0_types_identifiant_systeme.sql` (rejouable) | ☐ |
| 6.4 | Base de test : `INSERT INTO tec_meta_schema (version_schema, description, date_application) VALUES ('2.99.0', 'test', NOW());` puis démarrer | Avertissement « Althéa à mettre à jour », puis démarrage | - Retirer la ligne ensuite : `DELETE FROM tec_meta_schema WHERE version_schema = '2.99.0';` | ☐ |

## 📂 SECTION 7 : COHÉRENCE DES MONTANTS EN ATTENTE (22/10/2026)

> Relevé de Joëlle : la séance SE000004 (reportée, donc en lecture seule) gardait une ligne « En attente » de 55 € alors qu'elle ne doit plus rien ; impossible à corriger depuis la séance. Requête de contrôle du 22/10 : SE000004, SE000013, SE000031, SE000083.

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.1 | Compte **Admin** : Administration › État du système | Nouvelle section **Cohérence des données** : « Montants en attente des séances » — « 4 séance(s) dont le montant en attente diffère du reste dû… », pastille **À vérifier** ; bouton **Réaligner les montants en attente** actif | Le rapport copié contient la même ligne | ✅ |
| 7.2 | Cliquer **Réaligner les montants en attente** › **Réaligner** | « 4 séance(s) réalignée(s). » ; l'état se relit : ligne **OK** (« Égaux au reste dû… »), bouton grisé | Journal du jour : « Dus en attente réalignés : 4 séance(s) (ID=…) ; échecs : 0. » — identifiants seulement | ✅ |
| 7.3 | Relancer la requête de contrôle des séances (codes et montants) | **Aucune ligne** | — | ✅ |
| 7.4 | Dossier de SE000004 › onglet Paiements ; dossier de SE000083 › onglet Paiements | SE000004 : plus de ligne « En attente » (le dû clos n'apparaît que dans Détail paiements, « Clos ») ; SE000083 : ligne en attente de **45,00** ; résumé « En attente » diminué d'autant | Bandeau inchangé (ces séances ne sont pas des dus passés) | ✅ |
| 7.5 | Bouton **Annuler** dans la confirmation (avant 7.2, ou sur une base de test) | Rien n'est modifié | — | ➖ |

## 📝 BUGS IDENTIFIÉS PENDANT LES TESTS

| ID | Description | Sévérité | Statut |
|----|-------------|----------|--------|
| | | | |

---

## ✅ VALIDATION FINALE

- [x] Sections 1 à 4 validées (4.7, facultatif, non fait)
- [x] Aucun nom de patient dans les journaux créés après la correction
- [x] Documentation et CHANGELOG à jour
- [x] Section 7 (cohérence des montants en attente, 22/10/2026) — 7.1 à 7.4 validés par Joëlle le 22/10/2026 ; 7.5 (Annuler) non vérifiable après le réalignement réel, à refaire si des écarts réapparaissent

**Date de validation** : 13/10/2026  
**Testeur** : Joëlle
