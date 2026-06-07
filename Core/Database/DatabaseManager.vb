' -------------------------------------------------------------------------------------------------
' Module      : DatabaseManager
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 21/04/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise la gestion de la connexion à la base de données MariaDB.
' Point unique de création et d'ouverture des connexions.
'
' Responsabilités :
' - Construire la connection string depuis LocalDbConfig
' - Créer des connexions MySqlConnection
' - Ouvrir des connexions MySqlConnection
' - Tester la connexion à la base de données
' - Stocker la connection string en mémoire (variable privée)
'
' Remarques   :
' - Ne contient AUCUNE logique de lecture JSON (ConfigManager)
' - Ne contient AUCUNE logique de chiffrement (CryptoManagerDPAPI)
' - Le mot de passe doit être fourni déjà déchiffré
' - Module statique (variables et méthodes partagées)
'
' Dépendances :
' - LocalDbConfig (pour les paramètres de connexion)
' - MySqlConnector (bibliothèque de connexion MariaDB/MySQL)
'
' Imports     :
' - MySqlConnector (pour MySqlConnection)
' - System.Data (pour ConnectionState)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System
Imports System.Data
Imports MySqlConnector


Public Module DatabaseManager

#Region "Variables privées"

    ' Champ privé pour stocker la connection string construite à partir de la configuration
    Private _connectionString As String = String.Empty

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Initialize
    ' Version    : V1.0.0
    ' Date       : 21/04/2026
    '
    ' Rôle       :
    ' Initialise le DatabaseManager avec une configuration DB et un mot de passe déchiffré.
    ' Construit la connection string MySqlConnector.
    '
    ' Paramètres :
    ' - config            : Configuration de connexion à la base de données (LocalDbConfig)
    ' - decryptedPassword : Mot de passe en clair (déjà déchiffré par CryptoManagerDPAPI)
    '
    ' Remarques  :
    ' - La configuration doit être valide (IsConfigurationValid = True)
    ' - Aucune validation de la configuration n'est faite ici (responsabilité de l'appelant)
    ' - Le mot de passe doit être fourni en clair (déchiffrement fait avant l'appel)
    ' - Doit être appelée avant toute utilisation de CreateConnection ou OpenConnection
    '
    ' Exceptions :
    ' - ArgumentNullException : si config est Nothing
    ' - ArgumentException     : si decryptedPassword est vide ou Nothing
    ' -------------------------------------------------------------------------------------------------
    Public Sub Initialize(config As LocalDbConfig, decryptedPassword As String)

        If config Is Nothing Then
            Throw New ArgumentNullException(NameOf(config))
        End If

        If String.IsNullOrWhiteSpace(decryptedPassword) Then
            Throw New ArgumentException("Mot de passe invalide.", NameOf(decryptedPassword))
        End If

        _connectionString =
            $"Server={config.Host};" &
            $"Port={config.Port};" &
            $"Database={config.DatabaseName};" &
            $"User ID={config.UserName};" &
            $"Password={decryptedPassword};" &
            $"{config.AdditionalOptions}"

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : IsInitialized
    ' Version    : V1.0.0
    ' Date       : 21/04/2026
    '
    ' Type       : Boolean (ReadOnly)
    '
    ' Description : Indique si le DatabaseManager a été initialisé avec une connection string valide
    '
    ' Rôle       :
    ' Permet de vérifier si Initialize() a été appelée avant de tenter de créer des connexions.
    '
    ' Retour     :
    ' - True  : DatabaseManager initialisé, connexions possibles
    ' - False : DatabaseManager non initialisé, Initialize() doit être appelée
    '
    ' Remarques  :
    ' - Un DatabaseManager non initialisé lèvera une InvalidOperationException
    '   si CreateConnection() ou OpenConnection() sont appelées
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property IsInitialized As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(_connectionString)
        End Get
    End Property

#End Region

#Region "Connexion"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreateConnection
    ' Version    : V1.0.0
    ' Date       : 21/04/2026
    '
    ' Rôle       :
    ' Crée une nouvelle instance de MySqlConnection avec la connection string configurée.
    '
    ' Retour     :
    ' - MySqlConnection : Connexion non ouverte (l'appelant doit appeler .Open())
    '
    ' Remarques  :
    ' - Le DatabaseManager est le seul point de création de connexions
    ' - Garantit que toutes les connexions utilisent la même configuration
    ' - L'appelant doit utiliser Using pour garantir la libération des ressources
    '
    ' Exceptions :
    ' - InvalidOperationException : si DatabaseManager non initialisé (IsInitialized = False)
    ' -------------------------------------------------------------------------------------------------
    Public Function CreateConnection() As MySqlConnection

        If Not IsInitialized Then
            Throw New InvalidOperationException("DatabaseManager non initialisé.")
        End If

        Return New MySqlConnection(_connectionString)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : OpenConnection
    ' Version    : V1.0.0
    ' Date       : 28/04/2026
    '
    ' Rôle       :
    ' Crée et ouvre immédiatement une nouvelle connexion MySQL.
    '
    ' Retour     :
    ' - MySqlConnection : Connexion ouverte et prête à l'emploi
    '
    ' Remarques  :
    ' - L'appelant doit utiliser Using pour garantir la fermeture et libération
    ' - En cas d'échec d'ouverture, la connexion est automatiquement libérée
    ' - Le DatabaseManager reste le seul point de création/ouverture de connexion
    '
    ' Exceptions :
    ' - InvalidOperationException : si DatabaseManager non initialisé
    ' - MySqlException            : si l'ouverture de la connexion échoue
    ' -------------------------------------------------------------------------------------------------
    Public Function OpenConnection() As MySqlConnection

        Dim connection As MySqlConnection = CreateConnection()

        Try
            connection.Open()
            Return connection

        Catch
            connection.Dispose()
            Throw
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TestConnection
    ' Version    : V1.0.0
    ' Date       : 28/04/2026
    '
    ' Rôle       :
    ' Teste la connexion à la base de données MariaDB.
    '
    ' Retour     :
    ' - Boolean : True si connexion établie avec succès, False sinon
    '
    ' Remarques  :
    ' - Utilise CreateConnection pour garantir que la même configuration est testée
    ' - Ne lève aucune exception (capture interne)
    ' - Ferme automatiquement la connexion après le test (Using)
    ' - Utilisé par AppStartupManager et ConfigurationConnexion
    '
    ' Cas de retour False :
    ' - DatabaseManager non initialisé
    ' - Serveur MariaDB inaccessible
    ' - Identifiants invalides
    ' - Base de données inexistante
    ' - Exception MySqlConnector levée
    ' -------------------------------------------------------------------------------------------------
    Public Function TestConnection() As Boolean

        If Not IsInitialized Then
            Return False
        End If

        Try
            Using connection As MySqlConnection = CreateConnection()
                connection.Open()
                Return connection.State = ConnectionState.Open
            End Using
        Catch
            Return False
        End Try

    End Function

#End Region

End Module
