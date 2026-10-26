' -------------------------------------------------------------------------------------------------
' Classe      : UC_DossierFiche — Paiements (fichier partiel)
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Onglet Paiements du dossier : relevé chronologique paginé et filtré, résumé et totaux,
' encaissement, annulation, avoirs, détail des paiements d'une séance.
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

#Region "Paiements"

    ' Paiements du dossier (liste complète, non filtrée) : source de la grille et de son résumé.
    Private _paiements As List(Of PaiementListeItem)

    ' Axes du filtre cboFiltrePaiement : AxeFiltrePaiement (FiltresPaiements, partagé avec la
    ' recherche des paiements d'une période).

    ' Garde-fou : la grille des paiements n'est initialisée (colonnes, styles) qu'une seule fois.
    Private _grillePaiementsInitialisee As Boolean = False

    ' Résumé cliquable au-dessus de la grille des paiements (créé avec la grille).
    Private _resumePaiements As ResumeFiltre

    ' Montant d'un paiement annulé : police des cellules (InitDataGridBasique) barrée, partagée pour
    ' ne pas créer une police à chaque mise en forme de cellule.
    Private Shared ReadOnly PoliceMontantAnnule As New Font("Calibri", 10.0F, FontStyle.Strikeout)

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserGrillePaiements
    ' Version   : V2.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Fige les propriétés de base de la grille des paiements (une seule fois).
    '
    ' Remarques :
    ' - V2.0.0 : relevé chronologique (maquette « Paiements — A » du 17/10/2026) : lignes à pastilles,
    '   tri par en-tête rétabli (plus de regroupement par séance), résumé cliquable.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserGrillePaiements()

        If _grillePaiementsInitialisee Then
            Return
        End If

        UtilsDataGrid.InitDataGridBasique(dgvPaiements)

        dgvPaiements.AutoGenerateColumns = False
        dgvPaiements.ReadOnly = True
        dgvPaiements.MultiSelect = False
        dgvPaiements.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPaiements.AllowUserToAddRows = False
        dgvPaiements.AllowUserToDeleteRows = False

        colPaiementMontant.DefaultCellStyle.Format = "N2"
        colPaiementMontant.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        ' Hauteur des lignes : place pour la pastille « État ».
        dgvPaiements.RowTemplate.Height = HAUTEUR_LIGNE_PASTILLES

        ' Pagination (V1.9.0) : branchée avant les filtres, dont l'initialisation affiche déjà la liste.
        pagPaiements.Attacher(dgvPaiements, "paiement(s)")

        ' Avant les filtres : leur initialisation déclenche déjà un premier affichage (et le résumé).
        _resumePaiements = New ResumeFiltre(pnlResumePaiements, cboFiltrePaiement, AxeFiltrePaiement.Tous,
                                            "tous les paiements", "ces paiements", AddressOf DefinirInfobulle)

        InitialiserFiltresPaiements()

        _grillePaiementsInitialisee = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerPaiements
    ' Version   : V1.1.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Recharge la grille des paiements du dossier courant depuis la couche métier et met à jour
    ' l'état des boutons d'action.
    '
    ' Remarques :
    ' - V1.1.0 : chaque paiement apprend l'état de sa séance (RenseignerEtatSeances).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerPaiements()

        If _idDossier <= 0 Then
            _paiements = Nothing
            pagPaiements.DefinirSource(Nothing)
            MettreAJourResumePaiements()
            RafraichirEtatBoutonsPaiements()
            Return
        End If

        Try

            dgvPaiements.AutoGenerateColumns = False

            _paiements = GestionPaiements.GetPaiementsParDossier(_idDossier)
            RenseignerEtatSeances()
            AppliquerFiltresPaiements()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement des paiements (UC_DossierFiche, id={_idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des paiements.")
            End If

        End Try

        RafraichirEtatBoutonsPaiements()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RenseignerEtatSeances
    ' Version   : V1.1.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Recopie sur chaque paiement l'état de sa séance chargée dans l'onglet Séances : séance passée
    ' (même règle, heures comprises, pour que les deux grilles disent la même chose le jour de la
    ' séance) et avoir de la séance (trop-perçu d'une séance encore due).
    '
    ' Remarques :
    ' - Les séances sont chargées avant les paiements (ChargerSeances appelle ChargerPaiements). Si la
    '   séance n'est pas trouvée, la date seule décide (passée si antérieure à aujourd'hui) et aucun
    '   avoir n'est recopié.
    ' - V1.1.0 : ancien nom RenseignerSeancesPassees ; recopie aussi AvoirSeance.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RenseignerEtatSeances()

        If _paiements Is Nothing Then Return

        Dim seancesParId As Dictionary(Of Long, SeanceListeItem) =
            If(_seances, New List(Of SeanceListeItem)).ToDictionary(Function(s) s.IdSeance)

        For Each paiement In _paiements
            Dim seance As SeanceListeItem = Nothing
            If seancesParId.TryGetValue(paiement.IdSeance, seance) Then
                paiement.SeancePassee = seance.EstPassee
                paiement.AvoirSeance = seance.AvoirSeance
            Else
                paiement.SeancePassee = paiement.DateSeance.Date < Date.Today
                paiement.AvoirSeance = 0D
            End If
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserFiltresPaiements
    ' Version   : V1.3.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Initialise le filtre de la grille des paiements selon leur statut réel.
    '
    ' Remarques :
    ' - V1.1.0 : « À régler » (pastille « Reste à régler » du résumé).
    ' - V1.2.0 (21/10/2026) : axes aux couleurs des pastilles (ElementListePastille).
    ' - V1.3.0 (23/10/2026) : axes et couleurs dans FiltresPaiements.RemplirListe (partagés).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFiltresPaiements()

        FiltresPaiements.RemplirListe(cboFiltrePaiement)

        ' Période inactive par défaut ; bornes préremplies sur l'année civile en cours
        dtpPaiementDu.Valeur = New Date(Date.Today.Year, 1, 1)
        dtpPaiementAu.Valeur = New Date(Date.Today.Year, 12, 31)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltresPaiements
    ' Version   : V2.4.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Applique la recherche, la période et le filtre à la grille des paiements, présentée en relevé
    ' chronologique : un paiement par ligne, le plus récent en haut.
    '
    ' Remarques :
    ' - Les paiements annulés restent visibles par défaut : les masquer ferait disparaître la trace
    '   d'une opération passée, ce qui nuirait à l'auditabilité du dossier.
    ' - V2.0.0 : fin du regroupement par séance (en-têtes repliables) ; la colonne « Pour » et la
    '   pastille « État » disent à quelle séance s'applique le paiement et ce qu'il est devenu.
    '   Filtre « À régler » ; mise à jour du résumé.
    ' - V2.2.0 : dus en attente clos automatiquement masqués (EstAttenteClose).
    ' - V2.1.0 : le filtre Avoirs retient aussi les encaissements d'une séance due avec trop-perçu
    '   (la condition « reste dû négatif » ne pouvait jamais être vraie : la requête le borne à 0).
    ' - V2.3.0 (23/10/2026, recherche R1) : page du paiement ciblé (CiblerAuChargement) s'il est listé.
    ' - V2.4.0 (23/10/2026) : lignes du relevé et filtrage par axe dans FiltresPaiements (partagés).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltresPaiements()

        MettreAJourResumePaiements()

        If _paiements Is Nothing Then
            pagPaiements.DefinirSource(Nothing)
            Return
        End If

        ' Les dus en attente clos par l'application ne sont pas des mouvements (FiltresPaiements).
        Dim paiementsFiltres = FiltresPaiements.LignesDuReleve(_paiements)

        Dim texte As String = txtRecherchePaiement.Text.Trim()
        If Not String.IsNullOrEmpty(texte) Then
            paiementsFiltres = paiementsFiltres.Where(Function(p) CorrespondRecherchePaiement(p, texte))
        End If

        ' Filtre par période : date de paiement, ou date de séance pour un paiement encore en attente (sans date)
        If chkPeriodePaiement.Checked Then
            Dim du As Date = dtpPaiementDu.Valeur.Value
            Dim au As Date = dtpPaiementAu.Valeur.Value
            paiementsFiltres = paiementsFiltres.Where(
                Function(p)
                    Dim d As Date = p.DateMouvement
                    Return d >= du AndAlso d <= au
                End Function)
        End If

        paiementsFiltres = FiltresPaiements.Filtrer(paiementsFiltres, FiltresPaiements.Axe(cboFiltrePaiement))

        ' Ordre de l'écran (le plus récent en haut) : un éventuel tri par en-tête est oublié.
        UtilsDataGrid.ReinitialiserTri(dgvPaiements)

        Dim liste As List(Of PaiementListeItem) = paiementsFiltres.
            OrderByDescending(Function(p) p.DateMouvement).
            ThenByDescending(Function(p) p.IdPaiement).
            ToList()

        ' Ouverture ciblée (recherche) : la page du paiement demandé, s'il est dans la liste.
        Dim indexCible As Integer = If(_idPaiementCible > 0, liste.FindIndex(Function(p) p.IdPaiement = _idPaiementCible), -1)
        pagPaiements.DefinirSource(liste, If(indexCible >= 0, pagPaiements.PageDeIndex(indexCible), 1))

        RafraichirEtatBoutonsPaiements()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondRecherchePaiement
    ' Version  : V1.1.0
    ' Date     : 17/10/2026
    '
    ' Rôle     :
    ' Indique si un paiement correspond au texte recherché (code, séance, nature, mouvement, mode,
    ' référence, état ou date), en comparaison insensible à la casse.
    '
    ' Remarques :
    ' - La nature et la date de la séance sont incluses : on recherche souvent un encaissement à
    '   partir de la séance qu'il couvre, pas à partir de son code technique.
    ' - V1.1.0 : textes des nouvelles colonnes (Mouvement, Pour, État) ; le code et la référence,
    '   retirés de la grille, restent cherchables.
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondRecherchePaiement(paiement As PaiementListeItem, texte As String) As Boolean

        Dim champs As String =
            String.Join(" ", {
                If(paiement.CodePaiement, ""),
                paiement.PourAffichage,
                paiement.MouvementCourt,
                If(paiement.ModePaiement, ""),
                If(paiement.ReferencePaiement, ""),
                paiement.EtatAffichage,
                paiement.LibelleStatutPaiement,
                If(paiement.DatePaiement.HasValue, paiement.DatePaiement.Value.ToString("dd/MM/yyyy"), "")
            })

        Return champs.IndexOf(texte, StringComparison.OrdinalIgnoreCase) >= 0

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPaiements_CellFormatting
    ' Version   : V2.1.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Mise en forme des cellules ordinaires : date avec le jour (« ven. 17/10/2026 »), « — » pour un
    ' paiement encore attendu (pas de date d'encaissement), ligne neutralisée (annulée, avoir
    ' entièrement utilisé) en gris, montant barré s'il est annulé (l'opération est gardée pour la
    ' trace mais ne compte plus). La pastille « État » est dessinée par dgvPaiements_CellPainting.
    '
    ' Remarques :
    ' - V2.0.0 : remplace les couleurs de fond des colonnes Statut / Reste dû (retirées).
    ' - V2.1.0 : avoir entièrement utilisé grisé comme une annulation (sans barrer).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPaiements_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) _
        Handles dgvPaiements.CellFormatting

        If e.RowIndex < 0 Then Return

        Dim paiement As PaiementListeItem = TryCast(dgvPaiements.Rows(e.RowIndex).DataBoundItem, PaiementListeItem)
        If paiement Is Nothing Then Return

        If paiement.EstNeutralise Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

        If e.ColumnIndex = colPaiementDate.Index Then
            If paiement.DatePaiement.HasValue Then
                Dim datePaiement As Date = paiement.DatePaiement.Value
                e.Value = $"{datePaiement:ddd} {datePaiement.ToShortDateString()}"
            Else
                e.Value = "—"
                e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            End If
            e.FormattingApplied = True

        ElseIf e.ColumnIndex = colPaiementMontant.Index AndAlso paiement.EstAnnule Then
            e.CellStyle.Font = PoliceMontantAnnule
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPaiements_CellPainting
    ' Version   : V1.1.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Dessine la pastille « État » (ce que le paiement est devenu), colorée par famille comme la
    ' pastille « Paiement » des séances ; atténuée, avec sa précision grise, sur une ligne neutralisée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPaiements_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) _
        Handles dgvPaiements.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colPaiementEtat.Index Then Return

        Dim paiement As PaiementListeItem = TryCast(dgvPaiements.Rows(e.RowIndex).DataBoundItem, PaiementListeItem)
        If paiement Is Nothing Then Return

        UtilsDataGrid.DessinerPastilleCellule(dgvPaiements, e, paiement.EtatAffichage,
                                              UtilsPastilles.Teinte(paiement.FamilleEtat),
                                              paiement.MentionEtat, paiement.EstNeutralise)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPaiements_CellToolTipTextNeeded
    ' Rôle      : Infobulle de ligne : code du paiement, phrase complète du mouvement, montant du
    '             paiement s'il diffère de la ligne (paiement réparti, avoir utilisé), référence,
    '             mentions d'annulation et d'utilisation d'avoir ; origine d'un règlement par avoir
    '             (22/10/2026). Texte composé par PaiementListeItem.InfobulleLigne (23/10/2026, partagé
    '             avec la recherche des paiements).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPaiements_CellToolTipTextNeeded(sender As Object, e As DataGridViewCellToolTipTextNeededEventArgs) _
        Handles dgvPaiements.CellToolTipTextNeeded

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        Dim paiement As PaiementListeItem = TryCast(dgvPaiements.Rows(e.RowIndex).DataBoundItem, PaiementListeItem)
        If paiement Is Nothing Then Return

        e.ToolTipText = paiement.InfobulleLigne

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événements : filtres de la grille des paiements
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Réappliquent le filtrage dès que l'utilisateur modifie la recherche ou l'axe sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRecherchePaiement_TextChanged(sender As Object, e As EventArgs) Handles txtRecherchePaiement.TextChanged

        AppliquerFiltresPaiements()

    End Sub

    ' Réapplique le filtrage des paiements au changement d'axe de filtre.
    Private Sub cboFiltrePaiement_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFiltrePaiement.SelectedIndexChanged

        AppliquerFiltresPaiements()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : PaiementSelectionne
    ' Version  : V1.2.0
    ' Date     : 22/10/2026
    '
    ' Rôle     :
    ' Retourne le paiement sélectionné dans la grille par l'utilisatrice, ou Nothing.
    ' - V1.2.0 (22/10/2026) : la ligne courante posée d'office au chargement ne compte plus
    '   (UtilsDataGrid.ElementSelectionne).
    ' -------------------------------------------------------------------------------------------------
    Private Function PaiementSelectionne() As PaiementListeItem
        Return UtilsDataGrid.ElementSelectionne(Of PaiementListeItem)(dgvPaiements)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirEtatBoutonsPaiements
    ' Version   : V1.1.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Active/désactive le bouton « Annuler l'encaissement » selon le contexte (dossier non archivé) et
    ' la sélection courante.
    ' - V1.1.0 (20/10/2026) : raison de chaque bouton inactif, montrée au survol.
    ' - V1.2.0 (22/10/2026) : bouton « Nouveau paiement » retiré (décision de Joëlle).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirEtatBoutonsPaiements()

        Dim raisonDossier As String = RaisonDossierFige()

        Dim paiementSel As PaiementListeItem = PaiementSelectionne()
        Dim raison As String = raisonDossier

        If raison.Length = 0 Then
            If paiementSel Is Nothing Then
                raison = "Sélectionnez un encaissement dans la liste."
            ElseIf Not paiementSel.EstEncaisse Then
                raison = "Seul un paiement encaissé peut être annulé (pas un montant dû ni un paiement déjà annulé)."
            ElseIf _context Is Nothing OrElse Not _context.HasRole(AppRole.SuperUser) Then
                raison = "Annuler un encaissement est réservé au SuperUser."
            End If
        End If

        btnSupprimerPaiement.DefinirDisponibilite(raison.Length = 0, raison)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPaiements_SelectionChanged
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' Met à jour l'état des boutons d'action selon la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPaiements_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPaiements.SelectionChanged

        RafraichirEtatBoutonsPaiements()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPaiements_CellDoubleClick
    ' Version    : V1.0.1
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Ouvre en édition le paiement de la ligne double-cliquée (via la séance associée).
    ' - V1.0.1 (21/10/2026, audit B.4) : la séance reconstituée reprend le reste dû du paiement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPaiements_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPaiements.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        Dim paiement As PaiementListeItem = PaiementSelectionne()

        If paiement Is Nothing Then
            Return
        End If

        Dim seance As SeanceListeItem = Nothing

        If _seances IsNot Nothing Then
            seance = _seances.FirstOrDefault(Function(s) s.IdSeance = paiement.IdSeance)
        End If

        If seance Is Nothing Then
            ' La séance peut être absente de la liste chargée (filtre en cours) : on reconstitue le
            ' minimum affichable à partir du paiement pour que la modale rappelle bien la séance.
            seance = New SeanceListeItem With {
                .IdSeance = paiement.IdSeance,
                .CodeSeance = paiement.CodeSeance,
                .DateSeance = paiement.DateSeance,
                .LibelleNatureSeance = paiement.NatureSeance,
                .TarifSeance = paiement.TarifSeance,
                .ResteDu = paiement.ResteDuSeance
            }
        End If

        OuvrirPaiementPourSeance(seance)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnEncaisserSeance_Click
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' Ouvre la modale de saisie d'un paiement pour la séance sélectionnée (pré-remplie du reste dû),
    ' puis recharge séances et paiements après un enregistrement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEncaisserSeance_Click(sender As Object, e As EventArgs) Handles btnEncaisserSeance.Click

        If _idDossier <= 0 OrElse EstDossierArchive() Then
            Return
        End If

        Dim seance As SeanceListeItem = SeanceSelectionnee()

        If seance Is Nothing Then
            Return
        End If

        OuvrirPaiementPourSeance(seance)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleRappelSeance
    ' Version  : V1.1.0
    ' Date     : 17/10/2026
    '
    ' Rôle     :
    ' Compose le rappel « SE000074 — jj/mm/aaaa (Nature) » affiché dans la modale de paiement
    ' (V1.1.0 : code de la séance en tête, les dates seules ne suffisant pas à la distinguer).
    '
    ' Retour   :
    ' - String : Libellé de la séance, sans parenthèses vides si la nature est inconnue.
    ' -------------------------------------------------------------------------------------------------
    Private Function LibelleRappelSeance(seance As SeanceListeItem) As String

        If seance Is Nothing OrElse seance.DateSeance = Date.MinValue Then
            Return String.Empty
        End If

        Dim code As String = If(String.IsNullOrWhiteSpace(seance.CodeSeance), String.Empty, $"{seance.CodeSeance} — ")

        If String.IsNullOrWhiteSpace(seance.LibelleNatureSeance) Then
            Return $"{code}{seance.DateSeance:dd/MM/yyyy}"
        End If

        Return $"{code}{seance.DateSeance:dd/MM/yyyy} ({seance.LibelleNatureSeance})"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirPaiementPourSeance
    ' Version   : V1.1.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre la modale PaiementEditor pour une séance donnée (montant pré-rempli sur le reste dû),
    ' puis recharge séances et paiements si un enregistrement a eu lieu.
    '
    ' Remarques :
    ' - V1.1.0 (22/10/2026, audit C.2) : refus lu dans SeanceListeItem.RaisonNonEncaissable (même règle
    '   que le bouton et que GestionPaiements.CreatePaiement).
    ' - V1.1.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirPaiementPourSeance(seance As SeanceListeItem)

        If Not seance.EstEncaissable Then
            DialogChoix.Information(seance.RaisonNonEncaissable, "Encaissement impossible")
            Return
        End If

        Dim resteDu As Decimal = seance.ResteDu

        If ProposerUtilisationAvoir(seance, resteDu) Then
            Return
        End If

        Dim libelleSeance As String = LibelleRappelSeance(seance)

        Using editeur As New PaiementEditor(seance.IdSeance, seance.CodeSeance, resteDu, EstDossierArchive(), _context?.FenetrePrincipale, libelleSeance)

            editeur.SetContext(_context)
            SignalerAction($"Séance {seance.CodeSeance} > Encaissement",
                           $"Encaissement de la séance {seance.CodeSeance} (reste dû : {resteDu:N2} €).")

            If editeur.ShowDialog(FindForm()) = DialogResult.OK Then
                ChargerSeances()
            Else
                _context?.SetStatus("Encaissement abandonné.")
            End If

        End Using

        RafraichirContexteNavigation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ProposerUtilisationAvoir
    ' Version  : V1.1.0
    ' Date     : 22/10/2026
    '
    ' Rôle     :
    ' Si le patient dispose d'un avoir (tous dossiers), propose de l'imputer sur le reste dû de la
    ' séance. Seul le montant nécessaire est consommé ; le surplus reste en avoir.
    '
    ' Retour   :
    ' - Boolean : True si la séance est soldée par l'avoir (inutile d'ouvrir la saisie de paiement).
    ' - V1.0.1 (22/10/2026, audit B.5) : utilisateur transmis par son identifiant.
    ' - V1.1.0 (22/10/2026, audit C.1) : un refus du métier est montré tel quel ; un échec technique
    '   est journalisé et annoncé sans détail technique (le message de l'exception était affiché).
    ' -------------------------------------------------------------------------------------------------
    Private Function ProposerUtilisationAvoir(seance As SeanceListeItem, ByRef resteDu As Decimal) As Boolean

        Dim avoirs As List(Of AvoirDisponible)

        Try
            avoirs = GestionPaiements.GetAvoirsDisponiblesPourSeance(seance.IdSeance)
        Catch ex As Exception
            GestionLog.EcrireLog("Erreur lecture des avoirs patient.", GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            Return False
        End Try

        Dim totalAvoir As Decimal = avoirs.Sum(Function(a) a.Montant)

        If totalAvoir <= 0D Then
            Return False
        End If

        Dim aImputer As Decimal = Math.Min(totalAvoir, seance.ResteDu)
        Dim message As String =
            $"Le patient dispose d'un avoir de {totalAvoir:N2} €." & Environment.NewLine &
            $"Utiliser {aImputer:N2} € pour la séance {seance.CodeSeance} (reste dû {seance.ResteDu:N2} €) ?"

        If totalAvoir > aImputer Then
            message &= Environment.NewLine & $"Le solde de {totalAvoir - aImputer:N2} € restera disponible en avoir."
        End If

        If DialogChoix.Confirmer(message, "Utiliser l'avoir") <> DialogResult.Yes Then
            Return False
        End If

        Try

            Dim impute As Decimal = GestionPaiements.UtiliserAvoirPourSeance(seance.IdSeance, seance.ResteDu,
                                                                             _context?.ContexteOperation)

            ChargerSeances()

            If _context IsNot Nothing Then
                _context.SetStatus($"Avoir de {impute:N2} € imputé sur la séance {seance.CodeSeance}.")
            End If

            resteDu = Math.Max(0D, seance.ResteDu - impute)

            Return resteDu <= 0D

        Catch exRefus As InvalidOperationException
            DialogChoix.Information(exRefus.Message, "Utilisation de l'avoir")
            Return True

        Catch exDroits As UnauthorizedAccessException
            DialogChoix.Avertissement(exDroits.Message, "Utilisation de l'avoir")
            Return True

        Catch ex As Exception
            GestionLog.EcrireLog(
                $"Erreur utilisation d'avoir (UC_DossierFiche, idDossier={_idDossier}, idSeance={seance.IdSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )
            DialogChoix.Erreur(
                "L'avoir n'a pas pu être utilisé ; rien n'a été modifié." & Environment.NewLine &
                "Vérifiez la connexion à la base de données, puis recommencez.",
                "Utilisation de l'avoir")
            Return True
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnSupprimerPaiement_Click
    ' Version    : V1.3.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Annule l'encaissement sélectionné (SuperUser/Admin, motif obligatoire) : le paiement n'est
    ' jamais supprimé. Recharge ensuite séances et paiements.
    '
    ' Remarques  :
    ' - V1.0.1 : le bouton s'intitule « Annuler l'encaissement » (il affichait « Supprimer paiement »,
    '   contraire à la règle « aucun paiement supprimé ») ; nom du contrôle inchangé.
    ' - V1.1.0 : un règlement par avoir est refusé avec un message, avant la saisie du motif ; tout
    '   refus ou échec s'affiche dans une fenêtre (il ne restait que dans la barre d'état).
    ' - V1.2.0 (21/10/2026) : de même pour un encaissement dont une partie a déjà servi d'avoir.
    ' - V1.2.1 (21/10/2026, audit B.5) : règlement par avoir lu sur le paiement (référentiel).
    ' - V1.3.0 (22/10/2026, audit B.5) : l'utilisateur est transmis par son identifiant ; la date, le
    '   motif et l'auteur s'enregistrent dans les colonnes de l'annulation.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSupprimerPaiement_Click(sender As Object, e As EventArgs) Handles btnSupprimerPaiement.Click

        If _idDossier <= 0 OrElse EstDossierArchive() Then
            Return
        End If

        Dim paiement As PaiementListeItem = PaiementSelectionne()

        If paiement Is Nothing Then
            Return
        End If

        If Not paiement.EstEncaisse Then
            DialogChoix.Information(
                "Seul un paiement encaissé peut être annulé. Un dû en attente suit automatiquement le statut de sa séance.",
                "Annulation impossible")
            Return
        End If

        If _context Is Nothing OrElse Not _context.HasRole(AppRole.SuperUser) Then
            DialogChoix.Avertissement(
                "L'annulation d'un encaissement est réservée aux utilisateurs SuperUser ou Admin.",
                "Droits insuffisants")
            Return
        End If

        ' Refus annoncé avant de demander un motif (la couche métier le refuserait de toute façon).
        If paiement.EstReglementParAvoir Then
            DialogChoix.Information(
                $"Le paiement {paiement.CodePaiement} règle la séance {paiement.CodeSeance} avec un avoir : il ne peut pas être annulé, l'avoir d'origine serait perdu.",
                "Annulation impossible")
            Return
        End If

        Dim montantUtilise As Decimal = GestionPaiements.GetMontantUtiliseEnAvoir(paiement.IdPaiement)
        If montantUtilise > 0D Then
            DialogChoix.Information(
                $"Paiement {paiement.CodePaiement} : " & GestionPaiements.MessageAnnulationAvoirUtilise(montantUtilise),
                "Annulation impossible")
            Return
        End If

        Dim libelleDate As String =
            If(paiement.DatePaiement.HasValue, paiement.DatePaiement.Value.ToShortDateString(), "en attente")

        Dim motif As String

        Using dlg As New DialogMotifAnnulation(
            $"Annulation de l'encaissement {paiement.CodePaiement} du {libelleDate} ({paiement.Montant:N2} €), séance {paiement.CodeSeance}.",
            paiement.CodePaiement)

            SignalerAction($"Paiement {paiement.CodePaiement} > Annulation", $"Annulation de l'encaissement {paiement.CodePaiement} : motif requis.")

            If dlg.ShowDialog(FindForm()) <> DialogResult.OK Then
                RafraichirContexteNavigation()
                If _context IsNot Nothing Then
                    _context.SetStatus("Annulation du paiement abandonnée.")
                End If
                Return
            End If

            motif = dlg.Motif

        End Using

        RafraichirContexteNavigation()

        Try

            GestionPaiements.AnnulerPaiementEncaisse(_context?.ContexteOperation, paiement.IdPaiement, motif)

            If _context IsNot Nothing Then
                _context.SetStatus($"Paiement {paiement.CodePaiement} annulé (tracé).")
            End If

            ChargerSeances()

        Catch ex As InvalidOperationException

            ' Refus métier (message rédigé pour l'utilisatrice) : il doit se voir, pas seulement dans
            ' la barre d'état.
            DialogChoix.Information(ex.Message, "Annulation impossible")
            _context?.SetStatus("Annulation du paiement refusée.")

        Catch ex As UnauthorizedAccessException

            DialogChoix.Avertissement(ex.Message, "Accès refusé")
            _context?.SetStatus("Annulation du paiement refusée.")

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur annulation paiement (UC_DossierFiche, idDossier={_idDossier}, idPaiement={paiement.IdPaiement}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur("L'annulation n'a pas pu être effectuée. Le détail a été noté dans le journal.", "Annulation impossible")
            _context?.SetStatus("Erreur lors de l'annulation du paiement.")

        End Try

    End Sub

#End Region

End Class
