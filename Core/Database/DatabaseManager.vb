' -------------------------------------------------------------------------------------------------
' Module      : DatabaseManager
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 22/10/2026
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
' - Ouvrir, pour une opération d'administration ponctuelle (restauration), une connexion à la même
'   base avec un autre compte saisi par l'utilisateur (OpenConnectionAvecCompte)
'
' Remarques   :
' - Ne contient AUCUNE logique de lecture JSON (ConfigManager)
' - Ne contient AUCUNE logique de chiffrement (CryptoManagerDPAPI)
' - Le mot de passe doit être fourni déjà déchiffré
' - Module statique (variables et méthodes partagées)
' - V1.2.0 (20/10/2026, audit § 3.3 et § 4.5) : chaîne de connexion construite par
'   MySqlConnectionStringBuilder (un mot de passe contenant « ; » ou « " » ne la casse plus) ;
'   TesterConfiguration teste une configuration saisie sans toucher à la connexion de l'application
'   et rend la cause d'un échec (serveur, compte, base, options) ; la cause est journalisée.
' - V1.3.0 (21/10/2026, audit B.2) : ExecuterEnTransaction, unité de travail « tout ou rien » pour
'   les opérations métier en plusieurs écritures.
' - V1.4.0 (22/10/2026, audit C.1) : UseAffectedRows forcé à False : ExecuteNonQuery compte les
'   lignes trouvées, même inchangées ; les écritures conditionnelles (« 0 ligne = refus ») en dépendent.
'
' Dépendances :
' - LocalDbConfig (pour les paramètres de connexion)
' - GestionLog (cause d'un test échoué, sans mot de passe)
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
    ' Version    : V1.1.0
    ' Date       : 20/10/2026
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
    ' - V1.1.0 (20/10/2026) : chaîne construite par MySqlConnectionStringBuilder (ConstruireChaineConnexion).
    ' -------------------------------------------------------------------------------------------------
    Public Sub Initialize(config As LocalDbConfig, decryptedPassword As String)

        If config Is Nothing Then
            Throw New ArgumentNullException(NameOf(config))
        End If

        If String.IsNullOrWhiteSpace(decryptedPassword) Then
            Throw New ArgumentException("Mot de passe invalide.", NameOf(decryptedPassword))
        End If

        _connectionString = ConstruireChaineConnexion(config, decryptedPassword)

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
    ' Fonction   : ExecuterEnTransaction
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Unité de travail : ouvre une connexion et une transaction, exécute le travail fourni, puis
    ' valide (Commit). Toute exception annule l'ensemble (Rollback) avant d'être propagée : une
    ' opération en plusieurs écritures s'applique entièrement ou pas du tout.
    '
    ' Paramètres :
    ' - travail : Écritures à réaliser ; chaque commande doit recevoir la connexion ET la transaction
    '   fournies (MySqlConnector refuse une commande sans transaction sur une connexion qui en a une)
    '
    ' Retour     :
    ' - T : Valeur rendue par le travail (ex. identifiant créé) ; variante Sub pour un travail sans retour
    '
    ' Remarques  :
    ' - Les routines appelées à l'intérieur reçoivent (conn, tx) et n'ouvrent pas leur propre
    '   connexion pour écrire. Une lecture de référentiel faite avant l'appel reste possible.
    ' - Un échec du Rollback lui-même (connexion perdue) est journalisé sans masquer l'erreur d'origine ;
    '   MariaDB annule de toute façon une transaction non validée à la fermeture de la connexion.
    ' -------------------------------------------------------------------------------------------------
    Public Function ExecuterEnTransaction(Of T)(travail As Func(Of MySqlConnection, MySqlTransaction, T)) As T

        If travail Is Nothing Then
            Throw New ArgumentNullException(NameOf(travail))
        End If

        Using conn As MySqlConnection = OpenConnection()
            Using tx As MySqlTransaction = conn.BeginTransaction()
                Try
                    Dim resultat As T = travail(conn, tx)
                    tx.Commit()
                    Return resultat
                Catch
                    AnnulerTransactionSansMasquer(tx)
                    Throw
                End Try
            End Using
        End Using

    End Function

    Public Sub ExecuterEnTransaction(travail As Action(Of MySqlConnection, MySqlTransaction))

        If travail Is Nothing Then
            Throw New ArgumentNullException(NameOf(travail))
        End If

        ExecuterEnTransaction(Of Boolean)(
            Function(conn, tx)
                travail(conn, tx)
                Return True
            End Function)

    End Sub

    ' Rollback protégé : l'erreur d'origine doit rester celle qui remonte.
    Private Sub AnnulerTransactionSansMasquer(tx As MySqlTransaction)
        Try
            tx.Rollback()
        Catch exRollback As Exception
            GestionLog.EcrireLog(
                "Annulation de transaction impossible (connexion perdue ?) ; MariaDB l'annulera à la fermeture.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                exRollback
            )
        End Try
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TestConnection
    ' Version    : V1.1.0
    ' Date       : 20/10/2026
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
    ' - V1.1.0 (20/10/2026) : cause d'un échec journalisée (Tester) ; plus d'exception avalée en silence.
    ' -------------------------------------------------------------------------------------------------
    Public Function TestConnection() As Boolean

        If Not IsInitialized Then
            Return False
        End If

        Return Tester(_connectionString) = ResultatTestConnexion.Reussi

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : OpenConnectionAvecCompte
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Ouvre une connexion au même serveur et à la même base que la configuration courante, mais avec
    ' un autre compte MariaDB (compte d'administration saisi pour une restauration).
    '
    ' Remarques  :
    ' - Le compte de l'application n'a que des droits sur les données (moindre privilège) ; recréer
    '   les tables exige un compte d'administration, utilisé pour cette seule opération.
    ' - Pooling désactivé : la connexion d'administration ne reste pas ouverte dans le pool après usage.
    ' - Le mot de passe n'est ni conservé ni journalisé.
    '
    ' Exceptions :
    ' - InvalidOperationException : si DatabaseManager non initialisé
    ' - MySqlException            : compte refusé, serveur injoignable
    ' -------------------------------------------------------------------------------------------------
    Public Function OpenConnectionAvecCompte(compte As String, motDePasse As String) As MySqlConnection

        If Not IsInitialized Then
            Throw New InvalidOperationException("DatabaseManager non initialisé.")
        End If

        Dim builder As New MySqlConnectionStringBuilder(_connectionString) With {
            .UserID = compte,
            .Password = motDePasse,
            .Pooling = False
        }

        Dim connection As New MySqlConnection(builder.ConnectionString)
        Try
            connection.Open()
            Return connection
        Catch
            connection.Dispose()
            Throw
        End Try

    End Function

#End Region

#Region "Test d'une configuration"

    ' Durée maximale d'un test lancé depuis l'écran de configuration (secondes) : au-delà, le
    ' serveur est jugé injoignable plutôt que de laisser la fenêtre figée.
    Private Const DELAI_TEST_SECONDES As UInteger = 8

    ' Résultat d'un test de connexion, pour un message qui dit quoi vérifier.
    Public Enum ResultatTestConnexion
        Reussi = 0
        ServeurInjoignable = 1
        AccesRefuse = 2
        BaseInconnue = 3
        OptionsInvalides = 4
        Erreur = 5
    End Enum

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TesterConfiguration
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Teste une configuration saisie (écran de configuration) et rend la cause d'un échec.
    ' Remarques  :
    ' - Ne touche pas à la connexion de l'application : un test raté ne la casse plus (audit § 4.5) ;
    '   la configuration n'est appliquée qu'une fois enregistrée (Initialize).
    ' - Sans pool (la connexion d'essai ne reste pas ouverte) et avec un délai court.
    ' -------------------------------------------------------------------------------------------------
    Public Function TesterConfiguration(config As LocalDbConfig, motDePasse As String) As ResultatTestConnexion

        ArgumentNullException.ThrowIfNull(config)

        Dim chaine As String
        Try
            Dim builder As New MySqlConnectionStringBuilder(ConstruireChaineConnexion(config, motDePasse)) With {
                .Pooling = False,
                .ConnectionTimeout = DELAI_TEST_SECONDES
            }
            chaine = builder.ConnectionString
        Catch ex As ArgumentException
            GestionLog.EcrireException("Test de connexion : options de connexion invalides.", ex,
                                       GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)
            Return ResultatTestConnexion.OptionsInvalides
        End Try

        Return Tester(chaine)

    End Function

    ' Ouvre puis ferme une connexion ; la cause d'un échec est journalisée (sans mot de passe : le
    ' message de MariaDB ne cite que le compte et le poste).
    Private Function Tester(chaine As String) As ResultatTestConnexion

        Try
            Using connection As New MySqlConnection(chaine)
                connection.Open()
                Return If(connection.State = ConnectionState.Open, ResultatTestConnexion.Reussi, ResultatTestConnexion.Erreur)
            End Using
        Catch ex As MySqlException
            Dim cause As ResultatTestConnexion = CauseEchec(ex)
            GestionLog.EcrireException($"Test de connexion à la base échoué ({cause}).", ex,
                                       GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)
            Return cause
        Catch ex As Exception
            GestionLog.EcrireException("Test de connexion à la base échoué (Erreur).", ex,
                                       GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)
            Return ResultatTestConnexion.Erreur
        End Try

    End Function

    Private Function CauseEchec(ex As MySqlException) As ResultatTestConnexion

        Select Case ex.ErrorCode
            Case MySqlErrorCode.UnableToConnectToHost
                Return ResultatTestConnexion.ServeurInjoignable
            Case MySqlErrorCode.AccessDenied, MySqlErrorCode.DatabaseAccessDenied
                Return ResultatTestConnexion.AccesRefuse
            Case MySqlErrorCode.UnknownDatabase
                Return ResultatTestConnexion.BaseInconnue
            Case Else
                Return ResultatTestConnexion.Erreur
        End Select

    End Function

    ' Options supplémentaires d'abord (« SslMode=Required; »…), puis les champs de la configuration,
    ' qui l'emportent. Le constructeur protège les valeurs (« ; » ou « " » dans un mot de passe).
    ' UseAffectedRows = False (valeur par défaut, rendue explicite) : ExecuteNonQuery rend le nombre de
    ' lignes TROUVÉES, même si leurs valeurs ne changent pas. Les écritures conditionnelles en dépendent
    ' (0 ligne = condition non remplie = refus) : une option contraire les ferait refuser à tort.
    Private Function ConstruireChaineConnexion(config As LocalDbConfig, motDePasse As String) As String

        Dim builder As New MySqlConnectionStringBuilder(If(config.AdditionalOptions, String.Empty)) With {
            .Server = config.Host,
            .Port = CUInt(config.Port),
            .Database = config.DatabaseName,
            .UserID = config.UserName,
            .Password = motDePasse,
            .UseAffectedRows = False
        }

        Return builder.ConnectionString

    End Function

#End Region

End Module
