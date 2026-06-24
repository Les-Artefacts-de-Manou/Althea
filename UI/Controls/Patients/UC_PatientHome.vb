' -------------------------------------------------------------------------------------------------
' UserControl : UC_PatientHome
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' UserControl d'accueil des patients permettant de rechercher et de lister les patients.
' Chargé dynamiquement dans Home via NavigationManager.
'
' Responsabilités :
' - Afficher la liste des patients (triés par date de modification décroissante)
' - Permettre la recherche multi-critères en mémoire (nom, prénom, NISS, code, contact)
' - Filtrer les patients ayant au moins un dossier actif
' - Préparer l'ouverture de la fiche patient (création / modification / consultation)
' - Utiliser le contexte UI partagé fourni par Home
' - Journaliser les actions via GestionLog
'
' Remarques   :
' - Implémente IContextAwareUserControl
' - Aucun accès direct à la base de données
' - Toute logique métier passe par GestionPatients
' - Les actions Nouveau / Modifier / Ouvrir sont des points d'entrée vers UC_PatientFiche
'   (écran non encore implémenté : statut informatif en attendant)
'
' Dépendances :
' - GestionPatients
' - PatientListeItem
' - UserControlContext
' - UtilsButtons
' - UtilsDataGrid
' - GestionLog
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_PatientHome
    Implements IContextAwareUserControl

#Region "Variables privées"

    ' Index des items du filtre de suivi (cboFiltreSuivi). L'ordre doit correspondre à InitialiserFiltres().
    Private Const FiltreSuiviEnCours As Integer = 0
    Private Const FiltreSuiviCloture As Integer = 1
    Private Const FiltreSuiviTous As Integer = 2

    ' Injecté par Home via SetContext() (implémentation IContextAwareUserControl)
    Private _context As UserControlContext

    ' Liste complète des patients chargée depuis la base de données. Utilisée pour le filtrage en mémoire.
    Private _patients As List(Of PatientListeItem)

    ' Filtre de recherche à restaurer au retour depuis la fiche patient (mini-pile D-Q15).
    ' Renseigné par RestaurerFiltre() avant le Load ; appliqué aux contrôles avant le chargement.
    Private _filtreInitialTexte As String = Nothing
    Private _filtreInitialSuivi As Integer = FiltreSuiviEnCours

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
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
    ' Procédure : UC_PatientHome_Load
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Initialise le UserControl et charge la liste des patients.
    ' - Configure les éléments UI (boutons, DataGridView, ToolTips).
    ' - Charge les patients depuis la base de données via GestionPatients.
    '
    ' Remarques :
    ' - Aucune logique métier ni accès direct à la base de données (tout passe par les couches inférieures).
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_PatientHome_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        If _context IsNot Nothing Then
            _context.SetHeader("Patients")
            _context.SetStatus("Chargement de la liste des patients...")
        End If

        InitialiserBoutons()
        InitialiserDataGridView()
        InitialiserFiltres()
        InitialiserToolTips()
        AppliquerFiltreInitial()
        ChargerPatients()

    End Sub

#End Region

#Region "Initialisation UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutons
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Initialise les boutons standards du UserControl.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutons()

        UtilsButtons.InitStandardButton(btnNouveau)
        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnOuvrir)
        UtilsButtons.InitStandardButton(btnActualiser)
        UtilsButtons.InitStandardButton(btnRechercher)
        UtilsButtons.InitStandardButton(btnReinitialiserFiltres)

        ' Le bouton Réinitialiser est désactivé par défaut (aucun filtre au chargement)
        btnReinitialiserFiltres.Enabled = False

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserDataGridView
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Configure le DataGridView des patients.
    ' - Définit les propriétés de base (lecture seule, sélection, etc.).
    ' - Lie les colonnes aux propriétés de PatientListeItem.
    ' - Configure les formats d'affichage des dates.
    '
    ' Remarques :
    ' - Aucune logique métier ni accès direct à la base de données (tout passe par les couches inférieures).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserDataGridView()

        UtilsDataGrid.InitDataGridBasique(dgvPatients)

        dgvPatients.AutoGenerateColumns = False

        dgvPatients.ReadOnly = True
        dgvPatients.MultiSelect = False
        dgvPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPatients.AllowUserToAddRows = False
        dgvPatients.AllowUserToDeleteRows = False

        colCodePatient.DataPropertyName = NameOf(PatientListeItem.CodePatient)
        colNom.DataPropertyName = NameOf(PatientListeItem.Nom)
        colPrenom.DataPropertyName = NameOf(PatientListeItem.Prenom)
        colDateNaissance.DataPropertyName = NameOf(PatientListeItem.DateNaissance)
        colNiss.DataPropertyName = NameOf(PatientListeItem.Niss)
        colTelephone.DataPropertyName = NameOf(PatientListeItem.Telephone)
        colEmail.DataPropertyName = NameOf(PatientListeItem.Email)
        colAlerte.DataPropertyName = NameOf(PatientListeItem.AAlerte)
        colPhoto.DataPropertyName = NameOf(PatientListeItem.APhoto)
        colDateModification.DataPropertyName = NameOf(PatientListeItem.DateModification)
        colIdPatient.DataPropertyName = NameOf(PatientListeItem.IdPatient)

        ' Colonne icône de statut de suivi : non liée (alimentée dans CellFormatting),
        ' NullValue = Nothing pour éviter l'affichage du glyphe « image manquante ».
        colStatutSuivi.DefaultCellStyle.NullValue = Nothing
        colStatutSuivi.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        colDateNaissance.DefaultCellStyle.Format = "dd/MM/yyyy"
        colDateModification.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserFiltres
    ' Version   : V1.0.0
    ' Date      : 19/07/2026
    '
    ' Rôle      :
    ' Initialise le filtre de suivi (cboFiltreSuivi) avec ses trois états exclusifs.
    '
    ' Remarques :
    ' - L'ordre des items doit correspondre aux constantes FiltreSuivi* (index).
    ' - L'item par défaut est « Suivi en cours » (suivi_en_cours = 1).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFiltres()

        cboFiltreSuivi.Items.Clear()
        cboFiltreSuivi.Items.Add("Suivi en cours")
        cboFiltreSuivi.Items.Add("Suivi clôturé / archivé")
        cboFiltreSuivi.Items.Add("Tous")

        cboFiltreSuivi.SelectedIndex = FiltreSuiviEnCours

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Initialise les aides contextuelles du UserControl.
    '
    ' Remarques :
    ' - Utilise le ToolTip du contexte pour fournir des descriptions d'actions sur les boutons.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        If _context Is Nothing Then Exit Sub

        _context.SetToolTip(btnNouveau, "Créer un nouveau patient.")
        _context.SetToolTip(btnModifier, "Modifier le patient sélectionné.")
        _context.SetToolTip(btnOuvrir, "Ouvrir la fiche du patient sélectionné.")
        _context.SetToolTip(btnActualiser, "Recharger la liste des patients.")
        _context.SetToolTip(btnRechercher, "Appliquer les critères de recherche.")
        _context.SetToolTip(btnReinitialiserFiltres, "Réinitialiser tous les filtres de recherche.")
        _context.SetToolTip(cboFiltreSuivi, "Filtrer les patients selon le statut de suivi (en cours, clôturé/archivé ou tous).")

    End Sub

#End Region

#Region "Chargement des données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerPatients
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Charge la liste complète des patients depuis la base de données via GestionPatients,
    ' puis applique les filtres actifs pour alimenter le DataGridView.
    '
    ' Remarques :
    ' - La liste complète est conservée dans _patients pour le filtrage en mémoire.
    ' - Les erreurs sont journalisées via GestionLog et signalées dans le statut.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerPatients()

        Try
            Cursor = Cursors.WaitCursor

            _patients = GestionPatients.GetPatients()

            AppliquerFiltres()

            If _context IsNot Nothing Then
                _context.SetStatus($"{_patients.Count} patient(s) chargé(s).")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur chargement des patients (UC_PatientHome).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des patients.")
            End If

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltres
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Filtre la liste _patients en mémoire selon :
    ' - Le texte de recherche (nom, prénom, NISS, code, téléphone, email)
    ' - Le statut de suivi sélectionné (en cours / clôturé / tous)
    '
    ' Remarques :
    ' - Lie le résultat au DataGridView.
    ' - Met à jour l'état du bouton de réinitialisation des filtres.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltres()

        If _patients Is Nothing Then
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            Dim patientsFiltres = _patients.AsEnumerable()

            ' Filtre : texte de recherche multi-champs
            Dim texteRecherche As String = txtRecherchePatient.Text.Trim()
            If Not String.IsNullOrEmpty(texteRecherche) Then
                patientsFiltres = patientsFiltres.Where(
                    Function(p) CorrespondRecherche(p, texteRecherche)
                )
            End If

            ' Filtre : statut de suivi (3 états exclusifs)
            Select Case cboFiltreSuivi.SelectedIndex
                Case FiltreSuiviEnCours
                    patientsFiltres = patientsFiltres.Where(Function(p) p.SuiviEnCours)
                Case FiltreSuiviCloture
                    patientsFiltres = patientsFiltres.Where(Function(p) Not p.SuiviEnCours)
                Case Else
                    ' FiltreSuiviTous : aucun filtre de statut
            End Select

            Dim resultats = patientsFiltres.ToList()

            dgvPatients.DataSource = Nothing
            dgvPatients.DataSource = resultats

            dgvPatients.ClearSelection()

            ' Le bouton de réinitialisation n'est actif que si un filtre s'écarte de l'état par défaut
            btnReinitialiserFiltres.Enabled =
                Not String.IsNullOrEmpty(texteRecherche) OrElse cboFiltreSuivi.SelectedIndex <> FiltreSuiviEnCours

            If _context IsNot Nothing Then
                _context.SetStatus($"{resultats.Count} patient(s) affiché(s).")
            End If

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondRecherche
    ' Version  : V1.0.0
    ' Date     : 11/06/2026
    '
    ' Rôle     :
    ' Indique si un patient correspond au texte de recherche (comparaison insensible à la casse).
    '
    ' Paramètres :
    ' - patient : Le patient à tester
    ' - texte   : Le texte recherché (déjà nettoyé)
    '
    ' Retour   :
    ' - Boolean : True si l'un des champs (nom, prénom, NISS, code, téléphone, email) contient le texte
    '
    ' Remarques :
    ' - Les champs optionnels (NISS, téléphone, email) peuvent être Nothing : protégés par If(..., "").
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondRecherche(patient As PatientListeItem, texte As String) As Boolean

        Return If(patient.Nom, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(patient.Prenom, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(patient.Niss, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(patient.CodePatient, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(patient.Telephone, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(patient.Email, "").Contains(texte, StringComparison.OrdinalIgnoreCase)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : GetPatientSelectionne
    ' Version  : V1.0.0
    ' Date     : 11/06/2026
    '
    ' Rôle     :
    ' Retourne le patient actuellement sélectionné dans le DataGridView.
    '
    ' Retour   :
    ' - PatientListeItem : patient sélectionné, ou Nothing si aucune sélection valide.
    '
    ' Remarques :
    ' - Ne déclenche aucune action métier.
    ' -------------------------------------------------------------------------------------------------
    Private Function GetPatientSelectionne() As PatientListeItem

        If dgvPatients.CurrentRow Is Nothing Then
            Return Nothing
        End If

        Return TryCast(
            dgvPatients.CurrentRow.DataBoundItem,
            PatientListeItem
        )

    End Function

#End Region

#Region "Événements UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnRechercher_Click
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Applique les critères de recherche/filtrage sur la liste en mémoire.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRechercher_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRechercher.Click

        AppliquerFiltres()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtRecherchePatient_KeyDown
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Permet de déclencher la recherche par la touche Entrée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRecherchePatient_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles txtRecherchePatient.KeyDown

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            AppliquerFiltres()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboFiltreSuivi_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 19/07/2026
    '
    ' Rôle      :
    ' Réapplique les filtres lorsque le statut de suivi sélectionné change.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboFiltreSuivi_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles cboFiltreSuivi.SelectedIndexChanged

        AppliquerFiltres()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnReinitialiserFiltres_Click
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Réinitialise tous les critères de recherche et réaffiche la liste complète.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReinitialiserFiltres_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnReinitialiserFiltres.Click

        txtRecherchePatient.Clear()
        cboFiltreSuivi.SelectedIndex = FiltreSuiviEnCours

        AppliquerFiltres()

        txtRecherchePatient.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActualiser_Click
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Recharge la liste des patients depuis la base de données.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActualiser_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnActualiser.Click

        ChargerPatients()
        dgvPatients.ClearSelection()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnNouveau_Click
    ' Version   : V1.1.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Ouvre la fiche patient en mode création (UC_PatientFiche).
    '
    ' Remarques :
    ' - Empile l'écran courant (liste + filtre) dans la mini-pile de navigation pour le retour (D-Q15).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNouveau_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnNouveau.Click

        Dim homeForm As Home = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        Dim fiche As New UC_PatientFiche()
        fiche.InitialiserPourCreation()

        homeForm.NavigateToPatientFiche(
            fiche,
            "Patients > Nouveau patient",
            CreerEcranCourant()
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Ouvrira la fiche patient sélectionnée en mode modification (UC_PatientFiche, à implémenter).
    '
    ' Remarques :
    ' - L'écran de fiche patient n'est pas encore disponible : statut informatif en attendant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifier.Click

        OuvrirFichePatientSelectionne("Modification")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnOuvrir_Click
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Ouvrira la fiche patient sélectionnée en mode consultation (UC_PatientFiche, à implémenter).
    '
    ' Remarques :
    ' - L'écran de fiche patient n'est pas encore disponible : statut informatif en attendant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnOuvrir_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnOuvrir.Click

        OuvrirFichePatientSelectionne("Consultation")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvPatients_CellFormatting
    ' Version   : V1.1.0
    ' Date      : 19/07/2026
    '
    ' Rôle      :
    ' Met en forme l'affichage de certaines colonnes de la grille sans altérer la donnée sous-jacente :
    ' - Colonne Suivi : icône d'état (en cours / clôturé) selon PatientListeItem.SuiviEnCours.
    ' - Colonne Téléphone : masque international lisible (brique réutilisable UtilsTelephone).
    '
    ' Remarques :
    ' - Le numéro est stocké au format E.164 : le pays est déduit de l'indicatif présent dans le numéro.
    ' - Les autres colonnes conservent leur rendu par défaut.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPatients_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvPatients.CellFormatting

        If e.RowIndex < 0 Then
            Return
        End If

        ' Colonne Suivi : icône d'état selon le statut du patient
        If e.ColumnIndex = colStatutSuivi.Index Then

            Dim patient As PatientListeItem =
                TryCast(dgvPatients.Rows(e.RowIndex).DataBoundItem, PatientListeItem)

            If patient IsNot Nothing Then
                If patient.SuiviEnCours Then
                    e.Value = UtilsIcons.IconPatientEnCours()
                Else
                    e.Value = UtilsIcons.IconPatientNonEnCours()
                End If
                e.FormattingApplied = True
            End If

            Return

        End If

        ' Colonne Téléphone : masque international lisible
        If e.ColumnIndex <> colTelephone.Index Then
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
    ' Procédure : dgvPatients_CellDoubleClick
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Ouvre la fiche du patient sur double-clic dans la grille.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPatients_CellDoubleClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles dgvPatients.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        OuvrirFichePatientSelectionne("Consultation")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirFichePatientSelectionne
    ' Version   : V1.1.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Point d'entrée commun vers la fiche patient (modes Consultation / Modification).
    '
    ' Paramètres :
    ' - mode : Libellé du mode demandé ("Consultation" ou "Modification")
    '
    ' Remarques :
    ' - Vérifie qu'un patient est sélectionné.
    ' - Ouvre UC_PatientFiche dans le mode demandé en empilant l'écran courant pour le retour (D-Q15).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirFichePatientSelectionne(mode As String)

        Dim patient As PatientListeItem = GetPatientSelectionne()

        If patient Is Nothing Then
            If _context IsNot Nothing Then
                _context.SetStatus("Aucun patient sélectionné.")
            End If
            Return
        End If

        Dim homeForm As Home = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        Dim enModification As Boolean =
            String.Equals(mode, "Modification", StringComparison.OrdinalIgnoreCase)

        Dim fiche As New UC_PatientFiche()
        fiche.InitialiserPourPatient(patient.IdPatient, enModification)

        homeForm.NavigateToPatientFiche(
            fiche,
            $"Patients > {patient.NomComplet}",
            CreerEcranCourant()
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CreerEcranCourant
    ' Version  : V1.0.0
    ' Date     : 12/06/2026
    '
    ' Rôle     :
    ' Construit une fabrique recréant l'écran patients courant à l'identique au retour (mini-pile D-Q15).
    '
    ' Retour   :
    ' - Func(Of UserControl) : fabrique d'un nouveau UC_PatientHome restaurant le filtre courant
    '
    ' Remarques :
    ' - Le filtre courant (texte + statut de suivi) est capturé par closure et réinjecté via
    '   RestaurerFiltre() afin que la liste revienne avec ses critères de recherche.
    ' -------------------------------------------------------------------------------------------------
    Private Function CreerEcranCourant() As Func(Of UserControl)

        Dim texteFiltre As String = txtRecherchePatient.Text
        Dim filtreSuivi As Integer = cboFiltreSuivi.SelectedIndex

        Return Function()
                   Dim ecran As New UC_PatientHome()
                   ecran.RestaurerFiltre(texteFiltre, filtreSuivi)
                   Return ecran
               End Function

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RestaurerFiltre
    ' Version   : V1.1.0
    ' Date      : 19/07/2026
    '
    ' Rôle      :
    ' Mémorise le filtre à restaurer au prochain chargement de l'écran (retour depuis la fiche, D-Q15).
    '
    ' Paramètres :
    ' - texteRecherche : Texte de recherche à réappliquer
    ' - filtreSuivi    : Index du statut de suivi (cboFiltreSuivi) à réappliquer
    '
    ' Remarques :
    ' - Appelée par la fabrique CreerEcranCourant() avant l'ajout du contrôle ; les valeurs sont
    '   réinjectées dans les contrôles par AppliquerFiltreInitial() au Load.
    ' -------------------------------------------------------------------------------------------------
    Public Sub RestaurerFiltre(texteRecherche As String, filtreSuivi As Integer)

        _filtreInitialTexte = texteRecherche
        _filtreInitialSuivi = filtreSuivi

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltreInitial
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Réinjecte le filtre mémorisé (RestaurerFiltre) dans les contrôles avant le chargement de la liste.
    '
    ' Remarques :
    ' - Sans appel préalable à RestaurerFiltre, les contrôles conservent leur état par défaut.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltreInitial()

        If _filtreInitialTexte IsNot Nothing Then
            txtRecherchePatient.Text = _filtreInitialTexte
        End If

        If _filtreInitialSuivi >= 0 AndAlso _filtreInitialSuivi < cboFiltreSuivi.Items.Count Then
            cboFiltreSuivi.SelectedIndex = _filtreInitialSuivi
        End If

    End Sub

#End Region

End Class
