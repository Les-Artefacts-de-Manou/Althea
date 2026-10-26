# 📋 PLAN DE TESTS COMPLET - UC_PARAMETRES

**Module** : Gestion des paramètres applicatifs  
**Version** : V2.1  
**Date** : 12/10/2026 — refonte le 16/10/2026 (écran V2 sur une page, groupes en base v2.40.0) — 18/10/2026 : carte « Licences » (section 9)  
**Auteur** : Joëlle (Manou) / Projet Althéa

> La V1 de ce plan testait l'écran à onglets, grille et panneau de détail, remplacé le 16/10/2026. Les tests des paramètres lus par le code (section 8, ex-section 9) sont conservés.

---

## 🎯 Objectif des tests

Valider l'écran `UC_Parametres` V2 :
- page unique, une carte par groupe (titre et ordre venant de `tec_groupes_parametres`) ;
- consultation lisible, modification de toute la page, enregistrement des valeurs changées en une transaction ;
- validation des valeurs (types, bornes, chemins) ;
- droits Admin / SuperUser ;
- effet des paramètres sur l'application (non-régression du lot 30) ;
- carte « Licences — propre à ce poste » : clé Syncfusion saisie, chiffrée sur le poste, jamais réaffichée (section 9).

---

## ✅ CHECKLIST GÉNÉRALE

### Prérequis

- [ ] **Sauvegarde de la base**, puis migration `migration_v2.40.0_groupes_parametres.sql` appliquée (État du système : base 2.40.0 = version attendue)
- [ ] Application compilée sans erreur
- [ ] Un compte Admin et un compte SuperUser
- [ ] Journal accessible (`%APPDATA%\Althea\Logs`)

---

## 📂 SECTION 1 : MIGRATION v2.40.0

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.1 | Appliquer la migration | Aucune erreur | `SELECT code_groupe, libelle_groupe, ordre_affichage FROM tec_groupes_parametres ORDER BY ordre_affichage;` → 6 groupes : Dossier de données, Séances, Sauvegardes, Cabinet, Sécurité, Technique | ☐ |
| 1.2 | Rejouer la migration | Aucune erreur, aucun doublon | Toujours 6 groupes ; une seule ligne 2.40.0 dans `tec_meta_schema` | ☐ |
| 1.3 | Essayer d'attribuer un groupe inconnu : `UPDATE tec_parametres SET groupe_parametre = 'INCONNU' WHERE cle_parametre = 'LOG_RETENTION_JOURS';` | Refus (clé étrangère) | Valeur inchangée | ☐ |
| 1.4 | Réinitialisation des paramètres (`Maintenance/reinit_tec_parametres.sql`, sur une copie ou après sauvegarde) | Script exécuté sans erreur | 12 paramètres ; groupes intacts | ☐ |
| 1.5 | (Facultatif, sur une copie) ROLLBACK puis réapplication | Les deux passent | Après ROLLBACK : plus de `tec_groupes_parametres`, `groupe_parametre` reprend le défaut `GENERAL` | ☐ |

---

## 📂 SECTION 2 : AFFICHAGE

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.1 | Ouvrir Paramètres en **Admin** | Page unique, sans onglets, en consultation | Cartes dans l'ordre : Dossier de données, Séances, Sauvegardes, Cabinet, Sécurité, Technique ; fil d'Ariane « Administration > Paramètres (mode Admin) » | ☐ |
| 2.2 | Regarder une ligne | Libellé à gauche (gras sauge), valeur à droite, description en italique gris **sous** la valeur | Toutes les valeurs alignées sur une même colonne, dans toutes les cartes | ☐ |
| 2.3 | Nombres | Bornes affichées à droite (« 10 à 240 »…) | Valeur alignée à droite dans le champ | ☐ |
| 2.4 | Pastilles en Admin | « Admin » sur : Dossier des outils MariaDB, Échecs de connexion, Conservation des journaux | Infobulle de la pastille | ☐ |
| 2.5 | Infobulle d'un libellé en Admin | « Clé technique : … » | | ☐ |
| 2.6 | Consultation lisible | Texte normal sur fond teinté, case du kit lisible, aucun bouton Parcourir | Rien de grisé | ☐ |
| 2.7 | Faire défiler la page (ascenseur et molette sur la page) | Défilement net | Aucun morceau de carte recopié au mauvais endroit ; pas de barre horizontale | ☐ |
| 2.8 | Agrandir / réduire la fenêtre Home | Les cartes suivent la largeur | L'aide se replie autrement, rien ne se chevauche | ☐ |
| 2.9 | Ouvrir Paramètres en **SuperUser** | Seuls les paramètres ouverts | Pas de Sécurité ni Technique ; pas de « Dossier des outils MariaDB » ; aucune pastille « Admin » ; pas d'infobulle de clé ; fil d'Ariane « (mode SuperUser) » | ☐ |
| 2.10 | L'écran apparaît d'un coup | Pas de construction visible des cartes | | ☐ |

---

## 📂 SECTION 3 : MODIFIER, ENREGISTRER, ANNULER

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.1 | Cliquer « Modifier » | Tous les champs s'ouvrent (fond blanc) ; boutons dossier visibles à droite des chemins | Modifier indisponible ; Enregistrer indisponible tant que rien n'a changé ; Annuler disponible ; case « désactivés » indisponible | ☐ |
| 3.2 | Changer une valeur | Enregistrer devient disponible | | ☐ |
| 3.3 | Changer **deux** valeurs dans deux cartes différentes, Enregistrer | Un seul enregistrement ; avis « 2 paramètres sont enregistrés… » avec la description de chacun | Page rechargée en consultation ; journal : « paramètres enregistrés (CLE1, CLE2) », sans valeurs | ☐ |
| 3.4 | **Contrôle en base** après 3.3 | Seules les deux lignes ont changé | `SELECT cle_parametre, valeur_parametre, date_modification FROM tec_parametres ORDER BY date_modification DESC;` | ☐ |
| 3.5 | Modifier, changer une valeur, Annuler → Non | On reste en modification, saisie conservée | | ☐ |
| 3.6 | Annuler → Oui | Valeurs d'origine, consultation | Rien écrit en base | ☐ |
| 3.7 | Modifier sans rien changer, puis Annuler | Retour en consultation sans question | | ☐ |
| 3.8 | Valeur changée, puis aller sur une autre vue (menu) | « Quitter sans enregistrer ? » | Non : on reste ; Oui : on quitte, rien d'écrit | ☐ |
| 3.9 | Molette sur un nombre (consultation puis modification) | La valeur ne change pas ; la page défile | | ☐ |
| 3.11 | Modifier puis Enregistrer après avoir changé une seule valeur | Seule cette valeur est écrite | Les autres paramètres gardent leur `date_modification` | ☐ |
| 3.10 | Navigation clavier en modification | Tab passe de champ en champ dans l'ordre de la page | | ☐ |

---

## 📂 SECTION 4 : VALIDATION

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.1 | Durée de séance : taper 5 puis 300 | Le champ s'arrête à 10 et 240 | | ☐ |
| 4.2 | Vider le Répertoire racine des données, Enregistrer | Refus « « Répertoire racine des données » : La racine du dossier de données est obligatoire. — Rien n'a été enregistré. » | Champ signalé, page défilée jusqu'à lui ; rien en base | ☐ |
| 4.3 | Chemin relatif (« Sauvegardes ») dans Dossier des sauvegardes | Refus « Le chemin doit être complet… » | | ☐ |
| 4.4 | Dossier des sauvegardes à l'intérieur du dossier de données | Refus (règle des sauvegardes) | | ☐ |
| 4.5 | Dossier des sauvegardes inexistant | « Le dossier … n'existe pas. Voulez-vous le créer ? » | Oui : dossier créé, enregistrement ; Non : champ signalé, rien d'écrit | ☐ |
| 4.6 | Bouton dossier (Parcourir) | Choix du dossier Windows, titre = libellé du paramètre | Le chemin choisi remplit le champ ; Enregistrer disponible | ☐ |
| 4.7 | Deux valeurs changées dont la seconde refusée | Rien n'est enregistré, pas même la première | Contrôle en base | ☐ |
| 4.8 | Dossier des sauvegardes vers un dossier inexistant **et** racine des données vidée | Refus de la racine, **sans** proposition de créer le dossier | Aucun dossier créé sur le disque | ☐ |
| 4.9 | Taper une valeur dans un nombre puis, sans quitter le champ, aller sur une autre vue | « Quitter sans enregistrer ? » | La frappe compte comme une modification | ☐ |

---

## 📂 SECTION 5 : PARAMÈTRES DÉSACTIVÉS

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.1 | Désactiver un paramètre en base (`actif = 0`), rouvrir l'écran | Il n'apparaît pas | | ☐ |
| 5.2 | Cocher « Afficher les paramètres désactivés » | Il apparaît dans sa carte avec la pastille « Désactivé » | Infobulle : valeur de secours utilisée | ☐ |
| 5.3 | Remettre `actif = 1` | Pastille disparue au prochain chargement | | ☐ |

---

## 📂 SECTION 6 : DROITS ET ROBUSTESSE

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.1 | SuperUser : modifier et enregistrer un paramètre ouvert | Enregistré | | ☐ |
| 6.2 | (Simulation) Pendant que l'écran SuperUser est en modification, passer en base un paramètre affiché à `modifiable_utilisateur = 0`, puis Enregistrer | « Les paramètres n'ont pas été enregistrés… Aucune valeur n'a été modifiée. » | Rien en base ; journal « enregistrement annulé, paramètre … fermé » | ☐ |
| 6.3 | Base injoignable à l'ouverture de l'écran | Message « Les paramètres n'ont pas pu être chargés… » à la place des cartes | Modifier indisponible ; détail dans le journal | ☐ |
| 6.4 | Aucun paramètre visible (simulation) | « Aucun paramètre à afficher. » | | ☐ |

---

## 📂 SECTION 7 : AJOUT D'UN PARAMÈTRE SANS TOUCHER À L'ÉCRAN (facultatif, sur une copie)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.1 | Insérer en SQL un paramètre de test `INT` dans le groupe `CABINET` (bornes 1–5, description) | Il apparaît dans la carte Cabinet, avec ses bornes et son aide | Puis le supprimer en SQL | ☐ |
| 7.2 | Insérer un groupe de test (ordre 35) et un paramètre dedans | Nouvelle carte entre Sauvegardes et Cabinet | Puis supprimer le paramètre, puis le groupe | ☐ |

---

## 📂 SECTION 8 : PARAMÈTRES LUS PAR LE CODE (lot 30, 12/10/2026 — non-régression)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.1 | Durée de séance = 50, puis nouvelle séance (début 9:00) | Fin proposée 9:50 | Idem dans « Reporter la séance » | ☐ |
| 8.2 | Pas = 10 et durée = 50 : nouvelle séance | Listes d'heures de 10 en 10 ; 9:00 → 9:50 présent | | ☐ |
| 8.3 | Pas = 15 et durée = 50 | Listes de 5 en 5 (pas réduit) | Log Process « pas réduit à 5 min… » | ☐ |
| 8.4 | Première heure = 7, dernière = 21 | Listes de 7:00 à 21:00 | Première = 21, dernière = 7 : listes 8:00–20:00 et log | ☐ |
| 8.5 | Échecs de connexion = 3, puis 3 mauvais mots de passe sur un compte de test | Compte verrouillé au 3e échec | Messages « Il reste 2 / Dernière tentative » | ☐ |
| 8.6 | Cachet à l'encre noire coché, puis « Générer » dans Cabinet | Cachet noir | Décoché : bleu nuit | ☐ |
| 8.7 | Conservation des journaux = 2, relancer l'application | Journaux de plus de 2 jours supprimés | Aucune purge si la base est injoignable au démarrage | ☐ |
| 8.8 | Désactiver la durée de séance (ou valeur invalide en base), ouvrir une séance | 45 min proposées, écran normal | Log Database « … valeur retenue : 45 » | ☐ |
| 8.9 | Fiche patient (photo, documents), Cabinet (logo), utilisateur (photo) | Tout s'affiche et s'importe comme avant | Sous-dossiers fixes du dossier de données | ☐ |

---

## 📂 SECTION 9 : LICENCES — PROPRE À CE POSTE (clé Syncfusion, 18/10/2026, ADR-41)

> Fonctionnement validé par Joëlle le 18/10/2026 ; cocher les cas au fil des recettes.
>
> Prévoir la clé Syncfusion (de préférence une **nouvelle**, demandée sur le compte Syncfusion). Ne jamais la coller dans un ticket, un courriel ou ce document.

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 9.1 | Admin, poste sans clé : ouvrir Paramètres | Dernière carte « Licences — propre à ce poste », ligne « Clé Syncfusion », pastille « Absente » (ocre), aide sous la ligne | En consultation, pas de champ visible | ☐ |
| 9.2 | SuperUser : ouvrir Paramètres | Pas de carte « Licences » | | ☐ |
| 9.3 | Admin : Modifier | Champ « Clé Syncfusion » visible, vide, saisie masquée | Infobulle « … ne sera jamais réaffichée » | ☐ |
| 9.4 | Coller un texte trop court (ou avec des caractères comme `é`, `#`), puis Enregistrer | Refus « ce texte ne ressemble pas à une clé de licence… Rien n'a été enregistré » | Champ signalé ; un paramètre modifié en même temps n'est **pas** enregistré | ☐ |
| 9.5 | Coller la vraie clé (même coupée en deux lignes), Enregistrer | « Clé de licence Syncfusion enregistrée sur ce poste… » ; pastille « Définie le jj/mm/aaaa » | `%APPDATA%\Althea\althea.syncfusion.json` existe et ne contient pas la clé en clair | ☐ |
| 9.6 | Modifier un paramètre **et** coller la clé, Enregistrer | Les deux enregistrés : message de portée des paramètres ; barre d'état « … Clé de licence enregistrée sur ce poste. » | | ☐ |
| 9.7 | Coller une clé puis Annuler | Confirmation « Abandonner… » ; rien enregistré, pastille inchangée | | ☐ |
| 9.8 | Export PDF puis Word depuis l'éditeur de texte (notes d'une séance fictive) | Fichiers sans avertissement de licence Syncfusion | | ☐ |
| 9.9 | Relancer l'application | Journal Startup « Licence Syncfusion enregistrée. » | Chercher un morceau de la clé dans les journaux : **aucune** occurrence | ☐ |
| 9.10 | État du système | Section Application : « Licence Syncfusion (exports PDF et Word) — Définie sur ce poste » (OK) | Rapport copié : pas de clé | ☐ |
| 9.11 | Renommer `althea.syncfusion.json`, relancer, puis le remettre | Démarrage normal ; journal « Licence Syncfusion absente… » ; pastille « Absente » ; État du système « à vérifier » | Remettre le fichier : tout redevient « Définie » au redémarrage | ☐ |

---

## 🎯 RÉCAPITULATIF DES TESTS

| Catégorie | Total tests | Tests OK | Tests KO | Non testés |
|-----------|-------------|----------|----------|------------|
| Section 1 : Migration v2.40.0 | 5 | - | - | - |
| Section 2 : Affichage | 10 | - | - | - |
| Section 3 : Modifier, Enregistrer, Annuler | 11 | - | - | - |
| Section 4 : Validation | 9 | - | - | - |
| Section 5 : Paramètres désactivés | 3 | - | - | - |
| Section 6 : Droits et robustesse | 4 | - | - | - |
| Section 7 : Ajout sans toucher à l'écran (facultatif) | 2 | - | - | - |
| Section 8 : Paramètres lus par le code | 9 | - | - | - |
| Section 9 : Licences (clé Syncfusion) | 11 | - | - | - |
| **TOTAL** | **64** | **-** | **-** | **-** |

---

## 📝 BUGS IDENTIFIÉS PENDANT LES TESTS

| # | Bug | Priorité | Statut |
|---|-----|----------|--------|
| - | - | - | - |

---

## ✅ VALIDATION FINALE

- [ ] Sections 1 à 6 et 8 passées (7 facultative)
- [ ] Aucun bug bloquant restant
- [ ] Journaux vérifiés : clés des paramètres, jamais de valeurs ni de chemins
- [ ] Base cohérente après les tests (paramètres de test retirés)
- [ ] Droits Admin / SuperUser validés

---

> **Contact** : ***Joëlle (Manou) - Les Artefacts de Manou***
> 
> Projet Althéa - Tests UC_Parametres
