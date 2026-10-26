' -------------------------------------------------------------------------------------------------
' Classe   : GestionQuotaInamiTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Vérifie le calcul du quota INAMI (GestionQuotaInami.CalculerQuota, Friend) : numérotation des
' séances consommantes par catégorie, renouvellement de date à date, période décrite, règle fixée
' à l'ouverture de chaque période.
'
' Remarques :
' - La règle est fournie par une fonction (date → règle) : aucun accès à la base.
' - Séances et catégories fictives (identifiants 1 et 2 pour deux catégories de quota).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class GestionQuotaInamiTests

    Private Const SOUTIEN As ULong = 1UL
    Private Const TRAITEMENT As ULong = 2UL

    ' Séance de test : soumise au quota, statut consommant par défaut.
    Private Shared Function Seance(idSeance As Long, jour As Date, categorie As ULong,
                                   Optional heure As TimeSpan? = Nothing,
                                   Optional consomme As Boolean = True) As SeanceListeItem
        Return New SeanceListeItem With {
            .IdSeance = idSeance,
            .IdDossier = 1,
            .DateSeance = jour,
            .HeureDebut = heure,
            .SoumisQuota = True,
            .IdCategorieQuota = categorie,
            .LibelleCategorieQuota = If(categorie = SOUTIEN, "Soutien", "Traitement"),
            .StatutConsommeQuota = consomme
        }
    End Function

    Private Shared Function Regle(dureeMois As Integer, Optional plafondSoutien As Integer = 10,
                                  Optional plafondTraitement As Integer = 20) As QuotaInamiRegle
        Dim r As New QuotaInamiRegle With {.DureePeriodeMois = dureeMois, .SeuilAlerteTotal = 25}
        r.Plafonds.Add(New QuotaPlafond With {.IdCategorieQuota = SOUTIEN, .LibelleCategorie = "Soutien", .Plafond = plafondSoutien, .OrdreAffichage = 1})
        r.Plafonds.Add(New QuotaPlafond With {.IdCategorieQuota = TRAITEMENT, .LibelleCategorie = "Traitement", .Plafond = plafondTraitement, .OrdreAffichage = 2})
        Return r
    End Function

    Private Shared Function RegleFixe(r As QuotaInamiRegle) As Func(Of Date, QuotaInamiRegle)
        Return Function(jour) r
    End Function

    <Fact>
    Public Sub CalculerQuota_NumeroteChronologiquementParCategorie()
        ' Séances données dans le désordre ; deux séances le même jour : l'heure départage.
        Dim s1 = Seance(1, #1/10/2026#, SOUTIEN, New TimeSpan(14, 0, 0))
        Dim s2 = Seance(2, #1/5/2026#, SOUTIEN)
        Dim s3 = Seance(3, #1/7/2026#, TRAITEMENT)
        Dim s4 = Seance(4, #1/10/2026#, SOUTIEN, New TimeSpan(9, 0, 0))
        Dim seances As New List(Of SeanceListeItem) From {s1, s2, s3, s4}

        Dim info = GestionQuotaInami.CalculerQuota(seances, #2/1/2026#, RegleFixe(Regle(12)))

        Assert.Equal(1, s2.RangQuotaInami)
        Assert.Equal(2, s4.RangQuotaInami)
        Assert.Equal(3, s1.RangQuotaInami)
        Assert.Equal(1, s3.RangQuotaInami)
        Assert.All(seances, Sub(s) Assert.Equal(1, s.NumeroPeriodeInami))
        Assert.Equal(3, info.NbDe(SOUTIEN))
        Assert.Equal(1, info.NbDe(TRAITEMENT))
        Assert.Equal(4, info.NbTotal)
        Assert.Equal(New Date?(#1/5/2026#), info.DateDebutPeriode)
        Assert.Equal("Soutien", info.LibellesCategories(SOUTIEN))
    End Sub

    <Fact>
    Public Sub CalculerQuota_SeanceNonConsommante_NiRangNiPeriode()
        Dim annulee = Seance(1, #1/5/2026#, SOUTIEN, consomme:=False)
        annulee.RangQuotaInami = 5
        annulee.NumeroPeriodeInami = 2
        Dim horsQuota = Seance(2, #1/6/2026#, 0UL)
        Dim tenue = Seance(3, #1/8/2026#, SOUTIEN)

        Dim info = GestionQuotaInami.CalculerQuota(New List(Of SeanceListeItem) From {annulee, horsQuota, tenue},
                                                   #2/1/2026#, RegleFixe(Regle(12)))

        ' Remise à zéro de l'ancien rang ; la période s'ouvre à la première séance consommante.
        Assert.Equal(0, annulee.RangQuotaInami)
        Assert.Equal(0, annulee.NumeroPeriodeInami)
        Assert.Equal(0, horsQuota.RangQuotaInami)
        Assert.Equal(1, tenue.RangQuotaInami)
        Assert.Equal(New Date?(#1/8/2026#), info.DateDebutPeriode)
    End Sub

    <Fact>
    Public Sub CalculerQuota_AucuneSeanceConsommante_RegleDuJourSansPeriode()
        Dim r = Regle(12)
        Dim info = GestionQuotaInami.CalculerQuota(New List(Of SeanceListeItem) From {Seance(1, #1/5/2026#, SOUTIEN, consomme:=False)},
                                                   #2/1/2026#, RegleFixe(r))

        Assert.True(info.EstDomaineInami)
        Assert.Same(r, info.Regle)
        Assert.Empty(info.Periodes)
        Assert.Equal(0, info.NumeroPeriodeCourante)
        Assert.Null(info.DateDebutPeriode)
    End Sub

    <Fact>
    Public Sub CalculerQuota_RenouvellementDeDateADate()
        ' Période de 12 mois ouverte le 15/03/2026 : fin exclusive le 15/03/2027. La séance du
        ' 14/03/2027 y compte encore ; la suivante (20/04/2027) ouvre la période 2 à SA date.
        Dim a = Seance(1, #3/15/2026#, SOUTIEN)
        Dim b = Seance(2, #3/14/2027#, SOUTIEN)
        Dim c = Seance(3, #4/20/2027#, SOUTIEN)
        Dim d = Seance(4, #5/2/2027#, SOUTIEN)

        Dim info = GestionQuotaInami.CalculerQuota(New List(Of SeanceListeItem) From {a, b, c, d},
                                                   #6/1/2027#, RegleFixe(Regle(12)))

        Assert.Equal(2, info.Periodes.Count)
        Assert.Equal(#4/20/2027#, info.Periodes(1).DateDebut)
        Assert.Equal({1, 1, 2, 2}, {a.NumeroPeriodeInami, b.NumeroPeriodeInami, c.NumeroPeriodeInami, d.NumeroPeriodeInami})
        Assert.Equal({1, 2, 1, 2}, {a.RangQuotaInami, b.RangQuotaInami, c.RangQuotaInami, d.RangQuotaInami})
        ' Le 01/06/2027 est dans la période 2 : ses compteurs sont décrits.
        Assert.Equal(2, info.NumeroPeriodeCourante)
        Assert.Equal(2, info.NbDe(SOUTIEN))
    End Sub

    <Theory>
    <InlineData(2026, 6, 1, 1)>
    <InlineData(2027, 4, 1, 1)>
    <InlineData(2027, 6, 1, 2)>
    <InlineData(2025, 12, 31, 1)>
    Public Sub CalculerQuota_DecritLaPeriodeQuiContientLaDate(annee As Integer, mois As Integer, jour As Integer, periodeAttendue As Integer)
        ' Entre deux périodes (01/04/2027) : la dernière ouverte avant ; avant toute séance : la première.
        Dim seances As New List(Of SeanceListeItem) From {
            Seance(1, #3/15/2026#, SOUTIEN), Seance(2, #4/20/2027#, SOUTIEN)}

        Dim info = GestionQuotaInami.CalculerQuota(seances, New Date(annee, mois, jour), RegleFixe(Regle(12)))

        Assert.Equal(periodeAttendue, info.NumeroPeriodeCourante)
    End Sub

    <Fact>
    Public Sub CalculerQuota_RegleEnVigueurAOuvertureFixeLaDuree()
        ' Règle de 6 mois jusqu'au 31/12/2026, de 12 mois ensuite : les périodes ouvertes en 2026
        ' durent 6 mois (01/02 → 01/08, 01/08 → 01/02/2027), celle ouverte le 15/02/2027 en dure 12.
        Dim regle6 = Regle(6)
        Dim regle12 = Regle(12)
        Dim regleA As Func(Of Date, QuotaInamiRegle) = Function(jour) If(jour.Year <= 2026, regle6, regle12)
        Dim seances As New List(Of SeanceListeItem) From {
            Seance(1, #2/1/2026#, SOUTIEN), Seance(2, #8/1/2026#, SOUTIEN), Seance(3, #2/15/2027#, SOUTIEN)}

        Dim info = GestionQuotaInami.CalculerQuota(seances, #2/20/2027#, regleA)

        Assert.Equal(3, info.Periodes.Count)
        Assert.Equal(#8/1/2026#, info.Periodes(0).DateFin)
        Assert.Same(regle6, info.Periodes(1).Regle)
        Assert.Same(regle12, info.Periodes(2).Regle)
        Assert.Equal(12, info.Periodes(2).DureeMois)
        Assert.Same(regle12, info.Regle)
    End Sub

    <Fact>
    Public Sub CalculerQuota_SansRegle_DureeDeRepliEtAucunPlafond()
        Dim info = GestionQuotaInami.CalculerQuota(New List(Of SeanceListeItem) From {Seance(1, #3/1/2026#, SOUTIEN)},
                                                   #4/1/2026#, Function(jour) Nothing)

        Assert.False(info.ARegle)
        Assert.Equal(QuotaInamiInfo.DUREE_PERIODE_MOIS_DEFAUT, info.Periodes(0).DureeMois)
        Assert.False(info.QuotaAtteint)
    End Sub

    <Fact>
    Public Sub CalculerQuota_CompteursFaceAuxPlafonds()
        Dim seances As New List(Of SeanceListeItem)
        For i As Integer = 1 To 3
            seances.Add(Seance(i, New Date(2026, 3, 1).AddDays(i), SOUTIEN))
        Next
        Dim r = Regle(12, plafondSoutien:=2, plafondTraitement:=1)

        Dim info = GestionQuotaInami.CalculerQuota(seances, #4/1/2026#, RegleFixe(r))

        ' VB ne distingue pas les majuscules : « etatSoutien » et non « soutien » (constante SOUTIEN).
        Dim etatSoutien = info.Categories.Single(Function(c) c.IdCategorieQuota = SOUTIEN)
        Assert.True(etatSoutien.Epuise)
        Assert.True(etatSoutien.Depasse)
        Assert.Equal("3 / 2", etatSoutien.LibelleCompteur)
        Assert.True(info.QuotaAtteint)
        Assert.False(info.QuotaDepasse)
    End Sub

    <Fact>
    Public Sub AppliquerRangsQuota_DossierNonSoumis_RangsRemisAZeroSansCalcul()
        Dim s = Seance(1, #3/1/2026#, SOUTIEN)
        s.SoumisQuota = False
        s.RangQuotaInami = 4

        Dim info = GestionQuotaInami.AppliquerRangsQuota(New List(Of SeanceListeItem) From {s})

        Assert.False(info.EstDomaineInami)
        Assert.Equal(0, s.RangQuotaInami)
    End Sub

    <Theory>
    <InlineData(True, 1UL, True, True)>
    <InlineData(False, 1UL, True, False)>
    <InlineData(True, 0UL, True, False)>
    <InlineData(True, 1UL, False, False)>
    Public Sub ConsommeQuotaInami_SoumisCategorieEtStatut(soumis As Boolean, categorie As ULong, statutConsomme As Boolean, attendu As Boolean)
        Dim s As New SeanceListeItem With {.SoumisQuota = soumis, .IdCategorieQuota = categorie, .StatutConsommeQuota = statutConsomme}
        Assert.Equal(attendu, s.ConsommeQuotaInami)
    End Sub

End Class
