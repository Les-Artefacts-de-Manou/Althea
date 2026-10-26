' -------------------------------------------------------------------------------------------------
' Classe      : UC_ApercuPatient
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 17/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Panneau d'aperçu du patient sélectionné dans l'écran Patients (maquette « Aperçu patient » du
' 17/10/2026) : identité, alerte complète, séance du jour, prochaines et dernières séances,
' dossier, finances, contacts — pour voir l'essentiel sans ouvrir de fiche (demande du cahier
' des charges de Pearl).
'
' Responsabilités :
' - Afficher un ApercuPatient (AfficherApercu) ou l'invite « Cliquez sur un patient » (Vider)
' - Demander à l'écran hôte d'ouvrir la fiche, le dossier ou une séance (événements)
'
' Remarques   :
' - Lecture seule ; aucun accès base : l'écran hôte charge l'aperçu (GestionApercuPatient).
' - Le texte des notes n'est jamais affiché, seulement leur présence (décision de Joëlle :
'   « pour lire, on ouvre la séance »).
' - La place « Documents récents » est réservée à la gestion documentaire (aperçus PDF).
'
' Dépendances :
' - ApercuPatient, RepereSeance, UtilsDataGrid, UtilsPastilles, UITheme, AltheaIconeEtat,
'   CheminsPatientHelper, UtilsImages, UtilsTelephone, SeanceListeItem
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_ApercuPatient

#Region "Événements"

    ' Ouvrir la fiche du patient affiché.
    Public Event OuvrirFicheDemandee As EventHandler

    ' Ouvrir un dossier du patient (identifiant et code du dossier).
    Public Event OuvrirDossierDemande(idDossier As Long, codeDossier As String)

    ' Ouvrir une séance (prise de notes, consultation).
    Public Event OuvrirSeanceDemandee(seance As RepereSeance)

#End Region

#Region "Variables privées"

    ' Aperçu affiché (Nothing : invite « Cliquez sur un patient »).
    Private _apercu As ApercuPatient

    ' Séance ouverte par « Notes de la séance » (celle du jour à venir, sinon la dernière du jour).
    Private _seanceDuJour As SeanceListeItem

    ' Date d'une séance à venir, en gras (police partagée : CellFormatting est appelé très souvent).
    Private Shared ReadOnly PoliceDateAVenir As New Font("Calibri", 9.5F, FontStyle.Bold)

    ' Hauteur des lignes de la petite grille des séances (pastilles comprises).
    Private Const HAUTEUR_LIGNE_SEANCE As Integer = 26

    ' Contrôles montrés seulement quand un patient est affiché.
    Private ReadOnly Property ControlesContenu As Control()
        Get
            Return {pnlIdentite, flpBoutons, lblAlerte, titreAujourdhui, lblSeanceJour, btnNotesSeance,
                    titreSeances, dgvSeancesApercu, titreDossier, lblDossier, titreFinances, lblFinances,
                    titreContacts, lblContacts, titreDocuments, lblDocuments}
        End Get
    End Property

#End Region

#Region "Initialisation"

    Public Sub New()
        InitializeComponent()

        UtilsDataGrid.InitDataGridSecondaire(dgvSeancesApercu)
        dgvSeancesApercu.ColumnHeadersVisible = False
        dgvSeancesApercu.RowTemplate.Height = HAUTEUR_LIGNE_SEANCE
        dgvSeancesApercu.BackgroundColor = Color.White

        lblAlerte.BackColor = AltheaIconeEtat.Teintes(EtatIcone.Avertissement).Pale
        lblAlerte.ForeColor = AltheaIconeEtat.Teintes(EtatIcone.Avertissement).Foncee
        btnNotesSeance.Variante = VarianteBouton.Sauge

        Vider()
    End Sub

#End Region

#Region "Affichage"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Vider
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Affiche l'invite « Cliquez sur un patient… » (aucune sélection) ou un message donné.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Vider(Optional message As String = Nothing)

        _apercu = Nothing
        _seanceDuJour = Nothing
        UtilsImages.RemplacerImage(picPhoto, Nothing)

        flpContenu.SuspendLayout()
        For Each controle As Control In ControlesContenu
            controle.Visible = False
        Next
        lblVide.Text = If(String.IsNullOrEmpty(message), "Cliquez sur un patient pour voir son aperçu.", message)
        lblVide.Visible = True
        flpContenu.ResumeLayout(True)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherApercu
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Remplit le panneau avec l'aperçu d'un patient ; les sections sans contenu sont masquées
    ' (alerte, aujourd'hui) ou disent « aucun ».
    ' -------------------------------------------------------------------------------------------------
    Public Sub AfficherApercu(apercu As ApercuPatient)

        If apercu Is Nothing OrElse apercu.Patient Is Nothing Then
            Vider()
            Return
        End If

        _apercu = apercu

        flpContenu.SuspendLayout()
        Try
            lblVide.Visible = False
            For Each controle As Control In ControlesContenu
                controle.Visible = True
            Next

            RemplirIdentite(apercu)
            RemplirAujourdhui(apercu)
            RemplirSeances(apercu)
            lblDossier.Text = TexteDossier(apercu)
            RemplirFinances(apercu)
            lblContacts.Text = TexteContacts(apercu)

            flpContenu.AutoScrollPosition = New Point(0, 0)
        Finally
            flpContenu.ResumeLayout(True)
        End Try

        AjusterLargeurs()

    End Sub

    ' Identité, boutons, alerte.
    Private Sub RemplirIdentite(apercu As ApercuPatient)

        Dim fiche As Patient = apercu.Patient

        lblNom.Text = $"{If(fiche.Nom, String.Empty).ToUpperInvariant()} {fiche.Prenom}".Trim()

        Dim infos As New List(Of String) From {fiche.CodePatient}
        If fiche.DateNaissance.HasValue Then
            Dim age As Integer = Patient.CalculerAge(fiche.DateNaissance.Value, Date.Today)
            infos.Add($"{age} an{If(age > 1, "s", "")}")
            infos.Add($"né(e) le {fiche.DateNaissance.Value:dd/MM/yyyy}")
        End If
        If Not fiche.IsActif Then infos.Add("inactif")
        lblInfos.Text = String.Join(" · ", infos.Where(Function(t) Not String.IsNullOrWhiteSpace(t)))

        UtilsImages.RemplacerImage(picPhoto, CheminsPatientHelper.ChargerImagePatient(fiche.CodePatient, fiche.PhotoFichier))
        picPhoto.Invalidate()

        Dim dossier As DossierListeItem = apercu.DossierCourant
        btnOuvrirDossier.Visible = dossier IsNot Nothing
        If dossier IsNot Nothing Then btnOuvrirDossier.Text = $"Ouvrir le dossier {dossier.CodeDossier}"

        lblAlerte.Visible = fiche.AAlerte
        lblAlerte.Text = If(fiche.AAlerte, fiche.AlerteTxt.Trim(), String.Empty)

    End Sub

    ' Séance(s) du jour et bouton « Notes de la séance » (section masquée sans séance aujourd'hui).
    Private Sub RemplirAujourdhui(apercu As ApercuPatient)

        Dim seances As List(Of SeanceListeItem) = apercu.SeancesDuJour
        Dim visible As Boolean = seances.Count > 0

        titreAujourdhui.Visible = visible
        lblSeanceJour.Visible = visible
        btnNotesSeance.Visible = visible
        _seanceDuJour = Nothing
        If Not visible Then Return

        titreAujourdhui.Text = $"Aujourd'hui — {Date.Today:dddd d MMMM}"
        lblSeanceJour.Text = String.Join(Environment.NewLine,
            seances.Select(Function(s) $"{HoraireCourt(s)} · {s.CodeSeance} · {s.LibelleNatureSeance} · {apercu.CodeDossier(s.IdDossier)}" &
                                       If(s.EstNoShow, " · absence non excusée", If(s.EstAConfirmer, " · à confirmer", If(s.EstPassee, " · passée", String.Empty)))))

        ' La prochaine séance du jour, sinon la dernière passée (prise de notes juste après).
        _seanceDuJour = If(seances.FirstOrDefault(Function(s) Not s.EstPassee), seances.Last())
        btnNotesSeance.Text = $"Notes de la séance {_seanceDuJour.CodeSeance}"
        btnNotesSeance.Width = TextRenderer.MeasureText(btnNotesSeance.Text, btnNotesSeance.Font).Width + 40

    End Sub

    ' Prochaines séances puis dernières séances, dans la petite grille.
    Private Sub RemplirSeances(apercu As ApercuPatient)

        dgvSeancesApercu.Rows.Clear()

        For Each seance As SeanceListeItem In apercu.ProchainesSeances.Concat(apercu.DernieresSeances)
            Dim index As Integer = dgvSeancesApercu.Rows.Add(
                $"{seance.DateSeance:ddd dd/MM}",
                LibelleSeance(seance),
                String.Empty)
            dgvSeancesApercu.Rows(index).Tag = seance
        Next

        Dim aucune As Boolean = dgvSeancesApercu.Rows.Count = 0
        dgvSeancesApercu.Visible = Not aucune
        titreSeances.Text = If(aucune, "Séances — aucune", "Séances")
        dgvSeancesApercu.Height = Math.Max(1, dgvSeancesApercu.Rows.Count) * HAUTEUR_LIGNE_SEANCE + 2
        dgvSeancesApercu.ClearSelection()

    End Sub

    ' « 14:00 · Traitement · notes ✓ » : heure et nature ; pour une séance passée, présence des notes.
    Private Shared Function LibelleSeance(seance As SeanceListeItem) As String
        Dim texte As String = $"{HoraireCourt(seance)} · {seance.LibelleNatureSeance}"
        If seance.EstPassee AndAlso Not seance.EstNoShow Then
            texte &= If(String.IsNullOrWhiteSpace(seance.NotesSeanceTxt), " · sans notes", " · notes ✓")
        End If
        Return texte
    End Function

    ' Heure de début (« 14:00 ») ou « — » si la séance n'a pas d'heure.
    Private Shared Function HoraireCourt(seance As SeanceListeItem) As String
        Return If(seance.HeureDebut.HasValue, seance.HeureDebut.Value.ToString("hh\:mm"), "—")
    End Function

    ' « Actif · GRA · depuis le 05/09/2026 » ; autres dossiers ; quota INAMI.
    Private Shared Function TexteDossier(apercu As ApercuPatient) As String

        Dim dossier As DossierListeItem = apercu.DossierCourant
        If dossier Is Nothing Then
            Return "Aucun dossier."
        End If

        Dim lignes As New List(Of String) From {
            $"{dossier.CodeDossier} · {dossier.LibelleStatutDossier} · {dossier.LibelleDomaine} · depuis le {dossier.DateOuverture:dd/MM/yyyy}"
        }
        If Not String.IsNullOrWhiteSpace(apercu.QuotaDossierCourant) Then
            lignes.Add($"Quota INAMI : {apercu.QuotaDossierCourant}")
        End If

        Dim autres As List(Of DossierListeItem) = apercu.Dossiers.Where(Function(d) d.IdDossier <> dossier.IdDossier).ToList()
        If autres.Count > 0 Then
            lignes.Add("Autres : " & String.Join(", ", autres.Select(Function(d) $"{d.CodeDossier} ({d.LibelleStatutDossier}, {d.LibelleDomaine})")))
        End If

        Return String.Join(Environment.NewLine, lignes)

    End Function

    ' Montant à régler (rose) ou « À jour » (sauge), avoir, absences.
    Private Sub RemplirFinances(apercu As ApercuPatient)

        Dim lignes As New List(Of String)

        If apercu.MontantARegler > 0D Then
            lignes.Add($"À régler : {apercu.MontantARegler:N2} €")
            lblFinances.ForeColor = UITheme.StatutDuFore
        Else
            lignes.Add("À jour")
            lblFinances.ForeColor = UITheme.ColorSaugeFonce
        End If

        If apercu.Avoir > 0D Then lignes.Add($"Avoir : {apercu.Avoir:N2} €")
        lignes.Add(If(apercu.NbAbsences > 0, $"{apercu.NbAbsences} absence(s) non excusée(s)", "Aucune absence non excusée"))

        lblFinances.Text = String.Join(Environment.NewLine, lignes)

    End Sub

    ' Téléphone et e-mail du patient, puis l'entourage (« Mère : Julie LAMBERT · 0486 … »).
    Private Shared Function TexteContacts(apercu As ApercuPatient) As String

        Dim fiche As Patient = apercu.Patient
        Dim lignes As New List(Of String)

        Dim coordonnees As String = String.Join(" · ", {Telephone(fiche.Telephone), fiche.Email}.
                                                       Where(Function(t) Not String.IsNullOrWhiteSpace(t)))
        If coordonnees.Length > 0 Then lignes.Add(coordonnees)

        For Each contact As FamilleContact In apercu.Contacts.OrderBy(Function(c) c.OrdreAffichage)
            Dim nom As String = $"{contact.Prenom} {If(contact.Nom, String.Empty).ToUpperInvariant()}".Trim()
            Dim lien As String = If(String.IsNullOrWhiteSpace(contact.LibelleLienPatient), "Contact", contact.LibelleLienPatient)
            Dim tel As String = Telephone(contact.Telephone)
            lignes.Add($"{lien} : {nom}" & If(String.IsNullOrWhiteSpace(tel), String.Empty, $" · {tel}"))
        Next

        Return If(lignes.Count = 0, "Aucune coordonnée.", String.Join(Environment.NewLine, lignes))

    End Function

    ' Numéro mis en forme comme dans les grilles (UtilsTelephone), vide s'il n'y en a pas.
    Private Shared Function Telephone(numero As String) As String
        Return If(String.IsNullOrWhiteSpace(numero), String.Empty, UtilsTelephone.FormaterAffichage(numero, Nothing))
    End Function

#End Region

#Region "Mise en page"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AjusterLargeurs
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Fait suivre aux sections la largeur du panneau (moins l'ascenseur éventuel) : un
    ' FlowLayoutPanel vertical ne les étire pas de lui-même.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjusterLargeurs()

        Dim largeur As Integer = flpContenu.ClientSize.Width - flpContenu.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth
        If largeur < 120 Then Return

        flpContenu.SuspendLayout()
        For Each controle As Control In flpContenu.Controls
            If controle.AutoSize Then
                controle.MaximumSize = New Size(largeur, 0)
                controle.MinimumSize = New Size(If(controle Is lblAlerte OrElse controle Is flpBoutons, largeur, 0), 0)
            ElseIf controle IsNot btnNotesSeance Then
                controle.Width = largeur
            End If
        Next
        lblNom.Width = Math.Max(40, largeur - lblNom.Left)
        lblInfos.Width = lblNom.Width
        flpContenu.ResumeLayout(True)

    End Sub

    Private Sub flpContenu_Resize(sender As Object, e As EventArgs) Handles flpContenu.Resize
        AjusterLargeurs()
    End Sub

    ' Sans photo, les initiales du patient sont dessinées (comme dans le récapitulatif du dossier).
    Private Sub picPhoto_Paint(sender As Object, e As PaintEventArgs) Handles picPhoto.Paint

        If picPhoto.Image IsNot Nothing OrElse _apercu Is Nothing Then Return

        Dim fiche As Patient = _apercu.Patient
        Dim initiales As String = (Initiale(fiche.Prenom) & Initiale(fiche.Nom)).ToUpperInvariant()
        If initiales.Length = 0 Then Return

        Using police As New Font("Calibri", 15.0F, FontStyle.Bold)
            TextRenderer.DrawText(e.Graphics, initiales, police, picPhoto.ClientRectangle, UITheme.ColorMauveFonce,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
        End Using

    End Sub

    Private Shared Function Initiale(texte As String) As String
        Return If(String.IsNullOrWhiteSpace(texte), String.Empty, texte.Trim().Substring(0, 1))
    End Function

#End Region

#Region "Grille des séances"

    ' Pastille « État » : statut de paiement de la séance, ou « Absence non excusée ».
    Private Sub dgvSeancesApercu_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) _
        Handles dgvSeancesApercu.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colApEtat.Index Then Return

        Dim seance As SeanceListeItem = TryCast(dgvSeancesApercu.Rows(e.RowIndex).Tag, SeanceListeItem)
        If seance Is Nothing Then Return

        If seance.EstNoShow Then
            UtilsDataGrid.DessinerPastilleCellule(dgvSeancesApercu, e, "Absence", UtilsPastilles.Teinte(FamilleStatutSeance.AbsenceNonExcusee))
        Else
            UtilsDataGrid.DessinerPastilleCellule(dgvSeancesApercu, e, seance.PaiementAffichage, UtilsPastilles.Teinte(seance.FamillePaiement))
        End If

    End Sub

    ' Date à venir en bleu « à venir », date passée en gris.
    Private Sub dgvSeancesApercu_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) _
        Handles dgvSeancesApercu.CellFormatting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colApDate.Index Then Return

        Dim seance As SeanceListeItem = TryCast(dgvSeancesApercu.Rows(e.RowIndex).Tag, SeanceListeItem)
        If seance Is Nothing Then Return

        e.CellStyle.ForeColor = If(seance.EstPassee, UITheme.ColorGrisDiscret, UITheme.StatutPrevisionnelFore)
        If Not seance.EstPassee Then e.CellStyle.Font = PoliceDateAVenir

    End Sub

    Private Sub dgvSeancesApercu_CellToolTipTextNeeded(sender As Object, e As DataGridViewCellToolTipTextNeededEventArgs) _
        Handles dgvSeancesApercu.CellToolTipTextNeeded

        If e.RowIndex < 0 OrElse _apercu Is Nothing Then Return
        Dim seance As SeanceListeItem = TryCast(dgvSeancesApercu.Rows(e.RowIndex).Tag, SeanceListeItem)
        If seance Is Nothing Then Return

        e.ToolTipText = $"{seance.CodeSeance} · {_apercu.CodeDossier(seance.IdDossier)}{Environment.NewLine}Double-clic : ouvrir la séance"

    End Sub

    Private Sub dgvSeancesApercu_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) _
        Handles dgvSeancesApercu.CellDoubleClick

        If e.RowIndex < 0 Then Return
        Dim seance As SeanceListeItem = TryCast(dgvSeancesApercu.Rows(e.RowIndex).Tag, SeanceListeItem)
        If seance IsNot Nothing Then DemanderSeance(seance)

    End Sub

#End Region

#Region "Actions"

    Private Sub btnOuvrirFiche_Click(sender As Object, e As EventArgs) Handles btnOuvrirFiche.Click
        If _apercu IsNot Nothing Then RaiseEvent OuvrirFicheDemandee(Me, EventArgs.Empty)
    End Sub

    Private Sub btnOuvrirDossier_Click(sender As Object, e As EventArgs) Handles btnOuvrirDossier.Click
        If _apercu?.DossierCourant IsNot Nothing Then RaiseEvent OuvrirDossierDemande(_apercu.DossierCourant.IdDossier, _apercu.DossierCourant.CodeDossier)
    End Sub

    Private Sub btnNotesSeance_Click(sender As Object, e As EventArgs) Handles btnNotesSeance.Click
        If _seanceDuJour IsNot Nothing Then DemanderSeance(_seanceDuJour)
    End Sub

    Private Sub DemanderSeance(seance As SeanceListeItem)
        RaiseEvent OuvrirSeanceDemandee(New RepereSeance With {
            .IdSeance = seance.IdSeance,
            .CodeSeance = seance.CodeSeance,
            .IdDossier = seance.IdDossier
        })
    End Sub

#End Region

End Class
