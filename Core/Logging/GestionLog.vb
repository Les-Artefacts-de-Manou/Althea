' -------------------------------------------------------------------------------------------------
' Module      : GestionLog
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 22/04/2026
' Auteur      :  Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise la gestion du logging dans l'application.
'
' Responsabilités :
' - Log fichier de session journalier : %APPDATA%\Althea\Logs\Althea_YYYY-MM-DD.log
' - Création dossier/fichier au premier log
' - Purge > 7 jours
' - Niveaux = marqueurs de profondeur (pas de filtre global)
' - Support exceptions : ex.Message (Succinct) + StackTrace (Complet)
' - Thread-safe (SyncLock)
' - Masquage rudimentaire des secrets (Password=..., Pwd=...)
'
' Remarques   :
' - Ne contient aucune logique de chiffrement ou de lecture JSON (ConfigManager)
'
' Dépendances :
' - ConfigManager (pour le chemin de base des logs)
' - LocalDbConfig (pour le chemin de base des logs)
'
' Imports     :
' - System.IO (pour gestion fichiers)
' - System.Text (pour StringBuilder)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text

Public Module GestionLog

#Region "Constantes et variables privées"

    ' Durée de rétention des logs en jours (logs plus vieux que cette durée seront purgés)
    Private Const RetentionDays As Integer = 7

    ' Objet de synchronisation pour garantir que les opérations de log sont thread-safe
    Private ReadOnly _lockObj As New Object()

    ' Flag pour indiquer si le header de session a déjà été écrit (pour éviter les doublons)
    Private _sessionHeaderWritten As Boolean = False

    ' Chemin du fichier de log pour le header de session
    Private _sessionHeaderLogFile As String = ""

    ' Flag pour indiquer si la purge des anciens logs a deja ete effectuee
    Private _oldLogsPurged As Boolean = False

#End Region

#Region "Énumérations"

    ' Niveaux de log (marqueurs de profondeur, pas de filtre global)
    Public Enum LogLevel
        Rapide = 1
        Succinct = 2
        Complet = 3
    End Enum

    ' Catégories de log (pour structurer les messages, pas de filtre global)
    Public Enum LogCategory
        General = 0
        Startup = 1
        Database = 2
        UI = 3
        Process = 4
        Security = 5
    End Enum

#End Region

#Region "Méthodes publiques - Écriture des logs"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EcrireLog
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Écrit un message de log (sans filtre global).
    ' - Ajoute un header de session au premier log de l'exécution.
    '
    ' Paramètres :
    ' - message (String)       : Le message de log.
    ' - level (LogLevel)       : Le niveau de log (rapide, succinct, complet).
    ' - category (LogCategory) : La catégorie de log (general, startup, database, ui, process, security).
    ' - ex (Exception)         : L'exception associée au log (optionnel).
    '
    ' Remarques  :
    ' - La procédure est thread-safe grâce à l'utilisation de SyncLock.
    ' - Les messages de log sont masqués pour les secrets avant d'être écrits.
    '
    ' Exceptions :
    ' - En cas d'erreur d'écriture (ex: fichier verrouillé), l'exception est attrapée et un message
    '   est écrit dans la console de debug, mais l'application continue de fonctionner.
    ' -------------------------------------------------------------------------------------------------
    Public Sub EcrireLog(message As String,
                    Optional level As LogLevel = LogLevel.Succinct,
                    Optional category As LogCategory = LogCategory.General,
                    Optional ex As Exception = Nothing)

        Try
            Dim safeMessage As String = MaskSensitive(message)
            Dim line As String = BuildLogLine(safeMessage, level, category, ex)

            SyncLock _lockObj

                EnsureLogFolder()
                EnsureOldLogsPurged()

                Dim logFile As String = GetDailyLogFilePath()

                ' ✅ Header de session (une seule fois par run)
                EnsureSessionHeader(logFile)

                File.AppendAllText(logFile, line & Environment.NewLine, Encoding.UTF8)

            End SyncLock

            Debug.WriteLine(line)

        Catch ioEx As Exception
            Debug.WriteLine("GestionLog: erreur d'écriture log: " & ioEx.Message)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EcrireException
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Helper explicite pour journaliser une exception.
    '
    ' Paramètres :
    ' - contextMessage (String) : Le message contextuel associé à l'exception.
    ' - ex (Exception)          : L'exception à journaliser.
    ' - level (LogLevel)        : Le niveau de log (rapide, succinct, complet).
    ' - category (LogCategory)  : La catégorie de log (general, startup, database, ui, process, security).
    '
    ' Remarques  :
    ' - Cette procédure utilise EcrireLog en interne.
    ' -------------------------------------------------------------------------------------------------
    Public Sub EcrireException(contextMessage As String,
                              ex As Exception,
                              Optional level As LogLevel = LogLevel.Succinct,
                              Optional category As LogCategory = LogCategory.General)

        EcrireLog(contextMessage, level, category, ex)

    End Sub

#End Region

#Region "Gestion du cycle de vie des logs"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EnsureOldLogsPurged
    ' Version    : V1.1.0
    ' Date       : 17/06/2026
    '
    ' Rôle       :
    ' - Lance la purge des anciens logs une seule fois par session.
    ' - La purge ne doit pas être exécutée à chaque écriture de log.
    '
    ' Remarques  :
    ' - La purge est effectuée au moment du premier log, ce qui garantit que les anciens logs
    '   sont nettoyés avant d'écrire de nouveaux logs.
    ' -------------------------------------------------------------------------------------------------
    Private Sub EnsureOldLogsPurged()

        If _oldLogsPurged Then
            Return
        End If

        PurgeOldLogs()
        _oldLogsPurged = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EnsureLogFolder
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Crée %APPDATA%\Althea\Logs si nécessaire.
    '
    ' Remarques  :
    ' - La création ne doit pas bloquer l'application (fichier verrouillé / droits).
    ' -------------------------------------------------------------------------------------------------

    Private Sub EnsureLogFolder()

        Dim folder As String = GetLogFolderPath()
        If Not Directory.Exists(folder) Then
            Directory.CreateDirectory(folder)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EnsureSessionHeader
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Écrit un séparateur au 1er log de l'exécution.
    ' - Si le fichier de log change (ex: passage de minuit), ré-écrit un header.
    ' - Le header inclut un timestamp de session, le nom de la machine et de l'utilisateur.
    ' - Permet de différencier les sessions d'exécution dans les fichiers de log quotidiens.
    '
    ' Paramètres :
    ' - logFile : Le chemin vers le fichier de log actuel.
    '
    ' Remarques  :
    ' - Le header de session est écrit une seule fois par session d'exécution,
    '   même si plusieurs logs sont écrits.
    '
    ' Exceptions :
    ' - En cas d'erreur d'écriture (ex: fichier verrouillé), l'exception est attrapée et un message
    '   est écrit dans la console de debug, mais l'application continue de fonctionner.
    ' -------------------------------------------------------------------------------------------------
    Private Sub EnsureSessionHeader(logFile As String)

        If _sessionHeaderWritten AndAlso String.Equals(_sessionHeaderLogFile, logFile, StringComparison.OrdinalIgnoreCase) Then
            Exit Sub
        End If

        Dim sep As String = New String("-"c, 80)
        Dim ts As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim machine As String = Environment.MachineName
        Dim user As String = Environment.UserName

        Dim header As String =
        sep & Environment.NewLine &
        $"{ts} [SESSION] Démarrage application | Machine={machine} | User={user}" & Environment.NewLine &
        sep & Environment.NewLine

        File.AppendAllText(logFile, header, Encoding.UTF8)

        _sessionHeaderWritten = True
        _sessionHeaderLogFile = logFile

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : PurgeOldLogs
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Supprime les logs plus vieux que RetentionDays.
    '
    ' Remarques  :
    ' - La purge ne doit pas bloquer l'application (fichier verrouillé / droits).
    ' -------------------------------------------------------------------------------------------------
    Private Sub PurgeOldLogs()

        Dim folder As String = GetLogFolderPath()
        If Not Directory.Exists(folder) Then Exit Sub

        Dim limitDate As DateTime = DateTime.Now.Date.AddDays(-RetentionDays)

        For Each filePath In Directory.GetFiles(folder, "Althea_*.log")
            Try
                Dim lastWrite = File.GetLastWriteTime(filePath)
                If lastWrite.Date < limitDate Then
                    File.Delete(filePath)
                End If
            Catch
                ' Volontaire : la purge ne doit jamais bloquer l'application (fichier verrouillé / droits).
            End Try
        Next

    End Sub

#End Region

#Region "Construction et formatage"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : BuildLogLine
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Construit la ligne log finale (avec détails exception).
    '
    ' Paramètres :
    ' - message (String)       : Le message de log.
    ' - level (LogLevel)       : Le niveau de log (rapide, succinct, complet).
    ' - category (LogCategory) : La catégorie de log (general, startup, database, ui, process, security).
    ' - ex (Exception)         : L'exception associée au log (optionnel).
    '
    ' Retour     :
    ' - String formaté prêt à être écrit dans le fichier de log.
    '
    ' Remarques  :
    ' - Le format de la ligne de log inclut un timestamp, le niveau, la catégorie, le message,
    '   et les détails de l'exception selon le niveau choisi.
    '
    ' Exceptions :
    ' - Aucune exception n'est levée par cette fonction, elle doit être robuste même en cas
    '   d'exception mal formée (ex: message null).
    ' -------------------------------------------------------------------------------------------------
    Private Function BuildLogLine(message As String,
                                  level As LogLevel,
                                  category As LogCategory,
                                  ex As Exception) As String

        Dim ts As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim head As String = $"{ts} [{level}] [{category}] {message}"

        If ex Is Nothing Then
            Return head
        End If

        Dim exMsg As String = MaskSensitive(ex.Message)

        ' Succinct : message exception uniquement.
        If level <> LogLevel.Complet Then
            Return head & " | EX: " & exMsg
        End If

        ' Complet : message + stack + inner
        Dim sb As New StringBuilder()
        sb.Append(head)
        sb.Append(" | EX: ").Append(exMsg)

        If Not String.IsNullOrWhiteSpace(ex.StackTrace) Then
            sb.Append(" | STACK: ").Append(ex.StackTrace.Replace(Environment.NewLine, " ⇢ "))
        End If

        Dim inner As Exception = ex.InnerException
        Dim depth As Integer = 0

        While inner IsNot Nothing AndAlso depth < 3
            sb.Append(" | INNER: ").Append(MaskSensitive(inner.Message))
            If Not String.IsNullOrWhiteSpace(inner.StackTrace) Then
                sb.Append(" | INNER_STACK: ").Append(inner.StackTrace.Replace(Environment.NewLine, " ⇢ "))
            End If
            inner = inner.InnerException
            depth += 1
        End While

        Return sb.ToString()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MaskSensitive
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Masquage simple des secrets dans les messages de log.
    ' - Cible les patterns "Password=xxx" et "Pwd=xxx" (insensible à la casse) et remplace xxx par ***.
    '
    ' Paramètres :
    ' - input : La chaîne de caractères à modifier.
    '
    ' Retour     :
    ' - String avec les secrets masqués.
    '
    ' Remarques  :
    ' - Ce masquage est rudimentaire et ne doit pas être considéré comme une solution de sécurité complète,
    '   mais il permet d'éviter de logguer des secrets évidents dans les messages de log.
    '
    ' Exceptions :
    ' - Retourne la chaîne d'origine si elle est vide.
    ' -------------------------------------------------------------------------------------------------
    Private Function MaskSensitive(input As String) As String
        If String.IsNullOrEmpty(input) Then Return input

        Dim s As String = input

        ' Masque "Password=xxxx" / "Pwd=xxxx" (insensible à la casse)
        s = ReplaceKeyValueInsensitive(s, "Password", "***")
        s = ReplaceKeyValueInsensitive(s, "Pwd", "***")

        Return s
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ReplaceKeyValueInsensitive
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Remplace les valeurs de type Key=Value (terminées par ; ou fin de string).
    ' - Ex: Password=abc; -> Password=***;
    '
    ' Paramètres :
    ' - text        : La chaîne de caractères à modifier.
    ' - key         : La clé à rechercher (insensible à la casse).
    ' - replacement : Le remplacement pour la clé trouvée.
    '
    ' Retour     :
    ' - String avec les valeurs remplacées (insensible à la casse pour la clé).
    '
    ' Remarques  :
    ' - Cette fonction est utilisée pour masquer les valeurs sensibles dans les messages de log.
    ' - Elle recherche les occurrences de "Key=Value" (insensible à la casse pour la clé)
    '   et remplace la valeur par le remplacement spécifié.
    ' - La fonction gère les cas où la paire clé-valeur est suivie d'un point-virgule
    '   ou se trouve à la fin de la chaîne.
    '
    ' Exceptions :
    ' - Retourne la chaîne d'origine si la clé n'est pas trouvée.
    ' -------------------------------------------------------------------------------------------------
    Private Function ReplaceKeyValueInsensitive(text As String, key As String, replacement As String) As String

        Dim idx As Integer = 0
        Dim result As String = text

        While True
            Dim pos As Integer = result.IndexOf(key & "=", idx, StringComparison.OrdinalIgnoreCase)
            If pos < 0 Then Exit While

            Dim startVal As Integer = pos + key.Length + 1
            Dim endVal As Integer = result.IndexOf(";"c, startVal)

            If endVal < 0 Then
                ' Jusqu'à fin de string
                result = result.Substring(0, startVal) & replacement
                Exit While
            Else
                result = result.Substring(0, startVal) & replacement & result.Substring(endVal)
                idx = startVal + replacement.Length
            End If
        End While

        Return result

    End Function

#End Region

#Region "Chemins et fichiers"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDailyLogFilePath
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Retourne le chemin du fichier de log quotidien.
    '
    ' Retour     :
    ' - String
    '
    ' Remarques  :
    ' - Le nom du fichier est au format "Althea_YYYY-MM-DD.log".
    ' -------------------------------------------------------------------------------------------------
    Private Function GetDailyLogFilePath() As String

        Dim folder As String = GetLogFolderPath()
        Dim fileName As String = $"Althea_{DateTime.Now:yyyy-MM-dd}.log"
        Return Path.Combine(folder, fileName)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLogFolderPath
    ' Version    : V1.0.0
    ' Date       : 22/04/2026
    '
    ' Rôle       :
    ' - Source de vérité du chemin racine : ConfigManager.GetAppDataFolderPath()
    ' - Retourne le chemin du dossier de logs (sous le dossier AppData\Althea).
    '
    ' Retour     :
    ' - String
    '
    ' Remarques  :
    ' - Le chemin est au format "%APPDATA%\Althea\Logs".
    ' -------------------------------------------------------------------------------------------------
    Private Function GetLogFolderPath() As String

        Dim root As String = ConfigManager.GetAppDataFolderPath()
        Return Path.Combine(root, "Logs")

    End Function

#End Region

End Module
