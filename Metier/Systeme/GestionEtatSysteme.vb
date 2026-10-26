' -------------------------------------------------------------------------------------------------
' Module      : GestionEtatSysteme
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Rassemble l'état technique du poste et de l'application pour l'écran État du système
' (Administration) : application, base de données, cohérence des données, dossiers et espace disque,
' sauvegardes.
'
' Responsabilités :
' - Collecter l'état par sections, avec une appréciation par ligne (OK / à vérifier / problème)
' - Produire le rapport texte copié par l'écran
'
' Sécurité :
' - Aucun secret : ni mot de passe, ni clé de licence, ni chaîne de connexion complète ; le compte MariaDB de
'   l'application est nommé (utile au diagnostic), jamais son mot de passe.
'
' Remarques   :
' - CollecterEtat ne lève pas d'exception : chaque vérification qui échoue devient une ligne
'   « problème » avec son motif (appelée en arrière-plan, une exception figerait l'écran sous le
'   débogueur — leçon du 13/10/2026).
' - Lecture seule : rien n'est modifié.
' - V1.2.0 (18/10/2026, UI-9.3) : libellé d'appréciation sans symbole (l'écran le montre en
'   pastille) ; le rapport texte garde ses symboles (SymboleNiveau), il est inchangé.
' - V1.3.0 (20/10/2026) : LireVersionSchemaBase (plus haute migration appliquée), partagée avec le
'   contrôle de version au démarrage (AppStartupManager).
' - V1.4.0 (22/10/2026, relevé de Joëlle) : section « Cohérence des données » — séances dont le dû en
'   attente diffère du reste dû (GestionPaiements.CompterDusDesalignes) ; l'écran propose de les
'   réaligner.
'
' Dépendances :
' - DatabaseManager, ConfigManager, VersionSchema, GestionSauvegardes, OutilsMariaDB,
'   MotDePasseSauvegarde, LicenceSyncfusion, ArborescenceDonnees, GestionParametres / ClesParametres, GestionLog,
'   QueryEtatSysteme, GestionPaiements (cohérence des montants en attente)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports MySqlConnector

Public Module GestionEtatSysteme

    ' En dessous de ce volume libre, un disque est signalé « à vérifier ».
    Private Const SeuilEspaceFaibleOctets As Long = 1024L * 1024 * 1024

#Region "Collecte"

    ''' <summary>État complet, par sections. Ne lève pas d'exception.</summary>
    Public Function CollecterEtat() As List(Of SectionEtat)
        Return New List(Of SectionEtat) From {
            EtatApplication(),
            EtatBase(),
            EtatCoherence(),
            EtatDossiers(),
            EtatSauvegardes()
        }
    End Function

    Private Function EtatApplication() As SectionEtat

        Dim section As New SectionEtat With {.Titre = "Application"}
        section.Ajouter("Version d'Althéa", Securiser(Function() GestionSauvegardes.LireVersionApplication()))
        section.Ajouter("Environnement .NET", RuntimeInformation.FrameworkDescription)
        section.Ajouter("Système", RuntimeInformation.OSDescription)
        section.Ajouter("Poste", Environment.MachineName)
        section.Ajouter("Compte Windows", Environment.UserName)
        section.Ajouter("Dossier du programme", AppContext.BaseDirectory)

        ' Licence lue comme au démarrage : une clé enregistrée par un autre compte Windows ne sert pas.
        Dim licenceLisible As Boolean = Securiser(Function() Not String.IsNullOrEmpty(LicenceSyncfusion.Obtenir()))
        section.Ajouter("Licence Syncfusion (exports PDF et Word)",
                        If(licenceLisible, "Définie sur ce poste", "Absente : les exports portent l'avertissement de licence (Paramètres)"),
                        If(licenceLisible, NiveauEtat.Ok, NiveauEtat.Avertissement))

        Return section

    End Function

    ' Cohérence des données dérivées : le dû en attente de chaque séance doit égaler son reste dû.
    Private Function EtatCoherence() As SectionEtat

        Dim section As New SectionEtat With {.Titre = "Cohérence des données"}

        Try
            Dim nb As Integer = GestionPaiements.CompterDusDesalignes()
            If nb = 0 Then
                section.Ajouter("Montants en attente des séances", "Égaux au reste dû de chaque séance", NiveauEtat.Ok)
            Else
                section.Ajouter("Montants en attente des séances",
                                $"{nb} séance(s) dont le montant en attente diffère du reste dû — bouton « Réaligner les montants en attente »",
                                NiveauEtat.Avertissement)
            End If
        Catch ex As Exception
            Journaliser("contrôle des montants en attente impossible", ex)
            section.Ajouter("Montants en attente des séances", "Contrôle impossible (détail dans les journaux)", NiveauEtat.Probleme)
        End Try

        Return section

    End Function

    Private Function EtatBase() As SectionEtat

        Dim section As New SectionEtat With {.Titre = "Base de données"}

        Dim config As LocalDbConfig = Nothing
        Try
            config = ConfigManager.LoadLocalDbConfig()
        Catch ex As Exception
            Journaliser("configuration de connexion illisible", ex)
        End Try

        If config Is Nothing Then
            section.Ajouter("Configuration de connexion", "Illisible ou absente", NiveauEtat.Probleme)
            Return section
        End If

        section.Ajouter("Serveur", $"{config.Host}:{config.Port}")
        section.Ajouter("Base", config.DatabaseName)
        section.Ajouter("Compte de l'application", config.UserName)

        Try
            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryEtatSysteme.SelectInfosServeur, conn),
                      reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        section.Ajouter("Connexion", "Établie", NiveauEtat.Ok)
                        section.Ajouter("Version de MariaDB", DbHelper.LireString(reader, "version_serveur"))
                    End If
                End Using

                Dim dateMigration As DateTime? = Nothing
                Dim versionBase As String = LireVersionSchemaBase(conn, dateMigration)
                AjouterVersionSchema(section, versionBase, dateMigration)
            End Using
        Catch ex As Exception
            Journaliser("lecture de l'état de la base impossible", ex)
            section.Ajouter("Connexion", "Impossible (détail dans les journaux)", NiveauEtat.Probleme)
        End Try

        Return section

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LireVersionSchemaBase
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Version du schéma de la base : la plus haute migration inscrite dans tec_meta_schema, et sa
    ' date d'application. Nothing si aucune version lisible.
    '
    ' Remarques  :
    ' - Les versions sont comparées comme des numéros (2.10.0 > 2.9.0), pas comme du texte.
    ' - Lève une exception si la base ne répond pas (à l'appelant de décider).
    ' -------------------------------------------------------------------------------------------------
    Public Function LireVersionSchemaBase(conn As MySqlConnection, ByRef dateApplication As DateTime?) As String

        Dim meilleure As Version = Nothing
        Dim texteMeilleure As String = Nothing
        dateApplication = Nothing

        Using cmd As New MySqlCommand(QueryEtatSysteme.SelectMigrationsAppliquees, conn),
              reader = cmd.ExecuteReader()
            While reader.Read()
                Dim texte As String = DbHelper.LireString(reader, "version_schema")
                Dim lue As Version = Nothing
                If Not String.IsNullOrWhiteSpace(texte) AndAlso Version.TryParse(texte.Trim(), lue) AndAlso
                   (meilleure Is Nothing OrElse lue > meilleure) Then
                    meilleure = lue
                    texteMeilleure = texte.Trim()
                    dateApplication = DbHelper.LireDateNullable(reader, "date_application")
                End If
            End While
        End Using

        Return texteMeilleure

    End Function

    ' Même lecture sur une connexion ouverte pour l'occasion (contrôle au démarrage).
    Public Function LireVersionSchemaBase() As String
        Using conn = DatabaseManager.OpenConnection()
            Dim dateApplication As DateTime? = Nothing
            Return LireVersionSchemaBase(conn, dateApplication)
        End Using
    End Function

    ' Version de la base comparée à celle qu'attend l'application (VersionSchema.Attendue).
    Private Sub AjouterVersionSchema(section As SectionEtat, versionBase As String, dateMigration As DateTime?)

        Dim valeur As String = If(String.IsNullOrWhiteSpace(versionBase), "inconnue", versionBase) &
                               If(dateMigration.HasValue, $" (appliquée le {dateMigration.Value:dd/MM/yyyy})", String.Empty)
        Dim ecart As Integer? = VersionSchema.ComparerAAttendue(versionBase)

        If Not ecart.HasValue Then
            section.Ajouter("Version de la base", valeur, NiveauEtat.Probleme)
        ElseIf ecart.Value = 0 Then
            section.Ajouter("Version de la base", valeur, NiveauEtat.Ok)
        ElseIf ecart.Value < 0 Then
            section.Ajouter("Version de la base", valeur & $" — l'application attend la {VersionSchema.Attendue} : migration(s) à appliquer", NiveauEtat.Probleme)
        Else
            section.Ajouter("Version de la base", valeur & $" — plus récente que l'application (qui attend la {VersionSchema.Attendue}) : mettre Althéa à jour", NiveauEtat.Probleme)
        End If
        section.Ajouter("Version attendue par l'application", VersionSchema.Attendue)

    End Sub

    Private Function EtatDossiers() As SectionEtat

        Dim section As New SectionEtat With {.Titre = "Dossiers et espace disque"}

        Dim racine As String = Nothing
        Try
            racine = ArborescenceDonnees.GetRacineDonnees()
        Catch ex As Exception
            Journaliser("racine des données non lue", ex)
        End Try
        AjouterDossier(section, "Dossier de données", racine, "non configuré (Paramètres › PATHS)", NiveauEtat.Probleme)

        Dim dossierSauvegarde As String = Securiser(Function() GestionSauvegardes.GetDossierSauvegarde())
        AjouterDossier(section, "Dossier des sauvegardes", dossierSauvegarde, "pas encore choisi", NiveauEtat.Avertissement)
        AjouterDisqueSauvegardes(section, racine, dossierSauvegarde)

        Dim dossierJournaux As String = GestionLog.GetDossierJournaux()
        AjouterDossier(section, "Dossier des journaux", dossierJournaux, "absent", NiveauEtat.Avertissement)
        Try
            Dim fichiers = LecteurJournaux.ListerFichiers()
            section.Ajouter("Journaux conservés", $"{fichiers.Count} jour(s), {FormaterTaille(fichiers.Sum(Function(f) f.TailleOctets))}")
        Catch ex As Exception
            Journaliser("liste des journaux illisible", ex)
        End Try

        AjouterDossier(section, "Configuration du poste", ConfigManager.GetAppDataFolderPath(), "absente", NiveauEtat.Probleme)

        Return section

    End Function

    ' Un dossier : chemin, existence, espace libre sur son disque.
    Private Sub AjouterDossier(section As SectionEtat, libelle As String, chemin As String,
                               texteSiVide As String, niveauSiAbsent As NiveauEtat)

        If String.IsNullOrWhiteSpace(chemin) Then
            section.Ajouter(libelle, texteSiVide, niveauSiAbsent)
            Return
        End If

        If Not Directory.Exists(chemin) Then
            section.Ajouter(libelle, chemin & " — introuvable (disque débranché ?)", niveauSiAbsent)
            Return
        End If

        Try
            Dim lecteur As New DriveInfo(Path.GetPathRoot(Path.GetFullPath(chemin)))
            Dim libre As Long = lecteur.AvailableFreeSpace
            Dim niveau As NiveauEtat = If(libre < SeuilEspaceFaibleOctets, NiveauEtat.Avertissement, NiveauEtat.Ok)
            section.Ajouter(libelle, $"{chemin} — {FormaterTaille(libre)} libres sur {lecteur.Name}", niveau)
        Catch ex As Exception
            ' Lecteur réseau ou non standard : l'espace libre n'est pas lisible, le dossier existe.
            section.Ajouter(libelle, chemin & " — espace libre non lisible", NiveauEtat.Ok)
        End Try

    End Sub

    ' Sauvegardes et données sur le même disque : une panne de ce disque emporterait les deux.
    Private Sub AjouterDisqueSauvegardes(section As SectionEtat, racine As String, dossierSauvegarde As String)
        If String.IsNullOrWhiteSpace(racine) OrElse String.IsNullOrWhiteSpace(dossierSauvegarde) Then Return
        Try
            Dim disqueDonnees As String = Path.GetPathRoot(Path.GetFullPath(racine))
            Dim disqueSauvegardes As String = Path.GetPathRoot(Path.GetFullPath(dossierSauvegarde))
            If String.Equals(disqueDonnees, disqueSauvegardes, StringComparison.OrdinalIgnoreCase) Then
                section.Ajouter("Sauvegardes sur un autre disque",
                                $"Non : données et sauvegardes sur {disqueDonnees} — une panne de ce disque emporterait les deux (idéal : disque externe)",
                                NiveauEtat.Avertissement)
            Else
                section.Ajouter("Sauvegardes sur un autre disque", $"Oui ({disqueSauvegardes})", NiveauEtat.Ok)
            End If
        Catch ex As Exception
            Journaliser("comparaison des disques impossible", ex)
        End Try
    End Sub

    Private Function EtatSauvegardes() As SectionEtat

        Dim section As New SectionEtat With {.Titre = "Sauvegardes"}

        Try
            Dim derniere As DateTime? = GestionSauvegardes.GetDateDerniereReussie()
            If Not derniere.HasValue Then
                section.Ajouter("Dernière sauvegarde réussie", "Aucune", NiveauEtat.Avertissement)
            Else
                Dim jours As Integer = CInt(Math.Floor((DateTime.Now - derniere.Value).TotalDays))
                Dim delai As Integer = GestionParametres.GetEntier(ClesParametres.SauvegardeRappelJours,
                                                                   ClesParametres.DefautSauvegardeRappelJours)
                section.Ajouter("Dernière sauvegarde réussie",
                                $"{derniere.Value:dd/MM/yyyy HH:mm} (il y a {jours} jour(s))",
                                If(jours > delai, NiveauEtat.Avertissement, NiveauEtat.Ok))
            End If
        Catch ex As Exception
            Journaliser("date de la dernière sauvegarde non lue", ex)
            section.Ajouter("Dernière sauvegarde réussie", "Non lisible (détail dans les journaux)", NiveauEtat.Probleme)
        End Try

        Dim motDePasseDefini As Boolean = Securiser(Function() Not String.IsNullOrEmpty(MotDePasseSauvegarde.Obtenir()))
        section.Ajouter("Mot de passe des sauvegardes", If(motDePasseDefini, "Défini sur ce poste", "Non défini ou illisible sur ce compte Windows"),
                        If(motDePasseDefini, NiveauEtat.Ok, NiveauEtat.Probleme))

        AjouterOutil(section, "Outil de sauvegarde (mariadb-dump)", OutilsMariaDB.NomsOutilDump)
        AjouterOutil(section, "Outil de restauration (mariadb)", OutilsMariaDB.NomsOutilClient)

        Return section

    End Function

    Private Sub AjouterOutil(section As SectionEtat, libelle As String, noms As String())
        Dim chemin As String = Securiser(Function() OutilsMariaDB.TrouverOutil(noms))
        If String.IsNullOrEmpty(chemin) Then
            section.Ajouter(libelle, "Introuvable (Paramètres › SAUVEGARDE_DOSSIER_MARIADB)", NiveauEtat.Probleme)
        Else
            section.Ajouter(libelle, chemin, NiveauEtat.Ok)
        End If
    End Sub

#End Region

#Region "Rapport texte"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : RapportTexte
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Rapport lisible de l'état, pour le copier dans un message (diagnostic à distance).
    ' -------------------------------------------------------------------------------------------------
    Public Function RapportTexte(sections As IEnumerable(Of SectionEtat)) As String

        Dim sb As New StringBuilder()
        sb.AppendLine($"Althéa — État du système — {DateTime.Now:dd/MM/yyyy HH:mm}")

        For Each section In sections
            sb.AppendLine()
            sb.AppendLine($"[{section.Titre}]")
            For Each ligne In section.Lignes
                Dim marque As String = MarqueTexte(ligne.Niveau)
                sb.AppendLine($"  {marque}{ligne.Libelle} : {ligne.Valeur}")
            Next
        Next

        Return sb.ToString()

    End Function

    ''' <summary>Libellé court d'une appréciation (pastille de l'écran, rapport) ; vide pour une information.</summary>
    Public Function LibelleNiveau(niveau As NiveauEtat) As String
        Select Case niveau
            Case NiveauEtat.Ok : Return "OK"
            Case NiveauEtat.Avertissement : Return "À vérifier"
            Case NiveauEtat.Probleme : Return "Problème"
            Case Else : Return String.Empty
        End Select
    End Function

    ' Symbole d'une appréciation dans le rapport texte, où il n'y a pas de couleur pour la distinguer.
    Private Function SymboleNiveau(niveau As NiveauEtat) As String
        Select Case niveau
            Case NiveauEtat.Ok : Return "✓"
            Case NiveauEtat.Avertissement : Return "⚠"
            Case NiveauEtat.Probleme : Return "✗"
            Case Else : Return String.Empty
        End Select
    End Function

    Private Function MarqueTexte(niveau As NiveauEtat) As String
        Dim libelle As String = LibelleNiveau(niveau)
        Return If(libelle.Length = 0, String.Empty, $"[{SymboleNiveau(niveau)} {libelle}] ")
    End Function

#End Region

#Region "Outils"

    ' Valeur calculée sans exception : en cas d'erreur, valeur par défaut et ligne de journal.
    Private Function Securiser(Of T)(calcul As Func(Of T)) As T
        Try
            Return calcul()
        Catch ex As Exception
            Journaliser("valeur non lue", ex)
            Return Nothing
        End Try
    End Function

    Private Sub Journaliser(quoi As String, ex As Exception)
        GestionLog.EcrireLog($"État du système : {quoi}.", GestionLog.LogLevel.Succinct,
                             GestionLog.LogCategory.Process, ex)
    End Sub

    Private Function FormaterTaille(octets As Long) As String
        If octets < 1024 Then Return $"{octets} o"
        If octets < 1024L * 1024 Then Return $"{octets / 1024.0:0.#} Ko"
        If octets < 1024L * 1024 * 1024 Then Return $"{octets / (1024.0 * 1024):0.#} Mo"
        Return $"{octets / (1024.0 * 1024 * 1024):0.##} Go"
    End Function

#End Region

End Module
