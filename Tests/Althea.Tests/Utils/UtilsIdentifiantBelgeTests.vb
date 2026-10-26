' -------------------------------------------------------------------------------------------------
' Classe   : UtilsIdentifiantBelgeTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie UtilsIdentifiantBelge : numéro INAMI, numéro d'entreprise (BCE), TVA, IBAN, BIC et code
' titulaire.
'
' Remarques :
' - Numéros fictifs à clé juste (calculée), sauf l'IBAN d'exemple de la norme (BE68 5390 0754 7034).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UtilsIdentifiantBelgeTests

    <Theory>
    <InlineData("11234578780")>
    <InlineData("1-12345-78-780")>
    Public Sub ValiderInami_CleJuste_Accepte(saisie As String)
        Dim message As String = Nothing
        Assert.True(UtilsIdentifiantBelge.ValiderInami(saisie, message), message)
    End Sub

    <Theory>
    <InlineData("11234579780")>
    <InlineData("1123457878")>
    <InlineData("1-12345-78-78X")>
    Public Sub ValiderInami_Incorrect_Refuse(saisie As String)
        Dim message As String = Nothing
        Assert.False(UtilsIdentifiantBelge.ValiderInami(saisie, message))
        Assert.NotEmpty(message)
    End Sub

    <Fact>
    Public Sub FormaterNumeroPersonneInami_GardeLesHuitPremiersChiffres()
        Assert.Equal("1-12345-78", UtilsIdentifiantBelge.FormaterNumeroPersonneInami("11234578780"))
        Assert.Null(UtilsIdentifiantBelge.FormaterNumeroPersonneInami("11234579780"))
        Assert.Null(UtilsIdentifiantBelge.FormaterNumeroPersonneInami(""))
    End Sub

    <Theory>
    <InlineData("0403.170.701")>
    <InlineData("0403170701")>
    <InlineData("403170701")>
    <InlineData("0850.845.396")>
    Public Sub ValiderBce_CleJuste_Accepte(saisie As String)
        ' Neuf chiffres : l'ancien format, sans le 0 initial, est accepté.
        Dim message As String = Nothing
        Assert.True(UtilsIdentifiantBelge.ValiderBce(saisie, message), message)
    End Sub

    <Theory>
    <InlineData("0403.170.702")>
    <InlineData("2403.170.701")>
    <InlineData("0403/170/701")>
    Public Sub ValiderBce_Incorrect_Refuse(saisie As String)
        Dim message As String = Nothing
        Assert.False(UtilsIdentifiantBelge.ValiderBce(saisie, message))
    End Sub

    <Fact>
    Public Sub ValiderTva_ExigeBeEtUnNumeroBceValide()
        Dim message As String = Nothing
        Assert.True(UtilsIdentifiantBelge.ValiderTva("BE 0403.170.701", message), message)
        Assert.True(UtilsIdentifiantBelge.ValiderTva("be0403170701", message), message)
        Assert.False(UtilsIdentifiantBelge.ValiderTva("0403.170.701", message))
        Assert.False(UtilsIdentifiantBelge.ValiderTva("BE0403.170.702", message))
        Assert.StartsWith("Le numéro de TVA est invalide", message, StringComparison.Ordinal)
    End Sub

    <Theory>
    <InlineData("INAMI", "11234578780", True)>
    <InlineData("inami", "11234579780", False)>
    <InlineData("BCE", "0403.170.701", True)>
    <InlineData("TVA", "BE0403170701", True)>
    <InlineData("AUTRE", "AB-12", True)>
    <InlineData("AUTRE", "12", False)>
    <InlineData("INAMI", "", True)>
    Public Sub Valider_ChoisitLeControleSelonLeType(codeType As String, saisie As String, attendu As Boolean)
        ' Pour un type non contrôlé par Althéa, c'est l'expression du référentiel qui s'applique.
        Dim message As String = Nothing
        Assert.Equal(attendu, UtilsIdentifiantBelge.Valider(codeType, "^[A-Z]{2}-[0-9]{2}$", saisie, message))
    End Sub

    <Fact>
    Public Sub Valider_ExpressionDuReferentielInvalide_NeBloquePasLaSaisie()
        Dim message As String = Nothing
        Assert.True(UtilsIdentifiantBelge.Valider("AUTRE", "[", "n'importe quoi", message))
    End Sub

    <Theory>
    <InlineData("BE68 5390 0754 7034")>
    <InlineData("be68539007547034")>
    <InlineData("FR14 2004 1010 0505 0001 3M02 606")>
    <InlineData("")>
    Public Sub ValiderIban_CleJuste_Accepte(saisie As String)
        Dim message As String = Nothing
        Assert.True(UtilsIdentifiantBelge.ValiderIban(saisie, message), message)
    End Sub

    <Theory>
    <InlineData("BE68 5390 0754 7035")>
    <InlineData("BE68 5390 0754 703")>
    <InlineData("6853 9007 5470 34")>
    Public Sub ValiderIban_Incorrect_Refuse(saisie As String)
        Dim message As String = Nothing
        Assert.False(UtilsIdentifiantBelge.ValiderIban(saisie, message))
        Assert.NotEmpty(message)
    End Sub

    <Fact>
    Public Sub FormaterIban_GroupesDeQuatre_EtFormeCompacte()
        Assert.Equal("BE68 5390 0754 7034", UtilsIdentifiantBelge.FormaterIban(" be68539007547034 "))
        Assert.Equal("FR14 2004 1010 0505 0001 3M02 606", UtilsIdentifiantBelge.FormaterIban("FR1420041010050500013M02606"))
        Assert.Equal("BE68539007547034", UtilsIdentifiantBelge.IbanCompact("BE68 5390 0754 7034"))
    End Sub

    <Theory>
    <InlineData("GEBABEBB", True)>
    <InlineData("gkcc bebb", True)>
    <InlineData("GEBABEBB123", True)>
    <InlineData("GEBABE", False)>
    <InlineData("GEBABEBB12", False)>
    <InlineData("", True)>
    Public Sub ValiderBic_HuitOuOnzeCaracteres(saisie As String, attendu As Boolean)
        Dim message As String = Nothing
        Assert.Equal(attendu, UtilsIdentifiantBelge.ValiderBic(saisie, message))
    End Sub

    <Theory>
    <InlineData("110", True)>
    <InlineData(" 130 ", True)>
    <InlineData("", True)>
    <InlineData("11", False)>
    <InlineData("1100", False)>
    <InlineData("1A0", False)>
    Public Sub ValiderCodeTitulaire_TroisChiffres(saisie As String, attendu As Boolean)
        Dim message As String = Nothing
        Assert.Equal(attendu, UtilsIdentifiantBelge.ValiderCodeTitulaire(saisie, message))
    End Sub

End Class
