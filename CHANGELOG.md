# 📌 **Althéa - CHANGELOG**

>  *Dernière mise à jour : 23/07/2026*

## 📅 23/07/2026

### 🧹 Refactorisation : centralisation des helpers d'accès aux données (`DbHelper`)

#### Contexte
- Les fonctions utilitaires de lecture (`Lire*`) et de conversion vers paramètre SQL (`Valeur*`) étaient **dupliquées à l'identique** dans 4 modules métier (`GestionPatients`, `GestionFamilleContacts`, `GestionSuivisIntervenants`, `GestionTherapeutes`), soit ~21 fonctions redondantes.

#### Nouveau module centralisé
- **`Core/Database/DbHelper.vb`** (nouveau, `Public Module`) : regroupe 10 fonctions partagées appelables sans qualification depuis toute la couche métier.
  - Lecture : `LireString`, `LireInt`, `LireBool`, `LireDateNullable`, `LireLongNullable`, `LireULongNullable`.
  - Écriture : `ValeurOuDBNull`, `ValeurDateOuDBNull`, `ValeurLongOuDBNull`, `ValeurULongOuDBNull`.

#### Suppression des doublons
- **`Metier/Referentiels/GestionTherapeutes.vb`** : −3 helpers privés (`LireString`, `LireBool`, `ValeurOuDBNull`).
- **`Metier/Patients/GestionFamilleContacts.vb`** : −6 helpers privés (`LireString`, `LireInt`, `LireBool`, `LireDateNullable`, `ValeurOuDBNull`, `ValeurDateOuDBNull`).
- **`Metier/Patients/GestionSuivisIntervenants.vb`** : −5 helpers privés (`LireString`, `LireULongNullable`, `LireDateNullable`, `ValeurOuDBNull`, `ValeurDateOuDBNull`) ; `ValeurULongOuDBNull` promu dans `DbHelper`.
- **`Metier/Patients/GestionPatients.vb`** : −7 helpers privés (`LireString`, `LireInt`, `LireDateNullable`, `LireLongNullable`, `ValeurOuDBNull`, `ValeurDateOuDBNull`, `ValeurLongOuDBNull`).

#### Note technique (VB.NET)
- Les membres `Private` d'un `Module` restent visibles pour la résolution des appels **non qualifiés** des autres modules : la migration ne pouvait donc pas se faire fichier par fichier. Tous les doublons ont été retirés en une seule passe pour ne laisser qu'un unique candidat `Public` dans `DbHelper`. Aucun site d'appel n'a eu besoin d'être modifié.

#### Build & qualité
- ✅ Build réussi. Encodage UTF-8 avec BOM conservé sur tous les fichiers `.vb` (nouveau module inclus).

---

## 📅 21/07/2026

### 🩺 P3 - Patients : filtre de statut de suivi (`suivi_en_cours`) et icône d'état dans la liste

#### Brique 1 - Champ `suivi_en_cours` propagé du modèle à la liste
- **`Metier/Patients/PatientListeItem.vb`** : nouvelle propriété `SuiviEnCours As Boolean` (1 = suivi en cours, 0 = clôturé/archivé).
- **`Core/Database/Queries/QueryPatients.vb`** : `p.suivi_en_cours` ajouté au `SELECT` et au `GROUP BY` de `SelectPatientsListe`.
- **`Metier/Patients/GestionPatients.vb`** : mapping `MapListeItem` enrichi (`.SuiviEnCours = LireInt(reader, "suivi_en_cours") <> 0`, robuste pour un `TINYINT(1)`). La valeur par défaut à la création reste gérée par la base (`DEFAULT 1`, aucun champ fourni à l'INSERT applicatif).

#### Brique 2 - Filtre de suivi à 3 états dans `UC_PatientHome`
- **`UI/Controls/Patients/UC_PatientHome.vb`** + **`.Designer.vb`** : remplacement de la case `chkAvecDossiersActifs` (basée sur `NbDossiersActifs`, inopérante tant que les dossiers ne sont pas implémentés) par une **ComboBox `cboFiltreSuivi`** à choix exclusif : **« Suivi en cours »** (défaut, `suivi_en_cours = 1`), **« Suivi clôturé / archivé »** (`= 0`), **« Tous »** (sans filtre de statut). Constantes d'index `FiltreSuiviEnCours` / `FiltreSuiviCloture` / `FiltreSuiviTous` ; remplissage via `InitialiserFiltres()`.
- **`AppliquerFiltres()`** : filtrage en mémoire par `Select Case` sur le statut, cumulable avec la recherche texte. Le bouton **Réinitialiser** revient à l'état par défaut (texte vide + « Suivi en cours »).
- **Mini-pile D-Q15** : `CreerEcranCourant()` / `RestaurerFiltre()` / `AppliquerFiltreInitial()` migrés de `Boolean` (dossiers actifs) vers un index entier (statut de suivi), avec garde de bornes à la réinjection.

#### Brique 3 - Colonne icône de statut dans la grille
- **`UI/Controls/Patients/UC_PatientHome.Designer.vb`** : colonnes `colNbDossiers` / `colNbDossiersActifs` retirées ; nouvelle colonne image **`colStatutSuivi`** placée en **première position** (largeur 50, non redimensionnable, centrée, `NullValue = Nothing`).
- **`dgvPatients_CellFormatting`** (V1.1.0) : rendu de l'icône selon `PatientListeItem.SuiviEnCours` via `UtilsIcons.IconPatientEnCours()` / `IconPatientNonEnCours()`.
- **`My Project/Resources.resx`** + **`Resources.Designer.vb`** : déclaration des icônes `patientEncours_20` et `patientNonEncours_20` (depuis `Assets/Tech_Ico`).
- **`Utils/UI/UtilsIcons.vb`** : helpers `IconPatientEnCours()` / `IconPatientNonEnCours()` (icônes 20x20).

### RichTextEditor et RichTextEditorSimple
- Changement des icones dans la boîte à outil deouis /Assets/Editor_ico (export PDF / Word, copier/coller, gras/italique/souligné, puces, alignement etc..) - 24x24 ou 22x22 selon l'outil dans RichTextEditor ou 18x18 dans RichTextEditorSimple.
- ajout d'un outil "Couleur" dans RichTextEditorSimple

#### Documentation
- **`Docs/UI/Documentation_technique_UI_Althea.md`** : section `UC_PatientHome` actualisée (responsabilités, variables privées, contrôles, colonnes, méthodes et points d'attention) pour refléter le filtre de suivi et la colonne d'état.

#### Build & qualité
- ✅ Build réussi. Encodage UTF-8 avec BOM conservé sur tous les fichiers `.vb` modifiés.

---

## 📅 19/07/2026

### 🤝 I2 - Onglet Intervenants : flux thérapeute « friendly » + harmonisation du contexte UI des modales

#### Brique 1 - Ajout / consultation des thérapeutes depuis `IntervenantEdition`
- **`UI/Forms/Home.vb`** : ajout de `OuvrirCreationTherapeuteModal()` - ouvre directement `TherapeuteEdition` en création (avec gating de droits/élévation et injection du contexte UI partagé), puis retourne le `Therapeute` créé (ou `Nothing`). Évite le détour par l'écran de liste.
- **`UI/Forms/Patients/IntervenantEdition.vb`** + **`.Designer.vb`** : le bouton `[+]` (`btnAjouterTherapeute`) ouvre désormais **directement** la création d'un thérapeute et présélectionne le thérapeute créé dans le combo. Ajout d'un second bouton **`btnVoirTherapeutes`** (icône `recherche_24` via `UtilsButtons.InitDiversIconButton`) qui ouvre la **liste** `UC_Therapeutes` pour consulter / compléter une fiche, puis recharge le combo en préservant la sélection. Le bouton « voir liste » reste actif même en consultation (consultation pure, non destructive).
- **`UI/Forms/Communs/ReferentielModalHost.vb`** : ajout d'une barre d'actions basse (`DockStyle.Bottom`) avec un bouton **« Fermer »** (style standard sauge, `Tag = fermer_normal`) pour un retour explicite vers l'écran appelant lorsqu'aucun enregistrement n'est effectué (parcours / consultation d'un référentiel en contexte). Bouton ancré à droite et repositionné dynamiquement au redimensionnement ; touche **Échap** (`CancelButton`) ; `DialogResult.Cancel` à la fermeture. L'enregistrement réussi continue de fermer automatiquement la fenêtre.

#### Brique 2 - Harmonisation du contexte UI des modales (`ttMain` / `stsStatus`)
- **Constat d'audit** : les `UserControls` (`UC_*`) utilisent déjà tous le contexte partagé `UserControlContext` (`_context`). Les `ToolTip` / `ErrorProvider` locaux restants ne subsistent que dans `Home` (propriétaire des composants partagés), dans les forms autonomes de connexion / sécurité (`Login`, `ChangePassword`, `ElevationAcces`, `ConfigurationConnexion` - ouvertes hors contexte Home) et dans les 4 modales `ShowDialog`.
- **`UI/Forms/Patients/ContactEdition.vb`**, **`IntervenantEdition.vb`**, **`TherapeuteEdition.vb`** : implémentent désormais `IContextAwareForm` (champ `_context` + `SetContext`). L'initialisation des infobulles est déplacée du constructeur vers un handler `Load` (le contexte n'est injecté qu'après construction). Les infobulles passent par un helper `DefinirToolTip` qui privilégie `_context.SetToolTip` avec **repli** sur le `ToolTip` local si aucun contexte n'est injecté. Ajout d'un message de statut de confirmation à l'enregistrement (`_context.SetStatus`), affiché dans la barre de statut partagée de `Home`.
- **`UI/Forms/Utilisateur/UtilisateurEdition.vb`** : ses infobulles passent par le même helper `DefinirToolTip` (déjà `IContextAwareForm`, init en `Load`).
- **`errProvider` conservé local** dans les 4 modales : les icônes d'erreur doivent se positionner sur la fenêtre modale elle-même, pas sur `Home` en arrière-plan (`ContainerControl` distinct).
- **`UI/Controls/Patients/UC_PatientFiche.vb`** + **`UI/Controls/Referentiels/UC_Therapeutes.vb`** : injection de `SetContext(_context)` avant `ShowDialog` aux points d'ouverture des modales (`ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`), pour que les infobulles et le statut transitent par le contexte partagé.

#### Build & qualité
- ✅ Build réussi. Encodage UTF-8 avec BOM conservé sur tous les fichiers `.vb` modifiés.

---

## 📅 18/06/2026

### 🧭 P2 - Patients : navigation contextuelle, périmètre des actions et consultation d'abord

#### Brique 1 - Fil d'Ariane dynamique de la fiche patient
- **`UC_PatientFiche.vb`** : ajout de `RafraichirContexteNavigation()` - met à jour l'en-tête de navigation de `Home` (`UserControlContext.SetHeader`) au format `Patients > {patient} > {onglet}`. Le segment patient vaut « Nouveau patient » tant qu'aucun `id_patient` n'est obtenu ; le segment onglet provient de `tabFiche.SelectedTab.Text`. N'écrit pas dans `stsStatus` (réservé aux messages d'action).

#### Brique 2 - Périmètre des actions « niveau fiche »
- **`UC_PatientFiche.vb`** : ajout de `AppliquerVisibiliteActionsFiche()` - les boutons maîtres (Nouveau / Modifier / Enregistrer / Annuler) ne s'affichent que sur l'onglet **Identité**, selon le mode (consultation/édition), pour éviter la confusion d'un double panneau d'actions. Le bouton **`btnFermer`** est renommé « Retour liste » et reste visible sur tous les onglets.

#### Brique 3 - Édition locale de l'anamnèse
- **`Core/Database/Queries/QueryPatients.vb`** + **`Metier/Patients/GestionPatients.vb`** : ajout d'une mise à jour ciblée `UpdateAnamnesePatient(idPatient, anamneseRtf, anamneseTxt)` (colonnes `anamnese_rtf` / `anamnese_txt` uniquement), sur le modèle de `UpdatePhotoPatient`.
- **`UC_PatientFiche.vb`** + **`.Designer.vb`** : panneau local `pnlAnamneseActions` (Modifier / Enregistrer / Annuler) et `AppliquerModeAnamnese()` - cycle d'édition local indépendant du mode global. L'édition locale n'est proposée qu'en **consultation** d'un patient enregistré ; en Création / Modification globale, l'anamnèse est éditable directement (flux global) et le panneau local reste masqué. Suivi des modifications locales (`_modeEditionAnamnese`) avec avertissement sur « Retour liste » si des changements sont en attente ; le changement d'onglet n'avertit pas (persistance conservée).

#### Brique 4 - Consultation d'abord pour les dialogues Contacts / Intervenants
- **`UI/Forms/Patients/ContactEdition.vb`** + **`.Designer.vb`** : ajout des boutons **`btnModifier`** / **`btnFermer`** dans `pnlAction`, du champ `_consultation` et des méthodes `AppliquerMode()` / `DefinirEtatChamps(editable)`. Le constructeur reçoit un paramètre `Optional ouvrirEnConsultation As Boolean = False`. En consultation, tous les champs et boutons annexes (`[+]` lien/rôle, copie d'adresse) sont verrouillés ; le commentaire passe en `ReadOnlyMode`. Icônes des nouveaux boutons chargées au runtime via `UtilsButtons.InitStandardButton` (Assets `modifier_normal` / `fermer_normal`, sans modification des `.resx`).
- **`UI/Forms/Patients/IntervenantEdition.vb`** + **`.Designer.vb`** : même refactor consultation-first. Les champs snapshot (`txtNomProfessionnel`, `txtSpecialite`, `txtLieu`, `txtTelephone`) restent `ReadOnly` en permanence (trace pilotée par la sélection du thérapeute), seuls les vrais contrôles de saisie (`cboTherapeute`, `cboRole`, dates, commentaire) et les boutons `[+]` sont verrouillés/déverrouillés selon le mode.
- **`UC_PatientFiche.vb`** : `OuvrirEditionContact` et `OuvrirEditionIntervenant` propagent `ouvrirEnConsultation` au dialogue. Le **double-clic** sur une grille ouvre en consultation (`ouvrirEnConsultation:=True`) ; les boutons **Ajouter** (création) et **Modifier** de la grille conservent l'ouverture directe en édition.
- **Tooltips** : `btnModifier` (« Passer en modification… ») et `btnFermer` (« Fermer la consultation sans modifier. ») renseignés sur les deux dialogues.

#### Documentation
- **`Docs/UI/Documentation_technique_UI_Althea.md`** : sections `ContactEdition` et `IntervenantEdition` enrichies (rôle général, contrôles `btnModifier`/`btnFermer`, méthodes `AppliquerMode`/`DefinirEtatChamps`, point d'attention « consultation d'abord »).
- **`Docs/UI/Process_Althea.md`** : processus Contacts / Intervenants complétés avec le cycle consultation → Modifier → édition.
- **`Docs/Rules/ARCHITECTURE_DECISIONS.md`** : ADR-10 étendu (modes d'édition distincts généralisés aux dialogues modaux patients via le motif « consultation d'abord »).

#### Build & qualité
- ✅ Build réussi. Encodage UTF-8 avec BOM conservé sur tous les fichiers `.vb` modifiés.

---

## 📅 15/06/2026

### 🤝 I1 - Réseau d'intervenants du patient + référentiel Thérapeutes (migration v2.4 - lots 1 & 2)

#### Migration de schéma - Référentiel Thérapeutes et liaison N-N
- **Table `therapeutes`** (référentiel *entité riche*) : identité complète (nom, prénom, spécialité), coordonnées country-aware (téléphone, e-mail, adresse, code postal, localité, pays), `actif` (soft-delete) et `commentaire`.
- **Table `ref_role_legal`** : référentiel des rôles légaux des contacts (autorité parentale, représentant légal, …), `code` (max 30) / `libellé`, `ordre_affichage`, `actif` - rôle minimum d'accès `SuperUser`.
- **Table de liaison N-N `autres_suivis_patient`** (D-Q1bis) : rattache un patient à un thérapeute du référentiel, avec rôle d'intervenant (`ref_roles_intervenant`, optionnel), identité texte libre (nom/praticien, spécialité, lieu), période de suivi (dates début/fin optionnelles) et commentaire enrichi (RTF + texte).
- **Table `famille_contacts`** : contacts de l'entourage du patient (lien `ref_liens_patient`, rôle légal `ref_role_legal`, coordonnées country-aware, commentaire enrichi).

#### Brique 1 - Couche métier référentiel Thérapeutes
- **`Metier/Referentiels/Therapeute.vb`** : modèle métier riche (identité, coordonnées, `Actif`, commentaire).
- **`Metier/Referentiels/GestionTherapeutes.vb`** : service CRUD complet (liste actifs/tous, recherche, insertion, mise à jour, soft-delete, suppression physique conditionnelle, test d'usage).
- **`Core/Database/Queries/QueryTherapeutes.vb`** : requêtes SQL centralisées (SELECT, INSERT, UPDATE, soft-delete, DELETE, COUNT usage).

#### Brique 2 - Référentiel Rôles légaux
- **`Metier/Referentiels/RoleLegal.vb` + `GestionRoleLegal.vb`** : modèle et service (unicité code/libellé, soft-delete prioritaire).
- **`UI/Controls/Referentiels/UC_RoleLegal.vb`** : UserControl héritant de `UC_ReferentielBase` (métadonnées + 6 points d'extension Données) - 11ᵉ tuile du hub `UC_ReferentielHome`.

#### Brique 3 - Écran de liste Thérapeutes (entité riche)
- **`UI/Controls/Referentiels/UC_Therapeutes.vb`** : écran de liste dédié (ne dérive **pas** de `UC_ReferentielBase`) - recherche/filtre en mémoire, filtre « inactifs », CRUD, soft-delete prioritaire, suppression physique uniquement si non référencé.
- **`UI/Forms/Therapeute/TherapeuteEdition.vb`** : Form modale d'édition (identité, coordonnées country-aware via `UtilsTelephone`/`UtilsValidation`, état actif, commentaire).
- Intégrée au hub Référentiels (`UC_ReferentielHome` : 9 → **11 tuiles**) et accessible via le bouton `[+]` d'`IntervenantEdition`.

#### Brique 4 - Contacts famille (onglet Famille de la fiche patient)
- **`Metier/Patients/FamilleContact.vb` + `GestionFamilleContacts.vb`** : modèle et service de persistance des contacts.
- **`UI/Forms/Patients/ContactEdition.vb`** : Form modale (combos Lien + Rôle légal avec boutons `[+]`, coordonnées country-aware, commentaire enrichi `UC_RichTextEditorSimple`, copie de l'adresse patient).
- **`UC_PatientFiche.vb`** : onglet **Famille / Contacts** branché (grille `dgvContacts`, recherche, CRUD délégué à `ContactEdition`).

#### Brique 5 - Réseau d'intervenants (onglet Intervenants de la fiche patient)
- **`Metier/Patients/SuiviIntervenant.vb` + `GestionSuivisIntervenants.vb`** : modèle et service de la liaison N-N `autres_suivis_patient`.
- **`UI/Forms/Patients/IntervenantEdition.vb`** : Form modale - **thérapeute obligatoire** (sentinelle `IdTherapeuteNonSelectionne = 0`), rôle optionnel (`IdRoleNonPrecise = 0`), auto-remplissage nom/spécialité/lieu depuis le thérapeute choisi (`AppliquerSnapshotTherapeute`, neutralisé pendant le chargement par `_chargementEnCours`), période de suivi, commentaire enrichi, boutons `[+]` (thérapeute + rôle).
- **`UC_PatientFiche.vb`** : onglet **Intervenants** branché (grille `dgvIntervenants`, recherche, CRUD délégué à `IntervenantEdition`).

#### Refactor phase 2 - Harmonisation patients/référentiels
- **Activation progressive** (D-Q14) consolidée dans `UC_PatientFiche` : les onglets Famille/Contacts, Intervenants et Dossiers ne s'activent qu'après obtention de l'`id_patient`.
- **Mini-pile de navigation** (D-Q15) : `UC_PatientHome` conserve et restaure le filtre de recherche au retour depuis la fiche (`CreerEcranCourant`, `RestaurerFiltre`, `AppliquerFiltreInitial`).
- **Réutilisation du pipeline référentiel** : les boutons `[+]` de `ContactEdition` et `IntervenantEdition` passent par `Home.OuvrirReferentielModal` (gestion des droits + élévation), avec rechargement de combo et auto-sélection au retour.
- **Country-aware généralisé** : téléphone (`UtilsTelephone`) et e-mail (`UtilsValidation`) validés selon le pays sur les trois Forms (`ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`).

#### Documentation
- **`Docs/UI/Documentation_technique_UI_Althea.md`** : tableau de synthèse complété (7 écrans : `UC_PatientHome`, `UC_PatientFiche`, `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`, `UC_Therapeutes`, `UC_RoleLegal`) + sections détaillées correspondantes ; hub Référentiels recompté (11 tuiles / 10 référentiels concrets).
- **`Docs/UI/Process_Althea.md`** : ajout des Processus 11 (Gestion des patients), 12 (Contacts famille), 13 (Réseau d'intervenants) et 14 (Référentiel des thérapeutes), chacun avec son diagramme Mermaid.

#### Build & qualité
- ✅ Build réussi. Encodage UTF-8 avec BOM conservé sur tous les fichiers `.vb` créés/modifiés.

---

## 📅 14/06/2026

### 🏥 C1 - Patients : anamnèse, photo d'identité et export documentaire (migration v2.3 - lot 3)

#### Migration de schéma v2.3.0 lot 3 - Anamnèse patient (BD-15)
- **Création de `Docs/Database/Migration/migration_v2.3.0_lot3_anamnese.sql`** : ajout de `patients.anamnese_rtf` et `patients.anamnese_txt` (MEDIUMTEXT, après `alerte_txt`) ; versionnement `tec_meta_schema` → 2.3.0.
- **Création de `Docs/Database/Migration/migration_v2.3.0_lot3_anamnese_ROLLBACK.sql`** : rollback correspondant.
- **Mise à jour de `Docs/Database/Database_technique.md`** : ajout de `anamnese_rtf` / `anamnese_txt` dans la définition de la table `patients` (après `alerte_txt`).
- ✅ **Migration appliquée** en base de développement. Paramètres `PATH_GENERAL` et `PATH_DOCUMENT` déjà présents dans `tec_parametres` (groupe `PATHS`) - aucun ajout requis.

#### Brique 2 - Refactoring CheminsPatientHelper (chemins déterministes par code patient)
- **`Utils/Helpers/CheminsPatientHelper.vb`** - refactoring complet : les fonctions utilisent désormais `code_patient` (ex. `PA000003`) à la place de `id_patient`, conformément à la convention choisie (plus parlant, ADR-20).
  - Ajout de `GetSousDossierDocuments()`, `GetDossierDocuments()`, `FormaterCodePatient(idPatient)`.
  - `GetDossierPatient(codePatient)` reconstruit `{PATH_GENERAL}\{PATH_DOCUMENT}\{code_patient}`.
  - `AssurerDossierPatient(codePatient)` crée le dossier à la demande.
  - `GetNomFichierPhotoIdentite(extension)` : nom déterministe `Identite.ext` (minuscule, sans timestamp).
  - `GetCheminFichierPatient(codePatient, nomFichier)` : chemin complet depuis le nom seul.
- **`UC_PatientFiche.vb`** : `AfficherPhoto` mis à jour pour utiliser `_patient.CodePatient`.

#### Brique 3 - Extension du modèle Patient pour l'anamnèse
- **`Metier/Patients/Patient.vb`** : ajout des propriétés `AnamneseRtf` et `AnamneseTxt` (double format, règle §21 de `Rules.md`).
- **`Core/Database/Queries/QueryPatients.vb`** : `SelectPatientById`, `InsertPatient`, `UpdatePatient` intègrent `anamnese_rtf` / `anamnese_txt`. `SelectPatientsListe` inchangé (performance).
- **`Metier/Patients/GestionPatients.vb`** : `AjouterParametresPatient` lie `@anamnese_rtf`/`@anamnese_txt` ; `MapPatient` lit les nouvelles colonnes.

#### Brique 4 - Onglet Anamnèse dans UC_PatientFiche
- **`UC_PatientFiche.Designer.vb`** : ajout de l'onglet `tabPageAnamnese` (position 2, entre *Identité* et *Famille*), contenant `grpAnamnese` (`Dock=Fill`) avec un `UC_RichTextEditor` complet (`rteAnamnese`).
- **`UC_PatientFiche.vb`** :
  - `SetContext` propage le contexte à `rteAnamnese`.
  - `RemplirChamps` : `rteAnamnese.ChargerContenu(patient.AnamneseRtf, patient.AnamneseTxt)`.
  - `ViderChamps` : `rteAnamnese.RtfContent = String.Empty`.
  - `AppliquerMode` : `rteAnamnese.ReadOnlyMode = Not editable`.
  - `AlimenterPatientDepuisChamps` : sérialise `AnamneseRtf` / `AnamneseTxt`.
  - **Correctif** : `AppliquerMode` ne force le retour sur *Identité* qu'en **Création** ; en **Modification**, l'onglet courant est conservé.
- **`UI/Controls/Communs/UC_RichTextEditor.vb`** : ajout de `ChargerContenu(rtfContent, txtFallback)` - symétrique de `UC_RichTextEditorSimple`.

#### Brique 5 - Upload de la photo d'identité
- **`UC_PatientFiche.Designer.vb`** : ajout du bouton `btnUploadPhoto` (« Photo... ») dans le bandeau, sous le code patient ; style plat vert cohérent avec les boutons d'action existants.
- **`UC_PatientFiche.vb`** :
  - Constante `ExtensionsPhotoAutorisees` : `.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`, `.tif`, `.tiff`.
  - Tooltip sur `btnUploadPhoto` (formats acceptés).
  - `btnUploadPhoto.Enabled` lié à `patientEnregistre` dans `AppliquerMode`.
  - Handler `btnUploadPhoto_Click` : `OpenFileDialog` filtré → validation défensive d'extension (`Array.IndexOf`) → `AssurerDossierPatient` → copie vers `Identite.ext` (nom déterministe, `overwrite:=True`) → `UpdatePhotoPatient` (MAJ `patients.photo_fichier`) → `_patient.PhotoFichier` → `AfficherPhoto`.
  - Helpers privés : `SelectionnerFichierPhoto()` et `ExtensionPhotoEstAutorisee(extension)`.

#### Brique 6a - Export contextuel de l'anamnèse (PDF + Word) par délégation
- **`UI/Controls/Communs/UC_RichTextEditor.vb`** - ajout du mécanisme d'export délégué (ADR-21) :
  - `ExportFormat` (enum `Pdf`/`Word`).
  - `ExportRequestedEventArgs` : `Format`, `NomFichierInitial`, `DestinationPath` (rempli par le conteneur).
  - `ExportCompletedEventArgs` : `Format`, `DestinationPath`, `Success`.
  - Événements publics `ExportRequested` et `ExportCompleted`.
  - Méthode commune `DemanderExport` : lève `ExportRequested` → si `DestinationPath` fourni, export direct (`ExporterPDF`/`ExporterWord`) + lève `ExportCompleted` → sinon fallback `SaveFileDialog` (comportement générique préservé).
  - Les boutons `btnExportPDF_Click` / `btnExportWord_Click` délèguent à `DemanderExport`.
- **`UC_PatientFiche.vb`** :
  - `rteAnamnese_ExportRequested` : calcule le chemin `…\Documents\PA000003\anamnese_PA000003_20260614_153000.pdf` (horodaté, dossier assuré par `AssurerDossierPatient`). Uniquement pour un patient enregistré - sinon fallback dialogue.
  - `rteAnamnese_ExportCompleted` : ouvre le fichier exporté via `Process.Start` (`UseShellExecute = True`).
  - Abonnement déclaratif via `Handles` (`Friend WithEvents`).

#### Ancres de traçabilité documentaire (règle différée - brique Documents)
- **`UC_PatientFiche.vb`** : ajout de commentaires `TODO` aux deux points de matérialisation de fichiers (export anamnèse et upload photo), documentant la règle : « tout document créé ou uploadé doit avoir une trace dans `documents` + modèle + niveau ».
- **`Docs/Todo/ToDo.md §D1`** : formalisé en exigence de tête de section (règle, décision niveau Enum vs paramètre, points d'ancrage code).

#### Build & qualité
- ✅ Build réussi après chaque brique. Encodage UTF-8 avec BOM normalisé sur tous les fichiers `.vb` modifiés (`UC_PatientFiche.vb`, `UC_PatientFiche.Designer.vb`, `UC_RichTextEditor.vb`, `Patient.vb`, `QueryPatients.vb`, `GestionPatients.vb`, `CheminsPatientHelper.vb`).

---

## 📅 13/06/2026

### 🏥 C1 - Patients : contacts famille - rôle légal (migration v2.2 - lot 2)

#### Migration de schéma v2.2.0 lot 2 - Rôle légal des contacts (BD-14 / D-Q18)
- **Création de `Docs/Database/Migration/migration_v2.2.0_lot2_role_legal.sql`** :
  - Création de la table référentielle `ref_role_legal` (`id_role_legal` AUTO_INCREMENT, `code_role_legal` VARCHAR 30 unique, `libelle_role_legal` VARCHAR 100 unique, `actif`, `ordre_affichage`). Seed initial : Autorité parentale, Représentant légal, Personne autorisée, Contact d'urgence.
  - `famille_contacts` : ajout de `id_role_legal` (FK NOT NULL → `ref_role_legal`), reprise des 4 anciens booléens vers la valeur de priorité décroissante (autorite_parentale > representant_legal > personne_autorisee > contact_urgence, repli sur Contact d'urgence), puis suppression des 4 colonnes booléennes cumulables.
  - Versionnement dans `tec_meta_schema` → 2.2.0.
- **Création de `Docs/Database/Migration/migration_v2.2.0_lot2_role_legal_ROLLBACK.sql`** : script de rollback (recréation des 4 booléens depuis `id_role_legal`, suppression de la FK et de `ref_role_legal`).
- **Mise à jour de `Docs/Database/Database_technique.md`** : ajout de `ref_role_legal` dans la liste des référentiels, nouvelle section dédiée, remplacement des 4 booléens par `id_role_legal` dans la définition de `famille_contacts`.
- ✅ **Migration appliquée** en base de développement.

#### Brique A - Référentiel « Rôles légaux » (.NET)
- **Création de `Metier/Referentiels/RoleLegal.vb`** : DTO calqué sur `LienPatient` - `IdRoleLegal` (ULong), `CodeRoleLegal`, `LibelleRoleLegal`, `Actif`, `OrdreAffichage`.
- **Création de `Core/Database/Queries/QueryRoleLegal.vb`** : module SQL calqué sur `QueryLiensPatient` - `SelectRolesLegauxActifs`, `SelectRolesLegauxTous`, `SelectCountCodeRoleLegal`, `SelectCountLibelleRoleLegal`, `SelectCountUsageRoleLegal` (compte les usages dans `famille_contacts`), `UpdateRoleLegal`, `DesactiverRoleLegal`, `SupprimerRoleLegal`, `InsertRoleLegal`.
- **Création de `Metier/Referentiels/GestionRoleLegal.vb`** : module métier calqué sur `GestionLiensPatient` - `GetRolesLegauxActifs()`, `GetRolesLegaux(afficherInactifs)`, `InsertRoleLegal`, `UpdateRoleLegal`, `DesactiverRoleLegal`, `SupprimerRoleLegal`, `CodeRoleLegalExiste`, `LibelleRoleLegalExiste`, `RoleLegalEstUtilise`.
- **Création de `UI/Controls/Referentiels/UC_RoleLegal.vb`** : écran référentiel code-only héritant `UC_ReferentielBase` - métadonnées (titre « Rôles légaux », fil d'Ariane `Référentiels > Rôles légaux`, rôle minimum `SuperUser`, longueur max code 30), branchement complet sur `GestionRoleLegal` (`ChargerElements`, `CodeExisteDeja`, `LibelleExisteDeja`, `InsererElement`, `MettreAJourElement`, `DefinirActivation`).
- **Modification de `UI/Controls/Referentiels/UC_ReferentielHome.Designer.vb`** : `tblMenu` étendu de 3 à 4 lignes (4×25 %) ; ajout du bouton `btnRoleLegal` (cellule (0,3), Tag `roleLegal_normal`, texte « Rôles légaux / Contacts »).
- **Modification de `UI/Controls/Referentiels/UC_ReferentielHome.vb`** : `btnRoleLegal` intégré à `ActiverReferentielsDisponibles`, `DesactiverTousLesReferentiels` et au bloc `InitLargeIconButton` ; ajout du handler `btnRoleLegal_Click` naviguant vers `New UC_RoleLegal()` via `Home.NavigateToReferentielView`.
- ✅ **Jalon de test 1** - build brique A réussi.

#### Brique B - Refonte du stack contacts vers le rôle légal unique
- **Modification de `Metier/Patients/FamilleContact.vb`** : suppression des 4 propriétés booléennes (`AutoriteParentale`, `RepresentantLegal`, `PersonneAutorisee`, `ContactUrgence`) ; ajout de `IdRoleLegal` (ULong, FK obligatoire) et `LibelleRoleLegal` (String, alimenté par jointure).
- **Modification de `Core/Database/Queries/QueryFamilleContacts.vb`** :
  - `SelectContactsParPatient` et `SelectContactById` : colonnes booléennes remplacées par `fc.id_role_legal` + `LEFT JOIN ref_role_legal rl` → `rl.libelle_role_legal`.
  - `InsertContact` et `UpdateContact` : paramètres `@autorite_parentale/…` remplacés par `@id_role_legal`.
- **Modification de `Metier/Patients/GestionFamilleContacts.vb`** :
  - `AjouterParametresContact` : 4 `AddWithValue` booléens remplacés par `AddWithValue("@id_role_legal", contact.IdRoleLegal)`.
  - `MapContact` : lecture des 4 booléens remplacée par `IdRoleLegal` + `LibelleRoleLegal`.
- **Modification de `UI/Forms/Patients/ContactEdition.Designer.vb`** : `grpRoles` (renommé « Rôle légal ») - suppression des 4 `CheckBox` ; ajout de `lblRoleLegal`, `cboRoleLegal` (ComboBox DropDownList) et `btnAjouterRole` (`[+]`), calqués sur le modèle `lblLien/cboLien/btnAjouterLien`.
- **Modification de `UI/Forms/Patients/ContactEdition.vb`** :
  - Constante `CodeRoleParDefaut = "AUTORITE_PARENTALE"` (présélection à la création).
  - Nouvelles méthodes `ChargerRolesLegaux(idASelectionner)` et `SelectionnerRoleParDefaut()` - calquées sur leurs équivalents pour le lien.
  - `InitialiserCombos` : charge `cboRoleLegal`, masque `btnAjouterRole` si `_homeForm` est absent.
  - `InitialiserFenetre` : appelle `SelectionnerRoleParDefaut()` en mode création.
  - `RemplirChamps` : charge le rôle via `ChargerRolesLegaux(contact.IdRoleLegal)`.
  - `ValiderSaisie` : validation bloquante si `cboRoleLegal.SelectedValue Is Nothing` (FK NOT NULL).
  - `Enregistrer` : `contact.IdRoleLegal = CULng(cboRoleLegal.SelectedValue)`.
  - Nouveau handler `btnAjouterRole_Click` : ouvre `UC_RoleLegal` en modal, recharge le combo avec auto-sélection du nouvel élément - calqué sur `btnAjouterLien_Click`.
- **Modification de `UI/Controls/Patients/UC_PatientFiche.Designer.vb`** : grille `dgvContacts` - 4 `DataGridViewCheckBoxColumn` remplacées par une seule `DataGridViewTextBoxColumn` `colContactRole` (`DataPropertyName = "LibelleRoleLegal"`, `HeaderText = "Rôle légal"`).
- **Modification de `UI/Controls/Patients/UC_PatientFiche.vb`** : `CorrespondRechercheContact` - `contact.LibelleRoleLegal` ajouté aux champs de recherche en mémoire.
- ✅ **Jalon de test 2** - build final réussi ; balayage global : aucun usage résiduel des 4 anciennes propriétés booléennes dans la solution.
- ✅ Encodage UTF-8 BOM appliqué sur les 4 fichiers créés (`RoleLegal.vb`, `QueryRoleLegal.vb`, `GestionRoleLegal.vb`, `UC_RoleLegal.vb`).

---

## 📅 12/06/2026

### 🏥 C1 - Patients : fiche patient (UC_PatientFiche) - itération 3 (téléphone & pays)

#### Brique réutilisable de gestion du téléphone (UtilsTelephone)
- **Création de `Utils/UI/UtilsTelephone.vb`** : module statique de normalisation, formatage et validation du téléphone par pays (BE, FR, LU, DE, NL), réutilisable dans tout l'applicatif.
  - `DeduirePays(libellePays)` / `NormaliserLibellePays(libellePays)` - déduction du pays depuis le libellé (gère les variantes Belgique/Belgium/België…).
  - `LibellesPays()` / `LibellePaysParDefaut()` - liste canonique des pays pour les ComboBox.
  - `NormaliserE164(saisie, libellePays)` - normalisation en format E.164 pour le stockage.
  - `FormaterAffichage(saisie, libellePays)` - masque lisible (groupes par pays) à l'affichage.
  - `Valider(saisie, libellePays, ByRef messageErreur)` - contrôle de conformité aux règles nationales ; repli sur `UtilsValidation.IsValidTelephone` si le pays est indéterminé.

#### Sélecteur de pays par ComboBox (UC_PatientFiche)
- **`UC_PatientFiche.Designer.vb`** : `txtPays` (TextBox) remplacé par `cboPays` (ComboBox `DropDownList`) dans le groupe Coordonnées.
- **`UC_PatientFiche.vb`** :
  - `InitialiserCombos()` charge `cboPays` depuis `UtilsTelephone.LibellesPays()`.
  - `SelectionnerPays(libellePays)` canonicalise les anciens libellés / variantes localisées.
  - `RemplirChamps()` sélectionne le pays canonique et formate le téléphone via `UtilsTelephone.FormaterAffichage`.
  - `AlimenterPatientDepuisChamps()` stocke `cboPays.Text` et normalise le téléphone via `UtilsTelephone.NormaliserE164`.
  - `DefinirEtatChampsIdentite()` active/désactive `cboPays` selon le mode.
  - Handlers `txtTelephone_Leave` et `cboPays_SelectedIndexChanged` - re-formatent le téléphone à l'édition et au changement de pays.

#### Affichage du téléphone dans la liste (UC_PatientHome)
- **`UC_PatientHome.vb`** : ajout de `dgvPatients_CellFormatting` - formate la colonne téléphone via `UtilsTelephone.FormaterAffichage` au rendu uniquement (la source de données reste inchangée, stockée en canonique).

#### Correction du reset des champs (UC_PatientFiche)
- **`UC_PatientFiche.vb`** : `ViderChamps()` réinitialise désormais correctement la date de naissance (`dtpDateNaissance.Value = Date.Today`, `Checked = False`) et la situation familiale (`ReinitialiserSituationFamiliale()`), qui ne se réinitialisaient pas.

#### Build et validation
- ✅ Génération réussie ; encodage UTF-8 BOM rétabli sur les fichiers modifiés.



### 🏥 C1 - Patients : fiche patient (UC_PatientFiche) - itération 2

#### Validations réutilisables (UtilsValidation)
- **Extension de `Utils/UI/UtilsValidation.vb`** : ajout de deux fonctions de validation centralisées.
  - `IsValidTelephone(value, ByRef messageErreur)` - validation souple (6-15 chiffres, séparateurs courants autorisés) ; champ optionnel accepté vide.
  - `IsValidEmail(value, ByRef messageErreur)` - validation regex format `nom@domaine.ext` ; champ optionnel accepté vide.

#### Verrouillage réel des onglets dépendants (D-Q14)
- **`UC_PatientFiche.vb`** : ajout du handler `tabFiche_Selecting` - annule la sélection des onglets Famille, Intervenants, Dossiers tant que `_idPatient <= 0`. Corrige le comportement WinForms où `TabPage.Enabled = False` grise le contenu mais ne bloque pas le clic.

#### Âge calculé à la volée
- **`UC_PatientFiche.Designer.vb`** : ajout de `lblAge` positionné à droite de `dtpDateNaissance`.
- **`UC_PatientFiche.vb`** : ajout des méthodes `RafraichirAge()` et `CalculerAge(dateNaissance, reference)` ; handler `dtpDateNaissance_ValueChanged` recalcule l'affichage à chaque modification.

#### Pays par défaut « Belgique »
- **`UC_PatientFiche.vb`** : `ViderChamps()` initialise `txtPays` à `PaysParDefaut = "Belgique"` en création.

#### Validations bloquantes et messages explicites
- **`UC_PatientFiche.vb`** : réécriture de `ValiderSaisie()` - chaque champ invalide est signalé via `ErrorProvider` ; un message récapitulatif explicite est posé dans le statut Home (`SetStatus("Veuillez corriger : ...")`). Le premier champ fautif reçoit le focus. Champs validés : Nom (obligatoire), Prénom (obligatoire), Téléphone (format souple), E-mail (format), NISS (unicité).
- **`UC_PatientFiche.vb`** : `Enregistrer()` ne pose plus de message générique pour ne pas écraser le message explicite de `ValiderSaisie()`.

#### Intégration de l'alerte / notes thérapeute (D-Q12)
- **`UC_PatientFiche.Designer.vb`** : ajout de `grpAlerte` (GroupBox « Alerte / Notes », ancré `Top|Bottom|Left|Right`) contenant `rteAlerte` (`UC_RichTextEditorSimple` en `Dock.Fill`) en bas de l'onglet Identité. La zone s'étire verticalement avec la fenêtre.
- **`UC_PatientFiche.vb`** :
  - `RemplirChamps()` - utilise la nouvelle méthode `rteAlerte.ChargerContenu(rtf, txt)` (robuste, force `CreateControl()` pour fiabiliser le chargement RTF au premier affichage).
  - `ViderChamps()` - réinitialise l'éditeur.
  - `AlimenterPatientDepuisChamps()` - écrit `patient.AlerteRtf` et `patient.AlerteTxt` depuis l'éditeur.
  - `DefinirEtatChampsIdentite()` - active/désactive `rteAlerte.ReadOnlyMode` selon le mode (consultation/édition).
  - `SetContext()` - propage le contexte UI à `rteAlerte.SetContext()` pour les infobulles.
- **`UC_RichTextEditorSimple.vb`** : ajout de la méthode publique `ChargerContenu(rtfContent, txtFallback)` - force `CreateControl()` avant l'affectation RTF pour éviter la perte silencieuse du contenu quand le handle n'est pas encore créé.

#### Bandeau d'alerte/notes redesigné (D-Q12)
- **`UC_PatientFiche.Designer.vb`** : `lblAlerte` (Label, texte brut, orange) remplacé par `rtbAlerte` (`RichTextBox`, lecture seule, ascenseur vertical automatique, fond crème sobre). Le formatage RTF de la note est préservé à l'affichage.
- **`UC_PatientFiche.vb`** : `AfficherAlerte(alerteRtf, alerteTxt)` - charge le contenu RTF dans `rtbAlerte` via `RichTextEditorHelper.ChargerContenu` ; affiche « Aucune note. » en gris si aucun contenu. Suppression de la coloration orange (les notes ne sont pas des alertes critiques).

#### Build et validation
- ✅ Génération réussie sans erreur après l'ensemble des corrections de l'itération 2.



## 📅 11/06/2026

### 🏥 C0 - Briques transverses cœur métier

#### Lookup scalaire de paramètre applicatif
- **Extension de `Core/Database/Queries/QueryParametres.vb`** : ajout de `SelectValeurParametreByCle` - requête SQL retournant la valeur d'un paramètre actif par sa clé (ex. `PATH_GENERAL`).
- **Extension de `Metier/Parametres/GestionParametres.vb`** : ajout de `GetValeurParametre(cle)` - lecture d'une valeur scalaire depuis `tec_parametres` ; utilisé par les helpers nécessitant la racine de données.

#### Helper de chemins déterministes pour les fichiers patients (D-Q13)
- **Création de `Utils/Helpers/CheminsPatientHelper.vb`** : module statique calculant les chemins fichiers patients de manière déterministe à partir du paramètre `PATH_GENERAL`.
  - `GetRacineDonnees()` - lit `PATH_GENERAL` via `GestionParametres`
  - `GetDossierPatients()` - racine + `Patients\`
  - `GetDossierPatient(idPatient)` - dossier individuel zéro-paddé à 6 chiffres (`Patients\000001\`)
  - `AssurerDossierPatient(idPatient)` - crée le dossier si absent
  - `GetNomFichierPhotoIdentite(extension)` - retourne `Identite.<ext>`
  - `GetCheminFichierPatient(idPatient, nomFichier)` - chemin complet d'un fichier quelconque du dossier

#### Mini-pile de navigation (D-Q15)
- **Création de `UI/Navigation/NavigationEntry.vb`** : payload d'une entrée d'historique - porte la fabrique de vue (`Func(Of UserControl)`), le bouton menu, le libellé de contexte et un état de filtre optionnel.
- **Extension de `UI/Navigation/NavigationManager.vb`** : ajout de `CanNavigateBack`, `NavigateAndPush`, `NavigateBack`, `ClearHistory` - gestion d'une `Stack(Of NavigationEntry)` permettant un retour contextuel avec restauration du filtre. Fichier converti de Windows-1252 en UTF-8 BOM (correction d'encodage).
- **Extension de `UI/Forms/Home.vb`** : ajout de `NavigateAvecHistorique`, `NavigateRetour` (avec restauration du filtre et re-sélection du bouton menu) ; `NavigateTo` réinitialise désormais l'historique à chaque navigation racine.

#### Hôte modal générique de référentiel (D-Q17)
- **Création de `UI/Forms/Communs/ReferentielModalHost.vb`** : Form modale hébergeant n'importe quel `UC_Ref<X>` existant en `Dock.Fill`, avec son propre contexte UI local (`UserSession` + `ToolTip` + `ErrorProvider`). Aucune logique de sécurité dupliquée.
- **Extension de `UI/Forms/Home.vb`** : ajout de `OuvrirReferentielModal(vueReferentiel, titre)` - point d'entrée unique pour déclencher un référentiel en contexte modal depuis n'importe quel UC métier.

### 🏥 C1 - Patients : migration de schéma

#### Migration v2.1.0 - Lot 1 Patients
- **Création de `Docs/Database/Migration/migration_v2.1.0_lot1_patients.sql`** :
  - `patients.alerte` renommé `alerte_rtf` (LONGTEXT) + ajout `alerte_txt` (LONGTEXT) pour double stockage RTF/texte brut (D-Q12)
  - Ajout de `patients.photo_fichier` VARCHAR(255) - nom seul du fichier, chemin reconstruit (D-Q13)
  - Suppression de `dossiers.prescripteur` VARCHAR - remplacé par le réseau N-N `autres_suivis_patient` (BD-13)
  - Enrichissement commentaires `famille_contacts` et `autres_suivis_patient` : colonnes `commentaire_rtf` + `commentaire_txt` (BD-8)
  - Versionnement dans `tec_meta_schema` → 2.1.0
- **Création de `Docs/Database/Migration/migration_v2.1.0_lot1_patients_ROLLBACK.sql`** : script de rollback correspondant.
- **Mise à jour de `Docs/Database/Database_technique.md`** : synchronisation de la documentation de schéma avec tous les changements ci-dessus.
- ✅ **Migration appliquée** en base de développement.

### 🏥 C1 - Patients : couche métier

#### Modèles et requêtes SQL
- **Création de `Metier/Patients/Patient.vb`** : DTO complet patient - identité, coordonnées, situation familiale, `PhotoFichier`, `AlerteRtf`, `AlerteTxt` ; helpers `NomComplet`, `AAlerte`.
- **Création de `Metier/Patients/PatientListeItem.vb`** : modèle léger pour l'écran de liste - ajoute `NbDossiers`, `NbDossiersActifs`, `DateModification` ; helpers `NomComplet`, `AAlerte`, `APhoto`.
- **Création de `Core/Database/Queries/QueryPatients.vb`** : module SQL patients - `SelectPatientsListe` (avec compteurs de dossiers via LEFT JOIN), `SelectPatientById`, requêtes INSERT/UPDATE/DELETE, vérifications unicité NISS et doublon nom+prénom+naissance.

#### Service métier
- **Création de `Metier/Patients/GestionPatients.vb`** : module CRUD patients.
  - `GetPatients()` → `List(Of PatientListeItem)` triée par `date_modification DESC`
  - `GetPatientById(id)` → `Patient` complet
  - `CreatePatient(patient)`, `UpdatePatient(patient)`, `UpdatePhotoPatient(id, nomFichier)`, `DeletePatient(id)`
  - Validations : `NissExiste`, `DoublonExiste` (nom+prénom+naissance), `PeutSupprimerPatient`
  - Mappers privés `MapPatient` / `MapListeItem` ; helpers SQL `LireString`, `LireInt`, `LireDateNullable`, `LireLongNullable`, `ValeurOuDBNull`

### 🏥 C1 - Patients : écran d'accueil (UC_PatientHome)

#### Nouveau UserControl
- **Création de `UI/Controls/Patients/UC_PatientHome.vb`** et **`UC_PatientHome.Designer.vb`** : premier écran du cœur métier patients.
  - Implémente `IContextAwareUserControl` - contexte UI injecté par `Home` via `SetContext()`
  - **Grille** `dgvPatients` liée par `DataPropertyName` à `PatientListeItem` (13 colonnes : Code, Nom, Prénom, Naissance, NISS, Téléphone, Email, Dossiers, Actifs, Alerte ☑, Photo ☑, Modifié le + Id masqué)
  - **Recherche multi-champs en mémoire** : nom, prénom, NISS, code, téléphone, email - insensible à la casse, protégée contre les valeurs `Nothing`
  - **Filtre « Avec dossiers actifs »** : restreint aux patients avec `NbDossiersActifs > 0`
  - **Boutons** : Nouveau, Modifier, Ouvrir, Actualiser, Rechercher, Réinitialiser (via `UtilsButtons.InitStandardButton` + Tags)
  - **Double-clic** sur une ligne et touche **Entrée** dans le champ de recherche déclenchent la recherche
  - Boutons Nouveau/Modifier/Ouvrir : placeholders « à venir » (UC_PatientFiche, lot suivant)
  - `ErrorProvider` et `ToolTip` **supprimés du Designer** - délégués entièrement au contexte UI partagé de `Home` (`_context.SetToolTip`, `_context.SetError`)

#### Câblage navigation
- **Modification de `UI/Forms/Home.vb`** - `btnPatients_Click` : navigue désormais vers `New UC_PatientHome()` via `NavigateTo(..., btnPatients, "Patients")` ; le contexte UI est injecté automatiquement par `NavigationManager`.

#### Données de test
- **Création de `Docs/Database/Seeds/seed_patients_dev.sql`** : 10 patients fictifs couvrant les cas représentatifs (alerte + photo, photo seule, alerte seule, données minimales, NISS NULL, adresse complète, situation familiale variée, mutualité, adulte, gaucher). Tri `date_modification DESC` de J à J-9. Requête de vérification intégrée.

#### Build et validation
- ✅ Génération réussie sans erreur après l'ensemble du lot C0 + C1 couche métier + UC_PatientHome

---

## 📅 09/06/2026

### 🖊️ Composant UC_RichTextEditorSimple

#### Nouveau composant UI réutilisable (variante allégée)
- **Création de `UI/Controls/Communs/UC_RichTextEditorSimple.vb`** : Version compacte de `UC_RichTextEditor` destinée aux zones de commentaires et notes courtes embarquées dans d'autres UserControls ou Forms (référentiels, fiches patient, dossiers…).
  - **Toolbar minimale (7 boutons)** : Gras, Italique, Souligné, Annuler, Rétablir, Effacer formatage, Insérer date/heure
  - **Raccourcis clavier standards** : Ctrl+B, Ctrl+I, Ctrl+U natifs
  - **Sauvegarde double format** : `RtfContent` (formatage) + `TextContent` (texte brut pour recherche full-text SQL), identique à la version complète
  - **Mode lecture seule** (`ReadOnlyMode`) masquant la toolbar
  - **Affichage/masquage toolbar** (`ShowToolbar`)
  - **Événement `ContentChanged`** : notification des modifications au parent
  - **Contexte optionnel** : implémente `IContextAwareUserControl` de façon optionnelle - fonctionne sans contexte si le parent ne l'injecte pas
  - **Réutilise `RichTextEditorHelper` à 100 %** : aucune logique dupliquée
  - **Taille entièrement pilotée par le parent** (Dock / Anchor)

#### Différences avec UC_RichTextEditor (complet)
| Fonctionnalité | UC_RichTextEditor | UC_RichTextEditorSimple |
|---|---|---|
| Police / taille | ✅ 9 polices, 14 tailles | ❌ Non |
| Alignement, puces, retraits | ✅ | ❌ Non |
| Couleurs texte / fond | ✅ | ❌ Non |
| Couper / Copier / Coller | ✅ boutons toolbar | ✅ raccourcis OS natifs uniquement |
| Impression Win32 | ✅ | ❌ Non |
| Export PDF / Word | ✅ Syncfusion | ❌ Non |
| Double format RTF + TXT | ✅ | ✅ |
| Mode ReadOnly | ✅ | ✅ |
| Contexte UI partagé | ✅ obligatoire | ✅ optionnel |
| Taille | Fixe (grande) | Pilotée par le parent |

#### Cas d'usage cibles
- **Champ `commentaire`** dans les référentiels enrichis (`famille_contacts`, `autres_suivis_patient`)
- **Notes courtes** dans les fiches patient, dossiers, séances
- **Tout champ de texte libre** nécessitant du formatage basique sans l'overhead de la version complète

---

## 📅 08/06/2026

### 🗂️ Module complet de gestion des référentiels

#### Architecture générique
- **Création de `UI/Controls/Referentiels/UC_ReferentielBase.vb`** : Classe de base héritable pour tous les écrans de gestion de référentiels (`ref_*`). Centralise : chargement, modes (consultation/création/modification), CRUD via points d'extension, validation (unicité code + libellé), droits, journalisation, activation/désactivation (soft-delete).
  - **Points d'extension Overridable** : `ChargerElements`, `CodeExisteDeja`, `LibelleExisteDeja`, `InsererElement`, `MettreAJourElement`, `DefinirActivation`
  - **Hooks champ supplémentaire** : `ConfigurerChampSupplementaire`, `AfficherChampSupplementaire`, `ViderChampSupplementaire`, `ActiverChampSupplementaire`, `ValiderChampSupplementaire` - permettent à une classe dérivée d'ajouter un champ d'édition sans modifier la base (ex. `tarif_defaut` dans `ref_types_seance`)
  - **Modèle de présentation générique** : `ReferentielLigne` avec propriété `Tarif As Decimal?` optionnelle pour les référentiels avec montant associé
  - **Compatibilité Designer** : non `MustInherit`, chaque UC dérivé possède son propre `.Designer.vb`

- **Création de `UI/Controls/Referentiels/UC_ReferentielHome.vb`** : Hub d'accueil de la section Référentiels. Présente 9 tuiles, gère les droits (SuperUser/Admin) et navigue via `Home.NavigateToReferentielView()`.

#### 9 référentiels implémentés (pile complète Query + Modèle + Gestion + UC)

| Référentiel | Table | UC | Code max | Table d'usage |
|---|---|---|---|---|
| Domaines | `ref_domaines` | `UC_Domaines` | 10 | `dossiers`, `modeles_documents` |
| Liens patient | `ref_liens_patient` | `UC_LiensPatient` | 50 | `famille_contacts` |
| Rôles intervenant | `ref_roles_intervenant` | `UC_RolesIntervenant` | 30 | `autres_suivis_patient` |
| Situations familiales | `ref_situations_familiales` | `UC_SituationsFamiliales` | 50 | `patients` |
| Statuts dossier | `ref_statuts_dossier` | `UC_StatutsDossier` | 30 | `dossiers` |
| Statuts séance | `ref_statuts_seance` | `UC_StatutsSeance` | 30 | `seances` |
| Types documents | `ref_types_documents` | `UC_TypesDocuments` | 30 | `documents`, `modeles_documents` |
| Types rendez-vous | `ref_types_rendez_vous` | `UC_TypesRendezVous` | 30 | `rendez_vous` |
| **Types séance** ⭐ | `ref_types_seance` | `UC_TypesSeance` | 30 | `seances` + **tarif_defaut** |

#### Cas spécial : UC_TypesSeance avec tarif par défaut
- `ref_types_seance` possède un champ `tarif_defaut decimal(10,2) NOT NULL` absent des autres référentiels
- Géré via les hooks `ConfigurerChampSupplementaire` / `AfficherChampSupplementaire` : `UC_TypesSeance` greffe dynamiquement un `NumericUpDown` dans `pnlEdition` sans modifier la base commune
- `ReferentielLigne.Tarif As Decimal?` transporte la valeur entre couches

#### Modules créés par référentiel
Pour chacun des 9 référentiels (exemple sur `ref_types_seance`) :
- `Core/Database/Queries/Query<X>.vb` - requêtes SQL (SELECT actifs/tous, COUNT unicité code/libellé, COUNT usage, UPDATE, INSERT, DELETE soft+hard)
- `Metier/Referentiels/<X>.vb` - modèle métier (id, code, libelle, actif, ordre_affichage [+ tarif pour TypeSeance])
- `Metier/Referentiels/Gestion<X>.vb` - service CRUD + vérifications unicité + `<X>EstUtilise()`
- `UI/Controls/Referentiels/UC_<X>.vb` - UserControl concret héritant de `UC_ReferentielBase`

#### Intégration navigation
- `UC_ReferentielHome` : méthode `ActiverReferentielsDisponibles` active les 9 tuiles ; 7 handlers de clic ajoutés (`btnRolesIntervenant_Click` → `btnTypesSeance_Click`)
- `Home.NavigateToReferentielView()` : navigation partagée existante, aucune modification nécessaire

#### Build et validation
- ✅ Génération réussie sans erreur après création de l'ensemble du lot

---

## 📅 06/06/2026

### 🎨 Module complet d'édition de texte riche (RichTextEditor)

#### Nouveau composant UI réutilisable
- **Création de `UI/Controls/UC_RichTextEditor.vb`** : UserControl d'édition de texte riche complet pour notes patients, anamnèses, bilans, comptes-rendus de consultations
  - **30 boutons de toolbar** : formatage caractères, paragraphes, presse-papiers, actions, insertion, outils
  - **Formatage de caractères** : Gras (Ctrl+B), Italique (Ctrl+I), Souligné (Ctrl+U), Barré, Couleur texte, Surbrillance
  - **Formatage de paragraphes** : Alignement (gauche/centré/droite), Listes à puces, Retraits (augmentation/diminution)
  - **Polices et tailles** : 9 polices courantes, 14 tailles (8 à 72 points)
  - **Actions** : Couper, Copier, Coller, Annuler, Rétablir, Effacer formatage
  - **Insertion** : Date/heure courante (format paramétrable)
  - **Outils** : Mise en page (📄), Impression (🖨), Export PDF (📑), Export Word (📝)
  - **Modes** : ReadOnly (masque toolbar), ShowToolbar (afficher/masquer toolbar)
  - **Sauvegarde double** : RTF (formatage préservé pour affichage) + TXT (texte brut pour recherche full-text SQL)
  - **Marges internes optimisées** : 10px gauche/droite pour meilleure lisibilité
  - **Implémentation `IContextAwareUserControl`** : intégration complète avec le contexte UI partagé

#### Helper centralisé
- **Création de `Utils/Helpers/RichTextEditorHelper.vb`** : Module utilitaire centralisant toute la logique métier
  - **Configuration** : `ConfigurerRichTextBox()` avec thème Althéa
  - **Formatage caractères** : BasculerGras, BasculerItalique, BasculerSouligne, BasculerBarre, ChangerCouleurTexte, ChangerCouleurFond
  - **Formatage paragraphes** : ChangerAlignement, BasculerPuces, AugmenterRetrait, DiminuerRetrait
  - **Police** : ChangerPolice, ChangerTaille
  - **Presse-papiers** : Couper, Copier, Coller
  - **Contenu** : ChargerContenu, ExtraireRtf, ExtraireTxt, EffacerTout
  - **Insertion** : InsererDateHeure avec format personnalisable
  - **Impression Win32** : Print avec API EM_FORMATRANGE (formatage RTF préservé), Imprimer avec PrintDialog, AfficherApercu, ConfigurerMiseEnPage
  - **Export PDF** : ExporterPDF, ExporterPDFAvecDialogue (Syncfusion DocIO + DocToPDFConverter)
  - **Export Word** : ExporterWord, ExporterWordAvecDialogue (Syncfusion DocIO, format .docx éditable)

#### Système d'impression avancé
- **V1.0-V1.2** : Implémentation initiale avec API Windows `EM_FORMATRANGE` pour préserver le formatage RTF natif
- **V1.3** : Résolution du problème de pages blanches via `checkPrint = 0` après chaque impression
- **V1.4** : Suppression de l'aperçu automatique non souhaité → impression directe après `PrintDialog`
  - Nouvelle méthode `AfficherApercu()` optionnelle pour visualisation avant impression
  - Bouton **📄 Mise en page** pour configuration format papier (A4/A3/Letter), marges, orientation (portrait/paysage)
  - Dialogue système Windows natif pour sélection imprimante et paramètres
  - Pagination automatique avec marges configurables

#### Export PDF avec Syncfusion
- **V1.5** : Intégration Syncfusion pour export PDF natif RTF → PDF
  - **Packages NuGet installés** : `Syncfusion.DocIO.WinForms`, `Syncfusion.DocToPDFConverter.WinForms`, `Syncfusion.Licensing` (v33.2.10)
  - **Configuration licence** : Ajout `SyncfusionLicenseProvider.RegisterLicense()` dans `Program.vb`
  - **Bouton toolbar** : 📑 Export PDF avec SaveFileDialog
  - **Formatage préservé** : Gras, italique, couleurs, alignement, puces, polices, retraits, paragraphes
  - **Conversion native** : RTF → WordDocument (DocIO) → PDF (DocToPDFConverter)
  - **Licence Community gratuite** : Pour usage non-commercial ou revenus < 1M USD

#### Export Word avec intégration documentaire
- **V1.6** : Export .docx éditable avec intégration POC système documentaire
  - **Nouveau bouton toolbar** : 📝 Export Word (après Export PDF)
  - **Méthodes** : `ExporterWord()`, `ExporterWordAvecDialogue()`
  - **Conversion** : RTF → WordDocument → Save DOCX (Syncfusion.DocIO)
  - **Intégration POC documentaire** :
    - Architecture locale : `Patients/{id_patient}/{id_dossier}/Documents/`
    - Synchronisation Google Drive automatique
    - Édition possible avec Word (local) ou Google Docs (cloud)
    - Conversion PDF automatique pour archivage (le .docx est la source, le PDF est le dérivé)
  - **Flows compatibles** :
    - Flow 1 (Word local) : Export → Ouvrir Word → Éditer → Upload Drive
    - Flow 2 (Google Docs) : Export → Upload Drive → Ouvrir Google Docs
    - Flow 3 (Admission externe) : Export vers dossier patient existant
  - **Avantages vs PDF** : Éditable, collaboration multi-utilisateurs, standard attendu pour dossiers patients/bilans/comptes-rendus

#### Nettoyage et optimisations
- **Suppression bouton Rechercher** : Recherche se fera au niveau Dossier/Patient via SQL full-text sur colonnes `notes_txt`, pas de besoin de recherche dans une note isolée
- **Optimisation marges** : Augmentation de 8px à 10px (gauche/droite) pour meilleure lisibilité
- **Raccourcis clavier standards** : Ctrl+B (Gras), Ctrl+I (Italique), Ctrl+U (Souligné), Ctrl+Z (Annuler), Ctrl+Y (Rétablir), Ctrl+P (Imprimer)

#### Form de test
- **Création de `UI/Forms/Test/TestRichTextEditor.vb`** : Form de validation manuelle complète
  - Chargement de contenu de test
  - Simulation sauvegarde/chargement depuis base de données
  - Test modes ReadOnly et ShowToolbar
  - Validation de tous les boutons de formatage
  - Tests d'impression et exports (PDF + Word)
  - Bouton d'accès provisoire dans `Home.vb` pour tests

#### Documentation complète
- **`Docs/UC_RichTextEditor_Documentation.md`** (589 lignes) :
  - Vue d'ensemble et architecture
  - Guide d'utilisation complet (chargement, sauvegarde, intégration)
  - Référence exhaustive des fonctionnalités
  - Section impression et mise en page
  - Section export PDF avec Syncfusion
  - Section export Word avec intégration POC documentaire
  - Tableau comparatif PDF vs Word
  - Exemples d'intégration métier (anamnèses, bilans, consultations)
  - Guide maintenance et extension
- **`Docs/Historique_Implementation_RichTextEditor.md`** (468 lignes) :
  - Journal détaillé V1.0 à V1.6 avec toutes les itérations
  - Résolution des 10 problèmes rencontrés (pages blanches, aperçu forcé, etc.)
  - Décisions techniques argumentées
- **`Docs/Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md`** (479 lignes) :
  - Plan de tests exhaustif avec 120+ cas de test
  - 8 sections : Chargement, Formatage caractères, Presse-papiers, Annuler/Rétablir, Formatage paragraphes, Insertion, Modes, Impression/Export
- **`Docs/Guide_Licence_Syncfusion.md`** : Guide complet pour obtention licence Community gratuite

#### Intégration thématique
- **Respect charte graphique Althéa** :
  - Toolbar : `ColorSaugeClair` (178, 197, 186)
  - Boutons actifs : `ColorSauge` (122, 155, 135)
  - Fond éditeur : `ColorBeigeClair` (244, 239, 234)
  - Texte : `ColorTexte` (74, 74, 74)
- **Glyphes Unicode** pour icônes toolbar (léger, pas de dépendances images)

#### Prêt pour l'intégration
- **Modules métier cibles** :
  - `UC_PatientDetails` : Notes générales patient
  - `UC_DossierMedical` : Anamnèses, bilans psychologiques/graphothérapie
  - `UC_Consultation` : Comptes-rendus de consultations/séances
  - `UC_Ordonnance` : Notes prescriptions/recommandations
  - `UC_DocumentsPatient` : Notes associées aux documents
- **Base de données** : Colonnes `notes_rtf` (formatage) + `notes_txt` (recherche full-text) à ajouter aux tables concernées

#### Technologies utilisées
- **WinForms .NET 8.0** avec VB.NET (Option Strict On)
- **Syncfusion Community Edition v33.2.10** (DocIO, DocToPDFConverter, Pdf, Licensing)
- **API Windows** : `EM_FORMATRANGE` pour impression RTF native, `EM_SETMARGINS` pour marges internes
- **PrintDocument** : Gestion impression avec `PrintDialog`, `PrintPreviewDialog`, `PageSetupDialog`

#### Avantages pour les utilisateurs (psychologues, graphothérapeutes)
- ✅ **Édition riche** : Formatage professionnel des bilans et comptes-rendus
- ✅ **Export éditable** : Documents Word pour collaboration (écoles, médecins traitants, parents)
- ✅ **Archivage PDF** : Documents finalisés non modifiables
- ✅ **Templates réutilisables** : Modèles d'anamnèses et bilans pré-formatés
- ✅ **Recherche efficace** : Full-text SQL sur `notes_txt` au niveau Dossier/Patient
- ✅ **Intégration cloud** : Synchronisation automatique Google Drive (POC)
- ✅ **Conformité légale** : Documents patients éditables + archivage PDF sécurisé

---

## 📅 05/06/2026 - 06/06/2026

### Remplacement complet MessageBox → DialogChoix

#### UI / UX
- **Remplacement systématique** de tous les `MessageBox.Show` par le nouveau composant `DialogChoix` dans les modules UI suivants :
  - `UC_AdminHome.vb` : 3 remplacements (erreurs système)
  - `UC_Parametres.vb` : 3 remplacements (erreur, confirmation, validation)
  - `Home.vb` : 4 remplacements (confirmation élévation, erreurs, information debug)
  - `ElevationAcces.vb` : 1 remplacement (erreur authentification)
  - `UC_Utilisateurs.vb` : 4 remplacements (confirmation activation/désactivation, succès, erreurs, avertissements)
  - `UtilisateurEdition.vb` : **19 remplacements** (création/modification utilisateur, validation formulaire, reset mot de passe, déverrouillage compte, gestion erreurs et exceptions)

#### Résultat
- **34 MessageBox remplacés au total** dans tous les modules UI
- Interface utilisateur homogène avec icônes GIF animées personnalisées
- Cohérence visuelle respectant la charte graphique Althéa

---

## 📅 03/06/2026

### Création composant DialogChoix personnalisé

#### Nouveau composant UI
- **Création de `UI/Forms/Communs/DialogChoix.vb`** : Dialog personnalisé remplaçant MessageBox
  - Enum `TypeDialogue` : Information, Warning, Error, Question, Success, Loading, Processing
  - Configuration flexible de 1 à 3 boutons via méthodes `SetBoutons()`
  - Mapping DialogResult : Bouton1=Yes, Bouton2=No, Bouton3=Cancel
  - Méthodes statiques simplifiées : `Information()`, `Erreur()`, `Avertissement()`, `Succes()`, `Confirmer()`, `Question()`
  - Support des icônes GIF animées via `My.Resources`
  - Thématisation complète via `UITheme`
  - Taille adaptative selon le contenu du message

#### Utilitaires
- **Création de `Utils/UtilsIcons.vb`** : Centralisation de la gestion des icônes d'état
  - `IconOK()` : État actif/valide (icône verte)
  - `IconOFF()` : État inactif/désactivé (icône rouge/grise)
  - `IconLock()` : Compte verrouillé (cadenas)
  - `IconNo()` : Refus/interdiction
  - Support multi-tailles : 16x16, 20x20, 26x26, 32x32
  - Chargement centralisé depuis `My.Resources`

#### Intégration
- Refactorisation `UC_Utilisateurs.vb` et `UC_Parametres.vb` pour utiliser `UtilsIcons`
- Gestion prioritaire des icônes d'état : CompteVerrouillé → IconLock, Actif → IconOK, Inactif → IconOFF

---

## 📅 01/06/2026 - 02/06/2026

### Amélioration système de recherche et consultation utilisateurs

#### UC_Utilisateurs - Système de filtres
- **Recherche multi-critères** :
  - Filtre texte : recherche dans login, nom affichage, code utilisateur
  - Filtre par rôle : User, SuperUser, Admin
  - Filtre par date de dernier login (avec gestion `.Date` pour comparaison exacte)
  - Filtre comptes verrouillés uniquement
- **Bouton Réinitialiser filtres** : remise à zéro de tous les critères de recherche
- Gestion d'état du bouton Réinitialiser (activé uniquement si filtres actifs)
- Filtrage en mémoire sur `_utilisateurs` (performance optimisée)

#### UtilisateurEdition - Mode Consultation
- **Nouveau mode `ModeUtilisateurEdition.Consultation`** pour SuperUser :
  - Tous les champs en lecture seule
  - Bouton Enregistrer masqué
  - Accès limité aux actions de maintenance :
    - Déverrouillage de compte (si verrouillé)
    - Réinitialisation de mot de passe
  - Titre et contexte spécifiques : "Consultation utilisateur"

#### UC_AdminHome - Accès SuperUser
- Accès de SuperUser à `UC_Utilisateurs` en mode consultation
- Boutons création/modification masqués pour SuperUser
- Double-clic sur utilisateur → ouverture en mode consultation

---

## 📅 23/05/2026 - 30/05/2026

### Module complet de gestion utilisateurs

#### UI / Forms
- **`UI/Forms/Utilisateur/UtilisateurEdition.vb`** (Form modale) :
  - Enum `ModeUtilisateurEdition` : Création, Modification, Consultation
  - Mode Création : génération et affichage du mot de passe temporaire, copie automatique dans presse-papiers
  - Mode Modification : login en lecture seule, gestion des états de sécurité
  - Actions administratives :
    - Réinitialisation de mot de passe via `ChangePassword` en mode `AdminReset`
    - Déverrouillage de compte avec confirmation
  - Validation exhaustive du formulaire (champs obligatoires, cohérence des rôles)
  - Gestion des états de sécurité : verrouillage, nombre d'échecs login, dates importantes
  - Implémentation `IContextAwareForm` pour intégration contexte UI

- **`UI/Controls/UC_Utilisateurs.vb`** (UserControl d'administration) :
  - Liste complète des utilisateurs avec DataGridView thématisée
  - Colonnes : État (icône), Code, Login, Nom, Rôle, Max Élévation, Actif, Verrouillé, Dernier login
  - Actions : Créer, Modifier, Activer/Désactiver (avec confirmation)
  - Double-clic sur ligne → ouverture `UtilisateurEdition` en modification
  - Gestion des droits selon le rôle connecté (Admin complet, SuperUser consultation)
  - Refresh automatique après chaque action
  - Implémentation `IContextAwareUserControl` pour injection du contexte

#### Métier / Security
- **`Metier/Security/GestionUtilisateurs.vb`** :
  - `CreateUtilisateur()` : création avec mot de passe hashé, gestion séquence MariaDB `LASTVAL(seq_sec_utilisateurs)`
  - `UpdateUtilisateur()` : modification avec vérification des droits (Admin uniquement)
  - `GenererMotDePasseTemporaire()` : génération sécurisée de mots de passe (12 caractères, complexité garantie)
  - `ResetPasswordUtilisateur()` : réinitialisation avec mot de passe temporaire auto-généré
  - `ResetPasswordUtilisateurWithCustomPassword()` : réinitialisation avec mot de passe personnalisé (via admin)
  - `DeverrouillerUtilisateur()` : déverrouillage de compte avec vérification droits (Admin ou SuperUser)
  - `ActiverDesactiverUtilisateur()` : activation/désactivation de compte (Admin uniquement)
  - `GetUtilisateurPourEdition()` : récupération d'un utilisateur complet pour édition/consultation
  - `GetUtilisateursPourListe()` : récupération de la liste complète pour affichage DataGridView
  - Journalisation détaillée de toutes les actions de sécurité via `GestionLog`

#### Gestion mot de passe centralisée
- **`UI/Forms/Login/ChangePassword.vb`** :
  - Refactorisation complète pour supporter deux modes :
    - `ModeChangePassword.UserChange` : changement par l'utilisateur (validation ancien mot de passe)
    - `ModeChangePassword.AdminReset` : réinitialisation par Admin (sans validation ancien mot de passe)
  - Validation de la complexité du nouveau mot de passe via `PasswordSecurityHelper`
  - Double saisie du nouveau mot de passe avec vérification de correspondance
  - Intégration dans `Home.vb` via bouton "Changer mot de passe" (menu principal)
  - Mise à jour automatique du flag `must_change_password` après changement réussi

#### Base de données
- Correction requête création utilisateur : utilisation de `LASTVAL(seq_sec_utilisateurs)` au lieu de `LAST_INSERT_ID()` pour récupération de l'ID généré par la séquence MariaDB
- Validation du type de paramètre pour les rôles : passage en String au lieu de Int32

#### Tests et validation
- **Création de `Docs/Tests/PLAN_TESTS_UC_UTILISATEURS.md`** : plan de tests exhaustif couvrant :
  - Chargement et affichage (liste, icônes, filtres)
  - Création d'utilisateur (validation, génération mot de passe, droits)
  - Modification d'utilisateur (champs, états, validation)
  - Activation/désactivation de compte
  - Réinitialisation de mot de passe
  - Déverrouillage de compte
  - Droits et sécurité (Admin vs SuperUser)
  - Robustesse et gestion d'erreurs
  - Logs et audit
  - Cohérence UI
  - Non-régression

- Tests manuels effectués et corrections appliquées :
  - Correction récupération ID après insertion (MariaDB sequence)
  - Correction type de paramètre pour rôles (String vs Int32)
  - Correction effacement sélection après chargement liste
  - Correction texte dynamique bouton Activer/Désactiver
  - Correction gestion icônes d'état (OK/OFF/Lock)
  - Désactivation bouton Reset Password en mode création
  - Initialisation correcte du contexte UI au chargement

---

## 📅 17/05/2026

### Audit complet du projet et synchronisation documentation

#### Audit et analyse
- **Audit approfondi** de l'ensemble du code source Althéa
- Analyse de la structure, de l'architecture, de la sécurité et de la qualité du code
- Identification des points d'amélioration et des bonnes pratiques déjà en place
- Vérification de la cohérence entre le code et la documentation existante

#### Documentation créée
- **`Docs/RAPPORT_AUDIT_2026-05-17.md`** : Rapport d'audit complet disponible dans `Docs/`
  - État global du projet
  - Points forts identifiés
  - Axes d'amélioration
  - Recommandations pour la suite du développement

#### Documentation mise à jour
- **`CHANGELOG.md`** : Synchronisation avec l'état réel du projet
- **`Docs/ETAT_DU_PROJET.md`** : Mise à jour de l'état actuel et des prochaines étapes

#### Conclusion audit
Projet en excellent état pour reprise :
- Aucun bug critique
- Architecture solide et documentée
- Sécurité robuste
- Code propre et maintenable
- Build qui passe

**Prêt pour la suite du développement**

---

## 📅 31/03/2026
- Création de la database de développement, avec les tables et les relations définies. 
---

## 📅 01/04/2026
- Réflexion sur la gestion des dossiers, documents et fichiers, avec les différentes options possibles.
---

## 📅 09/04/2026
- Etablissement de la charte graphique de l'application, avec les couleurs, les icônes et les éléments visuels.
- Définition des règles de développement et de gestion du projet, avec les bonnes pratiques à suivre.
- Création de la documentation technique pour l'interface utilisateur, avec les composants et les interactions.
- Préparation de la documentation technique de la base de données, avec les schémas et les relations.
---

## 📅 10/04/2026
- Ajout table medecins
- Ajout autres_suivis_patient
- Ajout système documents et modèles
---

## 📅 20/04/2026

### 🎉 Initialisation du projet

#### 🧱 Architecture
- Mise en place de la structure de base de la solution
- Validation des principes fondamentaux :
  - Form unique (Shell)
  - Navigation via UserControls
  - Séparation stricte UI / métier
  - Point d’accès DB unique

#### 🎨 Interface Utilisateur
- Création de la Form principale `Home`
- Implémentation de la structure Shell :
  - Menu gauche
  - Header (contexte)
  - Zone centrale dynamique
  - StatusStrip

- Application de la charte graphique :
  - Palette couleurs validée
  - Icônes intégrées
  - Style professionnel et épuré

#### 🧠 UX / Métier
- Organisation du menu principal :
  - Patients
  - Domaines
  - Agenda
  - Documents
  - Référentiels
  - Paramètres

- Décision :
  - Suppression du menu "Séances" (logique métier respectée)

#### ⚙️ Technique
- Décision d’utiliser le Designer pour le layout
- Mise en place du principe :
  - UI statique (Designer)
  - comportements dynamiques (code)

- Préparation du module `UtilsForm` (à implémenter)

### 📌 État
- UI principale fonctionnelle
- Architecture validée
- Socle prêt pour démarrage du code
---

## 📅 21/04/2026

### Added
- Mise en place du point d’entrée manuel (`Program.vb`)
- Création structure projet (Core / UI / Security)
- Ajout classe `LocalDbConfig`
- Ajout module `ConfigManager` (JSON local)
- Ajout module `DatabaseManager` (connexion MariaDB)
- Ajout module `CryptoManagerDPAPI` (chiffrement DPAPI)

### UI
- Création Form `Home` (shell)
- Création Form `ConfigurationConnexion`
- Mise en place design TableLayoutPanel

---

## 📅 22/04/2026

### Added
- Intégration `GestionLog` (adapté Artefact)
- Ajout gestion état UI :
  - `_isFormDirty`
  - `_isConnectionTestSuccessful`

### Added
- Processus complet :
  - test connexion DB
  - enregistrement config JSON
  - chargement config existante

### Security
- Ajout chiffrement mot de passe via DPAPI
- Mise en place utilisateur DB dédié (`althea_app`)

### UX
- Masquage mot de passe (`********`)
- Validation dynamique des champs
- Désactivation bouton Enregistrer sans test valide

### Fixed
- Gestion correcte host (`127.0.0.1`)
- Correction noms contrôles StatusStrip

### In Progress
- Gestion avancée du mot de passe (conserver/remplacer)
- Intégration complète du logging dans tous les modules
- Mise en place `AppStartupManager`
---

## 📅 22/04/2026

### Added
- Mise en place du point d’entrée manuel (`Program.vb`)
- Création structure projet (Core / UI / Security)
- Ajout classe `LocalDbConfig`
- Ajout module `ConfigManager` (JSON local)
- Ajout module `DatabaseManager` (connexion MariaDB)
- Ajout module `CryptoManagerDPAPI` (chiffrement DPAPI)

### UI
- Création Form `Home` (shell)
- Création Form `ConfigurationConnexion`
- Mise en place design TableLayoutPanel

### Added
- Intégration `GestionLog` (adapté Artefact)
- Ajout gestion état UI :
  - `_isFormDirty`
  - `_isConnectionTestSuccessful`

### Added
- Processus complet :
  - test connexion DB
  - enregistrement config JSON
  - chargement config existante

### Security
- Ajout chiffrement mot de passe via DPAPI
- Mise en place utilisateur DB dédié (`althea_app`)

### UX
- Masquage mot de passe (`********`)
- Validation dynamique des champs
- Désactivation bouton Enregistrer sans test valide

### Fixed
- Gestion correcte host (`127.0.0.1`)
- Correction noms contrôles StatusStrip

### In Progress
- Gestion avancée du mot de passe (conserver/remplacer)
- Intégration complète du logging dans tous les modules
- Mise en place `AppStartupManager`
---

## 📅 23/04/2026 - 24/04/2026 - 25/04/2026 -

### 🧠 Architecture / UI

- Validation complète du modèle **UserControl pour navigation**
- Abandon définitif des Forms pour les écrans principaux
- Mise en place du pattern :
  - Form `Home` = Shell
  - `UserControls` = écrans métier injectés

### 🖥️ UI – Structure

- Clarification du rôle de `pnlCenter` :
  - zone unique d’injection des vues
- Mise en place d’une méthode standard de chargement :

`Private Sub LoadView(view As UserControl)
    pnlCenter.Controls.Clear()
    view.Dock = DockStyle.Fill
    pnlCenter.Controls.Add(view)
End Sub`

### 🎛️ UI – Boutons standards
Validation du modèle unique de boutons :
- Centralisation du comportement via futur module UtilsForm
Décision :
- aucun code métier directement dans les événements boutons
- tout passe par des méthodes métier dédiées

### 🏠 UI – Home
Clarification du rôle :
- uniquement navigation + contexte
- aucune logique métier
- Ajout du concept :
- application verrouillée tant que DB non connectée

### 🗄️ Database
Stabilisation du flux :
- test connexion obligatoire avant sauvegarde config
- Validation du pattern :
- config locale JSON + DPAPI
Confirmation :
- DatabaseManager = point d’accès unique

### 🔐 UX / Sécurité
- Blocage de l’application si connexion invalide
- Validation stricte des champs avant test
- Interdiction d’enregistrer sans test réussi

### 📌 Décisions structurantes
- UI = 100% découplée du métier
- Navigation = centralisée
- DB = accès unique via manager
- Configuration = locale + sécurisée
---

## 📅 26/04/2026

### Added

- Création du `UserControl` `UC_Accueil`
- Création du `UserControl` `UC_AdminHome`
- Ajout bouton "Connexion Database" dans `UC_AdminHome`
- Création classe `NavigationManager`
- Implémentation navigation centralisée via `Navigate`

### Changed

- Refactoring navigation dans `Home` :
  - introduction méthode `NavigateTo`
  - remplacement de `LoadView` par `NavigationManager`
- Chargement automatique de `UC_Accueil` au démarrage
- Mise à jour gestion des boutons menu (cohérence UI complète)

### Removed

- Suppression bouton temporaire `ConfigurationConnexion` dans `Home`
- Suppression `lblContentPlaceholder`
- Suppression entrée "Paramètres" du menu principal

### Architecture

- Mise en place séparation :
  - Shell (`Home`)
  - Navigation (`NavigationManager`)
  - UI (UserControls)
- Préparation intégration future `UC_Parametres`

### UX

- Correction incohérence :
  - bouton sélectionné sans contenu affiché
- Navigation fluide entre écrans
---

## 📅 27/04/2026

### Documentation

#### Changed
- Refonte complète du README :
  - restructuration logique
  - conservation des informations existantes
  - amélioration lisibilité et cohérence

- Refonte complète du fichier Rules :
  - suppression des doublons
  - regroupement par thématiques
  - clarification des règles UI / DB / architecture

### UI

#### Changed
- Suppression du menu "Paramètres" du menu principal
- Intégration des paramètres dans l’espace Admin

#### Added
- Structuration de `UC_AdminHome` en hub (tuiles)
- Création du UserControl `UC_Parametres`
- Ajout TabControl pour gestion des groupes de paramètres

### Architecture

#### Added
- Création classe métier `ParametreApplication`
- Mise en place séparation UI / Métier / DB pour les paramètres

#### Added
- Création module `QueryParametres`
- Première requête SELECT des paramètres actifs

#### Added
- Création module `GestionParametres`
- Implémentation méthode `GetParametresActifs`

#### Changed
- Correction utilisation DatabaseManager :
  - remplacement de `GetConnexionMariaDB`
  - utilisation de `CreateConnection()` + `Open()`

### Structure projet

#### Added
- Nouveau dossier :
  Core/Database/Queries/

- Nouveau dossier :
  Metier/Parametres/

### Technical

#### Decision
- Validation architecture :
  UI → Métier → Query → DatabaseManager

- Adoption stratégie :
  1 module de queries par domaine

### Status

- UC_Parametres structuré
- Couche métier prête
- Lecture DB opérationnelle
- UI dynamique non encore implémentée
---

## 📅 28/04/2026

### Database

#### Added

* Création du module `QueryParametres`
* Implémentation de la requête `SelectParametresActifs`

#### Added

* Insertion de données initiales dans `tec_parametres` :

  * groupes : `GENERAL`, `PATHS`, `TECHNIQUE`
  * paramètres de chemins (APP_DATA, PATH_DOCUMENT, etc.)
  * exemple de paramètre BOOL

### Métier

#### Added

* Création de la classe `ParametreApplication`
* Création du module `GestionParametres`
* Implémentation de la méthode `GetParametresActifs`

#### Fixed

* Correction du mapping Reader → propriétés métier
* Prise en compte des champs NULL (valeur_parametre, description_parametre)

### Core

#### Added

* Ajout de la méthode `OpenConnection()` dans `DatabaseManager`

#### Changed

* Standardisation de l’accès DB :

  * CreateConnection() → création
  * OpenConnection() → ouverture
* Suppression des vérifications inutiles de type :

  * `conn IsNot Nothing`
  * `conn.State`

### UI

#### Added

* Création du UserControl `UC_Parametres`
* Chargement des paramètres depuis la DB
* Affichage du nombre de paramètres chargés

#### Added

* Regroupement des paramètres par `groupe_parametre`

#### Added

* Génération dynamique des onglets (`TabControl`) selon les groupes

#### Added

* Génération dynamique des contrôles :

  * Label + TextBox
  * Label + CheckBox (BOOL)

### UX

#### Added

* Structure UI dynamique basée sur la base de données

#### Identified

* Limites de l'affichage actuel (non adapté à la gestion complète)

### Architecture

#### Decision

* Centralisation des requêtes SQL dans `Core/Database/Queries`
* Séparation stricte :

  * UI
  * Métier
  * DB

#### Decision

* Les chemins sont définis comme segments logiques en base
* Construction des chemins via `Path.Combine`

### Security / Functional

#### Decision

* Interdiction de suppression physique des paramètres
* Gestion future par niveaux d'accès :

  * ADMIN
  * SUPERUSER

### Status

* Chargement DB fonctionnel
* UI dynamique fonctionnelle (version technique)
* Écran de gestion avancée à concevoir
---

## 📅 29/04/2026 & 30/04/2026

### Ajout

- Implémentation complète CRUD logique des paramètres
  - Création (INSERT)
  - Modification (UPDATE)
  - Désactivation (soft delete)
- Gestion des modes d’accès (Admin / SuperUser)
- Ajout du panneau détail dynamique
- Ajout du champ Description multiligne
- Ajout d’un système de validation avant sauvegarde
- Ajout d’un style standard DataGridView via UtilsForm

### Amélioration

- Conservation de la sélection après sauvegarde
- Synchronisation grille ↔ détail
- Amélioration de la gestion des états UI (boutons, édition)
- Amélioration UX (annulation, confirmation, cohérence)

### Correction

- Correction du bug de sauvegarde (valeurs non injectées dans l’objet)
- Correction du focus après rechargement
- Correction incohérences visuelles (labels, sélection)

### Ajout

- Chargement dynamique des paramètres par groupe
- Mise en place du DataGridView et du panneau détail
- Introduction des structures ParametreApplication
- Mise en place des premières règles UI (édition contrôlée)

### Amélioration

- Structuration du code (régions, séparation logique)
- Introduction des outils de log

### Correction

- Correction erreurs de connexion DB
- Correction gestion des contrôles dynamiques (WithEvents)
------

## 📅 01/05/2026

### ✨ Ajouts
- Mise en place UI globale (stsStatus, ttMain, errProvider)
- Début structuration Navigation + Context

### 🔄 Modifications
- suppression MessageBox inutiles
- centralisation feedback utilisateur

------

## 📅 02/05/2026

### ✨ Ajouts

- Ajout gestion affichage paramètres désactivés (CheckBox + SQL)
- Ajout validation Type/Valeur via UtilsValidation
- Ajout normalisation technique via UtilsString
- Ajout contrôle unicité clé paramètre

### 🔄 Modifications

- Remplacement TextBox Type par ComboBox
- Refonte complète validation (errProvider + messages + logs)
- Amélioration gestion contexte (header/status)
- Stabilisation événements UI (SelectionChanged / TabControl)

### 🐛 Corrections

- Bug SelectedTab = Nothing
- Perte du détail après reload
- Type non pris en compte à la sauvegarde
- erreurs persistantes errProvider

### 🧱 Architecture

- séparation stricte Query / Gestion / UI
- factorisation validation
- préparation réutilisation multi-UC

------

## 📅 03/05/2026

#### UC_Parametres

### Added

- Ajout de contrôles dynamiques selon `TypeValeur` :
  - `BOOL` → `CheckBox`
  - `INT` → `NumericUpDown`
  - `DECIMAL` → `NumericUpDown`
  - `DATE` → `DateTimePicker`
  - `STRING` / `PATH` → `TextBox`
- Ajout d’une colonne d’état dans le `DataGridView`.
- Ajout d’icônes actif / désactivé adaptées à la charte Althéa.
- Ajout de tooltips sur l’état des paramètres.
- Ajout d’une ComboBox éditable pour le champ Groupe, avec liste des groupes existants.

### Changed

- Refactorisation de `ChargerParametres`.
- Extraction de :
  - `CreerDataGridParametres`
  - `ConfigurerDataGridParametres`
  - `InitialiserSelection`
- Adaptation de `LireValeursDepuisUI` pour gérer les contrôles dynamiques.
- Stockage des dates en format ISO `yyyy-MM-dd`.
- Normalisation du groupe via `UtilsString.NormalizeTechnicalCode`.

### Fixed

- Correction du faux doublon de clé technique lors de la modification d’un paramètre existant.
- La vérification d’unicité de `CleParametre` est maintenant limitée au mode Nouveau.
- Suppression de doublons dans l’initialisation de sélection.
- Stabilisation du rechargement des détails après changement d’affichage actif/inactif.

### Decisions

- `UC_Parametres` reste le modèle de référence pour les futurs UserControls.
- Pas de factorisation commune prématurée des méthodes spécifiques à UC_Parametres.
- Les méthodes extraites restent locales tant que les répétitions ne sont pas confirmées dans d’autres UC.
---

## 📅 07/05/2026

### Added

* Création table `sec_utilisateurs`
* Création premier compte Admin bootstrap
* Implémentation PBKDF2 + salt
* Création `PasswordSecurityHelper`
* Création `GestionUtilisateurs`
* Création `UtilisateurApplication`
* Création `UserSession`
* Création `AppRole`
* Intégration Login réel dans `Home`

### Changed

* Suppression session DEV_ADMIN
* Session utilisateur désormais issue du Login réel

### Security

* Aucun mot de passe stocké en clair
* Aucun mot de passe dans les logs
* Hash + salt uniques par utilisateur
* SQL sécurité centralisé

### UI

* Création Designer `ChangePassword`
* Préparation UX sécurité
---

## 📅 09/05/2026

### Added

- Classe `AuthenticationResult`

- Gestion des échecs login persistants

- Verrouillage automatique des comptes après 5 échecs

- Colonnes DB :

  - `compte_verrouille`

  - `date_verrouillage`

- Méthodes métier :

  - `IncrementerNbEchecsLogin`

  - `VerrouillerCompteUtilisateur`

  - `ReinitialiserNbEchecsLogin`

  - `ChangerMotDePasse`

- Requêtes SQL sécurité associées

- Messages UI contextualisés :

  - essais restants

  - verrouillage compte

### Changed

- Refactoring complet de `AuthentifierUtilisateur`

- `Login.vb` utilise désormais `AuthenticationResult`

- Mapping utilisateur enrichi :

  - `CompteVerrouille`

  - `DateVerrouillage`

### Security

- Verrouillage persistant DB
- Réinitialisation compteur après succès
- Suppression des retours `Nothing` bruts
- Contrôle centralisé des messages sécurité
---

## 📅 10/05/2026

### Added

- Constructeur sécurisé pour `ChangePassword`
- Validation locale des saisies utilisateur
- Validation règles complexité mot de passe
- Gestion des messages validation :
  - `ErrorProvider`
  - `lblMessage`
  - `stsLabelStatus`
- Gestion changement réel de mot de passe
- Appel métier `GestionUtilisateurs.ChangerMotDePasse`
- Préparation affichage utilisateur connecté dans `Home`

### Changed

- Refactoring architecture `ChangePassword`
- Séparation stricte UI / sécurité / DB
- Amélioration UX validation mot de passe

### Security

- Validation ancien mot de passe
- Validation complexité minimale
- Aucune logique sécurité dans l’UI
- Aucun mot de passe loggé
- `must_change_password` désactivé après succès

### UI

- Ajout futur `lblPasswordRules`
- Ajout `lblUtilisateurConnecte`
- Messages validation désormais visibles dans StatusStrip
---

## 📅 11/05/2026

### Added

- Flux complet `MustChangePassword`
- Validation avancée `ChangePassword`
- Boutons visibilité password
- Affichage utilisateur connecté dans Home
- Contrôle accès `AdminHome`
- Gestion dynamique des droits dans `UC_AdminHome`
- Support `role_max_elevation`
- Architecture élévation temporaire
- Vérification métier `VerifierElevationUtilisateur`
- Formulaire `ElevationAcces`
- Élévation temporaire via `UserSession.ElevateTo`

### Changed

- Refonte architecture élévation
- Suppression logique passwords partagés
- Passage à élévation utilisateur individuelle
- Amélioration UX sécurité
- Mise à jour mapping utilisateurs

### Removed

- `GetUtilisateursActifsByRole`
- `VerifierMotDePasseRole`

### Security

- Validation élévation sécurisée
- Contrôle rôle maximum autorisé
- Aucune modification DB pendant élévation
- Logs sécurité élévation
- Passwords jamais loggués

### Database

- Ajout colonne :
  - `role_max_elevation`
  ---

## 📅 12/05/2026

### Added

- Champ DB `role_max_elevation`
- Architecture élévation utilisateur individuelle
- Validation métier `VerifierElevationUtilisateur`
- Support élévation session temporaire
- Boutons :
  - `btnEleverAcces`
  - `btnRetourRoleBase`
- Méthode `SetContexte`
- Navigation retour Accueil
- Affichage état élévation dans Home
- Bouton Voir dans ElevationAcces

### Changed

- Refonte complète logique élévation
- Suppression logique comptes/passwords partagés
- Synchronisation :
  - lblContexte
  - stsLabelStatus
- Recalcul dynamique droits AdminHome
- Gestion contexte navigation

### Security

- Élévation via password utilisateur personnel
- Contrôle RoleMaxElevation
- Retour sécurisé rôle de base
- Sortie automatique zones interdites
- Logs sécurité élévation
- Aucun password loggué

### UX

- Affichage clair état élévation
- Cohérence navigation/contextes
- Retour automatique Accueil si perte droits
---

## 📅 13/05/2026

## UI Architecture

### Added
- Nouveau module `UITheme`
- Nouveau module `UtilsButtons`
- Nouveau module `UtilsDataGrid`

### Changed
- Extraction complète des responsabilités UI de `UtilsForm`
- Centralisation des couleurs dans `UITheme`
- Migration des boutons vers `UtilsButtons`
- Migration DataGrid vers `UtilsDataGrid`

### Refactoring
- Suppression progressive du monolithe `UtilsForm`
- Séparation :
  - style visuel
  - comportements boutons
  - style DataGrid

## Buttons

### Added
- Gestion centralisée :
  - boutons Home
  - boutons standards 32px
  - boutons larges 48px

### Improved
- Gestion handlers sécurisée
- Gestion images fallback
- Gestion états hover/down/disabled

## UITheme

### Added
- Palette Althéa centralisée
- Couleurs DataGrid
- Couleurs contrôles dynamiques
- Couleurs boutons
- Assets paths
- Image suffixes

## UC_Parametres

### Changed
- Remplacement des couleurs hardcodées
- Utilisation de `UITheme`

## Security

### Decision
- Clôture du Process Sécurité avant démarrage UC_Utilisateurs

### Architecture
- Séparation :
  - sécurité applicative
  - gestion utilisateurs
---

## 📅 16/05/2026

### UI / Design

#### Added
- Ajout d’un module `UtilsControls` pour centraliser le comportement visuel des contrôles WinForms standards.
- Ajout d’un rendu OwnerDraw pour les ComboBox générées dynamiquement.
- Ajout d’un rendu personnalisé des items ComboBox afin d’éviter la sélection bleue Windows dans les listes déroulantes.
- Ajout de `TextRenderer.DrawText` pour améliorer la netteté du texte dans les ComboBox OwnerDraw.
- Ajout d’une initialisation centralisée des ToolTips via `InitializeToolTips()` dans les Forms concernées.

#### Changed
- Clarification de la séparation :
  - `UITheme` = couleurs, constantes, chemins
  - `UtilsButtons` = boutons
  - `UtilsDataGrid` = DataGridView
  - `UtilsControls` = contrôles WinForms standards
- Amélioration du rendu des ComboBox dynamiques de `UC_Parametres`.
- Ajustement du rendu vertical des items ComboBox via `ItemHeight` et dessin dans un Rectangle.
- Regroupement des ToolTips dans des procédures dédiées au lieu d’initialisations dispersées.

#### Fixed
- Correction du texte trop fin / tremblotant dans les ComboBox OwnerDraw.
- Correction de l’alignement vertical du premier item dans les listes ComboBox.
- Correction de la sélection visuelle dans les listes déroulantes ComboBox.

### UI / Contexte global

#### Added
- Création du principe `IContextAwareForm` pour les Forms modales ouvertes depuis `Home`.
- Injection possible de `UserControlContext` dans les Forms modales.
- Utilisation du contexte global pour `ElevationAcces`.
- Utilisation du contexte global pour `ConfigurationConnexion`.

#### Changed
- Clarification entre :
  - Forms autonomes ouvertes avant `Home`
  - Forms modales ouvertes depuis `Home`
- `Login` et `ChangePassword` conservent leur `StatusStrip`, `ToolTip` et `ErrorProvider` locaux.
- `ElevationAcces` et `ConfigurationConnexion` utilisent le contexte global quand elles sont ouvertes depuis `Home`.

#### Removed
- Suppression du `StatusStrip` local dans les écrans où le contexte global de `Home` doit être utilisé.
- Suppression de la logique locale de status dans `UC_AdminHome`.

### Architecture

#### Decision
- Les UserControls hébergés dans `Home` doivent utiliser `IContextAwareUserControl` et `UserControlContext`.
- Les Forms modales ouvertes depuis `Home` peuvent utiliser `IContextAwareForm`.
- Les Forms ouvertes avant `Home` restent autonomes.
- Les ToolTips doivent être regroupés dans une méthode `InitializeToolTips()`.
- Les ToolTips de `UC_AdminHome` sont reportés jusqu’à stabilisation complète de l’écran Admin.

### Documentation à mettre à jour
- `Rules.md`
- `Documentation_technique_UI_Althea.md`
- sections :
  - Home
  - UC_Parametres
  - UC_AdminHome
  - ConfigurationConnexion
  - ElevationAcces
  - Login
  - ChangePassword
  ---

  ## 📅 17/05/2026

  Corrections de quelques bugs relevés par Copilot:

  ### 1. NavigationManager.Navigate provoque des fuites mémoire
  `_pnlContent.Controls.Clear()` enlève les contrôles du panel mais ne les dispose pas. Si un UserControl possède des ressources (handlers, images, connexions), elles ne seront jamais libérées.

 À corriger :

 `For Each ctrl As Control In _pnlContent.Controls
    ctrl.Dispose()
Next
_pnlContent.Controls.Clear() `

  ### 2. PurgeOldLogs appelée à chaque log - dans le SyncLock
La purge est déclenchée à chaque appel de `EcrireLog`, y compris à l'intérieur du verrou critique. Elle accède au système de fichiers à chaque log. Il faut purger une seule fois au démarrage ou une fois par session.

  ### 3. IncrementerNbEchecsLogin : objet en mémoire non synchronisé
Dans `AuthentifierUtilisateur`, après `IncrementerNbEchecsLogin(utilisateur)`, la propriété `utilisateur.NbEchecsLogin` est lue pour calculer les tentatives restantes. Si la méthode ne met pas à jour l'objet en mémoire (seulement la DB), le calcul `MaxEchecsLogin - utilisateur.NbEchecsLogin` sera incorrect.

 ### 4. LocalDbConfig.LastConnectionTestUtc n'est jamais mise à jour
La propriété existe, mais aucun appel à `SaveLocalDbConfig` ne la renseigne après un test de connexion réussi dans `AppStartupManager`. Donnée en dérive.

  ### 5. Validation de port incomplète dans LocalDbConfig
`Port > 0` est vérifié mais la plage valide est 1–65535. Un port à 99999 passerait la validation.

  ### 6. AccessibleName et AccessibleDescription détournés pour stocker l'état UI
Dans `UtilsButtons.vb` :
	• `btn.AccessibleName = imageFolder.ToString()` → stocke le dossier d'image
	• `btn.AccessibleDescription = "selected"/"normal"/"disabled"` → stocke l'état du bouton
Ces propriétés sont faites pour les lecteurs d'écran (NVDA, JAWS…). Leur détournement casse l'accessibilité. La solution propre est d'utiliser un `Dictionary(Of Button, ButtonState)` ou une classe wrapper `ButtonStateManager`.

---

## 📅 20/05/2026 & 21/05/2026

## 🔐 Sécurité & Authentification

### ✅ Séparation des responsabilités sécurité

Refactorisation majeure de la couche sécurité afin de séparer :

- l’authentification utilisateur
- l’administration des utilisateurs

#### Nouveau module

Création de :

- `Metier/Security/GestionAuthentification.vb`

#### Répartition des responsabilités

##### GestionAuthentification

Responsable de :

- authentification utilisateur
- vérification mot de passe
- gestion des échecs login
- verrouillage compte
- mise à jour dernier login
- gestion des sessions

##### GestionUtilisateurs

Responsable de :

- gestion administrative des utilisateurs
- chargement listes utilisateurs
- édition utilisateurs
- activation/désactivation
- reset password
- gestion élévation

### ✅ Correction critique authentification

Correction d’un bug empêchant toute connexion utilisateur.

#### Cause

Le mapper utilisateur utilisé lors de l’authentification ne rechargeait plus :

- `password_hash`
- `password_salt`

Suite à une modification liée à la préparation de `UC_Utilisateurs`.

#### Impact

Erreur :

```text
Value cannot be null. (Parameter 's')
```
dans :
`Convert.FromBase64String(salt)`

### Correctif

Création d’un mapper dédié :

- `MapUtilisateurPourAuthentification`

Séparation stricte :

- mapper authentification
- mapper administration

### ✅ Mise à jour dernier login

Ajout :

- `UpdateDernierLoginUtilisateur`

et mise à jour automatique du champ :

- `dernier_login`

après authentification réussie.

## 👥 Gestion utilisateurs

## ✅ Préparation complète du module utilisateurs

Création de l’architecture du futur module :

- `UC_Utilisateurs`
- `UtilisateurEdition`

## ✅ Design UC_Utilisateurs

Mise en place du design complet :

#### Contrôles principaux

- `dgvUtilisateurs`
- `txtRechercheUtilisateur`
- `chkAfficherInactifs`
- `btnNouveau`
- `btnModifier`
- `btnActiverDesactiver`
- `btnResetPassword`
- `btnActualiser`

#### Décisions UI

- grille lecture seule
- sélection FullRowSelect
- colonnes techniques cachées
- actions désactivées sans sélection
- aucune donnée sensible affichée

### ✅ Structure DataGridView standardisée

Colonnes visibles :

- État
- Login
- Nom affiché
- Rôle
- Dernier login

Colonnes cachées :

- id utilisateur
- rôle max élévation
- sécurité
- audit

### ✅ Binding DataGridView

Ajout des :

- `DataPropertyName`
- binding objets métier
- chargement via `List(Of UtilisateurApplication)`

### ✅ GestionUtilisateurs

Ajout :

- `GetUtilisateursPourListe`
- `MapUtilisateurPourAdministration`

#### Règle importante

Aucune exposition :

- `password_hash`
- `password_salt`

hors authentification.

### ✅ QueryUtilisateurs

Réorganisation complète des requêtes SQL :

#### Authentification

- `SelectUtilisateurPourAuthentification`

#### Administration

- `SelectUtilisateursPourListe`
- `SelectUtilisateurPourEdition`
- `InsertUtilisateur`
- `UpdateUtilisateur`
- `UpdateActifUtilisateur`
- `ResetPasswordUtilisateur`
- `ForcerChangementPasswordUtilisateur`
- `DeverrouillerUtilisateur`

#### ✅ Navigation Admin

Ajout navigation :

- `UC_AdminHome`
   → `UC_Utilisateurs`

via :

```
NavigateToAdminView(
    New UC_Utilisateurs(),
    "Utilisateurs"
)
```
## 🧱 Architecture

### ✅ Cloisonnement renforcé

Validation des principes :

```
UI → Métier → Query → DatabaseManager
```

#### Règles renforcées

- aucun SQL dans UI
- aucun accès DB direct depuis UserControls
- séparation authentification / administration
- un mapper par contexte métier
- réutilisation stricte des helpers UI

## 🎨 UI & Standards

### ✅ Standardisation UtilisateurEdition

Définition des contrôles :

#### Identité

- login
- nom affiché

#### Droits

- rôle
- élévation max
- actif

#### Sécurité

- verrouillage
- échecs login
- dernier login

#### Audit

- création
- modification

------

### ✅ Uniformisation boutons password

Décision :

- même action métier
- même bouton
- même icône
- même tag asset

pour :

```
Reset PW
```
---

## 📅 20/05/2026

### 👥 Gestion Utilisateurs

#### ✅ Stabilisation architecture UC_Utilisateurs / UtilisateurEdition

Validation définitive de l’architecture :

`UC_Utilisateurs` = liste / navigation / actions rapides

`UtilisateurEdition` = fiche complète utilisateur

### Décisions importantes

Le UserControl reste volontairement léger :

- liste utilisateurs
- filtres
- navigation
- actions rapides

Toute la logique détaillée et sécurité utilisateur est déplacée dans :

```
UtilisateurEdition
```

### 🔐 Sécurité utilisateurs

#### ✅ Décision architecture : login immuable

Validation officielle :

```
login_utilisateur
= identifiant fonctionnel permanent
```

##### Conséquences

- login modifiable uniquement à la création
- login readonly en modification
- suppression des risques de rupture audit / logs / traçabilité
- cohérence sécurité long terme

### 🧱 UtilisateurEdition

#### ✅ Création structure complète de la Form

Ajout :

- `ModeUtilisateurEdition`
- constructeur typé
- implémentation `IContextAwareForm`
- gestion contexte Home
- variables privées métier
- organisation par régions

### ✅ Navigation modale

Depuis `UC_Utilisateurs` :

### Nouveau

```
New UtilisateurEdition(
    ModeUtilisateurEdition.Creation
)
```

### Modification

```
New UtilisateurEdition(
    ModeUtilisateurEdition.Modification,
    utilisateur.IdUtilisateur
)
```

## ✅ Gestion contexte Home

Ajout :

- `SetHeader`
- `SetStatus`

pendant l’ouverture de la Form.

#### Restauration automatique

Après fermeture modale :

```
Administration > Utilisateurs
```

restauré automatiquement.

### 🎨 UI Modes Création / Modification

#### ✅ Création

Mise en place :

- login modifiable
- champs audit readonly
- champs sécurité readonly
- verrouillage désactivé
- must_change_password coché par défaut

#### ✅ Modification

Mise en place :

- login readonly
- audit visible
- sécurité visible
- déverrouillage conditionnel

#### ✅ Stabilisation UX

Décision UI importante :

Les champs audit/sécurité ne sont plus masqués en création.

### Motivation

Éviter :

- formulaires “troués”
- décalages UI
- comportement visuel instable

Les champs restent visibles mais désactivés / vides.

### 📋 Chargement utilisateur

#### ✅ Préparation chargement réel

Ajout dans `GestionUtilisateurs` :

- `GetUtilisateurPourEdition`

Ajout dans `UtilisateurEdition` :

- `_utilisateur`
- `ChargerUtilisateur`
- `AlimenterControlesDepuisUtilisateur`

#### ✅ Initialisation combos rôles

Ajout :

- `InitialiserCombos`

pour :

- rôle utilisateur
- rôle max élévation

basés sur :

```
AppRole
```

### 🧪 Validation

#### Fonctionnel

✅ Navigation Home/Admin

 ✅ Navigation Utilisateurs

 ✅ Ouverture modale

 ✅ Retour contexte

 ✅ Chargement grille utilisateurs

 ✅ Double-clic modification

 ✅ États UI selon mode

 ✅ Gestion Header/Status

 ✅ Aucun crash

 ✅ Aucun warning critique

 ✅ Aucun log erreur

### 🚧 Prochaine étape

#### À implémenter

- chargement réel utilisateur dans Form
- sauvegarde création utilisateur
- sauvegarde modification utilisateur
- validations métier
- reset password réel
- déverrouillage réel
- activation/désactivation utilisateur

------

## 📅 17/05/2026

### 🔍 Audit complet du projet

#### Contexte
- Reprise du projet après 2 semaines d'absence
- Audit complet de la solution (60 fichiers analysés)
- Vérification : bugs, améliorations, documentation

#### ✅ Résultats de l'audit

**Bugs critiques** : ✅ Aucun

**Build** : ✅ Génération réussie

**Architecture** : ✅ Solide et cohérente
- Séparation Core / Metier / UI / Utils respectée
- Point d'accès DB unique (DatabaseManager)
- SQL centralisé (Query*)
- Navigation centralisée (NavigationManager)

**Sécurité** : ✅ Robuste
- PBKDF2 SHA256 (100k itérations)
- DPAPI pour secrets locaux
- Logs sans mots de passe
- Élévation temporaire bien tracée

**Documentation** : ✅ Riche et complète
- ETAT_DU_PROJET.md : état des lieux détaillé
- Rules.md : règles claires
- Standards-Commentaires.md : modèles exhaustifs
- Reference_UI_Controles.md : documentation UI détaillée

#### ⚠️ Points d'attention identifiés

1. **UC_Utilisateurs incomplet** (Dette technique DT-02)
   - ✅ Liste implémentée
   - ❌ Détail/édition utilisateur manquant
   - ❌ Actions admin (création, reset MDP, verrouillage, activation) manquantes

2. **TODO dans GestionUtilisateurs.vb** (ligne ~37-44)
   - Commentaire sur chargement auto utilisateur "joelle" en DEBUG
   - À traiter : implémenter ou supprimer

3. **Documentation légèrement désynchronisée**
   - Quelques dates de version à actualiser
   - Statut UC_Utilisateurs à préciser

#### 🚀 Améliorations proposées (13 identifiées)

**Factorisations** :
- Extraire méthode publique `GetUtilisateurByLogin` dans GestionUtilisateurs
- Validation centralisée des paramètres dans GestionParametres

**Optimisations** :
- Cache des paramètres applicatifs
- Query précompilées (MySqlCommand.Prepare)
- Lazy loading des UserControls

**Simplicité/Lisibilité** :
- Simplifier initialisation boutons (`InitializeButtons(ParamArray)`)
- Réduire indentations dans `AuthentifierUtilisateur`

**Souhaitable** :
- Rotation des logs (taille/heure)
- Fallback Event Log pour erreurs d'écriture log

#### 📋 Plan d'action priorisé

**Priorité 1 - CRITIQUE** : ✅ Aucune action critique (code stable)

**Priorité 2 - IMPORTANT** (cette semaine) :
1. Finaliser UC_Utilisateurs (liste + détail + actions admin)
2. Nettoyer TODO dans GestionUtilisateurs
3. Synchroniser documentation (ETAT_DU_PROJET, CHANGELOG)

**Priorité 3 - SOUHAITABLE** (ce mois-ci) :
4. Extraire `GetUtilisateurByLogin` publique
5. Validation centralisée paramètres
6. Simplifier initialisation boutons

**Priorité 4 - OPTIONNEL** (à planifier) :
7. Cache paramètres
8. Rotation logs
9. Refactoring AuthentifierUtilisateur
10. Lazy loading UserControls

#### 📄 Documentation créée
- **RAPPORT_AUDIT_2026-05-17.md** : Rapport d'audit complet disponible dans `Docs/`

#### 🎯 Conclusion
Projet en excellent état pour reprise :
- ✅ Aucun bug critique
- ✅ Architecture solide et documentée
- ✅ Sécurité robuste
- ✅ Code propre et maintenable
- ✅ Build qui passe

**Prêt pour la suite du développement** 🌿

------




> ---
>
> - **Contact** : ***Joëlle (Manou)  - Les Artefacts de Manou***
>
>   Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.
>
>   - Site web P.Nguyen Duy:  https://pearlnguyenduy.be/
>   - mailto: `joelle@nguyen.eu`
>
>   - GitHub privé: Althea    https://github.com/AngeljoNG/Althea
>   - GitHub public : Althea None
>
> ---

[TOC]

