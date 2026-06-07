' -------------------------------------------------------------------------------------------------
' Form        : ElevationAcces
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Interface d'élévation temporaire des droits d'accès utilisateur.
'
' Responsabilités :
' - Permettre à un utilisateur de demander temporairement un rôle supérieur (Admin ou SuperUser)
' - Afficher la liste des rôles d'élévation disponibles selon les droits de l'utilisateur
' - Vérifier le mot de passe de l'utilisateur avant d'accorder l'élévation
' - Appeler GestionUtilisateurs.VerifierElevationUtilisateur() pour valider la demande
' - Mettre à jour la session utilisateur via UserSession.ElevateTo()
' - Journaliser les élévations d'accès
' - Gérer l'affichage temporaire du mot de passe (bouton "Voir")
'
' Remarques   :
' - Implémente IContextAwareForm pour l'injection du contexte UI partagé
' - L'élévation est temporaire et dure le temps de la session
' - Seuls les rôles supérieurs au rôle actuel et inférieurs ou égaux au RoleMaxElevation sont proposés
' - Le mot de passe est vérifié côté métier via GestionUtilisateurs
'
' Dépendances :
' - UserSession (session utilisateur à élever)
' - UtilisateurApplication (utilisateur connecté)
' - UserControlContext (contexte UI injecté par Home)
' - GestionUtilisateurs (vérification élévation)
' - UtilsButtons (thématisation des boutons)
' - GestionLog (journalisation)
' - AppRole (enumération des rôles)
'
' Interface   :
' - IContextAwareForm : Pour injection du contexte UI partagé
'
' Imports     :
' - Aucun (utilise seulement les types du projet)
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ElevationAcces
    Implements IContextAwareForm

#Region "Variables privées"

    ' -------------------------------------------------------------------------------------------------
    ' Variables privées de la Form
    ' Version     : V1.0.0
    ' Date        : 11/05/2026
    ' Rôle        :
    ' Stockent la session utilisateur, l'utilisateur connecté et le contexte UI injecté.
    ' -------------------------------------------------------------------------------------------------

    ' Session utilisateur à élever (reçue en paramètre du constructeur)
    Private ReadOnly _userSession As UserSession

    ' Utilisateur connecté pour lequel l'élévation est demandée
    Private ReadOnly _utilisateur As UtilisateurApplication

    ' Contexte UI partagé injecté par Home
    Private _context As UserControlContext

#End Region

#Region "Constructeurs"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 11/05/2026
    '
    ' Rôle         :
    ' Constructeur par défaut requis par le Designer WinForms.
    '
    ' Paramètres   :
    ' - Aucun
    '
    ' Remarques    :
    ' - Ne doit PAS être utilisé directement dans le code applicatif
    ' - Utilisé uniquement par le Designer Visual Studio
    ' - Utiliser le constructeur New(userSession, utilisateur) dans le code
    '
    ' Exceptions   :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub New()

        InitializeComponent()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 11/05/2026
    '
    ' Rôle         :
    ' Initialise la Form d'élévation avec la session et l'utilisateur connecté.
    '
    ' Paramètres   :
    ' - userSession : La session utilisateur à élever temporairement
    ' - utilisateur : L'utilisateur connecté demandant l'élévation
    '
    ' Remarques    :
    ' - Appelle InitializeComponent() pour initialiser les contrôles du designer
    ' - Stocke la session et l'utilisateur pour utilisation ultérieure
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
    ' Version    : V1.0.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Injecte le contexte UI partagé fourni par la Form Home.
    '
    ' Paramètres :
    ' - context : Le contexte UI partagé (UserControlContext)
    '
    ' Remarques  :
    ' - Implémente IContextAwareForm.SetContext
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(
        context As UserControlContext
    ) Implements IContextAwareForm.SetContext

        _context = context

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetStatus
    ' Version    : V1.0.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Met à jour le message de statut global via le contexte UI.
    '
    ' Paramètres :
    ' - message : Le message de statut à afficher
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

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetHeader
    ' Version    : V1.0.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Met à jour le message d'en-tête global via le contexte UI.
    '
    ' Paramètres :
    ' - message : Le message d'en-tête à afficher
    '
    ' Remarques  :
    ' - Si _context est Nothing, l'appel est ignoré sans erreur
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub SetHeader(
        message As String
    )

        If _context IsNot Nothing Then
            _context.SetHeader(message)
        End If

    End Sub


    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitializeToolTips
    ' Version    : V1.0.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Initialise les infobulles de tous les contrôles de la Form.
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Si _context est Nothing, l'initialisation est ignorée
    ' - Configure les infobulles pour cboRoleDemande, txtPassword, btnVoirPassword,
    '   btnValider, btnAnnuler
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeToolTips()

        If _context Is Nothing Then Return

        _context.SetToolTip(
            cboRoleDemande,
            "Sélectionner le rôle temporaire demandé."
        )

        _context.SetToolTip(
            txtPassword,
            "Saisir votre mot de passe utilisateur pour confirmer l’élévation."
        )

        _context.SetToolTip(
            btnVoirPassword,
            "Afficher temporairement le mot de passe pendant l’appui."
        )

        _context.SetToolTip(
            btnValider,
            "Valider la demande d’élévation temporaire."
        )

        _context.SetToolTip(
            btnAnnuler,
            "Annuler la demande et fermer la fenêtre."
        )

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ElevationAcces_Load
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Initialise l'affichage et l'état de la Form au chargement.
    '
    ' Responsabilités :
    ' - Initialiser la liste des rôles disponibles pour l'élévation selon le RoleMaxElevation de l'utilisateur
    ' - Vérifier que l'utilisateur et la session sont valides
    ' - Désactiver le bouton Valider si aucune élévation n'est disponible
    ' - Initialiser les infobulles, les messages d'en-tête et de statut
    ' - Masquer le mot de passe (UseSystemPasswordChar = True)
    ' - Thématiser les boutons via UtilsButtons
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Appelée automatiquement au chargement de la Form
    ' - Seuls les rôles supérieurs au rôle actuel sont proposés
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub ElevationAcces_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        cboRoleDemande.Items.Clear()

        InitializeToolTips()

        SetHeader("Administration > Élévation d'accès")
        SetStatus("Élévation d'accès.")

        If _utilisateur Is Nothing Then

            SetStatus("Utilisateur absent.")

            SetStatus("Élévation impossible.")

            btnValider.Enabled = False

            Return

        End If

        If _utilisateur.RoleMaxElevation >= AppRole.SuperUser _
            AndAlso _userSession.CurrentRole < AppRole.SuperUser Then

            cboRoleDemande.Items.Add(AppRole.SuperUser)

        End If

        If _utilisateur.RoleMaxElevation >= AppRole.Admin _
            AndAlso _userSession.CurrentRole < AppRole.Admin Then

            cboRoleDemande.Items.Add(AppRole.Admin)

        End If

        If cboRoleDemande.Items.Count = 0 Then

            lblMessage.Text =
                "Aucune élévation disponible pour cet utilisateur."

            SetStatus("Aucune élévation disponible.")

            btnValider.Enabled = False

            Return

        End If

        UtilsButtons.InitStandardButton(btnValider)
        UtilsButtons.InitStandardButton(btnAnnuler)
        UtilsButtons.InitStandardButton(btnVoirPassword)


        cboRoleDemande.SelectedIndex = 0

        txtPassword.UseSystemPasswordChar = True

        lblMessage.Text =
            "Sélectionnez le niveau d'accès demandé et confirmez avec votre mot de passe."

        SetStatus("Élévation d'accès.")

    End Sub

#End Region

#Region "Actions boutons"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnValider_Click
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Vérifie et applique l'élévation temporaire des droits d'accès.
    '
    ' Responsabilités :
    ' - Valider que le rôle demandé est sélectionné
    ' - Valider que le mot de passe est saisi
    ' - Appeler GestionUtilisateurs.VerifierElevationUtilisateur() pour vérifier l'élévation
    ' - Mettre à jour la session via UserSession.ElevateTo(targetRole)
    ' - Journaliser l'élévation via GestionLog
    ' - Afficher les messages de validation et les erreurs
    ' - Fermer la Form en cas de succès
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Définit DialogResult.OK et ferme la Form en cas de succès
    ' - Le mot de passe est vérifié côté métier (pas de hash dans l'UI)
    '
    ' Exceptions :
    ' - Exception : Logée via GestionLog ; MessageBox affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnValider_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnValider.Click

        Try

            errProvider.Clear()

            lblMessage.Text = String.Empty

            If cboRoleDemande.SelectedItem Is Nothing Then

                Dim message As String =
                    "Sélectionnez un rôle."

                errProvider.SetError(
                    cboRoleDemande,
                    message
                )

                lblMessage.Text = message
                SetStatus(message)

                cboRoleDemande.Focus()

                Return

            End If

            If String.IsNullOrWhiteSpace(
                txtPassword.Text
            ) Then

                Dim message As String =
                    "Le mot de passe est obligatoire."

                errProvider.SetError(
                    txtPassword,
                    message
                )

                lblMessage.Text = message
                SetStatus(message)

                txtPassword.Focus()

                Return

            End If

            Dim targetRole As AppRole =
                CType(
                    cboRoleDemande.SelectedItem,
                    AppRole
                )

            Dim elevationAllowed As Boolean =
                GestionUtilisateurs.VerifierElevationUtilisateur(
                    _utilisateur,
                    txtPassword.Text,
                    targetRole
                )

            If Not elevationAllowed Then

                Dim message As String =
                    "Élévation refusée."

                errProvider.SetError(
                    txtPassword,
                    message
                )

                lblMessage.Text = message
                SetStatus(message)

                txtPassword.Clear()
                txtPassword.Focus()

                Return

            End If

            _userSession.ElevateTo(targetRole)

            GestionLog.EcrireLog(
                $"Session élevée : {_utilisateur.LoginUtilisateur} -> {targetRole}.",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Security
            )

            SetStatus($"Session élevée : {targetRole}")

            DialogResult = DialogResult.OK

            Close()

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur btnValider_Click ElevationAcces.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            DialogChoix.Erreur(
                "Erreur lors de l'élévation d'accès.",
                "Erreur"
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAnnuler_Click
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Annule la demande d'élévation et ferme la Form.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Définit DialogResult.Cancel avant fermeture
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAnnuler.Click

        DialogResult = DialogResult.Cancel

        Close()

    End Sub

#End Region

#Region "Affichage mot de passe"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirPassword_MouseDown
    ' Version    : V1.0.0
    ' Date       : 12/05/2026
    ' Rôle       : Affiche temporairement le mot de passe tant que le bouton est maintenu appuyé.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirPassword_MouseDown(
        sender As Object,
        e As MouseEventArgs
    ) Handles btnVoirPassword.MouseDown

        txtPassword.UseSystemPasswordChar = False

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirPassword_MouseUp
    ' Version    : V1.0.0
    ' Date       : 12/05/2026
    ' Rôle       : Masque à nouveau le mot de passe dès que le bouton est relâché.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirPassword_MouseUp(
        sender As Object,
        e As MouseEventArgs
    ) Handles btnVoirPassword.MouseUp

        txtPassword.UseSystemPasswordChar = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirPassword_MouseLeave
    ' Version    : V1.0.0
    ' Date       : 12/05/2026
    ' Rôle       : Masque le mot de passe si la souris quitte le bouton pendant l'appui.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirPassword_MouseLeave(
        sender As Object,
        e As EventArgs
    ) Handles btnVoirPassword.MouseLeave

        txtPassword.UseSystemPasswordChar = True

    End Sub

#End Region

End Class