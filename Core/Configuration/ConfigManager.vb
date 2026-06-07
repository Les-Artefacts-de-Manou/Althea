' -------------------------------------------------------------------------------------------------
' Module      : ConfigManager
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 21/04/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gère la lecture et l’écriture de la configuration locale de connexion à la base de données.
' La configuration est stockée dans un fichier JSON local situé dans le dossier AppData utilisateur.
'
' Responsabilités :
' - Déterminer les chemins locaux de configuration
' - Créer le dossier de configuration si nécessaire
' - Charger le fichier JSON dans un objet LocalDbConfig
' - Sauvegarder un objet LocalDbConfig dans le fichier JSON
'
' Remarques   :
' - Aucun test de connexion MariaDB ici
' - Aucun chiffrement / déchiffrement ici
' - Aucune logique UI ici

' Imports :
' - Imports System.IO
' - Imports System.Text.Json
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text.Json

Public Module ConfigManager

#Region "Constantes privées"

    ' Nom du dossier de configuration dans AppData
    Private Const AppFolderName As String = "Althea"

    ' Nom du fichier de configuration locale
    Private Const LocalConfigFileName As String = "althea.local.json"

#End Region

#Region "Chemins"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetAppDataFolderPath
    ' Version     : V1.0.0 21/04/2026
    ' Rôle       : Retourne le chemin du dossier local de configuration Althéa dans AppData.
    '
    ' Retour     :
    ' - Le chemin complet du dossier local de configuration
    ' -------------------------------------------------------------------------------------------------
    Public Function GetAppDataFolderPath() As String
        Return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppFolderName)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLocalConfigFilePath
    ' Version     : V1.0.0 21/04/2026
    ' Rôle       : Retourne le chemin complet du fichier JSON de configuration locale.
    '
    ' Retour     :
    ' - Le chemin complet du fichier de configuration locale
    ' -------------------------------------------------------------------------------------------------
    Public Function GetLocalConfigFilePath() As String
        Return Path.Combine(GetAppDataFolderPath(), LocalConfigFileName)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EnsureAppDataFolderExists
    ' Version     : V1.0.0 21/04/2026
    ' Rôle       : Crée le dossier local de configuration s’il n’existe pas encore.
    '
    ' -------------------------------------------------------------------------------------------------
    Public Sub EnsureAppDataFolderExists()

        Dim folderPath As String = GetAppDataFolderPath()

        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If

    End Sub

#End Region

#Region "Chargement / sauvegarde"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LoadLocalDbConfig
    ' Version     : V1.0.0 21/04/2026
    ' Rôle       : Charge la configuration locale depuis le fichier JSON.
    '
    ' Retour     :
    ' - Un objet LocalDbConfig si le fichier existe et est valide
    ' - Nothing si le fichier n’existe pas
    '
    ' Exceptions :
    ' - InvalidOperationException si le contenu JSON est vide ou invalide
    ' -------------------------------------------------------------------------------------------------
    Public Function LoadLocalDbConfig() As LocalDbConfig

        Dim filePath As String = GetLocalConfigFilePath()

        If Not File.Exists(filePath) Then
            Return Nothing
        End If

        Dim jsonContent As String = File.ReadAllText(filePath)

        If String.IsNullOrWhiteSpace(jsonContent) Then
            Throw New InvalidOperationException("Le fichier de configuration locale est vide.")
        End If

        Dim config As LocalDbConfig = JsonSerializer.Deserialize(Of LocalDbConfig)(jsonContent)

        If config Is Nothing Then
            Throw New InvalidOperationException("Impossible de charger la configuration locale.")
        End If

        Return config

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SaveLocalDbConfig
    ' Version     : V1.0.0 21/04/2026
    ' Rôle       : Sauvegarde la configuration locale dans le fichier JSON.
    '
    ' Paramètres :
    ' - config : objet LocalDbConfig à sauvegarder
    '
    ' Retour     :
    ' - Aucun
    '
    ' Exceptions :
    ' - ArgumentNullException si config est Nothing
    ' -------------------------------------------------------------------------------------------------
    Public Sub SaveLocalDbConfig(config As LocalDbConfig)

        If config Is Nothing Then
            Throw New ArgumentNullException(NameOf(config))
        End If

        EnsureAppDataFolderExists()

        Dim filePath As String = GetLocalConfigFilePath()

        Dim jsonOptions As New JsonSerializerOptions With {
            .WriteIndented = True
        }

        Dim jsonContent As String = JsonSerializer.Serialize(config, jsonOptions)

        File.WriteAllText(filePath, jsonContent)

    End Sub

#End Region

End Module