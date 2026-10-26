' -------------------------------------------------------------------------------------------------
' Classe   : PolitiqueMotDePasseTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie PolitiqueMotDePasse : longueur minimale (10 pour un compte, 12 pour les archives),
' présence d'une majuscule, d'une minuscule et d'un chiffre.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class PolitiqueMotDePasseTests

    <Theory>
    <InlineData("Abcdefghi1", True)>
    <InlineData("Abcdefgh1", False)>
    <InlineData("abcdefghi1", False)>
    <InlineData("ABCDEFGHI1", False)>
    <InlineData("Abcdefghij", False)>
    <InlineData("", False)>
    <InlineData(Nothing, False)>
    Public Sub EstConforme_RegleDuCompte(motDePasse As String, attendu As Boolean)
        Assert.Equal(attendu, PolitiqueMotDePasse.EstConforme(motDePasse))
    End Sub

    <Fact>
    Public Sub EstConforme_ArchivesDemandentDouzeCaracteres()
        Assert.False(PolitiqueMotDePasse.EstConforme("Abcdefghi1", PolitiqueMotDePasse.LongueurMinSauvegarde))
        Assert.True(PolitiqueMotDePasse.EstConforme("Abcdefghij12", PolitiqueMotDePasse.LongueurMinSauvegarde))
    End Sub

    <Fact>
    Public Sub Valider_MotDePasseConforme_AucuneErreur()
        Assert.Empty(PolitiqueMotDePasse.Valider("Abcdefghi1"))
    End Sub

    <Fact>
    Public Sub Valider_MotDePasseTropFaible_UneErreurSurLeChampMotDePasse()
        Dim erreurs = PolitiqueMotDePasse.Valider("faible")
        Dim erreur = Assert.Single(erreurs)
        Assert.Equal(PolitiqueMotDePasse.ChampMotDePasse, erreur.Champ)
        Assert.Contains(PolitiqueMotDePasse.Description(), erreur.Message, StringComparison.Ordinal)
    End Sub

    <Fact>
    Public Sub Description_AnnonceLaLongueurDemandee()
        Assert.StartsWith("Au moins 12 caractères", PolitiqueMotDePasse.Description(12), StringComparison.Ordinal)
    End Sub

End Class
