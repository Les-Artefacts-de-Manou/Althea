' -------------------------------------------------------------------------------------------------
' Classe      : UC_PatientFiche — Intervenants (fichier partiel)
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Onglet Intervenants de la fiche patient : suivis par d'autres intervenants (thérapeutes
' du référentiel ou saisis), recherche, ajout, modification et suppression par IntervenantEdition.
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

#Region "Intervenants"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserGrilleIntervenants
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Applique la mise en forme commune (brique réutilisable UtilsDataGrid) à la grille des
    ' intervenants et fige ses propriétés de base, à l'identique de la grille des contacts.
    '
    ' Remarques :
    ' - Les colonnes et leurs DataPropertyName sont définis au Designer (AutoGenerateColumns = False).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserGrilleIntervenants()

        UtilsDataGrid.InitDataGridBasique(dgvIntervenants)

        dgvIntervenants.AutoGenerateColumns = False

        dgvIntervenants.ReadOnly = True
        dgvIntervenants.MultiSelect = False
        dgvIntervenants.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvIntervenants.AllowUserToAddRows = False
        dgvIntervenants.AllowUserToDeleteRows = False

        ' Le bouton Réinitialiser est désactivé par défaut (aucun filtre au chargement)
        btnReinitialiserIntervenants.Enabled = False

        ' Tri par clic sur l'en-tête de colonne (ascendant/descendant).
        UtilsDataGrid.ActiverTriColonnes(dgvIntervenants)
        UtilsDataGrid.AlignerCentre(colIntervenantDateDebut, colIntervenantDateFin)

        ' Pagination : liste filtrée complète en mémoire, seule la page courante est affichée.
        pagIntervenants.Attacher(dgvIntervenants, "intervenant(s)")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerIntervenants
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Recharge la grille des intervenants du patient courant depuis la couche métier et met à jour
    ' l'état des boutons d'action.
    '
    ' Remarques :
    ' - AutoGenerateColumns = False : les colonnes sont définies au Designer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerIntervenants()

        If _idPatient <= 0 Then
            _intervenants = Nothing
            pagIntervenants.DefinirSource(Nothing)
            RafraichirEtatBoutonsIntervenants()
            Return
        End If

        Try

            dgvIntervenants.AutoGenerateColumns = False

            _intervenants = GestionSuivisIntervenants.GetSuivisParPatient(_idPatient)

            AppliquerFiltresIntervenants()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement des intervenants du patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des intervenants.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvIntervenants_CellFormatting
    ' Version   : V1.2.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Affiche les dates de période en format court et masque les valeurs nulles (suivi sans date),
    ' sans altérer la donnée sous-jacente ; une intervention terminée (« Jusqu'au » passé) est
    ' atténuée (UI-9.2 : ce qui est terminé s'efface).
    '
    ' Remarques :
    ' - V1.2.0 (23/10/2026, audit C.2) : intervention terminée selon SuiviIntervenant.EstTerminee.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvIntervenants_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvIntervenants.CellFormatting

        If e.RowIndex < 0 Then
            Return
        End If

        Dim suivi As SuiviIntervenant = TryCast(dgvIntervenants.Rows(e.RowIndex).DataBoundItem, SuiviIntervenant)
        If suivi IsNot Nothing AndAlso suivi.EstTerminee Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

        If e.ColumnIndex <> colIntervenantDateDebut.Index AndAlso
           e.ColumnIndex <> colIntervenantDateFin.Index Then
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
    ' Procédure : dgvIntervenants_SelectionChanged
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Met à jour l'état des boutons Modifier / Supprimer selon la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvIntervenants_SelectionChanged(
        sender As Object,
        e As EventArgs
    ) Handles dgvIntervenants.SelectionChanged

        RafraichirEtatBoutonsIntervenants()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvIntervenants_CellDoubleClick
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Ouvre l'intervenant sélectionné en consultation (double-clic sur une ligne).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvIntervenants_CellDoubleClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles dgvIntervenants.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        Dim suivi As SuiviIntervenant = IntervenantSelectionne()

        If suivi IsNot Nothing Then
            OuvrirEditionIntervenant(suivi, ouvrirEnConsultation:=True)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltresIntervenants
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Filtre la liste mémoire des intervenants selon le texte de recherche (rôle, nom, spécialité,
    ' lieu) et réaffiche le résultat dans la grille, à l'identique des contacts.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltresIntervenants()

        If _intervenants Is Nothing Then
            pagIntervenants.DefinirSource(Nothing)
            RafraichirEtatBoutonsIntervenants()
            Return
        End If

        Try

            Dim intervenantsFiltres = _intervenants.AsEnumerable()

            Dim texteRecherche As String = txtRechercheIntervenant.Text.Trim()
            If Not String.IsNullOrEmpty(texteRecherche) Then
                intervenantsFiltres = intervenantsFiltres.Where(
                    Function(s) CorrespondRechercheIntervenant(s, texteRecherche)
                )
            End If

            Dim resultats = intervenantsFiltres.ToList()

            pagIntervenants.DefinirSource(resultats)

            btnReinitialiserIntervenants.Enabled = Not String.IsNullOrEmpty(texteRecherche)

            If _context IsNot Nothing Then
                _context.SetStatus($"{resultats.Count} intervenant(s) affiché(s).")
            End If

        Finally

            RafraichirEtatBoutonsIntervenants()

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondRechercheIntervenant
    ' Version  : V1.0.0
    ' Date     : 15/06/2026
    '
    ' Rôle     :
    ' Indique si un intervenant correspond au texte de recherche (comparaison insensible à la casse).
    '
    ' Paramètres :
    ' - suivi : L'intervenant à tester
    ' - texte : Le texte recherché (déjà nettoyé)
    '
    ' Retour   :
    ' - Boolean : True si l'un des champs (rôle, nom, spécialité, lieu) contient le texte recherché.
    '
    ' Remarques :
    ' - Les champs optionnels peuvent être Nothing : protégés par If(..., "").
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondRechercheIntervenant(suivi As SuiviIntervenant, texte As String) As Boolean

        Return If(suivi.LibelleRoleIntervenant, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(suivi.NomProfessionnel, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(suivi.Specialite, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(suivi.Lieu, "").Contains(texte, StringComparison.OrdinalIgnoreCase)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnRechercherIntervenant_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Applique le filtre de recherche sur la liste mémoire des intervenants.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRechercherIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRechercherIntervenant.Click

        AppliquerFiltresIntervenants()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtRechercheIntervenant_KeyDown
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Permet de déclencher la recherche des intervenants par la touche Entrée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRechercheIntervenant_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles txtRechercheIntervenant.KeyDown

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            AppliquerFiltresIntervenants()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnReinitialiserIntervenants_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Réinitialise le critère de recherche et réaffiche la liste complète des intervenants.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReinitialiserIntervenants_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnReinitialiserIntervenants.Click

        txtRechercheIntervenant.Clear()

        AppliquerFiltresIntervenants()

        txtRechercheIntervenant.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterIntervenant_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale de création d'un nouvel intervenant pour le patient courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAjouterIntervenant.Click

        OuvrirEditionIntervenant(Nothing)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifierIntervenant_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale de modification de l'intervenant sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifierIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifierIntervenant.Click

        Dim suivi As SuiviIntervenant = IntervenantSelectionne()

        If suivi IsNot Nothing Then
            OuvrirEditionIntervenant(suivi)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnSupprimerIntervenant_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Supprime physiquement l'intervenant sélectionné après confirmation explicite de l'utilisateur.
    '
    ' Remarques :
    ' - La liaison autres_suivis_patient ne comporte pas d'indicateur d'activité : suppression physique.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSupprimerIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnSupprimerIntervenant.Click

        Dim suivi As SuiviIntervenant = IntervenantSelectionne()

        If suivi Is Nothing Then
            Return
        End If

        Dim message As String =
            $"Supprimer définitivement l'intervenant « {suivi.LibelleAffichage} » ?" &
            Environment.NewLine & "Cette action est irréversible."

        If DialogChoix.Confirmer(message, "Suppression d'un intervenant") <> DialogResult.Yes Then
            Return
        End If

        Try

            GestionSuivisIntervenants.DeleteSuivi(suivi.IdAutreSuiviPatient)

            ChargerIntervenants()

            If _context IsNot Nothing Then
                _context.SetStatus("Intervenant supprimé.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur suppression d'un intervenant (UC_PatientFiche, idSuivi={suivi.IdAutreSuiviPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors de la suppression de l'intervenant.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirEditionIntervenant
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale d'édition d'un intervenant (création si suivi = Nothing, sinon
    ' consultation ou modification) et rafraîchit la grille en cas d'enregistrement.
    '
    ' Paramètres :
    ' - suivi                : Intervenant à consulter/modifier, ou Nothing pour une création.
    ' - ouvrirEnConsultation : True pour ouvrir un intervenant existant en lecture seule (double-clic).
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirEditionIntervenant(suivi As SuiviIntervenant, Optional ouvrirEnConsultation As Boolean = False)

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

        If fenetre Is Nothing Then
            Return
        End If

        Using dlg As New IntervenantEdition(_idPatient, fenetre, suivi, ouvrirEnConsultation)

            dlg.SetContext(_context)
            RafraichirContexteNavigation(If(suivi Is Nothing, "Nouvel intervenant", If(ouvrirEnConsultation, "Consultation intervenant", "Modification intervenant")))
            _context?.SetStatus(If(suivi Is Nothing, "Ajout d'un intervenant.", "Ouverture de la fiche intervenant."))

            If dlg.ShowDialog(Me) = DialogResult.OK Then

                ChargerIntervenants()

                If _context IsNot Nothing Then
                    _context.SetStatus(If(suivi Is Nothing, "Intervenant ajouté.", "Intervenant modifié."))
                End If

            End If

        End Using

        RafraichirContexteNavigation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirEtatBoutonsIntervenants
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Active ou désactive les boutons d'action de l'onglet Intervenants selon la présence
    ' d'un patient enregistré et d'une ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirEtatBoutonsIntervenants()

        Dim patientEnregistre As Boolean = _idPatient > 0
        Dim aIntervenantSelectionne As Boolean = IntervenantSelectionne() IsNot Nothing

        btnAjouterIntervenant.Enabled = patientEnregistre
        btnModifierIntervenant.Enabled = patientEnregistre AndAlso aIntervenantSelectionne
        btnSupprimerIntervenant.Enabled = patientEnregistre AndAlso aIntervenantSelectionne

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IntervenantSelectionne
    ' Version  : V1.0.0
    ' Date     : 15/06/2026
    '
    ' Rôle     :
    ' Retourne l'intervenant actuellement sélectionné dans la grille, ou Nothing si aucune ligne.
    ' -------------------------------------------------------------------------------------------------
    Private Function IntervenantSelectionne() As SuiviIntervenant

        If dgvIntervenants.CurrentRow Is Nothing Then
            Return Nothing
        End If

        Return TryCast(dgvIntervenants.CurrentRow.DataBoundItem, SuiviIntervenant)

    End Function

#End Region

End Class
