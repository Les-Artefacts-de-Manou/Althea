' -------------------------------------------------------------------------------------------------
' UserControl : UC_Journaux
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 18/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Consultation des journaux techniques de l'application (Administration › Logs), en lecture seule.
'
' Responsabilités :
' - Proposer les jours disponibles (un fichier par jour), le plus récent en premier
' - Afficher les entrées du jour choisi, les plus récentes en premier ; en-têtes de session en
'   repères titrés, entrées avec exception signalées par une pastille « Erreur »
' - Filtrer par niveau, catégorie, texte, erreurs seulement (sans relire le fichier)
' - Afficher le détail de l'entrée choisie (message complet, exception, piles d'appels)
' - Ouvrir le dossier des journaux dans l'explorateur
'
' Droits :
' - Administrateur seulement (tuile Logs ; contrôlé aussi au chargement)
'
' Remarques   :
' - Aucune modification ni suppression de journal : la purge reste celle de GestionLog
'   (LOG_RETENTION_JOURS).
' - Les listes de niveaux et de catégories viennent des énumérations de GestionLog (pas de liste
'   écrite en dur).
' - V1.1.0 (18/10/2026, UI-9.3, maquette « Grilles — états lisibles ») : pastille « Erreur » (ton
'   problème) dans la colonne Niveau, le niveau en petite mention ; en-tête de session dessiné en
'   repère titré (UtilsDataGrid.DessinerRepereSurLigne), toujours sélectionnable pour son détail ;
'   grille non triable (les sessions séparent les démarrages, un tri les mélangerait).
'
' Dépendances :
' - LecteurJournaux, GestionLog, UserControlContext, UtilsButtons, UtilsDataGrid, UtilsPastilles, DialogChoix
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Class UC_Journaux
    Implements IContextAwareUserControl

#Region "Variables"

    ' Contexte UI partagé fourni par Home.
    Private _context As UserControlContext

    ' Entrées du jour affiché (ordre du fichier) ; les filtres s'appliquent sur cette liste.
    Private _entrees As New List(Of EntreeJournal)

    ' Libellé de la première ligne des listes de filtres.
    Private Const LibelleTous As String = "(tous)"

    ' Texte de la pastille d'une entrée accompagnée d'une exception.
    Private Const LibelleErreur As String = "Erreur"

    ' Élément de la liste des jours : fichier, libellé lisible.
    Private NotInheritable Class ElementJour
        Public Property Fichier As FichierJournal
        Public Overrides Function ToString() As String
            Return $"{Fichier.Jour:dddd dd/MM/yyyy} ({Math.Max(1L, Fichier.TailleOctets \ 1024)} Ko)"
        End Function
    End Class

#End Region

#Region "Contexte UI"

    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext
        _context = context
    End Sub

#End Region

#Region "Chargement"

    Private Sub UC_Journaux_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        UtilsButtons.InitStandardButton(btnOuvrirDossier)
        UtilsButtons.InitStandardButton(btnActualiser)
        UtilsDataGrid.InitDataGridBasique(dgvJournal)
        UtilsDataGrid.DesactiverTri(dgvJournal)
        UtilsDataGrid.AlignerCentre(colHeure)

        _context?.SetHeader("Administration > Journaux")

        If _context Is Nothing OrElse Not _context.HasRole(AppRole.Admin) Then
            DesactiverEcran("La consultation des journaux est réservée à l'administrateur.")
            Return
        End If

        RemplirFiltres()
        ChargerJours(Nothing)
        InitialiserToolTips()

    End Sub

    Private Sub InitialiserToolTips()
        If _context Is Nothing Then Return
        _context.SetToolTip(cboJour, "Un journal par jour ; les plus anciens sont supprimés selon le paramètre LOG_RETENTION_JOURS.")
        _context.SetToolTip(chkErreursSeules, "N'afficher que les entrées accompagnées d'une erreur (exception).")
        _context.SetToolTip(btnOuvrirDossier, "Ouvrir le dossier des journaux dans l'explorateur Windows.")
        _context.SetToolTip(btnActualiser, "Relire la liste des jours et le journal affiché.")
    End Sub

    Private Sub DesactiverEcran(message As String)
        For Each controle In {cboJour, cboNiveau, cboCategorie}
            controle.Enabled = False
        Next
        txtRecherche.Enabled = False
        chkErreursSeules.Enabled = False
        btnOuvrirDossier.Enabled = False
        btnActualiser.Enabled = False
        lblCompteur.Text = message
        _context?.SetStatus(message)
    End Sub

    ' Niveaux et catégories : noms des énumérations de GestionLog, tels qu'ils sont écrits.
    Private Sub RemplirFiltres()

        cboNiveau.Items.Clear()
        cboNiveau.Items.Add(LibelleTous)
        cboNiveau.Items.AddRange([Enum].GetNames(GetType(GestionLog.LogLevel)))
        cboNiveau.SelectedIndex = 0

        cboCategorie.Items.Clear()
        cboCategorie.Items.Add(LibelleTous)
        cboCategorie.Items.AddRange([Enum].GetNames(GetType(GestionLog.LogCategory)))
        cboCategorie.SelectedIndex = 0

    End Sub

    Private Sub ChargerJours(cheminASelectionner As String)

        cboJour.Items.Clear()

        Try
            For Each fichier In LecteurJournaux.ListerFichiers()
                cboJour.Items.Add(New ElementJour With {.Fichier = fichier})
            Next
        Catch ex As Exception
            GestionLog.EcrireLog("UI: liste des journaux illisible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            _context?.SetStatus("Liste des journaux illisible. Consultez le dossier des journaux.")
        End Try

        If cboJour.Items.Count = 0 Then
            _entrees = New List(Of EntreeJournal)
            AppliquerFiltres()
            lblCompteur.Text = "Aucun journal disponible."
            Return
        End If

        Dim aSelectionner = cboJour.Items.Cast(Of ElementJour)() _
            .FirstOrDefault(Function(j) String.Equals(j.Fichier.Chemin, cheminASelectionner, StringComparison.OrdinalIgnoreCase))
        If aSelectionner IsNot Nothing Then
            cboJour.SelectedItem = aSelectionner
        Else
            cboJour.SelectedIndex = 0
        End If

    End Sub

#End Region

#Region "Lecture et filtres"

    Private Sub cboJour_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboJour.SelectedIndexChanged
        ChargerJournal()
    End Sub

    Private Sub ChargerJournal()

        Dim jour As ElementJour = TryCast(cboJour.SelectedItem, ElementJour)
        If jour Is Nothing Then Return

        Try
            UseWaitCursor = True
            _entrees = LecteurJournaux.Lire(jour.Fichier.Chemin)
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
            GestionLog.EcrireLog("UI: lecture d'un journal impossible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            _entrees = New List(Of EntreeJournal)
            DialogChoix.Avertissement("Ce journal n'a pas pu être lu : " & ex.Message, "Althéa - Journaux")
        Finally
            UseWaitCursor = False
        End Try

        AppliquerFiltres()

    End Sub

    Private Sub Filtres_Changed(sender As Object, e As EventArgs) Handles cboNiveau.SelectedIndexChanged,
        cboCategorie.SelectedIndexChanged, txtRecherche.TextChanged, chkErreursSeules.CheckedChanged
        AppliquerFiltres()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerFiltres
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Remplit la grille avec les entrées qui passent les filtres, les plus récentes en premier.
    '
    ' Remarques  :
    ' - Les en-têtes de session restent visibles tant qu'aucun filtre de contenu n'est actif : ils
    '   séparent les démarrages de l'application.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltres()

        Dim niveau As String = If(cboNiveau.SelectedIndex > 0, CStr(cboNiveau.SelectedItem), Nothing)
        Dim categorie As String = If(cboCategorie.SelectedIndex > 0, CStr(cboCategorie.SelectedItem), Nothing)
        Dim texte As String = txtRecherche.Text.Trim()
        Dim erreursSeules As Boolean = chkErreursSeules.Checked
        Dim filtreActif As Boolean = niveau IsNot Nothing OrElse categorie IsNot Nothing OrElse texte.Length > 0 OrElse erreursSeules

        dgvJournal.SuspendLayout()
        dgvJournal.Rows.Clear()

        Dim nbAffichees As Integer = 0
        For i As Integer = _entrees.Count - 1 To 0 Step -1

            Dim entree As EntreeJournal = _entrees(i)

            If entree.EstSession Then
                If filtreActif Then Continue For
            Else
                If niveau IsNot Nothing AndAlso Not String.Equals(entree.Niveau, niveau, StringComparison.OrdinalIgnoreCase) Then Continue For
                If categorie IsNot Nothing AndAlso Not String.Equals(entree.Categorie, categorie, StringComparison.OrdinalIgnoreCase) Then Continue For
                If erreursSeules AndAlso Not entree.AUneException Then Continue For
                If texte.Length > 0 AndAlso
                   entree.Message.IndexOf(texte, StringComparison.CurrentCultureIgnoreCase) < 0 AndAlso
                   entree.Exception.IndexOf(texte, StringComparison.CurrentCultureIgnoreCase) < 0 Then Continue For
            End If

            Dim index As Integer = dgvJournal.Rows.Add(
                If(entree.Horodatage.HasValue, entree.Horodatage.Value.ToString("HH:mm:ss"), String.Empty),
                If(entree.EstSession, "Session", entree.Niveau),
                entree.Categorie,
                ResumeMessage(entree))
            dgvJournal.Rows(index).Tag = entree

            If Not entree.EstSession Then nbAffichees += 1
        Next

        dgvJournal.ResumeLayout()

        Dim nbTotal As Integer = _entrees.Where(Function(x) Not x.EstSession).Count()
        lblCompteur.Text = If(filtreActif,
                              $"{nbAffichees} entrée(s) affichée(s) sur {nbTotal}",
                              $"{nbTotal} entrée(s)")

        AfficherDetail()

    End Sub

    ' Première ligne du message (+ l'exception en abrégé) : le détail complet est en bas.
    Private Shared Function ResumeMessage(entree As EntreeJournal) As String
        Dim message As String = If(entree.Message, String.Empty)
        Dim retour As Integer = message.IndexOf(Environment.NewLine, StringComparison.Ordinal)
        If retour >= 0 Then message = message.Substring(0, retour) & " …"
        If entree.AUneException Then message &= "  ⟶  " & entree.Exception
        Return message
    End Function

    ' En-tête de session : fond seul (repère dessiné par RowPostPaint) ; entrée avec exception :
    ' pastille « Erreur », le niveau en mention.
    Private Sub dgvJournal_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvJournal.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim entree As EntreeJournal = TryCast(dgvJournal.Rows(e.RowIndex).Tag, EntreeJournal)
        If entree Is Nothing Then Return

        If entree.EstSession Then
            e.PaintBackground(e.CellBounds, True)
            e.Handled = True
        ElseIf entree.AUneException AndAlso e.ColumnIndex = colNiveau.Index Then
            UtilsDataGrid.DessinerPastilleCellule(dgvJournal, e, LibelleErreur, UtilsPastilles.Teinte(TonEtat.Probleme),
                                                  mention:=entree.Niveau)
        End If

    End Sub

    Private Sub dgvJournal_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles dgvJournal.RowPostPaint
        Dim entree As EntreeJournal = TryCast(dgvJournal.Rows(e.RowIndex).Tag, EntreeJournal)
        If entree Is Nothing OrElse Not entree.EstSession Then Return
        Dim heure As String = If(entree.Horodatage.HasValue, entree.Horodatage.Value.ToString("HH:mm:ss") & " · ", String.Empty)
        UtilsDataGrid.DessinerRepereSurLigne(e, dgvJournal, $"Session · {heure}{entree.Message}")
    End Sub

    Private Sub dgvJournal_SelectionChanged(sender As Object, e As EventArgs) Handles dgvJournal.SelectionChanged
        AfficherDetail()
    End Sub

    Private Sub AfficherDetail()

        Dim entree As EntreeJournal = If(dgvJournal.CurrentRow Is Nothing, Nothing, TryCast(dgvJournal.CurrentRow.Tag, EntreeJournal))
        If entree Is Nothing Then
            txtDetail.Clear()
            Return
        End If

        Dim texte As New System.Text.StringBuilder()
        texte.Append(If(entree.Horodatage.HasValue, entree.Horodatage.Value.ToString("dd/MM/yyyy HH:mm:ss"), String.Empty))
        If entree.EstSession Then
            texte.Append("  —  Démarrage de session")
        Else
            texte.Append($"  —  {entree.Niveau} / {entree.Categorie}")
        End If
        texte.AppendLine($"  (ligne {entree.NumeroLigne})")
        texte.AppendLine()
        texte.AppendLine(entree.Message)
        If entree.AUneException Then
            texte.AppendLine()
            texte.AppendLine("Erreur : " & entree.DetailTechnique)
        End If

        txtDetail.Text = texte.ToString().Replace(vbLf, vbCrLf).Replace(vbCr & vbCrLf, vbCrLf)

    End Sub

#End Region

#Region "Actions"

    Private Sub btnActualiser_Click(sender As Object, e As EventArgs) Handles btnActualiser.Click
        Dim jour As ElementJour = TryCast(cboJour.SelectedItem, ElementJour)
        ChargerJours(jour?.Fichier.Chemin)
        _context?.SetStatus("Journaux actualisés.")
    End Sub

    Private Sub btnOuvrirDossier_Click(sender As Object, e As EventArgs) Handles btnOuvrirDossier.Click

        Dim dossier As String = GestionLog.GetDossierJournaux()
        If Not Directory.Exists(dossier) Then
            DialogChoix.Avertissement("Le dossier des journaux n'existe pas encore.", "Althéa - Journaux")
            Return
        End If

        Try
            Dim infos As New ProcessStartInfo("explorer.exe")
            infos.ArgumentList.Add(dossier)
            Process.Start(infos)
        Catch ex As Exception
            GestionLog.EcrireLog("UI: ouverture du dossier des journaux impossible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Le dossier n'a pas pu être ouvert. Consultez les logs.", "Althéa - Journaux")
        End Try

    End Sub

#End Region

End Class
