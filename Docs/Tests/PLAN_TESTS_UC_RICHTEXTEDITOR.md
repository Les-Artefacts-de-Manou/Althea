# 📋 PLAN DE TESTS COMPLET - UC_RICHTEXTEDITOR

**Module** : Éditeur de texte riche pour notes patients, dossiers et consultations  
**Version** : V1.0  
**Date** : 17/05/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa

---

## 🎯 Objectif des tests

Valider le bon fonctionnement complet du module `UC_RichTextEditor`, incluant :
- Affichage et initialisation du contrôle
- Fonctionnalités de formatage de texte (gras, italique, souligné, couleurs, polices, etc.)
- Actions presse-papiers (couper, copier, coller)
- Annuler/Rétablir
- Alignement et listes à puces
- Insertion de date/heure
- Impression avec aperçu
- Modes ReadOnly et ShowToolbar
- Gestion des événements ContentChanged et ContentSaved
- Persistance RTF + TXT
- Raccourcis clavier
- Respect du thème Althéa

---

## ✅ CHECKLIST GÉNÉRALE

### Prérequis

- [x] Application compilée sans erreur
- [x] `UC_RichTextEditor.vb` intégré au projet
- [x] `RichTextEditorHelper.vb` disponible
- [x] `TestRichTextEditor.vb` créée (form de test provisoire)
- [x] Bouton provisoire ajouté dans `Home.vb` pour lancer la form de test
- [x] Thème Althéa (UITheme.vb) configuré
- [x] Assets toolbar (glyphes Unicode) affichés correctement

---

## 📂 SECTION 1 : CHARGEMENT & INITIALISATION

### 1.1 - Chargement initial du UserControl

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.1.1 | Ouvrir la form de test `TestRichTextEditor` | La form se charge sans erreur | - UC_RichTextEditor visible<br>- Toolbar affichée<br>- RichTextBox vide ou avec contenu de test<br>- Pas d'exception | ☐<br />OK |
| 1.1.2 | Vérifier l'affichage de la toolbar | Toolbar complète visible | - 31 boutons/contrôles affichés<br>- Séparateurs visibles<br>- ComboBox polices et tailles remplies<br>- Couleur fond toolbar = ColorSaugeClair (178, 197, 186) | OK<br />Peut être un peu petit |
| 1.1.3 | Vérifier les tooltips | Tous les tooltips sont présents | - Survoler chaque bouton<br>- Tooltip descriptif affiché (ex: "Gras (Ctrl+B)") | ☐OK |
| 1.1.4 | Vérifier l'état initial des boutons | Boutons dans l'état correct | - Tous les boutons Enabled<br>- Aucun bouton enfoncé (sauf si texte sélectionné) | ☐ OK |
| 1.1.5 | Vérifier le RichTextBox | RichTextBox correctement configuré | - Fond = ColorBeigeClair (244, 239, 234)<br>- AcceptsTab = True<br>- DetectUrls = True<br>- ScrollBars = Vertical<br>- WordWrap = True | ☐ OK<br />Ajouter une petite marge dans la saisie de texte : Il est trop collé aux côtés |

### 1.2 - Initialisation des listes déroulantes

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 1.2.1 | Vérifier la liste des polices (cmbFontFamily) | 9 polices disponibles | - Calibri, Arial, Times New Roman, Verdana, Georgia, Courier New, Tahoma, Comic Sans MS, Trebuchet MS<br>- Police par défaut = Calibri | ☐<br />OK |
| 1.2.2 | Vérifier la liste des tailles (cmbFontSize) | 14 tailles disponibles | - 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 36, 48, 72<br>- Taille par défaut = 11 | ☐<br />OK |
| 1.2.3 | Sélectionner une police différente | Police appliquée au texte | - Texte sélectionné prend la nouvelle police<br>- Pas d'erreur | ☐<br />OK |
| 1.2.4 | Sélectionner une taille différente | Taille appliquée au texte | - Texte sélectionné prend la nouvelle taille<br>- Pas d'erreur | ☐<br />OK |

---

## 📂 SECTION 2 : FORMATAGE DE CARACTÈRES

### 2.1 - Gras, Italique, Souligné, Barré

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.1.1 | Sélectionner du texte et cliquer sur **Gras** (B) | Texte devient gras | - Texte sélectionné en gras<br>- Bouton Gras enfoncé (BackColor = ColorSauge)<br>- Raccourci Ctrl+B fonctionne | OK |
| 2.1.2 | Cliquer à nouveau sur **Gras** | Gras retiré | - Texte redevient normal<br>- Bouton Gras relâché | ☐<br />OK |
| 2.1.3 | Sélectionner du texte et cliquer sur **Italique** (I) | Texte devient italique | - Texte sélectionné en italique<br>- Bouton Italique enfoncé<br>- Raccourci Ctrl+I fonctionne | ☐<br />OK |
| 2.1.4 | Cliquer à nouveau sur **Italique** | Italique retiré | - Texte redevient normal<br>- Bouton Italique relâché | ☐<br />OK |
| 2.1.5 | Sélectionner du texte et cliquer sur **Souligné** (U) | Texte devient souligné | - Texte sélectionné souligné<br>- Bouton Souligné enfoncé<br>- Raccourci Ctrl+U fonctionne | ☐<br />OK |
| 2.1.6 | Cliquer à nouveau sur **Souligné** | Soulignement retiré | - Texte redevient normal<br>- Bouton Souligné relâché | ☐<br />OK |
| 2.1.7 | Sélectionner du texte et cliquer sur **Barré** (S) | Texte devient barré | - Texte sélectionné barré<br>- Bouton Barré enfoncé | ☐<br />OK |
| 2.1.8 | Cliquer à nouveau sur **Barré** | Barré retiré | - Texte redevient normal<br>- Bouton Barré relâché | ☐<br />OK |
| 2.1.9 | Appliquer plusieurs formats simultanément | Tous les formats appliqués | - Texte gras + italique + souligné<br>- Plusieurs boutons enfoncés | ☐<br />OK |

### 2.2 - Couleurs de texte et surbrillance

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 2.2.1 | Sélectionner du texte et cliquer sur **Couleur texte** | ColorDialog s'ouvre | - Dialogue de sélection de couleur affiché | ☐<br />OK |
| 2.2.2 | Choisir une couleur et valider | Couleur appliquée au texte | - Texte sélectionné prend la couleur choisie<br>- Pas d'erreur | ☐<br />OK |
| 2.2.3 | Annuler le ColorDialog | Couleur non modifiée | - Texte conserve sa couleur d'origine | ☐<br />OK |
| 2.2.4 | Sélectionner du texte et cliquer sur **Couleur surbrillance** | ColorDialog s'ouvre | - Dialogue de sélection de couleur affiché | ☐<br />OK |
| 2.2.5 | Choisir une couleur et valider | Surbrillance appliquée au texte | - Texte sélectionné avec fond coloré<br>- Pas d'erreur | ☐<br />OK |
| 2.2.6 | Annuler le ColorDialog | Surbrillance non modifiée | - Texte conserve son fond d'origine | ☐<br />OK |

---

## 📂 SECTION 3 : PRESSE-PAPIERS

### 3.1 - Couper, Copier, Coller

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 3.1.1 | Sélectionner du texte et cliquer sur **Couper** (✂) | Texte coupé | - Texte disparaît de l'éditeur<br>- Texte disponible dans le presse-papiers<br>- Raccourci Ctrl+X fonctionne | ☐<br />OK |
| 3.1.2 | Cliquer sur **Coller** (📄) | Texte collé | - Texte réapparaît à l'emplacement du curseur<br>- Formatage préservé<br>- Raccourci Ctrl+V fonctionne | ☐<br />OK |
| 3.1.3 | Sélectionner du texte et cliquer sur **Copier** (📋) | Texte copié | - Texte reste dans l'éditeur<br>- Texte disponible dans le presse-papiers<br>- Raccourci Ctrl+C fonctionne | ☐<br />OK |
| 3.1.4 | Cliquer sur **Coller** | Texte collé | - Texte dupliqué à l'emplacement du curseur<br>- Formatage préservé | ☐<br />OK |
| 3.1.5 | Coller du texte depuis une application externe | Texte collé | - Texte brut ou RTF accepté<br>- Formatage préservé si RTF<br>- Pas d'erreur | ☐<br />Depuis le Bloc-notes c'est correct. Mais depuis OneNote, par exemple cela se copie comme une image et on ne peut rien changer (mais c peu-être inhérent à cette appli). Depuis Word c'est OK avec ou sans formatage. |

---

## 📂 SECTION 4 : ANNULER / RÉTABLIR

### 4.1 - Annuler et Rétablir

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 4.1.1 | Saisir du texte et cliquer sur **Annuler** (↶) | Dernière action annulée | - Texte saisi disparaît<br>- Raccourci Ctrl+Z fonctionne | ☐<br />OK |
| 4.1.2 | Cliquer sur **Rétablir** (↷) | Action rétablie | - Texte réapparaît<br>- Raccourci Ctrl+Y fonctionne | ☐<br />OK |
| 4.1.3 | Appliquer formatage et annuler | Formatage annulé | - Texte revient à l'état précédent | ☐<br />OK |
| 4.1.4 | Annuler plusieurs fois | Historique complet annulé | - Chaque clic annule une action<br>- Retour à l'état initial possible | ☐<br /><br />OK |
| 4.1.5 | Rétablir plusieurs fois | Historique complet rétabli | - Chaque clic rétablit une action<br>- Retour à l'état final possible | ☐<br />OK |

---

## 📂 SECTION 5 : FORMATAGE DE PARAGRAPHE

### 5.1 - Alignement

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.1.1 | Positionner curseur dans un paragraphe et cliquer sur **Aligner à gauche** | Paragraphe aligné à gauche | - Texte aligné à gauche<br>- Bouton Gauche enfoncé | ☐<br />OK |
| 5.1.2 | Cliquer sur **Centrer** | Paragraphe centré | - Texte centré<br>- Bouton Centrer enfoncé<br>- Bouton Gauche relâché | ☐<br />OK |
| 5.1.3 | Cliquer sur **Aligner à droite** | Paragraphe aligné à droite | - Texte aligné à droite<br>- Bouton Droite enfoncé<br>- Bouton Centrer relâché | ☐<br />OK |
| 5.1.4 | Sélectionner plusieurs paragraphes et aligner | Tous les paragraphes alignés | - Alignement appliqué à tous les paragraphes sélectionnés | ☐<br />OK |

### 5.2 - Listes à puces

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.2.1 | Positionner curseur dans un paragraphe et cliquer sur **Puces** (•) | Puce ajoutée | - Caractère • ajouté au début de la ligne<br>- Retrait appliqué<br>- Bouton Puces enfoncé | ☐<br />OK |
| 5.2.2 | Appuyer sur Entrée | Nouvelle puce créée | - Nouvelle ligne avec puce | ☐<br />OK |
| 5.2.3 | Cliquer à nouveau sur **Puces** | Puces retirées | - Caractère • supprimé<br>- Retrait retiré<br>- Bouton Puces relâché | ☐<br />OK |
| 5.2.4 | Sélectionner plusieurs lignes et cliquer sur **Puces** | Puces ajoutées à toutes les lignes | - Chaque ligne commence par •<br>- Retraits appliqués | ☐<br />OK |

### 5.3 - Retraits

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 5.3.1 | Positionner curseur et cliquer sur **Augmenter retrait** (→) | Retrait augmenté | - Paragraphe décalé vers la droite<br>- Marge gauche augmentée | ☐<br />OK |
| 5.3.2 | Cliquer plusieurs fois sur **Augmenter retrait** | Retrait augmenté progressivement | - Chaque clic décale de 20 pixels supplémentaires | ☐<br />OK |
| 5.3.3 | Cliquer sur **Diminuer retrait** (←) | Retrait diminué | - Paragraphe revient vers la gauche<br>- Marge gauche diminuée | ☐<br />OK |
| 5.3.4 | Diminuer jusqu'à 0 | Retrait minimum atteint | - Paragraphe aligné à gauche sans retrait<br>- Clic supplémentaire sans effet | ☐<br />OK |

---

## 📂 SECTION 6 : EFFACER FORMATAGE

### 6.1 - Effacer tout le formatage

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 6.1.1 | Sélectionner du texte formaté et cliquer sur **Effacer formatage** (🗑) | Formatage supprimé | - Texte revient en police/taille par défaut (Calibri 11)<br>- Gras, italique, souligné supprimés<br>- Couleur revient à noir<br>- Surbrillance supprimée | ☐<br />OK |
| 6.1.2 | Sélectionner plusieurs paragraphes formatés et effacer | Tout le formatage supprimé | - Tous les paragraphes en texte brut<br>- Alignement revient à gauche<br>- Puces supprimées<br>- Retraits supprimés | ☐<br />OK |
| 6.1.3 | Effacer formatage sans sélection | Tout le texte affecté | - Si rien sélectionné, comportement dépend de l'implémentation (à vérifier) | ☐<br />OK |

---

## 📂 SECTION 7 : INSERTION

### 7.1 - Insertion de date et heure

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 7.1.1 | Positionner curseur et cliquer sur **Insérer date/heure** (📅) | Date et heure insérées | - Format : "dd/MM/yyyy HH:mm"<br>- Date et heure courantes<br>- Texte inséré à l'emplacement du curseur | ☐<br />OK |
| 7.1.2 | Insérer date/heure avec texte sélectionné | Date remplace la sélection | - Texte sélectionné supprimé<br>- Date/heure insérée à sa place | ☐<br />OK |

---

## 📂 SECTION 8 : IMPRESSION

### 8.1 - Configuration de la mise en page

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.1.1 | Cliquer sur **Mise en page** (📄) | PageSetupDialog s'ouvre | - Dialogue système Windows affiché<br>- Onglets Papier, Marges, Imprimante disponibles<br>- Configuration actuelle affichée | ☐ |
| 8.1.2 | Configurer les marges (ex: 2cm partout) | Marges enregistrées | - Valeurs modifiables<br>- Validation OK ferme le dialogue<br>- Paramètres mémorisés pour impression suivante | ☐ |
| 8.1.3 | Changer le format de papier (A4 → A3 → Letter) | Format appliqué | - Liste des formats disponibles<br>- Sélection enregistrée<br>- Aperçu mis à jour si on imprime après | ☐ |
| 8.1.4 | Changer l'orientation (Portrait ↔ Paysage) | Orientation appliquée | - Bascule entre Portrait/Paysage<br>- Prévisualisation dans le dialogue<br>- Impression respecte l'orientation | ☐ |
| 8.1.5 | Annuler le dialogue | Paramètres non modifiés | - Clic sur Annuler<br>- Paramètres précédents conservés | ☐ |

### 8.2 - Impression avec dialogue système et aperçu

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.2.1 | Cliquer sur **Imprimer** (🖨) ou Ctrl+P | PrintDialog s'ouvre | - Dialogue système Windows affiché<br>- Liste des imprimantes disponibles<br>- Imprimante par défaut sélectionnée<br>- Raccourci Ctrl+P fonctionne | ☐ |
| 8.2.2 | Sélectionner une autre imprimante | Imprimante changée | - Liste déroulante fonctionnelle<br>- Imprimante sélectionnée mémorisée<br>- Bouton "Propriétés" accessible | ☐ |
| 8.2.3 | Cliquer sur "Propriétés" dans PrintDialog | Dialogue pilote imprimante s'ouvre | - Configuration spécifique au driver<br>- Qualité, couleur/N&B, recto-verso, etc.<br>- Paramètres sauvegardés si OK | ☐ |
| 8.2.4 | Valider le PrintDialog | PrintPreviewDialog s'ouvre | - Aperçu avant impression affiché<br>- Contenu RTF visible avec formatage<br>- Pagination automatique<br>- Marges et format respectés | ☐ |
| 8.2.5 | Vérifier l'aperçu | Formatage préservé | - Gras, italique, couleurs visibles<br>- Alignement respecté<br>- Puces affichées correctement<br>- Marges configurées appliquées<br>- Format de page correct (A4/A3/Letter) | ☐ |
| 8.2.6 | Cliquer sur Imprimer dans le preview | Document envoyé à l'imprimante | - Impression physique réelle<br>- Formatage RTF préservé sur papier<br>- Pas de page blanche<br>- Texte complet imprimé | ☐ |
| 8.2.7 | Annuler le PrintDialog initial | Aucune impression | - Dialogue fermé<br>- Pas d'aperçu affiché<br>- Retour à l'éditeur | ☐ |
| 8.2.8 | Annuler le PrintPreviewDialog | Dialogue fermé | - Retour à l'éditeur<br>- Pas de modification du contenu<br>- Paramètres d'impression conservés | ☐ |
| 8.2.9 | Imprimer un document vide | Message ou aperçu vide | - Message "Document vide" affiché<br>- OU aperçu page vide<br>- Pas d'erreur | ☐ |

### 8.3 - Cohérence marges/format entre configuration et impression

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 8.3.1 | Configurer marges 3cm, imprimer ensuite | Marges de 3cm sur papier | - Texte commence à 3cm des bords<br>- Marges visibles dans l'aperçu<br>- Impression physique identique à l'aperçu | ☐ |
| 8.3.2 | Configurer format A3, imprimer | Impression sur A3 | - Aperçu montre format A3<br>- Plus de texte par page qu'en A4<br>- Si imprimante A3 : sortie A3<br>- Sinon : message d'erreur ou mise à l'échelle | ☐ |
| 8.3.3 | Configurer paysage, imprimer | Impression paysage | - Aperçu en orientation paysage<br>- Lignes plus longues<br>- Impression physique en paysage | ☐ |
| 8.3.4 | Changer paramètres entre deux impressions | Nouveaux paramètres appliqués | - Première impression avec config A<br>- Modifier config → B<br>- Deuxième impression utilise config B | ☐ |

**Remarques utilisateur** :
- ✅ **Corrigé V1.3** : Preview affiche maintenant le bon texte formaté
- ✅ **Corrigé V1.3** : Impression physique fonctionne (cache clear ajouté)
- ✅ **Amélioré V1.3** : Ajout PrintDialog système pour choisir l'imprimante
- ✅ **Amélioré V1.3** : Ajout PageSetupDialog pour configurer marges/format/orientation
- ✅ **Amélioré V1.3** : Bouton "Mise en page" 📄 ajouté dans la toolbar

---

## 📂 SECTION 9 : RECHERCHER / REMPLACER

### 9.1 - Rechercher (Phase 2 - préparé)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 9.1.1 | Cliquer sur **Rechercher** (🔍) ou Ctrl+F | Message "Fonctionnalité Phase 2" | - MessageBox informatif affiché<br>- Raccourci Ctrl+F fonctionne<br>- Note : fonctionnalité non implémentée en V1 | ☐<br />Pas encore implémenté |
| 9.1.2 | Vérifier que les méthodes existent dans le Helper | Méthodes présentes | - `RichTextEditorHelper.Rechercher()`<br>- `RichTextEditorHelper.Remplacer()`<br>- `RichTextEditorHelper.RemplacerTout()` | ☐ |

---

## 📂 SECTION 10 : MODES

### 10.1 - Mode lecture seule (ReadOnlyMode)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 10.1.1 | Définir `ucEditor.ReadOnlyMode = True` | Éditeur en lecture seule | - RichTextBox.ReadOnly = True<br>- Toolbar masquée automatiquement<br>- Texte non éditable<br>- Sélection de texte possible<br>- Copie possible | ☐<br />OK |
| 10.1.2 | Tenter de saisir du texte en mode ReadOnly | Aucune saisie possible | - Curseur ne clignote pas<br>- Saisie clavier ignorée | ☐<br />OK |
| 10.1.3 | Définir `ucEditor.ReadOnlyMode = False` | Éditeur éditable | - RichTextBox.ReadOnly = False<br>- Toolbar réaffichée<br>- Texte éditable | ☐<br />OK |

### 10.2 - Afficher/masquer toolbar (ShowToolbar)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 10.2.1 | Définir `ucEditor.ShowToolbar = False` | Toolbar masquée | - ToolStrip.Visible = False<br>- Éditeur reste éditable (si ReadOnlyMode = False)<br>- Plus d'espace pour le RichTextBox | ☐<br />OK |
| 10.2.2 | Définir `ucEditor.ShowToolbar = True` | Toolbar affichée | - ToolStrip.Visible = True<br>- Boutons de formatage accessibles | ☐<br />OK |
| 10.2.3 | Masquer toolbar en mode édition | Toolbar masquée, édition possible | - Texte éditable<br>- Formatage possible via raccourcis clavier<br>- Pas de boutons visuels | OK |

---

## 📂 SECTION 11 : PERSISTANCE RTF + TXT

### 11.1 - Propriété RtfContent (Get/Set) 

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 11.1.1 | Saisir du texte formaté et lire `ucEditor.RtfContent` | RTF extrait correctement | - Chaîne RTF valide retournée<br>- Commence par `{\rtf1`<br>- Contient les codes de formatage | ☐<br /><br />OK |
| 11.1.2 | Définir `ucEditor.RtfContent = rtfString` | Contenu chargé dans l'éditeur | - Texte affiché avec formatage<br>- Gras, couleurs, alignement préservés | ☐<br /><br />OK |
| 11.1.3 | Charger RTF invalide ou texte brut | Texte brut affiché | - Pas d'exception<br>- Texte affiché sans formatage | ☐<br /><br />OK |
| 11.1.4 | Charger contenu vide (`String.Empty`) | Éditeur vide | - RichTextBox.Text = ""<br>- Pas d'erreur | ☐<br /><br />OK |

### 11.2 - Propriété TextContent (Get only)

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 11.2.1 | Saisir du texte formaté et lire `ucEditor.TextContent` | Texte brut extrait | - Chaîne texte sans formatage<br>- Pas de codes RTF<br>- Sauts de ligne préservés | ☐<br />OK |
| 11.2.2 | Comparer longueur RtfContent vs TextContent | TextContent plus court | - RtfContent contient codes RTF (plus long)<br>- TextContent contient uniquement texte lisible | ☐<br />OK |
| 11.2.3 | Extraire TextContent d'un éditeur vide | Chaîne vide | - TextContent = ""<br>- Pas d'erreur | ☐<br />OK |

### 11.3 - Simulation sauvegarde/chargement BDD

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 11.3.1 | Utiliser la form de test `TestRichTextEditor` | Boutons Sauvegarder/Charger fonctionnels | - Cliquer "Sauvegarder" affiche RTF et TXT extraits<br>- Cliquer "Charger" charge contenu simulé<br>- Pas d'erreur | ☐<br />OK |
| 11.3.2 | Sauvegarder puis charger | Contenu restauré identique | - Formatage préservé après cycle sauvegarde/chargement<br>- RTF → DB (simulé) → RTF fonctionne | ☐<br />OK |

---

## 📂 SECTION 12 : ÉVÉNEMENTS

### 12.1 - Événement ContentChanged

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 12.1.1 | Connecter un handler à `ContentChanged` | Événement déclenché à chaque modification | - `AddHandler ucEditor.ContentChanged, AddressOf OnChanged`<br>- Handler appelé après saisie texte<br>- Handler appelé après formatage | ☐ |
| 12.1.2 | Saisir du texte | Événement déclenché | - Handler exécuté<br>- Peut activer bouton Enregistrer, etc. | ☐ |
| 12.1.3 | Charger contenu via RtfContent | Événement NON déclenché | - Flag `_isLoading` empêche déclenchement<br>- Évite faux positifs lors du chargement initial | ☐ |

### 12.2 - Événement ContentSaved

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 12.2.1 | Déclencher manuellement `RaiseEvent ContentSaved` | Événement déclenché | - Handler exécuté<br>- Peut mettre à jour statut "Sauvegardé" | ☐ |
| 12.2.2 | Utiliser dans un workflow sauvegarde | Confirmation visuelle | - Appeler après succès de sauvegarde DB<br>- Afficher message "Enregistré" | ☐ |

---

## 📂 SECTION 13 : RACCOURCIS CLAVIER

### 13.1 - Raccourcis de formatage

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 13.1.1 | Appuyer sur **Ctrl+B** | Gras appliqué/retiré | - Équivalent au bouton Gras | ☐OK |
| 13.1.2 | Appuyer sur **Ctrl+I** | Italique appliqué/retiré | - Équivalent au bouton Italique | ☐OK |
| 13.1.3 | Appuyer sur **Ctrl+U** | Souligné appliqué/retiré | - Équivalent au bouton Souligné | ☐OK |

### 13.2 - Raccourcis presse-papiers

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 13.2.1 | Appuyer sur **Ctrl+X** | Texte coupé | - Équivalent au bouton Couper | ☐OK |
| 13.2.2 | Appuyer sur **Ctrl+C** | Texte copié | - Équivalent au bouton Copier | ☐OK |
| 13.2.3 | Appuyer sur **Ctrl+V** | Texte collé | - Équivalent au bouton Coller | ☐OK |

### 13.3 - Raccourcis annuler/rétablir

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 13.3.1 | Appuyer sur **Ctrl+Z** | Dernière action annulée | - Équivalent au bouton Annuler | ☐OK |
| 13.3.2 | Appuyer sur **Ctrl+Y** | Action rétablie | - Équivalent au bouton Rétablir | ☐OK |

### 13.4 - Raccourcis outils

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 13.4.1 | Appuyer sur **Ctrl+F** | Rechercher activé (Phase 2) | - Équivalent au bouton Rechercher<br>- Message "Phase 2" affiché | ☐OK |
| 13.4.2 | Appuyer sur **Ctrl+P** | Aperçu avant impression | - Équivalent au bouton Imprimer<br>- PrintPreviewDialog ouvert | ☐OK |

---

## 📂 SECTION 14 : INTÉGRATION CONTEXTE UI

### 14.1 - IContextAwareUserControl

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 14.1.1 | Vérifier que UC_RichTextEditor implémente l'interface | Interface implémentée | - `Implements IContextAwareUserControl`<br>- Méthode `SetContext()` présente | ☐ |
| 14.1.2 | Injecter un contexte via `SetContext()` | Contexte stocké | - Variable `_context` définie<br>- Pas d'erreur | ☐ |
| 14.1.3 | Intégrer dans un parent avec NavigationManager | Contexte injecté automatiquement | - NavigationManager appelle `SetContext()`<br>- Contexte disponible dans le UC | ☐ |

---

## 📂 SECTION 15 : THÈME ALTHÉA

### 15.1 - Respect de la charte graphique

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 15.1.1 | Vérifier couleur toolbar | ColorSaugeClair appliqué | - Toolbar.BackColor = RGB(178, 197, 186) | ☐ OK |
| 15.1.2 | Vérifier couleur boutons actifs | ColorSauge appliqué | - Bouton enfoncé (Gras actif) : BackColor = RGB(122, 155, 135) | ☐OK |
| 15.1.3 | Vérifier couleur fond éditeur | ColorBeigeClair appliqué | - RichTextBox.BackColor = RGB(244, 239, 234) | ☐OK |
| 15.1.4 | Vérifier couleur texte par défaut | ColorTexte appliqué | - RichTextBox.ForeColor = RGB(74, 74, 74) | ☐OK |
| 15.1.5 | Vérifier police par défaut | Calibri 11 | - RichTextBox.Font = Calibri, 11pt | ☐OK |

---

## 📂 SECTION 16 : GESTION DES ERREURS

### 16.1 - Robustesse

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 16.1.1 | Charger RTF corrompu | Texte brut affiché sans crash | - Try/Catch dans `ChargerContenu()`<br>- Fallback vers texte brut<br>- Pas d'exception non gérée | ☐ |
| 16.1.2 | Appeler méthodes avec RichTextBox Nothing | Méthodes sortent proprement | - `If rtb Is Nothing Then Exit Sub` dans Helper<br>- Pas d'erreur NullReference | ☐ |
| 16.1.3 | Annuler ColorDialog | Aucun changement appliqué | - Vérification `If result = DialogResult.OK` dans les handlers<br>- Couleur non modifiée | ☐ |
| 16.1.4 | Tenter d'imprimer sans imprimante | Message d'erreur clair | - Exception PrintDocument gérée<br>- MessageBox explicatif | ☐ |

---

## 📂 SECTION 17 : PERFORMANCE & STABILITÉ

### 17.1 - Performance

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 17.1.1 | Charger document RTF volumineux (> 10 Ko) | Chargement rapide (< 1 seconde) | - Pas de freeze UI<br>- Texte affiché correctement | ☐ |
| 17.1.2 | Appliquer formatage sur texte long (> 1000 lignes) | Réponse immédiate | - Pas de lag perceptible<br>- Formatage appliqué instantanément | ☐ |
| 17.1.3 | Annuler/Rétablir sur historique long (> 50 actions) | Historique fonctionnel | - Chaque Undo/Redo fonctionne<br>- Pas de dégradation de performance | ☐ |

### 17.2 - Stabilité

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 17.2.1 | Ouvrir/fermer la form de test 10 fois | Aucune fuite mémoire | - Pas d'augmentation anormale de RAM<br>- Pas d'exception | ☐ |
| 17.2.2 | Modifier rapidement texte et formatage | Pas de crash | - Saisie rapide + clics boutons multiples<br>- Application stable | ☐ |
| 17.2.3 | Copier/coller du contenu externe (Word, navigateur) | Contenu collé correctement | - RTF ou texte brut accepté<br>- Formatage préservé si possible<br>- Pas d'erreur | ☐ |

---

## 📂 SECTION 18 : INTÉGRATION FUTURE (Préparation)

### 18.1 - Préparation base de données

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 18.1.1 | Vérifier schéma SQL recommandé | Schéma documenté | - Voir `UC_RichTextEditor_Documentation.md`<br>- Colonnes `notes_rtf TEXT` et `notes_txt TEXT`<br>- Index FULLTEXT sur `notes_txt` | ☐ |
| 18.1.2 | Préparer DAO pour Notes | Structure prévue | - Méthodes `GetNoteRtf()`, `SaveNote(rtf, txt)` à implémenter<br>- Documentation présente | ☐ |

### 18.2 - Intégration dans écrans

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 18.2.1 | Identifier écrans concernés | Liste documentée | - UC_PatientDetails<br>- UC_DossierMedical<br>- UC_Consultation<br>- UC_Ordonnance<br>- Voir documentation | ☐ |
| 18.2.2 | Plan d'intégration défini | Étapes claires | - Ajouter UC_RichTextEditor au Designer<br>- Connecter événements<br>- Implémenter sauvegarde/chargement<br>- Documentation complète | ☐ |

---

## 📂 SECTION 19 : EXPORT PDF (Phase 2 - À implémenter)

### 19.1 - Préparation export PDF

| # | Test | Résultat attendu | Vérifications | ✅ |
|---|------|------------------|---------------|---|
| 19.1.1 | Choisir bibliothèque PDF | Décision prise | - Syncfusion (POC existant)<br>- ou iTextSharp<br>- ou PDFsharp<br>- Documentation consultée | ☐ |
| 19.1.2 | Installer package NuGet | Package installé | - Syncfusion.DocIO / Syncfusion.Pdf<br>- Pas de conflit de dépendances | ☐ |
| 19.1.3 | Créer méthode `ExporterPDF()` dans Helper | Méthode créée | - Signature : `ExporterPDF(rtb As RichTextBox, filePath As String)`<br>- Conversion RTF → PDF<br>- Sauvegarde fichier | ☐ |
| 19.1.4 | Ajouter bouton Export PDF dans toolbar | Bouton ajouté | - Icône PDF (📄 ou glyph)<br>- Tooltip "Exporter en PDF"<br>- Handler connecté | ☐ |
| 19.1.5 | Tester export PDF | PDF généré correctement | - Fichier PDF créé<br>- Formatage préservé (gras, couleurs, alignement)<br>- Lisible dans Adobe Reader / navigateur | ☐ |

---

## ✅ VALIDATION FINALE

### Checklist de clôture

| # | Critère | Validation | ✅ |
|---|---------|-----------|---|
| 1 | Tous les tests des sections 1-18 passés | ✓ ou ✗ | ☐ |
| 2 | Zéro erreur de compilation | ✓ ou ✗ | ☐ |
| 3 | Zéro erreur runtime lors des tests | ✓ ou ✗ | ☐ |
| 4 | Documentation complète et à jour | ✓ ou ✗ | ☐ |
| 5 | Form de test `TestRichTextEditor` fonctionnelle | ✓ ou ✗ | ☐ |
| 6 | Bouton provisoire dans `Home.vb` opérationnel | ✓ ou ✗ | ☐ |
| 7 | Thème Althéa respecté (couleurs, polices) | ✓ ou ✗ | ☐ |
| 8 | Raccourcis clavier testés et fonctionnels | ✓ ou ✗ | ☐ |
| 9 | Impression avec aperçu opérationnelle | ✓ ou ✗ | ☐ |
| 10 | Modes ReadOnly et ShowToolbar validés | ✓ ou ✗ | ☐ |
| 11 | Persistance RTF + TXT validée | ✓ ou ✗ | ☐ |
| 12 | Événements ContentChanged et ContentSaved testés | ✓ ou ✗ | ☐ |
| 13 | Intégration IContextAwareUserControl validée | ✓ ou ✗ | ☐ |
| 14 | Gestion des erreurs robuste | ✓ ou ✗ | ☐ |
| 15 | Performance satisfaisante (documents volumineux) | ✓ ou ✗ | ☐ |

---

## 📝 NOTES & OBSERVATIONS

### Bugs identifiés

| # | Description | Gravité | Statut |
|---|-------------|---------|--------|
| - | (À compléter lors des tests réels) | - | - |

### Améliorations suggérées

| # | Description | Priorité | Statut |
|---|-------------|----------|--------|
| 1 | Implémenter dialogue Rechercher/Remplacer complet (Phase 2) | Moyenne | Préparé |
| 2 | Ajouter export PDF (Syncfusion ou autre) | Haute | À faire |
| 3 | Ajouter numérotation automatique (1. 2. 3.) | Basse | Optionnel |
| 4 | Implémenter liens hypertextes cliquables | Basse | Optionnel |

---

## 📚 RÉFÉRENCES

- **Documentation** : `Docs/UC_RichTextEditor_Documentation.md`
- **Historique** : `Docs/Historique_Implementation_RichTextEditor.md`
- **Helper** : `Utils/Helpers/RichTextEditorHelper.vb`
- **UserControl** : `UI/Controls/UC_RichTextEditor.vb`
- **Form de test** : `UI/Forms/TestRichTextEditor.vb`
- **Standards commentaires** : `Docs/Standards-Commentaires.md`

---

**Dernière mise à jour** : 17/05/2026  
**Auteur** : Joëlle (Manou) / Projet Althéa  
**Statut** : 🚀 Prêt pour les tests
