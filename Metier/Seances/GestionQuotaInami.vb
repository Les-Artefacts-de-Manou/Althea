' -------------------------------------------------------------------------------------------------
' Module      : GestionQuotaInami
' Projet      : Althéa
' Version     : V3.1.0
' Date        : 26/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des quotas de prestations applicables aux séances d'un dossier.
'
' Responsabilités :
' - Calculer l'état du quota d'un dossier (compteurs par catégorie, période de référence)
' - Déterminer la catégorie de quota consommée par une nature de séance
' - Simuler l'ajout d'une séance pour anticiper un franchissement de plafond
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Toute la logique SQL est déléguée à QueryQuotaInami (catégorie d'une nature) ; les séances
'   viennent de GestionSeances, les règles de GestionQuotasInami.
' - Les exceptions sont journalisées via GestionLog puis propagées (Throw)
' - Assujettissement, catégories, plafonds, seuils et durée de période proviennent des
'   référentiels datés ; aucune catégorie ni plafond n'est codé en dur.
'
' Historique  :
' - V3.0.0 (22/10/2026, audit B.6) : UN seul calcul du quota (CalculerQuota, renouvellement de date
'   à date), utilisé par l'onglet Séances (AppliquerRangsQuota), l'aperçu du patient
'   (GetQuotaDossier) et l'alerte de la fenêtre Séance (SimulerAjoutSeance, à la date de la séance).
'   Le calcul SQL de la seule première période (SelectDateDebutPeriodeInami,
'   SelectCompteursQuotaInami) est supprimé.
' - V3.1.0 (26/10/2026, audit D-2, ADR-58, décision de Joëlle) : CalculerQuota reçoit la règle
'   en vigueur par une fonction (date → règle) au lieu de la lire en base au milieu du calcul ; il
'   devient Friend et testable sans base (Althea.Tests). Même calcul, même résultat.
'
' Architecture :
' UI → GestionQuotaInami → QueryQuotaInami / GestionQuotasInami → DatabaseManager → MariaDB
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionQuotaInami

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetQuotaDossier
    ' Version    : V3.0.1
    ' Date       : 26/10/2026
    '
    ' Rôle       :
    ' Calcule l'état du quota d'un dossier pour la période en cours aujourd'hui, avec le même calcul
    ' que l'onglet Séances (renouvellement de date à date, CalculerQuota).
    '
    ' Remarques  :
    ' - Tant qu'aucune séance consommante n'est encodée, DateDebutPeriode vaut Nothing ; la règle
    '   du jour est alors chargée pour permettre la simulation d'une première séance.
    ' - V3.0.0 (22/10/2026, audit B.6) : calculait en SQL la seule PREMIÈRE période (ouverte par la
    '   toute première séance) : dès la 2e période, l'aperçu du patient et l'alerte de la fenêtre
    '   Séance ne comptaient plus les bonnes séances. Les séances du dossier sont désormais lues puis
    '   numérotées comme dans l'onglet Séances.
    ' - V3.0.1 (26/10/2026, audit D-2) : la règle est fournie au calcul par ChargerRegle.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetQuotaDossier(idDossier As Long) As QuotaInamiInfo

        Try

            If Not DossierSoumisQuota(idDossier) Then
                Return New QuotaInamiInfo With {.EstDomaineInami = False}
            End If

            Return CalculerQuota(GestionSeances.GetSeancesParDossier(idDossier), Date.Today,
                                 Function(jour) ChargerRegle(idDossier, jour))

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetQuotaDossier.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' Le domaine du dossier est soumis à un quota (organisme du domaine, GestionTarification).
    Private Function DossierSoumisQuota(idDossier As Long) As Boolean

        Dim ctx As GestionTarification.ContexteTarifaireDossier =
            GestionTarification.GetContexteTarifaireDossier(idDossier)

        Return ctx IsNot Nothing AndAlso ctx.SoumisQuota

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetCategorieQuotaParNature
    ' Version    : V2.0.0
    ' Date       : 20/09/2026
    '
    ' Rôle       :
    ' Retourne la catégorie de quota consommée par une nature de séance dans le domaine du dossier
    ' (lia_natures_seance_domaines).
    '
    ' Retour     :
    ' - Identifiant de la catégorie, ou 0 si la nature ne consomme aucun quota dans ce domaine
    ' -------------------------------------------------------------------------------------------------
    Public Function GetCategorieQuotaParNature(idDossier As Long, idNatureSeance As Long?) As ULong

        If Not idNatureSeance.HasValue Then Return 0UL

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryQuotaInami.SelectCategorieQuotaParNatureDossier, conn)

                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)
                    cmd.Parameters.AddWithValue("@id_nature_seance", idNatureSeance.Value)

                    Dim valeur As Object = cmd.ExecuteScalar()

                    If valeur Is Nothing OrElse valeur Is DBNull.Value Then Return 0UL

                    Return Convert.ToUInt64(valeur)

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetCategorieQuotaParNature.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

#End Region

#Region "Calcul sur liste"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AppliquerRangsQuota
    ' Version    : V3.0.1
    ' Date       : 26/10/2026
    '
    ' Rôle       :
    ' Numérote les séances consommantes d'un dossier dans leur catégorie de quota (1re, 2e...) et
    ' retourne l'état du quota pour la période en cours aujourd'hui (onglet Séances).
    '
    ' Remarques  :
    ' - V3.0.0 (22/10/2026, audit B.6) : le calcul est partagé (CalculerQuota) avec GetQuotaDossier
    '   et SimulerAjoutSeance ; il n'y en a plus qu'un.
    ' - V3.0.1 (26/10/2026, audit D-2) : la règle est fournie au calcul par ChargerRegle.
    ' -------------------------------------------------------------------------------------------------
    Public Function AppliquerRangsQuota(seances As List(Of SeanceListeItem)) As QuotaInamiInfo

        If seances Is Nothing OrElse seances.Count = 0 Then Return New QuotaInamiInfo

        If Not seances.Any(Function(s) s.SoumisQuota) Then
            ' Remise à zéro des rangs, comme pour un dossier soumis (statut ou nature changés).
            For Each seance In seances
                seance.RangQuotaInami = 0
                seance.NumeroPeriodeInami = 0
            Next
            Return New QuotaInamiInfo With {.EstDomaineInami = False}
        End If

        Dim idDossier As Long = seances(0).IdDossier
        Return CalculerQuota(seances, Date.Today, Function(jour) ChargerRegle(idDossier, jour))

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CalculerQuota
    ' Version    : V2.0.0
    ' Date       : 26/10/2026
    '
    ' Rôle       :
    ' LE calcul du quota d'un dossier soumis : numérote les séances consommantes et retourne l'état
    ' de la période qui contient dateReference.
    '
    ' Paramètres :
    ' - seances       : séances du dossier (rangs et numéros de période renseignés au passage)
    ' - dateReference : date dont on veut la période (aujourd'hui, ou date d'une séance simulée)
    ' - regleA        : règle de quota en vigueur à une date (Nothing si aucune) : ChargerRegle
    '                   du dossier dans l'application, une règle fixe dans les tests
    '
    ' Remarques  :
    ' - La 1re séance consommante ouvre une période dont la durée est celle de la règle en vigueur
    '   à cette date ; la 1re séance survenant après l'échéance ouvre la période suivante.
    ' - Les séances non consommantes ne sont pas numérotées et n'ouvrent jamais une période.
    ' - Le traitement est chronologique : le rang reflète l'ordre réel des prestations.
    ' - Reprend le corps d'AppliquerRangsQuota V2.0.0 (20/09/2026), la date du jour devenant un
    '   paramètre.
    ' - V2.0.0 (26/10/2026, audit D-2, ADR-58) : la règle vient de regleA (plus de lecture en base
    '   dans le calcul) ; Friend, pour les tests automatisés (Althea.Tests).
    ' -------------------------------------------------------------------------------------------------
    Friend Function CalculerQuota(seances As List(Of SeanceListeItem), dateReference As Date,
                                  regleA As Func(Of Date, QuotaInamiRegle)) As QuotaInamiInfo

        Dim info As New QuotaInamiInfo With {.EstDomaineInami = True}

        ' Remise à zéro : une séance peut avoir changé de statut ou de nature depuis le dernier calcul.
        For Each seance In seances
            seance.RangQuotaInami = 0
            seance.NumeroPeriodeInami = 0
        Next

        Dim consommatrices = seances.
            Where(Function(s) s.ConsommeQuotaInami).
            OrderBy(Function(s) s.DateSeance).
            ThenBy(Function(s) If(s.HeureDebut.HasValue, s.HeureDebut.Value, TimeSpan.Zero)).
            ThenBy(Function(s) s.IdSeance).
            ToList()

        If consommatrices.Count = 0 Then
            info.Regle = regleA(dateReference.Date)
            Return info
        End If

        Dim periodeCourante As QuotaPeriodeInami = Nothing

        For Each seance In consommatrices

            If Not info.LibellesCategories.ContainsKey(seance.IdCategorieQuota) Then
                info.LibellesCategories(seance.IdCategorieQuota) = If(seance.LibelleCategorieQuota, String.Empty)
            End If

            ' Renouvellement de date à date : la séance hors période ouvre un nouveau cycle.
            If periodeCourante Is Nothing OrElse seance.DateSeance.Date >= periodeCourante.DateFin Then
                periodeCourante = OuvrirPeriode(info.Periodes.Count + 1, seance.DateSeance.Date, regleA)
                info.Periodes.Add(periodeCourante)
            End If

            seance.NumeroPeriodeInami = periodeCourante.Numero
            seance.RangQuotaInami = periodeCourante.Incrementer(seance.IdCategorieQuota)

        Next

        ' Les compteurs décrivent la période qui CONTIENT la date de référence ; à défaut la dernière
        ' ouverte avant elle, sinon la première (date antérieure à toute séance).
        Dim jour As Date = dateReference.Date

        Dim periodeAffichee As QuotaPeriodeInami =
            info.Periodes.FirstOrDefault(Function(p) jour >= p.DateDebut AndAlso jour < p.DateFin)

        If periodeAffichee Is Nothing Then
            periodeAffichee = info.Periodes.LastOrDefault(Function(p) p.DateDebut <= jour)
            If periodeAffichee Is Nothing Then periodeAffichee = info.Periodes.First()
        End If

        info.NumeroPeriodeCourante = periodeAffichee.Numero
        info.DateDebutPeriode = periodeAffichee.DateDebut
        info.Regle = periodeAffichee.Regle
        info.Compteurs = New Dictionary(Of ULong, Integer)(periodeAffichee.Compteurs)

        Return info

    End Function

    ' Ouvre une période : la règle en vigueur à son ouverture fixe sa durée et ses plafonds.
    Private Function OuvrirPeriode(numero As Integer, dateDebut As Date,
                                   regleA As Func(Of Date, QuotaInamiRegle)) As QuotaPeriodeInami

        Dim regle As QuotaInamiRegle = regleA(dateDebut)

        Return New QuotaPeriodeInami With {
            .Numero = numero,
            .DateDebut = dateDebut,
            .Regle = regle,
            .DureeMois = If(regle IsNot Nothing, regle.DureePeriodeMois, QuotaInamiInfo.DUREE_PERIODE_MOIS_DEFAUT)
        }

    End Function

#End Region

#Region "Paramétrage"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ChargerRegle
    '
    ' Rôle       :
    ' Charge la règle de quota paramétrée applicable au dossier à la date indiquée.
    '
    ' Remarques  :
    ' - L'absence de règle ne doit pas empêcher l'affichage d'un dossier : la durée de période
    '   retombe sur sa valeur de repli et aucun plafond n'est supposé. L'anomalie est journalisée.
    ' - Une erreur d'accès au référentiel est également absorbée : un quota indicatif vaut mieux
    '   qu'un dossier inaccessible.
    ' -------------------------------------------------------------------------------------------------
    Private Function ChargerRegle(idDossier As Long, dateReference As Date) As QuotaInamiRegle

        Try

            Dim regle As QuotaInamiRegle =
                GestionQuotasInami.GetRegleApplicableParDossier(idDossier, dateReference)

            If regle Is Nothing Then

                GestionLog.EcrireLog(
                    $"Aucune regle de quota parametree pour le dossier {idDossier} au {dateReference:dd/MM/yyyy} : duree de periode par defaut, aucun plafond.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Database
                )

            End If

            Return regle

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur de lecture du parametrage de quota (dossier {idDossier}) : duree de periode par defaut, aucun plafond.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Return Nothing

        End Try

    End Function

#End Region

#Region "Simulation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : SimulerAjoutSeance
    ' Version    : V3.0.1
    ' Date       : 26/10/2026
    '
    ' Rôle       :
    ' Produit l'état du quota tel qu'il serait APRÈS l'ajout d'une séance de la catégorie indiquée,
    ' à sa date : c'est la période qui contiendrait cette séance qui est décrite.
    '
    ' Paramètres :
    ' - idDossier        : dossier de la séance
    ' - dateSeance       : date (et heure de début, facultative) de la séance projetée
    ' - heureDebut       : heure de début (ordre des séances d'un même jour)
    ' - idCategorieQuota : catégorie consommée par la nouvelle séance (0 = hors quota)
    '
    ' Remarques  :
    ' - Ne modifie pas la base : la séance projetée est ajoutée à la liste lue, puis le calcul
    '   commun (CalculerQuota) est appliqué.
    ' - V3.0.0 (22/10/2026, audit B.6) : ajoutait la séance aux compteurs de la période d'aujourd'hui,
    '   calculée en SQL (première période seulement) ; une séance planifiée dans la période suivante
    '   était comptée dans la mauvaise.
    ' - V3.0.1 (26/10/2026, audit D-2) : la règle est fournie au calcul par ChargerRegle.
    ' -------------------------------------------------------------------------------------------------
    Public Function SimulerAjoutSeance(idDossier As Long, dateSeance As Date, heureDebut As TimeSpan?,
                                       idCategorieQuota As ULong) As QuotaInamiInfo

        Try

            If Not DossierSoumisQuota(idDossier) Then
                Return New QuotaInamiInfo With {.EstDomaineInami = False}
            End If

            Dim seances As List(Of SeanceListeItem) = GestionSeances.GetSeancesParDossier(idDossier)

            If idCategorieQuota <> 0UL Then
                seances.Add(New SeanceListeItem With {
                    .IdSeance = 0,
                    .IdDossier = idDossier,
                    .DateSeance = dateSeance.Date,
                    .HeureDebut = heureDebut,
                    .SoumisQuota = True,
                    .IdCategorieQuota = idCategorieQuota,
                    .StatutConsommeQuota = True
                })
            End If

            Return CalculerQuota(seances, dateSeance.Date, Function(jour) ChargerRegle(idDossier, jour))

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur SimulerAjoutSeance.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

#End Region

End Module
