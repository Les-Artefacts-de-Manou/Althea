' -------------------------------------------------------------------------------------------------
' Form        : ConfigurationConnexion
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 20/10/2026
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
' - V1.2.1 (20/10/2026, audit B1-B2) : bloc de test « DialogChoix » oublié retiré (« Tester la
'   connexion » ne testait plus rien, donc rien ne pouvait être enregistré) ; mot de passe conservé
'   selon le mode (KeepExisting) et non plus selon un texte « ******** » jamais affiché, qui faisait
'   enregistrer un mot de passe vide ; txtPassword_Enter (même texte) supprimé.
' - V1.3.0 (20/10/2026, demande de Joëlle) : messages d'aide et d'erreur (DialogChoix) en plus de la
'   barre d'état, qui laissait croire qu'il ne se passait rien : champs manquants nommés, cause d'un
'   échec de test et quoi vérifier, test requis, enregistrement réussi ou impossible, configuration
'   illisible. Le test ne touche plus à la connexion de l'application ; la configuration
'   enregistrée lui est appliquée (audit § 4.5).
'
' Dépendances :
' - UserControlContext (contexte UI injecté par Home)
' - LocalDbConfig (modèle de configuration)
' - ConfigManager (chargement/sauvegarde configuration)
' - DatabaseManager (test de connexion : TesterConfiguration ; application à l'enregistrement)
' - DialogChoix (messages d'aide et d'erreur)
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
    ' Version    : V1.1.0
    ' Date       : 15/10/2026
    ' Rôle       : Initialise l’état de la Form au démarrage (fond et barre de titre du kit compris).
    ' Remarques  :
    ' - Le chargement d'une configuration existante peut écraser les valeurs par défaut.
    ' - Le bouton « œil », icône seule, garde l'image du Designer (règle des petits boutons du kit).

    Private Sub ConfigurationConnexion_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Fond beige à la feuille et barre de titre aux couleurs du kit.
        AltheaFondFenetre.Appliquer(Me)

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
    ' Version    : V2.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Teste la connexion avec les valeurs saisies (mot de passe stocké ou saisi selon le mode) et dit
    ' le résultat par un message : réussite et étape suivante, ou ce qu'il faut vérifier.
    '
    ' Remarques  :
    ' - Le mot de passe n'est jamais affiché en clair.
    ' - Le test ne touche pas à la connexion de l'application (DatabaseManager.TesterConfiguration) :
    '   un essai raté ne la casse plus ; elle n'est remplacée qu'à l'enregistrement.
    ' - Demande de Joëlle (20/10/2026) : la barre d'état seule ne suffit pas, l'utilisateur croyait
    '   qu'il ne se passait rien ; dans cet écran critique, chaque refus ou échec a son message.
    ' - V1.3.1 (20/10/2026) : bloc de test DialogChoix oublié retiré (audit B1).
    ' - V2.0.0 (20/10/2026) : messages (DialogChoix) avec la cause de l'échec ; champs manquants
    '   nommés ; test isolé de la connexion de l'application.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTesterConnexion_Click(sender As Object, e As EventArgs) Handles btnTesterConnexion.Click

        _isConnectionTestSuccessful = False

        Dim manquants As List(Of String) = ChampsManquants()
        If manquants.Count > 0 Then

            lblConnectionResult.Text = "Champs obligatoires incomplets : test non lancé"
            SetStatus("Test impossible - champs incomplets")
            UpdateFormState()

            DialogChoix.Information(
                "Le test n'a pas été lancé : complétez d'abord " & String.Join(", ", manquants) & ".",
                "Champs incomplets")
            Return

        End If

        Dim config As LocalDbConfig = BuildConfigFromForm()
        Dim resultat As DatabaseManager.ResultatTestConnexion

        Cursor = Cursors.WaitCursor
        Try
            resultat = DatabaseManager.TesterConfiguration(config, MotDePasseAUtiliser())
        Catch ex As Security.Cryptography.CryptographicException
            ' Mot de passe chiffré par un autre compte Windows ou un autre poste (configuration copiée).
            Cursor = Cursors.Default
            GestionLog.EcrireException("ConfigurationConnexion - mot de passe stocké illisible sur ce poste", ex,
                                       GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)
            lblConnectionResult.Text = "Mot de passe enregistré illisible : voir le message"
            UpdateFormState()
            DialogChoix.Erreur("Le mot de passe enregistré ne peut pas être relu sur ce poste ou par ce compte Windows " &
                               "(il est chiffré pour le compte qui l'a saisi)." & Environment.NewLine & Environment.NewLine &
                               "Cliquez sur « Modifier » pour le saisir de nouveau.",
                               "Connexion impossible")
            Return
        Catch ex As Exception
            GestionLog.EcrireException("ConfigurationConnexion - Erreur test connexion", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.Database)
            resultat = DatabaseManager.ResultatTestConnexion.Erreur
        Finally
            Cursor = Cursors.Default
        End Try

        If resultat = DatabaseManager.ResultatTestConnexion.Reussi Then

            _isConnectionTestSuccessful = True
            _isFormDirty = False
            lblConnectionResult.Text = "Connexion validée avec succès"
            SetStatus("Connexion OK")
            UpdateFormState()

            GestionLog.EcrireLog("Test connexion réussi", GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)

            DialogChoix.Succes(
                $"La connexion à la base « {config.DatabaseName} » sur « {config.Host} » fonctionne." & Environment.NewLine &
                "Cliquez sur « Enregistrer » pour garder cette configuration.",
                "Connexion réussie")
            Return

        End If

        lblConnectionResult.Text = "Échec de la connexion : voir le message"
        SetStatus("Test échoué")
        UpdateFormState()

        DialogChoix.Erreur(MessageEchec(resultat, config), "Connexion impossible")

    End Sub

    ' Mot de passe du test et de l'application : celui stocké (déchiffré) ou celui saisi, selon le mode.
    Private Function MotDePasseAUtiliser() As String
        Return If(_passwordMode = PasswordMode.KeepExisting,
                  CryptoManagerDPAPI.Unprotect(_storedEncryptedPassword),
                  txtPassword.Text)
    End Function

    ' Ce qu'il faut vérifier, selon la cause de l'échec (pas de détail technique : il est au journal).
    Private Function MessageEchec(resultat As DatabaseManager.ResultatTestConnexion, config As LocalDbConfig) As String

        Dim nl As String = Environment.NewLine

        Select Case resultat
            Case DatabaseManager.ResultatTestConnexion.ServeurInjoignable
                Return $"Althéa n'a pas pu joindre le serveur MariaDB « {config.Host} » (port {config.Port})." & nl & nl &
                       "Vérifiez :" & nl &
                       "• le nom ou l'adresse du serveur, et le port (3306 par défaut) ;" & nl &
                       "• que le service MariaDB est démarré sur ce serveur ;" & nl &
                       "• qu'aucun pare-feu ne bloque la connexion."
            Case DatabaseManager.ResultatTestConnexion.AccesRefuse
                Dim conseil As String = If(_passwordMode = PasswordMode.KeepExisting,
                    "Le mot de passe enregistré n'est peut-être plus le bon : cliquez sur « Modifier » pour en saisir un autre.",
                    "Vérifiez le nom du compte et le mot de passe (majuscules comprises).")
                Return $"Le serveur a refusé le compte « {config.UserName} » : compte ou mot de passe incorrect, ou compte non autorisé depuis ce poste." & nl & nl & conseil
            Case DatabaseManager.ResultatTestConnexion.BaseInconnue
                Return $"Le serveur répond, mais la base « {config.DatabaseName} » n'existe pas, ou ce compte n'y a pas accès." & nl & nl &
                       "Vérifiez le nom de la base."
            Case DatabaseManager.ResultatTestConnexion.OptionsInvalides
                Return "Les options supplémentaires ne sont pas valides." & nl & nl &
                       "Elles s'écrivent « Nom=valeur; » (par exemple « SslMode=Required; ») ; laissez le champ vide en cas de doute."
            Case Else
                Return "La connexion a échoué pour une raison inattendue." & nl & nl &
                       "Le détail technique est dans le journal d'Althéa."
        End Select

    End Function


    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnEnregistrerConnexion_Click
    ' Version    : V1.4.0
    ' Date       : 20/10/2026
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
    ' - V1.3.1 (20/10/2026) : mot de passe conservé selon le mode KeepExisting (audit B2) : il était
    '   remplacé par un mot de passe vide.
    ' - V1.4.0 (20/10/2026) : messages : test requis, enregistrement réussi ou impossible ; configuration appliquée à l'application (Initialize).
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

                DialogChoix.Information(
                    "Testez d'abord la connexion : l'enregistrement n'est possible qu'après un test réussi, " &
                    "pour ne pas garder une configuration qui empêcherait Althéa de démarrer.",
                    "Test requis")
                Return
            End If

            ' Construction de la configuration
            Dim config As LocalDbConfig = BuildConfigFromForm()

            ' -------------------------------------------------------------------------------------------------
            ' Gestion du mot de passe
            ' -------------------------------------------------------------------------------------------------

            If _passwordMode = PasswordMode.KeepExisting AndAlso _hasStoredEncryptedPassword Then
                ' Cas 1 : mot de passe conservé (bouton Modifier non utilisé) → on garde l'ancien
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

            ' La configuration testée puis enregistrée devient celle de l'application (le test ne la
            ' touche plus).
            DatabaseManager.Initialize(config, MotDePasseAUtiliser())

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

            DialogChoix.Succes("La configuration de connexion est enregistrée ; Althéa l'utilise dès maintenant.",
                               "Configuration enregistrée")

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

            DialogChoix.Erreur("La configuration n'a pas pu être enregistrée. Le détail technique est dans le journal d'Althéa.",
                               "Enregistrement impossible")

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
    ' Version    : V1.3.0
    ' Date       : 20/10/2026
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
    ' - V1.3.0 (20/10/2026) : s'appuie sur ChampsManquants (champs nommés dans le message du test).
    ' -------------------------------------------------------------------------------------------------
    Private Function AreRequiredFieldsValid() As Boolean
        Return ChampsManquants().Count = 0
    End Function

    ' Champs obligatoires encore vides, nommés pour le message (« le serveur, le compte »).
    Private Function ChampsManquants() As List(Of String)

        Dim manquants As New List(Of String)

        Dim hasPassword As Boolean = If(_passwordMode = PasswordMode.KeepExisting,
                                        Not String.IsNullOrWhiteSpace(_storedEncryptedPassword),
                                        Not String.IsNullOrWhiteSpace(txtPassword.Text))

        If String.IsNullOrWhiteSpace(txtHost.Text) Then manquants.Add("le serveur")
        If nudPort.Value <= 0 Then manquants.Add("le port")
        If String.IsNullOrWhiteSpace(txtDatabaseName.Text) Then manquants.Add("le nom de la base")
        If String.IsNullOrWhiteSpace(txtUserName.Text) Then manquants.Add("le compte")
        If Not hasPassword Then manquants.Add("le mot de passe")

        Return manquants

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
    ' Version    : V1.3.0
    ' Date       : 20/10/2026
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
    ' - V1.3.0 (20/10/2026) : message si la configuration enregistrée est illisible.
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

            DialogChoix.Avertissement("La configuration enregistrée n'a pas pu être lue : saisissez de nouveau les paramètres de connexion.",
                                      "Configuration illisible")

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