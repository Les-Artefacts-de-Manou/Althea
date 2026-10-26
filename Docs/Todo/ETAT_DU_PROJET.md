# Althéa - Etat du projet

> Document de synthèse vivant. Donne en un coup d'œil le statut réel du projet, les décisions actées, les dettes techniques connues et les prochaines étapes.
> Dernière mise à jour : 25/10/2026

> 📐 **Plan directeur de la phase métier** : [`Docs/Conception/Plan_Conception_Metier_Althea.md`](../Conception/Plan_Conception_Metier_Althea.md) — source de vérité détaillée des décisions de cadrage (D-Q1 à D-Q10).

---

## Légende

| Icône | Signification |
|-------|---------------|
| ✓ | Implémenté et stable |
| ⚠ | Présent mais incomplet / en cours |
| ❌ | Non démarré |
| 🧪 | POC réalisé, non intégré au cœur |

---

## 1. Statut des modules

### 🏗️ Socle technique

| Module | Fichier(s) principal(aux) | Statut | Notes |
|--------|--------------------------|--------|-------|
| Démarrage applicatif | `Core/Startup/AppStartupManager.vb`, `GestionErreursGlobales.vb` | ✓ | Orchestration complète ; bloquant par conception ; version de la base contrôlée (en retard : message et fermeture ; plus récente : avertissement) ; erreur imprévue journalisée, message clair sans détail technique (audit lot A, 20/10/2026) |
| Configuration locale | `Core/Configuration/ConfigManager.vb`, `LocalDbConfig.vb` | ✓ | Fichier JSON chiffré dans `%APPDATA%\Althea` ; écrit dans un fichier temporaire qui remplace l'ancien (audit § 4.5, 20/10/2026) |
| Connexion base de données | `Core/Database/DatabaseManager.vb` | ✓ | Point d'accès DB unique, connexion MariaDB ; chaîne construite par `MySqlConnectionStringBuilder`, test sans toucher la connexion de l'application (20/10/2026) ; unité de travail `ExecuterEnTransaction` : une opération = une transaction, tout ou rien (ADR-46) |
| Logging centralisé | `Core/Logging/GestionLog.vb`, `LecteurJournaux.vb` | ✓ | Sans données personnelles (identifiants seulement, doublons MariaDB masqués, ADR-39) ; conservation réglable (`LOG_RETENTION_JOURS`, purge au démarrage) ; consultation par l'Admin (`UC_Journaux`) |
| Chiffrement secrets (DPAPI) | `Core/Security/CryptoManagerDPAPI.vb` | ✓ | Scope `CurrentUser`, Base64 |
| Hachage mots de passe | `Core/Security/PasswordSecurityHelper.vb`, `Metier/Security/PolitiqueMotDePasse.vb` | ✓ | PBKDF2 ; règle du mot de passe définie une fois (`PolitiqueMotDePasse` : 10 caractères pour les comptes, 12 pour les archives), appliquée par le métier (23/10/2026) |
| Rôles et session | `Core/Security/AppRole.vb`, `UserSession.vb`, `UI/Context/SuiviActivite.vb` | ✓ | 3 rôles : User / SuperUser / Admin ; le rôle courant (élévation comprise) fait foi ; élévation limitée : essais comptés, fin après inactivité (`SECURITE_ELEVATION_MINUTES`, v2.48.0) |
| Droits et règles côté métier | `Metier/Security/ContexteOperation.vb`, `Autorisation.vb`, `Metier/Validation/ErreurSaisie.vb`, `ErreurValidationException.vb` | ✓ | Audit lots C.1 et C.2, terminés et validés les 22–23/10/2026 : méthodes sensibles reçoivent un `ContexteOperation`, garde unique `Autorisation` (ADR-50) ; règles de saisie dans un valideur métier, lu par l'écran, appliqué par l'écriture (ADR-51) ; affichage commun `AffichageErreursSaisie` |
| Dossier de données et chemins | `Utils/Helpers/ArborescenceDonnees.vb`, `CheminsPatientHelper.vb` | ✓ | Seule la racine `PATH_GENERAL` est paramétrable ; sous-dossiers fixes `Documents` / `Cabinet` / `Utilisateurs` (ADR-35, v2.37.0) ; `code_patient`, `AssurerDossierPatient`, `GetNomFichierPhotoIdentite` (ADR-20) |
| Paramètres lus par le code | `Metier/Parametres/ClesParametres.vb`, `GestionParametres.vb` | ✓ | Clés et valeurs de secours centralisées, lecture typée bornée (`GetEntier` / `GetBooleen`), écart journalisé (ADR-36) |
| Sauvegarde | `Metier/Sauvegardes/GestionSauvegardes.vb`, `Core/Configuration/MotDePasseSauvegarde.vb` | ✓ | Archive ZIP AES-256 : dump `mariadb-dump` + dossier de données + manifeste ; mot de passe DPAPI ; rotation, journal, rappel (ADR-37, v2.38.0) |
| Restauration | `Metier/Sauvegardes/GestionRestaurations.vb`, `OutilsMariaDB.vb` | ✓ | Implémentée (ADR-38, v2.39.0) : même version, compte d'administration MariaDB saisi, sauvegarde de sécurité, remise en état automatique ; validée le 13/10 (sauf 7.4 / 7.5 à revérifier ; 7.15 reporté) |

### 🧩 Interface utilisateur

| Module | Fichier(s) principal(aux) | Statut | Notes |
|--------|--------------------------|--------|-------|
| Shell principal | `UI/Forms/Home.vb` | ✓ | Form principale + verrouillage démarrage ; bouton **Recherche** du menu, sous Patients (23/10/2026) ; logo du cabinet lisible dans le bandeau (20/10/2026) |
| Navigation centralisée | `UI/Navigation/NavigationManager.vb` | ✓ | Contexte partagé `UserControlContext` |
| Dialogue personnalisé | `UI/Forms/Communs/DialogChoix.vb` | ✓ | Remplace tous les MessageBox, support icônes animées, thématisation UITheme |
| Écran connexion | `UI/Forms/Login/` | ✓ | Authentification, changement MDP |
| Écran configuration DB | `UI/Forms/Database/ConfigurationConnexion.vb` | ✓ | Test et enregistrement corrigés, messages d'aide et d'erreur (audit B1-B2, 20/10/2026) |
| Accueil / AdminHome | `UC_Accueil`, `UC_AdminHome` | ✓ | Hub d'administration complet avec élévation de droits |
| Paramètres | `UC_Parametres` | ✓ | Modification de la **valeur** seule (paramètres nés par migration), bornes, chemins vérifiés, rappel de portée (ADR-36) ; V2 du 16/10/2026 : une page, une carte par groupe (`tec_groupes_parametres`, v2.40.0), enregistrement en une transaction |
| Sauvegardes | `UC_Sauvegardes`, `DialogRestauration` | ✓ | SuperUser et Admin ; mot de passe des archives (Admin) ; jauge, historique ; navigation bloquée pendant l'opération ; restauration (Admin) validée |
| Journaux et état du système | `UC_Journaux`, `UC_EtatSysteme` | ✓ | Consultation des journaux (Admin, lecture seule) ; état du système (versions, base, dossiers, sauvegardes, rapport copiable) ; journaux sans données personnelles (ADR-39) — validé le 13/10 ; section « Cohérence des données » et réalignement des montants en attente (Admin, 22/10/2026) |
| Kit d'interface Althéa | `UI/Controls/Theme/` (`AltheaEnTete`, `AltheaCarte`, `AltheaBouton`, `AltheaOnglets`, `AltheaJauge`…) | ✓ | Refonte UI (lots UI-1 à UI-6, ADR-30 à ADR-34, ADR-40) ; `AltheaEtapes` ; UI-7 en cours — `DialogChoix` V2, `AltheaIconeEtat`, `AltheaDate` et `AltheaCalendrier` faits ; fiches Patient (Identité) et Dossier, fenêtres Contact, Intervenant, Thérapeute, Utilisateur et Séance en champs du kit (règle : fenêtres en cartes) ; fond des fenêtres `AltheaFondFenetre` (`Plan_Refonte_UI_Althea.md`) ; `AltheaHeure` (16/10), `AltheaZoneTexte` (20/10) ; plus aucun `DateTimePicker` Windows (20/10) ; bouton inactif qui dit pourquoi (`RaisonInactif`, 20/10) ; UI-8 nettoyage et UI-9 grilles en pastilles terminés et validés le 18/10/2026 |
| Utilisateurs | `UC_Utilisateurs` | ✓ | Liste, recherche/filtres, création, modification, consultation, activation/désactivation ; photo du compte et fiche en cartes (lot 28) ; boutons selon le rôle courant, élévation comprise (22/10/2026) |
| Édition utilisateur | `UtilisateurEdition.vb` | ✓ | Form modale : création/modification/consultation, reset password, déverrouillage ; garde-fous (pas de modification de son propre rôle, jamais sans administrateur actif) ; login et mot de passe jugés par le métier (`GestionUtilisateurs.ValiderCompte`, 23/10/2026) |
| Utils UI centralisés | `UtilsIcons.vb`, `UtilsButtons.vb`, `UtilsDataGrid.vb`, `UtilsPastilles.vb`, `GardeModifications.vb` | ✓ | Centralisation icônes d'état, styles boutons, config grilles ; pastilles à tons sémantiques (UI-9) ; ligne choisie avant d'agir (`ElementSelectionne`, onglets Séances et Paiements) ; alerte des modifications non enregistrées dans toute l'application (ADR-43) |
| Éditeur texte riche | `UC_RichTextEditor.vb`, `EditeurTexteAgrandi.vb`, `CorrecteurTexte.vb` | ✓ (temps 1 essayé par Joëlle, plan RTE-01 à RTE-93 à dérouler ; temps 2 testé les 19–20/10/2026) | Profils Complet et Compact (ADR-42, `UC_RichTextEditorSimple` supprimé) : styles de paragraphe, couleurs de la charte, listes, justifié, horodatage signé, recherche et remplacement, aperçu, impression Win32, exports PDF/Word Syncfusion délégués par événements (ADR-21), barre de consultation ; « Ouvrir en grand », en-tête et pied de page des documents (`EnTeteDocument`), textes types (ADR-44), correcteur d'orthographe de Windows (ADR-45) |
| Hub référentiels | `UC_ReferentielHome.vb` | ✓ | Tuiles par référentiel (dont Organismes, Latéralités, Textes types, Spécialités), droits SuperUser/Admin, navigation vers chaque référentiel ; tuile Domaines retirée le 20/10/2026 (menu principal) |
| Base commune référentiels | `UC_ReferentielBase.vb`, `Metier/Referentiels/ReglesReferentiels.vb` | ✓ | Classe de base héritable : CRUD, droits, validation, hooks champ supplémentaire ; colonne État en pastilles (UI-9) ; droit d'écriture vérifié par le métier et lignes système protégées dans le SQL (audit C.1, 22/10/2026) |
| Gestion des domaines (convention) | `UC_DomainesHome.vb`, `CategoriesQuotaEditor.vb` | ✓ | Écran de référence (menu Home › Domaines) : cinq étapes (Général / Tarifs / Catégories & quotas / Natures / Synthèse) avec leur état ; règles de quota corrigeables tant qu'elles n'ont pas servi (ADR-40) ; logo du domaine (v2.45.0) ; nouvelle règle de quota d'un seul tenant (ADR-46) ; saisie et complétude jugées par le métier (`ValiderDomaine`, `CalculerManques`, 23/10/2026) |
| Référentiel Domaines (ancien écran) | `UC_Domaines.vb` | 🗄️ | Archivé dans `UI/Forms/Test` (non compilé) le 02/10/2026, supprimé du dépôt le 18/10/2026 (UI-8, catégorie « code ») ; la tuile Référentiels › Domaines, qui ouvrait `UC_DomainesHome`, est retirée le 20/10/2026 (menu principal) |
| Référentiel Organismes | `UC_Organismes.vb` | ✓ | Pile complète ; ligne système INAMI protégée ; lot 29 (v2.33.0) : adresse, téléphones, e-mails, site, plateforme, procédure, périodicité |
| Référentiel Périodicités | `UC_Periodicites.vb` | ✓ | Lot 29 : `ref_periodicites` (`nb_mois` nullable), consommé par les organismes ; recette manuelle à faire |
| Pagination des listes | `UC_Pagination.vb` | ✓ | Contrôle commun réutilisable ; dans le thème, tri pris en charge (19/10/2026) ; Séances et Paiements du dossier paginés |
| Référentiel Liens patient | `UC_LiensPatient.vb` | ✓ | Pile complète |
| Référentiel Rôles intervenant | `UC_RolesIntervenant.vb` | ✓ | Pile complète |
| Référentiel Situations familiales | `UC_SituationsFamiliales.vb` | ✓ | Pile complète |
| Référentiel Situations juridiques | `UC_SituationsJuridiques.vb` | ✓ | Pile complète, porteur BCE (v2.31.0) |
| Référentiel Types d'identifiant | `UC_TypesIdentifiantPro.vb` | ✓ | 20/10/2026 : hub Administration, SuperUser ; porté par, masque, expression de contrôle ; INAMI, BCE, TVA lignes système (v2.47.0) ; testé le 20/10/2026 |
| Cabinet / praticien | `UC_Cabinet.vb` | ✓ | Cabinet, praticien, identifiants, qualifications, photo, cachet généré, compte de travail SuperUser lié (v2.29.0 → v2.32.0) ; tout s'enregistre ensemble, images et cachet différés jusqu'à Enregistrer (20/10/2026) ; compte du praticien créé d'un seul tenant, réservé à l'Admin |
| Référentiel Statuts dossier | `UC_StatutsDossier.vb` | ✓ | Pile complète |
| Référentiel Statuts séance | `UC_StatutsSeance.vb` | ✓ | Pile complète ; case « Absence non excusée (no-show) » (v2.53.0) |
| Référentiel Latéralités | `UC_Lateralites.vb` | ✓ | `ref_lateralites` (v2.46.0), lu par la fiche patient ; testé le 20/10/2026 |
| Référentiels Textes types | `UC_TextesTypes.vb`, `UC_UsagesTextesTypes.vb` | ✓ | Textes réutilisables proposés selon l'usage de l'éditeur (ADR-44, v2.42.0 → v2.43.0) ; testés le 19/10/2026 |
| Référentiel Spécialités | `UC_Specialites.vb` | ✓ | `ref_specialites` (v2.55.0), une spécialité par thérapeute, liste + bouton [+] dans la fiche Thérapeute ; validé le 23/10/2026 (icônes provisoires) |
| Référentiel Types documents | `UC_TypesDocuments.vb` | ✓ | Pile complète |
| Référentiel Types rendez-vous | `UC_TypesRendezVous.vb` | ✓ | Pile complète |
| Référentiels Pays et Civilités | `UC_Pays.vb`, `UC_Civilites.vb` | ✓ | `ref_pays` (ISO alpha-2, règles de validation par pays), `ref_civilites` ; utilisés dans toutes les saisies d'adresse et d'identité ; mutualité du patient = organisme de type `MUTUALITE` (R1–R4, v2.34.0 → v2.36.0) |
| Référentiels tarification et séances | `UC_NaturesSeance.vb`, `UC_PrixSeance.vb`, `UC_LiaisonsPrixDomaines.vb`, `UC_LiaisonsNaturesDomaines.vb` | ✓ | Paramétrage métier par domaine, périodes de validité et quotas par catégories (écran de référence : `UC_DomainesHome`) |
| Recherche patients | `UC_PatientHome.vb`, `UC_ApercuPatient.vb` | ✓ | Recherche multi-critères, domaine dominant, filtre domaine ; grille = hub de la journée (prochain rendez-vous, pastilles, résumé cliquable) et panneau d'aperçu (17/10/2026) ; NISS trouvé avec ou sans séparateurs |
| Recherche transversale | `UC_RechercheHome.vb`, `UC_RechercheSeances.vb`, `UC_RecherchePaiements.vb`, `UC_RecherchePatientsLieu.vb`, `UC_RechercheReseau.vb`, `Metier/Recherche/GestionRecherche.vb` | ⚠ | ADR-52 : hub du bouton Recherche ; R1 recherche par code, R2 séances et R3 paiements d'un jour ou d'une période (totaux encaissés par mode) faits et validés le 23/10/2026 ; ouverture ciblée (`OuvertureResultat`), période commune (`SelecteurPeriode`) ; R4 patients par lieu fait et validé le 24/10/2026 ; R5 réseau par nom fait et validé le 24/10/2026 |
| Fiche patient | `UC_PatientFiche.vb` | ✓ | Bandeau identité + onglets Identité / Anamnèse / Famille-Contacts / Intervenants / Dossiers ; modes Consultation/Création/Modification ; alerte avant de quitter sans enregistrer ; saisie jugée par le métier, erreurs groupées sur leurs champs (audit C.2, 23/10/2026) |
| Hub dossier | `UC_DossierFiche.vb` | ✓ | Identité, suivi clinique, séances et paiements ; dossier figé vérifié par le métier (clôturé : séances et paiements refusés ; archivé : tout refusé, 22/10/2026) ; grilles paginées, ligne choisie avant d'agir, colonne « Solde » retirée, bandeau « ici · tous dossiers » et ligne Prévu (22/10/2026) |
| Éditeur de séance et planning | `SeanceEditor.vb`, `PlanningJourViewer.vb` | ✓ | Tarif résolu, contrôle de chevauchement et planning journalier ; enregistrement et report d'un seul tenant (ADR-46) ; chevauchement, statuts proposés et montant libre (SuperUser) jugés par le métier (audit C.1–C.2) |
| Éditeur de paiement | `PaiementEditor.vb` | ✓ | Toujours ouvert sur sa séance (« Encaisser séance » ; « Nouveau paiement » et la sélection parmi les séances non soldées retirés le 22/10/2026) ; encaissement jugé par le métier (`ValiderEncaissement`) ; commentaire avec correcteur d'orthographe |
| Utilisation de l'avoir | `DialogUtilisationAvoir.vb` | ✓ | Remplace la réaffectation (21/10/2026) : l'encaissement d'une séance reportée ou non due devient un avoir, proposé aux séances dues du dossier ; chaque utilisation tracée (`lia_paiements_avoirs`, ADR-49) |

### 🧠 Cœur métier

| Module | Statut | Notes |
|--------|--------|-------|
| Patients (recherche, fiche, identité) | ✓ | `UC_PatientHome` + `UC_PatientFiche` : identité, alerte, famille/contacts, validations NISS, anti-doublon ; règles dans `GestionPatients.ValiderPatient` (ADR-51), NISS enregistré normalisé et affiché mis en forme (v2.54.0), homonyme signalé même sans date de naissance ; latéralité en référentiel (v2.46.0) |
| Patients (anamnèse) | ✓ | Onglet Anamnèse avec `UC_RichTextEditor`, sauvegarde double format, export PDF/Word contextuel horodaté (ADR-21) |
| Patients (photo d'identité) | ✓ | Upload bouton, formats GDI+ autorisés, nom fixe `Identite.ext`, `AssurerDossierPatient`, MAJ `patients.photo_fichier` (ADR-20) |
| Patients (intervenants) | ✓ | Réseau N-N via `autres_suivis_patient` + `ref_roles_intervenant` ; contacts, suivis et thérapeutes jugés par le métier, homonyme de thérapeute accepté après confirmation, spécialité en référentiel (v2.55.0), fiche Intervenant avec e-mail et adresse du thérapeute (23/10/2026) |
| Dossiers (statuts, cycle de vie) | ✓ | CRUD complet (`CreateDossier`/`UpdateDossier`), 7 zones cliniques (`UpdateZoneDossier`, garde-fou lecture seule `HistoriqueArchive`), transitions de statut par whitelist (réouverture depuis Clôturé, archivage terminal — D-Q5/D-Q10) via `DossierStatutEditor` ; le « suivi » du patient est **dérivé à la volée** de l'état du dernier dossier (plus de colonne `suivi_en_cours`) et affiché par le filtre/pastille de `UC_PatientHome` ; statut relu en base avant chaque transition (audit B.1) ; dossier dominant défini une fois (`v_dossier_dominant`, ADR-48) |
| Patients (activation manuelle) | ✓ | `is_actif` + `date_activation`/`date_desactivation` (migration v2.8.0), bascule Activer/Désactiver dans le bandeau, indépendante du cycle de vie des dossiers |
| Séances (saisie, planification) | ✓ | CRUD, notes, statuts, tarif résolu, contrôle de chevauchement et planning journalier ; écritures d'un seul tenant (ADR-46) ; une séance planifiée passée reste « Planifiée » et s'affiche « À confirmer » (ADR-47) ; absence non excusée réglée dans le référentiel des statuts (v2.53.0) ; liaison agenda automatique restant à intégrer |
| Paiements (liaison séances) | ✓ | Paiement rattaché à une séance, paiements partiels, reste dû défini une fois (`v_seances_solde`, ADR-48), filtres de suivi ; mode relié au référentiel, annulation en colonnes, utilisation d'avoir tracée (ADR-49, v2.50.0 → v2.52.0) ; réaffectation remplacée par l'avoir (21/10/2026) ; intégrité des avoirs et une seule ligne de dû par séance (audit B.1) |
| Tarification par domaine | ✓ | Règles portées par domaine, liaisons tarif/nature datées et montant figé dans la séance |
| Quotas par domaine | ✓ | Règles datées par domaine (`ref_quotas`), catégories libres et plafonds paramétrables, statuts consommateurs paramétrés, compteurs glissants et alerte métier ; un seul calcul du quota, règle relue après un changement de domaine (audit B.6, 22/10/2026) |
| Documents | 🧪 → périmètre acté | POC Drive/DocIO validé ; **V1 actée** : fichiers hors DB, chemin déterministe, Word local **et** Google Docs (D-Q4/D-Q7). Points d'ancrage posés dans le code (anamnèse export + upload photo) ; en-tête et pied de page des documents imprimés (19/10/2026) ; identifiant de trace d'un document corrigé (audit B5). Préparation du module : audit C.6, au démarrage. |
| Agenda | 🧪 → périmètre acté | POC Google Calendar/Scheduler validé ; **V1 actée** : Google = pilier, sync bidirectionnelle, reprise assistée (D-Q8) |

### 🗄️ Base de données

| Périmètre | Statut | Notes |
|-----------|--------|-------|
| Tables techniques (`tec_*`) | ✓ | Paramètres (bornes `valeur_min` / `valeur_max`, v2.37.0 ; groupes `tec_groupes_parametres`, v2.40.0 ; mention des documents, correcteur, modalité de paiement, fin de l'élévation, v2.41.0 → v2.48.0), méta-schéma, journaux des sauvegardes `tec_sauvegardes` (v2.38.0) et des restaurations `tec_restaurations` (v2.39.0) ; seule la racine `PATH_GENERAL` reste un chemin paramétré |
| Tables sécurité (`sec_*`) | ✓ | `sec_utilisateurs` avec gestion complète des rôles, verrouillage, élévation |
| Tables référentielles (`ref_*`) et liaisons | ✓ schéma + ✓ applicatif | Référentiels métier, prix, natures, modes, quotas et liaisons domaine–tarif/nature, avec administration dédiée ; textes types et leurs usages (v2.42.0 → v2.43.0), logo des domaines (v2.45.0), latéralités (v2.46.0), types d'identifiant système (v2.47.0), `ref_statuts_seance.est_absence` (v2.53.0), spécialités (v2.55.0), codes postaux belges (v2.56.0, contenu généré, lecture seule) |
| Tables métier patients | ✓ schéma + ✓ branché | v2.1 contacts/rôles légaux, v2.2 `photo_fichier`, v2.3 `anamnese_rtf/txt` — tous migrés et connectés |
| Tables métier (dossiers, séances, paiements, documents) | ✓ schéma / ✓ branché hors agenda-documents complet | Dossiers, séances et paiements sont connectés ; `paiements.id_mode_paiement`, colonnes d'annulation, `lia_paiements_avoirs` (v2.50.0 → v2.52.0) ; NISS normalisé (v2.54.0) ; documents et agenda restent à étendre depuis les POC |
| Vues SQL | ✓ | `v_seances_solde` (réglé, reste dû, échue, avoir) et `v_dossier_dominant` (v2.49.0, ADR-48) ; la restauration exige `CREATE VIEW` / `SHOW VIEW` |
| Scripts versionnés complets | ⚠ | Migrations versionnées avec retour arrière jusqu'à v2.56.0 (schéma actuel, `VersionSchema.Attendue`) ; la couverture documents/agenda reste à compléter |
| Séquences MariaDB | ✓ | Tables métier via `LASTVAL(seq_*)` (obligatoire) ; `AUTO_INCREMENT` admis pour les référentiels, les liaisons de paramétrage et les tables techniques (Rules.md) — les tables techniques anciennes (`tec_meta_schema`, `tec_parametres`…) gardent leur séquence |

### 🧩 Qualité & exploitation

| Élément | Statut | Notes |
|---------|--------|-------|
| Documentation technique | ✓ | README, Concepts métier (reconstitué le 13/10/2026), Process, Database, Rules, Standards, Architecture Decisions |
| Documentation UI complète | ✓ | `Documentation_technique_UI_Althea.md` (sommaire) et cinq parties par domaine dans `Docs/Architecture/UI/` (scindée le 25/10/2026, audit lot D point 4) ; toutes les Forms et UserControls ; illustrations complétées le 25/10/2026 |
| Documents de pilotage (Checklist / ToDo / Planning) | ✓ | Vues synchronisées dans `Docs/Todo/` |
| Plan de tests utilisateurs | ✓ | `PLAN_TESTS_UC_UTILISATEURS.md` complet |
| Audit du 18/10/2026 | ⚠ | `RAPPORT_AUDIT_2026-10-18.md`, suivi au § 9 : lots A (20–21/10), B (21–22/10), C.1 à C.5 (22–25/10) terminés et validés ; C.6 au module Documents ; lot D : documentation scindée (25/10), puis `Option Strict On` (D-1) ; installation, finition UI et documentation après Agenda, Documents et Accueil |
| Tests fonctionnels transverses | ⚠ | Campagne à lancer avant gel V1 |
| Sauvegarde / restauration | ✓ | Sauvegarde ✓ (v2.38.0, validée le 12/10) ; restauration validée le 13/10 (v2.39.0) (sauf 7.4 / 7.5 à revérifier ; 7.15 reporté) ; poste neuf : procédure manuelle (guide) |
| Package d'installation | ⚠ | Configuration initiale guidée à prévoir |

---

## 2. Décisions actées

| # | Décision | Statut |
|---|----------|--------|
| D-01 | Base de données centrale MariaDB (UTF8MB4) | ✓ Actée |
| D-02 | Chiffrement des secrets locaux par DPAPI (`CurrentUser`) | ✓ Actée |
| D-03 | Hachage mots de passe PBKDF2 | ✓ Actée |
| D-04 | Architecture en couches : `Core` / `Metier` / `UI` / `Utils` | ✓ Actée |
| D-05 | Point d'accès DB unique (`DatabaseManager`) | ✓ Actée |
| D-06 | SQL centralisé dans des modules `Query*` dédiés | ✓ Actée |
| D-07 | Démarrage applicatif bloquant (DB obligatoire à l'ouverture) | ✓ Actée |
| D-08 | Système de rôles à 3 niveaux (User / SuperUser / Admin) | ✓ Actée |
| D-09 | Navigation centralisée via `NavigationManager` | ✓ Actée |
| D-10 | Modes utilisateur distincts (Création/Modification/Consultation) | ✓ Actée |
| D-11 | Remplacement complet de MessageBox par DialogChoix personnalisé | ✓ Actée |
| D-12 | Centralisation des icônes d'état via UtilsIcons | ✓ Actée |
| D-13 | Intégration documents V1 : stockage **hors DB** (système de fichiers), chemin déterministe, flux Word local + Google Docs, export PDF | ✓ Actée |
| D-14 | Intégration agenda V1 : Google Calendar comme pilier, synchronisation bidirectionnelle, reprise assistée | ✓ Actée |
| D-15 | Composant `UC_RichTextEditor` + variante compacte `UC_RichTextEditorSimple` (double format) | ✓ Actée — variante remplacée par le profil Compact (D-34) |
| D-16 | Renommages métier : `volets`→`domaines`, `medecins`→`therapeutes`, réseau de suivi en N-N (`autres_suivis_patient` + `ref_roles_intervenant`) | ✓ Actée |
| D-17 | Séance créée dès la planification du rendez-vous lié, statut piloté par `ref_statuts_seance` | ✓ Actée |
| D-18 | UserControl dédié par référentiel sur une base commune (`UC_ReferentielBase`) — **9 UC concrets implémentés** | ✓ Actée + ✓ Implémentée |
| D-19 | Anticipation multi-utilisateur / multi-agenda dès la V1 (modèle préparé, activation différée) | ✓ Actée |
| D-20 | Stockage fichiers patient sous `{PATH_GENERAL}\Documents\{code_patient}` — chemin déterministe via `CheminsPatientHelper` (sous-dossier fixe depuis ADR-35) | ✓ Actée + ✓ Implémentée (ADR-20) |
| D-21 | Export `UC_RichTextEditor` par délégation d'événements (`ExportRequested`/`ExportCompleted`) — contrôle générique, logique métier dans le conteneur | ✓ Actée + ✓ Implémentée (ADR-21) |
| D-22 | Dossiers : identité générée par la base, statut Actif initial et référent porté par le patient | ✓ Actée + ✓ Implémentée (ADR-22) |
| D-23 | Documents exportés : nom déterministe, unicité par contexte et upsert | ✓ Actée + ✓ Implémentée (ADR-23) |
| D-24 | Tarification, no-show et quota : règles distinctes portées par le domaine et référentiels datés | ✓ Actée + ✓ Implémentée (ADR-24) |
| D-25 | Paiements contextualisés par séance ; statut d’encaissement distinct du solde de séance | ✓ Actée + ✓ Implémentée (ADR-25) |
| D-26 | Refonte UI : contrôles dessinés du kit Althéa, onglets hérités, deux niveaux de boutons | ✓ Actée + ✓ Implémentée (ADR-30 à ADR-34) |
| D-27 | Dossier de données : seule la racine est paramétrable, sous-dossiers fixes | ✓ Actée + ✓ Implémentée (ADR-35) |
| D-28 | Paramètres : une valeur, des bornes, une valeur de secours ; nés par migration | ✓ Actée + ✓ Implémentée (ADR-36) |
| D-29 | Sauvegarde : archive ZIP AES-256 (base + dossier de données), mot de passe propre au cabinet | ✓ Actée + ✓ Implémentée (ADR-37) |
| D-30 | Restauration : depuis l'écran, même version, sauvegarde de sécurité, compte d'administration saisi, journal | ✓ Actée + ✓ Implémentée (ADR-38) |
| D-31 | Journaux sans données personnelles ; version de base attendue déclarée dans le code | ✓ Actée + ✓ Implémentée (ADR-39) |
| D-32 | Domaines en étapes ; règle de quota corrigeable ou supprimable tant qu'elle n'a pas servi, figée ensuite | ✓ Actée + ✓ Implémentée (ADR-40) |
| D-33 | Clés de licence des composants tiers saisies dans l'application, chiffrées sur le poste, jamais dans le dépôt | ✓ Actée + ✓ Implémentée (ADR-41) |
| D-34 | Éditeur de texte : un seul contrôle à deux profils (Complet / Compact), styles de paragraphe, teintes de la charte | ✓ Actée + ✓ Implémentée (ADR-42 ; plan de tests à dérouler) |
| D-35 | Modifications non enregistrées : alerte tenue par l'écran, photo des champs, trois choix | ✓ Actée + ✓ Implémentée (ADR-43) |
| D-36 | Textes types : référentiel en base, proposés selon l'usage de l'éditeur | ✓ Actée + ✓ Implémentée (ADR-44) |
| D-37 | Correcteur d'orthographe : celui de Windows, dans la langue du clavier | ✓ Actée + ✓ Implémentée (ADR-45) |
| D-38 | Opérations en plusieurs écritures : une transaction, tout ou rien | ✓ Actée + ✓ Implémentée (ADR-46) |
| D-39 | Séance passée : la base garde le statut saisi, l'affichage dit « À confirmer » | ✓ Actée + ✓ Implémentée (ADR-47) |
| D-40 | Solde d'une séance et dossier dominant : une seule définition, dans des vues SQL | ✓ Actée + ✓ Implémentée (ADR-48) |
| D-41 | Paiements : mode relié par identifiant, annulation et utilisation d'avoir tracées en base | ✓ Actée + ✓ Implémentée (ADR-49) |
| D-42 | Droits côté métier : un `ContexteOperation` transmis, une garde unique `Autorisation` | ✓ Actée + ✓ Implémentée (ADR-50) |
| D-43 | Règles de saisie et d'action : un valideur métier, lu par l'écran, appliqué par l'écriture | ✓ Actée + ✓ Implémentée (ADR-51) |
| D-44 | Recherche transversale : un hub, des recherches spécialisées, un résultat et une ouverture communs | ✓ Actée + ⚠ En cours (ADR-52 : R1 à R5 faits et validés) |

> ⚠ Détail et justification de chaque décision : voir [`ARCHITECTURE_DECISIONS.md`](../Rules/ARCHITECTURE_DECISIONS.md) et le [plan de conception métier](../Conception/Plan_Conception_Metier_Althea.md).

---

## 3. Dettes techniques connues

| Réf | Description | Priorité | Impact |
|-----|-------------|----------|--------|
| DT-01 | Build `.NET` bloqué en environnement Linux (NETSDK1100, projet WinForms cible Windows) | Faible | Uniquement pour CI non-Windows ; aucun impact sur l'exécution réelle |
| DT-02 | ~~`UC_Utilisateurs` non finalisé~~ | ~~Haute~~ | ✓ **RÉSOLU** - Module complet et opérationnel |
| DT-03 | Couverture des migrations documents et agenda à compléter | Moyenne | Complexifie leur future intégration en production |
| DT-04 | Scénarios d'exploitation et recette transversale des modules métier à durcir | Moyenne | Risque de comportements inattendus aux limites |
| DT-05 | ~~Intégration des POC documents et agenda non planifiée~~ | ~~Moyenne~~ | ✓ **RÉSOLU** - Périmètre V1 acté (D-13, D-14) ; reste l'implémentation |
| DT-06 | Absence de campagne de tests fonctionnels transverses | Haute | Requise avant tout gel de la V1 |
| DT-07 | Captures d'écran de la documentation à refaire après la refonte UI (liens d'images en attente depuis le 16/10/2026) | Faible | Documentation UI, charte et guide de l'éditeur sans images à jour |
| DT-08 | ~~Migration de schéma à réaliser (Lot 0)~~ | ~~Haute~~ | ✓ **RÉSOLU** — Schéma migré + 9 piles applicatives référentiels complètes |
| DT-09 | Coexistence possible de photos d'identité multiples (ex. `identite.jpg` + `identite.png`) dans le dossier patient | Faible | Ambiguïté sur la photo active ; nettoyage à implémenter avec la brique Documents |
| DT-10 | Liaison automatique Agenda ↔ séances non implémentée | Moyenne | Double saisie potentielle jusqu’à l’intégration Google Calendar |
| DT-11 | Colonnes texte conservées après passage en référentiel, plus lues : `patients.lateralite` (v2.46.0), `paiements.mode_paiement` (v2.50.0), `therapeutes.specialite` (v2.55.0) | Faible | À retirer par migration une fois les reprises vérifiées |
| DT-12 | Codes système encore écrits dans le code ; comportement « séance non tenue » d'un statut en trois listes qui ne concordent pas (Maladie n'y libère pas le créneau) | Moyenne | Un statut ajouté n'a pas de comportement propre — audit C.3 (module `CodesSysteme`, drapeaux de référentiel) |
| DT-13 | Écrans couplés à `Home` et très gros fichiers (`UC_PatientFiche`, `UC_DossierFiche`) | Moyenne | Freine la maintenance et un futur passage au web — audit C.4 et C.5 |
| DT-14 | Aucun test automatisé ni intégration continue | Faible | Tests automatisés en place (`Tests/Althea.Tests`, 621 tests, ADR-58, 26/10/2026) ; intégration continue en place (ADR-59, 26/10/2026), premier passage à valider |
| DT-15 | Anciennes clés Syncfusion et ancien mot de passe dans l'historique Git (miroir public) | Haute | Nouvelle clé Syncfusion mise en place (23/10/2026) ; reste à purger l'historique du miroir (audit lot A, point 2) |
| DT-16 | Ligne choisie avant d'agir : faite pour les onglets Séances et Paiements seulement | Faible | Ailleurs, une action peut porter sur la première ligne sans qu'elle ait été choisie |

---

## 4. Prochaines étapes (ordre de priorité)

### ~~Priorité 1 - Finaliser l'administration~~ ✓ **TERMINÉ**
1. ~~Terminer `UC_Utilisateurs` (liste + détail + actions admin + contrôle d'accès + journalisation)~~ ✓
2. ~~Relire et fiabiliser le flux complet Login → Home → Admin~~ ✓
3. ~~Implémenter DialogChoix pour remplacer tous les MessageBox~~ ✓
4. ~~Centraliser les icônes d'état via UtilsIcons~~ ✓

### ~~Priorité 2 - Lot 0 : socle métier (migration de schéma)~~ ✓ **TERMINÉ**
5. ~~Versionner et appliquer la migration : `volets`→`domaines`, `medecins`→`therapeutes`~~ ✓
6. ~~Créer la liaison N-N `autres_suivis_patient` + le référentiel `ref_roles_intervenant`~~ ✓
7. ~~Créer/aligner `ref_statuts_seance` (cycle de vie séance)~~ ✓
8. ~~Créer la base UI commune `UC_ReferentielBase` + composants `UC_RichTextEditor` / `UC_RichTextEditorSimple`~~ ✓
   - ~~9 UC référentiels concrets déployés~~ ✓

### ~~Priorité 3 - Lot 1 : Patients (fiche de base)~~ ✓ **TERMINÉ** (14/06/2026)
9. ~~Créer `UC_PatientHome` (recherche + liste + bouton Nouveau)~~ ✓
10. ~~Créer `UC_PatientFiche` (identité, alerte, famille/contacts, validations)~~ ✓
11. ~~Implémenter onglet Anamnèse avec `UC_RichTextEditor` + export contextuel horodaté~~ ✓
12. ~~Implémenter upload photo d'identité (`CheminsPatientHelper`, `AssurerDossierPatient`, MAJ DB)~~ ✓
13. ~~Refactorer `CheminsPatientHelper` : `code_patient`, `PATH_GENERAL\PATH_DOCUMENT`~~ ✓
14. ~~Migrations DB v2.1/v2.2/v2.3 (contacts, photo_fichier, anamnese_rtf/txt)~~ ✓

### ~~Priorité 4 - Dossiers, séances, paiements et paramétrage métier~~ ✓ **TERMINÉ**
15. ~~Implémenter le réseau d’intervenants et le hub Dossier avec cycle de vie, zones cliniques et archivage~~ ✓
16. ~~Implémenter les séances : CRUD, tarif, statuts, contrôle de chevauchement et planning du jour~~ ✓
17. ~~Implémenter les paiements : affectation séance, reste dû calculé, paiements partiels et sélection d’une séance non soldée~~ ✓ (sélection retirée le 22/10/2026 : on encaisse depuis la séance)
18. ~~Rendre domaines, tarifs, natures et quotas portables par référentiels datés~~ ✓

### Priorité 5 - Audit du 18/10/2026 et recherche transversale (ordre validé par Joëlle)
19. ~~Audit, lot A (bugs et brèches), lot B (intégrité des données), lots C.1 (droits côté métier) et C.2 (règles de saisie dans le métier)~~ ✓ (20–23/10/2026)
20. ~~Recherche transversale R1 (par code), R2 (séances d'une période), R3 (paiements d'une période)~~ ✓ (23/10/2026)
21. ~~Recherche R4 : patients par lieu~~ ✓ (24/10/2026, `UC_RecherchePatientsLieu`, RCH-40 à RCH-50 validés).
22. ~~Recherche R5 : réseau par nom~~ ✓ (24/10/2026, `UC_RechercheReseau`, RCH-51 à RCH-66 validés). Dans la foulée : règle « le code toujours visible » (`Rules.md` § 15) et tour de l'application, écarts corrigés (RCH-67 à RCH-71 validés).
23. ~~Validation code postal / localité~~ ✓ (24/10/2026, v2.56.0, ADR-53, PAT-113 à PAT-129 et Cabinet 4.15 à 4.18 validés).
24. Audit C.3 : codes système centralisés et drapeaux de référentiel (dont le comportement « non tenu » des statuts de séance) — découpage validé le 24/10/2026 (ADR-54) ; étape 1 `CodesSysteme` faite et validée le 24/10/2026 (C3-01 à C3-06), étape 2 drapeaux (v2.57.0) faite et validée le 24/10/2026 (C3-07 à C3-16)  ; lignes système devenues inutiles déverrouillées le 25/10/2026 (v2.58.0).
25. Audit C.4 : découplage des écrans de `Home` — décisions de Joëlle le 25/10/2026 (ADR-55 : contrat `IFenetrePrincipale`, élévation réunie) ; étape 1 (contrat, contexte, fenêtres) faite et validée le 25/10/2026 ; étape 2 (écrans) faite et validée le 25/10/2026 ; étape 3 (recherches, hubs, élévation réunie) faite et validée le 25/10/2026 — C.4 terminé. C.5 : fiches dossier et patient réparties en fichiers partiels par onglet (ADR-56, fait et validé le 25/10/2026) — C.5 terminé.
26. Audit C.6 (préparation du module Documents) au démarrage de ce module. Lot D, ordre décidé par Joëlle le 25/10/2026 : grands documents scindés (doc UI par domaine, processus, CHANGELOG par mois) — fait le 25/10/2026 ; puis D-1 (ADR-57) : `Option Strict On` pour tout le projet, `.editorconfig` et analyseurs .NET — fait le 25/10/2026 ; D-2 tests automatisés (ADR-58) fait le 26/10/2026 (projet `Tests/Althea.Tests`, 621 tests dont le test de fumée des requêtes), puis D-3 intégration continue (ADR-59, workflow en place le 26/10/2026) ; installation et migrations, finition UI, reste de la documentation après Agenda, Documents et Accueil (priorité 6).

### Priorité 6 - Documents et agenda (intégration depuis POC)
27. Intégrer complètement le module **documents** V1 (fichiers hors DB, Google Docs/Drive, recherche et flux métier).
28. Intégrer le module **agenda** V1 (Google Calendar, synchronisation bidirectionnelle et création liée des séances).

### Priorité 7 - Exploitation, recette et stabilisation
29. ~~Sauvegarde DB + fichiers~~ ✓ (12/10/2026) ; restauration validée le 13/10/2026 ; assistant poste neuf avec l'installation.
30. Préparer le package d'installation avec configuration initiale guidée.
31. Exécuter la campagne de tests fonctionnels transverses (patients, dossiers, séances, tarifs, quotas, paiements).
32. Effectuer les corrections UX finales et durcir les cas limites.
33. Préparer la note de version V1 après recette.

---

## 5. Risques identifiés

| Risque | Probabilité | Impact | Mitigation |
|--------|-------------|--------|------------|
| Périmètre V1 qui s'étend (feature creep) | Moyenne | Élevé | Gel explicite du périmètre par module avant codage |
| Qualité du cœur métier insuffisante sans tests | Haute | Élevé | Planifier la recette fonctionnelle dès la phase 3 |
| Reprise du projet par un tiers sans documentation suffisante | Faible | Élevé | Maintenir ce fichier et les ADR à jour à chaque évolution majeure |
| Dépendance à des services externes (Google Drive, Google Calendar) dans les POC | Moyenne | Moyen | Définir une alternative locale si intégration impossible en V1 |

---

## 6. Réalisations récentes

### Recherche transversale R1 à R3 ✓ (23/10/2026, schéma inchangé)
- **ADR-52** : bouton « Recherche » du menu, hub `UC_RechercheHome` ; recherche par code (PA, DO, SE, PAI, FC, AS, saisie tolérante) ; ouverture ciblée (dossier sur le bon onglet, ligne sélectionnée ; Retour à la liste).
- **Séances** d'un jour ou d'une période, tous patients (`UC_RechercheSeances`) et **paiements** (`UC_RecherchePaiements` : relevé du dossier, filtre par mode, totaux encaissés par mode, avoir hors total).
- Filtres et résumés des onglets Séances et Paiements partagés (`FiltresSeances`, `FiltresPaiements`), période commune (`SelecteurPeriode`) ; journal sans terme recherché. Validé par Joëlle ; plan `PLAN_TESTS_RECHERCHE.md` (RCH-01 à RCH-39).

### Audit, lot C : droits et règles de saisie côté métier ✓ (22–23/10/2026, schéma 2.53.0 → 2.55.0)
- **C.1** (ADR-50) : `ContexteOperation` et garde unique `Autorisation` ; montant libre, séance encaissée et dossier figé vérifiés par le métier ; droit d'écriture des 25 référentiels et lignes système vérifiés par le métier et le SQL.
- **C.2** (ADR-51) : valideurs métier (`ErreurSaisie`, `ErreurValidationException`, affichage `AffichageErreursSaisie`) pour séances et paiements, fiche patient (NISS normalisé, v2.54.0), contacts, intervenants et thérapeutes (homonyme confirmé), domaines (saisie et complétude), comptes (`PolitiqueMotDePasse`, login).
- **Spécialités des thérapeutes** en référentiel `ref_specialites` (v2.55.0, étape intercalée).
- **Onglets Séances et Paiements** (22/10) : ligne choisie avant d'agir, « Nouveau paiement » et colonne « Solde » retirés, bandeau plus clair (ligne Prévu), réalignement des montants en attente dans l'État du système.
- Chaque étape testée et validée par Joëlle ; C.3 à C.5 restent, C.6 au module Documents.

### Audit, lot B : intégrité des données ✓ (21–22/10/2026, schéma 2.48.0 → 2.53.0)
- **B.1** : avoirs, dû unique par séance, statut du dossier relu en base. **B.2** (ADR-46) : séance, report, règle de quota, compte du praticien d'un seul tenant ; réaffectation remplacée par l'avoir (`DialogUtilisationAvoir`).
- **B.3** (ADR-47) : « À confirmer » au lieu d'une bascule en « Réalisée ». **B.4** (ADR-48) : vues `v_seances_solde` et `v_dossier_dominant` (v2.49.0).
- **B.5** (ADR-49) : mode de paiement par identifiant (v2.50.0), annulation en colonnes (v2.51.0), `lia_paiements_avoirs` (v2.52.0). **B.6** : un seul calcul du quota ; absence non excusée en case du référentiel (v2.53.0).
- Filtres Séances et Paiements : « Aujourd'hui », « Reportées », pastilles de couleur (21/10). Toutes les étapes validées par Joëlle.

### Audit, lot A et finitions ✓ (20–21/10/2026, schéma 2.44.0 → 2.48.0)
- **Lot A** : bugs B1 à B12, plus de mot de passe dans le code ni de traces `[DEBUG]`, erreurs imprévues journalisées, version de la base contrôlée au démarrage, droits sur le rôle courant, fichiers de configuration écrits sans risque, élévation limitée (v2.48.0).
- **Configuration de connexion** réparée (test, enregistrement, messages). **Fiche Cabinet** enregistrée d'un seul tenant, images différées.
- **Logo des domaines** (v2.45.0), **latéralités** en référentiel et modalité de paiement en paramètre (v2.46.0), **types d'identifiant** : écran de gestion, INAMI, BCE, TVA lignes système (v2.47.0).
- **Kit** : boutons inactifs lisibles et qui disent pourquoi ; plus aucun `DateTimePicker` Windows.

### Éditeur de texte refondu ✓ (19–20/10/2026, schéma 2.40.0 → 2.44.0)
- **Temps 1** (ADR-42) : un contrôle, deux profils (Complet / Compact), `UC_RichTextEditorSimple` supprimé ; essayé par Joëlle, plan RTE-01 à RTE-93 à dérouler au fil des essais.
- **Temps 2**, testé par Joëlle : « Ouvrir en grand », en-tête et pied de page des documents (v2.41.0), textes types et leurs usages (ADR-44, v2.42.0 → v2.43.0), correcteur d'orthographe de Windows (ADR-45, v2.44.0), aussi dans le commentaire d'un paiement et le motif d'annulation (`AltheaZoneTexte`).
- **Modifications non enregistrées** : même alerte à trois choix dans toute l'application (ADR-43, `GardeModifications`). **Pagination** dans le thème ; Séances et Paiements du dossier paginés.

### Nettoyage, grilles et rapport d'audit ✓ (17–18/10/2026)
- **UI-8** : fichiers, ressources, code et documents inutilisés retirés (quatre catégories validées). **UI-9** : états des grilles en pastilles à tons sémantiques ; icônes `Tech_Ico` retirées.
- **Clé de licence Syncfusion** hors du dépôt, saisie dans Paramètres (ADR-41). Erreur de tarif à l'ouverture d'une séance corrigée.
- **Rapport d'audit complet** du 18/10/2026 (`RAPPORT_AUDIT_2026-10-18.md`) : plan en lots A à D, suivi au § 9.

### Grilles des séances et des paiements lisibles (17/10/2026)
- Séances : pastilles d'état (statut, paiement), solde unique, ligne « Aujourd'hui », résumé cliquable.
- Paiements : relevé chronologique (maquette A) — pastille « État » aux couleurs des séances, colonne « Pour », résumé cliquable (encaissé, en attente, avoir disponible, reste à régler), filtre « À régler », tri rétabli.
- Cohérence des montants (préparation des tests) :
  - lignes et totaux au montant affecté (paiement réparti, avoir utilisé),
  - une seule définition de l'avoir (séances reportées comprises, trop-perçu d'une séance due),
  - reliquat de réaffectation laissé en avoir sur la séance d'origine,
  - « Encaisser » inactif sur une séance soldée,
  - bouton « Annuler l'encaissement ».
- Nouveau **Guide séances et paiements** (remplace le relevé des conséquences). Tests approfondis du plan Paiements faits (17/10/2026 ; reste P-15, facultatif).
- Clôture d'un dossier contrôlée : refusée tant que des séances sont à venir, montants à régler signalés ; statut du dossier bien visible (pastille colorée, bandeau « consultation » d'un dossier clôturé ou archivé).
- Écran Patients = hub de la journée : prochain rendez-vous, séance du jour, dernière séance, suivi et paiements en pastilles, trois groupes (aujourd'hui / à venir / sans rendez-vous), résumé cliquable, raccourci vers la séance (prise de notes), icônes d'état dessinées, panneau d'aperçu du patient à droite.

### Champ d'heure du kit (16/10/2026, à tester)
- **`AltheaHeure`** (maquette, liste A) : saisie hh:mm, liste des heures proposées avec la durée pour une heure de fin, lecture seule lisible ; fenêtre Séance et « Reporter la séance ».

### Paramètres sur une page (16/10/2026, à tester)
- **`UC_Parametres` V2.0.0** (maquette A) : une carte par groupe, aide sous chaque valeur, consultation lisible, un seul Modifier ; les valeurs changées s'enregistrent en une transaction (`GestionParametres.UpdateValeursParametres`).
- **Migration v2.40.0** : `tec_groupes_parametres` (titre et ordre des groupes, clé étrangère) ; `VersionSchema.Attendue` = 2.40.0.

### Référentiels : panneau « Détail » ✓ (16/10/2026)
- **`UC_ReferentielBase` V1.4.0** : carte « Détail » à la règle des fenêtres, État en dernier, consultation lisible ; `AjouterLigneDetail` pour les champs propres (7 référentiels repris, contrôles du kit, plus de couleurs en dur).

### Vitrine du kit ✓ (16/10/2026)
- **`TestKitAlthea` V2.0.0** : tous les contrôles du kit dans leurs états, en six onglets ; captures intégrées à `Reference_UI.md` ; correctif des titres fantômes sous les contrôles à fond recopié.
- **Cabinet** : onglets Identifiants et Cachet en cartes ; `MessageBox` remplacés par `DialogChoix`.

### DialogChoix V2.1 : petite fenêtre du kit ✓ (16/10/2026)
- **`DialogChoix` V2.1.0** (maquette « C ») :
  - barre de titre et fond beige à la feuille,
  - en-tête `AltheaEnTete` (le type se lit sur l'icône, titre mauve),
  - message dans une carte,
  - boutons dans une `AltheaBarreActions` et dans l'ordre habituel (action à gauche, Annuler à droite) ;
  - la croix de la barre de titre vaut Échap.
  - Méthodes et résultats inchangés.
- **Kit** : `AltheaIconeEtat.CreerImage` (icône en image), `AltheaFondFenetre.LargeurZone` publique.

### UI-7 commencé : DialogChoix V2 et champ de date du kit ✓ (14/10/2026)
- **`DialogChoix` V2.0.0** (maquette « B ») :
  - en-tête teinté selon le type,
  - icônes dessinées par le nouveau contrôle `AltheaIconeEtat` (plus de GIF),
  - corps à parties facultatives (détails, complément, attention, remarque, question finale),
  - boutons du kit qui disent l'action (`Confirmer(…, texteAction, texteAnnuler)`, `ConfirmerSuppression`),
  - taille selon le texte, Entrée / Échap.
  - Les 202 appels existants sont inchangés.
- **Adoption** : règles de quota des Domaines ; **démonstration** : `TestDialogChoix` (bouton de test de l'accueil, Debug).
- **Fiche patient** : onglet Identité aux champs du kit (listes `AltheaListe`, date de naissance `AltheaDate` facultative) et en consultation lisible.
- **Champ de date du kit** : `AltheaDate` (saisie au clavier, calendrier, date facultative « vide + croix », lecture seule lisible) et `AltheaCalendrier` (calendrier dessiné) ; démonstration `TestChampDate` ; à poser écran par écran.
- **Décidé** : le tour « lecture seule lisible » se fait écran par écran avec UI-7.

### Refonte de l'écran Domaines — UI-6 ✓ (14/10/2026, schéma inchangé)
- **Écran** : parcours en cinq étapes (`AltheaEtapes`, état de chaque étape), Général en cartes, Tarifs et Natures en carte avec barre locale (panneaux de liaison V2), Catégories & quotas en deux cartes, Synthèse en carte.
- **Quotas** : nouvelle règle guidée et récapitulée, date de début passée admise, périodes sans règle jamais comblées automatiquement mais mises en évidence (ligne d'alerte, étape et synthèse) ; règle corrigeable ou supprimable tant qu'elle n'a pas servi (la suppression rouvre la précédente), figée ensuite (ADR-40).
- **Transverse** : règle « Consultation lisible » (`UtilsControls.DefinirLectureSeule`, `ColorChampLectureSeule`) ; `DialogChoix` V1.1.0 (taille adaptée au texte).
- **Documentation** : guide utilisateur Domaines revu, plan de tests Domaines V2.0, table des ADR complétée.

### Restauration, administration et revue documentaire ✓ (13/10/2026, schéma 2.39.0)
- **Lot 30, P3 — restauration** : depuis l'écran Sauvegardes (Admin), archive de même version, compte d'administration MariaDB saisi, sauvegarde de sécurité préalable, remise en état automatique en cas d'échec, journal `tec_restaurations` (ADR-38) ; plan de tests validé (7.4 / 7.5 à revérifier, 7.15 reporté).
- **Administration** : écran Journaux (lecture seule, filtres, détail) et écran État du système (versions, base, dossiers et espace disque, sauvegardes, rapport copiable) ; journaux expurgés des données personnelles ; version de base attendue déclarée dans le code (`VersionSchema`, ADR-39) ; plan de tests validé.
- **Conventions** : règle des clés alignée (séquences pour le métier, `AUTO_INCREMENT` admis ailleurs) ; convention de numérotation des versions de l'application actée (1.0.0 à la première installation chez Pearl).
- **Documentation** : `Concepts_Metier_Althea.md` reconstitué (il contenait par erreur une copie des ADR depuis le 26/09).

### Cabinet, refonte UI, référentiels de saisie, paramètres et sauvegarde ✓ (06–12/10/2026, schéma 2.28.0 → 2.38.0)
- **Cabinet / praticien** (lots 27–28) : fiche Cabinet, cachet généré, compte de travail SuperUser lié, photo des comptes utilisateurs.
- **Organismes étoffés et périodicités** (lot 29).
- **Refonte UI** (UI-1 à UI-5) : kit d'interface Althéa (en-tête commun, cartes, boutons, onglets, bandeaux de recherche, barres d'actions), Home redessinée, import d'image unifié (`ImagesHelper`, `UtilsImages`).
- **Référentiels de saisie** (R1–R4) : pays, civilités, mutualités ; anciennes colonnes texte supprimées.
- **Lot 30, P1** : paramètres lus par le code (séances, sécurité, journaux, encre du cachet), arborescence fixe du dossier de données, écran Paramètres limité à la valeur.
- **Lot 30, P2** : sauvegarde complète chiffrée et écran Sauvegardes ; plan de tests validé.

### Refonte Domaine = convention ✓ (30/09–01/10/2026, schéma 2.21.0 → 2.28.0)
- **Modèle** : un domaine = tarif + organisme + quotas (ADR-27). Organisme facultatif (`ref_organismes`), quotas génériques `ref_quotas`, catégories par domaine `ref_categories_quota`, plafonds `ref_quotas_plafonds` ; catégorie consommée portée par la liaison nature–domaine ; `ref_statuts_seance.consomme_quota`.
- **Lignes système** protégées (`est_systeme`, ADR-28).
- **UI** : nouvel écran `UC_DomainesHome` + `CategoriesQuotaEditor`, `UC_Organismes`, `UC_Pagination`.
- **Historisation** : une règle de quota ne se modifie jamais ; l'ajout clôture automatiquement la règle ouverte. *(Assoupli le 14/10/2026, ADR-40 : une règle qui n'a pas encore servi peut être corrigée ou supprimée.)*
- **Nettoyage** : anciennes colonnes (`code_prestation_inami`, plafonds soutien/traitement…) supprimées.
- **Validation** : build et tests réussis, commit effectué. Guide : `Docs/Guide/Guide_Utilisateur_Domaines.md`.

### Fiabilisation séances ↔ paiements ✓ (25–26/09/2026)
- **Règle unique** `GestionPaiements.ResynchroniserDuSeance` appelée après création, modification, changement de statut, encaissement, annulation et utilisation d'avoir (la réaffectation est remplacée par l'avoir depuis le 21/10/2026).
- **Traçabilité** : aucun paiement supprimé ; annulation d’un encaissement réservée SuperUser/Admin avec motif obligatoire (`DialogMotifAnnulation`, form Designer) et mention auteur/date/motif.
- **Calculs** : réglé, reste dû et avoir basés sur les seules lignes PAYE ; avoir calculé et affiché partout.
- **SeanceEditor** : cohérence montant/tarif bloquante (cadenas), confirmation changement de tarif sur séance encaissée, avertissement de dépassement de quota non bloquant.
- **UC_DossierFiche** : grille Paiements groupée par séance repliable, filtres Avoirs / Séances reportées, bouton Détail des paiements (`DialogPaiementsSeance`).
- **Référence** : [Guide séances et paiements](../Guide/Guide_Seances_Paiements.md) (le relevé des conséquences d'origine a été repris dans ce guide le 17/10/2026).
- **Validation** : build réussi ; recette manuelle à exécuter (plans de tests mis à jour).

### Lots Domaines, tarification, quotas et grilles métier ✓ (19–20/09/2026)
- **Domaines et tarification** : comportements de domaine séparés (`tarif_convention`, `no_show_facturable`, `soumis_quota`), liaisons N-N domaine–tarif/nature et périodes de validité ; aucun code métier pivot pour ces règles.
- **Quotas INAMI** : référentiel daté par domaine, plafonds séparés soutien/traitement, compteur glissant et alerte métier *(modèle remplacé le 01/10/2026 par les catégories de quota, ADR-27)*.
- **Patients** : domaine dominant calculé, filtre domaine, recherche enrichie et grille compactée avec en-têtes multi-lignes.
- **Paiements** : séance affichée avec sa nature, reste dû calculé, paiements partiels, sélection d’une séance non soldée et distinction lisible Prévision / Soldé / Annulé.
- **Validation** : build complet réussi après les évolutions.

### Lot 2 — Séances : onglet Séances du dossier ✓ (14/09/2026)
- **`GestionSeances` / `QuerySeances`** : couche métier + requêtes complètes —
  - liste par dossier (avec montant réglé agrégé et reste dû calculé),
  - lecture,
  - création (statut initial **Planifiée**, code généré par la base),
  - mise à jour, notes/évolution RTF, changement de statut, **suppression**,
  - point d'entrée `AppliquerDateHeureSeance` (anticipation Google Agenda)
- **`SeanceEditor`** *(nouvelle modale)* : édition d'une séance (identité + notes/évolution RTF via `UC_RichTextEditorSimple`), défauts en création, mode lecture seule si dossier archivé
- **`UC_DossierFiche`** : onglet **Séances** opérationnel (liste `dgvSeances`, boutons Nouvelle/Modifier/Supprimer, double-clic, lecture seule si archivé) — remplace le placeholder « à venir »
- **À revoir (15/09)** : éditeur des notes de séance — évaluer le grand `UC_RichTextEditor` (décision en attente, question à Pearl)
- **Build complet validé** sans erreur

### Lot 1 — Dossiers : cycle de vie complet ✓ (clôturé 21/07/2026)
- **`GestionDossiers` / `QueryDossiers`** : création (statut forcé Actif, code généré par la base), mise à jour d'identité, lecture/écriture ciblée des **7 zones cliniques** (garde-fou lecture seule sur `HistoriqueArchive`), transitions de statut validées par **whitelist** (réouverture depuis Clôturé, archivage terminal — D-Q5/D-Q10)
- **`UC_DossierFiche`** : hub 3 sous-onglets (Suivi clinique / Documents / Séances), modes Consultation/Création/Modification, persistance câblée, validation `ErrorProvider` ; modales `DossierZoneEditor` (zone) et `DossierStatutEditor` (statut)
- **Documentation** : rétro-CHANGELOG du module Dossiers, alignement des commentaires de code périmés (mentions « brique B2 » supprimées), clôture du Lot 1 dans les docs de pilotage
- **Build complet validé** sans erreur

### Lot 1 — Patients : anamnèse, photo, export contextuel ✓ (13–14/06/2026)
- **Onglet Anamnèse** dans `UC_PatientFiche` : `UC_RichTextEditor` complet, sauvegarde double format RTF+TXT, export PDF/Word contextuel horodaté + ouverture automatique (ADR-21)
- **Upload photo d'identité** : bouton `btnUploadPhoto`, filtre formats GDI+ (jpg/jpeg/png/gif/bmp), nom fixe `Identite.ext`, `AssurerDossierPatient`, MAJ `patients.photo_fichier` (ADR-20)
- **Délégation export** : `ExportRequested` / `ExportCompleted` dans `UC_RichTextEditor` — contrôle générique, logique patient dans `UC_PatientFiche` (nommage horodaté, ouverture auto)
- **`CheminsPatientHelper` refactoré** : `code_patient` formaté (PA000003), dossier sous `PATH_GENERAL\PATH_DOCUMENT\{code}`, méthodes `AssurerDossierPatient` et `GetNomFichierPhotoIdentite`
- **Migrations DB** v2.1 (contacts/rôles légaux), v2.2 (`photo_fichier`), v2.3 (`anamnese_rtf/txt`) appliquées
- **Points d'ancrage TODO** posés dans le code pour la future traçabilité `documents` (export anamnèse + upload photo)
- **Build complet validé** sans erreur
- **Documentation mise à jour** : README, CHANGELOG, ARCHITECTURE_DECISIONS (ADR-20/21), Rules (§23/24), ToDo

### Lot 1 — Patients : fiche de base ✓ (12–13/06/2026)
- **`UC_PatientHome`** : recherche multi-critères + liste rapide + bouton Nouveau
- **`UC_PatientFiche`** : bandeau identité + onglets Identité / Anamnèse / Famille-Contacts / Intervenants / Dossiers ; modes Consultation/Création/Modification + activation progressive
- **Alerte patient** RTF via `UC_RichTextEditorSimple` (bandeau)
- **Validations** : NISS, anti-doublon nom+prénom+naissance
- **Migrations DB** v2.1 contacts/famille avec rôles légaux

### Lot 0 : socle métier terminé ✓ (09–10/06/2026)
- **9 référentiels implémentés** (pile complète Query + Modèle + Gestion + UC)
- **`UC_ReferentielBase`** : classe de base héritable avec hooks champ supplémentaire
- **`UC_ReferentielHome`** : hub 9 tuiles, droits SuperUser/Admin, navigation activée
- **`UC_RichTextEditorSimple`** : variante allégée (7 boutons), double format RTF+TXT
- **Build complet validé**

### Cadrage de la phase métier ✓ (08/06/2026)
- **Plan de conception métier** rédigé et validé : `Plan_Conception_Metier_Althea.md`
- **Décisions actées** (D-13 à D-19) : documents hors DB, Google Docs/Calendar piliers, renommages, réseau N-N, séance dès planification, UC par référentiel, multi-utilisateur anticipé
- **ADR mis à jour** : ARCHITECTURE_DECISIONS.md (ADR-13 à ADR-19) et Rules.md

### Gestion utilisateurs complète ✓
- **UC_Utilisateurs** : liste avec recherche/filtres avancés, actions admin complètes
- **UtilisateurEdition** : Form modale avec 3 modes, génération MDP temporaire, reset, déverrouillage
- **GestionUtilisateurs** : couche métier complète (CRUD, activation/désactivation, gestion séquences MariaDB)

### UI/UX cohérente ✓
- **DialogChoix** : remplacement complet de MessageBox, icônes animées GIF, thématisation UITheme
- **UtilsIcons** : centralisation des icônes d'état avec priorité (verrouillé > actif > inactif)
- **UtilsButtons** : gestion cohérente des styles boutons dans toute l'application



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

