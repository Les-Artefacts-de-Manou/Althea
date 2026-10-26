' -------------------------------------------------------------------------------------------------
' UserControl : UC_RecherchePaiements
' Projet      : Althéa
' Version     : V1.0.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Recherche transversale R3 (ADR-52) : les paiements d'un jour ou d'une période, tous patients —
' le relevé de l'onglet Paiements du dossier (décision de Joëlle, 23/10/2026) avec le patient de
' chaque ligne, les totaux encaissés par mode et l'accès direct au dossier.
'
' Responsabilités :
' - Choisir la période (SelecteurPeriode : dates, raccourcis, précédente / suivante)
' - Affiner : axe du filtre et résumé cliquable (FiltresPaiements, comme le dossier), mode de
'   paiement, texte (patient, code, séance, mouvement, mode, référence, état)
' - Totaux encaissés par mode sur la période et le mode choisi (demande de Joëlle)
' - Aller au paiement (hub du dossier, onglet Paiements, paiement sélectionné) ou à la fiche du patient
'
' Remarques   :
' - Aucun accès direct à la base : GestionRecherche ; aller à l'élément : OuvertureResultat.
' - Une ligne par affectation à une séance (comme le dossier) : un paiement réparti apparaît sur
'   plusieurs lignes.
' - Ouvert depuis le hub Recherche, empilé sur lui : Retour y ramène ; l'écran ouvert depuis cette
'   liste est empilé sur elle : Retour la recrée avec ses critères et resélectionne la ligne.
' - Liste plafonnée (GestionRecherche.LIMITE_RESULTATS) ; rien n'est journalisé de la période.
' - V1.0.1 (25/10/2026, audit C.4 étape 3, ADR-55) : fenêtre principale par IFenetrePrincipale
'   (lue dans le contexte) ; plus aucune référence à Home.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_RecherchePaiements
    Implements IContextAwareUserControl

#Region "Constantes"

    ' Contexte affiché (et réaffiché au retour) : « Recherche > Paiements ».
    Public Const CONTEXTE As String = OuvertureResultat.CONTEXTE_RECHERCHE & " > Paiements"

    ' Hauteur des lignes : place pour la pastille « État » (comme l'onglet Paiements du dossier).
    Private Const HAUTEUR_LIGNE_PASTILLES As Integer = 30

    ' Montant d'un paiement annulé : barré (comme le dossier).
    Private Shared ReadOnly PoliceMontantAnnule As New Font("Calibri", 10.0F, FontStyle.Strikeout)

#End Region

#Region "Variables privées"

    ' Contexte UI partagé fourni par Home (barre d'état, infobulles).
    Private _context As UserControlContext

    ' Critères à rétablir au chargement (retour depuis l'écran ouvert) ; Nothing : aujourd'hui.
    Private _duInitial As Date?
    Private _auInitial As Date?
    Private _axeInitial As AxeFiltrePaiement = AxeFiltrePaiement.Tous
    Private _idModeInitial As ULong?
    Private _texteInitial As String = String.Empty
    Private _idPaiementInitial As Long

    ' Lignes du relevé de la période (avant filtre), résumé cliquable, période.
    Private _paiements As New List(Of PaiementListeItem)
    Private _resume As ResumeFiltre
    Private WithEvents _periode As SelecteurPeriode

    ' Modes de la liste « Mode », dans l'ordre des éléments (index 0 : tous les modes).
    Private ReadOnly _idsModes As New List(Of ULong?)

    ' Vrai pendant que l'écran pose lui-même les critères : pas de recherche à chaque champ.
    Private _chargement As Boolean

#End Region

#Region "Contexte et initialisation"

    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext
        _context = context
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Prerenseigner
    ' Rôle      : Critères à rétablir au chargement (fabrique de retour) ; à appeler avant la navigation.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Prerenseigner(du As Date, au As Date, axe As AxeFiltrePaiement, idMode As ULong?, texte As String, idPaiement As Long)
        _duInitial = du
        _auInitial = au
        _axeInitial = axe
        _idModeInitial = idMode
        _texteInitial = If(texte, String.Empty)
        _idPaiementInitial = idPaiement
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_RecherchePaiements_Load
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Grille, filtres et résumé ; période d'aujourd'hui, ou celle du retour ; première recherche.
    '
    ' Remarques :
    ' - La ligne du retour est resélectionnée après l'affichage (BeginInvoke) : une grille pas encore
    '   visible perd sa sélection quand elle le devient.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_RecherchePaiements_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If DesignMode Then Return

        UtilsDataGrid.InitDataGridBasique(dgvPaiements)
        dgvPaiements.RowTemplate.Height = HAUTEUR_LIGNE_PASTILLES
        colMontant.DefaultCellStyle.Format = "N2"
        colMontant.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        UtilsDataGrid.ActiverTriColonnes(dgvPaiements)

        _chargement = True
        Try
            _resume = New ResumeFiltre(pnlResume, cboFiltre, AxeFiltrePaiement.Tous,
                                       "tous les paiements de la période", "ces paiements", AddressOf DefinirInfobulle)
            FiltresPaiements.RemplirListe(cboFiltre)
            cboFiltre.SelectedIndex = _axeInitial

            RemplirModes()

            _periode = New SelecteurPeriode(dtpDu, dtpAu, btnPrecedent, btnSuivant, btnAujourdhui, btnSemaine, btnMois)
            _periode.Definir(If(_duInitial, Date.Today), If(_auInitial, Date.Today), notifier:=False)

            txtFiltre.Text = _texteInitial
        Finally
            _chargement = False
        End Try

        If _context IsNot Nothing Then
            _periode.DefinirInfobulles(_context, "paiements")
            _context.SetToolTip(btnOuvrir, "Aller au paiement : le dossier, onglet Paiements (aussi : double-clic ou Entrée sur la ligne).")
            _context.SetToolTip(btnOuvrirPatient, "Ouvrir la fiche du patient du paiement sélectionné.")
            _context.SetToolTip(btnRetour, "Revenir au hub Recherche.")
            _context.SetToolTip(lblTotaux, "Encaissements non annulés de la période (et du mode choisi). Un règlement par avoir n'est pas de l'argent reçu : il est hors total.")
        End If

        Chercher()

        If _idPaiementInitial > 0 Then
            BeginInvoke(New Action(Sub() SelectionnerPaiement(_idPaiementInitial)))
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirModes
    ' Rôle      : Liste « Mode » : tous les modes, puis chaque mode du référentiel (désactivés compris :
    '             d'anciens paiements les portent) ; le mode du retour est resélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirModes()

        cboMode.Items.Clear()
        _idsModes.Clear()

        cboMode.Items.Add(New ElementListePastille("Tous les modes"))
        _idsModes.Add(Nothing)

        Try
            For Each mode As ModePaiement In GestionModesPaiement.GetModesPaiement(afficherInactifs:=True)
                cboMode.Items.Add(New ElementListePastille(mode.LibelleModePaiement))
                _idsModes.Add(mode.IdModePaiement)
            Next
        Catch ex As Exception
            ' Détail au journal (GestionModesPaiement) ; la liste reste utilisable sans filtre de mode.
            _context?.SetStatus("La liste des modes de paiement n'a pas pu être chargée : filtre par mode indisponible.")
        End Try

        Dim index As Integer = If(_idModeInitial.HasValue, _idsModes.IndexOf(_idModeInitial), 0)
        cboMode.SelectedIndex = Math.Max(index, 0)

    End Sub

    ' Mode choisi (Nothing : tous les modes).
    Private Function ModeChoisi() As ULong?
        If cboMode.SelectedIndex < 0 OrElse cboMode.SelectedIndex >= _idsModes.Count Then Return Nothing
        Return _idsModes(cboMode.SelectedIndex)
    End Function

#End Region

#Region "Recherche et affichage"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Chercher
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Charge les lignes de paiement de la période, puis les affiche selon les filtres ; dit quand la
    ' liste est plafonnée ou vide.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Chercher()

        If _chargement Then Return

        lblMessage.Text = String.Empty
        Dim tronquee As Boolean

        Try
            _paiements = GestionRecherche.RechercherPaiements(_periode.Du, _periode.Au, tronquee)
        Catch ex As Exception
            ' Détail au journal (GestionPaiements) ; ici, un message sans détail technique.
            _paiements = New List(Of PaiementListeItem)
            AfficherPaiements()
            lblMessage.Text = "La recherche n'a pas pu aboutir (le détail est dans le journal)."
            Return
        End Try

        AfficherPaiements()

        If tronquee Then
            lblMessage.Text = $"Plus de {GestionRecherche.LIMITE_RESULTATS} lignes : seules les plus récentes sont listées, réduisez la période."
        ElseIf Not FiltresPaiements.LignesDuReleve(_paiements).Any() Then
            lblMessage.Text = If(_periode.EstUnJour, "Aucun paiement ce jour-là.", "Aucun paiement sur cette période.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherPaiements
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Affiche le relevé selon le mode, l'axe du filtre et le texte, le plus récent en haut ; le
    ' résumé et les totaux par mode portent sur la période (et le mode choisi).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherPaiements()

        Dim releve As IEnumerable(Of PaiementListeItem) = FiltresPaiements.LignesDuReleve(_paiements)

        Dim idMode As ULong? = ModeChoisi()
        If idMode.HasValue Then
            releve = releve.Where(Function(p) p.IdModePaiement.HasValue AndAlso p.IdModePaiement.Value = idMode.Value)
        End If

        Dim duMode As List(Of PaiementListeItem) = releve.ToList()

        Dim liste As IEnumerable(Of PaiementListeItem) = FiltresPaiements.Filtrer(duMode, FiltresPaiements.Axe(cboFiltre))

        Dim texte As String = txtFiltre.Text.Trim()
        If texte.Length > 0 Then
            liste = liste.Where(Function(p) CorrespondTexte(p, texte))
        End If

        Dim affichees As List(Of PaiementListeItem) = liste.
            OrderByDescending(Function(p) p.DateMouvement).
            ThenByDescending(Function(p) p.IdPaiement).
            ToList()

        ' Nouvelle liste : l'ordre du relevé (un tri par en-tête précédent est oublié).
        UtilsDataGrid.ReinitialiserTri(dgvPaiements)
        dgvPaiements.DataSource = Nothing
        dgvPaiements.DataSource = affichees
        dgvPaiements.ClearSelection()

        FiltresPaiements.MettreAJourResume(_resume, duMode)
        AfficherTotaux(duMode)

        carteResultats.Titre = If(affichees.Count = duMode.Count,
                                  $"Paiements ({duMode.Count})",
                                  $"Paiements ({affichees.Count} sur {duMode.Count})")

        _context?.SetStatus($"Recherche des paiements : {affichees.Count} ligne(s) affichée(s).")

        RafraichirBoutons()

    End Sub

    ' Totaux encaissés par mode (« Espèces 120,00 € · Virement 300,00 € — total 420,00 € »).
    Private Sub AfficherTotaux(lignes As IEnumerable(Of PaiementListeItem))

        Dim regleParAvoir As Decimal
        Dim totaux = GestionRecherche.TotauxEncaissesParMode(lignes, regleParAvoir)

        If totaux.Count = 0 Then
            lblTotaux.Text = "Aucun encaissement sur la période."
        Else
            Dim parMode As String = String.Join(" · ", totaux.Select(Function(t) $"{t.Mode} {t.Montant:N2} €"))
            lblTotaux.Text = $"Encaissé par mode : {parMode} — total {totaux.Sum(Function(t) t.Montant):N2} €"
        End If

        If regleParAvoir > 0D Then
            lblTotaux.Text &= $" · réglé par avoir {regleParAvoir:N2} € (hors total)"
        End If

    End Sub

    ' Le texte cherché figure-t-il dans le patient, les codes, la séance, le mouvement, le mode, la
    ' référence ou l'état de la ligne ?
    Private Shared Function CorrespondTexte(paiement As PaiementListeItem, texte As String) As Boolean

        Dim champs As String = String.Join(" ", {
            paiement.PatientAffichage,
            If(paiement.CodePatient, String.Empty),
            If(paiement.CodePaiement, String.Empty),
            If(paiement.CodeDossier, String.Empty),
            paiement.PourAffichage,
            paiement.MouvementCourt,
            If(paiement.ModePaiement, String.Empty),
            If(paiement.ReferencePaiement, String.Empty),
            paiement.EtatAffichage
        })

        Return champs.IndexOf(texte, StringComparison.OrdinalIgnoreCase) >= 0

    End Function

    ' Resélectionne la ligne du retour si elle est listée (et la rend visible).
    Private Sub SelectionnerPaiement(idPaiement As Long)

        For Each ligne As DataGridViewRow In dgvPaiements.Rows
            Dim paiement As PaiementListeItem = TryCast(ligne.DataBoundItem, PaiementListeItem)
            If paiement Is Nothing OrElse paiement.IdPaiement <> idPaiement Then Continue For

            dgvPaiements.ClearSelection()
            dgvPaiements.CurrentCell = ligne.Cells(colPatient.Index)
            ligne.Selected = True
            If Not ligne.Displayed Then dgvPaiements.FirstDisplayedScrollingRowIndex = ligne.Index
            dgvPaiements.Focus()
            Exit For
        Next

        RafraichirBoutons()

    End Sub

    Private Function PaiementSelectionne() As PaiementListeItem
        Return UtilsDataGrid.ElementSelectionne(Of PaiementListeItem)(dgvPaiements)
    End Function

    Private Sub RafraichirBoutons()
        Dim p As PaiementListeItem = PaiementSelectionne()
        btnOuvrir.Enabled = p IsNot Nothing
        btnOuvrirPatient.Enabled = p IsNot Nothing AndAlso p.IdPatient > 0
    End Sub

    Private Sub DefinirInfobulle(controle As Control, texte As String)
        _context?.SetToolTip(controle, texte)
    End Sub

#End Region

#Region "Aller au paiement"

    Private Sub Ouvrir(patientSeulement As Boolean)

        Dim paiement As PaiementListeItem = PaiementSelectionne()
        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If paiement Is Nothing OrElse fenetre Is Nothing Then Return

        Dim resultat As ResultatRecherche = GestionRecherche.ResultatDePaiement(paiement)

        If patientSeulement Then
            OuvertureResultat.OuvrirPatient(fenetre, resultat, CreerEcranCourant(paiement.IdPaiement), CONTEXTE)
        Else
            OuvertureResultat.Ouvrir(fenetre, resultat, CreerEcranCourant(paiement.IdPaiement), _context, CONTEXTE)
        End If

    End Sub

    ' Fabrique de retour : la liste recréée avec ses critères, la ligne ouverte resélectionnée.
    Private Function CreerEcranCourant(idPaiement As Long) As Func(Of UserControl)

        Dim du As Date = _periode.Du
        Dim au As Date = _periode.Au
        Dim axe As AxeFiltrePaiement = FiltresPaiements.Axe(cboFiltre)
        Dim idMode As ULong? = ModeChoisi()
        Dim texte As String = txtFiltre.Text.Trim()

        Return Function()
                   Dim ecran As New UC_RecherchePaiements()
                   ecran.Prerenseigner(du, au, axe, idMode, texte, idPaiement)
                   Return ecran
               End Function

    End Function

    Private Sub Retourner()

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If fenetre Is Nothing Then Return

        If Not fenetre.NavigateRetour() Then
            fenetre.NavigateToRecherche()
        End If

    End Sub

#End Region

#Region "Gestionnaires d'événements"

    Private Sub _periode_PeriodeChangee(sender As Object, e As EventArgs) Handles _periode.PeriodeChangee
        If _chargement Then Return
        Chercher()
    End Sub

    Private Sub cboFiltre_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFiltre.SelectedIndexChanged
        If _chargement Then Return
        AfficherPaiements()
    End Sub

    Private Sub cboMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboMode.SelectedIndexChanged
        If _chargement Then Return
        AfficherPaiements()
    End Sub

    Private Sub txtFiltre_TextChanged(sender As Object, e As EventArgs) Handles txtFiltre.TextChanged
        If _chargement Then Return
        AfficherPaiements()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPaiements_CellFormatting
    ' Rôle      : Comme le dossier : date avec le jour, « — » pour un paiement encore attendu, ligne
    '             neutralisée (annulée, avoir entièrement utilisé) en gris, montant annulé barré.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPaiements_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvPaiements.CellFormatting

        If e.RowIndex < 0 Then Return

        Dim paiement As PaiementListeItem = TryCast(dgvPaiements.Rows(e.RowIndex).DataBoundItem, PaiementListeItem)
        If paiement Is Nothing Then Return

        If paiement.EstNeutralise Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

        If e.ColumnIndex = colDate.Index Then
            If paiement.DatePaiement.HasValue Then
                Dim datePaiement As Date = paiement.DatePaiement.Value
                e.Value = $"{datePaiement:ddd} {datePaiement.ToShortDateString()}"
            Else
                e.Value = "—"
                e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            End If
            e.FormattingApplied = True

        ElseIf e.ColumnIndex = colMontant.Index AndAlso paiement.EstAnnule Then
            e.CellStyle.Font = PoliceMontantAnnule
        End If

    End Sub

    ' Pastille « État » (ce que le paiement est devenu), comme le dossier.
    Private Sub dgvPaiements_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvPaiements.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colEtat.Index Then Return

        Dim paiement As PaiementListeItem = TryCast(dgvPaiements.Rows(e.RowIndex).DataBoundItem, PaiementListeItem)
        If paiement Is Nothing Then Return

        UtilsDataGrid.DessinerPastilleCellule(dgvPaiements, e, paiement.EtatAffichage,
                                              UtilsPastilles.Teinte(paiement.FamilleEtat),
                                              paiement.MentionEtat, paiement.EstNeutralise)

    End Sub

    ' Infobulle de ligne, la même que dans le dossier.
    Private Sub dgvPaiements_CellToolTipTextNeeded(sender As Object, e As DataGridViewCellToolTipTextNeededEventArgs) Handles dgvPaiements.CellToolTipTextNeeded

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        Dim paiement As PaiementListeItem = TryCast(dgvPaiements.Rows(e.RowIndex).DataBoundItem, PaiementListeItem)
        If paiement Is Nothing Then Return

        e.ToolTipText = paiement.InfobulleLigne

    End Sub

    Private Sub dgvPaiements_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvPaiements.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            e.Handled = True
            Ouvrir(False)
        End If
    End Sub

    Private Sub dgvPaiements_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPaiements.CellDoubleClick
        If e.RowIndex >= 0 Then Ouvrir(False)
    End Sub

    Private Sub dgvPaiements_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPaiements.SelectionChanged
        RafraichirBoutons()
    End Sub

    Private Sub btnOuvrir_Click(sender As Object, e As EventArgs) Handles btnOuvrir.Click
        Ouvrir(False)
    End Sub

    Private Sub btnOuvrirPatient_Click(sender As Object, e As EventArgs) Handles btnOuvrirPatient.Click
        Ouvrir(True)
    End Sub

    Private Sub btnRetour_Click(sender As Object, e As EventArgs) Handles btnRetour.Click
        Retourner()
    End Sub

#End Region

End Class
