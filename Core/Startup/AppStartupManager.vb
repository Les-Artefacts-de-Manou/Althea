' -------------------------------------------------------------------------------------------------
' Classe      : AppStartupManager
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 24/04/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gère le processus de démarrage de l'application.
' Orchestre la séquence : Configuration -> Connexion DB -> Home.
'
' Responsabilités :
' - Charger la configuration locale depuis le fichier JSON
' - Tester la connexion à la base de données
' - Ouvrir la fenêtre de configuration si nécessaire
' - Lancer Home uniquement si la connexion DB est validée
' - Journaliser toutes les étapes du démarrage
'
' Flux de démarrage :
' 1. Tentative de chargement de la configuration locale
' 2. Si configuration valide -> Test de connexion DB
' 3. Si connexion OK -> Lancement de Home
' 4. Sinon -> Ouverture de ConfigurationConnexion
' 5. Après validation manuelle -> Nouveau test -> Lancement de Home
' 6. Si annulation ou échec -> Fermeture de l'application
'
' Remarques   :
' - Ne contient pas de logique métier
' - Ne contient pas de requêtes SQL
' - Orchestre uniquement le démarrage applicatif
' - Classe statique (méthodes Shared uniquement)
'
' Dépendances :
' - ConfigManager (chargement configuration)
' - DatabaseManager (test connexion)
' - CryptoManagerDPAPI (déchiffrement mot de passe)
' - GestionLog (journalisation)
' - Home (formulaire principal)
' - ConfigurationConnexion (formulaire de configuration)
'
' Imports     :
' - System.Windows.Forms (pour Application.Run et DialogResult)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Windows.Forms

Public Class AppStartupManager

#Region "Méthode publique - Point d'entrée"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Run
    ' Version    : V1.1.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Lance le flux de démarrage complet de l'application Althéa.
    ' Point d'entrée principal appelé depuis Program.vb.
    '
    ' Flux       :
    ' 1. Journalise le démarrage
    ' 2. Tente d'initialiser la connexion DB depuis la configuration locale
    ' 3a. Si succès -> Lance Home (Application.Run)
    ' 3b. Si échec -> Ouvre ConfigurationConnexion
    ' 4. Si configuration validée manuellement -> Relance le test -> Lance Home
    ' 5. Si annulation ou échec persistant -> Ferme l'application
    '
    ' Remarques  :
    ' - Méthode statique (Shared)
    ' - Bloquante jusqu'à la fermeture de Home ou annulation de la configuration
    ' - Toutes les étapes sont journalisées dans GestionLog
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub Run()

        GestionLog.EcrireLog(
            "Démarrage du processus startup Althéa",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.Startup)

        If TryInitializeDatabaseFromConfig() Then

            GestionLog.EcrireLog(
                "Connexion DB validée au démarrage - ouverture Home",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Startup)

            Application.Run(New Home())
            Return

        End If

        GestionLog.EcrireLog(
            "Configuration DB absente ou invalide - ouverture ConfigurationConnexion",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.Startup)

        Using configForm As New ConfigurationConnexion()

            Dim result As DialogResult = configForm.ShowDialog()

            If result = DialogResult.OK AndAlso TryInitializeDatabaseFromConfig() Then

                GestionLog.EcrireLog(
                    "Configuration validée - ouverture Home",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Startup)

                Application.Run(New Home())
                Return

            End If

        End Using

        GestionLog.EcrireLog(
            "Démarrage annulé - aucune connexion DB valide",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.Startup)

    End Sub

#End Region

#Region "Méthodes privées"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TryInitializeDatabaseFromConfig
    ' Version    : V1.1.0
    ' Date       : 24/04/2026
    '
    ' Rôle       :
    ' Tente d'initialiser la connexion à la base de données depuis la configuration locale.
    '
    ' Processus  :
    ' 1. Charge LocalDbConfig depuis ConfigManager
    ' 2. Vérifie la validité minimale de la configuration
    ' 3. Déchiffre le mot de passe avec CryptoManagerDPAPI
    ' 4. Initialise DatabaseManager avec la configuration
    ' 5. Teste la connexion effective à la base de données
    '
    ' Retour     :
    ' - Boolean : True si la connexion DB est établie avec succès, False sinon
    '
    ' Remarques  :
    ' - Méthode statique (Shared)
    ' - Capture toutes les exceptions et retourne False en cas d'erreur
    ' - Journalise chaque étape et les erreurs rencontrées
    ' - Ne lève aucune exception (gestion interne)
    '
    ' Cas de retour False :
    ' - Aucune configuration locale trouvée (fichier JSON absent)
    ' - Configuration incomplète ou invalide
    ' - Échec du déchiffrement du mot de passe
    ' - Échec du test de connexion à la base de données
    ' - Exception levée durant le processus
    ' -------------------------------------------------------------------------------------------------
    Private Shared Function TryInitializeDatabaseFromConfig() As Boolean

        Try
            Dim config As LocalDbConfig = ConfigManager.LoadLocalDbConfig()

            If config Is Nothing Then

                GestionLog.EcrireLog(
                    "Aucune configuration locale trouvée",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Startup)

                Return False

            End If

            If Not config.IsConfigurationValid Then

                GestionLog.EcrireLog(
                    "Configuration locale incomplète ou invalide",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Startup)

                Return False

            End If

            Dim decryptedPassword As String = CryptoManagerDPAPI.Unprotect(config.EncryptedPassword)

            DatabaseManager.Initialize(config, decryptedPassword)

            Dim isConnected As Boolean = DatabaseManager.TestConnection()

            If Not isConnected Then

                GestionLog.EcrireLog(
                    "Test DB échoué depuis la configuration locale",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Database)

                Return False

            End If

            GestionLog.EcrireLog(
                "Test DB réussi depuis la configuration locale",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database)

            Return True

        Catch ex As Exception

            GestionLog.EcrireException(
                "AppStartupManager - Erreur lors de l'initialisation DB depuis la configuration locale",
                ex,
                GestionLog.LogLevel.Complet,
                GestionLog.LogCategory.Startup)

            Return False

        End Try

    End Function

#End Region

End Class