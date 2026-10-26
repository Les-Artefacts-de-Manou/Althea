' -------------------------------------------------------------------------------------------------
' UserControl : UC_RecherchePatientsLieu
' Projet      : Althéa
' Version     : V1.1.2
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Recherche transversale R4 (ADR-52, décisions de Joëlle du 24/10/2026) : les patients d'un code
' postal ou d'une localité, et l'accès direct à leur fiche.
'
' Responsabilités :
' - Une zone « code postal ou localité » : à gauche les lieux qui y répondent (« 4317 Faimes — 3 »),
'   à droite les patients du lieu choisi, ou de tous les lieux trouvés
' - Inactifs comme dans l'écran Patients : masqués, case « Afficher les inactifs » (lignes grisées)
' - Aller à la fiche du patient (double-clic, Entrée ou « Ouvrir la fiche »)
'
' Remarques   :
' - Aucun accès direct à la base : GestionRecherche (patients chargés une fois, recherche en
'   mémoire à chaque frappe) ; aller à la fiche : OuvertureResultat.
' - Ouvert depuis le hub Recherche, empilé sur lui : Retour y ramène. La fiche ouverte depuis cette
'   liste est empilée sur elle : Retour la recrée avec son texte, la case, le lieu et le patient.
' - Critères en mémoire seulement, le temps de la session ; rien de ce qui est cherché n'est
'   journalisé.
' - V1.1.0 (24/10/2026) : pastille « Inactif » devant le nom d'un patient inactif (demande de Joëlle,
'   comme dans l'écran Patients).
' - V1.1.1 (25/10/2026, relevé aux captures) : texte d'une ligne inactive sélectionnée en
'   ColorGrisDiscret, comme les autres grilles (ColorGrisClair était illisible sur la sélection).
' - V1.1.2 (25/10/2026, audit C.4 étape 3, ADR-55) : fenêtre principale par IFenetrePrincipale
'   (lue dans le contexte) ; plus aucune référence à Home.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_RecherchePatientsLieu
    Implements IContextAwareUserControl

#Region "Constantes"

    ' Contexte affiché (et réaffiché au retour) : « Recherche > Patients par lieu ».
    Public Const CONTEXTE As String = OuvertureResultat.CONTEXTE_RECHERCHE & " > Patients par lieu"

#End Region

#Region "Variables privées"

    ' Contexte UI partagé fourni par Home (barre d'état, infobulles).
    Private _context As UserControlContext

    ' Critères à rétablir au chargement (retour depuis la fiche) ; lieu : Nothing = tous les lieux,
    ' chaîne vide = sans adresse.
    Private _texteInitial As String = String.Empty
    Private _inactifsInitial As Boolean
    Private _cleLieuInitiale As String
    Private _idPatientInitial As Long

    ' Patients chargés une fois (avec leur adresse) et pays du cabinet.
    Private _patients As New List(Of PatientListeItem)
    Private _paysCabinet As String = String.Empty

    ' Vrai pendant que l'écran pose lui-même les critères ou remplit les grilles.
    Private _chargement As Boolean

    ' Vrai si les patients n'ont pas pu être chargés (le message reste affiché).
    Private _erreurChargement As Boolean

#End Region

#Region "Contexte et initialisation"

    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext
        _context = context
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Prerenseigner
    ' Rôle      : Critères à rétablir au chargement (fabrique de retour) ; à appeler avant la navigation.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Prerenseigner(texte As String, afficherInactifs As Boolean, cleLieu As String, idPatient As Long)
        _texteInitial = If(texte, String.Empty)
        _inactifsInitial = afficherInactifs
        _cleLieuInitiale = cleLieu
        _idPatientInitial = idPatient
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_RecherchePatientsLieu_Load
    ' Version   : V1.0.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Grilles, critères du retour, chargement des patients et première liste des lieux.
    '
    ' Remarques :
    ' - Le lieu et le patient du retour sont resélectionnés après l'affichage (BeginInvoke) : une
    '   grille pas encore visible perd sa sélection quand elle le devient.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_RecherchePatientsLieu_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If DesignMode Then Return

        UtilsDataGrid.InitDataGridBasique(dgvLieux)
        UtilsDataGrid.InitDataGridBasique(dgvPatients)
        UtilsDataGrid.ActiverTriColonnes(dgvPatients)
        colNbPatients.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        _chargement = True
        Try
            txtLieu.Text = _texteInitial
            chkAfficherInactifs.Checked = _inactifsInitial
        Finally
            _chargement = False
        End Try

        If _context IsNot Nothing Then
            _context.SetToolTip(txtLieu, "Code postal (ou son début : « 43 ») et / ou localité (ou une partie), sans souci des accents ni des majuscules.")
            _context.SetToolTip(chkAfficherInactifs, "Montrer aussi les patients désactivés (lignes grisées).")
            _context.SetToolTip(btnOuvrir, "Ouvrir la fiche du patient sélectionné (aussi : double-clic ou Entrée sur la ligne).")
            _context.SetToolTip(btnRetour, "Revenir au hub Recherche.")
        End If

        Try
            _patients = GestionRecherche.ChargerPatientsPourLieux(_paysCabinet)
        Catch ex As Exception
            ' Détail au journal (GestionPatients) ; ici, un message sans détail technique.
            _patients = New List(Of PatientListeItem)
            _erreurChargement = True
        End Try

        AfficherLieux(_cleLieuInitiale)

        ' Sélection reposée une fois l'écran affiché (lieu du retour, sinon « Tous les lieux trouvés »).
        Dim cle As String = _cleLieuInitiale
        Dim idPatient As Long = _idPatientInitial
        BeginInvoke(New Action(Sub()
                                   SelectionnerLieu(cle)
                                   If idPatient > 0 Then SelectionnerPatient(idPatient)
                               End Sub))

        ActiveControl = txtLieu

    End Sub

#End Region

#Region "Recherche et affichage"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherLieux
    ' Version   : V1.0.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Lieux qui répondent au texte (en tête « Tous les lieux trouvés »), puis les patients du lieu
    ' sélectionné ; garde le lieu demandé s'il est encore listé.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherLieux(cleASelectionner As String)

        Dim nbInactifsMasques As Integer
        Dim lieux As List(Of LieuPatients) = GestionRecherche.GrouperParLieu(
            _patients, txtLieu.Text, chkAfficherInactifs.Checked, _paysCabinet, nbInactifsMasques)

        Dim lignes As New List(Of LieuPatients)
        If lieux.Count > 0 Then lignes.Add(LieuPatients.Tous(lieux))
        lignes.AddRange(lieux)

        _chargement = True
        Try
            dgvLieux.DataSource = Nothing
            dgvLieux.DataSource = lignes
        Finally
            _chargement = False
        End Try

        carteLieux.Titre = $"Lieux ({lieux.Count})"

        If _erreurChargement Then
            lblMessage.Text = "La recherche n'a pas pu aboutir (le détail est dans le journal)."
        ElseIf lieux.Count = 0 Then
            lblMessage.Text = If(nbInactifsMasques > 0, "Seulement des patients inactifs : cochez « Afficher les inactifs ».", "Aucun lieu ne correspond.")
        Else
            lblMessage.Text = String.Empty
        End If

        SelectionnerLieu(cleASelectionner)

        If _context IsNot Nothing Then
            Dim mention As String = If(nbInactifsMasques > 0,
                                       $" — {nbInactifsMasques} patient(s) inactif(s) masqué(s) : cocher « Afficher les inactifs ».",
                                       String.Empty)
            _context.SetStatus($"Patients par lieu : {lieux.Count} lieu(x), {lieux.Sum(Function(l) l.NbPatients)} patient(s).{mention}")
        End If

    End Sub

    ' Patients du lieu sélectionné (aucun si la liste des lieux est vide).
    Private Sub AfficherPatients()

        Dim lieu As LieuPatients = LieuSelectionne()
        Dim patients As List(Of PatientListeItem) = If(lieu Is Nothing, New List(Of PatientListeItem), lieu.Patients)

        UtilsDataGrid.ReinitialiserTri(dgvPatients)
        dgvPatients.DataSource = Nothing
        dgvPatients.DataSource = patients
        dgvPatients.ClearSelection()

        carteResultats.Titre = If(lieu Is Nothing, "Patients", $"Patients — {lieu.Libelle} ({patients.Count})")

        RafraichirBoutons()

    End Sub

    ' Sélectionne le lieu de cette clé s'il est listé, sinon le premier (« Tous les lieux trouvés »).
    Private Sub SelectionnerLieu(cle As String)

        Dim cible As DataGridViewRow = Nothing
        For Each ligne As DataGridViewRow In dgvLieux.Rows
            Dim lieu As LieuPatients = TryCast(ligne.DataBoundItem, LieuPatients)
            If lieu IsNot Nothing AndAlso String.Equals(lieu.Cle, cle, StringComparison.Ordinal) Then
                cible = ligne
                Exit For
            End If
        Next
        If cible Is Nothing AndAlso dgvLieux.Rows.Count > 0 Then cible = dgvLieux.Rows(0)

        _chargement = True
        Try
            dgvLieux.ClearSelection()
            If cible IsNot Nothing Then
                dgvLieux.CurrentCell = cible.Cells(colLieu.Index)
                cible.Selected = True
                If Not cible.Displayed Then dgvLieux.FirstDisplayedScrollingRowIndex = cible.Index
            End If
        Finally
            _chargement = False
        End Try

        AfficherPatients()

    End Sub

    ' Resélectionne le patient du retour s'il est listé (et le rend visible).
    Private Sub SelectionnerPatient(idPatient As Long)

        For Each ligne As DataGridViewRow In dgvPatients.Rows
            Dim patient As PatientListeItem = TryCast(ligne.DataBoundItem, PatientListeItem)
            If patient Is Nothing OrElse patient.IdPatient <> idPatient Then Continue For

            dgvPatients.ClearSelection()
            dgvPatients.CurrentCell = ligne.Cells(colNom.Index)
            ligne.Selected = True
            If Not ligne.Displayed Then dgvPatients.FirstDisplayedScrollingRowIndex = ligne.Index
            dgvPatients.Focus()
            Exit For
        Next

        RafraichirBoutons()

    End Sub

    Private Function LieuSelectionne() As LieuPatients
        Return UtilsDataGrid.ElementSelectionne(Of LieuPatients)(dgvLieux)
    End Function

    Private Function PatientSelectionne() As PatientListeItem
        Return UtilsDataGrid.ElementSelectionne(Of PatientListeItem)(dgvPatients)
    End Function

    Private Sub RafraichirBoutons()
        btnOuvrir.Enabled = PatientSelectionne() IsNot Nothing
    End Sub

#End Region

#Region "Aller à la fiche"

    Private Sub Ouvrir()

        Dim patient As PatientListeItem = PatientSelectionne()
        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If patient Is Nothing OrElse fenetre Is Nothing Then Return

        OuvertureResultat.OuvrirPatient(fenetre, GestionRecherche.ResultatDePatient(patient),
                                        CreerEcranCourant(patient.IdPatient), CONTEXTE)

    End Sub

    ' Fabrique de retour : la liste recréée avec ses critères, le lieu et le patient resélectionnés.
    Private Function CreerEcranCourant(idPatient As Long) As Func(Of UserControl)

        Dim texte As String = txtLieu.Text.Trim()
        Dim inactifs As Boolean = chkAfficherInactifs.Checked
        Dim cle As String = LieuSelectionne()?.Cle

        Return Function()
                   Dim ecran As New UC_RecherchePatientsLieu()
                   ecran.Prerenseigner(texte, inactifs, cle, idPatient)
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

    Private Sub txtLieu_TextChanged(sender As Object, e As EventArgs) Handles txtLieu.TextChanged
        If _chargement Then Return
        AfficherLieux(LieuSelectionne()?.Cle)
    End Sub

    Private Sub chkAfficherInactifs_CheckedChanged(sender As Object, e As EventArgs) Handles chkAfficherInactifs.CheckedChanged
        If _chargement Then Return
        AfficherLieux(LieuSelectionne()?.Cle)
    End Sub

    Private Sub dgvLieux_SelectionChanged(sender As Object, e As EventArgs) Handles dgvLieux.SelectionChanged
        If _chargement Then Return
        AfficherPatients()
    End Sub

    ' Patient inactif en gris, comme dans l'écran Patients.
    Private Sub dgvPatients_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvPatients.CellFormatting

        If e.RowIndex < 0 Then Return
        Dim patient As PatientListeItem = TryCast(dgvPatients.Rows(e.RowIndex).DataBoundItem, PatientListeItem)
        If patient Is Nothing OrElse patient.IsActif Then Return

        e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
        e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret

    End Sub

    ' Pastille « Inactif » devant le nom d'un patient inactif, comme dans l'écran Patients.
    Private Sub dgvPatients_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvPatients.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colNom.Index Then Return
        Dim patient As PatientListeItem = TryCast(dgvPatients.Rows(e.RowIndex).DataBoundItem, PatientListeItem)
        If patient Is Nothing OrElse patient.IsActif Then Return

        UtilsDataGrid.DessinerPastilleAvantTexte(dgvPatients, e, "Inactif", UtilsPastilles.Teinte(TonEtat.Inactif))

    End Sub

    Private Sub dgvPatients_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvPatients.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            e.Handled = True
            Ouvrir()
        End If
    End Sub

    Private Sub dgvPatients_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPatients.CellDoubleClick
        If e.RowIndex >= 0 Then Ouvrir()
    End Sub

    Private Sub dgvPatients_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPatients.SelectionChanged
        RafraichirBoutons()
    End Sub

    Private Sub btnOuvrir_Click(sender As Object, e As EventArgs) Handles btnOuvrir.Click
        Ouvrir()
    End Sub

    Private Sub btnRetour_Click(sender As Object, e As EventArgs) Handles btnRetour.Click
        Retourner()
    End Sub

#End Region

End Class
