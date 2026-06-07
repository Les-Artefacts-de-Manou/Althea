' -------------------------------------------------------------------------------------------------
' Form        : Login
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 07/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Interface d'authentification principale de l'application Althéa.
'
' Responsabilités :
' - Afficher le formulaire de connexion avec login et mot de passe
' - Valider la saisie des identifiants (champs obligatoires)
' - Appeler GestionAuthentification.AuthentifierUtilisateur() pour vérifier les credentials
' - Gérer l'affichage temporaire du mot de passe (bouton "Voir")
' - Afficher les messages d'erreur via ErrorProvider et lblMessage
' - Journaliser les tentatives d'authentification via GestionLog
' - Exposer l'utilisateur authentifié via la propriété AuthenticatedUser
' - Supporter l'authentification par touche Entrée dans le champ mot de passe
'
' Remarques   :
' - Point d'entrée de l'application après le Splash
' - DialogResult.OK retourné en cas de succès, DialogResult.Cancel en cas d'annulation
' - Aucun mot de passe n'est loggué pour des raisons de sécurité
' - Les messages UI proviennent de la couche métier (AuthenticationResult.MessageUI)
'
' Dépendances :
' - GestionUtilisateurs (authentification métier)
' - AuthenticationResult (résultat d'authentification)
' - UtilisateurApplication (utilisateur authentifié)
' - UtilsButtons (thématisation des boutons)
' - GestionLog (journalisation sécurité)
'
' Imports     :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports Althea.Metier.Securite
Imports Althea.Core.Logging

Public Class Login

#Region "Variables privées"

    ' -------------------------------------------------------------------------------------------------
    ' Variables privées de la Form
    ' Version     : V1.0.0
    ' Date        : 07/05/2026
    '
    ' Rôle        :
    ' Stocke l'utilisateur authentifié suite à une connexion réussie.
    '
    ' Remarques   :
    ' - Exposé via la propriété publique AuthenticatedUser (ReadOnly)
    ' - Valorisé par la fonction Authentifier() après validation métier
    ' - Utilisé par Home pour initialiser la session utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private _authenticatedUser As UtilisateurApplication

#End Region

#Region "Propriétés publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété   : AuthenticatedUser
    ' Version     : V1.0.0
    ' Date        : 07/05/2026
    '
    ' Rôle        :
    ' Retourne l'utilisateur authentifié après une connexion réussie.
    '
    ' Type        : UtilisateurApplication (ReadOnly)
    '
    ' Remarques   :
    ' - Utilisé par Home pour initialiser la UserSession
    ' - Nothing tant que l'authentification n'a pas réussi
    '
    ' Exceptions  :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AuthenticatedUser As UtilisateurApplication
        Get
            Return _authenticatedUser
        End Get
    End Property

#End Region

#Region "Authentification"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Authentifier
    ' Version    : V1.1.0
    ' Date       : 09/05/2026
    '
    ' Rôle       :
    ' Vérifie les identifiants utilisateur et authentifie l'utilisateur via la couche métier.
    '
    ' Responsabilités :
    ' - Valider que le login est saisi (champ obligatoire)
    ' - Valider que le mot de passe est saisi (champ obligatoire)
    ' - Appeler GestionAuthentification.AuthentifierUtilisateur() pour vérifier les credentials
    ' - Afficher les erreurs de validation via ErrorProvider
    ' - Afficher les messages métier via lblMessage
    ' - Stocker l'utilisateur authentifié dans _authenticatedUser en cas de succès
    ' - Journaliser les erreurs d'authentification via GestionLog
    '
    ' Retour     :
    ' - Boolean : True si l'authentification a réussi, False sinon
    '
    ' Remarques  :
    ' - Aucun mot de passe n'est loggué pour des raisons de sécurité
    ' - Les messages UI proviennent du métier (AuthenticationResult.MessageUI)
    ' - Les erreurs d'exception sont loguées en catégorie Security / niveau Succinct
    '
    ' Exceptions :
    ' - Exception : Capturée, loguée via GestionLog, message générique affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Function Authentifier() As Boolean

        Try

            errProvider.Clear()

            lblMessage.Text = String.Empty

            Dim login As String =
            txtUserName.Text.Trim()

            Dim password As String =
            txtPassword.Text

            If String.IsNullOrWhiteSpace(login) Then

                errProvider.SetError(
                txtUserName,
                "Le login est obligatoire."
            )

                txtUserName.Focus()

                Return False

            End If

            If String.IsNullOrWhiteSpace(password) Then

                errProvider.SetError(
                txtPassword,
                "Le mot de passe est obligatoire."
            )

                txtPassword.Focus()

                Return False

            End If

            Dim authResult As AuthenticationResult =
            GestionAuthentification.AuthentifierUtilisateur(
                login,
                password
            )

            If Not authResult.Success Then

                lblMessage.Text =
                authResult.MessageUI

                Return False

            End If

            _authenticatedUser =
            authResult.Utilisateur

            Return True

        Catch ex As Exception

            GestionLog.EcrireLog(
            "Erreur authentification Login.",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.Security,
            ex
        )

            lblMessage.Text =
            "Erreur lors de l'authentification."

            Return False

        End Try

    End Function

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Login_Load
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Initialise l'affichage et l'état de la Form au chargement.
    '
    ' Responsabilités :
    ' - Configurer le masquage du mot de passe (UseSystemPasswordChar)
    ' - Afficher les messages de bienvenue (lblMessage, stsLabelStatus)
    ' - Initialiser les infobulles via InitializeToolTips()
    ' - Thématiser les boutons via UtilsButtons.InitStandardButton()
    ' - Définir le focus initial sur txtUserName
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Premier événement déclenché à l'ouverture de la Form
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        txtPassword.UseSystemPasswordChar = True
        lblMessage.Text = "Bienvenue dans Althéa. Veuillez-vous authentifier."
        stsLabelStatus.Text = "Bienvenue dans Althéa. Authentification"

        InitializeToolTips()

        UtilsButtons.InitStandardButton(btnConnexion)
        UtilsButtons.InitStandardButton(btnAnnuler)
        UtilsButtons.InitStandardButton(btnVoirPassword)
        txtUserName.Focus()

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
    ' - Configure les infobulles pour txtUserName, txtPassword, btnVoirPassword,
    '   btnConnexion, btnAnnuler
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeToolTips()

        ttMain.SetToolTip(
            txtUserName,
            "Saisir le nom d'utilisateur."
        )

        ttMain.SetToolTip(
            txtPassword,
            "Saisir le mot de passe."
        )

        ttMain.SetToolTip(
            btnVoirPassword,
            "Afficher temporairement le mot de passe pendant l'appui."
        )

        ttMain.SetToolTip(
            btnConnexion,
            "Valider la connexion."
        )

        ttMain.SetToolTip(
            btnAnnuler,
            "Annuler la connexion et fermer la fenêtre."
        )

    End Sub

#End Region

#Region "Actions boutons"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnConnexion_Click
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Lance l'authentification utilisateur et ferme la Form en cas de succès.
    '
    ' Responsabilités :
    ' - Appeler Authentifier() pour valider les credentials
    ' - Définir DialogResult.OK en cas de succès
    ' - Fermer la Form
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Si l'authentification échoue, la Form reste ouverte
    '
    ' Exceptions :
    ' - Aucune (gérées par Authentifier)
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnConnexion_Click(sender As Object, e As EventArgs) Handles btnConnexion.Click

        If Not Authentifier() Then
            Exit Sub
        End If

        DialogResult = DialogResult.OK

        Close()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAnnuler_Click
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Annule la connexion et ferme la Form.
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
    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click

        DialogResult = DialogResult.Cancel

        Close()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : txtPassword_KeyDown
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Permet de valider la connexion en appuyant sur la touche Entrée dans le champ mot de passe.
    '
    ' Responsabilités :
    ' - Détecter l'appui sur la touche Enter
    ' - Déclencher btnConnexion.PerformClick()
    ' - Marquer l'événement comme traité pour éviter le beep système
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement clavier
    '
    ' Remarques  :
    ' - e.Handled = True : empêche la propagation de l'événement
    ' - e.SuppressKeyPress = True : évite le "beep" du système
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtPassword_KeyDown(sender As Object, e As KeyEventArgs) Handles txtPassword.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnConnexion.PerformClick()
            e.Handled = True
            e.SuppressKeyPress = True ' Évite le "beep" du système
        End If
    End Sub

#End Region

#Region "Affichage mot de passe"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirPassword_MouseDown
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
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
    ' Date       : 11/05/2026
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
    ' Date       : 11/05/2026
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