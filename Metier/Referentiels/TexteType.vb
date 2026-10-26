' -------------------------------------------------------------------------------------------------
' Classes    : TexteType, UsageTexteType
' Projet     : Althéa
' Version    : V1.1.0
' Date       : 19/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Modèles des textes types (ref_textes_types) et de leurs usages (ref_usages_textes_types) :
' phrases ou trames réutilisables insérées dans les éditeurs de texte (temps 2 de l'éditeur).
'
' Remarques  :
' - Aucun accès à la base ici (GestionTextesTypes).
' - Un texte sans usage (IdsUsages vide) est proposé dans tous les éditeurs.
' - Les codes des usages sont repris par le code (UsagesTexteType) : lignes système.
' - V1.1.0 (19/10/2026, v2.43.0) : plusieurs usages par texte (IdsUsages, LibellesUsages) ;
'   EstSysteme sur l'usage (écran de gestion des usages).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class TexteType

    Public Property IdTexteType As ULong
    Public Property CodeTexteType As String
    Public Property LibelleTexteType As String

    ' Usages du texte ; aucun = proposé partout.
    Public Property IdsUsages As New List(Of ULong)

    ' Libellés des usages, séparés par des virgules (grille) ; vide = partout.
    Public Property LibellesUsages As String

    ' Contenu : RTF (insertion avec mise en forme) et texte brut (recherche, repli).
    Public Property ContenuRtf As String
    Public Property ContenuTxt As String

    Public Property Actif As Boolean
    Public Property OrdreAffichage As Integer

End Class

Public Class UsageTexteType

    Public Property IdUsageTexteType As ULong
    Public Property CodeUsage As String
    Public Property LibelleUsage As String
    Public Property Actif As Boolean
    Public Property EstSysteme As Boolean
    Public Property OrdreAffichage As Integer

End Class
