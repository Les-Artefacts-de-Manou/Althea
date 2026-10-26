' -------------------------------------------------------------------------------------------------
' Module      : OutilsMariaDB
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 13/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Services communs à la sauvegarde et à la restauration pour piloter les outils en ligne de
' commande de MariaDB (mariadb-dump, client mariadb).
'
' Responsabilités :
' - Trouver un outil : dossier de recours (SAUVEGARDE_DOSSIER_MARIADB), sinon la version la plus
'   récente installée dans Program Files\MariaDB…\bin
' - Écrire le fichier d'options [client] qui transmet le compte à l'outil, dans un dossier
'   temporaire propre à l'opération, et le supprimer
'
' Sécurité :
' - Le mot de passe n'est jamais passé sur la ligne de commande (visible des autres programmes) :
'   il est écrit dans le fichier d'options, supprimé dès la fin de l'opération. Un échec de
'   suppression est journalisé (catégorie Security).
'
' Dépendances :
' - GestionParametres / ClesParametres, GestionLog
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Friend Module OutilsMariaDB

    ' Noms des outils, du nom actuel à l'ancien nom MySQL.
    Friend ReadOnly NomsOutilDump As String() = {"mariadb-dump.exe", "mysqldump.exe"}
    Friend ReadOnly NomsOutilClient As String() = {"mariadb.exe", "mysql.exe"}

    Private Const NomFichierOptions As String = "client.cnf"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TrouverOutil
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Chemin d'un outil MariaDB : d'abord le dossier de recours (SAUVEGARDE_DOSSIER_MARIADB), sinon
    ' la version la plus récente installée dans Program Files\MariaDB…\bin. Nothing si introuvable.
    ' -------------------------------------------------------------------------------------------------
    Friend Function TrouverOutil(noms As String()) As String

        Dim dossierRecours As String =
            If(GestionParametres.GetValeurParametre(ClesParametres.SauvegardeDossierMariaDB), String.Empty).Trim()

        If dossierRecours.Length > 0 Then
            For Each nom In noms
                Dim candidat As String = Path.Combine(dossierRecours, nom)
                If File.Exists(candidat) Then Return candidat
            Next
        End If

        Dim racinesProgrammes = {Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                                 Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)} _
            .Where(Function(r) Not String.IsNullOrEmpty(r) AndAlso Directory.Exists(r)) _
            .Distinct(StringComparer.OrdinalIgnoreCase)

        Dim candidats As New List(Of String)
        For Each racineProgrammes In racinesProgrammes
            For Each dossierMariaDb In Directory.GetDirectories(racineProgrammes, "MariaDB*")
                For Each nom In noms
                    Dim candidat As String = Path.Combine(dossierMariaDb, "bin", nom)
                    If File.Exists(candidat) Then candidats.Add(candidat) : Exit For
                Next
            Next
        Next

        Return candidats.OrderByDescending(Function(c) VersionDepuisDossier(c)).FirstOrDefault()

    End Function

    ' « …\MariaDB 12.0\bin\mariadb-dump.exe » → 12.0 ; 0.0 si le dossier ne porte pas de version.
    Private Function VersionDepuisDossier(cheminOutil As String) As Version
        Dim dossier As String = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(cheminOutil)))
        Dim m As Match = Regex.Match(If(dossier, String.Empty), "\d+(\.\d+){0,3}")
        Dim versionLue As Version = Nothing
        If m.Success Then
            Dim texte As String = If(m.Value.Contains("."c), m.Value, m.Value & ".0")
            If System.Version.TryParse(texte, versionLue) Then Return versionLue
        End If
        Return New Version(0, 0)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreerFichierOptions
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Crée un dossier temporaire propre à l'opération (%TEMP%\Althea_<guid>) et y écrit le fichier
    ' d'options [client] (hôte, port, compte, mot de passe). Retourne le chemin du fichier ; le
    ' dossier est à supprimer par SupprimerFichierOptions, dans un Finally.
    '
    ' Remarques  :
    ' - Valeurs entre guillemets, \ et " échappés (syntaxe des fichiers d'options MariaDB).
    ' - L'outil doit recevoir --defaults-extra-file=<fichier> en PREMIÈRE option.
    ' -------------------------------------------------------------------------------------------------
    Friend Function CreerFichierOptions(hote As String, port As Integer, compte As String, motDePasse As String) As String

        Dim dossierTemporaire As String = Path.Combine(Path.GetTempPath(), "Althea_" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(dossierTemporaire)
        Dim fichierOptions As String = Path.Combine(dossierTemporaire, NomFichierOptions)

        Dim sb As New StringBuilder()
        sb.AppendLine("[client]")
        sb.AppendLine($"host=""{EchapperOption(hote)}""")
        sb.AppendLine($"port={port}")
        sb.AppendLine($"user=""{EchapperOption(compte)}""")
        sb.AppendLine($"password=""{EchapperOption(motDePasse)}""")

        File.WriteAllText(fichierOptions, sb.ToString(), New UTF8Encoding(False))
        Return fichierOptions

    End Function

    ''' <summary>Supprime le dossier temporaire du fichier d'options ; un échec est journalisé.</summary>
    Friend Sub SupprimerFichierOptions(fichierOptions As String)
        If String.IsNullOrEmpty(fichierOptions) Then Return
        Dim dossierTemporaire As String = Path.GetDirectoryName(fichierOptions)
        Try
            If Directory.Exists(dossierTemporaire) Then Directory.Delete(dossierTemporaire, True)
        Catch ex As Exception
            ' Le fichier contient un mot de passe : l'échec de suppression doit se voir.
            GestionLog.EcrireLog($"MariaDB : dossier temporaire non supprimé ({dossierTemporaire}).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
        End Try
    End Sub

    Private Function EchapperOption(valeur As String) As String
        Return If(valeur, String.Empty).Replace("\", "\\").Replace("""", "\""")
    End Function

End Module
