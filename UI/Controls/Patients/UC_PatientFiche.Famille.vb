' -------------------------------------------------------------------------------------------------
' Classe      : UC_PatientFiche — Famille / Contacts (fichier partiel)
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Onglet Famille / Contacts de la fiche patient : grille des contacts, recherche, ajout,
' modification, consultation et suppression par la fenêtre ContactEdition.
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

#Region "Famille / Contacts"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserGrilleContacts
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Applique la mise en forme commune (brique réutilisable UtilsDataGrid) à la grille des contacts
    ' et fige ses propriétés de base, à l'identique de la liste patients (UC_PatientHome).
    '
    ' Remarques :
    ' - Les colonnes et leurs DataPropertyName sont définis au Designer (AutoGenerateColumns = False).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserGrilleContacts()

        UtilsDataGrid.InitDataGridBasique(dgvContacts)

        dgvContacts.AutoGenerateColumns = False

        dgvContacts.ReadOnly = True
        dgvContacts.MultiSelect = False
        dgvContacts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvContacts.AllowUserToAddRows = False
        dgvContacts.AllowUserToDeleteRows = False

        ' Le bouton Réinitialiser est désactivé par défaut (aucun filtre au chargement)
        btnReinitialiserContacts.Enabled = False

        ' Tri par clic sur l'en-tête de colonne (ascendant/descendant).
        UtilsDataGrid.ActiverTriColonnes(dgvContacts)

        ' Pagination : liste filtrée complète en mémoire, seule la page courante est affichée.
        pagContacts.Attacher(dgvContacts, "contact(s)")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : tabFiche_Selected
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Charge (à la demande) la liste des contacts du patient lorsque l'onglet Famille / Contacts
    ' est sélectionné.
    '
    ' Remarques :
    ' - L'onglet n'est accessible qu'avec un patient enregistré (activation progressive D-Q14).
    ' -------------------------------------------------------------------------------------------------
    Private Sub tabFiche_Selected(
        sender As Object,
        e As TabControlEventArgs
    ) Handles tabFiche.Selected

        If e.TabPage Is tabPageFamille AndAlso _idPatient > 0 Then
            ChargerContacts()
        ElseIf e.TabPage Is tabPageIntervenants AndAlso _idPatient > 0 Then
            ChargerIntervenants()
        ElseIf e.TabPage Is tabPageDossiers AndAlso _idPatient > 0 Then
            ChargerDossiers()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerContacts
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Recharge la grille des contacts du patient courant depuis la couche métier et met à jour
    ' l'état des boutons d'action.
    '
    ' Remarques :
    ' - AutoGenerateColumns = False : les colonnes (dont les pictos de rôle) sont définies au Designer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerContacts()

        If _idPatient <= 0 Then
            _contacts = Nothing
            pagContacts.DefinirSource(Nothing)
            RafraichirEtatBoutonsContacts()
            Return
        End If

        Try

            dgvContacts.AutoGenerateColumns = False

            _contacts = GestionFamilleContacts.GetContactsParPatient(_idPatient)

            AppliquerFiltresContacts()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement des contacts du patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des contacts.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvContacts_CellFormatting
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Met en forme l'affichage du téléphone du contact (masque international lisible) sans altérer
    ' la donnée sous-jacente (brique réutilisable UtilsTelephone).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvContacts_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvContacts.CellFormatting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colContactTelephone.Index Then
            Return
        End If

        Dim valeur As String = TryCast(e.Value, String)

        If String.IsNullOrWhiteSpace(valeur) Then
            Return
        End If

        e.Value = UtilsTelephone.FormaterAffichage(valeur, Nothing)
        e.FormattingApplied = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvContacts_SelectionChanged
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Met à jour l'état des boutons Modifier / Supprimer selon la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvContacts_SelectionChanged(
        sender As Object,
        e As EventArgs
    ) Handles dgvContacts.SelectionChanged

        RafraichirEtatBoutonsContacts()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvContacts_CellDoubleClick
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre le contact sélectionné en consultation (double-clic sur une ligne).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvContacts_CellDoubleClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles dgvContacts.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        Dim contact As FamilleContact = ContactSelectionne()

        If contact IsNot Nothing Then
            OuvrirEditionContact(contact, ouvrirEnConsultation:=True)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltresContacts
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Filtre la liste mémoire des contacts selon le texte de recherche (nom, prénom, lien) et
    ' réaffiche le résultat dans la grille, à l'identique du fonctionnement de UC_PatientHome.
    '
    ' Remarques :
    ' - Recherche simple multi-champs, insensible à la casse, déclenchée par Entrée ou bouton.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltresContacts()

        If _contacts Is Nothing Then
            pagContacts.DefinirSource(Nothing)
            RafraichirEtatBoutonsContacts()
            Return
        End If

        Try

            Dim contactsFiltres = _contacts.AsEnumerable()

            Dim texteRecherche As String = txtRechercheContact.Text.Trim()
            If Not String.IsNullOrEmpty(texteRecherche) Then
                contactsFiltres = contactsFiltres.Where(
                    Function(c) CorrespondRechercheContact(c, texteRecherche)
                )
            End If

            Dim resultats = contactsFiltres.ToList()

            pagContacts.DefinirSource(resultats)

            btnReinitialiserContacts.Enabled = Not String.IsNullOrEmpty(texteRecherche)

            If _context IsNot Nothing Then
                _context.SetStatus($"{resultats.Count} contact(s) affiché(s).")
            End If

        Finally

            RafraichirEtatBoutonsContacts()

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondRechercheContact
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     :
    ' Indique si un contact correspond au texte de recherche (comparaison insensible à la casse).
    '
    ' Paramètres :
    ' - contact : Le contact à tester
    ' - texte   : Le texte recherché (déjà nettoyé)
    '
    ' Retour   :
    ' - Boolean : True si l'un des champs (nom, prénom, lien, rôle légal) contient le texte recherché.
    '
    ' Remarques :
    ' - Les champs optionnels peuvent être Nothing : protégés par If(..., "").
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondRechercheContact(contact As FamilleContact, texte As String) As Boolean

        Return If(contact.Nom, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(contact.Prenom, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(contact.LibelleLienPatient, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(contact.LibelleRoleLegal, "").Contains(texte, StringComparison.OrdinalIgnoreCase)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnRechercherContact_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Applique le filtre de recherche sur la liste mémoire des contacts.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRechercherContact_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRechercherContact.Click

        AppliquerFiltresContacts()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtRechercheContact_KeyDown
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Permet de déclencher la recherche des contacts par la touche Entrée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRechercheContact_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles txtRechercheContact.KeyDown

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            AppliquerFiltresContacts()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnReinitialiserContacts_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Réinitialise le critère de recherche et réaffiche la liste complète des contacts.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReinitialiserContacts_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnReinitialiserContacts.Click

        txtRechercheContact.Clear()

        AppliquerFiltresContacts()

        txtRechercheContact.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterContact_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale de création d'un nouveau contact pour le patient courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterContact_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAjouterContact.Click

        OuvrirEditionContact(Nothing)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifierContact_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale de modification du contact sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifierContact_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifierContact.Click

        Dim contact As FamilleContact = ContactSelectionne()

        If contact IsNot Nothing Then
            OuvrirEditionContact(contact)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnSupprimerContact_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Supprime physiquement le contact sélectionné après confirmation explicite de l'utilisateur.
    '
    ' Remarques :
    ' - La table famille_contacts ne comporte pas d'indicateur d'activité : la suppression est physique.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSupprimerContact_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnSupprimerContact.Click

        Dim contact As FamilleContact = ContactSelectionne()

        If contact Is Nothing Then
            Return
        End If

        Dim message As String =
            $"Supprimer définitivement le contact « {contact.NomComplet} » ?" &
            Environment.NewLine & "Cette action est irréversible."

        If DialogChoix.Confirmer(message, "Suppression d'un contact") <> DialogResult.Yes Then
            Return
        End If

        Try

            GestionFamilleContacts.DeleteContact(contact.IdFamilleContact)

            ChargerContacts()

            If _context IsNot Nothing Then
                _context.SetStatus("Contact supprimé.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur suppression d'un contact (UC_PatientFiche, idContact={contact.IdFamilleContact}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors de la suppression du contact.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirEditionContact
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale d'édition d'un contact (création si contact = Nothing, sinon
    ' consultation ou modification) et rafraîchit la grille en cas d'enregistrement.
    '
    ' Paramètres :
    ' - contact              : Contact à consulter/modifier, ou Nothing pour une création.
    ' - ouvrirEnConsultation : True pour ouvrir un contact existant en lecture seule (double-clic).
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirEditionContact(contact As FamilleContact, Optional ouvrirEnConsultation As Boolean = False)

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

        If fenetre Is Nothing Then
            Return
        End If

        Using dlg As New ContactEdition(_idPatient, fenetre, contact, _patient, ouvrirEnConsultation)

            dlg.SetContext(_context)
            RafraichirContexteNavigation(If(contact Is Nothing, "Nouveau contact", If(ouvrirEnConsultation, "Consultation contact", "Modification contact")))
            _context?.SetStatus(If(contact Is Nothing, "Ajout d'un contact familial.", "Ouverture de la fiche contact."))

            If dlg.ShowDialog(Me) = DialogResult.OK Then

                ChargerContacts()

                If _context IsNot Nothing Then
                    _context.SetStatus(If(contact Is Nothing, "Contact ajouté.", "Contact modifié."))
                End If

            End If

        End Using

        RafraichirContexteNavigation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirEtatBoutonsContacts
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Active ou désactive les boutons d'action de l'onglet Famille / Contacts selon la présence
    ' d'un patient enregistré et d'une ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirEtatBoutonsContacts()

        Dim patientEnregistre As Boolean = _idPatient > 0
        Dim aContactSelectionne As Boolean = ContactSelectionne() IsNot Nothing

        btnAjouterContact.Enabled = patientEnregistre
        btnModifierContact.Enabled = patientEnregistre AndAlso aContactSelectionne
        btnSupprimerContact.Enabled = patientEnregistre AndAlso aContactSelectionne

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ContactSelectionne
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     :
    ' Retourne le contact actuellement sélectionné dans la grille, ou Nothing si aucune ligne.
    '
    ' Retour   :
    ' - FamilleContact? : Contact ou Nothing
    ' -------------------------------------------------------------------------------------------------
    Private Function ContactSelectionne() As FamilleContact

        If dgvContacts.CurrentRow Is Nothing Then
            Return Nothing
        End If

        Return TryCast(dgvContacts.CurrentRow.DataBoundItem, FamilleContact)

    End Function

#End Region

End Class
