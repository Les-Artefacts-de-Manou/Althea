# Althéa — Checklist projet V1 (référence consolidée)


> Document de référence unique pour l'état d'avancement V1.  
> Les vues opérationnelles et planning sont synchronisées dans :
> - `Docs/Todo/ToDo.md` (backlog actionnable)
> - `Docs/Todo/Planning_actions_Althéa.md` (feuille de route)
> 
>  *Dernière mise à jour : 24/10/2026*

---

## Légende

- ✅ **Fait** : implémenté dans le code principal
- ⚠️ **Partiel / en cours** : présent mais incomplet
- ❌ **À faire** : non démarré dans le code principal
- 🛠️ **POC** : exploré dans un projet séparé, non intégré au cœur

---

## 0) Synthèse globale (état actuel)

### ✅ Déjà accompli

- Socle technique : `Core`, `UI`, `Metier`, `Utils` ; démarrage sécurisé (`AppStartupManager`) ; configuration locale chiffrée (DPAPI) ; accès base unique (`DatabaseManager`) et SQL centralisé dans `Core/Database/Queries/`
- Sécurité : connexion, rôles User / SuperUser / Admin, élévation, gestion complète des utilisateurs
- Journalisation centrale sans données personnelles, consultable par l'Admin ; écran État du système (ADR-39)
- Cœur métier : **patients, dossiers, séances, tarification par domaine, quotas par catégories, paiements** (cohérence séances ↔ paiements, ADR-26 / ADR-27)
- Référentiels administrables (dont domaines = conventions, organismes et mutualités, pays, civilités, périodicités) ; fiche Cabinet / praticien
- Paramètres lus par le code (ADR-36) ; dossier de données à sous-dossiers fixes (ADR-35)
- Sauvegarde chiffrée et restauration (ADR-37 / ADR-38)
- Kit d'interface Althéa (refonte UI-1 à UI-6, nettoyage UI-8, grilles UI-9) ; éditeur de texte refondu (deux profils, textes types, correcteur d'orthographe — ADR-42 à ADR-45)
- Audit du 18/10/2026 : lots A, B, C.1 et C.2 corrigés et validés (20–23/10/2026) — intégrité des paiements, opérations d'un seul tenant, droits et règles de saisie côté métier (ADR-46 à ADR-51)
- Recherche transversale : par code, séances et paiements d'une période (ADR-52, 23/10/2026)
- Documentation technique et plans de tests par module

### ❌ Reste majeur

- Suite de la recherche transversale (R4, R5), validation code postal / localité, puis suite de l'audit (C.3 à C.5 ; C.6 avec Documents ; lot D)
- Fin de la refonte UI (UI-7 ; UI-6 terminé le 14/10/2026)
- Documents et agenda intégrés au cœur (depuis les POC)
- Installation chez Pearl (package, configuration initiale, comptes MariaDB, version 1.0.0)
- Campagne de tests transverses et stabilisation avant gel V1

### 🛠️ POC déjà réalisés (hors code principal)

- Gestion documentaire (Drive/Docs, DocIO/DocToPdf)
- Agenda (Google Calendar / Scheduler)

---

## 1) Checklist détaillée V1

## 1. Cadrage fonctionnel V1

- ✅ Modèle métier clarifié (Patient / Domaine / Dossier / Séance / Tarif / Quota / Paiement) — [Concepts métier](../Architecture/Concepts_Metier_Althea.md)
- ✅ Orientation produit explicitée dans le README et les docs process
- ⚠️ Clarification finale du volet "Realism" (contenu métier précis à formaliser)
- ❌ Gel final du périmètre V1 (in/out définitif par module)

## 2. Architecture et socle applicatif

- ✅ Solution/projet WinForms .NET 8 structuré
- ✅ Dossiers techniques et séparation des responsabilités en place
- ✅ Modules socle créés : `DatabaseManager`, `ConfigManager`, `GestionLog`, `AppStartupManager`
- ✅ Navigation centralisée (`NavigationManager`) et contexte UI partagé (`UserControlContext`)
- ✅ Helpers UI mutualisés (`UITheme`, `UtilsButtons`, `UtilsControls`, `UtilsDataGrid`, `UtilsValidation`, `UtilsIcons`)
- ✅ DialogChoix personnalisé implémenté et déployé dans toute l'application
- ✅ Version de base attendue déclarée dans le code (`VersionSchema`), affichée dans l'État du système
- ✅ Contrôle de la version de base au démarrage (audit lot A, 20/10/2026 : en retard → fermeture, plus récente → avertissement)
- ✅ Gestionnaire global des erreurs imprévues (`GestionErreursGlobales`)
- ✅ Unité de travail transactionnelle (`DatabaseManager.ExecuterEnTransaction`, ADR-46)
- ✅ Droits et règles de saisie côté métier (`ContexteOperation`, `Autorisation`, `ErreurSaisie` — ADR-50, ADR-51)
- ❌ Codes système centralisés, découplage des écrans de `Home`, découpage des gros fichiers (audit C.3 à C.5)
- ❌ Tests automatisés et intégration continue (audit lot D)

## 3. Base de données

- ✅ Tables techniques, sécurité, référentielles et métier en place (schéma v2.55.0)
- ✅ Vues SQL du solde des séances et du dossier dominant (`v_seances_solde`, `v_dossier_dominant`, v2.49.0, ADR-48)
- ✅ Clés : séquences pour les tables métier, `AUTO_INCREMENT` admis pour référentiels, liaisons de paramétrage et tables techniques
- ✅ Migrations versionnées avec script de retour arrière ; documentation DB à jour (`Database_technique.md`)
- ⚠️ Tables documents et agenda à étendre depuis les POC
- ⚠️ Colonnes texte conservées après passage en référentiel (`patients.lateralite`, `paiements.mode_paiement`, `therapeutes.specialite`) : à retirer après vérification

## 4. Sécurité et accès

- ✅ Authentification utilisateur opérationnelle
- ✅ Gestion rôles (`User`, `SuperUser`, `Admin`) et élévation temporaire
- ✅ Chiffrement des secrets (DPAPI) + hachage mots de passe (PBKDF2)
- ✅ Journaux sans données personnelles (identifiants seulement, doublons masqués — ADR-39)
- ✅ Gestion complète utilisateurs (création, modification, consultation selon rôles)
- ✅ Actions admin sécurisées (reset password, déverrouillage compte)
- ✅ Gestion mots de passe temporaires sécurisés (générateur cryptographique, sans repli constant)
- ✅ Règle du mot de passe unique (`PolitiqueMotDePasse`), appliquée par le métier (23/10/2026)
- ✅ Élévation limitée : essais comptés, fin après inactivité (v2.48.0)
- ✅ Droits vérifiés par le métier sur le rôle courant, lignes système protégées dans le SQL (audit lot A, C.1)
- ✅ Plus de mot de passe ni de clé de licence dans le code (ADR-41, audit lot A)
- ✅ Nouvelle clé Syncfusion mise en place (23/10/2026) ; ⚠️ historique Git du miroir public à purger (anciennes clés, ancien mot de passe)
- ⚠️ Comptes `root` MariaDB à régler à l'installation (identification Windows `gssapi`, `root@'%'`)
- ⚠️ Durcir les scénarios d'exploitation (tests d'erreurs/permissions complets pour modules métier)

## 5. UI — coque et administration

- ✅ Form principale `Home` opérationnelle
- ✅ Écrans d'accès : login, changement mot de passe, configuration connexion, élévation accès
- ✅ `UC_Accueil`, `UC_AdminHome`, `UC_Parametres` opérationnels
- ✅ `UC_Utilisateurs` complet (liste, recherche/filtres, création, modification, consultation, activation/désactivation)
- ✅ `UtilisateurEdition` complet (Form modale avec 3 modes, reset password, déverrouillage)
- ✅ `DialogChoix` opérationnel (remplacement complet MessageBox)
- ✅ Icônes d'état centralisées (`UtilsIcons`) avec priorités (verrouillé > actif > inactif)
- ✅ `UC_Sauvegardes` / `DialogRestauration`, `UC_Journaux`, `UC_EtatSysteme` (validés le 12–13/10/2026)
- ✅ Écrans métier patients / dossiers / séances / paiements
- ⚠️ Refonte UI :
  - UI-6 (Domaines) réalisée et validée le 14/10/2026 ;
  - UI-7 en cours (`DialogChoix` V2, champ de date du kit, fiches Patient et Dossier, fenêtres Contact, Intervenant, Thérapeute et Utilisateur faits le 14/10/2026 ;
    - toutes les autres fenêtres et `UC_Cabinet` (onglets Cabinet, Praticien) le 15/10/2026 ;
    - champ d'heure le 16/10 ;
    - grille des Paiements le 17/10 ;
    - plus aucun `DateTimePicker` le 20/10 ;
    - restent quelques UserControls, les `NumericUpDown` et la lecture seule lisible écran par écran) ;
  - UI-8 (nettoyage) et UI-9 (grilles en pastilles) terminés et validés le 18/10/2026
- ✅ Éditeur de texte refondu : profils Complet et Compact, « Ouvrir en grand », en-tête et pied de page, textes types, correcteur d'orthographe (19–20/10/2026 ; plan RTE-01 à RTE-93 à dérouler)
- ✅ Modifications non enregistrées : même alerte dans toute l'application (ADR-43)
- ✅ Référentiels Latéralités, Textes types, Spécialités ; écran Types d'identifiant (hub Administration)

## 6. Métier patients/dossiers/suivi

- ✅ Recherche patients (filtre Suivi, domaine dominant)
- ✅ Fiche patient multi-onglets (identité, anamnèse, famille-contacts, intervenants, dossiers)
- ✅ Gestion dossiers (statuts, cycle de vie, zones cliniques)
- ✅ Gestion séances (saisie, statuts, tarif figé, report, chevauchement)
- ✅ Gestion paiements et liaison aux séances (règle unique, avoirs, traçabilité ; annulation et utilisation d'avoir tracées en base, ADR-49)
- ✅ Tarification par domaine et quotas par catégories (un seul calcul du quota, audit B.6)
- ⚠️ Pointage réel des séances passées (avec l'Agenda) — en attendant, une séance passée non confirmée s'affiche « À confirmer » (ADR-47)
- ✅ Recherche transversale R1 à R3 : par code, séances et paiements d'un jour ou d'une période (`UC_RechercheHome`, `UC_RechercheSeances`, `UC_RecherchePaiements`, 23/10/2026)
- ✅ Recherche R4 patients par lieu (`UC_RecherchePatientsLieu`, 24/10/2026, RCH-40 à RCH-50 validés)
- ✅ Recherche R5 réseau par nom (`UC_RechercheReseau`, 24/10/2026, RCH-51 à RCH-66 validés)
- ✅ Le code toujours visible : règle `Rules.md` § 15, écarts corrigés (Planning du jour, Utilisateurs, réseau, listes de choix ; 24/10/2026, RCH-67 à RCH-71 validés)
- ✅ Validation code postal / localité (codes postaux belges, v2.56.0, 24/10/2026, PAT-113 à PAT-129 et Cabinet 4.15 à 4.18 validés)

## 7. Documents & agenda

- ⚠️ POC documents validé, intégration applicative non faite
- ⚠️ POC agenda validé, intégration applicative non faite
- ✅ Périmètre V1 acté (D-13, D-14)

## 8. Sauvegarde, restauration, installation

- ✅ Sauvegarde DB + fichiers : archive ZIP AES-256, rotation, journal, rappel (lot 30 P2, 12/10/2026, ADR-37)
- ⚠️ Procédure de restauration testée de bout en bout (lot 30 P3 validé le 13/10/2026, ADR-38, remise en état automatique encore à tester ; procédure manuelle dans le guide des sauvegardes)
- ⚠️ Package d'installation + initialisation guidée

## 9. Qualité, tests, stabilisation

- ✅ Base documentaire structurée et exhaustive
- ✅ Documentation UI complète (`Documentation_technique_UI_Althea.md`)
- ✅ Documentation architecture à jour (ADR, Process, Rules)
- ✅ Plans de tests par module (`Docs/Tests/`), exécutés au fil des lots
- ⚠️ Audit du 18/10/2026 : lots A, B, C.1, C.2 ✅ ; C.3 à C.6 et lot D à faire (suivi au § 9 du rapport)
- ⚠️ Campagne de tests fonctionnels transverses avant gel V1
- ⚠️ Corrections UX finales et durcissement erreurs limites

---

## 2) Risques/contraintes identifiés

- Le build/test `dotnet` échoue en environnement Linux pour ce projet WinForms cible Windows (contrainte d'environnement à prendre en compte pour CI non-Windows).
- Dépendance aux services Google (Drive, Calendar) pour les modules documents et agenda.
- Captures d'écran de la documentation UI à refaire après la refonte (liens en attente depuis le 16/10/2026).

---

## 3) Prochaine cible prioritaire

1. ~~Finaliser `UC_Utilisateurs`.~~ ✅ **TERMINÉ**
2. ~~Livrer un premier lot métier utilisable : recherche + fiche patient + dossier.~~ ✅ **TERMINÉ**
3. ~~Enchaîner sur séances + paiements.~~ ✅ **TERMINÉ**
4. ~~Sauvegarde / restauration, administration technique.~~ ✅ **TERMINÉ** (13/10/2026)
5. Audit du 18/10/2026 et recherche transversale : lots A, B, C.1, C.2 et recherche R1 à R3 ✅ (20–23/10/2026) ; suite : R4, R5, codes postaux belges, puis C.3 → C.4 → C.5 (C.6 au module Documents).
6. Terminer la refonte UI (UI-7).
7. Intégrer documents / agenda (au moins périmètre V1 minimal).
8. Terminer par installation + stabilisation.

---

## 4) Réalisations récentes (depuis audit 17/05/2026)

### ✅ Éditeur de texte, audit du 18/10/2026, recherche transversale (18–23/10/2026)

- Éditeur de texte refondu (ADR-42 à ADR-45) ; alerte des modifications non enregistrées (ADR-43)
- Audit : lots A (bugs et sécurité), B (intégrité des données), C.1 (droits côté métier) et C.2 (règles de saisie dans le métier) ; schéma 2.40.0 → 2.55.0
- Recherche transversale R1 à R3 (ADR-52)
- Détail dans [État du projet](ETAT_DU_PROJET.md), section 6

### ✅ Exploitation : sauvegarde, restauration, administration (12–13/10/2026)

- Sauvegarde chiffrée (ADR-37) et restauration (ADR-38) depuis l'écran Sauvegardes
- Écrans Journaux et État du système ; journaux sans données personnelles (ADR-39)
- Le détail des lots métier (patients, dossiers, séances, tarification, quotas, paiements, cabinet, refonte UI) est dans [État du projet](ETAT_DU_PROJET.md), section 6

### ✅ Administration complète (23/05-07/06/2026)

- **Gestion utilisateurs** : module complet opérationnel
  - `UC_Utilisateurs` : liste, recherche/filtres avancés, actions admin
  - `UtilisateurEdition` : création/modification/consultation, reset password, déverrouillage
  - `GestionUtilisateurs` : couche métier complète
  - Gestion rôles Admin/SuperUser avec droits différenciés

- **UI/UX cohérente**
  - `DialogChoix` : remplacement de tous les MessageBox (34 occurrences)
  - `UtilsIcons` : centralisation icônes d'état avec priorités
  - Support icônes animées GIF

- **Documentation exhaustive**
  - Mise à jour complète CHANGELOG, README, Rules, Process_Althea, Architecture_Decisions
  - Nouvelle documentation UI complète pour DialogChoix, UtilisateurEdition, UC_Utilisateurs
  - Plan de tests exhaustif créé et exécuté

- **Tests et corrections**
  - Tests manuels complets effectués
  - Bugs identifiés et corrigés (séquences MariaDB, rôles, filtres, sélection, états)
  - Validation workflows création/modification/consultation/reset/unlock



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

