' -------------------------------------------------------------------------------------------------
' Formulaire  : DialogRestauration
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Restauration d'une archive de sauvegarde (lot 30, étape P3), ouverte depuis l'écran Sauvegardes
' par l'administrateur.
'
' Responsabilités :
' - Choisir l'archive (dossier des sauvegardes ou « Parcourir… »), le mot de passe de l'archive
'   (celui du poste, ou un autre) et le compte d'administration MariaDB
' - « Vérifier l'archive » : manifeste et contrôles, sans rien modifier
' - « Restaurer » : confirmation, opération en arrière-plan avec jauge, compte rendu
' - Afficher les dernières restaurations
'
' Important   :
' - Disposition et apparence portées par DialogRestauration.Designer.vb (Designer obligatoire).
' - Schéma des fenêtres du kit (UI-7, 15/10/2026) : cartes Archive à restaurer, Compte
'   d'administration MariaDB, Vérification, Dernières restaurations ; fond AltheaFondFenetre.
' - Toute la logique est dans GestionRestaurations ; l'écran ne fait que la piloter.
' - Une modification des choix (archive, mot de passe, compte) annule la vérification : on ne
'   restaure jamais une archive qui n'a pas été vérifiée telle quelle.
' - Pendant l'opération, la fenêtre ne peut pas être fermée.
' - Après une restauration (ou une remise en état de la base), RedemarrageNecessaire = True :
'   l'appelant redémarre l'application.
' - Les mots de passe saisis sont effacés des zones dès qu'ils ne servent plus.
' - V1.3.0 (18/10/2026, UI-9.3) : dernières restaurations — résultat en pastille
'   (UtilsPastilles.Teinte(StatutRestauration)) au lieu de la ligne en rouge ; date typée (tri juste)
'   et centrée.
'
' Dépendances :
' - GestionRestaurations, GestionSauvegardes, MotDePasseSauvegarde, UtilsButtons, UtilsDataGrid,
'   UtilsPastilles, DialogChoix, GestionLog
' - V1.4.0 (22/10/2026, audit C.1) : les opérations sensibles reçoivent le ContexteOperation de l'écran (utilisateur + rôle
'   courant), vérifié par le métier (Autorisation).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Class DialogRestauration

#Region "Champs"

    ' Qui restaure (utilisateur + rôle courant), transmis au métier (ContexteOperation, audit C.1).
    Private ReadOnly _contexte As ContexteOperation

    ' Archive vérifiée avec succès (chemin) ; Nothing tant qu'aucune vérification n'a abouti.
    Private _archiveVerifiee As String
    Private _infoArchive As InfoArchive
    Private _operationEnCours As Boolean = False
    Private _redemarrageNecessaire As Boolean = False

    ' Nombre de restaurations affichées.
    Private Const NbLignesHistorique As Integer = 10

    ' Élément de la liste des archives : chemin complet, libellé lisible.
    Private NotInheritable Class ElementArchive
        Public Property Chemin As String
        Public Property Libelle As String
        Public Overrides Function ToString() As String
            Return Libelle
        End Function
    End Class

#End Region

#Region "Propriétés publiques"

    ''' <summary>True si la base a été rechargée : l'application doit redémarrer.</summary>
    Public ReadOnly Property RedemarrageNecessaire As Boolean
        Get
            Return _redemarrageNecessaire
        End Get
    End Property

#End Region

#Region "Constructeur et chargement"

    Public Sub New(contexte As ContexteOperation)
        InitializeComponent()
        _contexte = contexte
    End Sub

    Private Sub DialogRestauration_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Fond beige à la feuille et barre de titre aux couleurs du kit.
        AltheaFondFenetre.Appliquer(Me)

        ' Les boutons « œil », icône seule, gardent l'image du Designer (règle des petits boutons du kit).
        For Each bouton In {btnRestaurer, btnFermer, btnParcourir, btnVerifier}
            UtilsButtons.InitStandardButton(bouton)
        Next
        UtilsDataGrid.InitDataGridBasique(dgvHistorique)
        colDate.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"
        UtilsDataGrid.AlignerCentre(colDate)

        txtCompteAdmin.Text = GestionRestaurations.CompteAdministrationParDefaut

        ' Sans mot de passe lisible sur ce poste, la saisie manuelle est la seule possibilité.
        If Not MotDePasseLisible() Then
            chkAutreMotDePasse.Checked = True
            chkAutreMotDePasse.Enabled = False
        End If

        ChargerArchives(Nothing)
        ChargerHistorique()
        MettreAJourEtat()

    End Sub

    Private Shared Function MotDePasseLisible() As Boolean
        Try
            Return Not String.IsNullOrEmpty(MotDePasseSauvegarde.Obtenir())
        Catch
            Return False
        End Try
    End Function

    Private Sub ChargerArchives(cheminASelectionner As String)

        cboArchives.Items.Clear()

        Try
            For Each chemin In GestionRestaurations.ListerArchives()
                cboArchives.Items.Add(New ElementArchive With {.Chemin = chemin, .Libelle = LibelleArchive(chemin)})
            Next
        Catch ex As Exception
            GestionLog.EcrireLog("UI: liste des archives illisible.", GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
        End Try

        If cheminASelectionner IsNot Nothing Then
            Dim existant = cboArchives.Items.Cast(Of ElementArchive)() _
                .FirstOrDefault(Function(a) String.Equals(a.Chemin, cheminASelectionner, StringComparison.OrdinalIgnoreCase))
            If existant Is Nothing Then
                existant = New ElementArchive With {.Chemin = cheminASelectionner, .Libelle = LibelleArchive(cheminASelectionner)}
                cboArchives.Items.Insert(0, existant)
            End If
            cboArchives.SelectedItem = existant
        ElseIf cboArchives.Items.Count > 0 Then
            cboArchives.SelectedIndex = 0
        End If

    End Sub

    ' « Althea_20261012_174512.zip » → « 12/10/2026 17:45 — Althea_20261012_174512.zip ».
    Private Shared Function LibelleArchive(chemin As String) As String
        Dim nom As String = Path.GetFileName(chemin)
        Dim dateNom As DateTime
        If nom.Length >= 22 AndAlso DateTime.TryParseExact(nom.Substring(7, 15), "yyyyMMdd_HHmmss",
                                                           Globalization.CultureInfo.InvariantCulture,
                                                           Globalization.DateTimeStyles.None, dateNom) Then
            Return $"{dateNom:dd/MM/yyyy HH:mm} — {nom}"
        End If
        Return nom
    End Function

    Private Sub ChargerHistorique()

        dgvHistorique.Rows.Clear()

        Try
            For Each ligne In GestionRestaurations.GetHistorique(NbLignesHistorique)
                Dim detail As String = If(ligne.Statut = StatutRestauration.Reussie,
                                          $"Sauvegarde de sécurité : {ligne.ArchiveSecurite} ; ancien dossier : {ligne.DossierDonneesPrecedent}",
                                          ligne.MessageErreur)
                ' Date typée (et non du texte) : le tri par en-tête compare des dates.
                Dim index As Integer = dgvHistorique.Rows.Add(
                    ligne.DateDebut,
                    ligne.LibelleStatut,
                    ligne.NomArchive,
                    ligne.LoginUtilisateur,
                    detail)
                dgvHistorique.Rows(index).Tag = ligne
            Next
        Catch ex As Exception
            GestionLog.EcrireLog("UI: historique des restaurations illisible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
        End Try

    End Sub

    Private Sub dgvHistorique_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvHistorique.CellPainting
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colStatut.Index Then Return
        Dim ligne As RestaurationJournal = TryCast(dgvHistorique.Rows(e.RowIndex).Tag, RestaurationJournal)
        If ligne Is Nothing Then Return
        UtilsDataGrid.DessinerPastilleCellule(dgvHistorique, e, ligne.LibelleStatut, UtilsPastilles.Teinte(ligne.Statut))
    End Sub

#End Region

#Region "État de l'écran"

    Private Sub MettreAJourEtat()

        Dim libre As Boolean = Not _operationEnCours
        Dim estAdmin As Boolean = Autorisation.Possede(_contexte, AppRole.Admin)

        cboArchives.Enabled = libre
        btnParcourir.Enabled = libre
        chkAutreMotDePasse.Enabled = libre AndAlso MotDePasseLisible()
        txtMotDePasseArchive.Enabled = libre AndAlso chkAutreMotDePasse.Checked
        btnVoirArchive.Enabled = txtMotDePasseArchive.Enabled
        txtCompteAdmin.Enabled = libre
        txtMotDePasseAdmin.Enabled = libre
        btnVoirAdmin.Enabled = libre
        btnVerifier.Enabled = libre AndAlso estAdmin AndAlso cboArchives.SelectedItem IsNot Nothing
        btnRestaurer.Enabled = libre AndAlso estAdmin AndAlso _archiveVerifiee IsNot Nothing
        btnFermer.Enabled = libre

    End Sub

    ' Toute modification d'un choix annule la vérification précédente.
    Private Sub AnnulerVerification(sender As Object, e As EventArgs) Handles cboArchives.SelectedIndexChanged,
        chkAutreMotDePasse.CheckedChanged, txtMotDePasseArchive.TextChanged, txtCompteAdmin.TextChanged,
        txtMotDePasseAdmin.TextChanged

        If _operationEnCours Then Return
        If _archiveVerifiee IsNot Nothing Then
            _archiveVerifiee = Nothing
            _infoArchive = Nothing
            lblResume.Text = "Choix modifié : vérifiez à nouveau l'archive avant de restaurer."
            AfficherVerdict(Nothing)
        End If
        MettreAJourEtat()

    End Sub

    Private Function ArchiveChoisie() As String
        Return TryCast(cboArchives.SelectedItem, ElementArchive)?.Chemin
    End Function

    Private Function MotDePasseArchive() As String
        If chkAutreMotDePasse.Checked Then Return txtMotDePasseArchive.Text
        Return MotDePasseSauvegarde.Obtenir()
    End Function

    Private Sub DemarrerOperation(etape As String)
        _operationEnCours = True
        UseWaitCursor = True
        lblEtape.Text = etape
        MettreAJourEtat()
    End Sub

    Private Sub TerminerOperation()
        _operationEnCours = False
        UseWaitCursor = False
        MettreAJourEtat()
    End Sub

#End Region

#Region "Actions"

    Private Sub btnParcourir_Click(sender As Object, e As EventArgs) Handles btnParcourir.Click

        Using dlg As New OpenFileDialog With {
            .Title = "Archive de sauvegarde à restaurer",
            .Filter = "Sauvegardes Althéa (Althea_*.zip)|Althea_*.zip|Archives ZIP (*.zip)|*.zip",
            .CheckFileExists = True
        }
            Dim dossier As String = GestionSauvegardes.GetDossierSauvegarde()
            If Directory.Exists(dossier) Then dlg.InitialDirectory = dossier
            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return
            ChargerArchives(dlg.FileName)
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVerifier_Click
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Lit le manifeste et lance tous les contrôles (en arrière-plan : la connexion au serveur peut
    ' prendre un moment). Rien n'est modifié.
    ' -------------------------------------------------------------------------------------------------
    Private Async Sub btnVerifier_Click(sender As Object, e As EventArgs) Handles btnVerifier.Click

        Dim chemin As String = ArchiveChoisie()
        If chemin Is Nothing OrElse _operationEnCours Then Return

        Dim motDePasse As String = MotDePasseArchive()
        Dim compte As String = txtCompteAdmin.Text.Trim()
        Dim motDePasseAdmin As String = txtMotDePasseAdmin.Text
        Dim contexte As ContexteOperation = _contexte

        DemarrerOperation("Vérification de l'archive…")
        lblResume.Text = String.Empty
        AfficherVerdict(Nothing)

        Dim verification As ResultatVerification

        Try
            verification = Await Task.Run(Function() GestionRestaurations.VerifierArchive(contexte, chemin, motDePasse,
                                                                                          compte, motDePasseAdmin))
        Catch ex As Exception
            GestionLog.EcrireLog("UI: vérification de l'archive impossible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            verification = New ResultatVerification With {.ErreurLecture = "La vérification a échoué. Consultez les logs."}
        End Try

        If IsDisposed Then Return
        lblEtape.Text = String.Empty
        TerminerOperation()

        If verification.ErreurLecture IsNot Nothing Then
            AfficherVerdict(False)
            lblResume.Text = verification.ErreurLecture
            Return
        End If

        Dim texte As New System.Text.StringBuilder(DecrireArchive(verification.Info))
        If verification.RestaurationPossible Then
            _archiveVerifiee = chemin
            _infoArchive = verification.Info
        Else
            texte.AppendLine()
            For Each motif In verification.Refus
                texte.AppendLine("• " & motif)
            Next
        End If
        AfficherVerdict(verification.RestaurationPossible)
        lblResume.Text = texte.ToString()
        MettreAJourEtat()

    End Sub

    ' Verdict de la vérification, en gros et en couleur : vert si possible, rouge sinon, vide si
    ' aucune vérification valable (couleur d'état dynamique : réglée par le code, pas le Designer).
    Private Sub AfficherVerdict(possible As Boolean?)
        If Not possible.HasValue Then
            lblVerdict.Text = String.Empty
        ElseIf possible.Value Then
            lblVerdict.Text = "✓  Restauration possible"
            lblVerdict.ForeColor = UITheme.ColorSaugeTresFonce
        Else
            lblVerdict.Text = "✗  Restauration impossible"
            lblVerdict.ForeColor = UITheme.DynamicErrorFore
        End If
    End Sub

    Private Shared Function DecrireArchive(info As InfoArchive) As String
        Dim dateTexte As String = If(info.DateSauvegarde.HasValue, info.DateSauvegarde.Value.ToLocalTime().ToString("dd/MM/yyyy à HH:mm"), "date inconnue")
        Return $"Sauvegarde du {dateTexte}, faite sur le poste {info.Machine} par {info.Utilisateur}." & Environment.NewLine &
               $"Version de la base : {info.VersionSchema} — documents : {info.NbFichiers} fichier(s), {FormaterTaille(info.TailleDonneesOctets)}." & Environment.NewLine
    End Function

    Private Shared Function FormaterTaille(octets As Long) As String
        If octets < 1024 Then Return $"{octets} o"
        If octets < 1024L * 1024 Then Return $"{octets / 1024.0:0.#} Ko"
        If octets < 1024L * 1024 * 1024 Then Return $"{octets / (1024.0 * 1024):0.#} Mo"
        Return $"{octets / (1024.0 * 1024 * 1024):0.##} Go"
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnRestaurer_Click
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Confirmation explicite puis restauration en arrière-plan ; compte rendu et, si la base a été
    ' rechargée, fermeture de la fenêtre avec demande de redémarrage.
    ' -------------------------------------------------------------------------------------------------
    Private Async Sub btnRestaurer_Click(sender As Object, e As EventArgs) Handles btnRestaurer.Click

        If _archiveVerifiee Is Nothing OrElse _operationEnCours Then Return

        Dim confirmation As String =
            "Restaurer cette sauvegarde ?" & Environment.NewLine & Environment.NewLine &
            DecrireArchive(_infoArchive) & Environment.NewLine &
            "TOUTES les données actuelles seront remplacées par celles de l'archive." & Environment.NewLine &
            "Une sauvegarde de sécurité de l'état actuel est faite juste avant, et Althéa redémarrera à la fin."
        If DialogChoix.Confirmer(confirmation, "Althéa - Restauration") <> DialogResult.Yes Then Return

        Dim chemin As String = _archiveVerifiee
        Dim motDePasse As String = MotDePasseArchive()
        Dim compte As String = txtCompteAdmin.Text.Trim()
        Dim motDePasseAdmin As String = txtMotDePasseAdmin.Text
        Dim contexte As ContexteOperation = _contexte
        Dim progression As New Progress(Of ProgressionSauvegarde)(AddressOf AfficherProgression)
        Dim resultat As ResultatRestauration

        DemarrerOperation("Préparation…")
        jaugeProgression.Valeur = 0
        jaugeProgression.Visible = True

        Try
            resultat = Await Task.Run(Function() GestionRestaurations.Restaurer(contexte, chemin, motDePasse,
                                                                                compte, motDePasseAdmin, progression))
        Catch ex As UnauthorizedAccessException
            resultat = New ResultatRestauration With {.Reussie = False, .Message = ex.Message}
        Catch ex As Exception
            GestionLog.EcrireLog("UI: erreur inattendue pendant la restauration.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            resultat = New ResultatRestauration With {.Reussie = False, .Message = "La restauration a échoué. Consultez les logs."}
        End Try

        If IsDisposed Then Return

        jaugeProgression.Visible = False
        lblEtape.Text = String.Empty
        txtMotDePasseAdmin.Clear()
        txtMotDePasseArchive.Clear()
        _archiveVerifiee = Nothing
        TerminerOperation()

        If resultat.Reussie Then
            DialogChoix.Succes(
                resultat.Message & Environment.NewLine & Environment.NewLine &
                $"Sauvegarde de sécurité de l'état précédent : {resultat.ArchiveSecurite}" & Environment.NewLine &
                $"Ancien dossier de données, conservé : {resultat.DossierDonneesPrecedent}" & Environment.NewLine &
                "(à supprimer vous-même quand vous aurez vérifié les données restaurées)" & Environment.NewLine & Environment.NewLine &
                "Althéa va maintenant redémarrer.",
                "Althéa - Restauration")
        Else
            DialogChoix.Avertissement(resultat.Message, "Althéa - Restauration")
        End If

        If resultat.RedemarrageNecessaire Then
            _redemarrageNecessaire = True
            DialogResult = DialogResult.OK
            Close()
            Return
        End If

        lblResume.Text = "La restauration n'a pas eu lieu (voir le message). Vous pouvez corriger et vérifier à nouveau."
        AfficherVerdict(Nothing)
        ChargerHistorique()

    End Sub

    Private Sub AfficherProgression(etat As ProgressionSauvegarde)
        If IsDisposed OrElse etat Is Nothing Then Return
        jaugeProgression.Valeur = etat.Pourcentage
        lblEtape.Text = etat.Etape
    End Sub

    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click
        Close()
    End Sub

    ' Pas de fermeture pendant l'opération ; les mots de passe ne restent pas dans les zones.
    Private Sub DialogRestauration_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If _operationEnCours Then
            e.Cancel = True
            DialogChoix.Avertissement("Une opération est en cours : attendez qu'elle soit terminée.", "Althéa - Restauration")
            Return
        End If
        txtMotDePasseAdmin.Clear()
        txtMotDePasseArchive.Clear()
    End Sub

#End Region

#Region "Afficher les mots de passe"

    ' Œil « maintenir pour voir », comme à la connexion.
    Private Sub btnVoirArchive_MouseDown(sender As Object, e As MouseEventArgs) Handles btnVoirArchive.MouseDown
        txtMotDePasseArchive.UseSystemPasswordChar = False
    End Sub

    Private Sub btnVoirArchive_Relache(sender As Object, e As EventArgs) Handles btnVoirArchive.MouseUp, btnVoirArchive.MouseLeave
        txtMotDePasseArchive.UseSystemPasswordChar = True
    End Sub

    Private Sub btnVoirAdmin_MouseDown(sender As Object, e As MouseEventArgs) Handles btnVoirAdmin.MouseDown
        txtMotDePasseAdmin.UseSystemPasswordChar = False
    End Sub

    Private Sub btnVoirAdmin_Relache(sender As Object, e As EventArgs) Handles btnVoirAdmin.MouseUp, btnVoirAdmin.MouseLeave
        txtMotDePasseAdmin.UseSystemPasswordChar = True
    End Sub

#End Region

End Class
