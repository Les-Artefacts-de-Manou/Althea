# Althéa - Guide d'Utilisation UI et Contrôles

 *Dernière mise à jour : 07/06/2026*

> Guide de référence rapide pour l'utilisation pratique des contrôles UI standards dans Althéa avec exemples de code.  
> Pour les propriétés techniques détaillées des contrôles, voir [`Reference_UI_Proprietes.md`](./Reference_UI_Proprietes.md).  

> Dernière mise à jour : 07 juin 2026

---

## Table des matières

1. [Boutons](#boutons)
2. [Panels](#panels)
3. [Labels](#labels)
4. [TextBox](#textbox)
5. [ComboBox](#combobox)
6. [CheckBox](#checkbox)
7. [DateTimePicker](#datetimepicker)
8. [DataGridView](#datagridview)
9. [TableLayoutPanel](#tablelayoutpanel)
10. [StatusStrip](#statusstrip)
11. [ToolTip](#tooltip)
12. [ErrorProvider](#errorprovider)
13. [PictureBox](#picturebox)
14. [NumericUpDown](#numericupdown)

---

## Boutons

### Types de boutons

Althéa utilise trois types de boutons principaux :

#### 1. Boutons Standard (40px height)

**Utilisation** : Actions courantes dans les Forms et UserControls

**Initialisation** :
```vb
UtilsButtons.InitStandardButton(btnNom)
```

**Propriétés** :
- Height : 40
- BackColor : `UITheme.ColorSauge`
- ForeColor : `Color.White`
- FlatStyle : `Flat`
- Font : Calibri 10pt
- Tag : `nom_normal` (sans `.png`)
- Icône : 32x32 pixels dans `/Assets/Boutons_ico_32/`

**Icônes disponibles** :
- nouveau_normal
- modifier_normal
- enregistrer_normal
- annuler_normal
- fermer_normal
- valider_normal
- actualiser_normal
- rechercher_normal
- reinitialiser_normal
- activer_normal
- desactiver_normal
- deverrouiller_normal
- reset_password_normal
- voir_normal
- eleverAcces_normal
- retourRole_normal
- testerConnexion_normal
- modifierPW_normal
- login_normal

#### 2. Boutons Tuile (90px height)

**Utilisation** : Menu administration (UC_AdminHome)

**Initialisation** :
```vb
UtilsButtons.InitLargeIconButton(btnNom)
```

**Propriétés** :
- Height : 90
- BackColor : `UITheme.ColorSauge`
- ForeColor : `Color.White`
- FlatStyle : `Flat`
- Font : Calibri 12pt Bold
- Tag : `nom_normal` (sans `.png`)
- Icône : 48x48 pixels dans `/Assets/Boutons_ico_48/`
- ImageAlign : `MiddleCenter`
- TextAlign : `MiddleCenter`

**Icônes disponibles** :
- utilisateurs_normal
- parametres_normal
- logs_normal
- sauvegarde_normal
- configurationDatabase_normal

#### 3. Boutons Menu Home (60px height)

**Utilisation** : Menu latéral dans Home

**Initialisation** :
```vb
UtilsButtons.InitHomeMenuButton(btnNom)
```

**Propriétés** :
- Height : 60
- BackColor : `UITheme.ColorBeige`
- ForeColor : `UITheme.ColorSaugeFonce`
- FlatStyle : `Flat`
- Font : Calibri 13pt Bold
- Tag : `nom` (sans suffixe)
- Icône : 48x48 pixels dans `/Assets/Boutons_Home/`
- États : `_normal`, `_selected`, `_disabled`

**Icônes disponibles** :
- accueil
- patients
- domaines
- agenda
- documents
- referentiels
- outils_admin

### États des boutons

Tous les boutons supportent trois états visuels :

| État | Suffixe | Utilisation |
|------|---------|-------------|
| Normal | `_normal` | État par défaut |
| Hover | `_hover` | Survol souris |
| Disabled | `_disabled` | Bouton désactivé |

**Exception** : Les boutons Home utilisent `_selected` au lieu de `_hover`.

---

## Panels

### Types de panels

#### 1. pnlForm (Panel principal)

**Utilisation** : Conteneur racine de toute Form ou UserControl

**Propriétés** :
- Dock : `Fill`
- BackgroundImage : `Fond_1000x800_Feuille4.png` (Forms) ou `Fond_1000x770_FeuilleCoupee1.png` (UC)
- BackgroundImageLayout : `Stretch`

#### 2. pnlTitre

**Utilisation** : Zone de titre en haut d'une Form/UC

**Propriétés** :
- Dock : `Top`
- Height : ~50-60
- BackColor : Transparent ou `UITheme.ColorSauge`

#### 3. pnlHeader (Home uniquement)

**Utilisation** : En-tête de la Form Home

**Propriétés** :
- Contient `picTitre` avec logo Althéa
- BackgroundImage : `Althea1_Transp_EnTeteHome.png`

#### 4. pnlCenter

**Utilisation** : Zone centrale de contenu

**Propriétés** :
- Dock : `Fill` ou `Top`
- BackColor : Transparent

#### 5. pnlActions

**Utilisation** : Zone des boutons d'action en bas

**Propriétés** :
- Dock : `Bottom`
- Height : ~60-80
- BackgroundImage : `Althea_Bandeau_Haut_Trans.png`
- BackgroundImageLayout : `Stretch`

#### 6. pnlMenu (Home uniquement)

**Utilisation** : Menu latéral gauche dans Home

**Propriétés** :
- Dock : `Left`
- Width : ~200
- BackColor : `UITheme.ColorBeige`

---

## Labels

### Types de labels

#### 1. lblTitreForm

**Utilisation** : Titre principal d'une Form/UC

**Propriétés** :
- Font : Calibri 14-16pt Bold
- ForeColor : `UITheme.ColorGrisTresFonce` ou `Color.White`
- AutoSize : False
- TextAlign : `MiddleCenter` ou `MiddleLeft`

#### 2. Labels de champs (lblNomChamp)

**Utilisation** : Libellés des champs de saisie

**Propriétés** :
- Font : Calibri 10pt
- ForeColor : `UITheme.ColorGrisFonce`
- AutoSize : True
- TextAlign : `MiddleLeft`

#### 3. lblContexte (Home)

**Utilisation** : Affichage du contexte actuel dans Home

**Propriétés** :
- Font : Calibri 11pt Bold
- ForeColor : `UITheme.ColorSaugeFonce`
- Dock : `Fill`
- TextAlign : `MiddleCenter`

#### 4. lblTop

**Utilisation** : Message informatif en haut d'une Form

**Propriétés** :
- Font : Calibri 10pt
- ForeColor : `UITheme.ColorGrisFonce`
- Dock : `Fill`
- TextAlign : `MiddleCenter`

---

## TextBox

### Configuration standard

**Propriétés** :
- Font : Calibri 10pt
- BackColor : `Color.White`
- ForeColor : `UITheme.ColorGrisTresFonce`
- BorderStyle : `FixedSingle`

### TextBox mot de passe

**Propriétés spéciales** :
- UseSystemPasswordChar : `True`
- PasswordChar : Pas utilisé (UseSystemPasswordChar suffit)

**Boutons "Voir"** : Affichage temporaire via `MouseDown` / `MouseUp` / `MouseLeave`

### TextBox lecture seule

**Propriétés** :
- ReadOnly : `True`
- BackColor : `UITheme.ColorGrisClair`
- TabStop : `False`

---

## ComboBox

### Configuration standard

**Propriétés** :
- Font : Calibri 10pt
- DropDownStyle : `DropDownList` (empêche la saisie manuelle)
- BackColor : `Color.White`
- ForeColor : `UITheme.ColorGrisTresFonce`

### Remplissage

```vb
' Enum
cboRole.DataSource = [Enum].GetValues(GetType(AppRole))

' Liste d'objets
cboRole.DisplayMember = "NomRole"
cboRole.ValueMember = "IdRole"
cboRole.DataSource = listeRoles
```

### Valeur par défaut

```vb
' Ajouter une entrée vide
cboRole.Items.Insert(0, "-- Tous --")
cboRole.SelectedIndex = 0
```

---

## CheckBox

### Configuration standard

**Propriétés** :
- Font : Calibri 10pt
- ForeColor : `UITheme.ColorGrisFonce`
- AutoSize : `True`
- CheckAlign : `MiddleLeft`
- TextAlign : `MiddleLeft`

### Utilisation

- États : `Checked` (True/False)
- Événement : `CheckedChanged`

---

## DateTimePicker

### Configuration standard

**Propriétés** :
- Font : Calibri 10pt
- Format : `Short` (date seule) ou `Custom`
- CustomFormat : `"dd/MM/yyyy"` ou `"dd/MM/yyyy HH:mm"`
- ShowCheckBox : `True` (pour dates optionnelles)
- ShowUpDown : `False` (utilise le calendrier)

### Initialisation

```vb
dtpDate.Format = DateTimePickerFormat.Custom
dtpDate.CustomFormat = "dd/MM/yyyy"
dtpDate.Value = DateTime.Now
```

---

## DataGridView

### Initialisation standard

```vb
UtilsDataGrid.InitDataGridBasique(dgvNom)
```

**Configuration appliquée** :
- ReadOnly : `True`
- AllowUserToAddRows : `False`
- AllowUserToDeleteRows : `False`
- SelectionMode : `FullRowSelect`
- MultiSelect : `False`
- AutoGenerateColumns : `False`
- RowHeadersVisible : `False`
- BackgroundColor : `Color.White`
- GridColor : `UITheme.ColorGrisMoyen`
- DefaultCellStyle : Font Calibri 9pt

### Colonnes typiques

#### Colonne d'état (icône)

```vb
Dim colEtat As New DataGridViewImageColumn()
colEtat.Name = "colEtat"
colEtat.HeaderText = "État"
colEtat.Width = 50
colEtat.Resizable = DataGridViewTriState.False
dgv.Columns.Add(colEtat)
```

**Gestion des icônes** :
```vb
Private Sub dgv_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) _
	Handles dgv.CellFormatting

	If e.ColumnIndex = dgv.Columns("colEtat").Index Then
		Dim row = dgv.Rows(e.RowIndex)
		Dim verrouille = CBool(row.Cells("colCompteVerrouille").Value)
		Dim actif = CBool(row.Cells("colActif").Value)

		' Priorité : verrouillé > actif > inactif
		If verrouille Then
			e.Value = UtilsIcons.IconLock(32)
		ElseIf actif Then
			e.Value = UtilsIcons.IconOK(32)
		Else
			e.Value = UtilsIcons.IconOFF(32)
		End If
	End If
End Sub
```

#### Colonne de date

```vb
colDate.DataPropertyName = "DateCreation"
colDate.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"
```

#### Colonne cachée (pour DataBinding)

```vb
colId.Visible = False
colId.DataPropertyName = "IdUtilisateur"
```

### Binding de données

```vb
dgv.AutoGenerateColumns = False
dgv.DataSource = listeUtilisateurs
```

---

## TableLayoutPanel

### Utilisation

Organiser des champs de saisie de manière alignée et responsive.

### Configuration standard

**Propriétés** :
- ColumnCount : 2 (libellés + champs)
- RowCount : Nombre de champs
- Dock : `Fill` ou `Top`
- AutoSize : `True`
- AutoSizeMode : `GrowAndShrink`

### Structure typique

| Colonne 1 (30%) | Colonne 2 (70%) |
|-----------------|-----------------|
| lblNom          | txtNom          |
| lblPrenom       | txtPrenom       |
| lblEmail        | txtEmail        |

```vb
tlp.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30.0F))
tlp.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 70.0F))
```

---

## StatusStrip

### Configuration standard

**Propriétés** :
- Dock : `Bottom`
- BackColor : `UITheme.ColorGrisClair`
- Font : Calibri 9pt

### Items

```vb
Dim stsLabelStatus As New ToolStripStatusLabel()
stsLabelStatus.Name = "stsLabelStatus"
stsLabelStatus.Spring = True
stsLabelStatus.TextAlign = ContentAlignment.MiddleLeft
stsStatus.Items.Add(stsLabelStatus)
```

### Utilisation

```vb
' Via contexte (UserControl)
_context.SetStatus("Message d'état")

' Direct (Form)
stsLabelStatus.Text = "Message d'état"
```

---

## ToolTip

### Configuration

```vb
ttMain.AutoPopDelay = 5000
ttMain.InitialDelay = 500
ttMain.ReshowDelay = 100
```

### Utilisation

```vb
' Via contexte (UserControl)
_context.SetToolTip(btnNom, "Description du bouton")

' Direct (Form)
ttMain.SetToolTip(btnNom, "Description du bouton")
```

---

## ErrorProvider

### Configuration

```vb
errProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink
errProvider.Icon = SystemIcons.Error
```

### Utilisation

```vb
' Afficher erreur
errProvider.SetError(txtNom, "Le nom est obligatoire")

' Effacer erreur
errProvider.SetError(txtNom, String.Empty)

' Effacer toutes les erreurs
errProvider.Clear()
```

---

## PictureBox

### Configuration standard

**Propriétés** :
- SizeMode : `StretchImage` ou `Zoom` ou `CenterImage`
- BackColor : `Transparent`

### Icônes d'état

```vb
picEtat.Image = UtilsIcons.IconOK(32)
picEtat.SizeMode = PictureBoxSizeMode.CenterImage
```

### Logo/titre

```vb
picTitre.Image = Image.FromFile("Assets\Splash\Althea1_Transp_EnTeteHome.png")
picTitre.SizeMode = PictureBoxSizeMode.StretchImage
```

---

## NumericUpDown

### Configuration standard

**Propriétés** :
- Font : Calibri 10pt
- TextAlign : `Right`
- DecimalPlaces : 0 (entiers) ou 2 (décimaux)
- Minimum / Maximum : Selon le contexte
- Increment : 1 ou 0.01

### Exemples

```vb
' Port réseau
nudPort.Minimum = 1
nudPort.Maximum = 65535
nudPort.Value = 3306

' Montant
nudMontant.DecimalPlaces = 2
nudMontant.Minimum = 0
nudMontant.Maximum = 999999
nudMontant.Increment = 0.01D
```

---

## Conventions de nommage

### Préfixes standards

| Contrôle | Préfixe | Exemple |
|----------|---------|---------|
| Button | btn | btnEnregistrer |
| Panel | pnl | pnlCenter |
| Label | lbl | lblNom |
| TextBox | txt | txtNom |
| ComboBox | cbo | cboRole |
| CheckBox | chk | chkActif |
| DateTimePicker | dtp | dtpDateNaissance |
| DataGridView | dgv | dgvUtilisateurs |
| TableLayoutPanel | tlp | tlpUtilisateur |
| StatusStrip | sts | stsStatus |
| ToolStripStatusLabel | stsLabel | stsLabelStatus |
| ToolTip | tt | ttMain |
| ErrorProvider | err | errProvider |
| PictureBox | pic | picTitre |
| NumericUpDown | nud | nudPort |
| DataGridViewColumn | col | colNom |

### Suffixes de colonnes DataGridView

Toujours préfixer par `col` :
- colIdUtilisateur
- colNomUtilisateur
- colActif
- colEtat

---

## Bonnes pratiques

### Initialisation

1. **Toujours** initialiser les boutons via `UtilsButtons`
2. **Toujours** initialiser les DataGridView via `UtilsDataGrid`
3. **Toujours** définir le `Tag` des boutons sans extension `.png`

### Contexte UI

Pour les UserControls implémentant `IContextAwareUserControl` :

```vb
Private _context As UserControlContext

Public Sub SetContext(context As UserControlContext) _
	Implements IContextAwareUserControl.SetContext
	_context = context
End Sub

' Utilisation
_context.SetHeader("Titre")
_context.SetStatus("Message")
_context.SetToolTip(btn, "Info")
_context.ClearErrors()
```

Pour les Forms implémentant `IContextAwareForm` :

```vb
Private _context As UserControlContext

Public Sub SetContext(context As UserControlContext) _
	Implements IContextAwareForm.SetContext
	_context = context
End Sub
```

### Validation

```vb
' Champs obligatoires
If String.IsNullOrWhiteSpace(txtNom.Text) Then
	errProvider.SetError(txtNom, "Le nom est obligatoire")
	Return False
End If

' Validation réussie
errProvider.SetError(txtNom, String.Empty)
```

### DialogChoix

**Toujours** utiliser `DialogChoix` au lieu de `MessageBox` :

```vb
' Information
DialogChoix.Information("Opération réussie")

' Confirmation
If DialogChoix.Confirmer("Voulez-vous continuer ?") = DialogResult.Yes Then
	' Action confirmée
End If

' Erreur
DialogChoix.Erreur("Une erreur s'est produite")
```

---

> **Contact** : ***Joëlle (Manou) - Les Artefacts de Manou***
>
> Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.
> - Site web P.Nguyen Duy: https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu`
>
> - GitHub privé: Althea https://github.com/AngeljoNG/Althea

[TOC]
