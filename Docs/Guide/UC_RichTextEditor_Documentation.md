# UC_RichTextEditor — Éditeur de texte riche Althéa

**Version :** V2.6
**Date :** 20/10/2026
**Auteur :** Joëlle (Manou) / Projet Althéa

## 📋 Vue d'ensemble

`UC_RichTextEditor` est l'éditeur de texte riche d'Althéa : anamnèse, zones du dossier, notes et évolution des séances, alerte de la fiche patient, commentaires des contacts, intervenants et thérapeutes. Le texte est enregistré deux fois : en **RTF** (affichage, mise en forme) et en **texte brut** (recherche).

Depuis la refonte du 19/10/2026 (ADR-42, maquette « Éditeurs de texte — refonte »), **un seul contrôle** couvre tous les usages, avec deux **profils** :

| Profil | Où | Barre |
|---|---|---|
| **Complet** (défaut) | Anamnèse, zones du dossier | Historique · style de paragraphe · caractère · paragraphe · date, textes types · recherche ; à droite aperçu, impression, export. Bande de recherche, menu du clic droit, ligne d'état. |
| **Compact** | Alerte, notes et évolution de séance, commentaires | Historique · G / I / S, couleurs, effacer · puces · date, textes types ; à droite **Ouvrir en grand**. |

En **consultation** (`ReadOnlyMode`), le profil Complet garde une barre de consultation (repère « Lecture seule », rechercher, copier, aperçu, imprimer, exporter) ; le profil Compact masque sa barre.

<img src="../Illustrations/Doc_UI/UC_RichTextEditor.png" alt="Profil Complet" style="zoom:60%;" />

<img src="../Illustrations/Doc_UI/UC_RichTextEditorSimple.png" alt="Profil Compact" style="zoom:60%;" />

*Captures du 24/10/2026 (fenêtre de test `TestRichTextEditor`).*

**Fichiers :**

- `UI/Controls/Communs/UC_RichTextEditor.vb` (+ `.Designer.vb`, `.resx`) — le contrôle ;
- `Utils/Helpers/RichTextEditorHelper.vb` — opérations techniques (mise en forme, listes, recherche, impression, export) ;
- `UI/Controls/Theme/AltheaRenduBarre.vb` — rendu du kit pour la barre et les menus ;
- `UI/Forms/Communs/EditeurTexteAgrandi.vb` (+ `.Designer.vb`, `.resx`) — fenêtre « Ouvrir en grand » du profil Compact ;
- `Assets/Editor_Ico/*_20_normal.png` — icônes (sources des images intégrées par le Designer) ;
- `UI/Forms/Test/TestRichTextEditor.vb` — fenêtre de test (Debug).

## ⚡ Fonctionnalités

### Caractères

- **G**ras (Ctrl+G ou Ctrl+B), *Italique* (Ctrl+I), Souligné (Ctrl+U), barré.
- **Couleur du texte** : Texte (par défaut), Sauge, Mauve, Ocre, Terre, Bleu — teintes de la charte uniquement.
- **Surligneur** : Jaune, Sauge, Mauve pâles ; « Aucun surlignage ».
- **Effacer la mise en forme** : police, couleur, surlignage, liste, retrait et alignement normaux ; sans sélection, s'applique au texte tapé ensuite.

### Paragraphes

- **Styles** (liste « Normal ») : Titre, Sous-titre, Normal, Citation, Petit.
  - Un style s'applique au **paragraphe entier** et remplace sa mise en forme de caractères
  - les tailles sont relatives à la police de l'éditeur (Titre + 3 pt, Sous-titre + 1 pt, Petit − 2 pt)
  - Citation = italique gris avec retrait.
  - Il n'y a plus de choix libre de police ni de taille.
- **Listes** : à puces, numérotée (1. 2. 3.) ; recliquer retire la liste. Retrait suspendu : les lignes longues s'alignent sur le texte.
- **Retraits** + / −.
- **Alignement** : gauche (Ctrl+L), centre (Ctrl+E), droite (Ctrl+R), justifié (Ctrl+J) ; l'icône du bouton montre l'alignement courant.

### Insertion

- **Date** : date du jour, date et heure, et **horodatage signé** « — 19/10/2026 14:32 · P. N. » (italique gris, sur sa propre ligne). L'horodatage signé n'est proposé que si l'auteur est connu (`NomAuteur`, sinon l'utilisateur du contexte reçu par `SetContext`). Ce n'est pas une signature électronique : le texte reste modifiable.

- **Textes types** (temps 2, ADR-44) : phrases et trames du référentiel **Textes types** (Référentiels, groupe Dossier), insérées au curseur avec leur mise en forme.
  - Le bouton n'apparaît que si l'écran a déclaré l'usage de l'éditeur (`UsageTextesTypes`)
  - il propose les textes de cet usage et ceux proposés « partout » (sans usage coché), relus à chaque ouverture
  - l'infobulle de chaque ligne montre le début du texte.
  - Un texte peut servir à **plusieurs usages** (v2.43.0).
  - Usages : anamnèse, alerte du patient, zones du dossier, notes de séance, évolution de séance, commentaire d'un contact, d'un intervenant, d'un thérapeute — tous les éditeurs en ont un
  - libellés et ordre dans Référentiels › **Usages des textes types**.

### Recherche et remplacement (profil Complet)

- Ctrl+F (ou la loupe) ouvre la bande de recherche ; le mot sélectionné y est repris. Recherche au fil de la frappe, « 2 sur 5 », Entrée / F3 (suivante), Maj+Entrée / Maj+F3 (précédente), « Respecter la casse ».
- Ctrl+H (ou « Remplacer… ») : Remplacer (garde la mise en forme de l'endroit), Tout remplacer (sans boucle possible). Pas de remplacement en consultation.
- Échap ferme la bande — sans fermer la fenêtre qui contient l'éditeur.

### Presse-papiers

- Raccourcis habituels ; **coller sans mise en forme** (Ctrl+Maj+V).
- Menu du clic droit : Couper, Copier, Coller, Coller sans mise en forme, Tout sélectionner (Copier et Tout sélectionner seulement en consultation).

### Sortie (profil Complet)

- **Aperçu**, **Imprimer** (Ctrl+P), **Exporter** ▾ : PDF, Document Word, Mise en page…
- La **mise en page** (format, orientation, marges en millimètres) est gardée pour la session et reprise par l'aperçu et l'impression.
- Titre des impressions : `TitreDocument` (« Anamnèse », nom de la zone…).
- **En-tête et pied de page** (temps 2, 19/10/2026) sur chaque page, à l'impression, à l'aperçu, en PDF et en Word :
  - en-tête : nom du cabinet (sauge, gras) et titre du document (gras, à droite) ; praticien principal (« Prénom Nom — titre professionnel ») et patient (`PatientDocument`, « Prénom NOM (P000123) », à droite) ; un trait fin ;
  - pied : un trait fin, la **mention** (paramètre `DOCUMENT_MENTION_CONFIDENTIALITE`, « Confidentiel » ; vide = pas de mention), « Édité le 19/10/2026 », « Page 1 / 3 ».
  - Le contenu (`EnTeteDocument`) est relu au moment de la sortie : une modification dans l'écran Cabinet est prise en compte tout de suite. Base injoignable ou cabinet non renseigné : le document sort sans ces lignes (journalisé).
  - L'en-tête se loge à l'intérieur des marges de la mise en page : le texte commence sous l'en-tête et s'arrête au-dessus du pied.
- **Langue du document Word** : celle de Windows (format régional, français de Belgique chez Pearl), pour que Word vérifie l'orthographe en français ; le RTF du `RichTextBox` marque le texte en anglais (relevé par Joëlle, 19/10/2026).

### Ouvrir en grand (profil Compact, temps 2)

- Le bouton à droite de la barre compacte ouvre le **même texte dans une grande fenêtre** (`EditeurTexteAgrandi`) au profil Complet : styles, listes, alignements, recherche et remplacement, ligne d'état. La fenêtre prend la taille du Designer, réduite à 90 % de l'écran au plus ; elle peut être agrandie.
- Titre de la fenêtre : `TitreDocument` du champ (« Notes de séance », « Alerte / Notes »…) ; icône de l'en-tête de la fenêtre qui contient le champ, s'il y en a une ; horodatage signé du même auteur.
- **Valider** reporte le texte, mise en forme comprise, dans le champ : c'est une modification du champ, enregistrée avec la fiche ou la fenêtre qui le contient (rien n'est écrit en base par la grande fenêtre). Rien n'est reporté si le texte n'a pas changé.
- **Annuler**, croix, Échap après une modification : Enregistrer (= Valider) / Quitter sans enregistrer / Rester (ADR-43).
- Pas d'aperçu, d'impression ni d'export dans la grande fenêtre (`AfficherSorties = False`) : ce sont des notes, pas des documents.
- Seulement en édition : en consultation, le profil Compact n'a pas de barre.

### Correcteur d'orthographe (temps 2, ADR-45)

- **Correcteur de Windows**, dans tous les éditeurs, **en saisie** : les mots inconnus sont soulignés en rouge pendant la frappe. Il vérifie l'orthographe, pas la grammaire.
- **Langue** : celle du clavier (français de Belgique chez Pearl) ; un texte chargé y est rattaché automatiquement (le RTF le marquait en anglais), sans compter comme une modification.
- **Clic droit sur un mot souligné** : jusqu'à cinq suggestions en gras (clic = remplacement, mise en forme gardée), **Ajouter au dictionnaire** (dictionnaire de Windows de l'utilisateur, servi aussi à Edge, au Bloc-notes…). Pas de « Ignorer » : essayé, il donnait un résultat incohérent (mot retapé souligné sans correction proposée).
- **Réglage** : Paramètres › Éditeur de texte › « Correcteur d'orthographe » (coché par défaut) ; s'applique aux écrans ouverts ensuite.
- Fenêtre de test : case « Correcteur » et ligne de diagnostic (clavier, langues prises en charge par Windows).
- **Même correcteur hors de l'éditeur** (20/10/2026) : les notes courtes en texte simple (commentaire d'un paiement, motif d'une annulation) utilisent `AltheaZoneTexte` ; le code commun est dans `CorrecteurTexte` (Reference_UI § 7.24).

### Ligne d'état (profil Complet)

Nombre de mots, « · Modifié » (en édition, jusqu'à `MarquerEnregistre` ou au prochain chargement), zoom (Ctrl + molette ; clic = 100 %).

## 🔧 Utilisation

### Poser l'éditeur

Dans le Designer, déposer `UC_RichTextEditor` (souvent `Dock = Fill` dans une `AltheaCarte`) et choisir **Profil** (catégorie « Althéa ») : `Complet` ou `Compact`.

### Charger et enregistrer

```vb
' Chargement : RTF, avec repli sur le texte brut (contenu ancien ou RTF illisible, journalisé)
rteNotes.ChargerContenu(seance.NotesSeanceRtf, seance.NotesSeanceTxt)

' Enregistrement : les deux formes
seance.NotesSeanceRtf = rteNotes.RtfContent   ' vide si le texte est vide
seance.NotesSeanceTxt = rteNotes.TextContent

' Après un enregistrement réussi (profil Complet) : « Modifié » disparaît
rteAnamnese.MarquerEnregistre()
```

Le chargement n'est pas une modification : `ContentChanged` n'est pas levé, l'historique Annuler repart de zéro.

### Détecter les modifications

```vb
Private Sub rteAnamnese_ContentChanged(sender As Object, e As EventArgs) Handles rteAnamnese.ContentChanged
    _anamneseModifiee = True
End Sub
```

`ContentChanged` est levé pour le texte **et** pour la mise en forme (un mot passé en gras compte).

### Consultation et barre

```vb
rteZone.ReadOnlyMode = EstLectureSeule   ' barre de consultation (Complet) ou barre masquée (Compact)
rteZone.ShowToolbar = False              ' masque la barre quel que soit le mode
```

### Contexte (auteur de l'horodatage)

```vb
Public Sub SetContext(context As UserControlContext) Implements IContextAwareForm.SetContext
    _context = context
    rteCommentaire.SetContext(context)
End Sub
```

### Export contextuel (ADR-21)

Le conteneur peut fournir le chemin du fichier (dossier du patient) ; sinon l'éditeur propose un emplacement.

```vb
Private Sub rteAnamnese_ExportRequested(sender As Object, e As ExportRequestedEventArgs) Handles rteAnamnese.ExportRequested
    e.DestinationPath = CheminsPatientHelper.GetCheminFichierPatient(code, $"anamnese_{code}{If(e.Format = ExportFormat.Word, ".docx", ".pdf")}")
End Sub

Private Sub rteAnamnese_ExportCompleted(sender As Object, e As ExportCompletedEventArgs) Handles rteAnamnese.ExportCompleted
    If e.Success Then ' tracer le document, ouvrir le fichier, annoncer dans la barre d'état
End Sub
```

En export contextuel, l'éditeur n'affiche qu'un message d'échec (le conteneur annonce la réussite). Sans chemin fourni, il affiche la réussite avec le nom du fichier.

## 📦 Membres publics

| Membre | Rôle |
|---|---|
| `Profil` | `Complet` (défaut) ou `Compact`. |
| `RtfContent` | Contenu RTF (lecture) ; affectation = chargement (texte brut accepté). Non sérialisé par le Designer. |
| `TextContent` | Texte brut, pour la colonne `_txt`. |
| `ReadOnlyMode` | Consultation. |
| `ShowToolbar` | Affiche la barre (défaut : oui). |
| `TitreDocument` | Titre des impressions, nom proposé à l'export (défaut « Notes Althéa »). |
| `NomAuteur` | Nom signé par l'horodatage (défaut : utilisateur du contexte). |
| `AfficherSorties` | Aperçu, impression et export (profil Complet ; défaut : oui). |
| `PatientDocument` | Patient imprimé dans l'en-tête des documents (`EnTeteDocument.DesignerPatient(patient)`). |
| `CorrecteurOrthographe` | Correcteur de Windows (défaut : oui) ; actif en saisie si le réglage le permet. |
| `UsageTextesTypes` | Usage de l'éditeur (`UsagesTexteType.NotesSeance`…), renseigné par le code de l'écran ; vide = pas de bouton Textes types. |
| `EstModifie` | Le contenu a changé depuis le chargement ou `MarquerEnregistre`. |
| `ChargerContenu(rtf, txt)` | Charge avec repli sur le texte brut. |
| `MarquerEnregistre()` | Efface l'état « Modifié ». |
| `SetContext(context)` | Contexte partagé (facultatif). |
| `ContentChanged` | Texte ou mise en forme modifiés (hors chargement). |
| `ExportRequested` / `ExportCompleted` | Export contextuel (ADR-21). |

## ⌨️ Raccourcis

| Touches | Action |
|---|---|
| Ctrl+G / Ctrl+B, Ctrl+I, Ctrl+U | Gras, italique, souligné |
| Ctrl+L / E / R / J | Aligner à gauche, centrer, à droite, justifier |
| Ctrl+Z / Ctrl+Y | Annuler, rétablir |
| Ctrl+X / C / V, Ctrl+Maj+V | Couper, copier, coller, coller sans mise en forme |
| Ctrl+F, Ctrl+H, F3 / Maj+F3, Échap | Rechercher, remplacer, occurrence suivante / précédente, fermer la recherche |
| Ctrl+P | Imprimer (profil Complet) |
| Ctrl + molette | Zoom |

Les raccourcis sont traités par `ProcessCmdKey`, avant le `RichTextBox` et avant la fenêtre : la touche est absorbée (plus de tabulation sur Ctrl+I) et, en consultation, aucun raccourci ne modifie le texte.

## 🎨 Apparence

- Barre blanche soulignée d'un trait beige ; boutons Outil (transparents, survol vert-de-gris pâle, coché ou enfoncé sauge clair) ; désactivés à 40 % d'opacité — rendu `AltheaRenduBarre`, réutilisable par toute `ToolStrip` ou `ContextMenuStrip`.
- Texte Calibri 11 gris foncé sur fond beige clair, marge intérieure de 10 px, sans cadre (la carte encadre).
- Couleurs : `UITheme` (dont la région « Éditeur de texte » : `EditeurTexteTerre`, `EditeurTexteBleu`, `EditeurSurligneurJaune`).

### Icônes

Un seul style : trait arrondi de 1,8 px, Sauge très foncé, 20 × 20 px.

Source unique en SVG (générateur de la maquette), exportée en PNG `nom_20_normal.png` dans `Assets/Editor_Ico/` ; le Designer les intègre au `.resx` de l'éditeur.

Pas de variantes `_hover` / `_disabled` : le survol est un fond, l'état désactivé une opacité.

Pour ajouter un bouton : dessiner l'icône dans le même style, l'exporter, la poser comme `Image` du bouton dans le Designer, puis l'ajouter à `DefinirPresences` (profils où il apparaît).

## 🛠️ Maintenance

- **Nouveau bouton** : élément de la `ToolStrip` dans le Designer (`DisplayStyle = Image`, 30 × 30, infobulle avec le raccourci), gestionnaire qui passe par `ExecuterMiseEnForme` s'il modifie le texte (consultation, `ContentChanged`, focus), présence dans `DefinirPresences`.
- **Nouvelle opération technique** : dans `RichTextEditorHelper`, sans dialogue ; en cas d'échec, journaliser (sans contenu ni chemin) et renvoyer `False`.
- **Nouveau style ou nouvelle couleur** : `RichTextEditorHelper.DefinitionStyle`, `UC_RichTextEditor.CouleursTexte` / `CouleursSurligneur` (teintes dans `UITheme`).

## ✅ Temps 2 (terminé le 20/10/2026)

Plan validé par Joëlle le 19/10/2026, un chantier à la fois : « Ouvrir en grand » pour le profil Compact ; en-tête et pied de page ; textes types ; correcteur d'orthographe (Windows, ADR-45). Pistes pour plus tard (ToDo) : champs à remplacer dans un texte type (date, patient), grammaire.

## 📚 Références

- ADR-15 (composant), ADR-21 (export délégué), ADR-41 (licence Syncfusion), ADR-42 (refonte), ADR-44 (textes types), ADR-45 (correcteur).
- Plan de tests : `Docs/Tests/PLAN_TESTS_UC_RICHTEXTEDITOR.md` (V2.6).
- Historique de l'implémentation : `Docs/Conception/Historique_Implementation_RichTextEditor.md`.

---

**Dernière mise à jour :** 19/10/2026
**Statut :** refonte (temps 1) — à valider par les tests

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
