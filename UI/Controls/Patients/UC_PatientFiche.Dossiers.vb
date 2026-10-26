' -------------------------------------------------------------------------------------------------
' Classe      : UC_PatientFiche — Dossiers (fichier partiel)
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Onglet Dossiers de la fiche patient : dossiers de suivi du patient, création et ouverture
' d'un dossier (UC_DossierFiche) avec retour à la fiche.
'
' Remarques   :
' - Audit C.5 (décision de Joëlle du 25/10/2026) : la fiche est répartie en fichiers partiels, un par
'   onglet ; cette région est déplacée telle quelle depuis UC_PatientFiche.vb (V1.12.2), sans
'   changement de comportement. Les procédures gardent leur en-tête et leur historique.
' - Champs, contexte et méthodes communes : UC_PatientFiche.vb ; contrôles : UC_PatientFiche.Designer.vb.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Partial Class UC_PatientFiche

#Region "Dossiers"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserGrilleDossiers
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Applique la mise en forme commune (brique réutilisable UtilsDataGrid) à la grille des
    ' dossiers et fige ses propriétés de base, à l'identique de la grille des intervenants.
    '
    ' Remarques :
    ' - Les colonnes et leurs DataPropertyName sont définis au Designer (AutoGenerateColumns = False).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserGrilleDossiers()

        UtilsDataGrid.InitDataGridBasique(dgvDossiers)

        dgvDossiers.AutoGenerateColumns = False

        dgvDossiers.ReadOnly = True
        dgvDossiers.MultiSelect = False
        dgvDossiers.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvDossiers.AllowUserToAddRows = False
        dgvDossiers.AllowUserToDeleteRows = False

        ' Tri par clic sur l'en-tête de colonne (ascendant/descendant).
        UtilsDataGrid.ActiverTriColonnes(dgvDossiers)
        UtilsDataGrid.AlignerCentre(colDossierDateOuverture, colDossierDateCloture)

        RafraichirEtatBoutonsDossiers()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerDossiers
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Recharge la grille des dossiers du patient courant depuis la couche métier et met à jour
    ' l'état des boutons d'action.
    '
    ' Remarques :
    ' - AutoGenerateColumns = False : les colonnes sont définies au Designer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerDossiers()

        If _idPatient <= 0 Then
            dgvDossiers.DataSource = Nothing
            RafraichirEtatBoutonsDossiers()
            Return
        End If

        Try

            dgvDossiers.AutoGenerateColumns = False

            dgvDossiers.DataSource = Nothing
            dgvDossiers.DataSource = GestionDossiers.GetDossiersParPatient(_idPatient)

            RafraichirEtatBoutonsDossiers()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement des dossiers du patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des dossiers.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvDossiers_CellFormatting
    ' Version   : V1.1.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Affiche les dates d'ouverture / clôture en format court et masque les valeurs nulles
    ' (dossier non clôturé), sans altérer la donnée sous-jacente ; un dossier clôturé ou archivé
    ' est atténué (UI-9.2).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvDossiers_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvDossiers.CellFormatting

        If e.RowIndex < 0 Then
            Return
        End If

        Dim dossier As DossierListeItem = TryCast(dgvDossiers.Rows(e.RowIndex).DataBoundItem, DossierListeItem)
        If dossier IsNot Nothing AndAlso (dossier.EstCloture OrElse dossier.EstArchive) Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

        If e.ColumnIndex <> colDossierDateOuverture.Index AndAlso
           e.ColumnIndex <> colDossierDateCloture.Index Then
            Return
        End If

        If e.Value Is Nothing OrElse e.Value Is DBNull.Value Then
            e.Value = String.Empty
            e.FormattingApplied = True
            Return
        End If

        Dim valeur As Date
        If Date.TryParse(e.Value.ToString(), valeur) Then
            e.Value = valeur.ToShortDateString()
            e.FormattingApplied = True
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvDossiers_CellPainting
    ' Version   : V1.0.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Statut du dossier en pastille, mêmes couleurs que la colonne « Suivi » de la grille Patients
    ' (UtilsPastilles.TeinteSuiviListe : sauge actif, ocre en pause, gris pâle terminé).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvDossiers_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) _
        Handles dgvDossiers.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colDossierStatut.Index Then Return

        Dim dossier As DossierListeItem = TryCast(dgvDossiers.Rows(e.RowIndex).DataBoundItem, DossierListeItem)
        If dossier Is Nothing Then Return

        UtilsDataGrid.DessinerPastilleCellule(dgvDossiers, e, dossier.LibelleStatutDossier,
                                              UtilsPastilles.TeinteSuiviListe(dossier.CodeStatutDossier))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvDossiers_SelectionChanged
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Met à jour l'état du bouton Ouvrir selon la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvDossiers_SelectionChanged(
        sender As Object,
        e As EventArgs
    ) Handles dgvDossiers.SelectionChanged

        RafraichirEtatBoutonsDossiers()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvDossiers_CellDoubleClick
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Ouvre le dossier sélectionné dans son hub (double-clic sur une ligne).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvDossiers_CellDoubleClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles dgvDossiers.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        OuvrirDossierSelectionne()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterDossier_Click
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Ouvre le hub dossier (UC_DossierFiche) en création pour le patient courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterDossier_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAjouterDossier.Click

        If _idPatient <= 0 Then
            Return
        End If

        Dim hub As New UC_DossierFiche()
        hub.InitialiserPourNouveauDossier(_idPatient)

        OuvrirHubDossier(hub, "Nouveau dossier")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnOuvrirDossier_Click
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Ouvre le dossier sélectionné dans son hub.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnOuvrirDossier_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnOuvrirDossier.Click

        OuvrirDossierSelectionne()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirDossierSelectionne
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Ouvre le dossier actuellement sélectionné dans la grille dans son hub (UC_DossierFiche).
    '
    ' Remarques :
    ' - Sans sélection, informe l'utilisateur via le status et n'ouvre rien.
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirDossierSelectionne()

        Dim dossier As DossierListeItem = DossierSelectionne()

        If dossier Is Nothing Then
            If _context IsNot Nothing Then
                _context.SetStatus("Aucun dossier sélectionné.")
            End If
            Return
        End If

        Dim hub As New UC_DossierFiche()
        hub.InitialiserPourDossier(dossier.IdDossier)

        OuvrirHubDossier(hub, dossier.CodeDossier)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirHubDossier
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Empile la fiche patient courante puis navigue vers le hub dossier (mini-pile D-Q15), afin
    ' de restaurer la fiche patient sur l'onglet Dossiers au retour.
    '
    ' Paramètres :
    ' - hub               : Hub dossier déjà instancié et initialisé (UserControl)
    ' - libelleDossier    : Segment de contexte représentant le dossier (ex. code ou « Nouveau dossier ») (String)
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirHubDossier(hub As UC_DossierFiche, libelleDossier As String)

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

        If fenetre Is Nothing Then
            Return
        End If

        Dim nomPatient As String = If(_patient IsNot Nothing, _patient.NomComplet, String.Empty)

        fenetre.NavigateToDossierFiche(
            hub,
            $"Patients > {nomPatient} > {libelleDossier}",
            CreerFicheDossiers()
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CreerFicheDossiers
    ' Version  : V1.0.0
    ' Date     : 21/07/2026
    '
    ' Rôle     :
    ' Construit une fabrique recréant la fiche patient courante et la repositionnant sur l'onglet
    ' Dossiers au retour du hub dossier (mini-pile D-Q15).
    '
    ' Retour   :
    ' - Func(Of UserControl) : fabrique d'une nouvelle UC_PatientFiche restaurée sur l'onglet Dossiers
    ' -------------------------------------------------------------------------------------------------
    Private Function CreerFicheDossiers() As Func(Of UserControl)

        Dim idPatient As Long = _idPatient
        Dim enModification As Boolean = _mode = ModeFiche.Modification

        Return Function()
                   Dim fiche As New UC_PatientFiche()
                   fiche.InitialiserPourPatient(idPatient, enModification)
                   fiche.SelectionnerOngletDossiers()
                   Return fiche
               End Function

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirEtatBoutonsDossiers
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Active ou désactive les boutons d'action de l'onglet Dossiers selon la présence d'un patient
    ' enregistré et d'une ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirEtatBoutonsDossiers()

        Dim patientEnregistre As Boolean = _idPatient > 0
        Dim aDossierSelectionne As Boolean = DossierSelectionne() IsNot Nothing

        btnAjouterDossier.Enabled = patientEnregistre
        btnOuvrirDossier.Enabled = patientEnregistre AndAlso aDossierSelectionne

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : DossierSelectionne
    ' Version  : V1.0.0
    ' Date     : 21/07/2026
    '
    ' Rôle     :
    ' Retourne le dossier actuellement sélectionné dans la grille, ou Nothing si aucune ligne.
    ' -------------------------------------------------------------------------------------------------
    Private Function DossierSelectionne() As DossierListeItem

        If dgvDossiers.CurrentRow Is Nothing Then
            Return Nothing
        End If

        Return TryCast(dgvDossiers.CurrentRow.DataBoundItem, DossierListeItem)

    End Function

#End Region

End Class
