' -------------------------------------------------------------------------------------------------
' Classe   : UtilsTelephoneTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie UtilsTelephone : pays reconnu par son nom, forme internationale enregistrée (E.164),
' affichage lisible et contrôle du nombre de chiffres.
'
' Remarques :
' - Numéros fictifs.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UtilsTelephoneTests

    <Theory>
    <InlineData("BE", "BE")>
    <InlineData("Belgique", "BE")>
    <InlineData("België", "BE")>
    <InlineData(" france ", "FR")>
    <InlineData("Lux", "LU")>
    <InlineData("Pays-Bas", "NL")>
    <InlineData("Deutschland", "DE")>
    Public Sub DeduirePays_ReconnaitCodeEtNoms(pays As String, attendu As String)
        Assert.Equal(attendu, UtilsTelephone.DeduirePays(pays))
    End Sub

    <Theory>
    <InlineData("Espagne")>
    <InlineData("")>
    Public Sub DeduirePays_PaysSansRegle_Nothing(pays As String)
        Assert.Null(UtilsTelephone.DeduirePays(pays))
    End Sub

    <Theory>
    <InlineData("0475 12 34 56", "BE", "+32475123456")>
    <InlineData("02/123.45.67", "Belgique", "+3221234567")>
    <InlineData("+32 475 12 34 56", "FR", "+32475123456")>
    <InlineData("0032 (0)475 12 34 56", "", "+32475123456")>
    <InlineData("06 12 34 56 78", "France", "+33612345678")>
    <InlineData("621 123 456", "LU", "+352621123456")>
    <InlineData("+352 621 123 456", "BE", "+352621123456")>
    <InlineData("+1 555 0100", "BE", "+15550100")>
    <InlineData("12 34 56", "Espagne", "12 34 56")>
    <InlineData("", "BE", "")>
    Public Sub NormaliserE164_FormeInternationale(saisie As String, pays As String, attendu As String)
        ' L'indicatif écrit dans la saisie l'emporte sur le pays ; le « 0 » national est retiré
        ' (sauf au Luxembourg, qui n'en a pas) ; pays inconnu : la saisie est rendue telle quelle.
        Assert.Equal(attendu, UtilsTelephone.NormaliserE164(saisie, pays))
    End Sub

    <Theory>
    <InlineData("0475123456", "BE", "+32 475 12 34 56")>
    <InlineData("02 123 45 67", "BE", "+32 2 123 45 67")>
    <InlineData("010 12 34 56", "BE", "+32 10 12 34 56")>
    <InlineData("0612345678", "FR", "+33 6 12 34 56 78")>
    <InlineData("621123456", "LU", "+352 621 123 456")>
    Public Sub FormaterAffichage_DecoupageLisible(saisie As String, pays As String, attendu As String)
        Assert.Equal(attendu, UtilsTelephone.FormaterAffichage(saisie, pays))
    End Sub

    <Theory>
    <InlineData("0475 12 34 56", "BE", True)>
    <InlineData("02 123 45 67", "BE", True)>
    <InlineData("0475 12 34", "BE", False)>
    <InlineData("0475 12 34 56 78", "BE", False)>
    <InlineData("06 12 34 56 7", "FR", False)>
    <InlineData("", "BE", True)>
    Public Sub Valider_NombreDeChiffresSelonLePays(saisie As String, pays As String, attendu As Boolean)
        Dim message As String = Nothing
        Assert.Equal(attendu, UtilsTelephone.Valider(saisie, pays, message))
        Assert.Equal(attendu, message.Length = 0)
    End Sub

End Class
