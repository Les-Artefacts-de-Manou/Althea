# Processus Althéa - Documentation technique

> *Dernière mise à jour : 25/10/2026 — document scindé en trois parties : ce fichier est le sommaire*

<details>
<summary>Historique des mises à jour</summary>

- 25/10/2026 — document scindé en trois parties (`Docs/Architecture/Processus/`, décision de Joëlle) ; ce fichier garde le sommaire et l'historique
- 24/10/2026 — séances : créneau libéré et statuts proposés lus dans le référentiel, v2.57.0
- 24/10/2026 — processus 24 : R5, réseau par nom
- 24/10/2026 — processus 24 : R4, patients par lieu
- 23/10/2026 — processus 24 : R3, paiements d'une période
- 23/10/2026 — processus 24 : R2, séances d'une période
- 23/10/2026 — processus 24 : recherche transversale, R1 par code
- 23/10/2026 — domaines : saisie jugée par `GestionDomaines.ValiderDomaine`, complétude par `CalculerManques`
- 23/10/2026 — thérapeutes : spécialité choisie dans un référentiel, v2.55.0
- 23/10/2026 — contacts et thérapeutes : saisie jugée par le métier, homonyme de thérapeute confirmé
- 22/10/2026 — paiements : encaissement depuis l’onglet Séances seulement, ligne choisie avant d’agir
- 22/10/2026 — séances : statuts proposés, chevauchement et encaissement vérifiés par le métier, audit C.2
- 22/10/2026 — séances : montant libre, séance encaissée et dossier figé vérifiés par le métier, audit C.1
- 22/10/2026 (annulation d’un encaissement et utilisation d’un avoir : colonnes et liaison typées, ADR-49)*
- 21/10/2026 — dossier dominant : une seule définition, ADR-48
- 20/10/2026 — processus 01 : contrôle de la version de la base et filet des erreurs non interceptées
- 19/10/2026

</details>

Ce document décrit les processus métier et techniques clés de l'application Althéa.
Chaque processus est détaillé avec son statut, ses fichiers associés, et un diagramme Mermaid pour visualiser le flux. Les processus sont répartis en trois parties :

| Partie | Contenu |
| --- | --- |
| [Socle technique](Processus/Processus_Socle_Technique.md) | Démarrage et connexion, erreurs et journaux, paramètres, interface globale, sécurité et rôles, utilisateurs, dialogue standard, éditeur de texte riche. |
| [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) | Référentiels, patients, contacts, thérapeutes, suivi, domaines (tarifs et quotas), dossier, séance, encaissement. |
| [Administration et recherche](Processus/Processus_Administration_Recherche.md) | Sauvegarde, restauration, état du système, recherche transversale. |

## Sommaire des processus

| N° | Processus | Partie |
| --- | --- | --- |
| 01 | Démarrage & Connexion MariaDB | [Socle technique](Processus/Processus_Socle_Technique.md) |
| 02 | Gestion des erreurs & Logs | [Socle technique](Processus/Processus_Socle_Technique.md) |
| 03 | Gestion des paramètres applicatifs | [Socle technique](Processus/Processus_Socle_Technique.md) |
| 04 | Gestion UI globale (Tooltip, Status, Errors, Contexte) | [Socle technique](Processus/Processus_Socle_Technique.md) |
| 05 | Sécurité applicative & gestion des rôles | [Socle technique](Processus/Processus_Socle_Technique.md) |
| 06 | Gestion des utilisateurs | [Socle technique](Processus/Processus_Socle_Technique.md) |
| 07 | DialogChoix - Système de dialogue personnalisé | [Socle technique](Processus/Processus_Socle_Technique.md) |
| 08 | UC_RichTextEditor - Édition de texte riche avec impression et exports | [Socle technique](Processus/Processus_Socle_Technique.md) |
| 09 | Éditeur de texte riche, profil Compact | [Socle technique](Processus/Processus_Socle_Technique.md) |
| 10 | Gestion des référentiels | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 11 | Gestion des patients (liste + fiche) | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 12 | Gestion des contacts famille | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 13 | Référentiel des thérapeutes | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 15 | Vue générale de gestion du suivi | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 16 | Paramétrage des domaines, tarifs et quotas | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 17 | Gestion du patient et ouverture d’un dossier | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 18 | Cycle de vie du dossier | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 19 | Gestion d’une séance : nature, tarif et quota | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 20 | Encaissement et suivi des paiements | [Métier : patients, dossiers, séances et paiements](Processus/Processus_Metier.md) |
| 21 | Sauvegarde complète chiffrée | [Administration et recherche](Processus/Processus_Administration_Recherche.md) |
| 22 | Restauration d'une sauvegarde | [Administration et recherche](Processus/Processus_Administration_Recherche.md) |
| 23 | État du système | [Administration et recherche](Processus/Processus_Administration_Recherche.md) |
| 24 | Recherche transversale | [Administration et recherche](Processus/Processus_Administration_Recherche.md) |

Un nouveau processus prend le numéro suivant et se range dans la partie de son domaine ; il est ajouté à ce sommaire.

---

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

