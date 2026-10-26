' -------------------------------------------------------------------------------------------------
' UserControl : UC_Utilisateurs
' Projet      : Althéa
' Version     : V1.9.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' UserControl d'administration permettant de gérer les utilisateurs applicatifs.
' Chargé dynamiquement dans Home via NavigationManager.
'
' Responsabilités :
' - Afficher la liste des utilisateurs
' - Permettre la recherche et le filtrage
' - Ouvrir la Form UtilisateurEdition en création/modification
' - Permettre les actions administratives autorisées
' - Utiliser le contexte UI partagé fourni par Home
' - Journaliser les actions via GestionLog
' - Gérer l'affichage et l'état des boutons selon les droits utilisateur
'
' Remarques   :
' - Implémente IContextAwareUserControl
' - Aucun accès direct à la base de données
' - Toute logique métier passe par GestionUtilisateurs
' - Les données sensibles password_hash/password_salt ne sont jamais chargées ici
' - V1.6.0 (18/10/2026, UI-9.3, maquette « Grilles — états lisibles ») : état du compte en pastille
'   (Actif, Verrouillé, Désactivé — UtilisateurApplication.EtatCompte) à la place des icônes PNG,
'   avec une infobulle sur « Verrouillé » ; rôle Admin et SuperUser en pastille mauve (User, le plus
'   courant, en texte) ; compte désactivé atténué ; tri par en-tête ; dernière connexion centrée.
'
' Dépendances :
' - GestionUtilisateurs
' - UserControlContext
' - UtilsButtons
' - UtilsDataGrid
' - GestionLog
' - UITheme
' - UtilsPastilles
' - UtilisateurEdition (Form de création/modification)
' - AppRole (pour gestion des droits)
' - PasswordSecurityHelper
'
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' - V1.7.0 (20/10/2026) : filtre « Dernière connexion depuis » : champ de date du kit (AltheaDate) ;
'   retient cette date et les suivantes (audit B7 : seule la date exacte était retenue).
' - V1.8.0 (22/10/2026, audit C.1) : les opérations sensibles reçoivent le ContexteOperation de l'écran (utilisateur + rôle
'   courant), vérifié par le métier (Autorisation). Boutons Nouveau / Modifier selon le rôle COURANT
'   (élévation comprise) : un SuperUser élevé en Admin n'avait que la consultation (test 20.2).
' - V1.9.0 (24/10/2026) : colonne Code (USR…) visible, en tête de grille (code toujours visible,
'   Rules § 15).
' - V1.9.1 (25/10/2026, audit C.4, ADR-55) : la fenêtre principale est lue dans le contexte
'   (_context.FenetrePrincipale, IFenetrePrincipale) ; plus aucune référence à Home.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Utilisateurs
    Implements IContextAwareUserControl

#Region "Variables privées"

    'Injecté par Home via SetContext() (implémentation IContextAwareUserControl)
    Private _context As UserControlContext

    ' Liste complète des utilisateurs chargée depuis la base de données. Utilisée pour les opérations de filtrage en mémoire.
    Private _utilisateurs As List(Of UtilisateurApplication)

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 21/05/2026
    '
    ' Rôle      :
    ' Reçoit le contexte UI partagé injecté par Home.
    '
    ' Paramètres :
    ' - context : Contexte UI partagé de l'application.
    '
    ' Remarques :
    ' - Obligatoire pour accéder au Status, ToolTip, ErrorProvider et contexte Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) _
        Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

#End Region

#Region "Chargement UserControl"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_Utilisateurs_Load
    ' Version   : V1.1.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Initialise le UserControl et charge la liste des utilisateurs.
    ' - Configure les éléments UI (boutons, DataGridView, ToolTips, filtres).
    ' - Applique les droits d'accès selon le rôle de l'utilisateur courant.
    ' - Charge les utilisateurs depuis la base de données via GestionUtilisateurs.
    ' - Affiche le nombre d'utilisateurs chargés dans le contexte.

    ' Remarques :
    ' - Aucune logique métier ni accès direct à la base de données (tout passe par les couches inférieures).
    ' - Les données sensibles password_hash/password_salt ne sont jamais chargées ici.
    ' - Utilise GestionUtilisateurs pour charger la liste des utilisateurs depuis la base de données.

    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_Utilisateurs_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        If _context IsNot Nothing Then
            _context.SetHeader("Administration > Utilisateurs")
            _context.SetStatus("Chargement de la liste des utilisateurs...")
        End If

        InitialiserBoutons()
        InitialiserDataGridView()
        InitialiserToolTips()
        InitialiserComboRole()
        InitialiserDatePicker()
        AppliquerDroitsUtilisateur()
        ChargerUtilisateurs()

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
        UtilsButtons.InitStandardButton(btnActualiser)
        UtilsButtons.InitStandardButton(btnRechercher)
        UtilsButtons.InitStandardButton(btnReinitialiserFiltres)

        ' Le bouton Réinitialiser est désactivé par défaut (aucun filtre au chargement)
        btnReinitialiserFiltres.Enabled = False

    End Sub


    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserDataGridView
    ' Version   : V1.0.0
    ' Date      : 21/05/2026
    '
    ' Rôle      :
    ' Configure le DataGridView des utilisateurs.
    ' - Définit les propriétés de base (lecture seule, sélection, etc.).
    ' - Lie les colonnes aux propriétés de UtilisateurApplication.
    ' - Configure le format d'affichage de la date du dernier login.

    ' Remarques
    ' - Aucune logique métier ni accès direct à la base de données (tout passe par les couches inférieures).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserDataGridView()

        UtilsDataGrid.InitDataGridBasique(dgvUtilisateurs)

        dgvUtilisateurs.AutoGenerateColumns = False

        dgvUtilisateurs.ReadOnly = True
        dgvUtilisateurs.MultiSelect = False
        dgvUtilisateurs.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvUtilisateurs.AllowUserToAddRows = False
        dgvUtilisateurs.AllowUserToDeleteRows = False
        dgvUtilisateurs.AutoGenerateColumns = False

        colIdUtilisateur.DataPropertyName = NameOf(UtilisateurApplication.IdUtilisateur)
        colCodeUtilisateur.DataPropertyName = NameOf(UtilisateurApplication.CodeUtilisateur)
        colLoginUtilisateur.DataPropertyName = NameOf(UtilisateurApplication.LoginUtilisateur)
        colNomAffichage.DataPropertyName = NameOf(UtilisateurApplication.NomAffichage)
        colRoleUtilisateur.DataPropertyName = NameOf(UtilisateurApplication.RoleUtilisateur)
        colRoleMaxElevation.DataPropertyName = NameOf(UtilisateurApplication.RoleMaxElevation)
        colActif.DataPropertyName = NameOf(UtilisateurApplication.Actif)
        colMustChangePassword.DataPropertyName = NameOf(UtilisateurApplication.MustChangePassword)
        colNbEchecsLogin.DataPropertyName = NameOf(UtilisateurApplication.NbEchecsLogin)
        colCompteVerrouille.DataPropertyName = NameOf(UtilisateurApplication.CompteVerrouille)
        colDateVerrouillage.DataPropertyName = NameOf(UtilisateurApplication.DateVerrouillage)
        colDernierLogin.DataPropertyName = NameOf(UtilisateurApplication.DernierLogin)
        colDateCreation.DataPropertyName = NameOf(UtilisateurApplication.DateCreation)
        colDateModification.DataPropertyName = NameOf(UtilisateurApplication.DateModification)

        colEtat.DataPropertyName = NameOf(UtilisateurApplication.LibelleEtatCompte)

        colDernierLogin.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"
        UtilsDataGrid.AlignerCentre(colDernierLogin)

        ' Après les DataPropertyName : une colonne liée devient triable par son en-tête.
        UtilsDataGrid.ActiverTriColonnes(dgvUtilisateurs)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Version   : V1.0.0
    ' Date      : 21/05/2026
    '
    ' Rôle      :
    ' Initialise les aides contextuelles du UserControl.

    ' Remarques :
    ' - Utilise le ToolTip du contexte pour fournir des descriptions d'actions sur les boutons et éléments interactifs.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        If _context Is Nothing Then Exit Sub

        _context.SetToolTip(btnNouveau, "Créer un nouvel utilisateur.")
        _context.SetToolTip(btnModifier, "Modifier l'utilisateur sélectionné.")
        _context.SetToolTip(btnActiverDesactiver, "Activer ou désactiver l'utilisateur sélectionné.")
        _context.SetToolTip(btnActualiser, "Recharger la liste des utilisateurs.")
        _context.SetToolTip(chkAfficherInactifs, "Afficher également les utilisateurs désactivés.")
        _context.SetToolTip(chkCompteVerrouille, "Afficher uniquement les comptes verrouillés.")
        _context.SetToolTip(chkFiltrerDate, "Activer le filtre par date de dernier login.")
        _context.SetToolTip(btnRechercher, "Appliquer les critères de recherche.")
        _context.SetToolTip(btnReinitialiserFiltres, "Réinitialiser tous les filtres de recherche.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserComboRole
    ' Version   : V1.0.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Initialise le ComboBox de filtrage par rôle.
    '
    ' Remarques :
    ' - Ajoute "Tous" comme option par défaut.
    ' - Ajoute les rôles User, SuperUser, Admin.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserComboRole()

        cboFiltreRole.Items.Clear()
        cboFiltreRole.Items.Add("Tous")
        cboFiltreRole.Items.Add("User")
        cboFiltreRole.Items.Add("SuperUser")
        cboFiltreRole.Items.Add("Admin")
        cboFiltreRole.SelectedIndex = 0 ' Sélectionner "Tous" par défaut

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserDatePicker
    ' Version   : V1.1.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Initialise le champ de date du filtre « Dernière connexion depuis ».
    '
    ' Remarques :
    ' - Champ du kit (AltheaDate) ; inactif tant que chkFiltrerDate n'est pas coché.
    ' - V1.1.0 (20/10/2026) : AltheaDate à la place du DateTimePicker Windows.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserDatePicker()

        dtpDernierLoginDepuis.Valeur = Date.Today.AddMonths(-1) ' Par défaut : 1 mois
        dtpDernierLoginDepuis.Enabled = False ' Désactivé par défaut

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltres
    ' Version   : V1.1.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Applique les critères de recherche et de filtrage sur la liste des utilisateurs.
    '
    ' Critères :
    ' - Texte de recherche (login ou nom affiché)
    ' - Rôle sélectionné
    ' - Date de dernier login (si activée)
    ' - Comptes verrouillés uniquement (si coché)
    ' - Affichage des utilisateurs inactifs (si coché)
    '
    ' Remarques :
    ' - Filtre la liste _utilisateurs en mémoire.
    ' - Lie le résultat au DataGridView.
    ' - V1.1.0 (20/10/2026) : « Dernière connexion depuis » retient cette date et les suivantes (audit B7).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltres()

        If _utilisateurs Is Nothing Then
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            ' Commencer avec la liste complète
            Dim utilisateursFiltres = _utilisateurs.AsEnumerable()

            ' Filtre : Texte de recherche (login ou nom affiché)
            Dim texteRecherche As String = txtRechercheUtilisateur.Text.Trim()
            If Not String.IsNullOrEmpty(texteRecherche) Then
                utilisateursFiltres = utilisateursFiltres.Where(
                    Function(u) u.LoginUtilisateur.Contains(texteRecherche, StringComparison.OrdinalIgnoreCase) OrElse
                                u.NomAffichage.Contains(texteRecherche, StringComparison.OrdinalIgnoreCase)
                )
            End If

            ' Filtre : Rôle
            Dim roleSelectionne As String = If(cboFiltreRole.SelectedItem?.ToString(), "Tous")
            If roleSelectionne <> "Tous" Then
                utilisateursFiltres = utilisateursFiltres.Where(
                    Function(u) u.RoleUtilisateur.ToString() = roleSelectionne
                )
            End If

            ' Filtre : Date de dernier login
            ' « Depuis » : la date choisie et après (audit B7 : seule la date exacte était retenue).
            If chkFiltrerDate.Checked AndAlso dtpDernierLoginDepuis.Valeur.HasValue Then
                Dim dateFiltre As Date = dtpDernierLoginDepuis.Valeur.Value.Date
                utilisateursFiltres = utilisateursFiltres.Where(
                    Function(u) u.DernierLogin.HasValue AndAlso u.DernierLogin.Value.Date >= dateFiltre)
            End If

            ' Filtre : Comptes verrouillés uniquement
            If chkCompteVerrouille.Checked Then
                utilisateursFiltres = utilisateursFiltres.Where(
                    Function(u) u.CompteVerrouille = True
                )
            End If

            ' Filtre : Utilisateurs inactifs
            If Not chkAfficherInactifs.Checked Then
                utilisateursFiltres = utilisateursFiltres.Where(
                    Function(u) u.Actif = True
                )
            End If

            ' Lier le résultat au DataGridView
            Dim resultats = utilisateursFiltres.ToList()
            ' Nouvelle liste dans l'ordre du chargement : le tri par en-tête précédent ne vaut plus.
            UtilsDataGrid.ReinitialiserTri(dgvUtilisateurs)
            dgvUtilisateurs.DataSource = Nothing
            dgvUtilisateurs.DataSource = resultats

            ' Désélectionner toutes les lignes
            dgvUtilisateurs.ClearSelection()

            ' Afficher le nombre de résultats dans le contexte
            If _context IsNot Nothing Then
                _context.SetStatus($"{resultats.Count} utilisateur(s) affiché(s).")
            End If

            ' Log
            GestionLog.EcrireLog(
                $"Filtres appliqués : {resultats.Count} résultat(s).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI
            )

        Catch ex As Exception
            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors de l'application des filtres.")
            End If

            GestionLog.EcrireLog(
                "Erreur lors de l'application des filtres.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        Finally
            Cursor = Cursors.Default
        End Try

        ' Vérifier l'état des filtres pour activer/désactiver le bouton Réinitialiser
        VerifierEtatFiltres()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : VerifierEtatFiltres
    ' Version   : V1.0.0
    ' Date      : 02/06/2026
    '
    ' Rôle      :
    ' Vérifie si des filtres sont actifs et active/désactive le bouton Réinitialiser.
    '
    ' Remarques :
    ' - Le bouton est actif si au moins un filtre est appliqué.
    ' -------------------------------------------------------------------------------------------------
    Private Sub VerifierEtatFiltres()

        Dim filtresActifs As Boolean = False

        ' Vérifier si un texte de recherche est saisi
        If Not String.IsNullOrWhiteSpace(txtRechercheUtilisateur.Text) Then
            filtresActifs = True
        End If

        ' Vérifier si un rôle spécifique est sélectionné
        If cboFiltreRole.SelectedItem IsNot Nothing AndAlso
           cboFiltreRole.SelectedItem.ToString() <> "Tous" Then
            filtresActifs = True
        End If

        ' Vérifier si le filtre de date est activé
        If chkFiltrerDate.Checked Then
            filtresActifs = True
        End If

        ' Vérifier si le filtre comptes verrouillés est activé
        If chkCompteVerrouille.Checked Then
            filtresActifs = True
        End If

        ' Vérifier si le filtre afficher inactifs est activé
        If chkAfficherInactifs.Checked Then
            filtresActifs = True
        End If

        ' Activer/désactiver le bouton en fonction de l'état des filtres
        btnReinitialiserFiltres.Enabled = filtresActifs

    End Sub

#End Region

#Region "Chargement données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerUtilisateurs
    ' Version   : V1.1.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Charge la liste complète des utilisateurs depuis la base de données.
    '
    ' Remarques :
    ' - Utilise GestionUtilisateurs.
    ' - Aucun accès DB direct depuis l'UI.
    ' - Après chargement, appelle AppliquerFiltres() pour afficher selon les critères.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerUtilisateurs()

        Try
            Cursor = Cursors.WaitCursor

            ' Charger tous les utilisateurs (actifs et inactifs)
            _utilisateurs =
                GestionUtilisateurs.GetUtilisateursPourListe(
                    afficherInactifs:=True
                )

            ' Appliquer les filtres de recherche
            AppliquerFiltres()

            MettreAJourEtatBoutons()

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur chargement utilisateurs.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des utilisateurs.")
            End If

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

#End Region

#Region "Gestion état UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerDroitsUtilisateur
    ' Version   : V2.0.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Adapte la visibilité des boutons selon le rôle COURANT de la session (élévation comprise).
    '
    ' Remarques :
    ' - Admin (ou élevé en Admin) : accès complet (création, modification).
    ' - Sinon : consultation (actions de maintenance dans UtilisateurEdition).
    ' - V2.0.0 (22/10/2026, audit C.1, test 20.2) : lisait le rôle DE BASE du compte ; un SuperUser
    '   élevé en Admin n'avait donc que la consultation, contrairement à la décision du 20/10/2026
    '   (« le rôle courant fait foi partout »). Le métier vérifie de toute façon (Autorisation).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerDroitsUtilisateur()

        Dim estAdmin As Boolean = Autorisation.Possede(_context?.ContexteOperation, AppRole.Admin)

        btnNouveau.Visible = estAdmin
        btnModifier.Visible = estAdmin

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourEtatBoutons
    ' Version   : V1.1.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Active ou désactive les boutons selon la sélection courante.
    ' Met à jour le texte du bouton Activer/Désactiver selon l'état de l'utilisateur sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourEtatBoutons()

        Dim hasSelection As Boolean =
            dgvUtilisateurs.CurrentRow IsNot Nothing

        btnModifier.Enabled = hasSelection
        btnActiverDesactiver.Enabled = hasSelection

        ' Mettre à jour le texte du bouton selon l'état de l'utilisateur sélectionné
        If hasSelection Then
            Dim utilisateur As UtilisateurApplication = GetUtilisateurSelectionne()
            If utilisateur IsNot Nothing Then
                If utilisateur.Actif Then
                    btnActiverDesactiver.Text = "Désactiver"
                Else
                    btnActiverDesactiver.Text = "Activer"
                End If
            End If
        Else
            btnActiverDesactiver.Text = "Activer/Désactiver"
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : GetUtilisateurSelectionne
    ' Type      : UtilisateurApplication
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Retourne l'utilisateur actuellement sélectionné dans le DataGridView.
    '
    ' Retour    :
    ' - UtilisateurApplication : utilisateur sélectionné, ou Nothing si aucune sélection valide.
    '
    ' Remarques :
    ' - Ne déclenche aucune action métier.
    ' - Utilisée uniquement pour les actions UI nécessitant une ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Function GetUtilisateurSelectionne() As UtilisateurApplication

        If dgvUtilisateurs.CurrentRow Is Nothing Then
            Return Nothing
        End If

        Return TryCast(
            dgvUtilisateurs.CurrentRow.DataBoundItem,
            UtilisateurApplication
        )

    End Function

#End Region

#Region "Événements UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvUtilisateurs_SelectionChanged
    ' Version   : V1.0.0
    ' Date      : 21/05/2026
    '
    ' Rôle      :
    ' Met à jour l'état des boutons lorsque la sélection change.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvUtilisateurs_SelectionChanged(
        sender As Object,
        e As EventArgs
    ) Handles dgvUtilisateurs.SelectionChanged

        MettreAJourEtatBoutons()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvUtilisateurs_CellFormatting
    ' Version   : V2.0.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Atténue la ligne d'un compte désactivé (comme les lignes terminées des autres grilles).
    '
    ' Remarques :
    ' - V2.0.0 (UI-9.3) : l'état n'est plus une icône posée ici ; il est dessiné en pastille
    '   (dgvUtilisateurs_CellPainting) d'après UtilisateurApplication.EtatCompte.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvUtilisateurs_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvUtilisateurs.CellFormatting

        If e.RowIndex < 0 Then Return

        Dim utilisateur As UtilisateurApplication =
            TryCast(dgvUtilisateurs.Rows(e.RowIndex).DataBoundItem, UtilisateurApplication)

        If utilisateur IsNot Nothing AndAlso Not utilisateur.Actif Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvUtilisateurs_CellPainting
    ' Version   : V1.0.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Pastilles de la liste : état du compte (toujours) et rôle (Admin, SuperUser ; le rôle User, le
    ' plus courant, reste en texte : seules les exceptions se voient).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvUtilisateurs_CellPainting(
        sender As Object,
        e As DataGridViewCellPaintingEventArgs
    ) Handles dgvUtilisateurs.CellPainting

        If e.RowIndex < 0 Then Return
        If e.ColumnIndex <> colEtat.Index AndAlso e.ColumnIndex <> colRoleUtilisateur.Index Then Return

        Dim utilisateur As UtilisateurApplication =
            TryCast(dgvUtilisateurs.Rows(e.RowIndex).DataBoundItem, UtilisateurApplication)
        If utilisateur Is Nothing Then Return

        If e.ColumnIndex = colEtat.Index Then
            UtilsDataGrid.DessinerPastilleCellule(dgvUtilisateurs, e, utilisateur.LibelleEtatCompte,
                                                  UtilsPastilles.Teinte(utilisateur.EtatCompte))
        ElseIf utilisateur.RoleUtilisateur <> AppRole.User Then
            UtilsDataGrid.DessinerPastilleCellule(dgvUtilisateurs, e, utilisateur.RoleUtilisateur.ToString(),
                                                  UtilsPastilles.Teinte(TonEtat.Role),
                                                  attenuee:=Not utilisateur.Actif)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvUtilisateurs_CellToolTipTextNeeded
    ' Version   : V1.0.0
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Sur la pastille « Verrouillé » : pourquoi, et où le lever (bouton Déverrouiller de la fiche).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvUtilisateurs_CellToolTipTextNeeded(
        sender As Object,
        e As DataGridViewCellToolTipTextNeededEventArgs
    ) Handles dgvUtilisateurs.CellToolTipTextNeeded

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colEtat.Index Then Return

        Dim utilisateur As UtilisateurApplication =
            TryCast(dgvUtilisateurs.Rows(e.RowIndex).DataBoundItem, UtilisateurApplication)
        If utilisateur Is Nothing OrElse Not utilisateur.CompteVerrouille Then Return

        e.ToolTipText = $"{utilisateur.NbEchecsLogin} échec(s) de connexion — déverrouiller depuis la fiche (Modifier)."

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnReinitialiserFiltres_Click
    ' Version   : V1.0.0
    ' Date      : 02/06/2026
    '
    ' Rôle      :
    ' Réinitialise tous les filtres de recherche et recharge la liste complète.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReinitialiserFiltres_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnReinitialiserFiltres.Click

        ' Vider le texte de recherche
        txtRechercheUtilisateur.Clear()

        ' Réinitialiser le combo rôle à "Tous"
        cboFiltreRole.SelectedIndex = 0

        ' Décocher les filtres
        chkFiltrerDate.Checked = False
        chkCompteVerrouille.Checked = False
        chkAfficherInactifs.Checked = False

        ' Réappliquer les filtres (qui sont maintenant tous désactivés)
        AppliquerFiltres()

        If _context IsNot Nothing Then
            _context.SetStatus("Filtres réinitialisés.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : chkAfficherInactifs_CheckedChanged
    ' Version   : V1.1.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Réapplique les filtres pour afficher ou masquer les utilisateurs inactifs.
    ' -------------------------------------------------------------------------------------------------
    Private Sub chkAfficherInactifs_CheckedChanged(
        sender As Object,
        e As EventArgs
    ) Handles chkAfficherInactifs.CheckedChanged

        AppliquerFiltres()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnRechercher_Click
    ' Version   : V1.0.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Déclenche l'application des filtres de recherche.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRechercher_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRechercher.Click

        AppliquerFiltres()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : chkFiltrerDate_CheckedChanged
    ' Version   : V1.0.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Active ou désactive le champ de date du filtre.
    ' -------------------------------------------------------------------------------------------------
    Private Sub chkFiltrerDate_CheckedChanged(
        sender As Object,
        e As EventArgs
    ) Handles chkFiltrerDate.CheckedChanged

        dtpDernierLoginDepuis.Enabled = chkFiltrerDate.Checked

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActualiser_Click
    ' Version   : V1.0.0
    ' Date      : 21/05/2026
    '
    ' Rôle      :
    ' Recharge manuellement la liste des utilisateurs.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActualiser_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnActualiser.Click

        ChargerUtilisateurs()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnNouveau_Click
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Ouvre la Form UtilisateurEdition en mode création.
    '
    ' Remarques :
    ' - La Form est ouverte en modal.
    ' - Si la création est validée, la liste des utilisateurs est rechargée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNouveau_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnNouveau.Click

        Using frm As New UtilisateurEdition(
            ModeUtilisateurEdition.Creation
        )

            If _context IsNot Nothing Then
                frm.SetContext(_context)
            End If

            If frm.ShowDialog() = DialogResult.OK Then
                ChargerUtilisateurs()
                ' Désélectionner après création pour éviter actions involontaires
                dgvUtilisateurs.ClearSelection()
            End If

        End Using

        If _context IsNot Nothing Then
            _context.SetHeader("Administration > Utilisateurs")
            _context.SetStatus("Gestion des utilisateurs.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre la Form UtilisateurEdition en mode modification pour l'utilisateur sélectionné.
    '
    ' Remarques :
    ' - La Form est ouverte en modal.
    ' - Si la modification est validée, la liste des utilisateurs est rechargée.
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifier.Click

        Dim utilisateur As UtilisateurApplication =
            GetUtilisateurSelectionne()

        If utilisateur Is Nothing Then
            If _context IsNot Nothing Then
                _context.SetStatus("Aucun utilisateur sélectionné.")
            End If

            Return
        End If

        Using frm As New UtilisateurEdition(
            ModeUtilisateurEdition.Modification,
            utilisateur.IdUtilisateur
        )

            If _context IsNot Nothing Then
                frm.SetContext(_context)
            End If

            If frm.ShowDialog() = DialogResult.OK Then
                ChargerUtilisateurs()
            End If

        End Using

        ' La photo s'enregistre dès son import (sans « Enregistrer ») : si c'est celle du compte
        ' connecté, le bandeau de la Home doit la refléter.
        _context?.FenetrePrincipale?.RafraichirPhotoUtilisateur()

        If _context IsNot Nothing Then
            _context.SetHeader("Administration > Utilisateurs")
            _context.SetStatus("Gestion des utilisateurs.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActiverDesactiver_Click
    ' Version   : V1.0.0
    ' Date      : 17/05/2026
    '
    ' Rôle      :
    ' Active ou désactive l'utilisateur sélectionné.
    '
    ' Remarques :
    ' - Demande confirmation avant de procéder
    ' - Recharge la liste après l'action
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActiverDesactiver_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnActiverDesactiver.Click

        Dim utilisateur As UtilisateurApplication = GetUtilisateurSelectionne()

        If utilisateur Is Nothing Then
            If _context IsNot Nothing Then
                _context.SetStatus("Aucun utilisateur sélectionné.")
            End If
            Return
        End If

        Try

            ' Déterminer l'action à effectuer
            Dim nouvelEtat As Boolean = Not utilisateur.Actif
            Dim action As String = If(nouvelEtat, "activer", "désactiver")

            ' Confirmation de l'action
            Dim result As DialogResult = DialogChoix.Confirmer(
                $"Voulez-vous vraiment {action} l'utilisateur '{utilisateur.NomAffichage}' ?",
                "Confirmation"
            )

            If result <> DialogResult.Yes Then
                Return
            End If

            ' Appel de la méthode métier
            Dim succes As Boolean = GestionUtilisateurs.ActiverDesactiverUtilisateur(
                utilisateur.IdUtilisateur,
                nouvelEtat,
                _context.ContexteOperation
            )

            If succes Then

                DialogChoix.Succes(
                    $"L'utilisateur '{utilisateur.NomAffichage}' a été {action} avec succès.",
                    "Succès"
                )

                ' Recharger la liste
                ChargerUtilisateurs()

                If _context IsNot Nothing Then
                    _context.SetStatus($"Utilisateur {action}.")
                End If

            Else

                DialogChoix.Erreur(
                    $"Erreur lors de l'action {action}. Consultez les logs.",
                    "Erreur"
                )

            End If

        Catch ex As UnauthorizedAccessException

            DialogChoix.Avertissement(
                ex.Message,
                "Accès refusé"
            )

        Catch ex As InvalidOperationException

            DialogChoix.Avertissement(
                ex.Message,
                "Opération invalide"
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur btnActiverDesactiver_Click.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            DialogChoix.Erreur(
                "Une erreur inattendue s'est produite. Consultez les logs.",
                "Erreur"
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvUtilisateurs_CellDoubleClick
    ' Version   : V1.1.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Ouvre la fiche de l'utilisateur sélectionné lors d'un double-clic sur une ligne.
    '
    ' Remarques :
    ' - Admin : ouvre en mode Modification (via btnModifier)
    ' - SuperUser : ouvre en mode Consultation avec actions admin (déverrouillage, reset password)
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvUtilisateurs_CellDoubleClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles dgvUtilisateurs.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        ' Si btnModifier est visible (Admin), l'utiliser
        If btnModifier.Visible Then
            btnModifier.PerformClick()
        Else
            ' Sinon (SuperUser), ouvrir en mode Consultation
            OuvrirFicheConsultation()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirFicheConsultation
    ' Version   : V1.0.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Ouvre la fiche utilisateur en mode Consultation pour les SuperUser.
    '
    ' Remarques :
    ' - Permet uniquement les actions admin (déverrouillage, reset password)
    ' - Pas de modification des champs utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirFicheConsultation()

        Dim utilisateur As UtilisateurApplication = GetUtilisateurSelectionne()

        If utilisateur Is Nothing Then
            If _context IsNot Nothing Then
                _context.SetStatus("Aucun utilisateur sélectionné.")
            End If
            Return
        End If

        Using frm As New UtilisateurEdition(
            ModeUtilisateurEdition.Consultation,
            utilisateur.IdUtilisateur
        )

            If _context IsNot Nothing Then
                frm.SetContext(_context)
            End If

            frm.ShowDialog()

        End Using

        If _context IsNot Nothing Then
            _context.SetHeader("Administration > Utilisateurs")
            _context.SetStatus("Gestion des utilisateurs.")
        End If

    End Sub


#End Region

End Class
