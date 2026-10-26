# Althéa — Bibliothèque de documentation

> Index de navigation des documents du projet. Il indique la source de vérité à consulter selon le besoin.
> Dernière mise à jour : 25/10/2026 — doc UI et processus scindés (sommaire + parties), CHANGELOG archivé par année.

## Objectif

Cette bibliothèque facilite la reprise du projet en recensant les documents de référence, leur objectif et leur emplacement. Elle ne duplique pas leur contenu : chaque sujet conserve une source de vérité identifiée.

## Démarrage et pilotage du projet

| Document | But | Lien |
|---|---|---|
| README | Porte d’entrée du dépôt : objectif, parcours métier, architecture synthétique, état et références essentielles. | [README.md](../../README.md) |
| Changelog | Historique chronologique des évolutions fonctionnelles, techniques et documentaires (mois en cours). | [CHANGELOG.md](../../CHANGELOG.md) |
| Changelog — archives 2026 | Entrées de mars à septembre 2026 ; chaque mois écoulé y rejoint les autres (une archive par année). | [CHANGELOG_2026.md](../Archives/CHANGELOG_2026.md) |
| État du projet | Photo de l’avancement, des réalisations, risques et priorités courantes. | [ETAT_DU_PROJET.md](../Todo/ETAT_DU_PROJET.md) |
| ToDo | Backlog opérationnel détaillé : fait, en cours et restant. | [ToDo.md](../Todo/ToDo.md) |
| Fiche de reprise | État du chantier en cours et façon de travailler propre au dépôt, à lire en début de session de travail. | [Althéa — fiche de reprise](<../Todo/Althéa — fiche de reprise (état au 22102026, soir).md>) |
| Rapport d’audit du 18/10/2026 | Plan de travail en cours : constats de l’audit complet du code et de la documentation, plan en lots A à D (§ 6), suivi point par point (§ 9). | [RAPPORT_AUDIT_2026-10-18.md](../Conception/RAPPORT_AUDIT_2026-10-18.md) |
| Checklist V1 | Liste de contrôle de préparation et de livraison de la première version. | [Checklist_projet_V1.md](../Todo/Checklist_projet_V1.md) |
| Planning des actions | Planification des actions prévues sur le projet. | [Planning_actions_Althéa.md](../Todo/Planning_actions_Althéa.md) |
| Plan de refonte UI | Chantier transverse d'habillage : contrôles dessinés, lots UI-0 à UI-8, documentation associée. | [Plan_Refonte_UI_Althea.md](../Todo/Plan_Refonte_UI_Althea.md) |
| Cahier des charges Patients | Besoins initiaux liés au module Patients. | [cahier_des_charges_patients.pdf](../Todo/cahier_des_charges_patients.pdf) |

## Architecture, structure et règles de développement

| Document | But | Lien |
|---|---|---|
| Architecture technique | Vue des couches `UI`, `Metier`, `Core` et `Utils`, de leurs responsabilités et frontières. | [Architecture_Technique.md](Architecture_Technique.md) |
| Structure détaillée de la solution | Arborescence des sources et inventaire des composants applicatifs actuels. | [Structure_Solution_Althea.md](Structure_Solution_Althea.md) |
| Décisions d’architecture | Registre des ADR : décisions structurantes, contexte et conséquences. | [ARCHITECTURE_DECISIONS.md](../Rules/ARCHITECTURE_DECISIONS.md) |
| Règles du projet | Règles normatives de développement, sécurité, référentiels, UI et documentation. | [Rules.md](../Rules/Rules.md) |
| Standards de commentaires | Conventions de commentaires dans le code VB.NET. | [Standards-Commentaires.md](../Rules/Standards-Commentaires.md) |
| Paramètres et référentiels | Architecture de la configuration et des règles métier pilotées en base. | [Parametres_Referentiels.md](Parametres_Referentiels.md) |
| Utilitaires transverses | Responsabilités et règles d’emploi des helpers et utilitaires partagés. | [Utilitaires_Transverses.md](Utilitaires_Transverses.md) |
| Environnement technique | Outils, versions et prérequis de développement. | [Environnement_Technique.md](Environnement_Technique.md) |

## Métier et processus

| Document | But | Lien |
|---|---|---|
| Concepts métier Althéa | Source de vérité fonctionnelle du parcours Patient → Domaine → Dossier → Séance → Tarif / Quota → Paiement. | [Concepts_Metier_Althea.md](Concepts_Metier_Althea.md) |
| Processus Althéa | Workflows détaillés, responsabilités et diagrammes Mermaid des processus métier et techniques. Le fichier est le sommaire ; les processus sont dans trois parties (socle technique, métier, administration et recherche). | [Process_Althea.md](Process_Althea.md) · [Processus/](Processus/) |
| Plan de conception métier | Cadrage de la conception fonctionnelle et des lots métier. | [Plan_Conception_Metier_Althea.md](../Conception/Plan_Conception_Metier_Althea.md) |
| Plan de conception Dossiers | Décisions et conception historique du module Dossiers. | [Plan_Conception_Dossiers_Althea.md](../Conception/Plan_Conception_Dossiers_Althea.md) |
| Plan de conception Paiements | Décisions et conception historique du module Paiements. | [Plan_Conception_Paiements_Althea.md](../Conception/Plan_Conception_Paiements_Althea.md) |
| Implémentation tarif patient | Notes de conception et d’implémentation de la tarification patient. | [IMPLEMENTATION_TARIF_PATIENT.md](../Conception/IMPLEMENTATION_TARIF_PATIENT.md) |
| Corrections tarif patient | Historique des corrections apportées au comportement tarifaire. | [CORRECTIONS_TARIF_PATIENT.md](../Conception/CORRECTIONS_TARIF_PATIENT.md) |
| Clarification ComboBox tarif | Décisions de présentation et de sélection des tarifs dans l’interface. | [CLARIFICATION_COMBOBOX_TARIF.md](../Conception/CLARIFICATION_COMBOBOX_TARIF.md) |

## Interface utilisateur et sécurité

| Document | But | Lien |
|---|---|---|
| Interface utilisateur | Architecture WinForms : navigation, contexte partagé, conventions UX et composants principaux. | [Interface_Utilisateur.md](Interface_Utilisateur.md) |
| Documentation technique UI | Revue détaillée des Forms, UserControls, contrôles, onglets, responsabilités et captures. Le fichier est le sommaire ; les fiches sont dans cinq parties par domaine. | [Documentation_technique_UI_Althea.md](Documentation_technique_UI_Althea.md) · [UI/](UI/) |
| Charte graphique | Principes graphiques et identité visuelle de l’application. | [Charte_Graphique.md](../Rules/Charte_Graphique.md) |
| Référence UI — construire un écran | Comment construire une fenêtre, un UserControl, une liste, une fiche : familles d'écrans illustrées, recettes pas à pas, comportements communs, catalogue des contrôles du kit (propriétés, défauts, usage). Fusion, le 15/10/2026, de l'ancien guide d'utilisation et de l'ancienne référence des propriétés. | [Reference_UI.md](../Rules/Reference_UI.md) |
| Sécurité et utilisateurs | Authentification, rôles, sessions, secrets locaux et administration des comptes. | [Securite_Utilisateurs.md](Securite_Utilisateurs.md) |

## Base de données

| Document | But | Lien |
|---|---|---|
| Architecture technique de la base | Schéma MariaDB actuel : tables, relations, contraintes, historisation et maintenance. | [Database_technique.md](Database_technique.md) |
| Sauvegarde sans données | Référence SQL de la structure finale : tables, index, contraintes et séquences. | [backup_NoData_althea.sql](../Database/backup_NoData_althea.sql) |
| Sauvegarde avec données | Sauvegarde de développement permettant de contrôler les référentiels et usages réels. | [backup_WithData_althea.sql](../Database/backup_WithData_althea.sql) |
| Migrations de schéma | Scripts versionnés de mise à niveau et de rollback. | [Migration/](../Database/Migration/) |
| Diagnostic FK tarif | Script d’investigation des contraintes de tarification. | [DIAGNOSTIC_FK_TARIF.sql](../Database/Migration/DIAGNOSTIC_FK_TARIF.sql) |

## Tests, POC et guides

| Document | But | Lien |
|---|---|---|
| Stratégie de tests | Organisation et principes de la validation fonctionnelle et technique. | [STRATEGIE_TESTS_ALTHEA.md](../Tests/STRATEGIE_TESTS_ALTHEA.md) |
| POC gestion documentaire | Preuve de concept et pistes d’intégration de la gestion documentaire. | [Gestion_documentaire_Althea.md](../Poc/Gestion_documentaire_Althea.md) |
| POC calendrier | Preuve de concept et pistes d’intégration de l’agenda. | [Gestion_Calendrier_Althea.md](../Poc/Gestion_Calendrier_Althea.md) |
| Guide séances et paiements | Utiliser les onglets Séances et Paiements (notions, grilles, pas à pas) et règles de maintenance : règle unique, calculs, conséquences de chaque action, diagnostic. Remplace le relevé des conséquences. | [Guide_Seances_Paiements.md](../Guide/Guide_Seances_Paiements.md) |
| Guide utilisateur Sauvegardes | Sauvegarder, restaurer depuis Althéa, que faire en cas d'échec, restauration manuelle (poste neuf, archive plus ancienne). | [Guide_Utilisateur_Sauvegardes.md](../Guide/Guide_Utilisateur_Sauvegardes.md) |
| Guide utilisateur Recherche | Guide de la recherche transversale : bouton Recherche du menu, recherche rapide par code, séances et paiements d’un jour ou d’une période, aller à l’élément et revenir. | [Guide_Utilisateur_Recherche.md](../Guide/Guide_Utilisateur_Recherche.md) |
| Guide utilisateur Domaines | Paramétrer un domaine-convention : organisme, tarifs, natures, catégories et règles de quota (`UC_DomainesHome`). | [Guide_Utilisateur_Domaines.md](../Guide/Guide_Utilisateur_Domaines.md) |
| Guide RichTextEditor |Documentation du contrôle RichTextEditor| [UC_RichTextEditor_Documentation.md](../Guide/UC_RichTextEditor_Documentation.md) |
| Guide de licence Syncfusion | Obtention et maintenance de la licence Syncfusion Community. | [Guide_Licence_Syncfusion.md](../Conception/Guide_Licence_Syncfusion.md) |

### Plans de tests

| Plan | Périmètre et explication | Lien |
|---|---|---|
| Authentification | Connexion, déconnexion, mots de passe, verrouillage et droits d’accès. | [PLAN_TESTS_LOGIN_AUTHENTIFICATION.md](../Tests/PLAN_TESTS_LOGIN_AUTHENTIFICATION.md) |
| Accueil et navigation | Navigation Home, chargement des écrans, contexte partagé et retours utilisateur. | [PLAN_TESTS_HOME_NAVIGATION.md](../Tests/PLAN_TESTS_HOME_NAVIGATION.md) |
| DialogChoix | Boîtes de dialogue standard : actions, confirmations, validations et cohérence visuelle. | [PLAN_TESTS_DIALOGCHOIX.md](../Tests/PLAN_TESTS_DIALOGCHOIX.md) |
| Contrôles du kit | Champs du kit à comportement propre : champ de date et calendrier (à compléter à chaque nouveau contrôle). | [PLAN_TESTS_KIT_ALTHEA.md](../Tests/PLAN_TESTS_KIT_ALTHEA.md) |
| Paramètres | Paramètres applicatifs, saisie, persistance et comportement des écrans associés. | [PLAN_TESTS_UC_PARAMETRES.md](../Tests/PLAN_TESTS_UC_PARAMETRES.md) |
| Utilisateurs | Gestion des comptes, rôles, activation, réinitialisation et sécurité des utilisateurs. | [PLAN_TESTS_UC_UTILISATEURS.md](../Tests/PLAN_TESTS_UC_UTILISATEURS.md) |
| Cabinet et praticien | Fiche Cabinet : identité légale, praticien, identifiants, cachet, compte du praticien. | [PLAN_TESTS_UC_CABINET.md](../Tests/PLAN_TESTS_UC_CABINET.md) |
| Sauvegardes | Archive chiffrée (base + dossier de données), mot de passe, rotation, rappel, droits ; restauration (section 7). | [PLAN_TESTS_SAUVEGARDES.md](../Tests/PLAN_TESTS_SAUVEGARDES.md) |
| Journaux et état du système | Consultation des journaux, absence de données personnelles, page État du système. | [PLAN_TESTS_ADMINISTRATION.md](../Tests/PLAN_TESTS_ADMINISTRATION.md) |
| Référentiels | Référentiels génériques et métier : domaines, statuts, natures, tarifs, modes de paiement et activation. | [PLAN_TESTS_REFERENTIELS.md](../Tests/PLAN_TESTS_REFERENTIELS.md) |
| Éditeur riche | Saisie RTF, formatage, annulation, persistance et intégration de `UC_RichTextEditor`. | [PLAN_TESTS_UC_RICHTEXTEDITOR.md](../Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md) |
| Patients | Liste et filtres, identité, NISS, activation, alertes, photo, anamnèse, contacts, intervenants et accès aux dossiers. | [PLAN_TESTS_PATIENTS.md](../Tests/PLAN_TESTS_PATIENTS.md) |
| Dossiers | Création et cycle de vie des dossiers, zones cliniques, transitions de statut et lecture seule. | [PLAN_TESTS_DOSSIERS.md](../Tests/PLAN_TESTS_DOSSIERS.md) |
| Séances | Création et modification des séances, natures autorisées, planning, chevauchements et verrouillage. | [PLAN_TESTS_SEANCES.md](../Tests/PLAN_TESTS_SEANCES.md) |
| Tarification et quotas | Résolution des tarifs datés, gel historique, no-shows et contrôle de quotas paramétrés. | [PLAN_TESTS_TARIFICATION_QUOTAS.md](../Tests/PLAN_TESTS_TARIFICATION_QUOTAS.md) |
| Gestion des domaines | Paramétrage d'un domaine-convention : organisme, tarifs, catégories et règles de quota, natures, assistant et synthèse. | [PLAN_TESTS_DOMAINES.md](../Tests/PLAN_TESTS_DOMAINES.md) |
| Paiements | Paiements partiels, prévisions, annulations, reste dû, soldes et avoirs. | [PLAN_TESTS_PAIEMENTS.md](../Tests/PLAN_TESTS_PAIEMENTS.md) |
| Recherche transversale | Bouton Recherche, recherche rapide par code, séances et paiements d’un jour ou d’une période, ouverture ciblée et Retour (RCH-01 à RCH-39). | [PLAN_TESTS_RECHERCHE.md](../Tests/PLAN_TESTS_RECHERCHE.md) |
| Parcours de suivi transversal | Recette de bout en bout : Patient → Domaine → Dossier → Séance → Tarif → Quota → Paiement. | [PLAN_TESTS_PARCOURS_SUIVI.md](../Tests/PLAN_TESTS_PARCOURS_SUIVI.md) |
| Tarif patient par défaut — archive | Ancien plan conservé pour la traçabilité ; remplacé par le plan Tarification et quotas. | [PLAN_TEST_TARIF_PATIENT.md](../Tests/PLAN_TEST_TARIF_PATIENT.md) |

## Historique et informations complémentaires

| Document | But | Lien |
|---|---|---|
| Historique RichTextEditor | Historique des décisions et implémentations de l’éditeur riche. | [Historique_Implementation_RichTextEditor.md](../Conception/Historique_Implementation_RichTextEditor.md) |
| Rapport d’audit du 17/05/2026 | Résultats de l’audit technique du 17/05/2026 (l’audit en cours est celui du 18/10/2026, plus haut). | [RAPPORT_AUDIT_2026-05-17.md](../Conception/RAPPORT_AUDIT_2026-05-17.md) |

## Règle de maintenance

Lorsqu’un document est créé, déplacé, renommé ou devient obsolète, mettre à jour cette bibliothèque dans le même changement. Lorsqu’un sujet possède une source de vérité spécialisée, le README et les autres documents doivent y renvoyer plutôt que dupliquer son contenu.

# Contact

> ***Joëlle De Poorter - Les Artefacts de Manou***
>
> Projet réalisé pour soutenir une pratique de psychologie et de graphologie dans la gestion fiable et évolutive des patients et documents.
>
> - Site web Auteur/Les Artefacts de Manou/Joëlle De Poorter : https://lesartefactsdemanou.be
> - Site web Psychologue & Graphologue : https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu` et `lesartefactsdemanou@gmail.com`
> - GitHub privé : https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea
>
> ---

[TOC]
