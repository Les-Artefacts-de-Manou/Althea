' -------------------------------------------------------------------------------------------------
' UserControl : UC_RichTextEditorSimple
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 10/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Version allégée de UC_RichTextEditor, destinée aux zones de commentaires et notes courtes
' embarquées dans d'autres UserControls ou Forms (référentiels, fiches patient, dossiers…).
'
' Responsabilités :
' - Fournir une toolbar minimale : Gras, Italique, Souligné, Annuler, Rétablir,
'   Effacer formatage, Insérer date/heure
' - Exposer RtfContent et TextContent pour sauvegarde en base de données
' - Gérer le mode lecture seule (ReadOnlyMode)
' - Permettre de masquer/afficher la toolbar (ShowToolbar)
' - Notifier les modifications via l'événement ContentChanged
' - Optionnellement recevoir le contexte UI via IContextAwareUserControl
'
' Différences avec UC_RichTextEditor (complet) :
' - Pas de sélection de police/taille
' - Pas d'alignement, puces, retraits
' - Pas de couleurs texte/fond
' - Pas de couper/copier/coller (raccourcis OS Ctrl+X/C/V natifs suffisent)
' - Pas d'impression ni d'export PDF/Word
' - Taille par défaut plus compacte (dimensionnée par le parent)
'
' Remarques   :
' - Réutilise RichTextEditorHelper à 100 % : aucune logique dupliquée
' - La sauvegarde reste en double format : RTF (formatage) + TXT (recherche)
' - Raccourcis Ctrl+B, Ctrl+I, Ctrl+U pris en charge nativement
' - La taille est entièrement gérée par le parent (Dock ou Anchor)
' - IContextAwareUserControl implémenté de façon optionnelle :
'   le contrôle fonctionne sans contexte si le parent ne l'injecte pas
'
' Dépendances :
' - RichTextEditorHelper (module utilitaire partagé avec UC_RichTextEditor)
' - IContextAwareUserControl (interface de contexte)
' - UserControlContext (contexte partagé, optionnel)
' - UITheme (couleurs charte)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On
Imports System.Runtime.InteropServices

Public Class UC_RichTextEditorSimple
    Implements IContextAwareUserControl

#Region "API Windows pour marges RichTextBox"

    Private Const EM_SETMARGINS As Integer = &HD3
    Private Const EC_LEFTMARGIN As Integer = &H1
    Private Const EC_RIGHTMARGIN As Integer = &H2

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    Private Sub DefinirMargesInterieures(rtb As RichTextBox, margeGauche As Integer, margeDroite As Integer)
        If rtb Is Nothing Then Exit Sub
        Dim lParam As IntPtr = New IntPtr(margeGauche Or (margeDroite << 16))
        SendMessage(rtb.Handle, EM_SETMARGINS, New IntPtr(EC_LEFTMARGIN Or EC_RIGHTMARGIN), lParam)
    End Sub

#End Region

#Region "Variables privées"

    Private _context As UserControlContext
    Private _isLoading As Boolean = False

#End Region

#Region "Propriétés publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : RtfContent
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Obtient ou définit le contenu RTF (sauvegarde base de données).
    ' Remarques  : Le setter accepte aussi du texte brut.
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
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Obtient le texte brut (colonne _txt pour recherche full-text).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property TextContent As String
        Get
            Return RichTextEditorHelper.ExtraireTxt(rtbEditor)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ReadOnlyMode
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Active/désactive le mode lecture seule. Masque la toolbar si True.
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
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Affiche ou masque la toolbar sans changer le mode lecture seule.
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

#Region "Événements"

    ' -------------------------------------------------------------------------------------------------
    ' Événement  : ContentChanged
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Déclenché à chaque modification du contenu (hors chargement initial).
    '              Permet au parent de détecter un état "modifié non enregistré".
    ' -------------------------------------------------------------------------------------------------
    Public Event ContentChanged(sender As Object, e As EventArgs)

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetContext
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Reçoit le contexte UI partagé (optionnel : fonctionne sans contexte).
    ' Remarques  : Implémente IContextAwareUserControl.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext
        _context = context
        If _context IsNot Nothing Then
            _context.SetToolTip(rtbEditor, "Zone de commentaire. Utilisez Gras (Ctrl+B), Italique (Ctrl+I), Souligné (Ctrl+U).")
        End If
    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UC_RichTextEditorSimple_Load
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Configure le RichTextBox au chargement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_RichTextEditorSimple_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RichTextEditorHelper.ConfigurerRichTextBox(rtbEditor)
        DefinirMargesInterieures(rtbEditor, 8, 8)
    End Sub

#End Region

#Region "Gestionnaires d'événements Toolbar"

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

    Private Sub btnUndo_Click(sender As Object, e As EventArgs) Handles btnUndo.Click
        If rtbEditor.CanUndo Then rtbEditor.Undo()
        rtbEditor.Focus()
    End Sub

    Private Sub btnRedo_Click(sender As Object, e As EventArgs) Handles btnRedo.Click
        If rtbEditor.CanRedo Then rtbEditor.Redo()
        rtbEditor.Focus()
    End Sub

    Private Sub btnClearFormatting_Click(sender As Object, e As EventArgs) Handles btnClearFormatting.Click
        RichTextEditorHelper.EffacerFormatage(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnInsertDateTime_Click(sender As Object, e As EventArgs) Handles btnInsertDateTime.Click
        RichTextEditorHelper.InsererDateHeure(rtbEditor)
        rtbEditor.Focus()
    End Sub

#End Region

#Region "Gestionnaires d'événements RichTextBox"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : rtbEditor_TextChanged
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Propage ContentChanged au parent (ignoré pendant le chargement initial).
    ' -------------------------------------------------------------------------------------------------
    Private Sub rtbEditor_TextChanged(sender As Object, e As EventArgs) Handles rtbEditor.TextChanged
        If Not _isLoading Then
            RaiseEvent ContentChanged(Me, EventArgs.Empty)
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : MettreAJourEtatBoutons
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Met en surbrillance les boutons actifs selon la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub rtbEditor_SelectionChanged(sender As Object, e As EventArgs) Handles rtbEditor.SelectionChanged
        MettreAJourEtatBoutons()
    End Sub

    Private Sub MettreAJourEtatBoutons()
        If rtbEditor.SelectionFont Is Nothing Then Exit Sub

        Dim activeBtnColor As Color = Color.FromArgb(122, 155, 135)  ' ColorSauge
        Dim defaultBtnColor As Color = Color.FromArgb(178, 197, 186) ' ColorSaugeClair

        btnBold.BackColor = If(rtbEditor.SelectionFont.Bold, activeBtnColor, defaultBtnColor)
        btnItalic.BackColor = If(rtbEditor.SelectionFont.Italic, activeBtnColor, defaultBtnColor)
        btnUnderline.BackColor = If(rtbEditor.SelectionFont.Underline, activeBtnColor, defaultBtnColor)

        btnUndo.Enabled = rtbEditor.CanUndo
        btnRedo.Enabled = rtbEditor.CanRedo
    End Sub

#End Region

#Region "Raccourcis clavier"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : rtbEditor_KeyDown
    ' Version    : V1.0.0
    ' Date       : 10/06/2026
    '
    ' Rôle       : Gère Ctrl+B, Ctrl+I, Ctrl+U (complète les raccourcis natifs du RichTextBox).
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
            End Select
        End If
    End Sub

#End Region

End Class
