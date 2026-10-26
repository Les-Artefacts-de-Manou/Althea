# Althéa — Architecture technique

> Vue de maintenance des responsabilités et des frontières applicatives.
> Dernière mise à jour : 25/10/2026 (fenêtre principale par contrat, ADR-55).

## Vue d’ensemble

```text
UI (WinForms)
	↓
Metier (cas d’usage, validations, règles)
	↓
Core (configuration, sécurité, démarrage, accès base)
	↓
MariaDB

Utils (services transverses réutilisables)
```

## Couches

| Couche | Responsabilité | Interdits principaux |
|---|---|---|
| `UI/` | Présentation, interactions, navigation, affichage des erreurs. | SQL direct, règles métier. |
| `Metier/` | Modèles, cas d’usage, validations et règles. | Manipulation directe de contrôles WinForms. |
| `Core/` | Démarrage, configuration, sécurité, accès MariaDB, journalisation. | Règles fonctionnelles propres aux écrans. |
| `Utils/` | Comportements réutilisables de bas niveau/UI. | Règles métier variables. |

## Infrastructure essentielle

- `AppStartupManager` orchestre le démarrage contrôlé, dont le contrôle de la version de la base (base en retard : fermeture).
- `GestionErreursGlobales` (installé par `Program.Main`) : filet des erreurs non interceptées — journalisées, message clair sans détail technique.
- `ConfigManager` gère la configuration locale protégée.
- `DatabaseManager` est l’unique point d’accès aux connexions MariaDB ; `ExecuterEnTransaction` porte les opérations en plusieurs écritures (tout ou rien, ADR-46).
- `GestionLog` centralise la journalisation (sans donnée personnelle, ADR-39) ; `LecteurJournaux` la relit pour l'écran Journaux.
- `VersionSchema.Attendue` déclare la version de base attendue par l'application (comparée à la plus haute migration de `tec_meta_schema`, `GestionEtatSysteme.LireVersionSchemaBase`, au démarrage et par l'écran État du système).
- Les requêtes SQL sont uniquement dans `Core/Database/Queries/`.
- Les règles métier variables (tarifs, organismes, catégories et plafonds de quota, statuts consommateurs) sont lues dans les référentiels datés : un domaine = une convention (ADR-27) ; les lignes système sont protégées par `est_systeme` (ADR-28).
- Les réglages techniques et fonctionnels globaux sont des paramètres `tec_parametres`, lus par leur clé centralisée dans `ClesParametres` avec une valeur de secours journalisée (ADR-36).
- Le dossier de données a une seule racine paramétrable (`PATH_GENERAL`) et des sous-dossiers fixes (`ArborescenceDonnees`, ADR-35).
- La sauvegarde (`GestionSauvegardes`) produit une archive ZIP AES-256 contenant la base (`mariadb-dump`) et le dossier de données ;
  - le mot de passe des archives est chiffré DPAPI sur le poste (`MotDePasseSauvegarde`, ADR-37).
  - La restauration (`GestionRestaurations`, ADR-38) recrée la base avec un compte d'administration MariaDB saisi pour l'opération : le compte de l'application n'a que des droits sur les données.

## Briques transverses du métier

| Brique | Fonctionnement | Emplacement |
|---|---|---|
| Fenêtre principale par contrat (ADR-55, audit C.4) | Les écrans n'atteignent plus `Home` par sa classe : ils appellent `IFenetrePrincipale` (naviguer, revenir, référentiel en fenêtre, fil d'Ariane, bandeau), trouvé dans `UserControlContext.FenetrePrincipale` ; les fenêtres le reçoivent en paramètre. Nothing dans un référentiel en fenêtre : ce qui en dépend est masqué. | `UI/Navigation`, `UI/Context`, `UI/Forms/Home.vb` |
| Droits côté métier (ADR-50, audit C.1) | L'écran construit un `ContexteOperation` (utilisateur authentifié et rôle courant, élévation comprise : `UserControlContext.ContexteOperation`) ; la méthode métier sensible commence par `Autorisation.Exiger` (ou `ExigerUtilisateur` quand l'auteur est enregistré) et refuse par `UnauthorizedAccessException`. Référentiels : `ReglesReferentiels.ExigerDroitEcriture`. Tableau rôle × opération : [Securite_Utilisateurs.md](Securite_Utilisateurs.md). | `Metier/Security`, `Metier/Referentiels` |
| Règles d'état | Un dossier clôturé ou archivé refuse l'écriture par `InvalidOperationException`, d'après le statut relu dans la transaction (`GestionDossiers.ExigerDossierModifiable`). | `Metier/Dossiers` |
| Règles de saisie et d'action (ADR-51, audit C.2) | Un valideur `Gestion*.Valider…` rend la liste des `ErreurSaisie` (propriété en cause par `NameOf`, message) ; l'écran l'affiche (`AffichageErreursSaisie`), la méthode d'écriture l'applique sur les données relues dans sa transaction et refuse par `ErreurValidationException` (hérite d'`InvalidOperationException`). Ce qu'un élément permet (reporter, encaisser, statuts proposés…) est une fonction du métier lue par les boutons. | `Metier/Validation`, modules `Gestion*`, `Utils/UI` |
| Calculs partagés (ADR-48) | Reste dû, avoir et dossier dominant ont une seule définition, dans les vues SQL `v_seances_solde` et `v_dossier_dominant`. | Base, `Core/Database/Queries/` |
| Recherche transversale (ADR-52) | Hub `UC_RechercheHome` et une recherche spécialisée par écran (`UC_Recherche…`) ; métier en lecture seule (`GestionRecherche`, `QueryRecherche`) ; résultat commun `ResultatRecherche` et ouverture commune `OuvertureResultat`, l'écran ouvert étant empilé sur la recherche (`Home.NavigateDepuisRecherche`). Une recherche nouvelle n'ajoute ni table ni migration. | `Metier/Recherche`, `UI/Controls/Recherche` |
| Listes de séances et de paiements partagées | Axes de filtre, filtrage et résumé cliquable communs à l'onglet du dossier et à la recherche : `FiltresSeances`, `FiltresPaiements`, `ResumeFiltre` ; période des recherches : `SelecteurPeriode`. | `UI/Controls/Communs`, `UI/Controls/Recherche` |

## Données locales du poste (`%APPDATA%\Althea`)

| Fichier / dossier | Contenu | Protection |
|---|---|---|
| `althea.local.json` | Configuration de connexion à la base | Mot de passe chiffré DPAPI (`ConfigManager`) |
| `althea.sauvegarde.json` | Mot de passe des archives de sauvegarde | Chiffré DPAPI, compte Windows courant (`MotDePasseSauvegarde`) |
| `althea.syncfusion.json` | Clé de licence Syncfusion (exports PDF / Word) | Chiffrée DPAPI, compte Windows courant (`LicenceSyncfusion`, ADR-41) |
| `Logs\` | Journaux quotidiens | Secrets masqués ; conservation `LOG_RETENTION_JOURS` |

## Dépendances externes

Syncfusion (exports PDF / Word), MySqlConnector (MariaDB), SharpZipLib (archives ZIP AES). Détail et versions : [Environnement_Technique.md](Environnement_Technique.md).

## Documentation complémentaire

| Sujet | Référence |
|---|---|
| Arborescence complète des sources | [Structure_Solution_Althea.md](Structure_Solution_Althea.md) |
| Interface WinForms | [Interface_Utilisateur.md](Interface_Utilisateur.md) |
| Sécurité et utilisateurs | [Securite_Utilisateurs.md](Securite_Utilisateurs.md) |
| Paramètres et référentiels | [Parametres_Referentiels.md](Parametres_Referentiels.md) |
| Base de données | [Database_technique.md](Database_technique.md) |
| Processus (démarrage, logs, paramètres, sauvegarde…) | [Process_Althea.md](Process_Althea.md) |
| Décisions structurantes | [ARCHITECTURE_DECISIONS.md](../Rules/ARCHITECTURE_DECISIONS.md) |





> ---
>
> **Contact** : ***Joëlle De Poorter - Les Artefacts de Manou***
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

