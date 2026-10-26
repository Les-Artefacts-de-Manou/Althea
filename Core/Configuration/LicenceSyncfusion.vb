' -------------------------------------------------------------------------------------------------
' Module      : LicenceSyncfusion
' Projet      : Althéa
' Version     : V1.0.1
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Conserve sur le poste la clé de licence Syncfusion (exports PDF et Word de l'éditeur de texte),
' chiffrée par Windows (DPAPI, utilisateur courant), dans %APPDATA%\Althea\althea.syncfusion.json.
'
' Remarques   :
' - Jamais en base, jamais dans le dépôt, jamais journalisée (décision du 18/10/2026 : la clé
'   était écrite en clair dans Program.vb, contrairement au guide de licence).
' - Fichier distinct de althea.local.json : l'écran de connexion réécrit ce dernier en entier.
' - Chiffrée pour le compte Windows courant : un autre compte ou un autre PC doit la ressaisir.
' - Même construction que MotDePasseSauvegarde. Le contrôle des droits (Admin) et de la forme de
'   la clé est fait par GestionLicences.
'
' - V1.0.1 (20/10/2026) : écriture sûre (ConfigManager.EcrireFichierConfiguration, audit § 4.5).
'
' Dépendances :
' - ConfigManager (dossier %APPDATA%\Althea), CryptoManagerDPAPI
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text.Json

Public Module LicenceSyncfusion

    Private Const NomFichier As String = "althea.syncfusion.json"

    Private Function CheminFichier() As String
        Return Path.Combine(ConfigManager.GetAppDataFolderPath(), NomFichier)
    End Function

    ' Nothing si le fichier n'existe pas ou ne contient pas de clé.
    Private Function LireContenu() As ContenuLicenceSyncfusion
        Dim chemin As String = CheminFichier()
        If Not File.Exists(chemin) Then Return Nothing
        Dim json As String = File.ReadAllText(chemin)
        If String.IsNullOrWhiteSpace(json) Then Return Nothing
        Dim contenu = JsonSerializer.Deserialize(Of ContenuLicenceSyncfusion)(json)
        If contenu Is Nothing OrElse String.IsNullOrWhiteSpace(contenu.CleChiffree) Then Return Nothing
        Return contenu
    End Function

    ''' <summary>Indique si une clé est enregistrée sur ce poste (sans vérifier qu'elle se déchiffre).</summary>
    Public Function EstDefinie() As Boolean
        Return LireContenu() IsNot Nothing
    End Function

    ''' <summary>Date (locale) à laquelle la clé a été enregistrée ; Nothing si elle ne l'est pas.</summary>
    Public Function DateDefinition() As DateTime?
        Dim contenu = LireContenu()
        If contenu Is Nothing OrElse Not contenu.DateDefinitionUtc.HasValue Then Return Nothing
        Return contenu.DateDefinitionUtc.Value.ToLocalTime()
    End Function

    ''' <summary>
    ''' Clé en clair, à remettre à Syncfusion ; Nothing si elle n'est pas enregistrée.
    ''' Lève une exception si le fichier ne peut pas être déchiffré (autre compte Windows).
    ''' </summary>
    Public Function Obtenir() As String
        Dim contenu = LireContenu()
        If contenu Is Nothing Then Return Nothing
        Return CryptoManagerDPAPI.Unprotect(contenu.CleChiffree)
    End Function

    ''' <summary>Enregistre (ou remplace) la clé, chiffrée pour le compte Windows courant.</summary>
    Public Sub Enregistrer(cle As String)

        If String.IsNullOrEmpty(cle) Then Throw New ArgumentException("Clé vide.", NameOf(cle))

        ConfigManager.EnsureAppDataFolderExists()

        Dim contenu As New ContenuLicenceSyncfusion With {
            .CleChiffree = CryptoManagerDPAPI.Protect(cle),
            .DateDefinitionUtc = DateTime.UtcNow
        }

        ConfigManager.EcrireFichierConfiguration(CheminFichier(),
                          JsonSerializer.Serialize(contenu, New JsonSerializerOptions With {.WriteIndented = True}))

    End Sub

End Module

' Contenu du fichier althea.syncfusion.json.
Friend NotInheritable Class ContenuLicenceSyncfusion
    Public Property CleChiffree As String
    Public Property DateDefinitionUtc As DateTime?
End Class
