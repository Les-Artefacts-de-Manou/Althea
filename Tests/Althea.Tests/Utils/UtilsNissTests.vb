' -------------------------------------------------------------------------------------------------
' Classe   : UtilsNissTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie UtilsNiss : validation des identifiants de sécurité sociale (Belgique, France,
' Luxembourg), forme enregistrée, affichage et déduction du type.
'
' Remarques :
' - Numéros fictifs, construits pour avoir une clé juste (ou volontairement fausse) : aucune
'   donnée réelle.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UtilsNissTests

    <Theory>
    <InlineData("85073003328")>
    <InlineData("85.07.30-033.28")>
    <InlineData("85 07 30 033 28")>
    <InlineData("05010100113")>
    Public Sub Valider_NissBelgeAvecBonneCle_Accepte(niss As String)
        Dim message As String = Nothing
        Assert.True(UtilsNiss.Valider(niss, UtilsNiss.TYPE_BE, message), message)
        Assert.Equal(String.Empty, message)
    End Sub

    <Theory>
    <InlineData("85073003329")>
    <InlineData("8507300332")>
    <InlineData("850730033281")>
    <InlineData("8507300332A")>
    Public Sub Valider_NissBelgeIncorrect_Refuse(niss As String)
        Dim message As String = Nothing
        Assert.False(UtilsNiss.Valider(niss, UtilsNiss.TYPE_BE, message))
        Assert.NotEmpty(message)
    End Sub

    <Theory>
    <InlineData("185077512345608")>
    <InlineData("1850775123456")>
    <InlineData("1 85 07 2A 033 033 84")>
    Public Sub Valider_NumeroFrancaisValide_Accepte(numero As String)
        Dim message As String = Nothing
        Assert.True(UtilsNiss.Valider(numero, UtilsNiss.TYPE_FR, message), message)
    End Sub

    <Theory>
    <InlineData("185077512345609")>
    <InlineData("585077512345608")>
    <InlineData("185072A03303385")>
    Public Sub Valider_NumeroFrancaisIncorrect_Refuse(numero As String)
        Dim message As String = Nothing
        Assert.False(UtilsNiss.Valider(numero, UtilsNiss.TYPE_FR, message))
    End Sub

    <Fact>
    Public Sub Valider_MatriculeLuxembourgeois_ControleLesDeuxChiffres()
        Dim message As String = Nothing
        Assert.True(UtilsNiss.Valider("1985073003353", UtilsNiss.TYPE_LU, message), message)
        Assert.False(UtilsNiss.Valider("1985073003363", UtilsNiss.TYPE_LU, message))
        Assert.False(UtilsNiss.Valider("1985073003354", UtilsNiss.TYPE_LU, message))
    End Sub

    <Theory>
    <InlineData(UtilsNiss.TYPE_BE)>
    <InlineData(UtilsNiss.TYPE_FR)>
    <InlineData(UtilsNiss.TYPE_LU)>
    <InlineData(UtilsNiss.TYPE_AUTRE)>
    Public Sub Valider_SaisieVide_Acceptee(typeId As String)
        Dim message As String = Nothing
        Assert.True(UtilsNiss.Valider("  ", typeId, message))
    End Sub

    <Fact>
    Public Sub Valider_TypeAutre_AccepteToutTexte()
        Dim message As String = Nothing
        Assert.True(UtilsNiss.Valider("X-123 libre", UtilsNiss.TYPE_AUTRE, message))
    End Sub

    <Theory>
    <InlineData("85.07.30-033.28", "85073003328")>
    <InlineData(" 1 85 07 2a 033 033 84 ", "185072A03303384")>
    <InlineData("", Nothing)>
    Public Sub Normaliser_RetireSeparateursEtMetEnMajuscules(saisie As String, attendu As String)
        Assert.Equal(attendu, UtilsNiss.Normaliser(saisie))
    End Sub

    <Theory>
    <InlineData("85073003328", UtilsNiss.TYPE_BE, "85.07.30-033.28")>
    <InlineData("185072A03303384", UtilsNiss.TYPE_FR, "1 85 07 2A 033 033 84")>
    <InlineData("1850775123456", UtilsNiss.TYPE_FR, "1 85 07 75 123 456")>
    <InlineData("1985073003353", UtilsNiss.TYPE_LU, "1985 07 30 033 53")>
    <InlineData(" 85073003329 ", UtilsNiss.TYPE_BE, "85073003329")>
    Public Sub Formater_AfficheSelonLeType_OuRendLaSaisie(saisie As String, typeId As String, attendu As String)
        Assert.Equal(attendu, UtilsNiss.Formater(saisie, typeId))
    End Sub

    <Theory>
    <InlineData("", UtilsNiss.TYPE_BE)>
    <InlineData("85.07.30-033.28", UtilsNiss.TYPE_BE)>
    <InlineData("185077512345608", UtilsNiss.TYPE_FR)>
    <InlineData("1985073003353", UtilsNiss.TYPE_LU)>
    <InlineData("1850775123456", UtilsNiss.TYPE_FR)>
    <InlineData("ABC123", UtilsNiss.TYPE_AUTRE)>
    Public Sub DeduireType_ReconnaitLeFormat(saisie As String, attendu As String)
        ' Un numéro français sans clé (13 caractères) a la longueur d'un matricule luxembourgeois :
        ' le Luxembourg, dont les deux chiffres de contrôle sont vérifiés, est essayé d'abord.
        Assert.Equal(attendu, UtilsNiss.DeduireType(saisie))
    End Sub

End Class
