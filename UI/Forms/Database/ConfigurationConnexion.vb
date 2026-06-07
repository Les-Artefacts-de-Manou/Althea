' -------------------------------------------------------------------------------------------------
' Form        : ConfigurationConnexion
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 22/04/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Interface de configuration de la connexion à la base de données MariaDB.
'
' Responsabilités :
' - Permettre la saisie/modification des paramètres de connexion (host, port, database, user, password, options)
' - Tester la connexion à la base de données avant enregistrement
' - Gérer le mot de passe de manière sécurisée (chiffrement DPAPI, masquage UI)
' - Sauvegarder la configuration locale via ConfigManager
' - Charger une configuration existante
' - Créer le dossier et le fichier de configuration si nécessaires
'
' Remarques   :
' - Implémente IContextAwareForm pour l'injection du contexte UI partagé
' - Le mot de passe n'est JAMAIS affiché en clair (sauf maintien du bouton "Voir")
' - Le mot de passe existant n'est jamais déchiffré pour affichage
' - L'enregistrement n'est possible qu'après un test de connexion réussi
'
' Dépendances :
' - UserControlContext (contexte UI injecté par Home)
' - LocalDbConfig (modèle de configuration)
' - ConfigManager (chargement/sauvegarde configuration)
' - DatabaseManager (test de connexion)
' - CryptoManagerDPAPI (chiffrement/déchiffrement mot de passe)
' - UtilsButtons (thématisation des boutons)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareForm : Pour injection du contexte UI partagé
'
' Imports     :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ConfigurationConnexion
    Implements IContextAwareForm

#Region "Variables privées"

    ' -------------------------------------------------------------------------------------------------
    ' Variables privées de la Form
    ' Version     : V1.0.0
    ' Date        : 22/04/2026
    ' Rôle        :
    ' Gèrent l'état interne de la Form et le mode de gestion du mot de passe.
    ' -------------------------------------------------------------------------------------------------

    ' Contexte UI partagé injecté par Home
    Private _context As UserControlContext

    ' Indique si un test de connexion valide a été effectué
    Private _isConnectionTestSuccessful As Boolean = False

    ' Indique si le formulaire a été modifié depuis le dernier test
    Private _isFormDirty As Boolean = False

    ' Indique s'il y a des changements non enregistrés
    Private _hasUnsavedChanges As Boolean = False

    ' Indique si une configuration existante contient déjà un mot de passe chiffré
    Private _hasStoredEncryptedPassword As Boolean = False

    ' Stocke le mot de passe chiffré déjà existant, si présent
    Private _storedEncryptedPassword As String = String.Empty

    ' -------------------------------------------------------------------------------------------------
    ' Énumération : PasswordMode
    ' Version     : V1.0.0
    ' Date        : 22/04/2026
    ' Rôle        :
    ' Définit le mode de gestion du mot de passe.
    ' - KeepExisting (0) : Conservation du mot de passe chiffré existant
    ' - SetNew (1)       : Saisie d'un nouveau mot de passe
    ' -------------------------------------------------------------------------------------------------
    Private Enum PasswordMode
        KeepExisting = 0
        SetNew = 1
    End Enum

    ' Mode de gestion du mot de passe actuel
    Private _passwordMode As PasswordMode = PasswordMode.KeepExisting

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
    ' - Configure les infobulles pour txtHost, nudPort, txtDatabaseName, txtUserName, txtPassword,
    '   txtAdditionalOptions, btnTesterConnexion, btnVoirPassword, btnModifierPassword,
    '   btnEnregistrerConnexion, btnFermer
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeToolTips()

        If _context Is Nothing Then Return

        _context.SetToolTip(
            txtHost,
            "Nom du serveur MariaDB ou adresse IP."
        )

        _context.SetToolTip(
            nudPort,
            "Port TCP utilisé par MariaDB."
        )

        _context.SetToolTip(
            txtDatabaseName,
            "Nom de la base de données Althéa."
        )

        _context.SetToolTip(
            txtUserName,
            "Utilisateur MariaDB utilisé par l’application."
        )

        _context.SetToolTip(
            txtPassword,
            "Mot de passe du compte MariaDB."
        )

        _context.SetToolTip(
            txtAdditionalOptions,
            "Options supplémentaires (optionnel)."
        )

        _context.SetToolTip(
            btnTesterConnexion,
            "Tester immédiatement la connexion MariaDB."
        )

        _context.SetToolTip(
            btnVoirPassword,
            "Afficher temporairement le mot de passe pendant l’appui."
        )

        _context.SetToolTip(
            btnModifierPassword,
            "Modifier le mot de passe."
        )

        _context.SetToolTip(
            btnEnregistrerConnexion,
            "Enregistrer les paramètres de connexion."
        )

        _context.SetToolTip(
            btnFermer,
            "Fermer la fenêtre sans enregistrer."
        )

    End Sub

#End Region

#Region "Événements Form"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ConfigurationConnexion_Load
    ' Version     : V1.0
    ' Date : 22/04/26
    ' Rôle       : Initialise l’état de la Form au démarrage
    ' Le chargement d'une configuration existante peut écraser les valeurs par défaut.

    Private Sub ConfigurationConnexion_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        txtHost.Text = "127.0.0.1"
        nudPort.Value = 3306
        nudPort.Minimum = 1
        nudPort.Maximum = 65535

        InitializeToolTips()

        SetHeader("Administration > Configuration connexion")
        SetStatus("Configuration de la connexion MariaDB.")

        _isConnectionTestSuccessful = False
        _isFormDirty = False
        _hasStoredEncryptedPassword = False
        _storedEncryptedPassword = String.Empty

        lblConnectionResult.Text = "Aucun test effectué"
        SetStatus("Configuration non validée")

        txtPassword.UseSystemPasswordChar = True

        LoadExistingConfiguration()
        UpdateFormState()

        UtilsButtons.InitStandardButton(btnFermer)
        UtilsButtons.InitStandardButton(btnTesterConnexion)
        UtilsButtons.InitStandardButton(btnEnregistrerConnexion)
        UtilsButtons.InitStandardButton(btnModifierPassword)
        UtilsButtons.InitStandardButton(btnVoirPassword)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ConfigurationConnexion_FormClosing
    ' Version    : V1.0.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Gère la fermeture de la Form en nettoyant le message de statut.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement FormClosingEventArgs
    '
    ' Remarques  :
    ' - Appelée automatiquement lors de la fermeture de la Form
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConfigurationConnexion_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        SetStatus("")

    End Sub

#End Region

#Region "Événements contrôles"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnFieldChanged
    ' Version    : V1.2.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Détecte toute modification utilisateur dans les champs de saisie et invalide le test précédent.
    '
    ' Responsabilités :
    ' - Marquer la Form comme modifiée (_isFormDirty = True)
    ' - Signaler des changements non enregistrés (_hasUnsavedChanges = True)
    ' - Invalider le test de connexion précédent (_isConnectionTestSuccessful = False)
    ' - Rafraîchir l'état de l'interface via UpdateFormState()
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Gère les événements TextChanged (txtHost, txtDatabaseName, txtUserName, txtPassword, txtAdditionalOptions)
    ' - Gère l'événement ValueChanged (nudPort)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnFieldChanged(sender As Object, e As EventArgs) _
    Handles txtHost.TextChanged,
            txtDatabaseName.TextChanged,
            txtUserName.TextChanged,
            txtPassword.TextChanged,
            txtAdditionalOptions.TextChanged,
            nudPort.ValueChanged

        _isFormDirty = True
        _hasUnsavedChanges = True
        _isConnectionTestSuccessful = False

        UpdateFormState()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : txtPassword_Enter
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' Vide le champ mot de passe si un mot de passe masqué "********" est affiché.
    '
    ' Responsabilités :
    ' - Détecter la présence du placeholder "********"
    ' - Vider le champ pour permettre la saisie d'un nouveau mot de passe
    ' - Marquer la Form comme modifiée
    ' - Invalider le test de connexion précédent
    ' - Mettre à jour l'état de l'interface
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Ne s'active que si un mot de passe chiffré existant est stocké
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtPassword_Enter(sender As Object, e As EventArgs) Handles txtPassword.Enter

        If _hasStoredEncryptedPassword AndAlso txtPassword.Text = "********" Then
            txtPassword.Clear()
            _isFormDirty = True
            _isConnectionTestSuccessful = False
            UpdateFormState()
        End If

    End Sub

#End Region

#Region "Actions boutons"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnModifierPassword_Click
    ' Version    : V1.0.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Permet à l'utilisateur de remplacer le mot de passe stocké dans la configuration locale.
    '
    ' Responsabilités :
    ' - Passer le formulaire en mode saisie d'un nouveau mot de passe (PasswordMode.SetNew)
    ' - Activer le champ mot de passe et lui donner le focus
    ' - Vider le champ mot de passe
    ' - Invalider le test de connexion précédent
    ' - Mettre à jour l'état de l'interface
    ' - Journaliser l'action
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Le mot de passe existant n'est JAMAIS affiché en clair
    ' - Le nouveau mot de passe ne sera enregistré qu'après un test réussi
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifierPassword_Click(sender As Object, e As EventArgs) _
    Handles btnModifierPassword.Click

        _passwordMode = PasswordMode.SetNew
        _isConnectionTestSuccessful = False
        _isFormDirty = True

        txtPassword.Enabled = True
        txtPassword.Text = String.Empty
        txtPassword.Focus()

        lblConnectionResult.Text = "Veuillez saisir le nouveau mot de passe puis tester la connexion"
        SetStatus("Modification du mot de passe")

        GestionLog.EcrireLog(
        "Modification du mot de passe demandée par l'utilisateur",
        GestionLog.LogLevel.Succinct,
        GestionLog.LogCategory.UI)

        UpdateFormState()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnTesterConnexion_Click
    ' Version    : V1.3.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Teste la connexion à la base de données à partir des valeurs saisies ou stockées.
    '
    ' Responsabilités :
    ' - Vérifier que les champs obligatoires sont valides via AreRequiredFieldsValid()
    ' - Construire la configuration à partir des champs de la Form
    ' - Déterminer le mot de passe à utiliser (stocké ou saisi) selon le mode
    ' - Déchiffrer le mot de passe stocké si nécessaire
    ' - Initialiser DatabaseManager avec la configuration et le mot de passe
    ' - Tester la connexion via DatabaseManager.TestConnection()
    ' - Mettre à jour l'état UI (lblConnectionResult, statut, indicateurs)
    ' - Journaliser les événements
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Le mot de passe n'est jamais affiché en clair
    ' - Le mode PasswordMode détermine quelle source de mot de passe utiliser
    ' - En cas d'échec, un message d'erreur explicite est affiché
    '
    ' Exceptions :
    ' - Exception : Logée via GestionLog.EcrireException ; message affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTesterConnexion_Click(sender As Object, e As EventArgs) Handles btnTesterConnexion.Click

        ' === TEST TEMPORAIRE DialogChoix ===
        DialogChoix.Erreur(
        "Erreur lors de l'ouverture de la configuration de connexion.",
        "Erreur"
    )
        Return ' Sortir pour ne pas exécuter le reste
        ' === FIN TEST ===
        Try
            _isConnectionTestSuccessful = False

            ' Validation minimale
            If Not AreRequiredFieldsValid() Then

                lblConnectionResult.Text = "Veuillez compléter les champs obligatoires avant le test"
                SetStatus("Test impossible - champs incomplets")

                GestionLog.EcrireLog(
                "Test refusé : champs incomplets",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI)

                UpdateFormState()
                Return

            End If

            ' Construction config
            Dim config As LocalDbConfig = BuildConfigFromForm()
            Dim passwordToUse As String

            ' Choix du mot de passe
            If _passwordMode = PasswordMode.KeepExisting Then

                passwordToUse = CryptoManagerDPAPI.Unprotect(_storedEncryptedPassword)

                GestionLog.EcrireLog(
                "Test avec mot de passe stocké",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database)

            Else

                passwordToUse = txtPassword.Text

                GestionLog.EcrireLog(
                "Test avec mot de passe saisi",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database)

            End If

            ' Initialisation DB
            DatabaseManager.Initialize(config, passwordToUse)

            ' Test réel
            Dim isSuccess As Boolean = DatabaseManager.TestConnection()

            If isSuccess Then

                _isConnectionTestSuccessful = True
                _isFormDirty = False

                lblConnectionResult.Text = "Connexion validée avec succès"
                SetStatus("Connexion OK")

                GestionLog.EcrireLog(
                "Test connexion réussi",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database)

            Else

                _isConnectionTestSuccessful = False

                If _passwordMode = PasswordMode.KeepExisting Then

                    lblConnectionResult.Text = "Connexion échouée - cliquez sur Modifier pour saisir un nouveau mot de passe"
                    SetStatus("Connexion invalide")

                    GestionLog.EcrireLog(
                    "Test échoué avec mot de passe stocké",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Database)

                Else

                    lblConnectionResult.Text = "Échec de connexion à la base de données"
                    SetStatus("Test échoué")

                    GestionLog.EcrireLog(
                    "Test échoué avec mot de passe saisi",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Database)

                End If

            End If

            UpdateFormState()

        Catch ex As Exception

            _isConnectionTestSuccessful = False

            GestionLog.EcrireException(
            "ConfigurationConnexion - Erreur test connexion",
            ex,
            GestionLog.LogLevel.Complet,
            GestionLog.LogCategory.Database)

            lblConnectionResult.Text = "Erreur lors du test de connexion"
            SetStatus("Erreur test")

            UpdateFormState()

        End Try

    End Sub


    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnEnregistrerConnexion_Click
    ' Version    : V1.3.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Sauvegarde la configuration de connexion de manière sécurisée.
    '
    ' Responsabilités :
    ' - Vérifier qu'un test de connexion valide a été effectué
    ' - Construire la configuration à partir des champs de la Form
    ' - Gérer le mot de passe (conservation ou remplacement)
    ' - Chiffrer le nouveau mot de passe si modifié
    ' - Sauvegarder via ConfigManager
    ' - Mettre à jour l'UI et journaliser les événements
    ' - Fermer la Form avec DialogResult.OK
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Le mot de passe n'est JAMAIS stocké en clair
    ' - Si non modifié, le mot de passe chiffré existant est conservé
    ' - L'enregistrement est refusé si aucun test de connexion réussi n'a été effectué
    '
    ' Exceptions :
    ' - Exception : Logée via GestionLog.EcrireException ; message affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------

    Private Sub btnEnregistrerConnexion_Click(sender As Object, e As EventArgs) _
    Handles btnEnregistrerConnexion.Click

        Try
            ' Vérification préalable : test de connexion obligatoire
            If Not _isConnectionTestSuccessful Then

                lblConnectionResult.Text = "Test de connexion requis avant enregistrement"
                SetStatus("Enregistrement refusé")

                GestionLog.EcrireLog(
                "Tentative d'enregistrement refusée : test de connexion non validé",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI)

                Return
            End If

            ' Construction de la configuration
            Dim config As LocalDbConfig = BuildConfigFromForm()

            ' -------------------------------------------------------------------------------------------------
            ' Gestion du mot de passe
            ' -------------------------------------------------------------------------------------------------

            If _hasStoredEncryptedPassword AndAlso txtPassword.Text = "********" Then
                ' Cas 1 : mot de passe inchangé → on conserve l'ancien
                config.EncryptedPassword = _storedEncryptedPassword

                GestionLog.EcrireLog(
                "Mot de passe inchangé - conservation de la valeur existante",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database)

            Else
                ' Cas 2 : mot de passe modifié → on chiffre le nouveau
                Dim encryptedPassword As String = CryptoManagerDPAPI.Protect(txtPassword.Text)
                config.EncryptedPassword = encryptedPassword

                GestionLog.EcrireLog(
                "Mot de passe modifié - nouvelle valeur chiffrée",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database)
            End If

            ' -------------------------------------------------------------------------------------------------
            ' Sauvegarde
            ' -------------------------------------------------------------------------------------------------

            config.LastConnectionTestUtc = DateTime.UtcNow

            ConfigManager.SaveLocalDbConfig(config)

            GestionLog.EcrireLog(
            "Configuration de connexion enregistrée avec succès",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.Database)

            ' Mise à jour état interne
            _isFormDirty = False
            _hasUnsavedChanges = False
            _isConnectionTestSuccessful = False
            _hasStoredEncryptedPassword = True
            _storedEncryptedPassword = config.EncryptedPassword

            ' Feedback utilisateur
            lblConnectionResult.Text = "Configuration enregistrée avec succès"
            SetStatus("Configuration sauvegardée")

            UpdateFormState()

            ' Fermeture propre (mode modal futur)
            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception

            GestionLog.EcrireException(
            "ConfigurationConnexion - Erreur lors de l'enregistrement",
            ex,
            GestionLog.LogLevel.Complet,
            GestionLog.LogCategory.Database)

            lblConnectionResult.Text = "Erreur lors de l'enregistrement"
            SetStatus("Erreur enregistrement")

            UpdateFormState()

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirPassword_MouseDown
    ' Version    : V1.2.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Affiche temporairement le mot de passe en clair tant que le bouton est maintenu appuyé.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement MouseEventArgs
    '
    ' Remarques  :
    ' - Ne fonctionne que si le champ mot de passe est activé et non vide
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirPassword_MouseDown(sender As Object, e As MouseEventArgs) Handles btnVoirPassword.MouseDown

        If txtPassword.Enabled AndAlso Not String.IsNullOrWhiteSpace(txtPassword.Text) Then
            txtPassword.UseSystemPasswordChar = False
            SetStatus("Mot de passe visible temporairement")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirPassword_MouseLeave
    ' Version    : V1.2.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Masque le mot de passe si la souris quitte le bouton pendant l'appui.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Protège contre la sortie accidentelle du bouton pendant l'affichage temporaire
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirPassword_MouseLeave(sender As Object, e As EventArgs) Handles btnVoirPassword.MouseLeave

        txtPassword.UseSystemPasswordChar = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnVoirPassword_MouseUp
    ' Version    : V1.2.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Masque à nouveau le mot de passe dès que le bouton est relâché.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement MouseEventArgs
    '
    ' Remarques  :
    ' - Remet le masquage du mot de passe automatiquement
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirPassword_MouseUp(sender As Object, e As MouseEventArgs) Handles btnVoirPassword.MouseUp

        txtPassword.UseSystemPasswordChar = True
        SetStatus("Mot de passe masqué")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnFermer_Click
    ' Version    : V1.0.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Ferme la fenêtre de configuration sans valider ni enregistrer la connexion.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Définit DialogResult.Cancel avant fermeture
    ' - Journalise la fermeture sans validation
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click

        GestionLog.EcrireLog(
        "Fermeture de ConfigurationConnexion sans validation",
        GestionLog.LogLevel.Succinct,
        GestionLog.LogCategory.UI)

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

#End Region

#Region "Validation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AreRequiredFieldsValid
    ' Version    : V1.2.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Vérifie si les champs obligatoires sont correctement renseignés.
    '
    ' Retour     :
    ' - Boolean : True si tous les champs obligatoires sont valides, False sinon
    '
    ' Remarques  :
    ' - En mode KeepExisting, le mot de passe stocké chiffré est considéré comme valide
    ' - En mode SetNew, un nouveau mot de passe doit être saisi dans le champ txtPassword
    ' - Vérifie : host, port, nom de base de données, nom d'utilisateur et mot de passe
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Function AreRequiredFieldsValid() As Boolean

        Dim hasPassword As Boolean

        If _passwordMode = PasswordMode.KeepExisting Then
            hasPassword = Not String.IsNullOrWhiteSpace(_storedEncryptedPassword)
        Else
            hasPassword = Not String.IsNullOrWhiteSpace(txtPassword.Text)
        End If

        Return Not String.IsNullOrWhiteSpace(txtHost.Text) AndAlso
           nudPort.Value > 0 AndAlso
           Not String.IsNullOrWhiteSpace(txtDatabaseName.Text) AndAlso
           Not String.IsNullOrWhiteSpace(txtUserName.Text) AndAlso
           hasPassword

    End Function

#End Region

#Region "Gestion état UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateFormState
    ' Version    : V1.2.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Met à jour l'état des boutons et des messages de la Form en fonction de la situation.
    '
    ' Responsabilités :
    ' - Activer/désactiver le bouton Tester selon la validité des champs obligatoires
    ' - Activer/désactiver le bouton Enregistrer selon le résultat du test et l'existence de modifications
    ' - Gérer l'état du bouton Modifier mot de passe
    ' - Gérer l'état du bouton Voir mot de passe
    ' - Mettre à jour lblConnectionResult selon la situation
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Le bouton Enregistrer n'est jamais actif sans test de connexion réussi
    ' - Le bouton Modifier n'est actif que si un mot de passe chiffré est présent et en mode KeepExisting
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub UpdateFormState()

        Dim isValid As Boolean = AreRequiredFieldsValid()

        btnTesterConnexion.Enabled = isValid
        btnEnregistrerConnexion.Enabled = _isConnectionTestSuccessful AndAlso _hasUnsavedChanges

        btnModifierPassword.Enabled = _hasStoredEncryptedPassword AndAlso
                                  _passwordMode = PasswordMode.KeepExisting

        btnVoirPassword.Enabled = txtPassword.Enabled AndAlso Not String.IsNullOrWhiteSpace(txtPassword.Text)

        If Not isValid Then
            lblConnectionResult.Text = "Veuillez compléter les champs obligatoires"
        ElseIf _isFormDirty AndAlso Not _isConnectionTestSuccessful Then
            lblConnectionResult.Text = "Configuration modifiée - test requis"
        End If

    End Sub

#End Region

#Region "Gestion configuration"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : BuildConfigFromForm
    ' Version    : V1.1.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' Construit un objet LocalDbConfig à partir des valeurs saisies dans la Form.
    '
    ' Retour     :
    ' - LocalDbConfig : Objet contenant les paramètres de connexion
    '
    ' Remarques  :
    ' - Le mot de passe n'est pas traité ici
    ' - La propriété EncryptedPassword est laissée vide et sera alimentée plus tard lors de l'enregistrement
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------

    Private Function BuildConfigFromForm() As LocalDbConfig

        Dim config As New LocalDbConfig()

        config.Host = txtHost.Text.Trim()
        config.Port = CInt(nudPort.Value)
        config.DatabaseName = txtDatabaseName.Text.Trim()
        config.UserName = txtUserName.Text.Trim()
        config.AdditionalOptions = txtAdditionalOptions.Text.Trim()

        ' Le mot de passe chiffré sera affecté lors du processus d'enregistrement.
        config.EncryptedPassword = String.Empty

        Return config


    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : LoadExistingConfiguration
    ' Version    : V1.2.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Charge la configuration locale existante dans la Form si elle existe.
    '
    ' Responsabilités :
    ' - Charger la configuration via ConfigManager.LoadLocalDbConfig()
    ' - Alimenter les champs de la Form avec les valeurs chargées
    ' - Gérer le mode mot de passe (KeepExisting si mot de passe chiffré présent, SetNew sinon)
    ' - Désactiver le champ mot de passe en mode KeepExisting
    ' - Réinitialiser les indicateurs de test et de modification
    ' - Afficher un message de chargement réussi
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Le mot de passe n'est JAMAIS affiché
    ' - Si un mot de passe chiffré existe, il est conservé en mémoire dans _storedEncryptedPassword
    ' - Le champ mot de passe reste vide et désactivé tant que l'utilisateur ne demande pas sa modification
    ' - En cas d'erreur, le formulaire bascule en mode SetNew
    '
    ' Exceptions :
    ' - Exception : Logée via GestionLog.EcrireException ; bascule en mode SetNew
    ' -------------------------------------------------------------------------------------------------
    Private Sub LoadExistingConfiguration()

        Try
            Dim config As LocalDbConfig = ConfigManager.LoadLocalDbConfig()

            If config Is Nothing Then
                _hasStoredEncryptedPassword = False
                _storedEncryptedPassword = String.Empty
                _passwordMode = PasswordMode.SetNew
                txtPassword.Text = String.Empty
                txtPassword.Enabled = True
                Return
            End If

            txtHost.Text = config.Host
            nudPort.Value = config.Port
            txtDatabaseName.Text = config.DatabaseName
            txtUserName.Text = config.UserName
            txtAdditionalOptions.Text = config.AdditionalOptions

            If Not String.IsNullOrWhiteSpace(config.EncryptedPassword) Then
                _hasStoredEncryptedPassword = True
                _storedEncryptedPassword = config.EncryptedPassword
                _passwordMode = PasswordMode.KeepExisting
                txtPassword.Text = String.Empty
                txtPassword.Enabled = False
            Else
                _hasStoredEncryptedPassword = False
                _storedEncryptedPassword = String.Empty
                _passwordMode = PasswordMode.SetNew
                txtPassword.Text = String.Empty
                txtPassword.Enabled = True
            End If

            _isFormDirty = False
            _hasUnsavedChanges = False
            _isConnectionTestSuccessful = False

            lblConnectionResult.Text = "Configuration existante chargée"
            SetStatus("Configuration chargée")

            UpdateFormState()

        Catch ex As Exception

            GestionLog.EcrireException(
            "ConfigurationConnexion - Erreur lors du chargement de la configuration existante",
            ex,
            GestionLog.LogLevel.Complet,
            GestionLog.LogCategory.Startup)

            lblConnectionResult.Text = "Erreur lors du chargement de la configuration"
            SetStatus("Erreur chargement configuration")

            _hasStoredEncryptedPassword = False
            _storedEncryptedPassword = String.Empty
            _passwordMode = PasswordMode.SetNew
            txtPassword.Text = String.Empty
            txtPassword.Enabled = True

            _isConnectionTestSuccessful = False
            _isFormDirty = False

            UpdateFormState()

        End Try

    End Sub

#End Region

End Class