' -------------------------------------------------------------------------------------------------
' Classe   : PasswordSecurityHelperTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie PasswordSecurityHelper : sel aléatoire, empreinte PBKDF2-SHA256 (100 000 itérations),
' vérification d'un mot de passe.
'
' Remarques :
' - Empreinte_EstStablePourUnSelConnu fige le calcul : si les paramètres (algorithme, itérations,
'   taille) changeaient, aucun compte existant ne pourrait plus se connecter. Valeur attendue
'   calculée hors d'Althéa (Python, hashlib.pbkdf2_hmac).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class PasswordSecurityHelperTests

    ' Sel fixe (octets 0 à 31) et mot de passe de test : aucune donnée réelle.
    Private Const SelConnu As String = "AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8="
    Private Const MotDePasseTest As String = "Althea2026!"
    Private Const EmpreinteAttendue As String = "BegZrzizB5ChVbri5nDXGGTlvzFVYJQnIDASRXdn1uU="

    <Fact>
    Public Sub Empreinte_EstStablePourUnSelConnu()
        Assert.Equal(EmpreinteAttendue, PasswordSecurityHelper.HashPassword(MotDePasseTest, SelConnu))
    End Sub

    <Fact>
    Public Sub GenerateSalt_TrenteDeuxOctetsAleatoires()
        Dim sel1 = PasswordSecurityHelper.GenerateSalt()
        Dim sel2 = PasswordSecurityHelper.GenerateSalt()
        Assert.Equal(32, Convert.FromBase64String(sel1).Length)
        Assert.NotEqual(sel1, sel2)
    End Sub

    <Fact>
    Public Sub VerifyPassword_BonMotDePasse_Accepte()
        Assert.True(PasswordSecurityHelper.VerifyPassword(MotDePasseTest, EmpreinteAttendue, SelConnu))
    End Sub

    <Theory>
    <InlineData("althea2026!")>
    <InlineData("Althea2026")>
    <InlineData("")>
    Public Sub VerifyPassword_AutreMotDePasse_Refuse(motDePasse As String)
        Assert.False(PasswordSecurityHelper.VerifyPassword(motDePasse, EmpreinteAttendue, SelConnu))
    End Sub

    <Fact>
    Public Sub HashPassword_AutreSel_AutreEmpreinte()
        Dim autreSel = PasswordSecurityHelper.GenerateSalt()
        Assert.NotEqual(EmpreinteAttendue, PasswordSecurityHelper.HashPassword(MotDePasseTest, autreSel))
    End Sub

End Class
