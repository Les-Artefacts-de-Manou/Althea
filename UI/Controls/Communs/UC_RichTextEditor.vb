' -------------------------------------------------------------------------------------------------
' UserControl : UC_RichTextEditor
' Projet      : Althéa
' Version     : V1.0
' Date        : 16/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' UserControl réutilisable d'édition de texte riche (RichTextBox enrichi) pour les notes patients,
' dossiers, consultations et autres besoins d'édition formatée dans l'application Althéa.
'
' Responsabilités :
' - Fournir une barre d'outils complète pour le formatage de texte
' - Exposer les propriétés RtfContent et TextContent pour sauvegarde en base de données
' - Gérer les événements de modification de contenu
' - Déléguer l'export (PDF/Word) au conteneur via l'événement ExportRequested (export contextuel)
' - Supporter le mode lecture seule (ReadOnlyMode)
' - Permettre de masquer/afficher la barre d'outils (ShowToolbar)
' - Implémenter IContextAwareUserControl pour accès au contexte partagé
' - Initialiser les listes de polices et tailles de police
' - Connecter tous les boutons de la toolbar aux méthodes du Helper
'
' Remarques   :
' - La sauvegarde se fait en double : RTF (formatage) + TXT (recherche)
' - Les raccourcis clavier standard sont supportés (Ctrl+B, Ctrl+I, Ctrl+U, etc.)
' - La barre d'outils utilise des glyphes Unicode (remplaçables par des PNG si besoin)
' - Le contenu peut être imprimé avec aperçu via le bouton Imprimer
'
' Dépendances :
' - RichTextEditorHelper (module utilitaire)
' - IContextAwareUserControl (interface de contexte)
' - UserControlContext (contexte partagé)
' - UITheme (thème de couleurs)
'
' Imports     :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On
Imports System.Runtime.InteropServices

Public Class UC_RichTextEditor
    Implements IContextAwareUserControl

#Region "API Windows pour marges RichTextBox"

    ' API Windows pour définir les marges internes du RichTextBox
    Private Const EM_SETMARGINS As Integer = &HD3
    Private Const EC_LEFTMARGIN As Integer = &H1
    Private Const EC_RIGHTMARGIN As Integer = &H2

    ' -------------------------------------------------------------------------------------------------
    ' Déclaration : SendMessage
    ' API         : user32.dll (P/Invoke)
    '
    ' Rôle        : Envoie un message Windows directement au handle d'un contrôle (interopérabilité Win32).
    '               Utilisée ici avec EM_SETMARGINS (&HD3) pour définir les marges intérieures gauche
    '               et droite du RichTextBox, fonctionnalité non exposée nativement par WinForms.
    '
    ' Paramètres  :
    ' - hWnd   : Handle de la fenêtre cible (handle du RichTextBox)
    ' - msg    : Identifiant du message Windows (ici EM_SETMARGINS = &HD3)
    ' - wParam : Masque des marges à modifier (EC_LEFTMARGIN Or EC_RIGHTMARGIN)
    ' - lParam : Valeurs des marges, gauche en bits 0-15 et droite en bits 16-31 (en pixels)
    '
    ' Retour      :
    ' - IntPtr : Résultat du message (non utilisé pour EM_SETMARGINS)
    ' -------------------------------------------------------------------------------------------------
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : DefinirMargesInterieures
    ' Version    : V1.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Définit les marges intérieures gauche et droite du RichTextBox.
    '
    ' Paramètres :
    ' - rtb          : RichTextBox cible
    ' - margeGauche  : Marge gauche en pixels
    ' - margeDroite  : Marge droite en pixels
    '
    ' Remarques  :
    ' - Utilise l'API Windows EM_SETMARGINS
    ' - Améliore la lisibilité en évitant que le texte soit collé aux bords
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirMargesInterieures(rtb As RichTextBox, margeGauche As Integer, margeDroite As Integer)

        If rtb Is Nothing Then Exit Sub

        ' Construire le paramètre LPARAM : LoWord = marge gauche, HiWord = marge droite
        Dim lParam As IntPtr = New IntPtr(margeGauche Or (margeDroite << 16))

        ' Envoyer le message EM_SETMARGINS
        SendMessage(rtb.Handle, EM_SETMARGINS, New IntPtr(EC_LEFTMARGIN Or EC_RIGHTMARGIN), lParam)

    End Sub

#End Region

#Region "Variables privées"

    ' Contexte UI partagé (StatusBar, ToolTips, ErrorProvider, etc.)
    Private _context As UserControlContext

    ' Indicateur de chargement initial (évite le déclenchement de ContentChanged)
    Private _isLoading As Boolean = False

#End Region

#Region "Propriétés publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : RtfContent
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Obtient ou définit le contenu RTF de l'éditeur (pour sauvegarde en base de données).
    '
    ' Type       : String (Get/Set)
    '
    ' Remarques  :
    ' - Utilisé pour charger le contenu depuis la base de données
    ' - Utilisé pour extraire le contenu avant sauvegarde
    ' - Le setter déclenche le chargement via RichTextEditorHelper.ChargerContenu()
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Property RtfContent As String
        Get
            Return RichTextEditorHelper.ExtraireRtf(rtbEditor)
        End Get
        Set(value As String)
            _isLoading = True
            RichTextEditorHelper.ChargerContenu(rtbEditor, value, String.Empty)
            _isLoading = False
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : TextContent
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Obtient le contenu texte brut de l'éditeur (pour sauvegarde en base de données).
    '
    ' Type       : String (ReadOnly)
    '
    ' Remarques  :
    ' - Utilisé pour la colonne notes_txt (recherche full-text)
    ' - Extrait automatiquement depuis le RTF
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property TextContent As String
        Get
            Return RichTextEditorHelper.ExtraireTxt(rtbEditor)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ReadOnlyMode
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Active ou désactive le mode lecture seule de l'éditeur.
    '
    ' Type       : Boolean (Get/Set)
    '
    ' Remarques  :
    ' - En mode lecture seule, la toolbar est automatiquement masquée
    ' - Le RichTextBox devient non éditable
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Property ReadOnlyMode As Boolean
        Get
            Return rtbEditor.ReadOnly
        End Get
        Set(value As Boolean)
            rtbEditor.ReadOnly = value
            toolStrip.Visible = Not value
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ShowToolbar
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Affiche ou masque la barre d'outils.
    '
    ' Type       : Boolean (Get/Set)
    '
    ' Remarques  :
    ' - Utile pour masquer la toolbar sans activer le mode lecture seule
    ' - Par défaut : True
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Property ShowToolbar As Boolean
        Get
            Return toolStrip.Visible
        End Get
        Set(value As Boolean)
            toolStrip.Visible = value
        End Set
    End Property

#End Region

#Region "Méthodes publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChargerContenu
    ' Version    : V1.0
    ' Date       : 14/06/2026
    '
    ' Rôle       : Charge le contenu de l'éditeur depuis le RTF, avec repli sur le texte brut.
    ' Paramètres :
    ' - rtfContent  : Contenu RTF (peut être vide/Nothing)
    ' - txtFallback : Texte brut affiché si le RTF est vide ou invalide
    ' Remarques  :
    ' - Force la création du handle si nécessaire pour fiabiliser l'affectation du RTF
    '   (un RichTextBox sans handle peut ignorer silencieusement le contenu).
    ' - Symétrique de UC_RichTextEditorSimple.ChargerContenu (même comportement de repli).
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerContenu(rtfContent As String, txtFallback As String)

        If Not rtbEditor.IsHandleCreated Then
            rtbEditor.CreateControl()
        End If

        _isLoading = True
        RichTextEditorHelper.ChargerContenu(rtbEditor, rtfContent, txtFallback)
        _isLoading = False

    End Sub

#End Region

#Region "Événements"

    ' -------------------------------------------------------------------------------------------------
    ' Événement  : ContentChanged
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Déclenché lorsque le contenu de l'éditeur est modifié.
    '
    ' Remarques  :
    ' - Permet au parent de détecter les modifications et activer un bouton Enregistrer
    ' - Non déclenché lors du chargement initial
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Event ContentChanged(sender As Object, e As EventArgs)

    ' -------------------------------------------------------------------------------------------------
    ' Événement  : ContentSaved
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Déclenché lorsque le contenu est sauvegardé (optionnel, à déclencher manuellement par le parent).
    '
    ' Remarques  :
    ' - Permet de notifier que la sauvegarde est terminée
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Event ContentSaved(sender As Object, e As EventArgs)

    ' -------------------------------------------------------------------------------------------------
    ' Événement  : ExportRequested
    ' Version    : V1.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Déclenché lorsque l'utilisateur demande un export (PDF ou Word) depuis la barre d'outils.
    ' Permet au conteneur de décider, par délégation, de l'emplacement et du nom du fichier (export
    ' contextuel) sans coupler le contrôle générique à une logique métier (ex. dossier patient).
    '
    ' Remarques  :
    ' - Le conteneur renseigne e.DestinationPath pour un export direct (sans dialogue).
    ' - Si aucun abonné ne fournit de chemin, le contrôle retombe sur un SaveFileDialog (générique).
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Event ExportRequested(sender As Object, e As ExportRequestedEventArgs)

    ' -------------------------------------------------------------------------------------------------
    ' Événement  : ExportCompleted
    ' Version    : V1.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Déclenché après un export contextuel (chemin fourni par le conteneur), une fois le fichier écrit.
    ' Permet au conteneur de réagir (ouvrir le fichier, tracer le document en base ultérieurement).
    '
    ' Remarques  :
    ' - Non déclenché pour le fallback dialogue (le helper gère déjà son propre retour).
    ' - e.Success indique si l'export a réussi ; e.DestinationPath porte le chemin écrit.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Event ExportCompleted(sender As Object, e As ExportCompletedEventArgs)

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetContext
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Implémente IContextAwareUserControl.SetContext pour recevoir le contexte UI partagé.
    '
    ' Paramètres :
    ' - context (UserControlContext) : Le contexte partagé injecté par NavigationManager
    '
    ' Remarques  :
    ' - Appelé automatiquement lors du chargement du UserControl dans Home
    ' - Donne accès à StatusBar, ToolTips, ErrorProvider, AuthenticatedUser, etc.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext

        _context = context

        ' Configurer les ToolTips via le contexte
        If _context IsNot Nothing Then
            _context.SetToolTip(rtbEditor, "Éditeur de texte riche. Utilisez la barre d'outils pour formater votre texte.")
        End If

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UC_RichTextEditor_Load
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Initialise le UserControl au chargement : configure le RichTextBox, initialise les ComboBox.
    '
    ' Paramètres :
    ' - sender (Object) : La source de l'événement
    ' - e (EventArgs) : Les arguments de l'événement
    '
    ' Remarques  :
    ' - Remplit les ComboBox de polices et tailles
    ' - Configure le RichTextBox via le Helper
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_RichTextEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Configurer le RichTextBox
        RichTextEditorHelper.ConfigurerRichTextBox(rtbEditor)

        ' Définir des marges intérieures pour améliorer la lisibilité
        DefinirMargesInterieures(rtbEditor, 10, 10)

        ' Initialiser les ComboBox
        InitialiserComboPolices()
        InitialiserComboTailles()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitialiserComboPolices (privée)
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Remplit la ComboBox des familles de police avec les polices courantes.
    '
    ' Remarques  :
    ' - Liste limitée aux polices les plus utilisées pour clarté
    ' - Sélectionne Calibri par défaut (police standard Althéa)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserComboPolices()

        cmbFontFamily.Items.Clear()
        cmbFontFamily.Items.AddRange(New String() {
            "Calibri",
            "Arial",
            "Times New Roman",
            "Verdana",
            "Georgia",
            "Courier New",
            "Comic Sans MS",
            "Trebuchet MS",
            "Tahoma"
        })

        cmbFontFamily.SelectedIndex = 0 ' Calibri par défaut

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitialiserComboTailles (privée)
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Remplit la ComboBox des tailles de police avec les tailles courantes.
    '
    ' Remarques  :
    ' - Tailles de 8 à 72 points
    ' - Sélectionne 11 par défaut (taille standard Althéa)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserComboTailles()

        cmbFontSize.Items.Clear()
        cmbFontSize.Items.AddRange(New String() {
            "8", "9", "10", "11", "12", "14", "16", "18", "20", "24", "28", "36", "48", "72"
        })

        cmbFontSize.SelectedIndex = 3 ' 11 par défaut

    End Sub

#End Region

#Region "Gestionnaires d'événements Toolbar"

    ' -------------------------------------------------------------------------------------------------
    ' Groupe : Presse-papiers
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Actions :
    ' - Couper
    ' - Copier
    ' - Coller
    ' -------------------------------------------------------------------------------------------------

    Private Sub btnCut_Click(sender As Object, e As EventArgs) Handles btnCut.Click
        RichTextEditorHelper.Couper(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        RichTextEditorHelper.Copier(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnPaste_Click(sender As Object, e As EventArgs) Handles btnPaste.Click
        RichTextEditorHelper.Coller(rtbEditor)
        rtbEditor.Focus()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Groupe : Formatage de caractères
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Actions :
    ' - Gras
    ' - Italique
    ' - Souligné
    ' - Barre
    ' - Couleur
    ' - Highlight
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnBold_Click(sender As Object, e As EventArgs) Handles btnBold.Click
        RichTextEditorHelper.BasculerGras(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnItalic_Click(sender As Object, e As EventArgs) Handles btnItalic.Click
        RichTextEditorHelper.BasculerItalique(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnUnderline_Click(sender As Object, e As EventArgs) Handles btnUnderline.Click
        RichTextEditorHelper.BasculerSouligne(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnStrikeout_Click(sender As Object, e As EventArgs) Handles btnStrikeout.Click
        RichTextEditorHelper.BasculerBarre(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnTextColor_Click(sender As Object, e As EventArgs) Handles btnTextColor.Click
        Using colorDialog As New ColorDialog()
            colorDialog.Color = rtbEditor.SelectionColor
            If colorDialog.ShowDialog() = DialogResult.OK Then
                RichTextEditorHelper.ChangerCouleurTexte(rtbEditor, colorDialog.Color)
                rtbEditor.Focus()
            End If
        End Using
    End Sub

    Private Sub btnHighlightColor_Click(sender As Object, e As EventArgs) Handles btnHighlightColor.Click
        Using colorDialog As New ColorDialog()
            colorDialog.Color = rtbEditor.SelectionBackColor
            If colorDialog.ShowDialog() = DialogResult.OK Then
                RichTextEditorHelper.ChangerCouleurFond(rtbEditor, colorDialog.Color)
                rtbEditor.Focus()
            End If
        End Using
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Groupe : Police
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Actions :
    ' - Changer la police
    ' - Changer la taille de la police
    ' -------------------------------------------------------------------------------------------------

    Private Sub cmbFontFamily_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFontFamily.SelectedIndexChanged
        If cmbFontFamily.SelectedItem IsNot Nothing Then
            RichTextEditorHelper.ChangerPolice(rtbEditor, cmbFontFamily.SelectedItem.ToString())
            rtbEditor.Focus()
        End If
    End Sub

    Private Sub cmbFontSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFontSize.SelectedIndexChanged
        If cmbFontSize.SelectedItem IsNot Nothing Then
            Dim taille As Single
            If Single.TryParse(cmbFontSize.SelectedItem.ToString(), taille) Then
                RichTextEditorHelper.ChangerTaillePolice(rtbEditor, taille)
                rtbEditor.Focus()
            End If
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Groupe : Paragraphe
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Actions :
    ' - Aligner à gauche
    ' - Aligner au centre
    ' - Aligner à droite
    ' - Basculer les puces
    ' - Augmenter le retrait
    ' - Diminuer le retrait
    ' -------------------------------------------------------------------------------------------------

    Private Sub btnAlignLeft_Click(sender As Object, e As EventArgs) Handles btnAlignLeft.Click
        RichTextEditorHelper.ChangerAlignement(rtbEditor, HorizontalAlignment.Left)
        rtbEditor.Focus()
    End Sub

    Private Sub btnAlignCenter_Click(sender As Object, e As EventArgs) Handles btnAlignCenter.Click
        RichTextEditorHelper.ChangerAlignement(rtbEditor, HorizontalAlignment.Center)
        rtbEditor.Focus()
    End Sub

    Private Sub btnAlignRight_Click(sender As Object, e As EventArgs) Handles btnAlignRight.Click
        RichTextEditorHelper.ChangerAlignement(rtbEditor, HorizontalAlignment.Right)
        rtbEditor.Focus()
    End Sub

    Private Sub btnBullets_Click(sender As Object, e As EventArgs) Handles btnBullets.Click
        RichTextEditorHelper.BasculerPuces(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnIncreaseIndent_Click(sender As Object, e As EventArgs) Handles btnIncreaseIndent.Click
        RichTextEditorHelper.AugmenterRetrait(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnDecreaseIndent_Click(sender As Object, e As EventArgs) Handles btnDecreaseIndent.Click
        RichTextEditorHelper.DiminuerRetrait(rtbEditor)
        rtbEditor.Focus()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Groupe : Actions
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Actions :
    ' - Annuler
    ' - Annuler
    ' - Effacer le formatage
    ' -------------------------------------------------------------------------------------------------

    Private Sub btnUndo_Click(sender As Object, e As EventArgs) Handles btnUndo.Click
        If rtbEditor.CanUndo Then
            rtbEditor.Undo()
        End If
        rtbEditor.Focus()
    End Sub

    Private Sub btnRedo_Click(sender As Object, e As EventArgs) Handles btnRedo.Click
        If rtbEditor.CanRedo Then
            rtbEditor.Redo()
        End If
        rtbEditor.Focus()
    End Sub

    Private Sub btnClearFormatting_Click(sender As Object, e As EventArgs) Handles btnClearFormatting.Click
        EffacerFormatage(rtbEditor)
        rtbEditor.Focus()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Groupe : Insertion
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Actions :
    ' - Inserer une date et une heure
    ' -------------------------------------------------------------------------------------------------

    Private Sub btnInsertDateTime_Click(sender As Object, e As EventArgs) Handles btnInsertDateTime.Click
        RichTextEditorHelper.InsererDateHeure(rtbEditor)
        rtbEditor.Focus()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Groupe : Outils
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Actions :
    ' - Imprimer
    ' - Configurer la mise en page
    ' - Exporter en PDF
    ' - Exporter en Word
    ' -------------------------------------------------------------------------------------------------

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        RichTextEditorHelper.Imprimer(rtbEditor, "Notes Althéa")
        rtbEditor.Focus()
    End Sub

    Private Sub btnPageSetup_Click(sender As Object, e As EventArgs) Handles btnPageSetup.Click
        RichTextEditorHelper.ConfigurerMiseEnPage()
        rtbEditor.Focus()
    End Sub

    Private Sub btnExportPDF_Click(sender As Object, e As EventArgs) Handles btnExportPDF.Click
        DemanderExport(ExportFormat.Pdf, "Notes_Althea.pdf")
        rtbEditor.Focus()
    End Sub

    Private Sub btnExportWord_Click(sender As Object, e As EventArgs) Handles btnExportWord.Click
        DemanderExport(ExportFormat.Word, "Notes_Althea.docx")
        rtbEditor.Focus()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DemanderExport
    ' Version    : V1.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Centralise la logique d'export par délégation : lève ExportRequested pour laisser le conteneur
    ' fournir un chemin de destination (export contextuel). Si un chemin est fourni, exporte
    ' directement ; sinon, retombe sur le SaveFileDialog générique (comportement par défaut).
    '
    ' Paramètres :
    ' - format          : Format d'export demandé (Pdf ou Word)
    ' - nomFichierInitial : Nom proposé par défaut pour le fallback dialogue
    '
    ' Remarques  :
    ' - Le contrôle reste générique : il ne connaît pas la règle de nommage/dossier du conteneur.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DemanderExport(format As ExportFormat, nomFichierInitial As String)

        Dim args As New ExportRequestedEventArgs(format, nomFichierInitial)
        RaiseEvent ExportRequested(Me, args)

        Dim cheminFourni As Boolean = Not String.IsNullOrWhiteSpace(args.DestinationPath)

        Select Case format

            Case ExportFormat.Pdf
                If cheminFourni Then
                    Dim ok As Boolean = RichTextEditorHelper.ExporterPDF(rtbEditor, args.DestinationPath)
                    RaiseEvent ExportCompleted(Me, New ExportCompletedEventArgs(format, args.DestinationPath, ok))
                Else
                    RichTextEditorHelper.ExporterPDFAvecDialogue(rtbEditor, nomFichierInitial)
                End If

            Case ExportFormat.Word
                If cheminFourni Then
                    Dim ok As Boolean = RichTextEditorHelper.ExporterWord(rtbEditor, args.DestinationPath)
                    RaiseEvent ExportCompleted(Me, New ExportCompletedEventArgs(format, args.DestinationPath, ok))
                Else
                    RichTextEditorHelper.ExporterWordAvecDialogue(rtbEditor, nomFichierInitial)
                End If

        End Select

    End Sub

#End Region

#Region "Gestionnaires d'événements RichTextBox"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : rtbEditor_TextChanged
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Déclenche l'événement ContentChanged lorsque le texte est modifié.
    '
    ' Paramètres :
    ' - sender (Object) : La source de l'événement
    ' - e (EventArgs) : Les arguments de l'événement
    '
    ' Remarques  :
    ' - Ignoré pendant le chargement initial (_isLoading = True)
    ' - Permet au parent de détecter les modifications
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub rtbEditor_TextChanged(sender As Object, e As EventArgs) Handles rtbEditor.TextChanged

        If Not _isLoading Then
            RaiseEvent ContentChanged(Me, EventArgs.Empty)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : rtbEditor_SelectionChanged
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Met à jour l'état des boutons de la toolbar selon la sélection courante.
    '
    ' Paramètres :
    ' - sender (Object) : La source de l'événement
    ' - e (EventArgs) : Les arguments de l'événement
    '
    ' Remarques  :
    ' - Reflète l'état actuel (gras, italique, souligné, etc.) dans les boutons
    ' - Met à jour les ComboBox de police et taille
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub rtbEditor_SelectionChanged(sender As Object, e As EventArgs) Handles rtbEditor.SelectionChanged

        MettreAJourEtatBoutons()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : MettreAJourEtatBoutons (privée)
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Met à jour l'apparence des boutons de la toolbar selon la sélection courante.
    '
    ' Remarques  :
    ' - Change le BackColor des boutons actifs (gras, italique, souligné, etc.)
    ' - Met à jour les ComboBox de police et taille si sélection unique
    ' - Phase 1 : implémentation simplifiée
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourEtatBoutons()

        If rtbEditor.SelectionFont Is Nothing Then Exit Sub

        ' Mettre en surbrillance les boutons actifs
        Dim activeBtnColor As Color = Color.FromArgb(122, 155, 135) ' ColorSauge
        Dim defaultBtnColor As Color = Color.FromArgb(178, 197, 186) ' ColorSaugeClair

        btnBold.BackColor = If(rtbEditor.SelectionFont.Bold, activeBtnColor, defaultBtnColor)
        btnItalic.BackColor = If(rtbEditor.SelectionFont.Italic, activeBtnColor, defaultBtnColor)
        btnUnderline.BackColor = If(rtbEditor.SelectionFont.Underline, activeBtnColor, defaultBtnColor)
        btnStrikeout.BackColor = If(rtbEditor.SelectionFont.Strikeout, activeBtnColor, defaultBtnColor)
        btnBullets.BackColor = If(rtbEditor.SelectionBullet, activeBtnColor, defaultBtnColor)

        ' Mettre à jour les ComboBox (si possible)
        Try
            If Not String.IsNullOrEmpty(rtbEditor.SelectionFont.FontFamily.Name) Then
                Dim fontIndex As Integer = cmbFontFamily.FindString(rtbEditor.SelectionFont.FontFamily.Name)
                If fontIndex >= 0 Then cmbFontFamily.SelectedIndex = fontIndex
            End If

            Dim sizeIndex As Integer = cmbFontSize.FindString(CInt(rtbEditor.SelectionFont.Size).ToString())
            If sizeIndex >= 0 Then cmbFontSize.SelectedIndex = sizeIndex
        Catch
            ' Sélection multiple ou police mixte, on ne fait rien
        End Try

        ' Mettre à jour l'état des boutons Undo/Redo
        btnUndo.Enabled = rtbEditor.CanUndo
        btnRedo.Enabled = rtbEditor.CanRedo

    End Sub

#End Region

#Region "Raccourcis clavier"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : rtbEditor_KeyDown
    ' Version    : V1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Gère les raccourcis clavier personnalisés (Ctrl+B, Ctrl+I, Ctrl+U, etc.).
    '
    ' Paramètres :
    ' - sender (Object) : La source de l'événement
    ' - e (KeyEventArgs) : Les arguments de l'événement (touches pressées)
    '
    ' Remarques  :
    ' - Complète les raccourcis natifs du RichTextBox
    ' - Empêche la propagation de l'événement après traitement (e.Handled = True)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub rtbEditor_KeyDown(sender As Object, e As KeyEventArgs) Handles rtbEditor.KeyDown

        If e.Control Then
            Select Case e.KeyCode
                Case Keys.B
                    RichTextEditorHelper.BasculerGras(rtbEditor)
                    e.Handled = True
                Case Keys.I
                    RichTextEditorHelper.BasculerItalique(rtbEditor)
                    e.Handled = True
                Case Keys.U
                    RichTextEditorHelper.BasculerSouligne(rtbEditor)
                    e.Handled = True
                Case Keys.P
                    btnPrint_Click(Nothing, Nothing)
                    e.Handled = True
            End Select
        End If

    End Sub

#End Region

End Class

' -------------------------------------------------------------------------------------------------
' Enum        : ExportFormat
' Projet      : Althéa
' Version     : V1.0
' Date        : 14/06/2026
'
' Rôle        :
' Formats d'export proposés par UC_RichTextEditor (utilisés dans ExportRequestedEventArgs).
' -------------------------------------------------------------------------------------------------
Public Enum ExportFormat
    Pdf
    Word
End Enum

' -------------------------------------------------------------------------------------------------
' Classe      : ExportRequestedEventArgs
' Projet      : Althéa
' Version     : V1.0
' Date        : 14/06/2026
'
' Rôle        :
' Arguments de l'événement ExportRequested : transporte le format demandé et permet au conteneur
' de fournir, par délégation, le chemin de destination (export contextuel).
'
' Propriétés  :
' - Format          : Format d'export demandé (Pdf ou Word)
' - NomFichierInitial : Nom proposé par défaut pour le fallback dialogue (lecture seule)
' - DestinationPath : Chemin complet de destination, renseigné par le conteneur (Nothing = fallback)
'
' Remarques   :
' - Si DestinationPath reste Nothing/vide après l'événement, le contrôle ouvre un SaveFileDialog.
' -------------------------------------------------------------------------------------------------
Public Class ExportRequestedEventArgs
    Inherits EventArgs

    Public ReadOnly Property Format As ExportFormat
    Public ReadOnly Property NomFichierInitial As String
    Public Property DestinationPath As String

    Public Sub New(exportFormat As ExportFormat, nomFichierInitial As String)
        Me.Format = exportFormat
        Me.NomFichierInitial = nomFichierInitial
    End Sub

End Class

' -------------------------------------------------------------------------------------------------
' Classe      : ExportCompletedEventArgs
' Projet      : Althéa
' Version     : V1.0
' Date        : 14/06/2026
'
' Rôle        :
' Arguments de l'événement ExportCompleted : informe le conteneur de la fin d'un export contextuel.
'
' Propriétés  :
' - Format          : Format exporté (Pdf ou Word)
' - DestinationPath : Chemin complet du fichier écrit
' - Success         : Indique si l'export a réussi
' -------------------------------------------------------------------------------------------------
Public Class ExportCompletedEventArgs
    Inherits EventArgs

    Public ReadOnly Property Format As ExportFormat
    Public ReadOnly Property DestinationPath As String
    Public ReadOnly Property Success As Boolean

    Public Sub New(exportFormat As ExportFormat, destinationPath As String, success As Boolean)
        Me.Format = exportFormat
        Me.DestinationPath = destinationPath
        Me.Success = success
    End Sub

End Class
