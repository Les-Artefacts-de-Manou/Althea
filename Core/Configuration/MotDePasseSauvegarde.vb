' -------------------------------------------------------------------------------------------------
' Module      : MotDePasseSauvegarde
' Projet      : Althéa
' Version     : V1.0.1
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Conserve sur le poste le mot de passe qui chiffre les archives de sauvegarde, chiffré par
' Windows (DPAPI, utilisateur courant), dans %APPDATA%\Althea\althea.sauvegarde.json.
'
' Remarques   :
' - Jamais en base, jamais dans le dépôt, jamais journalisé (décision du 12/10/2026).
' - Fichier distinct de althea.local.json : l'écran de connexion réécrit ce dernier en entier et
'   effacerait le mot de passe des sauvegardes.
' - Chiffré pour le compte Windows courant : un autre compte ou un autre PC ne peut pas le relire.
'   Pour restaurer ailleurs, il faut le ressaisir : d'où la consigne de le noter sur papier.
' - Le contrôle des droits (Admin) et de la longueur est fait par GestionSauvegardes.
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

Public Module MotDePasseSauvegarde

    Private Const NomFichier As String = "althea.sauvegarde.json"

    Private Function CheminFichier() As String
        Return Path.Combine(ConfigManager.GetAppDataFolderPath(), NomFichier)
    End Function

    ' Nothing si le fichier n'existe pas ou ne contient pas de mot de passe.
    Private Function LireContenu() As ContenuMotDePasseSauvegarde
        Dim chemin As String = CheminFichier()
        If Not File.Exists(chemin) Then Return Nothing
        Dim json As String = File.ReadAllText(chemin)
        If String.IsNullOrWhiteSpace(json) Then Return Nothing
        Dim contenu = JsonSerializer.Deserialize(Of ContenuMotDePasseSauvegarde)(json)
        If contenu Is Nothing OrElse String.IsNullOrWhiteSpace(contenu.MotDePasseChiffre) Then Return Nothing
        Return contenu
    End Function

    ''' <summary>Indique si un mot de passe de sauvegarde est enregistré sur ce poste.</summary>
    Public Function EstDefini() As Boolean
        Return LireContenu() IsNot Nothing
    End Function

    ''' <summary>Date (locale) à laquelle le mot de passe a été défini ; Nothing s'il ne l'est pas.</summary>
    Public Function DateDefinition() As DateTime?
        Dim contenu = LireContenu()
        If contenu Is Nothing OrElse Not contenu.DateDefinitionUtc.HasValue Then Return Nothing
        Return contenu.DateDefinitionUtc.Value.ToLocalTime()
    End Function

    ''' <summary>
    ''' Mot de passe en clair, pour chiffrer une archive ; Nothing s'il n'est pas défini.
    ''' Lève une exception si le fichier ne peut pas être déchiffré (autre compte Windows).
    ''' </summary>
    Public Function Obtenir() As String
        Dim contenu = LireContenu()
        If contenu Is Nothing Then Return Nothing
        Return CryptoManagerDPAPI.Unprotect(contenu.MotDePasseChiffre)
    End Function

    ''' <summary>Enregistre (ou remplace) le mot de passe, chiffré pour le compte Windows courant.</summary>
    Public Sub Enregistrer(motDePasse As String)

        If String.IsNullOrEmpty(motDePasse) Then Throw New ArgumentException("Mot de passe vide.", NameOf(motDePasse))

        ConfigManager.EnsureAppDataFolderExists()

        Dim contenu As New ContenuMotDePasseSauvegarde With {
            .MotDePasseChiffre = CryptoManagerDPAPI.Protect(motDePasse),
            .DateDefinitionUtc = DateTime.UtcNow
        }

        ConfigManager.EcrireFichierConfiguration(CheminFichier(),
                          JsonSerializer.Serialize(contenu, New JsonSerializerOptions With {.WriteIndented = True}))

    End Sub

End Module

' Contenu du fichier althea.sauvegarde.json.
Friend NotInheritable Class ContenuMotDePasseSauvegarde
    Public Property MotDePasseChiffre As String
    Public Property DateDefinitionUtc As DateTime?
End Class
