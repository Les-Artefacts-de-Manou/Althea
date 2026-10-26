# Althéa — Gestion des dossiers patients

> Dernière mise à jour : 24/10/2026.

**Althéa** est une application WinForms professionnelle de suivi psychologique et thérapeutique. Elle centralise patients, dossiers, séances, documents et paiements, avec une architecture conçue pour préserver la confidentialité, la traçabilité et l’évolutivité métier.

## Origine du nom

**Althéa** est un prénom féminin d'origine grecque (Ἀλθαία – *Althāía*), dérivé du verbe *altho* signifiant :
« guérir » ou « être en bonne santé »

Il peut être interprété comme : **« celle qui guérit »**

Son nom est parfois interprété comme portant la promesse de réconfort et de protection.

Ce nom incarne parfaitement l'objectif de l'application : accompagner le travail thérapeutique avec rigueur, douceur et efficacité.

### Icône

 <img src="Assets/Appli_Ico/Althea_A _96x96.ico" alt="Althea" style="zoom:100%;" />

 ### Splash

  <img src="Assets/Splash/Splash_Althea_Main_from_image.png" alt="Splash Althea" style="zoom:50%;" />

## Accès rapide

| Besoin | Référence |
|---|---|
| Comprendre le métier | [Concepts métier](Docs/Architecture/Concepts_Metier_Althea.md) |
| Comprendre l’architecture | [Architecture technique](Docs/Architecture/Architecture_Technique.md) |
| Retrouver les sources | [Structure de la solution](Docs/Architecture/Structure_Solution_Althea.md) |
| Connaître les règles obligatoires | [Rules.md](Docs/Rules/Rules.md) |
| Consulter les décisions | [Décisions d’architecture](Docs/Rules/ARCHITECTURE_DECISIONS.md) |
| Suivre l’avancement | [État du projet](Docs/Todo/ETAT_DU_PROJET.md) et [ToDo](Docs/Todo/ToDo.md) |
| Utiliser l’application | Guides : [séances et paiements](Docs/Guide/Guide_Seances_Paiements.md), [recherche](Docs/Guide/Guide_Utilisateur_Recherche.md), [domaines](Docs/Guide/Guide_Utilisateur_Domaines.md), [sauvegardes](Docs/Guide/Guide_Utilisateur_Sauvegardes.md) |

## Objectifs

- gérer les patients, leurs dossiers et leur réseau d’intervenants ;
- planifier et suivre les séances, leur évolution et les documents associés ;
- appliquer une tarification, des quotas et des paiements cohérents ;
- protéger les accès et journaliser les actions importantes ;
- permettre l’évolution des règles métier par paramétrage.

## Parcours métier

```text
Patient → Domaine → Dossier → Séance → Tarif / Quota → Paiement
```

1. Un **patient** peut disposer de plusieurs dossiers et d’un réseau d’intervenants.
2. Un **domaine** représente une **convention** : tarifs, organisme de remboursement éventuel et quotas ; il détermine aussi le conventionnement tarifaire, le no-show facturable et la soumission aux quotas. Un nouvel organisme se traduit par un nouveau domaine, paramétré dans l’écran Domaines.
3. Un **dossier** rassemble le suivi d’un patient pour un domaine, ses zones cliniques, séances, documents et paiements.
4. Une **séance** porte un statut, une nature autorisée par le domaine et un tarif résolu à sa date, puis figé pour préserver l’historique.
5. Un **quota** est appliqué uniquement lorsqu’une règle est paramétrée pour le domaine à la date concernée : catégories libres, plafonds par catégorie, période glissante et statuts de séance consommateurs paramétrables.
   - Une règle qui a déjà servi n’est plus modifiée : on en ajoute une nouvelle, qui clôture la précédente ;
   - une règle encore inutilisée peut être corrigée (ADR-40).
6. Un **paiement** est affecté à une séance ; plusieurs paiements peuvent couvrir une même séance. Le reste dû est calculé, indépendamment du statut de chaque paiement.

Les données variables — domaines, natures, tarifs, liaisons, quotas et statuts — sont pilotées en base par des référentiels, jamais par des constantes métier. Le détail fonctionnel et les règles de cycle de vie sont dans [Concepts métier](Docs/Architecture/Concepts_Metier_Althea.md).

## Architecture en bref

- **VB.NET / .NET 8 Windows / WinForms** ; base **MariaDB** en `utf8mb4`.
- `UI` présente les écrans et délègue les cas d’usage à `Metier`.
- `Metier` applique les règles fonctionnelles et ne manipule pas les contrôles WinForms.
- `Core` centralise démarrage, configuration locale protégée, sécurité, journalisation et accès MariaDB.
- `Utils` regroupe les comportements transverses réutilisables.
- `DatabaseManager` est l’unique point d’accès aux connexions et le SQL est limité à `Core/Database/Queries/`.

Le découpage détaillé et les conventions d’implantation sont dans [Architecture technique](Docs/Architecture/Architecture_Technique.md) et [Structure de la solution](Docs/Architecture/Structure_Solution_Althea.md).

## Environnement technique

Détail dans [Environnement technique](Docs/Architecture/Environnement_Technique.md).

## Fonctionnalités et composants

| Sujet | Synthèse | Référence détaillée |
|---|---|---|
| Démarrage et connexion | L’application reste verrouillée jusqu’à une configuration locale valide et une connexion MariaDB opérationnelle. | [Processus](Docs/Architecture/Process_Althea.md) |
| Sécurité et utilisateurs | Authentification, rôles, session, élévation et comptes sont contrôlés côté métier : chaque opération sensible reçoit le contexte de l’utilisateur (rôle courant, élévation comprise) et passe par une garde unique (ADR-50) ; les secrets ne sont jamais journalisés. | [Sécurité et utilisateurs](Docs/Architecture/Securite_Utilisateurs.md) |
| Paramètres et référentiels | Les paramètres globaux (nés par migration, valeur seule modifiable, bornes et valeur de secours) et les référentiels administrables (dont pays, civilités, mutualités, spécialités des thérapeutes) permettent de faire évoluer le comportement sans recompilation. | [Paramètres et référentiels](Docs/Architecture/Parametres_Referentiels.md) |
| Séances et paiements | Planification, statuts, report, encaissement depuis l’onglet Séances du dossier, avoirs, annulation d’un encaissement ; reste dû et avoir calculés à un seul endroit (vue `v_seances_solde`, ADR-48). Les règles de saisie sont jugées par le métier et toutes les erreurs s’affichent d’un coup sur leurs champs (ADR-51). | [Guide séances et paiements](Docs/Guide/Guide_Seances_Paiements.md) |
| Recherche transversale | Bouton **Recherche** du menu (sous Patients) : recherche rapide par code (PA, DO, SE, PAI, FC, AS) ; séances et paiements d’un jour ou d’une période, tous patients, avec les filtres du dossier et, pour les paiements, les totaux encaissés par mode. Un double-clic mène à l’élément (le dossier au bon onglet, la ligne sélectionnée) ; Retour ramène à la recherche. Rien de ce qui est cherché n’est journalisé. | [Guide de la recherche](Docs/Guide/Guide_Utilisateur_Recherche.md), [Processus](Docs/Architecture/Process_Althea.md) 24, ADR-52 |
| Cabinet et praticien | Identité légale du cabinet (situation juridique, BCE, TVA, IBAN/BIC) et du praticien (identifiants INAMI/COMPY, qualifications, photo, cachet généré). Le numéro BCE est saisi à un seul endroit selon la situation juridique ; les images sont stockées en fichiers. V1 : un cabinet, un praticien. | [Architecture base de données](Docs/Architecture/Database_technique.md) |
| Sauvegarde | Archive ZIP chiffrée AES-256 de la base et du dossier de données, mot de passe propre au cabinet, rotation, historique et rappel ; restauration par l'Admin avec sauvegarde de sécurité et remise en état automatique. | [Guide des sauvegardes](Docs/Guide/Guide_Utilisateur_Sauvegardes.md), processus 21-22, ADR-37 / ADR-38 |
| Journaux et état du système | L'Admin consulte les journaux (lecture seule, sans données personnelles) et un état du système : versions, base, dossiers et espace disque, sauvegardes, avec un rapport copiable sans secret. | [Processus](Docs/Architecture/Process_Althea.md) 02 et 23, ADR-39 |
| Interface utilisateur | `Home` héberge les UserControls métier ; les éditions et confirmations sont modales ; l’UI ne contient pas de SQL ; kit d'interface Althéa (en-têtes, cartes, boutons, onglets dessinés). | [Architecture UI](Docs/Architecture/Interface_Utilisateur.md) |
| Édition riche et dialogues | Les éditeurs RTF, `DialogChoix`, les grilles et le contexte UI sont des composants communs. | [Documentation UI](Docs/Architecture/Documentation_technique_UI_Althea.md) |
| Utilitaires | Thème, boutons, grilles, icônes, validations, téléphones et chemins de fichiers sont centralisés. | [Utilitaires transverses](Docs/Architecture/Utilitaires_Transverses.md) |
| Base de données | Tables métier, référentielles, techniques et sécurité, avec leurs relations. | [Architecture base de données](Docs/Architecture/Database_technique.md) |

## État actuel

Le cœur Patients / Dossiers, les séances, la tarification par domaine, les quotas et les paiements sont implémentés, ainsi que

- la fiche Cabinet / praticien (identité légale, identifiants, cachet),
- la refonte de l'interface (kit Althéa),
- les référentiels de saisie (pays, civilités, mutualités, spécialités des thérapeutes, codes postaux belges),
- les paramètres lus par le code,
- la sauvegarde chiffrée et la restauration,
- l'administration technique (journaux, état du système),
- ainsi que la recherche transversale (recherche par code, séances et paiements d'un jour ou d'une période, patients par lieu, réseau par nom)

(schéma v2.56.0).

L'audit complet du 18/10/2026 ([rapport](Docs/Conception/RAPPORT_AUDIT_2026-10-18.md)) est en cours de traitement : lots A (bugs et brèches), B (intégrité des données), C.1 (droits côté métier) et C.2 (règles de saisie dans le métier) terminés.

Restent :

- la fin de la recherche transversale (patients par lieu, réseau par nom),
- puis les lots C.3 à C.5 et D de l'audit,
- la fin de la refonte UI,
- l'agenda Google et les documents,
- et la préparation de l'installation.

La situation détaillée par module, les risques et priorités sont maintenus dans [État du projet](Docs/Todo/ETAT_DU_PROJET.md). Le plan d’action opérationnel est dans [ToDo](Docs/Todo/ToDo.md).

## Développement et qualité

| Élément | Référence |
|---|---|
| Décisions structurantes | [ARCHITECTURE_DECISIONS.md](Docs/Rules/ARCHITECTURE_DECISIONS.md) |
| Règles de développement | [Rules.md](Docs/Rules/Rules.md) |
| Historique des évolutions | [CHANGELOG.md](CHANGELOG.md) |
| Tests fonctionnels | [Plans de tests](Docs/Tests/) |
| Scripts et migrations | [Documentation base](Docs/Architecture/Database_technique.md) et   [Migration](Docs/Database/Migration) |

## Environnement

- Visual Studio Community 2026 ;
- MariaDB ;
- MySqlConnector et Syncfusion DocIO pour la persistance et les documents ;
- SharpZipLib pour les archives de sauvegarde chiffrées ;
- GitHub privé et miroir public du projet.

Les versions précises et outils de maintenance sont suivis dans l’historique et la documentation technique.
Voir [Environnement technique](Docs/Architecture/Environnement_Technique.md) pour les détails.

## Tests et règles de codage

Tests fonctionnels et plans de recette sont dans [Tests](Docs/Tests/) et les scripts/migrations SQL dans [Migration](Docs/Database/Migration). Les conventions de codage, règles de sécurité et bonnes pratiques sont dans [Rules](Docs/Rules/).

## Documentation technique

L'index des documents se trouve dans [Bibliothèque de Documentation](Docs/Architecture/Bibliotheque_Documentation.md). Il contient les liens vers les concepts métier, l’architecture technique, la structure de la solution, les référentiels et paramètres, l’interface utilisateur, les utilitaires transverses et la base de données.


# Contact

> ***Joëlle De Poorter - Les Artefacts de Manou***
>
> Projet réalisé pour soutenir une pratique de psychologie et de graphologie dans la gestion fiable et évolutive des patients et documents.
>
> - Site web Auteur/Les Artefacts de Manou/Joëlle De Poorter : https://lesartefactsdemanou.be
>
> - Site web Psychologue & Graphologue:  https://pearlnguyenduy.be/
> - mailto: [Privé](mailto:joelle@nguyen.eu) et [Les Artefacts de Manou](mailto:lesartefactsdemanou@gmail.com)
>
> - GitHub privé: Althea    https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea
>
> ---



[TOC]
