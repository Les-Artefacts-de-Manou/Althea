# 📘 **Documentation technique – UI Althéa**

> *Dernière mise à jour : 25-10-2026 — document scindé par domaine : ce fichier est le sommaire (détail dans l'historique ci-dessous)*

<details>
<summary>Historique des mises à jour</summary>

- 25-10-2026
  - document scindé par domaine (décision de Joëlle) : ce fichier garde le sommaire, la trame, la vue d'ensemble et l'historique ; les fiches des composants sont dans `Docs/Architecture/UI/` (5 parties) ; fiche `IFenetrePrincipale` ajoutée au socle
  - audit C.5 (ADR-56) : `UC_DossierFiche` et `UC_PatientFiche` répartis en fichiers partiels, un par onglet lourd
  - `ElevationAcces` (V1.3.1) n'écrit plus le fil d'Ariane (`SetHeader` retiré) : c'est l'appelant qui le pose
  - audit C.4, étape 3 (ADR-55) : recherches (`OuvertureResultat`, `UC_Recherche*`) et hubs (`UC_AdminHome`, `UC_ReferentielHome` : `Ouvrir`, élévation par `DemanderElevation`) ; élévation réunie dans `Home`
  - audit C.4, étape 2 (ADR-55) : `UC_PatientHome`, `UC_PatientFiche`, `UC_DossierFiche`, `UC_DomainesHome`, `UC_Cabinet`, `UC_Utilisateurs`, `UC_Therapeutes` lisent la fenêtre principale dans `_context.FenetrePrincipale`
  - audit C.4, étape 1 (ADR-55) : `Home` implémente `IFenetrePrincipale`, fourni par `UserControlContext.FenetrePrincipale` ; `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`, `SeanceEditor`, `PaiementEditor` reçoivent `IFenetrePrincipale` au lieu de `Home`
  - lignes système déverrouillées (v2.58.0) : `MERE`, `AUTORITE_PARENTALE`, statuts `ABSENTE`, `ANNULEE_PATIENT`, `ANNULEE_PRATICIEN`, `MALADE` — sections « Lignes système » de `UC_LiensPatient`, `UC_RoleLegal`, `UC_StatutsSeance`
  - captures refaites après les changements du 24/10 (15 écrans et fenêtres) ; nouvelles : `UC_RecherchePatientsLieu`, `UC_RechercheReseau` ; téléphone et e-mail floutés sur `UC_Cabinet_Praticien`
  - relevés aux captures : ligne inactive sélectionnée lisible (`UC_PatientHome`, `UC_RecherchePatientsLieu`, `UC_RechercheReseau`) ; case « Proposé pour un nouveau contact » (libellé raccourci) ; `ContactEdition` et `IntervenantEdition` en consultation : focus sur « Fermer » ; carte « Personnes » de `UC_RechercheReseau` élargie (580), colonne « Patients » entière
- 24-10-2026
  - audit C.3 étape 2 (v2.57.0, ADR-54) : cases « Non tenue, libère le créneau » et « Proposé à la création d'une séance » de `UC_StatutsSeance` (V1.4.0) ; case « Proposé par défaut pour un nouveau contact » de `UC_LiensPatient` et `UC_RoleLegal` ; `ContactEdition` (V1.8.0) présélectionne ces valeurs
  - pastille « Inactif » devant le patient inactif dans `UC_RecherchePatientsLieu` (V1.1.0) et dans les liens de `UC_RechercheReseau` (V1.2.0)
  - écran Patients (`UC_PatientHome` V1.7.0) : pastille « Inactif » devant le nom d'un patient inactif ; liste « Patients : Actifs / Inactifs / Tous » à la place de la case « Afficher les inactifs »
  - codes postaux belges (v2.56.0, ADR-53) : code existant, localité proposée et suggérée, code trouvé depuis la localité, avertissement puis confirmation (`SaisieLocaliteBelge`) dans la fiche patient (V1.12.0), `ContactEdition` (V1.7.0), `TherapeuteEdition` (V1.8.0) et `UC_Cabinet` (V1.11.0)
  - code toujours visible (Rules § 15) : colonne Code de `UC_Utilisateurs` (V1.9.0), de `PlanningJourViewer` (V1.4.0) et de la liste des personnes de `UC_RechercheReseau` (V1.1.0) ; `UtilisateurEdition` (V1.10.0) nommée par le code ; compte lié du praticien rappelé par son code (`UC_Cabinet` V1.10.0) ; `AltheaListe` (V1.3.0) : liste ouverte à la largeur de son plus long élément
  - recherche R5 : `UC_RechercheReseau` (réseau par nom), tuile du hub
  - codes visibles : colonne Code des grilles des contacts et des intervenants (`UC_PatientFiche` V1.11.0) et des thérapeutes (`UC_Therapeutes` V1.3.0) ; code au titre de `ContactEdition` (V1.6.0), `IntervenantEdition` et `TherapeuteEdition` (V1.7.0)
  - recherche R4 : `UC_RecherchePatientsLieu` (patients par lieu), tuile du hub
  - illustrations refaites avec l'outil de capture (Debug) `FiltreCaptureEcran` : tous les écrans, onglets et fenêtres, rangées dans `Docs/Illustrations/Doc_UI` (anciennes références `Docs/Illustrations/*.png` corrigées)
  - captures de Joëlle : `Login`, `ElevationAcces`, `DialogUtilisationAvoir`, `DialogComptePraticien`, `UC_ApercuPatient`, vue SuperUser de `UC_AdminHome`, `UC_Accueil`
- 23-10-2026
  - recherche transversale : section 1.3 (`UC_RechercheHome`, `UC_RechercheSeances`, `UC_RecherchePaiements`), `OuvertureResultat`, `SelecteurPeriode`, `FiltresSeances`, `FiltresPaiements`
  - `Home` (bouton Recherche, `NavigateToRecherche`, `NavigateDepuisRecherche`, fin de l'élévation après inactivité, `SuiviActivite`)
  - ouverture ciblée de `UC_DossierFiche` et `UC_PatientFiche`
  - colonne Code des paiements
  - règles de saisie jugées par le métier (ADR-51 : fiche patient, contacts, intervenants, thérapeutes, domaines, comptes, `AffichageErreursSaisie`)
  - règles des séances et paiements dans le métier, « À confirmer », utilisation de l'avoir, ligne choisie avant d'agir
  - spécialités en référentiel (`UC_Specialites`, `TherapeuteEdition`)
  - `UC_Lateralites`, `DialogComptePraticien`, fenêtres de test, `ResumeFiltre`, `IUnsavedChangesAware` documentés
  - emplacements d'illustrations à compléter
- 20-10-2026
  - latéralités en référentiel (`UC_Lateralites`, tuile du groupe Patient), modalités de paiement proposées en paramètre
  - boutons inactifs qui disent pourquoi (`AltheaBouton` V1.3.0, barres Séances et Paiements)
  - logo des domaines (`UC_DomainesHome` V1.10.0, `UC_SyntheseDomaine` V2.4.0, `UC_DossierFiche` V1.11.0)
  - correcteur dans les notes courtes (`AltheaZoneTexte` : commentaire d'un paiement, motif d'annulation ; `CorrecteurTexte`, `UC_RichTextEditor` V2.4.1)
  - correcteur d'orthographe de Windows dans l'éditeur (`UC_RichTextEditor` V2.4.0, `CorrecteurWindows`)
- 19-10-2026
  - textes types : plusieurs usages, écran des usages (`UC_UsagesTextesTypes`), `UC_ReferentielBase.CreationPermise`
  - textes types (`UC_TextesTypes`, tuile des Référentiels, bouton de l'éditeur V2.3.0)
  - éditeur de texte, temps 2 : « Ouvrir en grand » (`UC_RichTextEditor` V2.1.0, `EditeurTexteAgrandi`), en-tête et pied de page des documents (V2.2.0, `PatientDocument`, `EnTeteDocument`)
  - modifications non enregistrées, même comportement partout : fiches patient et dossier, Domaines, référentiels, Cabinet, Paramètres (`IUnsavedChangesAware`), fenêtres Séance, Paiement, Contact, Intervenant, Thérapeute, Utilisateur et Zone du dossier (`GardeModifications`, `DialogChoix.DemanderAvantDepart`)
  - Annuler sur place : Abandonner / Continuer la saisie (`ConfirmerAbandon`)
  - éditeur de texte refondu (`UC_RichTextEditor` V2.0.0, profils Complet et Compact, `UC_RichTextEditorSimple` supprimé, barre du kit `AltheaRenduBarre`)
  - `UC_PatientFiche`, `SeanceEditor`, fenêtres Contact, Intervenant, Thérapeute et Zone du dossier sur le nouvel éditeur
  - `UC_Pagination` V1.1.0 dans le thème, Séances et Paiements du dossier paginés
  - aperçu de la liste des patients stabilisé
- 18-10-2026
  - UI-9.4 : référentiels, Thérapeutes, Domaines, liaisons et catégories de quota en pastilles (« Système », « Inactif », états des rattachements et des règles), cadenas et icônes `Tech_Ico` retirés
  - UI-9.3 : grilles d'administration (Utilisateurs, État du système, Journaux, historiques des sauvegardes et des restaurations) en pastilles et repères
  - UI-9.2 : planning du jour (pastilles de statut, créneaux libres), paiements d'une séance (pastille État), fiche patient (statut des dossiers en pastille, terminés atténués)
  - clé Syncfusion saisie dans Paramètres (carte « Licences ») et signalée dans l'État du système
  - UI-8 : ancien écran `UC_Domaines` (archivé) supprimé
  - ancien fond étiré des UserControls supprimé, fenêtre « en passant » des référentiels au fond du kit
- 17-10-2026
  - nettoyage UI-8 des assets inutilisés (icônes des tuiles Cabinet, Utilisateurs, Thérapeutes, Natures séance et Prix séance corrigées)
  - clôture d'un dossier contrôlée (séances à venir, montants à régler), état du dossier visible (pastille colorée, bandeau « consultation »)
  - grilles Séances et Paiements lisibles (pastilles d'état, résumés cliquables, Paiements en relevé chronologique)
- 16-10-2026 — UI-7 : panneau « Détail » des référentiels à la règle des fenêtres

</details>

---

## **Organisation du document**

Chaque composant du dossier `UI/` de la solution est décrit selon une trame unique (modèle : `UC_Utilisateurs`) :

1. Rôle général
2. Responsabilités
3. Dépendances
4. Variables privées
5. Contrôles principaux
6. DataGridView : colonnes (si grille)
7. Autres composants à utilité dédiée
8. Méthodes principales
9. Gestion des icônes (si présente)
10. Gestion des filtres et recherche (si présente)
11. Gestion des droits (si spécificités)
12. Règles en vigueur
13. Points d'attention
14. Message au repreneur.

Lorsqu'un composant comporte un `TabControl`, une section **Onglets** reprend la trame pour chaque onglet, limitée à ce qui lui est propre.

Les fiches sont réparties **par domaine**, en cinq parties (25/10/2026, décision de Joëlle) ; dans chacune : écrans, puis fenêtres, dialogues et infrastructure du domaine :

| Partie | Contenu |
| --- | --- |
| [Administration et sécurité](UI/UI_Administration.md) | Hub Administration et ses écrans (paramètres, cabinet, utilisateurs, sauvegardes, journaux, état du système), connexion, mot de passe, élévation d'accès et configuration de la base. |
| [Patients, dossiers, séances et paiements](UI/UI_Patients_Dossiers.md) | Écran Patients et aperçu, fiche patient, fiche dossier, et leurs fenêtres : contact, intervenant, zones et statut du dossier, séance, paiement, planning du jour, dialogues d'annulation, de report et d'avoir. |
| [Référentiels et domaines](UI/UI_Referentiels_Domaines.md) | Hub et socle des référentiels, référentiels standards, spécifiques et métier, domaines (conventions) et leurs rattachements, fenêtres Thérapeute, catégories de quota et hôte des référentiels en fenêtre. |
| [Recherche transversale](UI/UI_Recherche.md) | Hub Recherche, recherche rapide par code, recherches spécialisées (séances, paiements, lieux, réseau), sélecteur de période et ouverture d'un résultat. |
| [Socle commun](UI/UI_Socle.md) | Fenêtre principale (Home), accueil, éditeur de texte riche, pagination, dialogue standard (DialogChoix), fenêtres de test, et infrastructure de l'interface (contexte, navigation, contrats, filtres partagés). |

Un composant est rangé avec le domaine qui l'utilise ; ce qui sert partout (fenêtre principale, kit, dialogue standard, contexte, navigation) est dans le **socle commun**.

## **Vue d'ensemble – Écrans, classes et niveau de complétude**

| Partie | Classe | Type | Dossier | Statut |
| --- | --- | --- | --- | --- |
| [Socle](UI/UI_Socle.md) | `UC_Accueil` | UserControl | `UI/Controls` | ⬜ Prévu – classe vide, à implémenter |
| [Administration](UI/UI_Administration.md) | `UC_AdminHome` | UserControl | `UI/Controls/Administration` | ✅ Complet |
| [Administration](UI/UI_Administration.md) | `UC_Parametres` | UserControl | `UI/Controls/Administration` | ✅ Complet (V2 : page unique, 16/10/2026) |
| [Administration](UI/UI_Administration.md) | `UC_Cabinet` | UserControl | `UI/Controls/Administration` | ✅ Complet (V1 mono-praticien) |
| [Administration](UI/UI_Administration.md) | `UC_Utilisateurs` | UserControl | `UI/Controls/Administration` | ✅ Complet |
| [Administration](UI/UI_Administration.md) | `UC_Sauvegardes` | UserControl | `UI/Controls/Administration` | ✅ Sauvegarde (P2) et accès à la restauration (P3) |
| [Administration](UI/UI_Administration.md) | `UC_Journaux` | UserControl | `UI/Controls/Administration` | ✅ Consultation des journaux (13/10/2026) |
| [Administration](UI/UI_Administration.md) | `UC_EtatSysteme` | UserControl | `UI/Controls/Administration` | ✅ État du système (13/10/2026) |
| [Socle](UI/UI_Socle.md) | `UC_RichTextEditor` | UserControl | `UI/Controls/Communs` | ✅ Refondu le 19/10/2026 (profils Complet / Compact) |
| [Socle](UI/UI_Socle.md) | `UC_Pagination` | UserControl | `UI/Controls/Communs` | ✅ Complet (barre réutilisable, thème du kit 19/10/2026) |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `UC_PatientHome` | UserControl | `UI/Controls/Patients` | ✅ Complet (hub de la journée, 17/10/2026) |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `UC_ApercuPatient` | UserControl | `UI/Controls/Patients` | ✅ Panneau d'aperçu de l'écran Patients (17/10/2026) |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `UC_PatientFiche` | UserControl | `UI/Controls/Patients` | ✅ Complet (5 onglets) |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `UC_DossierFiche` | UserControl | `UI/Controls/Dossiers` | ✅ Complet (5 onglets) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_ReferentielHome` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_ReferentielBase` | UserControl | `UI/Controls/Referentiels` | ✅ Complet (héritable) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_LiensPatient` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_RolesIntervenant` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_RoleLegal` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_SituationsFamiliales` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_SituationsJuridiques` | UserControl | `UI/Controls/Referentiels` | ✅ Complet (champ porteur BCE) — **ouvert depuis `UC_AdminHome`** depuis le 10/10/2026 |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_TypesIdentifiantPro` | UserControl | `UI/Controls/Referentiels` | ✅ 20/10/2026 (porté par, masque, expression de contrôle) — **ouvert depuis `UC_AdminHome`** |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_StatutsDossier` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_StatutsSeance` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_TypesDocuments` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_TextesTypes` | UserControl | `UI/Controls/Referentiels` | ✅ Nouveau le 19/10/2026 (ADR-44) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_UsagesTextesTypes` | UserControl | `UI/Controls/Referentiels` | ✅ Nouveau le 19/10/2026 (ADR-44) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_TypesRendezVous` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_Lateralites` | UserControl | `UI/Controls/Referentiels` | ✅ 20/10/2026 (v2.46.0) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_Specialites` | UserControl | `UI/Controls/Referentiels` | ✅ Nouveau le 23/10/2026 (v2.55.0) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_Therapeutes` | UserControl | `UI/Controls/Referentiels` | ✅ Complet (entité riche) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_NaturesSeance` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_PrixSeance` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_ModesPaiement` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_Organismes` | UserControl | `UI/Controls/Referentiels` | ✅ Complet (lignes système, coordonnées et périodicité — lot 29 ; type remboursement / mutualité — v2.34.0) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_Periodicites` | UserControl | `UI/Controls/Referentiels` | ✅ Complet (lot 29, `nb_mois` nullable) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_Pays` | UserControl | `UI/Controls/Referentiels` | ✅ Complet (v2.34.0, code ISO, colonne « Contrôles ») |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_Civilites` | UserControl | `UI/Controls/Referentiels` | ✅ Complet (v2.34.0, abréviation) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_DomainesHome` | UserControl | `UI/Controls/Domaines` | ✅ Complet (5 onglets + assistant) — écran de référence |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_SyntheseDomaine` | UserControl | `UI/Controls/Domaines` | ✅ Complet — synthèse partagée (DomainesHome, DossierFiche) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_LiaisonsPrixDomaines` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `UC_LiaisonsNaturesDomaines` | UserControl | `UI/Controls/Referentiels` | ✅ Complet |
| [Recherche](UI/UI_Recherche.md) | `UC_RechercheHome` | UserControl | `UI/Controls/Recherche` | ✅ Nouveau le 23/10/2026 (ADR-52, R1) |
| [Recherche](UI/UI_Recherche.md) | `UC_RechercheSeances` | UserControl | `UI/Controls/Recherche` | ✅ Nouveau le 23/10/2026 (R2) |
| [Recherche](UI/UI_Recherche.md) | `UC_RecherchePaiements` | UserControl | `UI/Controls/Recherche` | ✅ Nouveau le 23/10/2026 (R3) |
| [Recherche](UI/UI_Recherche.md) | `UC_RecherchePatientsLieu` | UserControl | `UI/Controls/Recherche` | ✅ Nouveau le 24/10/2026 (R4) |
| [Recherche](UI/UI_Recherche.md) | `UC_RechercheReseau` | UserControl | `UI/Controls/Recherche` | ✅ Nouveau le 24/10/2026 (R5) |
| [Socle](UI/UI_Socle.md) | `Home` | Form | `UI/Forms` | ✅ Complet |
| [Administration](UI/UI_Administration.md) | `Login` | Form | `UI/Forms/Login` | ✅ Complet |
| [Administration](UI/UI_Administration.md) | `ChangePassword` | Form | `UI/Forms/Login` | ✅ Complet |
| [Administration](UI/UI_Administration.md) | `ElevationAcces` | Form | `UI/Forms/Login` | ✅ Complet |
| [Administration](UI/UI_Administration.md) | `ConfigurationConnexion` | Form | `UI/Forms/Database` | ✅ Complet |
| [Administration](UI/UI_Administration.md) | `UtilisateurEdition` | Form | `UI/Forms/Utilisateur` | ✅ Complet |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `ContactEdition` | Form | `UI/Forms/Patients` | ✅ Complet |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `IntervenantEdition` | Form | `UI/Forms/Patients` | ✅ Complet (réf. thérapeutes) |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `TherapeuteEdition` | Form | `UI/Forms/Patients` | ✅ Complet |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `DossierZoneEditor` | Form | `UI/Forms/Dossiers` | ✅ Complet |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `DossierStatutEditor` | Form | `UI/Forms/Dossiers` | ✅ Complet (transitions + archivage) |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `SeanceEditor` | Form | `UI/Forms/Dossiers` | ✅ Complet (nature, date/heure, tarif, statut, cadenas montant, chevauchements, notes/évolution RTF) |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `PaiementEditor` | Form | `UI/Forms/Dossiers` | ✅ Complet |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `PlanningJourViewer` | Form | `UI/Forms/Dossiers` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `CategoriesQuotaEditor` | Form | `UI/Forms/Domaines` | ✅ Complet |
| [Référentiels](UI/UI_Referentiels_Domaines.md) | `ReferentielModalHost` | Form | `UI/Forms/Communs` | ✅ Complet (ajout inline [+]) |
| [Socle](UI/UI_Socle.md) | `EditeurTexteAgrandi` | Form | `UI/Forms/Communs` | ✅ Nouveau le 19/10/2026 (« Ouvrir en grand ») |
| [Socle](UI/UI_Socle.md) | `TestRichTextEditor` | Form | `UI/Forms/Test` | ✅ Complet |
| [Socle](UI/UI_Socle.md) | `TestKitAlthea`, `TestDialogChoix`, `TestChampDate` | Form | `UI/Forms/Test` | ✅ Fenêtres de test (Debug) |
| [Socle](UI/UI_Socle.md) | `DialogChoix` | Dialogue | `UI/Forms/Communs` | ✅ Complet |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `DialogMotifAnnulation` | Dialogue | `UI/Forms/Dossiers` | ✅ Complet |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `DialogReportSeance` | Dialogue | `UI/Forms/Dossiers` | ✅ Complet |
| [Administration](UI/UI_Administration.md) | `DialogRestauration` | Dialogue | `UI/Forms/Sauvegardes` | ✅ Lot 30 (P3) |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `DialogPaiementsSeance` | Dialogue | `UI/Forms/Dossiers` | ✅ Complet |
| [Patients et dossiers](UI/UI_Patients_Dossiers.md) | `DialogUtilisationAvoir` | Dialogue | `UI/Forms/Dossiers` | ✅ Complet |
| [Administration](UI/UI_Administration.md) | `DialogComptePraticien` | Dialogue | `UI/Forms/Cabinet` | ✅ Complet (création d'un seul tenant ; règle du login partagée, 23/10/2026) |
| [Socle](UI/UI_Socle.md) | `UserControlContext` | Classe | `UI/Context` | ✅ Complet |
| [Socle](UI/UI_Socle.md) | `IContextAwareUserControl` | Interface | `UI/Navigation` | ✅ Complet |
| [Socle](UI/UI_Socle.md) | `IContextAwareForm` | Interface | `UI/Navigation` | ✅ Complet |
| [Socle](UI/UI_Socle.md) | `NavigationEntry` | Classe | `UI/Navigation` | ✅ Complet |
| [Socle](UI/UI_Socle.md) | `NavigationManager` | Classe | `UI/Navigation` | ✅ Complet |
| [Socle](UI/UI_Socle.md) | `IUnsavedChangesAware` | Interface | `UI/Navigation` | ✅ Complet |
| [Socle](UI/UI_Socle.md) | `IFenetrePrincipale` | Interface | `UI/Navigation` | ✅ Nouveau le 25/10/2026 (audit C.4, ADR-55) : implémentée par `Home`, fournie par `UserControlContext.FenetrePrincipale` |
| [Socle](UI/UI_Socle.md) | `SuiviActivite` | Classe | `UI/Context` | ✅ 20/10/2026 (fin de l'élévation après inactivité) |
| [Socle](UI/UI_Socle.md) | `ResumeFiltre` | Classe | `UI/Controls/Communs` | ✅ Complet |
| [Socle](UI/UI_Socle.md) | `FiltresSeances`, `FiltresPaiements` | Modules | `UI/Controls/Communs` | ✅ 23/10/2026 (partagés dossier / recherche) |
| [Recherche](UI/UI_Recherche.md) | `SelecteurPeriode`, `OuvertureResultat` | Classe / Module | `UI/Controls/Recherche` | ✅ 23/10/2026 |

> [!NOTE]
> Ce tableau et les fiches des parties doivent être mis à jour à chaque ajout ou modification d'un composant UI ; un nouveau composant prend place dans la partie de son domaine.

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

