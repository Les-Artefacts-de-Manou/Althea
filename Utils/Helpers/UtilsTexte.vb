' -------------------------------------------------------------------------------------------------
' Module      : UtilsTexte
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Forme de comparaison d'un texte saisi (nom, localité…) : ce que l'utilisatrice tape et ce qui est
' enregistré se comparent sans tenir compte des accents, des majuscules ni de la ponctuation des
' noms composés.
'
' Remarques   :
' - Module statique sans dépendance UI ni base.
' - Utilisé par la recherche transversale (R4 lieux, R5 noms) et UtilsCodePostal.CleLocalite.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions

Public Module UtilsTexte

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CleComparaison
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Texte sans accents ni majuscules, traits d'union et apostrophes lus comme des espaces,
    '            espaces multiples réduits (« Grâce-Hollogne » et « grace hollogne », « D'Hondt » et
    '            « d hondt » ont la même clé) ; chaîne vide si rien n'est saisi.
    ' -------------------------------------------------------------------------------------------------
    Public Function CleComparaison(texte As String) As String

        Dim brut As String = If(texte, String.Empty).Trim()
        If brut.Length = 0 Then Return String.Empty

        Dim sb As New StringBuilder(brut.Length)
        For Each c As Char In brut.Normalize(NormalizationForm.FormD)
            If CharUnicodeInfo.GetUnicodeCategory(c) = UnicodeCategory.NonSpacingMark Then Continue For
            sb.Append(If(c = "-"c OrElse c = "'"c OrElse c = "’"c, " "c, Char.ToLowerInvariant(c)))
        Next

        Return Regex.Replace(sb.ToString(), "\s+", " ").Trim()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Mots
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Les mots d'un texte, sous leur forme de comparaison (CleComparaison), sans doublon de
    '            séparateur ; tableau vide si rien n'est saisi.
    ' -------------------------------------------------------------------------------------------------
    Public Function Mots(texte As String) As String()
        Return CleComparaison(If(texte, String.Empty).Replace(","c, " "c).Replace(";"c, " "c)).
            Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
    End Function

End Module
