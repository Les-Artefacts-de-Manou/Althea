# Althéa — Utilitaires transverses

> Référence des modules réutilisables. Les règles d’usage sont normatives dans `Docs/Rules/Rules.md`.
> Dernière mise à jour : 24/10/2026 (`SaisieLocaliteBelge`, codes postaux belges ; `UtilsTexte`, comparaisons de `UtilsCodePostal`).

## `Utils/UI/`

| Module | Responsabilité |
|---|---|
| `UITheme` | Palette, constantes visuelles et couleurs d’état. |
| `UtilsButtons` | Initialisation et comportement des boutons standards. |
| `UtilsDataGrid` | Initialisation, tri et densification des DataGridView ; pastilles dessinées dans une cellule (`DessinerPastilleCellule`, `DessinerPastilleAvantTexte` : pastille puis texte, ex. « Inactif » devant le nom d'un patient, 24/10/2026). |
| `UtilsIcons` | Accès centralisé aux icônes d’état. |
| `UtilsControls` | Manipulations UI communes ; `DefinirLectureSeule(zone, lectureSeule)` (14/10/2026) : consultation lisible d'une zone entière (zones de texte en `ReadOnly` sur fond `ColorChampLectureSeule`, nombres sans flèches, listes, cases, dates et boutons indisponibles, libellés et UserControls intacts ; `AltheaDate` par sa propriété `LectureSeule`) ; `DefinirChampLectureSeule(champ, lectureSeule)` (15/10/2026) : la même règle pour un champ isolé (ex. montant de séance selon le cadenas). |
| `UtilsValidation` | Validations réutilisables de saisie. |
| `GardeModifications` | Modifications non enregistrées (19/10/2026, ADR-43) : photo des champs saisissables d'un écran (listes à cocher comprises, V1.1.0), `EstModifie` ; `SurveillerFenetre` branche une fenêtre (photo à l'affichage, Enregistrer / Quitter sans enregistrer / Rester à la fermeture). La photo reste en mémoire, jamais journalisée. ; `AjouterValeur` : état tenu par l'écran (logo choisi non enregistré, V1.2.0). |
| `LogosDomaines` | Miniatures des logos de domaine (20/10/2026) : `Miniature(id, fichier, largeur, hauteur)` (cadre en longueur) gardée en mémoire (partagée, ne pas libérer), `Oublier(id)` après un changement, `Dessiner` (liste dessinée par l'écran). |
| `CorrecteurTexte` | Correcteur d'orthographe de Windows dans une zone de texte (20/10/2026, ADR-45), commun à `UC_RichTextEditor` et `AltheaZoneTexte` : `ReglagePermis` (réglage `EDITEUR_CORRECTEUR_ORTHOGRAPHE`), `Appliquer` (soulignement et langue du clavier, sans « Modifié »), `LangueClavier` / `EtiquetteLangue` ; une instance par contrôle ajoute au clic droit les suggestions et « Ajouter au dictionnaire » (`Preparer`). |
| `UtilsTelephone` | Normalisation, formatage et validation des numéros. Règles par pays (Belgique, France, Luxembourg, Allemagne, Pays-Bas) indexées par le **code ISO** de `ref_pays` ; un libellé reconnu reste accepté. Pays sans règle : validation souple. |
| `UtilsCodePostal` | **Format** des codes postaux (Belgique, France, Luxembourg) selon le code ISO du pays ; autres pays : saisie libre. L'existence d'un code belge et sa localité : `GestionCodesPostauxBe` (v2.56.0, ADR-53). Formes de comparaison d'un lieu (24/10/2026) : `CleCodePostal` (sans espaces, majuscules), `CleLocalite` (règle de `UtilsTexte.CleComparaison`, plus points, « œ » et « Saint / Sint / St » confondus — même règle que le script du référentiel). |
| `SaisieLocaliteBelge` | Aide à la saisie du couple code postal / localité d'une adresse (24/10/2026, ADR-53), une instance par adresse d'écran : pour une adresse belge, dans les deux sens : localité proposée quand on quitte le code (si elle est vide) et noms du code suggérés pendant la frappe ; code vide, suggestions « localité · code », aussi sans accents (un choix remplit les deux champs, séparé en quittant le champ, localité remise à l'écriture officielle), code rempli si la localité n'en a qu'un, sinon codes signalés et suggérés dans le champ Code ; avertissement (icône) d'une localité hors du code ; `MessageCodePostal` (erreur du code, existence comprise), `Actualiser` (après chargement ou changement de pays), `LocaliteAConfirmer` / `ConfirmerLocalite` (« Enregistrer quand même » / « Corriger ») ; ne remplit jamais une zone en lecture seule. Utilisée par la fiche patient, `ContactEdition`, `TherapeuteEdition`, `UC_Cabinet` (cabinet et praticien). |
| `UtilsSaisieReferentiels` | Listes « Pays », « Civilité », « Mutualité » (`ChargerMutualites`, `IdMutualiteSaisie`, `IdsMutualites`), « Latéralité » (`ChargerLateralites`, `IdLateraliteSaisie`, 20/10/2026) et « Spécialité » (`ChargerSpecialites`, `IdSpecialiteSaisie`, 23/10/2026) des écrans de saisie d'une personne : valeurs actives + valeur courante désactivée, pays par défaut (pays du cabinet, sinon premier pays), lecture de l'identifiant et du code ISO saisis (`CodePaysSaisi`, à transmettre à `UtilsTelephone` / `UtilsCodePostal`). |
| `UtilsNiss` | Numéro d'identification de sécurité sociale selon son type (Belgique, France, Luxembourg, Autre sans contrôle) : `Valider`, `DeduireType` (le type n'est pas stocké), `Normaliser` (forme enregistrée : sans séparateurs, majuscules, v2.54.0) et `Formater` (affichage lisible) — 23/10/2026. Sans dépendance à l'interface : appelé par le valideur métier. |
| `AffichageErreursSaisie` | Affichage commun des erreurs d'un valideur métier (23/10/2026, ADR-51) : `Signaler` place l'icône et le message sur chaque champ en cause (correspondance propriété → contrôle fournie par l'écran), récapitule toutes les erreurs dans une fenêtre et donne le focus au premier champ ; l'icône disparaît dès que le champ est modifié. |
| `UtilsPastilles` | Teintes des pastilles d'état (fond pâle + texte foncé) par famille d'affichage : séances, paiements, dossiers, rendez-vous, grilles d'administration et de référentiels ; couleurs prises dans `UITheme`. |
| `UtilsAffichage` | `SuspendreDessin` / `ReprendreDessin` : gel du dessin d'une zone pendant une construction en plusieurs étapes (toujours reprendre dans un `Finally`). |
| `UtilsIdentifiantBelge` | Identifiants belges : INAMI, BCE, TVA, IBAN / BIC (Cabinet) et codes titulaire CT1 / CT2 de la mutualité du patient (`ValiderCodeTitulaire`, 3 chiffres). Présentation : `FormaterIban` (groupes de 4), `IbanCompact` (forme enregistrée), `FormaterBic`. `EstControleParAlthea` : types d'identifiant dont le contrôle est écrit ici (INAMI, BCE, TVA ; 20/10/2026). |
| `UtilsImages` | Import d'image unifié, partie interface : dialogue de choix commun (avec contrôle du fichier), confirmation avant de remplacer une image existante, remplacement d'une image affichée (`RemplacerImage`). |

> Pagination des grilles longues : contrôle réutilisable `UC_Pagination` (`UI/Controls/Communs`), à attacher à une DataGridView alimentée en mémoire.

## Briques communes hors `Utils/`

Comportements partagés rangés près de leur couche ; à réutiliser plutôt que recopier.

| Brique | Emplacement | Responsabilité |
|---|---|---|
| `ResumeFiltre` | `UI/Controls/Communs` | Résumé cliquable au-dessus d'une grille : une pastille par filtre, un nouveau clic revient à « Tous » ; l'index est lu et choisi par l'écran (onglets Séances et Paiements du dossier, grille Patients, recherches). |
| `FiltresSeances` | `UI/Controls/Communs` | Axes du filtre des séances (`AxeFiltreSeance`), filtrage et résumé, communs à l'onglet Séances du dossier et à la recherche des séances (23/10/2026, ADR-52). Les axes se lisent sur `SeanceListeItem`, jamais sur une liste de codes. |
| `FiltresPaiements` | `UI/Controls/Communs` | Relevé des paiements (lignes affichées), axes du filtre (`AxeFiltrePaiement`), filtrage et résumé, communs à l'onglet Paiements du dossier et à la recherche des paiements (23/10/2026, ADR-52). |
| `SelecteurPeriode` | `UI/Controls/Recherche` | Période « du … au … » des recherches : Aujourd'hui, Cette semaine, Ce mois, période précédente / suivante ; un seul évènement `PeriodeChangee` par geste. Ne crée aucun contrôle : il relie ceux du Designer de l'écran. |
| `AltheaDate.CorrigerBornes` | `UI/Controls/Theme` (kit) | Garde deux champs de date « du … au … » dans le bon ordre : l'autre borne suit celle que l'on vient de choisir (23/10/2026). |
| `ErreurSaisie`, `ErreurValidationException` | `Metier/Validation` | Erreur trouvée par un valideur métier (propriété en cause par `NameOf`, message) et refus d'écriture qui les porte (`LeverSiErreurs`) — ADR-51. Affichage : `AffichageErreursSaisie`. |
| `PolitiqueMotDePasse` | `Metier/Security` | Règle unique de solidité d'un mot de passe (`LongueurMinCompte` 10, `LongueurMinSauvegarde` 12 ; majuscule, minuscule, chiffre) : `EstConforme`, `Valider`, `Description` (texte affiché par les fenêtres). |

## `Utils/Helpers/`

| Module | Responsabilité |
|---|---|
| `ArborescenceDonnees` | Racine du dossier de données (`PATH_GENERAL`, `GetRacineDonnees`) et noms fixes des sous-dossiers (`Documents`, `Cabinet`, `Utilisateurs`, préfixes `Praticien_` / `Utilisateur_`). Seul endroit où ils sont définis (ADR-35). |
| `CheminsPatientHelper` | Calcul des dossiers/fichiers patients depuis le code patient (sous `Documents/`). |
| `CheminsDossierHelper` | Arborescence déterministe des fichiers de dossier. |
| `CheminsCabinetHelper` | Dossiers et noms fixes des images du cabinet et du praticien (logo, photo, cachet, signature). |
| `CheminsDomaineHelper` | Logo facultatif d'un domaine (20/10/2026, v2.45.0) : `{PATH_GENERAL}/Domaines/Domaine_{id}/Logo.png`, PNG réduit à `TailleMaxLogo` (256 px) ; `EnregistrerLogo`, `ChargerLogo`, `SupprimerLogo`, `SupprimerDossier`. Nom de fichier seul en base (`ref_domaines.logo_fichier`). |
| `CheminsUtilisateurHelper` | Photo propre d'un compte utilisateur : `{PATH_GENERAL}/Utilisateurs/Utilisateur_{id}/Photo.<ext>` (nom fixe, rangement par identifiant), `ImporterPhoto`, `ChargerPhoto`. La règle « compte lié = photo du praticien » est portée par `PhotosUtilisateur` (`Metier/Security`), pas par le helper. |
| `ImagesHelper` | Import d'image unifié, sans interface : formats acceptés (JPG, PNG, BMP, GIF, TIFF), `EstImageValide` (extension **et** contenu réellement lisible), `CopierSousNomFixe` / `EnregistrerPngSousNomFixe` (anciennes versions supprimées quelle que soit l'extension), `ChargerSansVerrou`. ; `Reduire` / `ReduireDans` (copie réduite en haute qualité, proportions et transparence gardées) ; la nouvelle image est écrite avant que l'ancienne soit retirée (`Remplacer`, 20/10/2026) ; `SupprimerImage`. |
| `RichTextEditorHelper` | Opérations techniques de l'éditeur riche : mise en forme, styles de paragraphe, listes, recherche, impression, exports, correcteur d'orthographe et langue du texte. Sans dialogue : journalise et renvoie un résultat (ADR-42). |
| `UtilsTexte` | Forme de comparaison d'un texte saisi (24/10/2026, recherche R4 / R5) : `CleComparaison` (sans accents ni majuscules, traits d'union et apostrophes lus comme des espaces : « Grâce-Hollogne » = « grace hollogne »), `Mots` (mots sous cette forme). |
| `CorrecteurWindows` | Correcteur d'orthographe de Windows (COM, 20/10/2026, ADR-45) : `LangueDisponible`, `MotMalOrthographie`, `Suggestions`, `AjouterAuDictionnaire` ; un correcteur par langue ; aucun mot journalisé. |

## Règles importantes

- Ne pas dupliquer un comportement déjà centralisé dans un helper.
- Une couleur, une icône ou un style réutilisable ne doit pas être défini localement dans une Form ou un UserControl.
- Les helpers de chemin sont l’unique moyen de calculer un emplacement de fichier patient ou dossier.
- Les utilitaires ne portent pas de règle métier variable ; ces règles appartiennent aux services métier et aux référentiels.

## Points d’entrée utiles

- `UtilsDataGrid.ActiverEntetesMultiLignes` : en-têtes sur plusieurs lignes pour grilles compactes.
- `UtilsDataGrid.ActiverTriColonnes` : tri homogène des listes.
- `UtilsDataGrid.ElementSelectionne(Of T)` : élément de la ligne **sélectionnée** ; la ligne courante qu'une grille pose d'office au chargement ne compte pas (22/10/2026). Toute action sur une ligne passe par lui.
- `UtilsDataGrid.DessinerPastilleZone` : pastille dessinée dans une zone quelconque (éléments d'une liste de filtre `AltheaListe`).
- `UtilsDataGrid.DessinerPastilleCellule` : pastille d'état dans `CellPainting` ; rend le mode de dessin (anticrénelage) à la grille, sinon les traits des cellules suivantes deviennent flous et gris (corrigé le 19/10/2026). Tout dessin dans une grille doit faire de même.
- `CheminsPatientHelper` : répertoire déterministe sous `{PATH_GENERAL}\Documents\{code_patient}`.
- `ArborescenceDonnees` : arborescence complète du dossier de données (racine `PATH_GENERAL`, sous-dossiers fixes `Documents`, `Cabinet`, `Utilisateurs`, `Domaines`) ; tout nouveau sous-dossier s'y ajoute (ADR-35).
- `UtilsIcons` : priorité explicite lorsqu’un élément cumule plusieurs états.
- **Import d'une image** (photo, logo, signature, cachet) : `UtilsImages.ChoisirFichierImage` → `UtilsImages.ConfirmerRemplacement` → helper de chemins (`ImporterPhotoIdentite`, `ImporterImage`, `CheminsUtilisateurHelper.ImporterPhoto`) qui s'appuie sur `ImagesHelper` → `UtilsImages.RemplacerImage` pour l'affichage.
  - Le fichier est copié tel quel (pas de redimensionnement).
  - **Logo d'un domaine** : choisi puis réduit (`ImagesHelper.Reduire`) en saisie, écrit à l'enregistrement du domaine (`CheminsDomaineHelper.EnregistrerLogo`), sans confirmation de remplacement (Annuler rend l'ancien).



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

