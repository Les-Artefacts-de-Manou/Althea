' -------------------------------------------------------------------------------------------------
' Classe   : UtilsCodePostalTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie UtilsCodePostal : format du code postal selon le pays, formes de comparaison du code et
' de la localité.
'
' Remarques :
' - CleLocalite doit rester d'accord avec cle() du script generer_codes_postaux_be.py (liste des
'   codes postaux belges) : ces cas en fixent la règle.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UtilsCodePostalTests

    <Theory>
    <InlineData("1000", "BE", True)>
    <InlineData("9999", "Belgique", True)>
    <InlineData("0999", "BE", False)>
    <InlineData("100", "BE", False)>
    <InlineData("75001", "FR", True)>
    <InlineData("00123", "France", False)>
    <InlineData("99123", "FR", False)>
    <InlineData("L-1234", "LU", True)>
    <InlineData("l1234", "LU", True)>
    <InlineData("12345", "LU", False)>
    <InlineData("ABC-12", "Espagne", True)>
    <InlineData("", "BE", True)>
    Public Sub Valider_FormatSelonLePays(saisie As String, pays As String, attendu As Boolean)
        ' Pays sans règle : aucun contrôle.
        Dim message As String = Nothing
        Assert.Equal(attendu, UtilsCodePostal.Valider(saisie, pays, message))
    End Sub

    <Theory>
    <InlineData(" l-1234 ", "L-1234")>
    <InlineData("1 000", "1000")>
    <InlineData(Nothing, "")>
    Public Sub CleCodePostal_SansEspacesEnMajuscules(saisie As String, attendu As String)
        Assert.Equal(attendu, UtilsCodePostal.CleCodePostal(saisie))
    End Sub

    <Theory>
    <InlineData("Grâce-Hollogne", "grace hollogne")>
    <InlineData("Sint-Truiden", "st truiden")>
    <InlineData("St.-Truiden", "st truiden")>
    <InlineData("Saint-Hubert", "st hubert")>
    <InlineData("Sainte-Ode", "ste ode")>
    <InlineData("Ste-Ode", "ste ode")>
    <InlineData("Cœur  d'Alène", "coeur d alene")>
    <InlineData("", "")>
    Public Sub CleLocalite_RapprocheLesEcrituresDUnMemeLieu(saisie As String, attendu As String)
        Assert.Equal(attendu, UtilsCodePostal.CleLocalite(saisie))
    End Sub

End Class
