# 📋 PLAN DE TESTS - SAUVEGARDES ET RESTAURATION (UC_Sauvegardes, DialogRestauration)

**Module** : Sauvegarde complète (lot 30, étape P2) et restauration (étape P3)  
**Version** : V1.4  
**Date** : 21/10/2026 (base avec vues, migration v2.49.0 : 7.6 revu, 7.18) — 18/10/2026 (section 8 : historiques en pastilles et tri, UI-9.3) — 15/10/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

> Références : ADR-37 (sauvegarde, sections 1 à 6, validées le 12/10/2026) et ADR-38 (restauration, section 7).

---

## 🎯 Objectif des tests

Valider qu'une sauvegarde produit une archive chiffrée **complète et relisible** (base + dossier de données), que les prérequis et les droits sont contrôlés, que rien ne reste en clair sur le disque, et que l'écran reste sûr pendant l'opération.

---

## ✅ PRÉREQUIS

- [ ] Sauvegarde de la base faite, migration `migration_v2.38.0_sauvegardes.sql` appliquée
- [ ] Paquet NuGet **SharpZipLib 1.4.2** restauré (référence ajoutée au projet), build réussi
- [ ] MariaDB installé sur le poste (`Program Files\MariaDB …\bin\mariadb-dump.exe`)
- [ ] Un disque ou dossier de destination **hors** du dossier de données (ex. `D:\Althea_Sauvegardes` ou clé USB)
- [ ] 7-Zip ou WinRAR installé (contrôle de l'archive hors Althéa) — l'explorateur Windows ne lit pas le chiffrement AES : il ne demande pas de mot de passe et ne peut pas extraire
- [ ] Un compte Admin, un compte SuperUser (ex. compte de travail du praticien), un compte User

---

## 📂 SECTION 1 : ACCÈS ET DROITS

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.1 | Hub Administration en **User** | Tuile Sauvegardes désactivée | | ✅ |
| 1.2 | Hub en **SuperUser** | Tuile active ; l'écran s'ouvre | - « Sauvegarder maintenant » et « Choisir… » actifs<br>- Confirmation et « Définir » grisés | ✅ |
| 1.3 | Hub en **Admin** (ou SuperUser élevé) | Tout actif | | ✅ |
| 1.4 | Écran : en-tête « Sauvegardes » avec icône, 3 cartes lisibles, rien ne déborde | | | ✅ |

## 📂 SECTION 2 : MOT DE PASSE DES ARCHIVES

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.1 | État initial | « Non défini : aucune sauvegarde possible… » ; « Vérifier » grisé | | ✅ |
| 2.2 | Admin : mot de passe trop court, ou sans majuscule / minuscule / chiffre | Refus avec les règles affichées | - Zones vidées<br>- Règles visibles sous la confirmation | ✅ |
| 2.2b | Maintenir l'œil appuyé, puis relâcher (ou sortir du bouton) | Mot de passe et confirmation en clair, puis masqués | - Idem pour l'œil de la zone « Vérifier » | ✅ |
| 2.3 | Deux saisies différentes | Refus « ne correspondent pas » | | ✅ |
| 2.4 | Mot de passe valide (avec accents, espaces, guillemets) | « enregistré sur ce poste » + consigne de le noter | - État « Défini sur ce poste (le …) »<br>- Fichier `%APPDATA%\Althea\althea.sauvegarde.json` : valeur illisible (chiffrée)<br>- Log Security « défini par … » **sans** le mot de passe | ✅ |
| 2.5 | Zone « Vérifier un mot de passe » : le bon / un mauvais | « C'est bien… » / « Ce n'est pas… » | - SuperUser : zone Vérifier utilisable, zone Définir grisée | ✅ |
| 2.6 | Redéfinir | Confirmation « les archives déjà faites resteront protégées par l'ancien » | - Non : rien ne change | ✅ |
| 2.7 | Ouvrir une session Windows sous un **autre compte** (si possible) | État « Illisible sur ce compte Windows… » | - Sauvegarde refusée avec message clair | ✅ |

## 📂 SECTION 3 : DOSSIER DES SAUVEGARDES

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.1 | « Choisir… » → un sous-dossier du dossier de données | Refus « ne peut pas se trouver dans le dossier de données » | | ✅ |
| 3.2 | « Choisir… » → `D:\Althea_Sauvegardes` | Dossier affiché | - Paramètres › SAUVEGARDE : valeur identique<br>- Log Process « Dossier des sauvegardes défini » | ✅ |
| 3.3 | Paramètres : saisir pour `SAUVEGARDE_DOSSIER` un dossier dans le dossier de données | Refus (même message) | | ✅ |
| 3.4 | Débrancher la clé / renommer le dossier, puis sauvegarder | « inaccessible (disque débranché ?) » | - Aucune ligne de journal | ✅ |

## 📂 SECTION 4 : SAUVEGARDE

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.1 | « Sauvegarder maintenant » (SuperUser) | Jauge et étapes : export de la base, copie des fichiers (n / N), finalisation | - Boutons grisés pendant l'opération<br>- Message final : archive, taille, nombre de fichiers | ✅ |
| 4.2 | Pendant la sauvegarde : cliquer Accueil, puis fermer l'application | Refus avec « Une sauvegarde est en cours… » | - Rien n'est interrompu | ✅ |
| 4.3 | Contenu du dossier | `Althea_AAAAMMJJ_HHMMSS.zip` ; **aucun** `.tmp` restant | | ✅ |
| 4.4 | Ouvrir l'archive avec **7-Zip ou WinRAR** (pas l'explorateur Windows) | Mot de passe demandé ; avec le bon : `base/althea.sql`, `donnees/…` (Documents, Cabinet, Utilisateurs), `manifeste.json` | - 7-Zip indique AES-256<br>- Mauvais mot de passe : refus | ✅ |
| 4.5 | `manifeste.json` | Date, versions application et schéma (2.38.0), base, machine, compte, nombre de fichiers | | ✅ |
| 4.6 | `base/althea.sql` | Dump complet ; **aucun** `LOCK TABLES` ; `CREATE SEQUENCE` présents | | ✅ |
| 4.7 | Rechargement du dump dans une base de test (HeidiSQL, session root) : créer `althea_test` (utf8mb4_uca1400_ai_ci), **la sélectionner dans l'arbre**, *Fichier › Exécuter un fichier SQL…* sur `althea.sql` extrait | Rechargement sans erreur ; nombres de patients / séances identiques ; dernière version 2.38.0 | - ⚠️ Le dump n'a pas de `USE` : il s'exécute dans la base sélectionnée (jamais `althea`)<br>- Ensuite : `DROP DATABASE althea_test`, supprimer le `.sql` extrait (données en clair)<br>- (Préfigure P3) | ✅ |
| 4.8 | Pendant la sauvegarde, regarder `%TEMP%` | Un dossier `Althea_<guid>` apparaît puis **disparaît** à la fin | - Jamais de dump `.sql` en clair dans `%TEMP%` ni ailleurs | ✅ |
| 4.9 | Historique | Ligne « Réussie » : date, archive, taille, fichiers, compte, dossier | - « Dernière sauvegarde réussie : … (aujourd'hui) » | ✅ |
| 4.10 | Ouvrir dans Word un document du dossier de données **en service** (sous PATH_GENERAL, ex. un document de patient), le laisser ouvert et lancer une sauvegarde | Si Word laisse lire le fichier : la sauvegarde réussit et le document figure dans l'archive. Sinon : échec propre, message qui **nomme le fichier** et demande de le fermer, aucune archive `.zip`/`.tmp` laissée, tentative « Échec » dans l'historique. Jamais d'archive « réussie » avec un document vide | | ✅ |
| 4.11 | Après 4.10, fermer Word et relancer la sauvegarde ; ouvrir l'archive | Sauvegarde réussie ; le document est complet ; aucun fichier `~$…` dans `donnees/` | | ✅ |

## 📂 SECTION 5 : ÉCHECS ET PRÉREQUIS

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.1 | Sans mot de passe défini | « Le mot de passe des sauvegardes n'est pas défini… » | - Pas de ligne de journal | ✅ |
| 5.2 | Sans dossier choisi | « pas encore choisi » | | ✅ |
| 5.3 | `mariadb-dump` introuvable (simuler : `SAUVEGARDE_DOSSIER_MARIADB` vers un dossier vide et renommer temporairement le dossier MariaDB n'est pas conseillé — à défaut, contrôler le message dans le code) | « L'outil mariadb-dump est introuvable… » | | ✅ |
| 5.4 | `SAUVEGARDE_DOSSIER_MARIADB` = dossier `bin` correct | Sauvegarde OK avec l'outil indiqué | | ✅ |
| 5.5 | Échec pendant l'opération (ex. retirer la clé USB pendant la copie) | Message d'échec ; ligne « Échec » avec le détail | - Aucun `.zip` incomplet ; `.tmp` supprimé | ✅ |
| 5.6 | Fermer Althéa de force (gestionnaire des tâches) pendant une sauvegarde | Au lancement suivant : ligne « Interrompue » | - Le `.tmp` abandonné est supprimé après 24 h par la rotation suivante | ✅ |

## 📂 SECTION 6 : ROTATION ET RAPPEL

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.1 | `SAUVEGARDE_NB_CONSERVEES` = 2, faire 3 sauvegardes | Seules les 2 plus récentes restent | - Message « Anciennes archives supprimées : 1 »<br>- Un fichier `notes.txt` posé dans le dossier n'est **pas** supprimé | ✅ |
| 6.2 | Aucune sauvegarde réussie, se connecter en SuperUser | Rappel « Aucune sauvegarde n'a encore été faite » à l'ouverture | - Pas de rappel pour un User | ✅ |
| 6.3 | `SAUVEGARDE_RAPPEL_JOURS` = 1 et dernière sauvegarde d'avant-hier (modifier `date_fin` en base de test) | Rappel « La dernière sauvegarde date de 2 jours… » | | ✅ |
| 6.4 | Sauvegarde du jour | Aucun rappel | | ✅ |

---

## 📂 SECTION 7 : RESTAURATION (P3)

### Prérequis de la section 7

- [x] Migration `migration_v2.39.0_restaurations.sql` appliquée (dernière version : 2.39.0), build réussi
- [x] **Filet de sécurité hors Althéa** (le code de restauration n'a encore jamais tourné) : export HeidiSQL de la base `althea` (structure + données) **et** copie du dossier de données dans un autre dossier
- [x] Mot de passe `root` de MariaDB connu ; Word et l'explorateur fermés sur le dossier de données
- [x] **Archive A** : une sauvegarde faite après la migration (version 2.39.0)
- [x] **Après A**, une modification reconnaissable : un patient « TEST RESTAURATION » et un document ajouté à un patient

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.1 | Écran Sauvegardes en **SuperUser**, puis en **Admin** | « Restaurer… » grisé pour le SuperUser, actif pour l'Admin | | ✅ |
| 7.2 | Ouvrir « Restaurer… » | Liste des archives (« date — nom »), la plus récente en premier ; « Restaurer » grisé ; compte `root` proposé | - En-tête, avertissement, historique lisibles, rien ne déborde | ✅ |
| 7.3 | Vérifier une archive d'**hier** (version 2.38.0) | « ✗ Restauration impossible » en gros, en rouge ; motif : « faite avec la version 2.38.0… la base actuelle est en version 2.39.0 » | - « Restaurer » reste grisé | ✅ |
| 7.4 | Archive A, cocher « autre mot de passe » et en saisir un faux | « L'archive n'a pas pu être lue : mot de passe incorrect ou archive endommagée » | | 🔁 à revérifier (B4) |
| 7.5 | Archive A, bon mot de passe, compte d'administration **inexistant** (ex. `rooot`) ; puis `root` avec un **mauvais** mot de passe | Refus « compte d'administration MariaDB … est refusé » | - Rien n'est modifié<br>- ⚠️ Sur un poste où root accepte aussi l'identification Windows (`gssapi`), le mauvais mot de passe est **accepté** par MariaDB (constaté le 13/10/2026 sur le poste de développement) : normal, seule la variante « compte inexistant » est alors probante | 🔁 à refaire (variante compte inexistant) |
| 7.6 | Compte `althea_app` (et son mot de passe) comme compte d'administration | Refus : droits manquants listés (CREATE, DROP, ALTER, INDEX) ; depuis la v2.49.0 (base avec des vues), aussi CREATE VIEW | | ✅ (revérifié le 21/10/2026 avec 7.18) |
| 7.7 | « Parcourir… » vers une copie de A placée **dans** le dossier de données | Refus « L'archive se trouve dans le dossier de données… » | - Supprimer ensuite la copie | ✅ |
| 7.8 | Archive A, bons mots de passe → « Vérifier l'archive » | « ✓ Restauration possible » en gros, en vert ; résumé : date, poste, compte, version 2.39.0, nombre de fichiers ; « Restaurer » actif | | ✅ |
| 7.9 | Après 7.8, modifier le compte (ou choisir une autre archive) | « Restaurer » grisé, « Choix modifié : vérifiez à nouveau » | | ✅ |
| 7.10 | Document ouvert dans Word (dossier de données), vérifier A puis « Restaurer » | « La restauration n'a pas eu lieu : la sauvegarde de sécurité … a échoué : le fichier « … » n'a pas pu être lu… » ; « Les données actuelles n'ont pas été modifiées » | - Ligne « Échec » dans « Dernières restaurations »<br>- Le patient TEST RESTAURATION est toujours là | ✅ |
| 7.11 | Word fermé : vérifier A, « Restaurer », confirmer | Jauge : sauvegarde de sécurité, extraction, mise de côté, base, documents, finalisation ; message de réussite avec la sauvegarde de sécurité et l'ancien dossier ; **redémarrage** sur l'écran de connexion | - Pendant l'opération, la fenêtre refuse de se fermer | ✅ |
| 7.12 | Se reconnecter et contrôler | Patient TEST RESTAURATION **absent** ; document ajouté après A absent ; autres documents et photos présents | - Paramètres : `PATH_GENERAL` et `SAUVEGARDE_DOSSIER` inchangés<br>- `…_avant_restauration_…` à côté du dossier de données, avec le document ajouté après A<br>- Sauvegarde de sécurité dans le dossier des sauvegardes<br>- « Dernières restaurations » : ligne « Réussie »<br>- Historique des sauvegardes : l'archive A est « Réussie » (pas « Interrompue ») et la sauvegarde de sécurité y figure | ✅ |
| 7.13 | Traces | Aucun dossier `Althea_<guid>` dans `%TEMP%`, aucun dossier `…_restauration_…` restant, aucun `althea.sql` en clair | - Journal : « Restauration lancée / réussie » en catégorie Security, **sans** aucun mot de passe | ✅ |
| 7.14 | Restaurer la **sauvegarde de sécurité** de 7.11 | Retour à l'état d'avant 7.11 : patient TEST RESTAURATION et document de retour | - Un second dossier `…_avant_restauration_…` est créé | ✅ |
| 7.15 | *(Avancé, facultatif)* Remise en état automatique : copier A, l'ouvrir avec 7-Zip, ajouter une ligne `SELECT * FROM table_inexistante;` à la fin de `base/althea.sql`, recompresser l'archive en ZIP **AES-256** avec le même mot de passe et la même arborescence ; la restaurer | « La restauration a échoué : le rechargement de la base a échoué (mariadb, code 1 : erreur 1146 (42S02) at line …) » ; « La base a été remise dans son état d'avant… » ; redémarrage | - Données identiques à avant le test<br>- Le message ne contient aucun extrait de données<br>- Ligne « Échec » dans l'historique | ⏭ reporté |
| 7.16 | Nettoyage | Supprimer les dossiers `…_avant_restauration_…` et la copie de 7.15 ; supprimer le patient TEST RESTAURATION | | ✅ |
| 7.17 | *(UI-7, 15/10/2026)* Ouvrir « Restaurer… » | Fenêtre au schéma du kit | Fond beige à la feuille, barre de titre beige ; quatre cartes (Archive à restaurer, Compte d'administration MariaDB, Vérification, Dernières restaurations) ; yeux visibles et actifs pendant l'appui ; verdict à droite de « Vérifier l'archive » ; résumé et avertissement lisibles sans être coupés ; tests 7.2 à 7.9 inchangés | ☐ |
| 7.18 | *(v2.49.0, 21/10/2026, ADR-48)* Base avec ses deux vues : faire une sauvegarde (archive V), créer un patient « TEST VUES », puis restaurer V avec `root` (filet de sécurité HeidiSQL fait avant, comme en section 7) | Restauration réussie, redémarrage ; patient TEST VUES **absent** | - `SHOW FULL TABLES WHERE Table_type = 'VIEW';` → `v_dossier_dominant` et `v_seances_solde`<br>- Grille Patients : colonnes « Suivi » et « Paiements » remplies ; onglets Séances et Paiements d'un dossier de test : montants comme avant<br>- 7.6 refait : CREATE VIEW figure dans les droits manquants de `althea_app` | ✅ |

## 📂 SECTION 8 : HISTORIQUES — PASTILLES ET TRI (UI-9.3, 18/10/2026)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.1 | Écran Sauvegardes, historique | Colonne « Résultat » en pastille : **Réussie** (sauge), **Échec** (rose), **Interrompue** (ocre) ; plus de ligne entière en rouge | - Date centrée ; taille (« 12,3 Mo ») et nombre de fichiers à droite | ☐ |
| 8.2 | Trier par Date, puis par Taille, puis par Fichiers | Dates dans l'ordre chronologique (pas alphabétique) ; tailles et nombres dans l'ordre numérique (2 Mo avant 10 Mo) | - Les lignes sans taille se regroupent en tête ou en fin | ☐ |
| 8.3 | « Restaurer… », Dernières restaurations | Résultat en pastille **Réussie** / **Échec** ; date centrée ; tri par date chronologique | | ☐ |

---

## 📝 BUGS IDENTIFIÉS PENDANT LES TESTS

| ID | Description | Sévérité | Statut |
|----|-------------|----------|--------|
| B1 | Document ouvert dans Word lu vide (0 octet) pendant la sauvegarde : échec avec un message SharpZipLib incompréhensible (« size was 0, but 7227 was expected ») | Moyenne | ✅ Corrigé : octets lus comparés à la taille, message qui nomme le fichier ; verrous Office `~$…` exclus |
| B2 | Icône d'en-tête chargée par code depuis `Boutons_ico_48` (mauvaise couleur, invisible dans le Designer) | Faible | ✅ Corrigé : `Titre_Ico/sauvegarde.png` dans le Designer / .resx |
| B3 | Restauration : après une restauration, l'archive restaurée apparaît « Interrompue » et la sauvegarde de sécurité manque dans l'historique des sauvegardes | Faible | ✅ Corrigé : ligne clôturée et sauvegarde de sécurité réinscrite (`ReporterJournalSauvegardes`) |
| B4 | Restauration, test 7.4 : « Vérifier » avec un mot de passe faux fige l'application sous Visual Studio (exception renvoyée par la tâche de fond, arrêt du débogueur) | Moyenne | Corrigé : `VerifierArchive` rend l'erreur dans son résultat — à revérifier |

---

## ✅ VALIDATION FINALE

- [x] Sections 1 à 6 validées (12/10/2026)
- [x] Section 7 (restauration) validée le 13/10/2026, sauf 7.4 (correction B4 à revérifier) et 7.5 (variante « compte inexistant » à faire) ; 7.15 (remise en état automatique) reporté : non testé à ce jour, vérifié seulement par relecture du code
- [x] Archive relue avec WinRAR et dump rechargé dans une base de test
- [x] Aucun mot de passe dans les journaux ; aucun dump en clair sur le disque
- [x] Documentation et CHANGELOG à jour

**Date de validation** : 12/10/2026  
**Testeur** : Joëlle
