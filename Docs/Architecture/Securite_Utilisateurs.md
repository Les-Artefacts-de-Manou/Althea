# Althéa — Sécurité et gestion des utilisateurs

> Référence technique détaillée pour l’authentification, les rôles, les sessions et les comptes utilisateurs.
> Dernière mise à jour : 25/10/2026 — élévation : une seule fenêtre, ouverte par la fenêtre principale (audit C.4, ADR-55)

<details>
<summary>Historique des mises à jour</summary>

- 23/10/2026 — confidentialité de la recherche transversale, ADR-52 ; dépendances `ContexteOperation`, `Autorisation`
- 23/10/2026 — règle du mot de passe et du login dans le métier, audit C.2
- 22/10/2026 — garde unique des droits côté métier : `ContexteOperation`, `Autorisation`, audit C.1 ; tableau rôle × opération ; étape 2 : montant libre et séance encaissée, dossier figé ; étape 3 : référentiels
- 21/10/2026 — vues SQL en `SQL SECURITY INVOKER` : aucun droit nouveau pour le compte de l'application
- 20/10/2026 — élévation : essais comptés et fin après inactivité ; droits sur le rôle courant, garde-fous des administrateurs, compte du praticien
- 18/10/2026

</details>

## Objectif

Protéger les données de santé, contrôler les accès et rendre chaque action sensible traçable, sans coupler la sécurité à la seule visibilité de l’interface.

## Architecture

```text
UI (Login, ChangePassword, ElevationAcces, UC_Utilisateurs)
	↓
Metier/Security (GestionAuthentification, GestionUtilisateurs, ContexteOperation, Autorisation, PolitiqueMotDePasse)
	↓
Core/Security (AppRole, UserSession, PasswordSecurityHelper, CryptoManagerDPAPI)
	↓
Core/Database/Queries/QueryUtilisateurs + sec_utilisateurs
```

## Authentification et secrets

- Les mots de passe utilisateurs sont hachés avec PBKDF2 ; ils ne sont jamais réversibles ni journalisés.
- Les secrets locaux, dont le mot de passe de connexion MariaDB, sont protégés par DPAPI sous le profil Windows courant.
- `DatabaseManager` est le seul point d’accès aux connexions ; aucun écran ne lit ni ne construit une chaîne de connexion.
- Le démarrage reste bloqué tant qu’une configuration locale valide et une connexion MariaDB fonctionnelle ne sont pas disponibles.

## Rôles et session

| Rôle | Responsabilité principale |
|---|---|
| `User` | Usage métier courant dans les limites autorisées. |
| `SuperUser` | Administration fonctionnelle et consultation/maintenance autorisée. |
| `Admin` | Administration complète, dont l’activation/désactivation des comptes. |

`UserSession` représente la session courante et l’éventuelle élévation temporaire. Les droits sont vérifiés dans la couche métier : masquer un bouton ne constitue jamais une protection suffisante.

**Rôle courant et élévation** (décision de Joëlle, 20/10/2026, audit A.3) : partout, les droits se jugent sur le **rôle courant** de la session, élévation comprise (`UserControlContext.RoleCourant`) — un compte élevé en Admin a les droits Admin, y compris la gestion des utilisateurs (avant, celle-ci regardait le rôle de base et refusait).

Les méthodes métier sensibles reçoivent ce rôle et refusent d'elles-mêmes (`UnauthorizedAccessException`) :

- création, modification, déverrouillage, photo, activation, réinitialisation du mot de passe d'un compte (Admin) ;
- création et liaison du compte du praticien (Admin) ;
- annulation d'un encaissement (SuperUser).

**Garde unique côté métier** (audit C.1, décision de Joëlle du 22/10/2026, ADR-50) :

- l'écran transmet un **`ContexteOperation`** — l'utilisateur authentifié et son rôle courant, construit par `UserControlContext.ContexteOperation` — et la méthode métier commence par **`Autorisation.Exiger(contexte, rôle, message)`** (et `Autorisation.ExigerUtilisateur` quand l'auteur est enregistré).
- Le refus est une `UnauthorizedAccessException` au message rédigé pour l'utilisatrice ; l'écran l'affiche tel quel.
- Griser un bouton reste une aide, jamais la protection.

**Tableau rôle × opération** (contrôles côté métier ; complété au fil du lot C) :

| Opération | Rôle exigé | Méthode métier |
|---|---|---|
| Créer, modifier, déverrouiller, activer / désactiver un compte ; photo ; réinitialiser un mot de passe | Admin | `GestionUtilisateurs` (`CreateUtilisateur`, `UpdateUtilisateur`, `DeverrouillerUtilisateur`, `ActiverDesactiverUtilisateur`, `UpdatePhotoUtilisateur`, `ResetPasswordUtilisateurWithCustomPassword`) |
| Créer, lier ou délier le compte de travail du praticien | Admin | `GestionCabinet.CreerCompteSuperUser`, `LierUtilisateur` |
| Lancer une sauvegarde ; choisir le dossier des sauvegardes | SuperUser | `GestionSauvegardes.Sauvegarder`, `DefinirDossierSauvegarde` |
| Définir le mot de passe des sauvegardes | Admin | `GestionSauvegardes.DefinirMotDePasse` |
| Vérifier et restaurer une archive | Admin | `GestionRestaurations.VerifierArchive`, `Restaurer` |
| Réaligner les montants en attente des séances (État du système) | Admin | `GestionPaiements.RealignerDusDesalignes` |
| Enregistrer la clé de licence Syncfusion | Admin | `GestionLicences.DefinirCleSyncfusion` |
| Annuler un encaissement | SuperUser (auteur enregistré) | `GestionPaiements.AnnulerPaiementEncaisse` |
| Utiliser un avoir | tout utilisateur identifié (auteur enregistré) | `GestionPaiements.UtiliserAvoirPourSeance`, `UtiliserAvoirDeSeance` |
| Fixer un montant de séance différent du tarif attendu (cadenas) | SuperUser | `GestionSeances.EnregistrerSeance` (un montant enregistré laissé tel quel, sur le même tarif, reste accepté) |
| Modifier ou déplacer une séance réalisée et encaissée | SuperUser | `GestionSeances.EnregistrerSeance`, `AppliquerDateHeureSeance` (`SeanceEstVerrouillee`) |
| Créer, modifier, désactiver (ou supprimer) une valeur de référentiel : statuts, tarifs, natures, modes de paiement, domaines et quotas, rattachements, pays, civilités, organismes, thérapeutes, textes types… | SuperUser (`ReglesReferentiels.ROLE_ECRITURE`) | les 25 `Metier/Referentiels/Gestion*` (`ReglesReferentiels.ExigerDroitEcriture`) ; l'ajout « à la volée » ([+]) demande l'élévation à un User |

**Règles d'état** (pas des droits, mais vérifiées au même endroit, dans la transaction) :

- un dossier **clôturé** refuse séances et paiements ;
- un dossier **archivé** refuse tout, fiche et zones cliniques comprises (`GestionDossiers.ExigerDossierModifiable`, `ExigerSeanceModifiable`).
- Refus : `InvalidOperationException`.

**Lignes système** des référentiels : la condition est dans la requête SQL elle-même (code inchangé, ligne active ; ADR-28) ; refus explicite si aucune ligne n'est concernée.

Le lot C.1 est terminé (22/10/2026) : toutes les écritures sensibles connues vérifient leurs droits côté métier.

## Gestion des comptes

`UC_Utilisateurs` présente la liste et les actions de gestion. `UtilisateurEdition` fonctionne selon trois modes explicites :

- **Création** : génération d’un mot de passe temporaire et obligation de changement au premier accès ; login d'au moins 3 caractères et unique, nom d'affichage obligatoire, rôle de base au plus égal à l'élévation maximale (`GestionUtilisateurs.ValiderCompte`, aussi pour le compte du praticien) ;
- **Modification** : identifiant de connexion immuable, champs modifiables selon le rôle ;
- **Consultation** : lecture seule avec les seules actions de maintenance autorisées.

Les opérations de création, modification, réinitialisation de mot de passe, déverrouillage et activation/désactivation sont journalisées.

**Règle du mot de passe** (audit C.2, 23/10/2026) : au moins 10 caractères, dont une majuscule, une minuscule et un chiffre — 12 pour le mot de passe des archives de sauvegarde.

- Elle est définie une seule fois (`PolitiqueMotDePasse`) et appliquée par le métier au changement, à la réinitialisation et à la création d'un compte (le mot de passe temporaire généré la respecte) ;
- la fenêtre de changement l'affiche et la vérifie avant l'envoi.
- Un nouveau mot de passe identique à l'ancien est refusé.

**Élévation** (décisions de Joëlle, 20/10/2026, audit A.5) :

- un mot de passe faux compte comme à la connexion (`SECURITE_MAX_ECHECS_LOGIN`) ;
- au seuil, le compte est verrouillé — la session en cours continue au rôle de base, un administrateur déverrouille.
- L'élévation prend fin après `SECURITE_ELEVATION_MINUTES` minutes sans activité (15 par défaut ; clavier, clic, molette, notés par `SuiviActivite`) : message, retour à l'accueil depuis l'Administration ou les Référentiels.
- Une seule porte (25/10/2026, audit C.4, ADR-55) : `ElevationAcces` ne s'ouvre que par la fenêtre principale (`Home.OuvrirFenetreElevation`) — zones réservées (menu Administration ou Référentiels, référentiel ou thérapeute en fenêtre : question d'abord), bouton « Élever accès » des hubs (`IFenetrePrincipale.DemanderElevation`). `ElevationAcces` journalise l'élévation accordée.

**Garde-fous** (décision de Joëlle, 20/10/2026) : on ne modifie pas **son propre** rôle, son élévation ni l'état de son compte (ni se retirer ses droits, ni se les donner) ; aucune modification ne peut laisser la base **sans administrateur actif** (`GestionUtilisateurs.VerifierResteUnAdmin`). Le refus est expliqué à l'écran.

**Compte de travail du praticien** (Administration › Cabinet) :

- créer, lier ou délier ce compte est réservé à l'administrateur (élévation comprise) — le compte créé peut s'élever jusqu'à Admin, un SuperUser ne doit donc pas pouvoir le créer.
- Pour un SuperUser, les boutons sont inactifs et disent pourquoi.
- Le compte et son lien au praticien sont créés ensemble ou pas du tout (21/10/2026) : jamais de compte orphelin au mot de passe temporaire perdu.

**Sauvegardes** (lot 30, 12/10/2026) :

- sauvegarder est ouvert au SuperUser et à l'Admin ;
- définir le mot de passe des archives et restaurer (P3) sont réservés à l'Admin, droits revérifiés dans `GestionSauvegardes` / `GestionRestaurations`.
- Les archives sont chiffrées AES-256 ; leur mot de passe est conservé chiffré par Windows (DPAPI, compte courant) dans `%APPDATA%\Althea\althea.sauvegarde.json`, jamais en base ni dans les journaux.
- Le dump de la base passe directement de `mariadb-dump` à l'archive chiffrée (jamais en clair sur le disque) ;
- le mot de passe de la base est transmis par un fichier d'options temporaire supprimé aussitôt (jamais sur la ligne de commande).

Voir ADR-37.

**Comptes MariaDB** (13/10/2026) :

- le compte de l'application n'a que des droits sur les données de sa base (`SELECT, INSERT, UPDATE, DELETE, EXECUTE, SHOW VIEW`), protégé par un mot de passe fort, chiffré DPAPI dans la configuration locale.
- Les vues (`v_seances_solde`, `v_dossier_dominant`, ADR-48) s'exécutent avec les droits du compte qui les lit (`SQL SECURITY INVOKER`) : ce compte n'a besoin d'aucun droit supplémentaire.
- **Restaurer** (Admin) exige en plus un **compte d'administration MariaDB** (en général `root`) saisi dans la fenêtre de restauration :
  - connexion dédiée sans pool (`DatabaseManager.OpenConnectionAvecCompte`),
  - droits contrôlés avant de commencer (dont `CREATE VIEW` et `SHOW VIEW` depuis que la base contient des vues, v2.49.0 ; `root` les a),
  - mot de passe transmis au client `mariadb` par fichier d'options temporaire, **jamais enregistré ni journalisé**.
- Les messages d'erreur du client, qui recopient la requête fautive, sont réduits au code et à la ligne.

Voir ADR-38.

**Constat du 13/10/2026 (poste de développement)** : les comptes `root` y acceptent « mot de passe **ou** identification Windows » (`gssapi`, option de l'installateur MariaDB) : depuis la session Windows, un mot de passe root faux est accepté.

Ce n'est pas un défaut d'Althéa, mais un réglage du serveur à décider à l'installation chez Pearl, de même que l'existence d'un compte `root@'%'` (connexion root depuis le réseau).

**Journaux et état du système** (13/10/2026) :

- consultation réservée à l'Admin (tuiles Journaux et État du système ; contrôle aussi au chargement des écrans), lecture seule.
- Les journaux ne contiennent ni mot de passe ni donnée personnelle (identifiants seulement, ADR-39) ;
- le rapport d'état ne contient aucun secret (le compte MariaDB de l'application est nommé, jamais son mot de passe).

**Recherche transversale** (ADR-52, 23/10/2026) :

- rien de ce qui est cherché n'est écrit dans le journal — seulement le type de recherche et le nombre de résultats (`GestionRecherche`), jamais un code, un nom ou une période ;
- les critères ne vivent qu'en mémoire, le temps de la session.
- Une liste est plafonnée à 1000 lignes (`GestionRecherche.LIMITE_RESULTATS`) : au-delà, seules les premières séances ou les paiements les plus récents sont listés et l'écran demande de réduire la période.
- Les recherches ne font que lire (requêtes paramétrées) et ne donnent aucun droit propre : l'écran ouvert depuis un résultat applique ses droits habituels, comme depuis l'écran Patients.

**Photo du compte** (12/10/2026) :

- un compte lié à un praticien affiche la photo de la fiche praticien, modifiable seulement dans Administration › Cabinet ;
- un compte non lié peut recevoir une photo propre depuis `UtilisateurEdition` (mode Modification).
- L'enregistrement est réservé à l'Admin, vérifié dans `GestionUtilisateurs.UpdatePhotoUtilisateur`, et journalisé.
- La règle d'affichage est centralisée dans `PhotosUtilisateur` (bandeau de la Home et fiche utilisateur).

**Clés de licence des composants tiers** (18/10/2026, ADR-41) : la clé Syncfusion (exports PDF et Word) n'est plus dans le code.

- L'Admin la saisit dans Administration › Paramètres (carte « Licences — propre à ce poste ») ;
- elle est chiffrée DPAPI pour le compte Windows courant, jamais en base ni dans les journaux, jamais réaffichée.
- Droits revérifiés dans `GestionLicences`.
- Les clés restées dans l'historique Git sont à considérer comme publiques (à renouveler chez Syncfusion).

## Dépendances principales

| Élément | Rôle |
|---|---|
| `Core/Security/AppRole.vb` | Énumération des rôles. |
| `Core/Security/UserSession.vb` | Session et élévation temporaire. |
| `Core/Security/PasswordSecurityHelper.vb` | Hachage et vérification PBKDF2. |
| `Core/Security/CryptoManagerDPAPI.vb` | Chiffrement local des secrets. |
| `Metier/Security/GestionAuthentification.vb` | Connexion, changement de mot de passe, verrouillage. |
| `Metier/Security/ContexteOperation.vb` | Utilisateur authentifié et rôle courant (élévation comprise) transmis aux méthodes métier sensibles ; construit par `UserControlContext.ContexteOperation` (ADR-50). |
| `Metier/Security/Autorisation.vb` | Garde unique des droits : `Exiger`, `ExigerUtilisateur`, `Possede` (ADR-50). |
| `Metier/Security/GestionUtilisateurs.vb` | CRUD et actions de maintenance des comptes ; règles d'un compte (`ValiderCompte`). |
| `Metier/Security/PolitiqueMotDePasse.vb` | Règle du mot de passe (comptes et archives de sauvegarde). |
| `Core/Configuration/MotDePasseSauvegarde.vb` | Mot de passe des archives de sauvegarde, chiffré DPAPI sur le poste (jamais en base). |
| `Core/Configuration/LicenceSyncfusion.vb` | Clé de licence Syncfusion, chiffrée DPAPI sur le poste (jamais en base ni dans le dépôt). |
| `Metier/Parametres/GestionLicences.vb` | Remise de la clé à Syncfusion au démarrage ; saisie d'une nouvelle clé (Admin, forme vérifiée). |
| `Metier/Sauvegardes/GestionRestaurations.vb` | Restauration (Admin) : contrôles, sauvegarde de sécurité, compte d'administration MariaDB non conservé. |
| `Metier/Security/PhotosUtilisateur.vb` | Règle d'affichage de la photo d'un compte (praticien lié, photo propre, initiales). |
| `UI/Forms/Login/` | Écrans Login, changement de mot de passe et élévation. |
| `UI/Controls/Administration/UC_Utilisateurs.vb` | Administration des comptes. |

## Règles de maintenance

- Toute action sensible doit être validée côté métier et journalisée.
- Ne jamais écrire de mot de passe, secret ou chaîne de connexion dans les logs.
- Toute évolution des droits doit être documentée dans [Rules.md](../Rules/Rules.md) et, si elle est structurante, dans [ARCHITECTURE_DECISIONS.md](../Rules/ARCHITECTURE_DECISIONS.md)
- Les modalités détaillées de développement restent normatives dans [Rules.md](../Rules/Rules.md).



# Contact

> ***Joëlle De Poorter - Les Artefacts de Manou***
>
> Projet réalisé pour soutenir une pratique de psychologie et de graphologie dans la gestion fiable et évolutive des patients et documents.
>
> - Site web Auteur/Les Artefacts de Manou/Joëlle De Poorter : https://lesartefactsdemanou.be
>
> - Site web Psychologue & Graphologue:  https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu` et `lesartefactsdemanou@gmail.com`
>
> - GitHub privé: Althea    https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea
>
> ---



[TOC]

