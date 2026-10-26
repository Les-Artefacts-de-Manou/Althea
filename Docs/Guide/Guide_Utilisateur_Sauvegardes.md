# Guide utilisateur — Sauvegardes et restauration

> Dernière mise à jour : 13/10/2026 (lot 30, étapes P2 et P3).
> Public : administrateur du cabinet (Admin) et praticien (SuperUser). Référence technique : ADR-37 et ADR-38, processus 21 et 22.

## 1. Ce que contient une sauvegarde

Une sauvegarde est **un seul fichier** `Althea_AAAAMMJJ_HHMMSS.zip` (date et heure de la sauvegarde), rangé dans le dossier des sauvegardes. Il contient tout ce qu'il faut pour remettre Althéa en service :

| Partie de l'archive | Contenu |
|---|---|
| `base/althea.sql` | La base complète : patients, dossiers, séances, paiements, référentiels, paramètres, comptes. |
| `donnees/…` | Le dossier de données : documents des patients, images du cabinet, photos des comptes. |
| `manifeste.json` | La carte d'identité de la sauvegarde : date, poste, compte, version de la base, nombre de fichiers. |

**Ce que l'archive ne contient pas, volontairement** : les fichiers propres au poste rangés dans `%APPDATA%\Althea` — `althea.local.json` (connexion à la base), `althea.sauvegarde.json` (mot de passe des sauvegardes) et les journaux.

Leurs mots de passe sont chiffrés par Windows pour **ce** compte sur **ce** PC : copiés ailleurs, ils seraient illisibles.

Sur un autre poste, on ressaisit la connexion et le mot de passe des sauvegardes (chapitre 6, étape 7).

Et mettre le mot de passe des archives dans l'archive qu'il protège n'aurait aucun sens.

L'archive est **chiffrée (AES-256)** avec le mot de passe des sauvegardes. Sans ce mot de passe, personne ne peut la lire, pas même Althéa. Elle s'ouvre aussi, avec le mot de passe, dans **7-Zip** ou **WinRAR**. L'explorateur de Windows ne sait pas lire ce chiffrement : il ne demande pas de mot de passe et ne peut rien extraire.

## 2. Avant la première sauvegarde (Admin)

1. **Administration › Sauvegardes**, carte « Mot de passe des sauvegardes » : définir le mot de passe (12 caractères au moins, avec une majuscule, une minuscule et un chiffre ; une phrase de plusieurs mots est idéale).
2. **Le noter sur papier** et le ranger en lieu sûr. Il sera demandé pour restaurer sur un autre poste ou après une réinstallation. Si on le perd, les archives sont perdues.
3. **Choisir le dossier des sauvegardes**, idéalement sur un **disque externe**. Il ne peut pas se trouver dans le dossier de données : les archives partiraient avec le disque en cas de panne.

## 3. Faire une sauvegarde (SuperUser ou Admin)

**Administration › Sauvegardes › « Sauvegarder maintenant »**. La jauge suit l'export de la base puis la copie des documents. On ne peut pas quitter l'écran pendant l'opération.

- **Fermer les documents ouverts** (Word, PDF) avant de lancer : un document ouvert peut être illisible, et la sauvegarde échoue alors en nommant le fichier.
- Les **N dernières archives** sont gardées (paramètre `SAUVEGARDE_NB_CONSERVEES`, 10 par défaut) ; les plus anciennes sont supprimées automatiquement.
- Si la dernière sauvegarde date de plus de `SAUVEGARDE_RAPPEL_JOURS` jours (7 par défaut), un rappel s'affiche à l'ouverture d'Althéa.

## 4. Restaurer une sauvegarde depuis Althéa (Admin)

À utiliser pour revenir à un état antérieur sur **ce poste**, Althéa fonctionnant normalement.

> ⚠️ La restauration **remplace toutes les données actuelles** par celles de l'archive. Tout ce qui a été saisi depuis la date de l'archive est perdu, sauf dans la sauvegarde de sécurité faite automatiquement juste avant.

1. Fermer les documents ouverts et toute fenêtre de l'explorateur ouverte dans le dossier de données.
2. **Administration › Sauvegardes › « Restaurer… »**.
3. **Archive** : choisir dans la liste (archives du dossier des sauvegardes), ou « Parcourir… » pour une archive ailleurs (clé USB). Une archive placée dans le dossier de données est refusée : la copier ailleurs d'abord.
4. **Mot de passe de l'archive** : celui enregistré sur le poste est utilisé ; cocher « Saisir un autre mot de passe » pour une archive faite avec un ancien mot de passe ou sur un autre poste.
5. **Compte MariaDB d'administration** (en général `root`) et son mot de passe.
   - Recréer la base exige ce compte : le compte de l'application n'a volontairement que des droits sur les données.
   - Ce compte sert à cette seule opération et n'est jamais enregistré.
   - *Si root est configuré pour accepter aussi l'identification Windows (`gssapi`, option de l'installateur MariaDB), la session Windows suffit : le mot de passe saisi est alors ignoré, même faux — c'est un comportement de MariaDB, pas d'Althéa.*
6. **« Vérifier l'archive »** : affiche la date, le poste, le compte et la version de la sauvegarde, et contrôle tout sans rien modifier (mot de passe, version de la base, outil MariaDB, compte d'administration et ses droits, espace disque).
7. **« Restaurer »**, puis confirmer. Déroulé automatique :
   - sauvegarde de sécurité de l'état actuel ;
   - extraction des documents de l'archive ;
   - mise de côté du dossier de données actuel, **renommé** `…_avant_restauration_AAAAMMJJ_HHMMSS` (conservé) ;
   - remplacement de la base ;
   - mise en place des documents restaurés.
8. **Althéa redémarre** et revient à l'écran de connexion. On se connecte avec les comptes **tels qu'ils étaient à la date de l'archive**.

**Après une restauration réussie :**
- vérifier les données (quelques patients, les derniers dossiers et documents) ;
- quand tout est correct, **supprimer soi-même** l'ancien dossier `…_avant_restauration_…` ; il occupe de la place et contient des données de patients ;
- la sauvegarde de sécurité reste dans le dossier des sauvegardes, comme les autres archives.

**Ce que la restauration ne change pas :** les chemins propres au poste (racine du dossier de données, dossier des sauvegardes, dossier des outils MariaDB) et le mot de passe des sauvegardes enregistré sur le poste.

**Archives refusées :** seule une archive de **la même version de la base** se restaure depuis Althéa. Une archive plus ancienne (faite avant une mise à jour d'Althéa) se restaure à la main (chapitre 6), avec l'aide du développeur.

## 5. Si la restauration échoue

Le message dit exactement ce qui s'est passé :

| Message | Situation | Que faire |
|---|---|---|
| « La restauration n'a pas eu lieu… Les données actuelles n'ont pas été modifiées. » | Arrêt avant de toucher à la base (document ouvert, espace disque, sauvegarde de sécurité impossible…). | Corriger la cause indiquée et recommencer. |
| « La base a été remise dans son état d'avant… Althéa va redémarrer. » | Échec pendant le remplacement de la base ; la base a été rechargée depuis la sauvegarde de sécurité et l'ancien dossier remis en place. | Rien n'est perdu. Noter le message et prévenir le développeur. |
| « ATTENTION : … la remise en état automatique de la base aussi. » | Cas très rare : la base est incomplète. | **Ne pas travailler dans Althéa.** Restaurer à la main la sauvegarde de sécurité indiquée (chapitre 6) ou prévenir le développeur. |

Toutes les tentatives sont inscrites dans « Dernières restaurations » (fenêtre de restauration) et dans les journaux d'Althéa.

## 6. Restauration à la main (poste neuf, archive plus ancienne, base incomplète)

À faire **avec le développeur** ou en suivant pas à pas. Il faut : 7-Zip ou WinRAR, HeidiSQL, le compte `root` de MariaDB et le mot de passe de l'archive.

1. **MariaDB et la base** (poste neuf seulement) :
   - installer MariaDB
   - créer la base `althea` en `utf8mb4` / `utf8mb4_uca1400_ai_ci`
   - créer le compte de l'application avec un mot de passe fort et ses seuls droits sur les données (`SELECT, INSERT, UPDATE, DELETE, EXECUTE, SHOW VIEW` sur `althea.*`).
   - L'assistant d'installation (lot 33) automatisera cette étape.
2. **Base existante à remplacer** (base incomplète, archive plus ancienne) : dans HeidiSQL, en root, supprimer et recréer la base (`DROP DATABASE althea; CREATE DATABASE althea CHARACTER SET utf8mb4 COLLATE utf8mb4_uca1400_ai_ci;`). Les droits du compte de l'application sont conservés par MariaDB.
3. **Ouvrir l'archive** avec 7-Zip (mot de passe demandé) :
   - extraire `base/althea.sql` dans un dossier temporaire : **ce fichier contient toutes les données en clair** ;
   - extraire le contenu de `donnees/` dans le dossier de données (la racine `PATH_GENERAL`, par exemple `D:\Althea_Donnees`), qui doit être vide au départ.
4. **Recharger la base** : dans HeidiSQL, en root, **sélectionner la base `althea` dans l'arbre** (le dump ne contient pas de `USE`), puis *Fichier › Exécuter un fichier SQL…* sur `althea.sql`.
   - L'avertissement « (1906) The value specified for generated column … has been ignored » est normal : les codes `PAxxxxxx`, `SExxxxxx`… sont recalculés par MariaDB.
5. **Archive plus ancienne** : appliquer ensuite, **dans l'ordre**, les migrations de `Docs/Database/Migration` postérieures à la version de l'archive (indiquée dans `manifeste.json`) ; vérifier avec `SELECT version_schema FROM tec_meta_schema ORDER BY date_application DESC LIMIT 1;`.
6. **Supprimer `althea.sql`** extrait et vider la corbeille.
7. **Lancer Althéa** : sur un poste neuf, renseigner la connexion à la base (compte de l'application). Puis, en Admin :
   - **Paramètres** : vérifier `PATH_GENERAL` (racine du dossier de données) et `SAUVEGARDE_DOSSIER`, qui sont ceux du poste d'origine ;
   - **Sauvegardes** : définir le mot de passe des sauvegardes sur ce poste (le même que celui noté sur papier) ;
   - faire une sauvegarde pour repartir d'un état connu.

## 7. Bonnes pratiques

- Sauvegarder au moins une fois par semaine, et avant toute mise à jour d'Althéa.
- Garder une copie récente **hors du cabinet** (disque externe rangé ailleurs) : les archives sont chiffrées.
- Faire de temps en temps une **vérification** dans la fenêtre de restauration, sans restaurer : elle prouve que l'archive est lisible et que le mot de passe est le bon.
