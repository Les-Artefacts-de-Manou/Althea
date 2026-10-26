' -------------------------------------------------------------------------------------------------
' Formulaire : DossierZoneEditor
' Projet     : Althéa
' Version    : V1.7.1
' Date       : 24/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale réutilisable permettant d'éditer UNE zone clinique d'un dossier (brique B4).
' Héberge l'éditeur riche complet (UC_RichTextEditor) et persiste la zone via GestionDossiers.
'
' Responsabilités :
' - Charger le contenu (RTF + texte brut) de la zone via GestionDossiers.GetZoneDossier
' - Éditer le contenu dans UC_RichTextEditor (ou l'afficher en lecture seule pour l'archive)
' - Enregistrer le contenu via GestionDossiers.UpdateZoneDossier (zone éditable uniquement)
'
' Fonctionnement :
' - Le constructeur reçoit l'identifiant du dossier et la zone à éditer
' - Retour DialogResult.OK après enregistrement réussi ; DialogResult.Cancel sinon
'
' Important   :
' - La zone HistoriqueArchive est en lecture seule (D-DOS-4) : le bouton Enregistrer est masqué.
' - Aucun accès base direct : délègue à GestionDossiers (couche métier).
' - V1.3.1 (19/10/2026) : éditeur de texte refondu (UC_RichTextEditor V2.0.0) ; la zone reçoit le
'   contexte (horodatage signé) et titre ses impressions du nom de la zone.
' - V1.4.0 (19/10/2026) : modifications non enregistrées — fermer la fenêtre (Annuler, croix, Échap)
'   après une modification propose Enregistrer / Quitter sans enregistrer / Rester (GardeModifications).
'   Remplace l'avertissement à deux boutons et son suivi par ContentChanged.
' - V1.5.0 (19/10/2026) : en-tête des documents — titre « zone — dossier », patient reçu de
'   l'appelant (EnTeteDocument).
' - V1.6.0 (19/10/2026) : textes types de l'usage « Zones du dossier » (UsageTextesTypes).
' - V1.7.0 (22/10/2026, audit C.1) : refus du métier (dossier archivé) et échec de l'enregistrement
'   affichés dans une fenêtre ; la fenêtre reste ouverte, le texte saisi n'est pas perdu.
' - V1.7.1 (24/10/2026, audit C.3) : type et source de document lus dans CodesSysteme (plus de code
'   en clair).
'
' Dépendances :
' - GestionDossiers / ZonesDossier / ContenuZoneDossier (lecture/écriture de la zone)
' - UC_RichTextEditor (éditeur riche complet)
' - UserControlContext / IContextAwareForm (contexte UI partagé)
' - GestionLog (journalisation)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class DossierZoneEditor
    Implements IContextAwareForm

#Region "Variables privées"

    ' Contexte UI partagé fourni par l'appelant (barre de statut, infobulles). Peut être Nothing.
    Private _context As UserControlContext

    ' Identifiant du dossier propriétaire de la zone.
    Private ReadOnly _idDossier As Long

    ' Zone clinique éditée.
    Private ReadOnly _zone As ZoneDossier

    ' Métadonnées (libellé, lecture seule) de la zone éditée.
    Private ReadOnly _info As ZoneDossierInfo

    ' Code patient propriétaire du dossier (pour l'export documentaire déterministe).
    Private ReadOnly _codePatient As String

    ' Code du dossier (pour l'export documentaire déterministe).
    Private ReadOnly _codeDossier As String

    ' Force la lecture seule quel que soit le type de zone (ex. dossier archivé, D-Q10).
    Private ReadOnly _lectureSeuleForcee As Boolean

    ' Patient du dossier (EnTeteDocument.DesignerPatient), pour l'en-tête des documents.
    Private ReadOnly _patient As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstLectureSeule
    ' Version    : V1.1.0
    ' Date       : 11/07/2026
    '
    ' Rôle       :
    ' Détermine si la zone doit être affichée sans possibilité de modification.
    '
    ' Retour     :
    ' - Boolean : True si la zone est archivée ou si la lecture seule est imposée par le dossier.
    ' -------------------------------------------------------------------------------------------------
    Private ReadOnly Property EstLectureSeule As Boolean
        Get
            Return _info.LectureSeule OrElse _lectureSeuleForcee
        End Get
    End Property

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V1.1.0
    ' Date          : 19/10/2026
    '
    ' Rôle          :
    ' Initialise la modale pour l'édition d'une zone clinique d'un dossier.
    '
    ' Paramètres    :
    ' - idDossier   : Identifiant du dossier (> 0)
    ' - zone        : Zone clinique à éditer
    ' - codePatient : Code patient propriétaire (pour l'export documentaire), optionnel
    ' - codeDossier : Code du dossier (pour l'export documentaire), optionnel
    ' - lectureSeuleForcee : Force la lecture seule (dossier archivé), optionnel
    ' - patient     : Patient du dossier, imprimé dans l'en-tête des documents, optionnel
    '
    ' Remarques     :
    ' - V1.1.0 (19/10/2026) : paramètre patient (en-tête des impressions et des exports).
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(idDossier As Long, zone As ZoneDossier,
                   Optional codePatient As String = Nothing,
                   Optional codeDossier As String = Nothing,
                   Optional lectureSeuleForcee As Boolean = False,
                   Optional patient As String = Nothing)

        InitializeComponent()

        _idDossier = idDossier
        _zone = zone
        _info = ZonesDossier.GetInfo(zone)
        _codePatient = codePatient
        _codeDossier = codeDossier
        _lectureSeuleForcee = lectureSeuleForcee
        _patient = patient

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Reçoit le contexte UI partagé fourni par l'appelant.
    '
    ' Paramètres :
    ' - context : Contexte UI partagé.
    '
    ' Remarques :
    ' - Doit être appelée par l'appelant AVANT ShowDialog.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareForm.SetContext

        _context = context
        rteZone.SetContext(context)

    End Sub

#End Region

#Region "Cycle de vie"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DossierZoneEditor_Load
    ' Version   : V1.4.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Applique le fond du kit, les titres, le mode lecture seule éventuel et charge le contenu.
    '
    ' Remarques :
    ' - UI-7 (15/10/2026) : la carte porte le nom de la zone (et « consultation » en lecture seule) ;
    '   l'en-tête rappelle le dossier quand son code est connu.
    ' - V1.2.0 (19/10/2026) : garde des modifications non enregistrées (GardeModifications.SurveillerFenetre).
    ' - V1.3.0 (19/10/2026) : titre « zone — dossier » et patient pour l'en-tête des documents.
    ' - V1.4.0 (19/10/2026) : textes types proposés dans l'éditeur (UsageTextesTypes).
    ' -------------------------------------------------------------------------------------------------
    Private Sub DossierZoneEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        GardeModifications.SurveillerFenetre(Me, btnEnregistrer, $"de la zone « {_info.Libelle} »")

        ' Fond beige à la feuille et barre de titre aux couleurs du kit.
        AltheaFondFenetre.Appliquer(Me)

        If Not String.IsNullOrWhiteSpace(_codeDossier) Then
            enTete.SousTitre = $"Zone clinique — dossier {_codeDossier}"
        End If

        carteZone.Titre = If(EstLectureSeule, $"{_info.Libelle} (consultation)", _info.Libelle)
        Me.Text = $"Zone clinique — {_info.Libelle}"

        ' Zone d'archive OU dossier archivé : lecture seule, pas d'enregistrement possible (D-DOS-4 / D-Q10)
        rteZone.ReadOnlyMode = EstLectureSeule
        ' Titre et patient de l'en-tête des documents (impression, aperçu, exports).
        rteZone.TitreDocument = If(String.IsNullOrWhiteSpace(_codeDossier), _info.Libelle, $"{_info.Libelle} — dossier {_codeDossier}")
        rteZone.PatientDocument = _patient
        rteZone.UsageTextesTypes = UsagesTexteType.ZoneDossier
        btnEnregistrer.Visible = Not EstLectureSeule

        If _context IsNot Nothing Then
            _context.SetToolTip(btnEnregistrer, "Enregistrer le contenu de la zone.")
            _context.SetToolTip(btnFermer, "Fermer la fenêtre.")
        End If

        ChargerContenu()

    End Sub


#End Region

#Region "Chargement"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerContenu
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Charge le contenu (RTF + texte brut de repli) de la zone dans l'éditeur.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerContenu()

        Try

            Dim contenu As ContenuZoneDossier = GestionDossiers.GetZoneDossier(_idDossier, _zone)

            If contenu Is Nothing Then
                rteZone.ChargerContenu(String.Empty, String.Empty)
                Return
            End If

            rteZone.ChargerContenu(contenu.Rtf, contenu.Txt)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement zone (DossierZoneEditor, id={_idDossier}, zone={_zone}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement de la zone.")
            End If

        End Try

    End Sub

#End Region

#Region "Actions"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Enregistre le contenu de la zone via la couche métier puis ferme la modale (DialogResult.OK).
    ' - V1.1.0 (22/10/2026, audit C.1) : refus et échec affichés dans une fenêtre (ils ne restaient
    '   que dans la barre d'état).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click

        If _info.LectureSeule Then
            Return
        End If

        Try

            GestionDossiers.UpdateZoneDossier(_idDossier, _zone, rteZone.RtfContent, rteZone.TextContent)

            If _context IsNot Nothing Then
                _context.SetStatus($"Zone « {_info.Libelle} » enregistrée.")
            End If

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch exRefus As InvalidOperationException

            ' Refus métier (message rédigé pour l'utilisatrice).
            DialogChoix.Information(exRefus.Message, "Enregistrement impossible")
            _context?.SetStatus("Enregistrement de la zone refusé.")

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur enregistrement zone (DossierZoneEditor, id={_idDossier}, zone={_zone}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "La zone n'a pas pu être enregistrée ; rien n'a été modifié et votre texte est toujours là." & Environment.NewLine &
                "Vérifiez la connexion à la base de données, puis cliquez à nouveau sur Enregistrer.",
                "Enregistrement impossible")
            _context?.SetStatus("Erreur lors de l'enregistrement de la zone.")

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnFermer_Click
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Ferme la modale sans enregistrer (DialogResult.Cancel).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

#End Region

#Region "Export documentaire"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : rteZone_ExportRequested
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Fournit (par délégation) le chemin de destination déterministe pour l'export de la zone :
    ' dossier du dossier de suivi ({…}/Documents/{code_patient}/{code_dossier}) et nom horodaté
    ' (suivi_{code_dossier}_{colonne_txt}_{yyyyMMdd_HHmmss}.{ext}).
    '
    ' Remarques :
    ' - Uniquement si les codes patient et dossier sont connus ; sinon fallback dialogue du contrôle.
    ' - Le dossier de suivi est créé à la demande (AssurerDossierDuDossier) avant l'écriture.
    ' -------------------------------------------------------------------------------------------------
    Private Sub rteZone_ExportRequested(sender As Object, e As ExportRequestedEventArgs) _
        Handles rteZone.ExportRequested

        If String.IsNullOrWhiteSpace(_codePatient) OrElse String.IsNullOrWhiteSpace(_codeDossier) Then
            Return
        End If

        Try

            CheminsDossierHelper.AssurerDossierDuDossier(_codePatient, _codeDossier)

            ' TODO (synchro Google Drive) : revenir ici lorsque la synchronisation des dossiers avec
            ' Google Drive sera implémentée. Selon l'approche retenue (upload à l'export, watcher de
            ' dossier, API Drive…), l'écriture locale du fichier exporté devra peut-être déclencher
            ' ou être coordonnée avec la synchro distante du dossier de suivi.

            Dim extension As String = If(e.Format = ExportFormat.Word, ".docx", ".pdf")

            ' Segment lisible dérivé de la colonne texte de la zone, sans le suffixe technique « _txt »
            ' (convention de nommage des exports : on retire ce suffixe pour un nom de fichier plus propre).
            Dim segmentZone As String = _info.ColonneTxt
            If segmentZone.EndsWith("_txt", StringComparison.OrdinalIgnoreCase) Then
                segmentZone = segmentZone.Substring(0, segmentZone.Length - 4)
            End If

            ' Nom déterministe SANS horodatage : le document exporté est unique par dossier + zone.
            ' Chaque export écrase la version précédente (une seule dernière version conservée).
            Dim nomFichier As String = $"suivi_{_codeDossier}_{segmentZone}{extension}"

            e.DestinationPath = CheminsDossierHelper.GetCheminFichierDossier(_codePatient, _codeDossier, nomFichier)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur préparation export zone (DossierZoneEditor, id={_idDossier}, zone={_zone}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : rteZone_ExportCompleted
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Ouvre automatiquement le fichier exporté (PDF/Word) après un export contextuel réussi.
    '
    ' Remarques :
    ' - Ouverture via l'association de fichiers du système (ShellExecute).
    ' - Trace le document exporté dans la table documents (niveau dossier, type ZONE_CLINIQUE).
    ' -------------------------------------------------------------------------------------------------
    Private Sub rteZone_ExportCompleted(sender As Object, e As ExportCompletedEventArgs) _
        Handles rteZone.ExportCompleted

        If Not e.Success OrElse String.IsNullOrWhiteSpace(e.DestinationPath) Then
            Return
        End If

        TracerDocumentExporte(e)

        Try

            Dim psi As New Diagnostics.ProcessStartInfo(e.DestinationPath) With {
                .UseShellExecute = True
            }
            Diagnostics.Process.Start(psi)

            If _context IsNot Nothing Then
                _context.SetStatus($"Zone « {_info.Libelle} » exportée et ouverte.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ouverture du fichier exporté (DossierZoneEditor, id={_idDossier}, zone={_zone}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : TracerDocumentExporte
    ' Version   : V1.0.0
    ' Date      : 09/06/2026
    '
    ' Rôle      :
    ' Insère une trace dans la table documents pour l'export de la zone clinique (niveau dossier).
    '
    ' Remarques :
    ' - Type de document : ZONE_CLINIQUE (le nom de fichier identifie déjà la zone précise).
    ' - chemin_relatif : relatif à PATH_GENERAL (Documents/{code_patient}/{code_dossier}/{fichier}).
    ' - type_source_document : PDF ou LOCAL_DOCX selon le format exporté.
    ' - Best-effort : une erreur de traçabilité ne doit pas empêcher l'ouverture du fichier exporté.
    ' -------------------------------------------------------------------------------------------------
    Private Sub TracerDocumentExporte(e As ExportCompletedEventArgs)

        If String.IsNullOrWhiteSpace(_codePatient) OrElse String.IsNullOrWhiteSpace(_codeDossier) Then
            Return
        End If

        Try

            Dim idType As ULong = GestionDocuments.ResoudreIdTypeDocumentParCode(CodesSysteme.TypesDocument.ZoneClinique)

            If idType = 0UL Then
                GestionLog.EcrireLog(
                    "Type de document ZONE_CLINIQUE introuvable : trace documentaire ignorée.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.UI)
                Return
            End If

            Dim nomFichier As String = System.IO.Path.GetFileName(e.DestinationPath)
            Dim extension As String = System.IO.Path.GetExtension(e.DestinationPath).TrimStart("."c)

            Dim doc As New Document With {
                .IdDossier = CULng(_idDossier),
                .IdTypeDocument = idType,
                .TypeSourceDocument = If(e.Format = ExportFormat.Word, CodesSysteme.SourcesDocument.LocalDocx, CodesSysteme.SourcesDocument.Pdf),
                .NomDocument = _info.Libelle,
                .NomFichier = nomFichier,
                .ExtensionFichier = extension,
                .CheminRelatif = CheminsDossierHelper.GetCheminRelatifFichierDossier(_codePatient, _codeDossier, nomFichier),
                .DateDocument = Date.Today
            }

            GestionDocuments.TracerDocument(doc)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur traçabilité document export zone (DossierZoneEditor, id={_idDossier}, zone={_zone}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

#End Region

End Class
