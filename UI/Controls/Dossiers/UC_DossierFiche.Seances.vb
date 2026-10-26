' -------------------------------------------------------------------------------------------------
' Classe      : UC_DossierFiche — Séances (fichier partiel)
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Onglet Séances du dossier : grille paginée et filtrée, résumé, création, modification,
' report, suppression, encaissement d'une séance et règles d'affichage (pastilles, quotas).
'
' Remarques   :
' - Audit C.5 (décision de Joëlle du 25/10/2026) : la fiche est répartie en fichiers partiels, un par
'   onglet ; cette région est déplacée telle quelle depuis UC_DossierFiche.vb (V1.23.2), sans
'   changement de comportement. Les procédures gardent leur en-tête et leur historique.
' - Champs, contexte et méthodes communes : UC_DossierFiche.vb ; contrôles : UC_DossierFiche.Designer.vb.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Partial Class UC_DossierFiche

#Region "Séances"

    ' Séances actuellement affichées dans la grille (source de données courante).
    Private _seances As List(Of SeanceListeItem)

    ' Axes du filtre cboFiltreSeance : AxeFiltreSeance (FiltresSeances, partagé avec la recherche
    ' des séances d'une période).

    ' Marge de la ligne « Aujourd'hui » (sous la dernière séance à venir) et hauteur des lignes des
    ' grilles à pastilles (séances, paiements), qui doit laisser la place aux pastilles.
    Private Const HAUTEUR_SEPARATEUR_JOUR As Integer = 24
    Private Const HAUTEUR_LIGNE_PASTILLES As Integer = 30

    ' Résumé cliquable au-dessus de la grille des séances (créé avec la grille).
    Private _resumeSeances As ResumeFiltre

    ' Garde-fou : la grille n'est initialisée (colonnes, styles) qu'une seule fois.
    Private _grilleSeancesInitialisee As Boolean = False

    ' Numéros de période correspondant aux items de cboFiltrePeriodeInami (0 = « Toutes les périodes »).
    Private ReadOnly _numerosPeriodesFiltre As New List(Of Integer)

    ' Garde-fou : la reconstruction du filtre de période déclenche SelectedIndexChanged, ce qui
    ' relancerait AppliquerFiltresSeances en pleine exécution.
    Private _rechargementFiltrePeriodes As Boolean = False

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserGrilleSeances
    ' Version   : V1.1.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Fige les propriétés de base de la grille des séances (une seule fois).
    '
    ' Remarques :
    ' - V1.1.0 : hauteur des lignes à pastilles ; création du résumé cliquable.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserGrilleSeances()

        If _grilleSeancesInitialisee Then
            Return
        End If

        UtilsDataGrid.InitDataGridBasique(dgvSeances)

        dgvSeances.AutoGenerateColumns = False
        dgvSeances.ReadOnly = True
        dgvSeances.MultiSelect = False
        dgvSeances.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvSeances.AllowUserToAddRows = False
        dgvSeances.AllowUserToDeleteRows = False

        ' Colonnes monétaires formatées et alignées à droite. Le montant prévu (séance à venir), le
        ' montant à régler et l'avoir ne sont pas des colonnes : ils se lisent dans la pastille
        ' « Paiement » (« Prévue · 60 € », « À régler · 60 € », « Reste · 40 € », « Avoir 10 € ») ; la
        ' colonne « Solde », qui les répétait, est retirée (22/10/2026, décision de Joëlle).
        For Each colonne As DataGridViewColumn In New DataGridViewColumn() {colSeanceTarif, colSeanceRegle}
            colonne.DefaultCellStyle.Format = "N2"
            colonne.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        Next

        ' Hauteur des lignes : place pour les pastilles.
        dgvSeances.RowTemplate.Height = HAUTEUR_LIGNE_PASTILLES

        ' Pagination (V1.9.0) : branchée avant les filtres, dont l'initialisation affiche déjà
        ' la liste ; tri par en-tête sur la liste complète ; place réservée à la ligne « Aujourd'hui ».
        pagSeances.Attacher(dgvSeances, "séance(s)")
        pagSeances.HauteurReservee = HAUTEUR_SEPARATEUR_JOUR

        ' Avant les filtres : leur initialisation déclenche déjà un premier affichage (et le résumé).
        _resumeSeances = New ResumeFiltre(pnlResumeSeances, cboFiltreSeance, AxeFiltreSeance.Toutes,
                                          "toutes les séances", "ces séances", AddressOf DefinirInfobulle)

        InitialiserFiltresSeances()

        _grilleSeancesInitialisee = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerSeances
    ' Version   : V1.0.0
    ' Date      : 14/09/2026
    '
    ' Rôle      :
    ' Recharge la grille des séances du dossier courant depuis la couche métier et met à jour
    ' l'état des boutons d'action.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerSeances()

        If _idDossier <= 0 Then
            _seances = Nothing
            pagSeances.DefinirSource(Nothing)
            RafraichirEtatBoutonsSeances()
            Return
        End If

        Try

            dgvSeances.AutoGenerateColumns = False

            _seances = GestionSeances.GetSeancesParDossier(_idDossier)
            RenseignerDatesReport()
            AppliquerFiltresSeances()

            ChargerPaiements()

            ' Le bandeau reflète l'état financier : montant dû et avoir peuvent changer après une
            ' opération sur les séances ou les paiements (annulation, report, encaissement).
            ChargerTarifEtMontantsDus()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement des séances (UC_DossierFiche, id={_idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des séances.")
            End If

        End Try

        RafraichirEtatBoutonsSeances()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserFiltresSeances
    ' Version   : V2.1.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Initialise le filtre de la grille des séances avec les axes métier utiles au suivi financier.
    '
    ' Remarques :
    ' - L'ordre des items est celui de AxeFiltreSeance.
    ' - V2.0.0 (21/10/2026, demandes de Joëlle) : « Aujourd'hui » et « Reportées » ajoutés ; ordre :
    '   temps, puis statut, puis argent ; chaque axe porte la couleur de sa pastille (ElementListePastille).
    ' - V2.1.0 (23/10/2026) : axes et couleurs dans FiltresSeances.RemplirListe (partagés avec la
    '   recherche des séances d'une période).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFiltresSeances()

        FiltresSeances.RemplirListe(cboFiltreSeance)

        ' Période inactive par défaut ; bornes préremplies sur l'année civile en cours
        dtpSeanceDu.Valeur = New Date(Date.Today.Year, 1, 1)
        dtpSeanceAu.Valeur = New Date(Date.Today.Year, 12, 31)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirFiltrePeriodesInami
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Reconstruit la liste déroulante permettant de n'afficher que les séances d'une période annuelle
    ' de quota INAMI (P1, P2, P3...).
    '
    ' Paramètres :
    ' - quota : État du quota du dossier, périodes comprises
    '
    ' Remarques :
    ' - Le filtre reste masqué hors domaine INAMI ou tant qu'une seule période existe : il n'y aurait
    '   alors rien à distinguer.
    ' - La sélection en cours est préservée si la période existe toujours après rechargement, afin de
    '   ne pas perdre le contexte de lecture de l'utilisateur après un ajout ou une modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirFiltrePeriodesInami(quota As QuotaInamiInfo)

        Dim afficher As Boolean = quota IsNot Nothing AndAlso
                                  quota.EstDomaineInami AndAlso
                                  quota.Periodes.Count > 1

        cboFiltrePeriodeInami.Visible = afficher

        If Not afficher Then
            _numerosPeriodesFiltre.Clear()
            cboFiltrePeriodeInami.Items.Clear()
            Return
        End If

        Dim numeroSelectionne As Integer = NumeroPeriodeFiltre()

        _rechargementFiltrePeriodes = True

        Try

            _numerosPeriodesFiltre.Clear()
            cboFiltrePeriodeInami.Items.Clear()

            _numerosPeriodesFiltre.Add(0)
            cboFiltrePeriodeInami.Items.Add("Toutes les périodes")

            For Each periode In quota.Periodes

                Dim suffixe As String = String.Empty
                If periode.Numero = quota.NumeroPeriodeCourante Then
                    suffixe = " (en cours)"
                ElseIf periode.DateDebut > Date.Today Then
                    suffixe = " (à venir)"
                End If

                _numerosPeriodesFiltre.Add(periode.Numero)
                cboFiltrePeriodeInami.Items.Add(
                    $"P{periode.Numero} : {periode.DateDebut:dd/MM/yy} au {periode.DateDernierJour:dd/MM/yy}{suffixe}")

            Next

            Dim index As Integer = _numerosPeriodesFiltre.IndexOf(numeroSelectionne)
            cboFiltrePeriodeInami.SelectedIndex = If(index >= 0, index, 0)

        Finally

            _rechargementFiltrePeriodes = False

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : NumeroPeriodeFiltre
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Retourne le numéro de période sélectionné dans le filtre, ou 0 si aucune restriction n'est active.
    ' -------------------------------------------------------------------------------------------------
    Private Function NumeroPeriodeFiltre() As Integer

        If Not cboFiltrePeriodeInami.Visible Then Return 0

        Dim index As Integer = cboFiltrePeriodeInami.SelectedIndex
        If index < 0 OrElse index >= _numerosPeriodesFiltre.Count Then Return 0

        Return _numerosPeriodesFiltre(index)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltresSeances
    ' Version   : V1.2.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Applique le texte de recherche et l'axe de filtre sélectionné à la grille des séances, puis
    ' réaffecte la source de données filtrée.
    '
    ' Remarques :
    ' - Le filtrage est purement visuel : la collection _seances reste la référence complète, ce qui
    '   évite de perdre des lignes lors d'un changement de filtre.
    ' - V1.1.0 (23/10/2026, recherche R1) : page de la séance ciblée (CiblerAuChargement) si elle est listée.
    ' - V1.2.0 (23/10/2026) : filtrage par axe dans FiltresSeances.Filtrer (même définition que la recherche).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltresSeances()

        If _seances Is Nothing Then
            pagSeances.DefinirSource(Nothing)
            AfficherQuotaInami(Nothing)
            MettreAJourResumeSeances()
            Return
        End If

        ' Le rang de chaque séance dans le quota INAMI dépend de l'ensemble des séances du dossier :
        ' il est donc recalculé sur la collection complète, avant tout filtrage visuel.
        Dim quota As QuotaInamiInfo = GestionQuotaInami.AppliquerRangsQuota(_seances)
        AfficherQuotaInami(quota)

        ' Hors circuit INAMI, les colonnes de quota resteraient vides sur toute la grille.
        colSeancePeriodeInami.Visible = quota.EstDomaineInami
        colSeanceQuotaInami.Visible = quota.EstDomaineInami

        RemplirFiltrePeriodesInami(quota)

        Dim seancesFiltrees = _seances.AsEnumerable()

        Dim periodeFiltree As Integer = NumeroPeriodeFiltre()
        If periodeFiltree > 0 Then
            seancesFiltrees = seancesFiltrees.Where(Function(s) s.NumeroPeriodeInami = periodeFiltree)
        End If

        Dim texte As String = txtRechercheSeance.Text.Trim()
        If Not String.IsNullOrEmpty(texte) Then
            seancesFiltrees = seancesFiltrees.Where(Function(s) CorrespondRechercheSeance(s, texte))
        End If

        ' Filtre par période (dates de séance incluses)
        If chkPeriodeSeance.Checked Then
            Dim du As Date = dtpSeanceDu.Valeur.Value
            Dim au As Date = dtpSeanceAu.Valeur.Value
            seancesFiltrees = seancesFiltrees.Where(Function(s) s.DateSeance.Date >= du AndAlso s.DateSeance.Date <= au)
        End If

        seancesFiltrees = FiltresSeances.Filtrer(seancesFiltrees, FiltresSeances.Axe(cboFiltreSeance))

        ' Tri par défaut : les séances les plus récentes en premier, car ce sont celles sur lesquelles
        ' l'utilisateur agit (encaissement, correction de statut).
        ' Ordre de l'écran (le plus récent en haut) : un éventuel tri par en-tête est oublié, ce qui
        ' rend la ligne « Aujourd'hui » à nouveau pertinente.
        UtilsDataGrid.ReinitialiserTri(dgvSeances)
        MettreAJourResumeSeances()

        Dim liste As List(Of SeanceListeItem) = seancesFiltrees.
            OrderByDescending(Function(s) s.DateSeance).
            ThenByDescending(Function(s) s.HeureDebut).
            ToList()

        ' La grille s'ouvre sur la page de la ligne « Aujourd'hui » (dernière séance à venir) :
        ' c'est là que l'on agit, même quand beaucoup de séances sont déjà planifiées.
        Dim indexAujourdhui As Integer = liste.FindIndex(Function(s) s.EstPassee) - 1
        Dim page As Integer = pagSeances.PageDeIndex(indexAujourdhui)

        ' Ouverture ciblée (recherche) : la page de la séance demandée, si elle est dans la liste.
        Dim indexCible As Integer = If(_idSeanceCible > 0, liste.FindIndex(Function(s) s.IdSeance = _idSeanceCible), -1)
        If indexCible >= 0 Then page = pagSeances.PageDeIndex(indexCible)

        pagSeances.DefinirSource(liste, page)

        RafraichirEtatBoutonsSeances()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondRechercheSeance
    ' Version  : V1.0.0
    ' Date     : 17/09/2026
    '
    ' Rôle     :
    ' Indique si une séance correspond au texte recherché (code, nature, statut, état de paiement
    ' ou date), en comparaison insensible à la casse.
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondRechercheSeance(seance As SeanceListeItem, texte As String) As Boolean

        Dim champs As String =
            String.Join(" ", {
                If(seance.CodeSeance, ""),
                If(seance.LibelleNatureSeance, ""),
                If(seance.LibelleStatutAffichage, ""),
                seance.PaiementAffichage,
                seance.DateSeance.ToString("dd/MM/yyyy"),
                seance.HorairesAffichage
            })

        Return champs.IndexOf(texte, StringComparison.OrdinalIgnoreCase) >= 0

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Événements : filtres de la grille des séances
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Réappliquent le filtrage dès que l'utilisateur modifie la recherche ou l'axe sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRechercheSeance_TextChanged(sender As Object, e As EventArgs) Handles txtRechercheSeance.TextChanged

        AppliquerFiltresSeances()

    End Sub

    ' Réapplique le filtrage des séances au changement d'axe de filtre.
    Private Sub cboFiltreSeance_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFiltreSeance.SelectedIndexChanged

        AppliquerFiltresSeances()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événements : filtre par période (séances et paiements)
    ' Rôle       : Active les dates quand la case est cochée et réapplique le filtrage.
    '              Une borne « du » postérieure à la borne « au » décale automatiquement l'autre borne.
    ' -------------------------------------------------------------------------------------------------
    Private Sub chkPeriodeSeance_CheckedChanged(sender As Object, e As EventArgs) Handles chkPeriodeSeance.CheckedChanged
        dtpSeanceDu.Enabled = chkPeriodeSeance.Checked
        dtpSeanceAu.Enabled = chkPeriodeSeance.Checked
        AppliquerFiltresSeances()
    End Sub

    Private Sub dtpSeancePeriode_ValeurChangee(sender As Object, e As EventArgs) Handles dtpSeanceDu.ValeurChangee, dtpSeanceAu.ValeurChangee
        AltheaDate.CorrigerBornes(dtpSeanceDu, dtpSeanceAu, sender)
        If chkPeriodeSeance.Checked Then AppliquerFiltresSeances()
    End Sub

    Private Sub chkPeriodePaiement_CheckedChanged(sender As Object, e As EventArgs) Handles chkPeriodePaiement.CheckedChanged
        dtpPaiementDu.Enabled = chkPeriodePaiement.Checked
        dtpPaiementAu.Enabled = chkPeriodePaiement.Checked
        AppliquerFiltresPaiements()
    End Sub

    Private Sub dtpPaiementPeriode_ValeurChangee(sender As Object, e As EventArgs) Handles dtpPaiementDu.ValeurChangee, dtpPaiementAu.ValeurChangee
        AltheaDate.CorrigerBornes(dtpPaiementDu, dtpPaiementAu, sender)
        If chkPeriodePaiement.Checked Then AppliquerFiltresPaiements()
    End Sub

    ' Réapplique le filtrage des séances au changement de période INAMI (ignoré pendant la reconstruction de la liste).
    Private Sub cboFiltrePeriodeInami_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFiltrePeriodeInami.SelectedIndexChanged

        ' Pendant la reconstruction de la liste, la sélection change sans action de l'utilisateur :
        ' réappliquer les filtres ici provoquerait une récursion.
        If _rechargementFiltrePeriodes Then Return

        AppliquerFiltresSeances()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvSeances_CellFormatting
    ' Version   : V2.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Mise en forme des cellules ordinaires : date avec le jour (« ven. 17/10/2026 »), réglé vide
    ' quand rien n'est payé, solde signé et coloré (− dû en rose foncé, + avoir en mauve), lignes non
    ' dues atténuées. Statut et paiement sont dessinés en pastilles (dgvSeances_CellPainting).
    '
    ' Remarques :
    ' - V2.0.0 : remplace les couleurs posées sur les colonnes Prévisionnel / Exigible / Paiement
    '   (maquette « Grille des séances » du 17/10/2026).
    ' - Les couleurs viennent de UITheme, comme les autres grilles.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvSeances_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) _
        Handles dgvSeances.CellFormatting

        If e.RowIndex < 0 Then Return

        Dim seance As SeanceListeItem = TryCast(dgvSeances.Rows(e.RowIndex).DataBoundItem, SeanceListeItem)
        If seance Is Nothing Then Return

        ' Ligne non due (annulée, malade, reportée…) : texte atténué, l'œil passe à ce qui compte.
        If seance.FamilleStatut = FamilleStatutSeance.NonDue Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

        If e.ColumnIndex = colSeanceDate.Index Then
            e.Value = $"{seance.DateSeance:ddd} {seance.DateSeance.ToShortDateString()}"
            e.FormattingApplied = True

        ElseIf e.ColumnIndex = colSeanceRegle.Index AndAlso seance.MontantRegle = 0D Then
            e.Value = String.Empty
            e.FormattingApplied = True
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvSeances_CellPainting
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Dessine les pastilles « Statut » (avec la mention de report) et « Paiement », colorées par
    ' famille (UtilsPastilles) ; atténuées sur une ligne non due.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvSeances_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) _
        Handles dgvSeances.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If e.ColumnIndex <> colSeanceStatut.Index AndAlso e.ColumnIndex <> colSeanceEtatPaiement.Index Then Return

        Dim seance As SeanceListeItem = TryCast(dgvSeances.Rows(e.RowIndex).DataBoundItem, SeanceListeItem)
        If seance Is Nothing Then Return

        Dim attenuee As Boolean = seance.FamilleStatut = FamilleStatutSeance.NonDue

        If e.ColumnIndex = colSeanceStatut.Index Then
            UtilsDataGrid.DessinerPastilleCellule(dgvSeances, e, seance.LibelleStatutAffichage,
                                                  UtilsPastilles.Teinte(seance.FamilleStatut),
                                                  seance.MentionStatut, attenuee)
        Else
            UtilsDataGrid.DessinerPastilleCellule(dgvSeances, e, seance.PaiementAffichage,
                                                  UtilsPastilles.Teinte(seance.FamillePaiement),
                                                  Nothing, attenuee)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvSeances_DataBindingComplete
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Place la ligne « Aujourd'hui » : une marge sous la dernière séance à venir, quand la grille
    ' est dans l'ordre de l'écran (plus récent en haut) et montre à la fois des séances à venir et
    ' des séances passées. Après un tri par en-tête, plus de séparateur (il n'aurait plus de sens).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvSeances_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) _
        Handles dgvSeances.DataBindingComplete

        Dim indexSeparateur As Integer = -1

        If Not UtilsDataGrid.TriUtilisateurActif(dgvSeances) Then
            For i As Integer = 0 To dgvSeances.Rows.Count - 2
                Dim actuelle = TryCast(dgvSeances.Rows(i).DataBoundItem, SeanceListeItem)
                Dim suivante = TryCast(dgvSeances.Rows(i + 1).DataBoundItem, SeanceListeItem)
                If actuelle IsNot Nothing AndAlso suivante IsNot Nothing AndAlso
                   Not actuelle.EstPassee AndAlso suivante.EstPassee Then
                    indexSeparateur = i
                    Exit For
                End If
            Next
        End If

        For i As Integer = 0 To dgvSeances.Rows.Count - 1
            dgvSeances.Rows(i).DividerHeight = If(i = indexSeparateur, HAUTEUR_SEPARATEUR_JOUR, 0)
            dgvSeances.Rows(i).Height = HAUTEUR_LIGNE_PASTILLES + dgvSeances.Rows(i).DividerHeight
        Next

    End Sub

    Private Sub dgvSeances_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) _
        Handles dgvSeances.RowPostPaint

        UtilsDataGrid.DessinerSeparateurSousLigne(e, dgvSeances, $"Aujourd'hui — {Date.Today:dddd d MMMM yyyy}")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvSeances_CellToolTipTextNeeded
    ' Rôle      : Infobulle de ligne : code de la séance, tarif appliqué et, pour un domaine sous quota,
    '             période et rang dans le quota (informations retirées des colonnes le 17/10/2026).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvSeances_CellToolTipTextNeeded(sender As Object, e As DataGridViewCellToolTipTextNeededEventArgs) _
        Handles dgvSeances.CellToolTipTextNeeded

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        Dim seance As SeanceListeItem = TryCast(dgvSeances.Rows(e.RowIndex).DataBoundItem, SeanceListeItem)
        If seance Is Nothing Then Return

        Dim lignes As New List(Of String) From {$"Séance {seance.CodeSeance}"}
        If Not String.IsNullOrWhiteSpace(seance.LibellePrixSeance) Then lignes.Add($"Tarif : {seance.LibellePrixSeance}")
        If Not String.IsNullOrEmpty(seance.LibellePeriodeInami) Then lignes.Add($"Période {seance.LibellePeriodeInami} · quota {seance.LibelleQuotaInami}")
        If Not String.IsNullOrEmpty(seance.MentionStatut) Then lignes.Add(seance.MentionStatut)
        e.ToolTipText = String.Join(Environment.NewLine, lignes)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RenseignerDatesReport
    ' Rôle      : Pour chaque séance reportée, retrouve dans la liste du dossier la séance qui la remplace
    '             et note sa date et son code (mention « → 31/10 · S000045 » de la pastille de statut).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RenseignerDatesReport()

        If _seances Is Nothing Then Return

        Dim parId As Dictionary(Of Long, SeanceListeItem) = _seances.ToDictionary(Function(s) s.IdSeance)

        For Each seance In _seances
            seance.DateReport = Nothing
            seance.CodeSeanceReport = Nothing
        Next

        For Each remplacante In _seances.Where(Function(s) s.EstIssueDeReport)
            Dim origine As SeanceListeItem = Nothing
            If parId.TryGetValue(remplacante.IdSeanceOrigine.Value, origine) Then
                origine.DateReport = remplacante.DateSeance
                origine.CodeSeanceReport = remplacante.CodeSeance
            End If
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : SeanceSelectionnee
    ' Version  : V1.1.0
    ' Date     : 22/10/2026
    '
    ' Rôle     :
    ' Retourne la séance sélectionnée dans la grille par l'utilisatrice, ou Nothing.
    ' - V1.1.0 (22/10/2026) : la ligne courante posée d'office au chargement ne compte plus
    '   (UtilsDataGrid.ElementSelectionne).
    ' -------------------------------------------------------------------------------------------------
    Private Function SeanceSelectionnee() As SeanceListeItem
        Return UtilsDataGrid.ElementSelectionne(Of SeanceListeItem)(dgvSeances)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : EstDossierArchive
    ' Version  : V2.0.0
    ' Date     : 17/09/2026
    '
    ' Rôle     :
    ' Indique si le dossier courant est figé, c'est-à-dire fermé à toute création ou modification de
    ' séance et de paiement. Sont concernés les dossiers archivés (D-Q10) et les dossiers clôturés :
    ' dans les deux cas le suivi est terminé, l'onglet passe en lecture seule.
    '
    ' Remarques :
    ' - Un dossier « en pause » reste modifiable : le suivi est suspendu, pas terminé, et l'utilisateur
    '   doit pouvoir replanifier une séance au moment de la reprise.
    ' - L'absence de dossier chargé est traitée par les appelants (test sur _idDossier).
    ' -------------------------------------------------------------------------------------------------
    Private Function EstDossierArchive() As Boolean

        If _dossier Is Nothing Then
            Return False
        End If

        Return _dossier.EstArchive OrElse _dossier.EstCloture

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirEtatBoutonsSeances
    ' Version   : V1.2.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Active/désactive les boutons Nouvelle/Modifier/Supprimer selon le contexte (dossier chargé,
    ' non archivé) et la sélection courante.
    ' - V1.1.0 (20/10/2026) : raison de chaque bouton inactif, montrée au survol.
    ' - V1.2.0 (22/10/2026, audit C.2) : reporter, supprimer, encaisser : règles de SeanceListeItem.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirEtatBoutonsSeances()

        Dim raisonDossier As String = RaisonDossierFige()
        Dim seance As SeanceListeItem = SeanceSelectionnee()
        Dim selection As Boolean = seance IsNot Nothing

        btnNouvelleSeance.DefinirDisponibilite(raisonDossier.Length = 0, raisonDossier)
        btnModifierSeance.DefinirDisponibilite(selection, SANS_SELECTION_SEANCE)
        btnDetailPaiementsSeance.DefinirDisponibilite(selection, SANS_SELECTION_SEANCE)

        DefinirActionSeance(btnReporterSeance, raisonDossier, seance, selection AndAlso seance.EstReportable,
                            SEANCE_NON_REPORTABLE)
        DefinirActionSeance(btnSupprimerSeance, raisonDossier, seance, selection AndAlso seance.EstSupprimable,
                            "Une séance reportée reste dans le dossier : elle garde la trace du report.")
        DefinirActionSeance(btnEncaisserSeance, raisonDossier, seance, selection AndAlso seance.EstEncaissable,
                            "Seule une séance due et pas encore soldée peut être encaissée.")

    End Sub

    ' Message des boutons qui demandent une séance sélectionnée.
    Private Const SANS_SELECTION_SEANCE As String = "Sélectionnez une séance dans la liste."

    ' Raison d'un report impossible (bouton inactif et clic).
    Private Const SEANCE_NON_REPORTABLE As String = "Seule une séance planifiée peut être reportée."

    ' Action sur la séance sélectionnée : la première raison qui l'empêche (dossier figé, pas de
    ' sélection, condition propre à l'action) est montrée au survol du bouton inactif.
    Private Shared Sub DefinirActionSeance(bouton As AltheaBouton, raisonDossier As String, seance As SeanceListeItem,
                                           possible As Boolean, raisonImpossible As String)

        Dim raison As String = If(raisonDossier.Length > 0, raisonDossier,
                               If(seance Is Nothing, SANS_SELECTION_SEANCE,
                                  If(possible, String.Empty, raisonImpossible)))
        bouton.DefinirDisponibilite(raison.Length = 0, raison)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : RaisonDossierFige
    ' Version  : V1.1.0
    ' Date     : 22/10/2026
    '
    ' Rôle     : Pourquoi le dossier n'accepte ni séance ni paiement (vide s'il les accepte), pour
    '            l'infobulle des boutons inactifs (relevé de Joëlle, 16/10/2026).
    ' - V1.1.0 (22/10/2026, audit C.1) : texte du métier (GestionDossiers.RaisonDossierFige), qui
    '   applique la même règle à l'écriture.
    ' -------------------------------------------------------------------------------------------------
    Private Function RaisonDossierFige() As String

        If _idDossier <= 0 Then Return "Enregistrez d'abord le dossier."
        If _dossier Is Nothing Then Return String.Empty

        Return GestionDossiers.RaisonDossierFige(_dossier.CodeStatutDossier, GestionDossiers.ElementDossier.SeancesEtPaiements)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvSeances_SelectionChanged
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Met à jour l'état des boutons d'action selon la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvSeances_SelectionChanged(sender As Object, e As EventArgs) Handles dgvSeances.SelectionChanged

        RafraichirEtatBoutonsSeances()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnDetailPaiementsSeance_Click
    ' Version    : V1.0.0
    ' Date       : 26/09/2026
    '
    ' Rôle       :
    ' Ouvre le détail (lecture seule) de tous les mouvements de paiement de la séance sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnDetailPaiementsSeance_Click(sender As Object, e As EventArgs) Handles btnDetailPaiementsSeance.Click

        Dim seance As SeanceListeItem = SeanceSelectionnee()

        If seance Is Nothing Then
            Return
        End If

        Try
            Dim paiements As List(Of PaiementListeItem) = GestionPaiements.GetPaiementsParSeance(seance.IdSeance)

            Using dlg As New DialogPaiementsSeance(seance, paiements)
                SignalerAction($"Séance {seance.CodeSeance} > Détail des paiements", $"Consultation des paiements de la séance {seance.CodeSeance}.")
                dlg.ShowDialog(Me)
            End Using

            RafraichirContexteNavigation()

        Catch ex As Exception
            DialogChoix.Information("Impossible de charger le détail des paiements de cette séance.", "Détail paiements")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvSeances_CellDoubleClick
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Ouvre la séance sélectionnée en édition (double-clic sur une ligne).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvSeances_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSeances.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        OuvrirSeanceSelectionnee()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnNouvelleSeance_Click
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Ouvre la modale de création d'une séance, puis recharge la liste si un enregistrement a eu lieu.
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNouvelleSeance_Click(sender As Object, e As EventArgs) Handles btnNouvelleSeance.Click

        If _idDossier <= 0 OrElse EstDossierArchive() Then
            Return
        End If

        Using editeur As New SeanceEditor(_idDossier, 0, False, _context?.FenetrePrincipale)

            editeur.SetContext(_context)
            SignalerAction("Nouvelle séance", $"Création d'une séance dans le dossier {_dossier?.CodeDossier}.")

            If editeur.ShowDialog(FindForm()) = DialogResult.OK Then
                ChargerSeances()
            Else
                _context?.SetStatus("Création de séance abandonnée.")
            End If

        End Using

        RafraichirContexteNavigation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnModifierSeance_Click
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Ouvre la séance sélectionnée en édition (bouton Modifier).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifierSeance_Click(sender As Object, e As EventArgs) Handles btnModifierSeance.Click

        OuvrirSeanceSelectionnee()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnReporterSeance_Click
    ' Version    : V1.1.1
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Reporte la séance sélectionnée : après saisie de la nouvelle date, la séance d'origine est
    ' marquée « Reportée » (et son dû annulé) et une nouvelle séance planifiée est créée.
    '
    ' Remarques  :
    ' - V1.1.0 (21/10/2026) : le report est d'un seul tenant (GestionSeances.ReporterSeance V2.0.0) ;
    '   un refus métier s'affiche tel quel, un échec annonce que rien n'a été modifié.
    ' - V1.1.1 (22/10/2026, audit C.2) : règle lue dans SeanceListeItem.EstReportable ; le message
    '   (« déjà reportée ou annulée ») ne disait pas la règle réelle (seule une planifiée se reporte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReporterSeance_Click(sender As Object, e As EventArgs) Handles btnReporterSeance.Click

        If _idDossier <= 0 OrElse EstDossierArchive() Then
            Return
        End If

        Dim seance As SeanceListeItem = SeanceSelectionnee()

        If seance Is Nothing Then
            Return
        End If

        If Not seance.EstReportable Then
            DialogChoix.Information(SEANCE_NON_REPORTABLE)
            Return
        End If

        Dim dateSuggeree As Date = seance.DateSeance.AddDays(7)

        If dateSuggeree < Date.Today Then
            dateSuggeree = Date.Today.AddDays(7)
        End If

        Using dlg As New DialogReportSeance(dateSuggeree, seance.HeureDebut, seance.HeureFin, seance.CodeSeance)

            SignalerAction($"Séance {seance.CodeSeance} > Report", $"Report de la séance {seance.CodeSeance} du {seance.DateSeance.ToShortDateString()}.")

            If dlg.ShowDialog(FindForm()) <> DialogResult.OK Then
                RafraichirContexteNavigation()
                _context?.SetStatus("Report de séance abandonné.")
                Return
            End If

            RafraichirContexteNavigation()

            ' Seul le report lui-même est dans ce Try : les messages « rien n'a été modifié » ne doivent
            ' viser que lui (la suite a sa propre gestion d'erreur).
            Dim nouvelIdSeance As Long

            Try

                nouvelIdSeance = GestionSeances.ReporterSeance(seance.IdSeance, dlg.NouvelleDate, dlg.NouvelleHeureDebut, dlg.NouvelleHeureFin)

            Catch exRefus As InvalidOperationException

                ' Refus métier (message rédigé pour l'utilisatrice).
                DialogChoix.Information(exRefus.Message, "Report impossible")
                _context?.SetStatus("Report de séance refusé.")
                Return

            Catch ex As Exception

                GestionLog.EcrireLog(
                    $"Erreur report séance (UC_DossierFiche, idDossier={_idDossier}, idSeance={seance.IdSeance}).",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.UI,
                    ex
                )

                DialogChoix.Erreur(
                    "Le report n'a pas pu être enregistré ; rien n'a été modifié (la séance reste planifiée)." & Environment.NewLine &
                    "Vérifiez la connexion à la base de données, puis recommencez.",
                    "Report impossible")
                _context?.SetStatus("Erreur lors du report de la séance.")
                Return

            End Try

            ' Si la séance d'origine avait déjà été encaissée, son encaissement est devenu un avoir :
            ' proposer de l'utiliser pour la nouvelle séance (présélectionnée).
            ProposerUtilisationAvoirApresReport(seance.IdSeance, nouvelIdSeance, "Reportée")

            _context?.SetStatus($"Séance {seance.CodeSeance} reportée au {dlg.NouvelleDate.ToShortDateString()}.")

            ChargerSeances()

            If _paiements IsNot Nothing OrElse dgvPaiements.DataSource IsNot Nothing Then
                ChargerPaiements()
            End If

        End Using

    End Sub
    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirSeanceSelectionnee
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre la séance sélectionnée dans la modale d'édition (lecture seule si dossier archivé),
    ' puis recharge la liste après un enregistrement.
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirSeanceSelectionnee()

        Dim seance As SeanceListeItem = SeanceSelectionnee()

        If seance Is Nothing Then
            Return
        End If

        Using editeur As New SeanceEditor(_idDossier, seance.IdSeance, EstDossierArchive(), _context?.FenetrePrincipale)

            editeur.SetContext(_context)

            Dim libelleAction As String = If(EstDossierArchive(), "Consultation", "Modification")
            SignalerAction($"Séance {seance.CodeSeance} > {libelleAction}",
                           $"{libelleAction} de la séance {seance.CodeSeance} du {seance.DateSeance.ToShortDateString()}.")

            If editeur.ShowDialog(FindForm()) = DialogResult.OK Then
                ChargerSeances()
            End If

        End Using

        RafraichirContexteNavigation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnSupprimerSeance_Click
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Supprime la séance sélectionnée après confirmation, puis recharge la liste.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSupprimerSeance_Click(sender As Object, e As EventArgs) Handles btnSupprimerSeance.Click

        If _idDossier <= 0 OrElse EstDossierArchive() Then
            Return
        End If

        Dim seance As SeanceListeItem = SeanceSelectionnee()

        If seance Is Nothing Then
            Return
        End If

        Dim confirmation As String =
            $"Confirmez-vous la suppression définitive de la séance {seance.CodeSeance} du {seance.DateSeance.ToShortDateString()} ?"

        SignalerAction($"Séance {seance.CodeSeance} > Suppression", $"Suppression de la séance {seance.CodeSeance} : confirmation demandée.")

        If DialogChoix.Confirmer(confirmation, "Supprimer la séance") <> DialogResult.Yes Then
            RafraichirContexteNavigation()
            _context?.SetStatus("Suppression de séance abandonnée.")
            Return
        End If

        RafraichirContexteNavigation()

        Try

            GestionSeances.DeleteSeance(seance.IdSeance)

            If _context IsNot Nothing Then
                _context.SetStatus($"Séance {seance.CodeSeance} supprimée.")
            End If

            ChargerSeances()

        Catch ex As InvalidOperationException

            DialogChoix.Information(ex.Message, "Suppression impossible")

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur suppression séance (UC_DossierFiche, idDossier={_idDossier}, idSeance={seance.IdSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors de la suppression de la séance.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ProposerUtilisationAvoirApresReport
    ' Version   : V2.0.1
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Après le report d'une séance encaissée, propose d'utiliser l'avoir ainsi créé pour régler une
    ' séance due du dossier, la séance de remplacement présélectionnée (DialogUtilisationAvoir).
    '
    ' Paramètres :
    ' - idSeanceOrigine      : Séance reportée (porte l'avoir)
    ' - idSeancePreselection : Séance de remplacement
    ' - libelleStatut        : Libellé affiché dans le message
    '
    ' Remarques  :
    ' - V2.0.0 (21/10/2026, audit B.2, décision de Joëlle) : ex-ProposerReaffectationPaiement ; l'avoir
    '   est utilisé au lieu de réaffecter l'encaissement. Sans séance due, l'avoir est conservé (plus
    '   de proposition de créer une séance). Déroulé commun avec la fenêtre Séance.
    ' - V2.0.1 (22/10/2026, audit B.5) : utilisateur transmis par son identifiant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ProposerUtilisationAvoirApresReport(idSeanceOrigine As Long,
                                                    idSeancePreselection As Long,
                                                    libelleStatut As String)

        DialogUtilisationAvoir.ProposerApresChangementStatut(FindForm(), _idDossier, idSeanceOrigine, libelleStatut,
                                                             idSeancePreselection, _context?.ContexteOperation)

    End Sub

#End Region

End Class
