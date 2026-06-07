' -------------------------------------------------------------------------------------------------
' Form        : ChangePassword
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 11/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Interface de changement obligatoire ou volontaire du mot de passe utilisateur.
'
' Responsabilités :
' - Permettre la saisie de l'ancien mot de passe, du nouveau mot de passe et de sa confirmation
' - Valider la conformité du nouveau mot de passe aux règles de sécurité (10 caractères min, maj, min, chiffre)
' - Vérifier que le nouveau mot de passe est différent de l'ancien
' - Vérifier la correspondance entre nouveau mot de passe et confirmation
' - Appeler GestionUtilisateurs pour effectuer le changement côté métier/base de données
' - Gérer l'affichage temporaire des mots de passe (boutons "Voir")
' - Afficher les messages d'erreur de validation et les retours d'opération
'
' Remarques   :
' - Aucun accès SQL direct : toute logique sécurité passe par GestionUtilisateurs
' - Aucun hash dans l'UI : les mots de passe sont transmis en clair au module métier
' - Le mot de passe n'est visible que pendant l'appui sur le bouton "Voir"
' - Affichage modal depuis le formulaire de connexion ou depuis l'interface principale
'
' Dépendances :
' - UtilisateurApplication (utilisateur authentifié reçu en paramètre du constructeur)
' - GestionUtilisateurs (module métier pour le changement de mot de passe)
' - UtilsButtons (thématisation des boutons)
' - GestionLog (journalisation des erreurs)
'
' Interface   :
' - Aucune
'
' Imports     :
' - System.Linq (pour les méthodes Any sur caractères)
' - System.Text
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Imports System.Linq
Imports System.Text

' Enumération pour définir le mode de changement de mot de passe
Public Enum ModeChangePassword
    UserChange = 0  ' Utilisateur change son propre mot de passe (mode par défaut)
    AdminReset = 1  ' Admin réinitialise le mot de passe d'un utilisateur
End Enum


Public Class ChangePassword

#Region "Variables privées"

    ' -------------------------------------------------------------------------------------------------
    ' Variables privées de la Form
    ' Version     : V1.0.0
    ' Date        : 10/05/2026
    ' Rôle        :
    ' Stocke l'utilisateur authentifié pour lequel le mot de passe doit être changé.
    ' -------------------------------------------------------------------------------------------------

    ' Utilisateur authentifié reçu lors de la création de la Form
    Private ReadOnly _utilisateur As UtilisateurApplication

    ' Mode de changement de mot de passe (UserChange ou AdminReset)
    Private ReadOnly _mode As ModeChangePassword

#End Region

#Region "Constructeurs"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.1.0
    ' Date         : 17/05/2026
    '
    ' Rôle         :
    ' Initialise la Form de changement de mot de passe avec l'utilisateur authentifié.
    '
    ' Paramètres   :
    ' - utilisateur : L'utilisateur authentifié pour lequel le mot de passe doit être changé
    ' - mode        : Mode de changement (UserChange ou AdminReset)
    '
    ' Remarques    :
    ' - Appelle InitializeComponent() pour initialiser les contrôles du designer
    ' - Stocke l'utilisateur dans _utilisateur pour utilisation ultérieure
    ' - En mode AdminReset, l'ancien mot de passe n'est pas demandé
    '
    ' Exceptions   :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(
        utilisateur As UtilisateurApplication,
        Optional mode As ModeChangePassword = ModeChangePassword.UserChange
    )

        InitializeComponent()

        _utilisateur = utilisateur
        _mode = mode

    End Sub

#End Region

#Region "Validation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderSaisies
    ' Version    : V1.1.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Vérifie la validité de toutes les saisies utilisateur avant appel au module métier.
    '
    ' Responsabilités :
    ' - Vérifier que l'ancien mot de passe est saisi
    ' - Vérifier que le nouveau mot de passe est saisi
    ' - Vérifier que le nouveau mot de passe contient au moins 10 caractères
    ' - Vérifier que le nouveau mot de passe contient au moins une majuscule
    ' - Vérifier que le nouveau mot de passe contient au moins une minuscule
    ' - Vérifier que le nouveau mot de passe contient au moins un chiffre
    ' - Vérifier que le nouveau mot de passe est différent de l'ancien
    ' - Vérifier que la confirmation correspond au nouveau mot de passe
    ' - Afficher les erreurs via ErrorProvider et dans les labels de statut
    '
    ' Retour     :
    ' - Boolean : True si toutes les validations sont réussies, False sinon
    '
    ' Remarques  :
    ' - La validation des règles de mot de passe est effectuée côté UI (contraintes de saisie)
    ' - La vérification de l'ancien mot de passe est effectuée côté métier (GestionUtilisateurs)
    ' - Le focus est automatiquement placé sur le champ en erreur
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisies() As Boolean

        errProvider.Clear()

        Dim passwordRulesMessage As String =
        "Le nouveau mot de passe doit contenir au moins 10 caractères, une majuscule, une minuscule et un chiffre."

        Dim oldPassword As String =
        txtOldPassword.Text

        Dim newPassword As String =
        txtNewPassword.Text

        Dim confirmation As String =
        txtConfirmation.Text

        ' En mode AdminReset, on ne valide pas l'ancien mot de passe
        If _mode = ModeChangePassword.UserChange Then

            If String.IsNullOrWhiteSpace(oldPassword) Then

                Dim message As String =
                "L'ancien mot de passe est obligatoire."

                errProvider.SetError(txtOldPassword, message)
                AfficherErreurValidation(message)
                txtOldPassword.Focus()

                Return False

            End If

        End If

        If String.IsNullOrWhiteSpace(newPassword) Then

            errProvider.SetError(txtNewPassword, passwordRulesMessage)
            AfficherErreurValidation(passwordRulesMessage)
            txtNewPassword.Focus()

            Return False

        End If

        If newPassword.Length < 10 Then

            errProvider.SetError(txtNewPassword, passwordRulesMessage)
            AfficherErreurValidation(passwordRulesMessage)
            txtNewPassword.Focus()

            Return False

        End If

        If Not newPassword.Any(AddressOf Char.IsUpper) Then

            errProvider.SetError(txtNewPassword, passwordRulesMessage)
            AfficherErreurValidation(passwordRulesMessage)
            txtNewPassword.Focus()

            Return False

        End If

        If Not newPassword.Any(AddressOf Char.IsLower) Then

            errProvider.SetError(txtNewPassword, passwordRulesMessage)
            AfficherErreurValidation(passwordRulesMessage)
            txtNewPassword.Focus()

            Return False

        End If

        If Not newPassword.Any(AddressOf Char.IsDigit) Then

            errProvider.SetError(txtNewPassword, passwordRulesMessage)
            AfficherErreurValidation(passwordRulesMessage)
            txtNewPassword.Focus()

            Return False

        End If

        If oldPassword = newPassword Then

            Dim message As String =
        "Le nouveau mot de passe doit être différent de l'ancien."

            errProvider.SetError(txtNewPassword, message)
            AfficherErreurValidation(message)

            txtNewPassword.Focus()

            Return False

        End If


        If confirmation <> newPassword Then

            Dim message As String =
            "La confirmation ne correspond pas au nouveau mot de passe."

            errProvider.SetError(txtConfirmation, message)
            AfficherErreurValidation(message)
            txtConfirmation.Focus()

            Return False

        End If

        lblMessage.Text = String.Empty
        stsLabelStatus.Text = "Validation correcte."

        Return True

    End Function

#End Region

#Region "Actions boutons"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnValider_Click
    ' Version    : V1.0.0
    ' Date       : 10/05/2026
    '
    ' Rôle       :
    ' Lance le changement de mot de passe après validation des saisies.
    '
    ' Responsabilités :
    ' - Appeler ValiderSaisies() pour vérifier les contraintes de saisie
    ' - Appeler GestionUtilisateurs.ChangerMotDePasse() pour effectuer le changement
    ' - Afficher un message d'erreur si l'ancien mot de passe est incorrect
    ' - Afficher un message de succès et fermer la Form en cas de réussite
    ' - Gérer les exceptions et journaliser les erreurs
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Définit DialogResult.OK et ferme la Form en cas de succès
    ' - Les mots de passe sont transmis en clair au module métier (pas de hash dans l'UI)
    '
    ' Exceptions :
    ' - Exception : Logée via GestionLog.EcrireLog ; message affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnValider_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnValider.Click

        Try

            If Not ValiderSaisies() Then
                Exit Sub
            End If

            Dim success As Boolean = False

            Select Case _mode

                Case ModeChangePassword.UserChange
                    ' Mode utilisateur : appeler ChangerMotDePasse avec vérification ancien mot de passe
                    success = GestionUtilisateurs.ChangerMotDePasse(
                        _utilisateur,
                        txtOldPassword.Text,
                        txtNewPassword.Text
                    )

                    If Not success Then
                        AfficherErreurValidation("Ancien mot de passe incorrect.")
                        txtOldPassword.Focus()
                        Return
                    End If

                Case ModeChangePassword.AdminReset
                    ' Mode admin : appeler ResetPasswordUtilisateur avec le nouveau mot de passe saisi
                    ' On utilise une méthode métier spécifique pour l'admin qui ne vérifie pas l'ancien mot de passe
                    success = GestionUtilisateurs.ResetPasswordUtilisateurWithCustomPassword(
                        _utilisateur.IdUtilisateur,
                        txtNewPassword.Text
                    )

                    If Not success Then
                        AfficherErreurValidation("Erreur lors de la réinitialisation du mot de passe.")
                        Return
                    End If

            End Select

            stsLabelStatus.Text = "Mot de passe modifié."
            DialogResult = DialogResult.OK
            Close()

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur ChangePassword.btnValider_Click.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            lblMessage.Text =
                "Erreur lors du changement du mot de passe."

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAnnuler_Click
    ' Version    : V1.0.0
    ' Date       : 10/05/2026
    '
    ' Rôle       :
    ' Annule le changement de mot de passe et ferme la Form.
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

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AfficherErreurValidation
    ' Version    : V1.0.0
    ' Date       : 10/05/2026
    '
    ' Rôle       :
    ' Affiche un message d'erreur de validation dans la Form et dans le StatusStrip.
    '
    ' Paramètres :
    ' - message : Le message d'erreur à afficher
    '
    ' Remarques  :
    ' - Met à jour lblMessage (label principal) et stsLabelStatus (barre de statut)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherErreurValidation(
        message As String
    )

        lblMessage.Text = message
        stsLabelStatus.Text = message

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChangePassword_Load
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Initialise l'affichage et l'état de la Form au chargement.
    '
    ' Responsabilités :
    ' - Afficher le message d'information (changement obligatoire)
    ' - Initialiser les infobulles via InitializeToolTips()
    ' - Masquer les mots de passe (UseSystemPasswordChar = True)
    ' - Thématiser les boutons via UtilsButtons
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Appelée automatiquement au chargement de la Form
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChangePassword_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        lblMessage.Text =
            "Le changement du mot de passe est obligatoire avant de continuer."

        stsLabelStatus.Text =
            "Changement obligatoire du mot de passe."

        InitializeToolTips()

        txtOldPassword.UseSystemPasswordChar = True
        txtNewPassword.UseSystemPasswordChar = True
        txtConfirmation.UseSystemPasswordChar = True

        UtilsButtons.InitStandardButton(btnValider)
        UtilsButtons.InitStandardButton(btnAnnuler)
        UtilsButtons.InitStandardButton(btnVoirOldPassword)
        UtilsButtons.InitStandardButton(btnVoirNewPassword)
        UtilsButtons.InitStandardButton(btnVoirConfirmation)

        ' Adapter l'UI selon le mode
        Select Case _mode

            Case ModeChangePassword.UserChange
                ' Mode utilisateur : affichage normal
                Text = $"Changer mon mot de passe - {_utilisateur.NomAffichage}"

            Case ModeChangePassword.AdminReset
                ' Mode admin : masquer l'ancien mot de passe, pré-remplir le nouveau
                Text = $"Réinitialiser le mot de passe - {_utilisateur.NomAffichage}"

                lblMessage.Text = "Réinitialisation du mot de passe par l'administrateur."
                stsLabelStatus.Text = "Réinitialisation du mot de passe."

                ' Masquer le champ ancien mot de passe
                lblOldPassword.Visible = False
                txtOldPassword.Visible = False
                btnVoirOldPassword.Visible = False

                ' Générer un mot de passe temporaire et le pré-remplir
                Dim passwordTemporaire As String = GestionUtilisateurs.GenererMotDePasseTemporaire()
                txtNewPassword.Text = passwordTemporaire
                txtConfirmation.Text = passwordTemporaire

                ' Focus sur le nouveau mot de passe pour permettre à l'admin de le modifier si souhaité
                txtNewPassword.Focus()
                txtNewPassword.SelectAll()

        End Select

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
    ' - Configure les infobulles pour txtOldPassword, btnVoirOldPassword, txtNewPassword,
    '   btnVoirNewPassword, txtConfirmation, btnVoirConfirmation, lblPasswordRules,
    '   btnValider, btnAnnuler
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeToolTips()

        ttMain.SetToolTip(
            txtOldPassword,
            "Saisir le mot de passe actuel."
        )

        ttMain.SetToolTip(
            btnVoirOldPassword,
            "Afficher temporairement le mot de passe actuel pendant l'appui."
        )

        ttMain.SetToolTip(
            txtNewPassword,
            "Saisir le nouveau mot de passe."
        )

        ttMain.SetToolTip(
            btnVoirNewPassword,
            "Afficher temporairement le nouveau mot de passe pendant l'appui."
        )

        ttMain.SetToolTip(
            txtConfirmation,
            "Confirmer le nouveau mot de passe."
        )

        ttMain.SetToolTip(
            btnVoirConfirmation,
            "Afficher temporairement la confirmation du mot de passe pendant l'appui."
        )

        ttMain.SetToolTip(
            lblPasswordRules,
            "Règles de sécurité applicables au mot de passe."
        )

        ttMain.SetToolTip(
            btnValider,
            "Valider le changement du mot de passe."
        )

        ttMain.SetToolTip(
            btnAnnuler,
            "Fermer la fenêtre sans modifier le mot de passe."
        )

    End Sub

#End Region

#Region "Affichage mots de passe"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetPasswordVisible
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Affiche ou masque temporairement un champ mot de passe.
    '
    ' Paramètres :
    ' - targetTextBox : Le TextBox contenant le mot de passe
    ' - visible       : True pour afficher en clair, False pour masquer
    '
    ' Remarques  :
    ' - Bascule la propriété UseSystemPasswordChar (True = masqué, False = visible)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub SetPasswordVisible(
        targetTextBox As TextBox,
        visible As Boolean
    )

        targetTextBox.UseSystemPasswordChar = Not visible

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirOldPassword_MouseDown
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    ' Rôle       : Affiche temporairement l'ancien mot de passe tant que le bouton est maintenu appuyé.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirOldPassword_MouseDown(
        sender As Object,
        e As MouseEventArgs
    ) Handles btnVoirOldPassword.MouseDown

        SetPasswordVisible(txtOldPassword, True)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirOldPassword_MouseUp
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    ' Rôle       : Masque à nouveau l'ancien mot de passe dès que le bouton est relâché.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirOldPassword_MouseUp(
        sender As Object,
        e As MouseEventArgs
    ) Handles btnVoirOldPassword.MouseUp

        SetPasswordVisible(txtOldPassword, False)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirOldPassword_MouseLeave
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    ' Rôle       : Masque l'ancien mot de passe si la souris quitte le bouton pendant l'appui.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirOldPassword_MouseLeave(
        sender As Object,
        e As EventArgs
    ) Handles btnVoirOldPassword.MouseLeave

        SetPasswordVisible(txtOldPassword, False)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirNewPassword_MouseDown
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    ' Rôle       : Affiche temporairement le nouveau mot de passe tant que le bouton est maintenu appuyé.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirNewPassword_MouseDown(
        sender As Object,
        e As MouseEventArgs
    ) Handles btnVoirNewPassword.MouseDown

        SetPasswordVisible(txtNewPassword, True)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirNewPassword_MouseUp
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    ' Rôle       : Masque à nouveau le nouveau mot de passe dès que le bouton est relâché.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirNewPassword_MouseUp(
        sender As Object,
        e As MouseEventArgs
    ) Handles btnVoirNewPassword.MouseUp

        SetPasswordVisible(txtNewPassword, False)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirNewPassword_MouseLeave
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    ' Rôle       : Masque le nouveau mot de passe si la souris quitte le bouton pendant l'appui.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirNewPassword_MouseLeave(
        sender As Object,
        e As EventArgs
    ) Handles btnVoirNewPassword.MouseLeave

        SetPasswordVisible(txtNewPassword, False)

    End Sub


    Private Sub btnVoirConfirmation_MouseDown(
        sender As Object,
        e As MouseEventArgs
    ) Handles btnVoirConfirmation.MouseDown

        SetPasswordVisible(txtConfirmation, True)

    End Sub


    Private Sub btnVoirConfirmation_MouseUp(
        sender As Object,
        e As MouseEventArgs
    ) Handles btnVoirConfirmation.MouseUp

        SetPasswordVisible(txtConfirmation, False)

    End Sub


    Private Sub btnVoirConfirmation_MouseLeave(
        sender As Object,
        e As EventArgs
    ) Handles btnVoirConfirmation.MouseLeave

        SetPasswordVisible(txtConfirmation, False)

    End Sub

#End Region

End Class