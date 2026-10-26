' -------------------------------------------------------------------------------------------------
' Classe   : Civilite
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 11/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente une civilité du référentiel ref_civilites (Madame, Monsieur, Mx…), proposée partout
' où une personne est saisie (praticien, patient, contact, thérapeute, intervenant).
'
' Remarques :
' - Aucun accès base de données ici.
' - AbreviationCivilite (facultative) est destinée aux courriers et documents (« Mme », « M. »).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Civilite

#Region "Propriétés"

    Public Property IdCivilite As ULong

    Public Property CodeCivilite As String

    Public Property LibelleCivilite As String

    ' Forme abrégée pour les courriers ; Nothing si non renseignée
    Public Property AbreviationCivilite As String

    Public Property EstSysteme As Boolean

    Public Property Actif As Boolean

    Public Property OrdreAffichage As Integer

#End Region

    Public Overrides Function ToString() As String
        Return LibelleCivilite
    End Function

End Class
