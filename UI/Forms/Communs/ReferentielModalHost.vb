' -------------------------------------------------------------------------------------------------
' Form        : ReferentielModalHost
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Hôte modal générique permettant d'afficher n'importe quel écran de référentiel (UC_Ref<X>,
' dérivé de UC_ReferentielBase) dans une fenêtre modale, sans créer d'UI dédiée par référentiel
' (D-Q17). Sert à l'ajout d'une valeur de référentiel « en contexte » depuis un bouton [+]
' placé près d'un combo (fiche patient, dossier, etc.).
'
' Responsabilités :
' - Héberger un UserControl de référentiel fourni, en Dock.Fill
' - Lui injecter un contexte UI dédié au modal (IContextAwareUserControl)
' - Présenter une fenêtre modale propre (titre, taille, centrage, icône héritée)
'
' Remarques   :
' - Form codée à la main (pas de Designer) : hôte volontairement minimal et générique
' - Le contrôle de droit et l'élévation éventuelle sont gérés EN AMONT par l'appelant
'   (Home.OuvrirReferentielModal), sur le modèle de Home.btnReferentiels_Click : l'hôte ne
'   duplique aucune logique de sécurité
' - Le contexte fourni à l'UC porte la session (pour les droits) mais des labels d'en-tête /
'   de statut à Nothing afin de NE PAS écraser le contexte de la fenêtre Home en arrière-plan ;
'   un ToolTip et un ErrorProvider propres au modal sont créés ici
'
' Dépendances :
' - UC_ReferentielBase (et ses dérivés UC_Ref<X>) hébergés dans la fenêtre
' - UserControlContext / IContextAwareUserControl (injection du contexte)
' - UserSession, UtilisateurApplication (session courante pour le contrôle des droits dans l'UC)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ReferentielModalHost
    Inherits Form

#Region "Variables privées"

    ' UserControl de référentiel hébergé dans la fenêtre (UC_Ref<X>).
    Private ReadOnly _vueReferentiel As UserControl

    ' Contexte UI dédié au modal, injecté à l'UC hébergé.
    Private ReadOnly _contexteModal As UserControlContext

    ' Composants UI propres au modal (le contexte de Home n'est pas réutilisé pour éviter les effets de bord).
    Private ReadOnly _toolTip As New ToolTip()
    Private ReadOnly _errorProvider As New ErrorProvider()

    ' Barre d'actions basse (bouton « Fermer ») permettant un retour propre à l'écran appelant
    ' lorsqu'aucun enregistrement n'est effectué (consultation / liste du référentiel).
    Private _pnlActions As Panel
    Private _btnFermer As Button

    ' Hauteur réservée à la barre d'actions basse (cohérente avec pnlActions de Login/ElevationAcces).
    Private Const HauteurBarreActions As Integer = 48

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 11/06/2026
    '
    ' Rôle         :
    ' Initialise l'hôte modal avec un écran de référentiel et la session courante.
    '
    ' Paramètres   :
    ' - vueReferentiel    : UserControl de référentiel à héberger (UC_Ref<X>, obligatoire)
    ' - titre             : Titre de la fenêtre modale (String)
    ' - userSession       : Session utilisateur courante (pour le contrôle des droits dans l'UC)
    ' - authenticatedUser : Utilisateur authentifié courant
    '
    ' Exceptions   :
    ' - ArgumentNullException : si vueReferentiel est Nothing
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(vueReferentiel As UserControl,
                   titre As String,
                   userSession As UserSession,
                   authenticatedUser As UtilisateurApplication)

        If vueReferentiel Is Nothing Then
            Throw New ArgumentNullException(NameOf(vueReferentiel))
        End If

        _vueReferentiel = vueReferentiel

        ' Contexte dédié au modal : session présente (droits), labels d'en-tête/statut à Nothing
        ' pour ne pas écraser le contexte de la fenêtre Home en arrière-plan.
        _contexteModal = New UserControlContext(
            statusLabel:=Nothing,
            toolTip:=_toolTip,
            errorProvider:=_errorProvider,
            headerLabel:=Nothing,
            userSession:=userSession,
            authenticatedUser:=authenticatedUser)

        ConfigurerFenetre(titre)
        HebergerVue()
        ConstruireBarreActions()

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ConfigurerFenetre
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Applique les propriétés visuelles de la fenêtre modale (titre, taille, centrage, bordure).
    '
    ' Paramètres :
    ' - titre : Titre affiché dans la barre de la fenêtre
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConfigurerFenetre(titre As String)

        Text = If(String.IsNullOrWhiteSpace(titre), "Référentiel", titre.Trim())
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.Sizable
        MinimizeBox = False
        MaximizeBox = True
        ShowInTaskbar = False

        ' Dimensionne la fenêtre d'après la taille réelle de la vue hébergée, afin que TOUS ses
        ' contrôles (notamment les boutons d'action en bas à droite) soient entièrement visibles.
        ' On réserve en plus la hauteur de la barre d'actions basse (bouton « Fermer »).
        Dim largeur As Integer = Math.Max(_vueReferentiel.Width, 720)
        Dim hauteur As Integer = Math.Max(_vueReferentiel.Height, 520) + HauteurBarreActions

        ' Borne la taille à la zone de travail de l'écran pour éviter tout débordement.
        Dim ecran As Screen = Screen.PrimaryScreen
        If ecran IsNot Nothing Then
            largeur = Math.Min(largeur, ecran.WorkingArea.Width - 40)
            hauteur = Math.Min(hauteur, ecran.WorkingArea.Height - 40)
        End If

        MinimumSize = New Drawing.Size(720, 520 + HauteurBarreActions)
        ClientSize = New Drawing.Size(largeur, hauteur)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : HebergerVue
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Injecte le contexte modal dans l'UC hébergé (s'il le supporte) puis l'ajoute en Dock.Fill.
    '
    ' Remarques  :
    ' - L'injection du contexte se fait AVANT l'ajout pour éviter un Load sans contexte
    '   (même précaution que NavigationManager.Navigate)
    ' -------------------------------------------------------------------------------------------------
    Private Sub HebergerVue()

        Dim contextAwareControl As IContextAwareUserControl =
            TryCast(_vueReferentiel, IContextAwareUserControl)

        If contextAwareControl IsNot Nothing Then
            contextAwareControl.SetContext(_contexteModal)
        End If

        ' En contexte modal (ajout « en passant » depuis un combo), on propose un retour
        ' automatique vers l'écran appelant dès qu'un enregistrement a réussi.
        Dim vueReferentielBase As UC_ReferentielBase =
            TryCast(_vueReferentiel, UC_ReferentielBase)

        If vueReferentielBase IsNot Nothing Then
            AddHandler vueReferentielBase.EnregistrementEffectue, AddressOf VueReferentiel_EnregistrementEffectue
        End If

        _vueReferentiel.Dock = DockStyle.Fill
        Controls.Add(_vueReferentiel)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ConstruireBarreActions
    ' Version    : V1.0.0
    ' Date       : 01/07/2026
    '
    ' Rôle       :
    ' Construit la barre d'actions basse (Dock.Bottom) contenant le bouton « Fermer », afin d'offrir
    ' un retour explicite et propre vers l'écran appelant lorsqu'aucun enregistrement n'est effectué
    ' (consultation / parcours de la liste du référentiel). L'enregistrement réussi continue de fermer
    ' automatiquement la fenêtre via VueReferentiel_EnregistrementEffectue.
    '
    ' Remarques  :
    ' - Doit être appelée APRÈS HebergerVue : la vue est en Dock.Fill (ajoutée en premier) et la barre
    '   en Dock.Bottom (ajoutée ensuite), conformément aux règles de z-order WinForms (cf. Login).
    ' - Le bouton réutilise le style standard sauge (UtilsButtons.InitStandardButton, Tag fermer_normal).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConstruireBarreActions()

        _pnlActions = New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = HauteurBarreActions,
            .BackColor = UITheme.ColorBeige,
            .Padding = New Padding(6, 7, 6, 7)
        }

        _btnFermer = New Button With {
            .Text = "Fermer",
            .Tag = "fermer_normal",
            .Size = New Drawing.Size(112, 34),
            .BackColor = UITheme.ColorSauge,
            .ForeColor = Color.White,
            .Font = New Font("Calibri", 10F, FontStyle.Bold),
            .ImageAlign = ContentAlignment.MiddleLeft,
            .TextAlign = ContentAlignment.MiddleRight,
            .TextImageRelation = TextImageRelation.ImageBeforeText,
            .Anchor = AnchorStyles.Top Or AnchorStyles.Right
        }

        UtilsButtons.InitStandardButton(_btnFermer)

        AddHandler _btnFermer.Click, AddressOf BtnFermer_Click

        _toolTip.SetToolTip(_btnFermer, "Fermer et revenir à l'écran précédent.")

        _pnlActions.Controls.Add(_btnFermer)
        Controls.Add(_pnlActions)

        ' Positionne le bouton à droite : maintenant que le panneau est docké (largeur réelle connue),
        ' puis à chaque redimensionnement (fenêtre Sizable / maximisée).
        PositionnerBoutonFermer()
        AddHandler _pnlActions.SizeChanged, AddressOf PnlActions_SizeChanged

        ' Bouton « Fermer » par défaut pour la touche Échap.
        CancelButton = _btnFermer

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : PositionnerBoutonFermer
    ' Version    : V1.0.0
    ' Date       : 01/07/2026
    '
    ' Rôle       :
    ' Aligne le bouton « Fermer » sur le bord droit de la barre d'actions, centré verticalement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub PositionnerBoutonFermer()

        If _pnlActions Is Nothing OrElse _btnFermer Is Nothing Then Return

        _btnFermer.Location = New Point(
            _pnlActions.ClientSize.Width - _btnFermer.Width - _pnlActions.Padding.Right,
            (_pnlActions.ClientSize.Height - _btnFermer.Height) \ 2)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : PnlActions_SizeChanged
    ' Version    : V1.0.0
    ' Date       : 01/07/2026
    '
    ' Rôle       :
    ' Repositionne le bouton « Fermer » lorsque la barre d'actions change de taille (redimensionnement
    ' de la fenêtre modale).
    ' -------------------------------------------------------------------------------------------------
    Private Sub PnlActions_SizeChanged(sender As Object, e As EventArgs)

        PositionnerBoutonFermer()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : BtnFermer_Click
    ' Version    : V1.0.0
    ' Date       : 01/07/2026
    '
    ' Rôle       :
    ' Ferme la fenêtre modale sur demande explicite de l'utilisateur (retour à l'écran appelant),
    ' sans signaler d'enregistrement (DialogResult.Cancel).
    ' -------------------------------------------------------------------------------------------------
    Private Sub BtnFermer_Click(sender As Object, e As EventArgs)

        DialogResult = DialogResult.Cancel
        Close()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : VueReferentiel_EnregistrementEffectue
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Rôle       :
    ' Ferme automatiquement la fenêtre modale après un enregistrement réussi dans le référentiel,
    ' afin de ramener l'utilisateur sur l'écran appelant (ex. fiche patient) sans manipulation.
    '
    ' Remarques  :
    ' - Ce comportement est propre à l'usage modal « en contexte » : l'écran de référentiel
    '   ouvert depuis AdminHome ne passe pas par cet hôte et reste donc inchangé.
    ' -------------------------------------------------------------------------------------------------
    Private Sub VueReferentiel_EnregistrementEffectue(sender As Object, e As EventArgs)

        DialogResult = DialogResult.OK
        Close()

    End Sub

#End Region

#Region "Nettoyage"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnFormClosed
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Libère les composants propres au modal à la fermeture de la fenêtre.
    '
    ' Paramètres :
    ' - e : Arguments de l'événement de fermeture
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

        Dim vueReferentielBase As UC_ReferentielBase =
            TryCast(_vueReferentiel, UC_ReferentielBase)

        If vueReferentielBase IsNot Nothing Then
            RemoveHandler vueReferentielBase.EnregistrementEffectue, AddressOf VueReferentiel_EnregistrementEffectue
        End If

        If _btnFermer IsNot Nothing Then
            RemoveHandler _btnFermer.Click, AddressOf BtnFermer_Click
        End If

        If _pnlActions IsNot Nothing Then
            RemoveHandler _pnlActions.SizeChanged, AddressOf PnlActions_SizeChanged
        End If

        _toolTip.Dispose()
        _errorProvider.Dispose()

        MyBase.OnFormClosed(e)

    End Sub

#End Region

End Class
