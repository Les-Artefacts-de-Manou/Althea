' -------------------------------------------------------------------------------------------------
' UserControl : UC_EtatSysteme
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Page « État du système » (Administration) : application, base de données, dossiers et espace
' disque, sauvegardes, avec une appréciation par ligne (OK / à vérifier / problème).
'
' Responsabilités :
' - Collecter l'état en arrière-plan (GestionEtatSysteme) et l'afficher par sections
' - Copier le rapport texte dans le presse-papiers (diagnostic à distance)
' - Réaligner les montants en attente des séances quand la section « Cohérence des données » en
'   signale (GestionPaiements.RealignerDusDesalignes, après confirmation)
'
' Droits :
' - Administrateur seulement (tuile du hub ; contrôlé aussi au chargement)
'
' Remarques   :
' - Lecture seule, sauf le réalignement des montants en attente (donnée dérivée, confirmé, journalisé) ;
'   aucun secret affiché (voir GestionEtatSysteme).
' - Couleurs des appréciations réglées par le code : états dynamiques (règle « Designer d'abord »).
' - V1.1.0 (18/10/2026, UI-9.3, maquette « Grilles — états lisibles ») : appréciation en pastille
'   à trois tons (UtilsPastilles.Teinte(NiveauEtat) : « À vérifier » n'est plus rouge comme
'   « Problème ») ; titres de section en repères titrés (UtilsDataGrid.DessinerRepereSurLigne) au
'   lieu de fausses lignes en gras ; grille non triable (l'ordre des sections porte le sens).
' - V1.2.0 (22/10/2026, relevé de Joëlle) : bouton « Réaligner les montants en attente », actif quand
'   des séances ont un dû en attente différent de leur reste dû (données d'avant le lot B).
'
' Dépendances :
' - GestionEtatSysteme, UserControlContext, UtilsButtons, UtilsDataGrid, UtilsPastilles, DialogChoix, GestionLog
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_EtatSysteme
    Implements IContextAwareUserControl

#Region "Variables"

    Private _context As UserControlContext

    ' Dernier état affiché (pour le rapport copié).
    Private _sections As New List(Of SectionEtat)
    Private _collecteEnCours As Boolean = False

    ' Séances dont le dû en attente est à réaligner (lu à chaque actualisation).
    Private _nbDusDesalignes As Integer = 0

#End Region

#Region "Contexte UI"

    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext
        _context = context
    End Sub

#End Region

#Region "Chargement"

    Private Sub UC_EtatSysteme_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        UtilsButtons.InitStandardButton(btnCopierRapport)
        UtilsButtons.InitStandardButton(btnActualiser)
        UtilsDataGrid.InitDataGridBasique(dgvEtat)
        UtilsDataGrid.DesactiverTri(dgvEtat)

        _context?.SetHeader("Administration > État du système")

        If _context Is Nothing OrElse Not _context.HasRole(AppRole.Admin) Then
            btnCopierRapport.Enabled = False
            btnActualiser.Enabled = False
            lblHorodatage.Text = "L'état du système est réservé à l'administrateur."
            Return
        End If

        If _context IsNot Nothing Then
            _context.SetToolTip(btnCopierRapport, "Copier l'état complet (sans aucun mot de passe) pour le coller dans un message.")
            _context.SetToolTip(btnActualiser, "Relire l'état du système.")
            _context.SetToolTip(btnRealignerDus, "Remettre le montant en attente de chaque séance signalée égal à ce qu'il reste à payer.")
        End If

        Actualiser()

    End Sub

#End Region

#Region "Affichage"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Actualiser
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Collecte l'état en arrière-plan (connexion, disques, recherche des outils) et l'affiche.
    '
    ' Remarques  :
    ' - GestionEtatSysteme.CollecterEtat ne lève pas d'exception : chaque échec est une ligne.
    ' -------------------------------------------------------------------------------------------------
    Private Async Sub Actualiser()

        If _collecteEnCours Then Return
        _collecteEnCours = True
        btnActualiser.Enabled = False
        btnCopierRapport.Enabled = False
        btnRealignerDus.Enabled = False
        UseWaitCursor = True
        lblHorodatage.Text = "Lecture de l'état…"

        Dim sections As List(Of SectionEtat)
        Try
            sections = Await Task.Run(Function() GestionEtatSysteme.CollecterEtat())
        Catch ex As Exception
            GestionLog.EcrireLog("UI: état du système non collecté.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            sections = New List(Of SectionEtat)
        End Try

        ' Bouton « Réaligner » : la section Cohérence dit déjà si le contrôle a échoué (bouton inactif).
        Try
            _nbDusDesalignes = Await Task.Run(Function() GestionPaiements.CompterDusDesalignes())
        Catch ex As Exception
            GestionLog.EcrireLog("UI: contrôle des montants en attente impossible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            _nbDusDesalignes = 0
        End Try

        If IsDisposed Then Return

        _sections = sections
        Afficher()

        UseWaitCursor = False
        _collecteEnCours = False
        btnActualiser.Enabled = True
        btnCopierRapport.Enabled = _sections.Count > 0
        btnRealignerDus.Enabled = _nbDusDesalignes > 0
        lblHorodatage.Text = $"État lu le {DateTime.Now:dd/MM/yyyy à HH:mm:ss}"

        Dim nbProblemes As Integer = _sections.Sum(Function(s) s.Lignes.Where(Function(l) l.Niveau = NiveauEtat.Probleme).Count())
        Dim nbAVerifier As Integer = _sections.Sum(Function(s) s.Lignes.Where(Function(l) l.Niveau = NiveauEtat.Avertissement).Count())
        _context?.SetStatus(If(nbProblemes + nbAVerifier = 0,
                               "État du système : tout est en ordre.",
                               $"État du système : {nbProblemes} problème(s), {nbAVerifier} point(s) à vérifier."))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Afficher
    ' Version    : V1.1.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Une ligne réservée par section (repère titré, non sélectionnable), puis ses lignes ; chaque ligne
    ' porte son élément (LigneEtat) ou sa section (SectionEtat) dans Tag, lu au dessin.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Afficher()

        dgvEtat.SuspendLayout()
        dgvEtat.Rows.Clear()

        For Each section In _sections

            Dim indexTitre As Integer = dgvEtat.Rows.Add(String.Empty, String.Empty, String.Empty)
            With dgvEtat.Rows(indexTitre)
                .Tag = section
                ' Un repère ne se sélectionne pas : sélection invisible sur sa ligne.
                .DefaultCellStyle.SelectionBackColor = .InheritedStyle.BackColor
            End With

            For Each ligne In section.Lignes
                Dim index As Integer = dgvEtat.Rows.Add(ligne.Libelle, ligne.Valeur,
                                                        GestionEtatSysteme.LibelleNiveau(ligne.Niveau))
                dgvEtat.Rows(index).Tag = ligne
            Next

        Next

        dgvEtat.ResumeLayout()
        dgvEtat.ClearSelection()

    End Sub

    ' Ligne de section : fond seul (le titre est dessiné par RowPostPaint) ; appréciation : pastille.
    Private Sub dgvEtat_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvEtat.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim tag As Object = dgvEtat.Rows(e.RowIndex).Tag

        If TypeOf tag Is SectionEtat Then
            e.PaintBackground(e.CellBounds, False)
            e.Handled = True
            Return
        End If

        Dim ligne As LigneEtat = TryCast(tag, LigneEtat)
        If ligne IsNot Nothing AndAlso e.ColumnIndex = colEtat.Index Then
            UtilsDataGrid.DessinerPastilleCellule(dgvEtat, e, GestionEtatSysteme.LibelleNiveau(ligne.Niveau),
                                                  UtilsPastilles.Teinte(ligne.Niveau))
        End If

    End Sub

    Private Sub dgvEtat_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles dgvEtat.RowPostPaint
        Dim section As SectionEtat = TryCast(dgvEtat.Rows(e.RowIndex).Tag, SectionEtat)
        If section IsNot Nothing Then UtilsDataGrid.DessinerRepereSurLigne(e, dgvEtat, section.Titre)
    End Sub

#End Region

#Region "Actions"

    Private Sub btnActualiser_Click(sender As Object, e As EventArgs) Handles btnActualiser.Click
        Actualiser()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnRealignerDus_Click
    ' Version   : V1.0.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Après confirmation, réaligne le montant en attente des séances signalées sur leur reste dû,
    ' puis relit l'état.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRealignerDus_Click(sender As Object, e As EventArgs) Handles btnRealignerDus.Click

        If _nbDusDesalignes <= 0 Then Return

        If DialogChoix.Confirmer(
            $"{_nbDusDesalignes} séance(s) ont un montant en attente différent de ce qu'il reste à payer." & Environment.NewLine &
            "Il sera recalculé (comme à chaque enregistrement d'une séance) ; aucune séance ni aucun encaissement n'est modifié.",
            "Réaligner les montants en attente", "Réaligner", "Annuler") <> DialogResult.Yes Then
            Return
        End If

        Try
            Dim nb As Integer = GestionPaiements.RealignerDusDesalignes(_context?.ContexteOperation)
            DialogChoix.Succes($"{nb} séance(s) réalignée(s).", "Réaligner les montants en attente")
        Catch exRefus As InvalidOperationException
            DialogChoix.Information(exRefus.Message, "Réaligner les montants en attente")
        Catch exDroits As UnauthorizedAccessException
            DialogChoix.Information(exDroits.Message, "Réaligner les montants en attente")
        Catch ex As Exception
            GestionLog.EcrireLog("UI: réalignement des montants en attente impossible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Le réalignement n'a pas pu être fait ; vérifiez la connexion à la base, puis réessayez.",
                               "Réaligner les montants en attente")
        End Try

        Actualiser()

    End Sub

    Private Sub btnCopierRapport_Click(sender As Object, e As EventArgs) Handles btnCopierRapport.Click
        Try
            Clipboard.SetText(GestionEtatSysteme.RapportTexte(_sections))
            _context?.SetStatus("Rapport copié dans le presse-papiers.")
        Catch ex As Exception
            GestionLog.EcrireLog("UI: copie du rapport d'état impossible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Le rapport n'a pas pu être copié. Réessayez.", "Althéa - État du système")
        End Try
    End Sub

#End Region

End Class
