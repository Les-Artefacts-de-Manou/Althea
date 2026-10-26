# Althéa — Architecture de l’interface utilisateur

> Référence synthétique et actuelle de l’architecture WinForms. Les fiches écran détaillées et illustrations restent dans `Docs/Architecture/Documentation_technique_UI_Althea.md`.
> Dernière mise à jour : 24/10/2026 — recherche du réseau par nom, R5

<details>
<summary>Historique des mises à jour</summary>

- 24/10/2026 — recherche des patients par lieu, R4
- 23/10/2026 — recherche des paiements d'une période, R3
- 23/10/2026 — recherche des séances d'une période, R2
- 23/10/2026 — bouton « Recherche » et hub de recherche, ADR-52
- 19/10/2026

</details>

## Principes

- Une Form principale unique : `Home`.
- Les écrans métier sont des `UserControl` injectés dans le contenu central.
- Les interactions ponctuelles, éditions et confirmations sont des Forms modales.
- L’UI ne contient ni accès base direct ni règle métier ; elle délègue aux services `Metier`.
- La navigation passe par `NavigationManager`.

## Contexte partagé

`UserControlContext` fournit aux écrans le statut, les erreurs et les aides contextuelles de `Home`. Les écrans et modales compatibles implémentent respectivement `IContextAwareUserControl` ou `IContextAwareForm`.

```text
Home
 ├─ NavigationManager → UserControl actif
 ├─ UserControlContext → statut / ErrorProvider / ToolTip
 └─ Forms modales contextualisées
```

## Écrans principaux

| Zone | Éléments clés |
|---|---|
| Accueil et administration | `UC_Accueil`, `UC_AdminHome`, `UC_Parametres`, `UC_Utilisateurs`, `UC_Cabinet`, `UC_SituationsJuridiques`, `UC_TypesIdentifiantPro`, `UC_Sauvegardes` |
| Patients | `UC_PatientHome`, `UC_PatientFiche`, `ContactEdition`, `IntervenantEdition` |
| Recherche (ADR-52) | `UC_RechercheHome` (bouton « Recherche » du menu, sous Patients : recherche rapide par code, tuiles des recherches) ; `UC_RechercheSeances` (tuile « Séances » : séances d'un jour ou d'une période, tous patients ; filtre et résumé de l'onglet Séances du dossier, `FiltresSeances`) ; `UC_RecherchePaiements` (tuile « Paiements » : relevé des paiements d'une période, tous patients, filtre par mode, totaux encaissés par mode ; `FiltresPaiements`) ; `UC_RecherchePatientsLieu` (tuile « Patients par lieu » : zone code postal ou localité, liste des lieux et leurs patients, `LieuPatients`) ; `UC_RechercheReseau` (tuile « Réseau par nom » : thérapeute, intervenant ou contact par son nom, ses patients, sa fiche ; `PersonneReseau`) ; période des recherches : `SelecteurPeriode` ; `OuvertureResultat` ouvre l'élément trouvé (fiche patient, dossier au bon onglet), Retour ramène à la recherche, critères compris |
| Dossiers | `UC_DossierFiche`, `DossierZoneEditor`, `DossierStatutEditor` |
| Séances et paiements | `SeanceEditor`, `PlanningJourViewer`, `PaiementEditor`, utilisation de l'avoir (`DialogUtilisationAvoir`) |
Synthèse, assistant), `UC_SyntheseDomaine` (aussi dans la Fiche Dossier), `CategoriesQuotaEditor`,
(dont `UC_Organismes` ; les Domaines s'ouvrent par le menu principal, `UC_DomainesHome`)
| Composants communs | `DialogChoix`, `UC_RichTextEditor` (profils Complet / Compact), `UC_Pagination` |

## Conventions UX structurantes

- `DialogChoix` remplace les `MessageBox` de l’application.
- Les couleurs programmatiques viennent de `UITheme`.
- Les icônes d’état viennent de `UtilsIcons`.
- Les DataGridView utilisent `UtilsDataGrid` ; les en-têtes multi-lignes sont utilisés lorsque cela améliore réellement la densité sans réduire la lisibilité.
- Une grille métier doit montrer le contexte nécessaire à la décision : par exemple la séance associée pour un paiement.
- **Composition grille + détail** : la grille est calée sur la largeur de ses colonnes (`Dock = Left`) et le panneau « Détail » lui est accolé à environ 2 cm, même hauteur. La logique est centralisée dans `UC_ReferentielBase` et héritée par tous les référentiels.
- **Habillage** : aucune image de fond ; les contrôles du kit (ci-dessous) se dessinent eux-mêmes. L'essai d'habillage par images (`Assets/ui-backgrounds/`, 09/10/2026) est abandonné et ses fichiers ont été supprimés (UI-8, 17/10/2026).
- **Kit d'interface Althéa** (`UI/Controls/Theme/`, refonte UI, ADR-30 à ADR-34) :
  - `AltheaEnTete` (titre, sous-titre et icône de chaque écran, icône choisie dans le Designer depuis `Assets/Titre_Ico`),
  - `AltheaCarte` (regroupement en cartes),
  - `AltheaBouton` (variantes Sauge / Ocre / Terre / Clair),
  - `AltheaOnglets`,
  - `AltheaEtapes` (parcours en étapes, écran Domaines),
  - `AltheaDate` / `AltheaCalendrier` (dates),
  - `AltheaFondFenetre` (fond des fenêtres : beige clair et petite feuille dessinée),
  - `AltheaBarreTitre` (barre de titre Windows aux couleurs du kit),
  - `AltheaIconeEtat` (icônes d'état),
  - `AltheaJauge` (avancement),
  - bandeaux de recherche et barres d'actions.
  - Construction des écrans et propriétés des contrôles : `Reference_UI.md` ; état du chantier : `Docs/Todo/Plan_Refonte_UI_Althea.md`.
- **Opérations longues** (ex. sauvegarde) : traitement en arrière-plan (`Task.Run` + `IProgress`), jauge et étape affichées ; l'écran implémente `IUnsavedChangesAware` pour bloquer navigation et fermeture tant que l'opération tourne.

## Documentation associée

- Fiches détaillées d’écrans : [Documentation_technique_UI_Althea.md](Documentation_technique_UI_Althea.md).
- Workflows de navigation et d’usage : [Process_Althea.md](Process_Althea.md).
- Conventions visuelles et construction des écrans : [Reference_UI.md](../Rules/Reference_UI.md).







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

