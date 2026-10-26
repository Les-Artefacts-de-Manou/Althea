' -------------------------------------------------------------------------------------------------
' UserControl : UC_Sauvegardes
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran des sauvegardes (lot 30, étape P2), ouvert depuis la tuile « Sauvegardes » du hub
' Administration : lancer une sauvegarde complète (base + dossier de données) dans une archive
' chiffrée, choisir le dossier des archives, gérer le mot de passe, consulter l'historique.
'
' Responsabilités :
' - Afficher le dossier des sauvegardes, la dernière sauvegarde réussie, l'état du mot de passe
' - Lancer la sauvegarde en arrière-plan avec sa progression (jauge + étape)
' - Bloquer la navigation et la fermeture de l'application pendant une sauvegarde
'   (IUnsavedChangesAware)
' - Définir / vérifier le mot de passe des sauvegardes (définition réservée à l'Admin) : mot de
'   passe PROPRE aux archives, commun au cabinet, distinct des mots de passe de connexion ;
'   œil « maintenir pour voir », règles affichées
' - Afficher le journal tec_sauvegardes
' - Ouvrir la restauration (DialogRestauration) et redémarrer l'application si la base a été
'   rechargée
'
' Droits (décision du 12/10/2026) :
' - Sauvegarder, choisir le dossier : SuperUser et Admin (élévation comprise)
' - Définir le mot de passe et restaurer : Admin
' - Contrôlés aussi dans GestionSauvegardes / GestionRestaurations (l'écran n'est pas la seule barrière)
'
' Remarques   :
' - Aucune logique de sauvegarde ici : tout passe par GestionSauvegardes.
' - Le mot de passe saisi n'est jamais conservé dans l'écran (zones vidées après usage).
' - V1.2.0 (18/10/2026, UI-9.3) : historique — résultat en pastille (UtilsPastilles.Teinte
'   (StatutSauvegarde)) au lieu de la ligne entière en rouge ; cellules typées (date, taille,
'   nombre) pour un tri juste par en-tête ; date centrée, taille et nombre à droite.
'
' Dépendances :
' - GestionSauvegardes, MotDePasseSauvegarde, UserControlContext, UtilsButtons, UtilsDataGrid,
'   UtilsPastilles, DialogChoix, DialogRestauration, GestionLog
' - V1.3.0 (22/10/2026, audit C.1) : les opérations sensibles reçoivent le ContexteOperation de l'écran (utilisateur + rôle
'   courant), vérifié par le métier (Autorisation).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Class UC_Sauvegardes
    Implements IContextAwareUserControl
    Implements IUnsavedChangesAware

#Region "Variables"

    ' Contexte UI partagé fourni par Home.
    Private _context As UserControlContext

    ' Vrai pendant une sauvegarde : boutons désactivés, navigation et fermeture bloquées.
    Private _sauvegardeEnCours As Boolean = False

    ' Nombre de lignes d'historique affichées.
    Private Const NbLignesHistorique As Integer = 50

#End Region

#Region "Contexte UI"

    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext
        _context = context
    End Sub

#End Region

#Region "Chargement"

    Private Sub UC_Sauvegardes_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        UtilsButtons.InitStandardButton(btnSauvegarder)
        UtilsButtons.InitStandardButton(btnActualiser)
        UtilsButtons.InitStandardButton(btnRestaurer)
        UtilsButtons.InitStandardButton(btnChoisirDossier)
        UtilsButtons.InitStandardButton(btnDefinirMotDePasse)
        UtilsButtons.InitStandardButton(btnVerifierMotDePasse)
        UtilsButtons.InitStandardButton(btnVoirMotDePasse)
        UtilsButtons.InitStandardButton(btnVoirVerification)

        lblReglesMotDePasse.Text = GestionSauvegardes.DescriptionReglesMotDePasse

        UtilsDataGrid.InitDataGridBasique(dgvHistorique)
        colDate.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"
        UtilsDataGrid.AlignerCentre(colDate)
        UtilsDataGrid.AlignerDroite(colTaille, colNbFichiers)

        InitialiserToolTips()

        If _context IsNot Nothing Then
            _context.SetHeader("Administration > Sauvegardes")
        End If

        Rafraichir()

    End Sub

    Private Sub InitialiserToolTips()
        If _context Is Nothing Then Return
        _context.SetToolTip(btnSauvegarder, "Sauvegarder maintenant la base et le dossier de données dans une archive chiffrée.")
        _context.SetToolTip(btnActualiser, "Relire l'état et l'historique des sauvegardes.")
        _context.SetToolTip(btnRestaurer, "Remplacer la base et les documents par ceux d'une archive (administrateur).")
        _context.SetToolTip(btnChoisirDossier, "Choisir le dossier des archives (hors du dossier de données, idéalement un disque externe).")
        _context.SetToolTip(btnDefinirMotDePasse, "Enregistrer le mot de passe des sauvegardes sur ce poste (administrateur).")
        _context.SetToolTip(btnVerifierMotDePasse, "Vérifier qu'un mot de passe est bien celui des sauvegardes.")
        _context.SetToolTip(btnVoirMotDePasse, "Maintenir appuyé pour voir le mot de passe et sa confirmation.")
        _context.SetToolTip(btnVoirVerification, "Maintenir appuyé pour voir le mot de passe saisi.")
    End Sub

#End Region

#Region "Affichage"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Rafraichir
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Relit et affiche le dossier, la dernière sauvegarde réussie, l'état du mot de passe et
    ' l'historique ; met à jour les boutons.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Rafraichir()

        Try
            Dim dossier As String = GestionSauvegardes.GetDossierSauvegarde()
            lblDossier.Text = If(dossier.Length = 0, "Pas encore choisi", dossier)

            Dim derniere As DateTime? = GestionSauvegardes.GetDateDerniereReussie()
            lblDerniere.Text = If(derniere.HasValue,
                                  $"{derniere.Value:dd/MM/yyyy HH:mm} ({DecrireAnciennete(derniere.Value)})",
                                  "Aucune")

            AfficherEtatMotDePasse()
            ChargerHistorique()

        Catch ex As Exception
            GestionLog.EcrireLog("UI: lecture de l'état des sauvegardes impossible.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            _context?.SetStatus("Lecture des sauvegardes impossible. Consultez les logs.")
        End Try

        MettreAJourEtatBoutons()

    End Sub

    Private Sub AfficherEtatMotDePasse()
        Try
            If MotDePasseSauvegarde.EstDefini() Then
                Dim dateDefinition As DateTime? = MotDePasseSauvegarde.DateDefinition()
                lblEtatMotDePasse.Text = "Défini sur ce poste" &
                    If(dateDefinition.HasValue, $" (le {dateDefinition.Value:dd/MM/yyyy})", String.Empty)
            Else
                lblEtatMotDePasse.Text = "Non défini : aucune sauvegarde possible tant qu'il ne l'est pas."
            End If
        Catch ex As Exception
            ' Fichier présent mais illisible pour ce compte Windows (DPAPI) : à redéfinir.
            GestionLog.EcrireLog("UI: mot de passe des sauvegardes illisible sur ce compte Windows.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
            lblEtatMotDePasse.Text = "Illisible sur ce compte Windows : l'administrateur doit le redéfinir."
        End Try
    End Sub

    Private Sub ChargerHistorique()

        dgvHistorique.Rows.Clear()

        For Each ligne In GestionSauvegardes.GetHistorique(NbLignesHistorique)

            Dim detail As String
            Select Case ligne.Statut
                Case StatutSauvegarde.Reussie : detail = ligne.DossierSauvegarde
                Case StatutSauvegarde.Echec : detail = ligne.MessageErreur
                Case Else : detail = "Sauvegarde interrompue (application fermée ou arrêtée pendant la sauvegarde)."
            End Select

            ' Valeurs typées (et non du texte déjà mis en forme) : le tri par en-tête de la grille
            ' compare alors des dates et des nombres. La taille est mise en forme à l'affichage.
            Dim index As Integer = dgvHistorique.Rows.Add(
                ligne.DateDebut,
                ligne.LibelleStatut,
                ligne.NomFichier,
                If(ligne.TailleOctets.HasValue, CObj(ligne.TailleOctets.Value), Nothing),
                If(ligne.NbFichiers.HasValue, CObj(ligne.NbFichiers.Value), Nothing),
                ligne.LoginUtilisateur,
                detail)
            dgvHistorique.Rows(index).Tag = ligne

        Next

    End Sub

    Private Sub dgvHistorique_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvHistorique.CellFormatting
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colTaille.Index OrElse Not TypeOf e.Value Is Long Then Return
        e.Value = FormaterTaille(CLng(e.Value))
        e.FormattingApplied = True
    End Sub

    Private Sub dgvHistorique_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvHistorique.CellPainting
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colStatut.Index Then Return
        Dim ligne As SauvegardeJournal = TryCast(dgvHistorique.Rows(e.RowIndex).Tag, SauvegardeJournal)
        If ligne Is Nothing Then Return
        UtilsDataGrid.DessinerPastilleCellule(dgvHistorique, e, ligne.LibelleStatut, UtilsPastilles.Teinte(ligne.Statut))
    End Sub

    Private Sub MettreAJourEtatBoutons()

        Dim role As AppRole = RoleCourant()
        Dim estAdmin As Boolean = role = AppRole.Admin
        Dim peutSauvegarder As Boolean = role >= AppRole.SuperUser
        Dim libre As Boolean = Not _sauvegardeEnCours

        btnSauvegarder.Enabled = peutSauvegarder AndAlso libre
        btnActualiser.Enabled = libre
        btnRestaurer.Enabled = estAdmin AndAlso libre
        btnChoisirDossier.Enabled = peutSauvegarder AndAlso libre

        ' Définir : administrateur ; vérifier : dès qu'un mot de passe existe sur ce poste.
        Dim defini As Boolean = MotDePasseDefiniSansErreur()
        txtMotDePasse.Enabled = estAdmin AndAlso libre
        txtConfirmation.Enabled = estAdmin AndAlso libre
        btnVoirMotDePasse.Enabled = estAdmin AndAlso libre
        btnDefinirMotDePasse.Enabled = estAdmin AndAlso libre
        txtVerification.Enabled = defini AndAlso libre
        btnVoirVerification.Enabled = defini AndAlso libre
        btnVerifierMotDePasse.Enabled = defini AndAlso libre

    End Sub

    Private Function MotDePasseDefiniSansErreur() As Boolean
        Try
            Return MotDePasseSauvegarde.EstDefini()
        Catch
            Return False
        End Try
    End Function

    ' Rôle effectif (élévation comprise).
    ' Rôle courant de la session (règle unique : UserControlContext.RoleCourant).
    Private Function RoleCourant() As AppRole
        Return If(_context Is Nothing, AppRole.User, _context.RoleCourant)
    End Function

    Private Shared Function DecrireAnciennete(dateSauvegarde As DateTime) As String
        Dim jours As Integer = CInt(Math.Floor((DateTime.Now - dateSauvegarde).TotalDays))
        If jours <= 0 Then Return "aujourd'hui"
        If jours = 1 Then Return "hier"
        Return $"il y a {jours} jours"
    End Function

    Private Shared Function FormaterTaille(octets As Long) As String
        If octets < 1024 Then Return $"{octets} o"
        If octets < 1024L * 1024 Then Return $"{octets / 1024.0:0.#} Ko"
        If octets < 1024L * 1024 * 1024 Then Return $"{octets / (1024.0 * 1024):0.#} Mo"
        Return $"{octets / (1024.0 * 1024 * 1024):0.##} Go"
    End Function

#End Region

#Region "Sauvegarde"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnSauvegarder_Click
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Lance la sauvegarde en arrière-plan ; la jauge et l'étape suivent sa progression.
    '
    ' Remarques  :
    ' - Progress(Of T) est créé sur le fil de l'écran : les mises à jour y reviennent seules.
    ' - Pendant la sauvegarde, PeutQuitter bloque la navigation et la fermeture.
    ' -------------------------------------------------------------------------------------------------
    Private Async Sub btnSauvegarder_Click(sender As Object, e As EventArgs) Handles btnSauvegarder.Click

        If _sauvegardeEnCours Then Return

        Dim contexte As ContexteOperation = _context?.ContexteOperation
        Dim progression As New Progress(Of ProgressionSauvegarde)(AddressOf AfficherProgression)
        Dim resultat As ResultatSauvegarde

        _sauvegardeEnCours = True
        MettreAJourEtatBoutons()
        jaugeProgression.Valeur = 0
        jaugeProgression.Visible = True
        lblEtape.Text = "Préparation…"
        _context?.SetStatus("Sauvegarde en cours…")

        Try
            resultat = Await Task.Run(Function() GestionSauvegardes.Sauvegarder(contexte, progression))
        Catch ex As UnauthorizedAccessException
            resultat = New ResultatSauvegarde With {.Reussie = False, .Message = ex.Message}
        Catch ex As Exception
            GestionLog.EcrireLog("UI: erreur inattendue pendant la sauvegarde.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            resultat = New ResultatSauvegarde With {.Reussie = False, .Message = "La sauvegarde a échoué. Consultez les logs."}
        Finally
            _sauvegardeEnCours = False
        End Try

        If IsDisposed Then Return

        jaugeProgression.Visible = False

        If resultat.Reussie Then
            lblEtape.Text = $"Dernière archive : {Path.GetFileName(resultat.CheminArchive)} " &
                            $"({FormaterTaille(resultat.TailleOctets)}, {resultat.NbFichiers} fichiers)."
            _context?.SetStatus("Sauvegarde terminée.")
            DialogChoix.Succes(
                "Sauvegarde terminée." & Environment.NewLine & Environment.NewLine &
                $"Archive : {resultat.CheminArchive}" & Environment.NewLine &
                $"Taille : {FormaterTaille(resultat.TailleOctets)}" & Environment.NewLine &
                $"Fichiers du dossier de données : {resultat.NbFichiers}" &
                If(resultat.NbArchivesSupprimees > 0,
                   Environment.NewLine & $"Anciennes archives supprimées : {resultat.NbArchivesSupprimees}",
                   String.Empty),
                "Althéa - Sauvegardes")
        Else
            lblEtape.Text = String.Empty
            _context?.SetStatus("Sauvegarde non effectuée.")
            DialogChoix.Avertissement(resultat.Message, "Althéa - Sauvegardes")
        End If

        Rafraichir()

    End Sub

    Private Sub AfficherProgression(etat As ProgressionSauvegarde)
        If IsDisposed OrElse etat Is Nothing Then Return
        jaugeProgression.Valeur = etat.Pourcentage
        lblEtape.Text = etat.Etape
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnRestaurer_Click
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Ouvre la restauration (administrateur) ; si la base a été rechargée, redémarre l'application :
    ' la session en cours ne correspond plus à la base (les comptes peuvent avoir changé).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRestaurer_Click(sender As Object, e As EventArgs) Handles btnRestaurer.Click

        If _sauvegardeEnCours Then Return

        Dim redemarrer As Boolean
        Using dlg As New DialogRestauration(_context?.ContexteOperation)
            dlg.ShowDialog(FindForm())
            redemarrer = dlg.RedemarrageNecessaire
        End Using

        If redemarrer Then
            GestionLog.EcrireLog("Redémarrage de l'application après restauration.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.Process)
            Application.Restart()
            Return
        End If

        Rafraichir()

    End Sub

    Private Sub btnActualiser_Click(sender As Object, e As EventArgs) Handles btnActualiser.Click
        Rafraichir()
        _context?.SetStatus("Sauvegardes actualisées.")
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnChoisirDossier_Click
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Choix du dossier des archives ; refusé dans le dossier de données (contrôle du service).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnChoisirDossier_Click(sender As Object, e As EventArgs) Handles btnChoisirDossier.Click

        Using dlg As New FolderBrowserDialog With {
            .Description = "Dossier des sauvegardes (hors du dossier de données, idéalement un disque externe)",
            .UseDescriptionForTitle = True,
            .ShowNewFolderButton = True
        }
            Dim actuel As String = GestionSauvegardes.GetDossierSauvegarde()
            If Directory.Exists(actuel) Then dlg.InitialDirectory = actuel
            If dlg.ShowDialog(FindForm()) <> DialogResult.OK Then Return

            Try
                Dim refus As String = GestionSauvegardes.DefinirDossierSauvegarde(_context?.ContexteOperation, dlg.SelectedPath)
                If refus IsNot Nothing Then
                    DialogChoix.Avertissement(refus, "Althéa - Sauvegardes")
                    Return
                End If
                _context?.SetStatus("Dossier des sauvegardes enregistré.")
            Catch ex As UnauthorizedAccessException
                DialogChoix.Avertissement(ex.Message, "Accès refusé")
            Catch ex As Exception
                GestionLog.EcrireLog("UI: enregistrement du dossier des sauvegardes impossible.",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
                DialogChoix.Erreur("Le dossier n'a pas pu être enregistré. Consultez les logs.", "Althéa - Sauvegardes")
            End Try
        End Using

        Rafraichir()

    End Sub

#End Region

#Region "Mot de passe"

    Private Sub btnDefinirMotDePasse_Click(sender As Object, e As EventArgs) Handles btnDefinirMotDePasse.Click

        Try
            If MotDePasseDefiniSansErreur() AndAlso
               DialogChoix.Confirmer("Un mot de passe est déjà défini." & Environment.NewLine &
                                     "Les archives déjà faites resteront protégées par l'ancien : gardez-le aussi." &
                                     Environment.NewLine & Environment.NewLine & "Remplacer le mot de passe ?",
                                     "Althéa - Sauvegardes") <> DialogResult.Yes Then
                Return
            End If

            GestionSauvegardes.DefinirMotDePasse(_context?.ContexteOperation, txtMotDePasse.Text, txtConfirmation.Text)

            DialogChoix.Succes("Mot de passe des sauvegardes enregistré sur ce poste." & Environment.NewLine & Environment.NewLine &
                               "Notez-le et gardez-le en lieu sûr : il sera demandé pour restaurer une sauvegarde " &
                               "sur un autre poste ou après une réinstallation.",
                               "Althéa - Sauvegardes")

        Catch ex As UnauthorizedAccessException
            DialogChoix.Avertissement(ex.Message, "Accès refusé")
        Catch ex As ArgumentException
            DialogChoix.Avertissement(ex.Message, "Althéa - Validation")
        Catch ex As Exception
            GestionLog.EcrireLog("UI: enregistrement du mot de passe des sauvegardes impossible.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
            DialogChoix.Erreur("Le mot de passe n'a pas pu être enregistré. Consultez les logs.", "Althéa - Sauvegardes")
        Finally
            ViderSaisiesMotDePasse()
            AfficherEtatMotDePasse()
            MettreAJourEtatBoutons()
        End Try

    End Sub

    Private Sub btnVerifierMotDePasse_Click(sender As Object, e As EventArgs) Handles btnVerifierMotDePasse.Click

        Try
            If GestionSauvegardes.VerifierMotDePasse(txtVerification.Text) Then
                DialogChoix.Succes("C'est bien le mot de passe des sauvegardes.", "Althéa - Sauvegardes")
            Else
                DialogChoix.Avertissement("Ce n'est pas le mot de passe des sauvegardes.", "Althéa - Sauvegardes")
            End If
        Catch ex As Exception
            GestionLog.EcrireLog("UI: vérification du mot de passe des sauvegardes impossible.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
            DialogChoix.Erreur("La vérification a échoué. Consultez les logs.", "Althéa - Sauvegardes")
        Finally
            ViderSaisiesMotDePasse()
        End Try

    End Sub

    Private Sub ViderSaisiesMotDePasse()
        txtMotDePasse.Clear()
        txtConfirmation.Clear()
        txtVerification.Clear()
    End Sub

    ' Œil « maintenir pour voir », comme à la connexion : le mot de passe n'apparaît en clair que
    ' tant que le bouton est enfoncé.
    Private Sub btnVoirMotDePasse_MouseDown(sender As Object, e As MouseEventArgs) Handles btnVoirMotDePasse.MouseDown
        AfficherEnClair(True, txtMotDePasse, txtConfirmation)
    End Sub

    Private Sub btnVoirMotDePasse_Relache(sender As Object, e As EventArgs) Handles btnVoirMotDePasse.MouseUp, btnVoirMotDePasse.MouseLeave
        AfficherEnClair(False, txtMotDePasse, txtConfirmation)
    End Sub

    Private Sub btnVoirVerification_MouseDown(sender As Object, e As MouseEventArgs) Handles btnVoirVerification.MouseDown
        AfficherEnClair(True, txtVerification)
    End Sub

    Private Sub btnVoirVerification_Relache(sender As Object, e As EventArgs) Handles btnVoirVerification.MouseUp, btnVoirVerification.MouseLeave
        AfficherEnClair(False, txtVerification)
    End Sub

    Private Shared Sub AfficherEnClair(enClair As Boolean, ParamArray zones As TextBox())
        For Each zone In zones
            zone.UseSystemPasswordChar = Not enClair
        Next
    End Sub

#End Region

#Region "Garde de navigation"

    ' Pas de navigation ni de fermeture pendant une sauvegarde : l'archive serait abandonnée.
    Public Function PeutQuitter() As Boolean Implements IUnsavedChangesAware.PeutQuitter

        If Not _sauvegardeEnCours Then Return True

        DialogChoix.Avertissement("Une sauvegarde est en cours." & Environment.NewLine &
                                  "Attendez qu'elle soit terminée avant de quitter cet écran ou l'application.",
                                  "Althéa - Sauvegardes")
        Return False

    End Function

#End Region

End Class
