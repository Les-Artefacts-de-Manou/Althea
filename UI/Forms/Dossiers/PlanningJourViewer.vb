' -------------------------------------------------------------------------------------------------
' Formulaire  : PlanningJourViewer
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Affiche, sous forme de grille en lecture seule, les séances déjà planifiées pour une date
' donnée (tous dossiers/patients confondus), afin d'aider l'utilisateur à repérer les créneaux
' libres avant l'arrivée de l'agenda Google (Lot ultérieur). Ce n'est qu'une aide visuelle : le
' contrôle de non-chevauchement métier (GestionSeances.ChercherChevauchement) reste la garantie.
'
' Responsabilités :
' - Charger et afficher les séances du jour choisi (heure, patient, domaine, code, statut)
' - Permettre de changer de jour depuis la fenêtre (navigation rapide, sans rouvrir la modale)
'
' Important   :
' - Fenêtre de pure consultation : aucune modification n'est possible depuis cette grille.
' - Disposition portée par le Designer (PlanningJourViewer.Designer.vb).
' - V1.2.0 (15/10/2026, UI-7) : date et grille dans une carte « Séances du jour », fond
'   AltheaFondFenetre, date AltheaDate.
' - V1.3.0 (18/10/2026, UI-9.2, maquette « Grilles — états lisibles ») : statut en pastille (mêmes
'   familles que la grille des séances), créneaux libres en repères pointillés entre les séances
'   (GestionSeances.GetPlanningJour), heures centrées, tarif à droite ; erreur par DialogChoix.
' - V1.4.0 (24/10/2026) : colonne Code de la séance (code toujours visible, Rules § 15) ; fenêtre
'   élargie de 80 px pour la loger sans défilement.
'
' Dépendances :
' - GestionSeances (GetPlanningJour) / LignePlanningJour (séances et créneaux libres).
' - UtilsDataGrid, UtilsPastilles et UITheme (présentation cohérente de la grille), DialogChoix.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Partial Public Class PlanningJourViewer

#Region "Variables privées"

    Private _dateAffichee As Date

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V1.0.0
    ' Date          : 16/01/2025
    '
    ' Rôle          :
    ' Initialise la fenêtre de consultation du planning pour la date fournie.
    '
    ' Paramètres    :
    ' - dateInitiale : Date à afficher à l'ouverture
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(dateInitiale As Date)

        _dateAffichee = dateInitiale.Date

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée.
        AltheaFondFenetre.Appliquer(Me)

        ' Charte Althéa : grille et bouton stylés par les utilitaires communs (source unique UITheme).
        UtilsDataGrid.InitDataGridBasique(dgvPlanning)
        dgvPlanning.RowHeadersVisible = False
        colHeureDebut.DefaultCellStyle.Format = "hh\:mm"
        colHeureFin.DefaultCellStyle.Format = "hh\:mm"
        colTarif.DefaultCellStyle.Format = "N2"
        UtilsDataGrid.AlignerCentre(colHeureDebut, colHeureFin)
        UtilsDataGrid.AlignerDroite(colTarif)
        UtilsButtons.InitStandardButton(btnFermer)

        dtpDate.Valeur = _dateAffichee
        AddHandler dtpDate.ValeurChangee, AddressOf dtpDate_ValeurChangee
        ChargerPlanning()

    End Sub

#End Region

#Region "Chargement des données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerPlanning
    ' Version   : V1.1.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Charge les séances du jour sélectionné et les créneaux libres entre elles, dans l'ordre des
    ' heures (GestionSeances.GetPlanningJour).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerPlanning()

        Try

            Dim lignes As List(Of LignePlanningJour) = GestionSeances.GetPlanningJour(_dateAffichee)

            enTete.SousTitre = $"Séances planifiées ({_dateAffichee:dddd dd MMMM yyyy})"
            dgvPlanning.DataSource = lignes

            If Not lignes.Any(Function(l) Not l.EstLibre) Then
                enTete.SousTitre &= " — Aucune séance"
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement planning du jour (PlanningJourViewer, date={_dateAffichee:yyyy-MM-dd}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur("Le planning du jour n'a pas pu être chargé. Le détail a été noté dans le journal.",
                               "Planning du jour")

        End Try

    End Sub

#End Region

#Region "Événements"

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dtpDate_ValeurChangee
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Recharge le planning lorsque l'utilisateur change de date via le sélecteur.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dtpDate_ValeurChangee(sender As Object, e As EventArgs)

        _dateAffichee = dtpDate.Valeur.Value.Date
        ChargerPlanning()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPlanning_DataBindingComplete
    ' Version   : V1.0.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Prépare les lignes des créneaux libres : fond uni et sélection invisible (ce sont des repères,
    ' pas des lignes à choisir) ; aucune ligne présélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPlanning_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) _
        Handles dgvPlanning.DataBindingComplete

        For Each ligne As DataGridViewRow In dgvPlanning.Rows
            Dim item As LignePlanningJour = TryCast(ligne.DataBoundItem, LignePlanningJour)
            If item Is Nothing OrElse Not item.EstLibre Then Continue For
            ligne.DefaultCellStyle.BackColor = dgvPlanning.DefaultCellStyle.BackColor
            ligne.DefaultCellStyle.SelectionBackColor = dgvPlanning.DefaultCellStyle.BackColor
            ligne.DefaultCellStyle.SelectionForeColor = dgvPlanning.DefaultCellStyle.ForeColor
        Next

        dgvPlanning.ClearSelection()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPlanning_CellPainting
    ' Version   : V1.0.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Statut en pastille (couleurs de la grille des séances) ; cellules d'un créneau libre laissées
    ' vides (le repère est dessiné sur toute la ligne par RowPostPaint).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPlanning_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) _
        Handles dgvPlanning.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        Dim item As LignePlanningJour = TryCast(dgvPlanning.Rows(e.RowIndex).DataBoundItem, LignePlanningJour)
        If item Is Nothing Then Return

        If item.EstLibre Then
            e.PaintBackground(e.CellBounds, False)
            e.Handled = True
        ElseIf e.ColumnIndex = colStatut.Index Then
            UtilsDataGrid.DessinerPastilleCellule(dgvPlanning, e, item.LibelleStatutSeance,
                                                  UtilsPastilles.Teinte(item.Seance.FamilleStatut))
        End If

    End Sub

    ' Repère pointillé « Libre · 10:20 – 14:00 » sur toute la ligne d'un créneau libre.
    Private Sub dgvPlanning_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) _
        Handles dgvPlanning.RowPostPaint

        Dim item As LignePlanningJour = TryCast(dgvPlanning.Rows(e.RowIndex).DataBoundItem, LignePlanningJour)
        If item IsNot Nothing AndAlso item.EstLibre Then
            UtilsDataGrid.DessinerRepereSurLigne(e, dgvPlanning, item.LibelleLibre, pointille:=True)
        End If

    End Sub

#End Region

End Class
