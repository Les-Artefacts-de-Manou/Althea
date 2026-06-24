# Standards de Documentation - Projet Althéa

>  *Dernière mise à jour : 19/05/2026*

Ce document définit les modèles standardisés de documentation pour tous les types d'éléments du projet Althéa.

---

## Table des matières

1. [En-têtes de fichiers](#en-têtes-de-fichiers)
   - [Module](#module)
   - [Classe](#classe)
   - [Classe DTO](#classe-dto)
   - [Classe utilitaire/Helper](#classe-utilitairehelper)
   - [Classe partielle (Designer)](#classe-partielle-designer)
   - [Module Extensions](#module-extensions)
   - [Interface](#interface)
   - [UserControl](#usercontrol)
   - [Form](#form)
2. [En-têtes de membres](#en-têtes-de-membres)
   - [Procédure (Sub)](#procédure-sub)
   - [Fonction (Function)](#fonction-function)
   - [Propriété simple (Property)](#propriété-simple-property)
   - [Propriété calculée (ReadOnly Property)](#propriété-calculée-readonly-property)
   - [Variable privée](#variable-privée)
   - [Constante](#constante)
   - [Énumération (Enum)](#énumération-enum)
   - [Constructeur](#constructeur)
   - [Gestionnaire d'événement (Event Handler)](#gestionnaire-dévénement-event-handler)
   - [Événement personnalisé](#événement-personnalisé)
3. [Organisation en Régions](#organisation-en-régions)
4. [Recommandations finales](#recommandations-finales)

---

## En-têtes de fichiers

### Module

```vb
' -------------------------------------------------------------------------------------------------
' Module     : NomModule
' Projet     : Althéa
' Version    : V1.0
' Date       : JJ/MM/AAAA
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Description du rôle du module : fonctions/procédures utilitaires, helpers partagés, etc.
'
' Responsabilités :
' - Responsabilité 1
' - Responsabilité 2
' - Responsabilité 3
'
' Remarques  :
' - Remarque importante 1
' - Remarque importante 2
' - Contraintes ou spécificités du module
' - Contexte d'utilisation
'
' Dépendances :
' - Dépendance1 (description)
' - Dépendance2 (description)
'
' Imports    :
' - System.xxx
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Module NomModule
	' Contenu du module
End Module
```

**Remarques spécifiques pour les Modules :**
- Les modules contiennent généralement des fonctions/procédures **partagées** (Shared équivalent)
- Pas de constructeur ni d'état (pas de variables d'instance)
- Utilisés pour regrouper des utilitaires globaux

---

### Classe

```vb
' -------------------------------------------------------------------------------------------------
' Classe     : NomClasse
' Projet     : Althéa
' Version    : V1.0
' Date       : JJ/MM/AAAA
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Description du rôle de la classe : responsabilité principale, domaine fonctionnel.
'
' Responsabilités :
' - Responsabilité 1 (ex: Gérer la connexion à la base de données)
' - Responsabilité 2 (ex: Encapsuler les paramètres de configuration)
' - Responsabilité 3 (ex: Journaliser les actions critiques)
'
' Remarques  :
' - Remarque sur l'architecture (singleton, statique, instanciée, etc.)
' - Contraintes d'utilisation
' - Contexte métier ou technique
' - Patterns appliqués (Factory, Repository, etc.)
'
' Dépendances :
' - Dépendance1 (description de l'usage)
' - Dépendance2 (description de l'usage)
'
' Imports    :
' - System.xxx
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Public Class NomClasse
	' Contenu de la classe
End Class
```

**Remarques spécifiques pour les Classes :**
- Bien distinguer les **responsabilités** (ce que fait la classe) des **remarques** (comment elle fonctionne)
- Mentionner si la classe est un **Singleton**, **Factory**, **Manager**, **Service**, etc.
- Préciser si la classe est **thread-safe** ou non

---

### Classe DTO

```vb
' -------------------------------------------------------------------------------------------------
' Classe DTO : NomDTO
' Projet     : Althéa
' Version    : V1.0
' Date       : JJ/MM/AAAA
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Objet de transfert de données (DTO) représentant [entité métier ou concept].
' Utilisé pour transférer les données entre la couche [source] et la couche [destination].
'
' Responsabilités :
' - Encapsuler les données de [entité/table/concept]
' - Fournir un modèle fortement typé pour les échanges de données
' - Assurer la validation des propriétés (si applicable)
'
' Remarques  :
' - Pas de logique métier (uniquement propriétés et validation basique)
' - Mapping 1:1 avec [table/entité/API] ou composition de plusieurs sources
' - Utilisé dans [contextes d'usage : DAL, API, Services, etc.]
' - Propriétés auto-implémentées ou avec validation
'
' Dépendances :
' - Aucune (DTO pur) ou dépendances minimales (ex: System.ComponentModel pour validation)
'
' Imports    :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Public Class NomDTO
	' Propriétés auto-implémentées
End Class
```

**Remarques spécifiques pour les Classes DTO :**
- Les DTO doivent être **simples** : pas de logique métier complexe
- Mentionner le **mapping** avec la source de données (table, API, etc.)
- Indiquer si le DTO est **mutable** ou **immutable**
- Préciser si des **validations** sont implémentées (DataAnnotations, IDataErrorInfo, etc.)

---

### Classe utilitaire/Helper

```vb
' -------------------------------------------------------------------------------------------------
' Classe     : NomHelper
' Projet     : Althéa
' Version    : V1.0
' Date       : JJ/MM/AAAA
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Classe utilitaire fournissant des fonctions/procédures helpers pour [domaine spécifique].
'
' Responsabilités :
' - Fournir des méthodes statiques (Shared) pour [usage 1]
' - Centraliser la logique commune de [domaine]
' - Éviter la duplication de code dans [contexte]
'
' Remarques  :
' - Classe statique (NotInheritable + constructeur privé) ou instanciable selon le contexte
' - Toutes les méthodes sont Shared (ou majoritairement)
' - Pas d'état interne (ou état minimal en cache)
' - Thread-safe si utilisée dans un contexte multi-thread
'
' Dépendances :
' - Dépendance1 (description)
' - Dépendance2 (description)
'
' Imports    :
' - System.xxx
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Public NotInheritable Class NomHelper
	' Constructeur privé pour empêcher l'instanciation
	Private Sub New()
	End Sub

	' Méthodes Shared
End Class
```

**Remarques spécifiques pour les Classes utilitaires :**
- Généralement **NotInheritable** avec constructeur **Private** (empêcher instanciation)
- Toutes les méthodes sont **Shared**
- Pas d'état interne (ou très limité, ex: cache statique)
- Nom généralement suffixé par **Helper**, **Utils**, **Manager** (ex: UtilsButtons, CryptoHelper)

---

### Classe partielle (Designer)

```vb
' -------------------------------------------------------------------------------------------------
' Classe partielle (Designer) : NomForm
' Projet     : Althéa
' Version    : V1.0
' Date       : JJ/MM/AAAA
'
' Rôle       :
' Partie générée automatiquement par le Designer pour [Form/UserControl].
' Contient les définitions de contrôles et l'initialisation de l'interface (InitializeComponent).
'
' Remarques  :
' - Fichier généré automatiquement par Visual Studio : NE PAS MODIFIER MANUELLEMENT
' - Les modifications visuelles doivent être faites via le Designer Visual Studio
' - Synchronisé avec le fichier .resx pour les ressources (textes, images, etc.)
' - Toute modification manuelle sera écrasée lors de la prochaine régénération
'
' Imports    :
' - System.ComponentModel
' - System.Windows.Forms
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NomForm
    Inherits System.Windows.Forms.Form

    ' Contrôles générés par le Designer

    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        ' Libération des ressources
    End Sub

    Private Sub InitializeComponent()
        ' Initialisation générée par le Designer
    End Sub

End Class
```

**Remarques spécifiques pour les Classes partielles Designer :**
- Fichier **.Designer.vb** généré automatiquement
- **Ne jamais modifier manuellement** (écrasé par le Designer)
- Contient uniquement les **définitions de contrôles** et **InitializeComponent**
- Toute logique métier doit être dans le fichier principal (.vb)

---

### Module Extensions

```vb
' -------------------------------------------------------------------------------------------------
' Module Extensions : NomExtensions
' Projet     : Althéa
' Version    : V1.0
' Date       : JJ/MM/AAAA
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Fournit des méthodes d'extension pour [type étendu : String, List(Of T), DataTable, etc.].
' Permet d'ajouter des fonctionnalités utilitaires aux types existants sans les modifier.
'
' Responsabilités :
' - Étendre [type] avec des méthodes utilitaires courantes
' - Centraliser les opérations répétitives sur [type]
' - Simplifier le code appelant par une syntaxe fluide
'
' Remarques  :
' - Utilise l'attribut <Extension()> pour chaque méthode d'extension
' - Les méthodes d'extension sont disponibles dès que le namespace est importé
' - Préférer des noms explicites et non ambigus pour éviter les conflits avec d'autres extensions
' - Convention : premier paramètre préfixé par "this" en C# (implicite en VB.NET)
' - Utilisable uniquement sur les instances (pas sur Nothing)
'
' Dépendances :
' - System.Runtime.CompilerServices (attribut Extension)
'
' Imports    :
' - System.Runtime.CompilerServices
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Imports System.Runtime.CompilerServices

Module NomExtensions

    ' -------------------------------------------------------------------------------------------------
    ' Extension  : NomMethode
    ' Type étendu : TypeEtendu
    ' Type retour : TypeRetour
    ' Version    : V1.0.0
    ' Date       : JJ/MM/AAAA
    '
    ' Rôle       :
    ' Étend [TypeEtendu] pour [description de la fonctionnalité ajoutée].
    '
    ' Paramètres :
    ' - value      : Instance sur laquelle la méthode d'extension est appelée (TypeEtendu)
    ' - parametre1 : Description du paramètre additionnel (Type1)
    '
    ' Retour     :
    ' - TypeRetour : Description du résultat retourné
    '
    ' Remarques  :
    ' - Syntaxe d'appel : instance.NomMethode(parametre1)
    ' - [Cas d'usage typique]
    ' - [Comportement si value est Nothing] : Exception ou retour valeur par défaut
    '
    ' Exceptions :
    ' - ArgumentNullException : Si value est Nothing
    ' - [Autres exceptions selon le contexte]
    '
    ' Exemples   :
    ' ' Utilisation standard
    ' Dim result = maVariable.NomMethode(param1)
    ' -------------------------------------------------------------------------------------------------
    <Extension()>
    Public Function NomMethode(value As TypeEtendu, parametre1 As Type1) As TypeRetour
        ' Vérification Nothing
        If value Is Nothing Then
            Throw New ArgumentNullException(NameOf(value))
        End If

        ' Logique de l'extension
        Return resultat
    End Function

End Module
```

**Remarques spécifiques pour les Modules Extensions :**
- Nom du module généralement suffixé par **Extensions** (ex: StringExtensions, ListExtensions)
- Toutes les méthodes doivent avoir l'attribut **<Extension()>**
- Le premier paramètre est toujours l'instance sur laquelle la méthode est appelée
- **Vérifier Nothing** au début de chaque méthode d'extension (sécurité)
- Utiliser **NameOf()** pour les messages d'erreur (refactoring-safe)

---

### Interface

```vb
' -------------------------------------------------------------------------------------------------
' Interface  : INomInterface
' Projet     : Althéa
' Version    : V1.0
' Date       : JJ/MM/AAAA
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Définit le contrat permettant à [type d'objet] de [capacité offerte].
'
' Responsabilités :
' - Déclarer la/les méthode(s) pour [usage 1]
' - Permettre [comportement ou découplage visé]
'
' Remarques  :
' - Implémentée par [liste des classes implémentant l'interface]
' - Contexte d'utilisation : [injection de dépendance, découplage, polymorphisme, etc.]
' - Pattern appliqué : [Strategy, Observer, etc.]
' - Appelée/utilisée par [qui utilise cette interface]
'
' Dépendances :
' - Type1 (utilisé dans les signatures de méthodes)
' - Type2 (utilisé dans les signatures de méthodes)
'
' Imports    :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Public Interface INomInterface

#Region "Méthodes"

	' Documentation de chaque méthode de l'interface
	Sub NomMethode(parametre As Type)
	Function NomFonction() As TypeRetour

#End Region

End Interface
```

**Remarques spécifiques pour les Interfaces :**
- Nom préfixé par **I** (convention .NET)
- Lister les **classes qui implémentent** l'interface (pour traçabilité)
- Expliquer le **contexte d'injection** : automatique (NavigationManager) ou manuelle (appelant)
- Mentionner le **pattern** appliqué (Strategy, Dependency Injection, etc.)

---

### UserControl

```vb
' -------------------------------------------------------------------------------------------------
' UserControl : UC_NomUserControl
' Projet      : Althéa
' Version     : V1.0
' Date        : JJ/MM/AAAA
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' UserControl [description du domaine fonctionnel : gestion des paramètres, accueil, admin, etc.].
' Chargé dynamiquement dans le panneau central de Home via NavigationManager.
'
' Responsabilités :
' - Responsabilité métier 1 (ex: Afficher la liste des paramètres actifs)
' - Responsabilité métier 2 (ex: Permettre la modification des paramètres autorisés)
' - Responsabilité UI 1 (ex: Gérer l'état des boutons selon le mode d'accès)
' - Responsabilité UI 2 (ex: Valider les saisies utilisateur)
' - Journaliser les actions via GestionLog
'
' Remarques   :
' - Chargé dynamiquement dans le panneau central de Home via NavigateToXXX()
' - Implémente IContextAwareUserControl pour injection du contexte UI partagé
' - Aucun accès direct à la base de données (tout passe par les services métier)
' - Navigation retour vers Home via [méthode publique de Home]
' - Le contexte UI (_context) donne accès à : StatusStrip, ToolTip, ErrorProvider, lblContexte, UserSession
'
' Dépendances :
' - ServiceMetier1 (chargement/sauvegarde des données)
' - ServiceMetier2 (validation métier)
' - UserControlContext (contexte UI injecté par Home)
' - UtilsButtons (thématisation des boutons)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : Pour injection du contexte UI partagé
'
' Imports     :
' - System.xxx
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Public Class UC_NomUserControl
	Implements IContextAwareUserControl

	' Contenu du UserControl
End Class
```

**Remarques spécifiques pour les UserControls :**
- Mentionner explicitement le **chargement dynamique** dans Home
- Préciser l'**implémentation de IContextAwareUserControl**
- Lister les **composants du contexte UI** accessibles (_context.SetStatus(), etc.)
- Expliquer la **navigation retour** vers Home (méthodes publiques)
- Insister sur le **découplage** : pas d'accès direct à la DB, pas de référence directe à Home

---

### Form

```vb
' -------------------------------------------------------------------------------------------------
' Form       : NomForm
' Projet     : Althéa
' Version    : V1.0
' Date       : JJ/MM/AAAA
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form [modale/principale] pour [usage : connexion, changement de mot de passe, configuration, etc.].
'
' Responsabilités :
' - Responsabilité métier 1 (ex: Authentifier l'utilisateur)
' - Responsabilité métier 2 (ex: Valider les informations saisies)
' - Responsabilité UI 1 (ex: Gérer l'état des boutons)
' - Responsabilité UI 2 (ex: Afficher les messages d'erreur via ErrorProvider)
' - Retourner le résultat via DialogResult (si modale)
' - Journaliser les actions via GestionLog
'
' Remarques   :
' - Form [modale/principale] ouverte via ShowDialog()/Show() depuis [contexte]
' - [Si modale] : Retourne DialogResult.OK/Cancel selon le résultat de l'action
' - [Si modale + IContextAwareForm] : Implémente IContextAwareUserControl pour injection du contexte UI partagé
' - Aucun accès direct à la base de données (tout passe par les services métier)
' - [Spécificités de navigation ou d'initialisation]
'
' Dépendances :
' - ServiceMetier1 (ex: GestionUtilisateurs pour authentification)
' - ServiceMetier2 (ex: GestionParametres pour configuration)
' - UserControlContext (contexte UI si IContextAwareForm implémenté)
' - UtilsButtons (thématisation des boutons)
' - GestionLog (journalisation)
'
' [Optionnel si IContextAwareForm]
' Interface   :
' - IContextAwareForm : Pour injection du contexte UI partagé
'
' Imports     :
' - System.xxx
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Public Class NomForm
	[Implements IContextAwareForm]

	' Contenu de la Form
End Class
```

**Remarques spécifiques pour les Forms :**
- Distinguer **Form modale** (ShowDialog, DialogResult) et **Form principale** (Show, Close)
- Mentionner le **contexte d'ouverture** (depuis Home, depuis un UserControl, au démarrage, etc.)
- Préciser si **IContextAwareForm** est implémenté (injection contexte UI)
- Expliquer le **DialogResult** (OK/Cancel) et ce qu'il signifie métier
- Documenter les **valeurs de retour** via propriétés publiques (ex: AuthenticatedUser)

---

## En-têtes de membres

### Procédure (Sub)

```vb
	' -------------------------------------------------------------------------------------------------
	' Procédure  : NomProcedure
	' Version    : V1.0.0
	' Date       : JJ/MM/AAAA
	'
	' Rôle       :
	' Description claire et concise du rôle de la procédure (ce qu'elle fait).
	'
	' Paramètres :
	' - parametre1 : Description du paramètre 1 (Type1)
	' - parametre2 : Description du paramètre 2 (Type2)
	'
	' Remarques  :
	' - Remarque importante sur le comportement
	' - Contraintes ou prérequis (ex: parametre1 ne doit pas être Nothing)
	' - Effets de bord (ex: modifie l'état de la base de données)
	' - Contexte d'appel (ex: appelée lors de l'initialisation du UserControl)
	'
	' Exceptions :
	' - ArgumentNullException : Si parametre1 est Nothing
	' - InvalidOperationException : Si [condition invalide]
	' - Ou : Aucune gestion explicite (erreurs propagées)
	' -------------------------------------------------------------------------------------------------
	Private Sub NomProcedure(parametre1 As Type1, parametre2 As Type2)
		' Corps de la procédure
	End Sub
```

**Remarques spécifiques pour les Procédures :**
- Pas de section **Retour** (Sub ne retourne rien)
- Bien documenter les **effets de bord** (modification d'état, appels de méthodes externes, etc.)
- Préciser le **contexte d'appel** (événement, initialisation, etc.)

---

### Fonction (Function)

```vb
	' -------------------------------------------------------------------------------------------------
	' Fonction   : NomFonction
	' Type       : TypeRetour
	' Version    : V1.0.0
	' Date       : JJ/MM/AAAA
	'
	' Rôle       :
	' Description claire et concise du rôle de la fonction (ce qu'elle calcule/retourne).
	'
	' Paramètres :
	' - parametre1 : Description du paramètre 1 (Type1)
	' - parametre2 : Description du paramètre 2 (Type2)
	'
	' Retour     :
	' - TypeRetour : Description du résultat retourné
	' - [Cas spécifique 1] : Retourne [valeur] si [condition]
	' - [Cas spécifique 2] : Retourne Nothing si [condition]
	'
	' Remarques  :
	' - Remarque importante sur le comportement
	' - Contraintes ou prérequis (ex: parametre1 doit être > 0)
	' - Performance (ex: O(n) si liste non triée)
	' - Contexte d'appel (ex: utilisée pour valider les données avant sauvegarde)
	'
	' Exceptions :
	' - ArgumentNullException : Si parametre1 est Nothing
	' - ArgumentOutOfRangeException : Si parametre2 < 0
	' - Ou : Aucune (retourne Nothing/valeur par défaut en cas d'erreur)
	' -------------------------------------------------------------------------------------------------
	Private Function NomFonction(parametre1 As Type1, parametre2 As Type2) As TypeRetour
		' Corps de la fonction
		Return resultat
	End Function
```

**Remarques spécifiques pour les Fonctions :**
- Section **Retour** obligatoire avec description précise de la valeur retournée
- Documenter les **cas de retour spécifiques** (Nothing, 0, True/False selon conditions)
- Mentionner les **considérations de performance** si pertinent (complexité algorithmique)

---

### Propriété simple (Property)

```vb
	' -------------------------------------------------------------------------------------------------
	' Propriété  : NomPropriete
	' Type       : TypePropriete
	' Version    : V1.0.0
	' Date       : JJ/MM/AAAA
	'
	' Rôle       :
	' Description de la propriété et de sa signification métier ou technique.
	'
	' Get        :
	' - Retourne la valeur de [backing field ou source]
	'
	' Set        :
	' - Définit la valeur de [backing field ou source]
	' - [Optionnel] : Effectue une validation (ex: valeur > 0)
	' - [Optionnel] : Déclenche un événement PropertyChanged
	'
	' Remarques  :
	' - Propriété auto-implémentée ou avec backing field
	' - Utilisée pour [contexte d'usage]
	' - Validation effectuée dans le setter (si applicable)
	'
	' Exceptions :
	' - ArgumentException : Si la valeur est invalide
	' - Ou : Aucune
	' -------------------------------------------------------------------------------------------------
	Public Property NomPropriete As TypePropriete
```

**Version simplifiée pour propriétés auto-implémentées simples :**

```vb
	' Propriété auto-implémentée : Description courte de la propriété
	Public Property NomPropriete As TypePropriete
```

**Remarques spécifiques pour les Propriétés :**
- Si la propriété est **auto-implémentée** et triviale, un commentaire inline suffit
- Si la propriété a une **logique de validation** ou **effets de bord**, utiliser le format complet
- Documenter les **Get** et **Set** séparément si leur comportement est complexe

---

### Propriété calculée (ReadOnly Property)

```vb
	' -------------------------------------------------------------------------------------------------
	' Propriété  : NomProprieteCalculee
	' Type       : TypePropriete (ReadOnly)
	' Version    : V1.0.0
	' Date       : JJ/MM/AAAA
	'
	' Rôle       :
	' Description de la propriété calculée et de sa signification.
	'
	' Retour     :
	' - TypePropriete : Description du résultat calculé
	' - [Cas spécifique] : Retourne [valeur] si [condition]
	'
	' Remarques  :
	' - Propriété calculée (pas de setter)
	' - Basée sur [source de données : champs privés, calcul, agrégation, etc.]
	' - Recalculée à chaque accès (ou mise en cache si optimisation)
	' - Utilisée pour [contexte d'usage]
	'
	' Exceptions :
	' - Aucune (ou exceptions spécifiques si calcul complexe)
	' -------------------------------------------------------------------------------------------------
	Public ReadOnly Property NomProprieteCalculee As TypePropriete
		Get
			Return [calcul ou valeur dérivée]
		End Get
	End Property
```

**Remarques spécifiques pour les Propriétés calculées :**
- Section **Retour** obligatoire (comme pour une fonction)
- Préciser si le résultat est **mis en cache** ou **recalculé à chaque accès**
- Documenter la **source du calcul** (champs privés, agrégation, etc.)

---

### Variable privée

```vb
	' -------------------------------------------------------------------------------------------------
	' Variable   : _nomVariable
	' Type       : TypeVariable [ReadOnly si applicable]
	' Date       : JJ/MM/AAAA
	'
	' Rôle       :
	' Description du rôle de la variable dans l'état de la classe/form/usercontrol.
	'
	' Remarques  :
	' - Initialisée dans [constructeur/méthode d'initialisation]
	' - [ReadOnly] : Ne peut pas être modifiée après construction (si applicable)
	' - Utilisée pour [contexte d'usage]
	' - [Spécificités] : Thread-safe, cache, état temporaire, etc.
	' -------------------------------------------------------------------------------------------------
	Private [ReadOnly] _nomVariable As TypeVariable
```

**Version simplifiée pour variables privées simples :**

```vb
	' Description courte de la variable
	Private _nomVariable As TypeVariable
```

**Remarques spécifiques pour les Variables :**
- Préfixer par **underscore** (`_`) selon la convention projet
- Préciser si **ReadOnly** (immutable après construction)
- Pour les variables complexes ou critiques, utiliser le format complet
- Pour les variables triviales (backing fields simples), un commentaire inline suffit

---

### Constante

```vb
	' -------------------------------------------------------------------------------------------------
	' Constante  : NOM_CONSTANTE
	' Type       : TypeConstante
	' Date       : JJ/MM/AAAA
	'
	' Rôle       :
	' Description de la signification de la constante (valeur magique, configuration, limite, etc.).
	'
	' Remarques  :
	' - Utilisée pour [contexte d'usage]
	' - Valeur [justification de la valeur si pertinent]
	' - Ne pas modifier sans [précaution ou impact]
	' -------------------------------------------------------------------------------------------------
	Private Const NOM_CONSTANTE As TypeConstante = Valeur
```

**Version simplifiée pour constantes simples :**

```vb
	' Description courte de la constante
	Private Const NOM_CONSTANTE As TypeConstante = Valeur
```

**Remarques spécifiques pour les Constantes :**
- Convention de nommage : **MAJUSCULES_AVEC_UNDERSCORES** ou **PascalCase** selon le contexte projet
- Justifier la **valeur** si elle n'est pas évidente (pourquoi 100 et pas 50 ?)
- Grouper les constantes liées dans un **#Region "Constantes"**

---

### Énumération (Enum)

```vb
	' -------------------------------------------------------------------------------------------------
	' Énumération : NomEnum
	' Date        : JJ/MM/AAAA
	'
	' Rôle        :
	' Description du domaine représenté par l'énumération (états, modes, rôles, types, etc.).
	'
	' Valeurs     :
	' - Valeur1 : Description de la valeur 1 (signification métier)
	' - Valeur2 : Description de la valeur 2 (signification métier)
	' - Valeur3 : Description de la valeur 3 (signification métier)
	'
	' Remarques   :
	' - Utilisée pour [contexte d'usage : typage fort, switch/select case, etc.]
	' - [Spécificités] : Flags (avec <Flags>), valeurs numériques explicites, etc.
	' -------------------------------------------------------------------------------------------------
	Public Enum NomEnum
		Valeur1 = 0
		Valeur2 = 1
		Valeur3 = 2
	End Enum
```

**Remarques spécifiques pour les Énumérations :**
- Documenter **chaque valeur** avec sa signification métier
- Préciser si l'enum est un **Flags** (avec attribut `<Flags>` et valeurs en puissances de 2)
- Mentionner les **valeurs par défaut** recommandées

---

### Constructeur

```vb
	' -------------------------------------------------------------------------------------------------
	' Constructeur : New
	' Version      : V1.0.0
	' Date         : JJ/MM/AAAA
	'
	' Rôle         :
	' Initialise une nouvelle instance de [Classe] avec [description des paramètres ou initialisation].
	'
	' Paramètres   :
	' - parametre1 : Description du paramètre 1 (Type1)
	' - parametre2 : Description du paramètre 2 (Type2)
	'
	' Remarques    :
	' - Initialise [liste des membres initialisés]
	' - [Validations effectuées] : Vérifie que parametre1 n'est pas Nothing, etc.
	' - [Effets de bord] : Appelle [méthode d'initialisation], charge [ressources], etc.
	' - Constructeur par défaut / surcharge avec paramètres
	'
	' Exceptions   :
	' - ArgumentNullException : Si parametre1 est Nothing
	' - ArgumentException : Si parametre2 est invalide
	' - Ou : Aucune
	' -------------------------------------------------------------------------------------------------
	Public Sub New(parametre1 As Type1, parametre2 As Type2)
		' Initialisation
	End Sub
```

**Remarques spécifiques pour les Constructeurs :**
- Documenter les **initialisations effectuées** (champs, propriétés, ressources)
- Préciser les **validations** des paramètres
- Mentionner les **effets de bord** (chargement de configuration, connexion DB, etc.)
- Pour les constructeurs par défaut sans paramètre ni logique, un commentaire inline suffit

---

### Gestionnaire d'événement (Event Handler)

```vb
	' -------------------------------------------------------------------------------------------------
	' Handler    : btnNom_Click
	' Événement  : Click
	' Version    : V1.0.0
	' Date       : JJ/MM/AAAA
	'
	' Rôle       :
	' Gère l'événement Click du bouton [Nom] pour [action métier : valider, enregistrer, naviguer, etc.].
	'
	' Paramètres :
	' - sender : Objet source de l'événement (Object)
	' - e      : Arguments de l'événement Click (EventArgs)
	'
	' Remarques  :
	' - Déclenché lorsque l'utilisateur clique sur [bouton/contrôle]
	' - Actions effectuées : [Validation des saisies, Navigation vers X, Sauvegarde des données, etc.]
	' - Appels métier : Appelle [service/méthode] pour effectuer [action]
	' - [Si navigation] : Charge [UserControl/Form] via [méthode de navigation]
	' - [Si validation] : Vérifie [conditions] avant d'effectuer [action]
	'
	' Exceptions :
	' - Gérées via Try/Catch avec MessageBox + GestionLog (pattern standard)
	' - Ou : Propagées au gestionnaire global si non critique
	' -------------------------------------------------------------------------------------------------
	Private Sub btnNom_Click(sender As Object, e As EventArgs) Handles btnNom.Click
		' Logique du handler
	End Sub
```

**Version simplifiée pour handlers triviaux :**

```vb
	' Gère le clic sur [bouton] : [action courte]
	Private Sub btnNom_Click(sender As Object, e As EventArgs) Handles btnNom.Click
		' Logique simple
	End Sub
```

**Remarques spécifiques pour les Event Handlers :**
- Si le handler est **trivial** (appel d'une seule méthode), un commentaire inline suffit
- Si le handler contient de la **logique métier ou validation**, utiliser le format complet
- Mentionner explicitement les **appels métier** (services, managers, etc.)
- Pour les handlers de **navigation**, préciser la destination et la méthode utilisée
- Pattern standard : **Try/Catch + MessageBox + GestionLog** pour les erreurs

**Autres types d'événements courants :**

```vb
	' Handler pour SelectionChanged
	Private Sub cbo_SelectionChanged(sender As Object, e As EventArgs) Handles cbo.SelectionChanged

	' Handler pour TextChanged
	Private Sub txt_TextChanged(sender As Object, e As EventArgs) Handles txt.TextChanged

	' Handler pour KeyDown
	Private Sub txt_KeyDown(sender As Object, e As KeyEventArgs) Handles txt.KeyDown

	' Handler pour MouseEnter/MouseLeave (affichage mot de passe, etc.)
	Private Sub pic_MouseDown(sender As Object, e As MouseEventArgs) Handles pic.MouseDown
```

---

### Événement personnalisé

```vb
	' -------------------------------------------------------------------------------------------------
	' Événement  : NomEvenementChanged
	' Délégué    : EventHandler(Of NomEventArgs)
	' Date       : JJ/MM/AAAA
	'
	' Rôle       :
	' Déclenché lorsque [condition/action se produit : changement d'état, fin de traitement, etc.].
	'
	' Arguments  :
	' - sender : Objet source de l'événement (Object)
	' - e      : Arguments personnalisés contenant [données de l'événement] (NomEventArgs)
	'
	' Remarques  :
	' - Abonnés typiques : [Qui écoute cet événement : Form parent, Manager, etc.]
	' - Déclenché par : [Méthode/action qui appelle RaiseEvent]
	' - Permet de [usage métier de l'événement : notifier, synchroniser, réagir à un changement, etc.]
	' - Pattern : Les abonnés doivent utiliser AddHandler/RemoveHandler ou Handles
	'
	' Exemples   :
	' ' Abonnement via AddHandler
	' AddHandler objetSource.NomEvenementChanged, AddressOf MaMethodeHandler
	'
	' ' Abonnement via Handles (si contrôle dans le Designer)
	' Private Sub ObjetSource_NomEvenementChanged(sender As Object, e As NomEventArgs) Handles objetSource.NomEvenementChanged
	' -------------------------------------------------------------------------------------------------
	Public Event NomEvenementChanged As EventHandler(Of NomEventArgs)
```

**Classe d'arguments d'événement associée :**

```vb
	' -------------------------------------------------------------------------------------------------
	' Classe     : NomEventArgs
	' Hérite de  : EventArgs
	' Date       : JJ/MM/AAAA
	'
	' Rôle       :
	' Arguments personnalisés pour l'événement NomEvenementChanged.
	' Encapsule les données transmises lors du déclenchement de l'événement.
	'
	' Remarques  :
	' - Hérite de EventArgs (convention .NET)
	' - Propriétés ReadOnly pour éviter la modification par les abonnés
	' - Créé par la méthode qui déclenche l'événement (RaiseEvent)
	' -------------------------------------------------------------------------------------------------
	Public Class NomEventArgs
		Inherits EventArgs

		' Propriétés de l'événement
		Public ReadOnly Property Propriete1 As Type1
		Public ReadOnly Property Propriete2 As Type2

		' Constructeur
		Public Sub New(propriete1 As Type1, propriete2 As Type2)
			Me.Propriete1 = propriete1
			Me.Propriete2 = propriete2
		End Sub
	End Class
```

**Déclenchement de l'événement :**

```vb
	' Déclenche l'événement NomEvenementChanged
	Protected Overridable Sub OnNomEvenementChanged(e As NomEventArgs)
		RaiseEvent NomEvenementChanged(Me, e)
	End Sub
```

**Remarques spécifiques pour les Événements personnalisés :**
- Nom de l'événement généralement suffixé par **Changed**, **Completed**, **Failed**, etc.
- Classe d'arguments hérite toujours de **EventArgs**
- Propriétés des arguments en **ReadOnly** (immutabilité)
- Méthode protégée **OnNomEvenement()** pour déclencher l'événement (convention .NET)
- Documenter les **abonnés typiques** (qui écoute l'événement)
- Documenter le **contexte de déclenchement** (quand et pourquoi)

---

## Principes généraux

### 1. Sections obligatoires vs optionnelles

**Toujours présentes :**
- **Rôle** : Ce que fait l'élément (raison d'être)
- **Version / Date** : Traçabilité

**Selon le type :**
- **Responsabilités** : Pour Classes, Modules, Forms, UserControls (liste des actions/rôles multiples)
- **Paramètres** : Pour Procédures, Fonctions, Constructeurs (si paramètres présents)
- **Retour** : Pour Fonctions, Propriétés calculées (description du résultat)
- **Remarques** : Contexte, contraintes, spécificités (toujours utile)
- **Exceptions** : Erreurs levées ou gestion des erreurs (si applicable)
- **Dépendances** : Pour fichiers complets (Classes, Modules, Forms, UserControls, Interfaces)
- **Interface** : Pour Classes implémentant des interfaces (traçabilité)

### 2. Niveau de détail

**Format complet :**
- En-têtes de **fichiers** (Classes, Modules, Forms, UserControls, Interfaces)
- **Procédures/Fonctions publiques** ou complexes
- **Propriétés avec logique** (validation, calcul, effets de bord)
- **Variables critiques** (état important, ReadOnly, cache)

**Format simplifié (commentaire inline) :**
- **Propriétés auto-implémentées** triviales
- **Variables privées simples** (backing fields)
- **Constantes évidentes**
- **Constructeurs par défaut** sans logique

### 3. Cohérence et lisibilité

- Utiliser des **phrases complètes** dans Rôle et Remarques
- Utiliser des **listes à puces** pour Responsabilités, Remarques multiples, Paramètres
- **Aligner** les deux-points (`:`) pour une meilleure lisibilité
- **Éviter la redondance** : ne pas répéter le nom de la méthode dans la description
- **Être explicite** : "Charge les paramètres actifs depuis la base de données" plutôt que "Charge les données"

### 4. Régionalisation

**Régions recommandées selon le type :**

**Classes/Modules :**
- `#Region "Constantes"` (si plusieurs constantes)
- `#Region "Variables privées"` ou `#Region "Champs privés"`
- `#Region "Énumérations"` (si enum interne à la classe)
- `#Region "Constructeur"` (si un seul) ou `#Region "Constructeurs"` (si plusieurs surcharges)
- `#Region "Propriétés publiques"`
- `#Region "Méthodes publiques"` (si nombreuses, sinon regrouper par domaine fonctionnel)
- `#Region "Méthodes privées"` ou découper par responsabilité fonctionnelle
- `#Region "Événements"` (si applicable)
- `#Region "Implémentation interface"` (si interface implémentée)

**Forms :**
- `#Region "Variables privées"`
- `#Region "Constructeur"`
- `#Region "Propriétés publiques"`
- `#Region "Initialisation"` (Load, InitializeComponent, InitializeToolTips, etc.)
- `#Region "Actions boutons"` ou `#Region "Gestion événements UI"`
- `#Region "Validation"` (si logique de validation métier)
- `#Region "Helpers / Utilitaires"` (méthodes privées de support)
- `#Region "Implémentation IContextAwareForm"` (si applicable)

**UserControls :**
- `#Region "Variables privées"`
- `#Region "Constructeur"`
- `#Region "Propriétés publiques"`
- `#Region "Contexte UI"` (SetContext si IContextAwareUserControl)
- `#Region "Initialisation"` (Load, InitializeToolTips, etc.)
- `#Region "Chargement des données"` (appels aux services métier)
- `#Region "Gestion événements UI"` (handlers de clic, selection changed, etc.)
- `#Region "Validation"` (si logique de validation métier)
- `#Region "Actions utilisateur"` (méthodes appelées par les boutons)
- `#Region "Helpers / Utilitaires"` (méthodes privées de support)

**Interfaces :**
- `#Region "Méthodes"` (unique région pour déclarer les méthodes du contrat)

**Règle d'or :** Limiter à **5-8 régions maximum** par fichier pour éviter la sur-segmentation.

---

## Exemples concrets du projet Althéa

### Classe Manager (GestionParametres)
```vb
' -------------------------------------------------------------------------------------------------
' Classe     : GestionParametres
' Projet     : Althéa
' Version    : V1.0
' Date       : 15/05/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Gestionnaire métier pour la gestion des paramètres applicatifs.
' Centralise les opérations CRUD sur les paramètres et encapsule la logique métier.
'
' Responsabilités :
' - Charger les paramètres actifs depuis la base de données
' - Sauvegarder les modifications de paramètres
' - Valider les contraintes métier (valeur_min, valeur_max, longueur_max)
' - Gérer le filtrage par mode d'accès (Admin, SuperUser)
' - Journaliser les actions via GestionLog
'
' Remarques  :
' - Classe singleton (ou instanciée selon architecture)
' - Aucun accès direct aux contrôles UI (découplage présentation/métier)
' - Toute validation métier doit passer par cette classe
' - Thread-safe si utilisée dans un contexte multi-thread
'
' Dépendances :
' - DatabaseManager (accès base de données)
' - ParametreApplication (DTO)
' - GestionLog (journalisation)
'
' Imports    :
' - System.Data.SqlClient
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------
```

### Form modale (Login)
```vb
' -------------------------------------------------------------------------------------------------
' Form       : Login
' Projet     : Althéa
' Version    : V1.0
' Date       : 07/05/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale d'authentification permettant à l'utilisateur de se connecter à l'application.
'
' Responsabilités :
' - Afficher l'interface de saisie login/mot de passe
' - Valider les informations saisies (non vides)
' - Authentifier l'utilisateur via GestionUtilisateurs
' - Retourner l'utilisateur authentifié via la propriété AuthenticatedUser
' - Retourner DialogResult.OK si authentification réussie, Cancel sinon
' - Gérer l'affichage du mot de passe (masqué/visible)
' - Journaliser les tentatives de connexion (succès/échec)
'
' Remarques   :
' - Form modale ouverte au démarrage de l'application via Home.Load
' - Retourne DialogResult.OK si authentification réussie
' - L'utilisateur authentifié est exposé via la propriété publique AuthenticatedUser
' - Si l'utilisateur annule, Home se ferme (application se termine)
' - Utilise GestionUtilisateurs.Authentifier() pour la logique métier
'
' Dépendances :
' - GestionUtilisateurs (authentification)
' - UtilisateurApplication (DTO)
' - AuthenticationResult (résultat d'authentification)
' - UtilsButtons (thématisation)
' - GestionLog (journalisation)
'
' Imports     :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------
```

### UserControl (UC_AdminHome)
```vb
' -------------------------------------------------------------------------------------------------
' UserControl : UC_AdminHome
' Projet      : Althéa
' Version     : V1.0
' Date        : 11/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' UserControl d'accueil de l'espace Administration donnant accès aux différentes zones
' d'administration selon les droits de l'utilisateur (Admin/SuperUser).
'
' Responsabilités :
' - Afficher les boutons d'accès aux zones d'administration (Paramètres, Connexion DB, etc.)
' - Gérer l'état des boutons selon le rôle utilisateur (Admin/SuperUser)
' - Permettre l'élévation temporaire de privilèges
' - Permettre le retour au rôle de base
' - Naviguer vers les sous-UserControls d'administration via Home.NavigateToAdminView()
' - Mettre à jour l'affichage utilisateur dans Home après changement de rôle
'
' Remarques   :
' - Chargé dynamiquement dans le panneau central de Home via btnAdmin_Click
' - Implémente IContextAwareUserControl pour injection du contexte UI partagé
' - Aucun accès direct à la base de données
' - Navigation vers les sous-écrans via Home.NavigateToAdminView() (contexte hiérarchique)
' - Accès aux droits utilisateur via _context.UserSession
' - Retour à l'accueil via Home.NavigateToAccueil() après perte de privilèges
'
' Dépendances :
' - Home (navigation via méthodes publiques)
' - UserSession (gestion des rôles et élévation)
' - UtilisateurApplication (utilisateur authentifié)
' - UserControlContext (contexte UI injecté)
' - ConfigurationConnexion, UC_Parametres (sous-écrans d'administration)
' - ElevationAcces (élévation de privilèges)
' - UtilsButtons (thématisation)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : Pour injection du contexte UI partagé
'
' Imports     :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------
```

---

## Organisation en Régions

Les régions (`#Region` / `#End Region`) permettent de structurer et de regrouper logiquement le code dans un fichier.

### Régions standards suggérées

Les modèles présentés dans ce document utilisent des régions **suggérées** qui conviennent à la majorité des cas :

**Pour les Forms et UserControls :**
- `Variables privées et constantes`
- `Enumerations`
- `Constructeurs`
- `Contexte UI`
- `Initialisation`
- `Gestion des événements`
- `Méthodes privées`
- `ToolTips`

**Pour les Classes :**
- `Variables privées`
- `Constantes`
- `Enumerations`
- `Constructeurs`
- `Propriétés`
- `Méthodes publiques`
- `Méthodes privées`

### Adaptation des régions selon le contexte

**Les régions doivent être adaptées au contexte et au processus métier du fichier.**

Elles ne sont **pas figées** et peuvent être renommées ou réorganisées pour refléter :
- **Les responsabilités fonctionnelles** du fichier
- **Les processus métier** implémentés
- **La logique de traitement** spécifique

#### Exemples d'adaptations

**Fichiers de requêtes SQL / Data Access :**
```vb
#Region "Requêtes SELECT"
#Region "Requêtes INSERT"
#Region "Requêtes UPDATE"
#Region "Requêtes DELETE"
#Region "Autres requêtes"
```

**Fichiers de gestion de workflows :**
```vb
#Region "Initialisation du workflow"
#Region "Validation des étapes"
#Region "Transitions d'états"
#Region "Finalisation"
```

**Fichiers de traitement de fichiers :**
```vb
#Region "Lecture"
#Region "Parsing"
#Region "Validation"
#Region "Transformation"
#Region "Écriture"
```

**Fichiers de calculs ou rapports :**
```vb
#Region "Collecte des données"
#Region "Calculs intermédiaires"
#Region "Agrégation"
#Region "Formatage et export"
```

### Principes directeurs pour les régions

1. **Clarté avant convention** : Si renommer une région améliore la compréhension, faites-le
2. **Regroupement logique** : Grouper par **responsabilité** ou **processus**, pas seulement par type
3. **Éviter la sur-segmentation** : Ne pas créer de région pour 1-2 éléments isolés
4. **Cohérence dans un fichier** : Garder une logique claire et progressive
5. **Documentation** : Si les régions sont non-standard, ajouter un commentaire explicatif en en-tête

### Note importante

Les régions listées dans les modèles de ce document sont des **points de départ recommandés**. 
Elles doivent être ajustées intelligemment pour servir la lisibilité et l'organisation du code, 
pas appliquées de manière rigide.

---

## Recommandations finales

1. **Adapter le niveau de détail** au contexte :
   - En-têtes de fichiers : toujours complets
   - Membres publics : format complet
   - Membres privés simples : format simplifié acceptable

2. **Éviter la duplication** :
   - Ne pas répéter l'information déjà dans le code (nom, type)
   - Se concentrer sur le **pourquoi** et le **contexte**, pas le **quoi**

3. **Maintenir la cohérence** :
   - Utiliser les mêmes termes dans tout le projet (ex: "Journaliser" vs "Logger")
   - Respecter le format d'alignement (deux-points alignés)

4. **Actualiser la documentation** :
   - Mettre à jour la version/date lors de modifications significatives
   - Ajouter des remarques pour les évolutions importantes

5. **Régionalisation intelligente** :
   - Grouper par **responsabilité fonctionnelle** plutôt que par type strict
   - Éviter les régions avec 1-2 éléments (sauf constructeur unique)
   - Limiter à 5-8 régions maximum par fichier

---

**Ce document est la référence officielle pour la documentation du projet Althéa.**
**Toute nouvelle classe/form/module doit suivre ces standards.**

*Dernière mise à jour : 16/05/2026*



------

> **Contact** : ***Joëlle (Manou) - Les Artefacts de Manou***
>
> Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.
>
> - Site web P.Nguyen Duy: https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu`
> - GitHub privé: Althea https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea

------



[TOC]

