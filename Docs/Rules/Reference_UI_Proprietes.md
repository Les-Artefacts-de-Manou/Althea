# Althéa - Référence des Propriétés UI

 *Dernière mise à jour : 07/06/2026*

Ce document répertorie les propriétés personnalisées (non-par-défaut) de tous les contrôles utilisés dans l'application Althéa.

> **Note** : Seules les propriétés **modifiées par rapport aux valeurs par défaut** sont documentées ici.  
> Pour un guide d'utilisation pratique avec exemples de code, voir [`Reference_UI_Guide_Utilisation.md`](./Reference_UI_Guide_Utilisation.md).

---

## Table des matières

1. [Form](#form)
2. [Panel](#panel)
   - [pnlForm](#pnlform)
   - [pnlTop](#pnltop)
   - [pnlTitre](#pnltitre)
   - [pnlCenter](#pnlcenter)
   - [pnlActions](#pnlactions)
   - [pnlHeader (Home)](#pnlheader-home)
   - [pnlMenu (Home)](#pnlmenu-home)
   - [pnlContent (Home)](#pnlcontent-home)
3. [Button](#button)
   - [Bouton Standard](#bouton-standard)
   - [Bouton Tuile](#bouton-tuile)
   - [Bouton Home Menu](#bouton-home-menu)
   - [Bouton Icône seul](#bouton-icône-seul)
4. [Label](#label)
   - [lblTitreForm](#lbltitreform)
   - [lblTop](#lbltop)
   - [lblContexte (Home)](#lblcontexte-home)
   - [lblUtilisateurConnecte (Home)](#lblutilisateurconnecte-home)
   - [Label standard](#label-standard)
   - [Label message erreur](#label-message-erreur)
   - [Label info (badge)](#label-info-badge)
5. [TextBox](#textbox)
6. [ComboBox](#combobox)
7. [CheckBox](#checkbox)
8. [DateTimePicker](#datetimepicker)
9. [NumericUpDown](#numericupdown)
10. [DataGridView](#datagridview)
11. [TableLayoutPanel](#tablelayoutpanel)
12. [PictureBox](#picturebox)
13. [StatusStrip](#statusstrip)
14. [ErrorProvider](#errorprovider)
15. [ToolTip](#tooltip)
16. [UserControl](#usercontrol)
17. [DialogChoix (Form personnalisée)](#dialogchoix-form-personnalisée)

---

## Form

### Propriétés communes aux Forms

```vb
' Form standard (petite/moyenne)
AutoScaleDimensions = New SizeF(6F, 14F)
AutoScaleMode = AutoScaleMode.Font
ClientSize = New Size(760, 560)  ' ou Size(1000, 800) pour large
Font = New Font("Calibri", 9F)
Icon = CType(resources.GetObject("$this.Icon"), Icon)  ' Althea_A_96x96.ico
StartPosition = FormStartPosition.CenterScreen
Text = "Althéa - [Nom de la fonctionnalité]"
```

### Form Home (principale)

```vb
ClientSize = New Size(1184, 901)
MinimizeBox = False
MinimumSize = New Size(945, 703)
Name = "Home"
Text = "Althéa"
```

### Tailles standard des Forms

| Taille  | ClientSize      | Usage                                |
| ------- | --------------- | ------------------------------------ |
| Petite  | 464 x 361       | Login, dialogs simples               |
| Moyenne | 760 x 560       | Forms modales, configuration         |
| Large   | 1000 x 800      | Forms de gestion complexes           |
| Home    | 1184 x 901      | Form principale (Shell)              |

---

## Panel

### pnlForm

**Rôle** : Panel conteneur principal de toute Form ou UserControl, sert de fond et contient tous les autres contrôles.

```vb
' Form standard
BackColor = Color.FromArgb(244, 239, 234)  ' ColorBeigeClair
BackgroundImage = My.Resources.Resources.Fond_1000x800_Feuille4
BackgroundImageLayout = ImageLayout.Stretch
Dock = DockStyle.Fill
Font = New Font("Calibri", 11F)

' UserControl
BackgroundImage = My.Resources.Resources.Fond_1000x770_FeuilleCoupee1
Padding = New Padding(16, 16, 16, 2)
```

**Règle** : Une seule `pnlForm` par écran, avec `Dock = Fill` (règle respectée 👍)

---

### pnlTop

**Rôle** : Panel de description/instructions en haut de la zone de contenu.

```vb
BackColor = Color.Transparent
Dock = DockStyle.Top
Font = New Font("Calibri", 11F)
Height = 39  ' à 58 selon contenu
Padding = New Padding(7) ' ou (8)
```

---

### pnlTitre

**Rôle** : Panel contenant le titre de la Form/UserControl.

```vb
BackColor = Color.Transparent
Dock = DockStyle.Top
Height = 54  ' à 72 selon contenu
Padding = New Padding(8)
```

---

### pnlCenter

**Rôle** : Panel central contenant le contenu principal (formulaires, grilles, etc.).

```vb
BackColor = Color.Transparent
Dock = DockStyle.Fill
Padding = New Padding(4, 6, 4, 6)  ' ou (5, 6, 5, 6) ou (16)
```

---

### pnlActions

**Rôle** : Panel en bas contenant les boutons d'action (Valider, Annuler, Enregistrer, etc.).

```vb
Anchor = AnchorStyles.Top Or AnchorStyles.Left  ' si non docké
BackColor = Color.Transparent
BackgroundImage = My.Resources.Resources.Althea_Bandeau_Haut_Trans
BackgroundImageLayout = ImageLayout.Stretch
BorderStyle = BorderStyle.Fixed3D
Dock = DockStyle.Bottom
Font = New Font("Calibri", 10F)
Height = 50  ' minimum 60 selon boutons
Padding = New Padding(8) ' ou (12, 7, 12, 7)
```

---

### pnlHeader (Home)

**Rôle** : Panel d'en-tête de la Form Home contenant logo, utilisateur connecté et contexte.

```vb
BackColor = Color.Transparent
Dock = DockStyle.Top
Height = 94
```

**Contient** :
- `picTitre` : Logo Althéa
- `lblUtilisateurConnecte` : Affichage utilisateur connecté
- `lblContexte` : Affichage du contexte courant

---

### pnlMenu (Home)

**Rôle** : Panel menu vertical gauche dans Home, contient les boutons de navigation principaux.

```vb
BackColor = Color.FromArgb(235, 226, 217)  ' ColorBeige
Dock = DockStyle.Left
Padding = New Padding(8)
Width = 189
```

---

### pnlContent (Home)

**Rôle** : Panel de contenu dynamique dans Home, reçoit les UserControls injectés.

```vb
AllowDrop = True
BackColor = Color.Transparent
Dock = DockStyle.Fill
Padding = New Padding(8)
Size = New Size(995, 775)  ' Taille standard des UC
```

---

## Button

### Bouton Standard

**Usage** : Boutons d'action dans `pnlActions` (Enregistrer, Annuler, Valider, Fermer, etc.).

```vb
BackColor = Color.FromArgb(122, 155, 135)  ' ColorSauge
FlatAppearance.BorderSize = 0
FlatStyle = FlatStyle.Flat
Font = New Font("Calibri", 10F)
ForeColor = Color.White
Height = 37  ' minimum 40
Image = CType(resources.GetObject("btn[Nom].Image"), Image)  ' 32x32 PNG
ImageAlign = ContentAlignment.MiddleLeft
Size = New Size(93, 37)  ' largeur variable selon texte
Tag = "[action]_normal"  ' ex: "enregistrer_normal", "annuler_normal"
Text = "[Texte]"
TextAlign = ContentAlignment.MiddleLeft  ' (optionnel si ImageBeforeText suffit)
TextImageRelation = TextImageRelation.ImageBeforeText
UseVisualStyleBackColor = False
```

**Icônes** : PNG 32x32 dans `/Assets/Boutons_ico_32/`
- `[action]_normal.png` : état par défaut
- `[action]_hover.png` : survol
- `[action]_disabled.png` : désactivé

**Actions courantes** : `enregistrer`, `annuler`, `valider`, `fermer`, `modifier`, `nouveau`, `login`, `voir`, `testerConnexion`, `eleverAcces`, `retourRole`, `modifierPW`, `actualiser`, `rechercher`, `reinitialiser`, `activer`, `desactiver`, `deverrouiller`, `reset_password`

---

### Bouton Tuile

**Usage** : Grands boutons de navigation dans les écrans d'accueil (UC_AdminHome, etc.).

```vb
Anchor = AnchorStyles.None
BackColor = Color.FromArgb(122, 155, 135)  ' ColorSauge
BackgroundImageLayout = ImageLayout.Center
FlatAppearance.BorderSize = 0
FlatStyle = FlatStyle.Flat
Font = New Font("Calibri", 12F, FontStyle.Bold)
ForeColor = Color.White
Image = CType(resources.GetObject("btn[Nom].Image"), Image)  ' 48x48 PNG
Size = New Size(235, 90)  ' minimum Height = 90
Tag = "[action]_normal"
Text = "[Titre]" & vbCrLf & "[Description]"  ' texte multi-ligne
TextImageRelation = TextImageRelation.ImageBeforeText
UseCompatibleTextRendering = True  ' (optionnel)
UseVisualStyleBackColor = False
```

**Icônes** : PNG 48x48 dans `/Assets/Boutons_ico_48/`
- Actions : `parametres`, `utilisateurs`, `logs`, `sauvegarde`, `configurationDatabase`

---

### Bouton Home Menu

**Usage** : Boutons de navigation dans le menu gauche de Home.

```vb
BackColor = Color.FromArgb(235, 226, 217)  ' ColorBeige
BackgroundImageLayout = ImageLayout.None  ' ou Stretch
FlatAppearance.BorderSize = 0
FlatStyle = FlatStyle.Flat
Font = New Font("Calibri", 13F, FontStyle.Bold)
ForeColor = Color.FromArgb(95, 125, 110)  ' ColorSaugeFonce
Image = CType(resources.GetObject("btn[Nom].Image"), Image)  ' 48x48 PNG
ImageAlign = ContentAlignment.MiddleLeft
Location = New Point(10, [Y])  ' espacement vertical de 60
Padding = New Padding(10, 0, 0, 0)  ' ou (4, 0, 0, 0) pour btnAdmin
Size = New Size(169, 60)  ' ou (175, 60)
Tag = "[nom]"  ' ex: "accueil", "patients", "outils_admin"
Text = "[Texte]"
TextAlign = ContentAlignment.MiddleLeft
TextImageRelation = TextImageRelation.ImageBeforeText
UseVisualStyleBackColor = False
```

**Icônes** : PNG 48x48 dans `/Assets/Boutons_Home/`
- `[nom]_normal.png` : état par défaut
- `[nom]_selected.png` : sélectionné
- `[nom]_disabled.png` : désactivé

**Menus** : `accueil`, `patients`, `domaines`, `agenda`, `documents`, `referentiels`, `outils_admin`

---

### Bouton Icône seul

**Usage** : Boutons d'action visuels sans texte (ex: voir/masquer mot de passe).

```vb
BackColor = Color.FromArgb(122, 155, 135)  ' ColorSauge
Image = CType(resources.GetObject("btn[Nom].Image"), Image)  ' 32x32 PNG
Size = New Size(38, 38)
Tag = "[action]_normal"
TextImageRelation = TextImageRelation.ImageAboveText
UseVisualStyleBackColor = False
```

---

## Label

### lblTitreForm

**Rôle** : Titre principal de la Form/UserControl, affiché dans `pnlTitre`.

```vb
AutoSize = True
BackColor = Color.FromArgb(244, 239, 234)  ' ColorBeigeClair (style "badge")
Font = New Font("Calibri", 15F, FontStyle.Bold)
ForeColor = Color.FromArgb(95, 125, 110)  ' ColorSaugeFonce
Location = New Point(32, 14)
Padding = New Padding(8, 4, 8, 4)
Text = "[Titre de la Form/UC]"
```

**Style** : Badge avec fond coloré et padding, aligné à gauche (pas centré).

---

### lblTop

**Rôle** : Label d'instructions/description dans `pnlTop`.

```vb
Dock = DockStyle.Fill
Font = New Font("Calibri", 10F)  ' ou 11F
ForeColor = Color.FromArgb(95, 125, 110)  ' ColorSaugeFonce
Text = "[Instructions ou description]"
```

---

### lblContexte (Home)

**Rôle** : Affiche le contexte courant dans l'en-tête de Home (patient/dossier, mode d'accès, etc.).

```vb
BorderStyle = BorderStyle.Fixed3D
Font = New Font("Calibri", 11F, FontStyle.Bold)
ForeColor = Color.FromArgb(74, 74, 74)  ' ColorGrisFonce
Location = New Point(325, 50)
Padding = New Padding(6)
Size = New Size(600, 32)
Text = "Contexte courant ou nom du patient/dossier"
```

---

### lblUtilisateurConnecte (Home)

**Rôle** : Affiche l'utilisateur connecté et son rôle dans l'en-tête de Home.

```vb
BorderStyle = BorderStyle.Fixed3D
Font = New Font("Calibri", 11F, FontStyle.Bold)
ForeColor = Color.FromArgb(95, 125, 110)  ' ColorSaugeFonce
Location = New Point(325, 9)
Padding = New Padding(6)
Size = New Size(600, 32)
Text = "Connecté : -"
```

---

### Label standard

**Usage** : Labels de formulaire (descriptions de champs).

```vb
AutoSize = True
Font = New Font("Calibri", 11F)  ' ou taille du panel parent
ForeColor = Color.FromArgb(95, 125, 110)  ' ColorSaugeFonce
Text = "[Texte du label]"
```

---

### Label message erreur

**Usage** : Affichage de messages d'erreur ou d'avertissement.

```vb
Font = New Font("Calibri", 11F, FontStyle.Bold)
ForeColor = Color.FromArgb(194, 106, 118)  ' ColorRoseFonce (erreur)
Text = "[Message d'erreur]"
```

---

### Label info (badge)

**Usage** : Labels d'information mis en évidence (rôle courant, élévation, etc.).

```vb
Font = New Font("Calibri", 12F, FontStyle.Bold)
ForeColor = Color.FromArgb(11, 95, 125)  ' ColorMauveFonce
TextAlign = ContentAlignment.MiddleCenter
Text = "[Information]"
```

---

## TextBox

### TextBox standard

```vb
BackColor = Color.FromArgb(244, 239, 234)  ' ColorBeigeClair
Font = New Font("Calibri", 11F)  ' ou taille du panel parent
ForeColor = Color.FromArgb(74, 74, 74)  ' ColorGrisFonce
Size = New Size(209, 22)  ' largeur variable
```

### TextBox mot de passe

```vb
BackColor = Color.FromArgb(244, 239, 234)  ' ColorBeigeClair
Font = New Font("Calibri", 11F)
ForeColor = Color.FromArgb(74, 74, 74)  ' ColorGrisFonce
UseSystemPasswordChar = True  ' masquer le texte
```

---

## ComboBox

### ComboBox standard

```vb
BackColor = Color.FromArgb(244, 239, 234)  ' ColorBeigeClair
DropDownStyle = ComboBoxStyle.DropDownList  ' empêche la saisie manuelle
Font = New Font("Calibri", 11F)
ForeColor = Color.FromArgb(74, 74, 74)  ' ColorGrisFonce
```

**Usage typique** : Sélection de rôles, filtres, catégories.

---

## CheckBox

### CheckBox standard

```vb
AutoSize = True
Font = New Font("Calibri", 11F)
ForeColor = Color.FromArgb(95, 125, 110)  ' ColorSaugeFonce
CheckAlign = ContentAlignment.MiddleLeft
TextAlign = ContentAlignment.MiddleLeft
```

---

## DateTimePicker

### DateTimePicker standard

```vb
Font = New Font("Calibri", 11F)
Format = DateTimePickerFormat.Custom
CustomFormat = "dd/MM/yyyy"  ' ou "dd/MM/yyyy HH:mm" avec heure
ShowCheckBox = True  ' pour dates optionnelles
```

---

## NumericUpDown

### NumericUpDown standard

```vb
BackColor = Color.FromArgb(244, 239, 234)  ' ColorBeigeClair
Font = New Font("Calibri", 11F)
ForeColor = Color.FromArgb(74, 74, 74)  ' ColorGrisFonce
TextAlign = HorizontalAlignment.Right
DecimalPlaces = 0  ' ou 2 pour décimaux
Minimum = 0
Maximum = 999999
Increment = 1  ' ou 0.01 pour décimaux
```

---

## DataGridView

### DataGridView standard

```vb
AllowUserToAddRows = False
AllowUserToDeleteRows = False
AutoGenerateColumns = False
BackgroundColor = Color.White
ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
DefaultCellStyle.Font = New Font("Calibri", 9F)
GridColor = Color.FromArgb(210, 210, 210)  ' ColorGrisMoyen
MultiSelect = False
ReadOnly = True
RowHeadersVisible = False
SelectionMode = DataGridViewSelectionMode.FullRowSelect
```

### DataGridViewImageColumn (colonne icône d'état)

```vb
Name = "colEtat"
HeaderText = "État"
Width = 50
Resizable = DataGridViewTriState.False
```

**Gestion des icônes** : Via `UtilsIcons.IconOK()`, `IconOFF()`, `IconLock()` dans `CellFormatting`.

### DataGridViewTextBoxColumn (colonne date)

```vb
DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"
```

---

## TableLayoutPanel

**Usage** : Disposition structurée de contrôles en grille (formulaires, menus).

### TableLayoutPanel formulaire (2-3 colonnes)

```vb
ColumnCount = 3
ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30F))  ' Labels
ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))  ' Champs
ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20F))  ' Actions/Icônes
Dock = DockStyle.Fill
Padding = New Padding(12)
RowCount = [N]
RowStyles.Add(New RowStyle(SizeType.Percent, [X]F))  ' par ligne
```

### TableLayoutPanel menu (2 colonnes tuiles)

```vb
ColumnCount = 2
ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
Dock = DockStyle.Fill
RowCount = 3
RowStyles.Add(New RowStyle(SizeType.Percent, 33.3333321F))  ' par ligne
```

---

## PictureBox

### picTitre (Home)

**Rôle** : Logo Althéa dans l'en-tête de Home.

```vb
BackgroundImage = My.Resources.Resources.Althea1_Transp_EnTeteHome
BackgroundImageLayout = ImageLayout.Stretch
Location = New Point(56, 12)
Size = New Size(191, 48)
TabStop = False
```

**Image** : `/Assets/Splash/Althea1_Transp_EnTeteHome.png`

---

## StatusStrip

### stsStatus

**Rôle** : Barre de statut en bas de Home (centralisée) et des Forms individuelles.

```vb
AllowItemReorder = True
AutoSize = False
BackColor = Color.FromArgb(218, 201, 184)  ' ColorBeigeFonce
Dock = DockStyle.Bottom  ' automatique
Height = 22  ' ou 32 pour Home
Padding = New Padding(1, 0, 12, 0)
```

### stsLabelStatus

**Rôle** : Label de statut dans le StatusStrip.

```vb
BackColor = Color.FromArgb(244, 239, 234)  ' ColorBeigeClair
Font = New Font("Calibri", 9.75F, FontStyle.Bold)
ForeColor = Color.FromArgb(11, 95, 125)  ' ColorMauveFonce
Text = "Althea"  ' texte par défaut
```

**Note** : Les UserControls n'ont pas de StatusStrip et utilisent celui de Home via `_context.SetStatus()`.

---

## ErrorProvider

### errProvider

**Rôle** : Affichage des icônes d'erreur à côté des contrôles invalides.

```vb
ContainerControl = Me  ' référence à la Form/UC
```

**Usage** :
- Dans Form : instance locale `errProvider`
- Dans UserControl : via `_context.SetError(control, message)` et `_context.ClearError(control)`

---

## ToolTip

### ttMain

**Rôle** : Affichage des infobulles au survol des contrôles.

```vb
' Pas de propriétés personnalisées standard, défini par composant
```

**Usage** :
- Dans Form : `ttMain.SetToolTip(control, "Texte de l'infobulle")`
- Dans UserControl : `_context.SetToolTip(control, "Texte de l'infobulle")`

**Règle** : Chaque contrôle interactif doit avoir son tooltip (via Designer ou code).

---

## UserControl

### Propriétés de base

```vb
AutoScaleDimensions = New SizeF(6F, 14F)
AutoScaleMode = AutoScaleMode.Font
BackColor = Color.Transparent  ' hérite du parent
Font = New Font("Calibri", 9F)
Size = New Size(997, 771)  ' taille standard pour injection dans pnlContent (995x775)
```

### Interface IContextAwareUserControl

**Tous les UserControls doivent implémenter `IContextAwareUserControl`** pour recevoir le contexte partagé via `NavigationManager`.

```vb
Public Class UC_[Nom]
	Inherits System.Windows.Forms.UserControl
	Implements IContextAwareUserControl

	Private _context As UserControlContext

	Public Sub SetContext(context As UserControlContext) _
		Implements IContextAwareUserControl.SetContext
		_context = context
	End Sub
End Class
```

**Accès au contexte** :
- `_context.SetStatus(message)` : afficher un message dans le StatusStrip de Home
- `_context.SetHeader(text)` : mettre à jour le label de contexte de Home
- `_context.SetToolTip(control, text)` : définir une infobulle
- `_context.SetError(control, message)` : afficher une erreur
- `_context.ClearError(control)` : effacer une erreur
- `_context.ClearAllErrors()` : effacer toutes les erreurs
- `_context.AuthenticatedUser` : utilisateur connecté
- `_context.HomeForm` : référence à la Form Home

---

## Icônes et images techniques

### Icônes d'état (PictureBox / DataGridView)

| Nom       | Image    | Fichier                         | Taille | Usage                        | Module     |
| --------- | -------- | ------------------------------- | ------ | ---------------------------- | ---------- |
| OK_32x32  | ✅ Vert   | `/Assets/Tech_ico/OK_32x32.png` | 32x32  | État OK/Actif                | UtilsIcons |
| OFF_32x32 | ⚪ Gris   | `/Assets/Tech_ico/OFF_32x32.png` | 32x32 | État inactif/Désactivé       | UtilsIcons |
| LOCK_32x32| 🔒 Rouge | `/Assets/Tech_ico/LOCK_32x32.png` | 32x32 | Compte verrouillé (priorité) | UtilsIcons |
| NO_32x32  | ❌ Rouge | `/Assets/Tech_ico/NO_32x32.png` | 32x32 | Refus/Erreur                 | UtilsIcons |

**Règle de priorité des icônes d'état** : Verrouillé > Actif > Inactif

**Couleur OK** : `Color.FromArgb(95, 125, 110)` (ColorSaugeFonce)

**Gestion centralisée** : Toutes les icônes d'état doivent être obtenues via `UtilsIcons` :
```vb
picEtat.Image = UtilsIcons.IconOK(32)
picEtat.Image = UtilsIcons.IconOFF(32)
picEtat.Image = UtilsIcons.IconLock(32)
picEtat.Image = UtilsIcons.IconNo(32)
```

### Icônes DialogChoix (animées)

| Type        | Image | Fichier                                 | Taille | Format |
| ----------- | ----- | --------------------------------------- | ------ | ------ |
| Information | ℹ️     | `/Assets/Dialogue_ico/information.gif`  | 64x64  | GIF    |
| Warning     | ⚠️     | `/Assets/Dialogue_ico/warning.gif`      | 64x64  | GIF    |
| Error       | ❌     | `/Assets/Dialogue_ico/error.gif`        | 64x64  | GIF    |
| Success     | ✅     | `/Assets/Dialogue_ico/success.gif`      | 64x64  | GIF    |
| Question    | ❓     | `/Assets/Dialogue_ico/question.gif`     | 64x64  | GIF    |
| Loading     | ⌛     | `/Assets/Dialogue_ico/loading.gif`      | 64x64  | GIF    |
| Processing  | ⚙️     | `/Assets/Dialogue_ico/processing.gif`   | 64x64  | GIF    |

**Support animations** : Les fichiers GIF animés sont supportés et s'affichent automatiquement.

---

## DialogChoix (Form personnalisée)

### Propriétés DialogChoix

**Rôle** : Form de dialogue personnalisée remplaçant tous les `MessageBox.Show()` dans l'application.

```vb
AutoScaleDimensions = New SizeF(6F, 14F)
AutoScaleMode = AutoScaleMode.Font
ClientSize = New Size(450, 250)  ' ajusté dynamiquement selon le contenu
Font = New Font("Calibri", 10F)
FormBorderStyle = FormBorderStyle.FixedDialog
MaximizeBox = False
MinimizeBox = False
StartPosition = FormStartPosition.CenterParent
Text = "[Titre du dialogue]"
```

### Propriétés publiques

```vb
Public Property Titre As String = "Confirmation"
Public Property Message As String = ""
Public Property TypeDialogue As TypeDialogue = TypeDialogue.Information
```

### Enum TypeDialogue

```vb
Public Enum TypeDialogue
    Information
    Warning
    [Error]
    Question
    Success
    Loading
    Processing
End Enum
```

### Méthodes statiques

```vb
' Usage simplifié
DialogChoix.Information("Message")
DialogChoix.Avertissement("Message")
DialogChoix.Erreur("Message")
DialogChoix.Succes("Message")
If DialogChoix.Confirmer("Message") = DialogResult.Yes Then...
```

### Configuration des boutons

```vb
' 1 bouton
dlg.SetBoutons("OK")

' 2 boutons
dlg.SetBoutons("Oui", "Non")

' 3 boutons
dlg.SetBoutons("Oui", "Non", "Annuler")
```

**Mapping DialogResult** :
- Bouton1 → `DialogResult.Yes`
- Bouton2 → `DialogResult.No`
- Bouton3 → `DialogResult.Cancel`

---

## Récapitulatif des assets

| Type             | Répertoire                | Format | Tailles         | Usage                              |
| ---------------- | ------------------------- | ------ | --------------- | ---------------------------------- |
| Fond Form        | `/Assets/Fond/`           | PNG    | 1000x800        | `pnlForm.BackgroundImage`          |
| Fond UC          | `/Assets/Fond/`           | PNG    | 1000x770        | `pnlForm.BackgroundImage`          |
| Fond Actions     | `/Assets/Fond/`           | PNG    | Stretch         | `pnlActions.BackgroundImage`       |
| Logo Althéa      | `/Assets/Splash/`         | PNG    | 191x48          | `picTitre.BackgroundImage` (Home)  |
| Icône App        | `/Assets/Appli_Ico/`      | ICO    | 96x96           | `Form.Icon`                        |
| Boutons Standard | `/Assets/Boutons_ico_32/` | PNG    | 32x32           | `Button.Image` (actions)                     |
| Boutons Tuile    | `/Assets/Boutons_ico_48/` | PNG    | 48x48           | `Button.Image` (navigation)                  |
| Boutons Home     | `/Assets/Boutons_Home/`   | PNG    | 48x48           | `Button.Image` (menu Home)                   |
| Icônes Tech      | `/Assets/Tech_ico/`       | PNG    | 32x32           | État OK/OFF/Lock dans grilles/PictureBox     |
| Icônes DialogChoix | `/Assets/Dialogue_ico/` | GIF    | 64x64           | Icônes animées dans DialogChoix              |

**Nomenclature des images de bouton** :
- `[action]_normal.png` : état par défaut
- `[action]_hover.png` ou `_selected.png` : survol/sélection
- `[action]_disabled.png` : désactivé

---

## Notes d'implémentation

### Règles de nommage

| Contrôle        | Préfixe | Exemple                |
| --------------- | ------- | ---------------------- |
| Button          | `btn`   | `btnEnregistrer`       |
| Label           | `lbl`   | `lblTitreForm`         |
| TextBox         | `txt`   | `txtUserName`          |
| Panel           | `pnl`   | `pnlForm`, `pnlActions`|
| TableLayoutPanel| `tlp` ou `tbl` | `tlpLogin`, `tblMenu` |
| PictureBox      | `pic`   | `picTitre`             |
| StatusStrip     | `sts`   | `stsStatus`            |
| ToolStripStatusLabel | `sts` | `stsLabelStatus` |
| ErrorProvider   | `err`   | `errProvider`          |
| ToolTip         | `tt`    | `ttMain`               |
| UserControl     | `UC_`   | `UC_AdminHome`         |

### Hiérarchie des conteneurs

```
Form/UserControl
└── pnlForm (Dock=Fill, fond d'écran)
	├── pnlTitre (Dock=Top)
	│   └── lblTitreForm
	├── pnlTop (Dock=Top)
	│   └── lblTop
	├── pnlCenter (Dock=Fill)
	│   └── [Contenu : TableLayoutPanel, grilles, etc.]
	└── pnlActions (Dock=Bottom)
		└── [Boutons d'action]
```

### Ordre de docking

1. **Top** : `pnlTitre`, `pnlTop`
2. **Bottom** : `pnlActions`
3. **Left** : `pnlMenu` (Home seulement)
4. **Fill** : `pnlCenter` ou `pnlContent`

### Gestion des états de boutons (Hover/Disabled)

Les changements d'images de boutons sont gérés par `UtilsButtons.vb` via les événements :
- `MouseEnter` : passe à l'image `_hover` ou `_selected`
- `MouseLeave` : retour à l'image `_normal`
- `EnabledChanged` : passe à l'image `_disabled` si `Enabled = False`

**Convention de Tag** : `[action]_normal` (le suffixe `_normal` est remplacé dynamiquement).

---

## Adaptation des régions

Les régions suggérées dans les fichiers Designer ne doivent **pas être modifiées manuellement** (elles sont générées automatiquement par Visual Studio).

Pour l'organisation du code métier, voir le document [`Standards-Commentaires.md`](./Standards-Commentaires.md), section **Organisation en Régions**.

---

> **Contact** : ***Joëlle (Manou) - Les Artefacts de Manou***
>
> Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.
> - Site web P.Nguyen Duy: https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu`
> - GitHub privé: Althea https://github.com/AngeljoNG/Althea
> - GitHub public: Althea https://github.com/Les-Artefacts-de-Manou/Althea
>
> ---
>





[TOC]

