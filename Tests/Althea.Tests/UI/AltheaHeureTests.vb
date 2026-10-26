' -------------------------------------------------------------------------------------------------
' Classe   : AltheaHeureTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie l'interprète de saisie du champ d'heure (AltheaHeure.Interpreter, Friend), la mise
' dans la journée d'une heure calculée (Tronquer) et le placement automatique du « : »
' (DeuxPointsAttendus).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class AltheaHeureTests

    <Theory>
    <InlineData("9", 9, 0)>
    <InlineData("09", 9, 0)>
    <InlineData("930", 9, 30)>
    <InlineData("0930", 9, 30)>
    <InlineData("9:30", 9, 30)>
    <InlineData("9h30", 9, 30)>
    <InlineData("9H30", 9, 30)>
    <InlineData("9.30", 9, 30)>
    <InlineData("9h", 9, 0)>
    <InlineData("14:", 14, 0)>
    <InlineData("9:5", 9, 5)>
    <InlineData("0", 0, 0)>
    <InlineData("23:59", 23, 59)>
    <InlineData(" 1415 ", 14, 15)>
    Public Sub Interpreter_SaisiesAcceptees(saisie As String, heures As Integer, minutes As Integer)
        Assert.Equal(New TimeSpan?(New TimeSpan(heures, minutes, 0)), AltheaHeure.Interpreter(saisie))
    End Sub

    <Theory>
    <InlineData("24")>
    <InlineData("2400")>
    <InlineData("9:60")>
    <InlineData("960")>
    <InlineData("12345")>
    <InlineData(":30")>
    <InlineData("123:00")>
    <InlineData("9:300")>
    <InlineData("9:3:0")>
    <InlineData("9a")>
    <InlineData("")>
    Public Sub Interpreter_HeureImpossible_Nothing(saisie As String)
        Assert.Null(AltheaHeure.Interpreter(saisie))
    End Sub

    <Fact>
    Public Sub Tronquer_GardeHeuresEtMinutesDansLaJournee()
        Assert.Equal(New TimeSpan(9, 30, 0), AltheaHeure.Tronquer(New TimeSpan(9, 30, 45)))
        ' Fin calculée après minuit (début tardif + durée) : arrêtée à 23:59.
        Assert.Equal(New TimeSpan(23, 59, 0), AltheaHeure.Tronquer(New TimeSpan(1, 1, 30, 0)))
        Assert.Equal(TimeSpan.Zero, AltheaHeure.Tronquer(TimeSpan.FromMinutes(-10)))
    End Sub

    <Theory>
    <InlineData("9", True)>
    <InlineData("3", True)>
    <InlineData("2", False)>
    <InlineData("1", False)>
    <InlineData("14", True)>
    <InlineData("23", True)>
    <InlineData("24", False)>
    <InlineData("30", False)>
    <InlineData("123", False)>
    Public Sub DeuxPointsAttendus_QuandAucuneHeureNePeutSePoursuivre(chiffres As String, attendu As Boolean)
        ' « 9 » → « 9: » (aucune heure à deux chiffres ne commence par 3 à 9) ; « 14 » → « 14: ».
        Assert.Equal(attendu, AltheaHeure.DeuxPointsAttendus(chiffres))
    End Sub

End Class
