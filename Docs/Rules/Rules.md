# Rules – Althéa

> *Dernière mise à jour : 10/06/2026*

## Objectif
Ce document fixe les règles de conception et de développement pour maintenir une application fiable, lisible et reprise facilement.

## Priorité des règles (ordre d'arbitrage)
Quand plusieurs règles entrent en tension, appliquer cet ordre :

1. **Sécurité** (protection des données, secrets, droits, traçabilité)
2. **Architecture** (séparation des responsabilités, couches, points d'accès uniques)
3. **Style & conventions** (noms, présentation, homogénéité)

**Règle** : Une règle de niveau inférieur ne doit jamais affaiblir une règle de niveau supérieur.

---

## 1. Philosophie générale
- Réfléchir avant de coder.
- Toute fonctionnalité est analysée, décrite et validée.
- Développement progressif, sans accumulation de code non stabilisé.
- Validation étape par étape obligatoire.

## 2. Approche de développement
- Refactorisation autorisée et encouragée si elle améliore clarté/maintenabilité.
- Pas de développement exploratoire sur des éléments structurants.
- Toute évolution doit rester traçable (documentation + changelog).

## 3. Qualité du code
### Clarté
- Noms explicites.
- Aucun code « magique ».
- Lisibilité immédiate.

### Structure
- Une classe = une responsabilité.
- Séparation claire des rôles.
- Aucune logique mélangée.

## 4. Architecture applicative
### Structure globale
- `Core` → technique
- `Metier` → logique métier
- `UI` → interface
- `Utils` → utilitaires

### Principes
- Séparation stricte des couches.
- Aucun mélange UI / métier / accès DB.
- SQL centralisé dans les modules Query.
- Toute logique répétable est factorisée.

## 5. Sécurité
- Aucun mot de passe en clair.
- Aucun secret dans les logs.
- Toute exception est gérée (aucune exception silencieuse).
- Toute action sensible est validée côté métier.

### Rôles
- Rôles actuels : `User`, `SuperUser`, `Admin`.
- Les droits ne sont jamais contrôlés uniquement par visibilité UI.
- L'élévation temporaire doit être explicite, tracée, et réversible.

### Gestion des utilisateurs
- **Modes d'édition** : `Creation`, `Modification`, `Consultation`.
- **Mode Consultation** : réservé aux SuperUsers pour visualisation seule + actions de maintenance limitées (déverrouillage, reset password).
- **Mode Création** : génération automatique de mot de passe temporaire, copie dans le presse-papiers, flag `must_change_password = True`.
- **Mode Modification** : login en lecture seule (non modifiable), modification des autres champs selon droits.
- **Actions de maintenance** :
  - Réinitialisation de mot de passe : via `ChangePassword` en mode `AdminReset` (Admin ou SuperUser)
  - Déverrouillage de compte : Admin ou SuperUser
  - Activation/désactivation de compte : Admin uniquement
- **Journalisation** : toutes les actions de sécurité sont loggées (création, modification, reset password, déverrouillage, activation/désactivation).

## 6. Base de données
### Principes
- Clé primaire numérique obligatoire (`id_xxx`).
- `AUTO_INCREMENT` interdit (utiliser séquences/stratégie projet).
- Codes métier lisibles.
- Pour MariaDB avec séquences : utiliser `LASTVAL(nom_sequence)` pour récupérer l'ID généré.

### Référentiels
Structure normalisée :
- `id`
- `code`
- `libelle`
- `actif`
- `ordre_affichage`

### Relations et séparation
- Table N-N : préfixe `lia_`.
- Données métier ≠ données techniques.
- Données UI ≠ données persistées.

## 7. Environnement technique
- Framework : `net8.0-windows`.
- Packages compatibles uniquement.
- `MySqlConnector` obligatoire.

## 8. Démarrage application
- Démarrage via `Home`.
- Connexion DB obligatoire avant déverrouillage UI.

### Connexion DB
- `DatabaseManager` est le seul point d'accès DB.
- Pas de connexion directe.
- `OpenConnection()` doit retourner une connexion valide ou lever une exception.
- Utilisation systématique de `Using`.
- Ne pas dupliquer des vérifications de connexion hors `DatabaseManager` (`conn.State`, `conn IsNot Nothing`, etc.).

## 9. UI / Navigation
### Structure
- Une seule Form principale : `Home`.
- Pas de MDI.
- Navigation centralisée via `NavigationManager`.
- Un seul écran actif.

### Écrans
- Écrans principaux = UserControls.
- Forms réservées aux cas ponctuels/techniques/modaux.

### Interdictions
- Pas d'accès DB dans l'UI.
- Pas de logique métier dans l'UI.
- Pas de manipulation directe de `pnlContent` hors mécanisme prévu.
- Pas de `SplitContainer` (instabilités identifiées).

## 10. Cohérence UI
- Style uniforme obligatoire.
- Composants homogènes dans toute l'application.
- Aucune variation locale non validée.
- Référence UI : [Reference_UI_Proprietes.md](./Reference_UI_Proprietes.md) et [Reference_UI_Guide_Utilisation.md](./Reference_UI_Guide_Utilisation.md)

## 11. Boutons
### Principes
- Aucun traitement métier dans les événements `Click`.
- Appel de méthodes dédiées.
- Initialisation via `UtilsButtons`.
- Cohérence graphique via `UITheme`.

### Standards
- `Tag` obligatoire : nom image `_normal` sans extension.
- États visuels centralisés (`normal`, `hover/selected`, `disabled`).
- Les alignements du Designer ne doivent pas être écrasés par `UtilsButtons`.
- L'état `Enabled` reste piloté par la logique écran.

### Initialisation
- Tout bouton est initialisé dans le `Load` de la Form/UserControl.

## 12. Gestion des erreurs & logs
- Tout `Catch` doit produire un log exploitable.
- Aucun log de mot de passe ni de chaîne de connexion sensible.
- Utiliser le mécanisme de masquage des secrets.
- Chaque session applicative produit un en-tête de log (date/heure, machine, utilisateur).
- Tout nouveau processus inclut logging structuré + catégorie adaptée (`Startup`, `Database`, `UI`, `Process`, `Security`).

## 13. Documentation & traçabilité
- En-tête de fichier versionné.
- En-tête de méthode.
- `CHANGELOG.md` mis à jour.
- Documentation de décision maintenue.
- Résumé quotidien (OneNote) maintenu.

## 14. Paramètres applicatifs
- Paramètres stockés dans `tec_parametres`.
- Aucune suppression physique ; désactivation via `actif`.
- `cle_parametre` est technique et immuable après création.
- Clés/groupes normalisés : MAJUSCULE, sans accents, sans espaces.
- Type contrôlé via listes fermées quand applicable.

### Chemins
- Stockage en segments logiques en base.
- Construction des chemins via `Path.Combine`.
- Variables système résolues côté code.

## 15. UI globale, contexte et modales

### Interfaces de contexte

L'application utilise deux interfaces pour l'injection du contexte UI :

- **`IContextAwareUserControl`** : pour les UserControls chargés dans `Home` via `NavigationManager`.
- **`IContextAwareForm`** : pour les Forms modales ouvertes depuis `Home` ou un UserControl.

**Implémentation obligatoire** :

```vb
' UserControl
Public Class UC_MonModule
	Implements IContextAwareUserControl

	Private _context As UserControlContext

	Public Sub SetContext(context As UserControlContext) _
		Implements IContextAwareUserControl.SetContext
		_context = context
	End Sub
End Class

' Form modale
Public Class MaFormModale
	Implements IContextAwareForm

	Private _context As UserControlContext

	Public Sub SetContext(context As UserControlContext) _
		Implements IContextAwareForm.SetContext
		_context = context
	End Sub
End Class
```

### UserControls dans `Home`
- Ne pas créer localement `StatusStrip`, `ToolTip`, `ErrorProvider`, label de contexte.
- Utiliser `UserControlContext` via `IContextAwareUserControl`.
- Interactions via méthodes de contexte (`SetStatus`, `SetHeader`/`SetContexte`, `SetToolTip`, `SetError`, `ClearError`).

### Forms autonomes avant `Home`
- Ex.: `Login`, `ChangePassword` depuis `Login`.
- Peuvent conserver leurs composants locaux.

### Forms modales ouvertes depuis `Home`
- Utiliser `IContextAwareForm`.
- Appeler `SetContext(...)` avant `ShowDialog(...)`.
- Restaurer le contexte précédent après contexte temporaire.

### ToolTips
- Initialisation dans une méthode dédiée (`InitializeToolTips` ou `InitialiserToolTips`).
- Pas de ToolTips dispersés dans les événements.

## 16. Règles issues de `UC_Parametres` (référence) et évolutions DialogChoix

### UI - Feedback utilisateur

- **Éviter `MessageBox`** : utiliser systématiquement `DialogChoix` pour la cohérence visuelle et l'expérience utilisateur.
- **DialogChoix obligatoire** pour :
  - Messages d'information (`DialogChoix.Information`)
  - Avertissements (`DialogChoix.Avertissement`)
  - Erreurs (`DialogChoix.Erreur`)
  - Confirmations (`DialogChoix.Confirmer`)
  - Messages de succès (`DialogChoix.Succes`)
- **MessageBox toléré uniquement** en cas de blocage critique système (ex. : erreur démarrage avant chargement des ressources).
- Séparer Header (contexte) et Status (action).
- Nettoyer `errProvider` à chaque changement d'état.

### DialogChoix - Composant de dialogue personnalisé

**Principe** : `DialogChoix` remplace tous les `MessageBox.Show` de l'application pour garantir une cohérence visuelle avec la charte graphique.

**Types de dialogue** :
- `Information` : messages informatifs
- `Warning` : avertissements
- `Error` : erreurs
- `Question` : questions (utiliser `Confirmer` pour Yes/No)
- `Success` : confirmations de succès
- `Loading` / `Processing` : opérations en cours

**Utilisation** :

```vb
' Méthodes statiques simples (usage courant)
DialogChoix.Information("Opération réussie")
DialogChoix.Erreur("Une erreur s'est produite")
DialogChoix.Avertissement("Attention : données manquantes")
DialogChoix.Succes("Enregistrement effectué avec succès")

' Confirmation Yes/No
If DialogChoix.Confirmer("Voulez-vous continuer ?") = DialogResult.Yes Then
	' action
End If

' Configuration avancée (3 boutons personnalisés)
Dim dlg As New DialogChoix()
dlg.Titre = "Action critique"
dlg.Message = "Confirmer la suppression définitive ?"
dlg.TypeDialogue = TypeDialogue.Warning
dlg.SetBoutons("Supprimer", "Annuler", "Aide")
Select Case dlg.ShowDialog()
	Case DialogResult.Yes
		' Supprimer
	Case DialogResult.No
		' Annuler
	Case DialogResult.Cancel
		' Aide
End Select
```

**Règles** :
- Les icônes sont définies par `TypeDialogue` (chargées depuis `My.Resources`, support GIF animés).
- La taille s'adapte automatiquement au contenu du message.
- Le thème est appliqué via `UITheme` (cohérence globale).
- Mapping DialogResult : Bouton1=Yes, Bouton2=No, Bouton3=Cancel.

### Validation
- Validation visible (`errProvider`), expliquée (status/message), et loggée.
- Ne jamais laisser un champ technique libre quand une liste contrôlée est possible.

### Architecture
- SQL → Query
- DB → Gestion
- UI → UserControl
- Validation → Utils

### UX
- Une erreur = feedback local + global.
- Un succès = message discret (DialogChoix.Succes).
- Une action critique = DialogChoix.Confirmer.

## 17. UITheme et couleurs
- Toutes les couleurs UI pilotées par code proviennent de `UITheme`.
- Interdiction de `Color.FromArgb(...)` directement dans Forms/UserControls, sauf exception documentée.
- Les contrôles dynamiques utilisent aussi `UITheme`.

## 18. UtilsIcons - Centralisation des icônes d'état

**Principe** : `UtilsIcons` centralise l'accès aux icônes d'état de l'application pour éviter les chargements multiples et garantir une cohérence visuelle.

**Fonctions disponibles** :
- `IconOK(Optional size As Integer = 32)` : État actif/valide (icône verte)
- `IconOFF(Optional size As Integer = 32)` : État inactif/désactivé (icône rouge/grise)
- `IconLock(Optional size As Integer = 32)` : Compte verrouillé (cadenas)
- `IconNo(Optional size As Integer = 32)` : Refus/interdiction

**Support multi-tailles** : 16x16, 20x20, 26x26, 32x32 (valeur par défaut : 32).

**Chargement centralisé** : depuis `My.Resources`, évitant les chargements multiples depuis les fichiers.

**Utilisation** :

```vb
' Dans un DataGridView CellFormatting
Private Sub dgvListe_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) _
	Handles dgvListe.CellFormatting

	If e.ColumnIndex = colEtat.Index AndAlso e.RowIndex >= 0 Then
		Dim row As DataGridViewRow = dgvListe.Rows(e.RowIndex)
		Dim isActif As Boolean = CBool(row.Cells("Actif").Value)

		If isActif Then
			e.Value = UtilsIcons.IconOK(32)
		Else
			e.Value = UtilsIcons.IconOFF(32)
		End If
	End If
End Sub

' Gestion prioritaire des états (exemple : utilisateurs)
If utilisateur.CompteVerrouille Then
	e.Value = UtilsIcons.IconLock(32)
ElseIf utilisateur.Actif Then
	e.Value = UtilsIcons.IconOK(32)
Else
	e.Value = UtilsIcons.IconOFF(32)
End If
```

**Règles** :
- Toujours utiliser `UtilsIcons` pour les icônes d'état (ne jamais charger directement depuis `My.Resources` dans les UserControls/Forms).
- Privilégier la taille 32x32 pour les DataGridView (cohérence visuelle).
- Définir une priorité d'affichage claire quand plusieurs états coexistent (ex. : Verrouillé > Actif > Inactif).

## 19. Terminologie métier / technique
| Contexte | Terme |
| --- | --- |
| Métier (psychologie) | Dossier |
| Système de fichiers (UI) | Répertoire |
| Technique / code | Path / Folder |

Règle : pas de mélange de terminologie dans un même contexte.

## 20. Écrans dynamiques
- Configurer complètement les contrôles à la création.
- Attacher les événements immédiatement après instanciation.
- Lire explicitement les valeurs UI via méthode dédiée (ex. `LireValeursDepuisUI`).
- Toujours valider l'UI avant opération DB.
- Éviter les rechargements complets inutiles si une mise à jour locale suffit.

## 21. Composant UC_RichTextEditor - Gestion des notes

**Principe** : `UC_RichTextEditor` est le composant standard pour toute saisie de notes formatées dans l'application (anamnèses, bilans, comptes-rendus, correspondances, etc.).

### Règles de sauvegarde obligatoires

Toute utilisation de `UC_RichTextEditor` impose une **sauvegarde double format** en base de données :

1. **Champ RTF (TEXT/MEDIUMTEXT)** : stocke le formatage complet
   - Préserve gras, italique, couleurs, alignement, puces, polices, retraits
   - Utilisé pour l'affichage fidèle du document
   - Exemple : `notes_rtf`, `anamnese_rtf`, `bilan_rtf`, `compte_rendu_rtf`

2. **Champ TXT (TEXT/MEDIUMTEXT)** : stocke le texte brut
   - Texte sans formatage, utilisé pour la recherche full-text SQL
   - Permet des recherches performantes au niveau Dossier/Patient
   - Exemple : `notes_txt`, `anamnese_txt`, `bilan_txt`, `compte_rendu_txt`

### Architecture base de données

Lors de la création ou modification d'une table contenant des notes :

```sql
CREATE TABLE xxx_bilans (
    id_bilan INT PRIMARY KEY,
    id_dossier INT NOT NULL,
    -- autres champs métier
    bilan_rtf MEDIUMTEXT,        -- ⚠️ Obligatoire : formatage préservé
    bilan_txt MEDIUMTEXT,        -- ⚠️ Obligatoire : recherche full-text
    date_creation DATETIME NOT NULL,
    date_modification DATETIME,
    -- ...
    FOREIGN KEY (id_dossier) REFERENCES xxx_dossiers(id_dossier)
);

-- Index full-text pour recherche performante
CREATE FULLTEXT INDEX idx_bilan_txt ON xxx_bilans(bilan_txt);
```

### Code de sauvegarde type

```vb
' Récupération du contenu éditeur
Dim contenuRtf As String = ucEditor.RtfContent
Dim contenuTxt As String = ucEditor.TextContent

' Sauvegarde en base (via classe métier)
bilan.BilanRtf = contenuRtf  ' Formatage complet
bilan.BilanTxt = contenuTxt  ' Texte brut pour recherche
GestionBilans.Enregistrer(bilan)
```

### Code de chargement type

```vb
' Chargement depuis base
Dim bilan As BilanPatient = GestionBilans.ChargerParId(idBilan)

' Affichage dans l'éditeur (RTF uniquement)
ucEditor.RtfContent = bilan.BilanRtf
```

### Règles strictes

1. **Interdiction de stocker uniquement RTF** : sans TXT, pas de recherche possible
2. **Interdiction de stocker uniquement TXT** : perte du formatage professionnel
3. **Vérification création tables** : lors de l'ajout de notes dans un écran, toujours vérifier que la table sous-jacente possède bien les deux champs (`xxx_rtf` + `xxx_txt`)
4. **Cohérence des noms** : même préfixe pour les deux champs (ex. : `anamnese_rtf` / `anamnese_txt`, pas `anamnese_rtf` / `notes_txt`)
5. **Types de champs** : `TEXT` (64KB max) ou `MEDIUMTEXT` (16MB max) selon besoin, **jamais VARCHAR**
6. **Index full-text** : toujours créer un index full-text sur le champ `xxx_txt` pour optimiser les recherches

### Modules métier concernés (à venir)

- **Patients** : notes générales patient (`notes_patient_rtf` / `notes_patient_txt`)
- **Dossiers** : synthèse dossier (`synthese_rtf` / `synthese_txt`)
- **Anamnèses** : historique patient (`anamnese_rtf` / `anamnese_txt`)
- **Bilans** : comptes-rendus bilans (`bilan_rtf` / `bilan_txt`)
- **Séances** : notes consultation (`notes_seance_rtf` / `notes_seance_txt`)
- **Correspondances** : courriers professionnels (`courrier_rtf` / `courrier_txt`)
- **Plans d'accompagnement** : documents évolutifs (`plan_rtf` / `plan_txt`)

### Recherche full-text exemple

```sql
-- Recherche dans tous les bilans d'un patient
SELECT b.id_bilan, b.date_bilan, 
       MATCH(b.bilan_txt) AGAINST ('graphomotricité' IN NATURAL LANGUAGE MODE) AS score
FROM xxx_bilans b
INNER JOIN xxx_dossiers d ON b.id_dossier = d.id_dossier
WHERE d.id_patient = @idPatient
  AND MATCH(b.bilan_txt) AGAINST ('graphomotricité' IN NATURAL LANGUAGE MODE)
ORDER BY score DESC, b.date_bilan DESC;
```

### Point de vigilance

⚠️ **Lors de l'ajout d'un `UC_RichTextEditor` dans un nouvel écran** :
1. Vérifier que la table cible possède les deux champs RTF + TXT
2. Si les champs manquent, créer une migration SQL avant d'intégrer le composant
3. Mettre à jour la classe métier pour exposer les deux propriétés
4. Documenter l'usage dans la documentation technique du module

**Règle** : Aucun `UC_RichTextEditor` **ni `UC_RichTextEditorSimple`** ne doit être intégré dans un écran sans vérification préalable de la structure base de données (présence des colonnes `*_rtf` **et** `*_txt`). Les champs `commentaire` enrichis (`famille_contacts`, `autres_suivis_patient`) suivent la même règle double format (cf. ADR-15, D-Q7bis).

## 22. Nommage & discipline projet
### Nommage
- Noms explicites, sans abréviation obscure.
- Classes : `Patient`, `Dossier`, etc.
- Modules : `GestionX`, `UtilsX`, etc.
- UI : préfixes obligatoires (`txt`, `btn`, `dgv`, `lbl`, `pnl`, ...).
- Champs privés : préfixe `_`.

### Standards de code/documentation
- Respecter `Option Strict On` et `Option Explicit On`.
- Appliquer les standards de commentaires : [Standards-Commentaires.md](./Standards-Commentaires.md)

### Discipline
- Changelog à chaque évolution.
- Documentation tenue à jour.
- Règles évolutives, mais validées et contrôlées.

### Vocabulaire métier officiel
- **Domaine** (et non « volet ») : domaine d'activité (Psychologie, Graphothérapie, Realism…). Tables : `ref_domaines`, `id_domaine`.
- **Thérapeute** (et non « médecin ») : tout intervenant (médecin traitant, logopède, adresseur…). Table : `therapeutes`.

### Référentiels (UC physique + classe de base)
- Un **UserControl physique par référentiel** (design libre, `.Designer.vb` propre à chaque écran).
- Logique mutualisée dans `UC_ReferentielBase` ; les noms réels des UCs sont `UC_Domaines`, `UC_LiensPatient`, `UC_RolesIntervenant`, `UC_SituationsFamiliales`, `UC_StatutsDossier`, `UC_StatutsSeance`, `UC_TypesDocuments`, `UC_TypesRendezVous`, `UC_TypesSeance` (pas de préfixe `Ref` dans le nom de fichier).
- `ReferentielLigne` est le modèle de présentation générique — l'UC de base ne connaît pas les modèles métier.
- Les hooks champ supplémentaire (`ConfigurerChampSupplementaire`, etc.) sont no-op par défaut ; seul `UC_TypesSeance` les surcharge pour `tarif_defaut`.
- Soft-delete prioritaire : si `<X>EstUtilise()` retourne `True`, désactiver uniquement (jamais supprimer physiquement).
- Cf. ADR-18.
- 
---

> **Contact**: **Joëlle (Manou) - Les Artefacts de Manou**  
> Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.  
> - Site web P.Nguyen Duy : https://pearlnguyenduy.be/  
> - mailto: `joelle@nguyen.eu`  
> - GitHub privé : Althea https://github.com/AngeljoNG/Althea  
> - GitHub public :  Althea https://github.com/Les-Artefacts-de-Manou/Althea
> 
> ---

[TOC]

