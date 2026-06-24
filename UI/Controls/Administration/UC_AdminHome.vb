' -------------------------------------------------------------------------------------------------
' UserControl : UC_AdminHome
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 12/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' UserControl représentant l'écran d'administration principal de l'application Althéa.
'
' Responsabilités :
' - Afficher l'interface d'accueil de la section Administration
' - Gérer l'affichage et l'état des boutons selon le rôle utilisateur (Admin, SuperUser)
' - Proposer l'accès aux outils d'administration : Paramètres, Utilisateurs, Logs, Sauvegardes, Connexion DB
' - Gérer l'élévation temporaire des droits d'accès (bouton "Élever accès")
' - Gérer le retour au rôle de base après élévation (bouton "Retour rôle de base")
' - Afficher le rôle courant et l'état de l'élévation (lblRoleCourant, lblElevation)
' - Ouvrir des Forms ponctuelles avec contexte temporaire (ConfigurationConnexion, ElevationAcces)
' - Naviguer vers d'autres UserControls via NavigateToAdminView() (UC_Parametres)
'
' Remarques   :
' - Chargé dynamiquement dans le panneau central de Home via le mécanisme de navigation
' - Implémente IContextAwareUserControl pour injection du contexte UI partagé
' - Aucune logique métier ni accès direct à la base de données (tout passe par les couches inférieures)
' - Point d'entrée vers les fonctionnalités techniques et de maintenance de l'application
' - Le retour au rôle de base peut déclencher un retour automatique à l'Accueil si le rôle < SuperUser
'
' Dépendances :
' - UserSession (session utilisateur avec élévation)
' - UtilisateurApplication (utilisateur connecté avec RoleMaxElevation)
' - UserControlContext (contexte UI injecté par Home)
' - Home (Form parente : navigation, contexte, affichage utilisateur)
' - ConfigurationConnexion (Form configuration DB)
' - ElevationAcces (Form élévation d'accès)
' - UC_Parametres (UserControl paramètres)
' - UtilsButtons (thématisation des boutons)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : Pour injection du contexte UI partagé
'
' Imports     :
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_AdminHome
    Implements IContextAwareUserControl

#Region "Variables privées"

    'Injecté par Home via SetContext() (implémentation IContextAwareUserControl)
    Private _context As UserControlContext

    ' Fourni au constructeur, contient CurrentRole et IsElevated
    Private ReadOnly _userSession As UserSession

    'Fourni au constructeur, contient RoleMaxElevation
    Private ReadOnly _utilisateur As UtilisateurApplication

#End Region

#Region "Constructeurs"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.2.0
    ' Date         : 11/05/2026
    '
    ' Rôle         :
    ' Constructeur par défaut requis par le WinForms Designer.
    '
    ' Paramètres   :
    ' - Aucun
    '
    ' Remarques    :
    ' - Ne pas supprimer : obligatoire pour le Designer
    ' - N'initialise pas _userSession ni _utilisateur (seront Nothing)
    ' - En production, utiliser le constructeur surchargé New(userSession, utilisateur)
    '
    ' Exceptions   :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub New()

        InitializeComponent()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.3.0
    ' Date         : 12/05/2026
    '
    ' Rôle         :
    ' Initialise UC_AdminHome avec la session utilisateur et les données métier de l'utilisateur.
    '
    ' Paramètres   :
    ' - userSession : Session utilisateur courante (contient CurrentRole, IsElevated)
    ' - utilisateur : Données métier de l'utilisateur connecté (contient RoleMaxElevation)
    '
    ' Remarques    :
    ' - Constructeur utilisé en production par Home lors du chargement du UserControl
    ' - _context sera injecté ultérieurement via SetContext()
    '
    ' Exceptions   :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(
        userSession As UserSession,
        utilisateur As UtilisateurApplication
    )

        InitializeComponent()

        _userSession = userSession
        _utilisateur = utilisateur

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetContext
    ' Version    : V1.1.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Injecte le contexte UI partagé fourni par Home (implémentation IContextAwareUserControl).
    '
    ' Paramètres :
    ' - context : Instance de UserControlContext partagée entre Home et tous les UserControls
    '
    ' Remarques  :
    ' - Appelé automatiquement par Home après instanciation du UserControl
    ' - Permet d'accéder aux contrôles partagés : StatusStrip, ErrorProvider, ToolTip
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(
        context As UserControlContext
    ) Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetStatus
    ' Version    : V1.0.0
    ' Date       : 12/05/2026
    '
    ' Rôle       :
    ' Met à jour le message de la barre d'état via le contexte partagé.
    '
    ' Paramètres :
    ' - message : Texte à afficher dans la StatusStrip de Home
    '
    ' Remarques  :
    ' - Si _context est Nothing, l'appel est ignoré sans erreur
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub SetStatus(
        message As String
    )

        If _context IsNot Nothing Then
            _context.SetStatus(message)
        End If

    End Sub

#End Region

#Region "Gestion des droits"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerDroitsUtilisateur
    ' Version    : V1.3.0
    ' Date       : 12/05/2026
    '
    ' Rôle       :
    ' Configure l'affichage et l'état des boutons selon le rôle utilisateur courant et l'état d'élévation.
    '
    ' Responsabilités :
    ' - Afficher le rôle courant et l'état d'élévation (lblRoleCourant, lblElevation)
    ' - Activer/désactiver les boutons d'administration selon le rôle (Admin : tous, SuperUser : Paramètres uniquement)
    ' - Activer/désactiver le bouton "Élever accès" selon le RoleMaxElevation de l'utilisateur
    ' - Activer/désactiver le bouton "Retour rôle de base" selon l'état d'élévation
    ' - Gérer le cas où _userSession est Nothing (désactiver tout)
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée au chargement du UserControl (UC_AdminHome_Load)
    ' - Appelée après élévation ou retour au rôle de base pour rafraîchir l'affichage
    ' - Stratégie : Admin -> tous les boutons, SuperUser -> Paramètres uniquement, autres -> rien
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerDroitsUtilisateur()

        If _userSession Is Nothing Then

            DesactiverTousLesBoutonsAdmin()

            btnEleverAcces.Enabled = False
            btnRetourRoleBase.Enabled = False

            lblRoleCourant.Text =
            "Rôle courant : inconnu"

            lblElevation.Text =
            "Aucune session active"

            If _context IsNot Nothing Then
                _context.SetStatus("Session utilisateur absente.")
            End If

            Return

        End If

        lblRoleCourant.Text =
        $"Rôle courant : {_userSession.CurrentRole}"

        If _userSession.IsElevated Then
            lblElevation.Text =
            "Élévation active"
        Else
            lblElevation.Text =
            "Élévation inactive"
        End If

        If _utilisateur IsNot Nothing Then
            btnEleverAcces.Enabled =
            _userSession.CurrentRole < _utilisateur.RoleMaxElevation
        Else
            btnEleverAcces.Enabled = False
        End If

        btnRetourRoleBase.Enabled =
        _userSession.IsElevated

        Select Case _userSession.CurrentRole

            Case AppRole.Admin

                ActiverTousLesBoutonsAdmin()

                If _context IsNot Nothing Then
                    _context.SetStatus(
                                "Administration complète.")
                End If

            Case AppRole.SuperUser

                btnParametres.Enabled = True

                btnUtilisateurs.Enabled = False
                btnLogs.Enabled = False
                btnSauvegardes.Enabled = False
                btnConnexionDatabase.Enabled = False

                If _context IsNot Nothing Then
                    _context.SetStatus(
                                "Administration limitée SuperUser.")
                End If

            Case Else

                DesactiverTousLesBoutonsAdmin()

                If _context IsNot Nothing Then
                    _context.SetStatus(
                                "Accès administration non autorisé.")
                End If

        End Select

        If _utilisateur IsNot Nothing Then
            btnEleverAcces.Enabled =
            _userSession.CurrentRole < _utilisateur.RoleMaxElevation
        End If
        btnRetourRoleBase.Enabled =
        _userSession.IsElevated
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ActiverTousLesBoutonsAdmin
    ' Version    : V1.2.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Active tous les boutons d'administration (Paramètres, Utilisateurs, Logs, Sauvegardes, Connexion DB).
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée uniquement pour le rôle Admin
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActiverTousLesBoutonsAdmin()

        btnParametres.Enabled = True
        btnUtilisateurs.Enabled = True
        btnLogs.Enabled = True
        btnSauvegardes.Enabled = True
        btnConnexionDatabase.Enabled = True

    End Sub

    Private Sub InitialiserToolTips()

        If _context Is Nothing Then Exit Sub

        _context.SetToolTip(btnEleverAcces, "Élever les droits de l'utilisateur.")
        _context.SetToolTip(btnRetourRoleBase, "Retourner au rôle de base.")
        _context.SetToolTip(btnParametres, "Ouvrir les paramètres de l'application.")
        _context.SetToolTip(btnUtilisateurs, "Gérer les utilisateurs et leur rôle.")
        _context.SetToolTip(btnLogs, "Afficher les journaux d'activité.")
        _context.SetToolTip(btnSauvegardes, "Gérer les sauvegardes de l'application.")
        _context.SetToolTip(btnConnexionDatabase, "Configurer la connexion à la base de données.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverTousLesBoutonsAdmin
    ' Version    : V1.2.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Désactive tous les boutons d'administration (Paramètres, Utilisateurs, Logs, Sauvegardes, Connexion DB).
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée pour les rôles inférieurs à SuperUser ou si _userSession est Nothing
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub DesactiverTousLesBoutonsAdmin()

        btnParametres.Enabled = False
        btnUtilisateurs.Enabled = False
        btnLogs.Enabled = False
        btnSauvegardes.Enabled = False
        btnConnexionDatabase.Enabled = False

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UC_AdminHome_Load
    ' Version    : V1.2.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Initialise l'affichage du UserControl au chargement.
    '
    ' Responsabilités :
    ' - Appeler AppliquerDroitsUtilisateur() pour configurer l'état des boutons selon le rôle
    ' - Thématiser les boutons tuiles (Paramètres, Utilisateurs, Logs, Sauvegardes, Connexion DB) via UtilsButtons.InitLargeIconButton()
    ' - Thématiser les boutons standards (Élever accès, Retour rôle de base) via UtilsButtons.InitStandardButton()
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Premier événement déclenché lors du chargement du UserControl dans Home
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_AdminHome_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        AppliquerDroitsUtilisateur()

        'Boutons Tuiles
        UtilsButtons.InitLargeIconButton(btnParametres)
        UtilsButtons.InitLargeIconButton(btnUtilisateurs)
        UtilsButtons.InitLargeIconButton(btnLogs)
        UtilsButtons.InitLargeIconButton(btnSauvegardes)
        UtilsButtons.InitLargeIconButton(btnConnexionDatabase)

        'Boutons Standards
        UtilsButtons.InitStandardButton(btnEleverAcces)
        UtilsButtons.InitStandardButton(btnRetourRoleBase)

        'ToolTips
        InitialiserToolTips()

    End Sub

#End Region

#Region "Actions administration"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnParametres_Click
    ' Version    : V1.1.0
    ' Date       : 29/04/2026
    '
    ' Rôle       :
    ' Ouvre la vue de paramètres généraux de l'application (UserControl UC_Parametres).
    '
    ' Responsabilités :
    ' - Récupérer la Form parente Home via FindForm()
    ' - Déterminer le mode d'accès selon le rôle courant (Admin ou SuperUser)
    ' - Naviguer vers UC_Parametres via Home.NavigateToAdminView()
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Accès réservé aux administrateurs ou SuperUser
    ' - Permet de consulter et modifier les paramètres applicatifs stockés en base de données
    ' - Le mode Admin donne accès à tous les paramètres, le mode SuperUser est plus limité
    '
    ' Exceptions :
    ' - Aucune (gestion silencieuse si Home est introuvable)
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnParametres_Click(sender As Object, e As EventArgs) Handles btnParametres.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        Dim modeAcces As ModeAccesParametres =
            ModeAccesParametres.SuperUser

        If _userSession IsNot Nothing AndAlso _userSession.IsAdmin() Then

            modeAcces =
                ModeAccesParametres.Admin

        End If

        homeForm.NavigateToAdminView(
            New UC_Parametres(modeAcces),
            "Paramètres"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnUtilisateurs_Click
    ' Version   : V1.0.0
    ' Date      : 21/05/2026
    '
    ' Rôle      :
    ' Ouvre l'écran d'administration des utilisateurs.
    '
    ' Remarques :
    ' - Réservé aux administrateurs.
    ' - La navigation passe par Home.NavigateToAdminView.
    ' - Aucun accès direct à la base de données depuis UC_AdminHome.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnUtilisateurs_Click(
    sender As Object,
    e As EventArgs
) Handles btnUtilisateurs.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToAdminView(
        New UC_Utilisateurs(),
        "Utilisateurs"
    )

    End Sub


    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnConnexionDatabase_Click
    ' Version    : V1.1.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Ouvre la Form de configuration de connexion à la base de données (ConfigurationConnexion).
    '
    ' Responsabilités :
    ' - Récupérer la Form parente Home via FindForm()
    ' - Pousser un contexte temporaire dans Home via PushContexteTemporaire()
    ' - Ouvrir ConfigurationConnexion en mode modal avec injection du contexte UI
    ' - Restaurer le contexte précédent après fermeture de la Form
    ' - Journaliser et afficher les erreurs via GestionLog et MessageBox
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Accès réservé aux administrateurs (bouton désactivé pour les autres rôles)
    ' - La Form est ouverte avec contexte temporaire pour afficher le contexte correct dans l'en-tête
    '
    ' Exceptions :
    ' - Exception : Loguée via GestionLog ; MessageBox affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnConnexionDatabase_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnConnexionDatabase.Click

        Try

            Dim homeForm As Home =
            CType(FindForm(), Home)

            If homeForm Is Nothing Then

                If _context IsNot Nothing Then
                    _context.SetStatus(
                                "Impossible de retrouver Home.")
                End If

                Return

            End If

            Dim contextePrecedent As String =
            homeForm.PushContexteTemporaire(
                homeForm.BuildAdminContexte("Configuration connexion DB")
            )

            Try

                Using frmConfiguration As New ConfigurationConnexion()

                    frmConfiguration.SetContext(_context)
                    frmConfiguration.ShowDialog(Me)

                End Using

            Finally

                homeForm.SetContexte("Administration")


            End Try

        Catch ex As Exception

            GestionLog.EcrireLog(
            "Erreur btnConnexionDatabase_Click.",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.UI,
            ex
        )

            DialogChoix.Erreur(
            "Erreur lors de l'ouverture de la configuration de connexion.",
            "Erreur"
        )


        End Try

    End Sub

#End Region

#Region "Gestion élévation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnRetourRoleBase_Click
    ' Version    : V1.1.0
    ' Date       : 12/05/2026
    '
    ' Rôle       :
    ' Annule l'élévation temporaire et revient au rôle utilisateur de base.
    '
    ' Responsabilités :
    ' - Vérifier que _userSession est valide et que l'élévation est active
    ' - Appeler UserSession.ResetElevation() pour revenir au rôle de base
    ' - Journaliser le retour via GestionLog (catégorie Security, niveau Rapide)
    ' - Rafraîchir l'affichage via AppliquerDroitsUtilisateur() et Home.UpdateConnectedUserDisplay()
    ' - Si le rôle de base ne permet plus l'accès à l'administration (< SuperUser), retourner automatiquement à l'Accueil
    ' - Afficher les erreurs via MessageBox
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Bouton activé uniquement si IsElevated est True
    ' - Si le rôle de base < SuperUser, navigation automatique vers Home.NavigateToAccueil()
    '
    ' Exceptions :
    ' - Exception : Loguée via GestionLog ; MessageBox affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRetourRoleBase_Click_1(sender As Object, e As EventArgs) Handles btnRetourRoleBase.Click

        Try

            If _userSession Is Nothing Then
                Return
            End If

            If Not _userSession.IsElevated Then

                If _context IsNot Nothing Then
                    _context.SetStatus(
                                "Aucune élévation active.")
                End If

                Return

            End If

            _userSession.ResetElevation()

            EcrireLog(
            $"Retour au rôle de base ({_userSession.UserName}, rôle={_userSession.CurrentRole}).",
            LogLevel.Rapide,
            LogCategory.Security
        )

            Dim homeForm =
            CType(FindForm(), Home)

            If homeForm IsNot Nothing Then

                homeForm.UpdateConnectedUserDisplay()

            End If

            AppliquerDroitsUtilisateur()

            If _context IsNot Nothing Then
                _context.SetStatus(
                        "Retour au rôle de base effectué.")
            End If

            If _userSession.CurrentRole < AppRole.SuperUser Then

                If homeForm IsNot Nothing Then

                    homeForm.NavigateToAccueil()

                End If

            End If

        Catch ex As Exception

            EcrireLog(
            "Erreur btnRetourRoleBase_Click.",
            LogLevel.Succinct,
            LogCategory.Security,
            ex
        )

            DialogChoix.Erreur(
            "Erreur lors du retour au rôle de base.",
            "Erreur"
        )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnEleverAcces_Click
    ' Version    : V1.2.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Ouvre la Form d'élévation d'accès (ElevationAcces) pour demander temporairement un rôle supérieur.
    '
    ' Responsabilités :
    ' - Vérifier que _userSession est valide
    ' - Récupérer la Form parente Home via FindForm()
    ' - Pousser un contexte temporaire dans Home via PushContexteTemporaire()
    ' - Ouvrir ElevationAcces en mode modal avec injection du contexte UI et de la session utilisateur
    ' - Restaurer le contexte précédent après fermeture de la Form
    ' - Rafraîchir l'affichage via AppliquerDroitsUtilisateur() et Home.UpdateConnectedUserDisplay()
    ' - Journaliser l'élévation via GestionLog (catégorie Security, niveau Rapide)
    ' - Afficher les erreurs via MessageBox
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Bouton activé uniquement si CurrentRole < RoleMaxElevation
    ' - Si l'élévation est annulée ou refusée, aucun changement n'est appliqué
    '
    ' Exceptions :
    ' - Exception : Loguée via GestionLog ; MessageBox affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEleverAcces_Click_1(sender As Object, e As EventArgs) Handles btnEleverAcces.Click

        Try

            If _userSession Is Nothing Then

                If _context IsNot Nothing Then
                    _context.SetStatus("Session utilisateur absente.")
                End If

                Return

            End If

            Dim homeForm =
            TryCast(FindForm(), Home)

            If homeForm Is Nothing Then

                If _context IsNot Nothing Then
                    _context.SetStatus("Impossible de retrouver Home.")
                End If

                Return

            End If

            Dim contextePrecedent =
            homeForm.PushContexteTemporaire(
                homeForm.BuildAdminContexte("Élévation d'accès")
            )

            Try

                Using frmElevation As New ElevationAcces(
                _userSession,
                homeForm.AuthenticatedUser
            )

                    frmElevation.SetContext(_context)

                    Dim result =
                    frmElevation.ShowDialog(homeForm)

                    If result <> DialogResult.OK Then

                        If _context IsNot Nothing Then
                            _context.SetStatus("Élévation annulée ou refusée.")
                        End If

                        Return

                    End If

                End Using

            Finally

                homeForm.SetContexte(contextePrecedent)

            End Try

            AppliquerDroitsUtilisateur()

            homeForm.UpdateConnectedUserDisplay()

            EcrireLog(
            $"Élévation appliquée depuis AdminHome ({_userSession.UserName} -> {_userSession.CurrentRole}).",
            LogLevel.Rapide,
            LogCategory.Security
        )

            If _context IsNot Nothing Then
                _context.SetStatus($"Rôle courant : {_userSession.CurrentRole}")
            End If

        Catch ex As Exception

            EcrireLog(
            "Erreur btnEleverAcces_Click.",
            LogLevel.Succinct,
            LogCategory.Security,
            ex
        )

            DialogChoix.Erreur(
            "Erreur lors de l'élévation d'accès.",
            "Erreur"
        )

        End Try

    End Sub

#End Region


End Class
