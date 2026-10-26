' -------------------------------------------------------------------------------------------------
' UserControl : UC_RechercheReseau
' Projet      : Althéa
' Version     : V1.2.2
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Recherche transversale R5 (ADR-52, décisions de Joëlle du 24/10/2026) : un thérapeute, un
' intervenant ou un contact par son nom (ou son début), sa fiche et les patients qui en relèvent.
'
' Responsabilités :
' - Une zone « nom » : à gauche les personnes qui y répondent (une ligne par personne : nature,
'   précision, nombre de patients), à droite ses liens avec les patients (rôle, période, fiche)
' - Patients inactifs comme dans l'écran Patients (case « Afficher les inactifs ») ; suivis terminés
'   inclus et signalés
' - Aller au patient (sa fiche, onglet Famille / Contacts ou Intervenants) ; ouvrir la fiche de la
'   personne (contact ou intervenant en consultation, fenêtre du thérapeute)
'
' Remarques   :
' - Aucun accès direct à la base : GestionRecherche (réseau chargé une fois, recherche en mémoire à
'   chaque frappe) ; aller au patient : OuvertureResultat.
' - Ouvert depuis le hub Recherche, empilé sur lui : Retour y ramène. La fiche du patient ouverte
'   depuis cette liste est empilée sur elle : Retour la recrée avec ses critères, la personne et le
'   lien resélectionnés.
' - Critères en mémoire seulement, le temps de la session ; rien de ce qui est cherché n'est
'   journalisé.
' - V1.1.0 (24/10/2026) : colonne Code de la liste des personnes : le code TH… d'un thérapeute du
'   référentiel ; vide pour un intervenant saisi ou un contact, dont chaque fiche (FC…, AS…) a son
'   code dans la colonne Fiche des liens (code toujours visible, Rules § 15).
' - V1.2.0 (24/10/2026) : pastille « Inactif » devant le nom d'un patient inactif dans la liste des
'   liens (demande de Joëlle, comme dans l'écran Patients).
' - V1.2.1 (25/10/2026, relevé aux captures) : texte d'une ligne inactive sélectionnée en
'   ColorGrisDiscret, comme les autres grilles (ColorGrisClair était illisible sur la sélection).
' - V1.2.2 (25/10/2026, audit C.4 étape 3, ADR-55) : fenêtre principale par IFenetrePrincipale
'   (lue dans le contexte) ; plus aucune référence à Home.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_RechercheReseau
    Implements IContextAwareUserControl

#Region "Constantes"

    ' Contexte affiché (et réaffiché au retour) : « Recherche > Réseau ».
    Public Const CONTEXTE As String = OuvertureResultat.CONTEXTE_RECHERCHE & " > Réseau"

#End Region

#Region "Variables privées"

    ' Contexte UI partagé fourni par Home (barre d'état, infobulles).
    Private _context As UserControlContext

    ' Critères à rétablir au chargement (retour depuis la fiche du patient).
    Private _texteInitial As String = String.Empty
    Private _inactifsInitial As Boolean
    Private _clePersonneInitiale As String
    Private _lienInitial As (Nature As NatureResultat, Id As Long)

    ' Réseau chargé une fois (thérapeutes, intervenants, contacts et leurs liens).
    Private _reseau As New List(Of PersonneReseau)

    ' Vrai pendant que l'écran pose lui-même les critères ou remplit les grilles.
    Private _chargement As Boolean

    ' Vrai si le réseau n'a pas pu être chargé (le message reste affiché).
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
    Public Sub Prerenseigner(texte As String, afficherInactifs As Boolean, clePersonne As String,
                             natureLien As NatureResultat, idLien As Long)
        _texteInitial = If(texte, String.Empty)
        _inactifsInitial = afficherInactifs
        _clePersonneInitiale = clePersonne
        _lienInitial = (natureLien, idLien)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_RechercheReseau_Load
    ' Version   : V1.0.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Grilles, critères du retour, chargement du réseau et première liste des personnes.
    '
    ' Remarques :
    ' - La personne et le lien du retour sont resélectionnés après l'affichage (BeginInvoke) : une
    '   grille pas encore visible perd sa sélection quand elle le devient.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_RechercheReseau_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If DesignMode Then Return

        UtilsDataGrid.InitDataGridBasique(dgvPersonnes)
        UtilsDataGrid.InitDataGridBasique(dgvLiens)
        UtilsDataGrid.ActiverTriColonnes(dgvLiens)
        colNbPatients.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        colDepuis.DefaultCellStyle.Format = "dd/MM/yyyy"

        _chargement = True
        Try
            txtNom.Text = _texteInitial
            chkAfficherInactifs.Checked = _inactifsInitial
        Finally
            _chargement = False
        End Try

        If _context IsNot Nothing Then
            _context.SetToolTip(txtNom, "Nom ou début du nom (et prénom), sans souci des accents ni des majuscules : « dup », « dupont ma ».")
            _context.SetToolTip(chkAfficherInactifs, "Montrer aussi les patients désactivés (lignes grisées).")
            _context.SetToolTip(btnOuvrir, "Ouvrir la fiche du patient sélectionné, sur l'onglet Famille / Contacts ou Intervenants (aussi : double-clic ou Entrée).")
            _context.SetToolTip(btnFichePersonne, "Ouvrir la fiche de la personne : le contact ou l'intervenant (en consultation), ou la fenêtre du thérapeute.")
            _context.SetToolTip(btnRetour, "Revenir au hub Recherche.")
        End If

        ChargerReseau()
        AfficherPersonnes(_clePersonneInitiale)

        ' Sélection reposée une fois l'écran affiché (personne et lien du retour, sinon la première).
        Dim cle As String = _clePersonneInitiale
        Dim lien As (Nature As NatureResultat, Id As Long) = _lienInitial
        BeginInvoke(New Action(Sub()
                                   SelectionnerPersonne(cle)
                                   If lien.Id > 0 Then SelectionnerLien(lien.Nature, lien.Id)
                               End Sub))

        ActiveControl = txtNom

    End Sub

#End Region

#Region "Recherche et affichage"

    Private Sub ChargerReseau()
        Try
            _reseau = GestionRecherche.ChargerReseau()
            _erreurChargement = False
        Catch ex As Exception
            ' Détail au journal (GestionRecherche) ; ici, un message sans détail technique.
            _reseau = New List(Of PersonneReseau)
            _erreurChargement = True
        End Try
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherPersonnes
    ' Version   : V1.0.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Personnes qui répondent au texte, puis les liens de la personne sélectionnée ; garde la personne
    ' demandée si elle est encore listée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherPersonnes(cleASelectionner As String)

        Dim nbLiensMasques As Integer
        Dim personnes As List(Of PersonneReseau) = GestionRecherche.FiltrerReseau(
            _reseau, txtNom.Text, chkAfficherInactifs.Checked, nbLiensMasques)

        _chargement = True
        Try
            dgvPersonnes.DataSource = Nothing
            dgvPersonnes.DataSource = personnes
        Finally
            _chargement = False
        End Try

        cartePersonnes.Titre = $"Personnes ({personnes.Count})"

        If _erreurChargement Then
            lblMessage.Text = "La recherche n'a pas pu aboutir (le détail est dans le journal)."
        ElseIf personnes.Count = 0 Then
            lblMessage.Text = "Aucune personne de ce nom."
        Else
            lblMessage.Text = String.Empty
        End If

        SelectionnerPersonne(cleASelectionner)

        If _context IsNot Nothing Then
            Dim mention As String = If(nbLiensMasques > 0,
                                       $" — {nbLiensMasques} lien(s) avec des patients inactifs masqué(s) : cocher « Afficher les inactifs ».",
                                       String.Empty)
            _context.SetStatus($"Réseau par nom : {personnes.Count} personne(s).{mention}")
        End If

    End Sub

    ' Liens de la personne sélectionnée (aucun si la liste est vide).
    Private Sub AfficherLiens()

        Dim personne As PersonneReseau = PersonneSelectionnee()
        Dim liens As List(Of LienReseau) = If(personne Is Nothing, New List(Of LienReseau), personne.Liens)

        UtilsDataGrid.ReinitialiserTri(dgvLiens)
        dgvLiens.DataSource = Nothing
        dgvLiens.DataSource = liens
        dgvLiens.ClearSelection()

        carteResultats.Titre = If(personne Is Nothing, "Patients",
                                  $"Patients — {personne.NomComplet} ({personne.NbPatients})")

        RafraichirBoutons()

    End Sub

    ' Sélectionne la personne de cette clé si elle est listée, sinon la première.
    Private Sub SelectionnerPersonne(cle As String)

        Dim cible As DataGridViewRow = Nothing
        For Each ligne As DataGridViewRow In dgvPersonnes.Rows
            Dim p As PersonneReseau = TryCast(ligne.DataBoundItem, PersonneReseau)
            If p IsNot Nothing AndAlso cle IsNot Nothing AndAlso String.Equals(p.Cle, cle, StringComparison.Ordinal) Then
                cible = ligne
                Exit For
            End If
        Next
        If cible Is Nothing AndAlso dgvPersonnes.Rows.Count > 0 Then cible = dgvPersonnes.Rows(0)

        _chargement = True
        Try
            dgvPersonnes.ClearSelection()
            If cible IsNot Nothing Then
                dgvPersonnes.CurrentCell = cible.Cells(colPersonne.Index)
                cible.Selected = True
                If Not cible.Displayed Then dgvPersonnes.FirstDisplayedScrollingRowIndex = cible.Index
            End If
        Finally
            _chargement = False
        End Try

        AfficherLiens()

    End Sub

    ' Resélectionne le lien du retour s'il est listé (et le rend visible).
    Private Sub SelectionnerLien(nature As NatureResultat, idElement As Long)

        For Each ligne As DataGridViewRow In dgvLiens.Rows
            Dim lien As LienReseau = TryCast(ligne.DataBoundItem, LienReseau)
            If lien Is Nothing OrElse lien.Nature <> nature OrElse lien.IdElement <> idElement Then Continue For

            dgvLiens.ClearSelection()
            dgvLiens.CurrentCell = ligne.Cells(colPatient.Index)
            ligne.Selected = True
            If Not ligne.Displayed Then dgvLiens.FirstDisplayedScrollingRowIndex = ligne.Index
            dgvLiens.Focus()
            Exit For
        Next

        RafraichirBoutons()

    End Sub

    Private Function PersonneSelectionnee() As PersonneReseau
        Return UtilsDataGrid.ElementSelectionne(Of PersonneReseau)(dgvPersonnes)
    End Function

    Private Function LienSelectionne() As LienReseau
        Return UtilsDataGrid.ElementSelectionne(Of LienReseau)(dgvLiens)
    End Function

    Private Sub RafraichirBoutons()
        Dim personne As PersonneReseau = PersonneSelectionnee()
        btnOuvrir.Enabled = LienSelectionne() IsNot Nothing
        btnFichePersonne.Enabled = personne IsNot Nothing AndAlso
            (personne.Nature = NaturePersonneReseau.Therapeute OrElse personne.Liens.Count > 0)
    End Sub

#End Region

#Region "Aller au patient, fiche de la personne"

    Private Sub OuvrirPatient()

        Dim personne As PersonneReseau = PersonneSelectionnee()
        Dim lien As LienReseau = LienSelectionne()
        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If personne Is Nothing OrElse lien Is Nothing OrElse fenetre Is Nothing Then Return

        OuvertureResultat.Ouvrir(fenetre, GestionRecherche.ResultatDeLien(personne, lien),
                                 CreerEcranCourant(personne.Cle, lien), _context, CONTEXTE)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirFichePersonne
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' La fiche de la personne (décision de Joëlle) : thérapeute → sa fenêtre (comme le référentiel
    ' Thérapeutes) ; contact ou intervenant → sa fiche en consultation, celle du lien sélectionné (à
    ' défaut, du premier lien). Après un enregistrement, le réseau est relu.
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale par IFenetrePrincipale (contexte)
    '   au lieu de Home.
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirFichePersonne()

        Dim personne As PersonneReseau = PersonneSelectionnee()
        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If personne Is Nothing OrElse fenetre Is Nothing Then Return

        Dim enregistre As Boolean

        Try
            If personne.Nature = NaturePersonneReseau.Therapeute Then
                Dim ficheTherapeute As Therapeute = GestionTherapeutes.GetTherapeuteById(personne.IdTherapeute)
                If ficheTherapeute Is Nothing Then Return
                Using frm As New TherapeuteEdition(ficheTherapeute, fenetre)
                    frm.SetContext(_context)
                    enregistre = frm.ShowDialog(Me) = DialogResult.OK
                End Using
            Else
                Dim lien As LienReseau = LienSelectionne()
                If lien Is Nothing OrElse Not personne.Liens.Contains(lien) Then lien = personne.Liens.FirstOrDefault()
                If lien Is Nothing Then Return

                If lien.Nature = NatureResultat.Contact Then
                    Dim ficheContact As FamilleContact = GestionFamilleContacts.GetContactById(lien.IdElement)
                    Dim patientDuLien As Patient = GestionPatients.GetPatientById(lien.IdPatient)
                    If ficheContact Is Nothing OrElse patientDuLien Is Nothing Then Return
                    Using dlg As New ContactEdition(lien.IdPatient, fenetre, ficheContact, patientDuLien, ouvrirEnConsultation:=True)
                        dlg.SetContext(_context)
                        enregistre = dlg.ShowDialog(Me) = DialogResult.OK
                    End Using
                Else
                    Dim ficheSuivi As SuiviIntervenant = GestionSuivisIntervenants.GetSuiviById(lien.IdElement)
                    If ficheSuivi Is Nothing Then Return
                    Using dlg As New IntervenantEdition(lien.IdPatient, fenetre, ficheSuivi, ouvrirEnConsultation:=True)
                        dlg.SetContext(_context)
                        enregistre = dlg.ShowDialog(Me) = DialogResult.OK
                    End Using
                End If
            End If

        Catch ex As Exception
            ' Détail au journal (couche métier) ; ici, un message sans détail technique.
            _context?.SetStatus("La fiche n'a pas pu être ouverte (le détail est dans le journal).")
            Return
        End Try

        _context?.SetHeader(CONTEXTE)

        If enregistre Then
            Dim cle As String = personne.Cle
            ChargerReseau()
            AfficherPersonnes(cle)
        End If

    End Sub

    ' Fabrique de retour : la liste recréée avec ses critères, la personne et le lien resélectionnés.
    Private Function CreerEcranCourant(clePersonne As String, lien As LienReseau) As Func(Of UserControl)

        Dim texte As String = txtNom.Text.Trim()
        Dim inactifs As Boolean = chkAfficherInactifs.Checked
        Dim nature As NatureResultat = lien.Nature
        Dim idLien As Long = lien.IdElement

        Return Function()
                   Dim ecran As New UC_RechercheReseau()
                   ecran.Prerenseigner(texte, inactifs, clePersonne, nature, idLien)
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

    Private Sub txtNom_TextChanged(sender As Object, e As EventArgs) Handles txtNom.TextChanged
        If _chargement Then Return
        AfficherPersonnes(PersonneSelectionnee()?.Cle)
    End Sub

    Private Sub chkAfficherInactifs_CheckedChanged(sender As Object, e As EventArgs) Handles chkAfficherInactifs.CheckedChanged
        If _chargement Then Return
        AfficherPersonnes(PersonneSelectionnee()?.Cle)
    End Sub

    Private Sub dgvPersonnes_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPersonnes.SelectionChanged
        If _chargement Then Return
        AfficherLiens()
    End Sub

    ' Thérapeute désactivé en gris.
    Private Sub dgvPersonnes_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvPersonnes.CellFormatting

        If e.RowIndex < 0 Then Return
        Dim personne As PersonneReseau = TryCast(dgvPersonnes.Rows(e.RowIndex).DataBoundItem, PersonneReseau)
        If personne Is Nothing OrElse personne.TherapeuteActif Then Return

        e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
        e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret

    End Sub

    ' Patient inactif en gris, comme dans l'écran Patients ; suivi terminé atténué.
    Private Sub dgvLiens_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvLiens.CellFormatting

        If e.RowIndex < 0 Then Return
        Dim lien As LienReseau = TryCast(dgvLiens.Rows(e.RowIndex).DataBoundItem, LienReseau)
        If lien Is Nothing Then Return

        If Not lien.PatientActif OrElse (lien.EstTermine AndAlso e.ColumnIndex = colEtat.Index) Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

    End Sub

    ' Pastille « Inactif » devant le nom d'un patient inactif, comme dans l'écran Patients.
    Private Sub dgvLiens_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvLiens.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colPatient.Index Then Return
        Dim lien As LienReseau = TryCast(dgvLiens.Rows(e.RowIndex).DataBoundItem, LienReseau)
        If lien Is Nothing OrElse lien.PatientActif Then Return

        UtilsDataGrid.DessinerPastilleAvantTexte(dgvLiens, e, "Inactif", UtilsPastilles.Teinte(TonEtat.Inactif))

    End Sub

    Private Sub dgvLiens_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvLiens.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            e.Handled = True
            OuvrirPatient()
        End If
    End Sub

    Private Sub dgvLiens_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLiens.CellDoubleClick
        If e.RowIndex >= 0 Then OuvrirPatient()
    End Sub

    Private Sub dgvLiens_SelectionChanged(sender As Object, e As EventArgs) Handles dgvLiens.SelectionChanged
        RafraichirBoutons()
    End Sub

    Private Sub dgvPersonnes_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPersonnes.CellDoubleClick
        If e.RowIndex >= 0 Then OuvrirFichePersonne()
    End Sub

    Private Sub btnOuvrir_Click(sender As Object, e As EventArgs) Handles btnOuvrir.Click
        OuvrirPatient()
    End Sub

    Private Sub btnFichePersonne_Click(sender As Object, e As EventArgs) Handles btnFichePersonne.Click
        OuvrirFichePersonne()
    End Sub

    Private Sub btnRetour_Click(sender As Object, e As EventArgs) Handles btnRetour.Click
        Retourner()
    End Sub

#End Region

End Class
