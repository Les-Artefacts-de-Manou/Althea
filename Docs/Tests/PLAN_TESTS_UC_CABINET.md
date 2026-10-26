# 📋 PLAN DE TESTS — CABINET & PRATICIEN (UC_Cabinet)

**Module** : Administration › Cabinet (identité du cabinet, praticien, identifiants, cachet)  
**Version** : V1.6  
**Date** : 24/10/2026 (4.15 à 4.18 : codes postaux belges, v2.56.0) — 23/10/2026 (10.8b : règles du compte lues dans le métier, audit C.2 étape 4) — 21/10/2026 (10.20 à 10.22 : compte du praticien d'un seul tenant, audit B.2)  
**Auteur** : Joëlle (Manou) / Projet Althéa

---

## 🎯 Objectif des tests

Valider l'écran `UC_Cabinet` (lot 27 — fondation cabinet) en couvrant :
- Première saisie (aucun cabinet en base) puis ouverture en **consultation** / passage en **édition** via Modifier
- Enregistrement du cabinet, du praticien, de ses identifiants professionnels et de ses qualifications
- Validation des champs (code postal, téléphone, e-mail, BCE, TVA, IBAN/BIC)
- Porteur du numéro BCE piloté par la situation juridique (`ref_situations_juridiques`)
- Import des images (logo, photo, signature, cachet) et génération du cachet
- Gestion des modifications non enregistrées (`IUnsavedChangesAware`)
- Contrainte V1 : un cabinet = un praticien

> Les règles de validation détaillées par pays sont décrites dans `PLAN_TESTS_PATIENTS.md` (PAT-35 à PAT-47) et le référentiel des situations juridiques dans `PLAN_TESTS_REFERENTIELS.md` (14.15).

---

## ✅ CHECKLIST GÉNÉRALE

### Prérequis

- [ ] Application compilée sans erreur
- [ ] Migrations v2.29.0, v2.30.0 et v2.31.0 appliquées (`cab_*`, `ref_types_identifiant_pro`, `ref_situations_juridiques`)
- [ ] Compte SuperUser connecté (écran réservé à l'administration)
- [ ] Dossier de stockage des images accessible en écriture (`CheminsCabinetHelper`)
- [ ] Trois fichiers image de test (PNG/JPG), dont un de grande taille
- [ ] Pour la section 1 : table `cab_cabinet` vide (ou base neuve)

---

## 📂 SECTION 1 — Première saisie (aucun cabinet)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 1.1 | Ouvrir Administration › Cabinet sans cabinet en base. | Fiche directement en **édition** ; Enregistrer (grisé) et Annuler (actif) visibles, Modifier masqué. |
| 1.2 | Vérifier l'onglet Praticien avant enregistrement. | Onglet désactivé. | `tpPraticien.Enabled = False` tant que le cabinet n'existe pas. | ☐ |
| 1.3 | Vérifier le bouton Importer du logo avant enregistrement. | Désactivé. | Le dossier de stockage dépend de l'id du cabinet. | ☐ |
| 1.4 | Saisir uniquement le nom du cabinet, Enregistrer. | Cabinet créé ; fiche repasse en consultation ; onglet Praticien et logo désormais actifs après Modifier. | `cab_cabinet` : 1 ligne, `id_cabinet > 0`. | ☐ |
| 1.5 | Enregistrer avec nom vide. | Refusé : « Le nom du cabinet est obligatoire. » | Aucune ligne créée. | ☐ |
| 1.6 | Modifier › onglet Praticien : saisir nom + prénom, Enregistrer. | Praticien créé, rattaché au cabinet ; boutons photo/signature/cachet et grilles actifs à la prochaine édition. | `cab_praticiens` : 1 ligne, `id_cabinet` correct. | ☐ |
| 1.7 | Enregistrer un praticien sans prénom. | Refusé : « Le nom et le prénom du praticien sont obligatoires. » | — | ☐ |

---

## 📂 SECTION 2 — Consultation / Modifier / Annuler

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 2.1 | Ouvrir la fiche avec un cabinet existant. | Mode **consultation** : TextBox en lecture seule, listes désactivées, grilles non éditables, imports d'images grisés ; seul **Modifier** visible. | Statut : « Informations du cabinet chargées (consultation). » | ☐ |
| 2.2 | Cliquer Modifier. | Champs saisissables ; Enregistrer grisé tant qu'aucune modification, Annuler toujours actif ; Modifier masqué. |
| 2.3 | Modifier un champ. | Enregistrer s'active. | `_isDirty = True`. | ☐ |
| 2.4 | Annuler avec modifications. | Confirmation « Abandonner les modifications non enregistrées ? » ; Oui → valeurs rechargées, retour en consultation. | Aucune écriture en base. | ☐ |
| 2.5 | Annuler sans modification. | Retour immédiat en consultation, sans confirmation. | — | ☐ |
| 2.6 | Enregistrer après modification valide. | Données persistées, retour en consultation. | Statut : « Informations du cabinet enregistrées. » | ☐ |
| 2.7 | Tenter d'éditer une grille (identifiants / qualifications) en consultation. | Impossible : pas de ligne d'ajout, cellules non éditables. | `ReadOnly = True`, `AllowUserToAddRows = False`. | ☐ |
| 2.8 | En consultation, changer d'onglet puis revenir. | Mode consultation conservé sur tous les onglets. | Aucun champ ne redevient éditable. | ☐ |

---

## 📂 SECTION 3 — Modifications non enregistrées (navigation)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 3.1 | Modifier un champ puis naviguer vers Accueil. | Boîte « Modifications non enregistrées » ; Non → on reste sur la fiche. | `IUnsavedChangesAware`. | ☐ |
| 3.2 | Idem, répondre Oui. | Navigation effectuée, modifications perdues. | Rouvrir : anciennes valeurs. | ☐ |
| 3.3 | Naviguer sans modification. | Aucune boîte de dialogue. | — | ☐ |
| 3.4 | Fermer l'application avec modifications en cours. | Même avertissement qu'en 3.1. | — | ☐ |

---

## 📂 SECTION 4 — Validation des champs (onglet Cabinet)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 4.1 | Pays Belgique, code postal `75001`. | Erreur de validation à l'enregistrement. | `UtilsCodePostal`. | ☐ |
| 4.2 | Pays Belgique, code postal `1000`. | Accepté. | — | ☐ |
| 4.3 | Téléphone `0470 12 34` (incomplet). | Refusé avec message. | `UtilsTelephone`. | ☐ |
| 4.4 | Téléphone `+32 470 12 34 56`. | Accepté, normalisé. | — | ☐ |
| 4.5 | E-mail `cabinet@` | Refusé. | — | ☐ |
| 4.6 | Numéro BCE `0123.456.789` (clé fausse). | Refusé : modulo 97 invalide. | `UtilsIdentifiantBelge`. | ☐ |
| 4.7 | Numéro BCE valide (ex. `0417.497.106`). | Accepté, affiché formaté. | — | ☐ |
| 4.8 | TVA `BE0417497106` avec BCE identique. | Accepté. | Cohérence BCE/TVA. | ☐ |
| 4.9 | TVA `BE0999999999` incohérente avec le BCE. | Refusé avec message explicite. | — | ☐ |
| 4.10 | IBAN `be68539007547034` (minuscules, sans espace), puis quitter le champ. | Devient `BE68 5390 0754 7034` en sortant du champ ; accepté. | - Contrôle modulo 97<br>- En base : `BE68539007547034` (sans espace, 20/10/2026) | ☐ |
| 4.11 | IBAN `BE68 5390 0754 7035`. | Refusé. | — | ☐ |
| 4.12 | BIC `GKCCBEBB` / `GKCC`. | Accepté / refusé (8 ou 11 caractères). | — | ☐ |
| 4.13 | Tous champs facultatifs vides. | Enregistrement accepté (seul le nom est obligatoire). | — | ☐ |
| 4.14 | IBAN et BIC vides en modification ; BIC `gkcc bebb`. | Exemples grisés dans les champs vides (`BE00 0000 0000 0000`, `GEBABEBB`) ; le BIC devient `GKCCBEBB` en sortant du champ. | - Consultation : rien n'est reformaté | ☐ |
| 4.15 | Pays Belgique, code postal `4999` ; Enregistrer. | Refusé : « Ce code postal n'existe pas en Belgique. », onglet Cabinet affiché, focus sur le code. | `GestionCodesPostauxBe` (v2.56.0). | ✅ |
| 4.16 | Code `4000` : quitter le champ avec la localité vide ; puis localité « Namur », Enregistrer depuis l'onglet Praticien → « Corriger », puis « Enregistrer quand même ». | « Liège » remplie ; avec « Namur » : icône d'avertissement sur la localité ; à l'enregistrement, l'onglet Cabinet s'affiche avec la question ; « Corriger » ramène au champ ; « Enregistrer quand même » enregistre. | `SaisieLocaliteBelge`. | ✅ |
| 4.17 | Praticien sans pays propre (« (pays du cabinet) », cabinet en Belgique) : code `4280`, Tab ; localité « Liège », Enregistrer. | « Hannut » proposée ; avec « Liège » : avertissement, puis onglet Praticien et question à l'enregistrement. | Adresse du praticien contrôlée selon le pays du cabinet. | ✅ |
| 4.18 | Cabinet en Belgique, code vide : localité « Avin », Tab ; puis code vide, localité « lie » → choisir « Liège · 4030 », Tab. | Code « 4280 » rempli ; puis Localité « Liège », Code « 4030 ». | Sens localité → code (`SaisieLocaliteBelge`). | ✅ |

---

## 📂 SECTION 5 — Situation juridique et porteur du BCE

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 5.1 | Choisir « Société » (porteur Cabinet). | Champ BCE saisissable sur l'onglet Cabinet ; champ BCE du praticien en lecture seule. | `ref_situations_juridiques.porteur_bce`. | ☐ |
| 5.2 | Choisir « Indépendant personne physique » (porteur Praticien). | Champ BCE du cabinet verrouillé ; BCE saisissable sur l'onglet Praticien. | — | ☐ |
| 5.3 | Passer de Société à Indépendant alors qu'un BCE est saisi côté cabinet. | Confirmation « La situation … prévoit que le praticien porte le numéro BCE … » ; Oui → valeur déplacée vers le praticien ; Non → changement annulé. | `DialogChoix.Confirmer`. | ☐ |
| 5.4 | Situation juridique vide (cabinet antérieur à v2.31.0). | Fiche chargée sans erreur ; aucune situation sélectionnée ; BCE porté par le cabinet (repli). | Colonne nullable. | ☐ |
| 5.5 | Situation inactive mais affectée au cabinet. | Toujours affichée dans la liste (libellé courant conservé). | Filtre `Actif OrElse id courant`. | ☐ |

---

## 📂 SECTION 6 — Images

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 6.1 | Modifier › Importer un logo PNG. | Aperçu affiché aussitôt, **Enregistrer** devient actif ; rien n'est écrit avant Enregistrer (20/10/2026). Après Enregistrer : fichier dans le dossier du cabinet, nom en base, bandeau de la Home à jour. | `cab_cabinet.chemin_logo`, pas de BLOB ; pas de demande « remplacer ? » (rien n'est définitif avant Enregistrer). | ☐ |
| 6.2 | Annuler après import du logo (Abandonner). | Retour en consultation avec l'**ancien** logo ; aucun fichier modifié. | Quitter l'écran après un import : la question « Enregistrer / Quitter sans enregistrer / Rester » est posée. | ☐ |
| 6.3 | Réimporter un logo. | L'ancien fichier est remplacé ; l'aperçu se met à jour. | Un seul fichier logo dans le dossier. | ☐ |
| 6.4 | Importer un fichier non image (ex. .txt renommé .png). | Refusé au choix du fichier (formats acceptés rappelés) ; état inchangé. | Aucun fichier écrit. | ☐ |
| 6.5 | Importer photo et signature du praticien, puis Enregistrer. | Aperçus affichés ; fichiers dans le dossier du praticien après Enregistrer. | `cab_praticiens.chemin_photo` / `chemin_signature`. | ☐ |
| 6.6 | Supprimer manuellement le fichier image puis rouvrir la fiche. | Fiche chargée sans exception ; aperçu vide. | Robustesse au fichier manquant. | ☐ |
| 6.7 | Tenter un import en consultation. | Impossible : boutons grisés. | — | ☐ |
| 6.9 | Image choisie, puis dossier de données rendu inaccessible (ex. paramètre PATH_GENERAL vers un lecteur absent) avant Enregistrer. | La fiche est enregistrée ; avertissement « une image n'a pas pu l'être », avec la marche à suivre. | Journal : « Image … non enregistrée ». | ☐ |
| 6.8 | Logo du cabinet en longueur (ex. 600 × 150), puis logo carré : regarder le bandeau de la Home. | Logo lisible : jusqu'à 180 × 40, centré sur la ligne du nom ; le nom et le praticien se placent après lui | Avant le 20/10/2026 : carré de 30 px, illisible | ☐ |

---

## 📂 SECTION 7 — Cachet

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 7.1 | Praticien enregistré avec nom, titre, identifiant INAMI : Modifier › Générer le cachet. | Aperçu affiché, Enregistrer actif ; écrit avec la fiche. | Statut : « Cachet généré depuis la saisie en cours : il sera enregistré avec la fiche (Enregistrer). » | ☐ |
| 7.2 | Modifier le nom et cocher « Cachet » sur une nouvelle ligne d'identifiant, sans enregistrer, puis Générer. | Le cachet montre la **saisie en cours** (nouveau nom, nouvelle ligne). | Changement du 20/10/2026 (avant : données en base). | ☐ |
| 7.3 | Importer un cachet externe. | Remplace le cachet généré. | `chemin_cachet` mis à jour. | ☐ |
| 7.4 | Générer à nouveau après import. | Le cachet généré remplace l'import. | — | ☐ |
| 7.5 | Générer, puis Annuler (Abandonner). | L'ancien cachet revient ; rien n'est écrit. | — | ☐ |
| 7.6 | Ligne d'identifiant invalide (INAMI à clé fausse), puis Générer. | Message de la ligne + « Corrigez cette ligne, puis générez le cachet. » ; onglet Identifiants affiché. | — | ☐ |

---

## 📂 SECTION 8 — Identifiants et qualifications

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 8.1 | Ajouter un identifiant INAMI valide (11 chiffres, clé correcte). | Ligne enregistrée. | `cab_praticien_identifiants`, type issu de `ref_types_identifiant_pro`. | ☐ |
| 8.2 | Ajouter un identifiant INAMI avec clé fausse. | Refusé à l'enregistrement, message précis. | — | ☐ |
| 8.3 | Ajouter deux identifiants du même type. | Refusé ou remplacé selon règle ; jamais deux lignes actives du même type. | Unicité praticien/type. | ☐ |
| 8.4 | Supprimer une ligne d'identifiant puis Enregistrer. | Ligne supprimée en base. | — | ☐ |
| 8.5 | Ajouter une qualification (diplôme, année, établissement). | Ligne enregistrée. | `cab_praticien_qualifications`. | ☐ |
| 8.6 | Ligne de qualification avec libellé vide. | Ignorée ou refusée avec message ; pas de ligne vide en base. | — | ☐ |
| 8.7 | Type d'identifiant désactivé dans le référentiel (Administration › Types d'identifiant, 20/10/2026). | N'est plus proposé pour un ajout ; lignes existantes toujours affichées (type lisible dans la cellule) et enregistrables. | Soft-delete. | ☐ |
| 8.8 | Valeur refusée par l'expression de contrôle d'un type qui a un masque (20/10/2026). | Message de la ligne complété par « Format attendu : (masque). » ; pas de rappel pour INAMI, BCE, TVA (leur message donne déjà le format). | — | ☐ |

---

## 📂 SECTION 9 — Contrainte V1 et robustesse

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 9.1 | Insérer manuellement un 2ᵉ praticien en base, rouvrir la fiche. | La fiche affiche le premier praticien sans erreur ; aucun écran de sélection (V1). | Schéma multi-praticiens conservé, UI mono-praticien. | ☐ |
| 9.2 | Base de données indisponible à l'ouverture. | Message « Impossible de charger les informations du cabinet. » ; pas de crash. | — | ☐ |
| 9.3 | Erreur SQL à l'enregistrement (ex. contrainte). | Message « Rien n'a été enregistré : votre saisie est toujours à l'écran », avec la marche à suivre ; fiche en édition, saisie conservée. | — | ☐ |
| 9.4 | Ordre de tabulation sur chaque onglet. | Parcours logique de haut en bas, gauche à droite. | Designer `TabIndex`. | ☐ |
| 9.5 | **Tout ou rien** (20/10/2026). Dans HeidiSQL : `START TRANSACTION; SELECT * FROM cab_praticien_qualifications WHERE id_qualification = <une qualification existante> FOR UPDATE;` (laisser la session ouverte). Dans Althéa : Modifier, changer le nom du cabinet **et** cette qualification, Enregistrer. | Après environ 50 s (attente du verrou), message « Rien n'a été enregistré » ; saisie conservée. | - En base : nom du cabinet **inchangé** (la transaction l'a annulé)<br>- Terminer par `ROLLBACK;` dans HeidiSQL, puis Enregistrer de nouveau : tout passe | ☐ |
| 9.7 | Onglet Praticien › compte de travail, en **SuperUser** non élevé, puis élevé en Admin (20/10/2026, audit A.3). | SuperUser : Créer / Lier / Délier inactifs, l'infobulle dit « Réservé à l'administrateur (élevez vos droits avec le cadenas) » ; élevé en Admin (rouvrir la fiche) : actifs selon l'état du compte. | Le métier refuse aussi s'il est appelé sans droit. | ☐ |
| 9.6 | Saisir dans les grilles plus long que la base n'accepte (valeur d'identifiant > 50 caractères, libellé > 200, établissement > 150). | La saisie s'arrête à la longueur permise (20/10/2026). | Avant : erreur SQL à l'enregistrement. | ☐ |

---

## 📂 SECTION 10 — Compte de travail du praticien (lot 28)

> Prérequis : migration v2.32.0 appliquée ; un compte Admin (ex. `joelle`) et un SuperUser actif non lié (ex. `superuser_test`) existent. **Le compte Admin ne doit jamais être modifié.**

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|----|
| 10.1 | Onglet Praticien, aucun compte lié. | Section « Compte de travail (SuperUser) » : « Aucun compte lié » ; Créer / Lier actifs, Délier grisé. | Boutons indépendants du mode Modifier (praticien enregistré suffit). | ☐ |
| 10.2 | Praticien non encore enregistré. | Les trois boutons grisés ; message « Enregistrez d'abord le praticien… ». | — | ☐ |
| 10.3 | Lier un compte existant… | Liste des SuperUser actifs non liés ; l'Admin n'y figure pas. Valider → libellé affiche nom, login, rôle, état ; Délier actif. | `cab_praticiens.id_utilisateur` renseigné ; log Security. | ☐ |
| 10.4 | Lier alors qu'aucun candidat. | Message « Aucun compte SuperUser actif disponible… » ; bouton Lier de la boîte désactivé. | — | ☐ |
| 10.5 | Délier. | Confirmation ; retour à « Aucun compte lié ». | Le compte reste actif et inchangé dans `sec_utilisateurs`. | ☐ |
| 10.6 | Créer le compte… | Login proposé (prénom sans accents), nom d'affichage pré-rempli. Valider → message avec mot de passe temporaire (copié dans le presse-papiers) ; compte lié. | `role_utilisateur = SuperUser`, `role_max_elevation = Admin`, `actif = 1`, `must_change_password = 1`. | ☐ |
| 10.7 | Créer avec un login existant (ex. `joelle`). | Erreur affichée dans la boîte, fenêtre reste ouverte ; aucun compte créé, aucun lien posé. | — | ☐ |
| 10.8 | Créer avec login < 3 caractères ou nom vide. | Erreur dans la boîte, focus sur le champ. | — | ☐ |
| 10.8b | (Audit C.2, 23/10/2026) Créer avec login `ab` **et** nom vide ; puis avec le login d'un compte existant. | Les deux messages dans la boîte, focus sur le login ; ensuite « Ce login est déjà utilisé par un autre compte. » (même règle que la fiche Utilisateur). | Rien n'est créé ni lié. | ✅ |
| 10.9 | Connexion avec le nouveau compte. | Changement de mot de passe forcé ; ensuite élévation Admin par le cadenas possible. | — | ☐ |
| 10.10 | Intégrité du compte Admin. | `SELECT * FROM sec_utilisateurs WHERE login_utilisateur='joelle'` identique avant/après toute la section. | Jamais proposé, jamais lié, jamais modifié. | ☐ |
| 10.11 | Tenter en SQL de lier le même compte à un 2ᵉ praticien. | Erreur de clé unique `uq_cab_praticiens_utilisateur`. | Un compte → un seul praticien. | ☐ |
| 10.12 | Supprimer en SQL le compte lié (base de test). | Le praticien subsiste, `id_utilisateur` repasse à NULL (`ON DELETE SET NULL`). | — | ☐ |
| 10.13 | Rollback v2.32.0 puis ré-application. | Colonne/FK/ligne `2.32.0` retirées puis recréées ; `sec_utilisateurs` intact. | Scripts idempotents. | ☐ |
| 10.14 | *(UI-7, 15/10/2026)* Ouvrir « Créer le compte… » puis « Lier un compte existant… ». | Fenêtre au schéma du kit. | Fond beige à la feuille, barre de titre beige ; carte « Compte de travail » ; création : login et nom d'affichage ; liaison : liste du kit ; une erreur (login trop court, login existant) s'affiche en rose dans la carte. | ☐ |
| 10.15 | *(UI-7, 15/10/2026)* Onglet Praticien, en consultation puis après Modifier. | Trois cartes : Identité, Coordonnées, Adresse professionnelle (vide = celle du cabinet) ; cartes Photo, Signature, Compte en dessous. | Consultation : texte normal sur fond teinté (plus de gris pâle), Civilité et Pays lisibles ; Modifier : zones blanches, listes du kit ; Enregistrer / Annuler inchangés ; icônes Importer, Créer, Lier, Délier bien visibles, atténuées quand le bouton est inactif. | ☐ |
| 10.16 | *(UI-7, 15/10/2026)* Onglet Cabinet, en consultation puis après Modifier. | Cartes Cabinet, Adresse, Coordonnées, Facturation, Logo du cabinet. | Consultation lisible (y compris le n° BCE) ; Modifier : zones blanches, Situation juridique et Pays en listes du kit, BCE désactivé si le praticien porte le BCE (5.x inchangés) ; texte d'aide du nom du cabinet entièrement visible ; icône Importer du logo visible. | ☐ |
| 10.17 | *(UI-7, 16/10/2026)* Onglet Identifiants, en consultation puis après Modifier. | Deux cartes « Identifiants professionnels » et « Diplômes et qualifications », chacune avec sa consigne en italique au-dessus de la grille. | Consultation : grilles verrouillées ; Modifier : ajout, Suppr, case « Cachet » ; sans praticien enregistré, grilles désactivées (8.x inchangés). | ✅ |
| 10.18 | *(UI-7, 16/10/2026)* Onglet Cachet. | Consigne dans la carte, sous la barre d'outils ; icônes tampon (Générer) et import (Importer un scan…) bien visibles. | Générer et Importer en Modifier (7.x inchangés). | ✅ |
| 10.19 | *(16/10/2026)* Enregistrer sans nom de cabinet ; provoquer une erreur d'image. | Messages dans `DialogChoix` ; une erreur ne montre aucun détail technique (« Le détail est dans le journal »). | Détail et exception dans le journal. | ☐ |
| 10.20 | *(21/10/2026, audit B.2 — base de test, ou délier puis relier ensuite le compte habituel)* Admin : praticien sans compte → Créer le compte… → Valider. | Mot de passe temporaire affiché ; « Compte lié » avec le nouveau compte. | Journal : « Utilisateur créé avec succès (ID=…, login=…) » puis « Praticien ID=… lié au compte utilisateur #… » (plus de nom). | ✅ |
| 10.21 | *(21/10/2026)* Créer le compte… avec un login existant. | Erreur dans la fenêtre « Création du compte refusée… » ; elle reste ouverte. | Aucun compte créé, aucun lien (comme 10.7). | ✅ |
| 10.22 | *(21/10/2026, poste de développement)* Créer le compte… : tout saisir, **arrêter le service MariaDB**, Valider ; redémarrer le service. | « L'opération n'a pas pu être effectuée ; rien n'a été enregistré… » | `SELECT id_utilisateur FROM sec_utilisateurs WHERE login_utilisateur = '<login saisi>';` : aucune ligne ; le praticien n'a toujours pas de compte. | ✅ |

---

## 📊 Récapitulatif

| Section | Nb tests | ✅ OK | ❌ KO | ⚠️ Partiel |
|---------|:--------:|:-----:|:-----:|:----------:|
| 1 — Première saisie | 7 | | | |
| 2 — Consultation / Modifier | 8 | | | |
| 3 — Modifications non enregistrées | 4 | | | |
| 4 — Validation des champs | 18 | | | |
| 5 — Situation juridique / BCE | 5 | | | |
| 6 — Images | 9 | | | |
| 7 — Cachet | 6 | | | |
| 8 — Identifiants / qualifications | 8 | | | |
| 9 — V1 et robustesse | 7 | | | |
| 10 — Compte de travail (lot 28) et UI-7 | 22 | | | |
| **TOTAL** | **94** | | | |
