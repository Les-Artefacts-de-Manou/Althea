# 📌 Althéa — ToDo opérationnel (backlog V1)

> Backlog actionnable, aligné sur l'état réel du dépôt.
> Plan directeur de la phase métier : `../Conception/Plan_Conception_Metier_Althea.md` (décisions actées D-13 à D-19).
> Référence d'avancement détaillée : `Checklist_projet_V1.md`.
*Dernière mise à jour : 25/10/2026*

---

## Légende

- [x] Terminé
- [~] En cours / partiel
- [ ] À faire

---

## A. ~~Priorité immédiate (stabilisation fonctionnelle)~~ ✅ **TERMINÉ**

- [x] Finaliser module **Utilisateurs**
  - [x] Implémenter `UC_Utilisateurs` (liste + détail + actions admin)
  - [x] Implémenter `UtilisateurEdition` (création/modification/consultation)
  - [x] Appliquer contrôle d'accès complet selon rôles (Admin/SuperUser)
  - [x] Journaliser actions sensibles (création, activation, réinitialisation, déverrouillage)
  - [x] Gérer mots de passe temporaires sécurisés
  - [x] Implémenter recherche et filtres avancés
  - [x] Afficher icônes d'état (actif/inactif/verrouillé) avec priorités

- [x] Fiabiliser parcours administration
  - [x] Relecture complète des flux Login → Home → Admin
  - [x] Vérifier cohérence messages statut / erreurs UI
  - [x] Vérifier garde-fous élévation de droits
  - [x] Remplacer tous les MessageBox par DialogChoix
  - [x] Centraliser icônes d'état via UtilsIcons

- [x] Documentation administration complète
  - [x] Mise à jour CHANGELOG.md (chronologie inversée)
  - [x] Mise à jour README.md (structure professionnelle)
  - [x] Mise à jour Rules.md (règles DialogChoix, UtilsIcons, modes utilisateur)
  - [x] Mise à jour Process_Althea.md (Processus 06 et 07 + diagrammes Mermaid)
  - [x] Mise à jour ARCHITECTURE_DECISIONS.md (ADR-10, ADR-11, ADR-12)
  - [x] Mise à jour Documentation_technique_UI_Althea.md (DialogChoix, UtilisateurEdition, UC_Utilisateurs)

---

## B. ~~Lot 0 — Socle métier (migration de schéma)~~ ✅ **TERMINÉ**

> Réalisé et versionné. Prérequis bloquant des lots suivants : levé.
> Référence : décisions D-16 à D-18 du plan de conception.

### B0a. Migration de schéma ✅

- [x] Versionner et appliquer le renommage `volets` → `domaines`
- [x] Versionner et appliquer le renommage `medecins` → `therapeutes`
- [x] Créer la table de liaison N-N `autres_suivis_patient` (patient ↔ thérapeute externe)
- [x] Créer le référentiel `ref_roles_intervenant`
- [x] Créer/aligner le référentiel `ref_statuts_seance` (cycle de vie séance)
- [x] Mettre à jour les scripts SQL versionnés et la documentation `Database`

### B0b. Briques UI transverses ✅

- [x] Créer la base commune `UC_ReferentielBase` (UserControl générique par référentiel) avec hooks champ supplémentaire
- [x] Créer `UC_ReferentielHome` (hub des référentiels, droits SuperUser/Admin, navigation)
- [x] Décliner les référentiels généraux initiaux :
  - [x] `UC_Domaines` (ref_domaines, code max 10)
  - [x] `UC_LiensPatient` (ref_liens_patient, code max 50)
  - [x] `UC_RolesIntervenant` (ref_roles_intervenant, code max 30)
  - [x] `UC_SituationsFamiliales` (ref_situations_familiales, code max 50)
  - [x] `UC_StatutsDossier` (ref_statuts_dossier, code max 30)
  - [x] `UC_StatutsSeance` (ref_statuts_seance, code max 30)
  - [x] `UC_TypesDocuments` (ref_types_documents, code max 30)
  - [x] `UC_TypesRendezVous` (ref_types_rendez_vous, code max 30)
  - [x] Ancien `UC_TypesSeance` remplacé par le paramétrage des natures, tarifs, liaisons de domaine et quotas (19/09)
- [x] Créer le composant riche `UC_RichTextEditor` (30 boutons, impression Win32, exports PDF/Word Syncfusion)
- [x] Créer la variante compacte `UC_RichTextEditorSimple` (7 boutons, double format RTF+TXT, contexte optionnel)

---

## C. Cœur métier V1

### C0. Briques transverses (pré-requis cœur métier)

- [ ] Ajouter une **mini-pile de navigation** (Push/Pop + conservation du dernier filtre) dans `NavigationManager` (D-Q15)
- [ ] Créer un **hôte modal générique de référentiel** réutilisant les `UC_Ref<X>` existants, déclenché par bouton `[+]` près des combos, avec contrôle de droit + élévation (D-Q17)
- [x] **Helper de chemins déterministes** `CheminsPatientHelper` (refactoré : `code_patient` PA000003, `PATH_GENERAL\PATH_DOCUMENT\{code}`, `AssurerDossierPatient`, `GetNomFichierPhotoIdentite`) (D-Q13 / ADR-20)
- [x] **Migrations DB préalables** : `patients.alerte_rtf/alerte_txt` ✅, `patients.photo_fichier` ✅, `patients.anamnese_rtf/anamnese_txt` ✅ (v2.3), `tec_parametres PATH_GENERAL/PATH_DOCUMENT` ✅

### C1. Patients

- [x] Créer module métier gestion patients (`GestionPatients.vb`) + `UpdatePhotoPatient`
- [x] Créer requêtes SQL patients (`QueryPatients.vb`) — incl. `anamnese_rtf/txt`
- [x] Créer UI `UC_PatientHome` (recherche multi-critères + liste rapide + bouton Nouveau)
- [x] Créer UI fiche patient `UC_PatientFiche` — bandeau identité + onglets **Identité / Anamnèse / Famille-Contacts / Intervenants / Dossiers** ; modes Consultation/Création/Modification + activation progressive (D-Q11, D-Q14)
- [x] Implémenter l'alerte patient RTF (`alerte_rtf/txt` via `UC_RichTextEditorSimple`, bandeau) (D-Q12)
- [x] **Onglet Anamnèse** : `UC_RichTextEditor` complet, sauvegarde double format, export PDF/Word contextuel horodaté + ouverture auto (ADR-21)
- [x] **Upload photo d'identité** : `btnUploadPhoto`, filtre formats GDI+, nom fixe `Identite.ext`, `AssurerDossierPatient`, MAJ `patients.photo_fichier` (D-Q13 / ADR-20)
- [x] Implémenter validations et règles de saisie (NISS, anti-doublon nom+prénom+naissance)
- [x] Implémenter le réseau d'intervenants externes (N-N via `autres_suivis_patient` + `ref_roles_intervenant`, rôle *Adresseur* inclus) (D-Q16) — **onglet à venir**
- [x] **Suivi dérivé du dernier dossier (5 états) + activation manuelle `is_actif`** : migration v2.8.0 (drop `suivi_en_cours`, add `is_actif`/dates), calcul à la volée, pastille 5 états + colonne « Actif » dans la liste, filtres orthogonaux, bandeau/contexte succincts, bascule Activer/Désactiver
- [x] **Grille Patients compacte et contextualisée** : domaine dominant calculé, filtre par domaine, recherche enrichie, en-têtes multi-lignes et identité mise en évidence (20/09)
- [ ] Implémenter recherche rapide depuis accueil
- [ ] **Grille Patients = hub de la journée** (maquette « Patients — proposition » et décisions de Joëlle du 17/10/2026) : la grille montre la situation du patient (prochain rendez-vous, séance du jour, dernière séance, suivi, paiements) plutôt que sa fiche d'identité.
  - Décidé :
    - pastilles texte au lieu des icônes ;
    - NISS, email et photo retirés de l'affichage (gardés dans la recherche) ;
    - patients inactifs masqués par défaut (« Afficher les inactifs », ligne grisée) ;
    - tri par défaut sur le prochain rendez-vous en trois groupes (Aujourd'hui / À venir / Sans rendez-vous à venir) ;
    - « En pause » en ocre (jaune modéré), partout — la pastille du dossier quitte l'orange des absences ;
    - clôturé / archivé en gris pâle dans la liste (gris foncé seulement dans l'en-tête d'un dossier ouvert).
  - [x] Étape 1 — Données ✅ 17/10/2026 (validé par Joëlle) :
    - `SelectPatientsListe` V2.4.0 —
      - prochain rendez-vous (première séance due pas encore passée),
      - dernière séance où le patient a été **vu** (une absence non excusée n'en est pas une : décision de Joëlle, elle a sa colonne),
      - séances du jour (passées ou non),
      - dossiers au statut Actif ;
    - `PatientListeItem` V1.1.0 (`ASeanceAujourdhui`, `ProchaineSeanceEstAujourdhui`, `EstSuiviSansRendezVous`, `Age`) ;
    - requête vérifiée sur une base de test fictive (schéma seul)
  - [x] Étape 2 — Grille ✅ 17/10/2026 (validée par Joëlle ; PAT-50 à PAT-59) :
    - colonnes Code, Patient, Âge, Suivi (pastille + domaine, « +1 »), Prochain RDV (« Aujourd'hui 14:00 » plein, à venir pâle, « Aucun RDV » pour un suivi actif), Dernière séance, Paiements, Abs., Alerte (icône + texte en infobulle), Téléphone ;
    - groupes, ligne du jour, inactifs
  - [x] Étape 2b — Raccourci de prise de notes ✅ 17/10/2026 (demande et validation de Joëlle) :
    - double-clic sur « Prochain RDV » (séance du jour, à venir, ou du jour déjà passée) ou « Dernière séance » = ouvrir la séance dans `SeanceEditor` ; ailleurs, la fiche patient.
    - À revoir avec la gestion documentaire (D1) si la prise de notes y évolue.
    - PAT-60 à PAT-62
  - [x] Étape 3 — Résumé cliquable ✅ 17/10/2026 (validée par Joëlle ; PAT-63 à PAT-70) :
    - aujourd'hui, sous 7 jours, suivis sans rendez-vous, à régler, avoir, absences, alertes — `ResumeFiltre` sorti de `UC_DossierFiche` dans `UI/Controls/Communs` ;
    - filtre « Dossier » limité au statut ;
    - case « Afficher les inactifs » (inactifs masqués par défaut, signalés dans la barre d'état) ;
    - liste « Dossier » sans « Dossier : » devant les libellés, texte sur une ligne
  - [x] Étape 4 — Icônes d'état redessinées ✅ 17/10/2026 (validée par Joëlle ; PAT-71) :
    - `UtilsIcons` V1.1.0 dessine les icônes de statut de dossier (style `AltheaIconeEtat`, couleurs de la pastille de statut) ;
    - `IconPatientActif`, `IconPatientEnCours`, `IconPatientNonEnCours` retirés ;
    - charte et documentation à jour.
    - PNG `actif_20x20`, `pause_20x20`, `cloture_20x20`, `archive_20x20`, `desactive_20x20`, `dossier_manquant_20x20`, `patientEncours_20`, `patientNonEncours_20` devenus inutiles → UI-8 ;
    - polices de la grille rétablies (Calibri 10, en-têtes 10 gras)
  - [ ] Étape 5 — **Panneau d'aperçu** à droite de la grille (accord de Joëlle, 17/10/2026 ; rejoint la demande du cahier des charges de Pearl : voir l'essentiel sans ouvrir les fiches) :
    - identité, alerte complète, dossiers, prochaines et dernières séances, finances, contacts ;
    - documents récents avec aperçu PDF quand la gestion documentaire (D1) sera là.
    - Maquette à faire avant l'étape
    - [x] Maquette (planche « Panneau d'aperçu », 17/10/2026) — validée par Joëlle : notes jamais affichées (« pour lire, on ouvre la séance ») ; panneau ouvert par défaut, à revoir à l'usage
    - [x] 5a — Panneau ✅ 17/10/2026 (validé par Joëlle ; PAT-72 à PAT-79) : `UC_ApercuPatient`, `GestionApercuPatient` / `ApercuPatient` (modules existants, aucune requête propre), `GestionDossiers.ChoisirDossierDominant` ; ouverture fiche / dossier / séance ; colonnes reprises masquées
    - [ ] 5b — Selon l'usage : mémoriser panneau ouvert / fermé ; « Encaisser » depuis l'aperçu (aujourd'hui : par le dossier) ; documents récents avec la gestion documentaire (D1)

### C1bis. Thérapeutes (référentiel riche — hub Référentiels)

> Brique **Gestion des thérapeutes** livrée (modèle `Therapeute` + `QueryTherapeutes` + `GestionTherapeutes` + `UC_Therapeutes` + `TherapeuteEdition` + tuile hub). Source du futur combo des Intervenants.
> Points d'ancrage `TODO` posés dans le code : `Therapeute.vb`, `TherapeuteEdition.vb`, `QueryTherapeutes.vb`.

- [x] Brique Gestion des thérapeutes (liste + modale, soft-delete, contrôle d'usage, tuile hub Référentiels)
- [x] **Commentaire thérapeute en RichTextEditorSimple** : transformer le commentaire (TextBox) en `UC_RichTextEditorSimple`. Implique d'ajouter un champ `.rtf` (`commentaire_rtf`, + texte brut pour la recherche) dans la table `therapeutes` et d'adapter les queries de gestion (`QueryTherapeutes` INSERT/UPDATE/SELECT) ainsi que `TherapeuteEdition`.
- [x] **Spécialité en table** (23/10/2026, v2.55.0, décisions de Joëlle : une seule spécialité par thérapeute ; liste de départ + reprise des valeurs saisies) : référentiel `ref_specialites` (`UC_Specialites`, groupe Cabinet du hub), liste + bouton `[+]` dans `TherapeuteEdition`.
- [ ] **Icônes « Spécialités »** (tuile du hub, 48 px normal / hover / disabled, et icône de titre) : provisoirement celles des thérapeutes et des rôles d'intervenant — à dessiner.
- [ ] **Retirer la colonne texte `therapeutes.specialite`** (migration) une fois la reprise v2.55.0 vérifiée ; idem `patients.lateralite` (v2.46.0).

### C2. Dossiers

> 🟢 **Conception actée le 21/07/2026** — voir `Docs/Conception/Plan_Conception_Dossiers_Althea.md` (arbitrages D-DOS-1 à D-DOS-5, architecture cible, plan incrémental B0→B8). Implémentation démarrée.
> **Décisions clés** :
>
> - dossier sorti de la fiche patient dans un hub `UC_DossierFiche` (sous-onglets Suivi clinique / Documents / Séances), ouvert depuis une grille dans l'onglet Dossiers de `UC_PatientFiche` (pattern `UC_PatientHome → UC_PatientFiche` répliqué) ;
> - onglet Suivi clinique = 7 boutons → modale unique réutilisable `DossierZoneEditor` (enum `ZoneDossier`) ;
> - chemins via `CheminsDossierHelper` ;
> - exports contextuels PDF/Word par zone ;
> - `historique_archive` visible en lecture seule à l'archivage uniquement.
>
> Aucune modification de schéma (table `dossiers`, `ref_domaines`, `ref_statuts_dossier` déjà existants) : lot purement applicatif.

- [x] **B0** — Fondations données : `Dossier.vb`, `DossierListeItem.vb`, `QueryDossiers.vb`, `GestionDossiers.vb` (lecture : `GetDossiersParPatient`, `GetDossier`) — validé sur base réelle (`DO000001`)
- [x] **B1** — `CheminsDossierHelper.vb` : `{code_patient}/{code_dossier}/`, `AssurerDossierDuDossier`, `FormaterCodeDossier`
- [x] **B2** — Grille dossiers dans l'onglet + navigation : `Home.NavigateToDossierFiche` + `UC_DossierFiche` coquille (bandeau + 3 sous-onglets) ; grille `dgvDossiers` + boutons Nouveau/Ouvrir + double-clic + retour restauré sur l'onglet Dossiers (tests 1→7 OK)
- [x] **B3** — Identité du dossier : création/modif (domaine, statut initial, date ouverture, référent, modalités) ; `InsertDossier` / `UpdateDossier`
- [x] **B4** — Suivi clinique : enum `ZoneDossier`, modale `DossierZoneEditor`, `UpdateZoneDossier` / `GetZoneDossier` ciblés (7 zones : motif_suivi, historique, antecedents, diagnostic, traitements_en_cours, objectifs_suivi, historique_archive)
- [x] **B5** — Exports contextuels PDF/Word par zone vers `CheminsDossierHelper` (pattern `ExportRequested` / `ExportCompleted`)
- [x] **B6** — Cycle de vie statut (actif/pause/clôturé/archivé) : transitions sécurisées, `date_cloture` à la clôture, réouverture même domaine (D-Q5), transfert inter-domaines (D-Q10), archivage lecture seule ; tracé `GestionLog` + `DialogChoix`
  - [x] Whitelist des transitions + `ChangerStatutDossier` (métier)
  - [x] Modale `DossierStatutEditor` (statut affiché + bouton unique)
  - [x] Verrouillage lecture seule intégral des zones en `Archivé`
  - [x] Message d'archivage définitif (Clôturé → Archivé)
  - [x] Clôture contrôlée (décision de Joëlle du 17/10/2026) :
    - refusée tant que des séances dues sont à venir, payées d'avance ou non (`GestionDossiers.ControlerCloture`, contrôle refait dans `ChangerStatutDossier`) ;
    - montants à régler signalés dans la confirmation (« Clôturer quand même ») ;
    - pastille de statut colorée et bandeau « consultation » d'un dossier clôturé ou archivé
  - [x] Codes de statut de dossier repris de `GestionDossiers.CODE_STATUT_*` partout dans le code VB ✅ 17/10/2026 :
    - `Dossier`, `DossierListeItem`, `UC_DossierFiche`, `DossierStatutEditor`, `UC_PatientHome` (filtre, icônes, filtrage), `UtilsIcons.IconStatutDossierDominant` ;
    - les requêtes SQL de `QueryPatients` gardent les codes en clair (commentaire de renvoi)
  - [x] Liste patients : colonne « Dossiers » (nombre) + pastille état dominant
  - [x] `suivi_en_cours` nullable dérivé des dossiers (recalcul auto à chaque transition)
- [ ] **B7** — Recherche et filtrage par statut/date/domaine (si le volume le justifie)
- [ ] **B8** — Documentation (`CHANGELOG.md`, `README.md`, docs UI/Process, ADR si nouveau pattern)

### C3. Séances, tarification et quotas

- [x] **Statuts de séance : drapeaux plutôt que codes** (relevé avec l'audit B9, 20/10/2026) — fait le 24/10/2026 (audit C.3 étape 2, v2.57.0, ADR-54) : cases « non tenue, libère le créneau » et « proposé à la création » ; codes devenus inutiles retirés de `CodesSysteme`. Historique :
  - **no-show fait le 22/10/2026** (`est_absence`, v2.53.0, audit B.6) ;
  - restent « libère le créneau » (`CodesStatutLiberantCreneau`) et « non tenu » (`GestionSeances.EstStatutNonTenu`, 22/10/2026 : annulée, maladie, reportée reconnues par leur code) :
  - le code reconnaît encore quelques statuts par leur code — désormais en constantes uniques (`GestionSeances.CODE_STATUT_*`) — pour « libère le créneau » (annulations, report), « no-show » (`ABSENTE`), « annulé ou non tenu ».
  - Piste : des drapeaux de `ref_statuts_seance` (comme `est_du`, `consomme_quota`), puis retirer aussi les `'ABSENTE'` des requêtes de `QueryPatients`.
  - Migration à prévoir ; pas urgent tant que ces statuts sont des lignes système.
- [x] Créer module métier gestion séances (`GestionSeances.vb`)
- [x] Créer requêtes SQL séances (`QuerySeances.vb`)
- [x] Créer UI gestion séances (`SeanceEditor` + onglet Séances de `UC_DossierFiche` : liste `dgvSeances`, CRUD)
- [~] Créer automatiquement la séance depuis l’agenda lié (statut via `ref_statuts_seance`) — la saisie manuelle est opérationnelle ; l’intégration Agenda reste à faire
- [ ] Ajouter historique lisible + tri/filtrage
- [x] Implémenter zone notes/synthèse par séance (`UC_RichTextEditorSimple`) — ⏳ à revoir : évaluer le grand `UC_RichTextEditor` (question à Pearl)
- [x] Lier séances aux dossiers
- [ ] Implémenter saisie rapide de séance
- [x] Résoudre le tarif à la date de séance selon statut, domaine, no-show et tarif patient du dossier
- [x] Contrôler le non-chevauchement des créneaux et fournir le planning journalier de consultation
- [x] Remplacer les anciens types de séance par les natures `ref_natures_seance`, rattachées aux domaines
- [x] Mettre en place les quotas INAMI paramétrables par domaine et période de validité
- [x] Refonte Domaine = convention (30/09–01/10, schéma 2.21.0 → 2.28.0, ADR-27) : organismes, catégories de quota par domaine, plafonds paramétrables, catégorie portée par la liaison nature–domaine, statuts consommateurs paramétrés, écran `UC_DomainesHome`
- [x] Protection des lignes système des référentiels `est_systeme` (ADR-28)
- [x] Suppression des colonnes héritées de l'ancien modèle quota (lots 25-26, v2.28.0)
- [x] Retirer `UC_Domaines` (accès Référentiels) : archivé dans `UI/Forms/Test`, exclu de la compilation ; tuile Référentiels redirigée vers `UC_DomainesHome` (02/10/2026)
- [ ] Renommer les classes techniques `GestionQuotaInami` / `QuotaInamiRegle` / `QuotaInamiInfo` (noms conservés pour l'instant)
- [x] Avertissement visible (non bloquant) de dépassement de quota à la création de séance (26/09)
- [x] Contrôle bloquant de cohérence montant / tarif à l’enregistrement, message adapté au rôle ; montant modifiable uniquement via le cadenas (26/09)
- [x] Confirmation avec conséquence (avoir / reste dû) lors d’un changement de tarif sur séance encaissée (26/09)
- [x] Report : seule une séance Planifiée, recopie du tarif et du montant d’origine (tarif lié au patient, pas à la date) (26/09)

### C4. Paiements

- [x] Créer le module métier paiements (`GestionPaiements.vb`) et les requêtes centralisées (`QueryPaiements.vb`)
- [x] Créer l’UI de gestion dans l’onglet Paiements de `UC_DossierFiche` et `PaiementEditor`
- [x] Lier paiements ↔ séances par `lia_paiements_seances` ; paiements partiels pris en charge
- [x] Calculer le suivi financier : montant dû patient, reste dû de séance et paiements en attente
- [x] Gérer les modes de paiement depuis leur référentiel
- [x] Afficher la date et la nature de séance dans la grille ; distinguer Prévision, Soldé, Annulé et reste partiel
- [x] Proposer les séances non soldées lors de la création libre d’un paiement
- [x] Règle unique `ResynchroniserDuSeance` : dû en attente = tarif − encaissés PAYE (25–26/09)
- [x] Annulation d’un encaissement (jamais de suppression) : SuperUser/Admin, motif obligatoire (`DialogMotifAnnulation`), trace auteur/date/motif (26/09)
- [x] Calculs réglé / reste dû / avoir limités aux lignes PAYE (26/09)
- [x] Avoir calculé et affiché : en-tête dossier, grilles séances, paiements et patients (26/09)
- [x] Grille Paiements groupée par séance, repliable, tri désactivé ; filtres Avoirs et Séances reportées (26/09) — *remplacée le 17/10/2026 par le relevé chronologique*
- [x] Bouton « Détail des paiements » + `DialogPaiementsSeance` (26/09)
- [x] Réutilisation d’un avoir lors d’un futur encaissement (v2.20.0 : proposition avec confirmation, avoir global patient, consommation partielle, mode « Avoir » `est_avoir`)
- [x] Report fiabilisé : si la création de la séance de remplacement échoue, l’origine est remise en Planifiée (compensation journalisée) (26/09) — remplacé le 21/10/2026 par un report d'un seul tenant (audit B.2)
- [ ] Prévoir les exports de suivi financier V1 (liste des paiements, balance)
- [x] **Revoir la grille des Paiements et son fonctionnement** ✅ 17/10/2026 — relevé chronologique (maquette A) : pastille « État », colonne « Pour », résumé cliquable, filtre « À régler », tri rétabli
  - (demande de Joëlle, 14/10/2026) : la disposition actuelle (paiements regroupés par séance, lignes repliables — compromis trouvé parce que la grille standard ne sait pas regrouper) n'est pas claire ;
  - on ne comprend pas au premier coup d'œil, pour chaque séance, ce qui est encaissé, en attente, dû ou en avoir ;
  - les couleurs de la charte ne sont pas respectées.
  - Piste : une **grille améliorée du kit** (regroupement, totaux par groupe, états lisibles, couleurs de la charte), utile ici et probablement dans d'autres écrans.
  - Maquette d'abord, puis décision sur le fonctionnement (et peut-être du processus de paiement).
  - **Tests approfondis des paiements à faire à ce moment-là** (plan `PLAN_TESTS_PAIEMENTS.md`, décision du 15/10/2026).
- [x] **Revoir aussi la grille des Séances, avec celle des Paiements** (demande de Joëlle, 16/10/2026 ; **maquettes du 17/10 retenues** : grille des Séances telle que proposée, Paiements en **relevé chronologique** (A) — Séances ✅ 17/10/2026, Paiements ✅ 17/10/2026, mêmes briques : `UtilsPastilles`, `DessinerPastilleCellule`, `ResumeFiltre`) :
  - en arrivant sur l'onglet, on doit voir sans filtrer quelles séances sont **à venir ou passées**, **dues ou non** (planifiée, réalisée, reportée, annulée, maladie, absence non excusée), et ce qui reste à régler — aujourd'hui seules quelques cellules de montants sont colorées et le statut est un simple texte.
  - Pistes proposées le 16/10 (à départager sur maquette, avec la grille des Paiements, dans la même grille améliorée du kit) :
    - séparation « À venir » / « Passées » (ligne « Aujourd'hui ») ;
    - statut en **pastille** colorée par famille, déduite des propriétés du statut (dû, quota, passée / à venir, lignes système) et non de couleurs écrites en dur ;
    - une seule colonne « Paiement » en pastille (Réglée, À régler + montant, Prévue, Non due) ;
    - lignes non dues en texte atténué ;
    - petit **résumé cliquable** au-dessus de la grille (« 3 à venir · 12 réalisées · 2 à régler — 120 € ») qui sert aussi de filtre rapide.
- [x] **Tests approfondis des paiements** ✅ 17/10/2026 (`PLAN_TESTS_PAIEMENTS.md` V1.8, P-01 à P-59 ; décision du 15/10/2026) — parcours recommandé exécuté par Joëlle, corrections intégrées au fil des étapes ; reste **P-15** (erreur base pendant un enregistrement, poste de développement uniquement), facultatif.
- [ ] **Nommer l'élément par son code dans les autres fenêtres** (demande de Joëlle du 17/10/2026, règle Reference_UI § 5.6 ; fait pour les fenêtres du dossier : séance, report, paiement, annulation, réaffectation, détail des paiements, statut du dossier) :
  - passer les autres fenêtres ouvertes sur un élément existant (patient, contacts, intervenants, thérapeutes, utilisateurs, référentiels…) et ajouter le code au sous-titre de l'en-tête là où il manque.
- [x] **« Montant dû » : une seule règle partout** ✅ 17/10/2026 (relevé et accord de Joëlle) :
  - bandeau du dossier (`GestionSeances.CalculerSoldesPatient`, l'ancien `CalculerMontantsDus` à codes `REALISEE` / `ABSENTE` est retiré), grille Patients (`montant_du` : heure de fin comprise) et grilles suivent la règle de `SeanceListeItem.MontantExigible` (séance due et passée).
  - Décision A du même jour : le bandeau montre les montants dus du dossier + le total du patient, et l'avoir **du patient** (tous dossiers) avec la part du dossier.
  - ~~Reste : l'absence non facturable est encore reconnue par le code `ABSENTE` écrit en dur~~ ✅ 22/10/2026 : case `ref_statuts_seance.est_absence` (audit B.6, v2.53.0).
- [x] **Trop-perçu sur une séance encore due** ✅ 17/10/2026 (décision de Joëlle : option A) :
  - la part encaissée au-delà du montant d'une séance due est un avoir partout (bandeau, grilles, grille Patients, réutilisation) ;
  - réutilisé, il diminue la dernière affectation encaissée de la séance, plafonnée au trop-perçu.
  - Guide séances et paiements, § 9.3.

---

## D. Documents et agenda (à intégrer depuis POC)

### D1. Documents

- [ ] **Hub Documents ?** (remarque de Joëlle, 20/10/2026) :
  - au démarrage de la gestion documentaire, voir si les tuiles qui concernent les documents (types de documents, textes types, usages des textes…) quittent le hub Référentiels pour un hub Documents, si on en crée un.
  - Même principe que la tuile Domaines, retirée du hub Référentiels le 20/10/2026 (accès par le menu principal de la Home) et que les situations juridiques (ADR-31) : la place d'une tuile suit l'usage.

> **Règle fondamentale (à implémenter avec la table `documents`)** : tout document **créé OU uploadé** dans
> l'application doit avoir une **trace dans `documents`**, couplée à un **modèle de document** et à un **niveau**.
>
> **Niveau de document** (décision à trancher au démarrage de la brique) : `patient` (ex. anamnèse, photo
> d'identité), `dossier`, `seance`, etc. Deux options envisagées :
> - **Enum publique** dans le code (simple, typé), ou
> - **Type de paramètre dans `tec_parametres`** (plus cohérent avec le reste de l'appli, valeurs configurables).
>
> **Points d'ancrage déjà posés dans le code** (commentaires `TODO`, à compléter à ce moment) :
> - `UC_PatientFiche.rteAnamnese_ExportCompleted` — trace de l'export anamnèse (PDF/Word), niveau = patient. ✅ **Fait**
> - `UC_PatientFiche.btnUploadPhoto_Click` — trace de la photo d'identité, niveau = patient. ✅ **Fait (13/07/2026, type `PHOTO_IDENTITE`, upsert patient+type)**
>
> ✅ **Les deux points d'ancrage sont désormais branchés.**

- [x] **Gérer le niveau de document** — déduit de la hiérarchie (`id_patient/id_dossier/id_seance`, pas de colonne `niveau`), table `documents` corrigée (v2.5.0)
- [x] **Brancher la traçabilité aux points d'ancrage existants** : ✅ export anamnèse + ✅ export zones cliniques + ✅ upload photo d'identité (`PHOTO_IDENTITE`, upsert patient+type, v2.6.0)
- [ ] Implémenter le stockage **hors base de données** (système de fichiers, chemin déterministe calculé)
  - [ ] Flux Word local + synchronisation Google Docs à la sauvegarde
  - [ ] Export PDF
  - [ ] Catégorisation par type (rapport, synthèse, courrier, etc.)
  - [ ] Liaison documents ↔ patients/dossiers (chemin reconstituable)
- [ ] Créer module métier gestion documents (`GestionDocuments.vb`)
- [ ] Intégrer création/liaison documents dans code principal
- [ ] Implémenter recherche documents par patient/dossier/type
- [ ] Prévoir envoi documents par email (ex. synthèse de séance)
- [ ] Historique des envois en lien avec le patient/dossier

#### Points différés (notés en cours de développement, à reprendre avec la table `documents`)

- [ ] **Indicateur « dernier export » dans l'onglet Anamnèse** :
  - afficher la date du dernier export documentaire (PDF/Word) + bouton d'ouverture directe.
  - À alimenter par la table `documents` (et non par un scan du dossier patient), pour rester valable pour tous les types de documents.
  - Distinguer clairement « dernier export » (fichier disque, potentiellement périmé) de la « sauvegarde » du contenu (`patients.anamnese_rtf/txt`).
- [x] **Nettoyage des photos d'identité multiples** ✅ (13/07/2026 ; depuis le 11/10/2026 via `ImagesHelper.CopierSousNomFixe`, appelé par `CheminsPatientHelper.ImporterPhotoIdentite`) :
  - l'ancien `SupprimerAnciennesPhotosIdentite` supprimait toutes les `Identite.*` (basé sur le nom fixe, sans extension) avant chaque nouvel upload dans `UC_PatientFiche.btnUploadPhoto_Click`.
  - Résout le cas extension différente (ex. `identite.png` sur `identite.jpg`).


### D2. Agenda

- [ ] Intégrer Google Calendar comme **pilier** (synchronisation bidirectionnelle, reprise assistée)
  - [ ] Gestion rendez-vous de base (date, heure, patient, type)
  - [ ] Vue calendrier mensuelle/hebdomadaire (Scheduler)
  - [ ] Création rapide de rendez-vous depuis fiche patient
- [ ] Créer module métier gestion agenda (`GestionAgenda.vb`)
- [ ] Implémenter notifications/rappels rendez-vous
- [ ] Lier rendez-vous ↔ séances réalisées (séance créée dès planification)
- [ ] Préparer le modèle multi-utilisateur / multi-agenda (activation différée)

---

## E. Technique & exploitation

- [~] Finaliser scripts SQL versionnés pour modules métier restants (migrations `Docs/Database/Migration/`, schéma v2.55.0)
  - [x] Scripts création/migration tables patients
  - [x] Scripts création/migration tables dossiers
  - [x] Scripts création/migration tables séances
  - [x] Scripts création/migration tables paiements
  - [ ] Scripts création/migration tables documents (métadonnées ; fichiers stockés hors DB)
  - [ ] Scripts création/migration tables agenda
- [x] Mettre en place procédure de sauvegarde DB + fichiers (lot 30 P2, 12/10/2026, ADR-37)
  - [ ] Sauvegarde automatique quotidienne — non faite : un **rappel à l'ouverture** est en place ; automatisme à décider
  - [x] Sauvegarde manuelle à la demande
  - [x] Localisation sécurisée des sauvegardes (archive chiffrée AES-256, dossier refusé dans le dossier de données, alerte « même disque » dans l'État du système)
- [~] Tester procédure de restauration complète (lot 30 P3, 13/10/2026, ADR-38 — tests 7.4, 7.5, 7.15 restants, voir lot 30)
  - [x] Restauration base de données
  - [x] Restauration fichiers locaux
  - [x] Validation intégrité avant restauration (mot de passe, manifeste, même version, droits, espace disque)
- [ ] **Fiche Utilisateur : bouton « Réinitialiser le mot de passe » à aligner sur le métier** (relevé à la revue documentaire du 23/10/2026) :
  - en consultation, `UtilisateurEdition` l'active pour tout rôle (`btnResetPassword.Enabled = True`), alors que `GestionUtilisateurs` exige un Admin pour réinitialiser et déverrouiller (`Autorisation.Exiger(…, AppRole.Admin, …)`) — un SuperUser reçoit donc un refus après coup ;
  - l'en-tête de `UC_Utilisateurs` annonce encore « SuperUser : actions admin ».
  - Vérifier aussi « Déverrouiller ».
  - À traiter avec le lot C.4 (écrans) ou avant, si gênant.
- [ ] Préparer installation guidée (configuration initiale)
  - [ ] Assistant de configuration DB
  - [ ] Création utilisateur admin initial
  - [ ] Import/export paramètres
- [ ] **Journal des modifications — « qui fait quoi »** (décision de Joëlle du 16/10/2026 : lot à part, **avant la mise en service chez Pearl** ; remonté des idées post-V1, section H) :
  - le journal technique ne convient pas (fichiers sur le poste, purgés après `LOG_RETENTION_JOURS`, sans utilisateur ni valeurs).
  - À concevoir une fois (ADR + maquette de l'écran de consultation), puis brancher partout :
    - table en base (piste `tec_journal_modifications`) : date, utilisateur, écran / table, ligne concernée, action (création, modification, activation, désactivation, suppression), champ, ancienne et nouvelle valeur ; sauvegardée avec la base, consultable dans l'Administration (filtres), durée de conservation à décider ;
    - périmètre : référentiels, paramètres, Cabinet (identité, identifiants, qualifications, images), Domaines (règles, catégories et plafonds, liaisons), utilisateurs et droits ;
    - confidentialité : valeurs enregistrées pour ces données d'administration, sauf champs exclus (mots de passe et secrets ; IBAN à trancher) ;
    - dossiers patients (consultations, modifications) : trace recommandée par le RGPD **sans recopier le contenu** — périmètre à décider dans le même lot.
- [ ] **Journaux techniques — révision complète** (demande de Joëlle du 16/10/2026, à faire avec le lot précédent) :
  - passer **tous** les écrans et modules en revue pour vérifier que chaque opération importante est journalisée (lancement, réussite, échec avec la cause), au bon niveau et dans la bonne catégorie, sans donnée personnelle ni valeur sensible (ADR-39), et que les messages sont exploitables dans l'écran Journaux.
  - Manques déjà constatés le 16/10/2026 :
    - `UC_DomainesHome` : seules les erreurs sont journalisées (aucune trace des règles, catégories, plafonds et liaisons enregistrés) ; `CategoriesQuotaEditor` : aucun journal ;
    - `UC_Cabinet` : une seule ligne « Informations du cabinet enregistrées » (rien pour les identifiants, qualifications, images, cachet) ;
    - référentiels et paramètres : action et identifiant seulement, ce qui est voulu dans le journal technique — le détail (qui, ancienne et nouvelle valeur) relèvera du journal des modifications.

---

## F. Qualité & documentation

- [x] Base documentaire technique existante (README/Rules/Process/Database)
- [x] Consolidation des documents de pilotage (`Checklist`, `ToDo`, `Planning`)
- [x] Documentation UI complète (`Documentation_technique_UI_Althea.md`)
- [x] Revue complète de la documentation au 23/10/2026 (état du projet, checklist, planning, architecture, base, concepts, sécurité, règles, charte, utilitaires, README, index, stratégie et plans de tests, guides ; nouveau guide Recherche) — illustrations à compléter par Joëlle (emplacements notés dans le guide Recherche)
- [x] Plan de conception métier rédigé et décisions actées (D-13 à D-19)
- [x] Ajouter illustrations manquantes dans `Documentation_technique_UI_Althea.md`
  - [x] DialogChoix (Information, Warning, Error, Success, Question)
  - [x] UtilisateurEdition (Création, Modification, Consultation)
  - [x] UC_Utilisateurs (vue Admin, vue SuperUser)
- [~] Construire checklist de tests fonctionnels V1 par parcours utilisateur
  - [ ] Tests parcours patients (création, recherche, modification)
  - [ ] Tests parcours dossiers (création, transitions statuts)
  - [ ] Tests parcours séances (saisie, tarif, chevauchement, quota)
  - [ ] Tests parcours paiements (sélection séance, paiement partiel, annulation, affichage du reste dû)
  - [ ] Tests parcours documents (upload, liaison, envoi)
  - [ ] Tests parcours agenda (création RV, notifications)
- [ ] Exécuter recette finale de stabilisation V1
- [ ] Mettre à jour changelog et note de version V1

---

## G. Vue "fait / en cours / reste"

### ✅ Déjà fait

- [x] Socle technique (startup, config, DB manager, logs)
- [x] Sécurité de base (auth, rôles, session, changement MDP, élévation)
- [x] Navigation et shell UI principal
- [x] Module paramètres opérationnel
- [x] **Module utilisateurs complet** (liste, création, modification, consultation, actions admin)
- [x] **DialogChoix personnalisé** (remplacement complet MessageBox)
- [x] **UtilsIcons centralisé** (icônes d'état avec priorités)
- [x] **Documentation complète administration** (tous les documents de référence à jour)
- [x] POC documents et agenda réalisés
- [x] **Lot 0 : socle métier complet**
  - [x] Migration schéma `domaines`/`therapeutes`/N-N
  - [x] 9 UC référentiels déployés (piles complètes Query/Modèle/Gestion/UC)
  - [x] `UC_ReferentielBase` avec hooks extensibles
  - [x] `UC_RichTextEditor` (complet) + `UC_RichTextEditorSimple` (allégé)
- [x] Paramétrage portable des domaines : liaisons tarifs/natures, comportements explicites et quotas INAMI datés (19/09)
- [x] Refonte Domaine = convention : organismes, catégories et règles de quota, écran `UC_DomainesHome`, guide utilisateur (30/09–01/10)
- [x] Paiements : liaison séance, reste dû calculé, paiements partiels, éditeur et grille contextualisés (20/09)
- [x] Grille Patients : domaine dominant, filtre et densification (20/09)
- [x] Cabinet / praticien, organismes et périodicités, référentiels pays / civilités / mutualités, refonte UI-1 à UI-5 (06–11/10)
- [x] Lot 30 : paramètres lus par le code, sauvegarde chiffrée, restauration (12–13/10)
- [x] Administration technique : journaux et état du système (13/10)

### 🔜 Reste à faire

- [ ] Finaliser la recette transversale des dossiers, séances, tarifs, quotas et paiements
- [ ] Intégration POC documents/agenda dans l'application principale (stockage hors DB, Google Calendar pilier)
- [ ] Installation chez Pearl (sauvegarde et restauration faites le 12–13/10)
- [ ] Fin de la refonte UI (UI-6 / UI-7)
- [ ] Stabilisation et validation fonctionnelle V1

---

## G bis. Habillage de la carcasse (Phase 4 bis — détail dans `Planning_actions_Althéa.md`)

- [x] Lot 27 — Fondation Cabinet : cabinet, praticiens, identifiants pro (référentiel), qualifications, images (photo/cachet/signature), cachet généré — réalisé 06/10/2026 (v2.29.0 → v2.31.0, situations juridiques, BCE à un seul endroit, IBAN/BIC)
  - [ ] Vérifier l'import du logo cabinet et de la signature (photo praticien et cachet validés)
  - [x] Logos par **domaine** ✅ 20/10/2026 (testé par Joëlle ; cadres en longueur après son relevé : logos lisibles, logo du cabinet aussi) (migration v2.45.0 ; `PLAN_TESTS_DOMAINES` section 11, `PLAN_TESTS_DOSSIERS` D-28 à D-30) :
    - chaque domaine (sauf Psychologie) a un logo ;
    - logo facultatif importé depuis l'étape 1 de `UC_DomainesHome` (enregistré avec le domaine), PNG réduit à 256 px dans le **dossier de données** (`Domaines/Domaine_{id}/Logo.png`), nom de fichier seul en base (`ref_domaines.logo_fichier`).
    - Affiché (choix de Joëlle, 20/10/2026) : écran Domaines, convention du domaine, bandeau du dossier, liste « Domaine » de la fiche dossier.
    - Documents : plus tard, avec la gestion documentaire.
  - [ ] Utiliser `porteur_bce` / situation juridique dans la future facturation (lot 32)
  - [ ] V2 multi-praticiens : liste, Nouveau, Actif/Inactif, lien dossier ↔ praticien (non prévu en V1)
- [x] Lot 28 — Utilisateurs : lien praticien ↔ compte SuperUser réalisé 08/10/2026 (v2.32.0, UC_Cabinet › Compte de travail, Admin préservé) ;
  - **photo utilisateur** réalisée le 12/10/2026 (v2.36.0 : compte lié = photo du praticien, photo propre sinon, initiales à défaut ; bandeau de la Home) avec l'habillage en cartes de `UtilisateurEdition` — tests validés par Joëlle le 12/10.
  - Préférences utilisateur → avec le lot 31 ;
  - journal d'audit → reporté (ordre de travail dans le Planning)
- [x] **Administration** (ajouté le 12/10/2026) :
  - consultation des logs (`UC_Journaux`) ;
  - revue de ce qui est journalisé (noms de patients, contacts et thérapeutes retirés, valeur des doublons MariaDB masquée — ADR-39) ;
  - page « État du système » (`UC_EtatSysteme`, version de base comparée à `VersionSchema.Attendue`) — réalisé et validé le 13/10 (`PLAN_TESTS_ADMINISTRATION.md`) ;
  - icônes dessinées le 14/10 (tuiles `journaux` et `etatSystem`, titres `journaux` et `etatSystem`)
- [x] Lot 29 — Organismes étoffés (coordonnées, contacts, plateforme, périodicité) — réalisé 09/10/2026 (v2.33.0, périmètre minimal : adresse, 2 téléphones, 2 e-mails, site, plateforme, procédure, périodicité portée par l'organisme ; nouveau référentiel `ref_periodicites` / `UC_Periodicites`)
  `PLAN_TESTS_REFERENTIELS.md` §14.12 et §14.16 à exécuter
  - [ ] Exploiter `nb_mois` de la périodicité dans la facturation (lot 32) et le widget « Séances à encoder » (lot 31)
  - [ ] Contacts nominatifs par organisme (table dédiée) : reporté, non demandé en V1
- [x] Habillage visuel transverse (09/10/2026) — **approche abandonnée** : les fonds arrondis `Assets/ui-backgrounds/` posés en `BackgroundImage` se déforment selon la taille du contrôle et doivent être posés un par un. Remplacés par des contrôles dessinés → voir **section G ter**.
  - [x] Protection de la transparence des contrôles habillés dans `UtilsButtons` (plus d'écrasement du `BackColor`) — reste valable
  - [x] Panneau « Détail » accolé à la grille (~2 cm), même hauteur, débordement géré — reste valable
- [x] **Référentiels de saisie** (Pays, Civilités, Mutualités — décisions du 11/10/2026) — en 4 étapes testables, terminées le 12/10/2026 :
  - [x] **R1** (11/10/2026) :
    - migration v2.34.0 (`ref_pays` ISO avec 5 lignes système, `ref_civilites`, `ref_organismes.type_organisme` + 7 organismes assureurs, colonnes `id_pays` / `id_civilite` / mutualité patient, conversion des textes) ;
    - écrans `UC_Pays`, `UC_Civilites`, type dans `UC_Organismes`, domaines limités aux organismes de remboursement.
    - `PLAN_TESTS_REFERENTIELS.md` §14.17 à §14.20 validés.
  - [x] **R2** (12/10/2026) :
    - pays et civilité dans les écrans de saisie (Cabinet, fiche Patient, ContactEdition, TherapeuteEdition ; civilité reprise du thérapeute dans IntervenantEdition) via `UtilsSaisieReferentiels` ;
    - règles de validation alimentées par le code ISO ;
    - pays par défaut = pays du cabinet ;
    - code mort de `UtilsTelephone` retiré.
    - `PLAN_TESTS_REFERENTIELS.md` §14.21 validés.
  - [x] **R3** (12/10/2026) : mutualité sur la fiche Patient (liste, n° d'affiliation, CT1 / CT2, loupe → fiche ou ajout en fenêtre modale filtrée sur les mutualités) ; `UC_Organismes` ouvrable en contexte (type imposé, présélection). `PLAN_TESTS_REFERENTIELS.md` §14.22 validés.
  - [x] **R4** (12/10/2026) : migration v2.35.0 — suppression des colonnes texte `pays`, `civilite`, `mutualite` (confirmée et appliquée le 12/10). `PLAN_TESTS_REFERENTIELS.md` §14.23 à exécuter ; régénérer `backup_NoData` / `backup_WithData` (encore au schéma v2.34.0).
  - [ ] **Script de remise à vide de la base** (à écrire) : conserver les lignes système (cadenas) et rejouer les amorçages des migrations (rejouables sans doublon), dont les 7 mutualités
- [~] Lot 30 — Paramètres enrichis et sauvegarde / restauration (découpage du 12/10/2026) :
  - [x] P1 — Paramètres enrichis (v2.37.0) :
    - durée et grille horaire des séances, échecs de connexion, conservation des logs, encre du cachet, bornes des nombres, type PATH avec « Parcourir… », avis de portée après modification, écran limité à la modification de la valeur (plus de création) ;
    - arborescence fixe du dossier de données (`ArborescenceDonnees`, ADR-35) ;
    - suppression des paramètres de test et des chemins obsolètes ;
    - correction de l'enregistrement sur le mauvais paramètre ;
    - table réinitialisée (`reinit_tec_parametres.sql`) — réalisé et validé le 12/10 ;
    - refonte visuelle de l'écran en UI-7
  - [x] P2 — Sauvegarde (v2.38.0, ADR-37) : archive ZIP AES-256 (dump `mariadb-dump` + dossier de données + manifeste), mot de passe mémorisé DPAPI (Admin), dossier hors données, rotation, journal `tec_sauvegardes`, rappel, écran `UC_Sauvegardes` (SuperUser et Admin) — réalisé et validé le 12/10
  - [x] P3 — Restauration (v2.39.0, ADR-38) :
    - `DialogRestauration` (Admin) ;
    - même version du schéma seulement ;
    - compte d'administration MariaDB saisi, jamais enregistré ;
    - sauvegarde de sécurité, documents extraits puis échangés (ancien dossier conservé `…_avant_restauration_…`), base vidée puis rechargée, remise en état automatique en cas d'échec, paramètres du poste rétablis, journal `tec_restaurations`, redémarrage ;
    - guide utilisateur des sauvegardes (procédure manuelle pour poste neuf / archive ancienne) — réalisé et validé le 13/10 (sauf 7.4 / 7.5 à revérifier ; 7.15 reporté)
- [ ] Tests restauration à finir : 7.4 (mot de passe d'archive faux, correction B4), 7.5 (compte inexistant), 7.15 (remise en état automatique avec une archive volontairement abîmée)
- [ ] Lot 31 — Tableau de bord d'Accueil : socle des widgets et premiers widgets (détail dans le planning) — **en dernier**, après Agenda et Documents (décision du 12/10/2026), avec les préférences utilisateur
- [ ] Lot 32 — Facturation : encodage mensuel, mutualités, numérotation
- [ ] Lot 33 — Assistant premier démarrage, export configuration, RGPD
- [ ] **Préparation de l'installation** (demande du 12/10/2026) :
  - inventaire des données de départ (lignes système « cadenas » et données préremplies utiles au SuperUser : mutualités, civilités, pays, domaines PSY / GRA / TCA / REA, etc.) ;
  - scripts d'installation (schéma + données de départ ; `tec_parametres` : `Docs/Database/Maintenance/reinit_tec_parametres.sql` sert déjà de jeu de référence) ;
  - la sauvegarde de structure cite la base `althea` dans les valeurs par défaut de 15 tables (`DEFAULT nextval(`althea`.`seq_…`)`) : une base installée sous un autre nom pointerait vers les séquences d'`althea` — décider (nom de base imposé, ou script d'installation qui retire la qualification, comme le font les tests ; constat du 26/10/2026, ADR-58) ;
  - remise à vide d'une base qui conserve ces lignes ;
  - choix du dossier de données par le SuperUser à l'installation (hors du dossier de dev OneDrive) ;
  - compte MariaDB de l'application créé avec un mot de passe fort et ses seuls droits sur les données (`SELECT, INSERT, UPDATE, DELETE, EXECUTE, SHOW VIEW`, 13/10/2026) ;
  - assistant de restauration sur poste neuf (aujourd'hui procédure manuelle : `Guide_Utilisateur_Sauvegardes.md`, chapitre 6) ;
  - contrôle de la version du schéma au démarrage ;
  - **numéro de version de l'application** (aujourd'hui 1.0.0.0, jamais incrémenté — constaté sur l'État du système le 13/10/2026) : le tenir à jour à chaque livraison (projet `.vbproj`, CHANGELOG), pour savoir quelle version tourne chez Pearl
    - convention retenue le 13/10/2026 : MAJEUR.MINEUR.CORRECTIF, version inchangée pendant le développement, **1.0.0 à la première mise en service chez Pearl**, puis une version par livraison et une étiquette Git (`v1.0.0`…) sur le commit livré ;
  - réglage des comptes root du serveur (identification Windows `gssapi` en plus du mot de passe — constaté sur le poste de dev le 13/10/2026 — et compte `root@'%'` ouvert au réseau : à décider, a priori pas de root à distance)

## G ter. Chantier UI — kit d'interface (plan : `Plan_Refonte_UI_Althea.md`)

Remplacement de l'habillage par images étirées par des contrôles dessinés. Plan validé le 10/10/2026.

- [x] **Préparation (09/10/2026)** — fond fixe de la Home 2560x1440, logo séparé, feuillage dessiné, « feuille coupée » automatique sur les UC hébergés. Vérifié à l'exécution.
- [x] **Maquettes validées (09/10/2026)** — kit, hubs Référentiels et Administration, liste Patients, dialogue Séance, Domaines, Cabinet.
- [x] **UI-0 — Compiler et valider le kit** (10/10/2026) : les 7 types compilent, le concepteur les affiche, `TestKitAlthea` est conforme. Bug trouvé au passage : `UITheme.ColorRougeMoyenClair` valait (168, **20**, 110) au lieu de (168, 120, 110) — corrigé.
  - [ ] Vérifier le rendu à 125 % et 150 % de mise à l'échelle Windows
- [x] **UI-1 — Hubs** (10/10/2026) : `UC_ReferentielHome` (15 tuiles, 4 sections) et `UC_AdminHome` (7 tuiles, 2 sections) habillés ; quadrillage supprimé ; `.resx` 91 → 38 Ko et 65 → 27 Ko ; fichiers métier inchangés hors déplacement ci-dessous.
  - [x] **Scintillement corrigé** : composition de la fenêtre (`WS_EX_COMPOSITED` sur `Home`), chargement de la vue masquée (`NavigationManager`), feuillage dessiné seulement sur la zone utile, ombres allégées. Bénéficie à tous les écrans.
  - [x] **Situations juridiques déplacées** des Référentiels vers l'Administration (ADR-31), droits alignés sur la fiche Cabinet. Testé avec les profils Admin et SuperUser.
  - [x] Titre redondant supprimé dans `UC_AdminHome` (`lblTop` + son panneau).
- [x] **UI-2 — Onglets** (10/10/2026) : `AltheaOnglets` (`TabControl` hérité et dessiné, ADR-32) validé sur `SeanceEditor`, puis posé sur `UC_DossierFiche`, `UC_PatientFiche`, `UC_Cabinet`, `UC_Parametres`, `UC_DomainesHome` et la zone « données liées » de `UC_ReferentielBase`. Syncfusion non ajouté.
  - [x] **Scintillement de la feuille corrigé** dans l'onglet Séances de `UC_DossierFiche` : la bande native repeignait le feuillage à l'écran ; le kit la dessine dans son tampon (`CouleurFond = Transparent`). Double tampon également activé sur les surfaces de feuillage de la Home.
  - [x] Cabinet / Domaines : texte des pages en gras (hérité de l'ancienne police d'onglets). **Décidé : tout passe en normal, comme les autres écrans** — fait au lot UI-5 (11/10/2026).
- [x] **UI-3 — `AltheaEnTete`** (10/10/2026) : en-tête commun (icône, titre, sous-titre sous le titre, trait fin) posé sur tous les UC et Forms (12 UC, 19 Forms, + 15 référentiels via `UC_ReferentielBase`) ; rétabli sur `DialogComptePraticien` ; textes corrigés (Connexion, Login, Élévation d'accès…).
- [x] **Bandeaux — maquettes validées** (10/10/2026) : recherche = modèle commun (3 densités + version onglet) ; récapitulatif patient = blocs groupés (Patient, Dossier, Finances, Quotas INAMI).
- [x] **UI-4 — Écrans de liste** (10/10/2026) —
  - bandeau de recherche (Patients, Thérapeutes, Utilisateurs, 15 référentiels, Domaines ; champs `AltheaRecherche` / `AltheaListe` / `AltheaCase`, ADR-33),
  - panneau « Détail » des référentiels en carte,
  - et **barres d'actions du kit sur les 28 écrans** qui en ont une (listes, fiches, fenêtres) : `UC_PatientHome`, `UC_Therapeutes`, `UC_Utilisateurs`, `UC_ReferentielBase` (→ 11 référentiels d'un coup).
- [x] **Barres locales** (11/10/2026) : maquette (version B retenue), `AltheaBarreOutils` + variantes Outil d'`AltheaBouton` (ADR-34), posées sur 7 listes (fiche Patient, fiche Dossier, Cabinet). Domaines (Tarifs, Natures) au lot UI-6.
- [x] **UI-5 — Fiches** (11/10/2026) : `UC_PatientFiche`, `UC_DossierFiche`, `UC_Cabinet`, `UC_Parametres` ; `GroupBox` des fiches remplacés par des cartes. Reportés : `GroupBox` de Domaines (UI-6) et des fenêtres (UI-7), `UC_Accueil` (lot 31).
- [x] **UI-6 — Domaines** (terminé et validé le 14/10/2026) : `AltheaEtapes`, panneau de détail, note d'aide. *Déjà fait : en-tête, recherche, barre d'actions, onglets, synthèse V2, texte en normal ; reste : 10 boutons natifs, 2 `GroupBox`, champs, panneaux de liaison.*
  - [x] 6a (14/10/2026) : `AltheaEtapes`, onglets masqués (`MasquerOnglets`), Général en deux cartes, lecture seule lisible — validé
  - [x] 6b (14/10/2026) : Tarifs et Natures (carte + barre locale, catégorie de quota dans la grille) — validé
  - [x] 6c (14/10/2026) : Catégories & quotas (pastilles, règle sélectionnée en lecture seule, saisie guidée et récapitulatif, correction des règles qui n'ont pas servi, suppression qui rouvre la règle précédente, périodes sans règle — ADR-40) — validé
  - [x] 6d (14/10/2026) : Synthèse en carte « Convention du domaine » ; guide `Guide_Utilisateur_Domaines.md` revu ; ADR-40 (+ table des ADR complétée jusqu'à 40) ; plan de tests Domaines V2.0 ; documentation technique, charte, références UI ; périodes sans règle mises en évidence ; quota de la synthèse en bande pleine largeur — validé
  - [x] Décidé (Joëlle, 14/10/2026) : une période sans règle n'est jamais comblée automatiquement (aucune date d'une autre règle déplacée d'office) ; elle est rendue très visible (ligne d'alerte dans la grille, étape « À compléter », synthèse, annonce avant l'action qui la crée) pour obliger à créer la règle intermédiaire — fait en 6d
- [ ] **Captures d'écran** (Joëlle, à la fin de la refonte UI) : refaire toutes celles de `Docs/Architecture/Documentation_technique_UI_Althea.md`, dont Domaines et panneaux de liaison (antérieures à UI-6).
  - **24/10/2026** : faites avec l'outil `FiltreCaptureEcran` (Debug, Ctrl+Maj+F8) — tous les écrans, onglets et fenêtres de la doc UI, rangés dans `Docs/Illustrations/Doc_UI` (liens corrigés), guide Recherche et guide de l'éditeur compris ;
  - complétées par Joëlle le 24/10 : `Login`, `ElevationAcces`, `DialogUtilisationAvoir`, `DialogComptePraticien`, `UC_ApercuPatient`, vue SuperUser de `UC_AdminHome` ;
  - restent : vue User de `UC_AdminHome` (ancienne), 6 images de la Charte graphique (boutons, panneaux, aperçu de Home) ;
  - **16/10/2026** : les anciennes images de `Docs/Illustrations/` ont été retirées du dépôt (antérieures à la refonte) ;
  - les liens de la documentation technique pointent vers des fichiers à refaire sous les mêmes noms
- [x] **Vitrine du kit** (décidé le 15/10/2026 ; **fait le 16/10/2026** : `TestKitAlthea` V2.0.0, six onglets — palette et polices, boutons, structure, champs, indicateurs, grilles —, bouton « Vitrine du kit » de l'accueil ; 12 captures intégrées à `Reference_UI.md`) :
  - fenêtre de test (Debug, à partir de `TestKitAlthea`), un onglet par famille de contrôles, chaque contrôle dans tous ses états (normal, survol, désactivé, consultation, erreur) ;
  - sert aux captures de `Docs/Rules/Reference_UI.md` (qui remplaceront les maquettes) et de banc de vérification après chaque modification du kit.
- [x] **Boutons désactivés peu lisibles** ✅ 20/10/2026 (validé par Joëlle) (`AltheaBouton` V1.3.0, texte (108, 120, 113) et icône à 55 % ; vitrine ; `PLAN_TESTS_KIT_ALTHEA` KB-01 à KB-02) (relevé sur la vitrine, 16/10/2026) :
  - un `AltheaBouton` d'action désactivé (Sauge, Ocre, Terre) a un texte presque invisible sur son fond gris-vert pâle ;
  - à revoir dans `AltheaBouton` (contraste du texte et de l'icône désactivés), puis vérifier sur la vitrine.
- [ ] **Bouton inactif : dire pourquoi** — **fait et validé le 20/10/2026 pour les barres Séances et Paiements** (reste : les autres écrans, au fil de l'audit)
  - (`AltheaBouton.DefinirDisponibilite` / `RaisonInactif`, infobulle au survol ; `PLAN_TESTS_KIT_ALTHEA` KB-03 à KB-04, `PLAN_TESTS_DOSSIERS` D-31 à D-32 ; autres écrans au fil de l'audit)
  - (relevé du 16/10/2026, Joëlle : « Nouveau » de l'onglet Séances gris sur un dossier clôturé, sans explication) : infobulle sur les boutons d'action désactivés qui donne la raison (ex. « Dossier clôturé : séances en lecture seule », « Sélectionnez une séance »), en commençant par les barres locales Séances et Paiements de `UC_DossierFiche` ;
  - à traiter avec la lisibilité des boutons désactivés.
  - *17/10/2026 : pour un dossier clôturé ou archivé, le bandeau sous l'en-tête le dit désormais ; l'infobulle reste à faire pour les autres cas.*
- [ ] **Lecture seule lisible — tour des écrans** (remarque de Joëlle, 14/10/2026 ; **décision du 14/10 : fait écran par écran avec UI-7**, en même temps que le passage aux champs du kit — relevé : 17 `ComboBox`, 7 `DateTimePicker`, 4 `CheckBox` et 1 `NumericUpDown` Windows restent grisés tant qu'ils ne sont pas remplacés) :
  - un champ consulté doit se lire sans effort.
  - Décision : ne plus désactiver une zone entière (`Enabled = False` grise aussi libellés et valeurs) ; utiliser `UtilsControls.DefinirLectureSeule` (texte normal, fond teinté `UITheme.ColorChampLectureSeule`).
  - Déjà fait le 14/10 : `AltheaCase` et `AltheaListe` désactivées se dessinent lisibles (profite à tous les écrans qui les utilisent), Général de Domaines.
  - Fait le 14/10 :
    - `UC_PatientFiche` (onglet Identité : 7 listes en `AltheaListe`, date de naissance en `AltheaDate` facultative, trois cartes en consultation lisible) ;
    - `UC_DossierFiche` (domaine en `AltheaListe`, date d'ouverture et filtres du / au des onglets Séances et Paiements en `AltheaDate`, carte Paramètres en consultation lisible).
  - Fait le 14/10 (à tester) : `ContactEdition`, `IntervenantEdition` (champs de trace toujours teintés), `TherapeuteEdition` (toujours en saisie), `UtilisateurEdition` (fond teinté selon la lecture seule de chaque mode) ;
  - le 15/10 : `SeanceEditor` (`DefinirSaisieActive`, durée et montant lisibles), `PaiementEditor` (`AppliquerEtatSaisie`), `DossierStatutEditor` (libellé de la date plus grisé).
  - Reste à revoir, écran par écran :
    - `UC_Cabinet` (boucle propre : zones de texte en lecture seule, autres contrôles désactivés),
    - ~~`UC_ReferentielBase` (panneau Détail, 15 référentiels)~~ ✅ 16/10/2026 (V1.4.0 : consultation lisible et règle des fenêtres, 7 référentiels à champs propres repris),
    - ~~`UC_Parametres`~~ ✅ 16/10/2026 (V2.0.0 : page unique en consultation lisible),
    - panneaux de liaison (traités en 6b).
  - Les `ComboBox` / `CheckBox` / `DateTimePicker` Windows resteront grisés tant qu'ils ne sont pas remplacés par les champs du kit : à faire avec UI-7 (`AltheaChamp`).
- [x] **`DialogChoix` — refonte complète** (demande de Joëlle, 14/10/2026 ; première étape d'UI-7 — **fait et validé le 14/10/2026** : B + icônes style 2, `DialogChoix` V2.0.0, `AltheaIconeEtat`, fenêtre `TestDialogChoix`, adoption dans les règles de quota ; reste à adopter les boutons qui disent l'action écran par écran avec UI-7) :
  - maquette d'abord — planches « DialogChoix — icônes », « A » et « B » proposées le 14/10/2026 ;
  - mise en page du kit (en-tête, message, zone de boutons de la barre d'actions) ;
  - fenêtre **extensible** (largeur et hauteur adaptées au message, défilement au-delà, redimensionnable si utile) ;
  - **nouvelles icônes** (Information, Avertissement, Erreur, Question, Succès, Attente) à la place des GIF actuels — dessinées par le code dans le style du kit, ou fournies par Joëlle, à décider sur la maquette.
  - Correctif provisoire déjà fait le 14/10 : hauteur adaptée au texte (V1.1.0).
  - **Revu le 16/10/2026** (V2.1.0, maquette C) : petite fenêtre du kit comme les autres (barre de titre, `AltheaEnTete`, carte, `AltheaBarreActions`, boutons dans l'ordre habituel), la V2.0 jurant avec les fenêtres en cartes.
- [x] **Cabinet — cohérence des enregistrements** ✅ 20/10/2026 (validé par Joëlle) (décision de Joëlle : « tout différer » ; `PLAN_TESTS_UC_CABINET` 6.1, 6.2, 6.4, 6.5, 6.9, 7.1, 7.2, 7.5, 7.6, 9.3, 9.5, 9.6 ; audit § 4.2) :
  - images et cachet attendent Enregistrer (Annuler les abandonne, plus de demande « remplacer ? ») ;
  - cachet généré depuis la saisie en cours ;
  - fiche écrite en une transaction (`GestionCabinet.EnregistrerFiche`), puis les images (fichier, puis nom en base ; avertissement si l'une échoue) ;
  - grilles limitées à la longueur des colonnes.
  - Les imports restent possibles une fois le cabinet / le praticien enregistrés (dossier de rangement).
  - *Relevé d'origine (16/10/2026) : images enregistrées aussitôt hors Modifier / Enregistrer, cachet depuis la base, écritures sans transaction.*
- [x] **Types d'identifiant professionnel — écran de gestion** ✅ 20/10/2026 (testé par Joëlle ; avec l'IBAN / BIC présentés dans la fiche Cabinet, `PLAN_TESTS_UC_CABINET` 4.10 et 4.14) (`PLAN_TESTS_REFERENTIELS` section 18, `PLAN_TESTS_UC_CABINET` 8.7 et 8.8) :
  - `UC_TypesIdentifiantPro` (socle `UC_ReferentielBase`, SuperUser), tuile dans le hub **Administration** à côté des situations juridiques (décision de Joëlle : paramètre du cabinet ; garder en vue la V2, cabinet de plusieurs praticiens — d'où « Porté par »).
  - Champs : porté par (Praticien / Cabinet), masque indicatif (rappelé quand une valeur est refusée), expression de contrôle vérifiée avant d'enregistrer ;
  - INAMI, BCE et TVA (contrôle écrit dans l'application) deviennent tous lignes système (migration v2.47.0) et leur expression est en lecture seule.
  - Fiche Cabinet : un type désactivé n'est plus proposé mais reste affiché sur les identifiants qui l'utilisent (avant : cellule vide).
  - *Relevé d'origine (16/10/2026) : aucun écran, ajout d'un type ou réglage du masque, du contrôle ou de la cible seulement par script SQL.*
- [x] **Latéralité : valeurs codées en dur** ✅ 20/10/2026 (testé par Joëlle ; pas de bouton [+], décision de Joëlle) (référentiel `ref_lateralites`, écran `UC_Lateralites`, migration v2.46.0 avec reprise des valeurs saisies ; `PLAN_TESTS_REFERENTIELS` section 17, `PLAN_TESTS_PATIENTS` PAT-82) ;
  - avec la modalité de paiement proposée, sortie du code en paramètre (`DOSSIER_MODALITE_PAIEMENT_DEFAUT`, `PLAN_TESTS_DOSSIERS` D-33).
  - Reste : retirer la colonne texte `patients.lateralite` après vérification (migration ultérieure).
- [x] **Champ d'heure du kit** (UI-7, 16/10/2026) : `AltheaHeure` (maquette « AltheaHeure », **liste A** retenue par Joëlle : liste déroulante, durée rappelée pour la fin) ;
  - saisie hh:mm (9, 930, 9h30…), Haut / Bas au pas, lecture seule lisible ;
  - posé dans la fenêtre Séance (`hreDebut` / `hreFin`, à la place des listes Windows au format HH:mm:ss) et « Reporter la séance » ;
  - vitrine du kit.
  - Tests : `PLAN_TESTS_KIT_ALTHEA.md` § 5 (KH-01 à KH-12).
- [x] **Dates encore en `DateTimePicker` Windows** ✅ 20/10/2026 (testé par Joëlle) (`PLAN_TESTS_UC_UTILISATEURS` 16.1–16.2, `PLAN_TESTS_DOMAINES` D-99, D-100 ; avec l'audit B7) (relevé du 16/10/2026 lors de l'inventaire des heures) :
  - filtre « Dernière connexion depuis » de `UC_Utilisateurs` (`dtpDernierLoginDepuis`),
  - dates des règles de quota de `UC_DomainesHome` (`dtpQuotaDebut` / `dtpQuotaFin`),
  - dates des tarifs liés de `UC_LiaisonsPrixDomaines` (`_dtpDebut` / `_dtpFin`) → `AltheaDate`.
- [x] **Champ de date du kit** (UI-7, 14/10/2026) : `AltheaDate` + `AltheaCalendrier` (maquette, calendrier A, date facultative « vide + croix ») ;
  - démonstration `TestChampDate` ;
  - `DefinirLectureSeule` le prend en charge.
  - Reste : remplacer les `DateTimePicker` écran par écran (fiches patient et dossier, fenêtres Contact et Intervenant faites le 14/10/2026, fenêtres Séance, Paiement, statut du dossier et report de séance le 15/10/2026 ; planning du jour le 15/10/2026 ; 8 restants, dont les 2 heures du report) ;
  - **champ d'heure du kit** plus tard : remplacera les listes d'heures saisissables de `SeanceEditor` (encore grisées en consultation) et les heures du report de séance.
- [ ] **UI-7 — Fenêtres et contrôles de saisie** : 20 Forms — ✅ **toutes reprises le 15/10/2026** (cartes, fond du kit, contrôles du kit ; champ d'heure du kit ✅ 16/10/2026 ; restent les `NumericUpDown`, trois dates Windows et la grille des Paiements ; `UC_Parametres` ✅ 16/10/2026 : page unique, maquette A, groupes en base v2.40.0) ;
  - `AltheaChamp` pour `TextBox` / `ComboBox`, cases à cocher, sélecteurs de date, grilles (dont la grille des Paiements, voir C4).
  - *Déjà fait : en-têtes, barres d'actions, onglets de SeanceEditor ; reste : panneaux centraux, boutons hors barre (25), `GroupBox` (11), fonds image (19), champs de toutes les fenêtres et des fiches — inventaire dans le plan UI.*
- [x] **🔒 Clé de licence Syncfusion hors du dépôt** ✅ 18/10/2026 (validée par Joëlle ; `PLAN_TESTS_UC_PARAMETRES` section 9 ; ADR-41) :
  - clé retirée de `Program.vb` ;
  - saisie par l'Admin dans Paramètres (carte « Licences — propre à ce poste »), chiffrée DPAPI dans `%APPDATA%\Althea\althea.syncfusion.json` (`LicenceSyncfusion`, `GestionLicences`), remise à Syncfusion au démarrage et dès la saisie ;
  - état dans l'État du système.
  - Guide de licence réécrit.
  - [x] **Joëlle** : demander une nouvelle clé sur le compte Syncfusion (les deux anciennes restent lisibles dans l'historique Git), la saisir sur le poste de développement puis sur celui du cabinet — ✅ nouvelle clé mise en place (Joëlle, 23/10/2026)
  - [ ] Purger l'historique Git du miroir public (anciennes clés, ancien mot de passe ; rapport d'audit, lot A point 2)
- [x] **UI-8 — Nettoyage complet** ✅ 18/10/2026 (quatre catégories validées ; bilan dans `Plan_Refonte_UI_Althea.md`, 8d) : audit des fichiers et assets non référencés, suppression par catégories, dépôt propre en local et sur GitHub.
  - [x] Catégorie « assets » ✅ 17/10/2026 (validée par Joëlle, commit d780ac47) (tuiles Administration et Référentiels avec survol, zones du dossier, en-têtes, barre RTF) :
    - audit des trois chemins de chargement (nom sur disque, `My.Resources`, images intégrées par le Designer) ;
    - 51 fichiers retirés du dépôt (PNG de statut de la grille, GIF, `ui-backgrounds`, anciens fonds, icônes jamais branchées, photo de la praticienne gardée sur le disque mais ignorée par Git), 20 ressources, 146 entrées du `.vbproj` (dont 106 visant des dossiers disparus) ;
    - chemin absolu de `.editorconfig` rendu relatif ;
    - icônes `therapeute_hover/disabled` renommées `therapeutes_*` ;
    - tuiles Cabinet et Utilisateurs de `UC_AdminHome`, Natures séance et Prix séance de `UC_ReferentielHome` sur leurs icônes (signalé par Joëlle pour Natures séance : pas de survol).
    - `Assets` : 356 → 305 fichiers suivis (4,8 → 4,0 Mo).
    - 8 PNG d'abord supprimés puis restaurés : sources d'images intégrées dans des écrans (comparaison au pixel).
    - Sources gardées : `Titre_Ico`, `Editor_Ico`, `Splash`, `Appli_Ico` (images intégrées par le Designer).
    - Relevés du 11, 14 et 17/10/2026 traités ;
    - `Assets/Boutons_Suivi` sert bien (zones du dossier, nom = enum `ZoneDossier`)
  - [x] Catégorie « resx » ✅ 18/10/2026 (validée par Joëlle) :
    - `Fond_1000x770_FeuilleCoupee1` retiré des Designers de 11 UserControls, puis ressource et fichier supprimés ;
    - `ReferentielModalHost` V1.2.0 donne à la vue hébergée le fond du kit (elle montrait encore l'ancien fond étiré).
    - Décisions de Joëlle : les 102 icônes intégrées en double (77 Ko) restent, pour l'aperçu dans le Designer ;
    - les familles d'icônes restent complètes (`_normal`, `_hover`, `_disabled`)
  - [x] Catégorie « code » ✅ 18/10/2026 (validée par Joëlle) :
    - supprimés `UI/Forms/Test/UC_Domaines` (+ `.resx`, ancien écran exclu de la compilation), `ApplicationEvents.vb` (modèle vide, inutile avec le `Sub Main` de `Program.vb`), `Utils/UI/UtilsString.vb` (deux fonctions jamais appelées) ;
    - commentaires de code qui citaient `UC_Domaines` en exemple mis à jour.
    - Les fenêtres de test de l'Accueil (Debug) restent (décision de Joëlle).
    - Audit : plus aucun autre `.vb` orphelin
  - [x] Catégorie « docs » ✅ 18/10/2026 (validée par Joëlle) :
    - plus aucun `_tmp_*` ni script de travail suivi ;
    - 14 liens vers des documents déplacés corrigés, 2 lignes vers des fichiers disparus retirées (Bibliothèque, Process) ;
    - 51 diagrammes de base périmés supprimés (`Docs/Database/Diagrams`, 14/06/2026, 25 tables sur 45) ;
    - `desktop.ini` et `settings.VisualStudio.json` (propres au poste) retirés du suivi et ignorés.
    - Décision de Joëlle : les 79 liens vers les captures supprimées le 16/10 restent (doc UI 71, Charte 6, guide RichTextEditor 2) — voir ci-dessous
  - [ ] **Captures d'écran à refaire** après la refonte UI : 79 liens d'images de `Docs/Illustrations/` en attente (doc UI 71, Charte 6, guide RichTextEditor 2) ; les captures supprimées le 16/10/2026 portaient les mêmes noms — **24/10/2026** : doc UI et guide RichTextEditor faits ; restent la Charte (6) et les quatre fenêtres ci-dessus
- [x] **UI-9 — Grilles : états lisibles partout** ✅ 18/10/2026 (quatre étapes validées) (revue des grilles demandée par Joëlle, 18/10/2026 ; découpage et maquette acceptés) :
  - hors écrans déjà repris (Patients, séances, paiements, aperçu), les états se lisent en texte coloré, case « Actif » ou icône PNG ;
  - aligner sur la règle des pastilles (`Reference_UI`), sur des **tons sémantiques** pris dans les teintes existantes
  - [x] Maquette « Grilles — états lisibles » ✅ 18/10/2026 (validée par Joëlle : créneaux libres du planning à essayer, « Système » en bleu, rôle User sans pastille ; image `Reference_UI/grilles_etats.png`).
    - Révèle un défaut des grilles déjà reprises : une pastille « OK » / « Actif » / « Réalisée » disparaît sur les lignes alternées (même teinte que le fond) → fin contour sur toutes les pastilles pleines (9.1)
  - [x] 9.1 — Socle ✅ 18/10/2026 (validé par Joëlle ; `PLAN_TESTS_KIT_ALTHEA` section 6) :
    - `UtilsPastilles` V1.4.0 (`TonEtat`, `Teinte(ton)` ; familles existantes appuyées sur les tons, couleurs inchangées) ;
    - `UtilsDataGrid` V1.7.0 (double tampon, polices partagées, `AlignerCentre` / `AlignerDroite`, fin contour sur toute pastille pleine, décision de Joëlle après un premier essai sur les seuls fonds proches)
  - [x] 9.2 — Patient et dossier ✅ 18/10/2026 (validé par Joëlle) :
    - Dossiers (pastille de statut) et Intervenants (terminés atténués) de la fiche patient ;
    - paiements d'une séance (`DialogPaiementsSeance` : couleur et police posées à la main, police créée à chaque cellule) ;
    - planning du jour (pastille de statut, créneaux libres entre la première et la dernière heure paramétrées)
    - (S-39 à S-43, P-54, P-55, PAT-80, PAT-81) : `UC_PatientFiche` V1.6.0, `DialogPaiementsSeance` V1.3.0, `PlanningJourViewer` V1.3.0, `GestionSeances` V1.4.0 (`GetPlanningJour`, `OccupeSonCreneau` partagé avec le contrôle de chevauchement), `LignePlanningJour` (nouveau), `UtilsDataGrid` V1.8.0 (`DessinerRepereSurLigne`)
  - [x] 9.3 — Administration ✅ 18/10/2026 (validé par Joëlle) :
    - Utilisateurs (pastilles d'état à la place des PNG, rôle, tri),
    - État du système (trois tons distincts — « À vérifier » est aujourd'hui rouge comme « Problème » — et sections en repères titrés),
    - Journaux (pastille Erreur, sessions en repères),
    - historiques des sauvegardes et des restaurations (pastille Résultat, tri)
    - (PLAN_TESTS_UC_UTILISATEURS § 15, PLAN_TESTS_ADMINISTRATION § 5, PLAN_TESTS_SAUVEGARDES § 8) : `UC_Utilisateurs` V1.6.0, `UtilisateurApplication` V1.2.0 (`EtatCompte`), `UC_EtatSysteme` V1.1.0, `GestionEtatSysteme` V1.2.0, `UC_Journaux` V1.1.0, `UC_Sauvegardes` V1.2.0, `DialogRestauration` V1.3.0, `UtilsPastilles` V1.5.0, `UtilsDataGrid` V1.9.0 (`DesactiverTri`).
    - `UtilsIcons.IconOK / IconOFF / IconLock` n'ont plus d'appelant : à retirer en 9.4 avec les PNG `Tech_Ico`
  - [x] 9.4 — Référentiels et Domaines ✅ 18/10/2026 (validé par Joëlle) :
    - référentiels et Thérapeutes (inactifs atténués + pastille à la place de la case « Actif », pastille « Système » à la place du cadenas, tri), Domaines, règles de quota, catégories, liaisons (pastilles sur une famille fournie par le métier) ;
    - puis retrait des PNG `Tech_Ico` OK / NO / OFF / Lock devenus inutiles
    - (PLAN_TESTS_REFERENTIELS § 16, PLAN_TESTS_DOMAINES D-84 à D-88) : `UC_ReferentielBase` V1.5.0 (`ReferentielLigne.Etat`), `UC_Therapeutes` V1.1.0, `UC_LiaisonsNaturesDomaines` / `UC_LiaisonsPrixDomaines` V2.1.0, `UC_DomainesHome` V1.7.0 (`QuotaInamiRegle.Etat`), `CategoriesQuotaEditor` V1.3.0, `UtilsPastilles` V1.6.0 ;
    - 16 PNG `Tech_Ico`, leurs ressources et `UtilsIcons.IconOK / IconOFF / IconLock / IconNo` supprimés (aucune image embarquée identique, contrôle pixel à pixel des 235 images des .resx)
- [x] **Erreur de résolution du tarif à l'ouverture d'une séance** ✅ 18/10/2026 (vérifié par Joëlle : plus aucune erreur au journal) (relevée par Joëlle dans les journaux, 18/10/2026) :
  - « Erreur résolution tarif séance (SeanceEditor) — repli sur le montant du tarif sélectionné. Unable to cast object of type 'Althea.StatutSeance' to type 'System.IConvertible' ».
  - Cause : dans `SeanceEditor`, `DataSource` était posé avant `DisplayMember` / `ValueMember` ; le changement de sélection qu'il déclenche arrivait quand `SelectedValue` était encore l'objet `StatutSeance`, et le recalcul du tarif (statut, sans garde de chargement) échouait sur `Convert.ToInt64`.
  - Sans effet sur les données (le tarif de la séance était relu juste après), mais une erreur à chaque ouverture.
  - Corrigé le 18/10/2026 (`SeanceEditor` V1.5.3 : membres posés avant la source, 8 endroits ; PLAN_TESTS_SEANCES S-44)
  - [ ] Même ordre à vérifier ailleurs (13 endroits, dont `UC_DossierFiche` (liste des domaines) remis dans l'ordre le 20/10/2026 : `PaiementEditor` 3, `UC_PatientFiche` 2, `ContactEdition` 2, `IntervenantEdition` 2, `UC_Cabinet`, `UC_DossierFiche`, `UC_LiaisonsNaturesDomaines`) :
    - sans gravité tant qu'aucun changement de sélection ne lit `SelectedValue` pendant le chargement — à remettre dans l'ordre lors de la revue de code générale
- [x] **Pagination dans le thème et sur les grilles longues** ✅ 19/10/2026 (validé par Joëlle) (revue demandée par Joëlle, 19/10/2026 ; proposition acceptée) (`PLAN_TESTS_KIT_ALTHEA` KP-01 à KP-05, `PLAN_TESTS_DOSSIERS` D-24 à D-27) :
  - `UC_Pagination` V1.1.0 (boutons Outil du kit, résumé, navigation masquée sur une page, calcul sans ligne coupée, `HauteurReservee` pour les repères, tri pris en charge) ;
  - Séances et Paiements du dossier paginés (`UC_DossierFiche` V1.9.0, séances ouvertes sur la page d'aujourd'hui).
  - Inutile ailleurs (grilles courtes ; Administration écartée par Joëlle).
  - Documentation UI à reprendre après les illustrations de Joëlle.
- [x] **Aperçu de `UC_PatientHome` qui clignote** ✅ 19/10/2026 (vérifié par Joëlle) (relevé par Joëlle, 19/10/2026) : rechargement en boucle entre le changement de sélection et l'aperçu ; il n'est plus relu que si le patient change (`UC_PatientHome` V1.5.1).

---

## G quater. À programmer — idées notées le 10/10/2026

- [ ] **Terminer la Phase 4 bis** (habillage de la carcasse, lots 27 à 33 — `Planning_actions_Althéa.md`) : finir les lots restants du plan engagé **avant d'entamer de nouveaux processus** (Phase 5 Documents / Agenda). À faire en premier : relever l'état réel de chaque lot.
- [ ] **Éditeurs de texte riche** (`UC_RichTextEditor` et `UC_RichTextEditorSimple`) : révision du design et des icônes de la barre d'outils, et revue du code.
  - Composants importants (anamnèse, notes de séance, alertes).
  - **Audit et maquette faits le 19/10/2026** (board « Éditeurs de texte — refonte » du canevas) ;
  - choix de Joëlle : un seul contrôle à deux profils (Complet / Compact), styles de paragraphe sans police ni taille, **G** pour gras, correcteur d'orthographe oui.
  - [x] **Temps 1** ✅ 19/10/2026 (compilé et essayé par Joëlle ; plan `PLAN_TESTS_UC_RICHTEXTEDITOR` V2.0, RTE-01 à RTE-93, à dérouler au fil des essais ; ADR-42) :
    - corrections (boucle des listes de police, mise en page ignorée à l'impression, Ctrl+I qui insère une tabulation, icônes jamais grisées, barre masquée en consultation, `MessageBox` → `DialogChoix` + journal, RTF illisible journalisé) ;
    - profil Compact ;
    - barre dessinée dans le thème, icônes redessinées ;
    - styles, barré, couleur et surligneur de la charte, listes, alignements, date et horodatage ;
    - rechercher / remplacer, aperçu, export, collage sans mise en forme, menu contextuel, ligne d'état.
  - [x] **Alerte avant de quitter sans enregistrer** ✅ 19/10/2026 (validé par Joëlle) (demande de Joëlle, 19/10/2026 ; recommandations acceptées)
    - code fait le 19/10/2026, compilé et testé par Joëlle (`PLAN_TESTS_KIT_ALTHEA` section 8, KM-01 à KM-20 ; ADR-43 ; `GardeModifications`)
    - étendu le 19/10/2026 à toutes les saisies (fiche du dossier, Domaines, référentiels, Cabinet, Paramètres, fenêtres Paiement et Utilisateur) et aux boutons Annuler (« Abandonner / Continuer la saisie »), demande de Joëlle : comportements identiques dans toute l'application :
      - dialogue à trois boutons **Enregistrer / Quitter sans enregistrer / Rester** ;
      - tenu par chaque écran pour **tous** ses champs (l'éditeur expose seulement son état modifié) ;
      - « photo » des valeurs au chargement comparée au départ.
    - Fiche patient : anamnèse au changement d'onglet, au retour, à la navigation de Home et à la fermeture d'Althéa (`IUnsavedChangesAware`), fiche en modification ;
    - fenêtres Séance, Contact, Intervenant, Thérapeute ;
    - Zone du dossier alignée sur le dialogue à trois boutons.
    - Avec, relevé par Joëlle le 19/10/2026 : **Domaines**, Modifier seulement sur l'étape 1 (Général) et consigne « se modifie directement » sur les étapes 2 à 4 (`UC_DomainesHome` V1.9.0 ; `PLAN_TESTS_DOMAINES` D-89, D-90).
  - [x] **Temps 2** ✅ 20/10/2026 — les quatre chantiers faits et testés (plan validé par Joëlle le 19/10/2026 : un chantier = une étape testée puis commitée, dans cet ordre) :
    - [x] **« Ouvrir en grand »** du profil Compact ✅ 19/10/2026 (testé par Joëlle) (`PLAN_TESTS_UC_RICHTEXTEDITOR` section 11, RTE-94 à RTE-99) :
      - bouton à droite de la barre compacte, fenêtre `EditeurTexteAgrandi` (profil Complet, sans aperçu ni impression ni export), Valider reporte le texte dans le champ ;
      - éditeurs compacts titrés (`TitreDocument`).
    - [x] **En-tête et pied de page** ✅ 19/10/2026 (testé par Joëlle) à l'impression, à l'aperçu et au PDF
      - choix de Joëlle : en-tête cabinet et praticien, titre du document, patient (nom et code) ; pied « Confidentiel », date d'impression, page n / N.
      - Code fait le 19/10/2026, migration v2.41.0 appliquée, compilé et testé (`PLAN_TESTS_UC_RICHTEXTEDITOR` section 12, RTE-100 à RTE-107) : `EnTeteDocument`, `RichTextEditorHelper` V2.1.0 (impression, aperçu, PDF et Word), mention paramétrable `DOCUMENT_MENTION_CONFIDENTIALITE` (carte « Documents imprimés »).
    - [x] **Textes types** ✅ 19/10/2026 (testé et approuvé par Joëlle) :
      - référentiel en base (migration), chaque texte avec un **usage** (anamnèse, notes de séance, zones du dossier… liste en base) ou « partout » ;
      - l'éditeur ne propose que ceux de sa zone (choix de Joëlle).
      - Code fait le 19/10/2026, migration v2.42.0 appliquée, testé (`PLAN_TESTS_UC_RICHTEXTEDITOR` section 13, RTE-110 à RTE-118 ; ADR-44) : `UC_TextesTypes` (tuile du groupe Dossier), bouton « Textes types » des éditeurs (`UsageTextesTypes`).
      - Pistes non retenues pour l'instant : champs à remplacer dans un texte (date, patient), lien « Gérer les textes types… » depuis le menu de l'éditeur.
      - [x] **Compléments** ✅ 19/10/2026 (testé par Joëlle) (propositions 1 à 4 acceptées par Joëlle, 19/10/2026) — migration v2.43.0 appliquée (RTE-119 à RTE-124) :
        - plusieurs usages par texte, usage pour tous les éditeurs (alerte, commentaires séparés), écran des usages, règle « tout éditeur reçoit son usage ».
        - Modèles de documents de Pearl : plus tard, dans la gestion des documents (les textes types en couvrent une partie).
    - [x] **Correcteur d'orthographe** ✅ 20/10/2026 (testé par Joëlle) :
      - essai concluant le 20/10/2026 → **correcteur de Windows** (ADR-45) ;
      - partout en saisie, réglage dans Paramètres, suggestions / ajouter au dictionnaire au clic droit (choix de Joëlle ; « Ignorer » essayé puis retiré, incohérent).
      - Migration v2.44.0 appliquée, testé (`PLAN_TESTS_UC_RICHTEXTEDITOR` section 14, RTE-130 à RTE-137).
      - Grammaire : hors de portée (ni Windows ni Syncfusion).
  - [x] **Champs de notes encore en zone de texte Windows** ✅ 20/10/2026 (testé par Joëlle) — décidé et fait le 20/10/2026 (`PLAN_TESTS_UC_RICHTEXTEDITOR` section 15, RTE-140 à RTE-148) (demande de Joëlle, 14/10/2026 — à décider **après** cette révision) :
    - pour chaque champ de notes / commentaire resté en `TextBox`, décider s'il reste ainsi ou passe en `UC_RichTextEditorSimple`.
    - Passer en éditeur riche demande une modification de la base pour stocker les deux formes : texte brut (recherche) et RTF (affichage), comme `commentaire_txt` / `commentaire_rtf`.
    - Relevé du 14/10/2026 : `PaiementEditor.txtCommentaire`, `DialogMotifAnnulation.txtMotif` (motif d'annulation) ; les commentaires de `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition` et `SeanceEditor` sont déjà en éditeur riche (txt + rtf en base).
    - À compléter par un relevé des champs créés par le code.
    - **Décision de Joëlle (20/10/2026, recommandation acceptée)** : `PaiementEditor.txtCommentaire` et `DialogMotifAnnulation.txtMotif` **restent en texte simple** — Althéa complète le commentaire d'un paiement en SQL (« [Annulé le … : motif] », utilisation d'un avoir), ce qu'un RTF ne supporterait pas — et reçoivent le **correcteur d'orthographe** (`AltheaZoneTexte`, code commun `CorrecteurTexte`) ;
    - le motif n'est plus journalisé.
    - Relevé des champs créés par le code (20/10/2026) : « Procédure » de `UC_Organismes` (mémo de référentiel) et zones de `UC_Parametres` : zones de texte, sans correcteur ; détail de `UC_Journaux` : lecture seule.
- [ ] **Annuler une utilisation d'avoir** (possible depuis la liaison `lia_paiements_avoirs`, v2.52.0 ; à décider) : aujourd'hui un règlement par avoir ne s'annule pas. La liaison dit exactement quel montant rendre à quelle affectation source : l'annulation rendrait l'avoir à son encaissement d'origine, en une transaction.
- [ ] **Commentaire d'un paiement invisible** (constat du 22/10/2026) : le commentaire saisi dans la fenêtre d'encaissement est enregistré mais ne s'affiche nulle part (ni grille, ni infobulle, ni détail des paiements). À afficher (infobulle de la ligne, détail des paiements).
- [ ] **Ajout de référentiels par un User, à la volée** (remarque de Joëlle, 22/10/2026, à cadrer) :
  - aujourd'hui le bouton [+] d'une fiche demande à un User de s'élever en SuperUser avant d'ajouter une valeur manquante (pays, civilité, lien patient…), ce qui interrompt la saisie.
  - À décider :
    - quels référentiels (de saisie seulement, pas ceux qui pilotent des règles : statuts, tarifs, modes de paiement…),
    - ajout seulement (modifier et désactiver restent SuperUser),
    - garde-fou contre les doublons ;
  - règle à porter par `ReglesReferentiels` (rôle de création par référentiel) et `OuvrirReferentielModal`.
- [x] **Lignes système devenues inutiles** (décision de Joëlle, 24/10/2026) — fait le 25/10/2026 (v2.58.0, ADR-54) : `ABSENTE`, `ANNULEE_PATIENT`, `ANNULEE_PRATICIEN`, `MALADE`, `NO_SHOW`, `MERE`, `AUTORITE_PARENTALE` déverrouillés ; restent `PLANIFIEE`, `REALISEE`, `REPORTEE`, statuts de dossier, types de document, `AVOIR`, `INAMI`.
- [ ] **Planning du jour pas rafraîchi dans le dossier** (constat de Joëlle, 24/10/2026, test C3-11 ; affichage seulement, non bloquant) : une séance passée à un statut qui libère le créneau reste affichée dans le Planning du jour d'une nouvelle séance créée sans quitter le dossier ; en changeant de patient, elle disparaît. L'enregistrement de la nouvelle séance passe bien (contrôle métier relu en base). Piste : la liste du planning vient des séances déjà chargées du dossier plutôt que d'une relecture.
- [x] **Statut de séance ajouté : lui donner un comportement** (remarque de Joëlle, 22/10/2026 — à traiter avec l'audit **C.3**, drapeaux de référentiel) — fait le 24/10/2026 (v2.57.0, ADR-54) : case « Non tenue, libère le créneau » lue partout ; Maladie la reçoit au départ ; l'absence non excusée garde son créneau. Constat d'origine :
  - un statut ajouté ne peut aujourd'hui que être dû ou non, consommer le quota ou être une absence non excusée.
  - Ce qui rend une séance « non tenue » est écrit dans le code, d'après le code du statut, en **trois listes qui ne concordent pas** :
    - créneau libéré (`GestionSeances.CodesStatutLiberantCreneau` : annulées et reportée — **pas Maladie**, qui occupe donc toujours son créneau dans le planning et le contrôle de chevauchement),
    - pas de contrôle d'horaire entre statuts non tenus (`EstStatutNonTenu` : annulées, Maladie, Reportée, absences),
    - `Seance.EstAnnulee` (annulées, Maladie, Absence ; plus lue nulle part).
  - Proposition : une case « Séance non tenue (libère le créneau) » dans le référentiel, lue partout à la place des trois listes, Joëlle décidant pour chaque statut (Maladie compris) ;
  - Reportée reste à part (circuit du report).
- [x] **Codes postaux belges : code existant et localité correspondante** (demande de Joëlle, 23/10/2026 ; **après le lot C.2**) — fait le 24/10/2026 (v2.56.0, ADR-53 : bpost + registre BeST, localité remplie et suggérée, code trouvé depuis la localité, avertissement puis confirmation, mise à jour par script), validé le 24/10/2026 (PAT-113 à PAT-129, Cabinet 4.15 à 4.18) :
  - liste officielle de bpost (≈ 2 800 codes, localités et sections de commune en français, néerlandais et allemand) chargée dans un référentiel (`ref_codes_postaux_be`, migration) ;
  - en Belgique, code inexistant refusé, localité **proposée** dès la saisie du code, localité différente des écritures officielles du code **signalée** (avertissement, pas refus : Bruxelles / Brussel, St- / Saint-…) ;
  - fiches patient, contact, thérapeute, cabinet et praticien ;
  - mise à jour de la liste quand bpost la change.
- [ ] **Ligne choisie avant d'agir, dans toutes les grilles** (relevé de Joëlle au test P-93, 22/10/2026 ; fait pour les onglets Séances et Paiements du dossier) :
  - une grille pose d'office sa première ligne en ligne courante, et les écrans qui lisent `CurrentRow` agissent sur elle sans qu'elle ait été choisie (Patients, contacts, intervenants et dossiers de la fiche patient, Utilisateurs, Thérapeutes, référentiels, Journaux).
  - À généraliser avec `UtilsDataGrid.ElementSelectionne` (principe : même comportement partout) — à confirmer écran par écran (une liste à ligne unique peut garder la sélection d'office).
- [ ] **Recherche transversale** (ADR-52, plan validé par Joëlle le 23/10/2026 :
  - R1 code — validé ;
  - R2 séances d'un jour ou d'une période — validé le 23/10/2026 ;
  - R3 paiements d'un jour ou d'une période (relevé du dossier, filtre par mode, totaux encaissés par mode — décisions de Joëlle) — validé le 23/10/2026 ;
  - R4 patients par lieu (décisions de Joëlle, 24/10/2026 : zone code postal ou localité, liste des lieux et leurs patients, inactifs comme l'écran Patients) — validé le 24/10/2026 (RCH-40 à RCH-50) ;
  - R5 réseau par nom — contact, thérapeute ou intervenant (nom ou début de nom) : sa fiche et les patients qui en relèvent (idée de Joëlle, 23/10/2026 ; décisions du 24/10/2026 : une ligne par personne, patient + fiche de la personne, suivis terminés inclus et signalés) — validé le 24/10/2026 (RCH-51 à RCH-66) ;
  - ensuite au fil des idées : documents et rendez-vous quand ces modules existeront, contacts, impayés…).
  - Idée d'origine (22/10/2026) : aujourd'hui, un paiement, une séance ou un dossier ne se retrouvent qu'en passant par leur patient (ou en SQL).
  - Un écran de recherche générale, avec des « pôles » (patients, dossiers, séances, paiements, documents plus tard) et des remontées transversales : à partir d'un paiement, retrouver sa séance, son dossier et son patient, et y aller d'un double-clic.
  - Points à décider au cadrage :
    - critères (codes PAI… / SE… / DO… / patient, date, montant, mode, statut),
    - droits (comme l'écran Patients),
    - confidentialité (aucun terme recherché dans le journal),
    - place dans le menu ou sur l'accueil (widget « Recherche » de la section H).
  - À placer après le lot C de l'audit (règles métier hors de l'écran), qui fournira les requêtes à réutiliser.
- [ ] **Réduction des très grandes images à l'import** (idée validée le 11/10/2026, pour plus tard) : ramener une photo de téléphone (12 Mpx) à une taille raisonnable lors de l'import — modifie le fichier copié, à décider (taille maximale, formats concernés, original conservé ou non).
- [ ] **Révision des icônes** : remplacer celles choisies faute de mieux ; viser un jeu cohérent (style, épaisseur de trait, palette Sauge).
- [ ] **Revue de code générale** : améliorations, oublis, sécurité, factorisation, code mort. À mener après le chantier UI, de préférence avant la Phase 5.
  - [ ] **Audit du 18/10/2026** (`Docs/Conception/RAPPORT_AUDIT_2026-10-18.md`) : tous les points passés en revue et corrigés, lot par lot (décision de Joëlle, 20/10/2026) ;
    - suivi point par point au § 9 du rapport.
    - Ordre :
      - B1-B2 (fait le 20/10/2026), logos des domaines, finitions du ToDo,
      - puis lot A (bugs et sécurité, fait les 20-21/10/2026),
      - lot B (intégrité ;
        - B.1 le 21/10/2026 ;
        - B.2 en cours : enregistrement d'une séance le 21/10/2026 (validé), report d'une séance (validé), réaffectation remplacée par l'avoir (validé), nouvelle règle de quota (validé), compte du praticien (validé) — lot B.2 terminé ;
        - B.3 « À confirmer » (ADR-47, validé) ;
        - B.4 vues SQL du solde et du dossier dominant (ADR-48, migration v2.49.0, validé) ;
        - B.5 en cours (ADR-49) : mode de paiement par identifiant (v2.50.0, validé), annulation en colonnes (v2.51.0, validé), utilisation d'avoir en liaison typée (v2.52.0, validé) — lot B.5 terminé ;
        - B.6 : un seul calcul du quota et règle relue après un changement de domaine (validé), case « Absence non excusée » (v2.53.0, validé) — lot B terminé le 22/10/2026),
      - lot C (découpage validé le 22/10/2026 : C.1 droits → C.2 règles → C.3 codes → C.4 découplage → C.5 découpage ; C.6 au module Documents) :
        - C.1 étape 1 `ContexteOperation` et garde `Autorisation` (ADR-50, validé),
        - C.1 étape 2 montant libre, séance encaissée et dossier figé côté métier (validé),
        - C.1 étape 3 référentiels : droit et lignes système côté métier et SQL (validé) — lot C.1 terminé le 22/10/2026 ;
        - C.2 (inventaire et découpage validés le 22/10/2026, ADR-51 : séances et paiements → patient et fiches liées → domaines → comptes) :
          - étape 1 séances et paiements (validé) ;
          - finitions des onglets Séances / Paiements (validé) ;
          - étape 2a fiche patient + NISS normalisé, v2.54.0 (validé) ;
          - étape 2b contacts, intervenants et thérapeutes, homonyme de thérapeute confirmé, fiche Intervenant complétée (validé) ;
          - spécialités des thérapeutes en référentiel, v2.55.0 (étape intercalée, décision de Joëlle — validé) ;
          - étape 3 domaines : saisie et complétude jugées par le métier (validé) ;
          - étape 4 comptes : règle du mot de passe et du login dans le métier (validé) — lot C.2 terminé le 23/10/2026 ;
        - ensuite recherche transversale (R1 à R5, validée) et codes postaux belges (validés), puis C.3 — découpage validé le 24/10/2026 (ADR-54) :
          - étape 1 codes système dans `CodesSysteme`, SQL bâti sur ces constantes (validé le 24/10/2026, C3-01 à C3-06) ;
          - étape 2 drapeaux de référentiel, v2.57.0 : séance non tenue, statut proposé à la création, lien et rôle légal par défaut d'un contact (fait et validé le 24/10/2026, C3-07 à C3-16) ;
          - suite : lignes système devenues inutiles déverrouillées, v2.58.0 (25/10/2026) ;
        - C.4 découplage des écrans de Home (ADR-55, décisions de Joëlle du 25/10/2026 : contrat IFenetrePrincipale, élévation réunie) :
          - étape 1 contrat, contexte, fenêtres (fait et validé le 25/10/2026, NAV-C4-01 à 05) ;
          - étape 2 écrans Patients, Dossier, Domaines, Cabinet, Utilisateurs, Thérapeutes (fait et validé le 25/10/2026, NAV-C4-06 à 10) ;
          - étape 3 recherches, hubs, élévation réunie, garde « modifications non enregistrées » vérifiée (fait et validé le 25/10/2026, NAV-C4-11 à 15) — lot C.4 terminé ;
        - C.5 (ADR-56, décisions de Joëlle du 25/10/2026) : fiches dossier et patient réparties en fichiers partiels par onglet (fait et validé le 25/10/2026, C5-01 à C5-04) — lot C.5 terminé ;
      - lot D, ordre décidé par Joëlle le 25/10/2026 :
        - grands documents scindés : doc UI par domaine, processus, CHANGELOG limité au mois en cours (fait le 25/10/2026, Rules § 13) ;
        - [x] D-1 `Option Strict On` pour tout le projet et `.editorconfig` (ADR-57) — terminé le 25/10/2026 :
          - étape 1 `Option Strict On` : inventaire, 12 corrections, bascule du projet (fait et validé le 25/10/2026, D1-01 à D1-08) ;
          - étape 2 `.editorconfig` à la racine et analyseurs .NET « Recommended » (fait et validé le 25/10/2026, D1-09, D1-10) ;
        - [ ] Analyseurs, cas à corriger en priorité puis règles à durcir (`warning`) : `UC_Cabinet.LireQualifications` (TryParse non vérifié, CA1806) ; comparaisons de textes sans mode précisé (CA1304, CA1309, CA1310 : `UC_DomainesHome`, `UC_LiaisonsNaturesDomaines` ×2, `UtilsIdentifiantBelge`, `TherapeuteEdition`, `UtilsDataGrid`, `UC_RichTextEditor`) ; `TestKitAlthea.Planche` (champ supprimable jamais libéré, CA1001). Les autres suggestions au fil des passages dans les fichiers ;
        - [x] D-2 tests automatisés — terminé le 26/10/2026 (ADR-58, décisions de Joëlle du 26/10/2026 : VB, xUnit v3, `Friend` + `InternalsVisibleTo`, quota alimenté par une fonction, base vide `althea_tests`) :
          - étape 1 socle (`Tests/Althea.Tests`, `Tools\test.cmd`) et 154 tests — identifiants, sécurité, saisie (fait et validé le 26/10/2026, D2-01 à D2-03) ;
          - étape 2 interprètes Date/Heure (`Friend`) et calcul du quota INAMI (règle fournie par une fonction) — 229 tests (fait et validé le 26/10/2026, D2-04 à D2-06) ;
          - étape 3 test de fumée des requêtes `Query*` sur la base vide `althea_tests` (`ALTHEA_TESTS_CONNEXION`, `Maintenance/creer_base_tests.sql`) — 389 requêtes, 621 tests (fait et validé le 26/10/2026, D2-07, D2-08) ;
        - [ ] D-3 intégration continue (ADR-59, décision de Joëlle du 26/10/2026 : une fois par jour le soir s'il y a eu un commit, et à la demande) — `.github/workflows/ci.yml` en place le 26/10/2026 ; premier passage au vert le 26/10/2026, 2 min (D3-01) ; reste à constater le passage du soir (D3-02) ;
        - [ ] installation et migrations, finition UI, reste de la documentation : après Agenda, Documents et Accueil.
  - [ ] **Sécurité** : mot de passe en clair dans l'outil de test « Hash mot de passe » (`UC_Accueil`, Debug seulement) — remplacer par une saisie ; changer le mot de passe du SuperUser s'il s'agit du vrai (présent dans l'historique Git).
  - [x] **Sécurité** (15/10/2026) : identifiants en clair retirés de `Login` ; le pré-remplissage Debug lit `ALTHEA_DEV_LOGIN` / `ALTHEA_DEV_PASSWORD` (variables du poste). Mot de passe changé par Joëlle (l'ancien reste dans l'historique Git).
  - [ ] **Fin de développement** : supprimer `PreRemplirIdentifiantsDev` (`Login`) et les deux variables d'environnement du poste.
  - [x] **Sécurité** ✅ 18/10/2026 (fait : `LicenceSyncfusion`, ADR-41, saisie dans Paramètres) : clé de licence Syncfusion dans `Program.vb` — la sortir du code (configuration ou variable d'environnement).
  - [ ] Noms internes encore marqués « Inami » (`QuotaInamiInfo`, `EstDomaineInami`…) alors que les quotas dépendent désormais de l'organisme du domaine.

---

## H. Idées pour la suite (post-V1)

**1. Sur l'accueil, on pourrait trouver des genres de Widgets** → planifié : socle et premiers widgets au lot 31, widgets Agenda/Documents après la Phase 5.
- RV du jour
- Nouveau RV à ajouter (Quick)
- Recherche patient (Quick)
- Dernier patient/dossier traité
- Todo (il faudra en introduire la notion dans l'appli)
- Lien avec le site Web de Pearl (et peut-être d'autres liens dont elle peut avoir besoin souvent)
- Notifications / alertes importantes
- On pourrait aussi introduire la notion de "favoris" pour les écrans les plus utilisés, avec un accès rapide depuis l'accueil ou une barre latérale.
- Dashboard avec statistiques (nombre de patients actifs, séances du mois, chiffre d'affaires, etc.)

**2. Lien avec le site Web**
Il existe un site web pour Pearl : https://pearlnguyenduy.be/

- Dans ce site, Pearl publie des articles. Il faudrait un système de publication simple et rapide à partir de l'appli.
- L'url a été acheté sur Combell et a été rapatrié sur Combell, depuis Netlify. Il est maintenant géré ici en local.
- Hebergé chez Combell - Versionné sur GitHub - Backend Supabase
     - Repos local : D:/source/repos/site_pearl_clean
     - Repos GitHub Privé : https://github.com/AngeljoNG/pearlnguyenduy-site
     - DEV local  http://127.0.0.1:5173/                   --> pearl-test                -->  Developpement sans impact prod 
     - TEST          https://test.pearlnguyenduy.be/   --> pearl-test                 -->  Validation hebergee Combell    
     - PROD        https://pearlnguyenduy.be/           --> pearl-nguyen-blog    -->  Site public                    
-  Combell : https://my.combell.com/fr/product/webhosting/linux/pearlnguyenduy.be/websites
- Databases sur Combell : https://my.combell.com/fr/product/webhosting/linux/pearlnguyenduy.be/databases
- 2 jobs cron pour la sauvegarde de la base de données depuis Supabase : Blog et Avis/Remerciements.

**3. IA**
- Je voudrais intégrer une IA dans l'appli, pour aider à la rédaction de documents, de notes, de synthèses, etc…
- Il faudrait aussi que cette IA puisse être utilisée pour faire des analyses de données, des prévisions, etc… à partir des données de l'appli (patients, dossiers, séances, paiements, etc…)
- Suggestions automatiques de diagnostics ou d'observations basées sur les notes de séances
- Génération automatique de synthèses de suivi patient
- Analyse de tendances (évolution du nombre de patients, types de consultations, etc.)

**4. Améliorations UX/UI**
- Mode sombre / clair
- Personnalisation des thèmes
- Raccourcis clavier avancés
- Multi-écrans / multi-fenêtres pour productivité
- Barre de recherche globale (patients, dossiers, documents, paramètres)

**5. Fonctionnalités métier avancées**
- Génération automatique de factures
- Gestion des mutuelles et remboursements
- Statistiques avancées et rapports personnalisés
- Export données pour comptabilité
- Gestion de la TVA
- Archivage automatique des dossiers anciens
- Alertes sur rendez-vous non confirmés
- Suivi des impayés

**6. Sécurité et conformité**
- ~~Audit trail complet (qui a fait quoi, quand)~~ → remonté en section E le 16/10/2026 (« Journal des modifications », avant la mise en service)
- Anonymisation/pseudonymisation des données pour RGPD
- Export/suppression données patient (droit à l'oubli)
- Chiffrement renforcé des données sensibles
- Authentification à deux facteurs (2FA)
- Sauvegarde cloud sécurisée (optionnelle)
- **Licence de l'application** (question de Joëlle, 22/10/2026 — seulement si Althéa est un jour distribuée à d'autres cabinets) :
  - fichier de licence **signé** par une clé privée gardée par l'éditrice (Joëlle), vérifié par la clé publique incluse dans l'application ;
  - contenu : cabinet, dates de validité, nombre de comptes par rôle, nombre de postes, modules ouverts ;
  - activation hors ligne possible ;
  - limites vérifiées côté métier (avec `Autorisation`, lot C.1).
  - **Pas de compte « super-admin » caché** hors licence : sur des données de santé, ce serait une porte dérobée (RGPD, secret professionnel) ;
  - à la place, un accès de maintenance ouvert par le cabinet, limité dans le temps et journalisé.

**7. Mobilité**
- Application mobile compagnon (consultation agenda, fiche patient simplifiée)
- Synchronisation locale/cloud
- Mode hors-ligne avec sync automatique

---

**Note importante** : Ces idées seront évaluées et priorisées après la livraison de la V1 stable.

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

