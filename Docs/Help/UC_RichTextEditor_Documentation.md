# UC_RichTextEditor - Éditeur de texte riche Althéa

## 📋 Vue d'ensemble

**UC_RichTextEditor** est un UserControl réutilisable qui fournit un éditeur de texte riche complet pour les notes patients, dossiers, consultations et autres besoins d'édition formatée dans l'application Althéa.

**Fichiers concernés :**
- `UI/Controls/UC_RichTextEditor.vb` - Logique métier
- `UI/Controls/UC_RichTextEditor.Designer.vb` - Interface visuelle
- `Utils/Helpers/RichTextEditorHelper.vb` - Fonctions utilitaires
- `UI/Forms/TestRichTextEditor.vb` - Form de test et validation

**Version :** V1.0  
**Date :** 16/05/2026  
**Auteur :** Joëlle (Manou) / Projet Althéa

---

## ⚡ Fonctionnalités

### ✅ Formatage de caractères
- **Gras** (Ctrl+B), **Italique** (Ctrl+I), **Souligné** (Ctrl+U), ~~Barré~~
- Couleur de texte (avec ColorDialog)
- Couleur de surbrillance / surligneur (avec ColorDialog)

### ✅ Police et taille
- Sélection de famille de police (9 polices courantes)
- Sélection de taille (de 8 à 72 points)
- Mise à jour automatique selon la sélection

### ✅ Formatage de paragraphe
- Alignement (gauche, centré, droite)
- Listes à puces
- Augmentation/diminution des retraits

### ✅ Actions
- Couper (Ctrl+X), Copier (Ctrl+C), Coller (Ctrl+V)
- Annuler (Ctrl+Z), Rétablir (Ctrl+Y)
- Effacer tout le formatage

### ✅ Insertion
- Date/heure courante (format paramétrable)

### ✅ Outils
- **Mise en page** (📄) - Configuration format papier (A4/A3/Letter...), marges, orientation (portrait/paysage)
- **Impression** (🖨 Ctrl+P) - Dialogue système pour choisir l'imprimante, pagination automatique, formatage RTF préservé
- **Export PDF** (📑) - Conversion RTF vers PDF avec Syncfusion, formatage préservé
- **Export Word** (📝) - Conversion RTF vers .docx avec Syncfusion, intégré au système documentaire Althéa

### ✅ Modes
- **Mode lecture seule** (`ReadOnlyMode`) - masque la toolbar automatiquement
- **Afficher/masquer toolbar** (`ShowToolbar`)

### ✅ Sauvegarde
- **RTF** : formatage complet préservé (pour affichage)
- **TXT** : texte brut (pour recherche full-text en base de données)

---

## 🔧 Utilisation

### 1. Ajouter le UserControl dans une Form ou un autre UserControl

**Via le Designer :**
1. Ouvrir le Designer de votre Form/UserControl
2. Glisser-déposer `UC_RichTextEditor` depuis la Toolbox (catégorie "Althea Controls")
3. Positionner et dimensionner selon vos besoins
4. Définir `Dock = Fill` pour occuper tout l'espace disponible

**Via le code :**
```vb
Dim editorNotes As New UC_RichTextEditor()
editorNotes.Dock = DockStyle.Fill
Me.Controls.Add(editorNotes)
```

---

### 2. Charger du contenu depuis la base de données

```vb
' Charger le contenu RTF depuis la base (colonne notes_rtf)
Dim rtfFromDB As String = NoteDAO.GetNoteRtf(patientId)
ucEditor.RtfContent = rtfFromDB

' Alternative : charger du texte brut si RTF non disponible
If String.IsNullOrEmpty(rtfFromDB) Then
	Dim txtFromDB As String = NoteDAO.GetNoteTxt(patientId)
	ucEditor.RtfContent = txtFromDB ' Le setter accepte aussi du texte brut
End If
```

---

### 3. Sauvegarder le contenu en base de données

```vb
' Extraire RTF et TXT pour sauvegarde
Dim rtfContent As String = ucEditor.RtfContent
Dim txtContent As String = ucEditor.TextContent

' Sauvegarder dans les 2 colonnes
NoteDAO.SaveNote(patientId, rtfContent, txtContent)
```

**Schéma base de données recommandé :**
```sql
CREATE TABLE notes (
	note_id INT PRIMARY KEY AUTO_INCREMENT,
	patient_id INT NOT NULL,
	notes_rtf TEXT,           -- Formatage RTF complet
	notes_txt TEXT,           -- Texte brut pour recherche full-text
	date_creation DATETIME,
	date_modification DATETIME,
	INDEX idx_patient (patient_id),
	FULLTEXT idx_search (notes_txt)
);
```

---

### 4. Détecter les modifications (événement ContentChanged)

```vb
' Dans le Load de votre Form/UserControl
AddHandler ucEditor.ContentChanged, AddressOf OnNoteModifiee

' Gestionnaire d'événement
Private Sub OnNoteModifiee(sender As Object, e As EventArgs)
	' Activer le bouton Enregistrer
	btnSauvegarder.Enabled = True
	lblStatus.Text = "Modifications non sauvegardées"
	lblStatus.ForeColor = Color.Red
End Sub
```

---

### 5. Mode lecture seule

```vb
' Activer le mode lecture seule (masque automatiquement la toolbar)
ucEditor.ReadOnlyMode = True

' Désactiver le mode lecture seule
ucEditor.ReadOnlyMode = False
```

---

### 6. Masquer/afficher la toolbar

```vb
' Masquer la toolbar sans activer le mode lecture seule
ucEditor.ShowToolbar = False

' Réafficher la toolbar
ucEditor.ShowToolbar = True
```

---

### 7. Intégration avec le contexte UI (IContextAwareUserControl)

```vb
' Le UserControl implémente IContextAwareUserControl
' Le contexte est injecté automatiquement par NavigationManager

' Si vous instanciez le contrôle manuellement, injectez le contexte :
If TypeOf ucEditor Is IContextAwareUserControl Then
	DirectCast(ucEditor, IContextAwareUserControl).SetContext(_context)
End If
```

---

## 📦 Propriétés publiques

| Propriété | Type | Get/Set | Description |
|-----------|------|---------|-------------|
| `RtfContent` | String | Get/Set | Contenu RTF complet (pour sauvegarde et chargement) |
| `TextContent` | String | Get only | Texte brut extrait (pour sauvegarde en colonne recherchable) |
| `ReadOnlyMode` | Boolean | Get/Set | Active/désactive le mode lecture seule (masque toolbar) |
| `ShowToolbar` | Boolean | Get/Set | Affiche/masque la barre d'outils |

---

## 🎯 Événements publics

| Événement | Description | Utilisation |
|-----------|-------------|-------------|
| `ContentChanged` | Déclenché à chaque modification du texte | Activer bouton Enregistrer |
| `ContentSaved` | Déclenché manuellement après sauvegarde | Confirmer persistance réussie |

**Exemple :**
```vb
AddHandler ucEditor.ContentChanged, Sub() btnSave.Enabled = True
AddHandler ucEditor.ContentSaved, Sub() lblStatus.Text = "Sauvegardé"
```

---

## 🖨️ Impression et mise en page

### Configuration de la mise en page

Avant d'imprimer, l'utilisateur peut configurer :
- **Format de papier** : A4, A3, Letter, Legal, etc.
- **Marges** : Haut, Bas, Gauche, Droite (en cm ou inches)
- **Orientation** : Portrait ou Paysage

**Via la toolbar** : Cliquer sur le bouton **📄 Mise en page**

**Via le code** :
```vb
' Ouvrir le dialogue de mise en page
RichTextEditorHelper.ConfigurerMiseEnPage()
```

Le dialogue système `PageSetupDialog` permet à l'utilisateur de :
- Visualiser les marges dans un aperçu
- Choisir le format de papier supporté par l'imprimante
- Basculer entre portrait et paysage
- Accéder aux paramètres de l'imprimante

### Impression avec dialogue système

**Via la toolbar** : Cliquer sur le bouton **🖨 Imprimer** ou `Ctrl+P`

**Via le code** :
```vb
' Lancer l'impression avec dialogue système
RichTextEditorHelper.Imprimer(rtbEditor, "Notes Althéa")

' OU afficher seulement l'aperçu (optionnel)
RichTextEditorHelper.AfficherApercu(rtbEditor, "Notes Althéa")
```

**Flux d'impression simplifié :**
1. **PrintDialog** s'ouvre → L'utilisateur choisit :
   - L'imprimante (liste des imprimantes installées)
   - Les propriétés du pilote (qualité, couleur/N&B, recto-verso...)
   - Les marges et format via le bouton "Propriétés"

2. **Impression directe** → Après validation :
   - Le document est immédiatement envoyé à l'imprimante
   - Le formatage RTF est préservé sur papier
   - Les marges, format et orientation sont respectés

**Si vous voulez un aperçu avant d'imprimer :**
- Utilisez le bouton **📄 Mise en page** ou appelez `AfficherApercu()` dans votre code
- L'aperçu affiche le document avec les marges/format configurés
- Vous pouvez imprimer depuis l'aperçu si le résultat vous convient

**Avantages :**
- ✅ Utilise le **driver système** de l'imprimante
- ✅ Respect du **formatage RTF** (gras, couleurs, alignement, puces)
- ✅ **Marges configurables** par l'utilisateur
- ✅ **Multi-formats** : A4, A3, Letter, Legal...
- ✅ **Impression directe** sans étape supplémentaire
- ✅ **Cache clear** pour éviter les pages blanches

---

## 📑 Export PDF avec Syncfusion

### Export rapide vers PDF

**Via la toolbar** : Cliquer sur le bouton **📑 Export PDF**

**Via le code** :
```vb
' Export avec dialogue de sauvegarde
RichTextEditorHelper.ExporterPDFAvecDialogue(rtbEditor, "Notes_Althea.pdf")

' OU export direct vers un chemin spécifique
Dim success As Boolean = RichTextEditorHelper.ExporterPDF(rtbEditor, "C:\Temp\MonDocument.pdf")
```

### Comment ça marche

1. **Extraction RTF** : Le contenu formaté est extrait du RichTextBox
2. **Conversion Syncfusion** : 
   - RTF → WordDocument (Syncfusion.DocIO)
   - WordDocument → PDF (Syncfusion.DocToPDFConverter)
3. **Sauvegarde** : Le PDF est enregistré à l'emplacement choisi

### Formatage préservé

✅ **Ce qui est préservé dans le PDF :**
- Gras, Italique, Souligné, Barré
- Couleurs de texte et de surbrillance
- Polices et tailles
- Alignement (gauche, centré, droite)
- Listes à puces
- Retraits de paragraphe
- Sauts de ligne et paragraphes

### Pré-requis

**Licence Syncfusion Community (gratuite) :**
- Inscrivez-vous sur [syncfusion.com/account/claim-license-key](https://www.syncfusion.com/account/claim-license-key)
- Copiez votre clé de licence
- Enregistrez-la dans `Program.vb` :

```vb
' Dans Program.vb, méthode Main()
SyncfusionLicenseProvider.RegisterLicense("VOTRE-CLE-ICI")
```

**Packages NuGet installés :**
- `Syncfusion.DocIO.WinForms` (v33.2.10)
- `Syncfusion.DocToPDFConverter.WinForms` (v33.2.10)
- `Syncfusion.Licensing` (v33.2.10)

### Cas d'usage typiques

**Export notes patient :**
```vb
' Lors de la fermeture d'une consultation
Dim nomFichier As String = $"Notes_Patient_{patientId}_{DateTime.Now:yyyyMMdd}.pdf"
RichTextEditorHelper.ExporterPDFAvecDialogue(ucEditor, nomFichier)
```

**Génération automatique de rapport :**
```vb
' Export vers un dossier spécifique sans dialogue
Dim cheminPDF As String = $"C:\Althea\Rapports\Rapport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
If RichTextEditorHelper.ExporterPDF(ucEditor, cheminPDF) Then
    ' Succès : ouvrir le PDF, envoyer par email, etc.
End If
```

**Avantages de Syncfusion :**
- ✅ **Conversion native RTF → PDF** (pas de parsing manuel)
- ✅ **Qualité professionnelle** (formatage fidèle)
- ✅ **Licence gratuite** pour usage non-commercial ou revenus < 1M USD
- ✅ **Intégration simple** (2 lignes de code)
- ✅ **Support complet** du formatage RTF avancé

---

## 📝 Export Word (.docx) avec Syncfusion

### Export rapide vers Word

**Via la toolbar** : Cliquer sur le bouton **📝 Export Word**

**Via le code** :
```vb
' Export avec dialogue de sauvegarde
RichTextEditorHelper.ExporterWordAvecDialogue(rtbEditor, "Notes_Althea.docx")

' OU export direct vers un chemin spécifique
Dim success As Boolean = RichTextEditorHelper.ExporterWord(rtbEditor, "C:\Temp\MonDocument.docx")
```

### Comment ça marche

1. **Extraction RTF** : Le contenu formaté est extrait du RichTextBox
2. **Conversion Syncfusion** : RTF → WordDocument → Sauvegarde DOCX (Syncfusion.DocIO)
3. **Fichier .docx** : Document Word compatible Office et Google Docs

### Formatage préservé

✅ **Ce qui est préservé dans le .docx :**
- Gras, Italique, Souligné, Barré
- Couleurs de texte et de surbrillance
- Polices et tailles
- Alignement (gauche, centré, droite)
- Listes à puces
- Retraits de paragraphe
- Sauts de ligne et paragraphes

### Intégration avec le système documentaire Althéa

L'export Word s'intègre parfaitement avec l'architecture documentaire du POC :

**📂 Architecture locale et cloud :**
```
Patients/
    {id_patient}/
        {id_dossier}/
            Documents/
                Notes_Consultation_20260517.docx  ← Export notes
                Ordonnance_20260517.docx          ← Export ordonnance
```

**☁️ Synchronisation Google Drive :**
- Les documents .docx exportés peuvent être placés dans les dossiers patients
- Upload automatique vers Google Drive (via `DocumentManager`)
- Édition possible avec Word (local) ou Google Docs (cloud)
- Conversion PDF automatique pour archivage

**🔁 Flows documentaires compatibles :**

| Flow | Description | Usage export notes |
|------|-------------|--------------------|
| **Flow 1** | Word local | Export → Ouvrir Word → Éditer → Upload Drive |
| **Flow 2** | Google Docs | Export → Upload Drive → Ouvrir Google Docs |
| **Flow 3** | Admission externe | Export vers dossier patient existant |

### Cas d'usage typiques

**Export notes consultation :**
```vb
' Après consultation, sauvegarder les notes en .docx
Dim nomFichier As String = $"Consultation_{patientNom}_{DateTime.Now:yyyyMMdd}.docx"
Dim cheminLocal As String = $"Patients\{patientId}\{dossierId}\Documents\{nomFichier}"

If RichTextEditorHelper.ExporterWord(ucEditor, cheminLocal) Then
    ' Upload vers Google Drive
    DocumentManager.UploadToGoogleDrive(cheminLocal, patientId, dossierId)

    ' Générer PDF pour archivage
    DocumentManager.ConvertToPDF(cheminLocal)
End If
```

**Export avec dialogue utilisateur :**
```vb
' Laisser l'utilisateur choisir l'emplacement
If RichTextEditorHelper.ExporterWordAvecDialogue(ucEditor, "Notes_Patient.docx") Then
    MessageBox.Show("Document exporté. Vous pouvez maintenant l'ouvrir avec Word ou le télécharger sur Drive.", "Succès")
End If
```

**Export automatique vers dossier patient :**
```vb
' Intégration directe avec le module Patients
Dim documentPath As String = PatientDocumentHelper.GetDocumentPath(patientId, dossierId, "Consultation", ".docx")
RichTextEditorHelper.ExporterWord(ucEditor, documentPath)
```

### Pré-requis

**Même licence et packages que PDF :**
- Licence Syncfusion Community (déjà enregistrée dans `Program.vb`)
- `Syncfusion.DocIO.WinForms` (v33.2.10)

**Avantages de l'export Word :**
- ✅ **Éditable** : Le document peut être modifié après export (contrairement au PDF)
- ✅ **Collaboration** : Compatible Word + Google Docs pour édition multi-utilisateurs
- ✅ **Archivage intelligent** : Le .docx est la source, le PDF est généré automatiquement
- ✅ **Intégration POC** : S'intègre directement avec le système documentaire existant (local + Drive)
- ✅ **Standard médical** : Format attendu pour dossiers patients, courriers, comptes-rendus

**Différence PDF vs Word :**

| Critère | PDF (📑) | Word (📝) |
|---------|----------|----------|
| **Édition** | ❌ Non éditable | ✅ Éditable |
| **Usage** | Archivage, envoi, impression | Collaboration, modification |
| **Compatibilité** | Universel (lecture seule) | Office + Google Docs |
| **Gestion Althéa** | Dérivé local uniquement | Source principale (POC) |

---

## ⌨️ Raccourcis clavier

| Raccourci | Action |
|-----------|--------|
| `Ctrl+B` | Gras |
| `Ctrl+I` | Italique |
| `Ctrl+U` | Souligné |
| `Ctrl+X` | Couper |
| `Ctrl+C` | Copier |
| `Ctrl+V` | Coller |
| `Ctrl+Z` | Annuler |
| `Ctrl+Y` | Rétablir |
| `Ctrl+F` | Rechercher (Phase 2) |
| `Ctrl+P` | Imprimer |

---

## 🎨 Apparence et thème

L'éditeur respecte le thème Althéa :
- **Toolbar** : `ColorSaugeClair` (178, 197, 186)
- **Boutons actifs** : `ColorSauge` (122, 155, 135)
- **Fond éditeur** : `ColorBeigeClair` (244, 239, 234)
- **Texte** : `ColorTexte` (74, 74, 74)

Les couleurs sont définies dans `UITheme.vb` et appliquées automatiquement.

---

## 🧪 Tester le UserControl

Utilisez la Form de test dédiée :
```vb
' Ouvrir la form de test
Dim testForm As New TestRichTextEditor()
testForm.ShowDialog()
```

**`TestRichTextEditor.vb`** permet de :
- Charger un contenu de test
- Simuler sauvegarde/chargement depuis base
- Tester modes ReadOnly et ShowToolbar
- Valider tous les boutons de formatage
- Tester impression
- Effacer le contenu

---

## 🔮 Fonctionnalités avancées (Phase 2)

Les fonctionnalités suivantes sont **préparées dans le Helper** mais peuvent être ajoutées plus tard :

### ❌ Dialogue Rechercher/Remplacer complet
- Les méthodes `Rechercher()`, `Remplacer()`, `RemplacerTout()` existent déjà
- Il suffit de créer une Form dédiée `SearchReplaceDialog.vb`
- Le bouton affiche actuellement un message "Phase 2"

### ❌ Numérotation automatique (1. 2. 3.)
- RichTextBox natif ne supporte pas la numérotation
- Nécessite une logique custom complexe
- À implémenter selon besoins utilisateur réels

### ❌ Liens hypertextes cliquables
- `DetectUrls = True` est activé (affichage en bleu souligné)
- Gestionnaire d'événement `LinkClicked` à ajouter si besoin

### ❌ Export PDF
- Nécessite une bibliothèque tierce (ex: iTextSharp, PDFsharp)
- À implémenter si besoin métier confirmé

---

## 🛠️ Maintenance et extension

### Ajouter un bouton à la toolbar

**1. Designer :**
```vb
' Dans UC_RichTextEditor.Designer.vb
Dim btnMaFonction As New ToolStripButton()
btnMaFonction.Text = "🎯"
btnMaFonction.ToolTipText = "Ma fonction"
toolStrip.Items.Add(btnMaFonction)
```

**2. Code-behind :**
```vb
' Dans UC_RichTextEditor.vb
Private Sub btnMaFonction_Click(sender As Object, e As EventArgs) Handles btnMaFonction.Click
	RichTextEditorHelper.MaNouvelleFonction(rtbEditor)
	rtbEditor.Focus()
End Sub
```

**3. Helper :**
```vb
' Dans RichTextEditorHelper.vb
Public Sub MaNouvelleFonction(rtb As RichTextBox)
	If rtb Is Nothing Then Exit Sub
	' Votre logique ici
End Sub
```

---

### Ajouter un raccourci clavier

```vb
' Dans rtbEditor_KeyDown (UC_RichTextEditor.vb)
Private Sub rtbEditor_KeyDown(sender As Object, e As KeyEventArgs) Handles rtbEditor.KeyDown
	If e.Control Then
		Select Case e.KeyCode
			Case Keys.M ' Ctrl+M
				RichTextEditorHelper.MaNouvelleFonction(rtbEditor)
				e.Handled = True
		End Select
	End If
End Sub
```

---

## 📝 Exemples d'intégration

### Exemple 1 : Écran Patient avec notes

```vb
Public Class UC_PatientDetails
	Implements IContextAwareUserControl

	Private Sub UC_PatientDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		' Charger les notes du patient
		Dim notes As Note = NoteDAO.GetByPatientId(_currentPatientId)
		ucEditorNotes.RtfContent = notes.NotesRtf

		' Détecter modifications
		AddHandler ucEditorNotes.ContentChanged, AddressOf OnNotesModifiees
	End Sub

	Private Sub OnNotesModifiees(sender As Object, e As EventArgs)
		btnEnregistrer.Enabled = True
	End Sub

	Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click
		Dim note As New Note() With {
			.PatientId = _currentPatientId,
			.NotesRtf = ucEditorNotes.RtfContent,
			.NotesTxt = ucEditorNotes.TextContent,
			.DateModification = DateTime.Now
		}

		If NoteDAO.Update(note) Then
			MessageBox.Show("Notes sauvegardées !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information)
			btnEnregistrer.Enabled = False
			RaiseEvent ContentSaved(Me, EventArgs.Empty)
		End If
	End Sub
End Class
```

---

### Exemple 2 : Mode lecture seule pour historique

```vb
Public Class UC_HistoriqueNotes

	Private Sub ChargerNote(noteId As Integer)
		Dim note As Note = NoteDAO.GetById(noteId)

		ucEditor.RtfContent = note.NotesRtf
		ucEditor.ReadOnlyMode = True ' Affichage seul, pas d'édition

		lblDateCreation.Text = $"Créé le {note.DateCreation:dd/MM/yyyy HH:mm}"
		lblAuteur.Text = $"Par {note.AuteurNom}"
	End Sub

End Class
```

---

### Exemple 3 : Recherche dans les notes

```vb
Public Class UC_RechercheNotes

	Private Sub btnRechercher_Click(sender As Object, e As EventArgs) Handles btnRechercher.Click
		Dim motCle As String = txtRecherche.Text

		' Recherche SQL full-text dans notes_txt
		Dim resultats As List(Of Note) = NoteDAO.SearchFullText(motCle)

		' Afficher les résultats
		dgvResultats.DataSource = resultats

		lblNbResultats.Text = $"{resultats.Count} note(s) trouvée(s)"
	End Sub

End Class
```

---

## 🚀 Intégration future dans Althéa

**Écrans concernés :**
- `UC_PatientDetails.vb` - Notes générales patient
- `UC_DossierMedical.vb` - Notes médicales
- `UC_Consultation.vb` - Compte-rendu consultation
- `UC_Ordonnance.vb` - Notes ordonnance
- `UC_DocumentsPatient.vb` - Notes documents

**Base de données :**
Ajouter les colonnes `notes_rtf` et `notes_txt` aux tables concernées :
- `patients`
- `dossiers_medicaux`
- `consultations`
- `ordonnances`

---

## 📚 Références

- **Helper** : `Utils/Helpers/RichTextEditorHelper.vb`
- **Standards commentaires** : `Docs/Standards-Commentaires.md`
- **Référence UI** : `Docs/Reference_UI_Controles.md`
- **Charte graphique** : `Docs/Charte_Graphique_et_Reference_UI.md`
- **Form de test** : `UI/Forms/TestRichTextEditor.vb`

---

## ✅ Validation

- ✅ Build réussi (zéro erreur)
- ✅ Tous les boutons de formatage testés
- ✅ Sauvegarde RTF+TXT validée
- ✅ Modes ReadOnly et ShowToolbar validés
- ✅ Impression avec aperçu fonctionnelle
- ✅ Raccourcis clavier opérationnels
- ✅ Intégration contexte UI validée
- ✅ Thème Althéa respecté

---

**Dernière mise à jour :** 16/05/2026  
**Statut :** ✅ Production-ready
