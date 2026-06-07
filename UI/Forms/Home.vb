' -------------------------------------------------------------------------------------------------
' Form       : Home
' Projet     : Althéa
' Version    : V1.6
' Date       : 04/06/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form principale et Shell applicatif de l''application Althéa.
' Centralise l''authentification, la navigation, les composants UI partagés, et l''hébergement
' des UserControls métier dans une zone de contenu dynamique.
'
' Responsabilités :
' - Gérer l''authentification initiale via Login.vb au démarrage
' - Gérer le changement de mot de passe obligatoire (MustChangePassword)
' - Créer et maintenir la session utilisateur (UserSession) pendant toute la durée de vie de l''application
' - Héberger les composants UI partagés (StatusStrip, ToolTip, ErrorProvider, lblContexte)
' - Fournir un contexte UI partagé (UserControlContext) à tous les UserControls via IContextAwareUserControl
' - Gérer le menu principal (pnlMenu) avec état visuel des boutons (normal/hover/sélectionné)
' - Charger dynamiquement les UserControls métier dans pnlContent via NavigationManager
' - Afficher le contexte de navigation hiérarchique (lblContexte : "Accueil", "Administration > Paramètres", etc.)
' - Gérer l''élévation temporaire de privilèges pour accéder aux zones réservées (Administration)
' - Afficher l''utilisateur connecté, son rôle courant, et l''état d''élévation dans le header (lblUtilisateurConnecte)
' - Synchroniser le menu, le contenu, et le StatusStrip lors de chaque navigation
' - Journaliser les actions de navigation et d''élévation via GestionLog
' - Permettre la navigation de retour depuis les UserControls enfants (NavigateToAccueil, NavigateToAdminView)
'
' Remarques  :
' - Home est le nœud central de l''application : tous les UserControls métier sont chargés dans pnlContent
' - Home ne contient aucune logique métier ni accès direct à la base de données
' - L''architecture est organisée en panneaux :
'   * pnlMenu (gauche)    : boutons de navigation principale
'   * pnlHeader (haut)    : contexte utilisateur (lblUtilisateurConnecte) et contexte de navigation (lblContexte)
'   * pnlContent (centre) : zone dynamique où sont chargés les UserControls
'   * stsStatus (bas)     : StatusStrip partagé (stsLabelStatus)
' - Le mécanisme de navigation est centralisé via NavigationManager (instancié dans Home_Load)
' - Les UserControls implémentant IContextAwareUserControl reçoivent automatiquement le contexte via SetContext()
' - L''élévation de privilèges est temporaire et limitée à la session en cours (gérée par UserSession)
' - Le DialogResult n''est jamais utilisé (Home est la Form principale, pas une modale)
' - Tous les UserControls naviguent en appelant des méthodes publiques de Home (NavigateToAccueil, NavigateToAdminView)
'
' Dépendances :
' - Login (authentification initiale au démarrage)
' - ChangePassword (changement de mot de passe obligatoire)
' - ElevationAcces (élévation temporaire de privilèges)
' - UserSession (session utilisateur, rôle courant, état d''élévation)
' - UtilisateurApplication (utilisateur authentifié)
' - UserControlContext (contexte UI partagé pour les UserControls)
' - NavigationManager (chargement centralisé des UserControls)
' - UC_Accueil (UserControl d''accueil chargé par défaut)
' - UC_AdminHome (UserControl administration)
' - UtilsButtons (thématisation et gestion des boutons du menu)
' - GestionLog (journalisation des actions de navigation et d''élévation)
'
' Imports    :
' - 
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Home

#Region "Variables privées"

    ' Liste centralisée des boutons du menu principal (pnlMenu).
    Private ReadOnly _homeMenuButtons As New List(Of Button)

    ' Gestionnaire de navigation centralisé pour charger les UserControls métier dans pnlContent.
    Private _navManager As NavigationManager

    ' Session utilisateur courante maintenant l'état d'authentification, le rôle actif, et l'élévation temporaire.
    Private _userSession As UserSession

    ' Contexte UI partagé fourni à tous les UserControls via IContextAwareUserControl.SetContext().
    Private _uiContext As UserControlContext

    ' Utilisateur authentifié retourné par Login.vb au démarrage de l'application.
    Private _authenticatedUser As UtilisateurApplication

#End Region

#Region "Propriétés publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AuthenticatedUser
    ' Type       : UtilisateurApplication (ReadOnly)
    ' Version    : V1.0.0
    ' Date       : 12/05/2026
    '
    ' Rôle       :
    ' Retourne l'utilisateur authentifié courant récupéré depuis Login.vb au démarrage.
    '
    ' Retour     :
    ' - UtilisateurApplication : Instance de l'utilisateur authentifié avec toutes ses propriétés
    '
    ' Remarques  :
    ' - Propriété ReadOnly exposée publiquement pour permettre aux UserControls d'accéder aux informations utilisateur
    ' - Utilisée notamment par UC_AdminHome pour passer l'utilisateur authentifié lors de l'élévation
    ' - Ne retourne jamais Nothing après authentification réussie (Home se ferme si login échoue)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AuthenticatedUser As UtilisateurApplication
        Get
            Return _authenticatedUser
        End Get
    End Property

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Home_Load
    ' Version    : V1.5.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Initialise la Form Home : authentification, session utilisateur, contexte UI partagé,
    ' changement de mot de passe obligatoire, et chargement du UserControl d'accueil par défaut.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement Load
    '
    ' Remarques  :
    ' - Séquence d'initialisation :
    '   1. Initialisation des boutons du menu principal (InitializeHomeMenuButtons)
    '   2. Affichage de Login.vb en modal et authentification utilisateur
    '   3. Si authentification échouée (DialogResult <> OK) : fermeture de l'application
    '   4. Création de la UserSession avec les informations de l'utilisateur authentifié
    '   5. Affichage de l'utilisateur connecté dans le header (UpdateConnectedUserDisplay)
    '   6. Création du contexte UI partagé (UserControlContext) pour injection dans les UserControls
    '   7. Si MustChangePassword = True : affichage de ChangePassword.vb en modal
    '   8. Si changement de mot de passe refusé : fermeture de l'application avec log sécurité
    '   9. Initialisation du NavigationManager avec pnlContent et _uiContext
    '   10. Chargement de UC_Accueil par défaut avec bouton Accueil sélectionné
    ' - Les boutons du menu Home ont un comportement spécifique : normal / hover / sélectionné
    ' - Le bouton sélectionné reste visuellement actif jusqu'au choix d'un autre menu
    ' - Home est la Form principale : si elle se ferme, l'application se termine
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées au niveau global)
    ' -------------------------------------------------------------------------------------------------
    Private Sub Home_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Initialisation du menu principal
        InitializeHomeMenuButtons()

        ' Connexion utilisateur
        Dim authenticatedUser As UtilisateurApplication

        Using frmLogin As New Login()

            Dim result As DialogResult =
        frmLogin.ShowDialog(Me)

            If result <> DialogResult.OK Then

                Close()

                Return

            End If

            authenticatedUser =
        frmLogin.AuthenticatedUser

        End Using

        _authenticatedUser =
    authenticatedUser

        ' Création session utilisateur
        _userSession = New UserSession(
    authenticatedUser.LoginUtilisateur,
    authenticatedUser.NomAffichage,
    authenticatedUser.RoleUtilisateur
)
        ' Affichage de l'utilisateur connecté dans le header
        UpdateConnectedUserDisplay()

        ' Initialisation du contexte partagé pour les UserControls
        _uiContext = New UserControlContext(
            stsLabelStatus,
            ttMain,
            errProvider,
            lblContexte,
            _userSession,
            _authenticatedUser
        )

        ' Demande de changement de mot de passe 
        If authenticatedUser.MustChangePassword Then

            Using frmChangePassword As New ChangePassword(
        authenticatedUser)

                Dim result As DialogResult =
            frmChangePassword.ShowDialog(Me)

                If result <> DialogResult.OK Then

                    GestionLog.EcrireLog(
                $"Application fermée : changement mot de passe obligatoire refusé ({authenticatedUser.LoginUtilisateur}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security
            )
                    Close()

                    Return

                End If

            End Using

        End If

        ' Initialisation du gestionnaire de navigation centralisé
        _navManager = New NavigationManager(pnlContent, _uiContext)
        NavigateTo(New UC_Accueil(), btnAccueil, "Accueil")

        '  _uiContext.SetStatus("Application prête")
        '  _uiContext.SetHeader("Accueil")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateConnectedUserDisplay
    ' Version    : V1.1.0
    ' Date       : 12/05/2026
    '
    ' Rôle       :
    ' Met à jour l'affichage de l'utilisateur connecté dans le header (lblUtilisateurConnecte)
    ' avec le nom, le rôle courant, et l'état d'élévation temporaire.
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée dans Home_Load après création de la UserSession
    ' - Appelée après chaque élévation de privilèges (btnAdmin_Click) pour refléter le changement de rôle
    ' - Appelée par UC_AdminHome après retour au rôle de base (btnRetourRoleBase_Click)
    ' - Format d'affichage : "Connecté : [DisplayName] | Rôle : [CurrentRole] | Élévation active"
    ' - Si _userSession est Nothing (cas anormal) : affiche "Connecté : -" sans erreur
    ' - lblUtilisateurConnecte est un Label dans pnlHeader, visible en permanence
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateConnectedUserDisplay()

        If _userSession Is Nothing Then

            lblUtilisateurConnecte.Text =
            "Connecté : -"

            Return

        End If

        Dim texte As String =
        $"Connecté : {_userSession.DisplayName} | Rôle : {_userSession.CurrentRole}"

        If _userSession.IsElevated Then

            texte &= " | Élévation active"

        End If

        lblUtilisateurConnecte.Text =
        texte

    End Sub

#End Region

#Region "Initialisation UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitializeHomeMenuButtons
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Initialise la liste centralisée des boutons du menu principal (_homeMenuButtons)
    ' et applique leur style standard via UtilsButtons.InitHomeMenuButton().
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée dans Home_Load avant l'authentification
    ' - Ajoute les 7 boutons du menu principal : btnAccueil, btnPatients, btnDomaines, btnAgenda,
    '   btnDocuments, btnReferentiels, btnAdmin
    ' - Chaque bouton doit avoir un Tag défini dans le Designer pour le système de navigation
    ' - Exemples de Tags : "accueil", "patients", "domaines", "agenda", "documents", "referentiels", "admin"
    ' - UtilsButtons.InitHomeMenuButton() configure les styles normal/hover/sélectionné pour chaque bouton
    ' - La liste _homeMenuButtons est utilisée par SetSelectedHomeMenuButton() pour gérer l'état visuel
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeHomeMenuButtons()

        _homeMenuButtons.Clear()

        _homeMenuButtons.AddRange(New Button() {
        btnAccueil,
        btnPatients,
        btnDomaines,
        btnAgenda,
        btnDocuments,
        btnReferentiels,
        btnAdmin
    })

        For Each btn As Button In _homeMenuButtons
            UtilsButtons.InitHomeMenuButton(btn)
        Next

    End Sub

#End Region

#Region "Navigation menu principal"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SelectHomeMenuButton
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Définit le bouton actif du menu principal, met à jour l'état visuel, affiche le statut,
    ' et journalise la sélection.
    '
    ' Paramètres :
    ' - selectedButton : Bouton à marquer comme sélectionné (Button)
    ' - statusText     : Texte à afficher dans la barre de statut (String)
    '
    ' Remarques  :
    ' - Appelée par NavigateTo() et par les handlers de clic simples (btnPatients_Click, etc.)
    ' - Utilise UtilsButtons.SetSelectedHomeMenuButton() pour gérer l'état visuel de tous les boutons
    ' - Le bouton sélectionné reste visuellement actif jusqu'au choix d'un autre menu
    ' - Met à jour stsLabelStatus.Text avec le contexte fourni
    ' - Journalise la sélection dans GestionLog (niveau Rapide, catégorie UI)
    ' - Ne charge pas de UserControl (c'est le rôle de NavigateTo())
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectHomeMenuButton(selectedButton As Button, statusText As String)

        UtilsButtons.SetSelectedHomeMenuButton(selectedButton, _homeMenuButtons)

        stsLabelStatus.Text = statusText

        GestionLog.EcrireLog(
        $"Menu Home sélectionné : {statusText}",
        GestionLog.LogLevel.Rapide,
        GestionLog.LogCategory.UI)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAccueil_Click
    ' Version    : V1.1.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Retourne à l'accueil via NavigateToAccueil().
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Utilisé pour revenir à l'écran d'accueil depuis n'importe quelle vue
    ' - NavigateToAccueil() charge UC_Accueil, sélectionne btnAccueil, et met à jour le contexte
    ' - Utilisé notamment après une perte d'accès à une zone réservée
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAccueil_Click(
    sender As Object,
    e As EventArgs
) Handles btnAccueil.Click

        NavigateToAccueil()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnPatients_Click
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Active l'environnement Patients (en attente d'implémentation complète).
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Actuellement : sélectionne uniquement le bouton et met à jour le statut
    ' - À terme : devra charger le UserControl de gestion des patients via NavigateTo()
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnPatients_Click(sender As Object, e As EventArgs) Handles btnPatients.Click

        SelectHomeMenuButton(btnPatients, "Patients")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnDomaines_Click
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Active l'environnement Domaines (en attente d'implémentation complète).
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Actuellement : sélectionne uniquement le bouton et met à jour le statut
    ' - À terme : devra charger le UserControl de gestion des domaines via NavigateTo()
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnDomaines_Click(sender As Object, e As EventArgs) Handles btnDomaines.Click

        SelectHomeMenuButton(btnDomaines, "Domaines")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAgenda_Click
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Active l'environnement Agenda (en attente d'implémentation complète).
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Actuellement : sélectionne uniquement le bouton et met à jour le statut
    ' - À terme : devra charger le UserControl de gestion de l'agenda via NavigateTo()
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAgenda_Click(sender As Object, e As EventArgs) Handles btnAgenda.Click

        SelectHomeMenuButton(btnAgenda, "Agenda")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnDocuments_Click
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Active l'environnement Documents (en attente d'implémentation complète).
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Actuellement : sélectionne uniquement le bouton et met à jour le statut
    ' - À terme : devra charger le UserControl de gestion des documents via NavigateTo()
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnDocuments_Click(sender As Object, e As EventArgs) Handles btnDocuments.Click

        SelectHomeMenuButton(btnDocuments, "Documents")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnReferentiels_Click
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Active l'environnement Référentiels (en attente d'implémentation complète).
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Actuellement : sélectionne uniquement le bouton et met à jour le statut
    ' - À terme : devra charger le UserControl de gestion des référentiels via NavigateTo()
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReferentiels_Click(sender As Object, e As EventArgs) Handles btnReferentiels.Click

        SelectHomeMenuButton(btnReferentiels, "Référentiels")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAdmin_Click
    ' Version    : V1.5.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Ouvre l'espace Administration (UC_AdminHome) avec contrôle d'accès et élévation de privilèges si nécessaire.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Vérifie d'abord si l'utilisateur a les droits d'accès via PeutAccederAdminHome()
    ' - Si droits insuffisants : propose une élévation temporaire de privilèges via ElevationAcces
    ' - Si l'utilisateur refuse l'élévation : annulation avec message de statut
    ' - Si l'élévation échoue : retour sans chargement de UC_AdminHome
    ' - Après élévation réussie : mise à jour de l'affichage utilisateur (UpdateConnectedUserDisplay)
    ' - Charge UC_AdminHome via NavigateTo() avec le contexte "Administration"
    ' - En cas d'erreur : log (Succinct/UI) + MessageBox d'erreur
    '
    ' Exceptions :
    ' - Toutes les exceptions sont capturées, journalisées, et affichées à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAdmin_Click(
    sender As Object,
    e As EventArgs
) Handles btnAdmin.Click

        Try

            If Not PeutAccederAdminHome() Then

                Dim response As DialogResult =
                DialogChoix.Confirmer(
                    "Cette zone nécessite une élévation de privilèges." &
                    Environment.NewLine &
                    Environment.NewLine &
                    "Souhaitez-vous ouvrir une session élevée ?",
                    "Élévation requise"
                )

                If response = DialogResult.No Then

                    stsLabelStatus.Text =
                    "Accès Administration annulé."

                    Return

                End If

                Using frmElevation As New ElevationAcces(
                _userSession,
                _authenticatedUser
            )

                    Dim elevationResult As DialogResult =
                    frmElevation.ShowDialog(Me)

                    If elevationResult <> DialogResult.OK Then

                        stsLabelStatus.Text =
                        "Élévation refusée."

                        Return

                    End If

                End Using

                UpdateConnectedUserDisplay()

            End If

            NavigateTo(
            New UC_AdminHome(_userSession, _authenticatedUser),
            btnAdmin,
            "Administration"
        )

        Catch ex As Exception

            GestionLog.EcrireLog(
            "Erreur btnAdmin_Click.",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.UI,
            ex
        )

            DialogChoix.Erreur(
            "Erreur lors de l'ouverture de l'administration.",
            "Erreur"
        )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnChangerMotDePasse_Click
    ' Version    : V1.0.0
    ' Date       : 01/06/2026
    '
    ' Rôle       :
    ' Ouvre la Form ChangePassword en mode UserChange pour permettre à l'utilisateur de changer son mot de passe.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Accessible à tous les utilisateurs authentifiés
    ' - N'affecte pas la navigation (pas de sélection de bouton menu)
    ' - En cas d'erreur : log (Succinct/UI) + MessageBox
    '
    ' Exceptions :
    ' - Toutes les exceptions sont capturées et journalisées
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnChangerMotDePasse_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnChangerMotDePasse.Click

        Try

            Using frmChangePassword As New ChangePassword(
                _authenticatedUser,
                ModeChangePassword.UserChange
            )

                frmChangePassword.ShowDialog(Me)

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur btnChangerMotDePasse_Click.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Information(
                "Erreur lors de l'ouverture du changement de mot de passe.",
                "Erreur"
            )

        End Try

    End Sub

#End Region

#Region "Navigation - Chargement des vues"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateTo
    ' Version    : V1.3.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Navigue vers un UserControl métier et synchronise le menu, le contenu (pnlContent),
    ' le contexte de navigation (lblContexte), et le StatusStrip.
    '
    ' Paramètres :
    ' - view           : UserControl à charger dans pnlContent (UserControl)
    ' - selectedButton : Bouton du menu à marquer comme sélectionné (Button)
    ' - statusText     : Texte de contexte à afficher dans lblContexte et stsLabelStatus (String)
    '
    ' Remarques  :
    ' - Séquence de navigation :
    '   1. SelectHomeMenuButton(selectedButton, statusText) : mise à jour de l'état visuel du menu
    '   2. _navManager.Navigate(view) : chargement du UserControl dans pnlContent (décharge l'ancien, charge le nouveau)
    '   3. SetContexte(statusText) : mise à jour de lblContexte et stsLabelStatus
    ' - Le contexte est appliqué APRES la navigation pour éviter qu'un UserControl ne l'écrase pendant son chargement
    ' - _navManager injecte automatiquement _uiContext aux UserControls implémentant IContextAwareUserControl
    ' - Utilisée par tous les handlers de navigation : btnAccueil_Click, btnAdmin_Click, NavigateToAccueil, etc.
    ' - Méthode centrale de navigation : toute navigation doit passer par ici pour garantir la cohérence UI
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigationManager)
    ' -------------------------------------------------------------------------------------------------
    Private Sub NavigateTo(
    view As UserControl,
    selectedButton As Button,
    statusText As String
)

        SelectHomeMenuButton(
        selectedButton,
        statusText
    )

        _navManager.Navigate(view)

        SetContexte(statusText)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateToAdminView
    ' Version    : V1.1.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Navigue vers un écran d'administration (sous-UserControl de UC_AdminHome) avec contexte hiérarchique.
    '
    ' Paramètres :
    ' - view       : UserControl d'administration à charger (ex: UC_Parametres) (UserControl)
    ' - statusText : Texte du sous-contexte (ex: "Paramètres") (String)
    '
    ' Remarques  :
    ' - Appelée publiquement par les UserControls d'administration (ex: UC_AdminHome.btnParametres_Click)
    ' - Construit un contexte hiérarchique via BuildAdminContexte() : "Administration > [statusText]"
    ' - Navigue via NavigateTo() avec btnAdmin sélectionné pour maintenir la cohérence du menu
    ' - Permet de naviguer entre les différents écrans d'administration sans repasser par UC_AdminHome
    ' - Le contexte hiérarchique indique clairement la position dans l'arborescence : "Administration > Connexion DB", etc.
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigateTo)
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateToAdminView(
    view As UserControl,
    statusText As String
)

        Dim contexte As String =
        BuildAdminContexte(statusText)

        NavigateTo(
        view,
        btnAdmin,
        contexte
    )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateToAccueil
    ' Version    : V1.0.0
    ' Date       : 12/05/2026
    '
    ' Rôle       :
    ' Retourne vers l'écran d'accueil (UC_Accueil) avec contexte "Accueil".
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée publiquement par les UserControls pour revenir à l'accueil
    ' - Utilisée notamment après perte d'accès à un écran réservé (ex: UC_AdminHome après retour au rôle de base)
    ' - Navigue via NavigateTo() avec btnAccueil sélectionné et contexte "Accueil"
    ' - Charge une nouvelle instance de UC_Accueil à chaque appel (pas de réutilisation)
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigateTo)
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateToAccueil()

        NavigateTo(
        New UC_Accueil(),
        btnAccueil,
        "Accueil"
    )

    End Sub

#End Region

#Region "Gestion des accès"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutAccederAdminHome
    ' Type       : Boolean
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Vérifie si l'utilisateur courant possède les droits d'accès à l'espace Administration (UC_AdminHome).
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Retour     :
    ' - True : Si le rôle courant est SuperUser ou Admin (via _userSession.IsSuperUserOrAdmin())
    ' - False : Si _userSession est Nothing ou si le rôle est insuffisant
    '
    ' Remarques  :
    ' - Appelée dans btnAdmin_Click avant de charger UC_AdminHome
    ' - Si retourne False : propose une élévation de privilèges via ElevationAcces
    ' - Vérifie le rôle COURANT (_userSession.CurrentRole), pas le rôle de base
    ' - Après élévation réussie, retournera True car CurrentRole aura changé
    ' - La vérification passe par UserSession.IsSuperUserOrAdmin() qui utilise l'enum AppRole
    '
    ' Exceptions :
    ' - Aucune (retourne False en cas de session invalide)
    ' -------------------------------------------------------------------------------------------------
    Private Function PeutAccederAdminHome() As Boolean

        If _userSession Is Nothing Then

            Return False

        End If

        Return _userSession.IsSuperUserOrAdmin()

    End Function

#End Region

#Region "Gestion du contexte"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : BuildAdminContexte
    ' Type       : String
    ' Version    : V1.1.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Construit un libellé de contexte hiérarchique pour les écrans issus de l'administration.
    '
    ' Paramètres :
    ' - sousContexte : Nom du sous-écran d'administration (ex: "Paramètres", "Connexion DB") (String)
    '
    ' Retour     :
    ' - "Administration" : Si sousContexte est vide ou null
    ' - "Administration > [sousContexte]" : Si sousContexte est fourni (Trim() appliqué)
    '
    ' Remarques  :
    ' - Appelée publiquement par NavigateToAdminView() pour construire le contexte hiérarchique
    ' - Le format hiérarchique permet de visualiser la navigation : "Administration > Paramètres > Détail"
    ' - Trim() est appliqué pour éviter les espaces indésirables dans le rendu final
    ' - Peut être appelée par les UserControls pour construire des contextes multi-niveaux
    '
    ' Exceptions :
    ' - Aucune (gère les chaînes vides/null sans erreur)
    ' -------------------------------------------------------------------------------------------------
    Public Function BuildAdminContexte(
        sousContexte As String
    ) As String

        If String.IsNullOrWhiteSpace(sousContexte) Then

            Return "Administration"

        End If

        Return $"Administration > {sousContexte.Trim()}"

    End Function


    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetContexte
    ' Version    : V1.2.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Met à jour le contexte de navigation affiché dans lblContexte (header) et stsLabelStatus (barre de statut).
    '
    ' Paramètres :
    ' - contexte : Libellé du contexte à afficher (ex: "Accueil", "Administration > Paramètres") (String)
    '
    ' Remarques  :
    ' - Appelée par NavigateTo() après chaque navigation pour synchroniser l'affichage
    ' - Appelée publiquement par les UserControls pour mettre à jour le contexte dynamiquement
    ' - lblContexte (Label dans pnlHeader) : indique "où je suis" dans l'application
    ' - stsLabelStatus (ToolStripStatusLabel) : affiche l'état courant associé au contexte
    ' - Les deux contrôles affichent le même texte pour cohérence visuelle
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContexte(
        contexte As String
    )

        lblContexte.Text = contexte
        stsLabelStatus.Text = contexte

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PushContexteTemporaire
    ' Type       : String
    ' Version    : V1.0.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Applique temporairement un contexte avant ouverture d'une Form modale et retourne le contexte précédent.
    '
    ' Paramètres :
    ' - contexteTemporaire : Libellé du contexte temporaire à afficher (String)
    '
    ' Retour     :
    ' - Contexte précédent (lblContexte.Text) à restaurer après fermeture de la modale
    '
    ' Remarques  :
    ' - Utilisée avant l'ouverture de Forms modales (ConfigurationConnexion, ElevationAcces, etc.)
    ' - Pattern typique : contexte = PushContexteTemporaire("...") > ShowDialog() > RestoreContexte(contexte)
    ' - Sauvegarde le contexte actuel (lblContexte.Text) avant de le remplacer
    ' - Appelle SetContexte() pour mettre à jour lblContexte et stsLabelStatus
    ' - Le contexte retourné doit être passé à RestoreContexte() après fermeture de la modale
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Function PushContexteTemporaire(
        contexteTemporaire As String
    ) As String

        Dim contextePrecedent As String =
            lblContexte.Text

        SetContexte(contexteTemporaire)

        Return contextePrecedent

    End Function


    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : RestoreContexte
    ' Version    : V1.0.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Restaure un contexte précédemment sauvegardé via PushContexteTemporaire().
    '
    ' Paramètres :
    ' - contextePrecedent : Contexte à restaurer (String)
    '
    ' Remarques  :
    ' - Utilisée après fermeture de Forms modales pour revenir au contexte précédent
    ' - Pattern typique : contexte = PushContexteTemporaire("...") > ShowDialog() > RestoreContexte(contexte)
    ' - Si contextePrecedent est vide ou null : retourne sans erreur (protection)
    ' - Appelle SetContexte() pour mettre à jour lblContexte et stsLabelStatus
    '
    ' Exceptions :
    ' - Aucune (gère les chaînes vides/null sans erreur)
    ' -------------------------------------------------------------------------------------------------
    Public Sub RestoreContexte(
        contextePrecedent As String
    )

        If String.IsNullOrWhiteSpace(contextePrecedent) Then

            Return

        End If

        SetContexte(contextePrecedent)

    End Sub

#End Region

#Region "Tests divers - à supprimer"



    'PROVISOIRE : TEMPORAIRE DEV - à supprimer après usage - Test DialogChoix
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        ' Test simple
        DialogChoix.Information("DialogChoix fonctionne !", "Test")

        ' Test confirmation
        If DialogChoix.Confirmer("Tester DialogChoix ?", "Test") = DialogResult.Yes Then
            DialogChoix.Erreur("Attention !")

        End If
    End Sub

    'PROVISOIRE : TEMPORAIRE DEV - à supprimer après usage - Test RichTextEditor
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Using TestRichTextEditor As New TestRichTextEditor()

            TestRichTextEditor.ShowDialog(Me)

        End Using

    End Sub

#End Region

End Class
