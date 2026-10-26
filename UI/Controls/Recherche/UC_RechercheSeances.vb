' -------------------------------------------------------------------------------------------------
' UserControl : UC_RechercheSeances
' Projet      : Althéa
' Version     : V1.1.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Recherche transversale R2 (ADR-52) : les séances d'un jour ou d'une période, tous patients
' confondus, avec leur patient, leur statut et leur paiement, et l'accès direct au dossier.
'
' Responsabilités :
' - Choisir la période : dates « du … au … », raccourcis (aujourd'hui, cette semaine, ce mois) et
'   période précédente / suivante
' - Affiner la liste : axe du filtre et résumé cliquable (mêmes axes que l'onglet Séances du
'   dossier, FiltresSeances), texte (patient, code, nature, domaine…)
' - Aller à la séance (hub du dossier, onglet Séances, séance sélectionnée) ou à la fiche du patient
'
' Remarques   :
' - Aucun accès direct à la base : GestionRecherche ; aller à l'élément : OuvertureResultat.
' - Ouvert depuis le hub Recherche, empilé sur lui : Retour y ramène. L'écran ouvert depuis cette
'   liste est empilé sur elle : Retour la recrée avec ses critères et resélectionne la séance.
' - Liste plafonnée (GestionRecherche.LIMITE_RESULTATS) : au-delà, « réduisez la période ».
' - Critères en mémoire seulement, le temps de la session ; rien n'est journalisé de la période.
' - V1.1.0 (23/10/2026, R3) : période gérée par SelecteurPeriode (partagée avec la recherche des
'   paiements), sans changement de comportement.
' - V1.1.1 (25/10/2026, audit C.4 étape 3, ADR-55) : fenêtre principale par IFenetrePrincipale
'   (lue dans le contexte) ; plus aucune référence à Home.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_RechercheSeances
    Implements IContextAwareUserControl

#Region "Constantes"

    ' Contexte affiché (et réaffiché au retour) : « Recherche > Séances ».
    Public Const CONTEXTE As String = OuvertureResultat.CONTEXTE_RECHERCHE & " > Séances"

    ' Hauteur des lignes : place pour les pastilles (comme l'onglet Séances du dossier).
    Private Const HAUTEUR_LIGNE_PASTILLES As Integer = 30

#End Region

#Region "Variables privées"

    ' Contexte UI partagé fourni par Home (barre d'état, infobulles).
    Private _context As UserControlContext

    ' Critères à rétablir au chargement (retour depuis l'écran ouvert) ; Nothing : aujourd'hui.
    Private _duInitial As Date?
    Private _auInitial As Date?
    Private _axeInitial As AxeFiltreSeance = AxeFiltreSeance.Toutes
    Private _texteInitial As String = String.Empty
    Private _idSeanceInitiale As Long

    ' Séances de la période (toutes, avant filtre) et résumé cliquable.
    Private _seances As New List(Of SeancePlanningItem)
    Private _resume As ResumeFiltre

    ' Vrai pendant que l'écran pose lui-même les critères : pas de recherche à chaque champ.
    Private _chargement As Boolean

    ' Période (dates, raccourcis, précédente / suivante), créée au chargement.
    Private WithEvents _periode As SelecteurPeriode

#End Region

#Region "Contexte et initialisation"

    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext
        _context = context
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Prerenseigner
    ' Rôle      : Critères à rétablir au chargement (fabrique de retour) ; à appeler avant la navigation.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Prerenseigner(du As Date, au As Date, axe As AxeFiltreSeance, texte As String, idSeance As Long)
        _duInitial = du
        _auInitial = au
        _axeInitial = axe
        _texteInitial = If(texte, String.Empty)
        _idSeanceInitiale = idSeance
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_RechercheSeances_Load
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Grille, filtre et résumé ; période d'aujourd'hui, ou celle du retour ; première recherche.
    '
    ' Remarques :
    ' - La séance du retour est resélectionnée après l'affichage (BeginInvoke) : une grille pas encore
    '   visible perd sa sélection quand elle le devient.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_RechercheSeances_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If DesignMode Then Return

        UtilsDataGrid.InitDataGridBasique(dgvSeances)
        dgvSeances.RowTemplate.Height = HAUTEUR_LIGNE_PASTILLES
        ' Date avec le jour (« ven. 23/10/2026 »), comme l'onglet Séances du dossier.
        colDate.DefaultCellStyle.Format = "ddd dd/MM/yyyy"
        UtilsDataGrid.ActiverTriColonnes(dgvSeances)

        _chargement = True
        Try
            _resume = New ResumeFiltre(pnlResume, cboFiltre, AxeFiltreSeance.Toutes,
                                       "toutes les séances de la période", "ces séances", AddressOf DefinirInfobulle)
            FiltresSeances.RemplirListe(cboFiltre)

            _periode = New SelecteurPeriode(dtpDu, dtpAu, btnPrecedent, btnSuivant, btnAujourdhui, btnSemaine, btnMois)
            _periode.Definir(If(_duInitial, Date.Today), If(_auInitial, Date.Today), notifier:=False)
            cboFiltre.SelectedIndex = _axeInitial
            txtFiltre.Text = _texteInitial
        Finally
            _chargement = False
        End Try

        If _context IsNot Nothing Then
            _periode.DefinirInfobulles(_context, "séances")
            _context.SetToolTip(btnOuvrir, "Aller à la séance : le dossier, onglet Séances (aussi : double-clic ou Entrée sur la ligne).")
            _context.SetToolTip(btnOuvrirPatient, "Ouvrir la fiche du patient de la séance sélectionnée.")
            _context.SetToolTip(btnRetour, "Revenir au hub Recherche.")
        End If

        Chercher()

        If _idSeanceInitiale > 0 Then
            BeginInvoke(New Action(Sub() SelectionnerSeance(_idSeanceInitiale)))
        End If

    End Sub

#End Region

#Region "Recherche et affichage"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Chercher
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Charge les séances de la période, puis les affiche selon le filtre ; dit quand la liste est
    ' plafonnée ou vide.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Chercher()

        If _chargement Then Return

        lblMessage.Text = String.Empty
        Dim tronquee As Boolean

        Try
            _seances = GestionRecherche.RechercherSeances(_periode.Du, _periode.Au, tronquee)
        Catch ex As Exception
            ' Détail au journal (GestionSeances) ; ici, un message sans détail technique.
            _seances = New List(Of SeancePlanningItem)
            AfficherSeances()
            lblMessage.Text = "La recherche n'a pas pu aboutir (le détail est dans le journal)."
            Return
        End Try

        AfficherSeances()

        If tronquee Then
            lblMessage.Text = $"Plus de {GestionRecherche.LIMITE_RESULTATS} séances : seules les premières sont listées, réduisez la période."
        ElseIf _seances.Count = 0 Then
            lblMessage.Text = If(_periode.EstUnJour, "Aucune séance ce jour-là.", "Aucune séance sur cette période.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherSeances
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Affiche les séances chargées selon l'axe du filtre et le texte, dans l'ordre du calendrier ;
    ' le résumé porte sur toute la période.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherSeances()

        Dim liste As IEnumerable(Of SeancePlanningItem) = FiltresSeances.Filtrer(_seances, FiltresSeances.Axe(cboFiltre))

        Dim texte As String = txtFiltre.Text.Trim()
        If texte.Length > 0 Then
            liste = liste.Where(Function(s) CorrespondTexte(s, texte))
        End If

        Dim affichees As List(Of SeancePlanningItem) = liste.ToList()

        ' Nouvelle liste : l'ordre du calendrier (un tri par en-tête précédent est oublié).
        UtilsDataGrid.ReinitialiserTri(dgvSeances)
        dgvSeances.DataSource = Nothing
        dgvSeances.DataSource = affichees
        dgvSeances.ClearSelection()

        FiltresSeances.MettreAJourResume(_resume, _seances)

        carteResultats.Titre = If(affichees.Count = _seances.Count,
                                  $"Séances ({_seances.Count})",
                                  $"Séances ({affichees.Count} sur {_seances.Count})")

        _context?.SetStatus($"Recherche des séances : {affichees.Count} séance(s) affichée(s).")

        RafraichirBoutons()

    End Sub

    ' Le texte cherché figure-t-il dans le patient, les codes, la nature, le domaine ou le statut ?
    Private Shared Function CorrespondTexte(seance As SeancePlanningItem, texte As String) As Boolean

        Dim champs As String = String.Join(" ", {
            seance.PatientAffichage,
            If(seance.CodePatient, String.Empty),
            If(seance.CodeSeance, String.Empty),
            If(seance.CodeDossier, String.Empty),
            If(seance.LibelleNatureSeance, String.Empty),
            If(seance.LibelleDomaine, String.Empty),
            If(seance.LibelleStatutAffichage, String.Empty)
        })

        Return champs.IndexOf(texte, StringComparison.OrdinalIgnoreCase) >= 0

    End Function

    ' Resélectionne la séance du retour si elle est listée (et la rend visible).
    Private Sub SelectionnerSeance(idSeance As Long)

        For Each ligne As DataGridViewRow In dgvSeances.Rows
            Dim seance As SeancePlanningItem = TryCast(ligne.DataBoundItem, SeancePlanningItem)
            If seance Is Nothing OrElse seance.IdSeance <> idSeance Then Continue For

            dgvSeances.ClearSelection()
            dgvSeances.CurrentCell = ligne.Cells(colPatient.Index)
            ligne.Selected = True
            If Not ligne.Displayed Then dgvSeances.FirstDisplayedScrollingRowIndex = ligne.Index
            dgvSeances.Focus()
            Exit For
        Next

        RafraichirBoutons()

    End Sub

    Private Function SeanceSelectionnee() As SeancePlanningItem
        Return UtilsDataGrid.ElementSelectionne(Of SeancePlanningItem)(dgvSeances)
    End Function

    Private Sub RafraichirBoutons()
        Dim s As SeancePlanningItem = SeanceSelectionnee()
        btnOuvrir.Enabled = s IsNot Nothing
        btnOuvrirPatient.Enabled = s IsNot Nothing AndAlso s.IdPatient > 0
    End Sub

    Private Sub DefinirInfobulle(controle As Control, texte As String)
        _context?.SetToolTip(controle, texte)
    End Sub

#End Region

#Region "Aller à la séance"

    Private Sub Ouvrir(patientSeulement As Boolean)

        Dim seance As SeancePlanningItem = SeanceSelectionnee()
        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If seance Is Nothing OrElse fenetre Is Nothing Then Return

        Dim resultat As ResultatRecherche = GestionRecherche.ResultatDeSeance(seance)

        If patientSeulement Then
            OuvertureResultat.OuvrirPatient(fenetre, resultat, CreerEcranCourant(seance.IdSeance), CONTEXTE)
        Else
            OuvertureResultat.Ouvrir(fenetre, resultat, CreerEcranCourant(seance.IdSeance), _context, CONTEXTE)
        End If

    End Sub

    ' Fabrique de retour : la liste recréée avec ses critères, la séance ouverte resélectionnée.
    Private Function CreerEcranCourant(idSeance As Long) As Func(Of UserControl)

        Dim du As Date = _periode.Du
        Dim au As Date = _periode.Au
        Dim axe As AxeFiltreSeance = FiltresSeances.Axe(cboFiltre)
        Dim texte As String = txtFiltre.Text.Trim()

        Return Function()
                   Dim ecran As New UC_RechercheSeances()
                   ecran.Prerenseigner(du, au, axe, texte, idSeance)
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
        AfficherSeances()
    End Sub

    Private Sub txtFiltre_TextChanged(sender As Object, e As EventArgs) Handles txtFiltre.TextChanged
        If _chargement Then Return
        AfficherSeances()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvSeances_CellPainting
    ' Rôle      : Statut et paiement en pastilles, comme l'onglet Séances du dossier (lignes non dues
    '             atténuées).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvSeances_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvSeances.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If e.ColumnIndex <> colStatut.Index AndAlso e.ColumnIndex <> colPaiement.Index Then Return

        Dim seance As SeancePlanningItem = TryCast(dgvSeances.Rows(e.RowIndex).DataBoundItem, SeancePlanningItem)
        If seance Is Nothing Then Return

        Dim attenuee As Boolean = seance.FamilleStatut = FamilleStatutSeance.NonDue

        If e.ColumnIndex = colStatut.Index Then
            UtilsDataGrid.DessinerPastilleCellule(dgvSeances, e, seance.LibelleStatutAffichage,
                                                  UtilsPastilles.Teinte(seance.FamilleStatut), Nothing, attenuee)
        Else
            UtilsDataGrid.DessinerPastilleCellule(dgvSeances, e, seance.PaiementAffichage,
                                                  UtilsPastilles.Teinte(seance.FamillePaiement), Nothing, attenuee)
        End If

    End Sub

    Private Sub dgvSeances_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvSeances.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            e.Handled = True
            Ouvrir(False)
        End If
    End Sub

    Private Sub dgvSeances_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSeances.CellDoubleClick
        If e.RowIndex >= 0 Then Ouvrir(False)
    End Sub

    Private Sub dgvSeances_SelectionChanged(sender As Object, e As EventArgs) Handles dgvSeances.SelectionChanged
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
