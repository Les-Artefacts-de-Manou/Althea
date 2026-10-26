' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaZoneTexte
' Projet     : Althéa
' Version    : V1.0.1
' Date       : 22/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Zone de texte simple du kit Althéa avec le correcteur d'orthographe de Windows : pour les notes
' courtes enregistrées en texte (commentaire d'un paiement, motif d'une annulation).
'
' Responsabilités :
' - Texte sans mise en forme : collage en texte seul, raccourcis de mise en forme du contrôle
'   neutralisés (Ctrl+L / E / R / J, Ctrl+1 / 2 / 5, Ctrl+I qui insère une tabulation…).
' - Correcteur d'orthographe : mots inconnus soulignés pendant la frappe (langue du clavier),
'   clic droit avec suggestions et « Ajouter au dictionnaire », comme dans l'éditeur de texte.
' - Menu du clic droit du kit : Couper, Copier, Coller, Tout sélectionner.
'
' Remarques  :
' - Décision de Joëlle (20/10/2026, recommandation acceptée) : ces champs restent en texte simple
'   (le commentaire d'un paiement était alors complété par Althéa : « [Annulé le … : motif] »,
'   utilisation d'un avoir) ; ils reçoivent seulement le correcteur. Depuis le 22/10/2026 (audit B.5,
'   ADR-49), annulation et utilisation d'avoir ont leurs colonnes : Althéa n'écrit plus dans le
'   commentaire ; le texte simple reste le bon choix pour une note courte.
' - Hérite de RichTextBox (le correcteur de Windows n'existe pas dans une TextBox) ; Text,
'   TextChanged, ReadOnly, BackColor et le code des écrans restent inchangés : la migration d'un
'   champ se limite au type déclaré. GardeModifications et UtilsControls le lisent comme une
'   zone de texte (TextBoxBase).
' - AcceptsReturn (faux par défaut, comme une TextBox) : Entrée déclenche le bouton par défaut de
'   la fenêtre ; Maj+Entrée passe à la ligne.
' - Correcteur actif en saisie seulement, si le réglage EDITEUR_CORRECTEUR_ORTHOGRAPHE le permet
'   (CorrecteurTexte, partagé avec UC_RichTextEditor).
' - Les raccourcis de la fenêtre (Ctrl+S…) passent avant ceux du champ : ils ne sont pas touchés.
'
' Dépendances :
' - CorrecteurTexte, RichTextEditorHelper (collage en texte seul), AltheaRenduBarre (menu)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Zone de texte Althéa : texte simple avec correcteur d'orthographe de Windows.")>
Public Class AltheaZoneTexte
    Inherits RichTextBox

#Region "Variables privées"

    Private ReadOnly _menu As New ContextMenuStrip()
    Private ReadOnly _policeMenu As New Font("Calibri", 10.5F)
    Private ReadOnly _imagesMenu As New List(Of Image)()
    Private ReadOnly mnuCouper As ToolStripMenuItem
    Private ReadOnly mnuCopier As ToolStripMenuItem
    Private ReadOnly mnuColler As ToolStripMenuItem
    Private ReadOnly _lignesCorrecteur As CorrecteurTexte

    Private _correcteur As Boolean = True
    Private _correcteurPermis As Boolean
    Private _reglageLu As Boolean
    Private _accepteRetour As Boolean

#End Region

#Region "Initialisation"

    Public Sub New()

        DetectUrls = False

        _menu.Renderer = New AltheaRenduBarre()
        _menu.Font = _policeMenu
        _menu.ImageScalingSize = New Size(20, 20)

        mnuCouper = LigneMenu("Couper", "Ctrl+X", My.Resources.Editeur_couper, Sub() Cut())
        mnuCopier = LigneMenu("Copier", "Ctrl+C", My.Resources.Editeur_copier, Sub() Copy())
        mnuColler = LigneMenu("Coller", "Ctrl+V", My.Resources.Editeur_coller, Sub() RichTextEditorHelper.CollerSansMiseEnForme(Me))
        Dim mnuToutSelectionner As ToolStripMenuItem = LigneMenu("Tout sélectionner", "Ctrl+A", Nothing, Sub() SelectAll())

        _menu.Items.AddRange(New ToolStripItem() {mnuCouper, mnuCopier, mnuColler, New ToolStripSeparator(), mnuToutSelectionner})
        AddHandler _menu.Opening, AddressOf Menu_Opening
        ContextMenuStrip = _menu

        _lignesCorrecteur = New CorrecteurTexte(Me, _menu, Sub(texte) SelectedText = texte)

    End Sub

    Private Function LigneMenu(texte As String, raccourci As String, image As Image, action As Action) As ToolStripMenuItem

        If image IsNot Nothing Then _imagesMenu.Add(image)

        Dim ligne As New ToolStripMenuItem(texte) With {
            .Image = image,
            .ShortcutKeyDisplayString = raccourci
        }
        AddHandler ligne.Click, Sub(s, e)
                                    action()
                                    Focus()
                                End Sub
        Return ligne

    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)

        If disposing Then
            _lignesCorrecteur.Dispose()
            _menu.Dispose()
            _policeMenu.Dispose()
            For Each image As Image In _imagesMenu
                image.Dispose()
            Next
            _imagesMenu.Clear()
        End If

        MyBase.Dispose(disposing)

    End Sub

#End Region

#Region "Propriétés"

    <Category("Althéa")>
    <DefaultValue(True)>
    <Description("Correcteur d'orthographe de Windows (soulignement pendant la frappe, langue du clavier).")>
    Public Property CorrecteurOrthographe As Boolean
        Get
            Return _correcteur
        End Get
        Set(value As Boolean)
            _correcteur = value
            AppliquerCorrecteur()
        End Set
    End Property

    ' Même nom et même règle que la TextBox : faux, Entrée déclenche le bouton par défaut.
    <Category("Comportement")>
    <DefaultValue(False)>
    <Description("Entrée passe à la ligne au lieu de déclencher le bouton par défaut de la fenêtre.")>
    Public Property AcceptsReturn As Boolean
        Get
            Return _accepteRetour
        End Get
        Set(value As Boolean)
            _accepteRetour = value
        End Set
    End Property

    ' Texte posé par le code : il prend la langue du clavier, comme le texte tapé.
    Public Overrides Property Text As String
        Get
            Return MyBase.Text
        End Get
        Set(value As String)
            MyBase.Text = value
            If CorrecteurActif AndAlso IsHandleCreated Then CorrecteurTexte.Appliquer(Me, True)
        End Set
    End Property

#End Region

#Region "Correcteur d'orthographe"

    ' Correcteur réellement actif : demandé, permis par le réglage, et texte en saisie.
    Private ReadOnly Property CorrecteurActif As Boolean
        Get
            Return _correcteur AndAlso _correcteurPermis AndAlso Not [ReadOnly]
        End Get
    End Property

    Private ReadOnly Property EnConception As Boolean
        Get
            Return DesignMode OrElse LicenseManager.UsageMode = LicenseUsageMode.Designtime
        End Get
    End Property

    ' Réglage lu une fois, à la première création du handle (pas dans le Designer).
    Protected Overrides Sub OnHandleCreated(e As EventArgs)

        MyBase.OnHandleCreated(e)

        If Not _reglageLu AndAlso Not EnConception Then
            _correcteurPermis = CorrecteurTexte.ReglagePermis()
            _reglageLu = True
        End If

        AppliquerCorrecteur()

    End Sub

    Protected Overrides Sub OnReadOnlyChanged(e As EventArgs)
        MyBase.OnReadOnlyChanged(e)
        AppliquerCorrecteur()
    End Sub

    Private Sub AppliquerCorrecteur()
        If EnConception OrElse Not IsHandleCreated Then Return
        CorrecteurTexte.Appliquer(Me, CorrecteurActif)
    End Sub

#End Region

#Region "Clavier"

    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        If keyData = Keys.Enter AndAlso Not _accepteRetour Then Return False
        Return MyBase.IsInputKey(keyData)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnKeyDown
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Coller en texte seul (Ctrl+V, Maj+Inser) ; raccourcis de mise en forme du contrôle
    '              neutralisés (le texte est enregistré sans mise en forme).
    ' Remarques  : Traité ici et non dans ProcessCmdKey : les raccourcis de la fenêtre passent avant.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        MyBase.OnKeyDown(e)
        If e.Handled Then Return

        If e.KeyData = (Keys.Control Or Keys.V) OrElse e.KeyData = (Keys.Shift Or Keys.Insert) Then
            If Not [ReadOnly] Then RichTextEditorHelper.CollerSansMiseEnForme(Me)
            e.Handled = True
            e.SuppressKeyPress = True
        ElseIf EstRaccourciDeMiseEnForme(e.KeyData) Then
            e.Handled = True
            e.SuppressKeyPress = True
        End If

    End Sub

    ' Ctrl + touche (sans Alt : Alt Gr = Ctrl+Alt sert à taper @, €…), hors édition et déplacement.
    Private Shared Function EstRaccourciDeMiseEnForme(keyData As Keys) As Boolean

        If (keyData And Keys.Control) = 0 OrElse (keyData And Keys.Alt) <> 0 Then Return False

        Select Case keyData And Keys.KeyCode
            Case Keys.A, Keys.C, Keys.X, Keys.Z, Keys.Y,
                 Keys.Back, Keys.Delete, Keys.Insert, Keys.Return, Keys.Tab,
                 Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Home, Keys.End, Keys.PageUp, Keys.PageDown,
                 Keys.ControlKey, Keys.ShiftKey
                Return False
            Case Else
                Return True
        End Select

    End Function

#End Region

#Region "Menu du clic droit"

    ' Couper / coller seulement en saisie ; actions grisées sans sélection ou presse-papiers vide.
    Private Sub Menu_Opening(sender As Object, e As CancelEventArgs)

        Dim edition As Boolean = Not [ReadOnly]
        Dim presseTexte As Boolean = False

        Try
            presseTexte = Clipboard.ContainsText()
        Catch ex As Runtime.InteropServices.ExternalException
            ' Presse-papiers occupé par une autre application : Coller reste grisé.
        End Try

        mnuCouper.Available = edition
        mnuColler.Available = edition

        mnuCouper.Enabled = SelectionLength > 0
        mnuCopier.Enabled = SelectionLength > 0
        mnuColler.Enabled = presseTexte

        _lignesCorrecteur.Preparer(CorrecteurActif)

    End Sub

#End Region

End Class
