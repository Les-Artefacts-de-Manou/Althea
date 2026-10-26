' -------------------------------------------------------------------------------------------------
' UserControl : UC_RechercheHome
' Projet      : Althéa
' Version     : V1.4.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Hub Recherche (ADR-52, décisions de Joëlle du 23/10/2026), ouvert par le bouton « Recherche » du
' menu : retrouver un élément sans passer par son patient, et y aller.
'
' Responsabilités :
' - Recherche rapide par code (PA, DO, SE, PAI, FC, AS) : ce que c'est, de quel patient, et y aller
'   (Entrée ou double-clic sur la ligne, ou « Ouvrir » ; « Ouvrir le patient » pour sa fiche)
' - Accueillir les autres recherches (tuiles), ajoutées étape par étape : R2 séances d'un jour ou
'   d'une période (UC_RechercheSeances) ; R3 paiements d'un jour ou d'une période
'   (UC_RecherchePaiements) ; R4 patients par lieu (UC_RecherchePatientsLieu) ; R5 thérapeute,
'   intervenant ou contact par nom et leurs patients (UC_RechercheReseau)
'
' Remarques   :
' - Aucun accès direct à la base : GestionRecherche ; aller à l'élément : OuvertureResultat.
' - Retour depuis l'écran ouvert : la recherche est recréée avec son code et relancée (fabrique de
'   la mini-pile, D-Q15) ; rien n'est gardé au-delà de la session.
' - Une tuile ouvre sa recherche empilée sur le hub (IFenetrePrincipale.NavigateDepuisRecherche) : Retour y ramène.
' - V1.1.0 (23/10/2026, R2) : tuile « Séances ».
' - V1.2.0 (23/10/2026, R3) : tuile « Paiements ».
' - V1.3.0 (24/10/2026, R4) : tuile « Patients par lieu ».
' - V1.4.0 (24/10/2026, R5) : tuile « Réseau par nom » ; code TH… (thérapeute) dans l'aide, aide
'   raccourcie pour tenir sur une ligne.
' - V1.4.1 (25/10/2026, audit C.4 étape 3, ADR-55) : fenêtre principale par IFenetrePrincipale
'   (lue dans le contexte) ; plus aucune référence à Home.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_RechercheHome
    Implements IContextAwareUserControl

#Region "Variables privées"

    ' Contexte UI partagé fourni par Home (barre d'état, infobulles).
    Private _context As UserControlContext

    ' Code à relancer au chargement (retour depuis l'écran ouvert).
    Private _codeInitial As String

    ' Résultats affichés.
    Private _resultats As New List(Of ResultatRecherche)

#End Region

#Region "Contexte et initialisation"

    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext
        _context = context
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : PrerenseignerCode
    ' Rôle      : Code à relancer au chargement (fabrique de retour) ; à appeler avant la navigation.
    ' -------------------------------------------------------------------------------------------------
    Public Sub PrerenseignerCode(code As String)
        _codeInitial = code
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_RechercheHome_Load
    ' Version   : V1.4.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Style de la grille, aide des codes, infobulles ; relance la recherche du retour s'il y en a une.
    '
    ' Remarques :
    ' - V1.1.0 (23/10/2026) : tuile « Séances » (icône selon son état, UtilsButtons).
    ' - V1.2.0 (23/10/2026) : tuile « Paiements ».
    ' - V1.3.0 (24/10/2026) : tuile « Patients par lieu ».
    ' - V1.4.0 (24/10/2026) : tuile « Réseau par nom ».
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_RechercheHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If DesignMode Then Return

        UtilsDataGrid.InitDataGridBasique(dgvResultats)
        UtilsButtons.InitLargeIconButton(btnSeances)
        UtilsButtons.InitLargeIconButton(btnPaiements)
        UtilsButtons.InitLargeIconButton(btnPatientsLieu)
        UtilsButtons.InitLargeIconButton(btnReseau)
        lblAide.Text = $"Tapez un code — {GestionRecherche.AideCodes} — puis Entrée (ex. SE83)."

        If _context IsNot Nothing Then
            _context.SetToolTip(btnChercher, "Chercher l'élément qui porte ce code.")
            _context.SetToolTip(btnOuvrir, "Aller à l'élément sélectionné (aussi : double-clic ou Entrée sur la ligne).")
            _context.SetToolTip(btnOuvrirPatient, "Ouvrir la fiche du patient de l'élément sélectionné.")
            _context.SetToolTip(btnSeances, "Les séances d'un jour ou d'une période, tous patients confondus, et l'accès à leur dossier.")
            _context.SetToolTip(btnPaiements, "Les paiements d'un jour ou d'une période, tous patients confondus, les totaux par mode et l'accès à leur dossier.")
            _context.SetToolTip(btnPatientsLieu, "Les patients d'un code postal ou d'une localité, et l'accès à leur fiche.")
            _context.SetToolTip(btnReseau, "Un thérapeute, un intervenant ou un contact par son nom : sa fiche et les patients qui en relèvent.")
            _context.SetStatus("Recherche : tapez un code.")
        End If

        AfficherResultats(New List(Of ResultatRecherche))

        If Not String.IsNullOrWhiteSpace(_codeInitial) Then
            txtCode.Text = _codeInitial
            Chercher()
        End If

        ActiveControl = If(_resultats.Count > 0, CType(dgvResultats, Control), txtCode)

    End Sub

#End Region

#Region "Recherche par code"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Chercher
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Cherche l'élément du code saisi et l'affiche (sélectionné : Entrée l'ouvre) ; dit pourquoi
    ' quand rien n'est trouvé (code non reconnu, aucun élément).
    ' -------------------------------------------------------------------------------------------------
    Private Sub Chercher()

        lblMessage.Text = String.Empty
        Dim saisie As String = txtCode.Text.Trim()

        If saisie.Length = 0 Then
            AfficherResultats(New List(Of ResultatRecherche))
            lblMessage.Text = "Tapez un code."
            Return
        End If

        Dim nature As NatureResultat
        Dim id As Long
        If Not GestionRecherche.InterpreterCode(saisie, nature, id) Then
            AfficherResultats(New List(Of ResultatRecherche))
            lblMessage.Text = $"« {saisie} » n'est pas un code reconnu ({GestionRecherche.PrefixesReconnus} suivi de chiffres)."
            Return
        End If

        Try
            AfficherResultats(GestionRecherche.RechercherParCode(saisie))
        Catch ex As Exception
            ' Détail au journal (GestionRecherche) ; ici, un message sans détail technique.
            AfficherResultats(New List(Of ResultatRecherche))
            lblMessage.Text = "La recherche n'a pas pu aboutir (le détail est dans le journal)."
            Return
        End Try

        If _resultats.Count = 0 Then
            lblMessage.Text = "Aucun élément ne porte ce code."
        Else
            lblMessage.Text = "Entrée ou double-clic sur la ligne pour y aller."
            dgvResultats.Focus()
        End If

        _context?.SetStatus($"Recherche par code : {_resultats.Count} résultat(s).")

    End Sub

    ' Affiche les résultats ; le premier est sélectionné (Entrée l'ouvre).
    Private Sub AfficherResultats(resultats As List(Of ResultatRecherche))

        _resultats = If(resultats, New List(Of ResultatRecherche))
        dgvResultats.DataSource = Nothing
        dgvResultats.DataSource = _resultats
        dgvResultats.ClearSelection()

        If dgvResultats.Rows.Count > 0 Then
            dgvResultats.CurrentCell = dgvResultats.Rows(0).Cells(colCode.Index)
            dgvResultats.Rows(0).Selected = True
        End If

        RafraichirBoutons()

    End Sub

    Private Function ResultatSelectionne() As ResultatRecherche
        Return UtilsDataGrid.ElementSelectionne(Of ResultatRecherche)(dgvResultats)
    End Function

    Private Sub RafraichirBoutons()
        Dim r As ResultatRecherche = ResultatSelectionne()
        btnOuvrir.Enabled = r IsNot Nothing
        btnOuvrirPatient.Enabled = r IsNot Nothing AndAlso r.IdPatient > 0
    End Sub

#End Region

#Region "Aller à l'élément"

    Private Sub Ouvrir(patientSeulement As Boolean)

        Dim r As ResultatRecherche = ResultatSelectionne()
        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If r Is Nothing OrElse fenetre Is Nothing Then Return

        If patientSeulement Then
            OuvertureResultat.OuvrirPatient(fenetre, r, CreerEcranCourant())
        Else
            OuvertureResultat.Ouvrir(fenetre, r, CreerEcranCourant(), _context)
        End If

    End Sub

    ' Fabrique de retour : la recherche recréée avec son code, relancée au chargement.
    Private Function CreerEcranCourant() As Func(Of UserControl)

        Dim code As String = txtCode.Text.Trim()

        Return Function()
                   Dim ecran As New UC_RechercheHome()
                   ecran.PrerenseignerCode(code)
                   Return ecran
               End Function

    End Function

    ' Une recherche spécialisée (tuile), empilée sur le hub : Retour y ramène, code compris.
    Private Sub OuvrirRecherche(ecran As UserControl, contexte As String)
        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If fenetre Is Nothing Then Return
        fenetre.NavigateDepuisRecherche(ecran, contexte, CreerEcranCourant())
    End Sub

#End Region

#Region "Gestionnaires d'événements"

    Private Sub btnSeances_Click(sender As Object, e As EventArgs) Handles btnSeances.Click
        OuvrirRecherche(New UC_RechercheSeances(), UC_RechercheSeances.CONTEXTE)
    End Sub

    Private Sub btnPaiements_Click(sender As Object, e As EventArgs) Handles btnPaiements.Click
        OuvrirRecherche(New UC_RecherchePaiements(), UC_RecherchePaiements.CONTEXTE)
    End Sub

    Private Sub btnPatientsLieu_Click(sender As Object, e As EventArgs) Handles btnPatientsLieu.Click
        OuvrirRecherche(New UC_RecherchePatientsLieu(), UC_RecherchePatientsLieu.CONTEXTE)
    End Sub

    Private Sub btnReseau_Click(sender As Object, e As EventArgs) Handles btnReseau.Click
        OuvrirRecherche(New UC_RechercheReseau(), UC_RechercheReseau.CONTEXTE)
    End Sub

    Private Sub btnChercher_Click(sender As Object, e As EventArgs) Handles btnChercher.Click
        Chercher()
    End Sub

    Private Sub txtCode_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCode.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Chercher()
        End If
    End Sub

    Private Sub dgvResultats_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvResultats.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            e.Handled = True
            Ouvrir(False)
        End If
    End Sub

    Private Sub dgvResultats_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvResultats.CellDoubleClick
        If e.RowIndex >= 0 Then Ouvrir(False)
    End Sub

    Private Sub dgvResultats_SelectionChanged(sender As Object, e As EventArgs) Handles dgvResultats.SelectionChanged
        RafraichirBoutons()
    End Sub

    Private Sub btnOuvrir_Click(sender As Object, e As EventArgs) Handles btnOuvrir.Click
        Ouvrir(False)
    End Sub

    Private Sub btnOuvrirPatient_Click(sender As Object, e As EventArgs) Handles btnOuvrirPatient.Click
        Ouvrir(True)
    End Sub

#End Region

End Class
