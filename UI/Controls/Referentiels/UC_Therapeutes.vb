' -------------------------------------------------------------------------------------------------
' UserControl : UC_Therapeutes
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 16/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des thérapeutes (table therapeutes). Entité riche traitée
' selon le patron « Liste + Form modale » (comme UC_Utilisateurs / UtilisateurEdition),
' accessible depuis le hub Référentiels.
'
' Responsabilités :
' - Afficher la liste des thérapeutes (recherche texte + affichage des inactifs)
' - Ouvrir la Form TherapeuteEdition en création / modification
' - Activer / désactiver un thérapeute (soft-delete)
' - Supprimer physiquement un thérapeute non référencé (sinon proposer la désactivation)
' - Utiliser le contexte UI partagé fourni par Home
' - Journaliser les actions via GestionLog
'
' Remarques   :
' - Implémente IContextAwareUserControl
' - Aucun accès direct à la base de données (tout passe par GestionTherapeutes)
'
' Dépendances :
' - GestionTherapeutes / Therapeute (couche métier et modèle)
' - UserControlContext (contexte UI injecté par Home)
' - UtilsDataGrid (style du DataGridView)
' - DialogChoix (confirmations et messages)
' - TherapeuteEdition (Form de création / modification)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Therapeutes
    Implements IContextAwareUserControl

#Region "Variables privées"

    'Injecté par Home via SetContext() (implémentation IContextAwareUserControl)
    Private _context As UserControlContext

    ' Liste complète des thérapeutes chargée depuis la base (filtrage en mémoire).
    Private _therapeutes As List(Of Therapeute)

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Reçoit le contexte UI partagé injecté par Home.
    '
    ' Paramètres :
    ' - context : Contexte UI partagé de l'application.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) _
        Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

#End Region

#Region "Chargement UserControl"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_Therapeutes_Load
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Initialise le UserControl et charge la liste des thérapeutes.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_Therapeutes_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If _context IsNot Nothing Then
            _context.SetHeader("Référentiels > Thérapeutes")
            _context.SetStatus("Chargement de la liste des thérapeutes...")
        End If

        InitialiserDataGridView()
        InitialiserToolTips()
        InitialiserBoutons()
        ChargerTherapeutes()

    End Sub

#End Region

#Region "Initialisation UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutons
    ' Version   : V1.0.0
    ' Date      : 21/05/2026
    '
    ' Rôle      :
    ' Initialise les boutons standards du UserControl.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutons()

        UtilsButtons.InitStandardButton(btnNouveau)
        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnActiverDesactiver)
        UtilsButtons.InitStandardButton(btnSupprimer)
        UtilsButtons.InitStandardButton(btnActualiser)
        UtilsButtons.InitStandardButton(btnReinitialiserFiltres)

        ' Le bouton Réinitialiser est désactivé par défaut (aucun filtre au chargement)
        '   btnReinitialiserFiltres.Enabled = False

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserDataGridView
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Configure le DataGridView des thérapeutes (style et liaison des colonnes).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserDataGridView()

        UtilsDataGrid.InitDataGridBasique(dgvTherapeutes)

        dgvTherapeutes.AutoGenerateColumns = False
        dgvTherapeutes.ReadOnly = True
        dgvTherapeutes.MultiSelect = False
        dgvTherapeutes.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvTherapeutes.AllowUserToAddRows = False
        dgvTherapeutes.AllowUserToDeleteRows = False

        colIdTherapeute.DataPropertyName = NameOf(Therapeute.IdTherapeute)
        colCodeTherapeute.DataPropertyName = NameOf(Therapeute.CodeTherapeute)
        colNom.DataPropertyName = NameOf(Therapeute.Nom)
        colPrenom.DataPropertyName = NameOf(Therapeute.Prenom)
        colSpecialite.DataPropertyName = NameOf(Therapeute.Specialite)
        colTelephone.DataPropertyName = NameOf(Therapeute.Telephone)
        colEmail.DataPropertyName = NameOf(Therapeute.Email)
        colLocalite.DataPropertyName = NameOf(Therapeute.Localite)
        colActif.DataPropertyName = NameOf(Therapeute.Actif)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Initialise les aides contextuelles (infobulles) des boutons d'action.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        _context.SetToolTip(btnNouveau, "Créer un nouveau thérapeute.")
        _context.SetToolTip(btnModifier, "Modifier le thérapeute sélectionné.")
        _context.SetToolTip(btnActiverDesactiver, "Activer ou désactiver le thérapeute sélectionné.")
        _context.SetToolTip(btnSupprimer, "Supprimer le thérapeute sélectionné (si non utilisé).")
        _context.SetToolTip(btnActualiser, "Recharger la liste des thérapeutes.")
        _context.SetToolTip(btnRechercher, "Appliquer le filtre de recherche.")
        _context.SetToolTip(btnReinitialiserFiltres, "Réinitialiser le filtre de recherche.")
        _context.SetToolTip(chkAfficherInactifs, "Afficher également les thérapeutes désactivés.")

    End Sub

#End Region

#Region "Chargement données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerTherapeutes
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Charge la liste complète des thérapeutes (actifs + inactifs) puis applique le filtre courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerTherapeutes()

        Try

            Cursor = Cursors.WaitCursor

            _therapeutes = GestionTherapeutes.GetTherapeutes(afficherInactifs:=True)

            AppliquerFiltres()
            MettreAJourEtatBoutons()

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur chargement thérapeutes.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des thérapeutes.")
            End If

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltres
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Applique le filtre texte (nom + prénom + spécialité) et l'option d'affichage des inactifs.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltres()

        If _therapeutes Is Nothing Then
            Return
        End If

        Dim therapeutesFiltres = _therapeutes.AsEnumerable()

        ' Filtre : affichage des inactifs
        If Not chkAfficherInactifs.Checked Then
            therapeutesFiltres = therapeutesFiltres.Where(Function(t) t.Actif)
        End If

        ' Filtre : texte de recherche (nom + prénom + spécialité)
        Dim texteRecherche As String = txtRecherche.Text.Trim()

        If Not String.IsNullOrEmpty(texteRecherche) Then
            therapeutesFiltres = therapeutesFiltres.Where(
                Function(t) CorrespondRecherche(t, texteRecherche))
        End If

        Dim resultats = therapeutesFiltres.OrderBy(Function(t) t.Nom).ThenBy(Function(t) t.Prenom).ToList()

        dgvTherapeutes.DataSource = Nothing
        dgvTherapeutes.DataSource = resultats
        dgvTherapeutes.ClearSelection()

        If _context IsNot Nothing Then
            _context.SetStatus($"{resultats.Count} thérapeute(s) affiché(s).")
        End If

        btnReinitialiserFiltres.Enabled = Not String.IsNullOrWhiteSpace(txtRecherche.Text) OrElse chkAfficherInactifs.Checked

        MettreAJourEtatBoutons()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondRecherche
    ' Version  : V1.0.0
    ' Date     : 16/06/2026
    '
    ' Rôle     :
    ' Indique si un thérapeute correspond au texte recherché (nom, prénom ou spécialité).
    '
    ' Paramètres :
    ' - therapeute     : Thérapeute à tester
    ' - texteRecherche : Texte saisi par l'utilisateur
    '
    ' Retour   :
    ' - Boolean : True si l'un des champs contient le texte (insensible à la casse)
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondRecherche(therapeute As Therapeute, texteRecherche As String) As Boolean

        Return CChamps(therapeute.Nom).Contains(texteRecherche, StringComparison.OrdinalIgnoreCase) OrElse
               CChamps(therapeute.Prenom).Contains(texteRecherche, StringComparison.OrdinalIgnoreCase) OrElse
               CChamps(therapeute.Specialite).Contains(texteRecherche, StringComparison.OrdinalIgnoreCase)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CChamps
    ' Version  : V1.0.0
    ' Date     : 16/06/2026
    '
    ' Rôle     :
    ' Retourne une chaîne sûre (jamais Nothing) pour la recherche.
    '
    ' Paramètres :
    ' - valeur : Chaîne à sécuriser
    '
    ' Retour   :
    ' - String : Chaîne sécurisée (jamais Nothing)
    ' -------------------------------------------------------------------------------------------------
    Private Function CChamps(valeur As String) As String
        Return If(valeur, String.Empty)
    End Function

#End Region

#Region "Gestion état UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourEtatBoutons
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Active ou désactive les boutons selon la sélection et adapte le texte d'Activer/Désactiver.
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourEtatBoutons()

        Dim hasSelection As Boolean = dgvTherapeutes.CurrentRow IsNot Nothing

        btnModifier.Enabled = hasSelection
        btnActiverDesactiver.Enabled = hasSelection
        btnSupprimer.Enabled = hasSelection

        If hasSelection Then

            Dim therapeute As Therapeute = GetTherapeuteSelectionne()

            If therapeute IsNot Nothing Then
                btnActiverDesactiver.Text = If(therapeute.Actif, "Désactiver", "Activer")
            End If

        Else
            btnActiverDesactiver.Text = "Activer/Désactiver"
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : GetTherapeuteSelectionne
    ' Version  : V1.0.0
    ' Date     : 16/06/2026
    '
    ' Rôle     :
    ' Retourne le thérapeute actuellement sélectionné dans le DataGridView, ou Nothing.
    ' -------------------------------------------------------------------------------------------------
    Private Function GetTherapeuteSelectionne() As Therapeute

        If dgvTherapeutes.CurrentRow Is Nothing Then
            Return Nothing
        End If

        Return TryCast(dgvTherapeutes.CurrentRow.DataBoundItem, Therapeute)

    End Function

#End Region

#Region "Actions CRUD"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirEdition
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Ouvre la Form modale TherapeuteEdition en création (therapeute Nothing) ou modification.
    '
    ' Paramètres :
    ' - therapeute : Thérapeute à modifier, ou Nothing pour une création
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirEdition(therapeute As Therapeute)

        Using frm As New TherapeuteEdition(therapeute)

            frm.SetContext(_context)

            If frm.ShowDialog() = DialogResult.OK Then
                ChargerTherapeutes()
                dgvTherapeutes.ClearSelection()
            End If

        End Using

        If _context IsNot Nothing Then
            _context.SetHeader("Référentiels > Thérapeutes")
            _context.SetStatus("Gestion des thérapeutes.")
        End If

    End Sub

#End Region

#Region "Événements UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvTherapeutes_SelectionChanged
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Met à jour l'état des boutons lorsque la sélection change.

    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvTherapeutes_SelectionChanged(sender As Object, e As EventArgs) Handles dgvTherapeutes.SelectionChanged
        MettreAJourEtatBoutons()
    End Sub

    Private Sub dgvTherapeutes_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvTherapeutes.CellFormatting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colTelephone.Index Then
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
    ' Procédure : dgvTherapeutes_CellDoubleClick
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Ouvre la modification du thérapeute sur double-clic d'une ligne.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvTherapeutes_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTherapeutes.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        Dim therapeute As Therapeute = GetTherapeuteSelectionne()

        If therapeute IsNot Nothing Then
            OuvrirEdition(therapeute)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnRechercher_Click
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Applique le filtre de recherche.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRechercher_Click(sender As Object, e As EventArgs) Handles btnRechercher.Click
        AppliquerFiltres()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtRecherche_KeyDown
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Déclenche la recherche sur la touche Entrée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRecherche_KeyDown(sender As Object, e As KeyEventArgs) Handles txtRecherche.KeyDown

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            AppliquerFiltres()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnReinitialiserFiltres_Click
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      : Réinitialise le filtre de recherche et l'affichage des inactifs.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReinitialiserFiltres_Click(sender As Object, e As EventArgs) Handles btnReinitialiserFiltres.Click

        txtRecherche.Clear()
        chkAfficherInactifs.Checked = False
        AppliquerFiltres()

        If _context IsNot Nothing Then
            _context.SetStatus("Filtre réinitialisé.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : chkAfficherInactifs_CheckedChanged
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Réapplique le filtre pour afficher ou masquer les thérapeutes inactifs.
    ' -------------------------------------------------------------------------------------------------
    Private Sub chkAfficherInactifs_CheckedChanged(sender As Object, e As EventArgs) Handles chkAfficherInactifs.CheckedChanged
        AppliquerFiltres()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActualiser_Click
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Recharge la liste des thérapeutes.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActualiser_Click(sender As Object, e As EventArgs) Handles btnActualiser.Click
        ChargerTherapeutes()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnNouveau_Click
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Ouvre la Form TherapeuteEdition en mode création.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNouveau_Click(sender As Object, e As EventArgs) Handles btnNouveau.Click
        OuvrirEdition(Nothing)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Ouvre la Form TherapeuteEdition en mode modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click

        Dim therapeute As Therapeute = GetTherapeuteSelectionne()

        If therapeute Is Nothing Then
            If _context IsNot Nothing Then
                _context.SetStatus("Aucun thérapeute sélectionné.")
            End If
            Return
        End If

        OuvrirEdition(therapeute)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActiverDesactiver_Click
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Active ou désactive (soft-delete) le thérapeute sélectionné, après confirmation.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActiverDesactiver_Click(sender As Object, e As EventArgs) Handles btnActiverDesactiver.Click

        Dim therapeute As Therapeute = GetTherapeuteSelectionne()

        If therapeute Is Nothing Then
            If _context IsNot Nothing Then
                _context.SetStatus("Aucun thérapeute sélectionné.")
            End If
            Return
        End If

        Try

            Dim nouvelEtat As Boolean = Not therapeute.Actif
            Dim action As String = If(nouvelEtat, "activer", "désactiver")

            Dim result As DialogResult = DialogChoix.Confirmer(
                $"Voulez-vous vraiment {action} le thérapeute '{therapeute.NomComplet}' ?",
                "Confirmation"
            )

            If result <> DialogResult.Yes Then
                Return
            End If

            If nouvelEtat Then
                therapeute.Actif = True
                GestionTherapeutes.UpdateTherapeute(therapeute)
            Else
                GestionTherapeutes.DesactiverTherapeute(therapeute.IdTherapeute)
            End If

            ChargerTherapeutes()

            If _context IsNot Nothing Then
                _context.SetStatus($"Thérapeute {action}.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur activation/désactivation thérapeute (id={therapeute.IdTherapeute}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "Impossible de modifier l'état du thérapeute.",
                "Erreur"
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnSupprimer_Click
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Supprime physiquement le thérapeute s'il n'est pas référencé, sinon propose
    '             la désactivation (soft-delete).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSupprimer_Click(sender As Object, e As EventArgs) Handles btnSupprimer.Click

        Dim therapeute As Therapeute = GetTherapeuteSelectionne()

        If therapeute Is Nothing Then
            If _context IsNot Nothing Then
                _context.SetStatus("Aucun thérapeute sélectionné.")
            End If
            Return
        End If

        Try

            ' Un thérapeute référencé (dossiers, autres_suivis_patient) ne peut pas être supprimé physiquement.
            If GestionTherapeutes.TherapeuteEstUtilise(therapeute.IdTherapeute) Then

                Dim resultUsage As DialogResult = DialogChoix.Confirmer(
                    $"Le thérapeute '{therapeute.NomComplet}' est référencé dans des dossiers ou des suivis." & vbCrLf &
                    "Il ne peut pas être supprimé définitivement." & vbCrLf & vbCrLf &
                    "Voulez-vous le désactiver à la place ?",
                    "Suppression impossible"
                )

                If resultUsage = DialogResult.Yes Then

                    GestionTherapeutes.DesactiverTherapeute(therapeute.IdTherapeute)
                    ChargerTherapeutes()

                    If _context IsNot Nothing Then
                        _context.SetStatus("Thérapeute désactivé.")
                    End If

                End If

                Return

            End If

            ' Suppression physique d'un thérapeute non référencé, après confirmation.
            Dim result As DialogResult = DialogChoix.Confirmer(
                $"Voulez-vous vraiment supprimer définitivement le thérapeute '{therapeute.NomComplet}' ?",
                "Confirmation de suppression"
            )

            If result <> DialogResult.Yes Then
                Return
            End If

            GestionTherapeutes.SupprimerTherapeute(therapeute.IdTherapeute)
            ChargerTherapeutes()

            If _context IsNot Nothing Then
                _context.SetStatus("Thérapeute supprimé.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur suppression thérapeute (id={therapeute.IdTherapeute}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "Impossible de supprimer le thérapeute.",
                "Erreur"
            )

        End Try

    End Sub

#End Region

End Class
