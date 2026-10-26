' -------------------------------------------------------------------------------------------------
' Module      : GestionRestaurations
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Restauration complète d'une archive de sauvegarde (lot 30, étape P3) : la base ET le dossier de
' données sont remplacés par ceux de l'archive.
'
' Déroulé (Restaurer) :
'   1. Contrôles sans rien modifier : archive lisible avec le mot de passe, manifeste, même version
'      du schéma, outil mariadb trouvé, compte d'administration accepté et doté des droits utiles,
'      espace disque
'   2. Sauvegarde de sécurité de l'état actuel (sans rotation) ; si elle échoue, on s'arrête
'   3. Extraction des documents dans un dossier provisoire voisin (<racine>_restauration_<horodatage>)
'   4. Dossier de données actuel mis de côté (<racine>_avant_restauration_<horodatage>, conservé)
'   5. Base vidée puis rechargée depuis le dump de l'archive (flux chiffré → client mariadb)
'   6. Dossier provisoire installé à la place de la racine
'   7. Paramètres propres au poste rétablis (racine, dossiers de sauvegarde), journaux (restauration,
'      et sauvegardes : ligne de l'archive restaurée clôturée, sauvegarde de sécurité réinscrite),
'      redémarrage
'
' En cas d'échec à l'étape 5 ou 6 : la base est rechargée depuis la sauvegarde de sécurité et
' l'ancien dossier remis en place ; le message dit précisément ce qui a été fait.
'
' Sécurité :
' - Réservée à l'administrateur (élévation comprise).
' - Recréer les tables exige un compte d'administration MariaDB, saisi pour l'opération et jamais
'   conservé : le compte de l'application garde ses seuls droits sur les données (moindre privilège).
' - Le dump passe de l'archive chiffrée au client mariadb sans être écrit en clair sur le disque ;
'   les mots de passe passent par un fichier d'options temporaire (OutilsMariaDB).
' - Les chemins de l'archive sont contrôlés (aucun fichier écrit hors du dossier provisoire).
' - Les messages d'erreur de MariaDB peuvent citer un extrait du dump (donc des données de
'   patients) : seuls le code d'erreur et le numéro de ligne sont repris dans les messages, le
'   journal et les logs.
'
' Remarques   :
' - Seule une archive de la même version du schéma est acceptée (décision du 13/10/2026) ; une
'   archive plus ancienne se restaure à la main, migrations rejouées ensuite (guide).
' - Fonctions longues et synchrones : l'écran les lance en arrière-plan (Task.Run).
'
' Dépendances :
' - GestionSauvegardes (sauvegarde de sécurité, version du schéma), OutilsMariaDB, DatabaseManager,
'   ConfigManager, ArborescenceDonnees, GestionParametres / ClesParametres, MotDePasseSauvegarde,
'   QueryRestaurations, ICSharpCode.SharpZipLib
' - V1.1.0 (22/10/2026, audit C.1) : ControlerRestauration, VerifierArchive et Restaurer reçoivent
'   un ContexteOperation (Autorisation.Exiger / Possede).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports System.Text.RegularExpressions
Imports ICSharpCode.SharpZipLib.Zip
Imports MySqlConnector

Public Module GestionRestaurations

#Region "Constantes"

    ' Compte proposé par défaut pour l'administration de MariaDB.
    Public Const CompteAdministrationParDefaut As String = "root"

    ' Dossiers voisins de la racine des données (même disque : le renommage est immédiat).
    Private Const SuffixeDossierProvisoire As String = "_restauration_"
    Private Const SuffixeDossierPrecedent As String = "_avant_restauration_"
    Private Const FormatHorodatage As String = "yyyyMMdd_HHmmss"

    ' Droits du compte d'administration sur la base : vider et recharger les tables et séquences.
    Private ReadOnly DroitsStructure As String() = {"SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP", "ALTER", "INDEX"}
    Private ReadOnly DroitsVues As String() = {"CREATE VIEW", "SHOW VIEW"}
    Private ReadOnly DroitsDeclencheurs As String() = {"TRIGGER"}
    Private ReadOnly DroitsRoutines As String() = {"CREATE ROUTINE", "ALTER ROUTINE"}

    ' Paramètres propres au poste : ils décrivent CE poste, pas les données ; une archive venue
    ' d'un autre poste ne doit pas les remplacer.
    Private ReadOnly ClesParametresPoste As String() = {ClesParametres.CheminRacineDonnees,
                                                        ClesParametres.SauvegardeDossier,
                                                        ClesParametres.SauvegardeDossierMariaDB}

    ' Marge d'espace disque exigée en plus du volume des documents à extraire.
    Private Const MargeEspaceDisqueOctets As Long = 200L * 1024 * 1024

    Private Const TailleTamponCopie As Integer = 81920

    ' Extrait sûr d'un message du client mariadb : « ERROR 1064 (42000) at line 523 ».
    Private ReadOnly MotifErreurMariaDB As New Regex("ERROR\s+(\d+)\s*(\(\w+\))?(\s+at\s+line\s+\d+)?", RegexOptions.IgnoreCase)

#End Region

#Region "Archives disponibles"

    ''' <summary>Archives du dossier des sauvegardes, de la plus récente à la plus ancienne (chemins complets).</summary>
    Public Function ListerArchives() As List(Of String)

        Dim dossier As String = GestionSauvegardes.GetDossierSauvegarde()
        If dossier.Length = 0 OrElse Not Directory.Exists(dossier) Then Return New List(Of String)

        Return New DirectoryInfo(dossier).GetFiles(GestionSauvegardes.PrefixeArchive & "*.zip") _
            .Where(Function(f) GestionSauvegardes.MotifArchive.IsMatch(f.Name)) _
            .OrderByDescending(Function(f) f.Name, StringComparer.OrdinalIgnoreCase) _
            .Select(Function(f) f.FullName) _
            .ToList()

    End Function

#End Region

#Region "Lecture et contrôle de l'archive"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LireArchive
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Ouvre l'archive avec le mot de passe, vérifie qu'elle contient le dump et le manifeste, et
    ' retourne le contenu du manifeste.
    '
    ' Exceptions :
    ' - InvalidDataException : fichier introuvable, pas une archive d'Althéa, mot de passe incorrect
    '   ou archive endommagée (message destiné à l'utilisateur)
    ' -------------------------------------------------------------------------------------------------
    Public Function LireArchive(cheminArchive As String, motDePasse As String) As InfoArchive

        If String.IsNullOrWhiteSpace(cheminArchive) OrElse Not File.Exists(cheminArchive) Then
            Throw New InvalidDataException("L'archive est introuvable.")
        End If
        If String.IsNullOrEmpty(motDePasse) Then
            Throw New InvalidDataException("Le mot de passe de l'archive n'est pas renseigné.")
        End If

        Try
            Using zip As New ZipFile(cheminArchive) With {.Password = motDePasse}

                Dim entreeManifeste As ZipEntry = zip.GetEntry(GestionSauvegardes.EntreeManifeste)
                If entreeManifeste Is Nothing OrElse zip.GetEntry(GestionSauvegardes.EntreeDump) Is Nothing Then
                    Throw New InvalidDataException("Ce fichier n'est pas une sauvegarde d'Althéa (manifeste ou base absents).")
                End If

                Using flux As Stream = zip.GetInputStream(entreeManifeste),
                      document As JsonDocument = JsonDocument.Parse(flux)
                    Return LireManifeste(document.RootElement, cheminArchive)
                End Using

            End Using

        Catch ex As InvalidDataException
            Throw
        Catch ex As Exception When TypeOf ex Is ZipException OrElse TypeOf ex Is JsonException OrElse TypeOf ex Is IOException
            GestionLog.EcrireLog($"Restauration : lecture impossible de l'archive {Path.GetFileName(cheminArchive)}.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process, ex)
            Throw New InvalidDataException("L'archive n'a pas pu être lue : mot de passe incorrect ou archive endommagée.", ex)
        End Try

    End Function

    Private Function LireManifeste(racine As JsonElement, cheminArchive As String) As InfoArchive

        Dim format As Integer = LireEntier(racine, "formatArchive")
        If format < 1 OrElse format > GestionSauvegardes.VersionFormatArchive Then
            Throw New InvalidDataException($"Format d'archive non pris en charge ({format}) : archive faite par une version plus récente d'Althéa ?")
        End If

        Dim dateTexte As String = LireTexte(racine, "dateSauvegarde")
        Dim dateSauvegarde As DateTime
        Dim dateLue As DateTime? = Nothing
        If DateTime.TryParse(dateTexte, Globalization.CultureInfo.InvariantCulture,
                             Globalization.DateTimeStyles.RoundtripKind, dateSauvegarde) Then
            dateLue = dateSauvegarde
        End If

        Return New InfoArchive With {
            .CheminArchive = cheminArchive,
            .NomArchive = Path.GetFileName(cheminArchive),
            .DateSauvegarde = dateLue,
            .VersionSchema = LireTexte(racine, "versionSchema"),
            .VersionApplication = LireTexte(racine, "versionApplication"),
            .NomBase = LireTexte(racine, "base"),
            .Machine = LireTexte(racine, "machine"),
            .Utilisateur = LireTexte(racine, "utilisateur"),
            .NbFichiers = LireEntier(racine, "nbFichiersDonnees"),
            .TailleDonneesOctets = LireLong(racine, "tailleDonneesOctets")
        }

    End Function

    Private Function LireTexte(racine As JsonElement, nom As String) As String
        Dim valeur As JsonElement
        If racine.TryGetProperty(nom, valeur) AndAlso valeur.ValueKind = JsonValueKind.String Then Return valeur.GetString()
        Return String.Empty
    End Function

    Private Function LireEntier(racine As JsonElement, nom As String) As Integer
        Dim valeur As JsonElement
        Dim nombre As Integer
        If racine.TryGetProperty(nom, valeur) AndAlso valeur.ValueKind = JsonValueKind.Number AndAlso valeur.TryGetInt32(nombre) Then Return nombre
        Return 0
    End Function

    Private Function LireLong(racine As JsonElement, nom As String) As Long
        Dim valeur As JsonElement
        Dim nombre As Long
        If racine.TryGetProperty(nom, valeur) AndAlso valeur.ValueKind = JsonValueKind.Number AndAlso valeur.TryGetInt64(nombre) Then Return nombre
        Return 0
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ControlerRestauration
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Vérifie, sans rien modifier, qu'une archive (déjà lue) peut être restaurée sur ce poste.
    ' Retourne la liste des empêchements (vide = restauration possible).
    '
    ' Remarques  :
    ' - Le compte d'administration est essayé ici : un compte refusé ou sans droits suffisants est
    '   signalé avant que quoi que ce soit ne soit touché.
    ' -------------------------------------------------------------------------------------------------
    Public Function ControlerRestauration(contexte As ContexteOperation, info As InfoArchive,
                                          compteAdmin As String, motDePasseAdmin As String) As List(Of String)

        Dim refus As New List(Of String)

        If Not Autorisation.Possede(contexte, AppRole.Admin) Then
            refus.Add("Seul l'administrateur peut restaurer une sauvegarde.")
            Return refus
        End If

        ' Version du schéma
        Dim versionCourante As String = GestionSauvegardes.LireVersionSchema()
        If Not String.Equals(info.VersionSchema, versionCourante, StringComparison.OrdinalIgnoreCase) Then
            refus.Add($"L'archive a été faite avec la version {Afficher(info.VersionSchema)} de la base ; la base actuelle est en version {Afficher(versionCourante)}." &
                      Environment.NewLine & "Seule une archive de la même version peut être restaurée depuis Althéa (une archive plus ancienne se restaure à la main : guide des sauvegardes, chapitre 6).")
        End If

        ' Outil
        If OutilsMariaDB.TrouverOutil(OutilsMariaDB.NomsOutilClient) Is Nothing Then
            refus.Add("L'outil mariadb (client en ligne de commande) est introuvable." & Environment.NewLine &
                      "Indiquez le dossier bin de MariaDB dans Paramètres › SAUVEGARDE.")
        End If

        ' Dossier de données et espace disque
        Dim racine As String = Nothing
        Try
            racine = ArborescenceDonnees.GetRacineDonnees()
        Catch ex As InvalidOperationException
            refus.Add("La racine du dossier de données n'est pas configurée (Paramètres › PATHS).")
        End Try

        If racine IsNot Nothing Then
            If Not Directory.Exists(racine) Then
                refus.Add($"Le dossier de données « {racine} » est introuvable.")
            ElseIf GestionSauvegardes.EstDansDossier(info.CheminArchive, racine) Then
                refus.Add("L'archive se trouve dans le dossier de données, qui va être remplacé : copiez-la ailleurs d'abord.")
            Else
                Dim manque As String = VerifierEspaceDisque(racine, info.TailleDonneesOctets)
                If manque IsNot Nothing Then refus.Add(manque)
            End If
        End If

        ' Mot de passe des archives : nécessaire à la sauvegarde de sécurité
        If String.IsNullOrEmpty(MotDePasseSauvegarde.Obtenir()) Then
            refus.Add("Le mot de passe des sauvegardes n'est pas défini sur ce poste : la sauvegarde de sécurité est impossible.")
        End If

        ' Compte d'administration
        If String.IsNullOrWhiteSpace(compteAdmin) OrElse String.IsNullOrEmpty(motDePasseAdmin) Then
            refus.Add("Le compte d'administration MariaDB et son mot de passe sont nécessaires.")
        Else
            Dim refusCompte As String = VerifierCompteAdministration(compteAdmin, motDePasseAdmin)
            If refusCompte IsNot Nothing Then refus.Add(refusCompte)
        End If

        Return refus

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : VerifierArchive
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' « Vérifier l'archive » de l'écran : lecture du manifeste puis contrôles, sans rien modifier.
    '
    ' Remarques  :
    ' - Les cas attendus (mot de passe faux, fichier illisible) sont rendus dans le résultat, pas
    '   par exception : appelée en arrière-plan (Task.Run), une exception qui remonte jusqu'à
    '   l'écran fait s'arrêter le débogueur de Visual Studio, ce qui fige l'application (constaté le
    '   13/10/2026 avec un mot de passe faux).
    ' -------------------------------------------------------------------------------------------------
    Public Function VerifierArchive(contexte As ContexteOperation, cheminArchive As String, motDePasse As String,
                                    compteAdmin As String, motDePasseAdmin As String) As ResultatVerification

        Dim resultat As New ResultatVerification()
        Try
            resultat.Info = LireArchive(cheminArchive, motDePasse)
        Catch ex As InvalidDataException
            resultat.ErreurLecture = ex.Message
            Return resultat
        End Try

        resultat.Refus = ControlerRestauration(contexte, resultat.Info, compteAdmin, motDePasseAdmin)
        Return resultat

    End Function

    Private Function Afficher(version As String) As String
        Return If(String.IsNullOrWhiteSpace(version), "inconnue", version)
    End Function

    Private Function VerifierEspaceDisque(racine As String, tailleDonnees As Long) As String
        Try
            Dim lecteur As New DriveInfo(Path.GetPathRoot(Path.GetFullPath(racine)))
            Dim besoin As Long = tailleDonnees + MargeEspaceDisqueOctets
            If lecteur.AvailableFreeSpace < besoin Then
                Return $"Espace disque insuffisant sur {lecteur.Name} pour extraire les documents ({besoin \ (1024 * 1024)} Mo nécessaires)."
            End If
        Catch ex As Exception
            ' Lecteur réseau ou non standard : pas de contrôle préalable, l'extraction échouera proprement.
            GestionLog.EcrireLog("Restauration : espace disque non vérifiable.", GestionLog.LogLevel.Complet,
                                 GestionLog.LogCategory.Process, ex)
        End Try
        Return Nothing
    End Function

    ' Connexion d'essai avec le compte d'administration et contrôle de ses droits sur la base.
    Private Function VerifierCompteAdministration(compte As String, motDePasse As String) As String

        Try
            Using conn = DatabaseManager.OpenConnectionAvecCompte(compte, motDePasse)

                Dim lignesDroits As New List(Of String)
                Using cmd As New MySqlCommand(QueryRestaurations.ShowGrants, conn),
                      reader = cmd.ExecuteReader()
                    While reader.Read()
                        lignesDroits.Add(reader.GetString(0))
                    End While
                End Using

                Dim attendus As New List(Of String)(DroitsStructure)
                Using cmd As New MySqlCommand(QueryRestaurations.SelectNbObjetsProgrammes, conn),
                      reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        If Convert.ToInt64(reader("nb_vues")) > 0 Then attendus.AddRange(DroitsVues)
                        If Convert.ToInt64(reader("nb_declencheurs")) > 0 Then attendus.AddRange(DroitsDeclencheurs)
                        If Convert.ToInt64(reader("nb_routines")) > 0 Then attendus.AddRange(DroitsRoutines)
                    End If
                End Using

                Dim manquants As List(Of String) = DroitsManquants(lignesDroits, conn.Database, attendus)
                If manquants.Count > 0 Then
                    Return $"Le compte MariaDB « {compte} » n'a pas tous les droits nécessaires sur la base « {conn.Database} » : " &
                           String.Join(", ", manquants) & "."
                End If

            End Using

        Catch ex As MySqlException
            GestionLog.EcrireLog($"Restauration : compte d'administration MariaDB « {compte} » refusé.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
            Return $"Le compte d'administration MariaDB « {compte} » est refusé (compte ou mot de passe incorrect, ou accès non autorisé depuis ce poste)."
        End Try

        Return Nothing

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DroitsManquants
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Compare les droits accordés (lignes de SHOW GRANTS) aux droits attendus sur la base.
    '
    ' Remarques  :
    ' - Prend en compte les droits globaux (ON *.*) et ceux de la base, y compris par motif
    '   (`althea%`, `\_` = trait de soulignement littéral). ALL PRIVILEGES couvre tout.
    ' - Les droits sur une table isolée ou accordés par un rôle ne sont pas pris en compte : dans ce
    '   cas le contrôle peut refuser à tort, jamais accepter à tort.
    ' -------------------------------------------------------------------------------------------------
    Friend Function DroitsManquants(lignesDroits As IEnumerable(Of String), nomBase As String,
                                    attendus As IEnumerable(Of String)) As List(Of String)

        Dim accordes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim motif As New Regex("^GRANT\s+(.+?)\s+ON\s+(\S+)\s+TO\s", RegexOptions.IgnoreCase)

        For Each ligne In lignesDroits
            Dim m As Match = motif.Match(ligne)
            If Not m.Success Then Continue For

            Dim cible As String = m.Groups(2).Value.Replace("`", String.Empty)
            Dim pointe As Integer = cible.LastIndexOf("."c)
            If pointe < 0 OrElse cible.Substring(pointe + 1) <> "*" Then Continue For   ' droit sur une table isolée

            Dim baseCible As String = cible.Substring(0, pointe)
            If baseCible <> "*" AndAlso Not BaseCorrespond(baseCible, nomBase) Then Continue For

            For Each droit In m.Groups(1).Value.Split(","c)
                Dim nom As String = droit.Trim()
                If nom.StartsWith("ALL", StringComparison.OrdinalIgnoreCase) Then Return New List(Of String)
                accordes.Add(nom)
            Next
        Next

        Return attendus.Where(Function(d) Not accordes.Contains(d)).Distinct(StringComparer.OrdinalIgnoreCase).ToList()

    End Function

    ' Motif de base des droits MariaDB : % = n'importe quelle suite, _ = un caractère, \_ = _ littéral.
    Private Function BaseCorrespond(motifBase As String, nomBase As String) As Boolean
        Dim sb As New StringBuilder("^")
        Dim i As Integer = 0
        While i < motifBase.Length
            Dim c As Char = motifBase(i)
            If c = "\"c AndAlso i + 1 < motifBase.Length Then
                sb.Append(Regex.Escape(motifBase(i + 1).ToString()))
                i += 2
                Continue While
            End If
            Select Case c
                Case "%"c : sb.Append(".*")
                Case "_"c : sb.Append(".")
                Case Else : sb.Append(Regex.Escape(c.ToString()))
            End Select
            i += 1
        End While
        sb.Append("$")
        Return Regex.IsMatch(nomBase, sb.ToString(), RegexOptions.IgnoreCase)
    End Function

#End Region

#Region "Restauration"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Restaurer
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Restaure une archive : sauvegarde de sécurité, documents, base, paramètres du poste, journal.
    '
    ' Paramètres :
    ' - contexte           : qui restaure (Admin requis, élévation comprise ; compte au journal)
    ' - cheminArchive      : archive à restaurer
    ' - motDePasseArchive  : mot de passe de l'archive
    ' - compteAdmin / motDePasseAdmin : compte d'administration MariaDB (non conservé)
    ' - progression        : avancement pour l'écran (peut être Nothing)
    '
    ' Retour     :
    ' - ResultatRestauration ; RedemarrageNecessaire = True dès que la base a été rechargée (archive
    '   ou sauvegarde de sécurité) : la session en cours ne correspond plus à la base.
    '
    ' Remarques  :
    ' - Un empêchement relevé par ControlerRestauration n'est pas une tentative : pas de journal.
    ' - Une étape 6 ou 7 qui échoue après le rechargement de l'archive n'annule pas la restauration :
    '   le message dit ce qui reste à faire.
    ' -------------------------------------------------------------------------------------------------
    Public Function Restaurer(contexte As ContexteOperation,
                              cheminArchive As String,
                              motDePasseArchive As String,
                              compteAdmin As String,
                              motDePasseAdmin As String,
                              progression As IProgress(Of ProgressionSauvegarde)) As ResultatRestauration

        Autorisation.Exiger(contexte, AppRole.Admin, "Seul l'administrateur peut restaurer une sauvegarde.")
        Dim utilisateur As UtilisateurApplication = contexte.Utilisateur

        Dim dateDebut As DateTime = DateTime.Now

        ' --- 1. Contrôles
        Signaler(progression, "Vérification de l'archive…", 0)
        Dim info As InfoArchive
        Try
            info = LireArchive(cheminArchive, motDePasseArchive)
        Catch ex As InvalidDataException
            Return Echec(ex.Message, Nothing, False)
        End Try

        Dim refus As List(Of String) = ControlerRestauration(contexte, info, compteAdmin, motDePasseAdmin)
        If refus.Count > 0 Then Return Echec(String.Join(Environment.NewLine & Environment.NewLine, refus), Nothing, False)

        Dim config As LocalDbConfig = ConfigManager.LoadLocalDbConfig()
        Dim outilClient As String = OutilsMariaDB.TrouverOutil(OutilsMariaDB.NomsOutilClient)
        Dim racine As String = Path.TrimEndingDirectorySeparator(Path.GetFullPath(ArborescenceDonnees.GetRacineDonnees()))
        Dim horodatage As String = DateTime.Now.ToString(FormatHorodatage)
        Dim dossierProvisoire As String = racine & SuffixeDossierProvisoire & horodatage
        Dim dossierPrecedent As String = racine & SuffixeDossierPrecedent & horodatage
        Dim parametresPoste As Dictionary(Of String, String) = LireParametresPoste()

        Dim archiveSecurite As String = Nothing
        Dim resultatSecurite As ResultatSauvegarde = Nothing
        Dim dossierMisDeCote As Boolean = False
        Dim baseTouchee As Boolean = False
        Dim nbFichiers As Integer = 0

        GestionLog.EcrireLog($"Restauration lancée par {If(utilisateur?.LoginUtilisateur, "?")} : {info.NomArchive}.",
                             GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)

        Try
            ' --- 2. Sauvegarde de sécurité (sans rotation : elle pourrait supprimer l'archive à restaurer)
            resultatSecurite =
                GestionSauvegardes.Sauvegarder(contexte,
                                               New ProgressionRelais(progression, 2, 35, "Sauvegarde de sécurité : "),
                                               avecRotation:=False)
            If Not resultatSecurite.Reussie Then
                Throw New InvalidOperationException("la sauvegarde de sécurité de l'état actuel a échoué : " & resultatSecurite.Message)
            End If
            archiveSecurite = resultatSecurite.CheminArchive

            Using zip As New ZipFile(cheminArchive) With {.Password = motDePasseArchive}

                ' --- 3. Documents dans le dossier provisoire
                nbFichiers = ExtraireDonnees(zip, dossierProvisoire, info.NbFichiers,
                                             New ProgressionRelais(progression, 35, 60, String.Empty))

                ' --- 4. Dossier actuel mis de côté
                Signaler(progression, "Mise de côté du dossier de données actuel…", 61)
                Try
                    Directory.Move(racine, dossierPrecedent)
                    dossierMisDeCote = True
                Catch ex As IOException
                    Throw New InvalidOperationException("le dossier de données actuel n'a pas pu être mis de côté (un document ou une fenêtre de l'explorateur y est peut-être ouvert, ou la synchronisation OneDrive le bloque).", ex)
                End Try

                ' --- 5. Base
                baseTouchee = True
                Signaler(progression, "Remplacement de la base…", 65)
                ViderBase(compteAdmin, motDePasseAdmin)
                Signaler(progression, "Rechargement de la base depuis l'archive…", 70)
                RechargerDump(zip, outilClient, config, compteAdmin, motDePasseAdmin)

            End Using

            ' --- 6. Documents restaurés à leur place
            Signaler(progression, "Mise en place des documents restaurés…", 92)
            Directory.Move(dossierProvisoire, racine)

        Catch ex As Exception
            Return GererEchec(ex, info, dateDebut, utilisateur, archiveSecurite, compteAdmin, motDePasseAdmin,
                              config, outilClient, racine, dossierProvisoire, dossierPrecedent, dossierMisDeCote, baseTouchee)
        End Try

        ' --- 7. Paramètres du poste et journal (la restauration elle-même est faite)
        Signaler(progression, "Finalisation…", 96)
        Dim avertissement As String = RetablirParametresPoste(parametresPoste)
        ReporterJournalSauvegardes(info, resultatSecurite, dateDebut, utilisateur)
        Journaliser(StatutRestauration.Reussie, info, dateDebut, nbFichiers, archiveSecurite, dossierPrecedent, utilisateur, Nothing)

        GestionLog.EcrireLog($"Restauration réussie : {info.NomArchive} ({nbFichiers} fichiers) ; sauvegarde de sécurité {Path.GetFileName(archiveSecurite)} ; ancien dossier {dossierPrecedent}.",
                             GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)
        Signaler(progression, "Restauration terminée.", 100)

        Return New ResultatRestauration With {
            .Reussie = True,
            .ArchiveSecurite = archiveSecurite,
            .DossierDonneesPrecedent = dossierPrecedent,
            .RedemarrageNecessaire = True,
            .Message = "Restauration terminée." & If(avertissement Is Nothing, String.Empty, Environment.NewLine & Environment.NewLine & avertissement)
        }

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GererEchec
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Remet le poste dans son état d'avant après un échec et construit le message exact :
    ' - base pas encore touchée : rien n'a changé (dossier remis en place s'il avait été déplacé) ;
    ' - base touchée : rechargement depuis la sauvegarde de sécurité et ancien dossier remis ;
    ' - si cette remise en état échoue aussi : message d'alerte avec la marche à suivre.
    ' -------------------------------------------------------------------------------------------------
    Private Function GererEchec(ex As Exception, info As InfoArchive, dateDebut As DateTime,
                                utilisateur As UtilisateurApplication, archiveSecurite As String,
                                compteAdmin As String, motDePasseAdmin As String,
                                config As LocalDbConfig, outilClient As String, racine As String,
                                dossierProvisoire As String, dossierPrecedent As String,
                                dossierMisDeCote As Boolean, baseTouchee As Boolean) As ResultatRestauration

        GestionLog.EcrireLog($"Restauration en échec : {info.NomArchive}.", GestionLog.LogLevel.Succinct,
                             GestionLog.LogCategory.Security, ex)

        Dim cause As String = CauseLisible(ex)
        Dim baseRetablie As Boolean = Not baseTouchee

        ' Base : rechargement depuis la sauvegarde de sécurité (même mot de passe que les archives du poste).
        If baseTouchee Then
            Try
                Using zipSecurite As New ZipFile(archiveSecurite) With {.Password = MotDePasseSauvegarde.Obtenir()}
                    ViderBase(compteAdmin, motDePasseAdmin)
                    RechargerDump(zipSecurite, outilClient, config, compteAdmin, motDePasseAdmin)
                End Using
                baseRetablie = True
                GestionLog.EcrireLog($"Restauration : base remise dans son état d'avant ({Path.GetFileName(archiveSecurite)}).",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)
            Catch exRetour As Exception
                GestionLog.EcrireLog("Restauration : ÉCHEC de la remise en état de la base depuis la sauvegarde de sécurité.",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, exRetour)
            End Try
        End If

        ' Documents : l'ancien dossier reprend sa place, le dossier provisoire est supprimé.
        Dim dossierRetabli As Boolean = True
        Try
            If dossierMisDeCote AndAlso Not Directory.Exists(racine) Then Directory.Move(dossierPrecedent, racine)
        Catch exDossier As Exception
            dossierRetabli = False
            GestionLog.EcrireLog($"Restauration : ancien dossier de données non remis en place ({dossierPrecedent}).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process, exDossier)
        End Try
        SupprimerDossierSansErreur(dossierProvisoire)

        Dim message As New StringBuilder()
        If Not baseTouchee Then
            message.AppendLine("La restauration n'a pas eu lieu : " & cause)
            message.AppendLine()
            message.Append("Les données actuelles n'ont pas été modifiées.")
        ElseIf baseRetablie Then
            message.AppendLine("La restauration a échoué : " & cause)
            message.AppendLine()
            message.Append("La base a été remise dans son état d'avant, depuis la sauvegarde de sécurité " &
                           $"« {Path.GetFileName(archiveSecurite)} ». Althéa va redémarrer.")
        Else
            message.AppendLine("ATTENTION : la restauration a échoué, et la remise en état automatique de la base aussi.")
            message.AppendLine("Cause : " & cause)
            message.AppendLine()
            message.AppendLine("La base est incomplète : ne travaillez pas dans Althéa.")
            message.Append($"Restaurez à la main la sauvegarde de sécurité « {archiveSecurite} » (Docs/Guide/Guide_Utilisateur_Sauvegardes.md, chapitre 6).")
        End If
        If Not dossierRetabli Then
            message.AppendLine()
            message.AppendLine()
            message.Append($"L'ancien dossier de données n'a pas pu reprendre sa place : il se trouve dans « {dossierPrecedent} ». Renommez-le en « {racine} ».")
        End If

        ' Journal : dans la base actuelle (intacte ou remise en état) ; impossible si elle est incomplète.
        If baseRetablie Then
            Journaliser(StatutRestauration.Echec, info, dateDebut, Nothing, archiveSecurite, Nothing, utilisateur,
                        GestionSauvegardes.Tronquer(cause))
        End If

        Return Echec(message.ToString(), archiveSecurite, baseTouchee)

    End Function

    ' Texte de la cause : nos messages (InvalidOperationException, InvalidDataException) et ceux du
    ' système de fichiers sont lisibles ; le reste est renvoyé aux journaux.
    Private Function CauseLisible(ex As Exception) As String
        If TypeOf ex Is InvalidOperationException OrElse TypeOf ex Is InvalidDataException OrElse
           TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException Then
            Return ex.Message
        End If
        Return "erreur inattendue (détail dans les journaux)."
    End Function

    Private Function Echec(message As String, archiveSecurite As String, redemarrage As Boolean) As ResultatRestauration
        Return New ResultatRestauration With {
            .Reussie = False,
            .Message = message,
            .ArchiveSecurite = archiveSecurite,
            .RedemarrageNecessaire = redemarrage
        }
    End Function

    Private Sub Signaler(progression As IProgress(Of ProgressionSauvegarde), etape As String, pourcentage As Integer)
        progression?.Report(New ProgressionSauvegarde With {.Etape = etape, .Pourcentage = Math.Max(0, Math.Min(100, pourcentage))})
    End Sub

    ' Ramène l'avancement d'une sous-étape (0–100) dans une plage de l'avancement global.
    Private NotInheritable Class ProgressionRelais
        Implements IProgress(Of ProgressionSauvegarde)

        Private ReadOnly _cible As IProgress(Of ProgressionSauvegarde)
        Private ReadOnly _debut As Integer
        Private ReadOnly _fin As Integer
        Private ReadOnly _prefixe As String

        Public Sub New(cible As IProgress(Of ProgressionSauvegarde), debut As Integer, fin As Integer, prefixe As String)
            _cible = cible
            _debut = debut
            _fin = fin
            _prefixe = prefixe
        End Sub

        Public Sub Report(value As ProgressionSauvegarde) Implements IProgress(Of ProgressionSauvegarde).Report
            If _cible Is Nothing OrElse value Is Nothing Then Return
            _cible.Report(New ProgressionSauvegarde With {
                .Etape = _prefixe & value.Etape,
                .Pourcentage = _debut + (_fin - _debut) * Math.Max(0, Math.Min(100, value.Pourcentage)) \ 100
            })
        End Sub
    End Class

#End Region

#Region "Étapes techniques"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ExtraireDonnees
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Extrait les entrées donnees/… de l'archive dans le dossier provisoire ; retourne le nombre de
    ' fichiers extraits.
    '
    ' Remarques  :
    ' - Chaque chemin est contrôlé : une entrée qui sortirait du dossier provisoire (« ..\ »,
    '   chemin absolu) fait échouer l'extraction.
    ' - Taille de chaque fichier et nombre de fichiers comparés à l'archive et au manifeste.
    ' -------------------------------------------------------------------------------------------------
    Private Function ExtraireDonnees(zip As ZipFile, dossierProvisoire As String, nbAttendus As Integer,
                                     progression As IProgress(Of ProgressionSauvegarde)) As Integer

        If Directory.Exists(dossierProvisoire) Then
            Throw New InvalidOperationException($"le dossier provisoire « {dossierProvisoire} » existe déjà.")
        End If
        Directory.CreateDirectory(dossierProvisoire)

        Dim baseComplete As String = Path.TrimEndingDirectorySeparator(Path.GetFullPath(dossierProvisoire)) & Path.DirectorySeparatorChar
        Dim entrees As List(Of ZipEntry) = zip.Cast(Of ZipEntry)() _
            .Where(Function(e) e.Name.StartsWith(GestionSauvegardes.PrefixeEntreesDonnees, StringComparison.Ordinal) AndAlso
                               e.Name.Length > GestionSauvegardes.PrefixeEntreesDonnees.Length) _
            .ToList()

        Dim nbFichiers As Integer = 0
        Dim tampon(TailleTamponCopie - 1) As Byte

        For i As Integer = 0 To entrees.Count - 1

            Dim entree As ZipEntry = entrees(i)
            Dim relatif As String = entree.Name.Substring(GestionSauvegardes.PrefixeEntreesDonnees.Length).Replace("/"c, Path.DirectorySeparatorChar)
            Dim cible As String = Path.GetFullPath(Path.Combine(dossierProvisoire, relatif))

            If Not cible.StartsWith(baseComplete, StringComparison.OrdinalIgnoreCase) Then
                Throw New InvalidOperationException($"l'archive contient un chemin de fichier invalide ({entree.Name}).")
            End If

            If entree.IsDirectory Then
                Directory.CreateDirectory(cible)
                Continue For
            End If

            progression?.Report(New ProgressionSauvegarde With {
                .Etape = $"Extraction des documents ({nbFichiers + 1} / {nbAttendus})…",
                .Pourcentage = CInt(100L * i \ Math.Max(entrees.Count, 1))
            })

            Directory.CreateDirectory(Path.GetDirectoryName(cible))
            Dim octets As Long = 0
            Using source As Stream = zip.GetInputStream(entree),
                  destination As New FileStream(cible, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                Dim lus As Integer = source.Read(tampon, 0, tampon.Length)
                While lus > 0
                    destination.Write(tampon, 0, lus)
                    octets += lus
                    lus = source.Read(tampon, 0, tampon.Length)
                End While
            End Using

            If entree.Size >= 0 AndAlso octets <> entree.Size Then
                Throw New InvalidOperationException($"le fichier « {relatif} » de l'archive est incomplet.")
            End If
            File.SetLastWriteTime(cible, entree.DateTime)
            nbFichiers += 1

        Next

        If nbFichiers <> nbAttendus Then
            Throw New InvalidOperationException($"l'archive contient {nbFichiers} fichier(s) de données au lieu des {nbAttendus} annoncés par son manifeste.")
        End If

        Return nbFichiers

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ViderBase
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Supprime tous les objets de la base configurée (tables, vues, séquences, routines), avec le
    ' compte d'administration, pour la recharger ensuite dans une base vide.
    '
    ' Remarques  :
    ' - Les requêtes d'inventaire portent sur DATABASE() : celle de la connexion, jamais une autre.
    ' - Contrôle des clés étrangères désactivé pour la seule session de cette connexion.
    ' - Recharger dans une base vide est le cas validé à la main (test 4.7 du plan de sauvegarde).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ViderBase(compteAdmin As String, motDePasseAdmin As String)

        Using conn = DatabaseManager.OpenConnectionAvecCompte(compteAdmin, motDePasseAdmin)

            Dim ordres As New List(Of String)

            Using cmd As New MySqlCommand(QueryRestaurations.SelectObjetsBase, conn),
                  reader = cmd.ExecuteReader()
                While reader.Read()
                    Dim nature As String
                    Select Case reader.GetString("table_type").ToUpperInvariant()
                        Case "VIEW" : nature = "VIEW"
                        Case "SEQUENCE" : nature = "SEQUENCE"
                        Case Else : nature = "TABLE"
                    End Select
                    ordres.Add($"DROP {nature} IF EXISTS {Identifiant(reader.GetString("table_name"))};")
                End While
            End Using

            Using cmd As New MySqlCommand(QueryRestaurations.SelectRoutinesBase, conn),
                  reader = cmd.ExecuteReader()
                While reader.Read()
                    Dim nature As String = If(reader.GetString("routine_type").ToUpperInvariant() = "FUNCTION", "FUNCTION", "PROCEDURE")
                    ordres.Add($"DROP {nature} IF EXISTS {Identifiant(reader.GetString("routine_name"))};")
                End While
            End Using

            Using cmd As New MySqlCommand(QueryRestaurations.DesactiverClesEtrangeres, conn)
                cmd.ExecuteNonQuery()
            End Using

            For Each ordre In ordres
                Using cmd As New MySqlCommand(ordre, conn)
                    cmd.ExecuteNonQuery()
                End Using
            Next

        End Using

    End Sub

    ' Nom d'objet issu d'information_schema, entre accents graves (doublés s'il en contient).
    Private Function Identifiant(nom As String) As String
        Return "`" & nom.Replace("`", "``") & "`"
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : RechargerDump
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Envoie le dump de l'archive (base/althea.sql) au client mariadb, dans la base configurée, avec
    ' le compte d'administration.
    '
    ' Remarques  :
    ' - Le dump est lu dans l'entrée chiffrée et transmis directement à l'entrée standard du client :
    '   il n'est jamais écrit en clair sur le disque.
    ' - Le client s'arrête à la première erreur (mode non interactif) et sort avec un code ≠ 0.
    ' - Sortie d'erreur lue en parallèle (sinon blocage si elle se remplit) ; la sortie standard
    '   n'est pas redirigée (le client n'écrit rien en rechargeant un dump).
    ' - Seuls le code d'erreur et la ligne sont repris (MessageSansDonnees) : le texte complet peut
    '   citer un extrait du dump.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RechargerDump(zip As ZipFile, outilClient As String, config As LocalDbConfig,
                              compteAdmin As String, motDePasseAdmin As String)

        Dim entreeDump As ZipEntry = zip.GetEntry(GestionSauvegardes.EntreeDump)
        If entreeDump Is Nothing Then Throw New InvalidOperationException("l'archive ne contient pas la base.")

        Dim fichierOptions As String = Nothing

        Try
            fichierOptions = OutilsMariaDB.CreerFichierOptions(config.Host, config.Port, compteAdmin, motDePasseAdmin)

            Dim infos As New ProcessStartInfo(outilClient) With {
                .UseShellExecute = False,
                .RedirectStandardInput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
            infos.ArgumentList.Add("--defaults-extra-file=" & fichierOptions)
            infos.ArgumentList.Add("--default-character-set=utf8mb4")
            infos.ArgumentList.Add(config.DatabaseName)

            Dim erreurs As New StringBuilder()

            Using processus As New Process With {.StartInfo = infos}

                AddHandler processus.ErrorDataReceived,
                    Sub(sender, e)
                        If e.Data Is Nothing Then Return
                        SyncLock erreurs
                            erreurs.AppendLine(e.Data)
                        End SyncLock
                    End Sub

                processus.Start()
                processus.BeginErrorReadLine()

                Try
                    Using source As Stream = zip.GetInputStream(entreeDump)
                        source.CopyTo(processus.StandardInput.BaseStream)
                    End Using
                    processus.StandardInput.Close()
                Catch ex As IOException
                    ' Le client s'est arrêté sur une erreur pendant l'envoi : son code de sortie le dira.
                End Try

                processus.WaitForExit()

                If processus.ExitCode <> 0 Then
                    Dim detail As String
                    SyncLock erreurs
                        detail = erreurs.ToString()
                    End SyncLock
                    Throw New InvalidOperationException(
                        $"le rechargement de la base a échoué (mariadb, code {processus.ExitCode} : {MessageSansDonnees(detail)}).")
                End If

            End Using

        Finally
            OutilsMariaDB.SupprimerFichierOptions(fichierOptions)
        End Try

    End Sub

    ' « ERROR 1064 (42000) at line 523: … near 'Dupont'… » → « erreur 1064 (42000) at line 523 ».
    Friend Function MessageSansDonnees(sortieErreur As String) As String
        Dim m As Match = MotifErreurMariaDB.Match(If(sortieErreur, String.Empty))
        If Not m.Success Then Return "détail non disponible"
        Return ("erreur " & m.Groups(1).Value & " " & m.Groups(2).Value & m.Groups(3).Value).Trim()
    End Function

    Private Function LireParametresPoste() As Dictionary(Of String, String)
        Dim valeurs As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each cle In ClesParametresPoste
            valeurs(cle) = GestionParametres.GetValeurParametre(cle)
        Next
        Return valeurs
    End Function

    ' Réécrit dans la base restaurée les paramètres propres à ce poste. Retourne un avertissement si
    ' l'un d'eux n'a pas pu l'être, sinon Nothing.
    Private Function RetablirParametresPoste(valeurs As Dictionary(Of String, String)) As String
        Dim nonRetablis As New List(Of String)
        For Each paire In valeurs
            Try
                GestionParametres.UpdateValeurParametreParCle(paire.Key, paire.Value, ModeAccesParametres.Admin)
            Catch ex As Exception
                nonRetablis.Add(paire.Key)
                GestionLog.EcrireLog($"Restauration : paramètre du poste {paire.Key} non rétabli.",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
            End Try
        Next
        If nonRetablis.Count = 0 Then Return Nothing
        Return "À vérifier dans Paramètres : " & String.Join(", ", nonRetablis) &
               " (valeur de l'archive conservée au lieu de celle de ce poste)."
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ReporterJournalSauvegardes
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Remet d'aplomb le journal des sauvegardes de la base restaurée :
    ' - la ligne de l'archive restaurée y figure « en cours » (elle a été écrite avant l'export de la
    '   base, et clôturée après) : elle est marquée réussie avec la taille et le nombre de fichiers ;
    ' - la sauvegarde de sécurité, journalisée dans la base remplacée, y est réinscrite.
    '
    ' Remarques  :
    ' - Ne lève pas d'exception : le journal ne doit pas faire échouer une restauration réussie.
    ' - Pas d'identifiant de compte (ceux de la base restaurée peuvent différer) : login seulement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ReporterJournalSauvegardes(info As InfoArchive, resultatSecurite As ResultatSauvegarde,
                                           dateDebut As DateTime, utilisateur As UtilisateurApplication)
        Try
            Using conn = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryRestaurations.UpdateLigneArchiveRestauree, conn)
                    cmd.Parameters.AddWithValue("@reussie", StatutsSauvegarde.CodeReussie)
                    cmd.Parameters.AddWithValue("@en_cours", StatutsSauvegarde.CodeEnCours)
                    cmd.Parameters.AddWithValue("@nom", info.NomArchive)
                    cmd.Parameters.AddWithValue("@taille", New FileInfo(info.CheminArchive).Length)
                    cmd.Parameters.AddWithValue("@nb_fichiers", info.NbFichiers)
                    cmd.ExecuteNonQuery()
                End Using

                If resultatSecurite IsNot Nothing AndAlso resultatSecurite.Reussie Then
                    Using cmd As New MySqlCommand(QueryRestaurations.InsertLigneSauvegardeSecurite, conn)
                        cmd.Parameters.AddWithValue("@date_debut", dateDebut)
                        cmd.Parameters.AddWithValue("@statut", StatutsSauvegarde.CodeReussie)
                        cmd.Parameters.AddWithValue("@nom_fichier", Path.GetFileName(resultatSecurite.CheminArchive))
                        cmd.Parameters.AddWithValue("@dossier", Path.GetDirectoryName(resultatSecurite.CheminArchive))
                        cmd.Parameters.AddWithValue("@taille", resultatSecurite.TailleOctets)
                        cmd.Parameters.AddWithValue("@nb_fichiers", resultatSecurite.NbFichiers)
                        cmd.Parameters.AddWithValue("@version_application", DbHelper.ValeurOuDBNull(GestionSauvegardes.LireVersionApplication()))
                        cmd.Parameters.AddWithValue("@login", DbHelper.ValeurOuDBNull(utilisateur?.LoginUtilisateur))
                        cmd.Parameters.AddWithValue("@machine", Environment.MachineName)
                        cmd.ExecuteNonQuery()
                    End Using
                End If

            End Using
        Catch ex As Exception
            GestionLog.EcrireLog("Restauration : journal des sauvegardes non mis à jour.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.Database, ex)
        End Try
    End Sub

    Private Sub SupprimerDossierSansErreur(dossier As String)
        Try
            If Directory.Exists(dossier) Then Directory.Delete(dossier, True)
        Catch ex As Exception
            GestionLog.EcrireLog($"Restauration : dossier provisoire non supprimé ({dossier}).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process, ex)
        End Try
    End Sub

#End Region

#Region "Journal"

    ' Ne lève pas d'exception : une erreur de journal ne doit pas masquer le résultat réel.
    Private Sub Journaliser(statut As StatutRestauration, info As InfoArchive, dateDebut As DateTime,
                            nbFichiers As Integer?, archiveSecurite As String, dossierPrecedent As String,
                            utilisateur As UtilisateurApplication, message As String)
        Try
            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryRestaurations.InsertJournal, conn)
                    cmd.Parameters.AddWithValue("@date_debut", dateDebut)
                    cmd.Parameters.AddWithValue("@statut", StatutsRestauration.VersCode(statut))
                    cmd.Parameters.AddWithValue("@nom_archive", DbHelper.ValeurOuDBNull(info.NomArchive))
                    cmd.Parameters.AddWithValue("@dossier_archive", DbHelper.ValeurOuDBNull(Path.GetDirectoryName(info.CheminArchive)))
                    cmd.Parameters.AddWithValue("@date_archive", If(info.DateSauvegarde.HasValue, CObj(info.DateSauvegarde.Value.ToLocalTime()), DBNull.Value))
                    cmd.Parameters.AddWithValue("@version_schema_archive", DbHelper.ValeurOuDBNull(info.VersionSchema))
                    cmd.Parameters.AddWithValue("@machine_archive", DbHelper.ValeurOuDBNull(info.Machine))
                    cmd.Parameters.AddWithValue("@nb_fichiers", If(nbFichiers.HasValue, CObj(nbFichiers.Value), DBNull.Value))
                    cmd.Parameters.AddWithValue("@archive_securite", DbHelper.ValeurOuDBNull(If(archiveSecurite Is Nothing, Nothing, Path.GetFileName(archiveSecurite))))
                    cmd.Parameters.AddWithValue("@dossier_precedent", DbHelper.ValeurOuDBNull(dossierPrecedent))
                    cmd.Parameters.AddWithValue("@login", DbHelper.ValeurOuDBNull(utilisateur?.LoginUtilisateur))
                    cmd.Parameters.AddWithValue("@machine", Environment.MachineName)
                    cmd.Parameters.AddWithValue("@version_application", DbHelper.ValeurOuDBNull(GestionSauvegardes.LireVersionApplication()))
                    cmd.Parameters.AddWithValue("@message", DbHelper.ValeurOuDBNull(message))
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            GestionLog.EcrireLog("Journal des restaurations : écriture impossible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.Database, ex)
        End Try
    End Sub

    ''' <summary>Dernières restaurations, de la plus récente à la plus ancienne.</summary>
    Public Function GetHistorique(Optional limite As Integer = 20) As List(Of RestaurationJournal)

        Dim resultat As New List(Of RestaurationJournal)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRestaurations.SelectHistorique, conn)
                cmd.Parameters.AddWithValue("@limite", Math.Max(limite, 1))
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        resultat.Add(New RestaurationJournal With {
                            .DateDebut = reader.GetDateTime("date_debut"),
                            .Statut = StatutsRestauration.DepuisCode(reader.GetString("statut_restauration")),
                            .NomArchive = DbHelper.LireString(reader, "nom_archive"),
                            .DateArchive = DbHelper.LireDateNullable(reader, "date_archive"),
                            .NbFichiers = If(reader.IsDBNull("nb_fichiers"), CType(Nothing, Integer?), Convert.ToInt32(reader("nb_fichiers"))),
                            .ArchiveSecurite = DbHelper.LireString(reader, "archive_securite"),
                            .DossierDonneesPrecedent = DbHelper.LireString(reader, "dossier_donnees_precedent"),
                            .LoginUtilisateur = DbHelper.LireString(reader, "login_utilisateur"),
                            .MessageErreur = DbHelper.LireString(reader, "message_erreur")
                        })
                    End While
                End Using
            End Using
        End Using

        Return resultat

    End Function

#End Region

End Module
