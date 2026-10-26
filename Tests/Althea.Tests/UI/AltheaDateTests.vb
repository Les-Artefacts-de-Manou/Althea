' -------------------------------------------------------------------------------------------------
' Classe   : AltheaDateTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie l'interprète de saisie du champ de date (AltheaDate.Interpreter, Friend) : formats
' acceptés, année sur deux chiffres, dates impossibles.
'
' Remarques :
' - Année sur deux chiffres : siècle actuel, sauf si la date tombe à plus de 20 ans dans le futur.
'   Les cas choisis ne dépendent pas de l'année en cours (sauf « jj/mm », calculé dans le test).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class AltheaDateTests

    <Theory>
    <InlineData("30/07/1985")>
    <InlineData("30/7/1985")>
    <InlineData("30/07/85")>
    <InlineData("300785")>
    <InlineData("30071985")>
    <InlineData("30-07-1985")>
    <InlineData("30.07.1985")>
    <InlineData(" 30/07/1985 ")>
    Public Sub Interpreter_FormatsAcceptes_MemeDate(saisie As String)
        Assert.Equal(New Date?(New Date(1985, 7, 30)), AltheaDate.Interpreter(saisie))
    End Sub

    <Fact>
    Public Sub Interpreter_JourEtMois_AnneeEnCours()
        Assert.Equal(New Date?(New Date(Date.Today.Year, 3, 15)), AltheaDate.Interpreter("15/03"))
    End Sub

    <Theory>
    <InlineData("01/01/30", 2030)>
    <InlineData("01/01/00", 2000)>
    <InlineData("01/01/85", 1985)>
    <InlineData("01/01/99", 1999)>
    Public Sub Interpreter_AnneeSurDeuxChiffres_SiecleSelonLaRegle(saisie As String, anneeAttendue As Integer)
        ' « 85 » donnerait 2085 : plus de 20 ans dans le futur, donc 1985.
        Assert.Equal(New Date?(New Date(anneeAttendue, 1, 1)), AltheaDate.Interpreter(saisie))
    End Sub

    <Theory>
    <InlineData("29/02/2024")>
    <InlineData("29/02/00")>
    Public Sub Interpreter_29FevrierAnneeBissextile_Accepte(saisie As String)
        Dim lue = AltheaDate.Interpreter(saisie)
        Assert.True(lue.HasValue)
        Assert.Equal(29, lue.Value.Day)
    End Sub

    <Theory>
    <InlineData("29/02/2023")>
    <InlineData("31/04/2026")>
    <InlineData("32/01/2026")>
    <InlineData("15/13/2026")>
    <InlineData("00/01/2026")>
    <InlineData("abc")>
    <InlineData("15/03/2026/1")>
    <InlineData("")>
    <InlineData("   ")>
    Public Sub Interpreter_DateImpossible_Nothing(saisie As String)
        Assert.Null(AltheaDate.Interpreter(saisie))
    End Sub

End Class
