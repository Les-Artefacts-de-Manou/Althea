' -------------------------------------------------------------------------------------------------
' Classe   : ParametreApplication
' Projet   : Althéa
' Version  : V1.2.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un paramètre applicatif issu de la table technique tec_parametres.
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l’interface UC_Parametres.
' - Les paramètres sont regroupés, typés et éventuellement modifiables par l’utilisateur.
' - V1.2.0 (16/10/2026) : titre et ordre du groupe (tec_groupes_parametres, migration v2.40.0),
'   pour les cartes de l'écran Paramètres.
' - V1.2.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ParametreApplication

#Region "Propriétés"

    ' Identifiant unique du paramètre (clé primaire)
    Public Property IdParametre As ULong

    ' Code technique du paramètre (généré automatiquement, non modifiable)
    Public Property CodeParametre As String

    ' Clé unique du paramètre (utilisée pour l'accès programmatique)
    Public Property CleParametre As String

    ' Libellé affiché dans l'interface utilisateur
    Public Property LibelleParametre As String

    ' Groupe de paramètres : code (tec_parametres.groupe_parametre), titre et ordre de sa carte
    ' dans l'écran Paramètres (tec_groupes_parametres, v2.40.0)
    Public Property GroupeParametre As String
    Public Property LibelleGroupe As String
    Public Property OrdreGroupe As Integer

    ' Type de valeur (Texte, Entier, Booléen, etc.)
    Public Property TypeValeur As String

    ' Valeur du paramètre (stockée en String, convertie selon TypeValeur)
    Public Property ValeurParametre As String

    ' Bornes des valeurs numériques (INT / DECIMAL) ; Nothing = sans borne (v2.37.0)
    Public Property ValeurMin As Decimal?
    Public Property ValeurMax As Decimal?

    ' Description détaillée du paramètre
    Public Property DescriptionParametre As String

    ' Indique si le paramètre est modifiable par l'utilisateur (ou réservé Admin)
    Public Property ModifiableUtilisateur As Boolean

    ' Indique si le paramètre est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

#End Region
End Class


' -------------------------------------------------------------------------------------------------
' Enum     : ModeAccesParametres
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 29/04/2026
' Auteur   : Manou / Projet Althéa
'
' Rôle :
' Définit le mode d’accès utilisé pour la gestion des paramètres applicatifs.
'
' Valeurs :
' - Admin     : accès complet de maintenance
' - SuperUser : accès limité aux paramètres modifiables par l’utilisatrice
' -------------------------------------------------------------------------------------------------
Public Enum ModeAccesParametres
        Admin = 1
        SuperUser = 2
    End Enum


