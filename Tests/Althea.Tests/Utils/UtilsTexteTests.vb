' -------------------------------------------------------------------------------------------------
' Classe   : UtilsTexteTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie UtilsTexte : forme de comparaison d'un texte (sans accents ni majuscules) et découpage
' en mots, utilisés par les recherches.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UtilsTexteTests

    <Theory>
    <InlineData("  D'Hondt ", "d hondt")>
    <InlineData("D’Hondt", "d hondt")>
    <InlineData("Grâce-Hollogne", "grace hollogne")>
    <InlineData("ÉLODIE   Müller", "elodie muller")>
    <InlineData("", "")>
    <InlineData(Nothing, "")>
    Public Sub CleComparaison_SansAccentsNiMajuscules(texte As String, attendu As String)
        Assert.Equal(attendu, UtilsTexte.CleComparaison(texte))
    End Sub

    <Fact>
    Public Sub Mots_DecoupeSurEspacesTiretsVirgules()
        Assert.Equal(New String() {"dupont", "jean", "marie"}, UtilsTexte.Mots("Dupont, Jean-Marie"))
        Assert.Equal(New String() {"a", "b"}, UtilsTexte.Mots(" a ;; b "))
        Assert.Empty(UtilsTexte.Mots(Nothing))
    End Sub

End Class
