' -------------------------------------------------------------------------------------------------
' Formulaire  : DialogPaiementsSeance
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Affiche, en lecture seule, l'ensemble des mouvements de paiement (attente, encaissements,
' annulations tracées) d'une séance, avec un résumé tarif / réglé / reste dû / avoir.
'
' Important   :
' - Disposition portée par le Designer (DialogPaiementsSeance.Designer.vb).
' - Ouvert par bouton dédié (le double-clic de la grille ouvre la séance elle-même).
' - V1.1.0 (15/10/2026, UI-7) : résumé et grille dans une carte titrée par la séance, fond
'   AltheaFondFenetre. L'ancien libellé lblTitre, caché sous l'en-tête depuis UI-3, est remplacé
'   par le titre de la carte.
' - V1.2.0 (17/10/2026) : l'en-tête nomme la séance par son code.
' - V1.3.0 (18/10/2026, UI-9.2) : colonne « État » en pastille, mêmes familles et précision que la
'   grille des paiements du dossier ; ligne neutralisée en gris (teintes UITheme, au lieu d'un gris
'   et d'une police barrée recréée à chaque cellule) ; date centrée.
' - V1.4.0 (23/10/2026, relevé de Joëlle) : colonne « Code » (PAI…) : le code d'un paiement ne se voyait
'   nulle part.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class DialogPaiementsSeance

    Public Sub New(seance As SeanceListeItem, paiements As List(Of PaiementListeItem))

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée.
        AltheaFondFenetre.Appliquer(Me)

        ' Charte Althéa : grille stylée par l'utilitaire commun (source unique UITheme).
        UtilsDataGrid.InitDataGridBasique(dgvPaiements)
        UtilsDataGrid.AlignerCentre(colDate)

        enTete.SousTitre = $"Détail des paiements de la séance {seance.CodeSeance}"

        carteSeance.Titre = String.Format("Séance {0} du {1:dd/MM/yyyy} - {2}", seance.CodeSeance, seance.DateSeance, seance.LibelleStatutSeance)

        Dim texteResume As String = String.Format(
            "Tarif : {0:N2} €   |   Réglé : {1:N2} €   |   Reste dû : {2:N2} €",
            seance.TarifSeance, seance.MontantRegle, seance.ResteDu)

        If seance.AvoirSeance > 0D Then
            texteResume &= String.Format("   |   Avoir : {0:N2} €", seance.AvoirSeance)
        End If

        If paiements Is Nothing OrElse paiements.Count = 0 Then
            texteResume &= Environment.NewLine & "Aucun mouvement de paiement enregistré pour cette séance."
        End If

        lblResume.Text = texteResume
        dgvPaiements.DataSource = If(paiements, New List(Of PaiementListeItem)())

    End Sub

    ' Montant d'un paiement annulé : barré (police partagée, jamais recréée à chaque cellule).
    Private Shared ReadOnly PoliceMontantAnnule As New Font("Calibri", 10.0F, FontStyle.Strikeout)

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPaiements_CellFormatting
    ' Version   : V1.1.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Ligne neutralisée (paiement annulé ou reporté) en gris ; montant d'un paiement annulé barré.
    ' Mêmes règles que la grille des paiements du dossier.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPaiements_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvPaiements.CellFormatting

        If e.RowIndex < 0 Then Return

        Dim item As PaiementListeItem = TryCast(dgvPaiements.Rows(e.RowIndex).DataBoundItem, PaiementListeItem)
        If item Is Nothing Then Return

        If item.EstNeutralise Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

        If e.ColumnIndex = colMontant.Index AndAlso item.EstAnnule Then
            e.CellStyle.Font = PoliceMontantAnnule
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPaiements_CellPainting
    ' Version   : V1.0.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Pastille « État » (ce que le paiement est devenu), colorée par famille comme dans la grille des
    ' paiements du dossier ; atténuée, avec sa précision grise, sur une ligne neutralisée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPaiements_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvPaiements.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colStatut.Index Then Return

        Dim item As PaiementListeItem = TryCast(dgvPaiements.Rows(e.RowIndex).DataBoundItem, PaiementListeItem)
        If item Is Nothing Then Return

        UtilsDataGrid.DessinerPastilleCellule(dgvPaiements, e, item.EtatAffichage,
                                              UtilsPastilles.Teinte(item.FamilleEtat),
                                              item.MentionEtat, item.EstNeutralise)

    End Sub


End Class
